using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Faceta;
using Es.Riam.Gnoss.AD.Facetado;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Facetado;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Es.Riam.Gnoss.CL.Facetado
{
    public class FacetaCL : BaseCL
    {

        #region Miembros

        /// <summary>
        /// Clave MAESTRA de la caché
        /// </summary>
        private readonly string[] mMasterCacheKeyArray = { "Faceta" };

        private FacetaCN mFacetaCN = null;

        private ConfigService mConfigService;
        private EntityContext mEntityContext;
        private LoggingService mLoggingService;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        #endregion Miembros

        #region constructores

        /// <summary>
        /// Constructor para TablasDeConfiguracionCL
        /// </summary>
        public FacetaCL(EntityContext entityContext, LoggingService loggingService, RedisCacheWrapper redisCacheWrapper, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<FacetaCL> logger, ILoggerFactory loggerFactory)
            : base(entityContext, loggingService, redisCacheWrapper, configService, servicesUtilVirtuosoAndReplication,logger,loggerFactory)
        {
            mConfigService = configService;
            mEntityContext = entityContext;
            mLoggingService = loggingService;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        /// <summary>
        /// Constructor para TablasDeConfiguracionCL
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Fichero de configuración</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        /// <param name="pPoolName">Nombre del pool de conexión</param>
        public FacetaCL(string pFicheroConfiguracionBD, EntityContext entityContext, LoggingService loggingService, RedisCacheWrapper redisCacheWrapper, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<FacetaCL> logger, ILoggerFactory loggerFactory)
            : base(pFicheroConfiguracionBD, entityContext, loggingService, redisCacheWrapper, configService, servicesUtilVirtuosoAndReplication,logger,loggerFactory)
        {
            mConfigService = configService;
            mEntityContext = entityContext;
            mLoggingService = loggingService;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        /// <summary>
        /// Constructor para TablasDeConfiguracionCL
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Fichero de configuración</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        /// <param name="pPoolName">Nombre del pool de conexión</param>
        public FacetaCL(string pFicheroConfiguracionBD, string pPoolName, string pUrlIntragnoss, EntityContext entityContext, LoggingService loggingService, RedisCacheWrapper redisCacheWrapper, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<FacetaCL> logger, ILoggerFactory loggerFactory)
            : base(pFicheroConfiguracionBD, pPoolName, entityContext, loggingService, redisCacheWrapper, configService, servicesUtilVirtuosoAndReplication,logger,loggerFactory)
        {
            mDominio = pUrlIntragnoss;

            mConfigService = configService;
            mEntityContext = entityContext;
            mLoggingService = loggingService;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        #endregion constructores

        /// <summary>
        /// Ivalida la caché de las facetas mayusculas de un proyecto
        /// </summary>
        /// <param name="ListaItems">Lista de elementos buscados</param>
        /// <param name="pProyectoID">Proyecto en el que se hace la búsqueda</param>
        /// <param name="pFacetasHome">Indica si las facetas para la configuración son para la home</param>
        public void InvalidarCacheFacetasMayusculas()
        {
            string rawKey = NombresCL.CONFIGURACIONFACETAS + "_MAYUSCULAS";
            InvalidarCache(rawKey);
        }

        /// <summary>
        /// Obtiene los datos para una consulto usando charts.
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <param name="pChartID">ID de chart o Guid.Empty si no se quiere especificar uno</param>
        /// <returns>datos para una consulto usando charts</returns>
        public DataWrapperFacetas ObtenerDatosChartProyecto(Guid pOrganizacionID, Guid pProyectoID)
        {
            string rawKey = "DatosChartProyecto" + "_" + pProyectoID;

            // Compruebo si está en la caché
            DataWrapperFacetas chartDW = ObtenerObjetoDeCacheLocal(rawKey) as DataWrapperFacetas;

            if (chartDW == null)
            {
                chartDW = ObtenerObjetoDeCache(rawKey, typeof(DataWrapperFacetas)) as DataWrapperFacetas;
                AgregarObjetoCacheLocal(pProyectoID, rawKey, chartDW);
            }

            if (chartDW == null)
            {
                // Si no está, lo cargo y lo almaceno en la caché
                chartDW = FacetaCN.ObtenerDatosChartProyecto(pOrganizacionID, pProyectoID, Guid.Empty);
                AgregarObjetoCache(rawKey, chartDW);
                AgregarObjetoCacheLocal(pProyectoID, rawKey, chartDW);
            }

            return chartDW;
        }

        /// <summary>
        /// Elimina los datos para una consulto usando charts.
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        public void EliminarDatosChartProyecto(Guid pProyectoID)
        {
            string rawKey = "DatosChartProyecto" + "_" + pProyectoID;
            InvalidarCache(rawKey);

            VersionarCacheLocal(pProyectoID);
        }

        public List<string> ObtenerPredicadosSemanticos(Guid pOrganizacionID, Guid pProyectoID)
        {

            string rawKey = "FormulariosSemanticos_" + pProyectoID.ToString();

            // Compruebo si está en la caché
            List<string> listaFormulariosSemanticos = ObtenerObjetoDeCacheLocal(rawKey) as List<string>;
            if (listaFormulariosSemanticos == null)
            {
                listaFormulariosSemanticos = ObtenerObjetoDeCache(rawKey, typeof(List<string>)) as List<string>;
                AgregarObjetoCacheLocal(pProyectoID, rawKey, listaFormulariosSemanticos);
            }

            if (listaFormulariosSemanticos == null)
            {
                // Si no está, lo cargo y lo almaceno en la caché
                listaFormulariosSemanticos = FacetaCN.ObtenerPredicadosSemanticos(pOrganizacionID, pProyectoID);

                AgregarObjetoCache(rawKey, listaFormulariosSemanticos);
                AgregarObjetoCacheLocal(pProyectoID, rawKey, listaFormulariosSemanticos);
            }

            return listaFormulariosSemanticos;
        }

        /// <summary>
        /// Obtiene el número de filtros de categoría que tiene configurados un proyecto
        /// </summary>
        /// <param name="ProyectoID">Proyecto en el que se hace la búsqueda</param>
        public int ObtenerNumeroFiltrosCategoriasDeProyecto(Guid pOrganizacionID, Guid pProyectoID)
        {
            string rawKey = NombresCL.CONFIGURACIONFACETAS + "_" + pProyectoID + "_numeroFiltrosCat";

            // Compruebo si está en la caché
            int? numero = ObtenerObjetoDeCacheLocal(rawKey) as int?;

            if (!numero.HasValue)
            {
                numero = ObtenerObjetoDeCache(rawKey, typeof(int)) as int?;
                AgregarObjetoCacheLocal(pProyectoID, rawKey, numero);
            }

            if (!numero.HasValue)
            {
                // Si no está, lo cargo y lo almaceno en la caché
                numero = FacetaCN.ObtenerNumeroFiltrosCategoriasDeProyecto(pOrganizacionID, pProyectoID);

                AgregarObjetoCache(rawKey, numero.Value);
                AgregarObjetoCacheLocal(pProyectoID, rawKey, numero);
            }

            return numero.Value;
        }

        public DataWrapperFacetas ObtenerFacetaObjetoConocimientoProyecto(Guid pOrganizacionID, Guid pProyectoID)
        {
            mEntityContext.UsarEntityCache = true;
            string rawKey = NombresCL.CONFIGURACIONFACETAS + "FacetaObjetoConocimientoProyecto";

            // Compruebo si está en la caché
            DataWrapperFacetas dataWrapperFacetas = ObtenerObjetoDeCacheLocal(rawKey) as DataWrapperFacetas;
            if (dataWrapperFacetas == null)
            {
                dataWrapperFacetas = ObtenerObjetoDeCache(rawKey, typeof(DataWrapperFacetas)) as DataWrapperFacetas;
                if (dataWrapperFacetas != null)
                {
                    dataWrapperFacetas.CargaRelacionesPerezosasCache();
                }
                AgregarObjetoCacheLocal(pProyectoID, rawKey, dataWrapperFacetas);
            }

            if (dataWrapperFacetas == null)
            {
                dataWrapperFacetas = FacetaCN.ObtenerFacetaObjetoConocimientoProyecto(pOrganizacionID, pProyectoID);
                if (dataWrapperFacetas != null)
                {
                    dataWrapperFacetas.CargaRelacionesPerezosasCache();
                }
                AgregarObjetoCache(rawKey, dataWrapperFacetas);
                AgregarObjetoCacheLocal(pProyectoID, rawKey, dataWrapperFacetas);
            }
            mEntityContext.UsarEntityCache = false;
            return dataWrapperFacetas;
        }

        /// <summary>
        /// Obtiene las ontologías de un proyecto
        /// </summary>
        /// <param name="pFacetaDS">Dataset de las facetas del proyecto donde se agregarán las ontologías del proyecto</param>
        /// <param name="pOrganizacionID">Identificador de la Organizacion donde se hace la búsqueda</param>
        /// <param name="pProyectoID">Proyecto en el que se hace la búsqueda</param>
        /// <param name="pIdioma"></param>
        public List<OntologiaProyecto> ObtenerOntologiasProyecto(Guid pOrganizacionID, Guid pProyectoID)
        {
            string rawKey = NombresCL.CONFIGURACIONFACETAS + "_Ontologias_2017_" + pProyectoID;

            // Compruebo si está en la caché
            List<OntologiaProyecto> listaOntologias = ObtenerObjetoDeCacheLocal(rawKey) as List<OntologiaProyecto>;

            if (listaOntologias == null)
            {
                listaOntologias = ObtenerObjetoDeCache(rawKey, typeof(List<OntologiaProyecto>)) as List<OntologiaProyecto>;
                AgregarObjetoCacheLocal(pProyectoID, rawKey, listaOntologias);
            }

            if (listaOntologias == null)
            {
                // Si no está, lo cargo y lo almaceno en la caché
                if (TieneComunidadPadreConfigurada(pProyectoID))
                {
                    listaOntologias = FacetaCN.ObtenerOntologiasProyecto(pOrganizacionID, ProyectoIDPadreEcosistema.Value);
                    ModificarDataWrapperComunidadHijaListaOntologia(listaOntologias, pProyectoID);
                }
                else
                {
                    listaOntologias = FacetaCN.ObtenerOntologiasProyecto(pOrganizacionID, pProyectoID);
                }


                AgregarObjetoCache(rawKey, listaOntologias);
                AgregarObjetoCacheLocal(pProyectoID, rawKey, listaOntologias);
            }
            return listaOntologias;
        }

        private void ModificarDataWrapperComunidadHijaListaOntologia(List<OntologiaProyecto> listaOntologias, Guid pProyectoID)
        {
            foreach (OntologiaProyecto ontologia in listaOntologias)
            {
                ontologia.ProyectoID = pProyectoID;
            }

        }

        /// <summary>
        /// Invalida las ontologías de un proyecto
        /// </summary>
        /// <param name="pProyectoID">Proyecto en el que se hace la búsqueda</param>
        public void InvalidarOntologiasProyecto(Guid pProyectoID)
        {
            string rawKey = NombresCL.CONFIGURACIONFACETAS + "_Ontologias_2017_" + pProyectoID;
            InvalidarCache(rawKey);          

            VersionarCacheLocal(pProyectoID);
        }

        /// <summary>
        /// Obtiene las ontologías buscables de un proyecto
        /// </summary>
        /// <param name="pFacetaDS">Dataset de las facetas del proyecto donde se agregarán las ontologías del proyecto</param>
        /// <param name="pOrganizacionID">Identificador de la Organizacion donde se hace la búsqueda</param>
        /// <param name="pProyectoID">Proyecto en el que se hace la búsqueda</param>
        /// <param name="pIdioma"></param>
        public List<OntologiaProyecto> ObtenerOntologiasBuscablesProyecto(Guid pOrganizacionID, Guid pProyectoID)
        {
            mEntityContext.UsarEntityCache = true;
            string rawKey = NombresCL.CONFIGURACIONFACETAS + "_OntologiasBuscables_" + pProyectoID;

            // Compruebo si está en la caché
            List<OntologiaProyecto> listaOntologias = ObtenerObjetoDeCacheLocal(rawKey) as List<OntologiaProyecto>;

            if (listaOntologias == null)
            {
                listaOntologias = ObtenerObjetoDeCache(rawKey, typeof(List<OntologiaProyecto>)) as List<OntologiaProyecto>;
                AgregarObjetoCacheLocal(pProyectoID, rawKey, listaOntologias);
            }

            if (listaOntologias == null)
            {
                // Si no está, lo cargo y lo almaceno en la caché
                listaOntologias = FacetaCN.ObtenerOntologiasProyecto(pOrganizacionID, pProyectoID, true, true);
                AgregarObjetoCache(rawKey, listaOntologias);
                AgregarObjetoCacheLocal(pProyectoID, rawKey, listaOntologias);
            }
            mEntityContext.UsarEntityCache = false;
            return listaOntologias;
        }


        /// <summary>
        /// Invalida las ontologías buscables de un proyecto
        /// </summary>
        /// <param name="pProyectoID">Proyecto en el que se hace la búsqueda</param>
        public void InvalidarOntologiasBuscablesProyecto(Guid pProyectoID)
        {
            string rawKey = NombresCL.CONFIGURACIONFACETAS + "_OntologiasBuscables_" + pProyectoID;
            InvalidarCache(rawKey);

            VersionarCacheLocal(pProyectoID);
        }

        /// <summary>
        /// Obtiene la lista de items extra que se obtendrá de la búsqueda y su prefijo (recetas, peliculas, etc)
        /// </summary>
        public Dictionary<string, List<string>> ObtenerPrefijosDeOntologia(string pOntologia, Guid pProyectoID)
        {
            //obtengo sólo los namespaces para esta ontología
            List<OntologiaProyecto> listaOntologias = ObtenerNamespacesDeOntologia(pOntologia, pProyectoID);

            return FacetadoAD.ObtenerInformacionOntologias(listaOntologias);
        }

        /// <summary>
        /// Obtiene la lista de items extra que se obtendrá de la búsqueda y su prefijo (recetas, peliculas, etc)
        /// </summary>
        public Dictionary<string, List<string>> ObtenerPrefijosOntologiasDeProyecto(Guid pProyectoID)
        {
            //obtengo los namespaces de todas las ontologías del proyecto
            List<OntologiaProyecto> listaOntologias = ObtenerOntologiasProyecto(Guid.Empty, pProyectoID);

            return FacetadoAD.ObtenerInformacionOntologias(listaOntologias);
        }

        private Dictionary<string, string> GenerarDiccionarioPrefijosOntologias(DataWrapperFacetas pConfiguracionFacetadoDWOntologia)
        {
            Dictionary<string, string> informacionOntologias = new Dictionary<string, string>();

            //Pongo los namespace extras como ontologías
            foreach (OntologiaProyecto myrow in pConfiguracionFacetadoDWOntologia.ListaOntologiaProyecto)
            {
                if (!string.IsNullOrEmpty(myrow.OntologiaProyecto1.ToString()) && !string.IsNullOrEmpty(myrow.Namespace))
                {
                    informacionOntologias.Add(myrow.OntologiaProyecto1, myrow.Namespace);
                }
            }

            return informacionOntologias;
        }

        /// <summary>
        /// Obtiene la url de una ontología a partir de su prefijo
        /// </summary>
        /// <param name="pPrefijo">Prefijo de la ontología</param>
        /// <param name="pInformacionOntologias">Diccionario con la lista de ontologías</param>
        /// <returns></returns>
        public static string ObtenerUrlDePrefijoOntologia(string pPrefijo, Dictionary<string, string> pInformacionOntologias)
        {
            string url = null;

            if (pInformacionOntologias != null && !string.IsNullOrEmpty(pPrefijo) && pInformacionOntologias.ContainsKey(pPrefijo))
            {
                url = pInformacionOntologias[pPrefijo];
            }

            return url;
        }

        /// <summary>
        /// Obtiene los namespaces de una ontología
        /// </summary>
        /// <param name="pOntologia">Ontología</param>
        /// <returns>DataSet con los namespaces de una ontología</returns>
        public List<OntologiaProyecto> ObtenerNamespacesDeOntologia(string pOntologia, Guid pProyectoID)
        {
            string rawKey = NombresCL.CONFIGURACIONFACETAS + "_NamespacesOntologia_" + pOntologia;
            mEntityContext.UsarEntityCache = true;
            // Compruebo si está en la caché
            List<OntologiaProyecto> listaOntologias = ObtenerObjetoDeCacheLocal(rawKey) as List<OntologiaProyecto>;

            if (listaOntologias == null)
            {
                listaOntologias = ObtenerObjetoDeCache(rawKey, typeof(List<OntologiaProyecto>)) as List<OntologiaProyecto>;
                AgregarObjetoCacheLocal(pProyectoID, rawKey, listaOntologias);
            }

            if (listaOntologias == null)
            {
                // Si no está, lo cargo y lo almaceno en la caché
                listaOntologias = FacetaCN.ObtenerNamespacesDeOntologia(pOntologia);
                AgregarObjetoCache(rawKey, listaOntologias);
                AgregarObjetoCacheLocal(pProyectoID, rawKey, listaOntologias);
            }
            mEntityContext.UsarEntityCache = false;
            return listaOntologias;
        }

        /// <summary>
        /// Obtiene los parámetros de configuración de un proyecto.
        /// </summary>        
        /// <param name="OrganizacionID">Organizacion en la que se hace la búsqueda</param>
        /// <param name="pProyectoID">Proyecto en el que se hace la búsqueda</param>
        /// <returns>Devuelve un dataset con la caonfiguracion del facetado</returns>
        public DataWrapperFacetas ObtenerTodasFacetasDeProyecto(List<string> pListaItems, Guid pOrganizacionID, Guid pProyectoID, bool pFacetasHome)
        {
            mEntityContext.UsarEntityCache = true;
            string rawKey = NombresCL.CONFIGURACIONFACETAS + "_" + pProyectoID;
            if (pFacetasHome)
            {
                rawKey += "_home";
            }
            if (pListaItems != null)
            {
                foreach (string item in pListaItems)
                {
                    rawKey += "_" + item;
                }
            }

            // Compruebo si está en la caché
            DataWrapperFacetas facetaDW = ObtenerObjetoDeCacheLocal(rawKey) as DataWrapperFacetas;
            if (facetaDW == null)
            {
                facetaDW = ObtenerObjetoDeCache(rawKey, typeof(DataWrapperFacetas)) as DataWrapperFacetas;
                AgregarObjetoCacheLocal(pProyectoID, rawKey, facetaDW);
            }

            if (facetaDW != null)
            {
                DataWrapperFacetas facetaAuxDW = new DataWrapperFacetas();

                try
                {
                    facetaAuxDW.Merge(facetaDW);
                    // Le asigno el creado en esta plataforma, porque si hay campos que no tenía el dataset de caché, luego da problemas cuando intentas acceder a ellos. 
                    // La comprobación de la estructura no sirve, porque la tabla sí contiene la columna, pero la fila no (mu raro). 
                    facetaDW = facetaAuxDW;
                }
                catch (Exception ex)
                {
                    AgregarEntradaTraza(string.Format("No sirve el data set de caché porque ha fallado al hacer un merge con el actual: ", ex.Message));
                    facetaDW = null;
                }

            }

            if (facetaDW == null)
            {
                // Si no está, lo cargo y lo almaceno en la caché

                if (TieneComunidadPadreConfigurada(pProyectoID))
                {
                    facetaDW = FacetaCN.ObtenerTodasFacetasDeProyecto(pOrganizacionID, ProyectoIDPadreEcosistema.Value);
                    ModificarDataWrapperComunidadHija(facetaDW, pProyectoID);
                }
                else
                {
                    facetaDW = FacetaCN.ObtenerTodasFacetasDeProyecto(pOrganizacionID, pProyectoID);
                }
                if (facetaDW != null)
                {
                    facetaDW.CargaRelacionesPerezosasCache();
                }
                AgregarObjetoCache(rawKey, facetaDW);
                AgregarObjetoCacheLocal(pProyectoID, rawKey, facetaDW);
            }
            mEntityContext.UsarEntityCache = false;
            return facetaDW;
        }

        private void ModificarDataWrapperComunidadHija(DataWrapperFacetas facetaDW, Guid pProyectoID)
        {
            /*
            ListaFacetaObjetoConocimiento
            ListaFacetaObjetoConocimientoProyecto
            ListaFacetaFiltroProyecto
            ListaFacetaMultiple
            ListaOntologiaProyecto
            ListaFacetaFiltroHome
            ListaFacetaHome
            ListaFacetaObjetoConocimientoProyectoPenstanya
            ListaFacetaConfigProyMapa 
            ListaFacetaExcluida
            ListaFacetaEntidadesExternas
            ListaFacetaConfigProyChart
            ListaFacetaConfigProyMapa
            ListaConfiguracionConexionGrafo
            ListaFacetaRedireccion
            */
            foreach (FacetaObjetoConocimientoProyecto faceta in facetaDW.ListaFacetaObjetoConocimientoProyecto)
            {
                faceta.ProyectoID = pProyectoID;
            }
            foreach (FacetaFiltroProyecto faceta in facetaDW.ListaFacetaFiltroProyecto)
            {
                faceta.ProyectoID = pProyectoID;
            }
            foreach (FacetaMultiple faceta in facetaDW.ListaFacetaMultiple)
            {
                faceta.ProyectoID = pProyectoID;
            }
            foreach (OntologiaProyecto faceta in facetaDW.ListaOntologiaProyecto)
            {
                faceta.ProyectoID = pProyectoID;
            }
            foreach (FacetaFiltroHome faceta in facetaDW.ListaFacetaFiltroHome)
            {
                faceta.ProyectoID = pProyectoID;
            }
            foreach (FacetaHome faceta in facetaDW.ListaFacetaHome)
            {
                faceta.ProyectoID = pProyectoID;
            }
            foreach (FacetaObjetoConocimientoProyectoPestanya faceta in facetaDW.ListaFacetaObjetoConocimientoProyectoPenstanya)
            {
                faceta.ProyectoID = pProyectoID;
            }
            foreach (FacetaConfigProyMapa faceta in facetaDW.ListaFacetaConfigProyMapa)
            {
                faceta.ProyectoID = pProyectoID;
            }
            foreach (FacetaExcluida faceta in facetaDW.ListaFacetaExcluida)
            {
                faceta.ProyectoID = pProyectoID;
            }
            foreach (FacetaEntidadesExternas faceta in facetaDW.ListaFacetaEntidadesExternas)
            {
                faceta.ProyectoID = pProyectoID;
            }
            foreach (FacetaConfigProyChart faceta in facetaDW.ListaFacetaConfigProyChart)
            {
                faceta.ProyectoID = pProyectoID;
            }
            foreach (FacetaConfigProyMapa faceta in facetaDW.ListaFacetaConfigProyMapa)
            {
                faceta.ProyectoID = pProyectoID;
            }
        }

        /// <summary>
        /// Obtiene los parámetros de configuración de un proyecto.
        /// </summary>        
        /// <param name="ListaItems">Lista de elementos buscados</param>
        /// <param name="OrganizacionID">Organizacion en la que se hace la búsqueda</param>
        /// <param name="pProyectoID">Proyecto en el que se hace la búsqueda</param>
        /// <param name="pFacetasHome">Indica si las facetas para la configuración son para la home</param>
        /// <returns>Devuelve un dataset con la configuración del facetado</returns>
        public DataWrapperFacetas ObtenerFacetasDeProyecto(List<string> pListaItems, Guid pProyectoID, bool pFacetasHome)
        {
            return ObtenerFacetasDeProyecto(pListaItems, null, pProyectoID, pFacetasHome, null);
        }

        /// <summary>
        /// Obtiene los parámetros de configuración de un proyecto.
        /// </summary>        
        /// <param name="ListaItems">Lista de elementos buscados</param>
        /// <param name="OrganizacionID">Organizacion en la que se hace la búsqueda</param>
        /// <param name="pProyectoID">Proyecto en el que se hace la búsqueda</param>
        /// <param name="pFacetasHome">Indica si las facetas para la configuración son para la home</param>
        /// <returns>Devuelve un dataset con la configuración del facetado</returns>
        public DataWrapperFacetas ObtenerFacetasDeProyecto(List<string> pListaItems, Guid? pOrganizacionID, Guid pProyectoID, bool pFacetasHome, Dictionary<string, List<string>> pFiltrosBusqueda)
        {
            mEntityContext.UsarEntityCache = true;
            string rawKey = NombresCL.CONFIGURACIONFACETAS + "_" + pProyectoID;

            if (pOrganizacionID.HasValue && !pOrganizacionID.Equals(ProyectoAD.MetaProyecto))
            {
                rawKey += "_" + pOrganizacionID.Value;
            }

            if (pFacetasHome)
            {
                rawKey += "_home";
            }
            if (pListaItems != null)
            {
                foreach (string item in pListaItems)
                {
                    rawKey += "_" + item;
                }
            }

            DataWrapperFacetas facetaDW = null;
            //Guid? idClavesActualizadas = null;


            // Compruebo si está en la caché
            facetaDW = ObtenerObjetoDeCacheLocal(rawKey) as DataWrapperFacetas;
            if (facetaDW == null)
            {
                facetaDW = ObtenerObjetoDeCache(rawKey, typeof(DataWrapperFacetas)) as DataWrapperFacetas;
                AgregarObjetoCacheLocal(pProyectoID, rawKey, facetaDW);
            }

            if (facetaDW != null)
            {
                DataWrapperFacetas facetaAuxDW = new DataWrapperFacetas();

                try
                {
                    facetaAuxDW.Merge(facetaDW);
                    // Le asigno el creado en esta plataforma, porque si hay campos que no tenía el dataset de caché, luego da problemas cuando intentas acceder a ellos. 
                    // La comprobación de la estructura no sirve, porque la tabla sí contiene la columna, pero la fila no (mu raro). 
                    facetaDW = facetaAuxDW;
                }
                catch (Exception ex)
                {
                    AgregarEntradaTraza(string.Format("No sirve el data set de caché porque ha fallado al hacer un merge con el actual: ", ex.Message));
                    facetaDW = null;
                }
            }

            if (facetaDW == null)
            {
                // Si no está, lo cargo y lo almaceno en la caché
                if (TieneComunidadPadreConfigurada(pProyectoID))
                {
                    facetaDW = FacetaCN.ObtenerFacetasDeProyecto(pListaItems, pOrganizacionID, ProyectoIDPadreEcosistema.Value);
                    ModificarDataWrapperComunidadHija(facetaDW, pProyectoID);
                }
                else
                {
                    facetaDW = FacetaCN.ObtenerFacetasDeProyecto(pListaItems, pOrganizacionID, pProyectoID);
                }


                if (facetaDW != null)
                {
                    facetaDW.CargaRelacionesPerezosasCache();
                }
                AgregarObjetoCache(rawKey, facetaDW);
                AgregarObjetoCacheLocal(pProyectoID, rawKey, facetaDW);
            }

            #region Faceta subType

            if (pFiltrosBusqueda != null && pFiltrosBusqueda.Keys.Any(key => key.EndsWith(FacetaAD.Faceta_Gnoss_SubType)))
            {
                List<string> lsitaItemsConSubType = new List<string>();
                lsitaItemsConSubType.AddRange(pListaItems);

                foreach (string valor in pFiltrosBusqueda.First(pair => pair.Key.EndsWith(FacetaAD.Faceta_Gnoss_SubType)).Value)
                {
                    foreach (OntologiaProyecto filaOnto in facetaDW.ListaOntologiaProyecto.Where(item => item.SubTipos != null))
                    {
                        string valorNamespaceado = FacetaAD.ObtenerValorAplicandoNamespaces(valor, filaOnto, true);
                        if (!string.IsNullOrEmpty(filaOnto.SubTipos) && filaOnto.SubTipos.Contains(valorNamespaceado + "|||"))
                        {
                            if (!lsitaItemsConSubType.Contains(filaOnto.OntologiaProyecto1))
                            {
                                lsitaItemsConSubType.Add(filaOnto.OntologiaProyecto1);
                            }
                            break;
                        }
                    }
                }

                if (lsitaItemsConSubType.Count > pListaItems.Count)
                {
                    // Vuelvo a cargar las facetas, ahora con la lista de Items actualizada según los subtipos:
                    AgregarEntradaTraza(string.Format("Traemos las facetas de proyecto, ahora con los subtipos: {0}", string.Join(",", lsitaItemsConSubType)));
                    facetaDW = ObtenerFacetasDeProyecto(lsitaItemsConSubType, pOrganizacionID, pProyectoID, pFacetasHome, null);
                }
            }

            #endregion
            mEntityContext.UsarEntityCache = false;
            return facetaDW;
        }

        /// <summary>
        /// Ivalida la caché de las facetas de un proyecto
        /// </summary>
        /// <param name="ListaItems">Lista de elementos buscados</param>
        /// <param name="pProyectoID">Proyecto en el que se hace la búsqueda</param>
        /// <param name="pFacetasHome">Indica si las facetas para la configuración son para la home</param>
        public void InvalidarCacheFacetasProyecto(Guid pProyectoID, bool pCachearFacetas)
        {
            string rawKey = NombresCL.CONFIGURACIONFACETAS + "_" + pProyectoID;

            InvalidarCacheQueContengaCadena(rawKey);

            if (!pCachearFacetas)
            {
                AgregarObjetoCache(rawKey + "_Actualizada", Guid.NewGuid(), 18 * 3600);
            }

            VersionarCacheLocal(pProyectoID);
        }

        /// <summary>
        /// Obtiene el filtro de latitud y longitud para la consulta de mapas.
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <param name="pOrganizacionID">ID de org</param>
        /// <param name="pTipoBusqueda">Tipo de búsqueda</param>
        /// <returns>Filtro de latitud y longitud para la consulta de mapas</returns>
        public DataWrapperFacetas ObtenerPropsMapaPerYOrgProyecto(Guid pOrganizacionID, Guid pProyectoID, TipoBusqueda pTipoBusqueda)
        {
            string rawKey = "FiltroPropsMapaPerYOrgProyecto" + "_" + pProyectoID;

            if (pTipoBusqueda == TipoBusqueda.PersonasYOrganizaciones)
            {
                rawKey += "_PerYOrg";
            }

            // Compruebo si está en la caché
            DataWrapperFacetas PropsMapaPerYOrg = ObtenerObjetoDeCacheLocal(rawKey) as DataWrapperFacetas;

            if (PropsMapaPerYOrg == null)
            {
                PropsMapaPerYOrg = ObtenerObjetoDeCache(rawKey, typeof(DataWrapperFacetas)) as DataWrapperFacetas;
                AgregarObjetoCacheLocal(pProyectoID, rawKey, PropsMapaPerYOrg);
            }

            if (PropsMapaPerYOrg == null)
            {
                // Si no está, lo cargo y lo almaceno en la caché
                PropsMapaPerYOrg = FacetaCN.ObtenerPropsMapaPerYOrgProyecto(pOrganizacionID, pProyectoID, pTipoBusqueda);
                AgregarObjetoCache(rawKey, PropsMapaPerYOrg);
                AgregarObjetoCacheLocal(pProyectoID, rawKey, PropsMapaPerYOrg);
            }

            return PropsMapaPerYOrg;
        }


        #region Propiedades

        /// <summary>
        /// Clave para la caché
        /// </summary>
        public override string[] ClaveCache
        {
            get
            {
                return mMasterCacheKeyArray;
            }
        }

        /// <summary>
        /// Clase de negocio
        /// </summary>
        protected FacetaCN FacetaCN
        {
            get
            {
                if (mFacetaCN == null)
                {
                    if (mFicheroConfiguracionBD != null && mFicheroConfiguracionBD != "")
                    {
                        mFacetaCN = new FacetaCN(mFicheroConfiguracionBD, mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetaCN>(), mLoggerFactory);
                    }
                    else
                    {
                        mFacetaCN = new FacetaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetaCN>(), mLoggerFactory);
                    }
                }

                return mFacetaCN;
            }
        }

        #endregion Propiedades
    }
}
