using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Applications.UserModules.Dtos.Product;
using AttechServer.Applications.UserModules.Dtos.ProductCategory;
using AttechServer.Domains.Entities.Main;
using AttechServer.Infrastructures.Persistances;
using AttechServer.Shared.ApplicationBase.Common;
using AttechServer.Shared.Consts;
using AttechServer.Shared.Consts.Exceptions;
using AttechServer.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace AttechServer.Applications.UserModules.Implements
{
    public class ProductCategoryService : IProductCategoryService
    {
        private readonly ILogger<ProductCategoryService> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductCategoryService(ApplicationDbContext dbContext, ILogger<ProductCategoryService> logger, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ProductCategoryDto> Create(CreateProductCategoryDto input)
        {
            _logger.LogInformation($"{nameof(Create)}: input = {JsonSerializer.Serialize(input)}");
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    // Validate input
                    if (string.IsNullOrWhiteSpace(input.Name))
                    {
                        throw new ArgumentException("Tên danh mục là bắt buộc.");
                    }
                    if (input.Description.Length > 160)
                    {
                        input.Description = input.Description.Substring(0, 157) + "...";
                    }

                    var userId = int.TryParse(_httpContextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value, out var id) ? id : 0;

                    // Tạo slug tự động
                    var slug = GenerateSlug(input.Name);
                    var slugExists = await _dbContext.ProductCategories.AnyAsync(c => c.Slug == slug && !c.Deleted);
                    if (slugExists)
                    {
                        slug = $"{slug}-{DateTime.Now.Ticks}";
                    }

                    var newProductCategory = new ProductCategory
                    {
                        Name = input.Name,
                        Slug = slug,
                        Description = input.Description,
                        Status = input.Status,
                        CreatedDate = DateTime.UtcNow,
                        CreatedBy = userId,
                        Deleted = false
                    };

                    _dbContext.ProductCategories.Add(newProductCategory);
                    await _dbContext.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return new ProductCategoryDto
                    {
                        Id = newProductCategory.Id,
                        Name = newProductCategory.Name,
                        Slug = newProductCategory.Slug,
                        Description = newProductCategory.Description,
                        Status = newProductCategory.Status
                    };
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error creating Product category");
                    throw;
                }
            }
        }

        public async Task Delete(int id)
        {
            _logger.LogInformation($"{nameof(Delete)}: id = {id}");

            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var ProductCategory = await _dbContext.ProductCategories
                        .FirstOrDefaultAsync(pc => pc.Id == id && !pc.Deleted)
                        ?? throw new UserFriendlyException(ErrorCode.ProductCategoryNotFound);

                    // Xóa mềm các Product liên quan
                    var Products = await _dbContext.Products
                        .Where(p => p.ProductCategoryId == id && !p.Deleted)
                        .ToListAsync();

                    foreach (var Product in Products)
                    {
                        Product.Deleted = true;
                    }

                    ProductCategory.Deleted = true;
                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, $"Error deleting Product category with id = {id}");
                    throw;
                }
            }
        }

        public async Task<PagingResult<ProductCategoryDto>> FindAll(PagingRequestBaseDto input)
        {
            _logger.LogInformation($"{nameof(FindAll)}: input = {JsonSerializer.Serialize(input)}");

            var baseQuery = _dbContext.ProductCategories.AsNoTracking()
                .Where(pc => !pc.Deleted
                    && (string.IsNullOrEmpty(input.Keyword) || pc.Name.Contains(input.Keyword)));

            var totalItems = await baseQuery.CountAsync();

            var pagedItems = await baseQuery
                .OrderBy(pc => pc.Name)
                .Skip(input.GetSkip())
                .Take(input.PageSize)
                .Select(pc => new ProductCategoryDto
                {
                    Id = pc.Id,
                    Name = pc.Name,
                    Slug = pc.Slug,
                    Description = pc.Description,
                    Status = pc.Status
                })
                .ToListAsync();

            return new PagingResult<ProductCategoryDto>
            {
                TotalItems = totalItems,
                Items = pagedItems
            };
        }

        public async Task<DetailProductCategoryDto> FindById(int id)
        {
            _logger.LogInformation($"{nameof(FindById)}: id = {id}");

            var productCategory = await _dbContext.ProductCategories
                .Where(pc => !pc.Deleted && pc.Id == id && pc.Status == CommonStatus.ACTIVE)
                .Select(pc => new DetailProductCategoryDto
                {
                    Id = pc.Id,
                    Name = pc.Name,
                    Slug = pc.Slug,
                    Description = pc.Description,
                    Status = pc.Status,
                    Products = pc.Products
                        .Where(p => !p.Deleted && p.Status == CommonStatus.ACTIVE)
                        .Select(p => new ProductDto
                        {
                            Id = p.Id,
                            Name = p.Name,
                            Slug = p.Slug,
                            Description = p.Description,
                            TimePosted = p.TimePosted,
                            Status = p.Status,
                            ProductCategoryId = p.ProductCategoryId,
                            ProductCategoryName = pc.Name,
                            ProductCategorySlug = pc.Slug
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (productCategory == null)
                throw new UserFriendlyException(ErrorCode.ProductCategoryNotFound);

            return productCategory;
        }

        public async Task<ProductCategoryDto> Update(UpdateProductCategoryDto input)
        {
            _logger.LogInformation($"{nameof(Update)}: input = {JsonSerializer.Serialize(input)}");
            var productCategory = await _dbContext.ProductCategories.FirstOrDefaultAsync(pc => pc.Id == input.Id && !pc.Deleted)
                ?? throw new UserFriendlyException(ErrorCode.ProductCategoryNotFound);
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    // Validate input
                    if (string.IsNullOrWhiteSpace(input.Name))
                    {
                        throw new ArgumentException("Tên danh mục là bắt buộc.");
                    }
                    if (input.Description.Length > 160)
                    {
                        input.Description = input.Description.Substring(0, 157) + "...";
                    }

                    var userId = int.TryParse(_httpContextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value, out var id) ? id : 0;

                    // Cập nhật slug nếu tên thay đổi
                    var slug = GenerateSlug(input.Name);
                    if (slug != productCategory.Slug)
                    {
                        var slugExists = await _dbContext.ProductCategories.AnyAsync(c => c.Slug == slug && !c.Deleted && c.Id != input.Id);
                        if (slugExists)
                        {
                            slug = $"{slug}-{DateTime.Now.Ticks}";
                        }
                        productCategory.Slug = slug;
                    }

                    productCategory.Name = input.Name;
                    productCategory.Description = input.Description;
                    productCategory.ModifiedBy = userId;
                    productCategory.ModifiedDate = DateTime.UtcNow;

                    await _dbContext.SaveChangesAsync();

                    await transaction.CommitAsync();
                    return new ProductCategoryDto
                    {
                        Id = productCategory.Id,
                        Name = productCategory.Name,
                        Slug = productCategory.Slug,
                        Description = productCategory.Description,
                        Status = productCategory.Status
                    };
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error updating Product category");
                    throw;
                }
            }
        }

        public async Task UpdateStatusProductCategory(int id, int status)
        {
            _logger.LogInformation($"{nameof(UpdateStatusProductCategory)}: Id = {id}, status = {status}");
            var ProductCategory = await _dbContext.ProductCategories.FirstOrDefaultAsync(pc => pc.Id == id && !pc.Deleted)
                ?? throw new UserFriendlyException(ErrorCode.ProductCategoryNotFound);
            ProductCategory.Status = status;
            await _dbContext.SaveChangesAsync();
        }

        private string GenerateSlug(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // Chuyển về chữ thường, loại bỏ dấu tiếng Việt
            var slug = input.ToLowerInvariant()
                .Normalize(NormalizationForm.FormD)
                .Where(c => char.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                .Aggregate(new StringBuilder(), (sb, c) => sb.Append(c))
                .ToString()
                .Normalize(NormalizationForm.FormC);

            // Thay khoảng trắng bằng dấu gạch ngang, loại bỏ ký tự không hợp lệ
            slug = Regex.Replace(slug, @"\s+", "-");
            slug = Regex.Replace(slug, @"[^a-z0-9-]", "");
            slug = slug.Trim('-');

            return slug;
        }
    }
}