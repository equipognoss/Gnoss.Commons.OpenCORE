using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.BASE_BD;
using Es.Riam.Gnoss.AD.BASE_BD.Model;
using Es.Riam.Gnoss.AD.Documentacion;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.BASE;
using Es.Riam.Gnoss.AD.EntityModel.Models.Faceta;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Facetado;
using Es.Riam.Gnoss.AD.Facetado.Model;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.Documentacion;
using Es.Riam.Gnoss.CL.Facetado;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.Documentacion;
using Es.Riam.Gnoss.Elementos.Facetado;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Elementos.Tesauro;
using Es.Riam.Gnoss.Logica.BASE_BD;
using Es.Riam.Gnoss.Logica.Documentacion;
using Es.Riam.Gnoss.Logica.Facetado;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.RabbitMQ;
using Es.Riam.Gnoss.Recursos;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.UtilServiciosWeb;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Semantica.OWL;
using Es.Riam.Util;
using Es.Riam.Util.AnalisisSintactico;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;
using CallFileService = Es.Riam.Gnoss.UtilServiciosWeb.CallFileService;

namespace Es.Riam.Gnoss.Servicios
{
    public class UtilidadesVirtuoso
    {

        #region Constantes

        private const string LETRAS_CON_ACENTOS = "âáàäêéèëîíìïôóòöûúüùñÂÁÀÄÊÉÈËÎÍÌÏÔÓÒÖÛÚÙÜÑçÇ";
        private const string LETRAS_SIN_ACENTOS = "aaaaeeeeiiiioooouuuunAAAAEEEEIIIIOOOOUUUUNcC";
        private const string SIGNOS_ELIMINAR_SEARCH = ",.;¿?!¡:";

        public const string PROPIEDADES_ONTOLOGIA_FECHA = "Fecha";
        public const string PROPIEDADES_ONTOLOGIA_NUMERO = "Numero";
        public static ConcurrentDictionary<Guid, DateTime> mProyectosActualizarContadores;

        #endregion

        readonly LoggingService mLoggingService;
        private EntityContext mEntityContext;
        private ConfigService mConfigService;
        private RedisCacheWrapper mRedisCacheWrapper;
        private EntityContextBASE mEntityContextBase;
        private VirtuosoAD mVirtuosoAD;
        private XmlDocument doc;
        private IServicesUtilVirtuosoAndReplication mServicesUtilVirtuosoAndReplication;

        public static Dictionary<Guid, byte[]> ONTOLOGIA_XML_CACHE = new Dictionary<Guid, byte[]>();

        /// <summary>
        /// Constructor sin parámetros.
        /// </summary>
        public UtilidadesVirtuoso(LoggingService loggingService, EntityContext entityContext, ConfigService configService, RedisCacheWrapper redisCacheWrapper, EntityContextBASE entityContextBase, VirtuosoAD virtuosoAD, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
        {
            mVirtuosoAD = virtuosoAD;
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;
            mRedisCacheWrapper = redisCacheWrapper;
            mEntityContextBase = entityContextBase;
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
        }
        /// <summary>
        /// Obtiene la configuración xml de una ontología.
        /// </summary>
        /// <param name="pTextoXML">Contenido Xml de la plantilla</param>
        /// <param name="pProyectoID">ID del proyecto actual</param>
        /// <returns>Diccionario con la configuración</returns>
        public byte[] ObtenerConfiguracionXml(Guid pOntologiaID)
        {
            byte[] byteArray = null;

            if (!ONTOLOGIA_XML_CACHE.ContainsKey(pOntologiaID))
            {
                CallFileService servicioArc = new CallFileService(mConfigService, mLoggingService);
                byteArray = servicioArc.ObtenerXmlOntologiaBytes(pOntologiaID);
                ONTOLOGIA_XML_CACHE.Add(pOntologiaID, byteArray);
            }
            else
            {
                byteArray = ONTOLOGIA_XML_CACHE[pOntologiaID];
            }

            doc = new XmlDocument();
            MemoryStream ms = new MemoryStream(byteArray);
            doc.Load(ms);
            return byteArray;
        }
        private XmlNode EspefEntidad(string proyID, string nombreCortoProy)
        {

            XmlNodeList listaespef = null;
            if (doc.DocumentElement != null)
            {
                listaespef = doc.DocumentElement.SelectNodes("EspefEntidad");
                if (listaespef != null)
                {
                    if (listaespef.Count > 1)
                    {
                        foreach (XmlNode espef in listaespef)
                        {
                            if (espef.Attributes.Count > 0)
                            {
                                if (espef.Attributes[0].InnerText.Equals(proyID))
                                {
                                    return espef;
                                }
                                else if (espef.Attributes[0].InnerText.Equals(nombreCortoProy))
                                {
                                    return espef;
                                }
                            }
                        }
                        foreach (XmlNode espef in listaespef)
                        {
                            if (espef.Attributes.Count < 1)
                            {
                                return espef;
                            }
                        }
                    }
                }
            }
            if (listaespef != null)
            {
                return listaespef.Item(0);
            }
            else { return null; }

        }

        private XmlNode EspefPropiedad(string proyID, string nombreCortoProy)
        {
            XmlNodeList listaEspefPropiedad = null;
            if (doc.DocumentElement != null)
            {
                listaEspefPropiedad = doc.DocumentElement.SelectNodes("EspefPropiedad");
                if (listaEspefPropiedad != null)
                {
                    if (listaEspefPropiedad.Count > 1)
                    {
                        foreach (XmlNode espef in listaEspefPropiedad)
                        {

                            if (espef.Attributes.Count > 0)
                            {
                                if (espef.Attributes[0].InnerText.Equals(proyID))
                                {
                                    return espef;
                                }
                                else if (espef.Attributes[0].InnerText.Equals(nombreCortoProy))
                                {
                                    return espef;
                                }
                            }
                        }
                        foreach (XmlNode espef in listaEspefPropiedad)
                        {
                            if (espef.Attributes.Count < 1)
                            {
                                return espef;
                            }
                        }
                        //Devolver el que coincida con el proyecto

                        //Devolver el que coincida con el nombrecorto

                        //Devolver el que no tenga attributos
                    }
                }
            }
            if (listaEspefPropiedad != null)
            {
                return listaEspefPropiedad.Item(0);
            }
            else { return null; }

        }
        /// <summary>
        /// Devuelve los selectores del archivo XML
        /// </summary> 
        /// <returns>La clave es el grafo y el valor la lista de selectores del grafo</returns>
        private Dictionary<string, List<string>> ObtenerSelectores(Ontologia pOntologia, string pProyectoID, string pNombreCortoProy)
        {
            Dictionary<string, List<string>> dicSelectores = new Dictionary<string, List<string>>();

            if (EspefEntidad(pProyectoID, pNombreCortoProy) != null)
            {

                XmlNodeList listaSeleccionEntidad = EspefPropiedad(pProyectoID, pNombreCortoProy).SelectNodes($"Propiedad/SeleccionEntidad");
                foreach (XmlNode seleccionEntidad in listaSeleccionEntidad)
                {
                    XmlNode nodoGrafo = seleccionEntidad.SelectSingleNode("Grafo");
                    if (nodoGrafo != null)
                    {
                        string nombreGrafo = nodoGrafo.InnerText;

                        List<string> listaSelectoresNombre = new List<string>();
                        XmlNode selector = seleccionEntidad.SelectSingleNode("UrlTipoEntSolicitada");
                        if (selector == null)
                        {

                            pOntologia.LeerOntologia();
                            List<ElementoOntologia> entidadesPrincipal = GestionOWL.ObtenerElementosContenedorSuperior(pOntologia.Entidades);
                            ElementoOntologia entidadPrincipal = null;
                            if (entidadesPrincipal != null && entidadesPrincipal.Count > 0)
                            {
                                entidadPrincipal = entidadesPrincipal[0];
                                string tipoEntidad = entidadPrincipal.TipoEntidadRelativo;
                                if (!listaSelectoresNombre.Contains(tipoEntidad))
                                {
                                    listaSelectoresNombre.Add(tipoEntidad);
                                }
                            }
                        }
                        else if (selector != null)
                        {
                            string[] sel = selector.InnerText.Split(',');
                            foreach (string selNombre in sel)
                            {
                                string nombre = "";
                                if (selNombre != null && selNombre.Contains("#"))
                                {
                                    nombre = selNombre.Substring(selNombre.LastIndexOf("#") + 1);
                                }
                                else if (selNombre != null && selNombre.Contains("/"))
                                {
                                    nombre = selNombre.Substring(selNombre.LastIndexOf("/") + 1);
                                }
                                else
                                {
                                    nombre = selNombre;
                                }

                                if (!listaSelectoresNombre.Contains(nombre))
                                {
                                    listaSelectoresNombre.Add(nombre);
                                }
                            }
                        }

                        if (dicSelectores.ContainsKey(nombreGrafo))
                        {
                            dicSelectores[nombreGrafo] = dicSelectores[nombreGrafo].Union(listaSelectoresNombre).ToList();
                        }
                        else
                        {
                            dicSelectores.Add(nombreGrafo, listaSelectoresNombre);
                        }
                    }
                }
            }

            return dicSelectores;
        }

        public string ObtenerTriplesFormularioSemantico(string pFicheroConfiguracionBD, string pFicheroConfiguracionBDBase, string pUrlIntragnoss, DataWrapperFacetas pFacetaDW, Guid pOrganizacionID, Guid pProyectoID, Guid pDocumentoID, Ontologia pOntologia, Dictionary<Guid, Dictionary<string, List<string>>> pDicPropiedadesOntologia, out string tipo, List<ElementoOntologia> pElementosContenedorSuperiorOHerencias)
        {
            StringBuilder triples = new StringBuilder();

            FacetadoCN facetadoCN = new FacetadoCN(pFicheroConfiguracionBD, pUrlIntragnoss, pProyectoID.ToString(), mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            facetadoCN.FacetadoAD.CadenaConexionBase = pFicheroConfiguracionBDBase;
            FacetadoDS facetadoDS = new FacetadoDS();
            FacetaCL facetaCL = new FacetaCL(pFicheroConfiguracionBD, pFicheroConfiguracionBD, pUrlIntragnoss, mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
            Dictionary<string, List<string>> informacionOntologias = facetaCL.ObtenerPrefijosOntologiasDeProyecto(pProyectoID);

            Dictionary<string, string> urlOntologiaPorNamespace = new Dictionary<string, string>();
            foreach (string urlOnt in informacionOntologias.Keys)
            {
                string nsOnt = informacionOntologias[urlOnt][0];
                if (!urlOntologiaPorNamespace.ContainsKey(nsOnt))
                {
                    string urlOntSinArroba = urlOnt;

                    if (urlOntSinArroba.StartsWith('@'))
                    {
                        urlOntSinArroba = urlOntSinArroba.Substring(1);
                    }

                    if (!urlOntSinArroba.StartsWith("http"))
                    {
                        // Es el namespace de una ontologia: 
                        urlOntSinArroba = $"{pUrlIntragnoss}Ontologia/{urlOntSinArroba}.owl#";
                    }

                    urlOntologiaPorNamespace.Add(nsOnt, urlOntSinArroba);
                }
            }

            FacetaCN facetaCN = new FacetaCN(pFicheroConfiguracionBD, mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            DocumentacionCN documentacionCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            // Siempre debe haber una entidad Padre.
            List<string> listRdfTypePadre = new List<string>();
            foreach (ElementoOntologia entidad in pElementosContenedorSuperiorOHerencias)
            {
                listRdfTypePadre.Add(entidad.TipoEntidad);
            }

            string type = facetadoCN.ObtieneTripletasFormularios(pFacetaDW, facetadoDS, pProyectoID.ToString(), pDocumentoID.ToString(), listRdfTypePadre);

            if (type.Contains(".owl"))
            {
                type = type.Substring(0, type.LastIndexOf(".owl"));
                type = type.Substring(type.LastIndexOf("/") + 1);
            }

            //Comprobamos si el recurso que se está agregando, además de ser semántico, pertenece a una base de recursos personal.
            if (pProyectoID.Equals(ProyectoAD.MetaProyecto))
            {
                type = FacetadoAD.BUSQUEDA_RECURSOS_PERSONALES;
            }
            tipo = type;

            //Obtengo el Tipo de cada propiedad
            pFacetaDW.Merge(facetaCN.ObtenerFacetaObjetoConocimientoProyecto(pOrganizacionID, pProyectoID, true));
            pFacetaDW.Merge(facetaCN.ObtenerFacetaObjetoConocimiento(type));
            facetaCN.CargarFacetaConfigProyRanfoFecha(pOrganizacionID, pProyectoID, pFacetaDW);

            List<string> Fecha = new List<string>();
            List<string> Numero = new List<string>();
            List<string> TextoInvariable = new List<string>();
            List<string> listaPropiedades = new List<string>();

            ObtenerListaTipoElementosOntologia(pFicheroConfiguracionBD, pDocumentoID, pProyectoID, pOntologia, pDicPropiedadesOntologia, ref Fecha, ref Numero);

            //Primero recorremos los objetos de conocimiento del proyecto
            List<FacetaObjetoConocimientoProyecto> filas = pFacetaDW.ListaFacetaObjetoConocimientoProyecto.Where(item => item.ProyectoID.Equals(pProyectoID)).ToList();

            foreach (FacetaObjetoConocimientoProyecto myrow in filas)
            {
                bool autocompletar = myrow.Autocompletar;
                if (autocompletar)
                {
                    string propiedad = myrow.Faceta;
                    string propiedadSinPrefijo = propiedad.Substring(propiedad.LastIndexOf(":") + 1);

                    if (!listaPropiedades.Contains(propiedadSinPrefijo))
                    {
                        listaPropiedades.Add(propiedadSinPrefijo);
                    }
                }

                AgregarFacetaADiccionario(myrow.TipoPropiedad.Value, myrow.Faceta, urlOntologiaPorNamespace, ref Fecha, ref Numero, ref TextoInvariable);
            }

            //Recorremos la tabla con los objetos de conocimiento.
            foreach (FacetaObjetoConocimiento myrow in pFacetaDW.ListaFacetaObjetoConocimiento)
            {
                bool autocompletar = myrow.Autocompletar;
                if (autocompletar)
                {
                    string propiedad = myrow.Faceta;
                    string propiedadSinPrefijo = propiedad.Substring(propiedad.LastIndexOf(":") + 1);
                    if (!listaPropiedades.Contains(propiedadSinPrefijo))
                    {
                        listaPropiedades.Add(propiedadSinPrefijo);
                    }
                }

                AgregarFacetaADiccionario(myrow.TipoPropiedad.Value, myrow.Faceta, urlOntologiaPorNamespace, ref Fecha, ref Numero, ref TextoInvariable);
            }

            triples.Append(FacetadoAD.GenerarTripleta($"<http://gnoss/{pDocumentoID.ToString().ToUpper()}>", "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>", "\"" + type + "\""));

            DataWrapperFacetas facetasDW = facetaCL.ObtenerFacetaObjetoConocimientoProyecto(pOrganizacionID, pProyectoID);

            ObtenerConfiguracionXml(pOntologia.OntologiaID);

            Dictionary<string, List<string>> selectores = ObtenerSelectores(pOntologia, pProyectoID.ToString(), "");
            List<AD.EntityModel.Models.Documentacion.Documento> listaEntidadesSecundarias = documentacionCN.ObtenerOntologiasSecundarias(pProyectoID);
            List<FacetaEntidadesExternas> EntExt = facetasDW.ListaFacetaEntidadesExternas;
            foreach (string grafo in selectores.Keys)
            {
                foreach (string selector in selectores[grafo])
                {
                    string uriEntidadExterna = pUrlIntragnoss + "items/" + selector;
                    if (!EntExt.Exists(item => item.EntidadID.Equals(uriEntidadExterna)))
                    {
                        FacetaEntidadesExternas facetaEntidad = new FacetaEntidadesExternas();
                        facetaEntidad.Grafo = grafo;
                        facetaEntidad.EntidadID = uriEntidadExterna;
                        facetaEntidad.ProyectoID = pProyectoID;
                        facetaEntidad.EsEntidadSecundaria = listaEntidadesSecundarias.Any(item => item.Enlace.Equals(grafo));
                        EntExt.Add(facetaEntidad);
                    }
                }
            }

            bool tipoAgregado = false;

            List<FacetaConfigProyRangoFecha> filasRangoFechas = new List<FacetaConfigProyRangoFecha>();


            Dictionary<string, DateTime?> propiedadFechaInicioValorFechaInicio = new Dictionary<string, DateTime?>();
            Dictionary<string, DateTime?> propiedadFechaFinValorFechaFin = new Dictionary<string, DateTime?>();
            if (pFacetaDW.ListaFacetaConfigProyRangoFecha.Count > 0)
            {
                foreach (FacetaConfigProyRangoFecha fila in pFacetaDW.ListaFacetaConfigProyRangoFecha)
                {
                    if (fila.PropiedadNueva != null && !string.IsNullOrEmpty(fila.PropiedadNueva))
                    {
                        filasRangoFechas.Add(fila);
                        if (!Fecha.Contains(fila.PropiedadNueva))
                        {
                            Fecha.Add(fila.PropiedadNueva);
                        }
                    }
                }
            }

            List<TripleWrapper> listaTriples = PasarDataSetAListString(facetadoDS);

            foreach (DataRow myrow in facetadoDS.Tables["FormulariosSemanticosPadre"].Rows)
            {
                try
                {
                    if (((string)myrow[1]).Contains("http://www.w3.org/1999/02/22-rdf-syntax-ns#type") && !tipoAgregado)
                    {
                        string objeto = (string)myrow[2];
                        triples.Append(FacetadoAD.GenerarTripleta($"<http://gnoss/{pDocumentoID.ToString().ToUpper()}>", "<http://gnoss/type>", $"\"{UtilCadenas.EliminarHtmlDeTexto(objeto)}\""));
                        tipoAgregado = true;
                    }
                    if (!((string)myrow[1]).Contains("http://www.w3.org/1999/02/22-rdf-syntax-ns#type") && !((string)myrow[1]).Contains("http://www.w3.org/2000/01/rdf-schema#label"))
                    {
                        string objeto = (string)myrow[2];

                        if (!TextoInvariable.Contains(myrow[1]))
                        {
                            objeto = UtilCadenas.EliminarHtmlDeTexto(objeto);
                        }

                        string idioma = null;

                        if (myrow.ItemArray.Length > 3 && !myrow.IsNull(3) && !string.IsNullOrEmpty((string)myrow[3]))
                        {
                            idioma = (string)myrow[3];
                        }

                        string predicado = (string)myrow[1];
                        if (Fecha.Contains(predicado))
                        {
                            string fechaConvertida = ConvertirFormatoFecha(objeto);
                            long fechaInt;
                            if (long.TryParse(fechaConvertida, out fechaInt))
                            {
                                objeto = $"{fechaConvertida} . ";
                            }
                            else
                            {
                                objeto = $"\"{fechaConvertida}\" . ";
                            }
                        }

                        ComprobarPropiedadEsRangoFecha(filasRangoFechas, predicado, objeto, listaTriples, propiedadFechaInicioValorFechaInicio, propiedadFechaFinValorFechaFin);

                        triples.Append(GenerarTripletaRecogidadeVirtuoso_ControlCheckPoint(pFicheroConfiguracionBD, pFicheroConfiguracionBDBase, pUrlIntragnoss, $"<http://gnoss/{pDocumentoID.ToString().ToUpper()}>", (string)myrow[1], PasarObjetoALower(objeto), objeto, Fecha, Numero, TextoInvariable, EntExt, idioma));
                    }
                }
                catch
                {
                    //Si falla al montar un triple, no deberíamos romper el resto.
                }
            }

            string sujetoAuxiliar = $"<http://gnossAuxiliar/{pDocumentoID.ToString().ToUpper()}>";

            //sin <> porque el GenerarTripletaRecogidadeVirtuoso_ControlCheckPoint ya lo mete
            string predicadoAuxiliar = "http://gnoss/hasEntidadAuxiliar";

            foreach (DataRow myrow in facetadoDS.Tables["FormulariosSemanticosHijo"].Rows)
            {
                try
                {
                    if (!((string)myrow[1]).Contains("http://www.w3.org/1999/02/22-rdf-syntax-ns#type") && !((string)myrow[1]).Contains("http://www.w3.org/2000/01/rdf-schema#label"))
                    {
                        string objeto = UtilCadenas.EliminarHtmlDeTexto((string)myrow[2]);

                        string idioma = null;

                        if (myrow.ItemArray.Length > 3 && !myrow.IsNull(3) && !string.IsNullOrEmpty((string)myrow[3]))
                        {
                            idioma = (string)myrow[3];
                        }

                        string predicado = (string)myrow[1];
                        string sujeto = (string)myrow[0];
                        if (Fecha.Contains(predicado))
                        {
                            string fechaConvertida = ConvertirFormatoFecha(objeto);
                            long fechaInt;
                            if (long.TryParse(fechaConvertida, out fechaInt))
                            {
                                objeto = $"{fechaConvertida} . ";
                            }
                            else
                            {
                                objeto = $"\"{fechaConvertida}\" . ";
                            }
                        }

                        triples.Append(GenerarTripletaRecogidadeVirtuoso_ControlCheckPoint(pFicheroConfiguracionBD, pFicheroConfiguracionBDBase, pUrlIntragnoss, PasarObjetoALower(sujeto), predicado, PasarObjetoALower(objeto), objeto, Fecha, Numero, TextoInvariable, EntExt, idioma));

                        //Insertamos tripleta de la entidad auxiliar
                        triples.Append(GenerarTripletaRecogidadeVirtuoso_ControlCheckPoint(pFicheroConfiguracionBD, pFicheroConfiguracionBDBase, pUrlIntragnoss, sujetoAuxiliar, predicadoAuxiliar, PasarObjetoALower(sujeto), sujeto, Fecha, Numero, TextoInvariable, EntExt, null));
                    }
                }
                catch
                {
                    //Si falla al montar un triple, no deberíamos romper el resto.
                }
            }

            GenerarTriplesRangoFechasConfiguradas(filasRangoFechas, propiedadFechaInicioValorFechaInicio, propiedadFechaFinValorFechaFin, facetadoDS);

            if (propiedadFechaInicioValorFechaInicio.Count > 0)
            {
                foreach (string propiedadFechaInicio in propiedadFechaInicioValorFechaInicio.Keys)
                {
                    DateTime? valorPropiedadFechaInicio = propiedadFechaInicioValorFechaInicio[propiedadFechaInicio];
                    DateTime? valorPropiedadFechaFin = null;
                    string valorPropiedad = ConvertirFormatoFecha(valorPropiedadFechaInicio.Value.ToString("dd/MM/yyyy"));
                    FacetaConfigProyRangoFecha filaRangoFecha = filasRangoFechas.First(fila => fila.PropiedadInicio.Equals(propiedadFechaInicio));

                    if (propiedadFechaFinValorFechaFin.ContainsKey(filaRangoFecha.PropiedadFin))
                    {
                        valorPropiedadFechaFin = propiedadFechaFinValorFechaFin[filaRangoFecha.PropiedadFin];
                    }

                    long fechaInt;
                    if (long.TryParse(valorPropiedad, out fechaInt))
                    {
                        valorPropiedad = $"{valorPropiedad} . ";
                    }
                    else
                    {
                        valorPropiedad = $"\"{valorPropiedad}\" . ";
                    }

                    triples.Append(GenerarTripletaRecogidadeVirtuoso_ControlCheckPoint(pFicheroConfiguracionBD, pFicheroConfiguracionBDBase, pUrlIntragnoss, $"<http://gnoss/{pDocumentoID.ToString().ToUpper()}>", filaRangoFecha.PropiedadNueva, PasarObjetoALower(valorPropiedad), valorPropiedad, Fecha, Numero, TextoInvariable, EntExt, null));
                    if (valorPropiedadFechaFin.HasValue && valorPropiedadFechaFin.Value > valorPropiedadFechaInicio.Value)
                    {
                        DateTime temp = valorPropiedadFechaInicio.Value.AddDays(1);

                        while (temp <= valorPropiedadFechaFin.Value)
                        {
                            string valorPropiedadTemp = ConvertirFormatoFecha(temp.ToString("dd/MM/yyyy"));
                            long fechaInt2;
                            if (long.TryParse(valorPropiedadTemp, out fechaInt2))
                            {
                                valorPropiedadTemp = $"{valorPropiedadTemp} . ";
                            }
                            else
                            {
                                valorPropiedadTemp = $"\"{valorPropiedadTemp}\" . ";
                            }

                            triples.Append(GenerarTripletaRecogidadeVirtuoso_ControlCheckPoint(pFicheroConfiguracionBD, pFicheroConfiguracionBDBase, pUrlIntragnoss, $"<http://gnoss/{pDocumentoID.ToString().ToUpper()}>", filaRangoFecha.PropiedadNueva, PasarObjetoALower(valorPropiedadTemp), valorPropiedadTemp, Fecha, Numero, TextoInvariable, EntExt, null));
                            temp = temp.AddDays(1);
                        }
                    }
                }
            }

            return triples.ToString();
        }

        /// <summary>
        /// Añade la propiedad configurada como fecha inicio y la propiedad configurada como fecha fin a su respectivo diccionario con el valor de la fecha
        /// </summary>
        /// <param name="pRangoFechasConfigurados">Lista con los rangos de fechas configurados en base de datos</param>
        /// <param name="pPropiedadFechaInicioValorFechaInicio">Diccionario con las propiedades configuradas como fecha inicio y el valor de esa propiedad</param>
        /// <param name="pPropiedadFechaFinValorFechaFin">Diccionario con las propiedades configuradas como fecha final y el valor de esa propiedad</param>
        /// <param name="pConjuntoTriples">DataSet con el conjunto de triples del recurso</param>
        private void GenerarTriplesRangoFechasConfiguradas(List<FacetaConfigProyRangoFecha> pRangoFechasConfigurados, Dictionary<string, DateTime?> pPropiedadFechaInicioValorFechaInicio, Dictionary<string, DateTime?> pPropiedadFechaFinValorFechaFin, FacetadoDS pConjuntoTriples)
        {
            foreach (FacetaConfigProyRangoFecha rangoFecha in pRangoFechasConfigurados)
            {
                if (rangoFecha.PropiedadInicio.Contains("@@@") && rangoFecha.PropiedadFin.Contains("@@@"))
                {
                    //Si las propiedades configuradas no son de primer nivel buscamos entre los datos el valor siguiendo el path configurado
                    GenerarTriplesRangoFechasConfiguradasAnidadas(rangoFecha, pPropiedadFechaInicioValorFechaInicio, pPropiedadFechaFinValorFechaFin, pConjuntoTriples);
                }
                else
                {
                    DataRow tripleFechaInicio;
                    DataRow tripleFechaFin;

                    ObtenerFilaTripleDeFormularioSemanticoPadreOHijo(out tripleFechaInicio, out tripleFechaFin, pConjuntoTriples, rangoFecha.PropiedadInicio, rangoFecha.PropiedadFin);

                    AddTriplesRangoFechaInicioFechaFin(rangoFecha, pPropiedadFechaInicioValorFechaInicio, pPropiedadFechaFinValorFechaFin, tripleFechaInicio, tripleFechaFin);
                }
            }
        }

        /// <summary>
        /// Añadimos la propiedad con su valor buscando entre el conjunto de triples para propiedades anidadas siguiendo el path definido en la propiedad
        /// </summary>
        /// <param name="pRangoFecha">Rango de fecha configuada a obtener el valor de incio y fin</param>
        /// <param name="pPropiedadFechaInicioValorFechaInicio">Diccionario con las propiedades configuradas como fecha inicio y el valor de esa propiedad</param>
        /// <param name="pPropiedadFechaFinValorFechaFin">Diccionario con las propiedades configuradas como fecha final y el valor de esa propiedad</param>
        /// <param name="pConjuntoTriples">DataSet con el conjunto de triples del recurso</param>
        private void GenerarTriplesRangoFechasConfiguradasAnidadas(FacetaConfigProyRangoFecha pRangoFecha, Dictionary<string, DateTime?> pPropiedadFechaInicioValorFechaInicio, Dictionary<string, DateTime?> pPropiedadFechaFinValorFechaFin, FacetadoDS pConjuntoTriples)
        {
            string[] segmentosRangoFechaInicio = pRangoFecha.PropiedadInicio.Split("@@@", StringSplitOptions.RemoveEmptyEntries);
            string[] segmentosRangoFechaFin = pRangoFecha.PropiedadFin.Split("@@@", StringSplitOptions.RemoveEmptyEntries);

            string sujetoPropiedadBuscadaInicio = string.Empty;
            string sujetoPropiedadBuscadaFin = string.Empty;

            DataRow tripleFechaInicio = null;
            DataRow tripleFechaFin = null;

            //Recorremos los segmentos de la propiedad configurada
            for (int i = 0; i < segmentosRangoFechaInicio.Length; i++)
            {
                string propiedadFechaInicio = segmentosRangoFechaInicio[i];
                string propiedadFechaFin = segmentosRangoFechaFin[i];

                ObtenerFilaTripleDeFormularioSemanticoPadreOHijo(out tripleFechaInicio, out tripleFechaFin, pConjuntoTriples, propiedadFechaInicio, propiedadFechaFin, sujetoPropiedadBuscadaInicio, sujetoPropiedadBuscadaFin);

                if (tripleFechaInicio == null || tripleFechaFin == null)
                {
                    break;
                }

                sujetoPropiedadBuscadaInicio = (string)tripleFechaInicio[2];
                sujetoPropiedadBuscadaFin = (string)tripleFechaFin[2];
            }

            AddTriplesRangoFechaInicioFechaFin(pRangoFecha, pPropiedadFechaInicioValorFechaInicio, pPropiedadFechaFinValorFechaFin, tripleFechaInicio, tripleFechaFin);
        }

        /// <summary>
        /// Obtiene el triple de la fecha inicio y el triple de la fecha fin cuyos predicados se pasan por parámetro. Si aportamos un sujeto se filtra en el conjunto de triples también por el sujeto. Busca en la tabla del dataset FormulariosSemanticosPadre y si no encuentra el predicado buscado ahí lo busca en la tabla Formularios
        /// </summary>
        /// <param name="pTripleFechaIncio">Parámetro de salida. Devolvemos el triple que cumpla las condiciones de predicado y sujeto (si es aportado) de la propiedad configurada para la fecha inicial.</param>
        /// <param name="pTripleFechaFin">Parámetro de salida. Devolvemos el triple que cumpla las condiciones de predicado y sujeto (si es aportado) de la propiedad configurada para la fecha final.</param>
        /// <param name="pConjuntoTriples">DataSet con el conjunto de triples del recurso</param>
        /// <param name="pPredicadoFechaInicio">Predicado de la fecha de inicio del cual queremos obtener el triple</param>
        /// <param name="pPredicadoFechaFin">Predicado de la fecha de fin del cual queremos obtener el triple</param>
        /// <param name="pSujetoFechaIncio">(Opcional) Sujeto de la fecha inicio del cual queremos obtener el triple. Solo necesario para seguir el path de las propiedades anidadas</param>
        /// <param name="pSujetoFechaFin">(Opcional) Sujeto de la fecha fin del cual queremos obtener el triple. Solo necesario para seguir el path de las propiedades anidadas</param>
        private void ObtenerFilaTripleDeFormularioSemanticoPadreOHijo(out DataRow pTripleFechaIncio, out DataRow pTripleFechaFin, FacetadoDS pConjuntoTriples, string pPredicadoFechaInicio, string pPredicadoFechaFin, string pSujetoFechaIncio = "", string pSujetoFechaFin = "")
        {
            //Buscamos en la taba FormulariosSemanticosPadre
            pTripleFechaIncio = ObtenerPropiedadApuntada(pConjuntoTriples.Tables["FormulariosSemanticosPadre"], pPredicadoFechaInicio, pSujetoFechaIncio);
            pTripleFechaFin = ObtenerPropiedadApuntada(pConjuntoTriples.Tables["FormulariosSemanticosPadre"], pPredicadoFechaFin, pSujetoFechaFin);

            if (pTripleFechaIncio == null || pTripleFechaFin == null)
            {
                //Si no lo hemos encontrado en la tabla FormulariosSemanticosPadre buscamos en FormulariosSemanticosHijo
                pTripleFechaIncio = ObtenerPropiedadApuntada(pConjuntoTriples.Tables["FormulariosSemanticosHijo"], pPredicadoFechaInicio, pSujetoFechaIncio);
                pTripleFechaFin = ObtenerPropiedadApuntada(pConjuntoTriples.Tables["FormulariosSemanticosHijo"], pPredicadoFechaFin, pSujetoFechaFin);
            }
        }

        /// <summary>
        /// Obtenemos de la tabla pasada por parámetro el triple al que pertenece el predicado y sujeto (opcional) dado. Si no se pasa el sujeto solo se filtrará por el predicado.
        /// </summary>
        /// <param name="pConjuntoTriples">DataTable con el conjunto de triples entre los que buscar</param>
        /// <param name="pPredicadoObtener">Predicado del triple a buscar en la tabla. Solo traerá el primer resultado, no debería haber más.</param>
        /// <param name="pSujetoObtener">(Opcional) Sujeto del triple a buscar en la tabla. Solo traerá el primer resultado, no debería haber más</param>
        /// <returns>La fila de la tabla con el triple obtenido.</returns>
        private DataRow ObtenerPropiedadApuntada(DataTable pConjuntoTriples, string pPredicadoObtener, string pSujetoObtener = "")
        {
            if (string.IsNullOrEmpty(pSujetoObtener))
            {
                return pConjuntoTriples.AsEnumerable().Where(item => item[1].Equals(pPredicadoObtener)).FirstOrDefault();
            }
            else
            {
                return pConjuntoTriples.AsEnumerable().Where(item => item[1].Equals(pPredicadoObtener) && item[0].Equals(pSujetoObtener)).FirstOrDefault();
            }
        }

        /// <summary>
        /// Añade a el diccionario de propiedades configuradas para la fecha inicio su propiedad con el valor y al diccionario de propiedades configuradas para la fecha fin su propiedad con su valor. Si la propiedad era anidada la añadirá anidadá al diccionario con el valor final de la fecha a la que apunta la última propiedad del anidamiento.
        /// </summary>
        /// <param name="pRangoFecha">Rango de la fecha configurada en base de datos que vamos a añadir a los diccionarios</param>
        /// <param name="pPropiedadFechaInicioValorFechaInicio">Diccionario con las propiedades configuradas como fecha inicio y el valor de esa propiedad</param>
        /// <param name="pPropiedadFechaFinValorFechaFin">Diccionario con las propiedades configuradas como fecha fin y el valor de esa propiedad</param>
        /// <param name="pTripleFechaInicio">Triple con el valor de la propiedad configurada como fecha inicial</param>
        /// <param name="pTripleFechaFin">Triple con el valor de la propiedad configurada como fecha final</param>
        private void AddTriplesRangoFechaInicioFechaFin(FacetaConfigProyRangoFecha pRangoFecha, Dictionary<string, DateTime?> pPropiedadFechaInicioValorFechaInicio, Dictionary<string, DateTime?> pPropiedadFechaFinValorFechaFin, DataRow pTripleFechaInicio, DataRow pTripleFechaFin)
        {
            if (pTripleFechaInicio != null && pTripleFechaFin != null)
            {
                string predicadoFechaInicio = (string)pTripleFechaInicio[1];
                string predicadoFechaFin = (string)pTripleFechaFin[1];
                DateTime fechaInicio;
                DateTime fechaFin;

                if (!pPropiedadFechaInicioValorFechaInicio.ContainsKey(predicadoFechaInicio) && DateTime.TryParseExact(DesconvertirFormatoFecha((string)pTripleFechaInicio[2]), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaInicio))
                {
                    pPropiedadFechaInicioValorFechaInicio.Add(pRangoFecha.PropiedadInicio, fechaInicio);
                }
                if (!pPropiedadFechaFinValorFechaFin.ContainsKey(predicadoFechaFin) && DateTime.TryParseExact(DesconvertirFormatoFecha((string)pTripleFechaFin[2]), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaFin))
                {
                    pPropiedadFechaFinValorFechaFin.Add(pRangoFecha.PropiedadFin, fechaFin);
                }
            }
        }

        private static List<TripleWrapper> PasarDataSetAListString(FacetadoDS pFacetadoDS)
        {
            List<TripleWrapper> listaTriples = new List<TripleWrapper>();
            foreach (DataRow myrow in pFacetadoDS.Tables["FormulariosSemanticosPadre"].Rows)
            {
                TripleWrapper triple = null;
                int tripleLength = 3;
                if (myrow.ItemArray.Length > 3 && !myrow.IsNull(3) && !string.IsNullOrEmpty((string)myrow[3]))
                {
                    tripleLength = 4;
                }

                triple = new TripleWrapper();
                triple.Subject = $"<{myrow[0]}>";
                triple.Predicate = $"<{myrow[1]}>";
                if (string.IsNullOrEmpty(myrow[2].ToString()))
                {
                    triple.Object = "";
                }
                else
                {
                    triple.Object = (string)myrow[2];
                }

                if (tripleLength == 4)
                {
                    triple.ObjectLanguage = (string)myrow[3];
                }
                listaTriples.Add(triple);
            }
            foreach (DataRow myrow in pFacetadoDS.Tables["FormulariosSemanticosHijo"].Rows)
            {
                TripleWrapper triple = null;
                int tripleLength = 3;
                if (myrow.ItemArray.Length > 3 && !myrow.IsNull(3) && !string.IsNullOrEmpty((string)myrow[3]))
                {
                    tripleLength = 4;
                }

                triple = new TripleWrapper();
                triple.Subject = $"<{myrow[0]}>";
                triple.Predicate = $"<{myrow[1]}>";
                triple.Object = (string)myrow[2];

                if (tripleLength == 4)
                {
                    triple.ObjectLanguage = (string)myrow[3];
                }
                listaTriples.Add(triple);
            }

            return listaTriples;
        }

        public static void ObtenerListaTipoElementosOntologia(string pFicheroConfiguracionBD, Guid pDocumentoID, Guid pProyectoID, Ontologia pOntologia, Dictionary<Guid, Dictionary<string, List<string>>> pDicPropiedadesOntologia, ref List<string> pFecha, ref List<string> pNumero)
        {
            if (pOntologia != null)
            {
                if (!pDicPropiedadesOntologia.ContainsKey(pOntologia.OntologiaID))
                {
                    foreach (ElementoOntologia elementoOntologia in pOntologia.Entidades)
                    {
                        foreach (Propiedad propiedad in elementoOntologia.Propiedades)
                        {
                            if (!string.IsNullOrEmpty(propiedad.NombreConNamespace))
                            {
                                string propiedadElementoOntologia = propiedad.NombreConNamespace.Substring(propiedad.NombreConNamespace.LastIndexOf(":") + 1);

                                if (propiedad.RangoEsFecha && !pFecha.Contains(propiedadElementoOntologia))
                                {
                                    pFecha.Add(propiedadElementoOntologia);

                                    if (!string.IsNullOrEmpty(propiedad.Nombre) && !pFecha.Contains(propiedad.Nombre))
                                    {
                                        pFecha.Add(propiedad.Nombre);
                                    }
                                }

                                if (propiedad.RangoEsNumerico && !pNumero.Contains(propiedadElementoOntologia))
                                {
                                    pNumero.Add(propiedadElementoOntologia);

                                    if (!string.IsNullOrEmpty(propiedad.Nombre) && !pNumero.Contains(propiedad.Nombre))
                                    {
                                        pNumero.Add(propiedad.Nombre);
                                    }
                                }
                            }
                        }
                    }

                    Dictionary<string, List<string>> listaPropiedades = new Dictionary<string, List<string>>();
                    listaPropiedades.Add(PROPIEDADES_ONTOLOGIA_FECHA, pFecha);
                    listaPropiedades.Add(PROPIEDADES_ONTOLOGIA_NUMERO, pNumero);

                    pDicPropiedadesOntologia.Add(pOntologia.OntologiaID, listaPropiedades);
                }
                else
                {
                    pFecha = pDicPropiedadesOntologia[pOntologia.OntologiaID][PROPIEDADES_ONTOLOGIA_FECHA];
                    pNumero = pDicPropiedadesOntologia[pOntologia.OntologiaID][PROPIEDADES_ONTOLOGIA_NUMERO];
                }
            }
        }

        public static void AgregarFacetaADiccionario(short pTipoPropiedad, string pFaceta, Dictionary<string, string> pUrlOntologiaPorNamespace, ref List<string> pListaElementosFecha, ref List<string> pListaElementosNumero, ref List<string> pListaElementosTextoInvariable)
        {
            if (pTipoPropiedad.Equals((short)TipoPropiedadFaceta.Fecha) || pTipoPropiedad.Equals((short)TipoPropiedadFaceta.Calendario) || pTipoPropiedad.Equals((short)TipoPropiedadFaceta.CalendarioConRangos) || pTipoPropiedad.Equals((short)TipoPropiedadFaceta.Siglo))
            {
                AgregarFacetaADiccionarioEspecifico(ref pListaElementosFecha, pFaceta, pUrlOntologiaPorNamespace);
            }

            if (pTipoPropiedad.Equals((short)TipoPropiedadFaceta.Numero))
            {
                AgregarFacetaADiccionarioEspecifico(ref pListaElementosNumero, pFaceta, pUrlOntologiaPorNamespace);
            }

            if (pTipoPropiedad.Equals((short)TipoPropiedadFaceta.TextoInvariable))
            {
                AgregarFacetaADiccionarioEspecifico(ref pListaElementosTextoInvariable, pFaceta, pUrlOntologiaPorNamespace);
            }
        }

        private static void AgregarFacetaADiccionarioEspecifico(ref List<string> pListaElementos, string pFacetaAux, Dictionary<string, string> pUrlOntologiaPorNamespace)
        {
            string nsOnt = pFacetaAux.Substring(0, pFacetaAux.LastIndexOf(":"));
            if (pFacetaAux.Contains("@@@"))
            {
                nsOnt = pFacetaAux.Substring(pFacetaAux.LastIndexOf("@@@") + 3);
                nsOnt = nsOnt.Substring(0, nsOnt.LastIndexOf(":"));
            }

            string nombrePropiedad = pFacetaAux.Substring(pFacetaAux.LastIndexOf(":") + 1);
            string urlCompletaPropiedad = null;

            if (pUrlOntologiaPorNamespace.ContainsKey(nsOnt))
            {
                urlCompletaPropiedad = pUrlOntologiaPorNamespace[nsOnt] + nombrePropiedad;
            }

            if (!pListaElementos.Contains(nombrePropiedad))
            {
                pListaElementos.Add(nombrePropiedad);
            }
            if (!pListaElementos.Contains(pFacetaAux))
            {
                pListaElementos.Add(pFacetaAux);
            }
            if (!string.IsNullOrEmpty(urlCompletaPropiedad) && !pListaElementos.Contains(urlCompletaPropiedad))
            {
                pListaElementos.Add(urlCompletaPropiedad);
            }
        }

        public string ObtenerValoresSemanticosSearch(string pFicheroConfiguracionBD, string pUrlIntragnoss, Guid pProyectoID, Guid pDocumentoID)
        {
            string valorSearch = string.Empty;

            FacetaCN facCN = new FacetaCN(pFicheroConfiguracionBD, mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            List<OntologiaProyecto> listaOntologias = facCN.ObtenerOntologias(pProyectoID);
            facCN.Dispose();

            Dictionary<string, List<string>> informacionOntologias = FacetadoAD.ObtenerInformacionOntologias(listaOntologias);

            Dictionary<string, string> diccionarioSearch = new Dictionary<string, string>();

            List<ConfigAutocompletarProy> configAutocompletarProyRows = mEntityContext.ConfigAutocompletarProy.Where(item => item.ProyectoID.Equals(pProyectoID)).ToList();
            List<ConfigSearchProy> configSearchProyRows = mEntityContext.ConfigSearchProy.Where(item => item.ProyectoID.Equals(pProyectoID)).ToList();

            foreach (ConfigSearchProy filaConfigSearch in configSearchProyRows)
            {
                if (!diccionarioSearch.ContainsKey(filaConfigSearch.Clave))
                {
                    diccionarioSearch.Add($"{filaConfigSearch.Clave}_search", filaConfigSearch.Valor);
                }
            }
            foreach (ConfigAutocompletarProy filaTagsAuto in configAutocompletarProyRows)
            {
                if (!diccionarioSearch.ContainsKey(filaTagsAuto.Clave))
                {
                    diccionarioSearch.Add($"{filaTagsAuto.Clave}_autocompletar", filaTagsAuto.Valor);
                }
            }
            List<string> listaPropiedades = new List<string>();
            foreach (string clave in diccionarioSearch.Keys)
            {
                string namespaces = diccionarioSearch[clave];
                char[] separadores = { '|' };
                foreach (string propiedad in namespaces.Split(separadores, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (!listaPropiedades.Contains(propiedad))
                    {
                        listaPropiedades.Add(propiedad);
                    }
                }
            }

            if (listaPropiedades.Count > 0)
            {
                FacetadoCN facetadoCN = new FacetadoCN(pUrlIntragnoss, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);

                facetadoCN.InformacionOntologias = informacionOntologias;
                List<Guid> listaDocs = new List<Guid>();
                listaDocs.Add(pDocumentoID);
                StringBuilder stringBuilderSearch = new StringBuilder();
                for (int i = 0; i < listaPropiedades.Count; i += 40)
                {
                    FacetadoDS facDSAux = facetadoCN.ObtenerValoresPropiedadesEntidadesPorDocumentoID(pProyectoID.ToString(), listaDocs, listaPropiedades.GetRange(i, Math.Min(40, listaPropiedades.Count - i)).ToList());

                    foreach (DataRow fila in facDSAux.Tables["SelectPropEnt"].Rows)
                    {
                        //Concatenamos el objeto
                        stringBuilderSearch.Append($" {fila["o"].ToString()}");
                    }
                }

                valorSearch += stringBuilderSearch.ToString();
                facetadoCN.Dispose();
            }

            return valorSearch;
        }

        /// <summary>
        /// Control de insercciones en virtuoso en las horas del checkpoint.
        /// </summary>
        /// <param name="pFacetadoAD">Controlador de virtuoso AD.</param>
        /// <param name="pProyectoID">ProyectoID</param>
        /// <param name="pTripletas">Tripletas a insertar</param>
        /// <param name="pListaElementosaModificarID"></param>
        /// <param name="pCondicionesWhere"></param>
        /// <param name="pCondicionesFilter"></param>
        public void InsertarFicheroNTEnVirtuoso(string pFicheroConfiguracionBD, string pFicheroConfiguracionBDBase, string pUrlIntragnoss, string pProyectoID)
        {
            FacetadoAD pFacetadoAD = null;
            try
            {
                pFacetadoAD = new FacetadoAD(pFicheroConfiguracionBD, pUrlIntragnoss, "", mLoggingService, mEntityContext, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                pFacetadoAD.CadenaConexionBase = pFicheroConfiguracionBDBase;

                //Creamos el índice (si ya existe, esta instrucción no hace nada)
                pFacetadoAD.ActualizarVirtuoso("DB.DBA.RDF_OBJ_FT_RULE_ADD('" + pUrlIntragnoss + pProyectoID.ToLower() + "', 'http://gnoss/search', 'search')", pProyectoID);

                //Actualizamos el índice
                pFacetadoAD.ActualizarVirtuoso("VT_INC_INDEX_DB_DBA_RDF_OBJ()", pProyectoID);
            }
            catch (Exception)
            {
                //Cerramos las conexiones
                ControladorConexiones.CerrarConexiones();

                //Realizamos una consulta ask a virtuoso para comprobar si está funcionando
                while (!ServidorOperativo(pFicheroConfiguracionBD, pUrlIntragnoss))
                {
                    //Dormimos 30 segundos
                    Thread.Sleep(30 * 1000);
                }

                pFacetadoAD = new FacetadoAD(pFicheroConfiguracionBD, pUrlIntragnoss, "", mLoggingService, mEntityContext, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                pFacetadoAD.CadenaConexionBase = pFicheroConfiguracionBDBase;

                //Creamos el índice (si ya existe, esta instrucción no hace nada)
                pFacetadoAD.ActualizarVirtuoso("DB.DBA.RDF_OBJ_FT_RULE_ADD('" + pUrlIntragnoss + pProyectoID.ToLower() + "', 'http://gnoss/search', 'search');", pProyectoID);

                //Actualizamos el índice
                pFacetadoAD.ActualizarVirtuoso("VT_INC_INDEX_DB_DBA_RDF_OBJ();", pProyectoID);
            }
            finally
            {
                pFacetadoAD.Dispose();
                pFacetadoAD = null;
            }
        }



        /// <summary>
        /// Control de insercciones en virtuoso en las horas del checkpoint.
        /// </summary>
        /// <param name="pFacetadoAD">Controlador de virtuoso AD.</param>
        /// <param name="pProyectoID">ProyectoID</param>
        /// <param name="pTripletas">Tripletas a insertar</param>
        /// <param name="pListaElementosaModificarID"></param>
        /// <param name="pCondicionesWhere"></param>
        /// <param name="pCondicionesFilter"></param>
        public void CrearIndiceSearchEnComunidad(string pFicheroConfiguracionBD, string pFicheroConfiguracionBDBase, string pUrlIntragnoss, string pProyectoID)
        {
            FacetadoAD pFacetadoAD = null;
            try
            {
                pFacetadoAD = new FacetadoAD(pFicheroConfiguracionBD, pUrlIntragnoss, "", mLoggingService, mEntityContext, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                pFacetadoAD.CadenaConexionBase = pFicheroConfiguracionBDBase;

                //Creamos el índice (si ya existe, esta instrucción no hace nada)
                pFacetadoAD.ActualizarVirtuoso("DB.DBA.RDF_OBJ_FT_RULE_ADD('" + pUrlIntragnoss + pProyectoID.ToLower() + "', 'http://gnoss/search', 'search')", pProyectoID);
            }
            catch (Exception)
            {
                //Cerramos las conexiones
                ControladorConexiones.CerrarConexiones();

                //Realizamos una consulta ask a virtuoso para comprobar si está funcionando
                while (!ServidorOperativo(pFicheroConfiguracionBD, pUrlIntragnoss))
                {
                    //Dormimos 30 segundos
                    Thread.Sleep(30 * 1000);
                }

                pFacetadoAD = new FacetadoAD(pFicheroConfiguracionBD, pUrlIntragnoss, "", mLoggingService, mEntityContext, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                pFacetadoAD.CadenaConexionBase = pFicheroConfiguracionBDBase;

                //Creamos el índice (si ya existe, esta instrucción no hace nada)
                pFacetadoAD.ActualizarVirtuoso("DB.DBA.RDF_OBJ_FT_RULE_ADD('" + pUrlIntragnoss + pProyectoID.ToLower() + "', 'http://gnoss/search', 'search');", pProyectoID);
            }
            finally
            {
                pFacetadoAD.Dispose();
                pFacetadoAD = null;
            }
        }

        /// <summary>
        /// Control de insercciones en virtuoso en las horas del checkpoint.
        /// </summary>
        /// <param name="pFacetadoAD">Controlador de virtuoso AD.</param>
        /// <param name="pProyectoID">ProyectoID</param>
        /// <param name="pTripletas">Tripletas a insertar</param>
        /// <param name="pListaElementosaModificarID"></param>
        /// <param name="pCondicionesWhere"></param>
        /// <param name="pCondicionesFilter"></param>
        public void ActualizarIndiceSearch_ControlCheckPoint(string pFicheroConfiguracionBD, string pFicheroConfiguracionBDBase, string pUrlIntragnoss, string pTablaReplica)
        {
            FacetadoAD pFacetadoAD = null;
            try
            {
                pFacetadoAD = new FacetadoAD(pFicheroConfiguracionBD, pUrlIntragnoss, pTablaReplica, mLoggingService, mEntityContext, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                pFacetadoAD.CadenaConexionBase = pFicheroConfiguracionBDBase;

                //Actualizamos el índice
                //pFacetadoAD.ActualizarVirtuoso("DB.DBA.RDF_OBJ_FT_RULE_ADD(null, 'http://gnoss/search', 'search');", "");
                pFacetadoAD.ActualizarVirtuoso("VT_INC_INDEX_DB_DBA_RDF_OBJ()", "");
            }
            catch (Exception)
            {
                //Cerramos las conexiones
                ControladorConexiones.CerrarConexiones();

                //Realizamos una consulta ask a virtuoso para comprobar si está funcionando
                while (!ServidorOperativo(pFicheroConfiguracionBD, pUrlIntragnoss))
                {
                    //Dormimos 30 segundos
                    Thread.Sleep(30 * 1000);
                }

                pFacetadoAD = new FacetadoAD(pFicheroConfiguracionBD, pUrlIntragnoss, "", mLoggingService, mEntityContext, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                pFacetadoAD.CadenaConexionBase = pFicheroConfiguracionBDBase;

                //Actualizamos el índice
                //pFacetadoAD.ActualizarVirtuoso("DB.DBA.RDF_OBJ_FT_RULE_ADD(null, 'http://gnoss/search', 'search');", "");
                pFacetadoAD.ActualizarVirtuoso("VT_INC_INDEX_DB_DBA_RDF_OBJ();", "");
            }
            finally
            {
                pFacetadoAD.Dispose();
                pFacetadoAD = null;
            }
        }


        /// <summary>
        /// Coge una fecha en formato 01/01/2010 y lo cambia a 20100101000000
        /// </summary>
        /// <returns>Fecha Cambiada</returns>
        private static string ConvertirFormatoFecha(string fecha)
        {
            List<string> separadores = new List<string>();
            separadores.Add("/");
            separadores.Add("-");

            foreach (string separador in separadores)
            {
                bool fechaNegativa = false;
                if (fecha.StartsWith(separador))
                {
                    // Si es una fecha negativa, quitamos el primer carácter y se lo añadimos al final.
                    fechaNegativa = true;
                    fecha = fecha.Substring(separador.Length);
                }

                if (fecha.Contains(separador))
                {
                    fecha = fecha.Trim();

                    string nfecha = string.Empty;
                    if (fecha.IndexOf(separador).Equals(2))
                    {
                        nfecha = fecha.Substring(fecha.LastIndexOf(separador) + 1, 4);
                        fecha = fecha.Substring(0, fecha.LastIndexOf(separador));
                        nfecha += fecha.Substring(fecha.LastIndexOf(separador) + 1, 2);
                        fecha = fecha.Substring(0, fecha.LastIndexOf(separador));
                        nfecha += fecha.Substring(0, 2);
                    }
                    else if (!fecha.IndexOf(separador).Equals(-1))
                    {
                        nfecha = fecha.Substring(0, fecha.IndexOf(separador));
                        fecha = fecha.Substring(fecha.IndexOf(separador) + 1);
                        nfecha += fecha.Substring(0, fecha.IndexOf(separador));
                        fecha = fecha.Substring(fecha.IndexOf(separador) + 1);
                        nfecha += fecha;
                    }

                    fecha = nfecha + "000000";
                }

                if (fechaNegativa)
                {
                    fecha = separador + fecha;
                }
            }

            return fecha;
        }

        /// <summary>
        /// Coge una fecha en formato 20100101000000 y lo cambia a 01/01/2010
        /// </summary>
        /// <returns>Fecha Cambiada</returns>
        private static string DesconvertirFormatoFecha(string fecha)
        {

            fecha = fecha.Trim();

            string anio = fecha.Substring(0, 4);
            string mes = fecha.Substring(4, 2);
            string dia = fecha.Substring(6, 2);

            fecha = dia + "/" + mes + "/" + anio;

            return fecha;
        }


        /// <summary>
        /// Devuelve si un texto tiene o no acentos
        /// </summary>
        public static bool TieneAcentos(String texto)
        {
            bool b = false;

            for (int i = 0; i < texto.Length; i++)
            {
                if (LETRAS_CON_ACENTOS.Contains(texto.Substring(i, 1)))
                { b = true; }
            }


            return b;
        }

        /// <summary>
        /// Devuelve el texto sin acentos
        /// </summary>
        public static string RemoverSignosAcentos(string texto)
        {
            StringBuilder textoSinAcentos = new StringBuilder(texto.Length);
            int indexConAcento;
            foreach (char caracter in texto)
            {
                indexConAcento = LETRAS_CON_ACENTOS.IndexOf(caracter);
                if (indexConAcento > -1)
                    textoSinAcentos.Append(LETRAS_SIN_ACENTOS.Substring(indexConAcento, 1));
                else
                    textoSinAcentos.Append(caracter);
            }
            return textoSinAcentos.ToString();
        }

        /// <summary>
        /// Devuelve el texto sin caracteres como . , ; ? ¿ !¡ ...
        /// </summary>
        public static string RemoverSignosSearch(string texto)
        {
            StringBuilder textoSinSignos = new StringBuilder(texto.Length);
            int indexConSigno;
            foreach (char caracter in texto)
            {
                indexConSigno = SIGNOS_ELIMINAR_SEARCH.IndexOf(caracter);
                if (indexConSigno > -1)
                    textoSinSignos.Append(" ");
                else
                    textoSinSignos.Append(caracter);
            }
            return textoSinSignos.ToString();
        }

        public static string PasarObjetoALower(string pObjeto)
        {
            List<string> listaTipos = new List<string>();
            listaTipos.Add("\"" + FacetadoAD.BUSQUEDA_BLOGS + "\" .");
            listaTipos.Add("\"" + FacetadoAD.BUSQUEDA_CLASE_UNIVERSIDAD + "\" .");
            listaTipos.Add("\"" + FacetadoAD.BUSQUEDA_CLASE_SECUNDARIA + "\" .");
            listaTipos.Add("\"" + FacetadoAD.BUSQUEDA_COMUNIDAD_EDUCATIVA + "\" .");
            listaTipos.Add("\"" + FacetadoAD.BUSQUEDA_COMUNIDAD_NO_EDUCATIVA + "\" .");
            listaTipos.Add("\"" + FacetadoAD.BUSQUEDA_COMUNIDADES + "\" .");
            listaTipos.Add("\"" + FacetadoAD.BUSQUEDA_DAFOS + "\" .");
            listaTipos.Add("\"" + FacetadoAD.BUSQUEDA_DEBATES + "\" .");
            listaTipos.Add("\"" + FacetadoAD.BUSQUEDA_ORGANIZACION + "\" .");
            listaTipos.Add("\"" + FacetadoAD.BUSQUEDA_PERSONA + "\" .");
            listaTipos.Add("\"" + FacetadoAD.BUSQUEDA_PREGUNTAS + "\" .");
            listaTipos.Add("\"" + FacetadoAD.BUSQUEDA_ENCUESTAS + "\" .");
            listaTipos.Add("\"" + FacetadoAD.BUSQUEDA_RECURSOS + "\" .");

            if ((!pObjeto.StartsWith("<http://gnoss/")) && (!listaTipos.Contains(pObjeto)))
            {
                return pObjeto.ToLowerSearchGraph();
            }
            return pObjeto;
        }

        #region Generar Tripletas

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mTripletas"></param>
        /// <param name="mTripletasGnoss"></param>
        /// <param name="mTripletasContribuciones"></param>
        /// <param name="id"></param>
        /// <param name="proyid"></param>
        /// <param name="pPropiedad"></param>
        /// <param name="pTextoSinSeparar"></param>
        public static string AgregarTripletasDescompuestasTitulo(string pRecursoId, string pPropiedad, string pTextoSinSeparar)
        {
            pTextoSinSeparar = pTextoSinSeparar.Replace("\"", "").Replace(((char)13).ToString() + ((char)10).ToString() + ((char)10).ToString() + ((char)13).ToString(), " ").Replace((char)10, ' ').Replace("\\", " ").Replace((char)13, ' ');

            StringBuilder triplesGenerados = new StringBuilder();
            int numeroTagsDespreciadosTitulo = 0;

            //Recorro los tags individuales
            List<string> miniTagsTitulo = AnalizadorSintactico.ObtenerTagsFrase(pTextoSinSeparar, out numeroTagsDespreciadosTitulo);

            if (miniTagsTitulo.Count + numeroTagsDespreciadosTitulo > 0)
            {
                foreach (string tagDescompuesto in miniTagsTitulo)
                {
                    if ((!tagDescompuesto.Contains("º")) && (!tagDescompuesto.Contains("ª")))
                    {
                        triplesGenerados.Append(FacetadoAD.GenerarTripleta("<http://gnoss/" + pRecursoId.ToUpper() + "> ", pPropiedad, "\"" + tagDescompuesto.ToLowerSearchGraph() + "\""));
                    }

                }
            }

            return triplesGenerados.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mTripletas"></param>
        /// <param name="mTripletasGnoss"></param>
        /// <param name="mTripletasContribuciones"></param>
        /// <param name="id"></param>
        /// <param name="proyid"></param>
        /// <param name="pPropiedad"></param>
        /// <param name="pTextoSinSeparar"></param>
        public static string AgregarTripletaDesnormalizadaTitulo(Guid pRecursoId, string pTitulo)
        {
            pTitulo = pTitulo.Replace("\"", "").Replace(((char)13).ToString() + ((char)10).ToString() + ((char)10).ToString() + ((char)13).ToString(), " ").Replace((char)10, ' ').Replace("\\", " ").Replace((char)13, ' ');

            return FacetadoAD.GenerarTripleta("<http://gnoss/" + pRecursoId.ToString().ToUpper() + "> ", "<http://gnoss/hasTituloDesc>", "\" " + pTitulo.ToLowerSearchGraph() + " \"");
        }

        public string GenerarTripletaRecogidadeVirtuoso_ControlCheckPoint(string pFicheroConfiguracionBD, string pFicheroConfiguracionBDBase, string pUrlIntragnoss, string pSujeto, string pPredicado, string pObjeto, string pObjetoSinMinuscula, List<string> Fecha, List<string> Numero, List<string> TextoInvariable, List<FacetaEntidadesExternas> EntExt, string pIdioma)
        {
            string tripletasDeVirtuoso = "";
            FacetadoAD pFacetadoAD = null;
            try
            {
                pFacetadoAD = new FacetadoAD(pFicheroConfiguracionBD, pUrlIntragnoss, "", mLoggingService, mEntityContext, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                pFacetadoAD.CadenaConexionBase = pFicheroConfiguracionBDBase;

                tripletasDeVirtuoso = pFacetadoAD.GenerarTripletaRecogidadeVirtuoso(pSujeto, pPredicado, pObjeto, pObjetoSinMinuscula, Fecha, Numero, TextoInvariable, EntExt, pIdioma);
            }
            catch (Exception)
            {
                //Cerramos las conexiones
                ControladorConexiones.CerrarConexiones();

                //Realizamos una consulta ask a virtuoso para comprobar si está funcionando
                while (!ServidorOperativo(pFicheroConfiguracionBD, pUrlIntragnoss))
                {
                    //Dormimos 30 segundos
                    Thread.Sleep(30 * 1000);
                }

                pFacetadoAD = new FacetadoAD(pFicheroConfiguracionBD, pUrlIntragnoss, "", mLoggingService, mEntityContext, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                pFacetadoAD.CadenaConexionBase = pFicheroConfiguracionBDBase;
                tripletasDeVirtuoso = pFacetadoAD.GenerarTripletaRecogidadeVirtuoso(pSujeto, pPredicado, pObjeto, pObjetoSinMinuscula, Fecha, Numero, TextoInvariable, EntExt, pIdioma);
            }
            finally
            {
                pFacetadoAD.Dispose();
                pFacetadoAD = null;
            }

            return tripletasDeVirtuoso;
        }


        public void GuardarIdentidadEnGrafoBusqueda(Identidad pIdentidad, Guid pProyectoID, string pUrlIntragnoss)
        {
            try
            {
                string triplesGrafoBusqueda = GenerarTriplesGrafoBusquedaIdentidad(pIdentidad, pProyectoID);

                if (!string.IsNullOrEmpty(triplesGrafoBusqueda))
                {
                    Dictionary<string, string> listaIdsEliminar = new Dictionary<string, string>();
                    listaIdsEliminar.Add(pIdentidad.Clave.ToString(), "rdf:type");

                    InsertarTriplesIdentidadGrafoBusqueda(pProyectoID, triplesGrafoBusqueda, listaIdsEliminar, pUrlIntragnoss);
                }
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex);
            }
        }


        private void InsertarTriplesIdentidadGrafoBusqueda(Guid pGrafo, string pTripletas, Dictionary<string, string> pListaElementosaModificarID, string pUrlIntragnoss)
        {
            FacetadoAD facetadoAD = null;
            bool insertado = false;

            try
            {
                facetadoAD = new FacetadoAD(pUrlIntragnoss, mLoggingService, mEntityContext, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                facetadoAD.InsertaTripletasConModify(pGrafo.ToString(), pTripletas, pListaElementosaModificarID);
                insertado = true;
            }
            catch
            {
                //Cerramos las conexiones
                ControladorConexiones.CerrarConexiones();

                //Realizamos una consulta ask a virtuoso para comprobar si está funcionando
                if (ServidorOperativo("", pUrlIntragnoss))
                {
                    try
                    {
                        facetadoAD = new FacetadoAD(pUrlIntragnoss, mLoggingService, mEntityContext, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);

                        if (!insertado)
                        {
                            facetadoAD.InsertaTripletasConModify(pGrafo.ToString(), pTripletas, pListaElementosaModificarID);
                            insertado = true;
                        }
                    }
                    catch (Exception exc)
                    {
                        mLoggingService.GuardarLogError(exc);
                    }
                }
            }
            finally
            {
                facetadoAD.Dispose();
                facetadoAD = null;
            }
        }


        /// <summary>
        /// Genera lo triples de la identidad para el grafo de búsqueda.
        /// </summary>
        /// <param name="pIdentidad"></param>
        /// <param name="pProyecto"></param>
        /// <returns>Cadena con los triples a insertar en el grafo de búsqueda</returns>
        public string GenerarTriplesGrafoBusquedaIdentidad(Identidad pIdentidad, Guid pProyectoID)
        {
            StringBuilder tripletas = new StringBuilder();

            string sujeto = "<http://gnoss/" + pIdentidad.Clave.ToString().ToUpper() + ">";
            string predicado = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>";
            string objeto = "\"Persona\"";
            tripletas.Append(FacetadoAD.GenerarTripleta(sujeto, predicado, objeto));


            tripletas.Append(GenerarTriplesIdentidad(pIdentidad, pProyectoID));

            return tripletas.ToString();
        }



        private string GenerarTriplesIdentidad(Identidad pIdentidad, Guid pProyectoID)
        {
            StringBuilder tripletas = new StringBuilder();
            string sujeto = "<http://gnoss/" + pIdentidad.Clave.ToString().ToUpper() + ">";
            string predicado = "<http://gnoss/hasnumerorecursos>";
            string objeto = "0";
            tripletas.Append(FacetadoAD.GenerarTripleta(sujeto, predicado, objeto));

            //Insertar Privacidad//
            predicado = "<http://gnoss/hasprivacidadCom>";
            objeto = ObtenerPrivacidad(pIdentidad.FilaIdentidad);
            tripletas.Append(FacetadoAD.GenerarTripleta(sujeto, predicado, objeto));

            predicado = "<http://gnoss/hasfechaAlta>";
            objeto = ObtenerFechaAlta(pIdentidad.FilaIdentidad.FechaAlta);
            tripletas.Append(FacetadoAD.GenerarTripleta(sujeto, predicado, objeto));

            predicado = "<http://xmlns.com/foaf/0.1/familyName>";
            objeto = "\"" + pIdentidad.Persona.Apellidos.ToLowerSearchGraph() + "\"";
            tripletas.Append(FacetadoAD.GenerarTripleta(sujeto, predicado, objeto));

            predicado = "<http://gnoss/hasPerfil>";
            objeto = "<http://gnoss/" + pIdentidad.PerfilID.ToString().ToUpper() + ">";
            tripletas.Append(FacetadoAD.GenerarTripleta(sujeto, predicado, objeto));

            predicado = "<http://gnoss/hasfoto>";
            objeto = "\"" + pIdentidad.FilaIdentidad.Foto + "\"";
            tripletas.Append(FacetadoAD.GenerarTripleta(sujeto, predicado, objeto));

            predicado = "<http://xmlns.com/foaf/0.1/firstName>";
            objeto = "\"" + pIdentidad.NombreCorto.ToLowerSearchGraph() + "\"";
            tripletas.Append(FacetadoAD.GenerarTripleta(sujeto, predicado, objeto));

            predicado = "<http://gnoss/hasPerfilProyecto>";
            objeto = "<htt://gnoss/" + pIdentidad.PerfilID.ToString().ToUpper() + pProyectoID.ToString().ToUpper() + ">";
            tripletas.Append(FacetadoAD.GenerarTripleta(sujeto, predicado, objeto));

            predicado = "<http://gnoss/hasnombrecompleto>";
            objeto = "\"" + pIdentidad.NombreCorto.ToLowerSearchGraph() + " " + pIdentidad.Persona.Apellidos.ToLowerSearchGraph() + "\"";
            tripletas.Append(FacetadoAD.GenerarTripleta(sujeto, predicado, objeto));

            predicado = "<http://gnoss/nombreCortoUsu>";
            objeto = "\"" + pIdentidad.PerfilUsuario.NombreCortoUsu + "\"";
            tripletas.Append(FacetadoAD.GenerarTripleta(sujeto, predicado, objeto));

            predicado = "<http://gnoss/search> ";
            objeto = "\" " + pIdentidad.NombreCorto.ToLowerSearchGraph() + " " + pIdentidad.Persona.Apellidos.ToLowerSearchGraph() + " \"";
            tripletas.Append(FacetadoAD.GenerarTripleta(sujeto, predicado, objeto));

            predicado = "<http://rdfs.org/sioc/ns#has_space>";
            objeto = "<http://gnoss/" + pProyectoID.ToString().ToUpper() + ">";
            tripletas.Append(FacetadoAD.GenerarTripleta(sujeto, predicado, objeto));

            predicado = "<http://gnoss/hasPopularidad>";
            objeto = pIdentidad.FilaIdentidad.Rank.ToString();
            tripletas.Append(FacetadoAD.GenerarTripleta(sujeto, predicado, objeto));

            if (pProyectoID.Equals(ProyectoAD.MetaProyecto))
            {
                predicado = "<http://gnoss/rol>";
                string tipoRol = DevolverTipoRolUsuario(pIdentidad);
                objeto = tipoRol;
                tripletas.Append(FacetadoAD.GenerarTripleta(sujeto, predicado, objeto));
            }

            predicado = "<http://gnoss/userstatus>";
            objeto = ObtenerUserStatus(pIdentidad);
            tripletas.Append(FacetadoAD.GenerarTripleta(sujeto, predicado, objeto));

            Guid usuarioID = pIdentidad.Persona.UsuarioID;
            predicado = "<http://gnoss/hasIdentidadID>";
            string sujetoUsuarioID = "<http://gnoss/" + pIdentidad.Persona.UsuarioID.ToString().ToUpper() + ">";
            tripletas.Append(FacetadoAD.GenerarTripleta(sujetoUsuarioID, predicado, sujeto));

            GenerarTriplesDatosExtraIdentidad(pIdentidad, pProyectoID, tripletas);

            return tripletas.ToString();
        }

        private void GenerarTriplesDatosExtraIdentidad(Identidad pIdentidad, Guid pProyectoID, StringBuilder pTripletas)
        {
            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            DataWrapperIdentidad identidadDW = identidadCN.ObtenerDatosExtraProyectoOpcionIdentidadPorIdentidadID(pIdentidad.Clave);

            if (identidadDW.ListaDatoExtraEcosistemaOpcionPerfil.Count > 0 || identidadDW.ListaDatoExtraProyectoOpcionIdentidad.Count > 0)
            {
                /*ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                DataWrapperProyecto datoExtraDW = proyectoCN.ObtenerDatosExtraProyectoPorID(pProyectoID);
                proyectoCN.Dispose();
                string opcion = null;*/

                ActualizacionFacetadoCN actualizacionFacetadoCN = new ActualizacionFacetadoCN(null, null, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                Guid? id = actualizacionFacetadoCN.ObtieneIDIdentidad(pProyectoID, pIdentidad.Persona.Clave, false);
                string idRecursoMayuscula = id.Value.ToString().ToUpper();
                string idRecursoMinuscula = id.Value.ToString();

                List<QueryTriples> listaTriplesPersona = actualizacionFacetadoCN.ObtieneDatosExtraIdentidad(id.Value, pProyectoID);

                foreach (QueryTriples query in listaTriplesPersona)
                {
                    if (!string.IsNullOrEmpty(query.Objeto))
                    {
                        string predicado = query.Predicado;
                        string objeto = (string)query.Objeto;

                        pTripletas.Append(FacetadoAD.GenerarTripleta((string)query.Sujeto.Replace(idRecursoMinuscula, idRecursoMayuscula), predicado, objeto));

                    }
                }

                /*if (identidadDW.ListaDatoExtraEcosistemaOpcionPerfil.Count > 0)
                {
                    var datoExtraOpcion = identidadDW.ListaDatoExtraEcosistemaOpcionPerfil.FirstOrDefault();
                    var datoExtra = datoExtraDW.ListaDatoExtraEcosistemaOpcion.First(item => item.OpcionID.Equals(datoExtraOpcion.OpcionID));
                    opcion = datoExtra.Opcion.ToLowerSearchGraph();
                }
                else if (identidadDW.ListaDatoExtraEcosistemaVirtuosoPerfil.Count > 0)
                {
                    var datoExtraOpcion = identidadDW.ListaDatoExtraProyectoOpcionIdentidad.FirstOrDefault();
                    var datoExtra = datoExtraDW.ListaDatoExtraProyectoOpcion.First(item => item.OpcionID.Equals(datoExtraOpcion.OpcionID));
                    opcion = datoExtra.Opcion.ToLowerSearchGraph();
                }

                if (!string.IsNullOrEmpty(opcion))
                {
                    string sujeto = "<http://gnoss/" + pIdentidad.Clave.ToString().ToUpper() + ">";
                    string predicado = "<http://purl.org/dc/elements/1.1/type>";
                    if (opcion.Contains("|||"))
                    {
                        Dictionary<string, string> idiomas = UtilCadenas.ObtenerTextoPorIdiomas(opcion);
                        foreach (string idioma in idiomas.Keys)
                        {
                            string objeto = $"\"{idiomas[idioma]}\"";
                            pTripletas.Append(FacetadoAD.GenerarTripleta(sujeto, predicado, objeto, idioma));
                        }
                    }
                    else
                    {
                        string objeto = $"\"{opcion}\"";
                        pTripletas.Append(FacetadoAD.GenerarTripleta(sujeto, predicado, objeto));
                    }
                }*/
            }
        }

        private static string ObtenerUserStatus(Identidad pIdentidad)
        {
            if (pIdentidad.FilaIdentidad.FechaBaja.HasValue)
            {
                return "3";
            }
            else if (!pIdentidad.FilaIdentidad.FechaExpulsion.HasValue && pIdentidad.Usuario != null && pIdentidad.Usuario.EstaBloqueado)
            {
                return "2";
            }
            else
            {
                return "1";
            }
        }

        private static string ObtenerFechaAlta(DateTime fechaAlta)
        {
            return (fechaAlta.ToString("yyyyMMddhhmmss"));
        }

        private static string ObtenerPrivacidad(AD.EntityModel.Models.IdentidadDS.Identidad filaIdentidad)
        {
            string objeto;
            try
            {
                string fechaBaja = !filaIdentidad.FechaBaja.HasValue ? null : filaIdentidad.FechaBaja.Value.ToString();
                string fechaExpulsion = !filaIdentidad.FechaExpulsion.HasValue ? null : filaIdentidad.FechaExpulsion.Value.ToString();

                if (fechaBaja == null && fechaExpulsion == null && filaIdentidad.ActivoEnComunidad)
                {
                    objeto = "\"publico\"";
                }
                else
                {
                    objeto = "\"privado\"";
                }
            }
            catch (Exception)
            {
                objeto = "\"publico\"";
            }
            return objeto;
        }

        private static string DevolverTipoRolUsuario(Identidad pIdentidad)
        {
            //if (pIdentidad.EsAdministrador)
            //{
            //    return "0";
            //}
            //else if (pIdentidad.EsDiseñador)
            //{
            //    return TipoRolUsuario.Diseñador.ToString();
            //}
            //else if (pIdentidad.EsSupervisor)
            //{
            //    return TipoRolUsuario.Supervisor.ToString();
            //}
            //else
            {
                return "2";
            }
        }


        /// <summary>
        /// Genera lo triples del documento para el grafo de búsqueda.
        /// </summary>
        /// <param name="pDocumento"></param>
        /// <param name="pProyecto"></param>
        /// <param name="pRdfConfiguradoRecursoNoSemantico">Para que recursos no semánticos tengan rdfType</param>
        /// <param name="pListaTriplesRecurso">Lista de triples previamente insertados en el grafo de ontología</param>
        /// <param name="pOntologia"></param>
        /// <param name="pGestorTesauro"></param>
        /// <param name="pFicheroConfiguracionBD"></param>
        /// <param name="pFicheroConfiguracionBDBase"></param>
        /// <param name="pUrlIntragnoss"></param>
        /// <returns>Cadena con los triples a insertar en el grafo de búsqueda</returns>
        public string GenerarTriplesGrafoBusquedaRecurso(Documento pDocumento, Elementos.ServiciosGenerales.Proyecto pProyecto, string pRdfConfiguradoRecursoNoSemantico, List<TripleWrapper> pListaTriplesRecurso, Ontologia pOntologia, GestionTesauro pGestorTesauro, string pFicheroConfiguracionBD, string pFicheroConfiguracionBDBase, string pUrlIntragnoss)
        {
            StringBuilder tripletas = new StringBuilder();
            StringBuilder valorSearch = new StringBuilder();

            if (!pDocumento.EsBorrador)
            {
                valorSearch.Append(ObtenerSearchDescripcionRecurso(pDocumento.Descripcion));
                if (!string.IsNullOrEmpty(pDocumento.Autor))
                {
                    valorSearch.Append(pDocumento.Autor.Replace(',', ' '));
                }
                tripletas.Append(GenerarTriplesTagsRecurso(pDocumento.Clave, pDocumento.ListaTagsSoloLectura, ref valorSearch));

                List<AD.EntityModel.Models.Documentacion.DocumentoWebAgCatTesauro> listaCategorias = pDocumento.GestorDocumental.DataWrapperDocumentacion.ListaDocumentoWebAgCatTesauro.Where(item => item.DocumentoID.Equals(pDocumento.Clave) && pGestorTesauro.ListaCategoriasTesauro.ContainsKey(item.CategoriaTesauroID)).ToList();
                tripletas.Append(GenerarTripletasCategoriasRecurso(pDocumento.Clave, pProyecto.Clave, listaCategorias, pGestorTesauro, pUrlIntragnoss, false, ref valorSearch));

                if (pDocumento.FilaDocumentoWebVinBR != null)
                {
                    tripletas.Append(GenerarTripletaComparticionAutomaticaRecurso(pDocumento.Clave, pProyecto.Clave, pDocumento.FilaDocumentoWebVinBR.TipoPublicacion));
                }
                tripletas.Append(GenerarTripletasAutoresRecurso(pDocumento.Clave, pDocumento.Autor));
                tripletas.Append(GenerarTripletasInformacionComunRecurso(pDocumento, pProyecto.TipoAcceso, pProyecto.Clave));
                tripletas.Append(GenerarTripletasInformacionExtraRecurso(pDocumento, pProyecto));
                tripletas.Append(GenerarTripletasRecursoPorTipoDocumento(pFicheroConfiguracionBD, pFicheroConfiguracionBDBase, pUrlIntragnoss, pDocumento, pProyecto.Clave, pRdfConfiguradoRecursoNoSemantico, pListaTriplesRecurso, pOntologia));

                GenerarTriplesTituloRecurso(tripletas, pDocumento.Clave, pDocumento.Titulo, ref valorSearch);

                string triplesSearch = valorSearch.ToString();
                if (!string.IsNullOrEmpty(triplesSearch))
                {
                    tripletas.Append(GenerarTriplesSearchRecurso(pDocumento.Clave, triplesSearch));
                }
            }

            return tripletas.ToString();
        }

        public void GuardarRecursoEnGrafoBusqueda(Documento pDocumento, bool pEsDocRaiz, List<Documento> pDocumentosExtraGuardar, Elementos.ServiciosGenerales.Proyecto pProyecto, List<TripleWrapper> pListaTriplesSemanticos, Ontologia pOntologia, GestionTesauro pGestorTesauro, string pRdfConfiguradoRecursoNoSemantico, bool pCreandoVersion, Guid? pDocumentoOriginalID, string pUrlIntragnoss, string pOtrosArgumentosBase, PrioridadBase pPrioridadBase, StringBuilder pSbVirtuoso = null)
        {
            string triplesGrafoBusqueda = "";
            bool insercionCorrecta = false;

            try
            {
                triplesGrafoBusqueda = GenerarTriplesGrafoBusquedaRecurso(pDocumento, pProyecto, pRdfConfiguradoRecursoNoSemantico, pListaTriplesSemanticos, pOntologia, pGestorTesauro, null, null, pUrlIntragnoss);
                if (!string.IsNullOrEmpty(triplesGrafoBusqueda))
                {
                    if (pSbVirtuoso != null)
                    {
                        //pSbVirtuoso
                        StringReader stReader = new StringReader(triplesGrafoBusqueda);
                        string linea = "";
                        while ((linea = stReader.ReadLine()) != null)
                        {

                            if (linea.TrimStart().StartsWith("<http"))
                            {
                                pSbVirtuoso.AppendLine($"<{pUrlIntragnoss}{pProyecto.Clave.ToString()}> {linea}");
                            }
                            else if (!string.IsNullOrEmpty(linea.Trim(' ')))
                            {
                                pSbVirtuoso.AppendLine(linea);
                            }
                        }
                    }
                    else
                    {
                        Dictionary<string, string> listaIdsEliminar = new Dictionary<string, string>();
                        listaIdsEliminar.Add(pDocumento.Clave.ToString(), "rdf:type");

                        insercionCorrecta = InsertarTriplesGrafoBusqueda(pDocumento.Clave, pProyecto.Clave, pCreandoVersion, pDocumentoOriginalID, triplesGrafoBusqueda, listaIdsEliminar, "", "", (short)pDocumento.TipoDocumentacion, pUrlIntragnoss, pOtrosArgumentosBase, pPrioridadBase);

                        if (insercionCorrecta /*&& pDocumento.TipoDocumentacion.Equals(TiposDocumentacion.Semantico)*/)
                        {
                            //insertar al Base semánticos para que genere lo que falta (search con metatags ontologia...), resto para que genere grafo contribuciones
                            AgregarRecursoModeloBaseSimple(pDocumento.Clave, pProyecto.Clave, (short)pDocumento.TipoDocumentacion, null, null, pOtrosArgumentosBase, PrioridadBase.Alta, false, -1, (short)TiposElementosEnCola.InsertadoEnGrafoBusquedaDesdeWeb);
                            InvalidarCacheGuardarRecursoEnGrafoBusqueda(pDocumento, pProyecto, pUrlIntragnoss, pPrioridadBase);
                        }
                    }
                }
                else if (pDocumento.EsBorrador)
                {
                    AgregarRecursoModeloBaseSimple(pDocumento.Clave, pProyecto.Clave, (short)pDocumento.TipoDocumentacion, null, null, pOtrosArgumentosBase, PrioridadBase.Alta, false, -1, (short)TiposElementosEnCola.InsertadoEnGrafoBusquedaDesdeWeb);
                    InvalidarCacheGuardarRecursoEnGrafoBusqueda(pDocumento, pProyecto, pUrlIntragnoss, pPrioridadBase);
                }
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex);
            }

            //ya se hace en InsertarTriplesGrafoBusqueda
            if (string.IsNullOrEmpty(triplesGrafoBusqueda) || !insercionCorrecta)
            {
                if (pCreandoVersion && pDocumentoOriginalID.HasValue)
                {
                    AgregarRecursoEliminadoModeloBaseSimple(pDocumentoOriginalID.Value, pProyecto.Clave, (short)pDocumento.TipoDocumentacion, PrioridadBase.Alta);
                }

                //ha fallado la inserción directa, insertar fila en el BASE para que genere la info en el grafo de búsqueda
                AgregarRecursoModeloBaseSimple(pDocumento.Clave, pProyecto.Clave, (short)pDocumento.TipoDocumentacion, null, null, pOtrosArgumentosBase, pPrioridadBase, false, -1, null);
            }

            //27/02/2017 ahora las entidades externas se añaden al grafo de búsqueda en GuardarEntidadExternaDePropSeleccEntExtEditable
            //if (pEsDocRaiz && pDocumento.TipoDocumentacion.Equals(TiposDocumentacion.Semantico) && pDocumentosExtraGuardar != null)
            //{
            //    foreach (Documento docExtra in pDocumentosExtraGuardar)
            //    {
            //        //GuardarRecursoEnGrafoBusqueda(docExtra, false, pDocumentosExtraGuardar, pProyecto, pListaTriplesSemanticos, pOntologia, pGestorTesauro, pRdfConfiguradoRecursoNoSemantico, pCreandoVersion, pDocumentoOriginalID, pUrlIntragnoss, pOtrosArgumentosBase, pPrioridadBase);

            //        //no tenemos los triples semanticos de la entidad auxiliar para la inserción directa, insertar fila en el BASE para que genere la info en el grafo de búsqueda
            //        AgregarRecursoModeloBaseSimple(docExtra.Clave, pProyecto.Clave, (short)docExtra.TipoDocumentacion, null, null, pOtrosArgumentosBase, pPrioridadBase, false, -1, null);
            //    }
            //}
        }

        public void GuardarEdicionTagsRecursoEnGrafoBusqueda(Documento pDocumento, Guid pProyectoId, string pUrlIntragnoss, PrioridadBase pPrioridadBase, bool pEliminadoTags)
        {
            string triplesGrafoBusqueda = "";
            bool insercionCorrecta = false;

            try
            {
                triplesGrafoBusqueda = GenerarTriplesEdicionTagsRecursoGrafoBusqueda(pDocumento, pProyectoId, pUrlIntragnoss);

                if (!string.IsNullOrEmpty(triplesGrafoBusqueda))
                {
                    insercionCorrecta = InsertarTriplesEdicionTagsCategoriasSearchRecurso(pDocumento.Clave, pProyectoId, triplesGrafoBusqueda, pUrlIntragnoss, pPrioridadBase, pEliminadoTags, false);
                }
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex);
            }

            //si ha ido bien hay que insertar en el Base para generar el grafo de contribuciones
            TiposElementosEnCola tipoElemento = TiposElementosEnCola.InsertadoEnGrafoBusquedaDesdeWeb;

            if (!insercionCorrecta)
            {
                //ha fallado la inserción, notificar al base para que lo genere él
                tipoElemento = TiposElementosEnCola.Agregado;
            }

            AgregarRecursoModeloBaseSimple(pDocumento.Clave, pProyectoId, (short)pDocumento.TipoDocumentacion, null, null, "", pPrioridadBase, false, -1, (short)tipoElemento);
        }

        public void GuardarEdicionCategoriasRecursoEnGrafoBusqueda(Documento pDocumento, Guid pProyectoId, bool pEliminandoCategorias, string pUrlIntragnoss, PrioridadBase pPrioridadBase)
        {
            string triplesGrafoBusqueda = "";
            StringBuilder valorSearch = new StringBuilder();
            bool insercionCorrecta = false;

            try
            {
                AD.EntityModel.Models.Documentacion.BaseRecursosProyecto filaBRProyecto = pDocumento.GestorDocumental.DataWrapperDocumentacion.ListaBaseRecursosProyecto.FirstOrDefault(br => br.ProyectoID.Equals(pProyectoId));

                if (filaBRProyecto != null)
                {
                    List<AD.EntityModel.Models.Documentacion.DocumentoWebAgCatTesauro> listaFilasCategorias = pDocumento.GestorDocumental.DataWrapperDocumentacion.ListaDocumentoWebAgCatTesauro.Where(cat => cat.BaseRecursosID.Equals(filaBRProyecto.BaseRecursosID)).ToList();

                    if (listaFilasCategorias != null && listaFilasCategorias.Count > 0)
                    {
                        triplesGrafoBusqueda = GenerarTripletasCategoriasRecurso(pDocumento.Clave, pProyectoId, listaFilasCategorias, pDocumento.GestorDocumental.GestorTesauro, pUrlIntragnoss, true, ref valorSearch);

                        if (!string.IsNullOrEmpty(triplesGrafoBusqueda))
                        {
                            insercionCorrecta = InsertarTriplesEdicionTagsCategoriasSearchRecurso(pDocumento.Clave, pProyectoId, triplesGrafoBusqueda, pUrlIntragnoss, pPrioridadBase, false, pEliminandoCategorias);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex);
            }

            //si ha ido bien hay que insertar en el Base para generar el grafo de contribuciones
            TiposElementosEnCola tipoElemento = TiposElementosEnCola.InsertadoEnGrafoBusquedaDesdeWeb;

            if (!insercionCorrecta)
            {
                //ha fallado la inserción, notificar al base para que lo genere él
                tipoElemento = TiposElementosEnCola.Agregado;
            }

            AgregarRecursoModeloBaseSimple(pDocumento.Clave, pProyectoId, (short)pDocumento.TipoDocumentacion, null, null, "", pPrioridadBase, false, -1, (short)tipoElemento);
        }

        public bool InsertarTriplesEdicionTagsCategoriasSearchRecurso(Guid pDocumentoId, Guid pGrafo, string pTripletas, string pUrlIntragnoss, PrioridadBase pPrioridadBase, bool pEliminadoTags, bool pEliminandoCategorias)
        {
            FacetadoAD facetadoAD = null;
            bool insertado = false;

            try
            {
                facetadoAD = new FacetadoAD(pUrlIntragnoss, mLoggingService, mEntityContext, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);


                facetadoAD.InsertarTriplesEdicionTagsCategoriasSearchRecurso(pDocumentoId.ToString(), pGrafo.ToString(), pTripletas, pPrioridadBase, pEliminadoTags, pEliminandoCategorias);
                insertado = true;
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex);

                //Cerramos las conexiones
                ControladorConexiones.CerrarConexiones();

                if (ServidorOperativo("", pUrlIntragnoss))
                {
                    try
                    {
                        facetadoAD = new FacetadoAD(pUrlIntragnoss, mLoggingService, mEntityContext, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);


                        if (!insertado)
                        {
                            facetadoAD.InsertarTriplesEdicionTagsCategoriasSearchRecurso(pDocumentoId.ToString(), pGrafo.ToString(), pTripletas, pPrioridadBase, pEliminadoTags, pEliminandoCategorias);
                            insertado = true;
                        }
                    }
                    catch (Exception exc)
                    {
                        mLoggingService.GuardarLogError(exc);
                    }
                }
            }
            finally
            {
                facetadoAD.Dispose();
                facetadoAD = null;
            }

            return insertado;
        }

        public string LeerSearchDeVirtuoso(Guid pDocumentoId, Guid pProyectoId, string pUrlIntragnoss)
        {
            if (!pUrlIntragnoss.EndsWith("/"))
            {
                pUrlIntragnoss += "/";
            }

            string valorSearch = "";
            string sujeto = "<http://gnoss/" + pDocumentoId.ToString().ToUpper() + ">";
            string predicadoSearch = "<http://gnoss/search>";
            string from = " FROM <" + pUrlIntragnoss + pProyectoId.ToString().ToLower() + ">";
            string where = " WHERE {" + sujeto + " " + predicadoSearch + " " + "?search.}";
            string querySPARQL = "SELECT ?search" + from + where;

            FacetadoCN facCN = new FacetadoCN(pUrlIntragnoss, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            FacetadoDS dataSetResult = facCN.RealizarConsultaAVirtuoso(pProyectoId.ToString().ToLower(), querySPARQL);
            if (dataSetResult != null && dataSetResult.Tables["Consulta"] != null && dataSetResult.Tables["Consulta"].Rows.Count > 0)
            {
                valorSearch = " " + dataSetResult.Tables["Consulta"].Rows[0]["search"].ToString();
            }
            facCN.Dispose();

            return valorSearch;
        }

        #region Privados

        private void InvalidarCacheGuardarRecursoEnGrafoBusqueda(Documento pDocumento, Elementos.ServiciosGenerales.Proyecto pProyecto, string pUrlIntragnoss, PrioridadBase pPrioridadBase)
        {
            string tipoSemantico = "";
            if (pDocumento.TipoDocumentacion.Equals(TiposDocumentacion.Semantico) && pDocumento.GestorDocumental != null && pDocumento.GestorDocumental.ListaDocumentos.ContainsKey(pDocumento.ElementoVinculadoID))
            {
                tipoSemantico = pDocumento.GestorDocumental.ListaDocumentos[pDocumento.ElementoVinculadoID].Enlace.Replace(".owl", "");
                if (tipoSemantico.Contains("/"))
                {
                    tipoSemantico = tipoSemantico.Substring(tipoSemantico.LastIndexOf("/") + 1);
                }
            }

            string dominio = pUrlIntragnoss.Replace("http://", "").Replace("www.", "");
            if (dominio.EndsWith("/"))
            {
                dominio = dominio.Substring(0, dominio.Length - 1);
            }

            //si se llama desde el ApiRecursos sólo invalida caché el último de la carga que llegará con prioridad 21 (PrioridadBase.ApiRecursosBorrarCache)
            if ((short)pPrioridadBase < 11 || (short)pPrioridadBase > 20)
            {
                if (pProyecto.FilaProyecto.NumeroRecursos > 100000)
                {
                    BaseComunidadCN baseComunidadCN = new BaseComunidadCN("base", mEntityContext, mLoggingService, mEntityContextBase, mConfigService, mServicesUtilVirtuosoAndReplication);
                    try
                    {
                        baseComunidadCN.InsertarFilaColaRefrescoCacheEnRabbitMQ(pProyecto.Clave, TiposEventosRefrescoCache.BusquedaVirtuoso, TipoBusqueda.Recursos, null);
                    }
                    catch (Exception ex)
                    {
                        mLoggingService.GuardarLogError(ex, "Fallo al insertar en Rabbit, insertamos en la base de datos BASE, tabla colaRefrescoCache");
                        baseComunidadCN.InsertarFilaEnColaRefrescoCache(pProyecto.Clave, TiposEventosRefrescoCache.BusquedaVirtuoso, TipoBusqueda.Recursos);
                    }

                    if (tipoSemantico != "")
                    {
                        try
                        {
                            baseComunidadCN.InsertarFilaColaRefrescoCacheEnRabbitMQ(pProyecto.Clave, TiposEventosRefrescoCache.BusquedaVirtuoso, TipoBusqueda.Recursos, $"rdf:type={tipoSemantico}");
                        }
                        catch (Exception ex)
                        {
                            mLoggingService.GuardarLogError(ex, "Fallo al insertar en Rabbit, insertamos en la base de datos BASE, tabla colaRefrescoCache");
                            baseComunidadCN.InsertarFilaEnColaRefrescoCache(pProyecto.Clave, TiposEventosRefrescoCache.BusquedaVirtuoso, TipoBusqueda.Recursos, $"rdf:type={tipoSemantico}");
                        }
                    }
                    baseComunidadCN.Dispose();
                }
                else
                {
                    FacetadoCL facetadoCL = new FacetadoCL(pUrlIntragnoss, mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                    facetadoCL.Dominio = dominio;
                    facetadoCL.InvalidarResultadosYFacetasDeBusquedaEnProyecto(pProyecto.Clave, FacetadoAD.TipoBusquedaToString(TipoBusqueda.Recursos));
                    facetadoCL.BorrarRSSDeComunidad(pProyecto.Clave);

                    /*if (tipoSemantico != "")
                    {
                        facetadoCL.InvalidarResultadosYFacetasDeBusquedaEnProyecto(pProyecto.Clave, "rdf:type=" + tipoSemantico);
                    }*/

                    facetadoCL.Dispose();

                    BaseComunidadCN baseComunidadCN = new BaseComunidadCN("base", mEntityContext, mLoggingService, mEntityContextBase, mConfigService, mServicesUtilVirtuosoAndReplication);
                    try
                    {
                        baseComunidadCN.InsertarFilaColaRefrescoCacheEnRabbitMQ(pProyecto.Clave, TiposEventosRefrescoCache.RefrescarComponentesRecursos, TipoBusqueda.Recursos, null);
                    }
                    catch (Exception ex)
                    {
                        mLoggingService.GuardarLogError(ex, "Fallo al insertar en Rabbit, insertamos en la base de datos BASE, tabla colaRefrescoCache");
                        baseComunidadCN.InsertarFilaEnColaRefrescoCache(pProyecto.Clave, TiposEventosRefrescoCache.RefrescarComponentesRecursos, TipoBusqueda.Recursos);
                    }

                    baseComunidadCN.Dispose();
                }

                //Metodo para hacer que no pregunte cada vez a base de datos los contadores.
                InsertarContadores(dominio, pProyecto);
            }

            DocumentacionCL docCLRec = new DocumentacionCL("recursos", mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
            docCLRec.Dominio = dominio;
            docCLRec.InvalidarFichaRecursoMVC(pDocumento.Clave, pProyecto.Clave);
            docCLRec.Dispose();
        }

        private void InsertarContadores(string dominio, Elementos.ServiciosGenerales.Proyecto pProyecto)
        {
            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

            int refrescoNumeroResultados = 0;
            ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            proyCL.Dominio = dominio;

            DateTime fechaActual = DateTime.UtcNow;

            if (mProyectosActualizarContadores == null)
            {
                mProyectosActualizarContadores = new ConcurrentDictionary<Guid, DateTime>();
                refrescoNumeroResultados = proyCN.ObtenerNumRecursosProyecto(pProyecto.Clave);
                mProyectosActualizarContadores.TryAdd(pProyecto.Clave, fechaActual);
            }
            else
            {
                if (mProyectosActualizarContadores.ContainsKey(pProyecto.Clave))
                {
                    DateTime fechaUltimaModificacion = mProyectosActualizarContadores[pProyecto.Clave];
                    TimeSpan diferencia = fechaActual - fechaUltimaModificacion;
                    if (diferencia.Days > 1)
                    {
                        refrescoNumeroResultados = proyCN.ObtenerNumRecursosProyecto(pProyecto.Clave);
                    }
                    else
                    {
                        refrescoNumeroResultados = proyCL.ObtenerContadorComunidad(pProyecto.Clave, TipoBusqueda.Recursos).Value;
                        refrescoNumeroResultados++;
                    }
                    mProyectosActualizarContadores.TryUpdate(pProyecto.Clave, fechaActual, fechaUltimaModificacion);
                }
                else
                {
                    refrescoNumeroResultados = proyCN.ObtenerNumRecursosProyecto(pProyecto.Clave);
                    mProyectosActualizarContadores.TryAdd(pProyecto.Clave, fechaActual);
                }
            }

            proyCN.Dispose();
            proyCL.AgregarContadorComunidad(pProyecto.Clave, TipoBusqueda.Recursos, refrescoNumeroResultados);
        }

        private string GenerarTriplesEdicionTagsRecursoGrafoBusqueda(Documento pDocumento, Guid pProyectoId, string pUrlIntragnoss)
        {
            StringBuilder triplesInsertar = new StringBuilder();
            StringBuilder valorSearch = new StringBuilder();
            string sujeto = "<http://gnoss/" + pDocumento.Clave.ToString().ToUpper() + ">";
            string predicadoSearch = "<http://gnoss/search>";

            //generar triples tags y search(leerlo previamente) e insertarlo todo junto
            List<string> listaTags = pDocumento.Tags.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            foreach (string tag in listaTags)
            {
                string objeto = "\"" + tag.Replace("\"", "'").Trim() + "\" .";
                triplesInsertar.Append(FacetadoAD.GenerarTripleta(sujeto, "<http://rdfs.org/sioc/types#Tag>", objeto));
                valorSearch.Append(" " + tag);
            }

            valorSearch.Append(" " + LeerSearchDeVirtuoso(pDocumento.Clave, pProyectoId, pUrlIntragnoss));

            string tripleSearch = UtilCadenas.EliminarHtmlDeTextoPorEspacios(valorSearch.ToString()).Replace("\\", "/");
            tripleSearch = "\" " + tripleSearch.Replace("\"", "'").Trim() + " \" .";
            tripleSearch = tripleSearch.ToLower();

            if (!string.IsNullOrEmpty(tripleSearch))
            {
                triplesInsertar.Append(FacetadoAD.GenerarTripleta(sujeto, predicadoSearch, UtilidadesVirtuoso.RemoverSignosSearch(UtilidadesVirtuoso.RemoverSignosAcentos(tripleSearch))));
            }

            return triplesInsertar.ToString();
        }

        private static string GenerarTriplesSearchRecurso(Guid pDocumentoId, string pValorSearch)
        {
            pValorSearch = UtilCadenas.EliminarHtmlDeTextoPorEspacios(pValorSearch).Replace("\\", "/");
            pValorSearch = "\" " + pValorSearch.Replace("\"", "'").Trim() + " \" .";
            pValorSearch = pValorSearch.ToLower();

            string tripleSearch = FacetadoAD.GenerarTripleta("<http://gnoss/" + pDocumentoId.ToString().ToUpper() + ">", "<http://gnoss/search>", RemoverSignosSearch(RemoverSignosAcentos(pValorSearch)));

            return tripleSearch;
        }

        private bool InsertarTriplesGrafoBusqueda(Guid pDocumentoId, Guid pGrafo, bool pEstaVersionando, Guid? pDocumentoOriginalID, string pTripletas, Dictionary<string, string> pListaElementosaModificarID, string pCondicionesWhere, string pCondicionesFilter, short pTipoDocumento, string pUrlIntragnoss, string pOtrosArgumentosBase, PrioridadBase pPrioridadBase)
        {
            FacetadoAD facetadoAD = null;
            bool insertado = false;
            bool eliminado = false;

            try
            {
                facetadoAD = new FacetadoAD(pUrlIntragnoss, mLoggingService, mEntityContext, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);


                if (pEstaVersionando && pDocumentoOriginalID.HasValue)
                {
                    facetadoAD.BorrarRecurso(pGrafo.ToString(), pDocumentoOriginalID.Value, (int)PrioridadBase.Alta, "", false);
                    eliminado = true;
                }

                facetadoAD.InsertaTripletasConModify(pGrafo.ToString(), pTripletas, pListaElementosaModificarID, pCondicionesWhere, pCondicionesFilter, true, 0, pTipoDocumento.Equals((short)TiposDocumentacion.Semantico));
                insertado = true;
            }
            catch (Exception ex)
            {
                //Cerramos las conexiones
                ControladorConexiones.CerrarConexiones();

                //Realizamos una consulta ask a virtuoso para comprobar si está funcionando
                if (ServidorOperativo("", pUrlIntragnoss))
                {
                    try
                    {
                        facetadoAD = new FacetadoAD(pUrlIntragnoss, mLoggingService, mEntityContext, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);


                        if (pEstaVersionando && !eliminado && pDocumentoOriginalID.HasValue)
                        {
                            facetadoAD.BorrarRecurso(pGrafo.ToString(), pDocumentoOriginalID.Value, (int)PrioridadBase.Alta);
                        }

                        if (!insertado)
                        {
                            facetadoAD.InsertaTripletasConModify(pGrafo.ToString(), pTripletas, pListaElementosaModificarID, pCondicionesWhere, pCondicionesFilter, true, 0, pTipoDocumento.Equals(TiposDocumentacion.Semantico));
                            insertado = true;
                        }
                    }
                    catch (Exception exc)
                    {
                        mLoggingService.GuardarLogError(exc);
                        //if (pEstaVersionando && !eliminado && pDocumentoOriginalID.HasValue)
                        //{
                        //    AgregarRecursoEliminadoModeloBaseSimple(pDocumentoOriginalID.Value, pGrafo, pTipoDocumento, PrioridadBase.Alta);
                        //}

                        ////ha fallado la inserción directa, insertar fila en el BASE para que genere la info en el grafo de búsqueda
                        //AgregarRecursoModeloBaseSimple(pDocumentoId, pGrafo, pTipoDocumento, null, null, pOtrosArgumentosBase, PrioridadBase.Alta, false, -1, null);
                    }
                }
            }
            finally
            {
                facetadoAD.Dispose();
                facetadoAD = null;
            }

            return insertado;
        }

        /// <summary>
        /// Notifica al modelo base que se han modificado una serie de documentos.
        /// </summary>
        /// <param name="pDocumentoID">Lista de documentos con su tipo</param>
        /// <param name="pProyectoID">Proyecto en el que se han modificado</param>
        /// <param name="pTipo">Tipo de documento</param>
        /// <param name="pBaseRecursosComDS">Dataset de base recursos comunidad</param>
        /// <param name="pFicheroConfiguracionBD">Fichero configuración BD</param>
        /// <param name="pOtrosArgumentos">Argumentos que van en el campo Tags de colatagscomunidades</param>
        /// <param name="pEliminado">Indica si la fila es de tipo eliminado o agregado. Válido si no se especifica pTipoElementoBase</param>
        /// <param name="pPrioridadBase">Prioridad de las filas introducidas en el modelo base</param>
        /// <param name="pEstadoCargaID"></param>
        /// <param name="pTipoElementoBase">Enumeración TipoElementoEnCola. Si no se especifica, será eliminado o agregado en función del parámetro pEliminado</param>
        private void AgregarRecursoModeloBaseSimple(Guid pDocumentoID, Guid pProyectoID, short pTipo, BaseRecursosComunidadDS pBaseRecursosComDS, string pFicheroConfiguracionBD, string pOtrosArgumentos, PrioridadBase pPrioridadBase, bool pEliminado, long pEstadoCargaID, short? pTipoElementoBase)
        {
            int id = -1;

            if (pProyectoID.Equals(Guid.Empty))
            {
                pProyectoID = ProyectoAD.MetaProyecto;
            }

            if (!string.IsNullOrEmpty(pFicheroConfiguracionBD))
            {
                ProyectoCN proyCN = new ProyectoCN(pFicheroConfiguracionBD, mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                id = proyCN.ObtenerTablaBaseProyectoIDProyectoPorID(pProyectoID);
                proyCN.Dispose();
            }
            else
            {
                ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                id = proyCL.ObtenerTablaBaseProyectoIDProyectoPorID(pProyectoID);
                proyCL.Dispose();
            }

            BaseRecursosComunidadDS baseRecursosComDS = pBaseRecursosComDS;
            if (baseRecursosComDS == null)
            {
                baseRecursosComDS = new BaseRecursosComunidadDS();
            }

            #region Marcar agregado

            BaseRecursosComunidadDS.ColaTagsComunidadesRow filaColaTagsDocs = baseRecursosComDS.ColaTagsComunidades.NewColaTagsComunidadesRow();

            filaColaTagsDocs.Estado = (short)EstadosColaTags.EnEspera;
            filaColaTagsDocs.FechaPuestaEnCola = DateTime.Now;
            filaColaTagsDocs.TablaBaseProyectoID = id;
            filaColaTagsDocs.Tags = $"{Constantes.ID_TAG_DOCUMENTO}{pDocumentoID}{Constantes.ID_TAG_DOCUMENTO},{Constantes.TIPO_DOC}{pTipo}{Constantes.TIPO_DOC}{pOtrosArgumentos},{TagBaseAfinidadVirtuoso}";

            if (!pTipoElementoBase.HasValue || !Enum.IsDefined(typeof(TiposElementosEnCola), (int)pTipoElementoBase.Value))
            {
                if (pEliminado)
                {
                    filaColaTagsDocs.Tipo = 1;
                }
                else
                {
                    filaColaTagsDocs.Tipo = 0;
                }
            }
            else
            {
                filaColaTagsDocs.Tipo = pTipoElementoBase.Value;
            }

            filaColaTagsDocs.Prioridad = (short)pPrioridadBase;

            //LANZAR Y PROBAR EL SERVICIO CREANDO UN RECURSO EN LA COMUNIDAD
            try
            {
                InsertarFilaColaTagsComunidadesEnRabbitMQ(filaColaTagsDocs);
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, "Fallo al insertar en Rabbit, insertamos en la base de datos BASE, tabla ColaTagsComunidades");
                baseRecursosComDS.ColaTagsComunidades.AddColaTagsComunidadesRow(filaColaTagsDocs);
            }

            #endregion

            if (pBaseRecursosComDS == null)
            {
                BaseComunidadCN brComCN = new BaseComunidadCN("base", -1, mEntityContext, mLoggingService, mEntityContextBase, mConfigService, mServicesUtilVirtuosoAndReplication);
                brComCN.InsertarFilasEnRabbit("ColaTagsComunidades", baseRecursosComDS);
                brComCN.Dispose();

                baseRecursosComDS.Dispose();
            }

        }

        public void InsertarFilaColaTagsComunidadesEnRabbitMQ(BaseRecursosComunidadDS.ColaTagsComunidadesRow pFilaCola)
        {
            string exchange = "";
            string colaRabbit = "ColaTagsComunidades";
            if (mConfigService.ExistRabbitConnection(RabbitMQClient.BD_SERVICIOS_WIN))
            {
                using (RabbitMQClient rabbitMQ = new RabbitMQClient(RabbitMQClient.BD_SERVICIOS_WIN, colaRabbit, mLoggingService, mConfigService, exchange, colaRabbit))
                {
                    rabbitMQ.AgregarElementoACola(JsonConvert.SerializeObject(pFilaCola.ItemArray));
                }
            }
        }


        /// <summary>
        /// Notifica al modelo base que se ha eliminado un documento.
        /// </summary>
        /// <param name="pDocumentoID">ID de documento</param>
        /// <param name="pProyectoID">Proyecto en el que se han modificado</param>
        /// <param name="pTipo">Tipo de documento</param>
        /// <param name="pPrioridadBase">Prioridad de las filas introducidas en el modelo base</param>
        private void AgregarRecursoEliminadoModeloBaseSimple(Guid pDocumentoID, Guid pProyectoID, short pTipo, PrioridadBase pPrioridadBase)
        {
            AgregarRecursoModeloBaseSimple(pDocumentoID, pProyectoID, pTipo, null, null, "", pPrioridadBase, true, -1, null);
        }

        private static string ObtenerSearchDescripcionRecurso(string pDescripcion)
        {
            StringBuilder valorSearch = new StringBuilder();
            Dictionary<string, string> idiomaDescp = UtilCadenas.ObtenerTextoPorIdiomas(pDescripcion);

            if (idiomaDescp.Count > 0)
            {
                List<string> idiomasSimilares = new List<string>();

                foreach (string idioma in idiomaDescp.Keys)
                {
                    if (!idiomasSimilares.Contains(idiomaDescp[idioma]))
                    {
                        valorSearch.Append(" " + idiomaDescp[idioma]);
                        idiomasSimilares.Add(idiomaDescp[idioma]);
                    }
                }
            }
            else
            {
                valorSearch.Append(" " + pDescripcion);
            }

            return valorSearch.ToString();
        }

        private static string GenerarTriplesTituloRecurso(StringBuilder pTriples, Guid pRecursoId, string pTitulo, ref StringBuilder pValorSearch)
        {
            string titulo = UtilCadenas.EliminarHtmlDeTexto(pTitulo);
            titulo = titulo.Replace("\r\n", "");
            titulo = titulo.Replace("\n", "");
            string tripleBusquedaFoafFirstName = "<http://gnoss/" + pRecursoId.ToString().ToUpper() + "> <http://xmlns.com/foaf/0.1/firstName> ";
            Dictionary<string, string> idiomaTitulo = UtilCadenas.ObtenerTextoPorIdiomas(titulo);

            if (idiomaTitulo.Count > 0)
            {
                List<string> idiomasSimilares = new List<string>();

                foreach (string idioma in idiomaTitulo.Keys)
                {
                    pTriples.Append(FacetadoAD.GenerarTripleta("<http://gnoss/" + pRecursoId.ToString().ToUpper() + ">", "<http://gnoss/hasnombrecompleto>", "\"" + UtilidadesVirtuoso.PasarObjetoALower(idiomaTitulo[idioma]) + "\"", idioma));

                    if (!pTriples.ToString().Contains(tripleBusquedaFoafFirstName))
                    {
                        pTriples.Append(FacetadoAD.GenerarTripleta("<http://gnoss/" + pRecursoId.ToString().ToUpper() + ">", "<http://xmlns.com/foaf/0.1/firstName>", "\"" + UtilidadesVirtuoso.PasarObjetoALower(idiomaTitulo[idioma]) + "\"", idioma));
                    }

                    pTriples.Append(AgregarTripletaDesnormalizadaTitulo(pRecursoId, UtilidadesVirtuoso.RemoverSignosSearch(UtilidadesVirtuoso.RemoverSignosAcentos(UtilidadesVirtuoso.PasarObjetoALower(idiomaTitulo[idioma])))));

                    if (!idiomasSimilares.Contains(idiomaTitulo[idioma]))
                    {
                        pTriples.Append(AgregarTripletasDescompuestasTitulo(pRecursoId.ToString().ToUpper(), "<http://gnoss/hasTagTituloDesc>", idiomaTitulo[idioma]));
                        //16/02/2017 se decide dejar de meterlas: hablar con Juan y Esteban
                        //tripletas.Append(ObtenerTagsTitulo(idiomaTitulo[idioma], pRecursoId));
                        pValorSearch.Append(" " + idiomaTitulo[idioma]);
                        idiomasSimilares.Add(idiomaTitulo[idioma]);
                    }
                }
            }
            else
            {
                pTriples.Append(AgregarTripletasDescompuestasTitulo(pRecursoId.ToString().ToUpper(), "<http://gnoss/hasTagTituloDesc>", titulo));
                pTriples.Append(FacetadoAD.GenerarTripleta("<http://gnoss/" + pRecursoId.ToString().ToUpper() + ">", "<http://gnoss/hasnombrecompleto>", "\"" + UtilidadesVirtuoso.PasarObjetoALower(titulo) + "\""));

                if (!pTriples.ToString().Contains(tripleBusquedaFoafFirstName))
                {
                    pTriples.Append(FacetadoAD.GenerarTripleta("<http://gnoss/" + pRecursoId.ToString().ToUpper() + ">", "<http://xmlns.com/foaf/0.1/firstName>", "\"" + UtilidadesVirtuoso.PasarObjetoALower(titulo) + "\""));
                }

                pTriples.Append(AgregarTripletaDesnormalizadaTitulo(pRecursoId, UtilidadesVirtuoso.RemoverSignosSearch(UtilidadesVirtuoso.RemoverSignosAcentos(UtilidadesVirtuoso.PasarObjetoALower(titulo)))));
                pValorSearch.Append(" " + titulo);
                //16/02/2017 se decide dejar de meterlas: hablar con Juan y Esteban
                //tripletas.Append(ObtenerTagsTitulo(titulo, pRecursoId));
            }

            return pTriples.ToString();
        }

        //16/02/2017 se decide dejar de meterlas: hablar con Juan y Esteban
        //private static string ObtenerTagsTitulo(string titulo, Guid idElemento)
        //{
        //    StringBuilder triples = new StringBuilder();
        //    int numeroTagsDespreciadosTitulo = 0;
        //    List<string> miniTagsTitulo = AnalizadorSintactico.ObtenerTagsFrase(titulo, out numeroTagsDespreciadosTitulo);
        //    foreach (string etiquetatitulo in miniTagsTitulo)
        //    {
        //        string objeto = "\"" + etiquetatitulo.Replace("\"", "'").Trim().ToLower() + "\" .";
        //        triples.Append(FacetadoAD.GenerarTripleta("<http://gnoss/" + idElemento.ToString().ToUpper() + ">", "<http://gnoss/hasTagDesc>", objeto));
        //    }

        //    return triples.ToString();
        //}

        private static string GenerarTriplesTagsRecurso(Guid pRecursoId, List<string> pListaTags, ref StringBuilder pValorSearch)
        {
            StringBuilder tripletas = new StringBuilder();
            foreach (string tag in pListaTags)
            {
                //16/02/2017 se decide dejar de meterlas: hablar con Juan y Esteban
                //tripletas.Append(AgregarTripletasDescompuestasTitulo(pRecursoId.ToString(), "<http://gnoss/hasTagDesc>", tag));
                string objeto = "\"" + tag.Replace("\"", "'").Trim() + "\" .";
                tripletas.Append(FacetadoAD.GenerarTripleta("<http://gnoss/" + pRecursoId.ToString().ToUpper() + ">", "<http://rdfs.org/sioc/types#Tag>", objeto));
                pValorSearch.Append(" " + tag);
            }

            return tripletas.ToString();
        }

        private string GenerarTripletasCategoriasRecurso(Guid pRecursoId, Guid pProyectoId, List<AD.EntityModel.Models.Documentacion.DocumentoWebAgCatTesauro> pFilasRelacionCategoria, GestionTesauro pGestorTesauro, string pUrlIntragnoss, bool pEsEdicionCategorias, ref StringBuilder pValorSearch)
        {
            List<Guid> categoriasAgregadas = new List<Guid>();

            foreach (AD.EntityModel.Models.Documentacion.DocumentoWebAgCatTesauro filaAgCat in pFilasRelacionCategoria)
            {
                Guid idCat = filaAgCat.CategoriaTesauroID;
                if (!categoriasAgregadas.Contains(idCat))
                {
                    categoriasAgregadas.Add(idCat);

                    //while padre
                    List<AD.EntityModel.Models.Tesauro.CatTesauroAgCatTesauro> filasCatAgCat = pGestorTesauro.TesauroDW.ListaCatTesauroAgCatTesauro.Where(item => item.CategoriaInferiorID.Equals(idCat)).ToList();

                    while (filasCatAgCat.Count > 0)
                    {
                        Guid catPadreID = filasCatAgCat.FirstOrDefault().CategoriaSuperiorID;
                        if (!categoriasAgregadas.Contains(catPadreID))
                        {
                            categoriasAgregadas.Add(catPadreID);
                        }
                        filasCatAgCat = pGestorTesauro.TesauroDW.ListaCatTesauroAgCatTesauro.Where(item => item.CategoriaInferiorID.Equals(catPadreID)).ToList();
                    }
                }
            }

            StringBuilder triplesInsertar = new StringBuilder();
            string sujeto = "<http://gnoss/" + pRecursoId.ToString().ToUpper() + "> ";
            string predicado = "<http://www.w3.org/2004/02/skos/core#ConceptID> ";
            foreach (Guid categoriaID in categoriasAgregadas)
            {
                if (pGestorTesauro.ListaCategoriasTesauro.ContainsKey(categoriaID))
                {
                    string objeto = "<http://gnoss/" + categoriaID.ToString().ToUpper() + "> ";

                    triplesInsertar.Append(sujeto + predicado + objeto + ". \n ");

                    string nombrecategoria = pGestorTesauro.ListaCategoriasTesauro[categoriaID].FilaCategoria.Nombre;

                    triplesInsertar.Append(objeto + "<http://gnoss/CategoryName> " + "\"" + nombrecategoria.ToLower() + "\" . \n ");

                    Dictionary<string, string> idiomaCategoria = UtilCadenas.ObtenerTextoPorIdiomas(nombrecategoria.ToLower());

                    if (idiomaCategoria.Count > 0)
                    {
                        List<string> idiomasSimilares = new List<string>();

                        foreach (string idioma in idiomaCategoria.Keys)
                        {
                            if (!idiomasSimilares.Contains(idiomaCategoria[idioma]))
                            {
                                pValorSearch.Append(" " + idiomaCategoria[idioma]);
                                idiomasSimilares.Add(idiomaCategoria[idioma]);
                            }
                        }
                    }
                    else
                    {
                        pValorSearch.Append(" " + nombrecategoria.ToLower());
                    }
                }
            }

            if (pEsEdicionCategorias)
            {
                pValorSearch.Append(" " + LeerSearchDeVirtuoso(pRecursoId, pProyectoId, pUrlIntragnoss));

                string predicadoSearch = "<http://gnoss/search>";
                string tripleSearch = UtilCadenas.EliminarHtmlDeTextoPorEspacios(pValorSearch.ToString()).Replace("\\", "/");
                tripleSearch = "\" " + tripleSearch.Replace("\"", "'").Trim() + " \" .";
                tripleSearch = tripleSearch.ToLower();

                if (!string.IsNullOrEmpty(tripleSearch))
                {
                    triplesInsertar.Append(FacetadoAD.GenerarTripleta(sujeto, predicadoSearch, UtilidadesVirtuoso.RemoverSignosSearch(UtilidadesVirtuoso.RemoverSignosAcentos(tripleSearch))));
                }
            }

            return triplesInsertar.ToString();
        }

        /// <summary>
        /// Genera el triple de compartición automática si el tipo de publicación del recurso es de tipo CompartidoAutomatico
        /// </summary>
        /// <param name="pRecursoId">Identificador del recurso</param>
        /// <param name="pProyectoId">Comunidad origen del recurso</param>
        /// <param name="pTipoPublicacion">DocumentoWebVinBaseRecursos.TipoPublicacion</param>
        /// <returns>Cadena con el triple de compartición automática si el tipo de publicación del recurso es de tipo CompartidoAutomatico. Vacío en caso contrario</returns>
        private static string GenerarTripletaComparticionAutomaticaRecurso(Guid pRecursoId, Guid pProyectoId, short pTipoPublicacion)
        {
            string tripleta = string.Empty;
            if (pTipoPublicacion.Equals((short)TipoPublicacion.CompartidoAutomatico))
            {
                tripleta = FacetadoAD.GenerarTripleta("<http://gnoss/" + pRecursoId.ToString().ToUpper() + ">", "<http://gnoss/hasComunidadOrigen>", "<http://gnoss/" + pProyectoId.ToString().ToUpper() + ">");
            }
            return tripleta;
        }

        /// <summary>
        /// Genera los triples de los autores del recurso.
        /// </summary>
        /// <param name="pRecursoId">Identificador del recurso</param>
        /// <param name="pAutoresDocumento">Cadena con los autores del recurso separados por comas</param>
        /// <returns>Cadena con los triples de los autores del recurso</returns>
        private static string GenerarTripletasAutoresRecurso(Guid pRecursoId, string pAutoresDocumento)
        {
            StringBuilder tripletas = new StringBuilder();
            if (!string.IsNullOrEmpty(pAutoresDocumento))
            {
                char[] separadores = { ',' };
                string[] autores = pAutoresDocumento.Split(separadores, StringSplitOptions.RemoveEmptyEntries);

                string sujeto = "<http://gnoss/" + pRecursoId.ToString().ToUpper() + "> ";
                string predicado = "<http://gnoss/hasautor> ";

                foreach (string autor in autores)
                {
                    string objeto = "\"" + autor.Replace("\"", "'").Trim() + "\" .";
                    string tripletaAutor = FacetadoAD.GenerarTripleta(sujeto, predicado, objeto.ToLowerSearchGraph());
                    tripletas.Append(tripletaAutor);
                }
            }
            return tripletas.ToString();
        }

        /// <summary>
        /// Genera los triples con la información común del recurso
        /// </summary>
        /// <param name="pDocumento"></param>
        /// <param name="pRecursoId"></param>
        /// <param name="pProyectoId"></param>
        /// <param name="pTipoAccesoProyecto"></param>
        /// <returns></returns>
        private string GenerarTripletasInformacionComunRecurso(Documento pDocumento, TipoAcceso pTipoAccesoProyecto, Guid pProyectoID)
        {
            StringBuilder tripletas = new StringBuilder();

            #region Predicados

            string sujeto = "<http://gnoss/" + pDocumento.Clave.ToString().ToUpper() + ">";
            string predHasPublicadorIdentidadId = "<http://gnoss/haspublicadorIdentidadID>";
            string predHasPublicador = "<http://gnoss/haspublicador>";
            string predHasNumeroVotos = "<http://gnoss/hasnumeroVotos>";
            string predHasPopularidad = "<http://gnoss/hasPopularidad>";
            string predHasfechapublicacion = "<http://gnoss/hasfechapublicacion>";
            string predHasfechamodificacion = "<http://gnoss/hasfechamodificacion>";
            string predHasnumeroVisitas = "<http://gnoss/hasnumeroVisitas>";

            #endregion

            //Grupos Editores y Grupos Lectores
            tripletas.Append(GenerarTripletasGruposEditoresLectoresRecurso(pDocumento));

            //Editores y Lectores
            tripletas.Append(GenerarTripletasEditoresLectoresRecurso(pDocumento, pProyectoID));

            //Privacidad
            tripletas.Append(GenerarTripletasHasPrivacidadRecurso(pDocumento));

            //hasPublicadorIdentidadId
            string objetoHasPublicadorIdentidadId = "<http://gnoss/" + pDocumento.CreadorID.ToString().ToUpper() + ">";
            tripletas.Append(FacetadoAD.GenerarTripleta(sujeto, predHasPublicadorIdentidadId, objetoHasPublicadorIdentidadId));

            //hasPublicador
            //string objetoHasPublicador = pDocumento.NombreCreador; -> no tiene cargado el "TipoPerfil" y siempre trae "UsuarioActual"
            //string objetoHasPublicador = "\"" + pDocumento.GestorDocumental.GestorIdentidades.ListaIdentidades[pDocumento.CreadorID].NombreCompuesto + "\"";
            //tripletas.Append(FacetadoAD.GenerarTripleta(sujeto, predHasPublicador, UtilidadesVirtuoso.PasarObjetoALower(objetoHasPublicador)));

            Identidad identidadPublicador = pDocumento.GestorDocumental.GestorIdentidades.ListaIdentidades[pDocumento.CreadorID];
            if (pDocumento.FilaDocumentoWebVinBR != null && pDocumento.FilaDocumentoWebVinBR.IdentidadPublicacionID.HasValue && pDocumento.GestorDocumental.GestorIdentidades.ListaIdentidades.ContainsKey(pDocumento.FilaDocumentoWebVinBR.IdentidadPublicacionID.Value))
            {
                identidadPublicador = pDocumento.GestorDocumental.GestorIdentidades.ListaIdentidades[pDocumento.FilaDocumentoWebVinBR.IdentidadPublicacionID.Value];
            }

            string objetoHasPublicador = "\"" + identidadPublicador.NombreCompuesto() + "\"";           

            if (identidadPublicador.TrabajaPersonaConOrganizacion)
            {
                objetoHasPublicador = "\"" + identidadPublicador.NombreOrganizacion + "\"";
                tripletas.Append(FacetadoAD.GenerarTripleta(sujeto, predHasPublicador, UtilidadesVirtuoso.PasarObjetoALower(objetoHasPublicador)));
            }
            else
            {
                tripletas.Append(FacetadoAD.GenerarTripleta(sujeto, predHasPublicador, PasarObjetoALower(objetoHasPublicador)));
            }

            if ((pTipoAccesoProyecto.Equals(TipoAcceso.Publico) || pTipoAccesoProyecto.Equals(TipoAcceso.Restringido)) && (pDocumento.FilaDocumento.Valoracion.HasValue))
            {
                //hasnumeroVotos
                string numeroVotos = pDocumento.FilaDocumento.Valoracion.ToString();
                tripletas.Append(FacetadoAD.GenerarTripleta(sujeto, predHasNumeroVotos, PasarObjetoALower(numeroVotos)));
            }

            if (pDocumento.FilaDocumentoWebVinBR != null && pDocumento.FilaDocumentoWebVinBR.Rank_Tiempo.HasValue)
            {
                //hasPopularidad
                string popularidad = pDocumento.FilaDocumentoWebVinBR.Rank_Tiempo.ToString();
                tripletas.Append(FacetadoAD.GenerarTripleta(sujeto, predHasPopularidad, PasarObjetoALower(popularidad)));
            }

            if (pDocumento.FilaDocumentoWebVinBR != null && pDocumento.FilaDocumentoWebVinBR.FechaPublicacion.HasValue)
            {
                //hasfechapublicacion
                string fechaPublicacion = pDocumento.FilaDocumentoWebVinBR.FechaPublicacion.Value.ToString("yyyyMMddHHmmss");
                tripletas.Append(FacetadoAD.GenerarTripleta(sujeto, predHasfechapublicacion, PasarObjetoALower(fechaPublicacion)));
            }

            if (pDocumento.FilaDocumento.FechaModificacion.HasValue)
            {
                //hasfechamodificacion
                string fechaModificacion = pDocumento.FilaDocumento.FechaModificacion.Value.ToString("yyyyMMddHHmmss");
                tripletas.Append(FacetadoAD.GenerarTripleta(sujeto, predHasfechamodificacion, PasarObjetoALower(fechaModificacion)));
            }

            //hasnumeroVisitas
            string numeroVistas = pDocumento.FilaDocumento.NumeroTotalConsultas.ToString();
            tripletas.Append(FacetadoAD.GenerarTripleta(sujeto, predHasnumeroVisitas, PasarObjetoALower(numeroVistas)));

            return tripletas.ToString();
        }

        public string GenerarTripletasGruposEditoresLectoresRecurso(Documento pDocumento)
        {
            //Cargo los grupos de los editores
            List<Guid> listaGrupos = new List<Guid>();
            foreach (GrupoEditorRecurso grupoEditor in pDocumento.ListaGruposEditores.Values)
            {
                if (!listaGrupos.Contains(grupoEditor.Clave) && (pDocumento.GestorDocumental.GestorIdentidades == null || !pDocumento.GestorDocumental.GestorIdentidades.DataWrapperIdentidad.ListaGrupoIdentidades.Any(item => item.GrupoID.Equals(grupoEditor.Clave))))
                {
                    listaGrupos.Add(grupoEditor.Clave);
                }
            }
            if (listaGrupos.Count > 0)
            {
                IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                DataWrapperIdentidad identDW = identidadCN.ObtenerGruposPorIDGrupo(listaGrupos, false);

                GestionIdentidades gestorIdent = new GestionIdentidades(identDW, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                identidadCN.Dispose();

                if (pDocumento.GestorDocumental.GestorIdentidades == null)
                {
                    pDocumento.GestorDocumental.GestorIdentidades = gestorIdent;
                }
                else
                {
                    pDocumento.GestorDocumental.GestorIdentidades.DataWrapperIdentidad.Merge(gestorIdent.DataWrapperIdentidad);
                    pDocumento.GestorDocumental.GestorIdentidades.RecargarHijos();
                }
            }

            StringBuilder tripletas = new StringBuilder();
            #region Predicados

            string sujeto = "<http://gnoss/" + pDocumento.Clave.ToString().ToUpper() + ">";
            string predHasEditorIdentidadId = "<http://gnoss/haseditorIdentidadID>";
            string predHasEditor = "<http://gnoss/haseditor>";
            string predHasParticipanteIdentidadId = "<http://gnoss/hasparticipanteIdentidadID>";
            string predHasParticipante = "<http://gnoss/hasparticipante>";
            string predHasGrupoEditor = "<http://gnoss/hasgrupoEditor>";
            string predHasGrupoLector = "<http://gnoss/hasgrupoLector>";

            #endregion

            foreach (Guid grupoEditorId in pDocumento.ListaGruposEditoresSinLectores.Keys)
            {
                //hasEditorIdentidadID
                string objetoGrupoHasEditorIdentidadID = "<http://gnoss/" + grupoEditorId.ToString().ToUpper() + ">";
                tripletas.Append(FacetadoAD.GenerarTripleta(sujeto, predHasEditorIdentidadId, objetoGrupoHasEditorIdentidadID));

                //hasEditor
                string objetoGrupoHasEditor = "\"" + pDocumento.ListaGruposEditoresSinLectores[grupoEditorId].Nombre + "\"";
                tripletas.Append(FacetadoAD.GenerarTripleta(sujeto, predHasEditor, PasarObjetoALower(objetoGrupoHasEditor)));

                //hasParticipanteIdentidadID
                tripletas.Append(FacetadoAD.GenerarTripleta(sujeto, predHasParticipanteIdentidadId, objetoGrupoHasEditorIdentidadID));

                //hasParticipante
                tripletas.Append(FacetadoAD.GenerarTripleta(sujeto, predHasParticipante, PasarObjetoALower(objetoGrupoHasEditor)));

                //hasgrupoEditor
                tripletas.Append(FacetadoAD.GenerarTripleta(sujeto, predHasGrupoEditor, PasarObjetoALower(objetoGrupoHasEditor)));
            }

            foreach (Guid grupoEditorId in pDocumento.ListaGruposLectores.Keys)
            {
                string objetoGrupoHasLectorIdentidadID = "<http://gnoss/" + grupoEditorId.ToString().ToUpper() + ">";

                //hasgrupoLector
                string objetoHasLector = "\"" + pDocumento.ListaGruposLectores[grupoEditorId].Nombre + "\"";
                tripletas.Append(FacetadoAD.GenerarTripleta(sujeto, predHasGrupoLector, PasarObjetoALower(objetoHasLector)));

                //hasParticipante
                string objetoGrupoHasLector = "\"" + pDocumento.ListaGruposLectores[grupoEditorId].Nombre + "\"";
                tripletas.Append(FacetadoAD.GenerarTripleta(sujeto, predHasParticipante, PasarObjetoALower(objetoGrupoHasLector)));

                //hasParticipanteIdentidadID
                tripletas.Append(FacetadoAD.GenerarTripleta(sujeto, predHasParticipanteIdentidadId, objetoGrupoHasLectorIdentidadID));
            }

            return tripletas.ToString();
        }

        public static string GenerarTripletasHasPrivacidadRecurso(Documento pDocumento)
        {
            StringBuilder tripletas = new StringBuilder();
            string sujeto = "<http://gnoss/" + pDocumento.Clave.ToString().ToUpper() + ">";

            string predHasPrivacidadCom = "<http://gnoss/hasprivacidadCom>";

            string privacidad;
            if (pDocumento.FilaDocumentoWebVinBR != null && pDocumento.FilaDocumentoWebVinBR.PrivadoEditores)
            {
                privacidad = "\"privado\" .";
            }
            else
            {
                if (pDocumento.FilaDocumento.Visibilidad.Equals((short)VisibilidadDocumento.PrivadoMiembrosComunidad))
                {
                    privacidad = "\"publicoreg\" .";
                }
                else
                {
                    privacidad = "\"publico\" .";
                }
            }

            if (!string.IsNullOrEmpty(privacidad))
            {
                //hasprivacidadCom
                tripletas.Append(FacetadoAD.GenerarTripleta(sujeto, predHasPrivacidadCom, PasarObjetoALower(privacidad)));
            }

            return tripletas.ToString();
        }

        public string GenerarTripletasEditoresLectoresRecurso(Documento pDocumento, Guid pProyectoID)
        {
            //Cargo los editores si no están cargados
            List<Guid> listaEditores = new List<Guid>();
            foreach (EditorRecurso editor in pDocumento.ListaPerfilesEditores.Values)
            {
                if (!listaEditores.Contains(editor.Clave) && (pDocumento.GestorDocumental.GestorIdentidades == null || pDocumento.GestorDocumental.GestorIdentidades.DataWrapperIdentidad.ListaPerfil.FirstOrDefault(perfil => perfil.PerfilID.Equals(editor.Clave)) == null))
                {
                    listaEditores.Add(editor.Clave);
                }
            }
            if (listaEditores.Count > 0)
            {
                IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                DataWrapperIdentidad identDW = identidadCN.ObtenerIdentidadesDePerfilesEnProyecto(listaEditores, pDocumento.ProyectoID);

                GestionIdentidades gestorIdent = new GestionIdentidades(identDW, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                identidadCN.Dispose();

                if (pDocumento.GestorDocumental.GestorIdentidades == null)
                {
                    pDocumento.GestorDocumental.GestorIdentidades = gestorIdent;
                }
                else
                {
                    pDocumento.GestorDocumental.GestorIdentidades.DataWrapperIdentidad.Merge(gestorIdent.DataWrapperIdentidad);
                    pDocumento.GestorDocumental.GestorIdentidades.RecargarHijos();
                }
            }


            StringBuilder tripletas = new StringBuilder();

            #region Predicados

            string sujeto = "<http://gnoss/" + pDocumento.Clave.ToString().ToUpper() + ">";
            string predHasEditorIdentidadId = "<http://gnoss/haseditorIdentidadID>";
            string predHasEditor = "<http://gnoss/haseditor>";
            string predHasParticipanteIdentidadId = "<http://gnoss/hasparticipanteIdentidadID>";
            string predHasParticipante = "<http://gnoss/hasparticipante>";

            #endregion

            //hasEditorIdentidadID
            //string objetoHasEditorIdentidadID = "<http://gnoss/" + pDocumento.CreadorID.ToString().ToUpper() + ">";
            //tripletas.Append(FacetadoAD.GenerarTripleta(sujeto, predHasEditorIdentidadId, objetoHasEditorIdentidadID));

            //hasEditor
            //string objetoHasEditor = pDocumento.NombreCreador;
            //tripletas.Append(FacetadoAD.GenerarTripleta(sujeto, predHasEditor, UtilidadesVirtuoso.PasarObjetoALower(objetoHasEditor)));

            string objetoHasEditor;
            foreach (Guid perfilIdEditor in pDocumento.ListaPerfilesEditoresSinLectores.Keys)
            {
                if (pDocumento.ListaPerfilesEditores.ContainsKey(perfilIdEditor))
                {
                    Identidad identidadEditor = null;
                    if (pDocumento.ListaPerfilesEditores[perfilIdEditor].IdentidadEnProyectoActual(pProyectoID) != null)
                    {
                        identidadEditor = pDocumento.ListaPerfilesEditores[perfilIdEditor].IdentidadEnProyectoActual(pProyectoID);
                    }
                    else
                    {
                        Guid identidadID = pDocumento.ListaPerfilesEditores[perfilIdEditor].GestionDocumental.GestorIdentidades.ObtenerIdentidadDePerfilEnProyecto(perfilIdEditor, pDocumento.ProyectoID);
                        identidadEditor = pDocumento.ListaPerfilesEditores[perfilIdEditor].GestionDocumental.GestorIdentidades.ListaIdentidades[identidadID];
                    }

                    if (identidadEditor == null)
                    {
                        throw new Exception("ERROR GenerarTripletasEditoresLectoresRecurso: No se ha podido obtener la identidad del editor para el perfil: " + perfilIdEditor + " en el proyecto: " + pDocumento.ProyectoID);
                    }

                    //hasEditorIdentidadID
                    string objetoHasEditorIdentidadID = "<http://gnoss/" + identidadEditor.Clave.ToString().ToUpper() + ">";
                    tripletas.Append(FacetadoAD.GenerarTripleta(sujeto, predHasEditorIdentidadId, objetoHasEditorIdentidadID));

                    //hasEditor
                    objetoHasEditor = "\"" + identidadEditor.Nombre() + "\"";
                    tripletas.Append(FacetadoAD.GenerarTripleta(sujeto, predHasEditor, UtilidadesVirtuoso.PasarObjetoALower(objetoHasEditor)));

                    //hasParticipanteIdentidadID
                    tripletas.Append(FacetadoAD.GenerarTripleta(sujeto, predHasParticipanteIdentidadId, objetoHasEditorIdentidadID));
                }
            }

            foreach (Guid perfilIdLector in pDocumento.ListaPerfilesLectores.Keys)
            {
                if (pDocumento.ListaPerfilesEditores.ContainsKey(perfilIdLector))
                {
                    Identidad identidadLector = null;
                    if (pDocumento.ListaPerfilesEditores[perfilIdLector].IdentidadEnProyectoActual(pProyectoID) != null)
                    {
                        identidadLector = pDocumento.ListaPerfilesEditores[perfilIdLector].IdentidadEnProyectoActual(pProyectoID);
                    }
                    else
                    {
                        Guid identidadID = pDocumento.ListaPerfilesEditores[perfilIdLector].GestionDocumental.GestorIdentidades.ObtenerIdentidadDePerfilEnProyecto(perfilIdLector, pDocumento.ProyectoID);
                        identidadLector = pDocumento.ListaPerfilesEditores[perfilIdLector].GestionDocumental.GestorIdentidades.ListaIdentidades[identidadID];
                    }

                    if (identidadLector.Equals(Guid.Empty))
                    {
                        throw new Exception("ERROR GenerarTripletasEditoresLectoresRecurso: No se ha podido obtener la identidad del editor para el perfil: " + perfilIdLector + " en el proyecto: " + pDocumento.ProyectoID);
                    }

                    //hasParticipanteIdentidadID
                    string objetoHasEditorIdentidadID = "<http://gnoss/" + identidadLector.Clave.ToString().ToUpper() + ">";
                    tripletas.Append(FacetadoAD.GenerarTripleta(sujeto, predHasParticipanteIdentidadId, objetoHasEditorIdentidadID));

                    //hasParticipante
                    objetoHasEditor = "\"" + identidadLector.Nombre() + "\"";
                    tripletas.Append(FacetadoAD.GenerarTripleta(sujeto, predHasParticipante, UtilidadesVirtuoso.PasarObjetoALower(objetoHasEditor)));
                }
            }

            return tripletas.ToString();
        }

        private string GenerarTripletasInformacionExtraRecurso(Documento pDocumento, Elementos.ServiciosGenerales.Proyecto pProyecto)
        {
            StringBuilder tripletas = new StringBuilder();

            #region Predicados

            string sujeto = "<http://gnoss/" + pDocumento.Clave.ToString().ToUpper() + ">";
            string predHasnivelcertification = "<http://gnoss/hasnivelcertification>";
            string predHastipodoc = "<http://gnoss/hastipodoc>";

            #endregion

            //valor por defecto si no tiene certificación
            string strOrdenNivelCertificacion = "100";

            if (pDocumento.FilaDocumentoWebVinBR != null && pDocumento.FilaDocumentoWebVinBR.NivelCertificacionID.HasValue)
            {
                ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                pProyecto.GestorProyectos.DataWrapperProyectos.Merge(proyCN.ObtenerNivelesCertificacionRecursosProyecto(pProyecto.Clave));
                proyCN.Dispose();
                short ordenNivelCertificacion = pProyecto.ObtenerOrdenDeNivelCertificacion(pDocumento.FilaDocumentoWebVinBR.NivelCertificacionID.Value);
                if (!ordenNivelCertificacion.Equals(-1))
                {
                    strOrdenNivelCertificacion = ordenNivelCertificacion.ToString();
                }
            }

            //hasnivelcertification
            tripletas.Append(FacetadoAD.GenerarTripleta(sujeto, predHasnivelcertification, PasarObjetoALower(strOrdenNivelCertificacion)));

            //hastipodoc
            TiposDocumentacion tipoDoc = pDocumento.TipoDocumentacion;
            switch (pDocumento.TipoDocumentacion)
            {
                case TiposDocumentacion.VideoBrightcove:
                case TiposDocumentacion.VideoTOP:
                    tipoDoc = TiposDocumentacion.Video;
                    break;
                case TiposDocumentacion.AudioBrightcove:
                case TiposDocumentacion.AudioTOP:
                    tipoDoc = TiposDocumentacion.Audio;
                    break;
            }

            tripletas.Append(FacetadoAD.GenerarTripleta(sujeto, predHastipodoc, UtilidadesVirtuoso.PasarObjetoALower("\"" + ((short)tipoDoc).ToString() + "\"")));
            return tripletas.ToString();
        }

        public string GenerarTripletasRecursoPorTipoDocumento(string pFicheroConfiguracionBD, string pFicheroConfiguracionBDBase, string pUrlIntragnoss, Documento pDocumento, Guid pProyectoId, string pRdfConfiguradoRecursoNoSemantico, List<TripleWrapper> pListaTriplesRecurso, Ontologia pOntologia)
        {
            StringBuilder tripletas = new StringBuilder();

            #region Predicados

            string sujeto = "<http://gnoss/" + pDocumento.Clave.ToString().ToUpper() + ">";
            string predicadoType = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>";
            string predHasestado = "<http://gnoss/hasestado>";

            #endregion

            string rdfType = "";
            string objetoHasestado;
            switch (pDocumento.TipoDocumentacion)
            {
                case TiposDocumentacion.Pregunta:
                    rdfType = "\"Pregunta\"";
                    objetoHasestado = "\"Cerrada\"";

                    if (pDocumento.PermiteComentarios)
                    {
                        objetoHasestado = "\"Sin respuestas\"";
                        if (pDocumento.FilaDocumentoWebVinBR.NumeroComentarios > 0)
                        {
                            objetoHasestado = "\"Abierta\"";
                        }
                    }

                    tripletas.Append(FacetadoAD.GenerarTripleta(sujeto, predHasestado, PasarObjetoALower(objetoHasestado)));
                    break;

                case TiposDocumentacion.Debate:
                    rdfType = "\"Debate\"";
                    objetoHasestado = "\"Cerrado\"";

                    if (pDocumento.PermiteComentarios)
                    {
                        objetoHasestado = "\"Sin comentarios\"";
                        if (pDocumento.FilaDocumentoWebVinBR.NumeroComentarios > 0)
                        {
                            objetoHasestado = "\"Abierto\"";
                        }
                    }

                    tripletas.Append(FacetadoAD.GenerarTripleta(sujeto, predHasestado, PasarObjetoALower(objetoHasestado)));
                    break;
                case TiposDocumentacion.Encuesta:
                    rdfType = "\"Encuesta\"";
                    objetoHasestado = "\"Cerrada\"";

                    if (pDocumento.PermiteComentarios)
                    {
                        objetoHasestado = "\"Abierta\"";
                    }

                    tripletas.Append(FacetadoAD.GenerarTripleta(sujeto, predHasestado, PasarObjetoALower(objetoHasestado)));
                    break;
                case TiposDocumentacion.Semantico:
                    tripletas.Append(GenerarTripletasTipoRecursoSemantico(pFicheroConfiguracionBD, pFicheroConfiguracionBDBase, pUrlIntragnoss, pDocumento, pProyectoId, pListaTriplesRecurso, pOntologia, out rdfType));
                    rdfType = "\"" + rdfType + "\"";
                    break;
                default:
                    rdfType = "\"Recurso\"";
                    if (!string.IsNullOrEmpty(pRdfConfiguradoRecursoNoSemantico))
                    {
                        rdfType = $"\"{pRdfConfiguradoRecursoNoSemantico}\"";
                    }
                    break;
            }

            if (!string.IsNullOrEmpty(rdfType))
            {
                //predicadoType
                tripletas.Append(FacetadoAD.GenerarTripleta(sujeto, predicadoType, rdfType));
            }

            return tripletas.ToString();
        }

        /// <summary>
        /// Obtiene los triples del recurso semántico para el grafo de búsqueda
        /// </summary>
        /// <param name="pDocumento">Documento</param>
        /// <param name="pListaTriplesRecurso">Lista de triples generados desde la web para el grafo de ontología</param>
        /// <returns></returns>
        public string GenerarTripletasTipoRecursoSemantico(string pFicheroConfiguracionBD, string pFicheroConfiguracionBDBase, string pUrlIntragnoss, Documento pDocumento, Guid pProyectoId, List<TripleWrapper> pListaTriplesRecurso, Ontologia pOntologia, out string pRdfType)
        {
            string enlaceOntologia = pDocumento.GestorDocumental.ListaDocumentos[pDocumento.FilaDocumento.ElementoVinculadoID.Value].Enlace;
            string type = enlaceOntologia.Replace(".owl", "");
            if (type.Contains("/"))
            {
                type = type.Substring(type.LastIndexOf("/") + 1);
            }

            //Obtengo el Tipo de cada propiedad
            FacetaCL facetaCL = new FacetaCL(pFicheroConfiguracionBD, pFicheroConfiguracionBDBase, pUrlIntragnoss, mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
            DataWrapperFacetas facetaDW = facetaCL.ObtenerTodasFacetasDeProyecto(new List<string> { type }, ProyectoAD.MetaOrganizacion, pProyectoId, false);
            Dictionary<string, List<string>> informacionOntologias = facetaCL.ObtenerPrefijosOntologiasDeProyecto(pProyectoId);

            FacetaCN facetaCN = new FacetaCN(pFicheroConfiguracionBD, mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            facetaCN.CargarFacetaConfigProyRanfoFecha(ProyectoAD.MetaOrganizacion, pProyectoId, facetaDW);

            return GenerarTripletasTipoRecursoSemantico(pFicheroConfiguracionBD, pFicheroConfiguracionBDBase, pUrlIntragnoss, enlaceOntologia, pDocumento.Clave, pProyectoId, pListaTriplesRecurso, pOntologia, out pRdfType, facetaDW, informacionOntologias);
        }

        /// <summary>
        /// Obtiene los triples del recurso semántico para el grafo de búsqueda
        /// </summary>
        /// <param name="pDocumento">Documento</param>
        /// <param name="pListaTriplesRecurso">Lista de triples generados desde la web para el grafo de ontología</param>
        /// <returns></returns>
        public string GenerarTripletasTipoRecursoSemantico(string pFicheroConfiguracionBD, string pFicheroConfiguracionBDBase, string pUrlIntragnoss, string pEnlaceOntologia, Guid pDocumentoID, Guid pProyectoId, List<TripleWrapper> pListaTriplesRecurso, Ontologia pOntologia, out string pRdfType, DataWrapperFacetas pFacetaDW, Dictionary<string, List<string>> pInformacionOntologias, bool pGenerarRdfType = true, string pRecursoIDPrincipal = null)
        {
            StringBuilder tripletas = new StringBuilder();

            Dictionary<string, string> urlOntologiaPorNamespace = new Dictionary<string, string>();
            foreach (string urlOnt in pInformacionOntologias.Keys)
            {
                string nsOnt = pInformacionOntologias[urlOnt][0];
                if (!urlOntologiaPorNamespace.ContainsKey(nsOnt))
                {
                    string urlOntSinArroba = urlOnt;

                    if (urlOntSinArroba.StartsWith('@'))
                    {
                        urlOntSinArroba = urlOntSinArroba.Substring(1);
                    }

                    if (!urlOntSinArroba.StartsWith("http"))
                    {
                        // Es el namespace de una ontologia: 
                        urlOntSinArroba = $"{pUrlIntragnoss}Ontologia/{urlOntSinArroba}.owl#";
                    }

                    urlOntologiaPorNamespace.Add(nsOnt, urlOntSinArroba);
                }
            }

            string type = pEnlaceOntologia.Replace(".owl", "");
            if (type.Contains("/"))
            {
                type = type.Substring(type.LastIndexOf("/") + 1);
            }
            pRdfType = type;

            List<string> Fecha = new List<string>();
            List<string> Numero = new List<string>();
            List<string> TextoInvariable = new List<string>();
            List<string> listaPropiedades = new List<string>();
            Dictionary<Guid, Dictionary<string, List<string>>> dicPropiedadesOntologia = new Dictionary<Guid, Dictionary<string, List<string>>>();

            ObtenerListaTipoElementosOntologia(pFicheroConfiguracionBD, pDocumentoID, pProyectoId, pOntologia, dicPropiedadesOntologia, ref Fecha, ref Numero);

            ObtenerListaPropiedadesFechaNumeroTextoIvariable(pFacetaDW, pProyectoId, urlOntologiaPorNamespace, ref Fecha, ref Numero, ref TextoInvariable, ref listaPropiedades);

            ObtenerConfiguracionXml(pOntologia.OntologiaID);

            Dictionary<string, List<string>> selectores = ObtenerSelectores(pOntologia, pProyectoId.ToString(), "");
            List<FacetaEntidadesExternas> EntExt = new List<FacetaEntidadesExternas>();
            DocumentacionCN documentacionCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            List<AD.EntityModel.Models.Documentacion.Documento> listaEntidadesSecundarias = documentacionCN.ObtenerOntologiasSecundarias(pProyectoId);
            EntExt = mEntityContext.FacetaEntidadesExternas.ToList();
            foreach (string grafo in selectores.Keys)
            {
                foreach (string selector in selectores[grafo])
                {
                    string entidadID = $"{pUrlIntragnoss}items/{selector}";
                    if (!EntExt.Exists(item => item.EntidadID.Equals(entidadID)))
                    {
                        FacetaEntidadesExternas facetaEntidad = new FacetaEntidadesExternas();
                        facetaEntidad.Grafo = grafo;
                        facetaEntidad.EntidadID = entidadID;
                        facetaEntidad.ProyectoID = pProyectoId;
                        facetaEntidad.EsEntidadSecundaria = listaEntidadesSecundarias.Any(item => item.Enlace.Equals(grafo));
                        EntExt.Add(facetaEntidad);
                    }
                }
            }

            bool tipoAgregado = false;
            string sujetoDocumentoId = $"<http://gnoss/{pDocumentoID.ToString().ToUpper()}>";
            string predicadoRdfType = "http://www.w3.org/1999/02/22-rdf-syntax-ns#type";
            string sujetoAuxiliar = $"<http://gnossAuxiliar/{pDocumentoID.ToString().ToUpper()}>";
            string predicadoAuxiliar = "http://gnoss/hasEntidadAuxiliar";

            List<FacetaConfigProyRangoFecha> filasRangoFechas = new List<FacetaConfigProyRangoFecha>();

            Dictionary<string, DateTime?> propiedadFechaInicioValorFechaInicio = new Dictionary<string, DateTime?>();
            Dictionary<string, DateTime?> propiedadFechaFinValorFechaFin = new Dictionary<string, DateTime?>();

            if (pFacetaDW.ListaFacetaConfigProyRangoFecha.Count > 0)
            {
                foreach (FacetaConfigProyRangoFecha fila in pFacetaDW.ListaFacetaConfigProyRangoFecha)
                {
                    if (fila.PropiedadNueva != null && !string.IsNullOrEmpty(fila.PropiedadNueva))
                    {
                        filasRangoFechas.Add(fila);
                        if (!Fecha.Contains(fila.PropiedadNueva))
                        {
                            Fecha.Add(fila.PropiedadNueva);
                        }
                    }
                }
            }

            if (pGenerarRdfType)
            {
                tripletas.Append(FacetadoAD.GenerarTripleta(sujetoDocumentoId, $"<{predicadoRdfType}>", $"\"{type}\""));
            }

            if (pListaTriplesRecurso != null && pListaTriplesRecurso.Count > 0 && pListaTriplesRecurso[0] != null && !string.IsNullOrEmpty(pListaTriplesRecurso[0].Subject))
            {
                string sujetoTripletaPadre = pListaTriplesRecurso[0].Subject;

                if (!string.IsNullOrEmpty(pRecursoIDPrincipal))
                {
                    sujetoTripletaPadre = pRecursoIDPrincipal;
                }

                foreach (TripleWrapper triple in pListaTriplesRecurso)
                {
                    bool esTripletaEntidadPadre = triple.Subject.Equals(sujetoTripletaPadre);
                    string sujeto = triple.Subject;
                    string predicado = triple.Predicate;

                    if (predicado.StartsWith('<') && predicado.EndsWith('>'))
                    {
                        predicado = predicado.Substring("<".Length, predicado.Length - (">".Length * 2));
                    }

                    string objeto = triple.Object;

                    if (objeto.StartsWith('<') && objeto.EndsWith('>'))
                    {
                        objeto = objeto.Substring("<".Length, objeto.Length - (">".Length * 2)); //quito los <> porque si no lo trata como html y llega vacío
                    }

                    if (objeto.StartsWith('"') && objeto.EndsWith('"'))
                    {
                        objeto = objeto.Substring("\"".Length, objeto.Length - ("\"".Length * 2)); //quito la 1ª y última " para que no las reemplace por ''
                    }

                    if (esTripletaEntidadPadre && triple.Predicate.Contains(predicadoRdfType) && !tipoAgregado)
                    {
                        tripletas.Append(FacetadoAD.GenerarTripleta(sujetoDocumentoId, "<http://gnoss/type>", $"\"{UtilCadenas.EliminarHtmlDeTexto(objeto)}\""));
                        tipoAgregado = true;
                    }

                    if (!(triple.Predicate).Contains(predicadoRdfType) && !(triple.Predicate).Contains("http://www.w3.org/2000/01/rdf-schema#label"))
                    {
                        if (!TextoInvariable.Contains(triple.Predicate.Trim('<').Trim('>')))
                        {
                            objeto = UtilCadenas.EliminarHtmlDeTexto(objeto);
                        }

                        if (Fecha.Contains(predicado))
                        {
                            string fechaConvertida = ConvertirFormatoFecha(objeto);
                            long fechaInt;
                            if (long.TryParse(fechaConvertida, out fechaInt))
                            {
                                objeto = $"{fechaConvertida} . ";
                            }
                            else
                            {
                                objeto = $"\"{fechaConvertida}\" . ";
                            }
                        }

                        bool esEntidadAuxiliar = pListaTriplesRecurso.Exists(item => item.Object.Equals(sujeto));
                        if (esEntidadAuxiliar)
                        {
                            sujeto = PasarObjetoALower(sujeto);
                        }
                        ComprobarPropiedadEsRangoFecha(filasRangoFechas, predicado, objeto, pListaTriplesRecurso, propiedadFechaInicioValorFechaInicio, propiedadFechaFinValorFechaFin);

                        if (esTripletaEntidadPadre)
                        {
                            tripletas.Append(GenerarTripletaRecogidadeVirtuoso_ControlCheckPoint(pFicheroConfiguracionBD, pFicheroConfiguracionBDBase, pUrlIntragnoss, sujetoDocumentoId, triple.Predicate, PasarObjetoALower(objeto), objeto, Fecha, Numero, TextoInvariable, EntExt, triple.ObjectLanguage));
                        }
                        else
                        {
                            tripletas.Append(GenerarTripletaRecogidadeVirtuoso_ControlCheckPoint(pFicheroConfiguracionBD, pFicheroConfiguracionBDBase, pUrlIntragnoss, PasarObjetoALower(sujeto), predicado, PasarObjetoALower(objeto), objeto, Fecha, Numero, TextoInvariable, EntExt, triple.ObjectLanguage));

                            //insertamos tripleta de la entidad auxiliar
                            tripletas.Append(GenerarTripletaRecogidadeVirtuoso_ControlCheckPoint(pFicheroConfiguracionBD, pFicheroConfiguracionBDBase, pUrlIntragnoss, sujetoAuxiliar, predicadoAuxiliar, PasarObjetoALower(sujeto), sujeto, Fecha, Numero, TextoInvariable, EntExt, null));
                        }
                    }
                }
            }

            if (propiedadFechaInicioValorFechaInicio.Count > 0)
            {
                foreach (string propiedadFechaInicio in propiedadFechaInicioValorFechaInicio.Keys)
                {
                    DateTime? valorPropiedadFechaInicio = propiedadFechaInicioValorFechaInicio[propiedadFechaInicio];
                    DateTime? valorPropiedadFechaFin = null;
                    string valorPropiedad = ConvertirFormatoFecha(valorPropiedadFechaInicio.Value.ToString("dd/MM/yyyy"));
                    FacetaConfigProyRangoFecha filaRangoFecha = filasRangoFechas.First(fila => fila.PropiedadInicio.Equals(propiedadFechaInicio));

                    if (propiedadFechaFinValorFechaFin.ContainsKey(filaRangoFecha.PropiedadFin))
                    {
                        valorPropiedadFechaFin = propiedadFechaFinValorFechaFin[filaRangoFecha.PropiedadFin];
                    }

                    long fechaInt;
                    if (long.TryParse(valorPropiedad, out fechaInt))
                    {
                        valorPropiedad = valorPropiedad + " . ";
                    }
                    else
                    {
                        valorPropiedad = "\"" + valorPropiedad + "\" . ";
                    }

                    tripletas.Append(GenerarTripletaRecogidadeVirtuoso_ControlCheckPoint(pFicheroConfiguracionBD, pFicheroConfiguracionBDBase, pUrlIntragnoss, "<http://gnoss/" + pDocumentoID.ToString().ToUpper() + ">", filaRangoFecha.PropiedadNueva, PasarObjetoALower(valorPropiedad), valorPropiedad, Fecha, Numero, TextoInvariable, EntExt, null));
                    if (valorPropiedadFechaFin.HasValue && valorPropiedadFechaFin.Value > valorPropiedadFechaInicio.Value)
                    {
                        DateTime temp = valorPropiedadFechaInicio.Value.AddDays(1);

                        while (temp <= valorPropiedadFechaFin.Value)
                        {
                            string valorPropiedadTemp = ConvertirFormatoFecha(temp.ToString("dd/MM/yyyy"));
                            long fechaInt2;
                            if (long.TryParse(valorPropiedadTemp, out fechaInt2))
                            {
                                valorPropiedadTemp = valorPropiedadTemp + " . ";
                            }
                            else
                            {
                                valorPropiedadTemp = "\"" + valorPropiedadTemp + "\" . ";
                            }

                            tripletas.Append(GenerarTripletaRecogidadeVirtuoso_ControlCheckPoint(pFicheroConfiguracionBD, pFicheroConfiguracionBDBase, pUrlIntragnoss, "<http://gnoss/" + pDocumentoID.ToString().ToUpper() + ">", filaRangoFecha.PropiedadNueva, PasarObjetoALower(valorPropiedadTemp), valorPropiedadTemp, Fecha, Numero, TextoInvariable, EntExt, null));
                            temp = temp.AddDays(1);

                        }
                    }
                }
            }

            return tripletas.ToString();
        }

        private static void ComprobarPropiedadEsRangoFecha(List<FacetaConfigProyRangoFecha> pFilasRangoFechas, string pPredicado, string pObjeto, List<TripleWrapper> pListaTriplesRecurso, Dictionary<string, DateTime?> pPropiedadFechaInicioValorFechaInicio, Dictionary<string, DateTime?> pPropiedadFechaFinValorFechaFin)
        {
            if (pFilasRangoFechas.Exists(item => item.PropiedadInicio.Equals(pPredicado)))
            {
                string fechaInicio = pObjeto;
                AgregarFechaAListaRangosFecha(pPropiedadFechaInicioValorFechaInicio, pPredicado, fechaInicio);
            }
            else if (pFilasRangoFechas.Exists(item => item.PropiedadFin.Equals(pPredicado)))
            {
                string fechaFin = pObjeto;
                AgregarFechaAListaRangosFecha(pPropiedadFechaFinValorFechaFin, pPredicado, fechaFin);
            }
            else
            {
                if (pFilasRangoFechas.Exists(item => item.PropiedadInicio.StartsWith($"{pPredicado}@@@")))
                {
                    var propiedad = pFilasRangoFechas.First(item => item.PropiedadInicio.StartsWith($"{pPredicado}@@@"));
                    string fechaInicio = NavegarEntreListaObjetosHastaUltimoNivel(propiedad.PropiedadInicio, pPredicado, pObjeto, pListaTriplesRecurso);

                    AgregarFechaAListaRangosFecha(pPropiedadFechaInicioValorFechaInicio, propiedad.PropiedadInicio, fechaInicio);
                }
                if (pFilasRangoFechas.Exists(item => item.PropiedadFin.StartsWith($"{pPredicado}@@@")))
                {
                    var propiedad = pFilasRangoFechas.First(item => item.PropiedadFin.StartsWith($"{pPredicado}@@@"));
                    string fechaFin = NavegarEntreListaObjetosHastaUltimoNivel(propiedad.PropiedadFin, pPredicado, pObjeto, pListaTriplesRecurso);

                    AgregarFechaAListaRangosFecha(pPropiedadFechaFinValorFechaFin, propiedad.PropiedadFin, fechaFin);
                }
            }
        }

        private static string NavegarEntreListaObjetosHastaUltimoNivel(string pPropiedades, string pPredicado, string pObjeto, List<TripleWrapper> pListaTriplesRecurso)
        {
            string ultimoNivel = pObjeto;
            string[] todasPropiedades = pPropiedades.Remove(0, pPredicado.Length).Split(new string[] { "@@@" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string propiedad in todasPropiedades)
            {
                if (!ultimoNivel.StartsWith('<') && !ultimoNivel.EndsWith('>'))
                {
                    ultimoNivel = $"<{ultimoNivel}>";
                }
                TripleWrapper item = pListaTriplesRecurso.FirstOrDefault(triples => triples.Subject.Equals(ultimoNivel) && triples.Predicate.Equals($"<{propiedad}>"));

                if (item != null)
                {
                    ultimoNivel = item.Object;
                }
                else
                {
                    //No hemos podido llegar hasta el último nivel
                    ultimoNivel = null;
                    break;
                }
            }

            if (!string.IsNullOrEmpty(ultimoNivel) && ultimoNivel.StartsWith("\"") && ultimoNivel.EndsWith("\""))
            {
                ultimoNivel = ultimoNivel.Substring("\"".Length, ultimoNivel.Length - ("\"".Length * 2)); //quito la 1ª y última " para que no las reemplace por ''
            }

            return ultimoNivel;
        }

        private static void AgregarFechaAListaRangosFecha(Dictionary<string, DateTime?> propiedadFechaValorFecha, string predicado, string objeto)
        {
            DateTime fecha;
            if (DateTime.TryParseExact(DesconvertirFormatoFecha(objeto), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha) && !propiedadFechaValorFecha.ContainsKey(predicado))
            {
                propiedadFechaValorFecha.Add(predicado, fecha);
            }
        }

        private static void ObtenerListaPropiedadesFechaNumeroTextoIvariable(DataWrapperFacetas pFacetaDW, Guid pProyectoID, Dictionary<string, string> urlOntologiaPorNamespace, ref List<string> Fecha, ref List<string> Numero, ref List<string> TextoInvariable, ref List<string> listaPropiedades)
        {
            //Primero recorremos los objetos de conocimiento del proyecto
            List<FacetaObjetoConocimientoProyecto> filas = pFacetaDW.ListaFacetaObjetoConocimientoProyecto.Where(item => item.ProyectoID.Equals(pProyectoID)).ToList();

            foreach (FacetaObjetoConocimientoProyecto myrow in filas)
            {
                bool autocompletar = (bool)myrow.Autocompletar;
                if (autocompletar)
                {
                    string propiedad = myrow.Faceta;
                    string propiedadSinPrefijo = propiedad.Substring(propiedad.LastIndexOf(":") + 1);

                    if (!listaPropiedades.Contains(propiedadSinPrefijo))
                    {
                        listaPropiedades.Add(propiedadSinPrefijo);
                    }
                }

                AgregarFacetaADiccionario(myrow.TipoPropiedad.Value, myrow.Faceta, urlOntologiaPorNamespace, ref Fecha, ref Numero, ref TextoInvariable);
            }

            //Recorremos la tabla con los objetos de conocimiento.
            foreach (FacetaObjetoConocimiento myrow in pFacetaDW.ListaFacetaObjetoConocimiento)
            {
                bool autocompletar = (bool)myrow.Autocompletar;
                if (autocompletar)
                {
                    string propiedad = myrow.Faceta;
                    string propiedadSinPrefijo = propiedad.Substring(propiedad.LastIndexOf(":") + 1);
                    if (!listaPropiedades.Contains(propiedadSinPrefijo))
                    {
                        listaPropiedades.Add(propiedadSinPrefijo);
                    }
                }

                AgregarFacetaADiccionario(myrow.TipoPropiedad.Value, myrow.Faceta, urlOntologiaPorNamespace, ref Fecha, ref Numero, ref TextoInvariable);
            }
        }
        #endregion

        #endregion

        /// <summary>
        /// (OLD) Realizamos una consulta sencilla al servidor para saber si ya está operativo
        /// </summary>
        /// <returns>TRUE = Servidor Levantado y operativo</returns>
        public bool ServidorOperativo(string pFicheroConfiguracionBD, string pUrlIntragnoss)
        {
            FacetadoCN pFacetadoCN = null;
            bool servidorOperativo = false;

            try
            {
                if (string.IsNullOrEmpty(pFicheroConfiguracionBD))
                {
                    pFacetadoCN = new FacetadoCN(pUrlIntragnoss, false, "", mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                }
                else
                {
                    pFacetadoCN = new FacetadoCN(pFicheroConfiguracionBD, pUrlIntragnoss, "", mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                }

                //pFacetadoCN = new FacetadoCN(pFicheroConfiguracionBD, false, pUrlIntragnoss, "");
                //Hacemos una consulta sencilla a virtuoso:
                string query = "SPARQL ASK {?s ?p ?o.}";
                pFacetadoCN.LeerDeVirtuoso(query, "TablaDePrueba", null);

                //GuardarLog("Servidor Operativo.", IpServidor);
                servidorOperativo = true;
            }
            catch (Exception)
            {
                //GuardarLog("Todavía no está operativo. '" + ex.Message + "'", IpServidor);
                servidorOperativo = false;

                //Cerramos las conexiones
                ControladorConexiones.CerrarConexiones();
            }
            finally
            {
                pFacetadoCN.Dispose();
                pFacetadoCN = null;
            }

            return servidorOperativo;
        }

        /// <summary>
        /// Realizamos una consulta sencilla al servidor para saber si ya está operativo
        /// </summary>
        /// <returns>TRUE = Servidor Levantado y operativo</returns>
        public bool VirtuosoOperativo(string pCadenaConexionVirtuoso)
        {
            VirtuosoAD virtuosoAD = null;
            bool servidorOperativo = false;

            try
            {
                //Hacemos una consulta sencilla a virtuoso:
                string query = "ASK {?s ?p ?o.}";
                mVirtuosoAD.LeerDeVirtuoso_WebClient(query, "TablaDePrueba");

                //GuardarLog("Servidor Operativo.", IpServidor);
                servidorOperativo = true;
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, pCadenaConexionVirtuoso);
                servidorOperativo = false;

                //Cerramos las conexiones
                ControladorConexiones.CerrarConexiones();
            }
            finally
            {
                if (virtuosoAD != null)
                {
                    virtuosoAD.Dispose();
                    virtuosoAD = null;
                }
            }

            return servidorOperativo;
        }

        /// <summary>
        /// Obtiene los filtros para obtener los contextos
        /// </summary>
        /// <param name="pFiltrosOrigenDestino">Fila gadget</param>
        public string Obtenerfiltros(string pFiltrosOrigenDestino, Guid pDocumentoID, Guid proyectoID, Guid pProyectoOrigenBusquedaID, string pUrlIntragnoss, Dictionary<string, List<string>> pInformacionOntologias, Guid pProyectoOrigenContextoID, GestionFacetas pGestorFacetas, bool pUsarAfinidad = false)
        {
            return Obtenerfiltros(pFiltrosOrigenDestino, pDocumentoID, proyectoID, pProyectoOrigenBusquedaID, pUrlIntragnoss, pInformacionOntologias, string.Empty, string.Empty, string.Empty, pProyectoOrigenContextoID, pGestorFacetas, pUsarAfinidad);
        }

        /// <summary>
        /// Obtiene los filtros para obtener los contextos
        /// </summary>
        /// <param name="pFiltrosOrigenDestino">Fila gadget</param>
        public string Obtenerfiltros(string pFiltrosOrigenDestino, Guid pDocumentoID, Guid proyectoID, Guid pProyectoOrigenBusquedaID, string pUrlIntragnoss, Dictionary<string, List<string>> pInformacionOntologias, string pIdioma, Guid pProyectoOrigenContextoID, GestionFacetas pGestorFacetas, bool pUsarAfinidad = false)
        {
            return Obtenerfiltros(pFiltrosOrigenDestino, pDocumentoID, proyectoID, pProyectoOrigenBusquedaID, pUrlIntragnoss, pInformacionOntologias, pIdioma, string.Empty, string.Empty, pProyectoOrigenContextoID, pGestorFacetas, pUsarAfinidad);
        }

        /// <summary>
        /// Obtiene los filtros para obtener los contextos
        /// </summary>
        /// <param name="pFiltrosOrigenDestino">Fila gadget</param>
        public string Obtenerfiltros(string pFiltrosOrigenDestino, Guid pDocumentoID, Guid pProyectoID, Guid pProyectoOrigenBusquedaID, string pUrlIntragnoss, Dictionary<string, List<string>> pInformacionOntologias, string pIdioma, string pFicheroConfigBD, string pGrafo, Guid pProyectoOrigenContextoID, GestionFacetas pGestorFacetas, bool pUsarAfinidad = false)
        {
            string filtros = "";
            string[] filas = pFiltrosOrigenDestino.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
            List<string[]> L = new List<string[]>();
            List<string> comDest = new List<string>();
            List<string> pesos = new List<string>();

            Dictionary<string, string> d = new Dictionary<string, string>();

            foreach (string fila in filas)
            {
                string[] datos = fila.Split('|');
                if (!L.Contains(datos))
                {
                    L.Add(datos);
                }

                if (!comDest.Contains(datos[0]))
                {
                    comDest.Add(datos[0]);
                }
                if (!pesos.Contains(datos[2]))
                {
                    pesos.Add(datos[2]);
                }

            }
            string select = "(";

            //Obtenemos propiedades del Recurso
            List<string> propiedadesAObtener = new List<string>();
            foreach (string peso in pesos)
            {
                foreach (string comDes in comDest)
                {
                    if (peso.Equals("distancia2d"))
                    {
                        if (!propiedadesAObtener.Contains(comDes))
                        {
                            propiedadesAObtener.Add(comDes);
                        }
                    }
                }
            }
            Dictionary<string, string> datosRecurso = new Dictionary<string, string>();
            if (propiedadesAObtener.Count > 0)
            {
                if (!pFicheroConfigBD.Equals(string.Empty) && !pGrafo.Equals(string.Empty))
                {
                    datosRecurso = ObtenerDatosRecurso(propiedadesAObtener, pDocumentoID, pProyectoID, pProyectoOrigenBusquedaID, pUrlIntragnoss, pInformacionOntologias, pIdioma, pFicheroConfigBD, pGrafo, pUsarAfinidad);
                }
                else
                {
                    datosRecurso = ObtenerDatosRecurso(propiedadesAObtener, pDocumentoID, pProyectoID, pProyectoOrigenBusquedaID, pUrlIntragnoss, pInformacionOntologias, pIdioma, pUsarAfinidad);
                }
            }

            bool cerrarParentesisDistancia = false;
            foreach (string peso in pesos)
            {
                if (!peso.Equals("distancia2d"))
                {
                    //Se ha puesto enre parentesis y se hacer el replace para los pesos negativos
                    select += "(" + peso + ")* count(?o" + peso.Replace("-", "_") + ") + ";
                }
                int i = 0;
                foreach (string comDes in comDest)
                {
                    if (peso.Equals("distancia2d"))
                    {
                        string comDesString = datosRecurso[comDes];
                        comDesString = comDesString.Substring(1, comDesString.Length - 2);
                        if (i == 0)
                        {
                            select += " -1*(";
                            cerrarParentesisDistancia = true;
                        }
                        if (comDesString.StartsWith("-"))
                        {
                            comDesString = comDesString.Substring(1);
                            comDesString = "(" + comDesString + " * -1)";
                        }

                        select += "(xsd:float(?o" + i + ") - " + comDesString.Replace(",", ".") + ") * (xsd:float(?o" + i + ")- " + comDesString.Replace(",", ".") + " ) +";
                        i++;
                    }
                    else
                    {
                        string valores = "";
                        foreach (string[] l in L)
                        {
                            if (l[2].Equals(peso) && l[0].Equals(comDes))
                            {
                                valores += l[1] + ", ";
                            }

                        }

                        if (!string.IsNullOrEmpty(valores))
                        {
                            d.Add(peso + "|" + comDes, valores.Substring(0, valores.Length - 2));
                        }
                    }
                }
            }

            string filtro = "";

            if (pesos[0].Equals("distancia2d"))
            {
                //filtro = "{?s <http://www.w3.org/2003/01/geo/wgs84_pos#location> ?a. ?a ?p ?o1 .  ?a ?p2 ?o0 .FILTER(?p=<http://www.w3.org/2003/01/geo/wgs84_pos#long>  and ?p2=<http://www.w3.org/2003/01/geo/wgs84_pos#lat>)} ";

                if (propiedadesAObtener.Count == 2)
                {
                    string[] props = propiedadesAObtener[0].Split(new string[] { "@@@" }, StringSplitOptions.RemoveEmptyEntries);
                    string[] props2 = propiedadesAObtener[1].Split(new string[] { "@@@" }, StringSplitOptions.RemoveEmptyEntries);

                    string sujeto = "?s";
                    filtro = "{";

                    for (int i = 0; i < props.Length - 1; i++)
                    {
                        filtro += $"{sujeto} {props[i]} ?objeto{i}. ";
                        sujeto = $"?objeto{i}";
                    }

                    filtro += $"{sujeto} {props[props.Length - 1]} ?o0. {sujeto} {props2[props2.Length - 1]} ?o1. }}";

                    //filtro = "{?s " + props[0] + " ?a. ?a ?p ?o1 .  ?a ?p2 ?o0 .FILTER(?p=" + props2[1] + "  and ?p2=" + props[1] + ")} ";
                }
                else
                {
                    filtro = "{?s <http://www.w3.org/2003/01/geo/wgs84_pos#location> ?a. ?a ?p ?o1 .  ?a ?p2 ?o0 .FILTER(?p=<http://www.w3.org/2003/01/geo/wgs84_pos#long>  and ?p2=<http://www.w3.org/2003/01/geo/wgs84_pos#lat>)} ";
                }
            }
            else
            {
                datosRecurso = new Dictionary<string, string>();
                propiedadesAObtener = new List<string>();
                if (!pesos[0].Equals("distancia2d"))
                {
                    foreach (KeyValuePair<string, string> item in d)
                    {
                        string clave = item.Key;
                        string peso = clave.Substring(0, clave.IndexOf("|"));
                        string tags = clave.Substring(clave.IndexOf("|") + 1);
                        propiedadesAObtener.Add(tags);
                    }
                }
                if (propiedadesAObtener.Count > 0)
                {
                    if (!pFicheroConfigBD.Equals(string.Empty) && !pGrafo.Equals(string.Empty))
                    {
                        datosRecurso = ObtenerDatosRecurso(propiedadesAObtener, pDocumentoID, pProyectoID, pProyectoOrigenBusquedaID, pUrlIntragnoss, pInformacionOntologias, pIdioma, pFicheroConfigBD, pGrafo, pUsarAfinidad);
                    }
                    else
                    {
                        datosRecurso = ObtenerDatosRecurso(propiedadesAObtener, pDocumentoID, pProyectoID, pProyectoOrigenBusquedaID, pUrlIntragnoss, pInformacionOntologias, pIdioma, pUsarAfinidad);
                    }
                }

                foreach (KeyValuePair<string, string> item in d)
                {
                    string clave = item.Key;
                    string peso = clave.Substring(0, clave.IndexOf("|"));


                    string tags = clave.Substring(clave.IndexOf("|") + 1);
                    tags = datosRecurso[tags.Replace("[MultiIdioma]", "")];

                    string multiIdioma = "";
                    if (item.Value.Contains("[MultiIdioma]"))
                    {
                        multiIdioma = "@" + pIdioma;
                    }

                    if (!string.IsNullOrEmpty(multiIdioma))
                    {
                        //se han añadido las \" al split porque el tag puede contener comas
                        string[] tagsArray = tags.Split(new string[] { "\",\"" }, StringSplitOptions.RemoveEmptyEntries);

                        tags = "";
                        string coma = "";
                        foreach (string tag in tagsArray)
                        {
                            //tags += coma + "\"" + tag.Replace("\"", "") + "\"" + multiIdioma;
                            tags += coma + "\"" + tag.Trim('\"') + "\"" + multiIdioma;
                            coma = ",";
                        }
                    }

                    List<string> propiedades = new List<string>();

                    if (item.Value.Contains("@@@") && item.Value.Contains(","))
                    {
                        //Si tiene niveles y hay mas de uno hay que mandar por separado las propiedades con niveles
                        string[] propSeparators = new string[] { "," };
                        string[] props = item.Value.Split(propSeparators, StringSplitOptions.None);

                        foreach (string prop in props)
                        {
                            propiedades.Add(prop.Replace("[MultiIdioma]", ""));
                        }
                    }
                    else
                    {
                        //Si no tiene niveles o solo hay uno se manda de una vez
                        propiedades.Add(item.Value.Replace("[MultiIdioma]", ""));
                    }

                    foreach (string propiedad in propiedades)
                    {
                        bool propInvariante = pGestorFacetas != null && pGestorFacetas.ListaFacetasPorClave.ContainsKey(propiedad) && pGestorFacetas.ListaFacetasPorClave[propiedad].TipoPropiedad == TipoPropiedadFaceta.TextoInvariable;
                        bool propNumerica = pGestorFacetas != null && pGestorFacetas.ListaFacetasPorClave.ContainsKey(propiedad) && (pGestorFacetas.ListaFacetasPorClave[propiedad].TipoPropiedad == TipoPropiedadFaceta.Numero || pGestorFacetas.ListaFacetasPorClave[propiedad].TipoPropiedad == TipoPropiedadFaceta.Fecha);

                        string[] stringSeparators = new string[] { "@@@" };
                        string[] LNiveles = propiedad.Split(stringSeparators, StringSplitOptions.None);

                        if (!string.IsNullOrEmpty(tags))
                        {
                            if (propNumerica)
                            {
                                tags = tags.Trim('\"');
                            }

                            filtro += "{";
                            if (LNiveles.Length > 1)
                            {
                                if (LNiveles[0].StartsWith("http:"))
                                {
                                    filtro += "?s ?p0 ?nivel0 . filter (?p0=<" + LNiveles[0] + "> )";
                                }
                                else
                                {
                                    filtro += "?s ?p0 ?nivel0 . filter (?p0=" + LNiveles[0] + " )";
                                }
                                int i = 0;
                                for (i = 0; i < LNiveles.Length - 2; i++)
                                {
                                    if (LNiveles[i + 1].StartsWith("http:"))
                                    {
                                        filtro += " ?nivel" + i + " ?p" + (i + 1) + " ?nivel" + (i + 1) + ".  filter (?p" + (i + 1) + "=<" + LNiveles[i + 1] + "> )";
                                    }
                                    else
                                    {
                                        filtro += " ?nivel" + i + " ?p" + (i + 1) + " ?nivel" + (i + 1) + ".  filter (?p" + (i + 1) + "=" + LNiveles[i + 1] + " )";
                                    }


                                }
                                if (LNiveles[i + 1].StartsWith("http:"))
                                {
                                    filtro += " ?nivel" + i + " ?p  ?o" + peso.Replace("-", "_") + " .   filter (?p=<" + LNiveles[i + 1] + "> ";
                                }
                                else
                                {
                                    filtro += " ?nivel" + i + " ?p  ?o" + peso.Replace("-", "_") + " .   filter (?p=" + LNiveles[i + 1] + " ";
                                }

                            }
                            else
                            {
                                if (item.Value.StartsWith("http:"))
                                {
                                    filtro += "?s ?p ?o" + peso.Replace("-", "_") + " filter (?p in (<" + propiedad + ">) ";
                                }
                                else { filtro += "?s ?p ?o" + peso.Replace("-", "_") + " filter (?p in (" + propiedad + ")  "; }
                            }

                            if (tags.Contains("gnoss:"))
                            {
                                filtro += " and ?o" + peso.Replace("-", "_") + "  in (" + tags + ")) } UNION ";
                            }
                            else
                            {
                                //Si el contexto es contra la propia comunidad y la faceta sobre la que se va a hacer la búsqueda está configurada como invariante NO hacemos el ToLower()
                                if (pProyectoID.Equals(pProyectoOrigenContextoID) && propInvariante)
                                {
                                    filtro += " and ?o" + peso.Replace("-", "_") + "  in (" + tags + ")) } UNION ";
                                }
                                else
                                {
                                    filtro += " and ?o" + peso.Replace("-", "_") + "  in (" + tags.ToLowerSearchGraph() + ")) } UNION ";
                                }
                            }
                        }
                    }
                }

                if (filtro.Length > 0)
                {
                    filtro = filtro.Substring(0, filtro.Length - 6);
                }
            }
            select = select.Substring(0, select.Length - 2);
            if (cerrarParentesisDistancia)
            {
                select += ")";
            }

            filtros = select + ")&&&" + filtro;

            return filtros;
        }


        public string ObtenerfiltrosOrigen(string pFiltrosOrigen, Guid pDocumentoID, Guid proyectoID, Guid pProyectoOrigenBusquedaID, string pUrlIntragnoss, Dictionary<string, List<string>> pInformacionOntologias, bool pUsarAfinidad = false)
        {
            return ObtenerfiltrosOrigen(pFiltrosOrigen, pDocumentoID, proyectoID, pProyectoOrigenBusquedaID, pUrlIntragnoss, pInformacionOntologias, string.Empty, string.Empty, pUsarAfinidad);
        }

        public string ObtenerfiltrosOrigen(string pFiltrosOrigen, Guid pDocumentoID, Guid proyectoID, Guid pProyectoOrigenBusquedaID, string pUrlIntragnoss, Dictionary<string, List<string>> pInformacionOntologias, string pFicheroConfigBD, string pGrafo, bool pUsarAfinidad = false)
        {
            string filtrosOrigenAux = pFiltrosOrigen;

            //Diccionario con el formato:
            //Clave: Numero de la propiedad
            //Valor: Bloque,Propiedad
            Dictionary<int, KeyValuePair<string, string>> listaKeyPropiedades = new Dictionary<int, KeyValuePair<string, string>>();
            List<string> listaPropiedades = new List<string>();
            List<string> listaPropiedadesObligatorias = new List<string>();
            int inicio = 0;
            string strinicioBloque = "#" + inicio + "#";
            string strinicioPropiedadString = "@" + inicio + "@";
            string strinicioPropiedadInt = "$" + inicio + "$";
            while (filtrosOrigenAux.Contains(strinicioBloque))
            {
                filtrosOrigenAux = filtrosOrigenAux.Substring(filtrosOrigenAux.IndexOf(strinicioBloque) + strinicioBloque.Length);
                string nombreBloque = filtrosOrigenAux.Substring(0, filtrosOrigenAux.IndexOf(strinicioBloque));


                string strinicioPropiedad = strinicioPropiedadString;
                if (nombreBloque.Contains(strinicioPropiedadInt))
                {
                    strinicioPropiedad = strinicioPropiedadInt;
                }

                string nombrePropiedad = nombreBloque.Substring(nombreBloque.IndexOf(strinicioPropiedad) + strinicioPropiedad.Length);
                nombrePropiedad = nombrePropiedad.Substring(0, nombrePropiedad.IndexOf(strinicioPropiedad));

                if (nombrePropiedad.StartsWith("*"))
                {
                    nombrePropiedad = nombrePropiedad.Substring(1);
                    if (!listaPropiedadesObligatorias.Contains(nombrePropiedad))
                    {
                        listaPropiedadesObligatorias.Add(nombrePropiedad);
                    }
                }

                if (!listaPropiedades.Contains(nombrePropiedad))
                {
                    listaPropiedades.Add(nombrePropiedad);
                }
                listaKeyPropiedades.Add(inicio, new KeyValuePair<string, string>(nombreBloque, nombrePropiedad));

                inicio++;
                strinicioPropiedadString = "@" + inicio + "@";
                strinicioPropiedadInt = "$" + inicio + "$";
                strinicioBloque = "#" + inicio + "#";
            }

            Dictionary<string, string> datosRecurso = new Dictionary<string, string>();
            if (listaPropiedades.Count > 0)
            {
                if (!pFicheroConfigBD.Equals(string.Empty) && !pGrafo.Equals(string.Empty))
                {
                    datosRecurso = ObtenerDatosRecurso(listaPropiedades, pDocumentoID, proyectoID, pProyectoOrigenBusquedaID, pUrlIntragnoss, pInformacionOntologias, "", pFicheroConfigBD, pGrafo, pUsarAfinidad);
                }
                else
                {
                    datosRecurso = ObtenerDatosRecurso(listaPropiedades, pDocumentoID, proyectoID, pProyectoOrigenBusquedaID, pUrlIntragnoss, pInformacionOntologias, pUsarAfinidad);
                }
            }

            foreach (string propiedadObligatoria in listaPropiedadesObligatorias)
            {
                if (string.IsNullOrEmpty(datosRecurso[propiedadObligatoria]))
                {
                    return "nomostrar";
                }
            }

            pFiltrosOrigen = pFiltrosOrigen.Replace("*", "");
            foreach (int key in listaKeyPropiedades.Keys)
            {
                string bloque = listaKeyPropiedades[key].Key;
                if (bloque.Contains("*"))
                {
                    bloque = bloque.Replace("*", "");
                }
                string propiedad = listaKeyPropiedades[key].Value;

                string valorPropiedad = datosRecurso[propiedad];

                if (string.IsNullOrEmpty(valorPropiedad))
                {
                    pFiltrosOrigen = pFiltrosOrigen.Replace("#" + key + "#" + bloque + "#" + key + "#", "");
                }
                else
                {
                    string bloqueAux = "";
                    if (bloque.Contains("$"))
                    {
                        bloqueAux = bloque.Replace("$" + key + "$" + propiedad + "$" + key + "$", valorPropiedad.Replace("\"", ""));
                    }
                    else
                    {
                        bloqueAux = bloque.Replace("@" + key + "@" + propiedad + "@" + key + "@", valorPropiedad);
                    }
                    pFiltrosOrigen = pFiltrosOrigen.Replace("#" + key + "#" + bloque + "#" + key + "#", bloqueAux);
                }
            }

            return pFiltrosOrigen;
        }

        /// <summary>
        /// Obtiene los datos de varias propiedades del recurso
        /// </summary>
        /// <param name="pPropiedades">Propiedades</param>
        private Dictionary<string, string> ObtenerDatosRecurso(List<string> pPropiedades, Guid pDocumentoID, Guid proyectoID, Guid pProyectoOrigenBusquedaID, string pUrlIntragnoss, Dictionary<string, List<string>> pInformacionOntologias, bool pUsarAfinidad = false)
        {
            return ObtenerDatosRecurso(pPropiedades, pDocumentoID, proyectoID, pProyectoOrigenBusquedaID, pUrlIntragnoss, pInformacionOntologias, string.Empty, string.Empty, string.Empty, pUsarAfinidad);
        }

        /// <summary>
        /// Obtiene los datos de varias propiedades del recurso
        /// </summary>
        /// <param name="pPropiedades">Propiedades</param>
        private Dictionary<string, string> ObtenerDatosRecurso(List<string> pPropiedades, Guid pDocumentoID, Guid proyectoID, Guid pProyectoOrigenBusquedaID, string pUrlIntragnoss, Dictionary<string, List<string>> pInformacionOntologias, string pIdioma, bool pUsarAfinidad = false)
        {
            return ObtenerDatosRecurso(pPropiedades, pDocumentoID, proyectoID, pProyectoOrigenBusquedaID, pUrlIntragnoss, pInformacionOntologias, pIdioma, string.Empty, string.Empty, pUsarAfinidad);
        }

        /// <summary>
        /// Obtiene los datos de varias propiedades del recurso
        /// </summary>
        /// <param name="pPropiedades">Propiedades</param>
        private Dictionary<string, string> ObtenerDatosRecurso(List<string> pPropiedades, Guid pDocumentoID, Guid proyectoID, Guid pProyectoOrigenBusquedaID, string pUrlIntragnoss, Dictionary<string, List<string>> pInformacionOntologias, string pIdioma, string pFicheroConfigDB, string pGrafo, bool pUsarAfinidad = false)
        {
            List<string> propiedadesAux = new List<string>();
            foreach (string prop in pPropiedades)
            {
                if (!propiedadesAux.Contains(prop))
                {
                    propiedadesAux.Add(prop);
                }
            }
            pPropiedades = propiedadesAux;


            Dictionary<string, string> diccionarioDatos = new Dictionary<string, string>();

            List<Guid> recursoID = new List<Guid>();
            recursoID.Add(pDocumentoID);

            string grafoBusqueda = proyectoID.ToString();
            if (pProyectoOrigenBusquedaID != Guid.Empty)
            {
                //Si es una vista busca en el virtuoso de la comunidad origen
                grafoBusqueda = pProyectoOrigenBusquedaID.ToString();
            }

            FacetadoCN facetadoCN = null;
            if (!pFicheroConfigDB.Equals(string.Empty) && !pGrafo.Equals(string.Empty))
            {
                facetadoCN = new FacetadoCN(pFicheroConfigDB, pUrlIntragnoss, pGrafo, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            }
            else
            {
                facetadoCN = new FacetadoCN(pUrlIntragnoss, true, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            }
            facetadoCN.InformacionOntologias = pInformacionOntologias;
            FacetadoDS facetadoDS = facetadoCN.ObtenerValoresPropiedadesEntidadesPorDocumentoID(grafoBusqueda, recursoID, pPropiedades, pIdioma, true, pUsarAfinidad);
            string namespacesVirtuoso = facetadoCN.FacetadoAD.NamespacesVirtuosoLectura;
            facetadoCN.Dispose();


            //Namespaces virtuoso
            namespacesVirtuoso = namespacesVirtuoso.Replace("SPARQL", "");
            string[] prefijos = namespacesVirtuoso.Split(new string[] { "prefix" }, StringSplitOptions.RemoveEmptyEntries);

            Dictionary<string, string> listaNamespacePrefijo = new Dictionary<string, string>();
            foreach (string pref in prefijos)
            {
                string cadena = pref.Trim();
                if (!string.IsNullOrEmpty(cadena))
                {
                    int indicePuntos = cadena.IndexOf(":");

                    string prefijo = cadena.Substring(0, indicePuntos).Trim();
                    string url = cadena.Substring(indicePuntos + 1).Trim().Replace("<", "").Replace(">", "");

                    if (!listaNamespacePrefijo.ContainsKey(url))
                    {
                        listaNamespacePrefijo.Add(url, prefijo);
                    }
                }
            }

            foreach (DataRow fila in facetadoDS.Tables[0].Rows)
            {
                string valor = fila["o"].ToString();
                if (valor.StartsWith("http://gnoss/"))
                {
                    //Categorías
                    valor = valor.Replace("http://gnoss/", "gnoss:");
                }
                else
                {
                    if (valor.StartsWith("http:"))
                    {
                        valor = "<" + valor + ">";
                    }
                    else
                    {
                        valor = "\"" + valor + "\"";
                    }
                }

                if (!pPropiedades.Contains(fila["p"].ToString()))
                {
                    foreach (string url in listaNamespacePrefijo.Keys)
                    {
                        if (fila["p"].ToString().StartsWith(url) || fila["p"].ToString().Contains("@@@" + url))
                        {
                            fila["p"] = fila["p"].ToString().Replace(url, listaNamespacePrefijo[url] + ":");
                        }
                    }
                }

                if (diccionarioDatos.ContainsKey(fila["p"].ToString()))
                {
                    diccionarioDatos[fila["p"].ToString()] += "," + valor;
                }
                else
                {
                    diccionarioDatos.Add(fila["p"].ToString(), valor);
                }
            }

            //Rellenamos vacias las que no existan
            foreach (string propiedad in pPropiedades)
            {
                string propAux = propiedad.Replace("[MultiIdioma]", "");
                if (!diccionarioDatos.ContainsKey(propAux))
                {
                    diccionarioDatos.Add(propAux, "");
                }
            }

            return diccionarioDatos;
        }

        public void ActualizarPublicadorEditorRecursosComunidad(FacetadoAD pFacetadoAD, string pFicheroConfiguracionBD, string pUrlIntragnoss, bool pUsarColaActualizacion, Guid pProyID, Guid pIdentidadID, string pNombrePersona, List<Guid> pListaRecursos)
        {
            if (pFacetadoAD == null)
            {
                if (!pFicheroConfiguracionBD.Equals(string.Empty) && !pProyID.Equals(Guid.Empty))
                {
                    pFacetadoAD = new FacetadoAD(pFicheroConfiguracionBD, pUrlIntragnoss, pProyID.ToString(), mLoggingService, mEntityContext, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                }
                else
                {
                    pFacetadoAD = new FacetadoAD(pUrlIntragnoss, true, mLoggingService, mEntityContext, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                }
            }

            pFacetadoAD.ActualizarPublicadoresRecursos(pUrlIntragnoss, pUsarColaActualizacion, pProyID, pIdentidadID, pNombrePersona);

            //Obtener todos los recursos de la comunidad con un solo editor
            pFacetadoAD.ActualizarEditoresRecursos(pUrlIntragnoss, pUsarColaActualizacion, pProyID, pIdentidadID, pNombrePersona, pListaRecursos);
        }

        #region Propiedades

        /// <summary>
        /// Obtiene, si está definida, la afinidad de virtuoso del objeto de petición
        /// </summary>
        public string ConexionAfinidadVirtuoso
        {
            get
            {
                return mServicesUtilVirtuosoAndReplication.ConexionAfinidadVirtuoso;
            }
        }

        /// <summary>
        /// Obtiene el tag con la afinidad de virtuoso para la fila de ColaTagsComunidades
        /// </summary>
        public string TagBaseAfinidadVirtuoso
        {
            get
            {
                string tag = "";
                string afinidad = ConexionAfinidadVirtuoso;
                if (!string.IsNullOrEmpty(afinidad))
                {
                    tag = Constantes.AFINIDAD_VIRTUOSO + afinidad + Constantes.AFINIDAD_VIRTUOSO;
                }
                return tag;
            }
        }

        #endregion
    }
}
