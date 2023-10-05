using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Es.Riam.Gnoss.AD.Migrations.EntityContextMigrations
{
    public partial class _202305220823_AddTableVistaVirtualDominio : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AlterColumn<string>(
            //    name: "MetaDescription",
            //    table: "ProyectoPestanyaMenu",
            //    type: "nvarchar(1000)",
            //    maxLength: 1000,
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(max)",
            //    oldMaxLength: 10000,
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "AvisoLegal",
            //    table: "ParametroGeneral",
            //    type: "nvarchar(max)",
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(4000)",
            //    oldMaxLength: 4000,
            //    oldNullable: true);

            migrationBuilder.CreateTable(
                name: "VistaVirtualDominio",
                columns: table => new
                {
                    PersonalizacionID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Dominio = table.Column<string>(type: "nvarchar(450)", nullable: false)
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

            //migrationBuilder.AlterColumn<string>(
            //    name: "MetaDescription",
            //    table: "ProyectoPestanyaMenu",
            //    type: "nvarchar(max)",
            //    maxLength: 10000,
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(1000)",
            //    oldMaxLength: 1000,
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "AvisoLegal",
            //    table: "ParametroGeneral",
            //    type: "nvarchar(4000)",
            //    maxLength: 4000,
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(max)",
            //    oldNullable: true);
        }
    }
}
