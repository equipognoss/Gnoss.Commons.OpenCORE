using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Collections;
using System.Net;
using Es.Riam.Util;
using System.Web;
using Es.Riam.Gnoss.Util.General;
using Microsoft.Extensions.Logging;
using Serilog.Core;

namespace Es.Riam.Gnoss.Web.RSS.Redifusion
{

    public enum TipoFeed
    {
        Atom,
        RSS,
        RDF
    }


    public class RedReader
    {
        #region Constantes
        private const string namespaceATOM03 = "http://purl.org/atom/ns#";
        private const string namespaceATOM1 = "http://www.w3.org/2005/Atom";
        private const string namespaceRSS1 = "http://purl.org/rss/1.0/";
        private const string namespaceRDF = "http://www.w3.org/1999/02/22-rdf-syntax-ns#";
        private const string namespaceContent = "http://purl.org/rss/1.0/modules/content/";
        private const string namespaceDublinCore = "http://purl.org/dc/elements/1.1/";
        #endregion

        #region Miembros

        /// <summary>
        /// Lector XML
        /// </summary>
        private XmlReader mReader = null;

        /// <summary>
        /// Fuente resultado de la lectura
        /// </summary>
        private RedFuente mFuente = null;

        /// <summary>
        /// Tipo de fuente que leemos
        /// </summary>
        private TipoFeed mTipoFeed;

        /// <summary>
        /// Pila con la secuencia de Nodos
        /// </summary>
        private Stack mPilaNodosXML = new Stack();

        /// <summary>
        /// Nombre del nodo actual
        /// </summary>
        private string mNodoActual = "";

        /// <summary>
        /// Nombre del nodo padre del actual
        /// </summary>
        private string mNodoPadre = "";

        /// <summary>
        /// Dictionary con los namespaces de la fuente que leemos clave
        /// </summary>
        private Dictionary<string, string> mNamespaces = new Dictionary<string, string>();

        /// <summary>
        /// Lista con los identificadores de los items RDF
        /// </summary>
        private List<string> mRDFItems = new List<string>();

        /// <summary>
        /// Identificador de la imagen RDF
        /// </summary>
        private string mRDFImage = null;

        /// <summary>
        /// Identificador del TextInput RDF
        /// </summary>
        private string mRDFTextInput = null;

        private string mUrl = "";

        private string mNombre = "";

        private string mPass = "";

        private LoggingService mLoggingService;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        #endregion

        #region Constructores

        /// <summary>
        /// Construcor de un lector de fuente de redifusión que puede lanzar WebException
        /// </summary>
        /// <param name="url">URL que representa la fuente de redifusion</param>
        /// <param name="pNombre">Nombre del usuario de la fuente (vacio si no tiene)</param>
        /// <param name="pUrl">Password del usuario de la fuente (vacio si no tiene)</param>
        public RedReader(string pUrl, string pNombre, string pPass, LoggingService loggingService, ILogger<RedReader> logger, ILoggerFactory loggerFactory)
        {
            mLoggingService = loggingService;
            mUrl = pUrl;
            mNombre = pNombre;
            mPass = pPass;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        #endregion

        #region métodos comunes

        /// <summary>
        /// Método encargado de leer la fuente
        /// </summary>
        public RedFuente Read()
        {
            return Read(false);
        }

        /// <summary>
        /// Método encargado de leer la fuente (solo debe ser usado desde el servicio)
        /// </summary>
        public RedFuente Read(bool pArchivoLocal)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            XmlUrlResolver resolver = new XmlUrlResolver();
            resolver.Credentials = new NetworkCredential(mNombre, mPass);
            settings.XmlResolver = resolver;
            settings.IgnoreWhitespace = true;

            //Si no se trata de un archivo local
            string ficheroRSS = "";
            if (!pArchivoLocal)
            {
                //Creamos una copia en local para poder leerlo y quitar algunos datos HTML que producen excepciones (&íacute;, &ntilde; ...)
                WebClient wc = new WebClient();

                string tempNombre = Path.GetTempPath() + Path.GetRandomFileName();
                ficheroRSS = Path.GetTempPath() + Path.GetRandomFileName();

                wc.DownloadFile(mUrl, tempNombre);
                wc.Dispose();

                mReader = XmlTextReader.Create(tempNombre);
                mReader.Read();
                string codificacion = "utf-8";
                if (mReader.GetAttribute("encoding") != null && mReader.GetAttribute("encoding") != "")
                {
                    codificacion = mReader.GetAttribute("encoding");
                }

                mReader.Close();

                StreamReader sr = new StreamReader(tempNombre, Encoding.GetEncoding(codificacion));
                StreamWriter sw = new StreamWriter(ficheroRSS);

                string aux = "";

                while (!sr.EndOfStream)
                {
                    aux = sr.ReadLine().Replace((Char)0147, '"').Replace((Char)0148, '"');
                    aux = aux.Replace((Char)0145, '\'').Replace((Char)0146, '\'');
                    ////aux = UtilCadenas.HtmlEncode(aux);
                    //aux = UtilCadenas.HtmlDecode(aux);
                    aux = UtilCadenas.ReemplazarCaracteresHTML(aux);
                    sw.WriteLine(aux);
                }

                sr.DiscardBufferedData();
                sr.Close();
                sr.Dispose();

                sw.Close();
                sw.Dispose();
                File.Delete(tempNombre);

                sr = new StreamReader(ficheroRSS);

                mReader = XmlTextReader.Create(sr, settings);
            }
            else
            {
                ficheroRSS = mUrl;
                mReader = XmlTextReader.Create(ficheroRSS, settings);
            }

            //Leemos El tipo de fuente
            LeerTipoDeFuente();

            switch (mTipoFeed)
            {
                case TipoFeed.RSS:
                    while (mReader.Read() && mPilaNodosXML.Count == 1)
                    {
                        gestionarNodos();
                        if (mNodoActual == "channel" && mNodoPadre == "rss")
                        {
                            leerFuente();
                        }
                    }
                    break;

                case TipoFeed.Atom:
                    if (mNodoActual == "feed" && (mReader.NamespaceURI == namespaceATOM1 || mReader.NamespaceURI == namespaceATOM03))
                    {
                        leerFuente();
                    }
                    break;
                case TipoFeed.RDF:
                    while (mReader.Read() && mPilaNodosXML.Count == 1)
                    {
                        gestionarNodos();
                        if (mNodoActual == "channel" && mNodoPadre == "rdf")
                        {
                            leerFuente();
                        }
                        else if (mNodoActual == "image")
                        {
                            string id = "";
                            for (int i = 0; i < mReader.AttributeCount; i++)
                            {
                                mReader.MoveToAttribute(i);
                                if (mReader.NamespaceURI == namespaceRDF)
                                {
                                    switch (mReader.LocalName.ToLower())
                                    {
                                        case "about":
                                            id = mReader.Value;
                                            break;
                                    }
                                }
                            }
                            if (mRDFImage == id)
                            {
                                mFuente.Imagen = leerImagen();
                            }
                        }
                        else if (mNodoActual == "item")
                        {
                            string id = "";
                            for (int i = 0; i < mReader.AttributeCount; i++)
                            {
                                mReader.MoveToAttribute(i);
                                if (mReader.NamespaceURI == namespaceRDF)
                                {
                                    switch (mReader.LocalName.ToLower())
                                    {
                                        case "about":
                                            id = mReader.Value;
                                            break;
                                    }
                                }
                            }
                            //if (mRDFItems.Contains(id))
                            //{
                            RedItem item = leerItem();
                            item.Id = new RedItemId();
                            item.Id.Id = id;
                            mFuente.AgnadirItem(item);
                            //}
                        }
                        else if (mNodoActual == "textinput")
                        {
                            string id = "";
                            for (int i = 0; i < mReader.AttributeCount; i++)
                            {
                                mReader.MoveToAttribute(i);
                                if (mReader.NamespaceURI == namespaceRDF)
                                {
                                    switch (mReader.LocalName.ToLower())
                                    {
                                        case "about":
                                            id = mReader.Value;
                                            break;
                                    }
                                }
                            }
                            if (mRDFTextInput == id)
                            {
                                mFuente.TextInput = leerTextInput();
                            }
                        }
                    }
                    break;
            }
            mReader.Close();

            return mFuente;
        }




        private void leerFuente()
        {
            mFuente = new RedFuente();

            //Tomamos el tamaño de la pila para luego saber cuando se acaba el ítem
            int tammPilaNodosXML = mPilaNodosXML.Count;

            RedCategoria categoria = null;

            //Mientras que el tamaño de la pila sea mayor o igual a la inicial seguimos leyendo el ítem
            while (tammPilaNodosXML < mPilaNodosXML.Count + 1 && mReader.Read())
            {
                gestionarNodos();

                #region ATOM
                if (mReader.NamespaceURI == namespaceATOM1 || mReader.NamespaceURI == namespaceATOM03)
                {
                    if (mReader.NodeType == XmlNodeType.Element)
                    {
                        switch (mNodoActual)
                        {
                            case "title":
                                mFuente.Titulo = leerTextoATOM();
                                break;
                            case "rights":
                                mFuente.Copyright = leerTextoATOM();
                                break;
                            case "subtitle":
                                mFuente.Descripcion = leerTextoATOM();
                                break;
                            // case "link": TODO
                            //   break;
                            case "category":
                                RedCategoria categoriaAtom = new RedCategoria();
                                for (int i = 0; i < mReader.AttributeCount; i++)
                                {
                                    mReader.MoveToAttribute(i);
                                    switch (mReader.Name.ToLower())
                                    {
                                        case "term":
                                            if (categoriaAtom.Nombre == "")
                                            {
                                                categoriaAtom.Nombre = mReader.Value;
                                            }
                                            break;
                                        case "scheme":
                                            categoriaAtom.Dominio = new Uri(mReader.Value);
                                            break;
                                        case "label":
                                            categoriaAtom.Nombre = mReader.Value;
                                            break;
                                    }
                                }
                                mFuente.Categorias.Add(categoriaAtom);
                                break;
                            case "author":
                                mFuente.Autores.Add(leerPersona());
                                break;
                            case "contributor":
                                mFuente.Contribuyentes.Add(leerPersona());
                                break;
                            case "entry":
                                mFuente.AgnadirItem(leerItem());
                                break;
                            case "id":
                                mFuente.Identificador = leerTexto();
                                break;
                            case "generator":
                                mFuente.Generador = leerTexto();
                                break;
                            case "logo":
                                RedImage logo = new RedImage();
                                logo.Url = new Uri(leerTexto());
                                mFuente.Imagen = logo;

                                break;
                            case "updated":
                                string txtUpdated = leerTexto();
                                try
                                {
                                    mFuente.FechaActualizacion = DateTime.Parse(txtUpdated);
                                }
                                catch
                                {
                                    try
                                    {
                                        mFuente.FechaActualizacion = DateTime.Parse(txtUpdated.Substring(0, txtUpdated.IndexOf('Z')));
                                    }
                                    catch (Exception ex)
                                    {
                                        mLoggingService.GuardarLogError(ex,mlogger);
                                    }
                                }
                                break;
                            case "link":
                                Object link = leerLinkATOM();
                                if (link is Uri)
                                {
                                    mFuente.Link = (Uri)link;
                                }
                                break;
                        }
                    }
                }
                #endregion

                #region RSS
                if (mTipoFeed == TipoFeed.RSS && mReader.NamespaceURI == "")
                {
                    if (mReader.NodeType == XmlNodeType.Element)
                    {
                        switch (mNodoActual)
                        {
                            case "category":
                                categoria = new RedCategoria();
                                for (int i = 0; i < mReader.AttributeCount; i++)
                                {
                                    mReader.MoveToAttribute(i);
                                    switch (mReader.Name.ToLower())
                                    {
                                        case "domain":
                                            categoria.Dominio = new Uri(mReader.Value);
                                            break;
                                    }
                                }
                                categoria.Nombre = leerTexto();
                                mFuente.Categorias.Add(categoria);
                                break;
                            case "image":
                                mFuente.Imagen = leerImagen();
                                break;
                            case "textinput":
                                mFuente.TextInput = leerTextInput();
                                break;
                            case "item":
                                mFuente.AgnadirItem(leerItem());
                                break;
                            case "title":
                                mFuente.Titulo = leerTexto();
                                break;
                            case "link":
                                mFuente.Link = new Uri(leerTexto());
                                break;
                            case "description":
                                mFuente.Descripcion = leerTexto();
                                break;
                            case "copyright":
                                mFuente.Copyright = leerTexto();
                                break;
                            case "lastbuilddate":
                                string txtLastBuilDate = leerTexto();
                                try
                                {
                                    mFuente.FechaActualizacion = DateTime.Parse(txtLastBuilDate);
                                }
                                catch (Exception)
                                {
                                    try
                                    {
                                        string tmp = txtLastBuilDate;
                                        tmp = tmp.Substring(0, tmp.Length - 5);
                                        tmp += "GMT";
                                        mFuente.FechaActualizacion = DateTime.Parse(tmp);
                                    }
                                    catch
                                    {
                                        throw new Exception("horaincorrecta");
                                    }
                                }
                                break;
                            case "generator":
                                mFuente.Generador = leerTexto();
                                break;
                            case "docs":
                                mFuente.Documentacion = leerTexto();
                                break;
                            case "pubdate":
                                string txtPubdate = leerTexto();
                                try
                                {
                                    mFuente.FechaPublicaion = DateTime.Parse(txtPubdate);
                                }
                                catch (Exception)
                                {
                                    try
                                    {
                                        string tmp = txtPubdate;
                                        tmp = tmp.Substring(0, tmp.Length - 5);
                                        tmp += "GMT";
                                        mFuente.FechaPublicaion = DateTime.Parse(tmp);
                                    }
                                    catch
                                    {
                                        throw new Exception("horaincorrecta");
                                    }
                                }

                                break;
                        }
                    }
                }
                #endregion

                #region RDF

                if (mTipoFeed == TipoFeed.RDF && mReader.NamespaceURI == namespaceRSS1)
                {
                    if (mReader.NodeType == XmlNodeType.Element)
                    {
                        switch (mNodoActual)
                        {
                            case "title":
                                mFuente.Titulo = leerTexto();
                                break;
                            case "link":
                                mFuente.Link = new Uri(leerTexto());
                                break;
                            case "description":
                                mFuente.Descripcion = mReader.Value;
                                break;
                            case "image":
                                for (int i = 0; i < mReader.AttributeCount; i++)
                                {
                                    mReader.MoveToAttribute(i);
                                    if (mReader.NamespaceURI == namespaceRDF)
                                    {
                                        switch (mReader.LocalName.ToLower())
                                        {
                                            case "resource":
                                                mRDFImage = mReader.Value;
                                                break;
                                        }
                                    }
                                }
                                break;
                            case "items":
                                mRDFItems = leerListadoItems();
                                break;
                            case "textinput":
                                for (int i = 0; i < mReader.AttributeCount; i++)
                                {
                                    mReader.MoveToAttribute(i);
                                    if (mReader.NamespaceURI == namespaceRDF)
                                    {
                                        switch (mReader.LocalName.ToLower())
                                        {
                                            case "resource":
                                                mRDFTextInput = mReader.Value;
                                                break;
                                        }
                                    }
                                }
                                break;
                        }
                    }
                }
                #endregion

                #region Dublin Core
                if (mReader.NamespaceURI == namespaceDublinCore)
                {
                    if (mReader.NodeType == XmlNodeType.Element)
                    {
                        switch (mNodoActual)
                        {
                            case "creator":
                                RedPersona creator = new RedPersona();
                                creator.Nombre = leerTexto();
                                mFuente.Autores.Add(creator);
                                break;
                            case "publisher":
                                RedPersona publisher = new RedPersona();
                                publisher.Nombre = leerTexto();
                                mFuente.Publicadores.Add(publisher);
                                break;
                            case "contributor":
                                RedPersona contributor = new RedPersona();
                                contributor.Nombre = leerTexto();
                                mFuente.Contribuyentes.Add(contributor);
                                break;
                        }
                    }
                }

                #endregion
            }
        }


        private List<string> leerListadoItems()
        {
            List<string> listadoItems = new List<string>();

            //Tomamos el tamaño de la pila para luego saber cuando se acaba el istado
            int tammPilaNodosXML = mPilaNodosXML.Count;

            //Mientras que el tamaño de la pila sea mayor o igual a la inicial seguimos leyendo el ítem
            while (tammPilaNodosXML < mPilaNodosXML.Count + 1 && mReader.Read())
            {
                gestionarNodos();

                if (mNodoActual == "li" && mNodoPadre == "seq" && mReader.NamespaceURI == namespaceRDF)
                {
                    for (int i = 0; i < mReader.AttributeCount; i++)
                    {
                        mReader.MoveToAttribute(i);

                        switch (mReader.LocalName.ToLower())
                        {
                            case "resource":
                                listadoItems.Add(mReader.Value);
                                break;
                        }
                    }
                }
            }
            return listadoItems;
        }


        private RedItem leerItem()
        {
            RedItem item = new RedItem();

            //Tomamos el tamaño de la pila para luego saber cuando se acaba el ítem
            int tammPilaNodosXML = mPilaNodosXML.Count;


            //Mientras que el tamaño de la pila sea mayor o igual a la inicial seguimos leyendo el ítem
            while (tammPilaNodosXML < mPilaNodosXML.Count + 1 && mReader.Read())
            {
                gestionarNodos();

                #region RSS
                if (mTipoFeed == TipoFeed.RSS)
                {
                    if (mReader.NodeType == XmlNodeType.Element && mNodoPadre == "item" && mReader.NamespaceURI == "")
                    {
                        switch (mNodoActual)
                        {
                            case "category":
                                RedCategoria categoria = new RedCategoria();
                                for (int i = 0; i < mReader.AttributeCount; i++)
                                {
                                    mReader.MoveToAttribute(i);
                                    switch (mReader.Name.ToLower())
                                    {
                                        case "domain":
                                            try
                                            {
                                                if (!string.IsNullOrEmpty(mReader.Value))
                                                {
                                                    categoria.Dominio = new Uri(mReader.Value);
                                                }
                                            }
                                            catch (Exception)
                                            {
                                                throw new Exception("Exception");
                                            }
                                            break;
                                    }
                                }
                                categoria.Nombre = leerTexto();
                                item.Categorias.Add(categoria);
                                break;
                            case "guid":
                                item.Id = new RedItemId();
                                item.Id.IsPermaLink = true;
                                for (int i = 0; i < mReader.AttributeCount; i++)
                                {
                                    mReader.MoveToAttribute(i);
                                    switch (mReader.Name.ToLower())
                                    {
                                        case "ispermalink":
                                            if (mReader.Value == "false")
                                            {
                                                item.Id.IsPermaLink = false;
                                            }

                                            break;
                                    }
                                }
                                item.Id.Id = leerTexto();
                                break;
                            case "enclosure":
                                RedAdjunto adjunto = new RedAdjunto();
                                for (int i = 0; i < mReader.AttributeCount; i++)
                                {
                                    mReader.MoveToAttribute(i);
                                    switch (mReader.Name.ToLower())
                                    {
                                        case "url":
                                            try
                                            {
                                                adjunto.Href = new Uri(mReader.Value);
                                            }
                                            catch (Exception)
                                            {
                                                throw new Exception("Exception");
                                            }
                                            break;
                                        case "length":
                                            adjunto.Length = int.Parse(mReader.Value);
                                            break;
                                        case "type":
                                            adjunto.Type = mReader.Value;
                                            break;
                                    }
                                }
                                item.Adjuntos.Add(adjunto);
                                break;
                            case "title":
                                item.Titulo = leerTexto();
                                break;
                            case "link":
                                item.Link = new Uri(leerTexto());
                                break;
                            case "description":
                                string descripcion = leerTexto();
                                if (item.Descripcion == "")
                                {
                                    item.Descripcion = descripcion;
                                }
                                else
                                {
                                    item.Resumen = descripcion;
                                }
                                break;
                            case "comments":
                                item.Comentarios = new Uri(leerTexto());
                                break;
                            case "pubdate":
                                string fecha = leerTexto();
                                DateTime fechaTemp;
                                if (!DateTime.TryParse(DateTime.Now.Date.ToShortDateString() + " " + fecha.Split(' ')[4], out fechaTemp))
                                {
                                    fecha = fecha.Replace(fecha.Split(' ')[4], "00:00:00");
                                }
                                if (DateTime.TryParse(fecha, out fechaTemp))
                                {
                                    item.FechaPublicacion = fechaTemp;
                                }
                                else
                                {
                                    string tmp = fecha;
                                    tmp = tmp.Substring(0, tmp.Length - 5);
                                    tmp += "GMT";
                                    if (DateTime.TryParse(tmp, out fechaTemp))
                                    {
                                        item.FechaPublicacion = fechaTemp;
                                    }
                                }
                                break;
                            case "author":
                                string textoAutor = leerTexto();
                                int inicioNombre = textoAutor.IndexOf('(');
                                int finNombre = textoAutor.IndexOf(')');
                                RedPersona autor = new RedPersona();
                                if (inicioNombre < finNombre && !textoAutor.Substring(inicioNombre, finNombre - inicioNombre).Contains("@"))
                                {
                                    autor.Nombre = textoAutor.Substring(inicioNombre + 1, finNombre - inicioNombre - 1);
                                    autor.Mail = textoAutor.Substring(0, inicioNombre);
                                }
                                else
                                {
                                    autor.Nombre = textoAutor;
                                }
                                item.Autores.Add(autor);

                                break;
                        }
                    }
                }
                #endregion

                #region RDF
                if (mTipoFeed == TipoFeed.RDF && mNodoPadre == "item" && mReader.NamespaceURI == namespaceRSS1)
                {
                    if (mReader.NodeType == XmlNodeType.Element)
                    {
                        switch (mNodoActual)
                        {
                            case "title":
                                item.Titulo = leerTexto();
                                break;
                            case "link":
                                item.Link = new Uri(leerTexto());
                                break;
                            case "description":
                                string descripcion = leerTexto();
                                if (item.Descripcion == "")
                                {
                                    item.Descripcion = descripcion;
                                }
                                else
                                {
                                    item.Resumen = descripcion;
                                }

                                //Puede darse el caso de que una descripcion contenga mas elementos dentro de ella, los recorremos para que no se descoloque el puntero:
                                if (mReader.Name != "description")
                                {
                                    //El primer elemento en el fichero estaba mal escrito y era una descipcion vacia tal que <description /> por lo que pasaba por alto el segundo item.
                                    gestionarNodos();
                                    while (mReader.Name != "description" || mReader.NodeType != XmlNodeType.EndElement)
                                    {
                                        //Vamos leyendo el resto de los elementos que pueda haber dentro.
                                        mReader.Read();
                                        gestionarNodos();
                                    }
                                }

                                break;
                        }
                    }
                }
                #endregion RDF

                #region ATOM
                if (mReader.NamespaceURI == namespaceATOM1 || mReader.NamespaceURI == namespaceATOM03)
                {
                    //content link
                    if (mReader.NodeType == XmlNodeType.Element && mNodoPadre == "entry")
                    {
                        switch (mNodoActual)
                        {
                            case "title":
                                item.Titulo = leerTextoATOM();
                                break;
                            case "content":
                                item.Descripcion = leerTextoATOM();
                                break;
                            case "summary":
                                item.Resumen = leerTextoATOM();
                                break;
                            case "category":
                                RedCategoria categoriaAtomItem = new RedCategoria();
                                for (int i = 0; i < mReader.AttributeCount; i++)
                                {
                                    mReader.MoveToAttribute(i);
                                    switch (mReader.Name.ToLower())
                                    {
                                        case "term":
                                            if (categoriaAtomItem.Nombre == "")
                                            {
                                                categoriaAtomItem.Nombre = mReader.Value;
                                            }
                                            break;
                                        case "scheme":
                                            categoriaAtomItem.Dominio = new Uri(mReader.Value);
                                            break;
                                        case "label":
                                            categoriaAtomItem.Nombre = mReader.Value;
                                            break;
                                    }
                                }
                                item.Categorias.Add(categoriaAtomItem);
                                break;
                            case "contributor":
                                item.Contribuyentes.Add(leerPersona());
                                break;
                            case "author":
                                item.Autores.Add(leerPersona());
                                break;
                            case "updated":
                                string txtUpdated = leerTexto();
                                try
                                {
                                    item.FechaPublicacion = DateTime.Parse(txtUpdated);
                                }
                                catch
                                {
                                    try
                                    {
                                        item.FechaPublicacion = DateTime.Parse(txtUpdated.Substring(0, txtUpdated.IndexOf('Z')));
                                    }
                                    catch (Exception ex)
                                    {
                                        mLoggingService.GuardarLogError(ex, mlogger);
                                    }
                                }
                                break;
                            case "id":
                                RedItemId id = new RedItemId();
                                id.Id = leerTexto();
                                item.Id = id;
                                break;
                            case "link":
                                Object link = leerLinkATOM();
                                if (link is Uri)
                                {
                                    item.Link = (Uri)link;
                                }
                                else if (link is RedAdjunto)
                                {
                                    item.Adjuntos.Add((RedAdjunto)link);
                                }
                                break;
                        }
                    }
                }

                #endregion

                #region Content
                if (mReader.NamespaceURI == namespaceContent)
                {
                    if (mReader.NodeType == XmlNodeType.Element && mNodoPadre == "item")
                    {
                        switch (mNodoActual)
                        {
                            case "encoded":
                                string encoded = leerTexto();
                                if (encoded != "")
                                {
                                    if (item.Descripcion != "")
                                    {
                                        item.Resumen = item.Descripcion;
                                    }
                                    item.Descripcion = encoded;
                                }
                                break;
                        }
                    }
                }

                #endregion

                #region Dublin Core
                if (mReader.NamespaceURI == namespaceDublinCore)
                {
                    if (mReader.NodeType == XmlNodeType.Element)
                    {
                        switch (mNodoActual)
                        {
                            case "creator":
                                RedPersona creator = new RedPersona();
                                creator.Nombre = leerTexto();
                                item.Autores.Add(creator);
                                break;
                            case "publisher":
                                RedPersona publisher = new RedPersona();
                                publisher.Nombre = leerTexto();
                                item.Publicadores.Add(publisher);
                                break;
                            case "contributor":
                                RedPersona contributor = new RedPersona();
                                contributor.Nombre = leerTexto();
                                item.Contribuyentes.Add(contributor);
                                break;
                            case "date":
                                string txtUpdated = leerTexto();
                                try
                                {
                                    item.FechaPublicacion = DateTime.Parse(txtUpdated);
                                }
                                catch
                                {
                                    try
                                    {
                                        item.FechaPublicacion = DateTime.Parse(txtUpdated.Substring(0, txtUpdated.IndexOf('Z')));
                                    }
                                    catch (Exception ex)
                                    {
                                        mLoggingService.GuardarLogError(ex, mlogger);
                                    }
                                }
                                break;
                        }
                    }
                }

                #endregion
            }
            return item;
        }



        /// <summary>
        /// Método llamado al inicio para leer el tipo de fuente RSS,RDF o ATOM
        /// </summary>
        private void LeerTipoDeFuente()
        {
            bool tipoLeido = false;
            while (!tipoLeido && mReader.Read())
            {
                if (mReader.NodeType == XmlNodeType.Element)
                {
                    mPilaNodosXML.Push(mReader.LocalName.ToLower());
                    mNodoActual = (string)mPilaNodosXML.Peek();
                    switch (mNodoActual)
                    {
                        case "rss":
                            tipoLeido = true;
                            mTipoFeed = TipoFeed.RSS;
                            break;
                        case "rdf":
                            if (mReader.NamespaceURI == namespaceRDF)
                            {
                                tipoLeido = true;
                                mTipoFeed = TipoFeed.RDF;
                            }
                            break;
                        case "feed":
                            if (mReader.NamespaceURI == namespaceATOM1 || mReader.NamespaceURI == namespaceATOM03)
                            {
                                tipoLeido = true;
                                mTipoFeed = TipoFeed.Atom;
                            }
                            break;
                        default:
                            //TODO fuente no reconocida
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// FIN
        /// </summary>
        private void gestionarNodos()
        {
            if (mReader.NodeType == XmlNodeType.Element)
            {
                mNodoPadre = (string)mPilaNodosXML.Peek();
                if (!mReader.IsEmptyElement)
                {
                    mPilaNodosXML.Push(mReader.LocalName.ToLower());
                }
                mNodoActual = mReader.LocalName.ToLower();
            }
            if (mReader.NodeType == XmlNodeType.EndElement)
            {
                mPilaNodosXML.Pop();
            }
        }
        #endregion                     

        #region métodos ATOM


        private string leerTextoATOM()
        {
            string textoDevuelto = "";
            //Tomamos el tamaño de la pila para luego saber cuando se acaba la persona
            int tammPilaNodosXML = mPilaNodosXML.Count;

            //En esta variable se almacenarán temporalmente los valores de los elementos de ítem
            string texto = "";

            string tipoTexto = "";
            for (int i = 0; i < mReader.AttributeCount; i++)
            {
                mReader.MoveToAttribute(i);
                if (mReader.Name.ToLower() == "type")
                {
                    switch (mReader.Value.ToLower())
                    {
                        case "":
                            tipoTexto = "text";
                            break;
                        case "text":
                            tipoTexto = "text";
                            break;
                        case "html":
                            tipoTexto = "html";
                            break;
                        case "xhtml":
                            tipoTexto = "xhtml";
                            break;
                    }
                }
            }

            //Mientras que el tamaño de la pila sea mayor o igual a la inicial seguimos leyendo el ítem
            while (tammPilaNodosXML < mPilaNodosXML.Count + 1 && mReader.Read())
            {
                if (mReader.NodeType == XmlNodeType.Element && !mReader.IsEmptyElement)
                {
                    mNodoPadre = (string)mPilaNodosXML.Peek();
                    mPilaNodosXML.Push(mReader.Name.ToLower());
                    mNodoActual = (string)mPilaNodosXML.Peek();
                }
                if (mReader.NodeType == XmlNodeType.EndElement)
                {
                    mPilaNodosXML.Pop();
                }
                if (tipoTexto == "xhtml")
                {
                    if (mReader.NodeType == XmlNodeType.Element)
                    {
                        if (mReader.IsEmptyElement)
                        {
                            textoDevuelto += "<" + mReader.Name + "/>";
                        }
                        else
                        {
                            textoDevuelto += "<" + mReader.Name;
                            for (int i = 0; i < mReader.AttributeCount; i++)
                            {
                                mReader.MoveToAttribute(i);
                                textoDevuelto += " " + mReader.Name + "=" + mReader.Value;

                            }
                            textoDevuelto += ">";
                        }
                    }
                    else if (mReader.NodeType == XmlNodeType.EndElement)
                    {
                        if (mPilaNodosXML.Count < tammPilaNodosXML)
                        {
                            break;
                        }
                        else
                        {
                            textoDevuelto += "</" + mReader.Name + ">";
                        }
                    }
                    else if (mReader.NodeType == XmlNodeType.Text || mReader.NodeType == XmlNodeType.CDATA)
                    {
                        textoDevuelto += mReader.Value;
                    }
                }
                else if (mReader.NodeType == XmlNodeType.Text || mReader.NodeType == XmlNodeType.CDATA)
                {
                    texto = mReader.Value;
                    textoDevuelto = texto;
                }
            }
            return textoDevuelto;
        }


        /// <summary>
        /// Leer persona ATOM FIN
        /// </summary>
        /// <returns></returns>
        private RedPersona leerPersona()
        {
            RedPersona persona = new RedPersona();
            //Tomamos el tamaño de la pila para luego saber cuando se acaba la persona
            int tammPilaNodosXML = mPilaNodosXML.Count;


            //Mientras que el tamaño de la pila sea mayor o igual a la inicial seguimos leyendo el ítem
            while (tammPilaNodosXML < mPilaNodosXML.Count + 1 && mReader.Read())
            {
                gestionarNodos();
                if (mReader.NodeType == XmlNodeType.Text && mPilaNodosXML.Count == tammPilaNodosXML + 1)
                {
                    switch (mNodoActual)
                    {
                        case "name":
                            persona.Nombre = mReader.Value;
                            break;
                        case "email":
                            persona.Mail = mReader.Value;
                            break;
                        case "uri":
                            persona.Link = new Uri(mReader.Value);
                            break;
                    }

                }
            }
            return persona;
        }

        /// <summary>
        /// Lee un link ATOM
        /// </summary>
        /// <returns></returns>
        private Object leerLinkATOM()
        {

            //Tomamos el tamaño de la pila para luego saber cuando se acaba el link
            int tammPilaNodosXML = mPilaNodosXML.Count;

            RedAdjunto adjunto = new RedAdjunto();

            bool esadjunto = false;
            bool esalternate = true;
            if (mReader.NodeType == XmlNodeType.Element && mReader.LocalName == "link" && (mReader.NamespaceURI == namespaceATOM1 || mReader.NamespaceURI == namespaceATOM03))
            {
                for (int i = 0; i < mReader.AttributeCount; i++)
                {
                    mReader.MoveToAttribute(i);
                    switch (mReader.Name.ToLower())
                    {
                        case "href":
                            adjunto.Href = new Uri(mReader.Value);
                            break;
                        case "rel":
                            if (mReader.Value == "enclosure")
                            {
                                esadjunto = true;
                            }
                            if (mReader.Value != "alternate")
                            {
                                esalternate = false;
                            }
                            break;
                        case "type":
                            adjunto.Type = mReader.Value;
                            break;
                        case "title":
                            adjunto.Titulo = mReader.Value;
                            break;
                        case "length":
                            adjunto.Length = int.Parse(mReader.Value);
                            break;
                    }
                }
            }
            if (esadjunto)
            {
                return adjunto;
            }
            else if (esalternate)
            {
                return adjunto.Href;
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region  métodos RSS


        /// <summary>
        /// Leer imagen RSS
        /// </summary>
        /// <returns></returns>
        private RedImage leerImagen()
        {
            RedImage image = new RedImage();
            //Tomamos el tamaño de la pila para luego saber cuando se acaba el ítem
            int tammPilaNodosXML = mPilaNodosXML.Count;

            //En esta variable se almacenarán temporalmente los valores de los elementos de ítem
            string texto = "";

            //Mientras que el tamaño de la pila sea mayor o igual a la inicial seguimos leyendo el ítem
            while (tammPilaNodosXML < mPilaNodosXML.Count + 1 && mReader.Read())
            {
                gestionarNodos();
                if (mReader.NodeType == XmlNodeType.Text)
                {
                    texto = mReader.Value;
                    switch (mNodoActual)
                    {
                        case "url":
                            image.Url = new Uri(texto);
                            break;
                        case "link":
                            image.Link = new Uri(texto);
                            break;
                        case "title":
                            image.Titulo = texto;
                            break;
                        case "width":
                            image.Ancho = int.Parse(texto);
                            break;
                        case "height":
                            image.Alto = int.Parse(texto);
                            break;
                        case "description":
                            image.Descripcion = texto;
                            break;
                    }
                }
            }
            return image;

        }


        /// <summary>
        /// Leer text input RSS
        /// </summary>
        /// <returns></returns>
        private RedTextInput leerTextInput()
        {
            RedTextInput textinput = new RedTextInput();
            //Tomamos el tamaño de la pila para luego saber cuando se acaba el ítem
            int tammPilaNodosXML = mPilaNodosXML.Count;

            //En esta variable se almacenarán temporalmente los valores de los elementos de ítem
            string texto = "";

            //Mientras que el tamaño de la pila sea mayor o igual a la inicial seguimos leyendo el ítem
            while (tammPilaNodosXML < mPilaNodosXML.Count + 1 && mReader.Read())
            {
                if (mReader.NodeType == XmlNodeType.Element && !mReader.IsEmptyElement)
                {
                    mNodoPadre = (string)mPilaNodosXML.Peek();
                    mPilaNodosXML.Push(mReader.Name.ToLower());
                    mNodoActual = (string)mPilaNodosXML.Peek();
                }
                if (mReader.NodeType == XmlNodeType.EndElement)
                {
                    mPilaNodosXML.Pop();
                }
                if (mReader.NodeType == XmlNodeType.Text)
                {
                    texto = mReader.Value;
                    switch ((string)mPilaNodosXML.Peek())
                    {
                        case "link":
                            textinput.Link = new Uri(texto);
                            break;
                        case "title":
                            textinput.Titulo = texto;
                            break;
                        case "name":
                            textinput.Nombre = texto;
                            break;
                        case "description":
                            textinput.Descripcion = texto;
                            break;
                    }
                }
            }
            return textinput;
        }
        #endregion


        private string leerTexto()
        {
            if (!mReader.IsEmptyElement)
            {
                mReader.Read();
                if (mReader.NodeType == XmlNodeType.Text || mReader.NodeType == XmlNodeType.CDATA)
                {
                    try
                    {
                        return mReader.Value;
                    }
                    catch
                    {
                        return mReader.ReadInnerXml();
                    }
                }
            }
            return "";
        }
    }
}