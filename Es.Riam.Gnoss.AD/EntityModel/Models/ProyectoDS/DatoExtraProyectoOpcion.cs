using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS
{
    [Serializable]
    [Table("DatoExtraProyectoOpcion")]
    public partial class DatoExtraProyectoOpcion
    {
        [Column(Order = 0)]
        public Guid OrganizacionID { get; set; }

        [Column(Order = 1)]
        public Guid ProyectoID { get; set; }

        [Column(Order = 2)]
        public Guid DatoExtraID { get; set; }

        [Column(Order = 3)]
        public Guid OpcionID { get; set; }

        public int Orden { get; set; }

        [Required]
        [StringLength(500)]
        public string Opcion { get; set; }

        public virtual DatoExtraProyecto DatoExtraProyecto { get; set; }
    }
}
