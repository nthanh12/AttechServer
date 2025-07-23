using System.ComponentModel.DataAnnotations;

namespace AttechServer.Domains.Entities.Main
{
    public class Service
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(200)]
        public string SlugVi { get; set; } = string.Empty;
        [Required, StringLength(200)]
        public string SlugEn { get; set; } = string.Empty;
        [Required, StringLength(200)]
        public string NameVi { get; set; } = null!;
        [Required, StringLength(200)]
        public string NameEn { get; set; } = null!;

        [Required, StringLength(160)]
        public string DescriptionVi { get; set; } = null!;
        [Required, StringLength(160)]
        public string DescriptionEn { get; set; } = null!;

        [Required]
        public string ContentVi { get; set; } = string.Empty;
        [Required]
        public string ContentEn { get; set; } = string.Empty;

        public DateTime TimePosted { get; set; }
        public string ImageUrl { get; set; } = string.Empty;

        /// <summary>
        /// Trạng thái
        /// <see cref="ServiceStatuses"/>
        /// </summary>
        public int Status { get; set; }

        #region audit
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public bool Deleted { get; set; }
        #endregion
    }
}
