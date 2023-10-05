using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.ParametroGeneralDS;
using Es.Riam.Gnoss.Logica.ParametrosProyecto;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Util;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Xml;

namespace Es.Riam.Gnoss.Recursos
{

    /// <summary>
    /// Métodos de utilidad para hacer la web multiidioma
    /// </summary>
    [Serializable]
    public class UtilIdiomas : ISerializable
    {
        private static volatile Dictionary<Guid, List<LanguajeText>> PersonalizacionIdiomaPersonalizacion = new Dictionary<Guid, List<LanguajeText>>();

        private static volatile Dictionary<Guid, List<LanguajeText>> PersonalizacionIdiomaProyecto = new Dictionary<Guid, List<LanguajeText>>();

        private static volatile List<LanguajeText> PersonalizacionIdiomaPlataforma = new List<LanguajeText>();

        #region Miembros

        private string IdiomaUsuario = "";

        private XmlDocument mDocumento = null;
        private XmlDocument mDocumento_Personalizado = null;
        private XmlNode mPage = null;
        private static Dictionary<string, Dictionary<string, string>> mDiccionarioLenguajes = null;
        //Diccionario por idioma de las claves, sin tag.
        private static Dictionary<string, Dictionary<string, string>> mDiccionarioLenguajesClaves = null;
        private static object mBloqueoDiccionario = new object();
        private string mDirectorioLenguajes = "";
        private string[] mPreferenciasLenguajes;
        private string mFileName;
        private string mPaginaActual = "";
        private string mCode = "";
        //private static Dictionary<string, string> mListaIdiomas = new Dictionary<string, string>();

        private string IdiomaDefecto = "es";
        private UtilIdiomas UtilIdiomasDefecto = null;

        private static string mTextosEs = null;
        private static string mTextosEn = null;
        private static string mTextosPt = null;
        private static string mTextosDe = null;
        private static string mTextosFr = null;
        private static string mTextosIt = null;

        private static string mTextosCa = null;
        private static string mTextosEu = null;
        private static string mTextosGl = null;

        private static string mTextosPersonalizadosEs = null;
        private static string mTextosPersonalizadosEn = null;
        private static string mTextosPersonalizadosPt = null;

        private bool mTraerDeCache = false;

        private Guid mProyectoID = Guid.Empty;
        private Guid mPersonalizacionID = Guid.Empty;
        private Guid mPersonalizacionEcosistemaID = Guid.Empty;

        private LoggingService mLoggingService;
        private EntityContext mEntityContext;
        private ConfigService mConfigService;

        #endregion

        #region Constructores

        /// <summary>
        /// Constructor a partir de el contenido del fichero de idomas
        /// </summary>
        /// <param name="pClaveIdioma">Clave del idioma</param>
        public UtilIdiomas(string pClaveIdioma, LoggingService loggingService, EntityContext entityContext, ConfigService configService)
        {
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;

            mDirectorioLenguajes = "";
            mPreferenciasLenguajes = Array.Empty<string>();
            IdiomaUsuario = pClaveIdioma;
            ObtenerDiccionarioTraducciones();
            LoadFile();
        }

        /// <summary>
        /// Constructor a partir de el contenido del fichero de idomas
        /// </summary>
        /// <param name="pClaveIdioma">Clave del idioma</param>
        public UtilIdiomas(string pClaveIdioma, Guid pProyectoID, LoggingService loggingService, EntityContext entityContext, ConfigService configService)
        {
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;

            mProyectoID = pProyectoID;

            mDirectorioLenguajes = "";
            mPreferenciasLenguajes = Array.Empty<string>();
            IdiomaUsuario = pClaveIdioma;
            ObtenerDiccionarioTraducciones();
            LoadFile();
        }

        /// <summary>
        /// Constructor a partir de el contenido del fichero de idomas
        /// </summary>
        /// <param name="pClaveIdioma">Clave del idioma</param>
        public UtilIdiomas(string pClaveIdioma, Guid pProyectoID, Guid pPersonalizacionID, Guid pPersonalizacionEcosistemaID, LoggingService loggingService, EntityContext entityContext, ConfigService configService)
        {
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;

            mProyectoID = pProyectoID;
            mPersonalizacionID = pPersonalizacionID;
            mPersonalizacionEcosistemaID = pPersonalizacionEcosistemaID;

            mDirectorioLenguajes = "";
            mPreferenciasLenguajes = Array.Empty<string>();
            IdiomaUsuario = pClaveIdioma;
            ObtenerDiccionarioTraducciones();
            LoadFile();
        }

        /// <summary>
        /// Constructor a partir de el contenido del fichero de idomas
        /// </summary>
        /// <param name="pClaveIdioma">Clave del idioma</param>
        /// <param name="pContenidoArchivo">Contenido del archivo de idioma</param>
        public UtilIdiomas(string pClaveIdioma, string pContenidoArchivo, Guid pProyectoID, Guid pPersonalizacionID, Guid pPersonalizacionEcosistemaID, LoggingService loggingService, EntityContext entityContext, ConfigService configService)
        {
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;

            mProyectoID = pProyectoID;
            mPersonalizacionID = pPersonalizacionID;
            mPersonalizacionEcosistemaID = pPersonalizacionEcosistemaID;

            mDirectorioLenguajes = "";
            mPreferenciasLenguajes = Array.Empty<string>();
            IdiomaUsuario = pClaveIdioma;
            ObtenerDiccionarioTraducciones();
            LoadFileDesdeTexto(pContenidoArchivo);
        }

        /// <summary>
        /// Constructor a partir de un fichero
        /// </summary>
        /// <param name="pDirectorioLenguajes">Directorio donde se encuentran los xml de los idiomas</param>
        /// <param name="pPreferenciasLenguajes">Array con las preferencias de idioma del usuario</param>
        /// <param name="pIdiomaUsuario">Idioma del usuario</param>
        public UtilIdiomas(string pDirectorioLenguajes, string[] pPreferenciasLenguajes, string pIdiomaUsuario, Guid pProyectoID, Guid pPersonalizacionID, Guid pPersonalizacionEcosistemaID, LoggingService loggingService, EntityContext entityContext, ConfigService configService)
        {
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;

            mProyectoID = pProyectoID;
            mPersonalizacionID = pPersonalizacionID;
            mPersonalizacionEcosistemaID = pPersonalizacionEcosistemaID;

            mDirectorioLenguajes = pDirectorioLenguajes;
            mPreferenciasLenguajes = pPreferenciasLenguajes;
            IdiomaUsuario = pIdiomaUsuario;
            ObtenerDiccionarioTraducciones();
            LoadFile();
        }

        /// <summary>
        /// Constructor para la deseralización
        /// </summary>
        /// <param name="info">Datos serializados</param>
        /// <param name="context">Contexto de serialización</param>
        protected UtilIdiomas(SerializationInfo info, LoggingService loggingService, EntityContext entityContext, ConfigService configService)
        {
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;

            mFileName = info.GetString("FileName");
            mDirectorioLenguajes = info.GetString("DirectorioLenguajes");
            mPreferenciasLenguajes = (string[])info.GetValue("PreferenciasLenguajes", typeof(string[]));
            ObtenerDiccionarioTraducciones();
            LoadFile();
        }

        #endregion

        #region Métodos

        #region Privados

        /// <summary>
        /// Carga el documento XML a partir del contenido de un archivo de idiomas
        /// </summary>
        /// <param name="pContenidoArchivoIdioma">Contenido del archivo de idiomas</param>
        private void LoadFileDesdeTexto(string pContenidoArchivoIdioma)
        {
            if ((pContenidoArchivoIdioma != null) && (!pContenidoArchivoIdioma.Trim().Equals(string.Empty)))
            {
                try
                {
                    mDocumento = new XmlDocument();
                    mDocumento.LoadXml(pContenidoArchivoIdioma);
                    if (mDocumento.DocumentElement.Attributes["code"] != null)
                        mCode = mDocumento.DocumentElement.Attributes["code"].Value;
                    else
                        mCode = "es";
                }
                catch
                {
                    mDocumento = null;
                }
            }
        }

        private void LoadFile(string pIdioma = null)
        {
            string contenido = "";
            string contenido_personalizado = "";
            mDocumento = new XmlDocument();
            mDocumento_Personalizado = new XmlDocument();

            if (string.IsNullOrEmpty(pIdioma)) 
            {
                if (!string.IsNullOrEmpty(IdiomaUsuario) && mConfigService.ObtenerListaIdiomasDictionary().ContainsKey(IdiomaUsuario))
                {
                    mFileName = IdiomaUsuario;
                }
                else
                {
                    mFileName = IdiomaDefecto;
                }
            }
            else
            {
                mFileName = pIdioma;
            }
            string idiomaFichero = mFileName;

            if (idiomaFichero.Length > 2) { idiomaFichero = idiomaFichero.Substring(0, 2); }
            
            contenido_personalizado = TextosPersonalizadosEs;

            switch (idiomaFichero)
            {
                case "es":
                    contenido = TextosEs;
                    contenido_personalizado = TextosPersonalizadosEs;
                    break;
                case "en":
                    contenido = TextosEn;
                    contenido_personalizado = TextosPersonalizadosEn;
                    break;
                case "pt":
                    contenido = TextosPt;
                    contenido_personalizado = TextosPersonalizadosPt;
                    break;
                case "de":
                    contenido = TextosDe;
                    break;
                case "fr":
                    contenido = TextosFr;
                    break;
                case "it":
                    contenido = TextosIt;
                    break;
                case "ca":
                    contenido = TextosCa;
                    break;
                case "eu":
                    contenido = TextosEu;
                    break;
                case "gl":
                    contenido = TextosGl;
                    break;
                default:
                    contenido = TextosEs;
                    contenido_personalizado = TextosPersonalizadosEs;
                    break;
            }

            try
            {
                mDocumento.Load(new StringReader(contenido));

                if (!string.IsNullOrEmpty(contenido_personalizado))
                {
                    mDocumento_Personalizado.Load(new StringReader(contenido_personalizado));
                }
                mCode = mFileName;
            }
            catch
            {
                mDocumento = null;
                mDocumento_Personalizado = null;
            }
        }

        #endregion

        #region Públicos

        /*
        /// <summary>
        /// Carga el contenido de un fichero
        /// </summary>
        /// <param name="pFileName">Nombre del fichero</param>
        public void LoadFile(string pFileName)
        {
            mFileName = pFileName;
            LoadFile();
        }
        */
        /// <summary>
        /// Selecciona la página pasada por parámetro como página actual
        /// </summary>
        /// <param name="pPage">Página de un documento</param>
        public void SetPage(string pPage)
        {
            if (mPaginaActual == pPage)
                return;

            mPage = null;
            mPaginaActual = "";

            if (mDocumento != null)
            {
                mPage = mDocumento.SelectSingleNode(string.Format("//page[@name='{0}']", pPage.ToUpper()));
                mPaginaActual = pPage;
            }

            if (mDocumento_Personalizado != null)
            {
                XmlNode nodo_personalizado = mDocumento_Personalizado.SelectSingleNode(string.Format("//page[@name='{0}']", pPage.ToUpper()));
                if (nodo_personalizado != null)
                {
                    if (mPage != null)
                    {
                        mPage.InnerXml += nodo_personalizado.InnerXml;
                    }
                    else
                    {
                        mPage = nodo_personalizado;
                    }
                }
            }
        }

        private string BuscarTextoPersonalizadoVistas(Guid pClavePersonalizacion, string pTextoID)
        {
            if (pClavePersonalizacion != Guid.Empty && PersonalizacionIdiomaPersonalizacion.ContainsKey(pClavePersonalizacion) && PersonalizacionIdiomaPersonalizacion[pClavePersonalizacion].Count > 0)
            {
                LanguajeText textoPersonalizado = PersonalizacionIdiomaPersonalizacion[pClavePersonalizacion].Find(textoIdioma => textoIdioma.ClaveTexto == pTextoID);

                if (textoPersonalizado != null)
                {
                    if (textoPersonalizado.TextoIdioma.ContainsKey(LanguageCode))
                    {
                        return textoPersonalizado.TextoIdioma[LanguageCode];
                    }
                    else
                    {
                        if (LanguageCode.Contains("-"))
                        {
                            string LanguageCodeAux = LanguageCode.Substring(0, LanguageCode.IndexOf('-'));

                            if (textoPersonalizado.TextoIdioma.ContainsKey(LanguageCodeAux))
                            {
                                return textoPersonalizado.TextoIdioma[LanguageCodeAux];
                            }
                        }

                        if (textoPersonalizado.TextoIdioma.ContainsKey(IdiomaDefecto))
                        {
                            return textoPersonalizado.TextoIdioma[IdiomaDefecto];
                        }
                        else
                        {
                            return textoPersonalizado.TextoIdioma.Values.First();
                        }
                    }
                }
            }

            return null;
        }

        private string BuscarTextoPersonalizadoProyecto(string pTextoID)
        {
            if (mProyectoID != Guid.Empty && PersonalizacionIdiomaProyecto.ContainsKey(mProyectoID) && PersonalizacionIdiomaProyecto[mProyectoID].Count > 0)
            {
                LanguajeText textoPersonalizado = PersonalizacionIdiomaProyecto[mProyectoID].Find(textoIdioma => textoIdioma.ClaveTexto == pTextoID);

                if (textoPersonalizado != null)
                {
                    if (textoPersonalizado.TextoIdioma.ContainsKey(LanguageCode))
                    {
                        return textoPersonalizado.TextoIdioma[LanguageCode];
                    }
                    else if (textoPersonalizado.TextoIdioma.ContainsKey(IdiomaDefecto))
                    {
                        return textoPersonalizado.TextoIdioma[IdiomaDefecto];
                    }
                    else
                    {
                        return textoPersonalizado.TextoIdioma.Values.First();
                    }
                }
            }

            return null;
        }

        private string BuscarTextoPersonalizadoPlataforma(string pTextoID)
        {
            if (PersonalizacionIdiomaPlataforma.Count > 0)
            {
                LanguajeText textoPersonalizado = PersonalizacionIdiomaPlataforma.Find(textoIdioma => textoIdioma.ClaveTexto == pTextoID);

                if (textoPersonalizado != null)
                {
                    if (textoPersonalizado.TextoIdioma.ContainsKey(LanguageCode))
                    {
                        return textoPersonalizado.TextoIdioma[LanguageCode];
                    }
                    else if (textoPersonalizado.TextoIdioma.ContainsKey(IdiomaDefecto))
                    {
                        return textoPersonalizado.TextoIdioma[IdiomaDefecto];
                    }
                    else
                    {
                        return textoPersonalizado.TextoIdioma.Values.FirstOrDefault();
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Busca el texto personalizado para el TextoID dado
        /// </summary>
        /// <param name="pTextoID">TextoID del texto personalizado a guardar</param>
        /// <returns>Cadena de texto</returns>
        public string GetTextoPersonalizado(string pTextoID, params string[] parametros)
        {
            string texto = null;

            if (PersonalizacionIdiomaPersonalizacion.Count > 0)
            {
                texto = BuscarTextoPersonalizadoVistas(mPersonalizacionID, pTextoID);

                if (texto == null)
                {
                    texto = BuscarTextoPersonalizadoVistas(mPersonalizacionEcosistemaID, pTextoID);
                }
            }

            //if (mParametroGeneralDS != null && mParametroGeneralDS.TextosPersonalizadosProyecto != null)
            //{
            //    ParametroGeneralDS.TextosPersonalizadosPersonalizacionRow[] filasTextos = (ParametroGeneralDS.TextosPersonalizadosPersonalizacionRow[])mParametroGeneralDS.TextosPersonalizadosPersonalizacion.Select("TextoID = '" + pTextoID + "' AND Language = '" + LanguageCode + "'");

            //    if (filasTextos.Length > 0)
            //    {
            //        texto = filasTextos[0].Texto;
            //    }
            //} 

            if (!string.IsNullOrEmpty(texto))
            {
                texto = texto.Replace('{', '<').Replace('}', '>');
            }

            if (!string.IsNullOrEmpty(texto) && parametros != null && parametros.Length > 0)
            {
                texto = string.Format(texto, parametros);

                if (texto != null && parametros.Length > 0)
                {
                    for (int i = 0; i < parametros.Length; i++)
                    {
                        texto = texto.Replace("@" + (i + 1) + "@", parametros[i]);
                    }
                }
            }

            return texto;
        }

        /// <summary>
        /// Busca el texto pasado por parámetro en un documento
        /// </summary>
        /// <param name="pText">Texto</param>
        /// <returns>Cadena de texto</returns>
        private string GetText(string pText)
        {
            string texto = null;

            texto = BuscarTextoPersonalizadoProyecto(mPaginaActual + "/" + pText);

            if (texto == null)
            {

                texto = BuscarTextoPersonalizadoPlataforma(mPaginaActual + "/" + pText);
            }

            if (string.IsNullOrEmpty(texto))
            {
                pText = pText.ToUpper(new System.Globalization.CultureInfo("en"));

                mDiccionarioLenguajesClaves[mFileName].TryGetValue(pText, out texto);

                //Forma antigua
                /*
                if (mDocumento == null)
                    return "";

                XmlNode el = null;

                if (mPage != null)
                {
                    el = mPage.SelectSingleNode(string.Format("Resource[@tag='{0}']", pText));
                    // if in page subnode the text doesn't exist, try in whole file
                    if (el == null)
                    {
                        el = mDocumento.SelectSingleNode(string.Format("//Resource[@tag='{0}']", pText));
                    }
                }
                else
                {
                    el = mDocumento.SelectSingleNode(string.Format("//Resource[@tag='{0}']", pText));
                }

                if (el != null)
                {
                    texto = el.InnerText;
                }*/
            }

            if (!string.IsNullOrEmpty(texto))
            {
                texto = texto.Replace('{', '<').Replace('}', '>');

                if (texto.Equals("[NULL]"))
                {
                    texto = string.Empty;
                }

                if (mPaginaActual.ToLower() == "urlsem")
                {
                    texto = texto.ToLower();
                }
                return texto;
            }
            else
            {
                if (mCode.Equals(IdiomaDefecto))
                {
                    return null;
                }
                else
                {
                    if (UtilIdiomasDefecto == null)
                    {
                        UtilIdiomasDefecto = new UtilIdiomas("es", mProyectoID, mPersonalizacionID, mPersonalizacionEcosistemaID, mLoggingService, mEntityContext, mConfigService);
                    }
                    return UtilIdiomasDefecto.GetText(mPaginaActual, pText);
                }
            }
        }

        /// <summary>
        /// Obtiene el texto de una clave guardada en HTML
        /// </summary>
        /// <param name="pClave">Clave guardada en HTML</param>
        /// <returns>Texto en el idioma solicitado</returns>
        public string GetTextClaveHTML(string pClave)
        {
            string separador = ",|,";

            string[] arrayClave = pClave.Replace("@#@$", "").Replace("$@#@", "").Split(new string[] { separador }, StringSplitOptions.RemoveEmptyEntries);

            string page = arrayClave[0];
            string text = arrayClave[1];

            string[] parametros = new string[] { };
            if (arrayClave.Length > 2)
            {
                string separador2 = ";|;";
                parametros = arrayClave[2].Split(new string[] { separador2 }, StringSplitOptions.RemoveEmptyEntries);
            }
            //No necesario con el diccionario
            //SetPage(page);

            return GetText(page, text, parametros);
        }

        /// <summary>
        /// Obtiene una clave para insertarla en el HTML y reemplazarla luego
        /// </summary>
        /// <param name="pPage">Etiqueta de la página en el fichero de idioma </param>
        /// <param name="pText">Etiqueta del texto en el fichero de idioma</param>
        /// <returns>Clave para insertarla en el HTML y reemplazarla luego</returns>
        private string GetClaveText(string pPage, string pText)
        {
            string separador = ",|,";
            string clave = pPage + separador + pText;

            return "@#@$" + clave + "$@#@";
        }

        /// <summary>
        /// Obtiene una clave para insertarla en el HTML y reemplazarla luego
        /// </summary>
        /// <param name="pPage">Etiqueta de la página en el fichero de idioma </param>
        /// <param name="pText">Etiqueta del texto en el fichero de idioma</param>
        /// <param name="pParametros">Parámetros</param>
        /// <returns>Clave para insertarla en el HTML y reemplazarla luego</returns>
        public string GetClaveText(string pPage, string pText, params string[] pParametros)
        {
            string separador = ",|,";
            string clave = pPage + separador + pText + separador;

            if (pParametros.Length > 0)
            {
                string separador2 = ";|;";
                for (int i = 0; i < pParametros.Length; i++)
                {
                    clave = clave + separador2 + pParametros[i];
                }
            }

            return "@#@$" + clave + "$@#@";
        }

        /// <summary>
        /// Busca el texto pasado por parámetro en la página indicada de un documento
        /// </summary>
        /// <param name="pPage">Página de un documento</param>
        /// <param name="pText">Texto</param>
        /// <param name="pProyectoID">Identificador del proyecto actual</param>
        /// <returns>Cadena de texto</returns>
        public string GetText(string pPage, string pText)
        {
            //Recorrer el diccionario https://riamgnoss.atlassian.net/browse/GCSD-3186
            //Antiguo metodo
            return ObtenerTexto(pPage, pText.ToUpper());
            /*
            if (TraerDeCache)
            {
                return GetClaveText(pPage, pText);
            }
            else
            {
                SetPage(pPage);
                return GetText(pText);
            }*/
        }

        /// <summary>
        /// Busca el texto pasado por parámetro en la página indicada de un documento
        /// </summary>
        /// <param name="pPage">Página de un documento</param>
        /// <param name="pText">Texto</param>
        /// <param name="pParametros">Parámetros</param>
        /// <param name="pProyectoID">Identificador del proyecto actual</param>
        /// <returns>Cadena de texto</returns>
        public string GetText(string pPage, string pText, params string[] pParametros)
        {
            string texto = GetText(pPage, pText);
            if (texto != null && pParametros.Length > 0)
            {
                for (int i = 0; i < pParametros.Length; i++)
                {
                    texto = texto.Replace("@" + (i + 1) + "@", UtilCadenas.ObtenerTextoDeIdioma(pParametros[i], LanguageCode, IdiomaDefecto));
                }
            }
            return texto;
            //Viejo 
            /*
            if (TraerDeCache)
            {
                return GetClaveText(pPage, pText, pParametros);
            }
            else
            {
                SetPage(pPage);
                return GetText(pText, pParametros);
            }*/
        }

        /// <summary>
        /// Busca el texto pasado por parámetro en la página indicada de un documento
        /// </summary>
        /// <param name="pText">Texto</param>
        /// <param name="pParametros">Parámetros</param>
        /// <returns>Cadena de texto</returns>
        private string GetText(string pText, params string[] pParametros)
        {
            string texto = GetText(pText);
            if (texto != null && pParametros.Length > 0)
            {
                for (int i = 0; i < pParametros.Length; i++)
                {
                    texto = texto.Replace("@" + (i + 1) + "@", UtilCadenas.ObtenerTextoDeIdioma(pParametros[i], LanguageCode, IdiomaDefecto));
                }
            }
            return texto;
        }

        /// <summary>
        /// Obtiene todos las traducciones de un archivo XML en forma de diccionario de diccionarios de la manera (idioma -> (clave -> valor))
        /// </summary>
        /// <param name="pPage">Página</param>
        /// <returns>Diccionario</returns>
        public void ObtenerDiccionarioTraducciones()
        {
            if (mDiccionarioLenguajes == null)
            {
                lock (mBloqueoDiccionario)
                {
                    if (mDiccionarioLenguajes == null)
                    {
                        mDiccionarioLenguajes = new Dictionary<string, Dictionary<string, string>>();
                        mDiccionarioLenguajesClaves = new Dictionary<string, Dictionary<string, string>>();
                        List<string> listaIdiomas = mConfigService.ObtenerListaIdiomas();
                        foreach (string idioma in listaIdiomas)
                        {
                            Dictionary<string, string> diccionarioIdioma = new Dictionary<string, string>();
                            Dictionary<string, string> diccionarioClaves = new Dictionary<string, string>();
                            //recorrer los xml de los idiomas para cargar todas las claves
                            LoadFile(idioma);
                            XmlNodeList nodosPage = mDocumento.SelectNodes("//page");
                            foreach (XmlNode nodePage in nodosPage)
                            {
                                XmlAttributeCollection xmlAttributeCollection = nodePage.Attributes;
                                string pageName = nodePage.Attributes["name"].Value;
                                XmlNodeList nodosResource = nodePage.SelectNodes(string.Format("//page[@name='{0}']//Resource", pageName));

                                foreach (XmlNode nodeResource in nodosResource)
                                {
                                    string resourceTag = nodeResource.Attributes["tag"].Value;
                                    string valor = nodeResource.InnerText;
                                    diccionarioIdioma.TryAdd($"{pageName}__{resourceTag}", valor);
                                    diccionarioClaves.TryAdd($"{resourceTag}", valor);
                                }
                            }
							XmlNodeList nodosPagePersonalizado = mDocumento_Personalizado.SelectNodes("//page");
							foreach (XmlNode nodePage in nodosPagePersonalizado)
							{
								XmlAttributeCollection xmlAttributeCollection = nodePage.Attributes;
								string pageName = nodePage.Attributes["name"].Value;
								XmlNodeList nodosResource = nodePage.SelectNodes(string.Format("//page[@name='{0}']//Resource", pageName));

								foreach (XmlNode nodeResource in nodosResource)
								{
									string resourceTag = nodeResource.Attributes["tag"].Value;
									string valor = nodeResource.InnerText;
									diccionarioIdioma.TryAdd($"{pageName}__{resourceTag}", valor);
									diccionarioClaves.TryAdd($"{resourceTag}", valor);
								}
							}
							mDiccionarioLenguajes.Add(idioma, diccionarioIdioma);
                            mDiccionarioLenguajesClaves.Add(idioma, diccionarioClaves);
                        }
                    }
                }
            }
        }

        public string ObtenerTexto(string pPage, string pText)
        {
            string texto = null;

            texto = BuscarTextoPersonalizadoProyecto(pPage + "/" + pText);

            if (texto == null)
            {

                texto = BuscarTextoPersonalizadoPlataforma(pPage + "/" + pText);
            }

            if (string.IsNullOrEmpty(texto))
            {
                pText = pText.ToUpper(new System.Globalization.CultureInfo("en"));

                mDiccionarioLenguajes[mFileName].TryGetValue($"{pPage}__{pText}", out texto);
                if (string.IsNullOrEmpty(texto))
                {
                    mDiccionarioLenguajesClaves[mFileName].TryGetValue($"{pText}", out texto);
                }

                //Forma antigua
                /*
                if (mDocumento == null)
                    return "";

                XmlNode el = null;

                if (mPage != null)
                {
                    el = mPage.SelectSingleNode(string.Format("Resource[@tag='{0}']", pText));
                    // if in page subnode the text doesn't exist, try in whole file
                    if (el == null)
                    {
                        el = mDocumento.SelectSingleNode(string.Format("//Resource[@tag='{0}']", pText));
                    }
                }
                else
                {
                    el = mDocumento.SelectSingleNode(string.Format("//Resource[@tag='{0}']", pText));
                }

                if (el != null)
                {
                    texto = el.InnerText;
                }*/
            }

            if (!string.IsNullOrEmpty(texto))
            {
                texto = texto.Replace('{', '<').Replace('}', '>');

                if (texto.Equals("[NULL]"))
                {
                    texto = string.Empty;
                }

                if (mPaginaActual.ToLower() == "urlsem")
                {
                    texto = texto.ToLower();
                }
                return texto;
            }
            else
            {
                if (mCode.Equals(IdiomaDefecto))
                {
                    return pText;
                }
                else
                {
                    if (UtilIdiomasDefecto == null)
                    {
                        UtilIdiomasDefecto = new UtilIdiomas("es", mProyectoID, mPersonalizacionID, mPersonalizacionEcosistemaID, mLoggingService, mEntityContext, mConfigService);
                    }
                    return UtilIdiomasDefecto.GetText(pPage, pText);
                }
            }
        }

        /// <summary>
        /// Obtiene todos los nodos de un archivo XML en forma de diccionario de la manera (clave -> valor)
        /// </summary>
        /// <param name="pPage">Página</param>
        /// <returns>Diccionario</returns>
        public Dictionary<string, string> ObtenerDiccionarioDePagina(string pPage)
        {
            SetPage(pPage);
            Dictionary<string, string> mLista = new Dictionary<string, string>();

            if (mDocumento == null)
                return null;

            XmlNodeList el = null;

            #region Pagina x defecto

            string contenido = "";
            switch (IdiomaDefecto)
            {
                case "es":
                    contenido = TextosEs;
                    break;
                case "en":
                    contenido = TextosEn;
                    break;
                case "pt":
                    contenido = TextosPt;
                    break;
                case "de":
                    contenido = TextosDe;
                    break;
                case "fr":
                    contenido = TextosFr;
                    break;
                case "it":
                    contenido = TextosIt;
                    break;
                case "ca":
                    contenido = TextosCa;
                    break;
                case "eu":
                    contenido = TextosEu;
                    break;
                case "gl":
                    contenido = TextosGl;
                    break;
                default:
                    contenido = TextosEs;
                    break;
            }

            XmlDocument docDefecto = new XmlDocument();
            docDefecto.Load(new StringReader(contenido));

            XmlNode nodoDefecto = docDefecto.SelectSingleNode(string.Format("//page[@name='{0}']", pPage.ToUpper()));


            #endregion

            if (mPage != null)
            {
                el = mPage.ChildNodes;
                foreach (XmlNode nodo in el)
                {
                    if (!string.IsNullOrEmpty(nodo.InnerText) && !mLista.ContainsKey(nodo.InnerText))
                    {
                        mLista.Add(nodo.InnerText, nodo.Attributes[0].Value);
                    }
                    else
                    {
                        XmlNode nodoDefectoHijo = nodoDefecto.SelectSingleNode(string.Format("Resource[@tag='{0}']", nodo.Attributes[0].Value.ToUpper()));
                        // if in page subnode the text doesn't exist, try in whole file
                        if (nodoDefectoHijo != null && !mLista.ContainsKey(nodoDefectoHijo.InnerText))
                        {
                            mLista.Add(nodoDefectoHijo.InnerText, nodoDefectoHijo.Attributes[0].Value);
                        }
                    }
                }
            }
            return mLista;
        }

        /// <summary>
        /// Obtiene la url de una imagen pasada por parámetro con idioma
        /// </summary>
        /// <param name="pAplicationPath">Path de la aplicación</param>
        /// <param name="pImage">Cadena de texto con el nombre de la imagen</param>
        /// <returns></returns>
        public string GetImagePath(string pAplicationPath, string pImage)
        {
            return pAplicationPath + "language/img/" + LanguageCode + "/" + pImage;
        }

        public static Stream ObtenerDocumentoTextoIdioma(string pIdioma)
        {
            string contenido = "";
            switch (pIdioma)
            {
                case "es":
                    contenido = TextosEs;
                    break;
                case "en":
                    contenido = TextosEn;
                    break;
                case "pt":
                    contenido = TextosPt;
                    break;
                case "de":
                    contenido = TextosDe;
                    break;
                case "fr":
                    contenido = TextosFr;
                    break;
                case "it":
                    contenido = TextosIt;
                    break;
                case "ca":
                    contenido = TextosCa;
                    break;
                case "eu":
                    contenido = TextosEu;
                    break;
                case "gl":
                    contenido = TextosGl;
                    break;
                default:
                    contenido = TextosEs;
                    break;
            }

            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(contenido);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        #endregion

        #endregion

        #region Propiedades

        public bool TraerDeCache
        {
            get { return mTraerDeCache; }
            set { mTraerDeCache = value; }
        }


        /// <summary>
        /// Obtiene el código del idioma
        /// </summary>
        public string LanguageCode
        {
            get
            {
                return mCode;
            }
        }

        private static string TextosEs
        {
            get
            {
                if (mTextosEs == null)
                {
                    mTextosEs = Properties.Resources.es;
                }
                return mTextosEs;
            }
        }
        private static string TextosEn
        {
            get
            {
                if (mTextosEn == null)
                {
                    mTextosEn = Properties.Resources.en;
                }
                return mTextosEn;
            }
        }
        private static string TextosPt
        {
            get
            {
                if (mTextosPt == null)
                {
                    mTextosPt = Properties.Resources.pt;
                }
                return mTextosPt;
            }
        }
        private static string TextosDe
        {
            get
            {
                if (mTextosDe == null)
                {
                    mTextosDe = Properties.Resources.de;
                }
                return mTextosDe;
            }
        }
        private static string TextosFr
        {
            get
            {
                if (mTextosFr == null)
                {
                    mTextosFr = Properties.Resources.fr;
                }
                return mTextosFr;
            }
        }
        private static string TextosIt
        {
            get
            {
                if (mTextosIt == null)
                {
                    mTextosIt = Properties.Resources.it;
                }
                return mTextosIt;
            }
        }

        private static string TextosPersonalizadosEs
        {
            get
            {
                if (mTextosPersonalizadosEs == null)
                {
                    mTextosPersonalizadosEs = Properties.Resources.es_personalizado;
                }
                return mTextosPersonalizadosEs;
            }
        }
        private static string TextosPersonalizadosEn
        {
            get
            {
                if (mTextosPersonalizadosEn == null)
                {
                    mTextosPersonalizadosEn = Properties.Resources.en_personalizado;
                }
                return mTextosPersonalizadosEn;
            }
        }
        private static string TextosPersonalizadosPt
        {
            get
            {
                if (mTextosPersonalizadosPt == null)
                {
                    mTextosPersonalizadosPt = Properties.Resources.pt_personalizado;
                }
                return mTextosPersonalizadosPt;
            }
        }

        private static string TextosCa
        {
            get
            {
                if (mTextosCa == null)
                {
                    mTextosCa = Properties.Resources.ca;
                }
                return mTextosCa;
            }
        }

        private static string TextosEu
        {
            get
            {
                if (mTextosEu == null)
                {
                    mTextosEu = Properties.Resources.eu;
                }
                return mTextosEu;
            }
        }

        private static string TextosGl
        {
            get
            {
                if (mTextosGl == null)
                {
                    mTextosGl = Properties.Resources.gl;
                }
                return mTextosGl;
            }
        }

        #endregion

        #region Miembros de ISerializable

        /// <summary>
        /// Metodo para serializar el objeto
        /// </summary>
        /// <param name="info">Datos serializados</param>
        /// <param name="context">Contexto de serialización</param>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter
        = true)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("FileName", mFileName);
            info.AddValue("DirectorioLenguajes", mDirectorioLenguajes);
            info.AddValue("PreferenciasLenguajes", mPreferenciasLenguajes);
        }

        #endregion

        public void CargarTextosPersonalizadosDominio(string dominio, Guid personalizacionEcosistemaID)
        {
            ParametroGeneralCN paramCN = new ParametroGeneralCN(mEntityContext, mLoggingService, mConfigService, null);

            List<TextosPersonalizadosPersonalizacion> listaIdiomaPersonalizacionVistas = paramCN.ObtenerTextosPersonalizadosDominio(dominio, personalizacionEcosistemaID);

            Dictionary<Guid, List<LanguajeText>> tempPersonalizacionIdiomaPersonalizacion = new Dictionary<Guid, List<LanguajeText>>();

            foreach (TextosPersonalizadosPersonalizacion textoPersonalizado in listaIdiomaPersonalizacionVistas)
            {
                if (!tempPersonalizacionIdiomaPersonalizacion.ContainsKey(textoPersonalizado.PersonalizacionID))
                {
                    tempPersonalizacionIdiomaPersonalizacion.Add(textoPersonalizado.PersonalizacionID, new List<LanguajeText>());
                }
                var textoIdiomas = tempPersonalizacionIdiomaPersonalizacion[textoPersonalizado.PersonalizacionID].FirstOrDefault(texto => texto.ClaveTexto.Equals(textoPersonalizado.TextoID));
                if (textoIdiomas == null)
                {
                    textoIdiomas = new LanguajeText();
                    textoIdiomas.ClaveTexto = textoPersonalizado.TextoID;
                    textoIdiomas.TextoIdioma = new Dictionary<string, string>();
                    tempPersonalizacionIdiomaPersonalizacion[textoPersonalizado.PersonalizacionID].Add(textoIdiomas);
                }
                if (!textoIdiomas.TextoIdioma.ContainsKey(textoPersonalizado.Language))
                {
                    textoIdiomas.TextoIdioma.Add(textoPersonalizado.Language, textoPersonalizado.Texto);
                }
                
            }

            PersonalizacionIdiomaPersonalizacion = tempPersonalizacionIdiomaPersonalizacion;

            List<TextosPersonalizadosProyecto> listaIdiomaPersonalizacionProyecto = paramCN.ObtenerTextosPersonalizadosProyecto(dominio);

            Dictionary<Guid, List<LanguajeText>> tempPersonalizacionIdiomaProyecto = new Dictionary<Guid, List<LanguajeText>>();

            foreach (TextosPersonalizadosProyecto textoPersonalizado in listaIdiomaPersonalizacionProyecto)
            {
                if (!tempPersonalizacionIdiomaProyecto.ContainsKey(textoPersonalizado.ProyectoID))
                {
                    tempPersonalizacionIdiomaProyecto.Add(textoPersonalizado.ProyectoID, new List<LanguajeText>());
                }
                var textoIdiomas = tempPersonalizacionIdiomaProyecto[textoPersonalizado.ProyectoID].FirstOrDefault(texto => texto.ClaveTexto.Equals(textoPersonalizado.TextoID));
                if (textoIdiomas == null)
                {
                    textoIdiomas = new LanguajeText();
                    textoIdiomas.ClaveTexto = textoPersonalizado.TextoID;
                    textoIdiomas.TextoIdioma = new Dictionary<string, string>();
                    tempPersonalizacionIdiomaProyecto[textoPersonalizado.ProyectoID].Add(textoIdiomas);
                }

                textoIdiomas.TextoIdioma.Add(textoPersonalizado.Language, textoPersonalizado.Texto);
            }

            PersonalizacionIdiomaProyecto = tempPersonalizacionIdiomaProyecto;

            List<TextosPersonalizadosPlataforma> listaIdiomaPersonalizacionPlataforma = paramCN.ObtenerTextosPersonalizadosPlataforma();

            List<LanguajeText> tempPersonalizacionIdiomaPlataforma = new List<LanguajeText>();

            foreach (TextosPersonalizadosPlataforma textoPersonalizado in listaIdiomaPersonalizacionPlataforma)
            {
                var textoIdiomas = tempPersonalizacionIdiomaPlataforma.FirstOrDefault(texto => texto.ClaveTexto.Equals(textoPersonalizado.TextoID));
                if (textoIdiomas == null)
                {
                    textoIdiomas = new LanguajeText();
                    textoIdiomas.ClaveTexto = textoPersonalizado.TextoID;
                    textoIdiomas.TextoIdioma = listaIdiomaPersonalizacionPlataforma.Where(texto => texto.TextoID.Equals(textoPersonalizado.TextoID)).ToDictionary(t => t.Language, t => t.Texto);

                    tempPersonalizacionIdiomaPlataforma.Add(textoIdiomas);
                }
            }

            PersonalizacionIdiomaPlataforma = tempPersonalizacionIdiomaPlataforma;
        }

        public UtilIdiomasSerializable GetUtilIdiomas()
        {
            string texto = mDocumento.OuterXml;
            string textoPersonalizado = null;
            if (mDocumento_Personalizado != null && mDocumento_Personalizado.HasChildNodes)
            {
                textoPersonalizado = mDocumento_Personalizado.OuterXml;
            }

            return new UtilIdiomasSerializable(mCode, mTraerDeCache, texto, textoPersonalizado, PersonalizacionIdiomaProyecto,
                PersonalizacionIdiomaPlataforma, PersonalizacionIdiomaPersonalizacion, mProyectoID, mPersonalizacionID, mPersonalizacionEcosistemaID,
                IdiomaUsuario, mDirectorioLenguajes, mFileName, mPage, mPaginaActual, mPreferenciasLenguajes);
        }
    }

    public class UtilIdiomasFactory
    {
        private LoggingService mLoggingService;
        private EntityContext mEntityContext;
        private ConfigService mConfigService;
        private ConcurrentDictionary<string, UtilIdiomas> mListaUtilIdiomas;

        public UtilIdiomasFactory(LoggingService loggingService, EntityContext entityContext, ConfigService configService)
        {
            mListaUtilIdiomas = new ConcurrentDictionary<string, UtilIdiomas>();
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;
        }

        public UtilIdiomas ObtenerUtilIdiomas(string pIdioma)
        {
            if (!mListaUtilIdiomas.ContainsKey(pIdioma))
            {
                mListaUtilIdiomas.TryAdd(pIdioma, new UtilIdiomas(pIdioma, mLoggingService, mEntityContext, mConfigService));
            }

            return mListaUtilIdiomas[pIdioma];
        }
    }
}
