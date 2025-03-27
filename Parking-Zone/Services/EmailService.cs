using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace Parking_Zone.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly IEmailTemplateService _templateService;
        private readonly EmailSettings _emailSettings;

        public EmailService(
            ILogger<EmailService> logger,
            IEmailTemplateService templateService,
            IOptions<EmailSettings> emailSettings)
        {
            _logger = logger;
            _templateService = templateService;
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            try
            {
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(email);

                using (var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.Port))
                {
                    client.EnableSsl = _emailSettings.EnableSsl;
                    client.Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password);

                    await client.SendMailAsync(mailMessage);
                }

                _logger.LogInformation($"Email sent successfully to {email}");
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"Failed to send email to {email}: {ex.Message}");
                throw;
            }
        }

        public async Task SendEmailConfirmationAsync(string email, string callbackUrl)
        {
            var message = _templateService.GetEmailConfirmationTemplate(callbackUrl);
            await SendEmailAsync(email, "Confirm your email", message);
        }

        public async Task SendPasswordResetAsync(string email, string callbackUrl)
        {
            var message = _templateService.GetPasswordResetTemplate(callbackUrl);
            await SendEmailAsync(email, "Reset your password", message);
        }

        public async Task SendAccountLockedAsync(string email)
        {
            var message = _templateService.GetAccountLockedTemplate();
            await SendEmailAsync(email, "Account Locked", message);
        }
    }

    public class EmailSettings
    {
        public string SenderEmail { get; set; }
        public string SenderName { get; set; }
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool EnableSsl { get; set; }
    }
}