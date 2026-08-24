using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Es.Riam.Gnoss.AD.Migrations.EntityContextPostgresMigrations
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
                type: "timestamp without time zone",
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
                type: "text",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 1);

            migrationBuilder.CreateTable(
                name: "Sitemaps",
                columns: table => new
                {
                    Dominio = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    SitemapIndexName = table.Column<string>(type: "text", nullable: false),
                    SitemapContent = table.Column<string>(type: "text", nullable: true)
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
