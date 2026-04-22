using CrediFlow.Application.Common;
using CrediFlow.Domain.Enums;
using CrediFlow.Domain.Interfaces;
using CrediFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CrediFlow.Infrastructure.Services;

/// <summary>
/// Motor de Cuotas: genera la tabla de vencimientos respetando feriados y domingos
/// </summary>
public class MotorCuotasService : IMotorCuotasService
{
    private readonly CrediFlowDbContext _context;

    public MotorCuotasService(CrediFlowDbContext context) => _context = context;

    public async Task<List<(int Numero, DateTime Fecha, decimal Monto)>> GenerarCuotasAsync(
        decimal capital, decimal tasaInteres, int numeroCuotas,
        FrecuenciaCuota frecuencia, DateTime fechaPrimerVencimiento,
        CancellationToken ct = default)
    {
        var montoTotal = capital * (1 + tasaInteres / 100m);
        var montoCuota = Math.Round(montoTotal / numeroCuotas, 2);

        // Ajuste de redondeo en la última cuota
        var montoUltimaCuota = montoTotal - (montoCuota * (numeroCuotas - 1));

        var diasFrecuencia = (int)frecuencia;

        // Cargar feriados del próximo año
        var desde = fechaPrimerVencimiento.Date;
        var hasta = desde.AddDays(diasFrecuencia * numeroCuotas + 30);
        var feriados = await _context.Feriados
            .Where(f => f.Activo && f.Fecha >= desde && f.Fecha <= hasta)
            .Select(f => f.Fecha.Date)
            .ToHashSetAsync(ct);

        var cuotas = new List<(int, DateTime, decimal)>();
        var fechaActual = fechaPrimerVencimiento.Date;

        for (int i = 1; i <= numeroCuotas; i++)
        {
            // Ajustar a próxima fecha hábil si cae en feriado o domingo
            fechaActual = AjustarFechaHabil(fechaActual, feriados);

            var monto = i == numeroCuotas ? montoUltimaCuota : montoCuota;
            cuotas.Add((i, fechaActual, monto));

            // Avanzar para la próxima cuota
            if (i < numeroCuotas)
                fechaActual = fechaActual.AddDays(diasFrecuencia);
        }

        return cuotas;
    }

    private static DateTime AjustarFechaHabil(DateTime fecha, HashSet<DateTime> feriados)
    {
        int intentos = 0;
        while ((fecha.DayOfWeek == DayOfWeek.Sunday || feriados.Contains(fecha.Date)) && intentos < 10)
        {
            fecha = fecha.AddDays(1);
            intentos++;
        }
        return fecha;
    }
}
