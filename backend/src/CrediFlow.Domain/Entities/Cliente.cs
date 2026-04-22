using CrediFlow.Domain.Common;
using CrediFlow.Domain.Enums;

namespace CrediFlow.Domain.Entities;

public class Cliente : BaseEntity
{
    // ─── Datos Personales ───────────────────────────────────────────────
    public string NombreCompleto { get; private set; } = string.Empty;
    public string DNI { get; private set; } = string.Empty;
    public string? CUIL { get; private set; }
    public DateTime? FechaNacimiento { get; private set; }
    public string? Sexo { get; private set; } // M, F, X
    public string? EstadoCivil { get; private set; }
    public Guid? OcupacionId { get; private set; }
    public Ocupacion? Ocupacion { get; private set; }
    public Guid? SectorId { get; private set; }
    public Sector? Sector { get; private set; }
    public Guid? TipoClienteId { get; private set; }
    public TipoCliente? TipoCliente { get; private set; }

    // ─── Contacto ───────────────────────────────────────────────────────
    public string? Telefono { get; private set; }
    public string? TelefonoAlternativo { get; private set; }
    public string? Email { get; private set; }

    // ─── Dirección ──────────────────────────────────────────────────────
    public string Direccion { get; private set; } = string.Empty;
    public string? Barrio { get; private set; }
    public string? Localidad { get; private set; }
    public string? CodigoPostal { get; private set; }
    public string? Provincia { get; private set; }
    public double? Latitud { get; private set; }
    public double? Longitud { get; private set; }

    // ─── Zona / Vendedor preferencial ──────────────────────────────────
    public Guid ZonaId { get; private set; }
    public Zona? Zona { get; private set; }
    public Guid? VendedorAsignadoId { get; private set; }

    // ─── Límites y Crédito ──────────────────────────────────────────────
    public decimal LimiteCredito { get; private set; } = 0;
    public Guid? ListaPrecioId { get; private set; } // lista de precios preferencial

    // ─── Fiador / Garante ───────────────────────────────────────────────
    public string? FiadorNombre { get; private set; }
    public string? FiadorDNI { get; private set; }
    public string? FiadorTelefono { get; private set; }
    public string? FiadorDireccion { get; private set; }

    // ─── Documentación ──────────────────────────────────────────────────
    public string? FotoDNIFrente { get; private set; }
    public string? FotoDNIDorso { get; private set; }
    public string? FotoPerfil { get; private set; }

    // ─── Estado ─────────────────────────────────────────────────────────
    public EstadoCliente Estado { get; private set; } = EstadoCliente.ACTIVO;
    public string? Observaciones { get; private set; }
    public DateTime FechaAlta { get; private set; } = DateTime.UtcNow;

    // ─── Relaciones ─────────────────────────────────────────────────────
    private readonly List<Credito> _creditos = new();
    public IReadOnlyCollection<Credito> Creditos => _creditos.AsReadOnly();

    private readonly List<PromesaPago> _promesas = new();
    public IReadOnlyCollection<PromesaPago> Promesas => _promesas.AsReadOnly();

    protected Cliente() { }

    public static Cliente Crear(
        string nombreCompleto, string dni, string direccion, Guid zonaId,
        string? cuil = null, DateTime? fechaNacimiento = null, string? sexo = null,
        Guid? tipoClienteId = null, Guid? ocupacionId = null, Guid? sectorId = null,
        string? telefono = null, string? telefonoAlternativo = null, string? email = null,
        string? barrio = null, string? localidad = null, string? codigoPostal = null, string? provincia = null,
        double? latitud = null, double? longitud = null,
        decimal limiteCredito = 0, Guid? listaPrecioId = null, Guid? vendedorAsignadoId = null,
        string? fiadorNombre = null, string? fiadorDNI = null, string? fiadorTelefono = null, string? fiadorDireccion = null,
        string? ocupacion = null, string? estadoCivil = null, string? observaciones = null)
    {
        if (string.IsNullOrWhiteSpace(nombreCompleto)) throw new ArgumentException("El nombre es requerido.");
        if (string.IsNullOrWhiteSpace(dni)) throw new ArgumentException("El DNI es requerido.");
        if (string.IsNullOrWhiteSpace(direccion)) throw new ArgumentException("La dirección es requerida.");

        return new Cliente
        {
            NombreCompleto = nombreCompleto.Trim(),
            DNI = dni.Trim(),
            CUIL = cuil?.Trim(),
            FechaNacimiento = fechaNacimiento,
            Sexo = sexo,
            EstadoCivil = estadoCivil,
            OcupacionId = ocupacionId,
            SectorId = sectorId,
            TipoClienteId = tipoClienteId,
            Telefono = telefono?.Trim(),
            TelefonoAlternativo = telefonoAlternativo?.Trim(),
            Email = email?.ToLower().Trim(),
            Direccion = direccion.Trim(),
            Barrio = barrio?.Trim(),
            Localidad = localidad?.Trim(),
            CodigoPostal = codigoPostal?.Trim(),
            Provincia = provincia?.Trim(),
            Latitud = latitud,
            Longitud = longitud,
            ZonaId = zonaId,
            VendedorAsignadoId = vendedorAsignadoId,
            LimiteCredito = limiteCredito,
            ListaPrecioId = listaPrecioId,
            FiadorNombre = fiadorNombre?.Trim(),
            FiadorDNI = fiadorDNI?.Trim(),
            FiadorTelefono = fiadorTelefono?.Trim(),
            FiadorDireccion = fiadorDireccion?.Trim(),
            Observaciones = observaciones?.Trim()
        };
    }

    public void ActualizarFotos(string? frente, string? dorso, string? perfil)
    {
        if (frente != null) FotoDNIFrente = frente;
        if (dorso != null) FotoDNIDorso = dorso;
        if (perfil != null) FotoPerfil = perfil;
    }

    public void Actualizar(
        string nombreCompleto, string direccion, Guid zonaId,
        string? cuil = null, DateTime? fechaNacimiento = null, string? sexo = null,
        Guid? tipoClienteId = null, Guid? ocupacionId = null, Guid? sectorId = null,
        string? telefono = null, string? telefonoAlternativo = null, string? email = null,
        string? barrio = null, string? localidad = null, string? codigoPostal = null, string? provincia = null,
        double? latitud = null, double? longitud = null,
        decimal limiteCredito = 0, Guid? listaPrecioId = null, Guid? vendedorAsignadoId = null,
        string? fiadorNombre = null, string? fiadorDNI = null, string? fiadorTelefono = null, string? fiadorDireccion = null,
        string? ocupacion = null, string? estadoCivil = null, string? observaciones = null)
    {
        NombreCompleto = nombreCompleto.Trim();
        CUIL = cuil?.Trim();
        FechaNacimiento = fechaNacimiento;
        Sexo = sexo;
        EstadoCivil = estadoCivil;
        OcupacionId = ocupacionId;
        SectorId = sectorId;
        TipoClienteId = tipoClienteId;
        Direccion = direccion.Trim();
        ZonaId = zonaId;
        VendedorAsignadoId = vendedorAsignadoId;
        Telefono = telefono?.Trim();
        TelefonoAlternativo = telefonoAlternativo?.Trim();
        Email = email?.ToLower().Trim();
        Barrio = barrio?.Trim();
        Localidad = localidad?.Trim();
        CodigoPostal = codigoPostal?.Trim();
        Provincia = provincia?.Trim();
        Latitud = latitud;
        Longitud = longitud;
        LimiteCredito = limiteCredito;
        ListaPrecioId = listaPrecioId;
        FiadorNombre = fiadorNombre?.Trim();
        FiadorDNI = fiadorDNI?.Trim();
        FiadorTelefono = fiadorTelefono?.Trim();
        FiadorDireccion = fiadorDireccion?.Trim();
        Observaciones = observaciones?.Trim();
    }

    public void CambiarEstado(EstadoCliente nuevoEstado) => Estado = nuevoEstado;

    public bool TieneDeudaActiva => _creditos.Any(c =>
        c.Estado == EstadoCredito.ACTIVO ||
        c.Estado == EstadoCredito.EN_MORA ||
        c.Estado == EstadoCredito.EN_MORA_GRAVE);

    public int? Edad => FechaNacimiento.HasValue
        ? (int)((DateTime.Today - FechaNacimiento.Value.ToLocalTime()).TotalDays / 365.25)
        : null;
}
