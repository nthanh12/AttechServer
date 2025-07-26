using AttechServer.Shared.ApplicationBase.Common;

namespace AttechServer.Applications.UserModules.Abstracts
{
    public interface IFileUploadService
    {
        Task<(string relativePath, string fileUrl)> UploadFileAsync(
            IFormFile file, 
            string fileType, 
            EntityType? entityType = null, 
            int? entityId = null);

        Task<List<(string relativePath, string fileUrl)>> UploadMultipleFilesAsync(
            IFormFile[] files, 
            EntityType? entityType = null, 
            int? entityId = null);
    }
}