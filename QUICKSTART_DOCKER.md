# ?? QUICK START - DOCKER DEPLOYMENT

## ? Ch?y nhanh trong 3 b??c

### **1?? Start Docker Desktop**
- M? Docker Desktop
- ??m b?o Docker ?ang ch?y (icon ? system tray màu xanh)

### **2?? Deploy API + Database**
```powershell
# M? PowerShell t?i th? m?c project
cd D:\DoMinhGiaBao_ SE1856_A01_BE\

# Ch?y script deploy
.\deploy.ps1
```

**Ho?c ch?y th? công:**
```powershell
docker-compose up -d --build
```

### **3?? Restore Database**
```powershell
.\restore-database.ps1
```

---

## ? Ki?m tra

### **API ?ã ch?y ch?a?**
```
http://localhost:5000/swagger
```

### **Test Login API:**
```http
POST http://localhost:5000/api/auth/login
Content-Type: application/json

{
  "email": "admin@FUNewsManagementSystem.org",
  "password": "@@abc123@@"
}
```

---

## ?? Connect Frontend

### **Trong Frontend appsettings.json:**
```json
{
  "ApiSettings": {
    "BaseUrl": "http://localhost:5000/api"
  }
}
```

### **Trong Frontend Program.cs:**
```csharp
builder.Services.AddHttpClient("FUNewsAPI", client =>
{
    client.BaseAddress = new Uri("http://localhost:5000/api/");
});
```

### **S? d?ng trong Controller/Service:**
```csharp
public class NewsController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    
    public NewsController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
    
    public async Task<IActionResult> Index()
    {
        var client = _httpClientFactory.CreateClient("FUNewsAPI");
        var response = await client.GetAsync("news-articles?status=active");
        
        if (response.IsSuccessStatusCode)
        {
            var articles = await response.Content
                .ReadFromJsonAsync<List<NewsArticleDto>>();
            return View(articles);
        }
        
        return View(new List<NewsArticleDto>());
    }
}
```

---

## ?? Thông tin k?t n?i

| Service | URL | Credentials |
|---------|-----|-------------|
| **API** | http://localhost:5000 | - |
| **Swagger** | http://localhost:5000/swagger | - |
| **SQL Server** | localhost:1433 | sa / YourStrong@Password123 |
| **Admin Login** | - | admin@FUNewsManagementSystem.org / @@abc123@@ |

---

## ?? Stop Services

```powershell
docker-compose down
```

**?? xóa luôn database:**
```powershell
docker-compose down -v
```

---

## ?? Restart Services

```powershell
docker-compose restart
```

---

## ?? View Logs

```powershell
# T?t c? services
docker-compose logs -f

# Ch? API
docker-compose logs -f api

# Ch? SQL Server
docker-compose logs -f sqlserver
```

---

## ?? Troubleshooting

### **L?i: Port already in use**
```powershell
# ??i port trong docker-compose.yml
# Thay "5000:80" thành "5001:80"
```

### **L?i: Cannot connect to SQL Server**
```powershell
# Ki?m tra SQL Server ?ã ready ch?a
docker-compose logs sqlserver

# Ch? thêm 30 giây r?i th? l?i
```

### **L?i: Database không t?n t?i**
```powershell
# Ch?y l?i restore script
.\restore-database.ps1
```

---

## ?? Frontend Connection Example

### **JavaScript/Fetch:**
```javascript
// Get news articles
fetch('http://localhost:5000/api/news-articles?status=active')
    .then(response => response.json())
    .then(data => console.log(data));

// Login
fetch('http://localhost:5000/api/auth/login', {
    method: 'POST',
    headers: {
        'Content-Type': 'application/json'
    },
    body: JSON.stringify({
        email: 'admin@FUNewsManagementSystem.org',
        password: '@@abc123@@'
    })
})
.then(response => response.json())
.then(data => console.log(data));
```

### **C# HttpClient:**
```csharp
// Razor Pages / MVC
public async Task<IActionResult> OnGetAsync()
{
    using var client = new HttpClient();
    client.BaseAddress = new Uri("http://localhost:5000/api/");
    
    var response = await client.GetAsync("news-articles?status=active");
    if (response.IsSuccessStatusCode)
    {
        NewsArticles = await response.Content
            .ReadFromJsonAsync<List<NewsArticleDto>>();
    }
    
    return Page();
}
```

---

## ?? Chi ti?t h?n

Xem file `DOCKER_DEPLOYMENT.md` ?? bi?t thêm chi ti?t v?:
- Cách restore database t? file .bak
- Deploy lên production server
- Advanced configuration
- Troubleshooting chi ti?t

---

**Enjoy coding! ??**
