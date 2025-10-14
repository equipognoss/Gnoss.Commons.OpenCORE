using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Faceta
{
    [Serializable]
    [Table("FacetaObjetoConocimientoProyectoPestanya")]
    public partial class FacetaObjetoConocimientoProyectoPestanya
    {
        [Key]
        [Column(Order = 0)]
        public Guid OrganizacionID { get; set; }

        [Key]
        [Column(Order = 1)]
        public Guid ProyectoID { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(50)]
        public string ObjetoConocimiento { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(300)]
        public string Faceta { get; set; }

        [Key]
        [Column(Order = 4)]
        public Guid PestanyaID { get; set; }
        [Column(Order = 5)]
        public bool AutocompletarEnriquecido {  get; set; }

        public virtual FacetaObjetoConocimientoProyecto FacetaObjetoConocimientoProyecto { get; set; }

        public virtual Proyecto Proyecto { get; set; }

        public virtual ProyectoPestanyaMenu ProyectoPestanyaMenu { get; set; }
    }
}
