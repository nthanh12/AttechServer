using AttechServer.Applications.UserModules.Dtos.Contact;
using AttechServer.Shared.ApplicationBase.Common;

namespace AttechServer.Applications.UserModules.Abstracts
{
    public interface IContactService
    {
        /// <summary>
        /// Submit contact form (Public API)
        /// </summary>
        Task<ContactDto> Submit(CreateContactDto input, string? ipAddress = null, string? userAgent = null);

        /// <summary>
        /// Get all contact messages with filtering and sorting (Admin only)
        /// </summary>
        Task<PagingResult<ContactDto>> FindAll(PagingRequestBaseDto input);

        /// <summary>
        /// Get contact message detail by ID (Admin only)
        /// </summary>
        Task<DetailContactDto> FindById(int id);

        /// <summary>
        /// Update contact message status (Admin only)
        /// </summary>
        Task<ContactDto> UpdateStatus(int id, UpdateContactStatusDto input);

        /// <summary>
        /// Delete contact message (Admin only)
        /// </summary>
        Task Delete(int id);

        /// <summary>
        /// Mark contact message as read (Admin only)
        /// </summary>
        Task MarkAsRead(int id);

        /// <summary>
        /// Mark contact message as unread (Admin only)
        /// </summary>
        Task MarkAsUnread(int id);

        /// <summary>
        /// Get unread contact count (Admin only)
        /// </summary>
        Task<int> GetUnreadCount();
    }
}