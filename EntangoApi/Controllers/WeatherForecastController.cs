using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using EntangoApi.Models;

namespace EntangoApi.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(IHttpClientFactory httpClientFactory, ILogger<WeatherForecastController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    [HttpGet("{location}")]
    public async Task<ActionResult<IEnumerable<WeatherForecastResponse>>> Get(string location, 
        [FromQuery] double latitude, 
        [FromQuery] double longitude)
    {
        string url = string.Empty;
        try
        {
            // Validate coordinates
            if (latitude < -90 || latitude > 90)
            {
                return BadRequest("Latitude must be between -90 and 90 degrees");
            }
            if (longitude < -180 || longitude > 180)
            {
                return BadRequest("Longitude must be between -180 and 180 degrees");
            }

            var client = _httpClientFactory.CreateClient();
            // Use InvariantCulture to ensure decimal point (.) is used instead of comma (,)
            url = string.Format(System.Globalization.CultureInfo.InvariantCulture, 
                "https://api.open-meteo.com/v1/forecast?latitude={0}&longitude={1}&hourly=temperature_2m,precipitation_probability",
                latitude, longitude);
            
            var response = await client.GetAsync(url);
            var responseBody = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error calling Open-Meteo API: {StatusCode} Url: {Url} ResponseBody: {Body}", (int)response.StatusCode, url, responseBody);
                return StatusCode((int)response.StatusCode, "Error retrieving weather forecast");
            }

            var forecast = JsonSerializer.Deserialize<OpenMeteoResponse>(responseBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (forecast == null)
            {
                _logger.LogError("Invalid response from weather service. Url: {Url} ResponseBody: {Body}", url, responseBody);
                return BadRequest("Invalid response from weather service");
            }

            // Return the full Open-Meteo response object so the client receives the complete
            // structure (latitude, longitude, hourly_units, hourly, etc.). This mirrors the
            // Open-Meteo API response shape.
            return Ok(forecast);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing weather forecast request. Url: {Url}", url);
            return StatusCode(500, "An error occurred while processing your request");
        }
    }
}