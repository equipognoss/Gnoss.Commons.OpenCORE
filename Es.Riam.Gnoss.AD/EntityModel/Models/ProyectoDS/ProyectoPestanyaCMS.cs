using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS
{
    [Serializable]
    public partial class ProyectoPestanyaCMS
    {
        [Column(Order = 0)]
        public Guid PestanyaID { get; set; }

        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short Ubicacion { get; set; }

        public virtual ProyectoPestanyaMenu ProyectoPestanyaMenu { get; set; }
    }
}
