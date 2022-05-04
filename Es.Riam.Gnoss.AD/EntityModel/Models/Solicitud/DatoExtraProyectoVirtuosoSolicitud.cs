using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
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
    [Table("DatoExtraProyectoVirtuosoSolicitud")]
    public partial class DatoExtraProyectoVirtuosoSolicitud
    {
        [Column(Order = 0)]
        public Guid OrganizacionID { get; set; }

        [Column(Order = 1)]
        public Guid ProyectoID { get; set; }

        [Column(Order = 2)]
        public Guid DatoExtraID { get; set; }

        [Column(Order = 3)]
        public Guid SolicitudID { get; set; }

        [Required]
        [StringLength(500)]
        public string Opcion { get; set; }

        public virtual DatoExtraProyectoVirtuoso DatoExtraProyectoVirtuoso { get; set; }
    }
}
