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
    [Table("ConfiguracionConexionGrafo")]
    public partial class ConfiguracionConexionGrafo
    {
        [Key]
        [StringLength(36)]
        public string Grafo { get; set; }

        [Required]
        [StringLength(200)]
        public string CadenaConexion { get; set; }
    }
}
