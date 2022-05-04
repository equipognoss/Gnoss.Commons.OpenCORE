using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Faceta
{
    [Serializable]
    [Table("FacetaHome")]
    public partial class FacetaHome
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FacetaHome()
        {
            FacetaFiltroHome = new HashSet<FacetaFiltroHome>();
        }

        [Column(Order = 0)]
        public Guid OrganizacionID { get; set; }

        [Column(Order = 1)]
        public Guid ProyectoID { get; set; }

        [Column(Order = 2)]
        [StringLength(50)]
        public string ObjetoConocimiento { get; set; }

        [Column(Order = 3)]
        [StringLength(300)]
        public string Faceta { get; set; }

        [Required]
        [StringLength(100)]
        public string PestanyaFaceta { get; set; }

        public short Orden { get; set; }
        
        public virtual FacetaObjetoConocimientoProyecto FacetaObjetoConocimientoProyecto { get; set; }

        public bool MostrarVerMas { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FacetaFiltroHome> FacetaFiltroHome { get; set; }
    }
}
