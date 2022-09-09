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
    [Table("ProyectoCookie")]
    [Serializable]
    public partial class ProyectoCookie
    {
        [Key]
        [Column(Order = 0)]
        public Guid CookieID { get; set; }

        [StringLength(1000)]
        public string Nombre { get; set; }

        [StringLength(50)]
        public string NombreCorto { get; set; }

        public short Tipo { get; set; }

        public string Descripcion { get; set; }

        public bool EsEditable { get; set; }

        public Guid CategoriaID { get; set; }

        public Guid ProyectoID { get; set; }
        
        public Guid OrganizacionID { get; set; }

        public virtual CategoriaProyectoCookie CategoriaProyectoCookie { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual Proyecto Proyecto { get; set; }
    }
}
