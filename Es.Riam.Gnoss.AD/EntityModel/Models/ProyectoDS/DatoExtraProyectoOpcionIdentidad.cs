using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS
{
    [Serializable]
    [Table("DatoExtraProyectoOpcionIdentidad")]
    public partial class DatoExtraProyectoOpcionIdentidad
    {
        [Column(Order = 0)]
        public Guid OrganizacionID { get; set; }

        [Column(Order = 1)]
        public Guid ProyectoID { get; set; }

        [Column(Order = 2)]
        public Guid DatoExtraID { get; set; }

        [Column(Order = 3)]
        public Guid OpcionID { get; set; }

        [Column(Order = 4)]
        public Guid IdentidadID { get; set; }

        public virtual IdentidadDS.Identidad Identidad { get; set; }
    }
}
