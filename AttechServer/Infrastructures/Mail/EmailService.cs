using System.Net;
using System.Net.Mail;
using System.Text;

namespace AttechServer.Infrastructures.Mail
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        private readonly SmtpClient _smtpClient;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _smtpClient = CreateSmtpClient();
        }

        public async Task<bool> SendEmailAsync(EmailRequest request)
        {
            try
            {
                _logger.LogInformation($"Sending email to {request.ToEmail} with subject: {request.Subject}");

                using var mailMessage = new MailMessage();
                
                // From
                var fromEmail = _configuration["Email:From"] ?? throw new InvalidOperationException("Email:From not configured");
                var fromName = _configuration["Email:FromName"] ?? fromEmail;
                mailMessage.From = new MailAddress(fromEmail, fromName, Encoding.UTF8);

                // To
                if (!string.IsNullOrEmpty(request.ToName))
                {
                    mailMessage.To.Add(new MailAddress(request.ToEmail, request.ToName, Encoding.UTF8));
                }
                else
                {
                    mailMessage.To.Add(request.ToEmail);
                }

                // CC
                if (request.CC != null && request.CC.Any())
                {
                    foreach (var cc in request.CC)
                    {
                        mailMessage.CC.Add(cc);
                    }
                }

                // BCC
                if (request.BCC != null && request.BCC.Any())
                {
                    foreach (var bcc in request.BCC)
                    {
                        mailMessage.Bcc.Add(bcc);
                    }
                }

                // Reply To
                if (!string.IsNullOrEmpty(request.ReplyTo))
                {
                    mailMessage.ReplyToList.Add(request.ReplyTo);
                }

                // Subject and Body
                mailMessage.Subject = request.Subject;
                mailMessage.SubjectEncoding = Encoding.UTF8;
                mailMessage.Body = request.Body;
                mailMessage.BodyEncoding = Encoding.UTF8;
                mailMessage.IsBodyHtml = request.IsHtml;

                // Attachments
                if (request.Attachments != null && request.Attachments.Any())
                {
                    foreach (var attachment in request.Attachments)
                    {
                        var stream = new MemoryStream(attachment.Content);
                        var mailAttachment = new Attachment(stream, attachment.FileName, attachment.ContentType);
                        mailMessage.Attachments.Add(mailAttachment);
                    }
                }

                await _smtpClient.SendMailAsync(mailMessage);
                _logger.LogInformation($"Email sent successfully to {request.ToEmail}");
                return true;
            }
            catch (SmtpException ex)
            {
                _logger.LogError(ex, $"SMTP error sending email to {request.ToEmail}: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending email to {request.ToEmail}: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> SendBulkEmailAsync(List<EmailRequest> requests)
        {
            var results = new List<bool>();
            
            foreach (var request in requests)
            {
                try
                {
                    var result = await SendEmailAsync(request);
                    results.Add(result);
                    
                    // Small delay between emails to avoid rate limiting
                    await Task.Delay(100);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error in bulk email to {request.ToEmail}");
                    results.Add(false);
                }
            }

            var successCount = results.Count(r => r);
            _logger.LogInformation($"Bulk email completed: {successCount}/{requests.Count} sent successfully");
            
            return successCount == requests.Count;
        }

        public async Task<bool> SendTemplateEmailAsync(string templatePath, object model, EmailRequest request)
        {
            try
            {
                if (!File.Exists(templatePath))
                {
                    _logger.LogError($"Email template not found: {templatePath}");
                    return false;
                }

                var template = await File.ReadAllTextAsync(templatePath);
                
                // Simple template replacement (you can use a proper template engine like Razor, Handlebars, etc.)
                var processedBody = ProcessTemplate(template, model);
                
                request.Body = processedBody;
                return await SendEmailAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending template email: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                _logger.LogInformation("Testing SMTP connection...");
                
                // Create a test email
                var testRequest = new EmailRequest
                {
                    ToEmail = _configuration["Email:From"] ?? "test@example.com",
                    Subject = "SMTP Connection Test",
                    Body = $"SMTP connection test sent at {DateTime.Now:yyyy-MM-dd HH:mm:ss}",
                    IsHtml = false
                };

                // Try to send test email
                return await SendEmailAsync(testRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"SMTP connection test failed: {ex.Message}");
                return false;
            }
        }

        private SmtpClient CreateSmtpClient()
        {
            var smtpHost = _configuration["Email:Smtp:Host"] ?? throw new InvalidOperationException("Email:Smtp:Host not configured");
            var smtpPort = int.Parse(_configuration["Email:Smtp:Port"] ?? "587");
            var enableSsl = bool.Parse(_configuration["Email:Smtp:EnableSsl"] ?? "true");
            var username = _configuration["Email:Smtp:Username"];
            var password = _configuration["Email:Smtp:Password"];

            var client = new SmtpClient(smtpHost, smtpPort)
            {
                EnableSsl = enableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false
            };

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                client.Credentials = new NetworkCredential(username, password);
            }

            return client;
        }

        private string ProcessTemplate(string template, object model)
        {
            // Simple template processing using reflection
            // For production, consider using a proper template engine like Razor Engine or Handlebars.Net
            
            var properties = model.GetType().GetProperties();
            var processedTemplate = template;

            foreach (var property in properties)
            {
                var value = property.GetValue(model)?.ToString() ?? "";
                var placeholder = $"{{{{{property.Name}}}}}";
                processedTemplate = processedTemplate.Replace(placeholder, value);
            }

            return processedTemplate;
        }

        public void Dispose()
        {
            _smtpClient?.Dispose();
        }
    }
}