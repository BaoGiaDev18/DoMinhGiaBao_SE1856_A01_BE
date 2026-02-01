using PRN232.LAB.Repo;
using PRN232.LAB.Repo.Repositories;
using PRN232.LAB.Services.Services;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.ModelBuilder;
using PRN232.LAB.Repo.Entities;
using AppConfigManager = PRN232.LAB.Repo.Configuration.ConfigurationManager;
using DoMinhGiaBao__SE1856_A01_BE.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using DoMinhGiaBao__SE1856_A01_BE.Middleware;

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

// ==================== JWT TOKEN SERVICE ====================
// Register JWT Token Service as Singleton with configuration from appsettings.json
var jwtSecretKey = builder.Configuration["JWT:SecretKey"] 
    ?? throw new InvalidOperationException("JWT:SecretKey not found in configuration");
var jwtIssuer = builder.Configuration["JWT:Issuer"] ?? "FUNewsManagementAPI";
var jwtAudience = builder.Configuration["JWT:Audience"] ?? "FUNewsManagementClient";
var jwtExpirationMinutes = int.Parse(builder.Configuration["JWT:ExpirationMinutes"] ?? "60");

builder.Services.AddSingleton<IJwtTokenService>(sp => 
    new JwtTokenService(jwtSecretKey, jwtIssuer, jwtAudience, jwtExpirationMinutes));
// ========================================================

// Add Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ISystemAccountService, SystemAccountService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<INewsArticleService, NewsArticleService>();
builder.Services.AddScoped<ITagService, TagService>();

// ==================== JWT AUTHENTICATION ====================
// Configure JWT Bearer Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// ==================== CUSTOM AUTHORIZATION HANDLER ====================
// Register custom authorization middleware result handler for consistent 401/403 responses
builder.Services.AddSingleton<Microsoft.AspNetCore.Authorization.IAuthorizationMiddlewareResultHandler, 
    CustomAuthorizationMiddlewareResultHandler>();
// ======================================================================

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
builder.Services.AddSwaggerGen(options =>
{
    // Add JWT Authentication to Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\""
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// ==================== GLOBAL EXCEPTION HANDLER ====================
// MUST be first middleware to catch all unhandled exceptions (500 Internal Server Error)
app.UseGlobalExceptionHandler();
// ==================================================================

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

// ==================== JWT AUTHENTICATION & AUTHORIZATION ====================
// IMPORTANT: UseAuthentication() MUST come before UseAuthorization()
app.UseAuthentication();  // ? Verify JWT token from Authorization header
app.UseAuthorization();   // ? Check user permissions
// ===========================================================================

app.MapControllers();

app.Run();
