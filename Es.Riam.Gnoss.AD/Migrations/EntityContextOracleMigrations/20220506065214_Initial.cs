using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Es.Riam.Gnoss.AD.Migrations.EntityContextOracleMigrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccionesExternas",
                columns: table => new
                {
                    TipoAccion = table.Column<short>(type: "NUMBER(5)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    URL = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccionesExternas", x => x.TipoAccion);
                });

            migrationBuilder.CreateTable(
                name: "AccionesExternasProyecto",
                columns: table => new
                {
                    TipoAccion = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    URL = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccionesExternasProyecto", x => new { x.TipoAccion, x.OrganizacionID, x.ProyectoID });
                });

            migrationBuilder.CreateTable(
                name: "Amigo",
                columns: table => new
                {
                    IdentidadAmigoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    IdentidadID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    EsFanMutuo = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    Fecha = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Amigo", x => new { x.IdentidadAmigoID, x.IdentidadID });
                });

            migrationBuilder.CreateTable(
                name: "BaseRecursos",
                columns: table => new
                {
                    BaseRecursosID = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaseRecursos", x => x.BaseRecursosID);
                });

            migrationBuilder.CreateTable(
                name: "Blog",
                columns: table => new
                {
                    BlogID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Titulo = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    Subtitulo = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true),
                    Descripcion = table.Column<string>(type: "NVARCHAR2(2000)", maxLength: 2000, nullable: true),
                    AutorID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Fecha = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Visibilidad = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    ArticulosPorPagina = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    PermitirComentarios = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    PermitirTrackbacks = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    CrearFuentesWeb = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    VisibilidadListadosBusquedas = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    VisibilidadBuscadoresWeb = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    ArticulosTotales = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    ComentariosTotales = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    Eliminado = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    NombreCorto = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false),
                    Seguidores = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    PermiteActualizarTwitter = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    Licencia = table.Column<string>(type: "NVARCHAR2(10)", maxLength: 10, nullable: true),
                    Tags = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blog", x => x.BlogID);
                });

            migrationBuilder.CreateTable(
                name: "CamposRegistroProyectoGenericos",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Orden = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    Tipo = table.Column<short>(type: "NUMBER(5)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CamposRegistroProyectoGenericos", x => new { x.OrganizacionID, x.ProyectoID, x.Orden });
                });

            migrationBuilder.CreateTable(
                name: "Carga",
                columns: table => new
                {
                    CargaID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Nombre = table.Column<string>(type: "NVARCHAR2(1200)", maxLength: 1200, nullable: true),
                    Estado = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    FechaAlta = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    IdentidadID = table.Column<Guid>(type: "RAW(16)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carga", x => x.CargaID);
                });

            migrationBuilder.CreateTable(
                name: "CategoriaTesauroPropiedades",
                columns: table => new
                {
                    TesauroID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    CategoriaTesauroID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Obligatoria = table.Column<short>(type: "NUMBER(5)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriaTesauroPropiedades", x => new { x.TesauroID, x.CategoriaTesauroID });
                });

            migrationBuilder.CreateTable(
                name: "CatTesauroPermiteTipoRec",
                columns: table => new
                {
                    TesauroID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    CategoriaTesauroID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    TipoRecurso = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    OntologiasID = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatTesauroPermiteTipoRec", x => new { x.TesauroID, x.CategoriaTesauroID, x.TipoRecurso });
                });

            migrationBuilder.CreateTable(
                name: "ClausulaRegistro",
                columns: table => new
                {
                    ClausulaID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Texto = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Tipo = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    Orden = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClausulaRegistro", x => new { x.ClausulaID, x.OrganizacionID, x.ProyectoID });
                });

            migrationBuilder.CreateTable(
                name: "CMSComponente",
                columns: table => new
                {
                    ComponenteID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Nombre = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    TipoComponente = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    TipoCaducidadComponente = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    FechaUltimaActualizacion = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    Estilos = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Activo = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    IdiomasDisponibles = table.Column<string>(type: "VARCHAR2(4000)", unicode: false, nullable: true),
                    NombreCortoComponente = table.Column<string>(type: "VARCHAR2(100)", unicode: false, maxLength: 100, nullable: false),
                    AccesoPublico = table.Column<bool>(type: "NUMBER(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CMSComponente", x => x.ComponenteID);
                });

            migrationBuilder.CreateTable(
                name: "CMSComponentePrivadoProyecto",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    TipoComponente = table.Column<short>(type: "NUMBER(5)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CMSComponentePrivadoProyecto", x => new { x.OrganizacionID, x.ProyectoID, x.TipoComponente });
                });

            migrationBuilder.CreateTable(
                name: "CMSPagina",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Ubicacion = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    Activa = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    Privacidad = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    HTMLAlternativo = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    MostrarSoloCuerpo = table.Column<bool>(type: "NUMBER(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CMSPagina", x => new { x.OrganizacionID, x.ProyectoID, x.Ubicacion });
                });

            migrationBuilder.CreateTable(
                name: "CMSRolGrupoIdentidades",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Ubicacion = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    GrupoID = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CMSRolGrupoIdentidades", x => new { x.OrganizacionID, x.ProyectoID, x.Ubicacion, x.GrupoID });
                });

            migrationBuilder.CreateTable(
                name: "CMSRolIdentidad",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Ubicacion = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    PerfilID = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CMSRolIdentidad", x => new { x.OrganizacionID, x.ProyectoID, x.Ubicacion, x.PerfilID });
                });

            migrationBuilder.CreateTable(
                name: "ColaCargaRecursos",
                columns: table => new
                {
                    ColaID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    UsuarioID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Fecha = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Estado = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    NombreFichImport = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ColaCargaRecursos", x => x.ColaID);
                });

            migrationBuilder.CreateTable(
                name: "ColaDocumento",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    DocumentoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    AccionRealizada = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    Estado = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    FechaEncolado = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    FechaProcesado = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    Prioridad = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    InfoExtra = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    EstadoCargaID = table.Column<long>(type: "NUMBER(19)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ColaDocumento", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ColaTwitter",
                columns: table => new
                {
                    ColaID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    PerfilID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    TokenTwitter = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: false),
                    TokenSecretoTwitter = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: false),
                    Mensaje = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: false),
                    Enlace = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: false),
                    ConsumerKey = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: false),
                    ConsumerSecret = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: false),
                    NumIntentos = table.Column<short>(type: "NUMBER(5)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ColaTwitter", x => x.ColaID);
                });

            migrationBuilder.CreateTable(
                name: "Comentario",
                columns: table => new
                {
                    ComentarioID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    IdentidadID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Fecha = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Descripcion = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Eliminado = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    ComentarioSuperiorID = table.Column<Guid>(type: "RAW(16)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comentario", x => x.ComentarioID);
                    table.ForeignKey(
                        name: "FK_Comentario_Comentario_ComentarioSuperiorID",
                        column: x => x.ComentarioSuperiorID,
                        principalTable: "Comentario",
                        principalColumn: "ComentarioID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ComparticionAutomatica",
                columns: table => new
                {
                    ComparticionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OrganizacionOrigenID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoOrigenID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OrganizacionDestinoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoDestinoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Nombre = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    IdentidadPublicadoraID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Eliminada = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    ActualizarHome = table.Column<bool>(type: "NUMBER(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComparticionAutomatica", x => x.ComparticionID);
                });

            migrationBuilder.CreateTable(
                name: "ConfigApplicationInsightsDominio",
                columns: table => new
                {
                    Dominio = table.Column<string>(type: "NVARCHAR2(250)", maxLength: 250, nullable: false),
                    ImplementationKey = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    UbicacionLogs = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    UbicacionTrazas = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfigApplicationInsightsDominio", x => x.Dominio);
                });

            migrationBuilder.CreateTable(
                name: "ConfigSearchProy",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Clave = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Valor = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfigSearchProy", x => new { x.OrganizacionID, x.ProyectoID, x.Clave });
                });

            migrationBuilder.CreateTable(
                name: "ConfiguracionAmbitoBusqueda",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Metabusqueda = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    NombreAmbitoTodaComunidad = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    TodoGnoss = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    DefectoHome = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracionAmbitoBusqueda", x => new { x.OrganizacionID, x.ProyectoID });
                });

            migrationBuilder.CreateTable(
                name: "ConfiguracionAmbitoBusquedaProyecto",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Metabusqueda = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    TodoGnoss = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    PestanyaDefectoID = table.Column<Guid>(type: "RAW(16)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracionAmbitoBusquedaProyecto", x => new { x.OrganizacionID, x.ProyectoID });
                });

            migrationBuilder.CreateTable(
                name: "ConfiguracionBBDD",
                columns: table => new
                {
                    NumConexion = table.Column<short>(type: "NUMBER(5)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NombreConexion = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    TipoConexion = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    Conexion = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: false),
                    DatosExtra = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    EsMaster = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    LecturaPermitida = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    ConectionTimeout = table.Column<int>(type: "NUMBER(10)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracionBBDD", x => x.NumConexion);
                });

            migrationBuilder.CreateTable(
                name: "ConfiguracionConexionGrafo",
                columns: table => new
                {
                    Grafo = table.Column<string>(type: "NVARCHAR2(36)", maxLength: 36, nullable: false),
                    CadenaConexion = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracionConexionGrafo", x => x.Grafo);
                });

            migrationBuilder.CreateTable(
                name: "ConfiguracionEnvioCorreo",
                columns: table => new
                {
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    email = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    smtp = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: false),
                    puerto = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    usuario = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: false),
                    clave = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: false),
                    emailsugerencias = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: false),
                    tipo = table.Column<string>(type: "VARCHAR2(10)", unicode: false, maxLength: 10, nullable: false),
                    SSL = table.Column<bool>(type: "NUMBER(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracionEnvioCorreo", x => x.ProyectoID);
                });

            migrationBuilder.CreateTable(
                name: "ConfiguracionServicios",
                columns: table => new
                {
                    NumServicio = table.Column<short>(type: "NUMBER(5)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Nombre = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Url = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracionServicios", x => x.NumServicio);
                });

            migrationBuilder.CreateTable(
                name: "ConfiguracionServiciosDominio",
                columns: table => new
                {
                    NumServicio = table.Column<short>(type: "NUMBER(5)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Dominio = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Nombre = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Url = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracionServiciosDominio", x => x.NumServicio);
                });

            migrationBuilder.CreateTable(
                name: "ConfiguracionServiciosProyecto",
                columns: table => new
                {
                    NumServicio = table.Column<short>(type: "NUMBER(5)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Nombre = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Url = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracionServiciosProyecto", x => x.NumServicio);
                });

            migrationBuilder.CreateTable(
                name: "ContadorPerfil",
                columns: table => new
                {
                    PerfilID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    NumComentarios = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    NumComentContribuciones = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    NumComentMisRec = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    NumComentBlog = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    NumNuevosMensajes = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    NuevosComentarios = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    FechaVisitaComentarios = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    NuevasInvitaciones = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    NuevasSuscripciones = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    FechaVisitaSuscripciones = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    NumMensajesSinLeer = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    NumInvitacionesSinLeer = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    NumSuscripcionesSinLeer = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    NumComentariosSinLeer = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContadorPerfil", x => x.PerfilID);
                });

            migrationBuilder.CreateTable(
                name: "CorreoInterno",
                columns: table => new
                {
                    CorreoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Autor = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Asunto = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    Cuerpo = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Fecha = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Eliminado = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    EnPapelera = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    ConversacionID = table.Column<Guid>(type: "RAW(16)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CorreoInterno", x => x.CorreoID);
                });

            migrationBuilder.CreateTable(
                name: "Curriculum",
                columns: table => new
                {
                    CurriculumID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Titulo = table.Column<string>(type: "NVARCHAR2(250)", maxLength: 250, nullable: true),
                    TipoVisibilidad = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    UsaDatosPersonalesPerfil = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    FechaPublicacion = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    Publicado = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    Tags = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Description = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Curriculum", x => x.CurriculumID);
                });

            migrationBuilder.CreateTable(
                name: "DatoExtraEcosistema",
                columns: table => new
                {
                    DatoExtraID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Orden = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Titulo = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false),
                    PredicadoRDF = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false),
                    Obligatorio = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    Paso1Registro = table.Column<bool>(type: "NUMBER(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatoExtraEcosistema", x => x.DatoExtraID);
                });

            migrationBuilder.CreateTable(
                name: "DatoExtraEcosistemaVirtuoso",
                columns: table => new
                {
                    DatoExtraID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Orden = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Titulo = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false),
                    InputID = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false),
                    InputsSuperiores = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false),
                    QueryVirtuoso = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false),
                    ConexionBD = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false),
                    Obligatorio = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    Paso1Registro = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    VisibilidadFichaPerfil = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    PredicadoRDF = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false),
                    NombreCampo = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false),
                    EstructuraHTMLFicha = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatoExtraEcosistemaVirtuoso", x => x.DatoExtraID);
                });

            migrationBuilder.CreateTable(
                name: "DatoExtraEcosistemaVirtuosoPerfil",
                columns: table => new
                {
                    DatoExtraID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    PerfilID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Opcion = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatoExtraEcosistemaVirtuosoPerfil", x => new { x.DatoExtraID, x.PerfilID });
                });

            migrationBuilder.CreateTable(
                name: "DatoExtraProyecto",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    DatoExtraID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Orden = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Titulo = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false),
                    PredicadoRDF = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false),
                    Obligatorio = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    Paso1Registro = table.Column<bool>(type: "NUMBER(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatoExtraProyecto", x => new { x.OrganizacionID, x.ProyectoID, x.DatoExtraID });
                });

            migrationBuilder.CreateTable(
                name: "DatoExtraProyectoVirtuoso",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    DatoExtraID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Orden = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Titulo = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false),
                    InputID = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false),
                    InputsSuperiores = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false),
                    QueryVirtuoso = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false),
                    ConexionBD = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false),
                    Obligatorio = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    Paso1Registro = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    VisibilidadFichaPerfil = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    PredicadoRDF = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false),
                    NombreCampo = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false),
                    EstructuraHTMLFicha = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatoExtraProyectoVirtuoso", x => new { x.OrganizacionID, x.ProyectoID, x.DatoExtraID });
                });

            migrationBuilder.CreateTable(
                name: "Documento",
                columns: table => new
                {
                    DocumentoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    CompartirPermitido = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    ElementoVinculadoID = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    Titulo = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: false),
                    Descripcion = table.Column<string>(type: "NCLOB", nullable: true),
                    Tipo = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    Enlace = table.Column<string>(type: "NVARCHAR2(1200)", maxLength: 1200, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    CreadorID = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    TipoEntidad = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    NombreCategoriaDoc = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    NombreElementoVinculado = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: true),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    Publico = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    Borrador = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    FichaBibliograficaID = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    CreadorEsAutor = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    Valoracion = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    Autor = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    FechaModificacion = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    IdentidadProteccionID = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    FechaProteccion = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    Protegido = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    PrivadoEditores = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    UltimaVersion = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    Eliminado = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    NumeroComentariosPublicos = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    NumeroTotalVotos = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    NumeroTotalConsultas = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    NumeroTotalDescargas = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    VersionFotoDocumento = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    Rank = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    Rank_Tiempo = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    Licencia = table.Column<string>(type: "NVARCHAR2(10)", maxLength: 10, nullable: true),
                    Visibilidad = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    Tags = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documento", x => x.DocumentoID);
                });

            migrationBuilder.CreateTable(
                name: "DocumentoEnEdicion",
                columns: table => new
                {
                    DocumentoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    IdentidadID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    FechaEdicion = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentoEnEdicion", x => x.DocumentoID);
                });

            migrationBuilder.CreateTable(
                name: "DocumentoEntidadGnoss",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    EntidadGnossID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    DocumentoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    CategoriaDocumentacionID = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentoEntidadGnoss", x => new { x.OrganizacionID, x.ProyectoID, x.EntidadGnossID, x.DocumentoID, x.CategoriaDocumentacionID });
                });

            migrationBuilder.CreateTable(
                name: "DocumentoEnvioNewsLetter",
                columns: table => new
                {
                    DocumentoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    IdentidadID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Fecha = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Idioma = table.Column<string>(type: "NVARCHAR2(5)", maxLength: 5, nullable: false),
                    EnvioSolicitado = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    EnvioRealizado = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    Grupos = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentoEnvioNewsLetter", x => new { x.DocumentoID, x.IdentidadID, x.Fecha });
                });

            migrationBuilder.CreateTable(
                name: "DocumentoGrupoUsuario",
                columns: table => new
                {
                    DocumentoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    GrupoUsuarioID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Editor = table.Column<bool>(type: "NUMBER(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentoGrupoUsuario", x => new { x.DocumentoID, x.GrupoUsuarioID });
                });

            migrationBuilder.CreateTable(
                name: "DocumentoNewsletter",
                columns: table => new
                {
                    DocumentoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    NewsletterTemporal = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Newsletter = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentoNewsletter", x => x.DocumentoID);
                });

            migrationBuilder.CreateTable(
                name: "DocumentoRespuesta",
                columns: table => new
                {
                    RespuestaID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    DocumentoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Descripcion = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    NumVotos = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Orden = table.Column<short>(type: "NUMBER(5)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentoRespuesta", x => x.RespuestaID);
                });

            migrationBuilder.CreateTable(
                name: "DocumentoTokenBrightcove",
                columns: table => new
                {
                    TokenID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    UsuarioID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    DocumentoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Estado = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    NombreArchivo = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentoTokenBrightcove", x => x.TokenID);
                });

            migrationBuilder.CreateTable(
                name: "DocumentoTokenTOP",
                columns: table => new
                {
                    TokenID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    UsuarioID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    DocumentoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Estado = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    NombreArchivo = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentoTokenTOP", x => x.TokenID);
                });

            migrationBuilder.CreateTable(
                name: "DocumentoUrlCanonica",
                columns: table => new
                {
                    DocumentoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    UrlCanonica = table.Column<string>(type: "NVARCHAR2(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentoUrlCanonica", x => x.DocumentoID);
                });

            migrationBuilder.CreateTable(
                name: "EcosistemaServicioExterno",
                columns: table => new
                {
                    NombreServicio = table.Column<string>(type: "VARCHAR2(150)", unicode: false, maxLength: 150, nullable: false),
                    UrlServicio = table.Column<string>(type: "VARCHAR2(4000)", unicode: false, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EcosistemaServicioExterno", x => x.NombreServicio);
                });

            migrationBuilder.CreateTable(
                name: "EmailIncorrecto",
                columns: table => new
                {
                    Email = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailIncorrecto", x => x.Email);
                });

            migrationBuilder.CreateTable(
                name: "EntradaBlog",
                columns: table => new
                {
                    BlogID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    EntradaBlogID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Titulo = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true),
                    Texto = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    AutorID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Visibilidad = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    Estado = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    Eliminado = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    Borrador = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Visitas = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Tags = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntradaBlog", x => new { x.BlogID, x.EntradaBlogID });
                });

            migrationBuilder.CreateTable(
                name: "FacetaConfigProyChart",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ChartID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Nombre = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Orden = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    SelectConsultaVirtuoso = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    FiltrosConsultaVirtuoso = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    JSBase = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    JSBusqueda = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Ontologias = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacetaConfigProyChart", x => new { x.OrganizacionID, x.ProyectoID, x.ChartID });
                });

            migrationBuilder.CreateTable(
                name: "FacetaConfigProyMapa",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    PropLatitud = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    PropLongitud = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    PropRuta = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    ColorRuta = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacetaConfigProyMapa", x => new { x.OrganizacionID, x.ProyectoID });
                });

            migrationBuilder.CreateTable(
                name: "FacetaConfigProyRangoFecha",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    PropiedadNueva = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    PropiedadInicio = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    PropiedadFin = table.Column<string>(type: "NVARCHAR2(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacetaConfigProyRangoFecha", x => new { x.OrganizacionID, x.ProyectoID, x.PropiedadNueva, x.PropiedadInicio, x.PropiedadFin });
                });

            migrationBuilder.CreateTable(
                name: "FacetaEntidadesExternas",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    EntidadID = table.Column<string>(type: "NVARCHAR2(300)", maxLength: 300, nullable: false),
                    Grafo = table.Column<string>(type: "NVARCHAR2(300)", maxLength: 300, nullable: false),
                    EsEntidadSecundaria = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    BuscarConRecursividad = table.Column<bool>(type: "NUMBER(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacetaEntidadesExternas", x => new { x.OrganizacionID, x.ProyectoID, x.EntidadID });
                });

            migrationBuilder.CreateTable(
                name: "FacetaExcluida",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Faceta = table.Column<string>(type: "NVARCHAR2(300)", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacetaExcluida", x => new { x.OrganizacionID, x.ProyectoID, x.Faceta });
                });

            migrationBuilder.CreateTable(
                name: "FacetaHome",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ObjetoConocimiento = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    Faceta = table.Column<string>(type: "NVARCHAR2(300)", maxLength: 300, nullable: false),
                    PestanyaFaceta = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Orden = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    MostrarVerMas = table.Column<bool>(type: "NUMBER(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacetaHome", x => new { x.OrganizacionID, x.ProyectoID, x.ObjetoConocimiento, x.Faceta });
                });

            migrationBuilder.CreateTable(
                name: "FacetaMultiple",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ObjetoConocimiento = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    Faceta = table.Column<string>(type: "NVARCHAR2(300)", maxLength: 300, nullable: false),
                    Consulta = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Filtro = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    NumeroFacetasObtener = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    NumeroFacetasDesplegar = table.Column<short>(type: "NUMBER(5)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacetaMultiple", x => new { x.OrganizacionID, x.ProyectoID, x.ObjetoConocimiento, x.Faceta });
                });

            migrationBuilder.CreateTable(
                name: "FacetaObjetoConocimiento",
                columns: table => new
                {
                    ObjetoConocimiento = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    Faceta = table.Column<string>(type: "NVARCHAR2(300)", maxLength: 300, nullable: false),
                    Nombre = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Orden = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    Autocompletar = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    TipoPropiedad = table.Column<short>(type: "NUMBER(5)", nullable: true),
                    TipoDisenio = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    ElementosVisibles = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    AlgoritmoTransformacion = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    EsSemantica = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    Mayusculas = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    EsPorDefecto = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    NombreFaceta = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    ComportamientoOr = table.Column<bool>(type: "NUMBER(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacetaObjetoConocimiento", x => new { x.ObjetoConocimiento, x.Faceta });
                });

            migrationBuilder.CreateTable(
                name: "FacetaRedireccion",
                columns: table => new
                {
                    Faceta = table.Column<string>(type: "NVARCHAR2(300)", maxLength: 300, nullable: false),
                    Nombre = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacetaRedireccion", x => x.Faceta);
                });

            migrationBuilder.CreateTable(
                name: "FichaBibliografica",
                columns: table => new
                {
                    FichaBibliograficaID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Nombre = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FichaBibliografica", x => x.FichaBibliograficaID);
                });

            migrationBuilder.CreateTable(
                name: "GeneralRolGrupoUsuario",
                columns: table => new
                {
                    GrupoUsuarioID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    RolPermitido = table.Column<string>(type: "CHAR(16)", unicode: false, fixedLength: true, maxLength: 16, nullable: false),
                    RolDenegado = table.Column<string>(type: "CHAR(16)", unicode: false, fixedLength: true, maxLength: 16, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneralRolGrupoUsuario", x => x.GrupoUsuarioID);
                });

            migrationBuilder.CreateTable(
                name: "GrupoAmigos",
                columns: table => new
                {
                    GrupoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Nombre = table.Column<string>(type: "NVARCHAR2(250)", maxLength: 250, nullable: false),
                    Fecha = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    IdentidadID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Tipo = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Automatico = table.Column<bool>(type: "NUMBER(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrupoAmigos", x => x.GrupoID);
                });

            migrationBuilder.CreateTable(
                name: "GrupoIdentidades",
                columns: table => new
                {
                    GrupoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Nombre = table.Column<string>(type: "NVARCHAR2(300)", maxLength: 300, nullable: false),
                    NombreCorto = table.Column<string>(type: "NVARCHAR2(300)", maxLength: 300, nullable: false),
                    Descripcion = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    FechaAlta = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    Tags = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Publico = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    PermitirEnviarMensajes = table.Column<bool>(type: "NUMBER(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrupoIdentidades", x => x.GrupoID);
                });

            migrationBuilder.CreateTable(
                name: "GrupoOrgParticipaProy",
                columns: table => new
                {
                    GrupoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    FechaAlta = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    TipoPerfil = table.Column<short>(type: "NUMBER(5)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrupoOrgParticipaProy", x => new { x.GrupoID, x.OrganizacionID, x.ProyectoID });
                });

            migrationBuilder.CreateTable(
                name: "GrupoUsuario",
                columns: table => new
                {
                    GrupoUsuarioID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Nombre = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    Descripcion = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrupoUsuario", x => x.GrupoUsuarioID);
                });

            migrationBuilder.CreateTable(
                name: "HistoricoProyectoUsuario",
                columns: table => new
                {
                    UsuarioID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OrganizacionGnossID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    IdentidadID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    FechaEntrada = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    FechaSalida = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoricoProyectoUsuario", x => new { x.UsuarioID, x.OrganizacionGnossID, x.ProyectoID, x.IdentidadID, x.FechaEntrada });
                });

            migrationBuilder.CreateTable(
                name: "IdentidadContadoresRecursos",
                columns: table => new
                {
                    IdentidadID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Tipo = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    NombreSem = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    Publicados = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Compartidos = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Comentarios = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentidadContadoresRecursos", x => new { x.IdentidadID, x.Tipo, x.NombreSem });
                });

            migrationBuilder.CreateTable(
                name: "IntegracionContinuaPropiedad",
                columns: table => new
                {
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    TipoObjeto = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    ObjetoPropiedad = table.Column<string>(type: "NVARCHAR2(250)", maxLength: 250, nullable: false),
                    TipoPropiedad = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    ValorPropiedad = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    ValorPropiedadDestino = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    MismoValor = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    Revisada = table.Column<bool>(type: "NUMBER(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntegracionContinuaPropiedad", x => new { x.ProyectoID, x.TipoObjeto, x.ObjetoPropiedad, x.TipoPropiedad });
                });

            migrationBuilder.CreateTable(
                name: "Notificacion",
                columns: table => new
                {
                    NotificacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Idioma = table.Column<string>(type: "NVARCHAR2(5)", maxLength: 5, nullable: true),
                    MensajeID = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    FechaNotificacion = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notificacion", x => x.NotificacionID);
                });

            migrationBuilder.CreateTable(
                name: "OntologiaProyecto",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OntologiaProyecto = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    NombreOnt = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Namespace = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    NamespacesExtra = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: true),
                    SubTipos = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    NombreCortoOnt = table.Column<string>(type: "VARCHAR2(4000)", unicode: false, nullable: true),
                    CachearDatosSemanticos = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    EsBuscable = table.Column<bool>(type: "NUMBER(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OntologiaProyecto", x => new { x.OrganizacionID, x.ProyectoID, x.OntologiaProyecto });
                });

            migrationBuilder.CreateTable(
                name: "Organizacion",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Nombre = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true),
                    Telefono = table.Column<string>(type: "NVARCHAR2(13)", maxLength: 13, nullable: true),
                    Email = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true),
                    Fax = table.Column<string>(type: "NVARCHAR2(13)", maxLength: 13, nullable: true),
                    Web = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true),
                    Logotipo = table.Column<byte[]>(type: "RAW(2000)", nullable: true),
                    PaisID = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    ProvinciaID = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    Provincia = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true),
                    OrganizacionPadreID = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    Direccion = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: true),
                    CP = table.Column<string>(type: "NVARCHAR2(15)", maxLength: 15, nullable: true),
                    Localidad = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true),
                    EsBuscable = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    EsBuscableExternos = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    ModoPersonal = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    Eliminada = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    NombreCorto = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    CoordenadasLogo = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: true),
                    TablaBaseOrganizacionID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Alias = table.Column<string>(type: "NVARCHAR2(80)", maxLength: 80, nullable: true),
                    VersionLogo = table.Column<int>(type: "NUMBER(10)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizacion", x => x.OrganizacionID);
                    table.ForeignKey(
                        name: "FK_Organizacion_Organizacion_OrganizacionPadreID",
                        column: x => x.OrganizacionPadreID,
                        principalTable: "Organizacion",
                        principalColumn: "OrganizacionID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrganizacionClase",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Centro = table.Column<string>(type: "NVARCHAR2(250)", maxLength: 250, nullable: false),
                    Asignatura = table.Column<string>(type: "NVARCHAR2(250)", maxLength: 250, nullable: false),
                    Curso = table.Column<string>(type: "NVARCHAR2(10)", maxLength: 10, nullable: false),
                    Grupo = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    CursoAcademico = table.Column<string>(type: "NVARCHAR2(10)", maxLength: 10, nullable: false),
                    NombreCortoCentro = table.Column<string>(type: "NVARCHAR2(10)", maxLength: 10, nullable: false),
                    NombreCortoAsig = table.Column<string>(type: "NVARCHAR2(10)", maxLength: 10, nullable: false),
                    TipoClase = table.Column<short>(type: "NUMBER(5)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizacionClase", x => x.OrganizacionID);
                });

            migrationBuilder.CreateTable(
                name: "Pais",
                columns: table => new
                {
                    PaisID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Nombre = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pais", x => x.PaisID);
                });

            migrationBuilder.CreateTable(
                name: "ParametroAplicacion",
                columns: table => new
                {
                    Parametro = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Valor = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParametroAplicacion", x => x.Parametro);
                });

            migrationBuilder.CreateTable(
                name: "ParametroGeneral",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    UmbralSuficienciaEnMejora = table.Column<float>(type: "BINARY_FLOAT", nullable: true),
                    DesviacionAdmitidaEnEvalua = table.Column<float>(type: "BINARY_FLOAT", nullable: true),
                    MetaAutomatPropietarioPro = table.Column<short>(type: "NUMBER(5)", nullable: true),
                    AvisoLegal = table.Column<string>(type: "NCLOB", maxLength: 4000, nullable: true),
                    WikiDisponible = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    BaseRecursosDisponible = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    CompartirRecursosPermitido = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    InvitacionesDisponibles = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    ServicioSuscripcionDisp = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    BlogsDisponibles = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    ForosDisponibles = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    EncuestasDisponibles = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    VotacionesDisponibles = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    ComentariosDisponibles = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    PreguntasDisponibles = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    DebatesDisponibles = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    BrightcoveDisponible = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    BrightcoveTokenWrite = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    BrightcoveTokenRead = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    BrightcoveReproductorID = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    LogoProyecto = table.Column<byte[]>(type: "RAW(2000)", nullable: true),
                    MensajeBienvenida = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    EntidadRevisadaObligatoria = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    UmbralDetPropietariosProc = table.Column<float>(type: "BINARY_FLOAT", nullable: true),
                    UmbralDetPropietariosObj = table.Column<float>(type: "BINARY_FLOAT", nullable: true),
                    UmbralDetPropietariosGF = table.Column<float>(type: "BINARY_FLOAT", nullable: true),
                    NombreDebilidadDafoProc = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: false),
                    NombreAmenazaDafoProc = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: false),
                    NombreFortalezaDafoProc = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: false),
                    NombreOportunidadDafoProc = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: false),
                    NombreDebilidadDafoObj = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: false),
                    NombreAmenazaDafoObj = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: false),
                    NombreFortalezaDafoObj = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: false),
                    NombreOportunidadDafoObj = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: false),
                    NombreDebilidadDafoGF = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: false),
                    NombreAmenazaDafoGF = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: false),
                    NombreFortalezaDafoGF = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: false),
                    NombreOportunidadDafoGF = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: false),
                    ImagenHome = table.Column<byte[]>(type: "RAW(2000)", nullable: true),
                    NombreImagenPeque = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    ImagenPersonalizadaPeque = table.Column<byte[]>(type: "RAW(2000)", nullable: true),
                    PermitirRevisionManualPro = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    PermitirRevisionManualGF = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    PermitirRevisionManualObj = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    PermitirRevisionManualComp = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    RutaTema = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    RutaImagenesTema = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    PermitirCertificacionRec = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    PoliticaCertificacion = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    CoordenadasHome = table.Column<string>(type: "NCHAR(30)", fixedLength: true, maxLength: 30, nullable: true),
                    ImagenHomeGrande = table.Column<byte[]>(type: "RAW(2000)", nullable: true),
                    DafoDisponible = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    PlantillaDisponible = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    CodigoGoogleAnalytics = table.Column<string>(type: "NVARCHAR2(15)", maxLength: 15, nullable: true),
                    VerVotaciones = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    VersionFotoImagenHomeGrande = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    VersionFotoImagenFondo = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    ClausulasRegistro = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    LicenciaPorDefecto = table.Column<string>(type: "NVARCHAR2(10)", maxLength: 10, nullable: true),
                    MensajeLicenciaPorDefecto = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    BrightcoveFTP = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    BrightcoveFTPUser = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    BrightcoveFTPPass = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    BrightcovePublisherID = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    VersionCSS = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    VersionJS = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    OcultarPersonalizacion = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    PestanyasDocSemanticos = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    PestanyaRecursosVisible = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    ScriptBusqueda = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    ImagenRelacionadosMini = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    CoordenadasMosaico = table.Column<string>(type: "NCHAR(30)", fixedLength: true, maxLength: 30, nullable: true),
                    CoordenadasSup = table.Column<string>(type: "NCHAR(30)", fixedLength: true, maxLength: 30, nullable: true),
                    VersionFotoImagenMosaicoGrande = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    VersionFotoImagenSupGrande = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    EsBeta = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    ScriptGoogleAnalytics = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: true),
                    NumeroRecursosRelacionados = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    GadgetsPieDisponibles = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    GadgetsCabeceraDisponibles = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    BiosCortas = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    RssDisponibles = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    RdfDisponibles = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    RegDidactalia = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    HomeVisible = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    CargasMasivasDisponibles = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    ComunidadGNOSS = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    IdiomasDisponibles = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    IdiomaDefecto = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    SupervisoresAdminGrupos = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    FechaNacimientoObligatoria = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    PrivacidadObligatoria = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    EventosDisponibles = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    SolicitarCoockieLogin = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    Copyright = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    CMSDisponible = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    VersionCSSWidget = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    InvitacionesPorContactoDisponibles = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    PermitirUsuNoLoginDescargDoc = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    TipoCabecera = table.Column<short>(type: "NUMBER(5)", nullable: true),
                    TipoFichaRecurso = table.Column<short>(type: "NUMBER(5)", nullable: true),
                    AvisoCookie = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    MostrarPersonasEnCatalogo = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    EnvioMensajesPermitido = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    EnlaceContactoPiePagina = table.Column<string>(type: "VARCHAR2(45)", unicode: false, maxLength: 45, nullable: true),
                    TieneSitemapComunidad = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    UrlServicioFichas = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true),
                    PermitirVotacionesNegativas = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    MostrarAccionesEnListados = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    PalcoActivado = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    PermitirRecursosPrivados = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    PlataformaVideoDisponible = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    TOPIDCuenta = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    TOPIDPlayer = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    TOPPublisherID = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    TOPFTPUser = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    TOPFTPPass = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    UrlMappingCategorias = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true),
                    AlgoritmoPersonasRecomendadas = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    PropsMapaPerYOrg = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    ChatDisponible = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    VersionCSSAdmin = table.Column<short>(type: "NUMBER(5)", nullable: true),
                    VersionJSAdmin = table.Column<short>(type: "NUMBER(5)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParametroGeneral", x => new { x.OrganizacionID, x.ProyectoID });
                });

            migrationBuilder.CreateTable(
                name: "ParametroProyecto",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Parametro = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Valor = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParametroProyecto", x => new { x.OrganizacionID, x.ProyectoID, x.Parametro });
                });

            migrationBuilder.CreateTable(
                name: "PerfilGadget",
                columns: table => new
                {
                    PerfilID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    GadgetID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Titulo = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    Contenido = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Orden = table.Column<short>(type: "NUMBER(5)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerfilGadget", x => new { x.PerfilID, x.GadgetID });
                });

            migrationBuilder.CreateTable(
                name: "PerfilOrganizacion",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    PerfilID = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerfilOrganizacion", x => new { x.PerfilID, x.OrganizacionID });
                });

            migrationBuilder.CreateTable(
                name: "PerfilRedesSociales",
                columns: table => new
                {
                    PerfilID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    NombreRedSocial = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    urlUsuario = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Usuario = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Token = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    TokenSecreto = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerfilRedesSociales", x => new { x.PerfilID, x.NombreRedSocial });
                });

            migrationBuilder.CreateTable(
                name: "PermisoAmigoOrg",
                columns: table => new
                {
                    IdentidadOrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    IdentidadUsuarioID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    IdentidadAmigoID = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermisoAmigoOrg", x => new { x.IdentidadOrganizacionID, x.IdentidadUsuarioID, x.IdentidadAmigoID });
                });

            migrationBuilder.CreateTable(
                name: "PermisoGrupoAmigoOrg",
                columns: table => new
                {
                    IdentidadOrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    IdentidadUsuarioID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    GrupoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    PermisoEdicion = table.Column<bool>(type: "NUMBER(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermisoGrupoAmigoOrg", x => new { x.IdentidadOrganizacionID, x.IdentidadUsuarioID, x.GrupoID });
                });

            migrationBuilder.CreateTable(
                name: "PermisosPaginasUsuarios",
                columns: table => new
                {
                    UsuarioID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Pagina = table.Column<short>(type: "NUMBER(5)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermisosPaginasUsuarios", x => new { x.UsuarioID, x.OrganizacionID, x.ProyectoID, x.Pagina });
                });

            migrationBuilder.CreateTable(
                name: "PersonaOcupacionFigura",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    EstructuraID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ElementoEstructuraID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    PersonaID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OrganizacionPersonalID = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    Dedicacion = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    EsPropietarioFigura = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    Orden = table.Column<short>(type: "NUMBER(5)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonaOcupacionFigura", x => new { x.OrganizacionID, x.ProyectoID, x.EstructuraID, x.ElementoEstructuraID, x.PersonaID });
                });

            migrationBuilder.CreateTable(
                name: "PersonaOcupacionFormaSec",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    EstructuraID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ElementoEstructuraID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    PersonaID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OrganizacionPersonalID = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    Dedicacion = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Orden = table.Column<short>(type: "NUMBER(5)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonaOcupacionFormaSec", x => new { x.OrganizacionID, x.ProyectoID, x.EstructuraID, x.ElementoEstructuraID, x.PersonaID });
                });

            migrationBuilder.CreateTable(
                name: "PersonaVisibleEnOrg",
                columns: table => new
                {
                    PersonaID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Orden = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonaVisibleEnOrg", x => new { x.PersonaID, x.OrganizacionID });
                });

            migrationBuilder.CreateTable(
                name: "Peticion",
                columns: table => new
                {
                    PeticionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    FechaPeticion = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    FechaProcesado = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    Tipo = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    Estado = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    UsuarioID = table.Column<Guid>(type: "RAW(16)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Peticion", x => x.PeticionID);
                });

            migrationBuilder.CreateTable(
                name: "PreferenciaProyecto",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    TesauroID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    CategoriaTesauroID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Orden = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreferenciaProyecto", x => new { x.OrganizacionID, x.ProyectoID, x.TesauroID, x.CategoriaTesauroID });
                });

            migrationBuilder.CreateTable(
                name: "Proyecto",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Nombre = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: false),
                    Descripcion = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    FechaInicio = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    FechaFin = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    TipoProyecto = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    TipoAcceso = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    NumeroRecursos = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    NumeroPreguntas = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    NumeroDebates = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    NumeroMiembros = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    NumeroOrgRegistradas = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    NumeroArticulos = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    NumeroDafos = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    NumeroForos = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    ProyectoSuperiorID = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    EsProyectoDestacado = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    URLPropia = table.Column<string>(type: "NVARCHAR2(250)", maxLength: 250, nullable: true),
                    NombreCorto = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    Estado = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    TieneTwitter = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    TagTwitter = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    UsuarioTwitter = table.Column<string>(type: "NVARCHAR2(15)", maxLength: 15, nullable: true),
                    TokenTwitter = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: true),
                    TokenSecretoTwitter = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: true),
                    EnviarTwitterComentario = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    EnviarTwitterNuevaCat = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    EnviarTwitterNuevoAdmin = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    EnviarTwitterNuevaPolitCert = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    EnviarTwitterNuevoTipoDoc = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    TablaBaseProyectoID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ProcesoVinculadoID = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    Tags = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    TagTwitterGnoss = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    NombrePresentacion = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proyecto", x => new { x.OrganizacionID, x.ProyectoID });
                    table.ForeignKey(
                        name: "FK_Proyecto_Proyecto_OrganizacionID_ProyectoSuperiorID",
                        columns: x => new { x.OrganizacionID, x.ProyectoSuperiorID },
                        principalTable: "Proyecto",
                        principalColumns: new[] { "OrganizacionID", "ProyectoID" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProyectoConfigExtraSem",
                columns: table => new
                {
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    UrlOntologia = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    SourceTesSem = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    Tipo = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    Nombre = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Idiomas = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    PrefijoTesSem = table.Column<string>(type: "NVARCHAR2(400)", maxLength: 400, nullable: true),
                    Editable = table.Column<bool>(type: "NUMBER(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProyectoConfigExtraSem", x => new { x.ProyectoID, x.UrlOntologia, x.SourceTesSem });
                });

            migrationBuilder.CreateTable(
                name: "ProyectoElementoHtml",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ElementoHeadID = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Ubicacion = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    Etiqueta = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    Atributos = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: true),
                    Contenido = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    CargarSinAceptarCookies = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    Privado = table.Column<bool>(type: "NUMBER(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProyectoElementoHtml", x => new { x.OrganizacionID, x.ProyectoID, x.ElementoHeadID });
                });

            migrationBuilder.CreateTable(
                name: "ProyectoEvento",
                columns: table => new
                {
                    EventoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Nombre = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Descripcion = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Activo = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    TipoEvento = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    InfoExtra = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    ComponenteID = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    Interno = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    Grupo = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true),
                    UrlRedirect = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProyectoEvento", x => x.EventoID);
                });

            migrationBuilder.CreateTable(
                name: "ProyectoEventoAccion",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Evento = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    AccionJS = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProyectoEventoAccion", x => new { x.OrganizacionID, x.ProyectoID, x.Evento });
                });

            migrationBuilder.CreateTable(
                name: "ProyectoGadgetContexto",
                columns: table => new
                {
                    GadgetID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ComunidadOrigen = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    ComunidadOrigenFiltros = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    FiltrosOrigenDestino = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    ComunidadDestinoFiltros = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    OrdenContexto = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Imagen = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    NumRecursos = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    ServicioResultados = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    ProyectoOrigenID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    MostrarEnlaceOriginal = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    OcultarVerMas = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    NamespacesExtra = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    ItemsBusqueda = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    ResultadosEliminar = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    NuevaPestanya = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    NombreCorto = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    ObtenerPrivados = table.Column<bool>(type: "NUMBER(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProyectoGadgetContexto", x => new { x.GadgetID, x.OrganizacionID, x.ProyectoID });
                });

            migrationBuilder.CreateTable(
                name: "ProyectoMetaRobots",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Tipo = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    Content = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProyectoMetaRobots", x => new { x.OrganizacionID, x.ProyectoID, x.Tipo });
                });

            migrationBuilder.CreateTable(
                name: "ProyectoPaginaHtml",
                columns: table => new
                {
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Nombre = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    Html = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Idioma = table.Column<string>(type: "NVARCHAR2(5)", maxLength: 5, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProyectoPaginaHtml", x => new { x.ProyectoID, x.Nombre });
                });

            migrationBuilder.CreateTable(
                name: "ProyectoPalabrasInapropiadas",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Tag = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    Rombos = table.Column<short>(type: "NUMBER(5)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProyectoPalabrasInapropiadas", x => new { x.OrganizacionID, x.ProyectoID, x.Tag });
                });

            migrationBuilder.CreateTable(
                name: "ProyectoPerfilNumElem",
                columns: table => new
                {
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    PerfilID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    NumRecursos = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProyectoPerfilNumElem", x => new { x.ProyectoID, x.PerfilID });
                });

            migrationBuilder.CreateTable(
                name: "ProyectoPestanya",
                columns: table => new
                {
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Nombre = table.Column<string>(type: "NVARCHAR2(400)", maxLength: 400, nullable: false),
                    Orden = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    Ruta = table.Column<string>(type: "NVARCHAR2(400)", maxLength: 400, nullable: false),
                    EsRutaInterna = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    EsSemantica = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    CampoFiltro = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: true),
                    NumeroRecursos = table.Column<short>(type: "NUMBER(5)", nullable: true),
                    VistaDisponible = table.Column<string>(type: "VARCHAR2(10)", unicode: false, maxLength: 10, nullable: true),
                    NuevaPestanya = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    MostrarFacetas = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    MostrarCajaBusqueda = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    ProyectoOrigenID = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    Visible = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    CMS = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    OcultarResultadosSinFiltros = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    posicionCentralMapa = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    NombrePestanyaPadre = table.Column<string>(type: "NVARCHAR2(400)", maxLength: 400, nullable: true),
                    GruposConfiguracion = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: true),
                    GruposPorTipo = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    TextoBusquedaSinResultado = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Privacidad = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    HTMLAlternativo = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProyectoPestanya", x => new { x.ProyectoID, x.Nombre });
                });

            migrationBuilder.CreateTable(
                name: "ProyectoPestanyaExportacionBusqueda",
                columns: table => new
                {
                    ExportacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    NombrePestanya = table.Column<string>(type: "NVARCHAR2(400)", maxLength: 400, nullable: false),
                    NombreExportacion = table.Column<string>(type: "NVARCHAR2(400)", maxLength: 400, nullable: false),
                    Orden = table.Column<short>(type: "NUMBER(5)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProyectoPestanyaExportacionBusqueda", x => x.ExportacionID);
                });

            migrationBuilder.CreateTable(
                name: "ProyectoRDFType",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    TipoDocumento = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    RdfType = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProyectoRDFType", x => new { x.OrganizacionID, x.ProyectoID, x.TipoDocumento });
                });

            migrationBuilder.CreateTable(
                name: "ProyectoRegistroObligatorio",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    VisibilidadUsuariosActivos = table.Column<short>(type: "NUMBER(5)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProyectoRegistroObligatorio", x => new { x.OrganizacionID, x.ProyectoID });
                });

            migrationBuilder.CreateTable(
                name: "ProyectoRolGrupoUsuario",
                columns: table => new
                {
                    OrganizacionGnossID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    GrupoUsuarioID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    RolPermitido = table.Column<string>(type: "CHAR(16)", unicode: false, fixedLength: true, maxLength: 16, nullable: false),
                    RolDenegado = table.Column<string>(type: "CHAR(16)", unicode: false, fixedLength: true, maxLength: 16, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProyectoRolGrupoUsuario", x => new { x.OrganizacionGnossID, x.ProyectoID, x.GrupoUsuarioID });
                });

            migrationBuilder.CreateTable(
                name: "ProyectoServicioWeb",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    AplicacionWeb = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    Nombre = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProyectoServicioWeb", x => new { x.OrganizacionID, x.ProyectoID, x.AplicacionWeb });
                });

            migrationBuilder.CreateTable(
                name: "ProyTipoRecNoActivReciente",
                columns: table => new
                {
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    TipoRecurso = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    OntologiasID = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProyTipoRecNoActivReciente", x => new { x.ProyectoID, x.TipoRecurso });
                });

            migrationBuilder.CreateTable(
                name: "RecursosRelacionadosPresentacion",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Orden = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    OntologiaID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Ontologia = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: false),
                    Propiedad = table.Column<string>(type: "NVARCHAR2(2000)", maxLength: 2000, nullable: false),
                    Nombre = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: true),
                    Imagen = table.Column<short>(type: "NUMBER(5)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecursosRelacionadosPresentacion", x => new { x.OrganizacionID, x.ProyectoID, x.Orden, x.OntologiaID });
                });

            migrationBuilder.CreateTable(
                name: "RedireccionRegistroRuta",
                columns: table => new
                {
                    RedireccionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    UrlOrigen = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Dominio = table.Column<string>(type: "NVARCHAR2(250)", maxLength: 250, nullable: false),
                    NombreParametro = table.Column<string>(type: "NVARCHAR2(250)", maxLength: 250, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RedireccionRegistroRuta", x => x.RedireccionID);
                });

            migrationBuilder.CreateTable(
                name: "ResultadoSuscripcion",
                columns: table => new
                {
                    SuscripcionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    RecursoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Titulo = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: false),
                    Descripcion = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    TipoResultado = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    AutorID = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    OrigenNombre = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: false),
                    OrigenNombreCorto = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    OrigenID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    TipoDocumento = table.Column<short>(type: "NUMBER(5)", nullable: true),
                    Enlace = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: true),
                    Leido = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    Sincaducidad = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    FechaProcesado = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResultadoSuscripcion", x => new { x.SuscripcionID, x.RecursoID });
                });

            migrationBuilder.CreateTable(
                name: "SeccionProyCatalogo",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OrganizacionBusquedaID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoBusquedaID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Orden = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    Nombre = table.Column<string>(type: "NVARCHAR2(550)", maxLength: 550, nullable: false),
                    Faceta = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Filtro = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true),
                    NumeroResultados = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    Tipo = table.Column<short>(type: "NUMBER(5)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeccionProyCatalogo", x => new { x.OrganizacionID, x.ProyectoID, x.OrganizacionBusquedaID, x.ProyectoBusquedaID, x.Orden });
                });

            migrationBuilder.CreateTable(
                name: "SitemapsIndex",
                columns: table => new
                {
                    Dominio = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    Sitemap = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Robots = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SitemapsIndex", x => x.Dominio);
                });

            migrationBuilder.CreateTable(
                name: "Solicitud",
                columns: table => new
                {
                    SolicitudID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    FechaSolicitud = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    FechaProcesado = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    Estado = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Solicitud", x => x.SolicitudID);
                });

            migrationBuilder.CreateTable(
                name: "Suscripcion",
                columns: table => new
                {
                    SuscripcionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    IdentidadID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Periodicidad = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Bloqueada = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    UltimoEnvio = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    FechaSuscripcion = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    ScoreUltimoEnvio = table.Column<int>(type: "NUMBER(10)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suscripcion", x => x.SuscripcionID);
                });

            migrationBuilder.CreateTable(
                name: "Tesauro",
                columns: table => new
                {
                    TesauroID = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tesauro", x => x.TesauroID);
                });

            migrationBuilder.CreateTable(
                name: "TextosPersonalizadosPersonalizacion",
                columns: table => new
                {
                    PersonalizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    TextoID = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Language = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Texto = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TextosPersonalizadosPersonalizacion", x => new { x.PersonalizacionID, x.TextoID, x.Language });
                });

            migrationBuilder.CreateTable(
                name: "TextosPersonalizadosPlataforma",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    TextoID = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Language = table.Column<string>(type: "NVARCHAR2(10)", maxLength: 10, nullable: false),
                    Texto = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TextosPersonalizadosPlataforma", x => new { x.OrganizacionID, x.ProyectoID, x.TextoID, x.Language });
                });

            migrationBuilder.CreateTable(
                name: "TextosPersonalizadosProyecto",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    TextoID = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Language = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Texto = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TextosPersonalizadosProyecto", x => new { x.OrganizacionID, x.ProyectoID, x.TextoID, x.Language });
                });

            migrationBuilder.CreateTable(
                name: "TipoDocDispRolUsuarioProy",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    TipoDocumento = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    RolUsuario = table.Column<short>(type: "NUMBER(5)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoDocDispRolUsuarioProy", x => new { x.OrganizacionID, x.ProyectoID, x.TipoDocumento, x.RolUsuario });
                });

            migrationBuilder.CreateTable(
                name: "TipoDocImagenPorDefecto",
                columns: table => new
                {
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    TipoRecurso = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    OntologiaID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    UrlImagen = table.Column<string>(type: "NVARCHAR2(300)", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoDocImagenPorDefecto", x => new { x.ProyectoID, x.TipoRecurso, x.OntologiaID });
                });

            migrationBuilder.CreateTable(
                name: "Tipologia",
                columns: table => new
                {
                    TipologiaID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    AtributoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Nombre = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: true),
                    Tipo = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Orden = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tipologia", x => new { x.TipologiaID, x.AtributoID });
                });

            migrationBuilder.CreateTable(
                name: "TipoOntoDispRolUsuarioProy",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OntologiaID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    RolUsuario = table.Column<short>(type: "NUMBER(5)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoOntoDispRolUsuarioProy", x => new { x.OrganizacionID, x.ProyectoID, x.OntologiaID, x.RolUsuario });
                });

            migrationBuilder.CreateTable(
                name: "TokenApiLogin",
                columns: table => new
                {
                    Token = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Login = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    Fecha = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TokenApiLogin", x => x.Token);
                });

            migrationBuilder.CreateTable(
                name: "UltimosDocumentosVisitados",
                columns: table => new
                {
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Documentos = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    FechaActualizacion = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UltimosDocumentosVisitados", x => x.ProyectoID);
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    UsuarioID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Login = table.Column<string>(type: "NVARCHAR2(12)", maxLength: 12, nullable: false),
                    Password = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    EstaBloqueado = table.Column<bool>(type: "NUMBER(1)", nullable: true),
                    NombreCorto = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    Version = table.Column<short>(type: "NUMBER(5)", nullable: true),
                    FechaCambioPassword = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    Validado = table.Column<short>(type: "NUMBER(5)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.UsuarioID);
                });

            migrationBuilder.CreateTable(
                name: "VistaVirtualPersonalizacion",
                columns: table => new
                {
                    PersonalizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VistaVirtualPersonalizacion", x => x.PersonalizacionID);
                });

            migrationBuilder.CreateTable(
                name: "Voto",
                columns: table => new
                {
                    VotoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    IdentidadID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ElementoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    IdentidadVotadaID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Voto = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    Tipo = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    FechaVotacion = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Voto", x => x.VotoID);
                });

            migrationBuilder.CreateTable(
                name: "BaseRecursosProyecto",
                columns: table => new
                {
                    BaseRecursosID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaseRecursosProyecto", x => new { x.BaseRecursosID, x.OrganizacionID, x.ProyectoID });
                    table.ForeignKey(
                        name: "FK_BaseRecursosProyecto_BaseRecursos_BaseRecursosID",
                        column: x => x.BaseRecursosID,
                        principalTable: "BaseRecursos",
                        principalColumn: "BaseRecursosID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BlogAgCatTesauro",
                columns: table => new
                {
                    TesauroID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    CategoriaTesauroID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    BlogID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Fecha = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogAgCatTesauro", x => new { x.TesauroID, x.CategoriaTesauroID, x.BlogID });
                    table.ForeignKey(
                        name: "FK_BlogAgCatTesauro_Blog_BlogID",
                        column: x => x.BlogID,
                        principalTable: "Blog",
                        principalColumn: "BlogID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BlogComunidad",
                columns: table => new
                {
                    BlogID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Compartido = table.Column<bool>(type: "NUMBER(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogComunidad", x => new { x.BlogID, x.ProyectoID, x.OrganizacionID });
                    table.ForeignKey(
                        name: "FK_BlogComunidad_Blog_BlogID",
                        column: x => x.BlogID,
                        principalTable: "Blog",
                        principalColumn: "BlogID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CargaPaquete",
                columns: table => new
                {
                    PaqueteID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    CargaID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    RutaOnto = table.Column<string>(type: "NVARCHAR2(2000)", maxLength: 2000, nullable: false),
                    RutaBusqueda = table.Column<string>(type: "NVARCHAR2(2000)", maxLength: 2000, nullable: false),
                    RutaSQL = table.Column<string>(type: "NVARCHAR2(2000)", maxLength: 2000, nullable: false),
                    Estado = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    Error = table.Column<string>(type: "NCLOB", maxLength: 4000, nullable: true),
                    FechaAlta = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    FechaProcesado = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    Ontologia = table.Column<string>(type: "NVARCHAR2(2000)", maxLength: 2000, nullable: false),
                    Comprimido = table.Column<bool>(type: "NUMBER(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CargaPaquete", x => x.PaqueteID);
                    table.ForeignKey(
                        name: "FK_CargaPaquete_Carga_CargaID",
                        column: x => x.CargaID,
                        principalTable: "Carga",
                        principalColumn: "CargaID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CMSComponenteRolGrupoIdentidades",
                columns: table => new
                {
                    ComponenteID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    GrupoID = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CMSComponenteRolGrupoIdentidades", x => new { x.ComponenteID, x.GrupoID });
                    table.ForeignKey(
                        name: "FK_CMSComponenteRolGrupoIdentidades_CMSComponente_ComponenteID",
                        column: x => x.ComponenteID,
                        principalTable: "CMSComponente",
                        principalColumn: "ComponenteID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CMSComponenteRolIdentidad",
                columns: table => new
                {
                    ComponenteID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    PerfilID = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CMSComponenteRolIdentidad", x => new { x.ComponenteID, x.PerfilID });
                    table.ForeignKey(
                        name: "FK_CMSComponenteRolIdentidad_CMSComponente_ComponenteID",
                        column: x => x.ComponenteID,
                        principalTable: "CMSComponente",
                        principalColumn: "ComponenteID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CMSPropiedadComponente",
                columns: table => new
                {
                    ComponenteID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    TipoPropiedadComponente = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    ValorPropiedad = table.Column<string>(type: "NCLOB", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CMSPropiedadComponente", x => new { x.ComponenteID, x.TipoPropiedadComponente });
                    table.ForeignKey(
                        name: "FK_CMSPropiedadComponente_CMSComponente_ComponenteID",
                        column: x => x.ComponenteID,
                        principalTable: "CMSComponente",
                        principalColumn: "ComponenteID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CMSBloque",
                columns: table => new
                {
                    BloqueID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Ubicacion = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    BloquePadreID = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    Orden = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    Estilos = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Borrador = table.Column<bool>(type: "NUMBER(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CMSBloque", x => x.BloqueID);
                    table.ForeignKey(
                        name: "FK_CMSBloque_CMSPagina_OrganizacionID_ProyectoID_Ubicacion",
                        columns: x => new { x.OrganizacionID, x.ProyectoID, x.Ubicacion },
                        principalTable: "CMSPagina",
                        principalColumns: new[] { "OrganizacionID", "ProyectoID", "Ubicacion" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ComentarioBlog",
                columns: table => new
                {
                    ComentarioID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    BlogID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    EntradaBlogID = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComentarioBlog", x => new { x.ComentarioID, x.BlogID, x.EntradaBlogID });
                    table.ForeignKey(
                        name: "FK_ComentarioBlog_Comentario_ComentarioID",
                        column: x => x.ComentarioID,
                        principalTable: "Comentario",
                        principalColumn: "ComentarioID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ComentarioCuestion",
                columns: table => new
                {
                    ComentarioID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    CuestionID = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComentarioCuestion", x => new { x.ComentarioID, x.CuestionID });
                    table.ForeignKey(
                        name: "FK_ComentarioCuestion_Comentario_ComentarioID",
                        column: x => x.ComentarioID,
                        principalTable: "Comentario",
                        principalColumn: "ComentarioID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DocumentoComentario",
                columns: table => new
                {
                    ComentarioID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    DocumentoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentoComentario", x => new { x.ComentarioID, x.DocumentoID });
                    table.ForeignKey(
                        name: "FK_DocumentoComentario_Comentario_ComentarioID",
                        column: x => x.ComentarioID,
                        principalTable: "Comentario",
                        principalColumn: "ComentarioID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ComparticionAutomaticaMapping",
                columns: table => new
                {
                    ComparticionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ReglaMapping = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    TesauroID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    CategoriaTesauroID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    GrupoMapping = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComparticionAutomaticaMapping", x => new { x.ComparticionID, x.ReglaMapping, x.TesauroID, x.CategoriaTesauroID });
                    table.ForeignKey(
                        name: "FK_ComparticionAutomaticaMapping_ComparticionAutomatica_ComparticionID",
                        column: x => x.ComparticionID,
                        principalTable: "ComparticionAutomatica",
                        principalColumn: "ComparticionID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ComparticionAutomaticaReglas",
                columns: table => new
                {
                    ComparticionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Regla = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    Navegacion = table.Column<string>(type: "VARCHAR2(10)", unicode: false, maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComparticionAutomaticaReglas", x => new { x.ComparticionID, x.Regla });
                    table.ForeignKey(
                        name: "FK_ComparticionAutomaticaReglas_ComparticionAutomatica_ComparticionID",
                        column: x => x.ComparticionID,
                        principalTable: "ComparticionAutomatica",
                        principalColumn: "ComparticionID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DatoExtraEcosistemaOpcion",
                columns: table => new
                {
                    DatoExtraID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OpcionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Orden = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Opcion = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatoExtraEcosistemaOpcion", x => new { x.DatoExtraID, x.OpcionID });
                    table.ForeignKey(
                        name: "FK_DatoExtraEcosistemaOpcion_DatoExtraEcosistema_DatoExtraID",
                        column: x => x.DatoExtraID,
                        principalTable: "DatoExtraEcosistema",
                        principalColumn: "DatoExtraID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DatoExtraProyectoOpcion",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    DatoExtraID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OpcionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Orden = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Opcion = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatoExtraProyectoOpcion", x => new { x.OrganizacionID, x.ProyectoID, x.DatoExtraID, x.OpcionID });
                    table.ForeignKey(
                        name: "FK_DatoExtraProyectoOpcion_DatoExtraProyecto_OrganizacionID_ProyectoID_DatoExtraID",
                        columns: x => new { x.OrganizacionID, x.ProyectoID, x.DatoExtraID },
                        principalTable: "DatoExtraProyecto",
                        principalColumns: new[] { "OrganizacionID", "ProyectoID", "DatoExtraID" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DatoExtraProyectoVirtuosoSolicitud",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    DatoExtraID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    SolicitudID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Opcion = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatoExtraProyectoVirtuosoSolicitud", x => new { x.OrganizacionID, x.ProyectoID, x.DatoExtraID, x.SolicitudID });
                    table.ForeignKey(
                        name: "FK_DatoExtraProyectoVirtuosoSolicitud_DatoExtraProyectoVirtuoso_OrganizacionID_ProyectoID_DatoExtraID",
                        columns: x => new { x.OrganizacionID, x.ProyectoID, x.DatoExtraID },
                        principalTable: "DatoExtraProyectoVirtuoso",
                        principalColumns: new[] { "OrganizacionID", "ProyectoID", "DatoExtraID" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DocumentoLecturaAumentada",
                columns: table => new
                {
                    DocumentoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    TituloAumentado = table.Column<string>(type: "VARCHAR2(4000)", unicode: false, nullable: true),
                    DescripcionAumentada = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Validada = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    EntitiesInfo = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    TopicsInfo = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentoLecturaAumentada", x => x.DocumentoID);
                    table.ForeignKey(
                        name: "FK_DocumentoLecturaAumentada_Documento_DocumentoID",
                        column: x => x.DocumentoID,
                        principalTable: "Documento",
                        principalColumn: "DocumentoID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentoRolGrupoIdentidades",
                columns: table => new
                {
                    DocumentoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    GrupoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Editor = table.Column<bool>(type: "NUMBER(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentoRolGrupoIdentidades", x => new { x.DocumentoID, x.GrupoID });
                    table.ForeignKey(
                        name: "FK_DocumentoRolGrupoIdentidades_Documento_DocumentoID",
                        column: x => x.DocumentoID,
                        principalTable: "Documento",
                        principalColumn: "DocumentoID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DocumentoRolIdentidad",
                columns: table => new
                {
                    DocumentoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    PerfilID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Editor = table.Column<bool>(type: "NUMBER(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentoRolIdentidad", x => new { x.DocumentoID, x.PerfilID });
                    table.ForeignKey(
                        name: "FK_DocumentoRolIdentidad_Documento_DocumentoID",
                        column: x => x.DocumentoID,
                        principalTable: "Documento",
                        principalColumn: "DocumentoID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DocumentoVincDoc",
                columns: table => new
                {
                    DocumentoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    DocumentoVincID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    IdentidadID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Fecha = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentoVincDoc", x => new { x.DocumentoID, x.DocumentoVincID });
                    table.ForeignKey(
                        name: "FK_DocumentoVincDoc_Documento_DocumentoID",
                        column: x => x.DocumentoID,
                        principalTable: "Documento",
                        principalColumn: "DocumentoID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DocumentoVincDoc_Documento_DocumentoVincID",
                        column: x => x.DocumentoVincID,
                        principalTable: "Documento",
                        principalColumn: "DocumentoID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HistorialDocumento",
                columns: table => new
                {
                    HistorialDocumentoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    DocumentoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    IdentidadID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Fecha = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    TagNombre = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    CategoriaTesauroID = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    Accion = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialDocumento", x => x.HistorialDocumentoID);
                    table.ForeignKey(
                        name: "FK_HistorialDocumento_Documento_DocumentoID",
                        column: x => x.DocumentoID,
                        principalTable: "Documento",
                        principalColumn: "DocumentoID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VersionDocumento",
                columns: table => new
                {
                    DocumentoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Version = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    DocumentoOriginalID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    IdentidadID = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VersionDocumento", x => x.DocumentoID);
                    table.ForeignKey(
                        name: "FK_VersionDocumento_Documento_DocumentoID",
                        column: x => x.DocumentoID,
                        principalTable: "Documento",
                        principalColumn: "DocumentoID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentoRespuestaVoto",
                columns: table => new
                {
                    DocumentoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    IdentidadID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    RespuestaID = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentoRespuestaVoto", x => new { x.DocumentoID, x.IdentidadID });
                    table.ForeignKey(
                        name: "FK_DocumentoRespuestaVoto_Documento_DocumentoID",
                        column: x => x.DocumentoID,
                        principalTable: "Documento",
                        principalColumn: "DocumentoID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DocumentoRespuestaVoto_DocumentoRespuesta_RespuestaID",
                        column: x => x.RespuestaID,
                        principalTable: "DocumentoRespuesta",
                        principalColumn: "RespuestaID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FacetaFiltroHome",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ObjetoConocimiento = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    Faceta = table.Column<string>(type: "NVARCHAR2(300)", maxLength: 300, nullable: false),
                    Orden = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    Filtro = table.Column<string>(type: "NVARCHAR2(280)", maxLength: 280, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacetaFiltroHome", x => new { x.OrganizacionID, x.ProyectoID, x.ObjetoConocimiento, x.Faceta, x.Orden });
                    table.ForeignKey(
                        name: "FK_FacetaFiltroHome_FacetaHome_OrganizacionID_ProyectoID_ObjetoConocimiento_Faceta",
                        columns: x => new { x.OrganizacionID, x.ProyectoID, x.ObjetoConocimiento, x.Faceta },
                        principalTable: "FacetaHome",
                        principalColumns: new[] { "OrganizacionID", "ProyectoID", "ObjetoConocimiento", "Faceta" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FacetaObjetoConocimientoProyecto",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ObjetoConocimiento = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    Faceta = table.Column<string>(type: "NVARCHAR2(300)", maxLength: 300, nullable: false),
                    Orden = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    Autocompletar = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    TipoPropiedad = table.Column<short>(type: "NUMBER(5)", nullable: true),
                    Comportamiento = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    MostrarSoloCaja = table.Column<bool>(type: "NUMBER(1)", nullable: true),
                    Excluida = table.Column<short>(type: "NUMBER(5)", nullable: true),
                    Oculta = table.Column<bool>(type: "NUMBER(1)", nullable: true),
                    TipoDisenio = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    ElementosVisibles = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    AlgoritmoTransformacion = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    NivelSemantico = table.Column<string>(type: "NVARCHAR2(5)", maxLength: 5, nullable: true),
                    EsSemantica = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    Mayusculas = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    NombreFaceta = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Excluyente = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    SubTipo = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Reciproca = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    FacetaPrivadaParaGrupoEditores = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    ComportamientoOr = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    OcultaEnFacetas = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    OcultaEnFiltros = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    Condicion = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    PriorizarOrdenResultados = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    Inmutable = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    AgrupacionID = table.Column<Guid>(type: "RAW(16)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacetaObjetoConocimientoProyecto", x => new { x.OrganizacionID, x.ProyectoID, x.ObjetoConocimiento, x.Faceta });
                });

            migrationBuilder.CreateTable(
                name: "AtributoFichaBibliografica",
                columns: table => new
                {
                    AtributoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    FichaBibliograficaID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Nombre = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: true),
                    Tipo = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Orden = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Longitud = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AtributoFichaBibliografica", x => new { x.AtributoID, x.FichaBibliograficaID });
                    table.ForeignKey(
                        name: "FK_AtributoFichaBibliografica_FichaBibliografica_FichaBibliograficaID",
                        column: x => x.FichaBibliograficaID,
                        principalTable: "FichaBibliografica",
                        principalColumn: "FichaBibliograficaID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AmigoAgGrupo",
                columns: table => new
                {
                    GrupoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    IdentidadID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    IdentidadAmigoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Fecha = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    GrupoAmigosGrupoID = table.Column<Guid>(type: "RAW(16)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AmigoAgGrupo", x => new { x.GrupoID, x.IdentidadID, x.IdentidadAmigoID });
                    table.ForeignKey(
                        name: "FK_AmigoAgGrupo_GrupoAmigos_GrupoAmigosGrupoID",
                        column: x => x.GrupoAmigosGrupoID,
                        principalTable: "GrupoAmigos",
                        principalColumn: "GrupoID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GrupoIdentidadesOrganizacion",
                columns: table => new
                {
                    GrupoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrupoIdentidadesOrganizacion", x => new { x.GrupoID, x.OrganizacionID });
                    table.ForeignKey(
                        name: "FK_GrupoIdentidadesOrganizacion_GrupoIdentidades_GrupoID",
                        column: x => x.GrupoID,
                        principalTable: "GrupoIdentidades",
                        principalColumn: "GrupoID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GrupoIdentidadesProyecto",
                columns: table => new
                {
                    GrupoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrupoIdentidadesProyecto", x => new { x.GrupoID, x.OrganizacionID, x.ProyectoID });
                    table.ForeignKey(
                        name: "FK_GrupoIdentidadesProyecto_GrupoIdentidades_GrupoID",
                        column: x => x.GrupoID,
                        principalTable: "GrupoIdentidades",
                        principalColumn: "GrupoID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Invitacion",
                columns: table => new
                {
                    InvitacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    TipoInvitacion = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    Estado = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    FechaInvitacion = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    FechaProcesado = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    NotificacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    IdentidadOrigenID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    IdentidadDestinoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ElementoVinculadoID = table.Column<Guid>(type: "RAW(16)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invitacion", x => x.InvitacionID);
                    table.ForeignKey(
                        name: "FK_Invitacion_Notificacion_NotificacionID",
                        column: x => x.NotificacionID,
                        principalTable: "Notificacion",
                        principalColumn: "NotificacionID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NotificacionAlertaPersona",
                columns: table => new
                {
                    NotificacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    PersonaID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    FechaLectura = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificacionAlertaPersona", x => new { x.NotificacionID, x.PersonaID });
                    table.ForeignKey(
                        name: "FK_NotificacionAlertaPersona_Notificacion_NotificacionID",
                        column: x => x.NotificacionID,
                        principalTable: "Notificacion",
                        principalColumn: "NotificacionID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NotificacionCorreoPersona",
                columns: table => new
                {
                    NotificacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    EmailEnvio = table.Column<string>(type: "NVARCHAR2(400)", maxLength: 400, nullable: false),
                    OrganizacionPersonaID = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    PersonaID = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    EstadoEnvio = table.Column<short>(type: "NUMBER(5)", nullable: true),
                    FechaEnvio = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    EnviadoRabbit = table.Column<bool>(type: "NUMBER(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificacionCorreoPersona", x => new { x.NotificacionID, x.EmailEnvio });
                    table.ForeignKey(
                        name: "FK_NotificacionCorreoPersona_Notificacion_NotificacionID",
                        column: x => x.NotificacionID,
                        principalTable: "Notificacion",
                        principalColumn: "NotificacionID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NotificacionEnvioMasivo",
                columns: table => new
                {
                    NotificacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Destinatarios = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Prioridad = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    EstadoEnvio = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    FechaEnvio = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificacionEnvioMasivo", x => x.NotificacionID);
                    table.ForeignKey(
                        name: "FK_NotificacionEnvioMasivo_Notificacion_NotificacionID",
                        column: x => x.NotificacionID,
                        principalTable: "Notificacion",
                        principalColumn: "NotificacionID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NotificacionParametro",
                columns: table => new
                {
                    NotificacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ParametroID = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    Valor = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificacionParametro", x => new { x.NotificacionID, x.ParametroID });
                    table.ForeignKey(
                        name: "FK_NotificacionParametro_Notificacion_NotificacionID",
                        column: x => x.NotificacionID,
                        principalTable: "Notificacion",
                        principalColumn: "NotificacionID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NotificacionParametroPersona",
                columns: table => new
                {
                    NotificacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ParametroID = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    PersonaID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Valor = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificacionParametroPersona", x => new { x.NotificacionID, x.ParametroID, x.PersonaID });
                    table.ForeignKey(
                        name: "FK_NotificacionParametroPersona_Notificacion_NotificacionID",
                        column: x => x.NotificacionID,
                        principalTable: "Notificacion",
                        principalColumn: "NotificacionID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NotificacionSuscripcion",
                columns: table => new
                {
                    NotificacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    SuscripcionID = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificacionSuscripcion", x => new { x.NotificacionID, x.SuscripcionID });
                    table.ForeignKey(
                        name: "FK_NotificacionSuscripcion_Notificacion_NotificacionID",
                        column: x => x.NotificacionID,
                        principalTable: "Notificacion",
                        principalColumn: "NotificacionID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BaseRecursosOrganizacion",
                columns: table => new
                {
                    BaseRecursosID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    EspacioMaxMyGnossMB = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    EspacioActualMyGnossMB = table.Column<double>(type: "BINARY_DOUBLE", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaseRecursosOrganizacion", x => new { x.BaseRecursosID, x.OrganizacionID });
                    table.ForeignKey(
                        name: "FK_BaseRecursosOrganizacion_BaseRecursos_BaseRecursosID",
                        column: x => x.BaseRecursosID,
                        principalTable: "BaseRecursos",
                        principalColumn: "BaseRecursosID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BaseRecursosOrganizacion_Organizacion_OrganizacionID",
                        column: x => x.OrganizacionID,
                        principalTable: "Organizacion",
                        principalColumn: "OrganizacionID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConfiguracionGnossOrg",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    VerRecursos = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    VerRecursosExterno = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    VisibilidadContactos = table.Column<short>(type: "NUMBER(5)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracionGnossOrg", x => x.OrganizacionID);
                    table.ForeignKey(
                        name: "FK_ConfiguracionGnossOrg_Organizacion_OrganizacionID",
                        column: x => x.OrganizacionID,
                        principalTable: "Organizacion",
                        principalColumn: "OrganizacionID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizacionEmpresa",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    CIF = table.Column<string>(type: "NVARCHAR2(9)", maxLength: 9, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    Empleados = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    TipoOrganizacion = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    SectorOrganizacion = table.Column<short>(type: "NUMBER(5)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizacionEmpresa", x => x.OrganizacionID);
                    table.ForeignKey(
                        name: "FK_OrganizacionEmpresa_Organizacion_OrganizacionID",
                        column: x => x.OrganizacionID,
                        principalTable: "Organizacion",
                        principalColumn: "OrganizacionID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizacionGnoss",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizacionGnoss", x => x.OrganizacionID);
                    table.ForeignKey(
                        name: "FK_OrganizacionGnoss_Organizacion_OrganizacionID",
                        column: x => x.OrganizacionID,
                        principalTable: "Organizacion",
                        principalColumn: "OrganizacionID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonaVinculoOrganizacion",
                columns: table => new
                {
                    PersonaID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Cargo = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true),
                    PaisTrabajoID = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    ProvinciaTrabajoID = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    ProvinciaTrabajo = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true),
                    CPTrabajo = table.Column<string>(type: "NVARCHAR2(15)", maxLength: 15, nullable: true),
                    DireccionTrabajo = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: true),
                    LocalidadTrabajo = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true),
                    TelefonoTrabajo = table.Column<string>(type: "NVARCHAR2(13)", maxLength: 13, nullable: true),
                    Extension = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    TelefonoMovilTrabajo = table.Column<string>(type: "NVARCHAR2(13)", maxLength: 13, nullable: true),
                    EmailTrabajo = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true),
                    CategoriaProfesionalID = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    TipoContratoID = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    FechaVinculacion = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Foto = table.Column<byte[]>(type: "RAW(2000)", nullable: true),
                    CoordenadasFoto = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: true),
                    UsarFotoPersonal = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    VersionFoto = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    FechaAnadidaFoto = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonaVinculoOrganizacion", x => new { x.PersonaID, x.OrganizacionID });
                    table.ForeignKey(
                        name: "FK_PersonaVinculoOrganizacion_Organizacion_OrganizacionID",
                        column: x => x.OrganizacionID,
                        principalTable: "Organizacion",
                        principalColumn: "OrganizacionID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Provincia",
                columns: table => new
                {
                    ProvinciaID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    PaisID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Nombre = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    CP = table.Column<string>(type: "NVARCHAR2(5)", maxLength: 5, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provincia", x => x.ProvinciaID);
                    table.ForeignKey(
                        name: "FK_Provincia_Pais_PaisID",
                        column: x => x.PaisID,
                        principalTable: "Pais",
                        principalColumn: "PaisID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PeticionInvitacionComunidad",
                columns: table => new
                {
                    PeticionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    NingID = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PeticionInvitacionComunidad", x => x.PeticionID);
                    table.ForeignKey(
                        name: "FK_PeticionInvitacionComunidad_Peticion_PeticionID",
                        column: x => x.PeticionID,
                        principalTable: "Peticion",
                        principalColumn: "PeticionID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PeticionInvitacionGrupo",
                columns: table => new
                {
                    PeticionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    GruposID = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PeticionInvitacionGrupo", x => x.PeticionID);
                    table.ForeignKey(
                        name: "FK_PeticionInvitacionGrupo_Peticion_PeticionID",
                        column: x => x.PeticionID,
                        principalTable: "Peticion",
                        principalColumn: "PeticionID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PeticionInvitaContacto",
                columns: table => new
                {
                    PeticionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    IdentidadID = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PeticionInvitaContacto", x => x.PeticionID);
                    table.ForeignKey(
                        name: "FK_PeticionInvitaContacto_Peticion_PeticionID",
                        column: x => x.PeticionID,
                        principalTable: "Peticion",
                        principalColumn: "PeticionID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PeticionNuevoProyecto",
                columns: table => new
                {
                    PeticionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Nombre = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: false),
                    NombreCorto = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Descripcion = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Tipo = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    ComunidadPrivadaPadreID = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    PerfilCreadorID = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PeticionNuevoProyecto", x => x.PeticionID);
                    table.ForeignKey(
                        name: "FK_PeticionNuevoProyecto_Peticion_PeticionID",
                        column: x => x.PeticionID,
                        principalTable: "Peticion",
                        principalColumn: "PeticionID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PeticionOrgInvitaPers",
                columns: table => new
                {
                    PeticionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Cargo = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true),
                    Email = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PeticionOrgInvitaPers", x => x.PeticionID);
                    table.ForeignKey(
                        name: "FK_PeticionOrgInvitaPers_Peticion_PeticionID",
                        column: x => x.PeticionID,
                        principalTable: "Peticion",
                        principalColumn: "PeticionID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AdministradorGrupoProyecto",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    GrupoID = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdministradorGrupoProyecto", x => new { x.OrganizacionID, x.ProyectoID, x.GrupoID });
                    table.ForeignKey(
                        name: "FK_AdministradorGrupoProyecto_Proyecto_OrganizacionID_ProyectoID",
                        columns: x => new { x.OrganizacionID, x.ProyectoID },
                        principalTable: "Proyecto",
                        principalColumns: new[] { "OrganizacionID", "ProyectoID" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AdministradorProyecto",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    UsuarioID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Tipo = table.Column<short>(type: "NUMBER(5)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdministradorProyecto", x => new { x.OrganizacionID, x.ProyectoID, x.UsuarioID, x.Tipo });
                    table.ForeignKey(
                        name: "FK_AdministradorProyecto_Proyecto_OrganizacionID_ProyectoID",
                        columns: x => new { x.OrganizacionID, x.ProyectoID },
                        principalTable: "Proyecto",
                        principalColumns: new[] { "OrganizacionID", "ProyectoID" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CategoriaProyectoCookie",
                columns: table => new
                {
                    CategoriaID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Nombre = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    NombreCorto = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    Descripcion = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    EsCategoriaTecnica = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriaProyectoCookie", x => x.CategoriaID);
                    table.ForeignKey(
                        name: "FK_CategoriaProyectoCookie_Proyecto_ProyectoID_OrganizacionID",
                        columns: x => new { x.ProyectoID, x.OrganizacionID },
                        principalTable: "Proyecto",
                        principalColumns: new[] { "ProyectoID", "OrganizacionID" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NivelCertificacion",
                columns: table => new
                {
                    NivelCertificacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Orden = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    Descripcion = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NivelCertificacion", x => x.NivelCertificacionID);
                    table.ForeignKey(
                        name: "FK_NivelCertificacion_Proyecto_OrganizacionID_ProyectoID",
                        columns: x => new { x.OrganizacionID, x.ProyectoID },
                        principalTable: "Proyecto",
                        principalColumns: new[] { "OrganizacionID", "ProyectoID" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PresentacionListadoSemantico",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OntologiaID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Orden = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    Ontologia = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: false),
                    Propiedad = table.Column<string>(type: "NVARCHAR2(2000)", maxLength: 2000, nullable: false),
                    Nombre = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PresentacionListadoSemantico", x => new { x.OrganizacionID, x.ProyectoID, x.OntologiaID, x.Orden });
                    table.ForeignKey(
                        name: "FK_PresentacionListadoSemantico_Proyecto_OrganizacionID_ProyectoID",
                        columns: x => new { x.OrganizacionID, x.ProyectoID },
                        principalTable: "Proyecto",
                        principalColumns: new[] { "OrganizacionID", "ProyectoID" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PresentacionMapaSemantico",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OntologiaID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Orden = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    Ontologia = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: false),
                    Propiedad = table.Column<string>(type: "NVARCHAR2(2000)", maxLength: 2000, nullable: false),
                    Nombre = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PresentacionMapaSemantico", x => new { x.OrganizacionID, x.ProyectoID, x.OntologiaID, x.Orden });
                    table.ForeignKey(
                        name: "FK_PresentacionMapaSemantico_Proyecto_OrganizacionID_ProyectoID",
                        columns: x => new { x.OrganizacionID, x.ProyectoID },
                        principalTable: "Proyecto",
                        principalColumns: new[] { "OrganizacionID", "ProyectoID" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PresentacionMosaicoSemantico",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OntologiaID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Orden = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    Ontologia = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: false),
                    Propiedad = table.Column<string>(type: "NVARCHAR2(2000)", maxLength: 2000, nullable: false),
                    Nombre = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PresentacionMosaicoSemantico", x => new { x.OrganizacionID, x.ProyectoID, x.OntologiaID, x.Orden });
                    table.ForeignKey(
                        name: "FK_PresentacionMosaicoSemantico_Proyecto_OrganizacionID_ProyectoID",
                        columns: x => new { x.OrganizacionID, x.ProyectoID },
                        principalTable: "Proyecto",
                        principalColumns: new[] { "OrganizacionID", "ProyectoID" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PresentacionPersonalizadoSemantico",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OntologiaID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Orden = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    Ontologia = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: false),
                    ID = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Select = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Where = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PresentacionPersonalizadoSemantico", x => new { x.OrganizacionID, x.ProyectoID, x.OntologiaID, x.Orden });
                    table.ForeignKey(
                        name: "FK_PresentacionPersonalizadoSemantico_Proyecto_OrganizacionID_ProyectoID",
                        columns: x => new { x.OrganizacionID, x.ProyectoID },
                        principalTable: "Proyecto",
                        principalColumns: new[] { "OrganizacionID", "ProyectoID" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProyectoAgCatTesauro",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    TesauroID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    CategoriaTesauroID = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProyectoAgCatTesauro", x => new { x.OrganizacionID, x.ProyectoID, x.TesauroID, x.CategoriaTesauroID });
                    table.ForeignKey(
                        name: "FK_ProyectoAgCatTesauro_Proyecto_OrganizacionID_ProyectoID",
                        columns: x => new { x.OrganizacionID, x.ProyectoID },
                        principalTable: "Proyecto",
                        principalColumns: new[] { "OrganizacionID", "ProyectoID" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProyectoCerradoTmp",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Motivo = table.Column<string>(type: "NVARCHAR2(2000)", maxLength: 2000, nullable: false),
                    FechaCierre = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    FechaReapertura = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProyectoCerradoTmp", x => new { x.OrganizacionID, x.ProyectoID });
                    table.ForeignKey(
                        name: "FK_ProyectoCerradoTmp_Proyecto_OrganizacionID_ProyectoID",
                        columns: x => new { x.OrganizacionID, x.ProyectoID },
                        principalTable: "Proyecto",
                        principalColumns: new[] { "OrganizacionID", "ProyectoID" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProyectoCerrandose",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    FechaCierre = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    PeriodoDeGracia = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProyectoCerrandose", x => new { x.OrganizacionID, x.ProyectoID });
                    table.ForeignKey(
                        name: "FK_ProyectoCerrandose_Proyecto_OrganizacionID_ProyectoID",
                        columns: x => new { x.OrganizacionID, x.ProyectoID },
                        principalTable: "Proyecto",
                        principalColumns: new[] { "OrganizacionID", "ProyectoID" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProyectoGadget",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    GadgetID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Titulo = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Contenido = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Orden = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Tipo = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    Ubicacion = table.Column<string>(type: "NVARCHAR2(10)", maxLength: 10, nullable: false),
                    Clases = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    TipoUbicacion = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    Visible = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    MultiIdioma = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    PersonalizacionComponenteID = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    CargarPorAjax = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    ComunidadDestinoFiltros = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    NombreCorto = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProyectoGadget", x => new { x.OrganizacionID, x.ProyectoID, x.GadgetID });
                    table.ForeignKey(
                        name: "FK_ProyectoGadget_Proyecto_OrganizacionID_ProyectoID",
                        columns: x => new { x.OrganizacionID, x.ProyectoID },
                        principalTable: "Proyecto",
                        principalColumns: new[] { "OrganizacionID", "ProyectoID" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProyectoGrafoFichaRec",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    GrafoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    PropEnlace = table.Column<string>(type: "NVARCHAR2(300)", maxLength: 300, nullable: false),
                    NodosLimiteNivel = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    Extra = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProyectoGrafoFichaRec", x => new { x.OrganizacionID, x.ProyectoID, x.GrafoID });
                    table.ForeignKey(
                        name: "FK_ProyectoGrafoFichaRec_Proyecto_OrganizacionID_ProyectoID",
                        columns: x => new { x.OrganizacionID, x.ProyectoID },
                        principalTable: "Proyecto",
                        principalColumns: new[] { "OrganizacionID", "ProyectoID" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProyectoLoginConfiguracion",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Mensaje = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProyectoLoginConfiguracion", x => new { x.OrganizacionID, x.ProyectoID });
                    table.ForeignKey(
                        name: "FK_ProyectoLoginConfiguracion_Proyecto_OrganizacionID_ProyectoID",
                        columns: x => new { x.OrganizacionID, x.ProyectoID },
                        principalTable: "Proyecto",
                        principalColumns: new[] { "OrganizacionID", "ProyectoID" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProyectoPasoRegistro",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Orden = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    PasoRegistro = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Obligatorio = table.Column<bool>(type: "NUMBER(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProyectoPasoRegistro", x => new { x.ProyectoID, x.OrganizacionID, x.Orden });
                    table.ForeignKey(
                        name: "FK_ProyectoPasoRegistro_Proyecto_OrganizacionID_ProyectoID",
                        columns: x => new { x.OrganizacionID, x.ProyectoID },
                        principalTable: "Proyecto",
                        principalColumns: new[] { "OrganizacionID", "ProyectoID" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProyectoPestanyaMenu",
                columns: table => new
                {
                    PestanyaID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    PestanyaPadreID = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    TipoPestanya = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    Nombre = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Ruta = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Orden = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    NuevaPestanya = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    Visible = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    Privacidad = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    HtmlAlternativo = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    IdiomasDisponibles = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Titulo = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    NombreCortoPestanya = table.Column<string>(type: "VARCHAR2(100)", unicode: false, maxLength: 100, nullable: false),
                    VisibleSinAcceso = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    CSSBodyClass = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Activa = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    MetaDescription = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProyectoPestanyaMenu", x => x.PestanyaID);
                    table.ForeignKey(
                        name: "FK_ProyectoPestanyaMenu_Proyecto_OrganizacionID_ProyectoID",
                        columns: x => new { x.OrganizacionID, x.ProyectoID },
                        principalTable: "Proyecto",
                        principalColumns: new[] { "OrganizacionID", "ProyectoID" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProyectoPestanyaMenu_ProyectoPestanyaMenu_PestanyaPadreID",
                        column: x => x.PestanyaPadreID,
                        principalTable: "ProyectoPestanyaMenu",
                        principalColumn: "PestanyaID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProyectoRelacionado",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OrganizacionRelacionadaID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoRelacionadoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Orden = table.Column<short>(type: "NUMBER(5)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProyectoRelacionado", x => new { x.OrganizacionID, x.ProyectoID, x.OrganizacionRelacionadaID, x.ProyectoRelacionadoID });
                    table.ForeignKey(
                        name: "FK_ProyectoRelacionado_Proyecto_OrganizacionID_ProyectoID",
                        columns: x => new { x.OrganizacionID, x.ProyectoID },
                        principalTable: "Proyecto",
                        principalColumns: new[] { "OrganizacionID", "ProyectoID" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProyectoRelacionado_Proyecto_OrganizacionRelacionadaID_ProyectoRelacionadoID",
                        columns: x => new { x.OrganizacionRelacionadaID, x.ProyectoRelacionadoID },
                        principalTable: "Proyecto",
                        principalColumns: new[] { "OrganizacionID", "ProyectoID" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProyectoSearchPersonalizado",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    NombreFiltro = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    WhereSPARQL = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    OrderBySPARQL = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    WhereFacetasSPARQL = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    OmitirRdfType = table.Column<bool>(type: "NUMBER(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProyectoSearchPersonalizado", x => new { x.OrganizacionID, x.ProyectoID, x.NombreFiltro });
                    table.ForeignKey(
                        name: "FK_ProyectoSearchPersonalizado_Proyecto_OrganizacionID_ProyectoID",
                        columns: x => new { x.OrganizacionID, x.ProyectoID },
                        principalTable: "Proyecto",
                        principalColumns: new[] { "OrganizacionID", "ProyectoID" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProyectoServicioExterno",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    NombreServicio = table.Column<string>(type: "VARCHAR2(150)", unicode: false, maxLength: 150, nullable: false),
                    UrlServicio = table.Column<string>(type: "VARCHAR2(4000)", unicode: false, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProyectoServicioExterno", x => new { x.OrganizacionID, x.ProyectoID, x.NombreServicio });
                    table.ForeignKey(
                        name: "FK_ProyectoServicioExterno_Proyecto_OrganizacionID_ProyectoID",
                        columns: x => new { x.OrganizacionID, x.ProyectoID },
                        principalTable: "Proyecto",
                        principalColumns: new[] { "OrganizacionID", "ProyectoID" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProyectosMasActivos",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Nombre = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: false),
                    NumeroConsultas = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Peso = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProyectosMasActivos", x => new { x.OrganizacionID, x.ProyectoID });
                    table.ForeignKey(
                        name: "FK_ProyectosMasActivos_Proyecto_OrganizacionID_ProyectoID",
                        columns: x => new { x.OrganizacionID, x.ProyectoID },
                        principalTable: "Proyecto",
                        principalColumns: new[] { "OrganizacionID", "ProyectoID" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProyectoElementoHTMLRol",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ElementoHeadID = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    GrupoID = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProyectoElementoHTMLRol", x => new { x.OrganizacionID, x.ProyectoID, x.ElementoHeadID, x.GrupoID });
                    table.ForeignKey(
                        name: "FK_ProyectoElementoHTMLRol_ProyectoElementoHtml_OrganizacionID_ProyectoID_ElementoHeadID",
                        columns: x => new { x.OrganizacionID, x.ProyectoID, x.ElementoHeadID },
                        principalTable: "ProyectoElementoHtml",
                        principalColumns: new[] { "OrganizacionID", "ProyectoID", "ElementoHeadID" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProyectoEventoParticipante",
                columns: table => new
                {
                    EventoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    IdentidadID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Fecha = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProyectoEventoParticipante", x => new { x.EventoID, x.IdentidadID });
                    table.ForeignKey(
                        name: "FK_ProyectoEventoParticipante_ProyectoEvento_EventoID",
                        column: x => x.EventoID,
                        principalTable: "ProyectoEvento",
                        principalColumn: "EventoID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProyectoPestanyaRolGrupoIdentidades",
                columns: table => new
                {
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Nombre = table.Column<string>(type: "NVARCHAR2(400)", maxLength: 400, nullable: false),
                    GrupoID = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProyectoPestanyaRolGrupoIdentidades", x => new { x.ProyectoID, x.Nombre, x.GrupoID });
                    table.ForeignKey(
                        name: "FK_ProyectoPestanyaRolGrupoIdentidades_ProyectoPestanya_ProyectoID_Nombre",
                        columns: x => new { x.ProyectoID, x.Nombre },
                        principalTable: "ProyectoPestanya",
                        principalColumns: new[] { "ProyectoID", "Nombre" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProyectoPestanyaRolIdentidad",
                columns: table => new
                {
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Nombre = table.Column<string>(type: "NVARCHAR2(400)", maxLength: 400, nullable: false),
                    PerfilID = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProyectoPestanyaRolIdentidad", x => new { x.ProyectoID, x.Nombre, x.PerfilID });
                    table.ForeignKey(
                        name: "FK_ProyectoPestanyaRolIdentidad_ProyectoPestanya_ProyectoID_Nombre",
                        columns: x => new { x.ProyectoID, x.Nombre },
                        principalTable: "ProyectoPestanya",
                        principalColumns: new[] { "ProyectoID", "Nombre" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProyectoSinRegistroObligatorio",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OrganizacionSinRegistroID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoSinRegistroID = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProyectoSinRegistroObligatorio", x => new { x.OrganizacionID, x.ProyectoID, x.OrganizacionSinRegistroID, x.ProyectoSinRegistroID });
                    table.ForeignKey(
                        name: "FK_ProyectoSinRegistroObligatorio_ProyectoRegistroObligatorio_OrganizacionSinRegistroID_ProyectoSinRegistroID",
                        columns: x => new { x.OrganizacionSinRegistroID, x.ProyectoSinRegistroID },
                        principalTable: "ProyectoRegistroObligatorio",
                        principalColumns: new[] { "OrganizacionID", "ProyectoID" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RedireccionValorParametro",
                columns: table => new
                {
                    RedireccionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ValorParametro = table.Column<string>(type: "NVARCHAR2(250)", maxLength: 250, nullable: false),
                    UrlRedireccion = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    MantenerFiltros = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    OrdenPresentacion = table.Column<short>(type: "NUMBER(5)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RedireccionValorParametro", x => new { x.RedireccionID, x.ValorParametro });
                    table.ForeignKey(
                        name: "FK_RedireccionValorParametro_RedireccionRegistroRuta_RedireccionID",
                        column: x => x.RedireccionID,
                        principalTable: "RedireccionRegistroRuta",
                        principalColumn: "RedireccionID",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateTable(
                name: "DatoExtraEcosistemaOpcionSolicitud",
                columns: table => new
                {
                    DatoExtraID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OpcionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    SolicitudID = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatoExtraEcosistemaOpcionSolicitud", x => new { x.DatoExtraID, x.OpcionID, x.SolicitudID });
                    table.ForeignKey(
                        name: "FK_DatoExtraEcosistemaOpcionSolicitud_Solicitud_SolicitudID",
                        column: x => x.SolicitudID,
                        principalTable: "Solicitud",
                        principalColumn: "SolicitudID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DatoExtraEcosistemaVirtuosoSolicitud",
                columns: table => new
                {
                    DatoExtraID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    SolicitudID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Opcion = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatoExtraEcosistemaVirtuosoSolicitud", x => new { x.DatoExtraID, x.Opcion, x.SolicitudID });
                    table.ForeignKey(
                        name: "FK_DatoExtraEcosistemaVirtuosoSolicitud_Solicitud_SolicitudID",
                        column: x => x.SolicitudID,
                        principalTable: "Solicitud",
                        principalColumn: "SolicitudID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DatoExtraProyectoOpcionSolicitud",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    DatoExtraID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OpcionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    SolicitudID = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatoExtraProyectoOpcionSolicitud", x => new { x.OrganizacionID, x.ProyectoID, x.DatoExtraID, x.OpcionID, x.SolicitudID });
                    table.ForeignKey(
                        name: "FK_DatoExtraProyectoOpcionSolicitud_Solicitud_SolicitudID",
                        column: x => x.SolicitudID,
                        principalTable: "Solicitud",
                        principalColumn: "SolicitudID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NotificacionSolicitud",
                columns: table => new
                {
                    NotificacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    SolicitudID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificacionSolicitud", x => new { x.NotificacionID, x.SolicitudID, x.OrganizacionID, x.ProyectoID });
                    table.ForeignKey(
                        name: "FK_NotificacionSolicitud_Notificacion_NotificacionID",
                        column: x => x.NotificacionID,
                        principalTable: "Notificacion",
                        principalColumn: "NotificacionID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NotificacionSolicitud_Solicitud_SolicitudID",
                        column: x => x.SolicitudID,
                        principalTable: "Solicitud",
                        principalColumn: "SolicitudID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SolicitudGrupo",
                columns: table => new
                {
                    SolicitudID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    GrupoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    IdentidadID = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudGrupo", x => new { x.SolicitudID, x.GrupoID, x.IdentidadID });
                    table.ForeignKey(
                        name: "FK_SolicitudGrupo_Solicitud_SolicitudID",
                        column: x => x.SolicitudID,
                        principalTable: "Solicitud",
                        principalColumn: "SolicitudID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SolicitudNuevaOrganizacion",
                columns: table => new
                {
                    SolicitudID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    UsuarioAdminID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Nombre = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true),
                    URLFoto = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true),
                    PaisID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProvinciaID = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    Provincia = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true),
                    CP = table.Column<string>(type: "NVARCHAR2(15)", maxLength: 15, nullable: true),
                    Poblacion = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    Direccion = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    PaginaWeb = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true),
                    EsBuscable = table.Column<bool>(type: "NUMBER(1)", nullable: true),
                    EsBuscableExternos = table.Column<bool>(type: "NUMBER(1)", nullable: true),
                    CargoContactoPrincipal = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true),
                    EmailContactoPrincipal = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    ModoPersonal = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    NombreCorto = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    Alias = table.Column<string>(type: "NVARCHAR2(80)", maxLength: 80, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudNuevaOrganizacion", x => new { x.SolicitudID, x.UsuarioAdminID });
                    table.ForeignKey(
                        name: "FK_SolicitudNuevaOrganizacion_Solicitud_SolicitudID",
                        column: x => x.SolicitudID,
                        principalTable: "Solicitud",
                        principalColumn: "SolicitudID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SolicitudNuevoProfesor",
                columns: table => new
                {
                    SolicitudID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    UsuarioID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Email = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    CentroEstudios = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    AreaEstudios = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudNuevoProfesor", x => new { x.SolicitudID, x.UsuarioID });
                    table.ForeignKey(
                        name: "FK_SolicitudNuevoProfesor_Solicitud_SolicitudID",
                        column: x => x.SolicitudID,
                        principalTable: "Solicitud",
                        principalColumn: "SolicitudID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SolicitudNuevoUsuario",
                columns: table => new
                {
                    SolicitudID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    UsuarioID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Nombre = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    Apellidos = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    URLFoto = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true),
                    PaisID = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    ProvinciaID = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    Provincia = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true),
                    CP = table.Column<string>(type: "NVARCHAR2(15)", maxLength: 15, nullable: true),
                    Poblacion = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true),
                    Sexo = table.Column<string>(type: "CHAR(1)", unicode: false, fixedLength: true, maxLength: 1, nullable: true),
                    FechaNacimiento = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    EsBuscable = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    EsBuscableExterno = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    Email = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true),
                    Idioma = table.Column<string>(type: "NVARCHAR2(5)", maxLength: 5, nullable: false),
                    NombreCorto = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    EmailTutor = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true),
                    CrearClase = table.Column<bool>(type: "NUMBER(1)", nullable: true),
                    ClausulasAdicionales = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: true),
                    CambioPassword = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    ProyectosAutoAcceso = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    FaltanDatos = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    TipoRegistro = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    Direccion = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudNuevoUsuario", x => new { x.UsuarioID, x.SolicitudID });
                    table.ForeignKey(
                        name: "FK_SolicitudNuevoUsuario_Solicitud_SolicitudID",
                        column: x => x.SolicitudID,
                        principalTable: "Solicitud",
                        principalColumn: "SolicitudID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SolicitudOrganizacion",
                columns: table => new
                {
                    SolicitudID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudOrganizacion", x => new { x.SolicitudID, x.OrganizacionID });
                    table.ForeignKey(
                        name: "FK_SolicitudOrganizacion_Solicitud_SolicitudID",
                        column: x => x.SolicitudID,
                        principalTable: "Solicitud",
                        principalColumn: "SolicitudID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SolicitudUsuario",
                columns: table => new
                {
                    SolicitudID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    UsuarioID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    PersonaID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    PerfilID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ClausulasAdicionales = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudUsuario", x => new { x.SolicitudID, x.UsuarioID, x.PersonaID });
                    table.ForeignKey(
                        name: "FK_SolicitudUsuario_Solicitud_SolicitudID",
                        column: x => x.SolicitudID,
                        principalTable: "Solicitud",
                        principalColumn: "SolicitudID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CategoriaTesVinSuscrip",
                columns: table => new
                {
                    SuscripcionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    TesauroID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    CategoriaTesauroID = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriaTesVinSuscrip", x => new { x.SuscripcionID, x.TesauroID, x.CategoriaTesauroID });
                    table.ForeignKey(
                        name: "FK_CategoriaTesVinSuscrip_Suscripcion_SuscripcionID",
                        column: x => x.SuscripcionID,
                        principalTable: "Suscripcion",
                        principalColumn: "SuscripcionID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SuscripcionBlog",
                columns: table => new
                {
                    SuscripcionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    BlogID = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SuscripcionBlog", x => new { x.BlogID, x.SuscripcionID });
                    table.ForeignKey(
                        name: "FK_SuscripcionBlog_Suscripcion_SuscripcionID",
                        column: x => x.SuscripcionID,
                        principalTable: "Suscripcion",
                        principalColumn: "SuscripcionID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SuscripcionIdentidadProyecto",
                columns: table => new
                {
                    SuscripcionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    IdentidadID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SuscripcionIdentidadProyecto", x => new { x.IdentidadID, x.SuscripcionID });
                    table.ForeignKey(
                        name: "FK_SuscripcionIdentidadProyecto_Suscripcion_SuscripcionID",
                        column: x => x.SuscripcionID,
                        principalTable: "Suscripcion",
                        principalColumn: "SuscripcionID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SuscripcionTesauroOrganizacion",
                columns: table => new
                {
                    SuscripcionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    TesauroID = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SuscripcionTesauroOrganizacion", x => x.SuscripcionID);
                    table.ForeignKey(
                        name: "FK_SuscripcionTesauroOrganizacion_Suscripcion_SuscripcionID",
                        column: x => x.SuscripcionID,
                        principalTable: "Suscripcion",
                        principalColumn: "SuscripcionID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SuscripcionTesauroProyecto",
                columns: table => new
                {
                    SuscripcionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    TesauroID = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SuscripcionTesauroProyecto", x => x.SuscripcionID);
                    table.ForeignKey(
                        name: "FK_SuscripcionTesauroProyecto_Suscripcion_SuscripcionID",
                        column: x => x.SuscripcionID,
                        principalTable: "Suscripcion",
                        principalColumn: "SuscripcionID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SuscripcionTesauroUsuario",
                columns: table => new
                {
                    SuscripcionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    UsuarioID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    TesauroID = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SuscripcionTesauroUsuario", x => x.SuscripcionID);
                    table.ForeignKey(
                        name: "FK_SuscripcionTesauroUsuario_Suscripcion_SuscripcionID",
                        column: x => x.SuscripcionID,
                        principalTable: "Suscripcion",
                        principalColumn: "SuscripcionID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CategoriaTesauro",
                columns: table => new
                {
                    TesauroID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    CategoriaTesauroID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Nombre = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: false),
                    Orden = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    NumeroRecursos = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    NumeroPreguntas = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    NumeroDebates = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    NumeroDafos = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    TieneFoto = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    VersionFoto = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    Estructurante = table.Column<short>(type: "NUMBER(5)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriaTesauro", x => new { x.TesauroID, x.CategoriaTesauroID });
                    table.ForeignKey(
                        name: "FK_CategoriaTesauro_Tesauro_TesauroID",
                        column: x => x.TesauroID,
                        principalTable: "Tesauro",
                        principalColumn: "TesauroID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TesauroOrganizacion",
                columns: table => new
                {
                    TesauroID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    CategoriaTesauroPublicoID = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    CategoriaTesauroPrivadoID = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    CategoriaTesauroFavoritosID = table.Column<Guid>(type: "RAW(16)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TesauroOrganizacion", x => new { x.TesauroID, x.OrganizacionID });
                    table.ForeignKey(
                        name: "FK_TesauroOrganizacion_Tesauro_TesauroID",
                        column: x => x.TesauroID,
                        principalTable: "Tesauro",
                        principalColumn: "TesauroID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TesauroProyecto",
                columns: table => new
                {
                    TesauroID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    IdiomaDefecto = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TesauroProyecto", x => new { x.TesauroID, x.OrganizacionID, x.ProyectoID });
                    table.ForeignKey(
                        name: "FK_TesauroProyecto_Tesauro_TesauroID",
                        column: x => x.TesauroID,
                        principalTable: "Tesauro",
                        principalColumn: "TesauroID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DocumentoTipologia",
                columns: table => new
                {
                    DocumentoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    TipologiaID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    AtributoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Valor = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentoTipologia", x => new { x.DocumentoID, x.TipologiaID, x.AtributoID });
                    table.ForeignKey(
                        name: "FK_DocumentoTipologia_Tipologia_TipologiaID_AtributoID",
                        columns: x => new { x.TipologiaID, x.AtributoID },
                        principalTable: "Tipologia",
                        principalColumns: new[] { "TipologiaID", "AtributoID" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AdministradorGeneral",
                columns: table => new
                {
                    UsuarioID = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdministradorGeneral", x => x.UsuarioID);
                    table.ForeignKey(
                        name: "FK_AdministradorGeneral_Usuario_UsuarioID",
                        column: x => x.UsuarioID,
                        principalTable: "Usuario",
                        principalColumn: "UsuarioID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AdministradorOrganizacion",
                columns: table => new
                {
                    UsuarioID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Tipo = table.Column<short>(type: "NUMBER(5)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdministradorOrganizacion", x => new { x.UsuarioID, x.OrganizacionID, x.Tipo });
                    table.ForeignKey(
                        name: "FK_AdministradorOrganizacion_Organizacion_OrganizacionID",
                        column: x => x.OrganizacionID,
                        principalTable: "Organizacion",
                        principalColumn: "OrganizacionID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AdministradorOrganizacion_Usuario_UsuarioID",
                        column: x => x.UsuarioID,
                        principalTable: "Usuario",
                        principalColumn: "UsuarioID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BaseRecursosUsuario",
                columns: table => new
                {
                    BaseRecursosID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    UsuarioID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    EspacioMaxMyGnossMB = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    EspacioActualMyGnossMB = table.Column<double>(type: "BINARY_DOUBLE", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaseRecursosUsuario", x => new { x.BaseRecursosID, x.UsuarioID });
                    table.ForeignKey(
                        name: "FK_BaseRecursosUsuario_BaseRecursos_BaseRecursosID",
                        column: x => x.BaseRecursosID,
                        principalTable: "BaseRecursos",
                        principalColumn: "BaseRecursosID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BaseRecursosUsuario_Usuario_UsuarioID",
                        column: x => x.UsuarioID,
                        principalTable: "Usuario",
                        principalColumn: "UsuarioID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GeneralRolUsuario",
                columns: table => new
                {
                    UsuarioID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    RolPermitido = table.Column<string>(type: "CHAR(16)", unicode: false, fixedLength: true, maxLength: 16, nullable: false),
                    RolDenegado = table.Column<string>(type: "CHAR(16)", unicode: false, fixedLength: true, maxLength: 16, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneralRolUsuario", x => x.UsuarioID);
                    table.ForeignKey(
                        name: "FK_GeneralRolUsuario_Usuario_UsuarioID",
                        column: x => x.UsuarioID,
                        principalTable: "Usuario",
                        principalColumn: "UsuarioID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GrupoUsuarioUsuario",
                columns: table => new
                {
                    UsuarioID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    GrupoUsuarioID = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrupoUsuarioUsuario", x => new { x.UsuarioID, x.GrupoUsuarioID });
                    table.ForeignKey(
                        name: "FK_GrupoUsuarioUsuario_Usuario_UsuarioID",
                        column: x => x.UsuarioID,
                        principalTable: "Usuario",
                        principalColumn: "UsuarioID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InicioSesion",
                columns: table => new
                {
                    UsuarioID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OrganizacionGnossID = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    PersonaID = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InicioSesion", x => x.UsuarioID);
                    table.ForeignKey(
                        name: "FK_InicioSesion_Usuario_UsuarioID",
                        column: x => x.UsuarioID,
                        principalTable: "Usuario",
                        principalColumn: "UsuarioID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizacionRolUsuario",
                columns: table => new
                {
                    UsuarioID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    RolPermitido = table.Column<string>(type: "CHAR(16)", unicode: false, fixedLength: true, maxLength: 16, nullable: false),
                    RolDenegado = table.Column<string>(type: "CHAR(16)", unicode: false, fixedLength: true, maxLength: 16, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizacionRolUsuario", x => new { x.UsuarioID, x.OrganizacionID });
                    table.ForeignKey(
                        name: "FK_OrganizacionRolUsuario_Usuario_UsuarioID",
                        column: x => x.UsuarioID,
                        principalTable: "Usuario",
                        principalColumn: "UsuarioID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Persona",
                columns: table => new
                {
                    PersonaID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    UsuarioID = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    Nombre = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    Apellidos = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true),
                    TipoDocumentoAcreditativo = table.Column<short>(type: "NUMBER(5)", nullable: true),
                    ValorDocumentoAcreditativo = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    Foto = table.Column<byte[]>(type: "RAW(2000)", nullable: true),
                    Sexo = table.Column<string>(type: "CHAR(1)", unicode: false, fixedLength: true, maxLength: 1, nullable: true),
                    FechaNacimiento = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    PaisPersonalID = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    ProvinciaPersonalID = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    ProvinciaPersonal = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true),
                    LocalidadPersonal = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true),
                    CPPersonal = table.Column<string>(type: "NVARCHAR2(15)", maxLength: 15, nullable: true),
                    DireccionPersonal = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: true),
                    TelefonoPersonal = table.Column<string>(type: "NVARCHAR2(13)", maxLength: 13, nullable: true),
                    Email = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true),
                    EmailTutor = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true),
                    EstadoCivilID = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    TitulacionID = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    Hijos = table.Column<short>(type: "NUMBER(5)", nullable: true),
                    EsBuscable = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    EsBuscableExternos = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    Eliminado = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    CoordenadasFoto = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: true),
                    Idioma = table.Column<string>(type: "NVARCHAR2(5)", maxLength: 5, nullable: false),
                    VersionFoto = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    FechaNotificacionCorreccion = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    EstadoCorreccion = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    FechaAnadidaFoto = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Persona", x => x.PersonaID);
                    table.ForeignKey(
                        name: "FK_Persona_Usuario_UsuarioID",
                        column: x => x.UsuarioID,
                        principalTable: "Usuario",
                        principalColumn: "UsuarioID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProyectoRolUsuario",
                columns: table => new
                {
                    OrganizacionGnossID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    UsuarioID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    RolPermitido = table.Column<string>(type: "CHAR(16)", unicode: false, fixedLength: true, maxLength: 16, nullable: false),
                    RolDenegado = table.Column<string>(type: "CHAR(16)", unicode: false, fixedLength: true, maxLength: 16, nullable: true),
                    EstaBloqueado = table.Column<bool>(type: "NUMBER(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProyectoRolUsuario", x => new { x.OrganizacionGnossID, x.ProyectoID, x.UsuarioID });
                    table.ForeignKey(
                        name: "FK_ProyectoRolUsuario_Usuario_UsuarioID",
                        column: x => x.UsuarioID,
                        principalTable: "Usuario",
                        principalColumn: "UsuarioID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TesauroUsuario",
                columns: table => new
                {
                    TesauroID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    UsuarioID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    CategoriaTesauroPublicoID = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    CategoriaTesauroPrivadoID = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    CategoriaTesauroMisImagenesID = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    CategoriaTesauroMisVideosID = table.Column<Guid>(type: "RAW(16)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TesauroUsuario", x => new { x.TesauroID, x.UsuarioID });
                    table.ForeignKey(
                        name: "FK_TesauroUsuario_Tesauro_TesauroID",
                        column: x => x.TesauroID,
                        principalTable: "Tesauro",
                        principalColumn: "TesauroID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TesauroUsuario_Usuario_UsuarioID",
                        column: x => x.UsuarioID,
                        principalTable: "Usuario",
                        principalColumn: "UsuarioID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioContadores",
                columns: table => new
                {
                    UsuarioID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    NumeroAccesos = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    FechaUltimaVisita = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioContadores", x => x.UsuarioID);
                    table.ForeignKey(
                        name: "FK_UsuarioContadores_Usuario_UsuarioID",
                        column: x => x.UsuarioID,
                        principalTable: "Usuario",
                        principalColumn: "UsuarioID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioRedirect",
                columns: table => new
                {
                    UsuarioID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    UrlRedirect = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioRedirect", x => x.UsuarioID);
                    table.ForeignKey(
                        name: "FK_UsuarioRedirect_Usuario_UsuarioID",
                        column: x => x.UsuarioID,
                        principalTable: "Usuario",
                        principalColumn: "UsuarioID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioVinculadoLoginRedesSociales",
                columns: table => new
                {
                    UsuarioID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    TipoRedSocial = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    IDenRedSocial = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioVinculadoLoginRedesSociales", x => new { x.UsuarioID, x.TipoRedSocial });
                    table.ForeignKey(
                        name: "FK_UsuarioVinculadoLoginRedesSociales_Usuario_UsuarioID",
                        column: x => x.UsuarioID,
                        principalTable: "Usuario",
                        principalColumn: "UsuarioID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VistaVirtual",
                columns: table => new
                {
                    PersonalizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    TipoPagina = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    HTML = table.Column<string>(type: "NCLOB", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VistaVirtual", x => new { x.PersonalizacionID, x.TipoPagina });
                    table.ForeignKey(
                        name: "FK_VistaVirtual_VistaVirtualPersonalizacion_PersonalizacionID",
                        column: x => x.PersonalizacionID,
                        principalTable: "VistaVirtualPersonalizacion",
                        principalColumn: "PersonalizacionID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VistaVirtualCMS",
                columns: table => new
                {
                    PersonalizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    TipoComponente = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    PersonalizacionComponenteID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    HTML = table.Column<string>(type: "NCLOB", nullable: false),
                    Nombre = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    DatosExtra = table.Column<string>(type: "NCLOB", unicode: false, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VistaVirtualCMS", x => new { x.PersonalizacionID, x.TipoComponente, x.PersonalizacionComponenteID });
                    table.ForeignKey(
                        name: "FK_VistaVirtualCMS_VistaVirtualPersonalizacion_PersonalizacionID",
                        column: x => x.PersonalizacionID,
                        principalTable: "VistaVirtualPersonalizacion",
                        principalColumn: "PersonalizacionID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VistaVirtualGadgetRecursos",
                columns: table => new
                {
                    PersonalizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    PersonalizacionComponenteID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    HTML = table.Column<string>(type: "NCLOB", nullable: false),
                    Nombre = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VistaVirtualGadgetRecursos", x => new { x.PersonalizacionID, x.PersonalizacionComponenteID });
                    table.ForeignKey(
                        name: "FK_VistaVirtualGadgetRecursos_VistaVirtualPersonalizacion_PersonalizacionID",
                        column: x => x.PersonalizacionID,
                        principalTable: "VistaVirtualPersonalizacion",
                        principalColumn: "PersonalizacionID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VistaVirtualProyecto",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    PersonalizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VistaVirtualProyecto", x => new { x.OrganizacionID, x.ProyectoID, x.PersonalizacionID });
                    table.ForeignKey(
                        name: "FK_VistaVirtualProyecto_VistaVirtualPersonalizacion_PersonalizacionID",
                        column: x => x.PersonalizacionID,
                        principalTable: "VistaVirtualPersonalizacion",
                        principalColumn: "PersonalizacionID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VistaVirtualRecursos",
                columns: table => new
                {
                    PersonalizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    RdfType = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    HTML = table.Column<string>(type: "NCLOB", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VistaVirtualRecursos", x => new { x.PersonalizacionID, x.RdfType });
                    table.ForeignKey(
                        name: "FK_VistaVirtualRecursos_VistaVirtualPersonalizacion_PersonalizacionID",
                        column: x => x.PersonalizacionID,
                        principalTable: "VistaVirtualPersonalizacion",
                        principalColumn: "PersonalizacionID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VotoComentario",
                columns: table => new
                {
                    VotoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ComentarioID = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VotoComentario", x => new { x.VotoID, x.ComentarioID });
                    table.ForeignKey(
                        name: "FK_VotoComentario_Comentario_ComentarioID",
                        column: x => x.ComentarioID,
                        principalTable: "Comentario",
                        principalColumn: "ComentarioID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VotoComentario_Voto_VotoID",
                        column: x => x.VotoID,
                        principalTable: "Voto",
                        principalColumn: "VotoID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VotoDocumento",
                columns: table => new
                {
                    DocumentoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    VotoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VotoDocumento", x => new { x.DocumentoID, x.VotoID });
                    table.ForeignKey(
                        name: "FK_VotoDocumento_Documento_DocumentoID",
                        column: x => x.DocumentoID,
                        principalTable: "Documento",
                        principalColumn: "DocumentoID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VotoDocumento_Voto_VotoID",
                        column: x => x.VotoID,
                        principalTable: "Voto",
                        principalColumn: "VotoID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VotoEntradaBlog",
                columns: table => new
                {
                    BlogID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    EntradaBlogID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    VotoID = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VotoEntradaBlog", x => new { x.BlogID, x.EntradaBlogID, x.VotoID });
                    table.ForeignKey(
                        name: "FK_VotoEntradaBlog_Voto_VotoID",
                        column: x => x.VotoID,
                        principalTable: "Voto",
                        principalColumn: "VotoID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VotoMensajeForo",
                columns: table => new
                {
                    VotoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ForoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    CategoriaForoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    TemaID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    MensajeForoID = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VotoMensajeForo", x => new { x.VotoID, x.ForoID, x.CategoriaForoID, x.TemaID, x.MensajeForoID });
                    table.ForeignKey(
                        name: "FK_VotoMensajeForo_Voto_VotoID",
                        column: x => x.VotoID,
                        principalTable: "Voto",
                        principalColumn: "VotoID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CMSBloqueComponente",
                columns: table => new
                {
                    BloqueID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ComponenteID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Orden = table.Column<short>(type: "NUMBER(5)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CMSBloqueComponente", x => new { x.BloqueID, x.ComponenteID });
                    table.ForeignKey(
                        name: "FK_CMSBloqueComponente_CMSBloque_BloqueID",
                        column: x => x.BloqueID,
                        principalTable: "CMSBloque",
                        principalColumn: "BloqueID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CMSBloqueComponente_CMSComponente_ComponenteID",
                        column: x => x.ComponenteID,
                        principalTable: "CMSComponente",
                        principalColumn: "ComponenteID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FacetaFiltroProyecto",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ObjetoConocimiento = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    Faceta = table.Column<string>(type: "NVARCHAR2(300)", maxLength: 300, nullable: false),
                    Orden = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    Filtro = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Condicion = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacetaFiltroProyecto", x => new { x.OrganizacionID, x.ProyectoID, x.ObjetoConocimiento, x.Faceta, x.Orden });
                    table.ForeignKey(
                        name: "FK_FacetaFiltroProyecto_FacetaObjetoConocimientoProyecto_OrganizacionID_ProyectoID_ObjetoConocimiento_Faceta",
                        columns: x => new { x.OrganizacionID, x.ProyectoID, x.ObjetoConocimiento, x.Faceta },
                        principalTable: "FacetaObjetoConocimientoProyecto",
                        principalColumns: new[] { "OrganizacionID", "ProyectoID", "ObjetoConocimiento", "Faceta" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DocumentoAtributoBiblio",
                columns: table => new
                {
                    DocumentoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    AtributoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    FichaBibliograficaID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Valor = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentoAtributoBiblio", x => new { x.DocumentoID, x.AtributoID, x.FichaBibliograficaID });
                    table.ForeignKey(
                        name: "FK_DocumentoAtributoBiblio_AtributoFichaBibliografica_AtributoID_FichaBibliograficaID",
                        columns: x => new { x.AtributoID, x.FichaBibliograficaID },
                        principalTable: "AtributoFichaBibliografica",
                        principalColumns: new[] { "AtributoID", "FichaBibliograficaID" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProyectoCookie",
                columns: table => new
                {
                    CookieID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Nombre = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    NombreCorto = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    Tipo = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    Descripcion = table.Column<string>(type: "NCLOB", nullable: true),
                    EsEditable = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    CategoriaID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProyectoCookie", x => x.CookieID);
                    table.ForeignKey(
                        name: "FK_ProyectoCookie_CategoriaProyectoCookie_CategoriaID",
                        column: x => x.CategoriaID,
                        principalTable: "CategoriaProyectoCookie",
                        principalColumn: "CategoriaID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProyectoCookie_Proyecto_ProyectoID_OrganizacionID",
                        columns: x => new { x.ProyectoID, x.OrganizacionID },
                        principalTable: "Proyecto",
                        principalColumns: new[] {"ProyectoID", "OrganizacionID" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DocumentoWebVinBaseRecursos",
                columns: table => new
                {
                    DocumentoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    BaseRecursosID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    IdentidadPublicacionID = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    FechaPublicacion = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    Compartido = table.Column<bool>(type: "NUMBER(1)", nullable: true),
                    Eliminado = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    NumeroComentarios = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    NumeroDescargas = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    NumeroConsultas = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    NumeroVotos = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    NombrePublicador = table.Column<string>(type: "NVARCHAR2(300)", maxLength: 300, nullable: true),
                    NombreCortoPublicador = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    TipoIdentidadPublicador = table.Column<short>(type: "NUMBER(5)", nullable: true),
                    PublicadorOrgID = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    NombreOrgPublicador = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true),
                    NombreCortoOrgPublicador = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    IdentPubVisibleExt = table.Column<bool>(type: "NUMBER(1)", nullable: true),
                    PermiteComentarios = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    NivelCertificacionID = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    Rank = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    Rank_Tiempo = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    IndexarRecurso = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    PrivadoEditores = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    TipoPublicacion = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    LinkAComunidadOrigen = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    FechaCertificacion = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    FechaUltimaVisita = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentoWebVinBaseRecursos", x => new { x.DocumentoID, x.BaseRecursosID });
                    table.ForeignKey(
                        name: "FK_DocumentoWebVinBaseRecursos_BaseRecursos_BaseRecursosID",
                        column: x => x.BaseRecursosID,
                        principalTable: "BaseRecursos",
                        principalColumn: "BaseRecursosID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DocumentoWebVinBaseRecursos_Documento_DocumentoID",
                        column: x => x.DocumentoID,
                        principalTable: "Documento",
                        principalColumn: "DocumentoID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DocumentoWebVinBaseRecursos_NivelCertificacion_NivelCertificacionID",
                        column: x => x.NivelCertificacionID,
                        principalTable: "NivelCertificacion",
                        principalColumn: "NivelCertificacionID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProyectoGadgetContextoHTMLplano",
                columns: table => new
                {
                    GadgetID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ComunidadDestinoFiltros = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProyectoGadgetContextoHTMLplano", x => new { x.GadgetID, x.OrganizacionID, x.ProyectoID });
                    table.ForeignKey(
                        name: "FK_ProyectoGadgetContextoHTMLplano_ProyectoGadget_OrganizacionID_ProyectoID_GadgetID",
                        columns: x => new { x.OrganizacionID, x.ProyectoID, x.GadgetID },
                        principalTable: "ProyectoGadget",
                        principalColumns: new[] { "OrganizacionID", "ProyectoID", "GadgetID" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProyectoGadgetIdioma",
                columns: table => new
                {
                    GadgetID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Idioma = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    Contenido = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProyectoGadgetIdioma", x => new { x.GadgetID, x.OrganizacionID, x.ProyectoID, x.Idioma });
                    table.ForeignKey(
                        name: "FK_ProyectoGadgetIdioma_ProyectoGadget_GadgetID_OrganizacionID_ProyectoID",
                        columns: x => new { x.GadgetID, x.OrganizacionID, x.ProyectoID },
                        principalTable: "ProyectoGadget",
                        principalColumns: new[] { "OrganizacionID", "ProyectoID", "GadgetID" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ConfigAutocompletarProy",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Clave = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Valor = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    PestanyaID = table.Column<Guid>(type: "RAW(16)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfigAutocompletarProy", x => new { x.OrganizacionID, x.ProyectoID, x.Clave });
                    table.ForeignKey(
                        name: "FK_ConfigAutocompletarProy_ProyectoPestanyaMenu_PestanyaID",
                        column: x => x.PestanyaID,
                        principalTable: "ProyectoPestanyaMenu",
                        principalColumn: "PestanyaID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FacetaObjetoConocimientoProyectoPestanya",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ObjetoConocimiento = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    Faceta = table.Column<string>(type: "NVARCHAR2(300)", maxLength: 300, nullable: false),
                    PestanyaID = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacetaObjetoConocimientoProyectoPestanya", x => new { x.OrganizacionID, x.ProyectoID, x.ObjetoConocimiento, x.Faceta, x.PestanyaID });
                    table.ForeignKey(
                        name: "FK_FacetaObjetoConocimientoProyectoPestanya_FacetaObjetoConocimientoProyecto_OrganizacionID_ProyectoID_ObjetoConocimiento_Faceta",
                        columns: x => new { x.OrganizacionID, x.ProyectoID, x.ObjetoConocimiento, x.Faceta },
                        principalTable: "FacetaObjetoConocimientoProyecto",
                        principalColumns: new[] { "OrganizacionID", "ProyectoID", "ObjetoConocimiento", "Faceta" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FacetaObjetoConocimientoProyectoPestanya_Proyecto_OrganizacionID_ProyectoID",
                        columns: x => new { x.OrganizacionID, x.ProyectoID },
                        principalTable: "Proyecto",
                        principalColumns: new[] { "OrganizacionID", "ProyectoID" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FacetaObjetoConocimientoProyectoPestanya_ProyectoPestanyaMenu_PestanyaID",
                        column: x => x.PestanyaID,
                        principalTable: "ProyectoPestanyaMenu",
                        principalColumn: "PestanyaID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PresentacionPestanyaListadoSemantico",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    PestanyaID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OntologiaID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Orden = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    Ontologia = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: false),
                    Propiedad = table.Column<string>(type: "NVARCHAR2(2000)", maxLength: 2000, nullable: false),
                    Nombre = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PresentacionPestanyaListadoSemantico", x => new { x.OrganizacionID, x.ProyectoID, x.PestanyaID, x.OntologiaID, x.Orden });
                    table.ForeignKey(
                        name: "FK_PresentacionPestanyaListadoSemantico_Proyecto_OrganizacionID_ProyectoID",
                        columns: x => new { x.OrganizacionID, x.ProyectoID },
                        principalTable: "Proyecto",
                        principalColumns: new[] { "OrganizacionID", "ProyectoID" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PresentacionPestanyaListadoSemantico_ProyectoPestanyaMenu_PestanyaID",
                        column: x => x.PestanyaID,
                        principalTable: "ProyectoPestanyaMenu",
                        principalColumn: "PestanyaID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PresentacionPestanyaMapaSemantico",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    PestanyaID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OntologiaID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Orden = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    Ontologia = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: false),
                    Propiedad = table.Column<string>(type: "NVARCHAR2(2000)", maxLength: 2000, nullable: false),
                    Nombre = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PresentacionPestanyaMapaSemantico", x => new { x.OrganizacionID, x.ProyectoID, x.PestanyaID, x.OntologiaID, x.Orden });
                    table.ForeignKey(
                        name: "FK_PresentacionPestanyaMapaSemantico_Proyecto_OrganizacionID_ProyectoID",
                        columns: x => new { x.OrganizacionID, x.ProyectoID },
                        principalTable: "Proyecto",
                        principalColumns: new[] { "OrganizacionID", "ProyectoID" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PresentacionPestanyaMapaSemantico_ProyectoPestanyaMenu_PestanyaID",
                        column: x => x.PestanyaID,
                        principalTable: "ProyectoPestanyaMenu",
                        principalColumn: "PestanyaID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PresentacionPestanyaMosaicoSemantico",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    PestanyaID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OntologiaID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Orden = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    Ontologia = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: false),
                    Propiedad = table.Column<string>(type: "NVARCHAR2(2000)", maxLength: 2000, nullable: false),
                    Nombre = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PresentacionPestanyaMosaicoSemantico", x => new { x.OrganizacionID, x.ProyectoID, x.PestanyaID, x.OntologiaID, x.Orden });
                    table.ForeignKey(
                        name: "FK_PresentacionPestanyaMosaicoSemantico_Proyecto_OrganizacionID_ProyectoID",
                        columns: x => new { x.OrganizacionID, x.ProyectoID },
                        principalTable: "Proyecto",
                        principalColumns: new[] { "OrganizacionID", "ProyectoID" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PresentacionPestanyaMosaicoSemantico_ProyectoPestanyaMenu_PestanyaID",
                        column: x => x.PestanyaID,
                        principalTable: "ProyectoPestanyaMenu",
                        principalColumn: "PestanyaID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProyectoPestanyaBusqueda",
                columns: table => new
                {
                    PestanyaID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    CampoFiltro = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    NumeroRecursos = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    VistaDisponible = table.Column<string>(type: "NVARCHAR2(10)", maxLength: 10, nullable: false),
                    MostrarFacetas = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    MostrarCajaBusqueda = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    ProyectoOrigenID = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    OcultarResultadosSinFiltros = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    PosicionCentralMapa = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    GruposConfiguracion = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    GruposPorTipo = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    TextoBusquedaSinResultados = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    TextoDefectoBuscador = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    MostrarEnComboBusqueda = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    IgnorarPrivacidadEnBusqueda = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    OmitirCargaInicialFacetasResultados = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    RelacionMandatory = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProyectoPestanyaBusqueda", x => x.PestanyaID);
                    table.ForeignKey(
                        name: "FK_ProyectoPestanyaBusqueda_ProyectoPestanyaMenu_PestanyaID",
                        column: x => x.PestanyaID,
                        principalTable: "ProyectoPestanyaMenu",
                        principalColumn: "PestanyaID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProyectoPestanyaCMS",
                columns: table => new
                {
                    PestanyaID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Ubicacion = table.Column<short>(type: "NUMBER(5)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProyectoPestanyaCMS", x => new { x.PestanyaID, x.Ubicacion });
                    table.ForeignKey(
                        name: "FK_ProyectoPestanyaCMS_ProyectoPestanyaMenu_PestanyaID",
                        column: x => x.PestanyaID,
                        principalTable: "ProyectoPestanyaMenu",
                        principalColumn: "PestanyaID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProyectoPestanyaFiltroOrdenRecursos",
                columns: table => new
                {
                    PestanyaID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Orden = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    FiltroOrden = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    NombreFiltro = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProyectoPestanyaFiltroOrdenRecursos", x => new { x.PestanyaID, x.Orden });
                    table.ForeignKey(
                        name: "FK_ProyectoPestanyaFiltroOrdenRecursos_ProyectoPestanyaMenu_PestanyaID",
                        column: x => x.PestanyaID,
                        principalTable: "ProyectoPestanyaMenu",
                        principalColumn: "PestanyaID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProyectoPestanyaMenuRolGrupoIdentidades",
                columns: table => new
                {
                    PestanyaID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    GrupoID = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProyectoPestanyaMenuRolGrupoIdentidades", x => new { x.PestanyaID, x.GrupoID });
                    table.ForeignKey(
                        name: "FK_ProyectoPestanyaMenuRolGrupoIdentidades_ProyectoPestanyaMenu_PestanyaID",
                        column: x => x.PestanyaID,
                        principalTable: "ProyectoPestanyaMenu",
                        principalColumn: "PestanyaID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProyectoPestanyaMenuRolIdentidad",
                columns: table => new
                {
                    PestanyaID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    PerfilID = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProyectoPestanyaMenuRolIdentidad", x => new { x.PestanyaID, x.PerfilID });
                    table.ForeignKey(
                        name: "FK_ProyectoPestanyaMenuRolIdentidad_ProyectoPestanyaMenu_PestanyaID",
                        column: x => x.PestanyaID,
                        principalTable: "ProyectoPestanyaMenu",
                        principalColumn: "PestanyaID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SolicitudNuevaOrgEmp",
                columns: table => new
                {
                    SolicitudID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    UsuarioAdminID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    CIF = table.Column<string>(type: "NVARCHAR2(9)", maxLength: 9, nullable: true),
                    Tipo = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    FechaFundacion = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    Empleados = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Sector = table.Column<short>(type: "NUMBER(5)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudNuevaOrgEmp", x => new { x.SolicitudID, x.UsuarioAdminID });
                    table.ForeignKey(
                        name: "FK_SolicitudNuevaOrgEmp_SolicitudNuevaOrganizacion_SolicitudID_UsuarioAdminID",
                        columns: x => new { x.SolicitudID, x.UsuarioAdminID },
                        principalTable: "SolicitudNuevaOrganizacion",
                        principalColumns: new[] { "SolicitudID", "UsuarioAdminID" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CategoriaTesauroSugerencia",
                columns: table => new
                {
                    SugerenciaID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    TesauroSugerenciaID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    TesauroCatPadreID = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    CategoriaTesauroPadreID = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    Nombre = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: false),
                    IdentidadID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Estado = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    CategoriaTesauroAceptadaID = table.Column<Guid>(type: "RAW(16)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriaTesauroSugerencia", x => x.SugerenciaID);
                    table.ForeignKey(
                        name: "FK_CategoriaTesauroSugerencia_CategoriaTesauro_TesauroCatPadreID_CategoriaTesauroPadreID",
                        columns: x => new { x.TesauroCatPadreID, x.CategoriaTesauroPadreID },
                        principalTable: "CategoriaTesauro",
                        principalColumns: new[] { "TesauroID", "CategoriaTesauroID" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CategoriaTesauroSugerencia_Tesauro_TesauroSugerenciaID",
                        column: x => x.TesauroSugerenciaID,
                        principalTable: "Tesauro",
                        principalColumn: "TesauroID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CatTesauroAgCatTesauro",
                columns: table => new
                {
                    TesauroID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    CategoriaSuperiorID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    CategoriaInferiorID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Orden = table.Column<short>(type: "NUMBER(5)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatTesauroAgCatTesauro", x => new { x.TesauroID, x.CategoriaSuperiorID, x.CategoriaInferiorID });
                    table.ForeignKey(
                        name: "FK_CatTesauroAgCatTesauro_CategoriaTesauro_TesauroID_CategoriaInferiorID",
                        columns: x => new { x.TesauroID, x.CategoriaInferiorID },
                        principalTable: "CategoriaTesauro",
                        principalColumns: new[] { "TesauroID", "CategoriaTesauroID" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CatTesauroAgCatTesauro_CategoriaTesauro_TesauroID_CategoriaSuperiorID",
                        columns: x => new { x.TesauroID, x.CategoriaSuperiorID },
                        principalTable: "CategoriaTesauro",
                        principalColumns: new[] { "TesauroID", "CategoriaTesauroID" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CatTesauroCompartida",
                columns: table => new
                {
                    TesauroOrigenID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    CategoriaOrigenID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    TesauroDestinoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    CategoriaSupDestinoID = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    Orden = table.Column<short>(type: "NUMBER(5)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatTesauroCompartida", x => new { x.TesauroOrigenID, x.CategoriaOrigenID, x.TesauroDestinoID });
                    table.ForeignKey(
                        name: "FK_CatTesauroCompartida_CategoriaTesauro_TesauroDestinoID_CategoriaSupDestinoID",
                        columns: x => new { x.TesauroDestinoID, x.CategoriaSupDestinoID },
                        principalTable: "CategoriaTesauro",
                        principalColumns: new[] { "TesauroID", "CategoriaTesauroID" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CatTesauroCompartida_CategoriaTesauro_TesauroOrigenID_CategoriaOrigenID",
                        columns: x => new { x.TesauroOrigenID, x.CategoriaOrigenID },
                        principalTable: "CategoriaTesauro",
                        principalColumns: new[] { "TesauroID", "CategoriaTesauroID" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DocumentoWebAgCatTesauro",
                columns: table => new
                {
                    TesauroID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    CategoriaTesauroID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    BaseRecursosID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    DocumentoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Fecha = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentoWebAgCatTesauro", x => new { x.TesauroID, x.CategoriaTesauroID, x.BaseRecursosID, x.DocumentoID });
                    table.ForeignKey(
                        name: "FK_DocumentoWebAgCatTesauro_CategoriaTesauro_TesauroID_CategoriaTesauroID",
                        columns: x => new { x.TesauroID, x.CategoriaTesauroID },
                        principalTable: "CategoriaTesauro",
                        principalColumns: new[] { "TesauroID", "CategoriaTesauroID" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ConfiguracionGnossPersona",
                columns: table => new
                {
                    PersonaID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    SolicitudesContacto = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    MensajesGnoss = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    ComentariosRecursos = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    InvitacionComunidad = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    InvitacionOrganizacion = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    BoletinSuscripcion = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    VerAmigos = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    VerAmigosExterno = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    VerRecursos = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    VerRecursosExterno = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    EnviarEnlaces = table.Column<bool>(type: "NUMBER(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracionGnossPersona", x => x.PersonaID);
                    table.ForeignKey(
                        name: "FK_ConfiguracionGnossPersona_Persona_PersonaID",
                        column: x => x.PersonaID,
                        principalTable: "Persona",
                        principalColumn: "PersonaID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DatosTrabajoPersonaLibre",
                columns: table => new
                {
                    PersonaID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    PaisTrabajoID = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    ProvinciaTrabajoID = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    ProvinciaTrabajo = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true),
                    TelefonoTrabajo = table.Column<string>(type: "NVARCHAR2(13)", maxLength: 13, nullable: true),
                    TelefonoMovilTrabajo = table.Column<string>(type: "NVARCHAR2(13)", maxLength: 13, nullable: true),
                    EmailTrabajo = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true),
                    LocalidadTrabajo = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true),
                    DireccionTrabajo = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: true),
                    CPTrabajo = table.Column<string>(type: "NVARCHAR2(15)", maxLength: 15, nullable: true),
                    Profesion = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatosTrabajoPersonaLibre", x => x.PersonaID);
                    table.ForeignKey(
                        name: "FK_DatosTrabajoPersonaLibre_Persona_PersonaID",
                        column: x => x.PersonaID,
                        principalTable: "Persona",
                        principalColumn: "PersonaID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Perfil",
                columns: table => new
                {
                    PerfilID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    NombrePerfil = table.Column<string>(type: "NVARCHAR2(300)", maxLength: 300, nullable: false),
                    NombreOrganizacion = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true),
                    Eliminado = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    NombreCortoOrg = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    NombreCortoUsu = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    PersonaID = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    TieneTwitter = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    UsuarioTwitter = table.Column<string>(type: "NVARCHAR2(15)", maxLength: 15, nullable: true),
                    TokenTwitter = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: true),
                    TokenSecretoTwitter = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: true),
                    CurriculumPerfilID = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    CaducidadResSusc = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    CurriculumID = table.Column<Guid>(type: "RAW(16)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Perfil", x => x.PerfilID);
                    table.ForeignKey(
                        name: "FK_Perfil_Persona_PersonaID",
                        column: x => x.PersonaID,
                        principalTable: "Persona",
                        principalColumn: "PersonaID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProyRolUsuClausulaReg",
                columns: table => new
                {
                    ClausulaID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OrganizacionGnossID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    UsuarioID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Valor = table.Column<bool>(type: "NUMBER(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProyRolUsuClausulaReg", x => new { x.ClausulaID, x.OrganizacionID, x.OrganizacionGnossID, x.ProyectoID, x.UsuarioID });
                    table.ForeignKey(
                        name: "FK_ProyRolUsuClausulaReg_ClausulaRegistro_ClausulaID_OrganizacionID_ProyectoID",
                        columns: x => new { x.ClausulaID, x.OrganizacionID, x.ProyectoID },
                        principalTable: "ClausulaRegistro",
                        principalColumns: new[] { "ClausulaID", "OrganizacionID", "ProyectoID" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProyRolUsuClausulaReg_ProyectoRolUsuario_OrganizacionGnossID_ProyectoID_UsuarioID",
                        columns: x => new { x.OrganizacionGnossID, x.ProyectoID, x.UsuarioID },
                        principalTable: "ProyectoRolUsuario",
                        principalColumns: new[] { "OrganizacionGnossID", "ProyectoID", "UsuarioID" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CMSBloqueComponentePropiedadComponente",
                columns: table => new
                {
                    BloqueID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ComponenteID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    TipoPropiedadComponente = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ValorPropiedad = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CMSBloqueComponentePropiedadComponente", x => new { x.BloqueID, x.ComponenteID, x.TipoPropiedadComponente });
                    table.ForeignKey(
                        name: "FK_CMSBloqueComponentePropiedadComponente_CMSBloqueComponente_BloqueID_ComponenteID",
                        columns: x => new { x.BloqueID, x.ComponenteID },
                        principalTable: "CMSBloqueComponente",
                        principalColumns: new[] { "BloqueID", "ComponenteID" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DocumentoWebVinBaseRecursosExtra",
                columns: table => new
                {
                    DocumentoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    BaseRecursosID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    NumeroDescargas = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    NumeroConsultas = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    FechaUltimaVisita = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentoWebVinBaseRecursosExtra", x => new { x.DocumentoID, x.BaseRecursosID });
                    table.ForeignKey(
                        name: "FK_DocumentoWebVinBaseRecursosExtra_DocumentoWebVinBaseRecursos_DocumentoID_BaseRecursosID",
                        columns: x => new { x.DocumentoID, x.BaseRecursosID },
                        principalTable: "DocumentoWebVinBaseRecursos",
                        principalColumns: new[] { "DocumentoID", "BaseRecursosID" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProyectoPestanyaBusquedaExportacion",
                columns: table => new
                {
                    ExportacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    PestanyaID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    NombreExportacion = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Orden = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    GruposExportadores = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    FormatosExportacion = table.Column<string>(type: "NVARCHAR2(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProyectoPestanyaBusquedaExportacion", x => x.ExportacionID);
                    table.ForeignKey(
                        name: "FK_ProyectoPestanyaBusquedaExportacion_ProyectoPestanyaBusqueda_PestanyaID",
                        column: x => x.PestanyaID,
                        principalTable: "ProyectoPestanyaBusqueda",
                        principalColumn: "PestanyaID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DatoExtraEcosistemaOpcionPerfil",
                columns: table => new
                {
                    DatoExtraID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OpcionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    PerfilID = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatoExtraEcosistemaOpcionPerfil", x => new { x.DatoExtraID, x.OpcionID, x.PerfilID });
                    table.ForeignKey(
                        name: "FK_DatoExtraEcosistemaOpcionPerfil_Perfil_PerfilID",
                        column: x => x.PerfilID,
                        principalTable: "Perfil",
                        principalColumn: "PerfilID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Identidad",
                columns: table => new
                {
                    IdentidadID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    PerfilID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    CurriculumID = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    FechaAlta = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    NumConnexiones = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Tipo = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    NombreCortoIdentidad = table.Column<string>(type: "NVARCHAR2(300)", maxLength: 300, nullable: false),
                    FechaExpulsion = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    RecibirNewsLetter = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    Rank = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    MostrarBienvenida = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    DiasUltActualizacion = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ValorAbsoluto = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    ActivoEnComunidad = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    ActualizaHome = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    Foto = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Identidad", x => x.IdentidadID);
                    table.ForeignKey(
                        name: "FK_Identidad_Perfil_PerfilID",
                        column: x => x.PerfilID,
                        principalTable: "Perfil",
                        principalColumn: "PerfilID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PerfilPersona",
                columns: table => new
                {
                    PerfilID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    PersonaID = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerfilPersona", x => x.PerfilID);
                    table.ForeignKey(
                        name: "FK_PerfilPersona_Perfil_PerfilID",
                        column: x => x.PerfilID,
                        principalTable: "Perfil",
                        principalColumn: "PerfilID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PerfilPersonaOrg",
                columns: table => new
                {
                    PersonaID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    PerfilID = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerfilPersonaOrg", x => new { x.PersonaID, x.OrganizacionID, x.PerfilID });
                    table.ForeignKey(
                        name: "FK_PerfilPersonaOrg_Perfil_PerfilID",
                        column: x => x.PerfilID,
                        principalTable: "Perfil",
                        principalColumn: "PerfilID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Profesor",
                columns: table => new
                {
                    ProfesorID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    PerfilID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Email = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    CentroEstudios = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    AreaEstudios = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profesor", x => new { x.ProfesorID, x.PerfilID });
                    table.ForeignKey(
                        name: "FK_Profesor_Perfil_PerfilID",
                        column: x => x.PerfilID,
                        principalTable: "Perfil",
                        principalColumn: "PerfilID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProyectoPestanyaBusquedaExportacionExterna",
                columns: table => new
                {
                    ExportacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    PestanyaID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    UrlServicioExterno = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProyectoPestanyaBusquedaExportacionExterna", x => x.ExportacionID);
                    table.ForeignKey(
                        name: "FK_ProyectoPestanyaBusquedaExportacionExterna_ProyectoPestanyaBusquedaExportacion_ExportacionID",
                        column: x => x.ExportacionID,
                        principalTable: "ProyectoPestanyaBusquedaExportacion",
                        principalColumn: "ExportacionID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProyectoPestanyaBusquedaExportacionPropiedad",
                columns: table => new
                {
                    ExportacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Orden = table.Column<short>(type: "NUMBER(5)", nullable: false),
                    OntologiaID = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    Ontologia = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    Propiedad = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    NombrePropiedad = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    DatosExtraPropiedad = table.Column<string>(type: "VARCHAR2(4000)", unicode: false, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProyectoPestanyaBusquedaExportacionPropiedad", x => new { x.ExportacionID, x.Orden });
                    table.ForeignKey(
                        name: "FK_ProyectoPestanyaBusquedaExportacionPropiedad_ProyectoPestanyaBusquedaExportacion_ExportacionID",
                        column: x => x.ExportacionID,
                        principalTable: "ProyectoPestanyaBusquedaExportacion",
                        principalColumn: "ExportacionID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DatoExtraProyectoOpcionIdentidad",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    DatoExtraID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OpcionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    IdentidadID = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatoExtraProyectoOpcionIdentidad", x => new { x.OrganizacionID, x.ProyectoID, x.DatoExtraID, x.OpcionID, x.IdentidadID });
                    table.ForeignKey(
                        name: "FK_DatoExtraProyectoOpcionIdentidad_Identidad_IdentidadID",
                        column: x => x.IdentidadID,
                        principalTable: "Identidad",
                        principalColumn: "IdentidadID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DatoExtraProyectoVirtuosoIdentidad",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    DatoExtraID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    IdentidadID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Opcion = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatoExtraProyectoVirtuosoIdentidad", x => new { x.OrganizacionID, x.ProyectoID, x.DatoExtraID, x.IdentidadID });
                    table.ForeignKey(
                        name: "FK_DatoExtraProyectoVirtuosoIdentidad_DatoExtraProyectoVirtuoso_OrganizacionID_ProyectoID_DatoExtraID",
                        columns: x => new { x.OrganizacionID, x.ProyectoID, x.DatoExtraID },
                        principalTable: "DatoExtraProyectoVirtuoso",
                        principalColumns: new[] { "OrganizacionID", "ProyectoID", "DatoExtraID" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DatoExtraProyectoVirtuosoIdentidad_Identidad_IdentidadID",
                        column: x => x.IdentidadID,
                        principalTable: "Identidad",
                        principalColumn: "IdentidadID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GrupoIdentidadesParticipacion",
                columns: table => new
                {
                    GrupoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    IdentidadID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    FechaAlta = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrupoIdentidadesParticipacion", x => new { x.GrupoID, x.IdentidadID });
                    table.ForeignKey(
                        name: "FK_GrupoIdentidadesParticipacion_GrupoIdentidades_GrupoID",
                        column: x => x.GrupoID,
                        principalTable: "GrupoIdentidades",
                        principalColumn: "GrupoID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GrupoIdentidadesParticipacion_Identidad_IdentidadID",
                        column: x => x.IdentidadID,
                        principalTable: "Identidad",
                        principalColumn: "IdentidadID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IdentidadContadores",
                columns: table => new
                {
                    IdentidadID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    NumeroVisitas = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    NumeroDescargas = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentidadContadores", x => x.IdentidadID);
                    table.ForeignKey(
                        name: "FK_IdentidadContadores_Identidad_IdentidadID",
                        column: x => x.IdentidadID,
                        principalTable: "Identidad",
                        principalColumn: "IdentidadID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizacionParticipaProy",
                columns: table => new
                {
                    OrganizacionID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OrganizacionProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    IdentidadID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    EstaBloqueada = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    RegistroAutomatico = table.Column<short>(type: "NUMBER(5)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizacionParticipaProy", x => new { x.OrganizacionID, x.OrganizacionProyectoID, x.ProyectoID });
                    table.ForeignKey(
                        name: "FK_OrganizacionParticipaProy_Identidad_IdentidadID",
                        column: x => x.IdentidadID,
                        principalTable: "Identidad",
                        principalColumn: "IdentidadID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrganizacionParticipaProy_Organizacion_OrganizacionID",
                        column: x => x.OrganizacionID,
                        principalTable: "Organizacion",
                        principalColumn: "OrganizacionID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProyectoUsuarioIdentidad",
                columns: table => new
                {
                    IdentidadID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    UsuarioID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    OrganizacionGnossID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProyectoID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    FechaEntrada = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    Reputacion = table.Column<int>(type: "NUMBER(10)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProyectoUsuarioIdentidad", x => new { x.IdentidadID, x.UsuarioID, x.OrganizacionGnossID, x.ProyectoID });
                    table.ForeignKey(
                        name: "FK_ProyectoUsuarioIdentidad_Identidad_IdentidadID",
                        column: x => x.IdentidadID,
                        principalTable: "Identidad",
                        principalColumn: "IdentidadID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProyectoUsuarioIdentidad_Proyecto_OrganizacionGnossID_ProyectoID",
                        columns: x => new { x.OrganizacionGnossID, x.ProyectoID },
                        principalTable: "Proyecto",
                        principalColumns: new[] { "OrganizacionID", "ProyectoID" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProyectoUsuarioIdentidad_Usuario_UsuarioID",
                        column: x => x.UsuarioID,
                        principalTable: "Usuario",
                        principalColumn: "UsuarioID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdministradorOrganizacion_OrganizacionID",
                table: "AdministradorOrganizacion",
                column: "OrganizacionID");

            migrationBuilder.CreateIndex(
                name: "IX_AmigoAgGrupo_GrupoAmigosGrupoID",
                table: "AmigoAgGrupo",
                column: "GrupoAmigosGrupoID");

            migrationBuilder.CreateIndex(
                name: "IX_AtributoFichaBibliografica_FichaBibliograficaID",
                table: "AtributoFichaBibliografica",
                column: "FichaBibliograficaID");

            migrationBuilder.CreateIndex(
                name: "IX_BaseRecursosOrganizacion_OrganizacionID",
                table: "BaseRecursosOrganizacion",
                column: "OrganizacionID");

            migrationBuilder.CreateIndex(
                name: "IX_BaseRecursosUsuario_UsuarioID",
                table: "BaseRecursosUsuario",
                column: "UsuarioID");

            migrationBuilder.CreateIndex(
                name: "IX_BlogAgCatTesauro_BlogID",
                table: "BlogAgCatTesauro",
                column: "BlogID");

            migrationBuilder.CreateIndex(
                name: "IX_CargaPaquete_CargaID",
                table: "CargaPaquete",
                column: "CargaID");

            migrationBuilder.CreateIndex(
                name: "IX_CategoriaProyectoCookie_ProyectoID_OrganizacionID",
                table: "CategoriaProyectoCookie",
                columns: new[] { "ProyectoID", "OrganizacionID" });

            migrationBuilder.CreateIndex(
                name: "IX_CategoriaTesauroSugerencia_TesauroCatPadreID_CategoriaTesauroPadreID",
                table: "CategoriaTesauroSugerencia",
                columns: new[] { "TesauroCatPadreID", "CategoriaTesauroPadreID" });

            migrationBuilder.CreateIndex(
                name: "IX_CategoriaTesauroSugerencia_TesauroSugerenciaID",
                table: "CategoriaTesauroSugerencia",
                column: "TesauroSugerenciaID");

            migrationBuilder.CreateIndex(
                name: "IX_CatTesauroAgCatTesauro_TesauroID_CategoriaInferiorID",
                table: "CatTesauroAgCatTesauro",
                columns: new[] { "TesauroID", "CategoriaInferiorID" });

            migrationBuilder.CreateIndex(
                name: "IX_CatTesauroCompartida_TesauroDestinoID_CategoriaSupDestinoID",
                table: "CatTesauroCompartida",
                columns: new[] { "TesauroDestinoID", "CategoriaSupDestinoID" });

            migrationBuilder.CreateIndex(
                name: "IX_CMSBloque_OrganizacionID_ProyectoID_Ubicacion",
                table: "CMSBloque",
                columns: new[] { "OrganizacionID", "ProyectoID", "Ubicacion" });

            migrationBuilder.CreateIndex(
                name: "IX_CMSBloqueComponente_ComponenteID",
                table: "CMSBloqueComponente",
                column: "ComponenteID");

            migrationBuilder.CreateIndex(
                name: "IX_Comentario_ComentarioSuperiorID",
                table: "Comentario",
                column: "ComentarioSuperiorID");

            migrationBuilder.CreateIndex(
                name: "IX_ConfigAutocompletarProy_PestanyaID",
                table: "ConfigAutocompletarProy",
                column: "PestanyaID");

            migrationBuilder.CreateIndex(
                name: "IX_DatoExtraEcosistemaOpcionPerfil_PerfilID",
                table: "DatoExtraEcosistemaOpcionPerfil",
                column: "PerfilID");

            migrationBuilder.CreateIndex(
                name: "IX_DatoExtraEcosistemaOpcionSolicitud_SolicitudID",
                table: "DatoExtraEcosistemaOpcionSolicitud",
                column: "SolicitudID");

            migrationBuilder.CreateIndex(
                name: "IX_DatoExtraEcosistemaVirtuosoSolicitud_SolicitudID",
                table: "DatoExtraEcosistemaVirtuosoSolicitud",
                column: "SolicitudID");

            migrationBuilder.CreateIndex(
                name: "IX_DatoExtraProyectoOpcionIdentidad_IdentidadID",
                table: "DatoExtraProyectoOpcionIdentidad",
                column: "IdentidadID");

            migrationBuilder.CreateIndex(
                name: "IX_DatoExtraProyectoOpcionSolicitud_SolicitudID",
                table: "DatoExtraProyectoOpcionSolicitud",
                column: "SolicitudID");

            migrationBuilder.CreateIndex(
                name: "IX_DatoExtraProyectoVirtuosoIdentidad_IdentidadID",
                table: "DatoExtraProyectoVirtuosoIdentidad",
                column: "IdentidadID");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentoAtributoBiblio_AtributoID_FichaBibliograficaID",
                table: "DocumentoAtributoBiblio",
                columns: new[] { "AtributoID", "FichaBibliograficaID" });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentoRespuestaVoto_RespuestaID",
                table: "DocumentoRespuestaVoto",
                column: "RespuestaID");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentoTipologia_TipologiaID_AtributoID",
                table: "DocumentoTipologia",
                columns: new[] { "TipologiaID", "AtributoID" });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentoVincDoc_DocumentoVincID",
                table: "DocumentoVincDoc",
                column: "DocumentoVincID");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentoWebVinBaseRecursos_BaseRecursosID",
                table: "DocumentoWebVinBaseRecursos",
                column: "BaseRecursosID");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentoWebVinBaseRecursos_NivelCertificacionID",
                table: "DocumentoWebVinBaseRecursos",
                column: "NivelCertificacionID");

            migrationBuilder.CreateIndex(
                name: "IX_FacetaObjetoConocimientoProyectoPestanya_PestanyaID",
                table: "FacetaObjetoConocimientoProyectoPestanya",
                column: "PestanyaID");

            migrationBuilder.CreateIndex(
                name: "IX_GrupoIdentidadesParticipacion_IdentidadID",
                table: "GrupoIdentidadesParticipacion",
                column: "IdentidadID");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialDocumento_DocumentoID",
                table: "HistorialDocumento",
                column: "DocumentoID");

            migrationBuilder.CreateIndex(
                name: "IX_Identidad_PerfilID",
                table: "Identidad",
                column: "PerfilID");

            migrationBuilder.CreateIndex(
                name: "IX_Invitacion_NotificacionID",
                table: "Invitacion",
                column: "NotificacionID");

            migrationBuilder.CreateIndex(
                name: "IX_NivelCertificacion_OrganizacionID_ProyectoID",
                table: "NivelCertificacion",
                columns: new[] { "OrganizacionID", "ProyectoID" });

            migrationBuilder.CreateIndex(
                name: "IX_NotificacionSolicitud_SolicitudID",
                table: "NotificacionSolicitud",
                column: "SolicitudID");

            migrationBuilder.CreateIndex(
                name: "IX_Organizacion_OrganizacionPadreID",
                table: "Organizacion",
                column: "OrganizacionPadreID");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizacionParticipaProy_IdentidadID",
                table: "OrganizacionParticipaProy",
                column: "IdentidadID");

            migrationBuilder.CreateIndex(
                name: "IX_Perfil_PersonaID",
                table: "Perfil",
                column: "PersonaID");

            migrationBuilder.CreateIndex(
                name: "IX_PerfilPersonaOrg_PerfilID",
                table: "PerfilPersonaOrg",
                column: "PerfilID");

            migrationBuilder.CreateIndex(
                name: "IX_Persona_UsuarioID",
                table: "Persona",
                column: "UsuarioID");

            migrationBuilder.CreateIndex(
                name: "IX_PersonaVinculoOrganizacion_OrganizacionID",
                table: "PersonaVinculoOrganizacion",
                column: "OrganizacionID");

            migrationBuilder.CreateIndex(
                name: "IX_PresentacionPestanyaListadoSemantico_PestanyaID",
                table: "PresentacionPestanyaListadoSemantico",
                column: "PestanyaID");

            migrationBuilder.CreateIndex(
                name: "IX_PresentacionPestanyaMapaSemantico_PestanyaID",
                table: "PresentacionPestanyaMapaSemantico",
                column: "PestanyaID");

            migrationBuilder.CreateIndex(
                name: "IX_PresentacionPestanyaMosaicoSemantico_PestanyaID",
                table: "PresentacionPestanyaMosaicoSemantico",
                column: "PestanyaID");

            migrationBuilder.CreateIndex(
                name: "IX_Profesor_PerfilID",
                table: "Profesor",
                column: "PerfilID");

            migrationBuilder.CreateIndex(
                name: "IX_Provincia_PaisID",
                table: "Provincia",
                column: "PaisID");

            migrationBuilder.CreateIndex(
                name: "IX_Proyecto_OrganizacionID_ProyectoSuperiorID",
                table: "Proyecto",
                columns: new[] { "OrganizacionID", "ProyectoSuperiorID" });

            migrationBuilder.CreateIndex(
                name: "IX_ProyectoCookie_CategoriaID",
                table: "ProyectoCookie",
                column: "CategoriaID");

            migrationBuilder.CreateIndex(
                name: "IX_ProyectoCookie_ProyectoID_OrganizacionID",
                table: "ProyectoCookie",
                columns: new[] { "ProyectoID", "OrganizacionID" });

            migrationBuilder.CreateIndex(
                name: "IX_ProyectoGadgetContextoHTMLplano_OrganizacionID_ProyectoID_GadgetID",
                table: "ProyectoGadgetContextoHTMLplano",
                columns: new[] { "OrganizacionID", "ProyectoID", "GadgetID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProyectoPasoRegistro_OrganizacionID_ProyectoID",
                table: "ProyectoPasoRegistro",
                columns: new[] { "OrganizacionID", "ProyectoID" });

            migrationBuilder.CreateIndex(
                name: "IX_ProyectoPestanyaBusquedaExportacion_PestanyaID",
                table: "ProyectoPestanyaBusquedaExportacion",
                column: "PestanyaID");

            migrationBuilder.CreateIndex(
                name: "IX_ProyectoPestanyaMenu_OrganizacionID_ProyectoID",
                table: "ProyectoPestanyaMenu",
                columns: new[] { "OrganizacionID", "ProyectoID" });

            migrationBuilder.CreateIndex(
                name: "IX_ProyectoPestanyaMenu_PestanyaPadreID",
                table: "ProyectoPestanyaMenu",
                column: "PestanyaPadreID");

            migrationBuilder.CreateIndex(
                name: "IX_ProyectoRelacionado_OrganizacionRelacionadaID_ProyectoRelacionadoID",
                table: "ProyectoRelacionado",
                columns: new[] { "OrganizacionRelacionadaID", "ProyectoRelacionadoID" });

            migrationBuilder.CreateIndex(
                name: "IX_ProyectoRolUsuario_UsuarioID",
                table: "ProyectoRolUsuario",
                column: "UsuarioID");

            migrationBuilder.CreateIndex(
                name: "IX_ProyectoSinRegistroObligatorio_OrganizacionSinRegistroID_ProyectoSinRegistroID",
                table: "ProyectoSinRegistroObligatorio",
                columns: new[] { "OrganizacionSinRegistroID", "ProyectoSinRegistroID" });

            migrationBuilder.CreateIndex(
                name: "IX_ProyectoUsuarioIdentidad_OrganizacionGnossID_ProyectoID",
                table: "ProyectoUsuarioIdentidad",
                columns: new[] { "OrganizacionGnossID", "ProyectoID" });

            migrationBuilder.CreateIndex(
                name: "IX_ProyectoUsuarioIdentidad_UsuarioID",
                table: "ProyectoUsuarioIdentidad",
                column: "UsuarioID");

            migrationBuilder.CreateIndex(
                name: "IX_ProyRolUsuClausulaReg_ClausulaID_OrganizacionID_ProyectoID",
                table: "ProyRolUsuClausulaReg",
                columns: new[] { "ClausulaID", "OrganizacionID", "ProyectoID" });

            migrationBuilder.CreateIndex(
                name: "IX_ProyRolUsuClausulaReg_OrganizacionGnossID_ProyectoID_UsuarioID",
                table: "ProyRolUsuClausulaReg",
                columns: new[] { "OrganizacionGnossID", "ProyectoID", "UsuarioID" });

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudNuevoUsuario_SolicitudID",
                table: "SolicitudNuevoUsuario",
                column: "SolicitudID");

            migrationBuilder.CreateIndex(
                name: "IX_SuscripcionBlog_SuscripcionID",
                table: "SuscripcionBlog",
                column: "SuscripcionID");

            migrationBuilder.CreateIndex(
                name: "IX_SuscripcionIdentidadProyecto_SuscripcionID",
                table: "SuscripcionIdentidadProyecto",
                column: "SuscripcionID");

            migrationBuilder.CreateIndex(
                name: "IX_TesauroUsuario_UsuarioID",
                table: "TesauroUsuario",
                column: "UsuarioID");

            migrationBuilder.CreateIndex(
                name: "IX_VistaVirtualProyecto_PersonalizacionID",
                table: "VistaVirtualProyecto",
                column: "PersonalizacionID");

            migrationBuilder.CreateIndex(
                name: "IX_VotoComentario_ComentarioID",
                table: "VotoComentario",
                column: "ComentarioID");

            migrationBuilder.CreateIndex(
                name: "IX_VotoDocumento_VotoID",
                table: "VotoDocumento",
                column: "VotoID");

            migrationBuilder.CreateIndex(
                name: "IX_VotoEntradaBlog_VotoID",
                table: "VotoEntradaBlog",
                column: "VotoID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccionesExternas");

            migrationBuilder.DropTable(
                name: "AccionesExternasProyecto");

            migrationBuilder.DropTable(
                name: "AdministradorGeneral");

            migrationBuilder.DropTable(
                name: "AdministradorGrupoProyecto");

            migrationBuilder.DropTable(
                name: "AdministradorOrganizacion");

            migrationBuilder.DropTable(
                name: "AdministradorProyecto");

            migrationBuilder.DropTable(
                name: "Amigo");

            migrationBuilder.DropTable(
                name: "AmigoAgGrupo");

            migrationBuilder.DropTable(
                name: "BaseRecursosOrganizacion");

            migrationBuilder.DropTable(
                name: "BaseRecursosProyecto");

            migrationBuilder.DropTable(
                name: "BaseRecursosUsuario");

            migrationBuilder.DropTable(
                name: "BlogAgCatTesauro");

            migrationBuilder.DropTable(
                name: "BlogComunidad");

            migrationBuilder.DropTable(
                name: "CamposRegistroProyectoGenericos");

            migrationBuilder.DropTable(
                name: "CargaPaquete");

            migrationBuilder.DropTable(
                name: "CategoriaTesauroPropiedades");

            migrationBuilder.DropTable(
                name: "CategoriaTesauroSugerencia");

            migrationBuilder.DropTable(
                name: "CategoriaTesVinSuscrip");

            migrationBuilder.DropTable(
                name: "CatTesauroAgCatTesauro");

            migrationBuilder.DropTable(
                name: "CatTesauroCompartida");

            migrationBuilder.DropTable(
                name: "CatTesauroPermiteTipoRec");

            migrationBuilder.DropTable(
                name: "CMSBloqueComponentePropiedadComponente");

            migrationBuilder.DropTable(
                name: "CMSComponentePrivadoProyecto");

            migrationBuilder.DropTable(
                name: "CMSComponenteRolGrupoIdentidades");

            migrationBuilder.DropTable(
                name: "CMSComponenteRolIdentidad");

            migrationBuilder.DropTable(
                name: "CMSPropiedadComponente");

            migrationBuilder.DropTable(
                name: "CMSRolGrupoIdentidades");

            migrationBuilder.DropTable(
                name: "CMSRolIdentidad");

            migrationBuilder.DropTable(
                name: "ColaCargaRecursos");

            migrationBuilder.DropTable(
                name: "ColaDocumento");

            migrationBuilder.DropTable(
                name: "ColaTwitter");

            migrationBuilder.DropTable(
                name: "ComentarioBlog");

            migrationBuilder.DropTable(
                name: "ComentarioCuestion");

            migrationBuilder.DropTable(
                name: "ComparticionAutomaticaMapping");

            migrationBuilder.DropTable(
                name: "ComparticionAutomaticaReglas");

            migrationBuilder.DropTable(
                name: "ConfigApplicationInsightsDominio");

            migrationBuilder.DropTable(
                name: "ConfigAutocompletarProy");

            migrationBuilder.DropTable(
                name: "ConfigSearchProy");

            migrationBuilder.DropTable(
                name: "ConfiguracionAmbitoBusqueda");

            migrationBuilder.DropTable(
                name: "ConfiguracionAmbitoBusquedaProyecto");

            migrationBuilder.DropTable(
                name: "ConfiguracionBBDD");

            migrationBuilder.DropTable(
                name: "ConfiguracionConexionGrafo");

            migrationBuilder.DropTable(
                name: "ConfiguracionEnvioCorreo");

            migrationBuilder.DropTable(
                name: "ConfiguracionGnossOrg");

            migrationBuilder.DropTable(
                name: "ConfiguracionGnossPersona");

            migrationBuilder.DropTable(
                name: "ConfiguracionServicios");

            migrationBuilder.DropTable(
                name: "ConfiguracionServiciosDominio");

            migrationBuilder.DropTable(
                name: "ConfiguracionServiciosProyecto");

            migrationBuilder.DropTable(
                name: "ContadorPerfil");

            migrationBuilder.DropTable(
                name: "CorreoInterno");

            migrationBuilder.DropTable(
                name: "Curriculum");

            migrationBuilder.DropTable(
                name: "DatoExtraEcosistemaOpcion");

            migrationBuilder.DropTable(
                name: "DatoExtraEcosistemaOpcionPerfil");

            migrationBuilder.DropTable(
                name: "DatoExtraEcosistemaOpcionSolicitud");

            migrationBuilder.DropTable(
                name: "DatoExtraEcosistemaVirtuoso");

            migrationBuilder.DropTable(
                name: "DatoExtraEcosistemaVirtuosoPerfil");

            migrationBuilder.DropTable(
                name: "DatoExtraEcosistemaVirtuosoSolicitud");

            migrationBuilder.DropTable(
                name: "DatoExtraProyectoOpcion");

            migrationBuilder.DropTable(
                name: "DatoExtraProyectoOpcionIdentidad");

            migrationBuilder.DropTable(
                name: "DatoExtraProyectoOpcionSolicitud");

            migrationBuilder.DropTable(
                name: "DatoExtraProyectoVirtuosoIdentidad");

            migrationBuilder.DropTable(
                name: "DatoExtraProyectoVirtuosoSolicitud");

            migrationBuilder.DropTable(
                name: "DatosTrabajoPersonaLibre");

            migrationBuilder.DropTable(
                name: "DocumentoAtributoBiblio");

            migrationBuilder.DropTable(
                name: "DocumentoComentario");

            migrationBuilder.DropTable(
                name: "DocumentoEnEdicion");

            migrationBuilder.DropTable(
                name: "DocumentoEntidadGnoss");

            migrationBuilder.DropTable(
                name: "DocumentoEnvioNewsLetter");

            migrationBuilder.DropTable(
                name: "DocumentoGrupoUsuario");

            migrationBuilder.DropTable(
                name: "DocumentoLecturaAumentada");

            migrationBuilder.DropTable(
                name: "DocumentoNewsletter");

            migrationBuilder.DropTable(
                name: "DocumentoRespuestaVoto");

            migrationBuilder.DropTable(
                name: "DocumentoRolGrupoIdentidades");

            migrationBuilder.DropTable(
                name: "DocumentoRolIdentidad");

            migrationBuilder.DropTable(
                name: "DocumentoTipologia");

            migrationBuilder.DropTable(
                name: "DocumentoTokenBrightcove");

            migrationBuilder.DropTable(
                name: "DocumentoTokenTOP");

            migrationBuilder.DropTable(
                name: "DocumentoUrlCanonica");

            migrationBuilder.DropTable(
                name: "DocumentoVincDoc");

            migrationBuilder.DropTable(
                name: "DocumentoWebAgCatTesauro");

            migrationBuilder.DropTable(
                name: "DocumentoWebVinBaseRecursosExtra");

            migrationBuilder.DropTable(
                name: "EcosistemaServicioExterno");

            migrationBuilder.DropTable(
                name: "EmailIncorrecto");

            migrationBuilder.DropTable(
                name: "EntradaBlog");

            migrationBuilder.DropTable(
                name: "FacetaConfigProyChart");

            migrationBuilder.DropTable(
                name: "FacetaConfigProyMapa");

            migrationBuilder.DropTable(
                name: "FacetaConfigProyRangoFecha");

            migrationBuilder.DropTable(
                name: "FacetaEntidadesExternas");

            migrationBuilder.DropTable(
                name: "FacetaExcluida");

            migrationBuilder.DropTable(
                name: "FacetaFiltroHome");

            migrationBuilder.DropTable(
                name: "FacetaFiltroProyecto");

            migrationBuilder.DropTable(
                name: "FacetaMultiple");

            migrationBuilder.DropTable(
                name: "FacetaObjetoConocimiento");

            migrationBuilder.DropTable(
                name: "FacetaObjetoConocimientoProyectoPestanya");

            migrationBuilder.DropTable(
                name: "FacetaRedireccion");

            migrationBuilder.DropTable(
                name: "GeneralRolGrupoUsuario");

            migrationBuilder.DropTable(
                name: "GeneralRolUsuario");

            migrationBuilder.DropTable(
                name: "GrupoIdentidadesOrganizacion");

            migrationBuilder.DropTable(
                name: "GrupoIdentidadesParticipacion");

            migrationBuilder.DropTable(
                name: "GrupoIdentidadesProyecto");

            migrationBuilder.DropTable(
                name: "GrupoOrgParticipaProy");

            migrationBuilder.DropTable(
                name: "GrupoUsuario");

            migrationBuilder.DropTable(
                name: "GrupoUsuarioUsuario");

            migrationBuilder.DropTable(
                name: "HistorialDocumento");

            migrationBuilder.DropTable(
                name: "HistoricoProyectoUsuario");

            migrationBuilder.DropTable(
                name: "IdentidadContadores");

            migrationBuilder.DropTable(
                name: "IdentidadContadoresRecursos");

            migrationBuilder.DropTable(
                name: "InicioSesion");

            migrationBuilder.DropTable(
                name: "IntegracionContinuaPropiedad");

            migrationBuilder.DropTable(
                name: "Invitacion");

            migrationBuilder.DropTable(
                name: "NotificacionAlertaPersona");

            migrationBuilder.DropTable(
                name: "NotificacionCorreoPersona");

            migrationBuilder.DropTable(
                name: "NotificacionEnvioMasivo");

            migrationBuilder.DropTable(
                name: "NotificacionParametro");

            migrationBuilder.DropTable(
                name: "NotificacionParametroPersona");

            migrationBuilder.DropTable(
                name: "NotificacionSolicitud");

            migrationBuilder.DropTable(
                name: "NotificacionSuscripcion");

            migrationBuilder.DropTable(
                name: "OntologiaProyecto");

            migrationBuilder.DropTable(
                name: "OrganizacionClase");

            migrationBuilder.DropTable(
                name: "OrganizacionEmpresa");

            migrationBuilder.DropTable(
                name: "OrganizacionGnoss");

            migrationBuilder.DropTable(
                name: "OrganizacionParticipaProy");

            migrationBuilder.DropTable(
                name: "OrganizacionRolUsuario");

            migrationBuilder.DropTable(
                name: "ParametroAplicacion");

            migrationBuilder.DropTable(
                name: "ParametroGeneral");

            migrationBuilder.DropTable(
                name: "ParametroProyecto");

            migrationBuilder.DropTable(
                name: "PerfilGadget");

            migrationBuilder.DropTable(
                name: "PerfilOrganizacion");

            migrationBuilder.DropTable(
                name: "PerfilPersona");

            migrationBuilder.DropTable(
                name: "PerfilPersonaOrg");

            migrationBuilder.DropTable(
                name: "PerfilRedesSociales");

            migrationBuilder.DropTable(
                name: "PermisoAmigoOrg");

            migrationBuilder.DropTable(
                name: "PermisoGrupoAmigoOrg");

            migrationBuilder.DropTable(
                name: "PermisosPaginasUsuarios");

            migrationBuilder.DropTable(
                name: "PersonaOcupacionFigura");

            migrationBuilder.DropTable(
                name: "PersonaOcupacionFormaSec");

            migrationBuilder.DropTable(
                name: "PersonaVinculoOrganizacion");

            migrationBuilder.DropTable(
                name: "PersonaVisibleEnOrg");

            migrationBuilder.DropTable(
                name: "PeticionInvitacionComunidad");

            migrationBuilder.DropTable(
                name: "PeticionInvitacionGrupo");

            migrationBuilder.DropTable(
                name: "PeticionInvitaContacto");

            migrationBuilder.DropTable(
                name: "PeticionNuevoProyecto");

            migrationBuilder.DropTable(
                name: "PeticionOrgInvitaPers");

            migrationBuilder.DropTable(
                name: "PreferenciaProyecto");

            migrationBuilder.DropTable(
                name: "PresentacionListadoSemantico");

            migrationBuilder.DropTable(
                name: "PresentacionMapaSemantico");

            migrationBuilder.DropTable(
                name: "PresentacionMosaicoSemantico");

            migrationBuilder.DropTable(
                name: "PresentacionPersonalizadoSemantico");

            migrationBuilder.DropTable(
                name: "PresentacionPestanyaListadoSemantico");

            migrationBuilder.DropTable(
                name: "PresentacionPestanyaMapaSemantico");

            migrationBuilder.DropTable(
                name: "PresentacionPestanyaMosaicoSemantico");

            migrationBuilder.DropTable(
                name: "Profesor");

            migrationBuilder.DropTable(
                name: "Provincia");

            migrationBuilder.DropTable(
                name: "ProyectoAgCatTesauro");

            migrationBuilder.DropTable(
                name: "ProyectoCerradoTmp");

            migrationBuilder.DropTable(
                name: "ProyectoCerrandose");

            migrationBuilder.DropTable(
                name: "ProyectoConfigExtraSem");

            migrationBuilder.DropTable(
                name: "ProyectoCookie");

            migrationBuilder.DropTable(
                name: "ProyectoElementoHTMLRol");

            migrationBuilder.DropTable(
                name: "ProyectoEventoAccion");

            migrationBuilder.DropTable(
                name: "ProyectoEventoParticipante");

            migrationBuilder.DropTable(
                name: "ProyectoGadgetContexto");

            migrationBuilder.DropTable(
                name: "ProyectoGadgetContextoHTMLplano");

            migrationBuilder.DropTable(
                name: "ProyectoGadgetIdioma");

            migrationBuilder.DropTable(
                name: "ProyectoGrafoFichaRec");

            migrationBuilder.DropTable(
                name: "ProyectoLoginConfiguracion");

            migrationBuilder.DropTable(
                name: "ProyectoMetaRobots");

            migrationBuilder.DropTable(
                name: "ProyectoPaginaHtml");

            migrationBuilder.DropTable(
                name: "ProyectoPalabrasInapropiadas");

            migrationBuilder.DropTable(
                name: "ProyectoPasoRegistro");

            migrationBuilder.DropTable(
                name: "ProyectoPerfilNumElem");

            migrationBuilder.DropTable(
                name: "ProyectoPestanyaBusquedaExportacionExterna");

            migrationBuilder.DropTable(
                name: "ProyectoPestanyaBusquedaExportacionPropiedad");

            migrationBuilder.DropTable(
                name: "ProyectoPestanyaCMS");

            migrationBuilder.DropTable(
                name: "ProyectoPestanyaExportacionBusqueda");

            migrationBuilder.DropTable(
                name: "ProyectoPestanyaFiltroOrdenRecursos");

            migrationBuilder.DropTable(
                name: "ProyectoPestanyaMenuRolGrupoIdentidades");

            migrationBuilder.DropTable(
                name: "ProyectoPestanyaMenuRolIdentidad");

            migrationBuilder.DropTable(
                name: "ProyectoPestanyaRolGrupoIdentidades");

            migrationBuilder.DropTable(
                name: "ProyectoPestanyaRolIdentidad");

            migrationBuilder.DropTable(
                name: "ProyectoRDFType");

            migrationBuilder.DropTable(
                name: "ProyectoRelacionado");

            migrationBuilder.DropTable(
                name: "ProyectoRolGrupoUsuario");

            migrationBuilder.DropTable(
                name: "ProyectoSearchPersonalizado");

            migrationBuilder.DropTable(
                name: "ProyectoServicioExterno");

            migrationBuilder.DropTable(
                name: "ProyectoServicioWeb");

            migrationBuilder.DropTable(
                name: "ProyectoSinRegistroObligatorio");

            migrationBuilder.DropTable(
                name: "ProyectosMasActivos");

            migrationBuilder.DropTable(
                name: "ProyectoUsuarioIdentidad");

            migrationBuilder.DropTable(
                name: "ProyRolUsuClausulaReg");

            migrationBuilder.DropTable(
                name: "ProyTipoRecNoActivReciente");

            migrationBuilder.DropTable(
                name: "RecursosRelacionadosPresentacion");

            migrationBuilder.DropTable(
                name: "RedireccionValorParametro");

            migrationBuilder.DropTable(
                name: "ResultadoSuscripcion");

            migrationBuilder.DropTable(
                name: "SeccionProyCatalogo");

            migrationBuilder.DropTable(
                name: "Sitemaps");

            migrationBuilder.DropTable(
                name: "SolicitudGrupo");

            migrationBuilder.DropTable(
                name: "SolicitudNuevaOrgEmp");

            migrationBuilder.DropTable(
                name: "SolicitudNuevoProfesor");

            migrationBuilder.DropTable(
                name: "SolicitudNuevoUsuario");

            migrationBuilder.DropTable(
                name: "SolicitudOrganizacion");

            migrationBuilder.DropTable(
                name: "SolicitudUsuario");

            migrationBuilder.DropTable(
                name: "SuscripcionBlog");

            migrationBuilder.DropTable(
                name: "SuscripcionIdentidadProyecto");

            migrationBuilder.DropTable(
                name: "SuscripcionTesauroOrganizacion");

            migrationBuilder.DropTable(
                name: "SuscripcionTesauroProyecto");

            migrationBuilder.DropTable(
                name: "SuscripcionTesauroUsuario");

            migrationBuilder.DropTable(
                name: "TesauroOrganizacion");

            migrationBuilder.DropTable(
                name: "TesauroProyecto");

            migrationBuilder.DropTable(
                name: "TesauroUsuario");

            migrationBuilder.DropTable(
                name: "TextosPersonalizadosPersonalizacion");

            migrationBuilder.DropTable(
                name: "TextosPersonalizadosPlataforma");

            migrationBuilder.DropTable(
                name: "TextosPersonalizadosProyecto");

            migrationBuilder.DropTable(
                name: "TipoDocDispRolUsuarioProy");

            migrationBuilder.DropTable(
                name: "TipoDocImagenPorDefecto");

            migrationBuilder.DropTable(
                name: "TipoOntoDispRolUsuarioProy");

            migrationBuilder.DropTable(
                name: "TokenApiLogin");

            migrationBuilder.DropTable(
                name: "UltimosDocumentosVisitados");

            migrationBuilder.DropTable(
                name: "UsuarioContadores");

            migrationBuilder.DropTable(
                name: "UsuarioRedirect");

            migrationBuilder.DropTable(
                name: "UsuarioVinculadoLoginRedesSociales");

            migrationBuilder.DropTable(
                name: "VersionDocumento");

            migrationBuilder.DropTable(
                name: "VistaVirtual");

            migrationBuilder.DropTable(
                name: "VistaVirtualCMS");

            migrationBuilder.DropTable(
                name: "VistaVirtualGadgetRecursos");

            migrationBuilder.DropTable(
                name: "VistaVirtualProyecto");

            migrationBuilder.DropTable(
                name: "VistaVirtualRecursos");

            migrationBuilder.DropTable(
                name: "VotoComentario");

            migrationBuilder.DropTable(
                name: "VotoDocumento");

            migrationBuilder.DropTable(
                name: "VotoEntradaBlog");

            migrationBuilder.DropTable(
                name: "VotoMensajeForo");

            migrationBuilder.DropTable(
                name: "GrupoAmigos");

            migrationBuilder.DropTable(
                name: "Blog");

            migrationBuilder.DropTable(
                name: "Carga");

            migrationBuilder.DropTable(
                name: "CMSBloqueComponente");

            migrationBuilder.DropTable(
                name: "ComparticionAutomatica");

            migrationBuilder.DropTable(
                name: "DatoExtraEcosistema");

            migrationBuilder.DropTable(
                name: "DatoExtraProyecto");

            migrationBuilder.DropTable(
                name: "DatoExtraProyectoVirtuoso");

            migrationBuilder.DropTable(
                name: "AtributoFichaBibliografica");

            migrationBuilder.DropTable(
                name: "DocumentoRespuesta");

            migrationBuilder.DropTable(
                name: "Tipologia");

            migrationBuilder.DropTable(
                name: "CategoriaTesauro");

            migrationBuilder.DropTable(
                name: "DocumentoWebVinBaseRecursos");

            migrationBuilder.DropTable(
                name: "FacetaObjetoConocimientoProyecto");

            migrationBuilder.DropTable(
                name: "GrupoIdentidades");

            migrationBuilder.DropTable(
                name: "Notificacion");

            migrationBuilder.DropTable(
                name: "Organizacion");

            migrationBuilder.DropTable(
                name: "Peticion");

            migrationBuilder.DropTable(
                name: "Pais");

            migrationBuilder.DropTable(
                name: "CategoriaProyectoCookie");

            migrationBuilder.DropTable(
                name: "ProyectoElementoHtml");

            migrationBuilder.DropTable(
                name: "ProyectoEvento");

            migrationBuilder.DropTable(
                name: "ProyectoGadget");

            migrationBuilder.DropTable(
                name: "ProyectoPestanyaBusquedaExportacion");

            migrationBuilder.DropTable(
                name: "ProyectoPestanya");

            migrationBuilder.DropTable(
                name: "ProyectoRegistroObligatorio");

            migrationBuilder.DropTable(
                name: "Identidad");

            migrationBuilder.DropTable(
                name: "ClausulaRegistro");

            migrationBuilder.DropTable(
                name: "ProyectoRolUsuario");

            migrationBuilder.DropTable(
                name: "RedireccionRegistroRuta");

            migrationBuilder.DropTable(
                name: "SitemapsIndex");

            migrationBuilder.DropTable(
                name: "SolicitudNuevaOrganizacion");

            migrationBuilder.DropTable(
                name: "Suscripcion");

            migrationBuilder.DropTable(
                name: "VistaVirtualPersonalizacion");

            migrationBuilder.DropTable(
                name: "Comentario");

            migrationBuilder.DropTable(
                name: "Voto");

            migrationBuilder.DropTable(
                name: "CMSBloque");

            migrationBuilder.DropTable(
                name: "CMSComponente");

            migrationBuilder.DropTable(
                name: "FichaBibliografica");

            migrationBuilder.DropTable(
                name: "Tesauro");

            migrationBuilder.DropTable(
                name: "BaseRecursos");

            migrationBuilder.DropTable(
                name: "Documento");

            migrationBuilder.DropTable(
                name: "NivelCertificacion");

            migrationBuilder.DropTable(
                name: "FacetaHome");

            migrationBuilder.DropTable(
                name: "ProyectoPestanyaBusqueda");

            migrationBuilder.DropTable(
                name: "Perfil");

            migrationBuilder.DropTable(
                name: "Solicitud");

            migrationBuilder.DropTable(
                name: "CMSPagina");

            migrationBuilder.DropTable(
                name: "ProyectoPestanyaMenu");

            migrationBuilder.DropTable(
                name: "Persona");

            migrationBuilder.DropTable(
                name: "Proyecto");

            migrationBuilder.DropTable(
                name: "Usuario");
        }
    }
}
