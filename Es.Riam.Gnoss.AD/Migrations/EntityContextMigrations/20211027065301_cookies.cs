using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Es.Riam.Gnoss.AD.Migrations.EntityContextMigrations
{
    public partial class cookies : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaAlta",
                table: "CargaPaquete",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.CreateTable(
                name: "CategoriaProyectoCookie",
                columns: table => new
                {
                    CategoriaID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", maxLength: 100, nullable: true),
                    NombreCorto = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EsCategoriaTecnica = table.Column<bool>(type: "bit", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizacionID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
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
                name: "ContadorPerfil",
                columns: table => new
                {
                    PerfilID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NumComentarios = table.Column<int>(type: "int", nullable: false),
                    NumComentContribuciones = table.Column<int>(type: "int", nullable: false),
                    NumComentMisRec = table.Column<int>(type: "int", nullable: false),
                    NumComentBlog = table.Column<int>(type: "int", nullable: false),
                    NumNuevosMensajes = table.Column<int>(type: "int", nullable: false),
                    NuevosComentarios = table.Column<int>(type: "int", nullable: false),
                    FechaVisitaComentarios = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NuevasInvitaciones = table.Column<int>(type: "int", nullable: false),
                    NuevasSuscripciones = table.Column<int>(type: "int", nullable: false),
                    FechaVisitaSuscripciones = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NumMensajesSinLeer = table.Column<int>(type: "int", nullable: false),
                    NumInvitacionesSinLeer = table.Column<int>(type: "int", nullable: false),
                    NumSuscripcionesSinLeer = table.Column<int>(type: "int", nullable: false),
                    NumComentariosSinLeer = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContadorPerfil", x => x.PerfilID);
                });
            
            migrationBuilder.CreateTable(
                name: "ProyectoCookie",
                columns: table => new
                {
                    CookieID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", maxLength: 100, nullable: true),
                    NombreCorto = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Tipo = table.Column<short>(type: "smallint", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EsEditable = table.Column<bool>(type: "bit", nullable: false),
                    CategoriaID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizacionID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
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
                        principalColumns: new[] { "ProyectoID", "OrganizacionID" },
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
                name: "ContadorPerfil");

            migrationBuilder.DropTable(
                name: "ProyectoCookie");

            migrationBuilder.DropTable(
                name: "CategoriaProyectoCookie");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaAlta",
                table: "CargaPaquete",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
