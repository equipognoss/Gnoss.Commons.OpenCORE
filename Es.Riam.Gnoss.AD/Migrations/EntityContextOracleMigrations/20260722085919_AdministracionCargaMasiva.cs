using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Es.Riam.Gnoss.AD.Migrations.EntityContextOracleMigrations
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
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    IdentidadID = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CargaMasivaConfiguracion", x => x.ProyectoID);
                });

            migrationBuilder.CreateTable(
                name: "CargaMasivaDominioPermitido",
                columns: table => new
                {
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Dominio = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    IdentidadID = table.Column<Guid>(type: "RAW(16)", nullable: false)
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
