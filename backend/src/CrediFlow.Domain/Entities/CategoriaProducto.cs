using CrediFlow.Domain.Common;

namespace CrediFlow.Domain.Entities;

public class CategoriaProducto : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public bool Activa { get; set; } = true;
    public string? ColorHex { get; set; }
    public string? Color { get; set; } // alias for frontend compatibility
}
