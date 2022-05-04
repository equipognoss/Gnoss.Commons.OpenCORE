using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion
{
    [NotMapped]
    [Serializable]
    public class NombreGrupo
    {
        public string NombreGrupoAtributo { get; set; }
        public Guid DocumentoID { get; set; }
    }
}
