using Es.Riam.Gnoss.AD.BASE_BD;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.ParametroGeneralDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.Identidad;
using Es.Riam.Gnoss.AD.Parametro;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Usuarios;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.Identidad;
using Es.Riam.Gnoss.CL.ParametrosAplicacion;
using Es.Riam.Gnoss.CL.ParametrosProyecto;
using Es.Riam.Gnoss.CL.Seguridad;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Elementos.ParametroAplicacion;
using Es.Riam.Gnoss.Elementos.ParametroGeneralDSEspacio;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Documentacion;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Usuarios;
using Es.Riam.Gnoss.Recursos;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Util.Seguridad;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Util;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Es.Riam.Gnoss.Web.Controles
{
    public partial class DatosPeticion
    {
        private static List<string> ListaUrlsSinIdentidad = new List<string>() { "/load-resource-actions", "/peticionesajax/cargarnumelementosnuevos" };
        public const string SESION_UNICA_POR_USUARIO = "sesionUnicaPorUsuarioConCookie";

        #region Miembros

        private string mUrlPresentacion = null;
        private Guid? mProyectoConexionID = null;
        private string mNombreCortoProyectoConexion = null;
        private string mNombreCortoProyectoPrincipal = null;

        private Guid mProyectoSeleccionadoError404 = Guid.Empty;
        private string mIdiomaUsuario = null;
        private UtilIdiomas mUtilIdiomas = null;

        /// <summary>
        /// Contiene la URL del servicio web de documentación.
        /// </summary>

        private string mBaseUrl = null;
        private string mBaseUrlIdioma = null;
        /// <summary>
        /// URL base de los formularios semánticos.
        /// </summary>
        private string mBaseURLFormulariosSem;
        private string mUrlPrincipal = null;

        private string mUrlSearchProyecto = "";

        private string mEspacioPersonal = null;

        /// <summary>
        /// Nombre del ecosistema
        /// </summary>
        private static string mNombreProyectoEcosistema = null;

        /// <summary>
        /// Indica si hay que pintar la ficha de recursos de inevery o la normal.
        /// </summary>
        private static bool? mPintarFichaRecInevery;

        /// <summary>
        /// Proyecto actual.
        /// </summary>
        private Elementos.ServiciosGenerales.Proyecto mProyecto;

        /// <summary>
        /// Proyecto Virtual actual.
        /// </summary>
        private Elementos.ServiciosGenerales.Proyecto mProyectoVirtual = null;

        /// <summary>
        /// Fila del dataset de parámetros generales del proyecto actual.
        /// </summary>
        protected ParametroGeneral mParametroGeneralRow;
        protected GestorParametroGeneral mGestorParametroGeneralRow;

        protected Elementos.ServiciosGenerales.ProyectoPestanyaMenu mProyectoPestanyaActual;

        /// <summary>
        /// Fila del dataset de parámetros generales del proyecto virtual.
        /// </summary>
        protected ParametroGeneral mParametroGeneralVirtualRow;

        private Guid? mProyectoPrincipalUnico = null;

        private bool? mPerfilGlobalEnComunidadPrincipal = null;

        /// <summary>
        /// Obtiene si el ecosistema tiene una personalizacion de vistas
        /// </summary>
        private Guid? mPersonalizacionEcosistemaID = null;
        private bool? mComunidadExcluidaPersonalizacionEcosistema = null;

        /// <summary>
        /// Obtiene si se trata de un ecosistema sin metaproyecto
        /// </summary>
        private bool? mEsEcosistemaSinMetaProyecto = null;

        /// <summary>
        /// Obtiene si se trata de un ecosistema sin bandeja de suscripciones
        /// </summary>
        private bool? mEsEcosistemaSinBandejaSuscripciones = null;

        /// <summary>
        /// Obtiene si se trata de un ecosistema sin contactos
        /// </summary>
        private bool? mEsEcosistemaSinContactos = null;

        /// <summary>
        /// Obtiene si se trata de un ecosistema donde se puede editar multiple
        /// </summary>
        private bool? mEsEdicionMultiple = null;

        /// <summary>
        /// Fila de parámetros de aplicación
        /// </summary>
        //private ParametroAplicacionDS mParametrosAplicacionDS;
        private List<AD.EntityModel.ParametroAplicacion> mListaParametrosAplicacion;

        private GestorParametroAplicacion mGestorParametrosAplicacion;
        /// <summary>
        /// ID del proyecto externo en el que se está haciendo la búsqueda, o GUID.EMPTY si se hace en el proyecto actual.
        /// </summary>
        private Guid? mProyectoOrigenBusquedaID;

        /// <summary>
        /// Obtiene el proyecto externo en el que se está haciendo la búsqueda, o NULL si se hace en el proyecto actual.
        /// </summary>
        private Elementos.ServiciosGenerales.Proyecto mProyectoOrigenBusqueda;

        /// <summary>
        /// 
        /// </summary>
        private Identidad mIdentidadActual;

        /// <summary>
        /// Gestor de identidades
        /// </summary>
        private GestionIdentidades mGestorIdentidades;

        private Guid[] mIdentidadIDActual = null;

        /// <summary>
        /// Indica si se trata de la página de login en una comunidad
        /// </summary>
        private bool mEstaEnLoginComunidad = false;

        TipoCabeceraProyecto? mTipoCabecera = null;

        /// <summary>
        /// Base Static
        /// </summary>
        private string mBaseURLStatic = null;

        /// <summary>
        /// BaseURL Content
        /// </summary>
        private string mBaseURLContent = null;

        /// <summary>
        /// BaseURL Personalizacion
        /// </summary>
        private string mBaseURLPersonalizacion = null;

        /// <summary>
        /// BaseURL Personalizacion
        /// </summary>
        private string mBaseURLPersonalizacionEcosistema = null;


        /// <summary>
        /// Url del servicio de contextos
        /// </summary>
        public string mUrlServicioContextos;

        /// <summary>
        /// Devueve o establece el tipo de pagina en la que estamos
        /// </summary>
        public TiposPagina mTipoPagina;

        /// <summary>
        /// Configuración para el autoCompletar de la comunidad.
        /// </summary>
        private TagsAutoDS mConfiguracionAutoCompetarComunidad;

        private string mUrlIntragnoss = null;

        /// <summary>
        /// 
        /// </summary>
        Dictionary<string, List<string>> mInformacionOntologias;

        private string mVersion = string.Empty;

        /// <summary>
        /// Url de intragnoss de servicios.
        /// </summary>
        private string mUrlIntragnossServicios;

        /// <summary>
        /// 
        /// </summary>
        private List<TiposDocumentacion> mListaPermisosDocumentos;

        /// <summary>
        /// Contiene la lista de permisos sobre los tipos de documentos.
        /// </summary>
        private List<Guid> mListaPermisosOntologias;

        /// <summary>
        /// FilaProy con los contadores del proyecto actual.
        /// </summary>
        private AD.EntityModel.Models.ProyectoDS.Proyecto mContadoresProyecto;

        /// <summary>
        /// Nombre del proyecto padre del ecosistema configurado en BD con el parametro ComunidadPadreEcosistemaID (comunidad/nombrecorto)
        /// </summary>
        private static string mNombreProyectoPadreEcositema = null;

        /// <summary>
        /// Nombre del proyecto padre del ecosistema configurado en BD con el parametro NombreCortoProyectoPadreEcosistema (comunidad/nombrecorto)
        /// </summary>
        private static string mNombreCortoProyectoPadreEcosistema = null;

        /// <summary>
        /// ProyectoID padre del ecosistema (comunidad/nombrecorto)
        /// </summary>
        private static Guid? mPadreEcosistemaProyectoID = null;

        /// <summary>
        /// HTML personalizado para el Login.
        /// </summary>
        private string mProyectoLoginConfiguracion = string.Empty;

        private bool? mRegistroAutomaticoEcosistema = null;

        private bool? mRegistroAutomaticoEnComunidad = null;

        private DataWrapperExportacionBusqueda mExportacionBusquedaDW;

        private string mCodigoCompletoGoogleAnalytics;

        private string mScriptGoogleAnalytics;

        /// <summary>
        /// Parametros de configuración del proyecto.
        /// </summary>
        private Dictionary<string, string> mParametroProyecto;

        /// <summary>
        /// Parametros de configuración del proyecto de Ecosistema.
        /// </summary>
        private Dictionary<string, string> mParametroProyectoEcosistema;

        /// <summary>
        /// Indica si hay que subir los recursos a GoogleDrive
        /// </summary>
        private bool mTieneGoogleDriveConfigurado;

        private TipoRolUsuario? mTipoRolUsuarioEnProyecto = null;

        #endregion

        private LoggingService mLoggingService;
        private UtilPeticion mUtilPeticion;
        private EntityContext mEntityContext;
        private IHostingEnvironment mEnv;
        private IHttpContextAccessor mHttpContextAccessor;
        private ConfigService mConfigService;
        private IMemoryCache mCache;

        public DatosPeticion(UtilPeticion utilPeticion, LoggingService loggingService, EntityContext entityContext, ConfigService configService, IHttpContextAccessor httpContextAccessor, IHostingEnvironment env, IMemoryCache cache)
        {
            mUtilPeticion = utilPeticion;
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;
            mHttpContextAccessor = httpContextAccessor;
            mEnv = env;
            mCache = cache;
        }

        #region Metodos

        /// <summary>
        /// Redirige a la página solicitarCookie.aspx, le pide la cookie al ser servicio de login y de hay redirige a la página actual
        /// </summary>
        public RedirectResult SolicitarCookieLoginUsuario()
        {
            HttpCookie usuarioLogueado = HttpContext.Current.Request.Cookies["UsuarioLogueado"];

            bool solicitarCookieSiempre = false;
            //EntityContext context = EntityContext.Instance;
            //AD.EntityModel.ParametroAplicacion busqueda = context.ParametroAplicacion.Where(parametro => parametro.Parametro == "SolicitarSiempreCookieLogin").First();
            //AD.EntityModel.ParametroAplicacion busqueda = ParametrosAplicacionDS.Where(parametro => parametro.Parametro == "SolicitarSiempreCookieLogin").FirstOrDefault();
            ParametroAplicacion busqueda = GestorParametrosAplicacion.ParametroAplicacion.FirstOrDefault(parametro => parametro.Parametro == "SolicitarSiempreCookieLogin");
            //if (ParametrosAplicacionDS.ParametroAplicacion.FindByParametro("SolicitarSiempreCookieLogin") != null && ParametrosAplicacionDS.ParametroAplicacion.FindByParametro("SolicitarSiempreCookieLogin").Valor == "1")
            if (busqueda != null && busqueda.Valor == "1")
            {
                solicitarCookieSiempre = true;
            }

            //Si la página es RSS no solicita cookie
            if (((usuarioLogueado != null && usuarioLogueado.Value.Equals("1")) || solicitarCookieSiempre) && HttpContext.Current.Request.HttpMethod.Equals("GET") && (!HttpContext.Current.Request.RawUrl.EndsWith("?rss")) && !HttpContext.Current.Request.Url.ToString().Contains("anyadir-gnoss-curriculum") && HttpContext.Current.Request.Params["buscar"] == null && (ProyectoSeleccionado.Clave.Equals(ProyectoAD.MetaProyecto) || (ProyectoSeleccionado.TipoAcceso != TipoAcceso.Reservado && ProyectoSeleccionado.TipoAcceso != TipoAcceso.Privado) || ParametrosGeneralesRow.SolicitarCoockieLogin))
            {
                if ((HttpContext.Current.Session["cookieSolicitada"] == null) && (!HttpContext.Current.Request.Browser.Crawler) && (HttpContext.Current.Request.Browser.Cookies) && (!EsBot))
                {
                    string url = UtilDominios.ObtenerDominioUrl(HttpContext.Current.Request.Url, true, false);

                    if (HttpContext.Current.Request.FilePath.Equals("/anyadirGnoss"))
                    {
                        url += "/anyadirGnoss?addToGnoss=" + HttpContext.Current.Request.Params["addToGnoss"] + "&verAddTo=" + HttpContext.Current.Request.Params["verAddTo"];
                    }
                    else
                    {
                        url += HttpContext.Current.Request.RawUrl;
                    }

                    url = url.Replace("\n", "").Replace("\t", "");

                    if (url.ToLower().Contains("/prehome/"))
                    {
                        url = UtilDominios.ObtenerDominioUrl(HttpContext.Current.Request.Url, true, true);
                    }

                    HttpContext.Current.Session.Add("cookieSolicitada", DateTime.Now);

                    string urlServicioLogin = UtilUsuario.UrlServicioLogin; //Sustituir por configService

                    string query = MachineKeyCryptography.Encriptar("urlVuelta=" + BaseURL + "/&token=" + UtilUsuario.TokenLoginUsuario + "&redirect=" + HttpUtility.UrlEncode(url));

                    if (!UtilUsuario.MaximoRedireccionesExcedidas())
                    {
                        string urlRedireccion = urlServicioLogin + "/obtenerCookie.aspx?" + query;
                        //HttpContext.Current.Response.Redirect(urlRedireccion, true);

                        if (mUtilPeticion.ExisteObjetoDePeticion("AnyadirCookieSesionAResponse"))
                        {
                            UtilSesion.AgregarCookieSessionAResponse(UtilUsuario.DominoAplicacion);
                        }

                        return new RedirectResult(urlRedireccion);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Devuelve el Page.Request.Params de la pagina para que funcione con ajax
        /// </summary>
        public string RequestParams(string pParametro)
        {
            string valorParametro = null;

            if (HttpContext.Current.Request.RequestContext.RouteData.Values[pParametro] != null)
            {
                valorParametro = HttpContext.Current.Request.RequestContext.RouteData.Values[pParametro] as string;
            }
            else if (HttpContext.Current.Request[pParametro] != null)
            {
                valorParametro = HttpContext.Current.Request[pParametro];
            }

            return valorParametro;
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

                foreach (AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaBusqueda filaPest in ProyectoSeleccionado.GestorProyectos.DataWrapperProyectos.ListaProyectoPestanyaBusqueda)
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

        private Guid[] ObtenerIdentidadUsuarioEnMygnoss(Guid pPersonaID)
        {
            Guid perfilID = Guid.Empty;
            Guid identidadID = Guid.Empty;

            List<AD.EntityModel.Models.IdentidadDS.Perfil> filasPerfil = null;
            if (Usuario.UsuarioActual.PersonaID == UsuarioAD.Invitado)
            {
                perfilID = UsuarioAD.Invitado;
                identidadID = UsuarioAD.Invitado;
            }
            else if (RequestParams("nombreOrgRewrite") != null)
            {

                //Perfil Profesional    //"NombreCortoOrg='" + RequestParams("nombreOrgRewrite") + "' AND PersonaID='" + Usuario.UsuarioActual.PersonaID + "'"
                filasPerfil = IdentidadActual.GestorIdentidades.DataWrapperIdentidad.ListaPerfil.Where(perf => perf.NombreCortoOrg != null && perf.NombreCortoOrg.Equals(RequestParams("nombreOrgRewrite")) && perf.PersonaID.HasValue && perf.PersonaID.Value.Equals(Usuario.UsuarioActual.PersonaID)).ToList();

                if (filasPerfil.Count == 0)
                {
                    //Profesor
                    filasPerfil = IdentidadActual.GestorIdentidades.DataWrapperIdentidad.ListaPerfil.Where(perf => perf.NombreCortoUsu != null && perf.NombreCortoUsu.Equals(RequestParams("nombreOrgRewrite")) && perf.PersonaID.HasValue && perf.PersonaID.Value.Equals(Usuario.UsuarioActual.PersonaID)).ToList();
                }

                if (filasPerfil.Count == 0)
                {
                    //Clase del profesor:
                    filasPerfil = IdentidadActual.GestorIdentidades.DataWrapperIdentidad.ListaPerfil.Where(perf => perf.NombreCortoOrg != null && perf.NombreCortoOrg.Equals(RequestParams("nombreOrgRewrite")) && !perf.PersonaID.HasValue).ToList();
                    Guid organizacionID = filasPerfil.First().OrganizacionID.Value;

                    OrganizacionCN orgCN = new OrganizacionCN(mUtilPeticion, mEntityContext, mLoggingService, mHttpContextAccessor, mEnv, mConfigService);
                    bool usuAdminClase = orgCN.EsUsuarioAdministradorClase(organizacionID, Usuario.UsuarioActual.UsuarioID);
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
                filasPerfil = IdentidadActual.GestorIdentidades.DataWrapperIdentidad.ListaPerfil.Where(perf => !perf.OrganizacionID.HasValue && perf.PersonaID.HasValue && perf.PersonaID.Value.Equals(Usuario.UsuarioActual.PersonaID)).ToList();

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
                            HttpContext.Current.Response.Redirect(BaseURLIdioma + "/" + UtilIdiomas.GetText("URLSEM", "IDENTIDAD") + "/" + filaIdent.Perfil.NombreCortoOrg + "/home");
                        }
                    }
                }
            }

            mIdentidadActual = null;

            if (identidadID == Guid.Empty)
            {
                if (filasPerfil.Count == 0)
                {
                    //El usuario no tiene identidad personal, cargamos la primera identidad de organización que venga. 
                    filasPerfil = IdentidadActual.GestorIdentidades.DataWrapperIdentidad.ListaPerfil.Where(perf => perf.PersonaID.HasValue && perf.PersonaID.Value.Equals(Usuario.UsuarioActual.PersonaID)).ToList();
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
            }

            return new Guid[] { perfilID, identidadID };
        }

        private void EstablecerIdentidadActual(Guid pIdentidadID, Guid pPerfilID, GnossIdentity pUsuario)
        {
            bool cambiaDeIdentidad = (Usuario.UsuarioActual.IdentidadID != pIdentidadID);
            mLoggingService.AgregarEntrada("cambio de identidad: " + cambiaDeIdentidad);

            if (HttpContext.Current.Session["CookieCreada"] != null)
            {
                HttpContext.Current.Session.Remove("CookieCreada");
                if (Usuario.UsuarioActual.EsUsuarioInvitado)
                {
                    //El usuario que estaba conectado como invitado se ha logueado desde otro dominio, leo la cookie que me ha enviado desde ese dominio
                    SolicitarCookieLoginUsuario();
                }
            }

            mLoggingService.AgregarEntrada("Inicio: Establecemos la identidad del usuario");

            Usuario.UsuarioActual.IdentidadID = pIdentidadID;
            Usuario.UsuarioActual.PerfilID = pPerfilID;
            Usuario.UsuarioActual.EsIdentidadInvitada = pIdentidadID.Equals(UsuarioAD.Invitado);
            Usuario.UsuarioActual.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
            Usuario.UsuarioActual.ProyectoID = ProyectoSeleccionado.FilaProyecto.ProyectoID;

            if (Usuario.UsuarioActual.PerfilID.Equals(Guid.Empty))
            {
                Usuario.UsuarioActual.PerfilID = UsuarioAD.Invitado;
            }

            string urlActual = HttpContext.Current.Request.Url.ToString().ToLower();
            if ((cambiaDeIdentidad || Usuario.UsuarioActual.EsIdentidadInvitada) && !ListaUrlsSinIdentidad.Any(url => urlActual.EndsWith(url)))
            {
                //Actualizo la identidad con la que se conecta ahora el usuario

                if (Usuario.UsuarioActual.IdentidadID.Equals(UsuarioAD.Invitado) || Usuario.UsuarioActual.EsUsuarioInvitado)
                {
                    //Si la identidad con la que se va a conectar es la de invitado, le creo una identidad virtual al usuario
                    Usuario.UsuarioActual.EsIdentidadInvitada = true;
                    Usuario.UsuarioActual.RolPermitidoProyecto = ulong.Parse("0000000000000000");
                }
                else
                {
                    Usuario.UsuarioActual.EsIdentidadInvitada = false;
                    try
                    {
                        //Actualizo el número de conexiones en la BD
                        IdentidadCN identidadCN = new IdentidadCN(mUtilPeticion, mEntityContext, mLoggingService, mHttpContextAccessor, mEnv, mConfigService);
                        identidadCN.ActualizarNumeroConexionesProyecto(Usuario.UsuarioActual.IdentidadID);
                        //Invalido la cache de la identidad MVC
                        IdentidadCL identidadCL = new IdentidadCL(mUtilPeticion, mEntityContext, mLoggingService, mHttpContextAccessor, mEnv, mCache, mConfigService);
                        identidadCL.InvalidarFichaIdentidadMVC(Usuario.UsuarioActual.IdentidadID);
                        identidadCL.Dispose();
                    }
                    catch (Exception ex)
                    {
                        mLoggingService.GuardarLogError(ex, " ERROR: " + ex.Message + "\r\nStackTrace: " + ex.StackTrace);
                    }
                }

                LoggingService.AgregarEntrada("Fin: Establecemos la identidad del usuario IdentidadID '" + Usuario.UsuarioActual.IdentidadID + "' PerfilID '" + Usuario.UsuarioActual.PerfilID + "'");

                mUtilPeticion.AgregarObjetoAPeticionActual("GnossIdentity", pUsuario);
                HttpContext.Current.Session["Usuario"] = Usuario.UsuarioActual;

                if (!HttpContext.Current.Request.FilePath.Equals("/404.aspx"))
                {
                    UtilUsuario.ActualizarCookiePerfil(Usuario.UsuarioActual.PerfilID, Usuario.UsuarioActual.IdentidadID);
                }
            }

            if ((cambiaDeIdentidad && !Usuario.UsuarioActual.EsUsuarioInvitado) || (Usuario.UsuarioActual.EsProfesor && ProyectoSeleccionado.Clave == ProyectoAD.MetaProyecto))
            {
                UsuarioCN usuarioCN = new UsuarioCN();
                UtilUsuario.CalcularPermisosUsuario(usuarioCN.ObtenerFilaUsuarioPorLoginOEmail(Usuario.UsuarioActual.Login), Usuario.UsuarioActual, Usuario.UsuarioActual.PerfilID);
                usuarioCN.Dispose();
            }

            if (Usuario.UsuarioActual != null && !Usuario.UsuarioActual.EsUsuarioInvitado && IdentidadActual.OrganizacionID.HasValue && !Usuario.UsuarioActual.EstaAutorizadoEnOrganizacion((ulong)Capacidad.Organizacion.CapacidadesAdministrador.AdministrarOrganizacion, IdentidadActual.OrganizacionID.Value))
            {
                // Recuperamos los permisos en las organizaciones de caché
                // Por si nos han dado permisos en la organización del perfil con el que estoy conectado

                IdentidadCL identidadCL = new IdentidadCL(mUtilPeticion, mEntityContext, mLoggingService, mHttpContextAccessor, mEnv, mCache, mConfigService);
                bool esAdministrador = (identidadCL.ObtenerPermisosUsuarioEnOrg(Usuario.UsuarioActual.UsuarioID, IdentidadActual.OrganizacionID.Value) == (short)TipoAdministradoresOrganizacion.Administrador);
                identidadCL.Dispose();

                if (esAdministrador)
                {
                    UsuarioCN usuarioCN = new UsuarioCN(mUtilPeticion, mEntityContext, mLoggingService, mHttpContextAccessor, mEnv, mConfigService);
                    UtilUsuario.CalcularPermisosUsuario(usuarioCN.ObtenerFilaUsuarioPorLoginOEmail(Usuario.UsuarioActual.Login), Usuario.UsuarioActual, IdentidadActual.PerfilID);
                    usuarioCN.Dispose();
                }
            }
        }

        /// <summary>
        /// Recupera la varieble global Usuario.UsuarioActual a partir de la sesión
        /// </summary>
        public void RecuperarUsuarioDeSesion()
        {
            if (HttpContext.Current.Session["Usuario"] == null)
            {
                CrearUsuarioInvitado();
            }
            else
            {
                GnossIdentity usuario = (GnossIdentity)HttpContext.Current.Session["Usuario"];

                mUtilPeticion.AgregarObjetoAPeticionActual("GnossIdentity", usuario);

                // Calculo IdentidadID

                Guid id = Guid.Empty;
                Guid perfilID = Guid.Empty;

                IdentidadCN identidadCN = new IdentidadCN(mUtilPeticion, mEntityContext, mLoggingService, mHttpContextAccessor, mEnv, mConfigService);

                if (ProyectoSeleccionado == null)
                {
                    return;
                }

                if (ProyectoSeleccionado.Clave == ProyectoAD.MetaProyecto)
                {
                    Guid[] identidadEnMygnoss = ObtenerIdentidadUsuarioEnMygnoss();

                    perfilID = identidadEnMygnoss[0];
                    id = identidadEnMygnoss[1];
                }
                else
                {
                    Guid[] identPerfil = null;

                    if (Usuario.UsuarioActual.PersonaID != UsuarioAD.Invitado)
                    {
                        string sesionIdentidadID = string.Format("IdentidadIDdePersonaEnProyecto_{0}_{1}", ProyectoSeleccionado.Clave, Usuario.UsuarioActual.PersonaID);
                        if (HttpContext.Current.Session[sesionIdentidadID] != null)
                        {
                            identPerfil = (Guid[])HttpContext.Current.Session[sesionIdentidadID];
                        }
                        else
                        {
                            identPerfil = identidadCN.ObtenerIdentidadIDDePersonaEnProyecto(ProyectoSeleccionado.Clave, Usuario.UsuarioActual.PersonaID);
                            HttpContext.Current.Session.Add(sesionIdentidadID, identPerfil);
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

                        if (Usuario.UsuarioActual.PersonaID.Equals(UsuarioAD.Invitado))
                        {
                            perfilID = UsuarioAD.Invitado;
                        }
                    }
                }

                if (id != Guid.Empty)
                {
                    EstablecerIdentidadActual(id, perfilID, usuario);
                }

                identidadCN.Dispose();

                if (Usuario.UsuarioActual == null)
                {
                    //Creo el usuario actual como invitado
                    UtilUsuario.CrearUsuarioInvitado(UtilIdiomas);
                }
                else if (!Usuario.UsuarioActual.EsUsuarioInvitado)
                {
                    AD.EntityModel.ParametroAplicacion busqueda = ListaParametrosAplicacion.FirstOrDefault(parametro => parametro.Parametro == TiposParametrosAplicacion.LoginUnicoPorUsuario);
                    //bool loginUnicoPorUsuario = busqueda!=null && busqueda.Valor.Equals("1");
                    bool loginUnicoPorUsuario = ListaParametrosAplicacion.Where(parametro => parametro.Parametro == TiposParametrosAplicacion.LoginUnicoPorUsuario).FirstOrDefault() != null && ListaParametrosAplicacion.Where(parametro => parametro.Parametro == TiposParametrosAplicacion.LoginUnicoPorUsuario).FirstOrDefault().Valor.Equals("1");
                    //Crear otro parametro para permitir la desconexión.
                    bool desconexionUsuarios = ListaParametrosAplicacion.Where(parametro => parametro.Parametro == TiposParametrosAplicacion.DesconexionUsuarios).FirstOrDefault() != null && ListaParametrosAplicacion.Where(parametro => parametro.Parametro == TiposParametrosAplicacion.DesconexionUsuarios).FirstOrDefault().Valor.Equals("1");
                    //Devolver a desconectar con un parametro que diga que se ha desconectado.

                    if (loginUnicoPorUsuario && desconexionUsuarios)
                    {
                        List<string> listaSesiones = GnossCache.ObtenerDeCache($"{SESION_UNICA_POR_USUARIO}_List_{Usuario.UsuarioActual.UsuarioID}") as List<string>;
                        if (listaSesiones == null)
                        {
                            listaSesiones = new List<string>();
                        }
                        if (listaSesiones.Contains(HttpContext.Current.Session.SessionID))
                        {
                            GnossCache.InvalidarDeCache($"{DatosPeticion.SESION_UNICA_POR_USUARIO}_List_{Usuario.UsuarioActual.UsuarioID}", true);
                        }
                    }
                }
            }
        }

        public string ObtenerIP()
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    return addresses[0];
                }
            }

            return context.Request.ServerVariables["REMOTE_ADDR"];
        }

        public int ObtenerNumIntentosIP(string IP)
        {
            SeguridadCL seguridadCL = new SeguridadCL(mUtilPeticion, mEntityContext, mLoggingService, mHttpContextAccessor, mEnv, mCache, mConfigService);
            return seguridadCL.ObtenerNumIntentosIP(IP);
        }

        /// <summary>
        /// Carga todo lo necesario para que se conecte el usuario invitado y agrega a la sesión el usuario invitado
        /// </summary>
        public void CrearUsuarioInvitado()
        {
            //Creo el usuario actual como invitado
            UtilUsuario.CrearUsuarioInvitado(UtilIdiomas);

            //Lo agrego a la sesión
            HttpContext.Current.Session.Add("Usuario", Usuario.UsuarioActual);
            HttpContext.Current.Session.Add("MantenerConectado", false);
        }

        /// <summary>
        /// Guarda el log de error
        /// </summary>
        /// <param name="pError">Cadena de texto con el error</param>
        public void GuardarLogError(string pError, string pNombreFichero)
        {
            string applicationPath = "";

            if (HttpContext.Current != null && HttpContext.Current.Request != null)
            {
                applicationPath = HttpContext.Current.Request.ApplicationPath;
            }

            string directorio = HostingEnvironment.MapPath(applicationPath + "/logs");
            string fichero = directorio + "\\" + pNombreFichero + "_" + DateTime.Now.ToString("yyyy-MM-dd") + ".log";

            if (!Directory.Exists(directorio))
            {
                Directory.CreateDirectory(directorio);
            }

            UtilUsuario.EscribirEnFichero(fichero, pError);
        }

        /// <summary>
        /// Guarda el log de error
        /// </summary>
        /// <param name="pError">Cadena de texto con el error</param>
        public void GuardarLogError(string pError)
        {
            string error = mLoggingService.DevolverCadenaError(null, "1.0.0.0") + Environment.NewLine + Environment.NewLine + pError;
            GuardarLogError(error, "error");
        }

        /// <summary>
        /// Guarda el log de error
        /// </summary>
        /// <param name="pExcepcion">Excepcion que ha producido el error</param>
        public void GuardarLogError(Exception pExcepcion)
        {
            GuardarLogError(mLoggingService.DevolverCadenaError(pExcepcion, "1.0.0.0"), "error");
        }

        /// <summary>
        /// Guarda el log de error
        /// </summary>
        /// <param name="pExcepcion">Excepción que a producido el error</param>
        /// <param name="pMensajeExtra">Mensaje extra para guardar en el log</param>
        public void GuardarLogError(Exception pExcepcion, string pMensajeExtra)
        {
            GuardarLogError(mLoggingService.DevolverCadenaError(pExcepcion, "1.0.0.0") + "\r\nInfo Extra: " + pMensajeExtra, "error");
        }

        /// <summary>
        /// Guarda el log de error
        /// </summary>
        /// <param name="pError">Cadena de texto con el error</param>
        public void GuardarLogErrorContextos(string pError)
        {
            string error = mLoggingService.DevolverCadenaError(null, "1.0.0.0") + Environment.NewLine + Environment.NewLine + pError;
            GuardarLogError(error, "errorContexto");
        }

        /// <summary>
        /// Guarda el log de error
        /// </summary>
        /// <param name="pError">Cadena de texto con el error</param>
        public void GuardarLogErrorAJAX(string pError)
        {
            string error = mLoggingService.DevolverCadenaError(null, "1.0.0.0") + Environment.NewLine + Environment.NewLine + pError;
            GuardarLogError(error, "errorAJAX");
        }

        /// <summary>
        /// Guarda el log de error
        /// </summary>
        /// <param name="pExcepcion">Excepcion que ha producido el error</param>
        public void GuardarLogErrorAJAX(Exception pExcepcion)
        {
            GuardarLogError(mLoggingService.DevolverCadenaError(pExcepcion, "1.0.0.0"), "errorAJAX");
        }

        /// <summary>
        /// Guarda el log de error
        /// </summary>
        /// <param name="pExcepcion">Excepción que a producido el error</param>
        /// <param name="pMensajeExtra">Mensaje extra para guardar en el log</param>
        public void GuardarLogErrorAJAX(Exception pExcepcion, string pMensajeExtra)
        {
            GuardarLogError(mLoggingService.DevolverCadenaError(pExcepcion, "1.0.0.0") + "\r\nInfo Extra: " + pMensajeExtra, "error");
        }

        /// <summary>
        /// Contiene la lista de permisos sobre los tipos de documentos de un proyecto.
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        public List<TiposDocumentacion> ListaPermisosDocumentosDeProyecto(Guid pProyectoID)
        {
            if (mListaPermisosDocumentos == null)
            {
                ProyectoCL proyCL = new ProyectoCL(mUtilPeticion, mEntityContext, mLoggingService, mHttpContextAccessor, mEnv, mCache, mConfigService);
                mListaPermisosDocumentos = proyCL.ObtenerTiposDocumentosPermitidosUsuarioEnProyecto(pProyectoID, TipoRolUsuarioEnProyecto);
                proyCL.Dispose();
            }

            return mListaPermisosDocumentos;
        }

        private static ConcurrentDictionary<Guid, ConcurrentBag<Guid>> mListaOntologiasPermitidasPorIdentidad = new ConcurrentDictionary<Guid, ConcurrentBag<Guid>>();

        /// <summary>
        /// Contiene la lista de permisos sobre los tipos de documentos de un proyecto.
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <param name="pDocumentoID">Identificador del documento ID para el que se desea comprobar el acceso</param>
        public bool ComprobarPermisoEnOntologiaDeProyectoEIdentidad(Guid pProyectoID, Guid pDocumentoID, bool pIdentidadDeOtroProyecto = true)
        {
            bool tieneAcceso = true;
            if (!mListaOntologiasPermitidasPorIdentidad.ContainsKey(IdentidadActual.Clave) || !mListaOntologiasPermitidasPorIdentidad[IdentidadActual.Clave].Contains(pDocumentoID))
            {
                if (!mListaOntologiasPermitidasPorIdentidad.ContainsKey(IdentidadActual.Clave))
                {
                    mListaOntologiasPermitidasPorIdentidad.TryAdd(IdentidadActual.Clave, new ConcurrentBag<Guid>());
                }

                ProyectoCN proyCN = new ProyectoCN(mUtilPeticion, mEntityContext, mLoggingService, mHttpContextAccessor, mEnv, mConfigService);
                tieneAcceso = proyCN.ComprobarOntologiasPermitidaParaIdentidadEnProyecto(IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave, pProyectoID, TipoRolUsuarioEnProyecto, pIdentidadDeOtroProyecto, pDocumentoID);

                if (tieneAcceso)
                {
                    mListaOntologiasPermitidasPorIdentidad[IdentidadActual.Clave].Add(pDocumentoID);
                }
            }

            return tieneAcceso;
        }

        /// <summary>
        /// Anula la identidad actual el gestor de identidades cargado en ella
        /// </summary>
        public void ResetearIdentidadActual()
        {
            mIdentidadActual = null;
            mGestorIdentidades = null;
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Devuelve la URL del servicio de documentación.
        /// </summary>
        public string UrlServicioWebDocumentacion
        {
            get
            {
                return Conexion.ObtenerUrlServicioDocumental();
            }
        }

        public DatosPeticion Current
        {
            get
            {
                DatosPeticion datosPeticionActual = (DatosPeticion)mUtilPeticion.ObtenerObjetoDePeticion("DatosPeticionActual");
                if (datosPeticionActual == null)
                {
                    datosPeticionActual = new DatosPeticion(mUtilPeticion, mLoggingService, mEntityContext, mConfigService, mHttpContextAccessor, mEnv, mCache);
                    mUtilPeticion.AgregarObjetoAPeticionActual("DatosPeticionActual", datosPeticionActual);
                }
                return datosPeticionActual;
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
                    if (string.IsNullOrEmpty(HttpContext.Current.Request.UserAgent) || HttpContext.Current.Request.UserAgent.Equals("GnossBotChequeoCache"))
                    {
                        return true;
                    }
                    else if (HttpContext.Current.Request.ServerVariables["HTTP_X_GOOG_SOURCE"] != null)
                    {
                        //google plus
                        return true;
                    }
                    else
                    {
                        if (GnossWebControl.ListaTodosBots == null)
                        {
                            GnossWebControl.LeerConfigBots(BaseURLSinHTTP);
                        }

                        if (GnossWebControl.ListaBotsCompletos != null && GnossWebControl.ListaBotsCompletos.Contains(HttpContext.Current.Request.UserAgent))
                        {
                            return true;
                        }
                        else if (!string.IsNullOrEmpty(GnossWebControl.ListaTodosBots))
                        {
                            return Regex.IsMatch(HttpContext.Current.Request.UserAgent, GnossWebControl.ListaTodosBots);
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

        public UtilIdiomas UtilIdiomas
        {
            get
            {
                if (mUtilIdiomas == null)
                {
                    string[] array = { "es" };

                    if (ProyectoVirtual != null)
                    {
                        mUtilIdiomas = new UtilIdiomas(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + Path.DirectorySeparatorChar + "languages", array, IdiomaUsuario, ProyectoVirtual.Clave, ProyectoVirtual.PersonalizacionID, PersonalizacionEcosistemaID);
                    }
                    else
                    {
                        mUtilIdiomas = new UtilIdiomas(IdiomaUsuario, mUtilPeticion, mLoggingService, mEntityContext, mConfigService, mHttpContextAccessor, mEnv);
                    }
                }
                return mUtilIdiomas;
            }
            set
            {
                mUtilIdiomas = value;
            }
        }

        /// <summary>
        /// Obtiene la dirección base
        /// </summary>
        public string BaseURLSinHTTP
        {
            get
            {
                string url = HttpContext.Current.Request.ApplicationPath.Trim();

                if (url.Length == 1)
                {
                    url = "";
                }
                return url;
            }
        }

        /// <summary>
        /// Obtiene la URL base de la página
        /// </summary>
        public string BaseURL
        {
            get
            {
                if (string.IsNullOrEmpty(mBaseUrl))
                {
                    string cadenaPuerto = "";
                    //int puerto = HttpContext.Current.Request.Url.Port;

                    //if (puerto != 80 && puerto != 443)
                    //{
                    //    cadenaPuerto = ":" + puerto.ToString();
                    //}

                    string host = HttpContext.Current.Request.Url.Host;
                    host = UtilDominios.DomainToPunyCode(host);

                    mBaseUrl = UtilUsuario.HTTP + host + cadenaPuerto + BaseURLSinHTTP;
                }
                return mBaseUrl;
            }
        }

        /// <summary>
        /// Obtiene la URL base de la página en el idioma correspondiente
        /// </summary>
        public string BaseURLIdioma
        {
            get
            {
                if (string.IsNullOrEmpty(mBaseUrlIdioma))
                {
                    string idioma = "";
                    if (UtilIdiomas.LanguageCode != GnossUrlsSemanticas.IdiomaPrincipalDominio)
                    {
                        idioma = "/" + UtilIdiomas.LanguageCode;
                    }

                    string url = BaseURL;
                    if (EsDominioComunidades || !ProyectoConexionID.Equals(ProyectoAD.MetaProyecto))
                    {
                        url = UrlPrincipal;
                    }

                    mBaseUrlIdioma = url + idioma;
                }
                return mBaseUrlIdioma;
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
                        mUrlPrincipal = Conexion.ObtenerUrlBase();
                    }
                    catch (Exception ex)
                    {
                        mLoggingService.GuardarLogError(ex);
                    }

                    if (string.IsNullOrEmpty(mUrlPrincipal))
                    {
                        mUrlPrincipal = BaseURL;
                    }
                }
                return mUrlPrincipal;
            }
        }

        /// <summary>
        /// Obtiene si el dominio actual es el dominio de las comunidades por defecto (http://comunidades.gnoss.com)
        /// </summary>
        public bool EsDominioComunidades
        {
            get
            {
                string sitioComPorDefecto = Conexion.ObtenerParametro("config/project.config", "ProyectoConexion/SitioComunidadesPorDefecto", false);
                return (!string.IsNullOrEmpty(sitioComPorDefecto) && (sitioComPorDefecto.Equals("1")));
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


        public string IdiomaPorDefecto
        {
            get
            {
                return !(Current.ParametrosGeneralesRow.IdiomaDefecto == null) ? Current.ParametrosGeneralesRow.IdiomaDefecto : Conexion.ObtenerListaIdiomas().Keys.First();
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
                    mUrlPresentacion = Conexion.ObtenerParametro("config/project.config", "config/PaginaPresentacion", false);

                    if (mUrlPresentacion == null)
                    {
                        mUrlPresentacion = "";
                    }
                }

                return mUrlPresentacion;
            }
        }
        Guid PerfilCookieID { get; set; }
        /// <summary>
        /// Obtiene el proyecto al que se conecta siempre la aplicación
        /// </summary>
        public Guid ProyectoConexionID
        {
            get
            {
                if (mProyectoConexionID == null)
                {
                    mProyectoConexionID = Conexion.ObtenerProyectoConexion();
                    if (!mProyectoConexionID.HasValue)
                    {
                        mProyectoConexionID = ProyectoAD.MetaProyecto;
                    }
                }
                return mProyectoConexionID.Value;
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
                        ProyectoCN proyectoCN = new ProyectoCN(mUtilPeticion, mEntityContext, mLoggingService, mHttpContextAccessor, mEnv, mConfigService);
                        proyectoCN.UsarMasterParaLectura = true;
                        mNombreCortoProyectoConexion = proyectoCN.ObtenerNombreCortoProyecto(ProyectoConexionID);
                        proyectoCN.Dispose();
                    }
                }

                return mNombreCortoProyectoConexion;
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
                    ProyectoCN proyectoCN = new ProyectoCN(mUtilPeticion, mEntityContext, mLoggingService, mHttpContextAccessor, mEnv, mConfigService);
                    proyectoCN.UsarMasterParaLectura = true;
                    mNombreCortoProyectoPrincipal = proyectoCN.ObtenerNombreCortoProyecto(ProyectoPrincipalUnico);
                    proyectoCN.Dispose();
                }

                return mNombreCortoProyectoPrincipal;
            }
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

        private Guid? mBaseRecursosProyectoSeleccionado;
        public Guid BaseRecursosProyectoSeleccionado
        {
            get
            {
                if (!mBaseRecursosProyectoSeleccionado.HasValue)
                {
                    if (ProyectoSeleccionado != null)
                    {
                        DocumentacionCN docCN = new DocumentacionCN(mUtilPeticion, mEntityContext, mLoggingService, mHttpContextAccessor, mEnv, mConfigService);
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
        /// Obtiene el proyecto seleccionado
        /// </summary>
        public Elementos.ServiciosGenerales.Proyecto ProyectoSeleccionado
        {
            get
            {
                //Fernando Expresion regular : Request\["([^"\]]*)"\]
                string nombreCortoProyecto = null;
                Guid proyectoID = Guid.Empty;

                if (ProyectoSeleccionadoError404 != null && !ProyectoSeleccionadoError404.Equals(Guid.Empty))
                {
                    proyectoID = ProyectoSeleccionadoError404;
                }
                else if (RequestParams("nombreProy") != null)
                {
                    nombreCortoProyecto = RequestParams("nombreProy");
                }
                else if (RequestParams("NombreCortoComunidad") != null)
                {
                    nombreCortoProyecto = RequestParams("NombreCortoComunidad");
                }
                else if (RequestParams("proyectoID") != null)
                {
                    proyectoID = new Guid(RequestParams("proyectoID"));
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
                    ProyectoCL proyectoCL = new ProyectoCL(mUtilPeticion, mEntityContext, mLoggingService, mHttpContextAccessor, mEnv, mCache, mConfigService);

                    if (!string.IsNullOrEmpty(nombreCortoProyecto))
                    {
                        proyectoID = proyectoCL.ObtenerProyectoIDPorNombreCorto(nombreCortoProyecto);
                    }

                    GestionProyecto gestorProyecto = new GestionProyecto(proyectoCL.ObtenerProyectoPorID(proyectoID), mLoggingService, mEntityContext);

                    if (gestorProyecto.ListaProyectos.Count > 0 && gestorProyecto.ListaProyectos.ContainsKey(proyectoID))
                    {
                        mProyecto = gestorProyecto.ListaProyectos[proyectoID];
                    }

                    if (mProyecto == null)
                    {
                        return null;
                    }
                    if (!mProyecto.Clave.Equals(ProyectoAD.MetaProyecto))
                    {
                        if (Usuario.UsuarioActual != null && !Usuario.UsuarioActual.EsUsuarioInvitado)
                        {
                            proyectoCL.AgregarUltimoProyectoUsuario(proyectoID, Usuario.UsuarioActual.UsuarioID, mProyecto.UrlPropia);
                        }
                    }

                    if (HttpContext.Current != null && HttpContext.Current.Session != null && HttpContext.Current.Session["Usuario"] != null && !((GnossIdentity)HttpContext.Current.Session["Usuario"]).EsIdentidadInvitada && mProyecto.ListaAdministradoresIDs.Contains(((GnossIdentity)HttpContext.Current.Session["Usuario"]).UsuarioID))
                    {
                        CrearPestanyaPersonasYOrganizaciones(mProyecto.GestorProyectos.DataWrapperProyectos, mProyecto.Clave);
                    }
                }
                return mProyecto;
            }
        }

        /// <summary>
        /// Carga pestañas que son exclusivas del administrador
        /// </summary>
        public static void CrearPestanyaPersonasYOrganizaciones(DataWrapperProyecto pDataWrapperProyecto, Guid pProyectoID)
        {
            if (pDataWrapperProyecto != null && pDataWrapperProyecto.ListaProyectoPestanyaMenu.Count(pestanya => pestanya.TipoPestanya.Equals((short)TipoPestanyaMenu.PersonasYOrganizaciones)) == 0)
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
                        ////////TODO ?  Eliminar de Context¿?
                        ProyectoSeleccionado.GestorProyectos.DataWrapperProyectos.ListaProyectoPestanyaMenu.Remove(ProyectoSeleccionado.GestorProyectos.DataWrapperProyectos.ListaProyectoPestanyaMenu.Find(proyPestanyaMenu => proyPestanyaMenu.PestanyaID.Equals(Guid.Empty)));
                    }

                    if (RequestParams("PestanyaID") != null)
                    {
                        mProyectoPestanyaActual = ProyectoSeleccionado.ListaPestanyasMenu[new Guid(RequestParams("PestanyaID"))];
                    }
                    else if (RequestParams("Controller") != null)
                    {
                        string controller = RequestParams("Controller");
                        switch (controller)
                        {
                            case "HomeComunidad":
                                foreach (Elementos.ServiciosGenerales.ProyectoPestanyaMenu pestanya in ProyectoSeleccionado.ListaPestanyasMenu.Values)
                                {
                                    if (pestanya.TipoPestanya == TipoPestanyaMenu.Home && string.IsNullOrEmpty(pestanya.Ruta))
                                    {
                                        mProyectoPestanyaActual = pestanya;
                                        break;
                                    }
                                }
                                break;
                            case "Indice":
                                foreach (Elementos.ServiciosGenerales.ProyectoPestanyaMenu pestanya in ProyectoSeleccionado.ListaPestanyasMenu.Values)
                                {
                                    if (pestanya.TipoPestanya == TipoPestanyaMenu.Indice && string.IsNullOrEmpty(pestanya.Ruta))
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

            //if (pestanyas.Count() > 0)
            //    return pestanyas.First();
            //else
            //    return null;

            return pestanya;
        }

        /// <summary>
        /// Obtiene el dataset de parámetros generales
        /// </summary>
        //public ParametroGeneral ParametrosGeneralesRow
        public GestorParametroGeneral GestorParametrosGeneralesRow
        {
            get
            {
                if (mGestorParametroGeneralRow == null || mGestorParametroGeneralRow.ListaParametroGeneral.FirstOrDefault().ProyectoID != ProyectoVirtual.Clave)
                {
                    mGestorParametroGeneralRow = new GestorParametroGeneral();
                    Guid proyectoid = ProyectoAD.MetaProyecto;
                    if (ProyectoVirtual != null)
                    {
                        proyectoid = ProyectoVirtual.Clave;
                    }

                    mGestorParametroGeneralRow = UtilUsuario.ObtenerFilaParametrosGeneralesDeProyecto(proyectoid);

                    if (PersonalizacionEcosistemaID != Guid.Empty && !ComunidadExcluidaPersonalizacionEcosistema)
                    {
                        //ParametroGeneralDS textosPersonalizadosPersonalizacionDS = UtilUsuario.ObtenerTextosPersonalizadosPersonalizacionEcosistema(PersonalizacionEcosistemaID);
                        List<TextosPersonalizadosPersonalizacion> listaTextosPersonalizados = UtilUsuario.ObtenerTextosPersonalizadosPersonalizacionEcosistema(PersonalizacionEcosistemaID).ListaTextosPersonalizadosPersonalizacion;

                        // ((ParametroGeneralDS)mGestorParametroGeneralRow.Table.DataSet).Merge(textosPersonalizadosPersonalizacionDS);
                        foreach (TextosPersonalizadosPersonalizacion textosPersonalizados in listaTextosPersonalizados)
                        {
                            mGestorParametroGeneralRow.ListaTextosPersonalizadosPersonalizacion.Add(textosPersonalizados);
                        }
                    }
                }
                return mGestorParametroGeneralRow;
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
                    }

                    mParametroGeneralRow = UtilUsuario.ObtenerFilaParametrosGeneralesDeProyecto(proyectoid).ListaParametroGeneral.FirstOrDefault();

                    //if (PersonalizacionEcosistemaID != Guid.Empty && !ComunidadExcluidaPersonalizacionEcosistema)
                    //{
                    //    ParametroGeneralDS textosPersonalizadosPersonalizacionDS = UtilUsuario.ObtenerTextosPersonalizadosPersonalizacionEcosistema(PersonalizacionEcosistemaID);
                    //    ((ParametroGeneralDS)mParametroGeneralRow.Table.DataSet).Merge(textosPersonalizadosPersonalizacionDS);
                    //}
                }
                return mParametroGeneralRow;
            }
        }

        /// <summary>
        /// Obtiene el dataset de parámetros generales
        /// </summary>
        //public ParametroGeneralDS ParametrosGeneralesDS
        public GestorParametroGeneral ParametrosGeneralesDS
        {
            get
            {
                return GestorParametrosGeneralesRow;
            }
        }

        private bool mCargandoProyectoVirtual = false;

        /// <summary>
        /// Obtiene el proyecto virtual (Para cuando es un ecosistema sin metaproyecto)
        /// </summary>
        public Elementos.ServiciosGenerales.Proyecto ProyectoVirtual
        {
            get
            {
                if (mProyectoVirtual == null && !mCargandoProyectoVirtual)
                {
                    mCargandoProyectoVirtual = true;
                    Guid proyectoVirtualID = Guid.Empty;
                    ProyectoCL proyCL = new ProyectoCL(mUtilPeticion, mEntityContext, mLoggingService, mHttpContextAccessor, mEnv, mCache, mConfigService);

                    if (EsEcosistemaSinMetaProyecto && ProyectoSeleccionado != null && ProyectoSeleccionado.Clave.Equals(ProyectoAD.MetaProyecto))
                    {
                        proyectoVirtualID = ProyectoPrincipalUnico;

                        if (proyectoVirtualID == ProyectoAD.MetaProyecto)
                        {
                            proyectoVirtualID = UtilUsuario.ProyectoPrincipal;

                            if (Usuario.UsuarioActual != null && !Usuario.UsuarioActual.EsUsuarioInvitado)
                            {
                                proyectoVirtualID = proyCL.ObtenerUltimoProyectoUsuario(Usuario.UsuarioActual.UsuarioID, BaseURL);

                                if (proyectoVirtualID.Equals(Guid.Empty))
                                {
                                    ProyectoCN proyCN = new ProyectoCN(mUtilPeticion, mEntityContext, mLoggingService, mHttpContextAccessor, mEnv, mConfigService);
                                    proyectoVirtualID = proyCN.ObtenerProyectoIDMasActivoPerfil(IdentidadActual.PerfilID);
                                    proyCN.Dispose();

                                    if (!proyectoVirtualID.Equals(Guid.Empty))
                                    {
                                        proyCL.AgregarUltimoProyectoUsuario(proyectoVirtualID, Usuario.UsuarioActual.UsuarioID, BaseURL);
                                    }
                                }
                            }
                        }
                    }

                    if (!proyectoVirtualID.Equals(Guid.Empty))
                    {
                        GestionProyecto gestProy = new GestionProyecto(proyCL.ObtenerProyectoPorID(proyectoVirtualID), mLoggingService, mEntityContext);
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
        /// Obtiene el proyecto principal de un ecosistema sin metaproyecto
        /// </summary>
        public Guid ProyectoPrincipalUnico
        {
            get
            {
                if (mProyectoPrincipalUnico == null)
                {
                    mProyectoPrincipalUnico = ProyectoAD.MetaProyecto;
                    //EntityContext context = EntityContext.Instance;
                    // List<AD.EntityModel.ParametroAplicacion> busqueda = context.ParametroAplicacion.Where(parametro => parametro.Parametro.Equals(TiposParametrosAplicacion.ComunidadPrincipalID)).ToList();
                    AD.EntityModel.ParametroAplicacion busqueda = ListaParametrosAplicacion.FirstOrDefault(parametro => parametro.Parametro.Equals(TiposParametrosAplicacion.ComunidadPrincipalID));
                    //if (ParametrosAplicacionDS.ParametroAplicacion.Select("Parametro = '" + TiposParametrosAplicacion.ComunidadPrincipalID.ToString() + "'").Length > 0)
                    if (busqueda != null)
                    {
                        mProyectoPrincipalUnico = new Guid(busqueda.Valor);
                    }
                }
                return mProyectoPrincipalUnico.Value;
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

        /// <summary>
        /// Obtiene si se trata de un ecosistema sin metaproyecto
        /// </summary>
        public bool EsEcosistemaSinMetaProyecto
        {
            get
            {
                if (!mEsEcosistemaSinMetaProyecto.HasValue)
                {
                    // EntityContext context = EntityContext.Instance;
                    // List<AD.EntityModel.ParametroAplicacion> busqueda = context.ParametroAplicacion.Where(parametro => parametro.Parametro.Equals(TiposParametrosAplicacion.EcosistemaSinMetaProyecto)).ToList();
                    ParametroAplicacion busqueda = GestorParametrosAplicacion.ParametroAplicacion.FirstOrDefault(parametro => parametro.Parametro.Equals(TiposParametrosAplicacion.EcosistemaSinMetaProyecto));
                    mEsEcosistemaSinMetaProyecto = busqueda != null && bool.Parse(busqueda.Valor);
                    // mEsEcosistemaSinMetaProyecto = ParametrosAplicacionDS.ParametroAplicacion.Select("Parametro = '" + TiposParametrosAplicacion.EcosistemaSinMetaProyecto.ToString() + "'").Length > 0 && bool.Parse((string)ParametrosAplicacionDS.ParametroAplicacion.Select("Parametro = '" + TiposParametrosAplicacion.EcosistemaSinMetaProyecto.ToString() + "'")[0]["Valor"]);
                }
                return mEsEcosistemaSinMetaProyecto.Value;
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
        /// Obtiene el dataset de parámetros de aplicación
        /// </summary>
        public List<AD.EntityModel.ParametroAplicacion> ListaParametrosAplicacion
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

        public GestorParametroAplicacion GestorParametrosAplicacion
        {
            get
            {
                if (mGestorParametrosAplicacion == null)
                {
                    ParametroAplicacionCL paramCL = new ParametroAplicacionCL(mUtilPeticion, mEntityContext, mLoggingService, mHttpContextAccessor, mEnv, mCache, mConfigService);

                    mGestorParametrosAplicacion = paramCL.ObtenerGestorParametros();
                }
                return mGestorParametrosAplicacion;
            }
        }

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
                    List<ProyectoPestanyaBusqueda> filasPestañaBusqueda = ProyectoSeleccionado.GestorProyectos.DataWrapperProyectos.ListaProyectoPestanyaBusqueda.Where(proyectoPestanyaBusqueda => proyectoPestanyaBusqueda.ProyectoOrigenID != null).ToList();
                    if (filasPestañaBusqueda.Count > 0)
                    {
                        mProyectoOrigenBusquedaID = filasPestañaBusqueda[0].ProyectoOrigenID;
                    }
                    else
                    {
                        mProyectoOrigenBusquedaID = Guid.Empty;
                    }
                }

                return mProyectoOrigenBusquedaID.Value;
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
                    ProyectoCL proyectoCL = new ProyectoCL(mUtilPeticion, mEntityContext, mLoggingService, mHttpContextAccessor, mEnv, mCache, mConfigService);
                    GestionProyecto gestorProyecto = new GestionProyecto(proyectoCL.ObtenerProyectoPorID(ProyectoOrigenBusquedaID), mLoggingService, mEntityContext);
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
        /// Obtiene la identidad del usuario actual
        /// </summary>
        public Identidad IdentidadActual
        {
            get
            {
                if (Usuario.UsuarioActual == null)
                {
                    RecuperarUsuarioDeSesion();
                }

                if ((Usuario.UsuarioActual != null) && ((mIdentidadActual == null) || (mIdentidadActual != null && mIdentidadActual.FilaIdentidad == null)))
                {
                    if (mGestorIdentidades == null)
                    {
                        mGestorIdentidades = UtilUsuario.CargarGestorIdentidadesUsuarioActual(UtilIdiomas);
                    }
                    if ((!mGestorIdentidades.ListaIdentidades.ContainsKey(Usuario.UsuarioActual.IdentidadID)) && (!Usuario.UsuarioActual.IdentidadID.Equals(UsuarioAD.Invitado)))
                    {
                        //El gestor cargado de caché no contiene la identidad actual, la vuelvo a cargar (seguramente se acabe de hacer miembro de una comunidad)

                        IdentidadCL identidadCL = new IdentidadCL(mUtilPeticion, mEntityContext, mLoggingService, mHttpContextAccessor, mEnv, mCache, mConfigService);
                        identidadCL.EliminarCacheGestorIdentidadActual();

                        mGestorIdentidades = UtilUsuario.CargarGestorIdentidadesUsuarioActual(UtilIdiomas);
                    }

                    PerfilCookieID = Usuario.UsuarioActual.PerfilID;
                    if (IdentidadIDActual != null && mGestorIdentidades.ListaPerfiles.ContainsKey(IdentidadIDActual[0]))
                    {
                        PerfilCookieID = IdentidadIDActual[0];
                    }

                    if (PerfilCookieID.Equals(UsuarioAD.Invitado) && !Usuario.UsuarioActual.EsUsuarioInvitado && mGestorIdentidades.ListaPerfiles.Values.Any(perfil => perfil.PersonaID.Equals(Usuario.UsuarioActual.PersonaID)))
                    {
                        if (mGestorIdentidades.ListaPerfiles.Values.Any(perfil => perfil.PersonaID.Equals(Usuario.UsuarioActual.PersonaID) && !perfil.OrganizacionID.HasValue))
                        {
                            PerfilCookieID = mGestorIdentidades.ListaPerfiles.Values.First(perfil => perfil.PersonaID.Equals(Usuario.UsuarioActual.PersonaID) && !perfil.OrganizacionID.HasValue).Clave;
                        }
                        else
                        {
                            PerfilCookieID = mGestorIdentidades.ListaPerfiles.Values.First(perfil => perfil.PersonaID.Equals(Usuario.UsuarioActual.PersonaID)).Clave;
                        }
                    }

                    if ((mGestorIdentidades.ListaIdentidades.ContainsKey(Usuario.UsuarioActual.IdentidadID)) || (Usuario.UsuarioActual.IdentidadID.Equals(UsuarioAD.Invitado)))
                    {
                        if ((!mGestorIdentidades.ListaIdentidades.ContainsKey(Usuario.UsuarioActual.IdentidadID)) && (Usuario.UsuarioActual.IdentidadID.Equals(UsuarioAD.Invitado)))
                        {
                            UtilUsuario.CrearIdentidadUsuarioInvitadoParaPerfil(mGestorIdentidades, PerfilCookieID, Usuario.UsuarioActual.OrganizacionID, Usuario.UsuarioActual.ProyectoID);
                        }
                        mIdentidadActual = mGestorIdentidades.ListaIdentidades[Usuario.UsuarioActual.IdentidadID];
                    }
                    else
                    {
                        List<AD.EntityModel.Models.IdentidadDS.Identidad> listaIdentidad = mGestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Where(ident => ident.ProyectoID.Equals(ProyectoAD.MetaProyecto) && ident.OrganizacionID.Equals(ProyectoAD.MetaOrganizacion) && !ident.Tipo.Equals((short)TiposIdentidad.Organizacion) && !ident.PerfilID.Equals(PerfilCookieID)).ToList();
                        if (listaIdentidad.Count > 0)
                        {
                            mIdentidadActual = mGestorIdentidades.ListaIdentidades[listaIdentidad.First().IdentidadID];
                        }
                        else if (mGestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Count(ident => ident.ProyectoID.Equals(ProyectoAD.MetaProyecto) && !ident.Tipo.Equals((short)TiposIdentidad.Organizacion)) > 0)
                        {
                            mIdentidadActual = mGestorIdentidades.ListaIdentidades[mGestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Where(ident => ident.ProyectoID.Equals(ProyectoAD.MetaProyecto) && !ident.Tipo.Equals((short)TiposIdentidad.Organizacion)).First().IdentidadID];
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
        /// Obtiene los identificadores de la identidad y perfil actual del usuario (PerfilID, IdentidadID)
        /// </summary>
        public Guid[] IdentidadIDActual
        {
            get
            {
                if (mIdentidadIDActual == null)
                {
                    Guid usuarioID = UsuarioAD.Invitado;
                    Guid personaID = UsuarioAD.Invitado;

                    if (Usuario.UsuarioActual != null)
                    {
                        usuarioID = Usuario.UsuarioActual.UsuarioID;
                        personaID = Usuario.UsuarioActual.PersonaID;
                    }
                    else
                    {
                        HttpCookie cookieRewrite = UtilUsuario.CookieRewrite;
                        if (cookieRewrite != null && cookieRewrite != null)
                        {
                            personaID = new Guid(cookieRewrite.Values["personaID"]);

                            if (cookieRewrite.Values["usuarioID"] != null)
                            {
                                usuarioID = new Guid(cookieRewrite.Values["usuarioID"]);
                            }
                            else
                            {
                                PersonaCN persCN = new PersonaCN(mUtilPeticion, mEntityContext, mLoggingService, mHttpContextAccessor, mEnv, mConfigService);
                                usuarioID = persCN.ObtenerUsuarioIDDePersonaID(personaID).Value;
                            }
                        }
                    }

                    IdentidadCL identCL = new IdentidadCL(mUtilPeticion, mEntityContext, mLoggingService, mHttpContextAccessor, mEnv, mCache, mConfigService);
                    mIdentidadIDActual = identCL.ObtenerIdentidadActualUsuario(usuarioID, personaID, HttpContext.Current.Request);
                }

                return mIdentidadIDActual;
            }
        }



        /// <summary>
        /// Obtiene o establece si el usuario se encuentra en la página de login de la comunidad o no
        /// </summary>
        public bool EstaEnLoginComunidad
        {
            get
            {
                return mEstaEnLoginComunidad;
            }
            set
            {
                this.mEstaEnLoginComunidad = value;
            }
        }

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
                        mParametroGeneralVirtualRow = UtilUsuario.ObtenerFilaParametrosGeneralesDeProyecto(ProyectoVirtual.Clave).ListaParametroGeneral.FirstOrDefault();
                    }
                    else
                    {
                        mParametroGeneralVirtualRow = ParametrosGeneralesRow;
                    }
                }
                return mParametroGeneralVirtualRow;
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
                        ParametroAplicacionCL paramCL = new ParametroAplicacionCL(mUtilPeticion, mEntityContext, mLoggingService, mHttpContextAccessor, mEnv, mCache, mConfigService);
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
                    mLoggingService.GuardarLogError(ex, " ERROR: " + ex.Message + "\r\nStackTrace: " + ex.StackTrace);
                }
                return urlPerfil;
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
        /// Obtiene la URL del los elementos estaticos de la página
        /// </summary>
        public string BaseURLStatic
        {
            get
            {
                if (string.IsNullOrEmpty(mBaseURLStatic))
                {

                    mBaseURLStatic = UtilUsuario.UrlStatics(ProyectoVirtual.Clave, HttpContext.Current.Request.Url.Host);

                    if (mBaseURLStatic == "")
                    {
                        mBaseURLStatic = BaseURL;
                    }
                }
                return mBaseURLStatic;
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
                    string dominio = ProyectoSeleccionado.UrlPropia.Replace("http://", "").Replace("https://", "");
                    mBaseURLContent = UtilUsuario.UrlContent(ProyectoSeleccionado.Clave, dominio);

                    if (mBaseURLContent == "")
                    {
                        mBaseURLContent = BaseURL;
                    }
                }
                return mBaseURLContent;
            }
        }

        /// <summary>
        /// Obtiene la URL del los elementos de contenido de la página
        /// </summary>
        public string BaseURLPersonalizacion
        {
            get
            {
                if (mBaseURLPersonalizacion == null)
                {
                    string dominio = ProyectoSeleccionado.UrlPropia.Replace("http://", "").Replace("https://", "");
                    mBaseURLPersonalizacion = UtilUsuario.UrlContent(ProyectoSeleccionado.Clave, dominio);

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
                    string dominio = ProyectoSeleccionado.UrlPropia.Replace("http://", "").Replace("https://", "");
                    mBaseURLPersonalizacionEcosistema = UtilUsuario.UrlContent(ProyectoAD.MetaProyecto, dominio);

                    if (mBaseURLPersonalizacionEcosistema == "")
                    {
                        mBaseURLPersonalizacionEcosistema = BaseURL;
                    }

                    if (ParametroProyectoEcosistema.ContainsKey("RutaEstilos") && !string.IsNullOrEmpty(ParametroProyectoEcosistema["RutaEstilos"]))
                    {
                        mBaseURLPersonalizacionEcosistema = mBaseURLPersonalizacionEcosistema + "/imagenes/proyectos/personalizacion/" + ParametroProyectoEcosistema["RutaEstilos"];
                        mBaseURLPersonalizacionEcosistema.Replace("\\", "/");
                    }
                    else
                    {
                        mBaseURLPersonalizacionEcosistema = mBaseURLPersonalizacionEcosistema + "/imagenes/proyectos/personalizacion/ecosistema";
                    }


                }
                return mBaseURLPersonalizacionEcosistema;
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
                    Guid proyectoID = Guid.Empty;
                    if (ProyectoSeleccionado != null)
                    {
                        proyectoID = ProyectoSeleccionado.Clave;
                    }

                    mUrlServicioContextos = Conexion.ObtenerServicio("urlServicioContextos", proyectoID, UtilUsuario.DominoAplicacion);
                }
                return mUrlServicioContextos;
            }
        }

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



        #region Configuración AutoCompletar

        /// <summary>
        /// Configuración para el autoCompletar de la comunidad.
        /// </summary>
        public TagsAutoDS ConfiguracionAutoCompetarComunidad
        {
            get
            {
                if (mConfiguracionAutoCompetarComunidad == null)
                {
                    ParametroGeneralCL paramGenCL = new ParametroGeneralCL(mUtilPeticion, mEntityContext, mLoggingService, mHttpContextAccessor, mEnv, mCache, mConfigService);
                    mConfiguracionAutoCompetarComunidad = paramGenCL.ObtenerConfiguracionAutoCompetarComunidad(ProyectoSeleccionado.FilaProyecto.OrganizacionID, ProyectoSeleccionado.Clave);
                    paramGenCL.Dispose();
                }

                return mConfiguracionAutoCompetarComunidad;
            }
        }

        /// <summary>
        /// Comprueba si el proyecto actual tiene tabla propia para autocompletar.
        /// </summary>
        public bool TieneProyectoTablaPropiaAutocompletar
        {
            get
            {
                return (ConfiguracionAutoCompetarComunidad.ConfigAutocompletarProy.Select("ProyectoID='" + ProyectoSeleccionado.Clave + "' AND Clave='" + ParametroCongifAutoEtiProy.TablaPropia + "' AND Valor='1'").Length > 0);
            }
        }


        /// <summary>
        /// Facetas para autocompletar la comunidad.
        /// </summary>
        public string FacetasProyAutoCompBuscadorCom
        {
            get
            {
                DataRow[] filasConfig = ConfiguracionAutoCompetarComunidad.ConfigAutocompletarProy.Select("ProyectoID='" + ProyectoSeleccionado.Clave + "' AND Clave='" + ParametroCongifAutoEtiProy.FacetasBuscadorCom + "'");

                if (filasConfig.Length > 0)
                {
                    return ((TagsAutoDS.ConfigAutocompletarProyRow)filasConfig[0]).Valor;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Facetas para autocompletar la comunidad.
        /// </summary>
        public string FacetasProyAutoCompBuscadorPagina
        {
            get
            {
                DataRow[] filasConfig = ConfiguracionAutoCompetarComunidad.ConfigAutocompletarProy.Select("ProyectoID='" + ProyectoSeleccionado.Clave + "' AND Clave='" + ParametroCongifAutoEtiProy.FacetasPagina + OrigenAutoCompletarPagina + "'");

                if (filasConfig.Length > 0)
                {
                    return ((TagsAutoDS.ConfigAutocompletarProyRow)filasConfig[0]).Valor;
                }
                else
                {
                    return null;
                }
            }
        }


        /// <summary>
        /// Obtiene el origen del autocompletar según el tipo de página.
        /// </summary>
        public string OrigenAutoCompletarPagina
        {
            get
            {
                string origenAutoCompletar = "";

                if (TipoPagina == TiposPagina.BaseRecursos)
                {
                    origenAutoCompletar = OrigenAutocompletar.Recursos;
                }
                else if (TipoPagina == TiposPagina.Debates)
                {
                    origenAutoCompletar = OrigenAutocompletar.Debates;
                }
                else if (TipoPagina == TiposPagina.Encuestas)
                {
                    origenAutoCompletar = OrigenAutocompletar.Encuestas;
                }
                else if (TipoPagina == TiposPagina.Preguntas)
                {
                    origenAutoCompletar = OrigenAutocompletar.Preguntas;
                }
                else if (TipoPagina == TiposPagina.PersonasYOrganizaciones)
                {
                    origenAutoCompletar = OrigenAutocompletar.PersyOrg;
                }
                else if (TipoPagina == TiposPagina.Semanticos)
                {
                    origenAutoCompletar = ProyectoPestanyaActual.FilaProyectoPestanyaMenu.Ruta;
                }

                return origenAutoCompletar;
            }
        }

        #endregion


        public string UrlIntragnoss
        {
            get
            {
                if (string.IsNullOrEmpty(mUrlIntragnoss))
                {
                    //EntityContext context = EntityContext.Instance;
                    //List<AD.EntityModel.ParametroAplicacion> busqueda = context.ParametroAplicacion.Where(parametro => parametro.Parametro.Equals("UrlIntragnoss")).ToList();
                    List<AD.EntityModel.ParametroAplicacion> busqueda = ListaParametrosAplicacion.Where(parametro => parametro.Parametro.Equals("UrlIntragnoss")).ToList();
                    mUrlIntragnoss = busqueda.First().Valor;
                    //  mUrlIntragnoss = (string)ParametrosAplicacionDS.ParametroAplicacion.Select("Parametro = 'UrlIntragnoss'")[0]["Valor"];
                }
                return mUrlIntragnoss;
            }
        }

        public string EspacioPersonal
        {
            get
            {
                //EntityContext context = EntityContext.Instance;
                //List<AD.EntityModel.ParametroAplicacion> busqueda = context.ParametroAplicacion.Where(parametro => parametro.Parametro.Equals("NombreEspacioPersonal")).ToList();
                List<AD.EntityModel.ParametroAplicacion> busqueda = ListaParametrosAplicacion.Where(parametro => parametro.Parametro.Equals("NombreEspacioPersonal")).ToList();
                if (string.IsNullOrEmpty(mEspacioPersonal) && busqueda.Count > 0)
                {
                    mEspacioPersonal = busqueda.First().Valor;
                    /*if (string.IsNullOrEmpty(mEspacioPersonal) && ParametrosAplicacionDS.ParametroAplicacion.Select("Parametro = 'NombreEspacioPersonal'").Length > 0)
                    {
                        mEspacioPersonal = (string)ParametrosAplicacionDS.ParametroAplicacion.Select("Parametro = 'NombreEspacioPersonal'")[0]["Valor"];*/

                }
                return mEspacioPersonal;
            }
        }

        /// <summary>
        /// Obtiene la lista de items extra que se obtendrá de la búsqueda y su prefijo (recetas, peliculas, etc)
        /// </summary>
        public Dictionary<string, List<string>> InformacionOntologias
        {
            get
            {
                if (mInformacionOntologias == null)
                {
                    mInformacionOntologias = UtilServiciosWeb.UtilServiciosFacetas.ObtenerInformacionOntologias(ProyectoSeleccionado.FilaProyecto.OrganizacionID, ProyectoSeleccionado.Clave);
                }

                return mInformacionOntologias;
            }
        }

        public string NombreProyectoEcosistema
        {
            get
            {
                if (string.IsNullOrEmpty(mNombreProyectoEcosistema))
                {
                    ProyectoCN proyCN = new ProyectoCN(mUtilPeticion, mEntityContext, mLoggingService, mHttpContextAccessor, mEnv, mConfigService);
                    mNombreProyectoEcosistema = proyCN.ObtenerNombreDeProyectoID(ProyectoAD.MetaProyecto);
                    proyCN.Dispose();
                }
                return mNombreProyectoEcosistema;
            }
        }

        public bool PuedeVerTodasLasPersonas
        {
            get
            {
                return (Usuario.UsuarioActual.ProyectoID.Equals(ProyectoAD.MetaProyecto) && Usuario.UsuarioActual.EstaAutorizado((ulong)Capacidad.General.CapacidadesPersonas.VerTODASpersonas));
            }
        }

        public bool AdministradorQuiereVerTodasLasPersonas
        {
            get
            {
                return (PuedeVerTodasLasPersonas && (!string.IsNullOrEmpty(RequestParams("admin"))));
            }
        }

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
                        //mVersion = Conexion.ObtenerParametro("configBD/bd.config", "version", true);

                        string ficheroVersion = "Config/version.txt";

                        System.IO.StreamReader sr = new System.IO.StreamReader(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + ficheroVersion);
                        mVersion = sr.ReadToEnd();
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
        /// Parametros de configuración del proyecto.
        /// </summary>
        private Dictionary<string, string> ParametroProyecto
        {
            get
            {
                if (mParametroProyecto == null)
                {
                    ProyectoCL proyectoCL = new ProyectoCL(mUtilPeticion, mEntityContext, mLoggingService, mHttpContextAccessor, mEnv, mCache, mConfigService);
                    mParametroProyecto = proyectoCL.ObtenerParametrosProyecto(ProyectoSeleccionado.Clave);
                    proyectoCL.Dispose();
                }

                return mParametroProyecto;
            }
        }

        /// <summary>
        /// Parametros de configuración del proyecto.
        /// </summary>
        private Dictionary<string, string> ParametroProyectoEcosistema
        {
            get
            {
                if (mParametroProyectoEcosistema == null)
                {
                    ProyectoCL proyectoCL = new ProyectoCL(mUtilPeticion, mEntityContext, mLoggingService, mHttpContextAccessor, mEnv, mCache, mConfigService);
                    mParametroProyecto = proyectoCL.ObtenerParametrosProyecto(ProyectoAD.MetaProyecto);
                    proyectoCL.Dispose();
                }

                return mParametroProyectoEcosistema;
            }
        }

        public string UrlIntragnossServicios
        {
            get
            {
                if (mUrlIntragnossServicios == null)
                {
                    if (ParametroProyecto != null && ParametroProyecto.ContainsKey(TiposParametrosAplicacion.UrlIntragnossServicios))
                    {
                        mUrlIntragnossServicios = ParametroProyecto[TiposParametrosAplicacion.UrlIntragnossServicios];
                    }
                    else
                    {
                        AD.EntityModel.ParametroAplicacion fila = GestorParametrosAplicacion.ParametroAplicacion.FirstOrDefault(parametro => parametro.Parametro.Equals(TiposParametrosAplicacion.UrlIntragnossServicios));

                        if (fila != null)
                        {
                            mUrlIntragnossServicios = fila.Valor;
                        }
                        else
                        {
                            mUrlIntragnossServicios = "";
                        }
                    }

                    if (mUrlIntragnossServicios.StartsWith("https://"))
                    {
                        mUrlIntragnossServicios = mUrlIntragnossServicios.Replace("https://", "http://");
                    }
                }
                return mUrlIntragnossServicios;
            }
        }

        /// <summary>
        /// Nombre del proyecto padre del ecosistema (comunidad/nombrecorto)
        /// </summary>
        public string NombreProyectoPadreEcositema
        {
            get
            {
                if (mNombreProyectoPadreEcositema == null)
                {
                    ParametroAplicacionCL paramCL = new ParametroAplicacionCL(mUtilPeticion, mEntityContext, mLoggingService, mHttpContextAccessor, mEnv, mCache, mConfigService);
                    List<AD.EntityModel.ParametroAplicacion> parametrosAplicacionDS = paramCL.ObtenerParametrosAplicacionPorContext();
                    ParametroAplicacion ComunidadPadreEcosistemaID = parametrosAplicacionDS.FirstOrDefault(parametro => parametro.Parametro.Equals("ComunidadPadreEcosistemaID"));
                    paramCL.Dispose();
                    if (ComunidadPadreEcosistemaID != null)
                    {
                        try
                        {
                            ProyectoCN proyCN = new ProyectoCN(mUtilPeticion, mEntityContext, mLoggingService, mHttpContextAccessor, mEnv, mConfigService);
                            mNombreProyectoPadreEcositema = proyCN.ObtenerNombreCortoProyecto(Guid.Parse(ComunidadPadreEcosistemaID.Valor));
                            proyCN.Dispose();
                        }
                        catch
                        {
                            mLoggingService.GuardarLogError("El parametro ComunidadPadreEcosistemaID no esta bien configurado.");
                            mNombreProyectoPadreEcositema = "";
                        }
                    }
                    if (mNombreProyectoPadreEcositema == null)
                    {
                        mNombreProyectoPadreEcositema = "";
                    }
                }
                return mNombreProyectoPadreEcositema;
            }
        }

        public string NombreCortoProyectoPadreEcositema
        {
            get
            {
                if (mNombreCortoProyectoPadreEcosistema == null)
                {
                    ParametroAplicacionCL paramCL = new ParametroAplicacionCL(mUtilPeticion, mEntityContext, mLoggingService, mHttpContextAccessor, mEnv, mCache, mConfigService);
                    List<AD.EntityModel.ParametroAplicacion> parametrosAplicacionDS = paramCL.ObtenerParametrosAplicacionPorContext();
                    ParametroAplicacion NombreCortoProyectoPadreEcositema = parametrosAplicacionDS.FirstOrDefault(parametro => parametro.Parametro.Equals("NombreCortoProyectoPadreEcositema"));
                    paramCL.Dispose();
                    if (NombreCortoProyectoPadreEcositema != null)
                    {
                        try
                        {
                            ProyectoCN proyCN = new ProyectoCN(mUtilPeticion, mEntityContext, mLoggingService, mHttpContextAccessor, mEnv, mConfigService);
                            mNombreCortoProyectoPadreEcosistema = NombreCortoProyectoPadreEcositema.Valor;
                            proyCN.Dispose();
                        }
                        catch
                        {
                            mLoggingService.GuardarLogError("El parametro NombreCortoProyectoPadreEcositema no esta bien configurado.");
                            mNombreCortoProyectoPadreEcosistema = "";
                        }
                    }
                    if (mNombreCortoProyectoPadreEcosistema == null)
                    {
                        mNombreCortoProyectoPadreEcosistema = "";
                    }
                }
                return mNombreProyectoPadreEcositema;
            }
        }

        /// <summary>
        /// Nombre del proyecto padre del ecosistema (comunidad/nombrecorto)
        /// </summary>
        public Guid? ProyectoIDPadreEcosistema
        {
            get
            {
                if (mPadreEcosistemaProyectoID == null)
                {

                    if (!string.IsNullOrEmpty(NombreCortoProyectoPadreEcositema))
                    {
                        try
                        {
                            ProyectoCN proyCN = new ProyectoCN(mUtilPeticion, mEntityContext, mLoggingService, mHttpContextAccessor, mEnv, mConfigService);
                            mPadreEcosistemaProyectoID = proyCN.ObtenerProyectoIDPorNombreCorto(NombreCortoProyectoPadreEcositema);
                            proyCN.Dispose();
                        }
                        catch
                        {
                            mLoggingService.GuardarLogError("El parametro NombreCortoProyectoPadreEcositema no esta bien configurado.");
                            mPadreEcosistemaProyectoID = Guid.Empty;
                        }
                    }

                    if (!string.IsNullOrEmpty(NombreProyectoPadreEcositema) && (mPadreEcosistemaProyectoID == null || (mPadreEcosistemaProyectoID != null && mPadreEcosistemaProyectoID.Value.Equals(Guid.Empty))))
                    {
                        try
                        {
                            ProyectoCN proyCN = new ProyectoCN(mUtilPeticion, mEntityContext, mLoggingService, mHttpContextAccessor, mEnv, mConfigService);
                            mPadreEcosistemaProyectoID = proyCN.ObtenerProyectoIDPorNombreCorto(NombreProyectoPadreEcositema);
                            proyCN.Dispose();
                        }
                        catch
                        {
                            mLoggingService.GuardarLogError("El parametro ComunidadPadreEcosistemaID no esta bien configurado.");
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

        /// <summary>
        /// Contiene la lista de permisos sobre los tipos de documentos.
        /// </summary>
        public List<TiposDocumentacion> ListaPermisosDocumentos
        {
            get
            {
                if (mListaPermisosDocumentos == null)
                {
                    ProyectoCL proyCL = new ProyectoCL(mUtilPeticion, mEntityContext, mLoggingService, mHttpContextAccessor, mEnv, mCache, mConfigService);
                    mListaPermisosDocumentos = proyCL.ObtenerTiposDocumentosPermitidosUsuarioEnProyecto(ProyectoVirtual.Clave, TipoRolUsuarioEnProyecto);
                    proyCL.Dispose();
                }
                return mListaPermisosDocumentos;
            }
        }


        /// <summary>
        /// Obtiene la URL base de los formularios semánticos.
        /// </summary>
        public string BaseURLFormulariosSem
        {
            get
            {
                if (mBaseURLFormulariosSem == null)
                {
                    //string host = UtilDominios.DomainToPunyCode(HttpContext.Current.Request.Url.Host);
                    //string url = "http://" + host + HttpContext.Current.Request.ApplicationPath.Trim();

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

        public TipoRolUsuario TipoRolUsuarioEnProyecto
        {
            get
            {
                if (!mTipoRolUsuarioEnProyecto.HasValue)
                {
                    mTipoRolUsuarioEnProyecto = TipoRolUsuario.Usuario;

                    if (Usuario.UsuarioActual != null && !Usuario.UsuarioActual.EsIdentidadInvitada)
                    {
                        var filaAdmin = ProyectoSeleccionado.GestorProyectos.DataWrapperProyectos.ListaAdministradorProyecto.FirstOrDefault(a => a.UsuarioID == Usuario.UsuarioActual.UsuarioID);
                        if (filaAdmin != null)
                        {
                            mTipoRolUsuarioEnProyecto = (TipoRolUsuario)filaAdmin.Tipo;
                        }
                    }
                }
                return mTipoRolUsuarioEnProyecto.Value;
            }
        }

        bool? mEstaUsuarioBloqueadoProyecto = null;

        public bool EstaUsuarioBloqueadoProyecto
        {
            get
            {
                if (!mEstaUsuarioBloqueadoProyecto.HasValue)
                {
                    ProyectoCL proyCL = new ProyectoCL(mUtilPeticion, mEntityContext, mLoggingService, mHttpContextAccessor, mEnv, mCache, mConfigService);
                    mEstaUsuarioBloqueadoProyecto = proyCL.ObtenerUsuarioBloqueadoProyecto(ProyectoVirtual.Clave, Usuario.UsuarioActual.UsuarioID);
                    proyCL.Dispose();
                }
                return mEstaUsuarioBloqueadoProyecto.Value;
            }
        }


        public bool EsUsuarioAdministradorProyecto
        {
            get
            {
                return TipoRolUsuarioEnProyecto.Equals(TipoRolUsuario.Administrador);
            }
        }

        public bool EsUsuarioAdministradorProyectoVirtual
        {
            get
            {
                //Es administrador
                if (ProyectoSeleccionado != ProyectoVirtual)
                {
                    //"UsuarioID = '" + Usuario.UsuarioActual.UsuarioID.ToString() + "'"
                    return ProyectoVirtual.GestorProyectos.DataWrapperProyectos.ListaAdministradorProyecto.Where(adminProy => adminProy.UsuarioID.Equals(Usuario.UsuarioActual.UsuarioID)).ToList().Count > 0;
                }
                else
                {
                    return EsUsuarioAdministradorProyecto;
                }
            }
        }

        /// <summary>
        /// Obtiene si algún miembro de la organización es administrador del proyecto al que se está conectando el usuario
        /// </summary>
        public bool EsAlguienDeLaOrganizacionAdministradorProyecto
        {
            get
            {
                if (HttpContext.Current.Session["EsAlguienDeLaOrganizacionAdministradorProyecto"] == null)
                {
                    ProyectoCN proyCN = new ProyectoCN();
                    HttpContext.Current.Session["EsAlguienDeLaOrganizacionAdministradorProyecto"] = proyCN.EsAlguienDeLAOrganizacionAdministradorProyecto((Guid)IdentidadActual.OrganizacionID, Usuario.UsuarioActual.ProyectoID);
                    proyCN.Dispose();
                }

                return (bool)HttpContext.Current.Session["EsAlguienDeLaOrganizacionAdministradorProyecto"];
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
                    ProyectoCN proyCN = new ProyectoCN(mUtilPeticion, mEntityContext, mLoggingService, mHttpContextAccessor, mEnv, mConfigService);
                    mContadoresProyecto = proyCN.ObtenerContadoresProyecto(ProyectoSeleccionado.Clave).ListaProyecto[0];
                    proyCN.Dispose();
                }

                return mContadoresProyecto;
            }
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
                    ProyectoGBD.ProyectoGBD proyectoGBD = new ProyectoGBD.ProyectoGBD();
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

        /// <summary>
        /// Devuelve si la organización es de gnossOrganiza
        /// </summary>
        public bool EsGnossOrganiza
        {
            get
            {
                return UtilUsuario.OrganizacionGnoss != ProyectoAD.MyGnoss;
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
                    //EntityContext context = EntityContext.Instance;
                    //List<AD.EntityModel.ParametroAplicacion> busqueda = context.ParametroAplicacion.Where(parametro => parametro.Parametro.Equals(TiposParametrosAplicacion.RegistroAutomaticoEcosistema)).ToList();
                    List<AD.EntityModel.ParametroAplicacion> busqueda = ListaParametrosAplicacion.Where(parametro => parametro.Parametro.Equals(TiposParametrosAplicacion.RegistroAutomaticoEcosistema)).ToList();
                    // if (ParametrosAplicacionDS.ParametroAplicacion.Select("Parametro = '" + TiposParametrosAplicacion.RegistroAutomaticoEcosistema.ToString() + "'").Length > 0)
                    if (busqueda.Count > 0)
                    {
                        mRegistroAutomaticoEcosistema = bool.Parse(busqueda.First().Valor);
                        // mRegistroAutomaticoEcosistema = bool.Parse((string)ParametrosAplicacionDS.ParametroAplicacion.Select("Parametro = '" + TiposParametrosAplicacion.RegistroAutomaticoEcosistema.ToString() + "'")[0]["Valor"]);
                    }
                }
                return mRegistroAutomaticoEcosistema.Value;
            }
        }

        /// <summary>
        /// Indica si el registro es abierto para una comunidad, aunque en el ecosistema no lo sea
        /// </summary>
        public bool RegistroAutomáticoEnComunidad
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
        /// Parámetros de un proyecto.
        /// </summary>
        public DataWrapperExportacionBusqueda ExportacionBusquedaDW
        {
            get
            {
                if (mExportacionBusquedaDW == null)
                {
                    ExportacionBusquedaCL exportacionCL = new ExportacionBusquedaCL();
                    mExportacionBusquedaDW = exportacionCL.ObtenerExportacionesProyecto(ProyectoSeleccionado.Clave);
                    exportacionCL.Dispose();
                }

                return mExportacionBusquedaDW;
            }
        }

        /// <summary>
        /// Obtiene la URL de la petición actual para añadirla a una url con redirect: Ej de respuesta: /redirect/comunidad/materialeducativo/recurso...
        /// </summary>
        public string UrlParaLoginConRedirect
        {
            get
            {
                string url = HttpContext.Current.Request.Url.ToString();
                string redirect = "";
                string dominio = UtilDominios.ObtenerDominioUrl(HttpContext.Current.Request.Url, true);
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

        private string mScriptGoogleAnalytics;
        private string mCodigoCompletoGoogleAnalytics;

        public string CodigoCompletoGoogleAnalytics
        {
            get
            {
                if (string.IsNullOrEmpty(mCodigoCompletoGoogleAnalytics))
                {
                    mCodigoCompletoGoogleAnalytics = "";

                    //Si no tiene URL Propia cargamos Google Analytics de Gnoss
                    string codigoGoogle = null;

                    ParametroGeneral filaParametrosGen = ParametrosGeneralesRow;
                    if ((ProyectoSeleccionado != null) && (ProyectoSeleccionado.FilaProyecto.URLPropia == null))
                    {
                        //Sacamos el código de google analytics
                        filaParametrosGen = UtilUsuario.ObtenerFilaParametrosGeneralesDeProyecto(ProyectoAD.MetaProyecto).ListaParametroGeneral.FirstOrDefault();
                    }

                    if (filaParametrosGen != null && !(filaParametrosGen.CodigoGoogleAnalytics == null)) //Si no es nulo mostramos el script de google analytics
                    {
                        codigoGoogle = filaParametrosGen.CodigoGoogleAnalytics;

                        if (!(filaParametrosGen.ScriptGoogleAnalytics == null))
                        {
                            mScriptGoogleAnalytics = filaParametrosGen.ScriptGoogleAnalytics;
                        }
                    }

                    if (!string.IsNullOrEmpty(codigoGoogle))
                    {
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

        /// <summary>
        /// Obtiene el script de google analyitics
        /// </summary>
        public string ScriptGoogleAnalytics
        {
            get
            {
                if (string.IsNullOrEmpty(mScriptGoogleAnalytics))
                {
                    //EntityContext context = EntityContext.Instance;
                    //AD.EntityModel.ParametroAplicacion busqueda = context.ParametroAplicacion.Where(parametro => parametro.Parametro.Equals("ScriptGoogleAnalytics")).First();
                    AD.EntityModel.ParametroAplicacion busqueda = ListaParametrosAplicacion.Where(parametro => parametro.Parametro.Equals("ScriptGoogleAnalytics")).First();
                    mScriptGoogleAnalytics = busqueda.Valor;
                    // mScriptGoogleAnalytics = ParametrosAplicacionDS.ParametroAplicacion.FindByParametro("ScriptGoogleAnalytics").Valor;
                }
                return mScriptGoogleAnalytics;
            }
        }

        private bool mCodigoGoogleAnalyticsBusquedaEstablecido = false;

        public bool CodigoGoogleAnalyticsBusquedaEstablecido
        {
            get
            {
                //string tipoDocSem = Documento.GestorDocumental.ListaDocumentos[ontologiaID].Enlace.Substring(0, Documento.GestorDocumental.ListaDocumentos[ontologiaID].Enlace.IndexOf("."));
                /* || (tipoDocSem.ToLower().Equals("tema") || tipoDocSem.ToLower().Equals("asignatura"))*/
                ;
                return mCodigoGoogleAnalyticsBusquedaEstablecido;
            }
            set
            {
                mCodigoGoogleAnalyticsBusquedaEstablecido = value;
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

        private bool? mCargarIdentidadesDeProyectosPrivadosComoAmigos;

        public bool CargarIdentidadesDeProyectosPrivadosComoAmigos
        {
            get
            {
                if (!mCargarIdentidadesDeProyectosPrivadosComoAmigos.HasValue)
                {
                    //EntityContext context = EntityContext.Instance;
                    //List<AD.EntityModel.ParametroAplicacion> busqueda = context.ParametroAplicacion.Where(parametro => parametro.Parametro.Equals(TiposParametrosAplicacion.CargarIdentidadesDeProyectosPrivadosComoAmigos) ).ToList();
                    List<AD.EntityModel.ParametroAplicacion> busqueda = ListaParametrosAplicacion.Where(parametro => parametro.Parametro.Equals((string)TiposParametrosAplicacion.CargarIdentidadesDeProyectosPrivadosComoAmigos)).ToList();
                    if (busqueda.Count > 0)
                    {
                        mCargarIdentidadesDeProyectosPrivadosComoAmigos = busqueda.Count > 0 && busqueda.FirstOrDefault().Valor.Equals("1") || busqueda.FirstOrDefault().Valor.ToLower().Equals("true");
                    }
                    else
                    {
                        mCargarIdentidadesDeProyectosPrivadosComoAmigos = false;
                    }

                    // mCargarIdentidadesDeProyectosPrivadosComoAmigos = ParametrosAplicacionDS.ParametroAplicacion.Select("Parametro = '" + TiposParametrosAplicacion.CargarIdentidadesDeProyectosPrivadosComoAmigos + "'").Length > 0 && (ParametrosAplicacionDS.ParametroAplicacion.Select("Parametro = '" + TiposParametrosAplicacion.CargarIdentidadesDeProyectosPrivadosComoAmigos + "'")[0].Equals("1") || ParametrosAplicacionDS.ParametroAplicacion.Select("Parametro = '" + TiposParametrosAplicacion.CargarIdentidadesDeProyectosPrivadosComoAmigos + "'")[0]["Valor"].ToString().ToLower().Equals("true"));
                }

                return mCargarIdentidadesDeProyectosPrivadosComoAmigos.Value;
            }
        }

        #endregion
    }
}
