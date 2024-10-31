using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Es.Riam.Gnoss.AD.Migrations.EntityContextPostgresMigrations
{
    public partial class AddClausulaOrderByProyectoPestanyaFiltroOrden : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Consulta",
                table: "ProyectoPestanyaFiltroOrdenRecursos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrderBy",
                table: "ProyectoPestanyaFiltroOrdenRecursos",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Consulta",
                table: "ProyectoPestanyaFiltroOrdenRecursos");

            migrationBuilder.DropColumn(
                name: "OrderBy",
                table: "ProyectoPestanyaFiltroOrdenRecursos");
        }
    }
}
