using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.IdentidadDS
{
    [Serializable]
    [Table("DatoExtraEcosistemaVirtuosoPerfil")]
    public class DatoExtraEcosistemaVirtuosoPerfil
    {
        [Column(Order = 0)]
        public Guid DatoExtraID { get; set; }

        [Column(Order = 1)]
        public Guid PerfilID { get; set; }

        [Required]
        [StringLength(500)]
        public string Opcion { get; set; }
    }
}
