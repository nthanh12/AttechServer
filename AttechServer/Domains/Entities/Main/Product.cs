using AttechServer.Domains.EntityBase;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AttechServer.Domains.Entities.Main
{
    [Table(nameof(Product))]
    [Index(
        nameof(Id),
        nameof(Deleted),
        Name = $"IX_{nameof(Product)}",
        IsUnique = false
    )]
    [Index(nameof(Slug), IsUnique = true)]
    [Index(nameof(ProductCategoryId))]
    public class Product : IFullAudited
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(200)]
        public string Slug { get; set; } = string.Empty;

        [Required, StringLength(200)]
        public string Name { get; set; } = null!;

        [Required, StringLength(160)]
        public string Description { get; set; } = null!;

        [Required]
        public string Content { get; set; } = string.Empty;

        public DateTime TimePosted { get; set; }

        /// <summary>
        /// Trạng thái
        /// <see cref="ProductStatuses"/>
        /// </summary>
        public int Status { get; set; }

        [Range(1, int.MaxValue)]
        public int ProductCategoryId { get; set; }

        public ProductCategory ProductCategory { get; set; } = null!;

        #region audit
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public bool Deleted { get; set; }
        #endregion
    }
}
