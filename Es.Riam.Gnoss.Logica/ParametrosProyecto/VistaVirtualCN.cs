using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.ParametrosProyecto;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Data;

namespace Es.Riam.Gnoss.Logica.ParametrosProyecto
{
    /// <summary>
    /// Lógica referente a VistaVirtual
    /// </summary>
    public class VistaVirtualCN : BaseCN, IDisposable
    {
        #region Miembros

        private LoggingService mLoggingService;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public VistaVirtualCN(string pFicheroConfiguracionBD, EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<VistaVirtualCN> logger, ILoggerFactory loggerFactory)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        {
            mLoggingService = loggingService;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            this.VistaVirtualAD = new VistaVirtualAD(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication,mLoggerFactory.CreateLogger<VistaVirtualAD>(),mLoggerFactory);
        }

        /// <summary>
        /// Constructor para CategoríaDocumentaciónCN
        /// </summary>
        public VistaVirtualCN(EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<VistaVirtualCN> logger, ILoggerFactory loggerFactory)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        {
            mLoggingService = loggingService;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            this.VistaVirtualAD = new VistaVirtualAD(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication,mLoggerFactory.CreateLogger<VistaVirtualAD>(),mLoggerFactory);
        }

        #endregion

        #region Métodos Generales

        #region Públicos

        /// <summary>
        /// Comprueba si existe una fila en la tabla VistaVirtualPersonalizacion con un id concreto
        /// </summary>
        /// <param name="pPersonalizacionID">Identificador de la personalización que se quiere comprobar</param>
        /// <returns>True si existe la personalización</returns>
        public bool ComprobarExistePersonalizacionID(Guid pPersonalizacionID)
        {
            return this.VistaVirtualAD.ComprobarExistePersonalizacionID(pPersonalizacionID);
        }

        /// <summary>
        /// Obtiene una lista con los proyectos y su personalizacion
        /// </summary>
        /// <returns>Lista con los proyectos y su personalizacion</returns>
        public Dictionary<Guid, KeyValuePair<string, Guid>> ObtenerProyectosConVistas()
        {
            return VistaVirtualAD.ObtenerProyectosConVistas();
        }


        /// <summary>
        /// Obtiene las tablas de VistaVirtual de un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>DataSet de VistaVirtual de un proyecto</returns>
        public DataWrapperVistaVirtual ObtenerVistasVirtualPorProyectoID(Guid pProyectoID)
        {
            return this.VistaVirtualAD.ObtenerVistasVirtualPorProyectoID(pProyectoID);
        }

        /// <summary>
        /// Obtiene la personalización.
        /// </summary>
        /// <param name="pPersonalizacionID">Id de la personalizacion</param>
        /// <param name="pNombreVista">Nombre de la vista</param>
        /// <returns>Guid de la personalizacion de la vista</returns>
        public Guid ObtenerPersonalizacionComponenteCMSdeProyecto(Guid? pPersonalizacionID, string pNombreVista, string pRutaTipoComponente)
        {
            return this.VistaVirtualAD.ObtenerPersonalizacionComponenteCMSdeProyecto(pPersonalizacionID, pNombreVista, pRutaTipoComponente);
        }


        /// <summary>
        /// Obtiene el Guid de personalizacion que le corresponde a un proyecto
        /// </summary>
        /// <param name="pProyecto">Guid del proyecto</param>
        /// <returns>Guid de la personalicacion</returns>
        public Guid ObtenerPersonalicacionIdDadoProyectoID(Guid pProyectoID)
        {
            return this.VistaVirtualAD.ObtenerPersonalicacionIdDadoProyectoID(pProyectoID);
        }

        public Guid ObtenerPersonalizacionDominio(string pDominio)
        {
            return this.VistaVirtualAD.ObtenerPersonalizacionDominio(pDominio);
        }

        /// <summary>
        /// Obtiene las tablas de VistaVirtual de una personaalizacion
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>DataSet de VistaVirtual de un proyecto</returns>
        public DataWrapperVistaVirtual ObtenerVistasVirtualPorEcosistemaID(Guid pPersonalizacionEcosistemaID)
        {
            return this.VistaVirtualAD.ObtenerVistasVirtualPorEcosistemaID(pPersonalizacionEcosistemaID);
        }

        /// <summary>
        /// Obtiene las tablas de VistaVirtual de una personaalizacion
        /// </summary>
        /// <param name="pPersonalizacionID">Identificador de la personalizacion</param>
        /// <returns>DataSet de VistaVirtual de una personalizacion</returns>
        public DataWrapperVistaVirtual ObtenerVistasVirtualPorPersonalizacionID(Guid pPersonalizacionID)
        {
            return this.VistaVirtualAD.ObtenerVistasVirtualPorPersonalizacionID(pPersonalizacionID);
        }

        public bool ComprobarPersonalizacionCompartidaEnDominio(string pDominio, Guid pProyectoID)
        {
            return this.VistaVirtualAD.ComprobarPersonalizacionCompartidaEnDominio(pDominio, pProyectoID);
        }

        public void CompartirPersonalizacionEnDominio(string pDominio, Guid pProyectoID)
        {
            this.VistaVirtualAD.CompartirPersonalizacionEnDominio(pDominio, pProyectoID);
        }

        public List<string> ObtenerDominiosEstaCompartidaPersonalizacion(Guid pProyecto)
        {
            return this.VistaVirtualAD.ObtenerDominiosEstaCompartidaPersonalizacion(pProyecto);
		}

        public void DejarDeCompartirPersonalizacionEnDominio(string pDominio, Guid pProyecto)
        {
            this.VistaVirtualAD.DejarDeCompartirPersonalizacionEnDominio(pDominio, pProyecto);
        }

		/// <summary>
		/// Inserta o actualiza el html para una vista de un proyecto
		/// </summary>
		/// <param name="pProyectoID">Identificador del proyecto a actualizar su vista</param>
		/// <param name="pOrganizacionID">Identificador de la organización del proyecto</param>
		/// <param name="pPersonalizacionID">Identificador de la personalización del proyecto (NULL si aún no tiene personalización)</param>
		/// <param name="pVista">Vista que se va a personalizar</param>
		/// <param name="pHtml">HTML de la vista personalizada</param>
		/// <param name="pEsVistaRdfType">Verdad si la vista es de un formulario semántico, falso si es una vista común</param>
		public void GuardarHtmlParaVistaDeEcosistema(Guid pPersonalizacionID, string pVista, string pHtml, bool pEsVistaRdfType)
        {
            try
            {
                IniciarTransaccion();
                {
                    VistaVirtualAD.ComprobarEInsertarPersonalizacionEcosistema(pPersonalizacionID);

                    if (VistaVirtualAD.ComprobarExisteVistaPersonalizadaEnProyecto(pPersonalizacionID, pVista, pEsVistaRdfType))
                    {
                        //La vista ya estaba personalizada, actualizo su valor
                        VistaVirtualAD.ActualizarVistaPersonalizadaEnProyecto(pPersonalizacionID, pVista, pHtml, pEsVistaRdfType);
                    }
                    else
                    {
                        //La vista no existe aún, la inserto
                        VistaVirtualAD.InsertarVistaPersonalizadaEnProyecto(pPersonalizacionID, pVista, pHtml, pEsVistaRdfType);
                    }

                    TerminarTransaccion(true);
                }
            }
            catch (DBConcurrencyException ex)
            {
                TerminarTransaccion(false);
                // Error de concurrencia
                mLoggingService.GuardarLogError(ex,mlogger);
                throw new ErrorConcurrencia(ex.Row);
            }
            catch (DataException ex)
            {
                TerminarTransaccion(false);
                //Error interno de la aplicación	
                mLoggingService.GuardarLogError(ex,mlogger);
                throw new ErrorInterno();
            }
            catch
            {
                TerminarTransaccion(false);
                throw;
            }
        }

        /// <summary>
        /// Inserta o actualiza el html para una vista de un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto a actualizar su vista</param>
        /// <param name="pOrganizacionID">Identificador de la organización del proyecto</param>
        /// <param name="pPersonalizacionID">Identificador de la personalización del proyecto (NULL si aún no tiene personalización)</param>
        /// <param name="pVista">Vista que se va a personalizar</param>
        /// <param name="pHtml">HTML de la vista personalizada</param>
        /// <param name="pEsVistaRdfType">Verdad si la vista es de un formulario semántico, falso si es una vista común</param>
        public void GuardarHtmlParaVistaDeProyecto(Guid pOrganizacionID, Guid pProyectoID, Guid? pPersonalizacionID, string pVista, string pHtml, bool pEsVistaRdfType)
        {
            try
            {
                IniciarTransaccion();
                {
                    if (!pPersonalizacionID.HasValue)
                    {
                        pPersonalizacionID = VistaVirtualAD.ComprobarEInsertarPersonalizacionProyecto(pOrganizacionID, pProyectoID);
                    }

                    if (VistaVirtualAD.ComprobarExisteVistaPersonalizadaEnProyecto(pPersonalizacionID.Value, pVista, pEsVistaRdfType))
                    {
                        //La vista ya estaba personalizada, actualizo su valor
                        VistaVirtualAD.ActualizarVistaPersonalizadaEnProyecto(pPersonalizacionID.Value, pVista, pHtml, pEsVistaRdfType);
                    }
                    else
                    {
                        //La vista no existe aún, la inserto
                        VistaVirtualAD.InsertarVistaPersonalizadaEnProyecto(pPersonalizacionID.Value, pVista, pHtml, pEsVistaRdfType);
                    }

                    TerminarTransaccion(true);
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
            catch
            {
                TerminarTransaccion(false);
                throw;
            }
        }

        /// <summary>
        /// Inserta o actualiza el html para una vista de un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto a actualizar su vista</param>
        /// <param name="pOrganizacionID">Identificador de la organización del proyecto</param>
        /// <param name="pPersonalizacionID">Identificador de la personalización del proyecto (NULL si aún no tiene personalización)</param>
        /// <param name="pPagina">Vista que se va a personalizar</param>
        /// <param name="pHtml">HTML de la vista personalizada</param>
        /// <param name="pIdPersonalizacion">Identificador de la personalización del componente</param>
        public void GuardarHtmlParaVistaDeComponenteCMSdeEcosistema(Guid pPersonalizacionID, string pPagina, string pHtml, Guid pIdPersonalizacion, string pNombre, string pDatosExtra)
        {
            try
            {
                IniciarTransaccion();
                {
                    VistaVirtualAD.ComprobarEInsertarPersonalizacionEcosistema(pPersonalizacionID);

                    if (VistaVirtualAD.ComprobarExisteVistaPersonalizadaDeComponenteCMSEnProyecto(pPersonalizacionID, pIdPersonalizacion))
                    {
                        //La vista ya estaba personalizada, actualizo su valor
                        VistaVirtualAD.ActualizarVistaPersonalizadaDeComponenteCMSEnProyecto(pPersonalizacionID, pPagina, pNombre, pIdPersonalizacion, pHtml, pDatosExtra);
                    }
                    else
                    {
                        //La vista no existe aún, la inserto
                        VistaVirtualAD.InsertarVistaPersonalizadaDeComponenteCMSEnProyecto(pPersonalizacionID, pPagina, pNombre, pHtml, pIdPersonalizacion, pDatosExtra);
                    }

                    TerminarTransaccion(true);
                }
            }
            catch (DBConcurrencyException ex)
            {
                TerminarTransaccion(false);
                // Error de concurrencia
                throw new ErrorConcurrencia(ex.Row);
            }
            catch (DataException)
            {
                TerminarTransaccion(false);
                //Error interno de la aplicación				
                throw new ErrorInterno();
            }
            catch
            {
                TerminarTransaccion(false);
                throw;
            }
        }

        /// <summary>
        /// Inserta o actualiza el html para una vista de un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto a actualizar su vista</param>
        /// <param name="pOrganizacionID">Identificador de la organización del proyecto</param>
        /// <param name="pPersonalizacionID">Identificador de la personalización del proyecto (NULL si aún no tiene personalización)</param>
        /// <param name="pPagina">Vista que se va a personalizar</param>
        /// <param name="pHtml">HTML de la vista personalizada</param>
        /// <param name="pIdPersonalizacion">Identificador de la personalización del componente</param>
        public void GuardarHtmlParaVistaDeComponenteCMSdeProyecto(Guid pOrganizacionID, Guid pProyectoID, Guid? pPersonalizacionID, string pPagina, string pHtml, Guid pIdPersonalizacion, string pNombre, string pDatosExtra)
        {
            try
            {
                IniciarTransaccion();
                {
                    if (!pPersonalizacionID.HasValue)
                    {
                        pPersonalizacionID = VistaVirtualAD.ComprobarEInsertarPersonalizacionProyecto(pOrganizacionID, pProyectoID);
                    }

                    if (VistaVirtualAD.ComprobarExisteVistaPersonalizadaDeComponenteCMSEnProyecto(pPersonalizacionID.Value, pIdPersonalizacion))
                    {
                        //La vista ya estaba personalizada, actualizo su valor
                        VistaVirtualAD.ActualizarVistaPersonalizadaDeComponenteCMSEnProyecto(pPersonalizacionID.Value, pPagina, pNombre, pIdPersonalizacion, pHtml, pDatosExtra);
                    }
                    else
                    {
                        //La vista no existe aún, la inserto
                        VistaVirtualAD.InsertarVistaPersonalizadaDeComponenteCMSEnProyecto(pPersonalizacionID.Value, pPagina, pNombre, pHtml, pIdPersonalizacion, pDatosExtra);
                    }

                    TerminarTransaccion(true);
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
            catch
            {
                TerminarTransaccion(false);
                throw;
            }
        }

        /// <summary>
        /// Actualiza las tablas del dataset de VistaVirtual
        /// </summary>
        /// <param name="pVistaVirtualDW">Dataset de vista virtual</param>
        public void ActualizarVistaVirtual(DataWrapperVistaVirtual pVistaVirtualDW)
        {
            try
            {
                if (Transaccion != null)
                {
                    VistaVirtualAD.ActualizarVistaVirtual();
                }
                else
                {
                    IniciarTransaccion();
                    {
                        VistaVirtualAD.ActualizarVistaVirtual();
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
            catch
            {
                TerminarTransaccion(false);
                throw;
            }
        }


        /// <summary>
        /// Elimina el html para una vista de un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto a actualizar su vista</param>
        /// <param name="pOrganizacionID">Identificador de la organización del proyecto</param>
        /// <param name="pPersonalizacionID">Identificador de la personalización del proyecto (NULL si aún no tiene personalización)</param>
        /// <param name="pVista">Vista que se va a personalizar</param>        
        /// <param name="pEsVistaRdfType">Verdad si la vista es de un formulario semántico, falso si es una vista común</param>
        public void EliminarHtmlParaVistaDeProyecto(Guid pOrganizacionID, Guid pProyectoID, Guid pPersonalizacionID, string pVista, bool pEsVistaRdfType)
        {
            if (VistaVirtualAD.ComprobarExisteVistaPersonalizadaEnProyecto(pPersonalizacionID, pVista, pEsVistaRdfType))
            {
                //La vista ya estaba personalizada, la elimino
                VistaVirtualAD.EliminarVistaPersonalizadaEnProyecto(pPersonalizacionID, pVista, pEsVistaRdfType);
            }
        }

        /// <summary>
        /// Elimina el html para una vista de un proyecto
        /// </summary>
        /// <param name="pPersonalizacionID">Identificador de la personalización del proyecto (NULL si aún no tiene personalización)</param>
        /// <param name="pPersonalizacionComponenteID">Identificador de la personalizacion</param>       
        /// <param name="pTipoComponente">Tipo de componente</param>
        public void EliminarHtmlParaVistaDeComponenteCMSdeProyecto(Guid pPersonalizacionID, Guid pPersonalizacionComponenteID, string pTipoComponente)
        {
            VistaVirtualAD.EliminarHtmlParaVistaDeComponenteCMSdeProyecto(pPersonalizacionID, pPersonalizacionComponenteID, pTipoComponente);
        }
        public void EliminarHtmlParaVistaDeComponenteCMSdeProyectoServicioFTP(Guid pPersonalizacionID, string pTipoComponente, string pNombreVista)
        {
            VistaVirtualAD.EliminarHtmlParaVistaDeComponenteCMSdeProyectoServicioFTP(pPersonalizacionID, pTipoComponente, pNombreVista);
        }
        public void RenombrarArchivoParaVistaComponenteCMSdeProyecto(Guid pPersonalizacionID, string pTipoComponente, string pNuevoTipoComponente)
        {
            VistaVirtualAD.RenombrarArchivoParaVistaComponenteCMSdeProyecto(pPersonalizacionID, pTipoComponente, pNuevoTipoComponente);
        }
        public void RenombrarVistaPersonalizadaEnProyecto(Guid pPersonalizacionID, string pTipoPagina, string pNuevoTipoPagina, bool pEsVistaRdfType)
        {
            VistaVirtualAD.RenombrarVistaPersonalizadaEnProyecto(pPersonalizacionID, pTipoPagina, pNuevoTipoPagina, pEsVistaRdfType);
        }
        /// <summary>
        /// Obtiene el html para una vista de un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto a actualizar su vista</param>
        /// <param name="pOrganizacionID">Identificador de la organización del proyecto</param>
        /// <param name="pVista">Vista que se va a personalizar</param>
        /// <param name="pEsVistaRdfType">Verdad si la vista es de un formulario semántico, falso si es una vista común</param>
        public string ObtenerHtmlParaVistaDeProyecto(Guid pOrganizacionID, Guid pProyectoID, string pVista, bool pEsVistaRdfType)
        {
            return VistaVirtualAD.ObtenerHtmlParaVistaDeProyecto(pOrganizacionID, pProyectoID, pVista, pEsVistaRdfType);
        }
        public string ObtenerHtmlParaVistaDeProyectoConpersonalizacion(Guid pPersonalicacionID, string pVista, bool pEsVistaRdfType)
        {
            return VistaVirtualAD.ObtenerHtmlParaVistaDeProyectoConpersonalizacion(pPersonalicacionID, pVista, pEsVistaRdfType);
        }

        public string ObtenerHtmlParaVistaDePersonalizacion(Guid pPersonalizacionID, string pVista)
        {
            return VistaVirtualAD.ObtenerHtmlParaVistaDePersonalizacion(pPersonalizacionID, pVista);
        }

        public string ObtenerHtmlParaVistaRDFTypeDePersonalizacion(Guid pPersonalizacionID, string pVista)
        {
            return VistaVirtualAD.ObtenerHtmlParaVistaRDFTypeDePersonalizacion(pPersonalizacionID, pVista);
        }

        public string ObtenerHtmlParaVistaCMSDePersonalizacion(Guid pPersonalizacionID, Guid pPersonalizacionComponenteID)
        {
            return VistaVirtualAD.ObtenerHtmlParaVistaCMSDePersonalizacion(pPersonalizacionID, pPersonalizacionComponenteID);
        }

        public string ObtenerHtmlParaVistaCMSPorTipo(string pTipoComponente)
        {
            return VistaVirtualAD.ObtenerHtmlParaVistaCMSPorTipo(pTipoComponente);
        }

        public string ObtenerHtmlParaVistaCMSPorTipoDePersonalizacion(string pTipoComponente, Guid pPersonalizacionComponenteID)
        {
            return VistaVirtualAD.ObtenerHtmlParaVistaCMSPorTipoDePersonalizacion(pTipoComponente, pPersonalizacionComponenteID);
        }

        public string ObtenerHtmlParaVistaGadgetDePersonalizacion(Guid pPersonalizacionID, Guid pPersonalizacionComponenteID)
        {
            return VistaVirtualAD.ObtenerHtmlParaVistaGadgetDePersonalizacion(pPersonalizacionID, pPersonalizacionComponenteID);
        }
        public void GuardarDatosExtraPersonalicacionComponente(Guid pPersonalicacionComponenteID, Tuple<string, string> pTuplasNombreDatosExtra)
        {
            VistaVirtualAD.GuardarDatosExtrapersonalicacionComponente(pPersonalicacionComponenteID, pTuplasNombreDatosExtra);
        }
        #endregion

        #endregion

        #region Dispose


        /// <summary>
        /// Determina si está disposed
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Destructor
        /// </summary>
        ~VistaVirtualCN()
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
                    if (this.VistaVirtualAD != null)
                    {
                        VistaVirtualAD.Dispose();
                    }
                }

                VistaVirtualAD = null;
            }
        }

        #endregion

        #region Propiedades

        private VistaVirtualAD VistaVirtualAD
        {
            get
            {
                return (VistaVirtualAD)AD;
            }
            set
            {
                AD = value;
            }
        }

        #endregion

    }
}
