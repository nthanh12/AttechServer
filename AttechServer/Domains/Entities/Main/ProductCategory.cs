﻿using AttechServer.Domains.EntityBase;
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
    [Index(nameof(SlugVi), IsUnique = true)]
    [Index(nameof(SlugEn), IsUnique = true)]
    public class ProductCategory : IFullAudited
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
