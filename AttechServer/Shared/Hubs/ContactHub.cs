using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using AttechServer.Applications.UserModules.Dtos.Contact;

namespace AttechServer.Shared.Hubs
{
    [Authorize]
    public class ContactHub : Hub
    {
        private readonly ILogger<ContactHub> _logger;

        public ContactHub(ILogger<ContactHub> logger)
        {
            _logger = logger;
        }

        public async Task JoinAdminGroup()
        {
            // Only allow admin users (role level 2+)
            var userRole = Context.User?.FindFirst("user_level")?.Value;
            if (int.TryParse(userRole, out int roleLevel) && roleLevel >= 2)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "AdminGroup");
                _logger.LogInformation($"User {Context.UserIdentifier} joined AdminGroup for contact notifications");
            }
        }

        public async Task LeaveAdminGroup()
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "AdminGroup");
            _logger.LogInformation($"User {Context.UserIdentifier} left AdminGroup");
        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation($"User {Context.UserIdentifier} connected to ContactHub");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogInformation($"User {Context.UserIdentifier} disconnected from ContactHub");
            await base.OnDisconnectedAsync(exception);
        }
    }
}