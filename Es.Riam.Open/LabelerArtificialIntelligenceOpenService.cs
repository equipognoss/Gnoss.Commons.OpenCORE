using Es.Riam.InterfacesOpen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Open
{
    public class LabelerArtificialIntelligenceOpenService : ILabelerService
    {
        public string ObtenerEtiquetas(string pTitulo, string pDescripcion, out string pEtiquetasPropuestas, string pProyectoID, string pDocumentoID, string pExtension = null)
        {
            pEtiquetasPropuestas = "";
            return "";
        }
    }
}
