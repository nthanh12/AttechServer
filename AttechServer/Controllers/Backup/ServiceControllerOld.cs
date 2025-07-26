using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Applications.UserModules.Dtos.Service;
using AttechServer.Shared.ApplicationBase.Common;
using AttechServer.Shared.WebAPIBase;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AttechServer.Shared.Filters;
using AttechServer.Shared.Consts.Permissions;
using Microsoft.AspNetCore.Authorization;

namespace AttechServer.Controllers
{
    [Route("api/service")]
    [ApiController]
    public class ServiceController : ApiControllerBase
    {
        private readonly IServiceService _serviceService;
        public ServiceController(IServiceService serviceService, ILogger<ServiceController> logger) : base(logger)
        {
            _serviceService = serviceService;
        }

        /// <summary>
        /// Danh s 1ch d 0ch v e5
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("find-all")]
        [AllowAnonymous]
        public async Task<ApiResponse> FindAll([FromQuery] PagingRequestBaseDto input)
        {
            try
            {
                return new(await _serviceService.FindAll(input));
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Th ng tin chi ti bt d 0ch v e5
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("find-by-id/{id}")]
        [AllowAnonymous]
        public async Task<ApiResponse> FindById(int id)
        {
            try
            {
                return new(await _serviceService.FindById(id));
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// L a5y chi ti bt d 0ch v e5 theo slug
        /// </summary>
        [HttpGet("detail/{slug}")]
        [AllowAnonymous]
        public async Task<ApiResponse> FindBySlug(string slug)
        {
            try
            {
                return new(await _serviceService.FindBySlug(slug));
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Th em m dji d 0ch v e5
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("create")]
        [Authorize]
        [PermissionFilter(PermissionKeys.CreateService)]
        public async Task<ApiResponse> Create([FromBody] CreateServiceDto input)
        {
            try
            {
                var result = await _serviceService.Create(input);
                return new ApiResponse(ApiStatusCode.Success, result, 200, "Ok");
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Cadp nh adt d 0ch v e5
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut("update")]
        [Authorize]
        [PermissionFilter(PermissionKeys.EditService)]
        public async Task<ApiResponse> Update([FromBody] UpdateServiceDto input)
        {
            try
            {
                await _serviceService.Update(input);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// X 3a d 0ch v e5
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("delete/{id}")]
        [Authorize]
        [PermissionFilter(PermissionKeys.DeleteService)]
        public async Task<ApiResponse> Delete(int id)
        {
            try
            {
                await _serviceService.Delete(id);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Kh 3a/M df kh 3a d 0ch v e5
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpPut("update-status")]
        [Authorize]
        [PermissionFilter(PermissionKeys.EditService)]
        public async Task<ApiResponse> UpdateStatus(int id, int status)
        {
            try
            {
                await _serviceService.UpdateStatusService(id, status);
                return new();
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }
    }
} 