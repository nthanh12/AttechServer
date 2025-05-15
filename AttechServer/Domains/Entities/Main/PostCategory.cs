using AttechServer.Domains.EntityBase;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AttechServer.Domains.Entities.Main; // Đảm bảo đã thêm dòng này

namespace AttechServer.Domains.Entities.Main
{
    [Table(nameof(PostCategory))]
    [Index(
        nameof(Id),
        nameof(Deleted),
        Name = $"IX_{nameof(PostCategory)}",
        IsUnique = false
    )]
    [Index(nameof(Slug), IsUnique = true)]
    public class PostCategory : IFullAudited
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string Slug { get; set; } = string.Empty;

        [StringLength(160)]
        public string Description { get; set; } = string.Empty;

        public int Status { get; set; }

        public PostType Type { get; set; }

        public List<Post> Posts { get; set; } = new List<Post>();

        #region audit
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public bool Deleted { get; set; }
        #endregion
    }
}
