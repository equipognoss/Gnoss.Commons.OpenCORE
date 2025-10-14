using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Es.Riam.Gnoss.AD.Migrations.EntityContextMigrations
{
    /// <inheritdoc />
    public partial class VersionPaginas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProyectoPestanyaMenuVersionPagina",
                columns: table => new
                {
                    VersionID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PestanyaID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdentidadID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VersionAnterior = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Comentario = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModeloJSON = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProyectoPestanyaMenuVersionPagina", x => x.VersionID);
                    table.ForeignKey(
                        name: "FK_ProyectoPestanyaMenuVersionPagina_ProyectoPestanyaMenuVersionPagina_VersionAnterior",
                        column: x => x.VersionAnterior,
                        principalTable: "ProyectoPestanyaMenuVersionPagina",
                        principalColumn: "VersionID");
                    table.ForeignKey(
                        name: "FK_ProyectoPestanyaMenuVersionPagina_ProyectoPestanyaMenu_PestanyaID",
                        column: x => x.PestanyaID,
                        principalTable: "ProyectoPestanyaMenu",
                        principalColumn: "PestanyaID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProyectoPestanyaMenuVersionPagina_PestanyaID",
                table: "ProyectoPestanyaMenuVersionPagina",
                column: "PestanyaID");

            migrationBuilder.CreateIndex(
                name: "IX_ProyectoPestanyaMenuVersionPagina_VersionAnterior",
                table: "ProyectoPestanyaMenuVersionPagina",
                column: "VersionAnterior");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProyectoPestanyaMenuVersionPagina");
        }
    }
}
