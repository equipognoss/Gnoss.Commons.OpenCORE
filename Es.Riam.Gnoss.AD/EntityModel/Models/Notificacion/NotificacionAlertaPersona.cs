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
    [Table("NotificacionAlertaPersona")]
    public partial class NotificacionAlertaPersona
    {
        [Column(Order = 0)]
        public Guid NotificacionID { get; set; }

        [Column(Order = 1)]
        public Guid PersonaID { get; set; }

        public DateTime? FechaLectura { get; set; }

        public virtual Notificacion Notificacion { get; set; }
    }
}
