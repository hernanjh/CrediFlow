using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrediFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ZonasColorHex_Activa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ColorHex",
                table: "Zonas",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ColorHex",
                table: "Zonas");
        }
    }
}
