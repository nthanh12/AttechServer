using System.ComponentModel.DataAnnotations;

namespace AttechServer.Applications.UserModules.Dtos.News
{
    public class UpdateDocumentDto
    {
        [Required(ErrorMessage = "Tiêu đề tiếng Việt là bắt buộc")]
        public string TitleVi { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Tiêu đề tiếng Anh là bắt buộc")]
        public string TitleEn { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Danh mục tin tức không hợp lệ")]
        public int NewsCategoryId { get; set; }

        // Featured image (optional for document)
        public int? FeaturedImageId { get; set; }

        // Document files (optional - null means no change, empty list means clear all)
        public List<int>? AttachmentIds { get; set; }

        /// <summary>
        /// Trạng thái
        /// 0 = Nháp, 1 = Xuất bản, 2 = Ẩn
        /// </summary>
        [Range(0, 2, ErrorMessage = "Trạng thái không hợp lệ")]
        public int Status { get; set; } = 0;
    }
}