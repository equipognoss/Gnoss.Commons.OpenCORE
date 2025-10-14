using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Es.Riam.Gnoss.AD.Migrations.EntityContextPostgresMigrations
{
    /// <inheritdoc />
    public partial class RolOntologiaPermiso : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Rol_ProyectoID_OrganizacionID",
                table: "Rol");

            migrationBuilder.CreateTable(
                name: "RolOntologiaPermiso",
                columns: table => new
                {
                    DocumentoID = table.Column<Guid>(type: "uuid", nullable: false),
                    RolID = table.Column<Guid>(type: "uuid", nullable: false),
                    Permisos = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolOntologiaPermiso", x => new { x.DocumentoID, x.RolID });
                    table.ForeignKey(
                        name: "FK_RolOntologiaPermiso_Documento_DocumentoID",
                        column: x => x.DocumentoID,
                        principalTable: "Documento",
                        principalColumn: "DocumentoID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolOntologiaPermiso_Rol_RolID",
                        column: x => x.RolID,
                        principalTable: "Rol",
                        principalColumn: "RolID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rol_OrganizacionID_ProyectoID",
                table: "Rol",
                columns: new[] { "OrganizacionID", "ProyectoID" });

            migrationBuilder.CreateIndex(
                name: "IX_RolOntologiaPermiso_RolID",
                table: "RolOntologiaPermiso",
                column: "RolID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RolOntologiaPermiso");

            migrationBuilder.DropIndex(
                name: "IX_Rol_OrganizacionID_ProyectoID",
                table: "Rol");

            migrationBuilder.CreateIndex(
                name: "IX_Rol_ProyectoID_OrganizacionID",
                table: "Rol",
                columns: new[] { "ProyectoID", "OrganizacionID" });
        }
    }
}
