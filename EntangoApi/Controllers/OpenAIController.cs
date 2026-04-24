using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using EntangoApi.Models;

namespace EntangoApi.Controllers;

[ApiController]
[Route("[controller]")]
public class OpenAIController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<OpenAIController> _logger;

    public OpenAIController(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<OpenAIController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    [HttpPost("chat")]
    public async Task<ActionResult<ChatResponse>> Chat([FromBody] ChatRequest request)
    {
        var apiKey = _configuration["OpenAI:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            return Problem(detail: "OpenAI API key is not configured. Set OpenAI:ApiKey in appsettings or environment.", statusCode: 500);
        }

        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        var payload = new
        {
            model = request.ModelName ?? "gpt-3.5-turbo",
            messages = new[]
            {
                new { role = "system", content = request.SystemPrompt ?? "You are a helpful AI assistant." },
                new { role = "user", content = request.Message }
            },
            temperature = request.Temperature,
            max_tokens = 1000
        };

        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        try
        {
            using var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);
            var responseJson = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("OpenAI API returned {Status}: {Body}", (int)response.StatusCode, responseJson);
                return StatusCode((int)response.StatusCode, responseJson);
            }

            using var doc = JsonDocument.Parse(responseJson);
            var root = doc.RootElement;

            var message = root.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString() ?? string.Empty;
            var model = root.TryGetProperty("model", out var m) ? m.GetString() ?? string.Empty : string.Empty;
            var tokens = root.TryGetProperty("usage", out var u) && u.TryGetProperty("total_tokens", out var t) ? t.GetInt32() : 0;

            var chatResponse = new ChatResponse
            {
                Message = message,
                Model = model,
                TokensUsed = tokens
            };

            return Ok(chatResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling OpenAI API");
            return StatusCode(500, "An error occurred while contacting OpenAI API");
        }
    }
}