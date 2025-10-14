using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Faceta;
using Es.Riam.Gnoss.AD.Facetado;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.Facetado;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.UtilServiciosWeb;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Web.Controles.Facetas
{
    /// <summary>
    /// Controlador de facetas.
    /// </summary>
    public class ControladorFacetas
    {
        private LoggingService mLoggingService;
        private EntityContext mEntityContext;
        private ConfigService mConfigService;
        private RedisCacheWrapper mRedisCacheWrapper;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        public ControladorFacetas(EntityContext entityContext, LoggingService loggingService, RedisCacheWrapper redisCacheWrapper, ConfigService configService, ILogger<ControladorFacetas> logger, ILoggerFactory loggerFactory)
        {
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;
            mRedisCacheWrapper = redisCacheWrapper;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        #region Facetado

        /// <summary>
        /// Obtiene la lista de items extra que se obtendrá de la búsqueda (recetas, peliculas, etc)
        /// </summary>
        /// <param name="pLaguageCode">LaguageCode del idioma actual del usuario (es, en...)</param>
        /// <param name="pListaFiltrosFacetasUsuario">Lista de filtros que ha realizado el usuario</param>
        /// <param name="pProyectoID">Identificador del proyecto en el que está buscando el usuario</param>
        /// <param name="pTipoBusqueda">Tipo de búsqueda</param>
        public List<string> ObtenerListaItemsBusquedaExtra(Dictionary<string, List<string>> pListaFiltrosFacetasUsuario, TipoBusqueda pTipoBusqueda, Guid pOrganizacionID, Guid pProyectoID)
        {
            List<string> listaItemsExtra = new List<string>();

            if ((!pListaFiltrosFacetasUsuario.ContainsKey("rdf:type")) && (pTipoBusqueda.Equals(TipoBusqueda.Recursos) || pTipoBusqueda.Equals(TipoBusqueda.BusquedaAvanzada)))
            {
                //ObtenerOntologias
                FacetaCL facetaCL = new FacetaCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, null, mLoggerFactory.CreateLogger<FacetaCL>(), mLoggerFactory);
                DataWrapperFacetas pFacetaDW = new DataWrapperFacetas();
                List<OntologiaProyecto> listaOntologias = facetaCL.ObtenerOntologiasProyecto(pOrganizacionID, pProyectoID);
                foreach (OntologiaProyecto myrow in listaOntologias)
                {
                    listaItemsExtra.Add((string)myrow.OntologiaProyecto1);
                }
            }
            return listaItemsExtra;
        }

        /// <summary>
        /// Obtiene los predicados semánticos de un proyecto concreto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto en el que está buscando el usuario</param>
        /// <param name="pTipoBusqueda">Tipo de búsqueda</param>
        /// <returns></returns>
        public List<string> ObtenerFormulariosSemanticos(TipoBusqueda pTipoBusqueda, Guid pProyectoID)
        {
            List<string> formulariosSemanticos = null;
            
            //TODO JUAN: quitarlo de aqui

            //if (pTipoBusqueda.Equals(TipoBusqueda.BusquedaAvanzada) || pTipoBusqueda.Equals(TipoBusqueda.Recursos) || pTipoBusqueda.Equals(TipoBusqueda.Contribuciones) || pTipoBusqueda.Equals(TipoBusqueda.EditarRecursosPerfil))
            //{
            //    FacetaCL facetaCL = new FacetaCL();
            //    formulariosSemanticos = facetaCL.ObtenerPredicadosSemanticos(pProyectoID.ToString());
            //}
            //else
            {
                formulariosSemanticos = new List<string>();
            }

            return formulariosSemanticos;
        }

        /// <summary>
        /// Obtiene la lista de items extra que se obtendrá de la búsqueda y su prefijo (recetas, peliculas, etc)
        /// </summary>
        /// <param name="pLaguageCode">LaguageCode del idioma actual del usuario (es, en...)</param>
        /// <param name="pListaFiltrosFacetasUsuario">Lista de filtros que ha realizado el usuario</param>
        /// <param name="pProyectoID">Identificador del proyecto en el que está buscando el usuario</param>
        /// <param name="pTipoBusqueda">Tipo de búsqueda</param>
        /// <param name="pTConfiguracionOntologia">Configuración del facetado</param>
        public Dictionary<string, string> ObtenerInformacionOntologias(TipoBusqueda pTipoBusqueda, Guid pOrganizacionID, Guid pProyectoID, ref DataWrapperFacetas pFacetaDW)
        {
            Dictionary<string, string> informacionOntologias = new Dictionary<string, string>();
            //(!pListaFiltrosFacetasUsuario.ContainsKey("rdf:type")) && 
            if ((pTipoBusqueda.Equals(TipoBusqueda.Recursos) || pTipoBusqueda.Equals(TipoBusqueda.BusquedaAvanzada) || pTipoBusqueda.Equals(TipoBusqueda.Contribuciones) || pTipoBusqueda.Equals(TipoBusqueda.EditarRecursosPerfil)))
            {
                //ObtenerOntologias
                FacetaCL facetaCL = new FacetaCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, null, mLoggerFactory.CreateLogger<FacetaCL>(), mLoggerFactory);
                List<OntologiaProyecto> listaOntologia = facetaCL.ObtenerOntologiasProyecto(pOrganizacionID, pProyectoID);
                foreach (OntologiaProyecto myrow in listaOntologia)
                {
                    if (!string.IsNullOrEmpty(myrow.OntologiaProyecto1.ToString()) && !string.IsNullOrEmpty(myrow.Namespace))
                        informacionOntologias.Add((string)myrow.OntologiaProyecto1, myrow.Namespace);
                }
            }
            return informacionOntologias;
        }

        #endregion

    }
}
