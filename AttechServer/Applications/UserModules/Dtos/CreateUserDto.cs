using AttechServer.Shared.ApplicationBase.Common.Validations;
using AttechServer.Shared.Consts;
using System.ComponentModel.DataAnnotations;

namespace AttechServer.Applications.UserModules.Dtos
{
    public class CreateUserDto
    {
        private string _username = null!;

        [Required]
        [MaxLength(50)]
        public string Username
        {
            get => _username;
            set => _username = value.Trim();
        }
        
        private string _password = null!;
        [Required]
        [MinLength(6)]
        public string Password
        {
            get => _password;
            set => _password = value.Trim();
        }
        
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = null!;
        
        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = null!;
        
        [Phone]
        [MaxLength(20)]
        public string? Phone { get; set; }
        
        [IntegerRange(AllowableValues = new int[] { UserLevels.SYSTEM, UserLevels.MANAGER, UserLevels.STAFF })]
        public int UserLevel { get; set; }
        
        public int Status { get; set; } = 1; // Active by default
        
        public List<int> RoleIds { get; set; } = new();
    }
}
