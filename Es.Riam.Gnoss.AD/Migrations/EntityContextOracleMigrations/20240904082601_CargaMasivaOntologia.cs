using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Es.Riam.Gnoss.AD.Migrations.EntityContextOracleMigrations
{
    public partial class CargaMasivaOntologia : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ontologia",
                table: "CargaPaquete");

            migrationBuilder.AddColumn<string>(
                name: "Ontologia",
                table: "Carga",
                type: "NVARCHAR2(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ontologia",
                table: "Carga");

            migrationBuilder.AddColumn<string>(
                name: "Ontologia",
                table: "CargaPaquete",
                type: "NVARCHAR2(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");
        }
    }
}
