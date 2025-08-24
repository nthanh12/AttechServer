namespace AttechServer.Applications.UserModules.Dtos.Dashboard
{
    public class ContentStatisticsDto
    {
        public int NewsPublishedThisMonth { get; set; }
        public int NotificationsThisMonth { get; set; }
        public int ProductsAddedThisMonth { get; set; }
        public int ServicesAddedThisMonth { get; set; }
        public List<CategoryDistributionDto> CategoryDistribution { get; set; } = new();
        public ContentTrendDto Trends { get; set; } = new();
    }

    public class CategoryDistributionDto
    {
        public string Category { get; set; } = string.Empty;
        public int Count { get; set; }
        public string Type { get; set; } = string.Empty;
        public double Percentage { get; set; }
    }

    public class ContentTrendDto
    {
        public int TotalContentThisMonth { get; set; }
        public int TotalContentLastMonth { get; set; }
        public double GrowthPercentage { get; set; }
        public string MostActiveCategory { get; set; } = string.Empty;
    }
}