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
    [Table("NotificacionSuscripcion")]
    public partial class NotificacionSuscripcion
    {
        [Key]
        [Column(Order = 0)]
        public Guid NotificacionID { get; set; }

        [Key]
        [Column(Order = 1)]
        public Guid SuscripcionID { get; set; }

        public virtual Notificacion Notificacion { get; set; }
    }
}
