using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS
{
    [Serializable]
    [Table("ProyectoConfigExtraSem")]
    public partial class ProyectoConfigExtraSem
    {
        [Column(Order = 0)]
        public Guid ProyectoID { get; set; }

        [Column(Order = 1)]
        [StringLength(200)]
        public string UrlOntologia { get; set; }

        [Column(Order = 2)]
        [StringLength(200)]
        public string SourceTesSem { get; set; }

        public short Tipo { get; set; }

        [Required]
        public string Nombre { get; set; }

        public string Idiomas { get; set; }

        [StringLength(400)]
        public string PrefijoTesSem { get; set; }

        public bool Editable { get; set; }
    }
}
