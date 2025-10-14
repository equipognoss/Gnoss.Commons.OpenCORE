using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.Amigos;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Faceta;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
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
            this.CargarConsultasYDataAdapters();
        }

        /// <summary>
        /// Constructor para TablasDeConfiguracionAD
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Fichero de configuración</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public FacetaAD(string pFicheroConfiguracionBD, LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<FacetaAD> logger, ILoggerFactory loggerFactory)
            : base(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication,logger,loggerFactory)
        {
            mEntityContext = entityContext;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            this.CargarConsultasYDataAdapters(IBD);
        }

        #endregion Constructores

        #region Consultas

        private string sqlSelectFacetaHome;
        private string sqlSelectFacetaFiltroHome;
        private string sqlSelectFacetaExcluida;
        private string sqlSelectOntologiaProyecto;
        private string sqlSelectFacetaObjetoConocimiento;
        private string sqlSelectFacetaObjetoConocimientoProyecto;
        private string sqlSelectFacetaObjetoConocimientoProyectoPestanya;
        private string sqlSelectFacetaFiltroProyecto;
        private string sqlSelectFacetaMultiple;
        private string sqlSelectFacetaEntidadesExternas;
        private string sqlSelectFacetaConfigProyChart;
        private string sqlSelectFacetaConfigProyMapa;
        private string sqlSelectFacetaRedireccion;
        private string sqlSelectConfiguracionConexionGrafo;
        private string sqlSelectFacetaConfigProyRangoFecha;

        #endregion

        #region DataAdapter

        #region FacetaHome
        private string sqlFacetaHomeInsert;
        private string sqlFacetaHomeDelete;
        private string sqlFacetaHomeModify;
        #endregion

        #region FacetaFiltroHome
        private string sqlFacetaFiltroHomeInsert;
        private string sqlFacetaFiltroHomeDelete;
        private string sqlFacetaFiltroHomeModify;
        #endregion

        #region FacetaExcluida
        private string sqlFacetaExcluidaInsert;
        private string sqlFacetaExcluidaDelete;
        private string sqlFacetaExcluidaModify;
        #endregion

        #region OntologiaProyecto
        private string sqlOntologiaProyectoInsert;
        private string sqlOntologiaProyectoDelete;
        private string sqlOntologiaProyectoModify;
        #endregion

        #region FacetaObjetoConocimiento
        private string sqlFacetaObjetoConocimientoInsert;
        private string sqlFacetaObjetoConocimientoDelete;
        private string sqlFacetaObjetoConocimientoModify;
        #endregion

        #region FacetaObjetoConocimientoProyecto
        private string sqlFacetaObjetoConocimientoProyectoInsert;
        private string sqlFacetaObjetoConocimientoProyectoDelete;
        private string sqlFacetaObjetoConocimientoProyectoModify;
        #endregion

        #region FacetaObjetoConocimientoProyectoPestanya
        private string sqlFacetaObjetoConocimientoProyectoPestanyaInsert;
        private string sqlFacetaObjetoConocimientoProyectoPestanyaDelete;
        private string sqlFacetaObjetoConocimientoProyectoPestanyaModify;
        #endregion


        #region FacetaFiltroProyecto
        private string sqlFacetaFiltroProyectoInsert;
        private string sqlFacetaFiltroProyectoDelete;
        private string sqlFacetaFiltroProyectoModify;
        #endregion

        #region FacetaEntidadesExternas
        private string sqlFacetaEntidadesExternasInsert;
        private string sqlFacetaEntidadesExternasDelete;
        private string sqlFacetaEntidadesExternasModify;
        #endregion

        #region FacetaConfigProyChart
        private string sqlFacetaConfigProyChartInsert;
        private string sqlFacetaConfigProyChartDelete;
        private string sqlFacetaConfigProyChartModify;
        #endregion

        #region FacetaConfigProyMapa
        private string sqlFacetaConfigProyMapaInsert;
        private string sqlFacetaConfigProyMapaDelete;
        private string sqlFacetaConfigProyMapaModify;
        #endregion

        #region FacetaRedireccion
        private string sqlFacetaRedireccionInsert;
        private string sqlFacetaRedireccionDelete;
        private string sqlFacetaRedireccionModify;
        #endregion

        #region ConfiguracionConexionGrafo
        private string sqlConfiguracionConexionGrafoInsert;
        private string sqlConfiguracionConexionGrafoDelete;
        private string sqlConfiguracionConexionGrafoModify;
        #endregion

        #endregion

        #region Métodos AD

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pDataSet"></param>
        public void ActualizarBD(DataSet pDataSet)
        {
            mEntityContext.SaveChanges();
            EliminarBorrados(pDataSet);
            GuardarActualizaciones(pDataSet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pDataSet"></param>
        public void EliminarBorrados(DataSet pDataSet)
        {
            try
            {
                DataSet deletedDataSet;
                deletedDataSet = pDataSet.GetChanges(DataRowState.Deleted);
                if (deletedDataSet != null)
                {
                    #region Deleted
                    #region Eliminar tabla ConfiguracionConexionGrafo
                    DbCommand DeleteConfiguracionConexionGrafoCommand = ObtenerComando(sqlConfiguracionConexionGrafoDelete);
                    AgregarParametro(DeleteConfiguracionConexionGrafoCommand, IBD.ToParam("Original_Grafo"), DbType.String, "Grafo", DataRowVersion.Original);
                    AgregarParametro(DeleteConfiguracionConexionGrafoCommand, IBD.ToParam("Original_CadenaConexion"), DbType.String, "CadenaConexion", DataRowVersion.Original);
                    ActualizarBaseDeDatos(deletedDataSet, "ConfiguracionConexionGrafo", null, null, DeleteConfiguracionConexionGrafoCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #region Eliminar tabla FacetaRedireccion
                    DbCommand DeleteFacetaRedireccionCommand = ObtenerComando(sqlFacetaRedireccionDelete);
                    AgregarParametro(DeleteFacetaRedireccionCommand, IBD.ToParam("Original_Faceta"), DbType.String, "Faceta", DataRowVersion.Original);
                    AgregarParametro(DeleteFacetaRedireccionCommand, IBD.ToParam("Original_Nombre"), DbType.String, "Nombre", DataRowVersion.Original);
                    ActualizarBaseDeDatos(deletedDataSet, "FacetaRedireccion", null, null, DeleteFacetaRedireccionCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #region Eliminar tabla FacetaConfigProyMapa
                    DbCommand DeleteFacetaConfigProyMapaCommand = ObtenerComando(sqlFacetaConfigProyMapaDelete);
                    AgregarParametro(DeleteFacetaConfigProyMapaCommand, IBD.ToParam("Original_OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Original);
                    AgregarParametro(DeleteFacetaConfigProyMapaCommand, IBD.ToParam("Original_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);
                    AgregarParametro(DeleteFacetaConfigProyMapaCommand, IBD.ToParam("Original_PropLatitud"), DbType.String, "PropLatitud", DataRowVersion.Original);
                    AgregarParametro(DeleteFacetaConfigProyMapaCommand, IBD.ToParam("Original_PropLongitud"), DbType.String, "PropLongitud", DataRowVersion.Original);
                    ActualizarBaseDeDatos(deletedDataSet, "FacetaConfigProyMapa", null, null, DeleteFacetaConfigProyMapaCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #region Eliminar tabla FacetaConfigProyChart
                    DbCommand DeleteFacetaConfigProyChartCommand = ObtenerComando(sqlFacetaConfigProyChartDelete);
                    AgregarParametro(DeleteFacetaConfigProyChartCommand, IBD.ToParam("Original_OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Original);
                    AgregarParametro(DeleteFacetaConfigProyChartCommand, IBD.ToParam("Original_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);
                    AgregarParametro(DeleteFacetaConfigProyChartCommand, IBD.ToParam("Original_ChartID"), IBD.TipoGuidToObject(DbType.Guid), "ChartID", DataRowVersion.Original);

                    ActualizarBaseDeDatos(deletedDataSet, "FacetaConfigProyChart", null, null, DeleteFacetaConfigProyChartCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #region Eliminar tabla FacetaEntidadesExternas
                    DbCommand DeleteFacetaEntidadesExternasCommand = ObtenerComando(sqlFacetaEntidadesExternasDelete);
                    AgregarParametro(DeleteFacetaEntidadesExternasCommand, IBD.ToParam("Original_OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Original);
                    AgregarParametro(DeleteFacetaEntidadesExternasCommand, IBD.ToParam("Original_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);
                    AgregarParametro(DeleteFacetaEntidadesExternasCommand, IBD.ToParam("Original_EntidadID"), DbType.String, "EntidadID", DataRowVersion.Original);
                    AgregarParametro(DeleteFacetaEntidadesExternasCommand, IBD.ToParam("Original_Grafo"), DbType.String, "Grafo", DataRowVersion.Original);
                    AgregarParametro(DeleteFacetaEntidadesExternasCommand, IBD.ToParam("Original_EsEntidadSecundaria"), DbType.Boolean, "EsEntidadSecundaria", DataRowVersion.Original);
                    AgregarParametro(DeleteFacetaEntidadesExternasCommand, IBD.ToParam("Original_BuscarConRecursividad"), DbType.Boolean, "BuscarConRecursividad", DataRowVersion.Original);
                    ActualizarBaseDeDatos(deletedDataSet, "FacetaEntidadesExternas", null, null, DeleteFacetaEntidadesExternasCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #region Eliminar tabla FacetaFiltroProyecto
                    DbCommand DeleteFacetaFiltroProyectoCommand = ObtenerComando(sqlFacetaFiltroProyectoDelete);
                    AgregarParametro(DeleteFacetaFiltroProyectoCommand, IBD.ToParam("Original_OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Original);
                    AgregarParametro(DeleteFacetaFiltroProyectoCommand, IBD.ToParam("Original_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);
                    AgregarParametro(DeleteFacetaFiltroProyectoCommand, IBD.ToParam("Original_ObjetoConocimiento"), DbType.String, "ObjetoConocimiento", DataRowVersion.Original);
                    AgregarParametro(DeleteFacetaFiltroProyectoCommand, IBD.ToParam("Original_Faceta"), DbType.String, "Faceta", DataRowVersion.Original);
                    AgregarParametro(DeleteFacetaFiltroProyectoCommand, IBD.ToParam("Original_Filtro"), DbType.String, "Filtro", DataRowVersion.Original);
                    AgregarParametro(DeleteFacetaFiltroProyectoCommand, IBD.ToParam("Original_Orden"), DbType.Int16, "Orden", DataRowVersion.Original);
                    ActualizarBaseDeDatos(deletedDataSet, "FacetaFiltroProyecto", null, null, DeleteFacetaFiltroProyectoCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #region Eliminar tabla FacetaFiltroHome
                    DbCommand DeleteFacetaFiltroHomeCommand = ObtenerComando(sqlFacetaFiltroHomeDelete);
                    AgregarParametro(DeleteFacetaFiltroHomeCommand, IBD.ToParam("Original_OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Original);
                    AgregarParametro(DeleteFacetaFiltroHomeCommand, IBD.ToParam("Original_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);
                    AgregarParametro(DeleteFacetaFiltroHomeCommand, IBD.ToParam("Original_ObjetoConocimiento"), DbType.String, "ObjetoConocimiento", DataRowVersion.Original);
                    AgregarParametro(DeleteFacetaFiltroHomeCommand, IBD.ToParam("Original_Faceta"), DbType.String, "Faceta", DataRowVersion.Original);
                    AgregarParametro(DeleteFacetaFiltroHomeCommand, IBD.ToParam("Original_Filtro"), DbType.String, "Filtro", DataRowVersion.Original);
                    AgregarParametro(DeleteFacetaFiltroHomeCommand, IBD.ToParam("Original_Orden"), DbType.Int16, "Orden", DataRowVersion.Original);
                    ActualizarBaseDeDatos(deletedDataSet, "FacetaFiltroHome", null, null, DeleteFacetaFiltroHomeCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #region Eliminar tabla FacetaHome
                    DbCommand DeleteFacetaHomeCommand = ObtenerComando(sqlFacetaHomeDelete);
                    AgregarParametro(DeleteFacetaHomeCommand, IBD.ToParam("Original_OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Original);
                    AgregarParametro(DeleteFacetaHomeCommand, IBD.ToParam("Original_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);
                    AgregarParametro(DeleteFacetaHomeCommand, IBD.ToParam("Original_ObjetoConocimiento"), DbType.String, "ObjetoConocimiento", DataRowVersion.Original);
                    AgregarParametro(DeleteFacetaHomeCommand, IBD.ToParam("Original_Faceta"), DbType.String, "Faceta", DataRowVersion.Original);
                    AgregarParametro(DeleteFacetaHomeCommand, IBD.ToParam("Original_PestanyaFaceta"), DbType.String, "PestanyaFaceta", DataRowVersion.Original);
                    AgregarParametro(DeleteFacetaHomeCommand, IBD.ToParam("Original_Orden"), DbType.Int16, "Orden", DataRowVersion.Original);
                    AgregarParametro(DeleteFacetaHomeCommand, IBD.ToParam("Original_MostrarVerMas"), DbType.Boolean, "MostrarVerMas", DataRowVersion.Original);
                    ActualizarBaseDeDatos(deletedDataSet, "FacetaHome", null, null, DeleteFacetaHomeCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #region Eliminar tabla FacetaObjetoConocimientoProyecto
                    DbCommand DeleteFacetaObjetoConocimientoProyectoCommand = ObtenerComando(sqlFacetaObjetoConocimientoProyectoDelete);
                    AgregarParametro(DeleteFacetaObjetoConocimientoProyectoCommand, IBD.ToParam("Original_OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Original);
                    AgregarParametro(DeleteFacetaObjetoConocimientoProyectoCommand, IBD.ToParam("Original_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);
                    AgregarParametro(DeleteFacetaObjetoConocimientoProyectoCommand, IBD.ToParam("Original_ObjetoConocimiento"), DbType.String, "ObjetoConocimiento", DataRowVersion.Original);
                    AgregarParametro(DeleteFacetaObjetoConocimientoProyectoCommand, IBD.ToParam("Original_Faceta"), DbType.String, "Faceta", DataRowVersion.Original);

                    ActualizarBaseDeDatos(deletedDataSet, "FacetaObjetoConocimientoProyecto", null, null, DeleteFacetaObjetoConocimientoProyectoCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #region Eliminar tabla FacetaObjetoConocimientoProyectoPestanya
                    DbCommand DeleteFacetaObjetoConocimientoProyectoPestanyaCommand = ObtenerComando(sqlFacetaObjetoConocimientoProyectoPestanyaDelete);
                    AgregarParametro(DeleteFacetaObjetoConocimientoProyectoPestanyaCommand, IBD.ToParam("Original_OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Original);
                    AgregarParametro(DeleteFacetaObjetoConocimientoProyectoPestanyaCommand, IBD.ToParam("Original_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);
                    AgregarParametro(DeleteFacetaObjetoConocimientoProyectoPestanyaCommand, IBD.ToParam("Original_ObjetoConocimiento"), DbType.String, "ObjetoConocimiento", DataRowVersion.Original);
                    AgregarParametro(DeleteFacetaObjetoConocimientoProyectoPestanyaCommand, IBD.ToParam("Original_Faceta"), DbType.String, "Faceta", DataRowVersion.Original);
                    AgregarParametro(DeleteFacetaObjetoConocimientoProyectoPestanyaCommand, IBD.ToParam("Original_PestanyaID"), IBD.TipoGuidToObject(DbType.Guid), "PestanyaID", DataRowVersion.Original);

                    ActualizarBaseDeDatos(deletedDataSet, "FacetaObjetoConocimientoProyectoPestanya", null, null, DeleteFacetaObjetoConocimientoProyectoPestanyaCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #region Eliminar tabla FacetaObjetoConocimiento
                    DbCommand DeleteFacetaObjetoConocimientoCommand = ObtenerComando(sqlFacetaObjetoConocimientoDelete);
                    AgregarParametro(DeleteFacetaObjetoConocimientoCommand, IBD.ToParam("Original_ObjetoConocimiento"), DbType.String, "ObjetoConocimiento", DataRowVersion.Original);
                    AgregarParametro(DeleteFacetaObjetoConocimientoCommand, IBD.ToParam("Original_Faceta"), DbType.String, "Faceta", DataRowVersion.Original);
                    AgregarParametro(DeleteFacetaObjetoConocimientoCommand, IBD.ToParam("Original_NombreFaceta"), DbType.String, "NombreFaceta", DataRowVersion.Original);
                    AgregarParametro(DeleteFacetaObjetoConocimientoCommand, IBD.ToParam("Original_Orden"), DbType.Int16, "Orden", DataRowVersion.Original);
                    AgregarParametro(DeleteFacetaObjetoConocimientoCommand, IBD.ToParam("Original_Autocompletar"), DbType.Boolean, "Autocompletar", DataRowVersion.Original);
                    AgregarParametro(DeleteFacetaObjetoConocimientoCommand, IBD.ToParam("Original_TipoPropiedad"), DbType.Int16, "TipoPropiedad", DataRowVersion.Original);
                    AgregarParametro(DeleteFacetaObjetoConocimientoCommand, IBD.ToParam("Original_TipoDisenio"), DbType.Int16, "TipoDisenio", DataRowVersion.Original);
                    AgregarParametro(DeleteFacetaObjetoConocimientoCommand, IBD.ToParam("Original_ElementosVisibles"), DbType.Int16, "ElementosVisibles", DataRowVersion.Original);
                    AgregarParametro(DeleteFacetaObjetoConocimientoCommand, IBD.ToParam("Original_AlgoritmoTransformacion"), DbType.Int16, "AlgoritmoTransformacion", DataRowVersion.Original);
                    AgregarParametro(DeleteFacetaObjetoConocimientoCommand, IBD.ToParam("Original_EsSemantica"), DbType.Boolean, "EsSemantica", DataRowVersion.Original);
                    AgregarParametro(DeleteFacetaObjetoConocimientoCommand, IBD.ToParam("Original_Mayusculas"), DbType.Int16, "Mayusculas", DataRowVersion.Original);
                    AgregarParametro(DeleteFacetaObjetoConocimientoCommand, IBD.ToParam("Original_EsPorDefecto"), DbType.Boolean, "Mayusculas", DataRowVersion.Original);
                    AgregarParametro(DeleteFacetaObjetoConocimientoCommand, IBD.ToParam("Original_ComportamientoOr"), DbType.Boolean, "ComportamientoOr", DataRowVersion.Original);

                    ActualizarBaseDeDatos(deletedDataSet, "FacetaObjetoConocimiento", null, null, DeleteFacetaObjetoConocimientoCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #region Eliminar tabla OntologiaProyecto
                    DbCommand DeleteOntologiaProyectoCommand = ObtenerComando(sqlOntologiaProyectoDelete);
                    AgregarParametro(DeleteOntologiaProyectoCommand, IBD.ToParam("Original_OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Original);
                    AgregarParametro(DeleteOntologiaProyectoCommand, IBD.ToParam("Original_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);
                    AgregarParametro(DeleteOntologiaProyectoCommand, IBD.ToParam("Original_OntologiaProyecto"), DbType.String, "OntologiaProyecto", DataRowVersion.Original);

                    ActualizarBaseDeDatos(deletedDataSet, "OntologiaProyecto", null, null, DeleteOntologiaProyectoCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #region Eliminar tabla FacetaExcluida
                    DbCommand DeleteFacetaExcluidaCommand = ObtenerComando(sqlFacetaExcluidaDelete);
                    AgregarParametro(DeleteFacetaExcluidaCommand, IBD.ToParam("Original_OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Original);
                    AgregarParametro(DeleteFacetaExcluidaCommand, IBD.ToParam("Original_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);
                    AgregarParametro(DeleteFacetaExcluidaCommand, IBD.ToParam("Original_Faceta"), DbType.String, "Faceta", DataRowVersion.Original);
                    ActualizarBaseDeDatos(deletedDataSet, "FacetaExcluida", null, null, DeleteFacetaExcluidaCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #endregion

                    deletedDataSet.Dispose();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Guarda en la tabla FacetaEntidadesExterna los datos al subir la ontología
        /// </summary>
        public void GuardarFacetasEntidadesExternas(FacetaEntidadesExternas facetaEntidadExterna)
        {
            mEntityContext.FacetaEntidadesExternas.Add(facetaEntidadExterna);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pDataSet"></param>
        public void GuardarActualizaciones(DataSet pDataSet)
        {
            try
            {
                DataSet addedAndModifiedDataSet;
                addedAndModifiedDataSet = pDataSet.GetChanges(DataRowState.Added | DataRowState.Modified);
                if (addedAndModifiedDataSet != null)
                {
                    #region AddedAndModified

                    #region Actualizar tabla FacetaExcluida
                    DbCommand InsertFacetaExcluidaCommand = ObtenerComando(sqlFacetaExcluidaInsert);
                    AgregarParametro(InsertFacetaExcluidaCommand, IBD.ToParam("OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaExcluidaCommand, IBD.ToParam("ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaExcluidaCommand, IBD.ToParam("Faceta"), DbType.String, "Faceta", DataRowVersion.Current);

                    DbCommand ModifyFacetaExcluidaCommand = ObtenerComando(sqlFacetaExcluidaModify);
                    AgregarParametro(ModifyFacetaExcluidaCommand, IBD.ToParam("Original_OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Original);
                    AgregarParametro(ModifyFacetaExcluidaCommand, IBD.ToParam("Original_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);
                    AgregarParametro(ModifyFacetaExcluidaCommand, IBD.ToParam("Original_Faceta"), DbType.String, "Faceta", DataRowVersion.Original);

                    AgregarParametro(ModifyFacetaExcluidaCommand, IBD.ToParam("OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaExcluidaCommand, IBD.ToParam("ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaExcluidaCommand, IBD.ToParam("Faceta"), DbType.String, "Faceta", DataRowVersion.Current);
                    ActualizarBaseDeDatos(addedAndModifiedDataSet, "FacetaExcluida", InsertFacetaExcluidaCommand, ModifyFacetaExcluidaCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #region Actualizar tabla OntologiaProyecto
                    DbCommand InsertOntologiaProyectoCommand = ObtenerComando(sqlOntologiaProyectoInsert);
                    AgregarParametro(InsertOntologiaProyectoCommand, IBD.ToParam("OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Current);
                    AgregarParametro(InsertOntologiaProyectoCommand, IBD.ToParam("ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Current);
                    AgregarParametro(InsertOntologiaProyectoCommand, IBD.ToParam("OntologiaProyecto"), DbType.String, "OntologiaProyecto", DataRowVersion.Current);
                    AgregarParametro(InsertOntologiaProyectoCommand, IBD.ToParam("NombreOnt"), DbType.String, "NombreOnt", DataRowVersion.Current);
                    AgregarParametro(InsertOntologiaProyectoCommand, IBD.ToParam("Namespace"), DbType.String, "Namespace", DataRowVersion.Current);
                    AgregarParametro(InsertOntologiaProyectoCommand, IBD.ToParam("NamespacesExtra"), DbType.String, "NamespacesExtra", DataRowVersion.Current);
                    AgregarParametro(InsertOntologiaProyectoCommand, IBD.ToParam("SubTipos"), DbType.String, "SubTipos", DataRowVersion.Current);
                    AgregarParametro(InsertOntologiaProyectoCommand, IBD.ToParam("NombreCortoOnt"), DbType.String, "NombreCortoOnt", DataRowVersion.Current);
                    AgregarParametro(InsertOntologiaProyectoCommand, IBD.ToParam("CachearDatosSemanticos"), DbType.Boolean, "CachearDatosSemanticos", DataRowVersion.Current);
                    AgregarParametro(InsertOntologiaProyectoCommand, IBD.ToParam("EsBuscable"), DbType.Boolean, "EsBuscable", DataRowVersion.Current);

                    DbCommand ModifyOntologiaProyectoCommand = ObtenerComando(sqlOntologiaProyectoModify);
                    AgregarParametro(ModifyOntologiaProyectoCommand, IBD.ToParam("Original_OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Original);
                    AgregarParametro(ModifyOntologiaProyectoCommand, IBD.ToParam("Original_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);
                    AgregarParametro(ModifyOntologiaProyectoCommand, IBD.ToParam("Original_OntologiaProyecto"), DbType.String, "OntologiaProyecto", DataRowVersion.Original);

                    AgregarParametro(ModifyOntologiaProyectoCommand, IBD.ToParam("OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Current);
                    AgregarParametro(ModifyOntologiaProyectoCommand, IBD.ToParam("ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Current);
                    AgregarParametro(ModifyOntologiaProyectoCommand, IBD.ToParam("OntologiaProyecto"), DbType.String, "OntologiaProyecto", DataRowVersion.Current);
                    AgregarParametro(ModifyOntologiaProyectoCommand, IBD.ToParam("NombreOnt"), DbType.String, "NombreOnt", DataRowVersion.Current);
                    AgregarParametro(ModifyOntologiaProyectoCommand, IBD.ToParam("Namespace"), DbType.String, "Namespace", DataRowVersion.Current);
                    AgregarParametro(ModifyOntologiaProyectoCommand, IBD.ToParam("NamespacesExtra"), DbType.String, "NamespacesExtra", DataRowVersion.Current);
                    AgregarParametro(ModifyOntologiaProyectoCommand, IBD.ToParam("SubTipos"), DbType.String, "SubTipos", DataRowVersion.Current);
                    AgregarParametro(ModifyOntologiaProyectoCommand, IBD.ToParam("NombreCortoOnt"), DbType.String, "NombreCortoOnt", DataRowVersion.Current);
                    AgregarParametro(ModifyOntologiaProyectoCommand, IBD.ToParam("CachearDatosSemanticos"), DbType.Boolean, "CachearDatosSemanticos", DataRowVersion.Current);
                    AgregarParametro(ModifyOntologiaProyectoCommand, IBD.ToParam("EsBuscable"), DbType.Boolean, "EsBuscable", DataRowVersion.Current);

                    //ActualizarBaseDeDatos(addedAndModifiedDataSet, "OntologiaProyecto", InsertOntologiaProyectoCommand, ModifyOntologiaProyectoCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #region Actualizar tabla FacetaObjetoConocimiento
                    DbCommand InsertFacetaObjetoConocimientoCommand = ObtenerComando(sqlFacetaObjetoConocimientoInsert);
                    AgregarParametro(InsertFacetaObjetoConocimientoCommand, IBD.ToParam("ObjetoConocimiento"), DbType.String, "ObjetoConocimiento", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaObjetoConocimientoCommand, IBD.ToParam("Faceta"), DbType.String, "Faceta", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaObjetoConocimientoCommand, IBD.ToParam("NombreFaceta"), DbType.String, "NombreFaceta", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaObjetoConocimientoCommand, IBD.ToParam("Orden"), DbType.Int16, "Orden", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaObjetoConocimientoCommand, IBD.ToParam("Autocompletar"), DbType.Boolean, "Autocompletar", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaObjetoConocimientoCommand, IBD.ToParam("TipoPropiedad"), DbType.Int16, "TipoPropiedad", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaObjetoConocimientoCommand, IBD.ToParam("TipoDisenio"), DbType.Int16, "TipoDisenio", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaObjetoConocimientoCommand, IBD.ToParam("ElementosVisibles"), DbType.Int16, "ElementosVisibles", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaObjetoConocimientoCommand, IBD.ToParam("AlgoritmoTransformacion"), DbType.Int16, "AlgoritmoTransformacion", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaObjetoConocimientoCommand, IBD.ToParam("EsSemantica"), DbType.Boolean, "EsSemantica", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaObjetoConocimientoCommand, IBD.ToParam("Mayusculas"), DbType.Int16, "Mayusculas", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaObjetoConocimientoCommand, IBD.ToParam("EsPorDefecto"), DbType.Boolean, "EsPorDefecto", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaObjetoConocimientoCommand, IBD.ToParam("ComportamientoOr"), DbType.Boolean, "ComportamientoOr", DataRowVersion.Current);

                    DbCommand ModifyFacetaObjetoConocimientoCommand = ObtenerComando(sqlFacetaObjetoConocimientoModify);
                    AgregarParametro(ModifyFacetaObjetoConocimientoCommand, IBD.ToParam("Original_ObjetoConocimiento"), DbType.String, "ObjetoConocimiento", DataRowVersion.Original);
                    AgregarParametro(ModifyFacetaObjetoConocimientoCommand, IBD.ToParam("Original_Faceta"), DbType.String, "Faceta", DataRowVersion.Original);
                    AgregarParametro(ModifyFacetaObjetoConocimientoCommand, IBD.ToParam("Original_NombreFaceta"), DbType.String, "NombreFaceta", DataRowVersion.Original);
                    AgregarParametro(ModifyFacetaObjetoConocimientoCommand, IBD.ToParam("Original_Orden"), DbType.Int16, "Orden", DataRowVersion.Original);
                    AgregarParametro(ModifyFacetaObjetoConocimientoCommand, IBD.ToParam("Original_Autocompletar"), DbType.Boolean, "Autocompletar", DataRowVersion.Original);
                    AgregarParametro(ModifyFacetaObjetoConocimientoCommand, IBD.ToParam("Original_TipoPropiedad"), DbType.Int16, "TipoPropiedad", DataRowVersion.Original);
                    AgregarParametro(ModifyFacetaObjetoConocimientoCommand, IBD.ToParam("Original_TipoDisenio"), DbType.Int16, "TipoDisenio", DataRowVersion.Original);
                    AgregarParametro(ModifyFacetaObjetoConocimientoCommand, IBD.ToParam("Original_ElementosVisibles"), DbType.Int16, "ElementosVisibles", DataRowVersion.Original);
                    AgregarParametro(ModifyFacetaObjetoConocimientoCommand, IBD.ToParam("Original_AlgoritmoTransformacion"), DbType.Int16, "AlgoritmoTransformacion", DataRowVersion.Original);
                    AgregarParametro(ModifyFacetaObjetoConocimientoCommand, IBD.ToParam("Original_EsSemantica"), DbType.Boolean, "EsSemantica", DataRowVersion.Original);
                    AgregarParametro(ModifyFacetaObjetoConocimientoCommand, IBD.ToParam("Original_Mayusculas"), DbType.Int16, "Mayusculas", DataRowVersion.Original);
                    AgregarParametro(ModifyFacetaObjetoConocimientoCommand, IBD.ToParam("Original_EsPorDefecto"), DbType.Boolean, "EsPorDefecto", DataRowVersion.Original);
                    AgregarParametro(ModifyFacetaObjetoConocimientoCommand, IBD.ToParam("Original_ComportamientoOr"), DbType.Boolean, "ComportamientoOr", DataRowVersion.Original);

                    AgregarParametro(ModifyFacetaObjetoConocimientoCommand, IBD.ToParam("ObjetoConocimiento"), DbType.String, "ObjetoConocimiento", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaObjetoConocimientoCommand, IBD.ToParam("Faceta"), DbType.String, "Faceta", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaObjetoConocimientoCommand, IBD.ToParam("NombreFaceta"), DbType.String, "NombreFaceta", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaObjetoConocimientoCommand, IBD.ToParam("Orden"), DbType.Int16, "Orden", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaObjetoConocimientoCommand, IBD.ToParam("Autocompletar"), DbType.Boolean, "Autocompletar", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaObjetoConocimientoCommand, IBD.ToParam("TipoPropiedad"), DbType.Int16, "TipoPropiedad", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaObjetoConocimientoCommand, IBD.ToParam("TipoDisenio"), DbType.Int16, "TipoDisenio", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaObjetoConocimientoCommand, IBD.ToParam("ElementosVisibles"), DbType.Int16, "ElementosVisibles", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaObjetoConocimientoCommand, IBD.ToParam("AlgoritmoTransformacion"), DbType.Int16, "AlgoritmoTransformacion", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaObjetoConocimientoCommand, IBD.ToParam("EsSemantica"), DbType.Boolean, "EsSemantica", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaObjetoConocimientoCommand, IBD.ToParam("Mayusculas"), DbType.Int16, "Mayusculas", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaObjetoConocimientoCommand, IBD.ToParam("EsPorDefecto"), DbType.Boolean, "EsPorDefecto", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaObjetoConocimientoCommand, IBD.ToParam("ComportamientoOr"), DbType.Boolean, "ComportamientoOr", DataRowVersion.Current);
                    //ActualizarBaseDeDatos(addedAndModifiedDataSet, "FacetaObjetoConocimiento", InsertFacetaObjetoConocimientoCommand, ModifyFacetaObjetoConocimientoCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);



                    #endregion

                    #region Actualizar tabla FacetaObjetoConocimientoProyecto
                    DbCommand InsertFacetaObjetoConocimientoProyectoCommand = ObtenerComando(sqlFacetaObjetoConocimientoProyectoInsert);
                    AgregarParametro(InsertFacetaObjetoConocimientoProyectoCommand, IBD.ToParam("OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaObjetoConocimientoProyectoCommand, IBD.ToParam("ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaObjetoConocimientoProyectoCommand, IBD.ToParam("ObjetoConocimiento"), DbType.String, "ObjetoConocimiento", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaObjetoConocimientoProyectoCommand, IBD.ToParam("Faceta"), DbType.String, "Faceta", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaObjetoConocimientoProyectoCommand, IBD.ToParam("Orden"), DbType.Int16, "Orden", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaObjetoConocimientoProyectoCommand, IBD.ToParam("Autocompletar"), DbType.Boolean, "Autocompletar", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaObjetoConocimientoProyectoCommand, IBD.ToParam("TipoPropiedad"), DbType.Int16, "TipoPropiedad", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaObjetoConocimientoProyectoCommand, IBD.ToParam("Comportamiento"), DbType.Int16, "Comportamiento", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaObjetoConocimientoProyectoCommand, IBD.ToParam("Excluyente"), DbType.Boolean, "Excluyente", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaObjetoConocimientoProyectoCommand, IBD.ToParam("NombreFaceta"), DbType.String, "NombreFaceta", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaObjetoConocimientoProyectoCommand, IBD.ToParam("TipoDisenio"), DbType.Int16, "TipoDisenio", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaObjetoConocimientoProyectoCommand, IBD.ToParam("ElementosVisibles"), DbType.Int16, "ElementosVisibles", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaObjetoConocimientoProyectoCommand, IBD.ToParam("AlgoritmoTransformacion"), DbType.Int16, "AlgoritmoTransformacion", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaObjetoConocimientoProyectoCommand, IBD.ToParam("EsSemantica"), DbType.Boolean, "EsSemantica", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaObjetoConocimientoProyectoCommand, IBD.ToParam("Mayusculas"), DbType.Int16, "Mayusculas", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaObjetoConocimientoProyectoCommand, IBD.ToParam("NivelSemantico"), DbType.String, "NivelSemantico", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaObjetoConocimientoProyectoCommand, IBD.ToParam("Condicion"), DbType.String, "Condicion", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaObjetoConocimientoProyectoCommand, IBD.ToParam("FacetaPrivadaParaGrupoEditores"), DbType.String, "FacetaPrivadaParaGrupoEditores", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaObjetoConocimientoProyectoCommand, IBD.ToParam("Reciproca"), DbType.Int16, "Reciproca", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaObjetoConocimientoProyectoCommand, IBD.ToParam("ComportamientoOr"), DbType.Boolean, "ComportamientoOr", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaObjetoConocimientoProyectoCommand, IBD.ToParam("OcultaEnFacetas"), DbType.Boolean, "OcultaEnFacetas", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaObjetoConocimientoProyectoCommand, IBD.ToParam("OcultaEnFiltros"), DbType.Boolean, "OcultaEnFiltros", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaObjetoConocimientoProyectoCommand, IBD.ToParam("PriorizarOrdenResultados"), DbType.Boolean, "PriorizarOrdenResultados", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaObjetoConocimientoProyectoCommand, IBD.ToParam("Inmutable"), DbType.Boolean, "Inmutable", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaObjetoConocimientoProyectoCommand, IBD.ToParam("AgrupacionID"), IBD.TipoGuidToObject(DbType.Guid), "AgrupacionID", DataRowVersion.Current);

                    DbCommand ModifyFacetaObjetoConocimientoProyectoCommand = ObtenerComando(sqlFacetaObjetoConocimientoProyectoModify);
                    AgregarParametro(ModifyFacetaObjetoConocimientoProyectoCommand, IBD.ToParam("Original_OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Original);
                    AgregarParametro(ModifyFacetaObjetoConocimientoProyectoCommand, IBD.ToParam("Original_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);
                    AgregarParametro(ModifyFacetaObjetoConocimientoProyectoCommand, IBD.ToParam("Original_ObjetoConocimiento"), DbType.String, "ObjetoConocimiento", DataRowVersion.Original);
                    AgregarParametro(ModifyFacetaObjetoConocimientoProyectoCommand, IBD.ToParam("Original_Faceta"), DbType.String, "Faceta", DataRowVersion.Original);

                    AgregarParametro(ModifyFacetaObjetoConocimientoProyectoCommand, IBD.ToParam("OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaObjetoConocimientoProyectoCommand, IBD.ToParam("ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaObjetoConocimientoProyectoCommand, IBD.ToParam("ObjetoConocimiento"), DbType.String, "ObjetoConocimiento", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaObjetoConocimientoProyectoCommand, IBD.ToParam("Faceta"), DbType.String, "Faceta", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaObjetoConocimientoProyectoCommand, IBD.ToParam("Orden"), DbType.Int16, "Orden", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaObjetoConocimientoProyectoCommand, IBD.ToParam("Autocompletar"), DbType.Boolean, "Autocompletar", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaObjetoConocimientoProyectoCommand, IBD.ToParam("TipoPropiedad"), DbType.Int16, "TipoPropiedad", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaObjetoConocimientoProyectoCommand, IBD.ToParam("Comportamiento"), DbType.Int16, "Comportamiento", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaObjetoConocimientoProyectoCommand, IBD.ToParam("Excluyente"), DbType.Boolean, "Excluyente", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaObjetoConocimientoProyectoCommand, IBD.ToParam("NombreFaceta"), DbType.String, "NombreFaceta", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaObjetoConocimientoProyectoCommand, IBD.ToParam("TipoDisenio"), DbType.Int16, "TipoDisenio", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaObjetoConocimientoProyectoCommand, IBD.ToParam("ElementosVisibles"), DbType.Int16, "ElementosVisibles", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaObjetoConocimientoProyectoCommand, IBD.ToParam("AlgoritmoTransformacion"), DbType.Int16, "AlgoritmoTransformacion", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaObjetoConocimientoProyectoCommand, IBD.ToParam("EsSemantica"), DbType.Boolean, "EsSemantica", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaObjetoConocimientoProyectoCommand, IBD.ToParam("Mayusculas"), DbType.Int16, "Mayusculas", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaObjetoConocimientoProyectoCommand, IBD.ToParam("NivelSemantico"), DbType.String, "NivelSemantico", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaObjetoConocimientoProyectoCommand, IBD.ToParam("Condicion"), DbType.String, "Condicion", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaObjetoConocimientoProyectoCommand, IBD.ToParam("FacetaPrivadaParaGrupoEditores"), DbType.String, "FacetaPrivadaParaGrupoEditores", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaObjetoConocimientoProyectoCommand, IBD.ToParam("Reciproca"), DbType.Int16, "Reciproca", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaObjetoConocimientoProyectoCommand, IBD.ToParam("ComportamientoOr"), DbType.Boolean, "ComportamientoOr", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaObjetoConocimientoProyectoCommand, IBD.ToParam("OcultaEnFacetas"), DbType.Boolean, "OcultaEnFacetas", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaObjetoConocimientoProyectoCommand, IBD.ToParam("OcultaEnFiltros"), DbType.Boolean, "OcultaEnFiltros", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaObjetoConocimientoProyectoCommand, IBD.ToParam("PriorizarOrdenResultados"), DbType.Boolean, "PriorizarOrdenResultados", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaObjetoConocimientoProyectoCommand, IBD.ToParam("Inmutable"), DbType.Boolean, "Inmutable", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaObjetoConocimientoProyectoCommand, IBD.ToParam("AgrupacionID"), IBD.TipoGuidToObject(DbType.Guid), "AgrupacionID", DataRowVersion.Current);

                    //ActualizarBaseDeDatos(addedAndModifiedDataSet, "FacetaObjetoConocimientoProyecto", InsertFacetaObjetoConocimientoProyectoCommand, ModifyFacetaObjetoConocimientoProyectoCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #region Actualizar tabla FacetaObjetoConocimientoProyectoPestanya
                    DbCommand InsertFacetaObjetoConocimientoProyectoPestanyaCommand = ObtenerComando(sqlFacetaObjetoConocimientoProyectoPestanyaInsert);
                    AgregarParametro(InsertFacetaObjetoConocimientoProyectoPestanyaCommand, IBD.ToParam("OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaObjetoConocimientoProyectoPestanyaCommand, IBD.ToParam("ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaObjetoConocimientoProyectoPestanyaCommand, IBD.ToParam("ObjetoConocimiento"), DbType.String, "ObjetoConocimiento", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaObjetoConocimientoProyectoPestanyaCommand, IBD.ToParam("Faceta"), DbType.String, "Faceta", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaObjetoConocimientoProyectoPestanyaCommand, IBD.ToParam("PestanyaID"), IBD.TipoGuidToObject(DbType.Guid), "PestanyaID", DataRowVersion.Current);

                    DbCommand ModifyFacetaObjetoConocimientoProyectoPestanyaCommand = ObtenerComando(sqlFacetaObjetoConocimientoProyectoPestanyaModify);
                    AgregarParametro(ModifyFacetaObjetoConocimientoProyectoPestanyaCommand, IBD.ToParam("Original_OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Original);
                    AgregarParametro(ModifyFacetaObjetoConocimientoProyectoPestanyaCommand, IBD.ToParam("Original_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);
                    AgregarParametro(ModifyFacetaObjetoConocimientoProyectoPestanyaCommand, IBD.ToParam("Original_ObjetoConocimiento"), DbType.String, "ObjetoConocimiento", DataRowVersion.Original);
                    AgregarParametro(ModifyFacetaObjetoConocimientoProyectoPestanyaCommand, IBD.ToParam("Original_Faceta"), DbType.String, "Faceta", DataRowVersion.Original);
                    AgregarParametro(ModifyFacetaObjetoConocimientoProyectoPestanyaCommand, IBD.ToParam("Original_PestanyaID"), IBD.TipoGuidToObject(DbType.Guid), "Original_PestanyaID", DataRowVersion.Original);

                    AgregarParametro(ModifyFacetaObjetoConocimientoProyectoPestanyaCommand, IBD.ToParam("OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaObjetoConocimientoProyectoPestanyaCommand, IBD.ToParam("ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaObjetoConocimientoProyectoPestanyaCommand, IBD.ToParam("ObjetoConocimiento"), DbType.String, "ObjetoConocimiento", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaObjetoConocimientoProyectoPestanyaCommand, IBD.ToParam("Faceta"), DbType.String, "Faceta", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaObjetoConocimientoProyectoPestanyaCommand, IBD.ToParam("PestanyaID"), IBD.TipoGuidToObject(DbType.Guid), "PestanyaID", DataRowVersion.Current);

                    ActualizarBaseDeDatos(addedAndModifiedDataSet, "FacetaObjetoConocimientoProyectoPestanya", InsertFacetaObjetoConocimientoProyectoPestanyaCommand, ModifyFacetaObjetoConocimientoProyectoPestanyaCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion


                    #region Actualizar tabla FacetaHome
                    DbCommand InsertFacetaHomeCommand = ObtenerComando(sqlFacetaHomeInsert);
                    AgregarParametro(InsertFacetaHomeCommand, IBD.ToParam("OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaHomeCommand, IBD.ToParam("ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaHomeCommand, IBD.ToParam("ObjetoConocimiento"), DbType.String, "ObjetoConocimiento", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaHomeCommand, IBD.ToParam("Faceta"), DbType.String, "Faceta", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaHomeCommand, IBD.ToParam("PestanyaFaceta"), DbType.String, "PestanyaFaceta", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaHomeCommand, IBD.ToParam("Orden"), DbType.Int16, "Orden", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaHomeCommand, IBD.ToParam("MostrarVerMas"), DbType.Boolean, "MostrarVerMas", DataRowVersion.Current);

                    DbCommand ModifyFacetaHomeCommand = ObtenerComando(sqlFacetaHomeModify);
                    AgregarParametro(ModifyFacetaHomeCommand, IBD.ToParam("Original_OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Original);
                    AgregarParametro(ModifyFacetaHomeCommand, IBD.ToParam("Original_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);
                    AgregarParametro(ModifyFacetaHomeCommand, IBD.ToParam("Original_ObjetoConocimiento"), DbType.String, "ObjetoConocimiento", DataRowVersion.Original);
                    AgregarParametro(ModifyFacetaHomeCommand, IBD.ToParam("Original_Faceta"), DbType.String, "Faceta", DataRowVersion.Original);
                    AgregarParametro(ModifyFacetaHomeCommand, IBD.ToParam("Original_PestanyaFaceta"), DbType.String, "PestanyaFaceta", DataRowVersion.Original);
                    AgregarParametro(ModifyFacetaHomeCommand, IBD.ToParam("Original_Orden"), DbType.Int16, "Orden", DataRowVersion.Original);
                    AgregarParametro(ModifyFacetaHomeCommand, IBD.ToParam("Original_MostrarVerMas"), DbType.Boolean, "MostrarVerMas", DataRowVersion.Current);

                    AgregarParametro(ModifyFacetaHomeCommand, IBD.ToParam("OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaHomeCommand, IBD.ToParam("ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaHomeCommand, IBD.ToParam("ObjetoConocimiento"), DbType.String, "ObjetoConocimiento", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaHomeCommand, IBD.ToParam("Faceta"), DbType.String, "Faceta", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaHomeCommand, IBD.ToParam("PestanyaFaceta"), DbType.String, "PestanyaFaceta", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaHomeCommand, IBD.ToParam("Orden"), DbType.Int16, "Orden", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaHomeCommand, IBD.ToParam("MostrarVerMas"), DbType.Boolean, "MostrarVerMas", DataRowVersion.Current);
                    ActualizarBaseDeDatos(addedAndModifiedDataSet, "FacetaHome", InsertFacetaHomeCommand, ModifyFacetaHomeCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #region Actualizar tabla FacetaFiltroHome
                    DbCommand InsertFacetaFiltroHomeCommand = ObtenerComando(sqlFacetaFiltroHomeInsert);
                    AgregarParametro(InsertFacetaFiltroHomeCommand, IBD.ToParam("OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaFiltroHomeCommand, IBD.ToParam("ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaFiltroHomeCommand, IBD.ToParam("ObjetoConocimiento"), DbType.String, "ObjetoConocimiento", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaFiltroHomeCommand, IBD.ToParam("Faceta"), DbType.String, "Faceta", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaFiltroHomeCommand, IBD.ToParam("Filtro"), DbType.String, "Filtro", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaFiltroHomeCommand, IBD.ToParam("Orden"), DbType.Int16, "Orden", DataRowVersion.Current);

                    DbCommand ModifyFacetaFiltroHomeCommand = ObtenerComando(sqlFacetaFiltroHomeModify);
                    AgregarParametro(ModifyFacetaFiltroHomeCommand, IBD.ToParam("Original_OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Original);
                    AgregarParametro(ModifyFacetaFiltroHomeCommand, IBD.ToParam("Original_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);
                    AgregarParametro(ModifyFacetaFiltroHomeCommand, IBD.ToParam("Original_ObjetoConocimiento"), DbType.String, "ObjetoConocimiento", DataRowVersion.Original);
                    AgregarParametro(ModifyFacetaFiltroHomeCommand, IBD.ToParam("Original_Faceta"), DbType.String, "Faceta", DataRowVersion.Original);
                    AgregarParametro(ModifyFacetaFiltroHomeCommand, IBD.ToParam("Original_Filtro"), DbType.String, "Filtro", DataRowVersion.Original);
                    AgregarParametro(ModifyFacetaFiltroHomeCommand, IBD.ToParam("Original_Orden"), DbType.Int16, "Orden", DataRowVersion.Original);

                    AgregarParametro(ModifyFacetaFiltroHomeCommand, IBD.ToParam("OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaFiltroHomeCommand, IBD.ToParam("ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaFiltroHomeCommand, IBD.ToParam("ObjetoConocimiento"), DbType.String, "ObjetoConocimiento", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaFiltroHomeCommand, IBD.ToParam("Faceta"), DbType.String, "Faceta", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaFiltroHomeCommand, IBD.ToParam("Filtro"), DbType.String, "Filtro", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaFiltroHomeCommand, IBD.ToParam("Orden"), DbType.Int16, "Orden", DataRowVersion.Current);
                    ActualizarBaseDeDatos(addedAndModifiedDataSet, "FacetaFiltroHome", InsertFacetaFiltroHomeCommand, ModifyFacetaFiltroHomeCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #region Actualizar tabla FacetaFiltroProyecto
                    DbCommand InsertFacetaFiltroProyectoCommand = ObtenerComando(sqlFacetaFiltroProyectoInsert);
                    AgregarParametro(InsertFacetaFiltroProyectoCommand, IBD.ToParam("OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaFiltroProyectoCommand, IBD.ToParam("ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaFiltroProyectoCommand, IBD.ToParam("ObjetoConocimiento"), DbType.String, "ObjetoConocimiento", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaFiltroProyectoCommand, IBD.ToParam("Faceta"), DbType.String, "Faceta", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaFiltroProyectoCommand, IBD.ToParam("Filtro"), DbType.String, "Filtro", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaFiltroProyectoCommand, IBD.ToParam("Orden"), DbType.Int16, "Orden", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaFiltroProyectoCommand, IBD.ToParam("Condicion"), DbType.String, "Condicion", DataRowVersion.Current);

                    DbCommand ModifyFacetaFiltroProyectoCommand = ObtenerComando(sqlFacetaFiltroProyectoModify);
                    AgregarParametro(ModifyFacetaFiltroProyectoCommand, IBD.ToParam("Original_OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Original);
                    AgregarParametro(ModifyFacetaFiltroProyectoCommand, IBD.ToParam("Original_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);
                    AgregarParametro(ModifyFacetaFiltroProyectoCommand, IBD.ToParam("Original_ObjetoConocimiento"), DbType.String, "ObjetoConocimiento", DataRowVersion.Original);
                    AgregarParametro(ModifyFacetaFiltroProyectoCommand, IBD.ToParam("Original_Faceta"), DbType.String, "Faceta", DataRowVersion.Original);
                    AgregarParametro(ModifyFacetaFiltroProyectoCommand, IBD.ToParam("Original_Filtro"), DbType.String, "Filtro", DataRowVersion.Original);
                    AgregarParametro(ModifyFacetaFiltroProyectoCommand, IBD.ToParam("Original_Orden"), DbType.Int16, "Orden", DataRowVersion.Original);

                    AgregarParametro(ModifyFacetaFiltroProyectoCommand, IBD.ToParam("OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaFiltroProyectoCommand, IBD.ToParam("ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaFiltroProyectoCommand, IBD.ToParam("ObjetoConocimiento"), DbType.String, "ObjetoConocimiento", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaFiltroProyectoCommand, IBD.ToParam("Faceta"), DbType.String, "Faceta", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaFiltroProyectoCommand, IBD.ToParam("Filtro"), DbType.String, "Filtro", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaFiltroProyectoCommand, IBD.ToParam("Orden"), DbType.Int16, "Orden", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaFiltroProyectoCommand, IBD.ToParam("Condicion"), DbType.String, "Condicion", DataRowVersion.Current);
                    //ActualizarBaseDeDatos(addedAndModifiedDataSet, "FacetaFiltroProyecto", InsertFacetaFiltroProyectoCommand, ModifyFacetaFiltroProyectoCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #region Actualizar tabla FacetaEntidadesExternas
                    DbCommand InsertFacetaEntidadesExternasCommand = ObtenerComando(sqlFacetaEntidadesExternasInsert);
                    AgregarParametro(InsertFacetaEntidadesExternasCommand, IBD.ToParam("OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaEntidadesExternasCommand, IBD.ToParam("ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaEntidadesExternasCommand, IBD.ToParam("EntidadID"), DbType.String, "EntidadID", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaEntidadesExternasCommand, IBD.ToParam("Grafo"), DbType.String, "Grafo", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaEntidadesExternasCommand, IBD.ToParam("EsEntidadSecundaria"), DbType.Boolean, "EsEntidadSecundaria", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaEntidadesExternasCommand, IBD.ToParam("BuscarConRecursividad"), DbType.Boolean, "BuscarConRecursividad", DataRowVersion.Current);

                    DbCommand ModifyFacetaEntidadesExternasCommand = ObtenerComando(sqlFacetaEntidadesExternasModify);
                    AgregarParametro(ModifyFacetaEntidadesExternasCommand, IBD.ToParam("Original_OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Original);
                    AgregarParametro(ModifyFacetaEntidadesExternasCommand, IBD.ToParam("Original_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);
                    AgregarParametro(ModifyFacetaEntidadesExternasCommand, IBD.ToParam("Original_EntidadID"), DbType.String, "EntidadID", DataRowVersion.Original);
                    AgregarParametro(ModifyFacetaEntidadesExternasCommand, IBD.ToParam("Original_Grafo"), DbType.String, "Grafo", DataRowVersion.Original);
                    AgregarParametro(ModifyFacetaEntidadesExternasCommand, IBD.ToParam("Original_EsEntidadSecundaria"), DbType.Boolean, "EsEntidadSecundaria", DataRowVersion.Original);
                    AgregarParametro(ModifyFacetaEntidadesExternasCommand, IBD.ToParam("Original_BuscarConRecursividad"), DbType.Boolean, "BuscarConRecursividad", DataRowVersion.Original);

                    AgregarParametro(ModifyFacetaEntidadesExternasCommand, IBD.ToParam("OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaEntidadesExternasCommand, IBD.ToParam("ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaEntidadesExternasCommand, IBD.ToParam("EntidadID"), DbType.String, "EntidadID", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaEntidadesExternasCommand, IBD.ToParam("Grafo"), DbType.String, "Grafo", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaEntidadesExternasCommand, IBD.ToParam("EsEntidadSecundaria"), DbType.Boolean, "EsEntidadSecundaria", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaEntidadesExternasCommand, IBD.ToParam("BuscarConRecursividad"), DbType.Boolean, "BuscarConRecursividad", DataRowVersion.Current);
                    //ActualizarBaseDeDatos(addedAndModifiedDataSet, "FacetaEntidadesExternas", InsertFacetaEntidadesExternasCommand, ModifyFacetaEntidadesExternasCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #region Actualizar tabla FacetaConfigProyChart
                    DbCommand InsertFacetaConfigProyChartCommand = ObtenerComando(sqlFacetaConfigProyChartInsert);
                    AgregarParametro(InsertFacetaConfigProyChartCommand, IBD.ToParam("OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaConfigProyChartCommand, IBD.ToParam("ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaConfigProyChartCommand, IBD.ToParam("ChartID"), IBD.TipoGuidToObject(DbType.Guid), "ChartID", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaConfigProyChartCommand, IBD.ToParam("Nombre"), DbType.String, "Nombre", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaConfigProyChartCommand, IBD.ToParam("Orden"), DbType.Int16, "Orden", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaConfigProyChartCommand, IBD.ToParam("SelectConsultaVirtuoso"), DbType.String, "SelectConsultaVirtuoso", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaConfigProyChartCommand, IBD.ToParam("FiltrosConsultaVirtuoso"), DbType.String, "FiltrosConsultaVirtuoso", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaConfigProyChartCommand, IBD.ToParam("JSBase"), DbType.String, "JSBase", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaConfigProyChartCommand, IBD.ToParam("JSBusqueda"), DbType.String, "JSBusqueda", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaConfigProyChartCommand, IBD.ToParam("Ontologias"), DbType.String, "Ontologias", DataRowVersion.Current);

                    DbCommand ModifyFacetaConfigProyChartCommand = ObtenerComando(sqlFacetaConfigProyChartModify);
                    AgregarParametro(ModifyFacetaConfigProyChartCommand, IBD.ToParam("Original_OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Original);
                    AgregarParametro(ModifyFacetaConfigProyChartCommand, IBD.ToParam("Original_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);
                    AgregarParametro(ModifyFacetaConfigProyChartCommand, IBD.ToParam("Original_ChartID"), IBD.TipoGuidToObject(DbType.Guid), "ChartID", DataRowVersion.Original);

                    AgregarParametro(ModifyFacetaConfigProyChartCommand, IBD.ToParam("OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaConfigProyChartCommand, IBD.ToParam("ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaConfigProyChartCommand, IBD.ToParam("ChartID"), IBD.TipoGuidToObject(DbType.Guid), "ChartID", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaConfigProyChartCommand, IBD.ToParam("Nombre"), DbType.String, "Nombre", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaConfigProyChartCommand, IBD.ToParam("Orden"), DbType.Int16, "Orden", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaConfigProyChartCommand, IBD.ToParam("SelectConsultaVirtuoso"), DbType.String, "SelectConsultaVirtuoso", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaConfigProyChartCommand, IBD.ToParam("FiltrosConsultaVirtuoso"), DbType.String, "FiltrosConsultaVirtuoso", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaConfigProyChartCommand, IBD.ToParam("JSBase"), DbType.String, "JSBase", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaConfigProyChartCommand, IBD.ToParam("JSBusqueda"), DbType.String, "JSBusqueda", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaConfigProyChartCommand, IBD.ToParam("Ontologias"), DbType.String, "Ontologias", DataRowVersion.Current);
                    ActualizarBaseDeDatos(addedAndModifiedDataSet, "FacetaConfigProyChart", InsertFacetaConfigProyChartCommand, ModifyFacetaConfigProyChartCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #region Actualizar tabla FacetaConfigProyMapa
                    DbCommand InsertFacetaConfigProyMapaCommand = ObtenerComando(sqlFacetaConfigProyMapaInsert);
                    AgregarParametro(InsertFacetaConfigProyMapaCommand, IBD.ToParam("OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaConfigProyMapaCommand, IBD.ToParam("ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaConfigProyMapaCommand, IBD.ToParam("PropLatitud"), DbType.String, "PropLatitud", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaConfigProyMapaCommand, IBD.ToParam("PropLongitud"), DbType.String, "PropLongitud", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaConfigProyMapaCommand, IBD.ToParam("PropRuta"), DbType.String, "PropRuta", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaConfigProyMapaCommand, IBD.ToParam("ColorRuta"), DbType.String, "ColorRuta", DataRowVersion.Current);

                    DbCommand ModifyFacetaConfigProyMapaCommand = ObtenerComando(sqlFacetaConfigProyMapaModify);
                    AgregarParametro(ModifyFacetaConfigProyMapaCommand, IBD.ToParam("Original_OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Original);
                    AgregarParametro(ModifyFacetaConfigProyMapaCommand, IBD.ToParam("Original_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);
                    AgregarParametro(ModifyFacetaConfigProyMapaCommand, IBD.ToParam("Original_PropLatitud"), DbType.String, "PropLatitud", DataRowVersion.Original);
                    AgregarParametro(ModifyFacetaConfigProyMapaCommand, IBD.ToParam("Original_PropLongitud"), DbType.String, "PropLongitud", DataRowVersion.Original);

                    AgregarParametro(ModifyFacetaConfigProyMapaCommand, IBD.ToParam("OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaConfigProyMapaCommand, IBD.ToParam("ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaConfigProyMapaCommand, IBD.ToParam("PropLatitud"), DbType.String, "PropLatitud", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaConfigProyMapaCommand, IBD.ToParam("PropLongitud"), DbType.String, "PropLongitud", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaConfigProyMapaCommand, IBD.ToParam("PropRuta"), DbType.String, "PropRuta", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaConfigProyMapaCommand, IBD.ToParam("ColorRuta"), DbType.String, "ColorRuta", DataRowVersion.Current);

                    ActualizarBaseDeDatos(addedAndModifiedDataSet, "FacetaConfigProyMapa", InsertFacetaConfigProyMapaCommand, ModifyFacetaConfigProyMapaCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #region Actualizar tabla FacetaRedireccion
                    DbCommand InsertFacetaRedireccionCommand = ObtenerComando(sqlFacetaRedireccionInsert);
                    AgregarParametro(InsertFacetaRedireccionCommand, IBD.ToParam("Faceta"), DbType.String, "Faceta", DataRowVersion.Current);
                    AgregarParametro(InsertFacetaRedireccionCommand, IBD.ToParam("Nombre"), DbType.String, "Nombre", DataRowVersion.Current);

                    DbCommand ModifyFacetaRedireccionCommand = ObtenerComando(sqlFacetaRedireccionModify);
                    AgregarParametro(ModifyFacetaRedireccionCommand, IBD.ToParam("Original_Faceta"), DbType.String, "Faceta", DataRowVersion.Original);
                    AgregarParametro(ModifyFacetaRedireccionCommand, IBD.ToParam("Original_Nombre"), DbType.String, "Nombre", DataRowVersion.Original);

                    AgregarParametro(ModifyFacetaRedireccionCommand, IBD.ToParam("Faceta"), DbType.String, "Faceta", DataRowVersion.Current);
                    AgregarParametro(ModifyFacetaRedireccionCommand, IBD.ToParam("Nombre"), DbType.String, "Nombre", DataRowVersion.Current);
                    ActualizarBaseDeDatos(addedAndModifiedDataSet, "FacetaRedireccion", InsertFacetaRedireccionCommand, ModifyFacetaRedireccionCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #region Actualizar tabla ConfiguracionConexionGrafo
                    DbCommand InsertConfiguracionConexionGrafoCommand = ObtenerComando(sqlConfiguracionConexionGrafoInsert);
                    AgregarParametro(InsertConfiguracionConexionGrafoCommand, IBD.ToParam("Grafo"), DbType.String, "Grafo", DataRowVersion.Current);
                    AgregarParametro(InsertConfiguracionConexionGrafoCommand, IBD.ToParam("CadenaConexion"), DbType.String, "CadenaConexion", DataRowVersion.Current);

                    DbCommand ModifyConfiguracionConexionGrafoCommand = ObtenerComando(sqlConfiguracionConexionGrafoModify);
                    AgregarParametro(ModifyConfiguracionConexionGrafoCommand, IBD.ToParam("Original_Grafo"), DbType.String, "Grafo", DataRowVersion.Original);
                    AgregarParametro(ModifyConfiguracionConexionGrafoCommand, IBD.ToParam("Original_CadenaConexion"), DbType.String, "CadenaConexion", DataRowVersion.Original);

                    AgregarParametro(ModifyConfiguracionConexionGrafoCommand, IBD.ToParam("Grafo"), DbType.String, "Grafo", DataRowVersion.Current);
                    AgregarParametro(ModifyConfiguracionConexionGrafoCommand, IBD.ToParam("CadenaConexion"), DbType.String, "CadenaConexion", DataRowVersion.Current);
                    ActualizarBaseDeDatos(addedAndModifiedDataSet, "ConfiguracionConexionGrafo", InsertConfiguracionConexionGrafoCommand, ModifyConfiguracionConexionGrafoCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #endregion

                    addedAndModifiedDataSet.Dispose();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// En caso de que se utilice un GnossConfig.xml que no es el de por defecto se pasa un objeto IBaseDatos creado con respecto
        /// al fichero de configuracion que se ha apsado como parámetro
        /// </summary>
        /// <param name="IBD">Objecto IBaseDatos para el archivo pasado al constructor del AD</param>
        private void CargarConsultasYDataAdapters(IBaseDatos IBD)
        {

            #region Consultas

            this.sqlSelectFacetaHome = "SELECT " + IBD.CargarGuid("FacetaHome.OrganizacionID") + ", " + IBD.CargarGuid("FacetaHome.ProyectoID") + ", FacetaHome.ObjetoConocimiento, FacetaHome.Faceta, FacetaHome.PestanyaFaceta, FacetaHome.Orden, FacetaHome.MostrarVerMas FROM FacetaHome";
            this.sqlSelectFacetaFiltroHome = "SELECT " + IBD.CargarGuid("FacetaFiltroHome.OrganizacionID") + ", " + IBD.CargarGuid("FacetaFiltroHome.ProyectoID") + ", FacetaFiltroHome.ObjetoConocimiento, FacetaFiltroHome.Faceta, FacetaFiltroHome.Filtro, FacetaFiltroHome.Orden FROM FacetaFiltroHome";
            this.sqlSelectFacetaExcluida = "SELECT " + IBD.CargarGuid("FacetaExcluida.OrganizacionID") + ", " + IBD.CargarGuid("FacetaExcluida.ProyectoID") + ", FacetaExcluida.Faceta FROM FacetaExcluida";
            this.sqlSelectOntologiaProyecto = "SELECT " + IBD.CargarGuid("OntologiaProyecto.OrganizacionID") + ", " + IBD.CargarGuid("OntologiaProyecto.ProyectoID") + ", OntologiaProyecto.OntologiaProyecto, OntologiaProyecto.NombreOnt, OntologiaProyecto.Namespace, OntologiaProyecto.NamespacesExtra, OntologiaProyecto.SubTipos, OntologiaProyecto.NombreCortoOnt, OntologiaProyecto.CachearDatosSemanticos, OntologiaProyecto.EsBuscable FROM OntologiaProyecto";
            this.sqlSelectFacetaObjetoConocimiento = "SELECT FacetaObjetoConocimiento.ObjetoConocimiento, FacetaObjetoConocimiento.Faceta, FacetaObjetoConocimiento.NombreFaceta, FacetaObjetoConocimiento.Orden, FacetaObjetoConocimiento.Autocompletar, FacetaObjetoConocimiento.TipoPropiedad,FacetaObjetoConocimiento.TipoDisenio, FacetaObjetoConocimiento.ElementosVisibles, FacetaObjetoConocimiento.AlgoritmoTransformacion, FacetaObjetoConocimiento.EsSemantica, FacetaObjetoConocimiento.Mayusculas, FacetaObjetoConocimiento.EsPorDefecto, FacetaObjetoConocimiento.ComportamientoOr FROM FacetaObjetoConocimiento";
            this.sqlSelectFacetaObjetoConocimientoProyecto = "SELECT " + IBD.CargarGuid("FacetaObjetoConocimientoProyecto.OrganizacionID") + ", " + IBD.CargarGuid("FacetaObjetoConocimientoProyecto.ProyectoID") + ", FacetaObjetoConocimientoProyecto.ObjetoConocimiento, FacetaObjetoConocimientoProyecto.Faceta, FacetaObjetoConocimientoProyecto.Orden, FacetaObjetoConocimientoProyecto.Autocompletar, FacetaObjetoConocimientoProyecto.TipoPropiedad, FacetaObjetoConocimientoProyecto.Comportamiento, FacetaObjetoConocimientoProyecto.Excluyente, FacetaObjetoConocimientoProyecto.NombreFaceta, FacetaObjetoConocimientoProyecto.TipoDisenio, FacetaObjetoConocimientoProyecto.ElementosVisibles, FacetaObjetoConocimientoProyecto.AlgoritmoTransformacion, FacetaObjetoConocimientoProyecto.EsSemantica, FacetaObjetoConocimientoProyecto.Mayusculas, FacetaObjetoConocimientoProyecto.NivelSemantico, FacetaObjetoConocimientoProyecto.Condicion, FacetaObjetoConocimientoProyecto.FacetaPrivadaParaGrupoEditores, FacetaObjetoConocimientoProyecto.Reciproca, FacetaObjetoConocimientoProyecto.ComportamientoOr, FacetaObjetoConocimientoProyecto.OcultaEnFacetas, FacetaObjetoConocimientoProyecto.OcultaEnFiltros, FacetaObjetoConocimientoProyecto.PriorizarOrdenResultados, FacetaObjetoConocimientoProyecto.Inmutable FROM FacetaObjetoConocimientoProyecto";
            this.sqlSelectFacetaObjetoConocimientoProyectoPestanya = "SELECT " + IBD.CargarGuid("FacetaObjetoConocimientoProyectoPestanya.OrganizacionID") + ", " + IBD.CargarGuid("FacetaObjetoConocimientoProyectoPestanya.ProyectoID") + ", FacetaObjetoConocimientoProyectoPestanya.ObjetoConocimiento, FacetaObjetoConocimientoProyectoPestanya.Faceta, " + IBD.CargarGuid("FacetaObjetoConocimientoProyectoPestanya.PestanyaID") + " FROM FacetaObjetoConocimientoProyectoPestanya";
            this.sqlSelectFacetaFiltroProyecto = "SELECT " + IBD.CargarGuid("FacetaFiltroProyecto.OrganizacionID") + ", " + IBD.CargarGuid("FacetaFiltroProyecto.ProyectoID") + ", FacetaFiltroProyecto.ObjetoConocimiento, FacetaFiltroProyecto.Faceta, FacetaFiltroProyecto.Filtro, FacetaFiltroProyecto.Orden, FacetaFiltroProyecto.Condicion FROM FacetaFiltroProyecto";
            sqlSelectFacetaMultiple = "SELECT " + IBD.CargarGuid("FacetaMultiple.OrganizacionID") + ", " + IBD.CargarGuid("FacetaMultiple.ProyectoID") + ", FacetaMultiple.ObjetoConocimiento, FacetaMultiple.Faceta, FacetaMultiple.Consulta, FacetaMultiple.Filtro, FacetaMultiple.NumeroFacetasObtener, FacetaMultiple.NumeroFacetasDesplegar FROM FacetaMultiple";

            this.sqlSelectFacetaEntidadesExternas = "SELECT " + IBD.CargarGuid("FacetaEntidadesExternas.OrganizacionID") + ", " + IBD.CargarGuid("FacetaEntidadesExternas.ProyectoID") + ", FacetaEntidadesExternas.EntidadID, FacetaEntidadesExternas.Grafo, FacetaEntidadesExternas.EsEntidadSecundaria, FacetaEntidadesExternas.BuscarConRecursividad FROM FacetaEntidadesExternas";
            this.sqlSelectFacetaConfigProyChart = "SELECT " + IBD.CargarGuid("FacetaConfigProyChart.OrganizacionID") + ", " + IBD.CargarGuid("FacetaConfigProyChart.ProyectoID") + ", " + IBD.CargarGuid("FacetaConfigProyChart.ChartID") + ", FacetaConfigProyChart.Nombre, FacetaConfigProyChart.Orden, FacetaConfigProyChart.SelectConsultaVirtuoso, FacetaConfigProyChart.FiltrosConsultaVirtuoso, FacetaConfigProyChart.JSBase, FacetaConfigProyChart.JSBusqueda, FacetaConfigProyChart.Ontologias FROM FacetaConfigProyChart";
            this.sqlSelectFacetaConfigProyMapa = "SELECT " + IBD.CargarGuid("FacetaConfigProyMapa.OrganizacionID") + ", " + IBD.CargarGuid("FacetaConfigProyMapa.ProyectoID") + ", FacetaConfigProyMapa.PropLatitud, FacetaConfigProyMapa.PropLongitud, FacetaConfigProyMapa.PropRuta, FacetaConfigProyMapa.ColorRuta FROM FacetaConfigProyMapa";
            this.sqlSelectFacetaRedireccion = "SELECT FacetaRedireccion.Faceta, FacetaRedireccion.Nombre FROM FacetaRedireccion";
            this.sqlSelectConfiguracionConexionGrafo = "SELECT ConfiguracionConexionGrafo.Grafo, ConfiguracionConexionGrafo.CadenaConexion FROM ConfiguracionConexionGrafo";
            this.sqlSelectFacetaConfigProyRangoFecha = "SELECT " + IBD.CargarGuid("FacetaConfigProyRangoFecha.OrganizacionID") + ", " + IBD.CargarGuid("FacetaConfigProyRangoFecha.ProyectoID") + ", FacetaConfigProyRangoFecha.PropiedadNueva, FacetaConfigProyRangoFecha.PropiedadInicio, FacetaConfigProyRangoFecha.PropiedadFin FROM FacetaConfigProyRangoFecha";

            #endregion

            #region DataAdapter

            #region FacetaHome
            this.sqlFacetaHomeInsert = IBD.ReplaceParam("INSERT INTO FacetaHome (OrganizacionID, ProyectoID, ObjetoConocimiento, Faceta, PestanyaFaceta, Orden, MostrarVerMas) VALUES (" + IBD.GuidParamColumnaTabla("OrganizacionID") + ", " + IBD.GuidParamColumnaTabla("ProyectoID") + ", @ObjetoConocimiento, @Faceta, @PestanyaFaceta, @Orden, @MostrarVerMas)");
            this.sqlFacetaHomeDelete = IBD.ReplaceParam("DELETE FROM FacetaHome WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ") AND (ObjetoConocimiento = @Original_ObjetoConocimiento) AND (Faceta = @Original_Faceta)");
            this.sqlFacetaHomeModify = IBD.ReplaceParam("UPDATE FacetaHome SET OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + ", ObjetoConocimiento = @ObjetoConocimiento, Faceta = @Faceta, PestanyaFaceta = @PestanyaFaceta, Orden = @Orden, MostrarVerMas = @MostrarVerMas WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ")  AND (ObjetoConocimiento = @Original_ObjetoConocimiento)  AND (Faceta = @Original_Faceta)");
            #endregion

            #region FacetaFiltroHome
            this.sqlFacetaFiltroHomeInsert = IBD.ReplaceParam("INSERT INTO FacetaFiltroHome (OrganizacionID, ProyectoID, ObjetoConocimiento, Faceta, Filtro, Orden) VALUES (" + IBD.GuidParamColumnaTabla("OrganizacionID") + ", " + IBD.GuidParamColumnaTabla("ProyectoID") + ", @ObjetoConocimiento, @Faceta, @Filtro, @Orden)");
            this.sqlFacetaFiltroHomeDelete = IBD.ReplaceParam("DELETE FROM FacetaFiltroHome WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ") AND (ObjetoConocimiento = @Original_ObjetoConocimiento) AND (Faceta = @Original_Faceta) AND (Filtro = @Original_Filtro) AND (Orden = @Original_Orden)");
            this.sqlFacetaFiltroHomeModify = IBD.ReplaceParam("UPDATE FacetaFiltroHome SET OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + ", ObjetoConocimiento = @ObjetoConocimiento, Faceta = @Faceta, Filtro = @Filtro, Orden = @Orden WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ")  AND (ObjetoConocimiento = @Original_ObjetoConocimiento) AND (Faceta = @Original_Faceta) AND (Filtro = @Original_Filtro) AND (Orden = @Original_Orden)");
            #endregion

            #region FacetaExcluida
            this.sqlFacetaExcluidaInsert = IBD.ReplaceParam("INSERT INTO FacetaExcluida (OrganizacionID, ProyectoID, Faceta) VALUES (" + IBD.GuidParamColumnaTabla("OrganizacionID") + ", " + IBD.GuidParamColumnaTabla("ProyectoID") + ", @Faceta)");
            this.sqlFacetaExcluidaDelete = IBD.ReplaceParam("DELETE FROM FacetaExcluida WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ") AND (Faceta = @Original_Faceta)");
            this.sqlFacetaExcluidaModify = IBD.ReplaceParam("UPDATE FacetaExcluida SET OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + ", Faceta = @Faceta WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ") AND (Faceta = @Original_Faceta)");
            #endregion

            #region OntologiaProyecto
            this.sqlOntologiaProyectoInsert = IBD.ReplaceParam("INSERT INTO OntologiaProyecto (OrganizacionID, ProyectoID, OntologiaProyecto, NombreOnt, Namespace, NamespacesExtra, SubTipos, NombreCortoOnt, CachearDatosSemanticos, EsBuscable) VALUES (" + IBD.GuidParamColumnaTabla("OrganizacionID") + ", " + IBD.GuidParamColumnaTabla("ProyectoID") + ", @OntologiaProyecto, @NombreOnt, @Namespace, @NamespacesExtra, @SubTipos, @NombreCortoOnt, @CachearDatosSemanticos, @EsBuscable)");
            this.sqlOntologiaProyectoDelete = IBD.ReplaceParam("DELETE FROM OntologiaProyecto WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ") AND (OntologiaProyecto = @Original_OntologiaProyecto)");
            this.sqlOntologiaProyectoModify = IBD.ReplaceParam("UPDATE OntologiaProyecto SET OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + ", OntologiaProyecto = @OntologiaProyecto, NombreOnt = @NombreOnt, Namespace = @Namespace, NamespacesExtra = @NamespacesExtra, SubTipos = @SubTipos, NombreCortoOnt = @NombreCortoOnt, CachearDatosSemanticos = @CachearDatosSemanticos, EsBuscable = @EsBuscable WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ") AND (OntologiaProyecto = @Original_OntologiaProyecto)");
            #endregion

            #region FacetaObjetoConocimiento

            this.sqlFacetaObjetoConocimientoInsert = IBD.ReplaceParam("INSERT INTO FacetaObjetoConocimiento (ObjetoConocimiento, Faceta, NombreFaceta, Orden, Autocompletar, TipoPropiedad,TipoDisenio,ElementosVisibles,AlgoritmoTransformacion,EsSemantica,Mayusculas,EsPorDefecto,ComportamientoOr) VALUES (@ObjetoConocimiento, @Faceta, @NombreFaceta, @Orden, @Autocompletar, @TipoPropiedad,@TipoDisenio,@ElementosVisibles,@AlgoritmoTransformacion,@EsSemantica,@Mayusculas,@EsPorDefecto,@ComportamientoOr)");
            this.sqlFacetaObjetoConocimientoDelete = IBD.ReplaceParam("DELETE FROM FacetaObjetoConocimiento WHERE (ObjetoConocimiento = @Original_ObjetoConocimiento) AND (Faceta = @Original_Faceta) ");
            this.sqlFacetaObjetoConocimientoModify = IBD.ReplaceParam("UPDATE FacetaObjetoConocimiento SET ObjetoConocimiento = @ObjetoConocimiento, Faceta = @Faceta, NombreFaceta = @NombreFaceta, Orden = @Orden, Autocompletar = @Autocompletar, TipoPropiedad = @TipoPropiedad, TipoDisenio = @TipoDisenio, ElementosVisibles = @ElementosVisibles, AlgoritmoTransformacion = @AlgoritmoTransformacion, EsSemantica = @EsSemantica, Mayusculas = @Mayusculas, EsPorDefecto = @EsPorDefecto, ComportamientoOr = @ComportamientoOr WHERE (ObjetoConocimiento = @Original_ObjetoConocimiento) AND (Faceta = @Original_Faceta) ");
            #endregion

            #region FacetaObjetoConocimientoProyecto
            this.sqlFacetaObjetoConocimientoProyectoInsert = IBD.ReplaceParam("INSERT INTO FacetaObjetoConocimientoProyecto (OrganizacionID, ProyectoID, ObjetoConocimiento, Faceta, Orden, Autocompletar, TipoPropiedad, Comportamiento, Excluyente, NombreFaceta, TipoDisenio, ElementosVisibles, AlgoritmoTransformacion, EsSemantica, Mayusculas, NivelSemantico, Condicion, FacetaPrivadaParaGrupoEditores, Reciproca,ComportamientoOr,OcultaEnFacetas,OcultaEnFiltros,PriorizarOrdenResultados,Inmutable,AgrupacionID) VALUES (" + IBD.GuidParamColumnaTabla("OrganizacionID") + ", " + IBD.GuidParamColumnaTabla("ProyectoID") + ", @ObjetoConocimiento, @Faceta, @Orden, @Autocompletar, @TipoPropiedad, @Comportamiento, @Excluyente, @NombreFaceta, @TipoDisenio, @ElementosVisibles, @AlgoritmoTransformacion, @EsSemantica, @Mayusculas, @NivelSemantico, @Condicion, @FacetaPrivadaParaGrupoEditores, @Reciproca, @ComportamientoOr, @OcultaEnFacetas, @OcultaEnFiltros, @PriorizarOrdenResultados, @Inmutable,@AgrupacionID)");
            this.sqlFacetaObjetoConocimientoProyectoDelete = IBD.ReplaceParam("DELETE FROM FacetaObjetoConocimientoProyecto WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ") AND (ObjetoConocimiento = @Original_ObjetoConocimiento) AND (Faceta = @Original_Faceta)");
            this.sqlFacetaObjetoConocimientoProyectoModify = IBD.ReplaceParam("UPDATE FacetaObjetoConocimientoProyecto SET OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + ", ObjetoConocimiento = @ObjetoConocimiento, Faceta = @Faceta, Orden = @Orden, Autocompletar = @Autocompletar, TipoPropiedad = @TipoPropiedad, Comportamiento = @Comportamiento, Excluyente = @Excluyente, NombreFaceta = @NombreFaceta, TipoDisenio = @TipoDisenio, ElementosVisibles = @ElementosVisibles, AlgoritmoTransformacion = @AlgoritmoTransformacion, EsSemantica = @EsSemantica, Mayusculas = @Mayusculas, NivelSemantico = @NivelSemantico, Condicion = @Condicion, FacetaPrivadaParaGrupoEditores = @FacetaPrivadaParaGrupoEditores, Reciproca = @Reciproca, ComportamientoOr = @ComportamientoOr, OcultaEnFacetas = @OcultaEnFacetas, OcultaEnFiltros = @OcultaEnFiltros, PriorizarOrdenResultados=@PriorizarOrdenResultados , Inmutable=@Inmutable, AgrupacionID=@AgrupacionID WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ") AND (ObjetoConocimiento = @Original_ObjetoConocimiento) AND (Faceta = @Original_Faceta)");
            #endregion

            #region FacetaObjetoConocimientoProyectoPestanya
            this.sqlFacetaObjetoConocimientoProyectoPestanyaInsert = IBD.ReplaceParam("INSERT INTO FacetaObjetoConocimientoProyectoPestanya (OrganizacionID, ProyectoID, ObjetoConocimiento, Faceta, PestanyaID) VALUES (" + IBD.GuidParamColumnaTabla("OrganizacionID") + ", " + IBD.GuidParamColumnaTabla("ProyectoID") + ", @ObjetoConocimiento, @Faceta, " + IBD.GuidParamColumnaTabla("PestanyaID") + ")");
            this.sqlFacetaObjetoConocimientoProyectoPestanyaDelete = IBD.ReplaceParam("DELETE FROM FacetaObjetoConocimientoProyectoPestanya WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ") AND (ObjetoConocimiento = @Original_ObjetoConocimiento) AND (Faceta = @Original_Faceta) AND (PestanyaID = " + IBD.GuidParamColumnaTabla("Original_PestanyaID") + ")");
            this.sqlFacetaObjetoConocimientoProyectoPestanyaModify = IBD.ReplaceParam("UPDATE FacetaObjetoConocimientoProyectoPestanya SET OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + ", ObjetoConocimiento = @ObjetoConocimiento, Faceta = @Faceta, PestanyaID = " + IBD.GuidParamColumnaTabla("PestanyaID") + " WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ") AND (ObjetoConocimiento = @Original_ObjetoConocimiento) AND (Faceta = @Original_Faceta) AND (PestanyaID = " + IBD.GuidParamColumnaTabla("Original_PestanyaID") + ")");
            #endregion

            #region FacetaFiltroProyecto
            this.sqlFacetaFiltroProyectoInsert = IBD.ReplaceParam("INSERT INTO FacetaFiltroProyecto (OrganizacionID, ProyectoID, ObjetoConocimiento, Faceta, Filtro, Orden, Condicion) VALUES (" + IBD.GuidParamColumnaTabla("OrganizacionID") + ", " + IBD.GuidParamColumnaTabla("ProyectoID") + ", @ObjetoConocimiento, @Faceta, @Filtro, @Orden, @Condicion)");
            this.sqlFacetaFiltroProyectoDelete = IBD.ReplaceParam("DELETE FROM FacetaFiltroProyecto WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ") AND (ObjetoConocimiento = @Original_ObjetoConocimiento) AND (Faceta = @Original_Faceta) AND (Filtro = @Original_Filtro) AND (Orden = @Original_Orden)");
            this.sqlFacetaFiltroProyectoModify = IBD.ReplaceParam("UPDATE FacetaFiltroProyecto SET OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + ", ObjetoConocimiento = @ObjetoConocimiento, Faceta = @Faceta, Filtro = @Filtro, Orden = @Orden, Condicion = @Condicion WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ") AND (ObjetoConocimiento = @Original_ObjetoConocimiento) AND (Faceta = @Original_Faceta) AND (Filtro = @Original_Filtro) AND (Orden = @Original_Orden)");
            #endregion

            #region FacetaEntidadesExternas
            this.sqlFacetaEntidadesExternasInsert = IBD.ReplaceParam("INSERT INTO FacetaEntidadesExternas (OrganizacionID, ProyectoID, EntidadID, Grafo, EsEntidadSecundaria, BuscarConRecursividad) VALUES (" + IBD.GuidParamColumnaTabla("OrganizacionID") + ", " + IBD.GuidParamColumnaTabla("ProyectoID") + ", @EntidadID, @Grafo, @EsEntidadSecundaria, @BuscarConRecursividad)");
            this.sqlFacetaEntidadesExternasDelete = IBD.ReplaceParam("DELETE FROM FacetaEntidadesExternas WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ") AND (EntidadID = @Original_EntidadID) AND (Grafo = @Original_Grafo)");
            this.sqlFacetaEntidadesExternasModify = IBD.ReplaceParam("UPDATE FacetaEntidadesExternas SET OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + ", EntidadID = @EntidadID, Grafo = @Grafo, EsEntidadSecundaria = @EsEntidadSecundaria, BuscarConRecursividad = @BuscarConRecursividad WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ") AND (EntidadID = @Original_EntidadID) AND (Grafo = @Original_Grafo)");
            #endregion

            #region FacetaConfigProyChart
            this.sqlFacetaConfigProyChartInsert = IBD.ReplaceParam("INSERT INTO FacetaConfigProyChart (OrganizacionID, ProyectoID, ChartID, Nombre, Orden, SelectConsultaVirtuoso, FiltrosConsultaVirtuoso, JSBase, JSBusqueda, Ontologias) VALUES (" + IBD.GuidParamColumnaTabla("OrganizacionID") + ", " + IBD.GuidParamColumnaTabla("ProyectoID") + ", " + IBD.GuidParamColumnaTabla("ChartID") + ", @Nombre, @Orden, @SelectConsultaVirtuoso, @FiltrosConsultaVirtuoso, @JSBase, @JSBusqueda, @Ontologias)");
            this.sqlFacetaConfigProyChartDelete = IBD.ReplaceParam("DELETE FROM FacetaConfigProyChart WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ") AND (ChartID = " + IBD.GuidParamColumnaTabla("Original_ChartID") + ")");
            this.sqlFacetaConfigProyChartModify = IBD.ReplaceParam("UPDATE FacetaConfigProyChart SET OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + ", ChartID = " + IBD.GuidParamColumnaTabla("ChartID") + ", Nombre = @Nombre, Orden = @Orden, SelectConsultaVirtuoso = @SelectConsultaVirtuoso, FiltrosConsultaVirtuoso = @FiltrosConsultaVirtuoso, JSBase = @JSBase, JSBusqueda = @JSBusqueda, Ontologias = @Ontologias WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ") AND (ChartID = " + IBD.GuidParamColumnaTabla("Original_ChartID") + ")");
            #endregion

            #region FacetaConfigProyMapa
            this.sqlFacetaConfigProyMapaInsert = IBD.ReplaceParam("INSERT INTO FacetaConfigProyMapa (OrganizacionID, ProyectoID, PropLatitud, PropLongitud, PropRuta, ColorRuta) VALUES (" + IBD.GuidParamColumnaTabla("OrganizacionID") + ", " + IBD.GuidParamColumnaTabla("ProyectoID") + ", @PropLatitud, @PropLongitud, @PropRuta, @ColorRuta)");
            this.sqlFacetaConfigProyMapaDelete = IBD.ReplaceParam("DELETE FROM FacetaConfigProyMapa WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ") AND (PropLatitud = @Original_PropLatitud) AND (PropLongitud = @Original_PropLongitud)");
            this.sqlFacetaConfigProyMapaModify = IBD.ReplaceParam("UPDATE FacetaConfigProyMapa SET OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + ", PropLatitud = @PropLatitud, PropLongitud = @PropLongitud, PropRuta = @PropRuta, ColorRuta = @ColorRuta WHERE (OrganizacionID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ") AND (PropLatitud = @Original_PropLatitud) AND (PropLongitud = @Original_PropLongitud)");
            #endregion

            #region FacetaRedireccion
            this.sqlFacetaRedireccionInsert = IBD.ReplaceParam("INSERT INTO FacetaRedireccion (Faceta, Nombre) VALUES (@Faceta, @Nombre)");
            this.sqlFacetaRedireccionDelete = IBD.ReplaceParam("DELETE FROM FacetaRedireccion WHERE (Faceta = @Original_Faceta) AND (Nombre = @Original_Nombre)");
            this.sqlFacetaRedireccionModify = IBD.ReplaceParam("UPDATE FacetaRedireccion SET Faceta = @Faceta, Nombre = @Nombre WHERE (Faceta = @Original_Faceta) AND (Nombre = @Original_Nombre)");
            #endregion

            #region ConfiguracionConexionGrafo
            this.sqlConfiguracionConexionGrafoInsert = IBD.ReplaceParam("INSERT INTO ConfiguracionConexionGrafo (Grafo, CadenaConexion) VALUES (@Grafo, @CadenaConexion)");
            this.sqlConfiguracionConexionGrafoDelete = IBD.ReplaceParam("DELETE FROM ConfiguracionConexionGrafo WHERE (Grafo = @Original_Grafo) AND (CadenaConexion = @Original_CadenaConexion)");
            this.sqlConfiguracionConexionGrafoModify = IBD.ReplaceParam("UPDATE ConfiguracionConexionGrafo SET Grafo = @Grafo, CadenaConexion = @CadenaConexion WHERE (Grafo = @Original_Grafo) AND (CadenaConexion = @Original_CadenaConexion)");
            #endregion

            #endregion

        }

        #endregion

        #region Métodos generales

        /// <summary>
        /// En caso de que se utilice el GnossConfig.xml por defecto se sigue utilizando el IBD estático
        /// </summary>
        private void CargarConsultasYDataAdapters()
        {
            this.CargarConsultasYDataAdapters(IBD);
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
            if (pSoloBuscables)
            {
                if (!pOrganizacionID.Equals(Guid.Empty))
                {
                    query = query.Where(item => item.EsBuscable.Equals(true));
                }
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
            List<OntologiaProyecto> listaOntologiaProyecto = new List<OntologiaProyecto>();
            var consulta = mEntityContext.OntologiaProyecto.Where(item => item.ProyectoID.Equals(pProyectoID));
            if (!pOrganizacionID.Equals(Guid.Empty))
            {
                consulta = consulta.Where(item => item.OrganizacionID.Equals(pOrganizacionID));

            }
            if (pSoloBuscables)
            {
                if (!pOrganizacionID.Equals(Guid.Empty))
                {
                    consulta = consulta.Where(item => item.EsBuscable == true);
                }
            }

            listaOntologiaProyecto = consulta.ToList();
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

        private void AgregarNamespacesComoOntologias(List<OntologiaProyecto> pListaFaceta)
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

                        // FacetaDS.OntologiaProyectoRow filaNueva = pListaFaceta.OntologiaProyecto.NewOntologiaProyectoRow();
                        OntologiaProyecto filaNueva = new OntologiaProyecto();

                        filaNueva.OrganizacionID = fila.OrganizacionID;
                        filaNueva.ProyectoID = fila.ProyectoID;
                        filaNueva.OntologiaProyecto1 = ontologiaProyecto;
                        filaNueva.NombreOnt = fila.NombreOnt;
                        filaNueva.Namespace = key;
                        filaNueva.NamespacesExtra = "";
                        filaNueva.EsBuscable = true;
                        filaNueva.CachearDatosSemanticos = true;

                        //EntityContext.OntologiaProyecto.Add(filaNueva);

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
                    //EntityContext.OntologiaProyecto.Add(filaExtra);
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
            List<string> listaPredicados = new List<string>();

            listaPredicados = mEntityContext.FacetaObjetoConocimientoProyecto.Where(item => item.Autocompletar.Equals(true) && item.OrganizacionID.Equals(pOrganizacionID) && item.ProyectoID.Equals(pProyectoID)).Select(item => item.Faceta).Distinct().ToList();

            return listaPredicados;
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
                string valorNamespaceado = FacetaAD.ObtenerValorAplicandoNamespaces(pValor, filaOnto, pUsarNamespaceSiempre);
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
                        return names + ":" + pValor.Replace(url, "");
                    }
                }
            }
            else if (pValor.Contains(":"))
            {
                return pValor;
            }
            else if (pUsarNamespaceSiempre)
            {
                return pFilaOntoNamespaces.OntologiaProyecto1 + ":" + pValor;
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
                string valorNamespaceado = FacetaAD.ObtenerValorAplicandoNamespaces(nombre, filaOnto, true);
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
            List<string> facetasAfectanOrden = pFacetaDW.ListaFacetaObjetoConocimientoProyecto.Where(f => f.PriorizarOrdenResultados).Select(f => f.Faceta).ToList();

            foreach (string facetaAfectaOrden in facetasAfectanOrden)
            {
                if (pListaFiltros.ContainsKey(facetaAfectaOrden))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion Métodos generales
    }
}
