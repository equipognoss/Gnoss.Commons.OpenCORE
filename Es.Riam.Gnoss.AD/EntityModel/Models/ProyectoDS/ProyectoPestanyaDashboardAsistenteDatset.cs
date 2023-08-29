using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS
{
    [Serializable]
    [Table("ProyectoPestanyaDashboardAsistenteDataset")]
    public partial class ProyectoPestanyaDashboardAsistenteDataset
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ProyectoPestanyaDashboardAsistenteDataset() { }

        [Key]
        public Guid DatasetID { get; set; }

        public Guid AsisID { get; set; }

        [Required(AllowEmptyStrings = true)]
        public string Datos { get; set; }
        public string Nombre { get; set; }

        public string Color { get; set; }
        public int Orden { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ProyectoPestanyaDashboardAsistente ProyectoPestanyaDashboardAsistente { get; set; }
    }
}
