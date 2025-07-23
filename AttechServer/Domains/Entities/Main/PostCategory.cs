using AttechServer.Domains.EntityBase;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AttechServer.Domains.Entities.Main
{
    [Table(nameof(PostCategory))]
    [Index(
        nameof(Id),
        nameof(Deleted),
        Name = $"IX_{nameof(PostCategory)}",
        IsUnique = false
    )]
    [Index(nameof(SlugVi), IsUnique = true)]
    [Index(nameof(SlugEn), IsUnique = true)]
    public class PostCategory : IFullAudited
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string NameVi { get; set; } = string.Empty;
        [Required, StringLength(100)]
        public string NameEn { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string SlugVi { get; set; } = string.Empty;
        [Required, StringLength(100)]
        public string SlugEn { get; set; } = string.Empty;

        [StringLength(160)]
        public string DescriptionVi { get; set; } = string.Empty;
        [StringLength(160)]
        public string DescriptionEn { get; set; } = string.Empty;

        public int Status { get; set; }

        public PostType Type { get; set; }

        /// <summary>
        /// Khóa ngoại self‐reference
        /// </summary>
        public int? ParentId { get; set; }

        /// <summary>
        /// Navigation tới danh mục cha (nullable)
        /// </summary>
        public PostCategory? Parent { get; set; }

        /// <summary>
        /// Tập hợp các danh mục con
        /// </summary>
        public List<PostCategory> Children { get; set; } = new();

        public List<Post> Posts { get; set; } = new();

        #region audit
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public bool Deleted { get; set; }
        #endregion
    }
}
