using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Es.Riam.Gnoss.AD.Migrations.EntityContextOracleMigrations
{
    public partial class AddTableVistaVirtualDominio : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VistaVirtualDominio",
                columns: table => new
                {
                    PersonalizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Dominio = table.Column<string>(type: "NVARCHAR2(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VistaVirtualDominio", x => new { x.PersonalizacionID, x.Dominio });
                    table.ForeignKey(
                        name: "FK_VistaVirtualDominio_VistaVirtualPersonalizacion_PersonalizacionID",
                        column: x => x.PersonalizacionID,
                        principalTable: "VistaVirtualPersonalizacion",
                        principalColumn: "PersonalizacionID",
                        onDelete: ReferentialAction.Restrict);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VistaVirtualDominio");

            
        }
    }
}
