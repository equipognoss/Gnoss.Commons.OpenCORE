using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.Web.MVC.Models.ConfiguracionOCVistas
{
    public class VistasModel
    {
        public string OntologiaID { get; set; }
        public string NombreOC { get; set; }
        public string EntidadPrincipal { get; set; }
        public List<string> IdiomasDisponibles { get; set; }
        public List<EntidadModel> Entidades { get; set; }
        public List<string> ConfiguracionGeneral { get; set; }
    }

    public class EntidadModel
    {
        public string ID { get; set; }

        public List<PropiedadModel> Propiedades { get; set; }

        public List<PropiedadModel> PropiedadesOrdenEntidad { get; set; }

        public List<PropiedadModel> PropiedadesOrdenEntidadLectura { get; set; }

        public List<Representante> Representantes { get; set; }

        public string ClaseCssPanel { get; set; }

        public string ClaseCssTitulo { get; set; }

        public string TagNameTituloEdicion { get; set; }

        public string TagNameTituloLectura { get; set; }

        public List<AtrNombre> AtrNombres { get; set; }

        public List<AtrNombreLectura> AtrNombreLecturas { get; set; }

        public string Microdatos { get; set; }

        public string CampoOrden { get; set; }

        public string CampoRepresentanteOrden { get; set; }
    } 

    public class PropiedadModel
    {
        public string ID { get; set; }

        public string Tipo { get; set; }

        public string Nombre { get; set; }

        public Dictionary<string, string> Atributos { get; set; }
    }
}
