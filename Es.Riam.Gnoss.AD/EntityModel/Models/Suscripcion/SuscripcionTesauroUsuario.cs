using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Suscripcion
{
    [Serializable]
    [Table("SuscripcionTesauroUsuario")]
    public partial class SuscripcionTesauroUsuario
    {
        [Key]
        public Guid SuscripcionID { get; set; }

        public Guid UsuarioID { get; set; }

        public Guid TesauroID { get; set; }

        public virtual Suscripcion Suscripcion { get; set; }
    }
}
