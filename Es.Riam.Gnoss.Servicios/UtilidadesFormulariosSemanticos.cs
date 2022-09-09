using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Es.Riam.Gnoss.Servicios
{
    public class UtilidadesFormulariosSemanticos
    {
        public static void ObtenerMetaEtiquetasXMLOntologia(byte[] pByteArray, Dictionary<string, List<MetaKeyword>> pDicOntologiaMetas, string pOntologiaEnlace)
        {
            List<MetaKeyword> listaMetaKeywords = ObtenerMetaEtiquetasXML(pByteArray, pOntologiaEnlace);

            if (!pDicOntologiaMetas.ContainsKey(pOntologiaEnlace))
            {
                pDicOntologiaMetas.Add(pOntologiaEnlace, listaMetaKeywords);
            }
            else
            {
                pDicOntologiaMetas[pOntologiaEnlace].AddRange(listaMetaKeywords);
            }
        }

        public static void ObtenerMetaEtiquetasXMLOntologia(byte[] pByteArray, Dictionary<Guid, List<MetaKeyword>> pDicOntologiaMetas, Guid pOntologiaID)
        {
            List<MetaKeyword> listaMetaKeywords = ObtenerMetaEtiquetasXML(pByteArray, pOntologiaID.ToString());

            if (!pDicOntologiaMetas.ContainsKey(pOntologiaID))
            {
                pDicOntologiaMetas.Add(pOntologiaID, listaMetaKeywords);
            }
            else
            {
                pDicOntologiaMetas[pOntologiaID].AddRange(listaMetaKeywords);
            }
        }

        private static List<MetaKeyword> ObtenerMetaEtiquetasXML(byte[] pByteArray, string pOntologiaID)
        {
            XmlDocument docXml = new XmlDocument();
            MemoryStream stream = new MemoryStream(pByteArray);
            docXml.Load(stream);
            XmlNodeList nodosMetas = docXml.SelectNodes("config/ConfiguracionGeneral/MetasPagina/meta");
            List<MetaKeyword> listaMetas = new List<MetaKeyword>();

            if (nodosMetas != null && nodosMetas.Count > 0)
            {
                foreach (XmlNode nodoMeta in nodosMetas)
                {
                    if (nodoMeta.Attributes["name"] != null && nodoMeta.Attributes["name"].Value.Equals("keywords"))
                    {
                        MetaKeyword metaKeyword = new MetaKeyword();
                        if (nodoMeta.Attributes["content"] != null)
                        {
                            metaKeyword.Content = nodoMeta.Attributes["content"].Value;
                        }
                        if (nodoMeta.Attributes["EntidadID"] != null)
                        {
                            metaKeyword.EntidadID = nodoMeta.Attributes["EntidadID"].Value;
                        }
                        listaMetas.Add(metaKeyword);
                    }
                }
            }

            return listaMetas;
        }
    }

    public class MetaKeyword
    {
        public string Content { get; set; }
        public string EntidadID { get; set; }
    }

    public class MetaKeywordsOntologia
    {
        public string OntologiaEnlace { get; set; }
        public List<MetaKeyword> MetaKeyWords { get; set; }
    }
}
