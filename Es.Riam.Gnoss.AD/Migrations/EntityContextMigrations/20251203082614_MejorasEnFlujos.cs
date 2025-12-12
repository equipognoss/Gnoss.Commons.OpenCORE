using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Es.Riam.Gnoss.AD.Migrations.EntityContextMigrations
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
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PermiteMejora",
                table: "Estado",
                type: "bit",
                nullable: false,
                defaultValue: false);
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
