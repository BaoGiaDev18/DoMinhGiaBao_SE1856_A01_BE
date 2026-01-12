# ========================================
# Restore Database t? file .bak ho?c script SQL
# ========================================

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Database Restore Script" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Ki?m tra container có ?ang ch?y không
$containerStatus = docker ps --filter "name=funews-sqlserver" --format "{{.Status}}"

if (-not $containerStatus) {
    Write-Host "? SQL Server container is not running!" -ForegroundColor Red
    Write-Host "Please run 'docker-compose up -d' first" -ForegroundColor Yellow
    exit 1
}

Write-Host "? SQL Server container is running" -ForegroundColor Green
Write-Host ""

# Option 1: T?o database m?i
Write-Host "[Option 1] Create new database" -ForegroundColor Yellow
$createDbScript = @"
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'FUNewsManagement')
BEGIN
    CREATE DATABASE FUNewsManagement;
    PRINT 'Database created successfully';
END
ELSE
BEGIN
    PRINT 'Database already exists';
END
"@

docker exec -it funews-sqlserver /opt/mssql-tools/bin/sqlcmd `
    -S localhost -U sa -P "YourStrong@Password123" `
    -Q "$createDbScript"

Write-Host ""
Write-Host "? Database ready!" -ForegroundColor Green
Write-Host ""
Write-Host "?? Next steps:" -ForegroundColor Cyan
Write-Host "1. Run Entity Framework migrations:" -ForegroundColor White
Write-Host "   dotnet ef database update --project DoMinhGiaBao_SE1856_A01_Repository" -ForegroundColor Gray
Write-Host ""
Write-Host "2. Or restore from backup file (.bak)" -ForegroundColor White
Write-Host ""
Write-Host "3. Or run your SQL scripts manually" -ForegroundColor White
Write-Host ""
