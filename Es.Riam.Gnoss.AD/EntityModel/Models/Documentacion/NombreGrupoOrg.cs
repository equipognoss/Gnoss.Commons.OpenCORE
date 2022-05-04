using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion
{
    [Serializable]
    [NotMapped]
    public class NombreGrupoOrg
    {
        public string NombreOrganizacion { get; set; }
        public string NombreGrupo { get; set; }
        public Guid DocumentoID { get; set; }
    }
}
