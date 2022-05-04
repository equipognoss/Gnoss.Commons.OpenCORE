using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.IdentidadDS
{
    [Serializable]
    [Table("GrupoIdentidadesParticipacion")]
    public partial class GrupoIdentidadesParticipacion
    {
        [Column(Order = 0)]
        public Guid GrupoID { get; set; }

        [Column(Order = 1)]
        public Guid IdentidadID { get; set; }

        public DateTime FechaAlta { get; set; }

        public DateTime? FechaBaja { get; set; }

        public virtual GrupoIdentidades GrupoIdentidades { get; set; }

        public virtual Identidad Identidad { get; set; }
    }
}
