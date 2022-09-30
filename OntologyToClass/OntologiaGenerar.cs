using Es.Riam.Semantica.OWL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OntologiaAClase
{
    public class OntologiaGenerar
    {
        public OntologiaGenerar(string nombreOnto, byte[] bytesOWL, byte[] bytesXML, bool esprimaria, List<string> listaIdiomas,string directorio)
        {
            this.nombreOnto = nombreOnto;
            this.ontologia = new Ontologia(bytesOWL);
            this.contentXML = bytesXML;
            this.directorio = directorio;
            this.esPrincipal = esprimaria;
            this.listaIdiomas = listaIdiomas;
        }

        public OntologiaGenerar(string nombreOnto, Ontologia ontologia, byte[] bytesXML, bool pEsPrincipal, List<string> listaIdiomas, string directorio)
        {
            this.nombreOnto = nombreOnto;
            this.ontologia = ontologia;
            this.contentXML = bytesXML;
            this.directorio = directorio;
            this.esPrincipal = pEsPrincipal;
            this.listaIdiomas = listaIdiomas;
        }

        public Ontologia ontologia { get; set; }
        public string nombreOnto { get; set; }
        public byte[] contentXML { get; set; }
        public bool esPrincipal { get; set; }
        public string directorio { get; set; }
        public List<string> listaIdiomas { get; set; }
    }
}
