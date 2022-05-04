using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Notificacion
{
    [Serializable]
    [Table("NotificacionSolicitud")]
    public partial class NotificacionSolicitud
    {
        [Column(Order = 0)]
        public Guid NotificacionID { get; set; }

        [Column(Order = 1)]
        public Guid SolicitudID { get; set; }

        [Column(Order = 2)]
        public Guid OrganizacionID { get; set; }

        [Column(Order = 3)]
        public Guid ProyectoID { get; set; }

        public virtual Notificacion Notificacion { get; set; }
        [ForeignKey("SolicitudID")]
        public virtual Solicitud.Solicitud Solicitud { get; set; }  
    }
}
