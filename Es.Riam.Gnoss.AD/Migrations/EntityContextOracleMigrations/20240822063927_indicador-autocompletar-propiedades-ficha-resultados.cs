using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Es.Riam.Gnoss.AD.Migrations.EntityContextOracleMigrations
{
    public partial class indicadorautocompletarpropiedadesficharesultados : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "MostrarEnAutocompletar",
                table: "PresentacionMosaicoSemantico",
                type: "NUMBER(1)",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "MostrarEnAutocompletar",
                table: "PresentacionMapaSemantico",
                type: "NUMBER(1)",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "MostrarEnAutocompletar",
                table: "PresentacionListadoSemantico",
                type: "NUMBER(1)",
                nullable: false,
                defaultValue: 0);
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
