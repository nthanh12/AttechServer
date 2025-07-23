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
                    if (string.IsNullOrWhiteSpace(input.NameVi) || string.IsNullOrWhiteSpace(input.SlugVi))
                    {
                        throw new ArgumentException("Tên danh mục và Slug (VI) là bắt buộc.");
                    }
                    if (string.IsNullOrWhiteSpace(input.NameEn) || string.IsNullOrWhiteSpace(input.SlugEn))
                    {
                        throw new ArgumentException("Tên danh mục và Slug (EN) là bắt buộc.");
                    }
                    if (input.DescriptionVi.Length > 160)
                    {
                        input.DescriptionVi = input.DescriptionVi.Substring(0, 157) + "...";
                    }
                    if (input.DescriptionEn.Length > 160)
                    {
                        input.DescriptionEn = input.DescriptionEn.Substring(0, 157) + "...";
                    }

                    var userId = int.TryParse(_httpContextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value, out var id) ? id : 0;

                    // Kiểm tra trùng slug
                    var slugViExists = await _dbContext.ProductCategories.AnyAsync(c => c.SlugVi == input.SlugVi && !c.Deleted);
                    if (slugViExists)
                    {
                        throw new ArgumentException("Slug VI đã tồn tại.");
                    }
                    var slugEnExists = await _dbContext.ProductCategories.AnyAsync(c => c.SlugEn == input.SlugEn && !c.Deleted);
                    if (slugEnExists)
                    {
                        throw new ArgumentException("Slug EN đã tồn tại.");
                    }

                    var newProductCategory = new ProductCategory
                    {
                        NameVi = input.NameVi,
                        NameEn = input.NameEn,
                        SlugVi = input.SlugVi,
                        SlugEn = input.SlugEn,
                        DescriptionVi = input.DescriptionVi,
                        DescriptionEn = input.DescriptionEn,
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
                        NameVi = newProductCategory.NameVi,
                        NameEn = newProductCategory.NameEn,
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
                    && (string.IsNullOrEmpty(input.Keyword) || pc.NameVi.Contains(input.Keyword) || pc.NameEn.Contains(input.Keyword)));

            var totalItems = await baseQuery.CountAsync();

            var pagedItems = await baseQuery
                .OrderBy(pc => pc.NameVi)
                .Skip(input.GetSkip())
                .Take(input.PageSize)
                .Select(pc => new ProductCategoryDto
                {
                    Id = pc.Id,
                    NameVi = pc.NameVi,
                    NameEn = pc.NameEn,
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
                    NameVi = pc.NameVi,
                    NameEn = pc.NameEn,
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
                            NameVi = p.NameVi,
                            NameEn = p.NameEn,
                            SlugVi = p.SlugVi,
                            SlugEn = p.SlugEn,
                            DescriptionVi = p.DescriptionVi,
                            DescriptionEn = p.DescriptionEn,
                            TimePosted = p.TimePosted,
                            Status = p.Status,
                            ProductCategoryId = p.ProductCategoryId,
                            ProductCategoryName = pc.NameVi,
                            ProductCategorySlug = pc.SlugEn
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
                    if (string.IsNullOrWhiteSpace(input.NameVi) || string.IsNullOrWhiteSpace(input.SlugVi))
                    {
                        throw new ArgumentException("Tên danh mục và Slug (VI) là bắt buộc.");
                    }
                    if (string.IsNullOrWhiteSpace(input.NameEn) || string.IsNullOrWhiteSpace(input.SlugEn))
                    {
                        throw new ArgumentException("Tên danh mục và Slug (EN) là bắt buộc.");
                    }
                    if (input.DescriptionVi.Length > 160)
                    {
                        input.DescriptionVi = input.DescriptionVi.Substring(0, 157) + "...";
                    }
                    if (input.DescriptionEn.Length > 160)
                    {
                        input.DescriptionEn = input.DescriptionEn.Substring(0, 157) + "...";
                    }

                    var userId = int.TryParse(_httpContextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value, out var id) ? id : 0;

                    // Kiểm tra trùng slug (trừ chính nó)
                    var slugViExists = await _dbContext.ProductCategories.AnyAsync(c => c.SlugVi == input.SlugVi && !c.Deleted && c.Id != input.Id);
                    if (slugViExists)
                    {
                        throw new ArgumentException("Slug VI đã tồn tại.");
                    }
                    var slugEnExists = await _dbContext.ProductCategories.AnyAsync(c => c.SlugEn == input.SlugEn && !c.Deleted && c.Id != input.Id);
                    if (slugEnExists)
                    {
                        throw new ArgumentException("Slug EN đã tồn tại.");
                    }

                    productCategory.NameVi = input.NameVi;
                    productCategory.NameEn = input.NameEn;
                    productCategory.SlugVi = input.SlugVi;
                    productCategory.SlugEn = input.SlugEn;
                    productCategory.DescriptionVi = input.DescriptionVi;
                    productCategory.DescriptionEn = input.DescriptionEn;
                    productCategory.ModifiedBy = userId;
                    productCategory.ModifiedDate = DateTime.UtcNow;

                    await _dbContext.SaveChangesAsync();

                    await transaction.CommitAsync();
                    return new ProductCategoryDto
                    {
                        Id = productCategory.Id,
                        NameVi = productCategory.NameVi,
                        NameEn = productCategory.NameEn,
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