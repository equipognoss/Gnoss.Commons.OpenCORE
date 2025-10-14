using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Peticion
{
    [Serializable]
    [Table("PeticionNuevoProyecto")]
    public partial class PeticionNuevoProyecto
    {
        [Key]
        public Guid PeticionID { get; set; }

        [Required]
        [StringLength(1000)]
        public string Nombre { get; set; }

        [Required]
        public string NombreCorto { get; set; }

        public string Descripcion { get; set; }

        public short Tipo { get; set; }

        public Guid? ComunidadPrivadaPadreID { get; set; }

        public Guid PerfilCreadorID { get; set; }

        public virtual Peticion Peticion { get; set; }

        [StringLength(50)]
        public string IdiomaDefecto { get; set; }
    }
}
