using Microsoft.Extensions.Logging;
using VueWebEnterpriseSQL.Application.DTOs;
using VueWebEnterpriseSQL.Application.Interfaces;

namespace VueWebEnterpriseSQL.Infrastructure.Messaging.Jobs
{
    public class ProcessBulkEmailJob
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<ProcessBulkEmailJob> _logger;

        public ProcessBulkEmailJob(IEmailService emailService, ILogger<ProcessBulkEmailJob> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }

        public async Task ExecuteAsync(List<EmailMessage> messages, bool sendReport, string? reportRecipient)
        {
            _logger.LogInformation("Processing bulk email job for {Count} recipients", messages.Count);
            await _emailService.SendBulkEmailAsync(messages, sendReport, reportRecipient);
        }
    }
}
