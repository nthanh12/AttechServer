using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Applications.UserModules.Dtos.Attachment;
using AttechServer.Shared.WebAPIBase;
using AttechServer.Shared.ApplicationBase.Common;
using AttechServer.Shared.Filters;
using AttechServer.Shared.Consts.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AttechServer.Controllers
{
    [Route("api/attachments")]
    [ApiController]
    [Authorize]
    public class AttachmentController : ApiControllerBase
    {
        private readonly IAttachmentService _attachmentService;
        private readonly IWebHostEnvironment _environment;

        public AttachmentController(
            IAttachmentService attachmentService,
            IWebHostEnvironment environment,
            ILogger<AttachmentController> logger) : base(logger)
        {
            _attachmentService = attachmentService;
            _environment = environment;
        }

        [HttpPost("")]
        [PermissionFilter(PermissionKeys.FileUpload)]
        public async Task<ApiResponse> UploadTemp([FromForm] UploadDto input)
        {
            try
            {
                var response = await _attachmentService.UploadTempAsync(input.File, input.RelationType);
                return new ApiResponse(ApiStatusCode.Success, response, 200, "Upload tạm thành công");
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [HttpPost("associate")]
        [PermissionFilter(PermissionKeys.FileUpload)]
        public async Task<ApiResponse> AssociateAttachments([FromBody] AttachAssociationDto input)
        {
            try
            {
                var result = await _attachmentService.AssociateAttachmentsAsync(
                    input.AttachmentIds, 
                    input.ObjectType, 
                    input.ObjectId,
                    isFeaturedImage: false // Default to gallery attachments
                );
                
                return new ApiResponse(ApiStatusCode.Success, result, 200, 
                    result ? "Liên kết attachment thành công" : "Không tìm thấy attachment");
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [HttpGet("{id}")]
        [Authorize] // Remove AllowAnonymous - require authentication
        public async Task<IActionResult> GetFile(int id)
        {
            try
            {
                var attachment = await _attachmentService.GetByIdAsync(id);
                if (attachment == null)
                    return NotFound();

                // SECURITY: Check if user has permission to access this attachment
                var userIdClaim = User.FindFirst("user_id")?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized("Invalid user");
                }

                // For now, allow access if user is authenticated
                // TODO: Add more granular permission checks based on ObjectType and ObjectId
                // Example: Check if user has permission to view News with ObjectId = attachment.ObjectId
                
                var filePath = Path.Combine(_environment.ContentRootPath, "Uploads", attachment.FilePath);
                if (!System.IO.File.Exists(filePath))
                    return NotFound();

                // For images, display inline; for other files, force download
                if (attachment.ContentType.StartsWith("image/"))
                {
                    return PhysicalFile(filePath, attachment.ContentType);
                }
                else
                {
                    return PhysicalFile(filePath, attachment.ContentType, attachment.OriginalFileName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting file {Id}", id);
                return StatusCode(500);
            }
        }


        [HttpDelete("{id}")]
        [PermissionFilter(PermissionKeys.FileUpload)]
        public async Task<ApiResponse> Delete(int id)
        {
            try
            {
                var result = await _attachmentService.DeleteAsync(id);
                return new ApiResponse(ApiStatusCode.Success, result, 200, result ? "Xóa thành công" : "Không tìm thấy file");
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        [HttpGet("entity/{objectType}/{objectId}")]
        [AllowAnonymous]
        public async Task<ApiResponse> GetByEntity(string objectType, int objectId)
        {
            try
            {
                if (!Enum.TryParse<Shared.ApplicationBase.Common.ObjectType>(objectType, true, out var type))
                    return new ApiResponse(ApiStatusCode.Success, null, 400, "Invalid object type");

                var attachments = await _attachmentService.GetByEntityAsync(type, objectId);
                var dtos = attachments.Select(a => new AttachmentDto
                {
                    Id = a.Id,
                    FilePath = a.Url,
                    OriginalFileName = a.OriginalFileName,
                    FileSize = a.FileSize,
                    ContentType = a.ContentType,
                    ObjectType = a.ObjectType,
                    ObjectId = a.ObjectId,
                    RelationType = a.RelationType,
                    IsPrimary = a.IsPrimary,
                    IsTemporary = a.IsTemporary,
                    CreatedDate = a.CreatedDate
                }).ToList();

                return new ApiResponse(ApiStatusCode.Success, dtos, 200, "Ok");
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Cleanup temp files older than specified hours (background task endpoint)
        /// </summary>
        [HttpPost("cleanup")]
        [PermissionFilter(PermissionKeys.FileUpload)]
        public async Task<ApiResponse> CleanupTempFiles()
        {
            try
            {
                var result = await _attachmentService.CleanupTempFilesAsync();
                return new ApiResponse(ApiStatusCode.Success, result, 200, "Cleanup temp files thành công");
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
    }
}