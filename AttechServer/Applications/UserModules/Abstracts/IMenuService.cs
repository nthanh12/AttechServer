using AttechServer.Applications.UserModules.Dtos.Menu;
using AttechServer.Shared.ApplicationBase.Common;

namespace AttechServer.Applications.UserModules.Abstracts
{
    public interface IMenuService
    {
        Task<List<MenuDto>> GetAllFlat();
        Task<List<MenuDto>> GetTree();
        Task<MenuDto> FindById(int id);
        Task Create(CreateMenuDto input);
        Task Update(UpdateMenuDto input);
        Task Delete(int id);
    }
} 