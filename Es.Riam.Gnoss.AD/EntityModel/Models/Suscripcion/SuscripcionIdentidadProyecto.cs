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
    [Table("SuscripcionIdentidadProyecto")]
    public partial class SuscripcionIdentidadProyecto
    {
        [Column(Order = 0)]
        public Guid SuscripcionID { get; set; }

        [Column(Order = 1)]
        public Guid IdentidadID { get; set; }

        public Guid ProyectoID { get; set; }

        public Guid OrganizacionID { get; set; }

        public virtual Suscripcion Suscripcion { get; set; }
    }
}
