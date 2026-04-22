using CrediFlow.Application.Common;
using CrediFlow.Domain.Entities;
using CrediFlow.Domain.Interfaces;
using CrediFlow.Domain.Enums;
using MediatR;

namespace CrediFlow.Application.Features.Configuracion;

// Handlers
public record GetListasPreciosQuery() : IRequest<Result<IEnumerable<ListaPrecio>>>;
public record CrearListaPrecioCommand(string Nombre, decimal PorcentajeAjuste, TipoListaPrecio Tipo = TipoListaPrecio.GLOBAL) : IRequest<Result<Guid>>;
public record ActualizarListaPrecioCommand(Guid Id, string Nombre, decimal PorcentajeAjuste, TipoListaPrecio Tipo) : IRequest<Result>;
public record ReactivarListaPrecioCommand(Guid Id) : IRequest<Result>;

public record AgregarItemListaPrecioCommand(
    Guid ListaPrecioId, 
    Guid? ProductoId, 
    Guid? CategoriaId, 
    decimal? PrecioFijo, 
    decimal? PorcentajeOverride) : IRequest<Result<Guid>>;

public record EliminarItemListaPrecioCommand(Guid Id) : IRequest<Result>;

public class ListaPreciosHandlers : 
    IRequestHandler<GetListasPreciosQuery, Result<IEnumerable<ListaPrecio>>>,
    IRequestHandler<CrearListaPrecioCommand, Result<Guid>>,
    IRequestHandler<ActualizarListaPrecioCommand, Result>,
    IRequestHandler<ReactivarListaPrecioCommand, Result>,
    IRequestHandler<AgregarItemListaPrecioCommand, Result<Guid>>,
    IRequestHandler<EliminarItemListaPrecioCommand, Result>
{
    private readonly IUnitOfWork _uow;
    public ListaPreciosHandlers(IUnitOfWork uow) => _uow = uow;

    public async Task<Result> Handle(ReactivarListaPrecioCommand req, CancellationToken ct)
    {
        var lista = await _uow.ListasPrecios.GetByIdAsync(req.Id); // Assuming generic GetById or specialized
        if (lista == null) return Result.Failure("Lista no encontrada");
        lista.Activar();
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<Result<IEnumerable<ListaPrecio>>> Handle(GetListasPreciosQuery request, CancellationToken ct)
    {
        var listas = await _uow.ListasPrecios.GetAllAsync(ct);
        return Result<IEnumerable<ListaPrecio>>.Success(listas);
    }

    public async Task<Result<Guid>> Handle(CrearListaPrecioCommand req, CancellationToken ct)
    {
        var lista = new ListaPrecio { Nombre = req.Nombre, PorcentajeAjuste = req.PorcentajeAjuste, Tipo = req.Tipo };
        await _uow.ListasPrecios.AddAsync(lista, ct);
        await _uow.SaveChangesAsync(ct);
        return Result<Guid>.Success(lista.Id);
    }

    public async Task<Result> Handle(ActualizarListaPrecioCommand req, CancellationToken ct)
    {
        var lista = await _uow.ListasPrecios.GetByIdAsync(req.Id);
        if (lista == null) return Result.Failure("Lista no encontrada");
        
        lista.Nombre = req.Nombre;
        lista.PorcentajeAjuste = req.PorcentajeAjuste;
        lista.Tipo = req.Tipo;
        
        _uow.ListasPrecios.Update(lista);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<Result<Guid>> Handle(AgregarItemListaPrecioCommand req, CancellationToken ct)
    {
        var lista = await _uow.ListasPrecios.GetByIdAsync(req.ListaPrecioId);
        if (lista == null) return Result<Guid>.Failure("Lista no encontrada");
        if (!req.PrecioFijo.HasValue && !req.PorcentajeOverride.HasValue)
            return Result<Guid>.Failure("Debe indicar un precio fijo o un porcentaje de ajuste.");

        var item = new ListaPrecioItem
        {
            ListaPrecioId = req.ListaPrecioId,
            ProductoId = req.ProductoId,
            CategoriaId = req.CategoriaId,
            PrecioFijo = req.PrecioFijo,
            PorcentajeOverride = req.PorcentajeOverride
        };

        await _uow.ListasPrecios.AddItemAsync(item, ct);
        await _uow.SaveChangesAsync(ct);
        return Result<Guid>.Success(item.Id);
    }

    public async Task<Result> Handle(EliminarItemListaPrecioCommand req, CancellationToken ct)
    {
        var item = await _uow.ListasPrecios.GetItemByIdAsync(req.Id, ct);
        if (item == null) return Result.Failure("Item de lista no encontrado");

        _uow.ListasPrecios.RemoveItem(item);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }
}
