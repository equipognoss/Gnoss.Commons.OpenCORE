using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Es.Riam.Gnoss.AD.Migrations.EntityContextPostgresMigrations
{
    public partial class pesoocbusquedas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProyectoPestanyaBusquedaPesoOC",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "uuid", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "uuid", nullable: false),
                    OntologiaProyecto = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PestanyaID = table.Column<Guid>(type: "uuid", nullable: false),
                    Tipo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Peso = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProyectoPestanyaBusquedaPesoOC", x => new { x.OrganizacionID, x.ProyectoID, x.OntologiaProyecto, x.PestanyaID, x.Tipo });
                    table.ForeignKey(
                        name: "FK_ProyectoPestanyaBusquedaPesoOC_OntologiaProyecto_Organizaci~",
                        columns: x => new { x.OrganizacionID, x.ProyectoID, x.OntologiaProyecto },
                        principalTable: "OntologiaProyecto",
                        principalColumns: new[] { "OrganizacionID", "ProyectoID", "OntologiaProyecto" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProyectoPestanyaBusquedaPesoOC_ProyectoPestanyaBusqueda_Pes~",
                        column: x => x.PestanyaID,
                        principalTable: "ProyectoPestanyaBusqueda",
                        principalColumn: "PestanyaID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProyectoPestanyaBusquedaPesoOC_PestanyaID",
                table: "ProyectoPestanyaBusquedaPesoOC",
                column: "PestanyaID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProyectoPestanyaBusquedaPesoOC");
        }
    }
}
