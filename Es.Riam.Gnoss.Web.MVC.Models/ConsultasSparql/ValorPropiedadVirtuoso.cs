using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.Web.MVC.Models.ConsultasSparql
{
    public class ValorPropiedadVirtuoso
    {
        public string Sujetolargo { get; set; }
        public string Propiedad { get; set; }
        public string ValorConIdioma { get; set; }
        public List<TraduccionIdioma> Traducciones { get; set; }
        public ValorPropiedadVirtuoso()
        {
            Traducciones = new List<TraduccionIdioma>();
        }
        
    }

    public class TraduccionIdioma
    {
        public string Texto {  get; set; }
        public string Idioma {  get; set; }
    }
}
