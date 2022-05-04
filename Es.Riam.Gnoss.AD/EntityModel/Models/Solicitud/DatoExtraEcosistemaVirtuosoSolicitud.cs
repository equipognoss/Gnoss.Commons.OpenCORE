using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Solicitud
{
    [Serializable]
    [Table("DatoExtraEcosistemaVirtuosoSolicitud")]
    public partial class DatoExtraEcosistemaVirtuosoSolicitud
    {
        [Column(Order = 0)]
        public Guid DatoExtraID { get; set; }

        [Column(Order = 1)]
        public Guid SolicitudID { get; set; }

        [Required]
        [StringLength(500)]
        public string Opcion { get; set; }

        public virtual Solicitud Solicitud { get; set; }
    }
}
