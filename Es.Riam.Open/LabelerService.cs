using Es.Riam.InterfacesOpen;
using Gnoss.Web.LabelerService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Open
{
    public class LabelerService : ILabelerService
    {
        public string ObtenerEtiquetas(string pTitulo, string pDescripcion, out string pEtiquetasPropuestas, string pProyectoID, string pDocumentoID, string pExtension = null)
        {
            string resultado = "";
            UtilLabelerService utilLabelerService = new UtilLabelerService();
            resultado = utilLabelerService.ObtenerEtiquetasDeTituloYDescripcion(pTitulo, pDescripcion, out pEtiquetasPropuestas);
            return resultado;
        }
    }
}
