using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion
{
    [Serializable]
    [Table("DocumentoEnvioNewsLetter")]
    public partial class DocumentoEnvioNewsLetter
    {
        [Column(Order = 0)]
        public Guid DocumentoID { get; set; }

        [Column(Order = 1)]
        public Guid IdentidadID { get; set; }

        [Column(Order = 2)]
        public DateTime Fecha { get; set; }

        [Required]
        [StringLength(20)]
        public string Idioma { get; set; }

        public bool EnvioSolicitado { get; set; }

        public bool EnvioRealizado { get; set; }

        public string Grupos { get; set; }
    }
}
