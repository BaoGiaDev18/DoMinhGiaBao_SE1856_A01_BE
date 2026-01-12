namespace DoMinhGiaBao__SE1856_A01_BE.Models.Responses
{
    /// <summary>
    /// API Response Model for news article operations
    /// Used exclusively by the API layer to return news article data to clients
    /// </summary>
    public class NewsArticleResponse
    {
        public string NewsArticleId { get; set; } = string.Empty;
        public string NewsTitle { get; set; } = string.Empty;
        public string Headline { get; set; } = string.Empty;
        public DateTime? CreatedDate { get; set; }
        public string? NewsContent { get; set; }
        public string? NewsSource { get; set; }
        public short CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public bool NewsStatus { get; set; }
        public short CreatedById { get; set; }
        public string? CreatedByName { get; set; }
        public short? UpdatedById { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public List<TagResponse> Tags { get; set; } = new List<TagResponse>();
    }

    /// <summary>
    /// Simplified news article response for list views
    /// </summary>
    public class NewsArticleListResponse
    {
        public string NewsArticleId { get; set; } = string.Empty;
        public string NewsTitle { get; set; } = string.Empty;
        public string Headline { get; set; } = string.Empty;
        public DateTime? CreatedDate { get; set; }
        public string? CategoryName { get; set; }
        public string? CreatedByName { get; set; }
        public bool NewsStatus { get; set; }
        public int TagCount { get; set; }
    }

    /// <summary>
    /// News article response for report generation
    /// </summary>
    public class NewsArticleReportResponse
    {
        public string NewsArticleId { get; set; } = string.Empty;
        public string NewsTitle { get; set; } = string.Empty;
        public string Headline { get; set; } = string.Empty;
        public DateTime? CreatedDate { get; set; }
        public string? CategoryName { get; set; }
        public string? CreatedByName { get; set; }
        public bool NewsStatus { get; set; }
    }
}
