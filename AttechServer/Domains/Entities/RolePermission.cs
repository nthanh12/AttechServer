using AttechServer.Domains.EntityBase;
using AttechServer.Domains.Schemas;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AttechServer.Domains.Entities
{
    [Table(nameof(RolePermission), Schema = DbSchemas.Auth)]
    [Index(nameof(Deleted), nameof(RoleId), nameof(PermissionKey), Name = $"IX_{nameof(RolePermission)}")]
    public class RolePermission : IFullAudited
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(128)]
        [Unicode(false)]
        public string PermissionKey { get; set; } = null!;
        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;

        #region audit
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public bool Deleted { get; set; }
        #endregion
    }
}
