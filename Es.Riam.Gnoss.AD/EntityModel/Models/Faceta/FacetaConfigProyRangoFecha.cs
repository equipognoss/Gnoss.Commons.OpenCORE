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
    [Table("FacetaConfigProyRangoFecha")]
    public partial class FacetaConfigProyRangoFecha
    {
        [Column(Order = 0)]
        public Guid OrganizacionID { get; set; }

        [Column(Order = 1)]
        public Guid ProyectoID { get; set; }

        [Column(Order = 2)]
        public string PropiedadNueva { get; set; }

        [Column(Order = 3)]
        public string PropiedadInicio { get; set; }

        [Column(Order = 4)]
        public string PropiedadFin { get; set; }
    }
}
