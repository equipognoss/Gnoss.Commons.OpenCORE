using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Es.Riam.Gnoss.AD.Migrations.EntityContextPostgresMigrations
{
    /// <inheritdoc />
    public partial class AgregarTablaIdiomaTraduccionAutomaticaDocumento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IdiomaTraduccionAutomaticaDocumento",
                columns: table => new
                {
                    DocumentoID = table.Column<Guid>(type: "uuid", nullable: false),
                    Idioma = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdiomaTraduccionAutomaticaDocumento", x => new { x.DocumentoID, x.Idioma });
                    table.ForeignKey(
                        name: "FK_IdiomaTraduccionAutomaticaDocumento_Documento_DocumentoID",
                        column: x => x.DocumentoID,
                        principalTable: "Documento",
                        principalColumn: "DocumentoID",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IdiomaTraduccionAutomaticaDocumento");
        }
    }
}
