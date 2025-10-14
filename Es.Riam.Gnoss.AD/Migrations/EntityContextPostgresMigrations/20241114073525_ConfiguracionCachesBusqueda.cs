using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Es.Riam.Gnoss.AD.Migrations.EntityContextPostgresMigrations
{
    public partial class ConfiguracionCachesBusqueda : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConfiguracionCachesCostosas",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "uuid", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "uuid", nullable: false),
                    CachesDeBusquedasActivas = table.Column<bool>(type: "boolean", nullable: false),
                    CachesAnonimas = table.Column<bool>(type: "boolean", nullable: false),
                    TiempoExpiracion = table.Column<long>(type: "bigint", nullable: false),
                    TiempoExpiracionCachesDeUsuario = table.Column<long>(type: "bigint", nullable: false),
                    DuracionConsulta = table.Column<long>(type: "bigint", nullable: false),
                    TiempoRecalcularCaches = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracionCachesCostosas", x => new { x.OrganizacionID, x.ProyectoID });
                    table.ForeignKey(
                        name: "FK_ConfiguracionCachesCostosas_Proyecto_OrganizacionID_Proyect~",
                        columns: x => new { x.OrganizacionID, x.ProyectoID },
                        principalTable: "Proyecto",
                        principalColumns: new[] { "OrganizacionID", "ProyectoID" },
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfiguracionCachesCostosas");
        }
    }
}
