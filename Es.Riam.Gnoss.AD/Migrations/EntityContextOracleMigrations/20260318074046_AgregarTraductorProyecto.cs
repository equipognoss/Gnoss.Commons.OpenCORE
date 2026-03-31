using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Es.Riam.Gnoss.AD.Migrations.EntityContextOracleMigrations
{
    /// <inheritdoc />
    public partial class AgregarTraductorProyecto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TraductorProyecto",
                columns: table => new
                {
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Token = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Endpoint = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Nivel = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Prompt = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Activo = table.Column<bool>(type: "NUMBER(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TraductorProyecto", x => new { x.OrganizacionID, x.ProyectoID });
                    table.ForeignKey(
                        name: "FK_TraductorProyecto_Proyecto_OrganizacionID_ProyectoID",
                        columns: x => new { x.OrganizacionID, x.ProyectoID },
                        principalTable: "Proyecto",
                        principalColumns: new[] { "OrganizacionID", "ProyectoID" });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TraductorProyecto");
        }
    }
}
