using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Carga
{
    [Table("Carga")]
    public partial class Carga
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Carga()
        {
            CargaPaquete = new HashSet<CargaPaquete>();
        }

        public Guid CargaID { get; set; }

        [StringLength(1200)]
        public string Nombre { get; set; }

        public short Estado { get; set; }

        public DateTime FechaAlta { get; set; }

        public Guid? OrganizacionID { get; set; }

        public Guid? ProyectoID { get; set; }

        public Guid? IdentidadID { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CargaPaquete> CargaPaquete { get; set; }
    }
}
