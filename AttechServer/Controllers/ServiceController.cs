using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Applications.UserModules.Dtos.Service;
using AttechServer.Shared.ApplicationBase.Common;
using AttechServer.Shared.Attributes;
using AttechServer.Shared.WebAPIBase;
using Microsoft.AspNetCore.Mvc;
using AttechServer.Shared.Filters;
using AttechServer.Shared.Consts.Permissions;
using Microsoft.AspNetCore.Authorization;

namespace AttechServer.Controllers
{
    [Route("api/service")]
    public class ServiceController : BaseCrudController<IServiceService, ServiceDto, DetailServiceDto, CreateServiceDto, UpdateServiceDto>
    {
        public ServiceController(IServiceService serviceService, ILogger<ServiceController> logger)
            : base(serviceService, logger)
        {
        }

        [HttpGet("find-all")]
        [AllowAnonymous]
        public override async Task<ApiResponse> FindAll([FromQuery] PagingRequestBaseDto input)
        {
            return await base.FindAll(input);
        }

        [HttpGet("find-by-id/{id}")]
        [AllowAnonymous]
        public override async Task<ApiResponse> FindById(int id)
        {
            return await base.FindById(id);
        }

        [HttpGet("detail/{slug}")]
        [AllowAnonymous]
        public override async Task<ApiResponse> FindBySlug(string slug)
        {
            return await base.FindBySlug(slug);
        }

        [HttpPost("create")]
        [Authorize]
        [PermissionFilter(PermissionKeys.CreateService)]
        public override async Task<ApiResponse> Create([FromBody] CreateServiceDto input)
        {
            return await base.Create(input);
        }

        [HttpPut("update")]
        [Authorize]
        [PermissionFilter(PermissionKeys.EditService)]
        public override async Task<ApiResponse> Update([FromBody] UpdateServiceDto input)
        {
            return await base.Update(input);
        }

        [HttpDelete("delete/{id}")]
        [Authorize]
        [PermissionFilter(PermissionKeys.DeleteService)]
        public override async Task<ApiResponse> Delete(int id)
        {
            return await base.Delete(id);
        }

        protected override async Task GetUpdateStatusAsync(AttechServer.Applications.UserModules.Dtos.UpdateStatusDto input)
        {
            await _service.UpdateStatusService(input.Id, input.Status);
        }

        #region Protected Implementation Methods

        protected override async Task<object> GetFindAllAsync(PagingRequestBaseDto input)
        {
            return await _service.FindAll(input);
        }

        protected override async Task<DetailServiceDto> GetFindByIdAsync(int id)
        {
            return await _service.FindById(id);
        }

        protected override async Task<DetailServiceDto> GetFindBySlugAsync(string slug)
        {
            return await _service.FindBySlug(slug);
        }

        protected override async Task<object> GetCreateAsync(CreateServiceDto input)
        {
            return await _service.Create(input);
        }

        protected override async Task<object?> GetUpdateAsync(UpdateServiceDto input)
        {
            await _service.Update(input);
            return null;
        }

        protected override async Task GetDeleteAsync(int id)
        {
            await _service.Delete(id);
        }

        #endregion
    }
}
