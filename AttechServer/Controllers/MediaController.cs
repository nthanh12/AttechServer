using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Applications.UserModules.Dtos.FileUpload;
using AttechServer.Shared.ApplicationBase.Common;
using AttechServer.Shared.Consts.Permissions;
using AttechServer.Shared.Filters;
using AttechServer.Shared.WebAPIBase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AttechServer.Controllers
{
    [Route("api/media")]
    [ApiController]
    [Authorize]
    public class MediaController : ApiControllerBase
    {
        private readonly IMediaService _mediaService;

        public MediaController(ILogger<MediaController> logger, IMediaService mediaService) : base(logger)
        {
            _mediaService = mediaService;
        }

        /// <summary>
        /// Danh sách file
        /// </summary>
        [HttpGet("find-all")]
        [PermissionFilter(PermissionKeys.FileUpload)]
        public async Task<ApiResponse> FindAll([FromQuery] PagingRequestBaseDto input)
        {
            try
            {
                return new(await _mediaService.FindAll(input));
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Danh sách file theo entity
        /// </summary>
        [HttpGet("find-by-entity/{entityType}/{entityId}")]
        [Authorize]
        public async Task<ApiResponse> FindByEntity(EntityType entityType, int entityId, [FromQuery] PagingRequestBaseDto input)
        {
            try
            {
                return new(await _mediaService.FindByEntity(entityType, entityId, input));
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Xóa file
        /// </summary>
        [HttpDelete("delete/{id}")]
        [Authorize]
        public async Task<ApiResponse> Delete(int id)
        {
            try
            {
                await _mediaService.Delete(id);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Xóa file theo entity
        /// </summary>
        [HttpDelete("delete-by-entity/{entityType}/{entityId}")]
        [Authorize]
        public async Task<ApiResponse> DeleteByEntity(EntityType entityType, int entityId)
        {
            try
            {
                await _mediaService.DeleteByEntity(entityType, entityId);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
    }
} 