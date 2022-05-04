using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.CMS
{
    [Serializable]
    [Table("CMSBloqueComponentePropiedadComponente")]
    public partial class CMSBloqueComponentePropiedadComponente
    {
        public Guid OrganizacionID { get; set; }

        public Guid ProyectoID { get; set; }

        [Column(Order = 0)]
        public Guid BloqueID { get; set; }

        [Column(Order = 1)]
        public Guid ComponenteID { get; set; }

        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short TipoPropiedadComponente { get; set; }

        [Required]
        public string ValorPropiedad { get; set; }

        public virtual CMSBloqueComponente CMSBloqueComponente { get; set; }
    }
}
