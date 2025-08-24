using AttechServer.Domains.EntityBase;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AttechServer.Domains.Entities.Main
{
    [Table(nameof(Gallery))]
    [Index(
        nameof(Id),
        nameof(Deleted),
        Name = $"IX_{nameof(Gallery)}",
        IsUnique = false
    )]
    [Index(nameof(SlugVi), IsUnique = true)]
    [Index(nameof(SlugEn), IsUnique = true)]
    public class Gallery : IFullAudited
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

        [StringLength(700)]
        public string? DescriptionVi { get; set; }
        [StringLength(700)]
        public string? DescriptionEn { get; set; }

        public DateTime TimePosted { get; set; } = DateTime.Now;

        /// <summary>
        /// Trạng thái
        /// </summary>
        public int Status { get; set; } = 1; // Active by default

        public bool IsOutstanding { get; set; } = false;
        public string ImageUrl { get; set; } = string.Empty; // Cover image
        public int? FeaturedImageId { get; set; } // Cover image attachment ID

        #region audit
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public bool Deleted { get; set; }
        #endregion
    }
}