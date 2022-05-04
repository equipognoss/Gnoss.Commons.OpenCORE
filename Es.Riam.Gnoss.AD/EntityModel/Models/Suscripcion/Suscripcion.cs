using Es.Riam.Gnoss.AD.EntityModel.Models.Suscripcion;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Suscripcion
{
    [Serializable]
    [Table("Suscripcion")]
    public partial class Suscripcion
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Suscripcion()
        {
            CategoriaTesVinSuscrip = new HashSet<CategoriaTesVinSuscrip>();
            SuscripcionIdentidadProyecto = new HashSet<SuscripcionIdentidadProyecto>();
            SuscripcionBlog = new HashSet<SuscripcionBlog>();
        }

        public Guid SuscripcionID { get; set; }

        public Guid IdentidadID { get; set; }

        public int Periodicidad { get; set; }

        public bool Bloqueada { get; set; }

        public DateTime UltimoEnvio { get; set; }

        public DateTime? FechaSuscripcion { get; set; }

        public int? ScoreUltimoEnvio { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CategoriaTesVinSuscrip> CategoriaTesVinSuscrip { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SuscripcionIdentidadProyecto> SuscripcionIdentidadProyecto { get; set; }

        public virtual SuscripcionTesauroOrganizacion SuscripcionTesauroOrganizacion { get; set; }

        public virtual SuscripcionTesauroProyecto SuscripcionTesauroProyecto { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SuscripcionBlog> SuscripcionBlog { get; set; }

        public virtual SuscripcionTesauroUsuario SuscripcionTesauroUsuario { get; set; }
    }
}
