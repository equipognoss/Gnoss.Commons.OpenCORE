using Es.Riam.Gnoss.Web.MVC.Models.ConsultasSparql;
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
    /// <summary>
    /// Modelo de consulta de SPARQL que contendrá la consulta (Query Text) junto con los resultados en formato JSON para el pintado de estos en una tabla
    /// </summary>
    public class ConsultaSPARQLViewModel { 
        public string consultaSPARQL { get; set; }
		public Root resultadosSPARQL { get; set; }
        public AdministrarConsultasSPARQLViewModel administrarConsultasSparql { get; set; }

		public ConsultaSPARQLViewModel(string pConsultaSPARQL, Root pResultadosSPARQL, AdministrarConsultasSPARQLViewModel pAdministrarConsultasSparql)
		{
            consultaSPARQL = pConsultaSPARQL;
            resultadosSPARQL = pResultadosSPARQL;
			administrarConsultasSparql = pAdministrarConsultasSparql;

		}
	}
}
