using DoMinhGiaBao_SE1856_A01_Repository.Entities;
using DoMinhGiaBao_SE1856_A01_Repository.Repositories;
using DoMinhGiaBao_SE1856_A01_Service.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoMinhGiaBao_SE1856_A01_Service.Services
{
    public class SystemAccountService : ISystemAccountService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SystemAccountService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<SystemAccountDto>> GetAllAccountsAsync()
        {
            var accounts = await _unitOfWork.SystemAccounts.GetAllAsync();
            return accounts.Select(a => new SystemAccountDto
            {
                AccountId = a.AccountId,
                AccountName = a.AccountName,
                AccountEmail = a.AccountEmail,
                AccountRole = a.AccountRole,
                AccountPassword = a.AccountPassword
            });
        }

        public async Task<SystemAccountDto?> GetAccountByIdAsync(short id)
        {
            var account = await _unitOfWork.SystemAccounts.GetByIdAsync(id);
            if (account == null) return null;

            return new SystemAccountDto
            {
                AccountId = account.AccountId,
                AccountName = account.AccountName,
                AccountEmail = account.AccountEmail,
                AccountRole = account.AccountRole,
                AccountPassword = account.AccountPassword
            };
        }

        public async Task<IEnumerable<SystemAccountDto>> SearchAccountsAsync(string searchTerm)
        {
            var accounts = await _unitOfWork.SystemAccounts
                .GetQueryable()
                .Where(a => a.AccountName!.Contains(searchTerm) || a.AccountEmail!.Contains(searchTerm))
                .ToListAsync();

            return accounts.Select(a => new SystemAccountDto
            {
                AccountId = a.AccountId,
                AccountName = a.AccountName,
                AccountEmail = a.AccountEmail,
                AccountRole = a.AccountRole,
                AccountPassword = a.AccountPassword
            });
        }

        public async Task<SystemAccountDto> CreateAccountAsync(CreateSystemAccountDto createDto)
        {
            // Check if email already exists
            var existingAccount = await _unitOfWork.SystemAccounts
                .GetQueryable()
                .FirstOrDefaultAsync(a => a.AccountEmail == createDto.AccountEmail);

            if (existingAccount != null)
            {
                throw new InvalidOperationException("Email already exists");
            }

            var account = new SystemAccount
            {
                AccountName = createDto.AccountName,
                AccountEmail = createDto.AccountEmail,
                AccountRole = createDto.AccountRole,
                AccountPassword = createDto.AccountPassword
            };

            await _unitOfWork.SystemAccounts.AddAsync(account);
            await _unitOfWork.SaveChangesAsync();

            return new SystemAccountDto
            {
                AccountId = account.AccountId,
                AccountName = account.AccountName,
                AccountEmail = account.AccountEmail,
                AccountRole = account.AccountRole,
                AccountPassword = account.AccountPassword
            };
        }

        public async Task<SystemAccountDto?> UpdateAccountAsync(short id, UpdateSystemAccountDto updateDto)
        {
            var account = await _unitOfWork.SystemAccounts.GetByIdAsync(id);
            if (account == null) return null;

            // Check if email already exists for another account
            var existingAccount = await _unitOfWork.SystemAccounts
                .GetQueryable()
                .FirstOrDefaultAsync(a => a.AccountEmail == updateDto.AccountEmail && a.AccountId != id);

            if (existingAccount != null)
            {
                throw new InvalidOperationException("Email already exists");
            }

            account.AccountName = updateDto.AccountName;
            account.AccountEmail = updateDto.AccountEmail;
            account.AccountRole = updateDto.AccountRole;
            
            if (!string.IsNullOrEmpty(updateDto.AccountPassword))
            {
                account.AccountPassword = updateDto.AccountPassword;
            }

            _unitOfWork.SystemAccounts.Update(account);
            await _unitOfWork.SaveChangesAsync();

            return new SystemAccountDto
            {
                AccountId = account.AccountId,
                AccountName = account.AccountName,
                AccountEmail = account.AccountEmail,
                AccountRole = account.AccountRole,
                AccountPassword = account.AccountPassword
            };
        }

        public async Task<bool> CanDeleteAccountAsync(short id)
        {
            // Check if account has created any news articles
            var hasArticles = await _unitOfWork.NewsArticles
                .ExistsAsync(n => n.CreatedById == id);

            return !hasArticles;
        }

        public async Task<bool> DeleteAccountAsync(short id)
        {
            var account = await _unitOfWork.SystemAccounts.GetByIdAsync(id);
            if (account == null) return false;

            // Check if account can be deleted
            if (!await CanDeleteAccountAsync(id))
            {
                throw new InvalidOperationException("Cannot delete account that has created news articles");
            }

            _unitOfWork.SystemAccounts.Delete(account);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
