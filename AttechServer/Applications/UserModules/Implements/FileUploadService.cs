using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Domains.Entities.Main;
using AttechServer.Infrastructures.Persistances;
using AttechServer.Shared.ApplicationBase.Common;
using AttechServer.Shared.Configurations;
using AttechServer.Shared.Consts.Exceptions;
using AttechServer.Shared.Exceptions;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using System.Security.Cryptography;
using System.Text;

namespace AttechServer.Applications.UserModules.Implements
{
    public class FileUploadService : IFileUploadService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<FileUploadService> _logger;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;
        private readonly FileUploadOptions _options;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private static readonly HashSet<string> SuspiciousFileSignatures = new()
        {
            "4D5A", // PE executable
            "7F454C46", // ELF executable
            "504B0304", // ZIP (potential executable)
            "52617221", // RAR
            "D0CF11E0A1B11AE1", // MS Office (could contain macros)
        };

        public FileUploadService(
            ApplicationDbContext dbContext,
            ILogger<FileUploadService> logger,
            IWebHostEnvironment env,
            IConfiguration configuration,
            IOptions<FileUploadOptions> options,
            IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _logger = logger;
            _env = env;
            _configuration = configuration;
            _options = options.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<(string relativePath, string fileUrl)> UploadFileAsync(
            IFormFile file, 
            string fileType, 
            EntityType? entityType = null, 
            int? entityId = null)
        {
            try
            {
                // Validate file
                await ValidateFileAsync(file, fileType);

                // Generate secure file path
                var (uploadPath, relativePath, fileName) = GenerateSecureFilePath(file, fileType);

                // Process and save file
                await ProcessAndSaveFileAsync(file, uploadPath, fileName, fileType);

                // Save to database
                var fileEntry = new FileUpload
                {
                    EntityType = entityType ?? EntityType.Temp,
                    EntityId = entityId ?? 0,
                    FilePath = relativePath,
                    FileType = fileType,
                    OriginalFileName = SanitizeFileName(file.FileName),
                    FileSizeInBytes = file.Length,
                    ContentType = file.ContentType,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = GetCurrentUserId()
                };

                _dbContext.FileUploads.Add(fileEntry);
                await _dbContext.SaveChangesAsync();

                // Generate file URL
                var baseUrl = _configuration["ApplicationUrl"] ?? 
                             $"{_httpContextAccessor.HttpContext?.Request.Scheme}://{_httpContextAccessor.HttpContext?.Request.Host}";
                var fileUrl = $"{baseUrl}/api/upload/file/{relativePath}";

                _logger.LogInformation("File uploaded successfully: {FileName} -> {RelativePath}", 
                                     file.FileName, relativePath);

                return (relativePath, fileUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file: {FileName}", file.FileName);
                throw;
            }
        }

        public async Task<List<(string relativePath, string fileUrl)>> UploadMultipleFilesAsync(
            IFormFile[] files, 
            EntityType? entityType = null, 
            int? entityId = null)
        {
            if (files.Length > _options.MaxFilesPerRequest)
            {
                throw new UserFriendlyException(ErrorCode.TooManyFiles);
            }

            var results = new List<(string relativePath, string fileUrl)>();
            var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                foreach (var file in files)
                {
                    var fileType = DetermineFileType(file);
                    var result = await UploadFileAsync(file, fileType, entityType, entityId);
                    results.Add(result);
                }

                await transaction.CommitAsync();
                return results;
            }
            catch
            {
                await transaction.RollbackAsync();
                // Clean up any uploaded files
                foreach (var (relativePath, _) in results)
                {
                    try
                    {
                        var fullPath = Path.Combine(_env.ContentRootPath, _options.UploadBasePath, relativePath);
                        if (File.Exists(fullPath))
                            File.Delete(fullPath);
                    }
                    catch (Exception cleanupEx)
                    {
                        _logger.LogWarning(cleanupEx, "Failed to cleanup file: {RelativePath}", relativePath);
                    }
                }
                throw;
            }
        }

        private async Task ValidateFileAsync(IFormFile file, string fileType)
        {
            // Check if file type is allowed
            if (!_options.AllowedFileTypes.TryGetValue(fileType, out var config))
            {
                throw new UserFriendlyException(ErrorCode.InvalidFileType);
            }

            // Check file size
            var maxSizeBytes = config.MaxSizeInMB * 1024 * 1024;
            if (file.Length > maxSizeBytes)
            {
                throw new UserFriendlyException(ErrorCode.FileTooLarge);
            }

            // Check file extension
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!config.Extensions.Contains(extension))
            {
                throw new UserFriendlyException(ErrorCode.InvalidFileExtension);
            }

            // Check MIME type
            if (!config.MimeTypes.Contains(file.ContentType))
            {
                throw new UserFriendlyException(ErrorCode.InvalidMimeType);
            }

            // Basic malware scan
            if (config.ScanForMalware)
            {
                await ScanForMalwareAsync(file);
            }

            // Validate file content for images
            if (fileType == "images")
            {
                await ValidateImageFileAsync(file);
            }
        }

        private async Task ScanForMalwareAsync(IFormFile file)
        {
            // Read first 512 bytes to check file signature
            var buffer = new byte[512];
            using var stream = file.OpenReadStream();
            await stream.ReadAsync(buffer, 0, buffer.Length);
            stream.Position = 0;

            var hexSignature = Convert.ToHexString(buffer[..16]).ToUpperInvariant();
            
            // Check against suspicious signatures
            if (SuspiciousFileSignatures.Any(signature => hexSignature.StartsWith(signature)))
            {
                _logger.LogWarning("Suspicious file detected: {FileName}, Signature: {Signature}", 
                                 file.FileName, hexSignature);
                throw new UserFriendlyException(ErrorCode.SuspiciousFile);
            }

            // Additional checks for embedded scripts in images
            if (file.ContentType.StartsWith("image/"))
            {
                var content = Encoding.UTF8.GetString(buffer);
                var suspiciousPatterns = new[] { "<script", "javascript:", "vbscript:", "onload=", "onerror=" };
                
                if (suspiciousPatterns.Any(pattern => content.Contains(pattern, StringComparison.OrdinalIgnoreCase)))
                {
                    throw new UserFriendlyException(ErrorCode.SuspiciousFile);
                }
            }
        }

        private async Task ValidateImageFileAsync(IFormFile file)
        {
            try
            {
                using var image = await SixLabors.ImageSharp.Image.LoadAsync(file.OpenReadStream());
                
                // Check image dimensions (prevent decompression bombs)
                if (image.Width > 10000 || image.Height > 10000)
                {
                    throw new UserFriendlyException(ErrorCode.ImageTooLarge);
                }

                // Check if image is valid
                if (image.Width == 0 || image.Height == 0)
                {
                    throw new UserFriendlyException(ErrorCode.InvalidImage);
                }
            }
            catch (UnknownImageFormatException)
            {
                throw new UserFriendlyException(ErrorCode.InvalidImage);
            }
            catch (InvalidImageContentException)
            {
                throw new UserFriendlyException(ErrorCode.InvalidImage);
            }
        }

        private (string uploadPath, string relativePath, string fileName) GenerateSecureFilePath(IFormFile file, string fileType)
        {
            // Generate secure filename
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var fileName = $"{Guid.NewGuid()}{extension}";

            // Create date-based folder structure
            var now = DateTime.UtcNow;
            var datePath = Path.Combine(now.Year.ToString(), now.Month.ToString("D2"), now.Day.ToString("D2"));
            
            var uploadPath = Path.Combine(_env.ContentRootPath, _options.UploadBasePath, fileType, datePath);
            var relativePath = $"{fileType}/{datePath.Replace("\\", "/")}/{fileName}";

            // Ensure directory exists
            Directory.CreateDirectory(uploadPath);

            return (uploadPath, relativePath, fileName);
        }

        private async Task ProcessAndSaveFileAsync(IFormFile file, string uploadPath, string fileName, string fileType)
        {
            var filePath = Path.Combine(uploadPath, fileName);

            // Process images
            if (fileType == "images" && _options.AllowedFileTypes[fileType].ConvertToWebP)
            {
                using var image = await SixLabors.ImageSharp.Image.LoadAsync(file.OpenReadStream());
                
                // Resize if too large
                if (image.Width > 2000 || image.Height > 2000)
                {
                    image.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Size = new Size(2000, 2000),
                        Mode = ResizeMode.Max
                    }));
                }

                var webpFileName = Path.ChangeExtension(fileName, ".webp");
                var webpFilePath = Path.Combine(uploadPath, webpFileName);
                await image.SaveAsWebpAsync(webpFilePath);
            }
            else
            {
                // Save file as-is
                using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                await file.CopyToAsync(fileStream);
            }
        }

        private string DetermineFileType(IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            
            foreach (var (fileType, config) in _options.AllowedFileTypes)
            {
                if (config.Extensions.Contains(extension))
                    return fileType;
            }

            throw new UserFriendlyException(ErrorCode.InvalidFileType);
        }

        private string SanitizeFileName(string fileName)
        {
            // Remove dangerous characters
            var invalidChars = Path.GetInvalidFileNameChars();
            var sanitized = new string(fileName.Where(c => !invalidChars.Contains(c)).ToArray());
            
            // Limit length
            if (sanitized.Length > 255)
                sanitized = sanitized.Substring(0, 255);

            return sanitized;
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("Id")?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : null;
        }
    }
}