using System.Threading.Tasks;
using EntangoApi.Models;

namespace EntangoApi.Services
{
    public interface ISmsService
    {
        /// <summary>
        /// Send an SMS message to one or more recipients
        /// </summary>
        Task SendSmsAsync(SmsRequest request);
    }
}
