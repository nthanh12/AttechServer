using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Applications.UserModules.Dtos.Setting;
using AttechServer.Shared.ApplicationBase.Common;
using AttechServer.Shared.WebAPIBase;
using AttechServer.Shared.Filters;
using AttechServer.Shared.Consts.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AttechServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SettingController : ApiControllerBase
    {
        private readonly IAttachmentService _attachmentService;

        public SettingController(IAttachmentService attachmentService, ILogger<SettingController> logger)
            : base(logger)
        {
            _attachmentService = attachmentService;
        }

        /// <summary>
        /// Upload file cho bất kỳ setting nào (banner1, banner2, logo, etc.)
        /// </summary>
        [HttpPost("{settingKey}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadSetting(
            [FromRoute] string settingKey, 
            [FromForm] UploadSettingDto dto)
        {
            try
            {
                if (string.IsNullOrEmpty(settingKey))
                {
                    return BadRequest("Setting key không hợp lệ");
                }

                if (dto?.File == null || dto.File.Length == 0)
                {
                    return BadRequest("File không hợp lệ");
                }

                if (!dto.File.ContentType.StartsWith("image/"))
                {
                    return BadRequest("Chỉ chấp nhận file ảnh");
                }

                // Chuyển setting key thành objectId
                var objectId = settingKey.ToObjectId();
                
                _logger.LogInformation($"Uploading {settingKey} file: {dto.File.FileName}, objectId: {objectId}");
                
                // Upload temp file
                var tempAttachment = await _attachmentService.UploadTempAsync(dto.File, "image");
                
                // Associate với setting
                await _attachmentService.AssociateAttachmentsAsync(
                    new List<int> { tempAttachment.Id }, 
                    ObjectType.Setting, 
                    objectId: objectId,
                    isFeaturedImage: true,
                    isContentImage: false
                );
                
                _logger.LogInformation($"{settingKey} uploaded successfully: {tempAttachment.Url}");
                
                return Ok(new { 
                    success = true, 
                    message = $"{settingKey} đã được cập nhật thành công",
                    settingKey = settingKey,
                    url = tempAttachment.Url,
                    id = tempAttachment.Id,
                    fileName = tempAttachment.FileName
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error uploading {settingKey}");
                return StatusCode(500, $"Có lỗi xảy ra khi upload {settingKey}");
            }
        }

        /// <summary>
        /// Lấy file của một setting cụ thể
        /// </summary>
        [HttpGet("{settingKey}")]
        public async Task<IActionResult> GetSetting(string settingKey)
        {
            try
            {
                if (string.IsNullOrEmpty(settingKey))
                {
                    return BadRequest("Setting key không hợp lệ");
                }

                var objectId = settingKey.ToObjectId();
                var attachments = await _attachmentService.GetByEntityAsync(ObjectType.Setting, objectId);
                var attachment = attachments.FirstOrDefault(a => a.IsPrimary);
                
                if (attachment == null)
                {
                    return Ok(new { 
                        settingKey = settingKey,
                        url = (string?)null, 
                        message = $"Chưa có {settingKey}" 
                    });
                }
                
                return Ok(new { 
                    settingKey = settingKey,
                    url = attachment.Url,
                    id = attachment.Id,
                    fileName = attachment.OriginalFileName,
                    fileSize = attachment.FileSize,
                    uploadDate = attachment.CreatedDate
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting {settingKey}");
                return StatusCode(500, $"Có lỗi xảy ra khi lấy {settingKey}");
            }
        }

        /// <summary>
        /// Lấy tất cả settings đã được định nghĩa
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllSettings()
        {
            try
            {
                var settings = new Dictionary<string, object?>();

                // Lấy tất cả enum values
                var settingTypes = Enum.GetValues<SettingType>().Where(s => s != SettingType.Custom);
                
                foreach (var settingType in settingTypes)
                {
                    var settingKey = settingType.ToString();
                    var objectId = (int)settingType;
                    
                    var attachments = await _attachmentService.GetByEntityAsync(ObjectType.Setting, objectId);
                    var attachment = attachments.FirstOrDefault(a => a.IsPrimary);
                    
                    if (attachment != null)
                    {
                        settings[settingKey] = new
                        {
                            url = attachment.Url,
                            id = attachment.Id,
                            fileName = attachment.OriginalFileName,
                            uploadDate = attachment.CreatedDate,
                            description = settingType.GetDescription()
                        };
                    }
                    else
                    {
                        settings[settingKey] = null;
                    }
                }
                
                return Ok(settings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all settings");
                return StatusCode(500, "Có lỗi xảy ra khi lấy settings");
            }
        }

        /// <summary>
        /// Lấy nhiều settings cùng lúc
        /// </summary>
        [HttpPost("batch")]
        public async Task<IActionResult> GetMultipleSettings([FromBody] List<string> settingKeys)
        {
            try
            {
                var result = new Dictionary<string, object?>();

                foreach (var settingKey in settingKeys)
                {
                    var objectId = settingKey.ToObjectId();
                    var attachments = await _attachmentService.GetByEntityAsync(ObjectType.Setting, objectId);
                    var attachment = attachments.FirstOrDefault(a => a.IsPrimary);
                    
                    if (attachment != null)
                    {
                        result[settingKey] = new
                        {
                            url = attachment.Url,
                            id = attachment.Id,
                            fileName = attachment.OriginalFileName,
                            uploadDate = attachment.CreatedDate
                        };
                    }
                    else
                    {
                        result[settingKey] = null;
                    }
                }
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting multiple settings");
                return StatusCode(500, "Có lỗi xảy ra khi lấy settings");
            }
        }

        /// <summary>
        /// Xóa một setting
        /// </summary>
        [HttpDelete("{settingKey}")]
        public async Task<IActionResult> DeleteSetting(string settingKey)
        {
            try
            {
                if (string.IsNullOrEmpty(settingKey))
                {
                    return BadRequest("Setting key không hợp lệ");
                }

                var objectId = settingKey.ToObjectId();
                await _attachmentService.SoftDeleteEntityAttachmentsAsync(ObjectType.Setting, objectId);
                
                return Ok(new { 
                    success = true, 
                    message = $"{settingKey} đã được xóa",
                    settingKey = settingKey
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting {settingKey}");
                return StatusCode(500, $"Có lỗi xảy ra khi xóa {settingKey}");
            }
        }

        /// <summary>
        /// Lấy danh sách tất cả setting types có thể dùng
        /// </summary>
        [HttpGet("types")]
        public IActionResult GetSettingTypes()
        {
            var settingTypes = Enum.GetValues<SettingType>()
                .Where(s => s != SettingType.Custom)
                .Select(s => new
                {
                    key = s.ToString(),
                    value = (int)s,
                    description = s.GetDescription()
                })
                .ToList();

            return Ok(settingTypes);
        }
    }
}