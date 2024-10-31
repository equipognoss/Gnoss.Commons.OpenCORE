using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Es.Riam.Gnoss.AD.Migrations.EntityContextPostgresMigrations
{
    public partial class DatoExtraProyecto_AddColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NombreCorto",
                table: "DatoExtraProyecto",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "VisiblePerfil",
                table: "DatoExtraProyecto",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NombreCorto",
                table: "DatoExtraProyecto");

            migrationBuilder.DropColumn(
                name: "VisiblePerfil",
                table: "DatoExtraProyecto");
        }
    }
}
