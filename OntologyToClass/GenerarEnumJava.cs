using Es.Riam.Gnoss.Util.GeneradorClases;
using Es.Riam.Gnoss.Web.MVC.Models.GeneradorClases;
using Es.Riam.Semantica.OWL;
using Es.Riam.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OntologiaAClase
{
    class GenerarEnumJava
    {
        /// <summary>
        /// Posibles tipos de propiedades
        /// </summary>
        public enum PropiedadTipo
        {
            StringSimple,
            StringMultiple,
            Bool,
            DateSimple,
            DateMultiple,
            DateNull,
            IntSimple,
            IntMultiple,
            Image
        }

        public StringBuilder Clase { get; }
        private XmlDocument doc;
        public Dictionary<string, PropiedadTipo> propListiedadesTipo = new Dictionary<string, PropiedadTipo>();
        public Dictionary<string, bool> propListiedadesMultidioma = new Dictionary<string, bool>();
        public List<Propiedad> listentidades = new List<Propiedad>();
        public List<Propiedad> listentidadesAux = new List<Propiedad>();
        public List<string> entListPadres = new List<string>();
        public Dictionary<string, List<Propiedad>> propListPadre = new Dictionary<string, List<Propiedad>>();
        public List<string> listaIdiomas;
        public List<string> nombresOntologia;
        public List<ObjetoPropiedad> listaObjetosPropiedad = new List<ObjetoPropiedad>();
        public readonly Guid proyID;
        public readonly string nombreCortoProy;

        public GenerarEnumJava()
        {
            Clase = new StringBuilder();
        }

        /// <summary>
        /// Constructor de la clase que necesita uan ontologia, su nombre, y el xml , además del string builder
        /// </summary>
        /// <param name="pContentXML">Contenido del xml de la ontología que se esta pintando</param>
        /// <param name="pListaIdiomas">Lista de idiomas disponibles para las propiedades multi idioma</param>
        /// <param name="pNombreCortoProy">Nombre corto del proyecto para el cual se estan pintando las clases</param>
        /// <param name="pProyID">Id del proyecto para el cual se estan pintando las clases</param>
        /// <param name="pNombresOntologia">Nombre de la ontología para la cual se estan pintando las clases</param>
        /// <param name="pListaObjetosPropiedad">Lista de propiedades de la ontología para la cual se estan pintando las clases</param>
        public GenerarEnumJava(byte[] pContentXML, List<string> pListaIdiomas, string pNombreCortoProy, Guid pProyID, List<string> pNombresOntologia, List<ObjetoPropiedad> pListaObjetosPropiedad)
        {
            nombreCortoProy = pNombreCortoProy;
            proyID = pProyID;
            Clase = new StringBuilder();
            listaObjetosPropiedad = pListaObjetosPropiedad;
            listaIdiomas = pListaIdiomas;
            nombresOntologia = pNombresOntologia;
            doc = new XmlDocument();
            if (pContentXML != null)
            {
                MemoryStream ms = new MemoryStream(pContentXML);
                doc.Load(ms);
            }
        }

        /// <summary>
        /// Pinta la enumeración con los idiomas posibles para las propiedades multiidioma
        /// </summary>
        /// <param name="pListaIdiomas">Lista de idimas posibles</param>
        /// <param name="pNombreOnto">Nombre de la ontología para la cual se estan pintando los métodos</param>
        /// <returns>String con el contendio de la clase</returns>
        public string CrearEnum(List<string> pListaIdiomas, string pNombreOnto)
        {
            Clase.AppendLine($"package {UtilCadenas.PrimerCaracterAMinuscula(pNombreOnto)};");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}enum LanguageEnum");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
            foreach (string idiom in pListaIdiomas)
            {
                if (pListaIdiomas.Count == 1)
                {
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{idiom}");
                }
                else
                {
                    if (pListaIdiomas.LastIndexOf(idiom) + 1 == pListaIdiomas.Count)
                    {
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{idiom}");
                    }
                    else
                    {
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{idiom},");
                    }
                }
            }
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
            return Clase.ToString();
        }
    }
}

