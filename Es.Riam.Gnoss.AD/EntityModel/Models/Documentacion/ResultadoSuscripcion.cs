using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion
{
    [Serializable]
    [Table("ResultadoSuscripcion")]
    public partial class ResultadoSuscripcion
    {
        [Column(Order = 0)]
        public Guid SuscripcionID { get; set; }

        [Column(Order = 1)]
        public Guid RecursoID { get; set; }

        [Required]
        [StringLength(1000)]
        public string Titulo { get; set; }

        public string Descripcion { get; set; }

        public short TipoResultado { get; set; }

        public DateTime FechaModificacion { get; set; }

        public Guid? AutorID { get; set; }

        [Required]
        [StringLength(1000)]
        public string OrigenNombre { get; set; }

        [Required]
        [StringLength(50)]
        public string OrigenNombreCorto { get; set; }

        public Guid OrigenID { get; set; }

        public short? TipoDocumento { get; set; }

        [StringLength(1000)]
        public string Enlace { get; set; }

        public bool Leido { get; set; }

        public bool Sincaducidad { get; set; }

        public DateTime? FechaProcesado { get; set; }
    }
}
