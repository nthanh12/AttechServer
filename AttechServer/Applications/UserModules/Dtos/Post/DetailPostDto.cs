using System.ComponentModel.DataAnnotations;

namespace AttechServer.Applications.UserModules.Dtos.Post
{
    public class DetailPostDto
    {
        public int Id { get; set; }

        [StringLength(200)]
        public string TitleVi { get; set; } = string.Empty;
        public string TitleEn { get; set; } = string.Empty;

        [StringLength(200)]
        public string SlugVi { get; set; } = string.Empty;
        public string SlugEn { get; set; } = string.Empty;

        [StringLength(160)]
        public string DescriptionVi { get; set; } = string.Empty;
        public string DescriptionEn { get; set; } = string.Empty;

        public string ContentVi { get; set; } = string.Empty;
        public string ContentEn { get; set; } = string.Empty;

        public DateTime TimePosted { get; set; }

        public int Status { get; set; }

        [Range(1, int.MaxValue)]
        public int PostCategoryId { get; set; }

        [StringLength(100)]
        public string PostCategoryName { get; set; } = string.Empty;

        [StringLength(100)]
        public string PostCategorySlug { get; set; } = string.Empty;
        public bool isOutstanding { get; set; } = false;
        public string ImageUrl { get; set; } = string.Empty;
    }
}