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
    [Table("FacetaObjetoConocimiento")]
    public partial class FacetaObjetoConocimiento
    {
        [Column(Order = 0)]
        [StringLength(50)]
        public string ObjetoConocimiento { get; set; }

        [Column(Order = 1)]
        [StringLength(300)]
        public string Faceta { get; set; }

        [Required]
        public string Nombre { get; set; }

        public short Orden { get; set; }

        public bool Autocompletar { get; set; }

        public short? TipoPropiedad { get; set; }

        public short TipoDisenio { get; set; }

        public short ElementosVisibles { get; set; }

        public short AlgoritmoTransformacion { get; set; }

        public bool EsSemantica { get; set; }

        public short Mayusculas { get; set; }

        public bool EsPorDefecto { get; set; }

        [Required]
        public string NombreFaceta { get; set; }

        public bool ComportamientoOr { get; set; }
    }
}
