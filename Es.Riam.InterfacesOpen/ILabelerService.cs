using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.InterfacesOpen
{
    public interface ILabelerService
    {
        public string ObtenerEtiquetas(string pTitulo, string pDescripcion, out string pEtiquetasPropuestas, string pProyectoID, string pDocumentoID, string pExtension = null);
    }
}
