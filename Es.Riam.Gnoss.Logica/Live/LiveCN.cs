using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.IdentidadDS;
using Es.Riam.Gnoss.AD.Live;
using Es.Riam.Gnoss.AD.Live.Model;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using System;
using System.Collections.Generic;
using System.Data;

namespace Es.Riam.Gnoss.Logica.Live
{
    /// <summary>
    /// Lógica para Live
    /// </summary>
    public class LiveCN : BaseCN, IDisposable
    {
        #region Constructores

        /// <summary>
        /// Constructor sin parámetros
        /// </summary>
        public LiveCN(EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication)
        {
            mLoggingService = loggingService;
            this.LiveAD = new LiveAD(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication);
        }

        /// <summary>
        /// Constructor a partir del fichero de configuración de base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuración de base de datos</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public LiveCN(string pFicheroConfiguracionBD, EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication)
        {
            mLoggingService = loggingService;
            this.LiveAD = new LiveAD(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication);
        }

        #endregion

        #region Métodos

        /// <summary>
        /// Actualiza BD
        /// </summary>
        /// <param name="pLiveDS">Dataset Live</param>
        public void ActualizarBD(LiveDS pLiveDS, bool pUsarRabbitSiEstaConfigurado = true)
        {
            try
            {
                if (Transaccion != null)
                {
                    LiveAD.ActualizarBD();
                }
                else
                {
                    IniciarTransaccion(false);

                    LiveAD.ActualizarBD();

                    if (pLiveDS != null)
                    {
                        pLiveDS.AcceptChanges();
                    }
                    TerminarTransaccion(true);
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
        /// Actualiza BD y el disminuye el numero de comentarios del contador de perfil.
        /// </summary>
        /// <param name="pLiveDS">Dataset Live</param>
        /// <param name="pPerfilID">ID del perfil actual</param>
        /// <param name="pTiposComentarioActualizar">Tipos de comentarios de los contadores que hay que disminuir</param>
        /// <param name="pDisminuir">Indica si se debe disminuir o aumentar los contadores</param>
        public void ActualizarComentariosBD(LiveDS pLiveDS, Guid pPerfilID, List<TipoLiveComentario> pTiposComentarioActualizar, bool pDisminuir)
        {
            try
            {
                if (Transaccion != null)
                {
                    LiveAD.ActualizarComentariosBD(pPerfilID, pTiposComentarioActualizar, pDisminuir);
                }
                else
                {
                    IniciarTransaccion();

                    LiveAD.ActualizarComentariosBD(pPerfilID, pTiposComentarioActualizar, pDisminuir);

                    if (pLiveDS != null)
                    {
                        pLiveDS.AcceptChanges();
                    }
                    TerminarTransaccion(true);

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
        ///// <summary>
        ///// Recalcula la popularidad (rank) en los miembros del live
        ///// </summary>
        //public void RecalcularRankDeIdentidades(string pNombreBD)
        //{
        //    LiveAD.RecalcularRankDeIdentidades(pNombreBD);
        //}

        ///// <summary>
        ///// Actualiza la popularidad (rank) en los miembros del live
        ///// </summary>
        //public void ActualizarRankMiembro(double pRank,Guid pProyectoID,Guid pPerfilID)
        //{
        //    LiveAD.ActualizarRankMiembro(pRank, pProyectoID, pPerfilID);
        //}

        /// <summary>

        /// <summary>
        /// Obtiene los contadores de un Perfil
        /// </summary>
        /// <param name="pPerfilID">perfilID</param>
        /// <returns>Data set con los contadores recuperados</returns>
        public ContadorPerfil ObtenerContadoresPerfil(Guid pPerfilID)
        {
            return LiveAD.ObtenerContadoresPerfil(pPerfilID);
        }

        /// <summary>
        /// Obtiene los contadores de varios Perfiles
        /// </summary>
        /// <param name="pPerfilID">perfilID</param>
        /// <returns>Data set con los contadores recuperados</returns>
        public List<ContadorPerfil> ObtenerContadoresPerfiles(List<Guid> pPerfilesIDs)
        {
            return LiveAD.ObtenerContadoresPerfiles(pPerfilesIDs);
        }
        #region Nuevos elementos bandeja

        /// <summary>
        /// Aumenta o disminuye en 1 el número de nuevos comentarios de un perfill.
        /// </summary>
        /// <param name="pPerfilID">ID del perfil actual</param>
        /// <param name="pDisminuir">Indica si se debe disminuir o aumentar los contadores</param>
        /// <param name="pFechaProcesado">Fecha en la que se realizo la 1º acción o NULL si no hay tenerla en cuenta</param>
        public void ActualizarNuevosComentarios(Guid pPerfilID, bool pDisminuir, DateTime? pFechaProcesado)
        {
            LiveAD.ActualizarNuevosComentarios(pPerfilID, pDisminuir, pFechaProcesado);
        }

        /// <summary>
        /// Aumenta o disminuye en 1 el número de nuevas suscripciones de un perfill.
        /// </summary>
        /// <param name="pPerfilID">ID del perfil actual</param>
        /// <param name="pDisminuir">Indica si se debe disminuir o aumentar los contadores</param>
        /// <param name="pFechaProcesado">Fecha en la que se realizo la 1º acción o NULL si no hay tenerla en cuenta</param>
        public void ActualizarNuevasSuscripciones(Guid pPerfilID, bool pDisminuir, DateTime? pFechaProcesado)
        {
            LiveAD.ActualizarNuevasSuscripciones(pPerfilID, pDisminuir, pFechaProcesado);
        }

        /// <summary>
        /// Aumenta en 1 el contador de nuevos mensajes.
        /// </summary>
        /// <param name="pPerfilID">ID del perfil actual</param>
        public void AumentarContadorNuevosMensajes(Guid pPerfilID)
        {
            LiveAD.AumentarContadorNuevosMensajes(pPerfilID);
        }

        /// <summary>
        /// Aumenta en 1 el contador de nuevas invitaciones.
        /// </summary>
        /// <param name="pPerfilID">ID del perfil actual</param>
        public void AumentarContadorNuevasInvitaciones(Guid pPerfilID)
        {
            LiveAD.AumentarContadorNuevasInvitaciones(pPerfilID);
        }

        /// <summary>
        /// Aumenta en 1 el contador de nuevas Suscripciones.
        /// </summary>
        /// <param name="pPerfilID">ID del perfil actual</param>
        public void AumentarContadorNuevasSuscripciones(Guid pPerfilID)
        {
            LiveAD.AumentarContadorNuevasSuscripciones(pPerfilID);
        }

        /// <summary>
        /// Aumenta en 1 el contador de nuevos comentarios.
        /// </summary>
        /// <param name="pPerfilID">ID del perfil actual</param>
        public void AumentarContadorNuevosComentarios(Guid pPerfilID)
        {
            LiveAD.AumentarContadorNuevosComentarios(pPerfilID);
        }

        /// <summary>
        /// Disminuye en 1 el contador de mensajes sin leer.
        /// </summary>
        /// <param name="pPerfilID">ID del perfil actual</param>
        public void DisminuirContadorMensajesLeidos(Guid pPerfilID)
        {
            LiveAD.DisminuirContadorMensajesLeidos(pPerfilID);
        }

        /// <summary>
        /// Disminuye en 1 el contador de invitaciones sin leer.
        /// </summary>
        /// <param name="pPerfilID">ID del perfil actual</param>
        public void DisminuirContadoInvitacionesLeidas(Guid pPerfilID)
        {
            LiveAD.DisminuirContadoInvitacionesLeidas(pPerfilID);
        }

        /// <summary>
        /// Disminuye en 1 el contador de Comentarios sin leer.
        /// </summary>
        /// <param name="pPerfilID">ID del perfil actual</param>
        public void DisminuirContadorComentariosLeidos(Guid pPerfilID)
        {
            LiveAD.DisminuirContadorComentariosLeidos(pPerfilID);
        }

        /// <summary>
        /// Disminuye en 1 el contador de suscripciones sin leer.
        /// </summary>
        /// <param name="pPerfilID">ID del perfil actual</param>
        /// <param name="pDisminuir">Indica si se disminuye o no el contador</param>
        public void AumentarDisminuirContadoSuscripcionesLeidas(Guid pPerfilID, bool pDisminuir)
        {
            LiveAD.AumentarDisminuirContadoSuscripcionesLeidas(pPerfilID, pDisminuir);
        }

        /// <summary>
        /// Establece el contador de suscripciones sin leer.
        /// </summary>
        /// <param name="pPerfilID">ID del perfil actual</param>
        /// <param name="pNumero">Número de suscripciones sin leer</param>
        public void EstablecerContadorSuscripcionesSinLeer(Guid pPerfilID, int pNumero)
        {
            LiveAD.EstablecerContadorSuscripcionesSinLeer(pPerfilID, pNumero);
        }

        /// <summary>
        /// Comprueba si un perfil tiene fila de contadorPerfil, si no es así la crea.
        /// </summary>
        /// <param name="pPerfilID">ID del perfil actual</param>
        public void ComprobarPerfilTieneFilaContadorPerfil(Guid pPerfilID)
        {
            LiveAD.ComprobarPerfilTieneFilaContadorPerfil(pPerfilID);
        }

        /// <summary>
        /// Pone a 0 el contador de nuevos mensajes.
        /// </summary>
        /// <param name="pPerfilID">ID del perfil actual</param>
        public void ResetearContadorNuevosMensajes(Guid pPerfilID)
        {
            LiveAD.ResetearContadorNuevosMensajes(pPerfilID);
        }

        /// <summary>
        /// Pone a 0 el contador de nuevas invitaciones.
        /// </summary>
        /// <param name="pPerfilID">ID del perfil actual</param>
        public void ResetearContadorNuevasInvitaciones(Guid pPerfilID)
        {
            LiveAD.ResetearContadorNuevasInvitaciones(pPerfilID);
        }

        /// <summary>
        /// Pone a 0 el contador de nuevos comentarios.
        /// </summary>
        /// <param name="pPerfilID">ID del perfil actual</param>
        public void ResetearContadorNuevosComentarios(Guid pPerfilID)
        {
            LiveAD.ResetearContadorNuevosComentarios(pPerfilID);
        }

        /// <summary>
        /// Pone a 0 el contador de nuevas sucripciones.
        /// </summary>
        /// <param name="pPerfilID">ID del perfil actual</param>
        public void ResetearContadorNuevasSuscripciones(Guid pPerfilID)
        {
            LiveAD.ResetearContadorNuevasSuscripciones(pPerfilID);
        }

        #endregion

        public void ActualizarContadorPerfilMensajesSinLeer(Guid pPerfilID, int pMensajesSinLeer)
        {
            LiveAD.ActualizarContadorPerfilMensajesSinLeer(pPerfilID, pMensajesSinLeer);
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Determina si está disposed
        /// </summary>
        private bool mDisposed = false;

        /// <summary>
        /// Destructor
        /// </summary>
        ~LiveCN()
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
                    if (LiveAD != null)
                    {
                        LiveAD.Dispose();
                    }
                }
                LiveAD = null;
            }
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Dataadapter para Live
        /// </summary>
        private LiveAD LiveAD
        {
            get
            {
                return (LiveAD)AD;
            }
            set
            {
                this.AD = value;
            }
        }

        #endregion
    }
}
