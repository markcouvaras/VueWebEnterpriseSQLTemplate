using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VueWebEnterpriseSQL.Application.DTOs;
using VueWebEnterpriseSQL.Application.Interfaces;
using VueWebEnterpriseSQL.Infrastructure.Messaging.Jobs;

namespace VueWebEnterpriseSQL.Infrastructure.Messaging
{
    public class EmailManager : IEmailManager
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<EmailManager> _logger;

        public EmailManager(IServiceProvider serviceProvider, ILogger<EmailManager> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public string EnqueueSingleEmail(EmailMessage message)
        {
            var jobClient = _serviceProvider.GetService<IBackgroundJobClient>();
            if (jobClient is null)
            {
                _logger.LogWarning("Hangfire is not configured. Email will not be sent. Run 'docker-compose up -d' to enable background jobs.");
                return string.Empty;
            }

            return jobClient.Enqueue<ProcessSingleEmailJob>(job => job.ExecuteAsync(message));
        }

        public string EnqueueBulkEmail(IEnumerable<EmailMessage> messages, bool sendReport = false, string? reportRecipient = null)
        {
            var jobClient = _serviceProvider.GetService<IBackgroundJobClient>();
            if (jobClient is null)
            {
                _logger.LogWarning("Hangfire is not configured. Emails will not be sent. Run 'docker-compose up -d' to enable background jobs.");
                return string.Empty;
            }

            var messageList = messages.ToList();
            return jobClient.Enqueue<ProcessBulkEmailJob>(job => job.ExecuteAsync(messageList, sendReport, reportRecipient));
        }
    }
}
