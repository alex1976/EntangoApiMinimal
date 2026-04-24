using System.ComponentModel;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using ModelContextProtocol.Server;

namespace PriceListMcpServer.Tools;

[McpServerToolType]
public sealed class PriceListTools
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public PriceListTools(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    [McpServerTool, Description("Get all items from the PriceList API.")]
    public async Task<string> GetAllPriceListItems(CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient();
        var response = await client.GetAsync(BuildUrl("PriceList"), cancellationToken);

        return await FormatResponse(response, cancellationToken);
    }

    [McpServerTool, Description("Get a single PriceList item by id.")]
    public async Task<string> GetPriceListItemById(
        [Description("PriceList id.")] int id,
        CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient();
        var response = await client.GetAsync(BuildUrl($"PriceList/{id}"), cancellationToken);

        return await FormatResponse(response, cancellationToken);
    }

    [McpServerTool, Description("Search PriceList items using optional filters.")]
    public async Task<string> SearchPriceListItems(
        [Description("Optional filter for author contains.")] string? authorContains,
        [Description("Optional filter for product description contains.")] string? descriptionContains,
        [Description("Optional case-insensitive equality filter for product classification.")] string? classificationEquals,
        [Description("Optional page number (1-based).") ] int? page,
        [Description("Optional page size.")] int? pageSize,
        CancellationToken cancellationToken)
    {
        var payload = new
        {
            authorContains,
            descriptionContains,
            classificationEquals,
            page,
            pageSize
        };

        var client = _httpClientFactory.CreateClient();
        var response = await client.PostAsJsonAsync(BuildUrl("PriceList/search"), payload, cancellationToken);

        return await FormatResponse(response, cancellationToken);
    }

    [McpServerTool, Description("Insert a new item into the PriceList API using POST /PriceList.")]
    public async Task<string> InsertPriceListItem(
        [Description("Product id (required).") ] string productId,
        [Description("Product description (required).") ] string productDescription,
        [Description("Optional price list description.")] string? priceListDescription,
        [Description("Optional price list author.")] string? priceListAuthor,
        [Description("Optional price list version.")] string? priceListVersion,
        [Description("Optional product extended description.")] string? productExtendedDescription,
        [Description("Optional product unit of measure.")] string? productUM,
        [Description("Optional product price.")] decimal? productPrice,
        [Description("Optional product net price.")] decimal? productNetPrice,
        [Description("Optional labour percentage (0-100).") ] decimal? productLabourPerc,
        [Description("Optional safety percentage (0-100).") ] decimal? productSafetyPerc,
        [Description("Optional material percentage (0-100).") ] decimal? productMaterialPerc,
        [Description("Optional equipment percentage (0-100).") ] decimal? productEquipmentPerc,
        [Description("Optional hire percentage (0-100).") ] decimal? productHirePerc,
        [Description("Optional general expense percentage (0-100).") ] decimal? productGeneralExpensePerc,
        [Description("Optional business profit percentage (0-100).") ] decimal? productBusinessProfitPerc,
        [Description("Optional product classification.")] string? productClassification,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(productId))
        {
            return JsonSerializer.Serialize(new
            {
                ok = false,
                statusCode = 400,
                error = "productId is required."
            });
        }

        if (string.IsNullOrWhiteSpace(productDescription))
        {
            return JsonSerializer.Serialize(new
            {
                ok = false,
                statusCode = 400,
                error = "productDescription is required."
            });
        }

        var payload = new
        {
            priceListDescription,
            priceListAuthor,
            priceListVersion,
            productId,
            productDescription,
            productExtendedDescription,
            productUM,
            productPrice,
            productNetPrice,
            productLabourPerc,
            productSafetyPerc,
            productMaterialPerc,
            productEquipmentPerc,
            productHirePerc,
            productGeneralExpensePerc,
            productBusinessProfitPerc,
            productClassification
        };

        var client = _httpClientFactory.CreateClient();
        var response = await client.PostAsJsonAsync(BuildUrl("PriceList"), payload, cancellationToken);

        return await FormatResponse(response, cancellationToken);
    }

    [McpServerTool, Description("Update an existing item in PriceList API using PUT /PriceList/{id}.")]
    public async Task<string> UpdatePriceListItem(
        [Description("PriceList id to update (required).") ] int id,
        [Description("Product id (required).") ] string productId,
        [Description("Product description (required).") ] string productDescription,
        [Description("Optional price list description.")] string? priceListDescription,
        [Description("Optional price list author.")] string? priceListAuthor,
        [Description("Optional price list version.")] string? priceListVersion,
        [Description("Optional product extended description.")] string? productExtendedDescription,
        [Description("Optional product unit of measure.")] string? productUM,
        [Description("Optional product price.")] decimal? productPrice,
        [Description("Optional product net price.")] decimal? productNetPrice,
        [Description("Optional labour percentage (0-100).") ] decimal? productLabourPerc,
        [Description("Optional safety percentage (0-100).") ] decimal? productSafetyPerc,
        [Description("Optional material percentage (0-100).") ] decimal? productMaterialPerc,
        [Description("Optional equipment percentage (0-100).") ] decimal? productEquipmentPerc,
        [Description("Optional hire percentage (0-100).") ] decimal? productHirePerc,
        [Description("Optional general expense percentage (0-100).") ] decimal? productGeneralExpensePerc,
        [Description("Optional business profit percentage (0-100).") ] decimal? productBusinessProfitPerc,
        [Description("Optional product classification.")] string? productClassification,
        CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            return JsonSerializer.Serialize(new
            {
                ok = false,
                statusCode = 400,
                error = "id must be greater than 0."
            });
        }

        if (string.IsNullOrWhiteSpace(productId))
        {
            return JsonSerializer.Serialize(new
            {
                ok = false,
                statusCode = 400,
                error = "productId is required."
            });
        }

        if (string.IsNullOrWhiteSpace(productDescription))
        {
            return JsonSerializer.Serialize(new
            {
                ok = false,
                statusCode = 400,
                error = "productDescription is required."
            });
        }

        var payload = new
        {
            priceListId = id,
            priceListDescription,
            priceListAuthor,
            priceListVersion,
            productId,
            productDescription,
            productExtendedDescription,
            productUM,
            productPrice,
            productNetPrice,
            productLabourPerc,
            productSafetyPerc,
            productMaterialPerc,
            productEquipmentPerc,
            productHirePerc,
            productGeneralExpensePerc,
            productBusinessProfitPerc,
            productClassification
        };

        var client = _httpClientFactory.CreateClient();
        var response = await client.PutAsJsonAsync(BuildUrl($"PriceList/{id}"), payload, cancellationToken);

        return await FormatResponse(response, cancellationToken);
    }

    [McpServerTool, Description("Delete an item from PriceList API using DELETE /PriceList/{id}.")]
    public async Task<string> DeletePriceListItem(
        [Description("PriceList id to delete (required).") ] int id,
        CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            return JsonSerializer.Serialize(new
            {
                ok = false,
                statusCode = 400,
                error = "id must be greater than 0."
            });
        }

        var client = _httpClientFactory.CreateClient();
        var response = await client.DeleteAsync(BuildUrl($"PriceList/{id}"), cancellationToken);

        return await FormatResponse(response, cancellationToken);
    }

    [McpServerTool, Description("Search documents by description and/or type and/or category.")]
    public async Task<string> SearchDocuments(
        [Description("Optional filter for document description contains.")] string? descriptionContains,
        [Description("Optional filter for document type contains.")] string? typeContains,
        [Description("Optional filter for document category contains.")] string? categoryContains,
        CancellationToken cancellationToken)
    {
        var payload = new
        {
            descriptionContains,
            typeContains,
            categoryContains
        };

        var client = _httpClientFactory.CreateClient();
        var response = await client.PostAsJsonAsync(BuildUrl("DocumentStore/search"), payload, cancellationToken);

        return await FormatResponse(response, cancellationToken);
    }

    [McpServerTool, Description("Download a document by id from DocumentStore and return metadata with Base64 content.")]
    public async Task<string> DownloadDocumentById(
        [Description("Document id.")] int id,
        CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient();
        using var response = await client.GetAsync(
            BuildUrl($"DocumentStore/{id}/download"),
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return await FormatResponse(response, cancellationToken);
        }

        var bytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
        var contentType = response.Content.Headers.ContentType?.ToString() ?? "application/octet-stream";
        var fileName = response.Content.Headers.ContentDisposition?.FileNameStar
            ?? response.Content.Headers.ContentDisposition?.FileName;

        if (!string.IsNullOrWhiteSpace(fileName))
        {
            fileName = fileName.Trim('"');
        }

        return JsonSerializer.Serialize(new
        {
            ok = true,
            statusCode = (int)response.StatusCode,
            fileName,
            contentType,
            sizeBytes = bytes.Length,
            dataBase64 = Convert.ToBase64String(bytes)
        });
    }

    [McpServerTool, Description("Upload a document to DocumentStore with required description and file path.")]
    public async Task<string> UploadDocument(
        [Description("Document description (required).") ] string description,
        [Description("Absolute or relative file path to upload (required).") ] string filePath,
        [Description("Optional document type.")] string? documentType,
        [Description("Optional document category.")] string? documentCategory,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return JsonSerializer.Serialize(new
            {
                ok = false,
                statusCode = 400,
                error = "Description is required."
            });
        }

        if (string.IsNullOrWhiteSpace(filePath))
        {
            return JsonSerializer.Serialize(new
            {
                ok = false,
                statusCode = 400,
                error = "File path is required."
            });
        }

        var resolvedFilePath = Path.GetFullPath(filePath);
        if (!File.Exists(resolvedFilePath))
        {
            return JsonSerializer.Serialize(new
            {
                ok = false,
                statusCode = 400,
                error = $"File not found: {resolvedFilePath}"
            });
        }

        var client = _httpClientFactory.CreateClient();
        await using var fileStream = File.OpenRead(resolvedFilePath);

        using var formData = new MultipartFormDataContent();
        formData.Add(new StringContent(description), "DocumentDescription");

        if (!string.IsNullOrWhiteSpace(documentType))
        {
            formData.Add(new StringContent(documentType), "DocumentType");
        }

        if (!string.IsNullOrWhiteSpace(documentCategory))
        {
            formData.Add(new StringContent(documentCategory), "DocumentCategory");
        }

        var fileContent = new StreamContent(fileStream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        formData.Add(fileContent, "Document", Path.GetFileName(resolvedFilePath));

        using var response = await client.PostAsync(BuildUrl("DocumentStore"), formData, cancellationToken);
        return await FormatResponse(response, cancellationToken);
    }

    private string BuildUrl(string relativePath)
    {
        var baseUrl = Environment.GetEnvironmentVariable("ENTANGO_API_BASE_URL")
            ?? Environment.GetEnvironmentVariable("PRICE_LIST_API_BASE_URL")
            ?? _configuration["EntangoApi:BaseUrl"]
            ?? _configuration["PriceListApi:BaseUrl"]
            ?? "http://localhost:5162";

        return $"{baseUrl.TrimEnd('/')}/{relativePath.TrimStart('/')}";
    }

    private static async Task<string> FormatResponse(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return JsonSerializer.Serialize(new
            {
                ok = false,
                statusCode = (int)response.StatusCode,
                error = body
            });
        }

        if (string.IsNullOrWhiteSpace(body))
        {
            return JsonSerializer.Serialize(new
            {
                ok = true,
                statusCode = (int)response.StatusCode,
                data = ""
            });
        }

        try
        {
            using var json = JsonDocument.Parse(body);
            return JsonSerializer.Serialize(new
            {
                ok = true,
                statusCode = (int)response.StatusCode,
                data = json.RootElement
            });
        }
        catch (JsonException)
        {
            return JsonSerializer.Serialize(new
            {
                ok = true,
                statusCode = (int)response.StatusCode,
                data = body
            });
        }
    }
}
