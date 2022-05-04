using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.IdentidadDS
{
    [Serializable]
    [Table("PerfilGadget")]
    public partial class PerfilGadget
    {
        [Column(Order = 0)]
        public Guid PerfilID { get; set; }

        [Column(Order = 1)]
        public Guid GadgetID { get; set; }

        [Required]
        [StringLength(200)]
        public string Titulo { get; set; }

        [Required]
        public string Contenido { get; set; }

        public short Orden { get; set; }
    }
}
