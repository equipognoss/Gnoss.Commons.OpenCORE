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
    [Table("AmigoAgGrupo")]

    public class AmigoAgGrupo
    {
        public DateTime Fecha { get; set; }

        [Column(Order = 0)]
        public Guid GrupoID { get; set; }

        [Column(Order = 1)]
        public Guid IdentidadID { get; set; }

        [Column(Order = 2)]
        public Guid IdentidadAmigoID { get; set; }
        public virtual GrupoAmigos GrupoAmigos { get; set; }
    }
}
