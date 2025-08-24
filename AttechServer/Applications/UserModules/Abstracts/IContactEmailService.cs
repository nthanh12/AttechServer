using AttechServer.Applications.UserModules.Dtos.Contact;

namespace AttechServer.Applications.UserModules.Abstracts
{
    public interface IContactEmailService
    {
        /// <summary>
        /// Send confirmation email to customer
        /// </summary>
        Task SendCustomerConfirmationAsync(ContactDto contact);

        /// <summary>
        /// Send notification email to admin/support team
        /// </summary>
        Task SendAdminNotificationAsync(ContactDto contact);

        /// <summary>
        /// Send auto-response email with estimated response time
        /// </summary>
        Task SendAutoResponseAsync(ContactDto contact, int estimatedResponseHours = 24);

        /// <summary>
        /// Send follow-up email if no response after certain time
        /// </summary>
        Task SendFollowUpEmailAsync(ContactDto contact, int daysSinceSubmission);

        /// <summary>
        /// Send service department notification with customer list
        /// </summary>
        Task SendServiceDepartmentNotificationAsync(List<ContactDto> contacts, string reportPeriod);
    }
}