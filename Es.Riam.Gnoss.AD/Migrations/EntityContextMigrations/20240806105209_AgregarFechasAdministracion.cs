using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Es.Riam.Gnoss.AD.Migrations.EntityContextMigrations
{
    public partial class AgregarFechasAdministracion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "VistaVirtualRecursos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaModificacion",
                table: "VistaVirtualRecursos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "VistaVirtualCMS",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaModificacion",
                table: "VistaVirtualCMS",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "VistaVirtual",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaModificacion",
                table: "VistaVirtual",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaActualizacion",
                table: "TextosPersonalizadosPersonalizacion",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "TextosPersonalizadosPersonalizacion",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "ProyectoServicioExterno",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaModificacion",
                table: "ProyectoServicioExterno",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaActualizacion",
                table: "ProyectoGadget",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "ProyectoGadget",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaActualizacion",
                table: "ProyectoCookie",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "ProyectoCookie",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "ProyectoConfigExtraSem",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "FacetaObjetoConocimientoProyecto",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "EcosistemaServicioExterno",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaModificacion",
                table: "EcosistemaServicioExterno",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaActualizacion",
                table: "ClausulaRegistro",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "ClausulaRegistro",
                type: "datetime2",
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
