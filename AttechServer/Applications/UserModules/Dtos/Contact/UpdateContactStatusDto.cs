using System.ComponentModel.DataAnnotations;

namespace AttechServer.Applications.UserModules.Dtos.Contact
{
    public class UpdateContactStatusDto
    {
        [Required]
        [Range(0, 1, ErrorMessage = "Trạng thái phải là 0 (chưa đọc) hoặc 1 (đã đọc)")]
        public int Status { get; set; }
    }
}