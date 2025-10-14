using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.Extensions.Logging;
using System;

namespace Es.Riam.Gnoss.AD
{
    /// <summary>
    /// Data adapter general
    /// </summary>
    public class GeneralAD : BaseAD
    {
        #region Constructores

        /// <summary>
        /// Constructor sin parámetros
        /// </summary>
        public GeneralAD(LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<GeneralAD> logger, ILoggerFactory loggerFactory)
            : base(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication, logger,loggerFactory)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuración de base de datos</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public GeneralAD(string pFicheroConfiguracionBD, LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<GeneralAD> logger, ILoggerFactory loggerFactory)
            : base(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication,logger,loggerFactory)
        {
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Devuelve la hora del servidor de base de datos
        /// </summary>
        public DateTime HoraServidor
        {
            get
            {
                return FechaHoraServidor();
            }
        }

        ///// <summary>
        ///// lista con los identificadores de error provocados por parte del servidor SQLServer
        ///// </summary>
        //public static int[] ErroresServidor = {9002,-2,1101,1105,3959,3967,3958,3966,4436,17182,18456};
        //ERRORES con el motor de base de datos

        // problemas de un registro de transacciones lleno (9002)
        // problemas de espacio en disco insuficiente para datos (1101,1105,3959,3967,3958,3966) 
        // problemas de visibilidad de los metadatos  (4436)
        // problemas de errores de protocolo al iniciar el motor de base de datos (17182)
        // problemas de tiempo de espera agotado (-2)
        // problemas de inicio de sesion de usuario (18456)

        #endregion
    }
}
