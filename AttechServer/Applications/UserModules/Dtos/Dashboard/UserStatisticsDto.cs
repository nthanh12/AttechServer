namespace AttechServer.Applications.UserModules.Dtos.Dashboard
{
    public class UserStatisticsDto
    {
        public int NewUsersToday { get; set; }
        public int NewUsersThisWeek { get; set; }
        public int NewUsersThisMonth { get; set; }
        public int ActiveUsersToday { get; set; }
        public int TotalUsers { get; set; }
        public List<UserRoleDistributionDto> UsersByRole { get; set; } = new();
        public UserGrowthTrendDto GrowthTrend { get; set; } = new();
    }

    public class UserRoleDistributionDto
    {
        public string Role { get; set; } = string.Empty;
        public int Count { get; set; }
        public double Percentage { get; set; }
    }

    public class UserGrowthTrendDto
    {
        public int WeeklyGrowth { get; set; }
        public int MonthlyGrowth { get; set; }
        public double GrowthRate { get; set; }
        public string TrendDirection { get; set; } = string.Empty; // "up", "down", "stable"
    }
}