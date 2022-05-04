using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.ComparticionAutomatica
{
    public partial class ComparticionAutomaticaReglas
    {
        [Column(Order = 0)]
        public Guid ComparticionID { get; set; }

        [Column(Order = 1)]
        [StringLength(200)]
        public string Regla { get; set; }

        [Required]
        [StringLength(10)]
        public string Navegacion { get; set; }

        public virtual ComparticionAutomatica ComparticionAutomatica { get; set; }
    }
}
