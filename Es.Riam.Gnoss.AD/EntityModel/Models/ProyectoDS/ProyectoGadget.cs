using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS
{
    [Serializable]
    [Table("ProyectoGadget")]
    public partial class ProyectoGadget
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ProyectoGadget()
        {
            ProyectoGadgetIdioma = new HashSet<ProyectoGadgetIdioma>();
        }

        public ProyectoGadget(Guid organizacionID, Guid proyectoID, Guid gadgetID, string titulo, string contenido, int orden, short tipo, string ubicacion, string clases, short tipoUbicacion, bool visible, bool multiIdioma, Guid personalizacionComponenteID, bool cargarPorAjax, string comunidadDestinoFiltros, string nombreCorto)
        {
            OrganizacionID = organizacionID;
            ProyectoID = proyectoID;
            GadgetID = gadgetID;
            Titulo = titulo;
            Contenido = contenido;
            Orden = orden;
            Tipo = tipo;
            Ubicacion = ubicacion;
            Clases = clases;
            TipoUbicacion = tipoUbicacion;
            Visible = visible;
            MultiIdioma = multiIdioma;
            PersonalizacionComponenteID = personalizacionComponenteID;
            CargarPorAjax = cargarPorAjax;
            ComunidadDestinoFiltros = comunidadDestinoFiltros;
            NombreCorto = nombreCorto;
            ProyectoGadgetIdioma = new HashSet<ProyectoGadgetIdioma>();
            Proyecto = null;
            ProyectoGadgetContextoHTMLplano = null;
            ProyectoGadgetContexto = null;
        }

        [Column(Order = 0)]
        public Guid OrganizacionID { get; set; }

        [Column(Order = 1)]
        public Guid ProyectoID { get; set; }

        [Column(Order = 2)]
        public Guid GadgetID { get; set; }

        [Required]
        public string Titulo { get; set; }

        [Required(AllowEmptyStrings =true)]
        public string Contenido { get; set; }

        public int Orden { get; set; }

        public short Tipo { get; set; }

        [StringLength(10)]
        public string Ubicacion { get; set; }

        [StringLength(100)]
        public string Clases { get; set; }

        public short TipoUbicacion { get; set; }

        public bool Visible { get; set; }

        public bool MultiIdioma { get; set; }

        public Guid? PersonalizacionComponenteID { get; set; }

        public bool CargarPorAjax { get; set; }

        public string ComunidadDestinoFiltros { get; set; }

        [StringLength(50)]
        public string NombreCorto { get; set; }

        public virtual Proyecto Proyecto { get; set; }

        public virtual ProyectoGadgetContextoHTMLplano ProyectoGadgetContextoHTMLplano { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProyectoGadgetIdioma> ProyectoGadgetIdioma { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<ProyectoGadgetContexto> ProyectoGadgetContexto { get; set; }
        [NotMapped]
        public virtual ProyectoGadgetContexto ProyectoGadgetContexto { get; set; }
    }
}
