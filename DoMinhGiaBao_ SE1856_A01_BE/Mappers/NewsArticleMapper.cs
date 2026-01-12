using DoMinhGiaBao_SE1856_A01_Service.DTOs;
using DoMinhGiaBao__SE1856_A01_BE.Models.Requests;
using DoMinhGiaBao__SE1856_A01_BE.Models.Responses;

namespace DoMinhGiaBao__SE1856_A01_BE.Mappers
{
    /// <summary>
    /// Mapper for converting between API Models and Service DTOs for News Articles
    /// Implements the Adapter Pattern to decouple API layer from Service layer
    /// </summary>
    public static class NewsArticleMapper
    {
        #region Request to DTO Mappings

        /// <summary>
        /// Maps CreateNewsArticleRequest (API) to CreateNewsArticleDto (Service)
        /// </summary>
        public static CreateNewsArticleDto ToCreateDto(this CreateNewsArticleRequest request)
        {
            return new CreateNewsArticleDto
            {
                NewsArticleId = request.NewsArticleId,
                NewsTitle = request.NewsTitle,
                Headline = request.Headline,
                NewsContent = request.NewsContent,
                NewsSource = request.NewsSource,
                CategoryId = request.CategoryId,
                NewsStatus = request.NewsStatus,
                TagIds = request.TagIds
            };
        }

        /// <summary>
        /// Maps UpdateNewsArticleRequest (API) to UpdateNewsArticleDto (Service)
        /// </summary>
        public static UpdateNewsArticleDto ToUpdateDto(this UpdateNewsArticleRequest request)
        {
            return new UpdateNewsArticleDto
            {
                NewsTitle = request.NewsTitle,
                Headline = request.Headline,
                NewsContent = request.NewsContent,
                NewsSource = request.NewsSource,
                CategoryId = request.CategoryId,
                NewsStatus = request.NewsStatus,
                TagIds = request.TagIds
            };
        }

        #endregion

        #region DTO to Response Mappings

        /// <summary>
        /// Maps NewsArticleDto (Service) to NewsArticleResponse (API)
        /// </summary>
        public static NewsArticleResponse ToResponse(this NewsArticleDto dto)
        {
            return new NewsArticleResponse
            {
                NewsArticleId = dto.NewsArticleId,
                NewsTitle = dto.NewsTitle ?? string.Empty,
                Headline = dto.Headline,
                CreatedDate = dto.CreatedDate,
                NewsContent = dto.NewsContent,
                NewsSource = dto.NewsSource,
                CategoryId = dto.CategoryId ?? 0,
                CategoryName = dto.CategoryName,
                NewsStatus = dto.NewsStatus ?? false,
                CreatedById = dto.CreatedById ?? 0,
                CreatedByName = dto.CreatedByName,
                UpdatedById = dto.UpdatedById,
                ModifiedDate = dto.ModifiedDate,
                Tags = dto.Tags?.Select(t => t.ToResponse()).ToList() ?? new List<TagResponse>()
            };
        }

        /// <summary>
        /// Maps NewsArticleDto (Service) to NewsArticleListResponse (API)
        /// </summary>
        public static NewsArticleListResponse ToListResponse(this NewsArticleDto dto)
        {
            return new NewsArticleListResponse
            {
                NewsArticleId = dto.NewsArticleId,
                NewsTitle = dto.NewsTitle ?? string.Empty,
                Headline = dto.Headline,
                CreatedDate = dto.CreatedDate,
                CategoryName = dto.CategoryName,
                CreatedByName = dto.CreatedByName,
                NewsStatus = dto.NewsStatus ?? false,
                TagCount = dto.Tags?.Count ?? 0
            };
        }

        /// <summary>
        /// Maps NewsArticleReportDto (Service) to NewsArticleReportResponse (API)
        /// </summary>
        public static NewsArticleReportResponse ToReportResponse(this NewsArticleReportDto dto)
        {
            return new NewsArticleReportResponse
            {
                NewsArticleId = dto.NewsArticleId,
                NewsTitle = dto.NewsTitle ?? string.Empty,
                Headline = dto.Headline,
                CreatedDate = dto.CreatedDate,
                CategoryName = dto.CategoryName,
                CreatedByName = dto.CreatedByName,
                NewsStatus = dto.NewsStatus ?? false
            };
        }

        #endregion
    }
}
