using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Es.Riam.Gnoss.Web.RSS.Redifusion
{
    class RedWriter
    {
        private RedFuente mFuente;

        /// <summary>
        /// Construcotr
        /// </summary>
        /// <param name="pRSSCanal">Fuente que se pintar�</param>
        public RedWriter(RedFuente pRedFuente)
        {
            mFuente = pRedFuente;
        }

        /// <summary>
        /// M�todo que pinta la fuente
        /// </summary>
        /// <returns>String que representa la fuente en formato RSS2.0</returns>
        public string pintarFuente()
        {
            MemoryStream ms = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(ms, System.Text.Encoding.UTF8);

            pintarInicio(writer);

            pintarCanal(writer);

            foreach (RedItem item in mFuente.Items)
            {
                pintarItem(writer, item);
            }
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.Flush();

            byte[] buffer;
            buffer = ms.ToArray();
            return System.Text.Encoding.UTF8.GetString(buffer);
        }

        /// <summary>
        /// M�todo encargado de pintar el inicio de la fuente
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
        /// M�todo encargado de pintar el canal
        /// </summary>
        /// <param name="pWriter"></param>
        private void pintarCanal(XmlTextWriter pWriter)
        {
            //Pintamos el T�tulo
            if (mFuente.Titulo != string.Empty)
            {
                pWriter.WriteElementString("title", mFuente.Titulo);
            }

            //Pintamos el link
            if (mFuente.Link != null)
            {
                pWriter.WriteElementString("link", mFuente.Link.ToString());
            }

            //Pintamos la descripci�n
            if (mFuente.Descripcion != string.Empty)
            {
                pWriter.WriteElementString("description", "<![CDATA[" + mFuente.Descripcion + "]]>");
            }

            //Pintamos el copyright
            if (mFuente.Copyright != string.Empty)
            {
                pWriter.WriteElementString("copyright", mFuente.Copyright);
            }

            //Pintamos la fecha de publicai�n
            if (mFuente.FechaPublicaion.HasValue)
            {
                pWriter.WriteElementString("PubDate", mFuente.FechaPublicaion.Value.ToUniversalTime().ToString("R"));
            }

            //Pintamos la fecha de actualizaci�n
            if (mFuente.FechaActualizacion.HasValue)
            {
                pWriter.WriteElementString("lastBuildDate", mFuente.FechaActualizacion.Value.ToUniversalTime().ToString("R"));
            }

            //Pintamos el generador
            if (mFuente.Generador != string.Empty)
            {
                pWriter.WriteElementString("generator", mFuente.Generador);
            }

            //Pintamos la documentacion
            if (mFuente.Documentacion != string.Empty)
            {
                pWriter.WriteElementString("docs", mFuente.Documentacion.ToString());
            }

            //Pintamos las categor�as
            if (mFuente.Categorias.Count != 0)
            {
                pintarCategorias(pWriter, mFuente.Categorias);
            }

            //Pintamos la Imagen            
            if (mFuente.Imagen != null)
            {
                pintarImagen(pWriter, mFuente.Imagen);
            }

            //Pintamos el text input
            if (mFuente.TextInput != null)
            {
                pintarTextInput(pWriter, mFuente.TextInput);
            }

            //TODO:Alvaro AUTORES PUBLICADORES CONTIBUYENMTES 
        }

        /// <summary>
        /// M�todo encargado de pintar un Item
        /// </summary>
        /// <param name="pWriter"></param>
        /// <param name="pItem"></param>
        private void pintarItem(XmlTextWriter pWriter, RedItem pItem)
        {
            //Si el �tem tiene t�tulo o descripci�n, lo pintamos
            if (mFuente.Titulo != string.Empty || mFuente.Descripcion != string.Empty)
            {
                pWriter.WriteStartElement("item");

                //Pintamos el t�tulo
                if (pItem.Titulo != string.Empty)
                {
                    pWriter.WriteElementString("title", pItem.Titulo);
                }

                //Pintamos el link
                if (pItem.Link != null)
                {
                    pWriter.WriteElementString("link", pItem.Link.ToString());
                }

                //Pintamos la descripci�n
                if (pItem.Descripcion != string.Empty)
                {
                    pWriter.WriteElementString("description", pItem.Descripcion);
                }

                //Pintamos las categor�as
                if (pItem.Categorias.Count != 0)
                {
                    pintarCategorias(pWriter, pItem.Categorias);
                }

                //Pintamos el link a los comentarios
                if (pItem.Comentarios != null)
                {
                    pWriter.WriteElementString("comments", pItem.Comentarios.ToString());
                }

                //Pintamos el ID
                if (pItem.Id != null)
                {
                    pintarGuid(pWriter, pItem.Id);
                }

                //Pintamos la fecha de publicaci�n
                if (pItem.FechaPublicacion.HasValue)
                {
                    pWriter.WriteElementString("pubDate", pItem.FechaPublicacion.Value.ToUniversalTime().ToString("R"));
                }
                //TODO: mAdjuntos mAutores mPublicadores mContribuyentes; 

                pWriter.WriteEndElement();
            }
        }

        /// <summary>
        /// M�todo encargado de pintar el identificador
        /// </summary>
        /// <param name="pWriter"></param>
        /// <param name="Id"></param>
        private void pintarGuid(XmlTextWriter pWriter, RedItemId Id)
        {
            //Si tiene ID, lo pinta
            if (Id.Id != string.Empty)
            {
                pWriter.WriteStartElement("guid");
                if (Id.IsPermaLink)
                {
                    pWriter.WriteAttributeString("isPermaLink", "true");
                }
                pWriter.WriteString(Id.Id);
                pWriter.WriteEndElement();
            }
        }


        /// <summary>
        /// M�todo encargado de pintar el TextInput
        /// </summary>
        /// <param name="pWriter"></param>
        /// <param name="pTextInput"></param>
        private void pintarTextInput(XmlTextWriter pWriter, RedTextInput pTextInput)
        {
            //Si tiene todos los par�metros que son obligatorios, lo pintamos
            if (pTextInput.Titulo != string.Empty && pTextInput.Nombre != string.Empty && pTextInput.Link != null && pTextInput.Descripcion != string.Empty)
            {
                pWriter.WriteStartElement("textInput");

                pWriter.WriteElementString("title", pTextInput.Titulo);

                pWriter.WriteElementString("name", pTextInput.Nombre);

                pWriter.WriteElementString("link", pTextInput.Link.ToString());

                pWriter.WriteElementString("description", pTextInput.Descripcion);

                pWriter.WriteEndElement();
            }
        }

        /// <summary>
        /// M�todo encargado de pintar la imagen representativa del canal
        /// </summary>
        /// <param name="pWriter"></param>
        /// <param name="pImagen"></param>
        private void pintarImagen(XmlTextWriter pWriter, RedImage pImagen)
        {
            //Si tiene los par�metros obligatorios, la pinta
            if (pImagen.Titulo != string.Empty && pImagen.Url != null && pImagen.Link != null)
            {
                pWriter.WriteStartElement("image");

                pWriter.WriteElementString("title", pImagen.Titulo);

                pWriter.WriteElementString("url", pImagen.Url.ToString());

                pWriter.WriteElementString("link", pImagen.Link.ToString());

                if (pImagen.Ancho != -1)
                {
                    pWriter.WriteElementString("width", pImagen.Ancho.ToString());
                }
                if (pImagen.Alto != -1)
                {
                    pWriter.WriteElementString("height", pImagen.Alto.ToString());
                }
                if (pImagen.Descripcion != string.Empty)
                {
                    pWriter.WriteElementString("description", pImagen.Descripcion);
                }
                pWriter.WriteEndElement();
            }
        }


        /// <summary>
        /// M�todo encargado de pintar una lista de categor�as
        /// </summary>
        /// <param name="pWriter"></param>
        /// <param name="pCategorias"></param>
        private void pintarCategorias(XmlTextWriter pWriter, List<RedCategoria> pCategorias)
        {
            foreach (RedCategoria categoria in pCategorias)
            {
                //Si tiene id, la pinta
                if (categoria.Nombre != string.Empty)
                {
                    pWriter.WriteStartElement("category");
                    if (categoria.Dominio != null)
                    {
                        pWriter.WriteAttributeString("domain", categoria.Dominio.ToString());
                    }
                    pWriter.WriteString(categoria.Nombre);
                    pWriter.WriteEndElement();
                }
            }
        }
    }
}