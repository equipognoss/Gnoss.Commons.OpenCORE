using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Faceta;
using Es.Riam.Gnoss.AD.Facetado;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.Usuarios;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Semantica.OWL;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Logica.Facetado
{
    public class FacetaCN : BaseCN, IDisposable
    {
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        #region Constructores

        /// <summary>
        /// Constructor sin parámetros
        /// </summary>
        public FacetaCN(EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<FacetaCN> logger, ILoggerFactory loggerFactory)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication,logger,loggerFactory)
        {
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            FacetaAD = new FacetaAD(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetaAD>(), mLoggerFactory);
        }

        /// <summary>
        /// Constructor para TablasDeConfiguracionCN
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Fichero de configuración</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public FacetaCN(string pFicheroConfiguracionBD, EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<FacetaCN> logger, ILoggerFactory loggerFactory)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        {
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            FacetaAD = new FacetaAD(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetaAD>(), mLoggerFactory);
        }

        #endregion

        #region Metodos generales

        /// <summary>
        /// Obtiene el número de filtros de categoría que tiene configurados un proyecto
        /// </summary>
        /// <param name="ProyectoID">Proyecto en el que se hace la búsqueda</param>
        public int ObtenerNumeroFiltrosCategoriasDeProyecto(Guid pOrganizacionID, Guid pProyectoID)
        {
            return FacetaAD.ObtenerNumeroFiltrosCategoriasDeProyecto(pOrganizacionID, pProyectoID);
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
            return FacetaAD.ObtenerPropsMapaPerYOrgProyecto(pOrganizacionID, pProyectoID, pTipoBusqueda);
        }

        public List<FacetaObjetoConocimientoProyecto> ObtenerFacetasObjetoConocimientoProyectoDeOntologia(string pNombreOntologia, Guid pProyectoID)
        {
            return FacetaAD.ObtenerFacetasObjetoConocimientoProyectoDeOntologia(pNombreOntologia, pProyectoID);

        }

        /// <summary>
        /// Obtiene la configuración de la tabla FacetaObjetoConocimientoProyecto
        /// </summary>
        /// <returns>DataSet con la configuración de las Facetas almacenada en la aplicación</returns>
        public DataWrapperFacetas ObtenerFacetaObjetoConocimientoProyecto(Guid pOrganizacionID, Guid pProyectoID)
        {
            return FacetaAD.ObtenerFacetaObjetoConocimientoProyecto(pOrganizacionID, pProyectoID, false);
        }

        /// <summary>
        /// Obtiene la configuración de la tabla FacetaObjetoConocimientoProyecto
        /// </summary>
        /// <returns>DataSet con la configuración de las Facetas almacenada en la aplicación</returns>
        public DataWrapperFacetas ObtenerFacetaObjetoConocimientoProyecto(Guid pOrganizacionID, Guid pProyectoID, bool pObtenerOcultas)
        {
            return FacetaAD.ObtenerFacetaObjetoConocimientoProyecto(pOrganizacionID, pProyectoID, pObtenerOcultas);
        }

        public DataWrapperFacetas ObtenerFacetaObjetoConocimiento(string pObjetoConocimiento)
        {
            return FacetaAD.ObtenerFacetaObjetoConocimiento(pObjetoConocimiento);
        }

        public DataWrapperFacetas ObtenerFacetasAdministrarProyecto(Guid pProyectoID)
        {
            return FacetaAD.ObtenerFacetasAdministrarProyecto(pProyectoID);
        }

        /// <summary>
        /// Obtiene los parámetros de configuración de un proyecto.
        /// </summary>        
        /// <param name="ListaItems">Lista de elementos buscados</param>
        /// <param name="OrganizacionID">Organizacion en la que se hace la búsqueda</param>
        /// <param name="pProyectoID">Proyecto en el que se hace la búsqueda</param>
        /// <returns>Devuelve un dataset con la configuracion del facetado</returns>
        public DataWrapperFacetas ObtenerFacetasDeProyecto(List<string> ListaItems, Guid? pOrganizacionID, Guid pProyectoID)
        {
            return FacetaAD.ObtenerFacetasDeProyecto(ListaItems, pOrganizacionID, pProyectoID);
        }

        public void CargarFacetasObjetoConocimientoProyectoyPestanya(DataWrapperFacetas pFacetaDW, Guid pProyectoID, Guid? pOrganizacionID)
        {
            FacetaAD.CargarFacetasObjetoConocimientoProyectoPestanya(pFacetaDW, pProyectoID, pOrganizacionID);
            FacetaAD.CargarFacetasObjetoConocimientoProyecto(pFacetaDW, pProyectoID, pOrganizacionID);
        }

        /// <summary>
        /// Obtiene los datos para una consulto usando charts.
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <param name="pChartID">ID de chart o Guid.Empty si no se quiere especificar uno</param>
        /// <returns>datos para una consulto usando charts</returns>
        public DataWrapperFacetas ObtenerDatosChartProyecto(Guid pOrganizacionID, Guid pProyectoID, Guid pChartID)
        {
            return FacetaAD.ObtenerDatosChartProyecto(pOrganizacionID, pProyectoID, pChartID);
        }

        /// <summary>
        /// Devuelve la fila de la ontología en ese proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <param name="pEnlace">Enlace de la ontología</param>
        /// <returns>La fila de la ontología en el proyecto</returns>
        public OntologiaProyecto ObtenerOntologiaProyectoPorEnlace(Guid pProyectoID, string pEnlace)
        {
            return FacetaAD.ObtenerOntologiaProyectoPorEnlace(pProyectoID, pEnlace);
        }

        public List<OntologiaProyecto> ObtenerOntologias(Guid pProyectoID, bool pAgregarNamespacesComoOntologias)
        {
            List<OntologiaProyecto> listaOntologiaProyecto = FacetaAD.ObtenerOntologiasProyecto(Guid.Empty, pProyectoID, pAgregarNamespacesComoOntologias);
            return listaOntologiaProyecto;
        }

        public List<OntologiaProyecto> ObtenerOntologias(Guid pProyectoID)
        {
            return FacetaAD.ObtenerOntologiasProyecto(Guid.Empty, pProyectoID);
        }

        public List<string> ObtenerPredicadosSemanticos(Guid pOrganizacionID, Guid pProyectoID)
        {
            return FacetaAD.ObtenerPredicadosSemanticos(pOrganizacionID, pProyectoID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pFacetaDS"></param>
        /// <param name="pOrganizacionID"></param>
        /// <param name="pProyectoID"></param>
        public List<OntologiaProyecto> ObtenerOntologiasProyecto(Guid pOrganizacionID, Guid pProyectoID, bool pAgregarNamespacesComoOntologias, bool pSoloBuscables)
        {
            return FacetaAD.ObtenerOntologiasProyecto(pOrganizacionID, pProyectoID, pAgregarNamespacesComoOntologias, pSoloBuscables);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pFacetaDS"></param>
        /// <param name="pOrganizacionID"></param>
        /// <param name="pProyectoID"></param>
        public List<OntologiaProyecto> ObtenerOntologiasProyecto(Guid pOrganizacionID, Guid pProyectoID, bool pAgregarNamespacesComoOntologias)
        {
            return FacetaAD.ObtenerOntologiasProyecto(pOrganizacionID, pProyectoID, pAgregarNamespacesComoOntologias);
        }

        public List<OntologiaProyecto> ObtenerOntologiasProyecto(Guid pOrganizacionID, Guid pProyectoID)
        {
            return FacetaAD.ObtenerOntologiasProyecto(pOrganizacionID, pProyectoID);
        }

        public void ObtenerOntologiasProyecto(DataWrapperFacetas pDataWrapperFacetas, Guid pOrganizacionID, Guid pProyectoID)
        {
            FacetaAD.ObtenerOntologiasProyecto(pDataWrapperFacetas, pOrganizacionID, pProyectoID);
        }
        /// <summary>
        /// Guarda en la tabla FacetaEntidadesExternas la faceta de la ontología
        /// </summary>
        public void GuardarFacetasEntidadesExternas(FacetaEntidadesExternas facetaEntidadExterna)
        {
            FacetaAD.GuardarFacetasEntidadesExternas(facetaEntidadExterna);
        }

        /// <summary>
        /// Obtiene los namespaces de una ontología
        /// </summary>
        /// <param name="pOntologia">Ontología</param>
        /// <returns>DataSet con los namespaces de una ontología</returns>
        public List<OntologiaProyecto> ObtenerNamespacesDeOntologia(string pOntologia)
        {
            return FacetaAD.ObtenerNamespacesDeOntologia(pOntologia);
        }

        /// <summary>
        /// Comprueba si un usuario es administrador de alguna comunidad que tenga una ontologia concreta
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <param name="pOntologia">Ontología de la que el usuario debe ser administrador</param>
        /// <returns>Verdad si el usuario administra alguna comunidad que contenga esta ontología</returns>
        public bool ComprobarUsuarioAdministraOntologia(Guid pUsuarioID, string pOntologia)
        {
            return FacetaAD.ComprobarUsuarioAdministraOntologia(pUsuarioID, pOntologia);
        }

        /// <summary>
        /// Obtenemos el modelo de base de datos de las facetas para la faceta y proyecto pasado por parámetro
        /// </summary>
        /// <param name="pFaceta">Clave de faceta que queremos obtener</param>
        /// <param name="pProyectoID">Identificador del proytecto del cual queremos obtener las facetas</param>
        /// <returns>El modelo de base de datos de las facetas solicitadas por parámetro</returns>
        public List<FacetaObjetoConocimientoProyecto> ObtenerFacetaObjetoConocimientoPorFaceta(string pFaceta, Guid pProyectoID)
        {
            return FacetaAD.ObtenerFacetaObjetoConocimientoPorFaceta(pFaceta, pProyectoID);
        }

        /// <summary>
        /// Obtiene los parámetros de configuración de un proyecto.
        /// </summary>        
        /// <param name="OrganizacionID">Organizacion en la que se hace la búsqueda</param>
        /// <param name="pProyectoID">Proyecto en el que se hace la búsqueda</param>
        /// <returns>Devuelve un dataset con la caonfiguracion del facetado</returns>
        public DataWrapperFacetas ObtenerTodasFacetasDeProyecto(Guid pOrganizacionID, Guid pProyectoID)
        {
            return FacetaAD.ObtenerTodasFacetasDeProyecto(pOrganizacionID, pProyectoID);
        }

        /// <summary>
        /// Carga en el dataset pasado como parámetro la tabla FacetaExcluida
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pFacetaDS">Dataset de facetas</param>
        /// <returns>Dataset original con la tabla FacetaExcluida añadida</returns>
        public void CargarFacetasExcluidas(Guid pOrganizacionID, Guid pProyectoID, DataWrapperFacetas pFacetaDW)
        {
            FacetaAD.CargarFacetasExcluidas(pOrganizacionID, pProyectoID, pFacetaDW);
        }

        /// <summary>
        /// Carga en el dataset pasado como parámetro la tabla FacetaEntidadesExternas
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pFacetaDS">Dataset de facetas</param>
        /// <returns>Dataset original con la tabla FacetaEntidadesExternas añadida</returns>
        public void CargarFacetasEntidadesExternas(Guid pOrganizacionID, Guid pProyectoID, DataWrapperFacetas pFacetaDW)
        {
            FacetaAD.CargarFacetasEntidadesExternas(pOrganizacionID, pProyectoID, pFacetaDW);
        }

        /// <summary>
        /// Carga en el dataset pasado como parámetro la tabla FacetaRedireccion
        /// </summary>
        /// <param name="pFacetaDS">Dataset de facetas</param>
        /// <returns>Dataset original con la tabla FacetaEntidadesExternas añadida</returns>
        public void CargarFacetaRedireccion(DataWrapperFacetas pFacetaDW)
        {
            FacetaAD.CargarFacetaRedireccion(pFacetaDW);
        }

        /// <summary>
        /// Carga en el dataset pasado como parámetro la tabla FacetaRedireccion
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organizacion</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pFacetaDS">Dataset de facetas</param>
        /// <returns>Dataset original con la tabla FacetaEntidadesExternas añadida</returns>
        public void CargarFacetaConfigProyMapa(Guid pOrganizacionID, Guid pProyectoID, DataWrapperFacetas pFacetaDW)
        {
            FacetaAD.CargarFacetaConfigProyMapa(pOrganizacionID, pProyectoID, pFacetaDW);
        }

        /// <summary>
        /// Carga en el dataset pasado como parámetro la tabla FacetaRedireccion
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organizacion</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pFacetaDS">Dataset de facetas</param>
        /// <returns>Dataset original con la tabla FacetaEntidadesExternas añadida</returns>
        public void CargarFacetaConfigProyChart(Guid pOrganizacionID, Guid pProyectoID, DataWrapperFacetas pFacetaDW)
        {
            FacetaAD.CargarFacetaConfigProyChart(pOrganizacionID, pProyectoID, pFacetaDW);
        }

        /// <summary>
        /// Devuelve el elemento de base de datos FacetaConfigProyChart a partir del identificador del chart
        /// </summary>
        /// <param name="pChartID">Identificador del chart</param>
        /// <returns>FacetaConfigProyChart buscado</returns>
        public FacetaConfigProyChart ObtenerFacetaConfigProyChartPorID(Guid pChartID)
        {
            return FacetaAD.ObtenerFacetaConfigProyChartPorID(pChartID);
        }

        /// <summary>
        /// Devuelve una lista de FacetaConfigProyChart de la base de datos a partir de una lista de identificadores
        /// </summary>
        /// <param name="pChartsIds">Lista con identificadores de charts</param>
        /// <returns>Lista de FacetaConfigProyChart buscados</returns>
        public List<FacetaConfigProyChart> ObtenerListaFacetaConfigProyChartPorIDs(List<Guid> pChartsIds)
        {
            return FacetaAD.ObtenerListaFacetaConfigProyChartPorIDs(pChartsIds);
        }

        /// <summary>
        /// Carga en el dataset pasado como parámetro la tabla FacetaConfigProyRanfoFecha
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organizacion</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pFacetaDS">Dataset de facetas</param>
        public void CargarFacetaConfigProyRanfoFecha(Guid pOrganizacionID, Guid pProyectoID, DataWrapperFacetas pFacetaDW)
        {
            FacetaAD.CargarFacetaConfigProyRanfoFecha(pOrganizacionID, pProyectoID, pFacetaDW);
        }

        /// <summary>
        /// Carga en el dataset pasado como parámetro la tabla ConfiguracionConexionGrafo
        /// </summary>
        /// <param name="pFacetaDS">Dataset de facetas</param>
        /// <returns>Dataset original con la tabla ConfiguracionConexionGrafo añadida</returns>
        public void CargarConfiguracionConexionGrafo(DataWrapperFacetas pFacetaDW)
        {
            FacetaAD.CargarConfiguracionConexionGrafo(pFacetaDW);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pFaceta">Faceta</param>
        /// <returns></returns>
        public string ObtenerNombreFacetaRedireccion(string pNombre)
        {
            return FacetaAD.ObtenerNombreFacetaRedireccion(pNombre);
        }

        /// <summary>
        /// Obtiene el número de proyectos afetados por esa faceta
        /// </summary>
        /// <param name="pFaceta"></param>
        /// <returns></returns>
        public int ObtenerNumeroProyectosConFaceta(string pFaceta)
        {
            return FacetaAD.ObtenerNumeroProyectosConFaceta(pFaceta);
        }

        /// <summary>
        /// Obtiene la personalización de facetas de un proyecto.
        /// </summary>        
        /// <param name="pProyectoID">Proyecto en el que se hace la búsqueda</param>
        /// <returns>Devuelve un dataset con la caonfiguracion del facetado (Tabla FacetaObjetoConocimientoProyecto)</returns>
        public DataWrapperFacetas ObtenerFacetasConPersonalizacionDeProyecto(Guid pProyectoID)
        {
            return FacetaAD.ObtenerFacetasConPersonalizacionDeProyecto(pProyectoID);
        }

        public void EliminarFacetas(List<FacetaObjetoConocimientoProyecto> pFacetas)
        {
            FacetaAD.EliminarFacetas(pFacetas);
        }

        public void GuardarCambios()
        {
            FacetaAD.GuardarCambios();
        }

        #endregion Metodos generales

        #region Dispose

        /// <summary>
        /// Determina si está disposed
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Destructor
        /// </summary>
        ~FacetaCN()
        {
            //Libero los recursos
            Dispose(false);
        }

        /// <summary>
        /// Libera los recursos
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            //impido que se finalice dos veces este objeto
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Libera los recursos
        /// </summary>
        /// <param name="disposing">Determina si se está llamando desde el Dispose()</param>
        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                disposed = true;
                if (disposing && FacetaAD != null)
                {
                    //Libero todos los recursos administrados que he añadido a esta clase                    
                    FacetaAD.Dispose();
                }

                FacetaAD = null;
            }
        }

        #endregion dispose

        #region Propiedades

        /// <summary>
        /// DataAdapter de TablasDeConfiguracion
        /// </summary>
        private FacetaAD FacetaAD
        {
            get
            {
                return (FacetaAD)AD;
            }
            set
            {
                AD = value;
            }
        }

        #endregion
    }
}