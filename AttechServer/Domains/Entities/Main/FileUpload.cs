using AttechServer.Shared.ApplicationBase.Common;

namespace AttechServer.Domains.Entities.Main
{
    public class FileUpload
    {
        public int Id { get; set; }
        public EntityType EntityType { get; set; }
        public int EntityId { get; set; }
        public string FilePath { get; set; }
        public string FileType { get; set; }
        public DateTime CreatedDate { get; set; }

    }
}
