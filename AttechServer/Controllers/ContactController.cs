using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Applications.UserModules.Dtos.Contact;
using AttechServer.Shared.ApplicationBase.Common;
using AttechServer.Shared.Attributes;
using AttechServer.Shared.WebAPIBase;
using Microsoft.AspNetCore.Mvc;
using AttechServer.Shared.Filters;
using AttechServer.Shared.Consts;
using Microsoft.AspNetCore.Authorization;
using System.Net;

namespace AttechServer.Controllers
{
    [Route("api/contact")]
    [ApiController]
    public class ContactController : ApiControllerBase
    {
        private readonly IContactService _contactService;

        public ContactController(IContactService contactService, ILogger<ContactController> logger)
            : base(logger)
        {
            _contactService = contactService;
        }

        /// <summary>
        /// Submit contact form (Public API)
        /// </summary>
        [HttpPost("submit")]
        [AllowAnonymous]
        [ServiceFilter(typeof(AntiSpamFilter))]
        public async Task<ApiResponse> Submit([FromBody] CreateContactDto input)
        {
            try
            {
                // Get client IP and User Agent for security tracking
                var ipAddress = GetClientIpAddress();
                var userAgent = Request.Headers["User-Agent"].ToString();

                var result = await _contactService.Submit(input, ipAddress, userAgent);
                
                return new ApiResponse(ApiStatusCode.Success, result, 200, "Liên hệ của bạn đã được gửi thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting contact form");
                return OkException(ex);
            }
        }

        /// <summary>
        /// Get all contact messages with filtering and sorting (Admin only)
        /// </summary>
        [HttpGet("find-all")]
        [Authorize]
        [RoleFilter(2)]
        [CacheResponse(CacheProfiles.ShortCache, "admin-contacts", varyByQueryString: true)]
        public async Task<ApiResponse> FindAll([FromQuery] PagingRequestBaseDto input)
        {
            try
            {
                var result = await _contactService.FindAll(input);
                return new ApiResponse(ApiStatusCode.Success, result, 200, "Ok");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all contacts");
                return OkException(ex);
            }
        }

        /// <summary>
        /// Get contact message detail by ID (Admin only)
        /// </summary>
        [HttpGet("find-by-id/{id}")]
        [Authorize]
        [RoleFilter(2)]
        [CacheResponse(CacheProfiles.MediumCache, "admin-contact-detail")]
        public async Task<ApiResponse> FindById(int id)
        {
            try
            {
                var result = await _contactService.FindById(id);
                return new ApiResponse(ApiStatusCode.Success, result, 200, "Ok");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting contact by id");
                return OkException(ex);
            }
        }

        /// <summary>
        /// Update contact message status (Admin only)
        /// </summary>
        [HttpPut("update-status/{id}")]
        [Authorize]
        [RoleFilter(2)]
        public async Task<ApiResponse> UpdateStatus(int id, [FromBody] UpdateContactStatusDto input)
        {
            try
            {
                var result = await _contactService.UpdateStatus(id, input);
                return new ApiResponse(ApiStatusCode.Success, result, 200, "Cập nhật trạng thái thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating contact status");
                return OkException(ex);
            }
        }

        /// <summary>
        /// Mark contact message as read (Admin only)
        /// </summary>
        [HttpPut("mark-as-read/{id}")]
        [Authorize]
        [RoleFilter(2)]
        public async Task<ApiResponse> MarkAsRead(int id)
        {
            try
            {
                await _contactService.MarkAsRead(id);
                return new ApiResponse(ApiStatusCode.Success, null, 200, "Đã đánh dấu là đã đọc");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking contact as read");
                return OkException(ex);
            }
        }

        /// <summary>
        /// Mark contact message as unread (Admin only)
        /// </summary>
        [HttpPut("mark-as-unread/{id}")]
        [Authorize]
        [RoleFilter(2)]
        public async Task<ApiResponse> MarkAsUnread(int id)
        {
            try
            {
                await _contactService.MarkAsUnread(id);
                return new ApiResponse(ApiStatusCode.Success, null, 200, "Đã đánh dấu là chưa đọc");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking contact as unread");
                return OkException(ex);
            }
        }

        /// <summary>
        /// Delete contact message (Admin only)
        /// </summary>
        [HttpDelete("delete/{id}")]
        [Authorize]
        [RoleFilter(2)]
        public async Task<ApiResponse> Delete(int id)
        {
            try
            {
                await _contactService.Delete(id);
                return new ApiResponse(ApiStatusCode.Success, null, 200, "Xóa thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting contact");
                return OkException(ex);
            }
        }

        /// <summary>
        /// Get unread contact count (Admin only)
        /// </summary>
        [HttpGet("unread-count")]
        [Authorize]
        [RoleFilter(2)]
        [CacheResponse(CacheProfiles.VeryShortCache, "admin-contacts-unread-count")]
        public async Task<ApiResponse> GetUnreadCount()
        {
            try
            {
                var count = await _contactService.GetUnreadCount();
                return new ApiResponse(ApiStatusCode.Success, new { count }, 200, "Ok");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting unread contact count");
                return OkException(ex);
            }
        }

        /// <summary>
        /// Get client IP address from request
        /// </summary>
        private string? GetClientIpAddress()
        {
            // Check for forwarded IP first (if behind proxy/load balancer)
            var forwardedFor = Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                // X-Forwarded-For can contain multiple IPs, take the first one
                return forwardedFor.Split(',')[0].Trim();
            }

            // Check for real IP (some proxies use this)
            var realIp = Request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrEmpty(realIp))
            {
                return realIp;
            }

            // Fall back to connection remote IP
            return Request.HttpContext.Connection.RemoteIpAddress?.ToString();
        }
    }
}