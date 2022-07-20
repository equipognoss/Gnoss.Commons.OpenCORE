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
    [Table("NotificacionParametro")]
    public partial class NotificacionParametro
    {
        [Column(Order = 0)]
        public Guid NotificacionID { get; set; }

        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short ParametroID { get; set; }

        public string Valor { get; set; }

        [ForeignKey("NotificacionID")]
        public virtual Notificacion Notificacion { get; set; }
    }
}
