using System.Net;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

namespace Es.Riam.Util
{
    /// <summary>
    /// Utilidades disponibles para su uso desde las vistas.
    /// Expone de forma controlada funcionalidades que residen en namespaces prohibidos por
    /// <see cref="UtilCadenas.ValidarUsings"/>, de manera que las vistas personalizables no
    /// necesiten declarar dichos namespaces.
    /// </summary>
    public static class UtilVistas
    {
        #region Métodos públicos

        /// <summary>
        /// Codifica una cadena para poder utilizarla de forma segura dentro de una URL.
        /// Equivale a System.Net.WebUtility.UrlEncode.
        /// </summary>
        /// <param name="pText">Texto a codificar</param>
        /// <returns>Texto codificado para URL</returns>
        public static string UrlEncode(string pText)
        {
            return WebUtility.UrlEncode(pText);
        }

        /// <summary>
        /// Decodifica una cadena codificada
        /// Equivale a System.Net.WebUtility.UrlDecode.
        /// </summary>
        /// <param name="pText">Texto a decodificar</param>
        /// <returns>Texto decodificado para URL</returns>
        public static string UrlDecode(string pText)
        {
            return WebUtility.UrlDecode(pText);
        }

        /// <summary>
        /// Decodifica una cadena codificada en HTML
        /// Equivale a System.Net.WebUtility.HtmlDecode.
        /// </summary>
        /// <param name="pText">Texto a decodificar</param>
        /// <returns>Texto decodificado de HTML</returns>
        public static string HtmlDecode(string pText)
        {
            return WebUtility.HtmlDecode(pText);
        }

        /// <summary>
        /// Devuelve la extensión de un nombre de archivo o ruta.
        /// Equivale a System.IO.Path.GetExtension.
        /// </summary>
        /// <param name="pText">Nombre de archivo o ruta</param>
        /// <returns>Extensión, incluido el punto</returns>
        public static string GetExtension(string pText)
        {
            return Path.GetExtension(pText);
        }

        /// <summary>
        /// Devuelve el nombre de archivo o ruta sin la extensión.
        /// Equivale a System.IO.Path.GetFileNameWithoutExtension.
        /// </summary>
        /// <param name="pText">Nombre de archivo o ruta</param>
        /// <returns>Nombre sin extensión</returns>
        public static string GetFileNameWithoutExtension(string pText)
        {
            return Path.GetFileNameWithoutExtension(pText);
        }

        /// <summary>
        /// Devuelve los nombres de las propiedades booleanas de un objeto cuyo valor es true.
        /// Permite recorrer los modelos de acciones desde las vistas sin necesidad de usar
        /// System.Reflection dentro de la propia vista.
        /// </summary>
        /// <param name="pObject">Objeto a inspeccionar</param>
        /// <param name="pDeclaringTypeName">Nombre del tipo que debe declarar la propiedad. Si es nulo o vacío no se filtra por tipo</param>
        /// <returns>Lista con los nombres de las propiedades booleanas cuyo valor es true</returns>
        public static List<string> GetActiveBooleanProperties(object pObject, string pDeclaringTypeName)
        {
            List<string> activeProperties = new List<string>();

            if (pObject == null)
            {
                return activeProperties;
            }

            foreach (PropertyInfo property in pObject.GetType().GetProperties())
            {
                if (property.PropertyType != typeof(bool))
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(pDeclaringTypeName) && property.DeclaringType.Name != pDeclaringTypeName)
                {
                    continue;
                }

                if ((bool)property.GetValue(pObject))
                {
                    activeProperties.Add(property.Name);
                }
            }

            return activeProperties;
        }

        #endregion
    }
}
