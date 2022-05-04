using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Es.Riam.Gnoss.AD.Migrations.EntityContextPostgresMigrations
{
    public partial class AddTableUltimosDocumentosVisitadosPostgres : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UltimosDocumentosVisitados",
                columns: table => new
                {
                    ProyectoID = table.Column<Guid>(type: "uuid", nullable: false),
                    Documentos = table.Column<string>(type: "text", nullable: true),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UltimosDocumentosVisitados", x => x.ProyectoID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UltimosDocumentosVisitados");
        }
    }
}
