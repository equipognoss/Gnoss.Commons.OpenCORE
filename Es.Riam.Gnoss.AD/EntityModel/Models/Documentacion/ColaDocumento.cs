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
    [Table("ColaDocumento")]
    public partial class ColaDocumento
    {
        public int ID { get; set; }

        public Guid DocumentoID { get; set; }

        public short AccionRealizada { get; set; }

        public short Estado { get; set; }

        public DateTime FechaEncolado { get; set; }

        public DateTime? FechaProcesado { get; set; }

        public short Prioridad { get; set; }

        public string InfoExtra { get; set; }

        public long? EstadoCargaID { get; set; }
    }
}
