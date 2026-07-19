using System.Net;
using System.Net.Mail;

namespace Albatros.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string htmlMessage);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
        {
            var smtpServer = _configuration["SmtpSettings:Server"];
            var portStr = _configuration["SmtpSettings:Port"];
            var senderName = _configuration["SmtpSettings:SenderName"] ?? "ALBATROS";
            var senderEmail = _configuration["SmtpSettings:SenderEmail"];
            var username = _configuration["SmtpSettings:Username"];
            var password = _configuration["SmtpSettings:Password"];
            var enableSslStr = _configuration["SmtpSettings:EnableSsl"];

            int.TryParse(portStr, out int port);
            bool.TryParse(enableSslStr, out bool enableSsl);

            if (string.IsNullOrEmpty(smtpServer) || string.IsNullOrEmpty(senderEmail))
            {
                _logger.LogWarning("SMTP Settings are not configured correctly. Skipping real email sending.");
                _logger.LogInformation($"[MOCK EMAIL] To: {toEmail}\nSubject: {subject}\nBody: {htmlMessage}");
                Console.WriteLine($"\n==================================================");
                Console.WriteLine($"[MOCK EMAIL SENT]");
                Console.WriteLine($"To: {toEmail}");
                Console.WriteLine($"Subject: {subject}");
                Console.WriteLine($"Html Body contains code / links. Check logs.");
                Console.WriteLine($"==================================================\n");
                return;
            }

            try
            {
                using var mailMessage = new MailMessage
                {
                    Subject = subject,
                    Body = htmlMessage,
                    IsBodyHtml = true,
                    Priority = MailPriority.Normal
                };
                mailMessage.HeadersEncoding = System.Text.Encoding.UTF8;
                mailMessage.SubjectEncoding = System.Text.Encoding.UTF8;
                mailMessage.BodyEncoding = System.Text.Encoding.UTF8;
                mailMessage.From = new MailAddress("ahmed1712ax@gmail.com", "ALBATROS Real Estate");
                mailMessage.ReplyTo = new MailAddress("ahmed1712ax@gmail.com", "ALBATROS Real Estate");
                mailMessage.To.Add(toEmail);
                mailMessage.Headers.Add("X-Mailer", "ALBATROS Real Estate");

                using var smtpClient = new SmtpClient(smtpServer, port == 0 ? 587 : port)
                {
                    Credentials = new NetworkCredential(username, password),
                    EnableSsl = enableSsl
                };

                await smtpClient.SendMailAsync(mailMessage);
                _logger.LogInformation($"Successfully sent email to {toEmail}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email to {toEmail}. Falling back to console logging.");
                
                // Print clearly to the stdout console so local developer can see the OTP if SMTP credentials are dummy
                Console.WriteLine($"\n==================================================");
                Console.WriteLine($"[EMAIL SEND FAILURE LOG - DEVELOPER FALLBACK]");
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"To: {toEmail}");
                Console.WriteLine($"Subject: {subject}");
                Console.WriteLine($"Body Snippet: Please view the code printed in the controller log or html.");
                Console.WriteLine($"==================================================\n");
            }
        }
    }
}
