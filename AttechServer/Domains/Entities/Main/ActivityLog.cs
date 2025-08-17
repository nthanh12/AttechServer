using AttechServer.Domains.EntityBase;

namespace AttechServer.Domains.Entities.Main
{
    public class ActivityLog : Entity, ICreatedBy
    {
        public string Type { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? Details { get; set; }
        public string Severity { get; set; } = "Info"; // Info, Warning, Error
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
        
        // Audit fields
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public bool Deleted { get; set; } = false;
    }
}
