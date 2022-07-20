using Microsoft.EntityFrameworkCore.Migrations;

namespace Es.Riam.Gnoss.AD.Migrations.EntityContextMigrations
{
    public partial class AumentoTamanyoMaxNombreCategoriaCookie : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "CategoriaProyectoCookie",
                type: "nvarchar(1000)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "CategoriaProyectoCookie",
                type: "nvarchar(100)");
        }
    }
}
