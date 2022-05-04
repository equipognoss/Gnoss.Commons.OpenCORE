using Es.Riam.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Es.Riam.Gnoss.Web.RSS.RSS20
{
    public class RSSWriter
    {
        private RSSCanal mCanalRSS;

        /// <summary>
        /// Construcor
        /// </summary>
        /// <param name="pRSSCanal">Canal que se pintara</param>
        public RSSWriter(RSSCanal pRSSCanal)
        {
            mCanalRSS = pRSSCanal;
        }

        /// <summary>
        /// Pinta el RSS
        /// </summary>
        /// <returns>string que representa el RSS </returns>
        public string pintarRSS()
        {
            MemoryStream ms = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(ms, System.Text.Encoding.UTF8);

            pintarInicio(writer);

            pintarCanal(writer);
            
            foreach(RSSItem item in mCanalRSS.Items)
            {
                pintarItem(writer,item);
            }
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.Flush();

            byte[] buffer;
            buffer = ms.ToArray();
            return System.Text.Encoding.UTF8.GetString(buffer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pWriter"></param>
        private void pintarInicio(XmlTextWriter pWriter)
        {
            pWriter.Formatting = Formatting.Indented;
            pWriter.Indentation = 2;
            pWriter.WriteStartDocument();
            pWriter.WriteStartElement("rss");
            pWriter.WriteAttributeString("version", "2.0");
            pWriter.WriteStartElement("channel");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pWriter"></param>
        private void pintarCanal(XmlTextWriter pWriter)
        {
            #region campos Obligatorios de un canal RSS
            if (mCanalRSS.Title != null)
            {
                pWriter.WriteElementString("title", mCanalRSS.Title);
            }
            if (mCanalRSS.Link != null)
            {
                pWriter.WriteElementString("link", mCanalRSS.Link.ToString());
            }
            if (mCanalRSS.Description != null)
            {
                pWriter.WriteElementString("description", UtilCadenas.HtmlDecode(mCanalRSS.Description));
            }
            #endregion

            #region campos optativos de un canal RSS
            if (mCanalRSS.Language != null)
            {
                pWriter.WriteElementString("language", mCanalRSS.Language);
            }
            if (mCanalRSS.Copyright != null)
            {
                pWriter.WriteElementString("copyright", mCanalRSS.Copyright);
            }
            if (mCanalRSS.ManagingEditor != null)
            {
                pWriter.WriteElementString("managingEditor", mCanalRSS.ManagingEditor);
            }
            if (mCanalRSS.WebMaster != null)
            {
                pWriter.WriteElementString("webMaster", mCanalRSS.WebMaster);
            }
            if (mCanalRSS.PubDate != new DateTime())
            {
                pWriter.WriteElementString("pubDate", mCanalRSS.PubDate.ToUniversalTime().ToString("R"));
            }
            if (mCanalRSS.LastBuildDate != new DateTime())
            {
                pWriter.WriteElementString("lastBuildDate", mCanalRSS.PubDate.ToUniversalTime().ToString("R"));
            }
            if (mCanalRSS.Category.Count != 0)
            {
                pintarCategorias(pWriter,mCanalRSS.Category);
            }
            if (mCanalRSS.Generator != null)
            {
                pWriter.WriteElementString("generator", mCanalRSS.Generator);
            }
            if (mCanalRSS.Docs != null)
            {
                pWriter.WriteElementString("docs", mCanalRSS.Docs.ToString());
            }
            if (mCanalRSS.Cloud != null)
            {
                pintarCloud(pWriter);
            }
            if (mCanalRSS.Ttl != -1)
            {
                pWriter.WriteElementString("ttl", mCanalRSS.Ttl.ToString());
            }
            if (mCanalRSS.Image != null)
            {
                pintarImagen(pWriter);
            }
            if (mCanalRSS.Rating != null)
            {
                pWriter.WriteElementString("rating", mCanalRSS.Rating);
            }
            if (mCanalRSS.TextInput != null)
            {
                pintarTextInput(pWriter);
            }
            if (mCanalRSS.SkipHours.Count != 0)
            {
                pintarSkipHours(pWriter);
            }
            if (mCanalRSS.SkipDays.Count != 0)
            {
                pintarSkipDays(pWriter);
            }
            #endregion
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pWriter"></param>
        /// <param name="pItem"></param>
        private void pintarItem(XmlTextWriter pWriter,RSSItem pItem)
        {
            pWriter.WriteStartElement("item");
            if (pItem.Title != null)
            {
                pWriter.WriteElementString("title", pItem.Title);
            }
            if (pItem.Link != null)
            {
                pWriter.WriteElementString("link", pItem.Link.ToString());
            }
            if (pItem.Description != null)
            {
                pWriter.WriteElementString("description", UtilCadenas.HtmlDecode(pItem.Description));
            }
            if (pItem.Author != null)
            {
                pWriter.WriteElementString("author", pItem.Author);
            }
            if (pItem.Category.Count != 0)
            {
                pintarCategorias(pWriter, pItem.Category);
            }
            if (pItem.Comments != null)
            {
                pWriter.WriteElementString("comments", pItem.Comments.ToString());
            }
            if (pItem.Enclosure != null)
            {
                pintarEnclosure(pWriter, pItem);
            }
            if (pItem.Guid != null)
            {
                pintarGuid(pWriter, pItem);
            }
            if (pItem.PubDate != new DateTime())
            {
                pWriter.WriteElementString("pubDate", pItem.PubDate.ToUniversalTime().ToString("R"));
            }
            if (pItem.Source != null)
            {
                pWriter.WriteStartElement("source");
                pWriter.WriteAttributeString("url", pItem.Source.Url.ToString());
                pWriter.WriteString(pItem.Source.Source);
                pWriter.WriteEndElement();
            }
            pWriter.WriteEndElement();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pWriter"></param>
        /// <param name="pItem"></param>
        private void pintarGuid(XmlTextWriter pWriter,RSSItem pItem)
        {
            pWriter.WriteStartElement("guid");
            if (pItem.Guid.IsPermaLink)
            {
                pWriter.WriteAttributeString("isPermaLink", "true");
            }
            pWriter.WriteString(pItem.Guid.Guid);
            pWriter.WriteEndElement();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pWriter"></param>
        /// <param name="pItem"></param>
        private void pintarEnclosure(XmlTextWriter pWriter,RSSItem pItem)
        {
            pWriter.WriteStartElement("enclosure");
            pWriter.WriteAttributeString("url", pItem.Enclosure.Url.ToString());
            pWriter.WriteAttributeString("length", pItem.Enclosure.Length.ToString());
            pWriter.WriteAttributeString("type", pItem.Enclosure.Type);
            pWriter.WriteEndElement();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pWriter"></param>
        private void pintarSkipDays(XmlTextWriter pWriter)
        {
            pWriter.WriteStartElement("skipDays");
            foreach (int dia in mCanalRSS.SkipDays)
            {
                switch (dia)
                {
                    case 1:
                        pWriter.WriteElementString("day", "Monday");
                        break;
                    case 2:
                        pWriter.WriteElementString("day", "Tuesday");
                        break;
                    case 3:
                        pWriter.WriteElementString("day", "Wednesday");
                        break;
                    case 4:
                        pWriter.WriteElementString("day", "Thursday");
                        break;
                    case 5:
                        pWriter.WriteElementString("day", "Friday");
                        break;
                    case 6:
                        pWriter.WriteElementString("day", "Saturday");
                        break;
                    case 7:
                        pWriter.WriteElementString("day", "Sunday");
                        break;
                }

            }
            pWriter.WriteEndElement();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pWriter"></param>
        private void pintarSkipHours(XmlTextWriter pWriter)
        {
            pWriter.WriteStartElement("skipHours");
            foreach (int hora in mCanalRSS.SkipHours)
            {
                pWriter.WriteElementString("hour", hora.ToString());
            }
            pWriter.WriteEndElement();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pWriter"></param>
        private void pintarTextInput(XmlTextWriter pWriter)
        {

            pWriter.WriteStartElement("textInput");
            if (mCanalRSS.TextInput.Title != "")
            {
                pWriter.WriteElementString("title", mCanalRSS.TextInput.Title);
            }
            if (mCanalRSS.TextInput.Name != "")
            {
                pWriter.WriteElementString("name", mCanalRSS.TextInput.Name);
            }
            if (mCanalRSS.TextInput.Link != null)
            {
                pWriter.WriteElementString("link", mCanalRSS.TextInput.Link.ToString());
            }
            if (mCanalRSS.TextInput.Description != "")
            {
                pWriter.WriteElementString("description", mCanalRSS.TextInput.Description);
            }
            pWriter.WriteEndElement();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pWriter"></param>
        private void pintarImagen(XmlTextWriter pWriter)
        {
            pWriter.WriteStartElement("image");
            pWriter.WriteElementString("title", mCanalRSS.Image.Title);
            pWriter.WriteElementString("url", mCanalRSS.Image.Url.ToString());
            pWriter.WriteElementString("link", mCanalRSS.Image.Link.ToString());
            
            if (mCanalRSS.Image.Description != null)
            {
                pWriter.WriteElementString("description", mCanalRSS.Image.Description);
            }
            if (mCanalRSS.Image.Width != -1)
            {
                pWriter.WriteElementString("width", mCanalRSS.Image.Width.ToString());
            }
            if (mCanalRSS.Image.Height != -1)
            {
                pWriter.WriteElementString("height", mCanalRSS.Image.Height.ToString());
            }
            pWriter.WriteEndElement();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pWriter"></param>
        private void pintarCloud(XmlTextWriter pWriter)
        {
            pWriter.WriteStartElement("cloud");
            pWriter.WriteAttributeString("domain", mCanalRSS.Cloud.Domain);
            pWriter.WriteAttributeString("port", mCanalRSS.Cloud.Port.ToString());
            pWriter.WriteAttributeString("path", mCanalRSS.Cloud.Path);
            pWriter.WriteAttributeString("registerProcedure", mCanalRSS.Cloud.RegisterProcedure);
            pWriter.WriteAttributeString("protocol", mCanalRSS.Cloud.Protocol.ToString());
            pWriter.WriteEndElement();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pWriter"></param>
        /// <param name="pCategorias"></param>
        private void pintarCategorias(XmlTextWriter pWriter, List<RSSCategory> pCategorias)
        {
            foreach (RSSCategory categoria in pCategorias)
            {
                pWriter.WriteStartElement("category");
                if (categoria.Domain != null)
                {
                    pWriter.WriteAttributeString("domain", categoria.Domain);
                }
                pWriter.WriteString(categoria.Category);
                pWriter.WriteEndElement();
            }
        }
    }
}
