using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.CMS
{
    [Serializable]
    [Table("CMSBloqueComponente")]
    public partial class CMSBloqueComponente
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CMSBloqueComponente()
        {
            CMSBloqueComponentePropiedadComponente = new HashSet<CMSBloqueComponentePropiedadComponente>();
        }

        public Guid OrganizacionID { get; set; }

        public Guid ProyectoID { get; set; }

        [Column(Order = 0)]
        public Guid BloqueID { get; set; }

        [Column(Order = 1)]
        public Guid ComponenteID { get; set; }

        public short Orden { get; set; }

        public virtual CMSBloque CMSBloque { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CMSBloqueComponentePropiedadComponente> CMSBloqueComponentePropiedadComponente { get; set; }

        public virtual CMSComponente CMSComponente { get; set; }
    }
}
