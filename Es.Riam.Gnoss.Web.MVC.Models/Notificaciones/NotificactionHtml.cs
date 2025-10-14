using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.Web.MVC.Models.Notificaciones
{
    public class NotificationHtml
    {
        public Guid perfilID { get; set; }
        public string html { get; set; }
        public bool leida { get; set; }
        public string fechaNotificacion { get; set; }
        public NotificationHtml(Guid pPerfilID, string pHtml, bool pLeida, string pFechaNotificacion)
        {
            this.perfilID = pPerfilID;
            this.html = pHtml;
            this.leida = pLeida;
            this.fechaNotificacion = pFechaNotificacion;
        }
    }
}
