using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml;

namespace Es.Riam.Gnoss.Traducciones
{
    public class TraduccionXmlOntologias
    {
        bool faltaID;
        string[] PALITOS = { "|||" };

        public void XmlOntologiaToExcel(Stream stream, ExcelPackage mExcel, string nombreHoja)
        {
            faltaID = false;
            Dictionary<string, Dictionary<string, string>> mDiccionario = new Dictionary<string, Dictionary<string, string>>();
            List<string> mListaClaves = new List<string>();

            XmlDocument doc = new XmlDocument();
            doc.Load(stream);

            addPropiedades(mDiccionario, doc);
            addEntidades(mDiccionario, doc);

            if (faltaID == false)
            {
                UtilFicheros.ConstruirExcel(mExcel, nombreHoja, mDiccionario);
            }

        }

        private void addPropiedades(Dictionary<string, Dictionary<string, string>> mDiccionario, XmlDocument doc)
        {
            XmlNodeList listaPropiedades = doc.GetElementsByTagName("Propiedad");

            foreach (XmlNode propiedad in listaPropiedades)
            {
                string id = propiedad.Attributes["ID"].Value;
                string entidadId = propiedad.Attributes["EntidadID"].Value;

                foreach (XmlNode nombre in propiedad)
                {
                    if (!string.IsNullOrEmpty(nombre.Name))
                    {
                        string clave = $"Propiedad{PALITOS[0]}{id}{PALITOS[0]}{entidadId}{PALITOS[0]}{nombre.Name}";

                        if (!nombre.InnerText.Equals("") && ((nombre.Name.Equals("AtrNombreLectura") || nombre.Name.Equals("AtrNombre")) && nombre.Attributes.Equals("xml:lang") && !string.IsNullOrEmpty(nombre.Attributes["xml:lang"].Value)))
                        {
                            string idioma = nombre.Attributes["xml:lang"]?.Value;

                            string texto = nombre.InnerText;

                            if (idioma == null)
                            {
                                idioma = "es";
                            }

                            if (!mDiccionario.ContainsKey(clave))
                            {
                                mDiccionario.Add(clave, new Dictionary<string, string>());
                            }

                            if (idioma != null && !mDiccionario[clave].ContainsKey(idioma))
                            {
                                mDiccionario[clave].Add(idioma, texto);
                            }
                        }
                    }
                }
            }
        }

        private void addEntidades(Dictionary<string, Dictionary<string, string>> mDiccionario, XmlDocument doc)
        {
            XmlNodeList listaEntidades = doc.GetElementsByTagName("Entidad");

            foreach (XmlNode entidad in listaEntidades)
            {
                string id = entidad.Attributes["ID"].Value;

                addEntidadesNombre(mDiccionario, entidad, id);
                addAtrNombreGrupoEntidades(mDiccionario, entidad, id);
                addLiterales(mDiccionario, entidad, id);
            }
        }

        private void addEntidadesNombre(Dictionary<string, Dictionary<string, string>> mDiccionario, XmlNode entidad, string id)
        {
            foreach (XmlNode nombre in entidad)
            {
                if (!nombre.InnerText.Equals("") && (nombre.Name.Equals("AtrNombreLectura") || nombre.Name.Equals("AtrNombre")))
                {
                    string clave = $"Entidad{PALITOS[0]}{id}{PALITOS[0]}{nombre.Name}";

                    string idioma = nombre.Attributes["xml:lang"]?.Value;

                    string texto = nombre.InnerText;

                    if (idioma == null)
                    {
                        idioma = "es";
                    }

                    if (!mDiccionario.ContainsKey(clave))
                    {
                        mDiccionario.Add(clave, new Dictionary<string, string>());
                    }

                    if (idioma != null && !mDiccionario[clave].ContainsKey(idioma))
                    {
                        mDiccionario[clave].Add(idioma, texto);
                    }
                }
            }
        }

        private void addAtrNombreGrupoEntidades(Dictionary<string, Dictionary<string, string>> mDiccionario, XmlNode entidad, string id)
        {
            XmlNodeList listaAtrNombreGrupo = entidad.SelectNodes(".//AtrNombreGrupo");

            foreach (XmlNode nombreGrupo in listaAtrNombreGrupo)
            {
                if (nombreGrupo.Attributes["xml:lang"] != null && (!string.IsNullOrEmpty(nombreGrupo.Attributes["xml:lang"].Value) && nombreGrupo.ParentNode.Attributes["ID"] != null && !string.IsNullOrEmpty(nombreGrupo.ParentNode.Attributes["ID"].Value)) && !string.IsNullOrEmpty(nombreGrupo.InnerText))
                {
                    string clave = $"Entidad{PALITOS[0]}{id}{PALITOS[0]}{nombreGrupo.ParentNode.Attributes["ID"].Value}{PALITOS[0]}{nombreGrupo.Name}";
                    string idioma = nombreGrupo.Attributes["xml:lang"].Value;
                    var texto = nombreGrupo.InnerText;

                    if (idioma == null)
                    {
                        idioma = "es";
                    }

                    if (!mDiccionario.ContainsKey(clave))
                    {
                        mDiccionario.Add(clave, new Dictionary<string, string>());
                    }

                    if (idioma != null && !mDiccionario[clave].ContainsKey(idioma))
                    {
                        mDiccionario[clave].Add(idioma, texto);
                    }
                }
                else if (nombreGrupo.ParentNode.Attributes["ID"] == null)
                {
                    faltaID = true;
                    //throw new Exception("Error: falta un atributo ID en la etiqueta Grupo padre de un AtrNombreGrupo");
                }
            }
        }

        private void addLiterales(Dictionary<string, Dictionary<string, string>> mDiccionario, XmlNode entidad, string id)
        {
            XmlNodeList listaLiterales = entidad.SelectNodes(".//Literal");

            foreach (XmlNode literal in listaLiterales)
            {
                if (literal.HasChildNodes && literal.FirstChild.Name.Equals("Literal") && literal.Attributes["ID"]?.Value != null)
                {
                    string clave = $"Entidad{PALITOS[0]}{id}{PALITOS[0]}{literal.Attributes["ID"].Value}{PALITOS[0]}{literal.Name}";

                    foreach (XmlNode literalHijo in literal)
                    {
                        string texto = literalHijo.InnerText;
                        if (literalHijo.Attributes["xml:lang"] != null && !string.IsNullOrEmpty(literalHijo.Attributes["xml:lang"].Value))
                        {
                            string idioma = literalHijo.Attributes["xml:lang"]?.Value;

                            if (idioma == null)
                            {
                                idioma = "es";
                            }

                            if (!mDiccionario.ContainsKey(clave))
                            {
                                mDiccionario.Add(clave, new Dictionary<string, string>());
                            }

                            if (idioma != null && !mDiccionario[clave].ContainsKey(idioma))
                            {
                                mDiccionario[clave].Add(idioma, texto);
                            }
                        }
                    }
                }
                else
                {
                    faltaID = true;
                   // throw new Exception("Error: falta un ID en la etiqueta Literal ");
                }
            }
        }



        public MemoryStream ExcelToXmlPublication(Stream stream)
        {
            MemoryStream ms = new MemoryStream();
            XmlDocument doc = new XmlDocument();
            doc.Load(stream);
            XmlNamespaceManager namespaces = new XmlNamespaceManager(doc.NameTable);
            namespaces.AddNamespace("xml", "http://www.w3.org/XML/1998/namespace");

            DataSet ds = UtilFicheros.LeerExcelDeRutaADataSet(stream);
            DataTable dt = ds.Tables[0];

            XmlNodeList listaPropiedad = doc.GetElementsByTagName("Propiedad");

            foreach (DataRow fila in dt.Rows)
            {
                string claveFila = fila[0].ToString();

                if (claveFila.Contains(PALITOS[0]))
                {
                    string[] auxLine = claveFila.Split(PALITOS, StringSplitOptions.RemoveEmptyEntries);

                    string propiedadEntidad = auxLine[0].Trim();

                    if (propiedadEntidad.Equals("Propiedad"))
                    {
                        cambiarPropiedad(auxLine, dt, fila, doc, namespaces);
                    }
                    else
                    {
                        cambiarEntidad(auxLine, dt, fila, doc, namespaces);
                    }
                }
            }

            doc.Save(ms);
            ms.Flush();
            ms.Position = 0;

            return ms;
        }

        private void cambiarPropiedad(string[] auxLine, DataTable dt, DataRow fila, XmlDocument doc, XmlNamespaceManager namespaces)
        {
            string propiedadID = auxLine[1].Trim();
            string entidadID = auxLine[2].Trim();
            string etiquetaNombre = auxLine[3].Trim();

            for (int i = 1; i < dt.Columns.Count; i++)
            {
                string idioma = dt.Columns[i].ColumnName;
                string valorTexto = fila[idioma] as string;

                if (!string.IsNullOrEmpty(valorTexto))
                {
                    XmlNode propiedad = doc.SelectSingleNode($"//Propiedad[@ID=\"{propiedadID}\" and @EntidadID=\"{entidadID}\"]");

                    meterNodosLang(propiedad, etiquetaNombre, idioma, namespaces, valorTexto, doc);
                }
            }
        }

        private void cambiarEntidad(string[] auxLine, DataTable dt, DataRow fila, XmlDocument doc, XmlNamespaceManager namespaces)
        {
            string entidadID = auxLine[1].Trim();

            XmlNode entidad = doc.SelectSingleNode($"//Entidad[@ID=\"{entidadID}\"]");

            string etiquetaNombre = auxLine.Last().Trim();

            for (int i = 1; i < dt.Columns.Count; i++)
            {
                string idioma = dt.Columns[i].ColumnName;
                string valorTexto = fila[idioma] as string;

                XmlNode padreNodo = entidad;

                if (!string.IsNullOrEmpty(valorTexto))
                {
                    //Si es literal o nombreattrgrupo                
                    if (auxLine.Length > 3)
                    {
                        string id = auxLine[2].Trim();
                        //Si es literal => padreNodo = Literal[@ID]
                        if (etiquetaNombre.Equals("Literal"))
                        {
                            padreNodo = entidad.SelectSingleNode($".//Literal[@ID=\"{id}\"]");

                        }

                        //Si es grupoattrNombre padreNodo = Grupo[@ID]/grupoattrNombre.parent
                        else
                        {
                            if (entidad.SelectSingleNode($".//Grupo[@ID=\"{id}\"]") == null) { continue; }
                            padreNodo = entidad.SelectSingleNode($".//Grupo[@ID=\"{id}\"]/AtrNombreGrupo").ParentNode;
                        }
                    }

                    if (padreNodo == null) { continue; }

                    meterNodosLang(padreNodo, etiquetaNombre, idioma, namespaces, valorTexto, doc);
                }
            }
        }

        private void meterNodosLang(XmlNode padreNodo, string etiquetaNombre, string idioma, XmlNamespaceManager namespaces, string valorTexto, XmlDocument doc)
        {
            if (padreNodo != null)
            {
                XmlNode elemento = padreNodo.SelectSingleNode(etiquetaNombre);
                XmlAttributeCollection atributos = elemento.Attributes;

                if (elemento != null)
                {
                    if (elemento.Attributes["xml:lang"] != null)
                    {
                        elemento = padreNodo.SelectSingleNode($"{etiquetaNombre}[@xml:lang=\"{idioma}\"]", namespaces);

                        if (elemento == null)
                        {
                            elemento = doc.CreateNode(XmlNodeType.Element, etiquetaNombre, null);

                            XmlAttribute atributoLenguaje = doc.CreateAttribute("xml:lang");
                            atributoLenguaje.Value = idioma;
                            elemento.Attributes.Append(atributoLenguaje);

                            foreach (XmlAttribute atributo in atributos)
                            {
                                if (!atributo.Name.Equals("xml:lang"))
                                {
                                    XmlAttribute atributoCopia = doc.CreateAttribute(atributo.Name);
                                    atributoCopia.Value = atributo.Value;
                                    elemento.Attributes.Append(atributoCopia);
                                }
                            }
                        }

                        elemento.InnerText = valorTexto;
                    }
                    else
                    {
                        XmlAttribute atributoLenguaje = doc.CreateAttribute("xml:lang");
                        atributoLenguaje.Value = idioma;
                        elemento.Attributes.Append(atributoLenguaje);
                    }

                    padreNodo.PrependChild(elemento);
                }
            }
        }
    }
}
