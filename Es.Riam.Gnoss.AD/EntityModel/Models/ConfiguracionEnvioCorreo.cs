namespace Es.Riam.Gnoss.AD.EntityModel.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema; 

    [Serializable]
    [Table("ConfiguracionEnvioCorreo")]
    public partial class ConfiguracionEnvioCorreo
    {
        [Key]
        public Guid ProyectoID { get; set; }

        [Required]
        [StringLength(100)]
        public string email { get; set; }

        [Required]
        [StringLength(1000)]
        public string smtp { get; set; }

        public short puerto { get; set; }

        [Required]
        [StringLength(1000)]
        public string usuario { get; set; }

        [Required]
        [StringLength(1000)]
        public string clave { get; set; }

        [Required]
        [StringLength(1000)]
        public string emailsugerencias { get; set; }

        [Required]
        [StringLength(10)]
        public string tipo { get; set; }

        public bool? SSL { get; set; }
    }
}
