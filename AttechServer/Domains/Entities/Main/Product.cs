using AttechServer.Domains.EntityBase;
using AttechServer.Shared.Consts;
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
    [Index(nameof(SlugVi), IsUnique = true)]
    [Index(nameof(SlugEn), IsUnique = true)]
    [Index(nameof(ProductCategoryId))]
    public class Product : IFullAudited
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(200)]
        public string SlugVi { get; set; } = string.Empty;
        [Required, StringLength(200)]
        public string SlugEn { get; set; } = string.Empty;

        [Required, StringLength(200)]
        public string TitleVi { get; set; } = null!;
        [Required, StringLength(200)]
        public string TitleEn { get; set; } = null!;

        [Required, StringLength(160)]
        public string DescriptionVi { get; set; } = null!;

        [Required, StringLength(160)]
        public string DescriptionEn { get; set; } = null!;

        [Required]
        public string ContentVi { get; set; } = string.Empty;
        [Required]
        public string ContentEn { get; set; } = string.Empty;

        public DateTime TimePosted { get; set; }

        /// <summary>
        /// Trạng thái
        /// <see cref="CommonStatus"/>
        /// </summary>
        public int Status { get; set; }

        [Range(1, int.MaxValue)]
        public int ProductCategoryId { get; set; }

        public ProductCategory ProductCategory { get; set; } = null!;
        public bool IsOutstanding { get; set; } = false;
        public string ImageUrl { get; set; } = string.Empty;
        public int? FeaturedImageId { get; set; }

        #region audit
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public bool Deleted { get; set; }
        #endregion
    }
}
