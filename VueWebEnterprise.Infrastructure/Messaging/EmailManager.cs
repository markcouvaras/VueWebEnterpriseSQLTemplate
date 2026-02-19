using Hangfire;
using VueWebEnterprise.Application.DTOs;
using VueWebEnterprise.Application.Interfaces;
using VueWebEnterprise.Infrastructure.Messaging.Jobs;

namespace VueWebEnterprise.Infrastructure.Messaging
{
    public class EmailManager : IEmailManager
    {
        private readonly IBackgroundJobClient _jobClient;

        public EmailManager(IBackgroundJobClient jobClient)
        {
            _jobClient = jobClient;
        }

        public string EnqueueSingleEmail(EmailMessage message)
        {
            return _jobClient.Enqueue<ProcessSingleEmailJob>(job => job.ExecuteAsync(message));
        }

        public string EnqueueBulkEmail(IEnumerable<EmailMessage> messages, bool sendReport = false, string? reportRecipient = null)
        {
            var messageList = messages.ToList();
            return _jobClient.Enqueue<ProcessBulkEmailJob>(job => job.ExecuteAsync(messageList, sendReport, reportRecipient));
        }
    }
}
