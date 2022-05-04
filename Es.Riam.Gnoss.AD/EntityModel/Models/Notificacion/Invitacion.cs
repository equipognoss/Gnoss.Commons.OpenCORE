using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Notificacion
{
    [Serializable]
    [Table("Invitacion")]
    public partial class Invitacion
    {
        public Guid InvitacionID { get; set; }

        public short TipoInvitacion { get; set; }

        public int Estado { get; set; }

        public DateTime FechaInvitacion { get; set; }

        public DateTime? FechaProcesado { get; set; }

        public Guid NotificacionID { get; set; }

        public Guid IdentidadOrigenID { get; set; }

        public Guid IdentidadDestinoID { get; set; }

        public Guid? ElementoVinculadoID { get; set; }

        public virtual Notificacion Notificacion { get; set; }
    }
}
