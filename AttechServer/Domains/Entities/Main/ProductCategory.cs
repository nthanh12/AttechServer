using AttechServer.Domains.EntityBase;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AttechServer.Domains.Entities.Main
{
    [Table(nameof(ProductCategory))]
    [Index(
        nameof(Id),
        nameof(Deleted),
        Name = $"IX_{nameof(ProductCategory)}",
        IsUnique = false
    )]
    [Index(nameof(Slug), IsUnique = true)]
    public class ProductCategory : IFullAudited
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

        public List<Product> Products { get; set; } = new List<Product>();

        #region audit
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public bool Deleted { get; set; }
        #endregion
    }
}
