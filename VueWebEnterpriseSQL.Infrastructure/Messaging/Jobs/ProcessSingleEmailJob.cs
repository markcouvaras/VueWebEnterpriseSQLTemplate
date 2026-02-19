using Microsoft.Extensions.Logging;
using VueWebEnterpriseSQL.Application.DTOs;
using VueWebEnterpriseSQL.Application.Interfaces;

namespace VueWebEnterpriseSQL.Infrastructure.Messaging.Jobs
{
    public class ProcessSingleEmailJob
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<ProcessSingleEmailJob> _logger;

        public ProcessSingleEmailJob(IEmailService emailService, ILogger<ProcessSingleEmailJob> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }

        public async Task ExecuteAsync(EmailMessage message)
        {
            _logger.LogInformation("Processing single email job for {Recipient}", message.To);
            await _emailService.SendEmailAsync(message);
        }
    }
}
