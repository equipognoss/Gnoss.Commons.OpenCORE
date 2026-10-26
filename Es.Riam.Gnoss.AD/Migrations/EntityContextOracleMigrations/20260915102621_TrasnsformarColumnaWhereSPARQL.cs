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
            // Oracle no permite convertir NVARCHAR2 a NCLOB con ALTER TABLE ... MODIFY (ORA-22858)
            migrationBuilder.AddColumn<string>(
                name: "WhereSPARQL_TMP",
                table: "ProyectoSearchPersonalizado",
                type: "NCLOB",
                nullable: true);

            migrationBuilder.Sql("UPDATE \"ProyectoSearchPersonalizado\" SET \"WhereSPARQL_TMP\" = TO_NCLOB(\"WhereSPARQL\")");

            migrationBuilder.DropColumn(
                name: "WhereSPARQL",
                table: "ProyectoSearchPersonalizado");

            migrationBuilder.RenameColumn(
                name: "WhereSPARQL_TMP",
                table: "ProyectoSearchPersonalizado",
                newName: "WhereSPARQL");

            migrationBuilder.AddColumn<string>(
                name: "WhereFacetasSPARQL_TMP",
                table: "ProyectoSearchPersonalizado",
                type: "NCLOB",
                nullable: true);

            migrationBuilder.Sql("UPDATE \"ProyectoSearchPersonalizado\" SET \"WhereFacetasSPARQL_TMP\" = TO_NCLOB(\"WhereFacetasSPARQL\")");

            migrationBuilder.DropColumn(
                name: "WhereFacetasSPARQL",
                table: "ProyectoSearchPersonalizado");

            migrationBuilder.RenameColumn(
                name: "WhereFacetasSPARQL_TMP",
                table: "ProyectoSearchPersonalizado",
                newName: "WhereFacetasSPARQL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Los valores de más de 2000 caracteres se truncan al volver a NVARCHAR2(2000)
            migrationBuilder.AddColumn<string>(
                name: "WhereSPARQL_TMP",
                table: "ProyectoSearchPersonalizado",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.Sql("UPDATE \"ProyectoSearchPersonalizado\" SET \"WhereSPARQL_TMP\" = TO_NCHAR(DBMS_LOB.SUBSTR(\"WhereSPARQL\", 2000, 1))");

            migrationBuilder.DropColumn(
                name: "WhereSPARQL",
                table: "ProyectoSearchPersonalizado");

            migrationBuilder.RenameColumn(
                name: "WhereSPARQL_TMP",
                table: "ProyectoSearchPersonalizado",
                newName: "WhereSPARQL");

            migrationBuilder.AddColumn<string>(
                name: "WhereFacetasSPARQL_TMP",
                table: "ProyectoSearchPersonalizado",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.Sql("UPDATE \"ProyectoSearchPersonalizado\" SET \"WhereFacetasSPARQL_TMP\" = TO_NCHAR(DBMS_LOB.SUBSTR(\"WhereFacetasSPARQL\", 2000, 1))");

            migrationBuilder.DropColumn(
                name: "WhereFacetasSPARQL",
                table: "ProyectoSearchPersonalizado");

            migrationBuilder.RenameColumn(
                name: "WhereFacetasSPARQL_TMP",
                table: "ProyectoSearchPersonalizado",
                newName: "WhereFacetasSPARQL");
        }
    }
}
