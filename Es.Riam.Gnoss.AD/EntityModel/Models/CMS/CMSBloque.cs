using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.CMS
{
    [Serializable]
    [Table("CMSBloque")]
    public partial class CMSBloque
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CMSBloque()
        {
            CMSBloqueComponente = new HashSet<CMSBloqueComponente>();
        }

        public Guid OrganizacionID { get; set; }

        public Guid ProyectoID { get; set; }

        public short Ubicacion { get; set; }

        [Key]
        public Guid BloqueID { get; set; }

        public Guid? BloquePadreID { get; set; }

        public short Orden { get; set; }

        private string mEstilos;

        public string Estilos
        {
            get 
            { 
                if(mEstilos == null)
                {
                    return "";
                }
                return mEstilos; 
            }
            set 
            { 
                mEstilos = value; 
            }
        }

        public bool Borrador { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CMSBloqueComponente> CMSBloqueComponente { get; set; }

        public virtual CMSPagina CMSPagina { get; set; }
    }
}
