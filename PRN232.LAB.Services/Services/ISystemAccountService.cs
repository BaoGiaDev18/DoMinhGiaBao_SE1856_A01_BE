using PRN232.LAB.Services.DTOs;
using PRN232.LAB.Services.DTOs.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PRN232.LAB.Services.Services
{
    public interface ISystemAccountService
    {
        // New paginated method
        Task<PagedResult<SystemAccountDto>> GetAccountsAsync(
            int page,
            int pageSize,
            string? sortBy,
            string sortOrder,
            string? search,
            int? role);

        // Legacy methods (for backward compatibility)
        Task<IEnumerable<SystemAccountDto>> GetAllAccountsAsync();
        Task<SystemAccountDto?> GetAccountByIdAsync(short id);
        Task<IEnumerable<SystemAccountDto>> SearchAccountsAsync(string searchTerm);
        Task<SystemAccountDto> CreateAccountAsync(CreateSystemAccountDto createDto);
        Task<SystemAccountDto?> UpdateAccountAsync(short id, UpdateSystemAccountDto updateDto);
        Task<bool> DeleteAccountAsync(short id);
        Task<bool> CanDeleteAccountAsync(short id);
    }
}

