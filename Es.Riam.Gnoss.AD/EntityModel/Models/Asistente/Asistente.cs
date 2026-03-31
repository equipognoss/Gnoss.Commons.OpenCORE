using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Asistente
{
    public class Asistente
    {
        [Key]
        public Guid AsistenteID {  get; set; }
        public Guid ProyectoID { get; set; }
        public Guid OrganizacionID { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Token { get; set; }
        public string HostAsistente { get; set; }
        public string Icono { get; set; }
        public bool Activo { get; set; }

        public virtual ProyectoDS.Proyecto Proyecto { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AsistenteConfigIdentidad> AsistentesConfigIdentidades { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RolAsistente> RolAsistentes {  get; set; }

    }
}
