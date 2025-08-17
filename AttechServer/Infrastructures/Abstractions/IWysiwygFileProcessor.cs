using AttechServer.Shared.ApplicationBase.Common;

namespace AttechServer.Infrastructures.Abstractions
{
    public interface IWysiwygFileProcessor
    {
        Task<(string ProcessedContent, List<FileEntry> FileEntries)> ProcessContentAsync(string content, ObjectType objectType, int objectId);
        Task<string> ReconstructContentAsync(string content);
        Task DeleteFilesAsync(ObjectType objectType, int objectId);
        Task ProcessAttachmentsAsync(string content, ObjectType objectType, int objectId);
        Task<List<int>> ExtractAttachmentIdsAsync(string content);
        Task<List<int>> ExtractUniqueAttachmentIdsAsync(string contentVi, string contentEn);
        bool HasTempFilesInContent(string content);
    }
    public class FileEntry
    {
        public string SubFolder { get; set; }
        public string FilePath { get; set; }
    }
}
