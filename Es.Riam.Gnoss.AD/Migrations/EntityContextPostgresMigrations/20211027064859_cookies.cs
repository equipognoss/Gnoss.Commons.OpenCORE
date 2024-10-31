using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Es.Riam.Gnoss.AD.Migrations.EntityContextPostgresMigrations
{
    public partial class cookies : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CategoriaProyectoCookie",
                columns: table => new
                {
                    CategoriaID = table.Column<Guid>(type: "uuid", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    NombreCorto = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Descripcion = table.Column<string>(type: "text", nullable: true),
                    EsCategoriaTecnica = table.Column<bool>(type: "boolean", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "uuid", nullable: false),
                    OrganizacionID = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriaProyectoCookie", x => x.CategoriaID);
                    table.ForeignKey(
                        name: "FK_CategoriaProyectoCookie_Proyecto_ProyectoID_OrganizacionID",
                        columns: x => new { x.ProyectoID, x.OrganizacionID },
                        principalTable: "Proyecto",
                        principalColumns: new[] { "ProyectoID", "OrganizacionID" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProyectoCookie",
                columns: table => new
                {
                    CookieID = table.Column<Guid>(type: "uuid", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    NombreCorto = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Tipo = table.Column<short>(type: "smallint", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: true),
                    EsEditable = table.Column<bool>(type: "boolean", nullable: false),
                    CategoriaID = table.Column<Guid>(type: "uuid", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "uuid", nullable: false),
                    OrganizacionID = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProyectoCookie", x => x.CookieID);
                    table.ForeignKey(
                        name: "FK_ProyectoCookie_CategoriaProyectoCookie_CategoriaID",
                        column: x => x.CategoriaID,
                        principalTable: "CategoriaProyectoCookie",
                        principalColumn: "CategoriaID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProyectoCookie_Proyecto_ProyectoID_OrganizacionID",
                        columns: x => new { x.ProyectoID, x.OrganizacionID },
                        principalTable: "Proyecto",
                        principalColumns: new[] {"ProyectoID", "OrganizacionID" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoriaProyectoCookie_ProyectoID_OrganizacionID",
                table: "CategoriaProyectoCookie",
                columns: new[] { "ProyectoID", "OrganizacionID" });

            migrationBuilder.CreateIndex(
                name: "IX_ProyectoCookie_CategoriaID",
                table: "ProyectoCookie",
                column: "CategoriaID");

            migrationBuilder.CreateIndex(
                name: "IX_ProyectoCookie_ProyectoID_OrganizacionID",
                table: "ProyectoCookie",
                columns: new[] { "ProyectoID", "OrganizacionID" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProyectoCookie");

            migrationBuilder.DropTable(
                name: "CategoriaProyectoCookie");
        }
    }
}
