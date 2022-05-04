using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.ParametroGeneralDS
{
    [Serializable]
    [Table("ProyectoElementoHtml")]
    public partial class ProyectoElementoHtml
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ProyectoElementoHtml()
        {
            ProyectoElementoHTMLRol = new HashSet<ProyectoElementoHTMLRol>();
        }

        [Column(Order = 0)]
        public Guid OrganizacionID { get; set; }

        [Column(Order = 1)]
        public Guid ProyectoID { get; set; }

        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ElementoHeadID { get; set; }

        public short Ubicacion { get; set; }

        [Required]
        [StringLength(50)]
        public string Etiqueta { get; set; }

        [StringLength(1000)]
        public string Atributos { get; set; }

        public string Contenido { get; set; }

        public bool CargarSinAceptarCookies { get; set; }

        public bool Privado { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProyectoElementoHTMLRol> ProyectoElementoHTMLRol { get; set; }

        public override bool Equals(object obj)
        {
            ProyectoElementoHtml objetoParametro = null;
            if (obj.GetType() != typeof(ProyectoElementoHtml))
            {
                return false;
            }
            else if (ReferenceEquals(null, obj))
            {
                return false;
            }
            else
            {
                objetoParametro = (ProyectoElementoHtml)obj;
            }

            if (OrganizacionID.Equals(objetoParametro.OrganizacionID) && ProyectoID.Equals(objetoParametro.ProyectoID) && ElementoHeadID.Equals(objetoParametro.ElementoHeadID) && Etiqueta.Equals(objetoParametro.Etiqueta))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
