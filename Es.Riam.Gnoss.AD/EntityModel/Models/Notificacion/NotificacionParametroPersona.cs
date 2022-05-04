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
    [Table("NotificacionParametroPersona")]
    public partial class NotificacionParametroPersona
    {
        [Column(Order = 0)]
        public Guid NotificacionID { get; set; }

        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short ParametroID { get; set; }

        [Column(Order = 2)]
        public Guid PersonaID { get; set; }

        [Required]
        public string Valor { get; set; }

        public virtual Notificacion Notificacion { get; set; }
    }
}
