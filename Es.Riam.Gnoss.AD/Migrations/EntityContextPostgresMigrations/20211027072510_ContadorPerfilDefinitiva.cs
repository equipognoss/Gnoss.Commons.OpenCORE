using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Es.Riam.Gnoss.AD.Migrations.EntityContextPostgresMigrations
{
    public partial class ContadorPerfilDefinitiva : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaAlta",
                table: "CargaPaquete",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.CreateTable(
                name: "ContadorPerfil",
                columns: table => new
                {
                    PerfilID = table.Column<Guid>(type: "uuid", nullable: false),
                    NumComentarios = table.Column<int>(type: "integer", nullable: false),
                    NumComentContribuciones = table.Column<int>(type: "integer", nullable: false),
                    NumComentMisRec = table.Column<int>(type: "integer", nullable: false),
                    NumComentBlog = table.Column<int>(type: "integer", nullable: false),
                    NumNuevosMensajes = table.Column<int>(type: "integer", nullable: false),
                    NuevosComentarios = table.Column<int>(type: "integer", nullable: false),
                    FechaVisitaComentarios = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    NuevasInvitaciones = table.Column<int>(type: "integer", nullable: false),
                    NuevasSuscripciones = table.Column<int>(type: "integer", nullable: false),
                    FechaVisitaSuscripciones = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    NumMensajesSinLeer = table.Column<int>(type: "integer", nullable: false),
                    NumInvitacionesSinLeer = table.Column<int>(type: "integer", nullable: false),
                    NumSuscripcionesSinLeer = table.Column<int>(type: "integer", nullable: false),
                    NumComentariosSinLeer = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContadorPerfil", x => x.PerfilID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContadorPerfil");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaAlta",
                table: "CargaPaquete",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);
        }
    }
}
