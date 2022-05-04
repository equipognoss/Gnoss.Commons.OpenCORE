using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS
{
    [Serializable]
    [Table("ProyectoPestanyaRolIdentidad")]
    public partial class ProyectoPestanyaRolIdentidad
    {
        [Column(Order = 0)]
        public Guid ProyectoID { get; set; }

        [Column(Order = 1)]
        [StringLength(400)]
        public string Nombre { get; set; }

        [Column(Order = 2)]
        public Guid PerfilID { get; set; }

        public virtual ProyectoPestanya ProyectoPestanya { get; set; }
    }
}
