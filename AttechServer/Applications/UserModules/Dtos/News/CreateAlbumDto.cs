using System.ComponentModel.DataAnnotations;

namespace AttechServer.Applications.UserModules.Dtos.News
{
    public class CreateAlbumDto
    {
        [Required]
        public string TitleVi { get; set; } = string.Empty;
        public string TitleEn { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Danh mục tin tức không hợp lệ")]
        public int NewsCategoryId { get; set; }

        // Album images (required) - ảnh đầu tiên tự động làm featured
        [Required]
        public List<int> AttachmentIds { get; set; } = new();
    }
}