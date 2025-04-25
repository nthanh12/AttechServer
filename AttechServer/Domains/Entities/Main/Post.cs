using AttechServer.Domains.EntityBase;
using AttechServer.Shared.ApplicationBase.Common.Validations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AttechServer.Domains.Entities.Main
{
    [Table(nameof(Post))]
    [Index(
        nameof(Id),
        nameof(Deleted),
        Name = $"IX_{nameof(Post)}",
        IsUnique = false
    )]
    public class Post : IFullAudited
    {
        [Key]
        public int Id { get; set; }
        public string Slug { get; set; } = string.Empty;
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Content { get; set; } = string.Empty;
        public DateTime TimePosted { get; set; }

        /// <summary>
        /// Trạng thái
        /// <see cref="PostStatuses"/>
        /// </summary>
        public int Status { get; set; }  
        /// <summary>
        /// Loại bài viết
        /// </summary>
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
