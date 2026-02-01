using DoMinhGiaBao__SE1856_A01_BE.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace DoMinhGiaBao__SE1856_A01_BE.Models.Requests
{
    /// <summary>
    /// Query parameters for SystemAccounts List API
    /// Extends base QueryParameters with account-specific filters
    /// </summary>
    public class SystemAccountQueryParameters : QueryParameters
    {
        /// <summary>
        /// Filter by account role (0=Admin, 1=Staff, 2=Lecturer)
        /// </summary>
        [Range(0, 2, ErrorMessage = "Role must be 0 (Admin), 1 (Staff), or 2 (Lecturer)")]
        public int? Role { get; set; }
    }
}
