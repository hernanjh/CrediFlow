# CrediFlow Backend Setup Script
Set-Location "d:\Hernan\OneDrive\OneDrive - Garantizar S.G.R\Repos\CrediFlow"

# Create project directories
New-Item -ItemType Directory -Force -Path "backend\src"
New-Item -ItemType Directory -Force -Path "backend\tests"

# Create projects
dotnet new classlib -n CrediFlow.Domain -o backend/src/CrediFlow.Domain --force
dotnet new classlib -n CrediFlow.Application -o backend/src/CrediFlow.Application --force
dotnet new classlib -n CrediFlow.Infrastructure -o backend/src/CrediFlow.Infrastructure --force
dotnet new webapi -n CrediFlow.API -o backend/src/CrediFlow.API --force

# Add to solution
dotnet sln add backend/src/CrediFlow.Domain/CrediFlow.Domain.csproj
dotnet sln add backend/src/CrediFlow.Application/CrediFlow.Application.csproj
dotnet sln add backend/src/CrediFlow.Infrastructure/CrediFlow.Infrastructure.csproj
dotnet sln add backend/src/CrediFlow.API/CrediFlow.API.csproj

# Add project references
dotnet add backend/src/CrediFlow.Application/CrediFlow.Application.csproj reference backend/src/CrediFlow.Domain/CrediFlow.Domain.csproj
dotnet add backend/src/CrediFlow.Infrastructure/CrediFlow.Infrastructure.csproj reference backend/src/CrediFlow.Domain/CrediFlow.Domain.csproj
dotnet add backend/src/CrediFlow.Infrastructure/CrediFlow.Infrastructure.csproj reference backend/src/CrediFlow.Application/CrediFlow.Application.csproj
dotnet add backend/src/CrediFlow.API/CrediFlow.API.csproj reference backend/src/CrediFlow.Application/CrediFlow.Application.csproj
dotnet add backend/src/CrediFlow.API/CrediFlow.API.csproj reference backend/src/CrediFlow.Infrastructure/CrediFlow.Infrastructure.csproj

Write-Host "Project structure created successfully!" -ForegroundColor Green
