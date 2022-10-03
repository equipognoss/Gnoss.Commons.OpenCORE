using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.RDF;
using Es.Riam.Gnoss.AD.RDF.Model;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using System;
using System.Collections.Generic;
using System.Data;

namespace Es.Riam.Gnoss.Logica.RDF
{
    /// <summary>
    /// Lógica de RdfCN
    /// </summary>
    public class RdfCN : BaseCN, IDisposable
    {
        private LoggingService mLoggingService;

        #region Constructores

        /// <summary>
        /// Constructor para RdfCN
        /// </summary>
        public RdfCN(EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication)
        {
            mLoggingService = loggingService;

            this.RdfAD = new RdfAD(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication);
        }

        /// <summary>
        /// Constructor a partir del fichero de configuración
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuración de base de datos</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public RdfCN(string pFicheroConfiguracionBD, EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication)
        {
            mLoggingService = loggingService;

            RdfAD = new RdfAD(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication);
            mFicheroConfiguracionBD = pFicheroConfiguracionBD;
        }

        /// <summary>
        /// Constructor a partir del fichero de configuración
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuración de conexión a base de datos</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        /// <param name="pCaracteresDoc">Ultimos caracteres que se añaden a la tabla RdfDocumento</param>
        public RdfCN(string pFicheroConfiguracionBD, string pCaracteresDoc, EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication)
        {
            mLoggingService = loggingService;

            RdfAD = new RdfAD(pFicheroConfiguracionBD, pCaracteresDoc, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication);
            mFicheroConfiguracionBD = pFicheroConfiguracionBD;
        }

        #endregion

        #region Métodos

        /// <summary>
        /// Actualiza la base de datos
        /// </summary>
        /// <param name="pRdfDS">Dataset de Rdf</param>
        public void ActualizarBD(RdfDS pRdfDS)
        {
            try
            {
                /*
                this.RdfAD.ActualizarBD(pRdfDS);
                if (Transaccion != null)
                {
                    this.RdfAD.ActualizarBD(pRdfDS);
                }
                else
                {
                    //IniciarTransaccion(false);
                    {
                        this.RdfAD.ActualizarBD(pRdfDS);

                        if (pRdfDS != null)
                        {
                            pRdfDS.AcceptChanges();
                        }

                        TerminarTransaccion(true);
                    }
                }
                */
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
        public RdfDS ObtenerRdfPorDocumentoID(Guid pDocumentoID, Guid pProyectoID)
        {
            return RdfAD.ObtenerRdfPorDocumentoID(pDocumentoID, pProyectoID);
        }

        /// <summary>
        /// Borra el documento de los proyectos donde se encuentra compartido
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        public void EliminarDocumentoDeRDF(Guid pDocumentoID)
        {
            RdfAD.EliminarDocumentoDeRDF(pDocumentoID);
        }

        /// <summary>
        /// Borra el documento de los proyectos donde se encuentra compartido
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        public void EliminarDocumentoDeRDFSinTransaccion(Guid pDocumentoID)
        {
            RdfAD.EliminarDocumentoDeRDFSinTransaccion(pDocumentoID);
        }

        /// <summary>
        /// Borra el documento de los proyectos donde se encuentra compartido
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        public void EliminarDocumentosDeRDF(string pNumTabla, List<Guid> pDocumentosID)
        {
            RdfAD.EliminarDocumentosDeRDF(pNumTabla, pDocumentosID);
        }

        /// <summary>
        /// Borra el documento de los proyectos donde se encuentra compartido
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        public void EliminarDocumentosDeRDF(List<Guid> pDocumentosID)
        {
            RdfAD.EliminarDocumentosDeRDF(pDocumentosID);
        }
        /// <summary>
        /// Vacia las tablas RdfDocumento_XXX
        /// </summary>
        public void VaciarTablasRdf()
        {
            RdfAD.VaciarTablasRdf();
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
        ~RdfCN()
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
                    if (RdfAD != null)
                        RdfAD.Dispose();
                }
                RdfAD = null;
            }
        }

        #endregion

        #region Propiedades

        private RdfAD RdfAD
        {
            get
            {
                return (RdfAD)AD;
            }
            set
            {
                AD = value;
            }
        }

        #endregion
    }
}
