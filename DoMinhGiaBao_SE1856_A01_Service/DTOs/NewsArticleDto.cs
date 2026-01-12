using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DoMinhGiaBao_SE1856_A01_Service.DTOs
{
    public class NewsArticleDto
    {
        public string NewsArticleId { get; set; } = null!;

        [Required(ErrorMessage = "News title is required")]
        [MaxLength(400, ErrorMessage = "News title cannot exceed 400 characters")]
        public string? NewsTitle { get; set; }

        [Required(ErrorMessage = "Headline is required")]
        [MaxLength(150, ErrorMessage = "Headline cannot exceed 150 characters")]
        public string Headline { get; set; } = null!;

        public DateTime? CreatedDate { get; set; }

        [MaxLength(4000, ErrorMessage = "News content cannot exceed 4000 characters")]
        public string? NewsContent { get; set; }

        [MaxLength(400, ErrorMessage = "News source cannot exceed 400 characters")]
        public string? NewsSource { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public short? CategoryId { get; set; }

        public bool? NewsStatus { get; set; }

        public short? CreatedById { get; set; }

        public short? UpdatedById { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string? CategoryName { get; set; }
        public string? CreatedByName { get; set; }
        public List<int> TagIds { get; set; } = new List<int>();
        public List<TagDto> Tags { get; set; } = new List<TagDto>();
    }

    public class CreateNewsArticleDto
    {
        [Required(ErrorMessage = "News article ID is required")]
        [MaxLength(20, ErrorMessage = "News article ID cannot exceed 20 characters")]
        public string NewsArticleId { get; set; } = null!;

        [Required(ErrorMessage = "News title is required")]
        [MaxLength(400, ErrorMessage = "News title cannot exceed 400 characters")]
        public string NewsTitle { get; set; } = null!;

        [Required(ErrorMessage = "Headline is required")]
        [MaxLength(150, ErrorMessage = "Headline cannot exceed 150 characters")]
        public string Headline { get; set; } = null!;

        [MaxLength(4000, ErrorMessage = "News content cannot exceed 4000 characters")]
        public string? NewsContent { get; set; }

        [MaxLength(400, ErrorMessage = "News source cannot exceed 400 characters")]
        public string? NewsSource { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public short CategoryId { get; set; }

        public bool NewsStatus { get; set; } = true;

        public List<int> TagIds { get; set; } = new List<int>();
    }

    public class UpdateNewsArticleDto
    {
        [Required(ErrorMessage = "News title is required")]
        [MaxLength(400, ErrorMessage = "News title cannot exceed 400 characters")]
        public string NewsTitle { get; set; } = null!;

        [Required(ErrorMessage = "Headline is required")]
        [MaxLength(150, ErrorMessage = "Headline cannot exceed 150 characters")]
        public string Headline { get; set; } = null!;

        [MaxLength(4000, ErrorMessage = "News content cannot exceed 4000 characters")]
        public string? NewsContent { get; set; }

        [MaxLength(400, ErrorMessage = "News source cannot exceed 400 characters")]
        public string? NewsSource { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public short CategoryId { get; set; }

        public bool NewsStatus { get; set; }

        public List<int> TagIds { get; set; } = new List<int>();
    }

    public class NewsArticleReportDto
    {
        public string NewsArticleId { get; set; } = null!;
        public string? NewsTitle { get; set; }
        public string Headline { get; set; } = null!;
        public DateTime? CreatedDate { get; set; }
        public string? CategoryName { get; set; }
        public string? CreatedByName { get; set; }
        public bool? NewsStatus { get; set; }
    }
}
