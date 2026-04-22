using CrediFlow.Domain.Common;
using CrediFlow.Domain.Enums;

namespace CrediFlow.Domain.Events;

public class CreditoOtorgadoEvent : DomainEventBase
{
    public Guid CreditoId { get; }
    public Guid ClienteId { get; }
    public Guid VendedorId { get; }
    public decimal Capital { get; }
    public int NumeroCuotas { get; }
    public FrecuenciaCuota Frecuencia { get; }
    public DateTime FechaPrimerVencimiento { get; }
    public decimal MontoPorCuota { get; }

    public CreditoOtorgadoEvent(Guid creditoId, Guid clienteId, Guid vendedorId,
        decimal capital, int numeroCuotas, FrecuenciaCuota frecuencia,
        DateTime fechaPrimerVencimiento, decimal montoPorCuota)
    {
        CreditoId = creditoId;
        ClienteId = clienteId;
        VendedorId = vendedorId;
        Capital = capital;
        NumeroCuotas = numeroCuotas;
        Frecuencia = frecuencia;
        FechaPrimerVencimiento = fechaPrimerVencimiento;
        MontoPorCuota = montoPorCuota;
    }
}

public class CuotaVencidaEvent : DomainEventBase
{
    public Guid CuotaId { get; }
    public Guid CreditoId { get; }
    public int DiasVencido { get; }
    public decimal InteresMora { get; }

    public CuotaVencidaEvent(Guid cuotaId, Guid creditoId, int diasVencido, decimal interesMora)
    {
        CuotaId = cuotaId;
        CreditoId = creditoId;
        DiasVencido = diasVencido;
        InteresMora = interesMora;
    }
}

public class PagoRegistradoEvent : DomainEventBase
{
    public Guid CuotaId { get; }
    public Guid CreditoId { get; }
    public decimal Monto { get; }
    public Guid PagoCobradoId { get; }

    public PagoRegistradoEvent(Guid cuotaId, Guid creditoId, decimal monto, Guid pagoCobradoId)
    {
        CuotaId = cuotaId;
        CreditoId = creditoId;
        Monto = monto;
        PagoCobradoId = pagoCobradoId;
    }
}

public class CierreCajaConDiferenciaEvent : DomainEventBase
{
    public Guid CierreCajaId { get; }
    public Guid VendedorId { get; }
    public DateTime Fecha { get; }
    public decimal Diferencia { get; }
    public decimal MontoSistema { get; }

    public CierreCajaConDiferenciaEvent(Guid cierreCajaId, Guid vendedorId,
        DateTime fecha, decimal diferencia, decimal montoSistema)
    {
        CierreCajaId = cierreCajaId;
        VendedorId = vendedorId;
        Fecha = fecha;
        Diferencia = diferencia;
        MontoSistema = montoSistema;
    }
}

public class CreditoPasadoAIncobrableEvent : DomainEventBase
{
    public Guid CreditoId { get; }
    public Guid ClienteId { get; }
    public decimal SaldoPerdido { get; }

    public CreditoPasadoAIncobrableEvent(Guid creditoId, Guid clienteId, decimal saldoPerdido)
    {
        CreditoId = creditoId;
        ClienteId = clienteId;
        SaldoPerdido = saldoPerdido;
    }
}
