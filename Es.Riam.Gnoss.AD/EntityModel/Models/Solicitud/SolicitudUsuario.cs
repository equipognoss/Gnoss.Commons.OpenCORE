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
    [Table("SolicitudUsuario")]
    public partial class SolicitudUsuario
    {
        [Column(Order = 0)]
        public Guid SolicitudID { get; set; }

        [Column(Order = 1)]
        public Guid UsuarioID { get; set; }

        [Column(Order = 2)]
        public Guid PersonaID { get; set; }

        public Guid PerfilID { get; set; }

        [StringLength(1000)]
        public string ClausulasAdicionales { get; set; }

        public virtual Solicitud Solicitud { get; set; }
    }
}
