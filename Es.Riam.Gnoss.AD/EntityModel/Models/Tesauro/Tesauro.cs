using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Tesauro
{
    [Serializable]
    [Table("Tesauro")]
    public partial class Tesauro
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Tesauro()
        {
            CategoriaTesauro = new HashSet<CategoriaTesauro>();
            CategoriaTesauroSugerencia = new HashSet<CategoriaTesauroSugerencia>();
            TesauroProyecto = new HashSet<TesauroProyecto>();
            TesauroUsuario = new HashSet<TesauroUsuario>();
            TesauroOrganizacion = new HashSet<TesauroOrganizacion>();
        }

        public Guid TesauroID { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CategoriaTesauro> CategoriaTesauro { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CategoriaTesauroSugerencia> CategoriaTesauroSugerencia { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TesauroProyecto> TesauroProyecto { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TesauroUsuario> TesauroUsuario { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TesauroOrganizacion> TesauroOrganizacion { get; set; }
    }
}
