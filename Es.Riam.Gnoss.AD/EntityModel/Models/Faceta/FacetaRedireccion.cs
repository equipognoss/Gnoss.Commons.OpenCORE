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
    [Table("FacetaRedireccion")]
    public partial class FacetaRedireccion
    {
        [Key]
        [StringLength(300)]
        public string Faceta { get; set; }

        [Required]
        [StringLength(200)]
        public string Nombre { get; set; }
    }
}
