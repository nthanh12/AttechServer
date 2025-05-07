using System.ComponentModel.DataAnnotations;

namespace AttechServer.Applications.UserModules.Dtos.Post
{
    public class PostDto
    {
        public int Id { get; set; }

        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(200)]
        public string Slug { get; set; } = string.Empty;

        [StringLength(160)]
        public string Description { get; set; } = string.Empty;

        public DateTime TimePosted { get; set; }

        public int Status { get; set; }

        [Range(1, int.MaxValue)]
        public int PostCategoryId { get; set; }

        [StringLength(100)]
        public string PostCategoryName { get; set; } = string.Empty;

        [StringLength(100)]
        public string PostCategorySlug { get; set; } = string.Empty;
    }
}