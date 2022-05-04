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
    public partial class FacetaEntidadesExternas
    {
        [Column(Order = 0)]
        public Guid OrganizacionID { get; set; }

        [Column(Order = 1)]
        public Guid ProyectoID { get; set; }

        [Column(Order = 2)]
        [StringLength(300)]
        public string EntidadID { get; set; }

        [Required]
        [StringLength(300)]
        public string Grafo { get; set; }

        public bool EsEntidadSecundaria { get; set; }

        public bool BuscarConRecursividad { get; set; }
    }
}
