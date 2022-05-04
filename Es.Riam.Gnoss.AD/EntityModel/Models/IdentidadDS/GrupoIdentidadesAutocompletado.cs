using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.IdentidadDS
{
    [Serializable]
    [NotMapped]
    public class GrupoIdentidadesAutocompletado : GrupoIdentidades
    {
        public GrupoIdentidadesAutocompletado():base()
        {
            
        }
        public string NombreBusqueda { get; set; }

    }
}
