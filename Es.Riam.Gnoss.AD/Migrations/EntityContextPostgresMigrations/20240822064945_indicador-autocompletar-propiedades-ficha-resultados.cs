using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Es.Riam.Gnoss.AD.Migrations.EntityContextPostgresMigrations
{
    public partial class indicadorautocompletarpropiedadesficharesultados : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "MostrarEnAutocompletar",
                table: "PresentacionMosaicoSemantico",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "MostrarEnAutocompletar",
                table: "PresentacionMapaSemantico",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "MostrarEnAutocompletar",
                table: "PresentacionListadoSemantico",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MostrarEnAutocompletar",
                table: "PresentacionMosaicoSemantico");

            migrationBuilder.DropColumn(
                name: "MostrarEnAutocompletar",
                table: "PresentacionMapaSemantico");

            migrationBuilder.DropColumn(
                name: "MostrarEnAutocompletar",
                table: "PresentacionListadoSemantico");
        }
    }
}
