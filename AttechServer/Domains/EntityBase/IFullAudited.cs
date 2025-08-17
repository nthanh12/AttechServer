namespace AttechServer.Domains.EntityBase
{
    public interface IFullAudited : ICreatedBy, IModifiedBy, ISoftDeleted
    {
    }

    public interface ISoftDeleted
    {
        public bool Deleted { get; set; }
    }

    public interface ICreatedBy
    {
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
    }

    public interface IModifiedBy
    {
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
    }
}
