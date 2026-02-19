using VueWebEnterprise.Application.DTOs;

namespace VueWebEnterprise.Application.Interfaces
{
    public interface IEmailManager
    {
        string EnqueueSingleEmail(EmailMessage message);
        string EnqueueBulkEmail(IEnumerable<EmailMessage> messages, bool sendReport = false, string? reportRecipient = null);
    }
}
