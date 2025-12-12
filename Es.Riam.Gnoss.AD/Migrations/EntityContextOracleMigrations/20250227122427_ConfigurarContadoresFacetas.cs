using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Es.Riam.Gnoss.AD.Migrations.EntityContextOracleMigrations
{
    /// <inheritdoc />
    public partial class ConfigurarContadoresFacetas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "MostrarContador",
                table: "FacetaObjetoConocimientoProyecto",
                type: "NUMBER(1)",
                nullable: false,
                defaultValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MostrarContador",
                table: "FacetaObjetoConocimientoProyecto");
        }
    }
}
