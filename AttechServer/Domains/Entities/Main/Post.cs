using AttechServer.Domains.EntityBase;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AttechServer.Domains.Entities.Main; // Thêm dòng này

namespace AttechServer.Domains.Entities.Main
{
    [Table(nameof(Post))]
    [Index(
        nameof(Id),
        nameof(Deleted),
        Name = $"IX_{nameof(Post)}",
        IsUnique = false
    )]
    [Index(nameof(Slug), IsUnique = true)]
    [Index(nameof(PostCategoryId))]
    public class Post : IFullAudited
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(200)]
        public string Slug { get; set; } = string.Empty;

        [Required, StringLength(200)]
        public string Title { get; set; } = null!;

        [Required, StringLength(160)]
        public string Description { get; set; } = null!;

        [Required]
        public string Content { get; set; } = string.Empty;

        public DateTime TimePosted { get; set; }

        /// <summary>
        /// Trạng thái
        /// <see cref="PostStatuses"/>
        /// </summary>
        public int Status { get; set; }

        public PostType Type { get; set; }

        [Range(1, int.MaxValue)]
        public int PostCategoryId { get; set; }

        public PostCategory PostCategory { get; set; } = null!;

        #region audit
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public bool Deleted { get; set; }
        #endregion
    }
}
