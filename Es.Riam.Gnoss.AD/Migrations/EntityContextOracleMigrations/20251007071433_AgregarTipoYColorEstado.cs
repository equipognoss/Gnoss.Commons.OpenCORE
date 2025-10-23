using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Es.Riam.Gnoss.AD.Migrations.EntityContextOracleMigrations
{
    /// <inheritdoc />
    public partial class AgregarTipoYColorEstado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transicion_Estado_EstadoDestinoID",
                table: "Transicion");

            migrationBuilder.DropForeignKey(
                name: "FK_Transicion_Estado_EstadoOrigenID",
                table: "Transicion");

            migrationBuilder.AddColumn<DateTime>(
                name: "Fecha",
                table: "Flujo",
                type: "TIMESTAMP(7)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "Estado",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "Tipo",
                table: "Estado",
                type: "NUMBER(5)",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddForeignKey(
                name: "FK_Transicion_Estado_EstadoDestinoID",
                table: "Transicion",
                column: "EstadoDestinoID",
                principalTable: "Estado",
                principalColumn: "EstadoID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transicion_Estado_EstadoOrigenID",
                table: "Transicion",
                column: "EstadoOrigenID",
                principalTable: "Estado",
                principalColumn: "EstadoID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transicion_Estado_EstadoDestinoID",
                table: "Transicion");

            migrationBuilder.DropForeignKey(
                name: "FK_Transicion_Estado_EstadoOrigenID",
                table: "Transicion");

            migrationBuilder.DropColumn(
                name: "Fecha",
                table: "Flujo");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "Estado");

            migrationBuilder.DropColumn(
                name: "Tipo",
                table: "Estado");

            migrationBuilder.AddForeignKey(
                name: "FK_Transicion_Estado_EstadoDestinoID",
                table: "Transicion",
                column: "EstadoDestinoID",
                principalTable: "Estado",
                principalColumn: "EstadoID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transicion_Estado_EstadoOrigenID",
                table: "Transicion",
                column: "EstadoOrigenID",
                principalTable: "Estado",
                principalColumn: "EstadoID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
