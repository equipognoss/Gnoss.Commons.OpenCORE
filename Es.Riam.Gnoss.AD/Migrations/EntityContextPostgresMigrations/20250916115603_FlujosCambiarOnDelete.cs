using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Es.Riam.Gnoss.AD.Migrations.EntityContextPostgresMigrations
{
    /// <inheritdoc />
    public partial class FlujosCambiarOnDelete : Migration
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
