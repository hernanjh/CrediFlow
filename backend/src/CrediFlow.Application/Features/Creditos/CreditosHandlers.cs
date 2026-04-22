using CrediFlow.Application.Common;
using CrediFlow.Application.DTOs;
using CrediFlow.Domain.Entities;
using CrediFlow.Domain.Enums;
using CrediFlow.Domain.Interfaces;
using MediatR;

namespace CrediFlow.Application.Features.Creditos;

// ─── COMMANDS ───────────────────────────────────────────────────────────────
public record CrearCreditoCommand(
    Guid ClienteId, decimal Capital, decimal TasaInteres, decimal TasaMoratoria,
    int NumeroCuotas, FrecuenciaCuota Frecuencia,
    DateTime FechaPrimerVencimiento, string? Observaciones = null,
    Guid? ProductoId = null, Guid? VendedorId = null) : IRequest<Result<Guid>>;

public record RegistrarPagoCommand(
    Guid CuotaId, decimal Monto, double? Latitud = null, double? Longitud = null,
    string? Observaciones = null, bool EsOffline = false,
    string? DeviceId = null) : IRequest<Result<Guid>>;

public record SyncPagosOfflineCommand(
    List<PagoOfflineDto> Pagos) : IRequest<Result<SyncResultDto>>;

// ─── QUERIES ────────────────────────────────────────────────────────────────
public record GetCreditoByIdQuery(Guid Id) : IRequest<Result<CreditoDto>>;
public record GetCreditosByClienteQuery(Guid ClienteId) : IRequest<Result<IEnumerable<CreditoDto>>>;
public record GetHojaRutaQuery(Guid VendedorId, DateTime? Fecha = null) : IRequest<Result<IEnumerable<HojaRutaItemDto>>>;

// ─── HANDLERS ───────────────────────────────────────────────────────────────
public class CrearCreditoHandler : IRequestHandler<CrearCreditoCommand, Result<Guid>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMotorCuotasService _motor;
    private readonly ICurrentUserService _currentUser;

    public CrearCreditoHandler(IUnitOfWork uow, IMotorCuotasService motor, ICurrentUserService currentUser)
    {
        _uow = uow;
        _motor = motor;
        _currentUser = currentUser;
    }

    public async Task<Result<Guid>> Handle(CrearCreditoCommand request, CancellationToken ct)
    {
        var cliente = await _uow.Clientes.GetByIdAsync(request.ClienteId, ct);
        if (cliente is null) return Result<Guid>.Failure("Cliente no encontrado.");
        if (cliente.Estado == EstadoCliente.BLOQUEADO)
            return Result<Guid>.Failure("El cliente está bloqueado para nuevos créditos.");

        var vendedorId = request.VendedorId ?? _currentUser.UserId!.Value;

        // Generar tabla de cuotas usando el Motor de Cuotas
        var cuotasGeneradas = await _motor.GenerarCuotasAsync(
            request.Capital, request.TasaInteres, request.NumeroCuotas,
            request.Frecuencia, request.FechaPrimerVencimiento, ct);

        await _uow.BeginTransactionAsync(ct);
        try
        {
            var credito = Credito.Crear(
                request.ClienteId, vendedorId, request.Capital,
                request.TasaInteres, request.TasaMoratoria,
                request.NumeroCuotas, request.Frecuencia,
                request.FechaPrimerVencimiento, request.Observaciones, request.ProductoId);

            credito.SetAudit(vendedorId.ToString());
            await _uow.Creditos.AddAsync(credito, ct);
            await _uow.SaveChangesAsync(ct); // necesitamos el ID del crédito

            var cuotas = cuotasGeneradas.Select(c =>
                Cuota.Crear(credito.Id, c.Numero, c.Fecha, c.Monto)).ToList();

            credito.AgregarCuotas(cuotas);
            await _uow.Cuotas.AddRangeAsync(cuotas, ct);
            await _uow.SaveChangesAsync(ct);
            await _uow.CommitTransactionAsync(ct);

            return Result<Guid>.Success(credito.Id);
        }
        catch
        {
            await _uow.RollbackTransactionAsync(ct);
            throw;
        }
    }
}

public class RegistrarPagoHandler : IRequestHandler<RegistrarPagoCommand, Result<Guid>>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public RegistrarPagoHandler(IUnitOfWork uow, ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<Result<Guid>> Handle(RegistrarPagoCommand request, CancellationToken ct)
    {
        var cuota = await _uow.Cuotas.GetByIdAsync(request.CuotaId, ct);
        if (cuota is null) return Result<Guid>.Failure("Cuota no encontrada.");
        if (cuota.Estado == EstadoCuota.PAGADA) return Result<Guid>.Failure("La cuota ya está pagada.");
        if (request.Monto <= 0) return Result<Guid>.Failure("El monto debe ser mayor a cero.");

        await _uow.BeginTransactionAsync(ct);
        try
        {
            var pago = PagoCobrado.Crear(
                request.CuotaId, _currentUser.UserId!.Value, request.Monto,
                request.Latitud, request.Longitud, request.Observaciones,
                request.EsOffline, request.DeviceId);

            await _uow.Pagos.AddAsync(pago, ct);

            cuota.RegistrarPago(request.Monto, pago.Id);
            _uow.Cuotas.Update(cuota);

            // Actualizar saldo del crédito
            var credito = await _uow.Creditos.GetByIdWithCuotasAsync(cuota.CreditoId, ct);
            if (credito != null)
            {
                credito.RegistrarPago(request.Monto);
                _uow.Creditos.Update(credito);
            }

            await _uow.SaveChangesAsync(ct);
            await _uow.CommitTransactionAsync(ct);
            return Result<Guid>.Success(pago.Id);
        }
        catch
        {
            await _uow.RollbackTransactionAsync(ct);
            throw;
        }
    }
}

public class GetHojaRutaHandler : IRequestHandler<GetHojaRutaQuery, Result<IEnumerable<HojaRutaItemDto>>>
{
    private readonly IUnitOfWork _uow;

    public GetHojaRutaHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<IEnumerable<HojaRutaItemDto>>> Handle(
        GetHojaRutaQuery request, CancellationToken ct)
    {
        var fecha = request.Fecha ?? DateTime.UtcNow.Date;
        var cuotas = await _uow.Cuotas.GetHojaRutaVendedorAsync(request.VendedorId, fecha, ct);

        var items = cuotas.Select(c => new HojaRutaItemDto(
            c.Id, c.CreditoId,
            c.Credito?.ClienteId ?? Guid.Empty,
            c.Credito?.Cliente?.NombreCompleto ?? "",
            c.Credito?.Cliente?.Direccion ?? "",
            c.Credito?.Cliente?.Latitud,
            c.Credito?.Cliente?.Longitud,
            c.NumeroCuota, c.MontoOriginal, c.InteresMora,
            c.SaldoPendiente, c.Estado, c.EstaVencida, c.DiasVencido,
            c.FechaVencimiento,
            c.Credito?.Cliente?.Promesas
                .Where(p => p.Estado == EstadoPromesaPago.PENDIENTE)
                .Select(p => new PromesaPagoDto(p.Id, p.FechaPromesa, p.MontoPrometido, p.Estado))
                .FirstOrDefault()));

        return Result<IEnumerable<HojaRutaItemDto>>.Success(items);
    }
}

// Auxiliary DTOs
public record PagoOfflineDto(
    Guid CuotaId, decimal Monto, DateTime FechaPago, double? Latitud, double? Longitud,
    string DeviceId, string? Observaciones);

public record SyncResultDto(int TotalProcesados, int Exitosos, int Fallidos, List<string> Errores);
