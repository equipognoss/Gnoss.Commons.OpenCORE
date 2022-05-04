using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.ParametroGeneralDS;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Es.Riam.Gnoss.AD.ParametrosProyecto
{

    #region Enumeraciones

    public enum TipoPaginaMetaRobots
    {
        Pagina = 0,
        Perfil = 1,
        Persona = 2,
        BusquedaPrimeraPagina = 3,
        BusquedaPaginada = 4
    }

    /// <summary>
    /// Enumeración de ubicaciónes de trozos de html dentro de la página
    /// </summary>
    public enum UbicacionHtmlProyecto
    {
        /// <summary>
        /// Al final de la etiqueta head
        /// </summary>
        FinHead = 0,

        /// <summary>
        /// Al final de la etiqueta body
        /// </summary>
        FinBody = 1,

        /// <summary>
        /// Al inicio de la etiqueta head
        /// </summary>
        InicioHead = 2,

        /// <summary>
        /// Al inicio de la etiqueta body
        /// </summary>
        InicioBody = 3
    }

    public enum PlataformaVideoDisponible
    {
        /// <summary>
        /// No tiene ninguna plataforma especial disponible
        /// </summary>
        Ninguna = 0,
        /// <summary>
        /// Brightcove
        /// </summary>
        Brightcove = 1,
        /// <summary>
        /// TOP
        /// </summary>
        TOP = 2
    }

    #endregion

    /// <summary>
    /// Data adapter de parámetros generales
    /// </summary>
    public class ParametroGeneralAD : BaseAD
    {
        #region Consultas

        #region Campos de tablas

        string selectCamposAnalisisCompletitud;
        string selectParametroGeneralPesado;
        string selectParametroGeneralLigero;
        string selectConfiguracionAmbitoBusquedaProyecto;
        string selectProyectoMetaRobots;
        string selectProyectoRDFType;
        string selectTextosPersonalizadosProyecto;
        string selectTextosPersonalizadosPersonalizacion;

        #endregion

        string sqlSelectCamposAnalisisCompletitudDeProyecto;
        string sqlSelectConfiguracionAmbitoBusquedaProyecto;
        string sqlSelectParametrosGeneralesDeProyecto;
        string sqlSelectParametrosGeneralesDeProyectoCargaLigera;
        string sqlSelectProyectoMetaRobots;
        string sqlSelectProyectoRDFType;
        string sqlSelectTextosPersonalizadosProyecto;
        string sqlSelectTextosPersonalizadosPersonalizacionProyecto;
        string sqlSelectTextosPersonalizadosPersonalizacionPersonalizacionID;

        #endregion

        #region DataAdapter

        #region ProyectoMetaRobots

        string sqlProyectoMetaRobotsInsert;
        string sqlProyectoMetaRobotsDelete;
        string sqlProyectoMetaRobotsModify;

        #endregion

        #region CamposAnalisisCompletitud

        string sqlCamposAnalisisCompletitudInsert;
        string sqlCamposAnalisisCompletitudDelete;
        string sqlCamposAnalisisCompletitudModify;

        #endregion

        #region ParametroGeneral

        string sqlParametroGeneralInsert;

        string sqlParametroGeneralDelete;

        string sqlParametroGeneralModify;

        #endregion

        #region ConfiguracionAmbitoBusquedaProyecto

        string sqlConfiguracionAmbitoBusquedaProyectoInsert;
        string sqlConfiguracionAmbitoBusquedaProyectoDelete;
        string sqlConfiguracionAmbitoBusquedaProyectoModify;

        #endregion

        #region TextosPersonalizadosProyecto

        string sqlTextosPersonalizadosProyectoInsert;
        string sqlTextosPersonalizadosProyectoDelete;
        string sqlTextosPersonalizadosProyectoModify;

        #endregion

        #region TextosPersonalizadosPersonalizacion

        string sqlTextosPersonalizadosPersonalizacionInsert;
        string sqlTextosPersonalizadosPersonalizacionDelete;
        string sqlTextosPersonalizadosPersonalizacionModify;

        #endregion

        #region ParametroProyecto

        string sqlParametroProyectoInsert;
        string sqlParametroProyectoDelete;
        string sqlParametroProyectoModify;

        #endregion

        #endregion

        #region Constructor

        private EntityContext mEntityContext;

        /// <summary>
        /// Constructor sin parámetros
        /// </summary>
        public ParametroGeneralAD(LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication)
        {
            mEntityContext = entityContext;
            CargarConsultasYDataAdapters(IBD);
        }

        /// <summary>
        /// Cuando se desea pasar directamente la ruta del fichero de configuracion de conexion a la Base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD"></param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public ParametroGeneralAD(string pFicheroConfiguracionBD, LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication)
        {
            mEntityContext = entityContext;
            this.CargarConsultasYDataAdapters(IBD);
        }

        #endregion

        #region Métodos generales

        #region Públicos

        /// <summary>
        /// Obtiene los textos personalizados de un proyecto de una organización
        /// </summary>
        /// <param name="pPersonalizacionID">Identificador del proyecto</param>
        /// <returns>Dataset de parámetros generales</returns>
        // public ParametroGeneralDS ObtenerTextosPersonalizacionProyecto(Guid pProyectoID)
        public List<TextosPersonalizadosPersonalizacion> ObtenerTextosPersonalizacionProyecto(Guid pProyectoID)
        {
            var textos = mEntityContext.TextosPersonalizadosPersonalizacion.Join(mEntityContext.VistaVirtualProyecto, textoPersonalizacion => textoPersonalizacion.PersonalizacionID, vistaVirtualProyecto => vistaVirtualProyecto.PersonalizacionID, (textoPersonalizacion, textoProyecto) => new
            {
                VistaVirtualProyecto = textoProyecto,
                TextoPersonalizacion = textoPersonalizacion
            }).Where(item => item.VistaVirtualProyecto.ProyectoID.Equals(pProyectoID)).Select(item => item.TextoPersonalizacion);

            var textosPers = textos.ToList();
            //foreach(var texto in textos)
            //{
            //    TextosPersonalizadosPersonalizacion textosPersonalizados = new TextosPersonalizadosPersonalizacion(texto.PersonalizacionID, texto.TextoID, texto.Language, texto.Texto);
            //    listaTextos.Add(textosPersonalizados);
            //}
            return textosPers;
        }

        public List<TextosPersonalizadosPersonalizacion> ObtenerTextosPersonalizadosDominio(string pDominio, Guid pPersonalizacionEcosistema)
        {
            List<TextosPersonalizadosPersonalizacion> listaTextos = new List<TextosPersonalizadosPersonalizacion>();

            if (string.IsNullOrEmpty(pDominio))
            {
                listaTextos = mEntityContext.TextosPersonalizadosPersonalizacion.ToList();
            }
            else
            {
                listaTextos = mEntityContext.TextosPersonalizadosPersonalizacion.Where(texto => texto.PersonalizacionID.Equals(pPersonalizacionEcosistema)).Select(item => item).Concat(mEntityContext.TextosPersonalizadosPersonalizacion.Join(mEntityContext.VistaVirtualProyecto, textoPersonalizacion => textoPersonalizacion.PersonalizacionID, vistaVirtualProyecto => vistaVirtualProyecto.PersonalizacionID, (textoPersonalizacion, vistaVirtualProyecto) => new
                {
                    TextoPersonalizacion = textoPersonalizacion,
                    VistaVirtualProyecto = vistaVirtualProyecto
                }).Join(mEntityContext.Proyecto, vistaVirtualProyecto => vistaVirtualProyecto.VistaVirtualProyecto.ProyectoID, proyecto => proyecto.ProyectoID, (textoPersonalizacion, proyecto) => new
                {
                    TextoPersonalizacion = textoPersonalizacion.TextoPersonalizacion,
                    Proyecto = proyecto
                }).Where(proyecto => proyecto.Proyecto.URLPropia.Contains("http://" + pDominio) || proyecto.Proyecto.URLPropia.Contains("https://" + pDominio) || proyecto.Proyecto.URLPropia.Contains("://" + pDominio + "@")).Select(item => item.TextoPersonalizacion)).ToList().Distinct().ToList();
            }

            return listaTextos;
        }

        public List<TextosPersonalizadosProyecto> ObtenerTextosPersonalizadosProyecto(string pDominio)
        {
            List<TextosPersonalizadosProyecto> listaTextosPersonalizados = new List<TextosPersonalizadosProyecto>();

            if (string.IsNullOrEmpty(pDominio))
            {
                listaTextosPersonalizados = mEntityContext.TextosPersonalizadosProyecto.ToList();
            }
            else
            {
                listaTextosPersonalizados = mEntityContext.TextosPersonalizadosProyecto.Join(mEntityContext.Proyecto, textosPersonalizadosProyecto => textosPersonalizadosProyecto.ProyectoID, proyecto => proyecto.ProyectoID, (textoPersonalizacion, proyecto) => new
                {
                    TextoPersonalizacion = textoPersonalizacion,
                    Proyecto = proyecto
                }).Where(item => item.Proyecto.URLPropia.Contains("http://" + pDominio) || item.Proyecto.URLPropia.Contains("https://" + pDominio) || item.Proyecto.URLPropia.Contains("://" + pDominio + "@")).Select(item => item.TextoPersonalizacion).ToList();
            }

            return listaTextosPersonalizados;
        }

        public void VersionarCSSYJS(Guid pProyectoID)
        {
            ParametroGeneral parametro = new ParametroGeneral();

            parametro = mEntityContext.ParametroGeneral.Where(p => p.ProyectoID == pProyectoID).FirstOrDefault();
            if (parametro.VersionCSS == null)
            {
                parametro.VersionCSS = 1;
            }
            else
            {
                parametro.VersionCSS = parametro.VersionCSS + 1;
            }

            if (parametro.VersionJS == null)
            {
                parametro.VersionJS = 1;
            }
            else
            {
                parametro.VersionJS = parametro.VersionJS + 1;
            }

            mEntityContext.SaveChanges();

        }

        public List<TextosPersonalizadosPlataforma> ObtenerTextosPersonalizadosPlataforma()
        {
            return mEntityContext.TextosPersonalizadosPlataforma.ToList();
        }

        /// <summary>
        /// Obtiene la política de certificación de un proyecto.
        /// </summary>
        /// <param name="pProyectoID">ID de proyecto</param>
        /// <returns>String con la política de certificación</returns>
        public string ObtenerPoliticaCertificacionDeProyecto(Guid pProyectoID)
        {
            return mEntityContext.ParametroGeneral.FirstOrDefault(parametrogeneral => parametrogeneral.ProyectoID.Equals(pProyectoID))?.PoliticaCertificacion;
        }

        /// <summary>
        /// Obtiene la fila con los parámetros generales de un proyecto pasado por parámetro (CARGA LIGERA)
        /// </summary>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <returns>Fila de párametros generales</returns>
        public ParametroGeneral ObtenerFilaParametrosGeneralesDeProyecto(Guid pProyectoID)
        {
            ParametroGeneral busqueda = mEntityContext.ParametroGeneral.Where(parametroG => parametroG.ProyectoID.Equals(pProyectoID)).FirstOrDefault();

            return busqueda;
        }

        public IEnumerable<ProyectoElementoHtml> ObtenerFilaProyectoElementoHtml(Guid pProyectoID)
        {
            return mEntityContext.ProyectoElementoHtml.Where(parametroG => parametroG.ProyectoID.Equals(pProyectoID)).ToList();
        }

        public IEnumerable<ProyectoMetaRobots> ObtenerProyectoMetaRobots(Guid pProyectoID)
        {
            return mEntityContext.ProyectoMetaRobots.Where(parametroG => parametroG.ProyectoID.Equals(pProyectoID)).ToList();
        }

        public ParametroGeneral ObtenerFilaParametrosGeneralesDeProyectoNuevo(Guid pProyectoID)
        {
            ParametroGeneral busqueda = mEntityContext.ParametroGeneral.Where(parametroG => parametroG.ProyectoID.Equals(pProyectoID)).FirstOrDefault();

            return busqueda;
        }

        public List<TextosPersonalizadosPersonalizacion> ObtenerTextosPersonalizadosPersonalizacionEcosistema(Guid pPersonalizacionEcosistemaID)
        {
            return mEntityContext.TextosPersonalizadosPersonalizacion.Where(textosPersonalizadosPersonalizacion => textosPersonalizadosPersonalizacion.PersonalizacionID.Equals(pPersonalizacionEcosistemaID)).ToList();
        }

        /// <summary>
        /// Obtiene el texto de una clave concreta de personalizacion
        /// </summary>
        /// <param name="pPersonalizacionID">Identificador de la personalizacion</param>
        /// <param name="pClaveIdioma">Clave del idioma (es, en...)</param>
        /// <param name="pTextoID">Identificador del texto</param>
        /// <returns></returns>
        public string ObtenerTextoPersonalizadoPersonalizacion(Guid pPersonalizacionID, string pClaveIdioma, string pTextoID)
        {
            return mEntityContext.TextosPersonalizadosPersonalizacion.FirstOrDefault(texto => texto.PersonalizacionID.Equals(pPersonalizacionID) && texto.TextoID.Equals(pTextoID) && texto.Language.Equals(pClaveIdioma)).Texto;
        }

        /// <summary>
        /// Obtiene el DS ProyectoRDFType
        /// </summary>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <param name="pTipoDoc">Tipo de documento que se necesita</param>
        /// <returns>DS de ProyectoRDFType</returns>
        //public ParametroGeneralDS ObtenerProyectoRDFType(Guid pProyectoID, int pTipoDoc)
        public List<ProyectoRDFType> ObtenerProyectoRDFType(Guid pProyectoID, int pTipoDoc)
        {
            List<ProyectoRDFType> proyectosRDFType = mEntityContext.ProyectoRDFType.Where(proyectoRDF => proyectoRDF.ProyectoID.Equals(pProyectoID) && proyectoRDF.TipoDocumento.Equals((short)pTipoDoc)).ToList();
            return proyectosRDFType;

        }
        /// <summary>
        /// Comprueba si un proyecto tiene establecida la imagen de la home
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns></returns>
        public bool TieneProyectoImagenHome(Guid pProyectoID)
        {
            return mEntityContext.ParametroGeneral.Any(param => param.ProyectoID.Equals(pProyectoID) && param.ImagenHome != null);
        }

        /// <summary>
        /// Obtiene los parámetros generales de una lista de proyectos (CARGA LIGERA)
        /// </summary>
        /// <param name="pListaProyectos">Lista de identificadores de proyecto</param>
        /// <returns>Dataset de párametros generales</returns>
        //public ParametroGeneralDS ObtenerParametrosGeneralesDeListaDeProyectos(List<Guid> pListaProyectos)
        public List<ParametroGeneral> ObtenerParametrosGeneralesDeListaDeProyectos(List<Guid> pListaProyectos)
        {
            List<ParametroGeneral> parametrosGeneral = mEntityContext.ParametroGeneral.Where(parametroGeneral => pListaProyectos.Contains(parametroGeneral.ProyectoID)).ToList();
            return parametrosGeneral;
        }

        /// <summary>
        /// Obtiene las licencias de parametros generales de los proyectos (Proy,licencia).
        /// </summary>
        /// <param name="pListaProyectosID">Identificadores de proyecto</param>
        public Dictionary<Guid, string> ObtenerLicenciasporProyectosID(List<Guid> pListaProyectosID)
        {
            return mEntityContext.ParametroGeneral.Where(parametroGeneral => pListaProyectosID.Contains(parametroGeneral.ProyectoID)).Select(parametroGeneral => new { parametroGeneral.ProyectoID, parametroGeneral.LicenciaPorDefecto }).ToDictionary(item => item.ProyectoID, item => item.LicenciaPorDefecto);
        }

        /// <summary>
        /// Indica si al registrar a un usuario nuevo en el proyecto se le debe hacer miembro de didactalia también.
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <returns>TRUE si al registrar a un usuario nuevo en el proyecto se le debe hacer miembro de didactalia también, FALSE en caso contrario</returns>
        public bool ObtenerRegistroDidactaliaProyecto(Guid pProyectoID)
        {
            return mEntityContext.ParametroGeneral.Where(parametroGeneral => parametroGeneral.ProyectoID.Equals(pProyectoID)).Select(parametroGeneral => parametroGeneral.RegDidactalia).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene una lista con los ElementosHeadID de la tabla ProyectoElementoHtmlRol
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pIdentidadUsuarioID">Identificador de la identidad del usuario actual</param>
        /// <returns>Lista de enteros con los ElementosHeadID de la tabla ProyectoElementoHtmlRol</returns>
        public List<int> ObtenerElementosHeadIDRol(Guid pProyectoID, Guid pIdentidadUsuarioID)
        {
            return mEntityContext.ProyectoElementoHTMLRol.Join(mEntityContext.GrupoIdentidadesParticipacion, elem => elem.GrupoID, grupo => grupo.GrupoID, (elem, grupo) => new { elem.ElementoHeadID, elem.ProyectoID, grupo.IdentidadID }).Where(elem => elem.IdentidadID.Equals(pIdentidadUsuarioID) && elem.ProyectoID.Equals(pProyectoID)).Select(elem => elem.ElementoHeadID).ToList();
        }

        /// <summary>
        /// Obtiene la lista de palabras inapropiadas definidas para un proyecto y que están contenidas en una lista de tags
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización del proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto en el que se va a etiquetar algo</param>
        /// <param name="pListaTags">Lista de tags a comprobar</param>
        /// <returns>Lista de palabras inapropiadas</returns>
        public List<string> ObtenerPalabrasInapropiadasDeListaTagsParaProyecto(Guid pOrganizacionID, Guid pProyectoID, List<string> pListaTags)
        {
            return mEntityContext.ProyectoPalabrasInapropiadas.Where(palabras => palabras.OrganizacionID.Equals(pOrganizacionID) && palabras.ProyectoID.Equals(pProyectoID) && pListaTags.Contains(palabras.Tag)).Select(palabras => palabras.Tag).ToList();
        }
        #endregion

        #region Privados

        /// <summary>
        /// Procedimiento para construir las consultas
        /// </summary>
        /// <param name="pIBD">Interfaz de base de datos</param>
        private void CargarConsultasYDataAdapters(IBaseDatos pIBD)
        {
            #region Campos de tablas

            selectCamposAnalisisCompletitud = "SELECT " + pIBD.CargarGuid("OrganizacionID") + ", " + pIBD.CargarGuid("ProyectoID") + ", AnalisisCompObligatorioPrc, FraseGnossNombreProceso, ColorProceso, AlcanceProceso, DescripcionProceso, ParticipantesProceso, FinalidadProceso, DocumentacionProceso, AnalisisCompObligatorioObj, FraseGnossNombreObjetivo, ColorObjetivo, AlcanceObjetivo, DescripcionObjetivo, ParticipantesObjetivo, FinalidadObjetivo, DocumentacionObjetivo, AnalisisCompObligatorioGF, FraseGnossGF, ColorGF, AlcanceGF, DescripcionGF, ParticipantesGF, FinalidadGF, DocumentacionGF, AnalisisCompObligatorioCmp, NombreCompetencia, DescripcionCompetencia, CompVinculadasCompetencia, DocVinculadosCompetencia, EntradasCompetencia, SalidasCompetencia, ControlesCompetencia, MecanismosCompetencia, EscalaMetasCompetencia FROM CamposAnalisisCompletitud";

            selectConfiguracionAmbitoBusquedaProyecto = "SELECT " + pIBD.CargarGuid("OrganizacionID") + ", " + pIBD.CargarGuid("ProyectoID") + ", Metabusqueda,  TodoGnoss, " + pIBD.CargarGuid("PestanyaDefectoID") + " FROM ConfiguracionAmbitoBusquedaProyecto";

            selectParametroGeneralPesado = "SELECT " + IBD.CargarGuid("ParametroGeneral.OrganizacionID") + ", " + IBD.CargarGuid("ParametroGeneral.ProyectoID") + ", ParametroGeneral.UmbralSuficienciaEnMejora, ParametroGeneral.DesviacionAdmitidaEnEvalua, ParametroGeneral.MetaAutomatPropietarioPro, ParametroGeneral.AvisoLegal, ParametroGeneral.WikiDisponible, ParametroGeneral.BaseRecursosDisponible, ParametroGeneral.CompartirRecursosPermitido, ParametroGeneral.InvitacionesDisponibles, ParametroGeneral.ServicioSuscripcionDisp, ParametroGeneral.BlogsDisponibles, ParametroGeneral.ForosDisponibles, ParametroGeneral.EncuestasDisponibles, ParametroGeneral.PlataformaVideoDisponible, ParametroGeneral.TOPIDCuenta, ParametroGeneral.TOPIDPlayer, ParametroGeneral.TOPPublisherID, ParametroGeneral.TOPFTPUser, ParametroGeneral.TOPFTPPass, ParametroGeneral.LogoProyecto, ParametroGeneral.MensajeBienvenida, ParametroGeneral.PreguntasDisponibles, ParametroGeneral.EntidadRevisadaObligatoria, ParametroGeneral.UmbralDetPropietariosProc, ParametroGeneral.UmbralDetPropietariosObj, ParametroGeneral.UmbralDetPropietariosGF, ParametroGeneral.NombreDebilidadDafoProc, ParametroGeneral.NombreAmenazaDafoProc, ParametroGeneral.NombreFortalezaDafoProc, ParametroGeneral.NombreOportunidadDafoProc, ParametroGeneral.NombreDebilidadDafoObj, ParametroGeneral.NombreAmenazaDafoObj, ParametroGeneral.NombreFortalezaDafoObj, ParametroGeneral.NombreOportunidadDafoObj, ParametroGeneral.NombreDebilidadDafoGF, ParametroGeneral.NombreAmenazaDafoGF, ParametroGeneral.NombreFortalezaDafoGF, ParametroGeneral.NombreOportunidadDafoGF, ParametroGeneral.ImagenHome, ParametroGeneral.NombreImagenPeque, ParametroGeneral.ImagenPersonalizadaPeque, ParametroGeneral.PermitirRevisionManualPro, ParametroGeneral.PermitirRevisionManualGF, ParametroGeneral.PermitirRevisionManualObj, ParametroGeneral.PermitirRevisionManualComp, ParametroGeneral.RutaTema, ParametroGeneral.RutaImagenesTema, ParametroGeneral.ImagenHomeGrande, ParametroGeneral.VotacionesDisponibles, ParametroGeneral.ComentariosDisponibles, ParametroGeneral.CoordenadasHome, ParametroGeneral.CoordenadasMosaico, ParametroGeneral.CoordenadasSup, ParametroGeneral.PermitirCertificacionRec, ParametroGeneral.PoliticaCertificacion, ParametroGeneral.DafoDisponible, ParametroGeneral.PlantillaDisponible, ParametroGeneral.CodigoGoogleAnalytics, ParametroGeneral.DebatesDisponibles, ParametroGeneral.VersionFotoImagenHomeGrande, ParametroGeneral.VersionFotoImagenMosaicoGrande, ParametroGeneral.VersionFotoImagenSupGrande, ParametroGeneral.VerVotaciones, ParametroGeneral.PermitirVotacionesNegativas, ParametroGeneral.VersionFotoImagenFondo, ParametroGeneral.ClausulasRegistro, ParametroGeneral.LicenciaPorDefecto, ParametroGeneral.MensajeLicenciaPorDefecto,  ParametroGeneral.BrightcoveTokenWrite, ParametroGeneral.BrightcoveTokenRead, ParametroGeneral.BrightcoveReproductorID, ParametroGeneral.BrightcoveFTP, ParametroGeneral.BrightcoveFTPUser, ParametroGeneral.BrightcoveFTPPass, ParametroGeneral.BrightcovePublisherID, ParametroGeneral.VersionCSS, ParametroGeneral.VersionJS, ParametroGeneral.OcultarPersonalizacion, ParametroGeneral.PestanyasDocSemanticos, ParametroGeneral.PestanyaRecursosVisible, ParametroGeneral.ScriptBusqueda, ParametroGeneral.ImagenRelacionadosMini, ParametroGeneral.EsBeta, ParametroGeneral.ScriptGoogleAnalytics,  ParametroGeneral.GadgetsPieDisponibles, ParametroGeneral.GadgetsCabeceraDisponibles, ParametroGeneral.NumeroRecursosRelacionados, ParametroGeneral.BiosCortas, ParametroGeneral.RdfDisponibles, ParametroGeneral.RssDisponibles, ParametroGeneral.SupervisoresAdminGrupos, ParametroGeneral.RegDidactalia, ParametroGeneral.HomeVisible, ParametroGeneral.CargasMasivasDisponibles, ParametroGeneral.ComunidadGNOSS, ParametroGeneral.IdiomasDisponibles, ParametroGeneral.IdiomaDefecto, ParametroGeneral.FechaNacimientoObligatoria, ParametroGeneral.PrivacidadObligatoria, ParametroGeneral.SolicitarCoockieLogin, ParametroGeneral.EventosDisponibles, ParametroGeneral.Copyright, ParametroGeneral.CMSDisponible, ParametroGeneral.InvitacionesPorContactoDisponibles, ParametroGeneral.VersionCSSWidget, ParametroGeneral.PermitirUsuNoLoginDescargDoc, ParametroGeneral.TipoCabecera, ParametroGeneral.TipoFichaRecurso, ParametroGeneral.AvisoCookie, ParametroGeneral.MostrarPersonasEnCatalogo, ParametroGeneral.EnvioMensajesPermitido, ParametroGeneral.EnlaceContactoPiePagina, ParametroGeneral.TieneSitemapComunidad, ParametroGeneral.MostrarAccionesEnListados, ParametroGeneral.UrlServicioFichas, ParametroGeneral.PalcoActivado, ParametroGeneral.PermitirRecursosPrivados, ParametroGeneral.AlgoritmoPersonasRecomendadas, ParametroGeneral.UrlMappingCategorias, ParametroGeneral.PropsMapaPerYOrg, ParametroGeneral.ChatDisponible, ParametroGeneral.VersionCSSAdmin, ParametroGeneral.VersionJSAdmin  FROM ParametroGeneral ";

            selectParametroGeneralLigero = "SELECT " + pIBD.CargarGuid("ParametroGeneral.OrganizacionID") + ", " + pIBD.CargarGuid("ParametroGeneral.ProyectoID") + ", ParametroGeneral.UmbralSuficienciaEnMejora, ParametroGeneral.DesviacionAdmitidaEnEvalua, ParametroGeneral.MetaAutomatPropietarioPro, ParametroGeneral.AvisoLegal, ParametroGeneral.WikiDisponible, ParametroGeneral.BaseRecursosDisponible, ParametroGeneral.CompartirRecursosPermitido, ParametroGeneral.InvitacionesDisponibles, ParametroGeneral.ServicioSuscripcionDisp, ParametroGeneral.BlogsDisponibles, ParametroGeneral.ForosDisponibles, ParametroGeneral.EncuestasDisponibles, ParametroGeneral.PlataformaVideoDisponible, ParametroGeneral.TOPIDCuenta, ParametroGeneral.TOPIDPlayer, ParametroGeneral.TOPPublisherID, ParametroGeneral.TOPFTPUser, ParametroGeneral.TOPFTPPass, ParametroGeneral.MensajeBienvenida, ParametroGeneral.EntidadRevisadaObligatoria, ParametroGeneral.UmbralDetPropietariosProc, ParametroGeneral.UmbralDetPropietariosObj, ParametroGeneral.UmbralDetPropietariosGF, ParametroGeneral.NombreDebilidadDafoProc, ParametroGeneral.NombreAmenazaDafoProc, ParametroGeneral.NombreFortalezaDafoProc, ParametroGeneral.NombreOportunidadDafoProc, ParametroGeneral.NombreDebilidadDafoObj, ParametroGeneral.NombreAmenazaDafoObj, ParametroGeneral.NombreFortalezaDafoObj, ParametroGeneral.NombreOportunidadDafoObj, ParametroGeneral.NombreDebilidadDafoGF, ParametroGeneral.NombreAmenazaDafoGF, ParametroGeneral.NombreFortalezaDafoGF, ParametroGeneral.NombreOportunidadDafoGF, ParametroGeneral.NombreImagenPeque, ParametroGeneral.PermitirRevisionManualPro, ParametroGeneral.PermitirRevisionManualGF, ParametroGeneral.PermitirRevisionManualObj, ParametroGeneral.PermitirRevisionManualComp, ParametroGeneral.RutaTema, ParametroGeneral.RutaImagenesTema, ParametroGeneral.ComentariosDisponibles, ParametroGeneral.VotacionesDisponibles, ParametroGeneral.VerVotaciones, ParametroGeneral.PermitirVotacionesNegativas, ParametroGeneral.PermitirCertificacionRec, ParametroGeneral.DafoDisponible, ParametroGeneral.PlantillaDisponible, ParametroGeneral.CoordenadasHome, ParametroGeneral.CoordenadasMosaico, ParametroGeneral.CoordenadasSup, ParametroGeneral.PreguntasDisponibles, ParametroGeneral.CodigoGoogleAnalytics, ParametroGeneral.DebatesDisponibles, ParametroGeneral.VersionFotoImagenHomeGrande, ParametroGeneral.VersionFotoImagenMosaicoGrande, ParametroGeneral.VersionFotoImagenSupGrande, ParametroGeneral.VersionFotoImagenFondo, ParametroGeneral.ClausulasRegistro, ParametroGeneral.LicenciaPorDefecto, ParametroGeneral.MensajeLicenciaPorDefecto, BrightcoveTokenWrite,BrightcoveTokenRead, BrightcoveReproductorID, ParametroGeneral.BrightcoveFTP, ParametroGeneral.BrightcoveFTPUser, ParametroGeneral.BrightcoveFTPPass, ParametroGeneral.BrightcovePublisherID, ParametroGeneral.VersionCSS, ParametroGeneral.VersionJS, ParametroGeneral.OcultarPersonalizacion, ParametroGeneral.PestanyasDocSemanticos, ParametroGeneral.PestanyaRecursosVisible, ParametroGeneral.ScriptBusqueda, ParametroGeneral.ImagenRelacionadosMini, ParametroGeneral.EsBeta, ParametroGeneral.ScriptGoogleAnalytics, ParametroGeneral.GadgetsPieDisponibles, ParametroGeneral.GadgetsCabeceraDisponibles, ParametroGeneral.NumeroRecursosRelacionados, ParametroGeneral.BiosCortas, ParametroGeneral.RdfDisponibles, ParametroGeneral.RssDisponibles, ParametroGeneral.SupervisoresAdminGrupos, ParametroGeneral.RegDidactalia, ParametroGeneral.HomeVisible, ParametroGeneral.CargasMasivasDisponibles, ParametroGeneral.ComunidadGNOSS, ParametroGeneral.IdiomasDisponibles, ParametroGeneral.IdiomaDefecto, ParametroGeneral.FechaNacimientoObligatoria, ParametroGeneral.PrivacidadObligatoria, ParametroGeneral.SolicitarCoockieLogin, ParametroGeneral.EventosDisponibles, ParametroGeneral.Copyright, ParametroGeneral.CMSDisponible, ParametroGeneral.InvitacionesPorContactoDisponibles, ParametroGeneral.VersionCSSWidget, ParametroGeneral.PermitirUsuNoLoginDescargDoc, ParametroGeneral.TipoCabecera, ParametroGeneral.TipoFichaRecurso, ParametroGeneral.AvisoCookie, ParametroGeneral.MostrarPersonasEnCatalogo, ParametroGeneral.EnvioMensajesPermitido, ParametroGeneral.EnlaceContactoPiePagina, ParametroGeneral.TieneSitemapComunidad, ParametroGeneral.MostrarAccionesEnListados, ParametroGeneral.UrlServicioFichas, ParametroGeneral.PalcoActivado, ParametroGeneral.PermitirRecursosPrivados, ParametroGeneral.AlgoritmoPersonasRecomendadas, ParametroGeneral.UrlMappingCategorias, ParametroGeneral.PropsMapaPerYOrg, ParametroGeneral.ChatDisponible, ParametroGeneral.VersionCSSAdmin, ParametroGeneral.VersionJSAdmin FROM ParametroGeneral ";

            selectProyectoMetaRobots = "SELECT " + pIBD.CargarGuid("OrganizacionID") + ", " + pIBD.CargarGuid("ProyectoID") + ", Tipo, Content FROM ProyectoMetaRobots";

            selectProyectoRDFType = "SELECT " + pIBD.CargarGuid("OrganizacionID") + ", " + pIBD.CargarGuid("ProyectoID") + ", TipoDocumento, RdfType FROM ProyectoRDFType";

            selectTextosPersonalizadosProyecto = "SELECT " + pIBD.CargarGuid("OrganizacionID") + ", " + pIBD.CargarGuid("ProyectoID") + ", TextoID, Language, Texto FROM TextosPersonalizadosProyecto";

            selectTextosPersonalizadosPersonalizacion = "SELECT " + pIBD.CargarGuid("TextosPersonalizadosPersonalizacion.PersonalizacionID") + ", TextoID, Language, texto FROM TextosPersonalizadosPersonalizacion ";

            #endregion

            #region DataAdapter

            #region CamposAnalisisCompletitud

            sqlCamposAnalisisCompletitudInsert = pIBD.ReplaceParam("INSERT INTO CamposAnalisisCompletitud (OrganizacionID, ProyectoID, AnalisisCompObligatorioPrc, FraseGnossNombreProceso, ColorProceso, AlcanceProceso, DescripcionProceso, ParticipantesProceso, FinalidadProceso, DocumentacionProceso, AnalisisCompObligatorioObj, FraseGnossNombreObjetivo, ColorObjetivo, AlcanceObjetivo, DescripcionObjetivo, ParticipantesObjetivo, FinalidadObjetivo, DocumentacionObjetivo, AnalisisCompObligatorioGF, FraseGnossGF, ColorGF, AlcanceGF, DescripcionGF, ParticipantesGF, FinalidadGF, DocumentacionGF, AnalisisCompObligatorioCmp, NombreCompetencia, DescripcionCompetencia, CompVinculadasCompetencia, DocVinculadosCompetencia, EntradasCompetencia, SalidasCompetencia, ControlesCompetencia, MecanismosCompetencia, EscalaMetasCompetencia) VALUES (" + pIBD.GuidParamColumnaTabla("OrganizacionID") + ", " + pIBD.GuidParamColumnaTabla("ProyectoID") + ", @AnalisisCompObligatorioPrc, @FraseGnossNombreProceso, @ColorProceso, @AlcanceProceso, @DescripcionProceso, @ParticipantesProceso, @FinalidadProceso, @DocumentacionProceso, @AnalisisCompObligatorioObj, @FraseGnossNombreObjetivo, @ColorObjetivo, @AlcanceObjetivo, @DescripcionObjetivo, @ParticipantesObjetivo, @FinalidadObjetivo, @DocumentacionObjetivo, @AnalisisCompObligatorioGF, @FraseGnossGF, @ColorGF, @AlcanceGF, @DescripcionGF, @ParticipantesGF, @FinalidadGF, @DocumentacionGF, @AnalisisCompObligatorioCmp, @NombreCompetencia, @DescripcionCompetencia, @CompVinculadasCompetencia, @DocVinculadosCompetencia, @EntradasCompetencia, @SalidasCompetencia, @ControlesCompetencia, @MecanismosCompetencia, @EscalaMetasCompetencia)");

            sqlCamposAnalisisCompletitudDelete = pIBD.ReplaceParam("DELETE FROM CamposAnalisisCompletitud WHERE (OrganizacionID = " + pIBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (ProyectoID = " + pIBD.GuidParamColumnaTabla("O_ProyectoID") + ") AND (AnalisisCompObligatorioPrc = @O_AnalisisCompObligatorioPrc) AND (FraseGnossNombreProceso = @O_FraseGnossNombreProceso) AND (ColorProceso = @O_ColorProceso) AND (AlcanceProceso = @O_AlcanceProceso) AND (DescripcionProceso = @O_DescripcionProceso) AND (ParticipantesProceso = @O_ParticipantesProceso) AND (FinalidadProceso = @O_FinalidadProceso) AND (DocumentacionProceso = @O_DocumentacionProceso) AND (AnalisisCompObligatorioObj = @O_AnalisisCompObligatorioObj) AND (FraseGnossNombreObjetivo = @O_FraseGnossNombreObjetivo) AND (ColorObjetivo = @O_ColorObjetivo) AND (AlcanceObjetivo = @O_AlcanceObjetivo) AND (DescripcionObjetivo = @O_DescripcionObjetivo) AND (ParticipantesObjetivo = @O_ParticipantesObjetivo) AND (FinalidadObjetivo = @O_FinalidadObjetivo) AND (DocumentacionObjetivo = @O_DocumentacionObjetivo) AND (AnalisisCompObligatorioGF = @O_AnalisisCompObligatorioGF) AND (FraseGnossGF = @O_FraseGnossGF) AND (ColorGF = @O_ColorGF) AND (AlcanceGF = @O_AlcanceGF) AND (DescripcionGF = @O_DescripcionGF) AND (ParticipantesGF = @O_ParticipantesGF) AND (FinalidadGF = @O_FinalidadGF) AND (DocumentacionGF = @O_DocumentacionGF) AND (AnalisisCompObligatorioCmp = @O_AnalisisCompObligatorioCmp) AND (NombreCompetencia = @O_NombreCompetencia) AND (DescripcionCompetencia = @O_DescripcionCompetencia) AND (CompVinculadasCompetencia = @O_CompVinculadasCompetencia) AND (DocVinculadosCompetencia = @O_DocVinculadosCompetencia) AND (EntradasCompetencia = @O_EntradasCompetencia) AND (SalidasCompetencia = @O_SalidasCompetencia) AND (ControlesCompetencia = @O_ControlesCompetencia) AND (MecanismosCompetencia = @O_MecanismosCompetencia) AND (EscalaMetasCompetencia = @O_EscalaMetasCompetencia)");

            sqlCamposAnalisisCompletitudModify = pIBD.ReplaceParam("UPDATE CamposAnalisisCompletitud SET OrganizacionID = " + pIBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + pIBD.GuidParamColumnaTabla("ProyectoID") + ", AnalisisCompObligatorioPrc = @AnalisisCompObligatorioPrc, FraseGnossNombreProceso = @FraseGnossNombreProceso, ColorProceso = @ColorProceso, AlcanceProceso = @AlcanceProceso, DescripcionProceso = @DescripcionProceso, ParticipantesProceso = @ParticipantesProceso, FinalidadProceso = @FinalidadProceso, DocumentacionProceso = @DocumentacionProceso, AnalisisCompObligatorioObj = @AnalisisCompObligatorioObj, FraseGnossNombreObjetivo = @FraseGnossNombreObjetivo, ColorObjetivo = @ColorObjetivo, AlcanceObjetivo = @AlcanceObjetivo, DescripcionObjetivo = @DescripcionObjetivo, ParticipantesObjetivo = @ParticipantesObjetivo, FinalidadObjetivo = @FinalidadObjetivo, DocumentacionObjetivo = @DocumentacionObjetivo, AnalisisCompObligatorioGF = @AnalisisCompObligatorioGF, FraseGnossGF = @FraseGnossGF, ColorGF = @ColorGF, AlcanceGF = @AlcanceGF, DescripcionGF = @DescripcionGF, ParticipantesGF = @ParticipantesGF, FinalidadGF = @FinalidadGF, DocumentacionGF = @DocumentacionGF, AnalisisCompObligatorioCmp = @AnalisisCompObligatorioCmp, NombreCompetencia = @NombreCompetencia, DescripcionCompetencia = @DescripcionCompetencia, CompVinculadasCompetencia = @CompVinculadasCompetencia, DocVinculadosCompetencia = @DocVinculadosCompetencia, EntradasCompetencia = @EntradasCompetencia, SalidasCompetencia = @SalidasCompetencia, ControlesCompetencia = @ControlesCompetencia, MecanismosCompetencia = @MecanismosCompetencia, EscalaMetasCompetencia = @EscalaMetasCompetencia WHERE (OrganizacionID = " + pIBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (ProyectoID = " + pIBD.GuidParamColumnaTabla("O_ProyectoID") + ") AND (FraseGnossNombreProceso = @O_FraseGnossNombreProceso) AND (ColorProceso = @O_ColorProceso) AND (AlcanceProceso = @O_AlcanceProceso) AND (DescripcionProceso = @O_DescripcionProceso) AND (ParticipantesProceso = @O_ParticipantesProceso) AND (FinalidadProceso = @O_FinalidadProceso) AND (DocumentacionProceso = @O_DocumentacionProceso) AND (FraseGnossNombreObjetivo = @O_FraseGnossNombreObjetivo) AND (ColorObjetivo = @O_ColorObjetivo) AND (AlcanceObjetivo = @O_AlcanceObjetivo) AND (DescripcionObjetivo = @O_DescripcionObjetivo) AND (ParticipantesObjetivo = @O_ParticipantesObjetivo) AND (FinalidadObjetivo = @O_FinalidadObjetivo) AND (DocumentacionObjetivo = @O_DocumentacionObjetivo) AND (FraseGnossGF = @O_FraseGnossGF) AND (ColorGF = @O_ColorGF) AND (AlcanceGF = @O_AlcanceGF) AND (DescripcionGF = @O_DescripcionGF) AND (ParticipantesGF = @O_ParticipantesGF) AND (FinalidadGF = @O_FinalidadGF) AND (DocumentacionGF = @O_DocumentacionGF) AND (AnalisisCompObligatorioCmp = @O_AnalisisCompObligatorioCmp) AND (NombreCompetencia = @O_NombreCompetencia) AND (DescripcionCompetencia = @O_DescripcionCompetencia) AND (CompVinculadasCompetencia = @O_CompVinculadasCompetencia) AND (DocVinculadosCompetencia = @O_DocVinculadosCompetencia) AND (EntradasCompetencia = @O_EntradasCompetencia) AND (SalidasCompetencia = @O_SalidasCompetencia) AND (ControlesCompetencia = @O_ControlesCompetencia) AND (MecanismosCompetencia = @O_MecanismosCompetencia) AND (EscalaMetasCompetencia = @O_EscalaMetasCompetencia)");

            #endregion

            #region ParametroGeneral
            //InvitacionesPorContactoDisponibles
            this.sqlParametroGeneralInsert = IBD.ReplaceParam("INSERT INTO ParametroGeneral (OrganizacionID, ProyectoID, UmbralSuficienciaEnMejora, DesviacionAdmitidaEnEvalua, MetaAutomatPropietarioPro, AvisoLegal, WikiDisponible, BaseRecursosDisponible, CompartirRecursosPermitido, InvitacionesDisponibles, ServicioSuscripcionDisp, BlogsDisponibles, ForosDisponibles, EncuestasDisponibles, PlataformaVideoDisponible,TOPIDCuenta,TOPIDPlayer,TOPPublisherID,TOPFTPUser,TOPFTPPass, BrightcoveTokenWrite, BrightcoveTokenRead, BrightcoveReproductorID, LogoProyecto, MensajeBienvenida, PreguntasDisponibles, EntidadRevisadaObligatoria, UmbralDetPropietariosProc, UmbralDetPropietariosObj, UmbralDetPropietariosGF, NombreDebilidadDafoProc, NombreAmenazaDafoProc, NombreFortalezaDafoProc, NombreOportunidadDafoProc, NombreDebilidadDafoObj, NombreAmenazaDafoObj, NombreFortalezaDafoObj, NombreOportunidadDafoObj, NombreDebilidadDafoGF, NombreAmenazaDafoGF, NombreFortalezaDafoGF, NombreOportunidadDafoGF, ImagenHome, NombreImagenPeque, ImagenPersonalizadaPeque, PermitirRevisionManualPro, PermitirRevisionManualGF, PermitirRevisionManualObj, PermitirRevisionManualComp, RutaTema, RutaImagenesTema, ImagenHomeGrande,   VotacionesDisponibles, ComentariosDisponibles, CoordenadasHome, CoordenadasMosaico, CoordenadasSup, PermitirCertificacionRec, PoliticaCertificacion, DafoDisponible, PlantillaDisponible, CodigoGoogleAnalytics, DebatesDisponibles, VersionFotoImagenHomeGrande, VersionFotoImagenMosaicoGrande, VersionFotoImagenSupGrande, VerVotaciones, PermitirVotacionesNegativas, VersionFotoImagenFondo, ClausulasRegistro, LicenciaPorDefecto, MensajeLicenciaPorDefecto,BrightcoveFTP,BrightcoveFTPUser,BrightcoveFTPPass,BrightcovePublisherID, VersionCSS, VersionJS, OcultarPersonalizacion, PestanyasDocSemanticos, PestanyaRecursosVisible, ScriptBusqueda, ImagenRelacionadosMini, EsBeta, ScriptGoogleAnalytics,GadgetsPieDisponibles,GadgetsCabeceraDisponibles,NumeroRecursosRelacionados, BiosCortas, RdfDisponibles, RssDisponibles, SupervisoresAdminGrupos, RegDidactalia, HomeVisible, CargasMasivasDisponibles, ComunidadGNOSS, IdiomasDisponibles, IdiomaDefecto, FechaNacimientoObligatoria, PrivacidadObligatoria, SolicitarCoockieLogin, EventosDisponibles, Copyright, CMSDisponible, InvitacionesPorContactoDisponibles, VersionCSSWidget, PermitirUsuNoLoginDescargDoc, TipoCabecera, TipoFichaRecurso, AvisoCookie, MostrarPersonasEnCatalogo, EnvioMensajesPermitido, EnlaceContactoPiePagina, TieneSitemapComunidad, MostrarAccionesEnListados, UrlServicioFichas, PalcoActivado, PermitirRecursosPrivados, AlgoritmoPersonasRecomendadas, UrlMappingCategorias, PropsMapaPerYOrg, ChatDisponible, VersionCSSAdmin, VersionJSAdmin) VALUES (" + IBD.GuidParamColumnaTabla("OrganizacionID") + ", " + IBD.GuidParamColumnaTabla("ProyectoID") + ", @UmbralSuficienciaEnMejora, @DesviacionAdmitidaEnEvalua, @MetaAutomatPropietarioPro, @AvisoLegal, @WikiDisponible, @BaseRecursosDisponible, @CompartirRecursosPermitido, @InvitacionesDisponibles, @ServicioSuscripcionDisp, @BlogsDisponibles, @ForosDisponibles, @EncuestasDisponibles, @PlataformaVideoDisponible, @TOPIDCuenta, @TOPIDPlayer, @TOPPublisherID, @TOPFTPUser, @TOPFTPPass, @BrightcoveTokenWrite, @BrightcoveTokenRead, @BrightcoveReproductorID, @LogoProyecto, @MensajeBienvenida, @PreguntasDisponibles, @EntidadRevisadaObligatoria, @UmbralDetPropietariosProc, @UmbralDetPropietariosObj, @UmbralDetPropietariosGF, @NombreDebilidadDafoProc, @NombreAmenazaDafoProc, @NombreFortalezaDafoProc, @NombreOportunidadDafoProc, @NombreDebilidadDafoObj, @NombreAmenazaDafoObj, @NombreFortalezaDafoObj, @NombreOportunidadDafoObj, @NombreDebilidadDafoGF, @NombreAmenazaDafoGF, @NombreFortalezaDafoGF, @NombreOportunidadDafoGF, @ImagenHome, @NombreImagenPeque, @ImagenPersonalizadaPeque, @PermitirRevisionManualPro, @PermitirRevisionManualGF, @PermitirRevisionManualObj, @PermitirRevisionManualComp, @RutaTema, @RutaImagenesTema, @ImagenHomeGrande, @VotacionesDisponibles, @ComentariosDisponibles, @CoordenadasHome, @CoordenadasMosaico, @CoordenadasSup, @PermitirCertificacionRec, @PoliticaCertificacion, @DafoDisponible, @PlantillaDisponible, @CodigoGoogleAnalytics, @DebatesDisponibles, @VersionFotoImagenHomeGrande, @VersionFotoImagenMosaicoGrande, @VersionFotoImagenSupGrande, @VerVotaciones, @PermitirVotacionesNegativas, @VersionFotoImagenFondo, @ClausulasRegistro, @LicenciaPorDefecto, @MensajeLicenciaPorDefecto, @BrightcoveFTP, @BrightcoveFTPUser, @BrightcoveFTPPass ,@brightcovePublisherID, @VersionCSS, @VersionJS, @OcultarPersonalizacion, @PestanyasDocSemanticos, @PestanyaRecursosVisible, @ScriptBusqueda, @ImagenRelacionadosMini, @EsBeta, @ScriptGoogleAnalytics, @GadgetsPieDisponibles, @GadgetsCabeceraDisponibles, @NumeroRecursosRelacionados, @BiosCortas, @RdfDisponibles, @RssDisponibles, @SupervisoresAdminGrupos, @RegDidactalia, @HomeVisible, @CargasMasivasDisponibles, @ComunidadGNOSS, @IdiomasDisponibles, @IdiomaDefecto, @FechaNacimientoObligatoria, @PrivacidadObligatoria, @SolicitarCoockieLogin, @EventosDisponibles, @Copyright, @CMSDisponible, @InvitacionesPorContactoDisponibles, @VersionCSSWidget, @PermitirUsuNoLoginDescargDoc, @TipoCabecera, @TipoFichaRecurso, @AvisoCookie, @MostrarPersonasEnCatalogo , @EnvioMensajesPermitido, @EnlaceContactoPiePagina, @TieneSitemapComunidad, @MostrarAccionesEnListados, @UrlServicioFichas, @PalcoActivado, @PermitirRecursosPrivados,@AlgoritmoPersonasRecomendadas,@UrlMappingCategorias, @PropsMapaPerYOrg, @ChatDisponible, @VersionCSSAdmin, @VersionJSAdmin)");
            this.sqlParametroGeneralDelete = IBD.ReplaceParam("DELETE FROM ParametroGeneral WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ") ");
            this.sqlParametroGeneralModify = IBD.ReplaceParam("UPDATE ParametroGeneral SET UmbralSuficienciaEnMejora = @UmbralSuficienciaEnMejora, DesviacionAdmitidaEnEvalua = @DesviacionAdmitidaEnEvalua, MetaAutomatPropietarioPro = @MetaAutomatPropietarioPro, AvisoLegal = @AvisoLegal, WikiDisponible = @WikiDisponible, BaseRecursosDisponible = @BaseRecursosDisponible, CompartirRecursosPermitido = @CompartirRecursosPermitido, InvitacionesDisponibles = @InvitacionesDisponibles, ServicioSuscripcionDisp = @ServicioSuscripcionDisp, BlogsDisponibles = @BlogsDisponibles, ForosDisponibles = @ForosDisponibles, EncuestasDisponibles = @EncuestasDisponibles, PlataformaVideoDisponible = @PlataformaVideoDisponible,TOPIDCuenta = @TOPIDCuenta,TOPIDPlayer = @TOPIDPlayer,TOPPublisherID = @TOPPublisherID,TOPFTPUser = @TOPFTPUser,TOPFTPPass = @TOPFTPPass, BrightcoveTokenWrite = @BrightcoveTokenWrite, BrightcoveTokenRead = @BrightcoveTokenRead, BrightcoveReproductorID = @BrightcoveReproductorID, LogoProyecto = @LogoProyecto, MensajeBienvenida = @MensajeBienvenida, PreguntasDisponibles = @PreguntasDisponibles, EntidadRevisadaObligatoria = @EntidadRevisadaObligatoria, UmbralDetPropietariosProc = @UmbralDetPropietariosProc, UmbralDetPropietariosObj = @UmbralDetPropietariosObj, UmbralDetPropietariosGF = @UmbralDetPropietariosGF, NombreDebilidadDafoProc = @NombreDebilidadDafoProc, NombreAmenazaDafoProc = @NombreAmenazaDafoProc, NombreFortalezaDafoProc = @NombreFortalezaDafoProc, NombreOportunidadDafoProc = @NombreOportunidadDafoProc, NombreDebilidadDafoObj = @NombreDebilidadDafoObj, NombreAmenazaDafoObj = @NombreAmenazaDafoObj, NombreFortalezaDafoObj = @NombreFortalezaDafoObj, NombreOportunidadDafoObj = @NombreOportunidadDafoObj, NombreDebilidadDafoGF = @NombreDebilidadDafoGF, NombreAmenazaDafoGF = @NombreAmenazaDafoGF, NombreFortalezaDafoGF = @NombreFortalezaDafoGF, NombreOportunidadDafoGF = @NombreOportunidadDafoGF, ImagenHome = @ImagenHome, NombreImagenPeque = @NombreImagenPeque, ImagenPersonalizadaPeque = @ImagenPersonalizadaPeque, PermitirRevisionManualPro = @PermitirRevisionManualPro, PermitirRevisionManualGF = @PermitirRevisionManualGF, PermitirRevisionManualObj = @PermitirRevisionManualObj, PermitirRevisionManualComp = @PermitirRevisionManualComp, RutaTema = @RutaTema, RutaImagenesTema = @RutaImagenesTema, ImagenHomeGrande = @ImagenHomeGrande, VotacionesDisponibles = @VotacionesDisponibles, ComentariosDisponibles = @ComentariosDisponibles, CoordenadasHome = @CoordenadasHome,CoordenadasMosaico = @CoordenadasMosaico,CoordenadasSup = @CoordenadasSup, PermitirCertificacionRec = @PermitirCertificacionRec, PoliticaCertificacion = @PoliticaCertificacion, DafoDisponible = @DafoDisponible, PlantillaDisponible = @PlantillaDisponible, CodigoGoogleAnalytics = @CodigoGoogleAnalytics, DebatesDisponibles = @DebatesDisponibles, VersionFotoImagenHomeGrande = @VersionFotoImagenHomeGrande,VersionFotoImagenMosaicoGrande = @VersionFotoImagenMosaicoGrande,VersionFotoImagenSupGrande = @VersionFotoImagenSupGrande, VerVotaciones = @VerVotaciones, PermitirVotacionesNegativas = @PermitirVotacionesNegativas, VersionFotoImagenFondo = @VersionFotoImagenFondo, ClausulasRegistro = @ClausulasRegistro, LicenciaPorDefecto = @LicenciaPorDefecto, MensajeLicenciaPorDefecto = @MensajeLicenciaPorDefecto, BrightcoveFTP = @BrightcoveFTP, BrightcoveFTPUser = @BrightcoveFTPUser, BrightcoveFTPPass = @BrightcoveFTPPass, BrightcovePublisherID = @BrightcovePublisherID, VersionCSS = @VersionCSS, VersionJS = @VersionJS, OcultarPersonalizacion = @OcultarPersonalizacion,PestanyasDocSemanticos = @PestanyasDocSemanticos,PestanyaRecursosVisible = @PestanyaRecursosVisible,ScriptBusqueda = @ScriptBusqueda,ImagenRelacionadosMini = @ImagenRelacionadosMini,EsBeta = @EsBeta, ScriptGoogleAnalytics = @ScriptGoogleAnalytics, GadgetsPieDisponibles = @GadgetsPieDisponibles, GadgetsCabeceraDisponibles = @GadgetsCabeceraDisponibles, NumeroRecursosRelacionados = @NumeroRecursosRelacionados, BiosCortas = @BiosCortas, RdfDisponibles = @RdfDisponibles, RssDisponibles = @RssDisponibles, SupervisoresAdminGrupos = @SupervisoresAdminGrupos, RegDidactalia = @RegDidactalia, HomeVisible = @HomeVisible, CargasMasivasDisponibles = @CargasMasivasDisponibles, ComunidadGNOSS = @ComunidadGNOSS, IdiomasDisponibles = @IdiomasDisponibles, IdiomaDefecto = @IdiomaDefecto, FechaNacimientoObligatoria=@FechaNacimientoObligatoria, PrivacidadObligatoria=@PrivacidadObligatoria, SolicitarCoockieLogin=@SolicitarCoockieLogin, EventosDisponibles=@EventosDisponibles, Copyright=@Copyright, CMSDisponible=@CMSDisponible, InvitacionesPorContactoDisponibles = @InvitacionesPorContactoDisponibles, VersionCSSWidget = @VersionCSSWidget, PermitirUsuNoLoginDescargDoc = @PermitirUsuNoLoginDescargDoc, TipoCabecera=@TipoCabecera, TipoFichaRecurso=@TipoFichaRecurso, AvisoCookie=@AvisoCookie, MostrarPersonasEnCatalogo=@MostrarPersonasEnCatalogo, EnvioMensajesPermitido=@EnvioMensajesPermitido, EnlaceContactoPiePagina=@EnlaceContactoPiePagina, TieneSitemapComunidad=@TieneSitemapComunidad,MostrarAccionesEnListados=@MostrarAccionesEnListados, UrlServicioFichas=@UrlServicioFichas,PalcoActivado=@PalcoActivado,PermitirRecursosPrivados=@PermitirRecursosPrivados,AlgoritmoPersonasRecomendadas=@AlgoritmoPersonasRecomendadas,  UrlMappingCategorias=@UrlMappingCategorias, PropsMapaPerYOrg=@PropsMapaPerYOrg,ChatDisponible=@ChatDisponible, VersionCSSAdmin = @VersionCSSAdmin, VersionJSAdmin = @VersionJSAdmin WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ")");

            #endregion

            #region ConfiguracionAmbitoBusquedaProyecto

            sqlConfiguracionAmbitoBusquedaProyectoInsert = pIBD.ReplaceParam("INSERT INTO ConfiguracionAmbitoBusquedaProyecto (OrganizacionID, ProyectoID, Metabusqueda, TodoGnoss, PestanyaDefectoID) VALUES (" + pIBD.GuidParamColumnaTabla("OrganizacionID") + ", " + pIBD.GuidParamColumnaTabla("ProyectoID") + ", @Metabusqueda, @TodoGnoss, " + pIBD.GuidParamColumnaTabla("PestanyaDefectoID") + ")");

            sqlConfiguracionAmbitoBusquedaProyectoDelete = pIBD.ReplaceParam("DELETE FROM ConfiguracionAmbitoBusquedaProyecto WHERE (OrganizacionID = " + pIBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (ProyectoID = " + pIBD.GuidParamColumnaTabla("O_ProyectoID") + ")");

            sqlConfiguracionAmbitoBusquedaProyectoModify = pIBD.ReplaceParam("UPDATE ConfiguracionAmbitoBusquedaProyecto SET OrganizacionID = " + pIBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + pIBD.GuidParamColumnaTabla("ProyectoID") + ", Metabusqueda = @Metabusqueda, TodoGnoss = @TodoGnoss, PestanyaDefectoID = " + pIBD.GuidParamColumnaTabla("PestanyaDefectoID") + " WHERE (OrganizacionID = " + pIBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (ProyectoID = " + pIBD.GuidParamColumnaTabla("O_ProyectoID") + ")");

            #endregion

            #region ProyectoMetaRobots

            sqlProyectoMetaRobotsInsert = pIBD.ReplaceParam("INSERT INTO ProyectoMetaRobots (OrganizacionID, ProyectoID, Tipo, Content) VALUES (" + pIBD.GuidParamColumnaTabla("OrganizacionID") + ", " + pIBD.GuidParamColumnaTabla("ProyectoID") + ", @Tipo, @Content)");

            sqlProyectoMetaRobotsDelete = pIBD.ReplaceParam("DELETE FROM ProyectoMetaRobots WHERE (OrganizacionID = " + pIBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (ProyectoID = " + pIBD.GuidParamColumnaTabla("O_ProyectoID") + " AND Tipo = @O_Tipo)");

            sqlProyectoMetaRobotsModify = pIBD.ReplaceParam("UPDATE ProyectoMetaRobots SET OrganizacionID = " + pIBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + pIBD.GuidParamColumnaTabla("ProyectoID") + ", Tipo=@Tipo, Content=@Content WHERE (OrganizacionID = " + pIBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (ProyectoID = " + pIBD.GuidParamColumnaTabla("O_ProyectoID") + " AND Tipo = @O_Tipo)");

            #endregion

            #region TextosPersonalizadosProyecto

            sqlTextosPersonalizadosProyectoInsert = pIBD.ReplaceParam("INSERT INTO TextosPersonalizadosProyecto (OrganizacionID, ProyectoID, TextoID, Language, Texto) VALUES (" + pIBD.GuidParamColumnaTabla("OrganizacionID") + ", " + pIBD.GuidParamColumnaTabla("ProyectoID") + ", @TextoID, @Language, @Texto)");

            sqlTextosPersonalizadosProyectoDelete = pIBD.ReplaceParam("DELETE FROM TextosPersonalizadosProyecto WHERE (OrganizacionID = " + pIBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (ProyectoID = " + pIBD.GuidParamColumnaTabla("O_ProyectoID") + ") AND (TextoID =  @O_TextoID) AND (Language = @O_Language)");

            sqlTextosPersonalizadosProyectoModify = pIBD.ReplaceParam("UPDATE TextosPersonalizadosProyecto SET OrganizacionID = " + pIBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + pIBD.GuidParamColumnaTabla("ProyectoID") + ", TextoID = @TextoID, Language = @Language, Texto =  @Texto" + " WHERE (OrganizacionID = " + pIBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (ProyectoID = " + pIBD.GuidParamColumnaTabla("O_ProyectoID") + ") AND (TextoID = @O_TextoID) AND (Language = @O_Language)");

            #endregion

            #region TextosPersonalizadosPersonalizacion

            sqlTextosPersonalizadosPersonalizacionInsert = pIBD.ReplaceParam("INSERT INTO TextosPersonalizadosPersonalizacion (PersonalizacionID, TextoID, Language, Texto) VALUES (" + pIBD.GuidParamColumnaTabla("PersonalizacionID") + ", @TextoID, @Language, @Texto)");

            sqlTextosPersonalizadosPersonalizacionDelete = pIBD.ReplaceParam("DELETE FROM TextosPersonalizadosPersonalizacion WHERE (PersonalizacionID = " + pIBD.GuidParamColumnaTabla("O_PersonalizacionID") + ") AND (TextoID =  @O_TextoID) AND (Language = @O_Language)");

            sqlTextosPersonalizadosPersonalizacionModify = pIBD.ReplaceParam("UPDATE TextosPersonalizadosPersonalizacion SET PersonalizacionID = " + pIBD.GuidParamColumnaTabla("PersonalizacionID") + ", TextoID = @TextoID, Language = @Language, Texto =  @Texto" + " WHERE (PersonalizacionID = " + pIBD.GuidParamColumnaTabla("O_PersonalizacionID") + ") AND (TextoID = @O_TextoID) AND (Language = @O_Language)");

            #endregion

            #region ParametroProyecto

            sqlParametroProyectoInsert = pIBD.ReplaceParam("INSERT INTO ParametroProyecto (OrganizacionID, ProyectoID, Parametro, Valor) VALUES (" + pIBD.GuidParamColumnaTabla("OrganizacionID") + ", " + pIBD.GuidParamColumnaTabla("ProyectoID") + ", @Parametro, @Valor)");

            sqlParametroProyectoDelete = pIBD.ReplaceParam("DELETE FROM ParametroProyecto WHERE (OrganizacionID = " + pIBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (ProyectoID =  " + pIBD.GuidParamColumnaTabla("O_ProyectoID") + ") AND (Parametro = @O_Parametro)");

            sqlParametroProyectoModify = pIBD.ReplaceParam("UPDATE ParametroProyecto SET OrganizacionID = " + pIBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + pIBD.GuidParamColumnaTabla("ProyectoID") + ", Parametro = @Parametro, Valor =  @Valor" + " WHERE (OrganizacionID = " + pIBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (ProyectoID = " + pIBD.GuidParamColumnaTabla("O_ProyectoID") + ") AND (Parametro = @O_Parametro)");

            #endregion

            #endregion

            sqlSelectCamposAnalisisCompletitudDeProyecto = selectCamposAnalisisCompletitud + " WHERE (ProyectoID = " + pIBD.GuidParamValor("proyectoID") + ")";

            sqlSelectParametrosGeneralesDeProyecto = selectParametroGeneralPesado + " WHERE (ProyectoID = " + pIBD.GuidParamValor("proyectoID") + ")";

            sqlSelectParametrosGeneralesDeProyectoCargaLigera = selectParametroGeneralLigero + " WHERE (ProyectoID = " + pIBD.GuidParamValor("proyectoID") + ")";

            sqlSelectConfiguracionAmbitoBusquedaProyecto = selectConfiguracionAmbitoBusquedaProyecto + " WHERE (ProyectoID = " + pIBD.GuidParamValor("proyectoID") + ")";

            sqlSelectProyectoMetaRobots = selectProyectoMetaRobots + " WHERE (ProyectoID = " + pIBD.GuidParamValor("proyectoID") + ")";

            sqlSelectProyectoRDFType = selectProyectoRDFType + " WHERE (ProyectoID = " + pIBD.GuidParamValor("proyectoID") + ") AND (TipoDocumento = @TipoDocumento)";

            sqlSelectTextosPersonalizadosProyecto = selectTextosPersonalizadosProyecto + " WHERE (ProyectoID = " + pIBD.GuidParamValor("proyectoID") + ")";

            sqlSelectTextosPersonalizadosPersonalizacionProyecto = selectTextosPersonalizadosPersonalizacion + " INNER JOIN VistaVirtualProyecto ON TextosPersonalizadosPersonalizacion.PersonalizacionID = VistaVirtualProyecto.PersonalizacionID WHERE (ProyectoID = " + pIBD.GuidParamValor("proyectoID") + ")";

            sqlSelectTextosPersonalizadosPersonalizacionPersonalizacionID = selectTextosPersonalizadosPersonalizacion + " WHERE (TextosPersonalizadosPersonalizacion.PersonalizacionID = " + pIBD.GuidParamValor("personalizacionID") + ")";
        }

        /// <summary>
        /// Asigna los datos pesados cargados desde un datareader a una fila de parámetros generales de proyecto
        /// </summary>
        /// <param name="pDataReader">Datareader</param>
        /// <param name="pFilaParametroGeneral">Fila de parámetros generales de proyecto</param>
        private void AsignarDatosPesadosADataRow(IDataReader pDataReader, ParametroGeneral pFilaParametroGeneral)
        {

            // DataRowState estadoAnterior = pFilaParametroGeneral.RowState;

            //AvisoLegal
            if (!pDataReader.IsDBNull(0))
            {
                long size = pDataReader.GetChars(0, 0, null, 0, 0);
                char[] values = new char[size];

                int bufferSize = 100;
                long bytesRead = 0;
                int curPos = 0;

                while (bytesRead < size)
                {
                    bytesRead += pDataReader.GetChars(0, curPos, values, curPos, bufferSize);
                    curPos += bufferSize;
                }
                pFilaParametroGeneral.AvisoLegal = new string(values);
            }

            //MensajeBienvenida
            if (!pDataReader.IsDBNull(1))
            {
                long size = pDataReader.GetChars(1, 0, null, 0, 0);
                char[] values = new char[size];

                int bufferSize = 100;
                long bytesRead = 0;
                int curPos = 0;

                while (bytesRead < size)
                {
                    bytesRead += pDataReader.GetChars(1, curPos, values, curPos, bufferSize);
                    curPos += bufferSize;
                }
                pFilaParametroGeneral.MensajeBienvenida = new string(values);
            }

            //PoliticaCertificacion
            if (!pDataReader.IsDBNull(2))
            {
                long size = pDataReader.GetChars(2, 0, null, 0, 0);
                char[] values = new char[size];

                int bufferSize = 100;
                long bytesRead = 0;
                int curPos = 0;

                while (bytesRead < size)
                {
                    bytesRead += pDataReader.GetChars(2, curPos, values, curPos, bufferSize);
                    curPos += bufferSize;
                }
                pFilaParametroGeneral.PoliticaCertificacion = new string(values);
            }

            // LogoProyecto
            if (!pDataReader.IsDBNull(3))
            {
                long size = pDataReader.GetBytes(3, 0, null, 0, 0);
                byte[] values = new byte[size];

                int bufferSize = 100;
                long bytesRead = 0;
                int curPos = 0;

                while (bytesRead < size)
                {
                    bytesRead += pDataReader.GetBytes(3, curPos, values, curPos, bufferSize);
                    curPos += bufferSize;
                }
                pFilaParametroGeneral.LogoProyecto = values;
            }

            //ImagenHome
            if (!pDataReader.IsDBNull(4))
            {
                long size = pDataReader.GetBytes(4, 0, null, 0, 0);
                byte[] values = new byte[size];

                int bufferSize = 100;
                long bytesRead = 0;
                int curPos = 0;

                while (bytesRead < size)
                {
                    bytesRead += pDataReader.GetBytes(4, curPos, values, curPos, bufferSize);
                    curPos += bufferSize;
                }
                pFilaParametroGeneral.ImagenHome = values;
            }

            //ImagenHomeGrande
            if (!pDataReader.IsDBNull(5))
            {
                long size = pDataReader.GetBytes(5, 0, null, 0, 0);
                byte[] values = new byte[size];

                int bufferSize = 100;
                long bytesRead = 0;
                int curPos = 0;

                while (bytesRead < size)
                {
                    bytesRead += pDataReader.GetBytes(5, curPos, values, curPos, bufferSize);
                    curPos += bufferSize;
                }
                pFilaParametroGeneral.ImagenHomeGrande = values;
            }

            //ImagenPersonalizadaPeque
            if (!pDataReader.IsDBNull(6))
            {
                long size = pDataReader.GetBytes(8, 0, null, 0, 0);
                byte[] values = new byte[size];

                int bufferSize = 100;
                long bytesRead = 0;
                int curPos = 0;

                while (bytesRead < size)
                {
                    bytesRead += pDataReader.GetBytes(8, curPos, values, curPos, bufferSize);
                    curPos += bufferSize;
                }
                pFilaParametroGeneral.ImagenPersonalizadaPeque = values;
            }

            //if (estadoAnterior == DataRowState.Unchanged)
            //{
            //    pFilaParametroGeneral.AcceptChanges();
            //}
        }

        #endregion

        #endregion
    }
}
