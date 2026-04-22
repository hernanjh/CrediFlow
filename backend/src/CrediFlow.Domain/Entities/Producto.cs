using CrediFlow.Domain.Common;
using CrediFlow.Domain.Enums;

namespace CrediFlow.Domain.Entities;

public class Producto : BaseEntity
{
    // ─── Identificación ─────────────────────────────────────────────────
    public string Nombre { get; private set; } = string.Empty;
    public string? CodigoInterno { get; private set; }
    public string? SKU { get; private set; } // Código de barras / EAN
    public string? Descripcion { get; private set; }
    public string? Foto { get; private set; }

    // ─── Precios y Costos ───────────────────────────────────────────────
    public decimal CostoUnitario { get; private set; }
    public decimal PrecioVenta { get; private set; }        // Precio base (lista pública)
    public decimal? PrecioVentaMayorista { get; private set; }

    // ─── Stock ──────────────────────────────────────────────────────────
    public int StockActual { get; private set; }
    public int StockMinimo { get; private set; } = 5;
    public int? StockMaximo { get; private set; }
    public Guid? UnidadMedidaId { get; private set; }
    public UnidadMedida? UnidadMedida { get; private set; }

    // ─── Relaciones ─────────────────────────────────────────────────────
    public Guid? ProveedorId { get; private set; }
    public Proveedor? Proveedor { get; private set; }
    public Guid? CategoriaId { get; private set; }
    public CategoriaProducto? Categoria { get; private set; }

    // ─── Estado ─────────────────────────────────────────────────────────
    public bool Activo { get; private set; } = true;
    public bool PermiteVentaSinStock { get; private set; } = false;

    private readonly List<MovimientoStock> _movimientos = new();
    public IReadOnlyCollection<MovimientoStock> Movimientos => _movimientos.AsReadOnly();

    protected Producto() { }

    public static Producto Crear(
        string nombre, decimal costoUnitario, decimal precioVenta,
        int stockInicial = 0, int stockMinimo = 5, string? descripcion = null,
        string? foto = null, Guid? proveedorId = null, Guid? categoriaId = null,
        string? codigoInterno = null, string? sku = null,
        decimal? precioVentaMayorista = null, int? stockMaximo = null,
        Guid? unidadMedidaId = null, bool permiteVentaSinStock = false)
    {
        if (string.IsNullOrWhiteSpace(nombre)) throw new ArgumentException("El nombre del producto es requerido.");
        if (costoUnitario < 0) throw new ArgumentException("El costo no puede ser negativo.");
        if (precioVenta <= 0) throw new ArgumentException("El precio de venta debe ser mayor a cero.");

        return new Producto
        {
            Nombre = nombre.Trim(),
            CodigoInterno = codigoInterno?.Trim(),
            SKU = sku?.Trim(),
            Descripcion = descripcion?.Trim(),
            Foto = foto,
            CostoUnitario = costoUnitario,
            PrecioVenta = precioVenta,
            PrecioVentaMayorista = precioVentaMayorista,
            StockActual = stockInicial,
            StockMinimo = stockMinimo,
            StockMaximo = stockMaximo,
            UnidadMedidaId = unidadMedidaId,
            ProveedorId = proveedorId,
            CategoriaId = categoriaId,
            PermiteVentaSinStock = permiteVentaSinStock
        };
    }

    public void AjustarStock(int cantidad, TipoMovimientoStock tipo, string? motivo = null)
    {
        StockActual += tipo switch
        {
            TipoMovimientoStock.COMPRA or TipoMovimientoStock.AJUSTE_POSITIVO or TipoMovimientoStock.DEVOLUCION => cantidad,
            TipoMovimientoStock.VENTA or TipoMovimientoStock.AJUSTE_NEGATIVO => -cantidad,
            _ => 0
        };
        if (StockActual < 0 && !PermiteVentaSinStock)
            throw new InvalidOperationException("Stock insuficiente.");
    }

    public void ActualizarPrecios(decimal nuevoCosto, decimal nuevoPrecio, decimal? mayorista = null)
    {
        CostoUnitario = nuevoCosto;
        PrecioVenta = nuevoPrecio;
        PrecioVentaMayorista = mayorista;
    }

    public void Actualizar(
        string nombre, string? descripcion, decimal costoUnitario, decimal precioVenta,
        int stockMinimo, Guid? categoriaId, Guid? proveedorId, string? foto,
        string? codigoInterno, string? sku, decimal? precioVentaMayorista = null,
        int? stockMaximo = null, Guid? unidadMedidaId = null, bool permiteVentaSinStock = false)
    {
        Nombre = nombre.Trim();
        Descripcion = descripcion?.Trim();
        CostoUnitario = costoUnitario;
        PrecioVenta = precioVenta;
        PrecioVentaMayorista = precioVentaMayorista;
        StockMinimo = stockMinimo;
        StockMaximo = stockMaximo;
        CategoriaId = categoriaId;
        ProveedorId = proveedorId;
        CodigoInterno = codigoInterno?.Trim();
        SKU = sku?.Trim();
        UnidadMedidaId = unidadMedidaId;
        PermiteVentaSinStock = permiteVentaSinStock;
        if (foto != null) Foto = foto;
    }

    public void Desactivar() => Activo = false;
    public void Activar() => Activo = true;

    public decimal MargenGanancia => PrecioVenta > 0
        ? Math.Round((PrecioVenta - CostoUnitario) / PrecioVenta * 100, 2)
        : 0;

    public bool BajoStock => StockActual <= StockMinimo;
}

public class MovimientoStock : BaseEntity
{
    public Guid ProductoId { get; set; }
    public Producto? Producto { get; set; }
    public TipoMovimientoStock Tipo { get; set; }
    public int Cantidad { get; set; }
    public decimal CostoUnitario { get; set; }
    public decimal Total => Cantidad * CostoUnitario;
    public string? Motivo { get; set; }
    public Guid? ProveedorId { get; set; }
    public Proveedor? Proveedor { get; set; }
    public string? NumeroFactura { get; set; }
    public Guid? UsuarioId { get; set; }
}

public class Proveedor : BaseEntity
{
    // ─── Identificación ─────────────────────────────────────────────────
    public string RazonSocial { get; private set; } = string.Empty;
    public string? NombreFantasia { get; private set; }
    public string? CUIT { get; private set; }

    // ─── Contacto ───────────────────────────────────────────────────────
    public string? ContactoPrincipal { get; private set; }
    public string? Telefono { get; private set; }
    public string? Celular { get; private set; }
    public string? Email { get; private set; }
    public string? Web { get; private set; }

    // ─── Dirección ──────────────────────────────────────────────────────
    public string? Direccion { get; private set; }
    public string? Localidad { get; private set; }
    public string? Provincia { get; private set; }

    // ─── Comercial ──────────────────────────────────────────────────────
    public string? CondicionPago { get; private set; } // Contado, 30 días, etc.
    public string? CondicionIVA { get; private set; }  // Responsable Inscripto, Monotributo, etc.
    public string? Notas { get; private set; }
    public bool Activo { get; private set; } = true;

    protected Proveedor() { }

    public static Proveedor Crear(
        string razonSocial, string? cuit, string? telefono, string? email, string? direccion,
        string? nombreFantasia = null, string? contactoPrincipal = null, string? celular = null,
        string? web = null, string? localidad = null, string? provincia = null,
        string? condicionPago = null, string? condicionIVA = null, string? notas = null)
    {
        if (string.IsNullOrWhiteSpace(razonSocial)) throw new ArgumentException("Razón social requerida.");
        return new Proveedor
        {
            RazonSocial = razonSocial.Trim(),
            NombreFantasia = nombreFantasia?.Trim(),
            CUIT = cuit?.Trim(),
            ContactoPrincipal = contactoPrincipal?.Trim(),
            Telefono = telefono?.Trim(),
            Celular = celular?.Trim(),
            Email = email?.ToLower().Trim(),
            Web = web?.Trim(),
            Direccion = direccion?.Trim(),
            Localidad = localidad?.Trim(),
            Provincia = provincia?.Trim(),
            CondicionPago = condicionPago?.Trim(),
            CondicionIVA = condicionIVA?.Trim(),
            Notas = notas?.Trim()
        };
    }

    public void Actualizar(
        string razonSocial, string? cuit, string? telefono, string? email, string? direccion,
        string? nombreFantasia = null, string? contactoPrincipal = null, string? celular = null,
        string? web = null, string? localidad = null, string? provincia = null,
        string? condicionPago = null, string? condicionIVA = null, string? notas = null)
    {
        RazonSocial = razonSocial.Trim();
        NombreFantasia = nombreFantasia?.Trim();
        CUIT = cuit?.Trim();
        ContactoPrincipal = contactoPrincipal?.Trim();
        Telefono = telefono?.Trim();
        Celular = celular?.Trim();
        Email = email?.ToLower().Trim();
        Web = web?.Trim();
        Direccion = direccion?.Trim();
        Localidad = localidad?.Trim();
        Provincia = provincia?.Trim();
        CondicionPago = condicionPago?.Trim();
        CondicionIVA = condicionIVA?.Trim();
        Notas = notas?.Trim();
    }

    public void Desactivar() => Activo = false;
    public void Activar() => Activo = true;
}


