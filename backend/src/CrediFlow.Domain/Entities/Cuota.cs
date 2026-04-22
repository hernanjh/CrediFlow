using CrediFlow.Domain.Common;
using CrediFlow.Domain.Enums;
using CrediFlow.Domain.Events;

namespace CrediFlow.Domain.Entities;

public class Cuota : BaseEntity
{
    public Guid CreditoId { get; private set; }
    public Credito? Credito { get; private set; }
    public int NumeroCuota { get; private set; }
    public DateTime FechaVencimiento { get; private set; }
    public decimal MontoOriginal { get; private set; }
    public decimal MontoPagado { get; private set; }
    public decimal InteresMora { get; private set; }
    public decimal SaldoPendiente => MontoOriginal + InteresMora - MontoPagado;
    public EstadoCuota Estado { get; private set; } = EstadoCuota.PENDIENTE;
    public DateTime? FechaPago { get; private set; }

    private readonly List<PagoCobrado> _pagos = new();
    public IReadOnlyCollection<PagoCobrado> Pagos => _pagos.AsReadOnly();

    protected Cuota() { }

    public static Cuota Crear(Guid creditoId, int numeroCuota, DateTime fechaVencimiento, decimal monto)
    {
        return new Cuota
        {
            CreditoId = creditoId,
            NumeroCuota = numeroCuota,
            FechaVencimiento = fechaVencimiento,
            MontoOriginal = monto
        };
    }

    public void AplicarMora(decimal tasaDiaria)
    {
        if (Estado == EstadoCuota.PAGADA) return;
        var diasVencido = Math.Max(0, (DateTime.UtcNow.Date - FechaVencimiento.Date).Days);
        if (diasVencido <= 0) return;

        InteresMora = Math.Round(MontoOriginal * tasaDiaria * diasVencido, 2);
        Estado = EstadoCuota.EN_MORA;
        AddDomainEvent(new CuotaVencidaEvent(Id, CreditoId, diasVencido, InteresMora));
    }

    public void RegistrarPago(decimal monto, Guid pagoCobradoId)
    {
        MontoPagado += monto;
        if (MontoPagado >= MontoOriginal + InteresMora)
        {
            Estado = EstadoCuota.PAGADA;
            FechaPago = DateTime.UtcNow;
        }
        else if (MontoPagado > 0)
        {
            Estado = EstadoCuota.PAGADA_PARCIAL;
        }
        AddDomainEvent(new PagoRegistradoEvent(Id, CreditoId, monto, pagoCobradoId));
    }

    public bool EstaVencida => FechaVencimiento.Date < DateTime.UtcNow.Date && Estado != EstadoCuota.PAGADA;
    public int DiasVencido => EstaVencida ? (DateTime.UtcNow.Date - FechaVencimiento.Date).Days : 0;
}
