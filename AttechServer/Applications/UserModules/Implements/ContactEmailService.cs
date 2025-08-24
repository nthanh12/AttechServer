using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Applications.UserModules.Dtos.Contact;
using AttechServer.Infrastructures.Mail;
using System.Text;

namespace AttechServer.Applications.UserModules.Implements
{
    public class ContactEmailService : IContactEmailService
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<ContactEmailService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public ContactEmailService(
            IEmailService emailService,
            ILogger<ContactEmailService> logger,
            IConfiguration configuration,
            IWebHostEnvironment environment)
        {
            _emailService = emailService;
            _logger = logger;
            _configuration = configuration;
            _environment = environment;
        }

        public async Task SendCustomerConfirmationAsync(ContactDto contact)
        {
            try
            {
                var templatePath = Path.Combine(_environment.ContentRootPath, 
                    "Shared", "EmailTemplates", "Contact", "ContactConfirmation.html");
                
                if (!File.Exists(templatePath))
                {
                    _logger.LogError($"Email template not found: {templatePath}");
                    return;
                }

                var htmlTemplate = await File.ReadAllTextAsync(templatePath);
                
                // Replace template variables
                var htmlContent = htmlTemplate
                    .Replace("{{CustomerName}}", contact.Name)
                    .Replace("{{Subject}}", contact.Subject)
                    .Replace("{{Message}}", contact.Message)
                    .Replace("{{Email}}", contact.Email)
                    .Replace("{{PhoneNumber}}", contact.PhoneNumber ?? "")
                    .Replace("{{SubmittedAt}}", contact.SubmittedAt.ToString("dd/MM/yyyy HH:mm"))
                    .Replace("{{ContactId}}", contact.Id.ToString())
                    .Replace("{{Year}}", DateTime.Now.Year.ToString())
                    .Replace("{{CompanyName}}", GetCompanyName())
                    .Replace("{{SupportHotline}}", GetSupportHotline());

                // Handle conditional PhoneNumber section
                if (string.IsNullOrWhiteSpace(contact.PhoneNumber))
                {
                    htmlContent = RemovePhoneSection(htmlContent);
                }
                else
                {
                    // Replace {{#if}} {{/if}} with actual content when phone exists
                    htmlContent = htmlContent.Replace("{{#if PhoneNumber}}", "")
                                           .Replace("{{/if}}", "");
                }

                var emailRequest = new EmailRequest
                {
                    ToEmail = contact.Email,
                    ToName = contact.Name,
                    Subject = $"Xác nhận liên hệ - {contact.Subject}",
                    Body = htmlContent,
                    IsHtml = true
                };

                var success = await _emailService.SendEmailAsync(emailRequest);
                if (success)
                {
                    _logger.LogInformation($"Customer confirmation email sent successfully to {contact.Email}");
                }
                else
                {
                    _logger.LogWarning($"Failed to send customer confirmation email to {contact.Email}");
                }
                _logger.LogInformation($"Customer confirmation email sent to {contact.Email}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending customer confirmation email to {contact.Email}");
            }
        }

        public async Task SendAdminNotificationAsync(ContactDto contact)
        {
            try
            {
                var templatePath = Path.Combine(_environment.ContentRootPath,
                    "Shared", "EmailTemplates", "Contact", "AdminNotification.html");

                if (!File.Exists(templatePath))
                {
                    _logger.LogError($"Email template not found: {templatePath}");
                    return;
                }

                var htmlTemplate = await File.ReadAllTextAsync(templatePath);
                var adminUrl = GetAdminUrl();

                var htmlContent = htmlTemplate
                    .Replace("{{CustomerName}}", contact.Name)
                    .Replace("{{Email}}", contact.Email)
                    .Replace("{{PhoneNumber}}", contact.PhoneNumber ?? "")
                    .Replace("{{Subject}}", contact.Subject)
                    .Replace("{{Message}}", contact.Message)
                    .Replace("{{SubmittedAt}}", contact.SubmittedAt.ToString("dd/MM/yyyy HH:mm"))
                    .Replace("{{IpAddress}}", contact.IpAddress ?? "N/A")
                    .Replace("{{UserAgent}}", "N/A") // Will be added from ContactService
                    .Replace("{{ContactId}}", contact.Id.ToString())
                    .Replace("{{AdminUrl}}", adminUrl)
                    .Replace("{{CompanyName}}", GetCompanyName())
                    .Replace("{{CurrentTime}}", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));

                // Handle conditional PhoneNumber section
                if (string.IsNullOrWhiteSpace(contact.PhoneNumber))
                {
                    htmlContent = RemovePhoneSection(htmlContent);
                }
                else
                {
                    // Replace {{#if}} {{/if}} with actual content when phone exists
                    htmlContent = htmlContent.Replace("{{#if PhoneNumber}}", "")
                                           .Replace("{{/if}}", "");
                }

                // Determine if urgent (keywords in subject/message)
                var isUrgent = IsUrgentContact(contact);
                htmlContent = htmlContent.Replace("{{#if IsUrgent}}priority-high{{/if}}", 
                    isUrgent ? "priority-high" : "");

                var adminEmails = GetAdminEmails();
                foreach (var adminEmail in adminEmails)
                {
                    var emailRequest = new EmailRequest
                    {
                        ToEmail = adminEmail,
                        Subject = $"🔔 Liên hệ mới: {contact.Subject}",
                        Body = htmlContent,
                        IsHtml = true
                    };

                    var success = await _emailService.SendEmailAsync(emailRequest);
                    if (success)
                    {
                        _logger.LogInformation($"Admin notification email sent successfully to {adminEmail}");
                    }
                    else
                    {
                        _logger.LogWarning($"Failed to send admin notification email to {adminEmail}");
                    }
                }

                _logger.LogInformation($"Admin notification emails sent for contact {contact.Id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending admin notification email for contact {contact.Id}");
            }
        }

        public async Task SendAutoResponseAsync(ContactDto contact, int estimatedResponseHours = 24)
        {
            try
            {
                var autoResponseContent = GenerateAutoResponse(contact, estimatedResponseHours);

                var emailRequest = new EmailRequest
                {
                    ToEmail = contact.Email,
                    ToName = contact.Name,
                    Subject = $"Re: {contact.Subject}",
                    Body = autoResponseContent,
                    IsHtml = true
                };

                var success = await _emailService.SendEmailAsync(emailRequest);
                if (success)
                {
                    _logger.LogInformation($"Auto-response email sent successfully to {contact.Email}");
                }
                else
                {
                    _logger.LogWarning($"Failed to send auto-response email to {contact.Email}");
                }
                _logger.LogInformation($"Auto-response email sent to {contact.Email}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending auto-response email to {contact.Email}");
            }
        }

        public async Task SendFollowUpEmailAsync(ContactDto contact, int daysSinceSubmission)
        {
            try
            {
                var followUpContent = GenerateFollowUpContent(contact, daysSinceSubmission);

                var emailRequest = new EmailRequest
                {
                    ToEmail = contact.Email,
                    ToName = contact.Name,
                    Subject = $"Theo dõi yêu cầu: {contact.Subject}",
                    Body = followUpContent,
                    IsHtml = true
                };

                var success = await _emailService.SendEmailAsync(emailRequest);
                if (success)
                {
                    _logger.LogInformation($"Follow-up email sent successfully to {contact.Email}");
                }
                else
                {
                    _logger.LogWarning($"Failed to send follow-up email to {contact.Email}");
                }
                _logger.LogInformation($"Follow-up email sent to {contact.Email}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending follow-up email to {contact.Email}");
            }
        }

        private string RemovePhoneSection(string htmlContent)
        {
            // Remove entire conditional phone section including {{#if}}{{/if}}
            var phonePattern = @"\{\{#if PhoneNumber\}\}.*?\{\{/if\}\}";
            return System.Text.RegularExpressions.Regex.Replace(htmlContent, phonePattern, "", System.Text.RegularExpressions.RegexOptions.Singleline);
        }

        private bool IsUrgentContact(ContactDto contact)
        {
            var urgentKeywords = new[] { "khẩn cấp", "urgent", "gấp", "emergency", "asap", "ngay lập tức" };
            var content = $"{contact.Subject} {contact.Message}".ToLower();
            return urgentKeywords.Any(keyword => content.Contains(keyword));
        }

        private string GenerateAutoResponse(ContactDto contact, int hours)
        {
            return $@"
                <h3>Kính chào {contact.Name},</h3>
                <p>Cảm ơn bạn đã liên hệ với chúng tôi.</p>
                <p>Chúng tôi đã nhận được yêu cầu của bạn và sẽ phản hồi trong vòng <strong>{hours} giờ</strong>.</p>
                <p>Mã tham chiếu: <strong>#{contact.Id}</strong></p>
                <p>Trân trọng,<br>Đội ngũ Hỗ trợ khách hàng</p>";
        }

        public async Task SendServiceDepartmentNotificationAsync(List<ContactDto> contacts, string reportPeriod)
        {
            try
            {
                var templatePath = Path.Combine(_environment.ContentRootPath,
                    "Shared", "EmailTemplates", "Contact", "ServiceDepartmentNotification.html");

                if (!File.Exists(templatePath))
                {
                    _logger.LogError($"Service department email template not found: {templatePath}");
                    return;
                }

                var htmlTemplate = await File.ReadAllTextAsync(templatePath);

                // Prepare template data
                var urgentContacts = contacts.Count(c => IsUrgentContact(c));
                var todayContacts = contacts.Count(c => c.SubmittedAt.Date == DateTime.Today);

                var templateData = new
                {
                    ReportTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                    ReportPeriod = reportPeriod,
                    TotalContacts = contacts.Count,
                    UrgentContacts = urgentContacts,
                    NewContacts = todayContacts,
                    Contacts = contacts.Select(c => new
                    {
                        c.Id,
                        c.Name,
                        c.Email,
                        c.PhoneNumber,
                        c.Subject,
                        c.Message,
                        c.IpAddress,
                        SubmittedAt = c.SubmittedAt.ToString("dd/MM/yyyy HH:mm"),
                        IsUrgent = IsUrgentContact(c)
                    }),
                    AdminUrl = GetAdminUrl(),
                    CompanyName = GetCompanyName(),
                    CurrentTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")
                };

                // Process template (simple replacement)
                var htmlContent = ProcessServiceDepartmentTemplate(htmlTemplate, templateData);

                // Send to service department emails
                var serviceDepartmentEmails = GetServiceDepartmentEmails();
                foreach (var email in serviceDepartmentEmails)
                {
                    var emailRequest = new EmailRequest
                    {
                        ToEmail = email,
                        Subject = $"📋 Báo cáo liên hệ khách hàng - {reportPeriod} ({contacts.Count} liên hệ)",
                        Body = htmlContent,
                        IsHtml = true
                    };

                    var success = await _emailService.SendEmailAsync(emailRequest);
                    if (success)
                    {
                        _logger.LogInformation($"Service department notification sent successfully to {email}");
                    }
                    else
                    {
                        _logger.LogWarning($"Failed to send service department notification to {email}");
                    }
                }

                _logger.LogInformation($"Service department notifications sent for {contacts.Count} contacts");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending service department notification");
            }
        }

        private string GenerateFollowUpContent(ContactDto contact, int days)
        {
            return $@"
                <h3>Kính chào {contact.Name},</h3>
                <p>Chúng tôi muốn theo dõi yêu cầu liên hệ của bạn từ {days} ngày trước.</p>
                <p><strong>Yêu cầu:</strong> {contact.Subject}</p>
                <p>Nếu bạn chưa nhận được phản hồi hoặc cần hỗ trợ thêm, vui lòng liên hệ trực tiếp qua hotline.</p>
                <p>Mã tham chiếu: <strong>#{contact.Id}</strong></p>
                <p>Trân trọng,<br>Đội ngũ Hỗ trợ khách hàng</p>";
        }

        private string ProcessServiceDepartmentTemplate(string template, object model)
        {
            var processedTemplate = template;
            var properties = model.GetType().GetProperties();

            foreach (var property in properties)
            {
                var value = property.GetValue(model);
                var placeholder = $"{{{{{property.Name}}}}}";

                if (property.Name == "Contacts")
                {
                    // Handle contacts list separately
                    var contacts = value as IEnumerable<object>;
                    if (contacts != null)
                    {
                        var contactsHtml = ProcessContactsList(contacts);
                        processedTemplate = processedTemplate.Replace("{{#each Contacts}}", "")
                                                           .Replace("{{/each}}", "")
                                                           .Replace("{{Name}}", contactsHtml);
                    }
                }
                else
                {
                    var stringValue = value?.ToString() ?? "";
                    processedTemplate = processedTemplate.Replace(placeholder, stringValue);
                }
            }

            return processedTemplate;
        }

        private string ProcessContactsList(IEnumerable<object> contacts)
        {
            var html = new StringBuilder();
            
            foreach (var contact in contacts)
            {
                var contactProps = contact.GetType().GetProperties().ToDictionary(p => p.Name, p => p.GetValue(contact)?.ToString() ?? "");
                
                var isUrgent = bool.Parse(contactProps.GetValueOrDefault("IsUrgent", "false"));
                var phoneNumber = contactProps.GetValueOrDefault("PhoneNumber", "");

                html.Append($@"
                <div class=""customer-card {(isUrgent ? "priority-high" : "")}"">
                    <div class=""customer-header"">
                        <div class=""customer-name"">
                            {contactProps["Name"]}
                            {(isUrgent ? @"<span class=""urgent-badge"">Khẩn cấp</span>" : "")}
                        </div>
                        <div class=""customer-time"">{contactProps["SubmittedAt"]}</div>
                    </div>
                    
                    <div class=""customer-details"">
                        <div class=""detail-row"">
                            <span class=""detail-label"">📧 Email:</span>
                            <span class=""detail-value"">
                                <a href=""mailto:{contactProps["Email"]}"" class=""contact-email"">{contactProps["Email"]}</a>
                            </span>
                        </div>");

                if (!string.IsNullOrEmpty(phoneNumber))
                {
                    html.Append($@"
                        <div class=""detail-row"">
                            <span class=""detail-label"">📱 Điện thoại:</span>
                            <span class=""detail-value"">
                                <a href=""tel:{phoneNumber}"" class=""contact-phone"">{phoneNumber}</a>
                            </span>
                        </div>");
                }

                html.Append($@"
                        <div class=""detail-row"">
                            <span class=""detail-label"">📝 Tiêu đề:</span>
                            <span class=""detail-value""><strong>{contactProps["Subject"]}</strong></span>
                        </div>
                        
                        <div class=""detail-row"">
                            <span class=""detail-label"">💬 Nội dung:</span>
                            <div class=""detail-value"">
                                <div class=""message-box"">{contactProps["Message"]}</div>
                            </div>
                        </div>
                        
                        <div class=""detail-row"">
                            <span class=""detail-label"">🌐 IP:</span>
                            <span class=""detail-value"">{contactProps["IpAddress"]}</span>
                        </div>
                    </div>
                </div>");
            }

            return html.ToString();
        }

        private string GetCompanyName()
        {
            return _configuration["Company:Name"] ?? "Công ty";
        }

        private string GetSupportHotline()
        {
            return _configuration["Company:SupportHotline"] ?? "1900-xxxx";
        }

        private string GetAdminUrl()
        {
            return _configuration["AdminUrl"] ?? "https://admin.example.com";
        }

        private List<string> GetAdminEmails()
        {
            var emailsStr = _configuration["Contact:AdminEmails"] ?? "admin@example.com";
            return emailsStr.Split(',', StringSplitOptions.RemoveEmptyEntries)
                           .Select(e => e.Trim())
                           .ToList();
        }

        private List<string> GetServiceDepartmentEmails()
        {
            var emailsStr = _configuration["Contact:ServiceDepartmentEmails"] ?? _configuration["Contact:AdminEmails"] ?? "service@example.com";
            return emailsStr.Split(',', StringSplitOptions.RemoveEmptyEntries)
                           .Select(e => e.Trim())
                           .ToList();
        }
    }
}