using AttechServer.Applications.UserModules.Dtos.Contact;

namespace AttechServer.Applications.UserModules.Abstracts
{
    public interface ICrmIntegrationService
    {
        /// <summary>
        /// Create a new ticket/lead in CRM system when contact is submitted
        /// </summary>
        Task<string?> CreateCrmTicketAsync(ContactDto contact);

        /// <summary>
        /// Update CRM ticket status when contact status changes
        /// </summary>
        Task UpdateCrmTicketStatusAsync(string crmTicketId, int contactStatus);

        /// <summary>
        /// Sync contact information to CRM customer database
        /// </summary>
        Task<string?> SyncCustomerToCrmAsync(ContactDto contact);

        /// <summary>
        /// Add note to CRM ticket
        /// </summary>
        Task AddCrmTicketNoteAsync(string crmTicketId, string note, string addedBy);

        /// <summary>
        /// Check if CRM integration is enabled and configured
        /// </summary>
        bool IsCrmIntegrationEnabled();

        /// <summary>
        /// Test CRM connection
        /// </summary>
        Task<bool> TestCrmConnectionAsync();
    }
}