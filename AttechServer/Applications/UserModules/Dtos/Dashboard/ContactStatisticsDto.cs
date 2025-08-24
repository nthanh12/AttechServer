namespace AttechServer.Applications.UserModules.Dtos.Dashboard
{
    public class ContactStatisticsDto
    {
        public int TotalContacts { get; set; }
        public int UnreadContacts { get; set; }
        public int ReadContacts { get; set; }
        public int ContactsToday { get; set; }
        public int ContactsThisWeek { get; set; }
        public int ContactsThisMonth { get; set; }
        public double ResponseRate { get; set; }
        public ContactTrendDto Trends { get; set; } = new();
        public List<ContactSourceDto> ContactSources { get; set; } = new();
    }

    public class ContactTrendDto
    {
        public int TotalThisMonth { get; set; }
        public int TotalLastMonth { get; set; }
        public double GrowthPercentage { get; set; }
        public string TrendDirection { get; set; } = string.Empty; // "up", "down", "stable"
        public double AveragePerDay { get; set; }
    }

    public class ContactSourceDto
    {
        public string Source { get; set; } = string.Empty;
        public int Count { get; set; }
        public double Percentage { get; set; }
    }

    public class ContactChartDataDto : ChartDataDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalContacts { get; set; }
        public double AveragePerDay { get; set; }
    }
}