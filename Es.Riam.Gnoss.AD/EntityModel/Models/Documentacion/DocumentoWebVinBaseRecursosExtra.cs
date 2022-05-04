using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion
{
    [Serializable]
    [Table("DocumentoWebVinBaseRecursosExtra")]
    public partial class DocumentoWebVinBaseRecursosExtra
    {
        [Column(Order = 0)]
        public Guid DocumentoID { get; set; }

        [Column(Order = 1)]
        public Guid BaseRecursosID { get; set; }

        public int NumeroDescargas { get; set; }

        public int NumeroConsultas { get; set; }

        public DateTime? FechaUltimaVisita { get; set; }

        public virtual DocumentoWebVinBaseRecursos DocumentoWebVinBaseRecursos { get; set; }
    }
}
