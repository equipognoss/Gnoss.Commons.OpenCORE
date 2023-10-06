using Es.Riam.Gnoss.AD.Documentacion;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.Documentacion;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.UtilServiciosWeb;
using Es.Riam.Semantica.OWL;
using Es.Riam.Semantica.Plantillas;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;

namespace Es.Riam.Gnoss.Web.Controles.GeneradorPlantillasOWL
{
    /// <summary>
    /// Clase lectora del xml de configuración.
    /// </summary>
    public class LectorXmlConfig
    {
        #region Miembros

        #region Estáticos

        /// <summary>
        /// Nodo de ConfiguracionGeneral.
        /// </summary>
        public static string NodoConfigGen = "ConfiguracionGeneral"; //También está en Ontologia, si se cambia aquí, hay que cambiarlo allí.

        /// <summary>
        /// Estilos ya cargados.
        /// </summary>
        public volatile static Dictionary<string, KeyValuePair<Guid, Dictionary<string, List<EstiloPlantilla>>>> EstilosCargados = new Dictionary<string, KeyValuePair<Guid, Dictionary<string, List<EstiloPlantilla>>>>();

        private object mBloqueoEstilosCargados = new object();

        #endregion

        /// <summary>
        /// ID de la ontolgía.
        /// </summary>
        private Guid mOntologiaID;

        /// <summary>
        /// ID del proyecto actual.
        /// </summary>
        private Guid mProyectoID;

        /// <summary>
        /// Nombre corto del proyecto actual.
        /// </summary>
        private string mNombreCortoProy;

        /// <summary>
        /// estilo de configuración general.
        /// </summary>
        private EstiloPlantillaConfigGen estiloConfig;

        /// <summary>
        /// Fichero de configuración de la base de datos
        /// </summary>
        private string mFicheroConfiguracionBD;

        /// <summary>
        /// Nombre de la ontología más el tipo de entidad para xml fraccionados por entidad.
        /// </summary>
        private string mNombreOntoConTipoEntidad;

        /// <summary>
        /// Parametro especial semcms que se envia en la url.
        /// </summary>
        private string mParametroSemCms;

        #endregion

        #region Propiedades

        /// <summary>
        /// Parametro especial semcms que se envia en la url.
        /// </summary>
        public string ParametroSemCms
        {
            get
            {
                return mParametroSemCms;
            }
            set
            {
                mParametroSemCms = value;
            }
        }

        private LoggingService mLoggingService;
        private VirtuosoAD mVirtuosoAD;
        private EntityContext mEntityContext;
        private ConfigService mConfigService;
        private RedisCacheWrapper mRedisCacheWrapper;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="pOntologiaID">ID de la ontolgía</param>
        /// <param name="pProyectoID">ID del proyecto actual</param>
        public LectorXmlConfig(Guid pOntologiaID, Guid pProyectoID, LoggingService loggingService, EntityContext entityContext, ConfigService configService, RedisCacheWrapper redisCacheWrapper, VirtuosoAD virtuosoAD)
        {
            mVirtuosoAD = virtuosoAD;
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;
            mRedisCacheWrapper = redisCacheWrapper;

            mOntologiaID = pOntologiaID;
            mProyectoID = pProyectoID;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="pOntologiaID">ID de la ontolgía</param>
        /// <param name="pProyectoID">ID del proyecto actual</param>
        /// <param name="pFicheroConfiguracionBD">Fichero de configuración de BD</param>
        public LectorXmlConfig(Guid pOntologiaID, Guid pProyectoID, string pFicheroConfiguracionBD, LoggingService loggingService, EntityContext entityContext, ConfigService configService, RedisCacheWrapper redisCacheWrapper, VirtuosoAD virtuosoAD)
            : this(pOntologiaID, pProyectoID, loggingService, entityContext, configService, redisCacheWrapper, virtuosoAD)
        {
            mFicheroConfiguracionBD = pFicheroConfiguracionBD;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="pOntologiaID">ID de la ontolgía</param>
        /// <param name="pProyectoID">ID del proyecto actual</param>
        /// <param name="pFicheroConfiguracionBD">Fichero de configuración de BD</param>
        /// <param name="pNombreOnto">Nombre de la ontología</param>
        /// <param name="pTipoEntidad">Tipo de entidad con namespace</param>
        public LectorXmlConfig(Guid pOntologiaID, Guid pProyectoID, string pFicheroConfiguracionBD, string pNombreOnto, string pTipoEntidad, LoggingService loggingService, EntityContext entityContext, ConfigService configService, RedisCacheWrapper redisCacheWrapper, VirtuosoAD virtuosoAD)
            : this(pOntologiaID, pProyectoID, pFicheroConfiguracionBD, loggingService, entityContext, configService, redisCacheWrapper, virtuosoAD)
        {
            mNombreOntoConTipoEntidad = pNombreOnto.Substring(0, pNombreOnto.LastIndexOf(".")).ToLower() + "_" + pTipoEntidad.ToLower().Replace(":", "_");
        }

        #endregion

        #region Métodos

        /// <summary>
        /// Obtiene la configuración xml de una ontología.
        /// </summary>
        /// 
        /// <returns>Diccionario con la configuración</returns>
        public Dictionary<string, List<EstiloPlantilla>> ObtenerConfiguracionXml()
        {
            DocumentacionCL docCL = null;
            if (!string.IsNullOrEmpty(mFicheroConfiguracionBD))
            {
                docCL = new DocumentacionCL(mFicheroConfiguracionBD, mFicheroConfiguracionBD, mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, null);
            }
            else
            {
                docCL = new DocumentacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, null);
            }

            string claveEstilos = null;

            if (mNombreOntoConTipoEntidad != null)
            {
                claveEstilos = mNombreOntoConTipoEntidad;
            }
            else if (!string.IsNullOrEmpty(ParametroSemCms))
            {
                claveEstilos = mOntologiaID.ToString() + "_" + ParametroSemCms;
            }
            else
            {
                claveEstilos = mOntologiaID.ToString();
            }

            Guid? xmlID = docCL.ObtenerObjetoDeCacheLocal(mOntologiaID.ToString()) as Guid?;

            if (xmlID == null)
            {
                xmlID = docCL.ObtenerIDXmlOntologia(mOntologiaID);
                docCL.AgregarObjetoCacheLocal(mProyectoID, mOntologiaID.ToString(), xmlID);
            }

            if (xmlID == null)
            {
                xmlID = Guid.NewGuid();
                docCL.GuardarIDXmlOntologia(mOntologiaID, xmlID.Value);
                docCL.AgregarObjetoCacheLocal(mProyectoID, mOntologiaID.ToString(), xmlID);
            }

            docCL.Dispose();

            if (!EstilosCargados.ContainsKey(mProyectoID + "_" + claveEstilos) || EstilosCargados[mProyectoID + "_" + claveEstilos].Key != xmlID || EstilosCargados[mProyectoID + "_" + claveEstilos].Value == null)
            {
                if (EstilosCargados.ContainsKey(mProyectoID + "_" + claveEstilos))
                {
                    EstilosCargados.Remove(mProyectoID + "_" + claveEstilos);
                }

                Dictionary<string, List<EstiloPlantilla>> estilos = ObtenerConfiguracionXml("", mProyectoID, 2);

                try
                {
                    lock (mBloqueoEstilosCargados)
                    {
                        EstilosCargados.TryAdd(mProyectoID + "_" + claveEstilos, new KeyValuePair<Guid, Dictionary<string, List<EstiloPlantilla>>>(xmlID.Value, estilos));
                    }
                }
                catch (Exception ex)
                {
                    //puede que otro proceso haya cargado esta variable estática
                }
            }

            return EstilosCargados[mProyectoID + "_" + claveEstilos].Value;
        }

        /// <summary>
        /// Obtiene la configuración xml de una ontología.
        /// </summary>
        /// <param name="pTextoXML">Contenido Xml de la plantilla</param>
        /// <param name="pProyectoID">ID del proyecto actual</param>
        /// <returns>Diccionario con la configuración</returns>
        public Dictionary<string, List<EstiloPlantilla>> ObtenerConfiguracionXml(string pTextoXML, Guid pProyectoID, int pNumIntentos = 0)
        {
            byte[] byteArray = null;

            if (!string.IsNullOrEmpty(pTextoXML))
            {
                byteArray = Encoding.UTF8.GetBytes(pTextoXML);

            }
            else
            {

                CallFileService servicioArc = new CallFileService(mConfigService, mLoggingService);


                if (string.IsNullOrEmpty(mNombreOntoConTipoEntidad))
                {
                    byteArray = servicioArc.ObtenerXmlOntologiaBytes(mOntologiaID);
                }
                else
                {
                    byteArray = servicioArc.ObtenerXmlOntologiaFraccionado(mOntologiaID, mNombreOntoConTipoEntidad);
                }

            }

            if (byteArray == null)
            {
                mLoggingService.GuardarLogError($"No se ha conseguido encontrar el XML de la ontologia con ID {mOntologiaID}, se vuelve a intentar {pNumIntentos}");
                if (pNumIntentos > 0)
                {
                    Thread.Sleep(500);
                    return ObtenerConfiguracionXml("", pProyectoID, pNumIntentos - 1);
                }
                else
                {
                    return null;
                }
                
            }

            return ObtenerConfiguracionXml(byteArray, pProyectoID);
        }

        /// <summary>
        /// Obtiene la configuración xml de una ontología.
        /// </summary>
        /// <param name="pByteArray">Contenido Xml en byte[] de la plantilla</param>
        /// <param name="pProyectoID">ID del proyecto actual</param>
        /// <returns>Diccionario con la configuración</returns>
        public Dictionary<string, List<EstiloPlantilla>> ObtenerConfiguracionXml(byte[] pByteArray, Guid pProyectoID)
        {
            ProyectoCL proyCL = null;

            if (!string.IsNullOrEmpty(mFicheroConfiguracionBD))
            {
                proyCL = new ProyectoCL(mFicheroConfiguracionBD, mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, null);
            }
            else
            {
                proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, null);
            }

            mNombreCortoProy = proyCL.ObtenerNombreCortoProyecto(mProyectoID);
            proyCL.Dispose();

            Dictionary<string, List<EstiloPlantilla>> listaEstilos = new Dictionary<string, List<EstiloPlantilla>>();
            XmlDocument docXml = new XmlDocument();

            MemoryStream stream = new MemoryStream(pByteArray);
            docXml.Load(stream);

            #region Configuracion Genereal

            estiloConfig = new EstiloPlantillaConfigGen();

            XmlNode nodoConfig = GetNodo(docXml, "config/" + NodoConfigGen);

            if (nodoConfig != null)
            {
                XmlNode nodoNamespace = GetNodo(nodoConfig, "namespace");

                if (nodoNamespace != null)
                {
                    estiloConfig.Namespace = nodoNamespace.InnerText;
                }

                XmlNode nodoIdiomas = GetNodo(nodoConfig, "idiomasOnto");

                if (nodoIdiomas != null)
                {
                    foreach (XmlNode nodoIdioma in GetNodos(nodoIdiomas, "idiomaOnto"))
                    {
                        if (!estiloConfig.ListaIdiomas.Contains(nodoIdioma.InnerText))
                        {
                            estiloConfig.ListaIdiomas.Add(nodoIdioma.InnerText);
                        }
                    }
                }

                if (GetNodo(nodoConfig, "ocultarTituloDescpImgDoc") != null)
                {
                    estiloConfig.OcultarTituloDescpEImg = true;
                }

                if (GetNodo(nodoConfig, "ocultarRecursoAUsuarioInvitado") != null)
                {
                    estiloConfig.OcultarRecursoAUsuarioInvitado = true;
                }

                if (GetNodo(nodoConfig, "ocultarFechaRec") != null)
                {
                    if (GetNodo(nodoConfig, "ocultarFechaRec").InnerText.ToLower() == "true")
                    {
                        estiloConfig.OcultarFechaRec = true;
                    }
                    else
                    {
                        estiloConfig.MostrarFechaRec = true;
                    }
                }

                if (GetNodo(nodoConfig, "ocultarAutoriaEdicion") != null)
                {
                    estiloConfig.OcultarAutoria = true;
                }

                XmlNode nodoImagenPrincDoc = GetNodo(nodoConfig, "ImagenDoc");
                if (nodoImagenPrincDoc != null)
                {
                    estiloConfig.PropiedadImagenRepre = new KeyValuePair<string, string>(nodoImagenPrincDoc.InnerText, nodoImagenPrincDoc.Attributes["EntidadID"].Value);
                }

                foreach (XmlNode nodoImagenPrincDocFromURL in GetNodos(nodoConfig, "ImagenDocFromURL"))
                {
                    if (estiloConfig.PropiedadImagenFromURL == null)
                    {
                        estiloConfig.PropiedadImagenFromURL = new List<KeyValuePair<string, string>>();
                    }

                    estiloConfig.PropiedadImagenFromURL.Add(new KeyValuePair<string, string>(nodoImagenPrincDocFromURL.InnerText, nodoImagenPrincDocFromURL.Attributes["EntidadID"].Value));
                }

                XmlNode nodoTituloDoc = GetNodo(nodoConfig, "TituloDoc");
                if (nodoTituloDoc != null)
                {
                    estiloConfig.PropiedadTitulo = new KeyValuePair<string, string>(nodoTituloDoc.InnerText, nodoTituloDoc.Attributes["EntidadID"].Value);
                }

                XmlNode nodoTituloPagina = GetNodo(nodoConfig, "TituloPagina");
                if (nodoTituloPagina != null)
                {
                    foreach (XmlNode nodoProp in GetChildNodes(nodoTituloPagina))
                    {
                        if (nodoProp.Name.Equals("NombreOntologia"))
                        {
                            estiloConfig.PropiedadesTitulo.Add("[NombreOntologia]", null);
                        }
                        else if (nodoProp.Attributes["ID"] != null)
                        {
                            estiloConfig.PropiedadesTitulo.Add(nodoProp.Attributes["ID"].Value, nodoProp.Attributes["EntidadID"].Value);
                        }
                        else
                        {
                            estiloConfig.PropiedadesTitulo.Add(nodoProp.InnerText, null);
                        }
                    }
                }

                XmlNode nodoDescripcionDoc = GetNodo(nodoConfig, "DescripcionDoc");
                if (nodoDescripcionDoc != null)
                {
                    estiloConfig.PropiedadDescripcion = new KeyValuePair<string, string>(nodoDescripcionDoc.InnerText, nodoDescripcionDoc.Attributes["EntidadID"].Value);
                }

                XmlNode nodoMenuDocumentoAbajo = GetNodo(nodoConfig, "MenuDocumentoAbajo");
                if (nodoMenuDocumentoAbajo != null)
                {
                    estiloConfig.MenuDocumentoAbajo = (nodoMenuDocumentoAbajo.InnerText.ToLower() == "true");
                }

                foreach (XmlNode nodo in GetNodos(nodoConfig, "TituloSoloMiembros"))
                {
                    if (nodo.Attributes["xml:lang"] == null)
                    {
                        estiloConfig.TituloSoloMiembros.Add("es", nodo.InnerText);
                    }
                    else
                    {
                        estiloConfig.TituloSoloMiembros.Add(nodo.Attributes["xml:lang"].Value, nodo.InnerText);
                    }
                }

                XmlNode nodoCatSelecc = GetNodo(nodoConfig, "CategoriasPorDefecto");
                if (nodoCatSelecc != null)
                {
                    estiloConfig.CategoriasPorDefecto = new List<string>();

                    foreach (string catID in nodoCatSelecc.InnerText.Split(new string[] { ",", "|||" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        estiloConfig.CategoriasPorDefecto.Add(catID);
                    }

                    if (nodoCatSelecc.Attributes["OcultarTesauro"] != null)
                    {
                        estiloConfig.OcultarTesauro = nodoCatSelecc.Attributes["OcultarTesauro"].Value.ToLower().Equals("true");
                    }
                }

                XmlNode nodoIcoGnoss = GetNodo(nodoConfig, "IncluirIconoGnoss");
                if (nodoIcoGnoss != null)
                {
                    estiloConfig.IncluirIconoGnoss = true;
                }

                XmlNode nodoComentCkRec = GetNodo(nodoConfig, "CKEditorComentariosCompleto");
                if (nodoComentCkRec != null)
                {
                    estiloConfig.CKEditorComentariosCompleto = true;
                }

                XmlNode nodoGrupEditFij = GetNodo(nodoConfig, "GruposEditoresFijos");
                if (nodoGrupEditFij != null)
                {
                    estiloConfig.GruposEditoresFijos = new Dictionary<string, List<string>>();

                    List<XmlNode> listaNodosGrupos = GetNodos(nodoGrupEditFij, "Grupo");
                    foreach (XmlNode nodoGrupo in listaNodosGrupos)
                    {
                        string organizacionID = "";
                        if (nodoGrupo.Attributes["Organizacion"] != null)
                        {
                            organizacionID = nodoGrupo.Attributes["Organizacion"].InnerText;
                        }
                        if (!estiloConfig.GruposEditoresFijos.ContainsKey(organizacionID))
                        {
                            estiloConfig.GruposEditoresFijos.Add(organizacionID, new List<string>());
                        }
                        estiloConfig.GruposEditoresFijos[organizacionID].Add(nodoGrupo.InnerText);
                    }
                }

                XmlNode nodoGrupEditPriv = GetNodo(nodoConfig, "GruposEditoresPrivacidad");
                if (nodoGrupEditPriv != null)
                {
                    estiloConfig.GruposEditoresPrivacidad = new Dictionary<string, List<string>>();

                    List<XmlNode> listaNodosGrupos = GetNodos(nodoGrupEditPriv, "Grupo");
                    foreach (XmlNode nodoGrupo in listaNodosGrupos)
                    {
                        string organizacionID = "";
                        if (nodoGrupo.Attributes["Organizacion"] != null)
                        {
                            organizacionID = nodoGrupo.Attributes["Organizacion"].InnerText;
                        }
                        if (!estiloConfig.GruposEditoresPrivacidad.ContainsKey(organizacionID))
                        {
                            estiloConfig.GruposEditoresPrivacidad.Add(organizacionID, new List<string>());
                        }
                        estiloConfig.GruposEditoresPrivacidad[organizacionID].Add(nodoGrupo.InnerText);
                    }
                }

                XmlNode nodoVisiEdiRec = GetNodo(nodoConfig, "VisibilidadEdicionRecurso");
                if (nodoVisiEdiRec != null)
                {
                    XmlNode nodoGrupLectFij = GetNodo(nodoVisiEdiRec, "GruposLectoresFijos");
                    if (nodoGrupLectFij != null)
                    {
                        estiloConfig.TipoVisiblidadEdicionRec = "Lectores";
                        estiloConfig.GruposLectoresFijos = new Dictionary<string, List<string>>();

                        List<XmlNode> listaNodosGrupos = GetNodos(nodoGrupLectFij, "Grupo");
                        foreach (XmlNode nodoGrupo in listaNodosGrupos)
                        {
                            string organizacionID = "";
                            if (nodoGrupo.Attributes["Organizacion"] != null)
                            {
                                organizacionID = nodoGrupo.Attributes["Organizacion"].InnerText;
                            }

                            if (!estiloConfig.GruposLectoresFijos.ContainsKey(organizacionID))
                            {
                                estiloConfig.GruposLectoresFijos.Add(organizacionID, new List<string>());
                            }
                            estiloConfig.GruposLectoresFijos[organizacionID].Add(nodoGrupo.InnerText);
                        }
                    }
                    else if (nodoVisiEdiRec.InnerText == "Abierto" || nodoVisiEdiRec.InnerText == "Miembros" || nodoVisiEdiRec.InnerText == "Editores")
                    {
                        estiloConfig.TipoVisiblidadEdicionRec = nodoVisiEdiRec.InnerText;
                    }
                }

                XmlNode nodoHtmlNuevo = GetNodo(nodoConfig, "HtmlNuevo");
                if (nodoHtmlNuevo != null)
                {
                    if (nodoHtmlNuevo.InnerText.ToLower() == "true")
                    {
                        estiloConfig.HtmlNuevo = 1;
                    }
                    else
                    {
                        short html = 0;
                        short.TryParse(nodoHtmlNuevo.InnerText, out html);
                        estiloConfig.HtmlNuevo = html;
                    }
                }

                XmlNode nodoCatTesOblig = GetNodo(nodoConfig, "CategorizacionTesauroGnossObligatoria");
                if (nodoCatTesOblig != null && nodoCatTesOblig.InnerText.ToLower() == "false")
                {
                    estiloConfig.CategorizacionTesauroGnossNoObligatoria = true;
                }

                XmlNode nodoPropArchCargMasiva = GetNodo(nodoConfig, "PropiedadArchivoCargaMasiva");
                if (nodoPropArchCargMasiva != null)
                {
                    estiloConfig.PropiedadArchivoCargaMasiva = new KeyValuePair<string, string>(nodoPropArchCargMasiva.InnerText, nodoPropArchCargMasiva.Attributes["EntidadID"].Value);
                }

                //se cargan las metas
                List<XmlNode> nodosMetasPagina = GetNodos(nodoConfig, "MetasPagina");

                if (nodosMetasPagina != null && nodosMetasPagina.Count > 0)
                {
                    Dictionary<string, List<Dictionary<string, string>>> dicMetasPagina = new Dictionary<string, List<Dictionary<string, string>>>();

                    foreach (XmlNode nodoMetasPagina in nodosMetasPagina)
                    {
                        string idiomaMetaTags = string.Empty;

                        if (nodoMetasPagina.Attributes["xml:lang"] != null)
                        {
                            idiomaMetaTags = nodoMetasPagina.Attributes["xml:lang"].Value;
                        }

                        List<XmlNode> nodosMetas = GetNodos(nodoMetasPagina, "meta");
                        List<Dictionary<string, string>> listaMetas = new List<Dictionary<string, string>>();

                        if (nodosMetas != null && nodosMetas.Count > 0)
                        {
                            foreach (XmlNode nodoMeta in nodosMetas)
                            {
                                Dictionary<string, string> dic = new Dictionary<string, string>();

                                foreach (XmlAttribute atributo in nodoMeta.Attributes)
                                {
                                    if (!dic.ContainsKey(atributo.Name))
                                    {
                                        dic.Add(atributo.Name, atributo.Value);
                                    }
                                }

                                listaMetas.Add(dic);
                            }
                        }

                        if (listaMetas.Count > 0 && !dicMetasPagina.ContainsKey(idiomaMetaTags))
                        {
                            dicMetasPagina.Add(idiomaMetaTags, listaMetas);
                        }
                    }

                    estiloConfig.MetasHTMLOntologia = dicMetasPagina;
                }

                XmlNode nodoMultiIdioma = GetNodo(nodoConfig, "MultiIdioma");
                if (nodoMultiIdioma != null)
                {
                    estiloConfig.MultiIdioma = true;
                }

                XmlNode nodoIndexRobots = GetNodo(nodoConfig, "IndexRobots");
                if (nodoIndexRobots != null)
                {
                    estiloConfig.IndexRobots = nodoIndexRobots.InnerText;
                }

                XmlNode nodoPropsOnto = GetNodo(nodoConfig, "PropiedadesOntologia");
                if (nodoPropsOnto != null)
                {
                    estiloConfig.PropiedadesOntologia = new Dictionary<string, string>();

                    foreach (XmlNode nodoProp in GetChildNodes(nodoPropsOnto))
                    {
                        if (nodoProp.NodeType == XmlNodeType.Comment)
                        {
                            continue;
                        }

                        string textoNodo = nodoProp.InnerText;

                        if (nodoProp.Attributes["Prefijo"] != null)
                        {
                            textoNodo = nodoProp.Attributes["Prefijo"].Value + "@pref@" + textoNodo;
                        }

                        if (nodoProp.Name == "UrlServicioCreacionRecurso")
                        {
                            estiloConfig.PropiedadesOntologia.Add(PropiedadesOntologia.urlservicio.ToString(), textoNodo);
                        }
                        else if (nodoProp.Name == "UrlServicioComplementarioCreacionRecurso")
                        {
                            estiloConfig.PropiedadesOntologia.Add(PropiedadesOntologia.urlserviciocomplementario.ToString(), textoNodo);
                        }
                        else if (nodoProp.Name == "UrlServicioComplementarioCreacionRecursoSincrono")
                        {
                            estiloConfig.PropiedadesOntologia.Add(PropiedadesOntologia.urlserviciocomplementarioSincrono.ToString(), textoNodo);
                        }
                        else if (nodoProp.Name == "UrlServicioEliminacionRecurso")
                        {
                            estiloConfig.PropiedadesOntologia.Add(PropiedadesOntologia.urlservicioElim.ToString(), textoNodo);
                        }

                        if (nodoProp.Attributes["EnviarRdfAntiguo"] != null && nodoProp.Attributes["EnviarRdfAntiguo"].Value.ToLower() == "true" && !estiloConfig.PropiedadesOntologia.ContainsKey(PropiedadesOntologia.enviarRdfAntiguo.ToString()))
                        {
                            estiloConfig.PropiedadesOntologia.Add(PropiedadesOntologia.enviarRdfAntiguo.ToString(), "true");
                        }
                    }
                }


                #region Ocultar partes menú documento

                if (GetNodo(nodoConfig, "OcultarPublicadorDoc") != null)
                {
                    estiloConfig.OcultarPublicadorDoc = true;
                }

                if (GetNodo(nodoConfig, "OcultarUtilsDoc") != null)
                {
                    estiloConfig.OcultarUtilsDoc = true;
                }

                if (GetNodo(nodoConfig, "OcultarAccionesDoc") != null)
                {
                    estiloConfig.OcultarAccionesDoc = true;
                }

                if (GetNodo(nodoConfig, "OcultarCategoriasDoc") != null)
                {
                    estiloConfig.OcultarCategoriasDoc = true;
                }

                if (GetNodo(nodoConfig, "OcultarEtiquetasDoc") != null)
                {
                    estiloConfig.OcultarEtiquetasDoc = true;
                }

                if (GetNodo(nodoConfig, "OcultarEditoresDoc") != null)
                {
                    estiloConfig.OcultarEditoresDoc = true;
                }

                if (GetNodo(nodoConfig, "OcultarAutoresDoc") != null)
                {
                    estiloConfig.OcultarAutoresDoc = true;
                }

                if (GetNodo(nodoConfig, "OcultarVisitasDoc") != null)
                {
                    estiloConfig.OcultarVisitasDoc = true;
                }

                if (GetNodo(nodoConfig, "OcultarVotosDoc") != null)
                {
                    estiloConfig.OcultarVotosDoc = true;
                }

                if (GetNodo(nodoConfig, "OcultarCompartidoDoc") != null)
                {
                    estiloConfig.OcultarCompartidoDoc = true;
                }

                if (GetNodo(nodoConfig, "OcultarCompartidoEnDoc") != null)
                {
                    estiloConfig.OcultarCompartidoEnDoc = true;
                }

                if (GetNodo(nodoConfig, "OcultarLicenciaDoc") != null)
                {
                    estiloConfig.OcultarLicenciaDoc = true;
                }

                if (GetNodo(nodoConfig, "OcultarVersionDoc") != null)
                {
                    estiloConfig.OcultarVersionDoc = true;
                }


                if (GetNodo(nodoConfig, "OcultarBotonEditarDoc") != null)
                {
                    estiloConfig.OcultarBotonEditarDoc = true;
                }

                if (GetNodo(nodoConfig, "OcultarBotonCrearVersionDoc") != null)
                {
                    estiloConfig.OcultarBotonCrearVersionDoc = true;
                }

                if (GetNodo(nodoConfig, "OcultarBotonEnviarEnlaceDoc") != null)
                {
                    estiloConfig.OcultarBotonEnviarEnlaceDoc = true;
                }

                if (GetNodo(nodoConfig, "OcultarBotonVincularDoc") != null)
                {
                    estiloConfig.OcultarBotonVincularDoc = true;
                }

                XmlNode nodoOcultarEliminar = GetNodo(nodoConfig, "OcultarBotonEliminarDoc");
                if (nodoOcultarEliminar != null)
                {
                    if (nodoOcultarEliminar.Attributes.Count > 0 && nodoOcultarEliminar.Attributes["Condicion"] != null)
                    {
                        estiloConfig.OcultarBotonEliminarDocCondicion = nodoOcultarEliminar.Attributes["Condicion"].Value;
                    }
                    else
                    {
                        estiloConfig.OcultarBotonEliminarDoc = true;
                    }
                }

                if (GetNodo(nodoConfig, "OcultarBotonRestaurarVersionDoc") != null)
                {
                    estiloConfig.OcultarBotonRestaurarVersionDoc = true;
                }

                if (GetNodo(nodoConfig, "OcultarBotonAgregarCategoriaDoc") != null)
                {
                    estiloConfig.OcultarBotonAgregarCategoriaDoc = true;
                }

                if (GetNodo(nodoConfig, "OcultarBotonAgregarEtiquetasDoc") != null)
                {
                    estiloConfig.OcultarBotonAgregarEtiquetasDoc = true;
                }

                if (GetNodo(nodoConfig, "OcultarBotonHistorialDoc") != null)
                {
                    estiloConfig.OcultarBotonHistorialDoc = true;
                }

                if (GetNodo(nodoConfig, "OcultarBotonBloquearComentariosDoc") != null)
                {
                    estiloConfig.OcultarBotonBloquearComentariosDoc = true;
                }

                if (GetNodo(nodoConfig, "OcultarBotonCertificarDoc") != null)
                {
                    estiloConfig.OcultarBotonCertificarDoc = true;
                }

                if (GetNodo(nodoConfig, "OcultarCompartirEspecioPersonal") != null)
                {
                    estiloConfig.OcultarCompartirEspecioPersonal = true;
                }

                if (GetNodo(nodoConfig, "OcultarComentarios") != null)
                {
                    estiloConfig.OcultarComentarios = true;
                }

                if (GetNodo(nodoConfig, "OcultarBloquePrivacidadSeguridadEdicion") != null)
                {
                    estiloConfig.OcultarBloquePrivacidadSeguridadEdicion = true;
                }

                if (GetNodo(nodoConfig, "OcultarBloqueCompartirEdicion") != null)
                {
                    estiloConfig.OcultarBloqueCompartirEdicion = true;
                }

                if (GetNodo(nodoConfig, "OcultarBloquePropiedadIntelectualEdicion") != null)
                {
                    estiloConfig.OcultarBloquePropiedadIntelectualEdicion = true;
                }

                #endregion
            }

            List<EstiloPlantilla> listaAux = new List<EstiloPlantilla>();
            listaAux.Add(estiloConfig);

            listaEstilos.Add("[" + NodoConfigGen + "]", listaAux);

            #endregion

            #region Especificacion Propiedad

            XmlNode nodoEspecfProp = GetNodo(docXml, "config/EspefPropiedad");

            if (nodoEspecfProp != null)
            {
                foreach (XmlNode nodoProp in GetNodos(nodoEspecfProp, "Propiedad"))
                {
                    EstiloPlantillaEspecifProp estiloProp = ObtenerEstiloAPartirNodo(nodoProp, listaEstilos);

                    if (listaEstilos.ContainsKey(nodoProp.Attributes["ID"].Value))
                    {
                        if (((EstiloPlantillaEspecifProp)listaEstilos[nodoProp.Attributes["ID"].Value][0]).NombreEntidad == estiloProp.NombreEntidad)
                        {//Propiedad que se quiere que se pinte dos veces
                            KeyValuePair<string, string> propEnt = new KeyValuePair<string, string>(estiloProp.NombreRealPropiedad, estiloProp.NombreEntidad);

                            if (estiloConfig.PropsRepetidas == null)
                            {
                                estiloConfig.PropsRepetidas = new Dictionary<KeyValuePair<string, string>, List<string>>();
                            }

                            if (!estiloConfig.PropsRepetidas.ContainsKey(propEnt))
                            {
                                estiloConfig.PropsRepetidas.Add(propEnt, new List<string>());
                            }

                            estiloProp.NombreRealPropiedad += "_Rep_" + estiloConfig.PropsRepetidas[propEnt].Count;
                            estiloConfig.PropsRepetidas[propEnt].Add(estiloProp.NombreRealPropiedad);

                            listaAux = new List<EstiloPlantilla>();
                            listaAux.Add(estiloProp);
                            listaEstilos.Add(estiloProp.NombreRealPropiedad, listaAux);
                        }
                        else
                        {
                            listaEstilos[nodoProp.Attributes["ID"].Value].Add(estiloProp);
                        }
                    }
                    else
                    {
                        listaAux = new List<EstiloPlantilla>();
                        listaAux.Add(estiloProp);
                        listaEstilos.Add(nodoProp.Attributes["ID"].Value, listaAux);
                    }
                }
            }

            #endregion

            #region Especificacion Entidad

            XmlNode nodoEspecfEntidad = GetNodo(docXml, "config/EspefEntidad");

            if (nodoEspecfEntidad != null)
            {
                foreach (XmlNode nodoEntidad in GetNodos(nodoEspecfEntidad, "Entidad"))
                {
                    EstiloPlantillaEspecifEntidad estiloEntidad = new EstiloPlantillaEspecifEntidad();

                    foreach (XmlNode nodo in GetChildNodes(nodoEntidad))
                    {
                        if (nodo.Name == "Repesentantes" || nodo.Name == "Representantes")
                        {
                            List<XmlNode> nodosValor = new List<XmlNode>();

                            foreach (XmlNode nodoValor in GetNodos(nodo, "Repesentante"))
                            {
                                nodosValor.Add(nodoValor);
                            }

                            foreach (XmlNode nodoValor in GetNodos(nodo, "Representante"))
                            {
                                nodosValor.Add(nodoValor);
                            }

                            foreach (XmlNode nodoValor in nodosValor)
                            {
                                string nombreProp = nodoValor.InnerText;

                                if (nodoValor.Attributes["PropInterna"] != null)
                                {
                                    nombreProp = "*" + nombreProp;
                                }

                                Representante representante = new Representante();
                                representante.NombrePropiedad = nombreProp;

                                if (nodoValor.Attributes["tipo"] != null)
                                {
                                    representante.TipoRepres = (TipoRepresentacion)short.Parse(nodoValor.Attributes["tipo"].Value);
                                }
                                else
                                {
                                    representante.TipoRepres = TipoRepresentacion.TodosLosCaracteres;
                                }

                                if (nodoValor.Attributes["numCaracteres"] != null)
                                {
                                    representante.NumCaracteres = int.Parse(nodoValor.Attributes["numCaracteres"].Value);
                                }

                                estiloEntidad.AtrRepresentantes.Add(representante);
                            }
                        }
                        else if (nodo.Name == "ClaseCssEdicion")
                        {
                            estiloEntidad.ClaseCssPanel = nodo.InnerText;
                        }
                        else if (nodo.Name == "ClaseCssTitulo")
                        {
                            estiloEntidad.ClaseCssPanelTitulo = nodo.InnerText;
                        }
                        else if (nodo.Name == "TagNameTituloEdicion")
                        {
                            estiloEntidad.TagNameTituloEdicion = nodo.InnerText;
                        }
                        else if (nodo.Name == "TagNameTituloLectura")
                        {
                            estiloEntidad.TagNameTituloLectura = nodo.InnerText;
                        }
                        else if (nodo.Name == "AtrNombre")
                        {
                            if (nodo.Attributes["xml:lang"] == null)
                            {
                                estiloEntidad.AtrNombre.Add("es", nodo.InnerText);
                            }
                            else
                            {
                                estiloEntidad.AtrNombre.Add(nodo.Attributes["xml:lang"].Value, nodo.InnerText);
                            }
                        }
                        else if (nodo.Name == "AtrNombreLectura")
                        {
                            if (nodo.Attributes["xml:lang"] == null)
                            {
                                estiloEntidad.AtrNombreLectura.Add("es", nodo.InnerText);
                            }
                            else
                            {
                                estiloEntidad.AtrNombreLectura.Add(nodo.Attributes["xml:lang"].Value, nodo.InnerText);
                            }
                        }
                        else if (nodo.Name == "TextoLinkEditarDespliegue")
                        {
                            estiloEntidad.TextoLinkEditarDespliegue = nodo.InnerText;
                        }
                        else if (nodo.Name == "PropiedadVinculanteConEntidadHija")
                        {
                            estiloEntidad.PropiedadVinculanteConEntidadHija = nodo.InnerText;
                        }
                        else if (nodo.Name == "DivEntidadDesplegable")
                        {
                            estiloEntidad.DivEntidadDesplegable = true;
                        }
                        else if (nodo.Name == "OrdenEntidad" || nodo.Name == "OrdenEntidadLectura")
                        {
                            List<ElementoOrdenado> elemOrds = ObtenerOrdenEntidad(nodo, null, nodoEntidad.Attributes["ID"].Value);

                            if (nodo.Attributes["Condicion"] != null && !string.IsNullOrEmpty(nodo.Attributes["Condicion"].Value))
                            {
                                if (nodo.Name == "OrdenEntidad")
                                {
                                    if (estiloEntidad.ElementosOrdenadosPorCondicion == null)
                                    {
                                        estiloEntidad.ElementosOrdenadosPorCondicion = new Dictionary<string, List<ElementoOrdenado>>();
                                    }

                                    if (!estiloEntidad.ElementosOrdenadosPorCondicion.ContainsKey(nodo.Attributes["Condicion"].Value))
                                    {
                                        estiloEntidad.ElementosOrdenadosPorCondicion.Add(nodo.Attributes["Condicion"].Value, elemOrds);
                                    }
                                }
                                else
                                {
                                    if (estiloEntidad.ElementosOrdenadosLecturaPorCondicion == null)
                                    {
                                        estiloEntidad.ElementosOrdenadosLecturaPorCondicion = new Dictionary<string, List<ElementoOrdenado>>();
                                    }

                                    if (!estiloEntidad.ElementosOrdenadosLecturaPorCondicion.ContainsKey(nodo.Attributes["Condicion"].Value))
                                    {
                                        estiloEntidad.ElementosOrdenadosLecturaPorCondicion.Add(nodo.Attributes["Condicion"].Value, elemOrds);
                                    }
                                }
                            }
                            else if (nodo.Name == "OrdenEntidad")
                            {
                                estiloEntidad.ElementosOrdenados = elemOrds;
                            }
                            else
                            {
                                estiloEntidad.ElementosOrdenadosLectura = elemOrds;
                            }
                        }
                        else if (nodo.Name == "OrdenEntidadLectura")
                        {
                            estiloEntidad.ElementosOrdenadosLectura = ObtenerOrdenEntidad(nodo, null, nodoEntidad.Attributes["ID"].Value);
                        }
                        else if (nodo.Name == "ClaseCssPanel")
                        {
                            estiloEntidad.ClaseCssPanel = nodo.InnerText;
                        }
                        else if (nodo.Name == "Microdatos")
                        {
                            estiloEntidad.Microdatos = nodo.InnerText;
                        }
                        else if (nodo.Name == "Microformatos")
                        {
                            foreach (XmlNode nodoMicroFor in GetChildNodes(nodo))
                            {
                                estiloEntidad.Microformatos.Add(nodoMicroFor.Attributes["Atributo"].Value, nodoMicroFor.InnerText);
                            }
                        }
                        else if (nodo.Name == "MapaGoogle" || nodo.Name == "MapaGoogleRuta")
                        {
                            estiloEntidad.EsMapaGoogle = true;
                            estiloEntidad.NoSustituirEntidadEnMapaGoogle = (nodo.Attributes["SustituirEntidad"] != null && nodo.Attributes["SustituirEntidad"].Value.ToLower() == "false");

                            if (nodo.Name == "MapaGoogle")
                            {
                                estiloEntidad.PropiedadesDatosMapa = new KeyValuePair<string, string>(nodo.Attributes["lat"].Value, nodo.Attributes["long"].Value);
                            }
                            else
                            {
                                string color = null;

                                if (nodo.Attributes["PropColor"] != null)
                                {
                                    color = nodo.Attributes["PropColor"].Value;
                                }

                                estiloEntidad.PropiedadesDatosMapaRuta = new KeyValuePair<string, string>(nodo.Attributes["ruta"].Value, color);
                            }
                        }
                        else if (nodo.Name == "CampoOrden")
                        {
                            estiloEntidad.CampoOrden = nodo.InnerText;
                            estiloConfig.HayCampoOrden = true;
                        }
                        else if (nodo.Name == "CampoRepresentanteOrden")
                        {
                            estiloEntidad.CampoRepresentanteOrden = nodo.InnerText;
                        }
                        else if (nodo.Name == "CondicionMostrarEntidadPorValorPropiedad")
                        {
                            string props = nodo.InnerText;
                            bool cumplirProp = !(nodo.Attributes["CondicionNegada"] != null && nodo.Attributes["CondicionNegada"].Value.ToLower() == "true");

                            if (props.Contains("|||"))
                            {
                                estiloEntidad.PropsCondicionPintarEntSegunValores.Add(nodo.Attributes["ID"].Value, new KeyValuePair<bool, List<string>>(cumplirProp, new List<string>(props.Split(new string[] { "|||" }, StringSplitOptions.RemoveEmptyEntries))));
                            }
                            else
                            {
                                estiloEntidad.PropsCondicionPintarEntSegunValores.Add(nodo.Attributes["ID"].Value, new KeyValuePair<bool, List<string>>(cumplirProp, new List<string>(props.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))));
                            }
                        }
                        else if (nodo.Name == "AtrPrivadoParaGrupoEditores")
                        {
                            string[] delimiter = { "|" };
                            List<string> listaNombresCortosGrupos = new List<string>(nodo.InnerText.Split(delimiter, StringSplitOptions.RemoveEmptyEntries));

                            IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, null);
                            estiloEntidad.PrivadoParaGrupoEditores = identCN.ObtenerGruposIDPorNombreCortoEnProyectoYEnOrganizacion(listaNombresCortosGrupos, mProyectoID);

                            identCN.Dispose();
                        }
                    }

                    if (listaEstilos.ContainsKey(nodoEntidad.Attributes["ID"].Value))
                    {
                        listaEstilos[nodoEntidad.Attributes["ID"].Value].Add(estiloEntidad);
                    }
                    else
                    {
                        listaAux = new List<EstiloPlantilla>();
                        listaAux.Add(estiloEntidad);
                        listaEstilos.Add(nodoEntidad.Attributes["ID"].Value, listaAux);
                    }
                }
            }

            #endregion

            #region Condiciones

            XmlNode nodoCondiciones = GetNodo(docXml, "config/Condiciones");

            if (nodoCondiciones != null)
            {
                estiloConfig.Condiciones = new Dictionary<string, CondicionSemCms>();

                foreach (XmlNode nodoCondicion in GetNodos(nodoCondiciones, "Condicion"))
                {
                    CondicionSemCms condicion = ObtenerCondicionDeNodo(nodoCondicion);

                    if (condicion != null && !estiloConfig.Condiciones.ContainsKey(condicion.ID))
                    {
                        estiloConfig.Condiciones.Add(condicion.ID, condicion);
                    }
                }
            }

            #endregion

            #region Acciones

            XmlNode nodoAcciones = GetNodo(docXml, "config/Acciones");

            if (nodoAcciones != null)
            {
                estiloConfig.Acciones = new Dictionary<string, AccionSemCms>();

                foreach (XmlNode nodoAccion in GetNodos(nodoAcciones, "Accion"))
                {
                    AccionSemCms accion = ObtenerAccionDeNodo(nodoAccion);

                    if (accion != null && !estiloConfig.Acciones.ContainsKey(accion.ID))
                    {
                        estiloConfig.Acciones.Add(accion.ID, accion);
                    }
                }
            }

            #endregion

            docXml = null;

            return listaEstilos;
        }

        /// <summary>
        /// Obtiene el orden de una entidad.
        /// </summary>
        /// <param name="pNodoOrden">Nodo orden</param>
        /// <param name="pPadre">Elemento padre</param>
        /// <returns>Diccionario con el orden de una comunidad</returns>
        private List<ElementoOrdenado> ObtenerOrdenEntidad(XmlNode pNodoOrden, ElementoOrdenado pPadre, string pTipoEntidad)
        {
            List<ElementoOrdenado> elementosOrdenados = new List<ElementoOrdenado>();

            foreach (XmlNode nodoOrden in GetChildNodes(pNodoOrden))
            {
                ElementoOrdenado elemOrd = new ElementoOrdenado();
                elemOrd.ElementoPadre = pPadre;

                if (elemOrd.ElementoPadre != null && elemOrd.ElementoPadre.NoEditable.HasValue)
                {
                    elemOrd.NoEditable = elemOrd.ElementoPadre.NoEditable.Value;
                }
                else if (nodoOrden.Attributes != null && nodoOrden.Attributes["Editable"] != null)
                {
                    elemOrd.NoEditable = (nodoOrden.Attributes["Editable"].Value.ToLower() == "false");
                }


                if (nodoOrden.Name == "NameProp")
                {
                    elemOrd.NombrePropiedad = new KeyValuePair<string, Propiedad>(nodoOrden.InnerText, null);
                    elementosOrdenados.Add(elemOrd);

                    ObtenerAtributosElementoNameProp(nodoOrden, elemOrd);
                }
                else if (nodoOrden.Name == "Literal")
                {
                    elemOrd.EsLiteral = true;
                    string textoLiteral = "";
                    elementosOrdenados.Add(elemOrd);

                    if (nodoOrden.Attributes["link"] != null)
                    {
                        elemOrd.Link = nodoOrden.Attributes["link"].Value;
                    }

                    if (nodoOrden.Attributes["target"] != null)
                    {
                        elemOrd.TargetLink = nodoOrden.Attributes["target"].Value;
                    }

                    if (nodoOrden.Attributes["Visible"] != null)
                    {
                        elemOrd.LiteralImportante = true;
                    }

                    foreach (XmlNode nodoLitIdioma in GetChildNodes(nodoOrden))
                    {
                        textoLiteral += nodoLitIdioma.InnerText;

                        if (nodoLitIdioma.Attributes != null && nodoLitIdioma.Attributes["xml:lang"] != null)
                        {
                            textoLiteral += "@" + nodoLitIdioma.Attributes["xml:lang"].Value;
                        }

                        textoLiteral += "|||";
                    }

                    elemOrd.NombrePropiedad = new KeyValuePair<string, Propiedad>(textoLiteral, null);
                }
                else if (nodoOrden.Name == "Faceta")
                {
                    elementosOrdenados.Add(elemOrd);
                    elemOrd.EsEspecial = true;
                    elemOrd.DatosEspecial = new Dictionary<string, object>();
                    elemOrd.DatosEspecial.Add("TipoEspecial", nodoOrden.Name);

                    foreach (XmlNode nodoOpcion in GetChildNodes(nodoOrden))
                    {
                        elemOrd.DatosEspecial.Add(nodoOpcion.Name, nodoOpcion.InnerText);
                    }

                }
                else if (nodoOrden.Name == "SeleccionGrupo")
                {
                    elemOrd.EsSelectorGrupo = true;
                    elementosOrdenados.Add(elemOrd);

                    foreach (XmlNode nodoOpcion in GetChildNodes(nodoOrden))
                    {
                        elemOrd.OpcionesSelectorGrupo.Add(nodoOpcion.Attributes["id"].Value, nodoOpcion.InnerText);
                    }
                }
                else if (nodoOrden.Name == "AtrNombreGrupo")
                {
                    string textoPadre = null;
                    string idioma = "[||]";

                    if (nodoOrden.Attributes["xml:lang"] != null)
                    {
                        idioma = "@" + nodoOrden.Attributes["xml:lang"].Value + idioma;
                    }

                    if (string.IsNullOrEmpty(pPadre.NombrePropiedad.Key))
                    {
                        textoPadre = nodoOrden.InnerText + idioma;
                        textoPadre += "|||";

                        if (nodoOrden.Attributes["class"] != null)
                        {
                            textoPadre += nodoOrden.Attributes["class"].Value;
                        }

                        textoPadre += "|||";

                        if (nodoOrden.Attributes["Tipo"] != null)
                        {
                            textoPadre += nodoOrden.Attributes["Tipo"].Value;
                        }
                    }
                    else
                    {
                        textoPadre = pPadre.NombrePropiedad.Key;
                        string parteTexto = textoPadre.Substring(0, textoPadre.IndexOf("|||"));
                        textoPadre = textoPadre.Substring(textoPadre.IndexOf("|||"));
                        textoPadre = parteTexto + nodoOrden.InnerText + idioma + textoPadre;
                    }

                    pPadre.NombrePropiedad = new KeyValuePair<string, Propiedad>(textoPadre, null);
                }
                else if (nodoOrden.Name == "Grupo")
                {
                    elemOrd.EsGrupo = true;

                    if (nodoOrden.Attributes["idEdicion"] != null)
                    {
                        elemOrd.IdGrupo = nodoOrden.Attributes["idEdicion"].Value;
                    }

                    if (nodoOrden.Attributes["classEdicion"] != null)
                    {
                        elemOrd.ClaseGrupo = nodoOrden.Attributes["classEdicion"].Value;
                    }

                    if (nodoOrden.Attributes["idLectura"] != null)
                    {
                        elemOrd.IdGrupoLectura = nodoOrden.Attributes["idLectura"].Value;
                    }

                    if (nodoOrden.Attributes["classLectura"] != null)
                    {
                        elemOrd.ClaseGrupoLectura = nodoOrden.Attributes["classLectura"].Value;
                    }

                    if (nodoOrden.Attributes["id"] != null)
                    {
                        elemOrd.IdGrupo = nodoOrden.Attributes["id"].Value;
                        elemOrd.IdGrupoLectura = elemOrd.IdGrupo;
                    }

                    if (nodoOrden.Attributes["class"] != null)
                    {
                        elemOrd.ClaseGrupo = nodoOrden.Attributes["class"].Value;
                        elemOrd.ClaseGrupoLectura = elemOrd.ClaseGrupo;
                    }

                    if (nodoOrden.Attributes["Tipo"] != null)
                    {
                        elemOrd.TipoGrupo = nodoOrden.Attributes["Tipo"].Value;
                    }

                    elemOrd.Hijos = ObtenerOrdenEntidad(nodoOrden, elemOrd, pTipoEntidad);

                    elementosOrdenados.Add(elemOrd);
                }
                else if (nodoOrden.Name == "Propiedad")
                {
                    //Obtengo el nodo NameProp dentro de propiedad:
                    List<ElementoOrdenado> elementosOrdenadosAux = ObtenerOrdenEntidad(nodoOrden, pPadre, pTipoEntidad);
                    elementosOrdenados.AddRange(elementosOrdenadosAux);

                    foreach (XmlNode nodoProp in GetChildNodes(nodoOrden))
                    {
                        if (nodoProp.Name == "CondicionMostrarEntidadPorValorPropiedad")
                        {
                            string props = nodoProp.InnerText;
                            bool cumplirProp = !(nodoProp.Attributes["CondicionNegada"] != null && nodoProp.Attributes["CondicionNegada"].Value.ToLower() == "true");

                            if (props.Contains("|||"))
                            {
                                elementosOrdenadosAux[0].PropsCondicionPintarEntSegunValores.Add(nodoProp.Attributes["ID"].Value, new KeyValuePair<bool, List<string>>(cumplirProp, new List<string>(props.Split(new string[] { "|||" }, StringSplitOptions.RemoveEmptyEntries))));
                            }
                            else
                            {
                                elementosOrdenadosAux[0].PropsCondicionPintarEntSegunValores.Add(nodoProp.Attributes["ID"].Value, new KeyValuePair<bool, List<string>>(cumplirProp, new List<string>(props.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))));
                            }
                        }
                    }
                }
                else if (nodoOrden.Name == "Boton")
                {
                    elementosOrdenados.Add(elemOrd);
                    CompletarElemOrdNodoBoton(nodoOrden, elemOrd);
                }
            }

            return elementosOrdenados;
        }

        /// <summary>
        /// Completa un elemento ordenado a partir de un nodo botón.
        /// </summary>
        /// <param name="pNodoBoton">Nodo botón</param>
        /// <param name="pElemOrd">Elemento ordenado</param>
        private void CompletarElemOrdNodoBoton(XmlNode pNodoBoton, ElementoOrdenado pElemOrd)
        {
            pElemOrd.EsEspecial = true;
            pElemOrd.DatosEspecial = new Dictionary<string, object>();
            pElemOrd.DatosEspecial.Add("TipoEspecial", "Boton");
            string textoLiteral = "";

            foreach (XmlNode nodoLitIdioma in GetChildNodes(pNodoBoton))
            {
                if (nodoLitIdioma.Name == "AtrNombreLectura")
                {
                    textoLiteral += nodoLitIdioma.InnerText;

                    if (nodoLitIdioma.Attributes != null && nodoLitIdioma.Attributes["xml:lang"] != null)
                    {
                        textoLiteral += "@" + nodoLitIdioma.Attributes["xml:lang"].Value;
                    }

                    textoLiteral += "|||";
                }
            }

            if (pNodoBoton.Attributes["Condicion"] != null)
            {
                pElemOrd.DatosEspecial.Add("Condicion", pNodoBoton.Attributes["Condicion"].Value);
            }

            if (pNodoBoton.Attributes["Accion"] != null)
            {
                pElemOrd.DatosEspecial.Add("Accion", pNodoBoton.Attributes["Accion"].Value);
            }

            pElemOrd.NombrePropiedad = new KeyValuePair<string, Propiedad>(textoLiteral, null);
        }

        /// <summary>
        /// Obtiene los atributos de un elemento ordenado de tipo NameProp.
        /// </summary>
        /// <param name="pNodoOrden">Nodo NameProp</param>
        /// <param name="pElemOrd">Elemento ordenado</param>
        private void ObtenerAtributosElementoNameProp(XmlNode pNodoOrden, ElementoOrdenado pElemOrd)
        {
            if (pNodoOrden.Attributes["PropDeEntHija"] != null)
            {
                pElemOrd.PropDeEntHija = pNodoOrden.Attributes["PropDeEntHija"].Value;
            }

            if (pNodoOrden.Attributes["SoloPrimerValor"] != null)
            {
                pElemOrd.SoloPrimerValor = true;
            }

            if (pNodoOrden.Attributes["SinTitulo"] != null)
            {
                pElemOrd.SinTitulo = true;
            }

            if (pNodoOrden.Attributes["Tipo"] != null)
            {
                pElemOrd.TipoPresentacion = pNodoOrden.Attributes["Tipo"].Value;
            }

            if (pNodoOrden.Attributes["link"] != null)
            {
                pElemOrd.Link = pNodoOrden.Attributes["link"].Value;
            }

            if (pNodoOrden.Attributes["target"] != null)
            {
                pElemOrd.TargetLink = pNodoOrden.Attributes["target"].Value;
            }

            if (pNodoOrden.Attributes["SizeFoto"] != null)
            {
                pElemOrd.SizeFoto = pNodoOrden.Attributes["SizeFoto"].Value;
            }

            if (pNodoOrden.Attributes["SizeAumentoFoto"] != null)
            {
                pElemOrd.SizeAumentoFoto = pNodoOrden.Attributes["SizeAumentoFoto"].Value;
            }

            if (pNodoOrden.Attributes["MensajeAyuda"] != null)
            {
                pElemOrd.MensajeAyuda = pNodoOrden.Attributes["MensajeAyuda"].Value;
            }

            if (pNodoOrden.Attributes["SoloIdiomaNavegacion"] != null && pNodoOrden.Attributes["SoloIdiomaNavegacion"].Value.ToLower() == "true")
            {
                pElemOrd.SoloIdiomaNavegacion = true;
            }
        }

        /// <summary>
        /// Obtien el estilo de un nodo de una propiedad.
        /// </summary>
        /// <param name="pNodoProp">Nodo xml de la propiedad</param>
        /// <returns>Estilo de un nodo de una propiedad</returns>
        private EstiloPlantillaEspecifProp ObtenerEstiloAPartirNodo(XmlNode pNodoProp, Dictionary<string, List<EstiloPlantilla>> pLIstaEstilos)
        {
            EstiloPlantillaEspecifProp estiloProp = new EstiloPlantillaEspecifProp();

            if (pNodoProp.Attributes["EntidadID"] != null)
            {
                estiloProp.NombreEntidad = pNodoProp.Attributes["EntidadID"].Value;
            }

            estiloProp.NombreRealPropiedad = pNodoProp.Attributes["ID"].Value;

            foreach (XmlNode nodo in GetChildNodes(pNodoProp))
            {
                if (nodo.Name == "TipoCampo")
                {
                    if (nodo.InnerText == "Imagen")
                    {
                        estiloProp.TipoCampoSetter = TipoCampoOntologia.Imagen;
                    }
                    else if (nodo.InnerText == "Video")
                    {
                        estiloProp.TipoCampoSetter = TipoCampoOntologia.Video;
                    }
                    else if (nodo.InnerText == "Archivo")
                    {
                        estiloProp.TipoCampoSetter = TipoCampoOntologia.Archivo;
                    }
                    else if (nodo.InnerText == "ArchivoLink")
                    {
                        estiloProp.TipoCampoSetter = TipoCampoOntologia.ArchivoLink;
                    }
                    else if (nodo.InnerText == "TextEditor")
                    {
                        estiloProp.TipoCampoSetter = TipoCampoOntologia.Tiny;
                    }
                    else if (nodo.InnerText == "TextArea")
                    {
                        estiloProp.TipoCampoSetter = TipoCampoOntologia.TextArea;
                    }
                    else if (nodo.InnerText == "Link")
                    {
                        estiloProp.TipoCampoSetter = TipoCampoOntologia.Link;
                    }
                    else if (nodo.InnerText == "EmbebedLink")
                    {
                        estiloProp.TipoCampoSetter = TipoCampoOntologia.EmbebedLink;
                    }
                    else if (nodo.InnerText == "ImagenExterna")
                    {
                        estiloProp.TipoCampoSetter = TipoCampoOntologia.ImagenExterna;
                    }
                    else if (nodo.InnerText == "EmbebedObject")
                    {
                        estiloProp.TipoCampoSetter = TipoCampoOntologia.EmbebedObject;
                    }
                }
                else if (nodo.Name == "UsarChecks")
                {
                    estiloProp.EsPropiedadConValoresCheck = true;
                }
                else if (nodo.Name == "ValoresCombo")
                {
                    if (nodo.Attributes["valorDefecto"] != null)
                    {
                        estiloProp.ValorDefectoNoSeleccionable = nodo.Attributes["valorDefecto"].Value;
                    }

                    foreach (XmlNode nodoValor in GetNodos(nodo, "ValorCombo"))
                    {
                        if (!estiloProp.ListaValoresPermitidos.Contains(nodoValor.InnerText))
                        {
                            estiloProp.ListaValoresPermitidos.Add(nodoValor.InnerText);
                        }
                    }
                }
                else if (nodo.Name == "NumeroCaracteres")
                {
                    estiloProp.RestrNumCaract = new RestriccionNumCaracteres();

                    if (GetNodo(nodo, "TipoRestricion") != null)
                    {
                        estiloProp.RestrNumCaract.TipoRestricion = GetNodo(nodo, "TipoRestricion").InnerText;
                    }
                    else
                    {
                        estiloProp.RestrNumCaract.TipoRestricion = GetNodo(nodo, "TipoRestriccion").InnerText;
                    }
                    estiloProp.RestrNumCaract.Valor = int.Parse(GetNodo(nodo, "Valor").InnerText);

                    if (GetNodo(nodo, "ValorHasta") != null)
                    {
                        estiloProp.RestrNumCaract.ValorHasta = int.Parse(GetNodo(nodo, "ValorHasta").InnerText);
                    }
                }
                else if (nodo.Name == "TextoEliminarElemSel")
                {
                    estiloProp.TextoEliminarElemSel = nodo.InnerText;
                }
                else if (nodo.Name == "UsarJcrop")
                {
                    estiloProp.UsarJcrop = true;
                    estiloConfig.HayJcrop = true;

                    if (nodo.Attributes["MinSize"] != null)
                    {
                        string[] coords = nodo.Attributes["MinSize"].Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        int ancho = 0;
                        int alto = 0;

                        if (coords.Length == 2 && int.TryParse(coords[0], out ancho) && int.TryParse(coords[1], out alto) && ancho > 0 && alto > 0)
                        {
                            estiloProp.MinSizeJcrop = new KeyValuePair<int, int>(ancho, alto);
                        }
                        else
                        {
                            throw new Exception("El valor de 'MinSize' debe ser un cadena con 2 enteros mayores que 0 separados por ','");
                        }
                    }

                    if (nodo.Attributes["MaxSize"] != null)
                    {
                        string[] coords = nodo.Attributes["MaxSize"].Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        int ancho = 0;
                        int alto = 0;

                        if (coords.Length == 2 && int.TryParse(coords[0], out ancho) && int.TryParse(coords[1], out alto) && ancho > 0 && alto > 0)
                        {
                            estiloProp.MaxSizeJcrop = new KeyValuePair<int, int>(ancho, alto);
                        }
                        else
                        {
                            throw new Exception("El valor de 'MaxSize' debe ser un cadena con 2 enteros mayores que 0 separados por ','");
                        }
                    }
                }
                else if (nodo.Name == "UsarOpenSeaDragon")
                {
                    if (nodo.Attributes["PropiedadAnchoID"] != null && nodo.Attributes["PropiedadAltoID"] != null)
                    {
                        estiloProp.OpenSeaDragon = new KeyValuePair<string, string>(nodo.Attributes["PropiedadAnchoID"].Value, nodo.Attributes["PropiedadAltoID"].Value);

                        if (estiloConfig.PropiedadesOpenSeaDragon == null)
                        {
                            estiloConfig.PropiedadesOpenSeaDragon = new List<KeyValuePair<string, string>>();
                        }

                        estiloConfig.PropiedadesOpenSeaDragon.Add(new KeyValuePair<string, string>(estiloProp.NombreRealPropiedad, estiloProp.NombreEntidad));
                    }
                }
                else if (nodo.Name == "ImgMiniVP")
                {
                    estiloProp.ImagenMini = new ImagenMini();

                    foreach (XmlNode nodoValor in GetNodos(nodo, "Size"))
                    {
                        int ancho = -1;
                        int alto = -1;

                        XmlNode nodoAncho = GetNodo(nodoValor, "Ancho");
                        XmlNode nodoAlto = GetNodo(nodoValor, "Alto");

                        if (nodoAncho != null)
                        {
                            ancho = int.Parse(nodoAncho.InnerText);
                        }

                        if (nodoAlto != null)
                        {
                            alto = int.Parse(nodoAlto.InnerText);
                        }

                        while (estiloProp.ImagenMini.Tamanios.ContainsKey(ancho))
                        {
                            ancho--;
                        }

                        estiloProp.ImagenMini.Tamanios.Add(ancho, alto);

                        if (nodoValor.Attributes["Tipo"] != null)
                        {
                            estiloProp.ImagenMini.Tipo.Add(ancho, nodoValor.Attributes["Tipo"].Value);
                        }
                    }
                }
                else if (nodo.Name == "GaleriaElmentos")
                {
                    estiloProp.GaleriaImagenes = nodo.InnerText;
                }
                else if (nodo.Name == "ClaseCssTitulo")
                {
                    estiloProp.ClaseCssPanelTitulo = nodo.InnerText;
                }
                else if (nodo.Name == "TagNameTituloEdicion")
                {
                    estiloProp.TagNameTituloEdicion = nodo.InnerText;
                }
                else if (nodo.Name == "TagNameTituloLectura")
                {
                    estiloProp.TagNameTituloLectura = nodo.InnerText;
                }
                else if (nodo.Name == "AtrNombre")
                {
                    if (nodo.Attributes["xml:lang"] == null)
                    {
                        estiloProp.AtrNombre.Add("es", nodo.InnerText);
                    }
                    else if (!estiloProp.AtrNombre.ContainsKey(nodo.Attributes["xml:lang"].Value))
                    {
                        estiloProp.AtrNombre.Add(nodo.Attributes["xml:lang"].Value, nodo.InnerText);
                    }
                }
                else if (nodo.Name == "AtrNombreLectura")
                {
                    if (nodo.Attributes["xml:lang"] == null)
                    {
                        estiloProp.AtrNombreLectura.Add("es", nodo.InnerText);
                    }
                    else if (!estiloProp.AtrNombreLectura.ContainsKey(nodo.Attributes["xml:lang"].Value))
                    {
                        estiloProp.AtrNombreLectura.Add(nodo.Attributes["xml:lang"].Value, nodo.InnerText);
                    }
                }
                else if (nodo.Name == "ValoresSepComas")
                {
                    estiloProp.ValoresSepComas = true;
                }
                else if (nodo.Name == "FormatoFechaMesAño")
                {
                    estiloProp.FechaMesAnio = true;
                }
                else if (nodo.Name == "FormatoFechaLibre")
                {
                    estiloProp.FechaLibre = true;
                }
                else if (nodo.Name == "FormatoFechaConHora")
                {
                    estiloProp.FechaConHora = true;
                    estiloConfig.HayFechaConHora = true;
                }
                else if (nodo.Name == "GuardarFechaComoEntero")
                {
                    estiloProp.GuardarFechaComoEntero = true;
                }
                else if (nodo.Name == "AutoCompletar")
                {
                    string grafo = GetNodo(nodo, "Grafo").InnerText.ToLower();

                    if (!estiloConfig.GrafosSimplesAutocompletar.Contains(grafo))
                    {
                        estiloConfig.GrafosSimplesAutocompletar.Add(grafo);
                    }

                    estiloProp.GrafoAutocompletar = grafo;
                    XmlNode nodoTipoResul = GetNodo(nodo, "TipoResultado");

                    if (nodoTipoResul != null)
                    {
                        estiloProp.TipoResulAutocompletar = nodoTipoResul.InnerText;
                    }

                    if (nodo.Attributes["GuardarValores"] != null)
                    {
                        estiloProp.GuardarValoresAutocompletar = (nodo.Attributes["GuardarValores"].Value.ToLower() == "true");
                    }
                    else //Por defecto debe agregarlos para que funcione como antes:
                    {
                        estiloProp.GuardarValoresAutocompletar = true;
                    }

                    if (nodo.Attributes["PermitirNuevosValores"] != null)
                    {
                        estiloProp.NoPermitirNuevosValores = (nodo.Attributes["PermitirNuevosValores"].Value.ToLower() == "false");
                        estiloProp.GuardarValoresAutocompletar = estiloProp.GuardarValoresAutocompletar && !estiloProp.NoPermitirNuevosValores;
                    }
                }
                else if (nodo.Name == "TipoCampoLectura")
                {
                    estiloProp.TipoCampoLectura = nodo.InnerText;
                }
                else if (nodo.Name == "CampoDeshabilitado")
                {
                    estiloProp.CampoDeshabilitado = true;
                }
                else if (nodo.Name == "ClaseCss")
                {
                    estiloProp.ClaseCss = nodo.InnerText;
                }
                else if (nodo.Name == "TextoAgregarElem")
                {
                    estiloProp.TextoAgregarElem = nodo.InnerText;
                }
                else if (nodo.Name == "TextoBotonAceptarElemento")
                {
                    estiloProp.TextoBotonAceptarElemento = nodo.InnerText;
                }
                else if (nodo.Name == "VistaPrevEnEdicion")
                {
                    estiloProp.VistaPrevEnEdicion = true;
                }
                else if (nodo.Name == "ClaseCssPanel")
                {
                    estiloProp.ClaseCssPanel = nodo.InnerText;
                }
                else if (nodo.Name == "TextoCancelarElem")
                {
                    estiloProp.TextoCancelarElem = nodo.InnerText;
                }
                else if (nodo.Name == "TextoEdicionEntSel")
                {
                    estiloProp.TextoEdicionEntSel = nodo.InnerText;
                }
                else if (nodo.Name == "Microdatos")
                {
                    estiloProp.Microdatos = nodo.InnerText;
                }
                else if (nodo.Name == "Microformatos")
                {
                    foreach (XmlNode nodoMicroFor in GetChildNodes(nodo))
                    {
                        estiloProp.Microformatos.Add(nodoMicroFor.Attributes["Atributo"].Value, nodoMicroFor.InnerText);
                    }
                }
                else if (nodo.Name == "CapturarFlash")
                {
                    estiloProp.CapturarFlash = new KeyValuePair<string, string>(pNodoProp.Attributes["ID"].Value, pNodoProp.Attributes["EntidadID"].Value);
                }
                else if (nodo.Name == "HtmlObjeto")
                {
                    estiloProp.HtmlObjeto = nodo.InnerText;
                }
                else if (nodo.Name == "NuevaPestanya" && nodo.InnerText.ToLower() == "false")
                {
                    estiloProp.NuevaPestanya = false;
                }
                else if (nodo.Name == "ValorGrafoDependiente")
                {
                    estiloProp.GrafoDependiente = GetNodo(nodo, "Grafo").InnerText;
                    estiloProp.TipoEntDependiente = GetNodo(nodo, "TipoEntidad").InnerText;

                    XmlNode propDependiente = GetNodo(nodo, "PropiedadDepende");

                    if (propDependiente != null)
                    {
                        estiloProp.PropDependiente = new KeyValuePair<string, string>(propDependiente.Attributes["ID"].Value, propDependiente.Attributes["EntidadID"].Value);
                    }

                    estiloProp.TipoDependiente = "AutoCompletar";
                    XmlNode tipoGraDe = GetNodo(nodo, "Tipo");

                    if (tipoGraDe != null)
                    {
                        estiloProp.TipoDependiente = tipoGraDe.InnerText;
                    }

                    ((EstiloPlantillaConfigGen)pLIstaEstilos["[" + NodoConfigGen + "]"][0]).HayValoresGrafoDependienets = true;
                }
                else if (nodo.Name == "SeleccionEntidad")
                {
                    estiloProp.SelectorEntidad = new SelectorEntidad();
                    estiloProp.SelectorEntidad.TipoSeleccion = "Combo";

                    if (nodo.Attributes["valorDefecto"] != null)
                    {
                        estiloProp.ValorDefectoNoSeleccionable = nodo.Attributes["valorDefecto"].Value;
                    }
                    if (nodo.Attributes["Cache"] != null && !string.IsNullOrEmpty(nodo.Attributes["Cache"].Value) && nodo.Attributes["Cache"].Value.Equals("true"))
                    {
                        estiloProp.SelectorEntidad.Cache = true;
                    }

                    #region Props seleccionEntidad

                    foreach (XmlNode nodoSelecEnt in GetChildNodes(nodo))
                    {
                        if (nodoSelecEnt.Name == "UrlEntContenedora")
                        {
                            estiloProp.SelectorEntidad.UrlEntContenedora = nodoSelecEnt.InnerText;
                        }
                        else if (nodoSelecEnt.Name == "UrlPropiedad")
                        {
                            estiloProp.SelectorEntidad.UrlPropiedad = nodoSelecEnt.InnerText;
                        }
                        else if (nodoSelecEnt.Name == "UrlTipoEntSolicitada")
                        {
                            estiloProp.SelectorEntidad.UrlTipoEntSolicitada = nodoSelecEnt.InnerText;
                        }
                        else if (nodoSelecEnt.Name == "PropsEntEdicion")
                        {
                            foreach (XmlNode nodoNombrePropEdit in GetChildNodes(nodoSelecEnt))
                            {
                                if (nodoNombrePropEdit.Name == "Propiedad")
                                {
                                    EstiloPlantillaEspecifProp estiloPropEdit = ObtenerEstiloAPartirNodo(nodoNombrePropEdit, pLIstaEstilos);
                                    estiloProp.SelectorEntidad.PropiedadesEdicion.Add(ObtenerPropsJerarquitasSelEntExtEnTexto(estiloPropEdit));
                                }
                                else if (nodoNombrePropEdit.Name == "NameProp")
                                {
                                    estiloProp.SelectorEntidad.PropiedadesEdicion.Add(nodoNombrePropEdit.InnerText);
                                }
                            }
                        }
                        else if (nodoSelecEnt.Name == "PropsEntLectura")
                        {
                            foreach (XmlNode nodoPropLectura in GetChildNodes(nodoSelecEnt))
                            {
                                if (nodoPropLectura.Name == "Propiedad")
                                {
                                    EstiloPlantillaEspecifProp estiloPropPropsLect = ObtenerEstiloAPartirNodo(nodoPropLectura, pLIstaEstilos);
                                    estiloProp.SelectorEntidad.PropiedadesLectura.Add(estiloPropPropsLect);

                                    ElementoOrdenado elemOrd = new ElementoOrdenado();
                                    elemOrd.NombrePropiedad = new KeyValuePair<string, Propiedad>(estiloPropPropsLect.NombreRealPropiedad, null);
                                    ObtenerAtributosElementoNameProp(nodoPropLectura, elemOrd);
                                    estiloPropPropsLect.ElementoOrdenadoAuxiliar = elemOrd;

                                    if (elemOrd.ElementoPadre != null && elemOrd.ElementoPadre.NoEditable.HasValue)
                                    {
                                        elemOrd.NoEditable = elemOrd.ElementoPadre.NoEditable.Value;
                                    }
                                    else if (nodoPropLectura.Attributes != null && nodoPropLectura.Attributes["Editable"] != null)
                                    {
                                        elemOrd.NoEditable = (nodoPropLectura.Attributes["Editable"].Value.ToLower() == "false");
                                    }
                                }
                                else if (nodoPropLectura.Name == "Boton")
                                {
                                    EstiloPlantillaEspecifProp estiloPropPropsLect = new EstiloPlantillaEspecifProp();
                                    //estiloPropPropsLect.NombreRealPropiedad = "AuxBoton";
                                    estiloProp.SelectorEntidad.PropiedadesLectura.Add(estiloPropPropsLect);

                                    ElementoOrdenado elemOrd = new ElementoOrdenado();
                                    CompletarElemOrdNodoBoton(nodoPropLectura, elemOrd);
                                    estiloPropPropsLect.ElementoOrdenadoAuxiliar = elemOrd;
                                }
                            }
                        }
                        else if (nodoSelecEnt.Name == "TipoPresentacion")
                        {
                            estiloProp.SelectorEntidad.TipoPresentacion = nodoSelecEnt.InnerText;

                            if (nodoSelecEnt.Attributes["link"] != null)
                            {
                                ElementoOrdenado elemOrd = new ElementoOrdenado();
                                ObtenerAtributosElementoNameProp(nodoSelecEnt, elemOrd);
                                estiloProp.ElementoOrdenadoAuxiliar = elemOrd;
                            }
                        }
                        else if (nodoSelecEnt.Name == "Grafo")
                        {
                            estiloProp.SelectorEntidad.Grafo = nodoSelecEnt.InnerText;

                            if (nodoSelecEnt.Attributes["Namespace"] != null)
                            {
                                estiloProp.SelectorEntidad.NamespaceGrafo = nodoSelecEnt.Attributes["Namespace"].Value;
                            }
                        }
                        else if (nodoSelecEnt.Name == "TextoElemento0")
                        {
                            if (estiloProp.SelectorEntidad.TextoElemento0 == null)
                            {
                                estiloProp.SelectorEntidad.TextoElemento0 = "";
                            }

                            estiloProp.SelectorEntidad.TextoElemento0 += nodoSelecEnt.InnerText;

                            if (nodoSelecEnt.Attributes["xml:lang"] != null)
                            {
                                estiloProp.SelectorEntidad.TextoElemento0 += "@" + nodoSelecEnt.Attributes["xml:lang"].Value;
                            }

                            estiloProp.SelectorEntidad.TextoElemento0 += "|||";
                        }
                        else if (nodoSelecEnt.Name == "TipoSeleccion")
                        {
                            estiloProp.SelectorEntidad.TipoSeleccion = nodoSelecEnt.InnerText;

                            if (nodoSelecEnt.Attributes["MultiIdioma"] != null)
                            {
                                estiloProp.SelectorEntidad.MultiIdioma = (nodoSelecEnt.Attributes["MultiIdioma"].Value.ToLower() == "true");
                            }

                            if (estiloProp.SelectorEntidad.TipoSeleccion == "Edicion")
                            {
                                estiloConfig.HayEntidadesSeleccEditables = true;
                            }
                        }
                        else if (nodoSelecEnt.Name == "AnidamientoGnoss")
                        {
                            estiloProp.SelectorEntidad.AnidamientoGnoss = true;
                        }
                        else if (nodoSelecEnt.Name == "LinkARecurso")
                        {
                            estiloProp.SelectorEntidad.LinkARecurso = true;

                            if (!string.IsNullOrEmpty(nodoSelecEnt.InnerText))
                            {
                                if (nodoSelecEnt.InnerText.Contains(","))
                                {
                                    estiloProp.SelectorEntidad.TipoEntLinkARecurso = new List<string>(nodoSelecEnt.InnerText.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                                }
                                else
                                {
                                    estiloProp.SelectorEntidad.TipoEntLinkARecurso = new List<string>();
                                    estiloProp.SelectorEntidad.TipoEntLinkARecurso.Add(nodoSelecEnt.InnerText);
                                }
                            }

                            if (nodoSelecEnt.Attributes["PropiedadID"] != null)
                            {
                                if (nodoSelecEnt.Attributes["PropiedadID"].Value.Contains(","))
                                {
                                    estiloProp.SelectorEntidad.PropLinkARecurso = new List<string>(nodoSelecEnt.Attributes["PropiedadID"].Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                                }
                                else
                                {
                                    estiloProp.SelectorEntidad.PropLinkARecurso = new List<string>();
                                    estiloProp.SelectorEntidad.PropLinkARecurso.Add(nodoSelecEnt.Attributes["PropiedadID"].Value);
                                }
                            }

                            if (nodoSelecEnt.Attributes["IrAComunidad"] != null && nodoSelecEnt.Attributes["IrAComunidad"].Value.ToLower() == "true")
                            {
                                estiloProp.SelectorEntidad.LinkARecursoVaAComunidad = true;
                            }
                        }
                        else if (nodoSelecEnt.Name == "AtributosRecurso")
                        {
                            estiloProp.SelectorEntidad.AtributosRecurso = new List<string>();

                            foreach (XmlNode nodoPropLectura in GetChildNodes(nodoSelecEnt))
                            {
                                estiloProp.SelectorEntidad.AtributosRecurso.Add(nodoPropLectura.Name);
                            }
                        }
                        else if (nodoSelecEnt.Name == "NuevaPestanya" && nodoSelecEnt.InnerText.ToLower() == "false")
                        {
                            estiloProp.SelectorEntidad.NuevaPestanya = false;
                        }
                        else if (nodoSelecEnt.Name == "Reciproca")
                        {
                            estiloProp.SelectorEntidad.Reciproca = true;

                            if (nodoSelecEnt.Attributes["PropOrdenID"] != null)
                            {
                                if (nodoSelecEnt.Attributes["tipoOrden"] != null)
                                {
                                    estiloProp.SelectorEntidad.PropOrdenRecipocidad = new KeyValuePair<string, string>(nodoSelecEnt.Attributes["PropOrdenID"].Value, nodoSelecEnt.Attributes["tipoOrden"].Value);
                                }
                                else
                                {
                                    estiloProp.SelectorEntidad.PropOrdenRecipocidad = new KeyValuePair<string, string>(nodoSelecEnt.Attributes["PropOrdenID"].Value, "asc");
                                }
                            }

                            if (nodoSelecEnt.Attributes["PropRecipID"] != null)
                            {
                                estiloProp.SelectorEntidad.PropiedadReciproca = nodoSelecEnt.Attributes["PropRecipID"].Value;
                            }

                            if (nodoSelecEnt.Attributes["PropEdicionRecipID"] != null && nodoSelecEnt.Attributes["EntEdicionRecipID"] != null)
                            {
                                estiloProp.SelectorEntidad.PropiedadEdicionReciproca = nodoSelecEnt.Attributes["PropEdicionRecipID"].Value;
                                estiloProp.SelectorEntidad.EntidadEdicionReciproca = nodoSelecEnt.Attributes["EntEdicionRecipID"].Value;
                            }
                        }
                        else if (nodoSelecEnt.Name == "ConsultaReciproca")
                        {
                            estiloProp.SelectorEntidad.ConsultaReciproca = nodoSelecEnt.InnerText;
                        }
                        else if (nodoSelecEnt.Name == "Consulta")
                        {
                            estiloProp.SelectorEntidad.Consulta = nodoSelecEnt.InnerText;
                        }
                        else if (nodoSelecEnt.Name == "ConsultaEdicion")
                        {
                            estiloProp.SelectorEntidad.ConsultaEdicion = nodoSelecEnt.InnerText;
                        }
                        else if (nodoSelecEnt.Name == "ExtraWhereAutocompletar")
                        {
                            string extraWhere = nodoSelecEnt.InnerText;

                            if (nodoSelecEnt.Attributes["SeparadorPropPrinc"] != null)
                            {
                                extraWhere += "|||SeparadorPropPrinc=" + nodoSelecEnt.Attributes["SeparadorPropPrinc"].Value;
                            }

                            if (nodoSelecEnt.Attributes["SeparadorFinal"] != null)
                            {
                                extraWhere += "|||SeparadorFinal=" + nodoSelecEnt.Attributes["SeparadorFinal"].Value;
                            }

                            if (nodoSelecEnt.Attributes["SeparadorEntreProps"] != null)
                            {
                                extraWhere += "|||SeparadorEntreProps=" + nodoSelecEnt.Attributes["SeparadorEntreProps"].Value;
                            }

                            if (nodoSelecEnt.Attributes["Titulo"] != null)
                            {
                                extraWhere += "|||Titulo=" + nodoSelecEnt.Attributes["Titulo"].Value;
                            }

                            if (nodoSelecEnt.Attributes["Limite"] != null)
                            {
                                extraWhere += "|||Limite=" + nodoSelecEnt.Attributes["Limite"].Value;
                            }

                            if (estiloProp.SelectorEntidad.ExtraWhereAutocompletar == null)
                            {
                                estiloProp.SelectorEntidad.ExtraWhereAutocompletar = extraWhere;
                            }
                            else
                            {
                                if (estiloProp.SelectorEntidad.ExtraWhereAutocompletarExtras == null)
                                {
                                    estiloProp.SelectorEntidad.ExtraWhereAutocompletarExtras = new List<string>();
                                }

                                estiloProp.SelectorEntidad.ExtraWhereAutocompletarExtras.Add(extraWhere);
                            }
                        }
                        else if (nodoSelecEnt.Name == "MensajeNoResultados")
                        {
                            estiloProp.SelectorEntidad.MensajeNoResultados = nodoSelecEnt.InnerText;
                        }
                        else if (nodoSelecEnt.Name == "PaginarElementos" && nodoSelecEnt.Attributes["NumElemPorPag"] != null)
                        {
                            int resul = 0;

                            if (int.TryParse(nodoSelecEnt.Attributes["NumElemPorPag"].Value, out resul))
                            {
                                estiloProp.SelectorEntidad.NumElemPorPag = resul;
                            }

                            if (nodoSelecEnt.Attributes["VistaPersonalizada"] != null)
                            {
                                estiloProp.SelectorEntidad.VistaPersonalizadaPaginacion = nodoSelecEnt.Attributes["VistaPersonalizada"].Value;
                            }
                        }
                        else if (nodoSelecEnt.Name == "PropiedadDepende" && nodoSelecEnt.Attributes["ID"] != null && nodoSelecEnt.Attributes["EntidadID"] != null)
                        {
                            XmlNode nodoConsulta = GetNodo(nodoSelecEnt, "ConsultaDependiente");

                            if (nodoConsulta != null)
                            {
                                estiloProp.SelectorEntidad.ConsultaDependiente = nodoConsulta.InnerText;

                                if (estiloConfig.PropsSelecEntDependientes == null)
                                {
                                    estiloConfig.PropsSelecEntDependientes = new Dictionary<KeyValuePair<string, string>, List<KeyValuePair<string, string>>>();
                                }

                                KeyValuePair<string, string> clave = new KeyValuePair<string, string>(nodoSelecEnt.Attributes["ID"].Value, nodoSelecEnt.Attributes["EntidadID"].Value);
                                estiloProp.SelectorEntidad.PropiedadDeLaQueDepende = clave;

                                if (!estiloConfig.PropsSelecEntDependientes.ContainsKey(clave))
                                {
                                    estiloConfig.PropsSelecEntDependientes.Add(clave, new List<KeyValuePair<string, string>>());
                                }

                                estiloConfig.PropsSelecEntDependientes[clave].Add(new KeyValuePair<string, string>(estiloProp.NombreRealPropiedad, estiloProp.NombreEntidad));
                            }
                        }
                        else if (nodoSelecEnt.Name == "SoloIdiomaUsuario")
                        {
                            estiloProp.SelectorEntidad.SoloIdiomaUsuario = true;
                        }
                    }

                    #endregion

                    #region Tesauro Semántico

                    //IMPORTANTE: Si se cambia el orden de las propiedades hay que buscar y cambiar las apariciones de .PropiedadesEdicion[2] por ejemplo.

                    if (estiloProp.SelectorEntidad.TipoSeleccion == "Tesauro")
                    {
                        estiloConfig.PropiedadesTesSem.Add(new KeyValuePair<string, string>(estiloProp.NombreRealPropiedad, estiloProp.NombreEntidad));

                        if (estiloProp.SelectorEntidad.UrlTipoEntSolicitada == null)//Tipo del tesauro
                        {
                            estiloProp.SelectorEntidad.UrlTipoEntSolicitada = EstiloPlantilla.Collection_TesSem;
                        }

                        if (estiloProp.SelectorEntidad.PropiedadesEdicion.Count == 0)//source
                        {
                            estiloProp.SelectorEntidad.PropiedadesEdicion.Add(EstiloPlantilla.Source_TesSem);
                        }

                        if (estiloProp.SelectorEntidad.PropiedadesEdicion.Count == 1)//Vinculación Tesauro-Categoría
                        {
                            estiloProp.SelectorEntidad.PropiedadesEdicion.Add(EstiloPlantilla.Member_TesSem);
                        }

                        if (estiloProp.SelectorEntidad.PropiedadesEdicion.Count == 2)//id
                        {
                            estiloProp.SelectorEntidad.PropiedadesEdicion.Add(EstiloPlantilla.Identifier_TesSem);
                        }

                        if (estiloProp.SelectorEntidad.PropiedadesEdicion.Count == 3)//nombre categoría
                        {
                            estiloProp.SelectorEntidad.PropiedadesEdicion.Add(EstiloPlantilla.PrefLabel_TesSem);
                        }

                        if (estiloProp.SelectorEntidad.PropiedadesEdicion.Count == 4)//hasPadre
                        {
                            estiloProp.SelectorEntidad.PropiedadesEdicion.Add(EstiloPlantilla.Broader_TesSem);
                        }

                        if (estiloProp.SelectorEntidad.PropiedadesEdicion.Count == 5)//hasHijo
                        {
                            estiloProp.SelectorEntidad.PropiedadesEdicion.Add(EstiloPlantilla.Narrower_TesSem);
                        }

                        estiloProp.SelectorEntidad.PropiedadesLectura = new List<EstiloPlantillaEspecifProp>();

                        EstiloPlantillaEspecifProp estiloPropTes = new EstiloPlantillaEspecifProp();
                        estiloPropTes.NombreRealPropiedad = estiloProp.SelectorEntidad.PropiedadesEdicion[2];
                        estiloPropTes.NombreEntidad = EstiloPlantilla.Concept_TesSem;
                        estiloProp.SelectorEntidad.PropiedadesLectura.Add(estiloPropTes);

                        estiloPropTes = new EstiloPlantillaEspecifProp();
                        estiloPropTes.NombreRealPropiedad = estiloProp.SelectorEntidad.PropiedadesEdicion[3];
                        estiloPropTes.NombreEntidad = EstiloPlantilla.Concept_TesSem;
                        estiloProp.SelectorEntidad.PropiedadesLectura.Add(estiloPropTes);

                        estiloPropTes = new EstiloPlantillaEspecifProp();
                        estiloPropTes.NombreRealPropiedad = estiloProp.SelectorEntidad.PropiedadesEdicion[4];
                        estiloPropTes.NombreEntidad = EstiloPlantilla.Concept_TesSem;
                        estiloProp.SelectorEntidad.PropiedadesLectura.Add(estiloPropTes);

                        estiloPropTes = new EstiloPlantillaEspecifProp();
                        estiloPropTes.NombreRealPropiedad = estiloProp.SelectorEntidad.PropiedadesEdicion[5];
                        estiloPropTes.NombreEntidad = EstiloPlantilla.Concept_TesSem;
                        estiloProp.SelectorEntidad.PropiedadesLectura.Add(estiloPropTes);

                        estiloPropTes = new EstiloPlantillaEspecifProp();
                        estiloPropTes.NombreRealPropiedad = EstiloPlantilla.Symbol_TesSem;
                        estiloPropTes.NombreEntidad = EstiloPlantilla.Concept_TesSem;
                        estiloProp.SelectorEntidad.PropiedadesLectura.Add(estiloPropTes);
                    }

                    #endregion

                    ((EstiloPlantillaConfigGen)pLIstaEstilos["[" + NodoConfigGen + "]"][0]).HayEntidadesSelecc = true;
                }
                else if (nodo.Name == "Propiedad")
                {
                    if (estiloProp.PropiedadesAuxiliares == null)
                    {
                        estiloProp.PropiedadesAuxiliares = new List<EstiloPlantillaEspecifProp>();
                    }

                    estiloProp.PropiedadesAuxiliares.Add(ObtenerEstiloAPartirNodo(nodo, pLIstaEstilos));
                }
                else if (nodo.Name == "ValorDefecto")
                {
                    estiloProp.ValorPorDefecto = nodo.InnerText;
                }
                else if (nodo.Name == "ExpresionRegular")
                {
                    estiloProp.ExpresionRegular = nodo.InnerText;
                    estiloProp.PrimeraCoincidenciaExpresionRegular = (nodo.Attributes["PrimeraCoincidencia"] != null && nodo.Attributes["PrimeraCoincidencia"].Value.ToLower() == "true");
                }
                else if (nodo.Name == "AvisoValorRepetido")
                {
                    KeyValuePair<string, string> claveAviso = new KeyValuePair<string, string>(estiloProp.NombreRealPropiedad, estiloProp.NombreEntidad);

                    if (!estiloConfig.PropsComprobarRepeticion.ContainsKey(claveAviso))
                    {
                        bool impedirCont = (nodo.Attributes["ImpedirContinuar"] != null && nodo.Attributes["ImpedirContinuar"].Value.ToLower() == "true");
                        short tipo = 0;

                        if (nodo.Attributes["ComprobarSoloEnComunidad"] != null && nodo.Attributes["ComprobarSoloEnComunidad"].Value.ToLower() == "true")
                        {
                            tipo = 1;
                        }

                        estiloConfig.PropsComprobarRepeticion.Add(claveAviso, new KeyValuePair<bool, short>(impedirCont, tipo));
                    }
                }
                else if (nodo.Name == "MultiIdioma" && nodo.InnerText.ToLower() == "false")
                {
                    estiloProp.NoMultiIdioma = true;
                }
                else if (nodo.Name == "AtrPrivadoParaMiembrosComunidad")
                {
                    estiloProp.PrivadoPrivadoParaMiembrosComunidad = true;
                }
                else if (nodo.Name == "AtrPrivadoParaGrupoEditores")
                {
                    string[] delimiter = { "|" };
                    List<string> listaNombresCortosGrupos = new List<string>(nodo.InnerText.Split(delimiter, StringSplitOptions.RemoveEmptyEntries));

                    IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, null);
                    estiloProp.PrivadoParaGrupoEditores = identCN.ObtenerGruposIDPorNombreCortoEnProyectoYEnOrganizacion(listaNombresCortosGrupos, mProyectoID);

                    identCN.Dispose();
                }
                else if (nodo.Name == "PaginarElementos" && nodo.Attributes["NumElemPorPag"] != null)
                {
                    int resul = 0;

                    if (int.TryParse(nodo.Attributes["NumElemPorPag"].Value, out resul))
                    {
                        estiloProp.NumElemPorPag = resul;
                    }

                    if (nodo.Attributes["VistaPersonalizada"] != null)
                    {
                        estiloProp.VistaPersonalizadaPaginacion = nodo.Attributes["VistaPersonalizada"].Value;
                    }
                }
            }

            return estiloProp;
        }

        /// <summary>
        /// Obtiene una acción a partir de un nodo de acción.
        /// </summary>
        /// <param name="pNodoAccion">Nodo con la acción</param>
        /// <returns>Acción a partir de un nodo de acción</returns>
        private AccionSemCms ObtenerAccionDeNodo(XmlNode pNodoAccion)
        {
            if (pNodoAccion.Attributes["ID"] == null)
            {
                return null;
            }

            AccionSemCms accion = new AccionSemCms();
            accion.ID = pNodoAccion.Attributes["ID"].Value;
            accion.Acciones = new List<AccionSemCms.Accion>();

            foreach (XmlNode nodoAcc in GetChildNodes(pNodoAccion))
            {
                if (nodoAcc.Name == "EnviarInfoServicioExterno")
                {
                    XmlNode nodoUrlServExt = GetNodo(nodoAcc, "UrlServicio");

                    if (nodoUrlServExt != null && !string.IsNullOrEmpty(nodoUrlServExt.InnerText))
                    {
                        AccionSemCms.Accion accionInt = new AccionSemCms.Accion();
                        accionInt.TipoAccion = AccionSemCms.TipoAccion.EnviarServExterno;
                        accionInt.UrlServExterno = nodoUrlServExt.InnerText;
                        accion.Acciones.Add(accionInt);

                        if (nodoUrlServExt.Attributes["Prefijo"] != null)
                        {
                            accionInt.UrlServExterno = nodoUrlServExt.Attributes["Prefijo"].Value + "@pref@" + accionInt.UrlServExterno;
                        }
                    }
                }
            }

            if (accion.Acciones.Count == 0)
            {
                return null;
            }

            return accion;
        }

        /// <summary>
        /// Obtiene una condición a partir de un nodo de condición.
        /// </summary>
        /// <param name="pNodoCondicion">Nodo con la condición</param>
        /// <returns>Condición a partir de un nodo de condición</returns>
        private CondicionSemCms ObtenerCondicionDeNodo(XmlNode pNodoCondicion)
        {
            if (pNodoCondicion.Attributes["ID"] == null)
            {
                return null;
            }

            CondicionSemCms condicion = new CondicionSemCms();
            condicion.ID = pNodoCondicion.Attributes["ID"].Value;
            condicion.VariablesProp = ObtenerVariablesProp(pNodoCondicion);

            List<CondicionSemCms.ClausulaSemCms> clausulas = ObtenerClausulaDeNodo(pNodoCondicion);

            if (clausulas.Count == 0)
            {
                return null;
            }

            condicion.Clausula = clausulas[0];

            return condicion;
        }

        /// <summary>
        /// Obtiene la clausula de un nodo.
        /// </summary>
        /// <param name="pNodo">Nodo</param>
        /// <returns>Clausula de un nodo</returns>
        private List<CondicionSemCms.ClausulaSemCms> ObtenerClausulaDeNodo(XmlNode pNodo)
        {
            List<CondicionSemCms.ClausulaSemCms> clausulas = new List<CondicionSemCms.ClausulaSemCms>();

            foreach (XmlNode nodoClau in GetNodos(pNodo, "Clausula"))
            {
                CondicionSemCms.ClausulaSemCms clausula = new CondicionSemCms.ClausulaSemCms();

                if (nodoClau.Attributes["Tipo"] == null)
                {
                    continue;
                }

                clausula.Tipo = nodoClau.Attributes["Tipo"].Value;
                clausula.Clausulas = ObtenerClausulaDeNodo(nodoClau);

                if (clausula.Clausulas.Count == 0)
                {
                    clausula.Valores = new List<string>();

                    foreach (XmlNode nodoValor in GetNodos(nodoClau, "Valor"))
                    {
                        if (!string.IsNullOrEmpty(nodoValor.InnerText))
                        {
                            clausula.Valores.Add(nodoValor.InnerText);
                        }
                    }

                    if (clausula.Valores.Count == 0)
                    {
                        continue;
                    }

                    if (clausula.Tipo == "PerteneceAGrupo")
                    {
                        IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, null);
                        List<Guid> iDsGrupos = identCN.ObtenerGruposIDPorNombreCortoEnProyectoYEnOrganizacion(clausula.Valores, mProyectoID);
                        identCN.Dispose();
                        clausula.Valores.Clear();

                        foreach (Guid grupoID in iDsGrupos)
                        {
                            clausula.Valores.Add(grupoID.ToString());
                        }
                    }
                }

                clausulas.Add(clausula);
            }

            return clausulas;
        }

        /// <summary>
        /// Obtiene las variables declaradas para la condición.
        /// </summary>
        /// <param name="pNodo">Nodo con las variables</param>
        /// <returns>Variables declaradas para la condición</returns>
        private Dictionary<string, string> ObtenerVariablesProp(XmlNode pNodo)
        {
            Dictionary<string, string> variables = new Dictionary<string, string>();
            List<XmlNode> nodosSelVal = GetNodos(pNodo, "SeleccionarValorPropiedad");

            if (nodosSelVal.Count == 0)
            {
                variables.Add(pNodo.InnerText, null);
            }
            else
            {
                foreach (XmlNode nodoSelVal in nodosSelVal)
                {
                    if (nodoSelVal.Attributes["ID"] == null || nodoSelVal.Attributes["EntidadID"] == null)
                    {
                        return null;
                    }

                    string propEnt = nodoSelVal.Attributes["ID"].Value + "," + nodoSelVal.Attributes["EntidadID"].Value;
                    Dictionary<string, string> variablesAux = ObtenerVariablesProp(nodoSelVal);

                    if (variablesAux == null)
                    {
                        return null;
                    }

                    foreach (string key in variablesAux.Keys)
                    {
                        if (variablesAux[key] == null)
                        {
                            variables.Add(key, propEnt);
                        }
                        else
                        {
                            variables.Add(key, propEnt + "|" + variablesAux[key]);
                        }

                        break;
                    }
                }
            }

            return variables;
        }

        /// <summary>
        /// Obtiene una linea con las propiedades jerarquicas y de selección entidad que hay en un estilo de propiedad.
        /// </summary>
        /// <param name="pEstiloProp">Estilo prop</param>
        /// <returns>Linea con las propiedades jerarquicas y de selección entidad que hay en un estilo de propiedad</returns>
        private string ObtenerPropsJerarquitasSelEntExtEnTexto(EstiloPlantillaEspecifProp pEstiloProp)
        {
            string prop = pEstiloProp.NombreRealPropiedad;

            if (pEstiloProp.SelectorEntidad != null)
            {
                prop += "(" + pEstiloProp.SelectorEntidad.Grafo;

                foreach (string propEdit in pEstiloProp.SelectorEntidad.PropiedadesEdicion)
                {
                    prop += ";" + propEdit;
                }

                prop += ")";
            }
            else if (pEstiloProp.PropiedadesAuxiliares != null && pEstiloProp.PropiedadesAuxiliares.Count > 0)
            {
                prop += "|";

                foreach (EstiloPlantillaEspecifProp estiloHija in pEstiloProp.PropiedadesAuxiliares)
                {
                    prop += ObtenerPropsJerarquitasSelEntExtEnTexto(estiloHija) + ";";
                }

                prop = prop.Substring(0, prop.Length - 1);
            }

            return prop;
        }

        /// <summary>
        /// Obtiene un nodo de una raíz a partir de su nombre.
        /// </summary>
        /// <param name="pRaiz">Documento xml raíz</param>
        /// <param name="pNodo">Nombre del nodo</param>
        /// <returns>Nodo de una raíz a partir de su nombre</returns>
        private XmlNode GetNodo(XmlDocument pRaiz, string pNodo)
        {
            return GetNodo(pRaiz.SelectNodes(pNodo));
        }

        /// <summary>
        /// Obtiene un nodo de una raíz a partir de su nombre.
        /// </summary>
        /// <param name="pRaiz">Nodo xml raíz</param>
        /// <param name="pNodo">Nombre del nodo</param>
        /// <returns>Nodo de una raíz a partir de su nombre</returns>
        private XmlNode GetNodo(XmlNode pRaiz, string pNodo)
        {
            return GetNodo(pRaiz.SelectNodes(pNodo));
        }

        /// <summary>
        /// Obtiene un nodo de una lista de nodos a partir de su nombre.
        /// </summary>
        /// <param name="pListaNodos">Lista nodos xml para buscar</param>
        /// <returns>Nodo de una lista de nodos a partir de su nombre</returns>
        private XmlNode GetNodo(XmlNodeList pListaNodos)
        {
            XmlNode nodoSinProyecto = null;

            foreach (XmlNode nodo in pListaNodos)
            {
                if (nodo.NodeType == XmlNodeType.Comment)
                {
                    continue;
                }

                if (nodo.Attributes == null || (nodo.Attributes["ProyectoID"] == null && nodo.Attributes["Proyecto"] == null && nodo.Attributes["ParametroSemCms"] == null))
                {
                    nodoSinProyecto = nodo;
                }
                else if (nodo.Attributes["ParametroSemCms"] != null && !string.IsNullOrEmpty(ParametroSemCms) && nodo.Attributes["ParametroSemCms"].Value == ParametroSemCms)
                {
                    return nodo;
                }
                else
                {
                    if (nodo.Attributes["ProyectoID"] != null)
                    {
                        Guid proyID = Guid.Empty;
                        if (Guid.TryParse(nodo.Attributes["ProyectoID"].Value, out proyID) && proyID == mProyectoID)
                        {
                            return nodo;
                        }
                    }
                    else if (nodo.Attributes["Proyecto"] != null && nodo.Attributes["Proyecto"].Value == mNombreCortoProy)
                    {
                        return nodo;
                    }
                }
            }

            return nodoSinProyecto;
        }

        /// <summary>
        /// Obtiene los nodos de una raíz a partir de su nombre.
        /// </summary>
        /// <param name="pRaiz">Nodo xml raíz</param>
        /// <param name="pNodo">Nombre del nodo</param>
        /// <returns>Nodos de una raíz a partir de su nombre</returns>
        private List<XmlNode> GetNodos(XmlNode pRaiz, string pNodo)
        {
            XmlNodeList nodos = pRaiz.SelectNodes(pNodo);
            return GetNodos(nodos);
        }

        /// <summary>
        /// Obtiene los nodos de una raíz a partir de su nombre.
        /// </summary>
        /// <param name="pRaiz">Nodo xml raíz</param>
        /// <returns>Nodos de una raíz a partir de su nombre</returns>
        private List<XmlNode> GetChildNodes(XmlNode pRaiz)
        {
            XmlNodeList nodos = pRaiz.ChildNodes;
            return GetNodos(nodos);
        }

        /// <summary>
        /// Obtiene los nodos de una lista de nodos a partir de su nombre.
        /// </summary>
        /// <param name="pNodos">Nodos xml sobre los que buscar</param>
        /// <returns>Nodos de una lista de nodos a partir de su nombre</returns>
        private List<XmlNode> GetNodos(XmlNodeList pNodos)
        {
            List<XmlNode> nodosSinAlterar = new List<XmlNode>();
            Dictionary<string, List<XmlNode>> nodosSinProy = new Dictionary<string, List<XmlNode>>();
            Dictionary<string, List<XmlNode>> nodosConProy = new Dictionary<string, List<XmlNode>>();
            Dictionary<string, List<XmlNode>> nodosConParam = new Dictionary<string, List<XmlNode>>();

            foreach (XmlNode nodo in pNodos)
            {
                if (nodo.NodeType == XmlNodeType.Comment)
                {
                    continue;
                }

                Dictionary<string, List<XmlNode>> nodosAgregar = null;

                if (nodo.Attributes == null || (nodo.Attributes["ProyectoID"] == null && nodo.Attributes["Proyecto"] == null && nodo.Attributes["ParametroSemCms"] == null))
                {
                    nodosAgregar = nodosSinProy;
                }
                else if (nodo.Attributes["ParametroSemCms"] != null && !string.IsNullOrEmpty(ParametroSemCms) && nodo.Attributes["ParametroSemCms"].Value == ParametroSemCms)
                {
                    nodosAgregar = nodosConParam;
                }
                else
                {
                    if (nodo.Attributes["ProyectoID"] != null)
                    {
                        Guid proyID = Guid.Empty;
                        if (Guid.TryParse(nodo.Attributes["ProyectoID"].Value, out proyID) && proyID == mProyectoID)
                        {
                            nodosAgregar = nodosConProy;
                        }
                    }
                    else if (nodo.Attributes["Proyecto"] != null && nodo.Attributes["Proyecto"].Value == mNombreCortoProy)
                    {
                        nodosAgregar = nodosConProy;
                    }
                }

                if (nodosAgregar != null)
                {
                    if (!nodosAgregar.ContainsKey(nodo.Name))
                    {
                        nodosAgregar.Add(nodo.Name, new List<XmlNode>());
                    }

                    nodosAgregar[nodo.Name].Add(nodo);

                    nodosSinAlterar.Add(nodo);
                }
            }

            if (nodosConProy.Count == 0 && nodosConParam.Count == 0)
            {
                return nodosSinAlterar;
            }

            foreach (string nombre in nodosConParam.Keys)
            {
                if (nodosSinProy.ContainsKey(nombre))
                {
                    nodosSinProy[nombre] = nodosConParam[nombre];
                }
                else
                {
                    nodosSinProy.Add(nombre, nodosConParam[nombre]);
                }
            }

            foreach (string nombre in nodosConProy.Keys)
            {
                if (nodosSinProy.ContainsKey(nombre))
                {
                    nodosSinProy[nombre] = nodosConProy[nombre];
                }
                else
                {
                    nodosSinProy.Add(nombre, nodosConProy[nombre]);
                }
            }

            List<XmlNode> nodosFinal = new List<XmlNode>();

            foreach (List<XmlNode> listaNodos in nodosSinProy.Values)
            {
                nodosFinal.AddRange(listaNodos);
            }

            return nodosFinal;
        }

        /// <summary>
        /// Genera un archivo de configuración estandar, para una plantilla.
        /// </summary>
        /// <param name="pNamespaceOnto">Namespace ontología</param>
        /// <param name="pOntologia">Ontología</param>
        /// <param name="pOntologiaSecundaria">Indica si es una ontología secundaria</param>
        /// <returns>Array de bytes del fichero de configuración generado</returns>
        public static byte[] GenerarArchivoConfiguracionEstandar(string pNamespaceOnto, Ontologia pOntologia, bool pOntologiaSecundaria)
        {
            string tabulacion = ConstantesArchivoConfiguracion.Tabulacion;

            MemoryStream archMemoria = new MemoryStream();
            StreamWriter archivo = new StreamWriter(archMemoria);
            List<ElementoOntologia> listaEntidadesPrincipales = pOntologia.ObtenerElementosContenedorSuperior();

            if (listaEntidadesPrincipales == null || listaEntidadesPrincipales.Count == 0)
            {
                throw new ExcepcionGeneral("No existe ninguna entidad principal en la ontología. La entidad principal de la ontología no debe estar en el rango de ninguna propiedad");
            }

            string tipoEntidad = listaEntidadesPrincipales[0].TipoEntidad;

            archivo.WriteLine("<config>");
            archivo.WriteLine(tabulacion + "<ConfiguracionGeneral>");
            archivo.WriteLine(tabulacion + tabulacion + "<namespace>" + pNamespaceOnto + "</namespace>");
            archivo.WriteLine(tabulacion + tabulacion + "<idiomasOnto>");
            archivo.WriteLine(tabulacion + tabulacion + tabulacion + "<idiomaOnto>es</idiomaOnto>");
            archivo.WriteLine(tabulacion + tabulacion + "</idiomasOnto>");

            if (!pOntologiaSecundaria)
            {
                archivo.WriteLine(tabulacion + tabulacion + "<ocultarTituloDescpImgDoc></ocultarTituloDescpImgDoc>");
            }

            archivo.WriteLine(tabulacion + tabulacion + "<TituloDoc EntidadID=\"" + tipoEntidad + "\">@@@TITULO</TituloDoc>");

            if (!pOntologiaSecundaria)
            {
                archivo.WriteLine(tabulacion + tabulacion + "<DescripcionDoc EntidadID=\"" + tipoEntidad + "\">@@@DESCRIPCIÓN</DescripcionDoc>");
            }

            archivo.WriteLine(tabulacion + tabulacion + "<HtmlNuevo>true</HtmlNuevo>");
            archivo.WriteLine(tabulacion + "</ConfiguracionGeneral>");
            archivo.WriteLine(tabulacion + "<EspefPropiedad>");

            string nombre = string.Empty;
            string id = string.Empty;

            //recorrer propiedades
            foreach (ElementoOntologia onto in pOntologia.Entidades)
            {
                foreach (Propiedad propiedad in onto.Propiedades)
                {
                    nombre = propiedad.Nombre;
                    id = onto.TipoEntidad;

                    if (propiedad.LabelIdioma != null && propiedad.LabelIdioma.Keys.Count > 0)
                    {
                        //apertura nodo propiedad
                        archivo.WriteLine(tabulacion + tabulacion + "<Propiedad  ID=\"" + nombre + "\" EntidadID=\"" + id + "\">");

                        foreach (string idioma in propiedad.LabelIdioma.Keys)
                        {
                            archivo.WriteLine(tabulacion + tabulacion + tabulacion + "<AtrNombre xml:lang=\"" + idioma + "\">" + propiedad.LabelIdioma[idioma] + "</AtrNombre>");
                            archivo.WriteLine(tabulacion + tabulacion + tabulacion + "<AtrNombreLectura xml:lang=\"" + idioma + "\">" + propiedad.LabelIdioma[idioma] + "</AtrNombreLectura>");
                        }

                        //cierre nodo propiedad
                        archivo.WriteLine(tabulacion + tabulacion + "</Propiedad>");
                    }
                    else
                    {
                        string label = string.Empty;
                        if (!string.IsNullOrEmpty(propiedad.Label))
                        {
                            label = propiedad.Label;
                        }
                        else
                        {
                            label = nombre;
                        }

                        archivo.WriteLine(tabulacion + tabulacion + "<Propiedad  ID=\"" + nombre + "\" EntidadID=\"" + id + "\">");
                        archivo.WriteLine(tabulacion + tabulacion + tabulacion + "<AtrNombre>" + label + "</AtrNombre>");
                        archivo.WriteLine(tabulacion + tabulacion + tabulacion + "<AtrNombreLectura>" + label + "</AtrNombreLectura>");
                        archivo.WriteLine(tabulacion + tabulacion + "</Propiedad>");
                    }
                }
            }

            archivo.WriteLine(tabulacion + "</EspefPropiedad>");
            archivo.WriteLine(tabulacion + "<EspefEntidad>");

            foreach (ElementoOntologia onto in pOntologia.Entidades)
            {
                id = onto.TipoEntidad;

                string label = string.Empty;
                if (!string.IsNullOrEmpty(onto.Label))
                {
                    label = onto.Label;
                }
                else
                {
                    label = id;
                }

                if (onto.Propiedades.Count > 0)
                {

                    archivo.WriteLine(tabulacion + tabulacion + "<Entidad  ID=\"" + id + "\">");
                    archivo.WriteLine(tabulacion + tabulacion + tabulacion + "<AtrNombre>" + label + "</AtrNombre>");
                    archivo.WriteLine(tabulacion + tabulacion + tabulacion + "<AtrNombreLectura>" + label + "</AtrNombreLectura>");
                    archivo.WriteLine(tabulacion + tabulacion + tabulacion + "<OrdenEntidad>");
                    archivo.WriteLine(tabulacion + tabulacion + tabulacion + tabulacion + "<Grupo class=\"formtheme02\">");
                    archivo.WriteLine(tabulacion + tabulacion + tabulacion + tabulacion + tabulacion + "<Grupo class=\"fieldset\">");
                    archivo.WriteLine(tabulacion + tabulacion + tabulacion + tabulacion + tabulacion + tabulacion + "<Grupo Tipo=\"fieldset\" class=\"mediumLabels\">");

                    string textoProp = string.Empty;
                    foreach (Propiedad propiedad in onto.Propiedades)
                    {
                        nombre = propiedad.Nombre;
                        textoProp += tabulacion + tabulacion + tabulacion + tabulacion + tabulacion + tabulacion + tabulacion + "<NameProp>" + nombre + "</NameProp> \n";
                    }

                    textoProp = textoProp.Substring(0, textoProp.LastIndexOf("\n") - 1);
                    archivo.WriteLine(textoProp);
                    archivo.WriteLine(tabulacion + tabulacion + tabulacion + tabulacion + tabulacion + tabulacion + "</Grupo>");
                    archivo.WriteLine(tabulacion + tabulacion + tabulacion + tabulacion + tabulacion + "</Grupo>");
                    archivo.WriteLine(tabulacion + tabulacion + tabulacion + tabulacion + "</Grupo>");
                    archivo.WriteLine(tabulacion + tabulacion + tabulacion + "</OrdenEntidad>");

                    if (!pOntologiaSecundaria)
                    {
                        archivo.WriteLine(tabulacion + tabulacion + tabulacion + "<OrdenEntidadLectura>");
                        archivo.WriteLine(tabulacion + tabulacion + tabulacion + tabulacion + "<Grupo classLectura=\"contentGroup contenidoPrincipal\">");
                        archivo.WriteLine(tabulacion + tabulacion + tabulacion + tabulacion + tabulacion + "<Grupo classLectura=\"group title\">");
                        archivo.WriteLine(tabulacion + tabulacion + tabulacion + tabulacion + tabulacion + tabulacion + "<Grupo Tipo=\"titulo\">");
                        archivo.WriteLine(tabulacion + tabulacion + tabulacion + tabulacion + tabulacion + tabulacion + tabulacion + "<NameProp>@@@TITULO</NameProp>");
                        archivo.WriteLine(tabulacion + tabulacion + tabulacion + tabulacion + tabulacion + tabulacion + "</Grupo>");
                        archivo.WriteLine(tabulacion + tabulacion + tabulacion + tabulacion + tabulacion + tabulacion + "<Grupo Tipo=\"subtitulo\">");
                        archivo.WriteLine(tabulacion + tabulacion + tabulacion + tabulacion + tabulacion + tabulacion + tabulacion + "<NameProp>@@@SUBTITULO</NameProp>");
                        archivo.WriteLine(tabulacion + tabulacion + tabulacion + tabulacion + tabulacion + tabulacion + "</Grupo>");
                        archivo.WriteLine(tabulacion + tabulacion + tabulacion + tabulacion + tabulacion + "</Grupo>");
                        archivo.WriteLine(tabulacion + tabulacion + tabulacion + tabulacion + "</Grupo>");
                        archivo.WriteLine(tabulacion + tabulacion + tabulacion + tabulacion + "<Grupo classLectura=\"group content semanticView\">");
                        archivo.WriteLine(tabulacion + tabulacion + tabulacion + tabulacion + tabulacion + "<Grupo classLectura=\"group group_info\">");
                        archivo.WriteLine(tabulacion + tabulacion + tabulacion + tabulacion + tabulacion + tabulacion + "<AtrNombreGrupo>Propiedades</AtrNombreGrupo>");
                        archivo.WriteLine(tabulacion + tabulacion + tabulacion + tabulacion + tabulacion + tabulacion + "<Grupo classLectura=\"contentGroup\">");
                        archivo.WriteLine(textoProp);
                        archivo.WriteLine(tabulacion + tabulacion + tabulacion + tabulacion + tabulacion + tabulacion + "</Grupo>");
                        archivo.WriteLine(tabulacion + tabulacion + tabulacion + tabulacion + tabulacion + "</Grupo>");
                        archivo.WriteLine(tabulacion + tabulacion + tabulacion + tabulacion + "</Grupo>");
                        archivo.WriteLine(tabulacion + tabulacion + tabulacion + "</OrdenEntidadLectura>");
                    }

                    archivo.WriteLine(tabulacion + tabulacion + "</Entidad>");
                }
            }

            archivo.WriteLine(tabulacion + "</EspefEntidad>");
            archivo.WriteLine("</config>");

            archivo.Flush();

            byte[] bytes = archMemoria.ToArray();

            archivo.Close();
            archivo.Dispose();
            archivo = null;

            return bytes;
        }

        #endregion
    }
}
