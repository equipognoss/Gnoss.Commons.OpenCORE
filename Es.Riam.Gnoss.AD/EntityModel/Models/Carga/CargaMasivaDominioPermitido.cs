using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Carga
{
    [Table("CargaMasivaDominioPermitido")]
    public partial class CargaMasivaDominioPermitido
    {
        [Column(Order =0)]
        public Guid ProyectoID { get; set; }

        [Column(Order =1)]
        [StringLength(255)]
        public string Dominio { get; set; }

        public DateTime FechaCreacion { get; set; }

        public Guid IdentidadID { get; set; }
    }
}
