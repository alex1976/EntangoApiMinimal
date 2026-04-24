using Microsoft.AspNetCore.Mvc;
using EntangoApi.Models;
using EntangoApi.Services;

namespace EntangoApi.Controllers;

[ApiController]
[Route("[controller]")]
public class MailController : ControllerBase
{
    private readonly IMailService _mailService;
    private readonly ILogger<MailController> _logger;

    public MailController(IMailService mailService, ILogger<MailController> logger)
    {
        _mailService = mailService;
        _logger = logger;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendMail([FromBody] MailRequest request)
    {
        try
        {
            if (request.Recipients.Count == 0)
            {
                return BadRequest("At least one recipient is required");
            }

            if (string.IsNullOrWhiteSpace(request.Subject))
            {
                return BadRequest("Subject is required");
            }

            if (string.IsNullOrWhiteSpace(request.Body))
            {
                return BadRequest("Body is required");
            }

            // Validate email addresses
            foreach (var recipient in request.Recipients)
            {
                if (!IsValidEmail(recipient))
                {
                    return BadRequest($"Invalid email address: {recipient}");
                }
            }

            await _mailService.SendMailAsync(request);
            return Ok(new { message = "Email sent successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email to {Recipients}", string.Join(", ", request.Recipients));
            return StatusCode(500, "An error occurred while sending the email");
        }
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}