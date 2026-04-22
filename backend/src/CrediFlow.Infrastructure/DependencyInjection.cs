using CrediFlow.Application.Common;
using CrediFlow.Domain.Interfaces;
using CrediFlow.Infrastructure.Jobs;
using CrediFlow.Infrastructure.Persistence;
using CrediFlow.Infrastructure.Repositories;
using CrediFlow.Infrastructure.Services;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CrediFlow.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connStr = configuration.GetConnectionString("DefaultConnection")
            ?? "Data Source=crediflow.db";

        // ─── EF CORE + SQLITE ─────────────────────────────────────────────
        services.AddDbContext<CrediFlowDbContext>(opt =>
            opt.UseSqlite(connStr));

        // ─── REPOSITORIES ─────────────────────────────────────────────────
        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddScoped<ICreditoRepository, CreditoRepository>();
        services.AddScoped<ICuotaRepository, CuotaRepository>();
        services.AddScoped<IPagoCobradoRepository, PagoCobradoRepository>();
        services.AddScoped<ICierreCajaRepository, CierreCajaRepository>();
        services.AddScoped<IProductoRepository, ProductoRepository>();
        services.AddScoped<IProveedorRepository, ProveedorRepository>();
        services.AddScoped<ICategoriaProductoRepository, CategoriaProductoRepository>();
        services.AddScoped<IConfiguracionRepository, ConfiguracionRepository>();
        services.AddScoped<IPerfilRepository, PerfilRepository>();
        services.AddScoped<IVentaRepository, VentaRepository>();
        services.AddScoped<IListaPrecioRepository, ListaPrecioRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IZonaRepository, ZonaRepository>();
        services.AddScoped<IFeriadoRepository, FeriadoRepository>();
        services.AddScoped<IAlertaRepository, AlertaRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // ─── SERVICES ─────────────────────────────────────────────────────
        services.AddScoped<IMotorCuotasService, MotorCuotasService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IPasswordService, PasswordService>();

        // ─── HANGFIRE ─────────────────────────────────────────────────────
        services.AddHangfire(config =>
            config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                  .UseSimpleAssemblyNameTypeSerializer()
                  .UseRecommendedSerializerSettings()
                  .UseMemoryStorage());

        services.AddHangfireServer(opt =>
        {
            opt.WorkerCount = 2;
            opt.ServerName = "CrediFlow-Server";
        });

        services.AddScoped<MoraCalculationJob>();

        // ─── DATA SEEDER ──────────────────────────────────────────────────
        services.AddScoped<DataSeeder>();

        return services;
    }
}
