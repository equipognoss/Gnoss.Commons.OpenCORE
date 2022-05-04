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
    [Table("IdentidadProyecto")]
    public class IdentidadProyecto
    {
        public Identidad Identidad { get; set; }
        
        [Required]
        [StringLength(1000)]
        public string Nombre { get; set; }
    }
}
