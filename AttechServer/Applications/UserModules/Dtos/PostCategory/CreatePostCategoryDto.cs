using AttechServer.Shared.Consts;
using System.ComponentModel.DataAnnotations;

namespace AttechServer.Applications.UserModules.Dtos.PostCategory
{
    public class CreatePostCategoryDto
    {
        [Required(ErrorMessage = "Tên danh mục là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên danh mục không được vượt quá 100 ký tự")]
        public string Name { get; set; } = string.Empty;

        [StringLength(160, ErrorMessage = "Mô tả không được vượt quá 160 ký tự")]
        public string Description { get; set; } = string.Empty;

        public int Status { get; set; } = CommonStatus.ACTIVE;
    }
}