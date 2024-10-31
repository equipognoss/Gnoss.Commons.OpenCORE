using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Es.Riam.Gnoss.AD.Migrations.EntityContextMigrations
{
    public partial class CrearIndiceTablaDocumento : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Documento_Tipo_Eliminado_Visibilidad",
                table: "Documento",
                columns: new[] { "Tipo", "Eliminado", "Visibilidad" })
                .Annotation("SqlServer:Include", new[] { "ProyectoID" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Documento_Tipo_Eliminado_Visibilidad",
                table: "Documento");
        }
    }
}
