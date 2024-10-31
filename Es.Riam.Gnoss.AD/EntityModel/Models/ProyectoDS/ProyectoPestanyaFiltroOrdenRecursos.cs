using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS
{
    [Serializable]
    public partial class ProyectoPestanyaFiltroOrdenRecursos
    {
        [Column(Order = 0)]
        public Guid PestanyaID { get; set; }

        [Required]
        public string FiltroOrden { get; set; }

        [Required]
        public string NombreFiltro { get; set; }

        public string Consulta { get; set; }

        public string OrderBy { get; set; }

        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short Orden { get; set; }

        public virtual ProyectoPestanyaMenu ProyectoPestanyaMenu { get; set; }
    }
}
