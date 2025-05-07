using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Applications.UserModules.Dtos.Service;
using AttechServer.Shared.ApplicationBase.Common;
using AttechServer.Shared.WebAPIBase;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AttechServer.Controllers
{
    [Route("api/service")]
    [ApiController]
    public class ServiceController : ApiControllerBase
    {
        private readonly IServiceService _serviceService;
        public ServiceController(IServiceService ServiceService, ILogger<ServiceController> logger) : base (logger)
        {
            _serviceService = ServiceService;
        }

        /// <summary>
        /// Danh sách dịch vụ
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("find-all")]
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
        /// Thông tin chi tiết dịch vụ
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("find-by-id/{id}")]
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
        /// Thêm mới dịch vụ
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("create")]
        public async Task<ApiResponse> Create([FromBody] CreateServiceDto input)
        {
            try
            {
                var result = await _serviceService.Create(input);
                return new ApiResponse(result);
            }
            catch (Exception ex)
            {
                return OkException(ex);
            }
        }

        /// <summary>
        /// Cập nhật dịch vụ
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut("update")]
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
        /// Xóa dịch vụ
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("delete/{id}")]
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
        /// Khóa/Mở khóa dịch vụ
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpPut("update-status")]
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
