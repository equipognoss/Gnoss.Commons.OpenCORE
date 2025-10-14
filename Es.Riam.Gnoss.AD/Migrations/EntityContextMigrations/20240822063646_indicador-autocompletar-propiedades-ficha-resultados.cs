using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Es.Riam.Gnoss.AD.Migrations.EntityContextMigrations
{
    public partial class indicadorautocompletarpropiedadesficharesultados : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "MostrarEnAutocompletar",
                table: "PresentacionMosaicoSemantico",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "MostrarEnAutocompletar",
                table: "PresentacionMapaSemantico",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "MostrarEnAutocompletar",
                table: "PresentacionListadoSemantico",
                type: "bit",
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
