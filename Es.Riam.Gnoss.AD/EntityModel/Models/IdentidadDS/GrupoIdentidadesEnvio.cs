using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.IdentidadDS
{
    [Serializable]
    public class GrupoIdentidadesEnvio
    {
        [Key]
        public Guid GrupoID { get; set; }

        [Required]
        [StringLength(300)]
        public string Nombre { get; set; }

        [Required]
        [StringLength(300)]
        public string NombreCorto { get; set; }

        [Required]
        public string Descripcion { get; set; }

        public DateTime FechaAlta { get; set; }

        public DateTime? FechaBaja { get; set; }

        [Required]
        public string Tags { get; set; }

        public bool Publico { get; set; }

        public bool PermitirEnviarMensajes { get; set; }
        public string NombreBusqueda { get; set; }
    }
}
