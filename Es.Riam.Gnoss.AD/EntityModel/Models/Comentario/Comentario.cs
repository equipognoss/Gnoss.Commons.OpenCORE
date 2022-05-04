using Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Comentario
{
    [Serializable]
    [Table("Comentario")]
    public class Comentario
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Comentario()
        {
            DocumentoComentario = new HashSet<DocumentoComentario>();
            Comentario1 = new HashSet<Comentario>();
            VotoComentario = new HashSet<VotoComentario>();
            ComentarioBlog = new HashSet<ComentarioBlog>();
            ComentarioCuestion = new HashSet<ComentarioCuestion>();
        }

        public Guid ComentarioID { get; set; }

        public Guid IdentidadID { get; set; }

        public DateTime Fecha { get; set; }

        [Required]
        public string Descripcion { get; set; }

        public bool Eliminado { get; set; }

        public Guid? ComentarioSuperiorID { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ComentarioBlog> ComentarioBlog { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ComentarioCuestion> ComentarioCuestion { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DocumentoComentario> DocumentoComentario { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Comentario> Comentario1 { get; set; }

        public virtual Comentario Comentario2 { get; set; }
        public virtual ICollection<VotoComentario> VotoComentario { get; set; }

        //Campos extra 
        [NotMapped]
        public string NombreAutor { get; set; }
        [NotMapped]
        public string NombreOrganizacion { get; set; }
        [NotMapped]
        public short? TipoPerfil { get; set; }
        [NotMapped]
        public Guid? ProyectoID { get; set; }
        [NotMapped]
        public string NombreCorto { get; set; }
        [NotMapped]
        public string TituloElemento { get; set; }
        [NotMapped]
        public int? Tipo { get; set; }
        [NotMapped]
        public Guid? OrganizacionPerfil { get; set; }
        [NotMapped]
        public Guid? PersonaID { get; set; }
        [NotMapped]
        public bool Leido { get; set; }
        [NotMapped]
        public Guid? OrganizacionID { get; set; }
        [NotMapped]
        public string NombreElemVinculado { get; set; }
        [NotMapped]
        public Guid? ElementoVinculadoID { get; set; }
        [NotMapped]
        public Guid? PadreElemVinID { get; set; }
        [NotMapped]
        public string NombrePadreElemVin { get; set; }


        public override bool Equals(object obj)
        {
            Comentario objetoParametro = null;
            if (obj.GetType() != typeof(Comentario))
            {
                return false;
            }
            else if (ReferenceEquals(null, obj))
            {
                return false;
            }
            else
            {
                objetoParametro = (Comentario)obj;
            }

            if (ComentarioID.Equals(objetoParametro.ComentarioID) && Fecha.Equals(objetoParametro.Fecha) && Descripcion.Equals(objetoParametro.Descripcion) && (IdentidadID.Equals(objetoParametro.IdentidadID)))
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
