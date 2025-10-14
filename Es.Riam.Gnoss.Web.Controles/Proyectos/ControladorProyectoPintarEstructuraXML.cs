using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models;
using Es.Riam.Gnoss.AD.EntityModel.Models.Faceta;
using Es.Riam.Gnoss.AD.EntityModel.Models.ParametroGeneralDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Facetado;
using Es.Riam.Gnoss.AD.Parametro;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.ParametrosAplicacion;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Elementos.ParametroGeneralDSEspacio;
using Es.Riam.Gnoss.Elementos.ParametroGeneralDSName;
using Es.Riam.Gnoss.Logica.CMS;
using Es.Riam.Gnoss.Logica.ComparticionAutomatica;
using Es.Riam.Gnoss.Logica.Documentacion;
using Es.Riam.Gnoss.Logica.ExportacionBusqueda;
using Es.Riam.Gnoss.Logica.Facetado;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.ParametroAplicacion;
using Es.Riam.Gnoss.Logica.ParametrosProyecto;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Tesauro;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.UtilServiciosWeb;
using Es.Riam.Gnoss.Web.Controles.ParametroGeneralDSName;
using Es.Riam.Gnoss.Web.Controles.ServiciosGenerales;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml;

namespace Es.Riam.Gnoss.Web.Controles.Proyectos
{
    public class ControladorProyectoPintarEstructuraXML
    {
        #region Miembros

        private Guid mOrganizacionID;
        private Guid mProyectoID;
        private Dictionary<string, string> mParametroProyecto;
        private DataWrapperProyecto mDataWrapperProyecto;
        private DataWrapperFacetas mFacetaDW;
        private Proyecto mFilaProyecto;
        private GestorParametroGeneral mParametroGeneralDS;
        private ConfiguracionAmbitoBusquedaProyecto mFilaConfiguracionAmbitoBusqueda;
        private ParametroGeneral mFilaParametroGeneral;
        private DataWrapperExportacionBusqueda mExportacionBusquedaDW;
        private DataWrapperComparticionAutomatica mComparticionAutomaticaDW;
        private DataWrapperVistaVirtual mVistaVirtualDW;

        public bool CrearFilasPropiedadesExportacion = false;
        private List<IntegracionContinuaPropiedad> propiedadesIntegracionContinua = new List<IntegracionContinuaPropiedad>();

        private LoggingService mLoggingService;
        private VirtuosoAD mVirtuosoAD;
        private EntityContext mEntityContext;
        private ConfigService mConfigService;
        private IServicesUtilVirtuosoAndReplication mServicesUtilVirtuosoAndReplication;
        private RedisCacheWrapper mRedisCacheWrapper;
        private GnossCache mGnossCache;
        private IHttpContextAccessor mHttpContextAccessor;
        private EntityContextBASE mEntityContextBASE;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        #endregion Miembros

        #region Metodos

        public ControladorProyectoPintarEstructuraXML(Guid pOrganizacionID, Guid pProyectoID, LoggingService loggingService, EntityContext entityContext, ConfigService configService, RedisCacheWrapper redisCacheWrapper, VirtuosoAD virtuosoAD, GnossCache gnossCache, IHttpContextAccessor httpContextAccessor, EntityContextBASE entityContextBASE, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<ControladorProyectoPintarEstructuraXML> logger, ILoggerFactory loggerFactory)
        {
            mVirtuosoAD = virtuosoAD;
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;
            mGnossCache = gnossCache;
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
            mRedisCacheWrapper = redisCacheWrapper;
            mHttpContextAccessor = httpContextAccessor;
            mEntityContextBASE = entityContextBASE;

            this.mOrganizacionID = pOrganizacionID;
            this.mProyectoID = pProyectoID;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        /// <summary>
        /// Genera un XML a partir de los datos configurados en una comunidad
        /// </summary>
        public XmlDocument PintarEstructuraXML()
        {
            XmlDocument xmlDoc = new XmlDocument();
            // Write down the XML declaration
            XmlDeclaration xmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", null);
            xmlDoc.InsertBefore(xmlDeclaration, xmlDoc.DocumentElement);

            // Creamos el nodo raíz Comunidad
            XmlElement NodoComunidad = xmlDoc.CreateElement("Comunidad");
            xmlDoc.AppendChild(NodoComunidad);

            if (FilaProyecto != null)
            {

                // Creamos el nodo NombreComunidad
                AgregarNodoAlPadre(xmlDoc, NodoComunidad, "NombreComunidad", FilaProyecto.NombreCorto.ToLower());

                // Creamos el nodo NumeroCaracteresDescripcionSuscripcion
                if (ParametroProyecto.ContainsKey(ParametroAD.NumeroCaracteresDescripcion))
                {
                    AgregarNodoAlPadre(xmlDoc, NodoComunidad, "NumeroCaracteresDescripcionSuscripcion", ParametroProyecto[ParametroAD.NumeroCaracteresDescripcion]);
                }

                PintarEstructuraXMLConfiguracionUtilidades(xmlDoc, NodoComunidad, ProyectoID, FilaParametroGeneral);

                PintarEstructuraXMLGruposPermitidosSeleccionarVisibilidadRecurso(xmlDoc, NodoComunidad, ProyectoID);

                PintarEstructuraXMLComparticionAutomatica(xmlDoc, NodoComunidad, ComparticionAutomaticaDW, ProyectoID);

                PintarEstructuraXMLPestanyas(xmlDoc, NodoComunidad, DataWrapperProyecto, FilaParametroGeneral, FilaConfiguracionAmbitoBusqueda, ExportacionBusquedaDW, ProyectoID);

                PintarEstructuraXMLObjetoConocimiento(xmlDoc, NodoComunidad, DataWrapperProyecto, FacetaDW, ProyectoID, OrganizacionID);

                PintarEstructuraXMLFacetas(xmlDoc, NodoComunidad, FacetaDW, ProyectoID, OrganizacionID);

                PintarEstructuraXMLContextos(xmlDoc, NodoComunidad);

                PintarEstructuraXMLConfigFacetadoProy(xmlDoc, NodoComunidad);

                PintarEstructuraXMLSeccionProyCatalogo(xmlDoc, NodoComunidad);

                PintarEstructuraXMLConfigBBDDAutocompletarProyecto(xmlDoc, NodoComunidad);

                PintarEstructuraXMLAgruparRegistrosUsuariosEnProyecto(xmlDoc, NodoComunidad);

                PintarEstructuraXMLPintarEnlacesLODEtiquetasEnProyecto(xmlDoc, NodoComunidad);

                PintarEstructuraXMLTesaurosSemanticos(xmlDoc, NodoComunidad, DataWrapperProyecto, ProyectoID);

                PintarEstructuraXMLPintarUsarOntologiasOtroProyecto(xmlDoc, NodoComunidad);

                PintarEstructuraXMLPermitirDescargaIdentidadInvitada(xmlDoc, NodoComunidad);

                PintarEstructuraXMLCargarEditoresLectoresEnBusqueda(xmlDoc, NodoComunidad);
            }

            //if (CrearFilasPropiedadesExportacion)
            //{
            //    ProyectoCN proyCN = new ProyectoCN();
            //    proyCN.CrearFilasIntegracionContinuaParametro(propiedadesIntegracionContinua, ProyectoID, true);
            //    proyCN.Dispose();
            //}

            return xmlDoc;
        }

        private void PintarEstructuraXMLConfiguracionUtilidades(XmlDocument pXmlDoc, XmlElement pNodoComunidad, Guid pProyectoID, ParametroGeneral pParametrosGeneralesRow)
        {
            // Creamos el nodo Utilidades
            XmlElement NodoUtilidades = pXmlDoc.CreateElement("Utilidades");
            pNodoComunidad.AppendChild(NodoUtilidades);

            // Creamos el nodo Votaciones
            XmlElement NodoVotaciones = pXmlDoc.CreateElement("Votaciones");
            NodoUtilidades.AppendChild(NodoVotaciones);

            // Creamos el nodo Votaciones disponibles
            AgregarNodoAlPadre(pXmlDoc, NodoVotaciones, "VotacionesDisponibles", Convert.ToInt32(pParametrosGeneralesRow.VotacionesDisponibles).ToString());

            // Creamos el nodo Votaciones Negativas disponibles
            AgregarNodoAlPadre(pXmlDoc, NodoVotaciones, "VotacionesNegativasDisponibles", Convert.ToInt32(pParametrosGeneralesRow.PermitirVotacionesNegativas).ToString());

            // Creamos el nodo InvitacionesDisponibles
            AgregarNodoAlPadre(pXmlDoc, NodoUtilidades, "InvitacionesDisponibles", Convert.ToInt32(pParametrosGeneralesRow.InvitacionesDisponibles).ToString());

            // Creamos el nodo MostrarAccionesEnListados
            AgregarNodoAlPadre(pXmlDoc, NodoUtilidades, "MostrarAccionesEnListados", Convert.ToInt32(pParametrosGeneralesRow.MostrarAccionesEnListados).ToString());

            if (VistaVirtualDW.ListaVistaVirtualProyecto.Count > 0)
            {
                AgregarNodoAlPadre(pXmlDoc, NodoUtilidades, "PersonalizacionComunidad", VistaVirtualDW.ListaVistaVirtualProyecto.FirstOrDefault().PersonalizacionID.ToString());
            }

            //Creamos el nodo de NombrePoliticaCookies
            if (ParametroProyecto.ContainsKey(ParametroAD.NombrePoliticaCookies))
            {
                AgregarNodoAlPadre(pXmlDoc, NodoUtilidades, ParametroAD.NombrePoliticaCookies, ParametroProyecto[ParametroAD.NombrePoliticaCookies]);
            }
        }

        private void PintarEstructuraXMLGruposPermitidosSeleccionarVisibilidadRecurso(XmlDocument pXmlDoc, XmlElement pNodoComunidad, Guid pProyectoID)
        {
            if (ParametroProyecto.ContainsKey("GruposPermitidosSeleccionarPrivacidadRecursoAbierto") && !string.IsNullOrEmpty(ParametroProyecto["GruposPermitidosSeleccionarPrivacidadRecursoAbierto"]))
            {
                // Creamos el nodo ConfigSearchProy
                XmlElement NodoGrupos = pXmlDoc.CreateElement("GruposPermitidosSeleccionarPrivacidadRecursoAbierto");
                pNodoComunidad.AppendChild(NodoGrupos);

                string[] grupos = ParametroProyecto["GruposPermitidosSeleccionarPrivacidadRecursoAbierto"].Split(new string[] { "|||" }, StringSplitOptions.RemoveEmptyEntries);

                List<Guid> listaGruposID = new List<Guid>();
                foreach (string grupo in grupos)
                {
                    listaGruposID.Add(new Guid(grupo));
                }

                Dictionary<Guid, string> nombresGrupos = new ControladorIdentidades(new GestionIdentidades(new DataWrapperIdentidad(), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication), mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorIdentidades>(), mLoggerFactory).ObtenerNombresGrupos(listaGruposID);

                foreach (Guid grupoID in nombresGrupos.Keys)
                {
                    // Creamos el nodo Clave
                    XmlElement NodoGrupo = pXmlDoc.CreateElement("Grupo");
                    NodoGrupo.InnerText = nombresGrupos[grupoID];
                    NodoGrupos.AppendChild(NodoGrupo);
                }
            }
        }

        private void PintarEstructuraXMLTesaurosSemanticos(XmlDocument pXmlDoc, XmlElement pNodoComunidad, DataWrapperProyecto pDataWrapperProyecto, Guid pProyectoID)
        {
            List<ProyectoConfigExtraSem> filasTesSem = pDataWrapperProyecto.ListaProyectoConfigExtraSem.Where(proy=>proy.ProyectoID.Equals(pProyectoID) && proy.Tipo.Equals((short)TipoConfigExtraSemantica.TesauroSemantico)).ToList();

            if (filasTesSem != null && filasTesSem.Count > 0)
            {
                XmlElement nodoTesauros = pXmlDoc.CreateElement("TesaurosSemanticos");
                pNodoComunidad.AppendChild(nodoTesauros);

                foreach (ProyectoConfigExtraSem fila in filasTesSem)
                {
                    XmlElement nodoTesSem = pXmlDoc.CreateElement("TesauroSemantico");
                    nodoTesauros.AppendChild(nodoTesSem);

                    XmlElement nodoNombre = pXmlDoc.CreateElement("Nombre");
                    nodoTesSem.AppendChild(nodoNombre);
                    nodoNombre.InnerText = fila.Nombre;

                    XmlElement nodoPrefijo = pXmlDoc.CreateElement("Prefijo");
                    nodoTesSem.AppendChild(nodoPrefijo);
                    nodoPrefijo.InnerText = fila.PrefijoTesSem;

                    XmlElement nodoIdiomas = pXmlDoc.CreateElement("Idiomas");
                    nodoTesSem.AppendChild(nodoIdiomas);
                    nodoIdiomas.InnerText = fila.Idiomas;

                    XmlElement nodoOntologia = pXmlDoc.CreateElement("Ontologia");
                    nodoTesSem.AppendChild(nodoOntologia);
                    nodoOntologia.InnerText = fila.UrlOntologia.Substring(0, fila.UrlOntologia.LastIndexOf("."));

                    XmlElement nodoSource = pXmlDoc.CreateElement("FuenteTesauro");
                    nodoTesSem.AppendChild(nodoSource);
                    nodoSource.InnerText = fila.SourceTesSem;

                    XmlElement nodoEditable = pXmlDoc.CreateElement("Editable");
                    nodoTesSem.AppendChild(nodoEditable);
                    if (fila.Editable)
                    {
                        nodoEditable.InnerText = "1";
                    }
                    else
                    {
                        nodoEditable.InnerText = "0";
                    }
                }
            }
        }

        /// <summary>
        /// Pinta la estructura de la parte de usar ontologías de otro proyecto.
        /// </summary>
        /// <param name="pXmlDoc">Xml del doucmento</param>
        /// <param name="pNodoComunidad">Nodo XML de la comunidad</param>
        private void PintarEstructuraXMLPintarUsarOntologiasOtroProyecto(XmlDocument pXmlDoc, XmlElement pNodoComunidad)
        {
            if (ParametroProyecto.ContainsKey(ParametroAD.ProyectoIDPatronOntologias))
            {
                XmlElement NodoUsarOntologiasDeProyecto = pXmlDoc.CreateElement("UsarOntologiasDeProyecto");
                pNodoComunidad.AppendChild(NodoUsarOntologiasDeProyecto);

                ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);
                string nombreCortoProyExt = proyCL.ObtenerNombreCortoProyecto(new Guid(ParametroProyecto[ParametroAD.ProyectoIDPatronOntologias]));
                proyCL.Dispose();

                NodoUsarOntologiasDeProyecto.InnerText = nombreCortoProyExt;
            }
        }

        private void PintarEstructuraXMLPintarEnlacesLODEtiquetasEnProyecto(XmlDocument pXmlDoc, XmlElement pNodoComunidad)
        {
            XmlElement NodoEnlacesLODEtiquetasEnProyecto = pXmlDoc.CreateElement("PintarEnlacesLODEtiquetasEnProyecto");
            pNodoComunidad.AppendChild(NodoEnlacesLODEtiquetasEnProyecto);
            //Consultar en ParametroProyecto si existe y poner la enumeración correcta
            if (ParametroProyecto.ContainsKey("PintarEnlacesLODEtiquetasEnProyecto"))
            {
                // Creamos el nodo PintarEnlacesLODEtiquetasEnProyecto
                NodoEnlacesLODEtiquetasEnProyecto.InnerText = ParametroProyecto["PintarEnlacesLODEtiquetasEnProyecto"];
            }
            else
            {
                //Pintamos el valor por defecto
                NodoEnlacesLODEtiquetasEnProyecto.InnerText = (1).ToString();
            }
        }

        private void PintarEstructuraXMLPermitirDescargaIdentidadInvitada(XmlDocument pXmlDoc, XmlElement pNodoComunidad)
        {
            XmlElement NodoPermitirDescargaIdentidadInvitada = pXmlDoc.CreateElement(ParametroAD.PermitirDescargaIdentidadInvitada);
            pNodoComunidad.AppendChild(NodoPermitirDescargaIdentidadInvitada);
            //Consultar en ParametroProyecto si existe y poner la enumeración correcta
            if (ParametroProyecto.ContainsKey(ParametroAD.PermitirDescargaIdentidadInvitada))
            {
                // Creamos el nodo PermitirDescargaIdentidadInvitada
                NodoPermitirDescargaIdentidadInvitada.InnerText = ParametroProyecto[ParametroAD.PermitirDescargaIdentidadInvitada];
            }
        }

        private void PintarEstructuraXMLCargarEditoresLectoresEnBusqueda(XmlDocument pXmlDoc, XmlElement pNodoComunidad)
        {
            XmlElement NodoCargarEditoresLectoresEnBusqueda = pXmlDoc.CreateElement(ParametroAD.CargarEditoresLectoresEnBusqueda);
            pNodoComunidad.AppendChild(NodoCargarEditoresLectoresEnBusqueda);
            //Consultar en ParametroProyecto si existe y poner la enumeración correcta
            if (ParametroProyecto.ContainsKey(ParametroAD.CargarEditoresLectoresEnBusqueda))
            {
                // Creamos el nodo PermitirDescargaIdentidadInvitada
                NodoCargarEditoresLectoresEnBusqueda.InnerText = ParametroProyecto[ParametroAD.CargarEditoresLectoresEnBusqueda];
            }
        }

        private void PintarEstructuraXMLAgruparRegistrosUsuariosEnProyecto(XmlDocument pXmlDoc, XmlElement pNodoComunidad)
        {
            XmlElement NodoAgruparRegistrosUsuariosEnProyecto = pXmlDoc.CreateElement("AgruparRegistrosUsuariosEnProyecto");
            pNodoComunidad.AppendChild(NodoAgruparRegistrosUsuariosEnProyecto);
            //Consultar en ParametroProyecto si existe y poner la enumeración correcta
            if (ParametroProyecto.ContainsKey("AgruparRegistrosUsuariosEnProyecto"))
            {
                // Creamos el nodo AgruparRegistrosUsuariosEnProyecto
                NodoAgruparRegistrosUsuariosEnProyecto.InnerText = ParametroProyecto["AgruparRegistrosUsuariosEnProyecto"];
            }
            else
            {
                //Pintamos el valor por defecto
                NodoAgruparRegistrosUsuariosEnProyecto.InnerText = ((short)TipoBusquedasAutocompletar.BBDDTags).ToString();
            }
        }

        private void PintarEstructuraXMLConfigBBDDAutocompletarProyecto(XmlDocument pXmlDoc, XmlElement pNodoComunidad)
        {
            XmlElement NodoConfigBBDDAutocompletarProyecto = pXmlDoc.CreateElement("ConfigBBDDAutocompletarProyecto");
            pNodoComunidad.AppendChild(NodoConfigBBDDAutocompletarProyecto);
            //Consultar en ParametroProyecto si existe y poner la enumeración correcta
            if (ParametroProyecto.ContainsKey("ConfigBBDDAutocompletarProyecto"))
            {
                // Creamos el nodo ConfigBBDDAutocompletarProyecto
                NodoConfigBBDDAutocompletarProyecto.InnerText = ParametroProyecto["ConfigBBDDAutocompletarProyecto"];
            }
            else
            {
                //Pintamos el valor por defecto
                NodoConfigBBDDAutocompletarProyecto.InnerText = ((short)TipoBusquedasAutocompletar.BBDDTags).ToString();
            }
        }

        private void PintarEstructuraXMLSeccionProyCatalogo(XmlDocument pXmlDoc, XmlElement pNodoComunidad)
        {
            List<SeccionProyCatalogo> filasSeccion = DataWrapperProyecto.ListaSeccionProyCatalogo.Where(proy=>proy.OrganizacionID.Equals(OrganizacionID.ToString()) && proy.ProyectoID.Equals(ProyectoID.ToString())).ToList();
            if (filasSeccion != null && filasSeccion.Count > 0)
            {
                ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);

                foreach (SeccionProyCatalogo fila in filasSeccion)
                {
                    string proyectoBusqueda = proyectoCN.ObtenerNombreCortoProyecto(fila.ProyectoID);


                    // Creamos el nodo SeccionProyCatalogo
                    XmlElement NodoSeccionProyCatalogo = pXmlDoc.CreateElement("SeccionProyCatalogo");
                    pNodoComunidad.AppendChild(NodoSeccionProyCatalogo);

                    // Creamos el nodo ProyectoBusqueda
                    XmlElement NodoProyectoBusqueda = pXmlDoc.CreateElement("ProyectoBusqueda");
                    NodoSeccionProyCatalogo.AppendChild(NodoProyectoBusqueda);
                    NodoProyectoBusqueda.InnerText = proyectoBusqueda;

                    // Creamos el nodo Orden
                    XmlElement NodoOrden = pXmlDoc.CreateElement("Orden");
                    NodoSeccionProyCatalogo.AppendChild(NodoOrden);
                    NodoOrden.InnerText = fila.Orden.ToString();

                    // Creamos el nodo Nombre
                    XmlElement NodoNombre = pXmlDoc.CreateElement("Nombre");
                    NodoSeccionProyCatalogo.AppendChild(NodoNombre);
                    NodoNombre.InnerText = fila.Nombre;

                    // Creamos el nodo Faceta
                    XmlElement NodoFaceta = pXmlDoc.CreateElement("Faceta");
                    NodoSeccionProyCatalogo.AppendChild(NodoFaceta);
                    NodoFaceta.InnerText = fila.Faceta;

                    // Creamos el nodo Filtro
                    XmlElement NodoFiltro = pXmlDoc.CreateElement("Filtro");
                    NodoSeccionProyCatalogo.AppendChild(NodoFiltro);
                    NodoFiltro.InnerText = fila.Filtro;

                    // Creamos el nodo NumeroResultados
                    XmlElement NodoNumeroResultados = pXmlDoc.CreateElement("NumeroResultados");
                    NodoSeccionProyCatalogo.AppendChild(NodoNumeroResultados);
                    NodoNumeroResultados.InnerText = fila.NumeroResultados.ToString();

                    // Creamos el nodo Tipo
                    XmlElement NodoTipo = pXmlDoc.CreateElement("Tipo");
                    NodoSeccionProyCatalogo.AppendChild(NodoTipo);
                    NodoTipo.InnerText = fila.Tipo.ToString();
                }

                proyectoCN.Dispose();
            }
        }

        private void PintarEstructuraXMLConfigFacetadoProy(XmlDocument pXmlDoc, XmlElement pNodoComunidad)
        {
            #region ConfigFacetadoProyMap

            FacetaConfigProyMapa filaConfProyMapa = FacetaDW.ListaFacetaConfigProyMapa.Where(item => item.OrganizacionID.Equals(OrganizacionID) && item.ProyectoID.Equals(ProyectoID)).FirstOrDefault();
            if (filaConfProyMapa != null)
            {
                // Creamos el nodo ConfigFacetadoProyMapa
                XmlElement NodoConfigFacetadoProyMapa = pXmlDoc.CreateElement("ConfigFacetadoProyMapa");
                pNodoComunidad.AppendChild(NodoConfigFacetadoProyMapa);

                KeyValuePair<string, string> propLatitud = new KeyValuePair<string, string>("PropLatitud", filaConfProyMapa.PropLatitud);
                KeyValuePair<string, string> propLongitud = new KeyValuePair<string, string>("PropLongitud", filaConfProyMapa.PropLongitud);
                KeyValuePair<string, string> propRuta = new KeyValuePair<string, string>("PropRuta", filaConfProyMapa.PropRuta);
                KeyValuePair<string, string> colorRuta = new KeyValuePair<string, string>("ColorRuta", filaConfProyMapa.ColorRuta);

                AgregarNodosConfigFacetadoProyMapa(pXmlDoc, NodoConfigFacetadoProyMapa, propLatitud, propLongitud, propRuta, colorRuta);
            }

            #endregion ConfigFacetadoProyMap

            #region ConfigFacetadoProyChart

            List<FacetaConfigProyChart> filasConfProyChart = FacetaDW.ListaFacetaConfigProyChart.Where(item => item.OrganizacionID.Equals(OrganizacionID) && item.ProyectoID.Equals(ProyectoID)).ToList();

            if (filasConfProyChart != null && filasConfProyChart.Count > 0)
            {
                foreach (FacetaConfigProyChart fila in filasConfProyChart)
                {
                    // Creamos el nodo ConfigFacetadoProyChart
                    XmlElement NodoConfigFacetadoProyChart = pXmlDoc.CreateElement("ConfigFacetadoProyChart");
                    pNodoComunidad.AppendChild(NodoConfigFacetadoProyChart);

                    // Creamos el nodo Nombre
                    AgregarNodoAlPadre(pXmlDoc, NodoConfigFacetadoProyChart, "Nombre", fila.Nombre);

                    // Creamos el nodo Orden
                    AgregarNodoAlPadre(pXmlDoc, NodoConfigFacetadoProyChart, "Orden", fila.Orden.ToString());

                    // Creamos el nodo SelectConsultaVirtuoso
                    AgregarNodoAlPadre(pXmlDoc, NodoConfigFacetadoProyChart, "SelectConsultaVirtuoso", fila.SelectConsultaVirtuoso, true);

                    string[] filtrosChart = fila.FiltrosConsultaVirtuoso.Split(new string[] { "|||" }, StringSplitOptions.None);

                    // Creamos el nodo FiltrosConsultaVirtuoso
                    XmlElement NodoFiltrosConsultaVirtuoso = pXmlDoc.CreateElement("FiltrosConsultaVirtuoso");
                    NodoConfigFacetadoProyChart.AppendChild(NodoFiltrosConsultaVirtuoso);
                    NodoFiltrosConsultaVirtuoso.InnerXml = "<![CDATA[" + filtrosChart[0] + "]]>";

                    string groupBy = "";

                    if (filtrosChart.Length > 1)
                    {
                        groupBy = filtrosChart[1];
                    }

                    NodoFiltrosConsultaVirtuoso = pXmlDoc.CreateElement("FiltrosGroupByVirtuoso");
                    NodoConfigFacetadoProyChart.AppendChild(NodoFiltrosConsultaVirtuoso);
                    NodoFiltrosConsultaVirtuoso.InnerXml = "<![CDATA[" + groupBy + "]]>";

                    string orderBy = "";

                    if (filtrosChart.Length > 2)
                    {
                        orderBy = filtrosChart[2];
                    }

                    NodoFiltrosConsultaVirtuoso = pXmlDoc.CreateElement("FiltrosOrderByVirtuoso");
                    NodoConfigFacetadoProyChart.AppendChild(NodoFiltrosConsultaVirtuoso);
                    NodoFiltrosConsultaVirtuoso.InnerXml = "<![CDATA[" + orderBy + "]]>";

                    string limit = "";

                    if (filtrosChart.Length > 3)
                    {
                        limit = filtrosChart[3];
                    }

                    NodoFiltrosConsultaVirtuoso = pXmlDoc.CreateElement("FiltrosLimitVirtuoso");
                    NodoConfigFacetadoProyChart.AppendChild(NodoFiltrosConsultaVirtuoso);
                    NodoFiltrosConsultaVirtuoso.InnerXml = "<![CDATA[" + limit + "]]>";

                    // Creamos el nodo JSBase
                    AgregarNodoAlPadre(pXmlDoc, NodoConfigFacetadoProyChart, "JSBase", fila.JSBase, true);

                    // Creamos el nodo JSBusqueda
                    AgregarNodoAlPadre(pXmlDoc, NodoConfigFacetadoProyChart, "JSBusqueda", fila.JSBusqueda, true);

                    if (fila.Ontologias != null && !string.IsNullOrEmpty(fila.Ontologias))
                    {
                        foreach (string ontologia in fila.Ontologias.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            // Creamos el nodo Ontologia
                            AgregarNodoAlPadre(pXmlDoc, NodoConfigFacetadoProyChart, "Ontologia", ontologia, true);
                        }
                    }
                }
            }
            #endregion ConfigFacetadoProyChart

            #region ConfigFacetadoProyRangoFecha

            List<FacetaConfigProyRangoFecha> filasConfProyRangoFecha = FacetaDW.ListaFacetaConfigProyRangoFecha.Where(item => item.OrganizacionID.Equals(OrganizacionID) && item.ProyectoID.Equals(ProyectoID)).ToList();
            if (filasConfProyRangoFecha != null && filasConfProyRangoFecha.Count > 0)
            {
                foreach (FacetaConfigProyRangoFecha fila in filasConfProyRangoFecha)
                {
                    // Creamos el nodo ConfigFacetadoProyRangoFecha
                    XmlElement NodoConfigFacetadoProyRangoFecha = pXmlDoc.CreateElement("ConfigFacetadoProyRangoFecha");
                    pNodoComunidad.AppendChild(NodoConfigFacetadoProyRangoFecha);

                    KeyValuePair<string, string> propiedadNueva = new KeyValuePair<string, string>("PropiedadNueva", filaConfProyMapa.PropLatitud);
                    KeyValuePair<string, string> propiedadInicio = new KeyValuePair<string, string>("PropiedadInicio", filaConfProyMapa.PropLongitud);
                    KeyValuePair<string, string> propiedadFin = new KeyValuePair<string, string>("PropiedaFin", filaConfProyMapa.PropRuta);

                    AgregarNodosConfigFacetadoProyMapa(pXmlDoc, NodoConfigFacetadoProyRangoFecha, propiedadNueva, propiedadInicio, propiedadFin);
                }
            }
            #endregion ConfigFacetadoProyMapa
        }

        #region PintarEstructuraXMLContextos

        private void PintarEstructuraXMLContextos(XmlDocument pXmlDoc, XmlElement pNodoComunidad)
        {
            //("OrganizacionID='" + OrganizacionID.ToString() + "' and ProyectoID='" + ProyectoID + "' and TipoUbicacion=1", "Orden ASC");
            List<ProyectoGadget> filasProyectoGadget = DataWrapperProyecto.ListaProyectoGadget.Where(proy=>proy.OrganizacionID.Equals(OrganizacionID.ToString()) && proy.ProyectoID.Equals(ProyectoID) && proy.TipoUbicacion==1).OrderBy(proy=>proy.Orden).ToList(); // TipoUbicacion=1 son gadgets de recursos
            if (filasProyectoGadget != null && filasProyectoGadget.Count > 0)
            {
                // Creamos el nodo Gadgets
                XmlElement NodoGadgets = pXmlDoc.CreateElement("Gadgets");
                pNodoComunidad.AppendChild(NodoGadgets);

                foreach (ProyectoGadget filaGadget in filasProyectoGadget)
                {
                    //Creamos el nodo Gadget
                    XmlElement NodoGadget = pXmlDoc.CreateElement("Gadget");
                    NodoGadgets.AppendChild(NodoGadget);

                    //Creamos el nodo Titulo
                    AgregarNodoAlPadre(pXmlDoc, NodoGadget, "Titulo", filaGadget.Titulo);

                    //Creamos el nodo Tipo
                    string tipoGadget = Enum.GetName(typeof(TipoGadget), filaGadget.Tipo);
                    AgregarNodoAlPadre(pXmlDoc, NodoGadget, "Tipo", tipoGadget);

                    //Creamos el nodo NombreCorto
                    string nombreCorto = filaGadget.NombreCorto;
                    if (string.IsNullOrEmpty(nombreCorto))
                    {
                        nombreCorto = filaGadget.GadgetID.ToString();
                    }
                    AgregarNodoAlPadre(pXmlDoc, NodoGadget, "NombreCorto", nombreCorto);

                    //Creamos el nodo Clases
                    AgregarNodoAlPadre(pXmlDoc, NodoGadget, "Clases", filaGadget.Clases, false, false);

                    //Creamos el nodo PersonalizacionComponenteID
                    string personalizacionComponenteID = string.Empty;
                    if (filaGadget.PersonalizacionComponenteID.HasValue)
                    {
                        personalizacionComponenteID = filaGadget.PersonalizacionComponenteID.ToString();
                    }
                    AgregarNodoAlPadre(pXmlDoc, NodoGadget, "PersonalizacionComponenteID", personalizacionComponenteID, false, false);

                    ////Creamos el nodo Orden
                    //AgregarNodoAlPadre(pXmlDoc, NodoGadget, "Orden", filaGadget.Orden.ToString());

                    //Creamos el nodo Visible
                    AgregarNodoAlPadre(pXmlDoc, NodoGadget, "Visible", Convert.ToInt32(filaGadget.Visible).ToString());

                    //Creamos el nodo ComunidadDestinoFiltros
                    AgregarNodoAlPadre(pXmlDoc, NodoGadget, "ComunidadDestinoFiltros", filaGadget.ComunidadDestinoFiltros, true, false);

                    if (CrearFilasPropiedadesExportacion && !string.IsNullOrEmpty(filaGadget.ComunidadDestinoFiltros))
                    {
                        //Crear las filas de las porpiedades de Integracion Continua
                        IntegracionContinuaPropiedad propiedadFiltrosDestino = new IntegracionContinuaPropiedad();
                        propiedadFiltrosDestino.ProyectoID = ProyectoID;
                        propiedadFiltrosDestino.TipoObjeto = (short)TipoObjeto.Gadget;
                        propiedadFiltrosDestino.ObjetoPropiedad = filaGadget.NombreCorto;
                        propiedadFiltrosDestino.TipoPropiedad = (short)TipoPropiedad.FiltrosDestinoGadget;
                        propiedadFiltrosDestino.ValorPropiedad = filaGadget.ComunidadDestinoFiltros;
                        propiedadesIntegracionContinua.Add(propiedadFiltrosDestino);
                    }

                    if (filaGadget.Tipo.Equals((short)TipoGadget.CMS))
                    {
                        //Creamos el nodo CargarPorAjax
                        AgregarNodoAlPadre(pXmlDoc, NodoGadget, "CargarPorAjax", Convert.ToInt32(filaGadget.CargarPorAjax).ToString());
                        Guid idComp = Guid.Empty;
                        if (Guid.TryParse(filaGadget.Contenido, out idComp))
                        {
                            CMSCN cmsCN = new CMSCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<CMSCN>(), mLoggerFactory);
                            string nombreCortoComponente = cmsCN.ObtenerNombreCortoComponentePorIDComponenteEnProyecto(idComp, ProyectoID);

                            AgregarNodoAlPadre(pXmlDoc, NodoGadget, "IDComponenteCMS", filaGadget.Contenido, false, true);
                            AgregarNodoAlPadre(pXmlDoc, NodoGadget, "NombreCortoComponenteCMS", nombreCortoComponente, false, true);
                        }
                    }
                    else if (filaGadget.Tipo.Equals((short)TipoGadget.Consulta))
                    {
                        AgregarNodoAlPadre(pXmlDoc, NodoGadget, "UrlBusqueda", filaGadget.Contenido, false, true);
                    }
                    else if (filaGadget.Tipo.Equals((short)TipoGadget.HtmlIncrustado))
                    {
                        //Creamos el nodo MultiIdioma
                        AgregarNodoAlPadre(pXmlDoc, NodoGadget, "MultiIdioma", Convert.ToInt32(filaGadget.MultiIdioma).ToString());

                        string nombreNodoContenido = "HTML";
                        bool usarCDATA = true;

                        if (!filaGadget.MultiIdioma)
                        {
                            //Creamos el nodo Contenido/HTML/UrlBusqueda/IDComponenteCMS en función del tipo
                            AgregarNodoAlPadre(pXmlDoc, NodoGadget, nombreNodoContenido, filaGadget.Contenido, usarCDATA, true);
                        }
                        else
                        {
                            List<ProyectoGadgetIdioma> filasGadgetIdioma = filaGadget.ProyectoGadgetIdioma.ToList();
                            if (filasGadgetIdioma.Count > 0)
                            {
                                //Creamos el nodo ContenidoMultiIdioma
                                XmlElement NodoContenidoMultiIdioma = pXmlDoc.CreateElement("ContenidoMultiIdioma");
                                NodoGadget.AppendChild(NodoContenidoMultiIdioma);

                                foreach (ProyectoGadgetIdioma filaGadgetIdioma in filasGadgetIdioma)
                                {
                                    //Creamos el nodo del idioma correspondiente
                                    AgregarNodoAlPadre(pXmlDoc, NodoContenidoMultiIdioma, filaGadgetIdioma.Idioma, filaGadgetIdioma.Contenido, usarCDATA, false);
                                }
                            }
                        }
                    }
                    else if (filaGadget.Tipo.Equals((short)TipoGadget.RecursosContextos))
                    {
                        ProyectoGadgetContexto filasGadgetContexto = filaGadget.ProyectoGadgetContexto;
                        if (filasGadgetContexto != null)
                        {
                            ProyectoGadgetContexto filaGadgetContexto = filaGadget.ProyectoGadgetContexto;

                            //Creamos el nodo Contexto
                            XmlElement NodoContexto = pXmlDoc.CreateElement("Contexto");
                            NodoGadget.AppendChild(NodoContexto);

                            //Creamos el nodo ComunidadOrigen
                            AgregarNodoAlPadre(pXmlDoc, NodoContexto, "ComunidadOrigen", filaGadgetContexto.ComunidadOrigen);

                            //Creamos el nodo ComunidadOrigenFiltros
                            AgregarNodoAlPadre(pXmlDoc, NodoContexto, "ComunidadOrigenFiltros", filaGadgetContexto.ComunidadOrigenFiltros, true);

                            //Creamos el nodo FiltrosOrigenDestino
                            AgregarNodoAlPadre(pXmlDoc, NodoContexto, "FiltrosOrigenDestino", filaGadgetContexto.FiltrosOrigenDestino, true);

                            //Creamos el nodo OrdenContexto
                            AgregarNodoAlPadre(pXmlDoc, NodoContexto, "OrdenContexto", filaGadgetContexto.OrdenContexto);

                            //Creamos el nodo Imagen
                            string imagenGadgetContexto = ObtenerTipoImagen(filaGadgetContexto.Imagen);
                            AgregarNodoAlPadre(pXmlDoc, NodoContexto, "Imagen", imagenGadgetContexto);

                            //Creamos el nodo NumRecursos
                            AgregarNodoAlPadre(pXmlDoc, NodoContexto, "NumRecursos", filaGadgetContexto.NumRecursos.ToString());

                            //Creamos el nodo ServicioResultados
                            AgregarNodoAlPadre(pXmlDoc, NodoContexto, "ServicioResultados", filaGadgetContexto.ServicioResultados);

                            //Creamos el nodo MostrarEnlaceOriginal
                            AgregarNodoAlPadre(pXmlDoc, NodoContexto, "MostrarEnlaceOriginal", Convert.ToInt32(filaGadgetContexto.MostrarEnlaceOriginal).ToString());

                            //Creamos el nodo OcultarVerMas
                            AgregarNodoAlPadre(pXmlDoc, NodoContexto, "OcultarVerMas", Convert.ToInt32(filaGadgetContexto.OcultarVerMas).ToString());

                            //Creamos el nodo NamespacesExtra
                            AgregarNodoAlPadre(pXmlDoc, NodoContexto, "NamespacesExtra", filaGadgetContexto.NamespacesExtra);

                            //Creamos el nodo ItemsBusqueda
                            AgregarNodoAlPadre(pXmlDoc, NodoContexto, "ItemsBusqueda", filaGadgetContexto.ItemsBusqueda);

                            //Creamos el nodo ResultadosEliminar
                            AgregarNodoAlPadre(pXmlDoc, NodoContexto, "ResultadosEliminar", filaGadgetContexto.ResultadosEliminar);

                            //Creamos el nodo NuevaPestanya
                            AgregarNodoAlPadre(pXmlDoc, NodoContexto, "NuevaPestanya", Convert.ToInt32(filaGadgetContexto.NuevaPestanya).ToString());

                            //Creamos el nodo ObtenerPrivados
                            AgregarNodoAlPadre(pXmlDoc, NodoContexto, "ObtenerPrivados", Convert.ToInt32(filaGadgetContexto.ObtenerPrivados).ToString());

                            if (CrearFilasPropiedadesExportacion)
                            {
                                if (!string.IsNullOrEmpty(filaGadgetContexto.ComunidadOrigen))
                                {
                                    //Crear las filas de las porpiedades de Integracion Continua
                                    IntegracionContinuaPropiedad propiedadComunidadOrigen = new IntegracionContinuaPropiedad();
                                    propiedadComunidadOrigen.ProyectoID = ProyectoID;
                                    propiedadComunidadOrigen.TipoObjeto = (short)TipoObjeto.Gadget;
                                    propiedadComunidadOrigen.ObjetoPropiedad = filaGadget.NombreCorto;
                                    propiedadComunidadOrigen.TipoPropiedad = (short)TipoPropiedad.ComunidadOrigenGadget;
                                    propiedadComunidadOrigen.ValorPropiedad = filaGadgetContexto.ComunidadOrigen;
                                    propiedadesIntegracionContinua.Add(propiedadComunidadOrigen);
                                }

                                if (!string.IsNullOrEmpty(filaGadgetContexto.ComunidadOrigenFiltros))
                                {
                                    IntegracionContinuaPropiedad propiedadFiltrosOrigen = new IntegracionContinuaPropiedad();
                                    propiedadFiltrosOrigen.ProyectoID = ProyectoID;
                                    propiedadFiltrosOrigen.TipoObjeto = (short)TipoObjeto.Gadget;
                                    propiedadFiltrosOrigen.ObjetoPropiedad = filaGadget.NombreCorto;
                                    propiedadFiltrosOrigen.TipoPropiedad = (short)TipoPropiedad.FiltrosOrigenGadget;
                                    propiedadFiltrosOrigen.ValorPropiedad = filaGadgetContexto.ComunidadOrigenFiltros;
                                    propiedadesIntegracionContinua.Add(propiedadFiltrosOrigen);
                                }

                                if (!string.IsNullOrEmpty(filaGadgetContexto.FiltrosOrigenDestino))
                                {
                                    IntegracionContinuaPropiedad propiedadRelacionOrigenDestino = new IntegracionContinuaPropiedad();
                                    propiedadRelacionOrigenDestino.ProyectoID = ProyectoID;
                                    propiedadRelacionOrigenDestino.TipoObjeto = (short)TipoObjeto.Gadget;
                                    propiedadRelacionOrigenDestino.ObjetoPropiedad = filaGadget.NombreCorto;
                                    propiedadRelacionOrigenDestino.TipoPropiedad = (short)TipoPropiedad.RelacionOrigenDestinoGadget;
                                    propiedadRelacionOrigenDestino.ValorPropiedad = filaGadgetContexto.FiltrosOrigenDestino;
                                    propiedadesIntegracionContinua.Add(propiedadRelacionOrigenDestino);
                                }
                            }
                        }
                    }
                }
            }
        }

        private string ObtenerTipoImagen(short pImagen)
        {
            string imagen = string.Empty;
            switch (pImagen)
            {
                case 0:
                    imagen = "sin imagen";
                    break;
                case 1:
                    imagen = "imagen normal";
                    break;
                case 2:
                    imagen = "imagen reducida";
                    break;
            }

            return imagen;
        }

        #endregion PintarEstructuraXMLContextos

        #region PintarEstructuraXMLFacetas

        private void PintarEstructuraXMLFacetas(XmlDocument pXmlDoc, XmlElement pNodoComunidad, DataWrapperFacetas pFacetaDW, Guid pProyectoID, Guid pOrganizacionID)
        {
            if ((pFacetaDW.ListaFacetaObjetoConocimientoProyecto != null && pFacetaDW.ListaFacetaObjetoConocimientoProyecto.Count > 0) ||
                    (pFacetaDW.ListaFacetaExcluida != null && pFacetaDW.ListaFacetaExcluida.Count > 0) ||
                    (pFacetaDW.ListaFacetaEntidadesExternas != null && pFacetaDW.ListaFacetaEntidadesExternas.Count > 0))
            {
                // Creamos el nodo Facetas
                XmlElement NodoFacetas = pXmlDoc.CreateElement("Facetas");
                pNodoComunidad.AppendChild(NodoFacetas);

                PintarEstructuraXMLFacetasExcluidas(pXmlDoc, NodoFacetas, pFacetaDW);

                PintarEstructuraXMLFacetasEntidadesExternas(pXmlDoc, NodoFacetas, pFacetaDW);

                PintarEstructuraXMLFacetasObjetoConocimientoProyecto(pXmlDoc, NodoFacetas, pFacetaDW, pProyectoID, pOrganizacionID);
            }
        }

        private void PintarEstructuraXMLFacetasExcluidas(XmlDocument pXmlDoc, XmlElement pNodoFacetas, DataWrapperFacetas pFacetaDW)
        {
            if (pFacetaDW.ListaFacetaExcluida != null && pFacetaDW.ListaFacetaExcluida.Count > 0)
            {
                // Creamos el nodo FacetasExcluidas
                XmlElement NodoFacetasExcluidas = pXmlDoc.CreateElement("FacetasExcluidas");
                pNodoFacetas.AppendChild(NodoFacetasExcluidas);

                foreach (FacetaExcluida fe in pFacetaDW.ListaFacetaExcluida)
                {
                    // Creamos el nodo FacetaExcluida
                    AgregarNodoAlPadre(pXmlDoc, NodoFacetasExcluidas, "FacetaExcluida", fe.Faceta);
                }
            }
        }

        private void PintarEstructuraXMLFacetasEntidadesExternas(XmlDocument pXmlDoc, XmlElement pNodoFacetas, DataWrapperFacetas pFacetaDW)
        {
            if (pFacetaDW.ListaFacetaEntidadesExternas != null && pFacetaDW.ListaFacetaEntidadesExternas.Count > 0)
            {
                foreach (FacetaEntidadesExternas fee in pFacetaDW.ListaFacetaEntidadesExternas)
                {
                    // Creamos el nodo FacetaEntidadesExternas
                    XmlElement NodoFacetaEntidadesExternas = pXmlDoc.CreateElement("FacetaEntidadesExternas");
                    pNodoFacetas.AppendChild(NodoFacetaEntidadesExternas);

                    // Creamos el nodo Entidad
                    AgregarNodoAlPadre(pXmlDoc, NodoFacetaEntidadesExternas, "Entidad", fee.EntidadID);

                    // Creamos el nodo Grafo
                    AgregarNodoAlPadre(pXmlDoc, NodoFacetaEntidadesExternas, "Grafo", fee.Grafo);

                    // Creamos el nodo EsEntidadSecundaria
                    AgregarNodoAlPadre(pXmlDoc, NodoFacetaEntidadesExternas, "EsEntidadSecundaria", Convert.ToInt32(fee.EsEntidadSecundaria).ToString());

                    // Creamos el nodo BuscarConRecursividad
                    AgregarNodoAlPadre(pXmlDoc, NodoFacetaEntidadesExternas, "BuscarConRecursividad", Convert.ToInt32(fee.BuscarConRecursividad).ToString());
                }
            }
        }

        private void PintarEstructuraXMLFacetasObjetoConocimientoProyecto(XmlDocument pXmlDoc, XmlElement pNodoFacetas, DataWrapperFacetas pFacetaDW, Guid pProyectoID, Guid pOrganizacionID)
        {
            //TODO: Migrar a EF
            //List<FacetaObjetoConocimientoProyecto> filasFacOcPr = (List<FacetaObjetoConocimientoProyecto>)pFacetaDW.ListaFacetaObjetoConocimientoProyecto.Where(item => item.OrganizacionID.Equals(pOrganizacionID) && item.ProyectoID.Equals(pProyectoID));

            ////si existen filas FacetaObjetoConocimientoProyecto busca si tiene filtros
            //if (filasFacOcPr.Count() > 0)
            //{
            //    foreach (FacetaObjetoConocimientoProyecto fila in filasFacOcPr)
            //    {
            //        //busca la facetaHome
            //        List<FacetaHomeRow> filasFacetaHome = (List<FacetaHomeRow>)fila.Faceta.GetFacetaHomeRows();
            //        string pestanyaFaceta = string.Empty;
            //        string ordenHome = string.Empty;
            //        bool verMas = false;

            //        Dictionary<string, string> listaFiltrosHome = ObtenerFiltrosHomeProyecto(filasFacetaHome, out pestanyaFaceta, out ordenHome, out verMas);

            //        // Obtener los Filtros del proyecto.
            //        List<FacetaHomeRow> filasFiltroProyecto = (List<FacetaHomeRow>)fila.FacetaFiltroProyecto.ToList();
            //        Dictionary<string, string> listaFiltrosProyecto = ObtenerFiltrosProyecto(filasFiltroProyecto);
                    
            //        // Obtener las PestanyasID
            //        List<FacetaObjetoConocimientoProyectoPestanyaRow> filasFacetaObjetoConocimientoProyectoPestanya = (List<FacetaObjetoConocimientoProyectoPestanyaRow>)fila.GetFacetaObjetoConocimientoProyectoPestanyaRows();
            //        List<Guid> listaPestanyasFacetaProyecto = ObtenerPestanyasFacetaProyecto(filasFacetaObjetoConocimientoProyectoPestanya);

            //        pNodoFacetas.AppendChild(PintarFacetaObjetoConocimientoProyectoXML(pXmlDoc, pNodoFacetas, fila, listaFiltrosProyecto, listaPestanyasFacetaProyecto, pestanyaFaceta, ordenHome, verMas, listaFiltrosHome));
            //    }
            //}
        }

        private List<Guid> ObtenerPestanyasFacetaProyecto(List<FacetaObjetoConocimientoProyectoPestanya> pFilasFacetaObjetoConocimientoProyectoPestanya)
        {
            List<Guid> listaPestanyasFacetaProyecto = new List<Guid>();

            //si hay filtros, pinta la faceta con los filtros definidos
            if (pFilasFacetaObjetoConocimientoProyectoPestanya != null && pFilasFacetaObjetoConocimientoProyectoPestanya.Count > 0)
            {
                foreach (FacetaObjetoConocimientoProyectoPestanya filaFiltro in pFilasFacetaObjetoConocimientoProyectoPestanya)
                {
                    listaPestanyasFacetaProyecto.Add(filaFiltro.PestanyaID);
                }
            }

            return listaPestanyasFacetaProyecto;
        }

        private Dictionary<string, string> ObtenerFiltrosProyecto(List<FacetaFiltroProyecto> pFilasFiltroProyecto)
        {
            Dictionary<string, string> listaFiltrosProyecto = new Dictionary<string, string>();

            //si hay filtros, pinta la faceta con los filtros definidos
            if (pFilasFiltroProyecto != null && pFilasFiltroProyecto.Count() > 0)
            {
                foreach (FacetaFiltroProyecto filaFiltro in pFilasFiltroProyecto)
                {
                    listaFiltrosProyecto.Add(filaFiltro.Filtro, filaFiltro.Orden.ToString());
                }
            }

            return listaFiltrosProyecto;
        }

        private Dictionary<string, string> ObtenerFiltrosHomeProyecto(List<FacetaHome> pFilasFacetaHome, out string pPestanyaFaceta, out string pOrdenHome, out bool pVerMas)
        {
            pPestanyaFaceta = string.Empty;
            pOrdenHome = string.Empty;
            pVerMas = false;

            Dictionary<string, string> listaFiltrosHome = new Dictionary<string, string>();
            if (pFilasFacetaHome.Count > 0)
            {
                FacetaHome facetaHome = pFilasFacetaHome[0];

                pPestanyaFaceta = facetaHome.PestanyaFaceta;
                pOrdenHome = facetaHome.Orden.ToString();
                pVerMas = facetaHome.MostrarVerMas;

                var filasFiltroHome = facetaHome.FacetaFiltroHome;
                if (filasFiltroHome.Count > 0)
                {
                    foreach (FacetaFiltroHome filaFiltro in filasFiltroHome)
                    {
                        listaFiltrosHome.Add(filaFiltro.Filtro, filaFiltro.Orden.ToString());
                    }
                }
            }

            return listaFiltrosHome;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="facOcPr"></param>
        /// <param name="pListaFiltrosProyecto"></param>
        /// <param name="pestanyaFaceta"></param>
        /// <param name="ordenHome"></param>
        /// <param name="listaFiltrosHome"></param>
        /// <returns></returns>
        public XmlElement PintarFacetaObjetoConocimientoProyectoXML(XmlDocument pXmlDocument, XmlElement pNodoPadre, FacetaObjetoConocimientoProyecto pFacOcPr, Dictionary<string, string> pListaFiltrosProyecto, List<Guid> pListaPestanyasFaceta, string pestanyaFaceta, string ordenHome, bool verMas, Dictionary<string, string> pListaFiltrosHome)
        {
            string disenio = ObtenerDisenioFaceta((short)pFacOcPr.TipoDisenio);

            string transformacion = ObtenerAlgoritmoTransformacionFaceta((short)pFacOcPr.AlgoritmoTransformacion);

            string propiedad = ObtenerTipoPropiedadFaceta((short)pFacOcPr.TipoPropiedad);

            string comportamiento = ObtenerComportamientoFaceta((short)pFacOcPr.Comportamiento);

            string facetaEsSemantica = Convert.ToInt32(pFacOcPr.EsSemantica).ToString();
            string facetaMayusculas = Convert.ToInt32(pFacOcPr.Mayusculas).ToString();
            string facetaAutocompletar = Convert.ToInt32(pFacOcPr.Autocompletar).ToString();
            string facetaVerMas = Convert.ToInt32(verMas).ToString();

            XmlElement NodoFaceta = ObtenerNodoFaceta(pXmlDocument, pFacOcPr.Faceta, pFacOcPr.ObjetoConocimiento, pListaPestanyasFaceta, pFacOcPr.NombreFaceta, disenio, pFacOcPr.ElementosVisibles.ToString(), transformacion, pFacOcPr.NivelSemantico, facetaEsSemantica, facetaMayusculas, facetaAutocompletar, propiedad, pFacOcPr.Orden.ToString(), pListaFiltrosProyecto, comportamiento, pestanyaFaceta, ordenHome, facetaVerMas, pListaFiltrosHome, pFacOcPr.Excluyente, pFacOcPr.FacetaPrivadaParaGrupoEditores, pFacOcPr.Condicion, pFacOcPr.OcultaEnFacetas, pFacOcPr.OcultaEnFiltros, pFacOcPr.Reciproca, pFacOcPr.ComportamientoOr, pFacOcPr.PriorizarOrdenResultados);

            return NodoFaceta;
        }

        private XmlElement ObtenerNodoFaceta(XmlDocument pXmlDocument, string pClaveFaceta, string pObjetoConocimiento, List<Guid> pListPestanyasID, string pNombrefaceta, string pTipoDisenio, string pElementosVisibles, string pAlgoritmoTransformacion, string pNivelSemantico, string pEsSemantica, string pMayusculas, string pAutocompletar, string pTipoPropiedad, string pOrden, Dictionary<string, string> pListaFiltrosProyecto, string pComportamiento, string pPestanyaFaceta, string pOrdenHome, string pMostrarVerMas, Dictionary<string, string> pListaFiltrosHome, bool pExcluyente, string pFacetaPrivadaParaGrupoEditores, string pCondicion, bool pOcultaEnFacetas, bool pOcultaEnFiltros, int pReciproca, bool pComportamientoOr, bool pPriorizarOrdenResultados)
        {
            // Creamos el nodo Faceta
            XmlElement NodoFaceta = pXmlDocument.CreateElement("Faceta");

            
            if (pReciproca != 0)
            {
                //Si es reciproca generamos un nodo con la clave de la faceta y otro con la reciprocidad
                string[] faceta = pClaveFaceta.Split(new string[]{"@@@"},StringSplitOptions.RemoveEmptyEntries);
                string claveFaceta = "";
                string reciproca = "";

                int i = 0;
                foreach (string trozo in faceta)
                {
                    if (i < pReciproca)
                    {
                        if (!string.IsNullOrEmpty(reciproca))
                        {
                            reciproca += "@@@";
                        }
                        reciproca += trozo;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(claveFaceta))
                        {
                            claveFaceta += "@@@";
                        }
                        claveFaceta += trozo;
                    }
                    i++;
                }

                // Creamos el nodo ClaveFaceta
                AgregarNodoAlPadre(pXmlDocument, NodoFaceta, "ClaveFaceta", claveFaceta);

                // Creamos el nodo Reciproca
                AgregarNodoAlPadre(pXmlDocument, NodoFaceta, "Reciproca", reciproca);
            }
            else
            {
                // Creamos el nodo ClaveFaceta
                AgregarNodoAlPadre(pXmlDocument, NodoFaceta, "ClaveFaceta", pClaveFaceta);
            }

            // Creamos el nodo ObjetoConocimiento
            AgregarNodoAlPadre(pXmlDocument, NodoFaceta, "ObjetoConocimiento", pObjetoConocimiento);

            // Creamos el nodo PestanyaID
            AgregarPestanyasNodosPadre(pXmlDocument, NodoFaceta, pListPestanyasID, "Pestanyas", "NombreCortoPestanya");

            // Creamos el nodo NombreFaceta
            AgregarNodoAlPadre(pXmlDocument, NodoFaceta, "NombreFaceta", pNombrefaceta);

            // Creamos el nodo TipoDisenio
            AgregarNodoAlPadre(pXmlDocument, NodoFaceta, "TipoDisenio", pTipoDisenio);

            // Creamos el nodo ElementosVisibles
            AgregarNodoAlPadre(pXmlDocument, NodoFaceta, "ElementosVisibles", pElementosVisibles);

            // Creamos el nodo AlgoritmoTransformacion
            AgregarNodoAlPadre(pXmlDocument, NodoFaceta, "AlgoritmoTransformacion", pAlgoritmoTransformacion);

            if (pAlgoritmoTransformacion == "TesauroSemantico" && !string.IsNullOrEmpty(pNivelSemantico))
            {
                // Creamos el nodo AlgoritmoTransformacion
                AgregarNodoAlPadre(pXmlDocument, NodoFaceta, "NivelSemantico", pNivelSemantico);
            }

            // Creamos el nodo EsSemantica
            AgregarNodoAlPadre(pXmlDocument, NodoFaceta, "EsSemantica", pEsSemantica);

            // Creamos el nodo Mayusculas
            AgregarNodoAlPadre(pXmlDocument, NodoFaceta, "Mayusculas", pMayusculas);

            // Creamos el nodo Autocompletar
            AgregarNodoAlPadre(pXmlDocument, NodoFaceta, "Autocompletar", pAutocompletar);

            // Creamos el nodo TipoPropiedad
            AgregarNodoAlPadre(pXmlDocument, NodoFaceta, "TipoPropiedad", pTipoPropiedad);

            // Creamos el nodo OrdenFaceta
            AgregarNodoAlPadre(pXmlDocument, NodoFaceta, "OrdenFaceta", pOrden);

            AgregarFiltrosNodosPadre(pXmlDocument, NodoFaceta, pListaFiltrosProyecto, "Filtros", "Filtro");

            // Creamos el nodo Comportamiento
            AgregarNodoAlPadre(pXmlDocument, NodoFaceta, "Comportamiento", pComportamiento);

            // Creamos el nodo PestanyaFacetaHome
            AgregarNodoAlPadre(pXmlDocument, NodoFaceta, "PestanyaFacetaHome", pPestanyaFaceta);

            // Creamos el nodo OrdenHome
            AgregarNodoAlPadre(pXmlDocument, NodoFaceta, "OrdenHome", pOrdenHome);

            // Creamos el nodo MostrarVerMas
            AgregarNodoAlPadre(pXmlDocument, NodoFaceta, "MostrarVerMas", pMostrarVerMas);

            AgregarFiltrosNodosPadre(pXmlDocument, NodoFaceta, pListaFiltrosHome, "FiltrosHome", "FiltroHome");

            //la configuración por defecto es 1 (si el nodo no está definido en el XML)
            //por lo tanto se muestra cuando la faceta no es excluyente
            if (!pExcluyente)
            {
                // Creamos el nodo Excluida
                AgregarNodoAlPadre(pXmlDocument, NodoFaceta, "Excluyente", "0");
            }

            if (!string.IsNullOrEmpty(pFacetaPrivadaParaGrupoEditores))
            {
                // Creamos el nodo FacetaPrivadaParaGrupoEditores
                AgregarNodoAlPadre(pXmlDocument, NodoFaceta, "FacetaPrivadaParaGrupoEditores", pFacetaPrivadaParaGrupoEditores);
            }

            if (!string.IsNullOrEmpty(pCondicion))
            {
                // Creamos el nodo SubTipo
                AgregarNodoAlPadre(pXmlDocument, NodoFaceta, "Condicion", pCondicion);
            }

            if (pOcultaEnFacetas)
            {
                // Creamos el nodo Oculta en facetas
                AgregarNodoAlPadre(pXmlDocument, NodoFaceta, "OcultaEnFacetas", "1");
            }

            if (pOcultaEnFiltros)
            {
                // Creamos el nodo Oculta en filtros
                AgregarNodoAlPadre(pXmlDocument, NodoFaceta, "OcultaEnFiltros", "1");
            }

            if (pComportamientoOr)
            {
                // Creamos el nodo Oculta
                AgregarNodoAlPadre(pXmlDocument, NodoFaceta, "ComportamientoOr", "1");
            }

            if (pPriorizarOrdenResultados)
            {
                // Creamos el nodo Oculta en filtros
                AgregarNodoAlPadre(pXmlDocument, NodoFaceta, "PriorizarOrdenResultados", "1");
            }

            return NodoFaceta;
        }

        private void AgregarFiltrosNodosPadre(XmlDocument pXmlDocument, XmlElement pNodoFaceta, Dictionary<string, string> pListaFiltros, string pParentKey, string pChildsKeys)
        {
            if (pListaFiltros != null && pListaFiltros.Count > 0)
            {
                // Creamos el nodo Filtros
                XmlElement NodoFiltros = pXmlDocument.CreateElement(pParentKey);
                pNodoFaceta.AppendChild(NodoFiltros);

                foreach (string filtro in pListaFiltros.Keys)
                {
                    // Creamos el nodo Filtro
                    XmlElement NodoFiltro = pXmlDocument.CreateElement(pChildsKeys);
                    NodoFiltros.AppendChild(NodoFiltro);

                    //Cuando contiene un ';' es porque se separa la faceta de los niveles a pintar.
                    if (filtro.Contains(";"))
                    {
                        // Creamos el nodo Valor
                        AgregarNodoAlPadre(pXmlDocument, NodoFiltro, "Valor", filtro);
                    }
                    else
                    {
                        string[] filtros = filtro.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string filtroIn in filtros)
                        {
                            // Creamos el nodo Valor
                            AgregarNodoAlPadre(pXmlDocument, NodoFiltro, "Valor", filtroIn);
                        }
                    }


                    // Creamos el nodo OrdenFiltro
                    AgregarNodoAlPadre(pXmlDocument, NodoFiltro, "OrdenFiltro", pListaFiltros[filtro]);
                }
            }
        }

        private void AgregarPestanyasNodosPadre(XmlDocument pXmlDocument, XmlElement pNodoFaceta, List<Guid> pListaPestanyas, string pParentKey, string pChildsKeys)
        {
            if (pListaPestanyas != null && pListaPestanyas.Count > 0)
            {
                // Creamos el nodo Filtros
                XmlElement NodoFiltros = pXmlDocument.CreateElement(pParentKey);
                pNodoFaceta.AppendChild(NodoFiltros);

                foreach (Guid pestanyaID in pListaPestanyas)
                {
                    List<ProyectoPestanyaMenu> resuladoBusqueda = mDataWrapperProyecto.ListaProyectoPestanyaMenu.Where(proy => proy.PestanyaID.Equals(pestanyaID)).ToList();
                    if (resuladoBusqueda.Count > 0)
                    {
                        ProyectoPestanyaMenu fila = resuladoBusqueda.First();

                        // Creamos el nodo Filtro
                        AgregarNodoAlPadre(pXmlDocument, NodoFiltros, pChildsKeys, fila.NombreCortoPestanya);
                    }
                }
            }
        }

        private string ObtenerComportamientoFaceta(short pTipoMostrarSoloCaja)
        {
            return ((TipoMostrarSoloCaja)pTipoMostrarSoloCaja).ToString();
        }

        private string ObtenerTipoPropiedadFaceta(short pTipoPropiedadFaceta)
        {
            return ((TipoPropiedadFaceta)pTipoPropiedadFaceta).ToString();
        }

        private string ObtenerAlgoritmoTransformacionFaceta(short pTiposAlgoritmoTransformacion)
        {
            return ((TiposAlgoritmoTransformacion)pTiposAlgoritmoTransformacion).ToString();
        }

        private string ObtenerDisenioFaceta(short pTipoDisenio)
        {
            return ((TipoDisenio)pTipoDisenio).ToString();
        }

        #endregion PintarEstructuraXMLFacetas

        #region PintarEstructuraXMLObjetoConocimiento

        private void PintarEstructuraXMLObjetoConocimiento(XmlDocument pXmlDoc, XmlElement pNodoComunidad, DataWrapperProyecto pDataWrapperProyecto, DataWrapperFacetas pFacetaDW, Guid pProyectoID, Guid pOrganizacionID)
        {
            TesauroCN tesauroCN = new TesauroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<TesauroCN>(), mLoggerFactory);

            List<OntologiaProyecto> ontologias = pFacetaDW.ListaOntologiaProyecto.Where(item => item.OrganizacionID.Equals(pOrganizacionID) && item.ProyectoID.Equals(pProyectoID)).ToList();
            if (ontologias != null && ontologias.Count > 0)
            {
                // Creamos el nodo Ocs
                XmlElement NodoOcs = pXmlDoc.CreateElement("Ocs");
                pNodoComunidad.AppendChild(NodoOcs);

                foreach (OntologiaProyecto ont in ontologias)
                {
                    // Creamos el nodo Oc
                    XmlElement NodoOc = pXmlDoc.CreateElement("Oc");
                    NodoOcs.AppendChild(NodoOc);

                    //obtengo el ontologiaID
                    DocumentacionCN documentacionCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);

                    Guid idProyectoOntolgias = pProyectoID;
                    if (ParametroProyecto.ContainsKey(ParametroAD.ProyectoIDPatronOntologias))
                    {
                       idProyectoOntolgias=new Guid(ParametroProyecto[ParametroAD.ProyectoIDPatronOntologias]);
                    }

                    Guid ontologiaID = documentacionCN.ObtenerOntologiaAPartirNombre(idProyectoOntolgias, ont.OntologiaProyecto1 + ".owl");

                    // Creamos el nodo Ontologia
                    AgregarNodoAlPadre(pXmlDoc, NodoOc, "Ontologia", ont.OntologiaProyecto1);

                    // Creamos el nodo NombreOnt
                    AgregarNodoAlPadre(pXmlDoc, NodoOc, "NombreOnt", ont.NombreOnt);

                    // Creamos el nodo Namespace
                    AgregarNodoAlPadre(pXmlDoc, NodoOc, "Namespace", ont.Namespace);

                    // Creamos el nodo NamespacesExtra
                    AgregarNodoAlPadre(pXmlDoc, NodoOc, "NamespacesExtra", ont.NamespacesExtra);

                    if (ont.NombreCortoOnt != null && !string.IsNullOrEmpty(ont.NombreCortoOnt))
                    {
                        // Creamos el nodo NombreCortoOntologia
                        AgregarNodoAlPadre(pXmlDoc, NodoOc, "NombreCortoOnt", ont.NombreCortoOnt);
                    }
                    if(!ont.CachearDatosSemanticos)
                    {
                        // Creamos el nodo CachearDatosSemanticos
                        AgregarNodoAlPadre(pXmlDoc, NodoOc, "CachearDatosSemanticos", Convert.ToInt32(ont.CachearDatosSemanticos).ToString());
                    }

                    if (!ont.EsBuscable)
                    {
                        // Creamos el nodo NamespacesExtra
                        AgregarNodoAlPadre(pXmlDoc, NodoOc, "EsBuscable", Convert.ToInt32(ont.EsBuscable).ToString());
                    }

                    if (ont.SubTipos != null && !string.IsNullOrEmpty(ont.SubTipos))
                    {
                        // Creamos el nodo SubTipos
                        XmlElement NodoSubTipos = pXmlDoc.CreateElement("SubTipos");
                        NodoOc.AppendChild(NodoSubTipos);

                        foreach (string datosSubTipo in ont.SubTipos.Split(new string[] { "[|||]" }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            string subTipo = datosSubTipo.Substring(0, datosSubTipo.IndexOf("|||"));
                            string nombres = datosSubTipo.Substring(datosSubTipo.IndexOf("|||") + 3);

                            XmlElement NodoSubTipo = pXmlDoc.CreateElement("SubTipo");
                            NodoSubTipos.AppendChild(NodoSubTipo);

                            AgregarNodoAlPadre(pXmlDoc, NodoSubTipo, "Tipo", subTipo);

                            AgregarNodoAlPadre(pXmlDoc, NodoSubTipo, "NombreSubTipo", nombres);
                        }
                    }

                    string nombreTesauro = tesauroCN.ObtenerNombreTesauroProyOnt(pProyectoID, ontologiaID.ToString());
                    if (nombreTesauro != string.Empty)
                    {
                        // Creamos el nodo NombreTesauroExclusivo
                        AgregarNodoAlPadre(pXmlDoc, NodoOc, "NombreTesauroExclusivo", nombreTesauro);
                    }

                    PintarEstructuraXMLObjetoConocimientoListadoSemantico(pXmlDoc, NodoOc, pDataWrapperProyecto, pOrganizacionID, pProyectoID, ontologiaID);

                    PintarEstructuraXMLObjetoConocimientoMosaicoSemantico(pXmlDoc, NodoOc, pDataWrapperProyecto, pOrganizacionID, pProyectoID, ontologiaID);

                    PintarEstructuraXMLObjetoConocimientoMapaSemantico(pXmlDoc, NodoOc, pDataWrapperProyecto, pOrganizacionID, pProyectoID, ontologiaID);

                    PintarEstructuraXMLObjetoConocimientoRecursosRelacionadosPresentacion(pXmlDoc, NodoOc, pDataWrapperProyecto, pOrganizacionID, pProyectoID, ontologiaID);
                }
            }

            tesauroCN.Dispose();
        }

        private void PintarEstructuraXMLObjetoConocimientoRecursosRelacionadosPresentacion(XmlDocument pXmlDoc, XmlElement pNodoObjetoConocimiento, DataWrapperProyecto pDataWrapperProyecto, Guid pOrganizacionID, Guid pProyectoID, Guid pOntologiaID)
        {
            List<RecursosRelacionadosPresentacion> filasRecRel = pDataWrapperProyecto.ListaRecursosRelacionadosPresentacion.Where(recurso => recurso.OrganizacionID.Equals(pOrganizacionID) && recurso.ProyectoID.Equals(pProyectoID) && recurso.OntologiaID.Equals(pOntologiaID)).ToList();
            if (filasRecRel != null && filasRecRel.Count > 0)
            {
                foreach (RecursosRelacionadosPresentacion filaRecRel in filasRecRel)
                {
                    string imagen = ObtenerTipoImagen(filaRecRel.Imagen);

                    //quito ontología porque ya se la hemos pedido en <Oc> y la url y el .owl son constantes y sólo se le añade la propia ontología
                    //ej.: "http://gnoss.com/Ontologia/" researcher ".owl#"
                    //\n\t\t\t\t<Ontologia>" + filaRecRel.Ontologia + "</Ontologia>


                    // Creamos el nodo RecursosRelacionadosPresentacion
                    XmlElement NodoRecursosRelacionadosPresentacion = pXmlDoc.CreateElement("RecursosRelacionadosPresentacion");
                    pNodoObjetoConocimiento.AppendChild(NodoRecursosRelacionadosPresentacion);

                    AgregarNodosPresentacionAlNodoPadre(pXmlDoc, NodoRecursosRelacionadosPresentacion, filaRecRel.Orden.ToString(), filaRecRel.Propiedad, filaRecRel.Nombre, imagen);
                }
            }
        }

        private void PintarEstructuraXMLObjetoConocimientoMapaSemantico(XmlDocument pXmlDoc, XmlElement pNodoObjetoConocimiento, DataWrapperProyecto pDataWrapperProyecto, Guid pOrganizacionID, Guid pProyectoID, Guid pOntologiaID)
        {
            List<PresentacionMapaSemantico> filasMapaSem = pDataWrapperProyecto.ListaPresentacionMapaSemantico.Where(presentacion => presentacion.OrganizacionID.Equals(pOrganizacionID) && presentacion.ProyectoID.Equals(pProyectoID) && presentacion.OntologiaID.Equals(pOntologiaID)).ToList();
            if (filasMapaSem != null && filasMapaSem.Count > 0)
            {
                foreach (PresentacionMapaSemantico filaMapaSem in filasMapaSem)
                {
                    //quito ontología porque ya se la hemos pedido en <Oc> y la url y el .owl son constantes y sólo se le añade la propia ontología
                    //ej.: "http://gnoss.com/Ontologia/" researcher ".owl#"
                    //\n\t\t\t\t<Ontologia>" + filaMapaSem.Ontologia + "</Ontologia>

                    // Creamos el nodo PresentacionMapaSemantico
                    XmlElement NodoPresentacionMapaSemantico = pXmlDoc.CreateElement("PresentacionMapaSemantico");
                    pNodoObjetoConocimiento.AppendChild(NodoPresentacionMapaSemantico);

                    AgregarNodosPresentacionAlNodoPadre(pXmlDoc, NodoPresentacionMapaSemantico, filaMapaSem.Orden.ToString(), filaMapaSem.Propiedad, filaMapaSem.Nombre);
                }
            }
        }

        private void PintarEstructuraXMLObjetoConocimientoMosaicoSemantico(XmlDocument pXmlDoc, XmlElement pNodoObjetoConocimiento, DataWrapperProyecto pDataWrapperProyecto, Guid pOrganizacionID, Guid pProyectoID, Guid pOntologiaID)
        {
            List<PresentacionMosaicoSemantico> filasMosaicoSem = pDataWrapperProyecto.ListaPresentacionMosaicoSemantico.Where(presentacion=>presentacion.OrganizacionID.Equals(pOrganizacionID) && presentacion.ProyectoID.Equals(pProyectoID) && presentacion.OntologiaID.Equals(pOntologiaID)).ToList();
            if (filasMosaicoSem != null && filasMosaicoSem.Count > 0)
            {
                foreach (PresentacionMosaicoSemantico filaMosaicoSem in filasMosaicoSem)
                {
                    //quito ontología porque ya se la hemos pedido en <Oc> y la url y el .owl son constantes y sólo se le añade la propia ontología
                    //ej.: "http://gnoss.com/Ontologia/" researcher ".owl#"
                    //\n\t\t\t\t<Ontologia>" + filaMosaicoSem.Ontologia + "</Ontologia>


                    // Creamos el nodo PresentacionMosaicoSemantico
                    XmlElement NodoPresentacionMosaicoSemantico = pXmlDoc.CreateElement("PresentacionMosaicoSemantico");
                    pNodoObjetoConocimiento.AppendChild(NodoPresentacionMosaicoSemantico);

                    AgregarNodosPresentacionAlNodoPadre(pXmlDoc, NodoPresentacionMosaicoSemantico, filaMosaicoSem.Orden.ToString(), filaMosaicoSem.Propiedad, filaMosaicoSem.Nombre);
                }
            }
        }

        private void PintarEstructuraXMLObjetoConocimientoListadoSemantico(XmlDocument pXmlDoc, XmlElement pNodoObjetoConocimiento, DataWrapperProyecto pDataWrapperProyecto, Guid pOrganizacionID, Guid pProyectoID, Guid pOntologiaID)
        {
            List<PresentacionListadoSemantico> filasListadoSem = pDataWrapperProyecto.ListaPresentacionListadoSemantico.Where(presentacion => presentacion.OrganizacionID.Equals(pOrganizacionID) && presentacion.ProyectoID.Equals(pProyectoID) && presentacion.OntologiaID.Equals(pOntologiaID)).ToList();

            if (filasListadoSem != null && filasListadoSem.Count > 0)
            {
                foreach (PresentacionListadoSemantico filaListadoSem in filasListadoSem)
                {
                    //quito ontología porque ya se la hemos pedido en <Oc> y la url y el .owl son constantes y sólo se le añade la propia ontología
                    //ej.: "http://gnoss.com/Ontologia/" researcher ".owl#"
                    //\n\t\t\t\t<Ontologia>" + filaListadoSem.Ontologia + "</Ontologia>

                    // Creamos el nodo PresentacionListadoSemantico
                    XmlElement NodoPresentacionListadoSemantico = pXmlDoc.CreateElement("PresentacionListadoSemantico");
                    pNodoObjetoConocimiento.AppendChild(NodoPresentacionListadoSemantico);

                    AgregarNodosPresentacionAlNodoPadre(pXmlDoc, NodoPresentacionListadoSemantico, filaListadoSem.Orden.ToString(), filaListadoSem.Propiedad, filaListadoSem.Nombre);
                }
            }
        }
        
        private void PintarEstructuraXMLObjetoConocimientoMapaSemanticoPestanya(XmlDocument pXmlDoc, XmlElement pNodoPestanya, DataWrapperProyecto pDataWrapperProyecto, Guid pOrganizacionID, Guid pProyectoID, Guid pPestanyaID)
        {
            List<PresentacionPestanyaMapaSemantico> filasMapaSem = pDataWrapperProyecto.ListaPresentacionPestanyaMapaSemantico.Where(presentacion => presentacion.OrganizacionID.Equals(pOrganizacionID) && presentacion.ProyectoID.Equals(pProyectoID) && presentacion.PestanyaID.Equals(pPestanyaID)).ToList();
            if (filasMapaSem != null && filasMapaSem.Count > 0)
            {
                foreach (PresentacionPestanyaMapaSemantico filaMapaSem in filasMapaSem)
                {
                    // Creamos el nodo PresentacionMapaSemantico
                    XmlElement NodoPresentacionMapaSemantico = pXmlDoc.CreateElement("PresentacionMapaSemantico");
                    pNodoPestanya.AppendChild(NodoPresentacionMapaSemantico);

                    AgregarNodosPresentacionAlNodoPadre(pXmlDoc, NodoPresentacionMapaSemantico, filaMapaSem.Orden.ToString(), filaMapaSem.Propiedad, filaMapaSem.Nombre,null,filaMapaSem.OntologiaID,filaMapaSem.Ontologia);
                }
            }
        }

        private void PintarEstructuraXMLObjetoConocimientoMosaicoSemanticoPestanya(XmlDocument pXmlDoc, XmlElement pNodoPestanya, DataWrapperProyecto pDataWrapperProyecto, Guid pOrganizacionID, Guid pProyectoID, Guid pPestanyaID)
        {
            List<PresentacionPestanyaMosaicoSemantico> filasMosaicoSem = pDataWrapperProyecto.ListaPresentacionPestanyaMosaicoSemantico.Where(presentacion => presentacion.OrganizacionID.Equals(pOrganizacionID) && presentacion.ProyectoID.Equals(pProyectoID) && presentacion.PestanyaID.Equals(pPestanyaID)).ToList();
            if (filasMosaicoSem != null && filasMosaicoSem.Count > 0)
            {
                foreach (PresentacionPestanyaMosaicoSemantico filaMosaicoSem in filasMosaicoSem)
                {
                    // Creamos el nodo PresentacionMosaicoSemantico
                    XmlElement NodoPresentacionMosaicoSemantico = pXmlDoc.CreateElement("PresentacionMosaicoSemantico");
                    pNodoPestanya.AppendChild(NodoPresentacionMosaicoSemantico);

                    AgregarNodosPresentacionAlNodoPadre(pXmlDoc, NodoPresentacionMosaicoSemantico, filaMosaicoSem.Orden.ToString(), filaMosaicoSem.Propiedad, filaMosaicoSem.Nombre,null,filaMosaicoSem.OntologiaID,filaMosaicoSem.Ontologia);
                }
            }
        }

        private void PintarEstructuraXMLObjetoConocimientoListadoSemanticoPestanya(XmlDocument pXmlDoc, XmlElement pNodoPestanya, DataWrapperProyecto pDataWrapperProyecto, Guid pOrganizacionID, Guid pProyectoID, Guid pPestanyaID)
        {
            List<PresentacionPestanyaListadoSemantico> filasListadoSem = pDataWrapperProyecto.ListaPresentacionPestanyaListadoSemantico.Where(presentacion => presentacion.OrganizacionID.Equals(pOrganizacionID) && presentacion.ProyectoID.Equals(pProyectoID) && presentacion.PestanyaID.Equals(pPestanyaID)).ToList();

            if (filasListadoSem != null && filasListadoSem.Count > 0)
            {
                foreach (PresentacionPestanyaListadoSemantico filaListadoSem in filasListadoSem)
                {
                    // Creamos el nodo PresentacionListadoSemantico
                    XmlElement NodoPresentacionListadoSemantico = pXmlDoc.CreateElement("PresentacionListadoSemantico");
                    pNodoPestanya.AppendChild(NodoPresentacionListadoSemantico);

                    AgregarNodosPresentacionAlNodoPadre(pXmlDoc, NodoPresentacionListadoSemantico, filaListadoSem.Orden.ToString(), filaListadoSem.Propiedad, filaListadoSem.Nombre,null,filaListadoSem.OntologiaID,filaListadoSem.Ontologia);
                }
            }
        }


        private void AgregarNodosPresentacionAlNodoPadre(XmlDocument pXmlDoc, XmlElement pNodoPresentacion, string pOrden, string pPropiedad, string pNombre, string pImagen = null,Guid? pOntologiaID=null, string pOntologia=null)
        {
            // Creamos el nodo Orden
            AgregarNodoAlPadre(pXmlDoc, pNodoPresentacion, "Orden", pOrden);

            // Creamos el nodo Propiedad
            AgregarNodoAlPadre(pXmlDoc, pNodoPresentacion, "Propiedad", pPropiedad, true);

            // Creamos el nodo Nombre
            AgregarNodoAlPadre(pXmlDoc, pNodoPresentacion, "Nombre", pNombre, true);

            if (pOntologiaID.HasValue)
            {
                // Creamos el nodo OntologiaID
                AgregarNodoAlPadre(pXmlDoc, pNodoPresentacion, "OntologiaID", pOntologiaID.Value.ToString(), true);
            }

            if (!string.IsNullOrEmpty(pOntologia))
            {
                // Creamos el nodo Ontologia
                AgregarNodoAlPadre(pXmlDoc, pNodoPresentacion, "Ontologia", pOntologia, true);
            }

            if (!string.IsNullOrEmpty(pImagen))
            {
                // Creamos el nodo Imagen
                AgregarNodoAlPadre(pXmlDoc, pNodoPresentacion, "Imagen", pImagen);

            }
        }

        #endregion PintarEstructuraXMLObjetoConocimiento

        #region PintarEstructuraXMLPestanyas

        private void PintarEstructuraXMLPestanyas(XmlDocument pXmlDoc, XmlElement pNodoComunidad, DataWrapperProyecto pDataWrapperProyecto, ParametroGeneral pFilaParametroGral, ConfiguracionAmbitoBusquedaProyecto pFilaConfBusqueda, DataWrapperExportacionBusqueda pExportacionBusquedaDW, Guid pProyectoID)
        {
            // Creamos el nodo Pestanyas
            XmlElement NodoPestanyas = pXmlDoc.CreateElement("Pestanyas");
            pNodoComunidad.AppendChild(NodoPestanyas);
            List<ProyectoPestanyaMenu> pestanyas = pDataWrapperProyecto.ListaProyectoPestanyaMenu.Where(proy=>proy.ProyectoID.Equals(pProyectoID) && proy.TipoPestanya!= (short)TipoPestanyaMenu.CMS).OrderBy(proy=>proy.Orden).ToList();
            if (pestanyas != null && pestanyas.Count > 0)
            {
                if (pFilaParametroGral != null)
                {
                    // Creamos el nodo CMSDisponible
                    AgregarNodoAlPadre(pXmlDoc, NodoPestanyas, "CMSDisponible", Convert.ToInt32(pFilaParametroGral.CMSDisponible).ToString());
                }

                if (pFilaConfBusqueda != null)
                {
                    // Creamos el nodo AmbitoTodoGnossVisible
                    AgregarNodoAlPadre(pXmlDoc, NodoPestanyas, "AmbitoTodoGnossVisible", Convert.ToInt32(pFilaConfBusqueda.TodoGnoss).ToString());

                    // Creamos el nodo AmbitoTodaLaComunidadVisible
                    AgregarNodoAlPadre(pXmlDoc, NodoPestanyas, "AmbitoTodaLaComunidadVisible", Convert.ToInt32(pFilaConfBusqueda.Metabusqueda).ToString());
                }
                List<ProyectoPestanyaMenu> filasProyectoPestanyaMenu = pDataWrapperProyecto.ListaProyectoPestanyaMenu.Where(proy=>proy.ProyectoID.Equals(pProyectoID) && !proy.PestanyaPadreID.HasValue).OrderBy(proy=>proy.Orden).ToList();

                PintarPestanyasXML(pDataWrapperProyecto, filasProyectoPestanyaMenu, pXmlDoc, NodoPestanyas, pProyectoID, pExportacionBusquedaDW, pFilaConfBusqueda);

            }

        }

        private void PintarPestanyasXML(DataWrapperProyecto pDataWrapperProyecto, List<ProyectoPestanyaMenu> pPestanyas, XmlDocument pXmlDoc, XmlElement pNodoPestanyas, Guid pProyectoID, DataWrapperExportacionBusqueda pExportacionBusquedaDW, ConfiguracionAmbitoBusquedaProyecto pFilaConfBusqueda)
        {
            ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
            OrganizacionCN organizacionCN = new OrganizacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<OrganizacionCN>(), mLoggerFactory);

            foreach (ProyectoPestanyaMenu pestanya in pPestanyas)
            {
                // Creamos el nodo Pestanya
                XmlElement NodoPestanya = pXmlDoc.CreateElement("Pestanya");
                pNodoPestanyas.AppendChild(NodoPestanya);

                // Creamos el nodo TipoPestanya
                AgregarNodoAlPadre(pXmlDoc, NodoPestanya, "TipoPestanya", ((TipoPestanyaMenu)pestanya.TipoPestanya).ToString());

                PintarEstructuraXMLObjetoConocimientoListadoSemanticoPestanya(pXmlDoc, NodoPestanya, pDataWrapperProyecto, pestanya.OrganizacionID, pProyectoID, pestanya.PestanyaID);
                PintarEstructuraXMLObjetoConocimientoMosaicoSemanticoPestanya(pXmlDoc, NodoPestanya, pDataWrapperProyecto, pestanya.OrganizacionID, pProyectoID, pestanya.PestanyaID);
                PintarEstructuraXMLObjetoConocimientoMapaSemanticoPestanya(pXmlDoc, NodoPestanya, pDataWrapperProyecto, pestanya.OrganizacionID, pProyectoID, pestanya.PestanyaID);

                // Creamos el nodo NombreCorto
                AgregarNodoAlPadre(pXmlDoc, NodoPestanya, "NombreCorto", pestanya.NombreCortoPestanya.ToString());

                // Creamos el nodo PestanyaDefecto
                if (pFilaConfBusqueda != null && !(pFilaConfBusqueda.PestanyaDefectoID==null) && pFilaConfBusqueda.PestanyaDefectoID == pestanya.PestanyaID)
                {
                    AgregarNodoAlPadre(pXmlDoc, NodoPestanya, "PestanyaDefecto", "1");
                }

                // Creamos el nodo Visible
                AgregarNodoAlPadre(pXmlDoc, NodoPestanya, "Visible", Convert.ToInt32(pestanya.Visible).ToString());

                if (!pestanya.VisibleSinAcceso)
                {
                    // Creamos el nodo VisibleSinAcceso
                    AgregarNodoAlPadre(pXmlDoc, NodoPestanya, "VisibleSinAcceso", Convert.ToInt32(pestanya.VisibleSinAcceso).ToString());
                }

                if (!string.IsNullOrEmpty(pestanya.CSSBodyClass))
                {
                    // Creamos el nodo CSSBodyClass
                    AgregarNodoAlPadre(pXmlDoc, NodoPestanya, "CSSBodyClass", pestanya.CSSBodyClass);
                }

                // Creamos el nodo Activa
                AgregarNodoAlPadre(pXmlDoc, NodoPestanya, "Activa", Convert.ToInt32(pestanya.Activa).ToString());

                if (!string.IsNullOrEmpty(pestanya.Nombre))
                {
                    // Creamos el nodo Nombre
                    AgregarNodoAlPadre(pXmlDoc, NodoPestanya, "Nombre", pestanya.Nombre);
                }

                if (!string.IsNullOrEmpty(pestanya.Ruta))
                {
                    // Creamos el nodo Ruta
                    AgregarNodoAlPadre(pXmlDoc, NodoPestanya, "Ruta", pestanya.Ruta);
                }

                if (!string.IsNullOrEmpty(pestanya.Titulo))
                {
                    // Creamos el nodo Titulo
                    AgregarNodoAlPadre(pXmlDoc, NodoPestanya, "Titulo", pestanya.Titulo);
                }

                if (!string.IsNullOrEmpty(pestanya.IdiomasDisponibles))
                {
                    string idiomas="";
					ParametroAplicacionCL paramCL = new ParametroAplicacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCL>(), mLoggerFactory);
					List<string> listaIdiomas = paramCL.ObtenerListaIdiomas();
                    foreach (string idioma in listaIdiomas)
                    {
                        if (UtilCadenas.ObtenerTextoDeIdioma(pestanya.IdiomasDisponibles, idioma, null, true) == "true")
                        {
                            if (!string.IsNullOrEmpty(idiomas))
                            {
                                idiomas += ",";
                            }
                            idiomas += idioma;
                        }
                    }

                    // Creamos el nodo IdiomasDisponibles
                    AgregarNodoAlPadre(pXmlDoc, NodoPestanya, "IdiomasDisponibles", idiomas);
                }

                if (pestanya.NuevaPestanya)
                {
                    // Creamos el nodo NuevaPestanya
                    AgregarNodoAlPadre(pXmlDoc, NodoPestanya, "NuevaPestanya", Convert.ToInt32(pestanya.NuevaPestanya).ToString());
                }

                if (pestanya.Privacidad != (short)TipoPrivacidadPagina.Normal)
                {
                    // Creamos el nodo privacidad
                    AgregarNodoAlPadre(pXmlDoc, NodoPestanya, "Privacidad", ((TipoPrivacidadPagina)pestanya.Privacidad).ToString());

                    if (pestanya.HtmlAlternativo!=null && !string.IsNullOrEmpty(pestanya.HtmlAlternativo))
                    {
                        // Creamos el nodo HTML alternativo
                        AgregarNodoAlPadre(pXmlDoc, NodoPestanya, "HtmlAlternativo", pestanya.HtmlAlternativo, true);
                    }

                    List<ProyectoPestanyaMenuRolIdentidad> filasPerfiles = pestanya.ProyectoPestanyaMenuRolIdentidad.ToList();
                    List<ProyectoPestanyaMenuRolGrupoIdentidades> filasGrupos = pestanya.ProyectoPestanyaMenuRolGrupoIdentidades.ToList();

                    if (filasPerfiles.Count > 0 || filasGrupos.Count > 0)
                    {
                        XmlElement NodoConfiguracionPrivacidad = pXmlDoc.CreateElement("ConfiguracionPrivacidad");
                        NodoPestanya.AppendChild(NodoConfiguracionPrivacidad);

                        // Creamos el nodo PerfilesPrivados                    
                        if (filasPerfiles.Count > 0)
                        {
                            XmlElement NodoPrivacidadPerfiles = pXmlDoc.CreateElement("PrivacidadPerfiles");
                            NodoConfiguracionPrivacidad.AppendChild(NodoPrivacidadPerfiles);

                            foreach (ProyectoPestanyaMenuRolIdentidad filaPerfil in filasPerfiles)
                            {
                                AgregarNodoAlPadre(pXmlDoc, NodoPrivacidadPerfiles, "Perfil", identidadCN.ObtenerNombreCortoPerfil(filaPerfil.PerfilID).Key);
                            }
                        }

                        // Creamos el nodo GruposPrivados                    
                        if (filasGrupos.Count > 0)
                        {
                            XmlElement NodoPrivacidadGrupos = pXmlDoc.CreateElement("PrivacidadGrupos");
                            NodoConfiguracionPrivacidad.AppendChild(NodoPrivacidadGrupos);

                            List<Guid> listaGrupos = new List<Guid>();
                            foreach (ProyectoPestanyaMenuRolGrupoIdentidades filaGrupo in filasGrupos)
                            {
                                listaGrupos.Add(filaGrupo.GrupoID);
                            }
                            GestionIdentidades gestorIden = new GestionIdentidades(identidadCN.ObtenerGruposPorIDGrupo(listaGrupos, false), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);

                            foreach (Guid idGRupo in gestorIden.ListaGrupos.Keys)
                            {
                                GrupoIdentidades grupoIdentidad = gestorIden.ListaGrupos[idGRupo];
                                if (grupoIdentidad.EsGrupoDeProyecto)
                                {
                                    XmlElement NodoGrupoProy = pXmlDoc.CreateElement("GrupoProy");
                                    NodoPrivacidadGrupos.AppendChild(NodoGrupoProy);

                                    AgregarNodoAlPadre(pXmlDoc, NodoGrupoProy, "NombreCortoGrupo", identidadCN.ObtenerNombreCortoGrupoPorID(grupoIdentidad.Clave));
                                    AgregarNodoAlPadre(pXmlDoc, NodoGrupoProy, "NombreCortoProy", proyectoCN.ObtenerNombreCortoProyecto(grupoIdentidad.FilaGrupoProyecto.ProyectoID));
                                }
                                else
                                {
                                    XmlElement NodoGrupoOrg = pXmlDoc.CreateElement("GrupoOrg");
                                    NodoPrivacidadGrupos.AppendChild(NodoGrupoOrg);

                                    AgregarNodoAlPadre(pXmlDoc, NodoGrupoOrg, "NombreCortoGrupo", identidadCN.ObtenerNombreCortoGrupoPorID(grupoIdentidad.Clave));
                                    AgregarNodoAlPadre(pXmlDoc, NodoGrupoOrg, "NombreCortoOrg", organizacionCN.ObtenerNombreOrganizacionPorID(grupoIdentidad.FilaGrupoOrganizacion.OrganizacionID).NombreCorto);//[0].NombreCorto);
                                }
                            }
                        }
                    }
                }

                if (CrearFilasPropiedadesExportacion && pestanya.TipoPestanya.Equals((short)TipoPestanyaMenu.EnlaceExterno) && !string.IsNullOrEmpty(pestanya.Ruta))
                {
                    //Crear las filas de las porpiedades de Integracion Continua
                    IntegracionContinuaPropiedad propiedadRutaPagina = new IntegracionContinuaPropiedad();
                    propiedadRutaPagina.ProyectoID = ProyectoID;
                    propiedadRutaPagina.TipoObjeto = (short)TipoObjeto.Pagina;
                    propiedadRutaPagina.ObjetoPropiedad = pestanya.NombreCortoPestanya;
                    propiedadRutaPagina.TipoPropiedad = (short)TipoPropiedad.RutaPagina;
                    propiedadRutaPagina.ValorPropiedad = pestanya.Ruta;
                    propiedadesIntegracionContinua.Add(propiedadRutaPagina);
                }

                if (pestanya.TipoPestanya == (short)TipoPestanyaMenu.CMS)
                {
                    // Creamos el nodo UbicacionCMS
                    if(pestanya.ProyectoPestanyaCMS.FirstOrDefault() != null)
                    {
                        AgregarNodoAlPadre(pXmlDoc, NodoPestanya, "UbicacionCMS", pestanya.ProyectoPestanyaCMS.FirstOrDefault().Ubicacion.ToString());
                    }
                    
                }

                if (pestanya.TipoPestanya == (short)TipoPestanyaMenu.BusquedaSemantica
                   || pestanya.TipoPestanya == (short)TipoPestanyaMenu.Recursos
                   || pestanya.TipoPestanya == (short)TipoPestanyaMenu.Preguntas
                   || pestanya.TipoPestanya == (short)TipoPestanyaMenu.Encuestas
                   || pestanya.TipoPestanya == (short)TipoPestanyaMenu.Debates
                   || pestanya.TipoPestanya == (short)TipoPestanyaMenu.PersonasYOrganizaciones
                   || pestanya.TipoPestanya == (short)TipoPestanyaMenu.BusquedaAvanzada)
                {
                    List<ProyectoPestanyaFiltroOrdenRecursos> filasProyOrdRec = pDataWrapperProyecto.ListaProyectoPestanyaFiltroOrdenRecursos.Where(proy => proy.PestanyaID.Equals(pestanya.PestanyaID.ToString())).ToList();
                   
                    if (pestanya.ProyectoPestanyaBusqueda != null || filasProyOrdRec.Count > 0)
                    {
                        // Creamos el nodo Configuracion búsqueda
                        XmlElement NodoConfiguracionBusqueda = pXmlDoc.CreateElement("ConfiguracionBusqueda");
                        NodoPestanya.AppendChild(NodoConfiguracionBusqueda);

                        if (pestanya.ProyectoPestanyaBusqueda != null)
                        {
                            ProyectoPestanyaBusqueda pestanyabusqueda = pestanya.ProyectoPestanyaBusqueda;

                            if (!string.IsNullOrEmpty(pestanyabusqueda.CampoFiltro))
                            {
                                if (CrearFilasPropiedadesExportacion) {
                                    if (!string.IsNullOrEmpty(pestanyabusqueda.CampoFiltro) && pestanyabusqueda.CampoFiltro.Contains("skos:ConceptID"))
                                    {
                                        //Crear las filas de las porpiedades de Integracion Continua
                                        IntegracionContinuaPropiedad propiedadRutaPagina = new IntegracionContinuaPropiedad();
                                        propiedadRutaPagina.ProyectoID = ProyectoID;
                                        propiedadRutaPagina.TipoObjeto = (short)TipoObjeto.Pagina;
                                        propiedadRutaPagina.ObjetoPropiedad = pestanya.NombreCortoPestanya;
                                        propiedadRutaPagina.TipoPropiedad = (short)TipoPropiedad.CampoFiltroPagina;
                                        propiedadRutaPagina.ValorPropiedad = pestanyabusqueda.CampoFiltro;
                                        propiedadesIntegracionContinua.Add(propiedadRutaPagina);
                                    }
                                }

                                // Creamos el nodo CampoFiltro
                                AgregarNodoAlPadre(pXmlDoc, NodoConfiguracionBusqueda, "CampoFiltro", pestanyabusqueda.CampoFiltro);
                            }

                            if (pestanyabusqueda.NumeroRecursos > 0)
                            {
                                // Creamos el nodo NumeroRecursos
                                AgregarNodoAlPadre(pXmlDoc, NodoConfiguracionBusqueda, "NumeroRecursos", pestanyabusqueda.NumeroRecursos.ToString());
                            }

                            if (!string.IsNullOrEmpty(pestanyabusqueda.VistaDisponible))
                            {
                                // Creamos el nodo VistaDisponible
                                AgregarNodoAlPadre(pXmlDoc, NodoConfiguracionBusqueda, "VistaDisponible", pestanyabusqueda.VistaDisponible.ToString());
                            }

                            if (!pestanyabusqueda.MostrarFacetas)
                            {
                                // Creamos el nodo MostrarFacetas
                                AgregarNodoAlPadre(pXmlDoc, NodoConfiguracionBusqueda, "MostrarFacetas", Convert.ToInt32(pestanyabusqueda.MostrarFacetas).ToString());
                            }

                            if (!pestanyabusqueda.MostrarCajaBusqueda)
                            {
                                // Creamos el nodo MostrarCajaBusqueda
                                AgregarNodoAlPadre(pXmlDoc, NodoConfiguracionBusqueda, "MostrarCajaBusqueda", Convert.ToInt32(pestanyabusqueda.MostrarCajaBusqueda).ToString());
                            }

                            if (pestanyabusqueda.ProyectoOrigenID.HasValue)
                            {
                                // Creamos el nodo ProyectoOrigen
                                string proyectoOrigen = proyectoCN.ObtenerNombreCortoProyecto(pestanyabusqueda.ProyectoOrigenID.Value);

                                AgregarNodoAlPadre(pXmlDoc, NodoConfiguracionBusqueda, "ProyectoOrigen", proyectoOrigen);
                            }

                            if (pestanyabusqueda.OcultarResultadosSinFiltros)
                            {
                                // Creamos el nodo OcultarResultadosSinFiltros
                                AgregarNodoAlPadre(pXmlDoc, NodoConfiguracionBusqueda, "OcultarResultadosSinFiltros", Convert.ToInt32(pestanyabusqueda.OcultarResultadosSinFiltros).ToString());
                            }

                            if (!string.IsNullOrEmpty(pestanyabusqueda.PosicionCentralMapa))
                            {
                                // Creamos el nodo PosicionCentralMapa
                                AgregarNodoAlPadre(pXmlDoc, NodoConfiguracionBusqueda, "PosicionCentralMapa", pestanyabusqueda.PosicionCentralMapa);
                            }

                            if (pestanyabusqueda.GruposPorTipo)
                            {
                                // Creamos el nodo gruposPorTipo
                                AgregarNodoAlPadre(pXmlDoc, NodoConfiguracionBusqueda, "GruposPorTipo", Convert.ToInt32(pestanyabusqueda.GruposPorTipo).ToString());
                            }

                            if (!string.IsNullOrEmpty(pestanyabusqueda.GruposConfiguracion))
                            {
                                // Creamos el nodo GruposConfiguracion
                                AgregarNodoAlPadre(pXmlDoc, NodoConfiguracionBusqueda, "GruposConfiguracion", pestanyabusqueda.GruposConfiguracion.ToString());
                            }

                            if (!string.IsNullOrEmpty(pestanyabusqueda.TextoBusquedaSinResultados))
                            {
                                // Creamos el nodo TextoBusquedaSinResultados
                                AgregarNodoAlPadre(pXmlDoc, NodoConfiguracionBusqueda, "TextoBusquedaSinResultados", pestanyabusqueda.TextoBusquedaSinResultados);
                            }

                            if (!string.IsNullOrEmpty(pestanyabusqueda.TextoDefectoBuscador))
                            {
                                // Creamos el nodo TextoDefectoBuscador
                                AgregarNodoAlPadre(pXmlDoc, NodoConfiguracionBusqueda, "TextoDefectoBuscador", pestanyabusqueda.TextoDefectoBuscador);
                            }

                            if (!pestanyabusqueda.MostrarEnComboBusqueda)
                            {
                                // Creamos el nodo MostrarCajaBusqueda
                                AgregarNodoAlPadre(pXmlDoc, NodoConfiguracionBusqueda, "MostrarEnComboBusqueda", Convert.ToInt32(pestanyabusqueda.MostrarEnComboBusqueda).ToString());
                            }

                            if (pestanyabusqueda.IgnorarPrivacidadEnBusqueda)
                            {
                                // Creamos el nodo IgnorarPrivacidadEnBusqueda
                                AgregarNodoAlPadre(pXmlDoc, NodoConfiguracionBusqueda, "IgnorarPrivacidadEnBusqueda", Convert.ToInt32(pestanyabusqueda.IgnorarPrivacidadEnBusqueda).ToString());
                            }

                            if (pestanyabusqueda.OmitirCargaInicialFacetasResultados)
                            {
                                // Creamos el nodo OmitirCargaInicialFacetasResultados
                                AgregarNodoAlPadre(pXmlDoc, NodoConfiguracionBusqueda, "OmitirCargaInicialFacetasResultados", Convert.ToInt32(pestanyabusqueda.OmitirCargaInicialFacetasResultados).ToString());
                            }
                        }

                        if (filasProyOrdRec.Count > 0)
                        {
                            // Creamos el nodo Pestanya
                            XmlElement NodoProyectoPestanyaFiltroOrdenRecursos = pXmlDoc.CreateElement("ProyectoFiltrosOrden");
                            NodoConfiguracionBusqueda.AppendChild(NodoProyectoPestanyaFiltroOrdenRecursos);

                            foreach (ProyectoPestanyaFiltroOrdenRecursos filaProyOrdRec in filasProyOrdRec)
                            {
                                PintarEstructuraXMLFiltroOrdenRecurso(pXmlDoc, NodoProyectoPestanyaFiltroOrdenRecursos, filaProyOrdRec);
                            }
                        }
                    }
                }

                #region ExportacionesBusqueda

                List<ProyectoPestanyaBusquedaExportacion> exportaciones = pExportacionBusquedaDW.ListaProyectoPestanyaBusquedaExportacion.Where(item => item.PestanyaID.Equals(pestanya.PestanyaID)).ToList();
                if (exportaciones != null && exportaciones.Count > 0)
                {
                    foreach (ProyectoPestanyaBusquedaExportacion exportacion in exportaciones)
                    {
                        PintarEstructuraXMLExportacionesBusqueda(pExportacionBusquedaDW, pXmlDoc, NodoPestanya, exportacion);
                    }
                }

                #endregion FiltroOrdenRecursos
                List<ProyectoPestanyaMenu> resultadoConsulta = pDataWrapperProyecto.ListaProyectoPestanyaMenu.Where(proy => proy.ProyectoID.Equals(pProyectoID) && proy.PestanyaPadreID.Equals(pestanya.PestanyaID)).OrderBy(proy => proy.Orden).ToList();
                if (resultadoConsulta.Count > 0)
                {
                    // Creamos el nodo PestanyasHijas
                    XmlElement NodoPestanyasHijas = pXmlDoc.CreateElement("PestanyasHijas");
                    NodoPestanya.AppendChild(NodoPestanyasHijas);

                    PintarPestanyasXML(pDataWrapperProyecto, resultadoConsulta, pXmlDoc, NodoPestanyasHijas, pProyectoID, pExportacionBusquedaDW, pFilaConfBusqueda);
                }
            }

            proyectoCN.Dispose();
            organizacionCN.Dispose();
            identidadCN.Dispose();
        }

        /// <summary>
        /// Agrega la extructura XML de una fila de filtro de orden para recursos.
        /// </summary>
        /// <param name="pXml">Documento XML</param>
        /// <param name="pNodoPestaña">Nodo Pestaña padre del elemento filtro</param>
        /// <param name="pFilaFiltroOrden">Fila de filtro de orden para recursos</param>
        private void PintarEstructuraXMLFiltroOrdenRecurso(XmlDocument pXml, XmlElement pNodoPestaña, ProyectoPestanyaFiltroOrdenRecursos pFilaFiltroOrden)
        {
            // Creamos el nodo ProyectoPestanyaFiltroOrdenRecursos
            XmlElement NodoProyectoFiltroOrdenRecursos = pXml.CreateElement("ProyectoFiltroOrden");
            pNodoPestaña.AppendChild(NodoProyectoFiltroOrdenRecursos);

            // Creamos el nodo FiltroOrden
            XmlElement NodoFiltroOrden = pXml.CreateElement("FiltroOrden");
            NodoProyectoFiltroOrdenRecursos.AppendChild(NodoFiltroOrden);
            NodoFiltroOrden.InnerText = pFilaFiltroOrden.FiltroOrden.ToString();

            // Creamos el nodo NombreFiltro
            XmlElement NodoNombreFiltro = pXml.CreateElement("NombreFiltro");
            NodoProyectoFiltroOrdenRecursos.AppendChild(NodoNombreFiltro);
            NodoNombreFiltro.InnerText = pFilaFiltroOrden.NombreFiltro;
        }

        private void PintarEstructuraXMLExportacionesBusqueda(DataWrapperExportacionBusqueda pExportacionBusquedaDW, XmlDocument pXmlDoc, XmlElement pNodoPestaña, ProyectoPestanyaBusquedaExportacion pFilaExportacion)
        {
            //Creamos el nodo ExportacionBusqueda
            XmlElement NodoExportacionBusqueda = pXmlDoc.CreateElement("ExportacionBusqueda");
            pNodoPestaña.AppendChild(NodoExportacionBusqueda);

            // Creamos el nodo NombreExportacion
            AgregarNodoAlPadre(pXmlDoc, NodoExportacionBusqueda, "NombreExportacion", pFilaExportacion.NombreExportacion.ToString());

            // Creamos el nodo OrdenExportacion
            AgregarNodoAlPadre(pXmlDoc, NodoExportacionBusqueda, "OrdenExportacion", pFilaExportacion.Orden.ToString());

            // Creamos el nodo FormatosExportacion
            AgregarNodoAlPadre(pXmlDoc, NodoExportacionBusqueda, "FormatosExportacion", pFilaExportacion.FormatosExportacion.ToString());

            string nombresGrupos = "";
            if (pFilaExportacion.GruposExportadores != null)
            {
                List<Guid> listaGruposID = new List<Guid>();
                foreach (string grupo in pFilaExportacion.GruposExportadores.Split(new char[] { ',' }))
                {
                    Guid grupoID = Guid.Empty;
                    if (Guid.TryParse(grupo, out grupoID))
                    {
                        if (!listaGruposID.Contains(grupoID))
                        {
                            listaGruposID.Add(grupoID);
                        }
                    }
                }

                IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
                DataWrapperIdentidad dataWrapperIdentidad = identCN.ObtenerGruposPorIDGrupo(listaGruposID, false);
                identCN.Dispose();
                List<Guid> listaIDsOrganizaciones = new List<Guid>();
                Dictionary<Guid, Guid> dicGrupoIDOrganizacionID = new Dictionary<Guid, Guid>();
                
                foreach (AD.EntityModel.Models.IdentidadDS.GrupoIdentidadesOrganizacion gior in dataWrapperIdentidad.ListaGrupoIdentidadesOrganizacion)
                {
                    if (!listaIDsOrganizaciones.Contains(gior.OrganizacionID))
                    {
                        listaIDsOrganizaciones.Add(gior.OrganizacionID);
                    }

                    if (!dicGrupoIDOrganizacionID.ContainsKey(gior.GrupoID))
                    {
                        dicGrupoIDOrganizacionID.Add(gior.GrupoID, gior.OrganizacionID);
                    }
                }

                OrganizacionCN organizacionCN = new OrganizacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<OrganizacionCN>(), mLoggerFactory);
                Dictionary<Guid, KeyValuePair<string, string>> dicNombresOrganizaciones = organizacionCN.ObtenerNombreOrganizacionesPorIDs(listaIDsOrganizaciones);
                organizacionCN.Dispose();
                string coma = "";

                foreach (AD.EntityModel.Models.IdentidadDS.GrupoIdentidades gir in dataWrapperIdentidad.ListaGrupoIdentidades)
                {
                    if (dicNombresOrganizaciones != null && dicGrupoIDOrganizacionID.ContainsKey(gir.GrupoID) && dicNombresOrganizaciones.ContainsKey(dicGrupoIDOrganizacionID[gir.GrupoID]))
                    {
                        nombresGrupos += coma + dicNombresOrganizaciones[dicGrupoIDOrganizacionID[gir.GrupoID]].Value + "|" + gir.NombreCorto;
                        coma = ",";
                    }
                    else
                    {
                        nombresGrupos += coma + gir.NombreCorto;
                    }
                    coma = ",";
                }

                // Creamos el nodo GruposExportadores
                AgregarNodoAlPadre(pXmlDoc, NodoExportacionBusqueda, "GruposExportadores", nombresGrupos);
            }

            //Creamos los nodos PropiedadExportacion
            List<ProyectoPestanyaBusquedaExportacionPropiedad> propiedadesExportacion = pExportacionBusquedaDW.ListaProyectoPestanyaBusquedaExportacionPropiedad.Where(item => item.ExportacionID.Equals(pFilaExportacion.ExportacionID)).ToList();
            if (propiedadesExportacion != null && propiedadesExportacion.Count > 0)
            {
                foreach (ProyectoPestanyaBusquedaExportacionPropiedad filaPropiedad in propiedadesExportacion)
                {
                    PintarEstructuraXMLPropiedadesExportacionBusqueda(pXmlDoc, NodoExportacionBusqueda, filaPropiedad);
                }
            }

            //Creamos los nodos PropiedadExportacion
            List<ProyectoPestanyaBusquedaExportacionExterna> exportacionExterna = pExportacionBusquedaDW.ListaProyectoPestanyaBusquedaExportacionExterna.Where(item => item.ExportacionID.Equals(pFilaExportacion.ExportacionID)).ToList();
            if (exportacionExterna != null && exportacionExterna.Count> 0)
            {
                string urlServicioExterno = exportacionExterna.First().UrlServicioExterno;
                // Creamos el nodo NombreExportacion
                AgregarNodoAlPadre(pXmlDoc, NodoExportacionBusqueda, "EnlaceServicioExternoExportacion", urlServicioExterno);
            }
        }

        private void PintarEstructuraXMLPropiedadesExportacionBusqueda(XmlDocument pXmlDoc, XmlElement pNodoExportacion, ProyectoPestanyaBusquedaExportacionPropiedad pFilaPropiedad)
        {
            //Creamos el nodo ExportacionBusqueda
            XmlElement NodoPropiedadExportacion = pXmlDoc.CreateElement("PropiedadExportacion");
            pNodoExportacion.AppendChild(NodoPropiedadExportacion);

            // Creamos el nodo OrdenPropiedad
            AgregarNodoAlPadre(pXmlDoc, NodoPropiedadExportacion, "OrdenPropiedad", pFilaPropiedad.Orden.ToString());

            // Creamos el nodo Ontologia
            XmlElement NodoOntologia = pXmlDoc.CreateElement("Ontologia");
            NodoPropiedadExportacion.AppendChild(NodoOntologia);

            string nombreOntologia = string.Empty;

            if (!string.IsNullOrEmpty(pFilaPropiedad.Ontologia))
            {
                int indiceOWL = pFilaPropiedad.Ontologia.IndexOf(".owl");
                int indiceUltimaBarra = pFilaPropiedad.Ontologia.LastIndexOf("/");
                nombreOntologia = pFilaPropiedad.Ontologia.Substring(indiceUltimaBarra + 1, indiceOWL - indiceUltimaBarra - 1);
            }

            NodoOntologia.InnerText = nombreOntologia;

            // Creamos el nodo Ontologia
            AgregarNodoAlPadre(pXmlDoc, NodoPropiedadExportacion, "Propiedad", pFilaPropiedad.Propiedad.ToString(), true);

            // Creamos el nodo NombrePropiedad
            AgregarNodoAlPadre(pXmlDoc, NodoPropiedadExportacion, "NombrePropiedad", pFilaPropiedad.NombrePropiedad.ToString(), true);

            if (pFilaPropiedad.DatosExtraPropiedad != null)
            {
                // Creamos el nodo DatosExtraPropiedad
                AgregarNodoAlPadre(pXmlDoc, NodoPropiedadExportacion, "DatosExtraPropiedad", pFilaPropiedad.DatosExtraPropiedad.ToString(), true);
            }
        }

        #endregion PintarEstructuraXMLPestanyas

        #region PintarEstructuraXMLComparticionAutomatica

        private void PintarEstructuraXMLComparticionAutomatica(XmlDocument pXmlDoc, XmlElement pNodoComunidad, DataWrapperComparticionAutomatica pCompAutoDW, Guid pProyectoID)
        {
            List<AD.EntityModel.Models.ComparticionAutomatica.ComparticionAutomatica> filasCompAuto = pCompAutoDW.ListaComparticionAutomatica.Where(item => item.ProyectoOrigenID.Equals(pProyectoID)).ToList();

            if (filasCompAuto.Count > 0)
            {
                // Creamos el nodo ComparticionAutomatica
                XmlElement NodoComparticionAutomatica = pXmlDoc.CreateElement("ComparticionAutomatica");
                pNodoComunidad.AppendChild(NodoComparticionAutomatica);

                ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
                foreach (AD.EntityModel.Models.ComparticionAutomatica.ComparticionAutomatica filaC in filasCompAuto)
                {
                    string comDestino = proyectoCN.ObtenerNombreCortoProyecto(filaC.ProyectoDestinoID);

                    // Creamos el nodo ComparticionEnProyecto
                    XmlElement NodoComparticionEnProyecto = pXmlDoc.CreateElement("ComparticionEnProyecto");
                    NodoComparticionAutomatica.AppendChild(NodoComparticionEnProyecto);

                    // Creamos el nodo NombreComparticion
                    AgregarNodoAlPadre(pXmlDoc, NodoComparticionEnProyecto, "NombreComparticion", filaC.Nombre);

                    //como va a ser en todas la misma identidad puedo coger el primero
                    Guid identidadPublicadora = filaC.IdentidadPublicadoraID;
                    string publicador = string.Empty;

                    if (identidadPublicadora != Guid.Empty)
                    {
                        IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
                        publicador = identidadCN.ObtenerNombreCortoIdentidad(identidadPublicadora);
                        identidadCN.Dispose();

                        //if (CrearFilasPropiedadesExportacion)
                        //{
                        //    //Crear las filas de las porpiedades de Integracion Continua
                        //    IntegracionContinuaPropiedad propiedadMapping = new IntegracionContinuaPropiedad();
                        //    propiedadMapping.ProyectoID = ProyectoID;
                        //    propiedadMapping.TipoObjeto = (short)TipoObjeto.Comparticion;
                        //    propiedadMapping.ObjetoPropiedad = filaC.Nombre;
                        //    propiedadMapping.TipoPropiedad = (short)TipoPropiedad.IdentidadComparticion;
                        //    propiedadMapping.ValorPropiedad = publicador;
                        //    propiedadesIntegracionContinua.Add(propiedadMapping);
                        //}
                    }
                    else
                    {
                        publicador = "###publicador###";
                    }

                    // Creamos el nodo NombreComparticion
                    AgregarNodoAlPadre(pXmlDoc, NodoComparticionEnProyecto, "Publicador", publicador);

                    // Creamos el nodo ComunidadDestino
                    AgregarNodoAlPadre(pXmlDoc, NodoComparticionEnProyecto, "ComunidadDestino", comDestino);

                    // Creamos el nodo ActualizarHome
                    AgregarNodoAlPadre(pXmlDoc, NodoComparticionEnProyecto, "ActualizarHome", Convert.ToInt32(filaC.ActualizarHome).ToString());

                    List<AD.EntityModel.Models.ComparticionAutomatica.ComparticionAutomaticaReglas> filasCompAutoReglas = pCompAutoDW.ListaComparticionAutomaticaReglas.Where(item => item.ComparticionID.Equals(filaC.ComparticionID)).ToList();

                    if (filasCompAutoReglas.Count > 0)
                    {
                        // Creamos el nodo ReglaComparticion
                        XmlElement NodoReglaComparticion = pXmlDoc.CreateElement("ReglaComparticion");
                        NodoComparticionEnProyecto.AppendChild(NodoReglaComparticion);

                        foreach (AD.EntityModel.Models.ComparticionAutomatica.ComparticionAutomaticaReglas filaReglas in filasCompAutoReglas)
                        {
                            // Creamos el nodo Regla
                            AgregarNodoAlPadre(pXmlDoc, NodoReglaComparticion, "Regla", filaReglas.Regla);
                        }

                        // Creamos el nodo Navegacion
                        AgregarNodoAlPadre(pXmlDoc, NodoReglaComparticion, "Navegacion", filasCompAutoReglas[0].Navegacion);
                    }

                    List<AD.EntityModel.Models.ComparticionAutomatica.ComparticionAutomaticaMapping> filasCompAutoMapping = pCompAutoDW.ListaComparticionAutomaticaMapping.Where(item => item.ComparticionID.Equals(filaC.ComparticionID)).OrderBy(item => item.GrupoMapping).ToList();

                    if (filasCompAutoMapping.Count > 0 || (identidadPublicadora.Equals(Guid.Empty) && filasCompAutoMapping.Count == 0))
                    {
                        List<string> listaCategorias = new List<string>();
                        string catDestino = string.Empty;

                        Dictionary<int, XmlElement> listaGrruposMapping = new Dictionary<int, XmlElement>();

                        // Creamos el nodo MappingCategorias
                        XmlElement NodoMappingCategorias = pXmlDoc.CreateElement("MappingCategorias");
                        NodoComparticionEnProyecto.AppendChild(NodoMappingCategorias);

                        if (filasCompAutoMapping.Count > 0)
                        {
                            TesauroCN tesauroCN = new TesauroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<TesauroCN>(), mLoggerFactory);

                            foreach (AD.EntityModel.Models.ComparticionAutomatica.ComparticionAutomaticaMapping filaMaping in filasCompAutoMapping)
                            {
                                //catDestino = tesauroCN.ObtenerNombreCategoriaPorID(filaMaping.CategoriaTesauroID, UtilIdiomas.LanguageCode);
                                //comentada la anterior porque se quiere descargar en multiidioma
                                catDestino = tesauroCN.ObtenerNombreCategoriaPorID(filaMaping.CategoriaTesauroID, string.Empty);

                                XmlElement grupoMaping = null;
                                if (listaGrruposMapping.ContainsKey(filaMaping.GrupoMapping))
                                {
                                    grupoMaping = listaGrruposMapping[filaMaping.GrupoMapping];
                                }
                                else
                                {
                                    grupoMaping = pXmlDoc.CreateElement("Mapping");
                                    NodoMappingCategorias.AppendChild(grupoMaping);
                                }

                                // Creamos el nodo CatDestino
                                AgregarNodoAlPadre(pXmlDoc, grupoMaping, "CatDestino", catDestino);

                                // Creamos el nodo ReglaOrigen
                                AgregarNodoAlPadre(pXmlDoc, grupoMaping, "ReglaOrigen", filaMaping.ReglaMapping);

                                //if (CrearFilasPropiedadesExportacion)
                                //{
                                //    //Crear las filas de las porpiedades de Integracion Continua
                                //    IntegracionContinuaPropiedad propiedadMapping = new IntegracionContinuaPropiedad();
                                //    propiedadMapping.ProyectoID = ProyectoID;
                                //    propiedadMapping.TipoObjeto = (short)TipoObjeto.Comparticion;
                                //    propiedadMapping.ObjetoPropiedad = filaC.Nombre;
                                //    propiedadMapping.TipoPropiedad = (short)TipoPropiedad.MappingComparticion;
                                //    propiedadMapping.ValorPropiedad = filaMaping.ReglaMapping + "$$$" + catDestino;
                                //    propiedadesIntegracionContinua.Add(propiedadMapping);
                                //}
                            }

                            tesauroCN.Dispose();
                        }
                        else
                        {
                            XmlElement grupoMaping = pXmlDoc.CreateElement("Mapping");
                            NodoMappingCategorias.AppendChild(grupoMaping);

                            // Creamos el nodo CatDestino
                            AgregarNodoAlPadre(pXmlDoc, grupoMaping, "CatDestino", "###categoria###");

                            // Creamos el nodo ReglaOrigen
                            AgregarNodoAlPadre(pXmlDoc, grupoMaping, "ReglaOrigen", "Todas");
                        }
                    }
                }

                proyectoCN.Dispose();
            }
        }

        #endregion PintarEstructuraXMLComparticionAutomatica

        private void AgregarNodosConfigFacetadoProyMapa(XmlDocument pXmlDoc, XmlElement pNodoConfigFacetadoProyMapa, KeyValuePair<string, string> pPropLatitud, KeyValuePair<string, string> pPropLongitud, KeyValuePair<string, string> pPropRuta, KeyValuePair<string, string> pColorRuta = new KeyValuePair<string, string>())
        {
            // Creamos el nodo PropLatitud
            AgregarNodoAlPadre(pXmlDoc, pNodoConfigFacetadoProyMapa, pPropLatitud.Key, pPropLatitud.Value);

            // Creamos el nodo PropLongitud
            AgregarNodoAlPadre(pXmlDoc, pNodoConfigFacetadoProyMapa, pPropLongitud.Key, pPropLongitud.Value);

            // Creamos el nodo PropRuta
            AgregarNodoAlPadre(pXmlDoc, pNodoConfigFacetadoProyMapa, pPropRuta.Key, pPropRuta.Value);

            if (!string.IsNullOrEmpty(pColorRuta.Key))
            {
                // Creamos el nodo ColorRuta
                AgregarNodoAlPadre(pXmlDoc, pNodoConfigFacetadoProyMapa, pColorRuta.Key, pColorRuta.Value);
            }
        }

        public void AgregarNodoAlPadre(XmlDocument pXmlDocument, XmlElement pNodoPadre, string pNodoKey, string pNodoValue, bool pUsarCDATA = false, bool pInsertarVacio = true)
        {
            if (pInsertarVacio || !string.IsNullOrEmpty(pNodoValue))
            {
                XmlElement nodo = pXmlDocument.CreateElement(pNodoKey);
                pNodoPadre.AppendChild(nodo);

                if (pUsarCDATA)
                {
                    pNodoValue = "<![CDATA[" + pNodoValue + "]]>";
                    nodo.InnerXml = pNodoValue;
                }
                else
                {
                    nodo.InnerText = pNodoValue;
                }
            }
        }

        #endregion Metodos

        #region Propiedades

        public Guid OrganizacionID
        {
            get { return mOrganizacionID; }
            private set { mOrganizacionID = value; }
        }

        public Guid ProyectoID
        {
            get { return mProyectoID; }
            private set { mProyectoID = value; }
        }

        /// <summary>
        /// Parámetros de un proyecto.
        /// </summary>
        public Dictionary<string, string> ParametroProyecto
        {
            get
            {
                if (mParametroProyecto == null)
                {
                    ProyectoCL proyectoCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);
                    mParametroProyecto = proyectoCL.ObtenerParametrosProyecto(ProyectoID);
                    proyectoCL.Dispose();
                }

                return mParametroProyecto;
            }
        }

        public DataWrapperProyecto DataWrapperProyecto
        {
            get
            {
                if (mDataWrapperProyecto == null)
                {
                    ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
                    mDataWrapperProyecto = proyectoCN.ObtenerProyectoPorID(ProyectoID);
                    mDataWrapperProyecto.Merge(proyectoCN.ObtenerSeccionesHomeCatalogoDeProyecto(ProyectoID));
                    mDataWrapperProyecto.Merge(proyectoCN.ObtenerPresentacionSemantico(ProyectoID));
                    mDataWrapperProyecto.Merge(proyectoCN.ObtenerFiltrosOrdenesDeProyecto(ProyectoID));
                    mDataWrapperProyecto.Merge(proyectoCN.ObtenerRecursosRelacionadosPresentacion(ProyectoID));
                    mDataWrapperProyecto.Merge(proyectoCN.ObtenerTesaurosSemanticosConfigEdicionDeProyecto(ProyectoID));
                    proyectoCN.ObtenerGadgetsProyecto(ProyectoID, mDataWrapperProyecto);
                    proyectoCN.ObtenerPestanyasProyecto(ProyectoID, mDataWrapperProyecto);
                    proyectoCN.Dispose();
                }

                return mDataWrapperProyecto;
            }
        }

        public DataWrapperFacetas FacetaDW
        {
            get
            {
                if (mFacetaDW == null)
                {
                    FacetaCN facetaCN = new FacetaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetaCN>(), mLoggerFactory);
                    mFacetaDW = facetaCN.ObtenerTodasFacetasDeProyecto(OrganizacionID, ProyectoID);
                    facetaCN.Dispose();
                }

                return mFacetaDW;
            }
        }

        public Proyecto FilaProyecto
        {
            get
            {
                if (mFilaProyecto == null)
                {
                    mFilaProyecto = DataWrapperProyecto.ListaProyecto.FirstOrDefault(proyecto=>proyecto.OrganizacionID.Equals(OrganizacionID) && proyecto.ProyectoID.Equals(ProyectoID));
                }

                return mFilaProyecto;
            }
        }

        public ConfiguracionAmbitoBusquedaProyecto FilaConfiguracionAmbitoBusqueda
        {
            get
            {
                if (mFilaConfiguracionAmbitoBusqueda == null)
                {
                    //mFilaConfiguracionAmbitoBusqueda = ParametroGeneralDS.ConfiguracionAmbitoBusquedaProyecto.FindByOrganizacionIDProyectoID(FilaProyecto.OrganizacionID, FilaProyecto.ProyectoID);
                    mFilaConfiguracionAmbitoBusqueda = ParametroGeneralDS.ListaConfiguracionAmbitoBusquedaProyecto.Find(configuracion => configuracion.OrganizacionID.Equals(FilaProyecto.OrganizacionID) && configuracion.ProyectoID.Equals(FilaProyecto.ProyectoID));

                }

                return mFilaConfiguracionAmbitoBusqueda;
            }
        }

        public ParametroGeneral FilaParametroGeneral
        {
            get
            {
                if (mFilaParametroGeneral == null)
                {
                    mFilaParametroGeneral = ParametroGeneralDS.ListaParametroGeneral.Find(parametrosGeneral=>parametrosGeneral.OrganizacionID.Equals(OrganizacionID) && parametrosGeneral.ProyectoID.Equals(ProyectoID));
                    //mFilaParametroGeneral = ParametroGeneralDS.ParametroGeneral.FindByOrganizacionIDProyectoID(OrganizacionID, ProyectoID);
                }

                return mFilaParametroGeneral;
            }
        }

        public GestorParametroGeneral ParametroGeneralDS
        {
            get
            {
                if (mParametroGeneralDS == null)
                {
                    //ParametroGeneralCN parametroGralCN = new ParametroGeneralCN();
                    ParametroGeneralGBD parametroGeneralController = new ParametroGeneralGBD(mEntityContext);
                    mParametroGeneralDS = new GestorParametroGeneral();
                    mParametroGeneralDS=parametroGeneralController.ObtenerParametrosGeneralesDeProyecto(mParametroGeneralDS, ProyectoID);
                   // mParametroGeneralDS = parametroGralCN.ObtenerParametrosGeneralesDeProyecto(ProyectoID);
                    //parametroGralCN.Dispose();
                }

                return mParametroGeneralDS;
            }
        }

        public DataWrapperExportacionBusqueda ExportacionBusquedaDW
        {
            get
            {
                if (mExportacionBusquedaDW == null)
                {
                    ExportacionBusquedaCN exportacionBusquedaCN = new ExportacionBusquedaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ExportacionBusquedaCN>(), mLoggerFactory);
                    mExportacionBusquedaDW = exportacionBusquedaCN.ObtenerExportacionesProyecto(ProyectoID);
                    exportacionBusquedaCN.Dispose();
                }

                return mExportacionBusquedaDW;
            }
        }

        public DataWrapperComparticionAutomatica ComparticionAutomaticaDW
        {
            get
            {
                if (mComparticionAutomaticaDW == null)
                {
                    ComparticionAutomaticaCN comparticionCN = new ComparticionAutomaticaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ComparticionAutomaticaCN>(), mLoggerFactory);
                    mComparticionAutomaticaDW = comparticionCN.ObtenerComparticionProyectoPorProyectoID(OrganizacionID, ProyectoID, false);
                    comparticionCN.Dispose();
                }

                return mComparticionAutomaticaDW;
            }
        }

        private DataWrapperVistaVirtual VistaVirtualDW
        {
            get
            {
                if(mVistaVirtualDW == null)
                {
                    VistaVirtualCN vistaVirtualCN = new VistaVirtualCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<VistaVirtualCN>(), mLoggerFactory);
                    mVistaVirtualDW = vistaVirtualCN.ObtenerVistasVirtualPorProyectoID(ProyectoID);
                }

                return mVistaVirtualDW;
            }
        }

        #endregion Propiedades
    }
}
