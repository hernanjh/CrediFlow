using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrediFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CrediFlowV2_Entities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Productos_CategoriasProducto_CategoriaId",
                table: "Productos");

            migrationBuilder.DropForeignKey(
                name: "FK_Productos_Proveedores_ProveedorId",
                table: "Productos");

            migrationBuilder.DropForeignKey(
                name: "FK_VentaItems_Productos_ProductoId",
                table: "VentaItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Ventas_Clientes_ClienteId",
                table: "Ventas");

            migrationBuilder.AddColumn<decimal>(
                name: "Descuento",
                table: "Ventas",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "ListaPrecioId",
                table: "Ventas",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NumeroComprobante",
                table: "Ventas",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Observaciones",
                table: "Ventas",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SubTotal",
                table: "Ventas",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PrecioBase",
                table: "VentaItems",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Celular",
                table: "Proveedores",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CondicionIVA",
                table: "Proveedores",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CondicionPago",
                table: "Proveedores",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactoPrincipal",
                table: "Proveedores",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Localidad",
                table: "Proveedores",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NombreFantasia",
                table: "Proveedores",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notas",
                table: "Proveedores",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Provincia",
                table: "Proveedores",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Web",
                table: "Proveedores",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PermiteVentaSinStock",
                table: "Productos",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "PrecioVentaMayorista",
                table: "Productos",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StockMaximo",
                table: "Productos",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UnidadMedida",
                table: "Productos",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NumeroFactura",
                table: "MovimientosStock",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioId",
                table: "MovimientosStock",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "PorcentajeAjuste",
                table: "ListasPrecios",
                type: "decimal(8,4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "ListasPrecios",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Descripcion",
                table: "ListasPrecios",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tipo",
                table: "ListasPrecios",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Barrio",
                table: "Clientes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CUIL",
                table: "Clientes",
                type: "TEXT",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CodigoPostal",
                table: "Clientes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EstadoCivil",
                table: "Clientes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaNacimiento",
                table: "Clientes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FiadorDNI",
                table: "Clientes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FiadorDireccion",
                table: "Clientes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FiadorNombre",
                table: "Clientes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FiadorTelefono",
                table: "Clientes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "LimiteCredito",
                table: "Clientes",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "ListaPrecioId",
                table: "Clientes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Localidad",
                table: "Clientes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Ocupacion",
                table: "Clientes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Provincia",
                table: "Clientes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sexo",
                table: "Clientes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TelefonoAlternativo",
                table: "Clientes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TipoCliente",
                table: "Clientes",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "VendedorAsignadoId",
                table: "Clientes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "CategoriasProducto",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ListasPreciosItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ListaPrecioId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProductoId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CategoriaId = table.Column<Guid>(type: "TEXT", nullable: true),
                    PrecioFijo = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PorcentajeOverride = table.Column<decimal>(type: "decimal(8,4)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListasPreciosItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ListasPreciosItems_CategoriasProducto_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "CategoriasProducto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ListasPreciosItems_ListasPrecios_ListaPrecioId",
                        column: x => x.ListaPrecioId,
                        principalTable: "ListasPrecios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ListasPreciosItems_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ListasPreciosItems_CategoriaId",
                table: "ListasPreciosItems",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_ListasPreciosItems_ListaPrecioId",
                table: "ListasPreciosItems",
                column: "ListaPrecioId");

            migrationBuilder.CreateIndex(
                name: "IX_ListasPreciosItems_ProductoId",
                table: "ListasPreciosItems",
                column: "ProductoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Productos_CategoriasProducto_CategoriaId",
                table: "Productos",
                column: "CategoriaId",
                principalTable: "CategoriasProducto",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Productos_Proveedores_ProveedorId",
                table: "Productos",
                column: "ProveedorId",
                principalTable: "Proveedores",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_VentaItems_Productos_ProductoId",
                table: "VentaItems",
                column: "ProductoId",
                principalTable: "Productos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Ventas_Clientes_ClienteId",
                table: "Ventas",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Productos_CategoriasProducto_CategoriaId",
                table: "Productos");

            migrationBuilder.DropForeignKey(
                name: "FK_Productos_Proveedores_ProveedorId",
                table: "Productos");

            migrationBuilder.DropForeignKey(
                name: "FK_VentaItems_Productos_ProductoId",
                table: "VentaItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Ventas_Clientes_ClienteId",
                table: "Ventas");

            migrationBuilder.DropTable(
                name: "ListasPreciosItems");

            migrationBuilder.DropColumn(
                name: "Descuento",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "ListaPrecioId",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "NumeroComprobante",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "Observaciones",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "SubTotal",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "PrecioBase",
                table: "VentaItems");

            migrationBuilder.DropColumn(
                name: "Celular",
                table: "Proveedores");

            migrationBuilder.DropColumn(
                name: "CondicionIVA",
                table: "Proveedores");

            migrationBuilder.DropColumn(
                name: "CondicionPago",
                table: "Proveedores");

            migrationBuilder.DropColumn(
                name: "ContactoPrincipal",
                table: "Proveedores");

            migrationBuilder.DropColumn(
                name: "Localidad",
                table: "Proveedores");

            migrationBuilder.DropColumn(
                name: "NombreFantasia",
                table: "Proveedores");

            migrationBuilder.DropColumn(
                name: "Notas",
                table: "Proveedores");

            migrationBuilder.DropColumn(
                name: "Provincia",
                table: "Proveedores");

            migrationBuilder.DropColumn(
                name: "Web",
                table: "Proveedores");

            migrationBuilder.DropColumn(
                name: "PermiteVentaSinStock",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "PrecioVentaMayorista",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "StockMaximo",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "UnidadMedida",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "NumeroFactura",
                table: "MovimientosStock");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "MovimientosStock");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "ListasPrecios");

            migrationBuilder.DropColumn(
                name: "Descripcion",
                table: "ListasPrecios");

            migrationBuilder.DropColumn(
                name: "Tipo",
                table: "ListasPrecios");

            migrationBuilder.DropColumn(
                name: "Barrio",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "CUIL",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "CodigoPostal",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "EstadoCivil",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "FechaNacimiento",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "FiadorDNI",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "FiadorDireccion",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "FiadorNombre",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "FiadorTelefono",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "LimiteCredito",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "ListaPrecioId",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "Localidad",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "Ocupacion",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "Provincia",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "Sexo",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "TelefonoAlternativo",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "TipoCliente",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "VendedorAsignadoId",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "CategoriasProducto");

            migrationBuilder.AlterColumn<decimal>(
                name: "PorcentajeAjuste",
                table: "ListasPrecios",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(8,4)");

            migrationBuilder.AddForeignKey(
                name: "FK_Productos_CategoriasProducto_CategoriaId",
                table: "Productos",
                column: "CategoriaId",
                principalTable: "CategoriasProducto",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Productos_Proveedores_ProveedorId",
                table: "Productos",
                column: "ProveedorId",
                principalTable: "Proveedores",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VentaItems_Productos_ProductoId",
                table: "VentaItems",
                column: "ProductoId",
                principalTable: "Productos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ventas_Clientes_ClienteId",
                table: "Ventas",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id");
        }
    }
}
