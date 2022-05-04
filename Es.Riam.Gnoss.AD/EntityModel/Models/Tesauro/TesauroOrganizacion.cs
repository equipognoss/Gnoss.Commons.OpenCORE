using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Tesauro
{
    [Serializable]
    [Table("TesauroOrganizacion")]
    public partial class TesauroOrganizacion
    {
        [Column(Order = 0)]
        public Guid TesauroID { get; set; }

        [Column(Order = 1)]
        public Guid OrganizacionID { get; set; }

        public Guid? CategoriaTesauroPublicoID { get; set; }

        public Guid? CategoriaTesauroPrivadoID { get; set; }

        public Guid? CategoriaTesauroFavoritosID { get; set; }

        public virtual Tesauro Tesauro { get; set; }
    }
}
