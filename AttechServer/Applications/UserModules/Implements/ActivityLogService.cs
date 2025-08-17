using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Domains.Entities.Main;
using AttechServer.Infrastructures.Persistances;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AttechServer.Applications.UserModules.Implements
{
    public class ActivityLogService : IActivityLogService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ActivityLogService> _logger;

        public ActivityLogService(
            ApplicationDbContext context, 
            IHttpContextAccessor httpContextAccessor,
            ILogger<ActivityLogService> logger)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task LogAsync(string type, string message, string? details = null, string severity = "Info")
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                var userId = GetCurrentUserId();

                var activityLog = new ActivityLog
                {
                    Type = type,
                    Message = message,
                    Details = details,
                    Severity = severity,
                    IpAddress = GetClientIpAddress(),
                    UserAgent = httpContext?.Request.Headers["User-Agent"].ToString(),
                    Timestamp = DateTime.Now,
                    CreatedBy = userId
                };

                _context.ActivityLogs.Add(activityLog);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to log activity: {Type} - {Message}", type, message);
            }
        }

        public async Task LogUserActionAsync(string action, string message, int? userId = null, string? details = null)
        {
            var currentUserId = userId ?? GetCurrentUserId();
            var logMessage = currentUserId.HasValue 
                ? $"User {currentUserId}: {message}" 
                : $"Anonymous: {message}";

            await LogAsync($"user_{action}", logMessage, details, "Info");
        }

        public async Task LogSystemActionAsync(string action, string message, string? details = null)
        {
            await LogAsync($"system_{action}", message, details, "Info");
        }

        public async Task LogSecurityEventAsync(string eventType, string message, string? details = null)
        {
            await LogAsync($"security_{eventType}", message, details, "Warning");
        }

        public async Task<List<ActivityLog>> GetRecentActivitiesAsync(int limit = 10)
        {
            return await _context.ActivityLogs
                .Where(a => !a.Deleted)
                .OrderByDescending(a => a.Timestamp)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<List<ActivityLog>> GetActivitiesByTypeAsync(string type, int limit = 20)
        {
            return await _context.ActivityLogs
                .Where(a => !a.Deleted && a.Type == type)
                .OrderByDescending(a => a.Timestamp)
                .Take(limit)
                .ToListAsync();
        }

        private int? GetCurrentUserId()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity is ClaimsIdentity identity)
            {
                var claim = identity.FindFirst("user_id");
                if (claim != null && int.TryParse(claim.Value, out int userId))
                {
                    return userId;
                }
            }
            return null;
        }

        private string? GetClientIpAddress()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null) return null;

            // Check for forwarded IP first (in case of proxy/load balancer)
            var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                return forwardedFor.Split(',')[0].Trim();
            }

            var realIp = httpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrEmpty(realIp))
            {
                return realIp;
            }

            return httpContext.Connection.RemoteIpAddress?.ToString();
        }
    }
}
