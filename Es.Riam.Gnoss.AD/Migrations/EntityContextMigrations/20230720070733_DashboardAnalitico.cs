using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Es.Riam.Gnoss.AD.Migrations.EntityContextMigrations
{
    public partial class DashboardAnalitico : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProyectoPestanyaDashboardAsistente",
                columns: table => new
                {
                    AsisID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PestanyaID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Labels = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Select = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Where = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropExtra = table.Column<bool>(type: "bit", nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    Tamanyo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Titulo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProyectoPestanyaDashboardAsistente", x => x.AsisID);
                    table.ForeignKey(
                        name: "FK_ProyectoPestanyaDashboardAsistente_ProyectoPestanyaMenu_PestanyaID",
                        column: x => x.PestanyaID,
                        principalTable: "ProyectoPestanyaMenu",
                        principalColumn: "PestanyaID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProyectoPestanyaDashboardAsistenteDataset",
                columns: table => new
                {
                    DatasetID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AsisID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Datos = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Orden = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProyectoPestanyaDashboardAsistenteDataset", x => x.DatasetID);
                    table.ForeignKey(
                        name: "FK_ProyectoPestanyaDashboardAsistenteDataset_ProyectoPestanyaDashboardAsistente_AsisID",
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
