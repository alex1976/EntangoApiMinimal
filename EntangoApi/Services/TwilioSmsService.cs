using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using Microsoft.Extensions.Options;
using EntangoApi.Models;

namespace EntangoApi.Services;

/// <summary>
/// Minimal Twilio implementation using HttpClient so no external SDK is required.
/// If SmsSettings.Provider is not "Twilio" this implementation will still be registered
/// but will throw if required config is missing.
/// </summary>
public class TwilioSmsService : ISmsService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly SmsSettings _settings;
    private readonly ILogger<TwilioSmsService> _logger;

    public TwilioSmsService(IHttpClientFactory httpClientFactory, IOptions<SmsSettings> settings, ILogger<TwilioSmsService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task SendSmsAsync(SmsRequest request)
    {
        if (request.Recipients == null || request.Recipients.Count == 0)
            throw new ArgumentException("At least one recipient is required", nameof(request));

        if (string.IsNullOrWhiteSpace(request.Message))
            throw new ArgumentException("Message is required", nameof(request));

        if (!string.Equals(_settings.Provider, "Twilio", StringComparison.OrdinalIgnoreCase))
        {
            // Provider not set to Twilio - no-op to avoid unexpected charges. Log and return.
            _logger.LogWarning("SMS provider is not set to Twilio. Sms was not sent. Provider={Provider}", _settings.Provider);
            return;
        }

        if (string.IsNullOrWhiteSpace(_settings.AccountSid) || string.IsNullOrWhiteSpace(_settings.AuthToken))
            throw new InvalidOperationException("Twilio AccountSid and AuthToken must be configured when Provider is Twilio.");

        var httpClient = _httpClientFactory.CreateClient("twilio");

        // Twilio base url
        var baseUrl = string.IsNullOrWhiteSpace(_settings.BaseUrl) ? "https://api.twilio.com" : _settings.BaseUrl.TrimEnd('/');

        // Use basic auth (AccountSid:AuthToken)
        var auth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_settings.AccountSid}:{_settings.AuthToken}"));
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", auth);

        var from = request.From ?? _settings.FromNumber;
        if (string.IsNullOrWhiteSpace(from))
            throw new InvalidOperationException("From number must be set either in SmsSettings.FromNumber or in the request.From");

        foreach (var to in request.Recipients)
        {
            var url = $"{baseUrl}/2010-04-01/Accounts/{_settings.AccountSid}/Messages.json";

            var form = new Dictionary<string, string>
            {
                ["From"] = from!,
                ["To"] = to,
                ["Body"] = request.Message
            };

            using var content = new FormUrlEncodedContent(form);

            var response = await httpClient.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed to send SMS to {To}. Status: {Status}. Response: {Response}", to, response.StatusCode, body);
                throw new InvalidOperationException($"Failed to send SMS to {to}. Status: {response.StatusCode}");
            }

            _logger.LogInformation("SMS sent to {To} via Twilio (From: {From})", to, from);
        }
    }
}
