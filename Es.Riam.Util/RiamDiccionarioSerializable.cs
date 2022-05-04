using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Es.Riam.Util
{
    /// <summary>
    /// Tipos de diccionario
    /// </summary>
    public enum TiposDiccionary
    {
        /// <summary>
        /// Tipos de datos Guid
        /// </summary>
        Guid = 0,

        /// <summary>
        /// Tipos de datos string
        /// </summary>
        String
    }

    /// <summary>
    /// Diccionario serializable
    /// </summary>
    /// <typeparam name="TKey">Tipo de valor de la clave</typeparam>
    /// <typeparam name="TValue">Tipo de los valores asociados a la clave</typeparam>
    [XmlRoot("dictionary")]
    public class RiamDiccionarioSerializable<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
    {
        #region Métodos estáticos

        #region Métodos públicos

        /// <summary>
        /// Serializa un diccionario
        /// </summary>
        /// <param name="pDiccionario">Diccionario</param>
        /// <returns></returns>
        public static string SerializarDictionary(Dictionary<TKey, TValue> pDiccionario)
        {
            string resultado = "";

            if (pDiccionario != null && pDiccionario.Count > 0)
            {
                string separador = "";
                foreach (object key in pDiccionario.Keys)
                {
                    resultado += separador + key + "@" + pDiccionario[(TKey)key];
                    separador = "#";
                }
            }

            return resultado;
        }

        /// <summary>
        /// Deserializa un diccionario
        /// </summary>
        /// <param name="pDiccionario">Diccionario a llenar</param>
        /// <param name="pCadena">Cadena con los valores del diccionario</param>
        /// <param name="pTipoClave">Tipo de datos de la clave</param>
        /// <param name="pTipoValor">Tipo de datos de los valores</param>
        public static void DeserializarDictionary(Dictionary<TKey, TValue> pDiccionario, string pCadena, TiposDiccionary pTipoClave, TiposDiccionary pTipoValor)
        {
            if (!string.IsNullOrEmpty(pCadena))
            {
                char[] separador = { '#' };
                string[] items = pCadena.Split(separador, StringSplitOptions.RemoveEmptyEntries);
                if (items != null && items.Length > 0)
                {
                    foreach (string pareja in items)
                    {
                        if (!string.IsNullOrEmpty(pareja))
                        {
                            char[] separadorInterior = { '@' };
                            string[] claveValor = pareja.Split(separadorInterior, StringSplitOptions.RemoveEmptyEntries);
                            if (claveValor != null && claveValor.Length > 1)
                            {
                                object key, valor;
                                key = ObtenerTipoDesdeString(pTipoClave, claveValor[0]);
                                valor = ObtenerTipoDesdeString(pTipoValor, claveValor[1]);

                                pDiccionario.Add((TKey)key, (TValue)valor);
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Métodos privados

        private static object ObtenerTipoDesdeString(TiposDiccionary pTipoDatos, string pValor)
        {
            switch (pTipoDatos)
            {
                case TiposDiccionary.Guid:
                    return new Guid(pValor);
                default:
                    return pValor;
            }
        }

        #endregion

        #endregion

        #region IXmlSerializable Members

        /// <summary>
        /// Obtiene el esquema de la serialización
        /// </summary>
        /// <returns></returns>
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Lee los valores serializados
        /// </summary>
        /// <param name="reader">Reader</param>
        public void ReadXml(System.Xml.XmlReader reader)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));

            bool wasEmpty = reader.IsEmptyElement;

            reader.Read();

            if (wasEmpty)

                return;


            reader.ReadStartElement("l");//list
            while (reader.NodeType != System.Xml.XmlNodeType.None && reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {

                reader.ReadStartElement("i");//item
                reader.ReadStartElement("k");//key

                TKey key = (TKey)keySerializer.Deserialize(reader);

                reader.ReadEndElement();

                reader.ReadStartElement("v");//value

                TValue value = (TValue)valueSerializer.Deserialize(reader);

                reader.ReadEndElement();

                this.Add(key, value);

                reader.ReadEndElement();

                reader.MoveToContent();
            }
        }

        /// <summary>
        /// Serializa todos los elementos del diccionario
        /// </summary>
        /// <param name="writer">Writer</param>
        public void WriteXml(System.Xml.XmlWriter writer)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));
            writer.WriteStartElement("l");//list
            foreach (TKey key in this.Keys)
            {
                writer.WriteStartElement("i");//item
                writer.WriteStartElement("k");//key

                keySerializer.Serialize(writer, key);

                writer.WriteEndElement();
                writer.WriteStartElement("v");//value

                TValue value = this[key];

                valueSerializer.Serialize(writer, value);

                writer.WriteEndElement();
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        #endregion
    }
}
