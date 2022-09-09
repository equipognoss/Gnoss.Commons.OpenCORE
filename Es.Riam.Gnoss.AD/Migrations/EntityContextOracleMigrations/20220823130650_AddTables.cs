using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Es.Riam.Gnoss.AD.Migrations.EntityContextOracleMigrations
{
    public partial class AddTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "MetaDescription",
                table: "ProyectoPestanyaMenu",
                type: "NVARCHAR2(1000)",
                maxLength: 10000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ServicioResultados",
                table: "ProyectoGadgetContexto",
                type: "NVARCHAR2(2000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)");

            migrationBuilder.AlterColumn<string>(
                name: "OrdenContexto",
                table: "ProyectoGadgetContexto",
                type: "NVARCHAR2(2000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)");

            migrationBuilder.AlterColumn<string>(
                name: "ComunidadOrigenFiltros",
                table: "ProyectoGadgetContexto",
                type: "NVARCHAR2(2000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)");

            migrationBuilder.AlterColumn<string>(
                name: "Ubicacion",
                table: "ProyectoGadget",
                type: "NVARCHAR2(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "NombreCorto",
                table: "ProyectoGadget",
                type: "NVARCHAR2(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "ProyectoCookie",
                type: "NVARCHAR2(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Ontologia",
                table: "PresentacionMosaicoSemantico",
                type: "NVARCHAR2(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "PresentacionMosaicoSemantico",
                type: "NVARCHAR2(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "Ontologia",
                table: "PresentacionMapaSemantico",
                type: "NVARCHAR2(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "PresentacionMapaSemantico",
                type: "NVARCHAR2(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "Ontologia",
                table: "PresentacionListadoSemantico",
                type: "NVARCHAR2(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "PresentacionListadoSemantico",
                type: "NVARCHAR2(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "Valor",
                table: "NotificacionParametro",
                type: "NVARCHAR2(2000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)");

            migrationBuilder.AlterColumn<string>(
                name: "Tags",
                table: "GrupoIdentidades",
                type: "NVARCHAR2(2000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)");

            //migrationBuilder.AddColumn<string>(
            //    name: "FacetaObjetoConocimientoProyectoFaceta",
            //    table: "FacetaHome",
            //    type: "NVARCHAR2(300)",
            //    nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "FacetaObjetoConocimientoProyectoObjetoConocimiento",
            //    table: "FacetaHome",
            //    type: "NVARCHAR2(50)",
            //    nullable: true);

            //migrationBuilder.AddColumn<Guid>(
            //    name: "FacetaObjetoConocimientoProyectoOrganizacionID",
            //    table: "FacetaHome",
            //    type: "RAW(16)",
            //    nullable: true);

            //migrationBuilder.AddColumn<Guid>(
            //    name: "FacetaObjetoConocimientoProyectoProyectoID",
            //    table: "FacetaHome",
            //    type: "RAW(16)",
            //    nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NewsletterTemporal",
                table: "DocumentoNewsletter",
                type: "NVARCHAR2(2000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)");

            migrationBuilder.AlterColumn<string>(
                name: "TokenTwitter",
                table: "ColaTwitter",
                type: "NVARCHAR2(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "TokenSecretoTwitter",
                table: "ColaTwitter",
                type: "NVARCHAR2(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "ConsumerSecret",
                table: "ColaTwitter",
                type: "NVARCHAR2(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "ConsumerKey",
                table: "ColaTwitter",
                type: "NVARCHAR2(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "Estilos",
                table: "CMSBloque",
                type: "NVARCHAR2(2000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "CategoriaProyectoCookie",
                type: "NVARCHAR2(2000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "DocumentoMetaDatos",
                columns: table => new
                {
                    DocumentoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    MetaTitulo = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: true),
                    MetaDescripcion = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentoMetaDatos", x => x.DocumentoID);
                    table.ForeignKey(
                        name: "FK_DocumentoMetaDatos_Documento_DocumentoID",
                        column: x => x.DocumentoID,
                        principalTable: "Documento",
                        principalColumn: "DocumentoID",
                        onDelete: ReferentialAction.Cascade);
                });

            //migrationBuilder.CreateIndex(
            //    name: "IX_FacetaHome_FacetaObjetoConocimientoProyectoOrganizacionID_FacetaObjetoConocimientoProyectoProyectoID_FacetaObjetoConocimient~",
            //    table: "FacetaHome",
            //    columns: new[] { "FacetaObjetoConocimientoProyectoOrganizacionID", "FacetaObjetoConocimientoProyectoProyectoID", "FacetaObjetoConocimientoProyectoObjetoConocimiento", "FacetaObjetoConocimientoProyectoFaceta" });

            //migrationBuilder.AddForeignKey(
            //    name: "FK_FacetaHome_FacetaObjetoConocimientoProyecto_FacetaObjetoConocimientoProyectoOrganizacionID_FacetaObjetoConocimientoProyectoP~",
            //    table: "FacetaHome",
            //    columns: new[] { "FacetaObjetoConocimientoProyectoOrganizacionID", "FacetaObjetoConocimientoProyectoProyectoID", "FacetaObjetoConocimientoProyectoObjetoConocimiento", "FacetaObjetoConocimientoProyectoFaceta" },
            //    principalTable: "FacetaObjetoConocimientoProyecto",
            //    principalColumns: new[] { "OrganizacionID", "ProyectoID", "ObjetoConocimiento", "Faceta" },
            //    onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_FacetaHome_FacetaObjetoConocimientoProyecto_FacetaObjetoConocimientoProyectoOrganizacionID_FacetaObjetoConocimientoProyectoP~",
            //    table: "FacetaHome");

            migrationBuilder.DropTable(
                name: "DocumentoMetaDatos");

            //migrationBuilder.DropIndex(
            //    name: "IX_FacetaHome_FacetaObjetoConocimientoProyectoOrganizacionID_FacetaObjetoConocimientoProyectoProyectoID_FacetaObjetoConocimient~",
            //    table: "FacetaHome");

            //migrationBuilder.DropColumn(
            //    name: "FacetaObjetoConocimientoProyectoFaceta",
            //    table: "FacetaHome");

            //migrationBuilder.DropColumn(
            //    name: "FacetaObjetoConocimientoProyectoObjetoConocimiento",
            //    table: "FacetaHome");

            //migrationBuilder.DropColumn(
            //    name: "FacetaObjetoConocimientoProyectoOrganizacionID",
            //    table: "FacetaHome");

            //migrationBuilder.DropColumn(
            //    name: "FacetaObjetoConocimientoProyectoProyectoID",
            //    table: "FacetaHome");

            migrationBuilder.AlterColumn<string>(
                name: "HTML",
                table: "VistaVirtualRecursos",
                type: "NVARCHAR2(2000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NCLOB");

            migrationBuilder.AlterColumn<string>(
                name: "HTML",
                table: "VistaVirtualGadgetRecursos",
                type: "NVARCHAR2(2000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NCLOB");

            migrationBuilder.AlterColumn<string>(
                name: "HTML",
                table: "VistaVirtualCMS",
                type: "NVARCHAR2(2000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NCLOB");

            migrationBuilder.AlterColumn<string>(
                name: "HTML",
                table: "VistaVirtual",
                type: "NVARCHAR2(2000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NCLOB");

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Usuario",
                type: "NVARCHAR2(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Provincia",
                table: "SolicitudNuevoUsuario",
                type: "NVARCHAR2(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "SolicitudNuevoUsuario",
                type: "NVARCHAR2(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Provincia",
                table: "SolicitudNuevaOrganizacion",
                type: "NVARCHAR2(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CargoContactoPrincipal",
                table: "SolicitudNuevaOrganizacion",
                type: "NVARCHAR2(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "RecursosRelacionadosPresentacion",
                type: "NVARCHAR2(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MetaDescription",
                table: "ProyectoPestanyaMenu",
                type: "NVARCHAR2(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NCLOB",
                oldMaxLength: 10000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ServicioResultados",
                table: "ProyectoGadgetContexto",
                type: "NVARCHAR2(2000)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OrdenContexto",
                table: "ProyectoGadgetContexto",
                type: "NVARCHAR2(2000)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ComunidadOrigenFiltros",
                table: "ProyectoGadgetContexto",
                type: "NVARCHAR2(2000)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Ubicacion",
                table: "ProyectoGadget",
                type: "NVARCHAR2(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NombreCorto",
                table: "ProyectoGadget",
                type: "NVARCHAR2(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "ProyectoCookie",
                type: "NVARCHAR2(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Ontologia",
                table: "PresentacionMosaicoSemantico",
                type: "NVARCHAR2(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "PresentacionMosaicoSemantico",
                type: "NVARCHAR2(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Ontologia",
                table: "PresentacionMapaSemantico",
                type: "NVARCHAR2(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "PresentacionMapaSemantico",
                type: "NVARCHAR2(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Ontologia",
                table: "PresentacionListadoSemantico",
                type: "NVARCHAR2(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "PresentacionListadoSemantico",
                type: "NVARCHAR2(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Valor",
                table: "NotificacionParametro",
                type: "NVARCHAR2(2000)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Tags",
                table: "GrupoIdentidades",
                type: "NVARCHAR2(2000)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NewsletterTemporal",
                table: "DocumentoNewsletter",
                type: "NVARCHAR2(2000)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TokenTwitter",
                table: "ColaTwitter",
                type: "NVARCHAR2(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TokenSecretoTwitter",
                table: "ColaTwitter",
                type: "NVARCHAR2(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ConsumerSecret",
                table: "ColaTwitter",
                type: "NVARCHAR2(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ConsumerKey",
                table: "ColaTwitter",
                type: "NVARCHAR2(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Estilos",
                table: "CMSBloque",
                type: "NVARCHAR2(2000)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "CategoriaProyectoCookie",
                type: "NVARCHAR2(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)",
                oldNullable: true);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_FacetaObjetoConocimientoProyecto_FacetaHome_OrganizacionID_ProyectoID_ObjetoConocimiento_Faceta",
            //    table: "FacetaObjetoConocimientoProyecto",
            //    columns: new[] { "OrganizacionID", "ProyectoID", "ObjetoConocimiento", "Faceta" },
            //    principalTable: "FacetaHome",
            //    principalColumns: new[] { "OrganizacionID", "ProyectoID", "ObjetoConocimiento", "Faceta" },
            //    onDelete: ReferentialAction.Cascade);
        }
    }
}
