using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.VistaVirtualDS
{
    [Serializable]
    [Table("VistaVirtualPersonalizacion")]
    public partial class VistaVirtualPersonalizacion : IDisposable
    {
        private bool disposed = false;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public VistaVirtualPersonalizacion()
        {
            VistaVirtual = new HashSet<VistaVirtual>();
            VistaVirtualCMS = new HashSet<VistaVirtualCMS>();
            VistaVirtualGadgetRecursos = new HashSet<VistaVirtualGadgetRecursos>();
            VistaVirtualProyecto = new HashSet<VistaVirtualProyecto>();
            VistaVirtualRecursos = new HashSet<VistaVirtualRecursos>();
        }

        [Key]
        public Guid PersonalizacionID { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VistaVirtual> VistaVirtual { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VistaVirtualCMS> VistaVirtualCMS { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VistaVirtualGadgetRecursos> VistaVirtualGadgetRecursos { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VistaVirtualProyecto> VistaVirtualProyecto { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VistaVirtualRecursos> VistaVirtualRecursos { get; set; }

        public void Dispose()
        {
            //Falla cuando intenta traerlo con Entity
            //if (!disposed && VistaVirtual != null && VistaVirtualCMS != null && VistaVirtualGadgetRecursos != null && VistaVirtualProyecto != null && VistaVirtualRecursos != null)
            //{
            //    disposed = true;
            //    VistaVirtual.Clear();
            //    VistaVirtual = null;
            //    VistaVirtualCMS.Clear();
            //    VistaVirtualCMS = null;
            //    VistaVirtualGadgetRecursos.Clear();
            //    VistaVirtualGadgetRecursos = null;
            //    VistaVirtualProyecto.Clear();
            //    VistaVirtualProyecto = null;
            //    VistaVirtualRecursos.Clear();
            //    VistaVirtualRecursos = null;
            //}
        }
        ~VistaVirtualPersonalizacion()
        {
            Dispose();
        }
    }
}
