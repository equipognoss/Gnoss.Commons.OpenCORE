using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS
{
    [Serializable]
    [Table("ProyectoEvento")]
    public partial class ProyectoEvento
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ProyectoEvento()
        {
            ProyectoEventoParticipante = new HashSet<ProyectoEventoParticipante>();
        }

        public Guid OrganizacionID { get; set; }

        public Guid ProyectoID { get; set; }

        [Key]
        public Guid EventoID { get; set; }

        [Required]
        public string Nombre { get; set; }

        public string Descripcion { get; set; }

        public bool Activo { get; set; }

        public short TipoEvento { get; set; }

        public string InfoExtra { get; set; }

        public Guid? ComponenteID { get; set; }

        public bool Interno { get; set; }

        [StringLength(200)]
        public string Grupo { get; set; }

        [StringLength(200)]
        public string UrlRedirect { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProyectoEventoParticipante> ProyectoEventoParticipante { get; set; }
    }
}
