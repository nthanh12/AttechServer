using System.ComponentModel.DataAnnotations;

namespace AttechServer.Applications.UserModules.Dtos.Attachment
{
    public class UploadDto
    {
        [Required]
        public IFormFile File { get; set; } = null!;

        public string RelationType { get; set; } = "image";
    }

}