# Install NuGet packages
$baseDir = "d:\Hernan\OneDrive\OneDrive - Garantizar S.G.R\Repos\CrediFlow"
Set-Location $baseDir

$domainProj = "backend/src/CrediFlow.Domain/CrediFlow.Domain.csproj"
$appProj = "backend/src/CrediFlow.Application/CrediFlow.Application.csproj"
$infraProj = "backend/src/CrediFlow.Infrastructure/CrediFlow.Infrastructure.csproj"
$apiProj = "backend/src/CrediFlow.API/CrediFlow.API.csproj"

# Domain packages
dotnet add $domainProj package Microsoft.Extensions.DependencyInjection.Abstractions

# Application packages
dotnet add $appProj package MediatR
dotnet add $appProj package FluentValidation
dotnet add $appProj package FluentValidation.DependencyInjectionExtensions
dotnet add $appProj package Microsoft.Extensions.Logging.Abstractions
dotnet add $appProj package AutoMapper

# Infrastructure packages
dotnet add $infraProj package Microsoft.EntityFrameworkCore.Sqlite
dotnet add $infraProj package Microsoft.EntityFrameworkCore.Design
dotnet add $infraProj package Hangfire.Core
dotnet add $infraProj package Hangfire.SQLite
dotnet add $infraProj package Hangfire.AspNetCore
dotnet add $infraProj package QuestPDF
dotnet add $infraProj package ClosedXML
dotnet add $infraProj package Microsoft.AspNetCore.Http.Abstractions

# API packages
dotnet add $apiProj package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add $apiProj package Microsoft.EntityFrameworkCore.Design
dotnet add $apiProj package Swashbuckle.AspNetCore
dotnet add $apiProj package Serilog.AspNetCore
dotnet add $apiProj package Serilog.Sinks.File
dotnet add $apiProj package AspNetCoreRateLimit
dotnet add $apiProj package AutoMapper.Extensions.Microsoft.DependencyInjection

Write-Host "All packages installed!" -ForegroundColor Green
