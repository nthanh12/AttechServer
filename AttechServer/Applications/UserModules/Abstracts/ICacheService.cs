namespace AttechServer.Applications.UserModules.Abstracts
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key) where T : class;
        Task SetAsync<T>(string key, T value, TimeSpan expiration) where T : class;
        Task RemoveAsync(string key);
        Task RemoveByPatternAsync(string pattern);
        Task ClearAllAsync();
        Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> getItem, TimeSpan expiration) where T : class;
    }
}
