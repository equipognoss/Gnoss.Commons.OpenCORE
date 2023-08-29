using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS
{
    [Serializable]
    [Table("ProyectoPestanyaDashboardAsistente")]
    public partial class ProyectoPestanyaDashboardAsistente
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ProyectoPestanyaDashboardAsistente()
        {
            ProyectoPestanyaDashboardAsistenteDataset = new HashSet<ProyectoPestanyaDashboardAsistenteDataset>();
        }

        [Key]
        public Guid AsisID { get; set; }

        public Guid PestanyaID { get; set; }

        public string Labels { get; set; }

        [Required]
        public string Nombre { get; set; }
        public string Select { get; set; }
        public string Where { get; set; }

        public bool PropExtra { get; set; }

        public int Orden { get; set; }

        public string Tamanyo { get; set; }

        public int Tipo { get; set; }

        public bool Titulo { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProyectoPestanyaDashboardAsistenteDataset> ProyectoPestanyaDashboardAsistenteDataset { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ProyectoPestanyaMenu ProyectoPestanyaMenu { get; set; }
    }
}
