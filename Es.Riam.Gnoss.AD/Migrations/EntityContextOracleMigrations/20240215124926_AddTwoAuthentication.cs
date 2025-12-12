using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Es.Riam.Gnoss.AD.Migrations.EntityContextOracleMigrations
{
    public partial class AddTwoAuthentication : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "TwoFactorAuthentication",
                table: "Usuario",
                type: "NUMBER(1)",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TwoFactorAuthentication",
                table: "Usuario");
        }
    }
}
