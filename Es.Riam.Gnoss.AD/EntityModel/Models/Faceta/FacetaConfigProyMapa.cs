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
    [Table("FacetaConfigProyMapa")]
    public partial class FacetaConfigProyMapa
    {
        [Key]
        [Column(Order = 0)]
        public Guid OrganizacionID { get; set; }

        [Key]
        [Column(Order = 1)]
        public Guid ProyectoID { get; set; }

        public string PropLatitud { get; set; }

        public string PropLongitud { get; set; }

        public string PropRuta { get; set; }

        public string ColorRuta { get; set; }
    }
}
