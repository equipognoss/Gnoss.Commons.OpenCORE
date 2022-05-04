using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Blog
{
    [Serializable]
    [Table("Blog")]
    public partial class Blog
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Blog()
        {
            BlogAgCatTesauro = new HashSet<BlogAgCatTesauro>();
            BlogComunidad = new HashSet<BlogComunidad>();
        }

        public Guid BlogID { get; set; }

        [Required]
        [StringLength(50)]
        public string Titulo { get; set; }

        [StringLength(255)]
        public string Subtitulo { get; set; }

        [StringLength(2000)]
        public string Descripcion { get; set; }

        public Guid AutorID { get; set; }

        public DateTime Fecha { get; set; }

        public short Visibilidad { get; set; }

        public short ArticulosPorPagina { get; set; }

        public bool PermitirComentarios { get; set; }

        public bool PermitirTrackbacks { get; set; }

        public bool CrearFuentesWeb { get; set; }

        public bool VisibilidadListadosBusquedas { get; set; }

        public bool VisibilidadBuscadoresWeb { get; set; }

        public int? ArticulosTotales { get; set; }

        public int? ComentariosTotales { get; set; }

        public bool Eliminado { get; set; }

        [Required]
        [StringLength(30)]
        public string NombreCorto { get; set; }

        public int Seguidores { get; set; }

        public bool PermiteActualizarTwitter { get; set; }

        [StringLength(10)]
        public string Licencia { get; set; }

        public string Tags { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BlogAgCatTesauro> BlogAgCatTesauro { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BlogComunidad> BlogComunidad { get; set; }
    }
}
