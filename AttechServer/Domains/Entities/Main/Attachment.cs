using AttechServer.Domains.EntityBase;
using AttechServer.Shared.ApplicationBase.Common;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AttechServer.Domains.Entities.Main
{
    [Table(nameof(Attachment))]
    [Index(
        nameof(Id),
        nameof(Deleted),
        Name = $"IX_{nameof(Attachment)}",
        IsUnique = false
    )]
    [Index(nameof(ObjectType), nameof(ObjectId))]
    [Index(nameof(IsTemporary))]
    public class Attachment : IFullAudited
    {
        [Key]
        public int Id { get; set; }
        
        [Required, StringLength(500)]
        public string FilePath { get; set; } = string.Empty;

        [Required, StringLength(500)]
        public string Url { get; set; } = string.Empty;
        
        [StringLength(255)]
        public string OriginalFileName { get; set; } = string.Empty;
        
        public long FileSize { get; set; }
        
        [StringLength(100)]
        public string ContentType { get; set; } = string.Empty;

        public ObjectType? ObjectType { get; set; }
        public int? ObjectId { get; set; }
        
        [StringLength(50)]
        public string RelationType { get; set; } = "image";
        
        public bool IsPrimary { get; set; } = false;
        
        public bool IsContentImage { get; set; } = false;
        
        public bool IsTemporary { get; set; } = true;

        public int OrderIndex { get; set; } = 0;

        #region IFullAudited
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public bool Deleted { get; set; }
        #endregion
    }
}
