namespace DoMinhGiaBao__SE1856_A01_BE.Models.Responses
{
    /// <summary>
    /// API Response Model for tag operations
    /// Used exclusively by the API layer to return tag data to clients
    /// </summary>
    public class TagResponse
    {
        public int TagId { get; set; }
        public string TagName { get; set; } = string.Empty;
        public string? Note { get; set; }
    }

    /// <summary>
    /// Simplified tag response for list/dropdown views
    /// </summary>
    public class TagListResponse
    {
        public int TagId { get; set; }
        public string TagName { get; set; } = string.Empty;
    }
}
