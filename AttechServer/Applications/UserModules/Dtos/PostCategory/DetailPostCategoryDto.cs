using AttechServer.Applications.UserModules.Dtos.Post;
using System.ComponentModel.DataAnnotations;

namespace AttechServer.Applications.UserModules.Dtos.PostCategory
{
    public class DetailPostCategoryDto
    {
        public int Id { get; set; }

        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(100)]
        public string Slug { get; set; } = string.Empty;

        [StringLength(160)]
        public string Description { get; set; } = string.Empty;

        public int Status { get; set; }

        public List<PostDto> Posts { get; set; } = new List<PostDto>();
    }
}