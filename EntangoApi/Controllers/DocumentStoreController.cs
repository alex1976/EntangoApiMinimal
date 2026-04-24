using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EntangoApi.Models;

namespace EntangoApi.Controllers;

[ApiController]
[Route("[controller]")]
public class DocumentStoreController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DocumentStoreController> _logger;

    public DocumentStoreController(ApplicationDbContext context, ILogger<DocumentStoreController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: /DocumentStore
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DocumentResponse>>> GetDocuments()
    {
        var documents = await _context.DocumentStores
            .Select(d => new DocumentResponse
            {
                DocumentId = d.DocumentId,
                DocumentDescription = d.DocumentDescription,
                DocumentType = d.DocumentType,
                DocumentCategory = d.DocumentCategory,
                DocumentExtension = d.DocumentExtension,
                DocumentSize = d.DocumentSize,
                DownloadUrl = $"/DocumentStore/{d.DocumentId}/download"
            })
            .ToListAsync();

        return documents;
    }

    // POST: /DocumentStore/search
    [HttpPost("search")]
    public async Task<ActionResult<IEnumerable<DocumentResponse>>> SearchDocuments([FromBody] DocumentSearchRequest filter)
    {
        if (filter == null)
        {
            return BadRequest("Filter is required");
        }

        IQueryable<DocumentStore> query = _context.DocumentStores;

        if (!string.IsNullOrWhiteSpace(filter.DescriptionContains))
        {
            var descriptionTerm = filter.DescriptionContains.Trim().ToLowerInvariant();
            query = query.Where(d => d.DocumentDescription.ToLower().Contains(descriptionTerm));
        }

        if (!string.IsNullOrWhiteSpace(filter.TypeContains))
        {
            var typeTerm = filter.TypeContains.Trim().ToLowerInvariant();
            query = query.Where(d => d.DocumentType != null && d.DocumentType.ToLower().Contains(typeTerm));
        }

        if (!string.IsNullOrWhiteSpace(filter.CategoryContains))
        {
            var categoryTerm = filter.CategoryContains.Trim().ToLowerInvariant();
            query = query.Where(d => d.DocumentCategory != null && d.DocumentCategory.ToLower().Contains(categoryTerm));
        }

        var documents = await query
            .Select(d => new DocumentResponse
            {
                DocumentId = d.DocumentId,
                DocumentDescription = d.DocumentDescription,
                DocumentType = d.DocumentType,
                DocumentCategory = d.DocumentCategory,
                DocumentExtension = d.DocumentExtension,
                DocumentSize = d.DocumentSize,
                DownloadUrl = $"/DocumentStore/{d.DocumentId}/download"
            })
            .ToListAsync();

        return Ok(documents);
    }

    // GET: /DocumentStore/5
    [HttpGet("{id}")]
    public async Task<ActionResult<DocumentResponse>> GetDocument(int id)
    {
        var document = await _context.DocumentStores.FindAsync(id);

        if (document == null)
        {
            return NotFound();
        }

        return new DocumentResponse
        {
            DocumentId = document.DocumentId,
            DocumentDescription = document.DocumentDescription,
            DocumentType = document.DocumentType,
            DocumentCategory = document.DocumentCategory,
            DocumentExtension = document.DocumentExtension,
            DocumentSize = document.DocumentSize,
            DownloadUrl = $"/DocumentStore/{document.DocumentId}/download"
        };
    }

    // GET: /DocumentStore/5/download
    [HttpGet("{id}/download")]
    public async Task<IActionResult> DownloadDocument(int id)
    {
        var document = await _context.DocumentStores.FindAsync(id);

        if (document == null)
        {
            return NotFound();
        }

        return File(document.DocumentAttachment, "application/octet-stream", $"{document.DocumentDescription}{document.DocumentExtension}");
    }

    // POST: /DocumentStore
    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<DocumentResponse>> CreateDocument([FromForm] CreateDocumentRequest request)
    {
        if (request.Document == null || request.Document.Length == 0)
        {
            return BadRequest("No file was uploaded.");
        }

        var extension = Path.GetExtension(request.Document.FileName);
        if (string.IsNullOrEmpty(extension))
        {
            return BadRequest("File must have an extension.");
        }

        using var memoryStream = new MemoryStream();
        await request.Document.CopyToAsync(memoryStream);

        var document = new DocumentStore
        {
            DocumentDescription = request.DocumentDescription,
            DocumentType = request.DocumentType,
            DocumentCategory = request.DocumentCategory,
            DocumentExtension = extension,
            DocumentSize = request.Document.Length,
            DocumentAttachment = memoryStream.ToArray()
        };

        _context.DocumentStores.Add(document);
        await _context.SaveChangesAsync();

        var response = new DocumentResponse
        {
            DocumentId = document.DocumentId,
            DocumentDescription = document.DocumentDescription,
            DocumentType = document.DocumentType,
            DocumentCategory = document.DocumentCategory,
            DocumentExtension = document.DocumentExtension,
            DocumentSize = document.DocumentSize,
            DownloadUrl = $"/DocumentStore/{document.DocumentId}/download"
        };

        return CreatedAtAction(nameof(GetDocument), new { id = document.DocumentId }, response);
    }

    // PUT: /DocumentStore/5
    [HttpPut("{id}")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UpdateDocument(int id, [FromForm] UpdateDocumentRequest request)
    {
        var document = await _context.DocumentStores.FindAsync(id);
        if (document == null)
        {
            return NotFound();
        }

        document.DocumentDescription = request.DocumentDescription;
        document.DocumentType = request.DocumentType;
        document.DocumentCategory = request.DocumentCategory;

        if (request.Document != null && request.Document.Length > 0)
        {
            var extension = Path.GetExtension(request.Document.FileName);
            if (string.IsNullOrEmpty(extension))
            {
                return BadRequest("File must have an extension.");
            }

            using var memoryStream = new MemoryStream();
            await request.Document.CopyToAsync(memoryStream);

            document.DocumentExtension = extension;
            document.DocumentSize = request.Document.Length;
            document.DocumentAttachment = memoryStream.ToArray();
        }

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException) when (!DocumentExists(id))
        {
            return NotFound();
        }

        return NoContent();
    }

    // DELETE: /DocumentStore/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDocument(int id)
    {
        var document = await _context.DocumentStores.FindAsync(id);
        if (document == null)
        {
            return NotFound();
        }

        _context.DocumentStores.Remove(document);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool DocumentExists(int id)
    {
        return _context.DocumentStores.Any(e => e.DocumentId == id);
    }
}