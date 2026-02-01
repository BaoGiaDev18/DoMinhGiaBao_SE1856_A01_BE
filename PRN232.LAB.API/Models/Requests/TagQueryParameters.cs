using DoMinhGiaBao__SE1856_A01_BE.Models.Common;

namespace DoMinhGiaBao__SE1856_A01_BE.Models.Requests
{
    /// <summary>
    /// Query parameters for Tags List API
    /// Extends base QueryParameters with tag-specific filters
    /// </summary>
    public class TagQueryParameters : QueryParameters
    {
        // Tags don't have complex filters, just use base Search
    }
}
