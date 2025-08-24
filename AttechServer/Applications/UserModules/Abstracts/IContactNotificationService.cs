using AttechServer.Applications.UserModules.Dtos.Contact;

namespace AttechServer.Applications.UserModules.Abstracts
{
    public interface IContactNotificationService
    {
        /// <summary>
        /// Send real-time notification to admin when new contact is received
        /// </summary>
        Task NotifyAdminNewContactAsync(ContactDto contact);

        /// <summary>
        /// Send real-time notification when contact status is updated
        /// </summary>
        Task NotifyAdminContactStatusChangedAsync(ContactDto contact, int oldStatus, int newStatus);

        /// <summary>
        /// Send real-time notification for urgent contacts
        /// </summary>
        Task NotifyAdminUrgentContactAsync(ContactDto contact);

        /// <summary>
        /// Update real-time contact count for admin dashboard
        /// </summary>
        Task UpdateUnreadContactCountAsync(int unreadCount);
    }
}