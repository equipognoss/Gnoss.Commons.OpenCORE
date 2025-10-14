using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.Web.MVC.Models.Notificaciones
{
    public class NotificacionDefault
    {
        public string contenidoNotificacion { get; set; }
        public Guid idPerfil { get; set; }
        public string fechaNotificacion { get; set; }
        public bool leida { get; set; }
        public string urlNotificacion { get; set; }

        public NotificacionDefault(string pContenidoNotificacion, Guid pIdPerfil, string pFechaNotificacion, bool pLeida, string pUrlNotificacion)
        {
            this.contenidoNotificacion = pContenidoNotificacion;
            this.idPerfil = pIdPerfil;
            this.fechaNotificacion = pFechaNotificacion;
            this.leida = pLeida;
            this.urlNotificacion = pUrlNotificacion;
        }
    }
}
