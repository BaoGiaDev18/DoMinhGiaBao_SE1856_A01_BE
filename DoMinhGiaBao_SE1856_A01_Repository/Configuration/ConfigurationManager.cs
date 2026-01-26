using Microsoft.Extensions.Configuration;

namespace DoMinhGiaBao_SE1856_A01_Repository.Configuration
{
    /// <summary>
    /// Singleton Pattern: Quản lý cấu hình (Admin Account, Connection String)
    /// Chỉ có 1 instance duy nhất trong toàn bộ ứng dụng
    /// Thread-safe implementation using Double-Check Locking pattern
    /// </summary>
    public sealed class ConfigurationManager
    {
        private static ConfigurationManager? _instance;
        private static readonly object _lock = new object();
        private readonly IConfiguration _configuration;


        private ConfigurationManager(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public static ConfigurationManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    throw new InvalidOperationException(
                        "ConfigurationManager has not been initialized. Call Initialize() first in Program.cs.");
                }
                return _instance;
            }
        }


        /// <param name="configuration">IConfiguration t? ASP.NET Core DI</param>
        public static void Initialize(IConfiguration configuration)
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new ConfigurationManager(configuration);
                    }
                }
            }
        }

        // ==================== ADMIN ACCOUNT CONFIGURATION ====================
        
        /// <summary>
        /// Admin email t? appsettings.json
        /// Default: admin@FUNewsManagementSystem.org
        /// </summary>
        public string AdminEmail => _configuration["AdminAccount:Email"] 
            ?? "admin@FUNewsManagementSystem.org";

        /// <summary>
        /// Admin password t? appsettings.json
        /// Default: @@abc123@@
        /// </summary>
        public string AdminPassword => _configuration["AdminAccount:Password"] 
            ?? "@@abc123@@";

        /// <summary>
        /// Admin role t? appsettings.json
        /// Default: 0 (Admin role)
        /// </summary>
        public int AdminRole => int.Parse(_configuration["AdminAccount:Role"] ?? "0");

        // ==================== DATABASE CONFIGURATION ====================
        
        /// <summary>
        /// Connection string t? appsettings.json
        /// Throws exception n?u không tìm th?y
        /// </summary>
        public string ConnectionString => _configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found in appsettings.json");

        // ==================== HELPER METHODS ====================

        /// <summary>
        /// Ki?m tra xem email và password có ph?i c?a Admin không
        /// S? d?ng trong AuthService ?? validate admin login
        /// </summary>
        /// <param name="email">Email ?? ki?m tra</param>
        /// <param name="password">Password ?? ki?m tra</param>
        /// <returns>True n?u là admin account</returns>
        public bool IsAdminAccount(string email, string password)
        {
            return email == AdminEmail && password == AdminPassword;
        }

        /// <summary>
        /// L?y giá tr? c?u hình theo key
        /// </summary>
        /// <param name="key">Configuration key</param>
        /// <returns>Configuration value ho?c null</returns>
        public string? GetConfigValue(string key)
        {
            return _configuration[key];
        }


        /// <param name="key">Configuration key</param>
        /// <param name="defaultValue">Default value n?u không tìm th?y</param>
        /// <returns>Configuration value ho?c default value</returns>
        public string GetConfigValue(string key, string defaultValue)
        {
            return _configuration[key] ?? defaultValue;
        }
    }
}
