using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Faceta;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.Facetado;
using Es.Riam.Gnoss.Logica.Facetado;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.Controles;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Es.Riam.Gnoss.Web.MVC.Controles.Controladores
{
    public class ControladorMapasCharts : ControladorBase
    {
        private LoggingService _loggingService;
        private EntityContext _entityContext;
        private ConfigService _configService;
        private RedisCacheWrapper _redisCacheWrapper;
        private GnossCache _gnossCache;

        public ControladorMapasCharts(LoggingService loggingService, EntityContext entityContext, ConfigService configService, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, servicesUtilVirtuosoAndReplication)
        {
            _loggingService = loggingService;
            _entityContext = entityContext;
            _configService = configService;
            _redisCacheWrapper = redisCacheWrapper;
            _gnossCache = gnossCache;
        }

        #region Metodos Charts
        public AdministrarChartsViewModel LoadChartsFromBBDD()
        {
            AdministrarChartsViewModel resultado = new AdministrarChartsViewModel();
            try
            {
                resultado.Idiomas = _configService.ObtenerListaIdiomas();

                using (FacetaCN facetaCN = new FacetaCN(_entityContext, _loggingService, _configService, mServicesUtilVirtuosoAndReplication))
                {
                    DataWrapperFacetas facetaDW = new DataWrapperFacetas();
                    facetaCN.CargarFacetaConfigProyChart(ProyectoAD.MyGnoss, ProyectoSeleccionado.Clave, facetaDW);
                    foreach (var fila in facetaDW.ListaFacetaConfigProyChart)
                    {
                        ChartViewModel chart = new ChartViewModel();
                        chart.ChartID = fila.ChartID.ToString();
                        chart.FuncionJS = fila.JSBusqueda.ToString();
                        chart.Javascript = fila.JSBase.ToString();
                        chart.Select = fila.SelectConsultaVirtuoso.ToString();
                        chart.Where = fila.FiltrosConsultaVirtuoso.ToString();
                        chart.Orden = short.Parse(fila.Orden.ToString());
                        chart.Nombre = fila.Nombre.ToString();
                        resultado.ListaCharts.Add(chart);
                    }
                }
            }
            catch (Exception ex)
            {
                _loggingService.GuardarLogError(ex);
            }
            return resultado;
        }


        public void SaveListCharts(List<ChartViewModel> pListaCharts)
        {
            using (FacetaCN facetaCN = new FacetaCN(_entityContext, _loggingService, _configService, mServicesUtilVirtuosoAndReplication))
            using (FacetaCL facetadoCL = new FacetaCL(_entityContext, _loggingService, _redisCacheWrapper, _configService, mServicesUtilVirtuosoAndReplication))
            {
                DataWrapperFacetas facetaDW = new DataWrapperFacetas();
                facetaCN.CargarFacetaConfigProyChart(ProyectoAD.MyGnoss, ProyectoSeleccionado.Clave, facetaDW);
                foreach (var chart in pListaCharts)
                {
                    GuardarFilaNueva(facetaDW, chart);
                }
                foreach (var fila in facetaDW.ListaFacetaConfigProyChart)
                {
                    GuardarChartsEnDataSet(pListaCharts, fila);
                }
                _entityContext.SaveChanges();
                facetadoCL.InvalidarCache(pListaCharts.Select(x => x.ChartID).ToList());
                facetadoCL.EliminarDatosChartProyecto(ProyectoSeleccionado.Clave);
                _gnossCache.VersionarCacheLocal(ProyectoSeleccionado.Clave);
            }
        }

        private void GuardarFilaNueva(DataWrapperFacetas facetaDW, ChartViewModel chart)
        {
            Guid auxi = Guid.Parse(chart.ChartID);
            if (!facetaDW.ListaFacetaConfigProyChart.Any(x => x.ChartID == auxi))
            {
                var fila = new FacetaConfigProyChart();
                fila.ChartID = auxi;
                fila.OrganizacionID = ProyectoAD.MyGnoss;
                fila.ProyectoID = ProyectoSeleccionado.Clave;
                fila.FiltrosConsultaVirtuoso = chart.Where;
                fila.JSBase = chart.Javascript;
                fila.JSBusqueda = chart.FuncionJS;
                fila.Nombre = chart.Nombre;
                fila.Orden = chart.Orden;
                fila.SelectConsultaVirtuoso = chart.Select;
                facetaDW.ListaFacetaConfigProyChart.Add(fila);
                _entityContext.FacetaConfigProyChart.Add(fila);
            }
        }

        private void GuardarChartsEnDataSet(List<ChartViewModel> pListaCharts, FacetaConfigProyChart fila)
        {
            foreach (var chart in pListaCharts)
            {
                if (fila.ChartID == Guid.Parse(chart.ChartID))
                {
                    if (chart.Eliminada)
                    {
                        _entityContext.EliminarElemento(fila);
                    }
                    else
                    {
                        fila.FiltrosConsultaVirtuoso = chart.Where;
                        fila.JSBase = chart.Javascript;
                        fila.JSBusqueda = chart.FuncionJS;
                        fila.Nombre = chart.Nombre;
                        fila.Orden = chart.Orden;
                        fila.SelectConsultaVirtuoso = chart.Select;
                    }
                }
            }
        }
        #endregion
        #region Metodos Mapa
        public AdministrarMapaViewModel LoadMapFromBBDD()
        {
            AdministrarMapaViewModel resultado = new AdministrarMapaViewModel();
            try
            {
                using (FacetaCN facetaCN = new FacetaCN(_entityContext, _loggingService, _configService, mServicesUtilVirtuosoAndReplication))
                {
                    DataWrapperFacetas facetaDS = new DataWrapperFacetas();
                    facetaCN.CargarFacetaConfigProyMapa(ProyectoAD.MyGnoss, ProyectoSeleccionado.Clave, facetaDS);
                    var mapaBBDD = facetaDS.ListaFacetaConfigProyMapa.FirstOrDefault();
                    resultado.Latidud = mapaBBDD.PropLatitud;
                    resultado.Longitud = mapaBBDD.PropLongitud;
                    resultado.Ruta = mapaBBDD.PropRuta;
                    resultado.ColorRuta = mapaBBDD.ColorRuta;
                }
            }
            catch (Exception ex)
            {
                _loggingService.GuardarLogError(ex);
            }
            return resultado;
        }
        public void SaveMap(AdministrarMapaViewModel pMapa)
        {
            using (FacetaCN facetaCN = new FacetaCN(_entityContext, _loggingService, _configService, mServicesUtilVirtuosoAndReplication))
            using (FacetaCL facetadoCL = new FacetaCL(_entityContext, _loggingService, _redisCacheWrapper, _configService, mServicesUtilVirtuosoAndReplication))
            {
                DataWrapperFacetas facetaDS = new DataWrapperFacetas();
                facetaCN.CargarFacetaConfigProyMapa(ProyectoAD.MyGnoss, ProyectoSeleccionado.Clave, facetaDS);
                var mapaBBDD = facetaDS.ListaFacetaConfigProyMapa.FirstOrDefault();
                if (mapaBBDD != null)
                {
                    mapaBBDD.ColorRuta = pMapa.ColorRuta;
                    mapaBBDD.PropLatitud = pMapa.Latidud;
                    mapaBBDD.PropLongitud = pMapa.Longitud;
                    mapaBBDD.PropRuta = pMapa.Ruta;
                }
                else
                {
                    var nuevoMapaBBDDD = new FacetaConfigProyMapa();
                    nuevoMapaBBDDD.ColorRuta = pMapa.ColorRuta;
                    nuevoMapaBBDDD.PropLatitud = pMapa.Latidud;
                    nuevoMapaBBDDD.PropLongitud = pMapa.Longitud;
                    nuevoMapaBBDDD.PropRuta = pMapa.Ruta;
                    nuevoMapaBBDDD.OrganizacionID = ProyectoAD.MyGnoss;
                    nuevoMapaBBDDD.ProyectoID = ProyectoSeleccionado.Clave;
                    facetaDS.ListaFacetaConfigProyMapa.Add(nuevoMapaBBDDD);
                    _entityContext.FacetaConfigProyMapa.Add(nuevoMapaBBDDD);
                }
                _entityContext.SaveChanges();
                _gnossCache.VersionarCacheLocal(ProyectoSeleccionado.Clave);
            }
        }
    }
    #endregion
}
