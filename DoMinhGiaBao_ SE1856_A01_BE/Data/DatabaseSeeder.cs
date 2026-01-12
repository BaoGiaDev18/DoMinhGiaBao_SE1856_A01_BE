using DoMinhGiaBao_SE1856_A01_Repository;
using DoMinhGiaBao_SE1856_A01_Repository.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace DoMinhGiaBao__SE1856_A01_BE.Data;

public class DatabaseSeeder
{
    private readonly FUNewsManagementContext _context;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(FUNewsManagementContext context, ILogger<DatabaseSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        try
        {
            // Check if database already has data
            if (await _context.SystemAccounts.AnyAsync())
            {
                _logger.LogInformation("??  Database already contains data, skipping seeding.");
                return;
            }

            _logger.LogInformation("?? Starting database seeding from JSON file...");

            // Load JSON file
            var jsonPath = Path.Combine(AppContext.BaseDirectory, "SeedData", "initial-data.json");
            
            if (!File.Exists(jsonPath))
            {
                _logger.LogWarning("??  Seed data file not found: {Path}", jsonPath);
                _logger.LogInformation("Creating minimal seed data instead...");
                await SeedMinimalDataAsync();
                return;
            }

            var jsonContent = await File.ReadAllTextAsync(jsonPath);
            var seedData = JsonSerializer.Deserialize<SeedDataModel>(jsonContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (seedData == null)
            {
                _logger.LogWarning("??  Failed to deserialize seed data");
                return;
            }

            // Seed data in order (respect foreign key constraints)
            await SeedSystemAccountsAsync(seedData.SystemAccounts);
            await SeedCategoriesAsync(seedData.Categories);
            await SeedTagsAsync(seedData.Tags);
            await SeedNewsArticlesAsync(seedData.NewsArticles);

            _logger.LogInformation("? Database seeding completed successfully!");
            _logger.LogInformation($"   ?? Statistics:");
            _logger.LogInformation($"      - System Accounts: {seedData.SystemAccounts?.Count ?? 0}");
            _logger.LogInformation($"      - Categories: {seedData.Categories?.Count ?? 0}");
            _logger.LogInformation($"      - Tags: {seedData.Tags?.Count ?? 0}");
            _logger.LogInformation($"      - News Articles: {seedData.NewsArticles?.Count ?? 0}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "? Error during database seeding");
            throw;
        }
    }

    private async Task SeedSystemAccountsAsync(List<SystemAccountSeedModel>? accounts)
    {
        if (accounts == null || !accounts.Any()) return;

        // Use raw SQL with IDENTITY_INSERT in a single transaction
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT SystemAccount ON");

            foreach (var account in accounts)
            {
                await _context.Database.ExecuteSqlRawAsync(
                    @"INSERT INTO SystemAccount (AccountID, AccountName, AccountEmail, AccountRole, AccountPassword) 
                      VALUES ({0}, {1}, {2}, {3}, {4})",
                    account.AccountId, account.AccountName, account.AccountEmail, account.AccountRole, account.AccountPassword
                );
            }

            await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT SystemAccount OFF");
            await transaction.CommitAsync();

            _logger.LogInformation("? Seeded {Count} system accounts", accounts.Count);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private async Task SeedCategoriesAsync(List<CategorySeedModel>? categories)
    {
        if (categories == null || !categories.Any()) return;

        // Use raw SQL with IDENTITY_INSERT in a single transaction
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Category ON");

            foreach (var category in categories)
            {
                await _context.Database.ExecuteSqlRawAsync(
                    @"INSERT INTO Category (CategoryID, CategoryName, CategoryDesciption, ParentCategoryID) 
                      VALUES ({0}, {1}, {2}, {3})",
                    category.CategoryId, category.CategoryName, category.CategoryDesciption, 
                    category.ParentCategoryId.HasValue ? (object)category.ParentCategoryId.Value : DBNull.Value
                );
            }

            await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Category OFF");
            await transaction.CommitAsync();

            _logger.LogInformation("? Seeded {Count} categories", categories.Count);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private async Task SeedTagsAsync(List<TagSeedModel>? tags)
    {
        if (tags == null || !tags.Any()) return;

        // Tags need IDENTITY_INSERT too based on your seed data (specific IDs)
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Tag ON");

            foreach (var tag in tags)
            {
                await _context.Database.ExecuteSqlRawAsync(
                    @"IF NOT EXISTS (SELECT 1 FROM Tag WHERE TagID = {0})
                      INSERT INTO Tag (TagID, TagName, Note) VALUES ({0}, {1}, {2})",
                    tag.TagId, tag.TagName, tag.Note
                );
            }

            await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Tag OFF");
            await transaction.CommitAsync();
            
            _logger.LogInformation("? Seeded {Count} tags", tags.Count);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private async Task SeedNewsArticlesAsync(List<NewsArticleSeedModel>? articles)
    {
        if (articles == null || !articles.Any()) return;

        var tags = await _context.Tags.ToListAsync();

        foreach (var article in articles)
        {
            var entity = new NewsArticle
            {
                NewsArticleId = article.NewsArticleId,
                NewsTitle = article.NewsTitle,
                Headline = article.Headline,
                CreatedDate = article.CreatedDate,
                NewsContent = article.NewsContent,
                NewsSource = article.NewsSource,
                CategoryId = (short)article.CategoryId,
                NewsStatus = article.NewsStatus,
                CreatedById = (short)article.CreatedById,
                UpdatedById = article.UpdatedById.HasValue ? (short?)article.UpdatedById.Value : null,
                ModifiedDate = article.ModifiedDate
            };

            // Add associated tags
            if (article.TagIds != null && article.TagIds.Any())
            {
                foreach (var tagId in article.TagIds)
                {
                    var tag = tags.FirstOrDefault(t => t.TagId == tagId);
                    if (tag != null)
                    {
                        entity.Tags.Add(tag);
                    }
                }
            }

            _context.NewsArticles.Add(entity);
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation("? Seeded {Count} news articles with tags", articles.Count);
    }

    private async Task SeedMinimalDataAsync()
    {
        // Fallback minimal data
        var account = new SystemAccount
        {
            AccountName = "Administrator",
            AccountEmail = "admin@FUNewsManagement.org",
            AccountRole = 0,
            AccountPassword = "@@abc123@@"
        };
        _context.SystemAccounts.Add(account);
        await _context.SaveChangesAsync();

        var category = new Category
        {
            CategoryName = "General",
            CategoryDesciption = "General news and updates"
        };
        _context.Categories.Add(category);

        var tag = new Tag
        {
            TagName = "News",
            Note = "General news tag"
        };
        _context.Tags.Add(tag);

        await _context.SaveChangesAsync();
        _logger.LogInformation("? Minimal seed data created");
    }
}

// ==================== SEED DATA MODELS ====================

public class SeedDataModel
{
    public List<SystemAccountSeedModel> SystemAccounts { get; set; } = new();
    public List<CategorySeedModel> Categories { get; set; } = new();
    public List<TagSeedModel> Tags { get; set; } = new();
    public List<NewsArticleSeedModel> NewsArticles { get; set; } = new();
}

public class SystemAccountSeedModel
{
    public int AccountId { get; set; }
    public string AccountName { get; set; } = string.Empty;
    public string AccountEmail { get; set; } = string.Empty;
    public int AccountRole { get; set; }
    public string AccountPassword { get; set; } = string.Empty;
}

public class CategorySeedModel
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string CategoryDesciption { get; set; } = string.Empty;
    public int? ParentCategoryId { get; set; }
    public bool IsActive { get; set; }
}

public class TagSeedModel
{
    public int TagId { get; set; }
    public string TagName { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
}

public class NewsArticleSeedModel
{
    public string NewsArticleId { get; set; } = string.Empty;
    public string NewsTitle { get; set; } = string.Empty;
    public string Headline { get; set; } = string.Empty;
    public DateTime? CreatedDate { get; set; }
    public string NewsContent { get; set; } = string.Empty;
    public string NewsSource { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public bool NewsStatus { get; set; }
    public int CreatedById { get; set; }
    public int? UpdatedById { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public List<int>? TagIds { get; set; }
}
