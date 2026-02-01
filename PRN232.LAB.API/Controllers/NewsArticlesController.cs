using PRN232.LAB.Services.Services;
using DoMinhGiaBao__SE1856_A01_BE.Extensions;
using DoMinhGiaBao__SE1856_A01_BE.Mappers;
using DoMinhGiaBao__SE1856_A01_BE.Models.Common;
using DoMinhGiaBao__SE1856_A01_BE.Models.Requests;
using DoMinhGiaBao__SE1856_A01_BE.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoMinhGiaBao__SE1856_A01_BE.Controllers
{
    /// <summary>
    /// RESTful API cho News Articles
    /// Route: /api/news-articles (lowercase, kebab-case theo chu?n RESTful)
    /// </summary>
    [ApiController]
    [Route("api/news-articles")]
    public class NewsArticlesController : ControllerBase
    {
        private readonly INewsArticleService _newsArticleService;

        public NewsArticlesController(INewsArticleService newsArticleService)
        {
            _newsArticleService = newsArticleService;
        }

        /// <summary>
        /// RESTful: GET /api/news-articles with full support for:
        /// - Paging: ?page=1&pageSize=10
        /// - Sorting: ?sortBy=createdDate&sortOrder=desc
        /// - Search: ?search=technology
        /// - Filtering: ?status=active&createdById=1&categoryId=2&startDate=...&endDate=...&tagId=3
        /// - Field Selection: ?fields=newsArticleId,newsTitle,categoryName
        /// Example: GET /api/news-articles?page=1&pageSize=10&sortBy=createdDate&sortOrder=desc&status=active&categoryId=1
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<PaginatedApiResponse<NewsArticleListResponse>>> GetAll(
            [FromQuery] NewsArticleQueryParameters queryParams)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                    .ToList();
                return BadRequest(PaginatedApiResponse<NewsArticleListResponse>.ErrorResponse(
                    "Invalid query parameters",
                    errors
                ));
            }

            // Call service with all parameters
            var pagedResult = await _newsArticleService.GetNewsArticlesAsync(
                queryParams.Page,
                queryParams.PageSize,
                queryParams.SortBy,
                queryParams.SortOrder,
                queryParams.Search,
                queryParams.Status,
                queryParams.CreatedById,
                queryParams.CategoryId,
                queryParams.StartDate,
                queryParams.EndDate,
                queryParams.TagId
            );

            // Convert DTOs to Response Models
            var responses = pagedResult.Items.Select(dto => dto.ToListResponse());

            // Apply field selection if requested
            var finalData = string.IsNullOrWhiteSpace(queryParams.Fields)
                ? responses.Cast<object>()
                : responses.SelectFields(queryParams.Fields);

            // Create pagination metadata
            var pagination = new PaginationMetadata(
                pagedResult.Page,
                pagedResult.PageSize,
                pagedResult.TotalCount
            );

            return Ok(PaginatedApiResponse<object>.SuccessResponse(
                finalData,
                pagination,
                $"Retrieved {pagedResult.Items.Count()} of {pagedResult.TotalCount} articles"
            ));
        }

        /// <summary>
        /// RESTful: GET /api/news-articles/{id}
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<NewsArticleResponse>>> GetById(string id)
        {
            var dto = await _newsArticleService.GetNewsArticleByIdAsync(id);
            if (dto == null)
            {
                return NotFound(ApiResponse<NewsArticleResponse>.ErrorResponse(
                    "News article not found",
                    $"No article found with ID: {id}"
                ));
            }
            
            var response = dto.ToResponse();
            return Ok(ApiResponse<NewsArticleResponse>.SuccessResponse(
                response,
                "Article retrieved successfully"
            ));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<NewsArticleResponse>>> Create([FromBody] CreateNewsArticleRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                    .ToList();
                return BadRequest(ApiResponse<NewsArticleResponse>.ErrorResponse(
                    "Validation failed",
                    errors
                ));
            }

            try
            {
                var createDto = request.ToCreateDto();
                var dto = await _newsArticleService.CreateNewsArticleAsync(createDto, request.CreatedById);
                var response = dto.ToResponse();
                
                return CreatedAtAction(
                    nameof(GetById), 
                    new { id = response.NewsArticleId }, 
                    ApiResponse<NewsArticleResponse>.SuccessResponse(
                        response,
                        "Article created successfully"
                    )
                );
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<NewsArticleResponse>.ErrorResponse(
                    "Failed to create article",
                    ex.Message
                ));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<NewsArticleResponse>>> Update(string id, [FromBody] UpdateNewsArticleRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                    .ToList();
                return BadRequest(ApiResponse<NewsArticleResponse>.ErrorResponse(
                    "Validation failed",
                    errors
                ));
            }

            try
            {
                var updateDto = request.ToUpdateDto();
                var dto = await _newsArticleService.UpdateNewsArticleAsync(id, updateDto, request.UpdatedById);
                
                if (dto == null)
                {
                    return NotFound(ApiResponse<NewsArticleResponse>.ErrorResponse(
                        "News article not found",
                        $"No article found with ID: {id}"
                    ));
                }
                
                var response = dto.ToResponse();
                return Ok(ApiResponse<NewsArticleResponse>.SuccessResponse(
                    response,
                    "Article updated successfully"
                ));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<NewsArticleResponse>.ErrorResponse(
                    "Failed to update article",
                    ex.Message
                ));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse>> Delete(string id)
        {
            var result = await _newsArticleService.DeleteNewsArticleAsync(id);
            
            if (!result)
            {
                return NotFound(ApiResponse.ErrorResponse(
                    "News article not found",
                    $"No article found with ID: {id}"
                ));
            }
            
            return Ok(ApiResponse.SuccessResponse(
                "Article deleted successfully"
            ));
        }
    }
}
