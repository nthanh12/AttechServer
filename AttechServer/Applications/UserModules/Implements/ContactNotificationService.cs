using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Applications.UserModules.Dtos.Contact;
using AttechServer.Shared.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace AttechServer.Applications.UserModules.Implements
{
    public class ContactNotificationService : IContactNotificationService
    {
        private readonly IHubContext<ContactHub> _hubContext;
        private readonly ILogger<ContactNotificationService> _logger;

        public ContactNotificationService(IHubContext<ContactHub> hubContext, ILogger<ContactNotificationService> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task NotifyAdminNewContactAsync(ContactDto contact)
        {
            try
            {
                var notification = new
                {
                    Type = "NEW_CONTACT",
                    Contact = contact,
                    Message = $"Liên hệ mới từ {contact.Name}",
                    Timestamp = DateTime.UtcNow,
                    IsUrgent = IsUrgentContact(contact)
                };

                await _hubContext.Clients.Group("AdminGroup").SendAsync("NewContactReceived", notification);
                _logger.LogInformation($"Sent new contact notification to admin group for contact {contact.Id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending new contact notification for contact {contact.Id}");
            }
        }

        public async Task NotifyAdminContactStatusChangedAsync(ContactDto contact, int oldStatus, int newStatus)
        {
            try
            {
                var notification = new
                {
                    Type = "CONTACT_STATUS_CHANGED",
                    ContactId = contact.Id,
                    CustomerName = contact.Name,
                    Subject = contact.Subject,
                    OldStatus = oldStatus,
                    NewStatus = newStatus,
                    StatusText = GetStatusText(newStatus),
                    Message = $"Trạng thái liên hệ #{contact.Id} đã thay đổi",
                    Timestamp = DateTime.UtcNow
                };

                await _hubContext.Clients.Group("AdminGroup").SendAsync("ContactStatusChanged", notification);
                _logger.LogInformation($"Sent status change notification for contact {contact.Id}: {oldStatus} -> {newStatus}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending status change notification for contact {contact.Id}");
            }
        }

        public async Task NotifyAdminUrgentContactAsync(ContactDto contact)
        {
            try
            {
                var notification = new
                {
                    Type = "URGENT_CONTACT",
                    Contact = contact,
                    Message = $"🚨 KHẨN CẤP: Liên hệ từ {contact.Name}",
                    Timestamp = DateTime.UtcNow,
                    Priority = "HIGH"
                };

                // Send to admin group with special urgent flag
                await _hubContext.Clients.Group("AdminGroup").SendAsync("UrgentContactReceived", notification);
                _logger.LogWarning($"Sent urgent contact notification for contact {contact.Id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending urgent contact notification for contact {contact.Id}");
            }
        }

        public async Task UpdateUnreadContactCountAsync(int unreadCount)
        {
            try
            {
                var update = new
                {
                    Type = "UNREAD_COUNT_UPDATE",
                    UnreadCount = unreadCount,
                    Timestamp = DateTime.UtcNow
                };

                await _hubContext.Clients.Group("AdminGroup").SendAsync("UnreadCountUpdated", update);
                _logger.LogInformation($"Updated unread contact count: {unreadCount}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating unread contact count");
            }
        }

        private bool IsUrgentContact(ContactDto contact)
        {
            var urgentKeywords = new[] { "khẩn cấp", "urgent", "gấp", "emergency", "asap", "ngay lập tức" };
            var content = $"{contact.Subject} {contact.Message}".ToLower();
            return urgentKeywords.Any(keyword => content.Contains(keyword));
        }

        private string GetStatusText(int status)
        {
            return status switch
            {
                0 => "Chưa đọc",
                1 => "Đã đọc",
                _ => "Không xác định"
            };
        }
    }
}