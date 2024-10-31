using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.ParametroGeneralDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.VistaVirtualDS;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Identidad;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Usuarios;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.Cookie;
using Es.Riam.Gnoss.CL.Documentacion;
using Es.Riam.Gnoss.CL.ParametrosAplicacion;
using Es.Riam.Gnoss.CL.ParametrosProyecto;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.CL.Tesauro;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Elementos.ParametroAplicacion;
using Es.Riam.Gnoss.Elementos.Tesauro;
using Es.Riam.Gnoss.Logica.Documentacion;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.Parametro;
using Es.Riam.Gnoss.Logica.ParametroAplicacion;
using Es.Riam.Gnoss.Logica.ParametrosProyecto;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Recursos;
using Es.Riam.Gnoss.Servicios;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Util.Seguridad;
using Es.Riam.Gnoss.UtilServiciosWeb;
using Es.Riam.Gnoss.Web.Controles;
using Es.Riam.Gnoss.Web.Controles.Administracion;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Gnoss.Web.MVC.Models.ViewModels;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.Util;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Es.Riam.Gnoss.Web.MVC.Controles.Controladores
{
    public class ControllerBaseGnoss : Controller
    {

        #region Miembros

        private static Dictionary<Guid, List<MetaKeywordsOntologia>> mDicOntologiaMetasProyecto;

        /// <summary>
        /// 
        /// </summary>
        private string mIdiomaPorDefecto = null;

        /// <summary>
        /// Modelo de la comunidad.
        /// </summary>
        private CommunityModel mComunidad;

        private List<PersonalizacionCategoriaCookieModel> mListaPersonalizacionCategoriaCookieModel;

        private Guid mPersonalizacionProyecto = Guid.Empty;

        bool mUsarPersonalizacionComunidad = true;

        bool mUsarPersonalizacionAdministracionComunidad = false;

        protected ControladorBase mControladorBase;


        /// <summary>
        /// URL base de los formularios semánticos.
        /// </summary>
        private string mBaseURLFormulariosSem;

        private string mUrlPagina;

        private Dictionary<string, string> mParametroProyecto;

        private Dictionary<string, string> mParametroProyectoEcosistema;

        private Dictionary<string, string> mParametroProyectoVirtual;

        private bool mExisteNombrePoliticaCookiesMetaproyecto;

        private string mNombrePoliticaCookiesMetaproyecto;

        /// <summary>
        /// Lista de grupos a los que pertenece la identidad actual
        /// </summary>
        private List<Guid> mListaGruposIdentidadActual = null;

        /// <summary>
        /// Lista de grupos a los que pertenece el perfil en el proyecto virtual
        /// </summary>
        private List<Guid> mListaGruposPerfilEnProyectoVirtual = null;

        private string mUrlContent = null;

        private string mEntornoIntegracionContinua = null;
        private bool? mComprobarRepetidos = null;
        private string mNombrePlataforma = null;
        private string mProyectoConIntegracionContinua = null;
        private string mUrlApiDespliegues = null;
        private string mUrlApiDesplieguesEntornoAnterior = null;
        private string mUrlApiDesplieguesEntornoSiguiente = null;
        private string mIPFTP = null;
        private string mPuertoFTP = null;
        private string mRamaGit = null;

        private ControladorPestanyas mControladorPestanyas;

        protected IHttpContextAccessor mHttpContextAccessor;
        protected EntityContext mEntityContext;
        private EntityContextBASE mEntityContextBASE;
        protected LoggingService mLoggingService;
        protected VirtuosoAD mVirtuosoAD;
        protected ConfigService mConfigService;
        protected RedisCacheWrapper mRedisCacheWrapper;
        protected GnossCache mGnossCache;
        private ChequeoSeguridad mChequeoSeguridad;
        protected ICompositeViewEngine mViewEngine;
        protected IUtilServicioIntegracionContinua mUtilServicioIntegracionContinua;
        protected IServicesUtilVirtuosoAndReplication mServicesUtilVirtuosoAndReplication;
        protected IHostingEnvironment mEnv;

        #endregion

        public ControllerBaseGnoss(IHttpContextAccessor httpContextAccessor, EntityContext entityContext, LoggingService loggingService, ConfigService configService, RedisCacheWrapper redisCacheWrapper, VirtuosoAD virtuosoAD, GnossCache gnossCache, ICompositeViewEngine viewEngine, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IHostingEnvironment env, EntityContextBASE pEntityContextBASE)
        {
            mHttpContextAccessor = httpContextAccessor;
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mEntityContextBASE = pEntityContextBASE;
            mConfigService = configService;
            mRedisCacheWrapper = redisCacheWrapper;
            mVirtuosoAD = virtuosoAD;
            mGnossCache = gnossCache;
            mViewEngine = viewEngine;
            mUtilServicioIntegracionContinua = utilServicioIntegracionContinua;
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
            mEnv = env;
            mControladorBase = new ControladorBase(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, servicesUtilVirtuosoAndReplication);
        }

        #region Métodos de eventos

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            CargarViewBagBasico();
        }

        #endregion

        #region Metodos generales

        protected virtual CommunityModel CargarDatosComunidad()
        {
            ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);

            CommunityModel comunidad = proyCL.ObtenerComunidadMVC(ProyectoVirtual.Clave);
            string nombreMetaComunida = proyCL.ObtenerNombreDeProyectoID(ProyectoAD.MetaProyecto);

            if (comunidad == null)
            {
                Proyecto proyecto = proyCL.ObtenerProyectoPorID(ProyectoSeleccionado.Clave).ListaProyecto.FirstOrDefault();
                comunidad = new CommunityModel();

                comunidad.Name = ProyectoVirtual.Nombre;
                comunidad.PresentationName = ProyectoVirtual.Nombre;
                comunidad.Description = UtilCadenas.ObtenerTextoDeIdioma(ProyectoVirtual.FilaProyecto.Descripcion, UtilIdiomas.LanguageCode, ParametrosGeneralesRow.IdiomaDefecto);
                
                if (proyecto != null)                 
                {
                    comunidad.UrlPropia = proyecto.URLPropia;
                }

                if (!string.IsNullOrEmpty(ProyectoVirtual.FilaProyecto.NombrePresentacion))
                {
                    comunidad.PresentationName = ProyectoVirtual.FilaProyecto.NombrePresentacion;
                }

                comunidad.GNOSSCommunity = ParametrosGeneralesVirtualRow.ComunidadGNOSS;

                comunidad.Key = ProyectoVirtual.Clave;

                comunidad.ShortName = ProyectoVirtual.NombreCorto;

                comunidad.AccessType = (CommunityModel.TypeAccessProject)(ProyectoVirtual.FilaProyecto.TipoAcceso);

                comunidad.ProyectType = (CommunityModel.TypeProyect)(ProyectoVirtual.FilaProyecto.TipoProyecto);

                if (!string.IsNullOrEmpty(ProyectoVirtual.FilaProyecto.UsuarioTwitter))
                {
                    comunidad.TwitterLink = "http://twitter.com/#!/" + ProyectoVirtual.FilaProyecto.UsuarioTwitter.Trim('/');
                }

                VistaVirtualCL vistaVirtualCL = new VistaVirtualCL(mEntityContext, mLoggingService, mGnossCache, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                DataWrapperVistaVirtual vistaVirtualDW = vistaVirtualCL.ObtenerVistasVirtualPorProyectoID(ProyectoVirtual.Clave, mControladorBase.PersonalizacionEcosistemaID, mControladorBase.ComunidadExcluidaPersonalizacionEcosistema);

                if (vistaVirtualDW.ListaVistaVirtualProyecto.Count > 0)
                {
                    PersonalizacionProyecto = vistaVirtualDW.ListaVistaVirtualProyecto.FirstOrDefault().PersonalizacionID;
                }

                comunidad.PersonalizacionProyectoID = PersonalizacionProyecto;

                comunidad.ListaPersonalizaciones = new List<string>();
                comunidad.ListaPersonalizacionesEcosistema = new List<string>();

                if (PersonalizacionProyecto != Guid.Empty)
                {
                    foreach (VistaVirtual filaVistaVirtual in vistaVirtualDW.ListaVistaVirtual.Where(item => item.PersonalizacionID.Equals(PersonalizacionProyecto)).ToList())
                    {
                        comunidad.ListaPersonalizaciones.Add(filaVistaVirtual.TipoPagina);
                    }
                    foreach (VistaVirtualRecursos filaVistaVirtualRecurso in vistaVirtualDW.ListaVistaVirtualRecursos.Where(item => item.PersonalizacionID == PersonalizacionProyecto).ToList())
                    {
                        comunidad.ListaPersonalizaciones.Add("/Views/FichaRecurso_" + filaVistaVirtualRecurso.RdfType + "/Index.cshtml");
                    }
                    foreach (VistaVirtualCMS filaVistaVirtualCMS in vistaVirtualDW.ListaVistaVirtualCMS.Where(item => item.PersonalizacionID.Equals(PersonalizacionProyecto)))
                    {
                        comunidad.ListaPersonalizaciones.Add("/Views/CMSPagina/" + filaVistaVirtualCMS.PersonalizacionComponenteID + ".cshtml");
                    }
                    foreach (VistaVirtualGadgetRecursos filaVistaVirtualGadgetRecursos in vistaVirtualDW.ListaVistaVirtualGadgetRecursos.Where(item => item.PersonalizacionID.Equals(PersonalizacionProyecto)))
                    {
                        comunidad.ListaPersonalizaciones.Add("/Views/Shared/" + filaVistaVirtualGadgetRecursos.PersonalizacionComponenteID + ".cshtml");
                    }
                }

                if (mControladorBase.PersonalizacionEcosistemaID != Guid.Empty)
                {
                    foreach (VistaVirtual filaVistaVirtual in vistaVirtualDW.ListaVistaVirtual.Where(item => item.PersonalizacionID.Equals(mControladorBase.PersonalizacionEcosistemaID)).ToList())
                    {
                        comunidad.ListaPersonalizacionesEcosistema.Add(filaVistaVirtual.TipoPagina);
                    }
                    foreach (VistaVirtualRecursos filaVistaVirtualRecurso in vistaVirtualDW.ListaVistaVirtualRecursos.Where(item => item.PersonalizacionID.Equals(mControladorBase.PersonalizacionEcosistemaID)).ToList())
                    {
                        comunidad.ListaPersonalizacionesEcosistema.Add("/Views/FichaRecurso_" + filaVistaVirtualRecurso.RdfType + "/Index.cshtml");
                    }
                    foreach (VistaVirtualCMS filaVistaVirtualCMS in vistaVirtualDW.ListaVistaVirtualCMS.Where(item => item.PersonalizacionID.Equals(mControladorBase.PersonalizacionEcosistemaID)))
                    {
                        comunidad.ListaPersonalizacionesEcosistema.Add("/Views/CMSPagina/" + filaVistaVirtualCMS.PersonalizacionComponenteID + ".cshtml");
                    }
                    foreach (VistaVirtualGadgetRecursos filaVistaVirtualGadgetRecursos in vistaVirtualDW.ListaVistaVirtualGadgetRecursos.Where(item => item.PersonalizacionID.Equals(mControladorBase.PersonalizacionEcosistemaID)))
                    {
                        comunidad.ListaPersonalizacionesEcosistema.Add("/Views/Shared/" + filaVistaVirtualGadgetRecursos.PersonalizacionComponenteID + ".cshtml");
                    }
                }

                comunidad.Categories = CargarTesauroProyecto();

                if (ParametrosGeneralesRow.PermitirCertificacionRec)
                {
                    DataWrapperProyecto dataWrapperProyecto = proyCL.ObtenerNivelesCertificacionRecursosProyecto(ProyectoSeleccionado.Clave);

                    comunidad.CertificationLevels = new List<string>();

                    foreach (NivelCertificacion fila in dataWrapperProyecto.ListaNivelCertificacion.OrderBy(nivel => nivel.Orden).ToList())
                    {
                        comunidad.CertificationLevels.Add(UtilCadenas.ObtenerTextoDeIdioma(fila.Descripcion, UtilIdiomas.LanguageCode, ParametrosGeneralesRow.IdiomaDefecto));
                    }
                }

                proyCL.AgregarComunidadMVC(ProyectoVirtual.Clave, comunidad);
            }

            comunidad.NameMetaComunidad = nombreMetaComunida;
            if (UsarPersonalizacionComunidad && ParametrosGeneralesVirtualRow.VersionCSS != null)
            {
                comunidad.VersionCSS = ParametrosGeneralesVirtualRow.VersionCSS;
            }
            if (UsarPersonalizacionComunidad && ParametrosGeneralesVirtualRow.VersionJS != null)
            {
                comunidad.VersionJS = ParametrosGeneralesVirtualRow.VersionJS;
            }
            if (UsarPersonalizacionAdministracionComunidad && ParametrosGeneralesVirtualRow.VersionJSAdmin != null)
            {
                comunidad.VersionJSAdmin = ParametrosGeneralesVirtualRow.VersionJSAdmin;
            }
            if (UsarPersonalizacionAdministracionComunidad && ParametrosGeneralesVirtualRow.VersionCSSAdmin != null)
            {
                comunidad.VersionCSSAdmin = ParametrosGeneralesVirtualRow.VersionCSSAdmin;
            }

            //Este parametro debe estar fuera de la cache
            comunidad.MetaProyect = ProyectoSeleccionado.Clave.Equals(ProyectoAD.MetaProyecto);

            if (mHttpContextAccessor.HttpContext.Session != null && mHttpContextAccessor.HttpContext.Session.Keys.Contains("IdVersionEstilos"))
            {
                comunidad.VersionJS = (int)mHttpContextAccessor.HttpContext.Session.Get<long>("IdVersionEstilos");
                comunidad.VersionCSS = (int)mHttpContextAccessor.HttpContext.Session.Get<long>("IdVersionEstilos");
            }

            foreach (CategoryModel cat in comunidad.Categories)
            {
                cat.Lang = UtilIdiomas.LanguageCode;
            }

            if (ProyectoVirtual.Clave != ProyectoAD.MetaProyecto)
            {
                comunidad.Url = mControladorBase.UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoVirtual.NombreCorto);
            }
            else
            {
                comunidad.Url = BaseURLIdioma;
            }

            comunidad.UrlMyGnoss = BaseURLIdioma;

            bool escomunidadEducativa = ProyectoVirtual.TipoProyecto == TipoProyecto.Universidad20 || ProyectoVirtual.TipoProyecto == TipoProyecto.EducacionExpandida || ProyectoVirtual.TipoProyecto == TipoProyecto.EducacionPrimaria;
            if (!escomunidadEducativa && !string.IsNullOrEmpty(ParametrosGeneralesVirtualRow.CoordenadasSup))
            {
                string logoComunidad = BaseURLContent + "/" + UtilArchivos.ContentImagenes + "/" + UtilArchivos.ContentImagenesProyectos + "/" + ProyectoVirtual.Clave.ToString().ToLower() + ".png";
                if (ParametrosGeneralesVirtualRow.VersionFotoImagenSupGrande != null)
                {
                    logoComunidad += "?v=" + ParametrosGeneralesVirtualRow.VersionFotoImagenSupGrande.ToString();
                }
                comunidad.Logo = logoComunidad;
            }

            comunidad.ProjectState = (CommunityModel.StateProject)(ProyectoVirtual.FilaProyecto.Estado);

            if (ProyectoVirtual.FilaProyecto.Estado == (short)EstadoProyecto.CerradoTemporalmente && ProyectoVirtual.FilaProyecto.ProyectoCerradoTmp != null)
            {
                comunidad.OpenDate = ProyectoVirtual.FilaProyecto.ProyectoCerradoTmp.FechaReapertura;
            }

            if (ProyectoVirtual.FilaProyecto.Estado == (short)EstadoProyecto.Cerrandose && ProyectoVirtual.FilaProyecto.ProyectoCerrandose != null)
            {
                comunidad.PeriodoDeGracia = DateTime.Now.AddDays(ProyectoVirtual.FilaProyecto.ProyectoCerrandose.PeriodoDeGracia);
            }

            bool esMovil = RequestParams("esMovil") == "true";
            int? numRecursos = proyCL.ObtenerContadorRecursosComunidad(UrlIntragnoss, ProyectoVirtual.FilaProyecto.ProyectoID, ProyectoVirtual.FilaProyecto.OrganizacionID, ProyectoVirtual.TipoProyecto, esMovil);

            if (numRecursos.HasValue)
            {
                comunidad.NumberOfResources = numRecursos.Value;
            }

            int? numPersonasYOrganizaciones = proyCL.ObtenerContadorPersonasYOrganizacionesComunidad(UrlIntragnoss, ProyectoVirtual.FilaProyecto.ProyectoID, ProyectoVirtual.TipoProyecto);

            if (numPersonasYOrganizaciones.HasValue)
            {
                comunidad.NumberOfPerson = numPersonasYOrganizaciones.Value;
            }

            comunidad.ListaPersonalizacionesDominio = new List<string>();
            if (mControladorBase.PersonalizacionDominio != Guid.Empty)
            {
                VistaVirtualCN vistaVirtualCN = new VistaVirtualCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                DataWrapperVistaVirtual vistaVirtualDw2 = vistaVirtualCN.ObtenerVistasVirtualPorPersonalizacionID(mControladorBase.PersonalizacionDominio);

                foreach (VistaVirtual filaVistaVirtual in vistaVirtualDw2.ListaVistaVirtual.Where(item => item.PersonalizacionID.Equals(mControladorBase.PersonalizacionDominio)).ToList())
                {
                    comunidad.ListaPersonalizacionesDominio.Add(filaVistaVirtual.TipoPagina);
                }

                foreach (VistaVirtualRecursos filaVistaVirtualRecurso in vistaVirtualDw2.ListaVistaVirtualRecursos.Where(item => item.PersonalizacionID.Equals(mControladorBase.PersonalizacionDominio)).ToList())
                {
                    comunidad.ListaPersonalizacionesDominio.Add("/Views/FichaRecurso_" + filaVistaVirtualRecurso.RdfType + "/Index.cshtml");
                }

                foreach (VistaVirtualCMS filaVistaVirtualCMS in vistaVirtualDw2.ListaVistaVirtualCMS.Where(item => item.PersonalizacionID.Equals(mControladorBase.PersonalizacionEcosistemaID)))
                {
                    comunidad.ListaPersonalizacionesDominio.Add("/Views/CMSPagina/" + filaVistaVirtualCMS.PersonalizacionComponenteID + ".cshtml");
                }

                foreach (VistaVirtualGadgetRecursos filaVistaVirtualGadgetRecursos in vistaVirtualDw2.ListaVistaVirtualGadgetRecursos.Where(item => item.PersonalizacionID.Equals(mControladorBase.PersonalizacionDominio)))
                {
                    comunidad.ListaPersonalizacionesDominio.Add("/Views/Shared/" + filaVistaVirtualGadgetRecursos.PersonalizacionComponenteID + ".cshtml");
                }
            }

            comunidad.NumberOfOrganizations = ProyectoVirtual.FilaProyecto.NumeroOrgRegistradas.Value;

            ViewBag.PersonalizacionLayout = string.Empty;
            ViewBag.Personalizacion = string.Empty;

            //#if DEBUG
            //            comunidad.ListaPersonalizaciones = new List<string>();
            //#endif

            if (!mControladorBase.IgnorarVistasPersonalizadas)
            {
                if (comunidad.ListaPersonalizaciones != null && comunidad.ListaPersonalizaciones.Count > 0)
                {
                    if (comunidad.ListaPersonalizaciones.Contains("/Views/Shared/Layout/_Layout.cshtml"))
                    {
                        ViewBag.PersonalizacionLayout = "$$$" + comunidad.PersonalizacionProyectoID;
                    }

                    ViewBag.Personalizacion = "$$$" + comunidad.PersonalizacionProyectoID;
                }
                if (comunidad.ListaPersonalizacionesDominio != null && comunidad.ListaPersonalizacionesDominio.Count > 0)
                {
                    if (!mControladorBase.PersonalizacionDominio.Equals(Guid.Empty))
                    {
                        if (ViewBag.PersonalizacionLayout == string.Empty && comunidad.ListaPersonalizacionesDominio.Contains("/Views/Shared/Layout/_Layout.cshtml"))
                        {
                            ViewBag.PersonalizacionLayout = $"$$${mControladorBase.PersonalizacionDominio}";
                        }
                        ViewBag.PersonalizacionDominio = $"$$${mControladorBase.PersonalizacionDominio}";
                    }
                }
                if (comunidad.ListaPersonalizacionesEcosistema != null && comunidad.ListaPersonalizacionesEcosistema.Count > 0 && !mControladorBase.PersonalizacionEcosistemaID.Equals(Guid.Empty))
                {
                    if (ViewBag.PersonalizacionLayout == string.Empty && comunidad.ListaPersonalizacionesEcosistema.Contains("/Views/Shared/Layout/_Layout.cshtml"))
                    {
                        ViewBag.PersonalizacionLayout = "$$$" + mControladorBase.PersonalizacionEcosistemaID;
                    }

                    ViewBag.PersonalizacionEcosistema = "$$$" + mControladorBase.PersonalizacionEcosistemaID;
                }
            }

            comunidad.Name = UtilCadenas.ObtenerTextoDeIdioma(ProyectoVirtual.Nombre, UtilIdiomas.LanguageCode, ParametrosGeneralesRow.IdiomaDefecto);

            string controllerName = this.ToString();
            controllerName = controllerName.Substring(controllerName.LastIndexOf('.') + 1);
            if (controllerName.IndexOf("Controller") > -1)
            {
                controllerName = controllerName.Substring(0, controllerName.IndexOf("Controller"));
            }
            ViewBag.ControllerName = controllerName;

            ViewBag.CMSActivado = ParametrosGeneralesRow.CMSDisponible;
            ViewBag.VistasActivadas = ProyectoSeleccionado.PersonalizacionID != Guid.Empty;

            //TODO FRAN: No se debe hacer en el controllerBase.
            comunidad.IntegracionContinuaActivada = false;
            comunidad.IntContActivadaSinRamaEnUso = false;
            comunidad.UsuarioDadoAltaIntCont = true;
            comunidad.EntornoEsPre = false;
            comunidad.EntornoEsPro = false;

            //TODO FRAN: No se debe hacer en el controllerBase.
            if (HayIntegracionContinua)
            {
                comunidad.IntegracionContinuaActivada = true;

                if (!EntornoActualEsPreproduccion && !EntornoActualEsPruebas)
                {
                    comunidad.EntornoEsPro = true;
                }
            }
            else
            {
                if (mEnv.IsProduction())
                {
                    comunidad.EntornoEsPro = true;
                }
                else if(mEnv.IsStaging())
                {
                    comunidad.EntornoEsPre = true;
                }
            }

            proyCL.Dispose();

            return comunidad;
        }

        /// <summary>
        /// Devuelve el Page.Request.Params de la pagina para que funcione con ajax
        /// </summary>
        [NonAction]
        public string RequestParams(string pParametro)
        {
            return mControladorBase.RequestParams(pParametro);
        }

        [NonAction]
        protected List<CategoryModel> CargarTesauroProyecto(string pIdiomaTesauro = null)
        {
            return CargarTesauro(ProyectoVirtual.FilaProyecto.ProyectoID, pIdiomaTesauro);
        }

        [NonAction]
        protected List<CategoryModel> CargarTesauro(Guid pProyectoID, string pIdiomaTesauro = null)
        {
            List<CategoryModel> listaCategoriasTesauro = new List<CategoryModel>();

            TesauroCL tesauroCL = new TesauroCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
            DataWrapperTesauro tesauroDW = tesauroCL.ObtenerTesauroDeProyecto(pProyectoID);

            GestionTesauro gestorTesauro = new GestionTesauro(tesauroDW, mLoggingService, mEntityContext);

            foreach (CategoriaTesauro catTes in gestorTesauro.ListaCategoriasTesauro.Values)
            {
                CategoryModel categoriaTesauro = CargarCategoria(catTes);
                if (!string.IsNullOrEmpty(pIdiomaTesauro))
                {
                    categoriaTesauro.Lang = pIdiomaTesauro;
                }
                listaCategoriasTesauro.Add(categoriaTesauro);
            }

            return listaCategoriasTesauro;
        }


        [NonAction]
        protected CategoryModel CargarCategoria(CategoriaTesauro pCategoria, string pIdioma = null)
        {
            string idioma = UtilIdiomas.LanguageCode;
            if (!string.IsNullOrEmpty(pIdioma))
            {
                idioma = pIdioma;
            }

            CategoryModel categoriaTesauro = new CategoryModel();
            categoriaTesauro.Subcategories = new List<CategoryModel>();
            categoriaTesauro.Key = pCategoria.Clave;
            categoriaTesauro.Name = pCategoria.FilaCategoria.Nombre;
            categoriaTesauro.LanguageName = pCategoria.FilaCategoria.Nombre;
            categoriaTesauro.Lang = idioma;
            categoriaTesauro.Order = pCategoria.FilaCategoria.Orden;
            categoriaTesauro.Required = pCategoria.GestorTesauro.FilasPropiedadesPorCategoria.ContainsKey(pCategoria.Clave) && pCategoria.GestorTesauro.FilasPropiedadesPorCategoria[pCategoria.Clave].Obligatoria.Equals(1);

            categoriaTesauro.ParentCategoryKey = Guid.Empty;
            if (pCategoria.Padre is CategoriaTesauro && pCategoria.FilaAgregacion != null)
            {
                categoriaTesauro.Order = pCategoria.FilaAgregacion.Orden;
                categoriaTesauro.ParentCategoryKey = ((CategoriaTesauro)pCategoria.Padre).Clave;
            }

            if (pCategoria.SubCategorias != null)
            {
                foreach (CategoriaTesauro hija in pCategoria.SubCategorias)
                {
                    categoriaTesauro.Subcategories.Add(CargarCategoria(hija, idioma));
                }
            }

            return categoriaTesauro;
        }

        [NonAction]
        protected List<MetaKeywordsOntologia> ObtenerMetaEtiquetasXMLOntologiasProyectos()
        {
            if (mDicOntologiaMetasProyecto == null || !mDicOntologiaMetasProyecto.ContainsKey(ProyectoSeleccionado.Clave))
            {
                Dictionary<string, List<MetaKeyword>> dicOntologiaMetas = new Dictionary<string, List<MetaKeyword>>();
                if (mDicOntologiaMetasProyecto == null)
                {
                    mDicOntologiaMetasProyecto = new Dictionary<Guid, List<MetaKeywordsOntologia>>();
                }
                CallFileService servicioArc = new CallFileService(mConfigService, mLoggingService);

                DocumentacionCL documentacionCL = new DocumentacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                Guid proyectoIDPatronOntologias;
                DataWrapperDocumentacion dataWrapperDocumentacion = documentacionCL.ObtenerOntologiasProyecto(ProyectoSeleccionado.FilaProyecto.ProyectoID, true);
                if (ParametroProyecto.ContainsKey("ProyectoIDPatronOntologias"))
                {
                    Guid.TryParse(ParametroProyecto["ProyectoIDPatronOntologias"], out proyectoIDPatronOntologias);
                    dataWrapperDocumentacion.Merge(documentacionCL.ObtenerOntologiasProyecto(proyectoIDPatronOntologias, true));
                }

                foreach (var ontologia in dataWrapperDocumentacion.ListaDocumento)
                {
                    byte[] byteArray = servicioArc.ObtenerXmlOntologiaBytes(ontologia.DocumentoID);
                    if (byteArray != null)
                    {
                        UtilidadesFormulariosSemanticos.ObtenerMetaEtiquetasXMLOntologia(byteArray, dicOntologiaMetas, ontologia.Enlace);
                    }
                }
                List<MetaKeywordsOntologia> listaMetaKeywordsOntologia = new List<MetaKeywordsOntologia>();
                foreach (var ontologias in dicOntologiaMetas)
                {
                    MetaKeywordsOntologia metaKeyWordOntologia = new MetaKeywordsOntologia();
                    metaKeyWordOntologia.MetaKeyWords = ontologias.Value;
                    metaKeyWordOntologia.OntologiaEnlace = ontologias.Key;
                    listaMetaKeywordsOntologia.Add(metaKeyWordOntologia);
                }
                mDicOntologiaMetasProyecto.TryAdd(ProyectoSeleccionado.Clave, listaMetaKeywordsOntologia);
            }
            return mDicOntologiaMetasProyecto[ProyectoSeleccionado.Clave];
        }

        [NonAction]
        protected Dictionary<string, List<MetaKeyword>> ObtenerMetaEtiquetasXMLOntologias()
        {
            List<MetaKeywordsOntologia> metaEtiquetasXMLOntologias = ObtenerMetaEtiquetasXMLOntologiasProyectos();
            Dictionary<string, List<MetaKeyword>> metaKeyWords = new Dictionary<string, List<MetaKeyword>>();
            metaKeyWords = metaEtiquetasXMLOntologias.ToDictionary(x => x.OntologiaEnlace, x => x.MetaKeyWords);
            return metaKeyWords;
        }

        [NonAction]
        protected void InsertarMetaEtiquetasXMLOntologiasViewBag()
        {
            Dictionary<string, List<MetaKeyword>> metaKeywords = ObtenerMetaEtiquetasXMLOntologias();
            string json = JsonConvert.SerializeObject(metaKeywords);
            ViewBag.MetaEtiquetasXMLOntologias = json;
        }

        [NonAction]
        private void CargarViewBagBasico()
        {
            InsertarMetaEtiquetasXMLOntologiasViewBag();
            ViewBag.UtilIdiomas = UtilIdiomas;

            ViewBag.BaseUrl = BaseURL;
            ViewBag.BaseUrlIdioma = BaseURLIdioma;
            ViewBag.BaseUrlStatic = BaseURLStatic;
            ViewBag.BaseUrlContent = BaseURLContent;
            ViewBag.BaseUrlPersonalizacion = BaseURLPersonalizacion;
            ViewBag.BaseUrlPersonalizacionEcosistema = BaseURLPersonalizacionEcosistema;
            ViewBag.EsEcosistemaSinMetaProyecto = EsEcosistemaSinMetaProyecto;
            ViewBag.BaseUrlMetaProyecto = UrlPrincipal;
            ViewBag.UrlPagina = UrlPagina;
            ViewBag.UrlComunidadPrincipal = UrlComunidadPrincipal;
            ViewBag.UrlMatomo = UrlMatomo;

            if (!string.IsNullOrEmpty(ViewBag.UrlComunidadPrincipal))
            {
                ViewBag.MainCommunityTabs = MainCommunityTabs;
                ProcesarPestanyasComunidad(ViewBag.MainCommunityTabs, UtilIdiomas.LanguageCode, ParametrosGeneralesRow.IdiomaDefecto);
            }

            //Establecemos el CultureInfo
            CultureInfo cultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;
            switch (UtilIdiomas.LanguageCode)
            {
                case "es":
                    cultureInfo = new CultureInfo("es-ES");
                    break;
                case "en":
                    cultureInfo = new CultureInfo("en-US");
                    break;
                case "eu":
                    cultureInfo = new CultureInfo("eu-ES");
                    break;
                case "gl":
                    cultureInfo = new CultureInfo("gl-ES");
                    break;
                case "ca":
                    cultureInfo = new CultureInfo("ca-ES");
                    break;
                case "it":
                    cultureInfo = new CultureInfo("it-IT");
                    break;
                case "pt":
                    cultureInfo = new CultureInfo("pt-PT");
                    break;
                case "fr":
                    cultureInfo = new CultureInfo("fr-FR");
                    break;
                case "de":
                    cultureInfo = new CultureInfo("de-DE");
                    break;
            }
            System.Threading.Thread.CurrentThread.CurrentCulture = cultureInfo;


            ViewBag.Comunidad = Comunidad;

            ViewBag.ListaCategoriaCookie = ListaPersonalizacionCategoriaCookieModel;
        }

        /// <summary>
        /// Guarda el log de error
        /// </summary>
        /// <param name="pError">Cadena de texto con el error</param>
        [NonAction]
        public void GuardarLogError(string pError)
        {
            mLoggingService.GuardarLogError(pError);
        }

        /// <summary>
        /// Guarda el log de error
        /// </summary>
        /// <param name="pError">Cadena de texto con el error</param>
        [NonAction]
        public void GuardarLogError(Exception pExcepcion)
        {
            mLoggingService.GuardarLogError(pExcepcion);
        }

        /// <summary>
        /// Guarda el log de error
        /// </summary>
        /// <param name="pExcepcion">Excepción que a producido el error</param>
        /// <param name="pMensajeExtra">Mensaje extra para guardar en el log</param>
        [NonAction]
        public void GuardarLogError(Exception pExcepcion, string pMensajeExtra)
        {
            mLoggingService.GuardarLogError(pExcepcion, pMensajeExtra);
        }

        /// <summary>
        /// Guarda el log de error
        /// </summary>
        /// <param name="pError">Cadena de texto con el error</param>
        [NonAction]
        public void GuardarLogErrorContextos(string pError)
        {
            mLoggingService.GuardarLogError(pError);
        }

        /// <summary>
        /// Guarda el log de error
        /// </summary>
        /// <param name="pError">Cadena de texto con el error</param>
        [NonAction]
        public void GuardarLogErrorAJAX(string pError)
        {
            mLoggingService.GuardarLogErrorAJAX(pError);
        }

        /// <summary>
        /// Guarda el log de error
        /// </summary>
        /// <param name="pExcepcion">Excepción que a producido el error</param>
        [NonAction]
        public void GuardarLogErrorAJAX(Exception pExcepcion)
        {
            mLoggingService.GuardarLogError(pExcepcion);
        }

        /// <summary>
        /// Guarda el log de error
        /// </summary>
        /// <param name="pExcepcion">Excepción que a producido el error</param>
        /// <param name="pMensajeExtra">Mensaje extra para guardar en el log</param>
        [NonAction]
        public void GuardarLogErrorAJAX(Exception pExcepcion, string pMensajeExtra)
        {
            mLoggingService.GuardarLogError(pExcepcion, pMensajeExtra);
        }

        [NonAction]
        protected GnossResult GnossResultUrl(string pUrl)
        {
            return new GnossResult(pUrl, mViewEngine);
        }

        [NonAction]
        protected GnossResult GnossResultNoLogin(string pUrl)
        {
            return new GnossResult(pUrl, GnossResult.GnossStatus.NOLOGIN, mViewEngine);
        }

        [NonAction]
        protected GnossResult GnossResultOK()
        {
            return new GnossResult("", GnossResult.GnossStatus.OK, mViewEngine);
        }

        [NonAction]
        protected GnossResult GnossResultOK(string pMessage)
        {
            return new GnossResult(pMessage, GnossResult.GnossStatus.OK, mViewEngine);
        }

        [NonAction]
        protected GnossResult GnossResultERROR()
        {
            return new GnossResult("", GnossResult.GnossStatus.Error, mViewEngine);
        }

        [NonAction]
        protected GnossResult GnossResultERROR(string pMessage)
        {
            return new GnossResult(pMessage, GnossResult.GnossStatus.Error, mViewEngine);
        }

        [NonAction]
        protected GnossResult GnossResultHtml(string pPartialViewName, object pModel)
        {
            Response.ContentType = "text/html; charset=utf-8";
            return new GnossResult(this, pPartialViewName, pModel, mViewEngine);
        }

        /// <summary>
        /// Obtiene el estado de la cookie (Pendiente, Aceptado, No aceptado)
        /// </summary>
        /// <param name="pNombreCorto">Nombre corto de la categoría de la cookie</param>
        /// <returns>Estado de la cookie</returns>
        [NonAction]
        protected short ObtenerEstadoCookie(string pNombreCorto)
        {
            short estado = 0;
            string cookie = Request.Cookies[pNombreCorto];

            if (cookie != null)
            {
                if (cookie.Equals("yes"))
                {
                    estado = 1;
                }
                else if (cookie.Equals("no"))
                {
                    estado = 2;
                }
            }

            return estado;
        }

        [NonAction]
        protected bool TienePersonalizacion()
        {
            bool tienePersonalizacion = false;

            if ((!string.IsNullOrEmpty((string)ViewBag.Personalizacion) || !string.IsNullOrEmpty((string)ViewBag.PersonalizacionEcosistema) || !string.IsNullOrEmpty((string)ViewBag.PersonalizacionDominio)) && Comunidad != null)
            {
                tienePersonalizacion = true;
            }

            return tienePersonalizacion;
        }

        public GnossUrlsSemanticas UrlsSemanticas
        {
            get
            {
                return mControladorBase.UrlsSemanticas;
            }
        }

        [NonAction]
        public string ObtenerNombreVista(string viewName)
        {
            if (TienePersonalizacion())
            {
                string personalizacion = MultiViewResult.ComprobarPersonalizacion(this, viewName);

                viewName += personalizacion;
            }
            return viewName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pFichaComponente"></param>
        /// <returns></returns>
        [NonAction]
        public ActionResult PintarComponenteCMS(CMSComponent pFichaComponente)
        {
            if (pFichaComponente != null)
            {
                ViewBag.UtilIdiomas = UtilIdiomas;
                ViewData.Model = pFichaComponente;

                string resultado = "";
                using (StringWriter sw = new StringWriter())
                {
                    ViewEngineResult viewResult = mViewEngine.FindView(ControllerContext, ObtenerNombreVista("_ComponenteCMS"), false);

                    if (viewResult.View == null)
                        throw new Exception("View not found: _ComponenteCMS");
                    ViewContext viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw, new HtmlHelperOptions());
                    viewResult.View.RenderAsync(viewContext);
                    resultado = sw.GetStringBuilder().ToString();
                    Response.Headers.Add("content-type", "text/html; charset=utf-8");
                }

                return Content(resultado.Trim());
            }
            else
            {
                return Content("El componente que se intenta pintar no existe.");
            }
        }

        public GnossIdentity UsuarioActual
        {
            get
            {
                return mControladorBase.UsuarioActual;
            }
            set
            {
                mControladorBase.UsuarioActual = value;
            }
        }

        protected ChequeoSeguridad ChequeoSeguridad
        {
            get
            {
                if (mChequeoSeguridad == null)
                {
                    mChequeoSeguridad = new ChequeoSeguridad(UsuarioActual);
                }
                return mChequeoSeguridad;
            }
        }

        /// <summary>
        /// Devuelve si el usuario es administrador de la organización relacionada con su identidad
        /// </summary>
        [NonAction]
        public bool EsAdministrador(Identidad pIdentidad)
        {
            return pIdentidad.OrganizacionID.HasValue && ChequeoSeguridad.ComprobarCapacidadEnOrganizacion((ulong)Capacidad.Organizacion.CapacidadesAdministrador.AdministrarOrganizacion, (Guid)pIdentidad.OrganizacionID.Value);
        }

        /// <summary>
        /// Devuelve si el usuario es supervisor del proyecto relacionado con la identidad
        /// </summary>
        [NonAction]
        public bool EsSupervisor(Identidad pIdentidad)
        {
            return ChequeoSeguridad.ComprobarCapacidadEnProyecto((ulong)Capacidad.Proyecto.CapacidadesDocumentacion.SupervisarDocumentos);
        }

        /// <summary>
        /// Comprueba si un usuario es supervisor de la organización relacionada con su identidad.
        /// </summary>
        [NonAction]
        public bool EsSupervisorOrganizacion(Identidad pIdentidad)
        {
            return pIdentidad.OrganizacionID.HasValue && ChequeoSeguridad.ComprobarCapacidadEnOrganizacion((ulong)Capacidad.Organizacion.CapacidadesPropiedades.EditarOrganizacion, (Guid)pIdentidad.OrganizacionID.Value);
        }
        [NonAction]
        protected UserIdentityModel CargarDatosIdentidad(Identidad pIdentidad)
        {
            UserIdentityModel identidad = new UserIdentityModel();

            identidad.KeyIdentity = pIdentidad.Clave;
            identidad.KeyUser = UsuarioActual.UsuarioID;

            if (pIdentidad.IdentidadMyGNOSS != null)
            {
                // En proyectos no hay identidad MyGnoss y esto falla.
                identidad.KeyMetaProyectIdentity = pIdentidad.IdentidadMyGNOSS.Clave;
            }

            identidad.KeyIdentityOrg = Guid.Empty;
            identidad.KeyMetaProyectIdentityOrg = Guid.Empty;
            if (pIdentidad.IdentidadOrganizacion != null)
            {
                identidad.KeyIdentityOrg = pIdentidad.IdentidadOrganizacion.Clave;
                identidad.KeyMetaProyectIdentityOrg = pIdentidad.IdentidadOrganizacion.IdentidadMyGNOSS.Clave;
            }
            identidad.KeyOrganization = pIdentidad.OrganizacionID;
            identidad.KeyProfile = pIdentidad.PerfilID;
            identidad.KeyPerson = pIdentidad.PersonaID.Value;
            identidad.IsGuestIdentity = UsuarioActual.EsIdentidadInvitada;
            identidad.IsGuestUser = UsuarioActual.EsUsuarioInvitado;
            //TODO Alvaro recuperar propiedades
            identidad.IsOrgAdmin = EsAdministrador(pIdentidad);
            identidad.IsOrgSupervisor = EsSupervisorOrganizacion(pIdentidad);
            identidad.IsProyectAdmin = ProyectoSeleccionado.EsAdministradorUsuario(UsuarioActual.UsuarioID);
            identidad.IsProyectSupervisor = EsSupervisor(pIdentidad);
            identidad.IsTeacher = pIdentidad.EsIdentidadProfesor;
            identidad.PersonEmail = pIdentidad.Persona.FilaPersona.Email;
            identidad.PersonName = pIdentidad.Persona.FilaPersona.Nombre;
            identidad.PersonFamilyName = pIdentidad.Persona.FilaPersona.Apellidos;
            identidad.PersonalID = pIdentidad.Persona.FilaPersona.ValorDocumentoAcreditativo;
            identidad.ReceiveNewsletter = pIdentidad.FilaIdentidad.RecibirNewsLetter;
            identidad.BornDate = pIdentidad.Persona.Fecha;

            bool usuarioExpulsadoDeComunidad = false;

            if (pIdentidad.FilaIdentidad.PerfilID != UsuarioAD.Invitado)
            {
                IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                usuarioExpulsadoDeComunidad = identCN.EstaIdentidadExpulsadaDeproyecto(pIdentidad.FilaIdentidad.PerfilID, ProyectoVirtual.Clave);
                identCN.Dispose();
            }

            identidad.IsExpelled = usuarioExpulsadoDeComunidad;

            identidad.PermitEditAllPeople = UsuarioActual.EstaAutorizado((ulong)Capacidad.General.CapacidadesPersonas.EditarTODASpersonas);

            identidad.PermitEditAllOrganizations = UsuarioActual.EstaAutorizado((ulong)Capacidad.General.CapacidadesOrganizacion.EditarTODASorganizaciones);

            identidad.PermitEditAllCommunities = UsuarioActual.EstaAutorizado((ulong)Capacidad.General.CapacidadesProyectos.EditarTODOSproyectos);

            // Si es una identidad invitada, identificar si tiene una solicitud pendiente 
            identidad.CommunityRequestStatus = UserIdentityModel.CommunityRequestStatusEnum.NoRequest;

            return identidad;
        }

        [NonAction]
        protected new virtual PartialViewResult PartialView(string viewName, object model)
        {
            if (TienePersonalizacion())
            {
                string personalizacion = MultiViewResult.ComprobarPersonalizacion(this, viewName);

                return base.PartialView(viewName + personalizacion, model);
            }
            return base.PartialView(viewName, model);
        }

        private List<CommunityModel.TabModel> CargarPestanyas(List<ProyectoPestanyaMenu> pPestanyas)
        {
            List<CommunityModel.TabModel> listaPestanyas = new List<CommunityModel.TabModel>();
            foreach (ProyectoPestanyaMenu filaPestanya in pPestanyas)
            {
                CommunityModel.TabModel pestanya = new CommunityModel.TabModel();
                pestanya.Active = false;
                pestanya.Key = filaPestanya.PestanyaID;
                pestanya.Visible = filaPestanya.Visible;

                KeyValuePair<string, string> nameUrl = ObtenerNameUrlPestanya(filaPestanya);
                pestanya.Name = nameUrl.Key;
                pestanya.Url = nameUrl.Value;
                pestanya.OpenInNewWindow = filaPestanya.NuevaPestanya;

                List<ProyectoPestanyaMenu> proyectosPestanyaMenu = ProyectoVirtual.GestorProyectos.DataWrapperProyectos.ListaProyectoPestanyaMenu.Where(proyPestanyaMenu => proyPestanyaMenu.PestanyaPadreID.Equals(filaPestanya.PestanyaID)).ToList();
                if (proyectosPestanyaMenu.Count > 0)
                {
                    proyectosPestanyaMenu = proyectosPestanyaMenu.OrderBy(proyPestanyaMenu => proyPestanyaMenu.Orden).ToList();
                    pestanya.SubTab = CargarPestanyas(proyectosPestanyaMenu);
                }

                listaPestanyas.Add(pestanya);
            }
            return listaPestanyas;
        }

        private KeyValuePair<string, string> ObtenerNameUrlPestanya(ProyectoPestanyaMenu pFilaPestanya, bool pObtenerMultiIdioma = false)
        {
            KeyValuePair<string, string> nameUrl = new KeyValuePair<string, string>();
            if (pFilaPestanya != null)
            {
                string name = string.Empty;
                string url = string.Empty;

                if (!pObtenerMultiIdioma)
                {
                    ObtenerNameUrlPestanyaPorTipo((TipoPestanyaMenu)pFilaPestanya.TipoPestanya, out name, out url);
                }
                else
                {
                    ObtenerNameUrlMultiIdiomaPestanyaPorTipo((TipoPestanyaMenu)pFilaPestanya.TipoPestanya, out name, out url);
                }

                if (!string.IsNullOrEmpty(pFilaPestanya.Nombre))
                {
                    name = pFilaPestanya.Nombre;
                }
                if (!string.IsNullOrEmpty(pFilaPestanya.Ruta))
                {
                    url = pFilaPestanya.Ruta;
                    if (pFilaPestanya.TipoPestanya == (short)TipoPestanyaMenu.EnlaceExterno)
                    {
                        url = "enlace_externo_" + url;
                    }
                }
                if (string.IsNullOrEmpty(url))
                {
                    url = "";
                }

                if (!string.IsNullOrEmpty(name) || !string.IsNullOrEmpty(url))
                {
                    nameUrl = new KeyValuePair<string, string>(name, url);
                }
            }
            return nameUrl;
        }

        private void ObtenerNameUrlMultiIdiomaPestanyaPorTipo(TipoPestanyaMenu pTipoPestanya, out string pNombre, out string pUrl)
        {
            ControladorPestanyas.ObtenerNameUrlMultiIdiomaPestanyaPorTipo(pTipoPestanya, out pNombre, out pUrl);
        }

        protected ControladorPestanyas ControladorPestanyas
        {
            get
            {
                if (mControladorPestanyas == null)
                {
                    mControladorPestanyas = new ControladorPestanyas(ProyectoSeleccionado, new Dictionary<string, string>(), mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mEntityContextBASE);
                }
                return mControladorPestanyas;
            }
        }

        private void ObtenerNameUrlPestanyaPorTipo(TipoPestanyaMenu pTipoPestanya, out string pNombre, out string pUrl)
        {
            pNombre = string.Empty;
            pUrl = string.Empty;

            switch (pTipoPestanya)
            {
                case TipoPestanyaMenu.Home:
                    pNombre = UtilIdiomas.GetText("COMMON", "HOME");
                    break;
                case TipoPestanyaMenu.Indice:
                    pNombre = UtilIdiomas.GetText("COMMON", "INDICE");
                    pUrl = UtilIdiomas.GetText("URLSEM", "INDICE");
                    break;
                case TipoPestanyaMenu.Recursos:
                    pNombre = UtilIdiomas.GetText("COMMON", "BASERECURSOS");
                    pUrl = UtilIdiomas.GetText("URLSEM", "RECURSOS");
                    break;
                case TipoPestanyaMenu.Preguntas:
                    pNombre = UtilIdiomas.GetText("COMMON", "PREGUNTAS");
                    pUrl = UtilIdiomas.GetText("URLSEM", "PREGUNTAS");
                    break;
                case TipoPestanyaMenu.Debates:
                    pNombre = UtilIdiomas.GetText("COMMON", "DEBATES");
                    pUrl = UtilIdiomas.GetText("URLSEM", "DEBATES");
                    break;
                case TipoPestanyaMenu.Encuestas:
                    pNombre = UtilIdiomas.GetText("COMMON", "ENCUESTAS");
                    pUrl = UtilIdiomas.GetText("URLSEM", "ENCUESTAS");
                    break;
                case TipoPestanyaMenu.PersonasYOrganizaciones:
                    pNombre = UtilIdiomas.GetText("COMMON", "PERSONASYORGANIZACIONES");
                    if (ProyectoVirtual.TipoProyecto == TipoProyecto.EducacionExpandida || ProyectoVirtual.TipoProyecto == TipoProyecto.Universidad20 || ProyectoVirtual.TipoProyecto == TipoProyecto.EducacionPrimaria)
                    {
                        pNombre = UtilIdiomas.GetText("COMMON", "PROFESORESYALUMNOS");
                    }
                    pUrl = UtilIdiomas.GetText("URLSEM", "PERSONASYORGANIZACIONES");
                    break;
                case TipoPestanyaMenu.AcercaDe:
                    pNombre = UtilIdiomas.GetText("COMMON", "ACERCADE");
                    pUrl = UtilIdiomas.GetText("URLSEM", "ACERCADE");
                    break;
                case TipoPestanyaMenu.Comunidades:
                    pNombre = UtilIdiomas.GetText("COMMON", "COMUNIDADES");
                    pUrl = UtilIdiomas.GetText("URLSEM", "COMUNIDADES");
                    break;
                case TipoPestanyaMenu.BusquedaAvanzada:
                    pNombre = UtilIdiomas.GetText("BUSCADORFACETADO", "TODALACOMUNIDAD");
                    pUrl = UtilIdiomas.GetText("URLSEM", "BUSQUEDAAVANZADA");
                    break;
            }
        }

        private void ProcesarPestanyasComunidad(List<CommunityModel.TabModel> pListaPestanyas, string pLanguageCode, string pIdiomaDefecto)
        {
            ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            ParametroAplicacionCN parametroAplicacionCN = new ParametroAplicacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

            string proyectoID = parametroAplicacionCN.ObtenerParametroAplicacion("ComunidadPrincipalID");
            string nombreCortoComunidadPrincipal = proyectoCN.ObtenerNombreCortoProyecto(new Guid(proyectoID));

            List<CommunityModel.TabModel> pestanyasAEliminar = new List<CommunityModel.TabModel>();
            foreach (CommunityModel.TabModel pestanya in pListaPestanyas)
            {
                if (pestanya.Key != Guid.Empty)
                {
                    pestanya.Name = UtilCadenas.ObtenerTextoDeIdioma(pestanya.Name, UtilIdiomas.LanguageCode, ParametrosGeneralesRow.IdiomaDefecto);
                    bool externo = false;

                    if (pestanya.Url.StartsWith("enlace_externo_"))
                    {
                        pestanya.Url = pestanya.Url.Replace("enlace_externo_", "");
                        externo = true;
                    }

                    pestanya.Url = UtilCadenas.ObtenerTextoDeIdioma(pestanya.Url, UtilIdiomas.LanguageCode, ParametrosGeneralesRow.IdiomaDefecto);

                    if (!externo)
                    {
                        string urlComunidad = GnossUrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, nombreCortoComunidadPrincipal);
                        if (!string.IsNullOrEmpty(pestanya.Url))
                        {
                            pestanya.Url = urlComunidad + "/" + pestanya.Url;
                        }
                        else
                        {
                            pestanya.Url = urlComunidad;
                        }
                    }

                    if (pestanya.SubTab != null)
                    {
                        ProcesarPestanyasComunidad(pestanya.SubTab, pLanguageCode, pIdiomaDefecto);
                    }
                }
            }

            foreach (CommunityModel.TabModel pestaña in pestanyasAEliminar)
            {
                pListaPestanyas.Remove(pestaña);
            }
        }

        #endregion

        #region Propiedades

        public GnossUrlsSemanticas GnossUrlsSemanticas
        {
            get
            {
                return mControladorBase.UrlsSemanticas;
            }
        }

        public Guid PersonalizacionProyecto
        {
            get { return mPersonalizacionProyecto; }
            set { mPersonalizacionProyecto = value; }
        }

        public CommunityModel Comunidad
        {
            get
            {
                if (mComunidad == null)
                {
                    mComunidad = CargarDatosComunidad();
                }
                return mComunidad;
            }
        }

        public List<PersonalizacionCategoriaCookieModel> ListaPersonalizacionCategoriaCookieModel
        {
            get
            {
                if (mListaPersonalizacionCategoriaCookieModel == null || mListaPersonalizacionCategoriaCookieModel.Count == 0)
                {
                    mListaPersonalizacionCategoriaCookieModel = new List<PersonalizacionCategoriaCookieModel>();

                    CookieCL cookieCL = new CookieCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                    List<CategoriaProyectoCookieViewModel> listaCategoriasCookies = cookieCL.ObtenerCategoriasProyectoCookie(ProyectoSeleccionado.Clave);
                    if (listaCategoriasCookies.Count == 0)
                    {
                        listaCategoriasCookies = cookieCL.ObtenerCategoriasProyectoCookie(ProyectoAD.MetaProyecto);
                    }

                    foreach (CategoriaProyectoCookieViewModel categoriaCookie in listaCategoriasCookies)
                    {
                        PersonalizacionCategoriaCookieModel personalizacionCategoriaCookie = new PersonalizacionCategoriaCookieModel();
                        categoriaCookie.Nombre = UtilCadenas.ObtenerTextoDeIdioma(categoriaCookie.Nombre, UtilIdiomas.LanguageCode, "es");
                        categoriaCookie.Descripcion = UtilCadenas.ObtenerTextoDeIdioma(categoriaCookie.Descripcion, UtilIdiomas.LanguageCode, "es");
                        personalizacionCategoriaCookie.CategoriaCookie = categoriaCookie;
                        personalizacionCategoriaCookie.Estado = ObtenerEstadoCookie(categoriaCookie.NombreCorto.ToLower());

                        mListaPersonalizacionCategoriaCookieModel.Add(personalizacionCategoriaCookie);
                    }
                }

                return mListaPersonalizacionCategoriaCookieModel;
            }
        }

        /// <summary>
        /// Obtiene el proyecto seleccionado
        /// </summary>
        public Elementos.ServiciosGenerales.Proyecto ProyectoSeleccionado
        {
            get
            {
                return mControladorBase.ProyectoSeleccionado;
            }
        }

        public string EntornoIntegracionContinua
        {
            get
            {
                if (mEntornoIntegracionContinua == null)
                {
                    ParametroAplicacion filaParametro = ParametrosAplicacionDS.FirstOrDefault(parametro => parametro.Parametro.Equals(TiposParametrosAplicacion.EntornoIntegracionContinua));

                    if (filaParametro != null)
                    {
                        mEntornoIntegracionContinua = filaParametro.Valor;
                    }
                }

                return mEntornoIntegracionContinua;
            }
        }

        /// <summary>
        /// Si el ecosistema esta personalizado, obtiene el enlace del proyecto configurado
        /// </summary>
        public string UrlComunidadPrincipal
        {
            get
            {
                ParametroAplicacionCN parametroAplicacionCN = new ParametroAplicacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                string proyectoID = parametroAplicacionCN.ObtenerParametroAplicacion("ComunidadPrincipalID");

                if (!string.IsNullOrEmpty(proyectoID))
                {
                    ProyectoAD proyectoAD = new ProyectoAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                    return proyectoAD.ObtenerURLPropiaProyecto(new Guid(proyectoID));
                }
                else
                {
                    return "";
                }
            }
        }

        public string UrlMatomo
        {
            get
            {
                return mConfigService.ObtenerUrlMatomo();
            }
        }

        public bool IrAConfiguracionInicial
        {
            get
            {
                return mControladorBase.IrAConfiguracionInicial;
            }
            set
            {
                mControladorBase.IrAConfiguracionInicial = value;
            }
        }

        public List<CommunityModel.TabModel> MainCommunityTabs
        {
            get
            {
                ParametroAplicacionCN parametroAplicacionCN = new ParametroAplicacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                string proyectoID = parametroAplicacionCN.ObtenerParametroAplicacion("ComunidadPrincipalID");

                ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

                List<ProyectoPestanyaMenu> listaProyectoPestanyaMenu = proyectoCN.ObtenerPestanyasDeProyectoSegunPrivacidadDeIdentidad(new Guid(proyectoID), IdentidadActual.Clave);

                return CargarPestanyas(listaProyectoPestanyaMenu.Where(proyPestanyaMenu => proyPestanyaMenu.PestanyaPadreID == null).OrderBy(proyPestanyaMenu => proyPestanyaMenu.Orden).ToList());
            }
        }

        public bool ComprobarRepetidos
        {
            get
            {
                if (!mComprobarRepetidos.HasValue)
                {
                    try
                    {
                        ParametroAplicacion filaParametro = ParametrosAplicacionDS.FirstOrDefault(parametro => parametro.Parametro.Equals(TiposParametrosAplicacion.ComprobarRepetidos));
                        mComprobarRepetidos = filaParametro != null && bool.Parse(filaParametro.Valor);
                    }
                    catch
                    {
                        mComprobarRepetidos = true;
                    }
                }

                return mComprobarRepetidos.Value;
            }
        }

        public string NombrePlataforma
        {
            get
            {
                if (mNombrePlataforma == null)
                {
                    ParametroAplicacion filaParametro = ParametrosAplicacionDS.FirstOrDefault(parametro => parametro.Parametro.Equals(TiposParametrosAplicacion.NombrePlataforma));
                    if (filaParametro != null)
                    {
                        mNombrePlataforma = filaParametro.Valor;
                    }
                }
                return mNombrePlataforma;
            }
        }

        public string ProyectoConIntegracionContinua
        {
            get
            {
                if (mProyectoConIntegracionContinua == null)
                {
                    using (ProyectoCL proyectoCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication))
                    {

                        bool? activada = proyectoCL.TieneICactivada(ProyectoSeleccionado.Clave);
                        if (activada == null)
                        {
                            bool iniciado = false;
                            try
                            {
                                if (!string.IsNullOrEmpty(UrlApiIntegracionContinua))
                                {
                                    if (ProyectoSeleccionado.NombreCorto == "")
                                    {
                                        iniciado = mUtilServicioIntegracionContinua.EstaEnBD("mygnoss", UrlApiIntegracionContinua);
                                    }
                                    else
                                    {
                                        iniciado = mUtilServicioIntegracionContinua.EstaEnBD(ProyectoSeleccionado.NombreCorto, UrlApiIntegracionContinua);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                //mLoggingService.GuardarLogError(ex, "Problema al obtener el Proyecto con IC");
                            }

                            if (iniciado)
                            {
                                mProyectoConIntegracionContinua = "inciado";
                                proyectoCL.AgregarIC(ProyectoSeleccionado.Clave, true, false);
                            }
                        }
                        else
                        {
                            if (activada.HasValue && activada.Value)
                            {
                                mProyectoConIntegracionContinua = "inciado";
                            }
                            else
                            {
                                mProyectoConIntegracionContinua = "";
                            }
                        }
                    }
                }
                return mProyectoConIntegracionContinua;
            }
        }

        public string RamaEnUsoGit
        {
            get
            {
                if (string.IsNullOrEmpty(mRamaGit))
                {
                    string nombreRama = "";
                    try
                    {
                        if (!string.IsNullOrEmpty(UrlApiIntegracionContinua) && !string.IsNullOrEmpty(EntornoIntegracionContinua))
                        {
                            if (ProyectoSeleccionado.NombreCorto == "")
                            {
                                nombreRama = mUtilServicioIntegracionContinua.RamaEnUso("mygnoss", UrlApiIntegracionContinua);
                            }
                            else
                            {
                                nombreRama = mUtilServicioIntegracionContinua.RamaEnUso(ProyectoSeleccionado.NombreCorto, UrlApiIntegracionContinua);
                            }
                        }
                    }
                    catch
                    {
                        //mLoggingService.GuardarLogError(ex, "Problema al obtener el Proyecto con IC");
                    }
                    mRamaGit = nombreRama;
                }
                return mRamaGit;
            }
        }

        public string VersionRama
        {
            get
            {
                string auxVersion = "";
                string auxNombre = RamaEnUsoGit;

                if (!string.IsNullOrEmpty(auxNombre) && auxNombre.Contains("release_"))
                {
                    auxVersion = auxNombre.Replace("release_", "");
                }

                return auxVersion;
            }
        }

        public bool UsuarioEstaDadoDeAltaEnIntCont
        {
            get
            {
                try
                {
                    bool estaRegistrado = mUtilServicioIntegracionContinua.EstaLogeado(ProyectoSeleccionado.NombreCorto, UsuarioActual.UsuarioID, UrlApiIntegracionContinua);

                    return estaRegistrado;
                }
                catch
                {
                    return true;
                }
            }
        }

        public bool EntornoActualEsPruebas
        {
            get
            {
                try
                {
                    using (ProyectoCL proyectoCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication))
                    {
                        bool? pruebas = proyectoCL.EsEntornoPruebas(ProyectoSeleccionado.Clave, EntornoIntegracionContinua);
                        if (pruebas == null)
                        {
                            bool esPruebas = mUtilServicioIntegracionContinua.EsPruebas(EntornoIntegracionContinua, UrlApiIntegracionContinua);
                            if (esPruebas)
                            {
                                proyectoCL.AgregarEsEntornoPruebas(ProyectoSeleccionado.Clave, EntornoIntegracionContinua);
                            }
                            else
                            {
                                proyectoCL.AgregarEsEntornoPruebas(ProyectoSeleccionado.Clave, EntornoIntegracionContinua, false);
                            }
                            return esPruebas;
                        }
                        else
                        {
                            return pruebas.Value;
                        }

                    }
                }
                catch
                {
                    return false;
                }
            }
        }

        public bool EntornoActualEsPreproduccion
        {
            get
            {
                try
                {
                    using (ProyectoCL proyectoCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication))
                    {
                        bool? pruebas = proyectoCL.EsEntornoPreproduccion(ProyectoSeleccionado.Clave, EntornoIntegracionContinua);
                        if (pruebas == null)
                        {
                            bool esPruebas = mUtilServicioIntegracionContinua.EsPreproduccion(EntornoIntegracionContinua, UrlApiIntegracionContinua);
                            if (esPruebas)
                            {
                                proyectoCL.AgregarEsEntornoPreproduccion(ProyectoSeleccionado.Clave, EntornoIntegracionContinua);
                            }
                            else
                            {
                                proyectoCL.AgregarEsEntornoPreproduccion(ProyectoSeleccionado.Clave, EntornoIntegracionContinua, false);
                            }
                            return esPruebas;
                        }
                        else
                        {
                            return pruebas.Value;
                        }

                    }
                }
                catch
                {
                    return false;
                }
            }
        }

        public bool EntornoActualBloqueado
        {
            get
            {
                try
                {
                    bool entornoBloqueado = mUtilServicioIntegracionContinua.EstaEntornoBloqueado(ProyectoSeleccionado.Clave, EntornoIntegracionContinua, UrlApiIntegracionContinua);
                    return entornoBloqueado;
                }
                catch
                {
                    return false;
                }
            }
        }

        public bool HayIntegracionContinua
        {
            get
            {
                return !string.IsNullOrEmpty(ProyectoConIntegracionContinua);
            }
        }

        public string UrlApiIntegracionContinua
        {
            get
            {
                return mConfigService.ObtenerUrlApiIntegracionContinua();
            }
        }

        public string UrlApiDespliegues
        {
            get
            {
                if (mUrlApiDespliegues == null)
                {
                    mUrlApiDespliegues = mConfigService.ObtenerUrlApiDesplieguesEntorno();
                    if (mUrlApiDespliegues == null)
                    {
                        mUrlApiDespliegues = mUtilServicioIntegracionContinua.ObtenerUrlApiDesplieguesEntornoActual(ProyectoSeleccionado.Clave, EntornoIntegracionContinua, UrlApiIntegracionContinua, UsuarioActual.UsuarioID);
                    }
                }
                return mUrlApiDespliegues;
            }
        }

        public string IPFTP
        {
            get
            {
                if (mIPFTP == null)
                {
                    ParametroAplicacion filaParametro = ParametrosAplicacionDS.FirstOrDefault(parametro => parametro.Parametro.Equals("ipFTP"));
                    if (filaParametro != null)
                    {
                        mIPFTP = filaParametro.Valor;
                    }
                }
                return mIPFTP;
            }
        }

        public string PuertoFTP
        {
            get
            {
                if (mPuertoFTP == null)
                {
                    ParametroAplicacion filaParametro = ParametrosAplicacionDS.FirstOrDefault(parametro => parametro.Parametro.Equals("puertoFTP"));
                    if (filaParametro != null)
                    {
                        mPuertoFTP = filaParametro.Valor;
                    }
                }
                return mPuertoFTP;
            }
        }


        public string UrlApiDesplieguesEntornoAnterior
        {
            get
            {
                if (mUrlApiDesplieguesEntornoAnterior == null)
                {
                    if (!string.IsNullOrEmpty(Request.Query["EntornoOrigen"]))
                    {
                        mUrlApiDesplieguesEntornoAnterior = mUtilServicioIntegracionContinua.ObtenerUrlApiDesplieguesEntornoParametro(ProyectoSeleccionado.Clave, EntornoIntegracionContinua, UrlApiIntegracionContinua, UsuarioActual.UsuarioID, Request.Query["EntornoOrigen"]);
                    }
                    else
                    {
                        mUrlApiDesplieguesEntornoAnterior = mUtilServicioIntegracionContinua.ObtenerUrlApiDesplieguesEntornoAnterior(ProyectoSeleccionado.Clave, EntornoIntegracionContinua, UrlApiIntegracionContinua, UsuarioActual.UsuarioID);
                    }
                }
                return mUrlApiDesplieguesEntornoAnterior;
            }
        }

        public string UrlApiDesplieguesEntornoSiguiente
        {
            get
            {
                if (mUrlApiDesplieguesEntornoSiguiente == null)
                {
                    mUrlApiDesplieguesEntornoSiguiente = mUtilServicioIntegracionContinua.ObtenerUrlApiDesplieguesEntornoSiguiente(ProyectoSeleccionado.Clave, EntornoIntegracionContinua, UrlApiIntegracionContinua, UsuarioActual.UsuarioID);
                }
                return mUrlApiDesplieguesEntornoSiguiente;
            }
        }
        [NonAction]
        public string UrlApiEntornoSeleccionado(string pNombreEntorno)
        {
            try
            {
                string urlEnvironment = mUtilServicioIntegracionContinua.ObtenerUrlApiDesplieguesEntorno(pNombreEntorno, UrlApiIntegracionContinua);

                if (!string.IsNullOrEmpty(urlEnvironment))
                {
                    return urlEnvironment;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// ID del proyecto externo en el que se está haciendo la búsqueda, o GUID.EMPTY si se hace en el proyecto actual.
        /// </summary>
        public Guid ProyectoOrigenBusquedaID
        {
            get
            {
                return mControladorBase.ProyectoOrigenBusquedaID;
            }
        }

        /// <summary>
        /// Obtiene el proyecto principal de un ecosistema sin metaproyecto
        /// </summary>
        public Guid ProyectoPrincipalUnico
        {
            get
            {
                return mControladorBase.ProyectoPrincipalUnico;
            }
        }

        /// <summary>
        /// Obtiene el proyecto virtual (Para cuando es un ecosistema sin metaproyecto)
        /// </summary>
        public Elementos.ServiciosGenerales.Proyecto ProyectoVirtual
        {
            get
            {
                return mControladorBase.ProyectoVirtual;
            }
        }

        public UtilIdiomas UtilIdiomas
        {
            get
            {
                return mControladorBase.UtilIdiomas;
            }
            set
            {
                mControladorBase.UtilIdiomas = value;
            }
        }

        /// <summary>
        /// Obtiene el dataset de parámetros generales del proyectoVirtual
        /// </summary>
        public ParametroGeneral ParametrosGeneralesVirtualRow
        {
            get
            {
                return mControladorBase.ParametrosGeneralesVirtualRow;
            }
        }

        /// <summary>
        /// Obtiene el dataset de parámetros generales
        /// </summary>
        public ParametroGeneral ParametrosGeneralesRow
        {
            get
            {
                return mControladorBase.ParametrosGeneralesRow;
            }
        }

        /// <summary>
        /// Indica si debe usar la personalizacion de la comunidad
        /// </summary>
        public bool UsarPersonalizacionComunidad
        {
            get { return mUsarPersonalizacionComunidad; }
            set { mUsarPersonalizacionComunidad = value; }
        }

        /// <summary>
        /// Indica si debe usar la personalizacion de la administración de la comunidad
        /// </summary>
        public bool UsarPersonalizacionAdministracionComunidad
        {
            get { return mUsarPersonalizacionAdministracionComunidad; }
            set { mUsarPersonalizacionAdministracionComunidad = value; }
        }

        /// <summary>
        /// Obtiene la dirección base
        /// </summary>
        public string BaseURLSinHTTP
        {
            get
            {
                return mControladorBase.BaseURLSinHTTP;
            }
        }

        /// <summary>
        /// Obtiene la URL base de la página
        /// </summary>
        public string BaseURL
        {
            get
            {
                return mControladorBase.BaseURL;
            }
        }

        /// <summary>
        /// Obtiene la URL base de la página en el idioma correspondiente
        /// </summary>
        public string BaseURLIdioma
        {
            get
            {
                return mControladorBase.BaseURLIdioma;
            }
        }

        /// <summary>
        /// Obtiene la URL principal de esta aplicación (ej: para http://didactalia.net el dominio principal es http://gnoss.com)
        /// </summary>
        public string UrlPrincipal
        {
            get
            {
                return mControladorBase.UrlPrincipal;
            }
        }

        /// <summary>
        /// Obtiene la URL del los elementos estaticos de la página
        /// </summary>
        public string BaseURLStatic
        {
            get
            {
                return mControladorBase.BaseURLStatic;
            }
        }

        /// <summary>
        /// BaseURL Content
        /// </summary>
        private string mBaseURLContent = null;

        /// <summary>
        /// Obtiene la URL del los elementos de contenido de la página
        /// </summary>
        public string BaseURLContent
        {
            get
            {
                if (mBaseURLContent == null)
                {
                    string dominio = ProyectoSeleccionado.UrlPropia(UtilIdiomas.LanguageCode).Replace("http://", "").Replace("https://", "");
                    mBaseURLContent = UrlContent;

                    if (mBaseURLContent == "")
                    {
                        mBaseURLContent = BaseURL;
                    }
                }
                return mBaseURLContent;
            }
        }


        /// <summary>
        /// URL del los elementos de contenido de la página
        /// </summary>
        public string UrlContent
        {
            get
            {
                if (mUrlContent == null)
                {
                    mUrlContent = mConfigService.ObtenerUrlContent();
                }
                return mUrlContent;
            }
        }



        /// <summary>
        /// Obtiene la URL del los elementos de contenido de la página
        /// </summary>
        public string BaseURLPersonalizacion
        {
            get
            {
                return mControladorBase.BaseURLPersonalizacion;
            }
        }

        /// <summary>
        /// Obtiene la URL del los elementos de contenido de la página
        /// </summary>
        public string BaseURLPersonalizacionEcosistema
        {
            get
            {
                return mControladorBase.BaseURLPersonalizacionEcosistema;
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
        /// 
        /// </summary>
        protected string IdiomaPorDefecto
        {
            get
            {
                if (mIdiomaPorDefecto == null)
                {
					ParametroAplicacionCL paramCL = new ParametroAplicacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
					mIdiomaPorDefecto = !(ParametrosGeneralesRow.IdiomaDefecto == null) && paramCL.ObtenerListaIdiomas().Contains(ParametrosGeneralesRow.IdiomaDefecto) ? ParametrosGeneralesRow.IdiomaDefecto : paramCL.ObtenerListaIdiomas().FirstOrDefault();
                }
                return mIdiomaPorDefecto;
            }
        }

        /// <summary>
        /// Obtiene el código del idioma sólo si es distinto del idioma por defecto, si es el idioma por defecto devuelve vacío
        /// </summary>
        protected string IdiomaUsuarioDistintoPorDefecto
        {
            get
            {
                if (UtilIdiomas.LanguageCode == IdiomaPorDefecto)
                {
                    return "";
                }
                else
                {
                    return "/" + UtilIdiomas.LanguageCode;
                }
            }
        }

        public string UrlIntragnoss
        {
            get
            {
                return mControladorBase.UrlIntragnoss;
            }
        }

        /// <summary>
        /// Obtiene la versión de la aplicación
        /// </summary>
        public string Version
        {
            get
            {
                return mControladorBase.Version;
            }
        }

        /// <summary>
        /// Obtiene la lista de items extra que se obtendrá de la búsqueda y su prefijo (recetas, peliculas, etc)
        /// </summary>
        public Dictionary<string, List<string>> InformacionOntologias
        {
            get
            {
                return mControladorBase.InformacionOntologias;
            }
        }

        /// <summary>
        /// Obtiene si se trata de un ecosistema sin metaproyecto
        /// </summary>
        public bool EsEcosistemaSinMetaProyecto
        {
            get
            {
                return mControladorBase.EsEcosistemaSinMetaProyecto;
            }
        }

        /// <summary>
        /// Parámetros de un proyecto.
        /// </summary>
        public string UrlPagina
        {
            get
            {
                if (mUrlPagina == null)
                {
                    mUrlPagina = Request.Path;
                    if (mUrlPagina.IndexOf("?") > 0)
                    {
                        mUrlPagina = mUrlPagina.Substring(0, mUrlPagina.IndexOf("?"));
                    }
                    if (mUrlPagina.IndexOf("#") > 0)
                    {
                        mUrlPagina = mUrlPagina.Substring(0, mUrlPagina.IndexOf("#"));
                    }
                }
                return mUrlPagina;
            }
        }

        /// <summary>
        /// Parámetros de un proyecto.
        /// </summary>
        public Dictionary<string, string> ParametroProyecto
        {
            get
            {
                if (mParametroProyecto == null)
                {
                    ProyectoCL proyectoCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                    mParametroProyecto = proyectoCL.ObtenerParametrosProyecto(ProyectoSeleccionado.Clave);
                    proyectoCL.Dispose();
                }

                return mParametroProyecto;
            }
        }

        public bool ExisteNombrePoliticaCookiesMetaproyecto
        {
            get
            {
                if (!mExisteNombrePoliticaCookiesMetaproyecto)
                {
                    ParametroCN parametroCN = new ParametroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    mExisteNombrePoliticaCookiesMetaproyecto = parametroCN.ExisteNombrePoliticaCookiesMetaproyecto();
                }
                return mExisteNombrePoliticaCookiesMetaproyecto;
            }
        }

        public string NombrePoliticaCookiesMetaproyecto
        {
            get
            {
                if (string.IsNullOrEmpty(mNombrePoliticaCookiesMetaproyecto))
                {
                    ParametroCN parametroCN = new ParametroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    mNombrePoliticaCookiesMetaproyecto = parametroCN.ObtenerNombrePoliticaCookiesMetaproyecto();
                }
                return mNombrePoliticaCookiesMetaproyecto;
            }
        }

        /// <summary>
        /// Parámetros de un proyecto.
        /// </summary>
        public Dictionary<string, string> ParametroProyectoEcosistema
        {
            get
            {
                if (mParametroProyectoEcosistema == null)
                {
                    ProyectoCL proyectoCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                    mParametroProyectoEcosistema = proyectoCL.ObtenerParametrosProyecto(ProyectoAD.MetaProyecto);
                    proyectoCL.Dispose();
                }

                return mParametroProyectoEcosistema;
            }
        }

        /// <summary>
        /// Parámetros de un proyecto.
        /// </summary>
        public Dictionary<string, string> ParametroProyectoVirtual
        {
            get
            {
                if (mParametroProyectoVirtual == null)
                {
                    ProyectoCL proyectoCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                    mParametroProyectoVirtual = proyectoCL.ObtenerParametrosProyecto(ProyectoVirtual.Clave);
                    proyectoCL.Dispose();
                }

                return mParametroProyectoVirtual;
            }
        }

        /// <summary>
        /// Obtiene el dataset de parámetros de aplicación
        /// </summary>
        //public ParametroAplicacionDS ParametrosAplicacionDS
        public List<ParametroAplicacion> ParametrosAplicacionDS
        {
            get
            {
                return mControladorBase.ListaParametrosAplicacion;
            }
        }

        public GestorParametroAplicacion GestorParametroAplicacion
        {
            get
            {
                return mControladorBase.GestorParametrosAplicacion;
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
        /// Devuelve si el usuario que está navegando es un bot
        /// </summary>
        public bool EsBot
        {
            get
            {
                return mControladorBase.EsBot;
            }
        }

        /// <summary>
        /// Obtiene la identidad del usuario actual
        /// </summary>
        public Identidad IdentidadActual
        {
            get
            {
                return mControladorBase.IdentidadActual;
            }
            set
            {
                mControladorBase.IdentidadActual = value;
            }
        }

        /// <summary>
        /// Lista de grupo a los que pertenece la identidad actual
        /// </summary>
        public List<Guid> ListaGruposIdentidadActual
        {
            get
            {
                if (mListaGruposIdentidadActual == null && UsuarioActual != null)
                {
                    mListaGruposIdentidadActual = new List<Guid>();
                    if (IdentidadActual != null && !UsuarioActual.EsIdentidadInvitada)
                    {
                        IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                        GestionIdentidades gestorIdentidades = new GestionIdentidades(identidadCN.ObtenerGruposParticipaIdentidad(IdentidadActual.Clave, true), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                        if (IdentidadActual.IdentidadMyGNOSS != null && (IdentidadActual.ModoParticipacion == TiposIdentidad.ProfesionalCorporativo || IdentidadActual.ModoParticipacion == TiposIdentidad.ProfesionalPersonal))
                        {
                            gestorIdentidades.DataWrapperIdentidad.Merge(identidadCN.ObtenerGruposParticipaIdentidad(IdentidadActual.IdentidadMyGNOSS.Clave, true));
                        }
                        identidadCN.Dispose();
                        gestorIdentidades.CargarGrupos();

                        foreach (Guid idGrupo in gestorIdentidades.ListaGrupos.Keys)
                        {
                            mListaGruposIdentidadActual.Add(idGrupo);
                        }
                    }
                }
                return mListaGruposIdentidadActual;
            }
        }

        /// <summary>
        /// Lista de grupos a los que pertenece el perfil en el proyecto virtual
        /// </summary>
        public List<Guid> ListaGruposPerfilEnProyectoVirtual
        {
            get
            {
                if (mListaGruposPerfilEnProyectoVirtual == null)
                {
                    mListaGruposPerfilEnProyectoVirtual = new List<Guid>();
                    if (IdentidadActual != null && !UsuarioActual.EsIdentidadInvitada)
                    {
                        IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                        if (IdentidadActual.ListaProyectosPerfilActual.ContainsKey(ProyectoVirtual.Clave))
                        {
                            GestionIdentidades gestorIdentidades = new GestionIdentidades(identidadCN.ObtenerGruposParticipaIdentidad(IdentidadActual.ListaProyectosPerfilActual[ProyectoVirtual.Clave], true), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                            if (IdentidadActual.IdentidadMyGNOSS != null && (IdentidadActual.ModoParticipacion == TiposIdentidad.ProfesionalCorporativo || IdentidadActual.ModoParticipacion == TiposIdentidad.ProfesionalPersonal))
                            {
                                gestorIdentidades.DataWrapperIdentidad.Merge(identidadCN.ObtenerGruposParticipaIdentidad(IdentidadActual.IdentidadMyGNOSS.Clave, true));
                            }
                            identidadCN.Dispose();
                            gestorIdentidades.CargarGrupos();

                            foreach (Guid idGrupo in gestorIdentidades.ListaGrupos.Keys)
                            {
                                mListaGruposPerfilEnProyectoVirtual.Add(idGrupo);
                            }
                        }
                    }
                }
                return mListaGruposPerfilEnProyectoVirtual;
            }
        }

        /// <summary>
        /// Url de intragnoss para servicios.
        /// </summary>
        public string UrlIntragnossServicios
        {
            get
            {
                return mControladorBase.UrlIntragnossServicios;
            }
        }

        /// <summary>
        /// Refactorizamos el método de session para poder controlarlo
        /// </summary>
        public ISession Session
        {
            get
            {
                return HttpContext.Session;
            }
        }

        #endregion
    }
}