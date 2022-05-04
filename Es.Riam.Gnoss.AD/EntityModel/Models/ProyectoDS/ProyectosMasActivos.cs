using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS
{
    [Serializable]
    public partial class ProyectosMasActivos
    {
        [Column(Order = 0)]
        public Guid OrganizacionID { get; set; }

        [Column(Order = 1)]
        public Guid ProyectoID { get; set; }

        [Required]
        [StringLength(1000)]
        public string Nombre { get; set; }

        public int NumeroConsultas { get; set; }

        public int Peso { get; set; }

        public virtual Proyecto Proyecto { get; set; }
    }
}
