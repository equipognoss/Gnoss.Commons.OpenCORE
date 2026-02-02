using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Es.Riam.Gnoss.AD.Migrations.EntityContextOracleMigrations
{
    public partial class DatoExtraProyecto_AddColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NombreCorto",
                table: "DatoExtraProyecto",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "VisiblePerfil",
                table: "DatoExtraProyecto",
                type: "NUMBER(1)",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NombreCorto",
                table: "DatoExtraProyecto");

            migrationBuilder.DropColumn(
                name: "VisiblePerfil",
                table: "DatoExtraProyecto");
        }
    }
}
