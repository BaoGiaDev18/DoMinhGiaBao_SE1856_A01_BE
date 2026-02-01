# ========================================
# Dockerfile cho FU News Management API
# .NET 8 Backend Application
# ========================================

# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files và restore dependencies
COPY ["PRN232.LAB.API/PRN232.LAB.API.csproj", "PRN232.LAB.API/"]
COPY ["PRN232.LAB.Services/PRN232.LAB.Services.csproj", "PRN232.LAB.Services/"]
COPY ["PRN232.LAB.Repo/PRN232.LAB.Repo.csproj", "PRN232.LAB.Repo/"]

# Restore packages
RUN dotnet restore "PRN232.LAB.API/PRN232.LAB.API.csproj"

# Copy toàn b? source code
COPY . .

# Build project
WORKDIR "/src/PRN232.LAB.API"
RUN dotnet build "PRN232.LAB.API.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "PRN232.LAB.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Expose port 80 (HTTP) và 443 (HTTPS)
EXPOSE 80
EXPOSE 443

# Copy published files t? stage publish
COPY --from=publish /app/publish .

# Copy seed data JSON file (copy from source instead of build stage)
COPY ["PRN232.LAB.API/SeedData", "SeedData/"]

# Set entry point
ENTRYPOINT ["dotnet", "PRN232.LAB.API.dll"]
