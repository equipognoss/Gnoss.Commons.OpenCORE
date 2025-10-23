using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.CMS;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.CMS;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Es.Riam.Gnoss.Logica.CMS
{
    /// <summary>
    /// Lógica referente a Proyecto Gnoss
    /// </summary>
    public class CMSCN : BaseCN, IDisposable
    {
        private LoggingService mLoggingService;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        #region Constructores

        /// <summary>
        /// Constructor sin parámetros
        /// </summary>
        public CMSCN(EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<CMSCN> logger, ILoggerFactory loggerFactory)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        {
            mLoggingService = loggingService;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            CMSAD = new CMSAD(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<CMSAD>(), mLoggerFactory);
        }

        /// <summary>
        /// Constructor a partir del fichero de configuración
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuración de base de datos</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public CMSCN(string pFicheroConfiguracionBD, EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<CMSCN> logger, ILoggerFactory loggerFactory)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        {
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            mLoggingService = loggingService;
            CMSAD = new CMSAD(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<CMSAD>(), mLoggerFactory);
        }

        #endregion

        #region Métodos generales

        #region Públicos

        /// <summary>
        /// Obtiene si un proyecto tiene algun componente con caducidad de tipo recurso
        /// </summary>       
        /// <param name="pProyectoID">Clave del proyecto</param>
        /// <returns>TRUE si tiene algun componente con caducidad de tipo recurso</returns>
        public bool ObtenerSiTieneComponenteConCaducidadTipoRecurso(Guid pProyectoID)
        {
            return CMSAD.ObtenerSiTieneComponenteConCaducidadTipoRecurso(pProyectoID);
        }

        /// <summary>
        /// Obtiene las tablas CMSProyecto, CMSBLoque, CMSComponente y CMSComponente... de un proyecto
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <returns>DataSet de CMS</returns>
        public DataWrapperCMS ObtenerCMSDeProyecto(Guid pProyectoID)
        {
            return CMSAD.ObtenerCMSDeProyecto(pProyectoID);
        }

        /// <summary>
        /// Obtiene el Dataset CMSDS de la comunidad, las tablas CMSPagina, CMSBloque, CMSBloqueComponente
        /// </summary>
        /// <param name="pProyecto">Guid del proyecto</param>
        /// <returns>DataSet de CMS</returns>
        public DataWrapperCMS ObtenerConfiguracionCMSPorProyecto(Guid pProyectoID)
        {
            return CMSAD.ObtenerConfiguracionCMSPorProyecto(pProyectoID);
        }

        /// <summary>
        /// Obtiene las tablasCMSPagina, CMSBLoque,CMSBLoqueComponente, CMSComponente y CMSComponentePropiedad de una ubicacion en un proyecto
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <param name="pUbicacion">Ubicacion</param>
        /// <param name="pBorradorOPublicacdo">0-Todo,1-Borrador,2-Publicado</param>
        /// <returns>DataSet de CMS</returns>
        public DataWrapperCMS ObtenerCMSDeUbicacionDeProyecto(short pUbicacion, Guid pProyectoID, short pBorradorOPublicacdo, bool pTraerSoloActivos)
        {
            return CMSAD.ObtenerCMSDeUbicacionDeProyecto(pUbicacion, pProyectoID, pBorradorOPublicacdo, pTraerSoloActivos);
        }

        /// <summary>
        /// Obtiene las tablas CMSComponente y CMSComponente... de un componente
        /// </summary>
        /// <param name="pComponenteID">ID del componente</param>
        /// <returns>DataSet de CMS</returns>
        public DataWrapperCMS ObtenerComponentePorID(Guid pComponenteID, Guid pProyectoID, bool pCargarSoloActivos)
        {
            List<Guid> listaID = new List<Guid>();
            listaID.Add(pComponenteID);
            return CMSAD.ObtenerComponentePorListaID(listaID, pProyectoID, pCargarSoloActivos);
        }

        /// <summary>
        /// Obtiene la tabla CMSBloque
        /// </summary>
        /// <param name="pBloqueID">ID del bloque</param>
        /// <returns>DataSet de CMS</returns>
        public DataWrapperCMS ObtenerBloquePorID(Guid pBloqueID)
        {
            return CMSAD.ObtenerBloquePorID(pBloqueID);
        }

        /// <summary>
        /// Nos indica las páginas a las que esta vinculado el componente
        /// </summary>
        /// <param name="pComponenteID"></param>
        /// <returns></returns>
        public List<string> PaginasVinculadasComponente(Guid pComponenteID, Guid pProyectoID)
        {
            return CMSAD.PaginasVinculadasComponente(pComponenteID, pProyectoID);
        }


        /// <summary>
        /// Obtiene las tablas CMSComponente y CMSComponente... de una lista de componentes
        /// </summary>
        /// <param name="pComponenteID">lista de IDs de los componentes</param>
        /// <returns>DataSet de CMS</returns>
        public DataWrapperCMS ObtenerComponentePorListaID(List<Guid> pListaComponenteID, Guid pProyectoID, bool pTraerSoloActivos)
        {
            return CMSAD.ObtenerComponentePorListaID(pListaComponenteID, pProyectoID, pTraerSoloActivos);
        }

        /// <summary>
        /// Obtiene las filas CMSComponente de un proyecto
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <param name="pTipoCaducidad">Tipo de caducidad</param>
        /// <returns>Lista de componentes</returns>
        public List<CMSComponente> ObtenerFilasComponentesCMSDeProyectoPorTipoCaducidad(Guid pProyectoID, TipoCaducidadComponenteCMS pTipoCaducidad)
        {
            return CMSAD.ObtenerFilasComponentesCMSDeProyectoPorTipoCaducidad(pProyectoID, pTipoCaducidad);
        }

        /// <summary>
        /// Obtiene las filas CMSComponente caducadas
        /// </summary>
        /// <returns>Lista de componentes</returns>
        public List<CMSComponente> ObtenerFilasComponentesCMSCaducados()
        {
            return CMSAD.ObtenerFilasComponentesCMSCaducados();
        }

        /// <summary>
        /// Obtiene las tablas CMSComponente y CMSPropiedadComponente... de un proyecto
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <returns>DataWrapper de CMS</returns>
        public DataWrapperCMS ObtenerComponentesCMSDeProyecto(Guid pProyectoID)
        {
            return CMSAD.ObtenerComponentesCMSDeProyecto(pProyectoID);
        }

        /// <summary>
        /// Obtiene la tabla CMSComponente del proyecto indicado por parámetro
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pLimite">Número de componentes a cargar.</param>
        /// <param name="pBusqueda">Título del componente a buscar</param>
        /// <returns>Lista de CMSComponente</returns>
        public List<CMSComponente> ObtenerCMSComponentePorProyecto(Guid pProyectoID, int pLimite = -1, string pBusqueda = "")
        {
            return CMSAD.ObtenerCMSComponentePorProyecto(pProyectoID, pLimite, pBusqueda);
        }

        /// <summary>
        /// Obtiene las tablas CMSComponente y CMSPropiedadComponente... de un proyecto
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <returns>DataSet de CMS</returns>
        public DataWrapperCMS ObtenerComponentesCMSDeProyecto(Guid pProyectoID, string pTexto)
        {
            return CMSAD.ObtenerComponentesCMSDeProyecto(pProyectoID, pTexto);
        }

        /// <summary>
        /// Obtiene las tablas CMSComponente y CMSPropiedadComponente... de un proyecto
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <param name="pTipoComponente">Tipo de componente</param>
        /// <returns>DataSet de CMS</returns>
        public DataWrapperCMS ObtenerComponentesCMSDeProyectoDelTipoEspecificado(Guid pProyectoID, TipoComponenteCMS pTipoComponente, string pTexto)
        {
            return CMSAD.ObtenerComponentesCMSDeProyectoDelTipoEspecificado(pProyectoID, pTipoComponente, pTexto);
        }

        public Guid? ObtenerIDComponentePorNombreEnProyecto(string pNombreComponente, Guid pProyectoID)
        {
            return CMSAD.ObtenerIDComponentePorNombreEnProyecto(pNombreComponente, pProyectoID);
        }

        public string ObtenerNombreCortoComponentePorIDComponenteEnProyecto(Guid pComponenteID, Guid pProyectoID)
        {
            return CMSAD.ObtenerNombreCortoComponentePorIDComponenteEnProyecto(pComponenteID, pProyectoID);
        }
        public Dictionary<Guid, string> ObtenerNombreComponentesPorIDComponente(List<Guid> pIdsComponentes)
        {
            return CMSAD.ObtenerNombreComponentesPorIDComponente(pIdsComponentes);
        }

		public string ObtenerNombreComponentePorIDComponenteEnProyecto(Guid pComponenteID, Guid pProyectoID)
		{
			return CMSAD.ObtenerNombreComponentePorIDComponenteEnProyecto(pComponenteID, pProyectoID);
		}

		public DateTime? ObtenerFechaModificacionDeComponenteEnProyecto(Guid pComponenteID, Guid pProyectoID)
		{
			return CMSAD.ObtenerFechaModificacionDeComponenteEnProyecto(pComponenteID, pProyectoID);
		}

		/// <summary>
		/// Elimina los bloques de una pagina en un proyecto
		/// </summary>
		/// <param name="pCMSDS">Dataset de CMS</param>
		public void EliminarBloquesDePaginaDeProyecto(Guid pProyectoID, short pTipoUbicacionCMS, bool pSoloLosBorradores)
        {
            CMSAD.EliminarBloquesDePaginaDeProyecto(pProyectoID, pTipoUbicacionCMS, pSoloLosBorradores);
        }

        /// <summary>
        /// Actualiza la fecha de la siguiente actualización de un componente
        /// </summary>
        /// <param name="pComponenteID">ID del componente</param>
        /// <param name="pFechaProximaActualizacion">Fecha de la siguiente actualización</param>
        public void ActualizarCaducidadComponente(Guid pComponenteID, DateTime pFechaProximaActualizacion)
        {
            CMSAD.ActualizarCaducidadComponente(pComponenteID, pFechaProximaActualizacion);
        }

        public List<ProyectoPestanyaVersionCMS> ObtenerVersionesEstructuraPaginaCMS(Guid pPestanyaID)
        {
            return CMSAD.ObtenerVersionesEstructuraPaginaCMS(pPestanyaID);
        }

        /// <summary>
        /// Obtiene las versiones de la estructura de una pagina del CMS pero evitando cargar todo el modelo de la estructura.
        /// </summary>
        /// <param name="pPestanyaID">Id de la pagina a consultar</param>
        /// <returns></returns>
        public List<ProyectoPestanyaVersionCMS> ObtenerVersionesEstructuraPaginaCMSSinEstructura(Guid pPestanyaID)
        {
            return CMSAD.ObtenerVersionesEstructuraPaginaCMSSinEstructura(pPestanyaID);
        }
        public List<Guid> ObtenerIdVersionesEstructuraPaginaCMS(Guid pPestanyaID)
        {
            return CMSAD.ObtenerIdVersionesEstructuraPaginaCMS(pPestanyaID);
        }
        public ProyectoPestanyaVersionCMS ObtenerVersionEstructuraPaginaCMS(Guid pVersionID)
        {
            return CMSAD.ObtenerVersionEstructuraPaginaCMS(pVersionID);
        }

        /// <summary>
        /// Actualiza los cambios realizados en CMS
        /// </summary>
        /// <param name="pCMSDW">Dataset de CMS</param>
        public void ActualizarCMSEliminandoBloquesDePaginaDeProyecto(DataWrapperCMS pCMSDW, Guid pProyectoID, short pTipoUbicacionCMS, bool pSoloLosBorradores)
        {
            try
            {
                if (Transaccion != null)
                {
                    CMSAD.EliminarBloquesDePaginaDeProyecto(pProyectoID, pTipoUbicacionCMS, pSoloLosBorradores);
                    CMSAD.ActualizarBaseDeDatos();
                }
                else
                {
                    IniciarTransaccion();
                    {
                        CMSAD.EliminarBloquesDePaginaDeProyecto(pProyectoID, pTipoUbicacionCMS, pSoloLosBorradores);
                        CMSAD.ActualizarBaseDeDatos();

                        TerminarTransaccion(true);
                    }
                }
            }
            catch (DBConcurrencyException ex)
            {
                TerminarTransaccion(false);
                // Error de concurrencia
                mLoggingService.GuardarLogError(ex, mlogger);
                throw new ErrorConcurrencia();
            }
            catch (DataException ex)
            {
                TerminarTransaccion(false);
                //Error interno de la aplicación				
                mLoggingService.GuardarLogError(ex, mlogger);
                throw new ErrorInterno();
            }
            catch (SqlException ex)
            {
                TerminarTransaccion(false);
                //MessageBox.Show("No se puede eliminar el proyecto ya que existen elementos vinculados a él. (" + e.Message + ")", "Error en la eliminacion del proyecto", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                //Error interno de la aplicación
                throw ex;
            }
            catch
            {
                TerminarTransaccion(false);
                throw;
            }
        }


        /// <summary>
        /// Actualiza los cambios realizados en CMS
        /// </summary>
        /// <param name="pCMSDW">Dataset de CMS</param>
        public void ActualizarCMS(DataWrapperCMS pCMSDW)
        {
            try
            {
                if (Transaccion != null)
                {
                    CMSAD.ActualizarBaseDeDatos();
                }
                else
                {
                    IniciarTransaccion();
                    {
                        CMSAD.ActualizarBaseDeDatos();

                        TerminarTransaccion(true);
                    }
                }

            }
            catch (DBConcurrencyException ex)
            {
                TerminarTransaccion(false);
                // Error de concurrencia
                mLoggingService.GuardarLogError(ex, mlogger);
                throw new ErrorConcurrencia();
            }
            catch (DataException ex)
            {
                TerminarTransaccion(false);
                //Error interno de la aplicación				
                mLoggingService.GuardarLogError(ex, mlogger);
                throw new ErrorInterno();
            }
            catch (SqlException ex)
            {
                TerminarTransaccion(false);
                //MessageBox.Show("No se puede eliminar el proyecto ya que existen elementos vinculados a él. (" + e.Message + ")", "Error en la eliminacion del proyecto", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                //Error interno de la aplicación
                throw ex;
            }
            catch
            {
                TerminarTransaccion(false);
                throw;
            }
        }

        /// <summary>
        /// Guarda los datos de un dataset de CMS pasado como parámetro
        /// </summary>
        /// <param name="pProyecto">Dataset de proyecto</param>
        public void GuardarCMS()
        {
            try
            {
                if (Transaccion != null)
                {
                    this.CMSAD.ActualizarBaseDeDatos();
                }
                else
                {
                    IniciarTransaccion();
                    {
                        this.CMSAD.ActualizarBaseDeDatos();

                        TerminarTransaccion(true);
                    }
                }

            }
            catch (DBConcurrencyException ex)
            {
                TerminarTransaccion(false);
                // Error de concurrencia
                mLoggingService.GuardarLogError(ex, mlogger);
                throw new ErrorConcurrencia();
            }
            catch (DataException ex)
            {
                TerminarTransaccion(false);
                //Error interno de la aplicación				
                mLoggingService.GuardarLogError(ex, mlogger);
                throw new ErrorInterno();
            }
            catch
            {
                TerminarTransaccion(false);
                throw;
            }
        }

        /// <summary>
        /// Elimina los datos de un dataset de proyecto pasado como parámetro
        /// </summary>
        /// <param name="pCMSDW">Dataset de proyecto</param>
        public void EliminarCMS()
        {
            try
            {
                if (Transaccion != null)
                {
                    this.CMSAD.ActualizarBaseDeDatos();
                }
                else
                {
                    IniciarTransaccion();
                    {
                        this.CMSAD.ActualizarBaseDeDatos();

                        TerminarTransaccion(true);
                    }
                }
            }
            catch (DBConcurrencyException ex)
            {
                TerminarTransaccion(false);
                // Error de concurrencia
                mLoggingService.GuardarLogError(ex, mlogger);
                throw new ErrorConcurrencia();
            }
            catch (DataException ex)
            {
                TerminarTransaccion(false);
                //Error interno de la aplicación				
                mLoggingService.GuardarLogError(ex, mlogger);
                throw new ErrorInterno();
            }
            catch
            {
                TerminarTransaccion(false);
                throw;
            }
        }

        public DataWrapperCMS ObtenerPaginasPerteneceComponentePorComponenteID(Guid pComponenteID, Guid pProyectoID)
        {
            return CMSAD.ObtenerPaginasPerteneceComponentePorComponenteID(pComponenteID, pProyectoID);
        }

        public DataSet ObtenerConsultaComponenteSQLSERVER(string pQuery)
        {
            return CMSAD.ObtenerConsultaComponenteSQLSERVER(pQuery);
        }

        public DataWrapperCMS ObtenerCMSBloqueComponentePropiedadComponente(Guid pProyectoID, Guid pComponenteID)
        {
            return CMSAD.ObtenerCMSBloqueComponentePropiedadComponente(pProyectoID, pComponenteID);
        }

        public List<CMSComponenteVersion> ObtenerVersionesComponenteCMS(Guid pComponenteID)
        {
            return CMSAD.ObtenerVersionesComponenteCMS(pComponenteID);
        }

        /// <summary>
        /// Carga las versiones de un componente, evitando cargar el modelo del componente en cada versión
        /// </summary>
        /// <param name="pComponenteID"></param>
        /// <returns></returns>
        public List<CMSComponenteVersion> ObtenerVersionesComponenteCMSSinModelo(Guid pComponenteID)
        {
            return CMSAD.ObtenerVersionesComponenteCMSSinModelo(pComponenteID);
        }

        public CMSComponenteVersion ObtenerVersionComponenteCMS(Guid pComponenteID, Guid pVersionID)
        {
            return CMSAD.ObtenerVersionComponenteCMS(pComponenteID, pVersionID);
        }

        public CMSComponenteVersion ObtenerVersionComponenteCMSPorVersionAnterior(Guid pComponenteID, Guid pVersionID)
        {
            return CMSAD.ObtenerVersionComponenteCMSPorVersionAnterior(pComponenteID, pVersionID);
        }

        #endregion

        #endregion

        #region Dispose

        /// <summary>
        /// Determina si está disposed
        /// </summary>
        private bool mDisposed = false;

        /// <summary>
        /// Destructor
        /// </summary>
        ~CMSCN()
        {
            //Libero los recursos
            Dispose(false);
        }

        /// <summary>
        /// Libera los recursos
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            //impido que se finalice dos veces este objeto
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Libera los recursos
        /// </summary>
        /// <param name="pDisposing">Determina si se está llamando desde el Dispose()</param>
        protected virtual void Dispose(bool pDisposing)
        {
            if (!mDisposed)
            {
                mDisposed = true;

                if (pDisposing)
                {
                    //Libero todos los recursos administrados que he añadido a esta clase
                    if (this.CMSAD != null)
                    {
                        CMSAD.Dispose();
                    }
                }
                CMSAD = null;
            }
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// DataAdapter de proyecto
        /// </summary>
        private CMSAD CMSAD
        {
            get
            {
                return (CMSAD)AD;
            }
            set
            {
                AD = value;
            }
        }

        public void BorrarPaginaCMS(Guid pMyGnoss, Guid pClave, short pTipoUbicacion)
        {
            CMSAD.BorrarPaginaCMS(pMyGnoss, pClave, pTipoUbicacion);
        }
        /// <summary>
        /// Comprueba si un componete CMS pertenece a un grupo de componentes (TipoPropiedadComponente = 6)
        /// </summary>
        /// <param name="componenteID">Id del componete a verificar</param>
        /// <returns>true si pertenece a un grupo de componentes, falso en caso contrario</returns>
        public bool ComponenteCMSPerteneceGrupo(Guid componenteID)
        {
            return CMSAD.ComponenteCMSPerteneceGrupo(componenteID);
        }

        #endregion

    }
}
