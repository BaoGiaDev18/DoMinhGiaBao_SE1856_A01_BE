using PRN232.LAB.Repo.Repositories;
using PRN232.LAB.Services.DTOs;
using PRN232.LAB.Services.DTOs.Common;
using PRN232.LAB.Services.Extensions;
using PRN232.LAB.Services.Mappers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PRN232.LAB.Services.Services
{
    /// <summary>
    /// SystemAccount Service - Business Logic Layer
    /// Handles all system account-related business operations
    /// Works ONLY with DTOs (Business Models), never directly with Entity Models
    /// </summary>
    public class SystemAccountService : ISystemAccountService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SystemAccountService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Get paginated, sorted, and filtered system accounts
        /// Supports: Paging, Sorting, Search, Filtering by Role
        /// </summary>
        public async Task<PagedResult<SystemAccountDto>> GetAccountsAsync(
            int page,
            int pageSize,
            string? sortBy,
            string sortOrder,
            string? search,
            int? role)
        {
            // Start with base query
            var query = _unitOfWork.SystemAccounts.GetQueryable();

            // Apply role filtering
            if (role.HasValue)
            {
                query = query.Where(a => a.AccountRole == role.Value);
            }

            // Apply search
            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchLower = search.ToLower();
                query = query.Where(a =>
                    (a.AccountName != null && a.AccountName.ToLower().Contains(searchLower)) ||
                    (a.AccountEmail != null && a.AccountEmail.ToLower().Contains(searchLower)));
            }

            // Get total count BEFORE paging
            var totalCount = await query.CountAsync();

            // Apply sorting
            query = query.ApplySorting(sortBy ?? "AccountName", sortOrder);

            // Apply paging
            query = query.ApplyPaging(page, pageSize);

            // Execute query and convert to DTOs
            var accounts = await query.ToListAsync();
            var dtos = accounts.ToAccountDtoList();

            return new PagedResult<SystemAccountDto>(dtos, totalCount, page, pageSize);
        }

        public async Task<IEnumerable<SystemAccountDto>> GetAllAccountsAsync()
        {
            var accounts = await _unitOfWork.SystemAccounts.GetAllAsync();
            // Convert Entity to DTO using mapper
            return accounts.ToAccountDtoList();
        }

        public async Task<SystemAccountDto?> GetAccountByIdAsync(short id)
        {
            var account = await _unitOfWork.SystemAccounts.GetByIdAsync(id);
            if (account == null) return null;

            // Convert Entity to DTO using mapper
            return account.ToDto();
        }

        public async Task<IEnumerable<SystemAccountDto>> SearchAccountsAsync(string searchTerm)
        {
            var accounts = await _unitOfWork.SystemAccounts
                .GetQueryable()
                .Where(a => a.AccountName!.Contains(searchTerm) || a.AccountEmail!.Contains(searchTerm))
                .ToListAsync();

            // Convert Entity to DTO using mapper
            return accounts.ToAccountDtoList();
        }

        public async Task<SystemAccountDto> CreateAccountAsync(CreateSystemAccountDto createDto)
        {
            // Business Logic: Check if email already exists
            var existingAccount = await _unitOfWork.SystemAccounts
                .GetQueryable()
                .FirstOrDefaultAsync(a => a.AccountEmail == createDto.AccountEmail);

            if (existingAccount != null)
            {
                throw new InvalidOperationException("Email already exists");
            }

            // Convert DTO to Entity using mapper
            var account = createDto.ToEntity();

            await _unitOfWork.SystemAccounts.AddAsync(account);
            await _unitOfWork.SaveChangesAsync();

            // Convert saved Entity back to DTO
            return account.ToDto();
        }

        public async Task<SystemAccountDto?> UpdateAccountAsync(short id, UpdateSystemAccountDto updateDto)
        {
            var account = await _unitOfWork.SystemAccounts.GetByIdAsync(id);
            if (account == null) return null;

            // Business Logic: Check if email already exists for another account
            var existingAccount = await _unitOfWork.SystemAccounts
                .GetQueryable()
                .FirstOrDefaultAsync(a => a.AccountEmail == updateDto.AccountEmail && a.AccountId != id);

            if (existingAccount != null)
            {
                throw new InvalidOperationException("Email already exists");
            }

            // Update Entity using mapper method
            account.UpdateEntity(updateDto);

            _unitOfWork.SystemAccounts.Update(account);
            await _unitOfWork.SaveChangesAsync();

            // Convert updated Entity back to DTO
            return account.ToDto();
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
