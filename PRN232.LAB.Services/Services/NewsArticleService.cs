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
    /// NewsArticle Service - Business Logic Layer
    /// Handles all news article-related business operations
    /// Works ONLY with DTOs (Business Models), never directly with Entity Models
    /// </summary>
    public class NewsArticleService : INewsArticleService
    {
        private readonly IUnitOfWork _unitOfWork;

        public NewsArticleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Get paginated, sorted, and filtered news articles
        /// Supports: Paging, Sorting, Search, Multiple Filters
        /// </summary>
        public async Task<PagedResult<NewsArticleDto>> GetNewsArticlesAsync(
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
            int? tagId)
        {
            // Start with base query including related entities
            IQueryable<PRN232.LAB.Repo.Entities.NewsArticle> query = 
                _unitOfWork.NewsArticles
                    .GetQueryable()
                    .Include(n => n.Category)
                    .Include(n => n.CreatedBy)
                    .Include(n => n.Tags);

            // Apply status filtering
            if (!string.IsNullOrWhiteSpace(status))
            {
                var isActive = status.ToLower() == "active";
                query = query.Where(n => n.NewsStatus == isActive);
            }

            // Apply creator filtering
            if (createdById.HasValue)
            {
                query = query.Where(n => n.CreatedById == createdById.Value);
            }

            // Apply category filtering
            if (categoryId.HasValue)
            {
                query = query.Where(n => n.CategoryId == categoryId.Value);
            }

            // Apply date range filtering
            if (startDate.HasValue)
            {
                query = query.Where(n => n.CreatedDate >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                query = query.Where(n => n.CreatedDate <= endDate.Value);
            }

            // Apply tag filtering
            if (tagId.HasValue)
            {
                query = query.Where(n => n.Tags.Any(t => t.TagId == tagId.Value));
            }

            // Apply search
            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchLower = search.ToLower();
                query = query.Where(n =>
                    (n.NewsTitle != null && n.NewsTitle.ToLower().Contains(searchLower)) ||
                    (n.Headline != null && n.Headline.ToLower().Contains(searchLower)) ||
                    (n.NewsContent != null && n.NewsContent.ToLower().Contains(searchLower)));
            }

            // Get total count BEFORE paging
            var totalCount = await query.CountAsync();

            // Apply sorting
            query = query.ApplySorting(sortBy ?? "CreatedDate", sortOrder);

            // Apply paging
            query = query.ApplyPaging(page, pageSize);

            // Execute query and convert to DTOs
            var articles = await query.ToListAsync();
            var dtos = articles.ToNewsArticleDtoList();

            return new PagedResult<NewsArticleDto>(dtos, totalCount, page, pageSize);
        }

        public async Task<IEnumerable<NewsArticleDto>> GetAllNewsArticlesAsync()
        {
            var articles = await _unitOfWork.NewsArticles
                .GetQueryable()
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Include(n => n.Tags)
                .ToListAsync();

            // Convert Entity to DTO using mapper
            return articles.ToNewsArticleDtoList();
        }

        public async Task<IEnumerable<NewsArticleDto>> GetActiveNewsArticlesAsync()
        {
            var articles = await _unitOfWork.NewsArticles
                .GetQueryable()
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Include(n => n.Tags)
                .Where(n => n.NewsStatus == true)
                .ToListAsync();

            // Convert Entity to DTO using mapper
            return articles.ToNewsArticleDtoList();
        }

        public async Task<IEnumerable<NewsArticleDto>> GetNewsArticlesByCreatorAsync(short creatorId)
        {
            var articles = await _unitOfWork.NewsArticles
                .GetQueryable()
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Include(n => n.Tags)
                .Where(n => n.CreatedById == creatorId)
                .ToListAsync();

            // Convert Entity to DTO using mapper
            return articles.ToNewsArticleDtoList();
        }

        public async Task<NewsArticleDto?> GetNewsArticleByIdAsync(string id)
        {
            var article = await _unitOfWork.NewsArticles
                .GetQueryable()
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Include(n => n.Tags)
                .FirstOrDefaultAsync(n => n.NewsArticleId == id);

            if (article == null) return null;

            // Convert Entity to DTO using mapper
            return article.ToDto();
        }

        public async Task<NewsArticleDto> CreateNewsArticleAsync(CreateNewsArticleDto createDto, short createdById)
        {
            // Business Logic: Check if article ID already exists
            var exists = await _unitOfWork.NewsArticles
                .ExistsAsync(n => n.NewsArticleId == createDto.NewsArticleId);

            if (exists)
            {
                throw new InvalidOperationException("News Article ID already exists");
            }

            // Business Logic: Validate CategoryId
            var categoryExists = await _unitOfWork.Categories
                .ExistsAsync(c => c.CategoryId == createDto.CategoryId);
            
            if (!categoryExists)
            {
                throw new InvalidOperationException($"Category with ID {createDto.CategoryId} does not exist");
            }

            // Business Logic: Validate createdById
            var accountExists = await _unitOfWork.SystemAccounts
                .ExistsAsync(a => a.AccountId == createdById);
            
            if (!accountExists)
            {
                throw new InvalidOperationException($"System account with ID {createdById} does not exist");
            }

            // Convert DTO to Entity using mapper
            var article = createDto.ToEntity(createdById);

            // Add tags
            if (createDto.TagIds.Any())
            {
                var tags = await _unitOfWork.Tags
                    .GetQueryable()
                    .Where(t => createDto.TagIds.Contains(t.TagId))
                    .ToListAsync();

                article.Tags = tags;
            }

            await _unitOfWork.NewsArticles.AddAsync(article);
            await _unitOfWork.SaveChangesAsync();

            // Reload with includes
            var createdArticle = await _unitOfWork.NewsArticles
                .GetQueryable()
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Include(n => n.Tags)
                .FirstOrDefaultAsync(n => n.NewsArticleId == article.NewsArticleId);

            // Convert saved Entity back to DTO
            return createdArticle!.ToDto();
        }

        public async Task<NewsArticleDto?> UpdateNewsArticleAsync(string id, UpdateNewsArticleDto updateDto, short updatedById)
        {
            var article = await _unitOfWork.NewsArticles
                .GetQueryable()
                .Include(n => n.Tags)
                .FirstOrDefaultAsync(n => n.NewsArticleId == id);

            if (article == null) return null;

            // Business Logic: Validate CategoryId
            var categoryExists = await _unitOfWork.Categories
                .ExistsAsync(c => c.CategoryId == updateDto.CategoryId);
            
            if (!categoryExists)
            {
                throw new InvalidOperationException($"Category with ID {updateDto.CategoryId} does not exist");
            }

            // Business Logic: Validate updatedById
            var accountExists = await _unitOfWork.SystemAccounts
                .ExistsAsync(a => a.AccountId == updatedById);
            
            if (!accountExists)
            {
                throw new InvalidOperationException($"System account with ID {updatedById} does not exist");
            }

            // Update Entity using mapper method
            article.UpdateEntity(updateDto, updatedById);

            // Update tags
            article.Tags.Clear();
            if (updateDto.TagIds.Any())
            {
                var tags = await _unitOfWork.Tags
                    .GetQueryable()
                    .Where(t => updateDto.TagIds.Contains(t.TagId))
                    .ToListAsync();

                article.Tags = tags;
            }

            _unitOfWork.NewsArticles.Update(article);
            await _unitOfWork.SaveChangesAsync();

            // Reload with includes
            var updatedArticle = await _unitOfWork.NewsArticles
                .GetQueryable()
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Include(n => n.Tags)
                .FirstOrDefaultAsync(n => n.NewsArticleId == id);

            // Convert updated Entity back to DTO
            return updatedArticle!.ToDto();
        }

        public async Task<bool> DeleteNewsArticleAsync(string id)
        {
            var article = await _unitOfWork.NewsArticles.GetByIdAsync(id);
            if (article == null) return false;

            _unitOfWork.NewsArticles.Delete(article);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<NewsArticleReportDto>> GetNewsArticleReportAsync(DateTime startDate, DateTime endDate)
        {
            var articles = await _unitOfWork.NewsArticles
                .GetQueryable()
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Where(n => n.CreatedDate >= startDate && n.CreatedDate <= endDate)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();

            // Convert Entity to ReportDto using mapper
            return articles.ToReportDtoList();
        }
    }
}
