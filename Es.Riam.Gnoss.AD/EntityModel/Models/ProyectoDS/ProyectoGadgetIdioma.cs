using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS
{
    [Serializable]
    [Table("ProyectoGadgetIdioma")]
    public partial class ProyectoGadgetIdioma
    {
        [Column(Order = 0)]
        public Guid GadgetID { get; set; }

        [Column(Order = 1)]
        public Guid OrganizacionID { get; set; }

        [Column(Order = 2)]
        public Guid ProyectoID { get; set; }

        [Column(Order = 3)]
        [StringLength(50)]
        public string Idioma { get; set; }

        [Required]
        public string Contenido { get; set; }

        public virtual ProyectoGadget ProyectoGadget { get; set; }
    }
}
