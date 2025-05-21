using AttechServer.Shared.ApplicationBase.Common;

namespace AttechServer.Applications.UserModules.Dtos.FileUpload
{
    public class FileUploadDto
    {
        public int Id { get; set; }
        public string FileName { get; set; } = null!;
        public string FileType { get; set; } = null!;
        public string FilePath { get; set; } = null!;
        public EntityType EntityType { get; set; }
        public int EntityId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}