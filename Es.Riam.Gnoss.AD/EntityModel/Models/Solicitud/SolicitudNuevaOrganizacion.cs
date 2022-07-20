namespace Es.Riam.Gnoss.AD.EntityModel.Models.Solicitud
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Serializable]
    [Table("SolicitudNuevaOrganizacion")]
    public partial class SolicitudNuevaOrganizacion
    {
        [Column(Order = 0)]
        public Guid SolicitudID { get; set; }

        [Column(Order = 1)]
        public Guid UsuarioAdminID { get; set; }

        [StringLength(255)]
        public string Nombre { get; set; }

        [StringLength(255)]
        public string URLFoto { get; set; }

        public Guid PaisID { get; set; }

        public Guid? ProvinciaID { get; set; }

        [StringLength(255)]
        public string Provincia { get; set; }

        [StringLength(15)]
        public string CP { get; set; }

        [Required]
        [StringLength(255)]
        public string Poblacion { get; set; }

        [Required]
        [StringLength(255)]
        public string Direccion { get; set; }

        [StringLength(255)]
        public string PaginaWeb { get; set; }

        public bool? EsBuscable { get; set; }

        public bool? EsBuscableExternos { get; set; }

        [StringLength(255)]
        public string CargoContactoPrincipal { get; set; }

        [Required]
        [StringLength(255)]
        public string EmailContactoPrincipal { get; set; }

        public bool ModoPersonal { get; set; }

        [Required]
        [StringLength(50)]
        public string NombreCorto { get; set; }

        [StringLength(80)]
        public string Alias { get; set; }

        public virtual Solicitud Solicitud { get; set; }

        public virtual SolicitudNuevaOrgEmp SolicitudNuevaOrgEmp { get; set; }
    }
}
