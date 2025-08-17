

namespace AttechServer.Applications.UserModules.Dtos.Product
{
    public class ProductDto
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
        public int ProductCategoryId { get; set; }
        public string ProductCategoryTitleVi { get; set; } = string.Empty;
        public string ProductCategoryTitleEn { get; set; } = string.Empty;
        public string ProductCategorySlugVi { get; set; } = string.Empty;
        public string ProductCategorySlugEn { get; set; } = string.Empty;
        public bool IsOutstanding { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
    }
} 
