using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Es.Riam.Gnoss.AD.Migrations.EntityContextPostgresMigrations
{
    /// <inheritdoc />
    public partial class EstadosConVersiones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "EstadoID",
                table: "VersionDocumento",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_VersionDocumento_EstadoID",
                table: "VersionDocumento",
                column: "EstadoID");

            migrationBuilder.AddForeignKey(
                name: "FK_VersionDocumento_Estado_EstadoID",
                table: "VersionDocumento",
                column: "EstadoID",
                principalTable: "Estado",
                principalColumn: "EstadoID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VersionDocumento_Estado_EstadoID",
                table: "VersionDocumento");

            migrationBuilder.DropIndex(
                name: "IX_VersionDocumento_EstadoID",
                table: "VersionDocumento");

            migrationBuilder.DropColumn(
                name: "EstadoID",
                table: "VersionDocumento");
        }
    }
}
