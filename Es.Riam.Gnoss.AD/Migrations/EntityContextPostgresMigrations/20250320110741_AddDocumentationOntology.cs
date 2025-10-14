using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Es.Riam.Gnoss.AD.Migrations.EntityContextPostgresMigrations
{
    /// <inheritdoc />
    public partial class AddDocumentationOntology : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DetallesDocumentacion",
                columns: table => new
                {
                    ProyectoID = table.Column<Guid>(type: "uuid", nullable: false),
                    Titulo = table.Column<string>(type: "text", nullable: true),
                    Autores = table.Column<string>(type: "text", nullable: true),
                    OntologiasImportadas = table.Column<string>(type: "text", nullable: true),
                    EnlaceOWL = table.Column<string>(type: "text", nullable: true),
                    Licencia = table.Column<string>(type: "text", nullable: true),
                    UrlLicencia = table.Column<string>(type: "text", nullable: true),
                    Descripcion = table.Column<string>(type: "text", nullable: true),
                    Privado = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetallesDocumentacion", x => x.ProyectoID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DetallesDocumentacion");
        }
    }
}
