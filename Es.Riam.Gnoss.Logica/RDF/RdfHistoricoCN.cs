using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.RDF;
using Es.Riam.Gnoss.AD.RDF.Model;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Data;

namespace Es.Riam.Gnoss.Logica.RDF
{
    /// <summary>
    /// Lógica de RdfHistoricoCN
    /// </summary>
    public class RdfHistoricoCN : BaseCN, IDisposable
    {
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;

        #region Constructores

        /// <summary>
        /// Constructor para RdfHistoricoCN
        /// </summary>
        public RdfHistoricoCN(EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<RdfHistoricoCN> logger, ILoggerFactory loggerFactory)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        {
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            this.RdfHistoricoAD = new RdfHistoricoAD(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication, loggerFactory.CreateLogger<RdfHistoricoAD>(), loggerFactory);
        }

        /// <summary>
        /// Constructor a partir del fichero de configuración
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuración de base de datos</param>
        public RdfHistoricoCN(string pFicheroConfiguracionBD, EntityContext entityContext, EntityContextBASE entityContextBASE, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<RdfHistoricoCN> logger, ILoggerFactory loggerFactory)
            : base(entityContext, loggingService, configService, entityContextBASE, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        {
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            RdfHistoricoAD = new RdfHistoricoAD(pFicheroConfiguracionBD, loggingService, entityContext, entityContextBASE, configService, servicesUtilVirtuosoAndReplication, loggerFactory.CreateLogger<RdfHistoricoAD>(), loggerFactory);
            mFicheroConfiguracionBD = pFicheroConfiguracionBD;
        }

        /// <summary>
        /// Constructor a partir del fichero de configuración
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuración de base de datos</param>
        public RdfHistoricoCN(string pFicheroConfiguracionBD, EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<RdfHistoricoCN> logger, ILoggerFactory loggerFactory)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        {
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            RdfHistoricoAD = new RdfHistoricoAD(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication, loggerFactory.CreateLogger<RdfHistoricoAD>(), loggerFactory);
            mFicheroConfiguracionBD = pFicheroConfiguracionBD;
        }

        /// <summary>
        /// Constructor a partir del fichero de configuración
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuración de conexión a base de datos</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        /// <param name="pCaracteresDoc">Ultimos caracteres que se añaden a la tabla RdfDocumento</param>
        public RdfHistoricoCN(string pFicheroConfiguracionBD, string pCaracteresDoc, EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<RdfHistoricoCN> logger, ILoggerFactory loggerFactory)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        {
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            RdfHistoricoAD = new RdfHistoricoAD(pFicheroConfiguracionBD, pCaracteresDoc, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication, loggerFactory.CreateLogger<RdfHistoricoAD>(), loggerFactory);
            mFicheroConfiguracionBD = pFicheroConfiguracionBD;
        }

        public RdfHistoricoCN(string pFicheroConfiguracionBD, string pCaracteresDoc, EntityContext entityContext, EntityContextBASE entityContextBASE, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<RdfHistoricoCN> logger, ILoggerFactory loggerFactory)
            : base(entityContext, loggingService, configService, entityContextBASE, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        {
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            RdfHistoricoAD = new RdfHistoricoAD(pFicheroConfiguracionBD, pCaracteresDoc, loggingService, entityContext, entityContextBASE, configService, servicesUtilVirtuosoAndReplication, loggerFactory.CreateLogger<RdfHistoricoAD>(), loggerFactory);
            mFicheroConfiguracionBD = pFicheroConfiguracionBD;
        }

        #endregion

        #region Métodos

        /// <summary>
        /// Actualiza la base de datos
        /// </summary>
        /// <param name="pRdfHistoricoDS">Dataset de Rdf</param>
        public void ActualizarBD(RdfHistoricoDS pRdfHistoricoDS)
        {
            try
            {
                if (Transaccion != null)
                {
                    this.RdfHistoricoAD.ActualizarBD(pRdfHistoricoDS);
                }
                else
                {
                    {
                        this.RdfHistoricoAD.ActualizarBD(pRdfHistoricoDS);

                        if (pRdfHistoricoDS != null)
                        {
                            pRdfHistoricoDS.AcceptChanges();
                        }
                    }
                }

            }
            catch (DBConcurrencyException ex)
            {
                TerminarTransaccion(false);
                // Error de concurrencia
                mLoggingService.GuardarLogError(ex);
                throw new ErrorConcurrencia(ex.Row);
            }
            catch (DataException ex)
            {
                TerminarTransaccion(false);
                //Error interno de la aplicación
                mLoggingService.GuardarLogError(ex);
                throw new ErrorInterno();
            }
            catch
            {
                TerminarTransaccion(false);
                throw;
            }
        }

        /// <summary>
        /// Devuelve el RDF del documento solicitado.
        /// </summary>
        /// <param name="pDocumentoID">Identificador de documento</param>
        /// <param name="pProyectoID">Proyecto del documento</param>
        /// <returns>Dataset de RDFs</returns>
        public RdfHistoricoDS ObtenerRdfPorDocumentoID(Guid pDocumentoID)
        {
            return RdfHistoricoAD.ObtenerRdfPorDocumentoID(pDocumentoID);
        }

        /// <summary>
        /// Borra el documento de los proyectos donde se encuentra compartido
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        public void EliminarDocumentoDeRDF(Guid pDocumentoID)
        {
            RdfHistoricoAD.EliminarDocumentoDeRDF(pDocumentoID);
        }

        /// <summary>
        /// Borra el documento de los proyectos donde se encuentra compartido
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        public void EliminarDocumentoDeRDFSinTransaccion(Guid pDocumentoID)
        {
            RdfHistoricoAD.EliminarDocumentoDeRDFSinTransaccion(pDocumentoID);
        }

        /// <summary>
        /// Borra el documento de los proyectos donde se encuentra compartido
        /// </summary>
        /// <param name="pNumTabla"></param>
        /// <param name="pDocumentosID"></param>
        public void EliminarDocumentosDeRDF(string pNumTabla, List<Guid> pDocumentosID)
        {
            RdfHistoricoAD.EliminarDocumentosDeRDF(pNumTabla, pDocumentosID);
        }

        /// <summary>
        /// Borra el documento de los proyectos donde se encuentra compartido
        /// </summary>
        /// <param name="pDocumentosID">Identificador del documento</param>
        public void EliminarDocumentosDeRDF(List<Guid> pDocumentosID)
        {
            RdfHistoricoAD.EliminarDocumentosDeRDF(pDocumentosID);
        }
        /// <summary>
        /// Vacia las tablas RdfDocumento_XXX
        /// </summary>
        public void VaciarTablasRdf()
        {
            RdfHistoricoAD.VaciarTablasRdf();
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Determina si está disposed
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Destructor
        /// </summary>
        ~RdfHistoricoCN()
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
        /// <param name="disposing">Determina si se está llamando desde el Dispose()</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                disposed = true;

                if (disposing)
                {
                    //Libero todos los recursos administrados que he añadido a esta clase
                    if (RdfHistoricoAD != null)
                        RdfHistoricoAD.Dispose();
                }
                RdfHistoricoAD = null;
            }
        }

        #endregion

        #region Propiedades

        private RdfHistoricoAD RdfHistoricoAD
        {
            get
            {
                return (RdfHistoricoAD)AD;
            }
            set
            {
                AD = value;
            }
        }

        #endregion
    }
}
