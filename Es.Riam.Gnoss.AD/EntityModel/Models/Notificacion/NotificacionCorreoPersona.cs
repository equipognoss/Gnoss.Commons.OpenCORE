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
    [Table("NotificacionCorreoPersona")]
    public partial class NotificacionCorreoPersona
    {
        [Column(Order = 0)]
        public Guid NotificacionID { get; set; }

        public Guid? OrganizacionPersonaID { get; set; }

        [Column(Order = 1)]
        [StringLength(400)]
        public string EmailEnvio { get; set; }

        public Guid? PersonaID { get; set; }

        public short? EstadoEnvio { get; set; }

        public DateTime? FechaEnvio { get; set; }

        public bool EnviadoRabbit { get; set; }

        public virtual Notificacion Notificacion { get; set; }
    }
}
