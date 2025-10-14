using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Linq;

namespace Es.Riam.Gnoss.AD.ServiciosGenerales
{
    /// <summary>
    /// Data adapter para países
    /// </summary>
    public class PaisAD : BaseAD
    {
        private EntityContext mEntityContext;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        #region Constructores

        /// <summary>
        /// El por defecto, utilizado cuando se requiere el GnossConfig.xml por defecto
        /// </summary>
        public PaisAD(LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<PaisAD> logger, ILoggerFactory loggerFactory)
            : base(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        {
            mEntityContext = entityContext;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        /// <summary>
        /// Cuando se desea pasar directamente la ruta del fichero de configuración de conexión a base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuración de la conexión a base de datos</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public PaisAD(string pFicheroConfiguracionBD, LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<PaisAD> logger, ILoggerFactory loggerFactory)
            : base(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication,logger,loggerFactory)
        {
            mEntityContext = entityContext;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        #endregion

        #region Métodos Generales

        #region Públicos
        /// <summary>
        /// Obtiene todos los paises
        /// </summary>
        /// <returns>Lista de paises</returns>
        public DataWrapperPais ObtenerPaises()
        {
            DataWrapperPais dataWrapperPais = new DataWrapperPais();

            dataWrapperPais.ListaPais = mEntityContext.Pais.OrderBy(item => item.Nombre).ToList();

            return dataWrapperPais;
        }

        /// <summary>
        /// Obtiene todas las provincias
        /// </summary>
        /// <returns>Lista de provincias</returns>
        public DataWrapperPais ObtenerProvincias()
        {
            DataWrapperPais dataWrapperPais = new DataWrapperPais();

            dataWrapperPais.ListaProvincia = mEntityContext.Provincia.OrderBy(item => item.Nombre).ToList();

            return dataWrapperPais;
        }

        /// <summary>
        /// Obtiene todos los países y todas las provincias
        /// </summary>
        /// <returns>Lista de paises</returns>
        public DataWrapperPais ObtenerPaisesProvincias()
        {
            DataWrapperPais dataWrapperPais = new DataWrapperPais();

            //Pais
            dataWrapperPais.ListaPais = mEntityContext.Pais.OrderBy(item => item.Nombre).ToList();

            //Provincia
            dataWrapperPais.ListaProvincia = mEntityContext.Provincia.OrderBy(item => item.Nombre).ToList();

            return dataWrapperPais;
        }

        /// <summary>
        /// Obtiene las provincias de un pais
        /// </summary>
        /// <param name="pPaisID">Identificador del pais</param>
        /// <returns>Lista de provincias del pais</returns>
        public DataWrapperPais ObtenerProvinciasDePais(Guid pPaisID)
        {
            DataWrapperPais dataWrapperPais = new DataWrapperPais();

            dataWrapperPais.ListaPais = mEntityContext.Pais.OrderBy(item => item.Nombre).ToList();

            dataWrapperPais.ListaProvincia = mEntityContext.Provincia.Where(item => item.PaisID.Equals(pPaisID)).OrderBy(item => item.Nombre).ToList();

            return dataWrapperPais;
        }

        /// <summary>
        /// Obtiene el identificador del pais a partir de su nombre
        /// </summary>
        /// <param name="pNombrePais">Nombre del país</param>
        /// <returns>Identificador del país. Guid.Empty en caso de no encontrarlo</returns>
        public Guid ObtenerPaisIDPorNombre(string pNombrePais)
        {
            return mEntityContext.Pais.Where(item => item.Nombre.Equals(pNombrePais)).Select(item => item.PaisID).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene el nombre del país
        /// </summary>
        /// <param name="pPaisID">Identificador del país</param>
        /// <returns>Nombre del país</returns>
        public string ObtenerNombrePais(Guid pPaisID)
        {
            return mEntityContext.Pais.Where(item => item.PaisID.Equals(pPaisID)).Select(item => item.Nombre).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene el nombre de la provincia
        /// </summary>
        /// <param name="pProvinciaID">Identificador de la provincia</param>
        /// <returns>Nombre de la provincia</returns>
        public string ObtenerNombreProvincia(Guid pPaisID, Guid pProvinciaID)
        {
            return mEntityContext.Provincia.Where(item => item.PaisID.Equals(pPaisID) && item.ProvinciaID.Equals(pProvinciaID)).Select(item => item.Nombre).FirstOrDefault();
        }

        /// <summary>
        /// Actualiza la base de datos
        /// </summary>
        public void ActualizarPaises()
        {
            ActualizarBaseDeDatosEntityContext();
        }

        #endregion

        #endregion
    }
}
