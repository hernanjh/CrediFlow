using CrediFlow.Domain.Common;
using CrediFlow.Domain.Enums;

namespace CrediFlow.Domain.Entities;

public class Zona : BaseEntity
{
    public string Nombre { get; private set; } = string.Empty;
    public string? Descripcion { get; private set; }
    public string? CoordenadasGeoJson { get; private set; }
    public string? ColorHex { get; private set; }
    public bool Activa { get; private set; } = true;

    private readonly List<Cliente> _clientes = new();
    public IReadOnlyCollection<Cliente> Clientes => _clientes.AsReadOnly();

    private readonly List<UsuarioZona> _vendedores = new();
    public IReadOnlyCollection<UsuarioZona> Vendedores => _vendedores.AsReadOnly();

    protected Zona() { }

    public static Zona Crear(string nombre, string? descripcion = null, string? coordenadasGeoJson = null, string? colorHex = null)
    {
        if (string.IsNullOrWhiteSpace(nombre)) throw new ArgumentException("El nombre de la zona es requerido.");
        return new Zona
        {
            Nombre = nombre,
            Descripcion = descripcion,
            CoordenadasGeoJson = coordenadasGeoJson,
            ColorHex = colorHex
        };
    }

    public void Actualizar(string nombre, string? descripcion, string? coordenadasGeoJson, string? colorHex)
    {
        Nombre = nombre;
        Descripcion = descripcion;
        CoordenadasGeoJson = coordenadasGeoJson;
        ColorHex = colorHex;
    }

    public void Desactivar() => Activa = false;
    public void Activar() => Activa = true;
}

public class UsuarioZona
{
    public Guid UsuarioId { get; set; }
    public Guid ZonaId { get; set; }
    public Usuario? Usuario { get; set; }
    public Zona? Zona { get; set; }
    public bool EsPrincipal { get; set; }
}
