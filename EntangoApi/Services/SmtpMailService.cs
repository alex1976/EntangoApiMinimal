using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace EntangoApi.Services;

public interface IMailService
{
    Task SendMailAsync(Models.MailRequest mailRequest);
}

public class SmtpMailService : IMailService
{
    private readonly SmtpSettings _settings;
    private readonly ILogger<SmtpMailService> _logger;

    public SmtpMailService(IOptions<SmtpSettings> settings, ILogger<SmtpMailService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task SendMailAsync(Models.MailRequest mailRequest)
    {
        try
        {
            using var mailMessage = new MailMessage
            {
                From = new MailAddress(_settings.SenderEmail, _settings.SenderName),
                Subject = mailRequest.Subject,
                Body = mailRequest.Body,
                IsBodyHtml = true,
                Priority = (MailPriority)mailRequest.Priority
            };

            foreach (var recipient in mailRequest.Recipients)
            {
                mailMessage.To.Add(recipient);
            }

            using var smtpClient = new SmtpClient(_settings.Server, _settings.Port)
            {
                EnableSsl = _settings.EnableSsl,
                Credentials = new System.Net.NetworkCredential(_settings.Username, _settings.Password)
            };

            await smtpClient.SendMailAsync(mailMessage);
            _logger.LogInformation("Email sent successfully to {Recipients}", string.Join(", ", mailRequest.Recipients));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email to {Recipients}", string.Join(", ", mailRequest.Recipients));
            throw;
        }
    }
}

public class SmtpSettings
{
    public string Server { get; set; } = string.Empty;
    public int Port { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool EnableSsl { get; set; }
    public string SenderEmail { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
}