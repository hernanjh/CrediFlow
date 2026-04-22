using CrediFlow.Domain.Common;
using CrediFlow.Domain.Enums;

namespace CrediFlow.Domain.Entities;

/// <summary>
/// Venta de contado en el POS (caja).
/// </summary>
public class Venta : BaseEntity
{
    public string? NumeroComprobante { get; set; }
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
    public Guid VendedorId { get; set; }
    public Usuario? Vendedor { get; set; }
    public Guid? ClienteId { get; set; }
    public Cliente? Cliente { get; set; }
    public decimal SubTotal { get; set; }
    public decimal Descuento { get; set; }
    public decimal Total { get; set; }
    public Guid? FormaCobroId { get; set; }
    public FormaCobro? FormaCobro { get; set; }
    public bool EsContado { get; set; } = true;
    public Guid? ListaPrecioId { get; set; }
    public string? Observaciones { get; set; }

    private readonly List<VentaItem> _items = new();
    public IReadOnlyCollection<VentaItem> Items => _items.AsReadOnly();

    public void AgregarItem(Guid productoId, int cantidad, decimal precioUnitario, decimal? precioListaUnitario = null)
    {
        _items.Add(new VentaItem
        {
            VentaId = Id,
            ProductoId = productoId,
            Cantidad = cantidad,
            PrecioBase = precioListaUnitario ?? precioUnitario,
            PrecioUnitario = precioUnitario
        });
        SubTotal = _items.Sum(i => i.PrecioBase * i.Cantidad);
        Total = _items.Sum(i => i.Subtotal) - Descuento;
    }
}

public class VentaItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid VentaId { get; set; }
    public Venta? Venta { get; set; }
    public Guid ProductoId { get; set; }
    public Producto? Producto { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioBase { get; set; }      // Precio original del producto
    public decimal PrecioUnitario { get; set; }  // Precio aplicado (puede ser de lista)
    public decimal Subtotal => Cantidad * PrecioUnitario;
}

/// <summary>
/// Lista de precios con soporte para ajuste global, por categoría o por producto.
/// </summary>
public class ListaPrecio : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public TipoListaPrecio Tipo { get; set; } = TipoListaPrecio.GLOBAL;

    /// <summary>Porcentaje de ajuste global sobre precio base. Ej: 20 = 20% de recargo. Negativo = descuento.</summary>
    public decimal PorcentajeAjuste { get; set; }

    public bool Activa { get; private set; } = true;
    public string? Color { get; set; } // para identificación visual

    public void Activar() => Activa = true;
    public void Desactivar() => Activa = false;

    private readonly List<ListaPrecioItem> _items = new();
    public IReadOnlyCollection<ListaPrecioItem> Items => _items.AsReadOnly();

    /// <summary>
    /// Calcula el precio final para un producto dado, considerando overrides por producto o categoría.
    /// </summary>
    public decimal CalcularPrecio(decimal precioBase, Guid productoId, Guid? categoriaId = null)
    {
        // 1. Buscar override por producto específico (tiene prioridad máxima)
        var itemProducto = _items.FirstOrDefault(i => i.ProductoId == productoId);
        if (itemProducto != null)
        {
            return itemProducto.PrecioFijo ?? precioBase * (1 + (itemProducto.PorcentajeOverride ?? PorcentajeAjuste) / 100m);
        }

        // 2. Buscar override por categoría
        if (categoriaId.HasValue)
        {
            var itemCat = _items.FirstOrDefault(i => i.CategoriaId == categoriaId.Value && i.ProductoId == null);
            if (itemCat != null)
            {
                return precioBase * (1 + (itemCat.PorcentajeOverride ?? PorcentajeAjuste) / 100m);
            }
        }

        // 3. Ajuste global
        return precioBase * (1 + PorcentajeAjuste / 100m);
    }
}

/// <summary>
/// Override de precio para un producto o categoría específica dentro de una lista.
/// Si PrecioFijo tiene valor, se usa ese precio directamente (ignorando porcentaje).
/// Si solo PorcentajeOverride, se aplica ese % en lugar del global.
/// </summary>
public class ListaPrecioItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ListaPrecioId { get; set; }
    public ListaPrecio? ListaPrecio { get; set; }

    /// <summary>Si se aplica a un producto específico (null = aplica a categoría o es global)</summary>
    public Guid? ProductoId { get; set; }
    public Producto? Producto { get; set; }

    /// <summary>Si se aplica a todos los productos de una categoría</summary>
    public Guid? CategoriaId { get; set; }
    public CategoriaProducto? Categoria { get; set; }

    /// <summary>Precio fijo (si está definido, ignora el porcentaje)</summary>
    public decimal? PrecioFijo { get; set; }

    /// <summary>Porcentaje de ajuste sobre el precio base (override del global)</summary>
    public decimal? PorcentajeOverride { get; set; }
}

public class FormaCobroVenta
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid VentaId { get; set; }
    public Venta? Venta { get; set; }
    public Guid FormaCobroId { get; set; }
    public FormaCobro? FormaCobro { get; set; }
    public decimal Monto { get; set; }
}
