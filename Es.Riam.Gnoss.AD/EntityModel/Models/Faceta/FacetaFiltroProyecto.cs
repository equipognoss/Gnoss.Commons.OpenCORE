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
    [Table("FacetaFiltroProyecto")]
    public partial class FacetaFiltroProyecto
    {
        [Column(Order = 0)]
        public Guid OrganizacionID { get; set; }

        [Column(Order = 1)]
        public Guid ProyectoID { get; set; }

        [Column(Order = 2)]
        [StringLength(50)]
        public string ObjetoConocimiento { get; set; }

        [Column(Order = 3)]
        [StringLength(300)]
        public string Faceta { get; set; }

        [Required]
        public string Filtro { get; set; }

        [Column(Order = 4)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short Orden { get; set; }

        public string Condicion { get; set; }
        public virtual FacetaObjetoConocimientoProyecto FacetaObjetoConocimientoProyecto { get; set; }
        
    }
}
