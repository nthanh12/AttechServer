using AttechServer.Applications.UserModules.Dtos.Attachment;

namespace AttechServer.Applications.UserModules.Dtos.News
{
    public class AlbumDetailDto
    {
        public int Id { get; set; }
        public string TitleVi { get; set; } = string.Empty;
        public string TitleEn { get; set; } = string.Empty;
        public string? DescriptionVi { get; set; }
        public string? DescriptionEn { get; set; }
        public string SlugVi { get; set; } = string.Empty;
        public string SlugEn { get; set; } = string.Empty;
        
        public int NewsCategoryId { get; set; }
        public string? NewsCategoryName { get; set; }
        
        public int Status { get; set; }
        public bool IsOutstanding { get; set; }
        
        // Featured image info (giống News)
        public int? FeaturedImageId { get; set; }
        public string ImageUrl { get; set; } = string.Empty; // Legacy field giống News
        
        // All album attachments (giống News structure)
        public AttachmentsGroupDto Attachments { get; set; } = new();
        
        // Audit info
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
    }
}