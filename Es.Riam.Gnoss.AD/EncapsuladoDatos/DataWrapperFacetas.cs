using Es.Riam.Gnoss.AD.EntityModel.Models.Faceta;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Es.Riam.Gnoss.AD.EncapsuladoDatos
{
    [Serializable]
    public class DataWrapperFacetas : DataWrapperBase
    {
        public List<OntologiaProyecto> ListaOntologiaProyecto { get; set; }
        public List<FacetaObjetoConocimientoProyecto> ListaFacetaObjetoConocimientoProyecto { get; set; }
        public List<FacetaEntidadesExternas> ListaFacetaEntidadesExternas { get; set; }
        public List<FacetaObjetoConocimiento> ListaFacetaObjetoConocimiento { get; set; }
        public List<FacetaFiltroProyecto> ListaFacetaFiltroProyecto { get; set; }
        public List<FacetaMultiple> ListaFacetaMultiple { get; set; }
        public List<FacetaFiltroHome> ListaFacetaFiltroHome { get; set; }
        public List<FacetaHome> ListaFacetaHome { get; set; }
        public List<FacetaObjetoConocimientoProyectoPestanya> ListaFacetaObjetoConocimientoProyectoPenstanya { get; set; }
        public List<FacetaConfigProyMapa> ListaFacetaConfigProyMapa { get; set; }
        public List<FacetaExcluida> ListaFacetaExcluida { get; set; }
        public List<FacetaRedireccion> ListaFacetaRedireccion { get; set; }
        public List<FacetaConfigProyChart> ListaFacetaConfigProyChart { get; set; }
        public List<FacetaConfigProyRangoFecha> ListaFacetaConfigProyRangoFecha { get; set; }
        public List<ConfiguracionConexionGrafo> ListaConfiguracionConexionGrafo { get; set; }
        public List<string> ListaPropsMapaPerYOrg { get; set; }
        public override void Merge(DataWrapperBase pDataWrapper)
        {
            DataWrapperFacetas dataWrapperOntologia = (DataWrapperFacetas)pDataWrapper;
            this.ListaOntologiaProyecto = this.ListaOntologiaProyecto.Union(dataWrapperOntologia.ListaOntologiaProyecto).ToList();
            ListaFacetaEntidadesExternas = ListaFacetaEntidadesExternas.Union(dataWrapperOntologia.ListaFacetaEntidadesExternas).ToList();
            ListaFacetaObjetoConocimientoProyecto = ListaFacetaObjetoConocimientoProyecto.Union(dataWrapperOntologia.ListaFacetaObjetoConocimientoProyecto).ToList();
            ListaFacetaObjetoConocimiento = ListaFacetaObjetoConocimiento.Union(dataWrapperOntologia.ListaFacetaObjetoConocimiento).ToList();
            ListaFacetaFiltroProyecto = ListaFacetaFiltroProyecto.Union(dataWrapperOntologia.ListaFacetaFiltroProyecto).ToList();
            ListaFacetaMultiple = ListaFacetaMultiple.Union(dataWrapperOntologia.ListaFacetaMultiple).ToList();
            ListaFacetaFiltroHome = ListaFacetaFiltroHome.Union(dataWrapperOntologia.ListaFacetaFiltroHome).ToList();
            ListaFacetaHome = ListaFacetaHome.Union(dataWrapperOntologia.ListaFacetaHome).ToList();
            ListaFacetaObjetoConocimientoProyectoPenstanya = ListaFacetaObjetoConocimientoProyectoPenstanya.Union(dataWrapperOntologia.ListaFacetaObjetoConocimientoProyectoPenstanya).ToList();
            ListaFacetaConfigProyMapa = ListaFacetaConfigProyMapa.Union(dataWrapperOntologia.ListaFacetaConfigProyMapa).ToList();
            ListaFacetaExcluida = ListaFacetaExcluida.Union(dataWrapperOntologia.ListaFacetaExcluida).ToList();
            ListaFacetaRedireccion = ListaFacetaRedireccion.Union(dataWrapperOntologia.ListaFacetaRedireccion).ToList();
            ListaFacetaConfigProyChart = ListaFacetaConfigProyChart.Union(dataWrapperOntologia.ListaFacetaConfigProyChart).ToList();
            ListaFacetaConfigProyRangoFecha = ListaFacetaConfigProyRangoFecha.Union(dataWrapperOntologia.ListaFacetaConfigProyRangoFecha).ToList();
            ListaConfiguracionConexionGrafo = ListaConfiguracionConexionGrafo.Union(dataWrapperOntologia.ListaConfiguracionConexionGrafo).ToList();
            ListaPropsMapaPerYOrg = ListaPropsMapaPerYOrg.Union(dataWrapperOntologia.ListaPropsMapaPerYOrg).ToList();
        }
        
        public DataWrapperFacetas()
        {
            ListaFacetaObjetoConocimientoProyecto = new List<FacetaObjetoConocimientoProyecto>();
            ListaFacetaEntidadesExternas = new List<FacetaEntidadesExternas>();
            ListaFacetaObjetoConocimiento = new List<FacetaObjetoConocimiento>();
            ListaFacetaFiltroProyecto = new List<FacetaFiltroProyecto>();
            ListaOntologiaProyecto = new List<OntologiaProyecto>();
            ListaFacetaMultiple = new List<FacetaMultiple>();
            ListaFacetaFiltroHome = new List<FacetaFiltroHome>();
            ListaFacetaHome = new List<FacetaHome>();
            ListaFacetaObjetoConocimientoProyectoPenstanya = new List<FacetaObjetoConocimientoProyectoPestanya>();
            ListaFacetaConfigProyMapa = new List<FacetaConfigProyMapa>();
            ListaFacetaExcluida = new List<FacetaExcluida>();
            ListaFacetaRedireccion = new List<FacetaRedireccion>();
            ListaFacetaConfigProyChart = new List<FacetaConfigProyChart>();
            ListaFacetaConfigProyRangoFecha = new List<FacetaConfigProyRangoFecha>();
            ListaConfiguracionConexionGrafo = new List<ConfiguracionConexionGrafo>();
            ListaPropsMapaPerYOrg = new List<string>();
        }

        public DataWrapperFacetas Copy()
        {
            DataWrapperFacetas dataWrapperFacetas = new DataWrapperFacetas();
            dataWrapperFacetas.ListaFacetaEntidadesExternas = new List<FacetaEntidadesExternas>(ListaFacetaEntidadesExternas);
            dataWrapperFacetas.ListaFacetaFiltroProyecto = new List<FacetaFiltroProyecto>(ListaFacetaFiltroProyecto);
            dataWrapperFacetas.ListaFacetaObjetoConocimiento = new List<FacetaObjetoConocimiento>(ListaFacetaObjetoConocimiento);
            dataWrapperFacetas.ListaFacetaObjetoConocimientoProyecto = new List<FacetaObjetoConocimientoProyecto>(ListaFacetaObjetoConocimientoProyecto);
            dataWrapperFacetas.ListaOntologiaProyecto = new List<OntologiaProyecto>(ListaOntologiaProyecto);
            dataWrapperFacetas.ListaFacetaMultiple = new List<FacetaMultiple>(ListaFacetaMultiple);
            dataWrapperFacetas.ListaFacetaFiltroHome = new List<FacetaFiltroHome>(ListaFacetaFiltroHome);
            dataWrapperFacetas.ListaFacetaHome = new List<FacetaHome>(ListaFacetaHome);
            dataWrapperFacetas.ListaFacetaObjetoConocimientoProyectoPenstanya = new List<FacetaObjetoConocimientoProyectoPestanya>(ListaFacetaObjetoConocimientoProyectoPenstanya);
            dataWrapperFacetas.ListaFacetaConfigProyMapa = new List<FacetaConfigProyMapa>(ListaFacetaConfigProyMapa);
            dataWrapperFacetas.ListaFacetaExcluida = new List<FacetaExcluida>(ListaFacetaExcluida);
            dataWrapperFacetas.ListaFacetaRedireccion = new List<FacetaRedireccion>(ListaFacetaRedireccion);
            dataWrapperFacetas.ListaFacetaConfigProyChart = new List<FacetaConfigProyChart>(ListaFacetaConfigProyChart);
            dataWrapperFacetas.ListaFacetaConfigProyRangoFecha = new List<FacetaConfigProyRangoFecha>(ListaFacetaConfigProyRangoFecha);
            dataWrapperFacetas.ListaConfiguracionConexionGrafo = new List<ConfiguracionConexionGrafo>(ListaConfiguracionConexionGrafo);
            dataWrapperFacetas.ListaPropsMapaPerYOrg = new List<string>(ListaPropsMapaPerYOrg);
            return dataWrapperFacetas;
        }

        public void CargaRelacionesPerezosasCache()
        {
            foreach(FacetaFiltroProyecto facetaFiltroProyecto in ListaFacetaFiltroProyecto)
            {
                facetaFiltroProyecto.FacetaObjetoConocimientoProyecto = ListaFacetaObjetoConocimientoProyecto.FirstOrDefault(objetoConocimiento => objetoConocimiento.OrganizacionID.Equals(facetaFiltroProyecto.OrganizacionID) && objetoConocimiento.ProyectoID.Equals(facetaFiltroProyecto.ProyectoID) && objetoConocimiento.ObjetoConocimiento.Equals(facetaFiltroProyecto.ObjetoConocimiento) && objetoConocimiento.Faceta.Equals(facetaFiltroProyecto.Faceta));
            }

            foreach(FacetaObjetoConocimientoProyecto facetaObjetoConocimientoProyecto in ListaFacetaObjetoConocimientoProyecto)
            {
                facetaObjetoConocimientoProyecto.FacetaFiltroProyecto = ListaFacetaFiltroProyecto.Where(filtroProyecto => filtroProyecto.OrganizacionID.Equals(facetaObjetoConocimientoProyecto.OrganizacionID) && filtroProyecto.ProyectoID.Equals(facetaObjetoConocimientoProyecto.ProyectoID) && filtroProyecto.ObjetoConocimiento.Equals(facetaObjetoConocimientoProyecto.ObjetoConocimiento) && filtroProyecto.Faceta.Equals(facetaObjetoConocimientoProyecto.Faceta)).ToList();
            }
        }
    }
}

