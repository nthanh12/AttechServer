namespace AttechServer.Infrastructures.Mail
{
    public interface IEmailService
    {
        /// <summary>
        /// Send single email
        /// </summary>
        Task<bool> SendEmailAsync(EmailRequest request);

        /// <summary>
        /// Send multiple emails
        /// </summary>
        Task<bool> SendBulkEmailAsync(List<EmailRequest> requests);

        /// <summary>
        /// Send email with template
        /// </summary>
        Task<bool> SendTemplateEmailAsync(string templatePath, object model, EmailRequest request);

        /// <summary>
        /// Test SMTP connection
        /// </summary>
        Task<bool> TestConnectionAsync();
    }
}