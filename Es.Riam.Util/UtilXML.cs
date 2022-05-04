using System;
using System.Xml;

namespace Es.Riam.Util
{
    /// <summary>
	/// Utiles para XML
	/// </summary>
	public abstract class UtilXML
    {
        #region Métodos

        /// <summary>
        /// Lee un valor de un elemento XML
        /// </summary>
        /// <param name="pFichero">Fichero</param>
        /// <param name="pNodo">Nodo a leer</param>
        /// <param name="pCampo">Campo del nodo a leer</param>
        /// <returns>Valor</returns>
        public static string LeerValorXml(string pFichero, string pNodo, string pCampo)
        {
            string resultado = "";
            XmlDocument docXml = new XmlDocument();

            docXml.Load(pFichero);
            XmlNode nodo = docXml.SelectSingleNode(pNodo);

            if (nodo != null)
            {
                resultado = nodo[pCampo].InnerText;
            }
            else
            {
                throw new Exception("El nodo especificado no existe: " + pNodo);
            }
            return resultado;
        }


        /// <summary>
        /// Obtien el valor de un determinado nodo
        /// </summary>
        /// <param name="pDocXml">Documento xml</param>
        /// <param name="pNodo">Nodo del árbol XML</param>
        /// <returns>url</returns>
        public static string ObtenerNodo(XmlDocument pDocXml, string pNodo)
        {
            string resultado = "";
            XmlNode nodo = pDocXml.SelectSingleNode(pNodo);

            if (nodo != null)
            {
                resultado = nodo.InnerText;
            }
            else
            {
                throw new Exception("El nodo especificado no existe: " + pNodo);
            }
            return resultado;
        }

        /// <summary>
        /// Lee un atributo de un elemento XML
        /// </summary>
        /// <param name="pFichero">Fichero</param>
        /// <param name="pNodo">Nodo a leer</param>
        /// <param name="pAtributo">Atributo del nodo a leer</param>
        /// <returns>Valor del atributo</returns>
        public static string LeerAtributoCampoXml(string pFichero, string pNodo, string pAtributo)
        {
            string resultado = "";
            XmlDocument docXml = new XmlDocument();

            docXml.Load(pFichero);

            XmlNode nodo = docXml.SelectSingleNode(pNodo);

            if (nodo != null)
            {
                resultado = nodo.Attributes[pAtributo].InnerText;
            }
            else
            {
                throw new Exception("El nodo especificado no existe: " + pNodo);
            }
            return resultado;
        }

        /// <summary>
        /// Guarda un valor en un XML
        /// </summary>
        /// <param name="pFichero">Fichero</param>
        /// <param name="pNodo">Nodo donde guardar</param>
        /// <param name="pCampo">Campo del nodo donde guardar</param>
        /// <param name="pValor">Valor a guardar</param>
        /// <returns>Valor</returns>
        public static void GuardarValorXml(string pFichero, string pNodo, string pCampo, string pValor)
        {
            XmlDocument docXml = new XmlDocument();
            docXml.Load(pFichero);

            XmlNode nodo = docXml.SelectSingleNode(pNodo);

            if (nodo != null)
            {
                nodo[pCampo].InnerText = pValor;
                docXml.Save(pFichero);
            }
            else
            {
                throw new Exception("El nodo especificado no existe");
            }
        }

        /// <summary>
        /// Guarda un valor en un XML
        /// </summary>
        /// <param name="pFichero">Fichero</param>
        /// <param name="pNodo">Nodo donde guardar</param>
        /// <param name="pValor">Valor del nodo donde guardar</param>
        /// <param name="pCrearNodos">Si crea los nodos si no existen</param>
        /// <returns>Valor</returns>
        public static void GuardarValorXml(string pFichero, string pNodo, string pValor, bool pCrearNodos)
        {
            XmlDocument docXml = new XmlDocument();
            docXml.Load(pFichero);
            XmlNode nodo = docXml.SelectSingleNode(pNodo);

            if (nodo != null)
            {
                nodo.InnerText = pValor;
                docXml.Save(pFichero);
            }
            else
            {
                throw new Exception("El nodo especificado no existe");
            }
        }

        /// <summary>
        /// Lee el valor de un nodo en un fichero XML
        /// </summary>
        /// <param name="pFichero">Fichero XML</param>
        /// <param name="pNodo">Nodo del que leer</param>
        /// <param name="pIndiceNodo">Índice del nodo en el documento XML</param>
        /// <returns>Valor</returns>
        public static string LeerValorXmlNodoPosicion(string pFichero, string pNodo, int pIndiceNodo)
        {
            XmlDocument docXml = new XmlDocument();
            docXml.Load(pFichero);
            XmlNodeList listaNodos = docXml.GetElementsByTagName(pNodo);

            if (listaNodos.Count > pIndiceNodo)
            {
                return listaNodos[pIndiceNodo].InnerText;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Lee los nodos en los que podemos recibir un valor de tipo string o short
        /// </summary>
        /// <param name="nodo">Nodo del XML</param>
        /// <param name="nom">Nombre del nodo</param>
        /// <param name="pTipo">Tipo que se espera recibir</param>
        /// <returns>Object que contendrá una string o un short</returns>
        public static object LeerNodo(XmlNode nodo, string nom, Type pTipo)
        {
            Object salida = null;
            if (nodo != null)
            {
                if (nodo.SelectSingleNode(nom) != null)
                {
                    if (pTipo.Equals(typeof(short)))
                    {
                        short enteroCorto = 0;
                        short.TryParse(nodo.SelectSingleNode(nom).InnerText, out enteroCorto);
                        salida = enteroCorto;
                    }
                    if (pTipo.Equals(typeof(string)))
                    {
                        salida = nodo.SelectSingleNode(nom).InnerText;
                    }
                }
                else if (pTipo.Equals(typeof(short)))
                {
                    //es nulo
                    salida = (short)-1;
                }
                else
                {
                    salida = string.Empty;
                }
            }
            else if (pTipo.Equals(typeof(short)))
            {
                //es nulo
                salida = (short)-1;
            }
            else
            {
                salida = string.Empty;
            }
            return salida;
        }

        #endregion
    }
}
