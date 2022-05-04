using Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion;
using Es.Riam.Gnoss.AD.EntityModel.Models.UsuarioDS;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.OrganizacionDS
{
    [Serializable]
    [Table("Organizacion")]
    public partial class Organizacion
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Organizacion()
        {
            AdministradorOrganizacion = new HashSet<AdministradorOrganizacion>();
            PersonaVinculoOrganizacion = new HashSet<PersonaVinculoOrganizacion>();
            OrganizacionParticipaProy = new HashSet<OrganizacionParticipaProy>();
            Organizacion1 = new HashSet<Organizacion>();
            BaseRecursosOrganizacion = new HashSet<BaseRecursosOrganizacion>();
        }

        public Guid OrganizacionID { get; set; }

        [StringLength(255)]
        public string Nombre { get; set; }

        [StringLength(13)]
        public string Telefono { get; set; }

        [StringLength(255)]
        public string Email { get; set; }

        [StringLength(13)]
        public string Fax { get; set; }

        [StringLength(255)]
        public string Web { get; set; }

        public byte[] Logotipo { get; set; }

        public Guid? PaisID { get; set; }

        public Guid? ProvinciaID { get; set; }

        [StringLength(255)]
        public string Provincia { get; set; }

        public Guid? OrganizacionPadreID { get; set; }

        [StringLength(1000)]
        public string Direccion { get; set; }

        [StringLength(15)]
        public string CP { get; set; }

        [StringLength(255)]
        public string Localidad { get; set; }

        public bool EsBuscable { get; set; }

        public bool EsBuscableExternos { get; set; }

        public bool ModoPersonal { get; set; }

        public bool Eliminada { get; set; }

        [Required]
        [StringLength(50)]
        public string NombreCorto { get; set; }

        [StringLength(30)]
        public string CoordenadasLogo { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TablaBaseOrganizacionID { get; set; }

        [StringLength(80)]
        public string Alias { get; set; }

        public int? VersionLogo { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PersonaVinculoOrganizacion> PersonaVinculoOrganizacion { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrganizacionParticipaProy> OrganizacionParticipaProy { get; set; }

        public virtual ConfiguracionGnossOrg ConfiguracionGnossOrg { get; set; }
        [ForeignKey("OrganizacionID")]
        public virtual OrganizacionGnoss OrganizacionGnoss { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Organizacion> Organizacion1 { get; set; }

        public virtual Organizacion Organizacion2 { get; set; }
        [ForeignKey("OrganizacionID")]
        public virtual OrganizacionEmpresa OrganizacionEmpresa { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BaseRecursosOrganizacion> BaseRecursosOrganizacion { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AdministradorOrganizacion> AdministradorOrganizacion { get; set; }

    }
}
