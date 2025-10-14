using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.Web.MVC.Models
{
    [Serializable]
    public class NotificacionesModel
    {
        public string claveCache { get; set; }
        public string contenidoNotificacion { get; set; }
        public Guid idPerfil { get; set; }
        public string fechaNotificacion { get; set; }
        public bool leida { get; set; }
        public string urlNotificacion { get; set; }

        public NotificacionesModel(string pContenidoNotificacion, Guid pIdPerfil, string pFechaNotificacion, bool pLeida, string pUrlNotificacion)
        {
            this.contenidoNotificacion = pContenidoNotificacion;
            this.idPerfil = pIdPerfil;
            this.fechaNotificacion = pFechaNotificacion;
            this.leida = pLeida;
            this.urlNotificacion = pUrlNotificacion;
        }
        public NotificacionesModel(string pContenidoNotificacion, Guid pIdPerfil, string pFechaNotificacion, bool pLeida)
        {
            this.contenidoNotificacion = pContenidoNotificacion;
            this.idPerfil = pIdPerfil;
            this.fechaNotificacion = pFechaNotificacion;
            this.leida = pLeida;
        }
        public NotificacionesModel(string pClaveCache, string pContenidoNotificacion) 
        {
            this.claveCache = pClaveCache;
            this.contenidoNotificacion= pContenidoNotificacion;
        }
        public NotificacionesModel(string pClaveCache, string pContenidoNotificacion, Guid pIdPerfil, string pFechaNotificacion, bool pLeida, string pUrlNotificacion)
        {
            this.claveCache = pClaveCache;
            this.contenidoNotificacion = pContenidoNotificacion;
            this.idPerfil = pIdPerfil;
            this.fechaNotificacion = pFechaNotificacion;
            this.leida = pLeida;
            this.urlNotificacion = pUrlNotificacion;
        }
        public NotificacionesModel(string pClaveCache, string pContenidoNotificacion, Guid pIdPerfil, string pFechaNotificacion, bool pLeida)
        {
            this.claveCache = pClaveCache;
            this.contenidoNotificacion = pContenidoNotificacion;
            this.idPerfil = pIdPerfil;
            this.fechaNotificacion = pFechaNotificacion;
            this.leida = pLeida;
        }
    }
}
