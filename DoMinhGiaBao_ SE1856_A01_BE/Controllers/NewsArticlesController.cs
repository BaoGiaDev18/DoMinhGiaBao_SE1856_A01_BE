using DoMinhGiaBao_SE1856_A01_Service.Services;
using DoMinhGiaBao__SE1856_A01_BE.Mappers;
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
        /// RESTful: GET /api/news-articles v?i các query parameters:
        /// - ?status=active : L?y bài vi?t active
        /// - ?createdBy={id} : L?y bài vi?t theo ng??i t?o
        /// - ?startDate=...&endDate=... : L?y report theo kho?ng th?i gian
        /// Có th? k?t h?p: ?status=active&createdBy=1
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<NewsArticleListResponse>>>> GetAll(
            [FromQuery] string? status = null,
            [FromQuery] short? createdBy = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            // N?u có startDate và endDate, tr? v? report
            if (startDate.HasValue && endDate.HasValue)
            {
                var reportDtos = await _newsArticleService.GetNewsArticleReportAsync(startDate.Value, endDate.Value);
                var reportResponses = reportDtos.Select(dto => dto.ToReportResponse()).ToList();
                return Ok(ApiResponse<IEnumerable<NewsArticleReportResponse>>.SuccessResponse(
                    reportResponses, 
                    "Report generated successfully"
                ));
            }

            // L?y theo createdBy
            if (createdBy.HasValue)
            {
                var dtos = await _newsArticleService.GetNewsArticlesByCreatorAsync(createdBy.Value);
                var responses = dtos.Select(dto => dto.ToListResponse()).ToList();
                return Ok(ApiResponse<IEnumerable<NewsArticleListResponse>>.SuccessResponse(
                    responses,
                    $"Found {responses.Count} articles by creator {createdBy}"
                ));
            }

            // L?y theo status
            if (status?.ToLower() == "active")
            {
                var dtos = await _newsArticleService.GetActiveNewsArticlesAsync();
                var responses = dtos.Select(dto => dto.ToListResponse()).ToList();
                return Ok(ApiResponse<IEnumerable<NewsArticleListResponse>>.SuccessResponse(
                    responses,
                    $"Found {responses.Count} active articles"
                ));
            }

            // L?y t?t c?
            var allDtos = await _newsArticleService.GetAllNewsArticlesAsync();
            var allResponses = allDtos.Select(dto => dto.ToListResponse()).ToList();
            return Ok(ApiResponse<IEnumerable<NewsArticleListResponse>>.SuccessResponse(
                allResponses,
                $"Retrieved {allResponses.Count} articles"
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
