using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DoMinhGiaBao_SE1856_A01_Repository.Entities;

public partial class Tag
{
    [Key]
    public int TagId { get; set; }

    public string? TagName { get; set; }

    public string? Note { get; set; }

    public virtual ICollection<NewsArticle> NewsArticles { get; set; } = new List<NewsArticle>();
}
