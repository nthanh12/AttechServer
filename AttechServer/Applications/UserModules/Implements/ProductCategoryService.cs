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
        private readonly IActivityLogService _activityLogService;

        public ProductCategoryService(ApplicationDbContext dbContext, ILogger<ProductCategoryService> logger, IHttpContextAccessor httpContextAccessor, IActivityLogService activityLogService)
        {
            _dbContext = dbContext;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _activityLogService = activityLogService;
        }

        public async Task<ProductCategoryDto> Create(CreateProductCategoryDto input)
        {
            _logger.LogInformation($"{nameof(Create)}: input = {JsonSerializer.Serialize(input)}");
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    // Validate input
                    if (string.IsNullOrWhiteSpace(input.TitleVi) || string.IsNullOrWhiteSpace(input.SlugVi))
                    {
                        throw new ArgumentException("Tên danh m?c và Slug (VI) là b?t bu?c.");
                    }
                    if (string.IsNullOrWhiteSpace(input.TitleEn) || string.IsNullOrWhiteSpace(input.SlugEn))
                    {
                        throw new ArgumentException("Tên danh m?c và Slug (EN) là b?t bu?c.");
                    }
                    if (input.DescriptionVi.Length > 160)
                    {
                        input.DescriptionVi = input.DescriptionVi.Substring(0, 157) + "...";
                    }
                    if (input.DescriptionEn.Length > 160)
                    {
                        input.DescriptionEn = input.DescriptionEn.Substring(0, 157) + "...";
                    }

                    var userId = int.TryParse(_httpContextAccessor.HttpContext?.User?.FindFirst("user_id")?.Value, out var id) ? id : 0;

                    // Ki?m tra trùng tên danh m?c
                    var TitleViExists = await _dbContext.ProductCategories.AnyAsync(c => c.TitleVi == input.TitleVi && !c.Deleted);
                    if (TitleViExists)
                    {
                        throw new ArgumentException("Tên danh m?c (VI) dã t?n t?i.");
                    }
                    var nameEnExists = await _dbContext.ProductCategories.AnyAsync(c => c.TitleEn == input.TitleEn && !c.Deleted);
                    if (nameEnExists)
                    {
                        throw new ArgumentException("Tên danh m?c (EN) dã t?n t?i.");
                    }

                    // Ki?m tra trùng slug
                    var slugViExists = await _dbContext.ProductCategories.AnyAsync(c => c.SlugVi == input.SlugVi && !c.Deleted);
                    if (slugViExists)
                    {
                        throw new ArgumentException("Slug (VI) dã t?n t?i.");
                    }
                    var slugEnExists = await _dbContext.ProductCategories.AnyAsync(c => c.SlugEn == input.SlugEn && !c.Deleted);
                    if (slugEnExists)
                    {
                        throw new ArgumentException("Slug (EN) dã t?n t?i.");
                    }

                    var newProductCategory = new ProductCategory
                    {
                        TitleVi = input.TitleVi,
                        TitleEn = input.TitleEn,
                        SlugVi = input.SlugVi,
                        SlugEn = input.SlugEn,
                        DescriptionVi = input.DescriptionVi,
                        DescriptionEn = input.DescriptionEn,
                        Status = input.Status,
                        CreatedBy = userId,
                        Deleted = false
                    };

                    _dbContext.ProductCategories.Add(newProductCategory);
                    await _dbContext.SaveChangesAsync();

                    await transaction.CommitAsync();

                    // Log activity
                    await _activityLogService.LogUserActionAsync(
                        "CREATE_PRODUCT_CATEGORY",
                        $"Ðã t?o danh m?c s?n ph?m m?i: {input.TitleVi}",
                        userId,
                        JsonSerializer.Serialize(new { 
                            categoryId = newProductCategory.Id,
                            TitleVi = input.TitleVi,
                            nameEn = input.TitleEn,
                            slugVi = input.SlugVi,
                            slugEn = input.SlugEn
                        })
                    );

                    return new ProductCategoryDto
                    {
                        Id = newProductCategory.Id,
                        TitleVi = newProductCategory.TitleVi,
                        TitleEn = newProductCategory.TitleEn,
                        SlugVi = newProductCategory.SlugVi,
                        SlugEn = newProductCategory.SlugEn,
                        DescriptionVi = newProductCategory.DescriptionVi,
                        DescriptionEn = newProductCategory.DescriptionEn,
                        Status = newProductCategory.Status
                    };
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error creating ProductCategory");
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

                    // Xóa m?m các Product liên quan
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
                    && (string.IsNullOrEmpty(input.Keyword) || pc.TitleVi.Contains(input.Keyword) || pc.TitleEn.Contains(input.Keyword)));

            var totalItems = await baseQuery.CountAsync();

            var pagedItems = await baseQuery
                .OrderBy(pc => pc.TitleVi)
                .Skip(input.GetSkip())
                .Take(input.PageSize)
                .Select(pc => new ProductCategoryDto
                {
                    Id = pc.Id,
                    TitleVi = pc.TitleVi,
                    TitleEn = pc.TitleEn,
                    SlugVi = pc.SlugVi,
                    SlugEn = pc.SlugEn,
                    DescriptionVi = pc.DescriptionVi,
                    DescriptionEn = pc.DescriptionEn,
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
                    TitleVi = pc.TitleVi,
                    TitleEn = pc.TitleEn,
                    SlugVi = pc.SlugVi,
                    SlugEn = pc.SlugEn,
                    DescriptionVi = pc.DescriptionVi,
                    DescriptionEn = pc.DescriptionEn,
                    Status = pc.Status,
                    Products = pc.Products
                        .Where(p => !p.Deleted && p.Status == CommonStatus.ACTIVE)
                        .Select(p => new ProductDto
                        {
                            Id = p.Id,
                            TitleVi = p.TitleVi,
                            TitleEn = p.TitleEn,
                            SlugVi = p.SlugVi,
                            SlugEn = p.SlugEn,
                            DescriptionVi = p.DescriptionVi,
                            DescriptionEn = p.DescriptionEn,
                            TimePosted = p.TimePosted,
                            Status = p.Status,
                            ProductCategoryId = p.ProductCategoryId,
                            ProductCategoryTitleVi = pc.TitleVi,
                            ProductCategoryTitleEn = pc.TitleEn,
                            ProductCategorySlugVi = pc.SlugVi,
                            ProductCategorySlugEn = pc.SlugEn,
                            ImageUrl = p.ImageUrl
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
                    if (string.IsNullOrWhiteSpace(input.TitleVi) || string.IsNullOrWhiteSpace(input.SlugVi))
                    {
                        throw new ArgumentException("Tên danh m?c và Slug (VI) là b?t bu?c.");
                    }
                    if (string.IsNullOrWhiteSpace(input.TitleEn) || string.IsNullOrWhiteSpace(input.SlugEn))
                    {
                        throw new ArgumentException("Tên danh m?c và Slug (EN) là b?t bu?c.");
                    }
                    if (input.DescriptionVi.Length > 160)
                    {
                        input.DescriptionVi = input.DescriptionVi.Substring(0, 157) + "...";
                    }
                    if (input.DescriptionEn.Length > 160)
                    {
                        input.DescriptionEn = input.DescriptionEn.Substring(0, 157) + "...";
                    }

                    var userId = int.TryParse(_httpContextAccessor.HttpContext?.User?.FindFirst("user_id")?.Value, out var id) ? id : 0;

                    // Ki?m tra trùng tên danh m?c (tr? chính nó)
                    var TitleViExists = await _dbContext.ProductCategories.AnyAsync(c => c.TitleVi == input.TitleVi && !c.Deleted && c.Id != input.Id);
                    if (TitleViExists)
                    {
                        throw new ArgumentException("Tên danh m?c (VI) dã t?n t?i.");
                    }
                    var nameEnExists = await _dbContext.ProductCategories.AnyAsync(c => c.TitleEn == input.TitleEn && !c.Deleted && c.Id != input.Id);
                    if (nameEnExists)
                    {
                        throw new ArgumentException("Tên danh m?c (EN) dã t?n t?i.");
                    }

                    // Ki?m tra trùng slug (tr? chính nó)
                    var slugViExists = await _dbContext.ProductCategories.AnyAsync(c => c.SlugVi == input.SlugVi && !c.Deleted && c.Id != input.Id);
                    if (slugViExists)
                    {
                        throw new ArgumentException("Slug (VI) dã t?n t?i.");
                    }
                    var slugEnExists = await _dbContext.ProductCategories.AnyAsync(c => c.SlugEn == input.SlugEn && !c.Deleted && c.Id != input.Id);
                    if (slugEnExists)
                    {
                        throw new ArgumentException("Slug (EN) dã t?n t?i.");
                    }

                    productCategory.TitleVi = input.TitleVi;
                    productCategory.TitleEn = input.TitleEn;
                    productCategory.SlugVi = input.SlugVi;
                    productCategory.SlugEn = input.SlugEn;
                    productCategory.DescriptionVi = input.DescriptionVi;
                    productCategory.DescriptionEn = input.DescriptionEn;
                    productCategory.Status = input.Status;
                    productCategory.ModifiedBy = userId;
                    // ModifiedDate s? du?c set t? d?ng trong CheckAudit()

                    await _dbContext.SaveChangesAsync();

                    await transaction.CommitAsync();
                    return new ProductCategoryDto
                    {
                        Id = productCategory.Id,
                        TitleVi = productCategory.TitleVi,
                        TitleEn = productCategory.TitleEn,
                        SlugVi = productCategory.SlugVi,
                        SlugEn = productCategory.SlugEn,
                        DescriptionVi = productCategory.DescriptionVi,
                        DescriptionEn = productCategory.DescriptionEn,
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
            var productCategory = await _dbContext.ProductCategories.FirstOrDefaultAsync(pc => pc.Id == id && !pc.Deleted)
                ?? throw new UserFriendlyException(ErrorCode.ProductCategoryNotFound);
            productCategory.Status = status;
            await _dbContext.SaveChangesAsync();
        }
    }
}
