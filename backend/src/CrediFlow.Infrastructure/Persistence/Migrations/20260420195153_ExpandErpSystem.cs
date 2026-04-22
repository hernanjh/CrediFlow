using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrediFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ExpandErpSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AdelantoRetiraado",
                table: "Usuarios",
                newName: "AdelantoRetirado");

            migrationBuilder.AddColumn<Guid>(
                name: "PerfilId",
                table: "Usuarios",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CodigoInterno",
                table: "Productos",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SKU",
                table: "Productos",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Configuraciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    NombreEmpresa = table.Column<string>(type: "TEXT", nullable: false),
                    CUIT = table.Column<string>(type: "TEXT", nullable: true),
                    RazonSocial = table.Column<string>(type: "TEXT", nullable: true),
                    Direccion = table.Column<string>(type: "TEXT", nullable: true),
                    Telefono = table.Column<string>(type: "TEXT", nullable: true),
                    EmailContacto = table.Column<string>(type: "TEXT", nullable: true),
                    WebSite = table.Column<string>(type: "TEXT", nullable: true),
                    TasaInteresDefecto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TasaMoraDefecto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MonedaSimbolo = table.Column<string>(type: "TEXT", nullable: false),
                    MonedaCodigo = table.Column<string>(type: "TEXT", nullable: false),
                    LogoBase64 = table.Column<string>(type: "TEXT", nullable: true),
                    ThemeColor = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configuraciones", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ListasPrecios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Nombre = table.Column<string>(type: "TEXT", nullable: false),
                    PorcentajeAjuste = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Activa = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListasPrecios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Perfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Nombre = table.Column<string>(type: "TEXT", nullable: false),
                    Descripcion = table.Column<string>(type: "TEXT", nullable: true),
                    Activo = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Perfiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Permisos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Nombre = table.Column<string>(type: "TEXT", nullable: false),
                    Descripcion = table.Column<string>(type: "TEXT", nullable: false),
                    Seccion = table.Column<string>(type: "TEXT", nullable: false),
                    Activo = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permisos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ventas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Fecha = table.Column<DateTime>(type: "TEXT", nullable: false),
                    VendedorId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ClienteId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MetodoPago = table.Column<string>(type: "TEXT", nullable: true),
                    EsContado = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ventas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ventas_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Ventas_Usuarios_VendedorId",
                        column: x => x.VendedorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PerfilPermisos",
                columns: table => new
                {
                    PerfilId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PermisoId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerfilPermisos", x => new { x.PerfilId, x.PermisoId });
                    table.ForeignKey(
                        name: "FK_PerfilPermisos_Perfiles_PerfilId",
                        column: x => x.PerfilId,
                        principalTable: "Perfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PerfilPermisos_Permisos_PermisoId",
                        column: x => x.PermisoId,
                        principalTable: "Permisos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VentaItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    VentaId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProductoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Cantidad = table.Column<int>(type: "INTEGER", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VentaItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VentaItems_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VentaItems_Ventas_VentaId",
                        column: x => x.VentaId,
                        principalTable: "Ventas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_PerfilId",
                table: "Usuarios",
                column: "PerfilId");

            migrationBuilder.CreateIndex(
                name: "IX_PerfilPermisos_PermisoId",
                table: "PerfilPermisos",
                column: "PermisoId");

            migrationBuilder.CreateIndex(
                name: "IX_VentaItems_ProductoId",
                table: "VentaItems",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_VentaItems_VentaId",
                table: "VentaItems",
                column: "VentaId");

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_ClienteId",
                table: "Ventas",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_VendedorId",
                table: "Ventas",
                column: "VendedorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Perfiles_PerfilId",
                table: "Usuarios",
                column: "PerfilId",
                principalTable: "Perfiles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Perfiles_PerfilId",
                table: "Usuarios");

            migrationBuilder.DropTable(
                name: "Configuraciones");

            migrationBuilder.DropTable(
                name: "ListasPrecios");

            migrationBuilder.DropTable(
                name: "PerfilPermisos");

            migrationBuilder.DropTable(
                name: "VentaItems");

            migrationBuilder.DropTable(
                name: "Perfiles");

            migrationBuilder.DropTable(
                name: "Permisos");

            migrationBuilder.DropTable(
                name: "Ventas");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_PerfilId",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "PerfilId",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "CodigoInterno",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "SKU",
                table: "Productos");

            migrationBuilder.RenameColumn(
                name: "AdelantoRetirado",
                table: "Usuarios",
                newName: "AdelantoRetiraado");
        }
    }
}
