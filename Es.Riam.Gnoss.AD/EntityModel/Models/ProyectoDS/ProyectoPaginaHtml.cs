using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS
{
    [Serializable]
    [Table("ProyectoPaginaHtml")]
    public partial class ProyectoPaginaHtml
    {
        [Column(Order = 0)]
        public Guid ProyectoID { get; set; }

        [Column(Order = 1)]
        [StringLength(150)]
        public string Nombre { get; set; }

        [Required]
        public string Html { get; set; }

        [StringLength(5)]
        public string Idioma { get; set; }
    }
}
