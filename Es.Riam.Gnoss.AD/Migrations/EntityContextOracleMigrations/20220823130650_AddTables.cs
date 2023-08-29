using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Es.Riam.Gnoss.AD.Migrations.EntityContextOracleMigrations
{
    public partial class AddTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DocumentoMetaDatos",
                columns: table => new
                {
                    DocumentoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    MetaTitulo = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: true),
                    MetaDescripcion = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentoMetaDatos", x => x.DocumentoID);
                    table.ForeignKey(
                        name: "FK_DocumentoMetaDatos_Documento_DocumentoID",
                        column: x => x.DocumentoID,
                        principalTable: "Documento",
                        principalColumn: "DocumentoID",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropTable(
                name: "DocumentoMetaDatos");

            
        }
    }
}
