using CrediFlow.Domain.Common;
using CrediFlow.Domain.Enums;
using CrediFlow.Domain.Events;

namespace CrediFlow.Domain.Entities;

public class Credito : BaseEntity
{
    public Guid ClienteId { get; private set; }
    public Cliente? Cliente { get; private set; }
    public Guid VendedorId { get; private set; }
    public Usuario? Vendedor { get; private set; }
    public decimal Capital { get; private set; }
    public decimal TasaInteres { get; private set; } // porcentaje total sobre el capital
    public decimal TasaMoratoria { get; private set; } // porcentaje diario por mora
    public int NumeroCuotas { get; private set; }
    public FrecuenciaCuota Frecuencia { get; private set; }
    public decimal MontoTotalConInteres { get; private set; }
    public decimal MontoPorCuota { get; private set; }
    public decimal SaldoPendiente { get; private set; }
    public DateTime FechaOtorgamiento { get; private set; }
    public DateTime FechaPrimerVencimiento { get; private set; }
    public EstadoCredito Estado { get; private set; } = EstadoCredito.ACTIVO;
    public string? Observaciones { get; private set; }
    public Guid? ProductoId { get; private set; } // si fue una venta de producto

    private readonly List<Cuota> _cuotas = new();
    public IReadOnlyCollection<Cuota> Cuotas => _cuotas.AsReadOnly();

    protected Credito() { }

    public static Credito Crear(Guid clienteId, Guid vendedorId, decimal capital,
        decimal tasaInteres, decimal tasaMoratoria, int numeroCuotas,
        FrecuenciaCuota frecuencia, DateTime fechaPrimerVencimiento,
        string? observaciones = null, Guid? productoId = null)
    {
        if (capital <= 0) throw new ArgumentException("El capital debe ser mayor a cero.");
        if (numeroCuotas <= 0) throw new ArgumentException("El número de cuotas debe ser mayor a cero.");

        var montoTotal = capital * (1 + tasaInteres / 100m);
        var montoPorCuota = Math.Round(montoTotal / numeroCuotas, 2);

        var credito = new Credito
        {
            ClienteId = clienteId,
            VendedorId = vendedorId,
            Capital = capital,
            TasaInteres = tasaInteres,
            TasaMoratoria = tasaMoratoria,
            NumeroCuotas = numeroCuotas,
            Frecuencia = frecuencia,
            MontoTotalConInteres = montoTotal,
            MontoPorCuota = montoPorCuota,
            SaldoPendiente = montoTotal,
            FechaOtorgamiento = DateTime.UtcNow,
            FechaPrimerVencimiento = fechaPrimerVencimiento,
            Observaciones = observaciones,
            ProductoId = productoId
        };

        credito.AddDomainEvent(new CreditoOtorgadoEvent(credito.Id, clienteId, vendedorId, capital, numeroCuotas, frecuencia, fechaPrimerVencimiento, montoPorCuota));

        return credito;
    }

    public void RegistrarPago(decimal monto)
    {
        SaldoPendiente = Math.Max(0, SaldoPendiente - monto);
        if (SaldoPendiente <= 0)
        {
            Estado = EstadoCredito.CANCELADO;
        }
        else
        {
            RecalcularEstado();
        }
    }

    public void RecalcularEstado()
    {
        var cuotasVencidas = _cuotas.Where(c => c.Estado == EstadoCuota.EN_MORA).ToList();
        var diasMaxMora = cuotasVencidas.Any()
            ? cuotasVencidas.Max(c => (DateTime.UtcNow - c.FechaVencimiento).Days)
            : 0;

        Estado = diasMaxMora switch
        {
            0 => EstadoCredito.ACTIVO,
            <= 30 => EstadoCredito.EN_MORA,
            <= 90 => EstadoCredito.EN_MORA_GRAVE,
            _ => EstadoCredito.INCOBRABLE
        };
    }

    public void PasarAIncobrable()
    {
        Estado = EstadoCredito.INCOBRABLE;
        AddDomainEvent(new CreditoPasadoAIncobrableEvent(Id, ClienteId, SaldoPendiente));
    }

    public void AgregarCuotas(List<Cuota> cuotas) => _cuotas.AddRange(cuotas);
}
