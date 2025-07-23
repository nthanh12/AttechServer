using AngleSharp;
using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Domains.Entities.Main;
using AttechServer.Infrastructures.Abstractions;
using AttechServer.Infrastructures.Persistances;
using AttechServer.Shared.ApplicationBase.Common;
using Microsoft.EntityFrameworkCore;

namespace AttechServer.Infrastructures.ContentProcessing
{
    public class WysiwygFileProcessor : IWysiwygFileProcessor
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<WysiwygFileProcessor> _logger;
        private readonly IWebHostEnvironment _env;

        public WysiwygFileProcessor(
            ApplicationDbContext dbContext,
            IHttpContextAccessor httpContextAccessor,
            ILogger<WysiwygFileProcessor> logger,
            IWebHostEnvironment env)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _env = env ?? throw new ArgumentNullException(nameof(env));
        }

        public async Task<(string ProcessedContent, List<FileEntry> FileEntries)> ProcessContentAsync(string content, EntityType entityType, int entityId)
        {
            if (string.IsNullOrEmpty(content))
            {
                _logger.LogWarning("Content is null or empty.");
                return (content, new List<FileEntry>());
            }

            var fileEntries = new List<FileEntry>();

            var config = AngleSharp.Configuration.Default;
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(req => req.Content(content));

            var elementsWithSrc = document.QuerySelectorAll("img[src], video[src], source[src], a[href], embed[src], iframe[src]")
                .Where(e => e.GetAttribute("src")?.StartsWith("/api/upload/file/") == true ||
                            e.GetAttribute("href")?.StartsWith("/api/upload/file/") == true);

            foreach (var element in elementsWithSrc)
            {
                var url = element.GetAttribute("src") ?? element.GetAttribute("href");
                if (string.IsNullOrEmpty(url)) continue;

                var segments = url.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                if (segments.Length < 7)
                {
                    _logger.LogWarning($"Invalid file URL format: {url}");
                    continue;
                }

                var subFolder = segments[3];
                var datePath = Path.Combine(segments[4], segments[5], segments[6]);
                var fileName = segments[7];
                var relativePath = Path.Combine(subFolder, datePath, fileName);

                var filePath = Path.Combine(_env.ContentRootPath, "Uploads", relativePath);
                if (!System.IO.File.Exists(filePath))
                {
                    _logger.LogWarning($"File not found: {filePath}");
                    continue;
                }

                fileEntries.Add(new FileEntry { SubFolder = subFolder, FilePath = relativePath });

                if (element.TagName == "IMG" || element.TagName == "VIDEO")
                {
                    element.SetAttribute("data-src", relativePath);
                    element.SetAttribute("src", "placeholder.jpg");
                }
                else if (element.TagName == "A")
                {
                    element.SetAttribute("href", relativePath);
                }
                else
                {
                    element.SetAttribute("src", relativePath);
                }

                var fileEntryInDb = await _dbContext.FileUploads
                    .FirstOrDefaultAsync(f => f.FilePath == relativePath && f.EntityType == EntityType.Temp);
                if (fileEntryInDb != null)
                {
                    fileEntryInDb.EntityType = entityType;
                    fileEntryInDb.EntityId = entityId;
                }
            }

            await _dbContext.SaveChangesAsync();

            return (document.DocumentElement.OuterHtml, fileEntries);
        }

        public async Task<string> ReconstructContentAsync(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                _logger.LogWarning("Content is null or empty.");
                return content;
            }

            var domain = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
            var config = AngleSharp.Configuration.Default;
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(req => req.Content(content));

            var elementsWithSrc = document.QuerySelectorAll("img[data-src], video[data-src], source[src], a[href], embed[src], iframe[src]");
            foreach (var element in elementsWithSrc)
            {
                var attrName = element.TagName == "A" ? "href" : element.HasAttribute("data-src") ? "data-src" : "src";
                var relativePath = element.GetAttribute(attrName);
                if (relativePath?.Contains("/api/upload/file/") == true) continue;

                if (!string.IsNullOrEmpty(relativePath))
                {
                    var fullUrl = $"{domain}/api/upload/file/{relativePath}";
                    if (element.HasAttribute("data-src"))
                    {
                        element.SetAttribute("data-src", fullUrl);
                        element.SetAttribute("src", "placeholder.jpg");
                    }
                    else if (element.TagName == "A")
                    {
                        element.SetAttribute("href", fullUrl);
                    }
                    else
                    {
                        element.SetAttribute("src", fullUrl);
                    }
                }
            }

            return document.DocumentElement.OuterHtml;
        }

        public async Task DeleteFilesAsync(EntityType entityType, int entityId)
        {
            var files = await _dbContext.FileUploads
                .Where(f => f.EntityType == entityType && f.EntityId == entityId)
                .ToListAsync();

            foreach (var file in files)
            {
                var filePath = Path.Combine(_env.ContentRootPath, "Uploads", file.FilePath);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                    _logger.LogInformation($"Deleted file: {filePath}");
                }
            }

            _dbContext.FileUploads.RemoveRange(files);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation($"Deleted {files.Count} files for {entityType} ID {entityId}");
        }

        public async Task CleanTempFilesAsync()
        {
            var threshold = DateTime.Now.AddDays(-1);
            var tempFiles = await _dbContext.FileUploads
                .Where(f => f.EntityType == EntityType.Temp && f.CreatedDate < threshold)
                .ToListAsync();

            foreach (var file in tempFiles)
            {
                var filePath = Path.Combine(_env.ContentRootPath, "Uploads", file.FilePath);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                    _logger.LogInformation($"Deleted temp file: {filePath}");
                }
            }

            _dbContext.FileUploads.RemoveRange(tempFiles);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation($"Cleaned {tempFiles.Count} temporary files.");
        }
    }
}
