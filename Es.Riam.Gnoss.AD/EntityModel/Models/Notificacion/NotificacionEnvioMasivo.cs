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
    [Table("NotificacionEnvioMasivo")]
    public partial class NotificacionEnvioMasivo
    {
        [Key]
        public Guid NotificacionID { get; set; }

        [Required]
        public string Destinatarios { get; set; }

        public short Prioridad { get; set; }

        public short EstadoEnvio { get; set; }

        public DateTime? FechaEnvio { get; set; }

        public virtual Notificacion Notificacion { get; set; }
    }
}
