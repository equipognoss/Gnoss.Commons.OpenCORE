using Es.Riam.Gnoss.AD.EntityModel.Models.Cache;
using Es.Riam.Gnoss.AD.EntityModel.Models.Cookies;
using Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion;
using Es.Riam.Gnoss.AD.EntityModel.Models.Faceta;
using Es.Riam.Gnoss.AD.EntityModel.Models.Roles;
using Es.Riam.Gnoss.AD.EntityModel.Models.UsuarioDS;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS
{
    [Serializable]
    [Table("Proyecto")]
    public partial class Proyecto
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Proyecto()
        {
            AdministradorGrupoProyecto = new HashSet<AdministradorGrupoProyecto>();
            AdministradorProyecto = new HashSet<AdministradorProyecto>();
            NivelCertificacion = new HashSet<NivelCertificacion>();
            PresentacionListadoSemantico = new HashSet<PresentacionListadoSemantico>();
            PresentacionMapaSemantico = new HashSet<PresentacionMapaSemantico>();
            PresentacionMosaicoSemantico = new HashSet<PresentacionMosaicoSemantico>();
            PresentacionPersonalizadoSemantico = new HashSet<PresentacionPersonalizadoSemantico>();
            PresentacionPestanyaListadoSemantico = new HashSet<PresentacionPestanyaListadoSemantico>();
            PresentacionPestanyaMapaSemantico = new HashSet<PresentacionPestanyaMapaSemantico>();
            PresentacionPestanyaMosaicoSemantico = new HashSet<PresentacionPestanyaMosaicoSemantico>();
            ProyectoGadget = new HashSet<ProyectoGadget>();
            ProyectoRelacionado = new HashSet<ProyectoRelacionado>();
            ProyectoAgCatTesauro = new HashSet<ProyectoAgCatTesauro>();
            Proyecto1 = new HashSet<Proyecto>();
            ProyectoGrafoFichaRec = new HashSet<ProyectoGrafoFichaRec>();
            ProyectoPasoRegistro = new HashSet<ProyectoPasoRegistro>();
            ProyectoPestanyaMenu = new HashSet<ProyectoPestanyaMenu>();
            ProyectoSearchPersonalizado = new HashSet<ProyectoSearchPersonalizado>();
            ProyectoServicioExterno = new HashSet<ProyectoServicioExterno>();
            ProyectoRelacionado1 = new HashSet<ProyectoRelacionado>();
            ProyectoUsuarioIdentidad = new HashSet<ProyectoUsuarioIdentidad>();
            CategoriaProyectoCookie = new HashSet<CategoriaProyectoCookie>();
            FacetaObjetoConocimientoProyectoPestanya = new HashSet<FacetaObjetoConocimientoProyectoPestanya>();
			TareasSegundoPlano = new HashSet<TareasSegundoPlano.TareasSegundoPlano>();
		}

        [Column(Order = 0)]
        public Guid OrganizacionID { get; set; }

        [Column(Order = 1)]
        public Guid ProyectoID { get; set; }

        [Required]
        [StringLength(1000)]
        public string Nombre { get; set; }

        public string Descripcion { get; set; }

        public DateTime? FechaInicio { get; set; }

        public DateTime? FechaFin { get; set; }

        public short TipoProyecto { get; set; }

        public short TipoAcceso { get; set; }

        public int? NumeroRecursos { get; set; }

        public int? NumeroPreguntas { get; set; }

        public int? NumeroDebates { get; set; }

        public int? NumeroMiembros { get; set; }

        public int? NumeroOrgRegistradas { get; set; }

        public int? NumeroArticulos { get; set; }

        public int? NumeroDafos { get; set; }

        public int? NumeroForos { get; set; }

        public Guid? ProyectoSuperiorID { get; set; }

        public bool EsProyectoDestacado { get; set; }


        [StringLength(250)]
        public string URLPropia { get; set; }

        [Required]
        [StringLength(50)]
        public string NombreCorto { get; set; }

        public short Estado { get; set; }

        public bool TieneTwitter { get; set; }

        [StringLength(50)]
        public string TagTwitter { get; set; }

        [StringLength(15)]
        public string UsuarioTwitter { get; set; }

        [StringLength(1000)]
        public string TokenTwitter { get; set; }

        [StringLength(1000)]
        public string TokenSecretoTwitter { get; set; }

        public bool EnviarTwitterComentario { get; set; }

        public bool EnviarTwitterNuevaCat { get; set; }

        public bool EnviarTwitterNuevoAdmin { get; set; }

        public bool EnviarTwitterNuevaPolitCert { get; set; }

        public bool EnviarTwitterNuevoTipoDoc { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TablaBaseProyectoID { get; set; }

        public Guid? ProcesoVinculadoID { get; set; }

        public string Tags { get; set; }

        public bool TagTwitterGnoss { get; set; }

        [StringLength(1000)]
        public string NombrePresentacion { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AdministradorGrupoProyecto> AdministradorGrupoProyecto { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AdministradorProyecto> AdministradorProyecto { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NivelCertificacion> NivelCertificacion { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PresentacionListadoSemantico> PresentacionListadoSemantico { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PresentacionMapaSemantico> PresentacionMapaSemantico { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PresentacionMosaicoSemantico> PresentacionMosaicoSemantico { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PresentacionPersonalizadoSemantico> PresentacionPersonalizadoSemantico { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PresentacionPestanyaListadoSemantico> PresentacionPestanyaListadoSemantico { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PresentacionPestanyaMapaSemantico> PresentacionPestanyaMapaSemantico { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PresentacionPestanyaMosaicoSemantico> PresentacionPestanyaMosaicoSemantico { get; set; }        

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProyectoGadget> ProyectoGadget { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProyectoRelacionado> ProyectoRelacionado { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProyectoAgCatTesauro> ProyectoAgCatTesauro { get; set; }
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ICollection<TareasSegundoPlano.TareasSegundoPlano> TareasSegundoPlano { get; set; }

		public virtual ProyectoCerradoTmp ProyectoCerradoTmp { get; set; }

        public virtual ProyectoCerrandose ProyectoCerrandose { get; set; }

        public virtual ProyectoLoginConfiguracion ProyectoLoginConfiguracion { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Proyecto> Proyecto1 { get; set; }

        public virtual Proyecto Proyecto2 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProyectoGrafoFichaRec> ProyectoGrafoFichaRec { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProyectoPasoRegistro> ProyectoPasoRegistro { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProyectoPestanyaMenu> ProyectoPestanyaMenu { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProyectoSearchPersonalizado> ProyectoSearchPersonalizado { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProyectoServicioExterno> ProyectoServicioExterno { get; set; }

        public virtual ProyectosMasActivos ProyectosMasActivos { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProyectoRelacionado> ProyectoRelacionado1 { get; set; }
        public virtual ICollection<ProyectoUsuarioIdentidad> ProyectoUsuarioIdentidad { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CategoriaProyectoCookie> CategoriaProyectoCookie { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProyectoCookie> ProyectoCookie { get; set; }
        
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FacetaObjetoConocimientoProyectoPestanya> FacetaObjetoConocimientoProyectoPestanya { get; set; }

		public virtual ConfiguracionCachesCostosas ConfiguracionCachesCostosas { get; set; }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ICollection<Rol> Rol { get; set; }
    }
}
