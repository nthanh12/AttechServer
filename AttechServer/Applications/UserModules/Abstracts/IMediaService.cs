using AttechServer.Applications.UserModules.Dtos.FileUpload;
using AttechServer.Shared.ApplicationBase.Common;

namespace AttechServer.Applications.UserModules.Abstracts
{
    public interface IMediaService
    {
        Task<FileUploadDto> GetById(int id);
        Task<PagingResult<FileUploadDto>> FindAll(PagingRequestBaseDto input);
        Task<PagingResult<FileUploadDto>> FindByEntity(EntityType entityType, int entityId, PagingRequestBaseDto input);
        Task Delete(int id);
        Task DeleteByEntity(EntityType entityType, int entityId);
    }
}