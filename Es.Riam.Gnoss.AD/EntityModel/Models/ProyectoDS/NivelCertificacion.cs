using Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS
{
    [Serializable]
    [Table("NivelCertificacion")]
    public partial class NivelCertificacion
    {
        public Guid NivelCertificacionID { get; set; }

        public Guid OrganizacionID { get; set; }

        public Guid ProyectoID { get; set; }

        public short Orden { get; set; }

        [Required]        
        public string Descripcion { get; set; }

        public virtual Proyecto Proyecto { get; set; }
        public NivelCertificacion()
        {
            DocumentoWebVinBaseRecursos = new HashSet<DocumentoWebVinBaseRecursos>();
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DocumentoWebVinBaseRecursos> DocumentoWebVinBaseRecursos { get; set; }
    }

}
