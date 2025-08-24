namespace AttechServer.Applications.UserModules.Dtos.Dashboard
{
    public class ActivityDto
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string Severity { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string? UserName { get; set; }
        public string? Details { get; set; }
        public string TimeAgo { get; set; } = string.Empty;
    }

    public class RealtimeDataDto
    {
        public int CurrentActiveUsers { get; set; }
        public double SystemLoad { get; set; }
        public double MemoryUsage { get; set; }
        public long StorageUsed { get; set; }
        public DateTime LastUpdated { get; set; }
        public List<AlertDto> ActiveAlerts { get; set; } = new();
    }

    public class AlertDto
    {
        public string Type { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}