using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.CMS;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models;
using Es.Riam.Gnoss.AD.EntityModel.Models.Faceta;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.Parametro;
using Es.Riam.Gnoss.AD.ParametrosProyecto;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.CMS;
using Es.Riam.Gnoss.CL.Facetado;
using Es.Riam.Gnoss.CL.ParametrosProyecto;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.CMS;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.CMS;
using Es.Riam.Gnoss.Logica.Documentacion;
using Es.Riam.Gnoss.Logica.ExportacionBusqueda;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Recursos;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Web;

namespace Es.Riam.Gnoss.Web.Controles.Administracion
{
    public class ControladorPestanyas : ControladorBase
    {
        /// <summary>
        /// 
        /// </summary>
        private List<short> mListaPaginasCMS = null;

        private Elementos.ServiciosGenerales.Proyecto ProyectoSeleccionado = null;
        private GestionProyecto GestionProyectos = null;
        private Dictionary<string, string> ParametroProyecto = null;
        private GestionCMS mGestorCMS = null;
        private DataWrapperExportacionBusqueda mExportacionBusquedaDW = null;
        private DataWrapperFacetas mFacetasDW = null;
        private bool CrearFilasPropiedadesExportacion = false;
        private List<IntegracionContinuaPropiedad> propiedadesIntegracionContinua = null;

        private LoggingService mLoggingService;
        private VirtuosoAD mVirtuosoAD;
        private EntityContext mEntityContext;
        private ConfigService mConfigService;
        private RedisCacheWrapper mRedisCacheWrapper;
        private GnossCache mGnossCache;

        #region Constructor

        /// <summary>
        /// 
        /// </summary>
        public ControladorPestanyas(Elementos.ServiciosGenerales.Proyecto pProyecto, Dictionary<string, string> pParametroProyecto, LoggingService loggingService, EntityContext entityContext, ConfigService configService, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, bool pCrearFilasPropiedadesExportacion = false)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, servicesUtilVirtuosoAndReplication)
        {
            mVirtuosoAD = virtuosoAD;
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;
            mRedisCacheWrapper = redisCacheWrapper;
            mGnossCache = gnossCache;

            ProyectoSeleccionado = pProyecto;
            GestionProyectos = ProyectoSeleccionado.GestorProyectos;
            ParametroProyecto = pParametroProyecto;

            CrearFilasPropiedadesExportacion = pCrearFilasPropiedadesExportacion;
        }

        #endregion

        #region Metodos de Carga

        public TabModel CargarPestanya(AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu filaPestanya)
        {
            TabModel pestanya = new TabModel();

            pestanya.Active = filaPestanya.Activa;
            pestanya.Key = filaPestanya.PestanyaID;
            pestanya.Type = (TipoPestanyaMenu)filaPestanya.TipoPestanya;
            pestanya.Privacidad = filaPestanya.Privacidad;
            pestanya.HtmlAlternativoPrivacidad = filaPestanya.HtmlAlternativo;
            pestanya.MetaDescription = filaPestanya.MetaDescription;
            propiedadesIntegracionContinua = new List<IntegracionContinuaPropiedad>();

            if (filaPestanya.Privacidad.Equals(2))
            {
                List<Guid> listaGrupos = new List<Guid>();
                List<Guid> listaPerfiles = new List<Guid>();

                List<AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenuRolGrupoIdentidades> filasGruposIdentidades = filaPestanya.ProyectoPestanyaMenuRolGrupoIdentidades.ToList();

                foreach (AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenuRolGrupoIdentidades filaGrupoIdentidad in filasGruposIdentidades)
                {
                    if (!listaGrupos.Contains(filaGrupoIdentidad.GrupoID))
                    {
                        listaGrupos.Add(filaGrupoIdentidad.GrupoID);
                    }
                }

                List<AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenuRolIdentidad> filasIdentidades = filaPestanya.ProyectoPestanyaMenuRolIdentidad.ToList();
                foreach (AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenuRolIdentidad filaIdentidad in filasIdentidades)
                {
                    if (!listaPerfiles.Contains(filaIdentidad.PerfilID))
                    {
                        listaPerfiles.Add(filaIdentidad.PerfilID);
                    }
                }

                IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                pestanya.PrivacidadGrupos = identidadCN.ObtenerNombresDeGrupos(listaGrupos);
                pestanya.PrivacidadPerfiles = identidadCN.ObtenerNombresDePerfiles(listaPerfiles);
                identidadCN.Dispose();
            }

            if (filaPestanya.TipoPestanya.Equals((short)TipoPestanyaMenu.Home))
            {
                Dictionary<short, CMSPagina> listaPaginas = new Dictionary<short, CMSPagina>();
                if (GestorCMS.ListaPaginasProyectos.ContainsKey(ProyectoSeleccionado.Clave))
                {
                    listaPaginas = GestorCMS.ListaPaginasProyectos[ProyectoSeleccionado.Clave];
                }

                pestanya.HomeCMS = new TabModel.HomeCMTabSModel();
                pestanya.HomeCMS.HomeTodosUsuarios = listaPaginas.ContainsKey((short)TipoUbicacionCMS.HomeProyecto);
                pestanya.HomeCMS.HomeMiembros = listaPaginas.ContainsKey((short)TipoUbicacionCMS.HomeProyectoMiembro);
                pestanya.HomeCMS.HomeNoMiembros = listaPaginas.ContainsKey((short)TipoUbicacionCMS.HomeProyectoNoMiembro);
            }

            pestanya.Name = filaPestanya.Nombre;
            pestanya.Url = filaPestanya.Ruta;

            if (CrearFilasPropiedadesExportacion && filaPestanya.TipoPestanya.Equals((short)TipoPestanyaMenu.EnlaceExterno) && !string.IsNullOrEmpty(filaPestanya.Ruta))
            {
                //Crear las filas de las porpiedades de Integracion Continua
                IntegracionContinuaPropiedad propiedadRutaPagina = new IntegracionContinuaPropiedad();
                propiedadRutaPagina.ProyectoID = ProyectoSeleccionado.Clave;
                propiedadRutaPagina.TipoObjeto = (short)TipoObjeto.Pagina;
                propiedadRutaPagina.ObjetoPropiedad = filaPestanya.NombreCortoPestanya;
                propiedadRutaPagina.TipoPropiedad = (short)TipoPropiedad.RutaPagina;
                propiedadRutaPagina.ValorPropiedad = filaPestanya.Ruta;
                propiedadesIntegracionContinua.Add(propiedadRutaPagina);
            }

            if (string.IsNullOrEmpty(pestanya.Name) || string.IsNullOrEmpty(pestanya.Url))
            {
                //KeyValuePair<string, string> nameUrl = ObtenerNameUrlPestanya(filaPestanya, true);

                if (string.IsNullOrEmpty(pestanya.Name))
                {
                    //pestanya.Name = nameUrl.Key;
                    pestanya.EsNombrePorDefecto = true;
                }
                if (string.IsNullOrEmpty(pestanya.Url))
                {
                    //pestanya.Url = nameUrl.Value;
                    pestanya.EsUrlPorDefecto = true;
                }
            }

            pestanya.ShortName = filaPestanya.NombreCortoPestanya;
            pestanya.ClassCSSBody = filaPestanya.CSSBodyClass;
            pestanya.OpenInNewWindow = filaPestanya.NuevaPestanya;
            pestanya.Visible = filaPestanya.Visible;
            pestanya.VisibleSinAcceso = filaPestanya.VisibleSinAcceso;

            pestanya.ParentTabKey = Guid.Empty;
            if (filaPestanya.PestanyaPadreID.HasValue)
            {
                pestanya.ParentTabKey = filaPestanya.PestanyaPadreID.Value;
            }

            pestanya.Order = filaPestanya.Orden;

            bool esPestanyaBusqueda = filaPestanya.TipoPestanya == (short)TipoPestanyaMenu.Recursos || filaPestanya.TipoPestanya == (short)TipoPestanyaMenu.Preguntas || filaPestanya.TipoPestanya == (short)TipoPestanyaMenu.Debates || filaPestanya.TipoPestanya == (short)TipoPestanyaMenu.Encuestas || filaPestanya.TipoPestanya == (short)TipoPestanyaMenu.PersonasYOrganizaciones || filaPestanya.TipoPestanya == (short)TipoPestanyaMenu.BusquedaSemantica || filaPestanya.TipoPestanya == (short)TipoPestanyaMenu.BusquedaAvanzada;

            if (esPestanyaBusqueda)
            {
                pestanya.OpcionesBusqueda = CargarOpcionesBusqueda(filaPestanya);
                pestanya.ListaExportaciones = CargarExportacionesBusqueda(filaPestanya);
            }

            if (filaPestanya.TipoPestanya.Equals((short)TipoPestanyaMenu.CMS) && ParametroProyecto.ContainsKey(ParametroAD.PropiedadContenidoMultiIdioma))
            {
                pestanya.ListaIdiomasDisponibles = new List<string>();

                List<string> listaIdiomas = mConfigService.ObtenerListaIdiomas();
                foreach (string idioma in listaIdiomas)
                {
                    if (string.IsNullOrEmpty(filaPestanya.IdiomasDisponibles) || UtilCadenas.ObtenerTextoDeIdioma(filaPestanya.IdiomasDisponibles, idioma, null, true) == "true")
                    {
                        pestanya.ListaIdiomasDisponibles.Add(idioma);
                    }
                }
            }

            if (CrearFilasPropiedadesExportacion)
            {
                try
                {
                    ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    proyCN.CrearFilasIntegracionContinuaParametro(propiedadesIntegracionContinua, ProyectoSeleccionado.Clave, TipoObjeto.Pagina, filaPestanya.NombreCortoPestanya);
                    proyCN.Dispose();
                }
                catch
                {

                }
            }
            List<FacetaObjetoConocimientoProyectoPestanya> facetasPestanya = ObtenerFacetaObjetoConocimientoProyectoPestanya(filaPestanya.PestanyaID);
            List<TabModel.FacetasTabModel> lstFacetasTabModel = new List<TabModel.FacetasTabModel>();

            foreach (FacetaObjetoConocimientoProyectoPestanya item in facetasPestanya)
            {
                TabModel.FacetasTabModel fcTM = new TabModel.FacetasTabModel();
                fcTM.ClavePestanya = item.PestanyaID;
                fcTM.Faceta = item.Faceta;
                fcTM.ObjetoConocimiento = item.ObjetoConocimiento;
                lstFacetasTabModel.Add(fcTM);
            }
            pestanya.ListaFacetas = lstFacetasTabModel;

            return pestanya;
        }

        private TabModel.SearchTabModel CargarOpcionesBusqueda(AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu filaPestanya)
        {
            TabModel.SearchTabModel opcionesBusqueda;

            TabModel.SearchTabModel opcionesBusquedaDefecto = CargarOpcionesBusquedaPorDefecto((TipoPestanyaMenu)filaPestanya.TipoPestanya);

            ProyectoPestanyaBusqueda filasBusqueda = filaPestanya.ProyectoPestanyaBusqueda;
            if (filasBusqueda != null)
            {
                opcionesBusqueda = new TabModel.SearchTabModel();

                ProyectoPestanyaBusqueda filaPestanyaBusqueda = filasBusqueda;

                opcionesBusqueda.ValoresPorDefecto = true;

                string campoFiltro = filaPestanyaBusqueda.CampoFiltro;

                switch ((TipoPestanyaMenu)filaPestanya.TipoPestanya)
                {
                    case TipoPestanyaMenu.Recursos:
                        campoFiltro = "rdf:type=Recurso";
                        break;
                    case TipoPestanyaMenu.Debates:
                        campoFiltro = "rdf:type=Debate";
                        break;
                    case TipoPestanyaMenu.Preguntas:
                        campoFiltro = "rdf:type=Pregunta";
                        break;
                    case TipoPestanyaMenu.Encuestas:
                        campoFiltro = "rdf:type=Encuesta";
                        break;
                    case TipoPestanyaMenu.PersonasYOrganizaciones:
                        campoFiltro = "rdf:type=PerYOrg";
                        break;
                    case TipoPestanyaMenu.BusquedaAvanzada:
                        campoFiltro = "rdf:type=Meta";
                        break;
                }
                opcionesBusqueda.CampoFiltro = campoFiltro;
                opcionesBusqueda.GruposConfiguracion = filaPestanyaBusqueda.GruposConfiguracion;

                if (CrearFilasPropiedadesExportacion)
                {
                    if (!string.IsNullOrEmpty(opcionesBusqueda.CampoFiltro) && opcionesBusqueda.CampoFiltro.Contains("skos:ConceptID"))
                    {
                        //Crear las filas de las porpiedades de Integracion Continua
                        IntegracionContinuaPropiedad propiedadRutaPagina = new IntegracionContinuaPropiedad();
                        propiedadRutaPagina.ProyectoID = ProyectoSeleccionado.Clave;
                        propiedadRutaPagina.TipoObjeto = (short)TipoObjeto.Pagina;
                        propiedadRutaPagina.ObjetoPropiedad = filaPestanya.NombreCortoPestanya;
                        propiedadRutaPagina.TipoPropiedad = (short)TipoPropiedad.CampoFiltroPagina;
                        propiedadRutaPagina.ValorPropiedad = filasBusqueda.CampoFiltro;
                        propiedadesIntegracionContinua.Add(propiedadRutaPagina);
                    }
                }

                opcionesBusqueda.FiltrosOrden = new List<TabModel.SearchTabModel.FiltroOrden>();

                List<ProyectoPestanyaFiltroOrdenRecursos> filasFiltroOrden = filaPestanya.ProyectoPestanyaFiltroOrdenRecursos.ToList();
                if (filasFiltroOrden.Any())
                {
                    foreach (ProyectoPestanyaFiltroOrdenRecursos filaFiltroOrden in filasFiltroOrden.OrderBy(filtroOrden => filtroOrden.Orden))
                    {
                        TabModel.SearchTabModel.FiltroOrden filtroOrden = new TabModel.SearchTabModel.FiltroOrden();
                        filtroOrden.Orden = filaFiltroOrden.Orden;
                        filtroOrden.Filtro = filaFiltroOrden.FiltroOrden;
                        filtroOrden.Nombre = filaFiltroOrden.NombreFiltro;

                        opcionesBusqueda.FiltrosOrden.Add(filtroOrden);
                    }
                }

                opcionesBusqueda.MostrarEnBusquedaCabecera = filaPestanyaBusqueda.MostrarEnComboBusqueda;
                opcionesBusqueda.AgruparFacetasPorTipo = filaPestanyaBusqueda.GruposPorTipo;

                if (!filaPestanyaBusqueda.ProyectoOrigenID.HasValue)
                {
                    opcionesBusqueda.ProyectoOrigenBusqueda = Guid.Empty;
                }
                else
                {
                    opcionesBusqueda.ProyectoOrigenBusqueda = filaPestanyaBusqueda.ProyectoOrigenID.Value;
                }

                opcionesBusqueda.MostrarFacetas = filaPestanyaBusqueda.MostrarFacetas;
                opcionesBusqueda.MostrarCajaBusqueda = filaPestanyaBusqueda.MostrarCajaBusqueda;
                opcionesBusqueda.NumeroResultados = filaPestanyaBusqueda.NumeroRecursos;
                opcionesBusqueda.OcultarResultadosSinFiltros = filaPestanyaBusqueda.OcultarResultadosSinFiltros;
                opcionesBusqueda.TextoBusquedaSinResultados = (string.IsNullOrEmpty(filaPestanyaBusqueda.TextoBusquedaSinResultados) ? "" : filaPestanyaBusqueda.TextoBusquedaSinResultados);

                opcionesBusqueda.IgnorarPrivacidadEnBusqueda = filaPestanyaBusqueda.IgnorarPrivacidadEnBusqueda;
                opcionesBusqueda.OmitirCargaInicialFacetasResultados = filaPestanyaBusqueda.OmitirCargaInicialFacetasResultados;

                opcionesBusqueda.OpcionesVistas = CargarVistasDisponibles(filaPestanyaBusqueda);
                opcionesBusqueda.OpcionesVistas.PosicionCentralMapa = filaPestanyaBusqueda.PosicionCentralMapa;

                if (!CompararObjetos(opcionesBusqueda, opcionesBusquedaDefecto))
                {
                    opcionesBusqueda.ValoresPorDefecto = false;
                }
            }
            else
            {
                opcionesBusqueda = opcionesBusquedaDefecto;
            }

            return opcionesBusqueda;
        }


        public void CrearFilasPropiedadesIntegracionContinua(List<TabModel> pListaPestanyas)
        {
            foreach (TabModel pestanya in pListaPestanyas)
            {
                try
                {
                    propiedadesIntegracionContinua = new List<IntegracionContinuaPropiedad>();
                    if (pestanya.Modified)
                    {
                        if (pestanya.Type.Equals(TipoPestanyaMenu.EnlaceExterno) && !string.IsNullOrEmpty(pestanya.Url))
                        {
                            //Crear las filas de las porpiedades de Integracion Continua
                            IntegracionContinuaPropiedad propiedadRutaPagina = new IntegracionContinuaPropiedad();
                            propiedadRutaPagina.ProyectoID = ProyectoSeleccionado.Clave;
                            propiedadRutaPagina.TipoObjeto = (short)TipoObjeto.Pagina;
                            propiedadRutaPagina.ObjetoPropiedad = pestanya.ShortName;
                            propiedadRutaPagina.TipoPropiedad = (short)TipoPropiedad.RutaPagina;
                            propiedadRutaPagina.ValorPropiedad = pestanya.Url;
                            propiedadesIntegracionContinua.Add(propiedadRutaPagina);
                            //Encapsulamos propiedad
                            //pestanya.Url = UtilIntegracionContinua.ObtenerMascaraPropiedad(propiedadRutaPagina);
                        }
                        if (pestanya.OpcionesBusqueda != null && !string.IsNullOrEmpty(pestanya.OpcionesBusqueda.CampoFiltro) && pestanya.OpcionesBusqueda.CampoFiltro.Contains("skos:ConceptID"))
                        {
                            //Crear las filas de las porpiedades de Integracion Continua
                            IntegracionContinuaPropiedad propiedadRutaPagina = new IntegracionContinuaPropiedad();
                            propiedadRutaPagina.ProyectoID = ProyectoSeleccionado.Clave;
                            propiedadRutaPagina.TipoObjeto = (short)TipoObjeto.Pagina;
                            propiedadRutaPagina.ObjetoPropiedad = pestanya.ShortName;
                            propiedadRutaPagina.TipoPropiedad = (short)TipoPropiedad.CampoFiltroPagina;
                            propiedadRutaPagina.ValorPropiedad = pestanya.OpcionesBusqueda.CampoFiltro;
                            propiedadesIntegracionContinua.Add(propiedadRutaPagina);
                            //Encapsulamos propiedad
                            //pestanya.OpcionesBusqueda.CampoFiltro = UtilIntegracionContinua.ObtenerMascaraPropiedad(propiedadRutaPagina);
                        }
                    }
                    using (ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication))
                    {
                        proyCN.CrearFilasIntegracionContinuaParametro(propiedadesIntegracionContinua, ProyectoSeleccionado.Clave, TipoObjeto.Pagina, pestanya.ShortName);
                    }
                }
                catch
                {

                }
            }
        }


        public void ModificarFilasIntegracionContinuaEntornoSiguiente(List<TabModel> pListaPestanyas, string UrlApiDesplieguesEntornoSeleccionado, Guid pUsuarioID)
        {
            propiedadesIntegracionContinua = new List<IntegracionContinuaPropiedad>();
            foreach (TabModel pestanya in pListaPestanyas)
            {
                try
                {

                    //if (pestanya.Modified)
                    //{
                    if (pestanya.Type.Equals(TipoPestanyaMenu.EnlaceExterno) && !string.IsNullOrEmpty(pestanya.Url))
                    {
                        //Crear las filas de las porpiedades de Integracion Continua
                        IntegracionContinuaPropiedad propiedadRutaPagina = new IntegracionContinuaPropiedad();
                        propiedadRutaPagina.ProyectoID = ProyectoSeleccionado.Clave;
                        propiedadRutaPagina.TipoObjeto = (short)TipoObjeto.Pagina;
                        propiedadRutaPagina.ObjetoPropiedad = pestanya.ShortName;
                        propiedadRutaPagina.TipoPropiedad = (short)TipoPropiedad.RutaPagina;
                        propiedadRutaPagina.ValorPropiedad = pestanya.Url;
                        propiedadesIntegracionContinua.Add(propiedadRutaPagina);
                    }
                    if (pestanya.OpcionesBusqueda != null && !string.IsNullOrEmpty(pestanya.OpcionesBusqueda.CampoFiltro) && pestanya.OpcionesBusqueda.CampoFiltro.Contains("skos:ConceptID"))
                    {
                        //Crear las filas de las porpiedades de Integracion Continua
                        IntegracionContinuaPropiedad propiedadRutaPagina = new IntegracionContinuaPropiedad();
                        propiedadRutaPagina.ProyectoID = ProyectoSeleccionado.Clave;
                        propiedadRutaPagina.TipoObjeto = (short)TipoObjeto.Pagina;
                        propiedadRutaPagina.ObjetoPropiedad = pestanya.ShortName;
                        propiedadRutaPagina.TipoPropiedad = (short)TipoPropiedad.CampoFiltroPagina;
                        propiedadRutaPagina.ValorPropiedad = pestanya.OpcionesBusqueda.CampoFiltro;
                        propiedadesIntegracionContinua.Add(propiedadRutaPagina);
                    }
                    //}
                }
                catch
                {

                }

            }
            try
            {
                string peticion = $"{UrlApiDesplieguesEntornoSeleccionado}/PropiedadesIntegracion?nombreProy={ProyectoSeleccionado.NombreCorto}&UsuarioID={pUsuarioID}";
                string requestParameters = UtilWeb.WebRequestPostWithJsonObject(peticion, propiedadesIntegracionContinua, "");
            }
            catch
            {

            }
        }

        public TabModel.SearchTabModel CargarOpcionesBusquedaPorDefecto(TipoPestanyaMenu pTipoPestanya)
        {
            TabModel.SearchTabModel opcionesBusqueda = new TabModel.SearchTabModel();

            string campoFiltro = "";

            switch (pTipoPestanya)
            {
                case TipoPestanyaMenu.Recursos:
                    campoFiltro = "rdf:type=Recurso";
                    break;
                case TipoPestanyaMenu.Debates:
                    campoFiltro = "rdf:type=Debate";
                    break;
                case TipoPestanyaMenu.Preguntas:
                    campoFiltro = "rdf:type=Pregunta";
                    break;
                case TipoPestanyaMenu.Encuestas:
                    campoFiltro = "rdf:type=Encuesta";
                    break;
                case TipoPestanyaMenu.PersonasYOrganizaciones:
                    campoFiltro = "rdf:type=PerYOrg";
                    break;
                case TipoPestanyaMenu.BusquedaAvanzada:
                    campoFiltro = "rdf:type=Meta";
                    break;
            }

            opcionesBusqueda.ProyectoOrigenBusqueda = Guid.Empty;

            opcionesBusqueda.ValoresPorDefecto = true;
            opcionesBusqueda.CampoFiltro = campoFiltro;

            opcionesBusqueda.FiltrosOrden = new List<TabModel.SearchTabModel.FiltroOrden>();

            opcionesBusqueda.MostrarEnBusquedaCabecera = true;
            opcionesBusqueda.AgruparFacetasPorTipo = false;

            opcionesBusqueda.MostrarFacetas = true;
            opcionesBusqueda.MostrarCajaBusqueda = true;

            opcionesBusqueda.NumeroResultados = 10;
            opcionesBusqueda.OcultarResultadosSinFiltros = false;

            opcionesBusqueda.IgnorarPrivacidadEnBusqueda = false;
            opcionesBusqueda.OmitirCargaInicialFacetasResultados = false;

            opcionesBusqueda.OpcionesVistas = new TabModel.SearchTabModel.ViewsSearchTabModel();
            opcionesBusqueda.OpcionesVistas.VistaPorDefecto = 0;
            opcionesBusqueda.OpcionesVistas.VistaListado = true;
            opcionesBusqueda.OpcionesVistas.VistaMosaico = false;
            opcionesBusqueda.OpcionesVistas.VistaMapa = false;
            opcionesBusqueda.OpcionesVistas.VistaGrafico = false;
            opcionesBusqueda.OpcionesVistas.PosicionCentralMapa = null;

            return opcionesBusqueda;
        }

        private List<TabModel.ExportacionSearchTabModel> CargarExportacionesBusqueda(AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu filaPestanya)
        {
            List<TabModel.ExportacionSearchTabModel> listaExportaciones = new List<TabModel.ExportacionSearchTabModel>();

            foreach (ProyectoPestanyaBusquedaExportacion filaExport in ExpBusquedaDW.ListaProyectoPestanyaBusquedaExportacion.Where(exportacion => exportacion.PestanyaID == filaPestanya.PestanyaID).OrderBy(exportacion => exportacion.Orden))
            {
                listaExportaciones.Add(CargarExportacion(filaExport));
            }

            return listaExportaciones;
        }

        private TabModel.ExportacionSearchTabModel CargarExportacion(ProyectoPestanyaBusquedaExportacion filaExport)
        {
            TabModel.ExportacionSearchTabModel exportacion = new TabModel.ExportacionSearchTabModel();

            exportacion.Key = filaExport.ExportacionID;
            exportacion.Orden = filaExport.Orden;
            exportacion.Nombre = filaExport.NombreExportacion;

            List<Guid> grupos = new List<Guid>();
            if (filaExport.GruposExportadores != null)
            {
                foreach (string grupo in filaExport.GruposExportadores.Split(','))
                {
                    if (!string.IsNullOrEmpty(grupo))
                    {
                        grupos.Add(new Guid(grupo));
                    }
                }
            }

            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            exportacion.GruposPermiso = identidadCN.ObtenerNombresDeGrupos(grupos);
            identidadCN.Dispose();

            exportacion.ListaPropiedades = CargarPropiedadesExportacion(filaExport);

            return exportacion;
        }

        private List<TabModel.ExportacionSearchTabModel.PropiedadesExportacionSearchTabModel> CargarPropiedadesExportacion(ProyectoPestanyaBusquedaExportacion pFilaExportacion)
        {
            List<TabModel.ExportacionSearchTabModel.PropiedadesExportacionSearchTabModel> listaPropiedades = new List<TabModel.ExportacionSearchTabModel.PropiedadesExportacionSearchTabModel>();

            foreach (ProyectoPestanyaBusquedaExportacionPropiedad filaPropExport in ExpBusquedaDW.ListaProyectoPestanyaBusquedaExportacionPropiedad.Where(propiedadExport => propiedadExport.ExportacionID == pFilaExportacion.ExportacionID).OrderBy(propiedadExport => propiedadExport.Orden))
            {
                TabModel.ExportacionSearchTabModel.PropiedadesExportacionSearchTabModel propiedadExportacion = new TabModel.ExportacionSearchTabModel.PropiedadesExportacionSearchTabModel();

                propiedadExportacion.Orden = filaPropExport.Orden;
                propiedadExportacion.Nombre = filaPropExport.NombrePropiedad;
                propiedadExportacion.Ontologia = null;
                if (filaPropExport.OntologiaID.HasValue)
                {
                    string ontologia = filaPropExport.Ontologia.TrimEnd('#').Replace(".owl", "");
                    ontologia = ontologia.Substring(ontologia.LastIndexOf("/") + 1);
                    propiedadExportacion.Ontologia = ontologia;
                }
                propiedadExportacion.Propiedad = filaPropExport.Propiedad;

                propiedadExportacion.DatoExtraPropiedad = string.Empty;
                if (filaPropExport.DatosExtraPropiedad != null)
                {
                    propiedadExportacion.DatoExtraPropiedad = filaPropExport.DatosExtraPropiedad;
                }
                listaPropiedades.Add(propiedadExportacion);
            }

            return listaPropiedades;
        }

        private TabModel.SearchTabModel.ViewsSearchTabModel CargarVistasDisponibles(AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaBusqueda filaPestanyaBusqueda)
        {
            TabModel.SearchTabModel.ViewsSearchTabModel opcionesVistas = new TabModel.SearchTabModel.ViewsSearchTabModel();

            string vistas = filaPestanyaBusqueda.VistaDisponible;

            short vistaPorDefecto = 0;
            short valorMayor = 0;

            short listadoPermitido = 1;
            short mosaicosPermitido = 0;
            short mapaPermitido = 0;
            short chartPermitido = 0;

            short.TryParse(vistas[0].ToString(), out listadoPermitido);
            if (listadoPermitido > valorMayor) { vistaPorDefecto = 0; valorMayor = listadoPermitido; }

            short.TryParse(vistas[1].ToString(), out mosaicosPermitido);
            if (mosaicosPermitido > valorMayor) { vistaPorDefecto = 1; valorMayor = mosaicosPermitido; }

            if (vistas.Length > 2)
            {
                short.TryParse(vistas[2].ToString(), out mapaPermitido);
                if (mapaPermitido > valorMayor) { vistaPorDefecto = 2; valorMayor = mapaPermitido; }
            }

            if (vistas.Length > 3)
            {
                short.TryParse(vistas[3].ToString(), out chartPermitido);
                if (chartPermitido > valorMayor) { vistaPorDefecto = 3; }
            }

            opcionesVistas.VistaPorDefecto = vistaPorDefecto;
            opcionesVistas.VistaListado = listadoPermitido > 0;
            opcionesVistas.VistaMosaico = mosaicosPermitido > 0;
            opcionesVistas.VistaMapa = mapaPermitido > 0;
            opcionesVistas.VistaGrafico = chartPermitido > 0;

            string posicionCentralMapa = "";
            if (filaPestanyaBusqueda.PosicionCentralMapa != null)
            {
                posicionCentralMapa = filaPestanyaBusqueda.PosicionCentralMapa;
            }
            opcionesVistas.PosicionCentralMapa = posicionCentralMapa;

            return opcionesVistas;
        }

        #endregion

        #region Metodos de Guardado

        public void EliminarPestanyasNoIncluidas(List<TabModel> listaPestanyas, bool pPeticionIntegracionContinua = false)
        {
            List<AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu> listaBD = GestionProyectos.DataWrapperProyectos.ListaProyectoPestanyaMenu.ToList();
            foreach (AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu filaPestanya in listaBD)
            {
                if (!listaPestanyas.Exists(pestanya => pestanya.Key.Equals(filaPestanya.PestanyaID)))
                {
                    EliminarPestanya(filaPestanya, pPeticionIntegracionContinua);
                    GestionProyectos.DataWrapperProyectos.ListaProyectoPestanyaMenu.Remove(filaPestanya);
                }
            }
        }

        public void GuardarPestanyas(List<TabModel> pListaPestanyas, bool pPeticionIntegracionContinua = false)
        {
            List<Guid> listaPestanyasNuevas = new List<Guid>();
            List<string> listaRutasPestanyasRegistrarEnCache = new List<string>();
            List<string> listaRutasPestanyasInvalidarEnCache = new List<string>();

            //Añadir las nuevas
            foreach (TabModel pestanya in pListaPestanyas.OrderBy(x => x.ParentTabKey))
            {
                if (!pestanya.Deleted)
                {
                    //AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu filaPestanya = ObtenerPestanyaSiExsite(pestanya.Key);
                    AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu filaPestanya = GestionProyectos.DataWrapperProyectos.ListaProyectoPestanyaMenu.FirstOrDefault(pest => pest.PestanyaID.Equals(pestanya.Key));
                    if (filaPestanya == null)
                    {
                        listaPestanyasNuevas.Add(pestanya.Key);

                        if (!pestanya.Type.Equals(TipoPestanyaMenu.EnlaceExterno) && !pestanya.Type.Equals(TipoPestanyaMenu.EnlaceInterno))
                        {
                            listaRutasPestanyasRegistrarEnCache.Add(pestanya.Url);
                        }

                        AgregarPestanyaNueva(pestanya, pPeticionIntegracionContinua);
                    }
                }
            }

            //Modificar las que tienen cambios
            foreach (TabModel pestanya in pListaPestanyas)
            {
                if (!pestanya.Deleted && !listaPestanyasNuevas.Contains(pestanya.Key))
                {
                    AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu filaPestanya = GestionProyectos.DataWrapperProyectos.ListaProyectoPestanyaMenu.FirstOrDefault(pest => pest.PestanyaID.Equals(pestanya.Key));
                    if (mEntityContext.Entry(filaPestanya).State == EntityState.Detached)
                    {
                        GestionProyectos.DataWrapperProyectos.ListaProyectoPestanyaMenu.Remove(filaPestanya);
                        filaPestanya = mEntityContext.ProyectoPestanyaMenu.FirstOrDefault(pest => pest.PestanyaID.Equals(pestanya.Key));
                        GestionProyectos.DataWrapperProyectos.ListaProyectoPestanyaMenu.Add(filaPestanya);
                    }
                    if (pestanya.ParentTabKey.Equals(Guid.Empty))
                    {
                        filaPestanya.PestanyaPadreID = null;
                    }
                    else if (!filaPestanya.PestanyaPadreID.Equals(pestanya.ParentTabKey))
                    {
                        filaPestanya.PestanyaPadreID = pestanya.ParentTabKey;
                    }

                    if (!filaPestanya.Orden.Equals(pestanya.Order))
                    {
                        filaPestanya.Orden = pestanya.Order;
                    }



                    if (pestanya.Modified)
                    {
                        if (!pestanya.Type.Equals(TipoPestanyaMenu.EnlaceExterno) && !pestanya.Type.Equals(TipoPestanyaMenu.EnlaceInterno))
                        {
                            bool registrar = false;
                            bool invalidar = false;

                            if (!filaPestanya.Activa && pestanya.Active)
                            {
                                registrar = true;
                            }
                            if (filaPestanya.Activa && !pestanya.Active)
                            {
                                invalidar = true;
                            }
                            if (pestanya.Active && filaPestanya.Ruta != pestanya.Url && !pestanya.EsUrlPorDefecto)
                            {
                                registrar = true;
                                invalidar = true;
                            }

                            if (registrar)
                            {
                                listaRutasPestanyasRegistrarEnCache.Add(pestanya.Url);
                            }
                            if (invalidar)
                            {
                                listaRutasPestanyasInvalidarEnCache.Add(filaPestanya.Ruta);
                            }
                        }

                        GuardarDatosFilaPestanyaMenu(filaPestanya, pestanya);

                        if (pestanya.OpcionesBusqueda != null)
                        {
                            ProyectoPestanyaBusqueda filasBusqueda = filaPestanya.ProyectoPestanyaBusqueda;

                            TabModel.SearchTabModel opcionesBusquedaDefecto = CargarOpcionesBusquedaPorDefecto(pestanya.Type);
                            if (pestanya.OpcionesBusqueda.FiltrosOrden == null)
                            {
                                pestanya.OpcionesBusqueda.FiltrosOrden = new List<TabModel.SearchTabModel.FiltroOrden>();
                            }

                            if (!CompararObjetos(opcionesBusquedaDefecto, pestanya.OpcionesBusqueda) || TieneExportaciones(pestanya))
                            {
                                if (filasBusqueda != null)
                                {
                                    //Si existe, la modifico
                                    GuardarDatosFilaPestanyaBusqueda(filasBusqueda, pestanya);
                                }
                                else
                                {
                                    //Si no existe, la agrego
                                    AgregarPestanyaBusquedaNueva(pestanya);
                                }
                            }
                            else if (filasBusqueda != null)
                            {
                                //Si existe, la elimino
                                mEntityContext.EliminarElemento(filasBusqueda);
                            }
                        }

                        GuardarExportacionesPestanya(pestanya);

                        GuardarDatosFilaPestanyaCMS(filaPestanya, pestanya);
                        mEntityContext.SaveChanges();
                        if (pestanya.Type.Equals(TipoPestanyaMenu.BusquedaSemantica) || pestanya.Type.Equals(TipoPestanyaMenu.Recursos) || pestanya.Type.Equals(TipoPestanyaMenu.Home))
                        {
                            GuardarFacetasPestanya(pestanya);
                        }
                    }
                }
            }

            //Eliminar las eliminadas
            foreach (TabModel pestanya in pListaPestanyas)
            {
                if (pestanya.Deleted && !listaPestanyasNuevas.Contains(pestanya.Key))
                {
                    AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu filaPestanya = GestionProyectos.DataWrapperProyectos.ListaProyectoPestanyaMenu.FirstOrDefault(pest => pest.PestanyaID.Equals(pestanya.Key));
                    if (filaPestanya != null)
                    {
                        if (!pestanya.Type.Equals(TipoPestanyaMenu.EnlaceExterno) && !pestanya.Type.Equals(TipoPestanyaMenu.EnlaceInterno))
                        {
                            listaRutasPestanyasInvalidarEnCache.Add(filaPestanya.Ruta);
                        }
                        if (pestanya.Type.Equals(TipoPestanyaMenu.Home) || pestanya.Type.Equals(TipoPestanyaMenu.BusquedaSemantica) || pestanya.Type.Equals(TipoPestanyaMenu.Recursos))
                        {
                            EliminarFilaFacetasPestanya(pestanya);
                        }
                        EliminarPestanya(filaPestanya);
                    }
                }
            }

            ComprobarInconsistencias();

            mEntityContext.SaveChanges();

            ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            proyCL.AgregarObjetoCache(ProyectoCL.CLAVE_CACHE_LISTA_RUTAPESTANYAS_INVALIDAR + ProyectoSeleccionado.Clave, listaRutasPestanyasInvalidarEnCache, 64800);
            proyCL.AgregarObjetoCache(ProyectoCL.CLAVE_CACHE_LISTA_RUTAPESTANYAS_REGISTRAR + ProyectoSeleccionado.Clave, listaRutasPestanyasRegistrarEnCache, 64800);

        }

        private void ComprobarInconsistencias()
        {
            var nombreCortosRepetidos = GestionProyectos.DataWrapperProyectos.ListaProyectoPestanyaMenu.GroupBy(item => item.NombreCortoPestanya).Where(item => item.Count() > 1).Select(item => item.Key);

            foreach (string nombreCorto in nombreCortosRepetidos)
            {
                foreach (var pestanya in GestionProyectos.DataWrapperProyectos.ListaProyectoPestanyaMenu.Where(item => item.NombreCortoPestanya.Equals(nombreCorto)).ToList())
                {
                    if (mEntityContext.Entry(pestanya).State.Equals(EntityState.Unchanged))
                    {
                        EliminarPestanya(pestanya);
                    }
                }
            }
        }

        private void GuardarDatosFilaPestanyaMenu(AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu pFilaPestanya, TabModel pPestanya)
        {
            if (pPestanya.EsNombrePorDefecto)
            {
                pFilaPestanya.Nombre = null;
            }
            else
            {
                pFilaPestanya.Nombre = pPestanya.Name;
            }
            if (pPestanya.EsUrlPorDefecto)
            {
                pFilaPestanya.Ruta = null;
            }
            else
            {
                pFilaPestanya.Ruta = pPestanya.Url;
            }

            if (string.IsNullOrEmpty(pPestanya.ShortName))
            {
                pPestanya.ShortName = pPestanya.Key.ToString();
            }

            pFilaPestanya.NombreCortoPestanya = pPestanya.ShortName;

            if (pPestanya.ClassCSSBody == null)
            {
                pFilaPestanya.CSSBodyClass = "";
            }
            else
            {
                pFilaPestanya.CSSBodyClass = pPestanya.ClassCSSBody;
            }


            pFilaPestanya.Activa = pPestanya.Active;
            pFilaPestanya.Visible = pPestanya.Visible;
            pFilaPestanya.VisibleSinAcceso = pPestanya.VisibleSinAcceso;
            pFilaPestanya.NuevaPestanya = pPestanya.OpenInNewWindow;
            pFilaPestanya.Privacidad = pPestanya.Privacidad;
            pFilaPestanya.MetaDescription = pPestanya.MetaDescription;
            if (pPestanya.ParentTabKey.Equals(Guid.Empty))
            {
                pFilaPestanya.PestanyaPadreID = null;
            }
            else
            {
                // Si la ParentTabKey no existe en la lista de pestañas, significa que se ha usado una pestaña existente, no hay que actualizar PestanyaPadreID
                pFilaPestanya.PestanyaPadreID = pPestanya.ParentTabKey;
            }
            pFilaPestanya.Orden = pPestanya.Order;

            if (pPestanya.Type.Equals(TipoPestanyaMenu.Home))
            {
                pFilaPestanya.Activa = true;
                CrearPaginasHomeCMS(pPestanya);
            }

            if (pPestanya.Privacidad == 2)
            {
                AgregarFilasPrivacidad(pFilaPestanya, pPestanya);
            }

            if (pPestanya.Privacidad.Equals(2) || (pPestanya.Privacidad.Equals(1) && (ProyectoSeleccionado.TipoAcceso == TipoAcceso.Publico || ProyectoSeleccionado.TipoAcceso == TipoAcceso.Restringido)))
            {
                pFilaPestanya.HtmlAlternativo = HttpUtility.UrlDecode(pPestanya.HtmlAlternativoPrivacidad);
            }
        }

        private void GuardarDatosFilaPestanyaCMS(AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu pFilaPestanya, TabModel pPestanya)
        {
            if (pPestanya.Type == TipoPestanyaMenu.CMS)
            {
                short tipoPagina = 100;

                ProyectoPestanyaCMS filaPestanyaCMS = GestionProyectos.DataWrapperProyectos.ListaProyectoPestanyaCMS.FirstOrDefault(pestanya => pestanya.PestanyaID == pFilaPestanya.PestanyaID);

                if (filaPestanyaCMS == null || mEntityContext.Entry(filaPestanyaCMS).State.Equals(EntityState.Deleted))
                {
                    for (short i = 100; i < 1000; i++)
                    {
                        if (!ListaPaginasCMS.Contains(i))
                        {
                            tipoPagina = i;
                            ListaPaginasCMS.Add(tipoPagina);
                            break;
                        }
                    }

                    filaPestanyaCMS = new ProyectoPestanyaCMS();
                    filaPestanyaCMS.PestanyaID = pPestanya.Key;
                    filaPestanyaCMS.Ubicacion = tipoPagina;
                    GestionProyectos.DataWrapperProyectos.ListaProyectoPestanyaCMS.Add(filaPestanyaCMS);

                    if (!mEntityContext.ProyectoPestanyaCMS.Any(pestanya => pestanya.PestanyaID.Equals(pPestanya.Key)))
                    {
                        mEntityContext.ProyectoPestanyaCMS.Add(filaPestanyaCMS);
                    }
                }

                tipoPagina = filaPestanyaCMS.Ubicacion;

                AD.EntityModel.Models.CMS.CMSPagina filaCMSPagina = GestorCMS.CMSDW.ListaCMSPagina.FirstOrDefault(item => item.OrganizacionID.Equals(ProyectoSeleccionado.FilaProyecto.OrganizacionID) && item.ProyectoID.Equals(ProyectoSeleccionado.Clave) && item.Ubicacion.Equals((short)tipoPagina));

                if (filaCMSPagina == null)
                {
                    filaCMSPagina = new AD.EntityModel.Models.CMS.CMSPagina();
                    filaCMSPagina.ProyectoID = ProyectoSeleccionado.Clave;
                    filaCMSPagina.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
                    filaCMSPagina.Ubicacion = tipoPagina;
                    filaCMSPagina.Activa = false;
                    filaCMSPagina.MostrarSoloCuerpo = false;

                    GestorCMS.CMSDW.ListaCMSPagina.Add(filaCMSPagina);
                    mEntityContext.CMSPagina.Add(filaCMSPagina);
                }

                filaCMSPagina.Activa = pPestanya.Active;

                string idiomasDisponibles = "";

                if (ParametroProyecto.ContainsKey(ParametroAD.PropiedadContenidoMultiIdioma) && pPestanya.ListaIdiomasDisponibles != null)
                {
                    //Si estan marcados todos o ninguno, lo dejamos vacio, si no, guardamos los idiomas seleccionados
    
                    if (pPestanya.ListaIdiomasDisponibles != null && pPestanya.ListaIdiomasDisponibles.Count > 0 && pPestanya.ListaIdiomasDisponibles.Count < mConfigService.ObtenerListaIdiomas().Count)
                    {
                        foreach (string idioma in pPestanya.ListaIdiomasDisponibles)
                        {
                            idiomasDisponibles += "true@" + idioma + "|||";
                        }
                    }
                }

                pFilaPestanya.IdiomasDisponibles = idiomasDisponibles;
            }
        }

        private void GuardarDatosFilaPestanyaBusqueda(ProyectoPestanyaBusqueda pFilaBusqueda, TabModel pPestanya)
        {
            string campoFiltro = pPestanya.OpcionesBusqueda.CampoFiltro;

            if (pPestanya.Type == TipoPestanyaMenu.Recursos || pPestanya.Type == TipoPestanyaMenu.Preguntas || pPestanya.Type == TipoPestanyaMenu.Debates || pPestanya.Type == TipoPestanyaMenu.Encuestas || pPestanya.Type == TipoPestanyaMenu.PersonasYOrganizaciones || pPestanya.Type == TipoPestanyaMenu.BusquedaAvanzada)
            {
                campoFiltro = "";
            }
            //switch (pPestanya.Type)
            //{
            //    case TipoPestanyaMenu.Recursos:
            //        campoFiltro = "rdf:type=Recurso";
            //        break;
            //    case TipoPestanyaMenu.Debates:
            //        campoFiltro = "rdf:type=Debate";
            //        break;
            //    case TipoPestanyaMenu.Preguntas:
            //        campoFiltro = "rdf:type=Pregunta";
            //        break;
            //    case TipoPestanyaMenu.Encuestas:
            //        campoFiltro = "rdf:type=Encuesta";
            //        break;
            //    case TipoPestanyaMenu.PersonasYOrganizaciones:
            //        campoFiltro = "rdf:type=PerYOrg";
            //        break;
            //    case TipoPestanyaMenu.BusquedaAvanzada:
            //        campoFiltro = "rdf:type=Meta";
            //        break;
            //}

            pFilaBusqueda.CampoFiltro = campoFiltro;
            pFilaBusqueda.GruposConfiguracion = pPestanya.OpcionesBusqueda.GruposConfiguracion;

            GuardarFilasFiltroOrden(pPestanya, pFilaBusqueda.ProyectoPestanyaMenu);

            pFilaBusqueda.NumeroRecursos = pPestanya.OpcionesBusqueda.NumeroResultados;
            pFilaBusqueda.VistaDisponible = ObtenerVistaDisponible(pPestanya);
            pFilaBusqueda.MostrarFacetas = pPestanya.OpcionesBusqueda.MostrarFacetas;
            pFilaBusqueda.GruposPorTipo = pPestanya.OpcionesBusqueda.AgruparFacetasPorTipo;
            pFilaBusqueda.MostrarCajaBusqueda = pPestanya.OpcionesBusqueda.MostrarCajaBusqueda;
            pFilaBusqueda.MostrarEnComboBusqueda = pPestanya.OpcionesBusqueda.MostrarEnBusquedaCabecera;
            pFilaBusqueda.IgnorarPrivacidadEnBusqueda = pPestanya.OpcionesBusqueda.IgnorarPrivacidadEnBusqueda;
            pFilaBusqueda.OmitirCargaInicialFacetasResultados = pPestanya.OpcionesBusqueda.OmitirCargaInicialFacetasResultados;
            pFilaBusqueda.PosicionCentralMapa = pPestanya.OpcionesBusqueda.OpcionesVistas.PosicionCentralMapa;
            if (pPestanya.OpcionesBusqueda.ProyectoOrigenBusqueda.Equals(Guid.Empty))
            {
                pFilaBusqueda.ProyectoOrigenID = null;
            }
            else
            {
                pFilaBusqueda.ProyectoOrigenID = pPestanya.OpcionesBusqueda.ProyectoOrigenBusqueda;
            }
            pFilaBusqueda.OcultarResultadosSinFiltros = pPestanya.OpcionesBusqueda.OcultarResultadosSinFiltros;
            pFilaBusqueda.TextoBusquedaSinResultados = pPestanya.OpcionesBusqueda.TextoBusquedaSinResultados;
        }

        private void GuardarFilasFiltroOrden(TabModel pPestanya, AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu pFilaPestanya)
        {
            foreach (ProyectoPestanyaFiltroOrdenRecursos filaFiltroOrden in pFilaPestanya.ProyectoPestanyaFiltroOrdenRecursos)
            {
                if (pPestanya.OpcionesBusqueda.FiltrosOrden == null || !pPestanya.OpcionesBusqueda.FiltrosOrden.Any(filtro => filtro.Orden == filaFiltroOrden.Orden))
                {
                    mEntityContext.EliminarElemento(filaFiltroOrden);
                }
            }

            if (pPestanya.OpcionesBusqueda.FiltrosOrden != null)
            {
                foreach (TabModel.SearchTabModel.FiltroOrden filtroOrden in pPestanya.OpcionesBusqueda.FiltrosOrden)
                {
                    ProyectoPestanyaFiltroOrdenRecursos filaFiltroOrden = GestionProyectos.DataWrapperProyectos.ListaProyectoPestanyaFiltroOrdenRecursos.FirstOrDefault(proy => proy.PestanyaID.Equals(pPestanya.Key) && proy.Orden.Equals(filtroOrden.Orden));

                    if (filaFiltroOrden == null)
                    {
                        AgregarFilaFiltroOrdenNueva(filtroOrden, pPestanya.Key);
                    }
                    else if (!filtroOrden.Deleted)
                    {
                        GuardarDatosFiltroOrdenNueva(filaFiltroOrden, filtroOrden);
                    }
                    else
                    {
                        mEntityContext.EliminarElemento(filaFiltroOrden);
                    }
                }
            }
        }

        private void AgregarFilaFiltroOrdenNueva(TabModel.SearchTabModel.FiltroOrden pFiltroOrden, Guid pPestanyaID)
        {
            if (!pFiltroOrden.Deleted)
            {
                ProyectoPestanyaFiltroOrdenRecursos filaFiltroOrden = new ProyectoPestanyaFiltroOrdenRecursos();
                filaFiltroOrden.PestanyaID = pPestanyaID;

                GuardarDatosFiltroOrdenNueva(filaFiltroOrden, pFiltroOrden);

                GestionProyectos.DataWrapperProyectos.ListaProyectoPestanyaFiltroOrdenRecursos.Add(filaFiltroOrden);
                mEntityContext.ProyectoPestanyaFiltroOrdenRecursos.Add(filaFiltroOrden);

            }
        }

        private void GuardarDatosFiltroOrdenNueva(ProyectoPestanyaFiltroOrdenRecursos pFilaFiltroOrden, TabModel.SearchTabModel.FiltroOrden pFiltroOrden)
        {
            pFilaFiltroOrden.NombreFiltro = pFiltroOrden.Nombre;
            pFilaFiltroOrden.FiltroOrden = pFiltroOrden.Filtro;
            pFilaFiltroOrden.Orden = pFiltroOrden.Orden;
        }

        private bool TieneExportaciones(TabModel pPestanya)
        {
            return (pPestanya.ListaExportaciones != null && pPestanya.ListaExportaciones.Count > 0);
        }

        private void AgregarPestanyaNueva(TabModel pPestanya, bool pPeticionIntegracionContinua = false)
        {
            AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu filaPestanyaNueva = new AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu();
            filaPestanyaNueva.ProyectoID = ProyectoSeleccionado.Clave;
            filaPestanyaNueva.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
            filaPestanyaNueva.PestanyaID = pPestanya.Key;
            filaPestanyaNueva.TipoPestanya = (short)pPestanya.Type;

            GuardarDatosFilaPestanyaMenu(filaPestanyaNueva, pPestanya);
            
            GestionProyectos.DataWrapperProyectos.ListaProyectoPestanyaMenu.Add(filaPestanyaNueva);
            mEntityContext.ProyectoPestanyaMenu.Add(filaPestanyaNueva);
            
            if (pPestanya.OpcionesBusqueda != null)
            {
                TabModel.SearchTabModel opcionesBusquedaDefecto = CargarOpcionesBusquedaPorDefecto(pPestanya.Type);

                if (!CompararObjetos(opcionesBusquedaDefecto, pPestanya.OpcionesBusqueda) || TieneExportaciones(pPestanya))
                {
                    AgregarPestanyaBusquedaNueva(pPestanya);
                }
            }

            GuardarDatosFilaPestanyaCMS(filaPestanyaNueva, pPestanya);

            if (pPestanya.ListaExportaciones != null)
            {
                foreach (TabModel.ExportacionSearchTabModel exportacion in pPestanya.ListaExportaciones)
                {
                    AgregarExportacionBusquedaNueva(exportacion, pPestanya.Key);
                }
            }
            mEntityContext.SaveChanges();
            string texto = "CMS-core";
            var listaPrueba = mEntityContext.ProyectoPestanyaMenu.Where(item => item.Nombre.Equals(texto)).ToList();
            if (pPestanya.Type.Equals(TipoPestanyaMenu.BusquedaSemantica) || pPestanya.Type.Equals(TipoPestanyaMenu.Recursos) || pPestanya.Type.Equals(TipoPestanyaMenu.Home))
            {
                GuardarFacetasPestanyaNueva(pPestanya, pPeticionIntegracionContinua);
            }
        }

        private void GuardarFacetasPestanya(TabModel pPestanya)
        {
            if (pPestanya.ListaFacetas != null)
            {
                List<FacetaObjetoConocimientoProyectoPestanya> listaCompleta = FacetasDW.ListaFacetaObjetoConocimientoProyectoPenstanya.Where(item => item.PestanyaID.Equals(pPestanya.Key)).ToList();
                if (listaCompleta != null)
                {
                    foreach (FacetaObjetoConocimientoProyectoPestanya fila in listaCompleta)
                    {
                        FacetasDW.ListaFacetaObjetoConocimientoProyectoPenstanya.Remove(fila);

                        FacetaObjetoConocimientoProyectoPestanya aux = mEntityContext.FacetaObjetoConocimientoProyectoPestanya.FirstOrDefault(item => item.PestanyaID.Equals(fila.PestanyaID) && item.OrganizacionID.Equals(fila.OrganizacionID) && item.ProyectoID.Equals(fila.ProyectoID) && item.Faceta.Equals(fila.Faceta) && item.ObjetoConocimiento.Equals(fila.ObjetoConocimiento));
                        if (aux != null)
                        {
                            mEntityContext.EliminarElemento(aux);
                        }
                    }
                }
                GuardarFacetasPestanyaNueva(pPestanya);
            }
        }

        private void GuardarFacetasPestanyaNueva(TabModel pPestanya, bool pPeticionIntegracionContinua = false)
        {
            if (pPestanya.ListaFacetas != null)
            {
                foreach (var faceta in pPestanya.ListaFacetas)
                {
                    if (!faceta.Deleted)
                    {
                        FacetaObjetoConocimientoProyectoPestanya facetaPestanya = new FacetaObjetoConocimientoProyectoPestanya();
                        facetaPestanya.Faceta = faceta.Faceta;
                        facetaPestanya.ObjetoConocimiento = faceta.ObjetoConocimiento;
                        facetaPestanya.ProyectoID = ProyectoSeleccionado.Clave;
                        facetaPestanya.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
                        facetaPestanya.PestanyaID = pPestanya.Key;
                        if (!FacetasDW.ListaFacetaObjetoConocimientoProyectoPenstanya.Contains(facetaPestanya))
                        {
                            mLoggingService.GuardarLog($"Intento añadir a FacetaObjetoConocimientoProyectoPestanya {pPestanya.Key}\n");
                            FacetasDW.ListaFacetaObjetoConocimientoProyectoPenstanya.Add(facetaPestanya);
                            mEntityContext.FacetaObjetoConocimientoProyectoPestanya.Add(facetaPestanya);
                            //EntityContext.Instance.Entry(facetaPestanya).State = EntityState.Added;
                        }
                    }
                }
                if (!pPeticionIntegracionContinua)
                {
                    mEntityContext.SaveChanges();
                }
            }
        }

        private void EliminarFilaFacetasPestanya(TabModel pPestanya)
        {
            //if (pPestanya.ListaFacetas != null)
            //{
            List<FacetaObjetoConocimientoProyectoPestanya> listaCompleta = FacetasDW.ListaFacetaObjetoConocimientoProyectoPenstanya.Where(item => item.PestanyaID.Equals(pPestanya.Key)).ToList();
            if (listaCompleta != null)
            {
                foreach (FacetaObjetoConocimientoProyectoPestanya fila in listaCompleta)
                {
                    FacetasDW.ListaFacetaObjetoConocimientoProyectoPenstanya.Remove(fila);

                    FacetaObjetoConocimientoProyectoPestanya aux = mEntityContext.FacetaObjetoConocimientoProyectoPestanya.FirstOrDefault(item => item.PestanyaID.Equals(fila.PestanyaID) && item.OrganizacionID.Equals(fila.OrganizacionID) && item.ProyectoID.Equals(fila.ProyectoID) && item.Faceta.Equals(fila.Faceta) && item.ObjetoConocimiento.Equals(fila.ObjetoConocimiento));
                    if (aux != null)
                    {
                        mEntityContext.EliminarElemento(aux);
                    }

                }
                mEntityContext.SaveChanges();
            }

            //}
        }
        public List<FacetaObjetoConocimientoProyectoPestanya> ObtenerFacetaObjetoConocimientoProyectoPestanya(Guid pPestanya)
        {
            List<FacetaObjetoConocimientoProyectoPestanya> listaCompleta = FacetasDW.ListaFacetaObjetoConocimientoProyectoPenstanya.Where(item => item.PestanyaID.Equals(pPestanya)).ToList();
            List<FacetaObjetoConocimientoProyectoPestanya> listaAux = new List<FacetaObjetoConocimientoProyectoPestanya>();
            if (listaCompleta != null)
            {
                foreach (FacetaObjetoConocimientoProyectoPestanya fila in listaCompleta)
                {

                    FacetaObjetoConocimientoProyectoPestanya aux = mEntityContext.FacetaObjetoConocimientoProyectoPestanya.FirstOrDefault(item => item.PestanyaID.Equals(fila.PestanyaID) && item.OrganizacionID.Equals(fila.OrganizacionID) && item.ProyectoID.Equals(fila.ProyectoID) && item.Faceta.Equals(fila.Faceta) && item.ObjetoConocimiento.Equals(fila.ObjetoConocimiento));
                    if (aux != null)
                    {
                        listaAux.Add(aux);
                    }

                }
            }
            return listaAux;
        }

        private void GuardarExportacionesPestanya(TabModel pPestanya)
        {
            if (pPestanya.ListaExportaciones != null)
            {
                foreach (TabModel.ExportacionSearchTabModel exportacion in pPestanya.ListaExportaciones.ToList())
                {
                    ProyectoPestanyaBusquedaExportacion filaExportacion = ExpBusquedaDW.ListaProyectoPestanyaBusquedaExportacion.Where(item => item.ExportacionID.Equals(exportacion.Key)).FirstOrDefault();
                    if (filaExportacion == null)
                    {
                        AgregarExportacionBusquedaNueva(exportacion, pPestanya.Key);
                    }
                    else if (!exportacion.Deleted)
                    {
                        GuardarDatosExportacion(filaExportacion, exportacion);
                        if (exportacion.ListaPropiedades != null)
                        {
                            ReordenarFilasPropiedadesExportacion(filaExportacion);

                            foreach (TabModel.ExportacionSearchTabModel.PropiedadesExportacionSearchTabModel propiedad in exportacion.ListaPropiedades)
                            {
                                ProyectoPestanyaBusquedaExportacionPropiedad filaPropiedadExportacion = ExpBusquedaDW.ListaProyectoPestanyaBusquedaExportacionPropiedad.Where(item => item.ExportacionID.Equals(exportacion.Key) && item.Orden.Equals(propiedad.Orden)).FirstOrDefault();
                                if (filaPropiedadExportacion == null)
                                {
                                    AgregarPropiedadExportacionNueva(propiedad, exportacion.Key);
                                }
                                else if (!propiedad.Deleted)
                                {
                                    GuardarDatosPropiedadExportacion(filaPropiedadExportacion, propiedad);
                                }
                                else
                                {
                                    mEntityContext.EliminarElemento(filaPropiedadExportacion);
                                    ExpBusquedaDW.ListaProyectoPestanyaBusquedaExportacionPropiedad.Remove(filaPropiedadExportacion);
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (ProyectoPestanyaBusquedaExportacionPropiedad filaPropiedadExportacion in filaExportacion.ProyectoPestanyaBusquedaExportacionPropiedad.ToList())
                        {
                            mEntityContext.EliminarElemento(filaPropiedadExportacion);
                            filaExportacion.ProyectoPestanyaBusquedaExportacionPropiedad.Remove(filaPropiedadExportacion);
                        }
                        ExpBusquedaDW.ListaProyectoPestanyaBusquedaExportacion.Remove(filaExportacion);
                        mEntityContext.EliminarElemento(filaExportacion);
                    }
                }
            }
        }

        private void ReordenarFilasPropiedadesExportacion(ProyectoPestanyaBusquedaExportacion filaExportacion)
        {
            List<ProyectoPestanyaBusquedaExportacionPropiedad> filas = filaExportacion.ProyectoPestanyaBusquedaExportacionPropiedad.OrderBy(fila => fila.Orden).ToList();

            int orden = 0;
            if (filas.Any())
            {
                foreach (ProyectoPestanyaBusquedaExportacionPropiedad fila in filas)
                {
                    if (fila.Orden != orden)
                    {
                        ProyectoPestanyaBusquedaExportacionPropiedad filaNueva = new ProyectoPestanyaBusquedaExportacionPropiedad();
                        filaNueva.DatosExtraPropiedad = fila.DatosExtraPropiedad;
                        filaNueva.ExportacionID = fila.ExportacionID;
                        filaNueva.NombrePropiedad = fila.NombrePropiedad;
                        filaNueva.Ontologia = fila.Ontologia;
                        filaNueva.OntologiaID = fila.OntologiaID;
                        filaNueva.Propiedad = fila.Propiedad;
                        filaNueva.ProyectoPestanyaBusquedaExportacion = fila.ProyectoPestanyaBusquedaExportacion;
                        filaNueva.Orden = (short)orden;

                        //Borramos la fila con el orden incorrecto
                        mEntityContext.Entry(fila).State = EntityState.Deleted;
                        //filaExportacion.ProyectoPestanyaBusquedaExportacionPropiedad.Remove(fila);
                        ExpBusquedaDW.ListaProyectoPestanyaBusquedaExportacionPropiedad.Remove(fila);

                        //Añadimos la fila con el orden correcto
                        mEntityContext.ProyectoPestanyaBusquedaExportacionPropiedad.Add(filaNueva);
                        //filaExportacion.ProyectoPestanyaBusquedaExportacionPropiedad.Add(filaNueva);
                        ExpBusquedaDW.ListaProyectoPestanyaBusquedaExportacionPropiedad.Add(filaNueva);

                        //fila.Orden = (short)orden;
                    }
                    orden++;
                }
                mEntityContext.SaveChanges();
            }
        }

        private void AgregarExportacionBusquedaNueva(TabModel.ExportacionSearchTabModel pExportacion, Guid pPestanyaID)
        {
            if (!pExportacion.Deleted)
            {
                ProyectoPestanyaBusquedaExportacion filaExportacion = new ProyectoPestanyaBusquedaExportacion();
                filaExportacion.ExportacionID = pExportacion.Key;
                filaExportacion.PestanyaID = pPestanyaID;
                filaExportacion.FormatosExportacion = FormatosExportancion.CSV;
                GuardarDatosExportacion(filaExportacion, pExportacion);

                ExpBusquedaDW.ListaProyectoPestanyaBusquedaExportacion.Add(filaExportacion);
                mEntityContext.ProyectoPestanyaBusquedaExportacion.Add(filaExportacion);

                if (pExportacion.ListaPropiedades != null)
                {
                    foreach (TabModel.ExportacionSearchTabModel.PropiedadesExportacionSearchTabModel propiedad in pExportacion.ListaPropiedades)
                    {
                        AgregarPropiedadExportacionNueva(propiedad, pExportacion.Key);
                    }
                }
            }
        }

        private void GuardarDatosExportacion(ProyectoPestanyaBusquedaExportacion pFilaExportacion, TabModel.ExportacionSearchTabModel pExportacion)
        {
            pFilaExportacion.Orden = pExportacion.Orden;
            pFilaExportacion.NombreExportacion = pExportacion.Nombre;

            if (pExportacion.GruposPermiso != null && pExportacion.GruposPermiso.Count > 0)
            {
                string gruposPermiso = "";
                foreach (Guid grupoID in pExportacion.GruposPermiso.Keys)
                {
                    gruposPermiso += grupoID + ",";
                }
                pFilaExportacion.GruposExportadores = gruposPermiso.TrimEnd(',');
            }
            else
            {
                pFilaExportacion.GruposExportadores = null;
            }
        }

        private void AgregarPropiedadExportacionNueva(TabModel.ExportacionSearchTabModel.PropiedadesExportacionSearchTabModel pPropiedad, Guid pExportacionID)
        {
            if (!pPropiedad.Deleted)
            {
                ProyectoPestanyaBusquedaExportacionPropiedad filaPropiedadExportacion = new ProyectoPestanyaBusquedaExportacionPropiedad();
                filaPropiedadExportacion.ExportacionID = pExportacionID;
                filaPropiedadExportacion.DatosExtraPropiedad = "";

                GuardarDatosPropiedadExportacion(filaPropiedadExportacion, pPropiedad);

                ExpBusquedaDW.ListaProyectoPestanyaBusquedaExportacionPropiedad.Add(filaPropiedadExportacion);
                mEntityContext.ProyectoPestanyaBusquedaExportacionPropiedad.Add(filaPropiedadExportacion);
            }
        }

        private void GuardarDatosPropiedadExportacion(ProyectoPestanyaBusquedaExportacionPropiedad pFilaPropiedadExportacion, TabModel.ExportacionSearchTabModel.PropiedadesExportacionSearchTabModel pPropiedad)
        {
            pFilaPropiedadExportacion.Orden = pPropiedad.Orden;
            pFilaPropiedadExportacion.NombrePropiedad = pPropiedad.Nombre;
            pFilaPropiedadExportacion.Propiedad = pPropiedad.Propiedad;
            pFilaPropiedadExportacion.DatosExtraPropiedad = pPropiedad.DatoExtraPropiedad;

            string urlOntologia = "";
            Guid? ontologiaID = Guid.Empty;
            if (!string.IsNullOrEmpty(pPropiedad.Ontologia))
            {
                urlOntologia = "http://gnoss.com/Ontologia/" + pPropiedad.Ontologia + ".owl#";

                DocumentacionCN documentacionCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                ontologiaID = documentacionCN.ObtenerOntologiaAPartirNombre(ProyectoSeleccionado.Clave, pPropiedad.Ontologia + ".owl");
            }

            if (ontologiaID.HasValue && !ontologiaID.Equals(Guid.Empty))
            {
                //la propiedad tiene ontologia
                pFilaPropiedadExportacion.OntologiaID = ontologiaID.Value;
            }
            else
            {
                pFilaPropiedadExportacion.OntologiaID = null;
            }
            pFilaPropiedadExportacion.Ontologia = urlOntologia;
        }

        private void AgregarPestanyaBusquedaNueva(TabModel pPestanya)
        {
            ProyectoPestanyaBusqueda filaPestanyaBusquedaNueva = new ProyectoPestanyaBusqueda();

            filaPestanyaBusquedaNueva.PestanyaID = pPestanya.Key;
            filaPestanyaBusquedaNueva.ProyectoPestanyaMenu = GestionProyectos.DataWrapperProyectos.ListaProyectoPestanyaMenu.FirstOrDefault(pestanyaMenu => pestanyaMenu.PestanyaID.Equals(pPestanya.Key));
            GuardarDatosFilaPestanyaBusqueda(filaPestanyaBusquedaNueva, pPestanya);

            GestionProyectos.DataWrapperProyectos.ListaProyectoPestanyaBusqueda.Add(filaPestanyaBusquedaNueva);

            if (!(mEntityContext.ProyectoPestanyaBusqueda.Where(pest => pest.PestanyaID.Equals(filaPestanyaBusquedaNueva.PestanyaID)).ToList().Count > 0))
            {
                mEntityContext.ProyectoPestanyaBusqueda.Add(filaPestanyaBusquedaNueva);
            }

        }

        private string ObtenerVistaDisponible(TabModel pPestanya)
        {
            TabModel.SearchTabModel.ViewsSearchTabModel opcionesVistas = pPestanya.OpcionesBusqueda.OpcionesVistas;
            string vistaDisponible = "";

            vistaDisponible += (opcionesVistas.VistaPorDefecto == 0 ? "2" : (opcionesVistas.VistaListado ? "1" : "0"));
            vistaDisponible += (opcionesVistas.VistaPorDefecto == 1 ? "2" : (opcionesVistas.VistaMosaico ? "1" : "0"));
            vistaDisponible += (opcionesVistas.VistaPorDefecto == 2 ? "2" : (opcionesVistas.VistaMapa ? "1" : "0"));
            vistaDisponible += (opcionesVistas.VistaPorDefecto == 3 ? "2" : (opcionesVistas.VistaGrafico ? "1" : "0"));

            return vistaDisponible;
        }

        private void CrearPaginasHomeCMS(TabModel pestanya)
        {
            if (pestanya.HomeCMS != null)
            {
                if (ListaPaginasCMS.Contains((short)TipoUbicacionCMS.HomeProyecto) && !pestanya.HomeCMS.HomeTodosUsuarios)
                {
                    EliminarFilaCMSPagina((short)TipoUbicacionCMS.HomeProyecto);
                }
                else if (pestanya.HomeCMS.HomeTodosUsuarios)
                {
                    AgregarFilaCMSPagina((short)TipoUbicacionCMS.HomeProyecto, pestanya.Active);
                }

                if (ListaPaginasCMS.Contains((short)TipoUbicacionCMS.HomeProyectoMiembro) && !pestanya.HomeCMS.HomeMiembros)
                {
                    EliminarFilaCMSPagina((short)TipoUbicacionCMS.HomeProyectoMiembro);
                }
                else if (pestanya.HomeCMS.HomeMiembros)
                {
                    AgregarFilaCMSPagina((short)TipoUbicacionCMS.HomeProyectoMiembro, pestanya.Active);
                }

                if (ListaPaginasCMS.Contains((short)TipoUbicacionCMS.HomeProyectoNoMiembro) && !pestanya.HomeCMS.HomeNoMiembros)
                {
                    EliminarFilaCMSPagina((short)TipoUbicacionCMS.HomeProyectoNoMiembro);
                }
                else if (pestanya.HomeCMS.HomeNoMiembros)
                {
                    AgregarFilaCMSPagina((short)TipoUbicacionCMS.HomeProyectoNoMiembro, pestanya.Active);
                }
            }
        }

        private void AgregarFilaCMSPagina(short pTipoUbicacionCMS, bool pActive)
        {
            AD.EntityModel.Models.CMS.CMSPagina filaCMSPagina = GestorCMS.CMSDW.ListaCMSPagina.FirstOrDefault(item => item.ProyectoID.Equals(ProyectoSeleccionado.Clave) && item.OrganizacionID.Equals(ProyectoSeleccionado.FilaProyecto.OrganizacionID) && item.Ubicacion.Equals((short)pTipoUbicacionCMS));
            //FindByOrganizacionIDProyectoIDUbicacion(ProyectoSeleccionado.FilaProyecto.OrganizacionID, ProyectoSeleccionado.Clave, pTipoUbicacionCMS);

            if (filaCMSPagina != null)
            {
                filaCMSPagina.Activa = pActive;
            }
            else
            {
                filaCMSPagina = new AD.EntityModel.Models.CMS.CMSPagina();
                filaCMSPagina.ProyectoID = ProyectoSeleccionado.Clave;
                filaCMSPagina.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
                filaCMSPagina.Activa = pActive;
                filaCMSPagina.Ubicacion = pTipoUbicacionCMS;
                filaCMSPagina.MostrarSoloCuerpo = false;

                GestorCMS.CMSDW.ListaCMSPagina.Add(filaCMSPagina);
                mEntityContext.CMSPagina.Add(filaCMSPagina);
            }
        }

        private void AgregarFilasPrivacidad(AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu pFilaPestanya, TabModel pPestanya)
        {
            List<AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenuRolIdentidad> filasRolIdentidades = pFilaPestanya.ProyectoPestanyaMenuRolIdentidad.ToList();
            foreach (AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenuRolIdentidad filaPerfil in filasRolIdentidades)
            {
                Guid perfilID = filaPerfil.PerfilID;
                if (pPestanya.PrivacidadPerfiles == null || !pPestanya.PrivacidadPerfiles.ContainsKey(perfilID))
                {
                    //filaPerfil.Delete();
                    filasRolIdentidades.Remove(filaPerfil);
                    mEntityContext.EliminarElemento(filaPerfil);
                }
            }

            if (pPestanya.PrivacidadPerfiles != null)
            {
                foreach (Guid perfilID in pPestanya.PrivacidadPerfiles.Keys)
                {
                    //.FindByPestanyaIDPerfilID(pPestanya.Key, perfilID)
                    if (GestionProyectos.DataWrapperProyectos.ListaProyectoPestanyaMenuRolIdentidad.FirstOrDefault(pest => pest.PestanyaID.Equals(pPestanya.Key) && pest.PerfilID.Equals(perfilID)) == null)
                    {
                        AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenuRolIdentidad filaPerfil = new AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenuRolIdentidad();
                        filaPerfil.PestanyaID = pPestanya.Key;
                        filaPerfil.PerfilID = perfilID;
                        GestionProyectos.DataWrapperProyectos.ListaProyectoPestanyaMenuRolIdentidad.Add(filaPerfil);
                        if (mEntityContext.ProyectoPestanyaMenuRolIdentidad.FirstOrDefault(pest => pest.PestanyaID.Equals(pPestanya.Key) && pest.PerfilID.Equals(perfilID)) == null)
                        {
                            mEntityContext.ProyectoPestanyaMenuRolIdentidad.Add(filaPerfil);
                        }
                    }
                }
            }

            List<AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenuRolGrupoIdentidades> filasRolGrupoIdentidades = pFilaPestanya.ProyectoPestanyaMenuRolGrupoIdentidades.ToList();
            foreach (AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenuRolGrupoIdentidades filaGrupo in filasRolGrupoIdentidades)
            {
                Guid grupoID = filaGrupo.GrupoID;
                if (pPestanya.PrivacidadGrupos == null || !pPestanya.PrivacidadGrupos.ContainsKey(grupoID))
                {
                    pFilaPestanya.ProyectoPestanyaMenuRolGrupoIdentidades.Remove(filaGrupo);
                    mEntityContext.EliminarElemento(filaGrupo);
                }
            }

            if (pPestanya.PrivacidadGrupos != null)
            {
                IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

                pPestanya.PrivacidadGrupos = identCN.ObtenerNombresDeGrupos(pPestanya.PrivacidadGrupos.Keys.ToList());

                foreach (Guid grupoID in pPestanya.PrivacidadGrupos.Keys)
                {
                    if (GestionProyectos.DataWrapperProyectos.ListaProyectoPestanyaMenuRolGrupoIdentidades.FirstOrDefault(proy => proy.PestanyaID.Equals(pPestanya.Key) && proy.GrupoID.Equals(grupoID)) == null)//FindByPestanyaIDGrupoID(pPestanya.Key, grupoID) == null
                    {
                        AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenuRolGrupoIdentidades filaGrupo = new AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenuRolGrupoIdentidades();
                        filaGrupo.PestanyaID = pPestanya.Key;
                        filaGrupo.GrupoID = grupoID;
                        GestionProyectos.DataWrapperProyectos.ListaProyectoPestanyaMenuRolGrupoIdentidades.Add(filaGrupo);
                        if (mEntityContext.ProyectoPestanyaMenuRolGrupoIdentidades.FirstOrDefault(proy => proy.GrupoID.Equals(filaGrupo.GrupoID) && proy.PestanyaID.Equals(filaGrupo.PestanyaID)) == null)
                        {
                            mEntityContext.ProyectoPestanyaMenuRolGrupoIdentidades.Add(filaGrupo);
                        }

                    }
                }
            }
        }

        private void EliminarPestanya(AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu filaPestanya, bool pPeticionIntegracionContinua = false)
        {
            List<ProyectoPestanyaBusquedaExportacion> listaProyectoPestanyaBusquedaExportacionBorrar = ExpBusquedaDW.ListaProyectoPestanyaBusquedaExportacion.Where(exportacion => mEntityContext.Entry(exportacion).State != EntityState.Deleted && exportacion.PestanyaID == filaPestanya.PestanyaID).ToList().ToList();
            foreach (ProyectoPestanyaBusquedaExportacion filaExportacion in listaProyectoPestanyaBusquedaExportacionBorrar)
            {
                foreach (ProyectoPestanyaBusquedaExportacionPropiedad filaPropiedadExportacion in filaExportacion.ProyectoPestanyaBusquedaExportacionPropiedad.ToList())
                {
                    mEntityContext.EliminarElemento(filaPropiedadExportacion);
                    ExpBusquedaDW.ListaProyectoPestanyaBusquedaExportacionPropiedad.Remove(filaPropiedadExportacion);
                    filaExportacion.ProyectoPestanyaBusquedaExportacionPropiedad.Remove(filaPropiedadExportacion);
                }

                ExpBusquedaDW.ListaProyectoPestanyaBusquedaExportacion.Remove(filaExportacion);
                mEntityContext.EliminarElemento(filaExportacion);
            }

            ProyectoPestanyaBusqueda filasBusqueda = filaPestanya.ProyectoPestanyaBusqueda;
            if (filasBusqueda != null)
            {
                filaPestanya.ProyectoPestanyaBusqueda = null;
                mEntityContext.EliminarElemento(filasBusqueda);

            }

            List<ConfigAutocompletarProy> filasConfigAutocompletarProy = filaPestanya.ConfigAutocompletarProy.ToList();
            foreach (ConfigAutocompletarProy filaConfigAutocompletarProy in filasConfigAutocompletarProy)
            {
                filaPestanya.ConfigAutocompletarProy.Remove(filaConfigAutocompletarProy);
                mEntityContext.EliminarElemento(filaConfigAutocompletarProy);
            }
            List<ProyectoPestanyaFiltroOrdenRecursos> filasFiltroOrden = filaPestanya.ProyectoPestanyaFiltroOrdenRecursos.ToList();
            foreach (ProyectoPestanyaFiltroOrdenRecursos filaFiltroOrden in filasFiltroOrden)
            {
                filaPestanya.ProyectoPestanyaFiltroOrdenRecursos.Remove(filaFiltroOrden);
                mEntityContext.EliminarElemento(filaFiltroOrden);
            }

            List<AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenuRolGrupoIdentidades> filasGruposIdentidades = filaPestanya.ProyectoPestanyaMenuRolGrupoIdentidades.ToList();
            foreach (AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenuRolGrupoIdentidades filaGrupoIdentidad in filasGruposIdentidades)
            {
                filaPestanya.ProyectoPestanyaMenuRolGrupoIdentidades.Remove(filaGrupoIdentidad);
                mEntityContext.EliminarElemento(filaGrupoIdentidad);
            }

            List<AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenuRolIdentidad> filasIdentidades = filaPestanya.ProyectoPestanyaMenuRolIdentidad.ToList();
            foreach (AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenuRolIdentidad filaIdentidad in filasIdentidades)
            {
                filaPestanya.ProyectoPestanyaMenuRolIdentidad.Remove(filaIdentidad);
                mEntityContext.EliminarElemento(filaIdentidad);
            }

            if (filaPestanya.TipoPestanya.Equals((short)TipoPestanyaMenu.CMS) || filaPestanya.TipoPestanya.Equals((short)TipoPestanyaMenu.Home))
            {
                List<ProyectoPestanyaCMS> filasCMS = filaPestanya.ProyectoPestanyaCMS.ToList();
                if (filasCMS.Any())
                {
                    EliminarFilaCMSPagina(filasCMS[0].Ubicacion);
                    filaPestanya.ProyectoPestanyaCMS.Remove(filasCMS.First());
                    mEntityContext.EliminarElemento(filasCMS.First());
                }
            }

            if (filaPestanya.TipoPestanya.Equals((short)TipoPestanyaMenu.Home))
            {
                if (ListaPaginasCMS.Contains((short)TipoUbicacionCMS.HomeProyecto))
                {
                    EliminarFilaCMSPagina((short)TipoUbicacionCMS.HomeProyecto);
                }
                if (ListaPaginasCMS.Contains((short)TipoUbicacionCMS.HomeProyectoMiembro))
                {
                    EliminarFilaCMSPagina((short)TipoUbicacionCMS.HomeProyectoMiembro);
                }
                if (ListaPaginasCMS.Contains((short)TipoUbicacionCMS.HomeProyectoNoMiembro))
                {
                    EliminarFilaCMSPagina((short)TipoUbicacionCMS.HomeProyectoNoMiembro);
                }
            }

            //filaPestanya.Delete();
            filaPestanya.ProyectoPestanyaMenu1.Remove(filaPestanya);
            if (mEntityContext.Entry(filaPestanya).State.Equals(EntityState.Detached))
            {
                AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu pestanyaEliminar = mEntityContext.ProyectoPestanyaMenu.FirstOrDefault(pestanya => pestanya.PestanyaID.Equals(filaPestanya.PestanyaID));
                if (pestanyaEliminar != null)
                {
                    mEntityContext.EliminarElemento(pestanyaEliminar, pPeticionIntegracionContinua);
                }

            }
            else
            {
                mEntityContext.EliminarElemento(filaPestanya, pPeticionIntegracionContinua);
            }
        }

        private void EliminarFilaCMSPagina(short pTipoUbicacionCMS)
        {
            AD.EntityModel.Models.CMS.CMSPagina filaCMSPagina = GestorCMS.CMSDW.ListaCMSPagina.FirstOrDefault(item => item.ProyectoID.Equals(ProyectoSeleccionado.Clave) && item.OrganizacionID.Equals(ProyectoSeleccionado.FilaProyecto.OrganizacionID) && item.Ubicacion.Equals((short)pTipoUbicacionCMS));

            if (filaCMSPagina != null)
            {
                List<AD.EntityModel.Models.CMS.CMSBloque> filasBloques = GestorCMS.CMSDW.ListaCMSBloque.Where(item => item.ProyectoID.Equals(ProyectoSeleccionado.Clave) && item.OrganizacionID.Equals(ProyectoSeleccionado.FilaProyecto.OrganizacionID) && item.Ubicacion.Equals((short)pTipoUbicacionCMS)).ToList();

                foreach (AD.EntityModel.Models.CMS.CMSBloque filaBloque in filasBloques)
                {
                    List<Guid> listaIDBloque = filasBloques.Select(item => item.BloqueID).ToList();
                    List<AD.EntityModel.Models.CMS.CMSBloqueComponente> listaBloqueComponente = mEntityContext.CMSBloqueComponente.Where(item => listaIDBloque.Contains(item.BloqueID)).ToList();
                    foreach (AD.EntityModel.Models.CMS.CMSBloqueComponente filaBloqueComponente in listaBloqueComponente)
                    {
                        List<Guid> listaIDBloqueComponente = listaBloqueComponente.Select(item => item.BloqueID).ToList();
                        List<AD.EntityModel.Models.CMS.CMSBloqueComponentePropiedadComponente> listaBloqueComponentePropiedadaCompoenente = mEntityContext.CMSBloqueComponentePropiedadComponente.Where(item => listaIDBloqueComponente.Contains(item.BloqueID)).ToList();
                        foreach (AD.EntityModel.Models.CMS.CMSBloqueComponentePropiedadComponente filaBloquePropiedadComponente in listaBloqueComponentePropiedadaCompoenente)
                        {
                            GestorCMS.CMSDW.ListaCMSBloqueComponentePropiedadComponente.Remove(filaBloquePropiedadComponente);
                            mEntityContext.EliminarElemento(filaBloquePropiedadComponente);
                        }
                        GestorCMS.CMSDW.ListaCMSBloqueComponente.Remove(filaBloqueComponente);
                        mEntityContext.EliminarElemento(filaBloqueComponente);
                    }
                    GestorCMS.CMSDW.ListaCMSBloque.Remove(filaBloque);
                    mEntityContext.EliminarElemento(filaBloque);
                }
                GestorCMS.CMSDW.ListaCMSPagina.Remove(filaCMSPagina);
                mEntityContext.EliminarElemento(filaCMSPagina);
            }
        }

        private void EliminarFilasPrivacidad(AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu pFilaPestanya)
        {
            List<AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenuRolIdentidad> filasPerfiles = pFilaPestanya.ProyectoPestanyaMenuRolIdentidad.ToList();
            foreach (AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenuRolIdentidad filaPerfil in filasPerfiles)
            {
                pFilaPestanya.ProyectoPestanyaMenuRolIdentidad.Remove(filaPerfil);
                mEntityContext.EliminarElemento(filaPerfil);
            }

            List<AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenuRolGrupoIdentidades> filasGruposIdentidades = pFilaPestanya.ProyectoPestanyaMenuRolGrupoIdentidades.ToList();
            foreach (AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenuRolGrupoIdentidades filaGrupoIdentidad in filasGruposIdentidades)
            {
                pFilaPestanya.ProyectoPestanyaMenuRolGrupoIdentidades.Remove(filaGrupoIdentidad);
                mEntityContext.EliminarElemento(filaGrupoIdentidad);
            }
        }

        #endregion

        #region Gestion de errores
        public string ComprobarErrores(List<TabModel> pListaPestanyas)
        {
            string error = "";

            error = ComprobarErrorUrlsRepetidas(pListaPestanyas);

            if (error.Equals(string.Empty))
            {
                error = ComprobarErrorNombresVacios(pListaPestanyas);
            }

            if (error.Equals(string.Empty))
            {
                error = ComprobarErrorNombresCortosRepetidos(pListaPestanyas);
            }

            if (error.Equals(string.Empty))
            {
                error = ComprobarProyectoOrigenID(pListaPestanyas);
            }
            if (error.Equals(string.Empty))
            {
                error = ComprobarLongitudMetaDescription(pListaPestanyas);
            }

            return error;
        }

        private string ComprobarLongitudMetaDescription(List<TabModel> pListaPestanyas)
        {
            foreach (TabModel pestanya in pListaPestanyas)
            {
                if (!pestanya.Deleted && pestanya.Modified && pestanya.MetaDescription != null)
                {
                    if (pestanya.MetaDescription.Length > 500)
                    {
                        return $"{pestanya.Key.ToString()}: Longitud máxima permitida sobrepasada";
                    }
                }
            }

            return "";
        }

        private string ComprobarProyectoOrigenID(List<TabModel> pListaPestanyas)
        {
            foreach (TabModel pestanya in pListaPestanyas)
            {
                if (!pestanya.Deleted && pestanya.Modified)
                {
                    if (pestanya.OpcionesBusqueda != null && !pestanya.OpcionesBusqueda.ProyectoOrigenBusqueda.Equals(Guid.Empty))
                    {
                        //No permitimos la busqueda si el proyectoorigenid es privado o reservado
                        ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                        TipoAcceso tipoAcceso = proyCL.ObtenerTipoAccesoProyecto(pestanya.OpcionesBusqueda.ProyectoOrigenBusqueda);
                        if (tipoAcceso.Equals(TipoAcceso.Privado) || tipoAcceso.Equals(TipoAcceso.Reservado))
                        {
                            return "PROYECTO_ORIGEN_BUSQUEDA_PRIVADO|||" + pestanya.Key.ToString();
                        }
                    }
                }
            }

            return "";
        }

        private string ComprobarErrorNombresVacios(List<TabModel> pListaPestanyas)
        {
            string idiomaPorDefecto = !(ParametrosGeneralesRow.IdiomaDefecto == null) ? ParametrosGeneralesRow.IdiomaDefecto : mConfigService.ObtenerListaIdiomas().First();

            foreach (TabModel pestanya in pListaPestanyas)
            {
                if (!pestanya.Deleted && !pestanya.EsUrlPorDefecto)
                {
                    string nombreIdiomaPorDefecto = UtilCadenas.ObtenerTextoDeIdioma(pestanya.Url, idiomaPorDefecto, null, true);

                    if (string.IsNullOrEmpty(nombreIdiomaPorDefecto))
                    {
                        return "NOMBRE VACIO|||" + pestanya.Key.ToString();
                    }
                }
            }

            return "";
        }

        private string ComprobarErrorUrlsRepetidas(List<TabModel> pListaPestanyas)
        {
            
            string idiomaPorDefecto = !(ParametrosGeneralesRow.IdiomaDefecto == null) ? ParametrosGeneralesRow.IdiomaDefecto : mConfigService.ObtenerListaIdiomas().First();

            Dictionary<string, List<string>> listaRutasPorIdiomas = new Dictionary<string, List<string>>();

            foreach (TabModel pestanya in pListaPestanyas)
            {
                if (!pestanya.Deleted && !pestanya.Type.Equals(TipoPestanyaMenu.EnlaceInterno) && !pestanya.Type.Equals(TipoPestanyaMenu.EnlaceExterno))
                {
                    foreach (string idioma in mConfigService.ObtenerListaIdiomas())
                    {
                        string rutaActual = "";

                        if (!pestanya.EsUrlPorDefecto)
                        {
                            rutaActual = UtilCadenas.ObtenerTextoDeIdioma(pestanya.Url, idioma, idiomaPorDefecto);
                        }
                        else
                        {
                            string nombre = "";
                            ObtenerNameUrlMultiIdiomaPestanyaPorTipo(pestanya.Type, out nombre, out rutaActual);
                        }

                        if (!listaRutasPorIdiomas.ContainsKey(idioma))
                        {
                            listaRutasPorIdiomas.Add(idioma, new List<string>());
                        }
                        if (listaRutasPorIdiomas[idioma].Contains(rutaActual))
                        {
                            return "RUTA REPETIDA|||" + pestanya.Key.ToString();
                        }
                        else
                        {
                            listaRutasPorIdiomas[idioma].Add(rutaActual);
                        }
                    }
                }
                else if (!pestanya.Deleted && pestanya.Type.Equals(TipoPestanyaMenu.EnlaceInterno))
                {
                    string url = string.Empty;
                    if (ProyectoVirtual.Clave != ProyectoAD.MetaProyecto)
                    {
                        url = new GnossUrlsSemanticas(mLoggingService, mEntityContext, mConfigService).ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoVirtual.NombreCorto);
                    }
                    else
                    {
                        url = BaseURL;
                    }
                    string urlReplace = url + "/";

                    if (pestanya.Url.StartsWith(urlReplace))
                    {
                        pestanya.Url = pestanya.Url.Substring(urlReplace.Length);
                    }
                    if (pestanya.Url.Contains("|||" + urlReplace))
                    {
                        pestanya.Url = pestanya.Url.Replace("|||" + urlReplace, "|||");
                    }
                }
            }
            return "";
        }

        private string ComprobarErrorNombresCortosRepetidos(List<TabModel> pListaPestanyas)
        {

            List<string> listaNombresCortos = new List<string>();

            foreach (TabModel pestanya in pListaPestanyas)
            {
                if (!pestanya.Deleted)
                {
                    string nombreCorto = pestanya.ShortName;
                    if (string.IsNullOrEmpty(nombreCorto))
                    {
                        nombreCorto = pestanya.Key.ToString();
                    }

                    if (!listaNombresCortos.Contains(nombreCorto))
                    {
                        listaNombresCortos.Add(nombreCorto);
                    }
                    else
                    {
                        return "NOMBRECORTO REPETIDO|||" + pestanya.Key.ToString();
                    }
                }
            }

            return "";
        }

        public void ObtenerNameUrlMultiIdiomaPestanyaPorTipo(TipoPestanyaMenu pTipoPestanya, out string pNombre, out string pUrl)
        {
            pNombre = string.Empty;
            pUrl = string.Empty;

            switch (pTipoPestanya)
            {
                case TipoPestanyaMenu.Home:
                    pNombre = obtenerTextoMultiIdioma("COMMON", "HOME");
                    break;
                case TipoPestanyaMenu.Indice:
                    pNombre = obtenerTextoMultiIdioma("COMMON", "INDICE");
                    pUrl = obtenerTextoMultiIdioma("URLSEM", "INDICE");
                    break;
                case TipoPestanyaMenu.Recursos:
                    pNombre = obtenerTextoMultiIdioma("COMMON", "BASERECURSOS");
                    pUrl = obtenerTextoMultiIdioma("URLSEM", "RECURSOS");
                    break;
                case TipoPestanyaMenu.Preguntas:
                    pNombre = obtenerTextoMultiIdioma("COMMON", "PREGUNTAS");
                    pUrl = obtenerTextoMultiIdioma("URLSEM", "PREGUNTAS");
                    break;
                case TipoPestanyaMenu.Debates:
                    pNombre = obtenerTextoMultiIdioma("COMMON", "DEBATES");
                    pUrl = obtenerTextoMultiIdioma("URLSEM", "DEBATES");
                    break;
                case TipoPestanyaMenu.Encuestas:
                    pNombre = obtenerTextoMultiIdioma("COMMON", "ENCUESTAS");
                    pUrl = obtenerTextoMultiIdioma("URLSEM", "ENCUESTAS");
                    break;
                case TipoPestanyaMenu.PersonasYOrganizaciones:
                    pNombre = obtenerTextoMultiIdioma("COMMON", "PERSONASYORGANIZACIONES");
                    if (ProyectoVirtual.TipoProyecto == TipoProyecto.EducacionExpandida || ProyectoVirtual.TipoProyecto == TipoProyecto.Universidad20 || ProyectoVirtual.TipoProyecto == TipoProyecto.EducacionPrimaria)
                    {
                        pNombre = obtenerTextoMultiIdioma("COMMON", "PROFESORESYALUMNOS");
                    }
                    pUrl = obtenerTextoMultiIdioma("URLSEM", "PERSONASYORGANIZACIONES");
                    break;
                case TipoPestanyaMenu.AcercaDe:
                    pNombre = obtenerTextoMultiIdioma("COMMON", "ACERCADE");
                    pUrl = obtenerTextoMultiIdioma("URLSEM", "ACERCADE");
                    break;
                case TipoPestanyaMenu.BusquedaAvanzada:
                    pNombre = obtenerTextoMultiIdioma("BUSCADORFACETADO", "TODALACOMUNIDAD");
                    pUrl = obtenerTextoMultiIdioma("URLSEM", "BUSQUEDAAVANZADA");
                    break;
            }
        }

        private string obtenerTextoMultiIdioma(string pPage, string pText)
        {
            List<string> listaIdiomas = mConfigService.ObtenerListaIdiomas();

            string textoMultiIdioma = "";

            foreach (string idioma in listaIdiomas)
            {
                UtilIdiomas utilIdiomasAux = new UtilIdiomas(idioma, mLoggingService, mEntityContext, mConfigService);
                textoMultiIdioma += utilIdiomasAux.GetText(pPage, pText) + "@" + idioma + "|||";
            }

            return textoMultiIdioma.TrimEnd('|');
        }

        #endregion

        #region Invalidar cache

        public void InvalidarCaches(string UrlIntragnoss)
        {
            ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            proyCL.InvalidarPestanyasProyecto(ProyectoSeleccionado.Clave);
            proyCL.InvalidarSeccionesHomeCatalogoDeProyecto(ProyectoSeleccionado.Clave);
            proyCL.InvalidarFilaProyecto(ProyectoSeleccionado.Clave);

            proyCL.InvalidarFiltrosOrdenesDeProyecto(ProyectoSeleccionado.Clave);

            proyCL.InvalidarComunidadMVC(ProyectoSeleccionado.Clave);
            proyCL.InvalidarCabeceraMVC(ProyectoSeleccionado.Clave);

            CMSCL cmsCL = new CMSCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
            cmsCL.InvalidarCacheConfiguracionCMSPorProyecto(ProyectoSeleccionado.Clave);

            FacetaCL facetaCL = new FacetaCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
            bool cachearFacetas = !(this.ParametroProyecto.ContainsKey("CacheFacetas") && this.ParametroProyecto["CacheFacetas"].Equals("0"));
            facetaCL.InvalidarCacheFacetasProyecto(ProyectoSeleccionado.Clave, cachearFacetas);

            FacetadoCL facetadoCL = new FacetadoCL(UrlIntragnoss, mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            facetadoCL.InvalidarResultadosYFacetasDeBusquedaEnProyecto(ProyectoSeleccionado.Clave, "*");

            ExportacionBusquedaCL exportacionCL = new ExportacionBusquedaCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
            exportacionCL.InvalidarCacheExportacionesProyecto(ProyectoSeleccionado.Clave);
            exportacionCL.Dispose();

            mGnossCache.VersionarCacheLocal(ProyectoSeleccionado.Clave);
            mGnossCache.RecalcularRutasProyecto(ProyectoSeleccionado.Clave);
        }

        #endregion

        public bool ExistePestanyaDelMismoTipo(short TipoPestanya)
        {
            return GestionProyectos.DataWrapperProyectos.ListaProyectoPestanyaMenu.Any(p => p.TipoPestanya == TipoPestanya);
        }


        private bool CompararObjetos(object obj1, object obj2)
        {
            byte[] bytesObj1 = ObjectToByteArray(obj1);
            byte[] bytesObj2 = ObjectToByteArray(obj2);

            return bytesObj1.SequenceEqual(bytesObj2); ;
        }

        private byte[] ObjectToByteArray(object obj)
        {
            if (obj == null)
                return null;
            string serializar = JsonConvert.SerializeObject(obj);
            byte[] bytes = Encoding.ASCII.GetBytes(serializar);
            return bytes;
            
            /*BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }*/
        }

        #region Propiedades

        private List<short> ListaPaginasCMS
        {
            get
            {
                if (mListaPaginasCMS == null)
                {
                    mListaPaginasCMS = new List<short>();
                    if (GestorCMS.ListaPaginasProyectos.ContainsKey(ProyectoSeleccionado.Clave))
                    {
                        foreach (short paginaID in GestorCMS.ListaPaginasProyectos[ProyectoSeleccionado.Clave].Keys)
                        {
                            mListaPaginasCMS.Add(paginaID);
                        }
                    }
                }
                return mListaPaginasCMS;
            }
        }


        /// <summary>
        /// Gesotr de CMS del proyectoactual
        /// </summary>
        private GestionCMS GestorCMS
        {
            get
            {
                if (mGestorCMS == null)
                {
                    CMSCN cmsCN = new CMSCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    mGestorCMS = new GestionCMS(cmsCN.ObtenerCMSDeProyecto(ProyectoSeleccionado.Clave), mLoggingService, mEntityContext);
                    cmsCN.Dispose();
                }

                return mGestorCMS;
            }
        }

        /// <summary>
        /// Gesotr de CMS del proyectoactual
        /// </summary>
        private DataWrapperExportacionBusqueda ExpBusquedaDW
        {
            get
            {
                if (mExportacionBusquedaDW == null)
                {
                    ExportacionBusquedaCN exporBusCN = new ExportacionBusquedaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    mExportacionBusquedaDW = exporBusCN.ObtenerExportacionesProyecto(ProyectoSeleccionado.Clave);
                    exporBusCN.Dispose();
                }

                return mExportacionBusquedaDW;
            }
        }
        private DataWrapperFacetas FacetasDW
        {
            get
            {
                if (mFacetasDW == null)
                {
                    FacetaCL facetaCL = new FacetaCL(mEntityContext,mLoggingService,mRedisCacheWrapper,mConfigService, mServicesUtilVirtuosoAndReplication);
                    mFacetasDW = facetaCL.ObtenerTodasFacetasDeProyecto(null, ProyectoAD.MetaOrganizacion, ProyectoSeleccionado.Clave, false);
                    facetaCL.Dispose();
                }

                return mFacetasDW;
            }
        }

        #endregion

    }
}
