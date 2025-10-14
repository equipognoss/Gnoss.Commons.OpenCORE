using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Es.Riam.Gnoss.AD.EntityModel.Models.Faceta;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS
{
    [Serializable]
    [Table("ProyectoPestanyaMenu")]
    public partial class ProyectoPestanyaMenu
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ProyectoPestanyaMenu()
        {
            PresentacionPestanyaListadoSemantico = new HashSet<PresentacionPestanyaListadoSemantico>();
            PresentacionPestanyaMapaSemantico = new HashSet<PresentacionPestanyaMapaSemantico>();
            PresentacionPestanyaMosaicoSemantico = new HashSet<PresentacionPestanyaMosaicoSemantico>();
            ProyectoPestanyaCMS = new HashSet<ProyectoPestanyaCMS>();
            ProyectoPestanyaFiltroOrdenRecursos = new HashSet<ProyectoPestanyaFiltroOrdenRecursos>();
            ProyectoPestanyaMenu1 = new HashSet<ProyectoPestanyaMenu>();
            ProyectoPestanyaMenuRolGrupoIdentidades = new HashSet<ProyectoPestanyaMenuRolGrupoIdentidades>();
            ProyectoPestanyaMenuRolIdentidad = new HashSet<ProyectoPestanyaMenuRolIdentidad>();
            ConfigAutocompletarProy = new HashSet<ConfigAutocompletarProy>();
            FacetaObjetoConocimientoProyectoPestanya = new HashSet<FacetaObjetoConocimientoProyectoPestanya>(); 
            NombreCortoPestanya = "";
        }

        public ProyectoPestanyaMenu(Guid pestanyaId, Guid organizacionID, Guid proyectoID, ProyectoPestanyaMenu p1, short tipoPestanya, string nombre, string ruta, short orden, bool nuevaPestanya, bool visible, short privacidad, string htmlAlternativo, string idiomasDisponibles, string titulo, string nombreCortoPestanya, bool visibleSinAcceso, string cssBodyClass, string metaDescription, bool activa, string ultimoEditor)
        {
            PresentacionPestanyaListadoSemantico = new HashSet<PresentacionPestanyaListadoSemantico>();
            PresentacionPestanyaMapaSemantico = new HashSet<PresentacionPestanyaMapaSemantico>();
            PresentacionPestanyaMosaicoSemantico = new HashSet<PresentacionPestanyaMosaicoSemantico>();
            ProyectoPestanyaCMS = new HashSet<ProyectoPestanyaCMS>();
            ProyectoPestanyaFiltroOrdenRecursos = new HashSet<ProyectoPestanyaFiltroOrdenRecursos>();
            ProyectoPestanyaMenu1 = new HashSet<ProyectoPestanyaMenu>();
            ProyectoPestanyaMenuRolGrupoIdentidades = new HashSet<ProyectoPestanyaMenuRolGrupoIdentidades>();
            ProyectoPestanyaMenuRolIdentidad = new HashSet<ProyectoPestanyaMenuRolIdentidad>();
            FacetaObjetoConocimientoProyectoPestanya = new HashSet<FacetaObjetoConocimientoProyectoPestanya>();
            PestanyaID = pestanyaId;
            OrganizacionID = organizacionID;
            ProyectoID = proyectoID;
            ProyectoPestanyaMenu2 = p1;
            TipoPestanya = tipoPestanya;
            Nombre = nombre;
            Ruta = ruta;
            Orden = orden;
            NuevaPestanya = nuevaPestanya;
            Visible = visible;
            Privacidad = privacidad;
            HtmlAlternativo = htmlAlternativo;
            IdiomasDisponibles = idiomasDisponibles;
            NombreCortoPestanya = nombreCortoPestanya;
            VisibleSinAcceso = visibleSinAcceso;
            CSSBodyClass = cssBodyClass;
            MetaDescription = metaDescription;
            Activa = activa;
            ConfigAutocompletarProy = new HashSet<ConfigAutocompletarProy>();
            UltimoEditor = ultimoEditor;
            FechaCreacion = DateTime.Now;
            FechaModificacion = DateTime.Now;
        }

        public Guid PestanyaID { get; set; }

        public Guid OrganizacionID { get; set; }

        public Guid ProyectoID { get; set; }

        public Guid? PestanyaPadreID { get; set; }

        public short TipoPestanya { get; set; }

        public string Nombre { get; set; }

        public string Ruta { get; set; }

        public short Orden { get; set; }

        public bool NuevaPestanya { get; set; }

        public bool Visible { get; set; }

        public short Privacidad { get; set; }

        public string HtmlAlternativo { get; set; }

        public string IdiomasDisponibles { get; set; }

        public string Titulo { get; set; }

        [Required]
        [StringLength(100)]
        public string NombreCortoPestanya { get; set; }

        public bool VisibleSinAcceso { get; set; }

        public string CSSBodyClass { get; set; }

        public bool Activa { get; set; }

        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public string UltimoEditor { get; set; }

        [StringLength(1000)]
        public string MetaDescription { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PresentacionPestanyaListadoSemantico> PresentacionPestanyaListadoSemantico { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PresentacionPestanyaMapaSemantico> PresentacionPestanyaMapaSemantico { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PresentacionPestanyaMosaicoSemantico> PresentacionPestanyaMosaicoSemantico { get; set; }

        public virtual Proyecto Proyecto { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ProyectoPestanyaBusqueda ProyectoPestanyaBusqueda { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProyectoPestanyaDashboardAsistente> ProyectoPestanyaDashboardAsistente { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProyectoPestanyaCMS> ProyectoPestanyaCMS { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProyectoPestanyaFiltroOrdenRecursos> ProyectoPestanyaFiltroOrdenRecursos { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProyectoPestanyaMenu> ProyectoPestanyaMenu1 { get; set; }

        public virtual ProyectoPestanyaMenu ProyectoPestanyaMenu2 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProyectoPestanyaMenuRolGrupoIdentidades> ProyectoPestanyaMenuRolGrupoIdentidades { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProyectoPestanyaMenuRolIdentidad> ProyectoPestanyaMenuRolIdentidad { get; set; }
        
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ConfigAutocompletarProy> ConfigAutocompletarProy { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FacetaObjetoConocimientoProyectoPestanya> FacetaObjetoConocimientoProyectoPestanya { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProyectoPestanyaMenuVersionPagina> ProyectoPestanyaMenuVersionPagina { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProyectoPestanyaVersionCMS> ProyectoPestanyaVersionCMS { get; set; }
    }
}
