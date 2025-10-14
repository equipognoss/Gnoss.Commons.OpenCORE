using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Es.Riam.Gnoss.AD.Migrations.EntityContextPostgresMigrations
{
    /// <inheritdoc />
    public partial class RolesYPermisos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Rol",
                columns: table => new
                {
                    RolID = table.Column<Guid>(type: "uuid", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "uuid", nullable: false),
                    OrganizacionID = table.Column<Guid>(type: "uuid", nullable: false),
                    Tipo = table.Column<short>(type: "smallint", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: true),
                    Descripcion = table.Column<string>(type: "text", nullable: true),
                    PermisosAdministracion = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    PermisosContenidos = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    PermisosRecursos = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rol", x => x.RolID);
                    table.ForeignKey(
                        name: "FK_Rol_Proyecto_ProyectoID_OrganizacionID",
                        columns: x => new {x.OrganizacionID, x.ProyectoID },
                        principalTable: "Proyecto",
                        principalColumns: new[] { "OrganizacionID", "ProyectoID" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RolEcosistema",
                columns: table => new
                {
                    RolID = table.Column<Guid>(type: "uuid", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: true),
                    Descripcion = table.Column<string>(type: "text", nullable: true),
                    Permisos = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolEcosistema", x => x.RolID);
                });

            migrationBuilder.CreateTable(
                name: "RolGrupoIdentidades",
                columns: table => new
                {
                    RolID = table.Column<Guid>(type: "uuid", nullable: false),
                    GrupoID = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolGrupoIdentidades", x => new { x.RolID, x.GrupoID });
                    table.ForeignKey(
                        name: "FK_RolGrupoIdentidades_GrupoIdentidades_GrupoID",
                        column: x => x.GrupoID,
                        principalTable: "GrupoIdentidades",
                        principalColumn: "GrupoID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolGrupoIdentidades_Rol_RolID",
                        column: x => x.RolID,
                        principalTable: "Rol",
                        principalColumn: "RolID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RolIdentidad",
                columns: table => new
                {
                    RolID = table.Column<Guid>(type: "uuid", nullable: false),
                    IdentidadID = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolIdentidad", x => new { x.RolID, x.IdentidadID });
                    table.ForeignKey(
                        name: "FK_RolIdentidad_Identidad_IdentidadID",
                        column: x => x.IdentidadID,
                        principalTable: "Identidad",
                        principalColumn: "IdentidadID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolIdentidad_Rol_RolID",
                        column: x => x.RolID,
                        principalTable: "Rol",
                        principalColumn: "RolID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RolEcosistemaUsuario",
                columns: table => new
                {
                    RolID = table.Column<Guid>(type: "uuid", nullable: false),
                    UsuarioID = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolEcosistemaUsuario", x => new { x.RolID, x.UsuarioID });
                    table.ForeignKey(
                        name: "FK_RolEcosistemaUsuario_RolEcosistema_RolID",
                        column: x => x.RolID,
                        principalTable: "RolEcosistema",
                        principalColumn: "RolID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolEcosistemaUsuario_Usuario_UsuarioID",
                        column: x => x.UsuarioID,
                        principalTable: "Usuario",
                        principalColumn: "UsuarioID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rol_ProyectoID_OrganizacionID",
                table: "Rol",
                columns: new[] { "ProyectoID", "OrganizacionID" });

            migrationBuilder.CreateIndex(
                name: "IX_RolEcosistemaUsuario_UsuarioID",
                table: "RolEcosistemaUsuario",
                column: "UsuarioID");

            migrationBuilder.CreateIndex(
                name: "IX_RolGrupoIdentidades_GrupoID",
                table: "RolGrupoIdentidades",
                column: "GrupoID");

            migrationBuilder.CreateIndex(
                name: "IX_RolIdentidad_IdentidadID",
                table: "RolIdentidad",
                column: "IdentidadID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RolEcosistemaUsuario");

            migrationBuilder.DropTable(
                name: "RolGrupoIdentidades");

            migrationBuilder.DropTable(
                name: "RolIdentidad");

            migrationBuilder.DropTable(
                name: "RolEcosistema");

            migrationBuilder.DropTable(
                name: "Rol");
        }
    }
}
