using System.ComponentModel.DataAnnotations;

namespace AttechServer.Applications.UserModules.Dtos.Notification
{
    public class UpdateNotificationDto
    {
        // ID được truyền qua route parameter, không cần trong DTO

        [Required(ErrorMessage = "Tiêu đề là bắt buộc")]
        [StringLength(400, ErrorMessage = "Tiêu đề không được vượt quá 400 ký tự")]
        public string TitleVi { get; set; } = string.Empty;
        public string TitleEn { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mô tả là bắt buộc")]
        [StringLength(700, ErrorMessage = "Mô tả không được vượt quá 700 ký tự")]
        public string DescriptionVi { get; set; } = string.Empty;
        [StringLength(700, ErrorMessage = "Mô tả không được vượt quá 700 ký tự")]
        public string DescriptionEn { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nội dung là bắt buộc")]
        public string ContentVi { get; set; } = string.Empty;
        public string ContentEn { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Danh mục tin tức không hợp lệ")]
        public int NotificationCategoryId { get; set; }

        [Required(ErrorMessage = "Thời gian đăng bài là bắt buộc")]
        public DateTime TimePosted { get; set; }
        
        [Range(0, 1, ErrorMessage = "Trạng thái phải là 0 (không hoạt động) hoặc 1 (hoạt động)")]
        public int Status { get; set; }
        
        public bool IsOutstanding { get; set; } = false;
        
        [Required(ErrorMessage = "Slug tiếng Việt là bắt buộc")]
        public string SlugVi { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Slug tiếng Anh là bắt buộc")]
        public string SlugEn { get; set; } = string.Empty;

        // Attachment IDs - final desired state
        public int? FeaturedImageId { get; set; }
        public List<int>? AttachmentIds { get; set; }
    }
} 
