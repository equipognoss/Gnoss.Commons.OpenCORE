using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS
{
    [Table("EcosistemaServicioExterno")]
    public partial class EcosistemaServicioExterno
    {
        [Key]
        [StringLength(150)]
        public string NombreServicio { get; set; }

        [Required]
        public string UrlServicio { get; set; }
    }
}
