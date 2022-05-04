using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.PersonaDS
{
    [Serializable]
    [Table("Curriculum")]
    public partial class Curriculum
    {

        public Guid CurriculumID { get; set; }

        [StringLength(250)]
        public string Titulo { get; set; }

        public int TipoVisibilidad { get; set; }

        public bool UsaDatosPersonalesPerfil { get; set; }

        public DateTime? FechaPublicacion { get; set; }

        public bool Publicado { get; set; }

        public string Tags { get; set; }

        public string Description { get; set; }
    }
}
