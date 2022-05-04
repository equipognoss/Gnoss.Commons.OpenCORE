using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.EntityModelBASE.Models;
using Es.Riam.Gnoss.AD.Notificacion;
using Es.Riam.Gnoss.RabbitMQ;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Es.Riam.Gnoss.Logica.Notificacion
{
    /// <summary>
    /// Lógica para las notificaciones
    /// </summary>
    public class NotificacionCN : BaseCN, IDisposable
    {

        #region Miembros

        private EntityContext mEntityContext;
        private ConfigService mConfigService;
        private LoggingService mLoggingService;
        private EntityContextBASE mEntityContextBASE;

        #endregion

        #region Constructores

        /// <summary>
        /// Constructor sin parámetros
        /// </summary>
        public NotificacionCN(EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication)
        {
            mEntityContext = entityContext;
            mConfigService = configService;
            mLoggingService = loggingService;

            NotificacionAD = new NotificacionAD(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication);
        }

        public NotificacionCN(EntityContext entityContext, LoggingService loggingService, ConfigService configService, EntityContextBASE entitycontextBASE, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(entityContext, loggingService, configService, entitycontextBASE, servicesUtilVirtuosoAndReplication)
        {
            mEntityContext = entityContext;
            mConfigService = configService;
            mLoggingService = loggingService;
            mEntityContextBASE = entitycontextBASE;

            NotificacionAD = new NotificacionAD(loggingService, entityContext, configService, entitycontextBASE, servicesUtilVirtuosoAndReplication);
        }

        /// <summary>
        /// Constructor a partir del fichero de configuración de base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuración de base de datos</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public NotificacionCN(string pFicheroConfiguracionBD, EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication)
        {
            mEntityContext = entityContext;
            mConfigService = configService;
            mLoggingService = loggingService;

            NotificacionAD = new NotificacionAD(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication);
        }

        #endregion

        #region Métodos generales

        /// <summary>
        /// Actualiza la tabla NotificacionAlertaPersona y pone una fecha lectura a la notificiación de la persona que sea con un mensaje id igual al pasado
        /// </summary>
        /// <param name="pPersonaID">Guid de la persona afectada</param>
        /// <param name="pMensajeID">Entero referente a la notificación que se actualiza</param>
        public void MarcarNotificacionPersonaLeida(Guid pPersonaID, int pMensajeID)
        {
            NotificacionAD.MarcarNotificacionPersonaLeida(pPersonaID, pMensajeID);
        }

        /// <summary>
        /// Obtiene todas las notificaciones pendientes de enviar
        /// </summary>
        /// <param name="pCargarFallidas">Indica si es necesario cargar las notificaciones fallidas</param>
        /// <returns>Dataset de notificaciones con las notificaciones pendientes de enviar</returns>
        public DataWrapperNotificacion ObtenerEnvioNotificaciones(bool pCargarFallidas)
        {
            return NotificacionAD.ObtenerEnvioNotificaciones(pCargarFallidas);
        }

        /// <summary>
        /// Obtiene las notificaciones pendientes de enviar
        /// </summary>
        /// <param name="pCargarFallidas">Indica si es necesario cargar las notificaciones fallidas</param>
        /// <returns>Dataset de notificaciones cargado con las tablas 
        /// Notificacion,NotificacionCorreoPersona,NotificacionParametro,NotificacionParametrosPersona</returns>
        public DataWrapperNotificacion ObtenerEnvioNotificacionesRabbitMQ(Guid pNotificacionID)
        {
            return NotificacionAD.ObtenerEnvioNotificacionesRabbitMQ(pNotificacionID);
        }

        /// <summary>
        /// Obtiene todas las notificaciones de envíos masivos pendientes de procesar
        /// </summary>
        /// <returns>Dataset de notificaciones</returns>
        public DataWrapperNotificacion ObtenerNotificacionesEnviosMasivos()
        {
            return NotificacionAD.ObtenerNotificacionesEnviosMasivos();
        }

        /// <summary>
        /// Actualiza Notificacion 
        /// </summary>
        public void ActualizarNotificacion()
        {
            try
            {
                if (Transaccion != null)
                {
                    var notificaciones = mEntityContext.NotificacionCorreoPersona.Local.Where(item => mEntityContext.Entry(item).State.Equals(Microsoft.EntityFrameworkCore.EntityState.Added)).ToList();
                    foreach (var notificacion in notificaciones)
                    {
                        if (!notificacion.EnviadoRabbit)
                        {
                            notificacion.EnviadoRabbit = true;
                        }
                    }
                    NotificacionAD.ActualizarNotificacionEntity();
                    if (mConfigService.ExistRabbitConnection(RabbitMQClient.BD_SERVICIOS_WIN))
                    {
                        using (RabbitMQClient rabbitMQ = new RabbitMQClient(RabbitMQClient.BD_SERVICIOS_WIN, "ColaNotificacion", mLoggingService, mConfigService, "", "ColaNotificacion"))
                        {
                            foreach (var notificacion in notificaciones)
                            {
                                try
                                {
                                    rabbitMQ.AgregarElementoACola(JsonConvert.SerializeObject(notificacion.NotificacionID));
                                }
                                catch (Exception ex)
                                {
                                    mLoggingService.GuardarLogError(ex);
                                    notificacion.EnviadoRabbit = false;
                                    mEntityContext.SaveChanges();
                                }
                            }
                        }
                    }
                }
                else
                {
                    IniciarTransaccion();

                    var notificaciones = mEntityContext.NotificacionCorreoPersona.Local.Where(item => mEntityContext.Entry(item).State.Equals(Microsoft.EntityFrameworkCore.EntityState.Added)).ToList();
                    foreach (var notificacion in notificaciones)
                    {
                        if (!notificacion.EnviadoRabbit)
                        {
                            notificacion.EnviadoRabbit = true;
                        }
                    }
                    NotificacionAD.ActualizarNotificacionEntity();

                    if (mConfigService.ExistRabbitConnection(RabbitMQClient.BD_SERVICIOS_WIN))
                    {
                        using (RabbitMQClient rabbitMQ = new RabbitMQClient(RabbitMQClient.BD_SERVICIOS_WIN, "ColaNotificacion", mLoggingService, mConfigService, "", "ColaNotificacion"))
                        {
                            foreach (var notificacion in notificaciones)
                            {
                                try
                                {
                                    rabbitMQ.AgregarElementoACola(JsonConvert.SerializeObject(notificacion.NotificacionID));
                                }
                                catch (Exception ex)
                                {
                                    mLoggingService.GuardarLogError(ex);
                                    notificacion.EnviadoRabbit = false;
                                    mEntityContext.SaveChanges();
                                }
                            }
                        }
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
        /// Obtiene las invitaciones pendientes de MyGnoss para una persona
        /// </summary>
        /// <param name="pPerfilPersonaID">Identificador del perfil de la persona para la que se obtienen las invitaciones</param>
        /// <param name="pPersonaID">Persona para la que se obtienen las invitaciones, o null si es una organizacion</param>
        /// <param name="pOrganizacionID">Organizacion en la que se encuentra la persona, o null si está en modo personal</param>
        /// <param name="pEsAdmin">Verdad si es administrador de la organizacion en la que se encuentra</param>
        /// <returns>Dataset de notificacion cargado con las invitaciones</returns>
        public DataWrapperNotificacion ObtenerInvitacionesPendientesDeMyGnoss(Guid? pPerfilPersonaID, Guid? pPersonaID, Guid? pOrganizacionID, bool pEsAdmin)
        {
            return NotificacionAD.ObtenerInvitacionesPendientesDeMyGnoss(pPerfilPersonaID, pPersonaID, pOrganizacionID, pEsAdmin);
        }

        /// <summary>
        /// Obtiene el numero de  invitaciones pendientes de MyGnoss para una persona
        /// </summary>
        /// <param name="pPersonaID">Persona para la que se obtienen las invitaciones, o null si es una organizacion</param>
        /// <param name="pOrganizacionID">Organizacion en la que se encuentra la persona, o null si está en modo personal</param>
        /// <param name="pEsAdmin">Verdad si es administrador de la organizacion en la que se encuentra</param>
        /// <returns>numero de invitaciones pendientes de ese perfil(persona,organizacion)</returns>
        public int ObtenerNumeroInvitacionesPendientesDeMyGnoss(Guid? pPersonaID, Guid? pOrganizacionID, bool pEsAdmin)
        {
            return NotificacionAD.ObtenerNumeroInvitacionesPendientesDeMyGnoss(pPersonaID, pOrganizacionID, pEsAdmin);
        }

        /// <summary>
        /// Obtiene invitacion por id
        /// </summary>
        /// <param name="pInvitacionID">Clave de la invitacion que queremos cargar</param>
        /// <returns>DataSet de notificaciones</returns>
        public DataWrapperNotificacion ObtenerInvitacionPorID(Guid pInvitacionID)
        {
            List<Guid> invitaciones = new List<Guid>();
            invitaciones.Add(pInvitacionID);

            return ObtenerInvitacionesPorID(invitaciones);
        }

        /// <summary>
        /// Obtiene invitaciones por id.
        /// </summary>
        /// <param name="pInvitacionID">Claves de las invitaciones que queremos cargar</param>
        /// <returns>DataSet de notificaciones</returns>
        public DataWrapperNotificacion ObtenerInvitacionesPorID(List<Guid> pInvitacionesID)
        {
            return NotificacionAD.ObtenerInvitacionesPorID(pInvitacionesID);
        }

        /// <summary>
        /// Obtiene invitaciones por id.
        /// </summary>
        /// <param name="pInvitacionesID">Claves de las invitaciones que queremos cargar</param>
        /// <returns>DataSet de notificaciones</returns>
        public DataWrapperNotificacion ObtenerInvitacionesPorIDConNombreCorto(List<Guid> pInvitacionesID)
        {
            return NotificacionAD.ObtenerInvitacionesPorIDConNombreCorto(pInvitacionesID);
        }

        /// <summary>
        /// Lanza una consulta DELETE a la BDD para borrar una NotificacionSolicitud de una notificación pasada como parámetro
        /// </summary>
        public void EliminarNotificacionSolicitud(Guid pNotificacionID)
        {
            NotificacionAD.EliminarNotificacionSolicitud(pNotificacionID);
        }

        /// <summary>
        /// Lanza una consulta DELETE a la BDD para borrar una NotificacionCorreoPersona de una notificación pasada como parámetro
        /// </summary>
        public void EliminarNotificacionCorreoPersona(Guid pNotificacionID)
        {
            NotificacionAD.EliminarNotificacionCorreoPersona(pNotificacionID);
        }

        /// <summary>
        /// Lanza una consulta DELETE a la BDD para borrar un NotificacionParametroPersona de una notificación pasada como parámetro
        /// </summary>
        public void EliminarNotificacionParametroPersona(Guid pNotificacionID)
        {
            NotificacionAD.EliminarNotificacionParametroPersona(pNotificacionID);
        }

        /// <summary>
        /// Lanza una consulta DELETE a la BDD para borrar un NotificacionParametro de una notificación pasada como parámetro
        /// </summary>
        public void EliminarNotificacionParametro(Guid pNotificacionID)
        {
            NotificacionAD.EliminarNotificacionParametro(pNotificacionID);
        }

        /// <summary>
        /// Lanza una consulta DELETE a la BDD para borrar una Notificacion pasada como parámetro
        /// </summary>
        public void EliminarNotificacion(Guid pNotificacionID)
        {
            NotificacionAD.EliminarNotificacion(pNotificacionID);
        }

        /// <summary>
        /// Lanza una consulta DELETE a la BDD para borrar una NotificacionAlertaPersona pasada como parámetro
        /// </summary>
        public void EliminarNotificacionAlertaPersona(Guid pNotificacionID)
        {
            NotificacionAD.EliminarNotificacionAlertaPersona(pNotificacionID);
        }

        /// <summary>
        /// Obtiene todas las notificaciones relacionadas con una lista de solicitudes pasada como parámetro
        /// </summary>
        public DataWrapperNotificacion ObtenerNotificacionesDeSolicitudes(List<Guid> pListaSolicitudes)
        {
            return NotificacionAD.ObtenerNotificacionesDeSolicitudes(pListaSolicitudes);
        }

        /// <summary>
        /// Obtiene todas las notificaciones relacionadas con una lista de suscripciones pasada como parámetro
        /// </summary>
        /// <param name="plistaSuscripciones"></param>
        /// <returns>NotificacionDS</returns>
        public DataWrapperNotificacion ObtenerNotificacionesDeSuscripciones(List<Guid> plistaSuscripciones)
        {
            return NotificacionAD.ObtenerNotificacionesDeSuscripciones(plistaSuscripciones);
        }

        /// <summary>
        /// Obtiene NotificacionDS con todas las notificaciones de las suscripciones "NotificacionSuscripcion" 
        /// de todas las identidades del Usuario atraves del pPersonaID vinculada a dicho usuario
        /// </summary>
        /// <param name="pUsuarioID">Clave de la personaVinculada al Usuario</param>
        /// <returns>NotificacionDS</returns>
        public DataWrapperNotificacion ObtenerNotificacionesDeSuscripcionesDeUsuario(Guid pUsuarioID)
        {
            return NotificacionAD.ObtenerNotificacionesDeSuscripcionesDeUsuario(pUsuarioID);
        }

        /// <summary>
        /// Devuelve true si la persona indicada en la organizacion indicada tiene generado el boletin de suscripciones a partir de una fecha
        /// </summary>
        /// <param name="pPersonaID">Id de la persona</param>
        /// <param name="pOrganizacionID">Organizacion de la persona, o null si es personal</param>
        /// <param name="pFecha">Fecha a partir de la cual se comprueba si tiene boletin</param>
        /// <returns>true si existe boletin posterior a la fecha para la persona indicada</returns>
        public bool TienePerfilBoletinPosteriorAFecha(Guid pPersonaID, Guid? pOrganizacionID, DateTime pFecha)
        {
            return NotificacionAD.TienePerfilBoletinPosteriorAFecha(pPersonaID, pOrganizacionID, pFecha);
        }

        /// <summary>
        /// Obtiene las notificaciones que todavia no se han anviado a los administradores de un proyecto para que reabran el proyecto
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto</param>
        /// <returns>NotificacionDS</returns>
        public DataWrapperNotificacion ObtenerNotificacionesReaperturaDeProyecto(Guid pProyectoID)
        {
            return NotificacionAD.ObtenerNotificacionesReaperturaDeProyecto(pProyectoID);
        }

        /// <summary>
        /// Indica si se ha leido la notificacion del mensaje, pMensajeID por la persona, pPersonaID
        /// </summary>
        /// <param name="pPersonaID">Id de la persona</param>
        /// <param name="pMensajeID">Id del mensaje</param>
        /// <returns>TRUE si no se ha leido la notificación, FALSE si se ha leido la notificación</returns>
        public bool TieneNotificacionSinLeer(Guid pPersonaID, int pMensajeID)
        {
            return NotificacionAD.TieneNotificacionSinLeer(pPersonaID, pMensajeID);
        }

        /// <summary>
        /// Obtiene la cola de procesado de twitter de un consumerKey
        /// </summary>
        /// <param name="pNumElementos"></param>
        /// <returns></returns>
        public DataWrapperNotificacion ObtenerColaTwitter(int pNumElementos)
        {
            return NotificacionAD.ObtenerColaTwitter(pNumElementos);
        }

        /// <summary>
        /// Obtiene la lista de ConsumerKey de la cola de Twitter
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> ObtenerListaConsumerKeyTwitter()
        {
            return NotificacionAD.ObtenerListaConsumerKeyTwitter();
        }

        /// <summary>
        /// Actualiza la notificación ID pasada como parámetro con el estado y la fecha de envio.
        /// </summary>
        /// <param name="pNotificacionID">ID de la notificación</param>
        /// <param name="pEstadoEnvio">Estado del envio de la notificación</param>
        /// <param name="pFechaEnvio">Fecha en la que se ha enviado la notificación</param>
        public void ActualizarNotificacion(Guid pNotificacionID, short pEstadoEnvio, DateTime pFechaEnvio)
        {
            NotificacionAD.ActualizarNotificacion(pNotificacionID, pEstadoEnvio, pFechaEnvio);
        }

        public List<ColaCorreoDestinatario> ObtenerColaCorreoDestinatariosPorCorreoID(int pCorreoID)
        {
            return NotificacionAD.ObtenerColaCorreoDestinatariosPorCorreoID(pCorreoID);
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
        ~NotificacionCN()
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

            //Impedimos que se finalice dos veces este objeto
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
                    //Liberamos todos los recursos administrados que hemos añadido a esta clase
                    if (NotificacionAD != null)
                        NotificacionAD.Dispose();
                }
                NotificacionAD = null;
            }
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Dataadapter de notificaciones
        /// </summary>
        private NotificacionAD NotificacionAD
        {
            get
            {
                return (NotificacionAD)AD;
            }
            set
            {
                this.AD = value;
            }
        }

        #endregion
    }
}