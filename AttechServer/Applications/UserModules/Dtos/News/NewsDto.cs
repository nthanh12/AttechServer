

namespace AttechServer.Applications.UserModules.Dtos.News
{
    public class NewsDto
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
        public int NewsCategoryId { get; set; }
        public string NewsCategoryTitleVi { get; set; } = string.Empty;
        public string NewsCategoryTitleEn { get; set; } = string.Empty;
        public string NewsCategorySlugVi { get; set; } = string.Empty;
        public string NewsCategorySlugEn { get; set; } = string.Empty;
        public bool IsOutstanding { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public int? FeaturedImageId { get; set; }
    }
} 
