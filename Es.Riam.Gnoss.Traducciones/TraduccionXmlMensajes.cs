using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Xml;

//hecho

namespace Es.Riam.Gnoss.Traducciones
{
    public class TraduccionXmlMensajes
    {

        public void ExcelMensajesToXml(DataTable tabla, Dictionary<string, MemoryStream> listMStream)
        {
            Dictionary<string, Dictionary<string, Dictionary<string, string>>> diccionario = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();

            foreach (DataRow fila in tabla.Rows)
            {
                string claveFila = fila[0].ToString();

                if (claveFila.Contains("_"))
                {
                    string[] auxLine = claveFila.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
                    string id = auxLine[0].Trim();
                    string asuntoTexto = auxLine[1].Trim();

                    for (int i = 1; i < tabla.Columns.Count; i++)
                    {
                        string idioma = tabla.Columns[i].ColumnName;

                        if (!diccionario.ContainsKey(idioma))
                        {
                            diccionario.Add(idioma, new Dictionary<string, Dictionary<string, string>>());
                        }
                        if (!diccionario[idioma].ContainsKey(id))
                        {
                            diccionario[idioma].Add(id, new Dictionary<string, string>());
                        }
                        diccionario[idioma][id].Add(asuntoTexto, fila[idioma].ToString());
                    }
                }
            }
            CrearXml(diccionario, listMStream);
        }

        private void CrearXml(Dictionary<string, Dictionary<string, Dictionary<string, string>>> diccionario, Dictionary<string, MemoryStream> listMStream)
        {
            Dictionary<string, MemoryStream> listaFicherosIdiomas = new Dictionary<string, MemoryStream>();

            foreach (string idioma in diccionario.Keys)
            {
                string nombreFichero = idioma + "_XmlMensajesCore.xml";
                XmlDocument document = new XmlDocument();
                XmlDeclaration xmldecl = document.CreateXmlDeclaration("1.0", "utf-8", null);
                XmlElement Mensajes = document.CreateElement(string.Empty, "mensajes", string.Empty);

                foreach (string id in diccionario[idioma].Keys)
                {
                    XmlElement MensajesNodo = document.CreateElement(string.Empty, "mensajes", string.Empty);
                    XmlElement MensajeHijo = document.CreateElement(string.Empty, "mensaje", string.Empty);
                    MensajeHijo.SetAttribute("id", id);

                    foreach (KeyValuePair<string, string> asuntoTexto in diccionario[idioma][id])
                    {
                        if (asuntoTexto.Key.Equals("asunto"))
                        {
                            XmlElement asunto = document.CreateElement(string.Empty, "asunto", string.Empty);
                            asunto.InnerText = asuntoTexto.Value;
                            MensajesNodo.AppendChild(asunto);
                        }
                        else
                        {
                            XmlElement texto = document.CreateElement(string.Empty, "texto", string.Empty);
                            texto.InnerText = asuntoTexto.Value;
                            MensajesNodo.AppendChild(texto);
                        }
                    }
                    Mensajes.AppendChild(MensajesNodo);
                }

                document.AppendChild(Mensajes);
                document.InsertBefore(xmldecl, Mensajes);

                MemoryStream xmlStream = new MemoryStream();
                document.Save(xmlStream);
                xmlStream.Flush();
                xmlStream.Position = 0;

                listMStream.Add(nombreFichero, xmlStream);
            }
        }

        public void XmlToExcelMensajes(Dictionary<string, Stream> pListaFicheros, ExcelPackage mExcel)
        {
            string nombreHoja = "MensajesCore";
            Dictionary<string, Dictionary<string, string>> mDiccionario = new Dictionary<string, Dictionary<string, string>>();
            foreach (string idioma in pListaFicheros.Keys)
            {
                List<string> mListaClaves = new List<string>();
                XmlDocument doc = new XmlDocument();
                doc.Load(pListaFicheros[idioma]);

                var listaNodos = doc.SelectNodes("correos/mensajes/mensaje");
                foreach (XmlNode nodoPage in listaNodos)
                {
                    if (nodoPage != null)
                    {
                        string id = nodoPage.Attributes["id"].Value;
                        string asunto = "";
                        string texto = "";
                        string idAsunto = $"{id}_asunto";
                        string idTexto = $"{id}_texto";

                        if (nodoPage.SelectSingleNode("asunto") != null)
                        {
                            if (!string.IsNullOrEmpty(nodoPage.SelectSingleNode("asunto").InnerText))
                            {
                                asunto = nodoPage.SelectSingleNode("asunto").InnerText;
                            }
                        }

                        if (nodoPage.SelectSingleNode("texto") != null)
                        {
                            if (!string.IsNullOrEmpty(nodoPage.SelectSingleNode("texto").InnerText))
                            {
                                texto = nodoPage.SelectSingleNode("texto").InnerText;
                            }                            
                        }

                        if (!mDiccionario.ContainsKey(idAsunto))
                        {
                            mDiccionario.Add(idAsunto, new Dictionary<string, string>());
                            mDiccionario.Add(idTexto, new Dictionary<string, string>());
                        }

                        if (!mDiccionario[idAsunto].ContainsKey(idioma))
                        {
                            mDiccionario[idAsunto].Add(idioma, asunto);
                            mDiccionario[idTexto].Add(idioma, texto);
                        }
                    }
                }
                var nodoFormato = doc.SelectSingleNode("correos/formatoCorreo");
                if (nodoFormato != null)
                {
                    foreach (XmlNode nodosFormatoCorreo in nodoFormato.ChildNodes)
                    {
                        if (!mDiccionario.ContainsKey(nodosFormatoCorreo.Name))
                        {
                            mDiccionario.Add(nodosFormatoCorreo.Name, new Dictionary<string, string>());
                        }
                        if (!mDiccionario[nodosFormatoCorreo.Name].ContainsKey(idioma))
                        {
                            mDiccionario[nodosFormatoCorreo.Name].Add(idioma, nodosFormatoCorreo.InnerText);
                        }
                    }
                }
            }

            UtilFicheros.ConstruirExcel(mExcel, nombreHoja, mDiccionario);
        }
    }
}
