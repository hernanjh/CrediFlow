using CrediFlow.Domain.Entities;
using CrediFlow.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CrediFlow.Infrastructure.Persistence;

public class DataSeeder
{
    private readonly CrediFlowDbContext _context;
    private readonly ILogger<DataSeeder> _logger;

    public DataSeeder(CrediFlowDbContext context, ILogger<DataSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken ct = default)
    {
        _logger.LogInformation("Iniciando seed de datos demo...");

        await _context.Database.MigrateAsync(ct);

        if (await _context.Usuarios.AnyAsync(ct)) return;

        // ─── CONFIGURACIONES ───────────────────────────────────────────
        var config = new ConfiguracionGlobal
        {
            NombreEmpresa = "CrediFlow AR",
            CUIT = "30-12345678-9",
            TasaInteresDefecto = 20,
            TasaMoraDefecto = 0.5m,
            MonedaSimbolo = "$"
        };
        await _context.Configuraciones.AddAsync(config, ct);

        // ─── PERMISOS & PERFILES ────────────────────────────────────────
        var perVerClientes = new Permiso { Nombre = "CLIENTES_VER", Descripcion = "Ver lista de clientes", Seccion = "CLIENTES" };
        var perCrearCredito = new Permiso { Nombre = "CREDITOS_CREAR", Descripcion = "Crear nuevos créditos", Seccion = "CREDITOS" };
        var perVerReportes = new Permiso { Nombre = "REPORTES_VER", Descripcion = "Ver reportes y BI", Seccion = "REPORTES" };
        var perVentaContado = new Permiso { Nombre = "VENTAS_CONTADO", Descripcion = "Realizar ventas de contado", Seccion = "VENTAS" };
        var perAdminStock = new Permiso { Nombre = "INVENTARIO_ADMIN", Descripcion = "Administrar stock y productos", Seccion = "INVENTARIO" };

        await _context.Permisos.AddRangeAsync(perVerClientes, perCrearCredito, perVerReportes, perVentaContado, perAdminStock);

        var perfAdmin = new Perfil { Nombre = "Administrador Full", Descripcion = "Acceso total al sistema" };
        perfAdmin.AgregarPermiso(perVerClientes.Id);
        perfAdmin.AgregarPermiso(perCrearCredito.Id);
        perfAdmin.AgregarPermiso(perVerReportes.Id);
        perfAdmin.AgregarPermiso(perVentaContado.Id);
        perfAdmin.AgregarPermiso(perAdminStock.Id);

        var perfVendedor = new Perfil { Nombre = "Vendedor/Cobrador", Descripcion = "Solo ventas y cobranzas" };
        perfVendedor.AgregarPermiso(perVerClientes.Id);
        perfVendedor.AgregarPermiso(perCrearCredito.Id);
        perfVendedor.AgregarPermiso(perVentaContado.Id);

        await _context.Perfiles.AddRangeAsync(perfAdmin, perfVendedor);
        await _context.SaveChangesAsync(ct);

        // ─── USUARIOS ────────────────────────────────────────────────────
        var superAdmin = Usuario.Crear("Super Administrador", "superadmin@crediflow.com",
            BCrypt.Net.BCrypt.HashPassword("Admin123!"), TipoUsuario.SUPER_ADMIN);
        superAdmin.AsignarPerfil(perfAdmin.Id);

        var admin = Usuario.Crear("María González", "admin@crediflow.com",
            BCrypt.Net.BCrypt.HashPassword("Admin123!"), TipoUsuario.ADMIN, 2m);
        admin.AsignarPerfil(perfAdmin.Id);

        var vendedor1 = Usuario.Crear("Carlos Rodríguez", "carlos@crediflow.com",
            BCrypt.Net.BCrypt.HashPassword("Vendedor123!"), TipoUsuario.VENDEDOR, 5m);
        vendedor1.AsignarPerfil(perfVendedor.Id);

        var vendedor2 = Usuario.Crear("Ana Martínez", "ana@crediflow.com",
            BCrypt.Net.BCrypt.HashPassword("Vendedor123!"), TipoUsuario.VENDEDOR, 5m);
        vendedor2.AsignarPerfil(perfVendedor.Id);

        var vendedor3 = Usuario.Crear("Pedro López", "pedro@crediflow.com",
            BCrypt.Net.BCrypt.HashPassword("Vendedor123!"), TipoUsuario.VENDEDOR, 4m);
        vendedor3.AsignarPerfil(perfVendedor.Id);

        await _context.Usuarios.AddRangeAsync(superAdmin, admin, vendedor1, vendedor2, vendedor3);
        await _context.SaveChangesAsync(ct);

        // ─── ZONAS ──────────────────────────────────────────────────────
        var zonaNorte = Zona.Crear("Norte", "Zona norte de la ciudad");
        var zonaSur = Zona.Crear("Sur", "Zona sur de la ciudad");
        var zonaEste = Zona.Crear("Este", "Zona este de la ciudad");
        var zonaOeste = Zona.Crear("Oeste", "Zona oeste de la ciudad");
        var zonaCentro = Zona.Crear("Centro", "Zona centro de la ciudad");

        await _context.Zonas.AddRangeAsync(zonaNorte, zonaSur, zonaEste, zonaOeste, zonaCentro);
        await _context.SaveChangesAsync(ct);

        // Asignar vendedores a zonas
        await _context.UsuarioZonas.AddRangeAsync(
            new UsuarioZona { UsuarioId = vendedor1.Id, ZonaId = zonaNorte.Id, EsPrincipal = true },
            new UsuarioZona { UsuarioId = vendedor1.Id, ZonaId = zonaEste.Id, EsPrincipal = false },
            new UsuarioZona { UsuarioId = vendedor2.Id, ZonaId = zonaSur.Id, EsPrincipal = true },
            new UsuarioZona { UsuarioId = vendedor2.Id, ZonaId = zonaOeste.Id, EsPrincipal = false },
            new UsuarioZona { UsuarioId = vendedor3.Id, ZonaId = zonaCentro.Id, EsPrincipal = true });
        await _context.SaveChangesAsync(ct);

        // ─── FERIADOS ARGENTINA 2025 ─────────────────────────────────────
        var feriados = new[]
        {
            new Feriado { Fecha = new DateTime(2025, 1, 1), Descripcion = "Año Nuevo" },
            new Feriado { Fecha = new DateTime(2025, 3, 3), Descripcion = "Carnaval" },
            new Feriado { Fecha = new DateTime(2025, 3, 4), Descripcion = "Carnaval" },
            new Feriado { Fecha = new DateTime(2025, 3, 24), Descripcion = "Día de la Memoria" },
            new Feriado { Fecha = new DateTime(2025, 4, 2), Descripcion = "Día del Veterano" },
            new Feriado { Fecha = new DateTime(2025, 5, 1), Descripcion = "Día del Trabajador" },
            new Feriado { Fecha = new DateTime(2025, 5, 25), Descripcion = "25 de Mayo" },
            new Feriado { Fecha = new DateTime(2025, 6, 20), Descripcion = "Paso a la Inmortalidad del Gral. Belgrano" },
            new Feriado { Fecha = new DateTime(2025, 7, 9), Descripcion = "Independencia Argentina" },
            new Feriado { Fecha = new DateTime(2025, 8, 17), Descripcion = "Paso a la Inmortalidad del Gral. San Martín" },
            new Feriado { Fecha = new DateTime(2025, 10, 13), Descripcion = "Día del Respeto a la Diversidad Cultural" },
            new Feriado { Fecha = new DateTime(2025, 11, 20), Descripcion = "Día de la Soberanía Nacional" },
            new Feriado { Fecha = new DateTime(2025, 12, 8), Descripcion = "Inmaculada Concepción" },
            new Feriado { Fecha = new DateTime(2025, 12, 25), Descripcion = "Navidad" },
        };
        await _context.Feriados.AddRangeAsync(feriados);

        // ─── CATEGORÍAS ──────────────────────────────────────────────────
        var catMuebles = new CategoriaProducto { Nombre = "Muebles", Descripcion = "Mesas, sillas, camas", ColorHex = "#8B4513" };
        var catElectro = new CategoriaProducto { Nombre = "Electrodomésticos", Descripcion = "Línea blanca y marrón", ColorHex = "#4682B4" };
        var catTextil = new CategoriaProducto { Nombre = "Textil Blanco", Descripcion = "Sábanas, frazadas, toallas", ColorHex = "#2E8B57" };
        
        await _context.CategoriasProducto.AddRangeAsync(catMuebles, catElectro, catTextil);

        // ─── PROVEEDORES ─────────────────────────────────────────────────
        // ─── PROVEEDORES ─────────────────────────────────────────────────
        var proveedor1 = Proveedor.Crear("Distribuidora El Hogar S.A.", "30-12345678-9", "011-4567-8901", "ventas@elhogar.com", null);
        var proveedor2 = Proveedor.Crear("Textiles del Norte SRL", "30-98765432-1", "011-3456-7890", "info@textilesnorte.com", null);

        await _context.Proveedores.AddRangeAsync(new[] { proveedor1, proveedor2 }, ct);
        await _context.SaveChangesAsync(ct);

        // ─── PARAMÉTRICAS NUEVAS ──────────────────────────────────────────
        var condIvaRI = CondicionIva.Crear("Responsable Inscripto");
        var condIvaMono = CondicionIva.Crear("Monotributista");
        var condIvaCons = CondicionIva.Crear("Consumidor Final");
        await _context.CondicionesIva.AddRangeAsync(new[] { condIvaRI, condIvaMono, condIvaCons }, ct);

        var condPagoContado = CondicionPago.Crear("Contado", 0);
        var condPago30rd = CondicionPago.Crear("30 Días", 30);
        await _context.CondicionesPago.AddRangeAsync(new[] { condPagoContado, condPago30rd }, ct);

        var ocupEmpelado = Ocupacion.Crear("Empleado");
        var ocupIndependiente = Ocupacion.Crear("Independiente");
        var ocupJubilado = Ocupacion.Crear("Jubilado");
        await _context.Ocupaciones.AddRangeAsync(new[] { ocupEmpelado, ocupIndependiente, ocupJubilado }, ct);

        var sectorPrivado = Sector.Crear("Sector Privado");
        var sectorPublico = Sector.Crear("Sector Público");
        await _context.Sectores.AddRangeAsync(new[] { sectorPrivado, sectorPublico }, ct);

        var tipoClieNormal = TipoCliente.Crear("Regular");
        var tipoCliePreferencial = TipoCliente.Crear("Preferencial");
        await _context.TiposCliente.AddRangeAsync(new[] { tipoClieNormal, tipoCliePreferencial }, ct);

        var formaEfectivo = FormaCobro.Crear("Efectivo");
        var formaTransf = FormaCobro.Crear("Transferencia");
        var formaCobrador = FormaCobro.Crear("Cobrador Domicilio");
        await _context.FormasCobro.AddRangeAsync(new[] { formaEfectivo, formaTransf, formaCobrador }, ct);

        var umUnidad = UnidadMedida.Crear("Unidad", "un");
        var umKg = UnidadMedida.Crear("Kilogramo", "kg");
        await _context.UnidadesMedida.AddRangeAsync(new[] { umUnidad, umKg }, ct);

        await _context.SaveChangesAsync(ct);

        // ─── PRODUCTOS ───────────────────────────────────────────────────
        var productos = new[]
        {
            Producto.Crear("Colchón 2 Plazas", 15000m, 22000m, 10, 3, "Colchón resortes 2 plazas", null, proveedor1.Id, catMuebles.Id, unidadMedidaId: umUnidad.Id),
            Producto.Crear("TV 32\" LED", 45000m, 65000m, 5, 2, "Smart TV 32 pulgadas", null, proveedor1.Id, catElectro.Id, unidadMedidaId: umUnidad.Id),
            Producto.Crear("Heladera No Frost", 80000m, 115000m, 3, 2, "Heladera 360 litros", null, proveedor1.Id, catElectro.Id, unidadMedidaId: umUnidad.Id),
            Producto.Crear("Juego de Comedor 6 sillas", 25000m, 38000m, 8, 2, "Mesa y 6 sillas madera", null, proveedor1.Id, catMuebles.Id, unidadMedidaId: umUnidad.Id),
            Producto.Crear("Sábanas 2 Plazas", 3500m, 5500m, 50, 10, "Juego de sábanas algodón", null, proveedor2.Id, catTextil.Id, unidadMedidaId: umUnidad.Id),
            Producto.Crear("Frazada King", 4200m, 6800m, 30, 8, "Frazada matrimonial 2 plazas", null, proveedor2.Id, catTextil.Id, unidadMedidaId: umUnidad.Id),
        };

        await _context.Productos.AddRangeAsync(productos);
        await _context.SaveChangesAsync(ct);

        // ─── CLIENTES (30 clientes de ejemplo) ───────────────────────────
        var nombresM = new[] { "Juan Pérez", "Roberto García", "Diego Fernández", "Marcelo López",
            "Gustavo Martínez", "Héctor Suárez", "Néstor Romero", "Pablo Díaz",
            "Eduardo Ruiz", "Andrés Torres", "Ramiro Flores", "Lucas Castro",
            "Facundo Medina", "Rodrigo Jiménez", "Agustín Morales" };
        var nombresF = new[] { "María González", "Laura Rodríguez", "Patricia Álvarez", "Claudia Sánchez",
            "Verónica Cruz", "Silvia Vargas", "Adriana Ramos", "Carolina Gutiérrez",
            "Valeria Herrera", "Natalia Reyes", "Daniela Molina", "Cecilia Ortega",
            "Beatriz Aguilar", "Rosa Núñez", "Graciela Ríos" };

        var zonas = new[] { zonaNorte, zonaSur, zonaEste, zonaOeste, zonaCentro };
        var rnd = new Random(42);
        var clientes = new List<Cliente>();

        for (int i = 0; i < nombresM.Length; i++)
        {
            var zona = zonas[i % zonas.Length];
            var c = Cliente.Crear(nombresM[i], $"{20000000 + i * 1234567:00000000}",
                $"Calle {100 + i * 10} N° {200 + i}, Barrio Norte",
                zona.Id, $"11{4000 + i:0000}-{1000 + i:0000}",
                tipoClienteId: tipoClieNormal.Id, ocupacionId: ocupEmpelado.Id, sectorId: sectorPrivado.Id);
            clientes.Add(c);
        }

        for (int i = 0; i < nombresF.Length; i++)
        {
            var zona = zonas[i % zonas.Length];
            var c = Cliente.Crear(nombresF[i], $"{30000000 + i * 1234567:00000000}",
                $"Av. Rivadavia {500 + i * 15} Piso {i % 5 + 1} Dto {(char)('A' + i % 4)}",
                zona.Id, $"11{5000 + i:0000}-{2000 + i:0000}",
                tipoClienteId: tipoClieNormal.Id, ocupacionId: ocupIndependiente.Id, sectorId: sectorPrivado.Id);
            clientes.Add(c);
        }

        await _context.Clientes.AddRangeAsync(clientes);
        await _context.SaveChangesAsync(ct);

        // ─── CRÉDITOS Y CUOTAS ───────────────────────────────────────────
        var vendedores = new[] { vendedor1, vendedor2, vendedor3 };
        var frecuencias = new[] { FrecuenciaCuota.SEMANAL, FrecuenciaCuota.SEMANAL, FrecuenciaCuota.MENSUAL };
        var tasas = new[] { 30m, 35m, 40m, 25m };
        var capitales = new[] { 10000m, 15000m, 20000m, 8000m, 25000m, 12000m };

        for (int i = 0; i < Math.Min(clientes.Count, 20); i++)
        {
            var vendedor = vendedores[i % vendedores.Length];
            var capital = capitales[i % capitales.Length];
            var tasa = tasas[i % tasas.Length];
            var freq = frecuencias[i % frecuencias.Length];
            int nroCuotas = freq == FrecuenciaCuota.SEMANAL ? 20 : 6;
            int diasFreq = (int)freq;

            var fechaOtorgamiento = DateTime.UtcNow.AddDays(-(rnd.Next(1, 120)));
            var fechaPrimerVenc = fechaOtorgamiento.AddDays(diasFreq);
            var montoTotal = capital * (1 + tasa / 100m);
            var montoCuota = Math.Round(montoTotal / nroCuotas, 2);

            var credito = Credito.Crear(
                clientes[i].Id, vendedor.Id, capital, tasa, 0.1m,
                nroCuotas, freq, fechaPrimerVenc);

            await _context.Creditos.AddAsync(credito);
            await _context.SaveChangesAsync(ct);

            // Generar cuotas
            var cuotas = new List<Cuota>();
            for (int j = 0; j < nroCuotas; j++)
            {
                var fechaVenc = fechaPrimerVenc.AddDays(j * diasFreq);
                var cuota = Cuota.Crear(credito.Id, j + 1, fechaVenc, montoCuota);

                // Simular pagos para cuotas pasadas
                if (fechaVenc.Date < DateTime.UtcNow.Date)
                {
                    bool pago = rnd.Next(0, 100) < 80; // 80% pagan
                    if (pago)
                    {
                        cuota.RegistrarPago(montoCuota, Guid.NewGuid());
                        var pago_cobrado = PagoCobrado.Crear(cuota.Id, vendedor.Id, montoCuota);
                        await _context.PagosCobrados.AddAsync(pago_cobrado);
                    }
                    else
                    {
                        cuota.AplicarMora(0.001m);
                    }
                }
                cuotas.Add(cuota);
            }

            await _context.Cuotas.AddRangeAsync(cuotas);
        }

        await _context.SaveChangesAsync(ct);

        // ─── ALERTAS ─────────────────────────────────────────────────────
        var alertas = new[]
        {
            new AlertaOperativa
            {
                Tipo = TipoAlerta.MORA_GRAVE,
                Mensaje = "5 clientes superaron 90 días sin pago — evaluar pase a pérdida",
                FechaExpiracion = DateTime.UtcNow.AddDays(7)
            },
            new AlertaOperativa
            {
                Tipo = TipoAlerta.MORA_GRAVE,
                Mensaje = "Zona Norte — mora 23%, por encima del umbral crítico",
                FechaExpiracion = DateTime.UtcNow.AddDays(3)
            },
            new AlertaOperativa
            {
                Tipo = TipoAlerta.PRODUCTO_STOCK_BAJO,
                Mensaje = "Producto 'Colchón 2 Plazas' — stock crítico: 2 unidades",
                FechaExpiracion = DateTime.UtcNow.AddDays(14)
            },
            new AlertaOperativa
            {
                Tipo = TipoAlerta.PROMESA_PAGO_VENCIDA,
                Mensaje = "12 promesas de pago vencen mañana — recordar seguimiento",
                FechaExpiracion = DateTime.UtcNow.AddDays(1)
            },
        };
        // ─── LISTAS DE PRECIOS ──────────────────────────────────────────
        var listas = new[]
        {
            new ListaPrecio { Nombre = "Efectivo / Base", PorcentajeAjuste = 0 },
            new ListaPrecio { Nombre = "Tarjeta Crédito (+15%)", PorcentajeAjuste = 15 },
            new ListaPrecio { Nombre = "Transferencia (-5%)", PorcentajeAjuste = -5 }
        };
        await _context.ListasPrecios.AddRangeAsync(listas);
        await _context.SaveChangesAsync(ct);

        // ─── VENTAS HISTÓRICAS (Últimos 30 días) ───────────────────────
        var ventas = new List<Venta>();
        for (int i = 0; i < 30; i++)
        {
            var fecha = DateTime.UtcNow.AddDays(-i);
            int cantVentas = rnd.Next(1, 5);
            for (int j = 0; j < cantVentas; j++)
            {
                var cli = clientes[rnd.Next(clientes.Count)];
                var vend = vendedores[rnd.Next(vendedores.Length)];
                var prod = productos[rnd.Next(productos.Length)];
                
                var v = new Venta
                {
                    VendedorId = vend.Id,
                    ClienteId = cli.Id,
                    FormaCobroId = formaEfectivo.Id,
                    Total = prod.PrecioVenta,
                    EsContado = true
                };
                v.AgregarItem(prod.Id, rnd.Next(1, 3), prod.PrecioVenta);
                
                // Reflection hack if Fecha is private set
                var prop = typeof(Venta).GetProperty("Fecha");
                if (prop != null) prop.SetValue(v, fecha);
                
                ventas.Add(v);
            }
        }
        await _context.Ventas.AddRangeAsync(ventas);
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Seed de datos demo completado exitosamente.");
    }
}
