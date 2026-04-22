using CrediFlow.Application.Common;
using CrediFlow.Application.DTOs;
using CrediFlow.Domain.Interfaces;
using MediatR;

namespace CrediFlow.Application.Features.Reportes;

public record GetDeudoresReporteQuery : IRequest<Result<IEnumerable<DeudorReporteDto>>>;

public class GetDeudoresReporteHandler : IRequestHandler<GetDeudoresReporteQuery, Result<IEnumerable<DeudorReporteDto>>>
{
    private readonly IUnitOfWork _uow;

    public GetDeudoresReporteHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<IEnumerable<DeudorReporteDto>>> Handle(GetDeudoresReporteQuery request, CancellationToken ct)
    {
        var creditosConMora = await _uow.Creditos.GetConMoraAsync(ct);

        var deudores = creditosConMora
            .GroupBy(c => c.ClienteId)
            .Select(g =>
            {
                var creditoRef = g.First();
                var deuda = g.Sum(x => x.SaldoPendiente);
                var maxDiasMora = g
                    .SelectMany(x => x.Cuotas)
                    .Where(cuota => cuota.EstaVencida && cuota.Estado != Domain.Enums.EstadoCuota.PAGADA)
                    .Select(cuota => cuota.DiasVencido)
                    .DefaultIfEmpty(0)
                    .Max();

                return new DeudorReporteDto(
                    g.Key,
                    creditoRef.Cliente?.NombreCompleto ?? "Cliente sin nombre",
                    creditoRef.Cliente?.Zona?.Nombre ?? "Sin zona",
                    deuda,
                    $"{maxDiasMora} días");
            })
            .OrderByDescending(x => x.Deuda)
            .ToList();

        return Result<IEnumerable<DeudorReporteDto>>.Success(deudores);
    }
}
