using Es.Riam.Gnoss.Util.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.Util.GeneradorClases
{
    public class UtilCadenasOntology
    {
        public static string ObtenerNombreProp(string prop)
        {
            string nombreProp = prop;

            if (prop.Contains('#'))
            {
                nombreProp = prop.Split('#')[1];
            }
            else
            {
                nombreProp = prop.Substring(prop.LastIndexOf('/') + 1);
            }
            nombreProp = nombreProp.Replace("-", "_");

            nombreProp = nombreProp.Replace(".", "_");
            nombreProp = nombreProp.Replace(" ", "_");
            return nombreProp;
        }

        public static string ObtenerNombrePropSinNamespace(string prop)
        {
            string nombreProp = prop;

            if (prop.Contains('#'))
            {
                nombreProp = prop.Split('#')[1];
            }
            else
            {
                nombreProp = prop.Substring(prop.LastIndexOf('/') + 1);
            }
            return nombreProp;
        }

        public static string ObtenerNombrePropSinNamespaceJava(string prop)
        {
            string nombreProp = prop;

            if (prop.Contains('#'))
            {
                nombreProp = prop.Split('#')[1];
            }
            else
            {
                nombreProp = prop.Substring(prop.LastIndexOf('/') + 1);
            }
            return nombreProp;
        }

        public static string ObtenerNombrePropJava(string prop)
        {
            string nombreProp = prop;

            if (prop.Contains('#'))
            {
                nombreProp = prop.Split('#')[1];
            }
            else
            {
                nombreProp = prop.Substring(prop.LastIndexOf('/') + 1);
            }
            nombreProp = nombreProp.Replace("-", "_");

            nombreProp = nombreProp.Replace(".", "_");
            nombreProp = nombreProp.Replace(" ", "_");
            if (nombreProp.Contains("string")) {
                nombreProp = nombreProp.Replace("string", "String");
            }
            return nombreProp;
        }

        public static string ObtenerPrefijo(Dictionary<string, string> dicPref, string rang, LoggingService loggingService)
        {
            if (rang.Contains('#'))
            {
                try
                {
                    return dicPref[rang.Split('#')[0] + "#"];
                }
                catch (Exception ex)
                {
                    loggingService.GuardarLogError(ex, $"error en la propiedad {rang.Split('#')[0] + "#"}");
                    throw new Exception(ex.Message);
                }
            }
            else if(rang.Contains("/"))
            {
                return dicPref[rang.Substring(0, rang.LastIndexOf('/') + 1)];
            }
            else
            {
                return dicPref[rang];
            }
        }

        public static string Tabs(int n)
        {
            return new String('\t', n);
        }
    }
}
