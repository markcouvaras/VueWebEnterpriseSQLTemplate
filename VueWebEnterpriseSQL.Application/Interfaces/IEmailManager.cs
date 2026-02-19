using VueWebEnterpriseSQL.Application.DTOs;

namespace VueWebEnterpriseSQL.Application.Interfaces
{
    public interface IEmailManager
    {
        string EnqueueSingleEmail(EmailMessage message);
        string EnqueueBulkEmail(IEnumerable<EmailMessage> messages, bool sendReport = false, string? reportRecipient = null);
    }
}
