using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.Amigos;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Es.Riam.Gnoss.AD.ComparticionAutomatica
{

    public enum TiposEventoComparticion
    {
        ConfiguracionNueva = 0,
        ConfiguracionCambiada = 1,
        ConfiguracionEliminada = 2,
        RecursoAgregado = 3
    }

    public enum EstadoComparticion
    {
        EnProceso = 0,
        ComparticionParcial = 1,
        ComparticionFallida = 2,
        ComparticionCorrecta = 5
    }

    /// <summary>
    /// DataAdapter de ComparticionAutomaticaAD
    /// </summary>
    public class ComparticionAutomaticaAD : BaseAD
    {
        private EntityContext mEntityContext;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;

        #region Constructores
        /// <summary>
        /// El por defecto, utilizado cuando se requiere el GnossConfig.xml por defecto
        /// </summary>
        public ComparticionAutomaticaAD(LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<ComparticionAutomaticaAD> logger, ILoggerFactory loggerFactory)
            : base(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        {
            mEntityContext = entityContext;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        /// <summary>
        /// Cuando se desea pasar directamente la ruta del fichero de configuracion de conexion a la Base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD"></param>
        public ComparticionAutomaticaAD(string pFicheroConfiguracionBD, LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<ComparticionAutomaticaAD> logger, ILoggerFactory loggerFactory)
            : base(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        {
            mEntityContext = entityContext;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }
        #endregion

        #region Consultas
        private string sqlSelectComparticionAutomatica;
        private string sqlSelectComparticionAutomaticaReglas;
        private string sqlSelectComparticionAutomaticaMapping;
        #endregion

        #region DataAdapter
        #region ComparticionAutomatica
        private string sqlComparticionAutomaticaInsert;
        private string sqlComparticionAutomaticaDelete;
        private string sqlComparticionAutomaticaModify;
        #endregion

        #region ComparticionAutomaticaReglas
        private string sqlComparticionAutomaticaReglasInsert;
        private string sqlComparticionAutomaticaReglasDelete;
        private string sqlComparticionAutomaticaReglasModify;
        #endregion

        #region ComparticionAutomaticaMapping
        private string sqlComparticionAutomaticaMappingInsert;
        private string sqlComparticionAutomaticaMappingDelete;
        private string sqlComparticionAutomaticaMappingModify;
        #endregion

        #endregion

        #region Metodos generales

        #region Metodos AD

        public void ActualizarBD()
        {
            ActualizarBaseDeDatosEntityContext();
        }
        #endregion

        /// <summary>
        /// Obtiene el identificador de la compartición automática a partir del nombre de la compartición en proyecto
        /// </summary>
        /// <param name="pNombre">Nombre de la compartición en proyecto</param>
        /// <param name="pProyectoOrigenID">Identificador del proyecto de origen de la compartición</param>
        /// <returns>Guid con el identificador de la compartición. Guid empty si no lo encuentra</returns>
        public Guid? ObtenerComparticionIDPorNombre(Guid pProyectoOrigenID, string pNombre)
        {
            return mEntityContext.ComparticionAutomatica.Where(item => item.ProyectoOrigenID.Equals(pProyectoOrigenID) && item.Nombre.Equals(pNombre)).Select(item => item.ComparticionID).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene la configuración almacenada en ComparticionAutomatica, ComparticionAutomaticaMapping, ComparticionAutomaticaReglas
        /// para la compartición automática de un proyecto determinado
        /// </summary>
        /// <param name="pOrganizacionID">OrganizacionID</param>
        /// <param name="pProyectoID">ProyectoID</param>
        /// <returns>DataSet con las tablas cargadas</returns>
        public DataWrapperComparticionAutomatica ObtenerComparticionProyectoPorProyectoID(Guid pOrganizacionID, Guid pProyectoID, bool pTraerEliminadas)
        {
            DataWrapperComparticionAutomatica dataWrapperComparticionAutomatica = new DataWrapperComparticionAutomatica();

            var listaComparticionAutomatica = mEntityContext.ComparticionAutomatica.Where(item => item.OrganizacionOrigenID.Equals(pOrganizacionID) && item.ProyectoOrigenID.Equals(pProyectoID));

            if (!pTraerEliminadas)
            {
                listaComparticionAutomatica = listaComparticionAutomatica.Where(item => item.Eliminada.Equals(false));
            }

            dataWrapperComparticionAutomatica.ListaComparticionAutomatica = listaComparticionAutomatica.ToList();

            List<Guid> listaComparticionID = dataWrapperComparticionAutomatica.ListaComparticionAutomatica.Select(item => item.ComparticionID).ToList();

            dataWrapperComparticionAutomatica.ListaComparticionAutomaticaMapping = mEntityContext.ComparticionAutomaticaMapping.Where(item => listaComparticionID.Contains(item.ComparticionID)).ToList();

            dataWrapperComparticionAutomatica.ListaComparticionAutomaticaReglas = mEntityContext.ComparticionAutomaticaReglas.Where(item => listaComparticionID.Contains(item.ComparticionID)).ToList();

            return dataWrapperComparticionAutomatica;
        }

        /// <summary>
        /// Obtiene la configuración almacenada en ComparticionAutomatica, ComparticionAutomaticaMapping, ComparticionAutomaticaReglas
        /// a partir de un ComparticionID
        /// </summary>
        /// <param name="pComparticionID">Identificador de la compartición</param>
        /// <returns>DataSet con las tablas cargadas</returns>
        public DataWrapperComparticionAutomatica ObtenerComparticionAutomaticaPorComparticionID(Guid pComparticionID)
        {
            DataWrapperComparticionAutomatica dataWrapperComparticionAutomatica = new DataWrapperComparticionAutomatica();

            dataWrapperComparticionAutomatica.ListaComparticionAutomatica = mEntityContext.ComparticionAutomatica.Where(item => item.ComparticionID.Equals(pComparticionID)).ToList();

            dataWrapperComparticionAutomatica.ListaComparticionAutomaticaMapping = mEntityContext.ComparticionAutomaticaMapping.Where(item => item.ComparticionID.Equals(pComparticionID)).ToList();

            dataWrapperComparticionAutomatica.ListaComparticionAutomaticaReglas = mEntityContext.ComparticionAutomaticaReglas.Where(item => item.ComparticionID.Equals(pComparticionID)).ToList();

            return dataWrapperComparticionAutomatica;
        }

        #endregion
    }
}