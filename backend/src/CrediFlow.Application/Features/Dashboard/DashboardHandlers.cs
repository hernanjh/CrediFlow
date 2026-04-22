using CrediFlow.Application.Common;
using CrediFlow.Application.DTOs;
using CrediFlow.Domain.Enums;
using CrediFlow.Domain.Interfaces;
using MediatR;

namespace CrediFlow.Application.Features.Dashboard;

public record GetDashboardKpiQuery : IRequest<Result<DashboardKpiDto>>;
public record GetCobranzaTermometroQuery : IRequest<Result<CobranzaTermometroDto>>;
public record GetEvolucionMensualQuery(int Meses = 6) : IRequest<Result<IEnumerable<EvolucionMensualDto>>>;
public record GetDistribucionCarteraQuery : IRequest<Result<DistribucionCarteraDto>>;
public record GetRankingVendedoresQuery(DateTime? Desde = null, DateTime? Hasta = null) : IRequest<Result<IEnumerable<RankingVendedorDto>>>;
public record GetAgingDeudaQuery : IRequest<Result<AgingDeudaDto>>;
public record GetFlujoCajaProyectadoQuery : IRequest<Result<IEnumerable<FlujoCajaSemanaDto>>>;
public record GetMapaMoraQuery : IRequest<Result<IEnumerable<MapaMoraZonaDto>>>;
public record GetAlertasQuery : IRequest<Result<IEnumerable<AlertaDto>>>;

public class GetDashboardKpiHandler : IRequestHandler<GetDashboardKpiQuery, Result<DashboardKpiDto>>
{
    private readonly IUnitOfWork _uow;

    public GetDashboardKpiHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<DashboardKpiDto>> Handle(GetDashboardKpiQuery request, CancellationToken ct)
    {
        var capitalActivo = await _uow.Creditos.GetCapitalActivoTotalAsync(ct);
        var clientes = await _uow.Clientes.GetAllAsync(ct);
        var cuotasVencidas = await _uow.Cuotas.GetVencidasAsync(ct);
        var vencidasList = cuotasVencidas.ToList();

        var clientesActivos = clientes.Count(c => c.Estado == EstadoCliente.ACTIVO);
        var montoVencido = vencidasList.Sum(c => c.SaldoPendiente);
        var indiceMora = capitalActivo > 0
            ? Math.Round(montoVencido / capitalActivo * 100, 2)
            : 0;

        var dto = new DashboardKpiDto(
            CapitalActivo: capitalActivo,
            CobradoHoy: 0, // se calcula en otro endpoint
            MetaCobradoHoy: 0,
            ClientesActivos: clientesActivos,
            IndiceMora: indiceMora,
            CuotasVencidas: vencidasList.Count,
            MontoVencido: montoVencido);

        return Result<DashboardKpiDto>.Success(dto);
    }
}

public class GetAgingDeudaHandler : IRequestHandler<GetAgingDeudaQuery, Result<AgingDeudaDto>>
{
    private readonly IUnitOfWork _uow;

    public GetAgingDeudaHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<AgingDeudaDto>> Handle(GetAgingDeudaQuery request, CancellationToken ct)
    {
        var cuotasVencidas = await _uow.Cuotas.GetVencidasAsync(ct);
        var lista = cuotasVencidas.ToList();

        decimal t1 = 0, t2 = 0, t3 = 0, t4 = 0;
        int c1 = 0, c2 = 0, c3 = 0, c4 = 0;

        foreach (var c in lista)
        {
            var dias = c.DiasVencido;
            var saldo = c.SaldoPendiente;

            if (dias <= 7) { t1 += saldo; c1++; }
            else if (dias <= 30) { t2 += saldo; c2++; }
            else if (dias <= 90) { t3 += saldo; c3++; }
            else { t4 += saldo; c4++; }
        }

        var dto = new AgingDeudaDto(t1, t2, t3, t4, t1 + t2 + t3 + t4, c1, c2, c3, c4);
        return Result<AgingDeudaDto>.Success(dto);
    }
}

public class GetDistribucionCarteraHandler : IRequestHandler<GetDistribucionCarteraQuery, Result<DistribucionCarteraDto>>
{
    private readonly IUnitOfWork _uow;

    public GetDistribucionCarteraHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<DistribucionCarteraDto>> Handle(
        GetDistribucionCarteraQuery request, CancellationToken ct)
    {
        var clientes = (await _uow.Clientes.GetAllAsync(ct)).ToList();
        var total = clientes.Count;
        if (total == 0)
            return Result<DistribucionCarteraDto>.Success(new DistribucionCarteraDto(0, 0, 0, 0, 0, 0, 0, 0, 0));

        // Esta lógica se basa en el estado del crédito activo del cliente
        var creditos = await _uow.Creditos.GetActivosAsync(ct);
        var creditosDict = creditos.GroupBy(c => c.ClienteId)
            .ToDictionary(g => g.Key, g => g.Max(c => (int)c.Estado));

        int alDia = 0, moraLeve = 0, moraGrave = 0, incobrables = 0;
        foreach (var c in clientes)
        {
            if (!creditosDict.TryGetValue(c.Id, out var estado))
            { alDia++; continue; }

            switch ((EstadoCredito)estado)
            {
                case EstadoCredito.ACTIVO or EstadoCredito.AL_DIA: alDia++; break;
                case EstadoCredito.EN_MORA: moraLeve++; break;
                case EstadoCredito.EN_MORA_GRAVE: moraGrave++; break;
                case EstadoCredito.INCOBRABLE: incobrables++; break;
                default: alDia++; break;
            }
        }

        var dto = new DistribucionCarteraDto(
            alDia, moraLeve, moraGrave, incobrables, total,
            Math.Round((decimal)alDia / total * 100, 1),
            Math.Round((decimal)moraLeve / total * 100, 1),
            Math.Round((decimal)moraGrave / total * 100, 1),
            Math.Round((decimal)incobrables / total * 100, 1));

        return Result<DistribucionCarteraDto>.Success(dto);
    }
}

public class GetAlertasHandler : IRequestHandler<GetAlertasQuery, Result<IEnumerable<AlertaDto>>>
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public GetAlertasHandler(IUnitOfWork uow, ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<Result<IEnumerable<AlertaDto>>> Handle(GetAlertasQuery request, CancellationToken ct)
    {
        var alertas = await _uow.Alertas.GetNoLeidasAsync(_currentUser.UserId, ct);
        var dtos = alertas
            .Where(a => a.FechaExpiracion > DateTime.UtcNow)
            .Select(a => new AlertaDto(a.Id, a.Tipo.ToString(), a.Mensaje, a.Leida, a.CreatedAt));
        return Result<IEnumerable<AlertaDto>>.Success(dtos);
    }
}
