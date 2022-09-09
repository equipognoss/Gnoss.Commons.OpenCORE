using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Es.Riam.Gnoss.AD.EntityContextMigrations
{
    public partial class Metadatos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /*
            migrationBuilder.DropForeignKey(
                name: "FK_FacetaObjetoConocimientoProyecto_FacetaHome_OrganizacionID_ProyectoID_ObjetoConocimiento_Faceta",
                table: "FacetaObjetoConocimientoProyecto");

            migrationBuilder.DropForeignKey(
                name: "FK_Organizacion_OrganizacionEmpresa_OrganizacionID",
                table: "Organizacion");

            migrationBuilder.DropForeignKey(
                name: "FK_Organizacion_OrganizacionGnoss_OrganizacionID",
                table: "Organizacion");

            migrationBuilder.DropForeignKey(
                name: "FK_Proyecto_ProyectoCerradoTmp_OrganizacionID_ProyectoID",
                table: "Proyecto");

            migrationBuilder.DropForeignKey(
                name: "FK_Proyecto_ProyectoCerrandose_OrganizacionID_ProyectoID",
                table: "Proyecto");

            migrationBuilder.DropForeignKey(
                name: "FK_Proyecto_ProyectoLoginConfiguracion_OrganizacionID_ProyectoID",
                table: "Proyecto");

            migrationBuilder.DropForeignKey(
                name: "FK_Proyecto_ProyectosMasActivos_OrganizacionID_ProyectoID",
                table: "Proyecto");

            migrationBuilder.AlterColumn<string>(
                name: "HTML",
                table: "VistaVirtualRecursos",
                type: "NCLOB",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "HTML",
                table: "VistaVirtualGadgetRecursos",
                type: "NCLOB",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "HTML",
                table: "VistaVirtualCMS",
                type: "NCLOB",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "HTML",
                table: "VistaVirtual",
                type: "NCLOB",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Usuario",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Provincia",
                table: "SolicitudNuevoUsuario",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "SolicitudNuevoUsuario",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "Provincia",
                table: "SolicitudNuevaOrganizacion",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "CargoContactoPrincipal",
                table: "SolicitudNuevaOrganizacion",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "RecursosRelacionadosPresentacion",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);
            */
            migrationBuilder.AlterColumn<string>(
                name: "MetaDescription",
                table: "ProyectoPestanyaMenu",
                type: "nvarchar(max)",
                maxLength: 10000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);
            /*
            migrationBuilder.AlterColumn<string>(
                name: "ServicioResultados",
                table: "ProyectoGadgetContexto",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "OrdenContexto",
                table: "ProyectoGadgetContexto",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ComunidadOrigenFiltros",
                table: "ProyectoGadgetContexto",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Ubicacion",
                table: "ProyectoGadget",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "NombreCorto",
                table: "ProyectoGadget",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);
            */
            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "ProyectoCookie",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);
            /*
            migrationBuilder.AlterColumn<string>(
                name: "Ontologia",
                table: "PresentacionMosaicoSemantico",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "PresentacionMosaicoSemantico",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "Ontologia",
                table: "PresentacionMapaSemantico",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "PresentacionMapaSemantico",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "Ontologia",
                table: "PresentacionListadoSemantico",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "PresentacionListadoSemantico",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "AvisoLegal",
                table: "ParametroGeneral",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Valor",
                table: "NotificacionParametro",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Tags",
                table: "GrupoIdentidades",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "FacetaObjetoConocimientoProyectoFaceta",
                table: "FacetaHome",
                type: "nvarchar(300)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FacetaObjetoConocimientoProyectoObjetoConocimiento",
                table: "FacetaHome",
                type: "nvarchar(50)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "FacetaObjetoConocimientoProyectoOrganizacionID",
                table: "FacetaHome",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "FacetaObjetoConocimientoProyectoProyectoID",
                table: "FacetaHome",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NewsletterTemporal",
                table: "DocumentoNewsletter",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "TokenTwitter",
                table: "ColaTwitter",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "TokenSecretoTwitter",
                table: "ColaTwitter",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "ConsumerSecret",
                table: "ColaTwitter",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "ConsumerKey",
                table: "ColaTwitter",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "Estilos",
                table: "CMSBloque",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
            */
            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "CategoriaProyectoCookie",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "DocumentoMetaDatos",
                columns: table => new
                {
                    DocumentoID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MetaTitulo = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    MetaDescripcion = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
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
            /*
            migrationBuilder.CreateIndex(
                name: "IX_FacetaHome_FacetaObjetoConocimientoProyectoOrganizacionID_FacetaObjetoConocimientoProyectoProyectoID_FacetaObjetoConocimient~",
                table: "FacetaHome",
                columns: new[] { "FacetaObjetoConocimientoProyectoOrganizacionID", "FacetaObjetoConocimientoProyectoProyectoID", "FacetaObjetoConocimientoProyectoObjetoConocimiento", "FacetaObjetoConocimientoProyectoFaceta" });

            migrationBuilder.AddForeignKey(
                name: "FK_FacetaHome_FacetaObjetoConocimientoProyecto_FacetaObjetoConocimientoProyectoOrganizacionID_FacetaObjetoConocimientoProyectoP~",
                table: "FacetaHome",
                columns: new[] { "FacetaObjetoConocimientoProyectoOrganizacionID", "FacetaObjetoConocimientoProyectoProyectoID", "FacetaObjetoConocimientoProyectoObjetoConocimiento", "FacetaObjetoConocimientoProyectoFaceta" },
                principalTable: "FacetaObjetoConocimientoProyecto",
                principalColumns: new[] { "OrganizacionID", "ProyectoID", "ObjetoConocimiento", "Faceta" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizacionEmpresa_Organizacion_OrganizacionID",
                table: "OrganizacionEmpresa",
                column: "OrganizacionID",
                principalTable: "Organizacion",
                principalColumn: "OrganizacionID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizacionGnoss_Organizacion_OrganizacionID",
                table: "OrganizacionGnoss",
                column: "OrganizacionID",
                principalTable: "Organizacion",
                principalColumn: "OrganizacionID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProyectoCerradoTmp_Proyecto_OrganizacionID_ProyectoID",
                table: "ProyectoCerradoTmp",
                columns: new[] { "OrganizacionID", "ProyectoID" },
                principalTable: "Proyecto",
                principalColumns: new[] { "OrganizacionID", "ProyectoID" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProyectoCerrandose_Proyecto_OrganizacionID_ProyectoID",
                table: "ProyectoCerrandose",
                columns: new[] { "OrganizacionID", "ProyectoID" },
                principalTable: "Proyecto",
                principalColumns: new[] { "OrganizacionID", "ProyectoID" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProyectoLoginConfiguracion_Proyecto_OrganizacionID_ProyectoID",
                table: "ProyectoLoginConfiguracion",
                columns: new[] { "OrganizacionID", "ProyectoID" },
                principalTable: "Proyecto",
                principalColumns: new[] { "OrganizacionID", "ProyectoID" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProyectosMasActivos_Proyecto_OrganizacionID_ProyectoID",
                table: "ProyectosMasActivos",
                columns: new[] { "OrganizacionID", "ProyectoID" },
                principalTable: "Proyecto",
                principalColumns: new[] { "OrganizacionID", "ProyectoID" },
                onDelete: ReferentialAction.Cascade);
            */
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            /*
            migrationBuilder.DropForeignKey(
                name: "FK_FacetaHome_FacetaObjetoConocimientoProyecto_FacetaObjetoConocimientoProyectoOrganizacionID_FacetaObjetoConocimientoProyectoP~",
                table: "FacetaHome");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizacionEmpresa_Organizacion_OrganizacionID",
                table: "OrganizacionEmpresa");

            migrationBuilder.DropForeignKey(
                name: "FK_OrganizacionGnoss_Organizacion_OrganizacionID",
                table: "OrganizacionGnoss");

            migrationBuilder.DropForeignKey(
                name: "FK_ProyectoCerradoTmp_Proyecto_OrganizacionID_ProyectoID",
                table: "ProyectoCerradoTmp");

            migrationBuilder.DropForeignKey(
                name: "FK_ProyectoCerrandose_Proyecto_OrganizacionID_ProyectoID",
                table: "ProyectoCerrandose");

            migrationBuilder.DropForeignKey(
                name: "FK_ProyectoLoginConfiguracion_Proyecto_OrganizacionID_ProyectoID",
                table: "ProyectoLoginConfiguracion");

            migrationBuilder.DropForeignKey(
                name: "FK_ProyectosMasActivos_Proyecto_OrganizacionID_ProyectoID",
                table: "ProyectosMasActivos");
            */
            migrationBuilder.DropTable(
                name: "DocumentoMetaDatos");
            /*
            migrationBuilder.DropIndex(
                name: "IX_FacetaHome_FacetaObjetoConocimientoProyectoOrganizacionID_FacetaObjetoConocimientoProyectoProyectoID_FacetaObjetoConocimient~",
                table: "FacetaHome");

            migrationBuilder.DropColumn(
                name: "FacetaObjetoConocimientoProyectoFaceta",
                table: "FacetaHome");

            migrationBuilder.DropColumn(
                name: "FacetaObjetoConocimientoProyectoObjetoConocimiento",
                table: "FacetaHome");

            migrationBuilder.DropColumn(
                name: "FacetaObjetoConocimientoProyectoOrganizacionID",
                table: "FacetaHome");

            migrationBuilder.DropColumn(
                name: "FacetaObjetoConocimientoProyectoProyectoID",
                table: "FacetaHome");

            migrationBuilder.AlterColumn<string>(
                name: "HTML",
                table: "VistaVirtualRecursos",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NCLOB");

            migrationBuilder.AlterColumn<string>(
                name: "HTML",
                table: "VistaVirtualGadgetRecursos",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NCLOB");

            migrationBuilder.AlterColumn<string>(
                name: "HTML",
                table: "VistaVirtualCMS",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NCLOB");

            migrationBuilder.AlterColumn<string>(
                name: "HTML",
                table: "VistaVirtual",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NCLOB");

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Usuario",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Provincia",
                table: "SolicitudNuevoUsuario",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "SolicitudNuevoUsuario",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Provincia",
                table: "SolicitudNuevaOrganizacion",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CargoContactoPrincipal",
                table: "SolicitudNuevaOrganizacion",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "RecursosRelacionadosPresentacion",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);
            */
            migrationBuilder.AlterColumn<string>(
                name: "MetaDescription",
                table: "ProyectoPestanyaMenu",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 10000,
                oldNullable: true);
            /*
            migrationBuilder.AlterColumn<string>(
                name: "ServicioResultados",
                table: "ProyectoGadgetContexto",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OrdenContexto",
                table: "ProyectoGadgetContexto",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ComunidadOrigenFiltros",
                table: "ProyectoGadgetContexto",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Ubicacion",
                table: "ProyectoGadget",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NombreCorto",
                table: "ProyectoGadget",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);
            */
            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "ProyectoCookie",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);
            /*
            migrationBuilder.AlterColumn<string>(
                name: "Ontologia",
                table: "PresentacionMosaicoSemantico",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "PresentacionMosaicoSemantico",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Ontologia",
                table: "PresentacionMapaSemantico",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "PresentacionMapaSemantico",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Ontologia",
                table: "PresentacionListadoSemantico",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "PresentacionListadoSemantico",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AvisoLegal",
                table: "ParametroGeneral",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(4000)",
                oldMaxLength: 4000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Valor",
                table: "NotificacionParametro",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Tags",
                table: "GrupoIdentidades",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NewsletterTemporal",
                table: "DocumentoNewsletter",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TokenTwitter",
                table: "ColaTwitter",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TokenSecretoTwitter",
                table: "ColaTwitter",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ConsumerSecret",
                table: "ColaTwitter",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ConsumerKey",
                table: "ColaTwitter",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Estilos",
                table: "CMSBloque",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
            */
            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "CategoriaProyectoCookie",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
            /*
            migrationBuilder.AddForeignKey(
                name: "FK_FacetaObjetoConocimientoProyecto_FacetaHome_OrganizacionID_ProyectoID_ObjetoConocimiento_Faceta",
                table: "FacetaObjetoConocimientoProyecto",
                columns: new[] { "OrganizacionID", "ProyectoID", "ObjetoConocimiento", "Faceta" },
                principalTable: "FacetaHome",
                principalColumns: new[] { "OrganizacionID", "ProyectoID", "ObjetoConocimiento", "Faceta" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Organizacion_OrganizacionEmpresa_OrganizacionID",
                table: "Organizacion",
                column: "OrganizacionID",
                principalTable: "OrganizacionEmpresa",
                principalColumn: "OrganizacionID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Organizacion_OrganizacionGnoss_OrganizacionID",
                table: "Organizacion",
                column: "OrganizacionID",
                principalTable: "OrganizacionGnoss",
                principalColumn: "OrganizacionID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Proyecto_ProyectoCerradoTmp_OrganizacionID_ProyectoID",
                table: "Proyecto",
                columns: new[] { "OrganizacionID", "ProyectoID" },
                principalTable: "ProyectoCerradoTmp",
                principalColumns: new[] { "OrganizacionID", "ProyectoID" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Proyecto_ProyectoCerrandose_OrganizacionID_ProyectoID",
                table: "Proyecto",
                columns: new[] { "OrganizacionID", "ProyectoID" },
                principalTable: "ProyectoCerrandose",
                principalColumns: new[] { "OrganizacionID", "ProyectoID" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Proyecto_ProyectoLoginConfiguracion_OrganizacionID_ProyectoID",
                table: "Proyecto",
                columns: new[] { "OrganizacionID", "ProyectoID" },
                principalTable: "ProyectoLoginConfiguracion",
                principalColumns: new[] { "OrganizacionID", "ProyectoID" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Proyecto_ProyectosMasActivos_OrganizacionID_ProyectoID",
                table: "Proyecto",
                columns: new[] { "OrganizacionID", "ProyectoID" },
                principalTable: "ProyectosMasActivos",
                principalColumns: new[] { "OrganizacionID", "ProyectoID" },
                onDelete: ReferentialAction.Cascade);
            */
        }
    }
}
