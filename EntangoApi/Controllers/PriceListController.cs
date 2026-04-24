using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EntangoApi.Models;

namespace EntangoApi.Controllers;

[ApiController]
[Route("[controller]")]
public class PriceListController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PriceListController> _logger;

    public PriceListController(ApplicationDbContext context, ILogger<PriceListController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: /PriceList
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PriceList>>> GetPriceLists()
    {
        return await _context.PriceLists.ToListAsync();
    }

    // POST: /PriceList/search
    [HttpPost("search")]
    public async Task<ActionResult<IEnumerable<PriceList>>> Search([FromBody] Models.PriceListSearchRequest filter)
    {
        if (filter == null)
        {
            return BadRequest("Filter is required");
        }

        // Price range validation for between filter.
        if (filter.ProductPriceBetweenMin.HasValue && filter.ProductPriceBetweenMax.HasValue
            && filter.ProductPriceBetweenMin > filter.ProductPriceBetweenMax)
        {
            return BadRequest("ProductPriceBetweenMin cannot be greater than ProductPriceBetweenMax");
        }

        IQueryable<PriceList> query = _context.PriceLists;

        if (!string.IsNullOrWhiteSpace(filter.AuthorContains))
        {
            var authorTerm = filter.AuthorContains.Trim();
            var authorTermLower = authorTerm.ToLowerInvariant();
            // Search in PriceListAuthor using case-insensitive ToLower().Contains()
            query = query.Where(p =>
                p.PriceListAuthor != null && p.PriceListAuthor.ToLower().Contains(authorTermLower)
            );
        }

        if (!string.IsNullOrWhiteSpace(filter.DescriptionContains))
        {
            var term = filter.DescriptionContains.Trim();
            var termLower = term.ToLowerInvariant();
            // Search in ProductDescription OR ProductExtendedDescription using case-insensitive ToLower().Contains()
            query = query.Where(p =>
                (p.ProductDescription != null && p.ProductDescription.ToLower().Contains(termLower))
                || (p.ProductExtendedDescription != null && p.ProductExtendedDescription.ToLower().Contains(termLower))
            );
        }

        if (!string.IsNullOrWhiteSpace(filter.ClassificationEquals))
        {
            var cls = filter.ClassificationEquals.Trim();
            var clsLower = cls.ToLowerInvariant();
            query = query.Where(p => p.ProductClassification != null && p.ProductClassification.ToLower().Equals(clsLower));
        }

        // ProductPrice filters
        if (filter.ProductPriceEquals.HasValue)
        {
            var value = filter.ProductPriceEquals.Value;
            query = query.Where(p => p.ProductPrice.HasValue && p.ProductPrice.Value == value);
        }

        if (filter.ProductPriceGreaterThan.HasValue)
        {
            var minValue = filter.ProductPriceGreaterThan.Value;
            query = query.Where(p => p.ProductPrice.HasValue && p.ProductPrice.Value > minValue);
        }

        if (filter.ProductPriceLessThan.HasValue)
        {
            var maxValue = filter.ProductPriceLessThan.Value;
            query = query.Where(p => p.ProductPrice.HasValue && p.ProductPrice.Value < maxValue);
        }

        if (filter.ProductPriceBetweenMin.HasValue || filter.ProductPriceBetweenMax.HasValue)
        {
            var betweenMin = filter.ProductPriceBetweenMin ?? decimal.MinValue;
            var betweenMax = filter.ProductPriceBetweenMax ?? decimal.MaxValue;
            query = query.Where(p => p.ProductPrice.HasValue
                && p.ProductPrice.Value >= betweenMin
                && p.ProductPrice.Value <= betweenMax);
        }

        // Apply paging if provided
        if (filter.Page.HasValue && filter.PageSize.HasValue && filter.Page > 0 && filter.PageSize > 0)
        {
            var skip = (filter.Page.Value - 1) * filter.PageSize.Value;
            query = query.Skip(skip).Take(filter.PageSize.Value);
        }

        var results = await query.ToListAsync();
        return Ok(results);
    }

    // GET: /PriceList/5
    [HttpGet("{id}")]
    public async Task<ActionResult<PriceList>> GetPriceList(int id)
    {
        var priceList = await _context.PriceLists.FindAsync(id);

        if (priceList == null)
        {
            return NotFound();
        }

        return priceList;
    }

    // POST: /PriceList
    [HttpPost]
    public async Task<ActionResult<PriceList>> CreatePriceList(CreatePriceListRequest request)
    {
        var currentMaxId = await _context.PriceLists
            .Select(p => (int?)p.PriceListId)
            .MaxAsync() ?? 0;

        var priceList = new PriceList
        {
            PriceListId = currentMaxId + 1,
            PriceListDescription = request.PriceListDescription,
            PriceListAuthor = request.PriceListAuthor,
            PriceListVersion = request.PriceListVersion,
            ProductId = request.ProductId,
            ProductDescription = request.ProductDescription,
            ProductExtendedDescription = request.ProductExtendedDescription,
            ProductUM = request.ProductUM,
            ProductPrice = request.ProductPrice,
            ProductNetPrice = request.ProductNetPrice,
            ProductLabourPerc = request.ProductLabourPerc,
            ProductSafetyPerc = request.ProductSafetyPerc,
            ProductMaterialPerc = request.ProductMaterialPerc,
            ProductEquipmentPerc = request.ProductEquipmentPerc,
            ProductHirePerc = request.ProductHirePerc,
            ProductGeneralExpensePerc = request.ProductGeneralExpensePerc,
            ProductBusinessProfitPerc = request.ProductBusinessProfitPerc,
            ProductClassification = request.ProductClassification
        };

        _context.PriceLists.Add(priceList);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetPriceList), new { id = priceList.PriceListId }, priceList);
    }

    // PUT: /PriceList/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePriceList(int id, UpdatePriceListRequest request)
    {
        if (id != request.PriceListId)
        {
            return BadRequest();
        }

        var priceList = await _context.PriceLists.FindAsync(id);
        if (priceList == null)
        {
            return NotFound();
        }

        priceList.PriceListDescription = request.PriceListDescription;
        priceList.PriceListAuthor = request.PriceListAuthor;
        priceList.PriceListVersion = request.PriceListVersion;
        priceList.ProductId = request.ProductId;
        priceList.ProductDescription = request.ProductDescription;
        priceList.ProductExtendedDescription = request.ProductExtendedDescription;
        priceList.ProductUM = request.ProductUM;
        priceList.ProductPrice = request.ProductPrice;
        priceList.ProductNetPrice = request.ProductNetPrice;
        priceList.ProductLabourPerc = request.ProductLabourPerc;
        priceList.ProductSafetyPerc = request.ProductSafetyPerc;
        priceList.ProductMaterialPerc = request.ProductMaterialPerc;
        priceList.ProductEquipmentPerc = request.ProductEquipmentPerc;
        priceList.ProductHirePerc = request.ProductHirePerc;
        priceList.ProductGeneralExpensePerc = request.ProductGeneralExpensePerc;
        priceList.ProductBusinessProfitPerc = request.ProductBusinessProfitPerc;
        priceList.ProductClassification = request.ProductClassification;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException) when (!PriceListExists(id))
        {
            return NotFound();
        }

        return NoContent();
    }

    // DELETE: /PriceList/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePriceList(int id)
    {
        var priceList = await _context.PriceLists.FindAsync(id);
        if (priceList == null)
        {
            return NotFound();
        }

        _context.PriceLists.Remove(priceList);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool PriceListExists(int id)
    {
        return _context.PriceLists.Any(e => e.PriceListId == id);
    }
}