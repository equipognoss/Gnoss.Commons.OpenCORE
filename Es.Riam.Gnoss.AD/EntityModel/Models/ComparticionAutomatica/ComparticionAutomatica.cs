using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.ComparticionAutomatica
{
    [Table("ComparticionAutomatica")]
    public partial class ComparticionAutomatica
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ComparticionAutomatica()
        {
            ComparticionAutomaticaMapping = new HashSet<ComparticionAutomaticaMapping>();
            ComparticionAutomaticaReglas = new HashSet<ComparticionAutomaticaReglas>();
        }

        [Key]
        public Guid ComparticionID { get; set; }

        public Guid OrganizacionOrigenID { get; set; }

        public Guid ProyectoOrigenID { get; set; }

        public Guid OrganizacionDestinoID { get; set; }

        public Guid ProyectoDestinoID { get; set; }

        [Required]
        [StringLength(200)]
        public string Nombre { get; set; }

        public Guid IdentidadPublicadoraID { get; set; }

        public bool Eliminada { get; set; }

        public bool ActualizarHome { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ComparticionAutomaticaMapping> ComparticionAutomaticaMapping { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ComparticionAutomaticaReglas> ComparticionAutomaticaReglas { get; set; }
    }
}
