using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.Organizador.Correo;
using Es.Riam.Gnoss.AD.Organizador.Correo.Model;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;

namespace Es.Riam.Gnoss.Logica.Organizador.Correo
{
    /// <summary>
    /// Lógica referente a Correo interno
    /// </summary>
    public class CorreoCN : BaseCN, IDisposable
    {

        #region Miembros

        private LoggingService mLoggingService;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        #endregion

        #region Constructor

        /// <summary>
        /// Constructor para CorreoCN
        /// </summary>
        public CorreoCN(EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<CorreoCN> logger, ILoggerFactory loggerFactory)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        {
            mLoggingService = loggingService;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            if(loggerFactory == null)
            {
                this.CorreoAD = new CorreoAD(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication, null, null);
            }
            else
            {
                this.CorreoAD = new CorreoAD(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<CorreoAD>(), mLoggerFactory);
            }
        }

        /// <summary>
        /// Constructor a partir del fichero de configuración
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuración de base de datos</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public CorreoCN(string pFicheroConfiguracionBD, EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<CorreoCN> logger, ILoggerFactory loggerFactory)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        {
            mLoggingService = loggingService;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            this.CorreoAD = new CorreoAD(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication,mLoggerFactory.CreateLogger<CorreoAD>(),mLoggerFactory);
        }

        #endregion

        #region Métodos generales

        #region Públicos

        /// <summary>
        /// Obtiene un correo por ID
        /// </summary>
        /// <param name="pCorreoID">Identificador del correo</param>
        /// <param name="pIdentidad">Identidad involucrada en el correo (emisor o receptor)</param>
        /// <param name="pIdentidadOrg">Identidad de Organizacion involucrada en el correo (emisor o receptor)</param>
        /// <returns>DataSet de Correo</returns>
        public CorreoDS ObtenerCorreoPorID(Guid pCorreoID, Guid pIdentidad, Guid? pIdentidadOrg)
        {
            return CorreoAD.ObtenerCorreoPorID(pCorreoID, pIdentidad, pIdentidadOrg);
        }

        /// <summary>
        /// Obtiene un dataset de correo por una lista de IDs
        /// </summary>
        /// <param name="pListaCorreosIDs">Lista de identificadores de correso</param>
        /// <param name="pCorreoDS">DataSet de Correo</param>
        public void ObtenerCorreoPorListaIDs(List<Guid> pListaCorreosIDs, Guid pIdentidad, Guid? pIdentidadOrg, ref CorreoDS pCorreoDS)
        {
            CorreoAD.ObtenerCorreoPorListaIDs(pListaCorreosIDs, pIdentidad, pIdentidadOrg, ref pCorreoDS);
        }

        /// <summary>
        /// Obtiene un dataset de correo por una lista de IDs
        /// </summary>
        /// <param name="pListaCorreosIDs">Lista de identificadores de correso</param>
        /// <param name="pIdentidad">Identiad actual</param>
        /// <param name="pTipoBandeja">0 Entrada, 1 Enviados, 2 Eliminados, NULL sin especificar</param>
        /// <returns>DataSet de Correo</returns>
        public CorreoDS ObtenerCorreoPorListaIDs(List<Guid> pListaCorreosIDs, Guid pIdentidad, int? pTipoBandeja)
        {
            return CorreoAD.ObtenerCorreoPorListaIDs(pListaCorreosIDs, pIdentidad, pTipoBandeja);
        }

        /// <summary>
        /// Obtiene el correo siguiente de un correo a partir del identificador pasado como parámetro
        /// </summary>
        /// <param name="pCorreoID">Identificador del correo</param>
        /// <param name="pIdentidadID">Identificador de la identidad actual</param>
        /// <param name="pTipoCorreo">Tipo de listado de corro</param>
        /// <returns>Dataset de correo</returns>
        public Guid ObtenerCorreoIDSiguienteDeCorreoPorID(Guid pCorreoID, Guid pIdentidadID, Guid? pIdentidadOrganizacion, TiposListadoCorreo pTipoCorreo)
        {
            return CorreoAD.ObtenerCorreoIDSiguienteDeCorreoPorID(pCorreoID, pIdentidadID, pIdentidadOrganizacion, pTipoCorreo);
        }

        /// <summary>
        /// Obtiene el correo anterior de un correo a partir del identificador pasado como parámetro
        /// </summary>
        /// <param name="pCorreoID">Identificador del correo</param>
        /// <param name="pIdentidadID">Identificador de la identidad actual</param>
        /// <param name="pTipoCorreo">Tipo de listado de corro</param>
        /// <returns>Dataset de correo</returns>
        public Guid ObtenerCorreoIDAnteriorDeCorreoPorID(Guid pCorreoID, Guid pIdentidadID, Guid? pIdentidadOrganizacion, TiposListadoCorreo pTipoCorreo)
        {
            return CorreoAD.ObtenerCorreoIDAnteriorDeCorreoPorID(pCorreoID, pIdentidadID, pIdentidadOrganizacion, pTipoCorreo);
        }

        /// <summary>
        /// Obtiene el número de correos no leídos del usuario pasado por parámetro
        /// </summary>
        /// <param name="pIdentidad">Identificador de identidad de usuario</param>
        /// <param name="pIdentidadOrg">Identificador de la identidad de organización</param>
        /// <param name="pBandejaEliminados">Indica si se debe mostrar los no leídos de la bandeja de entrada o de la bandeja de eliminados</param>
        /// <returns>Número de correos no leídos por el usuario</returns>
        public int ObtenerNumeroCorreosNoLeidos(Guid pIdentidad, Guid? pIdentidadOrg, bool pBandejaEliminados)
        {
            return CorreoAD.ObtenerNumeroCorreosNoLeidos(pIdentidad, pIdentidadOrg, pBandejaEliminados);
        }

        /// <summary>
        /// Actualiza Correo 
        /// </summary>
        /// <param name="pCorreoDS">Dataset de correo para actualizar</param>
        public void ActualizarCorreo(CorreoDS pCorreoDS)
        {
            try
            {
                if (pCorreoDS != null)
                {
                    if (Transaccion != null)
                    {
                        this.CorreoAD.ActualizarCorreo((CorreoDS)pCorreoDS);
                    }
                    else
                    {
                        IniciarTransaccion();
                        {
                            this.CorreoAD.ActualizarCorreo((CorreoDS)pCorreoDS);

                            if (pCorreoDS != null)
                            {
                                pCorreoDS.AcceptChanges();
                            }
                            TerminarTransaccion(true);
                        }
                    }
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
        /// Actualiza un correo pasado por parámetro como leído
        /// </summary>
        /// <param name="pCorreoDS">Dataset de correos</param>
        /// <param name="pCorreoID">Identificador de correo</param>
        /// <param name="pDestinatario">Identificador del destinatario</param>
        public void ActualizarCorreoLeido(CorreoDS pCorreoDS, Guid pCorreoID, Guid pDestinatario)
        {
            this.CorreoAD.ActualizarCorreoLeido((CorreoDS)pCorreoDS, pCorreoID, pDestinatario);
        }

        /// <summary>
        /// Restaura el correo pasado por parámetro
        /// </summary>
        /// <param name="pCorreo">Correo</param>
        /// <param name="pAutor"></param>
        /// <param name="pCorreoID"></param>
        /// <param name="pDestinatario"></param>
        public void RestaurarCorreo(Guid pCorreoID, Guid pAutor, Guid pDestinatario)
        {
            this.CorreoAD.RestaurarCorreo(pCorreoID, pAutor, pDestinatario);
        }

        /// <summary>
        /// Elimina el correo pasado por parámetro
        /// </summary>
        /// <param name="pAutor"></param>
        /// <param name="pCorreoID"></param>
        /// <param name="pDestinatario"></param>
        public void EliminarCorreoDefinitivamente(Guid pCorreoID, Guid pAutor, Guid pDestinatario)
        {
            this.CorreoAD.EliminarCorreoDefinitivamente(pCorreoID, pAutor, pDestinatario);
        }

        /// <summary>
        /// Elimina el correo pasado por parámetro
        /// </summary>
        /// <param name="pAutor"></param>
        /// <param name="pCorreoID"></param>
        /// <param name="pDestinatario"></param>
        public void EliminarCorreo(Guid pCorreoID, Guid pAutor, Guid pDestinatario)
        {
            this.CorreoAD.EliminarCorreo(pCorreoID, pAutor, pDestinatario);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pListaIdentidades"></param>
        /// <returns></returns>
        public Dictionary<Guid, string> ObtenerNombresCorreos(List<Guid> pListaIdentidades)
        {
            return this.CorreoAD.ObtenerNombresCorreos(pListaIdentidades);
        }
        #endregion

        #region Privados

        /// <summary>
        /// Valida Correo
        /// </summary>
        /// <param name="pCorreo">Correo para validar</param>
        private void ValidarCorreo(CorreoDS.CorreoInternoRow[] pCorreo)
        {
            if (pCorreo != null)
            {
                for (int i = 0; i < pCorreo.Length; i++)
                {
                    //Asunto cadena vacía
                    if (pCorreo[i].Asunto.Trim().Length == 0)
                    {
                        throw new ErrorDatoNoValido("El asunto del correo '" + pCorreo[i].Asunto + "' no puede ser una cadena vacía");
                    }

                    //Asunto superior a 255 caracteres
                    if (pCorreo[i].Asunto.Trim().Length > 255)
                    {
                        throw new ErrorDatoNoValido("El asunto del correo '" + pCorreo[i].Asunto + "' no puede contener más de 255 caracteres");
                    }

                    //Cuerpo cadena vacía
                    if (!pCorreo[i].IsCuerpoNull())
                    {
                        //La ponemos a Null
                        if (pCorreo[i].Cuerpo.Trim().Length == 0)
                        {
                            pCorreo[i].SetCuerpoNull();
                        }
                    }
                }
            }
        }

        ///// <summary>
        ///// Valida CorreoEnvio
        ///// </summary>
        ///// <param name="pCorreoEnvio">CorreoEnvio para validar</param>
        //private void ValidarCorreoEnvio(CorreoDS.CorreoEnvioRow[] pCorreoEnvio)
        //{
        //    if (pCorreoEnvio != null)
        //    {
        //        for (int i = 0; i < pCorreoEnvio.Length; i++)
        //        {
        //        }
        //    }
        //}

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
        ~CorreoCN()
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
                if (disposing)
                {
                    //Libero todos los recursos administrados que he añadido a esta clase
                    if (CorreoAD != null)
                        CorreoAD.Dispose();
                }
                CorreoAD = null;

                disposed = true;
            }
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// DataAdapter para correo interno
        /// </summary>
        private CorreoAD CorreoAD
        {
            get
            {
                return (CorreoAD)AD;
            }
            set
            {
                AD = value;
            }
        }

        #endregion
    }
}
