namespace AttechServer.Applications.UserModules.Dtos.Dashboard
{
    public class DashboardOverviewDto
    {
        public int TotalUsers { get; set; }
        public int TotalProducts { get; set; }
        public int TotalServices { get; set; }
        public int TotalNews { get; set; }
        public int TotalNotifications { get; set; }
        public int TotalContacts { get; set; }
        public int ActiveUsers { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }
}