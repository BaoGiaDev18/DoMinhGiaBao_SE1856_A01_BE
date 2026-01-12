using DoMinhGiaBao_SE1856_A01_Repository;
using DoMinhGiaBao_SE1856_A01_Repository.Repositories;
using DoMinhGiaBao_SE1856_A01_Service.Services;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.ModelBuilder;
using DoMinhGiaBao_SE1856_A01_Repository.Entities;
using AppConfigManager = DoMinhGiaBao_SE1856_A01_Repository.Configuration.ConfigurationManager;
using DoMinhGiaBao__SE1856_A01_BE.Data;

var builder = WebApplication.CreateBuilder(args);

// ==================== SINGLETON PATTERN: Initialize Configuration Manager ====================
// Kh?i t?o Singleton instance ?? qu?n lý configuration t?p trung
// ?ây là implementation rõ ràng c?a Singleton Pattern theo yêu c?u ?? bài
// S? d?ng alias 'AppConfigManager' ?? tránh conflict v?i Microsoft.Extensions.Configuration.ConfigurationManager
AppConfigManager.Initialize(builder.Configuration);

// Add Database Context - S? d?ng ConfigurationManager Singleton
builder.Services.AddDbContext<FUNewsManagementContext>(options =>
    options.UseSqlServer(AppConfigManager.Instance.ConnectionString));

// Add Repository and Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Add Database Seeder
builder.Services.AddScoped<DatabaseSeeder>();

// Add Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ISystemAccountService, SystemAccountService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<INewsArticleService, NewsArticleService>();
builder.Services.AddScoped<ITagService, TagService>();

// Configure OData
var modelBuilder = new ODataConventionModelBuilder();
modelBuilder.EntitySet<SystemAccount>("SystemAccounts");
modelBuilder.EntitySet<NewsArticle>("NewsArticles");
modelBuilder.EntitySet<Category>("Categories");
modelBuilder.EntitySet<Tag>("Tags");

// Add services to the container.
builder.Services.AddControllers()
    .AddOData(options => options
        .Select()
        .Filter()
        .OrderBy()
        .Expand()
        .Count()
        .SetMaxTop(100)
        .AddRouteComponents("odata", modelBuilder.GetEdmModel()));

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ==================== AUTO DATABASE MIGRATION & SEEDING ====================
// Automatically apply Entity Framework migrations and seed data on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    
    try
    {
        var context = services.GetRequiredService<FUNewsManagementContext>();
        
        // Apply pending migrations
        context.Database.Migrate();
        logger.LogInformation("? Database migration completed successfully!");
        
        // Seed data from JSON file
        var seeder = services.GetRequiredService<DatabaseSeeder>();
        await seeder.SeedAsync();
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "? Error during migration/seeding");
    }
}
// ==================================================================

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
