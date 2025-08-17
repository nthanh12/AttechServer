namespace AttechServer.Shared.ApplicationBase.Common
{
    public class PagingResult<T>
    {
        public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
        public int TotalItems { get; set; }
        
        // Additional properties for backward compatibility
        public int Total => TotalItems;
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
