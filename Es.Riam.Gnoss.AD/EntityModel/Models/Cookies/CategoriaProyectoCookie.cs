using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Cookies
{
    [Table("CategoriaProyectoCookie")]
    [Serializable]
    public partial class CategoriaProyectoCookie
    {
        [Key]
        [Column(Order = 0)]
        public Guid CategoriaID { get; set; }

        public string Nombre { get; set; }

        [StringLength(50)]
        public string NombreCorto { get; set; }

        public string Descripcion { get; set; }

        public bool EsCategoriaTecnica { get; set; }
        public Guid ProyectoID { get; set; }
        public Guid OrganizacionID { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProyectoCookie> ProyectoCookie { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual Proyecto Proyecto { get; set; }
    }
}
