using Es.Riam.Gnoss.Web.MVC.Models.GeneradorClases;
using Es.Riam.Semantica.OWL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OntologiaAClase
{
   public class PropiedadesAMostrar
    {
        public ObjetoPropiedad ObjetoPropiedad { get; set;}

        public string NombreProp { get; set; }

        public List<PropiedadesAMostrar> ListaPropiedadesAMostrar { get; set; }
    }
}
