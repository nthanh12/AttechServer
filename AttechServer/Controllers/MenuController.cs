using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Applications.UserModules.Dtos.Menu;
using AttechServer.Shared.WebAPIBase;
using Microsoft.AspNetCore.Mvc;
using AttechServer.Shared.Consts.Permissions;
using AttechServer.Shared.Filters;

namespace AttechServer.Controllers
{
    [Route("api/menu")]
    [ApiController]
    public class MenuController : ApiControllerBase
    {
        private readonly IMenuService _menuService;
        public MenuController(IMenuService menuService, ILogger<MenuController> logger) : base(logger)
        {
            _menuService = menuService;
        }

        [HttpGet("find-all")]
        public async Task<ApiResponse> FindAll()
        {
            try { return new(await _menuService.GetAllFlat()); }
            catch (Exception ex) { return OkException(ex); }
        }

        [HttpGet("tree")]
        public async Task<ApiResponse> GetTree()
        {
            try { return new(await _menuService.GetTree()); }
            catch (Exception ex) { return OkException(ex); }
        }

        [HttpGet("find-by-id/{id}")]
        public async Task<ApiResponse> FindById(int id)
        {
            try { return new(await _menuService.FindById(id)); }
            catch (Exception ex) { return OkException(ex); }
        }

        [PermissionFilter(PermissionKeys.Menu_Create)]
        [HttpPost("create")]
        public async Task<ApiResponse> Create([FromBody] CreateMenuDto input)
        {
            try { await _menuService.Create(input); return new(); }
            catch (Exception ex) { return OkException(ex); }
        }

        [PermissionFilter(PermissionKeys.Menu_Update)]
        [HttpPut("update")]
        public async Task<ApiResponse> Update([FromBody] UpdateMenuDto input)
        {
            try { await _menuService.Update(input); return new(); }
            catch (Exception ex) { return OkException(ex); }
        }

        [PermissionFilter(PermissionKeys.Menu_Delete)]
        [HttpDelete("delete/{id}")]
        public async Task<ApiResponse> Delete(int id)
        {
            try { await _menuService.Delete(id); return new(); }
            catch (Exception ex) { return OkException(ex); }
        }
    }
} 