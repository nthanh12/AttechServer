using AttechServer.Domains.EntityBase;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AttechServer.Domains.Entities.Main
{
    [Table(nameof(Contact))]
    [Index(
        nameof(Id),
        nameof(Deleted),
        Name = $"IX_{nameof(Contact)}",
        IsUnique = false
    )]
    [Index(nameof(Email))]
    [Index(nameof(Status))]
    [Index(nameof(SubmittedAt))]
    public class Contact : IFullAudited
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(255)]
        public string Name { get; set; } = string.Empty;

        [Required, StringLength(255)]
        public string Email { get; set; } = string.Empty;

        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        [Required, StringLength(500)]
        public string Subject { get; set; } = string.Empty;

        [Required, StringLength(5000)]
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Trạng thái: 0 = Chưa đọc, 1 = Đã đọc
        /// </summary>
        public int Status { get; set; } = 0;

        /// <summary>
        /// Thời gian gửi liên hệ
        /// </summary>
        public DateTime SubmittedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// IP Address của người gửi (optional, cho security tracking)
        /// </summary>
        [StringLength(45)] // IPv6 max length
        public string? IpAddress { get; set; }

        /// <summary>
        /// User Agent của browser (optional, cho security tracking)
        /// </summary>
        [StringLength(500)]
        public string? UserAgent { get; set; }

        // IFullAudited properties
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ModifiedDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public bool Deleted { get; set; } = false;
    }
}