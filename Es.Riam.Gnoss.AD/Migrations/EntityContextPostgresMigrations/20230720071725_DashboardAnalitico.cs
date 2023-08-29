using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Es.Riam.Gnoss.AD.Migrations.EntityContextPostgresMigrations
{
    public partial class DashboardAnalitico : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProyectoPestanyaDashboardAsistente",
                columns: table => new
                {
                    AsisID = table.Column<Guid>(type: "uuid", nullable: false),
                    PestanyaID = table.Column<Guid>(type: "uuid", nullable: false),
                    Labels = table.Column<string>(type: "text", nullable: true),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Select = table.Column<string>(type: "text", nullable: true),
                    Where = table.Column<string>(type: "text", nullable: true),
                    PropExtra = table.Column<bool>(type: "boolean", nullable: false),
                    Orden = table.Column<int>(type: "integer", nullable: false),
                    Tamanyo = table.Column<string>(type: "text", nullable: true),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    Titulo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProyectoPestanyaDashboardAsistente", x => x.AsisID);
                    table.ForeignKey(
                        name: "FK_ProyectoPestanyaDashboardAsistente_ProyectoPestanyaMenu_Pes~",
                        column: x => x.PestanyaID,
                        principalTable: "ProyectoPestanyaMenu",
                        principalColumn: "PestanyaID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProyectoPestanyaDashboardAsistenteDataset",
                columns: table => new
                {
                    DatasetID = table.Column<Guid>(type: "uuid", nullable: false),
                    AsisID = table.Column<Guid>(type: "uuid", nullable: false),
                    Datos = table.Column<string>(type: "text", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: true),
                    Color = table.Column<string>(type: "text", nullable: true),
                    Orden = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProyectoPestanyaDashboardAsistenteDataset", x => x.DatasetID);
                    table.ForeignKey(
                        name: "FK_ProyectoPestanyaDashboardAsistenteDataset_ProyectoPestanyaD~",
                        column: x => x.AsisID,
                        principalTable: "ProyectoPestanyaDashboardAsistente",
                        principalColumn: "AsisID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProyectoPestanyaDashboardAsistente_PestanyaID",
                table: "ProyectoPestanyaDashboardAsistente",
                column: "PestanyaID");

            migrationBuilder.CreateIndex(
                name: "IX_ProyectoPestanyaDashboardAsistenteDataset_AsisID",
                table: "ProyectoPestanyaDashboardAsistenteDataset",
                column: "AsisID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProyectoPestanyaDashboardAsistenteDataset");

            migrationBuilder.DropTable(
                name: "ProyectoPestanyaDashboardAsistente");
        }
    }
}
