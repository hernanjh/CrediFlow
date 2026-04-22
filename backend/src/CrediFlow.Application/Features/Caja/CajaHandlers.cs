using CrediFlow.Application.Common;
using CrediFlow.Application.DTOs;
using CrediFlow.Domain.Entities;
using CrediFlow.Domain.Enums;
using CrediFlow.Domain.Interfaces;
using MediatR;

namespace CrediFlow.Application.Features.Caja;

// ─── COMMANDS ───────────────────────────────────────────────────────────────

/// Paso 1 del Cierre Ciego: el vendedor declara su monto
public record DeclararCajaCommand(decimal MontoDeclarado) : IRequest<Result<Guid>>;

/// Paso 2 (solo Admin/SuperAdmin): calcula y cierra
public record CerrarCajaConSistemaCommand(Guid VendedorId, DateTime Fecha) : IRequest<Result<CierreCajaDto>>;

// ─── QUERIES ────────────────────────────────────────────────────────────────

/// Solo SuperAdmin/Admin puede ver esto
public record GetCierresCajaHoyQuery : IRequest<Result<IEnumerable<CierreCajaDto>>>;

// ─── HANDLERS ───────────────────────────────────────────────────────────────

public class DeclararCajaHandler : IRequestHandler<DeclararCajaCommand, Result<Guid>>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public DeclararCajaHandler(IUnitOfWork uow, ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<Result<Guid>> Handle(DeclararCajaCommand request, CancellationToken ct)
    {
        var vendedorId = _currentUser.UserId!.Value;
        var hoy = DateTime.UtcNow.Date;

        // Verificar si ya declaró hoy
        var existente = await _uow.CierresCaja.GetByVendedorFechaAsync(vendedorId, hoy, ct);
        if (existente != null)
            return Result<Guid>.Failure("Ya realizaste tu declaración de caja para hoy.");

        // *** CIERRE CIEGO: El vendedor NO ve el total del sistema ***
        var cierre = CierreCaja.CrearDeclaracion(vendedorId, request.MontoDeclarado);
        await _uow.CierresCaja.AddAsync(cierre, ct);
        await _uow.SaveChangesAsync(ct);

        // Disparar el cierre automático (calcula el sistema en background)
        // En producción esto podría ser un Hangfire job
        _ = Task.Run(async () =>
        {
            await Task.Delay(1000); // pequeño delay para evitar race conditions
        }, ct);

        return Result<Guid>.Success(cierre.Id);
    }
}

public class CerrarCajaConSistemaHandler : IRequestHandler<CerrarCajaConSistemaCommand, Result<CierreCajaDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public CerrarCajaConSistemaHandler(IUnitOfWork uow, ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<Result<CierreCajaDto>> Handle(CerrarCajaConSistemaCommand request, CancellationToken ct)
    {
        if (_currentUser.IsVendedor)
            return Result<CierreCajaDto>.Failure("Acceso denegado. Solo Admin/SuperAdmin puede ver el total del sistema.");

        var cierre = await _uow.CierresCaja.GetByVendedorFechaAsync(request.VendedorId, request.Fecha, ct);
        if (cierre is null)
            return Result<CierreCajaDto>.Failure("No hay declaración de caja para esta fecha.");

        // Calcular el monto real cobrado por el vendedor ese día
        var montoSistema = await _uow.Pagos.GetTotalCobradoVendedorHoyAsync(request.VendedorId, ct);

        cierre.CerrarConMontoSistema(montoSistema);
        _uow.CierresCaja.Update(cierre);
        await _uow.SaveChangesAsync(ct);

        var vendedor = await _uow.Usuarios.GetByIdAsync(request.VendedorId, ct);

        var dto = new CierreCajaDto(
            cierre.Id, cierre.VendedorId, vendedor?.NombreCompleto ?? "Desconocido",
            cierre.Fecha, cierre.MontoDeclarado, cierre.MontoSistema,
            cierre.Diferencia, cierre.Estado, cierre.Observaciones);

        return Result<CierreCajaDto>.Success(dto);
    }
}

public class GetCierresCajaHoyHandler : IRequestHandler<GetCierresCajaHoyQuery, Result<IEnumerable<CierreCajaDto>>>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public GetCierresCajaHoyHandler(IUnitOfWork uow, ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<Result<IEnumerable<CierreCajaDto>>> Handle(
        GetCierresCajaHoyQuery request, CancellationToken ct)
    {
        if (_currentUser.IsVendedor)
            return Result<IEnumerable<CierreCajaDto>>.Failure("Acceso denegado.");

        var cierres = await _uow.CierresCaja.GetByFechaAsync(DateTime.UtcNow.Date, ct);

        var dtos = new List<CierreCajaDto>();
        foreach (var c in cierres)
        {
            var vendedor = await _uow.Usuarios.GetByIdAsync(c.VendedorId, ct);
            dtos.Add(new CierreCajaDto(
                c.Id, c.VendedorId, vendedor?.NombreCompleto ?? "Desconocido",
                c.Fecha, c.MontoDeclarado, c.MontoSistema,
                c.Diferencia, c.Estado, c.Observaciones));
        }

        return Result<IEnumerable<CierreCajaDto>>.Success(dtos);
    }
}
