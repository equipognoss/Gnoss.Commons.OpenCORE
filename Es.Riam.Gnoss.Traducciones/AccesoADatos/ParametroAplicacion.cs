namespace Traduccion.AccesoADatos
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ParametroAplicacion")]
    public partial class ParametroAplicacion
    {
        [Key]
        [StringLength(100)]
        public string Parametro { get; set; }

        [StringLength(1000)]
        public string Valor { get; set; }
    }
}
