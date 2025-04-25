namespace AttechServer.Shared.ApplicationBase.Common
{
    public class PagingResult<T>
    {
        public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
        public int TotalItems { get; set; }
    }
}
