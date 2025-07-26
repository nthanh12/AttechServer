using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Shared.ApplicationBase.Common;
using AttechServer.Shared.Consts.Permissions;
using AttechServer.Shared.Filters;
using AttechServer.Shared.WebAPIBase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AttechServer.Controllers
{
    [Route("api/upload")]
    [ApiController]
    [Authorize]
    public class UploadController : ApiControllerBase
    {
        private readonly IFileUploadService _fileUploadService;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;

        public UploadController(
            ILogger<UploadController> logger, 
            IFileUploadService fileUploadService,
            IWebHostEnvironment env,
            IConfiguration configuration) : base(logger)
        {
            _fileUploadService = fileUploadService;
            _env = env;
            _configuration = configuration;
        }

        /// <summary>
        /// Upload multiple files at once
        /// </summary>
        /// <param name="files">Files to upload</param>
        /// <param name="entityType">Entity type for file association</param>
        /// <param name="entityId">Entity ID for file association</param>
        /// <returns>List of uploaded file URLs</returns>

        [HttpPost("multi-upload")]
        [PermissionFilter(PermissionKeys.FileUpload)]
        public async Task<ApiResponse> MultiUpload(IFormFile[] files, [FromQuery] EntityType? entityType = null, [FromQuery] int? entityId = null)
        {
            try
            {
                if (files == null || !files.Any())
                {
                    return new ApiResponse(ApiStatusCode.Error, null, 400, "Không có file nào được tải lên");
                }

                var results = await _fileUploadService.UploadMultipleFilesAsync(files, entityType, entityId);
                var fileUrls = results.Select(r => r.fileUrl).ToList();
                
                return new ApiResponse(ApiStatusCode.Success, new { locations = fileUrls }, 200, "Ok");
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Upload image file
        /// </summary>
        [HttpPost("image")]
        [PermissionFilter(PermissionKeys.FileUpload)]
        public async Task<ApiResponse> UploadImage(IFormFile file, [FromQuery] EntityType? entityType = null, [FromQuery] int? entityId = null)
        {
            try
            {
                if (file == null)
                {
                    return new ApiResponse(ApiStatusCode.Error, null, 400, "Không có file nào được tải lên");
                }

                var (_, fileUrl) = await _fileUploadService.UploadFileAsync(file, "images", entityType, entityId);
                return new ApiResponse(ApiStatusCode.Success, new { location = fileUrl }, 200, "Ok");
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Upload video file
        /// </summary>
        [HttpPost("video")]
        [PermissionFilter(PermissionKeys.FileUpload)]
        public async Task<ApiResponse> UploadVideo(IFormFile file, [FromQuery] EntityType? entityType = null, [FromQuery] int? entityId = null)
        {
            try
            {
                if (file == null)
                {
                    return new ApiResponse(ApiStatusCode.Error, null, 400, "Không có file nào được tải lên");
                }

                var (_, fileUrl) = await _fileUploadService.UploadFileAsync(file, "videos", entityType, entityId);
                return new ApiResponse(ApiStatusCode.Success, new { location = fileUrl }, 200, "Ok");
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Upload document file
        /// </summary>
        [HttpPost("document")]
        [PermissionFilter(PermissionKeys.FileUpload)]
        public async Task<ApiResponse> UploadDocument(IFormFile file, [FromQuery] EntityType? entityType = null, [FromQuery] int? entityId = null)
        {
            try
            {
                if (file == null)
                {
                    return new ApiResponse(ApiStatusCode.Error, null, 400, "Không có file nào được tải lên");
                }

                var (_, fileUrl) = await _fileUploadService.UploadFileAsync(file, "documents", entityType, entityId);
                return new ApiResponse(ApiStatusCode.Success, new { location = fileUrl }, 200, "Ok");
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Get file by path (with date folder structure)
        /// </summary>
        [HttpGet("file/{subFolder}/{year}/{month}/{day}/{fileName}")]
        [AllowAnonymous] // Public file access
        public IActionResult GetFile(string subFolder, string year, string month, string day, string fileName)
        {
            try
            {
                // Validate path components to prevent directory traversal
                if (!IsValidPathComponent(subFolder) || !IsValidPathComponent(year) || 
                    !IsValidPathComponent(month) || !IsValidPathComponent(day) || 
                    !IsValidPathComponent(fileName))
                {
                    return BadRequest("Invalid path components");
                }

                var relativePath = Path.Combine(subFolder, year, month, day, fileName);
                var filePath = Path.Combine(_env.ContentRootPath, "Uploads", relativePath);
                
                // Security check: ensure file is within uploads directory
                var fullUploadPath = Path.GetFullPath(Path.Combine(_env.ContentRootPath, "Uploads"));
                var fullFilePath = Path.GetFullPath(filePath);
                
                if (!fullFilePath.StartsWith(fullUploadPath))
                {
                    return BadRequest("Invalid file path");
                }

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("File không tồn tại");
                }

                var mimeType = GetMimeType(fileName);
                var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                
                return File(fileStream, mimeType, enableRangeProcessing: true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error serving file: {SubFolder}/{Year}/{Month}/{Day}/{FileName}", 
                               subFolder, year, month, day, fileName);
                return StatusCode(500, "Error serving file");
            }
        }

        /// <summary>
        /// Legacy endpoint for backward compatibility
        /// </summary>
        [HttpGet("file/{subFolder}/{fileName}")]
        [AllowAnonymous]
        public IActionResult GetFile(string subFolder, string fileName)
        {
            try
            {
                if (!IsValidPathComponent(subFolder) || !IsValidPathComponent(fileName))
                {
                    return BadRequest("Invalid path components");
                }

                var filePath = Path.Combine(_env.ContentRootPath, "Uploads", subFolder, fileName);
                
                // Security check
                var fullUploadPath = Path.GetFullPath(Path.Combine(_env.ContentRootPath, "Uploads"));
                var fullFilePath = Path.GetFullPath(filePath);
                
                if (!fullFilePath.StartsWith(fullUploadPath))
                {
                    return BadRequest("Invalid file path");
                }

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("File không tồn tại");
                }

                var mimeType = GetMimeType(fileName);
                var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                
                return File(fileStream, mimeType, enableRangeProcessing: true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error serving file: {SubFolder}/{FileName}", subFolder, fileName);
                return StatusCode(500, "Error serving file");
            }
        }

        private bool IsValidPathComponent(string component)
        {
            // Prevent directory traversal and other malicious patterns
            if (string.IsNullOrWhiteSpace(component))
                return false;
                
            var invalidChars = new[] { "..", "/", "\\", ":", "*", "?", "\"", "<", ">", "|" };
            return !invalidChars.Any(component.Contains);
        }

        private string GetMimeType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                ".mp4" => "video/mp4",
                ".webm" => "video/webm",
                ".avi" => "video/x-msvideo",
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".mp3" => "audio/mpeg",
                ".wav" => "audio/wav",
                ".ogg" => "audio/ogg",
                _ => "application/octet-stream"
            };
        }
    }
}