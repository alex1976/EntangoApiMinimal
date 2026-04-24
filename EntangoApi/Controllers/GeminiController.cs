using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using EntangoApi.Models;
using Microsoft.AspNetCore.Http;

namespace EntangoApi.Controllers;

[ApiController]
[Route("[controller]")]
public class GeminiController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<GeminiController> _logger;

    public GeminiController(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<GeminiController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    [HttpPost("docextract")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> DocExtract([FromForm] DocExtractRequest request)
    {
        var apiKey = _configuration["Gemini:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            return Problem(detail: "Gemini API key is not configured. Set Gemini:ApiKey in appsettings or environment.", statusCode: 500);
        }
        if (request.Pdf == null || request.Pdf.Length == 0)
        {
            return BadRequest("PDF file is required in form-data under 'Pdf'");
        }

        if (string.IsNullOrWhiteSpace(request.SchemaJson))
        {
            return BadRequest("SchemaJson form field is required and must contain the JSON schema to populate.");
        }

        // Read PDF bytes and convert to base64 (note: large files may fail depending on API limits)
        string pdfBase64;
        using (var ms = new MemoryStream())
        {
            await request.Pdf.CopyToAsync(ms);
            pdfBase64 = Convert.ToBase64String(ms.ToArray());
        }

        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("x-goog-api-key", apiKey);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var endpoint = $"https://generativelanguage.googleapis.com/v1/models/{request.Model}:generateContent";

        // Build an instruction that asks Gemini to extract according to the provided schema.
        var instruction = new StringBuilder();
        instruction.AppendLine("You are a JSON extraction assistant.");
        instruction.AppendLine("Task: extract structured data from the provided PDF and return only valid JSON that matches the provided schema.");
        instruction.AppendLine("Important: Do not include any additional commentary or explanation. Return strictly the JSON object.");
        instruction.AppendLine();
        instruction.AppendLine("Schema:");
        instruction.AppendLine(request.SchemaJson);
        instruction.AppendLine();
        instruction.AppendLine("The PDF content is provided as a Base64-encoded string in the next part. Decode and extract text content from the PDF, then populate the JSON according to the schema. If a field is not present, set it to null or an empty string as appropriate.");
        instruction.AppendLine();
        instruction.AppendLine("PDF_BASE64:");
        instruction.AppendLine(pdfBase64);

        var payload = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = instruction.ToString() }
                    }
                }
            },
            generationConfig = new
            {
                temperature = request.Temperature,
                maxOutputTokens = request.MaxOutputTokens
            }
        };

        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        try
        {
            using var response = await client.PostAsync(endpoint, content);
            var responseJson = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Gemini API returned {Status}: {Body}", (int)response.StatusCode, responseJson);
                return StatusCode((int)response.StatusCode, responseJson);
            }

            using var doc = JsonDocument.Parse(responseJson);
            var root = doc.RootElement;

            var message = root
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString() ?? string.Empty;

            // The assistant is instructed to return pure JSON. Try to parse the message as JSON.
            try
            {
                using var parsed = JsonDocument.Parse(message);
                // Return the parsed JSON as the response body
                return new ContentResult
                {
                    Content = parsed.RootElement.GetRawText(),
                    ContentType = "application/json",
                    StatusCode = 200
                };
            }
            catch (JsonException)
            {
                // If parsing fails, return the raw text with 422 Unprocessable Entity
                _logger.LogWarning("Gemini returned non-JSON output for PDF extraction. Returning raw text.");
                return UnprocessableEntity(new { raw = message });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling Gemini API for PDF extraction");
            return StatusCode(500, "An error occurred while contacting Gemini API");
        }
    }

    [HttpPost("chat")]
    public async Task<ActionResult<GeminiResponse>> Chat([FromBody] GeminiRequest request)
    {
        var apiKey = _configuration["Gemini:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            return Problem(detail: "Gemini API key is not configured. Set Gemini:ApiKey in appsettings or environment.", statusCode: 500);
        }

        var client = _httpClientFactory.CreateClient();

        // Aggiungi la chiave API all'header specifico
        client.DefaultRequestHeaders.Add("x-goog-api-key", apiKey);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var endpoint = $"https://generativelanguage.googleapis.com/v1/models/{request.Model}:generateContent";

        var payload = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = request.Message }
                    }
                }
            },
            generationConfig = new
            {
                temperature = request.Temperature,
                maxOutputTokens = request.MaxOutputTokens,
            }
        };

        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        try
        {
            using var response = await client.PostAsync(endpoint, content);
            var responseJson = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Gemini API returned {Status}: {Body}", (int)response.StatusCode, responseJson);
                return StatusCode((int)response.StatusCode, responseJson);
            }

            using var doc = JsonDocument.Parse(responseJson);
            var root = doc.RootElement;

            var message = root
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString() ?? string.Empty;

            var geminiResponse = new GeminiResponse
            {
                Message = message,
                Model = request.Model
            };

            return Ok(geminiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling Gemini API");
            return StatusCode(500, "An error occurred while contacting Gemini API");
        }
    }
}