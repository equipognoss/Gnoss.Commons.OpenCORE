using Es.Riam.Gnoss.AD.BASE_BD.Model;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Data.Common;

namespace Es.Riam.Gnoss.AD.BASE_BD
{

    /// <summary>
    /// DataAdapter de BaseSuscripcionesAD
    /// </summary>
    public class BaseSuscripcionesAD : BaseComunidadAD
    {

        #region Constantes


        /// <summary>
        /// Constante para codificar el origen del ID de la Suscripcion
        /// </summary>
        public const string ID_SUSCRIPCION = "##ID_SUSCRIPCION##";


        /// <summary>
        /// Constante para codificar el origen del recurso al que se le asocia la Suscripcion
        /// </summary>
        public const string ID_SUSCRIPCION_RECURSO = "##ID_SUSCRIPCION_RECURSO##";

        /// <summary>
        /// Constante para codificar el origen del ID destino de la Suscripcion
        /// </summary>
        public const string ID_SUSCRIPCION_PERFIL = "##ID_SUSCRIPCION_PERFIL##";

        #endregion

        #region Constructores

        /// <summary>
        /// Cuando se desea pasar directamente la ruta del fichero de configuracion de conexion a la Base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Fichero de configuración de la base de datos BASE</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public BaseSuscripcionesAD(LoggingService loggingService, EntityContext entityContext, EntityContextBASE entityContextBASE, ConfigService configService)
            : base(-1, loggingService, entityContext, entityContextBASE, configService)
        {
            this.CargarConsultasYDataAdapters(IBD);
        }

        #endregion

        #region Consultas
        private string sqlSelectColaTagsSuscripcion;

        #endregion

        #region DataAdapter
        #region ColaTagsSuscripciones
        private string sqlColaTagsSuscripcionInsert;
        private string sqlColaTagsSuscripcionDelete;
        private string sqlColaTagsSuscripcionModify;
        #endregion



        #endregion

        #region Métodos generales

        #region Metodos AD

        /// <summary>
        /// Elimina los elementos borrados del DataSet
        /// </summary>
        /// <param name="pDataSet">DataSet de eliminados</param>
        public override void EliminarBorrados(DataSet pDataSet)
        {
            try
            {

                DataSet deletedDataSet;
                deletedDataSet = pDataSet.GetChanges(DataRowState.Deleted);
                if (deletedDataSet != null)
                {


                    #region Deleted



                    #region Eliminar tabla ColaTagsSuscripcion
                    DbCommand DeleteColaTagsSuscripcionCommand = ObtenerComando(sqlColaTagsSuscripcionDelete);
                    AgregarParametro(DeleteColaTagsSuscripcionCommand, IBD.ToParam("Original_TablaBaseProyectoID"), DbType.Int32, "TablaBaseProyectoID", DataRowVersion.Original);
                    AgregarParametro(DeleteColaTagsSuscripcionCommand, IBD.ToParam("Original_Tags"), DbType.String, "Tags", DataRowVersion.Original);
                    AgregarParametro(DeleteColaTagsSuscripcionCommand, IBD.ToParam("Original_Tipo"), DbType.Int16, "Tipo", DataRowVersion.Original);
                    AgregarParametro(DeleteColaTagsSuscripcionCommand, IBD.ToParam("Original_Estado"), DbType.Int16, "Estado", DataRowVersion.Original);
                    AgregarParametro(DeleteColaTagsSuscripcionCommand, IBD.ToParam("Original_FechaPuestaEnCola"), DbType.DateTime, "FechaPuestaEnCola", DataRowVersion.Original);
                    AgregarParametro(DeleteColaTagsSuscripcionCommand, IBD.ToParam("Original_FechaProcesado"), DbType.DateTime, "FechaProcesado", DataRowVersion.Original);
                    AgregarParametro(DeleteColaTagsSuscripcionCommand, IBD.ToParam("Original_Prioridad"), DbType.Int16, "Prioridad", DataRowVersion.Original);
                    ActualizarBaseDeDatos(deletedDataSet, "ColaTagsSuscripciones", null, null, DeleteColaTagsSuscripcionCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

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
        /// Guarda los cambios realizado en el DataSet
        /// </summary>
        /// <param name="pDataSet">DataSet con cambios</param>
        public override void GuardarActualizaciones(DataSet pDataSet)
        {
            try
            {
                DataSet addedAndModifiedDataSet;
                addedAndModifiedDataSet = pDataSet.GetChanges(DataRowState.Added | DataRowState.Modified);
                if (addedAndModifiedDataSet != null)
                {

                    #region AddedAndModified
                    #region Actualizar tabla ColaTagsSuscripciones
                    DbCommand InsertColaTagsSuscripcionCommand = ObtenerComando(sqlColaTagsSuscripcionInsert);
                    AgregarParametro(InsertColaTagsSuscripcionCommand, IBD.ToParam("TablaBaseProyectoID"), DbType.Int32, "TablaBaseProyectoID", DataRowVersion.Current);
                    AgregarParametro(InsertColaTagsSuscripcionCommand, IBD.ToParam("Tags"), DbType.String, "Tags", DataRowVersion.Current);
                    AgregarParametro(InsertColaTagsSuscripcionCommand, IBD.ToParam("Tipo"), DbType.Int16, "Tipo", DataRowVersion.Current);
                    AgregarParametro(InsertColaTagsSuscripcionCommand, IBD.ToParam("Estado"), DbType.Int16, "Estado", DataRowVersion.Current);
                    AgregarParametro(InsertColaTagsSuscripcionCommand, IBD.ToParam("FechaPuestaEnCola"), DbType.DateTime, "FechaPuestaEnCola", DataRowVersion.Current);
                    AgregarParametro(InsertColaTagsSuscripcionCommand, IBD.ToParam("FechaProcesado"), DbType.DateTime, "FechaProcesado", DataRowVersion.Current);
                    AgregarParametro(InsertColaTagsSuscripcionCommand, IBD.ToParam("Prioridad"), DbType.Int16, "Prioridad", DataRowVersion.Current);

                    DbCommand ModifyColaTagsSuscripcionCommand = ObtenerComando(sqlColaTagsSuscripcionModify);
                    AgregarParametro(ModifyColaTagsSuscripcionCommand, IBD.ToParam("Original_TablaBaseProyectoID"), DbType.Int32, "TablaBaseProyectoID", DataRowVersion.Original);
                    AgregarParametro(ModifyColaTagsSuscripcionCommand, IBD.ToParam("Original_Tags"), DbType.String, "Tags", DataRowVersion.Original);
                    AgregarParametro(ModifyColaTagsSuscripcionCommand, IBD.ToParam("Original_Tipo"), DbType.Int16, "Tipo", DataRowVersion.Original);
                    AgregarParametro(ModifyColaTagsSuscripcionCommand, IBD.ToParam("Original_Estado"), DbType.Int16, "Estado", DataRowVersion.Original);
                    AgregarParametro(ModifyColaTagsSuscripcionCommand, IBD.ToParam("Original_FechaPuestaEnCola"), DbType.DateTime, "FechaPuestaEnCola", DataRowVersion.Original);
                    AgregarParametro(ModifyColaTagsSuscripcionCommand, IBD.ToParam("Original_FechaProcesado"), DbType.DateTime, "FechaProcesado", DataRowVersion.Original);
                    AgregarParametro(ModifyColaTagsSuscripcionCommand, IBD.ToParam("Original_Prioridad"), DbType.Int16, "Prioridad", DataRowVersion.Original);

                    AgregarParametro(ModifyColaTagsSuscripcionCommand, IBD.ToParam("TablaBaseProyectoID"), DbType.Int32, "TablaBaseProyectoID", DataRowVersion.Current);
                    AgregarParametro(ModifyColaTagsSuscripcionCommand, IBD.ToParam("Tags"), DbType.String, "Tags", DataRowVersion.Current);
                    AgregarParametro(ModifyColaTagsSuscripcionCommand, IBD.ToParam("Tipo"), DbType.Int16, "Tipo", DataRowVersion.Current);
                    AgregarParametro(ModifyColaTagsSuscripcionCommand, IBD.ToParam("Estado"), DbType.Int16, "Estado", DataRowVersion.Current);
                    AgregarParametro(ModifyColaTagsSuscripcionCommand, IBD.ToParam("FechaPuestaEnCola"), DbType.DateTime, "FechaPuestaEnCola", DataRowVersion.Current);
                    AgregarParametro(ModifyColaTagsSuscripcionCommand, IBD.ToParam("FechaProcesado"), DbType.DateTime, "FechaProcesado", DataRowVersion.Current);
                    AgregarParametro(ModifyColaTagsSuscripcionCommand, IBD.ToParam("Prioridad"), DbType.Int16, "Prioridad", DataRowVersion.Current);
                    ActualizarBaseDeDatos(addedAndModifiedDataSet, "ColaTagsSuscripciones", InsertColaTagsSuscripcionCommand, ModifyColaTagsSuscripcionCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

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
        /// En caso de que se utilice el GnossConfig.xml por defecto se sigue utilizando el IBD estático
        /// </summary>
        private void CargarConsultasYDataAdapters()
        {
            this.CargarConsultasYDataAdapters(IBD);
        }

        /// <summary>
        /// En caso de que se utilice un GnossConfig.xml que no es el de por defecto se pasa un objeto IBaseDatos creado con respecto
        /// al fichero de configuracion que se ha apsado como parámetro
        /// </summary>
        /// <param name="IBD">Objecto IBaseDatos para el archivo pasado al constructor del AD</param>
        private void CargarConsultasYDataAdapters(IBaseDatos IBD)
        {

            #region Consultas
            this.sqlSelectColaTagsSuscripcion = "SELECT ColaTagsSuscripciones.OrdenEjecucion, ColaTagsSuscripciones.TablaBaseProyectoID, ColaTagsSuscripciones.Tags, ColaTagsSuscripciones.Tipo, ColaTagsSuscripciones.Estado, ColaTagsSuscripciones.FechaPuestaEnCola, ColaTagsSuscripciones.FechaProcesado, ColaTagsSuscripciones.Prioridad FROM ColaTagsSuscripciones";

            #endregion

            #region DataAdapter
            #region ColaTagsSuscripcion
            this.sqlColaTagsSuscripcionInsert = IBD.ReplaceParam("INSERT INTO ColaTagsSuscripciones (TablaBaseProyectoID, Tags, Tipo, Estado, FechaPuestaEnCola, FechaProcesado,Prioridad) VALUES (@TablaBaseProyectoID, @Tags, @Tipo, @Estado, @FechaPuestaEnCola, @FechaProcesado,@Prioridad)");
            this.sqlColaTagsSuscripcionDelete = IBD.ReplaceParam("DELETE FROM ColaTagsSuscripciones WHERE (TablaBaseProyectoID = @Original_TablaBaseProyectoID) AND (Tags = @Original_Tags) AND (Tipo = @Original_Tipo) AND (Estado = @Original_Estado) AND (FechaPuestaEnCola = @Original_FechaPuestaEnCola) AND (FechaProcesado = @Original_FechaProcesado OR @Original_FechaProcesado IS NULL AND FechaProcesado IS NULL)");
            this.sqlColaTagsSuscripcionModify = IBD.ReplaceParam("UPDATE ColaTagsSuscripciones SET TablaBaseProyectoID = @TablaBaseProyectoID, Tags = @Tags, Tipo = @Tipo, Estado = @Estado, FechaPuestaEnCola = @FechaPuestaEnCola, FechaProcesado = @FechaProcesado,Prioridad=@Prioridad WHERE (TablaBaseProyectoID = @Original_TablaBaseProyectoID) AND (Tags = @Original_Tags) AND (Tipo = @Original_Tipo) AND (Estado = @Original_Estado) AND (FechaPuestaEnCola = @Original_FechaPuestaEnCola) AND (FechaProcesado = @Original_FechaProcesado OR @Original_FechaProcesado IS NULL AND FechaProcesado IS NULL)");
            #endregion



            #endregion

        }

        #endregion

        #region Metodos de colas

        /// <summary>
        /// Obtiene los elementos pendientes de la cola de tags
        /// </summary>
        /// <returns>Devuelve un dataset con los elementos de la cola cargados</returns>
        /// <param name="pEstadoInferior">Estado mínimo de los elementos de la cola</param>
        /// <param name="pEstadoSuperior">Estado máximo de los elementos de la cola</param>
        /// <param name="pTiposElementos">Tipo de los elementos a obtener (Agregado, eliminado, ...). Null para obtener todos</param>
        /// <param name="pNumMaxItems">Numero máximo de items a traer</param>
        public override DataSet ObtenerElementosColaPendientes(EstadosColaTags pEstadoInferior, EstadosColaTags pEstadoSuperior, TiposElementosEnCola? pTiposElementos, int pNumMaxItems)
        {
            return ObtenerElementosColaPendientes(pEstadoInferior, pEstadoSuperior, pTiposElementos, pNumMaxItems, null);
        }

        /// <summary>
        /// Obtiene los elementos pendientes de la cola de tags
        /// </summary>
        /// <returns>Devuelve un dataset con los elementos de la cola cargados</returns>
        /// <param name="pEstadoInferior">Estado mínimo de los elementos de la cola</param>
        /// <param name="pEstadoSuperior">Estado máximo de los elementos de la cola</param>
        /// <param name="pTiposElementos">Tipo de los elementos a obtener (Agregado, eliminado, ...). Null para obtener todos</param>
        /// <param name="pNumMaxItems">Numero máximo de items a traer</param>
        public override DataSet ObtenerElementosColaPendientes(EstadosColaTags pEstadoInferior, EstadosColaTags pEstadoSuperior, TiposElementosEnCola? pTiposElementos, int pNumMaxItems, bool? pSoloPrioridad0)
        {
            BaseSuscripcionesDS brSuscripcionesComDS = new BaseSuscripcionesDS();

            string andTipoElemento = "";

            if (pTiposElementos.HasValue)
            {
                andTipoElemento = " AND Tipo = " + (short)pTiposElementos.Value;
            }

            string andPrioridad = "";
            if (pSoloPrioridad0.HasValue)
            {
                if (pSoloPrioridad0.Value)
                {
                    andPrioridad = " AND Prioridad = 0 ";
                }
                else
                {
                    andPrioridad = " AND Prioridad > 0 ";
                }
            }

            string consultaProyectosCom = sqlSelectColaTagsSuscripcion.Replace("SELECT ", "SELECT top(" + pNumMaxItems + ") ") + " WHERE Estado >= " + (short)pEstadoInferior + " AND Estado <= " + (short)pEstadoSuperior + andTipoElemento + andPrioridad + " ORDER BY Prioridad ASC, OrdenEjecucion ASC";

            DbCommand cmdObtnerElementosColaPendientesProyectosCom = ObtenerComando(consultaProyectosCom);
            CargarDataSet(cmdObtnerElementosColaPendientesProyectosCom, brSuscripcionesComDS, "ColaTagsSuscripciones");

            return brSuscripcionesComDS;
        }

        #endregion

        #endregion
        /// <summary>
        /// Obtiene la conexión a la base de datos Master
        /// </summary>
        protected override DbConnection ConexionMaster
        {
            get
            {
                var conexion = mEntityContextBASE.Database.GetDbConnection();
                if (conexion.State != ConnectionState.Open)
                {
                    conexion.Open();
                }
                return conexion;
            }
        }
    }
}
