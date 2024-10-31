using ClosedXML.Excel;
using Es.Riam.Gnoss.Util.Configuracion;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Xml;


//hecho

namespace Es.Riam.Gnoss.Traducciones
{
    public class TraduccionXmlCore
    {

        public void ExcelCoreToXml(DataTable tabla, Dictionary<string, MemoryStream> listMStream)
        {
            Dictionary<string, Dictionary<string, Dictionary<string, string>>> diccionario = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();

            foreach (DataRow fila in tabla.Rows)
            {
                string claveFila = fila[0].ToString();

                if (claveFila.Contains("/"))
                {
                    string[] auxLine = claveFila.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                    string page = auxLine[0].Trim();
                    string tag = auxLine[1].Trim();

                    for (int i = 1; i < tabla.Columns.Count; i++)
                    {
                        string idioma = tabla.Columns[i].ColumnName;

                        if (!diccionario.ContainsKey(idioma))
                        {
                            diccionario.Add(idioma, new Dictionary<string, Dictionary<string, string>>());
                        }
                        if (!diccionario[idioma].ContainsKey(page))
                        {
                            diccionario[idioma].Add(page, new Dictionary<string, string>());
                        }
                        diccionario[idioma][page].Add(tag, fila[idioma].ToString());
                    }
                }
            }
            CrearXml(diccionario, listMStream);
        }

        private void CrearXml(Dictionary<string, Dictionary<string, Dictionary<string, string>>> diccionario, Dictionary<string, MemoryStream> listMStream)
        {
            XmlDocument document = new XmlDocument();

            foreach (string idioma in diccionario.Keys)
            {
                string nombreArchivo =idioma + "_XmlCore.xml";
                MemoryStream ms = new MemoryStream();
                XmlDeclaration xmldecl = document.CreateXmlDeclaration("1.0", "utf-8", null);
                XmlElement ResourcesLang = document.CreateElement(string.Empty, "Resources", string.Empty);

                foreach (string page in diccionario[idioma].Keys)
                {
                    XmlElement PageNodo = document.CreateElement(string.Empty, "page", string.Empty);
                    PageNodo.SetAttribute("name", page);

                    foreach (KeyValuePair<string, string> tag in diccionario[idioma][page])
                    {
                        XmlElement ResourceHijo = document.CreateElement(string.Empty, "Resource", string.Empty);
                        ResourceHijo.SetAttribute("tag", tag.Key);
                        ResourceHijo.InnerText = tag.Value;
                        PageNodo.AppendChild(ResourceHijo);
                    }

                    ResourcesLang.AppendChild(PageNodo);
                }

                document.AppendChild(ResourcesLang);
                document.InsertBefore(xmldecl, ResourcesLang);
                document.Save(ms);
                document.RemoveAll();

                listMStream.Add(nombreArchivo, ms);
                ms.Flush();
                ms.Position = 0;
            }
        }

        public void XmlToExcelCore(Dictionary<string, Stream> pFicheros, XLWorkbook mExcel, ConfigService configService)
        {
            string nombreHoja = "Core";
            Dictionary<string, Dictionary<string, string>> mDiccionario = new Dictionary<string, Dictionary<string, string>>();
            foreach (string idioma in pFicheros.Keys)
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(pFicheros[idioma]);

                var listaNodos = doc.SelectNodes("Resources/page");
                foreach (XmlNode nodoPage in listaNodos)
                {
                    string pagina = nodoPage.Attributes["name"].Value;
                    foreach (XmlNode nodoResource in nodoPage.ChildNodes)
                    {
                        string tag = nodoResource.Attributes["tag"].Value;
                        string valor = nodoResource.InnerText;
                        string clave = $"{pagina}/{tag}";
                        if (!mDiccionario.ContainsKey(clave))
                        {
                            mDiccionario.Add(clave, new Dictionary<string, string>());
                        }

                        if (!mDiccionario[clave].ContainsKey(idioma))
                        {
                            mDiccionario[clave].Add(idioma, valor);
                        }
                    }
                }
            }

            UtilFicheros.ConstruirExcel(mExcel, nombreHoja, mDiccionario, configService);
        }
    }
}