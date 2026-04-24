using System.Collections.Generic;

namespace EntangoApi.Models
{
    public class SmsRequest
    {
        /// <summary>
        /// List of destination phone numbers (ideally in E.164 format)
        /// </summary>
        public List<string> Recipients { get; set; } = new List<string>();

        /// <summary>
        /// The text message to send
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Optional: override the configured sender (From). If empty, SmsSettings.FromNumber is used.
        /// </summary>
        public string? From { get; set; }
    }

    public class SmsSettings
    {
        /// <summary>
        /// Provider type, e.g. "Twilio". If empty or unknown a no-op implementation is used.
        /// </summary>
        public string Provider { get; set; } = "";

        // Twilio specific settings
        public string? AccountSid { get; set; }
        public string? AuthToken { get; set; }
        public string? FromNumber { get; set; }

        /// <summary>
        /// Optional base URL for provider API. Twilio default is used if empty.
        /// </summary>
        public string? BaseUrl { get; set; }
    }
}
