using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Es.Riam.Gnoss.AD.Migrations.EntityContextOracleMigrations
{
    /// <inheritdoc />
    public partial class VersionEstructuraPaginaCMS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProyectoPestanyaVersionCMS",
                columns: table => new
                {
                    VersionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    PestanyaID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    IdentidadID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    VersionAnterior = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Fecha = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Comentario = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    ModeloJSON = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProyectoPestanyaVersionCMS", x => x.VersionID);
                    table.ForeignKey(
                        name: "FK_ProyectoPestanyaVersionCMS_ProyectoPestanyaMenu_PestanyaID",
                        column: x => x.PestanyaID,
                        principalTable: "ProyectoPestanyaMenu",
                        principalColumn: "PestanyaID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProyectoPestanyaVersionCMS_PestanyaID",
                table: "ProyectoPestanyaVersionCMS",
                column: "PestanyaID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProyectoPestanyaVersionCMS");
        }
    }
}
