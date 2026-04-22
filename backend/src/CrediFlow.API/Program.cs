using CrediFlow.Application.Common;
using CrediFlow.Infrastructure;
using CrediFlow.Infrastructure.Jobs;
using CrediFlow.Infrastructure.Persistence;
using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ─── SERILOG ───────────────────────────────────────────────────────────────
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/crediflow-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// ─── SERVICES ───────────────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger con JWT
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CrediFlow API",
        Version = "v1",
        Description = "Sistema de Gestión de Microcréditos y Cobranza"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Ingrese el token JWT"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
            Array.Empty<string>()
        }
    });
});

// Infrastructure (EF, Repos, Hangfire)
builder.Services.AddInfrastructure(builder.Configuration);

// MediatR - Application Layer
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(
        typeof(CrediFlow.Application.Features.Clientes.GetClientesHandler).Assembly));

// Current User Service
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"]!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opts =>
    {
        opts.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddAuthorization();

// CORS para el frontend React
builder.Services.AddCors(opts =>
    opts.AddPolicy("ReactApp", policy =>
        policy.WithOrigins(
            "http://localhost:5173",
            "http://localhost:5174",
            "http://localhost:5175",
            "http://localhost:3000")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()));

var app = builder.Build();

// ─── MIDDLEWARE PIPELINE ────────────────────────────────────────────────────
app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CrediFlow API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseCors("ReactApp");
app.UseAuthentication();
app.UseAuthorization();

// app.UseHangfireDashboard("/hangfire", new DashboardOptions
// {
//     Authorization = new[] { new Hangfire.Dashboard.LocalRequestsOnlyAuthorizationFilter() }
// });

app.MapControllers();

// ─── SEED DATA Y BACKGROUND JOBS ───────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
    await seeder.SeedAsync();

    // Registrar jobs recurrentes usando IRecurringJobManager (DI)
    var recurringJobManager = scope.ServiceProvider.GetService<IRecurringJobManager>();
    if (recurringJobManager != null)
    {
        recurringJobManager.AddOrUpdate<MoraCalculationJob>(
            "calcular-mora",
            job => job.CalcularMoraAsync(),
            "0 * * * *"); // cada hora

        recurringJobManager.AddOrUpdate<MoraCalculationJob>(
            "flujo-fondos",
            job => job.RecalcularFlujoDeFondosAsync(),
            "0 */4 * * *"); // cada 4 horas
    }
}


app.Run();

// ─── CURRENT USER SERVICE IMPLEMENTATION ───────────────────────────────────
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        => _httpContextAccessor = httpContextAccessor;

    private System.Security.Claims.ClaimsPrincipal? User
        => _httpContextAccessor.HttpContext?.User;

    public Guid? UserId
    {
        get
        {
            var val = User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                ?? User?.FindFirst("sub")?.Value;
            return Guid.TryParse(val, out var id) ? id : null;
        }
    }

    public string? Email => User?.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
        ?? User?.FindFirst("email")?.Value;

    public string? Role => User?.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

    public bool IsSuperAdmin => Role == "SUPER_ADMIN";
    public bool IsAdmin => Role == "ADMIN" || IsSuperAdmin;
    public bool IsVendedor => Role == "VENDEDOR";

    public string? IpAddress
        => _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
}
