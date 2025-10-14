using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Es.Riam.Gnoss.AD.Migrations.EntityContextOracleMigrations
{
    public partial class tareasBackgroundClave2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TareasSegundoPlano_Proyecto_ProyectoOrganizacionID_ProyectoID1",
                table: "TareasSegundoPlano");

            migrationBuilder.DropIndex(
                name: "IX_TareasSegundoPlano_ProyectoOrganizacionID_ProyectoID1",
                table: "TareasSegundoPlano");

            migrationBuilder.DropColumn(
                name: "ProyectoID1",
                table: "TareasSegundoPlano");

            migrationBuilder.DropColumn(
                name: "ProyectoOrganizacionID",
                table: "TareasSegundoPlano");

            migrationBuilder.CreateIndex(
                name: "IX_TareasSegundoPlano_OrganizacionID_ProyectoID",
                table: "TareasSegundoPlano",
                columns: new[] { "OrganizacionID", "ProyectoID" });

            migrationBuilder.AddForeignKey(
                name: "FK_TareasSegundoPlano_Proyecto_OrganizacionID_ProyectoID",
                table: "TareasSegundoPlano",
                columns: new[] { "OrganizacionID", "ProyectoID" },
                principalTable: "Proyecto",
                principalColumns: new[] { "OrganizacionID", "ProyectoID" },
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TareasSegundoPlano_Proyecto_OrganizacionID_ProyectoID",
                table: "TareasSegundoPlano");

            migrationBuilder.DropIndex(
                name: "IX_TareasSegundoPlano_OrganizacionID_ProyectoID",
                table: "TareasSegundoPlano");

            migrationBuilder.AddColumn<Guid>(
                name: "ProyectoID1",
                table: "TareasSegundoPlano",
                type: "RAW(16)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ProyectoOrganizacionID",
                table: "TareasSegundoPlano",
                type: "RAW(16)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_TareasSegundoPlano_ProyectoOrganizacionID_ProyectoID1",
                table: "TareasSegundoPlano",
                columns: new[] { "ProyectoOrganizacionID", "ProyectoID1" });

            migrationBuilder.AddForeignKey(
                name: "FK_TareasSegundoPlano_Proyecto_ProyectoOrganizacionID_ProyectoID1",
                table: "TareasSegundoPlano",
                columns: new[] { "ProyectoOrganizacionID", "ProyectoID1" },
                principalTable: "Proyecto",
                principalColumns: new[] { "OrganizacionID", "ProyectoID" },
                onDelete: ReferentialAction.Restrict);
        }
    }
}
