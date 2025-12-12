using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Es.Riam.Gnoss.AD.Migrations.EntityContextPostgresMigrations
{
    /// <inheritdoc />
    public partial class EstadoDeUnaVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "EstadoVersion",
                table: "VersionDocumento",
                type: "smallint",
                nullable: false,
                defaultValue: (short)2);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstadoVersion",
                table: "VersionDocumento");
        }
    }
}
