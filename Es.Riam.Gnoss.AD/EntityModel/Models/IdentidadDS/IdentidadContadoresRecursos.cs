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
    [Table("IdentidadContadoresRecursos")]
    public partial class IdentidadContadoresRecursos
    {
        [Column(Order = 0)]
        public Guid IdentidadID { get; set; }

        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short Tipo { get; set; }

        [Column(Order = 2)]
        [StringLength(200)]
        public string NombreSem { get; set; }

        public int Publicados { get; set; }

        public int Compartidos { get; set; }

        public int Comentarios { get; set; }
    }
}
