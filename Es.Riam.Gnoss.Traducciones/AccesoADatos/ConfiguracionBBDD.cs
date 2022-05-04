namespace Traduccion
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ConfiguracionBBDD")]
    public partial class ConfiguracionBBDD
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short NumConexion { get; set; }

        [Required]
        [StringLength(100)]
        public string NombreConexion { get; set; }

        public short TipoConexion { get; set; }

        [Required]
        [StringLength(1000)]
        public string Conexion { get; set; }

        [StringLength(100)]
        public string DatosExtra { get; set; }

        public bool EsMaster { get; set; }

        public bool LecturaPermitida { get; set; }

        public int? ConectionTimeout { get; set; }
    }
}
