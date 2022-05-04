using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Carga
{
    [Table("CargaPaquete")]
    public partial class CargaPaquete
    {
        [Key]
        public Guid PaqueteID { get; set; }

        public Guid CargaID { get; set; }

        [Required]
        [StringLength(2000)]
        public string RutaOnto { get; set; }

        [Required]
        [StringLength(2000)]
        public string RutaBusqueda { get; set; }

        [Required]
        [StringLength(2000)]
        public string RutaSQL { get; set; }

        public short Estado { get; set; }

        [StringLength(4000)]
        public string Error { get; set; }

        public DateTime? FechaAlta { get; set; }

        public DateTime? FechaProcesado { get; set; }

        [Required]
        [StringLength(2000)]
        public string Ontologia { get; set; }

        public bool Comprimido { get; set; }

        public virtual Carga Carga { get; set; }
    }
}
