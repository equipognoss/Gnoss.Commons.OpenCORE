using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Es.Riam.Gnoss.AD.Migrations.EntityContextOracleMigrations
{
    /// <inheritdoc />
    public partial class MejorasEnFlujos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EsMejora",
                table: "VersionDocumento",
                type: "NUMBER(1)",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "PermiteMejora",
                table: "Estado",
                type: "NUMBER(1)",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EsMejora",
                table: "VersionDocumento");

            migrationBuilder.DropColumn(
                name: "PermiteMejora",
                table: "Estado"); 
        }
    }
}
