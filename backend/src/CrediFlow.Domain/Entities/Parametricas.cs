using CrediFlow.Domain.Common;

namespace CrediFlow.Domain.Entities;

public class CondicionIva : BaseEntity
{
    public string Nombre { get; private set; } = string.Empty;
    public bool Activa { get; private set; } = true;

    protected CondicionIva() { }

    public static CondicionIva Crear(string nombre) => new() { Nombre = nombre };
    public void Actualizar(string nombre) => Nombre = nombre;
    public void SetEstado(bool activa) => Activa = activa;
}

public class CondicionPago : BaseEntity
{
    public string Nombre { get; private set; } = string.Empty;
    public int DiasPlazo { get; private set; }
    public bool Activa { get; private set; } = true;

    protected CondicionPago() { }

    public static CondicionPago Crear(string nombre, int diasPlazo) => new() { Nombre = nombre, DiasPlazo = diasPlazo };
    public void Actualizar(string nombre, int diasPlazo)
    {
        Nombre = nombre;
        DiasPlazo = diasPlazo;
    }
    public void SetEstado(bool activa) => Activa = activa;
}

public class Ocupacion : BaseEntity
{
    public string Nombre { get; private set; } = string.Empty;
    public bool Activa { get; private set; } = true;

    protected Ocupacion() { }

    public static Ocupacion Crear(string nombre) => new() { Nombre = nombre };
    public void Actualizar(string nombre) => Nombre = nombre;
    public void SetEstado(bool activa) => Activa = activa;
}

public class Sector : BaseEntity
{
    public string Nombre { get; private set; } = string.Empty;
    public bool Activa { get; private set; } = true;

    protected Sector() { }

    public static Sector Crear(string nombre) => new() { Nombre = nombre };
    public void Actualizar(string nombre) => Nombre = nombre;
    public void SetEstado(bool activa) => Activa = activa;
}

public class TipoCliente : BaseEntity
{
    public string Nombre { get; private set; } = string.Empty;
    public bool Activa { get; private set; } = true;

    protected TipoCliente() { }

    public static TipoCliente Crear(string nombre) => new() { Nombre = nombre };
    public void Actualizar(string nombre) => Nombre = nombre;
    public void SetEstado(bool activa) => Activa = activa;
}

public class FormaCobro : BaseEntity
{
    public string Nombre { get; private set; } = string.Empty;
    public bool Activa { get; private set; } = true;

    protected FormaCobro() { }

    public static FormaCobro Crear(string nombre) => new() { Nombre = nombre };
    public void Actualizar(string nombre) => Nombre = nombre;
    public void SetEstado(bool activa) => Activa = activa;
}

public class UnidadMedida : BaseEntity
{
    public string Nombre { get; private set; } = string.Empty;
    public string Simbolo { get; private set; } = string.Empty;
    public bool Activa { get; private set; } = true;

    protected UnidadMedida() { }

    public static UnidadMedida Crear(string nombre, string simbolo) => new() { Nombre = nombre, Simbolo = simbolo };
    public void Actualizar(string nombre, string simbolo)
    {
        Nombre = nombre;
        Simbolo = simbolo;
    }
    public void SetEstado(bool activa) => Activa = activa;
}
