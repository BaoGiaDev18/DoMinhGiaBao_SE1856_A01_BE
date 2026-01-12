# ========================================
# Quick Deploy & Test Script
# ========================================

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "?? FU News Management - Quick Deploy" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Stop existing containers and remove volumes (fresh start)
Write-Host "?? Stopping existing containers and cleaning up..." -ForegroundColor Yellow
docker-compose down -v

Write-Host ""
Write-Host "?? Building and starting containers..." -ForegroundColor Yellow
docker-compose up -d --build

Write-Host ""
Write-Host "? Waiting 30 seconds for SQL Server to initialize..." -ForegroundColor Yellow
Start-Sleep -Seconds 30

Write-Host ""
Write-Host "?? Checking container status..." -ForegroundColor Cyan
docker-compose ps

Write-Host ""
Write-Host "?? Viewing API logs (last 30 lines)..." -ForegroundColor Cyan
docker-compose logs --tail=30 api

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "? Deployment Complete!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "?? Services:" -ForegroundColor Cyan
Write-Host "   - API: http://localhost:5000" -ForegroundColor White
Write-Host "   - Swagger: http://localhost:5000/swagger" -ForegroundColor White
Write-Host ""
Write-Host "?? Test Credentials:" -ForegroundColor Cyan
Write-Host "   - Admin: admin@FUNewsManagementSystem.org / @@abc123@@" -ForegroundColor White
Write-Host "   - User 1: EmmaWilliam@FUNewsManagement.org / @1" -ForegroundColor White
Write-Host "   - User 2: OliviaJames@FUNewsManagement.org / @1" -ForegroundColor White
Write-Host ""
Write-Host "?? Quick Test:" -ForegroundColor Cyan
Write-Host ""

# Test 1: Login
Write-Host "   1?? Testing Login..." -ForegroundColor Yellow
try {
    $loginResult = Invoke-RestMethod -Uri "http://localhost:5000/api/auth/login" -Method POST -ContentType "application/json" -Body '{"email":"EmmaWilliam@FUNewsManagement.org","password":"@1"}' -ErrorAction Stop
    Write-Host "      ? Login successful: $($loginResult.accountName)" -ForegroundColor Green
} catch {
    Write-Host "      ? Login failed: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 2: Get Categories
Write-Host ""
Write-Host "   2?? Testing Categories API..." -ForegroundColor Yellow
try {
    $categories = Invoke-RestMethod -Uri "http://localhost:5000/api/categories" -Method GET -ErrorAction Stop
    Write-Host "      ? Found $($categories.Count) categories" -ForegroundColor Green
    $categories | ForEach-Object {
        Write-Host "         - $($_.categoryName)" -ForegroundColor Gray
    }
} catch {
    Write-Host "      ? Categories failed: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 3: Get Tags
Write-Host ""
Write-Host "   3?? Testing Tags API..." -ForegroundColor Yellow
try {
    $tags = Invoke-RestMethod -Uri "http://localhost:5000/api/tags" -Method GET -ErrorAction Stop
    Write-Host "      ? Found $($tags.Count) tags" -ForegroundColor Green
} catch {
    Write-Host "      ? Tags failed: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 4: Get News Articles
Write-Host ""
Write-Host "   4?? Testing News Articles API..." -ForegroundColor Yellow
try {
    $articles = Invoke-RestMethod -Uri "http://localhost:5000/api/news-articles" -Method GET -ErrorAction Stop
    Write-Host "      ? Found $($articles.Count) news articles" -ForegroundColor Green
    $articles | Select-Object -First 3 | ForEach-Object {
        Write-Host "         - $($_.newsTitle)" -ForegroundColor Gray
    }
} catch {
    Write-Host "      ? News articles failed: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "?? All tests completed!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "?? Useful Commands:" -ForegroundColor Cyan
Write-Host "   - View logs: docker-compose logs -f api" -ForegroundColor White
Write-Host "   - Stop: docker-compose down" -ForegroundColor White
Write-Host "   - Restart: docker-compose restart" -ForegroundColor White
Write-Host ""
