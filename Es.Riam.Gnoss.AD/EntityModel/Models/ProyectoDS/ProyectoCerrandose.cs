using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS
{
    [Serializable]
    [Table("ProyectoCerrandose")]
    public partial class ProyectoCerrandose
    {
        [Column(Order = 0)]
        public Guid OrganizacionID { get; set; }

        [Column(Order = 1)]
        public Guid ProyectoID { get; set; }

        public DateTime FechaCierre { get; set; }

        public int PeriodoDeGracia { get; set; }

        public virtual Proyecto Proyecto { get; set; }
    }
}
