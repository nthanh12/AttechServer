using AttechServer.Domains.EntityBase;
using AttechServer.Domains.Schemas;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;

namespace AttechServer.Domains.Entities
{
    [Table(nameof(User), Schema = DbSchemas.Auth)]
    [Index(nameof(Deleted), nameof(Status), Name = $"IX_{nameof(User)}")]
    public class User : IFullAudited
    {
        public int Id { get; set; }
        [MaxLength(50)]
        public string Username { get; set; } = null!;
        [MaxLength(512)]
        public string Password { get; set; } = null!;

        /// <summary>
        /// Trạng thái
        /// <see cref="Status"/>
        /// </summary>
        public int Status { get; set; }
        public int UserType { get; set; }
        public List<UserRole> UserRoles { get; set; } = new();

        [MaxLength(512)]
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

        #region audit
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public bool Deleted { get; set; }
        #endregion
    }
}
