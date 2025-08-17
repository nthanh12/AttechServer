using AttechServer.Applications.UserModules.Dtos.ApiEndpoint;

namespace AttechServer.Applications.UserModules.Abstracts
{
    public interface IApiEndpointService
    {
        Task<List<ApiEndpointDto>> FindAll();
        Task<ApiEndpointDto> FindById(int id);
        Task Create(CreateApiEndpointDto input);
        Task Update(UpdateApiEndpointDto input);
        Task Delete(int id);
        Task<bool> CheckApiPermission(string path, string method, int userId);
    }
}
