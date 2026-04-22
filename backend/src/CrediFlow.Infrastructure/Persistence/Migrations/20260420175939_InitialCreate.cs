using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrediFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Alertas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Tipo = table.Column<string>(type: "TEXT", nullable: false),
                    Mensaje = table.Column<string>(type: "TEXT", nullable: false),
                    EntidadAfectadaId = table.Column<string>(type: "TEXT", nullable: true),
                    Leida = table.Column<bool>(type: "INTEGER", nullable: false),
                    DestinoUsuarioId = table.Column<Guid>(type: "TEXT", nullable: true),
                    FechaExpiracion = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alertas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuditoriaLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Entidad = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    EntidadId = table.Column<string>(type: "TEXT", nullable: false),
                    Accion = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    ValoresAntes = table.Column<string>(type: "TEXT", nullable: true),
                    ValoresDespues = table.Column<string>(type: "TEXT", nullable: true),
                    DireccionIp = table.Column<string>(type: "TEXT", nullable: true),
                    UserAgent = table.Column<string>(type: "TEXT", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditoriaLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Feriados",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Fecha = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Descripcion = table.Column<string>(type: "TEXT", nullable: false),
                    Activo = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feriados", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FlujoCajaProyectado",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    FechaCalculo = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Semana = table.Column<int>(type: "INTEGER", nullable: false),
                    FechaDesde = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FechaHasta = table.Column<DateTime>(type: "TEXT", nullable: false),
                    MontoProyectado = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MontoRecuperado = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CantidadCuotas = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlujoCajaProyectado", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Proveedores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    RazonSocial = table.Column<string>(type: "TEXT", nullable: false),
                    CUIT = table.Column<string>(type: "TEXT", nullable: true),
                    Telefono = table.Column<string>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    Direccion = table.Column<string>(type: "TEXT", nullable: true),
                    Activo = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proveedores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    NombreCompleto = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    Rol = table.Column<string>(type: "TEXT", nullable: false),
                    Activo = table.Column<bool>(type: "INTEGER", nullable: false),
                    FotoPerfil = table.Column<string>(type: "TEXT", nullable: true),
                    PorcentajeComision = table.Column<decimal>(type: "TEXT", nullable: false),
                    AdelantoRetiraado = table.Column<decimal>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Zonas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "TEXT", nullable: true),
                    CoordenadasGeoJson = table.Column<string>(type: "TEXT", nullable: true),
                    Activa = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zonas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Productos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Nombre = table.Column<string>(type: "TEXT", nullable: false),
                    Descripcion = table.Column<string>(type: "TEXT", nullable: true),
                    Foto = table.Column<string>(type: "TEXT", nullable: true),
                    CostoUnitario = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PrecioVenta = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StockActual = table.Column<int>(type: "INTEGER", nullable: false),
                    StockMinimo = table.Column<int>(type: "INTEGER", nullable: false),
                    Activo = table.Column<bool>(type: "INTEGER", nullable: false),
                    ProveedorId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Productos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Productos_Proveedores_ProveedorId",
                        column: x => x.ProveedorId,
                        principalTable: "Proveedores",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CierresCaja",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    VendedorId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Fecha = table.Column<DateTime>(type: "TEXT", nullable: false),
                    MontoDeclarado = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MontoSistema = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Estado = table.Column<string>(type: "TEXT", nullable: false),
                    UmbralDiferencia = table.Column<decimal>(type: "TEXT", nullable: false),
                    Observaciones = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CierresCaja", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CierresCaja_Usuarios_VendedorId",
                        column: x => x.VendedorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Token = table.Column<string>(type: "TEXT", nullable: false),
                    Expiracion = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Revocado = table.Column<bool>(type: "INTEGER", nullable: false),
                    DireccionIp = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    NombreCompleto = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    DNI = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    FotoDNIFrente = table.Column<string>(type: "TEXT", nullable: true),
                    FotoDNIDorso = table.Column<string>(type: "TEXT", nullable: true),
                    FotoPerfil = table.Column<string>(type: "TEXT", nullable: true),
                    Direccion = table.Column<string>(type: "TEXT", nullable: false),
                    Telefono = table.Column<string>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    Latitud = table.Column<double>(type: "REAL", nullable: true),
                    Longitud = table.Column<double>(type: "REAL", nullable: true),
                    ZonaId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Estado = table.Column<string>(type: "TEXT", nullable: false),
                    Observaciones = table.Column<string>(type: "TEXT", nullable: true),
                    FechaAlta = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clientes_Zonas_ZonaId",
                        column: x => x.ZonaId,
                        principalTable: "Zonas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioZonas",
                columns: table => new
                {
                    UsuarioId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ZonaId = table.Column<Guid>(type: "TEXT", nullable: false),
                    EsPrincipal = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioZonas", x => new { x.UsuarioId, x.ZonaId });
                    table.ForeignKey(
                        name: "FK_UsuarioZonas_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuarioZonas_Zonas_ZonaId",
                        column: x => x.ZonaId,
                        principalTable: "Zonas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MovimientosStock",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProductoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Tipo = table.Column<string>(type: "TEXT", nullable: false),
                    Cantidad = table.Column<int>(type: "INTEGER", nullable: false),
                    CostoUnitario = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Motivo = table.Column<string>(type: "TEXT", nullable: true),
                    ProveedorId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovimientosStock", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MovimientosStock_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MovimientosStock_Proveedores_ProveedorId",
                        column: x => x.ProveedorId,
                        principalTable: "Proveedores",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Creditos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ClienteId = table.Column<Guid>(type: "TEXT", nullable: false),
                    VendedorId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Capital = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TasaInteres = table.Column<decimal>(type: "decimal(8,4)", nullable: false),
                    TasaMoratoria = table.Column<decimal>(type: "decimal(8,4)", nullable: false),
                    NumeroCuotas = table.Column<int>(type: "INTEGER", nullable: false),
                    Frecuencia = table.Column<string>(type: "TEXT", nullable: false),
                    MontoTotalConInteres = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MontoPorCuota = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SaldoPendiente = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FechaOtorgamiento = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FechaPrimerVencimiento = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Estado = table.Column<string>(type: "TEXT", nullable: false),
                    Observaciones = table.Column<string>(type: "TEXT", nullable: true),
                    ProductoId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Creditos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Creditos_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Creditos_Usuarios_VendedorId",
                        column: x => x.VendedorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PromesasPago",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ClienteId = table.Column<Guid>(type: "TEXT", nullable: false),
                    VendedorId = table.Column<Guid>(type: "TEXT", nullable: false),
                    FechaPromesa = table.Column<DateTime>(type: "TEXT", nullable: false),
                    MontoPrometido = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Estado = table.Column<string>(type: "TEXT", nullable: false),
                    Observaciones = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromesasPago", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PromesasPago_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PromesasPago_Usuarios_VendedorId",
                        column: x => x.VendedorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cuotas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreditoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    NumeroCuota = table.Column<int>(type: "INTEGER", nullable: false),
                    FechaVencimiento = table.Column<DateTime>(type: "TEXT", nullable: false),
                    MontoOriginal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MontoPagado = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    InteresMora = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Estado = table.Column<string>(type: "TEXT", nullable: false),
                    FechaPago = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cuotas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cuotas_Creditos_CreditoId",
                        column: x => x.CreditoId,
                        principalTable: "Creditos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PagosCobrados",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CuotaId = table.Column<Guid>(type: "TEXT", nullable: false),
                    VendedorId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FechaPago = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Latitud = table.Column<double>(type: "REAL", nullable: true),
                    Longitud = table.Column<double>(type: "REAL", nullable: true),
                    Observaciones = table.Column<string>(type: "TEXT", nullable: true),
                    EsOffline = table.Column<bool>(type: "INTEGER", nullable: false),
                    DeviceId = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PagosCobrados", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PagosCobrados_Cuotas_CuotaId",
                        column: x => x.CuotaId,
                        principalTable: "Cuotas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PagosCobrados_Usuarios_VendedorId",
                        column: x => x.VendedorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CierresCaja_VendedorId",
                table: "CierresCaja",
                column: "VendedorId");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_DNI",
                table: "Clientes",
                column: "DNI",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_ZonaId",
                table: "Clientes",
                column: "ZonaId");

            migrationBuilder.CreateIndex(
                name: "IX_Creditos_ClienteId",
                table: "Creditos",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Creditos_VendedorId",
                table: "Creditos",
                column: "VendedorId");

            migrationBuilder.CreateIndex(
                name: "IX_Cuotas_CreditoId",
                table: "Cuotas",
                column: "CreditoId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosStock_ProductoId",
                table: "MovimientosStock",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosStock_ProveedorId",
                table: "MovimientosStock",
                column: "ProveedorId");

            migrationBuilder.CreateIndex(
                name: "IX_PagosCobrados_CuotaId",
                table: "PagosCobrados",
                column: "CuotaId");

            migrationBuilder.CreateIndex(
                name: "IX_PagosCobrados_VendedorId",
                table: "PagosCobrados",
                column: "VendedorId");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_ProveedorId",
                table: "Productos",
                column: "ProveedorId");

            migrationBuilder.CreateIndex(
                name: "IX_PromesasPago_ClienteId",
                table: "PromesasPago",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_PromesasPago_VendedorId",
                table: "PromesasPago",
                column: "VendedorId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UsuarioId",
                table: "RefreshTokens",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioZonas_ZonaId",
                table: "UsuarioZonas",
                column: "ZonaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alertas");

            migrationBuilder.DropTable(
                name: "AuditoriaLogs");

            migrationBuilder.DropTable(
                name: "CierresCaja");

            migrationBuilder.DropTable(
                name: "Feriados");

            migrationBuilder.DropTable(
                name: "FlujoCajaProyectado");

            migrationBuilder.DropTable(
                name: "MovimientosStock");

            migrationBuilder.DropTable(
                name: "PagosCobrados");

            migrationBuilder.DropTable(
                name: "PromesasPago");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "UsuarioZonas");

            migrationBuilder.DropTable(
                name: "Productos");

            migrationBuilder.DropTable(
                name: "Cuotas");

            migrationBuilder.DropTable(
                name: "Proveedores");

            migrationBuilder.DropTable(
                name: "Creditos");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Zonas");
        }
    }
}
