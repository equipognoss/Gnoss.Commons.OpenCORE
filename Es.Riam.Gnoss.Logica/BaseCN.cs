using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Cookies;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.Logica.ParametroAplicacion;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;
using System.Data;
using System.Data.Common;

namespace Es.Riam.Gnoss.Logica
{

    /// <summary>
    /// Clase base para la capa de negocio
    /// </summary>
    public abstract class BaseCN
    {

        #region Miembros

        private BaseAD mAD;

        /// <summary>
        /// Fichero de configuración
        /// </summary>
        protected string mFicheroConfiguracionBD = "";

        /// <summary>
        /// Verdad si se debe usar la variable estática de conexión a base de datos
        /// </summary>
        protected bool mUsarVariableEstatica = false;

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

        protected EntityContext mEntityContext;

        protected LoggingService mLoggingService;

        protected ConfigService mConfigService;

        protected EntityContextBASE mEntityContextBASE;
        protected IServicesUtilVirtuosoAndReplication mServicesUtilVirtuosoAndReplication;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        #endregion

        #region Constructor

        public BaseCN(EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<BaseCN> logger, ILoggerFactory loggerFactory)
        {
            mEntityContext = entityContext;
            mLoggingService = loggingService;
            mConfigService = configService;
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        public BaseCN(EntityContext entityContext, LoggingService loggingService, ConfigService configService, EntityContextBASE entityContextBASE, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<BaseCN> logger, ILoggerFactory loggerFactory)
        {
            mEntityContext = entityContext;
            mLoggingService = loggingService;
            mConfigService = configService;
            mEntityContextBASE = entityContextBASE;
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene o establece el acceso a datos
        /// </summary>
        protected BaseAD AD
        {
            get
            {
                return mAD;
            }
            set
            {
                this.mAD = value;
            }
        }

        /// <summary>
        /// Transacción actual
        /// </summary>
        protected DbTransaction Transaccion
        {
            get
            {
                return AD.Transaccion;
            }
        }

        protected DbTransaction TransaccionBASE
        {
            get
            {
                return AD.TransaccionBASE;
            }
        }

        /// <summary>
        /// Iniciamos la transacción
        /// </summary>
        /// <returns></returns>
        public DbTransaction IniciarTransaccion(bool pIniciarTransaccionEntity = true)
        {
            AD.IniciarTransaccion(pIniciarTransaccionEntity);
            return Transaccion;
        }

        /// <summary>
        /// Iniciamos la transacción
        /// </summary>
        /// <returns></returns>
        public DbTransaction IniciarTransaccionBASE(bool pIniciarTransaccionEntity = true)
        {
            AD.IniciarTransaccionBASE(pIniciarTransaccionEntity);
            return TransaccionBASE;
        }

        /// <summary>
        /// Terminamos la transaccción
        /// </summary>
        /// <param name="pExito"></param>
        public void TerminarTransaccion(bool pExito)
        {
            AD.TerminarTransaccion(pExito);
        }

        /// <summary>
        /// Terminamos la transaccción
        /// </summary>
        /// <param name="pExito"></param>
        public void TerminarTransaccionBASE(bool pExito)
        {
            AD.TerminarTransaccionBASE(pExito);
        }

        #endregion

        public string NombreProyectoPadreEcositema
        {
            get
            {
                if (mNombreProyectoPadreEcositema == null)
                {
                    ParametroAplicacionCN paramCN = new ParametroAplicacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCN>(), mLoggerFactory);
                    string ComunidadPadreEcosistemaID = paramCN.ObtenerParametroAplicacion("ComunidadPadreEcosistemaID");
                    paramCN.Dispose();
                    if (!string.IsNullOrEmpty(ComunidadPadreEcosistemaID))
                    {
                        try
                        {
                            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
                            mNombreProyectoPadreEcositema = proyCN.ObtenerNombreCortoProyecto(Guid.Parse(ComunidadPadreEcosistemaID));
                            proyCN.Dispose();
                        }
                        catch
                        {
                            mLoggingService.GuardarLogError("El parametro ComunidadPadreEcosistemaID no esta bien configurado.",mlogger);
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
                    ParametroAplicacionCN paramCN = new ParametroAplicacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCN>(), mLoggerFactory);
                    string NombreCortoEcosistema = paramCN.ObtenerParametroAplicacion("NombreCortoProyectoPadreEcositema");
                    paramCN.Dispose();
                    if (!string.IsNullOrEmpty(NombreCortoEcosistema))
                    {
                        try
                        {
                            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
                            mNombreCortoProyectoPadreEcosistema = NombreCortoEcosistema;
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
                            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
                            mPadreEcosistemaProyectoID = proyCN.ObtenerProyectoIDPorNombreCorto(NombreCortoProyectoPadreEcositema);
                            proyCN.Dispose();
                        }
                        catch
                        {
                            mLoggingService.GuardarLogError("El parametro NombreCortoProyectoPadreEcositema no esta bien configurado.",mlogger);
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
                            mLoggingService.GuardarLogError("El parametro ComunidadPadreEcosistemaID no esta bien configurado.",mlogger);
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

        public void AnyadirCategoriaProyectoCookie(CategoriaProyectoCookie pCategoriaProyectoCookie)
        {
            mEntityContext.CategoriaProyectoCookie.Add(pCategoriaProyectoCookie);
        }

        public void EliminarCategoriaProyectoCookie(CategoriaProyectoCookie pCategoriaProyectoCookie)
        {
            mEntityContext.CategoriaProyectoCookie.Remove(pCategoriaProyectoCookie);
        }

        public void AnyadirProyectoCookie(ProyectoCookie pProyectoCookie)
        {
            mEntityContext.ProyectoCookie.Add(pProyectoCookie);
        }

        public void EliminarProyectoCookie(ProyectoCookie pProyectoCookie)
        {
            mEntityContext.ProyectoCookie.Remove(pProyectoCookie);
        }

        /// <summary>
        /// Actualiza 
        /// </summary>
        public void Actualizar()
        {
            try
            {
                if (Transaccion != null)
                {
                    mEntityContext.SaveChanges();
                }
                else
                {
                    IniciarTransaccion();
                    {
                        mEntityContext.SaveChanges();
                        TerminarTransaccion(true);
                    }
                }
            }
            catch (DBConcurrencyException ex)
            {
                TerminarTransaccion(false);
                // Error de concurrencia
                mLoggingService.GuardarLogError(ex, mlogger);
                throw new ErrorConcurrencia(ex.Row);
            }
            catch (DataException ex)
            {
                TerminarTransaccion(false);
                //Error interno de la aplicación				
                mLoggingService.GuardarLogError(ex, mlogger);
                throw new ErrorInterno();
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, mlogger);
                TerminarTransaccion(false);
                throw;
            }
        }
    }
}
