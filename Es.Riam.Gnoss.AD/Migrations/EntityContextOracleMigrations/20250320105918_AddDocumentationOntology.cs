using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Es.Riam.Gnoss.AD.Migrations.EntityContextOracleMigrations
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
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Titulo = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Autores = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    OntologiasImportadas = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    EnlaceOWL = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Licencia = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    UrlLicencia = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Descripcion = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Privado = table.Column<bool>(type: "NUMBER(1)", nullable: false)
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
