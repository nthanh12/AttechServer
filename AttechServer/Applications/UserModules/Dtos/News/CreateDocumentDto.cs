using System.ComponentModel.DataAnnotations;

namespace AttechServer.Applications.UserModules.Dtos.News
{
    public class CreateDocumentDto
    {
        [Required(ErrorMessage = "Tiêu đề tiếng Việt là bắt buộc")]
        public string TitleVi { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Tiêu đề tiếng Anh là bắt buộc")]
        public string TitleEn { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Danh mục tin tức không hợp lệ")]
        public int NewsCategoryId { get; set; }

        // Featured image (optional for document)
        public int? FeaturedImageId { get; set; }

        // Document files (required for document)
        [Required(ErrorMessage = "Ít nhất một tài liệu là bắt buộc")]
        public List<int> AttachmentIds { get; set; } = new();
    }
}