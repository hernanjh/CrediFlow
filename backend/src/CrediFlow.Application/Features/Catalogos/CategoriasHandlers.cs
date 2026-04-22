using CrediFlow.Application.Common;
using CrediFlow.Application.DTOs;
using CrediFlow.Domain.Entities;
using CrediFlow.Domain.Interfaces;
using MediatR;

namespace CrediFlow.Application.Features.Catalogos;

public record GetCategoriasQuery(bool SoloActivas = true) : IRequest<Result<IEnumerable<CategoriaProductoDto>>>;

public class GetCategoriasHandler : IRequestHandler<GetCategoriasQuery, Result<IEnumerable<CategoriaProductoDto>>>
{
    private readonly IUnitOfWork _uow;
    public GetCategoriasHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<IEnumerable<CategoriaProductoDto>>> Handle(GetCategoriasQuery request, CancellationToken ct)
    {
        var cats = await _uow.CategoriasProducto.GetAllAsync(ct);
        if (request.SoloActivas) cats = cats.Where(c => c.Activa);

        var dtos = cats.Select(c => new CategoriaProductoDto(
            c.Id, c.Nombre, c.Descripcion, c.ColorHex, c.Activa, 0
        ));
        return Result<IEnumerable<CategoriaProductoDto>>.Success(dtos);
    }
}

public record CrearCategoriaCommand(string Nombre, string? Descripcion, string? ColorHex) : IRequest<Result<Guid>>;

public class CrearCategoriaHandler : IRequestHandler<CrearCategoriaCommand, Result<Guid>>
{
    private readonly IUnitOfWork _uow;
    public CrearCategoriaHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<Guid>> Handle(CrearCategoriaCommand req, CancellationToken ct)
    {
        var c = new CategoriaProducto { Nombre = req.Nombre, Descripcion = req.Descripcion, ColorHex = req.ColorHex };
        await _uow.CategoriasProducto.AddAsync(c, ct);
        await _uow.SaveChangesAsync(ct);
        return Result<Guid>.Success(c.Id);
    }
}
