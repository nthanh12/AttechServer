using AttechServer.Applications.UserModules.Dtos.Attachment;

namespace AttechServer.Applications.UserModules.Dtos.News
{
    public class DocumentDetailDto
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
        public string NewsCategoryNameVi { get; set; } = string.Empty;
        public string NewsCategoryNameEn { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int? FeaturedImageId { get; set; }
        public AttachmentDto? FeaturedImage { get; set; }
        
        /// <summary>
        /// Danh sách tài liệu đính kèm
        /// </summary>
        public List<AttachmentDto> Documents { get; set; } = new();
        
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}