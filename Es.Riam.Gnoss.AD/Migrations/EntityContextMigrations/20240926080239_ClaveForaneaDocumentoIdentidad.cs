using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Es.Riam.Gnoss.AD.Migrations.EntityContextMigrations
{
    public partial class ClaveForaneaDocumentoIdentidad : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "CreadorID",
                table: "Documento",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
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

            migrationBuilder.AlterColumn<Guid>(
                name: "CreadorID",
                table: "Documento",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");
        }
    }
}
