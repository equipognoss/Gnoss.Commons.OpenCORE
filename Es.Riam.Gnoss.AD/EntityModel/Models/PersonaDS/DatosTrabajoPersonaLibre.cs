namespace Es.Riam.Gnoss.AD.EntityModel.Models.PersonaDS
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Serializable]
    [Table("DatosTrabajoPersonaLibre")]
    public partial class DatosTrabajoPersonaLibre
    {
        [Key]
        public Guid PersonaID { get; set; }

        public Guid? PaisTrabajoID { get; set; }

        public Guid? ProvinciaTrabajoID { get; set; }

        [StringLength(255)]
        public string ProvinciaTrabajo { get; set; }

        [StringLength(13)]
        public string TelefonoTrabajo { get; set; }

        [StringLength(13)]
        public string TelefonoMovilTrabajo { get; set; }

        [StringLength(255)]
        public string EmailTrabajo { get; set; }

        [StringLength(255)]
        public string LocalidadTrabajo { get; set; }

        [StringLength(1000)]
        public string DireccionTrabajo { get; set; }

        [StringLength(15)]
        public string CPTrabajo { get; set; }

        [StringLength(255)]
        public string Profesion { get; set; }
        [Required]
        public virtual Persona Persona { get; set; }
    }
}
