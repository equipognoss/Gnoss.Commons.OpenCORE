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
    [Table("DatoExtraEcosistemaOpcionPerfil")]
    public partial class DatoExtraEcosistemaOpcionPerfil
    {
        [Column(Order = 0)]
        public Guid DatoExtraID { get; set; }

        [Column(Order = 1)]
        public Guid OpcionID { get; set; }

        [Column(Order = 2)]
        public Guid PerfilID { get; set; }

        public virtual Perfil Perfil { get; set; }
    }
}
