using Es.Riam.Gnoss.AD.EntityModel.Models.Faceta;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Models.GeneradorClases;
using Es.Riam.InterfacesOpen;
using Es.Riam.Semantica.OWL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Open
{
    public class MassiveOntologyToClassOpen : IMassiveOntologyToClass
    {

        public MassiveOntologyToClassOpen(LoggingService loggingService) : base(loggingService)
        {
        }

        public override void CrearToAcidData(bool esPrimaria, ElementoOntologia pEntidad, Ontologia ontologia, StringBuilder Clase, string nombrePropDescripcion, string nombrePropTitulo, string nombrePropTituloEntero, Dictionary<string, bool> propListiedadesMultidioma, List<ObjetoPropiedad> listaObjetosPropiedad, Dictionary<Propiedad, bool> dicPropiedadMultiidiomaFalse)
        {
            throw new NotImplementedException();
        }

        public override void CrearToOntologyGraphTriples(bool esPrimaria, ElementoOntologia pEntidad, StringBuilder Clase, List<Propiedad> listentidadesAux, Ontologia ontologia, Dictionary<string, string> dicPref, Dictionary<string, bool> propListiedadesMultidioma, List<ObjetoPropiedad> listaObjetosPropiedad)
        {
            throw new NotImplementedException();
        }

        public override void CrearToSearchGraphTriples(bool esPrimaria, ElementoOntologia pEntidad, List<string> pListaPropiedadesSearch, List<string> pListaPadrePropiedadesAnidadas, StringBuilder Clase, Ontologia ontologia, string nombrePropDescripcion, string nombrePropTitulo, string nombrePropTituloEntero, Dictionary<string, bool> propListiedadesMultidioma, List<ObjetoPropiedad> listaObjetosPropiedad, List<Propiedad> listentidadesAux, Dictionary<string, string> dicPref, Dictionary<Propiedad, bool> dicPropMultiidiomaFalse)
        {
            throw new NotImplementedException();
        }
    }
}
