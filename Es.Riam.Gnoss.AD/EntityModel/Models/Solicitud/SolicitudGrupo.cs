using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Solicitud
{
    [Serializable]
    [Table("SolicitudGrupo")]
    public partial class SolicitudGrupo
    {
        [Column(Order = 0)]
        public Guid SolicitudID { get; set; }

        [Column(Order = 1)]
        public Guid GrupoID { get; set; }

        [Column(Order = 2)]
        public Guid IdentidadID { get; set; }

        public virtual Solicitud Solicitud { get; set; }
    }
}
