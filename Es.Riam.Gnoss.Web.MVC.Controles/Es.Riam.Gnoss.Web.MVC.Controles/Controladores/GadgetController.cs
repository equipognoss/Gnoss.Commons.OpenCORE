using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.ParametroGeneralDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Facetado;
using Es.Riam.Gnoss.AD.Facetado.Model;
using Es.Riam.Gnoss.AD.MetaBuscadorAD;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Usuarios;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.Documentacion;
using Es.Riam.Gnoss.CL.Facetado;
using Es.Riam.Gnoss.CL.ParametrosAplicacion;
using Es.Riam.Gnoss.CL.ParametrosProyecto;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos;
using Es.Riam.Gnoss.Elementos.Amigos;
using Es.Riam.Gnoss.Elementos.Documentacion;
using Es.Riam.Gnoss.Elementos.Facetado;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Elementos.ParametroGeneralDSEspacio;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.BASE_BD;
using Es.Riam.Gnoss.Logica.Documentacion;
using Es.Riam.Gnoss.Logica.Facetado;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.MVC;
using Es.Riam.Gnoss.Logica.ParametroAplicacion;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Recursos;
using Es.Riam.Gnoss.Servicios;
using Es.Riam.Gnoss.Servicios.ControladoresServiciosWeb;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.UtilServiciosWeb;
using Es.Riam.Gnoss.Web.Controles.Proyectos;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.Semantica.OWL;
using Es.Riam.Semantica.Plantillas;
using Es.Riam.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;

namespace Es.Riam.Gnoss.Web.MVC.Controles.Controladores
{
    public class GadgetController : Web.Controles.ControladorBase
    {
        private readonly ControllerBaseGnoss ControllerBase;
        private Documento Documento = null;
        private SemCmsController GenPlantillasOWL = null;
        private string NombreSemPestanya = "";
        private GestionFacetas mGestorFacetas = null;
        private readonly Identidad mIdentidadUsuarioActual = null;

        private readonly EntityContextBASE mEntityContextBASE;
        private readonly ICompositeViewEngine mViewEngine;
        private readonly IUtilServicioIntegracionContinua mUtilServicioIntegracionContinua;
        private IAvailableServices mAvailableServices;
        Microsoft.AspNetCore.Hosting.IHostingEnvironment mEnv;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;

        /// <summary>
        /// Constructor a partir de la página que contiene al controlador
        /// </summary>
        /// <param name="pController">Controller</param>
        public GadgetController(ControllerBaseGnoss pController, IHttpContextAccessor httpContextAccessor, LoggingService loggingService, GnossCache gnossCache, ConfigService configService, VirtuosoAD virtuosoAD, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, EntityContextBASE entityContextBASE, ICompositeViewEngine viewEngine, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IAvailableServices availableServices, ILogger<GadgetController> logger, ILoggerFactory loggerFactory)
            : this(pController, null, httpContextAccessor, loggingService, gnossCache, configService, virtuosoAD, entityContext, redisCacheWrapper, entityContextBASE, viewEngine, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication,logger,loggerFactory)
        {
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            mEnv = env;
            mAvailableServices = availableServices;
            mIdentidadUsuarioActual = IdentidadActual;
        }

        /// <summary>
        /// Constructor a partir de la página que contiene al controlador
        /// </summary>
        /// <param name="pController">Controller</param>
        public GadgetController(ControllerBaseGnoss pController, Identidad pIdentidadUsuarioActual, IHttpContextAccessor httpContextAccessor, LoggingService loggingService, GnossCache gnossCache, ConfigService configService, VirtuosoAD virtuosoAD, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, EntityContextBASE entityContextBASE, ICompositeViewEngine viewEngine, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<GadgetController> logger, ILoggerFactory loggerFactory)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, servicesUtilVirtuosoAndReplication,logger,loggerFactory)
        {
            ControllerBase = pController;
            mIdentidadUsuarioActual = pIdentidadUsuarioActual;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            mEntityContextBASE = entityContextBASE;
            mViewEngine = viewEngine;
            mUtilServicioIntegracionContinua = utilServicioIntegracionContinua;
        }

        public List<GadgetModel> CargarListaGadgetsHome(bool pPrimeraPeticion, List<Guid> pListaGadgetsSinCargar)
        {
            return CargarListaGadgets(pPrimeraPeticion, pListaGadgetsSinCargar, TipoUbicacionGadget.LateralHomeComunidad);
        }

        public List<GadgetModel> CargarListaGadgetsRecurso(bool pPrimeraPeticion, List<Guid> pListaGadgetsSinCargar, Documento pDocumento, SemCmsController pGenPlantillasOWL, string pNombreSemPestanya, bool pUsarAfinidad = false)
        {
            Documento = pDocumento;
            GenPlantillasOWL = pGenPlantillasOWL;
            NombreSemPestanya = pNombreSemPestanya;

            return CargarListaGadgets(pPrimeraPeticion, pListaGadgetsSinCargar, TipoUbicacionGadget.FichaRecursoComunidad, pUsarAfinidad);
        }

        public List<GadgetModel> CargarListaGadgetsFichaPerfil(Guid pPerfilID)
        {
            return CargarListaGadgetsPerfil(pPerfilID);
        }

        public ActionResult CargarGadgetHome(Guid pGadgetID, int pNumPagina)
        {
            return CargarGadget(pGadgetID, pNumPagina, TipoUbicacionGadget.LateralHomeComunidad);
        }

        public ActionResult CargarGadgetRecurso(Guid pGadgetID, int pNumPagina, Documento pDocumento, SemCmsController pGenPlantillasOWL, string pNombreSemPestanya)
        {
            Documento = pDocumento;
            GenPlantillasOWL = pGenPlantillasOWL;
            NombreSemPestanya = pNombreSemPestanya;

            return CargarGadget(pGadgetID, pNumPagina, TipoUbicacionGadget.FichaRecursoComunidad);
        }

        private ActionResult CargarGadget(Guid pGadgetID, int pNumPagina, TipoUbicacionGadget pTipoUbicacionGadget)
        {
            ProyectoCL proyectoCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);
            DataWrapperProyecto proyectoDataWrapper = proyectoCL.ObtenerGadgetsProyecto(ControllerBase.ProyectoSeleccionado.Clave);
            proyectoCL.Dispose();

            if (proyectoDataWrapper.ListaProyectoGadget.Count > 0)
            {
                List<ProyectoGadget> filasProyectoGadget = null;

                filasProyectoGadget = proyectoDataWrapper.ListaProyectoGadget.Where(proyectoGadget => proyectoGadget.TipoUbicacion.Equals((short)pTipoUbicacionGadget) && proyectoGadget.Visible == true && proyectoGadget.GadgetID.Equals(pGadgetID)).OrderBy(proyectoGadget => proyectoGadget.Orden).ToList();

                if (filasProyectoGadget.Count > 0)
                {
                    //Comprobamos si debe mostrarse el gadget en función de ComunidadDestinoFiltros
                    string FiltrosDestino = filasProyectoGadget.First().ComunidadDestinoFiltros;
                    bool mostrar = MostrarGadget(FiltrosDestino);
                    if (mostrar)
                    {
                        MultiViewResult result = new MultiViewResult(ControllerBase, mViewEngine);

                        if (filasProyectoGadget.First().Tipo == (short)TipoGadget.RecursosRelacionados)
                        {
                            List<Guid> listaRecursosID = new List<Guid>();
                            int numElementos = ControllerBase.ParametrosGeneralesRow.NumeroRecursosRelacionados;
                            listaRecursosID = MontarRecursosRelacionados(pGadgetID, numElementos, pNumPagina, false);

                            if (listaRecursosID.Count > 0)
                            {
                                MVCCN mvcCN = new MVCCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<MVCCN>(), mLoggerFactory);

                                string baseUrlBusqueda = UrlsSemanticas.ObtenerURLComunidad(ControllerBase.UtilIdiomas, ControllerBase.BaseURLIdioma, ControllerBase.ProyectoSeleccionado.NombreCorto) + "/" + ControllerBase.UtilIdiomas.GetText("URLSEM", "BUSQUEDAAVANZADA");

                                List<string> listaBaseURLContent = new List<string>();
                                listaBaseURLContent.Add(ControllerBase.BaseURLContent);
                                ControladorProyectoMVC controladorProyectoMVC = new ControladorProyectoMVC(ControllerBase.UtilIdiomas, ControllerBase.BaseURL, listaBaseURLContent, ControllerBase.BaseURLStatic, ControllerBase.ProyectoSeleccionado, Guid.Empty, ControllerBase.ParametrosGeneralesRow, mIdentidadUsuarioActual, EsBot, mLoggingService, mEntityContext, mConfigService, mHttpContextAccessor, mRedisCacheWrapper, mVirtuosoAD, mGnossCache, mEntityContextBASE, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorProyectoMVC>(), mLoggerFactory);

                                Dictionary<Guid, ResourceModel> listaFichasRecursos = controladorProyectoMVC.ObtenerRecursosPorID(listaRecursosID, baseUrlBusqueda, null, false, pObtenerUltimaVersion: true);

                                string nombreVistaRecursos = "ControlesMVC/_FichaRecursoGadget";
                                if (filasProyectoGadget.First().PersonalizacionComponenteID != null)
                                {
                                    nombreVistaRecursos = filasProyectoGadget[0].PersonalizacionComponenteID.ToString();
                                }

                                foreach (Guid docID in listaRecursosID)
                                {
                                    result.AddView(nombreVistaRecursos, "FichaRecursoMini_" + docID, listaFichasRecursos[docID]);
                                }
                            }
                        }
                        else if (filasProyectoGadget[0].Tipo == (short)TipoGadget.RecursosContextos)
                        {
                            string nombreCorto = filasProyectoGadget[0].NombreCorto;
                            if (string.IsNullOrEmpty(nombreCorto))
                            {
                                nombreCorto = filasProyectoGadget[0].GadgetID.ToString();
                            }

                            ProyectoGadgetContexto filasProyectoGadgetContexto = filasProyectoGadget.First().ProyectoGadgetContexto;

                            KeyValuePair<string, List<ResourceModel>> listaFichasContexto = CargarRecursosContexto(filasProyectoGadgetContexto, nombreCorto, pNumPagina, false);
                            string nombreVistaRecursos = "ControlesMVC/_FichaRecursoGadget";
                            if (filasProyectoGadget.First().PersonalizacionComponenteID.HasValue)
                            {
                                nombreVistaRecursos = filasProyectoGadget[0].PersonalizacionComponenteID.ToString();
                            }

                            foreach (ResourceModel fichaRecurso in listaFichasContexto.Value)
                            {
                                result.AddView(nombreVistaRecursos, "FichaRecursoMini_" + fichaRecurso.Key, fichaRecurso);
                            }
                        }

                        return result;
                    }
                }
            }

            return new EmptyResult();
        }


        /// <summary>
        /// Montamos los Gadgets en la comunidad.
        /// </summary>
        /// <param name="pNumPeticion">Número de petición actual</param>
        /// <returns>TRUE si hay más gadget para pintar, FALSE en caso contrario</returns>
        private List<GadgetModel> CargarListaGadgets(bool pPrimeraPeticion, List<Guid> pListaGadgetsSinCargar, TipoUbicacionGadget pTipoUbicacionGadget, bool pUsarAfinidad = false)
        {
            List<GadgetModel> listaGadgets = new List<GadgetModel>();
            Dictionary<Guid, List<Guid>> listaRecursosGadgets = new Dictionary<Guid, List<Guid>>();
            Dictionary<Guid, List<Guid>> listaRecursosGadgetsPaginador = new Dictionary<Guid, List<Guid>>();
            List<Guid> listaEncuestas = new List<Guid>();

            string javascript = "";

            ProyectoCL proyectoCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);
            DataWrapperProyecto dataWrapperProyecto = proyectoCL.ObtenerGadgetsProyecto(ControllerBase.ProyectoSeleccionado.Clave);
            proyectoCL.Dispose();

            if (dataWrapperProyecto.ListaProyectoGadget.Count > 0)
            {
                List<ProyectoGadget> filasProyectoGadget = null;

                if (pPrimeraPeticion)
                {
                    filasProyectoGadget = dataWrapperProyecto.ListaProyectoGadget.Where(proyectoGadget => proyectoGadget.TipoUbicacion.Equals((short)pTipoUbicacionGadget) && proyectoGadget.Visible).OrderBy(proyectoGadget => proyectoGadget.Orden).ToList();
                }
                else
                {
                    filasProyectoGadget = dataWrapperProyecto.ListaProyectoGadget.Where(proyectoGadget => proyectoGadget.Tipo != (short)TipoGadget.HtmlPrincipalComunidad && proyectoGadget.Tipo != (short)TipoGadget.HtmlIncrustado && proyectoGadget.TipoUbicacion.Equals((short)pTipoUbicacionGadget) && proyectoGadget.Visible && pListaGadgetsSinCargar.Contains(proyectoGadget.GadgetID)).OrderBy(proyectoGadget => proyectoGadget.Orden).ToList();

                    //filasProyectoGadget = dataWrapperProyecto.ListaProyectoGadget.Where(proyectoGadget => proyectoGadget.Tipo != (short)TipoGadget.HtmlPrincipalComunidad && proyectoGadget.Tipo != (short)TipoGadget.HtmlIncrustado && proyectoGadget.TipoUbicacion.Equals((short)pTipoUbicacionGadget) && proyectoGadget.Visible == true).OrderBy(proyectoGadget => proyectoGadget.Orden).ToList();
                }

                foreach (ProyectoGadget filaProyectoGadget in filasProyectoGadget)
                {
                    if (pListaGadgetsSinCargar == null || pListaGadgetsSinCargar.Contains(filaProyectoGadget.GadgetID))
                    {
                        //Comprobamos si debe mostrarse el gadget en función de ComunidadDestinoFiltros
                        string FiltrosDestino = filaProyectoGadget.ComunidadDestinoFiltros;
                        bool mostrar = MostrarGadget(FiltrosDestino);
                        if (mostrar)
                        {
                            if (filaProyectoGadget.Tipo == (short)TipoGadget.CMS)
                            {
                                Guid componenteID;
                                if (!string.IsNullOrEmpty(filaProyectoGadget.Contenido) && Guid.TryParse(filaProyectoGadget.Contenido, out componenteID) && !componenteID.Equals(Guid.Empty))
                                {
                                    GadgetCMSModel gadget = new GadgetCMSModel();
                                    gadget.Key = filaProyectoGadget.GadgetID;
                                    gadget.Title = UtilCadenas.ObtenerTextoDeIdioma(filaProyectoGadget.Titulo, ControllerBase.UtilIdiomas.LanguageCode, ControllerBase.ParametrosGeneralesRow.IdiomaDefecto);
                                    gadget.ClassName = filaProyectoGadget.Clases;

                                    string nombreCorto = filaProyectoGadget.NombreCorto;
                                    if (string.IsNullOrEmpty(nombreCorto))
                                    {
                                        nombreCorto = filaProyectoGadget.GadgetID.ToString();
                                    }
                                    gadget.ShortName = nombreCorto;

                                    gadget.Order = filaProyectoGadget.Orden;

                                    if (!filaProyectoGadget.CargarPorAjax || !pPrimeraPeticion)
                                    {
                                        ControladorCMS controladorCMS = new ControladorCMS(this.ControllerBase, componenteID, null, this.ControllerBase.Comunidad, mHttpContextAccessor, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mViewEngine, mUtilServicioIntegracionContinua, mServicesUtilVirtuosoAndReplication, mEnv, true, mLoggerFactory.CreateLogger<ControladorCMS>(), mLoggerFactory);
                                        gadget.CMSComponent = controladorCMS.CargarComponente(true, false, ControllerBase.ParametrosGeneralesRow.IdiomaDefecto);
                                    }
                                    listaGadgets.Add(gadget);
                                }
                            }

                            if (filaProyectoGadget.Tipo == (short)TipoGadget.HtmlIncrustado)
                            {
                                #region HTML incrustado

                                string contenido = filaProyectoGadget.Contenido;
                                if (filaProyectoGadget.MultiIdioma)
                                {
                                    foreach (ProyectoGadgetIdioma fila in filaProyectoGadget.ProyectoGadgetIdioma)
                                    {
                                        if (fila.Idioma == ControllerBase.UtilIdiomas.LanguageCode)
                                        {
                                            contenido = fila.Contenido;
                                            break;
                                        }
                                        else if (fila.Idioma == ControllerBase.ParametrosGeneralesRow.IdiomaDefecto)
                                        {
                                            contenido = fila.Contenido;
                                        }
                                        else if (string.IsNullOrEmpty(contenido))
                                        {
                                            contenido = fila.Contenido;
                                        }
                                    }
                                }

                                //Indica si no existe la pripiedad/es en el caso de los doc semanticos
                                bool errorSemantico = false;

                                if (Documento != null)
                                {
                                    string inicioMarca = "@propsem-";
                                    string finMarca = "@";
                                    if (Documento.TipoDocumentacion == TiposDocumentacion.Semantico)
                                    {
                                        while (contenido.Contains(inicioMarca) && !errorSemantico)
                                        {
                                            int posicionInicioMarca = filaProyectoGadget.Contenido.IndexOf(inicioMarca);
                                            int posicionFinMarca = filaProyectoGadget.Contenido.IndexOf(finMarca, posicionInicioMarca + 1);

                                            string nombrepropiedad = filaProyectoGadget.Contenido.Substring(posicionInicioMarca + inicioMarca.Length, posicionFinMarca - posicionInicioMarca - inicioMarca.Length);

                                            ElementoOntologia eleOnto = GenPlantillasOWL.Entidades.First();
                                            if(eleOnto != null)
                                            {
                                                Propiedad propiedad = EstiloPlantilla.ObtenerPropiedadACualquierNivelPorNombre(nombrepropiedad, eleOnto);

                                                if (propiedad != null)
                                                {
                                                    if (propiedad.FunctionalProperty)
                                                    {
                                                        contenido = contenido.Replace(inicioMarca + nombrepropiedad + finMarca, propiedad.UnicoValor.Key);
                                                    }
                                                    else
                                                    {

                                                        contenido = contenido.Replace(inicioMarca + nombrepropiedad + finMarca, propiedad.PrimerValorPropiedad);
                                                    }
                                                }
                                                else
                                                {
                                                    errorSemantico = true;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (contenido.Contains(inicioMarca))
                                        {
                                            errorSemantico = true;
                                        }
                                    }
                                }

                                if (!errorSemantico)
                                {
                                    GadgetHtmlModel gadget = new GadgetHtmlModel();
                                    gadget.Key = filaProyectoGadget.GadgetID;
                                    gadget.Title = UtilCadenas.ObtenerTextoDeIdioma(filaProyectoGadget.Titulo, ControllerBase.UtilIdiomas.LanguageCode, ControllerBase.ParametrosGeneralesRow.IdiomaDefecto);
                                    gadget.ClassName = filaProyectoGadget.Clases;

                                    string nombreCorto = filaProyectoGadget.NombreCorto;
                                    if (string.IsNullOrEmpty(nombreCorto))
                                    {
                                        nombreCorto = filaProyectoGadget.GadgetID.ToString();
                                    }
                                    gadget.ShortName = nombreCorto;

                                    gadget.Order = filaProyectoGadget.Orden;

                                    if (contenido.IndexOf("[javascript]") == 0)
                                    {
                                        contenido = contenido.Substring(("[javascript]").Length);
                                        javascript += contenido.Substring(0, contenido.IndexOf("[javascript]"));
                                        contenido = contenido.Substring(contenido.IndexOf("[javascript]") + ("[javascript]").Length);
                                    }
                                    gadget.Html = contenido;

                                    listaGadgets.Add(gadget);
                                }
                                #endregion
                            }
                            else if (filaProyectoGadget.Tipo == (short)TipoGadget.RecursosRelacionados)
                            {
                                GadgetResourceListModel gadget = new GadgetResourceListModel();
                                gadget.Key = filaProyectoGadget.GadgetID;
                                gadget.Title = ControllerBase.UtilIdiomas.GetText("PERFILRECURSOSCOMPARTIDOSFICHA", "TEPUEDEINTERESAR");
                                gadget.Order = filaProyectoGadget.Orden;
                                gadget.ClassName = filaProyectoGadget.Clases;

                                string nombreCorto = filaProyectoGadget.NombreCorto;
                                if (string.IsNullOrEmpty(nombreCorto))
                                {
                                    nombreCorto = filaProyectoGadget.GadgetID.ToString();
                                }
                                gadget.ShortName = nombreCorto;

                                gadget.ViewNameResources = "ControlesMVC/_FichaRecursoGadget";

                                if (filaProyectoGadget.PersonalizacionComponenteID.HasValue)
                                {
                                    gadget.ViewNameResources = filaProyectoGadget.PersonalizacionComponenteID.ToString();
                                }

                                int numElementos = ControllerBase.ParametrosGeneralesRow.NumeroRecursosRelacionados;

                                List<Guid> listaIdsGadget = MontarRecursosRelacionados(filaProyectoGadget.GadgetID, numElementos, 0, pPrimeraPeticion);
                                if (listaIdsGadget != null && listaIdsGadget.Count > 0)
                                {
                                    listaRecursosGadgets.Add(filaProyectoGadget.GadgetID, new List<Guid>());
                                    listaRecursosGadgetsPaginador.Add(filaProyectoGadget.GadgetID, new List<Guid>());
                                    foreach (Guid idRecurso in listaIdsGadget)
                                    {
                                        if (listaRecursosGadgets[filaProyectoGadget.GadgetID].Count < numElementos)
                                        {
                                            listaRecursosGadgets[filaProyectoGadget.GadgetID].Add(idRecurso);
                                        }
                                        else
                                        {
                                            listaRecursosGadgetsPaginador[filaProyectoGadget.GadgetID].Add(idRecurso);
                                        }
                                    }
                                }
                                listaGadgets.Add(gadget);
                            }
                            else if (filaProyectoGadget.Tipo == (short)TipoGadget.LoMasInteresante)
                            {
                                GadgetResourceListModel gadget = new GadgetResourceListModel();
                                gadget.Key = filaProyectoGadget.GadgetID;
                                gadget.Title = ControllerBase.UtilIdiomas.GetText("COMINICIO", "LOMASINTERESANTE");
                                gadget.Order = filaProyectoGadget.Orden;
                                gadget.ClassName = filaProyectoGadget.Clases;

                                string nombreCorto = filaProyectoGadget.NombreCorto;
                                if (string.IsNullOrEmpty(nombreCorto))
                                {
                                    nombreCorto = filaProyectoGadget.GadgetID.ToString();
                                }
                                gadget.ShortName = nombreCorto;

                                gadget.ViewNameResources = "ControlesMVC/_FichaRecursoGadget";
                                if (filaProyectoGadget.PersonalizacionComponenteID.HasValue)
                                {
                                    gadget.ViewNameResources = filaProyectoGadget.PersonalizacionComponenteID.ToString();
                                }

                                List<Guid> listaIdsGadget = MontarRecursosMasInteresantes();
                                if (listaIdsGadget.Count > 0)
                                {
                                    listaRecursosGadgets.Add(filaProyectoGadget.GadgetID, listaIdsGadget);
                                }
                                listaGadgets.Add(gadget);
                            }
                            else if (filaProyectoGadget.Tipo == (short)TipoGadget.QueEstaPasando)
                            {
                                GadgetResourceListModel gadget = new GadgetResourceListModel();
                                gadget.Key = filaProyectoGadget.GadgetID;
                                gadget.Title = ControllerBase.UtilIdiomas.GetText("CONTROLESDOCUMENTACION", "QUEESTAPASANDO");
                                gadget.Order = filaProyectoGadget.Orden;
                                gadget.ClassName = filaProyectoGadget.Clases;

                                string nombreCorto = filaProyectoGadget.NombreCorto;
                                if (string.IsNullOrEmpty(nombreCorto))
                                {
                                    nombreCorto = filaProyectoGadget.GadgetID.ToString();
                                }
                                gadget.ShortName = nombreCorto;

                                gadget.ViewNameResources = "ControlesMVC/_FichaRecursoGadget";
                                if (filaProyectoGadget.PersonalizacionComponenteID.HasValue)
                                {
                                    gadget.ViewNameResources = filaProyectoGadget.PersonalizacionComponenteID.ToString();
                                }

                                List<Guid> listaIdsGadget = MontarQueEstaPasando();
                                if (listaIdsGadget.Count > 0)
                                {
                                    listaRecursosGadgets.Add(filaProyectoGadget.GadgetID, listaIdsGadget);
                                }
                                listaGadgets.Add(gadget);
                            }
                            else if (filaProyectoGadget.Tipo == (short)TipoGadget.UltDebates)
                            {
                                GadgetResourceListModel gadget = new GadgetResourceListModel();
                                gadget.Key = filaProyectoGadget.GadgetID;
                                gadget.Title = ControllerBase.UtilIdiomas.GetText("COMINICIO", "ULTIMOSDEBATES");
                                gadget.Order = filaProyectoGadget.Orden;
                                gadget.ClassName = filaProyectoGadget.Clases;

                                string nombreCorto = filaProyectoGadget.NombreCorto;
                                if (string.IsNullOrEmpty(nombreCorto))
                                {
                                    nombreCorto = filaProyectoGadget.GadgetID.ToString();
                                }
                                gadget.ShortName = nombreCorto;

                                gadget.ViewNameResources = "ControlesMVC/_FichaRecursoGadget";
                                if (filaProyectoGadget.PersonalizacionComponenteID.HasValue)
                                {
                                    gadget.ViewNameResources = filaProyectoGadget.PersonalizacionComponenteID.ToString();
                                }

                                List<Guid> listaIdsGadget = MontarDebates();
                                if (listaIdsGadget.Count > 0)
                                {
                                    listaRecursosGadgets.Add(filaProyectoGadget.GadgetID, listaIdsGadget);
                                }
                                listaGadgets.Add(gadget);
                            }
                            else if (filaProyectoGadget.Tipo == (short)TipoGadget.UltPreguntas)
                            {
                                GadgetResourceListModel gadget = new GadgetResourceListModel();
                                gadget.Key = filaProyectoGadget.GadgetID;
                                gadget.Title = ControllerBase.UtilIdiomas.GetText("COMINICIO", "ULTIMASPREGUNTAS");
                                gadget.Order = filaProyectoGadget.Orden;
                                gadget.ClassName = filaProyectoGadget.Clases;

                                string nombreCorto = filaProyectoGadget.NombreCorto;
                                if (string.IsNullOrEmpty(nombreCorto))
                                {
                                    nombreCorto = filaProyectoGadget.GadgetID.ToString();
                                }
                                gadget.ShortName = nombreCorto;

                                gadget.ViewNameResources = "ControlesMVC/_FichaRecursoGadget";
                                if (filaProyectoGadget.PersonalizacionComponenteID.HasValue)
                                {
                                    gadget.ViewNameResources = filaProyectoGadget.PersonalizacionComponenteID.ToString();
                                }

                                List<Guid> listaIdsGadget = MontarPreguntas();
                                if (listaIdsGadget.Count > 0)
                                {
                                    listaRecursosGadgets.Add(filaProyectoGadget.GadgetID, listaIdsGadget);
                                }
                                listaGadgets.Add(gadget);
                            }
                            else if (filaProyectoGadget.Tipo == (short)TipoGadget.UltEncuestas)
                            {
                                GadgetResourceListModel gadget = new GadgetResourceListModel();
                                gadget.Key = filaProyectoGadget.GadgetID;
                                gadget.Title = ControllerBase.UtilIdiomas.GetText("COMINICIO", "ULTIMAENCUESTA");
                                gadget.Order = filaProyectoGadget.Orden;
                                gadget.ClassName = filaProyectoGadget.Clases;

                                string nombreCorto = filaProyectoGadget.NombreCorto;
                                if (string.IsNullOrEmpty(nombreCorto))
                                {
                                    nombreCorto = filaProyectoGadget.GadgetID.ToString();
                                }
                                gadget.ShortName = nombreCorto;

                                gadget.ViewNameResources = "ControlesMVC/_FichaRecursoGadget";
                                if (filaProyectoGadget.PersonalizacionComponenteID.HasValue)
                                {
                                    gadget.ViewNameResources = filaProyectoGadget.PersonalizacionComponenteID.ToString();
                                }

                                List<Guid> listaIdsGadget = MontarEncuestas();
                                if (listaIdsGadget.Count > 0)
                                {
                                    listaRecursosGadgets.Add(filaProyectoGadget.GadgetID, listaIdsGadget);
                                    listaEncuestas.AddRange(listaIdsGadget);
                                }
                                listaGadgets.Add(gadget);
                            }
                            else if (filaProyectoGadget.Tipo == (short)TipoGadget.RecursosMasVistos)
                            {
                                GadgetResourceListModel gadget = new GadgetResourceListModel();
                                gadget.Key = filaProyectoGadget.GadgetID;
                                gadget.Title = UtilCadenas.ObtenerTextoDeIdioma(filaProyectoGadget.Titulo, ControllerBase.UtilIdiomas.LanguageCode, ControllerBase.ParametrosGeneralesRow.IdiomaDefecto);
                                gadget.Order = filaProyectoGadget.Orden;
                                gadget.ClassName = filaProyectoGadget.Clases;

                                string nombreCorto = filaProyectoGadget.NombreCorto;
                                if (string.IsNullOrEmpty(nombreCorto))
                                {
                                    nombreCorto = filaProyectoGadget.GadgetID.ToString();
                                }
                                gadget.ShortName = nombreCorto;

                                gadget.ViewNameResources = "ControlesMVC/_FichaRecursoGadget";
                                if (filaProyectoGadget.PersonalizacionComponenteID.HasValue)
                                {
                                    gadget.ViewNameResources = filaProyectoGadget.PersonalizacionComponenteID.ToString();
                                }

                                List<Guid> listaIdsGadget = MontarMasVistos();
                                if (listaIdsGadget.Count > 0)
                                {
                                    listaRecursosGadgets.Add(filaProyectoGadget.GadgetID, listaIdsGadget);
                                }
                                listaGadgets.Add(gadget);
                            }
                            else if (filaProyectoGadget.Tipo == (short)TipoGadget.RecursosContextos)
                            {
                                GadgetResourceListModel gadget = new GadgetResourceListModel();
                                gadget.Key = filaProyectoGadget.GadgetID;
                                gadget.Title = UtilCadenas.ObtenerTextoDeIdioma(filaProyectoGadget.Titulo, ControllerBase.UtilIdiomas.LanguageCode, ControllerBase.ParametrosGeneralesRow.IdiomaDefecto);
                                gadget.Order = filaProyectoGadget.Orden;
                                gadget.ClassName = filaProyectoGadget.Clases;
                                gadget.ViewNameResources = "ControlesMVC/_FichaRecursoGadget";
                                if (filaProyectoGadget.PersonalizacionComponenteID.HasValue)
                                {
                                    gadget.ViewNameResources = filaProyectoGadget.PersonalizacionComponenteID.ToString();
                                }

                                string nombreCorto = filaProyectoGadget.NombreCorto;
                                if (string.IsNullOrEmpty(nombreCorto))
                                {
                                    nombreCorto = filaProyectoGadget.GadgetID.ToString();
                                }
                                gadget.ShortName = nombreCorto;

                                ProyectoGadgetContexto filasProyectoGadgetContexto = filaProyectoGadget.ProyectoGadgetContexto;
                                int numElementos = filasProyectoGadgetContexto.NumRecursos;
                                if (!filasProyectoGadgetContexto.OcultarVerMas)
                                {
                                    gadget.UrlViewMore = filasProyectoGadgetContexto.ComunidadOrigen;
                                }

                                KeyValuePair<string, List<ResourceModel>> listaFichasContexto = CargarRecursosContexto(filasProyectoGadgetContexto, gadget.ShortName, 1, pPrimeraPeticion, pUsarAfinidad);

                                if (listaFichasContexto.Value != null)
                                {
                                    gadget.Resources = new List<ResourceModel>();
                                    gadget.ResourcesPagers = new List<ResourceModel>();

                                    foreach (ResourceModel fichaRecurso in listaFichasContexto.Value)
                                    {
                                        if (gadget.Resources.Count < numElementos)
                                        {
                                            gadget.Resources.Add(fichaRecurso);
                                        }
                                        else
                                        {
                                            gadget.ResourcesPagers.Add(fichaRecurso);
                                        }
                                    }
                                    listaGadgets.Add(gadget);
                                }
                            }
                            else if (filaProyectoGadget.Tipo == (short)TipoGadget.ProyRelacionados)
                            {
                                GadgetCommunitiesListModel gadget = new GadgetCommunitiesListModel();
                                gadget.Key = filaProyectoGadget.GadgetID;
                                gadget.Title = ControllerBase.UtilIdiomas.GetText("COMINICIO", "COMUNIDADESRELACIONADAS");
                                gadget.Order = filaProyectoGadget.Orden;
                                gadget.ClassName = filaProyectoGadget.Clases;

                                string nombreCorto = filaProyectoGadget.NombreCorto;
                                if (string.IsNullOrEmpty(nombreCorto))
                                {
                                    nombreCorto = filaProyectoGadget.GadgetID.ToString();
                                }
                                gadget.ShortName = nombreCorto;

                                gadget.Communities = CargarComunidadesRelacionadas();
                                listaGadgets.Add(gadget);
                            }
                            else if (filaProyectoGadget.Tipo == (short)TipoGadget.TemasRelacionadosDidactalia)
                            {
                                GadgetResourceListModel gadget = new GadgetResourceListModel();
                                gadget.Key = filaProyectoGadget.GadgetID;
                                gadget.Title = UtilCadenas.ObtenerTextoDeIdioma(filaProyectoGadget.Titulo, ControllerBase.UtilIdiomas.LanguageCode, ControllerBase.ParametrosGeneralesRow.IdiomaDefecto);
                                gadget.Order = filaProyectoGadget.Orden;
                                gadget.ClassName = filaProyectoGadget.Clases;

                                string nombreCorto = filaProyectoGadget.NombreCorto;
                                if (string.IsNullOrEmpty(nombreCorto))
                                {
                                    nombreCorto = filaProyectoGadget.GadgetID.ToString();
                                }
                                gadget.ShortName = nombreCorto;

                                gadget.ViewNameResources = "ControlesMVC/_FichaRecursoGadget";
                                if (filaProyectoGadget.PersonalizacionComponenteID.HasValue)
                                {
                                    gadget.ViewNameResources = filaProyectoGadget.PersonalizacionComponenteID.ToString();
                                }
                                KeyValuePair<string, List<ResourceModel>>? listaFichasContexto = CargarTemasRelacionadosDidactalia(filaProyectoGadget);
                                if (listaFichasContexto.HasValue && listaFichasContexto.Value.Value.Count > 0)
                                {
                                    gadget.Title += " " + listaFichasContexto.Value.Key;
                                    gadget.Resources = new List<ResourceModel>();
                                    gadget.ResourcesPagers = new List<ResourceModel>();
                                    foreach (ResourceModel fichaRecurso in listaFichasContexto.Value.Value)
                                    {
                                        gadget.Resources.Add(fichaRecurso);
                                    }
                                    listaGadgets.Add(gadget);
                                }
                            }
                            //else if (filaProyectoGadget.Tipo == (short)TipoGadget.Consulta)
                            //{
                            //    CargarResultadosGadgetConsulta(filaProyectoGadget, filaProyectoGadget.GadgetID);
                            //}
                            //else if (filaProyectoGadget.Tipo == (short)TipoGadget.RecursosContextosHTMLplano)
                            //{
                            //    CargarRecursosContextoHTMLplano(filaProyectoGadget, filaProyectoGadget.GadgetID);
                            //}


                            //algunGadgetSinCache = algunGadgetSinCache || mAlgunGadgetSinCargar;

                            //if (mAlgunGadgetSinCargar && pNumPeticion == -1)
                            //{
                            //    mAlgunGadgetSinCargar = false;

                            //    HtmlGenericControl divTemporal = new HtmlGenericControl("div");
                            //    divTemporal.Attributes.Add("id", filaProyectoGadget.GadgetID.ToString());
                            //    panGadgets.Controls.Add(divTemporal);

                            //    mGadgetsSinCargar += filaProyectoGadget.GadgetID.ToString() + ",";
                            //}
                        }
                    }
                }

                List<Guid> ListaRecursosID = new List<Guid>();
                foreach (Guid gadgetID in listaRecursosGadgets.Keys)
                {
                    ListaRecursosID.AddRange(listaRecursosGadgets[gadgetID]);
                }
                foreach (Guid gadgetID in listaRecursosGadgetsPaginador.Keys)
                {
                    ListaRecursosID.AddRange(listaRecursosGadgetsPaginador[gadgetID]);
                }

                if (ListaRecursosID.Count > 0)
                {
                    MVCCN mvcCN = new MVCCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<MVCCN>(), mLoggerFactory);

                    string baseUrlBusqueda = UrlsSemanticas.ObtenerURLComunidad(ControllerBase.UtilIdiomas, ControllerBase.BaseURLIdioma, ControllerBase.ProyectoSeleccionado.NombreCorto) + "/" + ControllerBase.UtilIdiomas.GetText("URLSEM", "BUSQUEDAAVANZADA");

                    List<string> listaBaseURLContent = new List<string>();
                    listaBaseURLContent.Add(ControllerBase.BaseURLContent);

                    ControladorProyectoMVC controladorProyectoMVC = new ControladorProyectoMVC(ControllerBase.UtilIdiomas, ControllerBase.BaseURL, listaBaseURLContent, ControllerBase.BaseURLStatic, ControllerBase.ProyectoSeleccionado, Guid.Empty, ControllerBase.ParametrosGeneralesRow, mIdentidadUsuarioActual, EsBot, mLoggingService, mEntityContext, mConfigService, mHttpContextAccessor, mRedisCacheWrapper, mVirtuosoAD, mGnossCache, mEntityContextBASE, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorProyectoMVC>(), mLoggerFactory);

                    Dictionary<Guid, ResourceModel> listaFichasRecursos = controladorProyectoMVC.ObtenerRecursosPorID(ListaRecursosID, baseUrlBusqueda, null, false);

                    foreach (Guid gadgetID in listaRecursosGadgets.Keys)
                    {
                        GadgetResourceListModel gadgetListaRecurso = (GadgetResourceListModel)listaGadgets.Find(gadget => gadget.Key == gadgetID);
                        gadgetListaRecurso.Resources = new List<ResourceModel>();

                        foreach (Guid recursoID in listaRecursosGadgets[gadgetID])
                        {
                            if (listaFichasRecursos.ContainsKey(recursoID))
                            {
                                gadgetListaRecurso.Resources.Add(listaFichasRecursos[recursoID]);
                            }
                        }
                        if (listaRecursosGadgetsPaginador.ContainsKey(gadgetID))
                        {
                            gadgetListaRecurso.ResourcesPagers = new List<ResourceModel>();
                            foreach (Guid recursoID in listaRecursosGadgetsPaginador[gadgetID])
                            {
                                if (listaFichasRecursos.ContainsKey(recursoID))
                                {
                                    gadgetListaRecurso.ResourcesPagers.Add(listaFichasRecursos[recursoID]);
                                }
                            }
                        }
                    }
                }
            }

            ControllerBase.ViewBag.OcultarPersonalizacion = ControllerBase.ParametrosGeneralesRow.OcultarPersonalizacion;

            return listaGadgets;
        }

        private List<GadgetModel> CargarListaGadgetsPerfil(Guid pPerfilID)
        {
            IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
            DataWrapperIdentidad gadgets = identCN.ObtenerGadgetsPerfil(pPerfilID);
            identCN.Dispose();

            List<GadgetModel> listaGadgets = new List<GadgetModel>();

            if (gadgets.ListaPerfilGadget.Count > 0)
            {
                List<AD.EntityModel.Models.IdentidadDS.PerfilGadget> filasPerfilGadget = gadgets.ListaPerfilGadget.OrderBy(item => item.Orden).ToList();
                foreach (AD.EntityModel.Models.IdentidadDS.PerfilGadget filaPerfilGadget in filasPerfilGadget)
                {
                    #region HTML incrustado

                    string contenido = filaPerfilGadget.Contenido;

                    GadgetHtmlModel gadget = new GadgetHtmlModel();
                    gadget.Key = filaPerfilGadget.GadgetID;
                    gadget.Title = UtilCadenas.ObtenerTextoDeIdioma(filaPerfilGadget.Titulo, ControllerBase.UtilIdiomas.LanguageCode, ControllerBase.ParametrosGeneralesRow.IdiomaDefecto);
                    gadget.ClassName = "";
                    gadget.Order = filaPerfilGadget.Orden;

                    if (contenido.IndexOf("[javascript]") == 0)
                    {
                        contenido = contenido.Substring("[javascript]".Length);
                        contenido = contenido.Substring(contenido.IndexOf("[javascript]") + ("[javascript]").Length);
                    }
                    gadget.Html = contenido;

                    listaGadgets.Add(gadget);

                    #endregion
                }

                ControllerBase.ViewBag.OcultarPersonalizacion = false;
            }

            return listaGadgets;
        }

        /// <summary>
        /// Monta la lista de las últimas preguntas planteadas
        /// </summary>
        private List<Guid> MontarRecursosRelacionados(Guid pGadgetID, int pNumElementos, int pPagina, bool pPrimeraPeticion)
        {
            bool mSoloGenerarRelacionados = false;
            if (ControllerBase.Request.Headers["cargarRelacionados"] == "true")
            {
                mSoloGenerarRelacionados = true;
            }

            List<Guid> listaIDs = null;

            try
            {
                int inicio = 0;
                if (pPagina > 0)
                {
                    inicio = pPagina * pNumElementos;
                }
                int elementosMostrar = pNumElementos;

                if (pPrimeraPeticion || pPagina == 0)
                {
                    elementosMostrar = elementosMostrar * 2;
                }

                DocumentacionCL docCL = new DocumentacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCL>(), mLoggerFactory);
                if (pPagina == 0 && !mSoloGenerarRelacionados)
                {
                    listaIDs = (List<Guid>)docCL.ObtenerRelacionadosRecursoMVC(Documento.Clave, ControllerBase.ProyectoSeleccionado.Clave);
                }
                if ((listaIDs == null && (pPagina > 0 || !pPrimeraPeticion)) || mSoloGenerarRelacionados)
                {
                    listaIDs = new List<Guid>();

                    int tablaProyectoId = ControllerBase.ProyectoSeleccionado.FilaProyecto.TablaBaseProyectoID;

                    string nombreTablaCOMUNIDADES = "COMUNIDAD_000000000000000".Substring(0, 25 - tablaProyectoId.ToString().Length) + tablaProyectoId.ToString();

                    FacetadoDS facetadoDS = new FacetadoDS();

                    FacetadoCN facetadoCN = new FacetadoCN(ControllerBase.UrlIntragnoss, ControllerBase.ProyectoSeleccionado.Clave.ToString(), mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetadoCN>(), mLoggerFactory);
                    facetadoCN.InformacionOntologias = ControllerBase.InformacionOntologias;

                    string tags = "";
                    if (Documento.ListaTagsSoloLectura.Count > 0)
                    {
                        foreach (string tag in Documento.ListaTagsSoloLectura)
                        {
                            tags += "\"" + tag.Replace("\"", "'").Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ") + "\",";
                        }
                        tags = tags.Substring(0, tags.Length - 1);
                    }

                    string categorias = "";
                    if (Documento.Categorias.Count > 0)
                    {
                        foreach (Guid categoriaID in Documento.Categorias.Keys)
                        {
                            categorias += "gnoss:" + categoriaID.ToString().ToUpper() + ",";
                        }
                        categorias = categorias.Substring(0, categorias.Length - 1);
                    }

                    facetadoCN.ObtenerRecursosRelacionadosNuevo(ControllerBase.ProyectoSeleccionado.Clave.ToString(), Documento.Clave.ToString(), facetadoDS, inicio, elementosMostrar, tags, categorias, ControllerBase.ProyectoSeleccionado.TipoProyecto == TipoProyecto.CatalogoNoSocialConUnTipoDeRecurso, NombreSemPestanya);

                    if (facetadoDS.Tables["RecurosRelacionados"].Rows.Count > 0)
                    {
                        foreach (DataRow myrow in facetadoDS.Tables["RecurosRelacionados"].Rows)
                        {
                            string recursoRelacionado = (string)myrow[0];
                            recursoRelacionado = recursoRelacionado.Substring(recursoRelacionado.IndexOf("gnoss") + 6);

                            listaIDs.Add(new Guid(recursoRelacionado));
                        }
                    }
                    if (pPagina == 0 && listaIDs.Count > 0)
                    {
                        docCL.AgregarRelacionadosRecursoMVC(Documento.Clave, ControllerBase.ProyectoSeleccionado.Clave, listaIDs);
                    }
                }
            }
            catch (Exception ex)
            {
                ControllerBase.GuardarLogError("Error pintando los recursos relacionados del recurso: " + Documento.Clave.ToString() + " en la comunidad " + ControllerBase.ProyectoSeleccionado.Nombre + " \n" + ex.ToString());
            }

            return listaIDs;
        }

        /// <summary>
        /// Monta la lista de las últimas preguntas planteadas
        /// </summary>
        private List<Guid> MontarRecursosMasInteresantes()
        {
            DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
            List<Guid> listaIDs = docCN.ObtenerListaRecursosPopularesProyecto(ControllerBase.ProyectoSeleccionado.Clave, 20);
            docCN.Dispose();

            return listaIDs;
        }

        /// <summary>
        /// 
        /// </summary>
        private List<Guid> MontarQueEstaPasando()
        {
            List<Guid> listaIDs = new List<Guid>();

            if (ControllerBase.ProyectoSeleccionado.Clave != ProyectoAD.MetaProyecto)
            {
                DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
                listaIDs = docCN.ObtenerListaUltimosDebatesPreguntasPublicados(ControllerBase.ProyectoSeleccionado.Clave, 3);
                docCN.Dispose();
            }
            return listaIDs;
        }

        /// <summary>
        /// 
        /// </summary>
        private List<Guid> MontarMasVistos()
        {
            List<Guid> listaIDs = new List<Guid>();

            if (ControllerBase.ProyectoSeleccionado.Clave != ProyectoAD.MetaProyecto)
            {
                //TODO CORE-2945
                BaseVisitasCN baseVisitasCN = new BaseVisitasCN("base", mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<BaseVisitasCN>(), mLoggerFactory);
                //Pedimos el doble de elementos por si alguno esta eliminado o es privado
                listaIDs = baseVisitasCN.ObtenerRecursosMasVisistadosProyecto(ControllerBase.ProyectoSeleccionado.Clave, 7, 6);
                baseVisitasCN.Dispose();

            }
            return listaIDs;
        }

        /// <summary>
        /// Monta la lista de los últimos debates
        /// </summary>
        private List<Guid> MontarDebates()
        {
            List<Guid> listaIDs = new List<Guid>();

            DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
            listaIDs = docCN.ObtenerListaUltimosDebatesPublicados(ControllerBase.ProyectoSeleccionado.Clave, 3);
            docCN.Dispose();

            return listaIDs;
        }

        /// <summary>
        /// Monta la lista de las últimas preguntas planteadas
        /// </summary>
        private List<Guid> MontarPreguntas()
        {
            List<Guid> listaIDs = new List<Guid>();

            DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
            listaIDs = docCN.ObtenerListaUltimasPreguntasPublicados(ControllerBase.ProyectoSeleccionado.Clave, 3);
            docCN.Dispose();

            return listaIDs;
        }

        /// <summary>
        /// Monta la lista de las últimas preguntas planteadas
        /// </summary>
        private List<Guid> MontarEncuestas()
        {
            DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
            Guid? encuestaID = docCN.ObtenerIDEncuestaParaHome(ControllerBase.ProyectoSeleccionado.Clave);
            docCN.Dispose();

            List<Guid> listaIDs = new List<Guid>();
            if (encuestaID.HasValue)
            {
                listaIDs.Add(encuestaID.Value);
            }
            return listaIDs;
        }

        /// <summary>
        /// Monta la lista de comunidades relacionadas (aquellas que compartan categorías en el tesauro de MyGNOSS)
        /// </summary>
        private List<CommunityModel> CargarComunidadesRelacionadas()
        {
            List<CommunityModel> listaComunidades = new List<CommunityModel>();

            ControladorProyecto controladorProyecto = new ControladorProyecto(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorProyecto>(), mLoggerFactory);
            List<ElementoGnoss> listaProyectos = controladorProyecto.CargarComunidadesRelacionadasProyecto(ControllerBase.ProyectoSeleccionado);

            if (ControllerBase.ProyectoSeleccionado.TipoAcceso == TipoAcceso.Privado)
            {
                List<ElementoGnoss> listaProyectosAux = new List<ElementoGnoss>();
                listaProyectosAux.AddRange(listaProyectos);

                //Elimino comunidades reservadas:
                foreach (ElementoGnoss proyecto in listaProyectosAux)
                {
                    if (((Elementos.ServiciosGenerales.Proyecto)proyecto).TipoAcceso == TipoAcceso.Reservado)
                    {
                        listaProyectos.Remove(proyecto);
                    }
                }
            }

            if (listaProyectos.Count > 0)
            {
                foreach (Elementos.ServiciosGenerales.Proyecto proyecto in listaProyectos)
                {
                    CommunityModel fichaProyecto = new CommunityModel();
                    fichaProyecto.Key = proyecto.Clave;
                    string nombreProyecto = proyecto.Nombre;
                    if (nombreProyecto.Contains("|||"))
                    {
                        List<string> nombresIdioma = nombreProyecto.Split(new string[] { "|||" }, StringSplitOptions.None).ToList();
                        Dictionary<string, string> nombreIdiomaDictionary = new Dictionary<string, string>();
                        foreach (string nombreCode in nombresIdioma)
                        {
                            if (nombreCode.Contains("@"))
                            {
                                string[] idiomas = nombreCode.Split('@');
                                nombreIdiomaDictionary.Add(idiomas[1], idiomas[0]);
                            }
                        }

                        if (nombreIdiomaDictionary.ContainsKey(UtilIdiomas.LanguageCode))
                        {
                            nombreProyecto = nombreIdiomaDictionary[UtilIdiomas.LanguageCode];
                        }
                        else
                        {
                            nombreProyecto = nombreIdiomaDictionary.Values.First();
                        }
                    }

                    fichaProyecto.Name = nombreProyecto;
                    fichaProyecto.Url = UrlsSemanticas.ObtenerURLComunidad(ControllerBase.UtilIdiomas, ControllerBase.BaseURLIdioma, proyecto.NombreCorto);

                    string nombreImagenePeque = new ControladorProyecto(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorProyecto>(), mLoggerFactory).ObtenerFilaParametrosGeneralesDeProyecto(proyecto.Clave).NombreImagenPeque;
                    string url = ControllerBase.BaseURLContent + "/" + UtilArchivos.ContentImagenes + "/" + UtilArchivos.ContentImagenesProyectos + "/" + nombreImagenePeque;
                    if (nombreImagenePeque == "peque")
                    {
                        url = ControllerBase.BaseURLStatic + "/img" + "/" + UtilArchivos.ContentImgIconos + "/" + UtilArchivos.ContentImagenesProyectos + "/" + "anonimo_peque.png";
                    }
                    fichaProyecto.Logo = url;

                    listaComunidades.Add(fichaProyecto);
                }
            }

            return listaComunidades;
        }

        private UtilidadesVirtuoso mUtilidadesVirtuoso;

        private UtilidadesVirtuoso UtilidadesVirtuoso
        {
            get
            {
                if (mUtilidadesVirtuoso == null)
                    mUtilidadesVirtuoso = new UtilidadesVirtuoso(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mEntityContextBASE, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<UtilidadesVirtuoso>(), mLoggerFactory);
                return mUtilidadesVirtuoso;
            }
        }

        /// <summary>
        /// Carga los resultados de la consulta que indica el gadget.
        /// </summary>
        /// <param name="pFilaProyGadget">Fila gadget</param>
        private KeyValuePair<string, List<ResourceModel>> CargarRecursosContexto(ProyectoGadgetContexto pFilaProyGadget, string pNombreCortoGadget, int pPagina, bool pPrimeraPeticion, bool pUsarAfinidad = false)
        {
            List<ResourceModel> listaRecursosContextos = null;

            bool mSoloGenerarRelacionados = false;
            if (ControllerBase.Request.Headers["cargarRelacionados"] == "true")
            {
                mSoloGenerarRelacionados = true;
            }

            Stopwatch sw = null;

            try
            {
                if (pPagina == 1 && !pPrimeraPeticion && !mSoloGenerarRelacionados && !pFilaProyGadget.ObtenerPrivados)
                {
                    //Intentamos obtener desde la cache
                    DocumentacionCL docCl = new DocumentacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCL>(), mLoggerFactory);
                    List<Guid> listaRecursos = docCl.ObtenerContextoRecursoMVC(Documento.Clave, ControllerBase.ProyectoSeleccionado.Clave, pFilaProyGadget.GadgetID);
                    docCl.Dispose();

                    if (listaRecursos != null)
                    {
                        if (listaRecursos.Count > 0)
                        {
                            CargadorResultados cargadorResultadosContexto = new CargadorResultados();
                            //cargadorResultadosContexto.Url = ObtenerUrlServicioResultados(pFilaProyGadget);
                            cargadorResultadosContexto.Url = mConfigService.ObtenerUrlServicioResultados();
                            string parametros = "listadoRecursosEstatico:";
                            foreach (Guid id in listaRecursos)
                            {
                                parametros += id + ",";
                            }

                            Guid identidadActualID = UsuarioAD.Invitado;
                            bool esUsuarioInvitado = false;
                            mLoggingService.AgregarEntrada("Carga contextos");

                            bool proyectoDeEcosistema = true;
                            if (!pFilaProyGadget.ProyectoOrigenID.Equals(ControllerBase.ProyectoSeleccionado.Clave))
                            {
                                ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
                                DataWrapperProyecto proyectoDataWrapper = proyectoCN.ObtenerProyectoCargaLigeraPorID(pFilaProyGadget.ProyectoOrigenID);
                                proyectoCN.Dispose();

                                if (proyectoDataWrapper.ListaProyecto.Count > 0)
                                {
                                    try
                                    {
                                        string urlpropiaPropioEcosistema = proyectoDataWrapper.ListaProyecto.First().URLPropia;
                                        EstadoProyecto estadoProyecto = (EstadoProyecto)proyectoDataWrapper.ListaProyecto.First().Estado;

                                        if (urlpropiaPropioEcosistema.Contains('@'))
                                        {
                                            urlpropiaPropioEcosistema = urlpropiaPropioEcosistema.Substring(0, urlpropiaPropioEcosistema.IndexOf('@'));
                                        }

                                        Uri uriPropioEcosistema = new Uri(urlpropiaPropioEcosistema);
                                        Uri uriContextoEcosistema = new Uri(pFilaProyGadget.ComunidadOrigen);
                                        if (estadoProyecto == EstadoProyecto.Cerrado || uriPropioEcosistema.Host != uriContextoEcosistema.Host)
                                        {
                                            proyectoDeEcosistema = false;
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        proyectoDeEcosistema = false;
                                    }
                                }
                                else
                                {
                                    proyectoDeEcosistema = false;
                                }
                            }

                            if (!proyectoDeEcosistema)
                            {
                                //El proyecto es de otro ecosistema, hago la petición a su servicio de resultados
                                sw = LoggingService.IniciarRelojTelemetria();
                                listaRecursosContextos = cargadorResultadosContexto.CargarResultadosContexto(pFilaProyGadget.ProyectoID, parametros, false, ControllerBase.UtilIdiomas.LanguageCode, TipoBusqueda.Recursos, 0, pFilaProyGadget.ProyectoOrigenID.ToString(), pFilaProyGadget.ComunidadOrigen, "", false, pFilaProyGadget.MostrarEnlaceOriginal, pFilaProyGadget.NamespacesExtra, pFilaProyGadget.ItemsBusqueda, "", pFilaProyGadget.NuevaPestanya, "", identidadActualID, esUsuarioInvitado);
                                mLoggingService.AgregarEntradaDependencia("Contexto externo: Llamar al servicio de resultados", false, "CargarRecursosContexto", sw, true);
                            }
                            else
                            {
                                mLoggingService.AgregarEntrada("Contexto interno");
                                Elementos.ServiciosGenerales.Proyecto proyectoContexto = ControllerBase.ProyectoSeleccionado;
                                ParametroGeneral filaParametrosGenerales = ControllerBase.ParametrosGeneralesRow;
                                if (ControllerBase.ProyectoSeleccionado.Clave != pFilaProyGadget.ProyectoOrigenID)
                                {
                                    ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);
                                    GestionProyecto gestorProyecto = new GestionProyecto(proyCL.ObtenerProyectoPorID(pFilaProyGadget.ProyectoOrigenID), mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestionProyecto>(), mLoggerFactory);
                                    proyectoContexto = gestorProyecto.ListaProyectos[pFilaProyGadget.ProyectoOrigenID];

                                    ParametroGeneralCL parametroGeneralCL = new ParametroGeneralCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroGeneralCL>(), mLoggerFactory);
                                    // ParametroGeneralDS paramGeneralDS = parametroGeneralCL.ObtenerParametrosGeneralesDeProyecto(proyectoContexto.Clave);
                                    GestorParametroGeneral paramGeneralDS = parametroGeneralCL.ObtenerParametrosGeneralesDeProyecto(proyectoContexto.Clave);
                                    //filaParametrosGenerales = paramGeneralDS.ParametroGeneral.FindByOrganizacionIDProyectoID(proyectoContexto.FilaProyecto.OrganizacionID, proyectoContexto.Clave);
                                    filaParametrosGenerales = paramGeneralDS.ListaParametroGeneral.Find(parametroG => parametroG.OrganizacionID.Equals(proyectoContexto.FilaProyecto.OrganizacionID) && parametroG.ProyectoID.Equals(proyectoContexto.Clave));
                                    mLoggingService.AgregarEntrada("Proyecto diferente");
                                }
                                ControladorProyectoMVC controladorProyectoMVC = new ControladorProyectoMVC(ControllerBase.UtilIdiomas, ControllerBase.BaseURL, ControllerBase.BaseURLContent, ControllerBase.BaseURLStatic, proyectoContexto, filaParametrosGenerales, mIdentidadUsuarioActual, EsBot, mLoggingService, mEntityContext, mConfigService, mHttpContextAccessor, mRedisCacheWrapper, mVirtuosoAD, mGnossCache, mEntityContextBASE, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorProyectoMVC>(), mLoggerFactory);
                                listaRecursosContextos = controladorProyectoMVC.ObtenerRecursosPorID(listaRecursos, pFilaProyGadget.ComunidadOrigen, null, true).Values.ToList();
                            }
                            mLoggingService.AgregarEntrada("Fin carga contextos");
                        }
                        else
                        {
                            listaRecursosContextos = new List<ResourceModel>();
                        }
                    }
                }
                if (((listaRecursosContextos == null || listaRecursosContextos.Count == 0) && (pPagina > 1 || !pPrimeraPeticion)) || mSoloGenerarRelacionados)
                {
                    #region Obtenemos los recursos

                    //Obtenemos la ruta desde la que obtendremos el contexto
                    string rutaGadgetOrigen = pFilaProyGadget.ComunidadOrigen;

                    #region Obtenemos filtros
                    string FiltrosOrigen = pFilaProyGadget.ComunidadOrigenFiltros;
                    string FiltrosOrigenDestino = pFilaProyGadget.FiltrosOrigenDestino;

                    //Obtenemos el peso minimo del contexto
                    int pesoMinimo = 0;
                    if (FiltrosOrigenDestino.Contains("|||") && FiltrosOrigenDestino.StartsWith("pesomin="))
                    {
                        string peso = FiltrosOrigenDestino.Split(new string[] { "|||" }, StringSplitOptions.RemoveEmptyEntries)[0];
                        peso = peso.Replace("pesomin=", "");
                        pesoMinimo = int.Parse(peso);
                        FiltrosOrigenDestino = FiltrosOrigenDestino.Replace("pesomin=" + peso + "|||", "");
                    }

                    string FiltrosOrden = pFilaProyGadget.OrdenContexto;
                    string TipoImagen = pFilaProyGadget.Imagen.ToString();
                    if (string.IsNullOrEmpty(FiltrosOrden))
                    {
                        FiltrosOrden = "null";
                    }
                    string filtros = UtilidadesVirtuoso.Obtenerfiltros(FiltrosOrigenDestino, Documento.Clave, ControllerBase.ProyectoSeleccionado.Clave, ControllerBase.ProyectoOrigenBusquedaID, ControllerBase.UrlIntragnoss, ControllerBase.InformacionOntologias, ControllerBase.UtilIdiomas.LanguageCode, pFilaProyGadget.ProyectoOrigenID, GestorFacetasComunidad, pUsarAfinidad);
                    FiltrosOrigen = UtilidadesVirtuoso.ObtenerfiltrosOrigen(FiltrosOrigen, Documento.Clave, ControllerBase.ProyectoSeleccionado.Clave, ControllerBase.ProyectoOrigenBusquedaID, ControllerBase.UrlIntragnoss, ControllerBase.InformacionOntologias, pUsarAfinidad);

                    if (FiltrosOrigen == "nomostrar")
                    {
                        return new KeyValuePair<string, List<ResourceModel>>(pNombreCortoGadget, new List<ResourceModel>());
                    }

                    #endregion

                    #region Creamos query
                    string filtroContextoSelect = filtros.Substring(0, filtros.IndexOf("&&&"));

                    if (!string.IsNullOrEmpty(FiltrosOrigenDestino) && string.IsNullOrEmpty(filtros.Substring(filtros.IndexOf("&&&") + 3)))
                    {
                        return new KeyValuePair<string, List<ResourceModel>>(pNombreCortoGadget, new List<ResourceModel>());
                    }

                    string filtroContextoWhere = FiltrosOrigen + " " + filtros.Substring(filtros.IndexOf("&&&") + 3);

                    #endregion

                    string tituloRecurso = UtilCadenas.ObtenerTextoDeIdioma(Documento.Titulo.Replace("\"", "'"), ControllerBase.UtilIdiomas.LanguageCode, ControllerBase.ParametrosGeneralesRow.IdiomaDefecto);
                    string nombreComunidad = UtilCadenas.ObtenerTextoDeIdioma(ControllerBase.ProyectoSeleccionado.Nombre.Replace("\"", "'"), ControllerBase.UtilIdiomas.LanguageCode, ControllerBase.ParametrosGeneralesRow.IdiomaDefecto);

                    string filtroContexto = "Recursos relacionados con: '" + tituloRecurso + "' en " + nombreComunidad + "|||" + filtroContextoSelect + "|||" + filtroContextoWhere + "|||" + FiltrosOrden + "|||" + pesoMinimo;

                    string docsAEliminar = Documento.Clave.ToString();

                    #region obtener resultados a eliminar

                    string[] resultadosEliminar;
                    if (pFilaProyGadget.ResultadosEliminar != null)
                    {
                        resultadosEliminar = pFilaProyGadget.ResultadosEliminar.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    }
                    else
                    {

                        resultadosEliminar = Array.Empty<string>();
                    }


                    List<Guid> listaIDEliminar = new List<Guid>();
                    foreach (string resultadoEliminar in resultadosEliminar)
                    {
                        Guid idRecurso;
                        if (Guid.TryParse(resultadoEliminar, out idRecurso))
                        {
                            listaIDEliminar.Add(idRecurso);
                        }
                        else
                        {
                            if (Documento.TipoDocumentacion == TiposDocumentacion.Semantico)
                            {
                                foreach (ElementoOntologia eleOnto in GenPlantillasOWL.Entidades)
                                {
                                    #region Profundidad
                                    //Tiene profundidad
                                    Dictionary<int, string> props = new Dictionary<int, string>();

                                    int i = 0;
                                    foreach (string prop in resultadoEliminar.Split(new string[] { "@@@" }, StringSplitOptions.RemoveEmptyEntries))
                                    {
                                        props.Add(i, prop);
                                        i++;
                                    }

                                    for (int keyProp = 0; keyProp < props.Count; keyProp++)
                                    {
                                        string prop = props[keyProp];

                                        if (prop.Contains(":"))
                                        {
                                            if (prop.Substring(0, prop.IndexOf(":")) == GenPlantillasOWL.Ontologia.GestorOWL.NamespaceOntologia)
                                            {
                                                prop = prop.Substring(prop.IndexOf(":") + 1);
                                            }
                                            else
                                            {
                                                foreach (string key in GenPlantillasOWL.Ontologia.NamespacesDefinidos.Keys)
                                                {
                                                    if (GenPlantillasOWL.Ontologia.NamespacesDefinidos[key] == prop.Substring(0, prop.IndexOf(":")))
                                                    {
                                                        prop = key + prop.Substring(prop.IndexOf(":") + 1);
                                                        break;
                                                    }
                                                }
                                            }
                                            props[keyProp] = prop;
                                        }
                                    }
                                    #endregion

                                    listaIDEliminar.AddRange(obtenerElementosExcluidos(eleOnto, new List<string>(props.Values)));
                                }
                            }
                        }
                    }

                    foreach (Guid idDocEliminar in listaIDEliminar)
                    {
                        docsAEliminar += "," + idDocEliminar.ToString();
                    }

                    #endregion

                    CargadorResultados cargadorResultadosContexto = new CargadorResultados();
                    //cargadorResultadosContexto.Url = ObtenerUrlServicioResultados(pFilaProyGadget);
                    cargadorResultadosContexto.Url = mConfigService.ObtenerUrlServicioResultados();
                    int numRecursosPagina = pFilaProyGadget.NumRecursos;
                    if (pPagina == 1)
                    {
                        numRecursosPagina = numRecursosPagina * 2;
                    }

                    short tipoBusqueda = (short)TipoBusqueda.Recursos;
                    string pagina = pFilaProyGadget.ComunidadOrigen.Substring(pFilaProyGadget.ComunidadOrigen.LastIndexOf("/") + 1);
					ParametroAplicacionCL paramCL = new ParametroAplicacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCL>(), mLoggerFactory);
					foreach (string idioma in paramCL.ObtenerListaIdiomas())
                    {
                        UtilIdiomas utilIdiomasAux = new UtilIdiomas(idioma, mLoggingService, mEntityContext, mConfigService,mRedisCacheWrapper, mLoggerFactory.CreateLogger<UtilIdiomas>(), mLoggerFactory);

                        if (pagina == utilIdiomasAux.GetText("URLSEM", "PREGUNTAS"))
                        {
                            tipoBusqueda = (short)TipoBusqueda.Preguntas;
                            break;
                        }
                        else if (pagina == utilIdiomasAux.GetText("URLSEM", "DEBATES"))
                        {
                            tipoBusqueda = (short)TipoBusqueda.Debates;
                            break;
                        }
                        else if (pagina == utilIdiomasAux.GetText("URLSEM", "ENCUESTAS"))
                        {
                            tipoBusqueda = (short)TipoBusqueda.Encuestas;
                            break;
                        }
                        else if (pagina == utilIdiomasAux.GetText("URLSEM", "RECURSOS"))
                        {
                            tipoBusqueda = (short)TipoBusqueda.Recursos;
                            break;
                        }
                    }

                    string parametrosAdicionales = "";
                    if (ControllerBase.ProyectoOrigenBusquedaID != Guid.Empty && pFilaProyGadget.ProyectoOrigenID == ControllerBase.ProyectoSeleccionado.Clave)
                    {
                        //Se trata de un proyecto de tipo vista, y el contexto es hacia si mismo (hacia el padre pero con los enlaces de la propia comunidad)
                        parametrosAdicionales = "proyectoOrigenID=" + ControllerBase.ProyectoOrigenBusquedaID + "|";
                    }

                    try
                    {
                        Guid identidadActualID = UsuarioAD.Invitado;
                        bool esUsuarioInvitado = false;
                        if (pFilaProyGadget.ObtenerPrivados)
                        {
                            identidadActualID = ControllerBase.IdentidadActual.Clave;
                            esUsuarioInvitado = ControllerBase.IdentidadActual.Clave == UsuarioAD.Invitado;
                        }

                        mLoggingService.AgregarEntrada("Llamada al servicio de resultados");

                        bool proyectoDeEcosistema = true;
                        if (!pFilaProyGadget.ProyectoOrigenID.Equals(ControllerBase.ProyectoSeleccionado.Clave))
                        {
                            ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
                            DataWrapperProyecto dataWrapperProyecto = proyectoCN.ObtenerProyectoCargaLigeraPorID(pFilaProyGadget.ProyectoOrigenID);
                            proyectoCN.Dispose();

                            if (dataWrapperProyecto.ListaProyecto.Count > 0)
                            {
                                try
                                {
                                    string urlpropiaPropioEcosistema = dataWrapperProyecto.ListaProyecto.First().URLPropia;
                                    EstadoProyecto estadoProyecto = (EstadoProyecto)dataWrapperProyecto.ListaProyecto.First().Estado;

                                    if (urlpropiaPropioEcosistema.Contains('@'))
                                    {
                                        urlpropiaPropioEcosistema = urlpropiaPropioEcosistema.Substring(0, urlpropiaPropioEcosistema.IndexOf('@'));
                                    }

                                    Uri uriPropioEcosistema = new Uri(urlpropiaPropioEcosistema);
                                    Uri uriContextoEcosistema = new Uri(pFilaProyGadget.ComunidadOrigen);
                                    if (estadoProyecto == EstadoProyecto.Cerrado || uriPropioEcosistema.Host != uriContextoEcosistema.Host)
                                    {
                                        mLoggingService.GuardarLog($"estadoProyecto: {estadoProyecto}, uriPropioEcosistema.Host {uriPropioEcosistema.Host}, uriContextoEcosistema.Host {uriContextoEcosistema.Host} Documento?.Clave {Documento?.Clave} pFilaProyGadget?.GadgetID {pFilaProyGadget?.GadgetID}", mlogger);
                                        proyectoDeEcosistema = false;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    mLoggingService.GuardarLogError(ex, $"No se ha podido concretar si el proyecto es del ecosistema o no  Documento?.Clave {Documento?.Clave} pFilaProyGadget?.GadgetID {pFilaProyGadget?.GadgetID}", mlogger);
                                    proyectoDeEcosistema = false;
                                }
                            }
                            else
                            {
                                mLoggingService.GuardarLog($"No se ha encontrado el proyecto de origen: dataWrapperProyecto.ListaProyecto.Count: {dataWrapperProyecto.ListaProyecto.Count}  Documento?.Clave {Documento?.Clave} pFilaProyGadget?.GadgetID {pFilaProyGadget?.GadgetID}", mlogger);
                                proyectoDeEcosistema = false;
                            }
                        }

                        if (!proyectoDeEcosistema)
                        {
                            //El proyecto es de otro ecosistema, hago la petición a su servicio de resultados
                            sw = LoggingService.IniciarRelojTelemetria();
                            filtroContexto = System.Web.HttpUtility.UrlEncode(filtroContexto).Replace("+", "%20");
                            listaRecursosContextos = cargadorResultadosContexto.CargarResultadosContexto(pFilaProyGadget.ProyectoID, "pagina=" + pPagina.ToString(), false, ControllerBase.UtilIdiomas.LanguageCode, (TipoBusqueda)tipoBusqueda, numRecursosPagina, pFilaProyGadget.ProyectoOrigenID.ToString(), rutaGadgetOrigen, filtroContexto, false, pFilaProyGadget.MostrarEnlaceOriginal, pFilaProyGadget.NamespacesExtra, pFilaProyGadget.ItemsBusqueda, docsAEliminar, pFilaProyGadget.NuevaPestanya, parametrosAdicionales, identidadActualID, esUsuarioInvitado);
                            mLoggingService.AgregarEntradaDependencia("Contexto externo: Llamar al servicio de resultados", false, "CargarRecursosContexto", sw, true);
                        }
                        else
                        {
                            mLoggingService.AgregarEntrada("Contexto interno, llamo a virtuoso");
                            listaRecursosContextos = CargarResultadosContexto(pFilaProyGadget.ProyectoID, "pagina=" + pPagina.ToString(), false, ControllerBase.UtilIdiomas.LanguageCode, tipoBusqueda, numRecursosPagina, pFilaProyGadget.ProyectoOrigenID.ToString(), rutaGadgetOrigen, filtroContexto, false, pFilaProyGadget.NamespacesExtra, pFilaProyGadget.ItemsBusqueda, docsAEliminar, parametrosAdicionales, identidadActualID, esUsuarioInvitado, pUsarAfinidad);
                            mLoggingService.AgregarEntrada("Fin carga contexto");
                        }

                        if (pPagina == 1 && listaRecursosContextos != null/* && listaRecursosContextos.Count > 0*/)
                        {
                            List<Guid> listaRecursos = new List<Guid>();
                            foreach (ResourceModel modeloRecurso in listaRecursosContextos)
                            {
                                if (!listaRecursos.Contains(modeloRecurso.Key))
                                {
                                    listaRecursos.Add(modeloRecurso.Key);
                                }
                            }
                            if (!pFilaProyGadget.ObtenerPrivados)
                            {
                                DocumentacionCL docCl = new DocumentacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCL>(), mLoggerFactory);
                                docCl.AgregarContextoRecursoMVC(Documento.Clave, ControllerBase.ProyectoSeleccionado.Clave, pFilaProyGadget.GadgetID, listaRecursos);
                                docCl.Dispose();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ControllerBase.GuardarLogErrorContextos("Error producido en la llamada al servicio de resultados(el error debería de estar en el servicio de resultados:" + cargadorResultadosContexto.Url + ") del contexto: " + pFilaProyGadget.GadgetID.ToString() + " del recurso: " + Documento.Clave.ToString() + " en la comunidad " + ControllerBase.ProyectoSeleccionado.Nombre + " \n " + mLoggingService.DevolverCadenaError(ex, ControllerBase.Version));
                        mLoggingService.AgregarEntradaDependencia($"Error producido en la llamada al servicio de resultados '{cargadorResultadosContexto.Url}' del contexto '{pFilaProyGadget.GadgetID}' del recurso '{Documento.Clave}' en la comunidad '{ControllerBase.ProyectoSeleccionado.Nombre}'", false, "CargarRecursosContexto", sw, false);
                    }

                    #endregion
                }
                else if (listaRecursosContextos == null)
                {
                    listaRecursosContextos = new List<ResourceModel>();
                }

            }
            catch (Exception ex)
            {
                if (mSoloGenerarRelacionados)
                {
                    throw;
                }
                ControllerBase.GuardarLogErrorContextos("Error pintando el contexto: " + pFilaProyGadget.GadgetID.ToString() + " del recurso: " + Documento.Clave.ToString() + " en la comunidad " + ControllerBase.ProyectoSeleccionado.Nombre + " \n " + mLoggingService.DevolverCadenaError(ex, ControllerBase.Version));
                mLoggingService.AgregarEntradaDependencia($"Error pintando el contexto '{pFilaProyGadget.GadgetID}' del recurso '{Documento.Clave}' en la comunidad '{ControllerBase.ProyectoSeleccionado.Nombre}'", false, "CargarRecursosContextos", sw, false);
            }

            return new KeyValuePair<string, List<ResourceModel>>(pNombreCortoGadget, listaRecursosContextos);
        }

        private KeyValuePair<string, List<ResourceModel>>? CargarTemasRelacionadosDidactalia(ProyectoGadget pFilaProyGadget)
        {
            try
            {
                if (Documento.TipoDocumentacion == TiposDocumentacion.Semantico && (Documento.ElementoVinculadoID == new Guid("9DFDFAA8-66C6-4AF3-9CCA-F6A0237AB5E3") || Documento.ElementoVinculadoID == new Guid("358b11d8-183d-4378-9c5b-f441df3a21a1") || Documento.Enlace.Equals("http://didactalia.net/Ontologia/tema.owl")))
                {
                    //nombreasignatura, otrotemaURL, order
                    FacetadoDS facetadoDS = new FacetadoDS();
                    //s o
                    FacetadoDS facetadoDS2 = new FacetadoDS();

                    #region Querys
                    string query = " select distinct ?nombreasignatura ?otrotemaURL ?order  from <" + ControllerBase.UrlIntragnoss + pFilaProyGadget.ProyectoID + "> where { ?asignatura <http://ltsc.ieee.org/rdf/lomv1p0/lom#title> ?nombreasignatura . ?asignatura <http://rdfs.org/sioc/ns#topic> ?tema.  ?tema  <" + ControllerBase.UrlIntragnoss + "Ontologia/asignatura.owl#topicURL> ?temaURL. filter(?temaURL LIKE '%" + Documento.Clave.ToString() + "') ?asignatura <http://rdfs.org/sioc/ns#topic> ?otrotema .  ?otrotema <" + ControllerBase.UrlIntragnoss + "Ontologia/asignatura.owl#topicURL> ?otrotemaURL. ?asignatura <http://rdfs.org/sioc/ns#topic> ?otrotema . ?otrotema <" + ControllerBase.UrlIntragnoss + "Ontologia/asignatura.owl#topicOrder> ?order. ?asignatura rdf:type 'asignatura'. } order by xsd:int(?order)";

                    FacetadoCN facetadoCN = new FacetadoCN(ControllerBase.UrlIntragnoss, ControllerBase.ProyectoSeleccionado.Clave.ToString(), mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetadoCN>(), mLoggerFactory);
                    facetadoCN.ObtenerResultadosBusqueda(query, ControllerBase.ProyectoSeleccionado.Clave.ToString(), facetadoDS, 0, 50, "");

                    List<Guid> listaIdsRecursos = new List<Guid>();
                    string nombreAsignatura = "";
                    foreach (DataRow fila in facetadoDS.Tables["RecursosBusqueda"].Rows)
                    {
                        nombreAsignatura = (string)fila["nombreAsignatura"];
                        Guid id = new Guid(((string)fila["otrotemaURL"]).Substring(((string)fila["otrotemaURL"]).LastIndexOf("/") + 1));
                        if (!listaIdsRecursos.Contains(id))
                        {
                            listaIdsRecursos.Add(id);
                        }
                    }
                    #endregion

                    #region ObtenemosModelos  

                    ControladorProyectoMVC controladorProyecto = new ControladorProyectoMVC(ControllerBase.UtilIdiomas, ControllerBase.BaseURL, ControllerBase.BaseURLContent, ControllerBase.BaseURLStatic, ControllerBase.ProyectoSeleccionado, ControllerBase.ParametrosGeneralesRow, mIdentidadUsuarioActual, EsBot, mLoggingService, mEntityContext, mConfigService, mHttpContextAccessor, mRedisCacheWrapper, mVirtuosoAD, mGnossCache, mEntityContextBASE, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorProyectoMVC>(), mLoggerFactory);

                    Dictionary<Guid, ResourceModel> listaRecursos = controladorProyecto.ObtenerRecursosPorID(listaIdsRecursos, null, null, false);

                    #endregion

                    KeyValuePair<string, List<ResourceModel>> listaRecursosDevolver = new KeyValuePair<string, List<ResourceModel>>(nombreAsignatura, new List<ResourceModel>());
                    if (listaRecursos != null && listaRecursos.Count > 0)
                    {
                        foreach (ResourceModel ficha in listaRecursos.Values)
                        {
                            listaRecursosDevolver.Value.Add(ficha);
                        }
                    }

                    return listaRecursosDevolver;
                }
            }
            catch (Exception ex)
            {
                ControllerBase.GuardarLogError("Error pintando el contexto: " + pFilaProyGadget.GadgetID.ToString() + " del recurso: " + Documento.Clave.ToString() + " en la comunidad " + ControllerBase.ProyectoSeleccionado.Nombre + " \n " + ex.ToString());
            }
            return null;
        }

        private string ObtenerUrlServicioResultados(ProyectoGadgetContexto filaGadgetContexto)
        {
            
            string urlServicioResultados = "";

            if (!string.IsNullOrEmpty(filaGadgetContexto.ServicioResultados))
            {
                //Si se especifica una URL en el servicio de resultados, se usa esta
                urlServicioResultados = filaGadgetContexto.ServicioResultados + "/CargadorResultados";
            }
            else
            {
                //Si no se especifica una URL la obtenemos de la ULR de la comunidad origen
                Guid proyectoOrigen = filaGadgetContexto.ProyectoOrigenID;
                Uri urlProyectoOrigen = new Uri(filaGadgetContexto.ComunidadOrigen);

                if (UrlServicioResultadosContextos == null)
                {
                    UrlServicioResultadosContextos = new Dictionary<Guid, string>();
                }

                if (UrlServicioResultadosContextos.ContainsKey(proyectoOrigen))
                {
                    //Si ya tenemos en la variable estática la URL la cogemos
                    urlServicioResultados = UrlServicioResultadosContextos[proyectoOrigen];
                }
                else
                {
                    string urlServicio = "";
                    //Obtener la URL del proyecto.
                    ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);
                    string urlPropiaProyecto = proyCL.ObtenerURLPropiaProyecto(proyectoOrigen, IdiomaUsuario);

                    if (string.IsNullOrEmpty(urlPropiaProyecto))
                    {
                        try
                        {
                            CallParametroConfiguracionService ParametrosConfiguracion = new CallParametroConfiguracionService();
                            ParametrosConfiguracion.Url = urlProyectoOrigen.Scheme + "://" + urlProyectoOrigen.Host + "/ParametrosConfiguracion.asmx";
                            //Obtenemos primero la URLPropia de la comundiad origen

                            string url = ParametrosConfiguracion.UrlPropiaProyecto(proyectoOrigen);
                            url = UtilCadenas.ObtenerTextoDeIdioma(url, "es", "es");
                            Uri urlPropia = new Uri(url);
                            if (urlPropia.Host != urlProyectoOrigen.Host)
                            {
                                //Si la URL propia es otra, la actualizamos
                                filaGadgetContexto.ComunidadOrigen = filaGadgetContexto.ComunidadOrigen.Replace(urlProyectoOrigen.Host, urlPropia.Host);
                                ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
                                proyCN.ActualizarProyectos();
                                proyCN.Dispose();

                                proyCL.InvalidarGadgetsProyecto(ControllerBase.ProyectoSeleccionado.Clave);
                                InvalidarCacheLocalGadget();
                            }
                            ParametrosConfiguracion.Url = urlPropia.Scheme + "://" + urlPropia.Host + "/ParametrosConfiguracion.asmx";
                            //Obtenemos la URL del servicio de resultados
                            urlServicio = ParametrosConfiguracion.UrlServicioResultados();
                        }
                        catch (WebException e)
                        {
                            string nuevaUrl = "";
                            using (WebResponse response = e.Response)
                            {
                                if (((HttpWebResponse)response).StatusCode == HttpStatusCode.MovedPermanently)
                                {
                                    nuevaUrl = ((HttpWebResponse)response).Headers["location"];
                                    try
                                    {
                                        CallParametroConfiguracionService ParametrosConfiguracion = new CallParametroConfiguracionService();
                                        ParametrosConfiguracion.Url = nuevaUrl;
                                        Uri urlPropia = new Uri(ParametrosConfiguracion.UrlPropiaProyecto(proyectoOrigen));
                                        nuevaUrl = urlPropia.Scheme + "://" + urlPropia.Host + "/ParametrosConfiguracion";
                                        ParametrosConfiguracion.Url = nuevaUrl;
                                        urlServicio = ParametrosConfiguracion.UrlServicioResultados();
                                    }
                                    catch (WebException)
                                    {
                                        CallParametroConfiguracionService ParametrosConfiguracion = new CallParametroConfiguracionService();
                                        ParametrosConfiguracion.Url = nuevaUrl;
                                        urlServicio = ParametrosConfiguracion.UrlServicioResultados();
                                    }
                                }
                            }

                            if (!string.IsNullOrEmpty(nuevaUrl))
                            {
                                Uri nuevaUri = new Uri(nuevaUrl);
                                filaGadgetContexto.ComunidadOrigen = filaGadgetContexto.ComunidadOrigen.Replace(urlProyectoOrigen.Host, nuevaUri.Host);

                                ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
                                proyCN.ActualizarProyectos();
                                proyCN.Dispose();

                                proyCL.InvalidarGadgetsProyecto(ControllerBase.ProyectoSeleccionado.Clave);
                                InvalidarCacheLocalGadget();
                            }
                            else
                            {
                                throw;
                            }
                        }
                    }
                    else
                    {
                        string url = UtilCadenas.ObtenerTextoDeIdioma(urlPropiaProyecto, "es", "es");
                        Uri urlPropia = new Uri(url);
                        if (urlPropia.Host != urlProyectoOrigen.Host)
                        {
                            //Si la URL propia es otra, la actualizamos
                            filaGadgetContexto.ComunidadOrigen = filaGadgetContexto.ComunidadOrigen.Replace(urlProyectoOrigen.Host, urlPropia.Host);
                            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
                            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();
                            proyCN.ActualizarProyectos();
                            proyCN.Dispose();

                            proyCL.InvalidarGadgetsProyecto(ControllerBase.ProyectoSeleccionado.Clave);
                            InvalidarCacheLocalGadget();
                        }
                        urlServicio = mConfigService.ObtenerUrlServicioResultados();
                    }

                    if (!string.IsNullOrEmpty(urlServicio))
                    {

                        urlServicioResultados = urlServicio;

                        if (!UrlServicioResultadosContextos.ContainsKey(proyectoOrigen))
                        {
                            UrlServicioResultadosContextos.Add(proyectoOrigen, urlServicio);
                        }
                        else
                        {
                            ControllerBase.GuardarLogErrorContextos("La variable estática no contenía la URL del servicio de resultados del proyecto:" + proyectoOrigen.ToString() + " pero tras la comprobación, ya estaba añadida; Si esto se repite es que se esta reciclando continuamente la Web.");
                        }
                    }
                }
            }

            return urlServicioResultados;
        }


        private void InvalidarCacheLocalGadget()
        {
            mGnossCache.VersionarCacheLocal(ControllerBase.ProyectoSeleccionado.Clave);
        }


        private List<Guid> obtenerElementosExcluidos(ElementoOntologia pElementoOntologia, List<string> pProps)
        {
            List<Guid> listaIDEliminar = new List<Guid>();


            if (pProps.Count > 1)
            {
                Propiedad propiedadSem = EstiloPlantilla.ObtenerPropiedadACualquierNivelPorNombre(pProps[0], pElementoOntologia);

                if (propiedadSem != null)
                {
                    List<string> pPropsAux = new List<string>();
                    int k = 0;
                    foreach (string prop in pProps)
                    {
                        if (k != 0)
                        {
                            pPropsAux.Add(prop);
                        }
                        k++;
                    }
                    if (propiedadSem.FunctionalProperty)
                    {
                        listaIDEliminar.AddRange(obtenerElementosExcluidos(propiedadSem.UnicoValor.Value, pPropsAux));
                    }
                    else
                    {
                        foreach (string key in propiedadSem.ListaValores.Keys)
                        {
                            listaIDEliminar.AddRange(obtenerElementosExcluidos(propiedadSem.ListaValores[key], pPropsAux));
                        }
                    }
                }
            }
            else
            {
                Propiedad propiedadSem = EstiloPlantilla.ObtenerPropiedadACualquierNivelPorNombre(pProps[0], pElementoOntologia);

                if (propiedadSem != null)
                {
                    if (propiedadSem.FunctionalProperty)
                    {
                        try
                        {
                            Guid idRecurso = new Guid(propiedadSem.UnicoValor.Key.Substring(propiedadSem.UnicoValor.Key.Length - 36));
                            listaIDEliminar.Add(idRecurso);
                        }
                        catch (Exception ex)
                        {
                            mLoggingService.GuardarLogError(ex, mlogger);
                        }
                    }
                    else
                    {
                        foreach (string key in propiedadSem.ListaValores.Keys)
                        {
                            try
                            {
                                Guid idRecurso = new Guid(key.Substring(key.Length - 36));
                                listaIDEliminar.Add(idRecurso);
                            }
                            catch (Exception ex)
                            {
                                mLoggingService.GuardarLogError(ex, mlogger);
                            }
                        }
                    }
                }
            }
            return listaIDEliminar;
        }



        private int ObtenerFinClausulaDeWhere(string pWhere, string pClausula, bool pBuscarAlFinal)
        {
            pWhere = pWhere.ToLower();
            pClausula = pClausula.ToLower();
            int indiceClausula = pWhere.IndexOf(pClausula);

            if (pBuscarAlFinal)
            {
                indiceClausula = pWhere.LastIndexOf(pClausula);
            }

            int indiceFinClausula = pWhere.IndexOf('}', indiceClausula);

            if (indiceClausula != -1 && indiceFinClausula != -1)
            {
                //compruebo si hay llaves dentro de la cláusula
                int indiceIntermedioLlave = pWhere.IndexOf('{', pWhere.IndexOf('{', indiceClausula) + 1);

                while (indiceIntermedioLlave != -1 && indiceIntermedioLlave < indiceFinClausula)
                {
                    // hay llaves dentro de la cláusula, busco la llave de fin correcta
                    // salimos del bucle cuando el índice intermedio de la llave es superior al de cierre
                    indiceIntermedioLlave = pWhere.IndexOf('{', indiceIntermedioLlave + 1);
                    indiceFinClausula = pWhere.IndexOf('}', indiceFinClausula + 1);
                }
            }
            return indiceFinClausula;
        }

        /// <summary>
        /// Carga los resultados de la consulta que indica el gadget.
        /// </summary>
        /// <param name="pFilaProyGadget">Fila gadget</param>
        private bool MostrarGadget(string pFiltroDestino)
        {
            if (!string.IsNullOrEmpty(pFiltroDestino))
            {
                while (pFiltroDestino.Contains("["))
                {
                    string exp = pFiltroDestino.Substring(pFiltroDestino.IndexOf('['), pFiltroDestino.IndexOf(']') - pFiltroDestino.IndexOf('[') + 1);

                    pFiltroDestino = pFiltroDestino.Replace(exp, CumplePropiedad(exp).ToString());
                }
            }

            if (string.IsNullOrEmpty(pFiltroDestino))
            {
                pFiltroDestino = "true";
            }


            System.Data.DataTable table = new System.Data.DataTable();
            table.Columns.Add("", typeof(Boolean));
            table.Columns[0].Expression = pFiltroDestino;

            System.Data.DataRow r = table.NewRow();
            table.Rows.Add(r);
            Boolean result = (Boolean)r[0];
            return result;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pPropiedadValor">Propiedad y valor separados por = </param>
        /// <returns></returns>
        private bool CumplePropiedad(string pPropiedadValor)
        {
            bool valorNulo = false;
            bool igual = true;
            string separador = "=";
            if (pPropiedadValor.Contains("!="))
            {
                igual = false;
                separador = "!=";
            }

            bool respuesta = false;

            string propiedad = pPropiedadValor.Substring(1, pPropiedadValor.IndexOf(separador) - 1);
            string valor = pPropiedadValor.Substring(pPropiedadValor.IndexOf(separador) + 1, pPropiedadValor.Length - pPropiedadValor.IndexOf(separador) - 2);

            //###VALORNULO###
            if (valor.ToUpper().Equals("###VALORNULO###"))
            {
                valorNulo = true;
                //Si buscamos el valor nulo, si no se encuentra nada se devuelve por defecto true
                respuesta = true;
            }

            if (propiedad.Equals("<http://rdfs.org/sioc/types#Tag>") || propiedad.Equals("sioc_t:Tag"))
            {
                if (valorNulo)
                {
                    respuesta = Documento.ListaTagsSoloLectura == null || Documento.ListaTagsSoloLectura.Count == 0;
                }
                else
                {
                    foreach (string tag in Documento.ListaTagsSoloLectura)
                    {
                        if (tag.ToLower() == valor.ToLower())
                        {
                            respuesta = true;
                        }
                    }
                }

            }
            else if (propiedad.Equals("<http://www.w3.org/2004/02/skos/core#ConceptID>") || propiedad.Equals("skos:ConceptID"))
            {
                if (valorNulo)
                {
                    respuesta = Documento.Categorias == null || Documento.Categorias.Count == 0;
                }
                else
                {
                    Guid catID;
                    if (!Guid.TryParse(valor, out catID) && valor.Contains(":"))
                    {
                        Guid.TryParse(valor.Substring(valor.IndexOf(":") + 1), out catID);
                    }

                    if (catID != Guid.Empty)
                    {
                        foreach (Guid cat in Documento.Categorias.Keys)
                        {
                            if (cat == catID)
                            {
                                respuesta = true;
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("Comprobar el Guid de la categoría del contexto, no cumple con el formato de un Guid o del texto gnoss:GUID '" + valor + "'");
                    }
                }
            }
            else if (propiedad.Equals("rdf:type"))
            {

                #region Obtener ontologías para enlaces

                string rdfTypeDoc = "";

                if (Documento.TipoDocumentacion == TiposDocumentacion.Semantico && Documento.FilaDocumento.ElementoVinculadoID.HasValue)
                {
                    if (Documento.GestorDocumental.ListaDocumentos.ContainsKey(Documento.FilaDocumento.ElementoVinculadoID.Value))
                    {
                        if (Documento.GestorDocumental.ListaDocumentos[Documento.FilaDocumento.ElementoVinculadoID.Value].Enlace.EndsWith(".owl"))
                        {
                            string enlace = Documento.GestorDocumental.ListaDocumentos[Documento.FilaDocumento.ElementoVinculadoID.Value].Enlace;
                            rdfTypeDoc = enlace.Substring(0, enlace.Length - 4);
                        }
                    }
                    else if (Documento.FilaDocumento.ElementoVinculadoID.HasValue)
                    {
                        List<Guid> listaGuidOntologias = new List<Guid>();
                        Dictionary<Guid, string> listaOntologias;
                        listaGuidOntologias.Add(Documento.FilaDocumento.ElementoVinculadoID.Value);

                        DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
                        listaOntologias = docCN.ObtenerEnlacesDocumentosPorDocumentoID(listaGuidOntologias);
                        docCN.Dispose();

                        foreach (Guid id in listaOntologias.Keys)
                        {
                            if (listaOntologias[id].EndsWith(".owl"))
                            {
                                rdfTypeDoc = listaOntologias[id].Substring(0, listaOntologias[id].Length - 4);
                            }
                        }
                    }
                }
                else
                {
                    if (Documento.TipoDocumentacion.Equals(TiposDocumentacion.Pregunta))
                    {
                        rdfTypeDoc = "Pregunta";
                    }
                    else if (Documento.TipoDocumentacion.Equals(TiposDocumentacion.Debate))
                    {
                        rdfTypeDoc = "Debate";
                    }
                    else if (Documento.TipoDocumentacion.Equals(TiposDocumentacion.Encuesta))
                    {
                        rdfTypeDoc = "Encuesta";
                    }
                    else
                    {
                        rdfTypeDoc = "Recurso";
                    }
                }

                #endregion

                Guid ontologiaID = Guid.Empty;
                if (Documento.TipoDocumentacion == TiposDocumentacion.Semantico && Documento.FilaDocumento.ElementoVinculadoID.HasValue)
                {
                    ontologiaID = Documento.FilaDocumento.ElementoVinculadoID.Value;
                    if (ontologiaID != Guid.Empty && rdfTypeDoc != "")
                    {
                        if (valor == rdfTypeDoc)
                        {
                            respuesta = true;
                        }
                    }
                }
                else
                {
                    if (rdfTypeDoc != "")
                    {
                        if (valor == rdfTypeDoc)
                        {
                            respuesta = true;
                        }
                    }
                }
            }
            else if (Documento.TipoDocumentacion == TiposDocumentacion.Semantico)
            {
                List<ElementoOntologia> entidadesOnto = new List<ElementoOntologia>();
                entidadesOnto.AddRange(GenPlantillasOWL.Entidades);

                if (propiedad.ToLower().Equals("gnoss:type"))
                {
                    Dictionary<string, List<string>> dicNombreAbreviatura = UtilServiciosFacetas.ObtenerInformacionOntologiasSinArroba(ControllerBase.UrlIntragnoss, ControllerBase.InformacionOntologias);

                    foreach (string abreviatura in dicNombreAbreviatura.Keys)
                    {
                        foreach (string ns in dicNombreAbreviatura[abreviatura])
                        {
                            if (valor.StartsWith(ns + ":"))
                            {
                                valor = valor.Replace(ns + ":", abreviatura);
                            }
                        }
                    }

                    respuesta = (entidadesOnto[0].TipoEntidad == valor);
                }
                else
                {

                    foreach (ElementoOntologia eleOnto in entidadesOnto)
                    {
                        //Tiene profundidad
                        Dictionary<int, string> props = new Dictionary<int, string>();

                        int i = 0;
                        foreach (string prop in propiedad.Split(new string[] { "@@@" }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            props.Add(i, prop);
                            i++;
                        }

                        for (int keyProp = 0; keyProp < props.Count; keyProp++)
                        {
                            string prop = props[keyProp];

                            if (prop.Contains(":"))
                            {
                                if (prop.Substring(0, prop.IndexOf(":")) == GenPlantillasOWL.Ontologia.GestorOWL.NamespaceOntologia)
                                {
                                    prop = prop.Substring(prop.IndexOf(":") + 1);
                                }
                                else
                                {
                                    foreach (string key in GenPlantillasOWL.Ontologia.NamespacesDefinidos.Keys)
                                    {
                                        if (GenPlantillasOWL.Ontologia.NamespacesDefinidos[key] == prop.Substring(0, prop.IndexOf(":")))
                                        {
                                            prop = key + prop.Substring(prop.IndexOf(":") + 1);
                                            break;
                                        }
                                    }
                                }

                                props[keyProp] = prop;
                            }
                        }


                        ElementoOntologia elemento = eleOnto;
                        for (int j = 0; j < props.Count; j++)
                        {
                            if (j < props.Count - 1)
                            {
                                Propiedad propiedadSem = elemento.ObtenerPropiedad(props[j]);

                                if (propiedadSem != null)
                                {
                                    if (propiedadSem.FunctionalProperty)
                                    {
                                        elemento = propiedadSem.UnicoValor.Value;
                                    }
                                    else
                                    {
                                        foreach (string key in propiedadSem.ListaValores.Keys)
                                        {
                                            elemento = propiedadSem.ListaValores[key];
                                        }
                                    }
                                }
                            }
                            else
                            {
                                Propiedad propiedadSem = elemento.ObtenerPropiedad(props[j]);
                                if (propiedadSem != null)
                                {
                                    if (propiedadSem.FunctionalProperty)
                                    {
                                        if (valorNulo)
                                        {
                                            respuesta = false;
                                        }
                                        else
                                        {
                                            if (propiedadSem.UnicoValor.Key.ToLower().Equals(valor.ToLower()))
                                            {
                                                respuesta = true;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        foreach (string key in propiedadSem.ListaValores.Keys)
                                        {
                                            if (valorNulo)
                                            {
                                                respuesta = false;
                                            }
                                            else
                                            {
                                                if (key.ToLower().Equals(valor.ToLower()))
                                                {
                                                    respuesta = true;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (igual)
            {
                return respuesta;
            }
            else
            {
                return !respuesta;
            }
        }

        private UtilServicioResultados mUtilServicioResultados;
        private UtilServicioResultados UtilServicioResultados
        {
            get
            {
                if (mUtilServicioResultados == null)
                {
                    mUtilServicioResultados = new UtilServicioResultados(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mAvailableServices, mLoggerFactory.CreateLogger<UtilServicioResultados>(), mLoggerFactory);
                }
                return mUtilServicioResultados;
            }
        }

        public List<ResourceModel> CargarResultadosContexto(Guid pProyectoID, string pParametros, bool pPrimeraCarga, string pLanguageCode, short pTipoBusqueda, int pNumRecursosPagina, string pGrafo, string pUrlPaginaBusqueda, string pFiltroContexto, bool pEsBot, string pNamespacesExtra, string pListaItemsBusqueda, string pResultadosEliminar, string pParametrosAdicionales, Guid pIdentidadID, bool pEsUsuarioInvitado, bool pUsarAfinidad = false)
        {
            CargadorResultadosModel cargadorResultadosModel = new CargadorResultadosModel(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<CargadorResultadosModel>(), mLoggerFactory);
            List<ResourceModel> listaRecursosDevolver = new List<ResourceModel>();
            Stopwatch sw = null;
            try
            {
                mLoggingService.AgregarEntrada("Empieza carga de contextos");
                TipoFichaResultados tipoFicha = TipoFichaResultados.Contexto;

                #region Obtenemos parámetros

                cargadorResultadosModel.EsContexto = true;
                cargadorResultadosModel.ProyectoSeleccionado = pProyectoID;
                cargadorResultadosModel.EsBot = pEsBot;

                cargadorResultadosModel.EsMyGnoss = pProyectoID == ProyectoAD.MetaProyecto;

                if (!string.IsNullOrEmpty(pNamespacesExtra))
                {
                    cargadorResultadosModel.NamespacesExtra = " " + pNamespacesExtra + " ";
                }
                if (!string.IsNullOrEmpty(pListaItemsBusqueda))
                {
                    string[] items = pListaItemsBusqueda.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string item in items)
                    {
                        cargadorResultadosModel.ListaItemsBusqueda.Add(item);
                    }
                }
                if (!string.IsNullOrEmpty(pResultadosEliminar))
                {
                    cargadorResultadosModel.ResultadosEliminar = pResultadosEliminar;
                }


                Dictionary<string, string> diccionarioPestanyas = new Dictionary<string, string>();
                foreach (ProyectoPestanyaBusqueda filaPestaña in cargadorResultadosModel.PestanyasProyecto.ListaProyectoPestanyaBusqueda)
                {
                    if (!string.IsNullOrEmpty(filaPestaña.ProyectoPestanyaMenu.Ruta))
                    {
                        diccionarioPestanyas.Add(filaPestaña.ProyectoPestanyaMenu.Ruta.ToLower(), filaPestaña.CampoFiltro);
                    }
                }

                string paginaBusqueda = pUrlPaginaBusqueda.ToLower().Substring(pUrlPaginaBusqueda.LastIndexOf("/") + 1);

                var filtrosPestaña = diccionarioPestanyas.FirstOrDefault(p => p.Key.StartsWith(paginaBusqueda + "@") || p.Key.Contains("|||" + paginaBusqueda + "@"));
                if (!string.IsNullOrEmpty(filtrosPestaña.Key))
                {
                    pParametrosAdicionales += filtrosPestaña.Value;
                }

                #endregion

                #region Buscamos resultados

                if (cargadorResultadosModel.IdentidadActual == null)
                {
                    cargadorResultadosModel.IdentidadActual = ObtenerIdentidadUsuarioInvitado(ControllerBase.UtilIdiomas).ListaIdentidades[UsuarioAD.Invitado];
                }

                bool esMovil = RequestParams("esMovil") == "true";
                sw = LoggingService.IniciarRelojTelemetria();

                UtilServicioResultados.CargarResultadosInt(pProyectoID, pIdentidadID, pIdentidadID != UsuarioAD.Invitado, pEsUsuarioInvitado, (TipoBusqueda)pTipoBusqueda, pGrafo, pParametros, pParametrosAdicionales, pPrimeraCarga, pLanguageCode, -1, pNumRecursosPagina, tipoFicha, pFiltroContexto, false, cargadorResultadosModel, esMovil, false, pUsarAfinidad);
                mLoggingService.AgregarEntradaDependencia("Llamar al servicio de resultados para la carga de contextos", false, "CargarResultadosContexto", sw, true);
                #endregion

                #region Cargamos los resultados
                List<Guid> listaRecursos = new List<Guid>();
                foreach (string idResultado in cargadorResultadosModel.ListaIdsResultado.Keys)
                {
                    switch (cargadorResultadosModel.ListaIdsResultado[idResultado])
                    {
                        case TiposResultadosMetaBuscador.Documento:
                        case TiposResultadosMetaBuscador.DocumentoBRPrivada:
                        case TiposResultadosMetaBuscador.DocumentoBRPersonal:
                        case TiposResultadosMetaBuscador.Pregunta:
                        case TiposResultadosMetaBuscador.Debate:
                        case TiposResultadosMetaBuscador.Encuesta:
                            listaRecursos.Add(new Guid(idResultado));
                            break;
                    }
                }

                string baseUrlBusqueda = "";
                if (!string.IsNullOrEmpty(pUrlPaginaBusqueda))
                {
                    baseUrlBusqueda = pUrlPaginaBusqueda;
                }
                else if (mHttpContextAccessor.HttpContext.Request.PathBase != null)
                {
                    baseUrlBusqueda = mHttpContextAccessor.HttpContext.Request.PathBase.ToString();
                }
                if (baseUrlBusqueda.Contains("?"))
                {
                    baseUrlBusqueda = baseUrlBusqueda.Substring(0, baseUrlBusqueda.IndexOf("?"));
                }
                ControladorProyectoMVC controladorMVC = new ControladorProyectoMVC(ControllerBase.UtilIdiomas, ControllerBase.BaseURL, ControllerBase.BaseURLContent, ControllerBase.BaseURLStatic, cargadorResultadosModel.Proyecto, cargadorResultadosModel.FilaParametroGeneral, mIdentidadUsuarioActual, EsBot, mLoggingService, mEntityContext, mConfigService, mHttpContextAccessor, mRedisCacheWrapper, mVirtuosoAD, mGnossCache, mEntityContextBASE, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorProyectoMVC>(), mLoggerFactory);

                #region Recursos
                Dictionary<Guid, ResourceModel> listaResultados = controladorMVC.ObtenerRecursosPorID(listaRecursos, baseUrlBusqueda, null, true);
                UtilServicioResultados.ProcesarFichasRecursoParaPresentacion(listaResultados);
                #endregion


                #endregion

                foreach (Guid id in listaResultados.Keys)
                {
                    listaRecursosDevolver.Add(listaResultados[id]);
                }
                mLoggingService.AgregarEntrada("Carga de contexto finalizada");
            }
            catch (Exception ex)
            {
                string url = mHttpContextAccessor.HttpContext.Request.Path.ToString();
                if (!url.Contains("?"))
                {
                    url += "?" + mHttpContextAccessor.HttpContext.Request.Query.ToString();
                }
                mLoggingService.AgregarEntradaDependencia($"Error en la llamada al servicio de resultados para la carga de contextos. Mensaje: {ex.Message} Llamada: {url}", false, "CargarResultadosContexto", sw, false);
                mLoggingService.GuardarLogError(ex, mlogger);
                return null;
                //UtilServicios.EnviarErrorYGuardarLog("Error: " + ex.Message + "\r\nPila: " + ex.StackTrace + "\r\nLlamada: " + url, "errorBots", cargadorResultadosModel.EsBot);
            }

            return listaRecursosDevolver;
        }

        /// <summary>
        /// Gestor de facetas de la comunidad
        /// </summary>
        private GestionFacetas GestorFacetasComunidad
        {
            get
            {
                if (mGestorFacetas == null)
                {
                    FacetaCL facetaCL = new FacetaCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetaCL>(), mLoggerFactory);
                    DataWrapperFacetas dataWrapperFacetas = new DataWrapperFacetas();
                    dataWrapperFacetas = facetaCL.ObtenerFacetasDeProyecto(null, ControllerBase.ProyectoSeleccionado.Clave, false);
                    if (dataWrapperFacetas == null)
                    {
                        dataWrapperFacetas = new DataWrapperFacetas();
                    }

                    mGestorFacetas = new GestionFacetas(dataWrapperFacetas);
                }
                return mGestorFacetas;
            }
        }
    }
}
