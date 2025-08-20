using System.ComponentModel.DataAnnotations;

namespace AttechServer.Applications.UserModules.Dtos.Setting
{
    public class UploadSettingDto
    {
        [Required]
        public IFormFile File { get; set; } = null!;
    }
}