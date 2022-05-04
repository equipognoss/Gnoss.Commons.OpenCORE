using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Es.Riam.Gnoss.AD.Migrations.EntityContextBASEMigrations
{
    public partial class CreateBaseInRDF : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ColaCorreo",
                columns: table => new
                {
                    CorreoID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Remitente = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Asunto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HtmlTexto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EsHtml = table.Column<bool>(type: "bit", nullable: false),
                    Prioridad = table.Column<short>(type: "smallint", nullable: false),
                    FechaPuestaEnCola = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MascaraRemitente = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    DireccionRespuesta = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    MascaraDireccionRespuesta = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    SMTP = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Usuario = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Puerto = table.Column<int>(type: "int", nullable: false),
                    EsSeguro = table.Column<bool>(type: "bit", nullable: false),
                    EnviadoRabbit = table.Column<bool>(type: "bit", nullable: false),
                    tipo = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ColaCorreo", x => x.CorreoID);
                });

            migrationBuilder.CreateTable(
                name: "ColaCorreoDestinatario",
                columns: table => new
                {
                    CorreoID = table.Column<int>(type: "int", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    MascaraDestinatario = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Estado = table.Column<short>(type: "smallint", nullable: false),
                    FechaProcesado = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ColaCorreoDestinatario", x => new { x.CorreoID, x.Email });
                    table.ForeignKey(
                        name: "FK_ColaCorreoDestinatario_ColaCorreo_CorreoID",
                        column: x => x.CorreoID,
                        principalTable: "ColaCorreo",
                        principalColumn: "CorreoID",
                        onDelete: ReferentialAction.Restrict);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ColaCorreoDestinatario");

            migrationBuilder.DropTable(
                name: "ColaCorreo");
        }
    }
}
