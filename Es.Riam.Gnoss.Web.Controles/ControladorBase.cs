using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.BASE_BD;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.ParametroGeneralDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.Identidad;
using Es.Riam.Gnoss.AD.Live;
using Es.Riam.Gnoss.AD.Live.Model;
using Es.Riam.Gnoss.AD.Parametro;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Usuarios;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.Identidad;
using Es.Riam.Gnoss.CL.ParametrosAplicacion;
using Es.Riam.Gnoss.CL.ParametrosProyecto;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Elementos.ParametroAplicacion;
using Es.Riam.Gnoss.Elementos.ParametroGeneralDSEspacio;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Documentacion;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.Live;
using Es.Riam.Gnoss.Logica.Parametro;
using Es.Riam.Gnoss.Logica.ParametroAplicacion;
using Es.Riam.Gnoss.Logica.ParametrosProyecto;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Usuarios;
using Es.Riam.Gnoss.RabbitMQ;
using Es.Riam.Gnoss.Recursos;
using Es.Riam.Gnoss.Servicios.ControladoresServiciosWeb;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Util.Seguridad;
using Es.Riam.Gnoss.UtilServiciosWeb;
using Es.Riam.Gnoss.Web.Controles.ParametroGeneralDSName;
using Es.Riam.Gnoss.Web.Controles.ServicioImagenesWrapper;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.Util;
using Es.Riam.Web.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace Es.Riam.Gnoss.Web.Controles
{
    /// <summary>
    /// Controlador para Proyectos
    /// </summary>
    public class ControladorBase
    {
        private static ConcurrentDictionary<Guid, ConcurrentDictionary<Guid, bool>> mListaOntologiasPermitidasPorIdentidad = new ConcurrentDictionary<Guid, ConcurrentDictionary<Guid, bool>>();

        /// <summary>
        /// Obtiene el proyecto externo en el que se está haciendo la búsqueda, o NULL si se hace en el proyecto actual.
        /// </summary>
        private Elementos.ServiciosGenerales.Proyecto mProyectoOrigenBusqueda;

        /// <summary>
        /// BaseURL Content
        /// </summary>
        private string mBaseURLContent = null;

        /// <summary>
        /// FilaProy con los contadores del proyecto actual.
        /// </summary>
        private AD.EntityModel.Models.ProyectoDS.Proyecto mContadoresProyecto;

        private string mUrlSearchProyecto = "";

        /// <summary>
        /// HTML personalizado para el Login.
        /// </summary>
        private string mProyectoLoginConfiguracion = string.Empty;

        private TipoRolUsuario? mTipoRolUsuarioEnProyecto = null;

        public const string FacetasBuscadorCom = "FacetasCom";

        /// <summary>
        /// Lista de los bots de los que registramos todo el user agent (ej: google+)
        /// </summary>
        public static List<string> ListaBotsCompletos = null;

        /// <summary>
        /// Nombre del ecosistema
        /// </summary>
        private static string mNombreProyectoEcosistema = null;

        /// <summary>
        /// 
        /// </summary>
        private List<TiposDocumentacion> mListaPermisosDocumentos;

        /// <summary>
        /// Url de intragnoss de servicios.
        /// </summary>
        private string mUrlIntragnossServicios;

        /// <summary>
        /// Indica si hay que subir los recursos a GoogleDrive
        /// </summary>
        private bool mTieneGoogleDriveConfigurado;

        /// <summary>
        /// Obtiene si se trata de un ecosistema donde se puede editar multiple
        /// </summary>
        private bool? mEsEdicionMultiple = null;

        /// <summary>
        /// Indica si hay que pintar la ficha de recursos de inevery o la normal.
        /// </summary>
        private static bool? mPintarFichaRecInevery;

        /// <summary>
        /// Lista con todos los bots registrados
        /// </summary>
        public static string ListaTodosBots = null;

        /// <summary>
        /// Identificador del proyecto principal de la aplicacion
        /// </summary>
        private static Guid? mProyectoPrincipal = null;

        /// <summary>
        /// Obtiene si se trata de un ecosistema sin bandeja de suscripciones
        /// </summary>
        private bool? mEsEcosistemaSinBandejaSuscripciones = null;

        /// <summary>
        /// Lista de Items que se usa para aplicaciones que no son Web. Hay una lista por thread de la aplicación
        /// </summary>
        private static ConcurrentDictionary<int, Dictionary<string, object>> mListaItemsPorThread = new ConcurrentDictionary<int, Dictionary<string, object>>();

        /// <summary>
        /// Obtiene si se trata de un ecosistema sin contactos
        /// </summary>
        private bool? mEsEcosistemaSinContactos = null;

        private string mNombreCortoProyectoPrincipal = null;

        private bool? mIrAConfiguracionInicial = null;

        private string mUrlPrincipal = null;

        private Guid? mProyectoConexionID = null;

        TipoCabeceraProyecto? mTipoCabecera = null;

        private string mUrlPresentacion = null;

        private string mEspacioPersonal = null;

        private string mNombreCortoProyectoConexion = null;

        private static List<string> ListaUrlsSinIdentidad = new List<string>() { "/load-resource-actions", "/peticionesajax/cargarnumelementosnuevos" };

        private string mBaseUrlIdioma = null;

        private Guid? mBaseRecursosProyectoSeleccionado;

        private DataWrapperExportacionBusqueda mExportacionBusquedaDW;

        private bool? mPerfilGlobalEnComunidadPrincipal = null;

        private string mUrlIntragnoss = null;

        private UtilIdiomas mUtilIdiomas = null;

        private Guid mProyectoSeleccionadoError404 = Guid.Empty;

        private bool? mComunidadExcluidaPersonalizacionEcosistema = null;

        public const string SESION_UNICA_POR_USUARIO = "sesionUnicaPorUsuarioConCookie";

        private bool? mRegistroAutomaticoEcosistema = null;

        private bool? mRegistroAutomaticoEnComunidad = null;

        /// <summary>
        /// Obtiene si el ecosistema tiene una personalizacion de vistas
        /// </summary>
        private Guid? mPersonalizacionEcosistemaID = null;

        private Guid mPersonalizacionDominio;

        private static string mUrlServicioLogin = null;

        private Guid[] mIdentidadIDActual = null;

        /// <summary>
        /// Proyecto Virtual actual.
        /// </summary>
        private Elementos.ServiciosGenerales.Proyecto mProyectoVirtual = null;

        protected Elementos.ServiciosGenerales.ProyectoPestanyaMenu mProyectoPestanyaActual;

        /// <summary>
        /// Gestor de identidades
        /// </summary>
        private GestionIdentidades mGestorIdentidades;

        private Identidad mIdentidadActual;

        private Guid? mProyectoPrincipalUnico = null;

        /// <summary>
        /// Fila del dataset de parámetros generales del proyecto actual.
        /// </summary>
        protected ParametroGeneral mParametroGeneralRow;

        /// <summary>
        /// Obtiene si se trata de un ecosistema sin metaproyecto
        /// </summary>
        private bool? mEsEcosistemaSinMetaProyecto = null;

        /// <summary>
        /// Contiene la URL del servicio web de documentación.
        /// </summary>
        private string mBaseUrl = null;

        /// <summary>
        /// Base Static
        /// </summary>
        private string mBaseURLStatic = null;

        /// <summary>
        /// BaseURL Personalizacion
        /// </summary>
        private string mBaseURLPersonalizacionEcosistema = null;


        /// <summary>
        /// Parametros de configuración del proyecto.
        /// </summary>
        private Dictionary<string, string> mParametroProyecto;

        /// <summary>
        /// Parametros de configuración del proyecto de Ecosistema.
        /// </summary>
        private Dictionary<string, string> mParametroProyectoEcosistema;

        /// <summary>
        /// Nombre del proyecto padre del ecosistema configurado en BD con el parametro ComunidadPadreEcosistemaID (comunidad/nombrecorto)
        /// </summary>
        private static string mNombreProyectoPadreEcositema = null;

        /// <summary>
        /// Nombre del proyecto padre del ecosistema configurado en BD con el parametro NombreCortoProyectoPadreEcosistema (comunidad/nombrecorto)
        /// </summary>
        private static string mNombreCortoProyectoPadreEcosistema = null;

        /// <summary>
        /// Fila de parámetros de aplicación
        /// </summary>
        //private ParametroAplicacionDS mParametrosAplicacionDS;
        private List<ParametroAplicacion> mListaParametrosAplicacion;

        /// <summary>
        /// ProyectoID padre del ecosistema (comunidad/nombrecorto)
        /// </summary>
        private static Guid? mPadreEcosistemaProyectoID = null;

        private GestorParametroAplicacion mGestorParametrosAplicacion;

        private string mIdiomaUsuario = null;
        private string mDominioPaginasAdministracion = null;
        /// <summary>
        /// URL base de los formularios semánticos.
        /// </summary>
        private string mBaseURLFormulariosSem;

        private const string EXCHANGE = "";
        private const string COLA_RABBIT = "cola";
        private const string COLA_VISITAS = "ColaVisitas";

        private static bool? mHayConexionRabbit = null;

        protected Elementos.ServiciosGenerales.Proyecto mProyecto;
        protected GnossUrlsSemanticas mGnossUrlsSemanticas;
        protected EntityContext mEntityContext;
        protected LoggingService mLoggingService;
        protected VirtuosoAD mVirtuosoAD;
        protected ConfigService mConfigService;
        protected RedisCacheWrapper mRedisCacheWrapper;
        protected GnossCache mGnossCache;
        protected IHttpContextAccessor mHttpContextAccessor;
        protected UtilWeb mUtilWeb;
        protected IServicesUtilVirtuosoAndReplication mServicesUtilVirtuosoAndReplication;
        protected ILogger mlogger;
        protected ILoggerFactory mLoggerFactory;

        public ControladorBase(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<ControladorBase> logger, ILoggerFactory loggerFactory)
        {
            mLoggingService = loggingService;
            mVirtuosoAD = virtuosoAD;
            mConfigService = configService;
            mEntityContext = entityContext;
            mRedisCacheWrapper = redisCacheWrapper;
            mGnossCache = gnossCache;
            mHttpContextAccessor = httpContextAccessor;
            mUtilWeb = new UtilWeb(mHttpContextAccessor);
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        /// <summary>
        /// Anula la identidad actual el gestor de identidades cargado en ella
        /// </summary>
        public void ResetearIdentidadActual()
        {
            mIdentidadActual = null;
            mGestorIdentidades = null;
        }

        public Guid ProyectoSeleccionadoError404
        {
            get
            {
                return mProyectoSeleccionadoError404;
            }
            set
            {
                mProyectoSeleccionadoError404 = value;
                mProyectoVirtual = null;
                mCargandoProyectoVirtual = false;
            }
        }

        /// <summary>
        /// Obtiene la URL principal de esta aplicación (ej: para http://didactalia.net el dominio principal es http://gnoss.com)
        /// </summary>
        public string UrlPrincipal
        {
            get
            {
                if (mUrlPrincipal == null)
                {
                    try
                    {
                        mUrlPrincipal = mConfigService.ObtenerUrlBase();
                    }
                    catch (Exception ex)
                    {
                        mLoggingService.GuardarLogError(ex, mlogger);
                    }

                    if (string.IsNullOrEmpty(mUrlPrincipal))
                    {
                        mUrlPrincipal = BaseURL;
                    }
                }
                return mUrlPrincipal;
            }
        }

        public GnossUrlsSemanticas UrlsSemanticas
        {
            get
            {
                if (mGnossUrlsSemanticas == null)
                {
                    mGnossUrlsSemanticas = new GnossUrlsSemanticas(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<GnossUrlsSemanticas>(), mLoggerFactory);
                }
                return mGnossUrlsSemanticas;
            }
        }

        public bool IrAConfiguracionInicial
        {
            get
            {
                if (!mIrAConfiguracionInicial.HasValue)
                {
                    ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
                    int numProyectos = proyectoCN.ObtenerNumeroDeProyectos();
                    if (numProyectos <= 1)
                    {
                        mIrAConfiguracionInicial = true;
                    }
                    else
                    {
                        mIrAConfiguracionInicial = false;
                    }
                }

                return mIrAConfiguracionInicial.Value;
            }
            set
            {
                mIrAConfiguracionInicial = value;
            }
        }

        public Elementos.ServiciosGenerales.Proyecto ProyectoSeleccionado
        {
            get
            {
                if (mProyecto == null)
                {
                    string nombreCortoProyecto = null;
                    Guid proyectoID = Guid.Empty;

                    if (ProyectoSeleccionadoError404 != null && !ProyectoSeleccionadoError404.Equals(Guid.Empty))
                    {
                        proyectoID = ProyectoSeleccionadoError404;
                    }
                    else if (RequestParams("proyectoID") != null)
                    {
                        proyectoID = new Guid(RequestParams("proyectoID"));
                    }
                    else if (RequestParams("nombreProy") != null)
                    {
                        nombreCortoProyecto = RequestParams("nombreProy");
                    }
                    else if (RequestParams("NombreCortoComunidad") != null)
                    {
                        nombreCortoProyecto = RequestParams("NombreCortoComunidad");
                    }
                    else if (RequestParams("proy") != null)
                    {
                        proyectoID = new Guid(RequestParams("proy"));
                    }
                    else if (RequestParams("pProyectoID") != null)
                    {
                        Guid.TryParse(RequestParams("pProyectoID").Replace("\"", ""), out proyectoID);
                    }
                    else
                    {
                        proyectoID = ProyectoAD.MetaProyecto;
                    }

                    if (!string.IsNullOrEmpty(RequestParams("ecosistema")) && RequestParams("ecosistema").Equals("true"))
                    {
                        proyectoID = ProyectoAD.MetaProyecto;
                        nombreCortoProyecto = "mygnoss";
                    }

                    if (mProyecto == null || (nombreCortoProyecto != mProyecto.NombreCorto && mProyecto.Clave != proyectoID))
                    {   
                        ProyectoCL proyectoCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);

                        if (!string.IsNullOrEmpty(nombreCortoProyecto) && !nombreCortoProyecto.Equals("mygnoss"))
                        {
                            proyectoID = proyectoCL.ObtenerProyectoIDPorNombreCorto(nombreCortoProyecto);
                        }
                        else if (proyectoID.Equals(Guid.Empty))
                        {
                            proyectoID = ProyectoAD.MetaProyecto;
                        }

                        GestionProyecto gestorProyecto = new GestionProyecto(proyectoCL.ObtenerProyectoPorID(proyectoID), mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestionProyecto>(), mLoggerFactory);

                        if (gestorProyecto.ListaProyectos.Count > 0 && gestorProyecto.ListaProyectos.ContainsKey(proyectoID))
                        {
                            mProyecto = gestorProyecto.ListaProyectos[proyectoID];
                        }

                        if (mProyecto == null)
                        {
                            return null;
                        }
                        ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
                        UtilPermisos utilPermisos = new UtilPermisos(mEntityContext, mLoggingService, mConfigService, mLoggerFactory.CreateLogger<UtilPermisos>(), mLoggerFactory);
                        bool esAdminMiembros = false;

						if (proyectoID.Equals(ProyectoAD.MetaProyecto) && (mHttpContextAccessor.HttpContext != null && IsSessionAvailable && mHttpContextAccessor.HttpContext.Session != null && mHttpContextAccessor.HttpContext.Session.Get<GnossIdentity>("Usuario") != null && !(mHttpContextAccessor.HttpContext.Session.Get<GnossIdentity>("Usuario")).EsIdentidadInvitada))
                        {
							esAdminMiembros = utilPermisos.UsuarioTienePermisoAdministracionEcosistema((ulong)PermisoEcosistema.AdministrarMiembrosEcosistema, (mHttpContextAccessor.HttpContext.Session.Get<GnossIdentity>("Usuario")).UsuarioID) || proyCN.EsUsuarioAdministradorProyecto(mHttpContextAccessor.HttpContext.Session.Get<GnossIdentity>("Usuario").UsuarioID, proyectoID);
                        }
                        else if (!proyectoID.Equals(ProyectoAD.MetaProyecto) && (mHttpContextAccessor.HttpContext != null && IsSessionAvailable && mHttpContextAccessor.HttpContext.Session != null && mHttpContextAccessor.HttpContext.Session.Get<GnossIdentity>("Usuario") != null && !(mHttpContextAccessor.HttpContext.Session.Get<GnossIdentity>("Usuario")).EsIdentidadInvitada)) 
                        {
							esAdminMiembros = utilPermisos.IdentidadTienePermiso((ulong)PermisoComunidad.GestionarMiembros, mHttpContextAccessor.HttpContext.Session.Get<GnossIdentity>("Usuario").IdentidadID, mHttpContextAccessor.HttpContext.Session.Get<GnossIdentity>("Usuario").IdentidadID, TipoDePermiso.Comunidad) || proyCN.EsUsuarioAdministradorProyecto(mHttpContextAccessor.HttpContext.Session.Get<GnossIdentity>("Usuario").UsuarioID, proyectoID);
						}

                        //TODO Javier cambiar esto de sitio cuando se migre la Web
                        if (mHttpContextAccessor.HttpContext != null && IsSessionAvailable && mHttpContextAccessor.HttpContext.Session != null && mHttpContextAccessor.HttpContext.Session.Get<GnossIdentity>("Usuario") != null && !(mHttpContextAccessor.HttpContext.Session.Get<GnossIdentity>("Usuario")).EsIdentidadInvitada && esAdminMiembros)
                        {
                            CrearPestanyaPersonasYOrganizaciones(mProyecto.GestorProyectos.DataWrapperProyectos, mProyecto.Clave);
                        }
                    }
                    return mProyecto;
                }
                return mProyecto;
            }
            set
            {
                mProyecto = value;
            }
        }

        private static bool? mIsSessionAvailable;

        private bool IsSessionAvailable
        {
            get
            {
                if (!mIsSessionAvailable.HasValue)
                {
                    try
                    {
                        var session = mHttpContextAccessor.HttpContext.Session;
                        mIsSessionAvailable = true;
                    }
                    catch
                    {
                        mIsSessionAvailable = false;
                    }
                }
                return mIsSessionAvailable.Value;
            }
        }

        /// <summary>
        /// Devuelve si el usuario que está navegando es un bot
        /// </summary>
        public bool EsBot
        {
            get
            {
                try
                {
                    if (string.IsNullOrEmpty(mHttpContextAccessor.HttpContext.Request.Headers["User-Agent"]) || mHttpContextAccessor.HttpContext.Request.Headers["User-Agent"].Equals("GnossBotChequeoCache"))
                    {
                        return true;
                    }
                    else
                    {
                        if (ListaTodosBots == null)
                        {
                            LeerConfigBots(BaseURLSinHTTP);
                        }

                        if (ListaBotsCompletos != null && ListaBotsCompletos.Contains(mHttpContextAccessor.HttpContext.Request.Headers["User-Agent"]))
                        {
                            return true;
                        }
                        else if (!string.IsNullOrEmpty(ListaTodosBots))
                        {
                            return Regex.IsMatch(mHttpContextAccessor.HttpContext.Request.Headers["User-Agent"], ListaTodosBots);
                        }

                        return false;
                    }
                }
                catch (Exception)
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// Lee el fichero de configuración para bots
        /// </summary>
        /// <param name="pBaseURLSinHTTP">Base URL sin http://</param>
        public void LeerConfigBots(string pBaseURLSinHTTP)
        {
            try
            {
                if (ListaTodosBots == null)
                {
                    string rutaFicherBots = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{pBaseURLSinHTTP}/config/bots.config");

                    if ((!string.IsNullOrEmpty(rutaFicherBots)) && File.Exists(rutaFicherBots))
                    {
                        string[] bots = File.ReadAllLines(rutaFicherBots);

                        if (bots != null)
                        {
                            ListaTodosBots = bots[0];

                            if (!string.IsNullOrEmpty(ListaTodosBots))
                            {
                                ListaTodosBots = ListaTodosBots.Substring(ListaTodosBots.IndexOf(':') + 1).Trim();
                            }
                            else
                            {
                                ListaTodosBots = "";
                            }

                            if (bots.Length > 1)
                            {
                                string[] separadores = { "|||" };
                                ListaBotsCompletos = new List<string>(bots[1].Substring(bots[1].IndexOf(':') + 1).Split(separadores, StringSplitOptions.RemoveEmptyEntries));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, mlogger);
            }
        }

        /// <summary>
        /// Carga pestañas que son exclusivas del administrador
        /// </summary>
        public static void CrearPestanyaPersonasYOrganizaciones(DataWrapperProyecto pDataWrapperProyecto, Guid pProyectoID)
        {
            if (pDataWrapperProyecto != null && !pDataWrapperProyecto.ListaProyectoPestanyaMenu.Any(pestanya => pestanya.TipoPestanya.Equals((short)TipoPestanyaMenu.PersonasYOrganizaciones)))
            {
                // No hay pestaña personas y organizaciones en este proyecto, le creo una para que el administrador pueda entrar 
                AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu filaPestanya = new AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu();

                filaPestanya.PestanyaID = Guid.NewGuid();
                filaPestanya.OrganizacionID = ProyectoAD.MetaOrganizacion;
                filaPestanya.ProyectoID = pProyectoID;
                filaPestanya.TipoPestanya = (short)TipoPestanyaMenu.PersonasYOrganizaciones;
                filaPestanya.Nombre = "";
                filaPestanya.Ruta = "";
                filaPestanya.Orden = 500;
                filaPestanya.NuevaPestanya = false;
                filaPestanya.Visible = false;
                filaPestanya.Privacidad = 0;
                filaPestanya.HtmlAlternativo = "";
                filaPestanya.IdiomasDisponibles = "";
                filaPestanya.Titulo = "";
                filaPestanya.NombreCortoPestanya = filaPestanya.PestanyaID.ToString();
                filaPestanya.VisibleSinAcceso = false;
                filaPestanya.CSSBodyClass = "";
                filaPestanya.Activa = true;

                pDataWrapperProyecto.ListaProyectoPestanyaMenu.Add(filaPestanya);
                //ProyectoGBD.ProyectoGBD proyectoGBD = new ProyectoGBD.ProyectoGBD();
                //proyectoGBD.AddProyectoPestanyaMenu(filaPestanya);
                //proyectoGBD.GuardarCambios();
            }
        }

        private string mScriptGoogleAnalytics;
        private string mCodigoCompletoGoogleAnalytics;

        private string mCodigoGooleAnalytics;

        /// <summary>
        /// Indica si hay que pintar la ficha de recursos de inevery o la normal.
        /// </summary>
        public bool PintarFichaRecInevery
        {
            get
            {
                if (!mPintarFichaRecInevery.HasValue)
                {
                    TipoFichaRecursoProyecto tipoFicha = TipoFichaRecursoProyecto.Normal;
                    //Si se especifica en el proyecto, lo cogemos de ahí
                    if (!(ParametrosGeneralesRow.TipoFichaRecurso == null))
                    {
                        tipoFicha = (TipoFichaRecursoProyecto)ParametrosGeneralesRow.TipoFichaRecurso;
                        mPintarFichaRecInevery = tipoFicha == TipoFichaRecursoProyecto.Inevery;
                    }
                    else
                    {
                        //Si no está especificado en el proyecto, lo cogemos del ecosistema       
                        ParametroAplicacionCL paramCL = new ParametroAplicacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCL>(), mLoggerFactory);
                        //ParametroAplicacionDS parametrosAplicacionDS = ((ParametroAplicacionDS)paramCL.ObtenerParametrosAplicacion());
                        List<AD.EntityModel.ParametroAplicacion> parametrosAplicacionDS = paramCL.ObtenerParametrosAplicacionPorContext();
                        //EntityContext context = EntityContext.Instance;
                        //List<AD.EntityModel.ParametroAplicacion> busqueda = context.ParametroAplicacion.Where(parametro => parametro.Parametro.Equals("TipoFichaRecurso")).ToList();
                        ParametroAplicacion busqueda = ListaParametrosAplicacion.FirstOrDefault(parametro => parametro.Parametro.Equals("TipoFichaRecurso"));
                        mPintarFichaRecInevery = (busqueda != null);
                        //parametrosAplicacionDS.Dispose();
                        paramCL.Dispose();
                    }
                }

                return mPintarFichaRecInevery.Value;
            }
        }

        public string CodigoCompletoGoogleAnalytics
        {
            get
            {
                if (string.IsNullOrEmpty(mCodigoCompletoGoogleAnalytics))
                {
                    mCodigoCompletoGoogleAnalytics = "";

                    //Obtenemos el código de google analytics del proyecto
                    string codigoGoogle = ParametrosGeneralesRow.CodigoGoogleAnalytics;

                    ParametroGeneral filaParametrosGen = ParametrosGeneralesRow;
                    if (string.IsNullOrEmpty(ParametrosGeneralesRow.ScriptGoogleAnalytics))
                    {
                        //Si el proyecto no tiene script definido, comprobamos si lo tiene el metaproyecto
                        filaParametrosGen = ObtenerFilaParametrosGeneralesDeProyecto(ProyectoAD.MetaProyecto).ListaParametroGeneral.FirstOrDefault();
                    }

                    if (!string.IsNullOrEmpty(filaParametrosGen.ScriptGoogleAnalytics)) //Si no es nulo mostramos el script de google analytics
                    {
                        if (string.IsNullOrEmpty(codigoGoogle) && !string.IsNullOrEmpty(filaParametrosGen.ScriptGoogleAnalytics))
                        {
                            codigoGoogle = filaParametrosGen.CodigoGoogleAnalytics;
                        }
                        mScriptGoogleAnalytics = filaParametrosGen.ScriptGoogleAnalytics;

                        mCodigoCompletoGoogleAnalytics = ScriptGoogleAnalytics.Replace("@@codigoga@@", codigoGoogle);
                    }
                }

                string codigoGoogleAnalytics = mCodigoCompletoGoogleAnalytics;
                if (CodigoGoogleAnalyticsBusquedaEstablecido && !string.IsNullOrEmpty(codigoGoogleAnalytics) && codigoGoogleAnalytics.IndexOf("</script><script type=\"text/javascript\">") > 0)
                {
                    //Si ya se ha establecido la parte del código que registra las búsquedas, quito la llamada que registra la visita inicial de la página
                    codigoGoogleAnalytics = codigoGoogleAnalytics.Substring(0, codigoGoogleAnalytics.IndexOf("</script><script type=\"text/javascript\">"));
                }

                return codigoGoogleAnalytics;
            }
        }

        private static bool? mJSYCSSunificado = null;
        public bool JSYCSSunificado
        {
            get
            {

                mJSYCSSunificado = mConfigService.JSYCSSunificado();


                return (bool)mJSYCSSunificado;
            }
            set
            {
                mJSYCSSunificado = value;
            }
        }

        public string CodigoGooleAnalytics
        {
            get
            {
                if (string.IsNullOrEmpty(mCodigoGooleAnalytics))
                {
                    ParametroGeneral parametroGeneral = ParametrosGeneralesRow;
                    if ((ProyectoSeleccionado != null) && (ProyectoSeleccionado.FilaProyecto.URLPropia == null))
                    {
                        //Sacamos el código de google analytics

                        parametroGeneral = new UtilUsuario(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mLoggerFactory.CreateLogger<UtilUsuario>(), mLoggerFactory).ObtenerFilaParametrosGeneralesDeProyecto(ProyectoAD.MetaProyecto);
                    }

                    if (parametroGeneral != null && !string.IsNullOrEmpty(parametroGeneral.CodigoGoogleAnalytics))
                    {
                        mCodigoGooleAnalytics = parametroGeneral.CodigoGoogleAnalytics;
                    }
                }

                return mCodigoGooleAnalytics;
            }
        }

        /// <summary>
        /// Obtiene el script de google analyitics
        /// </summary>
        public string ScriptGoogleAnalytics
        {
            get
            {
                if (string.IsNullOrEmpty(mScriptGoogleAnalytics))
                {
                    ParametroAplicacion busqueda = ListaParametrosAplicacion.Where(parametro => parametro.Parametro.Equals("ScriptGoogleAnalytics")).FirstOrDefault();
                    if (busqueda == null)
                    {
                        mScriptGoogleAnalytics = "";
                    }
                    else
                    {
                        mScriptGoogleAnalytics = busqueda.Valor;
                    }
                }
                return mScriptGoogleAnalytics;
            }
        }

        private bool mCodigoGoogleAnalyticsBusquedaEstablecido = false;

        public bool CodigoGoogleAnalyticsBusquedaEstablecido
        {
            get
            {
                return mCodigoGoogleAnalyticsBusquedaEstablecido;
            }
            set
            {
                mCodigoGoogleAnalyticsBusquedaEstablecido = value;
            }
        }


        /// <summary>
        /// Obtiene el nombre corto del proyecto al que se conecta por defecto siempre la aplicación
        /// </summary>
        public string NombreCortoProyectoConexion
        {
            get
            {
                if (mNombreCortoProyectoConexion == null)
                {
                    mNombreCortoProyectoConexion = "";
                    if (ProyectoConexionID != ProyectoAD.MetaProyecto)
                    {
                        ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
                        mNombreCortoProyectoConexion = proyectoCN.ObtenerNombreCortoProyecto(ProyectoConexionID);
                        proyectoCN.Dispose();
                    }
                }

                return mNombreCortoProyectoConexion;
            }
        }

        /// <summary>
        /// Url del servicio de contextos
        /// </summary>
        public string mUrlServicioContextos;

        /// <summary>
        /// Obtiene la URL del los elementos de contenido de la página
        /// </summary>
        public string UrlServicioContextos
        {
            get
            {
                if (mUrlServicioContextos == null)
                {
                    mUrlServicioContextos = mConfigService.ObtenerUrlServicio("urlServicioContextos");
                }
                return mUrlServicioContextos;
            }
        }


        /// <summary>
        /// Obtiene el dominio de la aplicación (gnoss.com, proyectos.gnoss.com, ...)
        /// </summary>
        public string DominoAplicacion
        {
            get
            {
                string host = mHttpContextAccessor.HttpContext.Request.Host.Host.ToLower().Trim();

                if ((!host.Equals("2003server")) && (!host.Equals("localhost")) && (!host.Equals("vm2003server")))
                {
                    return UtilDominios.ObtenerDominioUrl(mHttpContextAccessor.HttpContext.Request.GetDisplayUrl(), false);
                }
                return null;
            }
        }

        public string DominioPaginasAdministracion
        {
            get
            {
                if (mDominioPaginasAdministracion == null)
                {
                    ParametroAplicacionCN parametroAplicacionCN = new ParametroAplicacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCN>(), mLoggerFactory);
                    mDominioPaginasAdministracion = parametroAplicacionCN.ObtenerParametroAplicacion(TiposParametrosAplicacion.DominioPaginasAdministracion);

                    if (mDominioPaginasAdministracion == null)
                    {
                        mDominioPaginasAdministracion = string.Empty;
                    }
                }

                return mDominioPaginasAdministracion;
            }
        }

        /// <summary>
        /// Obtiene si se trata de un ecosistema sin metaproyecto
        /// </summary>
        public bool ComunidadExcluidaPersonalizacionEcosistema
        {
            get
            {
                if (!mComunidadExcluidaPersonalizacionEcosistema.HasValue)
                {
                    mComunidadExcluidaPersonalizacionEcosistema = false;
                    //EntityContext context = EntityContext.Instance;
                    //List<AD.EntityModel.ParametroAplicacion> busqueda = context.ParametroAplicacion.Where(parametro => parametro.Parametro.Equals(TiposParametrosAplicacion.ComunidadesExcluidaPersonalizacion)).ToList();
                    ParametroAplicacion busqueda = GestorParametrosAplicacion.ParametroAplicacion.FirstOrDefault(parametro => parametro.Parametro.Equals(TiposParametrosAplicacion.ComunidadesExcluidaPersonalizacion));
                    //if (ParametrosAplicacionDS.ParametroAplicacion.Select("Parametro = '" + TiposParametrosAplicacion.ComunidadesExcluidaPersonalizacion.ToString() + "'").Length > 0)
                    if (busqueda != null)
                    {
                        //List<string> listaComunidadesExcluidas = new List<string>(ParametrosAplicacionDS.ParametroAplicacion.Select("Parametro = '" + TiposParametrosAplicacion.ComunidadesExcluidaPersonalizacion.ToString() + "'")[0]["Valor"].ToString().ToUpper().Split(','));
                        List<string> listaComunidadesExcluidas = new List<string>(busqueda.Valor.ToUpper().Split(','));
                        mComunidadExcluidaPersonalizacionEcosistema = listaComunidadesExcluidas.Contains(ProyectoSeleccionado.Clave.ToString().ToUpper());
                    }
                }
                return mComunidadExcluidaPersonalizacionEcosistema.Value;
            }
        }

        public Guid PersonalizacionDominio
        {
            get
            {
                if (mPersonalizacionDominio.Equals(Guid.Empty))
                {
                    string dominio = mConfigService.ObtenerDominio();

                    if (!string.IsNullOrEmpty(dominio))
                    {
						VistaVirtualCN vistaVirtualCN = new VistaVirtualCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<VistaVirtualCN>(), mLoggerFactory);
						mPersonalizacionDominio = vistaVirtualCN.ObtenerPersonalizacionDominio(dominio);
					}					
				}

                return mPersonalizacionDominio;
            }
        }

        /// <summary>
        /// Obtiene si se trata de un ecosistema sin metaproyecto
        /// </summary>
        public Guid PersonalizacionEcosistemaID
        {
            get
            {
                if (!mPersonalizacionEcosistemaID.HasValue)
                {
                    mPersonalizacionEcosistemaID = Guid.Empty;
                    //EntityContext context = EntityContext.Instance;
                    //List<AD.EntityModel.ParametroAplicacion> busqueda = context.ParametroAplicacion.Where(parametro => parametro.Parametro.Equals(TiposParametrosAplicacion.PersonalizacionEcosistemaID)).ToList();
                    ParametroAplicacion busqueda = GestorParametrosAplicacion.ParametroAplicacion.FirstOrDefault(parametro => parametro.Parametro.Equals(TiposParametrosAplicacion.PersonalizacionEcosistemaID));
                    //if (!ComunidadExcluidaPersonalizacionEcosistema && ParametrosAplicacionDS.ParametroAplicacion.Select("Parametro = '" + TiposParametrosAplicacion.PersonalizacionEcosistemaID.ToString() + "'").Length > 0)
                    if (!ComunidadExcluidaPersonalizacionEcosistema && busqueda != null)
                    {
                        mPersonalizacionEcosistemaID = new Guid(busqueda.Valor);
                    }
                }
                return mPersonalizacionEcosistemaID.Value;
            }
        }

        /// <summary>
        /// Indica si debe pintar la cabecera simplificada
        /// </summary>
        public TipoCabeceraProyecto TipoCabecera
        {
            get
            {
                if (mTipoCabecera.HasValue)
                {
                    return mTipoCabecera.Value;
                }
                //Si se especifica en el proyecto, lo cogemos de ahí
                if (!(ParametrosGeneralesVirtualRow.TipoCabecera == null))
                {
                    mTipoCabecera = (TipoCabeceraProyecto)ParametrosGeneralesVirtualRow.TipoCabecera;
                    return mTipoCabecera.Value;
                }
                //EntityContext context = EntityContext.Instance;
                //List<AD.EntityModel.ParametroAplicacion> busqueda = context.ParametroAplicacion.Where(parametro => parametro.Parametro.Equals("TipoCabecera")).ToList();
                ParametroAplicacion busqueda = ListaParametrosAplicacion.FirstOrDefault(parametro => parametro.Parametro.Equals("TipoCabecera"));
                //Si no está especificado en el proyecto, lo cogemos del ecosistema
                // if (ParametrosAplicacionDS.ParametroAplicacion.Select("Parametro = 'TipoCabecera'").Length > 0 && ParametrosAplicacionDS.ParametroAplicacion.Select("Parametro = 'TipoCabecera'")[0]["Valor"].ToString() == "1")
                if (busqueda != null && busqueda.Valor == "1")
                {
                    mTipoCabecera = TipoCabeceraProyecto.Simplificada;
                    return mTipoCabecera.Value;
                }

                mTipoCabecera = TipoCabeceraProyecto.Normal;
                return mTipoCabecera.Value;
            }
        }

        /// <summary>
        /// Obtiene el nombre corto del proyecto al que se conecta por defecto siempre la aplicación
        /// </summary>
        public string NombreCortoProyectoPrincipal
        {
            get
            {
                if (mNombreCortoProyectoPrincipal == null)
                {
                    ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
                    mNombreCortoProyectoPrincipal = proyectoCN.ObtenerNombreCortoProyecto(ProyectoPrincipalUnico);
                    proyectoCN.Dispose();
                }

                return mNombreCortoProyectoPrincipal;
            }
        }

        /// <summary>
        /// Obtiene la URL de la petición actual para añadirla a una url con redirect: Ej de respuesta: /redirect/comunidad/materialeducativo/recurso...
        /// </summary>
        public string UrlParaLoginConRedirect
        {
            get
            {
                string url = mHttpContextAccessor.HttpContext.Request.Path.ToString();
                string redirect = "";
                string dominio = UtilDominios.ObtenerDominioUrl(new Uri(UriHelper.GetEncodedUrl(mHttpContextAccessor.HttpContext.Request)), true);
                if (url.Contains(dominio))
                {
                    url = url.Remove(0, url.IndexOf(dominio) + dominio.Length);
                }

                if (url.Length > 3)
                {
                    if (!url.StartsWith("/"))
                    {
                        url = "/" + url;
                    }

                    //Redirige al login de gnoss genérico
                    redirect = "/redirect" + url;
                }

                return redirect;
            }
        }

        /// <summary>
        /// Obtiene el dominio de la aplicación (gnoss.com, proyectos.gnoss.com, ...)
        /// </summary>
        public string DominoAplicacionConHTTP
        {
            get
            {
                if (mConfigService.PeticionHttps())
                {
                    return ObtenerDominioUrl(new Uri($"https://{mHttpContextAccessor.HttpContext.Request.Host}{mHttpContextAccessor.HttpContext.Request.Path}"), true);
                }
                else
                {
                    return ObtenerDominioUrl(new Uri($"http://{mHttpContextAccessor.HttpContext.Request.Host}{mHttpContextAccessor.HttpContext.Request.Path}"), true);
                }
            }
        }

        public UtilIdiomas UtilIdiomas
        {
            get
            {
                if (mUtilIdiomas == null)
                {
                    string[] array = { "es" };

                    if (ProyectoVirtual != null)
                    {
                        mUtilIdiomas = new UtilIdiomas(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + Path.DirectorySeparatorChar + "languages", array, IdiomaUsuario, ProyectoVirtual.Clave, ProyectoVirtual.PersonalizacionID, PersonalizacionEcosistemaID, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mLoggerFactory.CreateLogger<UtilIdiomas>(), mLoggerFactory);
                    }
                    else
                    {
                        mUtilIdiomas = new UtilIdiomas(IdiomaUsuario, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mLoggerFactory.CreateLogger<UtilIdiomas>(), mLoggerFactory);
                    }
                }
                return mUtilIdiomas;
            }
            set
            {
                mUtilIdiomas = value;
            }
        }

        //protected ParametroAplicacionDS ParametroAplicacionDS
        protected List<ParametroAplicacion> ParametroAplicacionDS
        {
            get
            {
                if (mListaParametrosAplicacion == null)
                {
                    mListaParametrosAplicacion = GestorParametrosAplicacion.ParametroAplicacion;
                }
                return mListaParametrosAplicacion;
            }
        }

        public string UrlIntragnoss
        {
            get
            {
                if (string.IsNullOrEmpty(mUrlIntragnoss))
                {
                    List<ParametroAplicacion> busqueda = ListaParametrosAplicacion.Where(parametro => parametro.Parametro.Equals("UrlIntragnoss")).ToList();
                    mUrlIntragnoss = busqueda.First().Valor;
                }
                return mUrlIntragnoss;
            }
        }

        /// <summary>
        /// Contiene la lista de permisos sobre los tipos de documentos de un proyecto.
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        public List<TiposDocumentacion> ListaPermisosDocumentosDeProyecto(Guid pProyectoID)
        {
            if (mListaPermisosDocumentos == null)
            {
                ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);
                mListaPermisosDocumentos = proyCL.ObtenerTiposDocumentosPermitidosUsuarioEnProyecto(pProyectoID, TipoRolUsuarioEnProyecto);
                proyCL.Dispose();
            }

            return mListaPermisosDocumentos;
        }

        public TipoRolUsuario TipoRolUsuarioEnProyecto
        {
            get
            {
                if (!mTipoRolUsuarioEnProyecto.HasValue)
                {
                    mTipoRolUsuarioEnProyecto = TipoRolUsuario.Usuario;

                    if (UsuarioActual != null && !UsuarioActual.EsIdentidadInvitada)
                    {
                        var filaAdmin = ProyectoSeleccionado.GestorProyectos.DataWrapperProyectos.ListaAdministradorProyecto.FirstOrDefault(a => a.UsuarioID == UsuarioActual.UsuarioID);
                        if (filaAdmin != null)
                        {
                            mTipoRolUsuarioEnProyecto = (TipoRolUsuario)filaAdmin.Tipo;
                        }
                    }
                }
                return mTipoRolUsuarioEnProyecto.Value;
            }
        }

        /// <summary>
        /// Contiene la lista de permisos sobre los tipos de documentos de un proyecto.
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <param name="pDocumentoID">Identificador del documento ID para el que se desea comprobar el acceso</param>
        /// <param name="pIdentidadDeOtroProyecto">Utiliza las identidades del resto de proyectos del usuario para comprobar si tiene permisos</param>
        /// <param name="pTieneProyectoPatronOntologias">Nos indica si tiene un proyecto patron para las ontologías y calcular los permisos utilizandolo</param>
        public bool ComprobarPermisoEnOntologiaDeProyectoEIdentidad(Guid pProyectoID, Guid pDocumentoID, bool pIdentidadDeOtroProyecto = true, bool pTieneProyectoPatronOntologias = false)
        {
            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
            
            bool tieneAcceso = true;
            if (!mListaOntologiasPermitidasPorIdentidad.ContainsKey(IdentidadActual.Clave) || !mListaOntologiasPermitidasPorIdentidad[IdentidadActual.Clave].ContainsKey(pDocumentoID))
            {
                if (!mListaOntologiasPermitidasPorIdentidad.ContainsKey(IdentidadActual.Clave))
                {
                    mListaOntologiasPermitidasPorIdentidad.TryAdd(IdentidadActual.Clave, new ConcurrentDictionary<Guid, bool>());
                }


                tieneAcceso = proyCN.ComprobarOntologiasPermitidaParaIdentidadEnProyecto(IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave, pProyectoID, TipoRolUsuarioEnProyecto, pIdentidadDeOtroProyecto, pDocumentoID);

                if (tieneAcceso)
                {
                    mListaOntologiasPermitidasPorIdentidad[IdentidadActual.Clave].TryAdd(pDocumentoID, true);
                }
                else
                {
                    mListaOntologiasPermitidasPorIdentidad[IdentidadActual.Clave].TryAdd(pDocumentoID, false);
                }
            }
            else
            {
                if (!mListaOntologiasPermitidasPorIdentidad[IdentidadActual.Clave][pDocumentoID] && pTieneProyectoPatronOntologias)
                {
                    mListaOntologiasPermitidasPorIdentidad[IdentidadActual.Clave][pDocumentoID] = proyCN.ComprobarOntologiasPermitidaParaIdentidadEnProyecto(IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave, new Guid(ParametroProyecto[ParametroAD.ProyectoIDPatronOntologias]), TipoRolUsuarioEnProyecto, pIdentidadDeOtroProyecto, pDocumentoID);
                }
                return mListaOntologiasPermitidasPorIdentidad[IdentidadActual.Clave][pDocumentoID];
            }

            return tieneAcceso;
        }

        public void LimpiarListaOntologiasPermitidasPorIdentidad()
        {
            mListaOntologiasPermitidasPorIdentidad = new ConcurrentDictionary<Guid, ConcurrentDictionary<Guid, bool>>();
        }

        public bool EsUsuarioAdministradorProyectoVirtual
        {
            get
            {
                //Es administrador
                if (ProyectoSeleccionado != ProyectoVirtual)
                {
                    //"UsuarioID = '" + Usuario.UsuarioActual.UsuarioID.ToString() + "'"
                    return ProyectoVirtual.GestorProyectos.DataWrapperProyectos.ListaAdministradorProyecto.Where(adminProy => adminProy.UsuarioID.Equals(UsuarioActual.UsuarioID)).ToList().Count > 0;
                }
                else
                {
                    return EsUsuarioAdministradorProyecto;
                }
            }
        }

        /// <summary>
        /// FilaProy con los contadores del proyecto actual.
        /// </summary>
        public AD.EntityModel.Models.ProyectoDS.Proyecto ContadoresProyecto
        {
            get
            {
                if (mContadoresProyecto == null)
                {
                    ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
                    mContadoresProyecto = proyCN.ObtenerContadoresProyecto(ProyectoSeleccionado.Clave).ListaProyecto[0];
                    proyCN.Dispose();
                }

                return mContadoresProyecto;
            }
        }

        public bool EsUsuarioAdministradorProyecto
        {
            get
            {
                return TipoRolUsuarioEnProyecto.Equals(TipoRolUsuario.Administrador);
            }
        }

        /// <summary>
        /// Contiene la lista de permisos sobre los tipos de documentos.
        /// </summary>
        public List<TiposDocumentacion> ListaPermisosDocumentos
        {
            get
            {
                if (mListaPermisosDocumentos == null)
                {
                    ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);
                    mListaPermisosDocumentos = proyCL.ObtenerTiposDocumentosPermitidosUsuarioEnProyecto(ProyectoVirtual.Clave, TipoRolUsuarioEnProyecto);
                    proyCL.Dispose();
                }
                return mListaPermisosDocumentos;
            }
        }

        private Guid[] ObtenerIdentidadUsuarioEnMygnoss(GnossIdentity pUsuario)
        {
            Guid perfilID = Guid.Empty;
            Guid identidadID = Guid.Empty;

            List<AD.EntityModel.Models.IdentidadDS.Perfil> filasPerfil = null;
            if (pUsuario.PersonaID == UsuarioAD.Invitado)
            {
                perfilID = UsuarioAD.Invitado;
                identidadID = UsuarioAD.Invitado;
            }
            else if (RequestParams("nombreOrgRewrite") != null)
            {

                //Perfil Profesional    //"NombreCortoOrg='" + RequestParams("nombreOrgRewrite") + "' AND PersonaID='" + Usuario.UsuarioActual.PersonaID + "'"
                filasPerfil = IdentidadActual.GestorIdentidades.DataWrapperIdentidad.ListaPerfil.Where(perf => perf.NombreCortoOrg != null && perf.NombreCortoOrg.ToLower().Equals(RequestParams("nombreOrgRewrite").ToLower()) && perf.PersonaID.HasValue && perf.PersonaID.Value.Equals(pUsuario.PersonaID)).ToList();

                if (filasPerfil.Count == 0)
                {
                    //Profesor
                    filasPerfil = IdentidadActual.GestorIdentidades.DataWrapperIdentidad.ListaPerfil.Where(perf => perf.NombreCortoUsu != null && perf.NombreCortoUsu.ToLower().Equals(RequestParams("nombreOrgRewrite").ToLower()) && perf.PersonaID.HasValue && perf.PersonaID.Value.Equals(pUsuario.PersonaID)).ToList();
                }

                if (filasPerfil.Count == 0)
                {
                    //Clase del profesor:
                    filasPerfil = IdentidadActual.GestorIdentidades.DataWrapperIdentidad.ListaPerfil.Where(perf => perf.NombreCortoOrg != null && perf.NombreCortoOrg.ToLower().Equals(RequestParams("nombreOrgRewrite").ToLower()) && !perf.PersonaID.HasValue).ToList();
                    if (filasPerfil != null && filasPerfil.Count > 0)
                    {
                        Guid organizacionID = filasPerfil[0].OrganizacionID.Value;

                        OrganizacionCN orgCN = new OrganizacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<OrganizacionCN>(), mLoggerFactory);
                        bool usuAdminClase = orgCN.EsUsuarioAdministradorClase(organizacionID, pUsuario.UsuarioID);
                        orgCN.Dispose();

                        if (usuAdminClase) //Es profesor
                        {
                            //Solo se debe cargar la fila de profesor   
                            List<AD.EntityModel.Models.IdentidadDS.Identidad> filasIdent = IdentidadActual.GestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Where(ident => ident.Tipo.Equals((short)TiposIdentidad.Profesor) && ident.ProyectoID.Equals(ProyectoAD.MetaProyecto) && !ident.FechaBaja.HasValue && !ident.FechaExpulsion.HasValue).ToList();

                            if (filasIdent.Count > 0)
                            {
                                Guid perfilProfesorID = (Guid)filasIdent[0].PerfilID;

                                filasPerfil = IdentidadActual.GestorIdentidades.DataWrapperIdentidad.ListaPerfil.Where(perf => perf.PerfilID.Equals(perfilProfesorID)).ToList();
                            }
                        }
                    }
                }

                if (filasPerfil.Count > 0)
                {//"PerfilID='" + perfilID + "' AND ProyectoID='" + ProyectoAD.MetaProyecto + "' AND FechaBaja is NULL AND FechaExpulsion is null"
                    perfilID = filasPerfil.First().PerfilID;
                    List<AD.EntityModel.Models.IdentidadDS.Identidad> filasIdent = IdentidadActual.GestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Where(ident => ident.PerfilID.Equals(perfilID) && ident.ProyectoID.Equals(ProyectoAD.MetaProyecto) && !ident.FechaBaja.HasValue && !ident.FechaExpulsion.HasValue).ToList();

                    if (filasIdent.Count > 0)
                    {
                        identidadID = filasIdent.First().IdentidadID;
                    }
                }
            }
            else
            {
                filasPerfil = IdentidadActual.GestorIdentidades.DataWrapperIdentidad.ListaPerfil.Where(perf => !perf.OrganizacionID.HasValue && perf.PersonaID.HasValue && perf.PersonaID.Value.Equals(pUsuario.PersonaID)).ToList();

                if (filasPerfil.Count > 0)
                {
                    List<AD.EntityModel.Models.IdentidadDS.Identidad> filasIdent = IdentidadActual.GestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Where(ident => ident.ProyectoID.Equals(ProyectoAD.MetaProyecto) && !ident.FechaBaja.HasValue && !ident.FechaExpulsion.HasValue && ident.Tipo.Equals((short)TiposIdentidad.Personal)).ToList();

                    AD.EntityModel.Models.IdentidadDS.Identidad filaIdent = null;
                    if (filasIdent.Count > 0)
                    {
                        mLoggingService.AgregarEntrada("Hay filas de identida personal");
                        filaIdent = filasIdent[0];

                        perfilID = filaIdent.PerfilID;
                        identidadID = filaIdent.IdentidadID;
                    }
                    else
                    {
                        mLoggingService.AgregarEntrada("No hay filas de identidad personal");
                        filasIdent = IdentidadActual.GestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Where(ident => ident.ProyectoID.Equals(ProyectoAD.MetaProyecto) && !ident.FechaBaja.HasValue && !ident.FechaExpulsion.HasValue).OrderBy(ident => ident.Tipo).ToList();

                        if (filasIdent.Count > 0)
                        {
                            filaIdent = filasIdent[0];
                            mHttpContextAccessor.HttpContext.Response.Redirect(BaseURLIdioma + "/" + UtilIdiomas.GetText("URLSEM", "IDENTIDAD") + "/" + filaIdent.Perfil.NombreCortoOrg + "/home");
                        }
                    }
                }
            }

            mIdentidadActual = null;

            if (identidadID == Guid.Empty && filasPerfil != null && filasPerfil.Count == 0 && RequestParams("nombreOrgRewrite") == null)
            {
                //El usuario no tiene identidad personal, cargamos la primera identidad de organización que venga. 
                filasPerfil = IdentidadActual.GestorIdentidades.DataWrapperIdentidad.ListaPerfil.Where(perf => perf.PersonaID.HasValue && perf.PersonaID.Value.Equals(pUsuario.PersonaID)).ToList();
                if (filasPerfil.Count > 0)
                {
                    perfilID = filasPerfil.First().PerfilID;

                    List<AD.EntityModel.Models.IdentidadDS.Identidad> filasIdent = IdentidadActual.GestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Where(ident => ident.ProyectoID.Equals(ProyectoAD.MetaProyecto) && !ident.FechaBaja.HasValue && !ident.FechaExpulsion.HasValue && ident.PerfilID.Equals(perfilID)).ToList();

                    if (filasIdent.Count > 0)
                    {
                        AD.EntityModel.Models.IdentidadDS.Identidad filaIdent = filasIdent.First();

                        perfilID = filaIdent.PerfilID;
                        identidadID = filaIdent.IdentidadID;
                    }
                    else
                    {
                        identidadID = UsuarioAD.Invitado;
                    }
                }
            }
            else if (filasPerfil != null && filasPerfil.Count == 0 && RequestParams("nombreOrgRewrite") != null)
            {
                return null;
            }

            return new Guid[] { perfilID, identidadID };
        }

        /// <summary>
        /// Elimina las variables de sesión al limpiarla desde las páginas.
        /// </summary>
        /// <param name="pPage">Página</param>
        /// <param name="pSesiones">Variables a mantener</param>
        /// <param name="pPerfilActualID">Identificador del perfil actual</param>
        public void LimpiarSesion(string[] pSesiones)
        {
            LimpiarSesion(pSesiones, false);
        }

        /// <summary>
        /// Elimina las variables de sesión al limpiarla desde las páginas.
        /// </summary>
        /// <param name="pPage">Página</param>
        /// <param name="pSesiones">Variables a mantener</param>
        public void LimpiarSesion(string[] pSesiones, bool pMantenerSolopSesiones)
        {
            if (!mUtilWeb.RequestUrl().Contains("img/")) //Solo si no es un postBack no deseado causado por una img que no existe
            {
                ////Eliminar todas las variables de sesión menos las pasadas como parámetro y las fijas
                //Dictionary<string, object> listaSesiones = new Dictionary<string, object>();


                ////SESSIONES FIJAS:
                ////ListaProyectosIdentidadActual - Lista de las comunidades que se pintan en la master <nombreCorto, <nombre, tipo>>;
                ////ListaPerfilesUsuarioActual - Lista de los perfiles que se pintan en la master;
                ////TodasMisIdentidades - Lista de <ProyectoID, IdentidadID> de todas las identidades del usuarioActual activas.
                ////TodasMisIdentidadesNoActivas - Lista de <ProyectoID, IdentidadID> de todas las identidades del usuarioActual NO activas.

                //string[] fijas = { "Usuario", "FilaUsuario", "PersonaDS", "DocumentoVisto", "VotacionDocumento", "VotacionComentario", "cookieSolicitada", "redirectAddToGnoss", "EnlaceDocumentoAgregar", "IdiomaUsuario", "ListaNumerosContribuciones", "MantenerConectado", "ListaBlogsUsuarios", "ListaBlogsUsuarioMasActivos", "UsuarioTieneDafos", "ListaGuidsPerfilesUsuarioActual", "EnvioCookie", "InicioRedirecciones", "NumeroRedirecciones", "tokenCookie", "CrearCookieEnServicioLogin", "ListaNombresDocumento", "authrequest" };
                //if (pMantenerSolopSesiones)
                //{
                //    fijas = new string[0];
                //}

                //string[] sesiones = new string[fijas.Length + pSesiones.Length];

                //Array.Copy(fijas, sesiones, fijas.Length);
                //Array.Copy(pSesiones, 0, sesiones, fijas.Length, pSesiones.Length);

                //// David: Añadir todas las sesiones a mantener a una lista
                //foreach (string s in sesiones)
                //{
                //    listaSesiones.Add(s, mHttpContextAccessor.HttpContext.Session.Get(s));
                //}
                //// David: Liberar toda la memoria de las variables a borrar y limpiar la lista
                //List<string> listaClavesVariablesSesion = new List<string>();

                //foreach (string clave in mHttpContextAccessor.HttpContext.Session.Keys)
                //{
                //    listaClavesVariablesSesion.Add(clave);
                //}

                //foreach (string clave in listaClavesVariablesSesion)
                //{
                //    object variable = mHttpContextAccessor.HttpContext.Session.Get(clave);

                //    if (variable != null && !listaSesiones.ContainsKey(clave) && variable is IDisposable)
                //    {
                //        //LOZA : Lo quito porque al trabajar con objetos en sesiones, si eliminamos uno que comparta hijos con una sesion buena (que salvamos) lo borra y casca en la mitad de páginas
                //        //((IDisposable)variable).Dispose();
                //        mLoggingService.AgregarEntrada("UtilSesion: Limpio la sesión de " + clave);
                //        mHttpContextAccessor.HttpContext.Session.Set(clave, null);
                //    }
                //}
                mHttpContextAccessor.HttpContext.Session.Clear();

                // David: Volver a añadir las variables fijas
                //foreach (string s in listaSesiones.Keys)
                //{
                //    mHttpContextAccessor.HttpContext.Session.Set(s, listaSesiones[s]);
                //}
            }
        }

        /// <summary>
        /// Obtiene si se trata de un ecosistema sin contactos
        /// </summary>
        public bool EsEcosistemaSinContactos
        {
            get
            {
                if (!mEsEcosistemaSinContactos.HasValue)
                {
                    // EntityContext context = EntityContext.Instance;
                    //List<AD.EntityModel.ParametroAplicacion> busqueda = context.ParametroAplicacion.Where(parametro => parametro.Parametro.Equals(TiposParametrosAplicacion.EcosistemaSinBandejaSuscripciones)).ToList();
                    ParametroAplicacion busqueda = GestorParametrosAplicacion.ParametroAplicacion.FirstOrDefault(parametro => parametro.Parametro.Equals(TiposParametrosAplicacion.EcosistemaSinBandejaSuscripciones));
                    mEsEcosistemaSinContactos = busqueda != null && bool.Parse(busqueda.Valor);
                    // mEsEcosistemaSinContactos = ParametrosAplicacionDS.ParametroAplicacion.Select("Parametro = '" + TiposParametrosAplicacion.EcosistemaSinContactos.ToString() + "'").Length > 0 && bool.Parse((string)ParametrosAplicacionDS.ParametroAplicacion.Select("Parametro = '" + TiposParametrosAplicacion.EcosistemaSinContactos.ToString() + "'")[0]["Valor"]);
                }
                return mEsEcosistemaSinContactos.Value;
            }
        }

        /// <summary>
        /// Obtiene si se trata de un ecosistema sin contactos
        /// </summary>
        public bool EsEdicionMultiple
        {
            get
            {
                if (!mEsEdicionMultiple.HasValue)
                {
                    mEsEdicionMultiple = ListaParametrosAplicacion.Where(parametroApp => parametroApp.Parametro.Equals(TiposParametrosAplicacion.EdicionMultiple.ToString())).ToList().Count > 0 && bool.Parse((string)ListaParametrosAplicacion.Find(parametroApp => parametroApp.Parametro.Equals(TiposParametrosAplicacion.EdicionMultiple.ToString())).Valor);
                }
                return mEsEdicionMultiple.Value;
            }
        }

        /// <summary>
        /// Cambia el perfil del usuario conectado
        /// </summary>
        /// <param name="pIdentidadCambio">Fila de identidad</param>
        public void CambiarPerfilUsuario(Es.Riam.Gnoss.AD.EntityModel.Models.IdentidadDS.Identidad pIdentidadCambio)
        {
            if (pIdentidadCambio != null)
            {
                //Cambio la cookie de perfiles para que la lea el módulo de rewrite
                ActualizarCookiePerfil(pIdentidadCambio.PerfilID, pIdentidadCambio.IdentidadID, UsuarioActual.UsuarioID);

                UsuarioActual.IdentidadID = pIdentidadCambio.IdentidadID;
                UsuarioActual.PerfilID = pIdentidadCambio.PerfilID;
                UsuarioActual.ProyectoID = pIdentidadCambio.ProyectoID;

                //UsuarioCN usuarioCN = new UsuarioCN();
                //UsuarioDS usuDS = usuarioCN.ObtenerRolUsuarioEnProyecto(Usuario.UsuarioActual.ProyectoID, Usuario.UsuarioActual.UsuarioID);

                //ProyectoCL proyCL = new ProyectoCL();
                //Usuario.UsuarioActual.RolPermitidoProyecto = proyCL.CalcularRolFinalUsuarioEnProyecto(Usuario.UsuarioActual.UsuarioID, Usuario.UsuarioActual.Login, Usuario.UsuarioActual.OrganizacionID, Usuario.UsuarioActual.ProyectoID, true, usuDS);

                UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication,mLoggerFactory.CreateLogger<UsuarioCN>(), mLoggerFactory);
                DataWrapperUsuario dataWrapperUsuario = usuarioCN.ObtenerRolesUsuarioPorPerfilYProyecto(UsuarioActual.UsuarioID, UsuarioActual.ProyectoID, UsuarioActual.PerfilID);
                usuarioCN.Dispose();

                if (dataWrapperUsuario.ListaOrganizacionRolUsuario.Count > 0)
                {
                    AD.EntityModel.Models.UsuarioDS.OrganizacionRolUsuario filaOrganizacionRolUsuario = dataWrapperUsuario.ListaOrganizacionRolUsuario.First();

                    if (!UsuarioActual.ListaRolesOrganizacion.ContainsKey(filaOrganizacionRolUsuario.OrganizacionID))
                    {
                        ulong rolPermitidoUsuarioOrganizacion = 0;
                        ulong rolDenegadoUsuarioOrganizacion = 0;

                        if (filaOrganizacionRolUsuario.RolPermitido != null)
                        {
                            rolPermitidoUsuarioOrganizacion = System.Convert.ToUInt64(filaOrganizacionRolUsuario.RolPermitido, 16);
                        }

                        if (filaOrganizacionRolUsuario.RolDenegado != null)
                        {
                            rolDenegadoUsuarioOrganizacion = System.Convert.ToUInt64(filaOrganizacionRolUsuario.RolDenegado, 16);
                        }

                        UsuarioActual.ListaRolesOrganizacion.Add(filaOrganizacionRolUsuario.OrganizacionID, rolPermitidoUsuarioOrganizacion & ~(rolDenegadoUsuarioOrganizacion));
                    }
                }

                UsuarioActual.RolPermitidoProyecto = (ulong)(0ul);
                try
                {
                    ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);
                    UsuarioActual.RolPermitidoProyecto = proyCL.CalcularRolFinalUsuarioEnProyecto(UsuarioActual.UsuarioID, UsuarioActual.Login, UsuarioActual.OrganizacionID, UsuarioActual.ProyectoID, dataWrapperUsuario);
                    proyCL.Dispose();
                }
                catch (Exception)
                {
                    UsuarioActual.RolPermitidoProyecto = ulong.Parse("0000000000000000");
                }


                UsuarioActual.EsIdentidadInvitada = pIdentidadCambio.IdentidadID.Equals(UsuarioAD.Invitado);

                if (pIdentidadCambio.Tipo.Equals((short)TiposIdentidad.Profesor))
                {
                    UsuarioActual.PerfilProfesorID = pIdentidadCambio.PerfilID;
                }
                else
                {
                    UsuarioActual.PerfilProfesorID = null;
                }
            }
        }

        bool? mEstaUsuarioBloqueadoProyecto = null;

        public bool EstaUsuarioBloqueadoProyecto
        {
            get
            {
                if (!mEstaUsuarioBloqueadoProyecto.HasValue)
                {
                    ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);
                    mEstaUsuarioBloqueadoProyecto = proyCL.ObtenerUsuarioBloqueadoProyecto(ProyectoVirtual.Clave, UsuarioActual.UsuarioID);
                    proyCL.Dispose();
                }
                return mEstaUsuarioBloqueadoProyecto.Value;
            }
        }

        /// <summary>
        /// Nombre del proyecto actual o del de OrigenBusqueda si hay.
        /// </summary>
        public string NombreProyBusquedaOActual
        {
            get
            {
                if (ProyectoOrigenBusqueda != null)
                {
                    return ProyectoOrigenBusqueda.NombreCorto;
                }
                else
                {
                    return ProyectoSeleccionado.NombreCorto;
                }
            }
        }

        /// <summary>
        /// Obtiene el proyecto externo en el que se está haciendo la búsqueda, o NULL si se hace en el proyecto actual.
        /// </summary>
        public Elementos.ServiciosGenerales.Proyecto ProyectoOrigenBusqueda
        {
            get
            {
                if (ProyectoOrigenBusquedaID != Guid.Empty && (mProyectoOrigenBusqueda == null || mProyectoOrigenBusqueda.Clave != ProyectoOrigenBusquedaID))
                {
                    ProyectoCL proyectoCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);
                    GestionProyecto gestorProyecto = new GestionProyecto(proyectoCL.ObtenerProyectoPorID(ProyectoOrigenBusquedaID), mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestionProyecto>(), mLoggerFactory);
                    proyectoCL.Dispose();

                    if (gestorProyecto.ListaProyectos.Count > 0 && gestorProyecto.ListaProyectos.ContainsKey(ProyectoOrigenBusquedaID))
                    {
                        mProyectoOrigenBusqueda = gestorProyecto.ListaProyectos[ProyectoOrigenBusquedaID];
                    }
                }
                return mProyectoOrigenBusqueda;
            }
        }

        /// <summary>
        /// Obtiene el dominio de una URL (de la manera .gnoss.com)
        /// </summary>
        /// <param name="pUrl">Url de la que se quiere saber su dominio</param>
        /// <param name="pConApplicationPath">Verdad si se quiere añadir el applicationPath después del dominio (/Es.Riam.Gnoss.Web.Principal). Por defecto True</param>
        /// <param name="pConHTTP">Verdad si se debe añadir http:// al principio del dominio</param>
        /// <returns></returns>
        public static string ObtenerDominioUrl(Uri pUrl, bool pConHTTP)
        {
            return UtilDominios.ObtenerDominioUrl(pUrl, pConHTTP);
        }

        /// <summary>
        /// Obtiene la cadena http:// o https:// para la petición actual
        /// </summary>
        public string HTTP
        {
            get
            {
                if (mConfigService.PeticionHttps())
                {
                    return "https://";
                }
                else
                {
                    return "http://";
                }
            }
        }

        /// <summary>
        /// Obtiene el identificador del proyecto GNOSS
        /// </summary>
        public Guid ProyectoGnoss
        {
            get
            {

                Guid? proyectoID = mConfigService.ObtenerProyectoGnoss();

                if (!proyectoID.HasValue)
                {
                    return ProyectoAD.MyGnoss;
                }
                return proyectoID.Value;
            }
        }

        /// <summary>
        /// Obtiene si el dominio actual es el dominio de las comunidades por defecto (http://comunidades.gnoss.com)
        /// </summary>
        public bool EsDominioComunidades
        {
            get
            {
                return mConfigService.SitioComunidadesPorDefecto();
            }
        }

        public void ExpirarCookies(bool pExisteNombrePoliticaCookiesMetaproyecto, string pNombrePoliticaCookiesMetaproyecto, GnossIdentity pUsuario = null)
        {
            if (pUsuario == null)
            {
                pUsuario = UsuarioActual;
            }

            string nombreCookieAviso = "cookieAviso";

            try
            {
                if (pUsuario != null)
                {
                    ParametroCN paramCN = new ParametroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroCN>(), mLoggerFactory);
                    Dictionary<string, string> parametrosProyecto = paramCN.ObtenerParametrosProyecto(pUsuario.ProyectoID);
                    paramCN.Dispose();

                    if (parametrosProyecto != null && parametrosProyecto.ContainsKey(ParametroAD.NombrePoliticaCookies))
                    {
                        if (!string.IsNullOrEmpty(parametrosProyecto[ParametroAD.NombrePoliticaCookies]))
                        {
                            nombreCookieAviso = parametrosProyecto[ParametroAD.NombrePoliticaCookies];
                        }
                    }
                    else if (pExisteNombrePoliticaCookiesMetaproyecto)
                    {
                        nombreCookieAviso = pNombrePoliticaCookiesMetaproyecto;
                    }
                }
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, "Error al obtener el nombre de la cookie " + ParametroAD.NombrePoliticaCookies, mlogger);
            }

            foreach (string key in mHttpContextAccessor.HttpContext.Request.Cookies.Keys)
            {
                string cookie = mHttpContextAccessor.HttpContext.Request.Cookies[key];
                if (cookie != null && !key.Equals("idioma" + DominoAplicacion) && !key.Equals(nombreCookieAviso + DominoAplicacion))
                {
                    ExpirarCookie(key);
                }
            }
        }

        /// <summary>
        /// Redirige al usuario invitado a la página de login para que se identifique o se registre
        /// </summary>
        /// <param name="pBaseUrl">URL base sin idioma</param>
        /// <param name="pBaseUrlIdioma">URL base con idioma</param>
        /// <param name="pUtilIdiomas">Util idiomas</param>
        public void RedirigirUsuarioInvitadoARegistrarse(string pBaseUrl, string pBaseUrlIdioma, UtilIdiomas pUtilIdiomas)
        {
            //Obtengo la url a la que intenta acceder
            string redirect = "";
            //string url = pPage.Request.Url.ToString();
            string url = new UtilWeb(mHttpContextAccessor).RequestUrl();

            if ((!url.Contains("anyadirGnoss?addToGnoss=")) && (!url.ToLower().Contains("download-file")) && (url.Contains("?")))
            {
                url = url.Remove(url.IndexOf('?'));
            }
            url = url.Replace("\n", "").Replace("\t", "");

            //Hay que codificar el titulo del addToGnoss:
            if (url.Contains("anyadirGnoss.aspx?addToGnoss="))
            {
                int indiceTitulo = url.IndexOf("&titl=");

                if (indiceTitulo != -1)
                {
                    string cadenaAPartirTitulo = url.Substring(indiceTitulo + 6);
                    int indiceSiguienteParametro = cadenaAPartirTitulo.IndexOf("&descp=");

                    if (indiceSiguienteParametro != -1)
                    {
                        string titulo = cadenaAPartirTitulo.Substring(0, indiceSiguienteParametro);
                        url = url.Replace(titulo, HttpUtility.UrlEncode(titulo));
                    }
                }
            }
            string urlOriginal = url;

            if (url.Contains(pBaseUrl))
            {
                url = url.Remove(0, url.IndexOf(pBaseUrl) + pBaseUrl.Length);
            }

            //if (url.Contains(UtilUsuario.HTTP) && url.IndexOf(UtilUsuario.HTTP) == 0)
            //{
            //    url = url.Remove(0, UtilUsuario.HTTP.Length);
            //    url = url.Remove(0, url.IndexOf("/"));
            //}

            #region Registro profesor

            if (url.Contains(pUtilIdiomas.GetText("URLSEM", "REGISTROPROFESOR")))
            {
                url = url.Replace(pUtilIdiomas.GetText("URLSEM", "NUEVOUSUARIO"), "");

                if (url[url.Length - 1] == '/')
                {
                    url = url.Remove(url.Length - 1);
                }
            }

            #endregion

            if (url.Length > 1)
            {
                //Compruebo si hay que redirigir a una comunidad o a myGnoss

                if (url.Contains("/redirect/"))
                {
                    //ya se ha hecho una redirección, no voy a hacer más
                    return;
                }

                char[] separadores = { ',' };
                string[] trozos = url.Split(separadores, StringSplitOptions.RemoveEmptyEntries);

                if (url.Trim().StartsWith("/" + pUtilIdiomas.GetText("URLSEM", "COMUNIDAD") + "/") && (trozos.Length > 1))
                {
                    //Redirige al login de una comunidad en castellano
                    redirect = "/" + trozos[0] + "/" + trozos[1];
                }
                else if ((trozos.Length > 2) && (UtilCadenas.RegexPrefijoIdioma.IsMatch(trozos[0])) && (trozos[1].Trim().Equals(pUtilIdiomas.GetText("URLSEM", "COMUNIDAD"))))
                {
                    //Redririge al login de una comunidad en inglés
                    redirect = "/" + trozos[0] + "/" + trozos[1] + "/" + trozos[2];
                }

                redirect += "/redirect" + url;
            }
            //if (!MaximoRedireccionesExcedidas())
            //{
            //    //Realiza la redirección
            //    if (redirect.StartsWith("http"))
            //    {
            //        pPage.Response.Redirect(redirect);
            //    }
            //    else
            //    {
            //        if (!redirect.Contains("/login/redirect"))
            //        {
            //            pPage.Response.Redirect(pBaseUrlIdioma + "/" + pUtilIdiomas.GetText("URLSEM", "LOGIN") + redirect);
            //        }
            //    }
            //}
        }

        /// <summary>
        /// Identificador del proyecto al que se conecta la aplicación
        /// </summary>
        private static Guid? mProyectoConexion = null;

        /// <summary>
        /// Obtiene el identificador del proyecto al que se conecta la aplicación
        /// </summary>
        public Guid ProyectoConexion
        {
            get
            {
                ParametroAplicacionCN paramCN = new ParametroAplicacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCN>(), mLoggerFactory);
                Guid? proyConexion = mConfigService.ObtenerProyectoConexion();
                if (proyConexion == null || proyConexion.Equals(Guid.Empty))
                {
                    string valor = paramCN.ObtenerParametroAplicacion(ProyectoSeleccionado.UrlPropia(IdiomaUsuario));

                    if (!string.IsNullOrEmpty(valor))
                    {
                        proyConexion = new Guid(valor);
                    }
                }
                if (proyConexion.HasValue)
                {
                    mProyectoConexion = proyConexion;
                }
                else
                {
                    mProyectoConexion = ProyectoAD.MetaProyecto;
                }
                return mProyectoConexion.Value;
            }
        }

        public void ExpirarCookie(string pCookieKey)
        {
            if (pCookieKey.Equals("UsuarioLogueado"))
            {
                string[] dominio = DominoAplicacion.Split(".", StringSplitOptions.RemoveEmptyEntries);
                if (dominio.Length > 1 && string.IsNullOrEmpty(DominioPaginasAdministracion))
                {
                    mHttpContextAccessor.HttpContext.Response.Cookies.Append(pCookieKey, "0", new CookieOptions { Expires = DateTime.Now.AddDays(-1), Domain = $".{dominio[dominio.Length - 2]}.{dominio[dominio.Length - 1]}" });
                    mLoggingService.GuardarLog($"Eliminamos la cookie del subdominio: .{dominio[dominio.Length - 2]}.{dominio[dominio.Length - 1]}", mlogger);
                }
                else
                {
                    mHttpContextAccessor.HttpContext.Response.Cookies.Append(pCookieKey, "0", new CookieOptions { Expires = DateTime.Now.AddDays(-1), Domain = DominoAplicacion });
                    mLoggingService.GuardarLog($"Eliminamos la cookie del dominio: .{DominoAplicacion}", mlogger);
                }
            }
            else if (pCookieKey.StartsWith("rewritte"))
            {
                mHttpContextAccessor.HttpContext.Response.Cookies.Append(pCookieKey, "0", new CookieOptions { Expires = DateTime.Now.AddDays(-1), Domain = DominoAplicacion });
            }
            else
            {
                mHttpContextAccessor.HttpContext.Response.Cookies.Append(pCookieKey, "0", new CookieOptions { Expires = DateTime.Now.AddDays(-1) });
            }

        }

        public void CrearCookieLogueado()
        {
            if (!mHttpContextAccessor.HttpContext.Request.Cookies.ContainsKey("UsuarioLogueado"))
            {
                //Si no existe cookie de usuario logueado para este dominio, la creo
                CookieOptions usuarioLogueadoOptions = new CookieOptions();
                string[] dominio = DominoAplicacion.Split('.', StringSplitOptions.RemoveEmptyEntries);

                if (dominio.Length > 1 && string.IsNullOrEmpty(DominioPaginasAdministracion))
                {
                    usuarioLogueadoOptions.Domain = $".{dominio[dominio.Length - 2]}.{dominio[dominio.Length - 1]}";
                }
                else
                {
                    usuarioLogueadoOptions.Domain = DominoAplicacion;
                }

                usuarioLogueadoOptions.Expires = DateTime.Now.AddDays(1);

                if (mConfigService.PeticionHttps())
                {
                    usuarioLogueadoOptions.Secure = true;
                    //usuarioLogueado.Path += "; SameSite=None";
                }

                mHttpContextAccessor.HttpContext.Response.Cookies.Append("UsuarioLogueado", "1", usuarioLogueadoOptions);
            }
        }

        /// <summary>
        /// Obtiene la URL base de la página en el idioma correspondiente
        /// </summary>
        public string BaseURLIdioma
        {
            get
            {
                if (string.IsNullOrEmpty(mBaseUrlIdioma) || !string.IsNullOrEmpty(DominioPaginasAdministracion))
                {
                    string idioma = "";
                    if (UtilIdiomas.LanguageCode != GnossUrlsSemanticas.IdiomaPrincipalDominio)
                    {
                        idioma = "/" + UtilIdiomas.LanguageCode;
                    }

                    string host = mHttpContextAccessor.HttpContext.Request.Host.ToString();
                    string url = BaseURL;
                    if (!EsDominioAdministracion(host) && (EsDominioComunidades || !ProyectoConexionID.Equals(ProyectoAD.MetaProyecto)))
                    {
                        url = UrlPrincipal;
                    }

                    mBaseUrlIdioma = url + idioma;
                }
                return mBaseUrlIdioma;
            }
        }

        /// <summary>
        /// Indica si debe aceptar automáticamente los registros en el ecosistema o mantenerlos en espera hasta que se acepte la solicitud por el administrador
        /// </summary>
        public bool RegistroAutomaticoEcosistema
        {
            get
            {
                if (!mRegistroAutomaticoEcosistema.HasValue)
                {
                    mRegistroAutomaticoEcosistema = true;
                    List<ParametroAplicacion> busqueda = ListaParametrosAplicacion.Where(parametro => parametro.Parametro.Equals(TiposParametrosAplicacion.RegistroAutomaticoEcosistema)).ToList();
                    if (busqueda.Count > 0)
                    {
                        mRegistroAutomaticoEcosistema = bool.Parse(busqueda.First().Valor);
                    }
                }
                return mRegistroAutomaticoEcosistema.Value;
            }
        }

        /// <summary>
        /// Obtiene si los enlaces tienen que apuntar al perfil global en la comunidad principal
        /// </summary>
        public bool PerfilGlobalEnComunidadPrincipal
        {
            get
            {
                if (!mPerfilGlobalEnComunidadPrincipal.HasValue)
                {
                    //EntityContext context = EntityContext.Instance;
                    // List<AD.EntityModel.ParametroAplicacion> busqueda = context.ParametroAplicacion.Where(parametro => parametro.Parametro.Equals( TiposParametrosAplicacion.PerfilGlobalEnComunidadPrincipal)).ToList();
                    List<AD.EntityModel.ParametroAplicacion> busqueda = ListaParametrosAplicacion.Where(parametro => parametro.Parametro.Equals(TiposParametrosAplicacion.PerfilGlobalEnComunidadPrincipal)).ToList();
                    mPerfilGlobalEnComunidadPrincipal = busqueda.Count > 0 && bool.Parse(busqueda.First().Valor);
                    //mPerfilGlobalEnComunidadPrincipal = ParametrosAplicacionDS.ParametroAplicacion.Select("Parametro = '" + TiposParametrosAplicacion.PerfilGlobalEnComunidadPrincipal.ToString() + "'").Length > 0 && bool.Parse((string)ParametrosAplicacionDS.ParametroAplicacion.Select("Parametro = '" + TiposParametrosAplicacion.PerfilGlobalEnComunidadPrincipal.ToString() + "'")[0]["Valor"]);
                }
                return mPerfilGlobalEnComunidadPrincipal.Value;
            }
        }

        /// <summary>
        /// Indica si el registro es abierto para una comunidad, aunque en el ecosistema no lo sea
        /// </summary>
        public bool RegistroAutomaticoEnComunidad
        {
            get
            {
                if (!mRegistroAutomaticoEnComunidad.HasValue)
                {
                    mRegistroAutomaticoEnComunidad = ParametroProyecto.ContainsKey(ParametroAD.RegistroAbiertoEnComunidad) && ParametroProyecto[ParametroAD.RegistroAbiertoEnComunidad].Equals("1");

                }
                return mRegistroAutomaticoEnComunidad.Value;
            }
        }

        /// <summary>
        /// Obtiene el proyecto al que se conecta siempre la aplicación
        /// </summary>
        public Guid ProyectoConexionID
        {
            get
            {
                if (mProyectoConexionID == null)
                {
                    ParametroAplicacionCN paramCN = new ParametroAplicacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCN>(), mLoggerFactory);
                    mProyectoConexionID = mConfigService.ObtenerProyectoConexion();
                    if (mProyectoConexionID == null || mProyectoConexionID.Equals(Guid.Empty))
                    {
                        string valor = paramCN.ObtenerParametroAplicacion(ProyectoSeleccionado.UrlPropia(IdiomaUsuario));

                        if (!string.IsNullOrEmpty(valor))
                        {
                            mProyectoConexionID = new Guid(valor);
                        }
                    }
                    if (!mProyectoConexionID.HasValue)
                    {
                        mProyectoConexionID = ProyectoAD.MetaProyecto;
                    }
                }
                return mProyectoConexionID.Value;
            }
        }

		public string CargarImagenSup(Guid pProyecto)
		{
			string rutaImg = "";
			ServicioImagenes servicioImagenes = new ServicioImagenes(mLoggingService, mConfigService, mLoggerFactory.CreateLogger<ServicioImagenes>(), mLoggerFactory);
			servicioImagenes.Url = UrlIntragnossServicios;
            string ruta = $"{UtilArchivos.ContentImagenesProyectos}/{pProyecto.ToString().ToLower()}";
            string rutaOriginal = $"{ruta}_sup_grande_temp";
            byte[] imagenOriginalBytes = servicioImagenes.ObtenerImagen(rutaOriginal, ".png");

            if (imagenOriginalBytes != null)
            {
                rutaImg = $"{BaseURLContent}/{UtilArchivos.ContentImagenes}/{rutaOriginal}.png";
            }

            return rutaImg;
        }

        /// <summary>
        /// Redirige a la página solicitarCookie.aspx, le pide la cookie al ser servicio de login y de hay redirige a la página actual
        /// </summary>
        public RedirectResult SolicitarCookieLoginUsuario(GnossIdentity pUsuario, string pTokenLoginUsuario)
        {
            var usuarioLogueado = mHttpContextAccessor.HttpContext.Request.Cookies["UsuarioLogueado"];

            bool solicitarCookieSiempre = false;
            ParametroAplicacion busqueda = GestorParametrosAplicacion.ParametroAplicacion.FirstOrDefault(parametro => parametro.Parametro == "SolicitarSiempreCookieLogin");

            if (busqueda != null && busqueda.Valor == "1")
            {
                solicitarCookieSiempre = true;
            }

            //Si la página es RSS no solicita cookie
            if (((usuarioLogueado != null && usuarioLogueado.Equals("1")) || solicitarCookieSiempre) && mHttpContextAccessor.HttpContext.Request.Method.Equals("GET") && (!mHttpContextAccessor.HttpContext.Request.GetDisplayUrl().EndsWith("?rss")) && !mHttpContextAccessor.HttpContext.Request.Host.ToString().Contains("anyadir-gnoss-curriculum") && !mHttpContextAccessor.HttpContext.Request.Query.ContainsKey("buscar") && (ProyectoSeleccionado.Clave.Equals(ProyectoAD.MetaProyecto) || (ProyectoSeleccionado.TipoAcceso != TipoAcceso.Reservado && ProyectoSeleccionado.TipoAcceso != TipoAcceso.Privado) || ParametrosGeneralesRow.SolicitarCoockieLogin))
            {
                if (!mHttpContextAccessor.HttpContext.Session.Keys.Contains("cookieSolicitada") && (!EsBot))
                {
                    string url = UtilDominios.ObtenerDominioUrl(mHttpContextAccessor.HttpContext.Request.GetDisplayUrl(), true);
                    if (mHttpContextAccessor.HttpContext.Request.Path.Equals("/anyadirGnoss"))
                    {
                        url += "/anyadirGnoss?addToGnoss=" + mHttpContextAccessor.HttpContext.Request.Query["addToGnoss"] + "&verAddTo=" + mHttpContextAccessor.HttpContext.Request.Query["verAddTo"];
                    }
                    else
                    {
                        url = mHttpContextAccessor.HttpContext.Request.GetDisplayUrl();
                    }

                    if (mConfigService.PeticionHttps())
                    {
                        url = url.Replace("http://", "https://");
                    }

                    url = url.Replace("\n", "").Replace("\t", "");

                    if (url.ToLower().Contains("/prehome/"))
                    {
                        url = UtilDominios.ObtenerDominioUrl(mHttpContextAccessor.HttpContext.Request.GetDisplayUrl(), true);
                    }

                    mHttpContextAccessor.HttpContext.Session.Set("cookieSolicitada", DateTime.Now);

                    string urlServicioLogin = mConfigService.ObtenerUrlServicioLogin();

                    string query = $"urlVuelta={BaseURL}/&token={pTokenLoginUsuario.Replace("\"", "")}&redirect={HttpUtility.UrlEncode(url)}&proyectoID={ProyectoSeleccionado.Clave}";

                    string urlRedireccion = urlServicioLogin + "/obtenerCookie?" + query;

                    return new RedirectResult(urlRedireccion);
                }
            }

            return null;
        }

        /// <summary>
        /// Guarda el log del tamaño.
        /// </summary>
        public static void GuardarLogSize(string pError, string pRutaFicheroError)
        {
            //Si el fichero supera el tamaño máximo lo elimino
            if (File.Exists(pRutaFicheroError))
            {
                FileInfo fichero = new FileInfo(pRutaFicheroError);
                if (fichero.Length > 1000000)
                {
                    fichero.Delete();
                }
            }

            //Añado el error al fichero
            using (StreamWriter sw = new StreamWriter(pRutaFicheroError, true, System.Text.Encoding.Default))
            {
                sw.WriteLine(Environment.NewLine + "Fecha: " + DateTime.Now + Environment.NewLine + Environment.NewLine);
                // Escribo el error
                sw.Write(pError);
                sw.WriteLine(Environment.NewLine + Environment.NewLine + "___________________________________________________________________________________________" + Environment.NewLine + Environment.NewLine + Environment.NewLine);
            }
        }

        /// <summary>
        /// Obtiene si se trata de un ecosistema sin Bandeja de Suscripcione
        /// </summary>
        public bool EsEcosistemaSinBandejaSuscripciones
        {
            get
            {
                if (!mEsEcosistemaSinBandejaSuscripciones.HasValue)
                {
                    //EntityContext context = EntityContext.Instance;
                    //List<AD.EntityModel.ParametroAplicacion> busqueda = context.ParametroAplicacion.Where(parametro => parametro.Parametro.Equals( TiposParametrosAplicacion.EcosistemaSinBandejaSuscripciones)).ToList();
                    ParametroAplicacion busqueda = GestorParametrosAplicacion.ParametroAplicacion.FirstOrDefault(parametro => parametro.Parametro.Equals(TiposParametrosAplicacion.EcosistemaSinBandejaSuscripciones));
                    mEsEcosistemaSinBandejaSuscripciones = busqueda != null && bool.Parse(busqueda.Valor);
                    //mEsEcosistemaSinBandejaSuscripciones = ParametrosAplicacionDS.ParametroAplicacion.Select("Parametro = '" + TiposParametrosAplicacion.EcosistemaSinBandejaSuscripciones.ToString() + "'").Length > 0 && bool.Parse((string)ParametrosAplicacionDS.ParametroAplicacion.Select("Parametro = '" + TiposParametrosAplicacion.EcosistemaSinBandejaSuscripciones.ToString() + "'")[0]["Valor"]);
                }
                return mEsEcosistemaSinBandejaSuscripciones.Value;
            }
        }

        public string IdiomaPorDefecto
        {
            get
            {
				ParametroAplicacionCL paramCL = new ParametroAplicacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCL>(), mLoggerFactory);
				return !(ParametrosGeneralesRow.IdiomaDefecto == null) ? ParametrosGeneralesRow.IdiomaDefecto : paramCL.ObtenerListaIdiomas().First();
            }
        }

        /// <summary>
        /// Obtiene la URL del servicio de login
        /// </summary>
        public string UrlServicioLogin
        {
            get
            {
                if (mUrlServicioLogin == null)
                {
                    mUrlServicioLogin = mConfigService.ObtenerUrlServicioLogin();
                }

                return mUrlServicioLogin;
            }
        }

        public static Dictionary<Guid, string> UrlServicioResultadosContextos
        {
            get;
            set;
        }

        /// <summary>
        /// Actualiza la cookie del perfil actual del usuario
        /// </summary>
        /// <param name="pPerfilID">Identificador del perfil</param>
        /// <param name="pIdentidadID">Identificador de la identidad</param>
        public void ActualizarCookiePerfil(Guid pPerfilID, Guid pIdentidadID, Guid pUsuarioID)
        {
            IdentidadCL identCL = new IdentidadCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCL>(), mLoggerFactory);
            identCL.AgregarCacheIdentidadActualUsuario(pUsuarioID, pPerfilID, pIdentidadID);
        }

        /// <summary>
        /// Calcula los permisos del usuario
        /// </summary>
        /// <param name="pFilaUsuario">Fila del usuario</param>
        /// <param name="pIdentity">GnossIdentity del usuario</param>
        /// <param name="pPerfilID">Identificador del perfil de la identidad con la que se conecta el usuario</param>
        public void CalcularPermisosUsuario(AD.EntityModel.Models.UsuarioDS.Usuario pFilaUsuario, GnossIdentity pIdentity)
        {
            UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication,mLoggerFactory.CreateLogger<UsuarioCN>(), mLoggerFactory);
            DataWrapperUsuario dataWrapperUsuario = usuarioCN.ObtenerRolesUsuarioPorPerfilYProyecto(pFilaUsuario.UsuarioID, pIdentity.ProyectoID, pIdentity.PerfilID);
            usuarioCN.Dispose();

            try
            {
                //Declaramos roles por defecto 
                ulong rolPermitidoUsuario = 0;
                ulong rolDenegadoUsuario = 0;


                if ((dataWrapperUsuario == null) || (dataWrapperUsuario.ListaGeneralRolUsuario.Count == 0))
                {
                    throw new Exception("Se desconoce el rol de usuario");
                }

                if (dataWrapperUsuario.ListaGeneralRolUsuario.First().RolPermitido != null)
                {
                    rolPermitidoUsuario = Convert.ToUInt64(dataWrapperUsuario.ListaGeneralRolUsuario.First().RolPermitido, 16);
                }

                if (dataWrapperUsuario.ListaGeneralRolUsuario.First().RolDenegado != null)
                {
                    rolDenegadoUsuario = Convert.ToUInt64(dataWrapperUsuario.ListaGeneralRolUsuario.First().RolDenegado, 16);
                }

                pIdentity.RolPermitidoGeneral = rolPermitidoUsuario & ~rolDenegadoUsuario;
            }
            catch (Exception)
            {
                pIdentity.RolPermitidoGeneral = ulong.Parse("0000000000000000");
            }

            ////Calculo el rol en ls organizacion actual

            if (dataWrapperUsuario.ListaOrganizacionRolUsuario.Count > 0)
            {
                foreach (AD.EntityModel.Models.UsuarioDS.OrganizacionRolUsuario filaOrganizacionRolUsuario in dataWrapperUsuario.ListaOrganizacionRolUsuario)
                {
                    if (!pIdentity.ListaRolesOrganizacion.ContainsKey(filaOrganizacionRolUsuario.OrganizacionID))
                    {
                        ulong rolPermitidoUsuarioOrganizacion = 0;
                        ulong rolDenegadoUsuarioOrganizacion = 0;

                        if (filaOrganizacionRolUsuario.RolPermitido != null)
                        {
                            rolPermitidoUsuarioOrganizacion = Convert.ToUInt64(filaOrganizacionRolUsuario.RolPermitido, 16);
                        }

                        if (filaOrganizacionRolUsuario.RolDenegado != null)
                        {
                            rolDenegadoUsuarioOrganizacion = Convert.ToUInt64(filaOrganizacionRolUsuario.RolDenegado, 16);
                        }

                        pIdentity.ListaRolesOrganizacion.Add(filaOrganizacionRolUsuario.OrganizacionID, rolPermitidoUsuarioOrganizacion & ~(rolDenegadoUsuarioOrganizacion));
                    }
                }
            }

            pIdentity.RolPermitidoProyecto = 0ul;
            try
            {
                ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);
                pIdentity.RolPermitidoProyecto = proyCL.CalcularRolFinalUsuarioEnProyecto(pIdentity.UsuarioID, pIdentity.Login, pIdentity.OrganizacionID, pIdentity.ProyectoID, dataWrapperUsuario);
                AD.EntityModel.Models.UsuarioDS.ProyectoRolUsuario filaProyectoRolUsuario = dataWrapperUsuario.ListaProyectoRolUsuario.FirstOrDefault(proyRolUs => proyRolUs.OrganizacionGnossID.Equals(pIdentity.OrganizacionID) && proyRolUs.ProyectoID.Equals(pIdentity.ProyectoID) && proyRolUs.UsuarioID.Equals(pIdentity.UsuarioID));

                if (filaProyectoRolUsuario != null)
                {
                    pIdentity.EstaBloqueadoEnProyecto = filaProyectoRolUsuario.EstaBloqueado;
                }

                proyCL.Dispose();
            }
            catch (Exception)
            {
                pIdentity.RolPermitidoProyecto = ulong.Parse("0000000000000000");
            }

        }

        /// <summary>
        /// Obtiene la fila de un proyecto
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización del proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Fila de proyecto</returns>
        public AD.EntityModel.Models.ProyectoDS.Proyecto ObtenerFilaProyecto(Guid pOrganizacionID, Guid pProyectoID)
        {
            DataWrapperProyecto dataWrapperProyecto = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory).ObtenerProyectoOrganizacion(pOrganizacionID, pProyectoID);
            return dataWrapperProyecto.ListaProyecto.FirstOrDefault(proyecto => proyecto.ProyectoID.Equals(pProyectoID));
        }

        /// <summary>
        /// Obtiene la cookie del usuario actual
        /// </summary>
        public Dictionary<string, string> CookieRewrite
        {
            get
            {
                Dictionary<string, string> value = new Dictionary<string, string>();
                try
                {
                    value = UtilCookies.FromLegacyCookieString(mHttpContextAccessor.HttpContext.Request.Cookies["rewrite" + DominoAplicacion], mEntityContext);
                }
                catch { }

                return value;
            }
        }

        public string ObtenerCookieSesion()
        {
            return mHttpContextAccessor.HttpContext.Request.Cookies["ASP.NET_SessionId"];
            //Dictionary<string, string> cookieEliminar = null;

            //if (requestCookie != null && mHttpContextAccessor.HttpContext.Request.Cookies.Keys.Count(key => key.Equals("ASP.NET_SessionId")) > 1)
            //{
            //    // Hay más de una cookie de sesión, busco la más larga
            //    // La busco de atrás hacia adelante porque seguramente está de las últimas
            //    for (int i = mHttpContextAccessor.HttpContext.Request.Cookies.Count - 1; i >= 0; i--)
            //    {
            //        if (mHttpContextAccessor.HttpContext.Request.Cookies[i].Name.Equals("ASP.NET_SessionId"))
            //        {
            //            cookieEliminar = requestCookie;
            //            requestCookie = mHttpContextAccessor.HttpContext.Request.Cookies[i];
            //            break;
            //        }
            //    }
            //}

            //if (cookieEliminar != null)
            //{
            //    mHttpContextAccessor.HttpContext.Request.Cookies.Remove(cookieEliminar.Name);

            //    if (mHttpContextAccessor.HttpContext.Request.Cookies["ASP.NET_SessionId"] == null)
            //    {
            //        mHttpContextAccessor.HttpContext.Request.Cookies.Add(requestCookie);
            //    }
            //}

            //return requestCookie;
        }

        /// <summary>
        /// Carga el gestor de identidades del usuario actual
        /// </summary>
        /// <returns>Gestor de identidades</returns>
        public GestionIdentidades CargarGestorIdentidadesUsuarioActual(UtilIdiomas pUtilIdiomas)
        {
            GestionIdentidades gestorIdentidades = null;

            if (UsuarioActual.EsUsuarioInvitado)
            {
                gestorIdentidades = ObtenerIdentidadUsuarioInvitado(pUtilIdiomas);
            }
            else
            {
                IdentidadCL identidadCL = new IdentidadCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCL>(), mLoggerFactory);
                gestorIdentidades = identidadCL.ObtenerCacheGestorIdentidadActual(UsuarioActual.PersonaID, UsuarioActual.PerfilID, UsuarioActual.OrganizacionID);
            }

            return gestorIdentidades;
        }

        /// <summary>
        /// Devuelve si la organización es de gnossOrganiza
        /// </summary>
        public bool EsGnossOrganiza
        {
            get
            {
                return OrganizacionGnoss != ProyectoAD.MyGnoss;
            }
        }

        /// <summary>
        /// Obtiene la identidad actual del usuario para la url-semantica, vacio si estas en una comunidad y "/" si estas en modo personal
        /// </summary>
        public string UrlPerfil
        {
            get
            {
                string urlPerfil = "/";

                try
                {
                    if (IdentidadActual != null && (IdentidadActual.TrabajaConOrganizacion || IdentidadActual.EsIdentidadProfesor))
                    {
                        string nombreCorto = IdentidadActual.PerfilUsuario.NombreCortoOrg;

                        if (IdentidadActual.EsIdentidadProfesor)
                        {
                            nombreCorto = IdentidadActual.PerfilUsuario.NombreCortoUsu;
                        }

                        urlPerfil += UtilIdiomas.GetText("URLSEM", "IDENTIDAD") + "/" + nombreCorto + "/";
                    }
                }
                catch (Exception ex)
                {
                    mLoggingService.GuardarLogError(ex, " ERROR: " + ex.Message + "\r\nStackTrace: " + ex.StackTrace, mlogger);
                }
                return urlPerfil;
            }
        }

        /// <summary>
        /// Obtiene la url de redirección a login con la url a la que se le quiere llevar 
        /// al usuario una vez logueado
        /// </summary>
        /// <param name="pUrl">URL a la que se le quiere llevar al usuario una vez logueado</param>
        /// <returns></returns>
        public string ObtenerUrlRedireccionLogin(string pUrl)
        {
            string redirect = "";
            if (!string.IsNullOrEmpty(pUrl))
            {
                redirect = "?redirect=" + HttpUtility.UrlEncode(pUrl);
            }

            return UrlServicioLogin + "/login" + redirect;
        }

        /// <summary>
        /// Obtiene el identificador de la Organización GNOSS
        /// </summary>
        public Guid OrganizacionGnoss
        {
            get
            {
                Guid? organizacionID = mConfigService.ObtenerOrganizacionGnoss();

                if (!organizacionID.HasValue)
                {
                    return ProyectoAD.MyGnoss;
                }
                return organizacionID.Value;
            }
        }

        /// <summary>
        /// Devuelve la URL del servicio de documentación.
        /// </summary>
        public string UrlServicioWebDocumentacion
        {
            get
            {
                return mConfigService.ObtenerUrlServicioDocumental();
            }
        }

        public bool TieneGoogleDriveConfigurado
        {
            get
            {
                //EntityContext context = EntityContext.Instance;
                // List<AD.EntityModel.ParametroAplicacion> busqueda = context.ParametroAplicacion.Where(parametro => parametro.Parametro.Equals(TiposParametrosAplicacion.GoogleDrive)).ToList();
                AD.EntityModel.ParametroAplicacion busqueda = ListaParametrosAplicacion.FirstOrDefault(parametro => parametro.Parametro.Equals(TiposParametrosAplicacion.GoogleDrive));
                if (ParametroProyecto != null)
                {
                    mTieneGoogleDriveConfigurado = ParametroProyecto.ContainsKey(TiposParametrosAplicacion.GoogleDrive) && (ParametroProyecto[TiposParametrosAplicacion.GoogleDrive].Equals("1") || ParametroProyecto[TiposParametrosAplicacion.GoogleDrive].ToLower().Equals("true"));
                }

                //if (!mTieneGoogleDriveConfigurado && ParametrosAplicacionDS.ParametroAplicacion.FindByParametro(TiposParametrosAplicacion.GoogleDrive) != null)
                if (!mTieneGoogleDriveConfigurado && busqueda != null)
                {
                    //mTieneGoogleDriveConfigurado = ParametrosAplicacionDS.ParametroAplicacion.Select("Parametro = '" + TiposParametrosAplicacion.GoogleDrive + "'").Length > 0 && (ParametrosAplicacionDS.ParametroAplicacion.Select("Parametro = '" + TiposParametrosAplicacion.GoogleDrive + "'")[0].Equals("1") || ParametrosAplicacionDS.ParametroAplicacion.Select("Parametro = '" + TiposParametrosAplicacion.GoogleDrive + "'")[0]["Valor"].ToString().ToLower().Equals("true"));
                    mTieneGoogleDriveConfigurado = busqueda.Valor.Equals("1") || busqueda.Valor.ToLower().Equals("true");
                }

                return mTieneGoogleDriveConfigurado;
            }
        }

        /// <summary>
        /// Obtiene atraves de un guid y el ParametroGeneralDS la fila de parametros generales del proyecto correspondiente
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto a buscar</param>
        /// <param name="pPage">Página</param>
        /// <returns>Fila del parametro general</returns>
        public List<ProyectoMetaRobots> ObtenerFilasProyectoMetaRobots(Guid pProyectoID)
        {
            List<ProyectoMetaRobots> filasProyectoMetaRobots = null;
            //ParametroGeneralDS paramDS = ObtenerParametrosGeneralesDeProyecto(pProyectoID);
            GestorParametroGeneral paramDS = ObtenerParametrosGeneralesDeProyecto(pProyectoID);

            if (paramDS != null && paramDS.ListaProyectoMetaRobots.Count > 0)
            {
                //filasProyectoMetaRobots = (ParametroGeneralDS.ProyectoMetaRobotsRow[])paramDS.ProyectoMetaRobots.Select("ProyectoID = '" + pProyectoID + "'");
                filasProyectoMetaRobots = paramDS.ListaProyectoMetaRobots.Where(proyectoMetaRobots => proyectoMetaRobots.ProyectoID.Equals(pProyectoID)).ToList();
            }
            return filasProyectoMetaRobots;
        }

        /// <summary>
        /// HTML personalizado para el Login.
        /// </summary>
        public string ProyectoLoginConfiguracion
        {
            get
            {
                if (string.IsNullOrEmpty(mProyectoLoginConfiguracion))
                {
                    //ProyectoCN proyCN = new ProyectoCN();
                    DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();
                    ProyectoGBD.ProyectoGBD proyectoGBD = new ProyectoGBD.ProyectoGBD(mEntityContext);
                    dataWrapperProyecto.ListaProyectoLoginConfiguracion = proyectoGBD.ObtenerProyectoLoginConfiguracion(ProyectoSeleccionado.Clave);
                    //ProyectoDS proyDS = proyCN.ObtenerProyectoLoginConfiguracion(ProyectoSeleccionado.Clave);
                    //proyCN.Dispose();

                    if (dataWrapperProyecto.ListaProyectoLoginConfiguracion.Count > 0)
                    {
                        mProyectoLoginConfiguracion = dataWrapperProyecto.ListaProyectoLoginConfiguracion[0].Mensaje;
                    }
                }

                return mProyectoLoginConfiguracion;
            }
        }

        public static DateTime DateTimeRemoveMilliseconds(DateTime pDate)
        {
            return new DateTime(pDate.Year, pDate.Month, pDate.Day, pDate.Hour, pDate.Minute, pDate.Second, 0, pDate.Kind);
        }

        /// <summary>
        /// Obtiene los parámetros extra para una búsqueda con proyecto origen.
        /// </summary>
        /// <returns></returns>
        public string ObtenerParametrosExtraBusquedaConProyOrigen()
        {
            string parametrosAdic = "";

            if (ProyectoSeleccionado.GestorProyectos.DataWrapperProyectos.ListaProyectoPestanyaBusqueda.Count == 1)
            {
                parametrosAdic = ProyectoSeleccionado.GestorProyectos.DataWrapperProyectos.ListaProyectoPestanyaBusqueda[0].CampoFiltro;
            }
            else if (ProyectoSeleccionado.GestorProyectos.DataWrapperProyectos.ListaProyectoPestanyaBusqueda.Count > 1)
            {
                List<string> filtro1 = new List<string>(ProyectoSeleccionado.GestorProyectos.DataWrapperProyectos.ListaProyectoPestanyaBusqueda[0].CampoFiltro.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries));
                List<string> filtroFinales = new List<string>(filtro1);

                foreach (ProyectoPestanyaBusqueda filaPest in ProyectoSeleccionado.GestorProyectos.DataWrapperProyectos.ListaProyectoPestanyaBusqueda)
                {
                    if (filaPest != ProyectoSeleccionado.GestorProyectos.DataWrapperProyectos.ListaProyectoPestanyaBusqueda[0])
                    {
                        string filtrosPes = filaPest.CampoFiltro + "|";
                        foreach (string filtro in filtro1)
                        {
                            if (!filtrosPes.Contains(filtro + "|"))
                            {
                                filtroFinales.Remove(filtro);
                            }
                        }
                    }
                }

                if (filtroFinales.Count > 0)
                {
                    foreach (string filtro in filtroFinales)
                    {
                        parametrosAdic += filtro + "|";
                    }

                    parametrosAdic = parametrosAdic.Substring(0, parametrosAdic.Length - 1);
                }
            }

            return parametrosAdic;
        }

        /// <summary>
        /// Carga todo lo necesario para que se conecte el usuario invitado y agrega a la sesión el usuario invitado
        /// </summary>
        public void CrearUsuarioInvitado()
        {
            //Creo el usuario actual como invitado
            UsuarioActual = CrearUsuarioInvitado(UtilIdiomas);

            //Lo agrego a la sesión
            mHttpContextAccessor.HttpContext.Session.Set("Usuario", UsuarioActual);
            mHttpContextAccessor.HttpContext.Session.Set("MantenerConectado", false);
        }

        /// <summary>
        /// Conecta a un usuario invitado que no existe en el sistema
        /// </summary>
        /// <param name="pUtilIdiomas">Util idiomas</param>
        public GnossIdentity CrearUsuarioInvitado(UtilIdiomas pUtilIdiomas)
        {
            //Crear la identidad
            GnossIdentity identity = new GnossIdentity();

            //Establezco los valores del invitado
            identity.UsuarioID = UsuarioAD.Invitado;
            identity.PersonaID = UsuarioAD.Invitado;
            identity.IdentidadID = UsuarioAD.Invitado;

            identity.EstaPasswordAutenticada = true;
            identity.EsUsuarioInvitado = true;
            identity.EsIdentidadInvitada = true;
            identity.Login = pUtilIdiomas.GetText("COMMON", "INVITADO");

            //Le conecto al metaproyecto
            identity.OrganizacionID = ProyectoAD.MetaOrganizacion;
            identity.ProyectoID = ProyectoAD.MetaProyecto;
            identity.Idioma = IdiomaUsuario;
            AgregarObjetoAPeticionActual("GnossIdentity", identity);


            //Establecemos los roles permitidos del usuario
            identity.RolPermitidoGeneral = (ulong)(0ul);
            identity.RolPermitidoProyecto = (ulong)(0ul);
            return identity;
        }

        private static bool? mShowErrors = null;
        public static bool ShowErrors
        {
            get
            {
                if (mShowErrors == null)
                {
                    mShowErrors = false;
                    if (false /*TODO Javier Conexion.ObtenerParametro("config/gnoss.config", "config/ShowErrors", false) == "1"*/)
                    {
                        mShowErrors = true;
                    }
                }
                return (bool)mShowErrors;
            }
        }

        /// <summary>
        /// Enumeración para distinguir tipos de acceso
        /// </summary>
        public enum TiposPagina
        {
            /// <summary>
            /// Home
            /// </summary>
            Home,
            /// <summary>
            /// Indice
            /// </summary>
            Indice,
            /// <summary>
            /// Personas y organizaciones
            /// </summary>
            PersonasYOrganizaciones,
            /// <summary>
            /// Base de Recursos
            /// </summary>
            BaseRecursos,
            /// <summary>
            /// Contribuciones
            /// </summary>
            Contribuciones,
            /// <summary>
            /// Preguntas
            /// </summary>
            Preguntas,
            /// <summary>
            /// Encuestas
            /// </summary>
            Encuestas,
            /// <summary>
            /// Debates
            /// </summary>
            Debates,
            /// <summary>
            /// Dafos
            /// </summary>
            Dafos,
            /// <summary>
            /// Blogs
            /// </summary>
            Blogs,
            /// <summary>
            /// Blogs
            /// </summary>
            Comunidades,
            /// <summary>
            /// Administracion
            /// </summary>
            Administacion,
            /// <summary>
            /// Enviar enlace.
            /// </summary>
            EnviarEnlace,
            /// <summary>
            /// Busqueda avanzada, de una comunidad o de MyGnoss
            /// </summary>
            BusquedaAvanzada,
            /// <summary>
            /// Presentacion de una comunidad.
            /// </summary>
            Presentacion,
            /// <summary>
            /// Documentos semanticos
            /// </summary>
            Semanticos,
            /// <summary>
            /// Acerca de la comunidad
            /// </summary>
            AcercaDe,
            /// <summary>
            /// Configurar Widget
            /// </summary>
            ConfigWidget,
            /// <summary>
            /// Configurar Widget de Busqueda
            /// </summary>
            ConfigWidgetBusqueda,
            /// <summary>
            /// Revisar RSS
            /// </summary>
            RSS,
            /// <summary>
            /// Contactos
            /// </summary>
            Contactos,
            /// <summary>
            /// Registro
            /// </summary>
            Registro,
            /// <summary>
            /// CMSEdicionPaginas
            /// </summary>
            CMSEdicionPaginas,
            /// <summary>
            /// CMSListadoPaginas
            /// </summary>
            CMSListadoPaginas,
            /// <summary>
            /// CMSListadoComponentes
            /// </summary>
            CMSListadoComponentes,
            /// <summary>
            /// CMSMultimedia
            /// </summary>
            CMSMultimedia,
            /// <summary>
            /// Borradores
            /// </summary>
            Borradores,
            /// <summary>
            /// Dashboard
            /// </summary>
            Dashboard
        }

        /// <summary>
        /// Obtiene el proyecto seleccionado
        /// </summary>
        public string UrlSearchProyecto
        {
            get
            {
                return mUrlSearchProyecto;
            }
            set
            {
                mUrlSearchProyecto = value;
            }
        }

        /// <summary>
        /// Devueve o establece el tipo de pagina en la que estamos
        /// </summary>
        public TiposPagina mTipoPagina;

        /// <summary>
        /// Devueve o establece el tipo de pagina en la que estamos
        /// </summary>
        public TiposPagina TipoPagina
        {
            get
            {
                return mTipoPagina;
            }
            set
            {
                mTipoPagina = value;
            }
        }

        /// <summary>
        /// Obtiene el origen del autocompletar según el tipo de página.
        /// </summary>
        public Guid OrigenAutoCompletarPagina
        {
            get
            {
                Guid origenAutoCompletar = Guid.Empty;

                //if (TipoPagina == TiposPagina.BaseRecursos)
                //{
                //    origenAutoCompletar = OrigenAutocompletar.Recursos;
                //}
                //else if (TipoPagina == TiposPagina.Debates)
                //{
                //    origenAutoCompletar = OrigenAutocompletar.Debates;
                //}
                //else if (TipoPagina == TiposPagina.Encuestas)
                //{
                //    origenAutoCompletar = OrigenAutocompletar.Encuestas;
                //}
                //else if (TipoPagina == TiposPagina.Preguntas)
                //{
                //    origenAutoCompletar = OrigenAutocompletar.Preguntas;
                //}
                //else if (TipoPagina == TiposPagina.PersonasYOrganizaciones)
                //{
                //    origenAutoCompletar = OrigenAutocompletar.PersyOrg;
                //}
                //else if (TipoPagina == TiposPagina.Semanticos)
                //{
                //    origenAutoCompletar = ProyectoPestanyaActual.FilaProyectoPestanyaMenu.Ruta;
                //}
                if (ProyectoPestanyaActual != null)
                {
                    origenAutoCompletar = ProyectoPestanyaActual.FilaProyectoPestanyaMenu.PestanyaID;
                }

                return origenAutoCompletar;
            }
        }

        /// <summary>
        /// Obtiene el proyecto seleccionado
        /// </summary>
        public Elementos.ServiciosGenerales.ProyectoPestanyaMenu ProyectoPestanyaActual
        {
            get
            {
                if (mProyectoPestanyaActual == null)
                {//FindByPestanyaID(Guid.Empty)
                    if (ProyectoSeleccionado.GestorProyectos.DataWrapperProyectos.ListaProyectoPestanyaMenu.Find(proyPestanyaMenu => proyPestanyaMenu.PestanyaID.Equals(Guid.Empty)) != null)
                    {
                        ////////TODO ?  Eliminar de Contexto?
                        ProyectoSeleccionado.GestorProyectos.DataWrapperProyectos.ListaProyectoPestanyaMenu.Remove(ProyectoSeleccionado.GestorProyectos.DataWrapperProyectos.ListaProyectoPestanyaMenu.Find(proyPestanyaMenu => proyPestanyaMenu.PestanyaID.Equals(Guid.Empty)));
                    }

                    if (RequestParams("PestanyaID") != null)
                    {
                        mProyectoPestanyaActual = ProyectoSeleccionado.ListaPestanyasMenu[new Guid(RequestParams("PestanyaID"))];
                    }
                    else if (RequestParams("PestanyaRuta") != null)
                    {
                        string pestanyaRuta = $"{RequestParams("PestanyaRuta")}@";
                        Guid pestanyaID = ProyectoSeleccionado.GestorProyectos.DataWrapperProyectos.ListaProyectoPestanyaMenu.Find(proyPestanyaMenu => proyPestanyaMenu.Ruta != null && proyPestanyaMenu.Ruta.Contains(pestanyaRuta)).PestanyaID;
                        mProyectoPestanyaActual = ProyectoSeleccionado.ListaPestanyasMenu[pestanyaID];
                    }
                    else if (RequestParams("Controller") != null)
                    {
                        string controller = RequestParams("Controller");
                        switch (controller)
                        {
                            case "HomeComunidad":
                                foreach (Elementos.ServiciosGenerales.ProyectoPestanyaMenu pestanya in ProyectoSeleccionado.ListaPestanyasMenu.Values)
                                {
                                    if (pestanya.TipoPestanya == TipoPestanyaMenu.Home)
                                    {
                                        mProyectoPestanyaActual = pestanya;
                                        break;
                                    }
                                }
                                break;
                            case "Indice":
                                foreach (Elementos.ServiciosGenerales.ProyectoPestanyaMenu pestanya in ProyectoSeleccionado.ListaPestanyasMenu.Values)
                                {
                                    if (pestanya.TipoPestanya == TipoPestanyaMenu.Indice)
                                    {
                                        mProyectoPestanyaActual = pestanya;
                                        break;
                                    }
                                }
                                break;
                            case "Busqueda":
                                bool recursos = !string.IsNullOrEmpty(RequestParams("recursos")) && RequestParams("recursos") == "true";
                                bool preguntas = !string.IsNullOrEmpty(RequestParams("preguntas")) && RequestParams("preguntas") == "true";
                                bool debates = !string.IsNullOrEmpty(RequestParams("debates")) && RequestParams("debates") == "true";
                                bool encuestas = !string.IsNullOrEmpty(RequestParams("encuestas")) && RequestParams("encuestas") == "true";
                                bool perorg = !string.IsNullOrEmpty(RequestParams("perorg")) && RequestParams("perorg") == "true";
                                bool busquedaAvanzada = !string.IsNullOrEmpty(RequestParams("Meta")) && RequestParams("Meta") == "true";
                                bool comunidades = !string.IsNullOrEmpty(RequestParams("comunidades")) && RequestParams("comunidades") == "true";
                                bool contribuciones = !string.IsNullOrEmpty(RequestParams("contribuciones")) && RequestParams("contribuciones") == "true";
                                bool borradores = !string.IsNullOrEmpty(RequestParams("borradores")) && RequestParams("borradores") == "true";

                                if (recursos)
                                {
                                    mProyectoPestanyaActual = ObtenerPestanyaPorTipo(TipoPestanyaMenu.Recursos);
                                }
                                else if (preguntas)
                                {
                                    mProyectoPestanyaActual = ObtenerPestanyaPorTipo(TipoPestanyaMenu.Preguntas);
                                }
                                else if (debates)
                                {
                                    mProyectoPestanyaActual = ObtenerPestanyaPorTipo(TipoPestanyaMenu.Debates);
                                }
                                else if (encuestas)
                                {
                                    mProyectoPestanyaActual = ObtenerPestanyaPorTipo(TipoPestanyaMenu.Encuestas);
                                }
                                else if (perorg)
                                {
                                    mProyectoPestanyaActual = ObtenerPestanyaPorTipo(TipoPestanyaMenu.PersonasYOrganizaciones);
                                }
                                else if (busquedaAvanzada)
                                {
                                    mProyectoPestanyaActual = ObtenerPestanyaPorTipo(TipoPestanyaMenu.BusquedaAvanzada);
                                    if (mProyectoPestanyaActual == null)
                                    {
                                        //Busqueda-avanzada
                                        AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu proyectoPestanyaMenu = new AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu();
                                        proyectoPestanyaMenu.PestanyaID = Guid.Empty;
                                        proyectoPestanyaMenu.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
                                        proyectoPestanyaMenu.ProyectoID = ProyectoSeleccionado.FilaProyecto.ProyectoID;
                                        proyectoPestanyaMenu.ProyectoPestanyaMenu1 = null;
                                        proyectoPestanyaMenu.TipoPestanya = (short)TipoPestanyaMenu.BusquedaAvanzada;
                                        proyectoPestanyaMenu.Nombre = null;
                                        proyectoPestanyaMenu.Ruta = null;
                                        proyectoPestanyaMenu.Orden = 1005;
                                        proyectoPestanyaMenu.NuevaPestanya = false;
                                        proyectoPestanyaMenu.Visible = false;
                                        proyectoPestanyaMenu.Privacidad = (short)TipoPrivacidadPagina.Normal;
                                        proyectoPestanyaMenu.HtmlAlternativo = null;
                                        proyectoPestanyaMenu.IdiomasDisponibles = null;
                                        proyectoPestanyaMenu.Titulo = null;
                                        proyectoPestanyaMenu.NombreCortoPestanya = "";
                                        proyectoPestanyaMenu.VisibleSinAcceso = true;
                                        proyectoPestanyaMenu.CSSBodyClass = "";
                                        proyectoPestanyaMenu.MetaDescription = null;
                                        proyectoPestanyaMenu.Activa = true;
                                        //AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu filaPestanya = ProyectoSeleccionado.GestorProyectos.DataWrapperProyecto.ListaProyectoPestanyaMenu.AddProyectoPestanyaMenuRow(Guid.Empty, ProyectoSeleccionado.FilaProyecto.OrganizacionID, ProyectoSeleccionado.FilaProyecto.ProyectoID, null, (short)TipoPestanyaMenu.BusquedaAvanzada, null, null, 1005, false, false, (short)TipoPrivacidadPagina.Normal, null, null, null, "", true, "", null, true);

                                        mProyectoPestanyaActual = new Elementos.ServiciosGenerales.ProyectoPestanyaMenu(proyectoPestanyaMenu, ProyectoSeleccionado.GestorProyectos, mLoggingService, mEntityContext);
                                    }
                                }
                                else if (comunidades)
                                {
                                    mProyectoPestanyaActual = ObtenerPestanyaPorTipo(TipoPestanyaMenu.Comunidades);
                                }
                                else if (contribuciones)
                                {
                                    //Contribuciones
                                    AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu proyectoPestanyaMenu = new AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu();
                                    proyectoPestanyaMenu.PestanyaID = Guid.Empty;
                                    proyectoPestanyaMenu.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
                                    proyectoPestanyaMenu.ProyectoID = ProyectoSeleccionado.FilaProyecto.ProyectoID;
                                    proyectoPestanyaMenu.ProyectoPestanyaMenu1 = null;
                                    proyectoPestanyaMenu.TipoPestanya = (short)TipoPestanyaMenu.Contribuciones;
                                    proyectoPestanyaMenu.Nombre = null;
                                    proyectoPestanyaMenu.Ruta = null;
                                    proyectoPestanyaMenu.Orden = 1010;
                                    proyectoPestanyaMenu.NuevaPestanya = false;
                                    proyectoPestanyaMenu.Visible = false;
                                    proyectoPestanyaMenu.Privacidad = (short)TipoPrivacidadPagina.Normal;
                                    proyectoPestanyaMenu.HtmlAlternativo = null;
                                    proyectoPestanyaMenu.IdiomasDisponibles = null;
                                    proyectoPestanyaMenu.Titulo = null;
                                    proyectoPestanyaMenu.NombreCortoPestanya = "";
                                    proyectoPestanyaMenu.VisibleSinAcceso = true;
                                    proyectoPestanyaMenu.CSSBodyClass = "";
                                    proyectoPestanyaMenu.MetaDescription = null;
                                    proyectoPestanyaMenu.Activa = true;
                                    //AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu filaPestanya = ProyectoSeleccionado.GestorProyectos.DataWrapperProyecto.ProyectoPestanyaMenu.AddProyectoPestanyaMenuRow(Guid.Empty, ProyectoSeleccionado.FilaProyecto.OrganizacionID, ProyectoSeleccionado.FilaProyecto.ProyectoID, null, (short)TipoPestanyaMenu.Contribuciones, null, null, 1010, false, false, (short)TipoPrivacidadPagina.Normal, null, null, null, "", true, "", null, true);

                                    mProyectoPestanyaActual = new Elementos.ServiciosGenerales.ProyectoPestanyaMenu(proyectoPestanyaMenu, ProyectoSeleccionado.GestorProyectos, mLoggingService, mEntityContext);
                                }
                                else if (borradores)
                                {
                                    //Borradores
                                    AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu proyectoPestanyaMenu = new AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu();
                                    proyectoPestanyaMenu.PestanyaID = Guid.Empty;
                                    proyectoPestanyaMenu.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
                                    proyectoPestanyaMenu.ProyectoID = ProyectoSeleccionado.FilaProyecto.ProyectoID;
                                    proyectoPestanyaMenu.ProyectoPestanyaMenu1 = null;
                                    proyectoPestanyaMenu.TipoPestanya = (short)TipoPestanyaMenu.Borradores;
                                    proyectoPestanyaMenu.Nombre = null;
                                    proyectoPestanyaMenu.Ruta = null;
                                    proyectoPestanyaMenu.Orden = 1010;
                                    proyectoPestanyaMenu.NuevaPestanya = false;
                                    proyectoPestanyaMenu.Visible = false;
                                    proyectoPestanyaMenu.Privacidad = (short)TipoPrivacidadPagina.Normal;
                                    proyectoPestanyaMenu.HtmlAlternativo = null;
                                    proyectoPestanyaMenu.IdiomasDisponibles = null;
                                    proyectoPestanyaMenu.Titulo = null;
                                    proyectoPestanyaMenu.NombreCortoPestanya = "";
                                    proyectoPestanyaMenu.VisibleSinAcceso = true;
                                    proyectoPestanyaMenu.CSSBodyClass = "";
                                    proyectoPestanyaMenu.MetaDescription = null;
                                    proyectoPestanyaMenu.Activa = true;
                                    //ProyectoDS.ProyectoPestanyaMenuRow filaPestanya = ProyectoSeleccionado.GestorProyectos.DataWrapperProyecto.ProyectoPestanyaMenu.AddProyectoPestanyaMenuRow(Guid.Empty, ProyectoSeleccionado.FilaProyecto.OrganizacionID, ProyectoSeleccionado.FilaProyecto.ProyectoID, null, (short)TipoPestanyaMenu.Borradores, null, null, 1010, false, false, (short)TipoPrivacidadPagina.Normal, null, null, null, "", true, "", null, true);

                                    mProyectoPestanyaActual = new Elementos.ServiciosGenerales.ProyectoPestanyaMenu(proyectoPestanyaMenu, ProyectoSeleccionado.GestorProyectos, mLoggingService, mEntityContext);
                                }

                                break;
                            case "AcercaDeComunidad":
                                foreach (Elementos.ServiciosGenerales.ProyectoPestanyaMenu pestanya in ProyectoSeleccionado.ListaPestanyasMenu.Values)
                                {
                                    if (pestanya.TipoPestanya == TipoPestanyaMenu.AcercaDe && string.IsNullOrEmpty(pestanya.Ruta))
                                    {
                                        mProyectoPestanyaActual = pestanya;
                                        break;
                                    }
                                }
                                break;
                            //TFG FRAN
                            case "Dashboard":
                                foreach (Elementos.ServiciosGenerales.ProyectoPestanyaMenu pestanya in ProyectoSeleccionado.ListaPestanyasMenu.Values)
                                {
                                    if (pestanya.TipoPestanya == TipoPestanyaMenu.Dashboard && string.IsNullOrEmpty(pestanya.Ruta))
                                    {
                                        mProyectoPestanyaActual = pestanya;
                                        break;
                                    }
                                }
                                break;
                        }
                    }
                }

                return mProyectoPestanyaActual;
            }
        }

        private Elementos.ServiciosGenerales.ProyectoPestanyaMenu ObtenerPestanyaPorTipo(TipoPestanyaMenu pTipoPestanya)
        {
            Elementos.ServiciosGenerales.ProyectoPestanyaMenu pestanya = ProyectoSeleccionado.ListaPestanyasMenu.Values.FirstOrDefault(pest => pest.TipoPestanya == pTipoPestanya && string.IsNullOrEmpty(pest.Ruta));

            if (pestanya == null)
            {
                pestanya = ProyectoSeleccionado.ListaPestanyasMenu.Values.FirstOrDefault(pest => pest.TipoPestanya == pTipoPestanya);
            }

            return pestanya;
        }

        public string UrlIntragnossServicios
        {
            get
            {
                if (mUrlIntragnossServicios == null)
                {
                    mUrlIntragnossServicios = mConfigService.ObtenerUrlServicioInterno();
                }
                return mUrlIntragnossServicios;
            }
        }

        /// <summary>
        /// Facetas para autocompletar la comunidad.
        /// </summary>
        public string FacetasProyAutoCompBuscadorCom
        {
            get
            {
                return mEntityContext.ConfigAutocompletarProy.FirstOrDefault(item => item.ProyectoID.Equals(ProyectoSeleccionado.Clave) && item.Clave.Equals(FacetasBuscadorCom))?.Valor;
            }
        }

        public string NombreProyectoEcosistema
        {
            get
            {
                if (string.IsNullOrEmpty(mNombreProyectoEcosistema))
                {
                    ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
                    mNombreProyectoEcosistema = proyCN.ObtenerNombreDeProyectoID(ProyectoAD.MetaProyecto);
                    proyCN.Dispose();
                }
                return mNombreProyectoEcosistema;
            }
        }

        private void EstablecerIdentidadActual(Guid pIdentidadID, Guid pPerfilID, GnossIdentity pUsuario)
        {
            bool cambiaDeIdentidad = pUsuario.IdentidadID != pIdentidadID;
            mLoggingService.AgregarEntrada("cambio de identidad: " + cambiaDeIdentidad);

            mLoggingService.AgregarEntrada("Inicio: Establecemos la identidad del usuario");

            pUsuario.IdentidadID = pIdentidadID;
            pUsuario.PerfilID = pPerfilID;
            pUsuario.EsIdentidadInvitada = pIdentidadID.Equals(UsuarioAD.Invitado);
            pUsuario.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
            pUsuario.ProyectoID = ProyectoSeleccionado.FilaProyecto.ProyectoID;

            if (pUsuario.PerfilID.Equals(Guid.Empty))
            {
                pUsuario.PerfilID = UsuarioAD.Invitado;
            }

            string urlActual = mHttpContextAccessor.HttpContext.Request.Host.ToString().ToLower();
            if ((cambiaDeIdentidad || pUsuario.EsIdentidadInvitada) && !ListaUrlsSinIdentidad.Any(url => urlActual.EndsWith(url)))
            {
                //Actualizo la identidad con la que se conecta ahora el usuario

                if (pUsuario.IdentidadID.Equals(UsuarioAD.Invitado) || pUsuario.EsUsuarioInvitado)
                {
                    //Si la identidad con la que se va a conectar es la de invitado, le creo una identidad virtual al usuario
                    pUsuario.EsIdentidadInvitada = true;
                    pUsuario.RolPermitidoProyecto = ulong.Parse("0000000000000000");
                }
                else
                {
                    pUsuario.EsIdentidadInvitada = false;
                    try
                    {
                        //Actualizo el número de conexiones en la BD
                        IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
                        identidadCN.ActualizarNumeroConexionesProyecto(pUsuario.IdentidadID);
                        //Invalido la cache de la identidad MVC
                        IdentidadCL identidadCL = new IdentidadCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCL>(), mLoggerFactory);
                        identidadCL.InvalidarFichaIdentidadMVC(pUsuario.IdentidadID);
                        identidadCL.Dispose();
                    }
                    catch (Exception ex)
                    {
                        mLoggingService.GuardarLogError(ex, " ERROR: " + ex.Message + "\r\nStackTrace: " + ex.StackTrace, mlogger);
                    }
                }

                mLoggingService.AgregarEntrada("Fin: Establecemos la identidad del usuario IdentidadID '" + pUsuario.IdentidadID + "' PerfilID '" + pUsuario.PerfilID + "'");

                AgregarObjetoAPeticionActual("GnossIdentity", pUsuario);
                mHttpContextAccessor.HttpContext.Session.Set("Usuario", pUsuario);

                if (!mHttpContextAccessor.HttpContext.Request.Path.Equals("/404.aspx"))
                {
                    ActualizarCookiePerfil(pUsuario.PerfilID, pUsuario.IdentidadID, pUsuario.UsuarioID);
                }
            }

            if ((cambiaDeIdentidad && !pUsuario.EsUsuarioInvitado) || (pUsuario.EsProfesor && ProyectoSeleccionado.Clave == ProyectoAD.MetaProyecto))
            {
                UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<UsuarioCN>(), mLoggerFactory);
                CalcularPermisosUsuario(usuarioCN.ObtenerUsuarioPorID(pUsuario.UsuarioID), pUsuario);
                usuarioCN.Dispose();
            }

            if (pUsuario != null && !pUsuario.EsUsuarioInvitado && IdentidadActual.OrganizacionID.HasValue && !pUsuario.EstaAutorizadoEnOrganizacion((ulong)Capacidad.Organizacion.CapacidadesAdministrador.AdministrarOrganizacion, IdentidadActual.OrganizacionID.Value))
            {
                // Recuperamos los permisos en las organizaciones de cache
                // Por si nos han dado permisos en la organización del perfil con el que estoy conectado

                IdentidadCL identidadCL = new IdentidadCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCL>(), mLoggerFactory);
                bool esAdministrador = identidadCL.ObtenerPermisosUsuarioEnOrg(pUsuario.UsuarioID, IdentidadActual.OrganizacionID.Value) == (short)TipoAdministradoresOrganizacion.Administrador;
                identidadCL.Dispose();

                if (esAdministrador)
                {
                    UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<UsuarioCN>(), mLoggerFactory);
                    CalcularPermisosUsuario(usuarioCN.ObtenerFilaUsuarioPorLoginOEmail(pUsuario.Login), pUsuario);
                    usuarioCN.Dispose();
                }
            }
        }

        /// <summary>
        /// Recupera la varieble global Usuario.UsuarioActual a partir de la sesión
        /// </summary>
        public void RecuperarUsuarioDeSesion(GnossIdentity pUsuario)
        {
            if (mHttpContextAccessor.HttpContext.Features.Get<ISessionFeature>() == null)
            {
                //Si no hay sesion configurada no se puede recuperar el usuario
                return;
            }

            if (!mHttpContextAccessor.HttpContext.Session.Keys.Contains("Usuario"))
            {
                CrearUsuarioInvitado(pUsuario);
            }
            else
            {


                GnossIdentity usuario = mHttpContextAccessor.HttpContext.Session.Get<GnossIdentity>("Usuario");
                if (usuario == null)
                {
                    return;
                }
                AgregarObjetoAPeticionActual("GnossIdentity", usuario);

                // Calculo IdentidadID

                Guid id = Guid.Empty;
                Guid perfilID = Guid.Empty;

                IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);

                if (ProyectoSeleccionado == null)
                {
                    return;
                }

                if (ProyectoSeleccionado.Clave == ProyectoAD.MetaProyecto)
                {
                    Guid[] identidadEnMygnoss = ObtenerIdentidadUsuarioEnMygnoss(pUsuario);
                    if (identidadEnMygnoss == null)
                    {
                        throw new Exception("Identidad not found");
                    }
                    perfilID = identidadEnMygnoss[0];
                    id = identidadEnMygnoss[1];
                }
                else
                {
                    Guid[] identPerfil = null;

                    if (pUsuario.PersonaID != UsuarioAD.Invitado)
                    {
                        string sesionIdentidadID = string.Format("IdentidadIDdePersonaEnProyecto_{0}_{1}", ProyectoSeleccionado.Clave, pUsuario.PersonaID);
                        if (mHttpContextAccessor.HttpContext.Session.Keys.Contains(sesionIdentidadID) && mHttpContextAccessor.HttpContext.Session.Get<Guid[]>(sesionIdentidadID) != null)
                        {
                            identPerfil = mHttpContextAccessor.HttpContext.Session.Get<Guid[]>(sesionIdentidadID);
                        }
                        else
                        {
                            identPerfil = identidadCN.ObtenerIdentidadIDDePersonaEnProyecto(ProyectoSeleccionado.Clave, pUsuario.PersonaID);
                            mHttpContextAccessor.HttpContext.Session.Set(sesionIdentidadID, identPerfil);
                        }
                    }

                    if (identPerfil != null && identPerfil.Length > 0)
                    {
                        id = identPerfil[0];
                        perfilID = identPerfil[1];
                    }

                    if (id.Equals(Guid.Empty))
                    {
                        //Le conecto como invitado
                        id = UsuarioAD.Invitado;

                        if (pUsuario.PersonaID.Equals(UsuarioAD.Invitado))
                        {
                            perfilID = UsuarioAD.Invitado;
                        }
                    }
                }

                if (id != Guid.Empty)
                {
                    EstablecerIdentidadActual(id, perfilID, usuario);
                    mHttpContextAccessor.HttpContext.Session.Set("Usuario", usuario);
                }

                identidadCN.Dispose();

                if (pUsuario == null)
                {
                    //Creo el usuario actual como invitado
                    CrearUsuarioInvitado(pUsuario);
                }
                else if (!pUsuario.EsUsuarioInvitado)
                {
                    AD.EntityModel.ParametroAplicacion busqueda = ListaParametrosAplicacion.FirstOrDefault(parametro => parametro.Parametro == TiposParametrosAplicacion.LoginUnicoPorUsuario);
                    //bool loginUnicoPorUsuario = busqueda!=null && busqueda.Valor.Equals("1");
                    bool loginUnicoPorUsuario = ListaParametrosAplicacion.Where(parametro => parametro.Parametro == TiposParametrosAplicacion.LoginUnicoPorUsuario).FirstOrDefault() != null && ListaParametrosAplicacion.Where(parametro => parametro.Parametro == TiposParametrosAplicacion.LoginUnicoPorUsuario).FirstOrDefault().Valor.Equals("1");
                    //Crear otro parametro para permitir la desconexión.
                    bool desconexionUsuarios = ListaParametrosAplicacion.Where(parametro => parametro.Parametro == TiposParametrosAplicacion.DesconexionUsuarios).FirstOrDefault() != null && ListaParametrosAplicacion.Where(parametro => parametro.Parametro == TiposParametrosAplicacion.DesconexionUsuarios).FirstOrDefault().Valor.Equals("1");
                    //Devolver a desconectar con un parametro que diga que se ha desconectado.

                    if (loginUnicoPorUsuario && desconexionUsuarios)
                    {
                        List<string> listaSesiones = mGnossCache.ObtenerDeCache($"{SESION_UNICA_POR_USUARIO}_List_{pUsuario.UsuarioID}") as List<string>;
                        if (listaSesiones == null)
                        {
                            listaSesiones = new List<string>();
                        }
                        if (listaSesiones.Contains(mHttpContextAccessor.HttpContext.Session.Id))
                        {
                            mGnossCache.InvalidarDeCache($"{SESION_UNICA_POR_USUARIO}_List_{pUsuario.UsuarioID}", true);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Agrega un objeto a la petición actual. Dicho objeto sílo estará disponible para ésta petición
        /// </summary>
        /// <param name="pClave">Clave del objeto</param>
        /// <param name="pObjeto">Objeto a almacenar</param>
        public void AgregarObjetoAPeticionActual(string pClave, object pObjeto)
        {
            try
            {
                if (mHttpContextAccessor.HttpContext != null && mHttpContextAccessor.HttpContext.Items != null)
                {
                    if (mHttpContextAccessor.HttpContext.Items.Keys.Contains(pClave))
                    {
                        mHttpContextAccessor.HttpContext.Items[pClave] = pObjeto;
                    }
                    else
                    {
                        mHttpContextAccessor.HttpContext.Items.Add(pClave, pObjeto);
                    }
                }
                else
                {
                    Dictionary<string, object> listaItems = ObtenerListaItemsDeThread(Thread.CurrentThread.ManagedThreadId);
                    if (listaItems.ContainsKey(pClave))
                    {
                        listaItems[pClave] = pObjeto;
                    }
                    else
                    {
                        listaItems.Add(pClave, pObjeto);
                    }
                }
            }
            catch { }
        }

        public Guid BaseRecursosProyectoSeleccionado
        {
            get
            {
                if (!mBaseRecursosProyectoSeleccionado.HasValue)
                {
                    if (ProyectoSeleccionado != null)
                    {
                        DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
                        mBaseRecursosProyectoSeleccionado = docCN.ObtenerBaseRecursosIDProyecto(ProyectoSeleccionado.Clave);
                    }
                    else
                    {
                        mBaseRecursosProyectoSeleccionado = Guid.Empty;
                    }
                }
                return mBaseRecursosProyectoSeleccionado.Value;
            }
        }

        /// <summary>
        /// Parámetros de un proyecto.
        /// </summary>
        public DataWrapperExportacionBusqueda ExportacionBusquedaDW
        {
            get
            {
                if (mExportacionBusquedaDW == null)
                {
                    ExportacionBusquedaCL exportacionCL = new ExportacionBusquedaCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ExportacionBusquedaCL>(), mLoggerFactory);
                    mExportacionBusquedaDW = exportacionCL.ObtenerExportacionesProyecto(ProyectoSeleccionado.Clave);
                    exportacionCL.Dispose();
                }

                return mExportacionBusquedaDW;
            }
        }

        /// <summary>
        /// Obtiene la lista de Items de un thread
        /// </summary>
        /// <param name="pThreadID">Identificador del Thread</param>
        /// <returns>Lista de Items del thread actual</returns>
        private static Dictionary<string, object> ObtenerListaItemsDeThread(int pThreadID)
        {
            if (!mListaItemsPorThread.ContainsKey(pThreadID))
            {
                mListaItemsPorThread.TryAdd(pThreadID, new Dictionary<string, object>());
            }
            return mListaItemsPorThread[pThreadID];
        }

        /// <summary>
        /// Carga todo lo necesario para que se conecte el usuario invitado y agrega a la sesión el usuario invitado
        /// </summary>
        public void CrearUsuarioInvitado(GnossIdentity pUsuario)
        {
            //Crear la identidad
            GnossIdentity identity = new GnossIdentity();

            //Establezco los valores del invitado
            identity.UsuarioID = UsuarioAD.Invitado;
            identity.PersonaID = UsuarioAD.Invitado;
            identity.IdentidadID = UsuarioAD.Invitado;

            identity.EstaPasswordAutenticada = true;
            identity.EsUsuarioInvitado = true;
            identity.EsIdentidadInvitada = true;
            identity.Login = UtilIdiomas.GetText("COMMON", "INVITADO");

            //Le conecto al metaproyecto
            identity.OrganizacionID = ProyectoAD.MetaOrganizacion;
            identity.ProyectoID = ProyectoAD.MetaProyecto;

            //AsignarIdentityAHiloActual(identity);
            AgregarObjetoAPeticionActual("GnossIdentity", identity);

            //Establecemos los roles permitidos del usuario
            identity.RolPermitidoGeneral = 0ul;
            identity.RolPermitidoProyecto = 0ul;

            //Lo agrego a la sesión
            mHttpContextAccessor.HttpContext.Session.Set("Usuario", pUsuario);
            mHttpContextAccessor.HttpContext.Session.Set("MantenerConectado", false);
        }

        Guid PerfilCookieID { get; set; }

        /// <summary>
        /// Obtiene los identificadores de la identidad y perfil actual del usuario (PerfilID, IdentidadID)
        /// </summary>
        public Guid[] IdentidadIDActual(GnossIdentity pUsuario)
        {
            if (mIdentidadIDActual == null)
            {
                Guid usuarioID = UsuarioAD.Invitado;
                Guid personaID = UsuarioAD.Invitado;

                if (pUsuario != null)
                {
                    usuarioID = pUsuario.UsuarioID;
                    personaID = pUsuario.PersonaID;
                }

                IdentidadCL identCL = new IdentidadCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCL>(), mLoggerFactory);
                mIdentidadIDActual = identCL.ObtenerIdentidadActualUsuario(usuarioID, personaID);
            }

            return mIdentidadIDActual;
        }

        /// <summary>
        /// Crea una identidad y una persona virtual para el usuario
        /// </summary>
        /// <param name="pUtilIdiomas">Util idiomas</param>
        /// <returns>Gestor de identidades</returns>
        public GestionIdentidades ObtenerIdentidadUsuarioInvitado(UtilIdiomas pUtilIdiomas)
        {
            PersonaCN personaCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<PersonaCN>(), mLoggerFactory);
            //Creo el gestor de personas
            GestionPersonas gestorPersonas = new GestionPersonas(personaCN.ObtenerPersonaInvitado(), mLoggingService, mEntityContext);
            personaCN.Dispose();

            gestorPersonas.ListaPersonas[UsuarioAD.Invitado].FilaPersona.Idioma = pUtilIdiomas.LanguageCode;
            gestorPersonas.ListaPersonas[UsuarioAD.Invitado].FilaPersona.Nombre = pUtilIdiomas.GetText("COMMON", "INVITADO");

            //Creo el gestor de identidades
            IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
            GestionIdentidades gestorIdent = new GestionIdentidades(identCN.ObtenerIdentidadInvitado(), gestorPersonas, new GestionOrganizaciones(new DataWrapperOrganizacion(), mLoggingService, mEntityContext), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
            identCN.Dispose();

            gestorIdent.ListaPerfiles[UsuarioAD.Invitado].FilaPerfil.NombreCortoUsu = pUtilIdiomas.GetText("COMMON", "INVITADO");
            gestorIdent.ListaPerfiles[UsuarioAD.Invitado].FilaPerfil.NombrePerfil = pUtilIdiomas.GetText("COMMON", "INVITADO");

            return gestorIdent;
        }


        ///// <summary>
        ///// Carga el gestor de identidades del usuario actual
        ///// </summary>
        ///// <returns>Gestor de identidades</returns>
        //public GestionIdentidades CargarGestorIdentidadesUsuarioActual(UtilIdiomas pUtilIdiomas, GnossIdentity pUsuario)
        //{
        //    GestionIdentidades gestorIdentidades = null;

        //    if (pUsuario.EsUsuarioInvitado)
        //    {
        //        gestorIdentidades = ObtenerIdentidadUsuarioInvitado(pUtilIdiomas);
        //    }
        //    else
        //    {
        //        IdentidadCL identidadCL = new IdentidadCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
        //        gestorIdentidades = identidadCL.ObtenerCacheGestorIdentidadActual(pUsuario.PersonaID, pUsuario.PerfilID, pUsuario.OrganizacionID);
        //    }

        //    return gestorIdentidades;
        //}

        /// <summary>
        /// Carga el gestor de identidades del usuario actual
        /// </summary>
        /// <returns>Gestor de identidades</returns>
        public GestionIdentidades CargarGestorIdentidadesUsuarioActual(UtilIdiomas pUtilIdiomas, GnossIdentity pUsuario)
        {
            GestionIdentidades gestorIdentidades = null;

            if (pUsuario.EsUsuarioInvitado)
            {
                gestorIdentidades = ObtenerIdentidadUsuarioInvitado(pUtilIdiomas);
            }
            else
            {
                IdentidadCL identidadCL = new IdentidadCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCL>(), mLoggerFactory);
                gestorIdentidades = identidadCL.ObtenerCacheGestorIdentidadActual(pUsuario.IdentidadID, pUsuario.PersonaID, pUsuario.OrganizacionID);
            }

            return gestorIdentidades;
        }

        /// <summary>
        /// Crea una identidad virtual para el perfil con el que está conectado actualmente el usuario
        /// </summary>
        /// <param name="pGestorIdentidades">Gestor de identidades</param>
        /// <param name="pPerfilID">Perfil al que se le quiere agregar la identidad de invitado</param>
        /// <param name="pOrganizacionID">Identificador de la organización del proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto para el que se crea la identidad</param>
        public void CrearIdentidadUsuarioInvitadoParaPerfil(GestionIdentidades pGestorIdentidades, Guid pPerfilID, Guid pOrganizacionID, Guid pProyectoID, Guid pPersonaID)
        {
            AD.EntityModel.Models.IdentidadDS.Identidad fila = null;

            if (pGestorIdentidades.ListaIdentidades.ContainsKey(UsuarioAD.Invitado))
            {
                //La identidad invitado ya existe, solo le cambio el perfil y el proyecto al que hace referencia
                Identidad identInvitado = pGestorIdentidades.ListaIdentidades[UsuarioAD.Invitado];
                fila = identInvitado.FilaIdentidad;

                fila.OrganizacionID = pOrganizacionID;
                fila.ProyectoID = pProyectoID;
                fila.PerfilID = pPerfilID;
                identInvitado.PerfilUsuario = pGestorIdentidades.ListaPerfiles[pPerfilID];
            }
            else
            {
                //Creo la identidad invitado en el gestor
                IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
                fila = identCN.CrearIdentidadUsuarioInvitadoParaPerfil(pGestorIdentidades.DataWrapperIdentidad, pPerfilID, pOrganizacionID, pProyectoID, pPersonaID);
                identCN.Dispose();

                pGestorIdentidades.RecargarHijos();
            }
            Perfil perfil = pGestorIdentidades.ListaPerfiles[pPerfilID];

            //Si la persona no está cargada en el gestor, cargo el gestor de personas
            if (perfil.PersonaID.HasValue && (perfil.PersonaPerfil == null))
            {
                PersonaCN personaCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<PersonaCN>(), mLoggerFactory);
                perfil.GestorIdentidades.GestorPersonas = new GestionPersonas(personaCN.ObtenerPersonaPorIDCargaLigera(perfil.PersonaID.Value), mLoggingService, mEntityContext);
                personaCN.Dispose();
            }

            if (perfil.OrganizacionID.HasValue)
            {
                if (perfil.OrganizacionPerfil == null)
                {
                    // David: Por misterios de la ciencia infusa la propiedad OrganizacionPerfil vale NULL a veces
                    //        Se recarga la org del perfil para que no pase esto
                    OrganizacionCN organizacionCN = new OrganizacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<OrganizacionCN>(), mLoggerFactory);
                    perfil.GestorIdentidades.GestorOrganizaciones.OrganizacionDW.Merge(organizacionCN.ObtenerOrganizacionPorID(perfil.OrganizacionID.Value));
                    organizacionCN.Dispose();

                    perfil.GestorIdentidades.GestorOrganizaciones.CargarOrganizaciones();
                }

                if (perfil.PersonaID.HasValue)
                {
                    if (perfil.OrganizacionPerfil.ModoPersonal)
                    {
                        fila.Tipo = (short)TiposIdentidad.ProfesionalPersonal;
                        fila.NombreCortoIdentidad = perfil.PersonaPerfil.Nombre;
                    }
                    else
                    {
                        fila.Tipo = (short)TiposIdentidad.ProfesionalCorporativo;
                        fila.NombreCortoIdentidad = perfil.NombreCortoOrg;
                    }
                }
                else
                {
                    fila.Tipo = (short)TiposIdentidad.Organizacion;
                    fila.NombreCortoIdentidad = perfil.NombreCortoOrg;
                }
            }
            else
            {
                if (perfil.IdentidadMyGNOSS != null && perfil.IdentidadMyGNOSS.EsIdentidadProfesor)
                {
                    fila.Tipo = (short)TiposIdentidad.Profesor;
                    fila.NombreCortoIdentidad = perfil.IdentidadMyGNOSS.NombreCorto;
                }
                else
                {
                    fila.Tipo = (short)TiposIdentidad.Personal;
                    if (!perfil.Clave.Equals(UsuarioAD.Invitado))
                    {
                        fila.NombreCortoIdentidad = perfil.PersonaPerfil.Nombre;
                    }
                }
            }
            pGestorIdentidades.RecargarHijos();
        }

        public Identidad IdentidadActual
        {
            get
            {
                if (UsuarioActual == null)
                {
                    RecuperarUsuarioDeSesion(UsuarioActual);
                }

                if (mHttpContextAccessor != null && mHttpContextAccessor.HttpContext != null && mHttpContextAccessor.HttpContext.Request != null && mHttpContextAccessor.HttpContext.Request.Headers.ContainsKey("Authorization") && (UsuarioActual == null || UsuarioActual.EsIdentidadInvitada))
                {
                    string authorization = mHttpContextAccessor.HttpContext.Request.Headers["Authorization"];
                    Guid token = Guid.Empty;
                    if (!authorization.Contains(":"))
                    {
                        Guid.TryParse(authorization, out token);
                        if (token != Guid.Empty && mEntityContext.TokenApiLogin.Any(item => item.Token.Equals(token)))
                        {
                            string email = mEntityContext.TokenApiLogin.FirstOrDefault(item => item.Token.Equals(token)).Login;
                            Es.Riam.Gnoss.AD.EntityModel.Models.UsuarioDS.Usuario usuario = mEntityContext.Usuario.FirstOrDefault(item => item.Login.Equals(email));
                            GnossIdentity identity = ValidarUsuario(email);
                            if (identity != null)
                            {
                                UsuarioActual = identity;
                            }
                        }
                    }
                }

                if ((UsuarioActual != null) && ((mIdentidadActual == null) || (mIdentidadActual != null && mIdentidadActual.FilaIdentidad == null)))
                {
                    if (mGestorIdentidades == null)
                    {
                        mGestorIdentidades = CargarGestorIdentidadesUsuarioActual(UtilIdiomas, UsuarioActual);
                    }
                    if ((!mGestorIdentidades.ListaIdentidades.ContainsKey(UsuarioActual.IdentidadID)) && (!UsuarioActual.IdentidadID.Equals(UsuarioAD.Invitado)))
                    {
                        //El gestor cargado de cache no contiene la identidad actual, la vuelvo a cargar (seguramente se acabe de hacer miembro de una comunidad)

                        IdentidadCL identidadCL = new IdentidadCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCL>(), mLoggerFactory);
                        identidadCL.EliminarCacheGestorIdentidadActual(UsuarioActual.UsuarioID, UsuarioActual.IdentidadID, UsuarioActual.PersonaID);

                        mGestorIdentidades = CargarGestorIdentidadesUsuarioActual(UtilIdiomas, UsuarioActual);
                    }

                    PerfilCookieID = UsuarioActual.PerfilID;
                    if (IdentidadIDActual(UsuarioActual) != null && mGestorIdentidades.ListaPerfiles.ContainsKey(IdentidadIDActual(UsuarioActual)[0]))
                    {
                        PerfilCookieID = IdentidadIDActual(UsuarioActual)[0];
                    }

                    if (PerfilCookieID.Equals(UsuarioAD.Invitado) && !UsuarioActual.EsUsuarioInvitado && mGestorIdentidades.ListaPerfiles.Values.Any(perfil => perfil.PersonaID.Equals(UsuarioActual.PersonaID)))
                    {
                        if (mGestorIdentidades.ListaPerfiles.Values.Any(perfil => perfil.PersonaID.Equals(UsuarioActual.PersonaID) && !perfil.OrganizacionID.HasValue))
                        {
                            PerfilCookieID = mGestorIdentidades.ListaPerfiles.Values.First(perfil => perfil.PersonaID.Equals(UsuarioActual.PersonaID) && !perfil.OrganizacionID.HasValue).Clave;
                        }
                        else
                        {
                            PerfilCookieID = mGestorIdentidades.ListaPerfiles.Values.First(perfil => perfil.PersonaID.Equals(UsuarioActual.PersonaID)).Clave;
                        }
                    }

                    if (mGestorIdentidades.ListaIdentidades.ContainsKey(UsuarioActual.IdentidadID) || UsuarioActual.IdentidadID.Equals(UsuarioAD.Invitado))
                    {
                        if ((!mGestorIdentidades.ListaIdentidades.ContainsKey(UsuarioActual.IdentidadID)) && UsuarioActual.IdentidadID.Equals(UsuarioAD.Invitado))
                        {
                            CrearIdentidadUsuarioInvitadoParaPerfil(mGestorIdentidades, PerfilCookieID, UsuarioActual.OrganizacionID, UsuarioActual.ProyectoID, UsuarioActual.PersonaID);
                        }
                        mIdentidadActual = mGestorIdentidades.ListaIdentidades[UsuarioActual.IdentidadID];
                    }
                    else
                    {
                        List<AD.EntityModel.Models.IdentidadDS.Identidad> listaIdentidad = mGestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Where(ident => ident.ProyectoID.Equals(ProyectoAD.MetaProyecto) && ident.OrganizacionID.Equals(ProyectoAD.MetaOrganizacion) && !ident.Tipo.Equals((short)TiposIdentidad.Organizacion) && !ident.PerfilID.Equals(PerfilCookieID)).ToList();
                        if (listaIdentidad.Count > 0)
                        {
                            mIdentidadActual = mGestorIdentidades.ListaIdentidades[listaIdentidad.First().IdentidadID];
                        }
                        else if (mGestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Any(ident => ident.ProyectoID.Equals(ProyectoAD.MetaProyecto) && !ident.Tipo.Equals((short)TiposIdentidad.Organizacion)))
                        {
                            mIdentidadActual = mGestorIdentidades.ListaIdentidades[mGestorIdentidades.DataWrapperIdentidad.ListaIdentidad.First(ident => ident.ProyectoID.Equals(ProyectoAD.MetaProyecto) && !ident.Tipo.Equals((short)TiposIdentidad.Organizacion)).IdentidadID];
                        }
                    }

                    if (mIdentidadActual != null && mIdentidadActual.EsIdentidadProfesor)
                    {
                        //Configuro la identidad del profesor
                        ConfigurarIdentidadProfesor(mIdentidadActual);
                    }
                }

                return mIdentidadActual;
            }
            set
            {
                mIdentidadActual = value;
            }
        }

        /// <summary>
        /// Configura la identidad del profesor para que pueda entrar tanto a su perfil de profesor como a la administración de sus clases
        /// </summary>
        public void ConfigurarIdentidadProfesor(Identidad pIdentidad)
        {
            AD.EntityModel.Models.IdentidadDS.Perfil filaPerfil = pIdentidad.PerfilUsuario.FilaPerfil;

            if (RequestParams("nombreOrgRewrite") != null)
            {
                List<AD.EntityModel.Models.OrganizacionDS.Organizacion> filasOrg = IdentidadActual.GestorIdentidades.GestorOrganizaciones.OrganizacionDW.ListaOrganizacion.Where(item => item.NombreCorto.Equals(RequestParams("nombreOrgRewrite"))).ToList();
                if (filasOrg.Count > 0)
                {
                    AD.EntityModel.Models.OrganizacionDS.Organizacion filaOrg = filasOrg.First();
                    filaPerfil.NombreCortoOrg = filaOrg.NombreCorto;
                    filaPerfil.NombreOrganizacion = filaOrg.Nombre;
                    filaPerfil.OrganizacionID = filaOrg.OrganizacionID;
                }
            }
            else
            {
                filaPerfil.OrganizacionID = null;
                filaPerfil.NombreCortoOrg = null;
                filaPerfil.NombreOrganizacion = null;
            }
        }

        /// <summary>
        /// Obtiene el dataset de parámetros de aplicación
        /// </summary>
        public List<ParametroAplicacion> ListaParametrosAplicacion
        {
            get
            {
                if (mListaParametrosAplicacion == null)
                {
                    mListaParametrosAplicacion = GestorParametrosAplicacion.ParametroAplicacion;
                }
                return mListaParametrosAplicacion;
            }
        }

        private bool? mIgnorarVistasPersonalizadas = null;
        public bool IgnorarVistasPersonalizadas
        {
            get
            {
                if (mIgnorarVistasPersonalizadas == null)
                {
                    mIgnorarVistasPersonalizadas = false;

                    string valueAuxi = mConfigService.ObtenerIgnorarVistasPersonalizadas();
                    if (!string.IsNullOrEmpty(valueAuxi) && string.Compare(valueAuxi, "true", true) == 0)
                    {
                        mIgnorarVistasPersonalizadas = true;
                    }
                }
                return (bool)mIgnorarVistasPersonalizadas;
            }
        }

        /// <summary>
        /// Obtiene la URL del los elementos de contenido de la página
        /// </summary>
        public string BaseURLContent
        {
            get
            {
                if (mBaseURLContent == null)
                {

                    mBaseURLContent = mConfigService.ObtenerUrlContent();

                    if (mBaseURLContent == "")
                    {
                        mBaseURLContent = BaseURL;
                    }
                }
                return mBaseURLContent;
            }
        }

        /// <summary>
        /// ID del proyecto externo en el que se está haciendo la búsqueda, o GUID.EMPTY si se hace en el proyecto actual.
        /// </summary>
        private Guid? mProyectoOrigenBusquedaID;

        /// <summary>
        /// ID del proyecto externo en el que se está haciendo la búsqueda, o GUID.EMPTY si se hace en el proyecto actual.
        /// </summary>
        public Guid ProyectoOrigenBusquedaID
        {
            get
            {
                if (!mProyectoOrigenBusquedaID.HasValue)
                {

                    //"ProyectoOrigenID IS NOT NULL"
                    List<ProyectoPestanyaBusqueda> filasPestaniaBusqueda = ProyectoSeleccionado.GestorProyectos.DataWrapperProyectos.ListaProyectoPestanyaBusqueda.Where(proyectoPestanyaBusqueda => proyectoPestanyaBusqueda.ProyectoOrigenID != null).ToList();
                    if (filasPestaniaBusqueda.Count > 0)
                    {
                        mProyectoOrigenBusquedaID = filasPestaniaBusqueda[0].ProyectoOrigenID;
                    }
                    else
                    {
                        mProyectoOrigenBusquedaID = Guid.Empty;
                    }
                }

                return mProyectoOrigenBusquedaID.Value;
            }
        }

        private string mVersion = string.Empty;

        /// <summary>
        /// Obtiene la versión de la aplicación
        /// </summary>
        public string Version
        {
            get
            {
                if (mVersion == string.Empty)
                {
                    try
                    {
                        string ficheroVersion = "Config/version.txt";

                        StreamReader sr = new StreamReader(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + ficheroVersion);
                        mVersion = sr.ReadLine();
                        sr.Close();
                    }
                    catch
                    {
                        mVersion = "1.0.0.0";
                    }
                }
                return mVersion;
            }
        }

        /// <summary>
        /// BaseURL Personalizacion
        /// </summary>
        private string mBaseURLPersonalizacion = null;

        /// <summary>
        /// Obtiene la URL del los elementos de contenido de la página
        /// </summary>
        public string BaseURLPersonalizacion
        {
            get
            {
                if (mBaseURLPersonalizacion == null)
                {
                    mBaseURLPersonalizacion = mConfigService.ObtenerUrlContent();

                    if (mBaseURLPersonalizacion == "")
                    {
                        mBaseURLPersonalizacion = BaseURL;
                    }

                    if (ParametroProyecto.ContainsKey("RutaEstilos") && !string.IsNullOrEmpty(ParametroProyecto["RutaEstilos"]))
                    {
                        mBaseURLPersonalizacion = mBaseURLPersonalizacion + "/imagenes/proyectos/personalizacion/" + ParametroProyecto["RutaEstilos"];
                        mBaseURLPersonalizacion = mBaseURLPersonalizacion.Replace("\\", "/");
                    }
                    else
                    {
                        mBaseURLPersonalizacion = mBaseURLPersonalizacion + "/imagenes/proyectos/personalizacion/" + ProyectoSeleccionado.Clave.ToString();
                    }


                }
                return mBaseURLPersonalizacion;
            }
        }

        /// <summary>
        /// Obtiene la URL del los elementos de contenido de la página
        /// </summary>
        public string BaseURLPersonalizacionEcosistema
        {
            get
            {
                if (mBaseURLPersonalizacionEcosistema == null)
                {
                    mBaseURLPersonalizacionEcosistema = mConfigService.ObtenerUrlContent();

                    if (mBaseURLPersonalizacionEcosistema == "")
                    {
                        mBaseURLPersonalizacionEcosistema = BaseURL;
                    }

                    if (ParametroProyectoEcosistema.ContainsKey("RutaEstilos") && !string.IsNullOrEmpty(ParametroProyectoEcosistema["RutaEstilos"]))
                    {
                        mBaseURLPersonalizacionEcosistema = mBaseURLPersonalizacionEcosistema + "/imagenes/proyectos/personalizacion/" + ParametroProyectoEcosistema["RutaEstilos"];
                        mBaseURLPersonalizacionEcosistema = mBaseURLPersonalizacionEcosistema.Replace("\\", "/");
                    }
                    else
                    {
                        mBaseURLPersonalizacionEcosistema = mBaseURLPersonalizacionEcosistema + "/imagenes/proyectos/personalizacion/ecosistema";
                    }


                }
                return mBaseURLPersonalizacionEcosistema;
            }
        }

        Dictionary<string, List<string>> mInformacionOntologias;

        /// <summary>
        /// Obtiene la lista de items extra que se obtendrá de la búsqueda y su prefijo (recetas, peliculas, etc)
        /// </summary>
        public Dictionary<string, List<string>> InformacionOntologias
        {
            get
            {
                if (mInformacionOntologias == null)
                {
                    UtilServiciosWeb.UtilServiciosFacetas utilServiciosFacetas = new UtilServiciosWeb.UtilServiciosFacetas(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<UtilServiciosFacetas>(), mLoggerFactory);
                    mInformacionOntologias = utilServiciosFacetas.ObtenerInformacionOntologias(ProyectoSeleccionado.FilaProyecto.OrganizacionID, ProyectoSeleccionado.Clave);
                }

                return mInformacionOntologias;
            }
        }

        /// <summary>
        /// Fila del dataset de parámetros generales del proyecto virtual.
        /// </summary>
        protected ParametroGeneral mParametroGeneralVirtualRow;

        /// <summary>
        /// Obtiene el dataset de parámetros generales del proyectoVirtual
        /// </summary>
        public ParametroGeneral ParametrosGeneralesVirtualRow
        {
            get
            {
                if (mParametroGeneralVirtualRow == null)
                {
                    if (!ProyectoVirtual.Clave.Equals(ProyectoSeleccionado.Clave))
                    {
                        mParametroGeneralVirtualRow = ObtenerFilaParametrosGeneralesDeProyecto(ProyectoVirtual.Clave).ListaParametroGeneral.FirstOrDefault();
                    }
                    else
                    {
                        mParametroGeneralVirtualRow = ParametrosGeneralesRow;
                    }
                }
                return mParametroGeneralVirtualRow;
            }
        }

        public Guid ProyectoPrincipalUnico
        {
            get
            {
                if (mProyectoPrincipalUnico == null)
                {
                    mProyectoPrincipalUnico = ProyectoAD.MetaProyecto;
                    ParametroAplicacion busqueda = ListaParametrosAplicacion.FirstOrDefault(parametro => parametro.Parametro.Equals(TiposParametrosAplicacion.ComunidadPrincipalID));
                    if (busqueda != null)
                    {
                        mProyectoPrincipalUnico = new Guid(busqueda.Valor);
                    }
                }
                return mProyectoPrincipalUnico.Value;
            }
        }

        public ParametroGeneral ParametrosGeneralesRow
        {
            get
            {
                if (mParametroGeneralRow == null || mParametroGeneralRow.ProyectoID != ProyectoVirtual.Clave)
                {
                    Guid proyectoid = ProyectoAD.MetaProyecto;
                    if (ProyectoVirtual != null)
                    {
                        proyectoid = ProyectoVirtual.Clave;
                        if (ProyectoIDPadreEcosistema.HasValue && !ProyectoIDPadreEcosistema.Equals(Guid.Empty) && ProyectoVirtual.FilaProyecto.ProyectoSuperiorID.HasValue)
                        {
                            proyectoid = ProyectoIDPadreEcosistema.Value;
                        }
                    }

                    mParametroGeneralRow = ObtenerFilaParametrosGeneralesDeProyecto(proyectoid).ListaParametroGeneral.FirstOrDefault();
                }
                return mParametroGeneralRow;
            }
        }


        /// <summary>
        /// Obtiene atraves de un guid y el ParametroGeneralDS la fila de parametros generales del proyecto correspondiente
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto a buscar</param>
        /// <returns>Fila del parametro general</returns>
        //public static ParametroGeneralDS.ParametroGeneralRow ObtenerFilaParametrosGeneralesDeProyecto(Guid pProyectoID)
        public GestorParametroGeneral ObtenerFilaParametrosGeneralesDeProyecto(Guid pProyectoID)
        {
            ParametroGeneral filaParametroGeneral = null;
            GestorParametroGeneral paramDS = ObtenerParametrosGeneralesDeProyecto(pProyectoID);
            List<ParametroGeneral> parametroGen = new List<ParametroGeneral>();
            if (paramDS != null && paramDS.ListaParametroGeneral.FirstOrDefault(parametroGeneral => parametroGeneral.ProyectoID.Equals(pProyectoID)) != null)
            {
                filaParametroGeneral = paramDS.ListaParametroGeneral.Find(parametroGeneral => parametroGeneral.ProyectoID.Equals(pProyectoID));
                parametroGen.Add(filaParametroGeneral);
                paramDS.ListaParametroGeneral = parametroGen;
            }
            return paramDS;
        }

        public GestorParametroGeneral ObtenerParametrosGeneralesDeProyecto(Guid pProyectoID)
        {
            GestorParametroGeneral gestorParametroGenral = new GestorParametroGeneral();
            ParametroGeneralGBD gestorController = new ParametroGeneralGBD(mEntityContext);
            gestorParametroGenral = gestorController.ObtenerParametrosGeneralesDeProyecto(gestorParametroGenral, pProyectoID);
            return gestorParametroGenral;
        }

        private bool? mCargarIdentidadesDeProyectosPrivadosComoAmigos;

        public bool CargarIdentidadesDeProyectosPrivadosComoAmigos
        {
            get
            {
                if (!mCargarIdentidadesDeProyectosPrivadosComoAmigos.HasValue)
                {
                    List<ParametroAplicacion> busqueda = ListaParametrosAplicacion.Where(parametro => parametro.Parametro.Equals((string)TiposParametrosAplicacion.CargarIdentidadesDeProyectosPrivadosComoAmigos)).ToList();
                    if (busqueda.Count > 0)
                    {
                        mCargarIdentidadesDeProyectosPrivadosComoAmigos = busqueda.Count > 0 && busqueda.FirstOrDefault().Valor.Equals("1") || busqueda.FirstOrDefault().Valor.ToLower().Equals("true");
                    }
                    else
                    {
                        mCargarIdentidadesDeProyectosPrivadosComoAmigos = false;
                    }
                }
                return mCargarIdentidadesDeProyectosPrivadosComoAmigos.Value;
            }
        }

        private bool mCargandoProyectoVirtual = false;

        /// <summary>
        /// Obtiene el identificador del proyecto al que se conecta la aplicación
        /// </summary>
        public Guid ProyectoPrincipal
        {
            get
            {
                Guid? proPrincipal = mConfigService.ObtenerProyectoPrincipal();
                if (proPrincipal.HasValue)
                {
                    mProyectoPrincipal = proPrincipal;
                }
                else
                {
                    mProyectoPrincipal = ProyectoAD.MetaProyecto;
                }
                return mProyectoPrincipal.Value;
            }
        }

        public GnossIdentity mUsuarioActual;

        public GnossIdentity UsuarioActual
        {
            get
            {
                if (mUsuarioActual != null)
                {
                    return mUsuarioActual;
                }
                else
                {
                    ISessionFeature sessionFeature = mHttpContextAccessor.HttpContext.Features.Get<ISessionFeature>();
                    return sessionFeature == null ? null : mHttpContextAccessor.HttpContext.Session.Get<GnossIdentity>("Usuario");
                }
            }
            set
            {
                mHttpContextAccessor.HttpContext.Session.Set("Usuario", value);
            }
        }
        /// <summary>
        /// Obtiene el proyecto virtual (Para cuando es un ecosistema sin metaproyecto)
        /// </summary>
        public Elementos.ServiciosGenerales.Proyecto ProyectoVirtual
        {
            get
            {
                //throw new Exception("TODO: Cargar esto en el on action executing");

                if (mProyectoVirtual == null)
                {
                    mCargandoProyectoVirtual = true;
                    Guid proyectoVirtualID = Guid.Empty;
                    ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);

                    if (EsEcosistemaSinMetaProyecto && ProyectoSeleccionado != null && ProyectoSeleccionado.Clave.Equals(ProyectoAD.MetaProyecto) && !this.TipoPagina.Equals(TiposPagina.Administacion) /*!string.IsNullOrEmpty(RequestParams("ecosistema")) && !RequestParams("ecosistema").Equals("true")*/)
                    {
                        proyectoVirtualID = ProyectoPrincipalUnico;

                        if (proyectoVirtualID == ProyectoAD.MetaProyecto)
                        {
                            proyectoVirtualID = ProyectoPrincipal;

                            if (UsuarioActual != null && !UsuarioActual.EsUsuarioInvitado)
                            {
                                proyectoVirtualID = proyCL.ObtenerUltimoProyectoUsuario(UsuarioActual.UsuarioID, BaseURL);

                                if (proyectoVirtualID.Equals(Guid.Empty))
                                {
                                    ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
                                    proyectoVirtualID = proyCN.ObtenerProyectoIDMasActivoPerfil(IdentidadActual.PerfilID);
                                    proyCN.Dispose();

                                    if (!proyectoVirtualID.Equals(Guid.Empty))
                                    {
                                        proyCL.AgregarUltimoProyectoUsuario(proyectoVirtualID, UsuarioActual.UsuarioID, BaseURL);
                                    }
                                }
                            }
                        }
                    }

                    if (!proyectoVirtualID.Equals(Guid.Empty))
                    {
                        GestionProyecto gestProy = new GestionProyecto(proyCL.ObtenerProyectoPorID(proyectoVirtualID), mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestionProyecto>(), mLoggerFactory);
                        mProyectoVirtual = gestProy.ListaProyectos[proyectoVirtualID];
                    }
                    else
                    {
                        mProyectoVirtual = ProyectoSeleccionado;
                    }
                }

                return mProyectoVirtual;
            }
        }

        /// <summary>
        /// Obtiene si se trata de un ecosistema sin metaproyecto
        /// </summary>
        public bool EsEcosistemaSinMetaProyecto
        {
            get
            {
                if (!mEsEcosistemaSinMetaProyecto.HasValue)
                {
                    ParametroAplicacion busqueda = GestorParametrosAplicacion.ParametroAplicacion.FirstOrDefault(parametro => parametro.Parametro.Equals(TiposParametrosAplicacion.EcosistemaSinMetaProyecto));
                    mEsEcosistemaSinMetaProyecto = busqueda != null && bool.Parse(busqueda.Valor);
                }
                return mEsEcosistemaSinMetaProyecto.Value;
            }
        }

        /// <summary>
        /// Obtiene la URL de presentación para este sitio web
        /// </summary>
        public string UrlPresentacion
        {
            get
            {
                //TODO Esto es imposible que funcione así
                if (mUrlPresentacion == null)
                {
                    mUrlPresentacion = mConfigService.ObtenerUrlPaginaPresentacion();

                    if (mUrlPresentacion == null)
                    {
                        mUrlPresentacion = "";
                    }
                }

                return mUrlPresentacion;
            }
        }

        /// <summary>
        /// Obtiene la dirección base
        /// </summary>
        public string BaseURLSinHTTP
        {
            get
            {
                string url = mHttpContextAccessor.HttpContext.Request.PathBase.ToString().Trim();

                if (url.Length == 1)
                {
                    url = "";
                }
                return url;
            }
        }

        private bool EsDominioAdministracion(string pDominioPeticion)
        {
            if (string.IsNullOrEmpty(DominioPaginasAdministracion))
            {
                return false;
            }

            string dominioPeticion = UtilDominios.DomainToPunyCode(pDominioPeticion);
            string dominioPaginasAdministracion = "";
            if (!string.IsNullOrEmpty(DominioPaginasAdministracion))
            {
                dominioPaginasAdministracion = new Uri(UtilDominios.DomainToPunyCode(DominioPaginasAdministracion)).Host;
            }

            return dominioPeticion.Equals(dominioPaginasAdministracion);
        }

        /// <summary>
        /// Obtiene la URL base de la página
        /// </summary>
        public string BaseURL
        {
            get
            {
                if (string.IsNullOrEmpty(mBaseUrl) || !string.IsNullOrEmpty(DominioPaginasAdministracion))
                {
                    string host = mHttpContextAccessor.HttpContext.Request.Host.ToString();
                    string rutaEjecucionWeb = mConfigService.ObtenerRutaEjecucionWeb();
                    mBaseUrl = mConfigService.ObtenerUrlBase();

                    if (!string.IsNullOrEmpty(DominioPaginasAdministracion))
                    {
                        if (EsDominioAdministracion(host))
                        {
                            mBaseUrl = $"{DominioPaginasAdministracion}/{rutaEjecucionWeb.Trim('/')}";
                        }
                    }
                    else
                    {
                        host = UtilDominios.DomainToPunyCode(host);
                        if (mConfigService.PeticionHttps())
                        {
                            mBaseUrl = $"https://{host}/{rutaEjecucionWeb}{BaseURLSinHTTP}".Trim('/');
                        }
                        else
                        {
                            mBaseUrl = $"http://{host}/{rutaEjecucionWeb}{BaseURLSinHTTP}".Trim('/');
                        }
                    }
                }

                return mBaseUrl;
            }
        }

        /// <summary>
        /// Redirecciona el response a sí misma
        /// </summary>
        /// <param name="pCambiarHttps">Verdad si se debe cambiar http por https</param>
        public void RedireccionarMismaPagina(bool pCambiarHttps)
        {
            string url = mUtilWeb.AbsoluteUri(mHttpContextAccessor.HttpContext.Request).Substring(0, mUtilWeb.AbsoluteUri(mHttpContextAccessor.HttpContext.Request).IndexOf('?'));
            if (pCambiarHttps)
            {
                url = UtilDominios.CambiarHttpAHttpsEnUrl(url);
            }
            mHttpContextAccessor.HttpContext.Response.Redirect(url);
        }

        /// <summary>
        /// Obtiene la URL del los elementos estaticos de la página
        /// </summary>
        public string BaseURLStatic
        {
            get
            {
                if (string.IsNullOrEmpty(mBaseURLStatic))
                {

                    mBaseURLStatic = mConfigService.ObtenerUrlServicio("urlStatic");

                    if (string.IsNullOrEmpty(mBaseURLStatic))
                    {
                        mBaseURLStatic = BaseURL;
                    }
                }
                return mBaseURLStatic;
            }
        }

        /// <summary>
        /// Parametros de configuración del proyecto.
        /// </summary>
        private Dictionary<string, string> ParametroProyecto
        {
            get
            {
                if (mParametroProyecto == null)
                {
                    ProyectoCL proyectoCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);
                    mParametroProyecto = proyectoCL.ObtenerParametrosProyecto(ProyectoSeleccionado.Clave);
                    proyectoCL.Dispose();
                }

                return mParametroProyecto;
            }
        }

        /// <summary>
        /// Parametros de configuración del proyecto.
        /// </summary>
        public Dictionary<string, string> ParametroProyectoEcosistema
        {
            get
            {
                if (mParametroProyectoEcosistema == null)
                {
                    ProyectoCL proyectoCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);
                    mParametroProyectoEcosistema = proyectoCL.ObtenerParametrosProyecto(ProyectoAD.MetaProyecto);
                    proyectoCL.Dispose();
                }

                return mParametroProyectoEcosistema;
            }
        }

        public string EspacioPersonal
        {
            get
            {
                List<ParametroAplicacion> busqueda = ListaParametrosAplicacion.Where(parametro => parametro.Parametro.Equals("NombreEspacioPersonal")).ToList();
                if (string.IsNullOrEmpty(mEspacioPersonal) && busqueda.Count > 0)
                {
                    mEspacioPersonal = busqueda.First().Valor;

                }

                if (string.IsNullOrEmpty(mEspacioPersonal))
                {
                    mEspacioPersonal = UtilIdiomas.GetText("COMMON", "MIESPACIOPERSONAL");
                }

                return mEspacioPersonal;
            }
        }

        protected string NombreProyectoPadreEcositema
        {
            get
            {
                return mNombreProyectoPadreEcositema;
            }
        }

        protected Guid? ProyectoIDPadreEcosistema
        {
            get
            {
                if (mPadreEcosistemaProyectoID == null)
                {

                    if (!string.IsNullOrEmpty(NombreCortoProyectoPadreEcositema))
                    {
                        try
                        {
                            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
                            mPadreEcosistemaProyectoID = proyCN.ObtenerProyectoIDPorNombreCorto(NombreCortoProyectoPadreEcositema);
                            proyCN.Dispose();
                        }
                        catch
                        {
                            mLoggingService.GuardarLogError("El parametro NombreCortoProyectoPadreEcositema no esta bien configurado.", mlogger);
                            mPadreEcosistemaProyectoID = Guid.Empty;
                        }
                    }

                    if (!string.IsNullOrEmpty(NombreProyectoPadreEcositema) && (mPadreEcosistemaProyectoID == null || (mPadreEcosistemaProyectoID != null && mPadreEcosistemaProyectoID.Value.Equals(Guid.Empty))))
                    {
                        try
                        {
                            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
                            mPadreEcosistemaProyectoID = proyCN.ObtenerProyectoIDPorNombreCorto(NombreProyectoPadreEcositema);
                            proyCN.Dispose();
                        }
                        catch
                        {
                            mLoggingService.GuardarLogError("El parametro ComunidadPadreEcosistemaID no esta bien configurado.", mlogger);
                            mPadreEcosistemaProyectoID = Guid.Empty;
                        }
                    }

                    if (mPadreEcosistemaProyectoID == null)
                    {
                        mPadreEcosistemaProyectoID = Guid.Empty;
                    }
                }
                return mPadreEcosistemaProyectoID;
            }
        }

        public string NombreCortoProyectoPadreEcositema
        {
            get
            {
                if (mNombreCortoProyectoPadreEcosistema == null)
                {
                    ParametroAplicacionCL paramCL = new ParametroAplicacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCL>(), mLoggerFactory);
                    List<ParametroAplicacion> parametrosAplicacionDS = paramCL.ObtenerParametrosAplicacionPorContext();
                    ParametroAplicacion NombreCortoProyectoPadreEcositema = parametrosAplicacionDS.FirstOrDefault(parametro => parametro.Parametro.Equals("NombreCortoProyectoPadreEcositema"));
                    paramCL.Dispose();
                    if (NombreCortoProyectoPadreEcositema != null)
                    {
                        try
                        {
                            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
                            mNombreCortoProyectoPadreEcosistema = NombreCortoProyectoPadreEcositema.Valor;
                            proyCN.Dispose();
                        }
                        catch
                        {
                            mLoggingService.GuardarLogError("El parametro NombreCortoProyectoPadreEcositema no esta bien configurado.", mlogger);
                            mNombreCortoProyectoPadreEcosistema = "";
                        }
                    }
                    if (mNombreCortoProyectoPadreEcosistema == null)
                    {
                        mNombreCortoProyectoPadreEcosistema = "";
                    }
                }
                return mNombreCortoProyectoPadreEcosistema;
            }
        }

        protected string BaseURLFormulariosSem
        {
            get
            {
                if (mBaseURLFormulariosSem == null)
                {
                    string url = UrlIntragnoss;

                    if (url[url.Length - 1] == '/')
                    {
                        url = url.Substring(0, url.Length - 1);
                    }

                    mBaseURLFormulariosSem = url.Replace("www.", "");
                }

                return mBaseURLFormulariosSem;
            }
        }

        /// <summary>
        /// Obtiene el idioma del usuario
        /// </summary>
        public string IdiomaUsuario
        {
            get
            {
                if (mIdiomaUsuario == null)
                {
                    mIdiomaUsuario = RequestParams("lang");
                }
                return mIdiomaUsuario;
            }
            set
            {
                mIdiomaUsuario = value;
            }
        }

        /// <summary>
        /// Devuelve el Page.Request.Params de la pagina para que funcione con ajax
        /// </summary>
        public string RequestParams(string pParametro)
        {
            string valorParametro = null;

            try
            {
                if (mHttpContextAccessor.HttpContext.Request.Query.ContainsKey(pParametro))
                {
                    valorParametro = mHttpContextAccessor.HttpContext.Request.Query[pParametro];
                }
                else if (mHttpContextAccessor.HttpContext.Request.RouteValues.ContainsKey(pParametro))
                {
                    valorParametro = mHttpContextAccessor.HttpContext.Request.RouteValues[pParametro] as string;

                }
                else if (mHttpContextAccessor.HttpContext.Request.HasFormContentType && mHttpContextAccessor.HttpContext.Request.Form != null && mHttpContextAccessor.HttpContext.Request.Form.ContainsKey(pParametro))
                {
                    valorParametro = mHttpContextAccessor.HttpContext.Request.Form[pParametro];
                }
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex.Message, mlogger);
            }

            return valorParametro;
        }

        public GestorParametroAplicacion GestorParametrosAplicacion
        {
            get
            {
                if (mGestorParametrosAplicacion == null)
                {
                    ParametroAplicacionCL paramCL = new ParametroAplicacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCL>(), mLoggerFactory);

                    mGestorParametrosAplicacion = paramCL.ObtenerGestorParametros();
                }
                return mGestorParametrosAplicacion;
            }
        }

        public bool HayConexionRabbit
        {
            get
            {
                return mConfigService.ExistRabbitConnection(RabbitMQClient.BD_SERVICIOS_WIN);

            }
        }


        /// <summary>
        /// Valida solo con el login del usuario
        /// </summary>
        /// <param name="pNombre">Nombre del usuario</param>
        /// <returns>TRUE si usuario y contraseña son correctos, FALSE en caso contrario</returns>
        public GnossIdentity ValidarUsuario(string pNombre)
        {
            return ValidarUsuario(pNombre, null, false, ProyectoAD.MetaOrganizacion, ProyectoAD.MetaProyecto, false);
        }

        /// <summary>
        /// Valida nombre y contraseña del usuario (pendiente de activar)
        /// </summary>
        /// <param name="pNombre">Nombre del usuario</param>
        /// <param name="pContrasenia">Contraseña de la cuenta</param>
        /// <returns>TRUE si usuario y contraseña son correctos, FALSE en caso contrario</returns>
        public bool ValidarUsuarioPendienteDeActivar(string pNombre, string pContrasenia)
        {
            UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<UsuarioCN>(), mLoggerFactory);

            Es.Riam.Gnoss.AD.EntityModel.Models.UsuarioDS.Usuario filaUsuario = usuarioCN.ObtenerFilaUsuarioPorLoginOEmail(pNombre);

            return filaUsuario != null && usuarioCN.ValidarPasswordUsuarioSinActivar(filaUsuario, pContrasenia);
        }

        /// <summary>
        /// Valida nombre y contraseña del usuario
        /// </summary>
        /// <param name="pNombre">Nombre del usuario</param>
        /// <param name="pContrasenia">Contraseña de la cuenta</param>
        /// <returns>TRUE si usuario y contraseña son correctos, FALSE en caso contrario</returns>
        public GnossIdentity ValidarUsuario(string pNombre, string pContrasenia)
        {
            return ValidarUsuario(pNombre, pContrasenia, false, ProyectoAD.MetaOrganizacion, ProyectoAD.MetaProyecto, true);
        }

        /// <summary>
        /// Valida nombre y contraseña del usuario
        /// </summary>
        /// <param name="pNombre">Nombre del usuario</param>
        /// <param name="pContrasenia">Contraseña de la cuenta</param>
        /// <param name="pLanzarExcepciones">TRUE si se deben lanzar las excepciones producidas, FALSE en caso contrario</param>
        /// <param name="pOrganizacionID">Identificador de la organización del proyecto al que se va a conectar el usuario</param>
        /// <param name="pProyectoID">Identificador del proyecto al que se conecta el usuario</param>
        /// <param name="pValidarPassword">Verdad si se debe validar el password</param>
        /// <returns>TRUE si usuario y contraseña son correctos, FALSE en caso contrario</returns>
        public GnossIdentity ValidarUsuario(string pNombre, string pContrasenia, bool pLanzarExcepciones, Guid pOrganizacionID, Guid pProyectoID, bool pValidarPassword)
        {
            GnossIdentity identity = null;
            try
            {
                // Autenticamos el login para la organización (autenticación parcial)
                UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<UsuarioCN>(), mLoggerFactory);
                DataWrapperUsuario dataWrapperUsuario = new DataWrapperUsuario();
                Es.Riam.Gnoss.AD.EntityModel.Models.UsuarioDS.Usuario filaUsuario = null;

                if (!string.IsNullOrEmpty(pNombre))
                {
                    dataWrapperUsuario = usuarioCN.AutenticarLogin(pNombre, false);
                    if (dataWrapperUsuario.ListaUsuario.Count > 0)
                    {
                        filaUsuario = dataWrapperUsuario.ListaUsuario.First();

                        identity = GenerarGnossIdentity(filaUsuario, pOrganizacionID, pProyectoID);

                        //Autenticamos la password (autenticación completa)
                        if (pValidarPassword)
                        {
                            if (!usuarioCN.ValidarPasswordUsuario(filaUsuario, pContrasenia))
                            {
                                throw new ErrorPassword();
                            }
                        }
                    }
                }

                usuarioCN.Dispose();
            }
            catch (Exception)
            {
                identity = null;
                if (pLanzarExcepciones)
                {
                    throw;
                }
            }
            return identity;
        }

        public GnossIdentity GenerarGnossIdentity(Es.Riam.Gnoss.AD.EntityModel.Models.UsuarioDS.Usuario pFilaUsuario, Guid pOrganizacionID, Guid pProyectoID)
        {
            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
            PersonaCN personaCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<PersonaCN>(), mLoggerFactory);

            Guid? personaID = null;
            Guid perfilID = Guid.Empty;
            Guid identidadID = Guid.Empty;
            Guid? profesorID = null;
            GnossIdentity identity = null;

            if (pFilaUsuario != null)
            {
                personaID = personaCN.ObtenerPersonaIDPorUsuarioID(pFilaUsuario.UsuarioID);

                try
                {
                    Guid[] array = identidadCN.ObtenerIdentidadIDDePersonaEnProyecto(pProyectoID, personaID.Value);

                    if (array != null && array.Length > 1)
                    {
                        identidadID = array[0];
                        perfilID = array[1];
                    }
                    else if (pProyectoID.Equals(ProyectoAD.MetaProyecto))
                    {
                        identidadID = UsuarioAD.Invitado;
                        perfilID = UsuarioAD.Invitado;
                    }
                }
                catch
                {
                    throw new ErrorAccesoProyecto(pProyectoID.ToString());
                }

                if (identidadID.Equals(Guid.Empty))
                {
                    throw new ErrorAccesoProyecto(pProyectoID.ToString());
                }

                if (personaID.HasValue)
                {
                    profesorID = identidadCN.ObtenerProfesorID(personaID.Value);
                }

                identidadCN.Dispose();

                //Crear la identidad
                identity = new GnossIdentity();
                identity.UsuarioID = pFilaUsuario.UsuarioID;
                identity.IdentidadID = identidadID;
                identity.PerfilID = perfilID;
                identity.Login = pFilaUsuario.Login;
                identity.OrganizacionID = pOrganizacionID;
                identity.ProyectoID = pProyectoID;
                identity.PerfilProfesorID = profesorID;
                identity.UsarMasterParaLectura = true;

                if (pFilaUsuario.FechaCambioPassword.HasValue)
                {
                    identity.FechaCambioPassword = pFilaUsuario.FechaCambioPassword;
                }

                if (!personaID.HasValue)
                {
                    throw new ErrorUsuarioSinPersona();
                }
                identity.PersonaID = personaID.Value;

                //Asigno el usuario al hilo principal
                //AsignarIdentityAHiloActual(identity);
                AgregarObjetoAPeticionActual("GnossIdentity", identity);

                //Obtengo la identidad y el proyecto al que se conecta el usuario
                identity.EstaPasswordAutenticada = true;

                //Calculo los permisos del usuario
                CalcularPermisosUsuario(pFilaUsuario, identity);
            }
            return identity;
        }

        public void InsertarFilaEnColaRabbitMQ(Guid pProyectoID, Guid pID, int pAccion, int pTipo, int pNumIntentos, DateTime pFecha, bool pSoloPersonal, short pPrioridad, IAvailableServices pAvailableServices, string pInfoExtra = null)
        {
            string elemento = PreprarFilaParaColaRabbitMQ(pProyectoID, pID, pAccion, pTipo, pNumIntentos, pFecha, pSoloPersonal, pPrioridad, pInfoExtra);

            //AcctionLive.VisitaRecurso

            if (AccionLive.VisitaRecurso.Equals(pAccion) && HayConexionRabbit)
            {
                using (RabbitMQClient rabbitMQ = new RabbitMQClient(RabbitMQClient.BD_SERVICIOS_WIN, COLA_VISITAS, mLoggingService, mConfigService, mLoggerFactory.CreateLogger<RabbitMQClient>(), mLoggerFactory ,EXCHANGE, COLA_VISITAS))
                {
                    rabbitMQ.AgregarElementoACola(elemento);
                }
            }
            else if (HayConexionRabbit)
            {
                using (RabbitMQClient rabbitMQ = new RabbitMQClient(RabbitMQClient.BD_SERVICIOS_WIN, COLA_RABBIT, mLoggingService, mConfigService, mLoggerFactory.CreateLogger<RabbitMQClient>(), mLoggerFactory, EXCHANGE, COLA_RABBIT))
                {
                    rabbitMQ.AgregarElementoACola(elemento);
                }
            }
        }

        public void InsertarFilasEnColaRabbitMQ(List<string> pFilasAInsertar, string pRutaBase = "")
        {
            if (HayConexionRabbit)
            {
                using (RabbitMQClient rabbitMQ = new RabbitMQClient(RabbitMQClient.BD_SERVICIOS_WIN, COLA_RABBIT, mLoggingService, mConfigService, mLoggerFactory.CreateLogger<RabbitMQClient>(), mLoggerFactory, EXCHANGE, COLA_RABBIT))
                {
                    List<string> filasEliminadas = (List<string>)rabbitMQ.AgregarElementosACola(pFilasAInsertar);

                    if (filasEliminadas.Count > 0)
                    {
                        LiveCN liveCN;
                        if (!string.IsNullOrEmpty(pRutaBase))
                        {
                            liveCN = new LiveCN(pRutaBase, mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<LiveCN>(), mLoggerFactory);
                        }
                        else
                        {
                            liveCN = new LiveCN("base", mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<LiveCN>(), mLoggerFactory);
                        }

                        LiveDS liveDS = new LiveDS();

                        foreach (string filaEliminada in filasEliminadas)
                        {
                            mLoggingService.GuardarLogError("Fallo al insertar en Rabbit, insertamos en la base de datos 'BASE', tabla 'cola'", mlogger);
                            liveDS.Cola.AddColaRow(JsonConvert.DeserializeObject<LiveDS.ColaRow>(filaEliminada));
                        }

                        liveCN.ActualizarBD(liveDS);

                        liveCN.Dispose();
                        liveDS.Dispose();
                    }
                }
            }
        }

        public string PreprarFilaParaColaRabbitMQ(Guid pProyectoID, Guid pID, int pAccion, int pTipo, int pNumIntentos, DateTime pFecha, bool pSoloPersonal, short pPrioridad, string pInfoExtra = null)
        {
            LiveDS.ColaRow filaCola = new LiveDS().Cola.NewColaRow();
            filaCola.ProyectoId = pProyectoID;
            filaCola.Id = pID;
            filaCola.Accion = pAccion;
            filaCola.Tipo = pTipo;
            filaCola.NumIntentos = pNumIntentos;
            filaCola.Fecha = pFecha;
            filaCola.SoloPersonal = pSoloPersonal;
            filaCola.Prioridad = pPrioridad;
            filaCola.InfoExtra = pInfoExtra;

            return JsonConvert.SerializeObject(filaCola.ItemArray);
        }
    }
}