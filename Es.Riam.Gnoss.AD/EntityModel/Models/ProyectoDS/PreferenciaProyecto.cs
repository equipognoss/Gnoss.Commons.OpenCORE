using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS
{
    [Serializable]
    [Table("PreferenciaProyecto")]
    public partial class PreferenciaProyecto
    {
        [Column(Order = 0)]
        public Guid OrganizacionID { get; set; }

        [Column(Order = 1)]
        public Guid ProyectoID { get; set; }

        [Column(Order = 2)]
        public Guid TesauroID { get; set; }

        [Column(Order = 3)]
        public Guid CategoriaTesauroID { get; set; }

        public int Orden { get; set; }
    }
}
