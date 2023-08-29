using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
    public class EditarObjetoConocimientoYOntologiaModel
    {
        public ObjetoConocimientoModel ObjetoConocimiento { get; set; }

        public EditOntologyViewModel Ontologia { get; set; }
        public bool OntologiaModificada { get; set; }
        public bool ObjetoConocimientoModificado { get; set; }
    }
}
