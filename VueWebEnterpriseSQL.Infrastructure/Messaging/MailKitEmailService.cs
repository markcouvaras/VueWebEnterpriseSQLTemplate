using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using VueWebEnterpriseSQL.Application.DTOs;
using VueWebEnterpriseSQL.Application.Interfaces;

namespace VueWebEnterpriseSQL.Infrastructure.Messaging
{
    public class MailKitEmailService : IEmailService
    {
        private readonly MailSettings _settings;
        private readonly ILogger<MailKitEmailService> _logger;

        public MailKitEmailService(IOptions<MailSettings> settings, ILogger<MailKitEmailService> logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(EmailMessage message)
        {
            var mimeMessage = BuildMimeMessage(message);

            using var client = new SmtpClient();
            await ConnectAndAuthenticateAsync(client);
            await client.SendAsync(mimeMessage);
            await client.DisconnectAsync(quit: true);

            _logger.LogInformation("Email sent to {Recipient}", message.To);
        }

        public async Task SendBulkEmailAsync(IEnumerable<EmailMessage> messages, bool sendReport = false, string? reportRecipient = null)
        {
            var messageList = messages.ToList();
            int successCount = 0;
            int failureCount = 0;

            using var client = new SmtpClient();
            await ConnectAndAuthenticateAsync(client);

            foreach (var message in messageList)
            {
                try
                {
                    var mimeMessage = BuildMimeMessage(message);
                    await client.SendAsync(mimeMessage);
                    successCount++;
                }
                catch (Exception ex)
                {
                    failureCount++;
                    _logger.LogError(ex, "Failed to send email to {Recipient}", message.To);
                }
            }

            await client.DisconnectAsync(quit: true);

            _logger.LogInformation("Bulk email complete: {Success} sent, {Failed} failed out of {Total}",
                successCount, failureCount, messageList.Count);

            if (sendReport && !string.IsNullOrEmpty(reportRecipient))
            {
                await SendReportAsync(reportRecipient, successCount, failureCount, messageList.Count);
            }
        }

        private MimeMessage BuildMimeMessage(EmailMessage message)
        {
            var mime = new MimeMessage();
            mime.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
            mime.To.Add(MailboxAddress.Parse(message.To));
            mime.Subject = message.Subject;

            var builder = new BodyBuilder { HtmlBody = message.HtmlBody };

            foreach (var attachment in message.Attachments)
            {
                builder.Attachments.Add(attachment.FileName, attachment.Content, ContentType.Parse(attachment.ContentType));
            }

            mime.Body = builder.ToMessageBody();
            return mime;
        }

        private async Task ConnectAndAuthenticateAsync(SmtpClient client)
        {
            await client.ConnectAsync(_settings.Host, _settings.Port, MailKit.Security.SecureSocketOptions.StartTls);

            if (!string.IsNullOrEmpty(_settings.Username))
            {
                await client.AuthenticateAsync(_settings.Username, _settings.Password);
            }
        }

        private async Task SendReportAsync(string recipient, int success, int failure, int total)
        {
            var report = new EmailMessage
            {
                To = recipient,
                Subject = $"Bulk Email Report — {success}/{total} Sent",
                HtmlBody = $"""
                    <h2>Bulk Email Summary</h2>
                    <ul>
                        <li><strong>Total:</strong> {total}</li>
                        <li><strong>Sent:</strong> {success}</li>
                        <li><strong>Failed:</strong> {failure}</li>
                    </ul>
                    """
            };

            await SendEmailAsync(report);
        }
    }
}
