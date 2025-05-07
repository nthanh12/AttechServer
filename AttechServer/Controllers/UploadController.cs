using AttechServer.Domains.Entities.Main;
using AttechServer.Infrastructures.Persistances;
using AttechServer.Shared.WebAPIBase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AttechServer.Shared.ApplicationBase.Common;

namespace AttechServer.Controllers
{
    [Route("api/upload")]
    [ApiController]
    public class UploadController : ApiControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly ApplicationDbContext _dbContext;
        private readonly long _maxFileSize = 10 * 1024 * 1024; // Giới hạn 10MB

        public UploadController(ILogger<UploadController> logger, IWebHostEnvironment env, ApplicationDbContext dbContext) : base(logger)
        {
            _env = env;
            _dbContext = dbContext;
        }

        private string GetUploadPath(string subFolder)
        {
            // Chia thư mục theo ngày: Uploads/images/2025/04/24
            var datePath = Path.Combine(DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString("D2"), DateTime.Now.Day.ToString("D2"));
            var uploadPath = Path.Combine(_env.ContentRootPath, "Uploads", subFolder, datePath);
            Directory.CreateDirectory(uploadPath);
            return uploadPath;
        }

        [HttpPost("multi-upload")]
        //[Authorize]
        public async Task<IActionResult> MultiUpload(IFormFile[] files)
        {
            if (files == null || !files.Any())
                return BadRequest("Không có file nào được tải lên.");

            var fileUrls = new List<string>();
            foreach (var file in files)
            {
                if (file.Length > _maxFileSize)
                    return BadRequest($"File {file.FileName} vượt quá kích thước cho phép (10MB).");

                // Xác định loại file và thư mục lưu trữ
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                string subFolder;
                if (new[] { ".jpg", ".jpeg", ".png", ".gif" }.Contains(extension))
                    subFolder = "images";
                else if (new[] { ".mp4", ".webm", ".ogg" }.Contains(extension))
                    subFolder = "videos";
                else if (new[] { ".pdf", ".doc", ".docx", ".xls", ".xlsx" }.Contains(extension))
                    subFolder = "documents";
                else if (new[] { ".mp3", ".wav", ".ogg" }.Contains(extension))
                    subFolder = "audio";
                else if (new[] { ".zip", ".rar" }.Contains(extension))
                    subFolder = "archives";
                else
                    return BadRequest($"Định dạng file {file.FileName} không được hỗ trợ.");

                // Kiểm tra MIME type
                if (!IsValidMimeType(file.ContentType, extension))
                    return BadRequest($"MIME type của file {file.FileName} không hợp lệ.");

                var datePath = Path.Combine(DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString("D2"), DateTime.Now.Day.ToString("D2"));
                var uploads = GetUploadPath(subFolder);
                var fileName = Guid.NewGuid() + extension;
                var filePath = Path.Combine(uploads, fileName);
                var relativePath = Path.Combine(subFolder, datePath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var fileEntry = new FileUpload
                {
                    EntityType = EntityType.Temp,
                    EntityId = 0,
                    FilePath = relativePath,
                    FileType = subFolder,
                    CreatedDate = DateTime.Now
                };
                _dbContext.FileUploads.Add(fileEntry);
                await _dbContext.SaveChangesAsync();

                var fileUrl = $"{Request.Scheme}://{Request.Host}/api/upload/file/{subFolder}/{datePath}/{fileName}";
                fileUrls.Add(fileUrl);

                _logger.LogInformation($"Uploaded file: {relativePath}");
            }

            return Ok(new { locations = fileUrls });
        }

        [HttpPost("image")]
        [Authorize]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            var result = await MultiUpload(new[] { file });
            if (result is OkObjectResult okResult && okResult.Value is { } value)
            {
                var locations = (value as dynamic)?.locations as List<string>;
                return Ok(new { location = locations?.FirstOrDefault() });
            }
            return result;
        }

        [HttpPost("video")]
        [Authorize]
        public async Task<IActionResult> UploadVideo(IFormFile file)
        {
            var result = await MultiUpload(new[] { file });
            if (result is OkObjectResult okResult && okResult.Value is { } value)
            {
                var locations = (value as dynamic)?.locations as List<string>;
                return Ok(new { location = locations?.FirstOrDefault() });
            }
            return result;
        }

        [HttpPost("document")]
        [Authorize]
        public async Task<IActionResult> UploadDocument(IFormFile file)
        {
            var result = await MultiUpload(new[] { file });
            if (result is OkObjectResult okResult && okResult.Value is { } value)
            {
                var locations = (value as dynamic)?.locations as List<string>;
                return Ok(new { location = locations?.FirstOrDefault() });
            }
            return result;
        }

        [HttpGet("file/{subFolder}/{year}/{month}/{day}/{fileName}")]
        public IActionResult GetFile(string subFolder, string year, string month, string day, string fileName)
        {
            var relativePath = Path.Combine(subFolder, year, month, day, fileName);
            var filePath = Path.Combine(_env.ContentRootPath, "Uploads", relativePath);
            if (!System.IO.File.Exists(filePath))
                return NotFound("File không tồn tại.");

            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var mimeType = GetMimeType(fileName);
            return File(fileStream, mimeType);
        }

        // Giữ lại endpoint cũ để tương thích
        [HttpGet("file/{subFolder}/{fileName}")]
        public IActionResult GetFile(string subFolder, string fileName)
        {
            var filePath = Path.Combine(_env.ContentRootPath, "Uploads", subFolder, fileName);
            if (!System.IO.File.Exists(filePath))
                return NotFound("File không tồn tại.");

            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var mimeType = GetMimeType(fileName);
            return File(fileStream, mimeType);
        }

        private bool IsValidMimeType(string mimeType, string extension)
        {
            var expectedMimeType = GetMimeType(extension);
            return mimeType == expectedMimeType || mimeType == "application/octet-stream";
        }

        private string GetMimeType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".mp4" => "video/mp4",
                ".webm" => "video/webm",
                ".ogg" => "video/ogg",
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".mp3" => "audio/mpeg",
                ".wav" => "audio/wav",
                ".zip" => "application/zip",
                ".rar" => "application/x-rar-compressed",
                _ => "application/octet-stream"
            };
        }
    }
}