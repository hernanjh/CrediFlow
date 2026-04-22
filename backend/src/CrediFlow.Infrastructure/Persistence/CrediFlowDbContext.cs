using CrediFlow.Domain.Entities;
using CrediFlow.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace CrediFlow.Infrastructure.Persistence;

public class CrediFlowDbContext : DbContext
{
    public CrediFlowDbContext(DbContextOptions<CrediFlowDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Zona> Zonas => Set<Zona>();
    public DbSet<UsuarioZona> UsuarioZonas => Set<UsuarioZona>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Credito> Creditos => Set<Credito>();
    public DbSet<Cuota> Cuotas => Set<Cuota>();
    public DbSet<PagoCobrado> PagosCobrados => Set<PagoCobrado>();
    public DbSet<CierreCaja> CierresCaja => Set<CierreCaja>();
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<MovimientoStock> MovimientosStock => Set<MovimientoStock>();
    public DbSet<Proveedor> Proveedores => Set<Proveedor>();
    public DbSet<CategoriaProducto> CategoriasProducto => Set<CategoriaProducto>();
    public DbSet<PromesaPago> PromesasPago => Set<PromesaPago>();
    public DbSet<AuditoriaLog> AuditoriaLogs => Set<AuditoriaLog>();
    public DbSet<Feriado> Feriados => Set<Feriado>();
    public DbSet<AlertaOperativa> Alertas => Set<AlertaOperativa>();
    public DbSet<FlujoCajaProyectado> FlujoCajaProyectado => Set<FlujoCajaProyectado>();
    // ─── PARAMÉTRICAS ────────────────────────────────────────────────────
    public DbSet<CondicionIva> CondicionesIva => Set<CondicionIva>();
    public DbSet<CondicionPago> CondicionesPago => Set<CondicionPago>();
    public DbSet<Ocupacion> Ocupaciones => Set<Ocupacion>();
    public DbSet<Sector> Sectores => Set<Sector>();
    public DbSet<TipoCliente> TiposCliente => Set<TipoCliente>();
    public DbSet<FormaCobro> FormasCobro => Set<FormaCobro>();
    public DbSet<UnidadMedida> UnidadesMedida => Set<UnidadMedida>();

    // ─── ERP ─────────────────────────────────────────────────────────────
    public DbSet<ConfiguracionGlobal> Configuraciones => Set<ConfiguracionGlobal>();
    public DbSet<Perfil> Perfiles => Set<Perfil>();
    public DbSet<Permiso> Permisos => Set<Permiso>();
    public DbSet<PerfilPermiso> PerfilPermisos => Set<PerfilPermiso>();
    public DbSet<ListaPrecio> ListasPrecios => Set<ListaPrecio>();
    public DbSet<ListaPrecioItem> ListasPreciosItems => Set<ListaPrecioItem>();
    public DbSet<Venta> Ventas => Set<Venta>();
    public DbSet<VentaItem> VentaItems => Set<VentaItem>();
    public DbSet<FormaCobroVenta> FormasCobroVenta => Set<FormaCobroVenta>(); // link logic for POS if needed

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ─── USUARIO ─────────────────────────────────────────────────────
        modelBuilder.Entity<Usuario>(e =>
        {
            e.HasKey(u => u.Id);
            e.Property(u => u.Email).IsRequired().HasMaxLength(256);
            e.HasIndex(u => u.Email).IsUnique();
            e.Property(u => u.NombreCompleto).IsRequired().HasMaxLength(200);
            e.Property(u => u.PasswordHash).IsRequired();
            e.Property(u => u.Rol).HasConversion<string>();
        });

        modelBuilder.Entity<RefreshToken>(e =>
        {
            e.HasKey(rt => rt.Id);
            e.HasOne(rt => rt.Usuario)
             .WithMany(u => u.RefreshTokens)
             .HasForeignKey(rt => rt.UsuarioId);
        });

        // ─── ZONA ────────────────────────────────────────────────────────
        modelBuilder.Entity<Zona>(e =>
        {
            e.HasKey(z => z.Id);
            e.Property(z => z.Nombre).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<UsuarioZona>(e =>
        {
            e.HasKey(uz => new { uz.UsuarioId, uz.ZonaId });
            e.HasOne(uz => uz.Usuario).WithMany(u => u.Zonas).HasForeignKey(uz => uz.UsuarioId);
            e.HasOne(uz => uz.Zona).WithMany(z => z.Vendedores).HasForeignKey(uz => uz.ZonaId);
        });

        // ─── CLIENTE ─────────────────────────────────────────────────────
        modelBuilder.Entity<Cliente>(e =>
        {
            e.HasKey(c => c.Id);
            e.Property(c => c.NombreCompleto).IsRequired().HasMaxLength(200);
            e.Property(c => c.DNI).IsRequired().HasMaxLength(20);
            e.HasIndex(c => c.DNI).IsUnique();
            e.Property(c => c.CUIL).HasMaxLength(20);
            e.Property(c => c.Estado).HasConversion<string>();
            e.HasOne(c => c.Zona).WithMany(z => z.Clientes).HasForeignKey(c => c.ZonaId);
            e.HasOne(c => c.Ocupacion).WithMany().HasForeignKey(c => c.OcupacionId);
            e.HasOne(c => c.Sector).WithMany().HasForeignKey(c => c.SectorId);
            e.HasOne(c => c.TipoCliente).WithMany().HasForeignKey(c => c.TipoClienteId);
        });

        // ─── CRÉDITO ─────────────────────────────────────────────────────
        modelBuilder.Entity<Credito>(e =>
        {
            e.HasKey(c => c.Id);
            e.Property(c => c.Capital).HasColumnType("decimal(18,2)");
            e.Property(c => c.TasaInteres).HasColumnType("decimal(8,4)");
            e.Property(c => c.TasaMoratoria).HasColumnType("decimal(8,4)");
            e.Property(c => c.MontoPorCuota).HasColumnType("decimal(18,2)");
            e.Property(c => c.MontoTotalConInteres).HasColumnType("decimal(18,2)");
            e.Property(c => c.SaldoPendiente).HasColumnType("decimal(18,2)");
            e.Property(c => c.Estado).HasConversion<string>();
            e.Property(c => c.Frecuencia).HasConversion<string>();
            e.HasOne(c => c.Cliente).WithMany(cl => cl.Creditos).HasForeignKey(c => c.ClienteId);
            e.HasOne(c => c.Vendedor).WithMany().HasForeignKey(c => c.VendedorId);
        });

        // ─── CUOTA ───────────────────────────────────────────────────────
        modelBuilder.Entity<Cuota>(e =>
        {
            e.HasKey(c => c.Id);
            e.Property(c => c.MontoOriginal).HasColumnType("decimal(18,2)");
            e.Property(c => c.MontoPagado).HasColumnType("decimal(18,2)");
            e.Property(c => c.InteresMora).HasColumnType("decimal(18,2)");
            e.Property(c => c.Estado).HasConversion<string>();
            e.HasOne(c => c.Credito).WithMany(cr => cr.Cuotas).HasForeignKey(c => c.CreditoId);
        });

        // ─── PAGO COBRADO ─────────────────────────────────────────────────
        modelBuilder.Entity<PagoCobrado>(e =>
        {
            e.HasKey(p => p.Id);
            e.Property(p => p.Monto).HasColumnType("decimal(18,2)");
            e.HasOne(p => p.Cuota).WithMany(c => c.Pagos).HasForeignKey(p => p.CuotaId);
            e.HasOne(p => p.Vendedor).WithMany().HasForeignKey(p => p.VendedorId);
        });

        // ─── CIERRE CAJA ─────────────────────────────────────────────────
        modelBuilder.Entity<CierreCaja>(e =>
        {
            e.HasKey(c => c.Id);
            e.Property(c => c.MontoDeclarado).HasColumnType("decimal(18,2)");
            e.Property(c => c.MontoSistema).HasColumnType("decimal(18,2)");
            e.Property(c => c.Estado).HasConversion<string>();
            e.HasOne(c => c.Vendedor).WithMany().HasForeignKey(c => c.VendedorId);
        });

        // ─── CATEGORÍA PRODUCTO ───────────────────────────────────────────
        modelBuilder.Entity<CategoriaProducto>(e =>
        {
            e.HasKey(c => c.Id);
            e.Property(c => c.Nombre).IsRequired().HasMaxLength(100);
        });

        // ─── PRODUCTO ─────────────────────────────────────────────────────
        modelBuilder.Entity<Producto>(e =>
        {
            e.HasKey(p => p.Id);
            e.Property(p => p.Nombre).IsRequired().HasMaxLength(200);
            e.Property(p => p.CostoUnitario).HasColumnType("decimal(18,2)");
            e.Property(p => p.PrecioVenta).HasColumnType("decimal(18,2)");
            e.Property(p => p.PrecioVentaMayorista).HasColumnType("decimal(18,2)");
            e.HasOne(p => p.Proveedor).WithMany().HasForeignKey(p => p.ProveedorId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(p => p.Categoria).WithMany().HasForeignKey(p => p.CategoriaId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(p => p.UnidadMedida).WithMany().HasForeignKey(p => p.UnidadMedidaId).OnDelete(DeleteBehavior.SetNull);
        });

        // ─── MOVIMIENTO STOCK ─────────────────────────────────────────────
        modelBuilder.Entity<MovimientoStock>(e =>
        {
            e.HasKey(m => m.Id);
            e.Property(m => m.Tipo).HasConversion<string>();
            e.Property(m => m.CostoUnitario).HasColumnType("decimal(18,2)");
            e.HasOne(m => m.Producto).WithMany(p => p.Movimientos).HasForeignKey(m => m.ProductoId);
        });

        // ─── PROVEEDOR ───────────────────────────────────────────────────
        modelBuilder.Entity<Proveedor>(e =>
        {
            e.HasKey(p => p.Id);
            e.Property(p => p.RazonSocial).IsRequired().HasMaxLength(200);
            e.Property(p => p.CUIT).HasMaxLength(15);
        });

        // ─── AUDITORÍA ───────────────────────────────────────────────────
        modelBuilder.Entity<AuditoriaLog>(e =>
        {
            e.HasKey(a => a.Id);
            e.Property(a => a.Entidad).IsRequired().HasMaxLength(100);
            e.Property(a => a.Accion).IsRequired().HasMaxLength(20);
        });

        // ─── ALERTA ──────────────────────────────────────────────────────
        modelBuilder.Entity<AlertaOperativa>(e =>
        {
            e.HasKey(a => a.Id);
            e.Property(a => a.Tipo).HasConversion<string>();
        });

        // ─── PROMESA PAGO ────────────────────────────────────────────────
        modelBuilder.Entity<PromesaPago>(e =>
        {
            e.HasKey(p => p.Id);
            e.Property(p => p.MontoPrometido).HasColumnType("decimal(18,2)");
            e.Property(p => p.Estado).HasConversion<string>();
            e.HasOne(p => p.Cliente).WithMany(c => c.Promesas).HasForeignKey(p => p.ClienteId);
        });

        // ─── FLUJO CAJA ───────────────────────────────────────────────────
        modelBuilder.Entity<FlujoCajaProyectado>(e =>
        {
            e.HasKey(f => f.Id);
            e.Property(f => f.MontoProyectado).HasColumnType("decimal(18,2)");
            e.Property(f => f.MontoRecuperado).HasColumnType("decimal(18,2)");
        });

        // ─── SEGURIDAD ───────────────────────────────────────────────────
        modelBuilder.Entity<PerfilPermiso>(e =>
        {
            e.HasKey(pp => new { pp.PerfilId, pp.PermisoId });
            e.HasOne(pp => pp.Perfil).WithMany(p => p.Permisos).HasForeignKey(pp => pp.PerfilId);
            e.HasOne(pp => pp.Permiso).WithMany().HasForeignKey(pp => pp.PermisoId);
        });

        modelBuilder.Entity<Usuario>(e =>
        {
            e.HasOne(u => u.Perfil).WithMany().HasForeignKey(u => u.PerfilId);
        });

        // ─── LISTA DE PRECIOS ─────────────────────────────────────────────
        modelBuilder.Entity<ListaPrecio>(e =>
        {
            e.HasKey(l => l.Id);
            e.Property(l => l.PorcentajeAjuste).HasColumnType("decimal(8,4)");
            e.Property(l => l.Tipo).HasConversion<string>();
        });

        modelBuilder.Entity<ListaPrecioItem>(e =>
        {
            e.HasKey(li => li.Id);
            e.Property(li => li.PrecioFijo).HasColumnType("decimal(18,2)");
            e.Property(li => li.PorcentajeOverride).HasColumnType("decimal(8,4)");
            e.HasOne(li => li.ListaPrecio)
             .WithMany(l => l.Items)
             .HasForeignKey(li => li.ListaPrecioId);
            e.HasOne(li => li.Producto).WithMany().HasForeignKey(li => li.ProductoId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(li => li.Categoria).WithMany().HasForeignKey(li => li.CategoriaId).OnDelete(DeleteBehavior.Cascade);
        });

        // ─── VENTA ───────────────────────────────────────────────────────
        modelBuilder.Entity<Venta>(e =>
        {
            e.HasKey(v => v.Id);
            e.Property(v => v.SubTotal).HasColumnType("decimal(18,2)");
            e.Property(v => v.Descuento).HasColumnType("decimal(18,2)");
            e.Property(v => v.Total).HasColumnType("decimal(18,2)");
            e.HasOne(v => v.Vendedor).WithMany().HasForeignKey(v => v.VendedorId);
            e.HasOne(v => v.Cliente).WithMany().HasForeignKey(v => v.ClienteId).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(v => v.FormaCobro).WithMany().HasForeignKey(v => v.FormaCobroId).OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<FormaCobroVenta>(e =>
        {
            e.HasKey(fv => fv.Id);
            e.Property(fv => fv.Monto).HasColumnType("decimal(18,2)");
            e.HasOne(fv => fv.Venta).WithMany().HasForeignKey(fv => fv.VentaId);
            e.HasOne(fv => fv.FormaCobro).WithMany().HasForeignKey(fv => fv.FormaCobroId);
        });

        modelBuilder.Entity<VentaItem>(e =>
        {
            e.HasKey(vi => vi.Id);
            e.Property(vi => vi.PrecioBase).HasColumnType("decimal(18,2)");
            e.Property(vi => vi.PrecioUnitario).HasColumnType("decimal(18,2)");
            e.HasOne(vi => vi.Venta).WithMany(v => v.Items).HasForeignKey(vi => vi.VentaId);
            e.HasOne(vi => vi.Producto).WithMany().HasForeignKey(vi => vi.ProductoId).OnDelete(DeleteBehavior.Restrict);
        });

        // ─── CONFIGURACIÓN GLOBAL ──────────────────────────────────────────
        modelBuilder.Entity<ConfiguracionGlobal>(e =>
        {
            e.HasKey(c => c.Id);
            e.Property(c => c.TasaInteresDefecto).HasColumnType("decimal(18,2)");
            e.Property(c => c.TasaMoraDefecto).HasColumnType("decimal(18,2)");
        });
    }
}
