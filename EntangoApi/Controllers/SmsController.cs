using Microsoft.AspNetCore.Mvc;
using EntangoApi.Models;
using EntangoApi.Services;

namespace EntangoApi.Controllers;

[ApiController]
[Route("[controller]")]
public class SmsController : ControllerBase
{
    private readonly ISmsService _smsService;
    private readonly ILogger<SmsController> _logger;

    public SmsController(ISmsService smsService, ILogger<SmsController> logger)
    {
        _smsService = smsService;
        _logger = logger;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendSms([FromBody] SmsRequest request)
    {
        try
        {
            if (request.Recipients == null || request.Recipients.Count == 0)
                return BadRequest("At least one recipient is required");

            if (string.IsNullOrWhiteSpace(request.Message))
                return BadRequest("Message is required");

            // Basic validation for phone number format (very permissive). Caller should use E.164.
            foreach (var r in request.Recipients)
            {
                if (string.IsNullOrWhiteSpace(r))
                    return BadRequest("Recipient phone number cannot be empty");
            }

            await _smsService.SendSmsAsync(request);
            return Ok(new { message = "SMS processing started (sent or queued)" });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validation error sending SMS");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending SMS to {Recipients}", string.Join(", ", request.Recipients));
            return StatusCode(500, "An error occurred while sending the SMS");
        }
    }
}
