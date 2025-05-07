using System.ComponentModel.DataAnnotations;

namespace AttechServer.Applications.UserModules.Dtos.Service
{
    public class CreateServiceDto
    {
        [Required(ErrorMessage = "Tiêu đề là bắt buộc")]
        [StringLength(200, ErrorMessage = "Tiêu đề không được vượt quá 200 ký tự")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mô tả là bắt buộc")]
        [StringLength(160, ErrorMessage = "Mô tả không được vượt quá 160 ký tự")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nội dung là bắt buộc")]
        public string Content { get; set; } = string.Empty;

        [Required(ErrorMessage = "Thời gian đăng bài là bắt buộc")]
        public DateTime TimePosted { get; set; }
    }
}