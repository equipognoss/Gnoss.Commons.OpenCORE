using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Es.Riam.Gnoss.AD.Migrations.EntityContextOracleMigrations
{
    /// <inheritdoc />
    public partial class asistentes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Asistente",
                columns: table => new
                {
                    AsistenteID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Nombre = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Descripcion = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Token = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    HostAsistente = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Icono = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Activo = table.Column<bool>(type: "NUMBER(1)", precision: 1, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Asistente", x => x.AsistenteID);
                    table.ForeignKey(
                        name: "FK_Asistente_Proyecto_OrganizacionID_ProyectoID",
                        columns: x => new { x.OrganizacionID, x.ProyectoID },
                        principalTable: "Proyecto",
                        principalColumns: new[] { "OrganizacionID", "ProyectoID" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AsistenteConfigIdentidad",
                columns: table => new
                {
                    AsistenteID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    IdentidadID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    AsistentePorDefecto = table.Column<bool>(type: "NUMBER(1)", precision: 1, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AsistenteConfigIdentidad", x => new { x.AsistenteID, x.IdentidadID });
                    table.ForeignKey(
                        name: "FK_AsistenteConfigIdentidad_Asistente_AsistenteID",
                        column: x => x.AsistenteID,
                        principalTable: "Asistente",
                        principalColumn: "AsistenteID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AsistenteConfigIdentidad_Identidad_IdentidadID",
                        column: x => x.IdentidadID,
                        principalTable: "Identidad",
                        principalColumn: "IdentidadID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RolAsistente",
                columns: table => new
                {
                    AsistenteID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    RolID = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolAsistente", x => new { x.AsistenteID, x.RolID });
                    table.ForeignKey(
                        name: "FK_RolAsistente_Asistente_AsistenteID",
                        column: x => x.AsistenteID,
                        principalTable: "Asistente",
                        principalColumn: "AsistenteID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RolAsistente_Rol_RolID",
                        column: x => x.RolID,
                        principalTable: "Rol",
                        principalColumn: "RolID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Asistente_OrganizacionID_ProyectoID",
                table: "Asistente",
                columns: new[] { "OrganizacionID", "ProyectoID" });

            migrationBuilder.CreateIndex(
                name: "IX_AsistenteConfigIdentidad_IdentidadID",
                table: "AsistenteConfigIdentidad",
                column: "IdentidadID");

            migrationBuilder.CreateIndex(
                name: "IX_RolAsistente_RolID",
                table: "RolAsistente",
                column: "RolID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AsistenteConfigIdentidad");

            migrationBuilder.DropTable(
                name: "RolAsistente");

            migrationBuilder.DropTable(
                name: "Asistente");
        }
    }
}
