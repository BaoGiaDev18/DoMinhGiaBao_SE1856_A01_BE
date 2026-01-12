# ?? DOCKER DEPLOYMENT GUIDE

## ?? Yêu c?u (Prerequisites)

### 1. Cài ??t Docker Desktop
- Download: https://www.docker.com/products/docker-desktop/
- Ch?y Docker Desktop và ??m b?o nó ?ang ho?t ??ng

### 2. Ki?m tra Docker ?ã cài ??t
```powershell
docker --version
docker-compose --version
```

---

## ?? CÁCH 1: Deploy v?i Docker Compose (Khuy?n ngh?)

### **B??c 1: Clone/Download Project**
```powershell
cd D:\DoMinhGiaBao_ SE1856_A01_BE\
```

### **B??c 2: Ch?y Deploy Script**
```powershell
.\deploy.ps1
```

**Ho?c ch?y th? công:**
```powershell
# Build và start containers
docker-compose up -d --build

# Xem logs
docker-compose logs -f
```

### **B??c 3: Restore Database**
```powershell
.\restore-database.ps1
```

**Ho?c restore th? công:**
```powershell
# Vào container SQL Server
docker exec -it funews-sqlserver bash

# Ch?y sqlcmd
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P YourStrong@Password123

# T?o database
CREATE DATABASE FUNewsManagement;
GO
```

### **B??c 4: Ki?m tra API**
```
http://localhost:5000/swagger
```

---

## ??? CÁCH 2: Ch? Deploy API (Dùng SQL Server local)

### **B??c 1: Build Docker Image**
```powershell
cd DoMinhGiaBao_ SE1856_A01_BE
docker build -t funews-api:latest .
```

### **B??c 2: Run Container**
```powershell
docker run -d `
  --name funews-api `
  -p 5000:80 `
  -e ASPNETCORE_ENVIRONMENT=Development `
  -e "ConnectionStrings__DefaultConnection=Server=host.docker.internal;Database=FUNewsManagement;User Id=sa;Password=12345;TrustServerCertificate=True;" `
  funews-api:latest
```

**L?u ý:** 
- `host.docker.internal` ?? connect t?i SQL Server trên máy host
- Thay `12345` b?ng password SQL Server c?a b?n

---

## ?? Services Information

### **API Endpoints:**
- **Base URL**: `http://localhost:5000`
- **Swagger UI**: `http://localhost:5000/swagger`
- **OData**: `http://localhost:5000/odata`

### **SQL Server (Docker):**
- **Host**: `localhost`
- **Port**: `1433`
- **User**: `sa`
- **Password**: `YourStrong@Password123`
- **Database**: `FUNewsManagement`

### **Admin Account:**
- **Email**: `admin@FUNewsManagementSystem.org`
- **Password**: `@@abc123@@`

---

## ?? Connect Frontend v?i API

### **Trong Frontend (.NET MVC ho?c Razor Pages):**

```csharp
// appsettings.json c?a Frontend
{
  "ApiSettings": {
    "BaseUrl": "http://localhost:5000/api"
  }
}
```

```csharp
// S? d?ng HttpClient
public class NewsService
{
    private readonly HttpClient _httpClient;
    
    public NewsService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(configuration["ApiSettings:BaseUrl"]);
    }
    
    public async Task<List<NewsArticleDto>> GetNewsArticlesAsync()
    {
        var response = await _httpClient.GetAsync("/news-articles?status=active");
        return await response.Content.ReadFromJsonAsync<List<NewsArticleDto>>();
    }
}
```

### **Register trong Program.cs c?a Frontend:**
```csharp
builder.Services.AddHttpClient<INewsService, NewsService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5000/api/");
});
```

---

## ?? Docker Commands

### **Xem logs:**
```powershell
# All services
docker-compose logs -f

# Ch? API
docker-compose logs -f api

# Ch? SQL Server
docker-compose logs -f sqlserver
```

### **Stop containers:**
```powershell
docker-compose down
```

### **Stop và xóa volumes:**
```powershell
docker-compose down -v
```

### **Restart containers:**
```powershell
docker-compose restart
```

### **Rebuild containers:**
```powershell
docker-compose up -d --build
```

### **Xem container status:**
```powershell
docker-compose ps
docker ps
```

### **Vào container shell:**
```powershell
# API container
docker exec -it funews-api bash

# SQL Server container
docker exec -it funews-sqlserver bash
```

---

## ?? Troubleshooting

### **1. Port ?ã ???c s? d?ng:**
```powershell
# Thay ??i port trong docker-compose.yml
ports:
  - "5001:80"  # Thay vì 5000
```

### **2. SQL Server không start:**
```powershell
# Xem logs
docker-compose logs sqlserver

# Ki?m tra password requirements
# Password ph?i có ít nh?t 8 ký t?, ch? hoa, ch? th??ng, s? và ký t? ??c bi?t
```

### **3. API không connect ???c SQL Server:**
```powershell
# Ki?m tra network
docker network inspect funews-network

# Test connection t? API container
docker exec -it funews-api ping sqlserver
```

### **4. Database ch?a t?n t?i:**
```powershell
# Ch?y script restore
.\restore-database.ps1

# Ho?c t?o manual
docker exec -it funews-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P YourStrong@Password123 -Q "CREATE DATABASE FUNewsManagement"
```

---

## ?? Restore Database t? File .bak

### **B??c 1: Copy file .bak vào container**
```powershell
docker cp FUNewsManagement.bak funews-sqlserver:/var/opt/mssql/data/
```

### **B??c 2: Restore**
```powershell
docker exec -it funews-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P YourStrong@Password123 -Q "RESTORE DATABASE FUNewsManagement FROM DISK='/var/opt/mssql/data/FUNewsManagement.bak' WITH REPLACE"
```

---

## ?? Deploy lên Server (Production)

### **1. Chu?n b?:**
```powershell
# T?o production appsettings
# DoMinhGiaBao_ SE1856_A01_BE/appsettings.Production.json
```

### **2. Build production image:**
```powershell
docker build -t funews-api:production -f DoMinhGiaBao_ SE1856_A01_BE/Dockerfile .
```

### **3. Push to Docker Hub (Optional):**
```powershell
docker tag funews-api:production yourusername/funews-api:latest
docker push yourusername/funews-api:latest
```

### **4. Deploy trên server:**
```bash
docker pull yourusername/funews-api:latest
docker-compose -f docker-compose.production.yml up -d
```

---

## ? Checklist

- [ ] Docker Desktop ?ã cài ??t và ?ang ch?y
- [ ] ?ã ch?y `docker-compose up -d --build`
- [ ] SQL Server container healthy (check: `docker ps`)
- [ ] Database ?ã ???c t?o/restore
- [ ] API accessible t?i `http://localhost:5000/swagger`
- [ ] Frontend có th? call API thành công
- [ ] CORS ?ã ???c c?u hình ?úng

---

## ?? Tài li?u tham kh?o

- Docker Documentation: https://docs.docker.com/
- Docker Compose: https://docs.docker.com/compose/
- SQL Server on Docker: https://hub.docker.com/_/microsoft-mssql-server
- .NET in Docker: https://learn.microsoft.com/en-us/dotnet/core/docker/

---

**Tác gi?:** DoMinhGiaBao  
**Project:** FU News Management System  
**Version:** 1.0
