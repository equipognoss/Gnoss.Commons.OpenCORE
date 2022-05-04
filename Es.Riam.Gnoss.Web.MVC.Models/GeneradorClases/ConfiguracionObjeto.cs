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
    public class ConfiguracionObjeto
    {
        public string Id { get; set; }
        public string PrefijoPropiedad { get; set; }
        public string NombrePropiedad { get; set; }
        public string SimboloInicio { get; set; }
        public string SimboloFin { get; set; }
        public string Aux { get; set; }
        public string Rango { get; set; }
        public bool EsObject { get; set; }

        public ConfiguracionObjeto(Dictionary<string, string> pDicPref, Propiedad pProp, ElementoOntologia pElem, bool esOntologia, LoggingService loggingService, bool pEsPropiedadTextoInvariable)
        {
            PrefijoPropiedad = UtilCadenas.PrimerCaracterAMayuscula(UtilCadenasOntology.ObtenerPrefijo(pDicPref, pProp.Nombre, loggingService));
            NombrePropiedad = UtilCadenasOntology.ObtenerNombreProp(pProp.Nombre);            
            
            Rango = UtilCadenasOntology.ObtenerNombreProp(pProp.Rango);
            switch (Rango.ToLower())
            {
                case "date":
                case "datetime":
                    if (pProp.ValorUnico)
                    {
                        if (!pProp.FunctionalProperty && pProp.CardinalidadMinima < 1)
                        {
                            Aux = ".Value.ToString(\"yyyyMMddHHmmss\")";
                        }
                        else
                        {
                            Aux = ".ToString(\"yyyyMMddHHmmss\")";
                        }
                    }
                    else
                    {
                        Aux = ".ToString(\"yyyyMMddHHmmss\")";
                    }
                    if (esOntologia)
                    {
                        SimboloInicio = "\\\"{";
                        SimboloFin = "}\\\"";
                    }
                    else
                    {
                        SimboloInicio = "{";
                        SimboloFin = "}";
                    }
                    break;
                case "int":
                    if (pProp.ValorUnico)
                    {
                        if (!pProp.FunctionalProperty)
                        {
                            Aux = ".Value.ToString()";
                        }
                        else
                        {
                            Aux = ".ToString()";
                        }
                    }
                    else
                    {
                        Aux = ".ToString()";
                    }
                    SimboloInicio = "{";
                    SimboloFin = "}";
                    break;
                case "float":
                    if (pProp.ValorUnico)
                    {
                        if (!pProp.FunctionalProperty)
                        {
                            Aux = ".Value.ToString(new CultureInfo(\"en-US\"))";
                        }
                        else
                        {
                            Aux = ".ToString(new CultureInfo(\"en-US\"))";
                        }
                    }
                    else
                    {
                        Aux = ".ToString(new CultureInfo(\"en-US\"))";
                    }
                    SimboloInicio = "{";
                    SimboloFin = "}";
                    break;
                case "boolean":
                    Aux = ".ToString()";
                    if (!esOntologia)
                    {
                        Aux += ".ToLower()";
                    }
                    SimboloInicio = "\\\"{";
                    SimboloFin = "}\\\"";
                    break;
                case "string":
                    //id = "Id";                   
                    //Aux = ".Replace(\"\\r\\n\", \" \").Replace(\"\\n\", \" \").Replace(\"\\r\", \" \").Replace(\"\\\"\", \"\\\\\\\"\")";
                    Aux = ")";
                    if (!esOntologia && !pEsPropiedadTextoInvariable)
                    {
                        Aux += ".ToLower()";
                    }
                    SimboloInicio = "\\\"{GenerarTextoSinSaltoDeLinea(";
                    SimboloFin = "}\\\"";
                    break;
                case "geometry":
                    Aux = ")";
                    SimboloInicio = "\\\"{GenerarTextoSinSaltoDeLinea(";
                    SimboloFin = "}\\\"";
                    if (!esOntologia)
                    {
                        SimboloFin += "^^<http://www.openlinksw.com/schemas/virtrdf#Geometry>";
                    }
                    break;
                default:
                    //caso que se escriben con <>
                    EsObject = true;
                    SimboloInicio = "<{";
                    SimboloFin = "}>";
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
