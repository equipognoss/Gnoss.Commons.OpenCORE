namespace Es.Riam.Gnoss.AD.EntityModel.Models.Solicitud
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Serializable]
    [Table("SolicitudNuevoUsuario")]
    public partial class SolicitudNuevoUsuario
    {
        [Column(Order = 0)]
        public Guid SolicitudID { get; set; }

        [Column(Order = 1)]
        public Guid UsuarioID { get; set; }

        [Required]
        [StringLength(255)]
        public string Nombre { get; set; }

        [Required]
        [StringLength(255)]
        public string Apellidos { get; set; }

        [StringLength(255)]
        public string URLFoto { get; set; }

        public Guid? PaisID { get; set; }

        public Guid? ProvinciaID { get; set; }

        [Required(AllowEmptyStrings = true)]
        [StringLength(255)]
        public string Provincia { get; set; }

        [StringLength(15)]
        public string CP { get; set; }

        [StringLength(255)]
        public string Poblacion { get; set; }

        [StringLength(1)]
        public string Sexo { get; set; }

        public DateTime? FechaNacimiento { get; set; }

        public bool EsBuscable { get; set; }

        public bool EsBuscableExterno { get; set; }

        [Required(AllowEmptyStrings = true)]
        [StringLength(255)]
        public string Email { get; set; }

        [Required]
        [StringLength(5)]
        public string Idioma { get; set; }

        [Required]
        [StringLength(50)]
        public string NombreCorto { get; set; }

        [StringLength(255)]
        public string EmailTutor { get; set; }

        public bool? CrearClase { get; set; }

        [StringLength(1000)]
        public string ClausulasAdicionales { get; set; }

        public bool CambioPassword { get; set; }

        public string ProyectosAutoAcceso { get; set; }

        public bool FaltanDatos { get; set; }

        public short TipoRegistro { get; set; }

        [StringLength(1000)]
        public string Direccion { get; set; }
        
        [ForeignKey("SolicitudID")]
        public virtual Solicitud Solicitud { get; set; }
    }
}
