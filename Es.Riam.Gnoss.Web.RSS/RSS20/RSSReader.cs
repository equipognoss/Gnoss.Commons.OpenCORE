using System;
using System.Collections;
using System.IO;
using System.Xml;

namespace Es.Riam.Gnoss.Web.RSS.RSS20
{
    public class RSSReader
    {
        private XmlTextReader reader = null;
        private RSSCanal canalRSS = null;

        #region Constructores

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        public RSSReader(string url)
        {
            try
            {
                reader = new XmlTextReader(url);
                reader.WhitespaceHandling = WhitespaceHandling.None;
            }
            catch (Exception e)
            {
                throw new ArgumentException("No se encuentra.", e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="textReader"></param>
        public RSSReader(TextReader textReader)
        {
            try
            {
                reader = new XmlTextReader(textReader);
                reader.WhitespaceHandling = WhitespaceHandling.None;
            }
            catch (Exception e)
            {
                throw new ArgumentException("No se encuentra.", e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        public RSSReader(Stream stream)
        {
            try
            {
                reader = new XmlTextReader(stream);
                reader.WhitespaceHandling = WhitespaceHandling.None;
            }
            catch (Exception e)
            {
                throw new ArgumentException("No se encuentra.", e);
            }
        }

        #endregion


        public void Read()
        {
            //Mientras que no se acabe el fichero no deja de leer 

            //elementos con hijos
            RSSItem item = null;
            RSSCategory category = null;
            Stack xmlNodeStack = new Stack();


            string elementText = "";

            while (reader.Read())
            {
                string readerName = reader.Name.ToLower();
                switch (reader.NodeType)
                {
                    #region Element
                    //Primero cogemos los elemenos con atributos o subelementos
                    case XmlNodeType.Element:
                        {
                            if (reader.IsEmptyElement)
                                break;
                            switch (readerName)
                            {
                                case "channel":
                                    canalRSS = new RSSCanal();
                                    break;

                                case "item":
                                    item = new RSSItem();
                                    canalRSS.Items.Add(item);
                                    break;

                                case "source":
                                    item.Source = new RSSItemSource();
                                    for (int i = 0; i < reader.AttributeCount; i++)
                                    {
                                        reader.MoveToAttribute(i);
                                        switch (reader.Name.ToLower())
                                        {
                                            case "url":
                                                try
                                                {
                                                    item.Source.Url = new Uri(reader.Value);
                                                }
                                                catch (Exception)
                                                {
                                                    throw new Exception("INvalidaurl");
                                                }
                                                break;
                                        }
                                    }
                                    break;
                                case "enclosure":
                                    item.Enclosure = new RSSItemEnclosure();
                                    for (int i = 0; i < reader.AttributeCount; i++)
                                    {
                                        reader.MoveToAttribute(i);
                                        switch (reader.Name.ToLower())
                                        {
                                            case "url":
                                                try
                                                {
                                                    item.Enclosure.Url = new Uri(reader.Value);
                                                }
                                                catch (Exception)
                                                {
                                                    throw new Exception("fallo URI");
                                                }
                                                break;
                                            case "length":
                                                try
                                                {
                                                    item.Enclosure.Length = int.Parse(reader.Value);
                                                }
                                                catch (Exception)
                                                {
                                                    throw new Exception("fallo URI");
                                                }
                                                break;
                                            case "type":
                                                item.Enclosure.Type = reader.Value;
                                                break;
                                        }
                                    }
                                    break;
                                case "guid":
                                    item.Guid = new RSSItemGuid();
                                    for (int i = 0; i < reader.AttributeCount; i++)
                                    {
                                        reader.MoveToAttribute(i);
                                        switch (reader.Name.ToLower())
                                        {
                                            case "ispermalink":
                                                try
                                                {
                                                    item.Guid.IsPermaLink = bool.Parse(reader.Value);
                                                }
                                                catch (Exception)
                                                {
                                                    throw new Exception("fallo URI");
                                                }
                                                break;
                                        }
                                    }
                                    break;

                                case "category":
                                    category = new RSSCategory();
                                    if ((string)xmlNodeStack.Peek() == "channel")
                                        canalRSS.Category.Add(category);
                                    else
                                        item.Category.Add(category);

                                    for (int i = 0; i < reader.AttributeCount; i++)
                                    {
                                        reader.MoveToAttribute(i);
                                        switch (reader.Name.ToLower())
                                        {
                                            case "domain":
                                                category.Domain = reader.Value;
                                                break;
                                        }
                                    }
                                    break;

                                case "image":
                                    canalRSS.Image = new RSSCanalImage();
                                    break;

                                case "textinput":
                                    canalRSS.TextInput = new RSSCanalTextInput();
                                    break;
                            }
                        }
                        xmlNodeStack.Push(readerName);
                        break;
                    #endregion

                    #region endelement
                    case XmlNodeType.EndElement:
                        {
                            if (xmlNodeStack.Count == 1)
                                break;
                            string childElementName = (string)xmlNodeStack.Pop();
                            string parentElementName = (string)xmlNodeStack.Peek();
                            switch (childElementName) // current element
                            {
                                case "channel":
                                    break;
                                case "item":
                                    break;
                                case "source":
                                    item.Source.Source = elementText;
                                    break;
                                case "enclosure":
                                    break;
                                case "guid":
                                    item.Guid.Guid = elementText;
                                    break;
                                case "category":
                                    category.Category = elementText;
                                    break;
                                case "textinput":
                                    break;
                                case "image":
                                    break;
                                case "cloud":
                                    break;
                            }
                            switch (parentElementName) // parent element
                            {
                                case "item":
                                    switch (childElementName)
                                    {
                                        case "title":
                                            item.Title = elementText;
                                            break;
                                        case "link":
                                            item.Link = new Uri(elementText);
                                            break;
                                        case "description":
                                            item.Description = elementText;
                                            break;
                                        case "author":
                                            item.Author = elementText;
                                            break;
                                        case "comments":
                                            try
                                            {
                                                item.Comments = new Uri(elementText.ToString());
                                            }
                                            catch (Exception)
                                            {
                                                throw new Exception("coments");
                                            }
                                            break;
                                        case "pubdate":
                                            try
                                            {
                                                item.PubDate = DateTime.Parse(elementText.ToString());
                                            }
                                            catch (Exception)
                                            {
                                                try
                                                {
                                                    string tmp = elementText.ToString();
                                                    tmp = tmp.Substring(0, tmp.Length - 5);
                                                    tmp += "GMT";
                                                    item.PubDate = DateTime.Parse(tmp);
                                                }
                                                catch
                                                {
                                                    throw new Exception("horaincorrecta");
                                                }
                                            }
                                            break;
                                    }
                                    break;
                                case "channel":
                                    switch (childElementName)
                                    {
                                        case "title":
                                            canalRSS.Title = elementText;
                                            break;
                                        case "link":
                                            try
                                            {
                                                canalRSS.Link = new Uri(elementText.ToString());
                                            }
                                            catch (Exception)
                                            {
                                                throw new Exception("fallouri");
                                            }
                                            break;
                                        case "description":
                                            canalRSS.Description = elementText;
                                            break;
                                        case "language":
                                            canalRSS.Language = elementText;
                                            break;
                                        case "copyright":
                                            canalRSS.Copyright = elementText;
                                            break;
                                        case "managingeditor":
                                            canalRSS.ManagingEditor = elementText;
                                            break;
                                        case "webmaster":
                                            canalRSS.WebMaster = elementText;
                                            break;
                                        case "pubdate":
                                            try
                                            {
                                                canalRSS.PubDate = DateTime.Parse(elementText);
                                            }
                                            catch (Exception)
                                            {
                                                throw new Exception("fallofecha");
                                            }
                                            break;
                                        case "lastbuilddate":
                                            try
                                            {
                                                canalRSS.LastBuildDate = DateTime.Parse(elementText);
                                            }
                                            catch (Exception)
                                            {
                                                throw new Exception("fallofecha");
                                            }
                                            break;
                                        case "generator":
                                            canalRSS.Generator = elementText;
                                            break;
                                        case "docs":
                                            try
                                            {
                                                canalRSS.Docs = new Uri(elementText);
                                            }
                                            catch (Exception)
                                            {
                                                throw new Exception("fallouri");
                                            }
                                            break;
                                        case "ttl":
                                            try
                                            {
                                                canalRSS.Ttl = int.Parse(elementText);
                                            }
                                            catch (Exception)
                                            {
                                                throw new Exception("fallo parse");
                                            }
                                            break;
                                    }
                                    break;
                                case "image":
                                    switch (childElementName)
                                    {
                                        case "url":
                                            try
                                            {
                                                canalRSS.Image.Url = new Uri(elementText);
                                            }
                                            catch (Exception)
                                            {
                                                throw new Exception("fallo URI");
                                            }
                                            break;
                                        case "title":
                                            canalRSS.Image.Title = elementText;
                                            break;
                                        case "link":
                                            try
                                            {
                                                canalRSS.Image.Link = new Uri(elementText);
                                            }
                                            catch (Exception)
                                            {
                                                throw new Exception("fallo URI");
                                            }
                                            break;
                                        case "description":
                                            canalRSS.Image.Description = elementText;
                                            break;
                                        case "width":
                                            try
                                            {
                                                canalRSS.Image.Width = Byte.Parse(elementText);
                                            }
                                            catch (Exception)
                                            {
                                                throw new Exception("fallo parse");
                                            }
                                            break;
                                        case "height":
                                            try
                                            {
                                                canalRSS.Image.Height = Byte.Parse(elementText);
                                            }
                                            catch (Exception)
                                            {
                                                throw new Exception("fallo parse");
                                            }
                                            break;
                                    }
                                    break;
                                case "textinput":
                                    switch (childElementName)
                                    {
                                        case "title":
                                            canalRSS.TextInput.Title = elementText;
                                            break;
                                        case "description":
                                            canalRSS.TextInput.Description = elementText;
                                            break;
                                        case "name":
                                            canalRSS.TextInput.Name = elementText;
                                            break;
                                        case "link":
                                            try
                                            {
                                                canalRSS.TextInput.Link = new Uri(elementText.ToString());
                                            }
                                            catch (Exception)
                                            {
                                                throw new Exception("fallo URI");
                                            }
                                            break;
                                    }
                                    break;
                                case "skipdays":
                                    if (childElementName == "day")
                                        switch (elementText.ToString().ToLower())
                                        {
                                            case "monday":
                                                canalRSS.SkipDays.Add(1);
                                                break;
                                            case "tuesday":
                                                canalRSS.SkipDays.Add(2);
                                                break;
                                            case "wednesday":
                                                canalRSS.SkipDays.Add(3);
                                                break;
                                            case "thursday":
                                                canalRSS.SkipDays.Add(4);
                                                break;
                                            case "friday":
                                                canalRSS.SkipDays.Add(5);
                                                break;
                                            case "saturday":
                                                canalRSS.SkipDays.Add(6);
                                                break;
                                            case "sunday":
                                                canalRSS.SkipDays.Add(7);
                                                break;
                                        }
                                    break;
                                case "skiphours":
                                    if (childElementName == "hour")
                                    {
                                        canalRSS.SkipHours.Add(Byte.Parse(elementText.ToLower()));
                                    }
                                    break;
                            }
                            break;
                        }
                    #endregion
                    #region text
                    case XmlNodeType.Text:
                        elementText = reader.Value;
                        break;
                    #endregion
                    #region cdata
                    case XmlNodeType.CDATA:
                        elementText = reader.Value;
                        break;
                        #endregion
                }
            }
        }
    }
}
