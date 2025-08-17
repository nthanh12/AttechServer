namespace AttechServer.Shared.Services
{
    public interface IUrlService
    {
        string GetBaseUrl();
        string GetFullUrl(string relativePath);
        string GetFileUrl(string relativePath);
    }
}
