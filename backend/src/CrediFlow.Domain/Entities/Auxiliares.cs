using CrediFlow.Domain.Common;
using CrediFlow.Domain.Enums;

namespace CrediFlow.Domain.Entities;

public class PromesaPago : BaseEntity
{
    public Guid ClienteId { get; set; }
    public Cliente? Cliente { get; set; }
    public Guid VendedorId { get; set; }
    public Usuario? Vendedor { get; set; }
    public DateTime FechaPromesa { get; set; }
    public decimal MontoPrometido { get; set; }
    public EstadoPromesaPago Estado { get; set; } = EstadoPromesaPago.PENDIENTE;
    public string? Observaciones { get; set; }
}

public class AuditoriaLog : BaseEntity
{
    public Guid? UsuarioId { get; set; }
    public string Entidad { get; set; } = string.Empty;
    public string EntidadId { get; set; } = string.Empty;
    public string Accion { get; set; } = string.Empty; // CREATE, UPDATE, DELETE
    public string? ValoresAntes { get; set; } // JSON
    public string? ValoresDespues { get; set; } // JSON
    public string? DireccionIp { get; set; }
    public string? UserAgent { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public class Feriado : BaseEntity
{
    public DateTime Fecha { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
}

public class AlertaOperativa : BaseEntity
{
    public TipoAlerta Tipo { get; set; }
    public string Mensaje { get; set; } = string.Empty;
    public string? EntidadAfectadaId { get; set; }
    public bool Leida { get; set; }
    public Guid? DestinoUsuarioId { get; set; } // null = todos los admins
    public DateTime FechaExpiracion { get; set; } = DateTime.UtcNow.AddDays(7);
}

public class FlujoCajaProyectado : BaseEntity
{
    public DateTime FechaCalculo { get; set; } = DateTime.UtcNow;
    public int Semana { get; set; } // 1-4
    public DateTime FechaDesde { get; set; }
    public DateTime FechaHasta { get; set; }
    public decimal MontoProyectado { get; set; }
    public decimal MontoRecuperado { get; set; } // actualizado en tiempo real
    public int CantidadCuotas { get; set; }
}
