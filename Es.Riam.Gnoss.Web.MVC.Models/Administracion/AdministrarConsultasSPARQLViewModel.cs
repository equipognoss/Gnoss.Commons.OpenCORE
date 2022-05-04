using System.Collections.Generic;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
    public class AdministrarConsultasSPARQLViewModel
    {
        public List<string> listaNomOntologias { get; }
        public Dictionary<string, string> nomPrefijos { get; }

        public AdministrarConsultasSPARQLViewModel(List<string> pLisNomOntologias, Dictionary<string, string> pNomPrefijos)
        {
            listaNomOntologias = pLisNomOntologias;
            nomPrefijos = pNomPrefijos;
        }

    }
}
