using Es.Riam.Gnoss.AD.EntityModel.Models.Flujos;
using Es.Riam.Gnoss.AD.EntityModel.Models.Roles;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion
{
    [Table("Documento")]
    [Serializable]
    public partial class Documento : IComparable
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Documento()
        {
            DocumentoRolIdentidad = new HashSet<DocumentoRolIdentidad>();
            DocumentoWebVinBaseRecursos = new HashSet<DocumentoWebVinBaseRecursos>();
            HistorialDocumento = new HashSet<HistorialDocumento>();
            DocumentoRolGrupoIdentidades = new HashSet<DocumentoRolGrupoIdentidades>();
            DocumentoVincDoc = new HashSet<DocumentoVincDoc>();
            DocumentoVincDoc1 = new HashSet<DocumentoVincDoc>();
            DocumentoRespuestaVoto = new HashSet<DocumentoRespuestaVoto>();
            VotoDocumento = new HashSet<VotoDocumento>();
            RolOntologiaPermiso = new HashSet<RolOntologiaPermiso>();
			HistorialTransicionDocumento = new HashSet<HistorialTransicionDocumento>();
        }

        public Guid DocumentoID { get; set; }

        public Guid OrganizacionID { get; set; }

        public bool CompartirPermitido { get; set; }

        public Guid? ElementoVinculadoID { get; set; }

        [Required]
        [StringLength(1000)]
        public string Titulo { get; set; }

        public string Descripcion { get; set; }

        public short Tipo { get; set; }

        [StringLength(1200)]
        public string Enlace { get; set; }

        public DateTime? FechaCreacion { get; set; }

        public Guid CreadorID { get; set; }

        public short TipoEntidad { get; set; }

        public string NombreCategoriaDoc { get; set; }

        [StringLength(1000)]
        public string NombreElementoVinculado { get; set; }

        public Guid? ProyectoID { get; set; }

        public bool Publico { get; set; }

        public bool Borrador { get; set; }

        public Guid? FichaBibliograficaID { get; set; }

        public bool CreadorEsAutor { get; set; }

        public double? Valoracion { get; set; }

        public string Autor { get; set; }

        public DateTime? FechaModificacion { get; set; }

        public Guid? IdentidadProteccionID { get; set; }

        public DateTime? FechaProteccion { get; set; }

        public bool Protegido { get; set; }

        public bool PrivadoEditores { get; set; }

        public bool UltimaVersion { get; set; }

        public bool Eliminado { get; set; }

        public int NumeroComentariosPublicos { get; set; }

        public int NumeroTotalVotos { get; set; }

        public int NumeroTotalConsultas { get; set; }

        public int NumeroTotalDescargas { get; set; }

        public int? VersionFotoDocumento { get; set; }

        public int? Rank { get; set; }

        public double? Rank_Tiempo { get; set; }

        [StringLength(10)]
        public string Licencia { get; set; }

        public short Visibilidad { get; set; }

        public string Tags { get; set; }

        public Guid? EstadoID { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DocumentoRolIdentidad> DocumentoRolIdentidad { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DocumentoWebVinBaseRecursos> DocumentoWebVinBaseRecursos { get; set; }

        public virtual VersionDocumento VersionDocumento { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HistorialDocumento> HistorialDocumento { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DocumentoRolGrupoIdentidades> DocumentoRolGrupoIdentidades { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DocumentoVincDoc> DocumentoVincDoc { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DocumentoVincDoc> DocumentoVincDoc1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VotoDocumento> VotoDocumento { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DocumentoRespuestaVoto> DocumentoRespuestaVoto { get; set; }

        public virtual DocumentoLecturaAumentada DocumentoLecturaAumentada { get; set; }

        public virtual DocumentoMetaDatos DocumentoMetaDatos { get; set; }
        
        public virtual IdentidadDS.Identidad Creador { get; set; }

        public virtual Estado Estado { get; set; }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ICollection<RolOntologiaPermiso> RolOntologiaPermiso { get; set; }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ICollection<HistorialTransicionDocumento> HistorialTransicionDocumento { get; set; }

		public int CompareTo(object obj)
        {
            Documento documento = (Documento) obj;
            if (documento.DocumentoID.Equals(this.DocumentoID))
            {
                return 0;
            }

            return 1;
        }
    }
}
