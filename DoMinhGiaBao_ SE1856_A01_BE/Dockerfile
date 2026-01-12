# ========================================
# Dockerfile cho FU News Management API
# .NET 8 Backend Application
# ========================================

# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files và restore dependencies
COPY ["DoMinhGiaBao_ SE1856_A01_BE/DoMinhGiaBao_ SE1856_A01_BE.csproj", "DoMinhGiaBao_ SE1856_A01_BE/"]
COPY ["DoMinhGiaBao_SE1856_A01_Service/DoMinhGiaBao_SE1856_A01_Service.csproj", "DoMinhGiaBao_SE1856_A01_Service/"]
COPY ["DoMinhGiaBao_SE1856_A01_Repository/DoMinhGiaBao_SE1856_A01_Repository.csproj", "DoMinhGiaBao_SE1856_A01_Repository/"]

# Restore packages
RUN dotnet restore "DoMinhGiaBao_ SE1856_A01_BE/DoMinhGiaBao_ SE1856_A01_BE.csproj"

# Copy toàn b? source code
COPY . .

# Build project
WORKDIR "/src/DoMinhGiaBao_ SE1856_A01_BE"
RUN dotnet build "DoMinhGiaBao_ SE1856_A01_BE.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "DoMinhGiaBao_ SE1856_A01_BE.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Expose port 80 (HTTP) và 443 (HTTPS)
EXPOSE 80
EXPOSE 443

# Copy published files t? stage publish
COPY --from=publish /app/publish .

# Copy seed data JSON file (copy from source instead of build stage)
COPY ["DoMinhGiaBao_ SE1856_A01_BE/SeedData", "SeedData/"]

# Set entry point
ENTRYPOINT ["dotnet", "DoMinhGiaBao_ SE1856_A01_BE.dll"]
