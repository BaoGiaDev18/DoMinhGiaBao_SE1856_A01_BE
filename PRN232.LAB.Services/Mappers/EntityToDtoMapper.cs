using PRN232.LAB.Repo.Entities;
using PRN232.LAB.Services.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace PRN232.LAB.Services.Mappers
{
    /// <summary>
    /// Centralized Mapper for Service Layer
    /// Converts between Repository Entities and Service DTOs (Business Models)
    /// This mapper should NEVER be exposed outside the Service Layer
    /// </summary>
    internal static class EntityToDtoMapper
    {
        #region Category Mappings

        /// <summary>
        /// Maps Category Entity to CategoryDto
        /// </summary>
        public static CategoryDto ToDto(this Category entity)
        {
            return new CategoryDto
            {
                CategoryId = entity.CategoryId,
                CategoryName = entity.CategoryName,
                CategoryDesciption = entity.CategoryDesciption,
                ParentCategoryId = entity.ParentCategoryId,
                ParentCategoryName = entity.ParentCategory?.CategoryName,
                IsActive = entity.IsActive,
                SubCategories = entity.InverseParentCategory?.Select(sc => new CategoryDto
                {
                    CategoryId = sc.CategoryId,
                    CategoryName = sc.CategoryName,
                    CategoryDesciption = sc.CategoryDesciption,
                    ParentCategoryId = sc.ParentCategoryId,
                    IsActive = sc.IsActive
                }).ToList()
            };
        }

        /// <summary>
        /// Maps collection of Category Entities to CategoryDto collection
        /// </summary>
        public static IEnumerable<CategoryDto> ToDtoList(this IEnumerable<Category> entities)
        {
            return entities.Select(e => e.ToDto());
        }

        /// <summary>
        /// Maps CreateCategoryDto to new Category Entity
        /// </summary>
        public static Category ToEntity(this CreateCategoryDto dto)
        {
            return new Category
            {
                CategoryName = dto.CategoryName,
                CategoryDesciption = dto.CategoryDesciption,
                ParentCategoryId = dto.ParentCategoryId,
                IsActive = dto.IsActive
            };
        }

        /// <summary>
        /// Updates existing Category Entity from UpdateCategoryDto
        /// </summary>
        public static void UpdateEntity(this Category entity, UpdateCategoryDto dto)
        {
            entity.CategoryName = dto.CategoryName;
            entity.CategoryDesciption = dto.CategoryDesciption;
            entity.ParentCategoryId = dto.ParentCategoryId;
            entity.IsActive = dto.IsActive;
        }

        #endregion

        #region Tag Mappings

        /// <summary>
        /// Maps Tag Entity to TagDto
        /// </summary>
        public static TagDto ToDto(this Tag entity)
        {
            return new TagDto
            {
                TagId = entity.TagId,
                TagName = entity.TagName,
                Note = entity.Note
            };
        }

        /// <summary>
        /// Maps collection of Tag Entities to TagDto collection
        /// </summary>
        public static IEnumerable<TagDto> ToTagDtoList(this IEnumerable<Tag> entities)
        {
            return entities.Select(e => e.ToDto());
        }

        /// <summary>
        /// Maps CreateTagDto to new Tag Entity
        /// </summary>
        public static Tag ToEntity(this CreateTagDto dto)
        {
            return new Tag
            {
                TagName = dto.TagName,
                Note = dto.Note
            };
        }

        /// <summary>
        /// Updates existing Tag Entity from UpdateTagDto
        /// </summary>
        public static void UpdateEntity(this Tag entity, UpdateTagDto dto)
        {
            entity.TagName = dto.TagName;
            entity.Note = dto.Note;
        }

        #endregion

        #region SystemAccount Mappings

        /// <summary>
        /// Maps SystemAccount Entity to SystemAccountDto
        /// </summary>
        public static SystemAccountDto ToDto(this SystemAccount entity)
        {
            return new SystemAccountDto
            {
                AccountId = entity.AccountId,
                AccountName = entity.AccountName,
                AccountEmail = entity.AccountEmail,
                AccountRole = entity.AccountRole,
                AccountPassword = entity.AccountPassword
            };
        }

        /// <summary>
        /// Maps collection of SystemAccount Entities to SystemAccountDto collection
        /// </summary>
        public static IEnumerable<SystemAccountDto> ToAccountDtoList(this IEnumerable<SystemAccount> entities)
        {
            return entities.Select(e => e.ToDto());
        }

        /// <summary>
        /// Maps CreateSystemAccountDto to new SystemAccount Entity
        /// </summary>
        public static SystemAccount ToEntity(this CreateSystemAccountDto dto)
        {
            return new SystemAccount
            {
                AccountName = dto.AccountName,
                AccountEmail = dto.AccountEmail,
                AccountRole = dto.AccountRole,
                AccountPassword = dto.AccountPassword
            };
        }

        /// <summary>
        /// Updates existing SystemAccount Entity from UpdateSystemAccountDto
        /// </summary>
        public static void UpdateEntity(this SystemAccount entity, UpdateSystemAccountDto dto)
        {
            entity.AccountName = dto.AccountName;
            entity.AccountEmail = dto.AccountEmail;
            entity.AccountRole = dto.AccountRole;
            
            if (!string.IsNullOrEmpty(dto.AccountPassword))
            {
                entity.AccountPassword = dto.AccountPassword;
            }
        }

        #endregion

        #region NewsArticle Mappings

        /// <summary>
        /// Maps NewsArticle Entity to NewsArticleDto
        /// </summary>
        public static NewsArticleDto ToDto(this NewsArticle entity)
        {
            return new NewsArticleDto
            {
                NewsArticleId = entity.NewsArticleId,
                NewsTitle = entity.NewsTitle,
                Headline = entity.Headline,
                CreatedDate = entity.CreatedDate,
                NewsContent = entity.NewsContent,
                NewsSource = entity.NewsSource,
                CategoryId = entity.CategoryId,
                NewsStatus = entity.NewsStatus,
                CreatedById = entity.CreatedById,
                UpdatedById = entity.UpdatedById,
                ModifiedDate = entity.ModifiedDate,
                CategoryName = entity.Category?.CategoryName,
                CreatedByName = entity.CreatedBy?.AccountName,
                TagIds = entity.Tags?.Select(t => t.TagId).ToList() ?? new List<int>(),
                Tags = entity.Tags?.Select(t => t.ToDto()).ToList() ?? new List<TagDto>()
            };
        }

        /// <summary>
        /// Maps collection of NewsArticle Entities to NewsArticleDto collection
        /// </summary>
        public static IEnumerable<NewsArticleDto> ToNewsArticleDtoList(this IEnumerable<NewsArticle> entities)
        {
            return entities.Select(e => e.ToDto());
        }

        /// <summary>
        /// Maps CreateNewsArticleDto to new NewsArticle Entity
        /// </summary>
        public static NewsArticle ToEntity(this CreateNewsArticleDto dto, short createdById)
        {
            return new NewsArticle
            {
                NewsArticleId = dto.NewsArticleId,
                NewsTitle = dto.NewsTitle,
                Headline = dto.Headline,
                NewsContent = dto.NewsContent,
                NewsSource = dto.NewsSource,
                CategoryId = dto.CategoryId,
                NewsStatus = dto.NewsStatus,
                CreatedById = createdById,
                CreatedDate = System.DateTime.Now
            };
        }

        /// <summary>
        /// Updates existing NewsArticle Entity from UpdateNewsArticleDto
        /// </summary>
        public static void UpdateEntity(this NewsArticle entity, UpdateNewsArticleDto dto, short updatedById)
        {
            entity.NewsTitle = dto.NewsTitle;
            entity.Headline = dto.Headline;
            entity.NewsContent = dto.NewsContent;
            entity.NewsSource = dto.NewsSource;
            entity.CategoryId = dto.CategoryId;
            entity.NewsStatus = dto.NewsStatus;
            entity.UpdatedById = updatedById;
            entity.ModifiedDate = System.DateTime.Now;
        }

        /// <summary>
        /// Maps NewsArticle Entity to NewsArticleReportDto
        /// </summary>
        public static NewsArticleReportDto ToReportDto(this NewsArticle entity)
        {
            return new NewsArticleReportDto
            {
                NewsArticleId = entity.NewsArticleId,
                NewsTitle = entity.NewsTitle,
                Headline = entity.Headline,
                CreatedDate = entity.CreatedDate,
                CategoryName = entity.Category?.CategoryName,
                CreatedByName = entity.CreatedBy?.AccountName,
                NewsStatus = entity.NewsStatus
            };
        }

        /// <summary>
        /// Maps collection of NewsArticle Entities to NewsArticleReportDto collection
        /// </summary>
        public static IEnumerable<NewsArticleReportDto> ToReportDtoList(this IEnumerable<NewsArticle> entities)
        {
            return entities.Select(e => e.ToReportDto());
        }

        #endregion
    }
}
