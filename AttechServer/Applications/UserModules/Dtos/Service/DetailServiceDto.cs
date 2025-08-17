
using AttechServer.Applications.UserModules.Dtos.Attachment;

namespace AttechServer.Applications.UserModules.Dtos.Service
{
    public class DetailServiceDto
    {
        public int Id { get; set; }
        public string SlugVi { get; set; } = string.Empty;
        public string TitleVi { get; set; } = string.Empty;
        public string DescriptionVi { get; set; } = string.Empty;
        public string ContentVi { get; set; } = string.Empty;
        public string SlugEn { get; set; } = string.Empty;
        public string TitleEn { get; set; } = string.Empty;
        public string DescriptionEn { get; set; } = string.Empty;
        public string ContentEn { get; set; } = string.Empty;
        public DateTime TimePosted { get; set; }
        public int Status { get; set; }
        public bool IsOutstanding { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public int? FeaturedImageId { get; set; }
        
        // Attachments for detail view
        public AttachmentsGroupDto Attachments { get; set; } = new();
    }
} 
