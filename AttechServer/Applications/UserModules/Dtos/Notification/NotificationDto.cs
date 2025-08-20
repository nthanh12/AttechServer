namespace AttechServer.Applications.UserModules.Dtos.Notification
{
    public class NotificationDto
    {
        public int Id { get; set; }
        public string SlugVi { get; set; } = string.Empty;
        public string SlugEn { get; set; } = string.Empty;
        public string TitleVi { get; set; } = string.Empty;
        public string TitleEn { get; set; } = string.Empty;
        public string DescriptionVi { get; set; } = string.Empty;
        public string DescriptionEn { get; set; } = string.Empty;
        public DateTime TimePosted { get; set; }
        public int Status { get; set; }
        public int NotificationCategoryId { get; set; }
        public string NotificationCategoryTitleVi { get; set; } = string.Empty;
        public string NotificationCategoryTitleEn { get; set; } = string.Empty;
        public string NotificationCategorySlugVi { get; set; } = string.Empty;
        public string NotificationCategorySlugEn { get; set; } = string.Empty;
        public bool IsOutstanding { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public int? FeaturedImageId { get; set; }
    }
} 
