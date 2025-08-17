using AttechServer.Domains.EntityBase;

namespace AttechServer.Domains.Entities.Main
{
    public class SystemMonitoring : Entity, ICreatedBy
    {
        public string MetricName { get; set; } = string.Empty;
        public double Value { get; set; }
        public string Unit { get; set; } = string.Empty; // MB, GB, %, ms, etc.
        public string Category { get; set; } = string.Empty; // Performance, Storage, Network, etc.
        public DateTime RecordedAt { get; set; } = DateTime.Now;
        public string? Description { get; set; }
        public bool IsAlert { get; set; } = false;
        public double? ThresholdValue { get; set; }
        
        // Audit fields
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public bool Deleted { get; set; } = false;
    }
}
