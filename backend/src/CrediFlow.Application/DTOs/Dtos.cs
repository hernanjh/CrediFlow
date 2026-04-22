using CrediFlow.Domain.Enums;

namespace CrediFlow.Application.DTOs;

// ─── CLIENTES ────────────────────────────────────────────────────────────────
public record ClienteDto(
    Guid Id, string NombreCompleto, string DNI, string? CUIL,
    DateTime? FechaNacimiento, string? Sexo, string? EstadoCivil,
    Guid? OcupacionId, string? OcupacionNombre,
    Guid? SectorId, string? SectorNombre,
    Guid? TipoClienteId, string? TipoClienteNombre,
    string Direccion,
    string? Barrio, string? Localidad, string? CodigoPostal, string? Provincia,
    string? Telefono, string? TelefonoAlternativo, string? Email,
    Guid ZonaId, string? ZonaNombre, Guid? VendedorAsignadoId,
    EstadoCliente Estado, double? Latitud, double? Longitud,
    decimal LimiteCredito, Guid? ListaPrecioId,
    string? FiadorNombre, string? FiadorDNI, string? FiadorTelefono, string? FiadorDireccion,
    string? FotoPerfil, string? FotoDNIFrente, string? FotoDNIDorso,
    string? Observaciones, DateTime FechaAlta, bool TieneDeudaActiva, int? Edad);

public record ClienteResumenDto(
    Guid Id, string NombreCompleto, string DNI, string? CUIL,
    Guid? TipoClienteId, string? TipoClienteNombre, EstadoCliente Estado,
    string? ZonaNombre, string? Telefono, string? Localidad, decimal LimiteCredito,
    bool TieneDeudaActiva);

// ─── CRÉDITOS ────────────────────────────────────────────────────────────────
public record CreditoDto(
    Guid Id, Guid ClienteId, string ClienteNombre, string ClienteDNI,
    Guid VendedorId, string VendedorNombre,
    decimal Capital, decimal TasaInteres, decimal TasaMoratoria, int NumeroCuotas,
    FrecuenciaCuota Frecuencia, decimal MontoTotalConInteres, decimal MontoPorCuota,
    decimal SaldoPendiente, DateTime FechaOtorgamiento, DateTime FechaPrimerVencimiento,
    EstadoCredito Estado, string? Observaciones,
    List<CuotaDto> Cuotas);

public record CreditoResumenDto(
    Guid Id, Guid ClienteId, string ClienteNombre,
    decimal Capital, decimal SaldoPendiente, decimal MontoPorCuota,
    int NumeroCuotas, FrecuenciaCuota Frecuencia,
    DateTime FechaOtorgamiento, EstadoCredito Estado,
    int CuotasVencidas, int CuotasPagadas);

public record CuotaDto(
    Guid Id, int NumeroCuota, DateTime FechaVencimiento, decimal MontoOriginal,
    decimal MontoPagado, decimal InteresMora, decimal SaldoPendiente,
    EstadoCuota Estado, bool EstaVencida, int DiasVencido, DateTime? FechaPago);

// ─── PAGOS ───────────────────────────────────────────────────────────────────
public record PagoCobradoDto(
    Guid Id, Guid CuotaId, Guid VendedorId, string VendedorNombre,
    decimal Monto, DateTime FechaPago, double? Latitud, double? Longitud,
    string? Observaciones, bool EsOffline);

// ─── CIERRE CAJA ─────────────────────────────────────────────────────────────
public record CierreCajaDto(
    Guid Id, Guid VendedorId, string VendedorNombre, DateTime Fecha,
    decimal MontoDeclarado, decimal MontoSistema, decimal Diferencia,
    EstadoCierreCaja Estado, string? Observaciones);

// ─── USUARIOS ────────────────────────────────────────────────────────────────
public record UsuarioDto(
    Guid Id, string NombreCompleto, string Email, TipoUsuario Rol,
    bool Activo, string? FotoPerfil, decimal PorcentajeComision,
    Guid? PerfilId, string? PerfilNombre);

public record LoginResponseDto(
    string AccessToken, string RefreshToken, DateTime Expiracion,
    UsuarioDto Usuario);

// ─── ZONA ─────────────────────────────────────────────────────────────────────
public record ZonaDto(
    Guid Id, string Nombre, string? Descripcion, string? Color, bool Activa,
    int CantidadClientes, decimal IndiceCobrabilidad);

// ─── CATEGORIA PRODUCTO ────────────────────────────────────────────────────────
public record CategoriaProductoDto(
    Guid Id, string Nombre, string? Descripcion, string? Color, bool Activa, int CantidadProductos);

// ─── PROVEEDOR ────────────────────────────────────────────────────────────────
public record ProveedorDto(
    Guid Id, string RazonSocial, string? NombreFantasia, string? CUIT,
    string? ContactoPrincipal, string? Telefono, string? Celular,
    string? Email, string? Web,
    string? Direccion, string? Localidad, string? Provincia,
    string? CondicionPago, string? CondicionIVA, string? Notas,
    bool Activo);

// ─── INVENTARIO ───────────────────────────────────────────────────────────────
public record ProductoDto(
    Guid Id, string Nombre, string? CodigoInterno, string? SKU,
    string? Descripcion, string? Foto,
    decimal CostoUnitario, decimal PrecioVenta, decimal? PrecioVentaMayorista,
    int StockActual, int StockMinimo, int? StockMaximo,
    Guid? UnidadMedidaId, string? UnidadMedidaNombre, bool BajoStock, decimal MargenGanancia,
    bool Activo, bool PermiteVentaSinStock,
    Guid? ProveedorId, string? ProveedorNombre,
    Guid? CategoriaId, string? CategoriaNombre);

// ─── DASHBOARD ────────────────────────────────────────────────────────────────
public record DashboardKpiDto(
    decimal CapitalActivo,
    decimal CobradoHoy,
    decimal MetaCobradoHoy,
    int ClientesActivos,
    decimal IndiceMora,
    int CuotasVencidas,
    decimal MontoVencido);

public record CobranzaTermometroDto(
    decimal MetaDia, decimal CobradoHoy, decimal PorcentajeAlcanzado,
    string Nivel);

public record EvolucionMensualDto(string Mes, decimal NuevasVentas, decimal CapitalRecuperado);

public record DistribucionCarteraDto(
    int AlDia, int MoraLeve, int MoraGrave, int Incobrables, int Total,
    decimal PctAlDia, decimal PctMoraLeve, decimal PctMoraGrave, decimal PctIncobrable);

public record RankingVendedorDto(
    Guid VendedorId, string Nombre, decimal MontoEsperado,
    decimal MontoCobrado, decimal EficienciaPct);

public record AgingDeudaDto(
    decimal Tramo1_7, decimal Tramo8_30, decimal Tramo31_90, decimal TramoPlusN90,
    decimal Total, int Cant1_7, int Cant8_30, int Cant31_90, int CantPlus90);

public record FlujoCajaSemanaDto(
    int Semana, DateTime Desde, DateTime Hasta,
    decimal MontoProyectado, decimal MontoRecuperado, int CantidadCuotas);

public record MapaMoraZonaDto(
    Guid ZonaId, string ZonaNombre, decimal IndicesMora,
    string Nivel, int CantidadEnMora, decimal MontoEnMora);

public record AlertaDto(
    Guid Id, string Tipo, string Mensaje, bool Leida, DateTime FechaCreacion);

// ─── COBRANZA / HOJA DE RUTA ─────────────────────────────────────────────────
public record HojaRutaItemDto(
    Guid CuotaId, Guid CreditoId, Guid ClienteId, string ClienteNombre,
    string ClienteDireccion, double? Latitud, double? Longitud,
    int NumeroCuota, decimal MontoOriginal, decimal InteresMora,
    decimal SaldoPendiente, EstadoCuota Estado, bool EstaVencida, int DiasVencido,
    DateTime FechaVencimiento, PromesaPagoDto? Promesa);

public record PromesaPagoDto(
    Guid Id, DateTime FechaPromesa, decimal MontoPrometido, EstadoPromesaPago Estado);

// ─── CONFIGURACIÓN (PARAMETRIZACIÓN) ──────────────────────────────────────────
public record ConfiguracionGlobalDto(
    string NombreEmpresa, string? CUIT, string? RazonSocial, string? Direccion,
    string? Telefono, string? EmailContacto, decimal TasaInteresDefecto,
    decimal TasaMoraDefecto, string MonedaSimbolo, string? LogoBase64,
    List<ListaPrecioDto> ListasPrecios);

// ─── SEGURIDAD (RBAC) ─────────────────────────────────────────────────────────
public record PermisoDto(Guid Id, string Nombre, string Descripcion, string Seccion, bool Activo);

public record PerfilDto(Guid Id, string Nombre, string? Descripcion, bool Activo, List<PermisoDto> Permisos);

// ─── LISTAS DE PRECIOS ─────────────────────────────────────────────────────────
public record ListaPrecioDto(
    Guid Id, string Nombre, string? Descripcion,
    string Tipo, decimal PorcentajeAjuste, bool Activa, string? Color,
    List<ListaPrecioItemDto> Items);

public record ListaPrecioItemDto(
    Guid Id, Guid ListaPrecioId,
    Guid? ProductoId, string? ProductoNombre,
    Guid? CategoriaId, string? CategoriaNombre,
    decimal? PrecioFijo, decimal? PorcentajeOverride);

// ─── VENTAS ───────────────────────────────────────────────────────────────────
public record VentaDto(
    Guid Id, string? NumeroComprobante, DateTime Fecha,
    Guid VendedorId, string VendedorNombre,
    Guid? ClienteId, string? ClienteNombre,
    decimal SubTotal, decimal Descuento, decimal Total,
    Guid? FormaCobroId, string? FormaCobroNombre, Guid? ListaPrecioId, string? ListaPrecioNombre,
    string? Observaciones,
    List<VentaItemDto> Items);

public record VentaItemDto(
    Guid ProductoId, string ProductoNombre,
    int Cantidad, decimal PrecioBase, decimal PrecioUnitario, decimal Subtotal);
