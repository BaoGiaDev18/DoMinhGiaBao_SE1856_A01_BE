using DoMinhGiaBao_SE1856_A01_Service.DTOs;
using DoMinhGiaBao_SE1856_A01_Service.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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
        public async Task<ActionResult<IEnumerable<NewsArticleDto>>> GetAll(
            [FromQuery] string? status = null,
            [FromQuery] short? createdBy = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            // N?u có startDate và endDate, tr? v? report
            if (startDate.HasValue && endDate.HasValue)
            {
                var report = await _newsArticleService.GetNewsArticleReportAsync(startDate.Value, endDate.Value);
                return Ok(report);
            }

            // L?y theo createdBy
            if (createdBy.HasValue)
            {
                var articlesByCreator = await _newsArticleService.GetNewsArticlesByCreatorAsync(createdBy.Value);
                return Ok(articlesByCreator);
            }

            // L?y theo status
            if (status?.ToLower() == "active")
            {
                var activeArticles = await _newsArticleService.GetActiveNewsArticlesAsync();
                return Ok(activeArticles);
            }

            // L?y t?t c?
            var articles = await _newsArticleService.GetAllNewsArticlesAsync();
            return Ok(articles);
        }

        /// <summary>
        /// RESTful: GET /api/news-articles/{id}
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<NewsArticleDto>> GetById(string id)
        {
            var article = await _newsArticleService.GetNewsArticleByIdAsync(id);
            if (article == null)
            {
                return NotFound(new { message = "News article not found" });
            }
            return Ok(article);
        }

        [HttpPost]
        public async Task<ActionResult<NewsArticleDto>> Create([FromBody] CreateNewsArticleDto createDto, [FromQuery] short createdById)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var article = await _newsArticleService.CreateNewsArticleAsync(createDto, createdById);
                return CreatedAtAction(nameof(GetById), new { id = article.NewsArticleId }, article);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<NewsArticleDto>> Update(string id, [FromBody] UpdateNewsArticleDto updateDto, [FromQuery] short updatedById)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var article = await _newsArticleService.UpdateNewsArticleAsync(id, updateDto, updatedById);
            if (article == null)
            {
                return NotFound(new { message = "News article not found" });
            }
            return Ok(article);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var result = await _newsArticleService.DeleteNewsArticleAsync(id);
            if (!result)
            {
                return NotFound(new { message = "News article not found" });
            }
            return Ok(new { message = "News article deleted successfully" });
        }
    }
}
