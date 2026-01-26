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
    public class NewsArticleService : INewsArticleService
    {
        private readonly IUnitOfWork _unitOfWork;

        public NewsArticleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<NewsArticleDto>> GetAllNewsArticlesAsync()
        {
            var articles = await _unitOfWork.NewsArticles
                .GetQueryable()
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Include(n => n.Tags)
                .ToListAsync();

            return articles.Select(MapToDto);
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

            return articles.Select(MapToDto);
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

            return articles.Select(MapToDto);
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

            return MapToDto(article);
        }

        public async Task<NewsArticleDto> CreateNewsArticleAsync(CreateNewsArticleDto createDto, short createdById)
        {
            // Check if article ID already exists
            var exists = await _unitOfWork.NewsArticles
                .ExistsAsync(n => n.NewsArticleId == createDto.NewsArticleId);

            if (exists)
            {
                throw new InvalidOperationException("News Article ID already exists");
            }

            // Validate CategoryId
            var categoryExists = await _unitOfWork.Categories
                .ExistsAsync(c => c.CategoryId == createDto.CategoryId);
            
            if (!categoryExists)
            {
                throw new InvalidOperationException($"Category with ID {createDto.CategoryId} does not exist");
            }

            // Validate createdById
            var accountExists = await _unitOfWork.SystemAccounts
                .ExistsAsync(a => a.AccountId == createdById);
            
            if (!accountExists)
            {
                throw new InvalidOperationException($"System account with ID {createdById} does not exist");
            }

            var article = new NewsArticle
            {
                NewsArticleId = createDto.NewsArticleId,
                NewsTitle = createDto.NewsTitle,
                Headline = createDto.Headline,
                NewsContent = createDto.NewsContent,
                NewsSource = createDto.NewsSource,
                CategoryId = createDto.CategoryId,
                NewsStatus = createDto.NewsStatus,
                CreatedById = createdById,
                CreatedDate = DateTime.Now
            };

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

            return MapToDto(createdArticle!);
        }

        public async Task<NewsArticleDto?> UpdateNewsArticleAsync(string id, UpdateNewsArticleDto updateDto, short updatedById)
        {
            var article = await _unitOfWork.NewsArticles
                .GetQueryable()
                .Include(n => n.Tags)
                .FirstOrDefaultAsync(n => n.NewsArticleId == id);

            if (article == null) return null;

            // Validate CategoryId
            var categoryExists = await _unitOfWork.Categories
                .ExistsAsync(c => c.CategoryId == updateDto.CategoryId);
            
            if (!categoryExists)
            {
                throw new InvalidOperationException($"Category with ID {updateDto.CategoryId} does not exist");
            }

            // Validate updatedById
            var accountExists = await _unitOfWork.SystemAccounts
                .ExistsAsync(a => a.AccountId == updatedById);
            
            if (!accountExists)
            {
                throw new InvalidOperationException($"System account with ID {updatedById} does not exist");
            }

            article.NewsTitle = updateDto.NewsTitle;
            article.Headline = updateDto.Headline;
            article.NewsContent = updateDto.NewsContent;
            article.NewsSource = updateDto.NewsSource;
            article.CategoryId = updateDto.CategoryId;
            article.NewsStatus = updateDto.NewsStatus;
            article.UpdatedById = updatedById;
            article.ModifiedDate = DateTime.Now;

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

            return MapToDto(updatedArticle!);
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

            return articles.Select(a => new NewsArticleReportDto
            {
                NewsArticleId = a.NewsArticleId,
                NewsTitle = a.NewsTitle,
                Headline = a.Headline,
                CreatedDate = a.CreatedDate,
                CategoryName = a.Category?.CategoryName,
                CreatedByName = a.CreatedBy?.AccountName,
                NewsStatus = a.NewsStatus
            });
        }

        private NewsArticleDto MapToDto(NewsArticle article)
        {
            return new NewsArticleDto
            {
                NewsArticleId = article.NewsArticleId,
                NewsTitle = article.NewsTitle,
                Headline = article.Headline,
                CreatedDate = article.CreatedDate,
                NewsContent = article.NewsContent,
                NewsSource = article.NewsSource,
                CategoryId = article.CategoryId,
                NewsStatus = article.NewsStatus,
                CreatedById = article.CreatedById,
                UpdatedById = article.UpdatedById,
                ModifiedDate = article.ModifiedDate,
                CategoryName = article.Category?.CategoryName,
                CreatedByName = article.CreatedBy?.AccountName,
                TagIds = article.Tags.Select(t => t.TagId).ToList(),
                Tags = article.Tags.Select(t => new TagDto
                {
                    TagId = t.TagId,
                    TagName = t.TagName,
                    Note = t.Note
                }).ToList()
            };
        }
    }
}
