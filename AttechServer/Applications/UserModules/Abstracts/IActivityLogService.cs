using AttechServer.Domains.Entities.Main;

namespace AttechServer.Applications.UserModules.Abstracts
{
    public interface IActivityLogService
    {
        Task LogAsync(string type, string message, string? details = null, string severity = "Info");
        Task LogUserActionAsync(string action, string message, int? userId = null, string? details = null);
        Task LogSystemActionAsync(string action, string message, string? details = null);
        Task LogSecurityEventAsync(string eventType, string message, string? details = null);
        Task<List<ActivityLog>> GetRecentActivitiesAsync(int limit = 10);
        Task<List<ActivityLog>> GetActivitiesByTypeAsync(string type, int limit = 20);
    }
}
