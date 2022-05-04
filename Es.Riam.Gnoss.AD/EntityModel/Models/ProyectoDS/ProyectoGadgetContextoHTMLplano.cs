using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS
{
    [Serializable]
    [Table("ProyectoGadgetContextoHTMLplano")]
    public partial class ProyectoGadgetContextoHTMLplano
    {
        [Column(Order = 0)]
        public Guid GadgetID { get; set; }

        [Column(Order = 1)]
        public Guid OrganizacionID { get; set; }

        [Column(Order = 2)]
        public Guid ProyectoID { get; set; }

        [Required]
        public string ComunidadDestinoFiltros { get; set; }

        public virtual ProyectoGadget ProyectoGadget { get; set; }
    }
}
