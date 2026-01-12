using DoMinhGiaBao_SE1856_A01_Service.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DoMinhGiaBao_SE1856_A01_Service.Services
{
    public interface INewsArticleService
    {
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
