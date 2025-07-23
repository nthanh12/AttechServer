using System.ComponentModel.DataAnnotations;

namespace AttechServer.Applications.UserModules.Dtos.Product
{
    public class CreateProductDto
    {
        [Required(ErrorMessage = "Tiêu đề là bắt buộc")]
        [StringLength(200, ErrorMessage = "Tiêu đề không được vượt quá 200 ký tự")]
        public string NameVi { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public string SlugVi { get; set; } = string.Empty;
        public string SlugEn { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mô tả là bắt buộc")]
        [StringLength(160, ErrorMessage = "Mô tả không được vượt quá 160 ký tự")]
        public string DescriptionVi { get; set; } = string.Empty;
        public string DescriptionEn { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nội dung là bắt buộc")]
        public string ContentVi { get; set; } = string.Empty;
        public string ContentEn { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Danh mục không hợp lệ")]
        public int ProductCategoryId { get; set; }

        [Required(ErrorMessage = "Thời gian đăng bài là bắt buộc")]
        public DateTime TimePosted { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
    }
}