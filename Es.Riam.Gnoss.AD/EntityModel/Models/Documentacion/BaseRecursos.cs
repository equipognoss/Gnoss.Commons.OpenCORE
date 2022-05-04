using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion
{
    [Serializable]
    public class BaseRecursos
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public BaseRecursos()
        {
            BaseRecursosProyecto = new HashSet<BaseRecursosProyecto>();
            DocumentoWebVinBaseRecursos = new HashSet<DocumentoWebVinBaseRecursos>();
            BaseRecursosOrganizacion = new HashSet<BaseRecursosOrganizacion>();
            BaseRecursosUsuario = new HashSet<BaseRecursosUsuario>();
        }

        public Guid BaseRecursosID { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BaseRecursosProyecto> BaseRecursosProyecto { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DocumentoWebVinBaseRecursos> DocumentoWebVinBaseRecursos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BaseRecursosUsuario> BaseRecursosUsuario { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BaseRecursosOrganizacion> BaseRecursosOrganizacion { get; set; }
    }
}
