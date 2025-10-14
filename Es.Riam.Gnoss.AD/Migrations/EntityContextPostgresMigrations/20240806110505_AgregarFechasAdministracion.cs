using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Es.Riam.Gnoss.AD.Migrations.EntityContextPostgresMigrations
{
    public partial class AgregarFechasAdministracion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "VistaVirtualRecursos",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaModificacion",
                table: "VistaVirtualRecursos",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "VistaVirtualCMS",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaModificacion",
                table: "VistaVirtualCMS",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "VistaVirtual",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaModificacion",
                table: "VistaVirtual",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaActualizacion",
                table: "TextosPersonalizadosPersonalizacion",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "TextosPersonalizadosPersonalizacion",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "ProyectoServicioExterno",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaModificacion",
                table: "ProyectoServicioExterno",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaActualizacion",
                table: "ProyectoGadget",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "ProyectoGadget",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaActualizacion",
                table: "ProyectoCookie",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "ProyectoCookie",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "ProyectoConfigExtraSem",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "FacetaObjetoConocimientoProyecto",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "EcosistemaServicioExterno",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaModificacion",
                table: "EcosistemaServicioExterno",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaActualizacion",
                table: "ClausulaRegistro",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "ClausulaRegistro",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "VistaVirtualRecursos");

            migrationBuilder.DropColumn(
                name: "FechaModificacion",
                table: "VistaVirtualRecursos");

            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "VistaVirtualCMS");

            migrationBuilder.DropColumn(
                name: "FechaModificacion",
                table: "VistaVirtualCMS");

            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "VistaVirtual");

            migrationBuilder.DropColumn(
                name: "FechaModificacion",
                table: "VistaVirtual");

            migrationBuilder.DropColumn(
                name: "FechaActualizacion",
                table: "TextosPersonalizadosPersonalizacion");

            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "TextosPersonalizadosPersonalizacion");

            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "ProyectoServicioExterno");

            migrationBuilder.DropColumn(
                name: "FechaModificacion",
                table: "ProyectoServicioExterno");

            migrationBuilder.DropColumn(
                name: "FechaActualizacion",
                table: "ProyectoGadget");

            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "ProyectoGadget");

            migrationBuilder.DropColumn(
                name: "FechaActualizacion",
                table: "ProyectoCookie");

            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "ProyectoCookie");

            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "ProyectoConfigExtraSem");

            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "FacetaObjetoConocimientoProyecto");

            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "EcosistemaServicioExterno");

            migrationBuilder.DropColumn(
                name: "FechaModificacion",
                table: "EcosistemaServicioExterno");

            migrationBuilder.DropColumn(
                name: "FechaActualizacion",
                table: "ClausulaRegistro");

            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "ClausulaRegistro");
        }
    }
}
