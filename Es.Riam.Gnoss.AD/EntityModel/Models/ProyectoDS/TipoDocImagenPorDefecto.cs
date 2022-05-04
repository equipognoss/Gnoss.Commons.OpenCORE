using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS
{
    [Serializable]
    [Table("TipoDocImagenPorDefecto")]
    public partial class TipoDocImagenPorDefecto
    {
        [Column(Order = 0)]
        public Guid ProyectoID { get; set; }

        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short TipoRecurso { get; set; }

        [Column(Order = 2)]
        public Guid OntologiaID { get; set; }

        [Required]
        [StringLength(300)]
        public string UrlImagen { get; set; }
    }
}
