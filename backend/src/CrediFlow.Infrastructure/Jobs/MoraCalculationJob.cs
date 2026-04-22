using CrediFlow.Domain.Enums;
using CrediFlow.Infrastructure.Persistence;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CrediFlow.Infrastructure.Jobs;

public class MoraCalculationJob
{
    private readonly CrediFlowDbContext _ctx;
    private readonly ILogger<MoraCalculationJob> _logger;

    public MoraCalculationJob(CrediFlowDbContext ctx, ILogger<MoraCalculationJob> logger)
    {
        _ctx = ctx;
        _logger = logger;
    }

    [AutomaticRetry(Attempts = 3)]
    public async Task CalcularMoraAsync()
    {
        _logger.LogInformation("Iniciando cálculo de mora...");

        var cuotasVencidas = await _ctx.Cuotas
            .Include(c => c.Credito)
            .Where(c =>
                c.Estado != EstadoCuota.PAGADA &&
                c.FechaVencimiento.Date < DateTime.UtcNow.Date)
            .ToListAsync();

        int actualizadas = 0;
        foreach (var cuota in cuotasVencidas)
        {
            var tasaDiaria = cuota.Credito?.TasaMoratoria / 100m / 30m ?? 0.001m;
            cuota.AplicarMora(tasaDiaria);
            actualizadas++;
        }

        // Actualizar estados de créditos
        var creditosConMora = await _ctx.Creditos
            .Include(c => c.Cuotas)
            .Where(c => c.Estado != EstadoCredito.CANCELADO && c.Estado != EstadoCredito.INCOBRABLE)
            .ToListAsync();

        foreach (var credito in creditosConMora)
            credito.RecalcularEstado();

        await _ctx.SaveChangesAsync();
        _logger.LogInformation("Mora calculada: {Count} cuotas actualizadas.", actualizadas);
    }

    [AutomaticRetry(Attempts = 2)]
    public async Task RecalcularFlujoDeFondosAsync()
    {
        _logger.LogInformation("Recalculando flujo de fondos proyectado...");

        // Limpiar proyecciones anteriores
        var anteriores = await _ctx.FlujoCajaProyectado.ToListAsync();
        _ctx.FlujoCajaProyectado.RemoveRange(anteriores);

        var hoy = DateTime.UtcNow.Date;
        var proyecciones = new List<Domain.Entities.FlujoCajaProyectado>();

        for (int semana = 1; semana <= 4; semana++)
        {
            var desde = hoy.AddDays((semana - 1) * 7);
            var hasta = hoy.AddDays(semana * 7 - 1);

            var cuotasSemana = await _ctx.Cuotas
                .Where(c =>
                    c.Estado != EstadoCuota.PAGADA &&
                    c.FechaVencimiento.Date >= desde &&
                    c.FechaVencimiento.Date <= hasta)
                .ToListAsync();

            proyecciones.Add(new Domain.Entities.FlujoCajaProyectado
            {
                Semana = semana,
                FechaDesde = desde,
                FechaHasta = hasta,
                MontoProyectado = cuotasSemana.Sum(c => c.SaldoPendiente),
                MontoRecuperado = 0,
                CantidadCuotas = cuotasSemana.Count
            });
        }

        await _ctx.FlujoCajaProyectado.AddRangeAsync(proyecciones);
        await _ctx.SaveChangesAsync();
        _logger.LogInformation("Flujo de fondos proyectado actualizado (próximas 4 semanas).");
    }
}
