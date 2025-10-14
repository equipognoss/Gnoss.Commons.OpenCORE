using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Es.Riam.Gnoss.AD.Migrations.EntityContextMigrations
{
    /// <inheritdoc />
    public partial class VersionComponenteCMS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CMSComponenteVersion",
                columns: table => new
                {
                    VersionID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ComponenteID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdentidadID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VersionAnterior = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Comentario = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModeloJSON = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CMSComponenteVersion", x => x.VersionID);
                    table.ForeignKey(
                        name: "FK_CMSComponenteVersion_CMSComponente_ComponenteID",
                        column: x => x.ComponenteID,
                        principalTable: "CMSComponente",
                        principalColumn: "ComponenteID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CMSComponenteVersion_ComponenteID",
                table: "CMSComponenteVersion",
                column: "ComponenteID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CMSComponenteVersion");
        }
    }
}
