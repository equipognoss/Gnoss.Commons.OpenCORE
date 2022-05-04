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
    [Table("SuscripcionTesauroOrganizacion")]
    public partial class SuscripcionTesauroOrganizacion
    {
        [Key]
        public Guid SuscripcionID { get; set; }

        public Guid OrganizacionID { get; set; }

        public Guid TesauroID { get; set; }

        public virtual Suscripcion Suscripcion { get; set; }
    }
}
