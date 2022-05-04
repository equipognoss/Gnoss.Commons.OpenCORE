using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Faceta
{
    [Serializable]
    [Table("FacetaConfigProyChart")]
    public partial class FacetaConfigProyChart
    {
        [Column(Order = 0)]
        public Guid OrganizacionID { get; set; }

        [Column(Order = 1)]
        public Guid ProyectoID { get; set; }

        [Column(Order = 2)]
        public Guid ChartID { get; set; }

        [Required]
        public string Nombre { get; set; }

        public short Orden { get; set; }

        [Required]
        public string SelectConsultaVirtuoso { get; set; }

        [Required]
        public string FiltrosConsultaVirtuoso { get; set; }

        [Required]
        public string JSBase { get; set; }

        [Required]
        public string JSBusqueda { get; set; }

        public string Ontologias { get; set; }
    }
}
