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
    /// DataAdapter de BaseProyectosDS
    /// </summary>
    public class BaseProyectosAD : BaseComunidadAD
    {
        #region Constantes


        /// <summary>
        /// Constante para codificar el origen del ID si es proyecto o usuario
        /// </summary>
        public const string COM_O_US = "##COM-US##";

        /// <summary>
        /// Clave para el tag de nombre de proyecto
        /// </summary>
        public const string NOMBRE_PROY = "##NOM_PROY##";

        /// <summary>
        /// Clave para el tag de Categoría de proyecto
        /// </summary>
        public const string CAT_PROY = "##CAT_PROY##";

        /// <summary>
        ///Constante para codificar los tags de tipo ID-Tag de proyecto
        /// </summary>
        public const string ID_TAG_PROY = "##ID_TAG_PROY##";

        #endregion

        #region Miembros

        /// <summary>
        /// Nombre real de la tabla que deberemos consultar para los tags de los blogs de una coomunidad en funcion del identificador numerico del proyecto
        /// </summary>
        private string mNombreTablaPROYECTOS = "PROYECTOS";

        #endregion

        #region Constructores

        /// <summary>
        /// Cuando se desea pasar directamente la ruta del fichero de configuracion de conexion a la Base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Fichero de configuración de la base de datos BASE</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public BaseProyectosAD(string pFicheroConfiguracionBD, LoggingService loggingService, EntityContext entityContext, EntityContextBASE entityContextBASE, ConfigService configService)
            : base(-1, loggingService, entityContext, entityContextBASE, configService)
        {
            this.CargarConsultasYDataAdapters(IBD);
        }

        #endregion

        #region Consultas
        private string sqlSelectColaTagsProyectos;
        private string sqlSelectPROYECTOS;
        #endregion

        #region DataAdapter

        #region ColaTagsProyectos
        private string sqlColaTagsProyectoInsert;
        private string sqlColaTagsProyectoDelete;
        private string sqlColaTagsProyectoModify;
        #endregion

        #region PROYECTOS
        private string sqlPROYECTOSInsert;
        private string sqlPROYECTOSDelete;
        private string sqlPROYECTOSModify;
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

                    #region Eliminar tabla PROYECTOS
                    DbCommand DeletePROYECTOSCommand = ObtenerComando(sqlPROYECTOSDelete);
                    AgregarParametro(DeletePROYECTOSCommand, IBD.ToParam("Original_Tag1"), DbType.String, "Tag1", DataRowVersion.Original);
                    AgregarParametro(DeletePROYECTOSCommand, IBD.ToParam("Original_Tag2"), DbType.String, "Tag2", DataRowVersion.Original);
                    AgregarParametro(DeletePROYECTOSCommand, IBD.ToParam("Original_Tipo"), DbType.Int16, "Tipo", DataRowVersion.Original);
                    AgregarParametro(DeletePROYECTOSCommand, IBD.ToParam("Original_CercaniaDirecta"), DbType.Int32, "CercaniaDirecta", DataRowVersion.Original);
                    AgregarParametro(DeletePROYECTOSCommand, IBD.ToParam("Original_CercaniaIndirecta"), DbType.Int32, "CercaniaIndirecta", DataRowVersion.Original);
                    ActualizarBaseDeDatos(deletedDataSet, "PROYECTOS", null, null, DeletePROYECTOSCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #region Eliminar tabla ColaTagsProyectos
                    DbCommand DeleteColaTagsProyectoCommand = ObtenerComando(sqlColaTagsProyectoDelete);
                    AgregarParametro(DeleteColaTagsProyectoCommand, IBD.ToParam("Original_TablaBaseProyectoID"), DbType.Int32, "TablaBaseProyectoID", DataRowVersion.Original);
                    AgregarParametro(DeleteColaTagsProyectoCommand, IBD.ToParam("Original_Tags"), DbType.String, "Tags", DataRowVersion.Original);
                    AgregarParametro(DeleteColaTagsProyectoCommand, IBD.ToParam("Original_Tipo"), DbType.Int16, "Tipo", DataRowVersion.Original);
                    AgregarParametro(DeleteColaTagsProyectoCommand, IBD.ToParam("Original_Estado"), DbType.Int16, "Estado", DataRowVersion.Original);
                    AgregarParametro(DeleteColaTagsProyectoCommand, IBD.ToParam("Original_FechaPuestaEnCola"), DbType.DateTime, "FechaPuestaEnCola", DataRowVersion.Original);
                    AgregarParametro(DeleteColaTagsProyectoCommand, IBD.ToParam("Original_FechaProcesado"), DbType.DateTime, "FechaProcesado", DataRowVersion.Original);
                    AgregarParametro(DeleteColaTagsProyectoCommand, IBD.ToParam("Original_Prioridad"), DbType.Int16, "Prioridad", DataRowVersion.Original);
                    ActualizarBaseDeDatos(deletedDataSet, "ColaTagsProyectos", null, null, DeleteColaTagsProyectoCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

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
                InsertarFilasEnRabbit("ColaTagsProyectos", pDataSet);

                DataSet addedAndModifiedDataSet;
                addedAndModifiedDataSet = pDataSet.GetChanges(DataRowState.Added | DataRowState.Modified);
                if (addedAndModifiedDataSet != null)
                {

                    #region AddedAndModified
                    #region Actualizar tabla ColaTagsProyectos
                    DbCommand InsertColaTagsProyectoCommand = ObtenerComando(sqlColaTagsProyectoInsert);
                    AgregarParametro(InsertColaTagsProyectoCommand, IBD.ToParam("TablaBaseProyectoID"), DbType.Int32, "TablaBaseProyectoID", DataRowVersion.Current);
                    AgregarParametro(InsertColaTagsProyectoCommand, IBD.ToParam("Tags"), DbType.String, "Tags", DataRowVersion.Current);
                    AgregarParametro(InsertColaTagsProyectoCommand, IBD.ToParam("Tipo"), DbType.Int16, "Tipo", DataRowVersion.Current);
                    AgregarParametro(InsertColaTagsProyectoCommand, IBD.ToParam("Estado"), DbType.Int16, "Estado", DataRowVersion.Current);
                    AgregarParametro(InsertColaTagsProyectoCommand, IBD.ToParam("FechaPuestaEnCola"), DbType.DateTime, "FechaPuestaEnCola", DataRowVersion.Current);
                    AgregarParametro(InsertColaTagsProyectoCommand, IBD.ToParam("FechaProcesado"), DbType.DateTime, "FechaProcesado", DataRowVersion.Current);
                    AgregarParametro(InsertColaTagsProyectoCommand, IBD.ToParam("Prioridad"), DbType.Int16, "Prioridad", DataRowVersion.Current);

                    DbCommand ModifyColaTagsProyectoCommand = ObtenerComando(sqlColaTagsProyectoModify);
                    AgregarParametro(ModifyColaTagsProyectoCommand, IBD.ToParam("Original_TablaBaseProyectoID"), DbType.Int32, "TablaBaseProyectoID", DataRowVersion.Original);
                    AgregarParametro(ModifyColaTagsProyectoCommand, IBD.ToParam("Original_Tags"), DbType.String, "Tags", DataRowVersion.Original);
                    AgregarParametro(ModifyColaTagsProyectoCommand, IBD.ToParam("Original_Tipo"), DbType.Int16, "Tipo", DataRowVersion.Original);
                    AgregarParametro(ModifyColaTagsProyectoCommand, IBD.ToParam("Original_Estado"), DbType.Int16, "Estado", DataRowVersion.Original);
                    AgregarParametro(ModifyColaTagsProyectoCommand, IBD.ToParam("Original_FechaPuestaEnCola"), DbType.DateTime, "FechaPuestaEnCola", DataRowVersion.Original);
                    AgregarParametro(ModifyColaTagsProyectoCommand, IBD.ToParam("Original_FechaProcesado"), DbType.DateTime, "FechaProcesado", DataRowVersion.Original);
                    AgregarParametro(ModifyColaTagsProyectoCommand, IBD.ToParam("Original_Prioridad"), DbType.Int16, "Prioridad", DataRowVersion.Original);

                    AgregarParametro(ModifyColaTagsProyectoCommand, IBD.ToParam("TablaBaseProyectoID"), DbType.Int32, "TablaBaseProyectoID", DataRowVersion.Current);
                    AgregarParametro(ModifyColaTagsProyectoCommand, IBD.ToParam("Tags"), DbType.String, "Tags", DataRowVersion.Current);
                    AgregarParametro(ModifyColaTagsProyectoCommand, IBD.ToParam("Tipo"), DbType.Int16, "Tipo", DataRowVersion.Current);
                    AgregarParametro(ModifyColaTagsProyectoCommand, IBD.ToParam("Estado"), DbType.Int16, "Estado", DataRowVersion.Current);
                    AgregarParametro(ModifyColaTagsProyectoCommand, IBD.ToParam("FechaPuestaEnCola"), DbType.DateTime, "FechaPuestaEnCola", DataRowVersion.Current);
                    AgregarParametro(ModifyColaTagsProyectoCommand, IBD.ToParam("FechaProcesado"), DbType.DateTime, "FechaProcesado", DataRowVersion.Current);
                    AgregarParametro(ModifyColaTagsProyectoCommand, IBD.ToParam("Prioridad"), DbType.Int16, "Prioridad", DataRowVersion.Current);
                    ActualizarBaseDeDatos(addedAndModifiedDataSet, "ColaTagsProyectos", InsertColaTagsProyectoCommand, ModifyColaTagsProyectoCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #region Actualizar tabla PROYECTOS
                    DbCommand InsertPROYECTOSCommand = ObtenerComando(sqlPROYECTOSInsert);
                    AgregarParametro(InsertPROYECTOSCommand, IBD.ToParam("Tag1"), DbType.String, "Tag1", DataRowVersion.Current);
                    AgregarParametro(InsertPROYECTOSCommand, IBD.ToParam("Tag2"), DbType.String, "Tag2", DataRowVersion.Current);
                    AgregarParametro(InsertPROYECTOSCommand, IBD.ToParam("Tipo"), DbType.Int16, "Tipo", DataRowVersion.Current);
                    AgregarParametro(InsertPROYECTOSCommand, IBD.ToParam("CercaniaDirecta"), DbType.Int32, "CercaniaDirecta", DataRowVersion.Current);
                    AgregarParametro(InsertPROYECTOSCommand, IBD.ToParam("CercaniaIndirecta"), DbType.Int32, "CercaniaIndirecta", DataRowVersion.Current);

                    DbCommand ModifyPROYECTOSCommand = ObtenerComando(sqlPROYECTOSModify);
                    AgregarParametro(ModifyPROYECTOSCommand, IBD.ToParam("Original_Tag1"), DbType.String, "Tag1", DataRowVersion.Original);
                    AgregarParametro(ModifyPROYECTOSCommand, IBD.ToParam("Original_Tag2"), DbType.String, "Tag2", DataRowVersion.Original);
                    AgregarParametro(ModifyPROYECTOSCommand, IBD.ToParam("Original_Tipo"), DbType.Int16, "Tipo", DataRowVersion.Original);
                    AgregarParametro(ModifyPROYECTOSCommand, IBD.ToParam("Original_CercaniaDirecta"), DbType.Int32, "CercaniaDirecta", DataRowVersion.Original);
                    AgregarParametro(ModifyPROYECTOSCommand, IBD.ToParam("Original_CercaniaIndirecta"), DbType.Int32, "CercaniaIndirecta", DataRowVersion.Original);

                    AgregarParametro(ModifyPROYECTOSCommand, IBD.ToParam("Tag1"), DbType.String, "Tag1", DataRowVersion.Current);
                    AgregarParametro(ModifyPROYECTOSCommand, IBD.ToParam("Tag2"), DbType.String, "Tag2", DataRowVersion.Current);
                    AgregarParametro(ModifyPROYECTOSCommand, IBD.ToParam("Tipo"), DbType.Int16, "Tipo", DataRowVersion.Current);
                    AgregarParametro(ModifyPROYECTOSCommand, IBD.ToParam("CercaniaDirecta"), DbType.Int32, "CercaniaDirecta", DataRowVersion.Current);
                    AgregarParametro(ModifyPROYECTOSCommand, IBD.ToParam("CercaniaIndirecta"), DbType.Int32, "CercaniaIndirecta", DataRowVersion.Current);
                    ActualizarBaseDeDatos(addedAndModifiedDataSet, "PROYECTOS", InsertPROYECTOSCommand, ModifyPROYECTOSCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

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
            this.sqlSelectColaTagsProyectos = "SELECT ColaTagsProyectos.OrdenEjecucion, ColaTagsProyectos.TablaBaseProyectoID, ColaTagsProyectos.Tags, ColaTagsProyectos.Tipo, ColaTagsProyectos.Estado, ColaTagsProyectos.FechaPuestaEnCola, ColaTagsProyectos.FechaProcesado, ColaTagsProyectos.Prioridad FROM ColaTagsProyectos";
            this.sqlSelectPROYECTOS = "SELECT PROYECTOS.Tag1, PROYECTOS.Tag2, PROYECTOS.Tipo, PROYECTOS.CercaniaDirecta, PROYECTOS.CercaniaIndirecta FROM PROYECTOS";
            #endregion

            #region DataAdapter
            #region ColaTagsProyectos
            this.sqlColaTagsProyectoInsert = IBD.ReplaceParam("INSERT INTO ColaTagsProyectos (TablaBaseProyectoID, Tags, Tipo, Estado, FechaPuestaEnCola, FechaProcesado,Prioridad) VALUES (@TablaBaseProyectoID, @Tags, @Tipo, @Estado, @FechaPuestaEnCola, @FechaProcesado,@Prioridad)");
            this.sqlColaTagsProyectoDelete = IBD.ReplaceParam("DELETE FROM ColaTagsProyectos WHERE (TablaBaseProyectoID = @Original_TablaBaseProyectoID) AND (Tags = @Original_Tags) AND (Tipo = @Original_Tipo) AND (Estado = @Original_Estado) AND (FechaPuestaEnCola = @Original_FechaPuestaEnCola) AND (FechaProcesado = @Original_FechaProcesado OR @Original_FechaProcesado IS NULL AND FechaProcesado IS NULL)");
            this.sqlColaTagsProyectoModify = IBD.ReplaceParam("UPDATE ColaTagsProyectos SET TablaBaseProyectoID = @TablaBaseProyectoID, Tags = @Tags, Tipo = @Tipo, Estado = @Estado, FechaPuestaEnCola = @FechaPuestaEnCola, FechaProcesado = @FechaProcesado,Prioridad=@Prioridad WHERE (TablaBaseProyectoID = @Original_TablaBaseProyectoID) AND (Tags = @Original_Tags) AND (Tipo = @Original_Tipo) AND (Estado = @Original_Estado) AND (FechaPuestaEnCola = @Original_FechaPuestaEnCola) AND (FechaProcesado = @Original_FechaProcesado OR @Original_FechaProcesado IS NULL AND FechaProcesado IS NULL)");
            #endregion

            #region PROYECTOS
            this.sqlPROYECTOSInsert = IBD.ReplaceParam("INSERT INTO PROYECTOS (Tag1, Tag2, Tipo, CercaniaDirecta, CercaniaIndirecta) VALUES (@Tag1, @Tag2, @Tipo, @CercaniaDirecta, @CercaniaIndirecta)");
            this.sqlPROYECTOSDelete = IBD.ReplaceParam("DELETE FROM PROYECTOS WHERE (Tag1 = @Original_Tag1) AND (Tag2 = @Original_Tag2) AND (Tipo = @Original_Tipo) AND (CercaniaDirecta = @Original_CercaniaDirecta) AND (CercaniaIndirecta = @Original_CercaniaIndirecta)");
            this.sqlPROYECTOSModify = IBD.ReplaceParam("UPDATE PROYECTOS SET Tag1 = @Tag1, Tag2 = @Tag2, Tipo = @Tipo, CercaniaDirecta = @CercaniaDirecta, CercaniaIndirecta = @CercaniaIndirecta WHERE (Tag1 = @Original_Tag1) AND (Tag2 = @Original_Tag2) AND (Tipo = @Original_Tipo) AND (CercaniaDirecta = @Original_CercaniaDirecta) AND (CercaniaIndirecta = @Original_CercaniaIndirecta)");
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
            BaseProyectosDS brProyectosComDS = new BaseProyectosDS();

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

            string consultaProyectosCom = sqlSelectColaTagsProyectos.Replace("SELECT ", "SELECT top(" + pNumMaxItems + ") ") + " WHERE Estado >= " + (short)pEstadoInferior + " AND Estado <= " + (short)pEstadoSuperior + andTipoElemento + andPrioridad + " ORDER BY Prioridad ASC, OrdenEjecucion ASC";

            DbCommand cmdObtnerElementosColaPendientesProyectosCom = ObtenerComando(consultaProyectosCom);
            CargarDataSet(cmdObtnerElementosColaPendientesProyectosCom, brProyectosComDS, "ColaTagsProyectos");

            return brProyectosComDS;
        }

        /// <summary>
        /// Obtiene el número de filas que hay entre los estados especificados en las últimas pHoras horas
        /// </summary>
        ///<param name="pHoras">Horas</param>
        ///<param name="pEstadoInferior">Estado inferior</param>
        ///<param name="pEstadoSuperior">Estado superior</param>
        /// <returns></returns>
        public override int ObtenerNumeroElementosEnXHoras(int pHoras, EstadosColaTags pEstadoInferior, EstadosColaTags pEstadoSuperior)
        {
            DbCommand cmdComprobarNumeroElementosFallidosEnXHoras = ObtenerComando("SELECT count(*) FROM ColaTagsProyectos  WHERE Estado >= " + (short)pEstadoInferior + " AND Estado <= " + (short)pEstadoSuperior + "  AND DATEADD(HH, 24, fechaprocesado)>GETDATE()");
            object resultado = EjecutarEscalar(cmdComprobarNumeroElementosFallidosEnXHoras);

            if (resultado != null)
            {
                return (int)resultado;
            }
            else
            {
                return 0;
            }
        }

        #endregion

        #region Verificacion de existencia y creación de tablas

        /// <summary>
        /// Comprueba si existe la tabla de blogs. Si no existe la crea 
        /// </summary>
        /// <param name="pCrearTablaSiNoExiste">Verdad si se debe crear la tabla en caso de que no exista</param>
        /// <param name="pTipoConsulta">Tipo de consulta que se va a realizar</param>
        /// <returns>Verdad si la tabla existe (o ha sido recién creada).</returns>
        public override bool VerificarExisteTabla(TiposConsultaObtenerTags pTipoConsulta, bool pCrearTablaSiNoExiste)
        {
            bool existeTabla = true;
            if (pTipoConsulta.Equals(TiposConsultaObtenerTags.Comunidades))
            {
                existeTabla = VerificarExisteTabla(mNombreTablaPROYECTOS);
            }
            else
            {
                existeTabla = base.VerificarExisteTabla(pTipoConsulta, pCrearTablaSiNoExiste);
            }

            if (!existeTabla && pCrearTablaSiNoExiste)
            {
                CrearTabla(pTipoConsulta);
                existeTabla = true;
            }

            return existeTabla;
        }

        /// <summary>
        /// Crea una tabla en función de un tipo de conuslta
        /// <param name="pTipoConsulta">Tipo de consulta</param>
        /// </summary>
        protected override void CrearTabla(TiposConsultaObtenerTags pTipoConsulta)
        {
            if (pTipoConsulta.Equals(TiposConsultaObtenerTags.Comunidades))
            {
                CrearTabla("CREATE TABLE [dbo]." + mNombreTablaPROYECTOS + "([Tag1] [nvarchar](1000) COLLATE Modern_Spanish_CI_AI NOT NULL, [Tag2] [nvarchar](1000) COLLATE Modern_Spanish_CI_AI NOT NULL,	[Tipo] [smallint] NOT NULL,	[CercaniaDirecta] [int] NOT NULL, [CercaniaIndirecta] [int] NOT NULL) ON [PRIMARY]");
            }
            else
            {
                base.CrearTabla(pTipoConsulta);
            }
        }

        #endregion

        /// <summary>
        /// Obtiene el sql correspondiente a un tipo de consulta
        /// </summary>
        /// <param name="pTipoConsulta">Tipo de consulta</param>
        /// <returns></returns>
        protected internal override string ObtenerNombreTablaPorTipoConsulta(TiposConsultaObtenerTags pTipoConsulta)
        {
            return mNombreTablaPROYECTOS;
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene el nombre real de la tabla que deberemos consultar para los tags de los blogs de una coomunidad publica en funcion del identificador numerico del proyecto
        /// </summary>
        public string NombreTablaPROYECTOS
        {
            get
            {
                return mNombreTablaPROYECTOS;
            }
        }
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
        #endregion
    }
}
