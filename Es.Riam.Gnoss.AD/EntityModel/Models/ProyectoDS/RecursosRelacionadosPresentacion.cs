using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS
{
    [Serializable]
    [Table("RecursosRelacionadosPresentacion")]
    public partial class RecursosRelacionadosPresentacion
    {
        [Column(Order = 0)]
        public Guid OrganizacionID { get; set; }

        [Column(Order = 1)]
        public Guid ProyectoID { get; set; }

        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short Orden { get; set; }

        [Column(Order = 3)]
        public Guid OntologiaID { get; set; }

        [Required]
        [StringLength(1000)]
        public string Ontologia { get; set; }

        [Required]
        [StringLength(2000)]
        public string Propiedad { get; set; }

        [StringLength(1000)]
        public string Nombre { get; set; }

        public short Imagen { get; set; }
    }
}
