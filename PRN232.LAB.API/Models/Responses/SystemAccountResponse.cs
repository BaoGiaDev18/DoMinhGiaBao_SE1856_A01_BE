namespace DoMinhGiaBao__SE1856_A01_BE.Models.Responses
{
    /// <summary>
    /// API Response Model for system account operations
    /// Used exclusively by the API layer to return account data to clients
    /// </summary>
    public class SystemAccountResponse
    {
        public short AccountId { get; set; }
        public string AccountName { get; set; } = string.Empty;
        public string AccountEmail { get; set; } = string.Empty;
        public int AccountRole { get; set; }
        public string RoleName { get; set; } = string.Empty; // "Admin", "Staff", "Lecturer"
    }

    /// <summary>
    /// Simplified system account response for list views
    /// </summary>
    public class SystemAccountListResponse
    {
        public short AccountId { get; set; }
        public string AccountName { get; set; } = string.Empty;
        public string AccountEmail { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
    }
}
