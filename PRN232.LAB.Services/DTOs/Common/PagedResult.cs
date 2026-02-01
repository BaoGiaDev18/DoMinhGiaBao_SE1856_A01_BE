namespace PRN232.LAB.Services.DTOs.Common
{
    /// <summary>
    /// Paginated result wrapper for Service Layer
    /// Contains data and pagination information
    /// </summary>
    /// <typeparam name="T">The type of DTO being returned</typeparam>
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }

        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public bool HasPrevious => Page > 1;
        public bool HasNext => Page < TotalPages;

        public PagedResult(IEnumerable<T> items, int totalCount, int page, int pageSize)
        {
            Items = items;
            TotalCount = totalCount;
            Page = page;
            PageSize = pageSize;
        }
    }

    /// <summary>
    /// Query parameters for Service Layer
    /// Used to pass filtering, sorting, and paging info from API to Service
    /// </summary>
    public class QueryOptions
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; }
        public string SortOrder { get; set; } = "asc";
        public string? Search { get; set; }
    }
}
