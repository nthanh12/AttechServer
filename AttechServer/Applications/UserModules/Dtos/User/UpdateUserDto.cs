using AttechServer.Shared.ApplicationBase.Common.Validations;
using AttechServer.Shared.Consts;
using System.ComponentModel.DataAnnotations;

namespace AttechServer.Applications.UserModules.Dtos.User
{
    public class UpdateUserDto
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = null!;
        
        [MaxLength(100)]
        public string? FullName { get; set; }
        
        [MaxLength(100)]
        public string? Email { get; set; }
        
        [MaxLength(20)]
        public string? Phone { get; set; }
        
        public int Status { get; set; }
        
        [IntegerRange(AllowableValues = new int[] { UserLevels.SYSTEM, UserLevels.MANAGER, UserLevels.STAFF })]
        public int UserLevel { get; set; }
        
        public List<int> RoleIds { get; set; } = new();
    }
}
