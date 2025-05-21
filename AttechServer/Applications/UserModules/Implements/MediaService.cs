using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Applications.UserModules.Dtos.FileUpload;
using AttechServer.Domains.Entities.Main;
using AttechServer.Infrastructures.Abstractions;
using AttechServer.Infrastructures.Persistances;
using AttechServer.Shared.ApplicationBase.Common;
using AttechServer.Shared.Consts.Exceptions;
using AttechServer.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace AttechServer.Applications.UserModules.Implements
{
    public class MediaService : IMediaService
    {
        private readonly ILogger<MediaService> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWysiwygFileProcessor _wysiwygFileProcessor;

        public MediaService(ApplicationDbContext dbContext, ILogger<MediaService> logger, IHttpContextAccessor httpContextAccessor, IWysiwygFileProcessor wysiwygFileProcessor)
        {
            _dbContext = dbContext;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _wysiwygFileProcessor = wysiwygFileProcessor;
        }

        public async Task<FileUploadDto> GetById(int id)
        {
            _logger.LogInformation($"{nameof(GetById)}: id = {id}");

            var file = await _dbContext.FileUploads
                .Where(f => f.Id == id)
                .Select(f => new FileUploadDto
                {
                    Id = f.Id,
                    FileName = Path.GetFileName(f.FilePath),
                    FileType = f.FileType,
                    FilePath = f.FilePath,
                    EntityType = f.EntityType,
                    EntityId = f.EntityId,
                    CreatedDate = f.CreatedDate
                })
                .FirstOrDefaultAsync();

            if (file == null)
            {
                throw new UserFriendlyException(ErrorCode.NotFound);
            }

            return file;
        }

        public async Task<PagingResult<FileUploadDto>> FindAll(PagingRequestBaseDto input)
        {
            _logger.LogInformation($"{nameof(FindAll)}: input = {JsonSerializer.Serialize(input)}");

            var baseQuery = _dbContext.FileUploads.AsNoTracking()
                .Where(f => f.EntityType != EntityType.Temp);

            // Tìm kiếm
            if (!string.IsNullOrEmpty(input.Keyword))
            {
                baseQuery = baseQuery.Where(f =>
                    f.FilePath.Contains(input.Keyword) ||
                    f.FileType.Contains(input.Keyword));
            }

            var totalItems = await baseQuery.CountAsync();

            // Sắp xếp
            var query = baseQuery.OrderByDescending(f => f.CreatedDate);
            if (input.Sort.Any())
            {
                // TODO: Implement dynamic sorting based on input.Sort
            }

            var pagedItems = await query
                .Skip(input.GetSkip())
                .Take(input.PageSize)
                .Select(f => new FileUploadDto
                {
                    Id = f.Id,
                    FileName = Path.GetFileName(f.FilePath),
                    FileType = f.FileType,
                    FilePath = f.FilePath,
                    EntityType = f.EntityType,
                    EntityId = f.EntityId,
                    CreatedDate = f.CreatedDate
                })
                .ToListAsync();

            return new PagingResult<FileUploadDto>
            {
                TotalItems = totalItems,
                Items = pagedItems
            };
        }

        public async Task<PagingResult<FileUploadDto>> FindByEntity(EntityType entityType, int entityId, PagingRequestBaseDto input)
        {
            _logger.LogInformation($"{nameof(FindByEntity)}: entityType = {entityType}, entityId = {entityId}, input = {JsonSerializer.Serialize(input)}");

            var baseQuery = _dbContext.FileUploads.AsNoTracking()
                .Where(f => f.EntityType == entityType && f.EntityId == entityId);

            // Tìm kiếm
            if (!string.IsNullOrEmpty(input.Keyword))
            {
                baseQuery = baseQuery.Where(f =>
                    f.FilePath.Contains(input.Keyword) ||
                    f.FileType.Contains(input.Keyword));
            }

            var totalItems = await baseQuery.CountAsync();

            // Sắp xếp
            var query = baseQuery.OrderByDescending(f => f.CreatedDate);
            if (input.Sort.Any())
            {
                // TODO: Implement dynamic sorting based on input.Sort
            }

            var pagedItems = await query
                .Skip(input.GetSkip())
                .Take(input.PageSize)
                .Select(f => new FileUploadDto
                {
                    Id = f.Id,
                    FileName = Path.GetFileName(f.FilePath),
                    FileType = f.FileType,
                    FilePath = f.FilePath,
                    EntityType = f.EntityType,
                    EntityId = f.EntityId,
                    CreatedDate = f.CreatedDate
                })
                .ToListAsync();

            return new PagingResult<FileUploadDto>
            {
                TotalItems = totalItems,
                Items = pagedItems
            };
        }

        public async Task Delete(int id)
        {
            _logger.LogInformation($"{nameof(Delete)}: id = {id}");

            var file = await _dbContext.FileUploads
                .FirstOrDefaultAsync(f => f.Id == id)
                ?? throw new UserFriendlyException(ErrorCode.NotFound);

            // Xóa file vật lý
            await _wysiwygFileProcessor.DeleteFilesAsync(file.EntityType, file.EntityId);
        }

        public async Task DeleteByEntity(EntityType entityType, int entityId)
        {
            _logger.LogInformation($"{nameof(DeleteByEntity)}: entityType = {entityType}, entityId = {entityId}");

            // Xóa file vật lý
            await _wysiwygFileProcessor.DeleteFilesAsync(entityType, entityId);
        }
    }
}