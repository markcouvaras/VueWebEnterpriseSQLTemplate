using VueWebEnterpriseSQL.Application.DTOs;

namespace VueWebEnterpriseSQL.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailMessage message);
        Task SendBulkEmailAsync(IEnumerable<EmailMessage> messages, bool sendReport = false, string? reportRecipient = null);
    }
}
