using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Es.Riam.Gnoss.AD.Migrations.EntityContextOracleMigrations
{
    /// <inheritdoc />
    public partial class TrasnsformarColumnaWhereSPARQL : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "WhereSPARQL",
                table: "ProyectoSearchPersonalizado",
                type: "NCLOB",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "WhereFacetasSPARQL",
                table: "ProyectoSearchPersonalizado",
                type: "NCLOB",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "WhereSPARQL",
                table: "ProyectoSearchPersonalizado",
                type: "NVARCHAR2(2000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NCLOB",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "WhereFacetasSPARQL",
                table: "ProyectoSearchPersonalizado",
                type: "NVARCHAR2(2000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NCLOB",
                oldNullable: true);
        }
    }
}
