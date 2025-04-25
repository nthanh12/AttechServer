using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AttechServer.Domains.Entities.Main
{
    [Table(nameof(PostPCategory))]
    [Index(
        nameof(Id),
        Name = $"IX_{nameof(PostPCategory)}",
        IsUnique = false
    )]
    public class PostPCategory
    {
        [Key]
        public int Id { get; set; }
        public int PostId { get; set; }
        public Post Post { get; set; } = null!;
        public int PostCategoryId { get; set; }
        public PostCategory PostCategory { get; set; } = null!;
    }
}
