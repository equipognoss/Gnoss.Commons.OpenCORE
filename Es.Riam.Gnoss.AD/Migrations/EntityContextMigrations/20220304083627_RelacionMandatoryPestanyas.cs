using Microsoft.EntityFrameworkCore.Migrations;

namespace Es.Riam.Gnoss.AD.Migrations.EntityContextMigrations
{
    public partial class RelacionMandatoryPestanyas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RelacionMandatory",
                table: "ProyectoPestanyaBusqueda",
                type: "nvarchar(max)",
                nullable: true);

            //migrationBuilder.CreateIndex(
            //    name: "IX_FacetaObjetoConocimientoProyectoPestanya_PestanyaID",
            //    table: "FacetaObjetoConocimientoProyectoPestanya",
            //    column: "PestanyaID");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_FacetaObjetoConocimientoProyectoPestanya_FacetaObjetoConocimientoProyecto_OrganizacionID_ProyectoID_ObjetoConocimiento_Faceta",
            //    table: "FacetaObjetoConocimientoProyectoPestanya",
            //    columns: new[] { "OrganizacionID", "ProyectoID", "ObjetoConocimiento", "Faceta" },
            //    principalTable: "FacetaObjetoConocimientoProyecto",
            //    principalColumns: new[] { "OrganizacionID", "ProyectoID", "ObjetoConocimiento", "Faceta" },
            //    onDelete: ReferentialAction.Restrict);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_FacetaObjetoConocimientoProyectoPestanya_Proyecto_OrganizacionID_ProyectoID",
            //    table: "FacetaObjetoConocimientoProyectoPestanya",
            //    columns: new[] { "OrganizacionID", "ProyectoID" },
            //    principalTable: "Proyecto",
            //    principalColumns: new[] { "OrganizacionID", "ProyectoID" },
            //    onDelete: ReferentialAction.Restrict);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_FacetaObjetoConocimientoProyectoPestanya_ProyectoPestanyaMenu_PestanyaID",
            //    table: "FacetaObjetoConocimientoProyectoPestanya",
            //    column: "PestanyaID",
            //    principalTable: "ProyectoPestanyaMenu",
            //    principalColumn: "PestanyaID",
            //    onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_FacetaObjetoConocimientoProyectoPestanya_FacetaObjetoConocimientoProyecto_OrganizacionID_ProyectoID_ObjetoConocimiento_Faceta",
            //    table: "FacetaObjetoConocimientoProyectoPestanya");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_FacetaObjetoConocimientoProyectoPestanya_Proyecto_OrganizacionID_ProyectoID",
            //    table: "FacetaObjetoConocimientoProyectoPestanya");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_FacetaObjetoConocimientoProyectoPestanya_ProyectoPestanyaMenu_PestanyaID",
            //    table: "FacetaObjetoConocimientoProyectoPestanya");

            //migrationBuilder.DropIndex(
            //    name: "IX_FacetaObjetoConocimientoProyectoPestanya_PestanyaID",
            //    table: "FacetaObjetoConocimientoProyectoPestanya");

            migrationBuilder.DropColumn(
                name: "RelacionMandatory",
                table: "ProyectoPestanyaBusqueda");
        }
    }
}
