using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Es.Riam.Gnoss.AD.Migrations.EntityContextOracleMigrations
{
    /// <inheritdoc />
    public partial class sitemaps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sitemaps");

            migrationBuilder.DropColumn(
                name: "Sitemap",
                table: "SitemapsIndex");

            migrationBuilder.AddColumn<DateTime>(
                name: "GeneratedAt",
                table: "SitemapsIndex",
                type: "TIMESTAMP(7)",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GeneratedAt",
                table: "SitemapsIndex");

            migrationBuilder.AddColumn<string>(
                name: "Sitemap",
                table: "SitemapsIndex",
                type: "NVARCHAR2(2000)",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 1);

            migrationBuilder.CreateTable(
                name: "Sitemaps",
                columns: table => new
                {
                    Dominio = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    SitemapIndexName = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    SitemapContent = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sitemaps", x => new { x.Dominio, x.SitemapIndexName });
                    table.ForeignKey(
                        name: "FK_Sitemaps_SitemapsIndex_Dominio",
                        column: x => x.Dominio,
                        principalTable: "SitemapsIndex",
                        principalColumn: "Dominio",
                        onDelete: ReferentialAction.Cascade);
                });
        }
    }
}
