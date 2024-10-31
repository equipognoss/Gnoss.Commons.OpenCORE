using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Es.Riam.Gnoss.AD.Migrations.EntityContextOracleMigrations
{
    public partial class AddClausulaOrderByProyectoPestanyaFiltroOrden : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Consulta",
                table: "ProyectoPestanyaFiltroOrdenRecursos",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrderBy",
                table: "ProyectoPestanyaFiltroOrdenRecursos",
                type: "NVARCHAR2(2000)",
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
