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
        /// <summary>
        /// 
        /// </summary>
        public ControladorSeoGoogle(Proyecto pProyecto, EntityContext entityContext, LoggingService loggingService, RedisCacheWrapper redisCacheWrapper, ConfigService configService, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor)
        {
            ProyectoSeleccionado = pProyecto;
            mEntityContext = entityContext;
            mLoggingService = loggingService;
            mRedisCacheWrapper = redisCacheWrapper;
            mConfigService = configService;
            mVirtuosoAD = virtuosoAD;
            mHttpContextAccessor = httpContextAccessor;
        }
        /// <summary>
        /// 
        /// </summary>
        public ControladorSeoGoogle()
        {

        }
        public void GuardarConfiguracionSeoGoogle(AdministrarSeoGoogleViewModel pOptions)
        {
            ControladorProyecto controladorProyecto = new ControladorProyecto(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, null, null, mVirtuosoAD, mHttpContextAccessor, null);
            controladorProyecto.GuardarParametroString(ParametrosGeneralesDS, ParametroAD.RobotsComunidad, pOptions.ValorRobotsBusqueda);
            ParametroProyecto filaParametro = mEntityContext.ParametroProyecto.Where(param => param.ProyectoID.Equals(ProyectoSeleccionado.Clave) && param.Parametro.Equals(ParametroAD.RobotsComunidad)).FirstOrDefault();
            if (filaParametro != null && !filaParametro.Valor.Equals(pOptions.ValorRobotsBusqueda))
            {
                // El parametro existe con otro valor, lo modifico
                ParametroProyecto param = new ParametroProyecto();
                param.Parametro = ParametroAD.RobotsComunidad;
                param.ProyectoID = ProyectoSeleccionado.Clave;
                param.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
                param.Valor = pOptions.ValorRobotsBusqueda;
                mEntityContext.EliminarElemento(filaParametro);
                mEntityContext.ParametroProyecto.Add(param);
            }

            //ControladorProyecto.GuardarParametroString(ParametrosGeneralesDS, "ScriptGoogleAnalytics", pOptions.ScriptGoogleAnalytics);
            ParametroGeneral paramScriptCache = ParametrosGeneralesDS.ListaParametroGeneral.Where(p => p.ProyectoID.Equals(ProyectoSeleccionado.Clave)).FirstOrDefault();
            paramScriptCache.ScriptGoogleAnalytics = pOptions.ScriptGoogleAnalytics;

            //ParametroProyecto parametroScript = mEntityContext.ParametroProyecto.Where(p => p.ProyectoID.Equals(ProyectoSeleccionado.Clave) && p.Parametro.Equals("ScriptGoogleAnalytics")).FirstOrDefault();
            ParametroGeneral parametroScript = mEntityContext.ParametroGeneral.Where(p => p.ProyectoID.Equals(ProyectoSeleccionado.Clave)).FirstOrDefault();

            if (parametroScript != null && !parametroScript.ScriptGoogleAnalytics.Equals(pOptions.ScriptGoogleAnalytics))
            {

                // si existe con otro valor, lo modifico
                mEntityContext.EliminarElemento(parametroScript);
                parametroScript.ScriptGoogleAnalytics = pOptions.ScriptGoogleAnalytics;
                mEntityContext.ParametroGeneral.Add(parametroScript);

            }

            //ControladorProyecto.GuardarParametroString(ParametrosGeneralesDS, "CodigoGoogleAnalytics", pOptions.CodigoGoogleAnalytics);
            ParametroGeneral paramCodigoCache = ParametrosGeneralesDS.ListaParametroGeneral.Where(p => p.ProyectoID.Equals(ProyectoSeleccionado.Clave)).FirstOrDefault();
            paramCodigoCache.CodigoGoogleAnalytics = pOptions.CodigoGoogleAnalytics;

            ParametroGeneral parametroCodigo = mEntityContext.ParametroGeneral.Where(p => p.ProyectoID.Equals(ProyectoSeleccionado.Clave)).FirstOrDefault();

            if (parametroCodigo != null && !parametroCodigo.CodigoGoogleAnalytics.Equals(pOptions.CodigoGoogleAnalytics))
            {
                // si existe con otro valor, lo modifico
                mEntityContext.EliminarElemento(parametroCodigo);
                parametroCodigo.CodigoGoogleAnalytics = pOptions.CodigoGoogleAnalytics;
                mEntityContext.ParametroGeneral.Add(parametroCodigo);
            }

            mEntityContext.SaveChanges();
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
        public void GuardarConfiguracionSeoGooglePlataforma(AdministrarSeoGooglePlataformaViewModel pOptions)
        {
            //comprobamos si está en cache
            ParametroAplicacion parametroEnCache = GestorParametrosAplicacion.ParametroAplicacion.Where(param => param.Parametro.Equals(ParametroAD.RobotsComunidad)).FirstOrDefault();

            if (parametroEnCache != null && !parametroEnCache.Valor.Equals(pOptions.RobotsBusqueda))
            {
                //si existe y el valor es diferente lo modifico
                parametroEnCache.Valor = pOptions.RobotsBusqueda;
            }
            else if (parametroEnCache == null)
            {
                //si no existe lo añadimos a cache
                GestorParametrosAplicacion.ParametroAplicacion.Add(parametroEnCache);
            }

            ParametroAplicacion filaParametro = mEntityContext.ParametroAplicacion.Where(param => param.Parametro.Equals(ParametroAD.RobotsComunidad)).FirstOrDefault();
            if (filaParametro != null && !filaParametro.Valor.Equals(pOptions.RobotsBusqueda))
            {
                // El parametro existe, lo modifico
                ParametroAplicacion param = new ParametroAplicacion();

                param.Parametro = filaParametro.Parametro;
                param.Valor = pOptions.RobotsBusqueda;
                mEntityContext.EliminarElemento(filaParametro);
                mEntityContext.ParametroAplicacion.Add(param);
            }
            else if (filaParametro == null)
            {
                //si no existe lo guardo en BD
                ParametroAplicacion param = new ParametroAplicacion();

                param.Parametro = ParametroAD.RobotsComunidad;
                param.Valor = pOptions.RobotsBusqueda;
                mEntityContext.ParametroAplicacion.Add(param);
            }

            ParametroAplicacion codigoEnCache = GestorParametrosAplicacion.ParametroAplicacion.Where(param => param.Parametro.Equals("CodigoGoogleAnalytics")).FirstOrDefault();
            ParametroAplicacion filaCodigo = mEntityContext.ParametroAplicacion.Where(param => param.Parametro.Equals("CodigoGoogleAnalytics")).FirstOrDefault();
            if (codigoEnCache != null && !codigoEnCache.Valor.Equals(pOptions.CodigoGoogleAnalytics))
            {
                //si existe y el valor es diferente lo modifico
                codigoEnCache.Valor = pOptions.CodigoGoogleAnalytics;
            }
            else if (codigoEnCache == null)
            {
                //si no existe lo añadimos a cache
                GestorParametrosAplicacion.ParametroAplicacion.Add(codigoEnCache);
            }

            if (filaCodigo != null && !filaCodigo.Valor.Equals(pOptions.CodigoGoogleAnalytics))
            {
                // El parametro existe, lo modifico
                ParametroAplicacion param = new ParametroAplicacion();

                param.Parametro = filaCodigo.Parametro;
                param.Valor = pOptions.CodigoGoogleAnalytics;
                mEntityContext.EliminarElemento(filaCodigo);
                mEntityContext.ParametroAplicacion.Add(param);
            }
            else if (filaCodigo == null)
            {
                // si no existe lo guardo en BD
                ParametroAplicacion param = new ParametroAplicacion();

                param.Parametro = "CodigoGoogleAnalytics";
                param.Valor = pOptions.CodigoGoogleAnalytics;
                mEntityContext.ParametroAplicacion.Add(param);
            }

            ParametroAplicacion scriptEnCache = GestorParametrosAplicacion.ParametroAplicacion.Where(param => param.Parametro.Equals("ScriptGoogleAnalytics")).FirstOrDefault();
            ParametroAplicacion filaScript = mEntityContext.ParametroAplicacion.Where(param => param.Parametro.Equals("ScriptGoogleAnalytics")).FirstOrDefault();
            if (scriptEnCache != null && !scriptEnCache.Valor.Equals(pOptions.ScriptGoogleAnalytics))
            {
                //si existe y el valor es diferente lo modifico
                scriptEnCache.Valor = pOptions.ScriptGoogleAnalytics;
            }
            else if (scriptEnCache == null)
            {
                //si no existe lo añadimos a cache
                GestorParametrosAplicacion.ParametroAplicacion.Add(scriptEnCache);
            }
            if (filaScript != null && !filaScript.Valor.Equals(pOptions.ScriptGoogleAnalytics))
            {
                // El parametro existe, lo modifico
                ParametroAplicacion param = new ParametroAplicacion();

                param.Parametro = filaScript.Parametro;
                param.Valor = pOptions.ScriptGoogleAnalytics;
                mEntityContext.EliminarElemento(filaScript);
                mEntityContext.ParametroAplicacion.Add(param);
            }
            else if (filaScript == null)
            {
                // si no existe lo guardo en BD
                ParametroAplicacion param = new ParametroAplicacion();

                param.Parametro = "ScriptGoogleAnalytics";
                param.Valor = pOptions.ScriptGoogleAnalytics;
                mEntityContext.ParametroAplicacion.Add(param);
            }

            mEntityContext.SaveChanges();

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
                        //mParametrosGeneralesDS.ParametroProyecto.AddParametroProyectoRow(ProyectoSeleccionado.FilaProyecto.OrganizacionID, ProyectoSeleccionado.Clave, parametro, ParametroProyecto[parametro]);
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
                    //mFilaParametrosGenerales = ParametrosGeneralesDS.ParametroGeneral.FindByOrganizacionIDProyectoID(ProyectoSeleccionado.FilaProyecto.OrganizacionID, ProyectoSeleccionado.Clave);
                    mFilaParametrosGenerales = ParametrosGeneralesDS.ListaParametroGeneral.Find(parametrosGenerales => parametrosGenerales.OrganizacionID.Equals(ProyectoSeleccionado.FilaProyecto.OrganizacionID) && parametrosGenerales.ProyectoID.Equals(ProyectoSeleccionado.Clave));
                }
                return mFilaParametrosGenerales;
            }
        }
        public void InvalidarCaches()
        {
            ParametroGeneralCL paramCL = new ParametroGeneralCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, null);
            paramCL.InvalidarCacheParametrosGeneralesDeProyecto(ProyectoSeleccionado.Clave);
            ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, null);
            proyCL.InvalidarParametrosProyecto(ProyectoSeleccionado.Clave, ProyectoSeleccionado.FilaProyecto.OrganizacionID);
            proyCL.InvalidarFilaProyecto(ProyectoSeleccionado.Clave);

            proyCL.InvalidarComunidadMVC(ProyectoSeleccionado.Clave);
            proyCL.InvalidarCabeceraMVC(ProyectoSeleccionado.Clave);

            proyCL.Dispose();
        }
    }
}
