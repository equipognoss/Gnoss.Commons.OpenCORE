using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Es.Riam.Gnoss.AD.Migrations.EntityContextPostgresMigrations
{
    public partial class ClaveForaneaDocumentoIdentidad : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "CreadorID",
                table: "Documento",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);            

            migrationBuilder.CreateIndex(
                name: "IX_Documento_CreadorID",
                table: "Documento",
                column: "CreadorID");

            migrationBuilder.AddForeignKey(
                name: "FK_Documento_Identidad_CreadorID",
                table: "Documento",
                column: "CreadorID",
                principalTable: "Identidad",
                principalColumn: "IdentidadID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documento_Identidad_CreadorID",
                table: "Documento");

            migrationBuilder.DropIndex(
                name: "IX_Documento_CreadorID",
                table: "Documento");

            migrationBuilder.DropColumn(
                name: "Ontologia",
                table: "Carga");

            migrationBuilder.AlterColumn<Guid>(
                name: "CreadorID",
                table: "Documento",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "Ontologia",
                table: "CargaPaquete",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");
        }
    }
}
