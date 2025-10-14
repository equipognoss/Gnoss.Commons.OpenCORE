using Es.Riam.Gnoss.AD.AutoCompetarEtiquetas;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Microsoft.Extensions.Logging;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Serilog.Core;

namespace Es.Riam.Gnoss.Logica.AutoCompletarEtiquetas
{
    public class AutoCompletarEtiquetasCN : BaseCN, IDisposable
    {
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        /// <summary>
        /// Constructor sin parámetros
        /// </summary>
        public AutoCompletarEtiquetasCN(LoggingService loggingService, EntityContext entityContext, ConfigService configService, EntityContextBASE entityContextBASE, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<AutoCompletarEtiquetasCN> logger, ILoggerFactory loggerFactory)
            : base(entityContext, loggingService, configService, entityContextBASE, servicesUtilVirtuosoAndReplication,logger,loggerFactory)
        {
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            this.AutoCompletarEtiquetasAD = new AutoCompetarEtiquetasAD(loggingService, entityContext, configService, entityContextBASE, servicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<AutoCompetarEtiquetasAD>(), mLoggerFactory);
        }


        /// <summary>
        /// Constructor a partir del fichero de configuración de base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuración de base de datos</param>
        public AutoCompletarEtiquetasCN(string pFicheroConfiguracionBD, string pCaracteresExtra, LoggingService loggingService, EntityContext entityContext, ConfigService configService, EntityContextBASE entityContextBASE, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<AutoCompletarEtiquetasCN> logger, ILoggerFactory loggerFactory)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication,logger,loggerFactory)
        {
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            this.AutoCompletarEtiquetasAD = new AutoCompetarEtiquetasAD(pFicheroConfiguracionBD, pCaracteresExtra, loggingService, entityContext, configService, entityContextBASE, servicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<AutoCompetarEtiquetasAD>(), mLoggerFactory);
        }


        public DataWrapperAutoCompletarEtiquetas ObtenerConfiguracionComunidad(Guid? pOrganizacionID, Guid pProyectoID)
        {
            return AutoCompletarEtiquetasAD.ObtenerConfiguracionComunidad(pOrganizacionID, pProyectoID);
        }

        public void EliminarTablasEtiquetasElemento()
        {
            this.AutoCompletarEtiquetasAD.EliminarTablasEtiquetasElemento();
        }

		/// <summary>
		/// Obtiene las etiquetas de un elemento en un proyecto.
		/// </summary>
		/// <param name="pElementoID">ID del elemento</param>
		/// <param name="pTipo">Tipo de elemento</param>
		/// <param name="pProyectoID">ID del proyecto del elemento</param>
		public string ObtenerEtiquetasElemento(Guid pElementoID, string pTipo, Guid pProyectoID)
        {
            return AutoCompletarEtiquetasAD.ObtenerEtiquetasElemento(pElementoID, pTipo, pProyectoID);
        }
        /// <summary>
        /// AD de autocompletar etiquetas.
        /// </summary>
        private AutoCompetarEtiquetasAD AutoCompletarEtiquetasAD
        {
            get
            {
                return (AutoCompetarEtiquetasAD)AD;
            }
            set
            {
                this.AD = value;
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
