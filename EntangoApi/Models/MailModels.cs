namespace EntangoApi.Models;

public class MailRequest
{
    public List<string> Recipients { get; set; } = new();
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public MailPriority Priority { get; set; } = MailPriority.Normal;
}

public enum MailPriority
{
    Low = 0,
    Normal = 1,
    High = 2
}