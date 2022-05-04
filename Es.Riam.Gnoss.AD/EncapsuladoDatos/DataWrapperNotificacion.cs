using Es.Riam.Gnoss.AD.EntityModel.Models.Notificacion;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Es.Riam.Gnoss.AD.EncapsuladoDatos
{
    [Serializable]
    public class DataWrapperNotificacion : DataWrapperBase
    {
        public List<ColaTwitter> ListaColaTwitter { get; set; }
        public List<EmailIncorrecto> ListaEmailIncorrecto { get; set; }
        public List<Invitacion> ListaInvitacion { get; set; }
        public List<EntityModel.Models.Notificacion.Notificacion> ListaNotificacion { get; set; }
        public List<NotificacionAlertaPersona> ListaNotificacionAlertaPersona { get; set; }
        public List<NotificacionCorreoPersona> ListaNotificacionCorreoPersona { get; set; }
        public List<NotificacionEnvioMasivo> ListaNotificacionEnvioMasivo { get; set; }
        public List<NotificacionParametro> ListaNotificacionParametro { get; set; }
        public List<NotificacionParametroPersona> ListaNotificacionParametroPersona { get; set; }
        public List<NotificacionSolicitud> ListaNotificacionSolicitud { get; set; }
        public List<NotificacionSuscripcion> ListaNotificacionSuscripcion { get; set; }

        public DataWrapperNotificacion()
        {
            ListaColaTwitter = new List<ColaTwitter>();
            ListaEmailIncorrecto = new List<EmailIncorrecto>();
            ListaInvitacion = new List<Invitacion>();
            ListaNotificacion = new List<EntityModel.Models.Notificacion.Notificacion>();
            ListaNotificacionAlertaPersona = new List<NotificacionAlertaPersona>();
            ListaNotificacionCorreoPersona = new List<NotificacionCorreoPersona>();
            ListaNotificacionEnvioMasivo = new List<NotificacionEnvioMasivo>();
            ListaNotificacionParametro = new List<NotificacionParametro>();
            ListaNotificacionParametroPersona = new List<NotificacionParametroPersona>();
            ListaNotificacionSolicitud = new List<NotificacionSolicitud>();
            ListaNotificacionSuscripcion = new List<NotificacionSuscripcion>();
        }

        public override void Merge(DataWrapperBase pDataWrapper)
        {
            DataWrapperNotificacion dataWrapperNotificacion = (DataWrapperNotificacion)pDataWrapper;

            ListaColaTwitter = ListaColaTwitter.Union(dataWrapperNotificacion.ListaColaTwitter).ToList();
            ListaEmailIncorrecto = ListaEmailIncorrecto.Union(dataWrapperNotificacion.ListaEmailIncorrecto).ToList();
            ListaInvitacion = ListaInvitacion.Union(dataWrapperNotificacion.ListaInvitacion).ToList();
            ListaNotificacion = ListaNotificacion.Union(dataWrapperNotificacion.ListaNotificacion).ToList();
            ListaNotificacionAlertaPersona = ListaNotificacionAlertaPersona.Union(dataWrapperNotificacion.ListaNotificacionAlertaPersona).ToList();
            ListaNotificacionCorreoPersona = ListaNotificacionCorreoPersona.Union(dataWrapperNotificacion.ListaNotificacionCorreoPersona).ToList();
            ListaNotificacionEnvioMasivo = ListaNotificacionEnvioMasivo.Union(dataWrapperNotificacion.ListaNotificacionEnvioMasivo).ToList();
            ListaNotificacionParametro = ListaNotificacionParametro.Union(dataWrapperNotificacion.ListaNotificacionParametro).ToList();
            ListaNotificacionParametroPersona = ListaNotificacionParametroPersona.Union(dataWrapperNotificacion.ListaNotificacionParametroPersona).ToList();
            ListaNotificacionSolicitud = ListaNotificacionSolicitud.Union(dataWrapperNotificacion.ListaNotificacionSolicitud).ToList();
            ListaNotificacionSuscripcion = ListaNotificacionSuscripcion.Union(dataWrapperNotificacion.ListaNotificacionSuscripcion).ToList();
        }
    }
}
