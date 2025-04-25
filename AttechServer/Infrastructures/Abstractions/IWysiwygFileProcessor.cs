using AttechServer.Shared.ApplicationBase.Common;

namespace AttechServer.Infrastructures.Abstractions
{
    public interface IWysiwygFileProcessor
    {
        Task<(string ProcessedContent, List<FileEntry> FileEntries)> ProcessContentAsync(string content, EntityType entityType, int entityId);
        Task<string> ReconstructContentAsync(string content);
        Task DeleteFilesAsync(EntityType entityType, int entityId);
        Task CleanTempFilesAsync();
    }
    public class FileEntry
    {
        public string SubFolder { get; set; }
        public string FilePath { get; set; }
    }
}
