using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS
{
    [Serializable]
    [Table("ProyectoPerfilNumElem")]
    public partial class ProyectoPerfilNumElem
    {
        [Column(Order = 0)]
        public Guid ProyectoID { get; set; }

        [Column(Order = 1)]
        public Guid PerfilID { get; set; }

        public int NumRecursos { get; set; }
    }
}
