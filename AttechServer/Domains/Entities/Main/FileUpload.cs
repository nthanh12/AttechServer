using AttechServer.Domains.EntityBase;
using AttechServer.Shared.ApplicationBase.Common;
using System.ComponentModel.DataAnnotations;

namespace AttechServer.Domains.Entities.Main
{
    public class FileUpload : IFullAudited
    {
        [Key]
        public int Id { get; set; }
        
        public EntityType EntityType { get; set; }
        public int EntityId { get; set; }
        
        [Required, StringLength(500)]
        public string FilePath { get; set; } = string.Empty;
        
        [Required, StringLength(50)]
        public string FileType { get; set; } = string.Empty;
        
        [StringLength(255)]
        public string OriginalFileName { get; set; } = string.Empty;
        
        public long FileSizeInBytes { get; set; }
        
        [StringLength(100)]
        public string ContentType { get; set; } = string.Empty;
        
        [StringLength(64)]
        public string? FileHash { get; set; }
        
        public bool IsScanned { get; set; } = false;
        public bool IsSafe { get; set; } = true;

        #region IFullAudited
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public bool Deleted { get; set; }
        #endregion
    }
}
