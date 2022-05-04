using Es.Riam.Gnoss.Util.GeneradorClases;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Semantica.OWL;
using Es.Riam.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.Web.MVC.Models.GeneradorClases
{
    public class ConfiguracionObjetoJava
    {
        public string Id { get; set; }
        public string PrefijoPropiedad { get; set; }
        public string NombrePropiedad { get; set; }
        public string SimboloInicio { get; set; }
        public string SimboloFin { get; set; }
        public string Aux { get; set; }
        public string Rango { get; set; }
        public bool EsObject { get; set; }
        public bool EsPrimitivo { get; set; }

        public ConfiguracionObjetoJava(Dictionary<string, string> pDicPref, Propiedad pProp, ElementoOntologia pElem, bool esOntologia, bool pEsPropiedadTextoInvariable, LoggingService loggingService)
        {
            PrefijoPropiedad = UtilCadenas.PrimerCaracterAMayuscula(UtilCadenasOntology.ObtenerPrefijo(pDicPref, pProp.Nombre, loggingService));
            NombrePropiedad = UtilCadenasOntology.ObtenerNombreProp(pProp.Nombre);

            Rango = UtilCadenasOntology.ObtenerNombreProp(pProp.Rango);
            switch (Rango.ToLower())
            {
                case "date":
                case "datetime":
                    if (esOntologia)
                    {
                        SimboloInicio = "\\\"";
                        SimboloFin = "\\\"";
                    }
                    break;
                case "int":
                    if (esOntologia)
                    {
                        SimboloFin = "\\\"";
                        SimboloInicio = "\\\"";
                    }
                    SimboloInicio = "";
                    SimboloFin = "";
                    EsPrimitivo = true;
                    break;
                case "float":
                    if (esOntologia)
                    {
                        SimboloInicio = "\\\"";
                        SimboloFin = "\\\"";
                    }
                    else
                    {
                        SimboloInicio = "";
                        SimboloFin = "";
                    }
                    EsPrimitivo = true;
                    break;
                case "boolean":
                    if (!esOntologia)
                    {
                        Aux += ".toLowerCase()";
                    }
                    SimboloInicio = "\\\"";
                    SimboloFin = "\\\"";
                    EsPrimitivo = true;
                    break;
                case "string":
                    Aux = ".replace(\"\\r\\n\", \" \").replace(\"\\n\", \" \").replace(\"\\r\", \" \").replace(\"\\\\\", \"\\\\\\\\\").replace(\"\\\"\", \"\\\\\\\"\")";
                    if (!esOntologia && !pEsPropiedadTextoInvariable)
                    {
                        Aux += ".toLowerCase()";
                    }
                    SimboloInicio = "\\\"";
                    SimboloFin = "\\\"";
                    break;
                default:
                    EsObject = true;
                    SimboloInicio = "<";
                    SimboloFin = ">";
                    Aux = "";
                    break;
            }
            if (EsObject && !pElem.Ontologia.EntidadesAuxiliares.Any(x => x.TipoEntidad.Equals(pProp.Rango)))
            {
                if (pProp.ValorUnico)
                {
                    Id = "Id";
                }
                else
                {
                    Id = "Ids";
                }
            }
        }
    }
}
