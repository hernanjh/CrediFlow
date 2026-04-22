using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrediFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FinalStructuralChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TipoCliente",
                table: "Clientes");

            migrationBuilder.RenameColumn(
                name: "MetodoPago",
                table: "Ventas",
                newName: "FormaCobroId");

            migrationBuilder.RenameColumn(
                name: "UnidadMedida",
                table: "Productos",
                newName: "UnidadMedidaId");

            migrationBuilder.RenameColumn(
                name: "Ocupacion",
                table: "Clientes",
                newName: "TipoClienteId");

            migrationBuilder.AlterColumn<decimal>(
                name: "LimiteCredito",
                table: "Clientes",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<Guid>(
                name: "OcupacionId",
                table: "Clientes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SectorId",
                table: "Clientes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FormasCobro",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Nombre = table.Column<string>(type: "TEXT", nullable: false),
                    Activa = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormasCobro", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sectores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Nombre = table.Column<string>(type: "TEXT", nullable: false),
                    Activa = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sectores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TiposCliente",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Nombre = table.Column<string>(type: "TEXT", nullable: false),
                    Activa = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposCliente", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UnidadesMedida",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Nombre = table.Column<string>(type: "TEXT", nullable: false),
                    Simbolo = table.Column<string>(type: "TEXT", nullable: false),
                    Activa = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnidadesMedida", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FormasCobroVenta",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    VentaId = table.Column<Guid>(type: "TEXT", nullable: false),
                    FormaCobroId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormasCobroVenta", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FormasCobroVenta_FormasCobro_FormaCobroId",
                        column: x => x.FormaCobroId,
                        principalTable: "FormasCobro",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FormasCobroVenta_Ventas_VentaId",
                        column: x => x.VentaId,
                        principalTable: "Ventas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_FormaCobroId",
                table: "Ventas",
                column: "FormaCobroId");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_UnidadMedidaId",
                table: "Productos",
                column: "UnidadMedidaId");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_OcupacionId",
                table: "Clientes",
                column: "OcupacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_SectorId",
                table: "Clientes",
                column: "SectorId");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_TipoClienteId",
                table: "Clientes",
                column: "TipoClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_FormasCobroVenta_FormaCobroId",
                table: "FormasCobroVenta",
                column: "FormaCobroId");

            migrationBuilder.CreateIndex(
                name: "IX_FormasCobroVenta_VentaId",
                table: "FormasCobroVenta",
                column: "VentaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Clientes_Ocupaciones_OcupacionId",
                table: "Clientes",
                column: "OcupacionId",
                principalTable: "Ocupaciones",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Clientes_Sectores_SectorId",
                table: "Clientes",
                column: "SectorId",
                principalTable: "Sectores",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Clientes_TiposCliente_TipoClienteId",
                table: "Clientes",
                column: "TipoClienteId",
                principalTable: "TiposCliente",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Productos_UnidadesMedida_UnidadMedidaId",
                table: "Productos",
                column: "UnidadMedidaId",
                principalTable: "UnidadesMedida",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Ventas_FormasCobro_FormaCobroId",
                table: "Ventas",
                column: "FormaCobroId",
                principalTable: "FormasCobro",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clientes_Ocupaciones_OcupacionId",
                table: "Clientes");

            migrationBuilder.DropForeignKey(
                name: "FK_Clientes_Sectores_SectorId",
                table: "Clientes");

            migrationBuilder.DropForeignKey(
                name: "FK_Clientes_TiposCliente_TipoClienteId",
                table: "Clientes");

            migrationBuilder.DropForeignKey(
                name: "FK_Productos_UnidadesMedida_UnidadMedidaId",
                table: "Productos");

            migrationBuilder.DropForeignKey(
                name: "FK_Ventas_FormasCobro_FormaCobroId",
                table: "Ventas");

            migrationBuilder.DropTable(
                name: "FormasCobroVenta");

            migrationBuilder.DropTable(
                name: "Sectores");

            migrationBuilder.DropTable(
                name: "TiposCliente");

            migrationBuilder.DropTable(
                name: "UnidadesMedida");

            migrationBuilder.DropTable(
                name: "FormasCobro");

            migrationBuilder.DropIndex(
                name: "IX_Ventas_FormaCobroId",
                table: "Ventas");

            migrationBuilder.DropIndex(
                name: "IX_Productos_UnidadMedidaId",
                table: "Productos");

            migrationBuilder.DropIndex(
                name: "IX_Clientes_OcupacionId",
                table: "Clientes");

            migrationBuilder.DropIndex(
                name: "IX_Clientes_SectorId",
                table: "Clientes");

            migrationBuilder.DropIndex(
                name: "IX_Clientes_TipoClienteId",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "OcupacionId",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "SectorId",
                table: "Clientes");

            migrationBuilder.RenameColumn(
                name: "FormaCobroId",
                table: "Ventas",
                newName: "MetodoPago");

            migrationBuilder.RenameColumn(
                name: "UnidadMedidaId",
                table: "Productos",
                newName: "UnidadMedida");

            migrationBuilder.RenameColumn(
                name: "TipoClienteId",
                table: "Clientes",
                newName: "Ocupacion");

            migrationBuilder.AlterColumn<decimal>(
                name: "LimiteCredito",
                table: "Clientes",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "TEXT");

            migrationBuilder.AddColumn<string>(
                name: "TipoCliente",
                table: "Clientes",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
