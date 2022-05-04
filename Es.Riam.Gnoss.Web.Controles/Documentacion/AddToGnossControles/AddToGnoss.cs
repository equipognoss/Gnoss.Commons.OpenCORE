using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Web.Controles.Documentacion.AddToGnossControles
{
    /// <summary>
    /// Herramienta para agregar recursos de forma rápida.
    /// </summary>
    public class AddToGnoss
    {
        /// <summary>
        /// Versión actual de la herramienta addToGnoss.
        /// </summary>
        public static string VERSION_ACTUAL = "2.1.1890";
        
        /// <summary>
        /// Indica si la versión pasada como parámetro es la actual o no.
        /// </summary>
        /// <param name="pVersion">Versión a comprobar</param>
        /// <returns>TRUE si pVersion es la actual, FALSE en caso contrario</returns>
        public static bool EsVersionActual(string pVersion)
        {
            char[] separador = { '.' };
            string[] numVersion = pVersion.Split(separador, StringSplitOptions.RemoveEmptyEntries);
            string[] numVersionActual = VERSION_ACTUAL.Split(separador, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < numVersion.Length; i++)
            {
                if (int.Parse(numVersion[i]) < int.Parse(numVersionActual[i]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Lista con una serie de simbolos que son introducidos en vez de caracteres no permitidos en la descripción para la URL.
        /// </summary>
        public static Dictionary<string, string> SimbolosCaracteresnNopermitidos
        {
            get
            {
                Dictionary<string, string> listaSimbolosCaracteres = new Dictionary<string, string>();
                listaSimbolosCaracteres.Add("[-and-]", "&");
                listaSimbolosCaracteres.Add("[-salto-]", "\n");
                listaSimbolosCaracteres.Add("[-tab-]", "\t");
                listaSimbolosCaracteres.Add("[-dilla-]", "#");
                listaSimbolosCaracteres.Add("[-2puntos-]", ":");
                return listaSimbolosCaracteres;
            }
        }
    }
}
