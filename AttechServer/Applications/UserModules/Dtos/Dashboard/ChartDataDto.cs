namespace AttechServer.Applications.UserModules.Dtos.Dashboard
{
    public class ChartDataDto
    {
        public string[] Labels { get; set; } = Array.Empty<string>();
        public List<ChartDatasetDto> Datasets { get; set; } = new();
        public ChartOptionsDto Options { get; set; } = new();
    }

    public class ChartDatasetDto
    {
        public string Label { get; set; } = string.Empty;
        public object[] Data { get; set; } = Array.Empty<object>();
        public string BorderColor { get; set; } = string.Empty;
        public string BackgroundColor { get; set; } = string.Empty;
        public double Tension { get; set; }
        public bool Fill { get; set; }
    }

    public class ChartOptionsDto
    {
        public bool Responsive { get; set; } = true;
        public bool MaintainAspectRatio { get; set; } = false;
        public Dictionary<string, object> Plugins { get; set; } = new();
    }

    public class UserGrowthChartDto : ChartDataDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalNewUsers { get; set; }
        public int TotalActiveUsers { get; set; }
    }

    public class ContentDistributionChartDto : ChartDataDto
    {
        public int TotalContent { get; set; }
        public string MostPopularType { get; set; } = string.Empty;
    }
}