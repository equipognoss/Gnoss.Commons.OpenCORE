using System;
using Es.Riam.Gnoss.AD.EntityModel;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Es.Riam.Gnoss.AD.Migrations.EntityContextMigrations
{
    public partial class AddtableUltimosDocumentosVisitados : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UltimosDocumentosVisitados",
                columns: table => new
                {
                    ProyectoID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Documentos = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime", nullable: false)
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
