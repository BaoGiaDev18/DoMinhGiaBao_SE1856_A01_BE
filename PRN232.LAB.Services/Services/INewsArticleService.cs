using PRN232.LAB.Services.DTOs;
using PRN232.LAB.Services.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PRN232.LAB.Services.Services
{
    public interface INewsArticleService
    {
        // New paginated method
        Task<PagedResult<NewsArticleDto>> GetNewsArticlesAsync(
            int page,
            int pageSize,
            string? sortBy,
            string sortOrder,
            string? search,
            string? status,
            short? createdById,
            short? categoryId,
            DateTime? startDate,
            DateTime? endDate,
            int? tagId);

        // Legacy methods (for backward compatibility)
        Task<IEnumerable<NewsArticleDto>> GetAllNewsArticlesAsync();
        Task<IEnumerable<NewsArticleDto>> GetActiveNewsArticlesAsync();
        Task<IEnumerable<NewsArticleDto>> GetNewsArticlesByCreatorAsync(short creatorId);
        Task<NewsArticleDto?> GetNewsArticleByIdAsync(string id);
        Task<NewsArticleDto> CreateNewsArticleAsync(CreateNewsArticleDto createDto, short createdById);
        Task<NewsArticleDto?> UpdateNewsArticleAsync(string id, UpdateNewsArticleDto updateDto, short updatedById);
        Task<bool> DeleteNewsArticleAsync(string id);
        Task<IEnumerable<NewsArticleReportDto>> GetNewsArticleReportAsync(DateTime startDate, DateTime endDate);
    }
}

