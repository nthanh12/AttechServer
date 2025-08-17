using AttechServer.Domains.EntityBase;
using AttechServer.Domains.Schemas;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AttechServer.Domains.Entities
{
    [Table(nameof(PermissionForApiEndpoint), Schema = DbSchemas.Auth)]
    [Index(nameof(Deleted), nameof(ApiEndpointId), nameof(PermissionId), Name = $"IX_{nameof(PermissionForApiEndpoint)}")]
    public class PermissionForApiEndpoint : IFullAudited
    {
        [Key]
        public int Id { get; set; }

        public int ApiEndpointId { get; set; }
        public ApiEndpoint ApiEndpoint { get; set; } = null!;

        public int PermissionId { get; set; }
        public Permission Permission { get; set; } = null!;

        public bool IsRequired { get; set; } = true;

        #region audit
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public bool Deleted { get; set; }
        #endregion
    }
}
