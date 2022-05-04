using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Blog
{
    [Serializable]
    [Table("EntradaBlog")]
    public partial class EntradaBlog
    {
        [Column(Order = 0)]
        public Guid BlogID { get; set; }

        [Column(Order = 1)]
        public Guid EntradaBlogID { get; set; }

        [StringLength(255)]
        public string Titulo { get; set; }

        [Required]
        public string Texto { get; set; }

        public Guid AutorID { get; set; }

        public short Visibilidad { get; set; }

        public short Estado { get; set; }

        public bool Eliminado { get; set; }

        public bool Borrador { get; set; }

        public DateTime FechaCreacion { get; set; }

        public DateTime FechaModificacion { get; set; }

        public int Visitas { get; set; }

        public string Tags { get; set; }
    }
}
