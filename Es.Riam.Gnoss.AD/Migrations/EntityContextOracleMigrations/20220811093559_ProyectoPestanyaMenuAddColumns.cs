using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Es.Riam.Gnoss.AD.Migrations.EntityContextOracleMigrations
{
    public partial class ProyectoPestanyaMenuAddColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "ProyectoPestanyaMenu",
                type: "TIMESTAMP(7)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaModificacion",
                table: "ProyectoPestanyaMenu",
                type: "TIMESTAMP(7)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UltimoEditor",
                table: "ProyectoPestanyaMenu",
                type: "NVARCHAR2(2000)",
                nullable: true);

            

            migrationBuilder.AddColumn<string>(
                name: "FacetaObjetoConocimientoProyectoFaceta",
                table: "FacetaHome",
                type: "NVARCHAR2(300)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FacetaObjetoConocimientoProyectoObjetoConocimiento",
                table: "FacetaHome",
                type: "NVARCHAR2(50)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "FacetaObjetoConocimientoProyectoOrganizacionID",
                table: "FacetaHome",
                type: "RAW(16)",
                nullable: true);


			migrationBuilder.AddColumn<Guid>(
				name: "FacetaObjetoConocimientoProyectoProyectoID",
				table: "FacetaHome",
				type: "RAW(16)",
				nullable: true);
		}

        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "ProyectoPestanyaMenu");

            migrationBuilder.DropColumn(
                name: "FechaModificacion",
                table: "ProyectoPestanyaMenu");

            migrationBuilder.DropColumn(
                name: "UltimoEditor",
                table: "ProyectoPestanyaMenu");

            migrationBuilder.DropColumn(
                name: "FacetaObjetoConocimientoProyectoFaceta",
                table: "FacetaHome");

            migrationBuilder.DropColumn(
                name: "FacetaObjetoConocimientoProyectoObjetoConocimiento",
                table: "FacetaHome");

            migrationBuilder.DropColumn(
                name: "FacetaObjetoConocimientoProyectoOrganizacionID",
                table: "FacetaHome");

            migrationBuilder.DropColumn(
                name: "FacetaObjetoConocimientoProyectoProyectoID",
                table: "FacetaHome");
        }
    }
}
