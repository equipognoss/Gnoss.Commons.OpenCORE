using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.Documentacion;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models;
using Es.Riam.Gnoss.AD.EntityModel.Models.Carga;
using Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion;
using Es.Riam.Gnoss.AD.EntityModel.Models.Faceta;
using Es.Riam.Gnoss.AD.EntityModel.Models.IdentidadDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.OrganizacionDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.PersonaDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.Identidad;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Gnoss.Web.MVC.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Es.Riam.Gnoss.AD.ServiciosGenerales
{
    #region Enumeraciones

    /// <summary>
    /// Enumeración para distinguir tipos de proyectos
    /// </summary>
    public enum TipoProyecto
    {
        /// <summary>
        /// Proyecto de organización
        /// </summary>
        DeOrganizacion = 0,
        /// <summary>
        /// Comunidad web
        /// </summary>
        Comunidad = 1,
        /// <summary>
        /// Metacomunidad
        /// </summary>
        MetaComunidad = 2,
        /// <summary>
        /// Universidad 2.0
        /// </summary>
        Universidad20 = 3,
        /// <summary>
        /// Educacion Expandida
        /// </summary>
        EducacionExpandida = 4,
        /// <summary>
        /// Educacion Expandida
        /// </summary>
        Catalogo = 5,
        /// <summary>
        /// Educación primaria
        /// </summary>
        EducacionPrimaria = 6,
        /// <summary>
        /// Catalogo no social publico con un unico tipo de recurso
        /// </summary>
        CatalogoNoSocialConUnTipoDeRecurso = 7,
        /// <summary>
        /// Catalogo no social
        /// </summary>
        CatalogoNoSocial = 8
    }

    public static class TipoProyectoExt
    {
        public static string ToFriendlyString(this TipoProyecto tipoProyecto)
        {
            switch (tipoProyecto)
            {
                case TipoProyecto.Catalogo:
                    return "catalog";
                case TipoProyecto.CatalogoNoSocial:
                    return "not social catalog";
                case TipoProyecto.CatalogoNoSocialConUnTipoDeRecurso:
                    return "one type resource not social catalog";
                case TipoProyecto.Comunidad:
                    return "community";
                case TipoProyecto.DeOrganizacion:
                    return "gnoss project";
                case TipoProyecto.EducacionExpandida:
                    return "secondary education";
                case TipoProyecto.EducacionPrimaria:
                    return "primary education";
                case TipoProyecto.MetaComunidad:
                    return "meta community";
                case TipoProyecto.Universidad20:
                    return "university";
                default:
                    return tipoProyecto.ToString();
            }
        }
    }

    /// <summary>
    /// Enumeración para distinguir tipos de acceso
    /// </summary>
    public enum TipoAcceso
    {
        /// <summary>
        /// Proyecto público
        /// </summary>
        Publico = 0,
        /// <summary>
        /// Proyecto privado
        /// </summary>
        Privado = 1,
        /// <summary>
        /// Proyecto restringido
        /// </summary>
        Restringido = 2,
        /// <summary>
        /// Proyecto reservado
        /// </summary>
        Reservado = 3
    }

    /// <summary>
    /// Enumeración para distinguir estados de un proyecto
    /// </summary>
    public enum EstadoProyecto
    {
        /// <summary>
        /// Proyecto bloqueado/cerrado
        /// </summary>
        Cerrado = 0,
        /// <summary>
        /// Proyecto bloqueado temporalmente x mantenimiento
        /// </summary>
        CerradoTemporalmente = 1,
        /// <summary>
        /// Proyecto en definicion
        /// </summary>
        Definicion = 2,
        /// <summary>
        /// Proyecto abierto
        /// </summary>
        Abierto = 3,
        /// <summary>
        /// Proyecto que se esta cerrando y esta unos dias en estado de gracia
        /// </summary>
        Cerrandose = 4
    }

    /// <summary>
    /// Enumeración para distinguir tipos administradores/supervisores/usuarios del proyecto
    /// </summary>
    public enum TipoRolUsuario
    {
        /// <summary>
        /// Administrador
        /// </summary>
        Administrador = 0,
        /// <summary>
        /// Supervisor
        /// </summary>
        Supervisor = 1,
        /// <summary>
        /// Usuario
        /// </summary>
        Usuario = 2,
        /// <summary>
        /// Diseñador
        /// </summary>
        Diseniador = 3
    }

    /// <summary>
    /// Vistas que pueden tener los recursos en la home de tipo catálogo
    /// </summary>
    public enum TipoVistaHomeCatalogo
    {
        /// <summary>
        /// Listado
        /// </summary>
        Listado = 0,

        /// <summary>
        /// Mosaicp
        /// </summary>
        Mosaico = 1,
    }

    public enum TipoUbicacionGadget
    {
        /// <summary>
        /// Gadgets en el lateral de la Home de la comunidad(No catálogos)
        /// </summary>
        LateralHomeComunidad = 0,
        /// <summary>
        /// Gadgets en la ficha de un recurso de la comunidad
        /// </summary>
        FichaRecursoComunidad = 1,
        /// <summary>
        /// Gadgets en el pie de la home de la comunidad
        /// </summary>
        PieHomeComunidad = 2,
        /// <summary>
        /// Gadgets en el cuerpo de la home de la comunidad
        /// </summary>
        CuerpoHomeComunidad = 3,
        /// <summary>
        /// Gadgets en la cabecera cuerpo de de la home de la comunidad
        /// </summary>
        CabeceraHomeComunidad = 4,
        /// <summary>
        /// Gadgets en la cabecera del índice de la comunidad
        /// </summary>
        CabeceraIndiceComunidad = 5,
        /// <summary>
        /// Gadgets en la cabecera de la página de recursos
        /// </summary>
        CabeceraRecursosComunidad = 6,
        /// <summary>
        /// Gadgets en la cabecera de las pestañas de la comunidad
        /// </summary>
        CabeceraPestanyasComunidad = 7,
        /// <summary>
        /// Gadgets en la cabecera de la página de debates
        /// </summary>
        CabeceraDebatesComunidad = 8,
        /// <summary>
        /// Gadgets en la cabecera de la página de dafos
        /// </summary>
        CabeceraDafosComunidad = 9,
        /// <summary>
        /// Gadgets en la cabecera de la página de preguntas
        /// </summary>
        CabeceraPreguntasComunidad = 10,
        /// <summary>
        /// Gadgets en la cabecera de la página de encuestas
        /// </summary>
        CabeceraEncuestasComunidad = 11,
        /// <summary>
        /// Gadgets en la cabecera de la página de personas y organizaciones
        /// </summary>
        CabeceraPersonasYOrgDeComunidad = 12,
        /// <summary>
        /// Gadgets en la cabecera de la página de acerca-de
        /// </summary>
        CabeceraAcercaDeComunidad = 13,
    }

    public enum TipoEventoProyecto
    {
        /// <summary>
        /// Evento disponible para todo el mundo
        /// </summary>
        SinRestriccion = 0,
        /// <summary>
        /// Evento solo disponible para nuevos miembros en la comunidad
        /// </summary>
        NuevoEnComunidad = 1,
        /// <summary>
        /// Evento solo disponible para nuevos miembros en el ecosistema
        /// </summary>
        NuevoEnEcosistema = 2,
    }

    /// <summary>
    /// Enumeración para distinguir los tipos de cabeceras
    /// </summary>
    public enum TipoCabeceraProyecto
    {
        /// <summary>
        /// Cabecera normal
        /// </summary>
        Normal = 0,
        /// <summary>
        /// Cabecera simplificada
        /// </summary>
        Simplificada = 1,
    }

    /// <summary>
    /// Enumeración para distinguir los tipos de fichas
    /// </summary>
    public enum TipoFichaRecursoProyecto
    {
        /// <summary>
        /// Ficha normal
        /// </summary>
        Normal = 0,
        /// <summary>
        /// FIcha de inevery
        /// </summary>
        Inevery = 1,
    }

    /// <summary>
    /// Enumeracion para identificar los tipos de eventos en las acciones de un proyecto
    /// </summary>
    public enum TipoProyectoEventoAccion
    {
        Registro = 0,
        Login = 1,
        LecturaArticulo = 3,

        // No usar el setTimeout!!!
        CompartirContenidoGoogle = 4,
        CompartirContenidoTwitter = 5,
        CompartirContenidoFacebook = 6,
        CompartirContenidoDelicious = 7,
        CompartirContenidoLinkedIn = 8,
        CompartirContenidoReddit = 9,
        CompartirContenidoBlogger = 10,
        CompartirContenidoDiigo = 11,

        PublicarEnalce = 20,
        PublicarVideoAudio = 21,
        PublicarAdjunto = 22,
        PublicarNota = 23,
        PublicarWiki = 24,
        PublicarPregunta = 25,
        PublicarDebate = 26,

        EnviarEnlace = 40,

        RegistroGoogle = 51,
        RegistroFacebook = 52,
        RegistroTwitter = 53,
        RegistroSantillana = 54,
        InicioRegistroGoogle = 55,
        InicioRegistroFacebook = 56,
        InicioRegistroTwitter = 57,
        InicioRegistroSantillana = 58,
        LoginGoogle = 61,
        LoginFacebook = 62,
        LoginTwitter = 63,
        LoginSantillana = 64,

        ClickGoogle = 71,
        ClickFacebook = 72,
        ClickTwitter = 73,
        ClickSantillana = 74,

        LoginConToken = 80,
        RegistroConToken = 81
    }

    /// <summary>
    /// Enumeración para distinguir los pasos del registro
    /// </summary>
    public enum PasosRegistro
    {
        /// <summary>
        /// Página de Preferencias
        /// </summary>
        Preferencias = 0,
        /// <summary>
        /// Página de Datos
        /// </summary>
        Datos = 1,
        /// <summary>
        /// Página de Conecta
        /// </summary>
        Conecta = 2
    }

    /// <summary>
    /// Enumeración para distinguir los campos genericos del registro
    /// </summary>
    public enum TipoCampoGenericoRegistro
    {
        /// <summary>
        /// País
        /// </summary>
        Pais = 0,
        /// <summary>
        /// Provincia
        /// </summary>
        Provincia = 1,
        /// <summary>
        /// Localidad
        /// </summary>
        Localidad = 2,
        /// <summary>
        /// Sexo
        /// </summary>
        Sexo = 3
    }

    /// <summary>
    /// Enumeración para distinguir tipos de privacidad de una pagina
    /// </summary>
    public enum TipoPrivacidadPagina
    {
        /// <summary>
        /// Hereda la privacidad de la comunidad (publica, privada..)
        /// </summary>
        Normal = 0,
        /// <summary>
        /// Tiene la privacidad contraria a la comunidad (si la comunidad es public es visible solo para miembros y si la comunidad es privada es publico para todo el mundo)
        /// </summary>
        Especial = 1,
        /// <summary>
        /// Solo tienen acceso los perfiles y grupos seleccionados
        /// </summary>
        Lectores = 2,
    }

    /// <summary>
    /// Tipos de configuración extra para los elementos semánticos de una comunidad.
    /// </summary>
    public enum TipoConfigExtraSemantica
    {
        /// <summary>
        /// Configuración para tesauro semántico.
        /// </summary>
        TesauroSemantico = 0,
        /// <summary>
        /// Configuración para una entidad secundaria.
        /// </summary>
        EntidadSecundaria = 1,
        /// <summary>
        /// Configuración para un grafo simple.
        /// </summary>
        GrafoSimple = 2
    }

    #endregion

    /// <summary>
    /// DataAdapter de proyecto
    /// </summary>
    public class ProyectoAD : BaseAD
    {
        #region Miembros

        /// <summary>
        /// Id del proyecto myGnoss
        /// </summary>
        private static Guid mMyGnoss = new Guid("11111111-1111-1111-1111-111111111111");

        /// <summary>
        /// Id de la meta-organizacion
        /// </summary>
        private static Guid mMetaOrganizacion = new Guid("11111111-1111-1111-1111-111111111111");

        /// <summary>
        /// Id del meta-proyecto
        /// </summary>
        private static Guid mMetaProyecto = new Guid("11111111-1111-1111-1111-111111111111");

        /// <summary>
        /// Tabla base del metaproyecto
        /// </summary>
        private static int mTablaBaseIdMetaProyecto = int.MinValue;

        private EntityContext mEntityContext;
        private ConfigService mConfigService;
        private LoggingService mLoggingService;

        #endregion

        #region Constructores

        /// <summary>
        /// Constructor por defecto, sin parámetros, utilizado cuando se requiere el GnossConfig.xml por defecto
        /// </summary>
        public ProyectoAD(LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication)
        {
            mEntityContext = entityContext;
            mLoggingService = loggingService;
            mConfigService = configService;

            this.CargarConsultasYDataAdapters();
        }

        /// <summary>
        /// Constructor a partir del fichero de configuración de conexión a la base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuración de la conexión a base de datos</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public ProyectoAD(string pFicheroConfiguracionBD, LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication)
        {
            mEntityContext = entityContext;
            mLoggingService = loggingService;
            mConfigService = configService;

            this.CargarConsultasYDataAdapters(IBD);
        }

        #endregion

        #region Consultas

        private string sqlSelectAdministradoresProyecto;
        private string sqlSelectAdministradoresGrupoProyecto;
        private string sqlSelectProyectoGadget;
        private string sqlSelectProyectoGadgetContexto;
        //private string sqlSelectProyectoGadgetContextoHTMLplano;
        private string sqlSelectExisteNombreCortoGadgetEnBD;
        private string sqlSelectProyectoGadgetIdioma;
        private string sqlSelectProyectoPestanyaFiltroOrdenRecursos;
        private string sqlSelectRecursosRelacionadosPresentacion;
        private string sqlSelectProyectoPestanyaMenu;
        private string sqlSelectProyectoPestanyaCMS;
        private string sqlSelectProyectoPestanyaBusqueda;
        private string sqlSelectProyectoPestanyaMenuRolIdentidad;
        private string sqlSelectProyectoPestanyaMenuRolGrupoIdentidades;
        private string sqlSelectProyectoPasoRegistro;
        private string sqlSelectProyectoPasoRegistroPorProyectoID;
        private string sqlSelectProyectoPaginaHtml;
        private string sqlSelectProyectoRelacionado;
        private string sqlSelectProyectosCerrandose;
        private string sqlSelectProyectosCerrandoTmp;
        private string sqlSelectProyectoSearchPersonalizado;
        private string sqlSelectProyectoSearchPersonalizadoPorProyectoID;
        private string sqlSelectOntologiaProyecto;
        private string sqlSelectOntologiaProyectoPorProyectoID;
        private string sqlSelectVistaVirtualProyecto;
        private string sqlSelectVistaVirtualProyectoPorProyectoID;
        private string sqlSelectPresentacionListadoSemantico;
        private string sqlSelectPresentacionMosaicoSemantico;
        private string sqlSelectPresentacionMapaSemantico;
        private string sqlSelectPresentacionPestanyaListadoSemantico;
        private string sqlSelectPresentacionPestanyaMosaicoSemantico;
        private string sqlSelectPresentacionPestanyaMapaSemantico;
        private string sqlSelectUrlPropiaProyecto;
        private string sqlSelectUrlPropiaProyectoPorNombreCorto;
        private string sqlSelectNombreCortoProyecto;
        private string sqlSelectOrganizacionIDProyecto;
        private string sqlSelectProyectosParticipaPerfilUsuarioSinMyGNOSS;
        private string sqlSelectWikiDeProyecto;
        private string sqlSelectProyectoAgCatTesauro;
        private string sqlSelectNivelCertificacion;
        private string sqlDeleteTodosProyectosMasActivos;
        private string sqlSelectProyectosMasActivos;
        private string sqlSelectProyectoDestacado;
        private string sqlSelectProyectosOrganizacion;
        private string sqlSelectTodosProyectos;
        private string sqlSelectTodosProyectosLigera;
        private string sqlSelectTodosAdministradoresProyectos;
        private string sqlSelectTodosAdministradoresGrupoProyectos;
        private string sqlSelectProyectosParticipaUsuario;
        private string sqlSelectProyectosParticipaUsuarioEnModoPersonal;
        private string sqlSelectProyectosUsuarioPuedeCompartirRecurso;
        private string sqlSelectProyectosUsuarioPuedeCompartirRecursoSinComprobarTipoDoc;
        private string sqlSelectProyectoUsuarioIdentidadUsuarioPuedeCompartirRecurso;
        private string sqlSelectProyectoUsuarioIdentidadUsuarioPuedeCompartirRecursoSinComprobarTipoDoc;
        private string sqlSelectProyectosParticipaUsuarioSinBloquear;
        private string sqlSelectProyectosParticipaUsuarioConPerfil;
        private string sqlSelectProyectoParticipaIdentidad;
        private string sqlSelectProyectoUsuarioIdentidadParticipaUsuario;
        private string sqlSelectProyectoUsuarioIdentidadParticipaUsuarioEnModoPersonal;
        private string sqlSelectProyectoUsuarioIdentidadParticipaUsuarioSinBloquear;
        private string sqlSelectProyectosParticipaPerfilUsuario;
        private string sqlSelectProyectosParticipaOrganizacion;
        private string sqlSelectProyectosParticipaOrganizacionPaginado;
        private string sqlSelectProyectosParticipaOrganizacionSinMyGNOSS;
        private string sqlSelectProyectosOrganizacionParticipaUsuario;
        private string sqlSelectProyectoInicioSesionDeUsuario;
        private string sqlSelectProyectosPuedoCompartirDefinicion;
        private string sqlSelectProyectoPorID;
        private string sqlSelectAdministradoresProyectoPorID;
        private string sqlSelectAdministradoresGrupoProyectoPorID;
        private string sqlSelectAdministradoresProyectoPorProyIDYUsuarioID;
        private string sqlSelectProyectoAgCatTesauroPorID;
        private string sqlSelectProyectosPorID;
        private string sqlSelectProyectosPorCategoria;
        private string sqlSelectProyectosDestacados;
        private string sqlSelectUltimosProyectos;
        private string sqlSelectProyectosPublicosPrimerNivel;
        private string sqlSelectProyectosPorPeso;
        private string sqlSelectComunidadesMasActivas;
        private string sqlSelectProyectosParticipaUsuarioDeLaOrganizacion;
        private string sqlSelectAdministradorProyectoParticipaUsuarioDeLaOrganizacion;
        private string sqlSelectProyectosParticipaUsuarioConSuPerfilPersonal;
        private string sqlSelectAdministradorProyectoParticipaUsuarioConSuPerfilPersonal;
        private string sqlSelectProyectosOrganizacionCargaLigera;
        private string sqlSelectExisteNombreCortoEnBD;
        private string sqlSelectExisteNombreEnBD;
        private string sqlUpdateAumentarNumeroMiembrosDelProyecto;
        private string sqlUpdateDisminuirNumeroMiembrosDelProyecto;
        private string sqlUpdateDisminuirNumeroOrParticipanEnProyecto;
        private string sqlUpdateAumentarNumeroOrgParticipanEnProyecto;
        private string sqlSelectUsuariosExceptoLosAdministradores;
        private string sqlSelectEntidadesGnossExceptoLosGestores;
        private string sqlSelectTieneCategoriasDeTesauro;
        private string sqlSelectTieneSolicitudesCategoriasDeTesauro;
        private string sqlSelectTieneDefiniciones;
        private string sqlSelectExisteProyectoFAQ;
        private string sqlSelectExisteProyectoNoticias;
        private string sqlSelectExisteProyectoDidactalia;
        private string sqlSelectNivelCertificacionPorID;
        private string sqlSelectTipoDocDispRolUsuarioProy;
        private string sqlSelectTipoOntoDispRolUsuarioProy;
        private string sqlSelectNombreCortoProyectoConSoloProyectoID;
        private string sqlSelectNombreDeProyecto;
        private string sqlSelectTipoDocDispRolUsuarioProyPorID;
        private string sqlSelectExisteDocAsociadoANivelCertif;
        private string sqlSelectProyectoPorAdministradorID;
        private string sqlSelectProyectoCerradoTmpPorID;
        private string sqlSelectProyectoCerrandosePorID;
        private string sqlSelectEmailsMiembrosDeProyecto;
        private string sqlSelectEmailsMiembrosDeEventoDeProyecto;
        private string sqlSelectEmailsAdministradoresDeProyecto;
        private string sqlSelectTablaBaseProyectoIDDeProyectoPorID;
        private string sqlSelectTodosIDProyectosConIDNumerico;
        private string sqlSelectDatoExtraProyectoPorIDProyecto;
        private string sqlSelectDatoExtraProyectoPorListaProyectosID;
        private string sqlSelectDatoExtraProyectoOpcionPorIDProyecto;
        private string sqlSelectDatoExtraProyectoOpcionPorListaProyectosID;
        private string sqlSelectDatoExtraProyectoVirtuosoPorIDProyecto;
        private string sqlSelectDatoExtraProyectoVirtuosoPorListaProyectosID;
        private string sqlSelectCamposRegistroProyectoGenericosPorIDProyecto;
        private string sqlSelectCamposRegistroProyectoGenericosPorListaProyectosID;
        private string sqlSelectDatoExtraEcosistema;
        private string sqlSelectDatoExtraEcosistemaOpcion;
        private string sqlSelectDatoExtraEcosistemaVirtuoso;
        private string sqlSelectPreferenciaProyectoPorIDProyecto;
        private string sqlSelectEventosProyectoPorProyectoID;
        private string sqlSelectEventosProyectoPorIdentidadID;
        private string sqlSelectEventoProyectoPorEventoID;
        private string sqlSelectEventoProyectoParticipantesPorEventoID;
        private string sqlSelectProyTipoRecNoActivReciente;
        private string sqlSelectTipoDocImagenPorDefecto;
        private string sqlSelectProyectoGrafoFichaRec;
        private string sqlSelectAccionesExternasProyecto;
        private string sqlSelectAccionesExternasProyectoPorProyectoID;
        private string sqlSelectAccionesExternasProyectoPorListaID;
        private string sqlSelectProyectoConfigExtraSem;

        //Consultas parte select
        private string sqlSelectProyectoCerradoTmp;
        private string sqlSelectProyectoCerrandose;
        private string SelectProyectoPesado;
        private string SelectProyectoLigero;
        private string SelectDistinctProyectoLigero;
        private string SelectAdministradorProyecto;
        private string SelectAdministradorGrupoProyecto;
        private string SelectProyectoServicioExterno;
        //private string SelectEcosistemaServicioExterno;

        private string sqlSelectProyectoPerfilNumElem;
        private string sqlSelectSeccionProyCatalogo;
        private string sqlSelectProyectoLoginConfiguracion;
        private string sqlSelectEventosAccionProyectoPorProyectoID;

        #region Documentación

        private string sqlSelectTipoDocsDispDeUsuarioEnUNProy;

        #endregion

        #region Twitter

        private string sqlUpdateTokenTwitterProyecto;

        #endregion

        #endregion

        #region DataAdapters

        #region ProyectoPestanyaFiltroOrdenRecursos
        private string sqlProyectoPestanyaFiltroOrdenRecursosInsert;
        private string sqlProyectoPestanyaFiltroOrdenRecursosDelete;
        private string sqlProyectoPestanyaFiltroOrdenRecursosModify;
        #endregion

        #region ProyectoRelacionado
        private string sqlProyectoRelacionadoInsert;
        private string sqlProyectoRelacionadoDelete;
        private string sqlProyectoRelacionadoModify;
        #endregion

        #region ProyectoAgCatTesauro
        private string sqlProyectoAgCatTesauroInsert;
        private string sqlProyectoAgCatTesauroDelete;
        private string sqlProyectoAgCatTesauroModify;
        #endregion

        #region Proyecto
        private string sqlProyectoInsert;
        private string sqlProyectoDelete;
        private string sqlProyectoModify;
        #endregion

        #region ProyectosMasActivos
        private string sqlProyectosMasActivosInsert;
        private string sqlProyectosMasActivosDelete;
        private string sqlProyectosMasActivosModify;
        #endregion

        #region AdministradorProyecto
        private string sqlAdministradorProyectoInsert;
        private string sqlAdministradorProyectoDelete;
        private string sqlAdministradorProyectoModify;
        #endregion

        #region AdministradorGrupoProyecto
        private string sqlAdministradorGrupoProyectoInsert;
        private string sqlAdministradorGrupoProyectoDelete;
        private string sqlAdministradorGrupoProyectoModify;
        #endregion

        #region ProyectoContactoEmpleadoDataAdapter
        private string sqlProyectoContactoEmpleadoInsert;
        private string sqlProyectoContactoEmpleadoDelete;
        private string sqlProyectoContactoEmpleadoModify;
        #endregion

        #region NivelCertificacion
        private string sqlNivelCertificacionInsert;
        private string sqlNivelCertificacionDelete;
        private string sqlNivelCertificacionModify;
        #endregion

        #region TipoDocDispRolUsuarioProy
        private string sqlTipoDocDispRolUsuarioProyInsert;
        private string sqlTipoDocDispRolUsuarioProyDelete;
        private string sqlTipoDocDispRolUsuarioProyModify;
        #endregion

        #region TipoOntoDispRolUsuarioProy
        private string sqlTipoOntoDispRolUsuarioProyInsert;
        private string sqlTipoOntoDispRolUsuarioProyDelete;
        private string sqlTipoOntoDispRolUsuarioProyModify;
        #endregion

        #region ProyectoCerradoTmp
        private string sqlProyectoCerradoTmpInsert;
        private string sqlProyectoCerradoTmpDelete;
        private string sqlProyectoCerradoTmpModify;
        #endregion

        #region ProyectoCerrandose
        private string sqlProyectoCerrandoseInsert;
        private string sqlProyectoCerrandoseDelete;
        private string sqlProyectoCerrandoseModify;
        #endregion

        #region ProyectoGadget

        private string sqlProyectoGadgetInsert;
        private string sqlProyectoGadgetDelete;
        private string sqlProyectoGadgetModify;

        #endregion

        #region ProyectoGadgetContexto

        private string sqlProyectoGadgetContextoInsert;
        private string sqlProyectoGadgetContextoDelete;
        private string sqlProyectoGadgetContextoModify;

        #endregion

        #region ProyectoGadgetContextoHTMLplano

        //private string sqlProyectoGadgetContextoHTMLplanoInsert;
        //private string sqlProyectoGadgetContextoHTMLplanoDelete;
        //private string sqlProyectoGadgetContextoHTMLplanoModify;

        #endregion

        #region ProyectoGadgetIdioma

        private string sqlProyectoGadgetIdiomaInsert;
        private string sqlProyectoGadgetIdiomaDelete;
        private string sqlProyectoGadgetIdiomaModify;

        #endregion

        #region RecursosRelacionadosPresentacion

        private string sqlRecursosRelacionadosPresentacionInsert;
        private string sqlRecursosRelacionadosPresentacionDelete;
        private string sqlRecursosRelacionadosPresentacionModify;

        #endregion

        #region ProyectoPestanyaMenu

        private string sqlProyectoPestanyaMenuInsert;
        private string sqlProyectoPestanyaMenuDelete;
        private string sqlProyectoPestanyaMenuModify;

        #endregion

        #region ProyectoPestanyaCMS

        private string sqlProyectoPestanyaCMSInsert;
        private string sqlProyectoPestanyaCMSDelete;
        private string sqlProyectoPestanyaCMSModify;

        #endregion

        #region ProyectoPestanyaBusqueda

        private string sqlProyectoPestanyaBusquedaInsert;
        private string sqlProyectoPestanyaBusquedaDelete;
        private string sqlProyectoPestanyaBusquedaModify;

        #endregion

        #region ProyectoPestanyaMenuRolGrupoIdentidades

        private string sqlProyectoPestanyaMenuRolGrupoIdentidadesInsert;
        private string sqlProyectoPestanyaMenuRolGrupoIdentidadesDelete;
        private string sqlProyectoPestanyaMenuRolGrupoIdentidadesModify;

        #endregion

        #region ProyectoPestanyaMenuRolIdentidad

        private string sqlProyectoPestanyaMenuRolIdentidadInsert;
        private string sqlProyectoPestanyaMenuRolIdentidadDelete;
        private string sqlProyectoPestanyaMenuRolIdentidadModify;

        #endregion

        #region ProyectoPaginaHtml

        private string sqlProyectoPaginaHtmlInsert;
        private string sqlProyectoPaginaHtmlDelete;
        private string sqlProyectoPaginaHtmlModify;

        #endregion

        #region ProyectoPerfilNumElem
        private string sqlProyectoPerfilNumElemInsert;
        private string sqlProyectoPerfilNumElemDelete;
        private string sqlProyectoPerfilNumElemModify;
        #endregion

        #region SeccionProyCatalogo
        private string sqlSeccionProyCatalogoInsert;
        private string sqlSeccionProyCatalogoDelete;
        private string sqlSeccionProyCatalogoModify;
        #endregion

        #region PresentacionListadoSemantico
        private string sqlPresentacionListadoSemanticoInsert;
        private string sqlPresentacionListadoSemanticoDelete;
        private string sqlPresentacionListadoSemanticoModify;
        #endregion

        #region PresentacionMosaicoSemantico
        private string sqlPresentacionMosaicoSemanticoInsert;
        private string sqlPresentacionMosaicoSemanticoDelete;
        private string sqlPresentacionMosaicoSemanticoModify;
        #endregion

        #region PresentacionMapaSemantico
        private string sqlPresentacionMapaSemanticoInsert;
        private string sqlPresentacionMapaSemanticoDelete;
        private string sqlPresentacionMapaSemanticoModify;
        #endregion

        #region PresentacionPestanyaListadoSemantico
        private string sqlPresentacionPestanyaListadoSemanticoInsert;
        private string sqlPresentacionPestanyaListadoSemanticoDelete;
        private string sqlPresentacionPestanyaListadoSemanticoModify;
        #endregion

        #region PresentacionPestanyaMosaicoSemantico
        private string sqlPresentacionPestanyaMosaicoSemanticoInsert;
        private string sqlPresentacionPestanyaMosaicoSemanticoDelete;
        private string sqlPresentacionPestanyaMosaicoSemanticoModify;
        #endregion

        #region PresentacionPestanyaMapaSemantico
        private string sqlPresentacionPestanyaMapaSemanticoInsert;
        private string sqlPresentacionPestanyaMapaSemanticoDelete;
        private string sqlPresentacionPestanyaMapaSemanticoModify;
        #endregion

        #region ProyectoLoginConfiguracion
        private string sqlProyectoLoginConfiguracionInsert;
        private string sqlProyectoLoginConfiguracionDelete;
        private string sqlProyectoLoginConfiguracionModify;
        #endregion

        #region CamposRegistroProyectoGenericos
        private string sqlCamposRegistroProyectoGenericosInsert;
        private string sqlCamposRegistroProyectoGenericosDelete;
        private string sqlCamposRegistroProyectoGenericosModify;
        #endregion

        #region DatoExtraProyecto
        private string sqlDatoExtraProyectoInsert;
        private string sqlDatoExtraProyectoDelete;
        private string sqlDatoExtraProyectoModify;
        #endregion

        #region DatoExtraProyectoOpcion
        private string sqlDatoExtraProyectoOpcionInsert;
        private string sqlDatoExtraProyectoOpcionDelete;
        private string sqlDatoExtraProyectoOpcionModify;
        #endregion

        #region DatoExtraProyectoVirtuoso
        private string sqlDatoExtraProyectoVirtuosoInsert;
        private string sqlDatoExtraProyectoVirtuosoDelete;
        private string sqlDatoExtraProyectoVirtuosoModify;
        #endregion

        #region DatoExtraEcosistema
        private string sqlDatoExtraEcosistemaInsert;
        private string sqlDatoExtraEcosistemaDelete;
        private string sqlDatoExtraEcosistemaModify;
        #endregion

        #region DatoExtraEcosistemaOpcion
        private string sqlDatoExtraEcosistemaOpcionInsert;
        private string sqlDatoExtraEcosistemaOpcionDelete;
        private string sqlDatoExtraEcosistemaOpcionModify;
        #endregion

        #region DatoExtraEcosistemaVirtuoso
        private string sqlDatoExtraEcosistemaVirtuosoInsert;
        private string sqlDatoExtraEcosistemaVirtuosoDelete;
        private string sqlDatoExtraEcosistemaVirtuosoModify;
        #endregion

        #region PreferenciaProyecto

        private string sqlPreferenciaProyectoInsert;
        private string sqlPreferenciaProyectoDelete;
        private string sqlPreferenciaProyectoModify;

        #endregion

        #region ProyectoEvento

        private string sqlProyectoEventoInsert;
        private string sqlProyectoEventoDelete;
        private string sqlProyectoEventoModify;

        #endregion

        #region ProyectoEventoParticipante

        private string sqlProyectoEventoParticipanteInsert;
        private string sqlProyectoEventoParticipanteDelete;
        private string sqlProyectoEventoParticipanteModify;

        #endregion

        #region ProyectoEventoAccion

        private string sqlProyectoEventoAccionInsert;
        private string sqlProyectoEventoAccionDelete;
        private string sqlProyectoEventoAccionModify;

        #endregion

        #region ProyectoPasoRegistro

        private string sqlProyectoPasoRegistroInsert;
        private string sqlProyectoPasoRegistroDelete;
        private string sqlProyectoPasoRegistroModify;

        #endregion

        #region AccionesExternasProyecto

        private string sqlAccionesExternasProyectoInsert;
        private string sqlAccionesExternasProyectoDelete;
        private string sqlAccionesExternasProyectoModify;

        #endregion

        #region ProyectoSearchPersonalizado

        private string sqlProyectoSearchPersonalizadoInsert;
        private string sqlProyectoSearchPersonalizadoDelete;
        private string sqlProyectoSearchPersonalizadoModify;

        #endregion

        #region ProyectoConfigExtraSem
        private string sqlProyectoConfigExtraSemInsert;
        private string sqlProyectoConfigExtraSemDelete;
        private string sqlProyectoConfigExtraSemModify;
        #endregion

        #region ProyectoServicioExterno
        private string sqlProyectoServicioExternoInsert;
        private string sqlProyectoServicioExternoDelete;
        private string sqlProyectoServicioExternoModify;
        #endregion

        #region EcosistemaServicioExterno
        private string sqlEcosistemaServicioExternoInsert;
        private string sqlEcosistemaServicioExternoDelete;
        private string sqlEcosistemaServicioExternoModify;
        #endregion

        #region OntologiaProyecto
        private string sqlOntologiaProyectoInsert;
        private string sqlOntologiaProyectoDelete;
        private string sqlOntologiaProyectoModify;
        #endregion

        #region ConfigAutocompletarProy

        private string sqlConfigAutocompletarProyInsert;
        private string sqlConfigAutocompletarProyDelete;
        private string sqlConfigAutocompletarProyModify;

        #endregion ConfigAutocompletarProy

        #endregion

        //#region Métodos generales

        #region Públicos

        /// <summary>
        /// Obtiene la fecha de alta del grupo de organización en un proyecto
        /// </summary>
        /// <param name="pGrupoID">Identificador del grupo de organización</param>
        /// <param name="pOrganizacionID">Identificador de la organización del proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>DateTime con la fecha de alta del grupo en el proyecto. Null en caso de no encontrarlo</returns>
        public DateTime? ObtenerFechaAltaGrupoOrganizacionEnProyecto(Guid pGrupoID, Guid pOrganizacionID, Guid pProyectoID)
        {
            DateTime? fechaAlta = null;

            GrupoOrgParticipaProy filaGrupoProyecto = mEntityContext.GrupoOrgParticipaProy.Find(pGrupoID, pOrganizacionID, pProyectoID);

            if (filaGrupoProyecto != null)
            {
                fechaAlta = filaGrupoProyecto.FechaAlta;
            }

            return fechaAlta;
        }

        /// <summary>
        /// Obtiene los proyectos en los que participa un determinado grupo de organización
        /// </summary>
        /// <param name="pGrupoID">Identificador del grupo de organización</param>
        /// <returns>Lista de filas de GrupoOrgParticipaProy con los proyectos del grupo</returns>
        public List<GrupoOrgParticipaProy> ObtenerProyectosParticipaGrupoOrganizacion(Guid pGrupoID)
        {
            List<GrupoOrgParticipaProy> filasProyectosGrupo = null;
            
            filasProyectosGrupo = mEntityContext.GrupoOrgParticipaProy.Where(fila => fila.GrupoID == pGrupoID).ToList();

            return filasProyectosGrupo;
        }

        /// <summary>
        /// Obtiene los proyectos en los que participa un determinado grupo de organización
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización del proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Lista de filas de GrupoOrgParticipaProy con los grupos del proyecto</returns>
        public List<GrupoOrgParticipaProy> ObtenerGruposOrganizacionParticipanProyecto(Guid pOrganizacionID, Guid pProyectoID)
        {
            List<GrupoOrgParticipaProy> filasGruposProyecto = null;

            filasGruposProyecto = mEntityContext.GrupoOrgParticipaProy.Where(fila => fila.OrganizacionID == pOrganizacionID && fila.ProyectoID == pProyectoID).ToList();

            return filasGruposProyecto;
        }

        /// <summary>
        /// Elimina la fila de GrupoOrgParticipaProy.
        /// </summary>
        /// <param name="pGrupoID">Identificador del grupo de organización</param>
        /// <param name="pOrganizacionID">Identificador de la organización del proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        public void BorrarFilaGrupoOrgParticipaProy(Guid pGrupoID, Guid pOrganizacionID, Guid pProyectoID)
        {
            GrupoOrgParticipaProy filaGrupo = new GrupoOrgParticipaProy { GrupoID = pGrupoID, OrganizacionID = pOrganizacionID, ProyectoID = pProyectoID };
            mEntityContext.Entry(filaGrupo).State = EntityState.Deleted;
            ActualizarBaseDeDatosEntityContext();
        }

        /// <summary>
        /// Crea una fila de GrupoOrgParticipaProy.
        /// </summary>
        /// <param name="pGrupoID">Identificador del grupo de organización</param>
        /// <param name="pOrganizacionID">Identificador de la organización del proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pTipoIdentidad">Tipo de perfil con el que participan los miembros del grupo</param>
        public void AddFilaGrupoOrgParticipaProy(Guid pGrupoID, Guid pOrganizacionID, Guid pProyectoID, TiposIdentidad pTipoIdentidad)
        {
            GrupoOrgParticipaProy filaGrupo = new GrupoOrgParticipaProy { GrupoID = pGrupoID, OrganizacionID = pOrganizacionID, ProyectoID = pProyectoID, FechaAlta = DateTime.Now, TipoPerfil = (short)pTipoIdentidad };
            mEntityContext.GrupoOrgParticipaProy.Add(filaGrupo);
            ActualizarBaseDeDatosEntityContext();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="DataSet"></param>
        /// <param name="orden"></param>
        /// <param name="IDProy"></param>
        /// <returns></returns>
        public string ObtieneDescripciondeNivelCertificacion(string pOrden, Guid pProyectoID)
        {
            return mEntityContext.NivelCertificacion.Where(item => item.Orden.ToString().Equals(pOrden) && item.ProyectoID.Equals(pProyectoID)).Select(item => item.Descripcion).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene los permisos de páginas de los usuarios de un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Lista de filas de PermisosPaginasUsuarios</returns>
        public List<PermisosPaginasUsuarios> ObtenerPermisosPaginasProyecto(Guid pOrganizacionID, Guid pProyectoID)
        {
            List<PermisosPaginasUsuarios> filasPermisos = null;

            filasPermisos = mEntityContext.PermisosPaginasUsuarios.Where(fila => fila.OrganizacionID == pOrganizacionID && fila.ProyectoID == pProyectoID).ToList();

            return filasPermisos;
        }

        /// <summary>
        /// Obtiene los permisos de páginas del usuario en un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <returns>Lista de filas de PermisosPaginasUsuarios del usuario en el proyecto</returns>
        public List<PermisosPaginasUsuarios> ObtenerPermisosPaginasProyectoUsuarioID(Guid pOrganizacionID, Guid pProyectoID, Guid pUsuarioID)
        {
            List<PermisosPaginasUsuarios> filasPermisos = null;

            filasPermisos = mEntityContext.PermisosPaginasUsuarios.Where(fila => fila.OrganizacionID == pOrganizacionID && fila.ProyectoID == pProyectoID && fila.UsuarioID == pUsuarioID).ToList();

            return filasPermisos;
        }

        /// <summary>
        /// Obtiene los permisos de páginas del usuario en un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <param name="pTipoPagina">Página sobre la que se comprueba el permiso</param>
        /// <returns>True si el usuario tiene permiso sobre el tipo de página en el proyecto</returns>
        public bool TienePermisoUsuarioEnPagina(Guid pOrganizacionID, Guid pProyectoID, Guid pUsuarioID, TipoPaginaAdministracion pTipoPagina)
        {
            PermisosPaginasUsuarios filaPermisos = null;

            filaPermisos = mEntityContext.PermisosPaginasUsuarios.Find(pUsuarioID, pOrganizacionID, pProyectoID, (short)pTipoPagina);

            return filaPermisos != null;
        }

        /// <summary>
        /// Obtiene una RedireccionRegistroRuta por su id y sus RedireccionValorParametro asociadas 
        /// </summary>
        /// <param name="pDominio"></param>
        /// <returns></returns>
        public RedireccionRegistroRuta ObtenerRedireccionRegistroRutaPorRedireccionID(Guid pRedireccionID)
        {
            RedireccionRegistroRuta filaRedireccion = null;

            filaRedireccion = mEntityContext.RedireccionRegistroRuta.Include("RedireccionValorParametro").FirstOrDefault(r => r.RedireccionID == pRedireccionID);

            return filaRedireccion;
        }
        /// <summary>
        /// Obtiene una lista con registros de un proyecto
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <returns>Lista con los registros de un proyecto</returns>
        public List<string> ObtenerProyectoPasoRegistro(Guid pProyectoID)
        {
            List<string> listaRegistros = new List<string>();
            listaRegistros = mEntityContext.ProyectoPasoRegistro.Where(proy => proy.ProyectoID.Equals(pProyectoID)).Select(proy => proy.PasoRegistro).ToList();
            return listaRegistros;
        }
        /// <summary>
        /// Obtiene una lista con las pestanyas para los pasos del registro
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <returns>Lista con las Pestanyas del proyecto</returns>
        public List<ProyectoPestanyaMenu> ListaPestanyasMenuRegistro(Guid pProyectoID)
        {
            List<ProyectoPestanyaMenu> listaPestanyas = new List<ProyectoPestanyaMenu>();
            listaPestanyas = mEntityContext.ProyectoPestanyaMenu.Where(item => item.ProyectoID.Equals(pProyectoID) && item.TipoPestanya.Equals((short)TipoPestanyaMenu.CMS)).ToList();
            return listaPestanyas;
        }
        /// <summary>
        /// Obtiene una lista con la obligatoriedad de los registros
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <returns>Lista con los la obligatoriedad de los registros</returns>
        public List<bool> ObtenerListaObligatoriedadRegistros(Guid pProyectoID)
        {
            List<bool> listaBooleanos = new List<bool>();
            listaBooleanos = mEntityContext.ProyectoPasoRegistro.Where(proy => proy.ProyectoID.Equals(pProyectoID)).Select(proy => proy.Obligatorio).ToList();
            return listaBooleanos;
        }
        /// <summary>
        /// Actualiza la tabla PreferenciaProyecto con la preferencia de los proyectos seleccionados
        /// </summary>
        /// <param name="listaCategoriasSeleccionadas">Lista de IDs de las categorias seleccionadas</param>
        /// <param name="pProyectoID">ID del proyecto seleccionado</param>
        public void ActualizarTablaPreferenciaProyecto(List<Guid> listaCategoriasSeleccionadas, Guid pProyectoID)
        {
            List<PreferenciaProyecto> yaSeleccionadas = mEntityContext.PreferenciaProyecto.Where(proy => proy.ProyectoID.Equals(pProyectoID)).ToList();
            foreach (PreferenciaProyecto pref in yaSeleccionadas)
            {
                mEntityContext.Entry(pref).State = EntityState.Deleted;
            }
            foreach (Guid id in listaCategoriasSeleccionadas)
            {
                ProyectoAD proyAD = new ProyectoAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                AD.EntityModel.Models.Tesauro.CategoriaTesauro cateTesauro = mEntityContext.CategoriaTesauro.Where(proy => proy.CategoriaTesauroID.Equals(id)).FirstOrDefault();
                PreferenciaProyecto pref = new PreferenciaProyecto();
                pref.TesauroID = cateTesauro.TesauroID;
                pref.CategoriaTesauroID = id;
                pref.OrganizacionID = proyAD.ObtenerOrganizacionIDAPartirDeProyectoID(pProyectoID);
                pref.ProyectoID = pProyectoID;
                pref.Orden = cateTesauro.Orden;
                mEntityContext.PreferenciaProyecto.Add(pref);
            }
            ActualizarBaseDeDatosEntityContext();
        }
        /// <summary>
        /// Devuelve los IDs de las categorias seleccionadas
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <returns>Lista de los IDs de las categorias seleccionadas</returns>
        public List<Guid> ObtenerCategoriasSeleccionadas(Guid pProyectoID)
        {
            return mEntityContext.PreferenciaProyecto.Where(proy => proy.ProyectoID.Equals(pProyectoID)).Select(proy => proy.CategoriaTesauroID).ToList();
        }
        /// <summary>
        /// Guarda en la base de datos los registros por pasos en la tabla ProyectoPasoRegistro
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <param name="listaPasos">Lista con los registros a guardar</param>
        public void GuardarRegistroPorPasos(Guid pProyectoID, List<PasoRegistroModel> listaPasos)
        {
            List<ProyectoPasoRegistro> listaPPR = mEntityContext.ProyectoPasoRegistro.Where(proy => proy.ProyectoID.Equals(pProyectoID)).ToList();
            int numListaPPR = listaPPR.Count;
            int i = 0;
            if (numListaPPR >= 1)
            {
                foreach (ProyectoPasoRegistro proy in listaPPR)
                {
                    mEntityContext.EliminarElemento(proy);
                }
                foreach (PasoRegistroModel registro in listaPasos)
                {
                    if (!registro.Deleted)
                    {
                        ProyectoPasoRegistro pPasoRegistro = new ProyectoPasoRegistro();
                        pPasoRegistro.OrganizacionID = MetaOrganizacion;
                        pPasoRegistro.ProyectoID = pProyectoID;
                        pPasoRegistro.Orden = (short)i;
                        pPasoRegistro.PasoRegistro = registro.NombrePasoRegistro;
                        pPasoRegistro.Obligatorio = registro.Obligatorio;
                        mEntityContext.ProyectoPasoRegistro.Add(pPasoRegistro);
                        i++;
                    }
                }
            }
            else
            {
                foreach (PasoRegistroModel registro in listaPasos)
                {
                    if (!registro.Deleted)
                    {
                        ProyectoPasoRegistro pPasoRegistro = new ProyectoPasoRegistro();
                        pPasoRegistro.OrganizacionID = MetaOrganizacion;
                        pPasoRegistro.ProyectoID = pProyectoID;
                        pPasoRegistro.Orden = (short)i;
                        pPasoRegistro.PasoRegistro = registro.NombrePasoRegistro;
                        pPasoRegistro.Obligatorio = registro.Obligatorio;
                        mEntityContext.ProyectoPasoRegistro.Add(pPasoRegistro);
                        i++;
                    }
                }
            }
            ActualizarBaseDeDatosEntityContext();
        }

        /// <summary>
        /// Obtiene las redirecciones de un dominio
        /// </summary>
        /// <param name="pDominio"></param>
        /// <returns></returns>
        public List<RedireccionRegistroRuta> ObtenerRedireccionRegistroRutaPorDominio(string pDominio, bool pMantenerContextoAbierto)
        {
            mLoggingService.AgregarEntrada("ObtenerRedireccionRegistroRutaPorDominio - INICIO");

            List<RedireccionRegistroRuta> filasRedirecciones = null;

            if (pMantenerContextoAbierto)
            {
                filasRedirecciones = mEntityContext.RedireccionRegistroRuta.Where(fila => fila.Dominio == pDominio).OrderByDescending(r => r.FechaCreacion).ToList();
            }
            else
            {
                filasRedirecciones = mEntityContext.RedireccionRegistroRuta.Include("RedireccionValorParametro").Where(r => r.Dominio == pDominio).OrderByDescending(r => r.FechaCreacion).ToList();

            }

            mLoggingService.AgregarEntrada("ObtenerRedireccionRegistroRutaPorDominio - FIN");

            return filasRedirecciones;
        }

        /// <summary>
        /// Crea/Modifica una lista de filas de RedireccionRegistroRuta y sus filas de RedireccionValorParametro asociadas.
        /// </summary>
        /// <param name="pDicFilasRedireccion"></param>
        /// <param name="pRetrasarGuardado">Indica si hay que guardar los cambios inmediatamente. Pasarlo a TRUE si no se mantiene el mismo contexto para todas las operaciones</param>
        public void GuardarFilaRedireccionRegistroRuta(Dictionary<RedireccionRegistroRuta, bool> pDicFilasRedireccion, bool pRetrasarGuardado)
        {
            #region Viejo
            //RedireccionRegistroRuta filaRedireccion = new RedireccionRegistroRuta { RedireccionID = pRedireccionID, UrlOrigen = pUrlOrigen, Dominio = pDominio, NombreParametro = pNombreParametro, RedireccionValorParametro = null };

            //List<RedireccionValorParametro> listaFilasValores = new List<RedireccionValorParametro>();
            //foreach(string valorParam in pValoresRedirecciones.Keys)
            //{
            //    RedireccionValorParametro filaValorParametro = new RedireccionValorParametro { RedireccionID = pRedireccionID, ValorParametro = valorParam, UrlRedireccion = pValoresRedirecciones[valorParam], MantenerFiltros = pMantenerFiltros, RedireccionRegistroRuta = filaRedireccion };
            //    listaFilasValores.Add(filaValorParametro);
            //}

            //filaRedireccion.RedireccionValorParametro = listaFilasValores;

            //mEntityContext.RedireccionRegistroRuta.Add(filaRedireccion);

            //foreach (RedireccionValorParametro fila in listaFilasValores)
            //{
            //    if (!mEntityContext.RedireccionValorParametro.Contains(listaFilasValores[0]))
            //    {
            //        mEntityContext.RedireccionValorParametro.AddRange(listaFilasValores);
            //    }
            //}
            #endregion

            foreach (KeyValuePair<RedireccionRegistroRuta, bool> redireccion in pDicFilasRedireccion)
            {
                if (redireccion.Value) //es nueva
                {
                    mEntityContext.RedireccionRegistroRuta.Add(redireccion.Key);
                }
                else
                {
                    var entidadRedireccion = mEntityContext.RedireccionRegistroRuta.First(item => item.RedireccionID.Equals(redireccion.Key.RedireccionID));
                    entidadRedireccion.NombreParametro = redireccion.Key.NombreParametro;
                    //entidadRedireccion.RedireccionValorParametro = redireccion.Key.RedireccionValorParametro;
                    entidadRedireccion.UrlOrigen = redireccion.Key.UrlOrigen;
                    if (redireccion.Key.RedireccionValorParametro != null)
                    {
                        
                        foreach (RedireccionValorParametro filaValor in redireccion.Key.RedireccionValorParametro.ToList())
                        {
                            var redireccionValorParametro = mEntityContext.RedireccionValorParametro.First(item => item.RedireccionID.Equals(filaValor.RedireccionID) && item.ValorParametro.Equals(filaValor.ValorParametro));
                            redireccionValorParametro.UrlRedireccion = filaValor.UrlRedireccion;
                            redireccionValorParametro.RedireccionRegistroRuta = filaValor.RedireccionRegistroRuta;
                        }
                    }
                }
            }

            if (!pRetrasarGuardado)
            {
                ActualizarBaseDeDatosEntityContext();
            }
        }

        public List<IntegracionContinuaPropiedad> ObtenerFilasIntegracionContinuaParametro(Guid pProyectoID)
        {
            List<IntegracionContinuaPropiedad> listaPropiedades = mEntityContext.IntegracionContinuaPropiedad.Where(propiedad => propiedad.ProyectoID.Equals(pProyectoID)).ToList();

            return listaPropiedades;
        }

        public void BorrarConfiguracionSemanticaExtraDeProyecto(Guid pClave, string pNombreOntologia)
        {
            //DbCommand comandodEL = ObtenerComando($"DELETE FROM ProyectoConfigExtraSem WHERE ProyectoID={IBD.ToParam("ProyectoID")} and UrlOntologia={IBD.ToParam("NombreOntologia")}");
            //AgregarParametro(comandodEL, IBD.ToParam("ProyectoID"), DbType.Guid, pClave);
            //AgregarParametro(comandodEL, IBD.ToParam("NombreOntologia"), DbType.String, pNombreOntologia);
            //ActualizarBaseDeDatos(comandodEL);
            //Haria falta saber el parametro SourceTesSem...

            List<ProyectoConfigExtraSem> listaBorrar = mEntityContext.ProyectoConfigExtraSem.Where(proyConfig => proyConfig.ProyectoID.Equals(pClave) && proyConfig.UrlOntologia.Equals(pNombreOntologia)).ToList();
            foreach (ProyectoConfigExtraSem proyBorrar in listaBorrar)
            {
                mEntityContext.ProyectoConfigExtraSem.Remove(proyBorrar);
            }
            mEntityContext.SaveChanges();

        }
        /// <summary>
        /// Crea una nueva carga masiva en la base de datos
        /// </summary>
        /// <param name="idCarga">Id de la carga</param>
        /// <param name="estado">Estado de la carga</param>
        /// <param name="fechaAlta">Fecha de creación de la carga</param>
        /// <param name="proyectoId">Id el proyecto al que pertenece la carga</param>
        /// <param name="identidadId">Id de la identidad del sujeto de la carga</param>
        /// <param name="nombre">Nombre de la carga</param>
        /// <param name="organizacionId">Id de la organizacion de la carga</param>
        /// <returns>Devuelve cierto si se ha añadido una nueva carga</returns>
        public bool CrearNuevaCargaMasiva(Guid idCarga, int estado, DateTime fechaAlta, Guid proyectoId, Guid identidadId, string nombre = null, Guid? organizacionId = null)
        {
            bool creada = false;
            try
            {
                bool existe = mEntityContext.Carga.Any(item => item.CargaID.Equals(idCarga));
                if (!existe)
                {
                    Carga nuevaCarga = new Carga();
                    nuevaCarga.CargaID = idCarga;
                    nuevaCarga.Nombre = nombre;
                    nuevaCarga.Estado = (short)estado;
                    nuevaCarga.FechaAlta = fechaAlta;
                    nuevaCarga.ProyectoID = proyectoId;
                    nuevaCarga.OrganizacionID = organizacionId;
                    nuevaCarga.IdentidadID = identidadId;

                    mEntityContext.Carga.Add(nuevaCarga);
                    mEntityContext.SaveChanges();
                    creada = true;
                }
                return creada;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<string> ObtenerPropiedadesSearch(Guid pProyectoID)
        {
            List<string> listaPropiedades = new List<string>();

            List<ConfigSearchProy> listaConfigSearchProy = mEntityContext.ConfigSearchProy.Where(item => item.ProyectoID.Equals(pProyectoID)).ToList();
            List<ConfigAutocompletarProy> listaConfigAutocompletarProy = mEntityContext.ConfigAutocompletarProy.Where(item => item.ProyectoID.Equals(pProyectoID)).ToList();

            foreach (ConfigSearchProy configSearchProy in listaConfigSearchProy)
            {
                listaPropiedades = listaPropiedades.Union(configSearchProy.Valor.Split('|').ToList()).ToList();
            }
            foreach (ConfigAutocompletarProy configAutocompletarProy in listaConfigAutocompletarProy)
            {
                listaPropiedades = listaPropiedades.Union(configAutocompletarProy.Valor.Split('|').ToList()).ToList();
            }

            return listaPropiedades;
        }

        /// <summary>
        /// Devuelve la lista de cargas de una identidadID
        /// </summary>
        /// <param name="identidadId">Id de la identidad de la carga</param>
        /// <returns>Lista de cargas correspondientes a la identidad</returns>
        public List<Carga> ObtenerCargasMasivasPorIdentidadID(Guid identidadId)
        {
            List<Carga> listaCargas = new List<Carga>();
            listaCargas = mEntityContext.Carga.Where(x => x.IdentidadID.Value.Equals(identidadId)).OrderByDescending(x => x.FechaAlta).ToList();
            return listaCargas;
        }

        /// <summary>
        /// Devuelve la lista de paquetes de una carga
        /// </summary>
        /// <param name="cargaID">Id de la carga</param>
        /// <returns>Lista de los paquetes de la carga</returns>
        public List<CargaPaquete> ObtenerPaquetesPorIDCarga(Guid cargaID)
        {
            List<CargaPaquete> listaPaquetes = new List<CargaPaquete>();
            listaPaquetes = mEntityContext.CargaPaquete.Where(x => x.CargaID.Equals(cargaID)).ToList();
            return listaPaquetes;
        }

        /// <summary>
        /// Crea un nuevo paquete asociado a una carga
        /// </summary>
        /// <param name="paqueteID">Id del paquete</param>
        /// <param name="cargaId">Id de la carga</param>
        /// <param name="rutaOnto">Ruta del archivo de triples de la ontologia</param>
        /// <param name="rutaSearch">Ruta del archivo de triples busqueda</param>
        /// <param name="rutaSql">Ruta del archivo del sql</param>
        /// <param name="estado">Estado del paquete</param>
        /// <param name="error">Error del paquete</param>
        /// <param name="fechaAlta">Fecha en la que se crea el paquete</param>
        /// <param name="ontologia">Ontologia a la que pertenece el paquete</param>
        /// <param name="comprimido">Los archivos estan comprimidos</param>
        /// <param name="fechaProcesado">Fecha en la que se ha procesado el paquete</param>
        /// <returns>Devuelve cierto si se ha creado el paquete</returns>
        public bool CrearNuevoPaqueteCargaMasiva(Guid paqueteID, Guid cargaId, string rutaOnto, string rutaSearch, string rutaSql, int estado, string error, DateTime? fechaAlta, string ontologia, bool comprimido = false, DateTime? fechaProcesado = null)
        {
            bool creado = false;
            try
            {
                bool existe = mEntityContext.CargaPaquete.Any(item => item.PaqueteID.Equals(paqueteID));
                if (!existe)
                {
                    CargaPaquete nuevoPaquete = new CargaPaquete();
                    nuevoPaquete.PaqueteID = paqueteID;
                    nuevoPaquete.CargaID = cargaId;
                    nuevoPaquete.RutaOnto = rutaOnto;
                    nuevoPaquete.RutaBusqueda = rutaSearch;
                    nuevoPaquete.RutaSQL = rutaSql;
                    nuevoPaquete.Estado = (short)estado;
                    nuevoPaquete.Error = string.Empty;
                    nuevoPaquete.FechaAlta = fechaAlta;
                    nuevoPaquete.FechaProcesado = fechaProcesado;
                    nuevoPaquete.Ontologia = ontologia;
                    nuevoPaquete.Comprimido = comprimido;

                    mEntityContext.CargaPaquete.Add(nuevoPaquete);
                    mEntityContext.SaveChanges();
                    creado = true;
                }
                return creado;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void CrearFilasIntegracionContinuaParametro(List<IntegracionContinuaPropiedad> pListaPropiedades, Guid pProyectoID, TipoObjeto pTipoObjeto, string pID)
        {
            try
            {
                //List<IntegracionContinuaPropiedad> propiedadesAnteriores = mEntityContext.IntegracionContinuaPropiedad.Where(propiedad => propiedad.ProyectoID == pProyectoID && !propiedad.TipoObjeto.Equals((short)TipoObjeto.Componente).Equals(pEsGuardadoCMS)).ToList();

                List<IntegracionContinuaPropiedad> propiedadesAnteriores;
                if (string.IsNullOrEmpty(pID))
                {
                    propiedadesAnteriores = mEntityContext.IntegracionContinuaPropiedad.Where(propiedad => propiedad.ProyectoID.Equals(pProyectoID) && propiedad.TipoObjeto.Equals((short)pTipoObjeto)).ToList();
                }
                else
                {
                    propiedadesAnteriores = mEntityContext.IntegracionContinuaPropiedad.Where(propiedad => propiedad.ProyectoID.Equals(pProyectoID) && propiedad.TipoObjeto.Equals((short)pTipoObjeto) && propiedad.ObjetoPropiedad.Equals(pID)).ToList();
                }
                //bool transaccionIniciada = false;
                bool transaccionIniciada = IniciarTransaccionEntityContext();
                try
                {
                    foreach (IntegracionContinuaPropiedad propiedad in propiedadesAnteriores)
                    {
                        if (!pListaPropiedades.Exists(propiedadfind => propiedadfind.TipoObjeto == propiedad.TipoObjeto && propiedadfind.ObjetoPropiedad == propiedad.ObjetoPropiedad && propiedadfind.TipoObjeto == propiedad.TipoObjeto && propiedadfind.ValorPropiedad == propiedad.ValorPropiedad))
                        {
                            mEntityContext.IntegracionContinuaPropiedad.Remove(propiedad);
                        }
                    }

                    ActualizarBaseDeDatosEntityContext();
                    foreach (IntegracionContinuaPropiedad propiedad in pListaPropiedades)
                    {
                        if (!propiedadesAnteriores.Exists(propiedadfind => propiedadfind.TipoObjeto == propiedad.TipoObjeto && propiedadfind.ObjetoPropiedad == propiedad.ObjetoPropiedad && propiedadfind.TipoObjeto == propiedad.TipoObjeto && propiedadfind.ValorPropiedad == propiedad.ValorPropiedad))
                        {
                            mEntityContext.IntegracionContinuaPropiedad.Add(propiedad);
                        }
                    }

                    ActualizarBaseDeDatosEntityContext();

                    if (transaccionIniciada)
                    {
                        TerminarTransaccion(true);
                    }
                }
                catch
                {
                    TerminarTransaccion(false);
                    throw;
                }
            }
            catch
            {

            }
        }

        public void GuardarFilasIntegracionContinuaParametro(List<IntegracionContinuaPropiedad> pListaPropiedades, Guid pProyectoID)
        {
            //Propiedades del entorno actual
            List<IntegracionContinuaPropiedad> propiedadesAnteriores = mEntityContext.IntegracionContinuaPropiedad.Where(propiedad => propiedad.ProyectoID == pProyectoID).ToList();

            foreach (IntegracionContinuaPropiedad propiedad in propiedadesAnteriores)
            {
                if (!pListaPropiedades.Exists(propiedadfind => propiedadfind.TipoObjeto == propiedad.TipoObjeto && propiedadfind.ObjetoPropiedad == propiedad.ObjetoPropiedad && propiedadfind.TipoObjeto == propiedad.TipoObjeto && propiedadfind.ValorPropiedad == propiedad.ValorPropiedad))
                {
                    mEntityContext.IntegracionContinuaPropiedad.Remove(propiedad);
                }
            }

            foreach (IntegracionContinuaPropiedad propiedad in pListaPropiedades)
            {
                IntegracionContinuaPropiedad propiedadAnterior = propiedadesAnteriores.FirstOrDefault(propiedadfind => propiedadfind.TipoObjeto == propiedad.TipoObjeto && propiedadfind.ObjetoPropiedad == propiedad.ObjetoPropiedad && propiedadfind.TipoObjeto == propiedad.TipoObjeto && propiedadfind.ValorPropiedad == propiedad.ValorPropiedad);

                if (propiedadAnterior == null)
                {
                    propiedadAnterior = propiedad;
                    propiedadAnterior.ProyectoID = pProyectoID;
                    mEntityContext.IntegracionContinuaPropiedad.Add(propiedadAnterior);
                }

                propiedadAnterior.ValorPropiedadDestino = propiedad.ValorPropiedadDestino;
                propiedadAnterior.MismoValor = propiedad.MismoValor;
                propiedadAnterior.Revisada = propiedad.Revisada;
            }

            ActualizarBaseDeDatosEntityContext();
        }

        /// <summary>
        /// Elimina la fila de RedireccionRegistroRuta y sus filas de RedireccionValorParametro asociadas.
        /// </summary>
        /// <param name="pListaRedireccionesID">Lista de identificadores de redirección</param>
        /// <param name="pRetrasarGuardado">Indica si hay que guardar los cambios inmediatamente. Pasarlo a TRUE si no se mantiene el mismo contexto para todas las operaciones</param>
        public void BorrarFilaRedireccionRegistroRuta(List<Guid> pListaRedireccionesID, bool pRetrasarGuardado)
        {
            //RedireccionRegistroRuta filaRedireccion = new RedireccionRegistroRuta { RedireccionID = pRedireccionID };
            //mEntityContext.RedireccionValorParametro.RemoveRange(filaRedireccion.RedireccionValorParametro);
            //mEntityContext.Entry(filaRedireccion).State = System.Data.Entity.EntityState.Deleted;

            //RedireccionRegistroRuta filaRedireccion2 = new RedireccionRegistroRuta { RedireccionID = pRedireccionID };
            //foreach(RedireccionValorParametro filaValor in mEntityContext.RedireccionValorParametro.Where(r => r.RedireccionID == pRedireccionID))
            //{
            //    mEntityContext.Entry(filaValor).State = System.Data.Entity.EntityState.Deleted;
            //}          
            //mEntityContext.Entry(filaRedireccion).State = System.Data.Entity.EntityState.Deleted;

            foreach (Guid redireccionID in pListaRedireccionesID)
            {
                RedireccionRegistroRuta filaRedireccion = mEntityContext.RedireccionRegistroRuta.Find(redireccionID);
                if (filaRedireccion != null)
                {
                    //mirar como aprovechar el borrado en cascada
                    mEntityContext.RedireccionValorParametro.RemoveRange(filaRedireccion.RedireccionValorParametro);
                    mEntityContext.RedireccionRegistroRuta.Remove(filaRedireccion);
                }
            }

            if (!pRetrasarGuardado)
            {
                ActualizarBaseDeDatosEntityContext();
            }
        }

        /// <summary>
        /// Elimina la fila de RedireccionRegistroRuta y sus filas de RedireccionValorParametro asociadas.
        /// </summary>
        /// <param name="pFilasValores">Diccionario de identificadores de redirección y lista de valores de parámetros</param>
        /// <param name="pRetrasarGuardado">Indica si hay que guardar los cambios inmediatamente. Pasarlo a TRUE si no se mantiene el mismo contexto para todas las operaciones</param>
        public void BorrarFilasRedireccionValorParametro(List<RedireccionValorParametro> pFilasValores, bool pRetrasarGuardado)
        {

            if (pFilasValores != null && pFilasValores.Count > 0)
            {
                mEntityContext.RedireccionValorParametro.RemoveRange(pFilasValores);
            }

            if (!pRetrasarGuardado)
            {
                ActualizarBaseDeDatosEntityContext();
            }
        }

        public string ObtenerExcepcionesMovil(Guid pProyectoID)
        {
            //DbCommand comdSelectProyectos = ObtenerComando(sql);
            //string excepciones = EjecutarEscalar(comdSelectProyectos) as string;
            string excepciones = mEntityContext.ParametroProyecto.Where(parametroProyecto => parametroProyecto.ProyectoID.Equals(pProyectoID) && parametroProyecto.Parametro.Equals("ExcepcionBusquedaMovil")).Select(parametroProyecto => parametroProyecto.Valor).FirstOrDefault();
            return excepciones;
        }

        /// <summary>
        /// Obtiene la lista de proyectos con acciones externas
        /// </summary>
        /// <returns>Lista de Identificadores de proyecto con acciones externas configuradas</returns>
        public List<Guid> ObtenerListaIDsProyectosConAccionesExternas()
        {
            //TODO
            List<Guid> listaProyectos = new List<Guid>();
            string sql = "";
            if (EsPostgres() || EsOracle())
            {
                sql = "SELECT Distinct \"ProyectoID\" FROM \"AccionesExternasProyecto\"";
            }
            else
            {
                sql = "SELECT Distinct ProyectoID FROM AccionesExternasProyecto";
            }


            DbCommand comdSelectProyectos = ObtenerComando(sql);
            IDataReader reader = EjecutarReader(comdSelectProyectos);
            while (reader.Read())
            {
                if (!listaProyectos.Contains(reader.GetGuid(0)))
                {
                    listaProyectos.Add(reader.GetGuid(0));
                }
            }
            reader.Close();
            reader.Dispose();

            return listaProyectos;
        }

        /// <summary>
        /// Carga los proyectos en estado (cerrandose)
        /// </summary>
        /// <returns>Dataset de proyectos</returns>
        public DataWrapperProyecto ObtenerProyectosCerrandose()
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            //Proyecto
            //DbCommand commandSQL = ObtenerComando(SelectProyectoPesado + " FROM Proyecto Where Estado = " + (short)EstadoProyecto.Cerrandose);
            List<Proyecto> listaProyectos = mEntityContext.Proyecto.Where(proyecto => proyecto.Estado.Equals((short)EstadoProyecto.Cerrandose)).ToList();
            dataWrapperProyecto.ListaProyecto = listaProyectos;

            //ProyectoCerrandose
            //DbCommand command2SQL = ObtenerComando(sqlSelectProyectoCerrandose + " FROM ProyectoCerrandose INNER JOIN Proyecto ON Proyecto.OrganizacionID = ProyectoCerrandose.OrganizacionID AND Proyecto.ProyectoID = ProyectoCerrandose.ProyectoID WHERE Estado = " + (short)EstadoProyecto.Cerrandose);
            List<ProyectoCerrandose> listaProyectoCerrandose = mEntityContext.ProyectoCerrandose.Join(mEntityContext.Proyecto, proyCerrandose => new { proyCerrandose.OrganizacionID, proyCerrandose.ProyectoID }, proyecto => new { proyecto.OrganizacionID, proyecto.ProyectoID }, (proyCerrandose, proyecto) => new
            {
                ProyectoCerrandose = proyCerrandose,
                Proyecto = proyecto
            }).Where(objeto => objeto.Proyecto.Estado.Equals((short)EstadoProyecto.Cerrandose)).Select(objeto => objeto.ProyectoCerrandose).ToList();
            dataWrapperProyecto.ListaProyectoCerrandose = listaProyectoCerrandose;

            //AdministradorProyecto
            //DbCommand command3SQL = ObtenerComando(sqlSelectAdministradoresProyecto + " INNER JOIN Proyecto ON AdministradorProyecto.OrganizacionID = Proyecto.OrganizacionID AND AdministradorProyecto.ProyectoID = Proyecto.ProyectoID WHERE Proyecto.Estado = " + (short)EstadoProyecto.Cerrandose + " AND AdministradorProyecto.Tipo = " + (short)TipoRolUsuario.Administrador);
            List<AdministradorProyecto> listaAdministradorProyecto = mEntityContext.AdministradorProyecto.Join(mEntityContext.Proyecto, adminProy => new { adminProy.OrganizacionID, adminProy.ProyectoID }, proyecto => new { proyecto.OrganizacionID, proyecto.ProyectoID }, (adminProy, proyecto) => new
            {
                AdministradorProyecto = adminProy,
                Proyecto = proyecto
            }).Where(objeto => objeto.Proyecto.Estado.Equals((short)EstadoProyecto.Cerrandose) && objeto.AdministradorProyecto.Tipo.Equals((short)TipoRolUsuario.Administrador)).Select(objeto => objeto.AdministradorProyecto).ToList();
            dataWrapperProyecto.ListaAdministradorProyecto = listaAdministradorProyecto;
            return dataWrapperProyecto;
        }

        /// <summary>
        /// Carga una lista con el identificador de todos los proyectos abiertos
        /// </summary>
        /// <returns>ProyectoDS</returns>
        public List<Guid> ObtenerTodosIDProyectosAbiertos()
        {
            return mEntityContext.Proyecto.Where(proyecto => proyecto.Estado.Equals((short)EstadoProyecto.Abierto)).Select(proyecto => proyecto.ProyectoID).ToList();
        }

        /// <summary>
        /// Comprueba si existen documentos que su nivel de certificación sea el pasado por parámetro
        /// </summary>
        /// <param name="pNivelCertificacionID">Identificador del nivel de certificación</param>
        /// <returns>TRUE si existe algún documento, FALSE en caso contrario</returns>
        public bool ExisteDocAsociadoANivelCertif(Guid pNivelCertificacionID)
        {
            Object encontrado;
            DbCommand commandsqlExisteDocAsociadoANivelCertif = ObtenerComando(sqlSelectExisteDocAsociadoANivelCertif);
            AgregarParametro(commandsqlExisteDocAsociadoANivelCertif, IBD.ToParam("pNivelCertificacionID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pNivelCertificacionID));
            encontrado = EjecutarEscalar(commandsqlExisteDocAsociadoANivelCertif);

            return (encontrado != null);
        }

        /// <summary>
        /// Comprueba si existen documentos que su nivel de certificación sea el pasado por parámetro
        /// </summary>
        /// <param name="pNivelesCertificacionID">Identificador de los niveles de certificación</param>
        /// <returns>Lista con los niveles de certificacion y un booleano que indica si tiene documentos</returns>
        public Dictionary<Guid, bool> ExisteDocAsociadoANivelCertif(List<Guid> pNivelesCertificacionID)
        {
            Dictionary<Guid, bool> resultado = new Dictionary<Guid, bool>();

            if (pNivelesCertificacionID.Count > 0)
            {
                var resultadoConsulta = mEntityContext.DocumentoWebVinBaseRecursos.Where(documentoWebVin => pNivelesCertificacionID.Contains((Guid)documentoWebVin.NivelCertificacionID)).Where(item => item.NivelCertificacionID.HasValue).GroupBy(documento => documento.NivelCertificacionID.Value).Select(item => new { NivelCertificacionID = item.Key, Documentos = item.Count() }).ToList();

                foreach (var fila in resultadoConsulta)
                {
                    Guid nivelCert = (Guid)fila.NivelCertificacionID;
                    int numDocsAsociados = fila.Documentos;

                    resultado.Add(nivelCert, numDocsAsociados > 0);
                }
            }

            return resultado;
        }

        /// <summary>
        /// Obtiene la URL propia de un proyecto
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización del proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>URL propia de proyecto</returns>
        public string ObtenerURLPropiaProyecto(Guid pProyectoID)
        {
            string resultado = null;

            //DbCommand selectUrlPropiaProyecto = ObtenerComando(sqlSelectUrlPropiaProyecto);
            //AgregarParametro(selectUrlPropiaProyecto, IBD.ToParam("proyectoID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pProyectoID));
            resultado = mEntityContext.Proyecto.Where(proyecto => proyecto.ProyectoID.Equals(pProyectoID)).Select(proyecto => proyecto.URLPropia).FirstOrDefault();
            return resultado;
        }

        /// <summary>
        /// Obtiene si el proyecto tienen un dominio con proyectros multiples
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns></returns>
        public int NumeroProyectosConMismosDominio(string pDominio)
        {
            int resultado = 0;

            //DbCommand selectUrlPropiaProyecto = ObtenerComando(" SELECT COUNT(*) FROM Proyecto WHERE Proyecto.URLPropia = '" + pDominio + "'");
            int num = mEntityContext.Proyecto.Where(proyecto => proyecto.URLPropia.Equals(pDominio)).ToList().Count;

            if (num > 0)
            {
                //Se trata de una web
                resultado += (int)num;
            }
            else
            {
                int numProy = mEntityContext.ConfiguracionServiciosProyecto.Where(conf => conf.Url.Equals(pDominio)).Select(conf => conf.ProyectoID).Distinct().ToList().Count;


                if (numProy > 0)
                {
                    //ConfiguracionServiciosProyecto
                    resultado += (int)numProy;
                }




                List<Guid> listaProyectoID = mEntityContext.ConfiguracionServiciosProyecto.Where(config => !config.Url.Equals(pDominio)).Select(config => config.ProyectoID).ToList();
                int numServ = mEntityContext.Proyecto.Where(proyecto => mEntityContext.ConfiguracionServiciosDominio.Any(config => config.Url.Equals(pDominio) && config.Dominio.Contains(proyecto.URLPropia)) && !listaProyectoID.Contains(proyecto.ProyectoID)).ToList().Count;

                if (numServ > 0)
                {
                    //ConfiguracionServiciosdominio
                    resultado += (int)numServ;
                }
                if (resultado > 0)
                {
                    return resultado;
                }
                else
                {

                    List<Guid> listaConfigProyectoID = mEntityContext.ConfiguracionServiciosProyecto.Select(proyecto => proyecto.ProyectoID).ToList();

                    List<Guid> listaProyectoProyectoID = mEntityContext.Proyecto.Where(proyecto => mEntityContext.ConfiguracionServiciosDominio.Any(config => config.Dominio.Contains(proyecto.URLPropia)) && !listaConfigProyectoID.Contains(proyecto.ProyectoID)).Select(proyecto => proyecto.ProyectoID).ToList();

                    List<Guid> listaGuidCompleta = mEntityContext.ConfiguracionServiciosProyecto.Select(config => config.ProyectoID).Union(listaProyectoProyectoID).ToList();

                    int numGen = mEntityContext.Proyecto.Where(proyecto => !listaGuidCompleta.Contains(proyecto.ProyectoID) && mEntityContext.ConfiguracionServicios.Any(conf => conf.Url.Equals(pDominio))).Select(proy => proy.ProyectoID).Distinct().ToList().Count;
                    if (numGen > 0)
                    {
                        //ConfiguracionServiciosGenerico
                        resultado = (int)numGen;
                    }
                }
            }
            return resultado;
        }

        /// <summary>
        /// Obtiene los proyectos por id del dominio
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns></returns>
        public List<Guid> ObtenerProyectosIdDeDominio(string pDominio)
        {
            return mEntityContext.Proyecto.Where(proyecto => proyecto.URLPropia.Equals(pDominio)).Select(proyecto => proyecto.ProyectoID).ToList();
        }

        public DataWrapperProyecto ObtenerProyectoLoginConfiguracion(Guid pProyectoID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();
            dataWrapperProyecto.ListaProyectoLoginConfiguracion = mEntityContext.ProyectoLoginConfiguracion.Where(proyecto => proyecto.ProyectoID.Equals(pProyectoID)).ToList();
            return dataWrapperProyecto;
        }

        /// <summary>
        /// Obtiene la URL propia de un proyecto
        /// </summary>
        /// <param name="pNombreCorto">Nombre corto del proyecto</param>
        /// <returns>URL propia de proyecto</returns>
        public string ObtenerURLPropiaProyectoPorNombreCorto(string pNombreCorto)
        {
            string resultado = null;

            string url = mEntityContext.Proyecto.Where(proyecto => proyecto.NombreCorto.Equals(pNombreCorto)).Select(proyecto => proyecto.URLPropia).FirstOrDefault();
            if (url != null)
            {
                resultado = url;
            }
            return resultado;
        }

        /// <summary>
        /// Obtiene las URLs propias de proyectos cuyos nombres cortos se pasan por parámetro
        /// </summary>
        /// <param name="pNombresCortos">Nombres cortos de los proyectos</param>
        /// <returns>URLS propias de los proyectos</returns>
        public Dictionary<string, string> ObtenerURLSPropiasProyectosPorNombresCortos(List<string> pNombresCortos)
        {
            Dictionary<string, string> resultado = new Dictionary<string, string>();

            bool demasiadosParametros = pNombresCortos.Count > 2000;

            if (pNombresCortos.Count > 0)
            {
                //string sqlSelectUrlPropiaProyectosPorNombresCortos = "SELECT DISTINCT NombreCorto,URLPropia FROM Proyecto WHERE Proyecto.URLPropia is not null AND NombreCorto in (";

                var resultadoConsulta = mEntityContext.Proyecto.Where(proyecto => proyecto.URLPropia != null && pNombresCortos.Contains(proyecto.NombreCorto)).Select(proyecto => new { proyecto.NombreCorto, proyecto.URLPropia }).Distinct().ToList();

                foreach (var fila in resultadoConsulta)
                {
                    if (fila.NombreCorto.ToString() != "" && fila.URLPropia.ToString() != "")
                    {
                        resultado.Add(fila.NombreCorto, fila.URLPropia);
                    }
                }
            }

            return resultado;
        }

        /// <summary>
        /// Obtiene el identificador de la organización de un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>identificador de la organización del proyecto</returns>
        public Guid ObtenerOrganizacionIDProyecto(Guid pProyectoID)
        {
            //DbCommand selectOrgIDProyecto = ObtenerComando(sqlSelectOrganizacionIDProyecto);
            //AgregarParametro(selectOrgIDProyecto, IBD.ToParam("proyectoID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pProyectoID));

            object idOrg = mEntityContext.Proyecto.Where(proyecto => proyecto.ProyectoID.Equals(pProyectoID)).Select(proyecto => proyecto.OrganizacionID).FirstOrDefault();

            if (idOrg is Guid)
            {
                return (Guid)idOrg;
            }
            return Guid.Empty;
        }

        /// <summary>
        /// Obtiene el nombre corto de un proyecto
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización del proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Nombre corto del proyecto</returns>
        public string ObtenerNombreCortoProyecto(Guid pProyectoID)
        {
            string resultado = null;
            //DbCommand selectNombreCortoProyecto = ObtenerComando(sqlSelectNombreCortoProyectoConSoloProyectoID);
            //AgregarParametro(selectNombreCortoProyecto, IBD.ToParam("proyectoID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pProyectoID));

            string nombreCorto = mEntityContext.Proyecto.Where(proyecto => proyecto.ProyectoID.Equals(pProyectoID)).Select(proyecto => proyecto.NombreCorto).FirstOrDefault();

            if (nombreCorto != null)
            {
                resultado = nombreCorto;
            }
            return resultado;
        }

        /// <summary>
        /// Obtiene los nombres cortos de los proyectos
        /// </summary>
        /// <param name="pProyectosID">Identificador de los proyectos</param>
        /// <returns>Nombres cortos de los proyectos</returns>
        public Dictionary<Guid, string> ObtenerNombresCortosProyectos(List<Guid> pProyectosID)
        {
            Dictionary<Guid, string> resultado = new Dictionary<Guid, string>();

            if (pProyectosID.Count > 0)
            {

                var resultadoConsulta = mEntityContext.Proyecto.Where(proyecto => pProyectosID.Contains(proyecto.ProyectoID)).Select(proyecto => new { proyecto.ProyectoID, proyecto.NombreCorto }).ToList();
                foreach (var linea in resultadoConsulta)
                {
                    Guid proyID = linea.ProyectoID;
                    string nombreCorto = linea.NombreCorto;

                    if (!resultado.ContainsKey(proyID))
                    {
                        resultado.Add(proyID, nombreCorto);
                    }
                }
            }

            return resultado;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pOrganizacionID"></param>
        /// <param name="pProyectoID"></param>
        /// <returns></returns>
        public Dictionary<string, Dictionary<Guid, string>> ObtenerNombresCortosProyectosConNombresCortosOntologias(Guid? pProyectoID, string pNombreCortoProyecto)
        {
            Dictionary<string, Dictionary<Guid, string>> resultado = new Dictionary<string, Dictionary<Guid, string>>();


            var resultadoConsulta = mEntityContext.Proyecto.Join(mEntityContext.OntologiaProyecto, proyecto => proyecto.ProyectoID, ontologiaProy => ontologiaProy.ProyectoID, (proyecto, ontologiaProy) => new
            {
                Proyecto = proyecto,
                OntologiaProyecto = ontologiaProy
            }).Join(mEntityContext.Documento, objeto => new { Enlace = objeto.OntologiaProyecto.OntologiaProyecto1 + ".owl", objeto.OntologiaProyecto.ProyectoID }, documento => new { documento.Enlace, ProyectoID = documento.ProyectoID.Value }, (objeto, documento) => new
            {
                Proyecto = objeto.Proyecto,
                OntologiaProyecto = objeto.OntologiaProyecto,
                Documento = documento
            }).Where(x => !string.IsNullOrEmpty(x.OntologiaProyecto.NombreCortoOnt) && x.Documento.Eliminado.Equals(false) && x.Documento.Tipo == 7).ToList();

            if (pProyectoID.HasValue)
            {
                resultadoConsulta = resultadoConsulta.Where(objeto => objeto.Proyecto.ProyectoID.Equals(pProyectoID.Value)).ToList();

            }
            else if (!string.IsNullOrEmpty(pNombreCortoProyecto))
            {
                resultadoConsulta = resultadoConsulta.Where(objeto => objeto.Proyecto.NombreCorto.Equals(pNombreCortoProyecto)).ToList();
            }

            var resultadoConsulta2 = resultadoConsulta.Select(objeto => new { objeto.Proyecto.NombreCorto, objeto.Documento.DocumentoID, objeto.OntologiaProyecto.NombreCortoOnt }).ToList();


            foreach (var fila in resultadoConsulta2)
            {
                string nombreCorto = fila.NombreCorto;
                Guid idOnt = fila.DocumentoID;
                string nombreOnt = fila.NombreCortoOnt;
                Dictionary<Guid, string> listaOntologias = new Dictionary<Guid, string>();
                if (!resultado.ContainsKey(nombreCorto))
                {
                    resultado.Add(nombreCorto, listaOntologias);
                }
                else
                {
                    listaOntologias = resultado[nombreCorto];
                }
                listaOntologias.Add(idOnt, nombreOnt);
            }

            return resultado;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pOrganizacionID"></param>
        /// <param name="pProyectoID"></param>
        /// <returns></returns>
        public Dictionary<string, Guid> ObtenerOntologiasConIDPorNombreCortoProy(string pNombreCortoProyecto)
        {
            Dictionary<string, Guid> resultado = new Dictionary<string, Guid>();

            var resultadoConsulta = mEntityContext.Documento.Join(mEntityContext.DocumentoWebVinBaseRecursos, documento => documento.DocumentoID, docWebVin => docWebVin.DocumentoID, (documento, docWebVin) => new
            {
                Documento = documento,
                DocumentoWebVinBaseRecursos = docWebVin
            }).Join(mEntityContext.BaseRecursosProyecto, objeto => objeto.DocumentoWebVinBaseRecursos.BaseRecursosID, baseRecursosProyecto => baseRecursosProyecto.BaseRecursosID, (objeto, baseRecursosProyecto) => new
            {
                Documento = objeto.Documento,
                DocumentoWebVinBaseRecursos = objeto.DocumentoWebVinBaseRecursos,
                BaseRecursosProyecto = baseRecursosProyecto
            }).Join(mEntityContext.Proyecto, objeto => objeto.BaseRecursosProyecto.ProyectoID, proyecto => proyecto.ProyectoID, (objeto, proyecto) => new
            {
                Documento = objeto.Documento,
                DocumentoWebVinBaseRecursos = objeto.DocumentoWebVinBaseRecursos,
                BaseRecursosProyecto = objeto.BaseRecursosProyecto,
                Proyecto = proyecto
            }).Where(objeto => objeto.Documento.Tipo.Equals((short)TiposDocumentacion.Ontologia) && objeto.Documento.Eliminado == false && objeto.DocumentoWebVinBaseRecursos.Eliminado == false && objeto.Proyecto.NombreCorto.Equals(pNombreCortoProyecto)).Select(objeto => new { objeto.Documento.DocumentoID, objeto.Documento.Enlace }).ToList();

            foreach (var fila in resultadoConsulta)
            {
                Guid idOnt = fila.DocumentoID;
                string nombreOnt = fila.Enlace;
                if (!resultado.ContainsKey(nombreOnt))
                {
                    resultado.Add(nombreOnt.ToLower(), idOnt);
                }
            }

            return resultado;
        }

        /// <summary>
        /// Obtiene las ontologías de los proyectos en los que participa un determinado perfil
        /// </summary>
        /// <param name="pPerfilID">Identificador del perfil</param>
        /// <returns>Dataset de proyecto con la tabla OntologiaProyecto cargada para el perfil</returns>
        public List<OntologiaProyecto> ObtenerOntologiasPorPerfilID(Guid pPerfilID)
        {
            List<OntologiaProyecto> listaOntologiaProyecto = new List<OntologiaProyecto>();
            //string sql = sqlSelectOntologiaProyecto.Replace("SELECT", "SELECT DISTINCT") + "inner join Identidad on OntologiaProyecto.ProyectoID = identidad.ProyectoID where identidad.PerfilID = " + IBD.GuidParamValor("PerfilID");
            //DbCommand cmd = ObtenerComando(sql);
            //AgregarParametro(cmd, IBD.ToParam("PerfilID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pPerfilID));
            listaOntologiaProyecto = mEntityContext.OntologiaProyecto.Join(mEntityContext.Identidad, ontologia => ontologia.ProyectoID, identidad => identidad.ProyectoID, (ontologia, identidad) => new
            {
                OntologiaProyecto = ontologia,
                PerfilID = identidad.PerfilID
            }).Where(objeto => objeto.PerfilID.Equals(pPerfilID)).Select(objeto => objeto.OntologiaProyecto).ToList();
            return listaOntologiaProyecto;
        }

        /// <summary>
        /// Obtiene el id autonumérico que se le asigna a cada proyecto para crear la tabla BASE
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns></returns>
        public int ObtenerTablaBaseProyectoIDProyectoPorID(Guid pProyectoID)
        {
            int resultado = -1;

            if (pProyectoID != Guid.Empty)
            {
                //"SELECT TablaBaseProyectoID FROM Proyecto WHERE ProyectoID = " + IBD.GuidParamValor("proyectoID");
                object id = mEntityContext.Proyecto.Where(proyecto => proyecto.ProyectoID.Equals(pProyectoID)).Select(proy => proy.TablaBaseProyectoID).FirstOrDefault();

                if ((id is int) && (((int)id) > 0))
                {
                    resultado = (int)id;
                }
            }
            return resultado;
        }

        /// <summary>
        /// Obtiene el id autonumérico que se le asigna a cada proyecto para crear la tabla BASE
        /// </summary>
        /// <param name="pListaProyectosID">Identificadores de los proyectos</param>
        /// <returns></returns>
        public Dictionary<Guid, int> ObtenerTablasBaseProyectoIDProyectoPorID(List<Guid> pListaProyectosID)
        {
            Dictionary<Guid, int> diccionarioProyectoBaseProyectoID = new Dictionary<Guid, int>();

            if (pListaProyectosID.Count > 0)
            {
                var resultadoConsulta = mEntityContext.Proyecto.Where(proyecto => pListaProyectosID.Contains(proyecto.ProyectoID)).Select(proyecto => new { proyecto.ProyectoID, proyecto.TablaBaseProyectoID }).ToList();
                foreach (var fila in resultadoConsulta)
                {
                    Guid proyectoID = fila.ProyectoID;
                    int baseProyectoID = fila.TablaBaseProyectoID;
                    if (!diccionarioProyectoBaseProyectoID.ContainsKey(proyectoID))
                    {
                        diccionarioProyectoBaseProyectoID.Add(proyectoID, baseProyectoID);
                    }
                }
            }
            return diccionarioProyectoBaseProyectoID;
        }

        /// <summary>
        /// Obtiene el proyecto a partir del id autonumérico que se le asigna al crear la tabla BASE.
        /// </summary>
        /// <param name="pTablaBaseProyectoID">Identificador de la tabla base del proyecto</param>
        /// <returns>DataSet del proyecto con el proyecto cargado</returns>
        public List<Proyecto> ObtenerProyectoPorTablaBaseProyectoID(int pTablaBaseProyectoID)
        {
            List<Proyecto> listaProyecto = mEntityContext.Proyecto.Where(proyecto => proyecto.TablaBaseProyectoID.Equals(pTablaBaseProyectoID) && proyecto.Estado > 0).ToList();
            return listaProyecto;
        }

        /// <summary>
        /// Obtiene el identificador proyecto a partir del id autonumérico que se le asigna al crear la tabla BASE.
        /// </summary>
        /// <param name="pTablaBaseProyectoID">Identificador de la tabla base del proyecto</param>
        /// <returns>Identificador del proyecto con el proyecto cargado</returns>
        public Guid ObtenerProyectoIDPorTablaBaseProyectoID(int pTablaBaseProyectoID)
        {
            Guid proyID = MetaProyecto;

            object resultado = mEntityContext.Proyecto.Where(proyecto => proyecto.TablaBaseProyectoID.Equals(pTablaBaseProyectoID)).Select(proyecto => proyecto.ProyectoID).FirstOrDefault();

            if (resultado != null && resultado is Guid)
            {
                proyID = (Guid)resultado;
            }

            return proyID;
        }

        /// <summary>
        /// Obtiene los proyectos con ontologias que administra el usuario
        /// </summary>
        /// <param name="pUsuarioID"></param>
        /// <returns></returns>
        public Dictionary<Guid, string> ObtenerProyectosConOntologiasAdministraUsuario(Guid pUsuarioID)
        {
            Dictionary<Guid, string> listadoProyectos = new Dictionary<Guid, string>();

            var resultadoConsulta = mEntityContext.OntologiaProyecto.Join(mEntityContext.Proyecto, ontologiaProy => ontologiaProy.ProyectoID, proyecto => proyecto.ProyectoID, (ontologiaProy, proyecto) => new
            {
                OntologiaProyecto = ontologiaProy,
                Proyecto = proyecto
            }).Join(mEntityContext.AdministradorProyecto, objeto => objeto.Proyecto.ProyectoID, adminProy => adminProy.ProyectoID, (objeto, adminProy) => new
            {
                OntologiaProyecto = objeto.OntologiaProyecto,
                Proyecto = objeto.Proyecto,
                AdministradorProyecto = adminProy
            }).Where(objeto => objeto.Proyecto.Estado.Equals((short)EstadoProyecto.Abierto) && objeto.AdministradorProyecto.UsuarioID.Equals(pUsuarioID)).Select(objeto => new { objeto.Proyecto.ProyectoID, objeto.Proyecto.Nombre }).ToList();

            foreach (var fila in resultadoConsulta)
            {
                Guid idProyecto = fila.ProyectoID;
                string nombreProyecto = fila.Nombre;
                if (!listadoProyectos.ContainsKey(idProyecto))
                {
                    listadoProyectos.Add(idProyecto, nombreProyecto);
                }
            }

            return listadoProyectos;
        }
        /// <summary>
        /// Obtiene los NombreFiltro de los ProyectosSearchPersonalizados del proyecto
        /// </summary>
        /// <param name="pProyectoID">Id del proyecto</param>
        /// <returns>Lista con los NombreFiltro del proyecto</returns>
        public List<ProyectoSearchPersonalizado> ObtenerProyectosSearchPersonalizado(Guid pProyectoID)
        {
            List<ProyectoSearchPersonalizado> listaNombreFiltro = new List<ProyectoSearchPersonalizado>();

            listaNombreFiltro = mEntityContext.ProyectoSearchPersonalizado.Where(proy => proy.ProyectoID.Equals(pProyectoID)).ToList();

            return listaNombreFiltro;
        }
        /// <summary>
        /// Obtiene los valores de la consulta SPARQL asociada al filtro
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <param name="nombreFiltro">Nombre del filtro de la consulta</param>
        /// <returns>Lista con los de la consulta de SPARQL</returns>
        public List<string> ObtenerValoresConsultaSPARQL(Guid pProyectoID, string nombreFiltro)
        {
            List<string> consultaSPARQL = new List<string>();

            var datosConsulta = mEntityContext.ProyectoSearchPersonalizado.Where(proy => proy.ProyectoID.Equals(pProyectoID) && proy.NombreFiltro.Equals(nombreFiltro)).FirstOrDefault();

            consultaSPARQL.Add(datosConsulta.WhereSPARQL);
            consultaSPARQL.Add(datosConsulta.OrderBySPARQL);
            consultaSPARQL.Add(datosConsulta.WhereFacetasSPARQL);

            return consultaSPARQL;
        }
        /// <summary>
        /// Actualiza los valores de la consulta de SPARQL, en caso de no existir lo añade 
        /// </summary>
        /// <param name="organizacionID">Id de la organizacion</param>
        /// <param name="pProyectoID">Id del proyecto</param>
        /// <param name="nombreFiltro">Nombre del filtro de las consultas</param>
        /// <param name="listaValores">Lista de los valores de las consultas de SPARQL</param>
        public void ActualizarValoresConsultaSPARQL(Guid organizacionID, Guid pProyectoID, string nombreFiltro, List<string> listaValores)
        {
            ProyectoSearchPersonalizado proySeleccionado = mEntityContext.ProyectoSearchPersonalizado.Where(proy => proy.ProyectoID.Equals(pProyectoID) && proy.OrganizacionID.Equals(organizacionID) && proy.NombreFiltro.Equals(nombreFiltro)).FirstOrDefault();

            if (proySeleccionado == null)
            {
                ProyectoSearchPersonalizado proyPersonalizado = new ProyectoSearchPersonalizado();
                proyPersonalizado.OrganizacionID = organizacionID;
                proyPersonalizado.ProyectoID = pProyectoID;
                proyPersonalizado.NombreFiltro = nombreFiltro;
                proyPersonalizado.WhereSPARQL = listaValores[0];
                proyPersonalizado.OrderBySPARQL = listaValores[1];
                proyPersonalizado.WhereFacetasSPARQL = listaValores[2];
                proyPersonalizado.OmitirRdfType = false;

                mEntityContext.ProyectoSearchPersonalizado.Add(proyPersonalizado);
            }
            else
            {
                proySeleccionado.WhereSPARQL = listaValores[0];
                proySeleccionado.OrderBySPARQL = listaValores[1];
                proySeleccionado.WhereFacetasSPARQL = listaValores[2];
            }

            ActualizarBaseDeDatosEntityContext();
        }
        /// <summary>
        /// Actualiza los valores de la tabla ProyectoSearchPersonalizado con los nuevos valores de búsqueda personalizada
        /// </summary>
        /// <param name="organizacionID">Id de la organizacion</param>
        /// <param name="pProyectoID">Id del proyecto</param>
        /// <param name="listaParametros">Lista de los parámetros de búsqueda personalizados</param>
        public void ActualizarParametrosBusquedaPersonalizados(Guid organizacionID, Guid pProyectoID, List<ParametroBusquedaPersonalizadoModel> listaParametros)
        {
            foreach (ParametroBusquedaPersonalizadoModel param in listaParametros)
            {
                ProyectoSearchPersonalizado proyecto = mEntityContext.ProyectoSearchPersonalizado.Where(proy => proy.ProyectoID.Equals(pProyectoID) && proy.NombreFiltro.Equals(param.NombreParametro)).FirstOrDefault();
                if (proyecto == null)
                {
                    if (param.NombreParametro != null)
                    {
                        ProyectoSearchPersonalizado nuevoProyecto = new ProyectoSearchPersonalizado();
                        nuevoProyecto.OrganizacionID = organizacionID;
                        nuevoProyecto.ProyectoID = pProyectoID;
                        nuevoProyecto.NombreFiltro = param.NombreParametro;
                        nuevoProyecto.WhereSPARQL = param.WhereParametro == null ? string.Empty : param.WhereParametro;
                        nuevoProyecto.OrderBySPARQL = param.OrderByParametro == null ? string.Empty : param.OrderByParametro;
                        nuevoProyecto.WhereFacetasSPARQL = param.WhereFacetaParametro == null ? string.Empty : param.WhereFacetaParametro;
                        nuevoProyecto.OmitirRdfType = false;

                        mEntityContext.ProyectoSearchPersonalizado.Add(nuevoProyecto);
                    }

                }
                else
                {
                    if (param.Deleted)
                    {
                        mEntityContext.Entry(proyecto).State = EntityState.Deleted;
                    }
                    else
                    {
                        if (param.NombreParametro != null)
                        {
                            proyecto.NombreFiltro = param.NombreParametro;
                            proyecto.WhereSPARQL = param.WhereParametro == null ? string.Empty : param.WhereParametro;
                            proyecto.OrderBySPARQL = param.OrderByParametro == null ? string.Empty : param.OrderByParametro;
                            proyecto.WhereFacetasSPARQL = param.WhereFacetaParametro == null ? string.Empty : param.WhereFacetaParametro;
                        }
                    }
                }

            }
            ActualizarBaseDeDatosEntityContext();

        }
        /// <summary>
        /// Comprueba si existe el proyecto FAQ
        /// </summary>
        /// <returns>TRUE si existe el proyecto FAQ</returns>
        public bool ExisteProyectoFAQ()
        {
            DbCommand comandoExiste = ObtenerComando(this.sqlSelectExisteProyectoFAQ);

            object resultado = EjecutarEscalar(comandoExiste);

            if ((resultado != null) && (resultado.Equals(1)))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Comprueba si existe el proyecto Noticias de Gnoss
        /// </summary>
        /// <returns>TRUE si existe el proyecto Noticias de Gnoss</returns>
        public bool ExisteProyectoNoticias()
        {
            DbCommand comandoExiste = ObtenerComando(this.sqlSelectExisteProyectoNoticias);

            object resultado = EjecutarEscalar(comandoExiste);

            if ((resultado != null) && (resultado.Equals(1)))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Comprueba si existe el proyecto Didactalia de Gnoss
        /// </summary>
        /// <returns>TRUE si existe el proyecto Didactalia de Gnoss</returns>
        public bool ExisteProyectoDidactalia()
        {
            DbCommand comandoExiste = ObtenerComando(this.sqlSelectExisteProyectoDidactalia);

            object resultado = EjecutarEscalar(comandoExiste);

            if ((resultado != null) && (resultado.Equals(1)))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Obtiene los proyectos de una organización a partir de una lista de proyectos pasados por parámetro
        /// </summary>
        /// <param name="pListaProyectoID">Lista de identificadores de proyecto</param>
        /// <returns>Dataset de proyectos</returns>
        public DataWrapperProyecto ObtenerProyectosPorID(List<Guid> pListaProyectoID)
        {
            DataWrapperProyecto dataWrapperproyectoDS = new DataWrapperProyecto();

            if (pListaProyectoID.Count > 0)
            {
                List<Proyecto> listaProyectos = mEntityContext.Proyecto.Where(proyecto => pListaProyectoID.Contains(proyecto.ProyectoID)).ToList();
                List<ProyectoAgCatTesauro> listaProyectosAgCatTesauro = mEntityContext.ProyectoAgCatTesauro.Where(proyectoAgCatTesauro => pListaProyectoID.Contains(proyectoAgCatTesauro.ProyectoID)).ToList();
                List<AdministradorProyecto> listaAdministradoresProyecto = mEntityContext.AdministradorProyecto.Where(adminProyecto => pListaProyectoID.Contains(adminProyecto.ProyectoID)).ToList();
                dataWrapperproyectoDS.ListaProyecto = listaProyectos;
                dataWrapperproyectoDS.ListaProyectoAgCatTesauro = listaProyectosAgCatTesauro;
                dataWrapperproyectoDS.ListaAdministradorProyecto = listaAdministradoresProyecto;
            }
            return (dataWrapperproyectoDS);
        }

        /// <summary>
        /// Obtiene proyectos (carga ligera) a partir de la lista de sus identificadores pasada como parámetro
        /// </summary>
        /// <param name="pListaProyectoID">Lista de identificadores de proyecto</param>
        /// <param name="pTraerSoloAbiertos">True si sólo se quieren obtener los proyectos de la lista que están abiertos (False por defecto)</param>
        /// <param name="pLimite">Número de proyectos máximo a obtener</param>
        /// <returns>Dataset de proyectos</returns>
        public DataWrapperProyecto ObtenerProyectosPorIDsCargaLigera(List<Guid> pListaProyectoID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            var listaProyectosLigero = mEntityContext.Proyecto.Where(proyecto => pListaProyectoID.Contains(proyecto.ProyectoID)).Select(proyecto => new
            {
                OrganizacionID = proyecto.OrganizacionID,
                ProyectoID = proyecto.ProyectoID,
                Nombre = proyecto.Nombre,
                TipoProyecto = proyecto.TipoProyecto,
                TipoAcceso = proyecto.TipoAcceso,
                NumeroRecursos = proyecto.NumeroRecursos,
                NumeroPreguntas = proyecto.NumeroPreguntas,
                NumeroDebates = proyecto.NumeroDebates,
                NumeroMiembros = proyecto.NumeroMiembros,
                NumeroOrgRegistradas = proyecto.NumeroOrgRegistradas,
                EsProyectoDestacado = proyecto.EsProyectoDestacado,
                Estado = proyecto.Estado,
                URLPropia = proyecto.URLPropia,
                NombreCorto = proyecto.NombreCorto,
                Descripcion = proyecto.Descripcion,
                ProyectoSuperiorID = proyecto.ProyectoSuperiorID,
                TieneTwitter = proyecto.TieneTwitter,
                TagTwitter = proyecto.TagTwitter,
                UsuarioTwitter = proyecto.UsuarioTwitter,
                TokenTwitter = proyecto.TokenTwitter,
                TokenSecretoTwitter = proyecto.TokenSecretoTwitter,
                EnviarTwitterComentario = proyecto.EnviarTwitterComentario,
                EnviarTwitterNuevaCat = proyecto.EnviarTwitterNuevaCat,
                EnviarTwitterNuevoAdmin = proyecto.EnviarTwitterNuevoAdmin,
                EnviarTwitterNuevaPolitCert = proyecto.EnviarTwitterNuevaPolitCert,
                EnviarTwitterNuevoTipoDoc = proyecto.EnviarTwitterNuevoTipoDoc,
                TablaBaseProyectoID = proyecto.TablaBaseProyectoID,
                ProcesoVinculadoID = proyecto.ProcesoVinculadoID,
                Tags = proyecto.Tags,
                TagTwitterGnoss = proyecto.TagTwitterGnoss,
                NombrePresentacion = proyecto.NombrePresentacion
            });
            foreach (var proyecto in listaProyectosLigero.ToList())
            {
                Proyecto proyectoObj = new Proyecto();
                proyectoObj.OrganizacionID = proyecto.OrganizacionID;
                proyectoObj.ProyectoID = proyecto.ProyectoID;
                proyectoObj.Nombre = proyecto.Nombre;
                proyectoObj.TipoProyecto = proyecto.TipoProyecto;
                proyectoObj.TipoAcceso = proyecto.TipoAcceso;
                proyectoObj.NumeroRecursos = proyecto.NumeroRecursos;
                proyectoObj.NumeroPreguntas = proyecto.NumeroPreguntas;
                proyectoObj.NumeroDebates = proyecto.NumeroDebates;
                proyectoObj.NumeroMiembros = proyecto.NumeroMiembros;
                proyectoObj.NumeroOrgRegistradas = proyecto.NumeroOrgRegistradas;
                proyectoObj.EsProyectoDestacado = proyecto.EsProyectoDestacado;
                proyectoObj.Estado = proyecto.Estado;
                proyectoObj.URLPropia = proyecto.URLPropia;
                proyectoObj.NombreCorto = proyecto.NombreCorto;
                proyectoObj.Descripcion = proyecto.Descripcion;
                proyectoObj.ProyectoSuperiorID = proyecto.ProyectoSuperiorID;
                proyectoObj.TieneTwitter = proyecto.TieneTwitter;
                proyectoObj.TagTwitter = proyecto.TagTwitter;
                proyectoObj.UsuarioTwitter = proyecto.UsuarioTwitter;
                proyectoObj.TokenTwitter = proyecto.TokenTwitter;
                proyectoObj.TokenSecretoTwitter = proyecto.TokenSecretoTwitter;
                proyectoObj.EnviarTwitterComentario = proyecto.EnviarTwitterComentario;
                proyectoObj.EnviarTwitterNuevaCat = proyecto.EnviarTwitterNuevaCat;
                proyectoObj.EnviarTwitterNuevoAdmin = proyecto.EnviarTwitterNuevoAdmin;
                proyectoObj.EnviarTwitterNuevaPolitCert = proyecto.EnviarTwitterNuevaPolitCert;
                proyectoObj.EnviarTwitterNuevoTipoDoc = proyecto.EnviarTwitterNuevoTipoDoc;
                proyectoObj.TablaBaseProyectoID = proyecto.TablaBaseProyectoID;
                proyectoObj.ProcesoVinculadoID = proyecto.ProcesoVinculadoID;
                proyectoObj.Tags = proyecto.Tags;
                proyectoObj.TagTwitterGnoss = proyecto.TagTwitterGnoss;
                proyectoObj.NombrePresentacion = proyecto.NombrePresentacion;
                dataWrapperProyecto.ListaProyecto.Add(proyectoObj);
            }
            return dataWrapperProyecto;
        }

        /// <summary>
        /// Obtiene (carga ligera) los datos de los proyectos mas populares a los que no pertenece el usuario
        /// </summary>
        /// <param name="pPersonaID">ID de la persona</param>
        /// <param name="pNumeroProyectos">Número de proyectos</param>
        /// <returns>Dataset de proyecto</returns>
        public List<Proyecto> ObtenerProyectosRecomendadosPorPersona(Guid pPersonaID, int pNumeroProyectos)
        {
            ////TODO NUEVO
            List<Proyecto> listaProyectos = new List<Proyecto>();
            if (pNumeroProyectos > 0)
            {
                var varGuids = mEntityContext.Identidad.Join(mEntityContext.Perfil, identidad => identidad.PerfilID, perfil => perfil.PerfilID, (identidad, perfil) => new
                {
                    ProyectoID = identidad.ProyectoID,
                    PersonaID = perfil.PersonaID
                }).Where(selecccion => selecccion.PersonaID.Value.Equals(pPersonaID));

                List<Guid> listaGuids = new List<Guid>();
                foreach (var id in varGuids.ToList())
                {
                    listaGuids.Add(id.ProyectoID);
                }

                var lista = mEntityContext.ProyectosMasActivos.Join(mEntityContext.Proyecto, proyectosMasActivos => proyectosMasActivos.ProyectoID, proyecto => proyecto.ProyectoID, (proyectosMasActivos, proyecto) => new
                {
                    ProyectoID = proyectosMasActivos.ProyectoID,
                    TipoAcceso = proyecto.TipoAcceso,
                    Estado = proyecto.Estado,
                    Peso = proyectosMasActivos.Peso
                }).Where(proyectoMasActivo => proyectoMasActivo.TipoAcceso.Equals((short)TipoAcceso.Publico) && proyectoMasActivo.Estado.Equals((short)EstadoProyecto.Abierto) && !listaGuids.Contains(proyectoMasActivo.ProyectoID)).OrderByDescending(proyectoMasActivo => proyectoMasActivo.Peso).Take(pNumeroProyectos);

                List<Guid> listaIdsProyectosMasActivos = new List<Guid>();
                foreach (var id in lista.ToList())
                {
                    listaIdsProyectosMasActivos.Add(id.ProyectoID);
                }

                if (listaIdsProyectosMasActivos.Count > 0)
                {
                    listaProyectos = mEntityContext.Proyecto.Where(proyecto => listaIdsProyectosMasActivos.Contains(proyecto.ProyectoID)).ToList();
                }
            }
            return listaProyectos;
        }

        /// <summary>
        /// Obtiene un proyecto (Proyecto, AdministradorProyecto,ProyectoAgCatTesauro, ProyectoCerradoTmp, ProyectoCerrandose, DatoExtraProyecto, DatoExtraProyectoOpcion, DatoExtraProyectoVirtuoso, ProyectoPasoRegistro, CamposRegistroProyectoGenericos,ProyectoPestanyaMenu, ProyectoPestanyaCMS, ProyectoPestanyaBusqueda, ProyectoPestanyaMenuRolIdetndidad, ProyectoPestanyaMenuRolGrupoIdentidades) a partir de su identificador
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Dataset de proyecto con el proyecto buscado</returns>
        public DataWrapperProyecto ObtenerProyectoPorID(Guid pProyectoID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();
            dataWrapperProyecto.ListaProyecto = mEntityContext.Proyecto.Where(proy => proy.ProyectoID.Equals(pProyectoID)).ToList();
            dataWrapperProyecto.ListaAdministradorProyecto = mEntityContext.AdministradorProyecto.Where(adminProy => adminProy.ProyectoID.Equals(pProyectoID)).ToList();
            dataWrapperProyecto.ListaAdministradorGrupoProyecto = mEntityContext.AdministradorGrupoProyecto.Where(adminGroupProy => adminGroupProy.ProyectoID.Equals(pProyectoID)).ToList();
            dataWrapperProyecto.ListaProyectoAgCatTesauro = mEntityContext.ProyectoAgCatTesauro.Where(proyectoAgCatTesauro => proyectoAgCatTesauro.ProyectoID.Equals(pProyectoID)).ToList();
            dataWrapperProyecto.ListaProyectoCerradoTmp = mEntityContext.ProyectoCerradoTmp.Where(proyectoCerradoTmp => proyectoCerradoTmp.ProyectoID.Equals(pProyectoID)).ToList();
            dataWrapperProyecto.ListaProyectoCerrandose = mEntityContext.ProyectoCerrandose.Where(proyectoCerrandose => proyectoCerrandose.ProyectoID.Equals(pProyectoID)).ToList();
            dataWrapperProyecto.ListaDatoExtraProyecto = mEntityContext.DatoExtraProyecto.Where(datoExtra => datoExtra.ProyectoID.Equals(pProyectoID)).ToList();
            dataWrapperProyecto.ListaDatoExtraProyectoOpcion = mEntityContext.DatoExtraProyectoOpcion.Where(datoExtra => datoExtra.ProyectoID.Equals(pProyectoID)).ToList();
            dataWrapperProyecto.ListaDatoExtraProyectoVirtuoso = mEntityContext.DatoExtraProyectoVirtuoso.Where(datoExtra => datoExtra.ProyectoID.Equals(pProyectoID)).ToList();
            dataWrapperProyecto.ListaProyectoPasoRegistro = mEntityContext.ProyectoPasoRegistro.Where(proyectoPasoRegistro => proyectoPasoRegistro.ProyectoID.Equals(pProyectoID)).ToList();
            dataWrapperProyecto.ListaCamposRegistroProyectoGenericos = mEntityContext.CamposRegistroProyectoGenericos.Where(campos => campos.ProyectoID.Equals(pProyectoID)).ToList();
            dataWrapperProyecto.ListaProyectoPestanyaMenu = mEntityContext.ProyectoPestanyaMenu.Where(proyectoPestanyaMenu => proyectoPestanyaMenu.ProyectoID.Equals(pProyectoID)).OrderBy(proyecto => proyecto.Orden).ToList();
            dataWrapperProyecto.ListaProyectoPestanyaCMS = mEntityContext.ProyectoPestanyaCMS.Join(mEntityContext.ProyectoPestanyaMenu, proyPestanyaCMS => proyPestanyaCMS.PestanyaID, proyPestanyaMenu => proyPestanyaMenu.PestanyaID, (proyPestanyaCMS, proyPestanyaMenu) => new
            {
                ProyectoPestanyaCMS = proyPestanyaCMS,
                ProyectPestanyaMenuID = proyPestanyaMenu.ProyectoID
            }).Where(proyPestanyaCMS => proyPestanyaCMS.ProyectPestanyaMenuID.Equals(pProyectoID)).Select(proyPestanyaCMS => proyPestanyaCMS.ProyectoPestanyaCMS).ToList();

            dataWrapperProyecto.ListaProyectoPestanyaBusqueda = mEntityContext.ProyectoPestanyaBusqueda.Join(mEntityContext.ProyectoPestanyaMenu, proyPestBusqueda => proyPestBusqueda.PestanyaID, proyPestMenu => proyPestMenu.PestanyaID, (proyPestBusqueda, proyPestMenu) => new
            {
                ProyectPestanyaBusqueda = proyPestBusqueda,
                ProyectoID = proyPestMenu.ProyectoID
            }).Where(proyecto => proyecto.ProyectoID.Equals(pProyectoID)).Select(proyecto => proyecto.ProyectPestanyaBusqueda).ToList();

            dataWrapperProyecto.ListaProyectoPestanyaMenuRolGrupoIdentidades = mEntityContext.ProyectoPestanyaMenuRolGrupoIdentidades.Join(mEntityContext.ProyectoPestanyaMenu, proyPestMenuRogrupolId => proyPestMenuRogrupolId.PestanyaID, proyPestMenu => proyPestMenu.PestanyaID, (proyPestMenuRogrupolId, proyPestMenu) => new
            {
                proyPestMenuRogrupolId = proyPestMenuRogrupolId,
                ProyectoID = proyPestMenu.ProyectoID
            }).Where(proy => proy.ProyectoID.Equals(pProyectoID)).Select(proy => proy.proyPestMenuRogrupolId).ToList();

            dataWrapperProyecto.ListaProyectoPestanyaMenuRolIdentidad = mEntityContext.ProyectoPestanyaMenuRolIdentidad.Join(mEntityContext.ProyectoPestanyaMenu, proyPestRolId => proyPestRolId.PestanyaID, proyPestMenu => proyPestMenu.PestanyaID, (proyPestRolId, proyPestMenu) => new
            {
                proyectPestanyaRolIdentidad = proyPestRolId,
                ProyectoID = proyPestMenu.ProyectoID
            }).Where(proy => proy.ProyectoID.Equals(pProyectoID)).Select(proy => proy.proyectPestanyaRolIdentidad).ToList();

            dataWrapperProyecto.ListaProyectoPestanyaFiltroOrdenRecursos = mEntityContext.ProyectoPestanyaFiltroOrdenRecursos.Join(mEntityContext.ProyectoPestanyaMenu, proyPestFiltro => proyPestFiltro.PestanyaID, proyPestMenu => proyPestMenu.PestanyaID, (proyPestFiltro, proyPestMenu) => new
            {
                proyectoPestanyaFiltroOrdenRecursos = proyPestFiltro,
                ProyectoID = proyPestMenu.ProyectoID
            }).Where(proy => proy.ProyectoID.Equals(pProyectoID)).Select(proy => proy.proyectoPestanyaFiltroOrdenRecursos).ToList();


            dataWrapperProyecto.ListaProyectoEventoAccion = mEntityContext.ProyectoEventoAccion.Where(proyectoEvento => proyectoEvento.ProyectoID.Equals(pProyectoID)).ToList();

            dataWrapperProyecto.ListaAccionesExternasProyecto = mEntityContext.AccionesExternasProyecto.Where(acciones => acciones.ProyectoID.Equals(pProyectoID)).ToList();

            dataWrapperProyecto.ListaProyectoSearchPersonalizado = mEntityContext.ProyectoSearchPersonalizado.Where(proyectoSearchPersonalizado => proyectoSearchPersonalizado.ProyectoID.Equals(pProyectoID)).ToList();
            dataWrapperProyecto.ListaOntologiaProyecto = mEntityContext.OntologiaProyecto.Where(ontologiaProy => ontologiaProy.ProyectoID.Equals(pProyectoID)).ToList();

            dataWrapperProyecto.ListaVistaVirtualProyecto = mEntityContext.VistaVirtualProyecto.Where(vistaVirtualProy => vistaVirtualProy.ProyectoID.Equals(pProyectoID)).ToList();

            dataWrapperProyecto.ListaConfigAutocompletarProy = mEntityContext.ConfigAutocompletarProy.Where(config => config.ProyectoID.Equals(pProyectoID)).ToList();

            return dataWrapperProyecto;
        }

        /// <summary>
        /// Obtiene los datos Extra de un proyecto (incluidos los del ecosistema) (para los regisrtros de usuarios)
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto</param>
        /// <returns>DataSet de Proyecto con las tablas DatoExtraProyecto y DatoExtraProyectoOpcion</returns>
        public DataWrapperProyecto ObtenerDatosExtraProyectoPorID(Guid pProyectoID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            dataWrapperProyecto.ListaDatoExtraProyecto = mEntityContext.DatoExtraProyecto.Where(dato => dato.ProyectoID.Equals(pProyectoID)).ToList();

            dataWrapperProyecto.ListaDatoExtraProyectoOpcion = mEntityContext.DatoExtraProyectoOpcion.Where(dato => dato.ProyectoID.Equals(pProyectoID)).ToList();

            dataWrapperProyecto.ListaDatoExtraProyectoVirtuoso = mEntityContext.DatoExtraProyectoVirtuoso.Where(dato => dato.ProyectoID.Equals(pProyectoID)).ToList();

            dataWrapperProyecto.ListaDatoExtraEcosistema = mEntityContext.DatoExtraEcosistema.ToList();

            dataWrapperProyecto.ListaDatoExtraEcosistemaOpcion = mEntityContext.DatoExtraEcosistemaOpcion.ToList();

            dataWrapperProyecto.ListaDatoExtraEcosistemaVirtuoso = mEntityContext.DatoExtraEcosistemaVirtuoso.ToList();

            dataWrapperProyecto.ListaCamposRegistroProyectoGenericos = mEntityContext.CamposRegistroProyectoGenericos.Where(campos => campos.ProyectoID.Equals(pProyectoID)).ToList();

            return (dataWrapperProyecto);
        }

        /// <summary>
        /// Obtiene los datos Extra de un proyecto (incluidos los del ecosistema) (para los registros de usuarios)
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto</param>
        /// <returns>DataSet de Proyecto con las tablas DatoExtraProyecto y DatoExtraProyectoOpcion</returns>
        public DataWrapperProyecto ObtenerDatosExtraProyectoPorListaIDs(List<Guid> pListaProyectosID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            dataWrapperProyecto.ListaDatoExtraProyecto = mEntityContext.DatoExtraProyecto.Where(dato => pListaProyectosID.Contains(dato.ProyectoID)).ToList();

            dataWrapperProyecto.ListaDatoExtraProyectoOpcion = mEntityContext.DatoExtraProyectoOpcion.Where(dato => pListaProyectosID.Contains(dato.ProyectoID)).ToList();

            dataWrapperProyecto.ListaDatoExtraProyectoVirtuoso = mEntityContext.DatoExtraProyectoVirtuoso.Where(dato => pListaProyectosID.Contains(dato.ProyectoID)).ToList();

            dataWrapperProyecto.ListaDatoExtraEcosistema = mEntityContext.DatoExtraEcosistema.ToList();

            dataWrapperProyecto.ListaDatoExtraEcosistemaOpcion = mEntityContext.DatoExtraEcosistemaOpcion.ToList();

            dataWrapperProyecto.ListaDatoExtraEcosistemaVirtuoso = mEntityContext.DatoExtraEcosistemaVirtuoso.ToList();

            dataWrapperProyecto.ListaCamposRegistroProyectoGenericos = mEntityContext.CamposRegistroProyectoGenericos.Where(campo => pListaProyectosID.Contains(campo.ProyectoID)).ToList();

            return (dataWrapperProyecto);
        }

        /// <summary>
        /// Obtiene las preferencias disponibles en un proyecto (para los registros de usuarios)
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto</param>
        /// <returns>DataSet de Proyecto con la tabla PreferenciasProyecto</returns>
        public DataWrapperProyecto ObtenerPreferenciasProyectoPorID(Guid pProyectoID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            dataWrapperProyecto.ListaPreferenciaProyecto = mEntityContext.PreferenciaProyecto.Where(preferencia => preferencia.ProyectoID.Equals(pProyectoID)).OrderBy(preferencia => preferencia.Orden).ToList();

            return (dataWrapperProyecto);
        }

        /// <summary>
        /// Obtiene las acciones externas en un proyecto
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto</param>
        /// <returns>DataSet de Proyecto con la tabla AccionesExternasProyecto</returns>
        public DataWrapperProyecto ObtenerAccionesExternasProyectoPorProyectoID(Guid pProyectoID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();
            dataWrapperProyecto.ListaAccionesExternasProyecto = mEntityContext.AccionesExternasProyecto.Where(acciones => acciones.ProyectoID.Equals(pProyectoID)).ToList();
            return (dataWrapperProyecto);
        }

        /// <summary>
        /// Obtiene las acciones externas en un proyecto
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto</param>
        /// <returns>DataSet de Proyecto con la tabla AccionesExternasProyecto</returns>
        public DataWrapperProyecto ObtenerAccionesExternasProyectoPorListaIDs(List<Guid> pListaProyectosID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();
            dataWrapperProyecto.ListaAccionesExternasProyecto = mEntityContext.AccionesExternasProyecto.Where(acciones => pListaProyectosID.Contains(acciones.ProyectoID)).ToList();
            return (dataWrapperProyecto);
        }

        /// <summary>
        /// Obtiene los eventos disponibles en un proyecto
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto</param>
        /// <returns>DataSet de Proyecto con la tabla EventoProyecto</returns>
        public DataWrapperProyecto ObtenerEventosProyectoPorProyectoID(Guid pProyectoID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            dataWrapperProyecto.ListaProyectoEvento = mEntityContext.ProyectoEvento.Where(proyecto => proyecto.ProyectoID.Equals(pProyectoID)).ToList();

            return (dataWrapperProyecto);
        }

        /// <summary>
        /// Obtiene los eventos disponibles en un proyecto para una identidad
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto</param>
        /// <param name="pIdentidadID">Identidad de la que se quieren obtener los eventos a los que está suscrita</param>
        /// <returns>DataSet de los eventos a los que está suscrita la identidad</returns>
        public DataSet ObtenerEventoProyectoIdentidadID(Guid pProyectoID, Guid pIdentidadID)
        {
            DataSet ds = new DataSet();

            DbCommand commandsqlSelectEventosProyectoPorIdentidadID = ObtenerComando(sqlSelectEventosProyectoPorIdentidadID);
            AgregarParametro(commandsqlSelectEventosProyectoPorIdentidadID, IBD.ToParam("proyectoID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pProyectoID));
            AgregarParametro(commandsqlSelectEventosProyectoPorIdentidadID, IBD.ToParam("identidadID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pIdentidadID));
            CargarDataSet(commandsqlSelectEventosProyectoPorIdentidadID, ds, "EventosProyectoIdentidad");

            return (ds);
        }


        /// <summary>
        /// Obtiene el evento con ese ID
        /// </summary>
        /// <param name="pEventoID">Clave del evento</param>
        /// <returns>DataSet de Proyecto con la tabla EventoProyecto</returns>
        public DataWrapperProyecto ObtenerEventoProyectoPorEventoID(Guid pEventoID, bool pSoloActivos)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            List<ProyectoEvento> listaProyectoEvento = mEntityContext.ProyectoEvento.Where(proyecto => proyecto.EventoID.Equals(pEventoID)).ToList();
            if (pSoloActivos)
            {
                listaProyectoEvento = listaProyectoEvento.Where(proyecto => proyecto.Activo == true).ToList();
            }
            dataWrapperProyecto.ListaProyectoEvento = listaProyectoEvento;

            return (dataWrapperProyecto);
        }

        /// <summary>
        /// Obtiene los participantes de un evento con ese ID
        /// </summary>
        /// <param name="pEventoID">Clave del evento</param>
        /// <returns>DataSet de Proyecto con la tabla EventoProyectoParticipante</returns>
        public DataWrapperProyecto ObtenerEventoProyectoParticipantesPorEventoID(Guid pEventoID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();
            dataWrapperProyecto.ListaProyectoEventoParticipante = mEntityContext.ProyectoEventoParticipante.Where(proyecto => proyecto.EventoID.Equals(pEventoID)).ToList();
            return (dataWrapperProyecto);
        }

        /// <summary>
        /// Obtiene el numero participantes de eventos de un proyecto
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto</param>
        /// <returns>Diccionario con clave evento y valor numero de participantes</returns>
        public Dictionary<Guid, int> ObtenerNumeroParticipantesEventosPorProyectoID(Guid pProyectoID)
        {
            Dictionary<Guid, int> EventosParticipantes = new Dictionary<Guid, int>();

            var resultadoConsulta = mEntityContext.ProyectoEventoParticipante.Join(mEntityContext.ProyectoEvento, proyectEvenParti => proyectEvenParti.EventoID, proyectoEvento => proyectoEvento.EventoID, (proyectEvenParti, proyectoEvento) => new
            {
                ProyectoEventoParticipante = proyectEvenParti,
                ProyectoEvento = proyectoEvento
            }).Where(objeto => objeto.ProyectoEvento.ProyectoID.Equals(pProyectoID)).GroupBy(objeto => objeto.ProyectoEventoParticipante.EventoID, objeto => objeto.ProyectoEventoParticipante.IdentidadID, (agrupacion, g) => new
            {
                EventoID = agrupacion,
                NumParticipantes = g.ToList().Count
            }).ToList();

            foreach (var fila in resultadoConsulta)
            {
                Guid idEvento = fila.EventoID;
                int numParticipantes = fila.NumParticipantes;
                EventosParticipantes.Add(idEvento, numParticipantes);
            }


            return EventosParticipantes;
        }

        /// <summary>
        /// Obtiene si una identidad participa en un evento
        /// </summary>
        /// <param name="pEventoID">Clave del evento</param>
        /// <param name="pIdentidad">Clave de la identidad</param>
        /// <returns>TRUE si participa en el evento</returns>
        public bool ObtenerSiIdentidadParticipaEnEvento(Guid pEventoID, Guid pIdentidad)
        {
            List<ProyectoEventoParticipante> lista = mEntityContext.ProyectoEventoParticipante.Where(proyecto => proyecto.EventoID.Equals(pEventoID) && proyecto.IdentidadID.Equals(pIdentidad)).ToList();
            bool participa = (lista.Count > 0);

            return (participa);
        }

        /// <summary>
        /// Obtiene la lista de proyectos en las que participa un usuario pasado por parámetro
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <returns>Dataset de proyecto con todos los proyectos en los que participa el usuario</returns>
        public DataWrapperProyecto ObtenerProyectosParticipaUsuario(Guid pUsuarioID)
        {
            return ObtenerProyectosParticipaUsuario(pUsuarioID, false);
        }

        /// <summary>
        /// Obtiene la lista de proyectos en las que participa un usuario pasado por parámetro
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <param name="pTipoDocumentoCompartido">Tipo del recurso que va a compartir el usuario</param>
        /// <returns>Dataset de proyecto con todos los proyectos en los que participa el usuario</returns>
        public DataWrapperProyecto ObtenerProyectosUsuarioPuedeCompartirRecurso(Guid pUsuarioID, TiposDocumentacion pTipoDocumentoCompartido)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            string sqlParaProyecto;
            string sqlParaProyectoUsuarioIdentidad;

            if (pTipoDocumentoCompartido != TiposDocumentacion.EntradaBlog)
            {
                sqlParaProyecto = sqlSelectProyectosUsuarioPuedeCompartirRecurso;
                sqlParaProyectoUsuarioIdentidad = sqlSelectProyectoUsuarioIdentidadUsuarioPuedeCompartirRecurso;

                dataWrapperProyecto.ListaProyecto = mEntityContext.Proyecto.Join(mEntityContext.ProyectoUsuarioIdentidad, proyecto => new { proyecto.OrganizacionID, proyecto.ProyectoID }, proyUserIden => new { OrganizacionID = proyUserIden.OrganizacionGnossID, proyUserIden.ProyectoID }, (proyecto, proyUserIden) => new
                {
                    Proyecto = proyecto,
                    ProyectoUsuarioIdentidad = proyUserIden
                }).Join(mEntityContext.AdministradorProyecto, objeto => new { objeto.Proyecto.OrganizacionID, objeto.Proyecto.ProyectoID, objeto.ProyectoUsuarioIdentidad.UsuarioID }, adminProy => new { adminProy.OrganizacionID, adminProy.ProyectoID, adminProy.UsuarioID }, (objeto, adminProy) => new
                {
                    Proyecto = objeto.Proyecto,
                    ProyectoUsuarioIdentidad = objeto.ProyectoUsuarioIdentidad,
                    AdministradorProyecto = adminProy
                }).Join(mEntityContext.TipoDocDispRolUsuarioProy, objeto => new { objeto.Proyecto.ProyectoID, objeto.Proyecto.OrganizacionID }, tipo => new { tipo.ProyectoID, tipo.OrganizacionID }, (objeto, tipo) => new
                {
                    Proyecto = objeto.Proyecto,
                    ProyectoUsuarioIdentidad = objeto.ProyectoUsuarioIdentidad,
                    AdministradorProyecto = objeto.AdministradorProyecto,
                    TipoDocDispRolUsuarioProy = tipo
                }).Where(objeto => objeto.TipoDocDispRolUsuarioProy.RolUsuario >= objeto.AdministradorProyecto.Tipo && objeto.ProyectoUsuarioIdentidad.UsuarioID.Equals(pUsuarioID) && objeto.Proyecto.Estado.Equals((short)EstadoProyecto.Abierto) && objeto.TipoDocDispRolUsuarioProy.TipoDocumento.Equals((short)pTipoDocumentoCompartido)).Select(objeto => objeto.Proyecto).Concat(
                    mEntityContext.Proyecto.Join(mEntityContext.ProyectoUsuarioIdentidad, proyecto => new { proyecto.OrganizacionID, proyecto.ProyectoID }, usuarioIdent => new { OrganizacionID = usuarioIdent.OrganizacionGnossID, usuarioIdent.ProyectoID }, (proyecto, usuarioIden) => new
                    {
                        Proyecto = proyecto,
                        ProyectoUsuarioIdentidad = usuarioIden
                    }).Join(mEntityContext.TipoDocDispRolUsuarioProy, objeto => new { objeto.Proyecto.ProyectoID, objeto.Proyecto.OrganizacionID }, tipoDoc => new { tipoDoc.ProyectoID, tipoDoc.OrganizacionID }, (objeto, tipoDoc) => new
                    {
                        Proyecto = objeto.Proyecto,
                        ProyectoUsuarioIdentidad = objeto.ProyectoUsuarioIdentidad,
                        TipoDocDispRolUsuarioProy = tipoDoc
                    }).Where(objeto => objeto.ProyectoUsuarioIdentidad.UsuarioID.Equals(pUsuarioID) && objeto.Proyecto.Estado.Equals((short)EstadoProyecto.Abierto) && objeto.TipoDocDispRolUsuarioProy.RolUsuario.Equals((short)TipoRolUsuario.Usuario) && objeto.TipoDocDispRolUsuarioProy.TipoDocumento.Equals((short)pTipoDocumentoCompartido)).Select(objeto => objeto.Proyecto)
                    ).Concat(
                        mEntityContext.Proyecto.Join(mEntityContext.AdministradorProyecto, proyecto => proyecto.ProyectoID, adminProy => adminProy.ProyectoID, (proyecto, adminProy) => new
                        {
                            Proyecto = proyecto,
                            AdministradorProyecto = adminProy
                        }).Join(mEntityContext.TesauroProyecto, objeto => objeto.Proyecto.ProyectoID, tesauro => tesauro.ProyectoID, (objeto, tesauro) => new
                        {
                            Proyecto = objeto.Proyecto,
                            AdministradorProyecto = objeto.AdministradorProyecto,
                            TesauroProyecto = tesauro
                        }).Join(mEntityContext.CategoriaTesauro, objeto => objeto.TesauroProyecto.TesauroID, catTes => catTes.TesauroID, (objeto, catTes) => new
                        {
                            Proyecto = objeto.Proyecto,
                            AdministradorProyecto = objeto.AdministradorProyecto,
                            TesauroProyecto = objeto.TesauroProyecto,
                            CategoriaTesauro = catTes
                        }).Where(objeto => objeto.AdministradorProyecto.UsuarioID.Equals(pUsuarioID)).Select(objeto => objeto.Proyecto)
                    ).OrderBy(objeto => objeto.Nombre).ToList().Distinct().ToList();
            }
            else
            {
                dataWrapperProyecto.ListaProyecto = mEntityContext.Proyecto.Join(mEntityContext.ProyectoUsuarioIdentidad, proyecto => new { proyecto.OrganizacionID, proyecto.ProyectoID }, proyUserIden => new { OrganizacionID = proyUserIden.OrganizacionGnossID, proyUserIden.ProyectoID }, (proyecto, proyUserIden) => new
                {
                    Proyecto = proyecto,
                    ProyectoUsuarioIdentidad = proyUserIden
                }).Where(objeto => objeto.ProyectoUsuarioIdentidad.UsuarioID.Equals(pUsuarioID) && objeto.Proyecto.Estado.Equals((short)EstadoProyecto.Abierto)).Select(objeto => objeto.Proyecto).Concat(

                    mEntityContext.Proyecto.Join(mEntityContext.AdministradorProyecto, proyecto => proyecto.ProyectoID, adminProy => adminProy.ProyectoID, (Proyecto, adminProy) => new
                    {
                        Proyecto = Proyecto,
                        AdministradorProyecto = adminProy
                    }).Join(mEntityContext.TesauroProyecto, objeto => objeto.Proyecto.ProyectoID, tesauroProy => tesauroProy.ProyectoID, (objeto, tesuaroProy) => new
                    {
                        Proyecto = objeto.Proyecto,
                        AdministradorProyecto = objeto.AdministradorProyecto,
                        TesauroProyecto = tesuaroProy
                    }).Join(mEntityContext.CategoriaTesauro, objeto => objeto.TesauroProyecto.TesauroID, catTes => catTes.TesauroID, (objeto, catTes) => new
                    {
                        Proyecto = objeto.Proyecto,
                        AdministradorProyecto = objeto.AdministradorProyecto,
                        TesauroProyecto = objeto.TesauroProyecto,
                        CategoriaTesauro = catTes
                    }).Where(objeto => objeto.AdministradorProyecto.UsuarioID.Equals(pUsuarioID)).Select(objeto => objeto.Proyecto)
                    ).OrderBy(objeto => objeto.Nombre).ToList().Distinct().ToList();

            }

            return (dataWrapperProyecto);
        }

        /// <summary>
        /// Obtiene la lista de proyectos en las que participa un usuario en modo personal o profesional personal
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <returns>Dataset de proyecto con todos los proyectos en los que participa el usuario</returns>
        public DataWrapperProyecto ObtenerProyectosParticipaUsuarioEnModoPersonal(Guid pUsuarioID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            mEntityContext.Database.SetCommandTimeout(1);

            var resultadoConsulta = mEntityContext.Proyecto.Join(mEntityContext.ProyectoUsuarioIdentidad, proyecto => new { proyecto.OrganizacionID, proyecto.ProyectoID }, proyectoUsuarioIden => new { OrganizacionID = proyectoUsuarioIden.OrganizacionGnossID, proyectoUsuarioIden.ProyectoID }, (proyecto, proyectoUsuarioIden) => new
            {
                Proyecto = proyecto,
                ProyectoUsuarioIdentidad = proyectoUsuarioIden
            }).Join(mEntityContext.ProyectoRolUsuario, objeto => new { objeto.Proyecto.OrganizacionID, objeto.Proyecto.ProyectoID, objeto.ProyectoUsuarioIdentidad.UsuarioID }, proyRolUser => new { OrganizacionID = proyRolUser.OrganizacionGnossID, proyRolUser.ProyectoID, proyRolUser.UsuarioID }, (objeto, proyRolUser) => new
            {
                Proyecto = objeto.Proyecto,
                ProyectoUsuarioIdentidad = objeto.ProyectoUsuarioIdentidad,
                ProyectoRolUsuario = proyRolUser
            }).Join(mEntityContext.Identidad, objeto => objeto.ProyectoUsuarioIdentidad.IdentidadID, identidad => identidad.IdentidadID, (objeto, identidad) => new
            {
                Proyecto = objeto.Proyecto,
                ProyectoUsuarioIdentidad = objeto.ProyectoUsuarioIdentidad,
                ProyectoRolUsuario = objeto.ProyectoRolUsuario,
                Identidad = identidad
            })
            .Where(objeto => objeto.ProyectoRolUsuario.UsuarioID.Equals(pUsuarioID) && (objeto.Proyecto.Estado.Equals((short)EstadoProyecto.Abierto) || objeto.Proyecto.Estado.Equals((short)EstadoProyecto.Cerrandose)) && objeto.ProyectoRolUsuario.EstaBloqueado == false && (objeto.Identidad.Tipo < 2 || objeto.Identidad.Tipo == 4) && !objeto.Identidad.FechaBaja.HasValue && !objeto.Identidad.FechaExpulsion.HasValue).OrderBy(objeto => objeto.Proyecto.Nombre);

            dataWrapperProyecto.ListaProyecto = resultadoConsulta.Select(objeto => objeto.Proyecto).ToList().Distinct().ToList();

            return (dataWrapperProyecto);
        }

        /// <summary>
        /// Obtiene la lista de proyectos en las que participa un usuario pasado por parámetro
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <param name="pSoloUsuariosSinBloquear">Indique si deben traerse solo los usuarios sin bloquear o todos</param>
        /// <returns>Dataset de proyecto con todos los proyectos en los que participa el usuario</returns>
        public DataWrapperProyecto ObtenerProyectosParticipaUsuario(Guid pUsuarioID, bool pSoloUsuariosSinBloquear)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            string sqlParaProyecto = sqlSelectProyectosParticipaUsuario;
            var lista = mEntityContext.Proyecto.Join(mEntityContext.ProyectoUsuarioIdentidad, proyecto => new { proyecto.OrganizacionID, proyecto.ProyectoID }, proyUserIden => new { OrganizacionID = proyUserIden.OrganizacionGnossID, proyUserIden.ProyectoID }, (proyecto, proyUserIden) => new
            {
                Proyecto = proyecto,
                ProyectoUsuarioIdentidad = proyUserIden
            }).Where(objeto => objeto.ProyectoUsuarioIdentidad.UsuarioID.Equals(pUsuarioID)).OrderBy(objeto => objeto.Proyecto.Nombre).Select(objeto => objeto.Proyecto).ToList();
            if (pSoloUsuariosSinBloquear)
            {

                sqlParaProyecto = sqlSelectProyectosParticipaUsuarioSinBloquear;
                lista = mEntityContext.Proyecto.Join(mEntityContext.ProyectoUsuarioIdentidad, proyecto => new { proyecto.OrganizacionID, proyecto.ProyectoID }, proyUserIden => new { OrganizacionID = proyUserIden.OrganizacionGnossID, proyUserIden.ProyectoID }, (proyecto, proyUserIden) => new
                {
                    Proyecto = proyecto,
                    ProyectoUsuarioIdentidad = proyUserIden
                }).Join(mEntityContext.ProyectoRolUsuario, objeto => new { objeto.Proyecto.ProyectoID, objeto.Proyecto.OrganizacionID }, proyectoRolIden => new { proyectoRolIden.ProyectoID, OrganizacionID = proyectoRolIden.OrganizacionGnossID }, (objeto, proyectoRolIden) => new
                {
                    Proyecto = objeto.Proyecto,
                    ProyectoUsuarioIdentidad = objeto.ProyectoUsuarioIdentidad,
                    ProyectoRolUsuario = proyectoRolIden
                }).Where(objeto => objeto.ProyectoUsuarioIdentidad.UsuarioID.Equals(pUsuarioID) && objeto.ProyectoRolUsuario.EstaBloqueado == false).OrderBy(objeto => objeto.Proyecto.Nombre).Select(objeto => objeto.Proyecto).ToList();
            }

            dataWrapperProyecto.ListaProyecto = lista;

            return (dataWrapperProyecto);
        }

        /// <summary>
        /// Obtiene la lista de proyectos en las que participa un usuario pasado por parámetro.
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <returns>Diccionario ProyectoID,NombreCorto de cada proyecto</returns>
        public Dictionary<Guid, string> ObtenerNombresCortosProyectosParticipaUsuarioSinBloquearNiAbandonar(Guid pUsuarioID)
        {
            var resParaProyecto = mEntityContext.Proyecto.Join(mEntityContext.Identidad, proyecto => new { proyecto.ProyectoID, proyecto.OrganizacionID }, identidad => new { identidad.ProyectoID, identidad.OrganizacionID }, (proyecto, identidad) => new
            {
                Proyecto = proyecto,
                Identidad = identidad
            }).Join(mEntityContext.Perfil, objeto => objeto.Identidad.PerfilID, perfil => perfil.PerfilID, (objeto, perfil) => new
            {
                Proyecto = objeto.Proyecto,
                Identidad = objeto.Identidad,
                Perfil = perfil
            }).Join(mEntityContext.Persona, objeto => objeto.Perfil.PersonaID.Value, persona => persona.PersonaID, (objeto, persona) => new
            {
                Proyecto = objeto.Proyecto,
                Identidad = objeto.Identidad,
                Perfil = objeto.Perfil,
                Persona = persona
            }).Where(objeto => objeto.Persona.UsuarioID.HasValue && objeto.Persona.UsuarioID.Value.Equals(pUsuarioID) && objeto.Perfil.PersonaID.HasValue && !objeto.Identidad.FechaBaja.HasValue && !objeto.Identidad.FechaExpulsion.HasValue).Select(objeto => new { ProyectoID = objeto.Proyecto.ProyectoID, NombreCorto = objeto.Proyecto.NombreCorto }).ToList();

            Dictionary<Guid, string> proys = new Dictionary<Guid, string>();

            foreach (var fila in resParaProyecto)
            {
                if (!proys.ContainsKey(fila.ProyectoID))
                {
                    proys.Add(fila.ProyectoID, fila.NombreCorto);
                }
            }

            return proys;
        }

        // TODO Pasar a EF
        /// <summary>
        /// Obtiene la lista de proyectos en las que participa un usuario pasado por parámetro.
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <param name="pTipoDoc">Tipo de documento que se va a cargar</param>
        /// <returns>Diccionario ProyectoID,NombreCorto de cada proyecto</returns>
        public Dictionary<Guid, string> ObtenerNombresCortosProyectosParticipaUsuarioSinBloquearNiAbandonarYConfigurables(Guid pUsuarioID)
        {
            DataSet proyectoDS = new DataSet();

            //Consulta que devuelve los proyectos a los que pertenece el usuario y puede cargar recursos a través del programa de carga masiva
            string sqlCargasMasivas = "SELECT Proyecto.ProyectoID, NombreCorto FROM Proyecto INNER JOIN Identidad ON (Proyecto.ProyectoID=Identidad.ProyectoID AND Proyecto.OrganizacionID=Identidad.OrganizacionID) INNER JOIN Perfil ON (Identidad.PerfilID=Perfil.PerfilID) INNER JOIN Persona ON (Perfil.PersonaID=Persona.PersonaID) INNER JOIN TipoDocDispRolUsuarioProy ON (Proyecto.ProyectoID=TipoDocDispRolUsuarioProy.ProyectoID) WHERE Persona.UsuarioID = " + IBD.GuidValor(pUsuarioID) + " AND Identidad.FechaBaja IS NULL AND Identidad.FechaExpulsion IS NULL AND TipoDocDispRolUsuarioProy.TipoDocumento = " + (int)TiposDocumentacion.CargasMasivas + " AND TipoDocDispRolUsuarioProy.RolUsuario >= (";

            //Subconsulta que devuelve el rol del usuario del proyecto o pro defecto siempre 2.
            string subConsulta = "(SELECT coalesce(AdministradorProyecto.Tipo , 2) FROM AdministradorProyecto WHERE AdministradorProyecto.ProyectoID = Proyecto.ProyectoID AND AdministradorProyecto.UsuarioID =" + IBD.GuidValor(pUsuarioID) + ") UNION (SELECT 2 FROM AdministradorProyecto WHERE not exists(SELECT coalesce(AdministradorProyecto.Tipo , 2) FROM AdministradorProyecto WHERE AdministradorProyecto.ProyectoID = Proyecto.ProyectoID AND AdministradorProyecto.UsuarioID =" + IBD.GuidValor(pUsuarioID) + "))";

            //Agnadimos la subconsulta que devuelve 2 o el rol de usuario en un determinado proyecto.
            sqlCargasMasivas += subConsulta + ") Group by Proyecto.ProyectoID, NombreCorto";

            //Consulta que devuelve los proyectos a los que pertenece el usuario y se pueden cargar recursos de cualquir tipo (imagenes/vídeos/archivos... etc)
            string sqlTipoDoc = "SELECT Proyecto.ProyectoID, NombreCorto FROM Proyecto INNER JOIN Identidad ON (Proyecto.ProyectoID=Identidad.ProyectoID AND Proyecto.OrganizacionID=Identidad.OrganizacionID) INNER JOIN Perfil ON (Identidad.PerfilID=Perfil.PerfilID) INNER JOIN Persona ON (Perfil.PersonaID=Persona.PersonaID) INNER JOIN TipoDocDispRolUsuarioProy ON (Proyecto.ProyectoID=TipoDocDispRolUsuarioProy.ProyectoID) WHERE Persona.UsuarioID = " + IBD.GuidValor(pUsuarioID) + " AND Identidad.FechaBaja IS NULL AND Identidad.FechaExpulsion IS NULL AND TipoDocDispRolUsuarioProy.TipoDocumento = " + (int)TiposDocumentacion.FicheroServidor + " AND TipoDocDispRolUsuarioProy.RolUsuario >= (" + subConsulta + ") Group by Proyecto.ProyectoID, NombreCorto";

            //Consulta que devuelve los proyectos en los que el usuario es el administrador
            string sqlAdministra = "SELECT Proyecto.ProyectoID, NombreCorto FROM Proyecto INNER JOIN AdministradorProyecto ON Proyecto.ProyectoID = AdministradorProyecto.ProyectoID INNER JOIN Identidad ON (Proyecto.ProyectoID=Identidad.ProyectoID AND Proyecto.OrganizacionID=Identidad.OrganizacionID) INNER JOIN Perfil ON (Identidad.PerfilID=Perfil.PerfilID) INNER JOIN Persona ON (Perfil.PersonaID=Persona.PersonaID AND Persona.UsuarioID=AdministradorProyecto.UsuarioID)  WHERE AdministradorProyecto.Tipo = " + (short)TipoRolUsuario.Administrador + " AND AdministradorProyecto.UsuarioID = " + IBD.GuidValor(pUsuarioID) + " AND Proyecto.ProyectoID != " + IBD.GuidValor(ProyectoAD.MetaProyecto) + " AND Identidad.FechaBaja IS NULL AND Identidad.FechaExpulsion IS NULL ";

            //INTERSECT entre los proyectos a los que el usuario puede subir recursos y a los que se pueden subir cualquier tipo de recursos UNION con los proyectos que administra el usuario.
            string sqlQuery = "(" + sqlCargasMasivas + " INTERSECT " + sqlTipoDoc + ") UNION " + sqlAdministra;

            DbCommand commandsqlSelectProyectosParticipaUsuario = ObtenerComando(sqlQuery);
            CargarDataSet(commandsqlSelectProyectosParticipaUsuario, proyectoDS, "Proyecto");

            Dictionary<Guid, string> proys = new Dictionary<Guid, string>();

            foreach (DataRow fila in proyectoDS.Tables[0].Rows)
            {
                if (!proys.ContainsKey((Guid)fila["ProyectoID"]))
                {
                    proys.Add((Guid)fila["ProyectoID"], (string)fila["NombreCorto"]);
                }
            }

            proyectoDS.Dispose();
            return proys;
        }


        /// <summary>
        /// Obtiene el UsuarioID de todos los miembros de un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <returns>Lista de UsuarioID</returns>
        public List<Guid> ObtenerUsuarioIDMiembrosProyecto(Guid pProyectoID)
        {
            List<Guid> resultados = mEntityContext.ProyectoUsuarioIdentidad.Join(mEntityContext.Identidad, proyectoUsuarioID => proyectoUsuarioID.IdentidadID, identidad => identidad.IdentidadID, (proyectoUsuarioID, identidad) => new { ProyectoUsuarioIdentidad = proyectoUsuarioID, Identidad = identidad }).Where(item => item.Identidad.ProyectoID.Equals(pProyectoID) && item.Identidad.FechaBaja == null && item.Identidad.FechaExpulsion == null).Select(proyecto => proyecto.ProyectoUsuarioIdentidad.UsuarioID).ToList();

            return resultados;
        }

        /// <summary>
        /// Obtiene los proyectos administrados por el perfil pasado por parámetro
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <returns>Diccionario ProyectoID,NombreCorto de cada proyecto</returns>
        public Dictionary<Guid, string> ObtenerNombresCortosProyectosAdministraUsuarioSinBloquearNiAbandonar(Guid pUsuarioID, bool ecosistema)
        {
            DataSet proyectoDS = new DataSet();

            string fromProyecto = " FROM Proyecto INNER JOIN AdministradorProyecto ON Proyecto.ProyectoID = AdministradorProyecto.ProyectoID INNER JOIN Identidad ON (Proyecto.ProyectoID=Identidad.ProyectoID AND Proyecto.OrganizacionID=Identidad.OrganizacionID) INNER JOIN Perfil ON (Identidad.PerfilID=Perfil.PerfilID) INNER JOIN Persona ON (Perfil.PersonaID=Persona.PersonaID AND Persona.UsuarioID=AdministradorProyecto.UsuarioID)";
            string whereProyecto = " WHERE AdministradorProyecto.Tipo = " + (short)TipoRolUsuario.Administrador + " AND AdministradorProyecto.UsuarioID = " + IBD.GuidValor(pUsuarioID);
            if (!ecosistema)
            {
                whereProyecto = whereProyecto + " AND Proyecto.ProyectoID != " + IBD.GuidValor(ProyectoAD.MetaProyecto);
            }

            whereProyecto = whereProyecto + " AND Identidad.FechaBaja IS NULL AND Identidad.FechaExpulsion IS NULL ";


            //Proyecto
            DbCommand commandsqlSelectProyecto = ObtenerComando("SELECT Proyecto.ProyectoID, NombreCorto" + fromProyecto + whereProyecto);

            var resultadoConsulta = mEntityContext.Proyecto.Join(mEntityContext.AdministradorProyecto, proyecto => proyecto.ProyectoID, adminProy => adminProy.ProyectoID, (proyecto, adminProy) => new
            {
                Proyecto = proyecto,
                AdministradorProyecto = adminProy
            }).Join(mEntityContext.Identidad, objeto => new { objeto.Proyecto.ProyectoID, objeto.Proyecto.OrganizacionID }, identidad => new { identidad.ProyectoID, identidad.OrganizacionID }, (objeto, identidad) => new
            {
                Proyecto = objeto.Proyecto,
                AdministradorProyecto = objeto.AdministradorProyecto,
                Identidad = identidad
            }).Join(mEntityContext.Perfil, objeto => objeto.Identidad.PerfilID, perfil => perfil.PerfilID, (objeto, perfil) => new
            {
                Proyecto = objeto.Proyecto,
                AdministradorProyecto = objeto.AdministradorProyecto,
                Identidad = objeto.Identidad,
                Perfil = perfil
            }).Join(mEntityContext.Persona, objeto => new { PersonaID = objeto.Perfil.PersonaID.Value, objeto.AdministradorProyecto.UsuarioID }, persona => new { persona.PersonaID, UsuarioID = persona.UsuarioID.Value }, (objeto, persona) => new
            {
                Proyecto = objeto.Proyecto,
                AdministradorProyecto = objeto.AdministradorProyecto,
                Identidad = objeto.Identidad,
                Perfil = objeto.Perfil,
                persona = persona,
                UsuarioID = persona.UsuarioID
            });

            var resultadoFinal = resultadoConsulta.Where(objeto => objeto.AdministradorProyecto.Tipo.Equals((short)TipoRolUsuario.Administrador) && objeto.Perfil.PersonaID.HasValue && objeto.AdministradorProyecto.UsuarioID.Equals(pUsuarioID) && objeto.UsuarioID.HasValue && !objeto.Identidad.FechaBaja.HasValue && !objeto.Identidad.FechaExpulsion.HasValue).Select(objeto => new
            {
                ProyectoID = objeto.Proyecto.ProyectoID,
                NombreCorto = objeto.Proyecto.NombreCorto
            }).ToList();

            if (!ecosistema)
            {
                resultadoFinal = resultadoConsulta.Where(objeto => objeto.AdministradorProyecto.Tipo.Equals((short)TipoRolUsuario.Administrador) && objeto.Perfil.PersonaID.HasValue && objeto.AdministradorProyecto.UsuarioID.Equals(pUsuarioID) && objeto.UsuarioID.HasValue && !objeto.Proyecto.ProyectoID.Equals(ProyectoAD.MetaProyecto) && !objeto.Identidad.FechaBaja.HasValue && !objeto.Identidad.FechaExpulsion.HasValue).Select(objeto => new
                {
                    ProyectoID = objeto.Proyecto.ProyectoID,
                    NombreCorto = objeto.Proyecto.NombreCorto
                }).ToList();
            }


            Dictionary<Guid, string> proys = new Dictionary<Guid, string>();

            foreach (var fila in resultadoFinal)
            {
                if (!proys.ContainsKey(fila.ProyectoID))
                {
                    proys.Add(fila.ProyectoID, fila.NombreCorto);
                }
            }
            return proys;
        }

        /// <summary>
        /// Obtiene la lista de proyectos en las que participa un usuario pasado por parámetro
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <param name="pPerfilID">Identificador de perfil</param>
        /// <returns>Dataset de proyecto con todos los proyectos en los que participa el usuario</returns>
        public DataWrapperProyecto ObtenerProyectosParticipaUsuarioConPerfil(Guid pUsuarioID, Guid pPerfilID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            dataWrapperProyecto.ListaProyecto = mEntityContext.Proyecto.Join(mEntityContext.ProyectoUsuarioIdentidad, proyecto => new { proyecto.OrganizacionID, proyecto.ProyectoID }, proyuserIden => new { OrganizacionID = proyuserIden.OrganizacionGnossID, proyuserIden.ProyectoID }, (proyecto, proyuserIden) => new
            {
                Proyecto = proyecto,
                ProyectoUsuarioIdentidad = proyuserIden
            }).Join(mEntityContext.Identidad, objeto => objeto.ProyectoUsuarioIdentidad.IdentidadID, identidad => identidad.IdentidadID, (objeto, identidad) => new
            {
                Proyecto = objeto.Proyecto,
                ProyectoUsuarioIdentidad = objeto.ProyectoUsuarioIdentidad,
                Identidad = identidad
            }).Where(objeto => objeto.ProyectoUsuarioIdentidad.UsuarioID.Equals(pUsuarioID) && objeto.Identidad.PerfilID.Equals(pPerfilID) && (objeto.Proyecto.Estado.Equals((short)EstadoProyecto.Abierto) || objeto.Proyecto.Estado.Equals((short)EstadoProyecto.Cerrandose))).OrderBy(objeto => objeto.Proyecto.Nombre).Select(objeto => objeto.Proyecto).ToList();

            return (dataWrapperProyecto);
        }

        /// <summary>
        /// Obtiene los proyectos en los que el administrador es un usuario pasado como parámetro
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <returns>Dataset de proyecto con todos los proyectos en los que el usuario es administrador</returns>
        public DataWrapperProyecto ObtenerAdministradorProyectoDeUsuario(Guid pUsuarioID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            DbCommand commandSQL = ObtenerComando(SelectAdministradorProyecto + " FROM AdministradorProyecto WHERE (UsuarioID = " + IBD.GuidParamValor("usuarioID") + ")");
            dataWrapperProyecto.ListaAdministradorProyecto = mEntityContext.AdministradorProyecto.Where(adminProy => adminProy.UsuarioID.Equals(pUsuarioID)).ToList();

            return (dataWrapperProyecto);
        }

        /// <summary>
        /// Obtiene los registros de AdministradorProyecto de una persona pasado como parámetro
        /// </summary>
        /// <param name="pPersonaID">Identificador de la persona</param>
        /// <returns>Dataset de proyecto</returns>
        public DataWrapperProyecto ObtenerAdministradorProyectoDePersona(Guid pPersonaID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            dataWrapperProyecto.ListaAdministradorProyecto = mEntityContext.AdministradorProyecto.Join(mEntityContext.Persona, adminProy => new { UsuarioID = adminProy.UsuarioID }, persona => new { UsuarioID = persona.UsuarioID.Value }, (adminProy, persona) => new
            {
                AdministradorProyecto = adminProy,
                Persona = persona
            }).Where(objeto => objeto.Persona.UsuarioID.HasValue && objeto.Persona.PersonaID.Equals(pPersonaID)).Select(objeto => objeto.AdministradorProyecto).ToList();

            return (dataWrapperProyecto);
        }

        /// <summary>
        /// Obtiene el UsuarioID y PerfilID de los administradores de un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <returns>Diccionario con perejas de UsuarioID, PerfilID</returns>
        public Dictionary<Guid, Guid> ObtenerUsuarioIDPerfilIDAdministradoresDeProyecto(Guid pProyectoID)
        {
            Dictionary<Guid, Guid> resultados = new Dictionary<Guid, Guid>();

            var resultado = mEntityContext.AdministradorProyecto.Join(mEntityContext.ProyectoUsuarioIdentidad, adminProy => new { adminProy.OrganizacionID, adminProy.ProyectoID, adminProy.UsuarioID }, proyUserIden => new { OrganizacionID = proyUserIden.OrganizacionGnossID, proyUserIden.ProyectoID, proyUserIden.UsuarioID }, (adminProy, proyUserIden) => new
            {
                AdministradorProyecto = adminProy,
                ProyectoUsuarioIdentidad = proyUserIden
            }).Join(mEntityContext.Identidad, objeto => new { objeto.ProyectoUsuarioIdentidad.IdentidadID, OrganizacionID = objeto.ProyectoUsuarioIdentidad.OrganizacionGnossID, objeto.ProyectoUsuarioIdentidad.ProyectoID }, identidad => new { identidad.IdentidadID, identidad.OrganizacionID, identidad.ProyectoID }, (objeto, identidad) => new
            {
                AdministradorProyecto = objeto.AdministradorProyecto,
                ProyectoUsuarioIdentidad = objeto.ProyectoUsuarioIdentidad,
                Identidad = identidad
            }).Where(objeto => objeto.AdministradorProyecto.Tipo == 0 && objeto.AdministradorProyecto.ProyectoID.Equals(pProyectoID)).ToList();

            foreach (var fila in resultado)
            {
                Guid id = fila.AdministradorProyecto.UsuarioID;
                if (!resultados.ContainsKey(id))
                {
                    resultados.Add(id, fila.Identidad.PerfilID);
                }
            }
            return resultados;
        }

        /// <summary>
        /// Obtiene los datos de la tabla AdministradorProyecto de un proyecto dado
        /// </summary>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <returns>Dataset de proyecto con la tabla AdministradorProyecto cargada</returns>
        public DataWrapperProyecto ObtenerAdministradorProyectoDeProyecto(Guid pProyectoID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();
            dataWrapperProyecto.ListaAdministradorProyecto = mEntityContext.AdministradorProyecto.Where(adminProy => adminProy.ProyectoID.Equals(pProyectoID)).ToList();
            return (dataWrapperProyecto);
        }


        /// <summary>
        /// Obtiene los proyectos hijos de los proyectos que se pasan por parametros
        /// </summary>
        /// <param name="pListaProyectosID">Lista de identificadores de los proyectos</param>
        /// <param name="pUsuarioID">Identificador del usuario actual</param>
        /// <returns>Dataset de proyecto con los proyectos hijos cargados</returns>
        public DataWrapperProyecto ObtenerProyectosHijosDeProyectos(List<Guid> pListaProyectosID, Guid pUsuarioID)
        {
            List<Proyecto> listaProyecto = mEntityContext.Proyecto.Where(proyecto => (!proyecto.TipoAcceso.Equals((short)TipoAcceso.Reservado) || mEntityContext.ProyectoRolUsuario.Any(rolUsuario => rolUsuario.EstaBloqueado == false && rolUsuario.OrganizacionGnossID.Equals(proyecto.OrganizacionID) && proyecto.ProyectoID.Equals(rolUsuario.ProyectoID) && rolUsuario.UsuarioID.Equals(pUsuarioID))) && pListaProyectosID.Contains((Guid)proyecto.ProyectoSuperiorID)).ToList();

            List<Guid> listaProyectos = new List<Guid>();

            foreach (Proyecto filaProy in listaProyecto)
            {
                listaProyectos.Add(filaProy.ProyectoID);
            }

            if (listaProyectos.Count > 0)
            {
                return ObtenerProyectosPorID(listaProyectos);
            }
            else
            {
                DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();
                dataWrapperProyecto.ListaProyecto = new List<Proyecto>();
                return dataWrapperProyecto;
            }
        }

        /// <summary>
        /// Obtiene los proyectos padres de los proyectos que se pasan por parametros
        /// </summary>
        /// <param name="pListaProyectosID">Lista de identificadores de los proyectos</param>
        /// <returns>Dataset de proyecto con los proyectos padres cargados</returns>
        public DataWrapperProyecto ObtenerProyectosPadresDeProyectos(List<Guid> pListaProyectosID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();


            //Hijos            
            List<Guid> subconsulta = mEntityContext.Proyecto.Where(proyecto => proyecto.ProyectoSuperiorID.HasValue && pListaProyectosID.Contains(proyecto
                  .ProyectoID)).Select(proyecto => proyecto.ProyectoSuperiorID.Value).ToList();
            dataWrapperProyecto.ListaProyecto = mEntityContext.Proyecto.Where(proy => subconsulta.Contains(proy.ProyectoID)).ToList();
            return dataWrapperProyecto;
        }

        /// <summary>
        /// Obtiene el id del proyecto padre del proyecto pasado por parámetro
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto del cual se quiere obtener el padre</param>
        /// <returns>Id del proyecto padre</returns>
        public Guid ObtenerProyectoPadreIDDeProyecto(Guid pProyectoID)
        {
            return mEntityContext.Proyecto.Where(item => item.ProyectoSuperiorID.HasValue && item.ProyectoID.Equals(pProyectoID)).Select(item => item.ProyectoSuperiorID.Value).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene los proyectos en los que participa una identidad (no muestra los que estén cerrados) pasada por parámetro
        /// </summary>
        /// <param name="pIdentidad">Identificador de la identidad</param>
        /// <returns>Dataset de proyectos con el proyecto en el que participa la identidad</returns>
        public DataWrapperProyecto ObtenerProyectoParticipaIdentidad(Guid pIdentidad)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            dataWrapperProyecto.ListaProyecto = mEntityContext.Proyecto.Join(mEntityContext.ProyectoUsuarioIdentidad, proyecto => new { proyecto.OrganizacionID, proyecto.ProyectoID }, proyUserIden => new { OrganizacionID = proyUserIden.OrganizacionGnossID, proyUserIden.ProyectoID }, (proyecto, proyUserIden) => new
            {
                Proyecto = proyecto,
                ProyectoUsuarioIdentidad = proyUserIden
            }).Join(mEntityContext.Identidad, objeto => objeto.ProyectoUsuarioIdentidad.IdentidadID, identidad => identidad.IdentidadID, (objeto, identidad) => new
            {
                Proyecto = objeto.Proyecto,
                ProyectoUsuarioIdentidad = objeto.ProyectoUsuarioIdentidad,
                Identidad = identidad
            }).Where(objeto => objeto.ProyectoUsuarioIdentidad.IdentidadID.Equals(pIdentidad) && (objeto.Proyecto.Estado.Equals((short)EstadoProyecto.Abierto) || objeto.Proyecto.Estado.Equals((short)EstadoProyecto.Cerrandose)) && !objeto.Identidad.FechaBaja.HasValue).Select(objeto => objeto.Proyecto).ToList().Distinct().ToList();

            return (dataWrapperProyecto);
        }

        /// <summary>
        /// Comprueba si el usuario es administrador del metaproyecto "MYGNOSS"
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <returns>TRUE si lo es, FALSE en caso contrario</returns>
        public bool EsUsuarioAdministradorProyectoMYGnoss(Guid pUsuarioID)
        {
            List<AdministradorProyecto> listaAdministradorProyecto = mEntityContext.AdministradorProyecto.Where(administradorProy => administradorProy.ProyectoID.Equals(MetaProyecto) && administradorProy.UsuarioID.Equals(pUsuarioID) && administradorProy.Tipo.Equals((short)TipoRolUsuario.Administrador)).ToList();
            bool EsAdministrador = (listaAdministradorProyecto.Count > 0);

            return (EsAdministrador);
        }


        /// <summary>
        /// Comprueba si el usuario esta bloqueado en el proyecto proyecto
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <returns>TRUE si lo es, FALSE en caso contrario</returns>
        public bool EstaUsuarioBloqueadoEnProyecto(Guid pUsuarioID, Guid pProyectoID)
        {
            return mEntityContext.ProyectoRolUsuario.Any(proyecto => proyecto.ProyectoID.Equals(pProyectoID) && proyecto.UsuarioID.Equals(pUsuarioID) && proyecto.EstaBloqueado == true);
        }

        /// <summary>
        /// Obtiene los proyectos (carga ligera de "Proyecto" y sus administradores "AdministradorProyecto") 
        /// en los que un usuario de organización participa con el perfil de la organización pasada por parámetro
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <param name="pUsuarioID">Identificador del usuario de la organización</param>
        /// <returns>Dataset de proyectos</returns>
        public DataWrapperProyecto ObtenerProyectosParticipaUsuarioDeLaOrganizacion(Guid pOrganizacionID, Guid pUsuarioID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            List<Guid> listaProyectoID = mEntityContext.ProyectoUsuarioIdentidad.Join(mEntityContext.Identidad, proyUserIden => proyUserIden.IdentidadID, identidad => identidad.IdentidadID, (proyUserIden, identidad) => new
            {
                ProyectoUsuarioIdentidad = proyUserIden,
                Identidad = identidad
            }).Join(mEntityContext.PerfilPersonaOrg, objeto => objeto.Identidad.PerfilID, perfilPersonaOrg => perfilPersonaOrg.PerfilID, (objeto, perfilPersonaOrg) => new
            {
                ProyectoUsuarioIdentidad = objeto.ProyectoUsuarioIdentidad,
                Identidad = objeto.Identidad,
                PerfilPersonaOrg = perfilPersonaOrg
            }).Where(objeto => objeto.ProyectoUsuarioIdentidad.UsuarioID.Equals(pUsuarioID) && objeto.PerfilPersonaOrg.OrganizacionID.Equals(pOrganizacionID) && !objeto.ProyectoUsuarioIdentidad.ProyectoID.Equals(MetaProyecto)).Select(objeto => objeto.ProyectoUsuarioIdentidad.ProyectoID).ToList();
            dataWrapperProyecto.ListaProyecto = mEntityContext.Proyecto.Where(proyecto => listaProyectoID.Contains(proyecto.ProyectoID)).ToList().Distinct().ToList();

            //AdministradorProyecto
            //SelectAdministradorProyecto + " FROM AdministradorProyecto WHERE AdministradorProyecto.ProyectoID IN (SELECT ProyectoUsuarioIdentidad.ProyectoID FROM ProyectoUsuarioIdentidad INNER JOIN Identidad ON Identidad.IdentidadID = ProyectoUsuarioIdentidad.IdentidadID INNER JOIN PerfilPersonaOrg ON PerfilPersonaOrg.PerfilID = Identidad.PerfilID WHERE ProyectoUsuarioIdentidad.UsuarioID = " + IBD.GuidParamValor("usuarioID") + " AND PerfilPersonaOrg.OrganizacionID = " + IBD.GuidParamValor("organizacionID") + " AND ProyectoUsuarioIdentidad.ProyectoID <> '" + MetaProyecto + "' )";

            List<Guid> listaAdminProyectoID = mEntityContext.ProyectoUsuarioIdentidad.Join(mEntityContext.Identidad, proyectoUsuarioIden => proyectoUsuarioIden.IdentidadID, identidad => identidad.IdentidadID, (proyectoUsuarioIden, identidad) => new
            {
                ProyectoUsuarioIdentidad = proyectoUsuarioIden,
                Identidad = identidad
            }).Join(mEntityContext.PerfilPersonaOrg, objeto => objeto.Identidad.PerfilID, perfilPersonaOrg => perfilPersonaOrg.PerfilID, (objeto, perfilPersonaOrg) => new
            {
                ProyectoUsuarioIdentidad = objeto.ProyectoUsuarioIdentidad,
                Identidad = objeto.Identidad,
                PerfilPersonaOrg = perfilPersonaOrg
            }).Where(objeto => objeto.ProyectoUsuarioIdentidad.UsuarioID.Equals(pUsuarioID) && objeto.PerfilPersonaOrg.OrganizacionID.Equals(pOrganizacionID) && !objeto.ProyectoUsuarioIdentidad.ProyectoID.Equals(MetaProyecto)).Select(objeto => objeto.ProyectoUsuarioIdentidad.ProyectoID).ToList();
            dataWrapperProyecto.ListaAdministradorProyecto = mEntityContext.AdministradorProyecto.Where(adminProy => listaAdminProyectoID.Contains(adminProy.ProyectoID)).ToList();

            return dataWrapperProyecto;
        }

        /// <summary>
        /// Comprueba si el usuario es administrador del proyecto
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pTipo">Tipo del rol que se quiere comprobar</param>
        /// <returns>TRUE si lo es, FALSE en caso contrario</returns>
        public bool EsUsuarioAdministradorProyecto(Guid pUsuarioID, Guid pProyectoID, TipoRolUsuario pTipo)
        {
            List<AdministradorProyecto> listaAdministradorProyecto = mEntityContext.AdministradorProyecto.Where(adminProy => adminProy.ProyectoID.Equals(pProyectoID) && adminProy.UsuarioID.Equals(pUsuarioID) && adminProy.Tipo.Equals((short)pTipo)).ToList();
            bool EsAdministrador = (listaAdministradorProyecto.Count > 0);
            return (EsAdministrador);
        }

        /// <summary>
        /// Comprueba si la identidad es administrador del proyecto
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la identidad</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pTipo">Tipo del rol que se quiere comprobar</param>
        /// <returns>TRUE si lo es, FALSE en caso contrario</returns>
        public bool EsIdentidadAdministradorProyecto(Guid pIdentidadID, Guid pProyectoID, TipoRolUsuario pTipo)
        {
            var listaAdministradorProyectoVar = mEntityContext.AdministradorProyecto.Join(mEntityContext.Persona, adminProy => adminProy.UsuarioID, persona => persona.UsuarioID, (adminProy, persona) => new
            {
                ProyectoID = adminProy.ProyectoID,
                PersonaID = persona.PersonaID,
                UsuarioID = adminProy.UsuarioID,
                Proyecto = adminProy.Proyecto,
                Tipo = adminProy.Tipo
            }).Join(mEntityContext.Perfil, adminProyPer => adminProyPer.PersonaID, perfil => perfil.PersonaID, (adminProyPer, perfil) => new
            {
                ProyectoID = adminProyPer.ProyectoID,
                Proyecto = adminProyPer.Proyecto,
                PersonaID = adminProyPer.PersonaID,
                PerfilID = perfil.PerfilID,
                Tipo = adminProyPer.Tipo
            }).Join(mEntityContext.Identidad, adminProyPerPer => adminProyPerPer.PerfilID, identidad => identidad.PerfilID, (adminProyPerPer, identidad) => new
            {
                ProyectoID = adminProyPerPer.ProyectoID,
                Proyecto = adminProyPerPer.Proyecto,
                PersonaID = adminProyPerPer.PersonaID,
                PerfilID = adminProyPerPer.PerfilID,
                Tipo = adminProyPerPer.Tipo,
                IdentidadID = identidad.IdentidadID
            }).Where(adminProyPerPerIden => adminProyPerPerIden.IdentidadID.Equals(pIdentidadID) && adminProyPerPerIden.ProyectoID.Equals(pProyectoID) && adminProyPerPerIden.Tipo <= (short)pTipo).ToList();
            bool EsAdministrador = (listaAdministradorProyectoVar.ToList().Count > 0);

            return (EsAdministrador);
        }

        /// <summary>
        /// Obtiene nombre completo de un determinado proyecto (pProyectoID)
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto buscado</param>
        /// <returns>Nombre del proyecto</returns>
        public string ObtenerNombreDeProyectoID(Guid pProyectoID)
        {
            string resultado = "";

            string nombre = mEntityContext.Proyecto.Where(proyecto => proyecto.ProyectoID.Equals(pProyectoID)).Select(proyecto => proyecto.Nombre).FirstOrDefault();

            if (nombre != null)
            {
                resultado = (string)nombre;
            }
            return resultado;
        }

        /// <summary>
        /// Obtiene nombre completo de un determinado proyecto (pProyectoID)
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto buscado</param>
        /// <returns>Nombre del proyecto</returns>
        public Guid ObtenerProyectoSuperiorIDDeProyectoID(Guid pProyectoID)
        {

            Guid? proyectoSuperiorID = mEntityContext.Proyecto.Where(proyecto => proyecto.ProyectoID.Equals(pProyectoID)).Select(proyecto => proyecto.ProyectoSuperiorID).FirstOrDefault();

            if (proyectoSuperiorID != null)
            {
                return proyectoSuperiorID.Value;
            }
            return Guid.Empty;
        }

        /// <summary>
        /// Obtiene los proyectos en los que participa una organización
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <param name="pExcluirMyGNOSS">TRUE si se debe de excluir de la consulta el proyecto de MyGnoss, FALSE en caso contrario</param>
        /// <returns>Dataset de proyectos</returns>
        //public DataWrapperProyecto ObtenerProyectosParticipaOrganizacion(Guid pOrganizacionID, bool pExcluirMyGNOSS)
        public DataWrapperProyecto ObtenerProyectosParticipaOrganizacion(Guid pOrganizacionID, bool pExcluirMyGNOSS)
        {
            //TODO
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            List<Proyecto> proyectos = mEntityContext.Proyecto.Join(mEntityContext.OrganizacionParticipaProy, proyecto => new { ProyectoID = proyecto.ProyectoID, OrganizcionID = proyecto.OrganizacionID }, orgParticipaProy => new { ProyectoID = orgParticipaProy.ProyectoID, OrganizcionID = orgParticipaProy.OrganizacionProyectoID }, (proyecto, orgParticipaProy) => new
            {
                Proyecto = proyecto,
                OrganizacionID = orgParticipaProy.OrganizacionID
            }).Where(proyectoOrg => proyectoOrg.OrganizacionID.Equals(pOrganizacionID) && !proyectoOrg.Proyecto.Estado.Equals((short)EstadoProyecto.Cerrado) && !proyectoOrg.Proyecto.Estado.Equals((short)EstadoProyecto.CerradoTemporalmente)).Select(proyectoOrg => proyectoOrg.Proyecto).ToList();
            if (pExcluirMyGNOSS)
            {
                proyectos = mEntityContext.Proyecto.Join(mEntityContext.OrganizacionParticipaProy, proyecto => new { proyecto.ProyectoID, proyecto.OrganizacionID }, orgParticipaProy => new { orgParticipaProy.ProyectoID, orgParticipaProy.OrganizacionID }, (proyecto, orgParticipaProy) => new
                {
                    Proyecto = proyecto,
                    OrganizacionID = orgParticipaProy.OrganizacionID
                }).Where(proyectoOrg => proyectoOrg.OrganizacionID.Equals(pOrganizacionID) && !proyectoOrg.Proyecto.ProyectoID.Equals(ProyectoAD.MetaProyecto) && !proyectoOrg.Proyecto.Estado.Equals((short)EstadoProyecto.Cerrado) && !proyectoOrg.Proyecto.Estado.Equals((short)EstadoProyecto.CerradoTemporalmente)).Select(proyectoOrg => proyectoOrg.Proyecto).ToList();
            }

            dataWrapperProyecto.ListaProyecto = proyectos;
            return (dataWrapperProyecto);
        }

        /// <summary>
        /// Obtiene los proyectos en los que participa una organización ordenados por relevancia (Número de visitas en GNOSS)
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <returns>Dataset de proyectos</returns>
        public DataWrapperProyecto ObtenerProyectosParticipaOrganizacionPorRelevancia(Guid pOrganizacionID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            List<Proyecto> listaProyectos = mEntityContext.Proyecto.Join(mEntityContext.OrganizacionParticipaProy, proyecto => new { proyecto.ProyectoID, ORganizacionID = proyecto.OrganizacionID }, orgPartProy => new { orgPartProy.ProyectoID, ORganizacionID = orgPartProy.OrganizacionProyectoID }, (proyecto, orgPartProy) => new
            {
                Proyecto = proyecto,
                OrganizacionParticipaProy = orgPartProy
            }).Where(objeto => objeto.OrganizacionParticipaProy.OrganizacionID.Equals(pOrganizacionID) && !objeto.Proyecto.Estado.Equals((short)EstadoProyecto.Cerrado) && !objeto.Proyecto.Estado.Equals((short)EstadoProyecto.CerradoTemporalmente)).Select(objeto => objeto.Proyecto).ToList();
            dataWrapperProyecto.ListaProyecto = listaProyectos.Join(mEntityContext.ProyectosMasActivos, proyecto => new { proyecto.ProyectoID, proyecto.OrganizacionID }, proyectoMasActivo => new { proyectoMasActivo.ProyectoID, proyectoMasActivo.OrganizacionID }, (proyecto, proyectoMasActivo) => new
            {
                Proyecto = proyecto,
                ProyectosMasActivos = proyectoMasActivo
            }).OrderByDescending(objeto => objeto.ProyectosMasActivos.Peso).Select(objeto => objeto.Proyecto).ToList();

            return (dataWrapperProyecto);
        }


        /// <summary>
        /// Obtiene una lista con los IDs de los proyectos en los que participa la organizacion
        /// </summary>
        /// <param name="pOrganizacion">Identificador de la organizacion</param>
        /// <param name="pObtenerSoloActivos">Indica si se debe traer los proyectos en los que ya no participa</param>
        /// <returns>Lista con los IDs de los proyectos en los que participa la organizacion</returns>
        public List<Guid> ObtenerListaProyectoIDDeOrganizacion(Guid pOrganizacion, bool pObtenerSoloActivos)
        {
            List<Guid> listaProyectos = new List<Guid>();

            var listaProyectosVAR = mEntityContext.Identidad.Join(mEntityContext.Perfil, identidad => identidad.PerfilID, perfil => perfil.PerfilID, (identidad, perfil) => new
            {
                Perfil = perfil,
                Identidad = identidad
            }).Where(objeto => objeto.Identidad.Tipo.Equals((short)TiposIdentidad.Organizacion) && objeto.Perfil.OrganizacionID.Value.Equals(pOrganizacion)).ToList();

            if (pObtenerSoloActivos)
            {
                listaProyectosVAR = listaProyectosVAR.Where(objeto => objeto.Identidad.FechaBaja == null && objeto.Identidad.FechaExpulsion == null).ToList();
            }
            listaProyectos = listaProyectosVAR.Select(objeto => objeto.Identidad.ProyectoID).Distinct().ToList();

            return listaProyectos;
        }

        /// <summary>
        /// Obtiene los proyectos en los que participa un usuario ordenados por relevancia (Número de visitas del usuario)
        /// </summary>
        /// <param name="pPerfilID">Identificador del perfil</param>
        /// <returns>Dataset de proyectos</returns>
        public DataWrapperProyecto ObtenerProyectosParticipaPerfilUsuarioPorRelevancia(Guid pPerfilID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            dataWrapperProyecto.ListaProyecto = mEntityContext.Proyecto.Join(mEntityContext.Identidad, proyecto => proyecto.ProyectoID, identidad => identidad.ProyectoID, (proyecto, identidad) => new
            {
                Proyecto = proyecto,
                Identidad = identidad
            }).Where(objeto => objeto.Identidad.PerfilID.Equals(pPerfilID) && !objeto.Identidad.FechaBaja.HasValue && !objeto.Identidad.FechaExpulsion.HasValue && !objeto.Proyecto.ProyectoID.Equals(ProyectoAD.MetaProyecto)).OrderByDescending(objeto => objeto.Identidad.NumConnexiones).Select(objeto => objeto.Proyecto).ToList();

            return (dataWrapperProyecto);
        }

        public DataSet ObtenerDatosProyectosDesplegarAcciones(List<Guid> pListaProyectos)
        {
            string listaProyectos = "";
            foreach (Guid proyectoid in pListaProyectos)
            {
                listaProyectos += IBD.GuidValor(proyectoid) + ", ";
            }
            listaProyectos = listaProyectos.Substring(0, listaProyectos.Length - 2);

            string select = "SELECT Proyecto.ProyectoID, Proyecto.TipoProyecto, Proyecto.NombreCorto, ParametroGeneral.MostrarPersonasEnCatalogo, ParametroGeneral.VotacionesDisponibles, ParametroGeneral.PermitirVotacionesNegativas, ParametroGeneral.ComentariosDisponibles, ParametroGeneral.CompartirRecursosPermitido FROM ParametroGeneral INNER JOIN Proyecto ON Proyecto.ProyectoID = ParametroGeneral.ProyectoID WHERE Proyecto.ProyectoID IN (" + listaProyectos + ")";
            DbCommand commandsql = ObtenerComando(select);

            DataSet dataset = new DataSet();
            CargarDataSet(commandsql, dataset, "DatosDesplegarAcciones");

            return dataset;
        }

        /// <summary>
        /// Obtiene los proyectos (carga ligera de "Proyecto" y sus administradores "AdministradorProyecto")
        /// en los que el usuario participa con el perfil personal
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <returns>Dataset de proyectos</returns>
        public DataWrapperProyecto ObtenerProyectosParticipaUsuarioConSuPerfilPersonal(Guid pUsuarioID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            List<Guid> listaProyectoID = mEntityContext.ProyectoUsuarioIdentidad.Join(mEntityContext.Identidad, proyecto => proyecto.IdentidadID, identidad => identidad.IdentidadID, (proyecto, identidad) => new
            {
                ProyectoUsuarioIdentidad = proyecto,
                Identidad = identidad
            }).Join(mEntityContext.PerfilPersona, objeto => objeto.Identidad.PerfilID, perfil => perfil.PerfilID, (objeto, perfil) => new
            {
                ProyectoUsuarioIdentidad = objeto.ProyectoUsuarioIdentidad,
                Identidad = objeto.Identidad,
                PerfilPersona = perfil
            }).Where(objeto => objeto.ProyectoUsuarioIdentidad.UsuarioID.Equals(pUsuarioID) && !objeto.ProyectoUsuarioIdentidad.ProyectoID.Equals(MetaProyecto)).Select(objeto => objeto.ProyectoUsuarioIdentidad.ProyectoID).ToList();

            dataWrapperProyecto.ListaProyecto = mEntityContext.Proyecto.Where(proyecto => listaProyectoID.Contains(proyecto.ProyectoID)).ToList();

            return dataWrapperProyecto;
        }

        /// <summary>
        /// Obtiene nombre completo de un determinado proyecto (pProyectoID)
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto buscado</param>
        /// <returns>Nombre del proyecto</returns>
        public Dictionary<Guid, string> ObtenerNombreDeProyectosID(List<Guid> pListaProyectosID)
        {
            Dictionary<Guid, string> listaProyectos = new Dictionary<Guid, string>();
            var proyectos = mEntityContext.Proyecto.Where(proyecto => pListaProyectosID.Contains(proyecto.ProyectoID)).Select(proyecto => new { proyecto.ProyectoID, proyecto.Nombre }).ToList();
            foreach (var proyecto in proyectos)
            {
                listaProyectos.Add(proyecto.ProyectoID, proyecto.Nombre);
            }
            return listaProyectos;
        }
        /// <summary>
        /// Obtiene si el usuario participa en el proyecto con alguna de sus identidades
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <returns>TRUE si el usuario participa con alguna de sus identidadez</returns>
        public bool ParticipaUsuarioEnProyecto(Guid pProyectoID, Guid pUsuarioID)
        {
            Object identidades;
            identidades = mEntityContext.Identidad.Join(mEntityContext.Perfil, identidad => identidad.PerfilID, perfil => perfil.PerfilID, (identidad, perfil) => new
            {
                Identidad = identidad,
                Perfil = perfil
            }).Join(mEntityContext.Persona, objeto => new { PersonaID = objeto.Perfil.PersonaID.Value }, persona => new { PersonaID = persona.PersonaID }, (objeto, persona) => new
            {
                Identidad = objeto.Identidad,
                Perfil = objeto.Perfil,
                Persona = persona
            }).Where(objeto => objeto.Perfil.PersonaID.HasValue && objeto.Identidad.ProyectoID.Equals(pProyectoID) && objeto.Persona.UsuarioID.HasValue && objeto.Persona.UsuarioID.Value.Equals(pUsuarioID)).FirstOrDefault();

            return identidades != null;
        }

        /// <summary>
        /// Obtiene el la URL del API de Integracion Continua
        /// </summary>
        /// <param name="pNombre">Nombre corto del proyecto</param>
        /// <returns>Identificador del proyecto</returns>
        public string ObtenerURLApiIntegracionContinua()
        {
            return mEntityContext.ParametroAplicacion.Where(parametroApp => parametroApp.Parametro.Equals("UrlServicioIntegracionEntornos")).Select(parametroApp => parametroApp.Valor).FirstOrDefault();
        }


        /// <summary>
        /// Obtiene el la URL del API de Integracion Continua
        /// </summary>
        /// <param name="pNombre">Nombre corto del proyecto</param>
        /// <returns>Identificador del proyecto</returns>
        public Guid ObtenerEntornoID()
        {
            object resultado = mEntityContext.ParametroAplicacion.Where(parametroApp => parametroApp.Parametro.Equals("EntornoIntegracionContinua")).Select(parametroApp => parametroApp.Valor).FirstOrDefault();

            if (resultado != null)
            {
                return Guid.Parse(resultado.ToString());
            }
            else
            {
                return Guid.Empty;
            }

        }

        /// <summary>
        /// Obtiene el identificador de un proyecto a partir de su nombre CORTO pasado por parámetro
        /// </summary>
        /// <param name="pNombre">Nombre corto del proyecto</param>
        /// <returns>Identificador del proyecto</returns>
        public Guid ObtenerProyectoIDPorNombre(string pNombre)
        {
            return mEntityContext.Proyecto.Where(proyecto => proyecto.NombreCorto.Equals(pNombre)).Select(proyecto => proyecto.ProyectoID).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene el identificador de un proyecto a partir de su nombre CORTO pasado por parámetro
        /// </summary>
        /// <param name="pNombreCorto">Nombre corto del proyecto</param>
        /// <returns>Identificador del proyecto</returns>
        public List<Guid> ObtenerProyectoIDOrganizacionIDPorNombreCorto(string pNombreCorto)
        {
            var resultadoConsulta = mEntityContext.Proyecto.Where(proyecto => proyecto.NombreCorto.Equals(pNombreCorto)).Select(proyecto => new { proyecto.OrganizacionID, proyecto.ProyectoID }).FirstOrDefault();

            List<Guid> resultado = null;

            if (resultadoConsulta != null)
            {
                resultado = new List<Guid>();
                resultado.Add(resultadoConsulta.OrganizacionID);
                resultado.Add(resultadoConsulta.ProyectoID);
            }

            return resultado;
        }

        /// <summary>
        /// Obtiene el identificador de un proyecto a partir de su nombre pasado por parámetro
        /// </summary>
        /// <returns>Identificador del proyecto</returns>
        public Guid ObtenerProyectoIDPorNombreLargo(string pNombre)
        {
            return mEntityContext.Proyecto.Where(proyecto => proyecto.Nombre.Equals(pNombre)).Select(proyecto => proyecto.ProyectoID).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene los proyectos en los que participa un perfil
        /// </summary>
        /// <param name="pPerfilID">Identificador del perfil</param>
        /// <param name="pTipoIdentidad">Tipo de identidad del perfil</param>
        /// <param name="pExcluirMyGNOSS">TRUE si se quiere excluir la metacomunidad, FALSE en caso contrario</param>
        /// <returns>Dataset de proyecto con los proyectos en los que participa un perfil</returns>
        public DataWrapperProyecto ObtenerProyectosParticipaPerfilUsuario(Guid pPerfilID, short pTipoIdentidad, bool pExcluirMyGNOSS, Guid pUsuarioID)
        {
            var listaProyectosIDParte1Var = mEntityContext.Proyecto.Join(mEntityContext.Identidad, proyecto => new { proyecto.ProyectoID, proyecto.OrganizacionID }, identidad => new { identidad.ProyectoID, identidad.OrganizacionID }, (proyecto, identidad) => new
            {
                Proyecto = proyecto,
                Identidad = identidad
            }).Where(proyIden => proyIden.Identidad.PerfilID.Equals(pPerfilID) && proyIden.Proyecto.Estado != (short)EstadoProyecto.Cerrado && proyIden.Proyecto.Estado != (short)EstadoProyecto.CerradoTemporalmente && proyIden.Proyecto.Estado != (short)EstadoProyecto.Definicion && !proyIden.Identidad.FechaBaja.HasValue).ToList();

            var listaProyectosIDParte2Var = mEntityContext.Proyecto.Join(mEntityContext.Identidad, proyecto => new { proyecto.ProyectoID, proyecto.OrganizacionID }, identidad => new { identidad.ProyectoID, identidad.OrganizacionID }, (proyecto, identidad) => new
            {
                Proyecto = proyecto,
                Identidad = identidad
            }).Join(mEntityContext.AdministradorProyecto, proyIden => new { proyIden.Proyecto.OrganizacionID, proyIden.Proyecto.ProyectoID }, adminProy => new { adminProy.OrganizacionID, adminProy.ProyectoID }, (proyIden, adminProy) => new
            {
                Proyecto = proyIden.Proyecto,
                Identidad = proyIden.Identidad,
                AdminProyecto = adminProy
            }).Where(proyIdentAdmin => proyIdentAdmin.Identidad.PerfilID.Equals(pPerfilID) && !proyIdentAdmin.Identidad.FechaBaja.HasValue && !proyIdentAdmin.Proyecto.TipoProyecto.Equals((short)TipoProyecto.MetaComunidad) && proyIdentAdmin.AdminProyecto.UsuarioID.Equals(pUsuarioID) && proyIdentAdmin.AdminProyecto.Tipo.Equals((short)TipoRolUsuario.Administrador)).ToList();

            List<Guid> listaProyectosIDParte2 = listaProyectosIDParte2Var.Select(objeto => objeto.Proyecto.ProyectoID).ToList();
            List<Guid> listaProyectosIDParte1 = listaProyectosIDParte1Var.Select(objeto => objeto.Proyecto.ProyectoID).ToList();

            if (pTipoIdentidad >= 0)
            {
                listaProyectosIDParte1 = listaProyectosIDParte1Var.Where(objeto => objeto.Identidad.Tipo.Equals(pTipoIdentidad)).Select(objeto => objeto.Proyecto.ProyectoID).ToList();
                listaProyectosIDParte2 = listaProyectosIDParte2Var.Where(objeto => objeto.Identidad.Tipo.Equals(pTipoIdentidad)).Select(objeto => objeto.Proyecto.ProyectoID).ToList();
            }
            List<Guid> listaProyectosID = listaProyectosIDParte1.Union(listaProyectosIDParte2).ToList();

            List<Proyecto> listaProyectosParticipaPerfilUsuario = mEntityContext.Proyecto.Where(proy => listaProyectosID.Contains(proy.ProyectoID)).OrderBy(proy => proy.Nombre).ToList();
            if (pExcluirMyGNOSS)
            {
                var listaProyectosIDSinMyGNOSSParte1Var = mEntityContext.Proyecto.Join(mEntityContext.Identidad, proyecto => new { proyecto.ProyectoID, proyecto.OrganizacionID }, identidad => new { identidad.ProyectoID, identidad.OrganizacionID }, (proyecto, identidad) => new
                {
                    Proyecto = proyecto,
                    Identidad = identidad
                }).Where(proyIden => proyIden.Identidad.PerfilID.Equals(pPerfilID) && proyIden.Proyecto.Estado != (short)EstadoProyecto.Cerrado && proyIden.Proyecto.Estado != (short)EstadoProyecto.CerradoTemporalmente && proyIden.Proyecto.Estado != (short)EstadoProyecto.Definicion && !proyIden.Identidad.FechaBaja.HasValue).ToList();

                var listaProyectosIDSinMyGNOSSParte2Var = mEntityContext.Proyecto.Join(mEntityContext.Identidad, proyecto => new { proyecto.ProyectoID, proyecto.OrganizacionID }, identidad => new { identidad.ProyectoID, identidad.OrganizacionID }, (proyecto, identidad) => new
                {
                    Proyecto = proyecto,
                    Identidad = identidad
                }).Join(mEntityContext.AdministradorProyecto, proyIden => new { proyIden.Proyecto.OrganizacionID, proyIden.Proyecto.ProyectoID }, adminProy => new { adminProy.OrganizacionID, adminProy.ProyectoID }, (proyIden, adminProy) => new
                {
                    Proyecto = proyIden.Proyecto,
                    Identidad = proyIden.Identidad,
                    AdminProyecto = adminProy
                }).Where(proyIdentAdmin => proyIdentAdmin.Identidad.PerfilID.Equals(pPerfilID) && !proyIdentAdmin.Identidad.FechaBaja.HasValue && proyIdentAdmin.Proyecto.TipoProyecto != (short)TipoProyecto.MetaComunidad && proyIdentAdmin.AdminProyecto.UsuarioID.Equals(pUsuarioID) && proyIdentAdmin.AdminProyecto.Tipo.Equals((short)TipoRolUsuario.Administrador)).ToList();
                List<Guid> listaProyectosIDSinMyGNOSSParte2 = listaProyectosIDSinMyGNOSSParte2Var.Select(objeto => objeto.Proyecto.ProyectoID).ToList();
                List<Guid> listaProyectosIDSinMyGNOSSParte1 = listaProyectosIDSinMyGNOSSParte1Var.Select(objeto => objeto.Proyecto.ProyectoID).ToList();
                if (pTipoIdentidad >= 0)
                {
                    listaProyectosIDSinMyGNOSSParte1 = listaProyectosIDSinMyGNOSSParte1Var.Where(objeto => objeto.Identidad.Tipo.Equals(pTipoIdentidad)).Select(objeto => objeto.Proyecto.ProyectoID).ToList();
                    listaProyectosIDSinMyGNOSSParte2 = listaProyectosIDSinMyGNOSSParte2Var.Where(objeto => objeto.Identidad.Tipo.Equals(pTipoIdentidad)).Select(objeto => objeto.Proyecto.ProyectoID).ToList();
                }
                List<Guid> listaProyectosSinMyGNOSSID = listaProyectosIDSinMyGNOSSParte1.Union(listaProyectosIDSinMyGNOSSParte2).ToList();

                listaProyectosParticipaPerfilUsuario = mEntityContext.Proyecto.Where(proy => listaProyectosSinMyGNOSSID.Contains(proy.ProyectoID)).OrderBy(proy => proy.Nombre).ToList();
            }

            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();
            dataWrapperProyecto.ListaProyecto = listaProyectosParticipaPerfilUsuario;
            return (dataWrapperProyecto);
        }

        /// <summary>
        /// Obtiene una lista con los proyectos en los que participa un perfil pasandole una de las identidades del perfil(NO incluye myGnoss)
        /// </summary>
        /// /// <param name="listaProyectos">Lista con los proyectos obtenidos de Virtuoso</param>
        /// <param name="pIdentidadID">Identificador de la identidad</param>
        /// <returns>Lista de los proyectos en los que participa un perfil</returns>
        public DataWrapperProyecto ObtenerProyectosParticipaPerfilIdentidad(string listaProyectos, Guid pIdentidadID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();
            List<int> tiposDeAcceso = new List<int>();
            tiposDeAcceso.Add(0); tiposDeAcceso.Add(1); tiposDeAcceso.Add(2);
            List<Guid> listaPerfilID = mEntityContext.Identidad.Where(identidad => identidad.IdentidadID.Equals(pIdentidadID)).Select(identidad => identidad.PerfilID).ToList();
            List<Guid> listaProyectoID = mEntityContext.Identidad.Where(identidad => listaPerfilID.Contains(identidad.PerfilID)).Select(identidad => identidad.ProyectoID).ToList();
            List<Proyecto> proyectos = mEntityContext.Proyecto.Where(proyecto => listaProyectos.Contains(proyecto.ProyectoID.ToString()) && (tiposDeAcceso.Contains(proyecto.TipoAcceso) || listaProyectoID.Contains(proyecto.ProyectoID)) && proyecto.ProyectoID != ProyectoAD.MetaProyecto && (proyecto.Estado.Equals((short)EstadoProyecto.Abierto) || proyecto.Estado.Equals((short)EstadoProyecto.Cerrandose))).ToList();

            dataWrapperProyecto.ListaProyecto = proyectos;
            return dataWrapperProyecto;
        }


        /// <summary>
        /// Obtiene una lista con los proyectos en los que participa un perfil(NO incluye myGnoss)
        /// </summary>
        /// <param name="pPerfilID">Identificador del perfil</param>
        /// <returns>Lista de los proyectos en los que participa un perfil</returns>
        public DataWrapperProyecto ObtenerProyectosParticipaPerfil(Guid pPerfilID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            List<Guid> listaProyectoID = mEntityContext.AdministradorProyecto.Join(mEntityContext.Usuario, adminProy => adminProy.UsuarioID, usuario => usuario.UsuarioID, (adminProy, usuario) => new
            {
                AdminProyecto = adminProy,
                Usuario = usuario
            }).Join(mEntityContext.Persona, objeto => objeto.Usuario.UsuarioID, persona => persona.UsuarioID.Value, (objeto, persona) => new
            {
                AdminProyecto = objeto.AdminProyecto,
                Persona = persona
            }).Join(mEntityContext.Perfil, persona => persona.Persona.PersonaID, perfil => perfil.PersonaID.Value, (persona, perfil) => new
            {
                AdminProyecto = persona.AdminProyecto,
                Perfil = perfil
            }).Where(perfil => perfil.Perfil.PerfilID == pPerfilID).Select(objeto => objeto.AdminProyecto.ProyectoID).ToList();

            var listaProyectosVar = mEntityContext.Proyecto.Join(mEntityContext.Identidad, proyecto => proyecto.ProyectoID, identidad => identidad.ProyectoID, (proyecto, identidad) => new
            {
                Proyecto = proyecto,
                Identidad = identidad
            }).Where(objeto => objeto.Identidad.PerfilID.Equals(pPerfilID) && !objeto.Identidad.FechaBaja.HasValue && !objeto.Identidad.FechaExpulsion.HasValue && objeto.Proyecto.ProyectoID != ProyectoAD.MetaProyecto && (objeto.Proyecto.Estado.Equals((short)EstadoProyecto.Abierto) || objeto.Proyecto.Estado.Equals((short)EstadoProyecto.Cerrandose) || (objeto.Proyecto.Estado.Equals((short)EstadoProyecto.Definicion) && listaProyectoID.Contains(objeto.Proyecto.ProyectoID)))).ToList().Select(objeto => new ProyectoNumConexiones
            {
                ProyectoID = objeto.Proyecto.ProyectoID,
                NombreCorto = objeto.Proyecto.NombreCorto,
                Nombre = objeto.Proyecto.Nombre,
                TipoAcceso = objeto.Proyecto.TipoAcceso,
                NumConexiones = objeto.Identidad.NumConnexiones,
                TipoProyecto = objeto.Proyecto.TipoProyecto
            }).ToList();

            dataWrapperProyecto.ListaProyectoNumConexiones = listaProyectosVar;
            return dataWrapperProyecto;
        }


        /// <summary>
        /// Obtiene una lista con los proyectos que no son de registro obligatorio.
        /// </summary>
        /// <returns>Lista de los proyectos que no son de registro obligatorio</returns>
        public List<Guid> ObtenerListaIDsProyectosSinRegistroObligatorio()
        {
            return mEntityContext.ProyectoSinRegistroObligatorio.Select(proyecto => proyecto.ProyectoID).Distinct().ToList();
        }



        /// <summary>
        /// Obtiene una lista con los proyectos en los que participa un perfil(NO incluye myGnoss)
        /// </summary>
        /// <param name="pPerfilID">Identificador del perfil</param>
        /// <returns>Lista de los proyectos en los que participa un perfil</returns>
        public Dictionary<string, KeyValuePair<string, short>> ObtenerListaProyectosParticipaPerfilUsuario(Guid pPerfilID)
        {
            Dictionary<string, KeyValuePair<string, short>> listaProyectos = new Dictionary<string, KeyValuePair<string, short>>();

            List<Guid> listaGuid = mEntityContext.AdministradorProyecto.Join(mEntityContext.Usuario, adminProy => adminProy.UsuarioID, usuario => usuario.UsuarioID, (adminProy, usuario) => new
            {
                AdminProy = adminProy,
                User = usuario
            }).Join(mEntityContext.Persona, objeto => objeto.User.UsuarioID, persona => persona.UsuarioID, (objeto, persona) => new
            {
                AdminProy = objeto.AdminProy,
                User = objeto.User,
                Persona = persona
            }).Join(mEntityContext.Perfil, objeto => objeto.Persona.PersonaID, perfil => perfil.PersonaID, (objeto, perfil) => new
            {
                AdminProy = objeto.AdminProy,
                PerfilID = perfil.PerfilID
            }).Where(objeto => objeto.PerfilID.Equals(pPerfilID)).Select(objeto => objeto.AdminProy.ProyectoID).ToList();

            var consulta = mEntityContext.Proyecto.Join(mEntityContext.Identidad, proyecto => proyecto.ProyectoID, identidad => identidad.ProyectoID, (proyecto, identidad) => new
            {
                Proyecto = proyecto,
                Identidad = identidad
            }).Where(objeto => objeto.Identidad.PerfilID.Equals(pPerfilID) && !objeto.Identidad.FechaBaja.HasValue && !objeto.Identidad.FechaExpulsion.HasValue && !objeto.Proyecto.ProyectoID.Equals(ProyectoAD.MetaProyecto) && (objeto.Proyecto.Estado.Equals((short)EstadoProyecto.Abierto) || objeto.Proyecto.Estado.Equals((short)EstadoProyecto.Cerrandose) || (objeto.Proyecto.Estado.Equals((short)EstadoProyecto.Definicion) && listaGuid.Contains(objeto.Proyecto.ProyectoID)))).OrderByDescending(objeto => objeto.Identidad.NumConnexiones).Select(objeto => new { objeto.Proyecto.NombreCorto, objeto.Proyecto.Nombre, objeto.Proyecto.TipoAcceso, objeto.Identidad.NumConnexiones });

            foreach (var objeto in consulta.ToList())
            {
                if (!listaProyectos.ContainsKey(objeto.NombreCorto))
                {
                    listaProyectos.Add(objeto.NombreCorto, new KeyValuePair<string, short>(objeto.Nombre, objeto.TipoAcceso));
                }
            }
            return listaProyectos;
        }

        /// <summary>
        /// Obtiene una lista con los identificadores de los proyectos en los que participa el usuario
        /// </summary>
        /// <param name="pUsuarioID">Identificador del Usuario</param>
        /// <returns>Lista con los identificadores de los proyectos en los que participa el Usuario</returns>
        public List<Guid> ObtenerListaIDsProyectosParticipaUsuario(Guid pUsuarioID)
        {
            List<Guid> listaProyecto;

            List<Guid> listaPerfilID = mEntityContext.Perfil.Join(mEntityContext.Persona, perfil => perfil.PersonaID.Value, persona => persona.PersonaID, (perfil, persona) => new
            {
                Perfil = perfil,
                Persona = persona
            }).Where(objeto => objeto.Persona.UsuarioID.Value.Equals(pUsuarioID)).Select(objeto => objeto.Perfil.PerfilID).ToList();

            List<Guid?> listaProyectoID = mEntityContext.AdministradorProyecto.Join(mEntityContext.Usuario, adminProy => adminProy.UsuarioID, usuario => usuario.UsuarioID, (adminProy, usuario) => new
            {
                AdminProy = adminProy,
                User = usuario
            }).Join(mEntityContext.Persona, objeto => objeto.User.UsuarioID, persona => persona.UsuarioID.Value, (objeto, persona) => new
            {
                AdminProy = objeto.AdminProy,
                User = objeto.User,
                Persona = persona
            }).Join(mEntityContext.Perfil, objeto => objeto.Persona.PersonaID, perfil => perfil.PersonaID.Value, (objeto, perfil) => new
            {
                UsurioID = objeto.Persona.UsuarioID,
                ProyectoID = objeto.AdminProy.ProyectoID
            }).Where(objeto => objeto.UsurioID.Value.Equals(pUsuarioID)).Select(objeto => objeto.UsurioID).ToList();

            listaProyecto = mEntityContext.Proyecto.Join(mEntityContext.Identidad, proyecto => proyecto.ProyectoID, identidad => identidad.ProyectoID, (proyecto, identidad) => new
            {
                Proyecto = proyecto,
                Identidad = identidad
            }).Where(objeto => listaPerfilID.Contains(objeto.Identidad.PerfilID) && !objeto.Identidad.FechaBaja.HasValue && !objeto.Identidad.FechaExpulsion.HasValue && !objeto.Proyecto.ProyectoID.Equals(ProyectoAD.MetaProyecto) && (objeto.Proyecto.Estado.Equals((short)EstadoProyecto.Abierto) || objeto.Proyecto.Estado.Equals((short)EstadoProyecto.Cerrandose) || (objeto.Proyecto.Estado.Equals((short)EstadoProyecto.Definicion)) && listaProyectoID.Contains(objeto.Proyecto.ProyectoID))).OrderBy(objeto => objeto.Identidad.NumConnexiones).Select(objeto => objeto.Proyecto.ProyectoID).Distinct().ToList();

            return listaProyecto;
        }

        /// <summary>
        /// Obtiene una lista con los proyectos en los que participa el usuarioID
        /// </summary>
        /// <param name="pUsuarioID">Identificado del Usuario</param>
        /// <returns>Lista de los proyectos en los que participa el Usuario</returns>
        public Dictionary<string, KeyValuePair<string, short>> ObtenerListaProyectosParticipaUsuario(Guid pUsuarioID)
        {
            Dictionary<string, KeyValuePair<string, short>> listaProyectos = new Dictionary<string, KeyValuePair<string, short>>();

            List<Guid> listaPerfilID = mEntityContext.Perfil.Join(mEntityContext.Persona, perfil => perfil.PersonaID.Value, persona => persona.PersonaID, (perfil, persona) => new
            {
                Perfil = perfil,
                Persona = persona
            }).Where(objeto => objeto.Persona.UsuarioID.HasValue && objeto.Persona.UsuarioID.Value.Equals(pUsuarioID)).Select(objeto => objeto.Perfil.PerfilID).ToList();

            List<Guid> listaProyectoID = mEntityContext.AdministradorProyecto.Join(mEntityContext.Usuario, adminProy => adminProy.UsuarioID, usuario => usuario.UsuarioID, (adminProy, usuario) => new
            {
                AdminProy = adminProy,
                User = usuario
            }).Join(mEntityContext.Persona, objeto => objeto.User.UsuarioID, persona => persona.UsuarioID.Value, (objeto, persona) => new
            {
                AdminProy = objeto.AdminProy,
                User = objeto.User,
                Persona = persona
            }).Join(mEntityContext.Perfil, objeto => objeto.Persona.PersonaID, perfil => perfil.PersonaID.Value, (objeto, perfil) => new
            {
                UsurioID = objeto.Persona.UsuarioID,
                ProyectoID = objeto.AdminProy.ProyectoID
            }).Where(objeto => objeto.UsurioID.HasValue && objeto.UsurioID.Value.Equals(pUsuarioID)).Select(objeto => objeto.UsurioID.Value).ToList();

            var listaProyecto = mEntityContext.Proyecto.Join(mEntityContext.Identidad, proyecto => proyecto.ProyectoID, identidad => identidad.ProyectoID, (proyecto, identidad) => new
            {
                Proyecto = proyecto,
                Identidad = identidad
            }).Where(objeto => listaPerfilID.Contains(objeto.Identidad.PerfilID) && !objeto.Identidad.FechaBaja.HasValue && !objeto.Identidad.FechaExpulsion.HasValue && !objeto.Proyecto.ProyectoID.Equals(ProyectoAD.MetaProyecto) && (objeto.Proyecto.Estado.Equals((short)EstadoProyecto.Abierto) || objeto.Proyecto.Estado.Equals((short)EstadoProyecto.Cerrandose) || (objeto.Proyecto.Estado.Equals((short)EstadoProyecto.Definicion) && listaProyectoID.Contains(objeto.Proyecto.ProyectoID)))).OrderByDescending(objeto => objeto.Identidad.NumConnexiones).Select(objeto => new
            {
                objeto.Proyecto.NombreCorto,
                objeto.Proyecto.Nombre,
                objeto.Proyecto.TipoAcceso,
                objeto.Identidad.NumConnexiones
            }).ToList();


            foreach (var proyecto in listaProyecto)
            {
                if (!listaProyectos.ContainsKey(proyecto.NombreCorto))
                {
                    listaProyectos.Add(proyecto.NombreCorto, new KeyValuePair<string, short>(proyecto.Nombre, proyecto.TipoAcceso));
                }
            }

            return listaProyectos;
        }
        /// <summary>
        /// Obtiene los proyectos en los que participa el usuario
        /// </summary>
        /// <param name="pUsuarioID">Id del usuario</param>
        /// <returns>Devuelve lista con los Id de los proyectos que participa el usuario</returns>
        public List<Guid> ObtenerProyectoIdParticipaUsuario(Guid pUsuarioID)
        {
            List<Guid> proyectosUsuario = new List<Guid>();

            List<Guid> listaPerfilID = mEntityContext.Perfil.Join(mEntityContext.Persona, perfil => perfil.PersonaID.Value, persona => persona.PersonaID, (perfil, persona) => new
            {
                Perfil = perfil,
                Persona = persona
            }).Where(objeto => objeto.Persona.UsuarioID.HasValue && objeto.Persona.UsuarioID.Value.Equals(pUsuarioID)).Select(objeto => objeto.Perfil.PerfilID).ToList();

            List<Guid> listaProyectoID = mEntityContext.AdministradorProyecto.Join(mEntityContext.Usuario, adminProy => adminProy.UsuarioID, usuario => usuario.UsuarioID, (adminProy, usuario) => new
            {
                AdminProy = adminProy,
                User = usuario
            }).Join(mEntityContext.Persona, objeto => objeto.User.UsuarioID, persona => persona.UsuarioID.Value, (objeto, persona) => new
            {
                AdminProy = objeto.AdminProy,
                User = objeto.User,
                Persona = persona
            }).Join(mEntityContext.Perfil, objeto => objeto.Persona.PersonaID, perfil => perfil.PersonaID.Value, (objeto, perfil) => new
            {
                UsurioID = objeto.Persona.UsuarioID,
                ProyectoID = objeto.AdminProy.ProyectoID
            }).Where(objeto => objeto.UsurioID.HasValue && objeto.UsurioID.Value.Equals(pUsuarioID)).Select(objeto => objeto.UsurioID.Value).ToList();

            var listaProyecto = mEntityContext.Proyecto.Join(mEntityContext.Identidad, proyecto => proyecto.ProyectoID, identidad => identidad.ProyectoID, (proyecto, identidad) => new
            {
                Proyecto = proyecto,
                Identidad = identidad
            }).Where(objeto => listaPerfilID.Contains(objeto.Identidad.PerfilID) && !objeto.Identidad.FechaBaja.HasValue && !objeto.Identidad.FechaExpulsion.HasValue && !objeto.Proyecto.ProyectoID.Equals(ProyectoAD.MetaProyecto) && (objeto.Proyecto.Estado.Equals((short)EstadoProyecto.Abierto) || objeto.Proyecto.Estado.Equals((short)EstadoProyecto.Cerrandose) || (objeto.Proyecto.Estado.Equals((short)EstadoProyecto.Definicion) && listaProyectoID.Contains(objeto.Proyecto.ProyectoID)))).OrderByDescending(objeto => objeto.Identidad.NumConnexiones).Select(objeto => new
            {
                objeto.Proyecto.ProyectoID
            }).ToList();

            foreach (var proyecto in listaProyecto)
            {
                if (!proyectosUsuario.Contains(proyecto.ProyectoID))
                {
                    proyectosUsuario.Add(proyecto.ProyectoID);
                }
            }
            return proyectosUsuario;
        }
        /// <summary>
        /// Devuvle los usuarios que no pertenecen al proyecto
        /// </summary>
        /// <param name="listaUsuarios">Lista de los usuarios de la organizacion</param>
        /// <param name="pProyectoID">Id del proyecto</param>
        /// <returns>Lista de los usuarios que no pertenecen a la organizacion</returns>
        public List<Guid> ObtenerUsuariosNoParticipanEnComunidad(List<Guid> listaUsuarios, Guid pProyectoID)
        {
            List<Guid> usuariosNoPertenecen = new List<Guid>();
            foreach (Guid usuario in listaUsuarios)
            {
                if (!mEntityContext.ProyectoRolUsuario.Any(item => item.UsuarioID.Equals(usuario) && item.ProyectoID.Equals(pProyectoID)))
                {
                    usuariosNoPertenecen.Add(usuario);
                }
            }
            return usuariosNoPertenecen;
        }
        /// <summary>
        /// Obtiene las urls de la busqueda
        /// </summary>
        /// <param name="pProyectoID">Id del proyecto</param>
        /// <returns>Lista con las Urls de la busqueda</returns>
        public List<string> ObtenerUrlsComunidadCajaBusqueda(Guid pProyectoID)
        {
            Guid componenteId = mEntityContext.CMSComponente.Where(tipo => tipo.TipoComponente.Equals((short)TipoComponenteCMS.CajaBuscador) && tipo.ProyectoID.Equals(pProyectoID)).Select(tipo => tipo.ComponenteID).FirstOrDefault();
            List<string> listaUrl = mEntityContext.CMSPropiedadComponente.Where(tipo => tipo.TipoPropiedadComponente.Equals((short)TipoPropiedadCMS.URLBusqueda) && tipo.ComponenteID.Equals(componenteId)).Select(tipo => tipo.ValorPropiedad).ToList();
            return listaUrl;
        }
        /// <summary>
        /// Elimina la comunidad de la url de búsqueda
        /// </summary>
        /// <param name="pProyectoID">Id del proyecto</param>
        /// <param name="url">Url a la que se le quiere quitar la comunidad</param>
        public void QuitarUrlComunidadCajaBusqueda(Guid pProyectoID, string url)
        {
            string urlNueva = string.Empty;
            string[] trozos = url.Split('/');
            if (trozos[3].Length == 2)
            {
                urlNueva = trozos[0] + "//" + trozos[2] + "/" + trozos[3];
                for (int i = 6; i < trozos.Length; i++)
                {
                    urlNueva = urlNueva + "/" + trozos[i];
                }
            }
            else
            {
                urlNueva = trozos[0] + "//" + trozos[2];
                for (int i = 5; i < trozos.Length; i++)
                {
                    urlNueva = urlNueva + "/" + trozos[i];
                }
            }
            Guid componenteId = mEntityContext.CMSComponente.Where(tipo => tipo.TipoComponente.Equals((short)TipoComponenteCMS.CajaBuscador) && tipo.ProyectoID.Equals(pProyectoID)).Select(tipo => tipo.ComponenteID).FirstOrDefault();
            mEntityContext.CMSPropiedadComponente.Where(tipo => tipo.TipoPropiedadComponente.Equals((short)TipoPropiedadCMS.URLBusqueda) && tipo.ComponenteID.Equals(componenteId) && tipo.ValorPropiedad.Equals(url)).FirstOrDefault().ValorPropiedad = urlNueva;
            mEntityContext.SaveChanges();

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pPerfilID"></param>
        /// <returns></returns>
        public Guid ObtenerProyectoIDMasActivoPerfil(Guid pPerfilID)
        {
            List<Guid> listaProyectoID = mEntityContext.AdministradorProyecto.Join(mEntityContext.Usuario, adminProy => adminProy.UsuarioID, usuario => usuario.UsuarioID, (adminProy, usuario) => new
            {
                AdminProy = adminProy,
                Usuario = usuario
            }).Join(mEntityContext.Persona, objeto => objeto.Usuario.UsuarioID, persona => persona.UsuarioID, (objeto, persona) => new
            {
                PersonaID = persona.PersonaID,
                ProyectoID = objeto.AdminProy.ProyectoID
            }).Join(mEntityContext.Perfil, objeto => objeto.PersonaID, perfil => perfil.PersonaID, (objeto, perfil) => new
            {
                ProyectoID = objeto.ProyectoID,
                PerfilID = perfil.PerfilID
            }).Where(objeto => objeto.PerfilID.Equals(pPerfilID)).Select(objeto => objeto.ProyectoID).ToList();

            Guid proyectoMasActivo = mEntityContext.Proyecto.Join(mEntityContext.Identidad, proyecto => proyecto.ProyectoID, identidad => identidad.ProyectoID, (proyecto, identidad) => new
            {
                Proyect = proyecto,
                Identidad = identidad
            }).Where(objeto => objeto.Identidad.PerfilID.Equals(pPerfilID) && !objeto.Identidad.FechaBaja.HasValue && !objeto.Identidad.FechaExpulsion.HasValue && !objeto.Proyect.ProyectoID.Equals(MetaProyecto) && (objeto.Proyect.Estado.Equals((short)EstadoProyecto.Abierto) || objeto.Proyect.Estado.Equals((short)EstadoProyecto.Cerrandose) || (objeto.Proyect.Estado.Equals((short)EstadoProyecto.Definicion) && listaProyectoID.Contains(objeto.Proyect.ProyectoID)))).OrderByDescending(objeto => objeto.Identidad.NumConnexiones).Select(objeto => objeto.Proyect.ProyectoID).FirstOrDefault();

            return proyectoMasActivo;
        }

        /// <summary>
        /// Obtiene una lista con los proyectos comunes en los que participan dos perfiles
        /// </summary>
        /// <param name="pPerfilID1">Identificador del perfil 1</param>
        /// <param name="pTipoIdentidad1">Tipo de identidad del perfil 1</param>
        /// <param name="pPerfilID2">Identificador del perfil 2</param>
        /// <param name="pTipoIdentidad2">Tipo de identidad del perfil 2</param>
        /// <param name="pIncluirMyGNOSS">Indica si se debe de buscar my gnoss</param>
        /// <returns>DataSet de Proyectos</returns>
        public DataWrapperProyecto ObtenerListaProyectosComunesParticipanPerfilesUsuarios(Guid pPerfilID1, TiposIdentidad pTipoIdentidad1, Guid pPerfilID2, TiposIdentidad pTipoIdentidad2, bool pIncluirMyGNOSS)
        {
            List<Guid> listaProyectos = new List<Guid>();

            var resultadoSQL1 = mEntityContext.Proyecto.Join(mEntityContext.Identidad, proy => proy.ProyectoID, identidad => identidad.ProyectoID, (proy, identidad) => new
            {
                ProyectoID = proy.ProyectoID,
                Nombre = proy.Nombre,
                PerfilID = identidad.PerfilID,
                FechaBaja = identidad.FechaBaja,
                FechaExpulsion = identidad.FechaExpulsion,
                Tipo = identidad.Tipo
            }).Where(res => res.PerfilID.Equals(pPerfilID1) && !res.ProyectoID.Equals(ProyectoAD.MyGnoss) && !res.FechaBaja.HasValue && !res.FechaExpulsion.HasValue && res.Tipo.Equals((short)pTipoIdentidad1)).Select(res => new { res.ProyectoID, res.Nombre });

            var resultadoSQL2 = mEntityContext.Proyecto.Join(mEntityContext.Identidad, proy => proy.ProyectoID, identidad => identidad.ProyectoID, (proy, identidad) => new
            {
                ProyectoID = proy.ProyectoID,
                Nombre = proy.Nombre,
                PerfilID = identidad.PerfilID,
                FechaBaja = identidad.FechaBaja,
                FechaExpulsion = identidad.FechaExpulsion,
                Tipo = identidad.Tipo
            }).Where(res => res.PerfilID.Equals(pPerfilID2) && !res.ProyectoID.Equals(ProyectoAD.MyGnoss) && !res.FechaBaja.HasValue && !res.FechaExpulsion.HasValue && res.Tipo.Equals((short)pTipoIdentidad2)).Select(res => new { res.ProyectoID, res.Nombre });

            if (pIncluirMyGNOSS)
            {
                resultadoSQL1 = mEntityContext.Proyecto.Join(mEntityContext.Identidad, proy => proy.ProyectoID, identidad => identidad.ProyectoID, (proy, identidad) => new
                {
                    ProyectoID = proy.ProyectoID,
                    Nombre = proy.Nombre,
                    PerfilID = identidad.PerfilID,
                    FechaBaja = identidad.FechaBaja,
                    FechaExpulsion = identidad.FechaExpulsion,
                    Tipo = identidad.Tipo
                }).Where(res => res.PerfilID.Equals(pPerfilID1) && !res.FechaBaja.HasValue && !res.FechaExpulsion.HasValue && res.Tipo.Equals((short)pTipoIdentidad1)).Select(res => new { res.ProyectoID, res.Nombre });

                resultadoSQL2 = mEntityContext.Proyecto.Join(mEntityContext.Identidad, proy => proy.ProyectoID, identidad => identidad.ProyectoID, (proy, identidad) => new
                {
                    ProyectoID = proy.ProyectoID,
                    Nombre = proy.Nombre,
                    PerfilID = identidad.PerfilID,
                    FechaBaja = identidad.FechaBaja,
                    FechaExpulsion = identidad.FechaExpulsion,
                    Tipo = identidad.Tipo
                }).Where(res => res.PerfilID.Equals(pPerfilID2) && !res.FechaBaja.HasValue && !res.FechaExpulsion.HasValue && res.Tipo.Equals((short)pTipoIdentidad2)).Select(res => new { res.ProyectoID, res.Nombre });
            }

            var resultadoFinal = resultadoSQL1.Intersect(resultadoSQL2).OrderBy(res => res.Nombre);

            foreach (var proyecto in resultadoFinal.ToList())
            {
                listaProyectos.Add(proyecto.ProyectoID);
            }

            return ObtenerProyectosPorIDsCargaLigera(listaProyectos);
        }

        /// <summary>
        /// Obtiene una DataSet sin tipar con una tabla "Emails" que contiene los campos (IdentidadID,PersonaID,Nombre,Email) de cada uno de los miembros que participan en un determinado proyecto
        /// </summary>
        /// <param name="pProyectoID">Clave del proyectos</param>
        /// <returns>DataSet</returns>
        public List<EmailsMiembrosDeProyecto> ObtenerEmailsMiembrosDeProyecto(Guid pProyectoID)
        {
            List<EmailsMiembrosDeProyecto> lista = mEntityContext.ProyectoUsuarioIdentidad.Join(mEntityContext.Identidad, proyectoUsuarioIdentidad => proyectoUsuarioIdentidad.IdentidadID, identidad => identidad.IdentidadID, (proyectoUsuarioIdentidad, identidad) => new
            {
                ProyectoUsuarioIdentidad = proyectoUsuarioIdentidad,
                Identidad = identidad
            }).Join(mEntityContext.Perfil, objeto => objeto.Identidad.PerfilID, perfil => perfil.PerfilID, (objeto, perfil) => new
            {
                ProyectoUsuarioIdentidad = objeto.ProyectoUsuarioIdentidad,
                Identidad = objeto.Identidad,
                Perfil = perfil
            }).Join(mEntityContext.Persona, objeto => objeto.Perfil.PersonaID, persona => persona.PersonaID, (objeto, persona) => new
            {
                ProyectoUsuarioIdentidad = objeto.ProyectoUsuarioIdentidad,
                Identidad = objeto.Identidad,
                Perfil = objeto.Perfil,
                Persona = persona
            }).Where(objeto => objeto.ProyectoUsuarioIdentidad.ProyectoID.Equals(pProyectoID) && objeto.Identidad.FechaBaja == null && objeto.Perfil.Eliminado == false && !objeto.Perfil.OrganizacionID.HasValue && objeto.Persona.Email != null).ToList().Select(objeto => new EmailsMiembrosDeProyecto { IdentidadID = objeto.ProyectoUsuarioIdentidad.IdentidadID, PersonaID = objeto.Perfil.PersonaID, Nombre = objeto.Persona.Nombre, Email = objeto.Persona.Email })
            .Union(mEntityContext.ProyectoUsuarioIdentidad.Join(mEntityContext.Identidad, proyectoUsuarioIdentidad => proyectoUsuarioIdentidad.IdentidadID, identidad => identidad.IdentidadID, (proyectoUsuarioIdentidad, identidad) => new
            {
                ProyectoUsuarioIdentidad = proyectoUsuarioIdentidad,
                Identidad = identidad
            }).Join(mEntityContext.Perfil, objeto => objeto.Identidad.PerfilID, perfil => perfil.PerfilID, (objeto, perfil) => new
            {
                ProyectoUsuarioIdentidad = objeto.ProyectoUsuarioIdentidad,
                Identidad = objeto.Identidad,
                Perfil = perfil
            }).Join(mEntityContext.Persona, objeto => objeto.Perfil.PersonaID, persona => persona.PersonaID, (objeto, persona) => new
            {
                ProyectoUsuarioIdentidad = objeto.ProyectoUsuarioIdentidad,
                Identidad = objeto.Identidad,
                Perfil = objeto.Perfil,
                Persona = persona
            }).Join(mEntityContext.PersonaVinculoOrganizacion, objeto => new { objeto.Persona.PersonaID, OrganizacionID = objeto.Perfil.OrganizacionID.Value }, personaVinculoOrganizacion => new { personaVinculoOrganizacion.PersonaID, personaVinculoOrganizacion.OrganizacionID }, (objeto, personaVinculoOrganizacion) => new
            {
                ProyectoUsuarioIdentidad = objeto.ProyectoUsuarioIdentidad,
                Identidad = objeto.Identidad,
                Perfil = objeto.Perfil,
                Persona = objeto.Persona,
                PersonaVinculoOrganizacion = personaVinculoOrganizacion
            }).Where(objeto => objeto.ProyectoUsuarioIdentidad.ProyectoID.Equals(pProyectoID) && !objeto.Identidad.FechaBaja.HasValue && objeto.Perfil.Eliminado == false && objeto.Perfil.PersonaID.HasValue && objeto.Perfil.OrganizacionID.HasValue && objeto.PersonaVinculoOrganizacion.EmailTrabajo != null).ToList().Select(objeto => new EmailsMiembrosDeProyecto { IdentidadID = objeto.ProyectoUsuarioIdentidad.IdentidadID, PersonaID = objeto.Perfil.PersonaID, Nombre = objeto.Persona.Nombre, Email = objeto.PersonaVinculoOrganizacion.EmailTrabajo })).ToList();

            return lista;
        }
        /// <summary>
        /// Obtiene una lista con los servicios externos si es MetaProyecto
        /// </summary>
        /// <returns>Lista de los servicios externos</returns>
        public List<EcosistemaServicioExterno> ObtenerEcosistemaServicioExterno()
        {
            return mEntityContext.EcosistemaServicioExterno.ToList();
        }
        /// <summary>
        /// Obtiene una lista con los servicios externos si no es MetaProyecto
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <returns>Lista de los servicios externos</returns>
        public List<ProyectoServicioExterno> ObtenerProyectoServicioExterno(Guid pProyectoID)
        {
            return mEntityContext.ProyectoServicioExterno.Where(proy => proy.ProyectoID.Equals(pProyectoID)).ToList();
        }

        /// <summary>
        /// Obtiene el valor de un parametro de aplicacion 
        /// </summary>
        /// <param name="parametro">Parametro de aplicacion</param>
        /// <returns>Valor correspondiente al parametro pasado</returns>
        public string ObtenerParametroAplicacion(string parametro)
        {
            return mEntityContext.ParametroAplicacion.Where(param => param.Parametro.Equals(parametro)).Select(param => param.Valor).FirstOrDefault();
        }
        /// <summary>
        /// Guarda un nuevo parametro aplicacion
        /// </summary>
        /// <param name="parametro">Nombre parametro de aplicacion</param>
        /// <param name="valor">Valor del paramentro</param>       
        public void GuardarParametroAplicacion(string parametro, string valor)
        {
            mEntityContext.ParametroAplicacion.Add(new AD.EntityModel.ParametroAplicacion(parametro, valor));
            ActualizarBaseDeDatosEntityContext();

        }
        /// <summary>
        /// Guarda en la tabla EcosistemaServicioExterno una nueva fila
        /// </summary>
        /// <param name="eco">Objeto de tipo EcosistemaServicioExterno</param> 
        public void ActualizarServiceNameEcosistema(EcosistemaServicioExterno eco)
        {
            mEntityContext.EcosistemaServicioExterno.Add(eco);
            ActualizarBaseDeDatosEntityContext();
        }
        /// <summary>
        ///  Guarda en la tabla ProyectoServicioExterno una nueva fila
        /// </summary>
        /// <param name="proy">Objeto de tipo ProyectoServicioExterno</param> 
        public void ActualizarServiceNameProyecto(ProyectoServicioExterno proy)
        {
            mEntityContext.ProyectoServicioExterno.Add(proy);
            ActualizarBaseDeDatosEntityContext();
        }
        /// <summary>
        ///  Elimina de la tabla EcosistemaServicioExterno una fila
        /// </summary>
        /// <param name="eco">Objeto de tipo EcosistemaServicioExterno</param> 
        public void EliminarEcosistemaServicioExterno(EcosistemaServicioExterno eco)
        {
            mEntityContext.Entry(eco).State = EntityState.Deleted;
            ActualizarBaseDeDatosEntityContext();
        }
        /// <summary>
        ///  Elimina de la tabla ProyectoServicioExterno una fila
        /// </summary>
        /// <param name="proy">Objeto de tipo ProyectoServicioExterno</param>
        public void EliminarProyectoServicioExterno(ProyectoServicioExterno proy)
        {
            mEntityContext.Entry(proy).State = EntityState.Deleted;
            ActualizarBaseDeDatosEntityContext();
        }
        /// <summary>
        ///  Obtiene la OrganizacionID a la que corresponde un proyecto
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <returns>ID de la organizacion</returns>
        public Guid ObtenerOrganizacionIDAPartirDeProyectoID(Guid pProyectoID)
        {
            return mEntityContext.Proyecto.Where(proy => proy.ProyectoID.Equals(pProyectoID)).Select(proy => proy.OrganizacionID).FirstOrDefault();
        }
        /// <summary>
        /// Obtiene una DataSet sin tipar con una tabla "Emails" que contiene los campos (IdentidadID,PersonaID,Nombre,Email) de cada uno de los miembros que participan en un determinado evento
        /// </summary>
        /// <param name="pEventoID">Clave del evento</param>
        /// <returns>DataSet</returns>
        public DataSet ObtenerEmailsMiembrosDeEventoDeProyecto(Guid pEventoID)
        {
            DataSet dataSet = new DataSet();

            DbCommand commandsqlSelectEmailsMiembrosDeEventoDeProyecto = ObtenerComando(sqlSelectEmailsMiembrosDeEventoDeProyecto);
            AgregarParametro(commandsqlSelectEmailsMiembrosDeEventoDeProyecto, IBD.ToParam("eventoID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pEventoID));
            CargarDataSet(commandsqlSelectEmailsMiembrosDeEventoDeProyecto, dataSet, "Emails");

            return (dataSet);
        }

        /// <summary>
        /// Devuelve una lista con los emails de los administradores del proyecto.
        /// </summary>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <returns>DataSet</returns>
        public List<string> ObtenerEmailsAdministradoresDeProyecto(Guid pProyectoID)
        {
            return mEntityContext.ProyectoUsuarioIdentidad.Join(mEntityContext.Identidad, proyectoUsuarioIdentidad => proyectoUsuarioIdentidad.IdentidadID, identidad => identidad.IdentidadID, (proyectoUsuarioIdentidad, identidad) => new
            {
                ProyectoUsuarioIdentidad = proyectoUsuarioIdentidad,
                Identidad = identidad
            }).Join(mEntityContext.Perfil, objeto => objeto.Identidad.PerfilID, perfil => perfil.PerfilID, (objeto, perfil) => new
            {
                ProyectoUsuarioIdentidad = objeto.ProyectoUsuarioIdentidad,
                Identidad = objeto.Identidad,
                Perfil = perfil
            }).Join(mEntityContext.Persona, objeto => objeto.Perfil.PersonaID.Value, persona => persona.PersonaID, (objeto, persona) => new
            {
                ProyectoUsuarioIdentidad = objeto.ProyectoUsuarioIdentidad,
                Identidad = objeto.Identidad,
                Perfil = objeto.Perfil,
                Persona = persona
            }).Join(mEntityContext.AdministradorProyecto, objeto => new { objeto.ProyectoUsuarioIdentidad.UsuarioID, objeto.ProyectoUsuarioIdentidad.ProyectoID }, adminProy => new { adminProy.UsuarioID, adminProy.ProyectoID }, (objeto, adminProy) => new
            {
                ProyectoUsuarioIdentidad = objeto.ProyectoUsuarioIdentidad,
                Identidad = objeto.Identidad,
                Perfil = objeto.Perfil,
                Persona = objeto.Persona,
                AdministradorProyecto = adminProy
            }).Where(objeto => objeto.Perfil.PersonaID.HasValue && objeto.ProyectoUsuarioIdentidad.ProyectoID.Equals(pProyectoID) && !objeto.Identidad.FechaBaja.HasValue && objeto.Perfil.Eliminado == false && !objeto.Perfil.OrganizacionID.HasValue && objeto.Persona.Email != null && objeto.AdministradorProyecto.Tipo == 0).Select(item => item.Persona.Email).Distinct().Union(
                mEntityContext.ProyectoUsuarioIdentidad.Join(mEntityContext.Identidad, proyectoUsuarioIdentidad => proyectoUsuarioIdentidad.IdentidadID, identidad => identidad.IdentidadID, (proyectoUsuarioIdentidad, identidad) => new
                {
                    ProyectoUsuarioIdentidad = proyectoUsuarioIdentidad,
                    Identidad = identidad,
                }).Join(mEntityContext.Perfil, objeto => objeto.Identidad.PerfilID, perfil => perfil.PerfilID, (objeto, perfil) => new
                {
                    ProyectoUsuarioIdentidad = objeto.ProyectoUsuarioIdentidad,
                    Identidad = objeto.Identidad,
                    Perfil = perfil
                }).Join(mEntityContext.Persona, objeto => objeto.Perfil.PersonaID, persona => persona.PersonaID, (objeto, persona) => new
                {
                    ProyectoUsuarioIdentidad = objeto.ProyectoUsuarioIdentidad,
                    Identidad = objeto.Identidad,
                    Perfil = objeto.Perfil,
                    Persona = persona
                }).Join(mEntityContext.PersonaVinculoOrganizacion, objeto => new { objeto.Persona.PersonaID, OrganizacionID = objeto.Perfil.OrganizacionID.Value }, personaVinculoOrganizacion => new { personaVinculoOrganizacion.PersonaID, personaVinculoOrganizacion.OrganizacionID }, (objeto, personaVinculoOrganizacion) => new
                {
                    ProyectoUsuarioIdentidad = objeto.ProyectoUsuarioIdentidad,
                    Identidad = objeto.Identidad,
                    Perfil = objeto.Perfil,
                    Persona = objeto.Persona,
                    PersonaVinculoOrganizacion = personaVinculoOrganizacion
                }).Join(mEntityContext.AdministradorProyecto, objeto => new { objeto.ProyectoUsuarioIdentidad.UsuarioID, objeto.ProyectoUsuarioIdentidad.ProyectoID }, adminProyect => new { adminProyect.UsuarioID, adminProyect.ProyectoID }, (objeto, adminProyect) => new
                {
                    ProyectoUsuarioIdentidad = objeto.ProyectoUsuarioIdentidad,
                    Identidad = objeto.Identidad,
                    Perfil = objeto.Perfil,
                    Persona = objeto.Persona,
                    PersonaVinculoOrganizacion = objeto.PersonaVinculoOrganizacion,
                    AdministradorProyecto = adminProyect
                }).Where(objeto => objeto.ProyectoUsuarioIdentidad.ProyectoID.Equals(pProyectoID) && !objeto.Identidad.FechaBaja.HasValue && objeto.Perfil.Eliminado == false && objeto.Perfil.PersonaID.HasValue && objeto.Perfil.OrganizacionID.HasValue && objeto.PersonaVinculoOrganizacion.EmailTrabajo != null && objeto.AdministradorProyecto.Tipo == 0).Select(item => item.Persona.Email).Distinct()
                ).ToList();
        }

        /// <summary>
        /// Obtiene el número de elementos de un perfil en unos proyectos.
        /// </summary>
        /// <param name="pProyectosID">IDs de los proyectos</param>
        /// <param name="pPerfilID">ID del perfil</param>
        /// <returns>DataSet con la tabla 'ProyectoPerfilNumElem' cargada para un perfil en uno proyectos</returns>
        public DataWrapperProyecto ObtenerNumeroElementosPerfilEnProyectos(List<Guid> pProyectosID, Guid pPerfilID)
        {
            DataWrapperProyecto proyectoDataWrapper = new DataWrapperProyecto();
            if (pProyectosID.Count > 0)
            {
                List<ProyectoPerfilNumElem> listaProyectoPerfilNumElem = mEntityContext.ProyectoPerfilNumElem.Where(proyectoPerfil => proyectoPerfil.PerfilID.Equals(pPerfilID) && pProyectosID.Contains(proyectoPerfil.ProyectoID)).ToList();
                proyectoDataWrapper.ListaProyectoPerfilNumElem = listaProyectoPerfilNumElem;
            }

            return proyectoDataWrapper;
        }

        /// <summary>
        /// Actualiza los contadores del proyecto
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto a actualizar</param>
        /// <param name="pNumOrg">Número de miembros de organización</param>
        /// <param name="pNumIden">Número de miembros normales</param>
        /// <param name="pNumRec">Número de recursos</param>
        /// <param name="pNumDafos">Número de dafos</param>
        /// <param name="pNumDebates">Número Debates</param>
        /// <param name="pNumPreg">Número Preguntas</param>
        public void ActualizarContadoresProyecto(Guid pProyectoID, int pNumOrg, int pNumIden, int pNumRec, int pNumDafos, int pNumDebates, int pNumPreg)
        {
            Proyecto proyecto = mEntityContext.Proyecto.Where(proyect => proyect.ProyectoID.Equals(pProyectoID)).FirstOrDefault();

            if (pNumOrg != -1)
            {
                proyecto.NumeroOrgRegistradas = pNumOrg;
            }
            if (pNumIden != -1)
            {
                proyecto.NumeroMiembros = pNumIden;
            }
            if (pNumRec != -1)
            {
                proyecto.NumeroRecursos = pNumRec;
            }
            if (pNumDafos != -1)
            {
                proyecto.NumeroDafos = pNumDafos;
            }
            if (pNumDebates != -1)
            {
                proyecto.NumeroDebates = pNumDebates;
            }
            if (pNumPreg != -1)
            {
                proyecto.NumeroPreguntas = pNumPreg;
            }

            mEntityContext.SaveChanges();
        }

        /// <summary>
        /// Obtiene el número de recursos del proyectoID publicados en los últimos 30 días
        /// </summary>
        /// <param name="pProyectoID">ProyectoID del que se quiere obtener el número de recursos</param>
        /// <param name="pNumDias">Días hasta hoy</param>
        /// <returns>Número de recursos</returns>
        public int ObtenerNumRecursosProyecto30Dias(Guid pProyectoID, int pNumDias)
        {
            //List<Documento> listaDocumento = mEntityContext.Documento.Join(mEntityContext.DocumentoWebVinBaseRecursos, doc => doc.DocumentoID, docWebVin => docWebVin.DocumentoID, (doc, docWebVin) => new
            //{
            //    Documento = doc,
            //    DocumentoWebVinBaseRecursos = docWebVin
            //}).Join(mEntityContext.BaseRecursosProyecto, objeto => objeto.DocumentoWebVinBaseRecursos.BaseRecursosID, baseRecurso => baseRecurso.BaseRecursosID, (objeto, baseRecurso) => new
            //{
            //    Documento = objeto.Documento,
            //    DocumentoWebVinBaseRecursos = objeto.DocumentoWebVinBaseRecursos,
            //    BaseRecursosProyecto = baseRecurso
            //}).Where(objeto => objeto.BaseRecursosProyecto.ProyectoID.Equals(pProyectoID) && objeto.Documento.Borrador.Equals(false) && objeto.Documento.Eliminado.Equals(false) && objeto.DocumentoWebVinBaseRecursos.Eliminado.Equals(false) && objeto.Documento.UltimaVersion.Equals(true) && objeto.DocumentoWebVinBaseRecursos.PrivadoEditores.Equals(false) && objeto.Documento.Tipo != (short)TiposDocumentacion.Ontologia && objeto.Documento.Tipo != (short)TiposDocumentacion.OntologiaSecundaria).Select(objeto => objeto.Documento).ToList();

            //if (pNumDias != -1)
            //{
            //    listaDocumento = listaDocumento.Where(documento => documento.FechaCreacion > DateTime.Now.AddDays(pNumDias)).ToList();
            //}

            //return listaDocumento.Count;

            var query = mEntityContext.Documento.JoinDocumentoWebVinBaseRecursos().JoinBaseRecursosProyecto().Where(item => item.BaseRecursosProyecto.ProyectoID.Equals(pProyectoID) && !item.Documento.Borrador && !item.Documento.Eliminado && !item.DocumentoWebVinBaseRecursos.Eliminado && item.Documento.UltimaVersion && !item.DocumentoWebVinBaseRecursos.PrivadoEditores && !item.Documento.Tipo.Equals((short)TiposDocumentacion.Ontologia) && !item.Documento.Tipo.Equals((short)TiposDocumentacion.OntologiaSecundaria));

            if (pNumDias != -1)
            {
                query = query.Where(item => item.Documento.FechaCreacion > DateTime.Now.AddDays(pNumDias));
            }

            return query.Count();
        }



        /// <summary>
        /// Obtiene las secciones de la home de un proyecto tipo catálogo
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns></returns>
        public DataWrapperProyecto ObtenerSeccionesHomeCatalogoDeProyecto(Guid pProyectoID)
        {
            //TODO TABLA
            //ProyectoDS proyectoDS = new ProyectoDS();
            //this.sqlSelectSeccionProyCatalogo = "SELECT " + IBD.CargarGuid("SeccionProyCatalogo.OrganizacionID") + ", " + IBD.CargarGuid("SeccionProyCatalogo.ProyectoID") + ", " + IBD.CargarGuid("SeccionProyCatalogo.OrganizacionBusquedaID") + ", " + IBD.CargarGuid("SeccionProyCatalogo.ProyectoBusquedaID") + ", SeccionProyCatalogo.Tipo, SeccionProyCatalogo.Nombre, SeccionProyCatalogo.Faceta, SeccionProyCatalogo.Filtro, SeccionProyCatalogo.NumeroResultados, SeccionProyCatalogo.Orden FROM SeccionProyCatalogo";
            //string consulta = this.sqlSelectSeccionProyCatalogo + " WHERE ProyectoID = " + IBD.ToParam("ProyectoID");

            List<SeccionProyCatalogo> listaSeccionProyCatalogo = mEntityContext.SeccionProyCatalogo.Where(proy => proy.ProyectoID.Equals(pProyectoID)).ToList();

            //DbCommand commandsqlSelectSeccionesHomeCatalogoDeProyecto = ObtenerComando(consulta);
            //AgregarParametro(commandsqlSelectSeccionesHomeCatalogoDeProyecto, IBD.ToParam("ProyectoID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pProyectoID));
            //CargarDataSet(commandsqlSelectSeccionesHomeCatalogoDeProyecto, proyectoDS, "SeccionProyCatalogo");

            //return proyectoDS;
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();
            dataWrapperProyecto.ListaSeccionProyCatalogo = listaSeccionProyCatalogo;
            //dataWrapperProyecto.ListaSeccionProyCatalogo
            return dataWrapperProyecto;
        }

        /// <summary>
        /// Aumenta en 1 el "NumeroMiembros" de la tabla Proyecto de un pProyectoID
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        public void AumentarNumeroMiembrosDelProyecto(Guid pProyectoID)
        {
            DbCommand commandsqlUpdateAumentarNumeroMiembrosDelProyecto = ObtenerComando(sqlUpdateAumentarNumeroMiembrosDelProyecto);
            AgregarParametro(commandsqlUpdateAumentarNumeroMiembrosDelProyecto, IBD.ToParam("proyectoID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pProyectoID));

            ActualizarBaseDeDatos(commandsqlUpdateAumentarNumeroMiembrosDelProyecto);
        }

        /// <summary>
        /// Disminuye en 1 el "NumeroMiembros" de la tabla Proyecto de un pProyectoID
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        public void DisminuirNumeroMiembrosDelProyecto(Guid pProyectoID)
        {
            Proyecto proyecto = mEntityContext.Proyecto.Where(proyect => proyect.ProyectoID.Equals(pProyectoID)).FirstOrDefault();
            if (proyecto != null)
            {
                proyecto.NumeroMiembros = proyecto.NumeroMiembros - 1;
                mEntityContext.SaveChanges();
            }
        }

        /// <summary>
        /// Actualiza el "NumeroMiembros" de la tabla Proyecto de un pProyectoID
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        public void ActualizarNumeroMiembrosDelProyecto(Guid pProyectoID)
        {
            DbCommand commandsqlUpdateActualizar = ObtenerComando("UPDATE Proyecto SET NumeroMiembros = (SELECT COUNT(*) FROM Usuario inner join Persona ON (Usuario.UsuarioID=Persona.UsuarioID) inner join Perfil ON (Persona.PersonaID=Perfil.PersonaID) inner join Identidad on (Perfil.PerfilID = Identidad.PerfilID) where proyectoid=" + IBD.ToParam("proyectoID") + " AND Fechabaja IS NULL and Fechaexpulsion IS NULL and Perfil.Eliminado = 0 and Persona.Eliminado = 0 and Tipo != " + (short)TiposIdentidad.ProfesionalCorporativo + ")");
            AgregarParametro(commandsqlUpdateActualizar, IBD.ToParam("proyectoID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pProyectoID));

            ActualizarBaseDeDatos(commandsqlUpdateActualizar);
        }

        /// <summary>
        ///  Disminuye en 1 el "NumeroOrgRegistradas" de la tabla Proyecto de un pProyectoID
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        public void DisminuirNumeroOrParticipanEnProyecto(Guid pProyectoID)
        {
            Proyecto proyecto = mEntityContext.Proyecto.Where(proyect => proyect.ProyectoID.Equals(pProyectoID)).FirstOrDefault();
            if (proyecto != null)
            {
                proyecto.NumeroOrgRegistradas = proyecto.NumeroOrgRegistradas - 1;
                mEntityContext.SaveChanges();
            }
        }

        /// <summary>
        /// Aumenta en 1 el "NumeroOrgRegistradas" de la tabla Proyecto de un pProyectoID
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        public void AumentarNumeroOrgParticipanEnProyecto(Guid pProyectoID)
        {
            Proyecto proyecto = mEntityContext.Proyecto.Where(proyect => proyect.ProyectoID.Equals(pProyectoID)).FirstOrDefault();
            if (proyecto != null)
            {
                proyecto.NumeroOrgRegistradas = proyecto.NumeroOrgRegistradas + 1;
                mEntityContext.SaveChanges();
            }
        }

        /// <summary>
        /// Obtiene un lista con los nombres de determinados proyectos
        /// </summary>
        /// <param name="pListaProyectos">Lista con los identificadores de los proyectos a nombrar</param>
        /// <returns>Lista (identificador, nombre) de proyectos</returns>
        public Dictionary<Guid, Proyecto> ObtenerNombreProyectos(List<Guid> pListaProyectos)
        {
            Dictionary<Guid, Proyecto> listaNombres = new Dictionary<Guid, Proyecto>();

            if (pListaProyectos.Count > 0)
            {
                string sqlSelectNombre = SelectProyectoLigero + "FROM Proyecto WHERE ";

                Dictionary<Guid, string> listaParmetros = new Dictionary<Guid, string>();

                List<Proyecto> listaProyectos = mEntityContext.Proyecto.Where(proyecto => pListaProyectos.Contains(proyecto.ProyectoID)).ToList();

                foreach (Proyecto filaProy in listaProyectos)
                {
                    listaNombres.Add(filaProy.ProyectoID, filaProy);
                }
            }
            return listaNombres;
        }

        /// <summary>
        /// Comprueba si en el proyecto existen usuarios que no sean los administradores del mismo
        /// </summary>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <returns>TRUE si existen, FALSE en caso contrario</returns>
        public bool TieneUsuariosExceptoLosAdministradores(Guid pProyectoID)
        {
            List<Guid> listaID = mEntityContext.AdministradorProyecto.Where(adminProy => adminProy.ProyectoID.Equals(pProyectoID) && adminProy.Tipo.Equals((short)TipoRolUsuario.Administrador)).Select(adminProy => adminProy.UsuarioID).ToList();

            List<Guid> ListaIdUsuarios = mEntityContext.ProyectoUsuarioIdentidad.Where(proyectoUser => proyectoUser.ProyectoID.Equals(pProyectoID) && !listaID.Contains(proyectoUser.UsuarioID)).Select(res => res.UsuarioID).ToList();

            return (ListaIdUsuarios.Count > 0);
        }

        /// <summary>
        /// Comprueba si existe alguna categoría de tesauro en el proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <returns>TRUE si existen, FALSE en caso contrario</returns>
        public bool TienecategoriasDeTesauro(Guid pProyectoID)
        {
            List<Guid> categoriasTesauro = mEntityContext.CategoriaTesauro.Join(mEntityContext.TesauroProyecto, categoriaTes => categoriaTes.TesauroID, tesProyecto => tesProyecto.TesauroID, (categoriaTes, tesProyecto) => new
            {
                CategoriaTesauroID = categoriaTes.CategoriaTesauroID,
                ProyectoID = tesProyecto.ProyectoID
            }).Where(objeto => objeto.ProyectoID.Equals(pProyectoID)).Select(objeto => objeto.CategoriaTesauroID).ToList();

            List<Guid> listaSugerenciasID = mEntityContext.CategoriaTesauroSugerencia.Join(mEntityContext.TesauroProyecto, categoriaTesSug => categoriaTesSug.TesauroSugerenciaID, tesProyecto => tesProyecto.TesauroID, (categoriaTesSug, tesProyecto) => new
            {
                SugerenciaID = categoriaTesSug.SugerenciaID,
                ProyectoID = tesProyecto.ProyectoID
            }).Where(objeto => objeto.ProyectoID.Equals(pProyectoID)).Select(objecto => objecto.SugerenciaID).ToList();

            return ((categoriasTesauro != null) || (listaSugerenciasID != null));
        }

        /// <summary>
        /// Obtienen los proyectos a los que acceden las identidades que tienen acceso a un proyecto pasado como parámetro
        /// </summary>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <returns>Dataset de proyecto</returns>
        public DataWrapperProyecto ObtenerProyectosDeIdentidadesAccedenAProyecto(Guid pProyectoID)
        {
            DataWrapperProyecto dataWrapperProycto = new DataWrapperProyecto();

            List<Guid> listaPerfilID = mEntityContext.Identidad.Where(identidad => identidad.FechaBaja == null && identidad.ProyectoID.Equals(pProyectoID)).Select(identidad => identidad.PerfilID).ToList();

            var proyectos = mEntityContext.Proyecto.Join(mEntityContext.Identidad, proyect => proyect.ProyectoID, identidad => identidad.ProyectoID, (proyect, identidad) => new
            {
                Proyecto = proyect,
                Identidad = identidad
            }).Where(proyectoIdentidad => proyectoIdentidad.Proyecto.TipoAcceso != (short)TipoAcceso.Privado && proyectoIdentidad.Proyecto.TipoAcceso != (short)TipoAcceso.Reservado && proyectoIdentidad.Proyecto.Estado != (short)EstadoProyecto.Cerrado && proyectoIdentidad.Proyecto.Estado != (short)EstadoProyecto.CerradoTemporalmente && proyectoIdentidad.Proyecto.TipoProyecto != (short)TipoProyecto.MetaComunidad && proyectoIdentidad.Identidad.FechaBaja == null && listaPerfilID.Contains(proyectoIdentidad.Identidad.PerfilID)).Select(objeto => new
            {
                OrganizacionID = objeto.Proyecto.OrganizacionID,
                ProyectoID = objeto.Proyecto.ProyectoID,
                Nombre = objeto.Proyecto.Nombre,
                TipoProyecto = objeto.Proyecto.TipoProyecto,
                TipoAcceso = objeto.Proyecto.TipoAcceso,
                NumeroRecursos = objeto.Proyecto.NumeroRecursos,
                NumeroPreguntas = objeto.Proyecto.NumeroPreguntas,
                NumeroDebates = objeto.Proyecto.NumeroDebates,
                NumeroMiembros = objeto.Proyecto.NumeroMiembros,
                NumeroOrgRegistradas = objeto.Proyecto.NumeroOrgRegistradas,
                EsProyectoDestacado = objeto.Proyecto.EsProyectoDestacado,
                Estado = objeto.Proyecto.Estado,
                URLPropia = objeto.Proyecto.URLPropia,
                NombreCorto = objeto.Proyecto.NombreCorto,
                Descripcion = objeto.Proyecto.Descripcion,
                ProyectoSuperiorID = objeto.Proyecto.ProyectoSuperiorID,
                TieneTwitter = objeto.Proyecto.TieneTwitter,
                TagTwitter = objeto.Proyecto.TagTwitter,
                UsuarioTwitter = objeto.Proyecto.UsuarioTwitter,
                TokenTwitter = objeto.Proyecto.TokenTwitter,
                TokenSecretoTwitter = objeto.Proyecto.TokenSecretoTwitter,
                EnviarTwitterComentario = objeto.Proyecto.EnviarTwitterComentario,
                EnviarTwitterNuevaCat = objeto.Proyecto.EnviarTwitterNuevaCat,
                EnviarTwitterNuevoAdmin = objeto.Proyecto.EnviarTwitterNuevoAdmin,
                EnviarTwitterNuevaPolitCert = objeto.Proyecto.EnviarTwitterNuevaPolitCert,
                EnviarTwitterNuevoTipoDoc = objeto.Proyecto.EnviarTwitterNuevoTipoDoc,
                TablaBaseProyectoID = objeto.Proyecto.TablaBaseProyectoID,
                ProcesoVinculadoID = objeto.Proyecto.ProcesoVinculadoID,
                Tags = objeto.Proyecto.Tags,
                TagTwitterGnoss = objeto.Proyecto.TagTwitterGnoss,
                NombrePresentacion = objeto.Proyecto.NombrePresentacion
            }).OrderBy(objeto => objeto.Nombre).ToList().Distinct().ToList();

            List<Proyecto> listaProyectos = new List<Proyecto>();
            foreach (var proyectoCorto in proyectos)
            {
                Proyecto proyect = new Proyecto();
                proyect.OrganizacionID = proyectoCorto.OrganizacionID;
                proyect.ProyectoID = proyectoCorto.ProyectoID;
                proyect.Nombre = proyectoCorto.Nombre;
                proyect.TipoProyecto = proyectoCorto.TipoProyecto;
                proyect.TipoAcceso = proyectoCorto.TipoAcceso;
                proyect.NumeroRecursos = proyectoCorto.NumeroRecursos;
                proyect.NumeroPreguntas = proyectoCorto.NumeroPreguntas;
                proyect.NumeroDebates = proyectoCorto.NumeroDebates;
                proyect.NumeroMiembros = proyectoCorto.NumeroMiembros;
                proyect.NumeroOrgRegistradas = proyectoCorto.NumeroOrgRegistradas;
                proyect.EsProyectoDestacado = proyectoCorto.EsProyectoDestacado;
                proyect.Estado = proyectoCorto.Estado;
                proyect.URLPropia = proyectoCorto.URLPropia;
                proyect.NombreCorto = proyectoCorto.NombreCorto;
                proyect.Descripcion = proyectoCorto.Descripcion;
                proyect.ProyectoSuperiorID = proyectoCorto.ProyectoSuperiorID;
                proyect.TieneTwitter = proyectoCorto.TieneTwitter;
                proyect.TagTwitter = proyectoCorto.TagTwitter;
                proyect.UsuarioTwitter = proyectoCorto.UsuarioTwitter;
                proyect.TokenTwitter = proyectoCorto.TokenTwitter;
                proyect.TokenSecretoTwitter = proyectoCorto.TokenSecretoTwitter;
                proyect.EnviarTwitterComentario = proyectoCorto.EnviarTwitterComentario;
                proyect.EnviarTwitterNuevaCat = proyectoCorto.EnviarTwitterNuevaCat;
                proyect.EnviarTwitterNuevoAdmin = proyectoCorto.EnviarTwitterNuevoAdmin;
                proyect.EnviarTwitterNuevaPolitCert = proyectoCorto.EnviarTwitterNuevaPolitCert;
                proyect.EnviarTwitterNuevoTipoDoc = proyectoCorto.EnviarTwitterNuevoTipoDoc;
                proyect.TablaBaseProyectoID = proyectoCorto.TablaBaseProyectoID;
                proyect.ProcesoVinculadoID = proyectoCorto.ProcesoVinculadoID;
                proyect.Tags = proyectoCorto.Tags;
                proyect.TagTwitterGnoss = proyectoCorto.TagTwitterGnoss;
                proyect.NombrePresentacion = proyectoCorto.NombrePresentacion;
                listaProyectos.Add(proyect);
            }
            dataWrapperProycto.ListaProyecto = listaProyectos;

            return dataWrapperProycto;
        }

        /// <summary>
        /// Obtiene los niveles de certificación de un proyecto pasado como parámetro
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Dataset de proyecto</returns>
        public DataWrapperProyecto ObtenerNivelesCertificacionRecursosProyecto(Guid pProyectoID)
        {
            List<NivelCertificacion> listNivelcertificacion = mEntityContext.NivelCertificacion.Where(nivelCertificacion => nivelCertificacion.ProyectoID.Equals(pProyectoID)).ToList();

            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            dataWrapperProyecto.ListaNivelCertificacion = listNivelcertificacion;

            return dataWrapperProyecto;
        }

        /// <summary>
        /// Obtiene los gadgets de un proyecto que se le pasa como parámetro
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto del que queremos obtener los gadgets</param>
        /// <param name="pDataWrapperProyecto">Dataset de proyectos</param>
        /// <returns>DataSet con los gadgets del proyecto que se pasa por parametros</returns>
        public void ObtenerGadgetsProyecto(Guid pProyectoID, DataWrapperProyecto pDataWrapperProyecto)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            List<ProyectoGadget> listaProyectoGadget = mEntityContext.ProyectoGadget.Where(proyectoGadget => proyectoGadget.ProyectoID.Equals(pProyectoID)).ToList();

            List<ProyectoGadgetContexto> listaProyectoGadgetContexto = mEntityContext.ProyectoGadgetContexto.Where(proyectoGadget => proyectoGadget.ProyectoID.Equals(pProyectoID)).ToList();

            foreach (ProyectoGadget gadget in listaProyectoGadget)
            {
                ProyectoGadgetContexto contexto = listaProyectoGadgetContexto.Where(x => x.GadgetID.Equals(gadget.GadgetID)).FirstOrDefault();
                gadget.ProyectoGadgetContexto = contexto;
            }

            string sqlGadgetIdioma = sqlSelectProyectoGadgetIdioma + " WHERE ProyectoGadgetIdioma.ProyectoID = " + IBD.GuidValor(pProyectoID);

            List<ProyectoGadgetIdioma> listaProyectoGadgetIdioma = mEntityContext.ProyectoGadgetIdioma.Where(proyectoGadgetIdioma => proyectoGadgetIdioma.ProyectoID.Equals(pProyectoID)).ToList();

            dataWrapperProyecto.ListaProyectoGadget = listaProyectoGadget;
            dataWrapperProyecto.ListaProyectoGadgetContexto = listaProyectoGadgetContexto;
            dataWrapperProyecto.ListaProyectoGadgetIdioma = listaProyectoGadgetIdioma;

            pDataWrapperProyecto.Merge(dataWrapperProyecto);

        }

        /// <summary>
        /// Obtiene los gadgets del tipo indicado de un proyecto que se le pasa como parámetro
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto del que queremos obtener los gadgets</param>
        /// <param name="pProyectoDS">Dataset de proyectos</param>
        /// <param name="pTipoUbicacionGadget">Indica la ubicación de los gadgets que se van a cargar(0-home, 1-ficha recursos)</param>
        /// <returns>DataSet con los gadgets del tipo indicado del proyecto que se pasa por parámetro</returns>
        public void ObtenerGadgetsProyectoUbicacion(Guid pProyectoID, DataWrapperProyecto pDataWrapperProyecto, TipoUbicacionGadget pTipoUbicacionGadget)
        {
            pDataWrapperProyecto.ListaProyectoGadget = mEntityContext.ProyectoGadget.Where(proyectoGadget => proyectoGadget.ProyectoID.Equals(pProyectoID) && proyectoGadget.TipoUbicacion.Equals((short)pTipoUbicacionGadget)).ToList();

            pDataWrapperProyecto.ListaProyectoGadgetContexto = mEntityContext.ProyectoGadgetContexto.Join(mEntityContext.ProyectoGadget, proyectoGadgetContexto => proyectoGadgetContexto.GadgetID, proyectoGadget => proyectoGadget.GadgetID, (proyectoGadgetContexto, proyectoGadget) => new
            {
                ProyectoGadgetContexto = proyectoGadgetContexto,
                TipoUbicacion = proyectoGadget.TipoUbicacion
            }).Where(gadget => gadget.TipoUbicacion.Equals((short)pTipoUbicacionGadget) && gadget.ProyectoGadgetContexto.ProyectoID.Equals(pProyectoID)).Select(gadget => gadget.ProyectoGadgetContexto).ToList();

            pDataWrapperProyecto.ListaProyectoGadgetIdioma = mEntityContext.ProyectoGadgetIdioma.Join(mEntityContext.ProyectoGadget, proyGadgetIdioma => proyGadgetIdioma.GadgetID, proyGadget => proyGadget.GadgetID, (proyGadgetIdioma, proyGadget) => new
            {
                ProyectGadgetIdioma = proyGadgetIdioma,
                TipoUbicacion = proyGadget.TipoUbicacion
            }).Where(gadget => gadget.TipoUbicacion.Equals((short)pTipoUbicacionGadget) && gadget.ProyectGadgetIdioma.ProyectoID.Equals(pProyectoID)).Select(gadget => gadget.ProyectGadgetIdioma).ToList();
        }

        /// <summary>
        /// Obtiene los gadgets y gadgets contexto de un proyecto que se le pasa como parámetro
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto del que queremos obtener los gadgets</param>
        /// <param name="pDataWrapperProyecto">Dataset de proyectos</param>
        /// <returns>DataSet con los gadgets del proyecto que se pasa por parametros</returns>
        public void ObtenerGadgetsProyectoOrigen(Guid pProyectoOrigenID, DataWrapperProyecto pDataWrapperProyecto)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            List<ProyectoGadgetContexto> listaProyectoGadgetContexto = mEntityContext.ProyectoGadgetContexto.Where(proyectoGadgetContexto => proyectoGadgetContexto.ProyectoOrigenID.Equals(pProyectoOrigenID)).ToList();
            dataWrapperProyecto.ListaProyectoGadgetContexto = listaProyectoGadgetContexto;

            List<ProyectoGadget> listaProyectoGadget = mEntityContext.ProyectoGadget.Join(mEntityContext.ProyectoGadgetContexto, gadget => gadget.GadgetID, contexto => contexto.GadgetID, (gadget, contexto) => new
            {
                ProyectoGadget = gadget,
                ProyectoOrigenID = contexto.ProyectoOrigenID
            }).Where(proyectoGadgetContexto => proyectoGadgetContexto.ProyectoOrigenID.Equals(pProyectoOrigenID)).Select(objeto => objeto.ProyectoGadget).ToList();

            dataWrapperProyecto.ListaProyectoGadget = listaProyectoGadget;
            pDataWrapperProyecto.Merge(dataWrapperProyecto);
        }

        /// <summary>
        /// Comprueba la existencia de gadgets de tipo Recursos Relacionados
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto origen</param>
        /// <returns>True si existen gadgets de tipo Recursos Relacionados</returns>
        public bool TieneGadgetRecursosRelacionados(Guid pProyectoID)
        {
            string sql = "select count(*) from ProyectoGadget where ProyectoID = " + IBD.GuidValor(pProyectoID) + " and Tipo = " + (short)TipoGadget.RecursosRelacionados;
            int numeroProyectoGadget = mEntityContext.ProyectoGadget.Where(proyectoGadget => proyectoGadget.ProyectoID.Equals(pProyectoID) && proyectoGadget.Tipo.Equals((short)TipoGadget.RecursosRelacionados)).ToList().Count;

            return numeroProyectoGadget > 0;
        }

        /// <summary>
        /// Obtiene el gadget que se le pasa por parametros
        /// </summary>
        /// <param name="pGadgetID">Clave del gadget</param>
        /// <param name="pDataWrapperProyecto">Dataset de proyectos</param>
        /// <returns>DataSet con los gadgets del proyecto que se pasa por parametros</returns>
        public void ObtenerGadget(Guid pGadgetID, DataWrapperProyecto pDataWrapperProyecto, Guid pProyectoID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            List<ProyectoGadget> listaProyectoGadget = mEntityContext.ProyectoGadget.Where(proyectoGadget => proyectoGadget.GadgetID.Equals(pGadgetID) && proyectoGadget.ProyectoID.Equals(pProyectoID)).ToList();
            dataWrapperProyecto.ListaProyectoGadget = listaProyectoGadget;

            List<ProyectoGadgetContexto> listaProyectoGadgetContexto = mEntityContext.ProyectoGadgetContexto.Where(proyectoGadgetContexto => proyectoGadgetContexto.GadgetID.Equals(pGadgetID) && proyectoGadgetContexto.ProyectoID.Equals(pProyectoID)).ToList();
            dataWrapperProyecto.ListaProyectoGadgetContexto = listaProyectoGadgetContexto;

            List<ProyectoGadgetIdioma> listaProyectoGadgetIdioma = mEntityContext.ProyectoGadgetIdioma.Where(proyectoGadgetIdioma => proyectoGadgetIdioma.GadgetID.Equals(pProyectoID)).ToList();
            dataWrapperProyecto.ListaProyectoGadgetIdioma = listaProyectoGadgetIdioma;

            pDataWrapperProyecto.Merge(dataWrapperProyecto);
        }
        /// <summary>
        /// Obtiene el gadget que se le pasa por parametros
        /// </summary>
        /// <param name="pGadgetID">Clave del gadget</param>
        /// <param name="pProyectoDS">Dataset de proyectos</param>
        /// <returns>DataSet con los gadgets del proyecto que se pasa por parametros</returns>
        public DataWrapperProyecto ObtenerDataSetGadget(Guid pGadgetID, Guid pProyectoID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();
            ObtenerGadget(pGadgetID, dataWrapperProyecto, pProyectoID);
            return dataWrapperProyecto;
        }

        /// <summary>
        /// Obtiene el gadget que se le pasa por parametros
        /// </summary>
        /// <param name="pNombreCorto">Nombrecorto del gadget</param>
        /// <param name="pDataWrapperProyecto">Dataset de proyectos</param>
        /// <returns>DataSet con los gadgets del proyecto que se pasa por parametros</returns>
        public void ObtenerGadgetContextoPorNombreCorto(string pNombreCorto, DataWrapperProyecto pDataWrapperProyecto)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            List<ProyectoGadgetContexto> listaProyectoGadgetContexto = mEntityContext.ProyectoGadgetContexto.Join(mEntityContext.ProyectoGadget, proyGadgetContext => proyGadgetContext.GadgetID, proyGadget => proyGadget.GadgetID, (proyGadgetContext, proyGadget) => new
            {
                ProyectoGadgetContexto = proyGadgetContext,
                NombreCorto = proyGadget.NombreCorto
            }).Where(objeto => objeto.NombreCorto.Equals(pNombreCorto)).Select(objeto => objeto.ProyectoGadgetContexto).ToList();
            dataWrapperProyecto.ListaProyectoGadgetContexto = listaProyectoGadgetContexto;

            List<ProyectoGadget> listaProyectoGadget = mEntityContext.ProyectoGadget.Where(proyectoGadget => proyectoGadget.NombreCorto.Equals(pNombreCorto)).ToList();
            dataWrapperProyecto.ListaProyectoGadget = listaProyectoGadget;
            pDataWrapperProyecto.Merge(dataWrapperProyecto);
        }

        /// <summary>
        /// Comprueba si existe un nombre corto de ProyectoGadget para el proyecto
        /// </summary>
        /// <param name="pNombreCortoGadget">Nombre corto del gadget</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>TRUE si algún gadget tiene ese nombre corto</returns>
        public bool ExisteNombreCortoProyectoGadget(string pNombreCortoGadget, Guid pProyectoID)
        {
            List<Guid> encontrado = mEntityContext.ProyectoGadget.Where(proyectoGadget => proyectoGadget.NombreCorto.ToUpper().Equals(pNombreCortoGadget.ToUpper()) && proyectoGadget.GadgetID.Equals(pProyectoID)).Select(proyectoGadget => proyectoGadget.GadgetID).ToList();

            return (encontrado.Count > 0);
        }
        /// <summary>
        /// Obtiene las pestañas de menú de un proyecto que se le pasa por parametro
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto del que queremos obtener los gadgets</param>
        /// <returns>Lista con las pestañas del proyecto que se pasa por parametros</returns>
        public Dictionary<Guid, string> ObtenerPestanyasProyectoNombre(Guid pProyectoID)
        {
            List<ProyectoPestanyaMenu> listaProyectoPestanyaMenu = new List<ProyectoPestanyaMenu>();
            var varProyectoPestanyaMenuQuery = mEntityContext.ProyectoPestanyaMenu.Where(proyPestanya => proyPestanya.TipoPestanya.Equals((short)TipoPestanyaMenu.Recursos) || proyPestanya.TipoPestanya.Equals((short)TipoPestanyaMenu.Preguntas) || proyPestanya.TipoPestanya.Equals((short)TipoPestanyaMenu.Debates) || proyPestanya.TipoPestanya.Equals((short)TipoPestanyaMenu.Encuestas) || proyPestanya.TipoPestanya.Equals((short)TipoPestanyaMenu.PersonasYOrganizaciones) || proyPestanya.TipoPestanya.Equals((short)TipoPestanyaMenu.BusquedaSemantica) || proyPestanya.TipoPestanya.Equals((short)TipoPestanyaMenu.BusquedaAvanzada)).Where(objeto => objeto.ProyectoID.Equals(pProyectoID));
            listaProyectoPestanyaMenu = varProyectoPestanyaMenuQuery.ToList();
            Dictionary<Guid, string> dicNombres = new Dictionary<Guid, string>();
            foreach (ProyectoPestanyaMenu pes in listaProyectoPestanyaMenu)
            {
                if (string.IsNullOrEmpty(pes.Nombre))
                {
                    dicNombres.Add(pes.PestanyaID, Enum.GetName(typeof(TipoPestanyaMenu), pes.TipoPestanya));
                }
                else
                {
                    dicNombres.Add(pes.PestanyaID, pes.Nombre);
                }
            }
            return dicNombres;
        }
        /// <summary>
        /// Obtiene las pestañas de un proyecto que se le pasa por parametros
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto del que queremos obtener los gadgets</param>
        /// <param name="pDataWrapperProyecto">Dataset de proyectos</param>
        /// <param name="pOmitirGenericas"></param>
        /// <returns>DataSet con las pestañas del proyecto que se pasa por parametros</returns>
        public void ObtenerPestanyasProyecto(Guid? pProyectoID, DataWrapperProyecto pDataWrapperProyecto, bool pOmitirGenericas)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            List<ProyectoPestanyaMenu> listaProyectoPestanyaMenu = new List<ProyectoPestanyaMenu>();
            var listaProyectoPestanyaMenuQuery = mEntityContext.ProyectoPestanyaMenu.AsQueryable();

            List<ProyectoPestanyaCMS> listaProyectoPestanyaCMS = new List<ProyectoPestanyaCMS>();
            var varProyectoPestanyaCMSQuery = mEntityContext.ProyectoPestanyaCMS.Join(mEntityContext.ProyectoPestanyaMenu, proyectoPestanyaCMS => proyectoPestanyaCMS.PestanyaID, proyectoPestanyaMenu => proyectoPestanyaMenu.PestanyaID, (proyectoPestanyaCMS, proyectoPestanyaMenu) => new
            {
                ProyectoPestanyaCMS = proyectoPestanyaCMS,
                ProyectoPestanyaMenu = proyectoPestanyaMenu
            });

            List<ProyectoPestanyaBusqueda> listaProyectoPestanyaBusqueda = new List<ProyectoPestanyaBusqueda>();
            var varProyectoPestanyaBusquedaQuery = mEntityContext.ProyectoPestanyaBusqueda.Join(mEntityContext.ProyectoPestanyaMenu, proyectoPestanyaBusqueda => proyectoPestanyaBusqueda.PestanyaID, proyectoPestanyaMenu => proyectoPestanyaMenu.PestanyaID, (proyectoPestanyaBusqueda, proyectoPestanyaMenu) => new
            {
                ProyectoPestanyaBusqueda = proyectoPestanyaBusqueda,
                ProyectoPestanyaMenu = proyectoPestanyaMenu
            });

            List<ProyectoPestanyaMenuRolGrupoIdentidades> listProyectoPestanyaMenuRolGrupoIdentidades = new List<ProyectoPestanyaMenuRolGrupoIdentidades>();
            var varProyectoPestanyaMenuRolGrupoIdentidadesQuery = mEntityContext.ProyectoPestanyaMenuRolGrupoIdentidades.Join(mEntityContext.ProyectoPestanyaMenu, proyectoPestanyaMenuRolGrupoIdentidades => proyectoPestanyaMenuRolGrupoIdentidades.PestanyaID, proyectoPestanya => proyectoPestanya.PestanyaID, (proyectoPestanyaMenuRolGrupoIdentidades, proyectoPestanya) => new
            {
                ProyectoPestanyaMenuRolGrupoIdentidades = proyectoPestanyaMenuRolGrupoIdentidades,
                ProyectoPestanyaMenu = proyectoPestanya
            });

            List<ProyectoPestanyaMenuRolIdentidad> listProyectoPestanyaMenuRolIdentidad = new List<ProyectoPestanyaMenuRolIdentidad>();
            var varProyectoPestanyaMenuRolIdentidadQuery = mEntityContext.ProyectoPestanyaMenuRolIdentidad.Join(mEntityContext.ProyectoPestanyaMenu, proyectoPestanyaMenuRolIdentidad => proyectoPestanyaMenuRolIdentidad.PestanyaID, proyectoPestanyaMenu => proyectoPestanyaMenu.PestanyaID, (proyectoPestanyaMenuRolIdentidad, proyectoPestanyaMenu) => new
            {
                ProyectoPestanyaMenuRolIdentidad = proyectoPestanyaMenuRolIdentidad,
                ProyectoPestanyaMenu = proyectoPestanyaMenu
            });

            if (pProyectoID.HasValue)
            {
                listaProyectoPestanyaMenuQuery = listaProyectoPestanyaMenuQuery.Where(objeto => objeto.ProyectoID.Equals(pProyectoID.Value));
                varProyectoPestanyaCMSQuery = varProyectoPestanyaCMSQuery.Where(objeto => objeto.ProyectoPestanyaMenu.ProyectoID.Equals(pProyectoID.Value));
                varProyectoPestanyaBusquedaQuery = varProyectoPestanyaBusquedaQuery.Where(objeto => objeto.ProyectoPestanyaMenu.ProyectoID.Equals(pProyectoID.Value));
                varProyectoPestanyaMenuRolGrupoIdentidadesQuery = varProyectoPestanyaMenuRolGrupoIdentidadesQuery.Where(objeto => objeto.ProyectoPestanyaMenu.ProyectoID.Equals(pProyectoID.Value));
                varProyectoPestanyaMenuRolIdentidadQuery = varProyectoPestanyaMenuRolIdentidadQuery.Where(objeto => objeto.ProyectoPestanyaMenu.ProyectoID.Equals(pProyectoID.Value));
            }
            if (pOmitirGenericas)
            {
                listaProyectoPestanyaMenuQuery = listaProyectoPestanyaMenuQuery.Where(objeto => !string.IsNullOrEmpty(objeto.Ruta));
                varProyectoPestanyaCMSQuery = varProyectoPestanyaCMSQuery.Where(objeto => !string.IsNullOrEmpty(objeto.ProyectoPestanyaMenu.Ruta));
                varProyectoPestanyaBusquedaQuery = varProyectoPestanyaBusquedaQuery.Where(objeto => !string.IsNullOrEmpty(objeto.ProyectoPestanyaMenu.Ruta));
                varProyectoPestanyaMenuRolGrupoIdentidadesQuery = varProyectoPestanyaMenuRolGrupoIdentidadesQuery.Where(objeto => !string.IsNullOrEmpty(objeto.ProyectoPestanyaMenu.Ruta));
                varProyectoPestanyaMenuRolIdentidadQuery = varProyectoPestanyaMenuRolIdentidadQuery.Where(objeto => !string.IsNullOrEmpty(objeto.ProyectoPestanyaMenu.Ruta));
            }

            listaProyectoPestanyaMenu = listaProyectoPestanyaMenuQuery.OrderBy(proyectoPestMenu => proyectoPestMenu.Orden).ToList();
            listaProyectoPestanyaCMS = varProyectoPestanyaCMSQuery.Select(objeto => objeto.ProyectoPestanyaCMS).ToList();
            listaProyectoPestanyaBusqueda = varProyectoPestanyaBusquedaQuery.Select(objeto => objeto.ProyectoPestanyaBusqueda).ToList();
            listProyectoPestanyaMenuRolGrupoIdentidades = varProyectoPestanyaMenuRolGrupoIdentidadesQuery.Select(objeto => objeto.ProyectoPestanyaMenuRolGrupoIdentidades).ToList();
            listProyectoPestanyaMenuRolIdentidad = varProyectoPestanyaMenuRolIdentidadQuery.Select(objeto => objeto.ProyectoPestanyaMenuRolIdentidad).ToList();

            dataWrapperProyecto.ListaProyectoPestanyaMenu = listaProyectoPestanyaMenu;
            dataWrapperProyecto.ListaProyectoPestanyaCMS = listaProyectoPestanyaCMS;
            dataWrapperProyecto.ListaProyectoPestanyaBusqueda = listaProyectoPestanyaBusqueda;
            dataWrapperProyecto.ListaProyectoPestanyaMenuRolGrupoIdentidades = listProyectoPestanyaMenuRolGrupoIdentidades;
            dataWrapperProyecto.ListaProyectoPestanyaMenuRolIdentidad = listProyectoPestanyaMenuRolIdentidad;
            pDataWrapperProyecto.Merge(dataWrapperProyecto);
        }

        public List<ProyectoPestanyaMenu> ObtenerProyectoPestanyaMenuPorProyectoID(Guid pProyectoID)
        {
            return mEntityContext.ProyectoPestanyaMenu.Where(item => item.ProyectoID.Equals(pProyectoID)).OrderBy(item => item.Orden).ToList();
        }

        public List<ProyectoPestanyaMenu> ObtenerPestanyasDeProyectoSegunPrivacidadDeIdentidad(Guid pProyectoID, Guid pIdentidadID)
        {
            Guid perfilID = mEntityContext.Identidad.Where(item => item.IdentidadID.Equals(pIdentidadID)).Select(item => item.PerfilID).FirstOrDefault();

            List<Guid> listaIdentidadesPerfil = mEntityContext.Identidad.Where(item => item.PerfilID.Equals(perfilID)).Select(item => item.IdentidadID).ToList();

            List<Guid> listaGruposPerteneceIdentidad = mEntityContext.GrupoIdentidadesParticipacion.Where(item => listaIdentidadesPerfil.Contains(item.IdentidadID)).Select(item => item.GrupoID).ToList();


            var subconsultaPestanyasGrupos = mEntityContext.ProyectoPestanyaMenuRolGrupoIdentidades.Where(item => listaGruposPerteneceIdentidad.Contains(item.GrupoID)).Select(item => item.PestanyaID);
            var subconsultaPestanyasPerfil = mEntityContext.ProyectoPestanyaMenuRolIdentidad.Where(item => item.PerfilID.Equals(perfilID)).Select(item => item.PestanyaID);

            var conjuntoPestanyasIDs = subconsultaPestanyasGrupos.Union(subconsultaPestanyasPerfil).Distinct();

            return mEntityContext.ProyectoPestanyaMenu.Where(item => item.ProyectoID.Equals(pProyectoID) && (conjuntoPestanyasIDs.Contains(item.PestanyaID) && item.Privacidad == 2 || item.Privacidad == 0)).ToList();
        }

        /// <summary>
        /// Obtiene las pestañas de un proyecto que se le pasa por parametros
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto del que queremos obtener los gadgets</param>
        /// <returns>DataSet con las páginas html del proyecto que se pasa por parametros</returns>
        public DataWrapperProyecto ObtenerPaginasHtmlProyecto(Guid pProyectoID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();
            dataWrapperProyecto.ListaProyectoPaginaHtml = mEntityContext.ProyectoPaginaHtml.Where(proyectoPaginaHTML => proyectoPaginaHTML.ProyectoID.Equals(pProyectoID)).ToList();
            return dataWrapperProyecto;
        }

        /// <summary>
        /// Obtiene la tabla RecursosRelacionadosPresentacion del proyecto
        /// </summary>
        /// <param name="pProyectoID">Clave del gadget</param>
        /// <returns>DataSet con RecursosRelacionadosPresentacion</returns>
        public DataWrapperProyecto ObtenerRecursosRelacionadosPresentacion(Guid pProyectoID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();
            dataWrapperProyecto.ListaRecursosRelacionadosPresentacion = mEntityContext.RecursosRelacionadosPresentacion.Where(recurso => recurso.ProyectoID.Equals(pProyectoID)).ToList();
            return dataWrapperProyecto;
        }

        /// <summary>
        /// Obtiene los proyectos relacionados de un proyecto que se le pasa como parámetro
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto del que queremos obtener los proyectos</param>
        /// <param name="pDataWrapperProyecto">Dataset de proyectos</param>
        /// <returns>DataSet con los proyectos del proyecto que se pasa por parametros</returns>
        public void ObtenerProyectosRelacionados(Guid pProyectoID, DataWrapperProyecto pDataWrapperProyecto)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();
            dataWrapperProyecto.ListaProyectoRelacionado = mEntityContext.ProyectoRelacionado.Where(proyectoRelacionado => proyectoRelacionado.ProyectoID.Equals(pProyectoID)).ToList();
            pDataWrapperProyecto.Merge(dataWrapperProyecto);
        }


        /// <summary>
        /// Actualiza los proyectos
        /// </summary>
        /// <param name="pProyectoDS">Dataset de proyectos para actualizar</param>
        /// <param name="pRecalculandoProyectosMasActivos">TRUE si se están actualizando los proyectos más activos</param>
        public void ActualizarProyectos(bool pPeticionIntegracionContinua = false)
        {
            mEntityContext.SaveChanges();
        }


        private List<DataRow> OrdenarFilasPestanyasmenu(DataRow[] filasPestanyaMenu, DataTable pTablaProyectoPestanyaMenu)
        {
            List<DataRow> listaOrdenada = new List<DataRow>();
            foreach (DataRow fila in filasPestanyaMenu)
            {
                listaOrdenada.Add(fila);
                if (pTablaProyectoPestanyaMenu.Select("PestanyaPadreID='" + fila["PestanyaID"] + "'").Length > 0)
                {
                    listaOrdenada.AddRange(OrdenarFilasPestanyasmenu(pTablaProyectoPestanyaMenu.Select("PestanyaPadreID='" + fila["PestanyaID"] + "'"), pTablaProyectoPestanyaMenu));
                }
            }

            return listaOrdenada;
        }



        /// <summary>
        /// Comprueba si existe un nombre corto de proyecto en BD
        /// </summary>
        /// <param name="pNombreCortoProyecto"></param>
        /// <returns>TRUE si existe un nombroCorto en BD igual al pasado por parámetro</returns>
        public bool ExisteNombreCortoEnBD(object pNombreCortoProyecto)
        {
            List<Guid> listaGuidEncontrado = mEntityContext.Proyecto.Where(proyecto => proyecto.NombreCorto.Equals(((string)pNombreCortoProyecto))).Select(proyecto => proyecto.ProyectoID).ToList();
            return (listaGuidEncontrado.Count > 0);
        }

        /// <summary>
        /// Comprueba si existe un nombre de proyecto en BD
        /// </summary>
        /// <param name="pNombreProyecto">Nombre de proyecto</param>
        /// <returns>TRUE si existe un nombre en BD igual al pasado por parámetro</returns>
        public bool ExisteNombreEnBD(object pNombreProyecto)
        {
            List<Guid> listaGuidEncontrado = mEntityContext.Proyecto.Where(proyecto => proyecto.Nombre.ToUpper().Equals(((string)pNombreProyecto).ToString())).Select(proyect => proyect.ProyectoID).ToList();

            return (listaGuidEncontrado.Count > 0);
        }

        /// <summary>
        /// Comprueba si algún usuario de la organización (personas con usuario vinculadas con la organización) 
        /// es administrador del proyecto
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>TRUE si se encuentra algun usuario de la organización que sea administrador del proyecto</returns>
        public bool EsAlguienDeLAOrganizacionAdministradorProyecto(Guid pOrganizacionID, Guid pProyectoID)
        {
            string sqlSelectAdministradores = "SELECT * FROM ( SELECT Perfil.OrganizacionID FROM Identidad INNER JOIN perfil ON perfil.perfilID = identidad.perfilID WHERE Identidad.identidadID IN ( SELECT IdentidadID FROM AdministradorProyecto INNER JOIN proyectousuarioIdentidad on proyectousuarioIdentidad.UsuarioID = AdministradorProyecto.UsuarioID WHERE (AdministradorProyecto.ProyectoID = " + IBD.GuidParamValor("proyectoID") + ") AND (proyectousuarioIdentidad.ProyectoID = " + IBD.GuidParamValor("proyectoID") + ") AND AdministradorProyecto.Tipo = " + (short)TipoRolUsuario.Administrador + " )AND Perfil.OrganizacionID IS NOT NULL ) OrganizacionesAdmin WHERE OrganizacionID = " + IBD.GuidParamValor("organizacionID");


            List<Guid> listaIdentidadID = mEntityContext.AdministradorProyecto.Join(mEntityContext.ProyectoUsuarioIdentidad, adminProy => adminProy.UsuarioID, proyUsuarioIdentidad => proyUsuarioIdentidad.UsuarioID, (adminProy, proyUsuarioIdentidad) => new
            {
                AdministradorProyecto = adminProy,
                ProyectoUsuarioIdentidad = proyUsuarioIdentidad
            }).Where(objeto => objeto.AdministradorProyecto.ProyectoID.Equals(pProyectoID) && objeto.ProyectoUsuarioIdentidad.ProyectoID.Equals(pProyectoID) && objeto.AdministradorProyecto.Tipo.Equals((short)TipoRolUsuario.Administrador)).Select(objeto => objeto.ProyectoUsuarioIdentidad.IdentidadID).ToList();

            List<Guid?> administradores = mEntityContext.Identidad.Join(mEntityContext.Perfil, identidad => identidad.PerfilID, perfil => perfil.PerfilID, (identidad, perfil) => new
            {
                Idenitdad = identidad,
                Perfil = perfil
            }).Where(objeto => listaIdentidadID.Contains(objeto.Idenitdad.IdentidadID) && objeto.Perfil.OrganizacionID != null).Select(objeto => objeto.Perfil.OrganizacionID).ToList().Where(orgID => orgID.Equals(pOrganizacionID)).ToList();


            bool EsAdministrador = (administradores.Count > 0);


            return (EsAdministrador);
        }

        /// <summary>
        /// Carga la presentación de todos los documentos semánticos en una comunidad 
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>    
        public DataWrapperProyecto ObtenerPresentacionSemantico(Guid pProyectoID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            #region Listado
            dataWrapperProyecto.ListaPresentacionListadoSemantico = mEntityContext.PresentacionListadoSemantico.Where(presentacionListSemantico => presentacionListSemantico.ProyectoID.Equals(pProyectoID)).OrderBy(presentacionListSemantico => presentacionListSemantico.Orden).ToList();

            dataWrapperProyecto.ListaPresentacionPestanyaListadoSemantico = mEntityContext.PresentacionPestanyaListadoSemantico.Where(presentacionPestListadoSeman => presentacionPestListadoSeman.ProyectoID.Equals(pProyectoID)).OrderBy(presentacionPestListadoSeman => presentacionPestListadoSeman.Orden).ToList();
            #endregion

            #region Mosaico
            dataWrapperProyecto.ListaPresentacionMosaicoSemantico = mEntityContext.PresentacionMosaicoSemantico.Where(presentacionMosaicoSmenatico => presentacionMosaicoSmenatico.ProyectoID.Equals(pProyectoID)).OrderBy(presentacion => presentacion.Orden).ToList();

            dataWrapperProyecto.ListaPresentacionPestanyaMosaicoSemantico = mEntityContext.PresentacionPestanyaMosaicoSemantico.Where(presentacionPestanyaMosaicoSemantico => presentacionPestanyaMosaicoSemantico.ProyectoID.Equals(pProyectoID)).OrderBy(presentacion => presentacion.Orden).ToList();
            #endregion

            #region Mapa
            dataWrapperProyecto.ListaPresentacionMapaSemantico = mEntityContext.PresentacionMapaSemantico.Where(presentacionMapa => presentacionMapa.ProyectoID.Equals(pProyectoID)).OrderBy(presentacionMapa => presentacionMapa.Orden).ToList();

            dataWrapperProyecto.ListaPresentacionPestanyaMapaSemantico = mEntityContext.PresentacionPestanyaMapaSemantico.Where(presentacionPestanyaMapa => presentacionPestanyaMapa.ProyectoID.Equals(pProyectoID)).OrderBy(presentacion => presentacion.Orden).ToList();

            #endregion

            #region Dataset
            dataWrapperProyecto.ListaPresentacionPersonalizadoSemantico = mEntityContext.PresentacionPersonalizadoSemantico.Where(presentacionPersonalizadoSemantico => presentacionPersonalizadoSemantico.ProyectoID.Equals(pProyectoID)).OrderBy(presentacionPersonalizadoSemantico => presentacionPersonalizadoSemantico.Orden).ToList();
            #endregion

            #region Contextos

            dataWrapperProyecto.ListaRecursosRelacionadosPresentacion = mEntityContext.RecursosRelacionadosPresentacion.Where(recursosRelacionados => recursosRelacionados.ProyectoID.Equals(pProyectoID)).OrderBy(recursosRelacionados => recursosRelacionados.Orden).ToList();
            #endregion
            return dataWrapperProyecto;
        }

        public List<PresentacionMapaSemantico> ObtenerListaPresentacionMapaSemantico(Guid pProyectoID, string pNombreOnto = "")
        {
            if (string.IsNullOrEmpty(pNombreOnto))
            {
                return mEntityContext.PresentacionMapaSemantico.Where(item => item.ProyectoID.Equals(pProyectoID)).OrderBy(item => item.Orden).ToList();
            }
            else
            {
                return mEntityContext.PresentacionMapaSemantico.Where(item => item.ProyectoID.Equals(pProyectoID) && item.Ontologia.ToLower().Contains($"{pNombreOnto.ToLower()}.owl")).OrderBy(item => item.Orden).ToList();
            }
        }

        /// <summary>
        /// Obtiene los tipo de documentos permitidos para un rol de usuario en un determinado proyecto.
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto en el que se hace la comprobación</param>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <returns>Lista con los tipos de documentos permitidos para el rol</returns>
        public List<TiposDocumentacion> ObtenerTiposDocumentosPermitidosUsuarioEnProyectoPorUsuID(Guid pProyectoID, Guid pUsuarioID)
        {
            List<AdministradorProyecto> listaAdministradoresProyecto = new List<AdministradorProyecto>();
            TipoRolUsuario tipoRolUsuario = TipoRolUsuario.Usuario;
            listaAdministradoresProyecto = mEntityContext.AdministradorProyecto.Where(admin => admin.ProyectoID.Equals(pProyectoID) && admin.UsuarioID.Equals(pUsuarioID)).ToList();

            if (listaAdministradoresProyecto.Count == 0)
            {
                tipoRolUsuario = TipoRolUsuario.Usuario;
            }
            else
            {
                tipoRolUsuario = (TipoRolUsuario)listaAdministradoresProyecto.First().Tipo;
            }

            return ObtenerTiposDocumentosPermitidosUsuarioEnProyecto(pProyectoID, tipoRolUsuario);
        }
        /// <summary>
        /// Carga la presentación de todos los documentos semánticos en una comunidad 
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>    
        public DataWrapperProyecto ObtenerPresentacionListadoSemantico(Guid pProyectoID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            #region Listado
            string sqlSelectPresentacionListadoSemanticoDeProyecto = sqlSelectPresentacionListadoSemantico + " WHERE PresentacionListadoSemantico.ProyectoID = " + IBD.ToParam("ProyectoID") + " Order by Orden ASC";

            dataWrapperProyecto.ListaPresentacionListadoSemantico = mEntityContext.PresentacionListadoSemantico.Where(presentacionListadoSmenantico => presentacionListadoSmenantico.ProyectoID.Equals(pProyectoID)).OrderBy(presentacion => presentacion.Orden).ToList();

            string sqlSelectPresentacionPestanyaListadoSemanticoDeProyecto = sqlSelectPresentacionPestanyaListadoSemantico + " WHERE PresentacionPestanyaListadoSemantico.ProyectoID = " + IBD.ToParam("ProyectoID") + " Order by Orden ASC";

            dataWrapperProyecto.ListaPresentacionPestanyaListadoSemantico = mEntityContext.PresentacionPestanyaListadoSemantico.Where(presentacionPestanya => presentacionPestanya.ProyectoID.Equals(pProyectoID)).OrderBy(presentacion => presentacion.Orden).ToList();
            #endregion

            return dataWrapperProyecto;
        }

        /// <summary>
        /// Carga la presentación de todos los documentos semánticos en una comunidad 
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>    
        public DataWrapperProyecto ObtenerPresentacionMosaicoSemantico(Guid pProyectoID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            #region Mosaico

            dataWrapperProyecto.ListaPresentacionMosaicoSemantico = mEntityContext.PresentacionMosaicoSemantico.Where(presentacionMosaicoSmenantico => presentacionMosaicoSmenantico.ProyectoID.Equals(pProyectoID)).OrderBy(presentacion => presentacion.Orden).ToList();

            dataWrapperProyecto.ListaPresentacionPestanyaMosaicoSemantico = mEntityContext.PresentacionPestanyaMosaicoSemantico.Where(presentacionPestanya => presentacionPestanya.ProyectoID.Equals(pProyectoID)).OrderBy(presentacion => presentacion.Orden).ToList();

            return dataWrapperProyecto;
        }

        /// <summary>
        /// Carga la presentación de todos los documentos semánticos en una comunidad 
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>    
        public DataWrapperProyecto ObtenerPresentacionMapaSemantico(Guid pProyectoID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            #region Mapa

            dataWrapperProyecto.ListaPresentacionMapaSemantico = mEntityContext.PresentacionMapaSemantico.Where(presentacionMapaSmenantico => presentacionMapaSmenantico.ProyectoID.Equals(pProyectoID)).OrderBy(presentacion => presentacion.Orden).ToList();

            dataWrapperProyecto.ListaPresentacionPestanyaMapaSemantico = mEntityContext.PresentacionPestanyaMapaSemantico.Where(presentacionPestanya => presentacionPestanya.ProyectoID.Equals(pProyectoID)).OrderBy(presentacion => presentacion.Orden).ToList();
            #endregion

            return dataWrapperProyecto;
        }




        /// <summary>
        /// Obtiene una lista con las tablas del dataSet en orden
        /// </summary>
        /// <param name="pDataSet">DataSet del que se quieren ordenar las tablas</param>
        /// <returns>Lista con las tablas del dataSet en orden</returns>
        //public List<DataTable> ObtenerOrdenTablas(ProyectoDS pProyectoDS)
        //{
        //    List<DataTable> listaTablas = new List<DataTable>();

        //    listaTablas.Add(pProyectoDS.Proyecto);
        //    listaTablas.Add(pProyectoDS.AdministradorProyecto);
        //    listaTablas.Add(pProyectoDS.AdministradorGrupoProyecto);
        //    listaTablas.Add(pProyectoDS.ProyectosMasActivos);
        //    listaTablas.Add(pProyectoDS.ProyectoAgCatTesauro);
        //    listaTablas.Add(pProyectoDS.NivelCertificacion);
        //    listaTablas.Add(pProyectoDS.TipoDocDispRolUsuarioProy);
        //    listaTablas.Add(pProyectoDS.TipoOntoDispRolUsuarioProy);
        //    listaTablas.Add(pProyectoDS.ProyectoCerradoTmp);
        //    listaTablas.Add(pProyectoDS.ProyectoCerrandose);
        //    listaTablas.Add(pProyectoDS.ProyectoRelacionado);
        //    listaTablas.Add(pProyectoDS.ProyectoGadget);
        //    listaTablas.Add(pProyectoDS.ProyectoGadgetContexto);
        //    //listaTablas.Add(pProyectoDS.ProyectoGadgetContextoHTMLplano);
        //    listaTablas.Add(pProyectoDS.ProyectoGadgetIdioma);
        //    listaTablas.Add(pProyectoDS.RecursosRelacionadosPresentacion);
        //    listaTablas.Add(pProyectoDS.ProyectoPaginaHtml);
        //    listaTablas.Add(pProyectoDS.ProyectoPasoRegistro);
        //    listaTablas.Add(pProyectoDS.ProyectoPestanyaMenu);
        //    listaTablas.Add(pProyectoDS.ProyectoPestanyaCMS);
        //    listaTablas.Add(pProyectoDS.ProyectoPestanyaBusqueda);
        //    listaTablas.Add(pProyectoDS.ProyectoPestanyaMenuRolGrupoIdentidades);
        //    listaTablas.Add(pProyectoDS.ProyectoPestanyaMenuRolIdentidad);
        //    listaTablas.Add(pProyectoDS.ProyectoSearchPersonalizado);
        //    listaTablas.Add(pProyectoDS.ProyectoServicioExterno);
        //    listaTablas.Add(pProyectoDS.EcosistemaServicioExterno);

        //    return listaTablas;
        //}

        /// <summary>
        /// Elimina los registros de proyectos del dataset pasado como parámetro
        /// </summary>
        /// <param name="pProyectoDS">Dataset de proyectos</param>
        public void EliminarProyectos()
        {
            mEntityContext.SaveChanges();
            //#region Eliminar tablas que dependen de proyecto

            // ** DAVID
            // A FECHA 13/03/2009 SÓLO ESTÁN LAS TABLAS QUE CONTIENEN DATOS INTRODUCIDOS EN EL ASISTENTE DE CREACIÓN
            // CUANDO SE HAGA LA ELIMINACIÓN COMPLETA SE DEBEN AÑADIR TODAS LAS TABLAS QUE DEPENDAN DE PROYECTO
            // **

            //ProyectoDS.ProyectoDataTable proyectosEliminados = (ProyectoDS.ProyectoDataTable)pProyectoDS.Proyecto.GetChanges(DataRowState.Deleted);

            //if (proyectosEliminados != null)
            //{
            //    string filtroBorrado = " WHERE (ProyectoID = " + IBD.GuidParamValor("proyectoID") + ")";

            //    foreach (ProyectoDS.ProyectoRow filaProyecto in proyectosEliminados.Rows)
            //    {
            //        Guid proyectoID = (Guid)filaProyecto["ProyectoID", DataRowVersion.Original];
            //        Guid organizacionID = (Guid)filaProyecto["OrganizacionID", DataRowVersion.Original];

            //        // Configurar los parámetros del comando sólo una vez
            //        DbCommand comandoSQL = ObtenerComando("Prueba");
            //        AgregarParametro(comandoSQL, IBD.ToParam("proyectoID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(proyectoID));

            //        #region Tablas de ProyectoDS

            //        // Eliminar las filas de ProyectosMasActivos
            //        comandoSQL.CommandText = "DELETE FROM ProyectosMasActivos" + filtroBorrado;
            //        ActualizarBaseDeDatos(comandoSQL);

            //        // Eliminar las filas de NivelCertificacion
            //        comandoSQL.CommandText = "DELETE FROM NivelCertificacion" + filtroBorrado;
            //        ActualizarBaseDeDatos(comandoSQL);

            //        #endregion

            //        #region Tablas de EstructuraDS

            //        // Eliminar las filas de MetaEstructura
            //        comandoSQL.CommandText = "DELETE FROM MetaEstructura" + filtroBorrado;
            //        ActualizarBaseDeDatos(comandoSQL);

            //        // Eliminar las filas de ElementoEstructura
            //        comandoSQL.CommandText = "DELETE FROM ElementoEstructura" + filtroBorrado;
            //        ActualizarBaseDeDatos(comandoSQL);

            //        // Eliminar las filas de Estructura
            //        comandoSQL.CommandText = "DELETE FROM Estructura" + filtroBorrado;
            //        ActualizarBaseDeDatos(comandoSQL);

            //        #endregion

            //        #region Tablas de LibroDS

            //        // Eliminar las filas de Libro
            //        comandoSQL.CommandText = "DELETE FROM Libro" + filtroBorrado;
            //        ActualizarBaseDeDatos(comandoSQL);

            //        #endregion

            //        #region Tablas de ProcesoDS

            //        // Eliminar las filas de Proceso
            //        // ** Las tablas que se encuentan en GrupoDS se borran después
            //        comandoSQL.CommandText = "DELETE FROM Proceso" + filtroBorrado;
            //        ActualizarBaseDeDatos(comandoSQL);

            //        #endregion

            //        #region Tablas de ObjetivoDS

            //        // Eliminar las filas de Objetivo
            //        // ** Las tablas que se encuentan en GrupoDS se borran después
            //        comandoSQL.CommandText = "DELETE FROM Objetivo" + filtroBorrado;
            //        ActualizarBaseDeDatos(comandoSQL);

            //        #endregion

            //        #region Tablas de GrupoFuncionalDS

            //        // Eliminar las filas de GrupoFuncional
            //        // ** Las tablas que se encuentan en GrupoDS se borran después
            //        comandoSQL.CommandText = "DELETE FROM GrupoFuncional" + filtroBorrado;
            //        ActualizarBaseDeDatos(comandoSQL);

            //        #endregion

            //        #region Tablas de TesauroDS

            //        // Obtener el ID del tesauro de proyecto para eliminarlo de la tabla Tesauro
            //        Guid idTesauro = Guid.Empty;

            //        using (TesauroAD tesauroAD = new TesauroAD())
            //        {
            //            idTesauro = tesauroAD.ObtenerIDTesauroDeProyecto(proyectoID);
            //        }
            //        // Eliminar las filas de TesauroProyecto
            //        comandoSQL.CommandText = "DELETE FROM TesauroProyecto" + filtroBorrado;
            //        ActualizarBaseDeDatos(comandoSQL);

            //        // Eliminar las filas de Tesauro
            //        comandoSQL.CommandText = "DELETE FROM Tesauro WHERE TesauroID = " + IBD.GuidValor(idTesauro);
            //        ActualizarBaseDeDatos(comandoSQL);

            //        #endregion

            //        #region Tablas de DocumentacionDS

            //        // Obtener el ID de la base de recursos de proyecto para eliminarlo de la tabla BaseRecursos
            //        Guid idBaseRecursos = Guid.Empty;

            //        using (DocumentacionAD documentacionAD = new DocumentacionAD())
            //        {
            //            DocumentacionDS documentacionDS = new DocumentacionDS();
            //            documentacionAD.ObtenerBaseRecursosProyecto(documentacionDS, proyectoID, organizacionID);

            //            if (documentacionDS.BaseRecursos.Rows.Count.Equals(1))
            //            {
            //                idBaseRecursos = ((DocumentacionDS.BaseRecursosRow)documentacionDS.BaseRecursos.Rows[0]).BaseRecursosID;
            //            }
            //            documentacionDS.Dispose();
            //        }
            //        // Eliminar las filas de BaseRecursosProyecto
            //        comandoSQL.CommandText = "DELETE FROM BaseRecursosProyecto" + filtroBorrado;
            //        ActualizarBaseDeDatos(comandoSQL);

            //        // Eliminar las filas de BaseRecursos
            //        comandoSQL.CommandText = "DELETE FROM BaseRecursos WHERE BaseRecursosID = " + IBD.GuidValor(idBaseRecursos);
            //        ActualizarBaseDeDatos(comandoSQL);

            //        #endregion

            //        #region Tablas de GrupoDS

            //        // Eliminar las filas de GestorGrupo
            //        comandoSQL.CommandText = "DELETE FROM GestorGrupo" + filtroBorrado;
            //        ActualizarBaseDeDatos(comandoSQL);

            //        // Eliminar las filas de Grupo
            //        comandoSQL.CommandText = "DELETE FROM Grupo" + filtroBorrado;
            //        ActualizarBaseDeDatos(comandoSQL);

            //        // ** Las tablas que se encuentan en EntidadDS se borran después

            //        #endregion

            //        #region Tablas de EntidadDS

            //        // Eliminar las filas de EntidadGnoss
            //        comandoSQL.CommandText = "DELETE FROM EntidadGnoss" + filtroBorrado;
            //        ActualizarBaseDeDatos(comandoSQL);

            //        #endregion

            //        #region Tablas de OrganizacionDS

            //        // Eliminar las filas de OrganizacionParticipaProy
            //        comandoSQL.CommandText = "DELETE FROM OrganizacionParticipaProy" + filtroBorrado;
            //        ActualizarBaseDeDatos(comandoSQL);

            //        // Eliminar las filas de HistoricoOrgParticipaProy
            //        comandoSQL.CommandText = "DELETE FROM HistoricoOrgParticipaProy" + filtroBorrado;
            //        ActualizarBaseDeDatos(comandoSQL);

            //        #endregion

            //        #region Tablas de UsuarioDS

            //        // Eliminar las filas de InicioSesion
            //        comandoSQL.CommandText = "DELETE FROM InicioSesion" + filtroBorrado;
            //        ActualizarBaseDeDatos(comandoSQL);

            //        // Eliminar las filas de HistoricoProyectoUsuario
            //        comandoSQL.CommandText = "DELETE FROM HistoricoProyectoUsuario" + filtroBorrado;
            //        ActualizarBaseDeDatos(comandoSQL);

            //        // Eliminar las filas de ProyectoRolUsuario
            //        comandoSQL.CommandText = "DELETE FROM ProyectoRolUsuario" + filtroBorrado;
            //        ActualizarBaseDeDatos(comandoSQL);

            //        // Eliminar las filas de ProyectoRolGrupoUsuario
            //        comandoSQL.CommandText = "DELETE FROM ProyectoRolGrupoUsuario" + filtroBorrado;
            //        ActualizarBaseDeDatos(comandoSQL);

            //        // Eliminar las filas de ProyectoUsuarioIdentidad
            //        comandoSQL.CommandText = "DELETE FROM ProyectoUsuarioIdentidad" + filtroBorrado;
            //        ActualizarBaseDeDatos(comandoSQL);

            //        #endregion

            //        #region Tablas de IdentidadDS

            //        // Eliminar las filas de Identidad
            //        comandoSQL.CommandText = "DELETE FROM Identidad" + filtroBorrado;
            //        ActualizarBaseDeDatos(comandoSQL);

            //        #endregion

            //        proyectosEliminados.Dispose();
            //    }
            //}
            //#endregion

            //#region Actualizar Nivel de certificación de Documento

            //ProyectoDS.NivelCertificacionDataTable nivelCertificacionEliminados = (ProyectoDS.NivelCertificacionDataTable)pProyectoDS.NivelCertificacion.GetChanges(DataRowState.Deleted);

            //if (nivelCertificacionEliminados != null)
            //{
            //    foreach (ProyectoDS.NivelCertificacionRow filaNivelCertificacion in nivelCertificacionEliminados.Rows)
            //    {

            //        string consulta = IBD.ReplaceParam("UPDATE DocumentoWebVinBaseRecursos SET NivelCertificacionID = NULL WHERE DocumentoWebVinBaseRecursos.NivelCertificacionID = " + IBD.GuidParamValor("NivelCertificacionID") + " ");

            //        DbCommand comandoNivelesCertificacion = ObtenerComando(consulta);
            //        AgregarParametro(comandoNivelesCertificacion, IBD.ToParam("NivelCertificacionID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid((Guid)filaNivelCertificacion["NivelCertificacionID", DataRowVersion.Original]));

            //        ActualizarBaseDeDatos(comandoNivelesCertificacion);
            //    }

            //    nivelCertificacionEliminados.Dispose();
            //}
            //#endregion

            //#region Tablas del Dataset ProyectoDS

            //DataSet deletedDataSet = pProyectoDS.GetChanges(DataRowState.Deleted);

            //if (deletedDataSet != null)
            //{
            //    #region Eliminar tabla ConfigAutocompletarProy
            //    DbCommand DeleteConfigAutocompletarProyCommand = ObtenerComando(sqlConfigAutocompletarProyDelete);
            //    AgregarParametro(DeleteConfigAutocompletarProyCommand, IBD.ToParam("Original_OrganizacionID"), DbType.Guid, "OrganizacionID", DataRowVersion.Original);
            //    AgregarParametro(DeleteConfigAutocompletarProyCommand, IBD.ToParam("Original_ProyectoID"), DbType.Guid, "ProyectoID", DataRowVersion.Original);
            //    AgregarParametro(DeleteConfigAutocompletarProyCommand, IBD.ToParam("Original_Clave"), DbType.String, "Clave", DataRowVersion.Original);

            //    ActualizarBaseDeDatos(deletedDataSet, "ConfigAutocompletarProy", null, null, DeleteConfigAutocompletarProyCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

            //    #endregion Eliminar tabla ConfigAutocompletarProy

            //    #region Eliminar tabla OntologiaProyecto
            //    DbCommand DeleteOntologiaProyectoCommand = ObtenerComando(sqlOntologiaProyectoDelete);
            //    AgregarParametro(DeleteOntologiaProyectoCommand, IBD.ToParam("Original_OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Original);
            //    AgregarParametro(DeleteOntologiaProyectoCommand, IBD.ToParam("Original_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);
            //    AgregarParametro(DeleteOntologiaProyectoCommand, IBD.ToParam("Original_OntologiaProyecto"), DbType.String, "OntologiaProyecto", DataRowVersion.Original);

            //    ActualizarBaseDeDatos(deletedDataSet, "OntologiaProyecto", null, null, DeleteOntologiaProyectoCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

            //    #endregion

            //    #region Eliminar tabla EcosistemaServicioExterno

            //    DbCommand DeleteEcosistemaServicioExternoCommand = ObtenerComando(sqlEcosistemaServicioExternoDelete);
            //    AgregarParametro(DeleteEcosistemaServicioExternoCommand, IBD.ToParam("O_NombreServicio"), IBD.TipoGuidToObject(DbType.String), "NombreServicio", DataRowVersion.Original);
            //    ActualizarBaseDeDatos(deletedDataSet, "EcosistemaServicioExterno", null, null, DeleteEcosistemaServicioExternoCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

            //    #endregion

            //    #region Eliminar tabla ProyectoServicioExterno

            //    DbCommand DeleteProyectoServicioExternoCommand = ObtenerComando(sqlProyectoServicioExternoDelete);
            //    AgregarParametro(DeleteProyectoServicioExternoCommand, IBD.ToParam("O_OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoServicioExternoCommand, IBD.ToParam("O_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoServicioExternoCommand, IBD.ToParam("O_NombreServicio"), IBD.TipoGuidToObject(DbType.String), "NombreServicio", DataRowVersion.Original);
            //    ActualizarBaseDeDatos(deletedDataSet, "ProyectoServicioExterno", null, null, DeleteProyectoServicioExternoCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

            //    #endregion

            //    #region Eliminar tabla PresentacionPestanyaMapaSemantico
            //    DbCommand DeletePresentacionPestanyaMapaSemanticoCommand = ObtenerComando(sqlPresentacionPestanyaMapaSemanticoDelete);
            //    AgregarParametro(DeletePresentacionPestanyaMapaSemanticoCommand, IBD.ToParam("Original_OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Original);
            //    AgregarParametro(DeletePresentacionPestanyaMapaSemanticoCommand, IBD.ToParam("Original_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);
            //    AgregarParametro(DeletePresentacionPestanyaMapaSemanticoCommand, IBD.ToParam("Original_PestanyaID"), IBD.TipoGuidToObject(DbType.Guid), "PestanyaID", DataRowVersion.Original);
            //    AgregarParametro(DeletePresentacionPestanyaMapaSemanticoCommand, IBD.ToParam("Original_OntologiaID"), IBD.TipoGuidToObject(DbType.Guid), "OntologiaID", DataRowVersion.Original);
            //    AgregarParametro(DeletePresentacionPestanyaMapaSemanticoCommand, IBD.ToParam("Original_Orden"), DbType.Int16, "Orden", DataRowVersion.Original);
            //    AgregarParametro(DeletePresentacionPestanyaMapaSemanticoCommand, IBD.ToParam("Original_Ontologia"), IBD.TipoGuidToObject(DbType.String), "Ontologia", DataRowVersion.Original);
            //    AgregarParametro(DeletePresentacionPestanyaMapaSemanticoCommand, IBD.ToParam("Original_Propiedad"), IBD.TipoGuidToObject(DbType.String), "Propiedad", DataRowVersion.Original);
            //    AgregarParametro(DeletePresentacionPestanyaMapaSemanticoCommand, IBD.ToParam("Original_Nombre"), IBD.TipoGuidToObject(DbType.String), "Nombre", DataRowVersion.Original);

            //    ActualizarBaseDeDatos(deletedDataSet, "PresentacionPestanyaMapaSemantico", null, null, DeletePresentacionPestanyaMapaSemanticoCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

            //    #endregion

            //    #region Eliminar tabla PresentacionPestanyaMosaicoSemantico
            //    DbCommand DeletePresentacionPestanyaMosaicoSemanticoCommand = ObtenerComando(sqlPresentacionPestanyaMosaicoSemanticoDelete);
            //    AgregarParametro(DeletePresentacionPestanyaMosaicoSemanticoCommand, IBD.ToParam("Original_OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Original);
            //    AgregarParametro(DeletePresentacionPestanyaMosaicoSemanticoCommand, IBD.ToParam("Original_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);
            //    AgregarParametro(DeletePresentacionPestanyaMosaicoSemanticoCommand, IBD.ToParam("Original_PestanyaID"), IBD.TipoGuidToObject(DbType.Guid), "PestanyaID", DataRowVersion.Original);
            //    AgregarParametro(DeletePresentacionPestanyaMosaicoSemanticoCommand, IBD.ToParam("Original_OntologiaID"), IBD.TipoGuidToObject(DbType.Guid), "OntologiaID", DataRowVersion.Original);
            //    AgregarParametro(DeletePresentacionPestanyaMosaicoSemanticoCommand, IBD.ToParam("Original_Orden"), DbType.Int16, "Orden", DataRowVersion.Original);
            //    AgregarParametro(DeletePresentacionPestanyaMosaicoSemanticoCommand, IBD.ToParam("Original_Ontologia"), IBD.TipoGuidToObject(DbType.String), "Ontologia", DataRowVersion.Original);
            //    AgregarParametro(DeletePresentacionPestanyaMosaicoSemanticoCommand, IBD.ToParam("Original_Propiedad"), IBD.TipoGuidToObject(DbType.String), "Propiedad", DataRowVersion.Original);
            //    AgregarParametro(DeletePresentacionPestanyaMosaicoSemanticoCommand, IBD.ToParam("Original_Nombre"), IBD.TipoGuidToObject(DbType.String), "Nombre", DataRowVersion.Original);

            //    ActualizarBaseDeDatos(deletedDataSet, "PresentacionPestanyaMosaicoSemantico", null, null, DeletePresentacionPestanyaMosaicoSemanticoCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

            //    #endregion

            //    #region Eliminar tabla PresentacionPestanyaListadoSemantico
            //    DbCommand DeletePresentacionPestanyaListadoSemanticoCommand = ObtenerComando(sqlPresentacionPestanyaListadoSemanticoDelete);
            //    AgregarParametro(DeletePresentacionPestanyaListadoSemanticoCommand, IBD.ToParam("Original_OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Original);
            //    AgregarParametro(DeletePresentacionPestanyaListadoSemanticoCommand, IBD.ToParam("Original_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);
            //    AgregarParametro(DeletePresentacionPestanyaListadoSemanticoCommand, IBD.ToParam("Original_PestanyaID"), IBD.TipoGuidToObject(DbType.Guid), "PestanyaID", DataRowVersion.Original);
            //    AgregarParametro(DeletePresentacionPestanyaListadoSemanticoCommand, IBD.ToParam("Original_OntologiaID"), IBD.TipoGuidToObject(DbType.Guid), "OntologiaID", DataRowVersion.Original);
            //    AgregarParametro(DeletePresentacionPestanyaListadoSemanticoCommand, IBD.ToParam("Original_Orden"), DbType.Int16, "Orden", DataRowVersion.Original);
            //    AgregarParametro(DeletePresentacionPestanyaListadoSemanticoCommand, IBD.ToParam("Original_Ontologia"), IBD.TipoGuidToObject(DbType.String), "Ontologia", DataRowVersion.Original);
            //    AgregarParametro(DeletePresentacionPestanyaListadoSemanticoCommand, IBD.ToParam("Original_Propiedad"), IBD.TipoGuidToObject(DbType.String), "Propiedad", DataRowVersion.Original);
            //    AgregarParametro(DeletePresentacionPestanyaListadoSemanticoCommand, IBD.ToParam("Original_Nombre"), IBD.TipoGuidToObject(DbType.String), "Nombre", DataRowVersion.Original);

            //    ActualizarBaseDeDatos(deletedDataSet, "PresentacionPestanyaListadoSemantico", null, null, DeletePresentacionPestanyaListadoSemanticoCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

            //    #endregion

            //    #region Eliminar tabla ProyectoPestanyaFiltroOrdenRecursos
            //    DbCommand DeleteProyectoPestanyaFiltroOrdenRecursosCommand = ObtenerComando(sqlProyectoPestanyaFiltroOrdenRecursosDelete);
            //    AgregarParametro(DeleteProyectoPestanyaFiltroOrdenRecursosCommand, IBD.ToParam("Original_PestanyaID"), IBD.TipoGuidToObject(DbType.Guid), "PestanyaID", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoPestanyaFiltroOrdenRecursosCommand, IBD.ToParam("Original_FiltroOrden"), DbType.String, "FiltroOrden", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoPestanyaFiltroOrdenRecursosCommand, IBD.ToParam("Original_NombreFiltro"), DbType.String, "NombreFiltro", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoPestanyaFiltroOrdenRecursosCommand, IBD.ToParam("Original_Orden"), DbType.Int16, "Orden", DataRowVersion.Original);
            //    ActualizarBaseDeDatos(deletedDataSet, "ProyectoPestanyaFiltroOrdenRecursos", null, null, DeleteProyectoPestanyaFiltroOrdenRecursosCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

            //    #endregion

            //    #region Eliminar tabla ProyectoSearchPersonalizado
            //    DbCommand DeleteProyectoSearchPersonalizadoCommand = ObtenerComando(sqlProyectoSearchPersonalizadoDelete);
            //    AgregarParametro(DeleteProyectoSearchPersonalizadoCommand, IBD.ToParam("Original_OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoSearchPersonalizadoCommand, IBD.ToParam("Original_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoSearchPersonalizadoCommand, IBD.ToParam("Original_NombreFaceta"), IBD.TipoGuidToObject(DbType.String), "NombreFaceta", DataRowVersion.Original);
            //    ActualizarBaseDeDatos(deletedDataSet, "ProyectoSearchPersonalizado", null, null, DeleteProyectoSearchPersonalizadoCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);
            //    #endregion

            //    #region Eliminar tabla ProyectoPestanyaMenuRolGrupoIdentidades
            //    DbCommand DeleteProyectoPestanyaMenuRolGrupoIdentidadesCommand = ObtenerComando(sqlProyectoPestanyaMenuRolGrupoIdentidadesDelete);
            //    AgregarParametro(DeleteProyectoPestanyaMenuRolGrupoIdentidadesCommand, IBD.ToParam("Original_PestanyaID"), IBD.TipoGuidToObject(DbType.Guid), "PestanyaID", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoPestanyaMenuRolGrupoIdentidadesCommand, IBD.ToParam("Original_GrupoID"), IBD.TipoGuidToObject(DbType.Guid), "GrupoID", DataRowVersion.Original);
            //    ActualizarBaseDeDatos(deletedDataSet, "ProyectoPestanyaMenuRolGrupoIdentidades", null, null, DeleteProyectoPestanyaMenuRolGrupoIdentidadesCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);
            //    #endregion

            //    #region Eliminar tabla ProyectoPestanyaMenuRolIdentidad
            //    DbCommand DeleteProyectoPestanyaMenuRolIdentidadCommand = ObtenerComando(sqlProyectoPestanyaMenuRolIdentidadDelete);
            //    AgregarParametro(DeleteProyectoPestanyaMenuRolIdentidadCommand, IBD.ToParam("Original_PestanyaID"), IBD.TipoGuidToObject(DbType.Guid), "PestanyaID", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoPestanyaMenuRolIdentidadCommand, IBD.ToParam("Original_PerfilID"), IBD.TipoGuidToObject(DbType.Guid), "PerfilID", DataRowVersion.Original);
            //    ActualizarBaseDeDatos(deletedDataSet, "ProyectoPestanyaMenuRolIdentidad", null, null, DeleteProyectoPestanyaMenuRolIdentidadCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);
            //    #endregion

            //    #region Eliminar tabla ProyectoPestanyaCMS
            //    DbCommand DeleteProyectoPestanyaCMSCommand = ObtenerComando(sqlProyectoPestanyaCMSDelete);
            //    AgregarParametro(DeleteProyectoPestanyaCMSCommand, IBD.ToParam("Original_PestanyaID"), IBD.TipoGuidToObject(DbType.Guid), "PestanyaID", DataRowVersion.Original);
            //    ActualizarBaseDeDatos(deletedDataSet, "ProyectoPestanyaCMS", null, null, DeleteProyectoPestanyaCMSCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);
            //    #endregion

            //    #region Eliminar tabla ProyectoPestanyaBusqueda
            //    DbCommand DeleteProyectoPestanyaBusquedaCommand = ObtenerComando(sqlProyectoPestanyaBusquedaDelete);
            //    AgregarParametro(DeleteProyectoPestanyaBusquedaCommand, IBD.ToParam("Original_PestanyaID"), IBD.TipoGuidToObject(DbType.Guid), "PestanyaID", DataRowVersion.Original);
            //    ActualizarBaseDeDatos(deletedDataSet, "ProyectoPestanyaBusqueda", null, null, DeleteProyectoPestanyaBusquedaCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);
            //    #endregion

            //    #region Eliminar tabla ProyectoPestanyaMenu
            //    DbCommand DeleteProyectoPestanyaMenuCommand = ObtenerComando(sqlProyectoPestanyaMenuDelete);
            //    AgregarParametro(DeleteProyectoPestanyaMenuCommand, IBD.ToParam("Original_PestanyaID"), IBD.TipoGuidToObject(DbType.Guid), "PestanyaID", DataRowVersion.Original);
            //    ActualizarBaseDeDatos(deletedDataSet, "ProyectoPestanyaMenu", null, null, DeleteProyectoPestanyaMenuCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);
            //    #endregion

            //    #region Eliminar tabla AccionesExternasProyecto
            //    DbCommand DeleteAccionesExternasProyectoCommand = ObtenerComando(sqlAccionesExternasProyectoDelete);
            //    AgregarParametro(DeleteAccionesExternasProyectoCommand, IBD.ToParam("O_TipoAccion"), DbType.Int16, "TipoAccion", DataRowVersion.Original);
            //    AgregarParametro(DeleteAccionesExternasProyectoCommand, IBD.ToParam("O_OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Original);
            //    AgregarParametro(DeleteAccionesExternasProyectoCommand, IBD.ToParam("O_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);
            //    AgregarParametro(DeleteAccionesExternasProyectoCommand, IBD.ToParam("O_URL"), DbType.String, "URL", DataRowVersion.Original);
            //    ActualizarBaseDeDatos(deletedDataSet, "AccionesExternasProyecto", null, null, DeleteAccionesExternasProyectoCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);
            //    #endregion

            //    #region Eliminar tabla ProyectoPasoRegistro

            //    DbCommand DeleteProyectoPasoRegistroCommand = ObtenerComando(sqlProyectoPasoRegistroDelete);
            //    AgregarParametro(DeleteProyectoPasoRegistroCommand, IBD.ToParam("Original_OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoPasoRegistroCommand, IBD.ToParam("Original_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoPasoRegistroCommand, IBD.ToParam("Original_Orden"), DbType.Int16, "Orden", DataRowVersion.Original);

            //    ActualizarBaseDeDatos(deletedDataSet, "ProyectoPasoRegistro", null, null, DeleteProyectoPasoRegistroCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

            //    #endregion

            //    #region Eliminar tabla ProyectoEventoParticipante

            //    DbCommand DeleteProyectoEventoParticipante = ObtenerComando(sqlProyectoEventoParticipanteDelete);
            //    AgregarParametro(DeleteProyectoEventoParticipante, IBD.ToParam("Original_IdentidadID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadID", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoEventoParticipante, IBD.ToParam("Original_EventoID"), IBD.TipoGuidToObject(DbType.Guid), "EventoID", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoEventoParticipante, IBD.ToParam("Original_Fecha"), IBD.TipoGuidToObject(DbType.DateTime), "Fecha", DataRowVersion.Original);

            //    ActualizarBaseDeDatos(deletedDataSet, "ProyectoEventoParticipante", null, null, DeleteProyectoEventoParticipante, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

            //    #endregion

            //    #region Eliminar tabla ProyectoEvento

            //    DbCommand DeleteProyectoEventoCommand = ObtenerComando(sqlProyectoEventoDelete);
            //    AgregarParametro(DeleteProyectoEventoCommand, IBD.ToParam("Original_EventoID"), IBD.TipoGuidToObject(DbType.Guid), "EventoID", DataRowVersion.Original);
            //    ActualizarBaseDeDatos(deletedDataSet, "ProyectoEvento", null, null, DeleteProyectoEventoCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

            //    #endregion

            //    #region Eliminar tabla ProyectoEventoAccion

            //    DbCommand DeleteProyectoEventoAccionCommand = ObtenerComando(sqlProyectoEventoAccionDelete);
            //    AgregarParametro(DeleteProyectoEventoAccionCommand, IBD.ToParam("Original_OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoEventoAccionCommand, IBD.ToParam("Original_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoEventoAccionCommand, IBD.ToParam("Original_Evento"), IBD.TipoGuidToObject(DbType.Int16), "Evento", DataRowVersion.Original);
            //    ActualizarBaseDeDatos(deletedDataSet, "ProyectoEventoAccion", null, null, DeleteProyectoEventoAccionCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

            //    #endregion

            //    #region Eliminar tabla PreferenciaProyecto

            //    DbCommand DeletePreferenciaProyectoCommand = ObtenerComando(sqlPreferenciaProyectoDelete);
            //    AgregarParametro(DeletePreferenciaProyectoCommand, IBD.ToParam("Original_OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Original);
            //    AgregarParametro(DeletePreferenciaProyectoCommand, IBD.ToParam("Original_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);
            //    AgregarParametro(DeletePreferenciaProyectoCommand, IBD.ToParam("Original_TesauroID"), IBD.TipoGuidToObject(DbType.Guid), "TesauroID", DataRowVersion.Original);
            //    AgregarParametro(DeletePreferenciaProyectoCommand, IBD.ToParam("Original_CategoriaTesauroID"), IBD.TipoGuidToObject(DbType.Guid), "CategoriaTesauroID", DataRowVersion.Original);
            //    AgregarParametro(DeletePreferenciaProyectoCommand, IBD.ToParam("Original_Orden"), IBD.TipoGuidToObject(DbType.Int16), "Orden", DataRowVersion.Original);
            //    ActualizarBaseDeDatos(deletedDataSet, "PreferenciaProyecto", null, null, DeletePreferenciaProyectoCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

            //    #endregion

            //    #region Eliminar tabla CamposRegistroProyectoGenericos

            //    DbCommand DeleteCamposRegistroProyectoGenericosCommand = ObtenerComando(sqlCamposRegistroProyectoGenericosDelete);
            //    AgregarParametro(DeleteCamposRegistroProyectoGenericosCommand, IBD.ToParam("Original_OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Original);
            //    AgregarParametro(DeleteCamposRegistroProyectoGenericosCommand, IBD.ToParam("Original_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);
            //    AgregarParametro(DeleteCamposRegistroProyectoGenericosCommand, IBD.ToParam("Original_Orden"), IBD.TipoGuidToObject(DbType.Int16), "Orden", DataRowVersion.Original);
            //    AgregarParametro(DeleteCamposRegistroProyectoGenericosCommand, IBD.ToParam("Original_Tipo"), IBD.TipoGuidToObject(DbType.Int16), "Tipo", DataRowVersion.Original);
            //    ActualizarBaseDeDatos(deletedDataSet, "CamposRegistroProyectoGenericos", null, null, DeleteCamposRegistroProyectoGenericosCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

            //    #endregion

            //    #region Eliminar tabla DatoExtraProyectoOpcion

            //    DbCommand DeleteDatoExtraProyectoOpcionCommand = ObtenerComando(sqlDatoExtraProyectoOpcionDelete);
            //    AgregarParametro(DeleteDatoExtraProyectoOpcionCommand, IBD.ToParam("Original_OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Original);
            //    AgregarParametro(DeleteDatoExtraProyectoOpcionCommand, IBD.ToParam("Original_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);
            //    AgregarParametro(DeleteDatoExtraProyectoOpcionCommand, IBD.ToParam("Original_DatoExtraID"), IBD.TipoGuidToObject(DbType.Guid), "DatoExtraID", DataRowVersion.Original);
            //    AgregarParametro(DeleteDatoExtraProyectoOpcionCommand, IBD.ToParam("Original_OpcionID"), IBD.TipoGuidToObject(DbType.Guid), "OpcionID", DataRowVersion.Original);
            //    AgregarParametro(DeleteDatoExtraProyectoOpcionCommand, IBD.ToParam("Original_Orden"), IBD.TipoGuidToObject(DbType.Int16), "Orden", DataRowVersion.Original);
            //    AgregarParametro(DeleteDatoExtraProyectoOpcionCommand, IBD.ToParam("Original_Opcion"), IBD.TipoGuidToObject(DbType.String), "Opcion", DataRowVersion.Original);
            //    ActualizarBaseDeDatos(deletedDataSet, "DatoExtraProyectoOpcion", null, null, DeleteDatoExtraProyectoOpcionCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

            //    #endregion

            //    #region Eliminar tabla DatoExtraProyecto
            //    DbCommand DeleteDatoExtraProyectoCommand = ObtenerComando(sqlDatoExtraProyectoDelete);
            //    AgregarParametro(DeleteDatoExtraProyectoCommand, IBD.ToParam("Original_OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Original);
            //    AgregarParametro(DeleteDatoExtraProyectoCommand, IBD.ToParam("Original_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);
            //    AgregarParametro(DeleteDatoExtraProyectoCommand, IBD.ToParam("Original_DatoExtraID"), IBD.TipoGuidToObject(DbType.Guid), "DatoExtraID", DataRowVersion.Original);
            //    AgregarParametro(DeleteDatoExtraProyectoCommand, IBD.ToParam("Original_Orden"), IBD.TipoGuidToObject(DbType.Int16), "Orden", DataRowVersion.Original);
            //    AgregarParametro(DeleteDatoExtraProyectoCommand, IBD.ToParam("Original_Titulo"), IBD.TipoGuidToObject(DbType.String), "Titulo", DataRowVersion.Original);
            //    AgregarParametro(DeleteDatoExtraProyectoCommand, IBD.ToParam("Original_PredicadoRDF"), IBD.TipoGuidToObject(DbType.String), "PredicadoRDF", DataRowVersion.Original);
            //    AgregarParametro(DeleteDatoExtraProyectoCommand, IBD.ToParam("Original_Obligatorio"), IBD.TipoGuidToObject(DbType.Boolean), "Obligatorio", DataRowVersion.Original);
            //    AgregarParametro(DeleteDatoExtraProyectoCommand, IBD.ToParam("Original_Paso1Registro"), IBD.TipoGuidToObject(DbType.Boolean), "Paso1Registro", DataRowVersion.Original);
            //    ActualizarBaseDeDatos(deletedDataSet, "DatoExtraProyecto", null, null, DeleteDatoExtraProyectoCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

            //    #endregion

            //    #region Eliminar tabla DatoExtraProyectoVirtuoso

            //    DbCommand DeleteDatoExtraProyectoVirtuosoCommand = ObtenerComando(sqlDatoExtraProyectoVirtuosoDelete);
            //    AgregarParametro(DeleteDatoExtraProyectoVirtuosoCommand, IBD.ToParam("Original_DatoExtraID"), IBD.TipoGuidToObject(DbType.Guid), "DatoExtraID", DataRowVersion.Original);
            //    ActualizarBaseDeDatos(deletedDataSet, "DatoExtraProyectoVirtuoso", null, null, DeleteDatoExtraProyectoVirtuosoCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

            //    #endregion

            //    #region Eliminar tabla DatoExtraEcosistemaOpcion

            //    DbCommand DeleteDatoExtraEcosistemaOpcionCommand = ObtenerComando(sqlDatoExtraEcosistemaOpcionDelete);
            //    AgregarParametro(DeleteDatoExtraEcosistemaOpcionCommand, IBD.ToParam("Original_DatoExtraID"), IBD.TipoGuidToObject(DbType.Guid), "DatoExtraID", DataRowVersion.Original);
            //    AgregarParametro(DeleteDatoExtraEcosistemaOpcionCommand, IBD.ToParam("Original_OpcionID"), IBD.TipoGuidToObject(DbType.Guid), "OpcionID", DataRowVersion.Original);
            //    AgregarParametro(DeleteDatoExtraEcosistemaOpcionCommand, IBD.ToParam("Original_Orden"), IBD.TipoGuidToObject(DbType.Int16), "Orden", DataRowVersion.Original);
            //    AgregarParametro(DeleteDatoExtraEcosistemaOpcionCommand, IBD.ToParam("Original_Opcion"), IBD.TipoGuidToObject(DbType.String), "Opcion", DataRowVersion.Original);
            //    ActualizarBaseDeDatos(deletedDataSet, "DatoExtraEcosistemaOpcion", null, null, DeleteDatoExtraEcosistemaOpcionCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

            //    #endregion

            //    #region Eliminar tabla DatoExtraEcosistema
            //    DbCommand DeleteDatoExtraEcosistemaCommand = ObtenerComando(sqlDatoExtraEcosistemaDelete);
            //    AgregarParametro(DeleteDatoExtraEcosistemaCommand, IBD.ToParam("Original_DatoExtraID"), IBD.TipoGuidToObject(DbType.Guid), "DatoExtraID", DataRowVersion.Original);
            //    AgregarParametro(DeleteDatoExtraEcosistemaCommand, IBD.ToParam("Original_Orden"), IBD.TipoGuidToObject(DbType.Int16), "Orden", DataRowVersion.Original);
            //    AgregarParametro(DeleteDatoExtraEcosistemaCommand, IBD.ToParam("Original_Titulo"), IBD.TipoGuidToObject(DbType.String), "Titulo", DataRowVersion.Original);
            //    AgregarParametro(DeleteDatoExtraEcosistemaCommand, IBD.ToParam("Original_PredicadoRDF"), IBD.TipoGuidToObject(DbType.String), "PredicadoRDF", DataRowVersion.Original);
            //    AgregarParametro(DeleteDatoExtraEcosistemaCommand, IBD.ToParam("Original_Obligatorio"), IBD.TipoGuidToObject(DbType.Boolean), "Obligatorio", DataRowVersion.Original);
            //    AgregarParametro(DeleteDatoExtraEcosistemaCommand, IBD.ToParam("Original_Paso1Registro"), IBD.TipoGuidToObject(DbType.Boolean), "Paso1Registro", DataRowVersion.Original);
            //    ActualizarBaseDeDatos(deletedDataSet, "DatoExtraEcosistema", null, null, DeleteDatoExtraEcosistemaCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

            //    #endregion

            //    #region Eliminar tabla DatoExtraEcosistemaVirtuoso

            //    DbCommand DeleteDatoExtraEcosistemaVirtuosoCommand = ObtenerComando(sqlDatoExtraEcosistemaVirtuosoDelete);
            //    AgregarParametro(DeleteDatoExtraEcosistemaVirtuosoCommand, IBD.ToParam("Original_DatoExtraID"), IBD.TipoGuidToObject(DbType.Guid), "DatoExtraID", DataRowVersion.Original);
            //    ActualizarBaseDeDatos(deletedDataSet, "DatoExtraEcosistemaVirtuoso", null, null, DeleteDatoExtraEcosistemaVirtuosoCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

            //    #endregion

            //    #region Eliminar tabla ProyectoLoginConfiguracion
            //    DbCommand DeleteProyectoLoginConfiguracionCommand = ObtenerComando(sqlProyectoLoginConfiguracionDelete);
            //    AgregarParametro(DeleteProyectoLoginConfiguracionCommand, IBD.ToParam("Original_OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoLoginConfiguracionCommand, IBD.ToParam("Original_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoLoginConfiguracionCommand, IBD.ToParam("Original_Mensaje"), IBD.TipoGuidToObject(DbType.String), "Mensaje", DataRowVersion.Original);
            //    ActualizarBaseDeDatos(deletedDataSet, "ProyectoLoginConfiguracion", null, null, DeleteProyectoLoginConfiguracionCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

            //    #endregion

            //    #region Eliminar tabla PresentacionMapaSemantico
            //    DbCommand DeletePresentacionMapaSemanticoCommand = ObtenerComando(sqlPresentacionMapaSemanticoDelete);
            //    AgregarParametro(DeletePresentacionMapaSemanticoCommand, IBD.ToParam("Original_OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Original);
            //    AgregarParametro(DeletePresentacionMapaSemanticoCommand, IBD.ToParam("Original_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);
            //    AgregarParametro(DeletePresentacionMapaSemanticoCommand, IBD.ToParam("Original_OntologiaID"), IBD.TipoGuidToObject(DbType.Guid), "OntologiaID", DataRowVersion.Original);
            //    AgregarParametro(DeletePresentacionMapaSemanticoCommand, IBD.ToParam("Original_Orden"), DbType.Int16, "Orden", DataRowVersion.Original);
            //    AgregarParametro(DeletePresentacionMapaSemanticoCommand, IBD.ToParam("Original_Ontologia"), IBD.TipoGuidToObject(DbType.String), "Ontologia", DataRowVersion.Original);
            //    AgregarParametro(DeletePresentacionMapaSemanticoCommand, IBD.ToParam("Original_Propiedad"), IBD.TipoGuidToObject(DbType.String), "Propiedad", DataRowVersion.Original);
            //    AgregarParametro(DeletePresentacionMapaSemanticoCommand, IBD.ToParam("Original_Nombre"), IBD.TipoGuidToObject(DbType.String), "Nombre", DataRowVersion.Original);

            //    ActualizarBaseDeDatos(deletedDataSet, "PresentacionMapaSemantico", null, null, DeletePresentacionMapaSemanticoCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

            //    #endregion

            //    #region Eliminar tabla PresentacionMosaicoSemantico
            //    DbCommand DeletePresentacionMosaicoSemanticoCommand = ObtenerComando(sqlPresentacionMosaicoSemanticoDelete);
            //    AgregarParametro(DeletePresentacionMosaicoSemanticoCommand, IBD.ToParam("Original_OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Original);
            //    AgregarParametro(DeletePresentacionMosaicoSemanticoCommand, IBD.ToParam("Original_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);
            //    AgregarParametro(DeletePresentacionMosaicoSemanticoCommand, IBD.ToParam("Original_OntologiaID"), IBD.TipoGuidToObject(DbType.Guid), "OntologiaID", DataRowVersion.Original);
            //    AgregarParametro(DeletePresentacionMosaicoSemanticoCommand, IBD.ToParam("Original_Orden"), DbType.Int16, "Orden", DataRowVersion.Original);
            //    AgregarParametro(DeletePresentacionMosaicoSemanticoCommand, IBD.ToParam("Original_Ontologia"), IBD.TipoGuidToObject(DbType.String), "Ontologia", DataRowVersion.Original);
            //    AgregarParametro(DeletePresentacionMosaicoSemanticoCommand, IBD.ToParam("Original_Propiedad"), IBD.TipoGuidToObject(DbType.String), "Propiedad", DataRowVersion.Original);
            //    AgregarParametro(DeletePresentacionMosaicoSemanticoCommand, IBD.ToParam("Original_Nombre"), IBD.TipoGuidToObject(DbType.String), "Nombre", DataRowVersion.Original);

            //    ActualizarBaseDeDatos(deletedDataSet, "PresentacionMosaicoSemantico", null, null, DeletePresentacionMosaicoSemanticoCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

            //    #endregion

            //    #region Eliminar tabla PresentacionListadoSemantico
            //    DbCommand DeletePresentacionListadoSemanticoCommand = ObtenerComando(sqlPresentacionListadoSemanticoDelete);
            //    AgregarParametro(DeletePresentacionListadoSemanticoCommand, IBD.ToParam("Original_OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Original);
            //    AgregarParametro(DeletePresentacionListadoSemanticoCommand, IBD.ToParam("Original_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);
            //    AgregarParametro(DeletePresentacionListadoSemanticoCommand, IBD.ToParam("Original_OntologiaID"), IBD.TipoGuidToObject(DbType.Guid), "OntologiaID", DataRowVersion.Original);
            //    AgregarParametro(DeletePresentacionListadoSemanticoCommand, IBD.ToParam("Original_Orden"), DbType.Int16, "Orden", DataRowVersion.Original);
            //    AgregarParametro(DeletePresentacionListadoSemanticoCommand, IBD.ToParam("Original_Ontologia"), IBD.TipoGuidToObject(DbType.String), "Ontologia", DataRowVersion.Original);
            //    AgregarParametro(DeletePresentacionListadoSemanticoCommand, IBD.ToParam("Original_Propiedad"), IBD.TipoGuidToObject(DbType.String), "Propiedad", DataRowVersion.Original);
            //    AgregarParametro(DeletePresentacionListadoSemanticoCommand, IBD.ToParam("Original_Nombre"), IBD.TipoGuidToObject(DbType.String), "Nombre", DataRowVersion.Original);

            //    ActualizarBaseDeDatos(deletedDataSet, "PresentacionListadoSemantico", null, null, DeletePresentacionListadoSemanticoCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

            //    #endregion

            //    #region Eliminar tabla SeccionProyCatalogo
            //    DbCommand DeleteSeccionProyCatalogoCommand = ObtenerComando(sqlSeccionProyCatalogoDelete);
            //    AgregarParametro(DeleteSeccionProyCatalogoCommand, IBD.ToParam("Original_OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Original);
            //    AgregarParametro(DeleteSeccionProyCatalogoCommand, IBD.ToParam("Original_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);
            //    AgregarParametro(DeleteSeccionProyCatalogoCommand, IBD.ToParam("Original_OrganizacionBusquedaID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionBusquedaID", DataRowVersion.Original);
            //    AgregarParametro(DeleteSeccionProyCatalogoCommand, IBD.ToParam("Original_ProyectoBusquedaID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoBusquedaID", DataRowVersion.Original);
            //    AgregarParametro(DeleteSeccionProyCatalogoCommand, IBD.ToParam("Original_Tipo"), DbType.Int16, "Tipo", DataRowVersion.Original);
            //    AgregarParametro(DeleteSeccionProyCatalogoCommand, IBD.ToParam("Original_Nombre"), DbType.String, "Nombre", DataRowVersion.Original);
            //    AgregarParametro(DeleteSeccionProyCatalogoCommand, IBD.ToParam("Original_Faceta"), DbType.String, "Faceta", DataRowVersion.Original);
            //    AgregarParametro(DeleteSeccionProyCatalogoCommand, IBD.ToParam("Original_Filtro"), DbType.String, "Filtro", DataRowVersion.Original);
            //    AgregarParametro(DeleteSeccionProyCatalogoCommand, IBD.ToParam("Original_NumeroResultados"), DbType.Int16, "NumeroResultados", DataRowVersion.Original);
            //    AgregarParametro(DeleteSeccionProyCatalogoCommand, IBD.ToParam("Original_Orden"), DbType.Int16, "Orden", DataRowVersion.Original);
            //    ActualizarBaseDeDatos(deletedDataSet, "SeccionProyCatalogo", null, null, DeleteSeccionProyCatalogoCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

            //    #endregion

            //    #region Eliminar tabla ProyectoPerfilNumElem
            //    DbCommand DeleteProyectoPerfilNumElemCommand = ObtenerComando(sqlProyectoPerfilNumElemDelete);
            //    AgregarParametro(DeleteProyectoPerfilNumElemCommand, IBD.ToParam("Original_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoPerfilNumElemCommand, IBD.ToParam("Original_PerfilID"), IBD.TipoGuidToObject(DbType.Guid), "PerfilID", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoPerfilNumElemCommand, IBD.ToParam("Original_NumRecursos"), DbType.Int32, "NumRecursos", DataRowVersion.Original);
            //    ActualizarBaseDeDatos(deletedDataSet, "ProyectoPerfilNumElem", null, null, DeleteProyectoPerfilNumElemCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

            //    #endregion

            //    #region Eliminar tabla ProyectoRelacionado
            //    DbCommand DeleteProyectoRelacionadoCommand = ObtenerComando(sqlProyectoRelacionadoDelete);
            //    AgregarParametro(DeleteProyectoRelacionadoCommand, IBD.ToParam("Original_OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoRelacionadoCommand, IBD.ToParam("Original_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoRelacionadoCommand, IBD.ToParam("Original_OrganizacionRelacionadaID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionRelacionadaID", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoRelacionadoCommand, IBD.ToParam("Original_ProyectoRelacionadoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoRelacionadoID", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoRelacionadoCommand, IBD.ToParam("O_Orden"), DbType.Int16, "Orden", DataRowVersion.Original);
            //    ActualizarBaseDeDatos(deletedDataSet, "ProyectoRelacionado", null, null, DeleteProyectoRelacionadoCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

            //    #endregion

            //    #region Eliminar tabla NivelCertificacion

            //    DbCommand DeleteNivelCertificacionCommand = ObtenerComando(sqlNivelCertificacionDelete);
            //    AgregarParametro(DeleteNivelCertificacionCommand, IBD.ToParam("O_NivelCertificacionID"), IBD.TipoGuidToObject(DbType.Guid), "NivelCertificacionID", DataRowVersion.Original);
            //    AgregarParametro(DeleteNivelCertificacionCommand, IBD.ToParam("O_OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Original);
            //    AgregarParametro(DeleteNivelCertificacionCommand, IBD.ToParam("O_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);
            //    AgregarParametro(DeleteNivelCertificacionCommand, IBD.ToParam("O_Orden"), DbType.Int16, "Orden", DataRowVersion.Original);
            //    AgregarParametro(DeleteNivelCertificacionCommand, IBD.ToParam("O_Descripcion"), DbType.String, "Descripcion", DataRowVersion.Original);
            //    ActualizarBaseDeDatos(deletedDataSet, "NivelCertificacion", null, null, DeleteNivelCertificacionCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

            //    #endregion

            //    #region Eliminar tabla ProyectosMasActivos

            //    DbCommand DeleteProyectosMasActivosCommand = ObtenerComando(sqlProyectosMasActivosDelete);
            //    AgregarParametro(DeleteProyectosMasActivosCommand, IBD.ToParam("O_OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectosMasActivosCommand, IBD.ToParam("O_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectosMasActivosCommand, IBD.ToParam("O_Nombre"), DbType.String, "Nombre", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectosMasActivosCommand, IBD.ToParam("O_Peso"), DbType.Int32, "Peso", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectosMasActivosCommand, IBD.ToParam("O_NumeroConsultas"), DbType.Int32, "NumeroConsultas", DataRowVersion.Original);
            //    ActualizarBaseDeDatos(deletedDataSet, "ProyectosMasActivos", null, null, DeleteProyectosMasActivosCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

            //    #endregion

            //    #region Eliminar tabla AdministradorProyecto

            //    DbCommand DeleteAdministradorProyectoCommand = ObtenerComando(sqlAdministradorProyectoDelete);
            //    AgregarParametro(DeleteAdministradorProyectoCommand, IBD.ToParam("O_OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Original);
            //    AgregarParametro(DeleteAdministradorProyectoCommand, IBD.ToParam("O_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);
            //    AgregarParametro(DeleteAdministradorProyectoCommand, IBD.ToParam("O_UsuarioID"), IBD.TipoGuidToObject(DbType.Guid), "UsuarioID", DataRowVersion.Original);
            //    AgregarParametro(DeleteAdministradorProyectoCommand, IBD.ToParam("O_Tipo"), DbType.Int16, "Tipo", DataRowVersion.Original);
            //    ActualizarBaseDeDatos(deletedDataSet, "AdministradorProyecto", null, null, DeleteAdministradorProyectoCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

            //    #endregion

            //    #region Eliminar tabla AdministradorGrupoProyecto

            //    DbCommand DeleteAdministradorGrupoProyectoCommand = ObtenerComando(sqlAdministradorGrupoProyectoDelete);
            //    AgregarParametro(DeleteAdministradorGrupoProyectoCommand, IBD.ToParam("O_OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Original);
            //    AgregarParametro(DeleteAdministradorGrupoProyectoCommand, IBD.ToParam("O_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);
            //    AgregarParametro(DeleteAdministradorGrupoProyectoCommand, IBD.ToParam("O_GrupoID"), IBD.TipoGuidToObject(DbType.Guid), "GrupoID", DataRowVersion.Original);
            //    ActualizarBaseDeDatos(deletedDataSet, "AdministradorGrupoProyecto", null, null, DeleteAdministradorGrupoProyectoCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

            //    #endregion

            //    #region Eliminar tabla ProyectoAgCatTesauro

            //    DbCommand DeleteProyectoAgCatTesauroCommand = ObtenerComando(sqlProyectoAgCatTesauroDelete);
            //    AgregarParametro(DeleteProyectoAgCatTesauroCommand, IBD.ToParam("O_OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoAgCatTesauroCommand, IBD.ToParam("O_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoAgCatTesauroCommand, IBD.ToParam("O_TesauroID"), IBD.TipoGuidToObject(DbType.Guid), "TesauroID", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoAgCatTesauroCommand, IBD.ToParam("O_CategoriaTesauroID"), IBD.TipoGuidToObject(DbType.Guid), "CategoriaTesauroID", DataRowVersion.Original);
            //    ActualizarBaseDeDatos(deletedDataSet, "ProyectoAgCatTesauro", null, null, DeleteProyectoAgCatTesauroCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

            //    #endregion

            //    #region Eliminar tabla TipoDocDispRolUsuarioProy

            //    DbCommand DeleteTipoDocDispRolUsuarioProyCommand = ObtenerComando(sqlTipoDocDispRolUsuarioProyDelete);
            //    AgregarParametro(DeleteTipoDocDispRolUsuarioProyCommand, IBD.ToParam("O_OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Original);
            //    AgregarParametro(DeleteTipoDocDispRolUsuarioProyCommand, IBD.ToParam("O_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);
            //    AgregarParametro(DeleteTipoDocDispRolUsuarioProyCommand, IBD.ToParam("O_TipoDocumento"), DbType.Int16, "TipoDocumento", DataRowVersion.Original);
            //    AgregarParametro(DeleteTipoDocDispRolUsuarioProyCommand, IBD.ToParam("O_RolUsuario"), DbType.Int16, "RolUsuario", DataRowVersion.Original);
            //    ActualizarBaseDeDatos(deletedDataSet, "TipoDocDispRolUsuarioProy", null, null, DeleteTipoDocDispRolUsuarioProyCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

            //    #endregion

            //    #region Eliminar tabla TipoOntoDispRolUsuarioProy

            //    DbCommand DeleteTipoOntoDispRolUsuarioProyCommand = ObtenerComando(sqlTipoOntoDispRolUsuarioProyDelete);
            //    AgregarParametro(DeleteTipoOntoDispRolUsuarioProyCommand, IBD.ToParam("O_OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Original);
            //    AgregarParametro(DeleteTipoOntoDispRolUsuarioProyCommand, IBD.ToParam("O_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);
            //    AgregarParametro(DeleteTipoOntoDispRolUsuarioProyCommand, IBD.ToParam("O_OntologiaID"), IBD.TipoGuidToObject(DbType.Guid), "OntologiaID", DataRowVersion.Original);
            //    AgregarParametro(DeleteTipoOntoDispRolUsuarioProyCommand, IBD.ToParam("O_RolUsuario"), DbType.Int16, "RolUsuario", DataRowVersion.Original);
            //    ActualizarBaseDeDatos(deletedDataSet, "TipoOntoDispRolUsuarioProy", null, null, DeleteTipoOntoDispRolUsuarioProyCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

            //    #endregion

            //    #region Eliminar tabla ProyectoCerradoTmp

            //    DbCommand DeleteProyectoCerradoTmpCommand = ObtenerComando(sqlProyectoCerradoTmpDelete);
            //    AgregarParametro(DeleteProyectoCerradoTmpCommand, IBD.ToParam("O_OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoCerradoTmpCommand, IBD.ToParam("O_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoCerradoTmpCommand, IBD.ToParam("O_Motivo"), DbType.String, "Motivo", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoCerradoTmpCommand, IBD.ToParam("O_FechaCierre"), DbType.DateTime, "FechaCierre", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoCerradoTmpCommand, IBD.ToParam("O_FechaReapertura"), DbType.DateTime, "FechaReapertura", DataRowVersion.Original);
            //    ActualizarBaseDeDatos(deletedDataSet, "ProyectoCerradoTmp", null, null, DeleteProyectoCerradoTmpCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

            //    #endregion

            //    #region Eliminar tabla ProyectoCerrandose

            //    DbCommand DeleteProyectoCerrandoseCommand = ObtenerComando(sqlProyectoCerrandoseDelete);
            //    AgregarParametro(DeleteProyectoCerrandoseCommand, IBD.ToParam("O_OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoCerrandoseCommand, IBD.ToParam("O_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoCerrandoseCommand, IBD.ToParam("O_FechaCierre"), DbType.DateTime, "FechaCierre", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoCerrandoseCommand, IBD.ToParam("O_PeriodoDeGracia"), DbType.Int32, "PeriodoDeGracia", DataRowVersion.Original);
            //    ActualizarBaseDeDatos(deletedDataSet, "ProyectoCerrandose", null, null, DeleteProyectoCerrandoseCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

            //    #endregion

            //    #region Eliminar tabla ProyectoGadgetContexto

            //    DbCommand DeleteProyectoGadgetContextoCommand = ObtenerComando(sqlProyectoGadgetContextoDelete);
            //    AgregarParametro(DeleteProyectoGadgetContextoCommand, IBD.ToParam("Original_OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoGadgetContextoCommand, IBD.ToParam("Original_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoGadgetContextoCommand, IBD.ToParam("Original_GadgetID"), IBD.TipoGuidToObject(DbType.Guid), "GadgetID", DataRowVersion.Original);

            //    ActualizarBaseDeDatos(deletedDataSet, "ProyectoGadgetContexto", null, null, DeleteProyectoGadgetContextoCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

            //    #endregion

            //    #region Eliminar tabla ProyectoGadgetContextoHTMLplano

            //    //DbCommand DeleteProyectoGadgetContextoHTMLplanoCommand = ObtenerComando(sqlProyectoGadgetContextoHTMLplanoDelete);
            //    //AgregarParametro(DeleteProyectoGadgetContextoHTMLplanoCommand, IBD.ToParam("Original_OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Original);
            //    //AgregarParametro(DeleteProyectoGadgetContextoHTMLplanoCommand, IBD.ToParam("Original_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);
            //    //AgregarParametro(DeleteProyectoGadgetContextoHTMLplanoCommand, IBD.ToParam("Original_GadgetID"), IBD.TipoGuidToObject(DbType.Guid), "GadgetID", DataRowVersion.Original);
            //    //AgregarParametro(DeleteProyectoGadgetContextoHTMLplanoCommand, IBD.ToParam("Original_ComunidadDestinoFiltros"), DbType.String, "ComunidadDestinoFiltros", DataRowVersion.Original);
            //    //ActualizarBaseDeDatos(deletedDataSet, "ProyectoGadgetContextoHTMLplano", null, null, DeleteProyectoGadgetContextoHTMLplanoCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

            //    #endregion

            //    #region Eliminar tabla ProyectoGadgetIdioma

            //    DbCommand DeleteProyectoGadgetIdiomaCommand = ObtenerComando(sqlProyectoGadgetIdiomaDelete);
            //    AgregarParametro(DeleteProyectoGadgetIdiomaCommand, IBD.ToParam("Original_OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoGadgetIdiomaCommand, IBD.ToParam("Original_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoGadgetIdiomaCommand, IBD.ToParam("Original_GadgetID"), IBD.TipoGuidToObject(DbType.Guid), "GadgetID", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoGadgetIdiomaCommand, IBD.ToParam("Original_Idioma"), DbType.String, "Idioma", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoGadgetIdiomaCommand, IBD.ToParam("Original_Contenido"), DbType.String, "Contenido", DataRowVersion.Original);
            //    ActualizarBaseDeDatos(deletedDataSet, "ProyectoGadgetIdioma", null, null, DeleteProyectoGadgetIdiomaCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

            //    #endregion

            //    #region Eliminar tabla ProyectoGadget

            //    DbCommand DeleteProyectoGadgetCommand = ObtenerComando(sqlProyectoGadgetDelete);
            //    AgregarParametro(DeleteProyectoGadgetCommand, IBD.ToParam("Original_OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoGadgetCommand, IBD.ToParam("Original_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoGadgetCommand, IBD.ToParam("Original_GadgetID"), IBD.TipoGuidToObject(DbType.Guid), "GadgetID", DataRowVersion.Original);

            //    ActualizarBaseDeDatos(deletedDataSet, "ProyectoGadget", null, null, DeleteProyectoGadgetCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

            //    #endregion             

            //    #region Eliminar tabla RecursosRelacionadosPresentacion

            //    DbCommand DeleteRecursosRelacionadosPresentacionCommand = ObtenerComando(sqlRecursosRelacionadosPresentacionDelete);

            //    AgregarParametro(DeleteRecursosRelacionadosPresentacionCommand, IBD.ToParam("Original_OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Original);
            //    AgregarParametro(DeleteRecursosRelacionadosPresentacionCommand, IBD.ToParam("Original_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);
            //    AgregarParametro(DeleteRecursosRelacionadosPresentacionCommand, IBD.ToParam("Original_OntologiaID"), DbType.Guid, "OntologiaID", DataRowVersion.Original);
            //    AgregarParametro(DeleteRecursosRelacionadosPresentacionCommand, IBD.ToParam("Original_Orden"), DbType.Int16, "Orden", DataRowVersion.Original);
            //    AgregarParametro(DeleteRecursosRelacionadosPresentacionCommand, IBD.ToParam("Original_Ontologia"), DbType.String, "Ontologia", DataRowVersion.Original);
            //    AgregarParametro(DeleteRecursosRelacionadosPresentacionCommand, IBD.ToParam("Original_Propiedad"), DbType.String, "Propiedad", DataRowVersion.Original);
            //    AgregarParametro(DeleteRecursosRelacionadosPresentacionCommand, IBD.ToParam("Original_Nombre"), DbType.String, "Nombre", DataRowVersion.Original);

            //    ActualizarBaseDeDatos(deletedDataSet, "RecursosRelacionadosPresentacion", null, null, DeleteRecursosRelacionadosPresentacionCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

            //    #endregion

            //    #region Eliminar tabla ProyectoPaginaHtml

            //    DbCommand DeleteProyectoPaginaHtmlCommand = ObtenerComando(sqlProyectoPaginaHtmlDelete);
            //    AgregarParametro(DeleteProyectoPaginaHtmlCommand, IBD.ToParam("Original_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoPaginaHtmlCommand, IBD.ToParam("Original_Nombre"), DbType.String, "Nombre", DataRowVersion.Original);

            //    ActualizarBaseDeDatos(deletedDataSet, "ProyectoPaginaHtml", null, null, DeleteProyectoPaginaHtmlCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

            //    #endregion

            //    #region Eliminar tabla Proyecto

            //    DbCommand DeleteProyectoCommand = ObtenerComando(sqlProyectoDelete);
            //    AgregarParametro(DeleteProyectoCommand, IBD.ToParam("O_OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoCommand, IBD.ToParam("O_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoCommand, IBD.ToParam("O_Nombre"), DbType.String, "Nombre", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoCommand, IBD.ToParam("O_Descripcion"), DbType.String, "Descripcion", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoCommand, IBD.ToParam("O_FechaInicio"), DbType.DateTime, "FechaInicio", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoCommand, IBD.ToParam("O_FechaFin"), DbType.DateTime, "FechaFin", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoCommand, IBD.ToParam("O_TipoProyecto"), DbType.Int16, "TipoProyecto", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoCommand, IBD.ToParam("O_TipoAcceso"), DbType.Int16, "TipoAcceso", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoCommand, IBD.ToParam("O_NumeroRecursos"), DbType.Int32, "NumeroRecursos", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoCommand, IBD.ToParam("O_NumeroPreguntas"), DbType.Int32, "NumeroPreguntas", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoCommand, IBD.ToParam("O_NumeroDebates"), DbType.Int32, "NumeroDebates", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoCommand, IBD.ToParam("O_NumeroMiembros"), DbType.Int32, "NumeroMiembros", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoCommand, IBD.ToParam("O_NumeroOrgRegistradas"), DbType.Int32, "NumeroOrgRegistradas", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoCommand, IBD.ToParam("O_NumeroArticulos"), DbType.Int32, "NumeroArticulos", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoCommand, IBD.ToParam("O_NumeroDafos"), DbType.Int32, "NumeroDafos", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoCommand, IBD.ToParam("O_NumeroForos"), DbType.Int32, "NumeroForos", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoCommand, IBD.ToParam("O_ProyectoSuperiorID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoSuperiorID", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoCommand, IBD.ToParam("O_EsProyectoDestacado"), DbType.Boolean, "EsProyectoDestacado", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoCommand, IBD.ToParam("O_Estado"), DbType.Int16, "Estado", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoCommand, IBD.ToParam("O_URLPropia"), DbType.String, "URLPropia", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoCommand, IBD.ToParam("O_NombreCorto"), DbType.String, "NombreCorto", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoCommand, IBD.ToParam("O_TieneTwitter"), DbType.Boolean, "TieneTwitter", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoCommand, IBD.ToParam("O_TagTwitter"), DbType.String, "TagTwitter", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoCommand, IBD.ToParam("O_UsuarioTwitter"), DbType.String, "UsuarioTwitter", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoCommand, IBD.ToParam("O_TokenTwitter"), DbType.String, "TokenTwitter", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoCommand, IBD.ToParam("O_TokenSecretoTwitter"), DbType.String, "TokenSecretoTwitter", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoCommand, IBD.ToParam("O_EnviarTwitterComentario"), DbType.Boolean, "EnviarTwitterComentario", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoCommand, IBD.ToParam("O_EnviarTwitterNuevaCat"), DbType.Boolean, "EnviarTwitterNuevaCat", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoCommand, IBD.ToParam("O_EnviarTwitterNuevoAdmin"), DbType.Boolean, "EnviarTwitterNuevoAdmin", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoCommand, IBD.ToParam("O_EnviarTwitterNuevaPolitCert"), DbType.Boolean, "EnviarTwitterNuevaPolitCert", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoCommand, IBD.ToParam("O_EnviarTwitterNuevoTipoDoc"), DbType.Boolean, "EnviarTwitterNuevoTipoDoc", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoCommand, IBD.ToParam("O_ProcesoVinculadoID"), IBD.TipoGuidToObject(DbType.Guid), "ProcesoVinculadoID", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoCommand, IBD.ToParam("O_Tags"), DbType.String, "Tags", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoCommand, IBD.ToParam("O_TagTwitterGnoss"), DbType.String, "TagTwitterGnoss", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoCommand, IBD.ToParam("O_NombrePresentacion"), DbType.String, "NombrePresentacion", DataRowVersion.Original);

            //    ActualizarBaseDeDatos(deletedDataSet, "Proyecto", null, null, DeleteProyectoCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

            //    #endregion

            //    #region Eliminar tabla ProyectoConfigExtraSem
            //    DbCommand DeleteProyectoConfigExtraSemCommand = ObtenerComando(sqlProyectoConfigExtraSemDelete);
            //    AgregarParametro(DeleteProyectoConfigExtraSemCommand, IBD.ToParam("Original_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoConfigExtraSemCommand, IBD.ToParam("Original_UrlOntologia"), DbType.String, "UrlOntologia", DataRowVersion.Original);
            //    AgregarParametro(DeleteProyectoConfigExtraSemCommand, IBD.ToParam("Original_SourceTesSem"), DbType.String, "SourceTesSem", DataRowVersion.Original);

            //    ActualizarBaseDeDatos(deletedDataSet, "ProyectoConfigExtraSem", null, null, DeleteProyectoConfigExtraSemCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

            //    #endregion
            //}

            //if (deletedDataSet != null)
            //{
            //    deletedDataSet.Dispose();
            //}
            //#endregion
        }

        /// <summary>
        /// Obtiene el identificador del metaproyecto
        /// </summary>
        /// <returns>Identificador del metaproyecto</returns>
        public Guid ObtenerMetaProyectoID()
        {
            //DbCommand sqlSelectMetaProyectoID = ObtenerComando("SELECT ProyectoID FROM Proyecto WHERE TipoProyecto = 2");

            object idMetaProyecto = mEntityContext.Proyecto.Where(proyecto => proyecto.TipoProyecto.Equals((short)TipoProyecto.MetaComunidad)).Select(proyecto => proyecto.ProyectoID).FirstOrDefault();

            if (idMetaProyecto is Guid)
            {
                return (Guid)idMetaProyecto;
            }
            return Guid.Empty;
        }

        /// <summary>
        /// Obtiene el identificador de la metaorganización
        /// </summary>
        /// <returns>Identificador de la metaorganización</returns>
        public Guid ObtenerMetaOrganizacionID()
        {
            //DbCommand sqlSelectMetaProyectoID = ObtenerComando("SELECT OrganizacionID FROM Proyecto WHERE TipoProyecto = 2");

            object idMetaOrganizacion = mEntityContext.Proyecto.Where(proyecto => proyecto.TipoProyecto.Equals((short)TipoProyecto.MetaComunidad)).Select(proyecto => proyecto.OrganizacionID).FirstOrDefault();

            if (idMetaOrganizacion is Guid)
            {
                return (Guid)idMetaOrganizacion;
            }
            return Guid.Empty;
        }

        /// <summary>
        /// Obtiene un listado con todas las urls propias de los proyectos
        /// </summary>
        /// <returns>Lista de strings</returns>
        public List<string> ObtenerUrlPropiasProyectos()
        {
            return mEntityContext.Proyecto.Where(proyecto => proyecto.URLPropia != null).Select(proyecto => proyecto.URLPropia).Distinct().ToList();
        }

        /// <summary>
        /// Obtiene un listado con todas las urls propias de los proyectos
        /// </summary>
        /// <returns>Lista de strings</returns>
        public List<string> ObtenerUrlPropiasProyectosPublicos()
        {
            List<string> listaUrlPropia = new List<string>();
            listaUrlPropia = mEntityContext.Proyecto.Where(proyecto => proyecto.URLPropia != null && (proyecto.TipoAcceso.Equals((short)TipoAcceso.Publico) || proyecto.TipoAcceso.Equals((short)TipoAcceso.Restringido))).Select(proyecto => proyecto.URLPropia).Distinct().ToList();

            return listaUrlPropia;
        }

        /// <summary>
        /// Obtiene los contadores de reucursos, personas y organizaciones de una comunidad.
        /// </summary>
        /// <param name="pProyectoID">ID de la comunidad</param>
        /// <returns>DataSet con Proyecto con los contadores de reucursos, personas y organizaciones de una comunidad</returns>
        public DataWrapperProyecto ObtenerContadoresProyecto(Guid pProyectoID)
        {
            DataWrapperProyecto proyectoDataWrapper = new DataWrapperProyecto();
            List<Proyecto> proyectos = mEntityContext.Proyecto.Where(proy => proy.ProyectoID.Equals(pProyectoID)).Select(proy => proy).ToList();
            //foreach( var proyect in proyectos.ToList())
            //{
            //    Proyecto proyecto = new Proyecto();
            //    proyecto.NumeroRecursos = proyect.NumeroRecursos;
            //    proyecto.NumeroMiembros = proyect.NumeroMiembros;
            //    proyecto.NumeroOrgRegistradas = proyect.NumeroOrgRegistradas;
            //    proyecto.NumeroPreguntas = proyect.NumeroPreguntas;
            //    proyecto.NumeroDebates = proyect.NumeroDebates;
            //    proyectoDataWrapper.ListaProyecto.Add(proyecto);
            //}
            proyectoDataWrapper.ListaProyecto = proyectos;
            return proyectoDataWrapper;
        }

        /// <summary>
        /// Obtiene los tags de varios proyectos
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns></returns>
        public Dictionary<Guid, string> ObtenerTagsDeProyectos(List<Guid> pListaProyectoID)
        {
            Dictionary<Guid, string> tags = new Dictionary<Guid, string>();

            foreach (Guid id in pListaProyectoID)
            {
                tags.Add(id, "");
            }

            if (pListaProyectoID.Count > 0)
            {
                var consulta = mEntityContext.Proyecto.Where(proyecto => pListaProyectoID.Contains(proyecto.ProyectoID)).Select(proyecto => new { proyecto.ProyectoID, proyecto.Tags }).ToList();

                foreach (var fila in consulta)
                {
                    if (!string.IsNullOrEmpty(fila.Tags))
                    {
                        Guid idDoc = fila.ProyectoID;
                        string tagss = fila.Tags;
                        tags[idDoc] = tagss;
                    }
                }

            }
            return tags;
        }
        /// <summary>
        /// Obtiene los datos de una carga a partir de su ID
        /// </summary>
        /// <param name="pCargaID">Identificador de la carga</param>
        /// <returns>Datos de la carga masiva</returns>
        public Carga ObtenerDatosCargaPorID(Guid pCargaID)
        {
            Carga carga = mEntityContext.Carga.Where(item => item.CargaID.Equals(pCargaID)).FirstOrDefault();
            return carga;
        }
        /// <summary>
        /// Obtener los datos del paquete de una carga masiva
        /// </summary>
        /// <param name="pPaqueteID"></param>
        /// <returns></returns>
        public CargaPaquete ObtenerDatosPaquete(Guid pPaqueteID)
        {
            CargaPaquete cargaPaquete = mEntityContext.CargaPaquete.Where(item => item.PaqueteID.Equals(pPaqueteID)).FirstOrDefault();
            return cargaPaquete;
        }
        /// <summary>
        /// Devuelve el proyectoID a partir de la base de recursos de un proyecto
        /// </summary>
        /// <param name="pBaseRecursosID"></param>
        /// <returns>Si encuentra el proyectoID sino Guid.empty</returns>
        public Guid ObtenerProyectoIDPorBaseRecursos(Guid pBaseRecursosID)
        {
            object resultado = mEntityContext.BaseRecursosProyecto.Where(baseRecursoProyecto => baseRecursoProyecto.BaseRecursosID.Equals(pBaseRecursosID)).Select(baseRecursoProyecto => baseRecursoProyecto.ProyectoID).FirstOrDefault();

            if (resultado != null)
            {
                return (Guid)resultado;
            }
            else
            {
                return Guid.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public void ObtenerDatosPorBaseRecursosPersona(Guid pBaseRecursosID, Guid pPersonaID, out Guid pProyectoID, out Guid pIdentidadID, out Guid pOrganizacionID)
        {
            var datos = mEntityContext.BaseRecursosProyecto.Join(mEntityContext.Identidad, baseRecursoProy => baseRecursoProy.ProyectoID, identidad => identidad.ProyectoID, (baseRecursoProy, identidad) => new
            {
                BaseRecursoProyecto = baseRecursoProy,
                Identidad = identidad
            }).Join(mEntityContext.Perfil, objeto => objeto.Identidad.PerfilID, perfil => perfil.PerfilID, (objeto, perfil) => new
            {
                BaseRecursoProyecto = objeto.BaseRecursoProyecto,
                Identidad = objeto.Identidad,
                Perfil = perfil
            }).Where(objeto => !objeto.Identidad.FechaBaja.HasValue && objeto.Perfil.PersonaID.HasValue && objeto.Perfil.PersonaID.Value.Equals(pPersonaID) && objeto.BaseRecursoProyecto.BaseRecursosID.Equals(pBaseRecursosID)).Select(objeto => new
            {
                ProyectoID = objeto.Identidad.ProyectoID,
                IdentidadID = objeto.Identidad.IdentidadID,
                OrganizacionID = objeto.Perfil.OrganizacionID
            }).ToList();


            /* "SELECT Identidad.ProyectoID, Identidad.IdentidadID, Perfil.OrganizacionID FROM BaseRecursosProyecto inner join Identidad on Identidad.ProyectoID = BaseRecursosProyecto.ProyectoID inner join Perfil on Identidad.PerfilID = Perfil.PerfilID WHERE Identidad.FechaBaja IS NULL AND Perfil.PersonaID = " + IBD.GuidValor(pPersonaID) + " AND BaseRecursosProyecto.BaseRecursosID = " + IBD.GuidValor(pBaseRecursosID);*/

            pProyectoID = Guid.Empty;
            pIdentidadID = Guid.Empty;
            pOrganizacionID = Guid.Empty;

            if (datos.Count > 0)
            {
                pProyectoID = datos.First().ProyectoID;
                pIdentidadID = datos.First().IdentidadID;

                if (datos.First().OrganizacionID.HasValue)
                {
                    pOrganizacionID = datos.First().OrganizacionID.Value;
                }
            }

        }

        /// <summary>
        /// Devuelve la Base de Recursos de un ProyectoID
        /// </summary>
        /// <param name="pProyectoID">Proyecto del que queremos la Base de recursos</param>
        /// <returns>Base de recursos ID</returns>
        public Guid ObtenerBaseRecursosProyectoPorProyectoID(Guid pProyectoID)
        {
            Guid proyectoID = Guid.Empty;

            List<Guid> listaProyectoID = mEntityContext.BaseRecursosProyecto.Where(baseRecurso => baseRecurso.ProyectoID.Equals(pProyectoID)).Select(baseRecurso => baseRecurso.BaseRecursosID).ToList();
            if (listaProyectoID.Count > 0)
            {
                proyectoID = listaProyectoID.First();
            }

            return proyectoID;
        }

        /// <summary>
        /// Obtiene los grafos gráficos configurados en un proyecto.
        /// </summary>
        /// <param name="pProyectoID">ID de proyecto</param>
        /// <returns>DataSet la tabla 'ProyectoGrafoFichaRec' con los grafos gráficos configurados en un proyecto</returns>
        public DataWrapperProyecto ObtenerGrafosProyecto(Guid pProyectoID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            dataWrapperProyecto.ListaProyectoGrafoFichaRec = mEntityContext.ProyectoGrafoFichaRec.Where(proyecto => proyecto.ProyectoID.Equals(pProyectoID)).ToList();

            return dataWrapperProyecto;
        }

        /// <summary>
        /// Obtiene los eventos de acciones disponibles en un proyecto
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto</param>
        /// <returns>Diccionario de eventos del Proyecto</returns>
        //public Dictionary<TipoProyectoEventoAccion, string> ObtenerEventosAccionProyectoPorProyectoID(Guid pProyectoID)
        //{

        //    Dictionary<TipoProyectoEventoAccion, string> listaEventos = new Dictionary<TipoProyectoEventoAccion, string>();
        //    ProyectoDS proyectoDS = new ProyectoDS();
        //    proyectoDS.EnforceConstraints = false;

        //    DbCommand commandsqlSelectEventosAccionProyectoPorProyectoID = ObtenerComando(sqlSelectEventosAccionProyectoPorProyectoID);
        //    AgregarParametro(commandsqlSelectEventosAccionProyectoPorProyectoID, IBD.ToParam("proyectoID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pProyectoID));
        //    CargarDataSet(commandsqlSelectEventosAccionProyectoPorProyectoID, proyectoDS, "ProyectoEventoAccion");
        //    if (proyectoDS.ProyectoEventoAccion.Rows.Count > 0)
        //    {
        //        foreach (ProyectoDS.ProyectoEventoAccionRow filaEvento in proyectoDS.ProyectoEventoAccion.Rows)
        //        {
        //            listaEventos.Add((TipoProyectoEventoAccion)filaEvento.Evento, filaEvento.AccionJS);
        //        }
        //    }

        //    proyectoDS.Dispose();
        //    return (listaEventos);
        //}

        /// <summary>
        /// Obtiene los nosmbres cortos de todas las comunidades de los tipos especificados
        /// </summary>
        /// <param name="pListaTipos">Lista de tipos de comunidades</param>
        /// <returns>Nombres cortos</returns>
        public List<string> ObtenerNombresCortosProyectosPorTipo(List<TipoProyecto> pListaTipos)
        {
            List<string> listaNombresCortos = new List<string>();

            listaNombresCortos = mEntityContext.Proyecto.Where(proyecto => pListaTipos.Contains((TipoProyecto)proyecto.TipoProyecto)).Select(proyecto => proyecto.NombreCorto).Distinct().ToList();

            return listaNombresCortos;
        }

        /// <summary>
        /// Obtiene un listado con los proyectos que tienen configuracion de newasletter por defecto (Guid.Empty especifica que es confgiracion del ecosistema)
        /// </summary>
        /// <returns></returns>
        public Dictionary<Guid, bool> ObtenerProyectosConConfiguracionNewsletterPorDefecto()
        {
            Dictionary<Guid, bool> NewsletterProyecto = new Dictionary<Guid, bool>();

            //Proyecto
            //DbCommand commandSQL = ObtenerComando("Select ProyectoID,Valor FROM parametroproyecto WHERE Parametro = '" + Es.Riam.Gnoss.AD.Parametro.ParametroAD.RecibirNewsletterDefecto + "'");
            var resultsProyecto = mEntityContext.ParametroProyecto.Where(parametroProyecto => parametroProyecto.Parametro.Equals(Parametro.ParametroAD.RecibirNewsletterDefecto)).Select(parametroProyecto => new { parametroProyecto.ProyectoID, parametroProyecto.Valor }).ToList();

            foreach (var resultProyecto in resultsProyecto)
            {
                Guid idproyecto = resultProyecto.ProyectoID;
                string valor = resultProyecto.Valor;
                NewsletterProyecto.Add(idproyecto, valor.Trim().ToLower() == "1" || valor.Trim().ToLower() == "true");
            }

            //Ecosistema
            //DbCommand commandSQLEcosistema = ObtenerComando("Select Valor FROM parametroaplicacion WHERE Parametro = '" + Es.Riam.Gnoss.AD.ParametroAplicacion.TiposParametrosAplicacion.RecibirNewsletterDefecto + "'");
            List<string> resultsEcosistema = mEntityContext.ParametroAplicacion.Where(parametroAplicacion => parametroAplicacion.Parametro.Equals(ParametroAplicacion.TiposParametrosAplicacion.RecibirNewsletterDefecto)).Select(parametro => parametro.Valor).ToList();

            foreach (string resultEcosistema in resultsEcosistema)
            {
                string valor = resultEcosistema;
                NewsletterProyecto.Add(Guid.Empty, valor.Trim().ToLower() == "1" || valor.Trim().ToLower() == "true");
            }

            return NewsletterProyecto;
        }


        #region Datos para Twitter

        /// <summary>
        /// Actualiza los tokens para Twitter del proyecto pasado por parámetro
        /// </summary>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <param name="pTokenTwitter">Token para Twitter</param>
        /// <param name="pTokenSecretoTwitter">Token secreto para Twitter</param>
        public void ActualizarTokenTwitterProyecto(Guid pProyectoID, string pTokenTwitter, string pTokenSecretoTwitter)
        {
            DbCommand commandsqlActualizarTokensTwitterProyecto = ObtenerComando(sqlUpdateTokenTwitterProyecto);

            AgregarParametro(commandsqlActualizarTokensTwitterProyecto, IBD.ToParam("ProyectoID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pProyectoID));
            AgregarParametro(commandsqlActualizarTokensTwitterProyecto, IBD.ToParam("TokenTwitter"), DbType.String, pTokenTwitter);
            AgregarParametro(commandsqlActualizarTokensTwitterProyecto, IBD.ToParam("TokenSecretoTwitter"), DbType.String, pTokenSecretoTwitter);

            int i = ActualizarBaseDeDatos(commandsqlActualizarTokensTwitterProyecto);
        }

        #endregion

        #region Documentación

        /// <summary>
        /// Actualiza el número de recursos, preguntas y debates de un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        public void ActulizarNumeroDocumentacion(Guid pProyectoID)
        {
            //Lo meto en un try/catch porque a veces falla en pruebas por la lentitud de este entorno, además si falla se recalculará al subir otro recurso o cada vez que sea pasado el servicio de optimización.
            try
            {
                var comun = mEntityContext.Documento.Join(mEntityContext.DocumentoWebVinBaseRecursos, documento => documento.DocumentoID, docWebVin => docWebVin.DocumentoID, (documento, docWebVin) => new
                {
                    Documento = documento,
                    DocumentoWebVinBaseRecursos = docWebVin
                }).Join(mEntityContext.BaseRecursosProyecto, objeto => objeto.DocumentoWebVinBaseRecursos.BaseRecursosID, baseRecursoProy => baseRecursoProy.BaseRecursosID, (objeto, baseRecursoProy) => new
                {
                    Documento = objeto.Documento,
                    DocumentoWebVinBaseRecursos = objeto.DocumentoWebVinBaseRecursos,
                    BaseRecursosProyecto = baseRecursoProy
                }).Where(objeto => objeto.BaseRecursosProyecto.ProyectoID.Equals(pProyectoID) && !objeto.Documento.Eliminado && !objeto.Documento.Borrador && objeto.Documento.UltimaVersion && !objeto.DocumentoWebVinBaseRecursos.Eliminado && objeto.Documento.TipoEntidad.Equals((short)TipoEntidadVinculadaDocumento.Web) && !objeto.DocumentoWebVinBaseRecursos.PrivadoEditores);

                int numRec = comun.Where(objeto => objeto.Documento.Tipo != (short)TiposDocumentacion.Debate && objeto.Documento.Tipo != (short)TiposDocumentacion.Pregunta && objeto.Documento.Tipo != (short)TiposDocumentacion.Ontologia).Select(objeto => objeto.Documento.DocumentoID).ToList().Count;

                int numPre = comun.Where(objeto => objeto.Documento.Tipo.Equals((short)TiposDocumentacion.Pregunta)).Select(objeto => objeto.Documento.DocumentoID).ToList().Count;

                int numDeb = comun.Where(objeto => objeto.Documento.Tipo.Equals((short)TiposDocumentacion.Debate)).Select(objeto => objeto.Documento.DocumentoID).ToList().Count;

                List<Proyecto> listaProyectos = mEntityContext.Proyecto.Where(proyecto => proyecto.ProyectoID.Equals(pProyectoID)).ToList();

                foreach (Proyecto proyecto in listaProyectos)
                {
                    proyecto.NumeroRecursos = numRec;
                    proyecto.NumeroPreguntas = numPre;
                    proyecto.NumeroDebates = numDeb;
                }


                #region ProyectoPerfilNumElem

                List<ProyectoPerfilNumElem> listaEliminar = mEntityContext.ProyectoPerfilNumElem.Where(proy => proy.ProyectoID.Equals(pProyectoID)).ToList();
                foreach (ProyectoPerfilNumElem proy in listaEliminar)
                {
                    mEntityContext.ProyectoPerfilNumElem.Remove(proy);
                }

                var elementoInsertar = mEntityContext.Documento.Join(mEntityContext.DocumentoWebVinBaseRecursos, documento => documento.DocumentoID, docWebBinBase => docWebBinBase.DocumentoID, (documento, docWebBinBase) => new
                {
                    Documento = documento,
                    DocumentoWebVinBaseRecursos = docWebBinBase
                }).Join(mEntityContext.BaseRecursosProyecto, objeto => objeto.DocumentoWebVinBaseRecursos.BaseRecursosID, baseRecurso => baseRecurso.BaseRecursosID, (objeto, baseRecurso) => new
                {
                    Documento = objeto.Documento,
                    DocumentoWebVinBaseRecursos = objeto.DocumentoWebVinBaseRecursos,
                    BaseRecursoProyecto = baseRecurso
                }).Join(mEntityContext.DocumentoRolIdentidad, objeto => objeto.Documento.DocumentoID, documentoRolIdenitdad => documentoRolIdenitdad.DocumentoID, (objeto, documentoRolIdentidad) => new
                {
                    Documento = objeto.Documento,
                    DocumentoWebVinBaseRecursos = objeto.DocumentoWebVinBaseRecursos,
                    BaseRecursoProyecto = objeto.BaseRecursoProyecto,
                    DocumentoRolIdentidad = documentoRolIdentidad
                }).Where(objeto => objeto.BaseRecursoProyecto.ProyectoID.Equals(pProyectoID) && !objeto.Documento.Eliminado && !objeto.Documento.Borrador && objeto.Documento.UltimaVersion && !objeto.DocumentoWebVinBaseRecursos.Eliminado && objeto.Documento.TipoEntidad.Equals((short)TipoEntidadVinculadaDocumento.Web) && !objeto.Documento.Tipo.Equals((short)TiposDocumentacion.Debate) && !objeto.Documento.Tipo.Equals((short)TiposDocumentacion.Pregunta) && !objeto.Documento.Tipo.Equals((short)TiposDocumentacion.Ontologia) && objeto.DocumentoWebVinBaseRecursos.PrivadoEditores).GroupBy(objeto => new { objeto.BaseRecursoProyecto.ProyectoID, objeto.DocumentoRolIdentidad.PerfilID }).Select(objeto => new { ProyectoID = objeto.Key.ProyectoID, PerfilID = objeto.Key.PerfilID, Documentos = objeto.Count()}).FirstOrDefault();
                ProyectoPerfilNumElem newProyectoPerfilNumElem = new ProyectoPerfilNumElem();
                if(elementoInsertar != null)
                {
                    newProyectoPerfilNumElem.PerfilID = elementoInsertar.PerfilID;
                    newProyectoPerfilNumElem.ProyectoID = elementoInsertar.ProyectoID;
                    newProyectoPerfilNumElem.NumRecursos = elementoInsertar.Documentos;

                    //ActualizarBaseDeDatos(comandoUpdateProyPerfilNumElem);
                    mEntityContext.ProyectoPerfilNumElem.Add(newProyectoPerfilNumElem);
                    mEntityContext.SaveChanges();
                }
                #endregion

            }
            catch (Exception e)
            {
                mLoggingService.GuardarLogError(e);
            }
        }

        /// <summary>
        /// Obtiene el tipo de proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns></returns>
        public TipoProyecto ObtenerTipoProyecto(Guid pProyectoID)
        {
            DbCommand commandsqlObtenerTipoProyecto = ObtenerComando("SELECT Proyecto.TipoProyecto FROM Proyecto WHERE Proyecto.ProyectoID = " + IBD.ToParam("ProyectoID"));

            short tipoProyecto = 0;

            object resultado = mEntityContext.Proyecto.Where(proyecto => proyecto.ProyectoID.Equals(pProyectoID)).Select(proyecto => proyecto.TipoProyecto).FirstOrDefault();

            if ((resultado != null) && (resultado is short))
            {
                tipoProyecto = (short)resultado;
            }
            return (TipoProyecto)tipoProyecto;
        }

        /// <summary>
        /// Obtiene los proyectos en los que participa una organizacion
        /// </summary>
        /// <param name="pOrganizacionID">Identificador del proyecto</param>
        /// <returns></returns>
        public DataWrapperProyecto CargarProyectosDeOrganizacionCargaLigeraParaFiltros(Guid pOrganizacionID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            List<ProyectoConAdministrado> proyectos = mEntityContext.Proyecto.Join(mEntityContext.OrganizacionParticipaProy, proyecto => proyecto.ProyectoID, orgParticipaProy => orgParticipaProy.ProyectoID, (proyecto, orgParticipaProy) => new
            {
                Proyecto = proyecto,
                OrganizacionParticipaProy = orgParticipaProy
            }).Join(mEntityContext.AdministradorProyecto, objeto => objeto.Proyecto.ProyectoID, adminProy => adminProy.ProyectoID, (objeto, adminProy) => new
            {
                Proyecto = objeto.Proyecto,
                OrganizacionParticipaProy = objeto.OrganizacionParticipaProy,
                AdministradorProyecto = adminProy
            }).Join(mEntityContext.Persona, objeto => objeto.AdministradorProyecto.UsuarioID, persona => persona.UsuarioID, (objeto, persona) => new
            {
                Proyecto = objeto.Proyecto,
                OrganizacionParticipaProy = objeto.OrganizacionParticipaProy,
                AdministradorProyecto = objeto.AdministradorProyecto,
                Persona = persona
            }).GroupJoin(mEntityContext.Perfil, objeto => objeto.Persona.PersonaID, perfil => perfil.PerfilID, (objeto, perfil) => new
            {
                Proyecto = objeto.Proyecto,
                OrganizacionParticipaProy = objeto.OrganizacionParticipaProy,
                AdministradorProyecto = objeto.AdministradorProyecto,
                Persona = objeto.Persona,
                Perfil = perfil
            }).SelectMany(x => x.Perfil.DefaultIfEmpty(), (x, y) => new
            {
                Proyecto = x.Proyecto,
                OrganizacionParticipaProy = x.OrganizacionParticipaProy,
                AdministradorProyecto = x.AdministradorProyecto,
                Persona = x.Persona,
                Perfil = y
            }).Where(objeto => objeto.OrganizacionParticipaProy.OrganizacionID.Equals(pOrganizacionID) && objeto.Proyecto.Estado != 0 && objeto.Proyecto.Estado != 1 && objeto.Proyecto.ProyectoID != ProyectoAD.MetaProyecto).ToList()
            .GroupBy(objeto => new { objeto.Proyecto.OrganizacionID, objeto.Proyecto.ProyectoID, objeto.Proyecto.Nombre }, objeto => objeto.Perfil?.OrganizacionID, (objetoAgrup, g) => new
            {
                OrganizacionID = objetoAgrup.OrganizacionID,
                ProyectoID = objetoAgrup.ProyectoID,
                Nombre = objetoAgrup.Nombre,
                Administrado = g.ToList().Count > 0 ? 0 : 1
            }).ToList().Select(objeto => new ProyectoConAdministrado { OrganizacionID = objeto.OrganizacionID, ProyectoID = objeto.ProyectoID, Nombre = objeto.Nombre, Administrado = objeto.Administrado }).Distinct().ToList();
            dataWrapperProyecto.ListaProyectoConAdministrado = proyectos;
            return (dataWrapperProyecto);
        }



        public List<UsuarioAdministradorComunidad> CargarProyectosParticipaPersonaOrg(Guid pOrganizacionID, Guid pPersonaID)
        {

            var administradoresProyectoSQL = mEntityContext.Proyecto.JoinOrganizacionParticipaProy().JoinPerfil().LeftJoinIdentidad().LeftJoinPerfil().LeftJoinPersona().LeftJoinAdministradorProyecto().Where(x => !x.Identidad.FechaBaja.HasValue && !x.Identidad.FechaExpulsion.HasValue && x.OrganizacionParticipaProy.OrganizacionID.Equals(pOrganizacionID) && x.Perfil.PersonaID.HasValue && x.Perfil.PersonaID.Value.Equals(pPersonaID) && x.Proyecto.Estado != 0 && x.Proyecto.Estado != 1 && !x.Proyecto.ProyectoID.Equals(ProyectoAD.MetaProyecto));

            var administradoresProyectoSQL2 = administradoresProyectoSQL.GroupBy(x => new { OrganizacionID = x.Proyecto.OrganizacionID, ProyectoID = x.Proyecto.ProyectoID, Nombre = x.Proyecto.Nombre, Tipo = x.Identidad.Tipo });

            var administradoresProyectoSQL3 = administradoresProyectoSQL2.Select(x => new
            {
                OrganizacionID = x.Key.OrganizacionID,
                ProyectoID = x.Key.ProyectoID,
                Nombre = x.Key.Nombre,
                Tipo = ((short?)x.Key.Tipo).HasValue ? (short?)x.Key.Tipo : null,
                Administrador = (x.Count(y => ((Guid?)y.AdministradorProyecto.UsuarioID).HasValue)) > 0 ? 1 : 0
            }).Distinct();


            List<UsuarioAdministradorComunidad> administradoresProyecto = administradoresProyectoSQL3.ToList().Select(x => new UsuarioAdministradorComunidad
            {
                OrganizacionID = x.OrganizacionID,
                ProyectoID = x.ProyectoID,
                Nombre = x.Nombre,
                Tipo = x.Tipo,
                Administrador = x.Administrador
            }).OrderBy(item => item.Nombre).ToList();


            return administradoresProyecto;
        }


        /// <summary>
        /// Obtiene el estado de un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns></returns>
        public EstadoProyecto ObtenerEstadoProyecto(Guid pProyectoID)
        {

            short estado = 0;

            object resultado = mEntityContext.Proyecto.Where(proyecto => proyecto.ProyectoID.Equals(pProyectoID)).Select(proyecto => proyecto.Estado).FirstOrDefault();

            if ((resultado != null) && (resultado is short))
            {
                estado = (short)resultado;
            }
            return (EstadoProyecto)estado;
        }

        /// <summary>
        /// Obtiene el tipo de acceso a un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns></returns>
        public TipoAcceso ObtenerTipoAccesoProyecto(Guid pProyectoID)
        {

            short tipoAcceso = 0;

            object resultado = mEntityContext.Proyecto.Where(proyecto => proyecto.ProyectoID.Equals(pProyectoID)).Select(proyecto => proyecto.TipoAcceso).FirstOrDefault();

            if ((resultado != null) && (resultado is short))
            {
                tipoAcceso = (short)resultado;
            }
            return (TipoAcceso)tipoAcceso;
        }

        /// <summary>
        /// Obtiene los tipo de documentos permitidos para un rol de usuario en un determinado proyecto.
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto en el que se hace la comprobación</param>
        /// <param name="pTipoRol">Tipo de rol del usuario actual</param>
        /// <returns>Lista con los tipos de documentos permitidos para el rol</returns>
        public List<TiposDocumentacion> ObtenerTiposDocumentosPermitidosUsuarioEnProyecto(Guid pProyectoID, TipoRolUsuario pTipoRol)
        {
            List<TiposDocumentacion> listaTiposDoc = new List<TiposDocumentacion>();

            List<TipoDocDispRolUsuarioProy> listaTipoDocRolUsuario = mEntityContext.TipoDocDispRolUsuarioProy.Where(tipoDocRolUs => tipoDocRolUs.ProyectoID.Equals(pProyectoID) && tipoDocRolUs.RolUsuario >= (short)pTipoRol).ToList();

            foreach (TipoDocDispRolUsuarioProy filaTipoDosRolUsu in listaTipoDocRolUsuario)
            {
                listaTiposDoc.Add((TiposDocumentacion)filaTipoDosRolUsu.TipoDocumento);
            }

            return listaTiposDoc;
        }

        /// <summary>
        /// Obtiene el rol de usuario en un determinado proyecto.
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto en el que se hace la comprobación</param>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <returns>El rol de usuario en un determinado proyecto</returns>
        public TipoRolUsuario ObtenerRolUsuarioEnProyecto(Guid pProyectoID, Guid pUsuarioID)
        {

            TipoRolUsuario tipoRolUsuario = TipoRolUsuario.Usuario;

            var adminProyecto = mEntityContext.AdministradorProyecto.Where(adminProy => adminProy.ProyectoID.Equals(pProyectoID) && adminProy.UsuarioID.Equals(pUsuarioID)).Select(adminProy => new
            {
                OrganizacionID = adminProy.OrganizacionID,
                ProyectoID = adminProy.ProyectoID,
                UsuarioID = adminProy.UsuarioID,
                Tipo = adminProy.Tipo
            });


            if (adminProyecto.ToList().Count == 0)
            {
                tipoRolUsuario = TipoRolUsuario.Usuario;
            }
            else
            {
                tipoRolUsuario = (TipoRolUsuario)adminProyecto.First().Tipo;
            }

            return tipoRolUsuario;
        }

        /// <summary>
        /// Obtiene los tipo de documentos permitidos para un rol de usuario en un determinado proyecto.
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto en el que se hace la comprobación</param>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <returns>Lista con los tipos de documentos permitidos para el rol</returns>
        public List<TiposDocumentacion> ObtenerTiposDocumentosPermitidosUsuarioEnProyectoPorSuID(Guid pProyectoID, Guid pUsuarioID)
        {
            List<TiposDocumentacion> listaTiposDoc = new List<TiposDocumentacion>();
            TipoRolUsuario tipoRolUsuario = TipoRolUsuario.Usuario;

            var adminProyecto = mEntityContext.AdministradorProyecto.Where(adminProy => adminProy.ProyectoID.Equals(pProyectoID) && adminProy.UsuarioID.Equals(pUsuarioID)).Select(adminProy => new
            {
                OrganizacionID = adminProy.OrganizacionID,
                ProyectoID = adminProy.ProyectoID,
                UsuarioID = adminProy.UsuarioID,
                Tipo = adminProy.Tipo
            });

            if (adminProyecto.ToList().Count == 0)
            {
                tipoRolUsuario = TipoRolUsuario.Usuario;
            }
            else
            {
                tipoRolUsuario = (TipoRolUsuario)adminProyecto.First().Tipo;
            }


            return ObtenerTiposDocumentosPermitidosUsuarioEnProyecto(pProyectoID, tipoRolUsuario);
        }

        /// <summary>
        /// Obtiene los filtros de ordenes disponibles para un proyecto.
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <returns>DataSet de ProyectoDS con la tabla 'ProyectoPestanyaFiltroOrdenRecursos' cargada para un proyecto</returns>
        public DataWrapperProyecto ObtenerFiltrosOrdenesDeProyecto(Guid pProyectoID)
        {
            DataWrapperProyecto dataWrapperProyDS = new DataWrapperProyecto();

            List<ProyectoPestanyaFiltroOrdenRecursos> listaProyectoPestanyaFiltroOrdenRecursos = mEntityContext.ProyectoPestanyaFiltroOrdenRecursos.Join(mEntityContext.ProyectoPestanyaMenu, proyectoPestanyaFiltro => proyectoPestanyaFiltro.PestanyaID, proyectoPestanyaMenu => proyectoPestanyaMenu.PestanyaID, (proyectoPestanyaFiltro, proyectoPestanyaMenu) => new
            {
                ProyectoPestanyaFiltro = proyectoPestanyaFiltro,
                ProyectoID = proyectoPestanyaMenu.ProyectoID
            }).Where(busqueda => busqueda.ProyectoID.Equals(pProyectoID)).Select(busqueda => busqueda.ProyectoPestanyaFiltro).ToList();

            dataWrapperProyDS.ListaProyectoPestanyaFiltroOrdenRecursos = listaProyectoPestanyaFiltroOrdenRecursos;
            return dataWrapperProyDS;
        }

        /// <summary>
        /// Obtiene los tesauros semánticos configurados para edición.
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <returns>DataSet de ProyectoDS con la tabla 'ProyectoConfigExtraSem' cargada para un proyecto</returns>
        public DataWrapperProyecto ObtenerTesaurosSemanticosConfigEdicionDeProyecto(Guid pProyectoID)
        {
            DataWrapperProyecto DataWrapperProyDS = new DataWrapperProyecto();
            List<ProyectoConfigExtraSem> listaProyectoConfigExtraSem = mEntityContext.ProyectoConfigExtraSem.Where(proyectoConfigExtra => proyectoConfigExtra.ProyectoID.Equals(pProyectoID) && proyectoConfigExtra.Tipo.Equals((short)TipoConfigExtraSemantica.TesauroSemantico)).ToList();
            DataWrapperProyDS.ListaProyectoConfigExtraSem = listaProyectoConfigExtraSem;
            return DataWrapperProyDS;
        }

        /// <summary>
        /// Obtiene los tesauros semánticos configurados para edición.
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <returns>DataSet de ProyectoDS con la tabla 'ProyectoConfigExtraSem' cargada para un proyecto</returns>
        public DataWrapperProyecto ObtenerConfiguracionSemanticaExtraDeProyecto(Guid pProyectoID)
        {
            DataWrapperProyecto dataWrapperProyDS = new DataWrapperProyecto();
            //this.sqlSelectProyectoConfigExtraSem = "SELECT " + IBD.CargarGuid("ProyectoConfigExtraSem.ProyectoID") + ", ProyectoConfigExtraSem.UrlOntologia, ProyectoConfigExtraSem.SourceTesSem, ProyectoConfigExtraSem.Tipo, ProyectoConfigExtraSem.Nombre, ProyectoConfigExtraSem.Idiomas, ProyectoConfigExtraSem.PrefijoTesSem, ProyectoConfigExtraSem.Editable FROM ProyectoConfigExtraSem";
            List<ProyectoConfigExtraSem> listProyectoConfigExtraSem = mEntityContext.ProyectoConfigExtraSem.Where(proyectoConfigExtr => proyectoConfigExtr.ProyectoID.Equals(pProyectoID)).ToList();
            dataWrapperProyDS.ListaProyectoConfigExtraSem = listProyectoConfigExtraSem;
            return dataWrapperProyDS;
        }

        /// <summary>
        /// Indica si el recurso es de un Tipo de recursos que se encuentra en la lista de recursos que no se publican en la actividad reciente
        /// </summary>
        /// <param name="pRecursoID">ID del recurso</param>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <returns></returns>
        public bool ComprobarSiRecursoSePublicaEnActividadReciente(Guid pRecursoID, Guid pProyectoID)
        {
            //TO-DO Juan: Comprobar el proyecto y mejorar la funcionalidad. Esto solo funciona

            Guid? ontologiaID = mEntityContext.Documento.Where(Documento => Documento.DocumentoID == pRecursoID && Documento.ElementoVinculadoID.HasValue).Select(Documento => Documento.ElementoVinculadoID).FirstOrDefault();

            if (ontologiaID.HasValue)
            {
                string ontoID = ontologiaID.Value.ToString();

                //Si encuentra alguna fila que cumpla las condiciones, devuelve false
                return !mEntityContext.ProyTipoRecNoActivReciente.Where(proyTipoNoRec => proyTipoNoRec.TipoRecurso == 5 && proyTipoNoRec.OntologiasID.Contains(ontoID)).Any();
            }

            return true;
        }

        /// <summary>
        /// Obtiene los tipos de recursos que no deben ir a la actividad reciente de la comunidad.
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <returns>DataSet con la tabla ProyTipoRecNoActivReciente cargada para el proyecto</returns>
        public DataWrapperProyecto ObtenerTiposRecursosNoActividadReciente(Guid pProyectoID)
        {
            DataWrapperProyecto dataWrapperProyectoDS = new DataWrapperProyecto();
            //this.sqlSelectProyTipoRecNoActivReciente = "SELECT " + IBD.CargarGuid("ProyectoID") + ", TipoRecurso, OntologiasID FROM ProyTipoRecNoActivReciente";

            List<ProyTipoRecNoActivReciente> listaProyTipoRecNoActivReciente = mEntityContext.ProyTipoRecNoActivReciente.Where(proyTipoNoRec => proyTipoNoRec.ProyectoID.Equals(pProyectoID)).ToList();
            dataWrapperProyectoDS.ListaProyTipoRecNoActivReciente = listaProyTipoRecNoActivReciente;
            return dataWrapperProyectoDS;
        }

        /// <summary>
        /// Devuelve las imágenes por defecto según el tipo de imagen por defecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Imagen por defecto según el tipo de imagen por defecto</returns>
        public Dictionary<short, Dictionary<Guid, string>> ObtenerTipoDocImagenPorDefecto(Guid pProyectoID)
        {
            //this.sqlSelectTipoDocImagenPorDefecto = "SELECT " + IBD.CargarGuid("ProyectoID") + ", TipoRecurso, " + IBD.CargarGuid("OntologiaID") + ", UrlImagen FROM TipoDocImagenPorDefecto";
            DataSet dataSet = new DataSet();

            DbCommand comandoSel = ObtenerComando(sqlSelectTipoDocImagenPorDefecto + " WHERE ProyectoID=" + IBD.GuidValor(pProyectoID));
            CargarDataSet(comandoSel, dataSet, "TipoDocImagenPorDefecto");
            var seleccion = mEntityContext.TipoDocImagenPorDefecto.Where(tipoDocImagen => tipoDocImagen.ProyectoID.Equals(pProyectoID)).Select(tipoDocImagen => new { tipoDocImagen.ProyectoID, tipoDocImagen.TipoRecurso, tipoDocImagen.OntologiaID, tipoDocImagen.UrlImagen }).ToList();
            Dictionary<short, Dictionary<Guid, string>> listaTipo = new Dictionary<short, Dictionary<Guid, string>>();

            foreach (var fila in seleccion)
            {
                if (!listaTipo.ContainsKey((short)fila.TipoRecurso))
                {
                    listaTipo.Add((short)fila.TipoRecurso, new Dictionary<Guid, string>());
                }

                listaTipo[(short)fila.TipoRecurso].Add((Guid)fila.OntologiaID, (string)fila.UrlImagen);
            }

            dataSet.Dispose();
            return listaTipo;
        }

        #endregion

        #region Administración proyecto

        /// <summary>
        /// Devuelve una lista con las claves de los proyectos que administra el grupo
        /// </summary>
        /// <param name="pUsuarioID">Clave del grupo</param>
        /// <returns>Lista de Claves de Proyectos</returns>
        public List<Guid> ObtenerListaProyectosAdministradosPorGrupo(Guid pGrupoID)
        {
            List<Guid> listaGuidProyectosAdministra = mEntityContext.AdministradorGrupoProyecto.Where(adminGrupoProy => adminGrupoProy.GrupoID.Equals(pGrupoID)).Select(adminGrupoProy => adminGrupoProy.ProyectoID).ToList();
            return listaGuidProyectosAdministra;
        }

        /// <summary>
        /// Obtiene una lista con los administradores de un proyecto 
        /// (Sólo los administradores --> Tipo = 0 )
        /// </summary>
        /// <param name="pProyectoID">Proyecto del que queremos obtener sus administradores</param>
        /// <returns>Lista con las identidades de los administradores de un proyecto</returns>
        /// 
            //TODO Realizar con IdentidadDS
        public List<Guid> ObtenerListaIdentidadesAdministradoresPorProyecto(Guid pProyectoID)
        {
            List<Guid> listaAdministradores = new List<Guid>();
            //string AdministradoresPorProyecto = "SELECT " + IBD.CargarGuid("Identidad.IdentidadID") + " FROM Identidad INNER JOIN ProyectoUsuarioIdentidad ON (ProyectoUsuarioIdentidad.IdentidadID=Identidad.IdentidadID) INNER JOIN AdministradorProyecto ON (AdministradorProyecto.UsuarioID = ProyectoUsuarioIdentidad.UsuarioID) AND (AdministradorProyecto.ProyectoID = Identidad.ProyectoID) WHERE Identidad.ProyectoID= " + IBD.GuidParamValor("proyectoID") + " AND AdministradorProyecto.Tipo=" + IBD.ToParam("tipoAdmin") + " AND Identidad.FechaBaja IS NULL AND Identidad.FechaExpulsion IS NULL";

            listaAdministradores = mEntityContext.Identidad.Join(mEntityContext.ProyectoUsuarioIdentidad, identidad => identidad.IdentidadID, proyectoUsuarioIdentidad => proyectoUsuarioIdentidad.IdentidadID, (identidad, proyectoUsuarioIdentidad) => new
            {
                Identidad = identidad,
                ProyectoUsuarioIdentidad = proyectoUsuarioIdentidad
            }).Join(mEntityContext.AdministradorProyecto, objeto => new { objeto.ProyectoUsuarioIdentidad.UsuarioID, objeto.Identidad.ProyectoID }, adminProy => new { adminProy.UsuarioID, adminProy.ProyectoID }, (objeto, adminProy) => new
            {
                Identidad = objeto.Identidad,
                ProyectoUsuarioIdentidad = objeto.ProyectoUsuarioIdentidad,
                AdministradorProyecto = adminProy
            }).Where(objeto => objeto.Identidad.ProyectoID.Equals(pProyectoID) && objeto.AdministradorProyecto.Tipo.Equals((short)TipoRolUsuario.Administrador) && objeto.Identidad.FechaBaja == null && objeto.Identidad.FechaExpulsion == null)
             .Select(objeto => objeto.Identidad.IdentidadID).ToList();

            return listaAdministradores;
        }

        /// <summary>
        /// Obtiene una lista con los supervisores de un proyecto 
        /// (Sólo los administradores --> Tipo = 1 )
        /// </summary>
        /// <param name="pProyectoID">Proyecto del que queremos obtener sus supervisores</param>
        /// <returns>Lista con las identidades de los supervisores de un proyecto</returns>
         //TODO Realizar con IdentidadDS
        public List<Guid> ObtenerListaIdentidadesSupervisoresPorProyecto(Guid pProyectoID)
        {
            List<Guid> listaSupervisores = new List<Guid>();

            listaSupervisores = mEntityContext.Identidad.Join(mEntityContext.Perfil, identidad => identidad.PerfilID, perfil => perfil.PerfilID, (identidad, perfil) => new
            {
                Idenitdad = identidad,
                Perfil = perfil
            }).Join(mEntityContext.Persona, objeto => objeto.Perfil.PersonaID, persona => persona.PersonaID, (objeto, persona) => new
            {
                Idenitdad = objeto.Idenitdad,
                Perfil = objeto.Perfil,
                Persona = persona
            }).Join(mEntityContext.Usuario, objeto => objeto.Persona.UsuarioID, usuario => usuario.UsuarioID, (objeto, usuario) => new
            {
                Idenitdad = objeto.Idenitdad,
                Perfil = objeto.Perfil,
                Persona = objeto.Persona,
                Usuario = usuario
            }).Join(mEntityContext.AdministradorProyecto, objeto => objeto.Usuario.UsuarioID, adminProy => adminProy.UsuarioID, (objeto, adminProy) => new
            {
                Idenitdad = objeto.Idenitdad,
                Perfil = objeto.Perfil,
                Persona = objeto.Persona,
                Usuario = objeto.Usuario,
                AdministradorProyecto = adminProy
            }).Where(objeto => objeto.AdministradorProyecto.ProyectoID.Equals(pProyectoID) && objeto.Idenitdad.ProyectoID.Equals(pProyectoID) && objeto.AdministradorProyecto.Tipo.Equals((short)TipoRolUsuario.Supervisor)).Select(objeto => objeto.Idenitdad.IdentidadID).ToList();

            return listaSupervisores;
        }

        /// <summary>
        /// Obtiene los nombres de los proyectos administrados por el usuario pasado por parámetro con el perfil dado
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <param name="pPerfilID">Identificador de perfil</param>
        /// <returns>Dataset de proyectos</returns>
        public DataWrapperProyecto ObtenerNombresProyectosAdministradosPorUsuarioID(Guid pUsuarioID, Guid pPerfilID)
        {
            ////TODO
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();
            //ProyectoDS proyectoDS = new ProyectoDS();

            //Lo ponemos porque solo cojemos el campo "Nombre"
            //proyectoDS.EnforceConstraints = false;

            //Proyecto
            //DbCommand commandsqlSelectProyectoPorAdministradorID = ObtenerComando(sqlSelectProyectoPorAdministradorID);
            //AgregarParametro(commandsqlSelectProyectoPorAdministradorID, IBD.ToParam("usuarioID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pUsuarioID));
            //AgregarParametro(commandsqlSelectProyectoPorAdministradorID, IBD.ToParam("perfilID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pPerfilID));
            //CargarDataSet(commandsqlSelectProyectoPorAdministradorID, proyectoDS, "Proyecto");
            //Lo ponemos porque solo cojemos el campo "Nombre"
            List<Proyecto> listaProyectos = mEntityContext.Proyecto.Join(mEntityContext.AdministradorProyecto, proy => proy.ProyectoID, adminProy => adminProy.ProyectoID, (proy, adminProy) => new
            {
                NombreProy = proy.Nombre,
                ProyectoID = proy.ProyectoID,
                TipoAcceso = proy.TipoAcceso,
                UsuarioID = adminProy.UsuarioID
            }).Join(
                mEntityContext.Identidad, proyAdminProy => proyAdminProy.ProyectoID, identidad => identidad.ProyectoID, (proyAdminProy, identidad) => new
                {
                    NombreProy = proyAdminProy.NombreProy,
                    ProyectoID = proyAdminProy.ProyectoID,
                    TipoAcceso = proyAdminProy.TipoAcceso,
                    UsuarioID = proyAdminProy.UsuarioID,
                    PerfilID = identidad.PerfilID
                }
                ).Where(proyAdminProy => ((proyAdminProy.TipoAcceso.Equals((short)TipoAcceso.Privado) || proyAdminProy.TipoAcceso.Equals((short)TipoAcceso.Reservado)) && proyAdminProy.UsuarioID.Equals(pUsuarioID)) && proyAdminProy.PerfilID.Equals(pPerfilID)).ToList().Select(proyAdminProyIden => new Proyecto { Nombre = proyAdminProyIden.NombreProy }).ToList();

            dataWrapperProyecto.ListaProyecto = listaProyectos;

            return (dataWrapperProyecto);
        }

        /// <summary>
        /// Obtiene los nombres de los proyectos administrados por el usuario pasado por parámetro con el perfil dado
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <param name="pPerfilID">Identificador de perfil</param>
        /// <returns>Dataset de proyectos</returns>
        public Dictionary<Guid, string> ObtenerNombresProyectosPrivadosAdministradosPorUsuario(Guid pUsuarioID, Guid pPerfilID)
        {
            Dictionary<Guid, string> listaComunidades = new Dictionary<Guid, string>();

            var objetosConsulta = mEntityContext.Proyecto.Join(mEntityContext.AdministradorProyecto, proyecto => proyecto.ProyectoID, adminProy => adminProy.ProyectoID, (proyecto, adminProy) => new
            {
                Proyecto = proyecto,
                AdministradorProyecto = adminProy
            }).Join(mEntityContext.Identidad, objeto => objeto.Proyecto.ProyectoID, idenitdad => idenitdad.ProyectoID, (objeto, identidad) => new
            {
                Proyecto = objeto.Proyecto,
                AdministradorProyecto = objeto.AdministradorProyecto,
                Identidad = identidad
            }).Where(objeto => ((objeto.Proyecto.TipoAcceso.Equals((short)TipoAcceso.Privado) || objeto.Proyecto.TipoAcceso.Equals((short)TipoAcceso.Reservado)) && objeto.AdministradorProyecto.UsuarioID.Equals(pUsuarioID)) && objeto.Identidad.PerfilID.Equals(pPerfilID)).Select(objeto => new { objeto.Proyecto.ProyectoID, objeto.Proyecto.Nombre }).ToList();

            //Proyecto
            foreach (var objetoConsulta in objetosConsulta)
            {
                Guid proyectoID = objetoConsulta.ProyectoID;
                if (!listaComunidades.ContainsKey(proyectoID))
                {
                    listaComunidades.Add(proyectoID, objetoConsulta.Nombre);
                }
            }

            return listaComunidades;
        }

        /// <summary>
        /// Obtiene los proyectos administrados por el perfil pasado por parámetro
        /// </summary>
        /// <param name="pPerfilID">Identificador de perfil</param>
        /// <returns>Dataset de proyectos</returns>
        public DataWrapperProyecto ObtenerProyectosAdministradosPorPerfilID(Guid pPerfilID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            //string fromProyecto = " FROM Proyecto INNER JOIN Identidad ON Identidad.ProyectoID = Proyecto.ProyectoID INNER JOIN ProyectoUsuarioIdentidad ON Identidad.IdentidadID = ProyectoUsuarioIdentidad.IdentidadID AND  Identidad.ProyectoID = ProyectoUsuarioIdentidad.ProyectoID INNER JOIN AdministradorProyecto ON ProyectoUsuarioIdentidad.ProyectoID = AdministradorProyecto.ProyectoID AND  ProyectoUsuarioIdentidad.UsuarioID = AdministradorProyecto.UsuarioID ";
            //string whereProyecto = " WHERE AdministradorProyecto.Tipo = " + (short)TipoRolUsuario.Administrador + " AND Identidad.PerfilID = " + IBD.GuidValor(pPerfilID) + " AND Proyecto.ProyectoID != " + IBD.GuidValor(ProyectoAD.MetaProyecto) + " ";
            List<Proyecto> listaProyectos = mEntityContext.Proyecto.Join(mEntityContext.Identidad, proyecto => proyecto.ProyectoID, identidad => identidad.ProyectoID, (proyecto, identidad) => new
            {
                Proyecto = proyecto,
                Identidad = identidad
            }).Join(mEntityContext.ProyectoUsuarioIdentidad, objeto => new { objeto.Identidad.IdentidadID, objeto.Identidad.ProyectoID }, proyUserIden => new { proyUserIden.IdentidadID, proyUserIden.ProyectoID }, (objeto, proyUserIden) => new
            {
                Proyecto = objeto.Proyecto,
                Identidad = objeto.Identidad,
                ProyectoUsuarioIdentidad = proyUserIden
            }).Join(mEntityContext.AdministradorProyecto, objeto => new { objeto.ProyectoUsuarioIdentidad.ProyectoID, objeto.ProyectoUsuarioIdentidad.UsuarioID }, adminProy => new { adminProy.ProyectoID, adminProy.UsuarioID }, (objeto, adminProy) => new
            {
                Proyecto = objeto.Proyecto,
                Identidad = objeto.Identidad,
                ProyectoUsuarioIdentidad = objeto.ProyectoUsuarioIdentidad,
                AdministradorProyecto = adminProy
            }).Where(objeto => objeto.AdministradorProyecto.Tipo.Equals((short)TipoRolUsuario.Administrador) && objeto.Identidad.PerfilID.Equals(pPerfilID) && !objeto.Proyecto.ProyectoID.Equals(ProyectoAD.MetaProyecto)).Select(objeto => objeto.Proyecto).ToList();
            //Proyecto

            dataWrapperProyecto.ListaProyecto = listaProyectos;

            return (dataWrapperProyecto);
        }

        /// <summary>
        /// Indica si el usuario ers el único administrador de un proyecto
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <returns>True si es el único administrador de algun proyecto</returns>
        public bool EsUsuarioAdministradorUnicoDeProyecto(Guid pUsuarioID)
        {
            return EsUsuarioAdministradorUnicoDeProyecto(pUsuarioID, Guid.Empty);
        }

        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>True si es el único administrador del proyecto</returns>
        public bool EsUsuarioAdministradorUnicoDeProyecto(Guid pUsuarioID, Guid pProyectoID)
        {
            //obtenemos una tabla con los proyectos de la q el usuario es admin y el numero de admin de los proyectos
            //string sql = "SELECT COUNT(*) NumAdmin,AdministradorProyecto.Proyectoid ";
            //sql += " FROM AdministradorProyecto ";
            //sql += " where proyectoid in ";
            //sql += " (select ProyectoID from AdministradorProyecto WHERE UsuarioID = " + IBD.GuidValor(pUsuarioID) + "   AND Tipo=" + (short)TipoRolUsuario.Administrador + ")";
            //sql += " group by ProyectoID ";

            List<Guid> listaGuidAdmin = mEntityContext.AdministradorProyecto.Where(adminProy => adminProy.UsuarioID.Equals(pUsuarioID) && adminProy.Tipo.Equals((short)TipoRolUsuario.Administrador)).Select(adminProy => adminProy.ProyectoID).ToList();

            var listaAdministradoresProyecto = mEntityContext.AdministradorProyecto.Where(adminProy => listaGuidAdmin.Contains(adminProy.ProyectoID)).GroupBy(adminProy => adminProy.ProyectoID, adminProy => adminProy.ProyectoID, (agrupacion, g) => new
            {
                ProyectoID = agrupacion,
                NumAdmin = g.ToList().Count
            }).Select(adminProy => new
            {
                adminProy.NumAdmin,
                adminProy.ProyectoID
            }).ToList();


            if (listaAdministradoresProyecto != null && listaAdministradoresProyecto.Count == 1)
            {
                return listaAdministradoresProyecto.First().NumAdmin == 1;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Obtiene los proyectos administrados por la organizacion pasado por parámetro
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organizacion</param>
        /// <returns>Dataset de proyectos</returns>
        public DataWrapperProyecto ObtenerProyectosAdministradosPorOrganizacionID(Guid pOrganizacionID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            //string fromProyecto = " FROM Proyecto INNER JOIN Identidad ON Identidad.ProyectoID = Proyecto.ProyectoID INNER JOIN Perfil ON Identidad.PerfilID = Perfil.PerfilID INNER JOIN ProyectoUsuarioIdentidad ON Identidad.IdentidadID = ProyectoUsuarioIdentidad.IdentidadID AND  Identidad.ProyectoID = ProyectoUsuarioIdentidad.ProyectoID INNER JOIN AdministradorProyecto ON ProyectoUsuarioIdentidad.ProyectoID = AdministradorProyecto.ProyectoID AND  ProyectoUsuarioIdentidad.UsuarioID = AdministradorProyecto.UsuarioID ";
            //string whereProyecto = " WHERE AdministradorProyecto.Tipo = " + (short)TipoRolUsuario.Administrador + " AND Perfil.OrganizacionID = " + IBD.GuidValor(pOrganizacionID) + " ";

            List<Proyecto> listaProyectosConsulta = mEntityContext.Proyecto.Join(mEntityContext.Identidad, proyecto => proyecto.ProyectoID, identidad => identidad.ProyectoID, (proyecto, identidad) => new
            {
                Proyecto = proyecto,
                Identidad = identidad
            }).Join(mEntityContext.Perfil, objeto => objeto.Identidad.PerfilID, perfil => perfil.PerfilID, (objeto, perfil) => new
            {
                Proyecto = objeto.Proyecto,
                Identidad = objeto.Identidad,
                Perfil = perfil
            }).Join(mEntityContext.ProyectoUsuarioIdentidad, objeto => new { objeto.Identidad.IdentidadID, objeto.Identidad.ProyectoID }, proyUsuarioIden => new { proyUsuarioIden.IdentidadID, proyUsuarioIden.ProyectoID }, (objeto, proyUsuarioIden) => new
            {
                Proyecto = objeto.Proyecto,
                Identidad = objeto.Identidad,
                Perfil = objeto.Perfil,
                ProyectoUsuarioIdentidad = proyUsuarioIden
            }).Join(mEntityContext.AdministradorProyecto, objeto => new { objeto.ProyectoUsuarioIdentidad.ProyectoID, objeto.ProyectoUsuarioIdentidad.UsuarioID }, adminProy => new { adminProy.ProyectoID, adminProy.UsuarioID }, (objeto, adminProy) => new
            {
                Proyecto = objeto.Proyecto,
                Identidad = objeto.Identidad,
                Perfil = objeto.Perfil,
                ProyectoUsuarioIdentidad = objeto.ProyectoUsuarioIdentidad,
                AdministradorProyecto = adminProy
            }).Where(objeto => objeto.AdministradorProyecto.Tipo.Equals((short)TipoRolUsuario.Administrador) && objeto.Perfil.OrganizacionID.Value.Equals(pOrganizacionID)).Select(objeto => objeto.Proyecto).ToList().Distinct().ToList();
            //Proyecto

            dataWrapperProyecto.ListaProyecto = listaProyectosConsulta;

            return (dataWrapperProyecto);
        }

        /// <summary>
        /// Obtiene el proyecto cuyo identificador se pasa por parámetro, además de sus niveles de certificación 
        /// y los permisos de los roles de usuario sobre los tipos de recursos del proyecto
        /// </summary>
        /// <param name="pProyectoID"></param>
        /// <returns></returns>
        public DataWrapperProyecto ObtenerProyectoPorIDConNiveles(Guid pProyectoID)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();

            //Proyecto
            List<Proyecto> listaProyectosConsulta = mEntityContext.Proyecto.Where(proyecto => proyecto.ProyectoID.Equals(pProyectoID)).ToList();

            //NivelCertificacion
            List<NivelCertificacion> listaNivelCertificacion = mEntityContext.NivelCertificacion.Where(nivelCertificacion => nivelCertificacion.ProyectoID.Equals(pProyectoID)).ToList();

            //TipoDocDispRolUsuarioProy
            List<TipoDocDispRolUsuarioProy> listaTipoDocDispRolUsuarioProy = mEntityContext.TipoDocDispRolUsuarioProy.Where(tipoDoc => tipoDoc.ProyectoID.Equals(pProyectoID)).ToList();

            //TipoOntoDispRolUsuarioProy
            List<TipoOntoDispRolUsuarioProy> listaTipoOntoDispRolUsuarioProy = mEntityContext.TipoOntoDispRolUsuarioProy.Where(tipoOnto => tipoOnto.ProyectoID.Equals(pProyectoID)).ToList();

            dataWrapperProyecto.ListaProyecto = listaProyectosConsulta;
            dataWrapperProyecto.ListaNivelCertificacion = listaNivelCertificacion;
            dataWrapperProyecto.ListaTipoDocDispRolUsuarioProy = listaTipoDocDispRolUsuarioProy;
            dataWrapperProyecto.ListaTipoOntoDispRolUsuarioProy = listaTipoOntoDispRolUsuarioProy;
            return (dataWrapperProyecto);
        }



        /// <summary>
        /// Obtiene los grupos que tienen permisos sobre una ontología en un determinado proyecto
        /// </summary>
        /// <param name="pListaOntologiasID">Lista de identificadores de ontología</param>
        /// <param name="pProyectoID">Identificador del proyecto en el que se hace la comprobación</param>
        /// <returns>Diccionario con los grupos de comunidad y organización que tienen permiso sobre la ontología</returns>
        public Dictionary<Guid, List<Guid>> ObtenerGruposPermitidosOntologiasEnProyecto(List<Guid> pListaOntologiasID, Guid pProyectoID)
        {
            Dictionary<Guid, List<Guid>> dicGruposOntologias = new Dictionary<Guid, List<Guid>>();

            if (pListaOntologiasID.Count > 0)
            {
                //string sql = "Select DocumentoID, GrupoID, Editor from DocumentoRolGrupoIdentidades where DocumentoID in (";


                var objetoConsulta = mEntityContext.DocumentoRolGrupoIdentidades.Where(documento => pListaOntologiasID.Contains(documento.DocumentoID)).ToList();


                foreach (DocumentoRolGrupoIdentidades fila in objetoConsulta)
                {
                    if (!dicGruposOntologias.ContainsKey(fila.DocumentoID))
                    {
                        dicGruposOntologias.Add(fila.DocumentoID, new List<Guid>());
                    }

                    if (!dicGruposOntologias[fila.DocumentoID].Contains(fila.GrupoID))
                    {
                        dicGruposOntologias[fila.DocumentoID].Add(fila.GrupoID);
                    }
                }
            }
            return dicGruposOntologias;
        }

        /// <summary>
        /// Obtiene las ontologías permitidas para un rol de usuario en un determinado proyecto
        /// </summary>
        /// <param name="pIdentidadEnProyID">Identificador de la identidad del usuario en el proyecto</param>
        /// <param name="pIdentidadEnMyGnossID">Identificador de la identidad del usuario en mygnoss</param>
        /// <param name="pProyectoID">Identificador del proyecto en el que se hace la comprobación</param>
        /// <param name="pTipoRol">Tipo de rol del usuario actual</param>
        /// <param name="pIdentidadDeOtroProyecto">Verdad si la identidad pertenece a otro proyecto distinto a pProyectoID</param>
        /// <param name="pOntologiasEcosistema">Ontologías del ecosistema. Si no se pasan, se obtienen de base de datos</param>
        /// <returns>Lista con las ontologías permitidas para la identidad</returns>
        public List<Guid> ObtenerOntologiasPermitidasIdentidadEnProyecto(Guid pIdentidadEnProyID, Guid pIdentidadEnMyGnossID, Guid pProyectoID, TipoRolUsuario pTipoRol, bool pIdentidadDeOtroProyecto, Dictionary<Guid, Guid> pOntologiasEcosistema = null, Guid? pDocumentoID = null)
        {
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();
            HashSet<Guid> listaOntologiasDisponibles = new HashSet<Guid>();
            bool primera = true;
            //TipoOntoDispRolUsuarioProy
            // DbCommand commandsqlSelectTipoOntoDispRolUsuarioProy = ObtenerComando(sqlSelectTipoOntoDispRolUsuarioProy + " WHERE ProyectoID=" + IBD.GuidValor(pProyectoID) + " AND TipoOntoDispRolUsuarioProy.RolUsuario >= " + (short)pTipoRol);
            List<TipoOntoDispRolUsuarioProy> listaTipoOntoDispRolUsuarioProy = mEntityContext.TipoOntoDispRolUsuarioProy.Where(tipoOnto => tipoOnto.ProyectoID.Equals(pProyectoID) && tipoOnto.RolUsuario >= (short)pTipoRol).ToList();
            dataWrapperProyecto.ListaTipoOntoDispRolUsuarioProy = listaTipoOntoDispRolUsuarioProy;
            if (pOntologiasEcosistema == null)
            {
                pOntologiasEcosistema = ObtenerOntologiasEcosistema();
            }

            if (pOntologiasEcosistema != null && pOntologiasEcosistema.Count > 0)
            {
                //commandsqlSelectTipoOntoDispRolUsuarioProy = ObtenerComando(sqlSelectTipoOntoDispRolUsuarioProy + " WHERE");

                foreach (Guid ontologiaID in pOntologiasEcosistema.Keys)
                {
                    if (!pOntologiasEcosistema[ontologiaID].Equals(pProyectoID))
                    {
                        if (primera)
                        {
                            listaTipoOntoDispRolUsuarioProy = mEntityContext.TipoOntoDispRolUsuarioProy.Where(tipoOnto => tipoOnto.ProyectoID.Equals(pOntologiasEcosistema[ontologiaID]) && tipoOnto.OntologiaID.Equals(ontologiaID) && tipoOnto.RolUsuario >= (short)pTipoRol).ToList();
                            primera = false;
                        }
                        else
                        {
                            listaTipoOntoDispRolUsuarioProy = listaTipoOntoDispRolUsuarioProy.Union(mEntityContext.TipoOntoDispRolUsuarioProy.Where(tipoOnto => tipoOnto.ProyectoID.Equals(pOntologiasEcosistema[ontologiaID]) && tipoOnto.OntologiaID.Equals(ontologiaID) && tipoOnto.RolUsuario >= (short)pTipoRol)).ToList();
                        }
                        //commandsqlSelectTipoOntoDispRolUsuarioProy.CommandText += " (ProyectoID=" + IBD.GuidValor(pOntologiasEcosistema[ontologiaID]) + " AND OntologiaID=" + IBD.GuidValor(ontologiaID) + " AND TipoOntoDispRolUsuarioProy.RolUsuario >= " + (short)pTipoRol + ") OR";
                    }
                }

                //commandsqlSelectTipoOntoDispRolUsuarioProy.CommandText = commandsqlSelectTipoOntoDispRolUsuarioProy.CommandText.Substring(0, commandsqlSelectTipoOntoDispRolUsuarioProy.CommandText.Length - 3);
                dataWrapperProyecto.ListaTipoOntoDispRolUsuarioProy = dataWrapperProyecto.ListaTipoOntoDispRolUsuarioProy.Union(listaTipoOntoDispRolUsuarioProy).ToList();
                //CargarDataSet(commandsqlSelectTipoOntoDispRolUsuarioProy, dataWrapperProyecto, "TipoOntoDispRolUsuarioProy");
            }

            //rellenar lista
            foreach (TipoOntoDispRolUsuarioProy fila in dataWrapperProyecto.ListaTipoOntoDispRolUsuarioProy)
            {
                listaOntologiasDisponibles.Add(fila.OntologiaID);
            }

            string whereDocumentoID = "";
            if (pDocumentoID.HasValue)
            {
                whereDocumentoID = $"and Documento.DocumentoID = {IBD.GuidValor(pDocumentoID.Value)}";
            }

            StringBuilder sb = new StringBuilder();
            string whereDocumento = "";
            List<Guid> listaGuids = new List<Guid>();
            if (pDocumentoID.HasValue)
            {
                //whereDocumento = $" and DocumentoID={pDocumentoID.Value}";
                listaGuids = mEntityContext.Documento.Join(mEntityContext.DocumentoRolGrupoIdentidades, doc => doc.DocumentoID, docRolGrupoIden => docRolGrupoIden.DocumentoID, (doc, docRolGrupoIden) => new
                {
                    Documento = doc,
                    DocumentoRolGrupoIdentidades = docRolGrupoIden
                }).Join(mEntityContext.GrupoIdentidadesParticipacion, objeto => objeto.DocumentoRolGrupoIdentidades.GrupoID, participacion => participacion.GrupoID, (objeto, participacion) => new
                {
                    Documento = objeto.Documento,
                    DocumentoRolGrupoIdentidades = objeto.DocumentoRolGrupoIdentidades,
                    GrupoIdentidadesParticipacion = participacion
                }).Where(objeto => objeto.Documento.ProyectoID.HasValue && objeto.Documento.ProyectoID.Value.Equals(pProyectoID) && (objeto.GrupoIdentidadesParticipacion.IdentidadID.Equals(pIdentidadEnProyID) || objeto.GrupoIdentidadesParticipacion.IdentidadID.Equals(pIdentidadEnMyGnossID)) && objeto.Documento.Tipo == 7 && objeto.Documento.Eliminado == false && objeto.Documento.DocumentoID.Equals(pDocumentoID.Value)).Select(objeto => objeto.Documento.DocumentoID).ToList();
            }
            else
            {
                listaGuids = mEntityContext.Documento.Join(mEntityContext.DocumentoRolGrupoIdentidades, doc => doc.DocumentoID, docRolGrupoIden => docRolGrupoIden.DocumentoID, (doc, docRolGrupoIden) => new
                {
                    Documento = doc,
                    DocumentoRolGrupoIdentidades = docRolGrupoIden
                }).Join(mEntityContext.GrupoIdentidadesParticipacion, objeto => objeto.DocumentoRolGrupoIdentidades.GrupoID, participacion => participacion.GrupoID, (objeto, participacion) => new
                {
                    Documento = objeto.Documento,
                    DocumentoRolGrupoIdentidades = objeto.DocumentoRolGrupoIdentidades,
                    GrupoIdentidadesParticipacion = participacion
                }).Where(objeto => objeto.Documento.ProyectoID.HasValue && objeto.Documento.ProyectoID.Value.Equals(pProyectoID) && (objeto.GrupoIdentidadesParticipacion.IdentidadID.Equals(pIdentidadEnProyID) || objeto.GrupoIdentidadesParticipacion.IdentidadID.Equals(pIdentidadEnMyGnossID)) && objeto.Documento.Tipo == 7 && objeto.Documento.Eliminado == false).Select(objeto => objeto.Documento.DocumentoID).ToList();
            }

            //sb.AppendLine("select Documento.DocumentoID FROM Documento");
            //sb.AppendLine("inner join DocumentoRolGrupoIdentidades on Documento.DocumentoID = DocumentoRolGrupoIdentidades.DocumentoID");
            //sb.AppendLine($"inner join GrupoIdentidadesParticipacion on GrupoIdentidadesParticipacion.GrupoID = DocumentoRolGrupoIdentidades.GrupoID WHERE Documento.ProyectoID = {IBD.GuidValor(pProyectoID)} and (GrupoIdentidadesParticipacion.IdentidadID = {IBD.GuidValor(pIdentidadEnProyID)} OR GrupoIdentidadesParticipacion.IdentidadID = {IBD.GuidValor(pIdentidadEnMyGnossID)}) and Documento.Tipo = 7 and Documento.Eliminado = 0 {whereDocumento}");

            if (pIdentidadDeOtroProyecto)
            {
                // La identidad es de otro proyecto (proviene del parametro ProyectoIDPatronOntologias)
                // Hago join a partir del nombrecorto de los grupos en los que participa el usuario en la comunidad de origen
                sb.AppendLine("UNION ALL");
                sb.AppendLine("select Documento.DocumentoID FROM Documento");
                sb.AppendLine("inner join DocumentoRolGrupoIdentidades on Documento.DocumentoID = DocumentoRolGrupoIdentidades.DocumentoID");
                sb.AppendLine("inner join GrupoIdentidades GrupoIdentidadesPatron on GrupoIdentidadesPatron.GrupoID = DocumentoRolGrupoIdentidades.GrupoID");
                sb.AppendLine("inner join GrupoidentidadesProyecto GrupoidentidadesProyectoPatron on GrupoIdentidadesPatron.Grupoid = GrupoidentidadesProyectoPatron.grupoid");
                sb.AppendLine("inner join GrupoIdentidades GurpoIdentidadesHija on GurpoIdentidadesHija.nombrecorto = GrupoIdentidadesPatron.nombrecorto");
                sb.AppendLine("inner join GrupoIdentidadesParticipacion on GrupoIdentidadesParticipacion.GrupoID = GurpoIdentidadesHija.GrupoID");
                sb.AppendLine($"where Documento.ProyectoID = {IBD.GuidValor(pProyectoID)}");
                sb.AppendLine($"  {whereDocumento} and GrupoIdentidadesParticipacion.IdentidadID = {IBD.GuidValor(pIdentidadEnProyID)}");

                if (pDocumentoID.HasValue)
                {
                    var primeraConsulta = mEntityContext.Documento.Join(mEntityContext.DocumentoRolGrupoIdentidades, documento => documento.DocumentoID, documentoRolIdentidad => documentoRolIdentidad.DocumentoID, (documento, documentoRolGrupoIdentidades) => new
                    {
                        Documento = documento,
                        DocumentoRolGrupoIdentidades = documentoRolGrupoIdentidades
                    }).Join(mEntityContext.GrupoIdentidades, item => item.DocumentoRolGrupoIdentidades.GrupoID, grupoIdentidades => grupoIdentidades.GrupoID, (item, grupoIdentidades) => new
                    {
                        Documento = item.Documento,
                        DocumentoRolGrupoIdentidades = item.DocumentoRolGrupoIdentidades,
                        GrupoIdentidades = grupoIdentidades
                    }).Join(mEntityContext.GrupoIdentidadesProyecto, item => item.GrupoIdentidades.GrupoID, grupoIdentidadesProyecto => grupoIdentidadesProyecto.GrupoID, (item, grupoIdentidadesProyecto) => new
                    {
                        Documento = item.Documento,
                        DocumentoRolGrupoIdentidades = item.DocumentoRolGrupoIdentidades,
                        GrupoIdentidades = item.GrupoIdentidades,
                        GrupoIdentidadesProyecto = grupoIdentidadesProyecto
                    }).Join(mEntityContext.GrupoIdentidades, item => item.GrupoIdentidades.NombreCorto, grupoIdentidadesHija => grupoIdentidadesHija.NombreCorto, (item, grupoIdentidadesHija) => new
                    {
                        Documento = item.Documento,
                        DocumentoRolGrupoIdentidades = item.DocumentoRolGrupoIdentidades,
                        GrupoIdentidades = item.GrupoIdentidades,
                        GrupoIdentidadesProyecto = item.GrupoIdentidadesProyecto,
                        GrupoIdentidadesHija = grupoIdentidadesHija
                    }).Join(mEntityContext.GrupoIdentidadesParticipacion, item => item.GrupoIdentidadesHija.GrupoID, grupoIdentidadesParticipacion => grupoIdentidadesParticipacion.GrupoID, (item, grupoIdentidadesParticipacion) => new
                    {
                        Documento = item.Documento,
                        DocumentoRolGrupoIdentidades = item.DocumentoRolGrupoIdentidades,
                        GrupoIdentidades = item.GrupoIdentidades,
                        GrupoIdentidadesProyecto = item.GrupoIdentidadesProyecto,
                        GrupoIdentidadesHija = item.GrupoIdentidadesHija,
                        GrupoIdentidadesParticipacion = grupoIdentidadesParticipacion
                    }).Where(item => item.Documento.ProyectoID.HasValue && item.Documento.ProyectoID.Value.Equals(pProyectoID) && item.GrupoIdentidadesParticipacion.IdentidadID.Equals(pIdentidadEnProyID) && item.Documento.DocumentoID.Equals(pDocumentoID.Value)).Select(item => item.Documento.DocumentoID);

                    listaGuids = listaGuids.Union(primeraConsulta).ToList();

                }
                else
                {
                    listaGuids = listaGuids.Union(mEntityContext.Documento.Join(mEntityContext.DocumentoRolGrupoIdentidades, documento => documento.DocumentoID, docRolGrupoIden => docRolGrupoIden.DocumentoID, (documento, docRolGrupoIden) => new
                    {
                        Documento = documento,
                        DocumentoRolGrupoIdentidades = docRolGrupoIden
                    }).Join(mEntityContext.GrupoIdentidades, objeto => objeto.DocumentoRolGrupoIdentidades.GrupoID, grupoIdentidadesPatron => grupoIdentidadesPatron.GrupoID, (objeto, grupoIdentidadesPatron) => new
                    {
                        Documento = objeto.Documento,
                        DocumentoRolGrupoIdentidades = objeto.DocumentoRolGrupoIdentidades,
                        GrupoIdentidadesPatron = grupoIdentidadesPatron
                    }).Join(mEntityContext.GrupoIdentidadesProyecto, objeto => objeto.GrupoIdentidadesPatron.GrupoID, grupoidentidadesProyectoPatron => grupoidentidadesProyectoPatron.GrupoID, (objeto, grupoidentidadesProyectoPatron) => new
                    {
                        Documento = objeto.Documento,
                        DocumentoRolGrupoIdentidades = objeto.DocumentoRolGrupoIdentidades,
                        GrupoIdentidadesPatron = objeto.GrupoIdentidadesPatron,
                        GrupoidentidadesProyectoPatron = grupoidentidadesProyectoPatron
                    }).Join(mEntityContext.GrupoIdentidades, objeto => objeto.GrupoIdentidadesPatron.NombreCorto, grupoIdentidadesHija => grupoIdentidadesHija.NombreCorto, (objeto, grupoIdentidadesHija) => new
                    {
                        Documento = objeto.Documento,
                        DocumentoRolGrupoIdentidades = objeto.DocumentoRolGrupoIdentidades,
                        GrupoIdentidadesPatron = objeto.GrupoIdentidadesPatron,
                        GrupoidentidadesProyectoPatron = objeto.GrupoidentidadesProyectoPatron,
                        GrupoIdentidadesHija = grupoIdentidadesHija
                    }).Join(mEntityContext.GrupoIdentidadesParticipacion, objeto => objeto.GrupoIdentidadesHija.GrupoID, grupoIdentidadesParticipacion => grupoIdentidadesParticipacion.GrupoID, (objeto, grupoIdentidadesParticipacion) => new
                    {
                        Documento = objeto.Documento,
                        DocumentoRolGrupoIdentidades = objeto.DocumentoRolGrupoIdentidades,
                        GrupoIdentidadesPatron = objeto.GrupoIdentidadesPatron,
                        GrupoidentidadesProyectoPatron = objeto.GrupoidentidadesProyectoPatron,
                        GrupoIdentidadesHija = objeto.GrupoIdentidadesHija,
                        GrupoIdentidadesParticipacion = grupoIdentidadesParticipacion
                    }).Where(objeto => objeto.Documento.ProyectoID.Value.Equals(pProyectoID) && objeto.GrupoIdentidadesParticipacion.IdentidadID.Equals(pIdentidadEnProyID)).Select(objeto => objeto.Documento.DocumentoID)
                    ).ToList();
                }
            }

            //Obtengo las ontologías disponibles por grupo



            foreach (Guid id in listaGuids)
            {
                listaOntologiasDisponibles.Add(id);
            }



            return listaOntologiasDisponibles.ToList();
        }

        /// <summary>
        /// Obtiene las ontologías del ecosistema
        /// </summary>
        /// <returns>Lista con los DocumentoID de todas las ontologías con su ProyectoID como valor del diccionario</returns>
        public Dictionary<Guid, Guid> ObtenerOntologiasEcosistema()
        {
            Dictionary<Guid, Guid> listaOntologias = new Dictionary<Guid, Guid>();
            //Obtengo las ontologías disponibles para todo el entorno:
            //DbCommand commandsqlSelectTipoOntoDispRolUsuarioProy = ObtenerComando("SELECT DocumentoID, ProyectoID FROM Documento WHERE Visibilidad=1 AND Tipo=" + (short)TiposDocumentacion.Ontologia + " AND Eliminado=0");
            var resultadoConsulta = mEntityContext.Documento.Where(documento => documento.Visibilidad == 1 && documento.Tipo.Equals((short)TiposDocumentacion.Ontologia) && documento.Eliminado.Equals(false)).Select(documento => new { documento.DocumentoID, documento.ProyectoID }).ToList();

            foreach (var fila in resultadoConsulta)
            {
                Guid documentoID = fila.DocumentoID;
                Guid proyectoID = fila.ProyectoID.Value;

                listaOntologias.Add(documentoID, proyectoID);
            }


            return listaOntologias;
        }

        ///// <summary>
        ///// Obtiene las ontologías permitidas para un rol de usuario en un determinado proyecto
        ///// </summary>
        ///// <param name="pProyectoID">ID de proyecto</param>
        ///// <returns>DataSet con las ontologías permitidas para un rol de usuario en un determinado proyecto cargadas</returns>
        //public ProyectoDS ObtenerOntologiasDisponiblesProyecto(Guid pProyectoID)
        //{
        //    ProyectoDS proyectoDS = new ProyectoDS();

        //    //TipoOntoDispRolUsuarioProy
        //    DbCommand commandsqlSelectTipoOntoDispRolUsuarioProy = ObtenerComando(sqlSelectTipoOntoDispRolUsuarioProy + " WHERE ProyectoID=" + IBD.GuidValor(pProyectoID));
        //    CargarDataSet(commandsqlSelectTipoOntoDispRolUsuarioProy, proyectoDS, "TipoOntoDispRolUsuarioProy");

        //    //Obtengo las ontologias disponibles para todo el entorno:
        //    commandsqlSelectTipoOntoDispRolUsuarioProy = ObtenerComando("SELECT DocumentoID, ProyectoID FROM Documento WHERE Visibilidad=1 AND ProyectoID<>" + IBD.GuidValor(pProyectoID) + " AND Tipo=" + (short)TiposDocumentacion.Ontologia + " AND Eliminado=0");
        //    DataSet dataSetAux = new DataSet();
        //    CargarDataSet(commandsqlSelectTipoOntoDispRolUsuarioProy, dataSetAux, "Documento");

        //    if (dataSetAux.Tables[0].Rows.Count > 0)
        //    {
        //        commandsqlSelectTipoOntoDispRolUsuarioProy = ObtenerComando(sqlSelectTipoOntoDispRolUsuarioProy + " WHERE");

        //        foreach (DataRow fila in dataSetAux.Tables[0].Rows)
        //        {
        //            commandsqlSelectTipoOntoDispRolUsuarioProy.CommandText += " (ProyectoID=" + IBD.GuidValor((Guid)fila["ProyectoID"]) + " AND OntologiaID=" + IBD.GuidValor((Guid)fila["DocumentoID"]) + ") OR";
        //        }

        //        commandsqlSelectTipoOntoDispRolUsuarioProy.CommandText = commandsqlSelectTipoOntoDispRolUsuarioProy.CommandText.Substring(0, commandsqlSelectTipoOntoDispRolUsuarioProy.CommandText.Length - 3);
        //        CargarDataSet(commandsqlSelectTipoOntoDispRolUsuarioProy, proyectoDS, "TipoOntoDispRolUsuarioProy");
        //    }

        //    dataSetAux.Dispose();

        //    return (proyectoDS);
        //}

        /// <summary>
        /// Devuelve una lista con las claves de los proyectos que tienen alguna categoria del tesauro de MyGnoss en comun con el pProyectoID
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto a buscar sus relacionados</param>
        /// <returns>Lista de Claves de Proyectos</returns>
        public List<Guid> ObtenerListaProyectoRelacionados(Guid pProyectoID)
        {
            List<int> l = new List<int>();
            return ObtenerListaProyectoRelacionados(pProyectoID, out l);
            ;
        }

        /// <summary>
        /// Devuelve una lista con las claves de los proyectos que tienen alguna categoria del tesauro de MyGnoss en comun con el pProyectoID
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto a buscar sus relacionados</param>
        /// <param name="pManual">Indica si los proyectos son manuales o automaticos</param>
        /// <returns>Lista de Claves de Proyectos</returns>
        public List<Guid> ObtenerListaProyectoRelacionados(Guid pProyectoID, out bool pManual)
        {
            List<int> l = new List<int>();
            return ObtenerListaProyectoRelacionados(pProyectoID, out l, out pManual);
        }

        /// <summary>
        /// Devuelve una lista con las claves de los proyectos que tienen alguna categoria del tesauro de MyGnoss en comun con el pProyectoID
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto a buscar sus relacionados</param>
        /// <param name="pManual">Indica si los proyectos son manuales o automaticos</param>
        /// <returns>Lista de Claves de Proyectos</returns>
        public List<Guid> ObtenerListaProyectoRelacionados(Guid pProyectoID, out List<int> listaIdNumerico)
        {
            bool manual = true;
            return ObtenerListaProyectoRelacionados(pProyectoID, out listaIdNumerico, out manual);
        }

        /// <summary>
        /// Devuelve una lista con las claves de los proyectos que tienen alguna categoria del tesauro de MyGnoss en comun con el pProyectoID
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto a buscar sus relacionados</param>
        /// <param name="pManual">Indica si los proyectos son manuales o automaticos</param>
        /// <returns>Lista de Claves de Proyectos</returns>
        public List<Guid> ObtenerListaProyectoRelacionados(Guid pProyectoID, out List<int> listaIdNumerico, out bool pManual)
        {
            pManual = true;

            listaIdNumerico = new List<int>();
            List<Guid> listaProyectosRelacionados = new List<Guid>();
            DataWrapperProyecto proyectoDS = new DataWrapperProyecto();
            //proyectoDS.EnforceConstraints = false;

            //string sqlProyectoTablaRelacion = "SELECT " + IBD.CargarGuid("ProyectoRelacionado.ProyectoRelacionadoID") + " ProyectoID, 0 Tipo, ProyectoRelacionado.Orden, TablaBaseProyectoId FROM Proyecto INNER JOIN ProyectoRelacionado ON (Proyecto.ProyectoID=ProyectoRelacionado.ProyectoID AND Proyecto.OrganizacionID=ProyectoRelacionado.OrganizacionID) WHERE Proyecto.ProyectoID = " + IBD.GuidParamValor("proyectoID") + " ";

            var listaProyectoTablaRelacion = mEntityContext.Proyecto.Join(mEntityContext.ProyectoRelacionado, proy => new { proy.ProyectoID, proy.OrganizacionID }, proyRelacionado => new { proyRelacionado.ProyectoID, proyRelacionado.OrganizacionID }, (proy, proyRelacionado) => new
            {
                ProyectoRelacionadoID = proyRelacionado.ProyectoRelacionadoID,
                ProyectoID = proy.ProyectoID,
                Tipo = 0,
                Orden = proyRelacionado.Orden,
                TablaBaseProyectoId = proy.TablaBaseProyectoID
            }).Where(proyectosTablaRelacion => proyectosTablaRelacion.ProyectoID.Equals(pProyectoID));

            //string sqlProyectoPadre = "SELECT " + IBD.CargarGuid("Proyecto.ProyectoID") + ", 1 Tipo, 0 Orden, TablaBaseProyectoId FROM Proyecto WHERE Proyecto.ProyectoID IN (Select Proyecto.ProyectoSuperiorID from proyecto WHERE Proyecto.ProyectoID = " + IBD.GuidParamValor("proyectoID") + ") AND Proyecto.TipoProyecto <> " + (short)TipoProyecto.MetaComunidad + " AND Proyecto.Estado <> " + (short)EstadoProyecto.Cerrado + " AND Proyecto.Estado <> " + (short)EstadoProyecto.CerradoTemporalmente + " AND Proyecto.Estado  <> " + (short)EstadoProyecto.Definicion + "  AND (Proyecto.TipoAcceso=" + (short)TipoAcceso.Publico + " OR Proyecto.TipoAcceso=" + (short)TipoAcceso.Restringido + ") ";

            var listaGuidsProyectoSuperiorID = mEntityContext.Proyecto.Where(proy => proy.ProyectoID.Equals(pProyectoID) && proy.ProyectoSuperiorID.HasValue).Select(proyect => proyect.ProyectoSuperiorID.Value);

            var resultadoProyectoPadre = mEntityContext.Proyecto.Where(proy => listaGuidsProyectoSuperiorID.Contains(proy.ProyectoID) && !proy.TipoProyecto.Equals((short)TipoProyecto.MetaComunidad) && !proy.Estado.Equals((short)EstadoProyecto.Cerrado) && !proy.Estado.Equals((short)EstadoProyecto.CerradoTemporalmente) && !proy.Estado.Equals((short)EstadoProyecto.Definicion) && (proy.TipoAcceso.Equals((short)TipoAcceso.Publico) || proy.TipoAcceso.Equals((short)TipoAcceso.Restringido))).Select(proy => new
            {
                ProyectoID = proy.ProyectoID,
                Tipo = 2,
                Orden = 0,
                TablaBaseProyectoID = proy.TablaBaseProyectoID
            });
            //////////
            //string sqlProyectosHijos = "SELECT " + IBD.CargarGuid("Proyecto.ProyectoID") + ", 2 Tipo, 0 Orden, TablaBaseProyectoId FROM Proyecto WHERE Proyecto.ProyectoSuperiorID = " + IBD.GuidParamValor("proyectoID") + " AND Proyecto.TipoProyecto <> " + (short)TipoProyecto.MetaComunidad + " AND Proyecto.Estado <> " + (short)EstadoProyecto.Cerrado + " AND Proyecto.Estado <> " + (short)EstadoProyecto.CerradoTemporalmente + " AND Proyecto.Estado  <> " + (short)EstadoProyecto.Definicion + "  AND (Proyecto.TipoAcceso=" + (short)TipoAcceso.Publico + " OR Proyecto.TipoAcceso=" + (short)TipoAcceso.Restringido + ") ";

            var listaProyectosHijos = mEntityContext.Proyecto.Where(proy => proy.ProyectoSuperiorID.HasValue && proy.ProyectoSuperiorID.Value.Equals(pProyectoID) && !proy.TipoProyecto.Equals((short)TipoProyecto.MetaComunidad) && !proy.Estado.Equals((short)EstadoProyecto.Cerrado) && !proy.Estado.Equals((short)EstadoProyecto.CerradoTemporalmente) && !proy.Estado.Equals((short)EstadoProyecto.Definicion) && (proy.TipoAcceso.Equals((short)TipoAcceso.Publico) || proy.TipoAcceso.Equals((short)TipoAcceso.Restringido))).Select(proy => new
            {
                ProyectoID = proy.ProyectoID,
                Tipo = 2,
                Orden = 0,
                TablaBaseProyectoID = proy.TablaBaseProyectoID
            });
            /////////
            //string sqlProyectosHermanos = "SELECT " + IBD.CargarGuid("Proyecto.ProyectoID") + ", 3 Tipo, 0 Orden, TablaBaseProyectoId FROM Proyecto WHERE Proyecto.ProyectoSuperiorID IN (Select Proyecto.ProyectoSuperiorID from proyecto WHERE Proyecto.ProyectoID = " + IBD.GuidParamValor("proyectoID") + ") AND Proyecto.TipoProyecto <> " + (short)TipoProyecto.MetaComunidad + " AND Proyecto.Estado <> " + (short)EstadoProyecto.Cerrado + " AND Proyecto.Estado <> " + (short)EstadoProyecto.CerradoTemporalmente + " AND Proyecto.Estado <> " + (short)EstadoProyecto.Definicion + "  AND (Proyecto.TipoAcceso=" + (short)TipoAcceso.Publico + " OR Proyecto.TipoAcceso=" + (short)TipoAcceso.Restringido + ") ";

            var listaProyectosHermanos = mEntityContext.Proyecto.Where(proy => listaGuidsProyectoSuperiorID.Contains(proy.ProyectoID) && !proy.TipoProyecto.Equals((short)TipoProyecto.MetaComunidad) && !proy.Estado.Equals((short)EstadoProyecto.Cerrado) && !proy.Estado.Equals((short)EstadoProyecto.CerradoTemporalmente) && !proy.Estado.Equals((short)EstadoProyecto.Definicion) && (proy.TipoAcceso.Equals((short)TipoAcceso.Publico) || proy.TipoAcceso.Equals((short)TipoAcceso.Restringido))).Select(proy => new
            {
                ProyectoID = proy.ProyectoID,
                Tipo = 3,
                Orden = 0,
                TablaBaseProyectoID = proy.TablaBaseProyectoID
            });

            /////////
            //string sqlProyectosRelacionados = "SELECT DISTINCT " + IBD.CargarGuid("Proyecto.ProyectoID") + ", 4 Tipo, 0 Orden, TablaBaseProyectoId FROM Proyecto INNER JOIN ProyectoAgCatTesauro ON ProyectoAgCatTesauro.ProyectoID = Proyecto.ProyectoID WHERE ProyectoAgCatTesauro.CategoriaTesauroID IN ( SELECT ProyectoAgCatTesauro.CategoriaTesauroID FROM Proyecto INNER JOIN ProyectoAgCatTesauro ON ProyectoAgCatTesauro.ProyectoID = Proyecto.ProyectoID WHERE Proyecto.ProyectoID= " + IBD.GuidParamValor("proyectoID") + ") AND Proyecto.ProyectoID <> '" + ProyectoAD.ProyectoFAQ + "' AND Proyecto.ProyectoID <> '" + ProyectoAD.ProyectoNoticias + "' AND Proyecto.TipoProyecto <> " + (short)TipoProyecto.MetaComunidad + " AND Proyecto.Estado <> " + (short)EstadoProyecto.Cerrado + " AND Proyecto.Estado <> " + (short)EstadoProyecto.CerradoTemporalmente + " AND Proyecto.Estado <> " + (short)EstadoProyecto.Definicion + "  AND (Proyecto.TipoAcceso=" + (short)TipoAcceso.Publico + " OR Proyecto.TipoAcceso=" + (short)TipoAcceso.Restringido + ") ";

            var listaGuidProyectoAgCategoriaTesauro = mEntityContext.Proyecto.Join(mEntityContext.ProyectoAgCatTesauro, proy => proy.ProyectoID, proyAgCatTes => proyAgCatTes.ProyectoID, (proy, ProyectoAgCatTesauro) => new
            {
                CategoriaTesauroID = ProyectoAgCatTesauro.CategoriaTesauroID,
                ProyectoID = proy.ProyectoID
            }).Where(proyCatTes => proyCatTes.ProyectoID.Equals(pProyectoID)).Select(objeto => objeto.CategoriaTesauroID);

            var listaProyectosRelacionadosRes = mEntityContext.Proyecto.Join(mEntityContext.ProyectoAgCatTesauro, proy => proy.ProyectoID, proyAgCatTesauro => proyAgCatTesauro.ProyectoID, (proy, proyAgCatTesauro) => new
            {
                ProyectoID = proy.ProyectoID,
                Tipo = 4,
                Orden = 0,
                CategoriaTesauroID = proyAgCatTesauro.CategoriaTesauroID,
                TablaBaseProyectoID = proy.TablaBaseProyectoID,
                TipoProyecto = proy.TipoProyecto,
                Estado = proy.Estado,
                TipoAcceso = proy.TipoAcceso
            }).Where(proyRel => listaGuidProyectoAgCategoriaTesauro.Contains(proyRel.CategoriaTesauroID) && !proyRel.ProyectoID.Equals(ProyectoAD.ProyectoFAQ) && !proyRel.ProyectoID.Equals(ProyectoAD.ProyectoNoticias) && !proyRel.TipoProyecto.Equals((short)TipoProyecto.MetaComunidad) && !proyRel.Estado.Equals((short)EstadoProyecto.Cerrado) && !proyRel.Estado.Equals((short)EstadoProyecto.CerradoTemporalmente) && !proyRel.Estado.Equals((short)EstadoProyecto.Definicion) && (proyRel.TipoAcceso.Equals((short)TipoAcceso.Publico) || proyRel.TipoAcceso.Equals((short)TipoAcceso.Restringido))).Select(proyRel => proyRel);
            /////////
            //string sqlManual = "SELECT " + IBD.CargarGuid("TablaProyectos.ProyectoID") + ", TablaProyectos.Tipo, TablaProyectos.Orden, ProyectosMasActivos.Peso FROM ( " + sqlProyectoTablaRelacion + " )TablaProyectos INNER JOIN ProyectosMasActivos ON TablaProyectos.ProyectoID = ProyectosMasActivos.ProyectoID Order By TablaProyectos.Tipo, TablaProyectos.Orden, ProyectosMasActivos.Peso DESC ";

            var resManual = listaProyectoTablaRelacion.Join(mEntityContext.ProyectosMasActivos, tablaProyectos => new { ProyectoID = tablaProyectos.ProyectoRelacionadoID }, masActivos => new { ProyectoID = masActivos.ProyectoID }, (tablaProyectos, masActivos) => new
            {
                ProyectoID = tablaProyectos.ProyectoRelacionadoID,
                Tipo = tablaProyectos.Tipo,
                Orden = tablaProyectos.Orden,
                Peso = masActivos.Peso,
                TablaBaseProyectoID = tablaProyectos.TablaBaseProyectoId
            }).OrderByDescending(tablaProyectosMasActivos => tablaProyectosMasActivos.Orden).ThenByDescending(tablaProyectosMasActivos => tablaProyectosMasActivos.Peso);
            /////////
            //string sqlAuto = "SELECT " + IBD.CargarGuid("TablaProyectos.ProyectoID") + ", TablaProyectos.Tipo, TablaProyectos.Orden, ProyectosMasActivos.Peso FROM ( " + sqlProyectoPadre + " UNION ALL " + sqlProyectosHijos + " UNION ALL " + sqlProyectosHermanos + " UNION ALL " + sqlProyectosRelacionados + " )TablaProyectos INNER JOIN ProyectosMasActivos ON TablaProyectos.ProyectoID = ProyectosMasActivos.ProyectoID Order By TablaProyectos.Tipo, TablaProyectos.Orden, ProyectosMasActivos.Peso DESC ";

            var listaProyectosRelacionadosResForUnion = listaProyectosRelacionadosRes.Select(proy => new
            {
                ProyectoID = proy.ProyectoID,
                Tipo = proy.Tipo,
                Orden = proy.Orden,
                TablaBaseProyectoID = proy.TablaBaseProyectoID
            });
            var ProyHijosHermanosRelUnionAll = resultadoProyectoPadre.Concat(listaProyectosHijos).Concat(listaProyectosHermanos).Concat(listaProyectosRelacionadosResForUnion);
            var auto = ProyHijosHermanosRelUnionAll.Join(mEntityContext.ProyectosMasActivos, tablaProyectos => tablaProyectos.ProyectoID, masActivos => masActivos.ProyectoID, (tablaProyectos, masActivos) => new
            {
                ProyectoId = tablaProyectos.ProyectoID,
                Tipo = tablaProyectos.Tipo,
                Orden = tablaProyectos.Orden,
                Peso = masActivos.Peso,
                TablaBaseProyectoID = tablaProyectos.TablaBaseProyectoID
            }).OrderByDescending(resultado => resultado.Tipo).ThenByDescending(resultado => resultado.Orden).ThenByDescending(resultado => resultado.Peso);


            foreach (var proyect in resManual.ToList())
            {
                Proyecto proyecto = new Proyecto();
                proyecto.ProyectoID = proyect.ProyectoID;
                proyecto.TablaBaseProyectoID = proyect.TablaBaseProyectoID;
                proyectoDS.ListaProyecto.Add(proyecto);
            }

            foreach (Proyecto filaProyecto in proyectoDS.ListaProyecto)
            {
                if (!listaProyectosRelacionados.Contains(filaProyecto.ProyectoID) && filaProyecto.ProyectoID != pProyectoID)
                {
                    listaIdNumerico.Add(filaProyecto.TablaBaseProyectoID);
                    listaProyectosRelacionados.Add(filaProyecto.ProyectoID);
                }
            }

            if (listaProyectosRelacionados.Count == 0)
            {
                pManual = false;

                //DbCommand commandProyectoRelacionadosAuto = ObtenerComando(sqlAuto);
                //AgregarParametro(commandProyectoRelacionadosAuto, IBD.ToParam("proyectoID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pProyectoID));
                //CargarDataSet(commandProyectoRelacionadosAuto, proyectoDS, "Proyecto");

                foreach (var proyect in auto.ToList())
                {
                    Proyecto proyecto = new Proyecto();
                    proyecto.ProyectoID = proyect.ProyectoId;
                    proyecto.TablaBaseProyectoID = proyect.TablaBaseProyectoID;
                    proyectoDS.ListaProyecto.Add(proyecto);
                }

                foreach (Proyecto filaProyecto in proyectoDS.ListaProyecto)
                {
                    if (!listaProyectosRelacionados.Contains(filaProyecto.ProyectoID) && filaProyecto.ProyectoID != pProyectoID)
                    {
                        listaIdNumerico.Add(filaProyecto.TablaBaseProyectoID);
                        listaProyectosRelacionados.Add(filaProyecto.ProyectoID);
                    }
                }
            }

            return listaProyectosRelacionados;
        }

        /// <summary>
        /// Devuelve una lista con las claves de los proyectos que administra el usuario
        /// </summary>
        /// <param name="pUsuarioID">Clave del usuario</param>
        /// <param name="pPerfilID">Clave del perfil</param>
        /// <returns>Lista de Claves de Proyectos</returns>
        public List<Guid> ObtenerListaProyectosAdministraUsuarioYPerfil(Guid pUsuarioID, Guid pPerfilID)
        {
            List<Guid> listaProyectosAdministra = new List<Guid>();

            listaProyectosAdministra = mEntityContext.AdministradorProyecto.Join(mEntityContext.Identidad, adminProy => adminProy.ProyectoID, identidad => identidad.ProyectoID, (adminProy, identidad) => new
            {
                AdministradorProyecto = adminProy,
                Identidad = identidad
            }).Where(objeto => objeto.AdministradorProyecto.UsuarioID.Equals(pUsuarioID) && objeto.Identidad.PerfilID.Equals(pPerfilID)).Select(objeto => objeto.AdministradorProyecto.ProyectoID).ToList();

            return listaProyectosAdministra;
        }

        #endregion

        #region Administración del tesauro

        /// <summary>
        /// Obtiene todos los documentos que están vinculados a un serie de categorias.
        /// </summary>
        /// <param name="pListaCategorias">Lista con las categorias a las que están agregados los documentos</param>
        /// <param name="pTesauroID">Identificador del tesauro al que pertenecen las categorías</param>
        /// <returns>DataSet de documentación con los documentos</returns>
        public DataWrapperProyecto ObtenerVinculacionProyectosDeCategoriasTesauro(List<Guid> pListaCategorias, Guid pTesauroID)
        {
            DataWrapperProyecto proyectoDS = new DataWrapperProyecto();

            if (pListaCategorias.Count > 0)
            {

                List<ProyectoAgCatTesauro> listaProyectoAgCatTesauro = mEntityContext.ProyectoAgCatTesauro.Where(proyAgCatTesauro => pListaCategorias.Contains(proyAgCatTesauro.CategoriaTesauroID) && proyAgCatTesauro.TesauroID.Equals(pTesauroID)).ToList();
                proyectoDS.ListaProyectoAgCatTesauro = listaProyectoAgCatTesauro;
            }
            return proyectoDS;
        }

        #endregion

        #region Proyectos hijos

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pProyectoID"></param>
        /// <returns></returns>
        public List<Guid> ObtenerProyectosHijosDeProyecto(Guid pProyectoID)
        {
            var resultadoComando = mEntityContext.ProyectoPestanyaBusqueda.Join(mEntityContext.ProyectoPestanyaMenu, proyPesBus => proyPesBus.PestanyaID, proyPesMenu => proyPesMenu.PestanyaID, (proyPesBus, proyPesMenu) => new
            {
                ProyectoID = proyPesMenu.ProyectoID,
                proyPesBus.ProyectoOrigenID
            }).Where(proyJoin => proyJoin.ProyectoOrigenID.Equals(pProyectoID)).GroupBy(proyJoin => proyJoin.ProyectoID).Select(proyJoin => proyJoin.Key);
            List<Guid> listaHijos = new List<Guid>();

            foreach (var fila in resultadoComando.ToList())
            {
                listaHijos.Add(fila);
            }
            return listaHijos;
        }

        /// <summary>
        /// Obtiene el ID del proyecto origen del actual, si lo tiene o GUID.Empty si no.
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <returns>ID del proyecto origen del actual, si lo tiene o GUID.Empty si no</returns>
        public Guid ObtenerProyectoOrigenDeProyecto(Guid pProyectoID)
        {//"SELECT Distinct ProyectoPestanyaBusqueda.ProyectoOrigenID FROM ProyectoPestanyaBusqueda INNER JOIN ProyectoPestanyaMenu ON ProyectoPestanyaMenu.PestanyaID=ProyectoPestanyaBusqueda.PestanyaID WHERE ProyectoPestanyaMenu.ProyectoID=" + IBD.GuidValor(pProyectoID));
            var resultadoConsulta = mEntityContext.ProyectoPestanyaBusqueda.Join(mEntityContext.ProyectoPestanyaMenu, proyPestBus => proyPestBus.PestanyaID, proyPestMenu => proyPestMenu.PestanyaID, (proyPestBus, proyPestMenu) => new
            {
                ProyectoOrigenID = proyPestBus.ProyectoOrigenID,
                ProyectoID = proyPestMenu.ProyectoID
            }).Where(proyJoin => proyJoin.ProyectoID.Equals(pProyectoID) && proyJoin.ProyectoOrigenID.HasValue).Select(proyJoin => proyJoin.ProyectoOrigenID.Value).Distinct();

            Guid proyOrgID = Guid.Empty;

            if (resultadoConsulta.ToList().Count > 0)
            {
                proyOrgID = resultadoConsulta.ToList().First();
            }
            return proyOrgID;
        }

        #endregion

        /// <summary>
        /// Obtiene la URL de un servicio externo de un proyecto y en caso de no existir lo busca en el ecosistema
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <param name="pNombre">Nomrbe del servicio</param>
        /// <returns></returns>
        public string ObtenerUrlServicioExterno(Guid pProyectoID, string pNombre)
        {
            var proyectoServicioExterno = mEntityContext.ProyectoServicioExterno.Where(proyServicioExterno => proyServicioExterno.ProyectoID.Equals(pProyectoID) && proyServicioExterno.NombreServicio.Equals(pNombre)).FirstOrDefault();
            string url = null;
            if (proyectoServicioExterno == null || string.IsNullOrEmpty(proyectoServicioExterno.UrlServicio))
            {
                var ecosistemaServicioExterno = mEntityContext.EcosistemaServicioExterno.Where(item => item.NombreServicio.Equals(pNombre)).FirstOrDefault();

                if (ecosistemaServicioExterno != null)
                {
                    url = ecosistemaServicioExterno.UrlServicio;
                }
            }
            else
            {
                url = proyectoServicioExterno.UrlServicio;
            }

            return url;
        }

        public string ObtenerIdiomaPrincipalDominio(string pDominio)
        {
            string idioma = "es";

            if (!string.IsNullOrEmpty(pDominio))
            {
                //DbCommand cmdSelectUrlPropia = ObtenerComando("SELECT URLPropia from Proyecto WHERE URLPropia LIKE " + IBD.ToParam("dominio"));
                //AgregarParametro(cmdSelectUrlPropia, IBD.ToParam("dominio"), DbType.String, "%" + pDominio + "@%");

                string url = mEntityContext.Proyecto.Where(proyecto => proyecto.URLPropia.Contains(pDominio + "@")).Select(proyecto => proyecto.URLPropia).FirstOrDefault();



                if ((url != null) && ((string)url).IndexOf(pDominio) >= 0)
                {
                    string urlAux = (string)url;

                    urlAux = urlAux.Substring(urlAux.IndexOf(pDominio) + pDominio.Length + 1);

                    if (urlAux.Contains("|||"))
                    {
                        idioma = urlAux.Substring(0, urlAux.IndexOf("|||"));
                    }
                    else
                    {
                        idioma = urlAux;
                    }
                }
            }

            return idioma;
        }

        #endregion

        #region Privados

        /// <summary>
        /// En caso de que se utilice el GnossConfig.xml por defecto se sigue utilizando el IBD estático
        /// </summary>
        private void CargarConsultasYDataAdapters()
        {
            this.CargarConsultasYDataAdapters(IBD);
        }

        /// <summary>
        /// En caso de que se utilice un GnossConfig.xml que no es el de por defecto se pasa un objeto IBaseDatos creado con respecto
        /// al fichero de configuracion que se ha apsado como parámetro
        /// </summary>
        /// <param name="IBD">Objecto IBaseDatos para el archivo pasado al constructor del AD</param>
        private void CargarConsultasYDataAdapters(IBaseDatos IBD)
        {
            #region Consultas

            this.sqlSelectProyectoPestanyaFiltroOrdenRecursos = "SELECT " + IBD.CargarGuid("ProyectoPestanyaFiltroOrdenRecursos.PestanyaID") + ", ProyectoPestanyaFiltroOrdenRecursos.FiltroOrden, ProyectoPestanyaFiltroOrdenRecursos.NombreFiltro, ProyectoPestanyaFiltroOrdenRecursos.Orden FROM ProyectoPestanyaFiltroOrdenRecursos";

            this.sqlSelectProyectoConfigExtraSem = "SELECT " + IBD.CargarGuid("ProyectoConfigExtraSem.ProyectoID") + ", ProyectoConfigExtraSem.UrlOntologia, ProyectoConfigExtraSem.SourceTesSem, ProyectoConfigExtraSem.Tipo, ProyectoConfigExtraSem.Nombre, ProyectoConfigExtraSem.Idiomas, ProyectoConfigExtraSem.PrefijoTesSem, ProyectoConfigExtraSem.Editable FROM ProyectoConfigExtraSem";

            this.sqlSelectProyectoRelacionado = "SELECT " + IBD.CargarGuid("ProyectoRelacionado.OrganizacionID") + ", " + IBD.CargarGuid("ProyectoRelacionado.ProyectoID") + ", " + IBD.CargarGuid("ProyectoRelacionado.OrganizacionRelacionadaID") + ", " + IBD.CargarGuid("ProyectoRelacionado.ProyectoRelacionadoID") + ", Orden FROM ProyectoRelacionado";

            this.sqlSelectProyectoPorAdministradorID = "SELECT Proyecto.Nombre FROM Proyecto INNER JOIN AdministradorProyecto ON Proyecto.ProyectoID = AdministradorProyecto.ProyectoID INNER JOIN Identidad ON Proyecto.ProyectoID = Identidad.ProyectoID WHERE ((Proyecto.TipoAcceso = " + (short)TipoAcceso.Privado + " OR Proyecto.TipoAcceso = " + (short)TipoAcceso.Reservado + ") AND AdministradorProyecto.UsuarioID = " + IBD.GuidParamValor("usuarioID") + ") AND (Identidad.PerfilID = " + IBD.GuidParamValor("perfilID") + ")";

            this.sqlSelectTipoDocDispRolUsuarioProy = "SELECT " + IBD.CargarGuid("TipoDocDispRolUsuarioProy.OrganizacionID") + ", " + IBD.CargarGuid("TipoDocDispRolUsuarioProy.ProyectoID") + ", TipoDocDispRolUsuarioProy.TipoDocumento, TipoDocDispRolUsuarioProy.RolUsuario FROM TipoDocDispRolUsuarioProy";

            this.sqlSelectTipoOntoDispRolUsuarioProy = "SELECT " + IBD.CargarGuid("TipoOntoDispRolUsuarioProy.OrganizacionID") + ", " + IBD.CargarGuid("TipoOntoDispRolUsuarioProy.ProyectoID") + ", TipoOntoDispRolUsuarioProy.OntologiaID, TipoOntoDispRolUsuarioProy.RolUsuario FROM TipoOntoDispRolUsuarioProy";

            this.sqlSelectNivelCertificacion = "SELECT " + IBD.CargarGuid("NivelCertificacion.NivelCertificacionID") + ", " + IBD.CargarGuid("NivelCertificacion.OrganizacionID") + ", " + IBD.CargarGuid("NivelCertificacion.ProyectoID") + ", NivelCertificacion.Orden, NivelCertificacion.Descripcion FROM NivelCertificacion";

            this.sqlSelectProyectoAgCatTesauro = "SELECT " + IBD.CargarGuid("ProyectoAgCatTesauro.OrganizacionID") + ", " + IBD.CargarGuid("ProyectoAgCatTesauro.ProyectoID") + ", " + IBD.CargarGuid("ProyectoAgCatTesauro.TesauroID") + ", " + IBD.CargarGuid("ProyectoAgCatTesauro.CategoriaTesauroID") + " FROM ProyectoAgCatTesauro";

            this.sqlDeleteTodosProyectosMasActivos = "DELETE FROM ProyectosMasActivos";

            this.sqlSelectProyectosMasActivos = "SELECT " + IBD.CargarGuid("ProyectosMasActivos.OrganizacionID") + ", " + IBD.CargarGuid("ProyectosMasActivos.ProyectoID") + ", ProyectosMasActivos.Nombre, ProyectosMasActivos.NumeroConsultas, ProyectosMasActivos.Peso FROM ProyectosMasActivos INNER JOIN Proyecto ON ProyectosMasActivos.ProyectoID = Proyecto.ProyectoID ";

            this.sqlSelectPresentacionListadoSemantico = "SELECT " + IBD.CargarGuid("PresentacionListadoSemantico.OrganizacionID") + ", " + IBD.CargarGuid("PresentacionListadoSemantico.ProyectoID") + ", " + IBD.CargarGuid("PresentacionListadoSemantico.OntologiaID") + ",  PresentacionListadoSemantico.Orden,PresentacionListadoSemantico.Ontologia, PresentacionListadoSemantico.Propiedad, PresentacionListadoSemantico.Nombre FROM PresentacionListadoSemantico ";

            this.sqlSelectPresentacionMosaicoSemantico = "SELECT " + IBD.CargarGuid("PresentacionMosaicoSemantico.OrganizacionID") + ", " + IBD.CargarGuid("PresentacionMosaicoSemantico.ProyectoID") + ", " + IBD.CargarGuid("PresentacionMosaicoSemantico.OntologiaID") + ", PresentacionMosaicoSemantico.Orden,PresentacionMosaicoSemantico.Ontologia, PresentacionMosaicoSemantico.Propiedad, PresentacionMosaicoSemantico.Nombre FROM PresentacionMosaicoSemantico ";

            this.sqlSelectPresentacionMapaSemantico = "SELECT " + IBD.CargarGuid("PresentacionMapaSemantico.OrganizacionID") + ", " + IBD.CargarGuid("PresentacionMapaSemantico.ProyectoID") + ", " + IBD.CargarGuid("PresentacionMapaSemantico.OntologiaID") + ", PresentacionMapaSemantico.Orden,PresentacionMapaSemantico.Ontologia, PresentacionMapaSemantico.Propiedad, PresentacionMapaSemantico.Nombre FROM PresentacionMapaSemantico ";

            this.sqlSelectPresentacionPestanyaListadoSemantico = "SELECT " + IBD.CargarGuid("PresentacionPestanyaListadoSemantico.OrganizacionID") + ", " + IBD.CargarGuid("PresentacionPestanyaListadoSemantico.ProyectoID") + ", " + IBD.CargarGuid("PresentacionPestanyaListadoSemantico.PestanyaID") + ", " + IBD.CargarGuid("PresentacionPestanyaListadoSemantico.OntologiaID") + ",  PresentacionPestanyaListadoSemantico.Orden,PresentacionPestanyaListadoSemantico.Ontologia, PresentacionPestanyaListadoSemantico.Propiedad, PresentacionPestanyaListadoSemantico.Nombre FROM PresentacionPestanyaListadoSemantico ";

            this.sqlSelectPresentacionPestanyaMosaicoSemantico = "SELECT " + IBD.CargarGuid("PresentacionPestanyaMosaicoSemantico.OrganizacionID") + ", " + IBD.CargarGuid("PresentacionPestanyaMosaicoSemantico.ProyectoID") + ", " + IBD.CargarGuid("PresentacionPestanyaMosaicoSemantico.PestanyaID") + ", " + IBD.CargarGuid("PresentacionPestanyaMosaicoSemantico.OntologiaID") + ", PresentacionPestanyaMosaicoSemantico.Orden,PresentacionPestanyaMosaicoSemantico.Ontologia, PresentacionPestanyaMosaicoSemantico.Propiedad, PresentacionPestanyaMosaicoSemantico.Nombre FROM PresentacionPestanyaMosaicoSemantico ";

            this.sqlSelectPresentacionPestanyaMapaSemantico = "SELECT " + IBD.CargarGuid("PresentacionPestanyaMapaSemantico.OrganizacionID") + ", " + IBD.CargarGuid("PresentacionPestanyaMapaSemantico.ProyectoID") + ", " + IBD.CargarGuid("PresentacionPestanyaMapaSemantico.PestanyaID") + ", " + IBD.CargarGuid("PresentacionPestanyaMapaSemantico.OntologiaID") + ", PresentacionPestanyaMapaSemantico.Orden,PresentacionPestanyaMapaSemantico.Ontologia, PresentacionPestanyaMapaSemantico.Propiedad, PresentacionPestanyaMapaSemantico.Nombre FROM PresentacionPestanyaMapaSemantico ";

            //parte SELECT
            this.sqlSelectProyectoCerradoTmp = "SELECT " + IBD.CargarGuid("ProyectoCerradoTmp.OrganizacionID") + ", " + IBD.CargarGuid("ProyectoCerradoTmp.ProyectoID") + ", ProyectoCerradoTmp.Motivo, ProyectoCerradoTmp.FechaCierre, ProyectoCerradoTmp.FechaReapertura ";

            this.sqlSelectProyectoCerrandose = "SELECT " + IBD.CargarGuid("ProyectoCerrandose.OrganizacionID") + ", " + IBD.CargarGuid("ProyectoCerrandose.ProyectoID") + ", ProyectoCerrandose.FechaCierre, ProyectoCerrandose.PeriodoDeGracia ";

            this.SelectProyectoPesado = "SELECT " + IBD.CargarGuid("Proyecto.OrganizacionID") + ", " + IBD.CargarGuid("Proyecto.ProyectoID") + ", Proyecto.Nombre, Proyecto.Descripcion, Proyecto.FechaInicio, Proyecto.FechaFin, Proyecto.TipoProyecto, Proyecto.TipoAcceso, Proyecto.NumeroRecursos, Proyecto.NumeroPreguntas, Proyecto.NumeroDebates, Proyecto.NumeroMiembros, Proyecto.NumeroOrgRegistradas, Proyecto.NumeroArticulos, Proyecto.NumeroDafos, Proyecto.NumeroForos, Proyecto.Estado , " + IBD.CargarGuid("Proyecto.ProyectoSuperiorID") + ", EsProyectoDestacado, Proyecto.URLPropia, Proyecto.NombreCorto, Proyecto.TieneTwitter, Proyecto.TagTwitter, Proyecto.UsuarioTwitter, Proyecto.TokenTwitter, Proyecto.TokenSecretoTwitter, Proyecto.EnviarTwitterComentario, Proyecto.EnviarTwitterNuevaCat, Proyecto.EnviarTwitterNuevoAdmin, Proyecto.EnviarTwitterNuevaPolitCert, Proyecto.EnviarTwitterNuevoTipoDoc, Proyecto.TablaBaseProyectoID, Proyecto.ProcesoVinculadoID, Proyecto.Tags, Proyecto.TagTwitterGnoss, Proyecto.NombrePresentacion ";

            this.SelectProyectoLigero = "SELECT " + IBD.CargarGuid("Proyecto.OrganizacionID") + ", " + IBD.CargarGuid("Proyecto.ProyectoID") + ", Proyecto.Nombre, Proyecto.TipoProyecto, Proyecto.TipoAcceso, Proyecto.NumeroRecursos, Proyecto.NumeroPreguntas, Proyecto.NumeroDebates, Proyecto.NumeroMiembros, Proyecto.NumeroOrgRegistradas, Proyecto.EsProyectoDestacado, Proyecto.Estado, Proyecto.URLPropia,Proyecto.NombreCorto, Proyecto.Descripcion, " + IBD.CargarGuid("Proyecto.ProyectoSuperiorID") + ", Proyecto.TieneTwitter, Proyecto.TagTwitter, Proyecto.UsuarioTwitter, Proyecto.TokenTwitter, Proyecto.TokenSecretoTwitter,  Proyecto.EnviarTwitterComentario, Proyecto.EnviarTwitterNuevaCat, Proyecto.EnviarTwitterNuevoAdmin, Proyecto.EnviarTwitterNuevaPolitCert, Proyecto.EnviarTwitterNuevoTipoDoc, Proyecto.TablaBaseProyectoID, Proyecto.ProcesoVinculadoID, Proyecto.Tags, Proyecto.TagTwitterGnoss, Proyecto.NombrePresentacion ";

            this.SelectDistinctProyectoLigero = this.SelectProyectoLigero.Replace("SELECT", "SELECT DISTINCT");

            this.sqlSelectProyectoDestacado = SelectProyectoPesado + " FROM Proyecto WHERE EsProyectoDestacado = 1";

            this.SelectAdministradorProyecto = "SELECT " + IBD.CargarGuid("AdministradorProyecto.OrganizacionID") + ", " + IBD.CargarGuid("AdministradorProyecto.ProyectoID") + ", " + IBD.CargarGuid("AdministradorProyecto.UsuarioID") + ", AdministradorProyecto.Tipo ";

            this.SelectAdministradorGrupoProyecto = "SELECT " + IBD.CargarGuid("AdministradorGrupoProyecto.OrganizacionID") + ", " + IBD.CargarGuid("AdministradorGrupoProyecto.ProyectoID") + ", " + IBD.CargarGuid("AdministradorGrupoProyecto.GrupoID");

            this.SelectProyectoServicioExterno = "SELECT " + IBD.CargarGuid("ProyectoServicioExterno.OrganizacionID") + ", " + IBD.CargarGuid("ProyectoServicioExterno.ProyectoID") + ", ProyectoServicioExterno.NombreServicio, ProyectoServicioExterno.UrlServicio ";

            //this.SelectEcosistemaServicioExterno = "SELECT EcosistemaServicioExterno.NombreServicio, EcosistemaServicioExterno.UrlServicio ";

            //Consultas
            this.sqlSelectUrlPropiaProyecto = "SELECT URLPropia FROM Proyecto WHERE ProyectoID = " + IBD.GuidParamValor("proyectoID");

            this.sqlSelectUrlPropiaProyectoPorNombreCorto = "SELECT URLPropia FROM Proyecto WHERE NombreCorto=" + IBD.ToParam("nombreCorto");

            sqlSelectNombreCortoProyecto = "SELECT NombreCorto FROM Proyecto WHERE OrganizacionID = " + IBD.GuidParamValor("organizacionID") + " AND ProyectoID = " + IBD.GuidParamValor("proyectoID");

            sqlSelectOrganizacionIDProyecto = "SELECT " + IBD.CargarGuid("Proyecto.OrganizacionID") + " FROM Proyecto WHERE ProyectoID = " + IBD.GuidParamValor("proyectoID");

            sqlSelectNombreCortoProyectoConSoloProyectoID = "SELECT NombreCorto FROM Proyecto WHERE ProyectoID = " + IBD.GuidParamValor("proyectoID");

            sqlSelectExisteProyectoFAQ = "SELECT 1 FROM Proyecto WHERE ProyectoID = " + IBD.GuidValor(ProyectoFAQ);

            sqlSelectExisteProyectoNoticias = "SELECT 1 FROM Proyecto WHERE ProyectoID = " + IBD.GuidValor(ProyectoNoticias);

            sqlSelectExisteProyectoDidactalia = "SELECT 1 FROM Proyecto WHERE ProyectoID = " + IBD.GuidValor(ProyectoDidactalia);

            this.sqlSelectTodosAdministradoresProyectos = SelectAdministradorProyecto + "FROM AdministradorProyecto";

            sqlSelectTodosAdministradoresGrupoProyectos = SelectAdministradorGrupoProyecto + " FROM AdministradorGrupoProyecto";

            this.sqlSelectWikiDeProyecto = "SELECT " + IBD.CargarGuid("WikiGnossID") + " FROM WikiGnossProyecto WHERE (OrganizacionID = " + IBD.GuidParamValor("organizacionID") + ") AND (ProyectoID = " + IBD.GuidParamValor("proyectoID") + ")";

            this.sqlSelectProyectosOrganizacion = SelectProyectoPesado + "  FROM Proyecto WHERE (OrganizacionID = " + IBD.GuidParamValor("OrganizacionID") + ")ORDER BY Nombre";

            this.sqlSelectTodosProyectos = SelectProyectoPesado + " FROM Proyecto Order By Nombre";

            this.sqlSelectTodosProyectosLigera = SelectProyectoLigero + " FROM Proyecto Order By Nombre";

            this.sqlSelectProyectosCerrandose = sqlSelectProyectoCerrandose + " FROM ProyectoCerrandose";

            this.sqlSelectProyectosCerrandoTmp = sqlSelectProyectoCerradoTmp + " FROM ProyectoCerradoTmp";

            sqlSelectProyectosUsuarioPuedeCompartirRecurso = SelectProyectoLigero.Replace("SELECT", "SELECT DISTINCT") + " FROM PROYECTO INNER JOIN ProyectoUsuarioIdentidad ON Proyecto.OrganizacionID = ProyectoUsuarioIdentidad.OrganizacionGnossID AND Proyecto.ProyectoID = ProyectoUsuarioIdentidad.ProyectoID INNER JOIN AdministradorProyecto ON AdministradorProyecto.OrganizacionID = Proyecto.OrganizacionID AND AdministradorProyecto.ProyectoID = Proyecto.ProyectoID AND AdministradorProyecto.UsuarioID = ProyectoUsuarioIdentidad.UsuarioID INNER JOIN TipoDocDispRolUsuarioProy ON TipoDocDispRolUsuarioProy.ProyectoID = Proyecto.ProyectoID AND TipoDocDispRolUsuarioProy.OrganizacionID = Proyecto.OrganizacionID AND TipoDocDispRolUsuarioProy.RolUsuario >= AdministradorProyecto.Tipo WHERE (ProyectoUsuarioIdentidad.UsuarioID = " + IBD.GuidParamValor("usuarioID") + ") AND Proyecto.Estado = " + (short)EstadoProyecto.Abierto + " AND TipoDocDispRolUsuarioProy.TipoDocumento = " + IBD.ToParam("tipoDocumento") + " UNION " + SelectProyectoLigero.Replace("SELECT", "SELECT DISTINCT") + " FROM PROYECTO INNER JOIN ProyectoUsuarioIdentidad ON Proyecto.OrganizacionID = ProyectoUsuarioIdentidad.OrganizacionGnossID AND Proyecto.ProyectoID = ProyectoUsuarioIdentidad.ProyectoID INNER JOIN TipoDocDispRolUsuarioProy ON TipoDocDispRolUsuarioProy.ProyectoID = Proyecto.ProyectoID AND TipoDocDispRolUsuarioProy.OrganizacionID = Proyecto.OrganizacionID WHERE (ProyectoUsuarioIdentidad.UsuarioID = " + IBD.GuidParamValor("usuarioID") + ") AND Proyecto.Estado = " + (short)EstadoProyecto.Abierto + " AND TipoDocDispRolUsuarioProy.RolUsuario = " + (short)TipoRolUsuario.Usuario + " AND TipoDocDispRolUsuarioProy.TipoDocumento = " + IBD.ToParam("tipoDocumento") + " UNION " + SelectProyectoLigero.Replace("SELECT", "SELECT DISTINCT") + " FROM Proyecto INNER JOIN AdministradorProyecto ON Proyecto.Proyectoid = AdministradorProyecto.ProyectoID INNER JOIN TesauroProyecto ON Proyecto.ProyectoID = TesauroProyecto.ProyectoID INNER JOIN CategoriaTesauro ON TesauroProyecto.TesauroID = CategoriaTesauro.TesauroID WHERE AdministradorProyecto.UsuarioID = " + IBD.GuidParamValor("usuarioID") + " ORDER BY Proyecto.Nombre";

            sqlSelectProyectosUsuarioPuedeCompartirRecursoSinComprobarTipoDoc = SelectProyectoLigero.Replace("SELECT", "SELECT DISTINCT") + " FROM PROYECTO INNER JOIN ProyectoUsuarioIdentidad ON Proyecto.OrganizacionID = ProyectoUsuarioIdentidad.OrganizacionGnossID AND Proyecto.ProyectoID = ProyectoUsuarioIdentidad.ProyectoID WHERE (ProyectoUsuarioIdentidad.UsuarioID = " + IBD.GuidParamValor("usuarioID") + ") AND Proyecto.Estado = " + (short)EstadoProyecto.Abierto + " UNION " + SelectProyectoLigero.Replace("SELECT", "SELECT DISTINCT") + " FROM Proyecto inner join AdministradorProyecto on Proyecto.Proyectoid = AdministradorProyecto.ProyectoID INNER JOIN TesauroProyecto ON Proyecto.ProyectoID = TesauroProyecto.ProyectoID INNER JOIN CategoriaTesauro ON TesauroProyecto.TesauroID = CategoriaTesauro.TesauroID WHERE AdministradorProyecto.UsuarioID = " + IBD.GuidParamValor("usuarioID") + " ORDER BY Proyecto.Nombre";
            //Ojito con esta consulta (3er union)
            sqlSelectProyectoUsuarioIdentidadUsuarioPuedeCompartirRecurso = "SELECT " + IBD.CargarGuid("ProyectoUsuarioIdentidad.OrganizacionGnossID") + ", " + IBD.CargarGuid("ProyectoUsuarioIdentidad.ProyectoID") + ", " + IBD.CargarGuid("ProyectoUsuarioIdentidad.UsuarioID") + ", " + IBD.CargarGuid("ProyectoUsuarioIdentidad.IdentidadID") + ", FechaEntrada, Reputacion FROM PROYECTO INNER JOIN ProyectoUsuarioIdentidad ON Proyecto.OrganizacionID = ProyectoUsuarioIdentidad.OrganizacionGnossID AND Proyecto.ProyectoID = ProyectoUsuarioIdentidad.ProyectoID INNER JOIN AdministradorProyecto ON AdministradorProyecto.OrganizacionID = Proyecto.OrganizacionID AND AdministradorProyecto.ProyectoID = Proyecto.ProyectoID AND AdministradorProyecto.UsuarioID = ProyectoUsuarioIdentidad.UsuarioID INNER JOIN TipoDocDispRolUsuarioProy ON TipoDocDispRolUsuarioProy.ProyectoID = Proyecto.ProyectoID AND TipoDocDispRolUsuarioProy.OrganizacionID = Proyecto.OrganizacionID AND TipoDocDispRolUsuarioProy.RolUsuario >= AdministradorProyecto.Tipo WHERE (ProyectoUsuarioIdentidad.UsuarioID = " + IBD.GuidParamValor("usuarioID") + ") AND Proyecto.Estado = " + (short)EstadoProyecto.Abierto + " AND TipoDocDispRolUsuarioProy.TipoDocumento = " + IBD.ToParam("tipoDocumento") + " UNION SELECT " + IBD.CargarGuid("ProyectoUsuarioIdentidad.OrganizacionGnossID") + ", " + IBD.CargarGuid("ProyectoUsuarioIdentidad.ProyectoID") + ", " + IBD.CargarGuid("ProyectoUsuarioIdentidad.UsuarioID") + ", " + IBD.CargarGuid("ProyectoUsuarioIdentidad.IdentidadID") + ", FechaEntrada, Reputacion FROM PROYECTO INNER JOIN ProyectoUsuarioIdentidad ON Proyecto.OrganizacionID = ProyectoUsuarioIdentidad.OrganizacionGnossID AND Proyecto.ProyectoID = ProyectoUsuarioIdentidad.ProyectoID INNER JOIN TipoDocDispRolUsuarioProy ON TipoDocDispRolUsuarioProy.ProyectoID = Proyecto.ProyectoID AND TipoDocDispRolUsuarioProy.OrganizacionID = Proyecto.OrganizacionID WHERE (ProyectoUsuarioIdentidad.UsuarioID = " + IBD.GuidParamValor("usuarioID") + ") AND Proyecto.Estado = " + (short)EstadoProyecto.Abierto + " AND TipoDocDispRolUsuarioProy.RolUsuario = " + (short)TipoRolUsuario.Usuario + " AND TipoDocDispRolUsuarioProy.TipoDocumento = " + IBD.ToParam("tipoDocumento") + " UNION SELECT DISTINCT " + IBD.CargarGuid("ProyectoUsuarioIdentidad.OrganizacionGnossID") + ", " + IBD.CargarGuid("ProyectoUsuarioIdentidad.ProyectoID") + ", " + IBD.CargarGuid("ProyectoUsuarioIdentidad.UsuarioID") + ", " + IBD.CargarGuid("ProyectoUsuarioIdentidad.IdentidadID") + ", FechaEntrada, Reputacion FROM Proyecto INNER JOIN AdministradorProyecto ON Proyecto.Proyectoid = AdministradorProyecto.ProyectoID INNER JOIN ProyectoUsuarioIdentidad ON Proyecto.OrganizacionID = ProyectoUsuarioIdentidad.OrganizacionGnossID AND ProyectoUsuarioIdentidad.UsuarioID = AdministradorProyecto.UsuarioID INNER JOIN TipoDocDispRolUsuarioProy ON TipoDocDispRolUsuarioProy.ProyectoID = Proyecto.ProyectoID AND TipoDocDispRolUsuarioProy.OrganizacionID = Proyecto.OrganizacionID INNER JOIN TesauroProyecto ON Proyecto.ProyectoID = TesauroProyecto.ProyectoID INNER JOIN CategoriaTesauro ON TesauroProyecto.TesauroID = CategoriaTesauro.TesauroID WHERE (ProyectoUsuarioIdentidad.UsuarioID = " + IBD.GuidParamValor("usuarioID") + ") AND Proyecto.Estado = " + (short)EstadoProyecto.Definicion + " AND TipoDocDispRolUsuarioProy.RolUsuario = " + (short)TipoRolUsuario.Administrador + " AND TipoDocDispRolUsuarioProy.TipoDocumento = " + IBD.ToParam("tipoDocumento") + " AND AdministradorProyecto.UsuarioID = " + IBD.GuidParamValor("usuarioID");

            //Ojito con esta consulta (3er union)
            sqlSelectProyectoUsuarioIdentidadUsuarioPuedeCompartirRecursoSinComprobarTipoDoc = "SELECT " + IBD.CargarGuid("ProyectoUsuarioIdentidad.OrganizacionGnossID") + ", " + IBD.CargarGuid("ProyectoUsuarioIdentidad.ProyectoID") + ", " + IBD.CargarGuid("ProyectoUsuarioIdentidad.UsuarioID") + ", " + IBD.CargarGuid("ProyectoUsuarioIdentidad.IdentidadID") + ", FechaEntrada, Reputacion FROM PROYECTO INNER JOIN ProyectoUsuarioIdentidad ON Proyecto.OrganizacionID = ProyectoUsuarioIdentidad.OrganizacionGnossID AND Proyecto.ProyectoID = ProyectoUsuarioIdentidad.ProyectoID WHERE (ProyectoUsuarioIdentidad.UsuarioID = " + IBD.GuidParamValor("usuarioID") + ") AND Proyecto.Estado = " + (short)EstadoProyecto.Abierto + " UNION SELECT DISTINCT " + IBD.CargarGuid("ProyectoUsuarioIdentidad.OrganizacionGnossID") + ", " + IBD.CargarGuid("ProyectoUsuarioIdentidad.ProyectoID") + ", " + IBD.CargarGuid("ProyectoUsuarioIdentidad.UsuarioID") + ", " + IBD.CargarGuid("ProyectoUsuarioIdentidad.IdentidadID") + ", FechaEntrada, Reputacion FROM Proyecto INNER JOIN AdministradorProyecto ON Proyecto.Proyectoid = AdministradorProyecto.ProyectoID INNER JOIN ProyectoUsuarioIdentidad ON Proyecto.OrganizacionID = ProyectoUsuarioIdentidad.OrganizacionGnossID AND ProyectoUsuarioIdentidad.UsuarioID = AdministradorProyecto.UsuarioID INNER JOIN TipoDocDispRolUsuarioProy ON TipoDocDispRolUsuarioProy.ProyectoID = Proyecto.ProyectoID AND TipoDocDispRolUsuarioProy.OrganizacionID = Proyecto.OrganizacionID INNER JOIN TesauroProyecto ON Proyecto.ProyectoID = TesauroProyecto.ProyectoID INNER JOIN CategoriaTesauro ON TesauroProyecto.TesauroID = CategoriaTesauro.TesauroID WHERE (ProyectoUsuarioIdentidad.UsuarioID = " + IBD.GuidParamValor("usuarioID") + ") AND Proyecto.Estado = " + (short)EstadoProyecto.Definicion + " AND TipoDocDispRolUsuarioProy.RolUsuario = " + (short)TipoRolUsuario.Administrador + " AND AdministradorProyecto.UsuarioID = " + IBD.GuidParamValor("usuarioID");

            this.sqlSelectProyectosParticipaUsuario = SelectProyectoPesado.Replace("SELECT", "SELECT DISTINCT") + " FROM PROYECTO INNER JOIN ProyectoUsuarioIdentidad ON Proyecto.OrganizacionID = ProyectoUsuarioIdentidad.OrganizacionGnossID AND Proyecto.ProyectoID = ProyectoUsuarioIdentidad.ProyectoID WHERE (ProyectoUsuarioIdentidad.UsuarioID = " + IBD.GuidParamValor("usuarioID") + ") ORDER BY Proyecto.Nombre";

            this.sqlSelectProyectosParticipaUsuarioEnModoPersonal = SelectProyectoPesado.Replace("SELECT", "SELECT DISTINCT") + " FROM PROYECTO INNER JOIN ProyectoUsuarioIdentidad ON Proyecto.OrganizacionID = ProyectoUsuarioIdentidad.OrganizacionGnossID AND Proyecto.ProyectoID = ProyectoUsuarioIdentidad.ProyectoID INNER JOIN ProyectoRolUsuario ON (Proyecto.ProyectoID=ProyectoRolUsuario.ProyectoID AND Proyecto.OrganizacionID=ProyectoRolUsuario.OrganizacionGnossID) INNER JOIN Identidad ON Identidad.IdentidadID = ProyectoUsuarioIdentidad.IdentidadID WHERE (ProyectoUsuarioIdentidad.UsuarioID = " + IBD.GuidParamValor("usuarioID") + ") AND ( Proyecto.Estado = " + (short)EstadoProyecto.Abierto + " OR Proyecto.Estado = " + (short)EstadoProyecto.Cerrandose + " ) AND ProyectoRolUsuario.EstaBloqueado=0 AND (Identidad.Tipo < 2 OR Identidad.Tipo = 4) AND Identidad.FechaBaja IS NULL AND Identidad.FechaExpulsion IS NULL ORDER BY Proyecto.Nombre";

            this.sqlSelectProyectosParticipaUsuarioSinBloquear = SelectProyectoPesado.Replace("SELECT", "SELECT DISTINCT") + " FROM Proyecto INNER JOIN ProyectoUsuarioIdentidad ON Proyecto.OrganizacionID = ProyectoUsuarioIdentidad.OrganizacionGnossID AND Proyecto.ProyectoID = ProyectoUsuarioIdentidad.ProyectoID INNER JOIN ProyectoRolUsuario ON (Proyecto.ProyectoID=ProyectoRolUsuario.ProyectoID AND Proyecto.OrganizacionID=ProyectoRolUsuario.OrganizacionGnossID) WHERE (ProyectoUsuarioIdentidad.UsuarioID = " + IBD.GuidParamValor("usuarioID") + ") AND ProyectoRolUsuario.EstaBloqueado = 0 ORDER BY Proyecto.Nombre";

            this.sqlSelectProyectosParticipaUsuarioConPerfil = SelectProyectoPesado.Replace("SELECT", "SELECT DISTINCT") + " FROM Proyecto INNER JOIN ProyectoUsuarioIdentidad ON Proyecto.OrganizacionID = ProyectoUsuarioIdentidad.OrganizacionGnossID AND Proyecto.ProyectoID = ProyectoUsuarioIdentidad.ProyectoID INNER JOIN Identidad ON (ProyectoUsuarioIdentidad.IdentidadID=Identidad.IdentidadID) WHERE (ProyectoUsuarioIdentidad.UsuarioID = " + IBD.GuidParamValor("usuarioID") + ") AND (Identidad.PerfilID = " + IBD.GuidParamValor("perfilID") + ") AND (Proyecto.Estado = " + (short)EstadoProyecto.Abierto + " OR Proyecto.Estado = " + (short)EstadoProyecto.Cerrandose + ") ORDER BY Proyecto.Nombre";

            this.sqlSelectProyectoParticipaIdentidad = SelectProyectoLigero.Replace("SELECT", "SELECT DISTINCT") + " FROM Proyecto INNER JOIN ProyectoUsuarioIdentidad ON Proyecto.OrganizacionID = ProyectoUsuarioIdentidad.OrganizacionGnossID AND Proyecto.ProyectoID = ProyectoUsuarioIdentidad.ProyectoID INNER JOIN Identidad ON ProyectoUsuarioIdentidad.IdentidadID = Identidad.IdentidadID WHERE (ProyectoUsuarioIdentidad.IdentidadID = " + IBD.GuidParamValor("identidadID") + ") AND ( Proyecto.Estado = " + (short)EstadoProyecto.Abierto + " OR Proyecto.Estado = " + (short)EstadoProyecto.Cerrandose + " ) AND (Identidad.FechaBaja IS NULL)";

            this.sqlSelectProyectoUsuarioIdentidadParticipaUsuario = "SELECT " + IBD.CargarGuid("ProyectoUsuarioIdentidad.OrganizacionGnossID") + ", " + IBD.CargarGuid("ProyectoUsuarioIdentidad.ProyectoID") + ", " + IBD.CargarGuid("ProyectoUsuarioIdentidad.UsuarioID") + ", " + IBD.CargarGuid("ProyectoUsuarioIdentidad.IdentidadID") + ", FechaEntrada, Reputacion FROM Proyecto INNER JOIN ProyectoUsuarioIdentidad ON Proyecto.OrganizacionID = ProyectoUsuarioIdentidad.OrganizacionGnossID AND Proyecto.ProyectoID = ProyectoUsuarioIdentidad.ProyectoID WHERE (ProyectoUsuarioIdentidad.UsuarioID = " + IBD.GuidParamValor("usuarioID") + ") ORDER BY Proyecto.Nombre";

            this.sqlSelectProyectoUsuarioIdentidadParticipaUsuarioEnModoPersonal = "SELECT " + IBD.CargarGuid("ProyectoUsuarioIdentidad.OrganizacionGnossID") + ", " + IBD.CargarGuid("ProyectoUsuarioIdentidad.ProyectoID") + ", " + IBD.CargarGuid("ProyectoUsuarioIdentidad.UsuarioID") + ", " + IBD.CargarGuid("ProyectoUsuarioIdentidad.IdentidadID") + ", FechaEntrada, Reputacion FROM Proyecto INNER JOIN ProyectoUsuarioIdentidad ON Proyecto.OrganizacionID = ProyectoUsuarioIdentidad.OrganizacionGnossID AND Proyecto.ProyectoID = ProyectoUsuarioIdentidad.ProyectoID INNER JOIN ProyectoRolUsuario ON (Proyecto.ProyectoID = ProyectoRolUsuario.ProyectoID AND Proyecto.OrganizacionID = ProyectoRolUsuario.OrganizacionGnossID) INNER JOIN Identidad ON Identidad.IdentidadID = ProyectoUsuarioIdentidad.IdentidadID WHERE (ProyectoUsuarioIdentidad.UsuarioID = " + IBD.GuidParamValor("usuarioID") + ") AND ( Proyecto.Estado = " + (short)EstadoProyecto.Abierto + " OR Proyecto.Estado = " + (short)EstadoProyecto.Cerrandose + " ) AND ProyectoRolUsuario.EstaBloqueado = 0 AND (Identidad.Tipo < 2 OR Identidad.Tipo = 4) AND Identidad.FechaBaja IS NULL AND Identidad.FechaExpulsion IS NULL ORDER BY Proyecto.Nombre";

            this.sqlSelectProyectoUsuarioIdentidadParticipaUsuarioSinBloquear = "SELECT DISTINCT " + IBD.CargarGuid("ProyectoUsuarioIdentidad.OrganizacionGnossID") + ", " + IBD.CargarGuid("ProyectoUsuarioIdentidad.ProyectoID") + ", " + IBD.CargarGuid("ProyectoUsuarioIdentidad.UsuarioID") + ", " + IBD.CargarGuid("ProyectoUsuarioIdentidad.IdentidadID") + ", FechaEntrada, Reputacion FROM Proyecto INNER JOIN ProyectoUsuarioIdentidad ON Proyecto.OrganizacionID = ProyectoUsuarioIdentidad.OrganizacionGnossID AND Proyecto.ProyectoID = ProyectoUsuarioIdentidad.ProyectoID INNER JOIN ProyectoRolUsuario ON (Proyecto.ProyectoID = ProyectoRolUsuario.ProyectoID AND Proyecto.OrganizacionID = ProyectoRolUsuario.OrganizacionGnossID) WHERE (ProyectoUsuarioIdentidad.UsuarioID = " + IBD.GuidParamValor("usuarioID") + ") AND ProyectoRolUsuario.EstaBloqueado = 0"; //ORDER BY Proyecto.Nombre";

            // David: A la siguiente consulta se le han añadido los proyectos de los que el usuario es administrador (sin tener cuenta el estado de este)
            this.sqlSelectProyectosParticipaPerfilUsuario = SelectProyectoPesado + " FROM Proyecto WHERE Proyecto.ProyectoID IN (SELECT DISTINCT Proyecto.ProyectoID FROM Proyecto INNER JOIN Identidad ON Proyecto.OrganizacionID = Identidad.OrganizacionID AND Proyecto.ProyectoID = Identidad.ProyectoID WHERE (Identidad.PerfilID = " + IBD.GuidParamValor("perfilID") + ") AND Proyecto.Estado <> " + (short)EstadoProyecto.Cerrado + " AND Proyecto.Estado <> " + (short)EstadoProyecto.CerradoTemporalmente + " AND Proyecto.Estado <> " + (short)EstadoProyecto.Definicion + " AND Identidad.FechaBaja IS NULL UNION SELECT DISTINCT Proyecto.ProyectoID FROM Proyecto INNER JOIN Identidad ON Proyecto.OrganizacionID = Identidad.OrganizacionID AND Proyecto.ProyectoID = Identidad.ProyectoID INNER JOIN AdministradorProyecto ON Proyecto.OrganizacionID = AdministradorProyecto.OrganizacionID AND Proyecto.ProyectoID = AdministradorProyecto.ProyectoID WHERE (Identidad.PerfilID = " + IBD.GuidParamValor("perfilID") + ") AND Identidad.FechaBaja IS NULL AND Proyecto.TipoProyecto != " + (short)TipoProyecto.MetaComunidad + " AND AdministradorProyecto.UsuarioID=" + IBD.GuidParamValor("UsuarioID") + " AND AdministradorProyecto.Tipo=" + (short)TipoRolUsuario.Administrador + ") ORDER BY Nombre";

            this.sqlSelectProyectosParticipaPerfilUsuarioSinMyGNOSS = SelectProyectoPesado + " FROM Proyecto WHERE Proyecto.ProyectoID IN(SELECT DISTINCT Proyecto.ProyectoID FROM PROYECTO INNER JOIN Identidad ON Proyecto.OrganizacionID = Identidad.OrganizacionID AND Proyecto.ProyectoID = Identidad.ProyectoID WHERE (Identidad.PerfilID = " + IBD.GuidParamValor("perfilID") + ") AND Proyecto.Estado <> " + (short)EstadoProyecto.Cerrado + " AND Proyecto.Estado <> " + (short)EstadoProyecto.CerradoTemporalmente + " AND Proyecto.Estado <> " + (short)EstadoProyecto.Definicion + " AND Identidad.FechaBaja is null AND Proyecto.TipoProyecto != " + (short)TipoProyecto.MetaComunidad + " UNION SELECT DISTINCT Proyecto.ProyectoID FROM Proyecto INNER JOIN Identidad ON Proyecto.OrganizacionID = Identidad.OrganizacionID AND Proyecto.ProyectoID = Identidad.ProyectoID INNER JOIN AdministradorProyecto ON PROYECTO.OrganizacionID = AdministradorProyecto.OrganizacionID AND PROYECTO.ProyectoID = AdministradorProyecto.ProyectoID WHERE (Identidad.PerfilID = " + IBD.GuidParamValor("perfilID") + ") AND Identidad.FechaBaja IS NULL AND Proyecto.TipoProyecto != " + (short)TipoProyecto.MetaComunidad + " AND AdministradorProyecto.UsuarioID=" + IBD.GuidParamValor("UsuarioID") + " AND AdministradorProyecto.Tipo=" + (short)TipoRolUsuario.Administrador + ") ORDER BY Proyecto.Nombre";

            this.sqlSelectProyectosOrganizacionParticipaUsuario = SelectProyectoPesado + " FROM Proyecto INNER JOIN ProyectoRolUsuario ON Proyecto.OrganizacionID = ProyectoRolUsuario.OrganizacionGnossID AND Proyecto.ProyectoID = ProyectoRolUsuario.ProyectoID WHERE (ProyectoRolUsuario.UsuarioID = " + IBD.GuidParamValor("usuarioID") + ") AND (Proyecto.OrganizacionID = " + IBD.GuidParamValor("organizacionID") + ") AND ( Proyecto.Estado = " + (short)EstadoProyecto.Abierto + " OR Proyecto.Estado = " + (short)EstadoProyecto.Cerrandose + " OR Proyecto.Estado = " + (short)EstadoProyecto.Definicion + ") AND ProyectoRolUsuario.EstaBloqueado = 0 ORDER BY Nombre";

            this.sqlSelectProyectoInicioSesionDeUsuario = SelectProyectoPesado + " FROM Proyecto INNER JOIN InicioSesion ON Proyecto.OrganizacionID = InicioSesion.OrganizacionGnossID AND Proyecto.ProyectoID = InicioSesion.ProyectoID WHERE (InicioSesion.UsuarioID = " + IBD.GuidParamValor("usuarioID") + ")";

            this.sqlSelectProyectosPuedoCompartirDefinicion = SelectProyectoPesado.Replace("SELECT", "SELECT DISTINCT ") + " FROM Proyecto INNER JOIN ParametroGeneral g ON Proyecto.ProyectoID = g.ProyectoID INNER JOIN ProyectoRolUsuario r ON Proyecto.ProyectoID = r.ProyectoID WHERE r.UsuarioID = " + IBD.GuidParamValor("usuarioID") + " AND g.WikiDisponible = 'true' AND Proyecto.ProyectoID!=" + IBD.GuidParamValor("proyectoID") + " AND Proyecto.ProyectoID NOT IN (SELECT DISTINCT w2.ProyectoID FROM WikiGnossProyecto w2 INNER JOIN Definicion d2 ON w2.WikiGnossID = d2.WikiGnossID WHERE d2.Nombre=" + IBD.GuidParamValor("definicion") + ")";

            this.sqlSelectAdministradoresProyecto = SelectAdministradorProyecto + " FROM AdministradorProyecto ";

            this.sqlSelectAdministradoresProyectoPorID = SelectAdministradorProyecto + " FROM AdministradorProyecto WHERE (ProyectoID = " + IBD.GuidParamValor("proyectoID") + ") ";

            this.sqlSelectAdministradoresGrupoProyecto = SelectAdministradorGrupoProyecto + " FROM AdministradorGrupoProyecto ";

            this.sqlSelectAdministradoresGrupoProyectoPorID = SelectAdministradorGrupoProyecto + " FROM AdministradorGrupoProyecto WHERE (ProyectoID = " + IBD.GuidParamValor("proyectoID") + ") ";

            this.sqlSelectAdministradoresProyectoPorProyIDYUsuarioID = SelectAdministradorProyecto + " FROM AdministradorProyecto WHERE (ProyectoID = " + IBD.GuidParamValor("ProyectoID") + ") AND (UsuarioID = " + IBD.GuidParamValor("UsuarioID") + ")";


            this.sqlSelectProyectoAgCatTesauroPorID = "SELECT " + IBD.CargarGuid("ProyectoAgCatTesauro.OrganizacionID") + ", " + IBD.CargarGuid("ProyectoAgCatTesauro.ProyectoID") + ", " + IBD.CargarGuid("ProyectoAgCatTesauro.TesauroID") + ", " + IBD.CargarGuid("ProyectoAgCatTesauro.CategoriaTesauroID") + " FROM ProyectoAgCatTesauro INNER JOIN Proyecto ON ProyectoAgCatTesauro.ProyectoID = Proyecto.ProyectoID AND ProyectoAgCatTesauro.OrganizacionID = Proyecto.OrganizacionID WHERE Proyecto.ProyectoID = " + IBD.GuidParamValor("proyectoID");

            this.sqlSelectProyectosPorCategoria = SelectProyectoPesado.Replace("SELECT", "SELECT DISTINCT") + " FROM Proyecto INNER JOIN ProyectoAgCatTesauro ON Proyecto.ProyectoID = ProyectoAgCatTesauro.ProyectoID INNER JOIN CategoriaTesauro ON ProyectoAgCatTesauro.CategoriaTesauroID = CategoriaTesauro.CategoriaTesauroID WHERE CategoriaTesauro.CategoriaTesauroID = " + IBD.GuidParamValor("pCategoria") + " AND Proyecto.TipoAcceso != " + (short)TipoAcceso.Reservado + " AND Proyecto.Estado != " + (short)EstadoProyecto.Cerrado + " AND Proyecto.Estado != " + (short)EstadoProyecto.CerradoTemporalmente + " AND Proyecto.Estado != " + (short)EstadoProyecto.Definicion + " ";

            this.sqlSelectProyectosPorID = SelectProyectoPesado + " FROM Proyecto WHERE ";

            this.sqlSelectProyectosDestacados = SelectProyectoPesado + ", (((Proyecto.NumeroRecursos + Proyecto.NumeroPreguntas + Proyecto.NumeroDebates) * 0.05) + (Proyecto.NumeroMiembros * 0.2) + (Proyecto.NumeroOrgRegistradas * 0.4) + (Proyecto.NumeroArticulos * 0.05) + (Proyecto.NumeroDafos * 0.2) + (Proyecto.NumeroForos * 0.1)) as ActividadMedia FROM Proyecto WHERE Proyecto.TipoProyecto = " + ((short)TipoAcceso.Publico).ToString();

            this.sqlSelectUltimosProyectos = SelectProyectoPesado + " FROM Proyecto";

            this.sqlSelectProyectosPorPeso = "SELECT " + IBD.CargarGuid("ProyectoID") + ", Nombre,  CAST(((Proyecto.NumeroRecursos + Proyecto.NumeroPreguntas + Proyecto.NumeroDebates) * 0.1)+(NumeroMiembros * 0.3)+(NumeroOrgRegistradas * 0.2)+(NumeroArticulos * 0.1)+(NumeroDafos * 0.15)+(NumeroForos * 0.15)as int) AS Peso FROM Proyecto WHERE TipoAcceso = 0 OR TipoAcceso = 2 ORDER BY Peso DESC";

            this.sqlSelectProyectosPublicosPrimerNivel = SelectProyectoPesado + " FROM Proyecto WHERE Proyecto.TipoAcceso != " + (short)TipoAcceso.Reservado + " AND Proyecto.TipoProyecto != " + (short)TipoProyecto.MetaComunidad + " AND ProyectoSuperiorID IS NULL AND Proyecto.Estado != " + (short)EstadoProyecto.Cerrado + " AND Proyecto.Estado != " + (short)EstadoProyecto.CerradoTemporalmente + " ";

            this.sqlSelectComunidadesMasActivas = "SELECT " + IBD.CargarGuid("ProyectoID") + ", Nombre, NumeroConsultas, Peso FROM ProyectosMasActivos ORDER BY Peso DESC";

            this.sqlSelectProyectosParticipaUsuarioDeLaOrganizacion = SelectProyectoLigero + " FROM Proyecto WHERE Proyecto.ProyectoID IN ( SELECT ProyectoUsuarioIdentidad.ProyectoID FROM ProyectoUsuarioIdentidad INNER JOIN Identidad ON Identidad.IdentidadID =  ProyectoUsuarioIdentidad.IdentidadID INNER JOIN PerfilPersonaOrg ON PerfilPersonaOrg.PerfilID = Identidad.PerfilID WHERE ProyectoUsuarioIdentidad.UsuarioID = " + IBD.GuidParamValor("usuarioID") + " AND PerfilPersonaOrg.OrganizacionID = " + IBD.GuidParamValor("organizacionID") + " AND ProyectoUsuarioIdentidad.ProyectoID <> '" + MetaProyecto + "' )";

            this.sqlSelectAdministradorProyectoParticipaUsuarioDeLaOrganizacion = SelectAdministradorProyecto + " FROM AdministradorProyecto WHERE AdministradorProyecto.ProyectoID IN (SELECT ProyectoUsuarioIdentidad.ProyectoID FROM ProyectoUsuarioIdentidad INNER JOIN Identidad ON Identidad.IdentidadID = ProyectoUsuarioIdentidad.IdentidadID INNER JOIN PerfilPersonaOrg ON PerfilPersonaOrg.PerfilID = Identidad.PerfilID WHERE ProyectoUsuarioIdentidad.UsuarioID = " + IBD.GuidParamValor("usuarioID") + " AND PerfilPersonaOrg.OrganizacionID = " + IBD.GuidParamValor("organizacionID") + " AND ProyectoUsuarioIdentidad.ProyectoID <> '" + MetaProyecto + "' )";

            this.sqlSelectProyectosParticipaOrganizacion = SelectProyectoPesado + " FROM Proyecto INNER JOIN OrganizacionParticipaProy ON Proyecto.ProyectoID = OrganizacionParticipaProy.ProyectoID AND Proyecto.OrganizacionID = OrganizacionParticipaProy.OrganizacionProyectoID WHERE OrganizacionParticipaProy.OrganizacionID = " + IBD.GuidParamValor("organizacionID") + "  AND Proyecto.Estado <> " + (short)EstadoProyecto.Cerrado + " AND Proyecto.Estado <> " + (short)EstadoProyecto.CerradoTemporalmente;

            this.sqlSelectProyectosParticipaOrganizacionPaginado = SelectProyectoPesado.Replace("SELECT", "SELECT DISTINCT") + ", 0 Administrado FROM Proyecto INNER JOIN OrganizacionParticipaProy ON Proyecto.ProyectoID = OrganizacionParticipaProy.ProyectoID AND Proyecto.OrganizacionID = OrganizacionParticipaProy.OrganizacionProyectoID INNER JOIN AdministradorProyecto ON Proyecto.ProyectoID = AdministradorProyecto.ProyectoID AND Proyecto.OrganizacionID = AdministradorProyecto.OrganizacionID WHERE OrganizacionParticipaProy.OrganizacionID = " + IBD.GuidParamValor("organizacionID") + "  AND Proyecto.Estado <> " + (short)EstadoProyecto.Cerrado + " AND Proyecto.Estado <> " + (short)EstadoProyecto.CerradoTemporalmente + " AND AdministradorProyecto.UsuarioID IN (SELECT Persona.UsuarioID FROM  Persona	INNER JOIN Perfil ON Persona.PersonaID = Perfil.PersonaID WHERE Perfil.OrganizacionID = " + IBD.GuidParamValor("organizacionID") + ")  AND Proyecto.ProyectoID <> '" + IBD.ValorDeGuid(ProyectoAD.MetaProyecto) + "' UNION ALL " + SelectProyectoPesado.Replace("SELECT", "SELECT DISTINCT") + ", 1 Administrado FROM Proyecto INNER JOIN OrganizacionParticipaProy ON Proyecto.ProyectoID = OrganizacionParticipaProy.ProyectoID AND Proyecto.OrganizacionID = OrganizacionParticipaProy.OrganizacionProyectoID WHERE OrganizacionParticipaProy.OrganizacionID = " + IBD.GuidParamValor("organizacionID") + "  AND Proyecto.Estado <> " + (short)EstadoProyecto.Cerrado + " AND Proyecto.Estado <> " + (short)EstadoProyecto.CerradoTemporalmente + " AND Proyecto.ProyectoID NOT IN (SELECT AdministradorProyecto.ProyectoID FROM  Persona INNER JOIN Perfil ON Persona.PersonaID = Perfil.PersonaID INNER JOIN AdministradorProyecto ON AdministradorProyecto.UsuarioID = Persona.UsuarioID WHERE Perfil.OrganizacionID = " + IBD.GuidParamValor("organizacionID") + ")  AND Proyecto.ProyectoID <> '" + IBD.ValorDeGuid(ProyectoAD.MetaProyecto) + "'";

            this.sqlSelectProyectosParticipaOrganizacionSinMyGNOSS = SelectProyectoPesado + " FROM Proyecto INNER JOIN OrganizacionParticipaProy ON Proyecto.ProyectoID = OrganizacionParticipaProy.ProyectoID AND Proyecto.OrganizacionID = OrganizacionParticipaProy.OrganizacionProyectoID WHERE OrganizacionParticipaProy.OrganizacionID = " + IBD.GuidParamValor("organizacionID") + " AND Proyecto.ProyectoID <> " + IBD.GuidValor(ProyectoAD.MetaProyecto) + "  AND Proyecto.Estado <> " + (short)EstadoProyecto.Cerrado + " AND Proyecto.Estado <> " + (short)EstadoProyecto.CerradoTemporalmente;

            this.sqlSelectProyectosParticipaUsuarioConSuPerfilPersonal = SelectProyectoLigero + " FROM Proyecto WHERE Proyecto.ProyectoID IN (SELECT ProyectoUsuarioIdentidad.ProyectoID FROM ProyectoUsuarioIdentidad INNER JOIN Identidad ON Identidad.IdentidadID =  ProyectoUsuarioIdentidad.IdentidadID INNER JOIN PerfilPersona ON PerfilPersona.PerfilID = Identidad.PerfilID WHERE ProyectoUsuarioIdentidad.UsuarioID = " + IBD.GuidParamValor("usuarioID") + " AND ProyectoUsuarioIdentidad.ProyectoID <> '" + MetaProyecto + "' )";

            this.sqlSelectAdministradorProyectoParticipaUsuarioConSuPerfilPersonal = SelectAdministradorProyecto + " FROM AdministradorProyecto WHERE AdministradorProyecto.ProyectoID IN (SELECT ProyectoUsuarioIdentidad.ProyectoID FROM ProyectoUsuarioIdentidad INNER JOIN Identidad ON Identidad.IdentidadID =  ProyectoUsuarioIdentidad.IdentidadID INNER JOIN PerfilPersona ON PerfilPersona.PerfilID = Identidad.PerfilID WHERE ProyectoUsuarioIdentidad.UsuarioID = " + IBD.GuidParamValor("usuarioID") + " AND ProyectoUsuarioIdentidad.ProyectoID <> '" + MetaProyecto + "' )";

            this.sqlSelectProyectoPorID = SelectProyectoPesado + " FROM Proyecto WHERE (ProyectoID = " + IBD.GuidParamValor("proyectoID") + ") ";
            this.sqlSelectProyectosOrganizacionCargaLigera = SelectProyectoLigero + " FROM Proyecto WHERE (OrganizacionID = " + IBD.GuidParamValor("OrganizacionID") + ") ORDER BY Nombre";

            this.sqlSelectExisteNombreCortoEnBD = "SELECT " + IBD.CargarGuid("ProyectoID") + " FROM Proyecto WHERE (UPPER(NombreCorto) = UPPER(" + IBD.ToParam("nombreCorto") + "))";

            this.sqlSelectExisteNombreEnBD = "SELECT " + IBD.CargarGuid("ProyectoID") + " FROM Proyecto WHERE (UPPER(Nombre) = UPPER(" + IBD.ToParam("nombre") + "))";

            this.sqlUpdateAumentarNumeroMiembrosDelProyecto = "UPDATE Proyecto SET NumeroMiembros = NumeroMiembros + 1 WHERE ProyectoID = " + IBD.GuidParamValor("proyectoID");

            this.sqlUpdateDisminuirNumeroMiembrosDelProyecto = "UPDATE Proyecto SET NumeroMiembros = NumeroMiembros - 1 WHERE ProyectoID = " + IBD.GuidParamValor("proyectoID");

            this.sqlUpdateDisminuirNumeroOrParticipanEnProyecto = "UPDATE Proyecto SET NumeroOrgRegistradas = NumeroOrgRegistradas - 1 WHERE ProyectoID = " + IBD.GuidParamValor("proyectoID");

            this.sqlUpdateAumentarNumeroOrgParticipanEnProyecto = "UPDATE Proyecto SET NumeroOrgRegistradas = NumeroOrgRegistradas + 1 WHERE ProyectoID = " + IBD.GuidParamValor("proyectoID");

            this.sqlSelectUsuariosExceptoLosAdministradores = "SELECT " + IBD.CargarGuid("UsuarioID") + " FROM ProyectoUsuarioIdentidad WHERE proyectoID = " + IBD.GuidParamValor("proyectoID") + " AND ProyectoUsuarioIdentidad.UsuarioID NOT IN (SELECT UsuarioID FROM AdministradorProyecto WHERE ProyectoID = " + IBD.GuidParamValor("proyectoID") + " AND Tipo = " + (short)TipoRolUsuario.Administrador + ")";

            this.sqlSelectEntidadesGnossExceptoLosGestores = "SELECT " + IBD.CargarGuid("EntidadGnossID") + " FROM EntidadGnoss WHERE proyectoID = " + IBD.GuidParamValor("proyectoID") + " AND EntidadGnossID NOT IN (SELECT GrupoID as EntidadGnossID FROM GestorGrupo WHERE proyectoID = " + IBD.GuidParamValor("proyectoID") + " UNION SELECT ElementoEstructuraID as EntidadGnossID FROM MetaEstructura WHERE proyectoID = " + IBD.GuidParamValor("proyectoID") + ")";

            this.sqlSelectTieneCategoriasDeTesauro = "SELECT " + IBD.CargarGuid("CategoriaTesauroID") + " FROM CategoriaTesauro INNER JOIN TesauroProyecto ON TesauroProyecto.TesauroID = CategoriaTesauro.TesauroID WHERE TesauroProyecto.ProyectoID = " + IBD.GuidParamValor("proyectoID");

            this.sqlSelectTieneSolicitudesCategoriasDeTesauro = "SELECT " + IBD.CargarGuid("SugerenciaID") + " FROM CategoriaTesauroSugerencia INNER JOIN TesauroProyecto ON CategoriaTesauroSugerencia.TesauroSugerenciaID = TesauroProyecto.TesauroID WHERE TesauroProyecto.ProyectoID = " + IBD.GuidParamValor("proyectoID");

            this.sqlSelectTieneDefiniciones = "SELECT " + IBD.CargarGuid("DefinicionID") + " FROM Definicion INNER JOIN WikiGnossProyecto ON WikiGnossProyecto.WikiGnossID = Definicion.WikiGnossID WHERE proyectoID = " + IBD.GuidParamValor("proyectoID");

            this.sqlSelectNombreDeProyecto = "SELECT Nombre FROM Proyecto WHERE ProyectoID = " + IBD.GuidParamValor("pProyectoID");

            this.sqlSelectNivelCertificacionPorID = sqlSelectNivelCertificacion + " WHERE NivelCertificacion.ProyectoID = " + IBD.GuidParamValor("proyectoID");

            this.sqlSelectTipoDocDispRolUsuarioProyPorID = sqlSelectTipoDocDispRolUsuarioProy + " WHERE TipoDocDispRolUsuarioProy.ProyectoID = " + IBD.GuidParamValor("proyectoID");

            this.sqlSelectProyectoCerradoTmpPorID = sqlSelectProyectoCerradoTmp + " FROM ProyectoCerradoTmp WHERE proyectoID = " + IBD.GuidParamValor("proyectoID");

            this.sqlSelectProyectoCerrandosePorID = sqlSelectProyectoCerrandose + " FROM ProyectoCerrandose WHERE proyectoID = " + IBD.GuidParamValor("proyectoID");

            this.sqlSelectEmailsMiembrosDeProyecto = "SELECT ProyectoUsuarioIdentidad.IdentidadID,Perfil.PersonaID,Persona.Nombre, Persona.Email FROM proyectoUsuarioIdentidad INNER JOIN Identidad ON proyectoUsuarioIdentidad.IdentidadID = Identidad.identidadID INNER JOIN Perfil ON Perfil.PerfilID = Identidad.PerfilID INNER JOIN Persona ON Persona.PersonaID = Perfil.PersonaID WHERE ProyectoUsuarioIdentidad.ProyectoID = " + IBD.GuidParamValor("proyectoID") + " AND Identidad.FechaBaja IS NULL AND Perfil.Eliminado = 0 AND Perfil.OrganizacionID IS NULL AND Persona.Email IS NOT NULL  UNION  Select proyectoUsuarioIdentidad.IdentidadID,Perfil.PersonaID,Persona.Nombre, PersonaVinculoOrganizacion.EmailTrabajo as Email FROM proyectoUsuarioIdentidad INNER JOIN Identidad ON proyectoUsuarioIdentidad.IdentidadID = Identidad.IdentidadID INNER JOIN Perfil ON Perfil.PerfilID = Identidad.PerfilID INNER JOIN Persona ON Persona.personaID = Perfil.PersonaID INNER JOIN PersonaVinculoOrganizacion ON PersonaVinculoOrganizacion.PersonaID = Persona.PersonaID AND PersonaVinculoOrganizacion.OrganizacionID = Perfil.OrganizacionID WHERE ProyectoUsuarioIdentidad.ProyectoID = " + IBD.GuidParamValor("proyectoID") + " AND Identidad.FechaBaja is null AND Perfil.Eliminado = 0 AND Perfil.PersonaID IS NOT NULL AND Perfil.OrganizacionID IS NOT NULL AND PersonaVinculoOrganizacion.EmailTrabajo IS NOT NULL ";

            this.sqlSelectEmailsMiembrosDeEventoDeProyecto = "SELECT ProyectoUsuarioIdentidad.IdentidadID,Perfil.PersonaID,Persona.Nombre,Persona.Apellidos, Persona.Email FROM proyectoUsuarioIdentidad INNER JOIN Identidad ON proyectoUsuarioIdentidad.IdentidadID = Identidad.identidadID INNER JOIN Perfil ON Perfil.PerfilID = Identidad.PerfilID INNER JOIN Persona ON Persona.PersonaID = Perfil.PersonaID INNER JOIN ProyectoeventoParticipante on ProyectoeventoParticipante.identidadid=Identidad.identidadid WHERE ProyectoeventoParticipante.EventoID = " + IBD.GuidParamValor("eventoID") + " AND Perfil.OrganizacionID IS NULL AND Persona.Email IS NOT NULL  UNION  Select proyectoUsuarioIdentidad.IdentidadID,Perfil.PersonaID,Persona.Nombre,Persona.Apellidos, PersonaVinculoOrganizacion.EmailTrabajo as Email FROM proyectoUsuarioIdentidad INNER JOIN Identidad ON proyectoUsuarioIdentidad.IdentidadID = Identidad.IdentidadID INNER JOIN Perfil ON Perfil.PerfilID = Identidad.PerfilID INNER JOIN Persona ON Persona.personaID = Perfil.PersonaID INNER JOIN PersonaVinculoOrganizacion ON PersonaVinculoOrganizacion.PersonaID = Persona.PersonaID AND PersonaVinculoOrganizacion.OrganizacionID = Perfil.OrganizacionID INNER JOIN ProyectoeventoParticipante on ProyectoeventoParticipante.identidadid=Identidad.identidadid WHERE ProyectoeventoParticipante.EventoID = " + IBD.GuidParamValor("eventoID") + " AND Perfil.PersonaID IS NOT NULL AND Perfil.OrganizacionID IS NOT NULL AND PersonaVinculoOrganizacion.EmailTrabajo IS NOT NULL ";

            this.sqlSelectEmailsAdministradoresDeProyecto = "SELECT DISTINCT ProyectoUsuarioIdentidad.IdentidadID,Perfil.PersonaID,Persona.Nombre, Persona.Email FROM proyectoUsuarioIdentidad INNER JOIN Identidad ON proyectoUsuarioIdentidad.IdentidadID = Identidad.identidadID INNER JOIN Perfil ON Perfil.PerfilID = Identidad.PerfilID INNER JOIN Persona ON Persona.PersonaID = Perfil.PersonaID INNER JOIN AdministradorProyecto ON AdministradorProyecto.UsuarioID = proyectoUsuarioIdentidad.UsuarioID WHERE ProyectoUsuarioIdentidad.ProyectoID = " + IBD.GuidParamValor("proyectoID") + " AND Identidad.FechaBaja IS NULL AND Perfil.Eliminado = 0 AND Perfil.OrganizacionID IS NULL AND Persona.Email IS NOT NULL  UNION  Select DISTINCT ProyectoUsuarioIdentidad.IdentidadID,Perfil.PersonaID,Persona.Nombre, PersonaVinculoOrganizacion.EmailTrabajo as Email FROM proyectoUsuarioIdentidad INNER JOIN Identidad ON proyectoUsuarioIdentidad.IdentidadID = Identidad.IdentidadID INNER JOIN Perfil ON Perfil.PerfilID = Identidad.PerfilID INNER JOIN Persona ON Persona.PersonaID = Perfil.PersonaID INNER JOIN PersonaVinculoOrganizacion ON PersonaVinculoOrganizacion.PersonaID = Persona.PersonaID AND PersonaVinculoOrganizacion.OrganizacionID = Perfil.OrganizacionID INNER JOIN AdministradorProyecto ON AdministradorProyecto.UsuarioID = ProyectoUsuarioIdentidad.UsuarioID WHERE ProyectoUsuarioIdentidad.ProyectoID = " + IBD.GuidParamValor("proyectoID") + " AND Identidad.FechaBaja is null AND Perfil.Eliminado = 0 AND Perfil.PersonaID IS NOT NULL AND Perfil.OrganizacionID IS NOT NULL AND PersonaVinculoOrganizacion.EmailTrabajo IS NOT NULL ";

            sqlSelectTablaBaseProyectoIDDeProyectoPorID = "SELECT TablaBaseProyectoID FROM Proyecto WHERE ProyectoID = " + IBD.GuidParamValor("proyectoID");

            this.sqlSelectTodosIDProyectosConIDNumerico = "SELECT " + IBD.CargarGuid("OrganizacionID") + ", " + IBD.CargarGuid("ProyectoID") + ", TablaBaseProyectoID, TipoAcceso FROM Proyecto";

            this.sqlSelectProyTipoRecNoActivReciente = "SELECT " + IBD.CargarGuid("ProyectoID") + ", TipoRecurso, OntologiasID FROM ProyTipoRecNoActivReciente";

            this.sqlSelectTipoDocImagenPorDefecto = "SELECT " + IBD.CargarGuid("ProyectoID") + ", TipoRecurso, " + IBD.CargarGuid("OntologiaID") + ", UrlImagen FROM TipoDocImagenPorDefecto";

            this.sqlSelectProyectoGrafoFichaRec = "SELECT " + IBD.CargarGuid("OrganizacionID") + ", " + IBD.CargarGuid("ProyectoID") + ", " + IBD.CargarGuid("GrafoID") + ", PropEnlace, NodosLimiteNivel, Extra FROM ProyectoGrafoFichaRec";

            this.sqlSelectProyectoGadget = "SELECT " + IBD.CargarGuid("ProyectoGadget.OrganizacionID") + ", " + IBD.CargarGuid("ProyectoGadget.ProyectoID") + ", " + IBD.CargarGuid("ProyectoGadget.GadgetID") + ", ProyectoGadget.Titulo, ProyectoGadget.Contenido, ProyectoGadget.Orden, ProyectoGadget.Tipo, ProyectoGadget.Ubicacion,ProyectoGadget.Clases, ProyectoGadget.TipoUbicacion, ProyectoGadget.Visible , ProyectoGadget.MultiIdioma,ProyectoGadget.PersonalizacionComponenteID, ProyectoGadget.CargarPorAjax, ProyectoGadget.ComunidadDestinoFiltros, ProyectoGadget.NombreCorto FROM ProyectoGadget";

            this.sqlSelectProyectoGadgetContexto = "SELECT " + IBD.CargarGuid("ProyectoGadgetContexto.OrganizacionID") + ", " + IBD.CargarGuid("ProyectoGadgetContexto.ProyectoID") + ", " + IBD.CargarGuid("ProyectoGadgetContexto.GadgetID") + ", ProyectoGadgetContexto.ComunidadOrigen, ProyectoGadgetContexto.ComunidadOrigenFiltros, ProyectoGadgetContexto.FiltrosOrigenDestino, ProyectoGadgetContexto.OrdenContexto,ProyectoGadgetContexto.Imagen, " + IBD.CargarGuid("ProyectoGadgetContexto.ProyectoOrigenID") + " ,ProyectoGadgetContexto.NumRecursos, ProyectoGadgetContexto.ServicioResultados, ProyectoGadgetContexto.MostrarEnlaceOriginal, ProyectoGadgetContexto.OcultarVerMas, ProyectoGadgetContexto.NamespacesExtra, ProyectoGadgetContexto.ItemsBusqueda, ProyectoGadgetContexto.ResultadosEliminar,ProyectoGadgetContexto.NuevaPestanya,ProyectoGadgetContexto.ObtenerPrivados FROM ProyectoGadgetContexto";

            //this.sqlSelectProyectoGadgetContextoHTMLplano = "SELECT " + IBD.CargarGuid("ProyectoGadgetContextoHTMLplano.OrganizacionID") + ", " + IBD.CargarGuid("ProyectoGadgetContextoHTMLplano.ProyectoID") + ", " + IBD.CargarGuid("ProyectoGadgetContextoHTMLplano.GadgetID") + ", ProyectoGadgetContextoHTMLplano.ComunidadDestinoFiltros FROM ProyectoGadgetContextoHTMLplano";

            this.sqlSelectExisteNombreCortoGadgetEnBD = "SELECT " + IBD.CargarGuid("GadgetID") + " FROM ProyectoGadget WHERE (UPPER(NombreCorto) = UPPER(" + IBD.ToParam("NombreCorto") + "))";

            this.sqlSelectProyectoGadgetIdioma = "SELECT " + IBD.CargarGuid("ProyectoGadgetIdioma.OrganizacionID") + ", " + IBD.CargarGuid("ProyectoGadgetIdioma.ProyectoID") + ", " + IBD.CargarGuid("ProyectoGadgetIdioma.GadgetID") + ", ProyectoGadgetIdioma.Idioma, ProyectoGadgetIdioma.Contenido FROM ProyectoGadgetIdioma";

            this.sqlSelectProyectoPestanyaFiltroOrdenRecursos = "SELECT " + IBD.CargarGuid("ProyectoPestanyaFiltroOrdenRecursos.PestanyaID") + ", ProyectoPestanyaFiltroOrdenRecursos.FiltroOrden, ProyectoPestanyaFiltroOrdenRecursos.NombreFiltro, ProyectoPestanyaFiltroOrdenRecursos.Orden FROM ProyectoPestanyaFiltroOrdenRecursos";

            this.sqlSelectRecursosRelacionadosPresentacion = "SELECT " + IBD.CargarGuid("RecursosRelacionadosPresentacion.OrganizacionID") + ", " + IBD.CargarGuid("RecursosRelacionadosPresentacion.ProyectoID") + ", " + IBD.CargarGuid(" RecursosRelacionadosPresentacion.OntologiaID") + ", RecursosRelacionadosPresentacion.Orden, RecursosRelacionadosPresentacion.Ontologia,RecursosRelacionadosPresentacion.Propiedad,RecursosRelacionadosPresentacion.Nombre,RecursosRelacionadosPresentacion.Imagen FROM RecursosRelacionadosPresentacion";

            this.sqlSelectProyectoPestanyaMenu = "SELECT " + IBD.CargarGuid("ProyectoPestanyaMenu.PestanyaID") + "," + IBD.CargarGuid("ProyectoPestanyaMenu.OrganizacionID") + "," + IBD.CargarGuid("ProyectoPestanyaMenu.ProyectoID") + "," + IBD.CargarGuid("ProyectoPestanyaMenu.PestanyaPadreID") + ", ProyectoPestanyaMenu.TipoPestanya, ProyectoPestanyaMenu.Nombre, ProyectoPestanyaMenu.Ruta, ProyectoPestanyaMenu.Orden, ProyectoPestanyaMenu.NuevaPestanya, ProyectoPestanyaMenu.Visible, ProyectoPestanyaMenu.Privacidad, ProyectoPestanyaMenu.HtmlAlternativo, ProyectoPestanyaMenu.IdiomasDisponibles,ProyectoPestanyaMenu.Titulo, ProyectoPestanyaMenu.NombreCortoPestanya, ProyectoPestanyaMenu.VisibleSinAcceso, ProyectoPestanyaMenu.CSSBodyClass, ProyectoPestanyaMenu.Activa, ProyectoPestanyaMenu.MetaDescription FROM ProyectoPestanyaMenu";

            this.sqlSelectProyectoPestanyaCMS = "SELECT " + IBD.CargarGuid("ProyectoPestanyaCMS.PestanyaID") + ", ProyectoPestanyaCMS.Ubicacion FROM ProyectoPestanyaCMS";

            this.sqlSelectProyectoPestanyaBusqueda = "SELECT " + IBD.CargarGuid("ProyectoPestanyaBusqueda.PestanyaID") + ", ProyectoPestanyaBusqueda.CampoFiltro,ProyectoPestanyaBusqueda.NumeroRecursos, ProyectoPestanyaBusqueda.VistaDisponible, ProyectoPestanyaBusqueda.MostrarFacetas, ProyectoPestanyaBusqueda.MostrarCajaBusqueda, ProyectoPestanyaBusqueda.ProyectoOrigenID, ProyectoPestanyaBusqueda.OcultarResultadosSinFiltros, ProyectoPestanyaBusqueda.PosicionCentralMapa,ProyectoPestanyaBusqueda.GruposConfiguracion,ProyectoPestanyaBusqueda.GruposPorTipo,ProyectoPestanyaBusqueda.TextoBusquedaSinResultados ,ProyectoPestanyaBusqueda.TextoDefectoBuscador, ProyectoPestanyaBusqueda.MostrarEnComboBusqueda, ProyectoPestanyaBusqueda.IgnorarPrivacidadEnBusqueda, ProyectoPestanyaBusqueda.OmitirCargaInicialFacetasResultados FROM ProyectoPestanyaBusqueda";

            this.sqlSelectProyectoPestanyaMenuRolGrupoIdentidades = "SELECT " + IBD.CargarGuid("ProyectoPestanyaMenuRolGrupoIdentidades.PestanyaID") + ", ProyectoPestanyaMenuRolGrupoIdentidades.GrupoID FROM ProyectoPestanyaMenuRolGrupoIdentidades";

            this.sqlSelectProyectoPestanyaMenuRolIdentidad = "SELECT " + IBD.CargarGuid("ProyectoPestanyaMenuRolIdentidad.PestanyaID") + ", ProyectoPestanyaMenuRolIdentidad.PerfilID FROM ProyectoPestanyaMenuRolIdentidad";

            this.sqlSelectProyectoSearchPersonalizado = "SELECT " + IBD.CargarGuid("ProyectoSearchPersonalizado.OrganizacionID") + ", " + IBD.CargarGuid("ProyectoSearchPersonalizado.ProyectoID") + ", ProyectoSearchPersonalizado.NombreFiltro, ProyectoSearchPersonalizado.WhereSPARQL, ProyectoSearchPersonalizado.OrderBySPARQL, ProyectoSearchPersonalizado.WhereFacetasSPARQL, ProyectoSearchPersonalizado.OmitirRdfType FROM ProyectoSearchPersonalizado ";

            this.sqlSelectProyectoSearchPersonalizadoPorProyectoID = sqlSelectProyectoSearchPersonalizado + " WHERE proyectoID = " + IBD.GuidParamValor("proyectoID");

            this.sqlSelectOntologiaProyecto = "SELECT " + IBD.CargarGuid("OntologiaProyecto.OrganizacionID") + ", " + IBD.CargarGuid("OntologiaProyecto.ProyectoID") + ", OntologiaProyecto.OntologiaProyecto, OntologiaProyecto.NombreOnt, OntologiaProyecto.Namespace, OntologiaProyecto.NamespacesExtra, OntologiaProyecto.SubTipos, OntologiaProyecto.NombreCortoOnt, OntologiaProyecto.CachearDatosSemanticos, OntologiaProyecto.EsBuscable FROM OntologiaProyecto ";

            this.sqlSelectOntologiaProyectoPorProyectoID = sqlSelectOntologiaProyecto + " WHERE proyectoID = " + IBD.GuidParamValor("proyectoID");

            this.sqlSelectVistaVirtualProyecto = "SELECT " + IBD.CargarGuid("VistaVirtualProyecto.OrganizacionID") + ", " + IBD.CargarGuid("VistaVirtualProyecto.ProyectoID") + ", " + IBD.CargarGuid("VistaVirtualProyecto.PersonalizacionID") + " FROM VistaVirtualProyecto ";
            this.sqlSelectVistaVirtualProyectoPorProyectoID = sqlSelectVistaVirtualProyecto + " WHERE proyectoID = " + IBD.GuidParamValor("proyectoID");

            this.sqlSelectProyectoPaginaHtml = "SELECT " + IBD.CargarGuid("ProyectoPaginaHtml.ProyectoID") + ", ProyectoPaginaHtml.Nombre, ProyectoPaginaHtml.Html, ProyectoPaginaHtml.Idioma FROM ProyectoPaginaHtml";

            this.sqlSelectProyectoPerfilNumElem = "SELECT " + IBD.CargarGuid("ProyectoPerfilNumElem.ProyectoID") + ", " + IBD.CargarGuid("ProyectoPerfilNumElem.PerfilID") + ", ProyectoPerfilNumElem.NumRecursos FROM ProyectoPerfilNumElem";

            this.sqlSelectSeccionProyCatalogo = "SELECT " + IBD.CargarGuid("SeccionProyCatalogo.OrganizacionID") + ", " + IBD.CargarGuid("SeccionProyCatalogo.ProyectoID") + ", " + IBD.CargarGuid("SeccionProyCatalogo.OrganizacionBusquedaID") + ", " + IBD.CargarGuid("SeccionProyCatalogo.ProyectoBusquedaID") + ", SeccionProyCatalogo.Tipo, SeccionProyCatalogo.Nombre, SeccionProyCatalogo.Faceta, SeccionProyCatalogo.Filtro, SeccionProyCatalogo.NumeroResultados, SeccionProyCatalogo.Orden FROM SeccionProyCatalogo";

            this.sqlSelectProyectoLoginConfiguracion = "SELECT " + IBD.CargarGuid("ProyectoLoginConfiguracion.OrganizacionID") + ", " + IBD.CargarGuid("ProyectoLoginConfiguracion.ProyectoID") + ", ProyectoLoginConfiguracion.Mensaje FROM ProyectoLoginConfiguracion";

            this.sqlSelectCamposRegistroProyectoGenericosPorIDProyecto = "SELECT " + IBD.CargarGuid("CamposRegistroProyectoGenericos.OrganizacionID") + "," + IBD.CargarGuid("CamposRegistroProyectoGenericos.ProyectoID") + ", CamposRegistroProyectoGenericos.Orden, CamposRegistroProyectoGenericos.Tipo FROM CamposRegistroProyectoGenericos WHERE proyectoID = " + IBD.GuidParamValor("proyectoID");

            this.sqlSelectCamposRegistroProyectoGenericosPorListaProyectosID = "SELECT " + IBD.CargarGuid("CamposRegistroProyectoGenericos.OrganizacionID") + "," + IBD.CargarGuid("CamposRegistroProyectoGenericos.ProyectoID") + ", CamposRegistroProyectoGenericos.Orden, CamposRegistroProyectoGenericos.Tipo FROM CamposRegistroProyectoGenericos WHERE proyectoID IN (";

            this.sqlSelectDatoExtraProyectoPorIDProyecto = "SELECT " + IBD.CargarGuid("DatoExtraProyecto.OrganizacionID") + "," + IBD.CargarGuid("DatoExtraProyecto.ProyectoID") + "," + IBD.CargarGuid("DatoExtraProyecto.DatoExtraID") + ", DatoExtraProyecto.Orden, DatoExtraProyecto.Titulo, DatoExtraProyecto.PredicadoRDF, DatoExtraProyecto.Obligatorio, DatoExtraProyecto.Paso1Registro FROM DatoExtraProyecto WHERE proyectoID = " + IBD.GuidParamValor("proyectoID");

            this.sqlSelectDatoExtraProyectoPorListaProyectosID = "SELECT " + IBD.CargarGuid("DatoExtraProyecto.OrganizacionID") + "," + IBD.CargarGuid("DatoExtraProyecto.ProyectoID") + "," + IBD.CargarGuid("DatoExtraProyecto.DatoExtraID") + ", DatoExtraProyecto.Orden, DatoExtraProyecto.Titulo, DatoExtraProyecto.PredicadoRDF, DatoExtraProyecto.Obligatorio, DatoExtraProyecto.Paso1Registro FROM DatoExtraProyecto WHERE proyectoID IN (";

            this.sqlSelectDatoExtraProyectoOpcionPorIDProyecto = "SELECT " + IBD.CargarGuid("DatoExtraProyectoOpcion.OrganizacionID") + "," + IBD.CargarGuid("DatoExtraProyectoOpcion.ProyectoID") + "," + IBD.CargarGuid("DatoExtraProyectoOpcion.DatoExtraID") + "," + IBD.CargarGuid("DatoExtraProyectoOpcion.OpcionID") + ", DatoExtraProyectoOpcion.Orden, DatoExtraProyectoOpcion.Opcion FROM DatoExtraProyectoOpcion WHERE proyectoID = " + IBD.GuidParamValor("proyectoID");

            this.sqlSelectDatoExtraProyectoOpcionPorListaProyectosID = "SELECT " + IBD.CargarGuid("DatoExtraProyectoOpcion.OrganizacionID") + "," + IBD.CargarGuid("DatoExtraProyectoOpcion.ProyectoID") + "," + IBD.CargarGuid("DatoExtraProyectoOpcion.DatoExtraID") + "," + IBD.CargarGuid("DatoExtraProyectoOpcion.OpcionID") + ", DatoExtraProyectoOpcion.Orden, DatoExtraProyectoOpcion.Opcion FROM DatoExtraProyectoOpcion WHERE proyectoID IN (";

            this.sqlSelectDatoExtraProyectoVirtuosoPorIDProyecto = "SELECT " + IBD.CargarGuid("DatoExtraProyectoVirtuoso.OrganizacionID") + "," + IBD.CargarGuid("DatoExtraProyectoVirtuoso.ProyectoID") + "," + IBD.CargarGuid("DatoExtraProyectoVirtuoso.DatoExtraID") + ", DatoExtraProyectoVirtuoso.Orden, DatoExtraProyectoVirtuoso.Titulo, DatoExtraProyectoVirtuoso.InputID, DatoExtraProyectoVirtuoso.InputsSuperiores, DatoExtraProyectoVirtuoso.QueryVirtuoso, DatoExtraProyectoVirtuoso.ConexionBD, DatoExtraProyectoVirtuoso.Obligatorio, DatoExtraProyectoVirtuoso.Paso1Registro, DatoExtraProyectoVirtuoso.VisibilidadFichaPerfil, DatoExtraProyectoVirtuoso.PredicadoRDF, DatoExtraProyectoVirtuoso.NombreCampo, DatoExtraProyectoVirtuoso.EstructuraHTMLFicha FROM DatoExtraProyectoVirtuoso WHERE proyectoID = " + IBD.GuidParamValor("proyectoID");

            this.sqlSelectDatoExtraProyectoVirtuosoPorListaProyectosID = "SELECT " + IBD.CargarGuid("DatoExtraProyectoVirtuoso.OrganizacionID") + "," + IBD.CargarGuid("DatoExtraProyectoVirtuoso.ProyectoID") + "," + IBD.CargarGuid("DatoExtraProyectoVirtuoso.DatoExtraID") + ", DatoExtraProyectoVirtuoso.Orden, DatoExtraProyectoVirtuoso.Titulo, DatoExtraProyectoVirtuoso.InputID, DatoExtraProyectoVirtuoso.InputsSuperiores, DatoExtraProyectoVirtuoso.QueryVirtuoso, DatoExtraProyectoVirtuoso.ConexionBD, DatoExtraProyectoVirtuoso.Obligatorio, DatoExtraProyectoVirtuoso.Paso1Registro, DatoExtraProyectoVirtuoso.VisibilidadFichaPerfil, DatoExtraProyectoVirtuoso.PredicadoRDF, DatoExtraProyectoVirtuoso.NombreCampo, DatoExtraProyectoVirtuoso.EstructuraHTMLFicha FROM DatoExtraProyectoVirtuoso WHERE proyectoID IN (";

            this.sqlSelectDatoExtraEcosistema = "SELECT " + IBD.CargarGuid("DatoExtraEcosistema.DatoExtraID") + ", DatoExtraEcosistema.Orden, DatoExtraEcosistema.Titulo, DatoExtraEcosistema.PredicadoRDF, DatoExtraEcosistema.Obligatorio, DatoExtraEcosistema.Paso1Registro FROM DatoExtraEcosistema ";

            this.sqlSelectDatoExtraEcosistemaOpcion = "SELECT " + IBD.CargarGuid("DatoExtraEcosistemaOpcion.DatoExtraID") + "," + IBD.CargarGuid("DatoExtraEcosistemaOpcion.OpcionID") + ", DatoExtraEcosistemaOpcion.Orden, DatoExtraEcosistemaOpcion.Opcion FROM DatoExtraEcosistemaOpcion ";

            this.sqlSelectDatoExtraEcosistemaVirtuoso = "SELECT " + IBD.CargarGuid("DatoExtraEcosistemaVirtuoso.DatoExtraID") + ", DatoExtraEcosistemaVirtuoso.Orden, DatoExtraEcosistemaVirtuoso.Titulo, DatoExtraEcosistemaVirtuoso.InputID, DatoExtraEcosistemaVirtuoso.InputsSuperiores, DatoExtraEcosistemaVirtuoso.QueryVirtuoso, DatoExtraEcosistemaVirtuoso.ConexionBD, DatoExtraEcosistemaVirtuoso.Obligatorio, DatoExtraEcosistemaVirtuoso.Paso1Registro, DatoExtraEcosistemaVirtuoso.VisibilidadFichaPerfil, DatoExtraEcosistemaVirtuoso.PredicadoRDF, DatoExtraEcosistemaVirtuoso.NombreCampo, DatoExtraEcosistemaVirtuoso.EstructuraHTMLFicha FROM DatoExtraEcosistemaVirtuoso ";

            this.sqlSelectPreferenciaProyectoPorIDProyecto = "SELECT " + IBD.CargarGuid("PreferenciaProyecto.OrganizacionID") + "," + IBD.CargarGuid("PreferenciaProyecto.ProyectoID") + "," + IBD.CargarGuid("PreferenciaProyecto.TesauroID") + "," + IBD.CargarGuid("PreferenciaProyecto.CategoriaTesauroID") + ", Orden FROM PreferenciaProyecto WHERE proyectoID = " + IBD.GuidParamValor("proyectoID");

            this.sqlSelectEventosProyectoPorProyectoID = "SELECT " + IBD.CargarGuid("ProyectoEvento.OrganizacionID") + "," + IBD.CargarGuid("ProyectoEvento.ProyectoID") + "," + IBD.CargarGuid("ProyectoEvento.EventoID") + ",Nombre, Descripcion, TipoEvento, Activo, InfoExtra, Interno, ComponenteID, Grupo, UrlRedirect FROM ProyectoEvento WHERE proyectoID = " + IBD.GuidParamValor("proyectoID");

            this.sqlSelectEventosProyectoPorIdentidadID = "SELECT " + IBD.CargarGuid("ProyectoEvento.EventoID") + ", ProyectoEvento.Nombre, ProyectoEvento.InfoExtra, ProyectoEventoParticipante.Fecha FROM ProyectoEvento inner join ProyectoEventoParticipante on ProyectoEventoParticipante.EventoID = ProyectoEvento.EventoID WHERE proyectoID = " + IBD.GuidParamValor("proyectoID") + " AND ProyectoEventoParticipante.IdentidadID = " + IBD.GuidParamValor("identidadID");

            this.sqlSelectEventoProyectoPorEventoID = "SELECT " + IBD.CargarGuid("ProyectoEvento.OrganizacionID") + "," + IBD.CargarGuid("ProyectoEvento.ProyectoID") + "," + IBD.CargarGuid("ProyectoEvento.EventoID") + ",Nombre, Descripcion, TipoEvento, Activo, InfoExtra, Interno, ComponenteID, Grupo, UrlRedirect FROM ProyectoEvento WHERE eventoID = " + IBD.GuidParamValor("eventoID");

            this.sqlSelectEventoProyectoParticipantesPorEventoID = "SELECT " + IBD.CargarGuid("ProyectoEventoParticipante.IdentidadID") + "," + IBD.CargarGuid("ProyectoEventoParticipante.EventoID") + " , ProyectoEventoParticipante.Fecha  FROM ProyectoEventoParticipante WHERE eventoID = " + IBD.GuidParamValor("eventoID");

            this.sqlSelectEventosAccionProyectoPorProyectoID = "SELECT " + IBD.CargarGuid("ProyectoEventoAccion.OrganizacionID") + "," + IBD.CargarGuid("ProyectoEventoAccion.ProyectoID") + ", Evento, AccionJS FROM ProyectoEventoAccion WHERE proyectoID = " + IBD.GuidParamValor("proyectoID");

            this.sqlSelectProyectoPasoRegistro = "SELECT " + IBD.CargarGuid("ProyectoPasoRegistro.OrganizacionID") + ", " + IBD.CargarGuid("ProyectoPasoRegistro.ProyectoID") + " , ProyectoPasoRegistro.Orden, ProyectoPasoRegistro.PasoRegistro FROM ProyectoPasoRegistro ";

            this.sqlSelectProyectoPasoRegistroPorProyectoID = sqlSelectProyectoPasoRegistro + " WHERE ProyectoPasoRegistro.ProyectoID = " + IBD.GuidParamValor("proyectoID");


            #region Documentacion

            this.sqlSelectTipoDocsDispDeUsuarioEnUNProy = IBD.ReplaceParam(sqlSelectTipoDocDispRolUsuarioProy + " WHERE ProyectoID = " + IBD.GuidParamValor("ProyectoID") + " AND RolUsuario >= " + IBD.ToParam("RolUsuario") + "");

            this.sqlSelectExisteDocAsociadoANivelCertif = "SELECT " + IBD.CargarGuid("DocumentoID") + " FROM DocumentoWebVinBaseRecursos WHERE DocumentoWebVinBaseRecursos.NivelCertificacionID = " + IBD.GuidParamValor("pNivelCertificacionID");

            #endregion

            #region Datos Twitter

            this.sqlUpdateTokenTwitterProyecto = IBD.ReplaceParam("UPDATE Proyecto SET TokenTwitter = @TokenTwitter, TokenSecretoTwitter = @TokenSecretoTwitter WHERE Proyecto.ProyectoID = " + IBD.GuidParamValor("ProyectoID") + " ");

            #endregion

            this.sqlSelectAccionesExternasProyecto = "SELECT AccionesExternasProyecto.TipoAccion, " + IBD.CargarGuid("AccionesExternasProyecto.OrganizacionID") + ", " + IBD.CargarGuid("AccionesExternasProyecto.ProyectoID") + ", AccionesExternasProyecto.URL FROM AccionesExternasProyecto";

            this.sqlSelectAccionesExternasProyectoPorProyectoID = sqlSelectAccionesExternasProyecto + " WHERE (AccionesExternasProyecto.ProyectoID = " + IBD.GuidParamValor("ProyectoID") + ") ";

            this.sqlSelectAccionesExternasProyectoPorListaID = sqlSelectAccionesExternasProyecto + " WHERE AccionesExternasProyecto.ProyectoID IN (";

            this.sqlSelectAccionesExternasProyecto = "SELECT AccionesExternasProyecto.TipoAccion, " + IBD.CargarGuid("AccionesExternasProyecto.OrganizacionID") + ", " + IBD.CargarGuid("AccionesExternasProyecto.ProyectoID") + ", AccionesExternasProyecto.URL FROM AccionesExternasProyecto";

            this.sqlSelectAccionesExternasProyectoPorProyectoID = sqlSelectAccionesExternasProyecto + " WHERE (AccionesExternasProyecto.ProyectoID = " + IBD.GuidParamValor("ProyectoID") + ") ";

            this.sqlSelectAccionesExternasProyectoPorListaID = sqlSelectAccionesExternasProyecto + " WHERE AccionesExternasProyecto.ProyectoID IN (";


            #endregion

            #region DataAdapters

            #region ProyectoPestanyaFiltroOrdenRecursos
            this.sqlProyectoPestanyaFiltroOrdenRecursosInsert = IBD.ReplaceParam("INSERT INTO ProyectoPestanyaFiltroOrdenRecursos (PestanyaID, FiltroOrden, NombreFiltro, Orden) VALUES (" + IBD.GuidParamColumnaTabla("PestanyaID") + ", @FiltroOrden, @NombreFiltro, @Orden)");
            this.sqlProyectoPestanyaFiltroOrdenRecursosDelete = IBD.ReplaceParam("DELETE FROM ProyectoPestanyaFiltroOrdenRecursos WHERE (PestanyaID = " + IBD.GuidParamColumnaTabla("Original_PestanyaID") + ") AND (FiltroOrden = @Original_FiltroOrden) AND (NombreFiltro = @Original_NombreFiltro) AND (Orden = @Original_Orden)");
            this.sqlProyectoPestanyaFiltroOrdenRecursosModify = IBD.ReplaceParam("UPDATE ProyectoPestanyaFiltroOrdenRecursos SET PestanyaID = " + IBD.GuidParamColumnaTabla("PestanyaID") + ", FiltroOrden = @FiltroOrden, NombreFiltro = @NombreFiltro, Orden = @Orden WHERE (PestanyaID = " + IBD.GuidParamColumnaTabla("Original_PestanyaID") + ") AND (FiltroOrden = @Original_FiltroOrden) AND (NombreFiltro = @Original_NombreFiltro) AND (Orden = @Original_Orden)");
            #endregion

            #region Proyecto

            this.sqlProyectoInsert = IBD.ReplaceParam("INSERT INTO Proyecto (OrganizacionID, ProyectoID, Nombre, Descripcion, FechaInicio, FechaFin, TipoProyecto, TipoAcceso, NumeroRecursos, NumeroPreguntas, NumeroDebates, NumeroMiembros, NumeroOrgRegistradas, NumeroArticulos, NumeroDafos, NumeroForos, ProyectoSuperiorID, EsProyectoDestacado, Estado, URLPropia, NombreCorto, TieneTwitter, TagTwitter, UsuarioTwitter, TokenTwitter, TokenSecretoTwitter, EnviarTwitterComentario, EnviarTwitterNuevaCat, EnviarTwitterNuevoAdmin, EnviarTwitterNuevaPolitCert, EnviarTwitterNuevoTipoDoc, ProcesoVinculadoID, Tags, TagTwitterGnoss, NombrePresentacion) VALUES (" + IBD.GuidParamColumnaTabla("OrganizacionID") + ", " + IBD.GuidParamColumnaTabla("ProyectoID") + ", @Nombre, @Descripcion, @FechaInicio, @FechaFin, @TipoProyecto, @TipoAcceso, @NumeroRecursos, @NumeroPreguntas, @NumeroDebates, @NumeroMiembros, @NumeroOrgRegistradas, @NumeroArticulos, @NumeroDafos, @NumeroForos, " + IBD.GuidParamColumnaTabla("ProyectoSuperiorID") + ", @EsProyectoDestacado, @Estado, @URLPropia, @NombreCorto, @TieneTwitter, @TagTwitter, @UsuarioTwitter, @TokenTwitter, @TokenSecretoTwitter, @EnviarTwitterComentario, @EnviarTwitterNuevaCat, @EnviarTwitterNuevoAdmin, @EnviarTwitterNuevaPolitCert, @EnviarTwitterNuevoTipoDoc, @ProcesoVinculadoID, @Tags, @TagTwitterGnoss, @NombrePresentacion)");

            this.sqlProyectoDelete = IBD.ReplaceParam("DELETE FROM Proyecto WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("O_ProyectoID") + ") AND (Nombre = @O_Nombre) AND " + IBD.ComparacionCamposTextoPesadoConOriginal("Descripcion", true) + " AND (FechaInicio = @O_FechaInicio OR @O_FechaInicio IS NULL AND FechaInicio IS NULL) AND (FechaFin = @O_FechaFin OR @O_FechaFin IS NULL AND FechaFin IS NULL) AND (TipoProyecto = @O_TipoProyecto) AND (TipoAcceso = @O_TipoAcceso) AND (NumeroRecursos = @O_NumeroRecursos OR @O_NumeroRecursos IS NULL AND NumeroRecursos IS NULL) AND (NumeroPreguntas = @O_NumeroPreguntas OR @O_NumeroPreguntas IS NULL AND NumeroPreguntas IS NULL) AND (NumeroDebates = @O_NumeroDebates OR @O_NumeroDebates IS NULL AND NumeroDebates IS NULL) AND (NumeroMiembros = @O_NumeroMiembros OR @O_NumeroMiembros IS NULL AND NumeroMiembros IS NULL) AND (NumeroOrgRegistradas = @O_NumeroOrgRegistradas OR @O_NumeroOrgRegistradas IS NULL AND NumeroOrgRegistradas IS NULL) AND (NumeroArticulos = @O_NumeroArticulos OR @O_NumeroArticulos IS NULL AND NumeroArticulos IS NULL) AND (NumeroDafos = @O_NumeroDafos OR @O_NumeroDafos IS NULL AND NumeroDafos IS NULL) AND (NumeroForos = @O_NumeroForos OR @O_NumeroForos IS NULL AND NumeroForos IS NULL) AND (ProyectoSuperiorID = " + IBD.GuidParamColumnaTabla("O_ProyectoSuperiorID") + " OR " + IBD.GuidParamColumnaTabla("O_ProyectoSuperiorID") + " IS NULL AND ProyectoSuperiorID IS NULL) AND (EsProyectoDestacado = @O_EsProyectoDestacado) AND (Estado = @O_Estado) AND (URLPropia = @O_URLPropia OR @O_URLPropia IS NULL AND URLPropia IS NULL) AND (NombreCorto = @O_NombreCorto) AND (TieneTwitter = @O_TieneTwitter) AND (TagTwitter = @O_TagTwitter OR @O_TagTwitter IS NULL AND TagTwitter IS NULL) AND (UsuarioTwitter = @O_UsuarioTwitter OR @O_UsuarioTwitter IS NULL AND UsuarioTwitter IS NULL) AND (TokenTwitter = @O_TokenTwitter OR @O_TokenTwitter IS NULL AND TokenTwitter IS NULL) AND (TokenSecretoTwitter = @O_TokenSecretoTwitter OR @O_TokenSecretoTwitter IS NULL AND TokenSecretoTwitter IS NULL) AND (EnviarTwitterComentario = @O_EnviarTwitterComentario) AND (EnviarTwitterNuevaCat = @O_EnviarTwitterNuevaCat) AND (EnviarTwitterNuevoAdmin = @O_EnviarTwitterNuevoAdmin) AND (EnviarTwitterNuevaPolitCert = @O_EnviarTwitterNuevaPolitCert) AND (EnviarTwitterNuevoTipoDoc = @O_EnviarTwitterNuevoTipoDoc) AND  (ProcesoVinculadoID = @O_ProcesoVinculadoID OR @O_ProcesoVinculadoID IS NULL AND ProcesoVinculadoID IS NULL) AND (Tags = @O_Tags OR @O_Tags IS NULL AND Tags IS NULL) AND (TagTwitterGnoss = @O_TagTwitterGnoss OR @O_TagTwitterGnoss IS NULL AND TagTwitterGnoss IS NULL) AND (NombrePresentacion = @O_NombrePresentacion OR @O_NombrePresentacion IS NULL AND NombrePresentacion IS NULL) ");

            this.sqlProyectoModify = IBD.ReplaceParam("UPDATE Proyecto SET OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + ", Nombre = @Nombre, Descripcion = @Descripcion, FechaInicio = @FechaInicio, FechaFin = @FechaFin, TipoProyecto = @TipoProyecto, TipoAcceso = @TipoAcceso, NumeroRecursos = @NumeroRecursos, NumeroPreguntas = @NumeroPreguntas, NumeroDebates = @NumeroDebates, NumeroMiembros = @NumeroMiembros, NumeroOrgRegistradas = @NumeroOrgRegistradas, NumeroArticulos = @NumeroArticulos, NumeroDafos = @NumeroDafos, NumeroForos = @NumeroForos, ProyectoSuperiorID = " + IBD.GuidParamColumnaTabla("ProyectoSuperiorID") + ", EsProyectoDestacado = @EsProyectoDestacado, Estado = @Estado, URLPropia = @URLPropia, NombreCorto = @NombreCorto, TieneTwitter = @TieneTwitter, TagTwitter = @TagTwitter, UsuarioTwitter = @UsuarioTwitter, TokenTwitter = @TokenTwitter, TokenSecretoTwitter = @TokenSecretoTwitter, EnviarTwitterComentario = @EnviarTwitterComentario, EnviarTwitterNuevaCat = @EnviarTwitterNuevaCat, EnviarTwitterNuevoAdmin = @EnviarTwitterNuevoAdmin, EnviarTwitterNuevaPolitCert = @EnviarTwitterNuevaPolitCert, EnviarTwitterNuevoTipoDoc = @EnviarTwitterNuevoTipoDoc, ProcesoVinculadoID = @ProcesoVinculadoID, Tags = @Tags, TagTwitterGnoss = @TagTwitterGnoss, NombrePresentacion = @NombrePresentacion WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("O_ProyectoID") + ")");

            #endregion

            #region AdministradorProyecto

            this.sqlAdministradorProyectoInsert = IBD.ReplaceParam("INSERT INTO AdministradorProyecto (OrganizacionID, ProyectoID, UsuarioID, Tipo) VALUES (" + IBD.GuidParamColumnaTabla("OrganizacionID") + ", " + IBD.GuidParamColumnaTabla("ProyectoID") + ", " + IBD.GuidParamColumnaTabla("UsuarioID") + ", @Tipo)");

            this.sqlAdministradorProyectoDelete = IBD.ReplaceParam("DELETE FROM AdministradorProyecto WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("O_ProyectoID") + ") AND (UsuarioID = " + IBD.GuidParamColumnaTabla("O_UsuarioID") + ") AND (Tipo = @O_Tipo)");

            this.sqlAdministradorProyectoModify = IBD.ReplaceParam("UPDATE AdministradorProyecto SET OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + ", UsuarioID = " + IBD.GuidParamColumnaTabla("UsuarioID") + ", Tipo = @Tipo WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("O_ProyectoID") + ") AND (UsuarioID = " + IBD.GuidParamColumnaTabla("O_UsuarioID") + ") AND (Tipo = @O_Tipo)");

            #endregion

            #region AdministradorGrupoProyecto

            this.sqlAdministradorGrupoProyectoInsert = IBD.ReplaceParam("INSERT INTO AdministradorGrupoProyecto (OrganizacionID, ProyectoID, GrupoID) VALUES (" + IBD.GuidParamColumnaTabla("OrganizacionID") + ", " + IBD.GuidParamColumnaTabla("ProyectoID") + ", " + IBD.GuidParamColumnaTabla("GrupoID") + ")");

            this.sqlAdministradorGrupoProyectoDelete = IBD.ReplaceParam("DELETE FROM AdministradorGrupoProyecto WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("O_ProyectoID") + ") AND (GrupoID = " + IBD.GuidParamColumnaTabla("O_GrupoID") + ")");

            this.sqlAdministradorGrupoProyectoModify = IBD.ReplaceParam("UPDATE AdministradorGrupoProyecto SET OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + ", GrupoID = " + IBD.GuidParamColumnaTabla("GrupoID") + " WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("O_ProyectoID") + ") AND (GrupoID = " + IBD.GuidParamColumnaTabla("O_GrupoID") + ")");

            #endregion


            #region ProyectosMasActivos

            this.sqlProyectosMasActivosInsert = IBD.ReplaceParam("INSERT INTO ProyectosMasActivos (OrganizacionID, ProyectoID, Nombre, Peso, NumeroConsultas) VALUES (" + IBD.GuidParamColumnaTabla("OrganizacionID") + ", " + IBD.GuidParamColumnaTabla("ProyectoID") + ", @Nombre, @Peso, @NumeroConsultas)");

            this.sqlProyectosMasActivosDelete = IBD.ReplaceParam("DELETE FROM ProyectosMasActivos WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("O_ProyectoID") + ") AND (Nombre = @O_Nombre) AND (Peso = @O_Peso) AND (NumeroConsultas = @O_NumeroConsultas)");

            this.sqlProyectosMasActivosModify = IBD.ReplaceParam("UPDATE ProyectosMasActivos SET OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + ", Nombre = @Nombre, Peso = @Peso, NumeroConsultas = @NumeroConsultas WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("O_ProyectoID") + ") AND (Nombre = @O_Nombre) AND (Peso = @O_Peso) AND (NumeroConsultas = @O_NumeroConsultas)");

            #endregion

            #region ProyectoContactoEmpleadoDataAdapter

            this.sqlProyectoContactoEmpleadoInsert = "INSERT INTO ProyectoContactoEmpleado(OrganizacionID, ProyectoID, PersonaID) VALUES (" + IBD.GuidParamColumnaTabla("OrganizacionID") + ", " + IBD.GuidParamColumnaTabla("ProyectoID") + ", " + IBD.GuidParamColumnaTabla("PersonaID") + ")";
            this.sqlProyectoContactoEmpleadoDelete = "DELETE FROM ProyectoContactoEmpleado WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (PersonaID = " + IBD.GuidParamColumnaTabla("O_PersonaID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("O_ProyectoID") + ")";

            this.sqlProyectoContactoEmpleadoModify = "UPDATE ProyectoContactoEmpleado SET OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + ", PersonaID = @PersonaID WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (PersonaID = " + IBD.GuidParamColumnaTabla("O_PersonaID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("O_ProyectoID") + ")";

            #endregion

            #region ProyectoAgCatTesauro

            this.sqlProyectoAgCatTesauroInsert = IBD.ReplaceParam("INSERT INTO ProyectoAgCatTesauro (OrganizacionID, ProyectoID, TesauroID, CategoriaTesauroID) VALUES (" + IBD.GuidParamColumnaTabla("OrganizacionID") + ", " + IBD.GuidParamColumnaTabla("ProyectoID") + ", " + IBD.GuidParamColumnaTabla("TesauroID") + ", " + IBD.GuidParamColumnaTabla("CategoriaTesauroID") + ")");

            this.sqlProyectoAgCatTesauroDelete = IBD.ReplaceParam("DELETE FROM ProyectoAgCatTesauro WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("O_ProyectoID") + ") AND (TesauroID = " + IBD.GuidParamColumnaTabla("O_TesauroID") + ") AND (CategoriaTesauroID = " + IBD.GuidParamColumnaTabla("O_CategoriaTesauroID") + ")");

            this.sqlProyectoAgCatTesauroModify = IBD.ReplaceParam("UPDATE ProyectoAgCatTesauro SET OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + ", TesauroID = " + IBD.GuidParamColumnaTabla("TesauroID") + ", CategoriaTesauroID = " + IBD.GuidParamColumnaTabla("CategoriaTesauroID") + " WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("O_ProyectoID") + ") AND (TesauroID = " + IBD.GuidParamColumnaTabla("O_TesauroID") + ") AND (CategoriaTesauroID = " + IBD.GuidParamColumnaTabla("O_CategoriaTesauroID") + ")");

            #endregion

            #region NivelCertificacion

            this.sqlNivelCertificacionInsert = IBD.ReplaceParam("INSERT INTO NivelCertificacion (NivelCertificacionID, OrganizacionID, ProyectoID, Orden, Descripcion) VALUES (" + IBD.GuidParamColumnaTabla("NivelCertificacionID") + ", " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", " + IBD.GuidParamColumnaTabla("ProyectoID") + ", @Orden, @Descripcion)");

            this.sqlNivelCertificacionDelete = IBD.ReplaceParam("DELETE FROM NivelCertificacion WHERE (NivelCertificacionID = " + IBD.GuidParamColumnaTabla("O_NivelCertificacionID") + ") AND (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("O_ProyectoID") + ") AND (Orden = @O_Orden) AND (Descripcion = @O_Descripcion)");

            this.sqlNivelCertificacionModify = IBD.ReplaceParam("UPDATE NivelCertificacion SET NivelCertificacionID = " + IBD.GuidParamColumnaTabla("NivelCertificacionID") + ", OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + ", Orden = @Orden, Descripcion = @Descripcion WHERE (NivelCertificacionID = " + IBD.GuidParamColumnaTabla("O_NivelCertificacionID") + ") AND (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("O_ProyectoID") + ") AND (Orden = @O_Orden) AND (Descripcion = @O_Descripcion)");

            #endregion

            #region TipoDocDispRolUsuarioProy

            this.sqlTipoDocDispRolUsuarioProyInsert = IBD.ReplaceParam("INSERT INTO TipoDocDispRolUsuarioProy (OrganizacionID, ProyectoID, TipoDocumento, RolUsuario) VALUES (" + IBD.GuidParamColumnaTabla("OrganizacionID") + ", " + IBD.GuidParamColumnaTabla("ProyectoID") + ", @TipoDocumento, @RolUsuario)");

            this.sqlTipoDocDispRolUsuarioProyDelete = IBD.ReplaceParam("DELETE FROM TipoDocDispRolUsuarioProy WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("O_ProyectoID") + ") AND (TipoDocumento = @O_TipoDocumento) AND (RolUsuario = @O_RolUsuario)");

            this.sqlTipoDocDispRolUsuarioProyModify = IBD.ReplaceParam("UPDATE TipoDocDispRolUsuarioProy SET OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + ", TipoDocumento = @TipoDocumento, RolUsuario = @RolUsuario WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("O_ProyectoID") + ") AND (TipoDocumento = @O_TipoDocumento) AND (RolUsuario = @O_RolUsuario)");

            #endregion

            #region TipoOntoDispRolUsuarioProy

            this.sqlTipoOntoDispRolUsuarioProyInsert = IBD.ReplaceParam("INSERT INTO TipoOntoDispRolUsuarioProy (OrganizacionID, ProyectoID, OntologiaID, RolUsuario) VALUES (" + IBD.GuidParamColumnaTabla("OrganizacionID") + ", " + IBD.GuidParamColumnaTabla("ProyectoID") + ", " + IBD.GuidParamColumnaTabla("OntologiaID") + ", @RolUsuario)");

            this.sqlTipoOntoDispRolUsuarioProyDelete = IBD.ReplaceParam("DELETE FROM TipoOntoDispRolUsuarioProy WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("O_ProyectoID") + ") AND (OntologiaID = " + IBD.GuidParamColumnaTabla("O_OntologiaID") + ") AND (RolUsuario = @O_RolUsuario)");

            this.sqlTipoOntoDispRolUsuarioProyModify = IBD.ReplaceParam("UPDATE TipoOntoDispRolUsuarioProy SET OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + ", OntologiaID = " + IBD.GuidParamColumnaTabla("OntologiaID") + ", RolUsuario = @RolUsuario WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("O_ProyectoID") + ") AND (OntologiaID = " + IBD.GuidParamColumnaTabla("O_OntologiaID") + ") AND (RolUsuario = @O_RolUsuario)");

            #endregion

            #region ProyectoCerradoTmp

            this.sqlProyectoCerradoTmpInsert = IBD.ReplaceParam("INSERT INTO ProyectoCerradoTmp (OrganizacionID, ProyectoID, Motivo, FechaCierre,FechaReapertura) VALUES (" + IBD.GuidParamColumnaTabla("OrganizacionID") + ", " + IBD.GuidParamColumnaTabla("ProyectoID") + ", @Motivo, @FechaCierre, @FechaReapertura)");

            this.sqlProyectoCerradoTmpDelete = IBD.ReplaceParam("DELETE FROM ProyectoCerradoTmp WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("O_ProyectoID") + ") AND (Motivo = @O_Motivo) AND (FechaCierre = @O_FechaCierre) AND  (FechaReapertura = @O_FechaReapertura) ");

            this.sqlProyectoCerradoTmpModify = IBD.ReplaceParam("UPDATE ProyectoCerradoTmp SET OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + ", Motivo = @Motivo, FechaCierre = @FechaCierre, FechaReapertura = @FechaReapertura WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("O_ProyectoID") + ") AND (Motivo = @O_Motivo) AND (FechaCierre = @O_FechaCierre) AND  (FechaReapertura = @O_FechaReapertura) ");

            #endregion

            #region ProyectoCerrandose

            this.sqlProyectoCerrandoseInsert = IBD.ReplaceParam("INSERT INTO ProyectoCerrandose (OrganizacionID, ProyectoID, FechaCierre, PeriodoDeGracia) VALUES (" + IBD.GuidParamColumnaTabla("OrganizacionID") + ", " + IBD.GuidParamColumnaTabla("ProyectoID") + ", @FechaCierre, @PeriodoDeGracia)");

            this.sqlProyectoCerrandoseDelete = IBD.ReplaceParam("DELETE FROM ProyectoCerrandose WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("O_ProyectoID") + ") AND (FechaCierre = @O_FechaCierre) AND (PeriodoDeGracia = @O_PeriodoDeGracia)");

            this.sqlProyectoCerrandoseModify = IBD.ReplaceParam("UPDATE ProyectoCerrandose SET OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + ", FechaCierre = @FechaCierre, PeriodoDeGracia = @PeriodoDeGracia WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("O_ProyectoID") + ") AND (FechaCierre = @O_FechaCierre) AND (PeriodoDeGracia = @PeriodoDeGracia)");

            #endregion

            #region ProyectoRelacionado
            this.sqlProyectoRelacionadoInsert = IBD.ReplaceParam("INSERT INTO ProyectoRelacionado (OrganizacionID, ProyectoID, OrganizacionRelacionadaID, ProyectoRelacionadoID, Orden) VALUES (" + IBD.GuidParamColumnaTabla("OrganizacionID") + ", " + IBD.GuidParamColumnaTabla("ProyectoID") + ", " + IBD.GuidParamColumnaTabla("OrganizacionRelacionadaID") + ", " + IBD.GuidParamColumnaTabla("ProyectoRelacionadoID") + ", @Orden)");
            this.sqlProyectoRelacionadoDelete = IBD.ReplaceParam("DELETE FROM ProyectoRelacionado WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ") AND (OrganizacionRelacionadaID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionRelacionadaID") + ") AND (ProyectoRelacionadoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoRelacionadoID") + " AND (Orden = @O_Orden) )");
            this.sqlProyectoRelacionadoModify = IBD.ReplaceParam("UPDATE ProyectoRelacionado SET OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + ", OrganizacionRelacionadaID = " + IBD.GuidParamColumnaTabla("OrganizacionRelacionadaID") + ", ProyectoRelacionadoID = " + IBD.GuidParamColumnaTabla("ProyectoRelacionadoID") + ", Orden=@Orden WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ") AND (OrganizacionRelacionadaID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionRelacionadaID") + ") AND (ProyectoRelacionadoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoRelacionadoID") + " AND (Orden = @O_Orden) )");
            #endregion

            #region ProyectoGadget

            this.sqlProyectoGadgetInsert = IBD.ReplaceParam("INSERT INTO ProyectoGadget (OrganizacionID, ProyectoID, GadgetID, Titulo, Contenido, Orden, Tipo,Ubicacion,Clases, TipoUbicacion, Visible, MultiIdioma,PersonalizacionComponenteID,CargarPorAjax,ComunidadDestinoFiltros, NombreCorto) VALUES (" + IBD.GuidParamColumnaTabla("OrganizacionID") + ", " + IBD.GuidParamColumnaTabla("ProyectoID") + ", " + IBD.GuidParamColumnaTabla("GadgetID") + ", @Titulo, @Contenido, @Orden, @Tipo, @Ubicacion,@Clases, @TipoUbicacion, @Visible, @MultiIdioma, @PersonalizacionComponenteID, @CargarPorAjax, @ComunidadDestinoFiltros, @NombreCorto)");

            this.sqlProyectoGadgetDelete = IBD.ReplaceParam("DELETE FROM ProyectoGadget WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ") AND (GadgetID = " + IBD.GuidParamColumnaTabla("Original_GadgetID") + ") ");

            this.sqlProyectoGadgetModify = IBD.ReplaceParam("UPDATE ProyectoGadget SET OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + ", GadgetID = " + IBD.GuidParamColumnaTabla("GadgetID") + ", Titulo = @Titulo, Contenido = @Contenido, Orden = @Orden, Tipo = @Tipo, Ubicacion = @Ubicacion, Clases = @Clases, TipoUbicacion = @TipoUbicacion, Visible = @Visible, MultiIdioma = @MultiIdioma, PersonalizacionComponenteID = " + IBD.GuidParamColumnaTabla("PersonalizacionComponenteID") + ", CargarPorAjax = @CargarPorAjax, ComunidadDestinoFiltros = @ComunidadDestinoFiltros, NombreCorto = @NombreCorto  WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ") AND (GadgetID = " + IBD.GuidParamColumnaTabla("Original_GadgetID") + ") ");

            #endregion

            #region ProyectoGadgetContexto

            this.sqlProyectoGadgetContextoInsert = IBD.ReplaceParam("INSERT INTO ProyectoGadgetContexto (OrganizacionID, ProyectoID, GadgetID, ComunidadOrigen,ComunidadOrigenFiltros, FiltrosOrigenDestino,OrdenContexto,Imagen,ProyectoOrigenID,NumRecursos,ServicioResultados,MostrarEnlaceOriginal,OcultarVerMas,NamespacesExtra,ItemsBusqueda,ResultadosEliminar,NuevaPestanya,ObtenerPrivados) VALUES (" + IBD.GuidParamColumnaTabla("OrganizacionID") + ", " + IBD.GuidParamColumnaTabla("ProyectoID") + ", " + IBD.GuidParamColumnaTabla("GadgetID") + ", @ComunidadOrigen, @ComunidadOrigenFiltros, @FiltrosOrigenDestino, @OrdenContexto,@Imagen, " + IBD.GuidParamColumnaTabla("ProyectoOrigenID") + " , @NumRecursos, @ServicioResultados, @MostrarEnlaceOriginal, @OcultarVerMas, @NamespacesExtra, @ItemsBusqueda, @ResultadosEliminar, @NuevaPestanya,@ObtenerPrivados") + ")";

            this.sqlProyectoGadgetContextoDelete = IBD.ReplaceParam("DELETE FROM ProyectoGadgetContexto WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ") AND (GadgetID = " + IBD.GuidParamColumnaTabla("Original_GadgetID") + ")");

            this.sqlProyectoGadgetContextoModify = IBD.ReplaceParam("UPDATE ProyectoGadgetContexto SET OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + ", GadgetID = " + IBD.GuidParamColumnaTabla("GadgetID") + ", ComunidadOrigen = @ComunidadOrigen, ComunidadOrigenFiltros = @ComunidadOrigenFiltros, FiltrosOrigenDestino = @FiltrosOrigenDestino, OrdenContexto = @OrdenContexto, Imagen = @Imagen, ProyectoOrigenID = " + IBD.GuidParamColumnaTabla("ProyectoOrigenID") + " , NumRecursos = @NumRecursos, ServicioResultados = @ServicioResultados, MostrarEnlaceOriginal = @MostrarEnlaceOriginal , OcultarVerMas = @OcultarVerMas , NamespacesExtra = @NamespacesExtra , ItemsBusqueda = @ItemsBusqueda , ResultadosEliminar = @ResultadosEliminar, NuevaPestanya = @NuevaPestanya, ObtenerPrivados = @ObtenerPrivados WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ") AND (GadgetID = " + IBD.GuidParamColumnaTabla("Original_GadgetID") + ") ");

            #endregion

            #region ProyectoGadgetContextoHTMLplano

            //this.sqlProyectoGadgetContextoHTMLplanoInsert = IBD.ReplaceParam("INSERT INTO ProyectoGadgetContextoHTMLplano (OrganizacionID, ProyectoID, GadgetID, ComunidadDestinoFiltros) VALUES (" + IBD.GuidParamColumnaTabla("OrganizacionID") + ", " + IBD.GuidParamColumnaTabla("ProyectoID") + ", " + IBD.GuidParamColumnaTabla("GadgetID") + ", @ComunidadDestinoFiltros");

            //this.sqlProyectoGadgetContextoHTMLplanoDelete = IBD.ReplaceParam("DELETE FROM ProyectoGadgetContextoHTMLplano WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ") AND (GadgetID = " + IBD.GuidParamColumnaTabla("Original_GadgetID") + ") ");

            //this.sqlProyectoGadgetContextoHTMLplanoModify = IBD.ReplaceParam("UPDATE ProyectoGadgetContextoHTMLplano SET OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + ", GadgetID = " + IBD.GuidParamColumnaTabla("GadgetID") + ", ComunidadDestinoFiltros = @ComunidadDestinoFiltros  WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ") AND (GadgetID = " + IBD.GuidParamColumnaTabla("Original_GadgetID") + ") ");

            #endregion

            #region ProyectoGadgetIdioma

            this.sqlProyectoGadgetIdiomaInsert = IBD.ReplaceParam("INSERT INTO ProyectoGadgetIdioma (OrganizacionID, ProyectoID, GadgetID, Idioma, Contenido) VALUES (" + IBD.GuidParamColumnaTabla("OrganizacionID") + ", " + IBD.GuidParamColumnaTabla("ProyectoID") + ", " + IBD.GuidParamColumnaTabla("GadgetID") + ", @Idioma, @Contenido)");

            this.sqlProyectoGadgetIdiomaDelete = IBD.ReplaceParam("DELETE FROM ProyectoGadgetIdioma WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ") AND (GadgetID = " + IBD.GuidParamColumnaTabla("Original_GadgetID") + ") AND (Idioma = " + IBD.GuidParamColumnaTabla("Original_Idioma") + ") ");

            this.sqlProyectoGadgetIdiomaModify = IBD.ReplaceParam("UPDATE ProyectoGadgetIdioma SET OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + ", GadgetID = " + IBD.GuidParamColumnaTabla("GadgetID") + ", Idioma = @Idioma, Contenido = @Contenido  WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ") AND (GadgetID = " + IBD.GuidParamColumnaTabla("Original_GadgetID") + ") AND (Idioma = " + IBD.GuidParamColumnaTabla("Original_Idioma") + ") ");

            #endregion

            #region RecursosRelacionadosPresentacion

            this.sqlRecursosRelacionadosPresentacionInsert = IBD.ReplaceParam("INSERT INTO RecursosRelacionadosPresentacion (OrganizacionID, ProyectoID, OntologiaID,Orden, Ontologia,Propiedad,Nombre,Imagen) VALUES (" + IBD.GuidParamColumnaTabla("OrganizacionID") + ", " + IBD.GuidParamColumnaTabla("ProyectoID") + ", " + IBD.GuidParamColumnaTabla("OntologiaID") + ", @Orden, @Ontologia,@Propiedad,@Nombre,@Imagen)");

            this.sqlRecursosRelacionadosPresentacionDelete = IBD.ReplaceParam("DELETE FROM RecursosRelacionadosPresentacion WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ") AND (OntologiaID = " + IBD.GuidParamColumnaTabla("Original_OntologiaID") + ") AND (Orden = " + IBD.GuidParamColumnaTabla("Original_Orden") + ") ");

            this.sqlRecursosRelacionadosPresentacionModify = IBD.ReplaceParam("UPDATE RecursosRelacionadosPresentacion SET OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + ",OntologiaID = " + IBD.GuidParamColumnaTabla("OntologiaID") + ", Orden = @Orden, Ontologia=@Ontologia, Propiedad = @Propiedad, Nombre = @Nombre, Imagen = @Imagen WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ") AND (OntologiaID = " + IBD.GuidParamColumnaTabla("Original_OntologiaID") + ") AND (Orden = " + IBD.GuidParamColumnaTabla("Original_Orden") + ")");

            #endregion

            #region ProyectoPestanyaMenu

            this.sqlProyectoPestanyaMenuInsert = IBD.ReplaceParam("INSERT INTO ProyectoPestanyaMenu (PestanyaID,OrganizacionID,ProyectoID,PestanyaPadreID,TipoPestanya, Nombre, Ruta, Orden, NuevaPestanya, Visible, Privacidad, HtmlAlternativo,IdiomasDisponibles, Titulo, NombreCortoPestanya,VisibleSinAcceso, CSSBodyClass, Activa, MetaDescription) VALUES (" + IBD.GuidParamColumnaTabla("PestanyaID") + "," + IBD.GuidParamColumnaTabla("OrganizacionID") + "," + IBD.GuidParamColumnaTabla("ProyectoID") + "," + IBD.GuidParamColumnaTabla("PestanyaPadreID") + ",@TipoPestanya, @Nombre, @Ruta, @Orden,@NuevaPestanya, @Visible, @Privacidad, @HtmlAlternativo,@IdiomasDisponibles, @Titulo, @NombreCortoPestanya, @VisibleSinAcceso, @CSSBodyClass, @Activa, @MetaDescription)");

            this.sqlProyectoPestanyaMenuDelete = IBD.ReplaceParam("DELETE FROM ProyectoPestanyaMenu WHERE (PestanyaID = " + IBD.GuidParamColumnaTabla("Original_PestanyaID") + ")");

            this.sqlProyectoPestanyaMenuModify = IBD.ReplaceParam("UPDATE ProyectoPestanyaMenu SET  PestanyaID = " + IBD.GuidParamColumnaTabla("PestanyaID") + ",OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ",ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + ",PestanyaPadreID = " + IBD.GuidParamColumnaTabla("PestanyaPadreID") + ", TipoPestanya = @TipoPestanya, Nombre = @Nombre, Ruta = @Ruta, Orden = @Orden, NuevaPestanya = @NuevaPestanya, Visible = @Visible, Privacidad = @Privacidad, HtmlAlternativo = @HtmlAlternativo, IdiomasDisponibles=@IdiomasDisponibles, Titulo=@Titulo, NombreCortoPestanya=@NombreCortoPestanya, VisibleSinAcceso=@VisibleSinAcceso, CSSBodyClass=@CSSBodyClass, Activa=@Activa, MetaDescription = @MetaDescription WHERE (PestanyaID = " + IBD.GuidParamColumnaTabla("Original_PestanyaID") + ") ");

            #endregion

            #region ProyectoPestanyaBusqueda

            this.sqlProyectoPestanyaBusquedaInsert = IBD.ReplaceParam("INSERT INTO ProyectoPestanyaBusqueda (PestanyaID, CampoFiltro, NumeroRecursos, VistaDisponible, MostrarFacetas, MostrarCajaBusqueda, ProyectoOrigenID,OcultarResultadosSinFiltros, PosicionCentralMapa, GruposConfiguracion, GruposPorTipo,TextoBusquedaSinResultados, TextoDefectoBuscador, MostrarEnComboBusqueda, IgnorarPrivacidadEnBusqueda, OmitirCargaInicialFacetasResultados) VALUES (" + IBD.GuidParamColumnaTabla("PestanyaID") + ", @CampoFiltro, @NumeroRecursos, @VistaDisponible, @MostrarFacetas, @MostrarCajaBusqueda, " + IBD.GuidParamColumnaTabla("ProyectoOrigenID") + ", @OcultarResultadosSinFiltros, @PosicionCentralMapa, @GruposConfiguracion, @GruposPorTipo, @TextoBusquedaSinResultados, @TextoDefectoBuscador, @MostrarEnComboBusqueda, @IgnorarPrivacidadEnBusqueda, @OmitirCargaInicialFacetasResultados)");

            this.sqlProyectoPestanyaBusquedaDelete = IBD.ReplaceParam("DELETE FROM ProyectoPestanyaBusqueda WHERE (PestanyaID = " + IBD.GuidParamColumnaTabla("Original_PestanyaID") + ")");

            this.sqlProyectoPestanyaBusquedaModify = IBD.ReplaceParam("UPDATE ProyectoPestanyaBusqueda SET  PestanyaID = " + IBD.GuidParamColumnaTabla("PestanyaID") + ", CampoFiltro = @CampoFiltro, NumeroRecursos = @NumeroRecursos, VistaDisponible = @VistaDisponible, MostrarFacetas = @MostrarFacetas, MostrarCajaBusqueda = @MostrarCajaBusqueda, ProyectoOrigenID = @ProyectoOrigenID, OcultarResultadosSinFiltros = @OcultarResultadosSinFiltros, PosicionCentralMapa = @PosicionCentralMapa,  GruposPorTipo = @GruposPorTipo, GruposConfiguracion = @GruposConfiguracion, TextoBusquedaSinResultados=@TextoBusquedaSinResultados, TextoDefectoBuscador=@TextoDefectoBuscador, MostrarEnComboBusqueda=@MostrarEnComboBusqueda, IgnorarPrivacidadEnBusqueda=@IgnorarPrivacidadEnBusqueda, OmitirCargaInicialFacetasResultados=@OmitirCargaInicialFacetasResultados WHERE (PestanyaID = " + IBD.GuidParamColumnaTabla("Original_PestanyaID") + ")");

            #endregion

            #region ProyectoPestanyaCMS

            this.sqlProyectoPestanyaCMSInsert = IBD.ReplaceParam("INSERT INTO ProyectoPestanyaCMS (PestanyaID, Ubicacion) VALUES (" + IBD.GuidParamColumnaTabla("PestanyaID") + ", @Ubicacion)");

            this.sqlProyectoPestanyaCMSDelete = IBD.ReplaceParam("DELETE FROM ProyectoPestanyaCMS WHERE (PestanyaID = " + IBD.GuidParamColumnaTabla("Original_PestanyaID") + ") ");

            this.sqlProyectoPestanyaCMSModify = IBD.ReplaceParam("UPDATE ProyectoPestanyaCMS SET  PestanyaID = " + IBD.GuidParamColumnaTabla("PestanyaID") + ", Ubicacion = @Ubicacion WHERE (PestanyaID = " + IBD.GuidParamColumnaTabla("Original_PestanyaID") + ") ");

            #endregion

            #region ProyectoPestanyaMenuRolIdentidad

            this.sqlProyectoPestanyaMenuRolIdentidadInsert = IBD.ReplaceParam("INSERT INTO ProyectoPestanyaMenuRolIdentidad (PestanyaID, PerfilID) VALUES (" + IBD.GuidParamColumnaTabla("PestanyaID") + ", " + IBD.GuidParamColumnaTabla("PerfilID") + ")");

            this.sqlProyectoPestanyaMenuRolIdentidadDelete = IBD.ReplaceParam("DELETE FROM ProyectoPestanyaMenuRolIdentidad WHERE (PestanyaID = " + IBD.GuidParamColumnaTabla("Original_PestanyaID") + ") AND (PerfilID = " + IBD.GuidParamColumnaTabla("Original_PerfilID") + ") ");

            this.sqlProyectoPestanyaMenuRolIdentidadModify = IBD.ReplaceParam("UPDATE ProyectoPestanyaMenuRolIdentidad SET  PestanyaID = " + IBD.GuidParamColumnaTabla("PestanyaID") + ", PerfilID = " + IBD.GuidParamColumnaTabla("PerfilID") + " WHERE (PestanyaID = " + IBD.GuidParamColumnaTabla("Original_PestanyaID") + ") AND (PerfilID = " + IBD.GuidParamColumnaTabla("Original_PerfilID") + ")");

            #endregion

            #region ProyectoPestanyaMenuRolGrupoIdentidades

            this.sqlProyectoPestanyaMenuRolGrupoIdentidadesInsert = IBD.ReplaceParam("INSERT INTO ProyectoPestanyaMenuRolGrupoIdentidades (PestanyaID, GrupoID) VALUES (" + IBD.GuidParamColumnaTabla("PestanyaID") + ", " + IBD.GuidParamColumnaTabla("GrupoID") + ")");

            this.sqlProyectoPestanyaMenuRolGrupoIdentidadesDelete = IBD.ReplaceParam("DELETE FROM ProyectoPestanyaMenuRolGrupoIdentidades WHERE (PestanyaID = " + IBD.GuidParamColumnaTabla("Original_PestanyaID") + ")  AND (GrupoID = " + IBD.GuidParamColumnaTabla("Original_GrupoID") + ") ");

            this.sqlProyectoPestanyaMenuRolGrupoIdentidadesModify = IBD.ReplaceParam("UPDATE ProyectoPestanyaMenuRolGrupoIdentidades SET  PestanyaID = " + IBD.GuidParamColumnaTabla("PestanyaID") + ", PerfilID = " + IBD.GuidParamColumnaTabla("PerfilID") + " WHERE (PestanyaID = " + IBD.GuidParamColumnaTabla("Original_PestanyaID") + ") AND (GrupoID = " + IBD.GuidParamColumnaTabla("Original_GrupoID") + ")");

            #endregion

            #region ProyectoPaginaHtml

            this.sqlProyectoPaginaHtmlInsert = IBD.ReplaceParam("INSERT INTO ProyectoPaginaHtml (ProyectoID, Nombre, Html, Idioma) VALUES (" + IBD.GuidParamColumnaTabla("ProyectoID") + ", @Nombre, @Ruta, @Html, @Idioma)");

            this.sqlProyectoPaginaHtmlDelete = IBD.ReplaceParam("DELETE FROM ProyectoPaginaHtml WHERE (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ")  AND (Nombre = @Original_Nombre)");

            this.sqlProyectoPaginaHtmlModify = IBD.ReplaceParam("UPDATE ProyectoPaginaHtml SET  ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + ", Nombre = @Nombre, Html = @Html, Idioma = @Idioma WHERE (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ") AND (Nombre = @Original_Nombre)");

            #endregion

            #region ProyectoPerfilNumElem
            this.sqlProyectoPerfilNumElemInsert = IBD.ReplaceParam("INSERT INTO ProyectoPerfilNumElem (ProyectoID, PerfilID, NumRecursos) VALUES (" + IBD.GuidParamColumnaTabla("ProyectoID") + ", " + IBD.GuidParamColumnaTabla("PerfilID") + ", @NumRecursos)");
            this.sqlProyectoPerfilNumElemDelete = IBD.ReplaceParam("DELETE FROM ProyectoPerfilNumElem WHERE (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ") AND (PerfilID = " + IBD.GuidParamColumnaTabla("Original_PerfilID") + ") AND (NumRecursos = @Original_NumRecursos)");
            this.sqlProyectoPerfilNumElemModify = IBD.ReplaceParam("UPDATE ProyectoPerfilNumElem SET ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + ", PerfilID = " + IBD.GuidParamColumnaTabla("PerfilID") + ", NumRecursos = @NumRecursos WHERE (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ") AND (PerfilID = " + IBD.GuidParamColumnaTabla("Original_PerfilID") + ") AND (NumRecursos = @Original_NumRecursos)");
            #endregion

            #region SeccionProyCatalogo
            this.sqlSeccionProyCatalogoInsert = IBD.ReplaceParam("INSERT INTO SeccionProyCatalogo (OrganizacionID, ProyectoID, OrganizacionBusquedaID, ProyectoBusquedaID, Tipo, Nombre, Faceta, Filtro, NumeroResultados,Orden) VALUES (" + IBD.GuidParamColumnaTabla("OrganizacionID") + ", " + IBD.GuidParamColumnaTabla("ProyectoID") + ", " + IBD.GuidParamColumnaTabla("OrganizacionBusquedaID") + ", " + IBD.GuidParamColumnaTabla("ProyectoBusquedaID") + ", @Tipo, @Nombre, @Faceta, @Filtro, @NumeroResultados, @Orden)");
            this.sqlSeccionProyCatalogoDelete = IBD.ReplaceParam("DELETE FROM SeccionProyCatalogo WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ") AND (OrganizacionBusquedaID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionBusquedaID") + ") AND (ProyectoBusquedaID = " + IBD.GuidParamColumnaTabla("Original_ProyectoBusquedaID") + ") AND (Tipo = @Original_Tipo) AND (Nombre = @Original_Nombre) AND (Faceta = @Original_Faceta) AND (Filtro = @Original_Filtro OR @Original_Filtro IS NULL AND Filtro IS NULL) AND (NumeroResultados = @Original_NumeroResultados) AND (Orden = @Original_Orden)");
            this.sqlSeccionProyCatalogoModify = IBD.ReplaceParam("UPDATE SeccionProyCatalogo SET OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + ", OrganizacionBusquedaID = " + IBD.GuidParamColumnaTabla("OrganizacionBusquedaID") + ", ProyectoBusquedaID = " + IBD.GuidParamColumnaTabla("ProyectoBusquedaID") + ", Tipo = @Tipo, Nombre = @Nombre, Faceta = @Faceta, Filtro = @Filtro, NumeroResultados = @NumeroResultados, Orden = @Orden WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ") AND (OrganizacionBusquedaID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionBusquedaID") + ") AND (ProyectoBusquedaID = " + IBD.GuidParamColumnaTabla("Original_ProyectoBusquedaID") + ") AND (Tipo = @Original_Tipo) AND (Nombre = @Original_Nombre) AND (Faceta = @Original_Faceta) AND (Filtro = @Original_Filtro OR @Original_Filtro IS NULL AND Filtro IS NULL) AND (NumeroResultados = @Original_NumeroResultados) AND (Orden = @Original_Orden)");
            #endregion

            #region PresentacionListadoSemantico
            this.sqlPresentacionListadoSemanticoInsert = IBD.ReplaceParam("INSERT INTO PresentacionListadoSemantico (OrganizacionID, ProyectoID, OntologiaID, Orden, Ontologia, Propiedad, Nombre) VALUES (" + IBD.GuidParamColumnaTabla("OrganizacionID") + ", " + IBD.GuidParamColumnaTabla("ProyectoID") + ", " + IBD.GuidParamColumnaTabla("OntologiaID") + ", @Orden, @Ontologia, @Propiedad, @Nombre)");
            this.sqlPresentacionListadoSemanticoDelete = IBD.ReplaceParam("DELETE FROM PresentacionListadoSemantico WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ") AND (OntologiaID = " + IBD.GuidParamColumnaTabla("Original_OntologiaID") + ") AND (Orden = @Original_Orden)");
            this.sqlPresentacionListadoSemanticoModify = IBD.ReplaceParam("UPDATE PresentacionListadoSemantico SET OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + ", OntologiaID = " + IBD.GuidParamColumnaTabla("OntologiaID") + ", Orden = @Orden, Ontologia = @Ontologia, Propiedad = @Propiedad, Nombre = @Nombre WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ") AND (OntologiaID = " + IBD.GuidParamColumnaTabla("Original_OntologiaID") + ") AND (Orden = @Original_Orden)");
            #endregion

            #region PresentacionMosaicoSemantico
            this.sqlPresentacionMosaicoSemanticoInsert = IBD.ReplaceParam("INSERT INTO PresentacionMosaicoSemantico (OrganizacionID, ProyectoID, OntologiaID, Orden, Ontologia, Propiedad,Nombre) VALUES (" + IBD.GuidParamColumnaTabla("OrganizacionID") + ", " + IBD.GuidParamColumnaTabla("ProyectoID") + ", " + IBD.GuidParamColumnaTabla("OntologiaID") + ", @Orden, @Ontologia, @Propiedad,@Nombre)");
            this.sqlPresentacionMosaicoSemanticoDelete = IBD.ReplaceParam("DELETE FROM PresentacionMosaicoSemantico WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ") AND (OntologiaID = " + IBD.GuidParamColumnaTabla("Original_OntologiaID") + ") AND (Orden = @Original_Orden)");
            this.sqlPresentacionMosaicoSemanticoModify = IBD.ReplaceParam("UPDATE PresentacionMosaicoSemantico SET OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + ", OntologiaID = " + IBD.GuidParamColumnaTabla("OntologiaID") + ", Orden = @Orden, Ontologia = @Ontologia, Propiedad = @Propiedad, Nombre = @Nombre WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ") AND (OntologiaID = " + IBD.GuidParamColumnaTabla("Original_OntologiaID") + ") AND (Orden = @Original_Orden)");
            #endregion

            #region PresentacionMapaSemantico
            this.sqlPresentacionMapaSemanticoInsert = IBD.ReplaceParam("INSERT INTO PresentacionMapaSemantico (OrganizacionID, ProyectoID, OntologiaID, Orden, Ontologia, Propiedad,Nombre) VALUES (" + IBD.GuidParamColumnaTabla("OrganizacionID") + ", " + IBD.GuidParamColumnaTabla("ProyectoID") + ", " + IBD.GuidParamColumnaTabla("OntologiaID") + ", @Orden, @Ontologia, @Propiedad,@Nombre)");
            this.sqlPresentacionMapaSemanticoDelete = IBD.ReplaceParam("DELETE FROM PresentacionMapaSemantico WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ") AND (OntologiaID = " + IBD.GuidParamColumnaTabla("Original_OntologiaID") + ") AND (Orden = @Original_Orden)");
            this.sqlPresentacionMapaSemanticoModify = IBD.ReplaceParam("UPDATE PresentacionMapaSemantico SET OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + ", OntologiaID = " + IBD.GuidParamColumnaTabla("OntologiaID") + ", Orden = @Orden, Ontologia = @Ontologia, Propiedad = @Propiedad, Nombre = @Nombre WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ") AND (OntologiaID = " + IBD.GuidParamColumnaTabla("Original_OntologiaID") + ") AND (Orden = @Original_Orden)");
            #endregion

            #region PresentacionPestanyaListadoSemantico
            this.sqlPresentacionPestanyaListadoSemanticoInsert = IBD.ReplaceParam("INSERT INTO PresentacionPestanyaListadoSemantico (OrganizacionID, ProyectoID, PestanyaID, OntologiaID, Orden, Ontologia, Propiedad, Nombre) VALUES (" + IBD.GuidParamColumnaTabla("OrganizacionID") + ", " + IBD.GuidParamColumnaTabla("ProyectoID") + ", " + IBD.GuidParamColumnaTabla("PestanyaID") + ", " + IBD.GuidParamColumnaTabla("OntologiaID") + ", @Orden, @Ontologia, @Propiedad, @Nombre)");
            this.sqlPresentacionPestanyaListadoSemanticoDelete = IBD.ReplaceParam("DELETE FROM PresentacionPestanyaListadoSemantico WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ") AND (PestanyaID = " + IBD.GuidParamColumnaTabla("Original_PestanyaID") + ") AND (OntologiaID = " + IBD.GuidParamColumnaTabla("Original_OntologiaID") + ") AND (Orden = @Original_Orden)");
            this.sqlPresentacionPestanyaListadoSemanticoModify = IBD.ReplaceParam("UPDATE PresentacionPestanyaListadoSemantico SET OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + ", PestanyaID = " + IBD.GuidParamColumnaTabla("PestanyaID") + ", OntologiaID = " + IBD.GuidParamColumnaTabla("OntologiaID") + ", Orden = @Orden, Ontologia = @Ontologia, Propiedad = @Propiedad, Nombre = @Nombre WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ") AND (PestanyaID = " + IBD.GuidParamColumnaTabla("Original_PestanyaID") + ") AND (OntologiaID = " + IBD.GuidParamColumnaTabla("Original_OntologiaID") + ") AND (Orden = @Original_Orden)");
            #endregion

            #region PresentacionPestanyaMosaicoSemantico
            this.sqlPresentacionPestanyaMosaicoSemanticoInsert = IBD.ReplaceParam("INSERT INTO PresentacionPestanyaMosaicoSemantico (OrganizacionID, ProyectoID, PestanyaID, OntologiaID, Orden, Ontologia, Propiedad,Nombre) VALUES (" + IBD.GuidParamColumnaTabla("OrganizacionID") + ", " + IBD.GuidParamColumnaTabla("ProyectoID") + ", " + IBD.GuidParamColumnaTabla("PestanyaID") + ", " + IBD.GuidParamColumnaTabla("OntologiaID") + ", @Orden, @Ontologia, @Propiedad,@Nombre)");
            this.sqlPresentacionPestanyaMosaicoSemanticoDelete = IBD.ReplaceParam("DELETE FROM PresentacionPestanyaMosaicoSemantico WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ") AND (PestanyaID = " + IBD.GuidParamColumnaTabla("Original_PestanyaID") + ") AND (OntologiaID = " + IBD.GuidParamColumnaTabla("Original_OntologiaID") + ") AND (Orden = @Original_Orden)");
            this.sqlPresentacionPestanyaMosaicoSemanticoModify = IBD.ReplaceParam("UPDATE PresentacionPestanyaMosaicoSemantico SET OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + ", PestanyaID = " + IBD.GuidParamColumnaTabla("PestanyaID") + ", OntologiaID = " + IBD.GuidParamColumnaTabla("OntologiaID") + ", Orden = @Orden, Ontologia = @Ontologia, Propiedad = @Propiedad, Nombre = @Nombre WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ") AND (PestanyaID = " + IBD.GuidParamColumnaTabla("Original_PestanyaID") + ") AND (OntologiaID = " + IBD.GuidParamColumnaTabla("Original_OntologiaID") + ") AND (Orden = @Original_Orden)");
            #endregion

            #region PresentacionPestanyaMapaSemantico
            this.sqlPresentacionPestanyaMapaSemanticoInsert = IBD.ReplaceParam("INSERT INTO PresentacionPestanyaMapaSemantico (OrganizacionID, ProyectoID, PestanyaID, OntologiaID, Orden, Ontologia, Propiedad,Nombre) VALUES (" + IBD.GuidParamColumnaTabla("OrganizacionID") + ", " + IBD.GuidParamColumnaTabla("ProyectoID") + ", " + IBD.GuidParamColumnaTabla("PestanyaID") + ", " + IBD.GuidParamColumnaTabla("OntologiaID") + ", @Orden, @Ontologia, @Propiedad,@Nombre)");
            this.sqlPresentacionPestanyaMapaSemanticoDelete = IBD.ReplaceParam("DELETE FROM PresentacioPestanyanMapaSemantico WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ") AND (PestanyaID = " + IBD.GuidParamColumnaTabla("Original_PestanyaID") + ") AND (OntologiaID = " + IBD.GuidParamColumnaTabla("Original_OntologiaID") + ") AND (Orden = @Original_Orden)");
            this.sqlPresentacionPestanyaMapaSemanticoModify = IBD.ReplaceParam("UPDATE PresentacionPestanyaMapaSemantico SET OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + ", PestanyaID = " + IBD.GuidParamColumnaTabla("PestanyaID") + ", OntologiaID = " + IBD.GuidParamColumnaTabla("OntologiaID") + ", Orden = @Orden, Ontologia = @Ontologia, Propiedad = @Propiedad, Nombre = @Nombre WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ") AND (PestanyaID = " + IBD.GuidParamColumnaTabla("Original_PestanyaID") + ") AND (OntologiaID = " + IBD.GuidParamColumnaTabla("Original_OntologiaID") + ") AND (Orden = @Original_Orden)");
            #endregion

            #region ProyectoLoginConfiguracion
            this.sqlProyectoLoginConfiguracionInsert = IBD.ReplaceParam("INSERT INTO ProyectoLoginConfiguracion (OrganizacionID, ProyectoID, Mensaje) VALUES (" + IBD.GuidParamColumnaTabla("OrganizacionID") + ",@Mensaje)");
            this.sqlProyectoLoginConfiguracionDelete = IBD.ReplaceParam("DELETE FROM ProyectoLoginConfiguracion WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ")");
            this.sqlProyectoLoginConfiguracionModify = IBD.ReplaceParam("UPDATE ProyectoLoginConfiguracion SET OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + ", Mensaje=@Mensaje WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ") ");
            #endregion

            #region CamposRegistroProyectoGenericos
            this.sqlCamposRegistroProyectoGenericosInsert = IBD.ReplaceParam("INSERT INTO CamposRegistroProyectoGenericos (OrganizacionID, ProyectoID, Orden, Tipo) VALUES (" + IBD.GuidParamColumnaTabla("OrganizacionID") + "," + IBD.GuidParamColumnaTabla("ProyectoID") + ", @Orden, @Tipo)");
            this.sqlCamposRegistroProyectoGenericosDelete = IBD.ReplaceParam("DELETE FROM CamposRegistroProyectoGenericos WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ") AND (DatoExtraID = " + IBD.GuidParamColumnaTabla("Original_DatoExtraID") + ")");
            this.sqlCamposRegistroProyectoGenericosModify = IBD.ReplaceParam("UPDATE CamposRegistroProyectoGenericos SET OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + ", Orden=@Orden, Tipo=@Tipo WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ") AND (DatoExtraID = " + IBD.GuidParamColumnaTabla("Original_DatoExtraID") + ") ");
            #endregion

            #region DatoExtraProyecto
            this.sqlDatoExtraProyectoInsert = IBD.ReplaceParam("INSERT INTO DatoExtraProyecto (OrganizacionID, ProyectoID, DatoExtraID, Orden, Titulo, PredicadoRDF, Obligatorio,Paso1Registro) VALUES (" + IBD.GuidParamColumnaTabla("OrganizacionID") + "," + IBD.GuidParamColumnaTabla("ProyectoID") + "," + IBD.GuidParamColumnaTabla("DatoExtraID") + ", @Orden, @Titulo, @PredicadoRDF, @Obligatorio, @Paso1Registro)");
            this.sqlDatoExtraProyectoDelete = IBD.ReplaceParam("DELETE FROM DatoExtraProyecto WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ") AND (DatoExtraID = " + IBD.GuidParamColumnaTabla("Original_DatoExtraID") + ")");
            this.sqlDatoExtraProyectoModify = IBD.ReplaceParam("UPDATE DatoExtraProyecto SET OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + ", DatoExtraID = " + IBD.GuidParamColumnaTabla("DatoExtraID") + ", Orden=@Orden, Titulo=@Titulo, PredicadoRDF=@PredicadoRD, Obligatorio=@Obligatorio, Paso1Registro=@Paso1Registro WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ") AND (DatoExtraID = " + IBD.GuidParamColumnaTabla("Original_DatoExtraID") + ") ");
            #endregion

            #region DatoExtraProyectoOpcion
            this.sqlDatoExtraProyectoOpcionInsert = IBD.ReplaceParam("INSERT INTO DatoExtraProyectoOpcion (OrganizacionID, ProyectoID, DatoExtraID, OpcionID, Orden, Opcion) VALUES (" + IBD.GuidParamColumnaTabla("OrganizacionID") + "," + IBD.GuidParamColumnaTabla("ProyectoID") + "," + IBD.GuidParamColumnaTabla("DatoExtraID") + "," + IBD.GuidParamColumnaTabla("OpcionID") + ", @Orden, @Opcion)");
            this.sqlDatoExtraProyectoOpcionDelete = IBD.ReplaceParam("DELETE FROM DatoExtraProyectoOpcion WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ") AND (DatoExtraID = " + IBD.GuidParamColumnaTabla("Original_DatoExtraID") + ") AND (OpcionID = " + IBD.GuidParamColumnaTabla("Original_OpcionID") + ")");
            this.sqlDatoExtraProyectoOpcionModify = IBD.ReplaceParam("UPDATE DatoExtraProyectoOpcion SET OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + ", DatoExtraID = " + IBD.GuidParamColumnaTabla("DatoExtraID") + ",OpcionID=@OpcionID, Orden=@Orden, Opcion=@Opcion WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ") AND (DatoExtraID = " + IBD.GuidParamColumnaTabla("Original_DatoExtraID") + ") AND (OpcionID = " + IBD.GuidParamColumnaTabla("Original_OpcionID") + ") ");
            #endregion

            #region DatoExtraProyectoVirtuoso
            this.sqlDatoExtraProyectoVirtuosoInsert = IBD.ReplaceParam("INSERT INTO DatoExtraProyectoVirtuoso (OrganizacionID, ProyectoID, DatoExtraID, Orden, Titulo, InputID, InputsSuperiores, QueryVirtuoso, ConexionBD, Obligatorio, Paso1Registro, VisibilidadFichaPerfil, PredicadoRDF,NombreCampo, EstructuraHTMLFicha) VALUES (" + IBD.GuidParamColumnaTabla("OrganizacionID") + "," + IBD.GuidParamColumnaTabla("ProyectoID") + "," + IBD.GuidParamColumnaTabla("DatoExtraID") + ", @Orden, @Titulo, @InputID, @InputsSuperiores, @QueryVirtuoso, @ConexionBD, @Obligatorio, @VisibilidadFichaPerfil, @PredicadoRDF, @NombreCampo, @EstructuraHTMLFicha)");

            this.sqlDatoExtraProyectoVirtuosoDelete = IBD.ReplaceParam("DELETE FROM DatoExtraProyectoVirtuoso WHERE (DatoExtraID = " + IBD.GuidParamColumnaTabla("Original_DatoExtraID") + ")");

            this.sqlDatoExtraProyectoVirtuosoModify = IBD.ReplaceParam("UPDATE DatoExtraProyectoVirtuoso SET OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + ", DatoExtraID = " + IBD.GuidParamColumnaTabla("DatoExtraID") + ", Orden=@Orden, Titulo = @Titulo, InputID=@InputID, InputsSuperiores=@InputsSuperiores, QueryVirtuoso=@QueryVirtuoso, ConexionBD=@ConexionBD, Obligatorio=@Obligatorio, Paso1Registro=@Paso1Registro, VisibilidadFichaPerfil=@VisibilidadFichaPerfil,  PredicadoRDF=@PredicadoRDF,  NombreCampo=@NombreCampo, EstructuraHTMLFicha=@EstructuraHTMLFicha WHERE (DatoExtraID = " + IBD.GuidParamColumnaTabla("Original_DatoExtraID") + ") ");
            #endregion

            #region DatoExtraEcosistema
            this.sqlDatoExtraEcosistemaInsert = IBD.ReplaceParam("INSERT INTO DatoExtraEcosistema (  DatoExtraID, Orden, Titulo, PredicadoRDF, Obligatorio,Paso1Registro) VALUES (" + IBD.GuidParamColumnaTabla("DatoExtraID") + ", @Orden, @Titulo, @PredicadoRDF, @Obligatorio,@Paso1Registro)");
            this.sqlDatoExtraEcosistemaDelete = IBD.ReplaceParam("DELETE FROM DatoExtraEcosistema WHERE (DatoExtraID = " + IBD.GuidParamColumnaTabla("Original_DatoExtraID") + ")");
            this.sqlDatoExtraEcosistemaModify = IBD.ReplaceParam("UPDATE DatoExtraEcosistema SET DatoExtraID = " + IBD.GuidParamColumnaTabla("DatoExtraID") + ", Orden=@Orden, Titulo=@Titulo, PredicadoRDF=@PredicadoRD, Obligatorio=@Obligatorio,Paso1Registro=@Paso1Registro WHERE (DatoExtraID = " + IBD.GuidParamColumnaTabla("Original_DatoExtraID") + ") ");
            #endregion

            #region DatoExtraEcosistemaOpcion
            this.sqlDatoExtraEcosistemaOpcionInsert = IBD.ReplaceParam("INSERT INTO DatoExtraEcosistemaOpcion (DatoExtraID, OpcionID, Orden, Opcion) VALUES (" + IBD.GuidParamColumnaTabla("DatoExtraID") + "," + IBD.GuidParamColumnaTabla("OpcionID") + ", @Orden, @Opcion)");
            this.sqlDatoExtraEcosistemaOpcionDelete = IBD.ReplaceParam("DELETE FROM DatoExtraEcosistemaOpcion WHERE (DatoExtraID = " + IBD.GuidParamColumnaTabla("Original_DatoExtraID") + ") AND (OpcionID = " + IBD.GuidParamColumnaTabla("Original_OpcionID") + ")");
            this.sqlDatoExtraEcosistemaOpcionModify = IBD.ReplaceParam("UPDATE DatoExtraEcosistemaOpcion SET DatoExtraID = " + IBD.GuidParamColumnaTabla("DatoExtraID") + ",OpcionID=@OpcionID, Orden=@Orden, Opcion=@Opcion WHERE (DatoExtraID = " + IBD.GuidParamColumnaTabla("Original_DatoExtraID") + ") AND (OpcionID = " + IBD.GuidParamColumnaTabla("Original_OpcionID") + ") ");
            #endregion

            #region DatoExtraEcosistemaVirtuoso
            this.sqlDatoExtraEcosistemaVirtuosoInsert = IBD.ReplaceParam("INSERT INTO DatoExtraEcosistemaVirtuoso (DatoExtraID, Orden, Titulo, InputID, InputsSuperiores, QueryVirtuoso, ConexionBD, Obligatorio, Paso1Registro, VisibilidadFichaPerfil, PredicadoRDF, NombreCampo, EstructuraHTMLFicha) VALUES (" + IBD.GuidParamColumnaTabla("DatoExtraID") + ", @Orden, @Titulo, @InputID, @InputsSuperiores, @QueryVirtuoso, @ConexionBD, @Obligatorio, @Paso1Registro, @VisibilidadFichaPerfil, @PredicadoRDF, @NombreCampo, @EstructuraHTMLFicha)");

            this.sqlDatoExtraEcosistemaVirtuosoDelete = IBD.ReplaceParam("DELETE FROM DatoExtraEcosistemaVirtuoso WHERE (EcosistemaID = " + IBD.GuidParamColumnaTabla("Original_DatoExtraID") + ")");

            this.sqlDatoExtraEcosistemaVirtuosoModify = IBD.ReplaceParam("UPDATE DatoExtraEcosistemaVirtuoso SET DatoExtraID = " + IBD.GuidParamColumnaTabla("DatoExtraID") + ", Orden=@Orden, Titulo = @Titulo, InputID=@InputID, InputsSuperiores=@InputsSuperiores, QueryVirtuoso=@QueryVirtuoso, ConexionBD=@ConexionBD, Obligatorio=@Obligatorio, Paso1Registro=@Paso1Registro, VisibilidadFichaPerfil=@VisibilidadFichaPerfil, PredicadoRDF=@PredicadoRDF, NombreCampo=@NombreCampo, EstructuraHTMLFicha=@EstructuraHTMLFicha WHERE (DatoExtraID = " + IBD.GuidParamColumnaTabla("Original_DatoExtraID") + ") ");
            #endregion

            #region PreferenciaProyecto
            this.sqlPreferenciaProyectoInsert = IBD.ReplaceParam("INSERT INTO PreferenciaProyecto (OrganizacionID, ProyectoID, TesauroID, CategoriaTesauroID, Orden) VALUES (" + IBD.GuidParamColumnaTabla("OrganizacionID") + "," + IBD.GuidParamColumnaTabla("ProyectoID") + "," + IBD.GuidParamColumnaTabla("TesauroID") + "," + IBD.GuidParamColumnaTabla("CategoriaTesauroID") + ", @Orden)");
            this.sqlPreferenciaProyectoDelete = IBD.ReplaceParam("DELETE FROM PreferenciaProyecto WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ") AND (TesauroID = " + IBD.GuidParamColumnaTabla("Original_TesauroID") + ") AND (CategoriaTesauroID = " + IBD.GuidParamColumnaTabla("Original_CategoriaTesauroID") + ")");
            this.sqlPreferenciaProyectoModify = IBD.ReplaceParam("UPDATE PreferenciaProyecto SET OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + ", TesauroID = " + IBD.GuidParamColumnaTabla("TesauroID") + ", CategoriaTesauroID = " + IBD.GuidParamColumnaTabla("CategoriaTesauroID") + " WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ") AND (TesauroID = " + IBD.GuidParamColumnaTabla("Original_TesauroID") + " AND (CategoriaTesauroID = " + IBD.GuidParamColumnaTabla("Original_CategoriaTesauroID") + ") AND (Orden = " + IBD.GuidParamColumnaTabla("Original_Orden") + ") ");
            #endregion

            #region ProyetoEvento
            this.sqlProyectoEventoInsert = IBD.ReplaceParam("INSERT INTO ProyectoEvento (OrganizacionID, ProyectoID, EventoID, Nombre, Descripcion, TipoEvento, Activo,InfoExtra,Interno, ComponenteID, Grupo, UrlRedirect) VALUES (" + IBD.GuidParamColumnaTabla("OrganizacionID") + "," + IBD.GuidParamColumnaTabla("ProyectoID") + "," + IBD.GuidParamColumnaTabla("EventoID") + ",@Nombre, @Descripcion, @TipoEvento, @Activo,@InfoExtra,@Interno," + IBD.GuidParamColumnaTabla("ComponenteID") + ", @Grupo, @UrlRedirect)");
            this.sqlProyectoEventoDelete = IBD.ReplaceParam("DELETE FROM ProyectoEvento WHERE (EventoID = " + IBD.GuidParamColumnaTabla("Original_EventoID") + ")");
            this.sqlProyectoEventoModify = IBD.ReplaceParam("UPDATE ProyectoEvento SET OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + ", EventoID = " + IBD.GuidParamColumnaTabla("EventoID") + ", Nombre=@Nombre, Descripcion=@Descripcion, TipoEvento=@TipoEvento, Activo=@Activo, InfoExtra=@InfoExtra, Interno=@Interno " + ", ComponenteID = " + IBD.GuidParamColumnaTabla("ComponenteID") + ", Grupo=@Grupo, UrlRedirect=@UrlRedirect WHERE (EventoID = " + IBD.GuidParamColumnaTabla("Original_EventoID") + ") ");
            #endregion

            #region ProyectoEventoParticipante
            this.sqlProyectoEventoParticipanteInsert = IBD.ReplaceParam("INSERT INTO ProyectoEventoParticipante (IdentidadID, EventoID, Fecha) VALUES (" + IBD.GuidParamColumnaTabla("IdentidadID") + "," + IBD.GuidParamColumnaTabla("EventoID") + ",@Fecha)");
            this.sqlProyectoEventoParticipanteDelete = IBD.ReplaceParam("DELETE FROM ProyectoEventoParticipante WHERE (IdentidadID = " + IBD.GuidParamColumnaTabla("Original_IdentidadID") + " AND EventoID = " + IBD.GuidParamColumnaTabla("Original_EventoID") + ")");
            this.sqlProyectoEventoParticipanteModify = IBD.ReplaceParam("UPDATE ProyectoEventoParticipante SET IdentidadID = " + IBD.GuidParamColumnaTabla("IdentidadID") + ", EventoID = " + IBD.GuidParamColumnaTabla("EventoID") + " , Fecha=@Fecha WHERE (EventoID = " + IBD.GuidParamColumnaTabla("Original_EventoID") + " AND IdentidadID = " + IBD.GuidParamColumnaTabla("Original_IdentidadID") + " ) ");
            #endregion

            #region ProyetoEventoAccion
            this.sqlProyectoEventoAccionInsert = IBD.ReplaceParam("INSERT INTO ProyectoEventoAccion (OrganizacionID, ProyectoID, Evento, AccionJS) VALUES (" + IBD.GuidParamColumnaTabla("OrganizacionID") + "," + IBD.GuidParamColumnaTabla("ProyectoID") + ", @Evento, @AccionJS)");
            this.sqlProyectoEventoAccionDelete = IBD.ReplaceParam("DELETE FROM ProyectoEventoAccion WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionID") + "AND ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + "AND Evento=@Original_Evento)");
            this.sqlProyectoEventoAccionModify = IBD.ReplaceParam("UPDATE ProyectoEventoAccion SET OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + ", Evento=@Evento, AccionJS=@AccionJS WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionID") + "AND ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + "AND Evento=@Original_Evento)");
            #endregion

            #region ProyectoPasoRegistro

            this.sqlProyectoPasoRegistroInsert = IBD.ReplaceParam("INSERT INTO ProyectoPasoRegistro (OrganizacionID, ProyectoID, Orden, PasoRegistro) VALUES (" + IBD.GuidParamColumnaTabla("OrganizacionID") + "," + IBD.GuidParamColumnaTabla("ProyectoID") + ", @Orden, @PasoRegistro)");

            this.sqlProyectoPasoRegistroDelete = IBD.ReplaceParam("DELETE FROM ProyectoPasoRegistro WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionID") + ")  AND ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ")  AND (Orden = @Original_Orden)");

            this.sqlProyectoPasoRegistroModify = IBD.ReplaceParam("UPDATE ProyectoPasoRegistro SET  OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ",ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + ", Orden = @Orden, PasoRegistro = @PasoRegistro WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionID") + ") AND  (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ") AND (Orden = @Orden_Nombre)");

            #endregion

            #region AccionesExternasProyecto

            this.sqlAccionesExternasProyectoInsert = IBD.ReplaceParam("INSERT INTO AccionesExternasProyecto (TipoAccion, OrganizacionID, ProyectoID, URL) VALUES (@TipoAccion, " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", " + IBD.GuidParamColumnaTabla("ProyectoID") + ", @URL)");
            this.sqlAccionesExternasProyectoDelete = IBD.ReplaceParam("DELETE FROM AccionesExternasProyecto WHERE (TipoAccion = @O_TipoAccion) AND (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("O_ProyectoID") + ") AND (URL = @O_URL)");
            this.sqlAccionesExternasProyectoModify = IBD.ReplaceParam("UPDATE AccionesExternasProyecto SET TipoAccion = @TipoAccion, OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + ", URL = @URL WHERE (TipoAccion = @O_TipoAccion) AND (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("O_ProyectoID") + ") AND (URL = @O_URL)");

            #endregion

            #region ProyectoSearchPersonalizado

            this.sqlProyectoSearchPersonalizadoInsert = IBD.ReplaceParam("INSERT INTO ProyectoSearchPersonalizado (OrganizacionID, ProyectoID, NombreFaceta, WhereSPARQL, OrderBySPARQL,WhereFacetasSPARQL, OmitirRdfType) VALUES (" + IBD.GuidParamColumnaTabla("OrganizacionID") + ", " + IBD.GuidParamColumnaTabla("ProyectoID") + ", @NombreFaceta, @WhereSPARQL, @OrderBySPARQL, @WhereFacetasSPARQL, @OmitirRdfType) ");

            this.sqlProyectoSearchPersonalizadoDelete = IBD.ReplaceParam("DELETE FROM ProyectoSearchPersonalizado WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ") AND NombreFaceta = @NombreFaceta ");

            this.sqlProyectoSearchPersonalizadoModify = IBD.ReplaceParam("UPDATE ProyectoSearchPersonalizado SET OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + ", NombreFaceta = @NombreFaceta, WhereSPARQL = @WhereSPARQL, OrderBySPARQL = @OrderBySPARQL, WhereFacetasSPARQL = @WhereFacetasSPARQL, OmitirRdfType = @OmitirRdfType WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ") AND NombreFaceta = @NombreFaceta ");

            #endregion

            #region ProyectoConfigExtraSem
            this.sqlProyectoConfigExtraSemInsert = IBD.ReplaceParam("INSERT INTO ProyectoConfigExtraSem (ProyectoID, UrlOntologia, SourceTesSem, Tipo, Nombre, Idiomas, PrefijoTesSem, Editable) VALUES (" + IBD.GuidParamColumnaTabla("ProyectoID") + ", @UrlOntologia, @SourceTesSem, @Tipo, @Nombre, @Idiomas, @PrefijoTesSem, @Editable)");
            this.sqlProyectoConfigExtraSemDelete = IBD.ReplaceParam("DELETE FROM ProyectoConfigExtraSem WHERE (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ") AND (UrlOntologia = @Original_UrlOntologia) AND (SourceTesSem = @Original_SourceTesSem)");
            this.sqlProyectoConfigExtraSemModify = IBD.ReplaceParam("UPDATE ProyectoConfigExtraSem SET ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + ", UrlOntologia = @UrlOntologia, SourceTesSem = @SourceTesSem, Tipo = @Tipo, Nombre = @Nombre, Idiomas = @Idiomas, PrefijoTesSem = @PrefijoTesSem, Editable = @Editable WHERE (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ") AND (UrlOntologia = @Original_UrlOntologia) AND (SourceTesSem = @Original_SourceTesSem)");
            #endregion

            #region ProyectoServicioExterno

            this.sqlProyectoServicioExternoInsert = IBD.ReplaceParam("INSERT INTO ProyectoServicioExterno (OrganizacionID, ProyectoID, NombreServicio, UrlServicio) VALUES (" + IBD.GuidParamColumnaTabla("OrganizacionID") + ", " + IBD.GuidParamColumnaTabla("ProyectoID") + ", @NombreServicio, @UrlServicio)");

            this.sqlProyectoServicioExternoDelete = IBD.ReplaceParam("DELETE FROM ProyectoServicioExterno WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("O_ProyectoID") + ") AND (NombreServicio = @O_NombreServicio)");

            this.sqlProyectoServicioExternoModify = IBD.ReplaceParam("UPDATE ProyectoServicioExterno SET OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + ", NombreServicio = @NombreServicio, UrlServicio = @UrlServicio WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("O_ProyectoID") + ") AND (NombreServicio = @O_NombreServicio)");

            #endregion

            #region EcosistemaServicioExterno

            this.sqlEcosistemaServicioExternoInsert = IBD.ReplaceParam("INSERT INTO EcosistemaServicioExterno (NombreServicio, UrlServicio) VALUES (@NombreServicio, @UrlServicio)");

            this.sqlEcosistemaServicioExternoDelete = IBD.ReplaceParam("DELETE FROM EcosistemaServicioExterno WHERE (NombreServicio = @O_NombreServicio)");

            this.sqlEcosistemaServicioExternoModify = IBD.ReplaceParam("UPDATE EcosistemaServicioExterno SET NombreServicio = @NombreServicio, UrlServicio = @UrlServicio WHERE (NombreServicio = @O_NombreServicio)");

            #endregion

            #region OntologiaProyecto
            this.sqlOntologiaProyectoInsert = IBD.ReplaceParam("INSERT INTO OntologiaProyecto (OrganizacionID, ProyectoID, OntologiaProyecto, NombreOnt, Namespace, NamespacesExtra, SubTipos, NombreCortoOnt, CachearDatosSemanticos, EsBuscable) VALUES (" + IBD.GuidParamColumnaTabla("OrganizacionID") + ", " + IBD.GuidParamColumnaTabla("ProyectoID") + ", @OntologiaProyecto, @NombreOnt, @Namespace, @NamespacesExtra, @SubTipos, @NombreCortoOnt, @CachearDatosSemanticos, @EsBuscable)");
            this.sqlOntologiaProyectoDelete = IBD.ReplaceParam("DELETE FROM OntologiaProyecto WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ") AND (OntologiaProyecto = @Original_OntologiaProyecto)");
            this.sqlOntologiaProyectoModify = IBD.ReplaceParam("UPDATE OntologiaProyecto SET OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + ", OntologiaProyecto = @OntologiaProyecto, NombreOnt = @NombreOnt, Namespace = @Namespace, NamespacesExtra = @NamespacesExtra, SubTipos = @SubTipos, NombreCortoOnt = @NombreCortoOnt, CachearDatosSemanticos = @CachearDatosSemanticos, EsBuscable = @EsBuscable WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ") AND (OntologiaProyecto = @Original_OntologiaProyecto)");
            #endregion

            #region ConfigAutocompletarProy

            this.sqlConfigAutocompletarProyInsert = IBD.ReplaceParam("INSERT INTO ConfigAutocompletarProy (OrganizacionID, ProyectoID, Clave, Valor,PestanyaID) VALUES (@OrganizacionID, @ProyectoID, @Clave, @Valor, @PestanyaID)");
            this.sqlConfigAutocompletarProyDelete = IBD.ReplaceParam("DELETE FROM ConfigAutocompletarProy WHERE (OrganizacionID = @Original_OrganizacionID) AND (ProyectoID = @Original_ProyectoID) AND (Clave = @Original_Clave)");
            this.sqlConfigAutocompletarProyModify = IBD.ReplaceParam("UPDATE ConfigAutocompletarProy SET OrganizacionID = @OrganizacionID, ProyectoID = @ProyectoID, Clave = @Clave, Valor = @Valor, PestanyaID = @PestanyaID WHERE (OrganizacionID = @Original_OrganizacionID) AND (ProyectoID = @Original_ProyectoID) AND (Clave = @Original_Clave)");

            #endregion ConfigAutocompletarProy

            #endregion

            #endregion
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene el identificador de myGnoss (todo unos)
        /// </summary>
        public static Guid MyGnoss
        {
            get
            {
                return mMyGnoss;
            }
        }

        /// <summary>
        /// Obtiene el identificador de la metaorganización
        /// </summary>
        public static Guid MetaOrganizacion
        {
            get
            {
                return mMetaOrganizacion;
            }
            set
            {
                mMetaOrganizacion = value;
            }
        }

        /// <summary>
        /// Obtiene el identificador del metaproyecto
        /// </summary>
        public static Guid MetaProyecto
        {
            get
            {
                return mMetaProyecto;
            }
            set
            {
                mMetaProyecto = value;
            }
        }

        /// <summary>
        /// Obtiene el identificador de la tabla base del metaproyecto
        /// </summary>
        public static int TablaBaseIdMetaProyecto
        {
            get
            {
                return mTablaBaseIdMetaProyecto;
            }
            set
            {
                mTablaBaseIdMetaProyecto = value;
            }
        }

        /// <summary>
        /// Obtiene el identificador del proyecto MetaGNOSS
        /// </summary>
        public static Guid MetaGNOSS
        {
            get
            {
                return new Guid("11111111-1111-1111-1111-111111111114");
            }
        }

        /// <summary>
        /// Obtiene el identificador del proyecto FAQ
        /// </summary>
        public static Guid ProyectoFAQ
        {
            get
            {
                return new Guid("11111111-1111-1111-1111-111111111112");
            }
        }

        /// <summary>
        /// Obtiene el identificador del proyecto Didactalia
        /// </summary>
        public static Guid ProyectoDidactalia
        {
            get
            {
                return new Guid("f22e757b-8116-4496-bec4-ae93a4792c28");
            }
        }

        /// <summary>
        /// Obtiene el identificador del proyecto Noticias
        /// </summary>
        public static Guid ProyectoNoticias
        {
            get
            {
                return new Guid("11111111-1111-1111-1111-111111111113");
            }
        }

        /// <summary>
        /// Obtiene el color azul para los logos pequeños cuando seleccionen imágenes genéricas para el proyecto
        /// </summary>
        public static Color COLOR_AZUL
        {
            get
            {
                return Color.FromArgb(38, 153, 182);
            }
        }

        /// <summary>
        /// Obtiene el color lila para los logos pequeños cuando seleccionen imágenes genéricas para el proyecto
        /// </summary>
        public static Color COLOR_LILA
        {
            get
            {
                return Color.FromArgb(116, 51, 131);
            }
        }

        /// <summary>
        /// Obtiene el color marrón para los logos pequeños cuando seleccionen imágenes genéricas para el proyecto
        /// </summary>
        public static Color COLOR_MARRON
        {
            get
            {
                return Color.FromArgb(196, 135, 54);
            }
        }

        /// <summary>
        /// Obtiene el color morado para los logos pequeños cuando seleccionen imágenes genéricas para el proyecto
        /// </summary>
        public static Color COLOR_MORADO
        {
            get
            {
                return Color.FromArgb(126, 135, 190);
            }
        }

        /// <summary>
        /// Obtiene el color rosa para los logos pequeños cuando seleccionen imágenes genéricas para el proyecto
        /// </summary>
        public static Color COLOR_ROSA
        {
            get
            {
                return Color.FromArgb(242, 120, 143);
            }
        }

        /// <summary>
        /// Obtiene el color salmón para los logos pequeños cuando seleccionen imágenes genéricas para el proyecto
        /// </summary>
        public static Color COLOR_SALMON
        {
            get
            {
                return Color.FromArgb(236, 129, 77);
            }
        }

        #endregion


    }

    public class JoinProyectoOrganizacionParticipaProy
    {
        public Proyecto Proyecto { get; set; }
        public OrganizacionParticipaProy OrganizacionParticipaProy{ get; set; }
    }

    public class JoinProyectoAdministradorProyecto
    {
        public Proyecto Proyecto { get; set; }
        public AdministradorProyecto AdministradorProyecto { get; set; }
    }

    public class JoinProyectoAdministradorProyectoPersona
    {
        public Proyecto Proyecto { get; set; }
        public AdministradorProyecto AdministradorProyecto { get; set; }
        public Persona Persona { get; set; }
    }

    public class JoinProyectoAdministradorProyectoPersonaPerfil
    {
        public Proyecto Proyecto { get; set; }
        public AdministradorProyecto AdministradorProyecto { get; set; }
        public Persona Persona { get; set; }
        public Perfil Perfil { get; set; }
    }

    public class JoinProyectoAdministradorProyectoPersonaPerfilIdentidad
    {
        public Proyecto Proyecto { get; set; }
        public AdministradorProyecto AdministradorProyecto { get; set; }
        public Persona Persona { get; set; }
        public Perfil Perfil { get; set; }
        public Es.Riam.Gnoss.AD.EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
    }

    public class JoinProyectoOrganizacionParticipaProyOrganizacion
    {
        public Proyecto Proyecto { get; set; }
        public OrganizacionParticipaProy OrganizacionParticipaProy { get; set; }
        public Organizacion Organizacion { get; set; }
    }

    public class JoinProyectoOrganizacionParticipaProyOrganizacionPais
    {
        public Proyecto Proyecto { get; set; }
        public OrganizacionParticipaProy OrganizacionParticipaProy { get; set; }
        public Organizacion Organizacion { get; set; }
        public AD.EntityModel.Models.Pais.Pais Pais { get; set; }
    }

    public class JoinProyectoOrganizacionParticipaProyOrganizacionPaisPerfil
    {
        public Proyecto Proyecto { get; set; }
        public OrganizacionParticipaProy OrganizacionParticipaProy { get; set; }
        public Organizacion Organizacion { get; set; }
        public AD.EntityModel.Models.Pais.Pais Pais { get; set; }
        public Perfil Perfil { get; set; }
    }

    public class JoinProyectoOrganizacionParticipaProyOrganizacionPaisPerfilIdentidad
    {
        public Proyecto Proyecto { get; set; }
        public OrganizacionParticipaProy OrganizacionParticipaProy { get; set; }
        public Organizacion Organizacion { get; set; }
        public AD.EntityModel.Models.Pais.Pais Pais { get; set; }
        public Perfil Perfil { get; set; }
        public AD.EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
    }

    public class JoinProyectoOrganizacionParticipaProyPerfil
    {
        public Proyecto Proyecto
        {
            get; set;
        }
        public OrganizacionParticipaProy OrganizacionParticipaProy
        {
            get; set;
        }

        public Perfil Perfil
        {
            get; set;
        }
    }

    public class JoinProyectoOrganizacionParticipaProyPerfilIdentidad
    {
        public Proyecto Proyecto
        {
            get; set;
        }
        public OrganizacionParticipaProy OrganizacionParticipaProy
        {
            get; set;
        }

        public Perfil Perfil
        {
            get; set;
        }

        public AD.EntityModel.Models.IdentidadDS.Identidad Identidad
        {
            get; set;
        }
    }

    public class JoinProyectoOrganizacionParticipaProyPerfilIdentidadPerfil
    {
        public Proyecto Proyecto
        {
            get; set;
        }
        public OrganizacionParticipaProy OrganizacionParticipaProy
        {
            get; set;
        }

        public Perfil Perfil
        {
            get; set;
        }

        public AD.EntityModel.Models.IdentidadDS.Identidad Identidad
        {
            get; set;
        }

        public Perfil Perfil2
        {
            get; set;
        }
    }

    public class JoinProyectoOrganizacionParticipaProyPerfilIdentidadPerfilPersona
    {
        public Proyecto Proyecto
        {
            get; set;
        }
        public OrganizacionParticipaProy OrganizacionParticipaProy
        {
            get; set;
        }

        public Perfil Perfil
        {
            get; set;
        }

        public AD.EntityModel.Models.IdentidadDS.Identidad Identidad
        {
            get; set;
        }

        public Perfil Perfil2
        {
            get; set;
        }

        public Persona Persona
        {
            get; set;
        }
    }

    public class JoinProyectoOrganizacionParticipaProyPerfilIdentidadPerfilPersonaAdministradorProyecto
    {
        public Proyecto Proyecto
        {
            get; set;
        }
        public OrganizacionParticipaProy OrganizacionParticipaProy
        {
            get; set;
        }

        public Perfil Perfil
        {
            get; set;
        }

        public AD.EntityModel.Models.IdentidadDS.Identidad Identidad
        {
            get; set;
        }

        public Perfil Perfil2
        {
            get; set;
        }

        public Persona Persona
        {
            get; set;
        }

        public AdministradorProyecto AdministradorProyecto
        {
            get; set;
        }

    }



}

