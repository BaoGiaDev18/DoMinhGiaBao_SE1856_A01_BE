using DoMinhGiaBao_SE1856_A01_Service.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DoMinhGiaBao_SE1856_A01_Service.Services
{
    public interface ISystemAccountService
    {
        Task<IEnumerable<SystemAccountDto>> GetAllAccountsAsync();
        Task<SystemAccountDto?> GetAccountByIdAsync(short id);
        Task<IEnumerable<SystemAccountDto>> SearchAccountsAsync(string searchTerm);
        Task<SystemAccountDto> CreateAccountAsync(CreateSystemAccountDto createDto);
        Task<SystemAccountDto?> UpdateAccountAsync(short id, UpdateSystemAccountDto updateDto);
        Task<bool> DeleteAccountAsync(short id);
        Task<bool> CanDeleteAccountAsync(short id);
    }
}
