using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Applications.UserModules.Dtos.Menu;
using AttechServer.Domains.Entities.Main;
using AttechServer.Infrastructures.Persistances;
using Microsoft.EntityFrameworkCore;

namespace AttechServer.Applications.UserModules.Implements
{
    public class MenuService : IMenuService
    {
        private readonly ApplicationDbContext _dbContext;
        public MenuService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<MenuDto>> GetAllFlat()
        {
            var menus = await _dbContext.Menus.AsNoTracking().ToListAsync();
            return menus.Select(MapToDto).ToList();
        }

        public async Task<List<MenuDto>> GetTree()
        {
            var menus = await _dbContext.Menus.AsNoTracking().ToListAsync();
            var lookup = menus.ToDictionary(m => m.Id, m => MapToDto(m));
            List<MenuDto> roots = new();
            foreach (var menu in lookup.Values)
            {
                if (menu.ParentId == null)
                    roots.Add(menu);
                else if (lookup.TryGetValue(menu.ParentId.Value, out var parent))
                    parent.Children.Add(menu);
            }
            return roots;
        }

        public async Task<MenuDto> FindById(int id)
        {
            var menu = await _dbContext.Menus.FindAsync(id);
            if (menu == null) throw new Exception("Menu not found");
            return MapToDto(menu);
        }

        public async Task Create(CreateMenuDto input)
        {
            var menu = new Menu
            {
                Key = input.Key,
                LabelVi = input.LabelVi,
                LabelEn = input.LabelEn,
                PathVi = input.PathVi,
                PathEn = input.PathEn,
                ParentId = input.ParentId
            };
            _dbContext.Menus.Add(menu);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Update(UpdateMenuDto input)
        {
            var menu = await _dbContext.Menus.FindAsync(input.Id);
            if (menu == null) throw new Exception("Menu not found");
            menu.Key = input.Key;
            menu.LabelVi = input.LabelVi;
            menu.LabelEn = input.LabelEn;
            menu.PathVi = input.PathVi;
            menu.PathEn = input.PathEn;
            menu.ParentId = input.ParentId;
            await _dbContext.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var menu = await _dbContext.Menus.FindAsync(id);
            if (menu == null) throw new Exception("Menu not found");
            _dbContext.Menus.Remove(menu);
            await _dbContext.SaveChangesAsync();
        }

        private static MenuDto MapToDto(Menu m) => new MenuDto
        {
            Id = m.Id,
            Key = m.Key,
            LabelVi = m.LabelVi,
            LabelEn = m.LabelEn,
            PathVi = m.PathVi,
            PathEn = m.PathEn,
            ParentId = m.ParentId
        };
    }
} 