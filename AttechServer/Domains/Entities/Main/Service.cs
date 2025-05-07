using System.ComponentModel.DataAnnotations;

namespace AttechServer.Domains.Entities.Main
{
    public class Service
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
