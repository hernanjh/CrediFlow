using CrediFlow.Domain.Common;

namespace CrediFlow.Domain.Entities;

public class ConfiguracionGlobal : BaseEntity
{
    public string NombreEmpresa { get; set; } = "CrediFlow";
    public string? CUIT { get; set; }
    public string? RazonSocial { get; set; }
    public string? Direccion { get; set; }
    public string? Telefono { get; set; }
    public string? EmailContacto { get; set; }
    public string? WebSite { get; set; }
    
    // Configuración Financiera
    public decimal TasaInteresDefecto { get; set; } = 20;
    public decimal TasaMoraDefecto { get; set; } = 0.5m;
    public string MonedaSimbolo { get; set; } = "$";
    public string MonedaCodigo { get; set; } = "ARS";

    // Visual
    public string? LogoBase64 { get; set; }
    public string? ThemeColor { get; set; } = "#6366f1"; // Indigo-500
}
