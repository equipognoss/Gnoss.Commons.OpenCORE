using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.ComparticionAutomatica
{
    [Table("ComparticionAutomaticaMapping")]
    public partial class ComparticionAutomaticaMapping
    {
        [Column(Order = 0)]
        public Guid ComparticionID { get; set; }

        [Column(Order = 1)]
        [StringLength(200)]
        public string ReglaMapping { get; set; }

        [Column(Order = 2)]
        public Guid TesauroID { get; set; }

        [Column(Order = 3)]
        public Guid CategoriaTesauroID { get; set; }

        public int GrupoMapping { get; set; }

        public virtual ComparticionAutomatica ComparticionAutomatica { get; set; }
    }
}
