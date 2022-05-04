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
    [Table("PresentacionPersonalizadoSemantico")]
    public partial class PresentacionPersonalizadoSemantico
    {
        [Column(Order = 0)]
        public Guid OrganizacionID { get; set; }

        [Column(Order = 1)]
        public Guid ProyectoID { get; set; }

        [Column(Order = 2)]
        public Guid OntologiaID { get; set; }

        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short Orden { get; set; }        

        [Required]
        [StringLength(1000)]
        public string Ontologia { get; set; }
        [Required]
        public string ID { get; set; }

        [Required]
        public string Select { get; set; }

        [Required]
        public string Where { get; set; }

        public virtual Proyecto Proyecto { get; set; }
    }
}
