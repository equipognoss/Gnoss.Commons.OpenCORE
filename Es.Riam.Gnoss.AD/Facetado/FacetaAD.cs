using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.Amigos;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Faceta;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Es.Riam.Gnoss.AD.Facetado
{
    /// <summary>
    /// Tipos de algoritmos de transformacion de faceta
    /// </summary>
    public enum TiposAlgoritmoTransformacion
    {
        /// <summary>
        /// Categoría
        /// </summary>
        Categoria = 0,
        /// <summary>
        /// Categoría
        /// </summary>
        Com2 = 1,
        /// <summary>
        /// Categoría
        /// </summary>
        ComContactos = 1,
        /// <summary>
        /// Categoría
        /// </summary>
        ComunidadesRecomendadas = 1,
        /// <summary>
        /// Codigo Postal
        /// </summary>
        CodPost = 3,
        /// <summary>
        /// Estado
        /// </summary>
        Estado = 4,
        /// <summary>
        /// Estado
        /// </summary>
        EstadoCorreccion = 5,
        /// <summary>
        /// Estado
        /// </summary>
        Fechas = 6,
        /// <summary>
        /// Codigo Postal
        /// </summary>
        NCer = 7,
        /// <summary>
        /// Codigo Postal
        /// </summary>
        Ninguno = 8,
        /// <summary>
        /// Rangos
        /// </summary>
        Rangos = 9,
        /// <summary>
        /// Codigo Postal
        /// </summary>
        SinCaja = 10,
        /// <summary>
        /// Tipo
        /// </summary>
        Tipo = 11,
        /// <summary>
        /// Tipo
        /// </summary>
        TipoContactos = 11,
        /// <summary>
        /// Tipo de documento
        /// </summary>
        TipoDoc = 13,
        /// <summary>
        /// Tipo de documento
        /// </summary>
        TipoDocExt = 14,
        /// <summary>
        /// Tipo de documento
        /// </summary>
        CategoriaArbol = 15,
        /// <summary>
        /// Tesauro Semántico
        /// </summary>
        TesauroSemantico = 16,
        /// <summary>
        /// MultiIdioma
        /// </summary>
        MultiIdioma = 17,
        /// <summary>
        /// MultiIdioma
        /// </summary>
        Calendario = 18,
        /// <summary>
        /// Siglo
        /// </summary>
        Siglo = 19,
        /// <summary>
        /// Calendario con Rangos Desde Hasta
        /// </summary>
        CalendarioConRangos = 20,
        /// <summary>
        /// Booleano
        /// </summary>
        Booleano = 21,
        /// <summary>
        /// Tesauro Semántico Ordenado
        /// </summary>
        TesauroSemanticoOrdenado = 22,
        /// <summary>
        /// Estado Usuario [IdentidadAD.TipoMiembros (Activos, Expulsados, Bloqueados)]
        /// </summary>
        EstadoUsuario = 23,
        /// <summary>
        /// Rol usuario [ProyectoAD.TipoRolUsuario (Usuario, Administrador, Supervisor)]
        /// </summary>
        RolUsuario = 24,
        /// <summary>
        /// Rol usuario [ProyectoAD.TipoRolUsuario (Usuario, Administrador, Supervisor)]
        /// </summary>
        Multiple = 25,
        /// <summary>
        /// Obtenemos solamente la fecha máxima y mínima
        /// </summary>
        FechaMinMax = 26
    }

    public enum TipoPropiedadFaceta
    {
        /// <summary>
        /// Texto
        /// </summary>
        Texto = 0,
        /// <summary>
        /// Fecha
        /// </summary>
        Fecha = 1,
        /// <summary>
        /// Número
        /// </summary>
        Numero = 2,
        /// <summary>
        /// Nulo
        /// </summary>
        NULL = 0,
        /// <summary>
        /// Fecha
        /// </summary>
        Calendario = 3,
        /// <summary>
        /// Siglo
        /// </summary>
        Siglo = 4,
        /// <summary>
        /// Texto invariable, no necesita convertirse a minusculas
        /// </summary>
        TextoInvariable = 5,
        /// <summary>
        /// Calendario con rangos Desde Hasta debajo del calendario.
        /// </summary>
        CalendarioConRangos = 6,
        /// <summary>
        ///  DE USO INTERNO: Sólo para calcular los meses de un año en una faceta de tipo Fecha
        /// </summary>
        FechaMeses = 10,
        /// <summary>
        /// DE USO INTERNO: Se utiliza para obtener unicamente la fecha mínima y máxima de una búsqueda
        /// </summary>
        FechaMinMax = 20
    }

    public enum TipoDisenio
    {
        DesdeHastaDiasMesAño = 0,

        ListaOrdCantidad = 1,

        Dafo = 2,

        ListaMayorAMenor = 3,

        ListaMenorAMayor = 4,

        Calendario = 5,

        RangoSoloDesde = 6,

        RangoSoloHasta = 7,

        CalendarioConRangos = 8,

        ListaOrdCantidadTesauro = 9
    }

    public enum TipoMostrarSoloCaja
    {
        PorDefecto = 0,

        SoloCajaPrimeraPagina = 1,

        SoloCajaSiempre = 2

    }

    public enum FacetaMayuscula
    {
        Nada = 0,
        MayusculasTodasPalabras = 1,
        MayusculasTodoMenosArticulos = 2,
        MayusculasPrimeraPalabra = 3,
        MayusculasTodasLetras = 4
    }

    public class FacetaAD : BaseAD
    {
        #region Miembros

        /// <summary>
        /// Faceta de GNOSS que se usa para indicar subtipos de entidades.
        /// </summary>
        public const string Faceta_Gnoss_SubType = "gnoss:type";
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;

        #endregion

        #region Constructores

        public FacetaAD(LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<FacetaAD> logger, ILoggerFactory loggerFactory)
            : base(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        { 
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        /// <summary>
        /// Constructor para TablasDeConfiguracionAD
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Fichero de configuración</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public FacetaAD(string pFicheroConfiguracionBD, LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<FacetaAD> logger, ILoggerFactory loggerFactory)
            : base(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        { 
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        #endregion

        #region Métodos generales

        /// <summary>
        /// Guarda en la tabla FacetaEntidadesExterna los datos al subir la ontología
        /// </summary>
        public void GuardarFacetasEntidadesExternas(FacetaEntidadesExternas facetaEntidadExterna)
        {
            mEntityContext.FacetaEntidadesExternas.Add(facetaEntidadExterna);
        }

        /// <summary>
        /// Obtiene el número de proyectos afetados por esa faceta
        /// </summary>
        /// <param name="pFaceta">clave de la faceta</param>
        /// <returns>Entero con el número de proyectos afectados por esa faceta</returns>
        public int ObtenerNumeroProyectosConFaceta(string pFaceta)
        {
            return mEntityContext.FacetaObjetoConocimientoProyecto.Where(item => item.Faceta.Equals(pFaceta)).Select(item => item.Faceta).Count();
        }

        /// <summary>
        /// Carga las tablas del FacetaDS
        /// </summary>        
        /// <param name="OrganizacionID">Organizacion en la que se hace la búsqueda</param>
        /// <param name="pProyectoID">Proyecto en el que se hace la búsqueda</param>
        /// <returns>Devuelve un dataset con la configuración del facetado de un proyecto, salvo las tablas FacetaMayusculasProyecto y FacetaRedireccion</returns>
        public DataWrapperFacetas ObtenerTodasFacetasDeProyecto(Guid pOrganizacionID, Guid pProyectoID)
        {
            DataWrapperFacetas facetaDW = ObtenerFacetasDeProyecto(null, pOrganizacionID, pProyectoID);
            CargarFacetasExcluidas(pOrganizacionID, pProyectoID, facetaDW);
            CargarFacetasEntidadesExternas(pOrganizacionID, pProyectoID, facetaDW);
            CargarFacetaConfigProyChart(pOrganizacionID, pProyectoID, facetaDW);
            CargarFacetaConfigProyMapa(pOrganizacionID, pProyectoID, facetaDW);
            CargarConfiguracionConexionGrafo(facetaDW);
            CargarFacetaRedireccion(facetaDW);

            return facetaDW;
        }

        /// <summary>
        /// Obtiene la configuración de las facetas de un proyecto.
        /// </summary>        
        /// <param name="ListaItems">Lista de elementos buscados</param>
        /// <param name="OrganizacionID">Organizacion en la que se hace la búsqueda</param>
        /// <param name="pProyectoID">Proyecto en el que se hace la búsqueda</param>
        /// <returns>Devuelve un dataset con la caonfiguracion del facetado</returns>
        public DataWrapperFacetas ObtenerFacetasDeProyecto(List<string> pListaItems, Guid? pOrganizacionID, Guid pProyectoID)
        {
            DataWrapperFacetas facetaDW = new DataWrapperFacetas();

            CargaFacetaObjetoConocimiento(facetaDW, pListaItems, pProyectoID);

            CargaFacetaObjetoConocimientoProyecto(facetaDW, pListaItems, pOrganizacionID, pProyectoID);

            CargaFacetaFiltroProyecto(facetaDW, pListaItems, pOrganizacionID, pProyectoID);

            CargaFacetaMultipleDeProyecto(facetaDW, pListaItems, pOrganizacionID, pProyectoID);

            facetaDW.ListaOntologiaProyecto = CargarOntologiaProyecto(pProyectoID, pOrganizacionID);

            CargarFiltroHome(facetaDW, pProyectoID, pOrganizacionID);

            CargarFacetasObjetoConocimientoProyectoPestanya(facetaDW, pProyectoID, pOrganizacionID);

            CargarFacetaConfigProyMapa(pOrganizacionID, pProyectoID, facetaDW);

            return facetaDW;
        }

        private void CargaFacetaObjetoConocimiento(DataWrapperFacetas pDataWrapperFacetas, List<string> pListaItems, Guid pProyectoID)
        {
            if (pListaItems == null || (pListaItems != null && pListaItems.Count == 0))
            {
                var consulta = mEntityContext.FacetaObjetoConocimiento.Where(facetaObjetoConocimiento => facetaObjetoConocimiento.EsPorDefecto && !mEntityContext.FacetaExcluida.Any(facetaExcluida => facetaExcluida.ProyectoID.Equals(pProyectoID) && facetaExcluida.Faceta.Equals(facetaObjetoConocimiento.Faceta)));
                pDataWrapperFacetas.ListaFacetaObjetoConocimiento = consulta.ToList();
            }
            else
            {
                pDataWrapperFacetas.ListaFacetaObjetoConocimiento = mEntityContext.FacetaObjetoConocimiento.Where(facetaObjetoConocimiento => facetaObjetoConocimiento.EsPorDefecto.Equals(true) && !(mEntityContext.FacetaExcluida.Any(facetaExcluida => facetaExcluida.ProyectoID.Equals(pProyectoID) && facetaExcluida.Faceta.Equals(facetaObjetoConocimiento.Faceta))) && pListaItems.Contains(facetaObjetoConocimiento.ObjetoConocimiento)).ToList();
            }
        }

        public void CargaFacetaFiltroProyecto(DataWrapperFacetas pDataWrapperFacetas, List<string> pListaItems, Guid? pOrganizacionID, Guid pProyectoID)
        {
            var select = mEntityContext.FacetaFiltroProyecto.Where(item => item.ProyectoID.Equals(pProyectoID));

            if (pOrganizacionID.HasValue)
            {
                select = select.Where(item => item.OrganizacionID.Equals(pOrganizacionID.Value));
            }

            if (pListaItems != null && pListaItems.Count > 0)
            {
                select = select.Where(facetaFiltroProy => pListaItems.Contains(facetaFiltroProy.ObjetoConocimiento));
            }

            pDataWrapperFacetas.ListaFacetaFiltroProyecto = pDataWrapperFacetas.ListaFacetaFiltroProyecto.Union(select.ToList()).ToList();
        }

        private void CargaFacetaMultipleDeProyecto(DataWrapperFacetas pFacetaDW, List<string> pListaItems, Guid? pOrganizacionID, Guid pProyectoID)
        {
            var select = mEntityContext.FacetaMultiple.Where(item => item.ProyectoID.Equals(pProyectoID));

            if (pOrganizacionID.HasValue)
            {
                select = select.Where(item => item.OrganizacionID.Equals(pOrganizacionID.Value));
            }

            if (pListaItems != null && pListaItems.Count > 0)
            {
                select = select.Where(facetaMultiple => pListaItems.Contains(facetaMultiple.ObjetoConocimiento));
            }

            pFacetaDW.ListaFacetaMultiple = pFacetaDW.ListaFacetaMultiple.Union(select.ToList()).ToList();
        }


        public List<OntologiaProyecto> CargarOntologiaProyecto(Guid pProyectoID, Guid? pOrganizacionID)
        {
            var select = mEntityContext.OntologiaProyecto.Where(item => item.ProyectoID.Equals(pProyectoID));

            if (pOrganizacionID.HasValue)
            {
                select = select.Where(item => item.OrganizacionID.Equals(pOrganizacionID.Value));
            }
            return select.ToList();
        }

        public void CargarFiltroHome(DataWrapperFacetas pFacetaDW, Guid pProyectoID, Guid? pOrganizacionID)
        {
            var select = mEntityContext.FacetaFiltroHome.Where(item => item.ProyectoID.Equals(pProyectoID));

            if (pOrganizacionID.HasValue)
            {
                select = select.Where(item => item.OrganizacionID.Equals(pOrganizacionID.Value));
            }

            pFacetaDW.ListaFacetaFiltroHome = pFacetaDW.ListaFacetaFiltroHome.Union(select.ToList()).Distinct().ToList();
        }

        public void CargarFacetasObjetoConocimientoProyectoPestanya(DataWrapperFacetas pFacetaDW, Guid pProyectoID, Guid? pOrganizacionID)
        {
            var select = mEntityContext.FacetaObjetoConocimientoProyectoPestanya.Where(item => item.ProyectoID.Equals(pProyectoID));

            if (pOrganizacionID.HasValue)
            {
                select = select.Where(item => item.OrganizacionID.Equals(pOrganizacionID.Value));
            }

            pFacetaDW.ListaFacetaObjetoConocimientoProyectoPenstanya = pFacetaDW.ListaFacetaObjetoConocimientoProyectoPenstanya.Union(select).Distinct().ToList();
        }

        public void CargarFacetasObjetoConocimientoProyecto(DataWrapperFacetas pFacetaDW, Guid pProyectoID, Guid? pOrganizacionID)
        {
            var select = mEntityContext.FacetaObjetoConocimientoProyecto.Where(item => item.ProyectoID.Equals(pProyectoID));

            if (pOrganizacionID.HasValue)
            {
                select = select.Where(item => item.OrganizacionID.Equals(pOrganizacionID.Value));
            }

            pFacetaDW.ListaFacetaObjetoConocimientoProyecto = pFacetaDW.ListaFacetaObjetoConocimientoProyecto.Union(select).Distinct().ToList();
        }

        /// <summary>
        /// Carga en el dataset pasado como parámetro la tabla FacetaRedireccion
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organizacion</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pFacetaDS">Dataset de facetas</param>
        public void CargarFacetaConfigProyMapa(Guid? pOrganizacionID, Guid pProyectoID, DataWrapperFacetas pFacetaDW)
        {
            var select = mEntityContext.FacetaConfigProyMapa.Where(item => item.ProyectoID.Equals(pProyectoID));

            if (pOrganizacionID.HasValue)
            {
                select = select.Where(item => item.OrganizacionID.Equals(pOrganizacionID.Value));
            }

            pFacetaDW.ListaFacetaConfigProyMapa = pFacetaDW.ListaFacetaConfigProyMapa.Union(select.ToList()).ToList();
        }


        /// <summary>
        /// Primero cargamos cualquier item != 'Recursos' después los recursos y finalmente se fusionan para que aparezcan los últimos.
        /// </summary>
        /// <param name="facetaDS">DS donde se van a cargar los elementos</param>
        /// <param name="ListaItems">Items que se deben cargar</param>
        /// <param name="pOrganizacionID">OrganizaciónID</param>
        /// <param name="pProyectoID">ProyectoID</param>
        private void CargaFacetaObjetoConocimientoProyecto(DataWrapperFacetas pDataWrapperFacetas, List<string> ListaItems, Guid? pOrganizacionID, Guid pProyectoID)
        {
            var query = mEntityContext.FacetaObjetoConocimientoProyecto.Where(item => item.ProyectoID.Equals(pProyectoID));

            if (pOrganizacionID.HasValue)
            {
                query = query.Where(item => item.OrganizacionID.Equals(pOrganizacionID.Value));
            }

            if (ListaItems != null && ListaItems.Count > 0)
            {
                query = query.Where(item => ListaItems.Contains(item.ObjetoConocimiento));

            }

            pDataWrapperFacetas.ListaFacetaObjetoConocimientoProyecto = query.ToList();
        }

        /// <summary>
        /// Obtiene la personalización de facetas de un proyecto.
        /// </summary>        
        /// <param name="pProyectoID">Proyecto en el que se hace la búsqueda</param>
        /// <returns>Devuelve un dataset con la caonfiguracion del facetado (Tabla FacetaObjetoConocimientoProyecto)</returns>
        public DataWrapperFacetas ObtenerFacetasConPersonalizacionDeProyecto(Guid pProyectoID)
        {
            DataWrapperFacetas dataWrapperFacetas = new DataWrapperFacetas();

            dataWrapperFacetas.ListaFacetaObjetoConocimientoProyecto = mEntityContext.FacetaObjetoConocimientoProyecto.Where(item => item.ProyectoID.Equals(pProyectoID)).ToList();

            return dataWrapperFacetas;
        }

        /// <summary>
        /// Carga en el dataset pasado como parámetro la tabla FacetaExcluida
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pFacetaDS">Dataset de facetas</param>
        public void CargarFacetasExcluidas(Guid pOrganizacionID, Guid pProyectoID, DataWrapperFacetas pFacetaDW)
        {
            pFacetaDW.ListaFacetaExcluida = mEntityContext.FacetaExcluida.Where(item => item.OrganizacionID.Equals(pOrganizacionID) && item.ProyectoID.Equals(pProyectoID)).ToList();
        }

        /// <summary>
        /// Carga en el dataset pasado como parámetro la tabla FacetaEntidadesExternas
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organizacion</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pFacetaDS">Dataset de facetas</param>
        public void CargarFacetasEntidadesExternas(Guid pOrganizacionID, Guid pProyectoID, DataWrapperFacetas pDataWrapperFacetas)
        {
            pDataWrapperFacetas.ListaFacetaEntidadesExternas = mEntityContext.FacetaEntidadesExternas.Where(item => item.OrganizacionID.Equals(pOrganizacionID) && item.ProyectoID.Equals(pProyectoID)).ToList();
        }

        /// <summary>
        /// Carga en el dataset pasado como parámetro la tabla FacetaRedireccion
        /// </summary>
        /// <param name="pFacetaDS">Dataset de facetas</param>
        public void CargarFacetaRedireccion(DataWrapperFacetas pFacetaDW)
        {
            pFacetaDW.ListaFacetaRedireccion = mEntityContext.FacetaRedireccion.ToList();
        }

        /// <summary>
        /// Carga en el dataset pasado como parámetro la tabla FacetaRedireccion
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organizacion</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pFacetaDS">Dataset de facetas</param>
        public void CargarFacetaConfigProyChart(Guid pOrganizacionID, Guid pProyectoID, DataWrapperFacetas pFacetaDW)
        {
            pFacetaDW.ListaFacetaConfigProyChart = mEntityContext.FacetaConfigProyChart.Where(item => item.OrganizacionID.Equals(pOrganizacionID) && item.ProyectoID.Equals(pProyectoID)).ToList();
        }

        /// <summary>
        /// Devuelve el elemento de base de datos FacetaConfigProyChart a partir del identificador del chart
        /// </summary>
        /// <param name="pChartID">Identificador del chart</param>
        /// <returns>FacetaConfigProyChart buscado</returns>
        public FacetaConfigProyChart ObtenerFacetaConfigProyChartPorID(Guid pChartID)
        {
            return mEntityContext.FacetaConfigProyChart.Where(item => item.ChartID.Equals(pChartID)).FirstOrDefault();
        }

        /// <summary>
        /// Devuelve una lista de FacetaConfigProyChart de la base de datos a partir de una lista de identificadores
        /// </summary>
        /// <param name="pChartsIds">Lista con identificadores de charts</param>
        /// <returns>Lista de FacetaConfigProyChart buscados</returns>
        public List<FacetaConfigProyChart> ObtenerListaFacetaConfigProyChartPorIDs(List<Guid> pChartsIds)
        {
            return mEntityContext.FacetaConfigProyChart.Where(item => pChartsIds.Contains(item.ChartID)).ToList();
        }

        /// <summary>
        /// Carga en el dataset pasado como parámetro la tabla FacetaConfigProyRanfoFecha
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organizacion</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pFacetaDS">Dataset de facetas</param>
        public void CargarFacetaConfigProyRanfoFecha(Guid pOrganizacionID, Guid pProyectoID, DataWrapperFacetas pFacetaDW)
        {
            pFacetaDW.ListaFacetaConfigProyRangoFecha = mEntityContext.FacetaConfigProyRangoFecha.Where(item => item.OrganizacionID.Equals(pOrganizacionID) && item.ProyectoID.Equals(pProyectoID)).ToList();
        }

        /// <summary>
        /// Carga en el dataset pasado como parámetro la tabla ConfiguracionConexionGrafo
        /// </summary>
        /// <param name="pFacetaDS">Dataset de facetas</param>
        public void CargarConfiguracionConexionGrafo(DataWrapperFacetas pFacetaDW)
        {
            pFacetaDW.ListaConfiguracionConexionGrafo = mEntityContext.ConfiguracionConexionGrafo.ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pOrganizacionID"></param>
        /// <param name="pProyectoID"></param>
        /// <returns></returns>
        public DataWrapperFacetas ObtenerFacetaObjetoConocimientoProyecto(Guid pOrganizacionID, Guid pProyectoID, bool pObtenerOcultas)
        {
            DataWrapperFacetas dataWrapperFacetas = new DataWrapperFacetas();

            var query = mEntityContext.FacetaObjetoConocimientoProyecto.Where(item => item.OrganizacionID.Equals(pOrganizacionID) && item.ProyectoID.Equals(pProyectoID));

            if (!pObtenerOcultas)
            {
                query = query.Where(item => item.OcultaEnFacetas.Equals(false) && item.OcultaEnFiltros.Equals(false));
            }
            dataWrapperFacetas.ListaFacetaObjetoConocimientoProyecto = query.ToList();

            dataWrapperFacetas.ListaFacetaEntidadesExternas = mEntityContext.FacetaEntidadesExternas.Where(item => item.OrganizacionID.Equals(pOrganizacionID) && item.ProyectoID.Equals(pProyectoID)).ToList();

            return dataWrapperFacetas;
        }

        public DataWrapperFacetas ObtenerFacetaObjetoConocimiento(string pObjetoConocimiento)
        {
            DataWrapperFacetas dataWrapperFacetas = new DataWrapperFacetas();

            dataWrapperFacetas.ListaFacetaObjetoConocimiento = mEntityContext.FacetaObjetoConocimiento.Where(item => item.ObjetoConocimiento.Equals(pObjetoConocimiento)).ToList();

            return dataWrapperFacetas;
        }

        public DataWrapperFacetas ObtenerFacetasDadoFacetaKey(string pFaceta, Guid pProyectoID)
        {
            DataWrapperFacetas dataWrapperFacetas = new DataWrapperFacetas();

            Guid facetaID = new Guid(pFaceta);

            dataWrapperFacetas.ListaFacetaObjetoConocimientoProyecto = mEntityContext.FacetaObjetoConocimientoProyecto.Where(item => item.AgrupacionID.Value.Equals(facetaID) && item.ProyectoID.Equals(pProyectoID)).ToList();

            return dataWrapperFacetas;
        }

        public DataWrapperFacetas ObtenerFacetasAdministrarProyecto(Guid pProyectoID)
        {
            DataWrapperFacetas dataWrapperFacetas = new DataWrapperFacetas();

            dataWrapperFacetas.ListaFacetaObjetoConocimiento = mEntityContext.FacetaObjetoConocimiento.Where(item => item.ObjetoConocimiento.Equals("recurso") || item.ObjetoConocimiento.Equals("debate") || item.ObjetoConocimiento.Equals("pregunta") || item.ObjetoConocimiento.Equals("encuensta") || item.ObjetoConocimiento.Equals("clase") || item.ObjetoConocimiento.Equals("Organizacion") || item.ObjetoConocimiento.Equals("Persona") || item.ObjetoConocimiento.Equals("grupo")).ToList();

            dataWrapperFacetas.ListaFacetaObjetoConocimientoProyecto = mEntityContext.FacetaObjetoConocimientoProyecto.Where(item => item.ProyectoID.Equals(pProyectoID)).ToList();

            dataWrapperFacetas.ListaFacetaFiltroProyecto = mEntityContext.FacetaFiltroProyecto.Where(item => item.ProyectoID.Equals(pProyectoID)).ToList();
            dataWrapperFacetas.ListaFacetaExcluida = mEntityContext.FacetaExcluida.Where(item => item.ProyectoID.Equals(pProyectoID)).ToList();
            return dataWrapperFacetas;
        }

        /// <summary>
        /// Devuelve la fila de la ontología en ese proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <param name="pEnlace">Enlace de la ontología</param>
        /// <returns>La fila de la ontología en el proyecto</returns>
        public OntologiaProyecto ObtenerOntologiaProyectoPorEnlace(Guid pProyectoID, string pEnlace)
        {
            return mEntityContext.OntologiaProyecto.FirstOrDefault(item => item.ProyectoID.Equals(pProyectoID) && item.OntologiaProyecto1.Equals(pEnlace));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pFacetaDS"></param>
        /// <param name="pOrganizacionID"></param>
        /// <param name="pProyectoID"></param>
        public List<OntologiaProyecto> ObtenerOntologiasProyecto(Guid pOrganizacionID, Guid pProyectoID)
        {
            return ObtenerOntologiasProyecto(pOrganizacionID, pProyectoID, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pFacetaDS"></param>
        /// <param name="pOrganizacionID"></param>
        /// <param name="pProyectoID"></param>
        public List<OntologiaProyecto> ObtenerOntologiasProyecto(Guid pOrganizacionID, Guid pProyectoID, bool pAgregarNamespacesComoOntologias)
        {
            return ObtenerOntologiasProyecto(pOrganizacionID, pProyectoID, pAgregarNamespacesComoOntologias, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pFacetaDS"></param>
        /// <param name="pOrganizacionID"></param>
        /// <param name="pProyectoID"></param>
        public List<OntologiaProyecto> ObtenerOntologiasProyecto(Guid pOrganizacionID, Guid pProyectoID, bool pAgregarNamespacesComoOntologias, bool pSoloBuscables)
        {
            var query = mEntityContext.OntologiaProyecto.Where(item => item.ProyectoID.Equals(pProyectoID));

            if (!pOrganizacionID.Equals(Guid.Empty))
            {
                query = query.Where(item => item.OrganizacionID.Equals(pOrganizacionID));
            }
            if (pSoloBuscables && !pOrganizacionID.Equals(Guid.Empty))
            {
                query = query.Where(item => item.EsBuscable.Equals(true));
            }

            List<OntologiaProyecto> listaOntologias = query.ToList();

            if (pAgregarNamespacesComoOntologias)
            {
                AgregarNamespacesComoOntologias(listaOntologias);
            }

            return listaOntologias;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pDataWrapperFacetas"></param>
        /// <param name="pOrganizacionID"></param>
        /// <param name="pProyectoID"></param>
        public void ObtenerOntologiasProyecto(DataWrapperFacetas pDataWrapperFacetas, Guid pOrganizacionID, Guid pProyectoID)
        {
            ObtenerOntologiasProyecto(pDataWrapperFacetas, pOrganizacionID, pProyectoID, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pDataWrapperFacetas"></param>
        /// <param name="pOrganizacionID"></param>
        /// <param name="pProyectoID"></param>
        public void ObtenerOntologiasProyecto(DataWrapperFacetas pDataWrapperFacetas, Guid pOrganizacionID, Guid pProyectoID, bool pAgregarNamespacesComoOntologias, bool pSoloBuscables)
        {
            var consulta = mEntityContext.OntologiaProyecto.Where(item => item.ProyectoID.Equals(pProyectoID));
            if (!pOrganizacionID.Equals(Guid.Empty))
            {
                consulta = consulta.Where(item => item.OrganizacionID.Equals(pOrganizacionID));

            }
            if (pSoloBuscables && !pOrganizacionID.Equals(Guid.Empty))
            {
                consulta = consulta.Where(item => item.EsBuscable);
            }

            List<OntologiaProyecto> listaOntologiaProyecto = consulta.ToList();
            if (pAgregarNamespacesComoOntologias)
            {
                AgregarNamespacesComoOntologias(listaOntologiaProyecto);
            }
            pDataWrapperFacetas.ListaOntologiaProyecto = pDataWrapperFacetas.ListaOntologiaProyecto.Union(listaOntologiaProyecto).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pDataWrapperFacetas"></param>
        /// <param name="pOrganizacionID"></param>
        /// <param name="pProyectoID"></param>
        public void ObtenerOntologiasProyecto(DataWrapperFacetas pDataWrapperFacetas, Guid pOrganizacionID, Guid pProyectoID, bool pAgregarNamespacesComoOntologias)
        {
            ObtenerOntologiasProyecto(pDataWrapperFacetas, pOrganizacionID, pProyectoID, pAgregarNamespacesComoOntologias, false);
        }
        
        /// <summary>
        /// Obtiene los namespaces de una ontología
        /// </summary>
        /// <param name="pOntologia">Ontología</param>
        /// <returns>DataSet con los namespaces de una ontología</returns>
        public List<OntologiaProyecto> ObtenerNamespacesDeOntologia(string pOntologia)
        {
            OntologiaProyecto ontologia = mEntityContext.OntologiaProyecto.Where(item => item.OntologiaProyecto1.Equals(pOntologia)).FirstOrDefault();
            List<OntologiaProyecto> lista = new List<OntologiaProyecto>();
            if (ontologia != null)
            {
                lista.Add(ontologia);
            }
            AgregarNamespacesComoOntologias(lista);

            return lista;
        }

        /// <summary>
        /// Comprueba si un usuario es administrador de alguna comunidad que tenga una ontologia concreta
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <param name="pOntologia">Ontología de la que el usuario debe ser administrador</param>
        /// <returns>Verdad si el usuario administra alguna comunidad que contenga esta ontología</returns>
        public bool ComprobarUsuarioAdministraOntologia(Guid pUsuarioID, string pOntologia)
        {
            return mEntityContext.AdministradorProyecto.Join(mEntityContext.OntologiaProyecto, admin => admin.ProyectoID, ontologia => ontologia.ProyectoID, (admin, ontologia) =>
            new
            {
                AdministradorProyecto = admin,
                OntologiaProyecto = ontologia
            }).Any(item => item.AdministradorProyecto.Tipo.Equals(0) && item.OntologiaProyecto.OntologiaProyecto1.Equals(pOntologia) && item.AdministradorProyecto.UsuarioID.Equals(pUsuarioID));
        }

        /// <summary>
        /// Obtenemos el modelo de base de datos de las facetas para la faceta y proyecto pasado por parámetro
        /// </summary>
        /// <param name="pFaceta">Clave de faceta que queremos obtener</param>
        /// <param name="pProyectoID">Identificador del proytecto del cual queremos obtener las facetas</param>
        /// <returns>El modelo de base de datos de las facetas solicitadas por parámetro</returns>
        public List<FacetaObjetoConocimientoProyecto> ObtenerFacetaObjetoConocimientoPorFaceta(string pFaceta, Guid pProyectoID)
        {
            return mEntityContext.FacetaObjetoConocimientoProyecto.Where(item => item.Faceta.Equals(pFaceta) && item.ProyectoID.Equals(pProyectoID)).ToList();
        }

        private static void AgregarNamespacesComoOntologias(List<OntologiaProyecto> pListaFaceta)
        {
            List<OntologiaProyecto> filasExtra = new List<OntologiaProyecto>();

            foreach (OntologiaProyecto fila in pListaFaceta)
            {
                if (fila.NamespacesExtra != null && !string.IsNullOrEmpty(fila.NamespacesExtra))
                {
                    char[] separadores = { '|' };
                    string[] namespaces = (fila.NamespacesExtra).Split(separadores, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string names in namespaces)
                    {
                        string key = names.Substring(0, names.IndexOf(":"));
                        string value = names.Substring(names.IndexOf(":") + 1);
                        string ontologiaProyecto = "@" + value;

                        OntologiaProyecto filaNueva = new OntologiaProyecto();
                        filaNueva.OrganizacionID = fila.OrganizacionID;
                        filaNueva.ProyectoID = fila.ProyectoID;
                        filaNueva.OntologiaProyecto1 = ontologiaProyecto;
                        filaNueva.NombreOnt = fila.NombreOnt;
                        filaNueva.Namespace = key;
                        filaNueva.NamespacesExtra = "";
                        filaNueva.EsBuscable = true;
                        filaNueva.CachearDatosSemanticos = true;

                        if (pListaFaceta.Any(ontologia => ontologia.OrganizacionID.Equals(fila.OrganizacionID) && ontologia.ProyectoID.Equals(fila.ProyectoID) && ontologia.OntologiaProyecto1.Equals(ontologiaProyecto)) || filasExtra.Any(filaExtra => (filaExtra.OntologiaProyecto1.Equals(ontologiaProyecto) || filaExtra.OntologiaProyecto1.StartsWith(ontologiaProyecto + "##REP")) && !filaExtra.Namespace.Equals(key)))
                        {
                            // Si hay dos ontologías con distintos namespaces, añado los dos namespaces para que no falle ninguna consulta. 
                            // Para que no falle al añadir al dataset, le añado algo más al nombre de la ontología
                            string nombreNuevo = ontologiaProyecto + "##REP" + Guid.NewGuid();
                            if (nombreNuevo.Length > 100)
                            {
                                nombreNuevo = nombreNuevo.Substring(0, 100);
                            }
                            filaNueva.OntologiaProyecto1 = nombreNuevo;
                        }

                        if (!pListaFaceta.Any(ontologia => ontologia.OrganizacionID.Equals(fila.OrganizacionID) && ontologia.ProyectoID.Equals(fila.ProyectoID) && ontologia.OntologiaProyecto1.Equals(ontologiaProyecto)))
                        {
                            filasExtra.Add(filaNueva);
                        }
                    }
                }
            }
            foreach (OntologiaProyecto filaExtra in filasExtra)
            {
                if (!pListaFaceta.Any(ontologia => ontologia.OrganizacionID.Equals(filaExtra.OrganizacionID) && ontologia.ProyectoID.Equals(filaExtra.ProyectoID) && ontologia.OntologiaProyecto1.Equals(filaExtra.OntologiaProyecto1)))
                {
                    pListaFaceta.Add(filaExtra);
                }
            }
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
            DataWrapperFacetas dataWrapperFaceta = new DataWrapperFacetas();

            if (pTipoBusqueda != TipoBusqueda.PersonasYOrganizaciones)
            {
                dataWrapperFaceta.ListaFacetaConfigProyMapa = mEntityContext.FacetaConfigProyMapa.Where(item => item.OrganizacionID.Equals(pOrganizacionID) && item.ProyectoID.Equals(pProyectoID)).ToList();
            }
            else
            {
                dataWrapperFaceta.ListaPropsMapaPerYOrg = mEntityContext.ParametroGeneral.Where(item => item.OrganizacionID.Equals(pOrganizacionID) && item.ProyectoID.Equals(pProyectoID)).Select(item => item.PropsMapaPerYOrg).ToList();
            }

            return dataWrapperFaceta;
        }

        /// <summary>
        /// Obtiene la lista de facetas para una ontología
        /// </summary>
        /// <param name="pNombreOntologia">Nombre de la ontología de la cual queremos las facetas</param>
        /// <param name="pProyectoID">El proyecto id del cual queremos la faceta</param>
        /// <returns></returns>
        public List<FacetaObjetoConocimientoProyecto> ObtenerFacetasObjetoConocimientoProyectoDeOntologia(string pNombreOntologia, Guid pProyectoID)
        {
            return mEntityContext.FacetaObjetoConocimientoProyecto.Where(item => item.ObjetoConocimiento.Equals(pNombreOntologia) && item.ProyectoID.Equals(pProyectoID)).ToList();
        }

        /// <summary>
        /// Obtiene los datos para una consulto usando charts.
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <param name="pChartID">ID de chart o Guid.Empty si no se quiere especificar uno</param>
        /// <returns>datos para una consulto usando charts</returns>
        public DataWrapperFacetas ObtenerDatosChartProyecto(Guid pOrganizacionID, Guid pProyectoID, Guid pChartID)
        {
            var query = mEntityContext.FacetaConfigProyChart.Where(item => item.OrganizacionID.Equals(pOrganizacionID) && item.ProyectoID.Equals(pProyectoID));

            if (pChartID != Guid.Empty)
            {
                query = query.Where(item => item.ChartID.Equals(pChartID));
            }

            DataWrapperFacetas dataWrapperFacetas = new DataWrapperFacetas();
            dataWrapperFacetas.ListaFacetaConfigProyChart = query.ToList();

            return dataWrapperFacetas;
        }

        /// <summary>
        /// Obtiene la configuración de conexiónes de todos los grafos
        /// </summary>
        /// <returns></returns>
        public DataWrapperFacetas ObtenerConfiguracionGrafoConexion()
        {
            DataWrapperFacetas dataWrapperFaceta = new DataWrapperFacetas();

            dataWrapperFaceta.ListaConfiguracionConexionGrafo = mEntityContext.ConfiguracionConexionGrafo.ToList();

            return dataWrapperFaceta;
        }

        /// <summary>
        /// Obtiene el número de filtros de categoría que tiene configurados un proyecto
        /// </summary>
        /// <param name="pProyectoID">Proyecto en el que se hace la búsqueda</param>
        public int ObtenerNumeroFiltrosCategoriasDeProyecto(Guid pOrganizacionID, Guid pProyectoID)
        {
            return mEntityContext.FacetaFiltroProyecto.Where(item => item.OrganizacionID.Equals(pOrganizacionID) && item.ProyectoID.Equals(pProyectoID) && item.Filtro != null).Count();
        }

        public string ObtenerNombreFacetaRedireccion(string pNombre)
        {

            return mEntityContext.FacetaRedireccion.Where(item => item.Nombre.Equals(pNombre)).Select(item => item.Faceta).FirstOrDefault();
        }

        public List<string> ObtenerPredicadosSemanticos(Guid pOrganizacionID, Guid pProyectoID)
        {
            return mEntityContext.FacetaObjetoConocimientoProyecto.Where(item => item.Autocompletar.Equals(true) && item.OrganizacionID.Equals(pOrganizacionID) && item.ProyectoID.Equals(pProyectoID)).Select(item => item.Faceta).Distinct().ToList();
        }

        /// <summary>
        /// Devuelve un valor de una ontología con el namespace correcto que le corresponda según ésta.
        /// </summary>
        /// <param name="pValor">Valor</param>
        /// <param name="pFacetaDS">DataSet con la configuración</param>
        /// <returns>Valor de una ontología con el namespace correcto que le corresponda según ésta</returns>
        public static string ObtenerValorAplicandoNamespaces(string pValor, List<OntologiaProyecto> pListaFaceta, bool pUsarNamespaceSiempre)
        {
            foreach (OntologiaProyecto filaOnto in pListaFaceta.Where(item => !string.IsNullOrEmpty(item.SubTipos)))
            {
                string valorNamespaceado = ObtenerValorAplicandoNamespaces(pValor, filaOnto, pUsarNamespaceSiempre);
                if (!string.IsNullOrEmpty(filaOnto.SubTipos) && filaOnto.SubTipos.Contains(valorNamespaceado + "|||"))
                {
                    pValor = valorNamespaceado;
                    break;
                }
            }

            return pValor;
        }

        /// <summary>
        /// Devuelve un valor de una ontología con el namespace correcto que le corresponda según ésta.
        /// </summary>
        /// <param name="pValor">Valor</param>
        /// <param name="pFilaOntoNamespaces">Fila de ontología con sus namespaces</param>
        /// <param name="pUsarNamespaceSiempre">Indica si se debe usar namespace siempre</param>
        /// <returns>Valor de una ontología con el namespace correcto que le corresponda según ésta</returns>
        public static string ObtenerValorAplicandoNamespaces(string pValor, OntologiaProyecto pFilaOntoNamespaces, bool pUsarNamespaceSiempre)
        {
            if (pValor.StartsWith("http://") || pValor.StartsWith("https://"))
            {
                foreach (string namespaceUrl in pFilaOntoNamespaces.NamespacesExtra.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    string names = namespaceUrl.Substring(0, namespaceUrl.IndexOf(":"));
                    string url = namespaceUrl.Substring(namespaceUrl.IndexOf(":") + 1);
                    if (pValor.Contains(url))
                    {
                        return $"{names}:{pValor.Replace(url, "")}";
                    }
                }
            }
            else if (pValor.Contains(":"))
            {
                return pValor;
            }
            else if (pUsarNamespaceSiempre)
            {
                return $"{pFilaOntoNamespaces.OntologiaProyecto1}:{pValor}";
            }

            return pValor;
        }

        /// <summary>
        /// Devuelve el texto configurado de un subTipo según el idioma.
        /// </summary>
        /// <param name="pSubTipo">SubTipo</param>
        /// <param name="pFacetaDS">DataSet con la configuración</param>
        /// <returns>Texto configurado de un subTipo según el idioma</returns>
        public static string ObtenerTextoSubTipoDeIdioma(string pSubTipo, List<OntologiaProyecto> pListaFaceta, string pIdioma)
        {
            string nombre = pSubTipo;
            foreach (OntologiaProyecto filaOnto in pListaFaceta.Where(item => !string.IsNullOrEmpty(item.SubTipos)))
            {
                string valorNamespaceado = ObtenerValorAplicandoNamespaces(nombre, filaOnto, true);
                if (!string.IsNullOrEmpty(filaOnto.SubTipos) && filaOnto.SubTipos.Contains(valorNamespaceado + "|||"))
                {
                    string subTipo = filaOnto.SubTipos.Substring(filaOnto.SubTipos.IndexOf(valorNamespaceado + "|||") + valorNamespaceado.Length + 3);
                    subTipo = subTipo.Substring(0, subTipo.IndexOf("[|||]")) + "|||";

                    if (subTipo.Contains("@" + pIdioma + "|||"))
                    {
                        subTipo = subTipo.Substring(0, subTipo.IndexOf("@" + pIdioma + "|||"));

                        if (subTipo.Contains("|||"))
                        {
                            subTipo = subTipo.Substring(subTipo.LastIndexOf("|||") + 3);
                        }
                    }
                    else
                    {
                        if (subTipo.Contains("@"))
                        {
                            subTipo = subTipo.Substring(0, subTipo.LastIndexOf("@"));
                        }
                        else
                        {
                            subTipo = subTipo.Substring(0, subTipo.LastIndexOf("|||"));
                        }
                    }

                    nombre = subTipo;
                    break;
                }
            }

            return nombre;
        }

        public static bool PriorizarFacetasEnOrden(DataWrapperFacetas pFacetaDW, Dictionary<string, List<string>> pListaFiltros)
        {
            return pFacetaDW.ListaFacetaObjetoConocimientoProyecto.Where(f => f.PriorizarOrdenResultados).Any(item => pListaFiltros.ContainsKey(item.Faceta));
        }

        /// <summary>
        /// Elimina las facetas pasadas por parámetro de su tabla y de las tablas relacionadas
        /// </summary>
        /// <param name="pFacetas"></param>
        public void EliminarFacetas(List<FacetaObjetoConocimientoProyecto> pFacetas)
        {
            foreach (FacetaObjetoConocimientoProyecto faceta in pFacetas)
            {
                mEntityContext.FacetaObjetoConocimientoProyectoPestanya.RemoveRange(mEntityContext.FacetaObjetoConocimientoProyectoPestanya.Where(item => item.ObjetoConocimiento == faceta.ObjetoConocimiento && item.Faceta == faceta.Faceta && item.ProyectoID.Equals(faceta.ProyectoID)));

                mEntityContext.FacetaFiltroProyecto.RemoveRange(mEntityContext.FacetaFiltroProyecto.Where(item => item.ObjetoConocimiento == faceta.ObjetoConocimiento && item.Faceta == faceta.Faceta && item.ProyectoID.Equals(faceta.ProyectoID)));

                mEntityContext.FacetaObjetoConocimientoProyecto.Remove(faceta);
            }
        }

        /// <summary>
        /// Guarda los cambios pendientes de confirmar en la base de datos
        /// </summary>
        public void GuardarCambios()
        {
            mEntityContext.SaveChanges();
        }

        #endregion Métodos generales

    }
}