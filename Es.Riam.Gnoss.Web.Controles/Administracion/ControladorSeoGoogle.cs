using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.ParametroGeneralDS;
using Es.Riam.Gnoss.AD.Parametro;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.ParametrosAplicacion;
using Es.Riam.Gnoss.CL.ParametrosProyecto;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.ParametroAplicacion;
using Es.Riam.Gnoss.Elementos.ParametroGeneralDSEspacio;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Parametro;
using Es.Riam.Gnoss.Logica.ParametroAplicacion;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.Controles.ParametroGeneralDSName;
using Es.Riam.Gnoss.Web.Controles.Proyectos;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;

namespace Es.Riam.Gnoss.Web.Controles.Administracion
{
    public class ControladorSeoGoogle
    {
        private Proyecto ProyectoSeleccionado = null;
        private Dictionary<string, string> mParametroProyecto = null;
        private GestorParametroGeneral mParametrosGeneralesDS;
        private ParametroGeneral mFilaParametrosGenerales = null;
        private GestorParametroAplicacion mGestorParametrosAplicacion = null;
        private EntityContext mEntityContext;
        private LoggingService mLoggingService;
        private RedisCacheWrapper mRedisCacheWrapper;
        private ConfigService mConfigService;
        private VirtuosoAD mVirtuosoAD;
        private IHttpContextAccessor mHttpContextAccessor;
        private IServicesUtilVirtuosoAndReplication mServicesUtilVirtuosoAndReplication;

        /// <summary>
        /// Constructor del ControladorSeoGoogle con parámetros
        /// </summary>
        public ControladorSeoGoogle(Proyecto pProyecto, EntityContext entityContext, LoggingService loggingService, RedisCacheWrapper redisCacheWrapper, ConfigService configService, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
        {
            ProyectoSeleccionado = pProyecto;
            mEntityContext = entityContext;
            mLoggingService = loggingService;
            mRedisCacheWrapper = redisCacheWrapper;
            mConfigService = configService;
            mVirtuosoAD = virtuosoAD;
            mHttpContextAccessor = httpContextAccessor;
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
        }

        /// <summary>
        /// Constructor del ControladorSeoGoogle 
        /// </summary>
        public ControladorSeoGoogle()
        {

        }

        public void GuardarConfiguracionSeoGoogle(AdministrarSeoGoogleViewModel pOptions)
        {
            ParametroGeneral parametroGeneral = ParametrosGeneralesDS.ListaParametroGeneral.Where(p => p.ProyectoID.Equals(ProyectoSeleccionado.Clave)).FirstOrDefault();
            parametroGeneral.ScriptGoogleAnalytics = pOptions.ScriptGoogleAnalyticsPropio;
            parametroGeneral.CodigoGoogleAnalytics = pOptions.CodigoGoogleAnalytics;

            mEntityContext.NoConfirmarTransacciones = true;
            try
            {
                ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, null);
                proyectoCN.Actualizar();

                mEntityContext.TerminarTransaccionesPendientes(true);
            }
            catch
            {
                mEntityContext.TerminarTransaccionesPendientes(false);
                throw;
            }
        }

        /// <summary>
        /// Se actualizan los parámetros indicados en la página de administración si existen, si no existen se añaden a base de datos
        /// </summary>
        /// <param name="pOptions">Modelo que contiene los datos de la vista de la página de administración</param>
        public void GuardarConfiguracionSeoGooglePlataforma(AdministrarSeoGooglePlataformaViewModel pOptions)
        {
            ParametroAplicacionCN parametroAplicacionCN = new ParametroAplicacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

            parametroAplicacionCN.ActualizarParametroAplicacion(ParametroAD.RobotsComunidad, pOptions.RobotsBusqueda);
            parametroAplicacionCN.ActualizarParametroAplicacion("CodigoGoogleAnalytics", pOptions.CodigoGoogleAnalytics);
            parametroAplicacionCN.ActualizarParametroAplicacion("ScriptGoogleAnalytics", pOptions.ScriptGoogleAnalytics);
            parametroAplicacionCN.Actualizar();
        }

        public GestorParametroAplicacion GestorParametrosAplicacion
        {
            get
            {
                if (mGestorParametrosAplicacion == null)
                {
                    ParametroAplicacionCL paramCL = new ParametroAplicacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, null);

                    mGestorParametrosAplicacion = paramCL.ObtenerGestorParametros();
                }
                return mGestorParametrosAplicacion;
            }
        }

        /// <summary>
        /// Obtiene el dataset de parámetros generales
        /// </summary>
        private GestorParametroGeneral ParametrosGeneralesDS
        {
            get
            {
                if (mParametrosGeneralesDS == null)
                {
                    ParametroGeneralGBD gestorController = new ParametroGeneralGBD(mEntityContext);
                    mParametrosGeneralesDS = new GestorParametroGeneral();
                    mParametrosGeneralesDS = gestorController.ObtenerParametrosGeneralesDeProyecto(mParametrosGeneralesDS, ProyectoSeleccionado.Clave);
                    foreach (string parametro in ParametroProyecto.Keys)
                    {
                        ParametroProyecto parametroProyecto = new ParametroProyecto(ProyectoSeleccionado.FilaProyecto.OrganizacionID, ProyectoSeleccionado.Clave, parametro, ParametroProyecto[parametro]);
                        mParametrosGeneralesDS.ListaParametroProyecto.Add(parametroProyecto);
                    }
                }
                return mParametrosGeneralesDS;
            }
        }

        /// <summary>
        /// Parámetros de un proyecto.
        /// </summary>
        private Dictionary<string, string> ParametroProyecto
        {
            get
            {
                if (mParametroProyecto == null)
                {
                    ParametroCN parametroCN = new ParametroCN(mEntityContext, mLoggingService, mConfigService, null);
                    mParametroProyecto = parametroCN.ObtenerParametrosProyecto(ProyectoSeleccionado.Clave);
                    parametroCN.Dispose();
                }

                return mParametroProyecto;
            }
        }

        public ParametroGeneral FilaParametrosGenerales
        {
            get
            {
                if (mFilaParametrosGenerales == null)
                {
                    mFilaParametrosGenerales = ParametrosGeneralesDS.ListaParametroGeneral.Find(parametrosGenerales => parametrosGenerales.OrganizacionID.Equals(ProyectoSeleccionado.FilaProyecto.OrganizacionID) && parametrosGenerales.ProyectoID.Equals(ProyectoSeleccionado.Clave));
                }
                return mFilaParametrosGenerales;
            }
        }

        public void InvalidarCaches()
        {
            ParametroGeneralCL parametroGeneralCL = new ParametroGeneralCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, null);
            parametroGeneralCL.InvalidarCacheParametrosGeneralesDeProyecto(ProyectoSeleccionado.Clave);
            parametroGeneralCL.Dispose();

            ProyectoCL proyectoCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, null);
            proyectoCL.InvalidarParametrosProyecto(ProyectoSeleccionado.Clave, ProyectoSeleccionado.FilaProyecto.OrganizacionID);
            proyectoCL.InvalidarFilaProyecto(ProyectoSeleccionado.Clave);
            proyectoCL.InvalidarComunidadMVC(ProyectoSeleccionado.Clave);
            proyectoCL.InvalidarCabeceraMVC(ProyectoSeleccionado.Clave);
            proyectoCL.Dispose();

            ParametroAplicacionCL parametroAplicacionCL = new ParametroAplicacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
            parametroAplicacionCL.InvalidarCacheParametrosAplicacion();
        }
    }
}
