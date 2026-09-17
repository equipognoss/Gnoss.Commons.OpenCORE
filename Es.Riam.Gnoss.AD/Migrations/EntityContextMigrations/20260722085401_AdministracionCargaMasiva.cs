using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Es.Riam.Gnoss.AD.Migrations.EntityContextMigrations
{
    /// <inheritdoc />
    public partial class AdministracionCargaMasiva : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CargaMasivaConfiguracion",
                columns: table => new
                {
                    ProyectoID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdentidadID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CargaMasivaConfiguracion", x => x.ProyectoID);
                });

            migrationBuilder.CreateTable(
                name: "CargaMasivaDominioPermitido",
                columns: table => new
                {
                    ProyectoID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Dominio = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdentidadID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CargaMasivaDominioPermitido", x => new { x.ProyectoID, x.Dominio });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CargaMasivaConfiguracion");

            migrationBuilder.DropTable(
                name: "CargaMasivaDominioPermitido");
        }
    }
}
