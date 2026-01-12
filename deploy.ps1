# ========================================
# Build và Deploy FU News Management API
# ========================================

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "FU News Management System - Docker Deploy" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# B??c 1: Stop và remove containers c?
Write-Host "[1/5] Stopping and removing old containers..." -ForegroundColor Yellow
docker-compose down -v

# B??c 2: Build Docker image
Write-Host ""
Write-Host "[2/5] Building Docker image..." -ForegroundColor Yellow
docker-compose build --no-cache

# B??c 3: Start containers
Write-Host ""
Write-Host "[3/5] Starting containers..." -ForegroundColor Yellow
docker-compose up -d

# B??c 4: Wait for SQL Server to be ready
Write-Host ""
Write-Host "[4/5] Waiting for SQL Server to be ready..." -ForegroundColor Yellow
Start-Sleep -Seconds 30

# B??c 5: Show container status
Write-Host ""
Write-Host "[5/5] Container status:" -ForegroundColor Yellow
docker-compose ps

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "? Deployment Complete!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "?? Services:" -ForegroundColor Cyan
Write-Host "   - API: http://localhost:5000" -ForegroundColor White
Write-Host "   - Swagger: http://localhost:5000/swagger" -ForegroundColor White
Write-Host "   - SQL Server: localhost:1433" -ForegroundColor White
Write-Host ""
Write-Host "?? SQL Server Credentials:" -ForegroundColor Cyan
Write-Host "   - User: sa" -ForegroundColor White
Write-Host "   - Password: YourStrong@Password123" -ForegroundColor White
Write-Host ""
Write-Host "?? Admin Account:" -ForegroundColor Cyan
Write-Host "   - Email: admin@FUNewsManagementSystem.org" -ForegroundColor White
Write-Host "   - Password: @@abc123@@" -ForegroundColor White
Write-Host ""
Write-Host "?? Docker Commands:" -ForegroundColor Cyan
Write-Host "   - View logs: docker-compose logs -f" -ForegroundColor White
Write-Host "   - Stop: docker-compose down" -ForegroundColor White
Write-Host "   - Restart: docker-compose restart" -ForegroundColor White
Write-Host ""
