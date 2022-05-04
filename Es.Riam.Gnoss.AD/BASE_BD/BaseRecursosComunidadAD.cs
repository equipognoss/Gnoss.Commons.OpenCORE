using Es.Riam.Gnoss.AD.BASE_BD.Model;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace Es.Riam.Gnoss.AD.BASE_BD
{
    /// <summary>
    /// Tipo de elemento en cola en MyGnoss.
    /// </summary>
    public enum TipoColaEnMyGnoss
    {
        /// <summary>
        /// Rec de BR personal.
        /// </summary>
        RecursosPersonales = 0,
        /// <summary>
        /// Rec de com publicas.
        /// </summary>
        RecursosPublicos = 1,
        /// <summary>
        /// Rec privados para usu.
        /// </summary>
        RecursosPrivadosUsuario = 2
    }

    /// <summary>
    /// Clase para el dataadapter de los recursos de comunidad
    /// </summary>
    public class BaseRecursosComunidadAD : BaseComunidadAD
    {
        #region Constantes

        /// <summary>
        /// Constante para codificar los tags de tipo categorias de documentos
        /// </summary>
        public const string CAT_DOC = "##CATDOC##";
        /// <summary>
        /// Constante para codificar los tags de tipo autor de documento
        /// </summary>
        public const string AUT_DOC = "##AUTDOC##";
        /// <summary>
        /// Constante para codificar los tags de tipo publicador de documento
        /// </summary>
        public const string PUB_DOC = "##PUB_DOC##";
        /// <summary>
        ///Constante para codificar los tags de tipo Fecha de publicacion de documento
        /// </summary>
        public const string FECHAPUB_DOC = "##FECHAPUB_DOC##";
        /// <summary>
        ///Constante para codificar los tags de tipo Extension de documento
        /// </summary>
        public const string EXT_DOC = "##EXT_DOC##";
        /// <summary>
        ///Constante para codificar los tags de tipo Nivel de certificacionde documento
        /// </summary>
        public const string NIVCER_DOC = "##NIVCER_DOC##";

        /// <summary>
        ///Constante para codificar los tags de tipo Nivel de certificacionde documento
        /// </summary>
        public const string TIPO_DOC = "##TIPO_DOC##";

        /// <summary>
        ///Constante para codificar los tags de tipo Nivel de certificacionde documento
        /// </summary>
        public const string NOMBRE_DOC = "##NOMBRE_DOC##";

        /// <summary>
        ///Constante para codificar los tags de tipo Nivel de certificacionde documento
        /// </summary>
        public const string ENLACE_DOC = "##ENLACE_DOC##";

        /// <summary>
        ///Constante para codificar los tags de tipo Nivel de certificacionde documento
        /// </summary>
        public const string ENLACE_DOC_ELIMINADO = "##ENLACE_DOC_ELIMINADO##";

        /// <summary>
        ///Constante para codificar los tags de tipo Nivel de certificacionde documento
        /// </summary>
        public const string ESTADO_COMENTADO = "##ESTADO_COM##";

        /// <summary>
        ///Constante para codificar los tags de tipo ID-Tag de dafo
        /// </summary>
        public const string ID_TAG_DOCUMENTO = "##ID_TAG_DOC##";

        /// <summary>
        ///Constante para codificar el id de los proyectos de destino para compartir ontologías
        /// </summary>
        public const string ID_PROY_DESTINO = "##ID_PROY_DESTINO##";

        /// <summary>
        ///Constante para codificar los tags de tipo ID-Tag de dafo
        /// </summary>
        public const string SAME_AS = "##enlaces##";

        /// <summary>
        ///Constante para codificar los ids de los proyectos origen.
        /// </summary>
        public const string ID_PROYECTO_ORIGEN = "##ID_PROYECTO_ORIGEN##";

        /// <summary>
        ///Constante para codificar el tipo de inserción de todos los recursos en el base.
        /// </summary>
        public const string GENERAR_TODOS_RECURSOS = "##GENERAR_TODOS_RECURSOS##";

        /// <summary>
        /// Constante para codificar la cadena de afinidad de virtuoso
        /// </summary>
        public const string AFINIDAD_VIRTUOSO = "##AFINIDAD_VIRTUOSO##";

        #endregion

        #region Miembros

        /// <summary>
        /// Si estamos consultando las tablas de una comunidad concreta, se especifica si tiene creada la tabla de documentos privados o no
        /// </summary>
        private bool mExisteTablaPrivada;

        #endregion

        #region Constructores

        /// <summary>
        /// Cuando se desea pasar directamente la ruta del fichero de configuracion de conexion a la Base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Fichero de configuración de la base de datos BASE</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        /// <param name="pTablaBaseProyectoID">Identificador numerico del proyecto (-1 si no se va a actualizar tablas de proyecto)</param>
        public BaseRecursosComunidadAD(string pFicheroConfiguracionBD, int pTablaBaseProyectoID, LoggingService loggingService, EntityContext entityContext, EntityContextBASE entityContextBASE, ConfigService configService)
            : base(pTablaBaseProyectoID, loggingService, entityContext, entityContextBASE, configService)
        {

            mNombreTablaCOMUNIDADES = "COMUNIDAD";
            mNombreTablaCOM_USU_PRIV = "COM_USU_PRIV";

            if (pTablaBaseProyectoID > -1)
            {
                if (BusquedaEnMyGnoss)
                {
                    mNombreTablaCOMUNIDADES = "REC_PUBLICOS_MYGNOSS";
                    if (ExisteTablaPrivada)
                    {
                        mNombreTablaCOM_USU_PRIV = "REC_PRIVADOS_COM_USU";
                    }
                }
                else
                {
                    mNombreTablaCOMUNIDADES = "COMUNIDAD_000000000000000".Substring(0, 25 - mTablaBaseProyectoID.ToString().Length) + mTablaBaseProyectoID.ToString();
                    mNombreTablaCOM_USU_PRIV = "COM_USU_PRIV_000000000000".Substring(0, 25 - mTablaBaseProyectoID.ToString().Length) + mTablaBaseProyectoID.ToString();
                }
            }

            this.CargarConsultasYDataAdapters(IBD);
        }


        #endregion

        #region Consultas

        private string sqlSelectColaTagsComunidades;
        private string sqlSelectColaTagsComunidadesSearch;
        private string sqlSelectColaTagsComPriv;
        protected string sqlSelectCOM_USU_PRIV;
        protected string sqlSelectCOMUNIDAD;
        private string sqlSelectColaTagsMyGnoss;
        //private string sqlSelectColaServicioUDP;
        private string sqlSelectREC_BR_PER_PUBLICOS;
        private string sqlSelectREC_PRIVADOS_COM_USU;
        private string sqlSelectREC_PUBLICOS_MYGNOSS;
        //private string sqlSelectExisteElementoEnColaTagsComunidades;

        #endregion

        #region DataAdapter

        #region ColaTagsComunidades

        private string sqlColaTagsComunidadesInsert;
        private string sqlColaTagsComunidadesDelete;
        private string sqlColaTagsComunidadesModify;

        #endregion

        #region ColaTagsComunidadesSearch

        private string sqlColaTagsComunidadesSearchInsert;
        private string sqlColaTagsComunidadesSearchDelete;
        private string sqlColaTagsComunidadesSearchModify;

        #endregion

        #region ColaTagsComPriv

        private string sqlColaTagsComPrivInsert;
        private string sqlColaTagsComPrivDelete;
        private string sqlColaTagsComPrivModify;

        #endregion

        #region COM_USU_PRIV

        private string sqlCOM_USU_PRIVInsert;
        private string sqlCOM_USU_PRIVDelete;
        private string sqlCOM_USU_PRIVModify;

        #endregion

        #region COMUNIDAD

        private string sqlCOMUNIDADInsert;
        private string sqlCOMUNIDADDelete;
        private string sqlCOMUNIDADModify;

        #endregion

        #region ColaTagsMyGnoss
        private string sqlColaTagsMyGnossInsert;
        private string sqlColaTagsMyGnossDelete;
        private string sqlColaTagsMyGnossModify;
        #endregion

        #region ColaServicioUDP
        private string sqlColaServicioUDPInsert;
        private string sqlColaServicioUDPDelete;
        private string sqlColaServicioUDPModify;
        #endregion

        #region REC_BR_PER_PUBLICOS
        private string sqlREC_BR_PER_PUBLICOSInsert;
        private string sqlREC_BR_PER_PUBLICOSDelete;
        private string sqlREC_BR_PER_PUBLICOSModify;
        #endregion

        #region REC_PRIVADOS_COM_USU
        private string sqlREC_PRIVADOS_COM_USUInsert;
        private string sqlREC_PRIVADOS_COM_USUDelete;
        private string sqlREC_PRIVADOS_COM_USUModify;
        #endregion

        #region REC_PUBLICOS_MYGNOSS
        private string sqlREC_PUBLICOS_MYGNOSSInsert;
        private string sqlREC_PUBLICOS_MYGNOSSDelete;
        private string sqlREC_PUBLICOS_MYGNOSSModify;
        #endregion

        #region ColaGrafoDbpedia

        private string sqlColaGrafoDbpediaInsert;
        private string sqlColaGrafoDbpediaDelete;
        private string sqlColaGrafoDbpediaModify;

        #endregion

        #endregion

        #region Métodos generales

        #region Métodos AD

        /// <summary>
        /// Elimina los elementos borrados del DataSet
        /// </summary>
        /// <param name="pDataSet">DataSet de eliminados</param>
        public override void EliminarBorrados(DataSet pDataSet)
        {
            try
            {
                DataSet deletedDataSet = pDataSet.GetChanges(DataRowState.Deleted);

                if (deletedDataSet != null)
                {
                    #region Deleted

                    if (mTablaBaseProyectoID > -1) //Si no hay proyectoID, no es necesario actualizar estas tablas
                    {
                        #region Eliminar tabla COMUNIDAD

                        DbCommand DeleteCOMUNIDADCommand = ObtenerComando(sqlCOMUNIDADDelete);

                        AgregarParametro(DeleteCOMUNIDADCommand, IBD.ToParam("O_Tag1"), DbType.String, "Tag1", DataRowVersion.Original);
                        AgregarParametro(DeleteCOMUNIDADCommand, IBD.ToParam("O_Tag2"), DbType.String, "Tag2", DataRowVersion.Original);
                        AgregarParametro(DeleteCOMUNIDADCommand, IBD.ToParam("O_Tipo"), DbType.Int16, "Tipo", DataRowVersion.Original);
                        AgregarParametro(DeleteCOMUNIDADCommand, IBD.ToParam("O_CercaniaDirecta"), DbType.Int32, "CercaniaDirecta", DataRowVersion.Original);
                        AgregarParametro(DeleteCOMUNIDADCommand, IBD.ToParam("O_CercaniaIndirecta"), DbType.Int32, "CercaniaIndirecta", DataRowVersion.Original);

                        ActualizarBaseDeDatos(deletedDataSet, "COMUNIDAD", null, null, DeleteCOMUNIDADCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                        #endregion

                        #region Eliminar tabla COM_USU_PRIV

                        DbCommand DeleteCOM_USU_PRIVCommand = ObtenerComando(sqlCOM_USU_PRIVDelete);

                        AgregarParametro(DeleteCOM_USU_PRIVCommand, IBD.ToParam("O_Tag1"), DbType.String, "Tag1", DataRowVersion.Original);
                        AgregarParametro(DeleteCOM_USU_PRIVCommand, IBD.ToParam("O_Tag2"), DbType.String, "Tag2", DataRowVersion.Original);
                        AgregarParametro(DeleteCOM_USU_PRIVCommand, IBD.ToParam("O_Tipo"), DbType.Int16, "Tipo", DataRowVersion.Original);
                        AgregarParametro(DeleteCOM_USU_PRIVCommand, IBD.ToParam("O_CercaniaDirecta"), DbType.Int32, "CercaniaDirecta", DataRowVersion.Original);
                        AgregarParametro(DeleteCOM_USU_PRIVCommand, IBD.ToParam("O_CercaniaIndirecta"), DbType.Int32, "CercaniaIndirecta", DataRowVersion.Original);
                        AgregarParametro(DeleteCOM_USU_PRIVCommand, IBD.ToParam("O_IdentidadID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadID", DataRowVersion.Original);

                        ActualizarBaseDeDatos(deletedDataSet, "COM_USU_PRIV", null, null, DeleteCOM_USU_PRIVCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                        #endregion
                    }

                    #region Eliminar tabla ColaGrafoDbpedia 

                    DbCommand DeleteColaGrafoDbpediaCommand = ObtenerComando(sqlColaGrafoDbpediaDelete);

                    AgregarParametro(DeleteColaGrafoDbpediaCommand, IBD.ToParam("Original_GnossID"), DbType.Guid, "GnossID", DataRowVersion.Original);
                    AgregarParametro(DeleteColaGrafoDbpediaCommand, IBD.ToParam("Original_UriDbpedia"), DbType.String, "UriDbpedia", DataRowVersion.Original);
                    AgregarParametro(DeleteColaGrafoDbpediaCommand, IBD.ToParam("Original_Accion"), DbType.Int16, "Accion", DataRowVersion.Original);

                    ActualizarBaseDeDatos(deletedDataSet, "ColaGrafoDbpedia", null, null, DeleteColaGrafoDbpediaCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #region Eliminar tabla REC_PUBLICOS_MYGNOSS
                    DbCommand DeleteREC_PUBLICOS_MYGNOSSCommand = ObtenerComando(sqlREC_PUBLICOS_MYGNOSSDelete);
                    AgregarParametro(DeleteREC_PUBLICOS_MYGNOSSCommand, IBD.ToParam("Original_Tag1"), DbType.String, "Tag1", DataRowVersion.Original);
                    AgregarParametro(DeleteREC_PUBLICOS_MYGNOSSCommand, IBD.ToParam("Original_Tag2"), DbType.String, "Tag2", DataRowVersion.Original);
                    AgregarParametro(DeleteREC_PUBLICOS_MYGNOSSCommand, IBD.ToParam("Original_Tipo"), DbType.Int16, "Tipo", DataRowVersion.Original);
                    AgregarParametro(DeleteREC_PUBLICOS_MYGNOSSCommand, IBD.ToParam("Original_CercaniaDirecta"), DbType.Int32, "CercaniaDirecta", DataRowVersion.Original);
                    AgregarParametro(DeleteREC_PUBLICOS_MYGNOSSCommand, IBD.ToParam("Original_CercaniaIndirecta"), DbType.Int32, "CercaniaIndirecta", DataRowVersion.Original);
                    ActualizarBaseDeDatos(deletedDataSet, "REC_PUBLICOS_MYGNOSS", null, null, DeleteREC_PUBLICOS_MYGNOSSCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #region Eliminar tabla REC_PRIVADOS_COM_USU
                    DbCommand DeleteREC_PRIVADOS_COM_USUCommand = ObtenerComando(sqlREC_PRIVADOS_COM_USUDelete);
                    AgregarParametro(DeleteREC_PRIVADOS_COM_USUCommand, IBD.ToParam("Original_Tag1"), DbType.String, "Tag1", DataRowVersion.Original);
                    AgregarParametro(DeleteREC_PRIVADOS_COM_USUCommand, IBD.ToParam("Original_Tag2"), DbType.String, "Tag2", DataRowVersion.Original);
                    AgregarParametro(DeleteREC_PRIVADOS_COM_USUCommand, IBD.ToParam("Original_Tipo"), DbType.Int16, "Tipo", DataRowVersion.Original);
                    AgregarParametro(DeleteREC_PRIVADOS_COM_USUCommand, IBD.ToParam("Original_CercaniaDirecta"), DbType.Int32, "CercaniaDirecta", DataRowVersion.Original);
                    AgregarParametro(DeleteREC_PRIVADOS_COM_USUCommand, IBD.ToParam("Original_CercaniaIndirecta"), DbType.Int32, "CercaniaIndirecta", DataRowVersion.Original);
                    AgregarParametro(DeleteREC_PRIVADOS_COM_USUCommand, IBD.ToParam("Original_UsuarioID"), IBD.TipoGuidToObject(DbType.Guid), "UsuarioID", DataRowVersion.Original);
                    ActualizarBaseDeDatos(deletedDataSet, "REC_PRIVADOS_COM_USU", null, null, DeleteREC_PRIVADOS_COM_USUCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #region Eliminar tabla REC_BR_PER_PUBLICOS
                    DbCommand DeleteREC_BR_PER_PUBLICOSCommand = ObtenerComando(sqlREC_BR_PER_PUBLICOSDelete);
                    AgregarParametro(DeleteREC_BR_PER_PUBLICOSCommand, IBD.ToParam("Original_Tag1"), DbType.String, "Tag1", DataRowVersion.Original);
                    AgregarParametro(DeleteREC_BR_PER_PUBLICOSCommand, IBD.ToParam("Original_Tag2"), DbType.String, "Tag2", DataRowVersion.Original);
                    AgregarParametro(DeleteREC_BR_PER_PUBLICOSCommand, IBD.ToParam("Original_Tipo"), DbType.Int16, "Tipo", DataRowVersion.Original);
                    AgregarParametro(DeleteREC_BR_PER_PUBLICOSCommand, IBD.ToParam("Original_CercaniaDirecta"), DbType.Int32, "CercaniaDirecta", DataRowVersion.Original);
                    AgregarParametro(DeleteREC_BR_PER_PUBLICOSCommand, IBD.ToParam("Original_CercaniaIndirecta"), DbType.Int32, "CercaniaIndirecta", DataRowVersion.Original);
                    ActualizarBaseDeDatos(deletedDataSet, "REC_BR_PER_PUBLICOS", null, null, DeleteREC_BR_PER_PUBLICOSCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #region Eliminar tabla ColaTagsMyGnoss
                    DbCommand DeleteColaTagsMyGnossCommand = ObtenerComando(sqlColaTagsMyGnossDelete);
                    //AgregarParametro(DeleteColaTagsMyGnossCommand, IBD.ToParam("Original_OrdenEjecucion"), DbType.Int32, "OrdenEjecucion", DataRowVersion.Original);
                    AgregarParametro(DeleteColaTagsMyGnossCommand, IBD.ToParam("Original_TablaBaseProyectoID"), DbType.Int32, "TablaBaseProyectoID", DataRowVersion.Original);
                    AgregarParametro(DeleteColaTagsMyGnossCommand, IBD.ToParam("Original_UsuarioID"), IBD.TipoGuidToObject(DbType.Guid), "UsuarioID", DataRowVersion.Original);
                    AgregarParametro(DeleteColaTagsMyGnossCommand, IBD.ToParam("Original_Tags"), DbType.String, "Tags", DataRowVersion.Original);
                    AgregarParametro(DeleteColaTagsMyGnossCommand, IBD.ToParam("Original_Tipo"), DbType.Int16, "Tipo", DataRowVersion.Original);
                    AgregarParametro(DeleteColaTagsMyGnossCommand, IBD.ToParam("Original_TipoEnMyGnoss"), DbType.Int16, "TipoEnMyGnoss", DataRowVersion.Original);
                    AgregarParametro(DeleteColaTagsMyGnossCommand, IBD.ToParam("Original_Estado"), DbType.Int16, "Estado", DataRowVersion.Original);
                    AgregarParametro(DeleteColaTagsMyGnossCommand, IBD.ToParam("Original_FechaPuestaEnCola"), DbType.DateTime, "FechaPuestaEnCola", DataRowVersion.Original);
                    AgregarParametro(DeleteColaTagsMyGnossCommand, IBD.ToParam("Original_FechaProcesado"), DbType.DateTime, "FechaProcesado", DataRowVersion.Original);
                    AgregarParametro(DeleteColaTagsMyGnossCommand, IBD.ToParam("Original_Prioridad"), DbType.Int16, "Prioridad", DataRowVersion.Original);
                    ActualizarBaseDeDatos(deletedDataSet, "ColaTagsMyGnoss", null, null, DeleteColaTagsMyGnossCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #region Eliminar tabla ColaTagsMyGnoss

                    DbCommand DeleteColaServicioUDPCommand = ObtenerComando(sqlColaServicioUDPDelete);
                    AgregarParametro(DeleteColaServicioUDPCommand, IBD.ToParam("Original_OrdenEjecucion"), DbType.Int32, "OrdenEjecucion", DataRowVersion.Original);
                    ActualizarBaseDeDatos(deletedDataSet, "ColaServicioUDP", null, null, DeleteColaServicioUDPCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #region Eliminar tabla ColaTagsComunidades (SOLO OrdenEjecucion)

                    DbCommand DeleteColaTagsComunidadesCommand = ObtenerComando(sqlColaTagsComunidadesDelete);

                    AgregarParametro(DeleteColaTagsComunidadesCommand, IBD.ToParam("O_OrdenEjecucion"), DbType.Int32, "OrdenEjecucion", DataRowVersion.Original);

                    ActualizarBaseDeDatos(deletedDataSet, "ColaTagsComunidades", null, null, DeleteColaTagsComunidadesCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #region Eliminar tabla ColaTagsComunidadesSearch (SOLO OrdenEjecucion)

                    DbCommand DeleteColaTagsComunidadesSearchCommand = ObtenerComando(sqlColaTagsComunidadesSearchDelete);

                    AgregarParametro(DeleteColaTagsComunidadesSearchCommand, IBD.ToParam("O_OrdenEjecucion"), DbType.Int32, "OrdenEjecucion", DataRowVersion.Original);

                    ActualizarBaseDeDatos(deletedDataSet, "ColaTagsComunidadesSearch", null, null, DeleteColaTagsComunidadesSearchCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #region Eliminar tabla ColaTagsComPriv

                    DbCommand DeleteColaTagsComPrivCommand = ObtenerComando(sqlColaTagsComPrivDelete);

                    AgregarParametro(DeleteColaTagsComPrivCommand, IBD.ToParam("O_TablaBaseProyectoID"), DbType.Int32, "TablaBaseProyectoID", DataRowVersion.Original);
                    AgregarParametro(DeleteColaTagsComPrivCommand, IBD.ToParam("O_IdentidadID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadID", DataRowVersion.Original);
                    AgregarParametro(DeleteColaTagsComPrivCommand, IBD.ToParam("O_Tags"), DbType.String, "Tags", DataRowVersion.Original);
                    AgregarParametro(DeleteColaTagsComPrivCommand, IBD.ToParam("O_Tipo"), DbType.Int16, "Tipo", DataRowVersion.Original);
                    AgregarParametro(DeleteColaTagsComPrivCommand, IBD.ToParam("O_Estado"), DbType.Int16, "Estado", DataRowVersion.Original);
                    AgregarParametro(DeleteColaTagsComPrivCommand, IBD.ToParam("O_FechaPuestaEnCola"), DbType.DateTime, "FechaPuestaEnCola", DataRowVersion.Original);
                    AgregarParametro(DeleteColaTagsComPrivCommand, IBD.ToParam("O_FechaProcesado"), DbType.DateTime, "FechaProcesado", DataRowVersion.Original);
                    AgregarParametro(DeleteColaTagsComPrivCommand, IBD.ToParam("O_Prioridad"), DbType.Int16, "Prioridad", DataRowVersion.Original);

                    ActualizarBaseDeDatos(deletedDataSet, "ColaTagsComPriv", null, null, DeleteColaTagsComPrivCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

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
                InsertarFilasEnRabbit("ColaTagsComunidades", pDataSet);

                DataSet addedAndModifiedDataSet = pDataSet.GetChanges(DataRowState.Added | DataRowState.Modified);

                if (addedAndModifiedDataSet != null)
                {
                    #region AddedAndModified

                    #region Actualizar tabla ColaTagsComunidades (Solo OrdenEjecucion)

                    DbCommand InsertColaTagsComunidadesCommand = ObtenerComando(sqlColaTagsComunidadesInsert);

                    AgregarParametro(InsertColaTagsComunidadesCommand, IBD.ToParam("TablaBaseProyectoID"), DbType.Int32, "TablaBaseProyectoID", DataRowVersion.Current);
                    AgregarParametro(InsertColaTagsComunidadesCommand, IBD.ToParam("Tags"), DbType.String, "Tags", DataRowVersion.Current);
                    AgregarParametro(InsertColaTagsComunidadesCommand, IBD.ToParam("Tipo"), DbType.Int16, "Tipo", DataRowVersion.Current);
                    AgregarParametro(InsertColaTagsComunidadesCommand, IBD.ToParam("Estado"), DbType.Int16, "Estado", DataRowVersion.Current);
                    AgregarParametro(InsertColaTagsComunidadesCommand, IBD.ToParam("FechaPuestaEnCola"), DbType.DateTime, "FechaPuestaEnCola", DataRowVersion.Current);
                    AgregarParametro(InsertColaTagsComunidadesCommand, IBD.ToParam("FechaProcesado"), DbType.DateTime, "FechaProcesado", DataRowVersion.Current);
                    AgregarParametro(InsertColaTagsComunidadesCommand, IBD.ToParam("Prioridad"), DbType.Int16, "Prioridad", DataRowVersion.Current);
                    AgregarParametro(InsertColaTagsComunidadesCommand, IBD.ToParam("EstadoTags"), DbType.Int16, "EstadoTags", DataRowVersion.Current);
                    AgregarParametro(InsertColaTagsComunidadesCommand, IBD.ToParam("EstadoCargaID"), DbType.Int64, "EstadoCargaID", DataRowVersion.Current);
                    AgregarParametro(InsertColaTagsComunidadesCommand, IBD.ToParam("TipoAccionCarga"), DbType.Int16, "TipoAccionCarga", DataRowVersion.Current);

                    DbCommand ModifyColaTagsComunidadesCommand = ObtenerComando(sqlColaTagsComunidadesModify);
                    AgregarParametro(ModifyColaTagsComunidadesCommand, IBD.ToParam("O_OrdenEjecucion"), DbType.Int32, "OrdenEjecucion", DataRowVersion.Original);

                    AgregarParametro(ModifyColaTagsComunidadesCommand, IBD.ToParam("TablaBaseProyectoID"), DbType.Int32, "TablaBaseProyectoID", DataRowVersion.Current);
                    AgregarParametro(ModifyColaTagsComunidadesCommand, IBD.ToParam("Tags"), DbType.String, "Tags", DataRowVersion.Current);
                    AgregarParametro(ModifyColaTagsComunidadesCommand, IBD.ToParam("Tipo"), DbType.Int16, "Tipo", DataRowVersion.Current);
                    AgregarParametro(ModifyColaTagsComunidadesCommand, IBD.ToParam("Estado"), DbType.Int16, "Estado", DataRowVersion.Current);
                    AgregarParametro(ModifyColaTagsComunidadesCommand, IBD.ToParam("FechaPuestaEnCola"), DbType.DateTime, "FechaPuestaEnCola", DataRowVersion.Current);
                    AgregarParametro(ModifyColaTagsComunidadesCommand, IBD.ToParam("FechaProcesado"), DbType.DateTime, "FechaProcesado", DataRowVersion.Current);
                    AgregarParametro(ModifyColaTagsComunidadesCommand, IBD.ToParam("Prioridad"), DbType.Int16, "Prioridad", DataRowVersion.Current);
                    AgregarParametro(ModifyColaTagsComunidadesCommand, IBD.ToParam("EstadoTags"), DbType.Int16, "EstadoTags", DataRowVersion.Current);
                    AgregarParametro(ModifyColaTagsComunidadesCommand, IBD.ToParam("EstadoCargaID"), DbType.Int64, "EstadoCargaID", DataRowVersion.Current);
                    AgregarParametro(ModifyColaTagsComunidadesCommand, IBD.ToParam("TipoAccionCarga"), DbType.Int16, "TipoAccionCarga", DataRowVersion.Current);

                    ActualizarBaseDeDatos(addedAndModifiedDataSet, "ColaTagsComunidades", InsertColaTagsComunidadesCommand, ModifyColaTagsComunidadesCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #region Actualizar tabla ColaTagsComunidadesSearch (Solo OrdenEjecucion)

                    DbCommand InsertColaTagsComunidadesSearchCommand = ObtenerComando(sqlColaTagsComunidadesSearchInsert);

                    AgregarParametro(InsertColaTagsComunidadesSearchCommand, IBD.ToParam("TablaBaseProyectoID"), DbType.Int32, "TablaBaseProyectoID", DataRowVersion.Current);
                    AgregarParametro(InsertColaTagsComunidadesSearchCommand, IBD.ToParam("Tags"), DbType.String, "Tags", DataRowVersion.Current);
                    AgregarParametro(InsertColaTagsComunidadesSearchCommand, IBD.ToParam("Tipo"), DbType.Int16, "Tipo", DataRowVersion.Current);
                    AgregarParametro(InsertColaTagsComunidadesSearchCommand, IBD.ToParam("Estado"), DbType.Int16, "Estado", DataRowVersion.Current);
                    AgregarParametro(InsertColaTagsComunidadesSearchCommand, IBD.ToParam("FechaPuestaEnCola"), DbType.DateTime, "FechaPuestaEnCola", DataRowVersion.Current);
                    AgregarParametro(InsertColaTagsComunidadesSearchCommand, IBD.ToParam("FechaProcesado"), DbType.DateTime, "FechaProcesado", DataRowVersion.Current);
                    AgregarParametro(InsertColaTagsComunidadesSearchCommand, IBD.ToParam("Prioridad"), DbType.Int16, "Prioridad", DataRowVersion.Current);
                    AgregarParametro(InsertColaTagsComunidadesSearchCommand, IBD.ToParam("EstadoTags"), DbType.Int16, "EstadoTags", DataRowVersion.Current);
                    AgregarParametro(InsertColaTagsComunidadesSearchCommand, IBD.ToParam("EstadoCargaID"), DbType.Int64, "EstadoCargaID", DataRowVersion.Current);
                    AgregarParametro(InsertColaTagsComunidadesSearchCommand, IBD.ToParam("TipoAccionCarga"), DbType.Int16, "TipoAccionCarga", DataRowVersion.Current);

                    DbCommand ModifyColaTagsComunidadesSearchCommand = ObtenerComando(sqlColaTagsComunidadesSearchModify);
                    AgregarParametro(ModifyColaTagsComunidadesSearchCommand, IBD.ToParam("O_OrdenEjecucion"), DbType.Int32, "OrdenEjecucion", DataRowVersion.Original);

                    AgregarParametro(ModifyColaTagsComunidadesSearchCommand, IBD.ToParam("TablaBaseProyectoID"), DbType.Int32, "TablaBaseProyectoID", DataRowVersion.Current);
                    AgregarParametro(ModifyColaTagsComunidadesSearchCommand, IBD.ToParam("Tags"), DbType.String, "Tags", DataRowVersion.Current);
                    AgregarParametro(ModifyColaTagsComunidadesSearchCommand, IBD.ToParam("Tipo"), DbType.Int16, "Tipo", DataRowVersion.Current);
                    AgregarParametro(ModifyColaTagsComunidadesSearchCommand, IBD.ToParam("Estado"), DbType.Int16, "Estado", DataRowVersion.Current);
                    AgregarParametro(ModifyColaTagsComunidadesSearchCommand, IBD.ToParam("FechaPuestaEnCola"), DbType.DateTime, "FechaPuestaEnCola", DataRowVersion.Current);
                    AgregarParametro(ModifyColaTagsComunidadesSearchCommand, IBD.ToParam("FechaProcesado"), DbType.DateTime, "FechaProcesado", DataRowVersion.Current);
                    AgregarParametro(ModifyColaTagsComunidadesSearchCommand, IBD.ToParam("Prioridad"), DbType.Int16, "Prioridad", DataRowVersion.Current);
                    AgregarParametro(ModifyColaTagsComunidadesSearchCommand, IBD.ToParam("EstadoTags"), DbType.Int16, "EstadoTags", DataRowVersion.Current);
                    AgregarParametro(ModifyColaTagsComunidadesSearchCommand, IBD.ToParam("EstadoCargaID"), DbType.Int64, "EstadoCargaID", DataRowVersion.Current);
                    AgregarParametro(ModifyColaTagsComunidadesSearchCommand, IBD.ToParam("TipoAccionCarga"), DbType.Int16, "TipoAccionCarga", DataRowVersion.Current);

                    ActualizarBaseDeDatos(addedAndModifiedDataSet, "ColaTagsComunidadesSearch", InsertColaTagsComunidadesSearchCommand, ModifyColaTagsComunidadesSearchCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #region Actualizar tabla ColaTagsComPriv

                    DbCommand InsertColaTagsComPrivCommand = ObtenerComando(sqlColaTagsComPrivInsert);

                    AgregarParametro(InsertColaTagsComPrivCommand, IBD.ToParam("TablaBaseProyectoID"), DbType.Int32, "TablaBaseProyectoID", DataRowVersion.Current);
                    AgregarParametro(InsertColaTagsComPrivCommand, IBD.ToParam("IdentidadID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadID", DataRowVersion.Current);
                    AgregarParametro(InsertColaTagsComPrivCommand, IBD.ToParam("Tags"), DbType.String, "Tags", DataRowVersion.Current);
                    AgregarParametro(InsertColaTagsComPrivCommand, IBD.ToParam("Tipo"), DbType.Int16, "Tipo", DataRowVersion.Current);
                    AgregarParametro(InsertColaTagsComPrivCommand, IBD.ToParam("Estado"), DbType.Int16, "Estado", DataRowVersion.Current);
                    AgregarParametro(InsertColaTagsComPrivCommand, IBD.ToParam("FechaPuestaEnCola"), DbType.DateTime, "FechaPuestaEnCola", DataRowVersion.Current);
                    AgregarParametro(InsertColaTagsComPrivCommand, IBD.ToParam("FechaProcesado"), DbType.DateTime, "FechaProcesado", DataRowVersion.Current);
                    AgregarParametro(InsertColaTagsComPrivCommand, IBD.ToParam("Prioridad"), DbType.Int16, "Prioridad", DataRowVersion.Current);

                    DbCommand ModifyColaTagsComPrivCommand = ObtenerComando(sqlColaTagsComPrivModify);
                    AgregarParametro(ModifyColaTagsComPrivCommand, IBD.ToParam("O_OrdenEjecucion"), DbType.Int32, "OrdenEjecucion", DataRowVersion.Original);
                    AgregarParametro(ModifyColaTagsComPrivCommand, IBD.ToParam("O_TablaBaseProyectoID"), DbType.Int32, "TablaBaseProyectoID", DataRowVersion.Original);
                    AgregarParametro(ModifyColaTagsComPrivCommand, IBD.ToParam("O_IdentidadID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadID", DataRowVersion.Original);
                    AgregarParametro(ModifyColaTagsComPrivCommand, IBD.ToParam("O_Tags"), DbType.String, "Tags", DataRowVersion.Original);
                    AgregarParametro(ModifyColaTagsComPrivCommand, IBD.ToParam("O_Tipo"), DbType.Int16, "Tipo", DataRowVersion.Original);
                    AgregarParametro(ModifyColaTagsComPrivCommand, IBD.ToParam("O_Estado"), DbType.Int16, "Estado", DataRowVersion.Original);
                    AgregarParametro(ModifyColaTagsComPrivCommand, IBD.ToParam("O_FechaPuestaEnCola"), DbType.DateTime, "FechaPuestaEnCola", DataRowVersion.Original);
                    AgregarParametro(ModifyColaTagsComPrivCommand, IBD.ToParam("O_FechaProcesado"), DbType.DateTime, "FechaProcesado", DataRowVersion.Original);
                    AgregarParametro(ModifyColaTagsComPrivCommand, IBD.ToParam("O_Prioridad"), DbType.Int16, "Prioridad", DataRowVersion.Original);

                    AgregarParametro(ModifyColaTagsComPrivCommand, IBD.ToParam("TablaBaseProyectoID"), DbType.Int32, "TablaBaseProyectoID", DataRowVersion.Current);
                    AgregarParametro(ModifyColaTagsComPrivCommand, IBD.ToParam("IdentidadID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadID", DataRowVersion.Current);
                    AgregarParametro(ModifyColaTagsComPrivCommand, IBD.ToParam("Tags"), DbType.String, "Tags", DataRowVersion.Current);
                    AgregarParametro(ModifyColaTagsComPrivCommand, IBD.ToParam("Tipo"), DbType.Int16, "Tipo", DataRowVersion.Current);
                    AgregarParametro(ModifyColaTagsComPrivCommand, IBD.ToParam("Estado"), DbType.Int16, "Estado", DataRowVersion.Current);
                    AgregarParametro(ModifyColaTagsComPrivCommand, IBD.ToParam("FechaPuestaEnCola"), DbType.DateTime, "FechaPuestaEnCola", DataRowVersion.Current);
                    AgregarParametro(ModifyColaTagsComPrivCommand, IBD.ToParam("FechaProcesado"), DbType.DateTime, "FechaProcesado", DataRowVersion.Current);
                    AgregarParametro(ModifyColaTagsComPrivCommand, IBD.ToParam("Prioridad"), DbType.Int16, "Prioridad", DataRowVersion.Current);

                    ActualizarBaseDeDatos(addedAndModifiedDataSet, "ColaTagsComPriv", InsertColaTagsComPrivCommand, ModifyColaTagsComPrivCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #region Actualizar tabla ColaServicioUDP

                    DbCommand InsertColaServicioUDPCommand = ObtenerComando(sqlColaServicioUDPInsert);
                    AgregarParametro(InsertColaServicioUDPCommand, IBD.ToParam("Consulta"), DbType.String, "Consulta", DataRowVersion.Current);
                    AgregarParametro(InsertColaServicioUDPCommand, IBD.ToParam("Estado"), DbType.Int16, "Estado", DataRowVersion.Current);
                    AgregarParametro(InsertColaServicioUDPCommand, IBD.ToParam("FechaPuestaEnCola"), DbType.DateTime, "FechaPuestaEnCola", DataRowVersion.Current);
                    AgregarParametro(InsertColaServicioUDPCommand, IBD.ToParam("FechaProcesado"), DbType.DateTime, "FechaProcesado", DataRowVersion.Current);
                    AgregarParametro(InsertColaServicioUDPCommand, IBD.ToParam("Prioridad"), DbType.Int16, "Prioridad", DataRowVersion.Current);

                    DbCommand ModifyColaServicioUDPCommand = ObtenerComando(sqlColaServicioUDPModify);
                    AgregarParametro(ModifyColaServicioUDPCommand, IBD.ToParam("Original_OrdenEjecucion"), DbType.Int32, "OrdenEjecucion", DataRowVersion.Original);

                    //AgregarParametro(ModifyColaServicioUDPCommand, IBD.ToParam("OrdenEjecucion"), DbType.Int32, "OrdenEjecucion", DataRowVersion.Current);
                    AgregarParametro(ModifyColaServicioUDPCommand, IBD.ToParam("Consulta"), DbType.String, "Consulta", DataRowVersion.Current);
                    AgregarParametro(ModifyColaServicioUDPCommand, IBD.ToParam("Estado"), DbType.Int16, "Estado", DataRowVersion.Current);
                    AgregarParametro(ModifyColaServicioUDPCommand, IBD.ToParam("Prioridad"), DbType.Int16, "Prioridad", DataRowVersion.Current);
                    AgregarParametro(ModifyColaServicioUDPCommand, IBD.ToParam("FechaPuestaEnCola"), DbType.DateTime, "FechaPuestaEnCola", DataRowVersion.Current);
                    AgregarParametro(ModifyColaServicioUDPCommand, IBD.ToParam("FechaProcesado"), DbType.DateTime, "FechaProcesado", DataRowVersion.Current);

                    ActualizarBaseDeDatos(addedAndModifiedDataSet, "ColaServicioUDP", InsertColaServicioUDPCommand, ModifyColaServicioUDPCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #region Actualizar tabla ColaTagsMyGnoss
                    DbCommand InsertColaTagsMyGnossCommand = ObtenerComando(sqlColaTagsMyGnossInsert);
                    AgregarParametro(InsertColaTagsMyGnossCommand, IBD.ToParam("TablaBaseProyectoID"), DbType.Int32, "TablaBaseProyectoID", DataRowVersion.Current);
                    AgregarParametro(InsertColaTagsMyGnossCommand, IBD.ToParam("UsuarioID"), IBD.TipoGuidToObject(DbType.Guid), "UsuarioID", DataRowVersion.Current);
                    AgregarParametro(InsertColaTagsMyGnossCommand, IBD.ToParam("Tags"), DbType.String, "Tags", DataRowVersion.Current);
                    AgregarParametro(InsertColaTagsMyGnossCommand, IBD.ToParam("Tipo"), DbType.Int16, "Tipo", DataRowVersion.Current);
                    AgregarParametro(InsertColaTagsMyGnossCommand, IBD.ToParam("TipoEnMyGnoss"), DbType.Int16, "TipoEnMyGnoss", DataRowVersion.Current);
                    AgregarParametro(InsertColaTagsMyGnossCommand, IBD.ToParam("Estado"), DbType.Int16, "Estado", DataRowVersion.Current);
                    AgregarParametro(InsertColaTagsMyGnossCommand, IBD.ToParam("FechaPuestaEnCola"), DbType.DateTime, "FechaPuestaEnCola", DataRowVersion.Current);
                    AgregarParametro(InsertColaTagsMyGnossCommand, IBD.ToParam("FechaProcesado"), DbType.DateTime, "FechaProcesado", DataRowVersion.Current);
                    AgregarParametro(InsertColaTagsMyGnossCommand, IBD.ToParam("Prioridad"), DbType.Int16, "Prioridad", DataRowVersion.Current);

                    DbCommand ModifyColaTagsMyGnossCommand = ObtenerComando(sqlColaTagsMyGnossModify);
                    //AgregarParametro(ModifyColaTagsMyGnossCommand, IBD.ToParam("Original_OrdenEjecucion"), DbType.Int32, "OrdenEjecucion", DataRowVersion.Original);
                    AgregarParametro(ModifyColaTagsMyGnossCommand, IBD.ToParam("Original_TablaBaseProyectoID"), DbType.Int32, "TablaBaseProyectoID", DataRowVersion.Original);
                    AgregarParametro(ModifyColaTagsMyGnossCommand, IBD.ToParam("Original_UsuarioID"), IBD.TipoGuidToObject(DbType.Guid), "UsuarioID", DataRowVersion.Original);
                    AgregarParametro(ModifyColaTagsMyGnossCommand, IBD.ToParam("Original_Tags"), DbType.String, "Tags", DataRowVersion.Original);
                    AgregarParametro(ModifyColaTagsMyGnossCommand, IBD.ToParam("Original_Tipo"), DbType.Int16, "Tipo", DataRowVersion.Original);
                    AgregarParametro(ModifyColaTagsMyGnossCommand, IBD.ToParam("Original_TipoEnMyGnoss"), DbType.Int16, "TipoEnMyGnoss", DataRowVersion.Original);
                    AgregarParametro(ModifyColaTagsMyGnossCommand, IBD.ToParam("Original_Estado"), DbType.Int16, "Estado", DataRowVersion.Original);
                    AgregarParametro(ModifyColaTagsMyGnossCommand, IBD.ToParam("Original_FechaPuestaEnCola"), DbType.DateTime, "FechaPuestaEnCola", DataRowVersion.Original);
                    AgregarParametro(ModifyColaTagsMyGnossCommand, IBD.ToParam("Original_Prioridad"), DbType.Int16, "Prioridad", DataRowVersion.Original);

                    //AgregarParametro(ModifyColaTagsMyGnossCommand, IBD.ToParam("OrdenEjecucion"), DbType.Int32, "OrdenEjecucion", DataRowVersion.Current);
                    AgregarParametro(ModifyColaTagsMyGnossCommand, IBD.ToParam("TablaBaseProyectoID"), DbType.Int32, "TablaBaseProyectoID", DataRowVersion.Current);
                    AgregarParametro(ModifyColaTagsMyGnossCommand, IBD.ToParam("UsuarioID"), IBD.TipoGuidToObject(DbType.Guid), "UsuarioID", DataRowVersion.Current);
                    AgregarParametro(ModifyColaTagsMyGnossCommand, IBD.ToParam("Tags"), DbType.String, "Tags", DataRowVersion.Current);
                    AgregarParametro(ModifyColaTagsMyGnossCommand, IBD.ToParam("Tipo"), DbType.Int16, "Tipo", DataRowVersion.Current);
                    AgregarParametro(ModifyColaTagsMyGnossCommand, IBD.ToParam("TipoEnMyGnoss"), DbType.Int16, "TipoEnMyGnoss", DataRowVersion.Current);
                    AgregarParametro(ModifyColaTagsMyGnossCommand, IBD.ToParam("Estado"), DbType.Int16, "Estado", DataRowVersion.Current);
                    AgregarParametro(ModifyColaTagsMyGnossCommand, IBD.ToParam("FechaPuestaEnCola"), DbType.DateTime, "FechaPuestaEnCola", DataRowVersion.Current);
                    AgregarParametro(ModifyColaTagsMyGnossCommand, IBD.ToParam("FechaProcesado"), DbType.DateTime, "FechaProcesado", DataRowVersion.Current);
                    AgregarParametro(ModifyColaTagsMyGnossCommand, IBD.ToParam("Prioridad"), DbType.Int16, "Prioridad", DataRowVersion.Current);
                    ActualizarBaseDeDatos(addedAndModifiedDataSet, "ColaTagsMyGnoss", InsertColaTagsMyGnossCommand, ModifyColaTagsMyGnossCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #region Actualizar tabla REC_BR_PER_PUBLICOS
                    DbCommand InsertREC_BR_PER_PUBLICOSCommand = ObtenerComando(sqlREC_BR_PER_PUBLICOSInsert);
                    AgregarParametro(InsertREC_BR_PER_PUBLICOSCommand, IBD.ToParam("Tag1"), DbType.String, "Tag1", DataRowVersion.Current);
                    AgregarParametro(InsertREC_BR_PER_PUBLICOSCommand, IBD.ToParam("Tag2"), DbType.String, "Tag2", DataRowVersion.Current);
                    AgregarParametro(InsertREC_BR_PER_PUBLICOSCommand, IBD.ToParam("Tipo"), DbType.Int16, "Tipo", DataRowVersion.Current);
                    AgregarParametro(InsertREC_BR_PER_PUBLICOSCommand, IBD.ToParam("CercaniaDirecta"), DbType.Int32, "CercaniaDirecta", DataRowVersion.Current);
                    AgregarParametro(InsertREC_BR_PER_PUBLICOSCommand, IBD.ToParam("CercaniaIndirecta"), DbType.Int32, "CercaniaIndirecta", DataRowVersion.Current);

                    DbCommand ModifyREC_BR_PER_PUBLICOSCommand = ObtenerComando(sqlREC_BR_PER_PUBLICOSModify);
                    AgregarParametro(ModifyREC_BR_PER_PUBLICOSCommand, IBD.ToParam("Original_Tag1"), DbType.String, "Tag1", DataRowVersion.Original);
                    AgregarParametro(ModifyREC_BR_PER_PUBLICOSCommand, IBD.ToParam("Original_Tag2"), DbType.String, "Tag2", DataRowVersion.Original);
                    AgregarParametro(ModifyREC_BR_PER_PUBLICOSCommand, IBD.ToParam("Original_Tipo"), DbType.Int16, "Tipo", DataRowVersion.Original);
                    AgregarParametro(ModifyREC_BR_PER_PUBLICOSCommand, IBD.ToParam("Original_CercaniaDirecta"), DbType.Int32, "CercaniaDirecta", DataRowVersion.Original);
                    AgregarParametro(ModifyREC_BR_PER_PUBLICOSCommand, IBD.ToParam("Original_CercaniaIndirecta"), DbType.Int32, "CercaniaIndirecta", DataRowVersion.Original);

                    AgregarParametro(ModifyREC_BR_PER_PUBLICOSCommand, IBD.ToParam("Tag1"), DbType.String, "Tag1", DataRowVersion.Current);
                    AgregarParametro(ModifyREC_BR_PER_PUBLICOSCommand, IBD.ToParam("Tag2"), DbType.String, "Tag2", DataRowVersion.Current);
                    AgregarParametro(ModifyREC_BR_PER_PUBLICOSCommand, IBD.ToParam("Tipo"), DbType.Int16, "Tipo", DataRowVersion.Current);
                    AgregarParametro(ModifyREC_BR_PER_PUBLICOSCommand, IBD.ToParam("CercaniaDirecta"), DbType.Int32, "CercaniaDirecta", DataRowVersion.Current);
                    AgregarParametro(ModifyREC_BR_PER_PUBLICOSCommand, IBD.ToParam("CercaniaIndirecta"), DbType.Int32, "CercaniaIndirecta", DataRowVersion.Current);
                    ActualizarBaseDeDatos(addedAndModifiedDataSet, "REC_BR_PER_PUBLICOS", InsertREC_BR_PER_PUBLICOSCommand, ModifyREC_BR_PER_PUBLICOSCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #region Actualizar tabla REC_PRIVADOS_COM_USU
                    DbCommand InsertREC_PRIVADOS_COM_USUCommand = ObtenerComando(sqlREC_PRIVADOS_COM_USUInsert);
                    AgregarParametro(InsertREC_PRIVADOS_COM_USUCommand, IBD.ToParam("Tag1"), DbType.String, "Tag1", DataRowVersion.Current);
                    AgregarParametro(InsertREC_PRIVADOS_COM_USUCommand, IBD.ToParam("Tag2"), DbType.String, "Tag2", DataRowVersion.Current);
                    AgregarParametro(InsertREC_PRIVADOS_COM_USUCommand, IBD.ToParam("Tipo"), DbType.Int16, "Tipo", DataRowVersion.Current);
                    AgregarParametro(InsertREC_PRIVADOS_COM_USUCommand, IBD.ToParam("CercaniaDirecta"), DbType.Int32, "CercaniaDirecta", DataRowVersion.Current);
                    AgregarParametro(InsertREC_PRIVADOS_COM_USUCommand, IBD.ToParam("CercaniaIndirecta"), DbType.Int32, "CercaniaIndirecta", DataRowVersion.Current);
                    AgregarParametro(InsertREC_PRIVADOS_COM_USUCommand, IBD.ToParam("UsuarioID"), IBD.TipoGuidToObject(DbType.Guid), "UsuarioID", DataRowVersion.Current);

                    DbCommand ModifyREC_PRIVADOS_COM_USUCommand = ObtenerComando(sqlREC_PRIVADOS_COM_USUModify);
                    AgregarParametro(ModifyREC_PRIVADOS_COM_USUCommand, IBD.ToParam("Original_Tag1"), DbType.String, "Tag1", DataRowVersion.Original);
                    AgregarParametro(ModifyREC_PRIVADOS_COM_USUCommand, IBD.ToParam("Original_Tag2"), DbType.String, "Tag2", DataRowVersion.Original);
                    AgregarParametro(ModifyREC_PRIVADOS_COM_USUCommand, IBD.ToParam("Original_Tipo"), DbType.Int16, "Tipo", DataRowVersion.Original);
                    AgregarParametro(ModifyREC_PRIVADOS_COM_USUCommand, IBD.ToParam("Original_CercaniaDirecta"), DbType.Int32, "CercaniaDirecta", DataRowVersion.Original);
                    AgregarParametro(ModifyREC_PRIVADOS_COM_USUCommand, IBD.ToParam("Original_CercaniaIndirecta"), DbType.Int32, "CercaniaIndirecta", DataRowVersion.Original);
                    AgregarParametro(ModifyREC_PRIVADOS_COM_USUCommand, IBD.ToParam("Original_UsuarioID"), IBD.TipoGuidToObject(DbType.Guid), "UsuarioID", DataRowVersion.Original);

                    AgregarParametro(ModifyREC_PRIVADOS_COM_USUCommand, IBD.ToParam("Tag1"), DbType.String, "Tag1", DataRowVersion.Current);
                    AgregarParametro(ModifyREC_PRIVADOS_COM_USUCommand, IBD.ToParam("Tag2"), DbType.String, "Tag2", DataRowVersion.Current);
                    AgregarParametro(ModifyREC_PRIVADOS_COM_USUCommand, IBD.ToParam("Tipo"), DbType.Int16, "Tipo", DataRowVersion.Current);
                    AgregarParametro(ModifyREC_PRIVADOS_COM_USUCommand, IBD.ToParam("CercaniaDirecta"), DbType.Int32, "CercaniaDirecta", DataRowVersion.Current);
                    AgregarParametro(ModifyREC_PRIVADOS_COM_USUCommand, IBD.ToParam("CercaniaIndirecta"), DbType.Int32, "CercaniaIndirecta", DataRowVersion.Current);
                    AgregarParametro(ModifyREC_PRIVADOS_COM_USUCommand, IBD.ToParam("UsuarioID"), IBD.TipoGuidToObject(DbType.Guid), "UsuarioID", DataRowVersion.Current);
                    ActualizarBaseDeDatos(addedAndModifiedDataSet, "REC_PRIVADOS_COM_USU", InsertREC_PRIVADOS_COM_USUCommand, ModifyREC_PRIVADOS_COM_USUCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #region Actualizar tabla REC_PUBLICOS_MYGNOSS
                    DbCommand InsertREC_PUBLICOS_MYGNOSSCommand = ObtenerComando(sqlREC_PUBLICOS_MYGNOSSInsert);
                    AgregarParametro(InsertREC_PUBLICOS_MYGNOSSCommand, IBD.ToParam("Tag1"), DbType.String, "Tag1", DataRowVersion.Current);
                    AgregarParametro(InsertREC_PUBLICOS_MYGNOSSCommand, IBD.ToParam("Tag2"), DbType.String, "Tag2", DataRowVersion.Current);
                    AgregarParametro(InsertREC_PUBLICOS_MYGNOSSCommand, IBD.ToParam("Tipo"), DbType.Int16, "Tipo", DataRowVersion.Current);
                    AgregarParametro(InsertREC_PUBLICOS_MYGNOSSCommand, IBD.ToParam("CercaniaDirecta"), DbType.Int32, "CercaniaDirecta", DataRowVersion.Current);
                    AgregarParametro(InsertREC_PUBLICOS_MYGNOSSCommand, IBD.ToParam("CercaniaIndirecta"), DbType.Int32, "CercaniaIndirecta", DataRowVersion.Current);

                    DbCommand ModifyREC_PUBLICOS_MYGNOSSCommand = ObtenerComando(sqlREC_PUBLICOS_MYGNOSSModify);
                    AgregarParametro(ModifyREC_PUBLICOS_MYGNOSSCommand, IBD.ToParam("Original_Tag1"), DbType.String, "Tag1", DataRowVersion.Original);
                    AgregarParametro(ModifyREC_PUBLICOS_MYGNOSSCommand, IBD.ToParam("Original_Tag2"), DbType.String, "Tag2", DataRowVersion.Original);
                    AgregarParametro(ModifyREC_PUBLICOS_MYGNOSSCommand, IBD.ToParam("Original_Tipo"), DbType.Int16, "Tipo", DataRowVersion.Original);
                    AgregarParametro(ModifyREC_PUBLICOS_MYGNOSSCommand, IBD.ToParam("Original_CercaniaDirecta"), DbType.Int32, "CercaniaDirecta", DataRowVersion.Original);
                    AgregarParametro(ModifyREC_PUBLICOS_MYGNOSSCommand, IBD.ToParam("Original_CercaniaIndirecta"), DbType.Int32, "CercaniaIndirecta", DataRowVersion.Original);

                    AgregarParametro(ModifyREC_PUBLICOS_MYGNOSSCommand, IBD.ToParam("Tag1"), DbType.String, "Tag1", DataRowVersion.Current);
                    AgregarParametro(ModifyREC_PUBLICOS_MYGNOSSCommand, IBD.ToParam("Tag2"), DbType.String, "Tag2", DataRowVersion.Current);
                    AgregarParametro(ModifyREC_PUBLICOS_MYGNOSSCommand, IBD.ToParam("Tipo"), DbType.Int16, "Tipo", DataRowVersion.Current);
                    AgregarParametro(ModifyREC_PUBLICOS_MYGNOSSCommand, IBD.ToParam("CercaniaDirecta"), DbType.Int32, "CercaniaDirecta", DataRowVersion.Current);
                    AgregarParametro(ModifyREC_PUBLICOS_MYGNOSSCommand, IBD.ToParam("CercaniaIndirecta"), DbType.Int32, "CercaniaIndirecta", DataRowVersion.Current);
                    ActualizarBaseDeDatos(addedAndModifiedDataSet, "REC_PUBLICOS_MYGNOSS", InsertREC_PUBLICOS_MYGNOSSCommand, ModifyREC_PUBLICOS_MYGNOSSCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #region Actualizar tabla ColaGrafoDbpedia

                    DbCommand InsertColaGrafoDbpediaCommand = ObtenerComando(sqlColaGrafoDbpediaInsert);

                    AgregarParametro(InsertColaGrafoDbpediaCommand, IBD.ToParam("GnossID"), DbType.Guid, "GnossID", DataRowVersion.Current);
                    AgregarParametro(InsertColaGrafoDbpediaCommand, IBD.ToParam("UriDbpedia"), DbType.String, "UriDbpedia", DataRowVersion.Current);
                    AgregarParametro(InsertColaGrafoDbpediaCommand, IBD.ToParam("ProyectoID"), DbType.Guid, "ProyectoID", DataRowVersion.Current);
                    AgregarParametro(InsertColaGrafoDbpediaCommand, IBD.ToParam("Accion"), DbType.Int16, "Accion", DataRowVersion.Current);
                    AgregarParametro(InsertColaGrafoDbpediaCommand, IBD.ToParam("Estado"), DbType.Int16, "Estado", DataRowVersion.Current);
                    AgregarParametro(InsertColaGrafoDbpediaCommand, IBD.ToParam("Prioridad"), DbType.Int16, "Prioridad", DataRowVersion.Current);
                    AgregarParametro(InsertColaGrafoDbpediaCommand, IBD.ToParam("FechaPuestaEnCola"), DbType.DateTime, "FechaPuestaEnCola", DataRowVersion.Current);
                    AgregarParametro(InsertColaGrafoDbpediaCommand, IBD.ToParam("TipoRecurso"), DbType.String, "TipoRecurso", DataRowVersion.Current);
                    AgregarParametro(InsertColaGrafoDbpediaCommand, IBD.ToParam("FechaProcesado"), DbType.DateTime, "FechaProcesado", DataRowVersion.Current);

                    DbCommand ModifyColaGrafoDbpediaCommand = ObtenerComando(sqlColaGrafoDbpediaModify);
                    AgregarParametro(ModifyColaGrafoDbpediaCommand, IBD.ToParam("Original_GnossID"), DbType.Guid, "GnossID", DataRowVersion.Original);
                    AgregarParametro(ModifyColaGrafoDbpediaCommand, IBD.ToParam("Original_UriDbpedia"), DbType.String, "UriDbpedia", DataRowVersion.Original);
                    AgregarParametro(ModifyColaGrafoDbpediaCommand, IBD.ToParam("Original_Accion"), DbType.Int16, "Accion", DataRowVersion.Original);

                    AgregarParametro(ModifyColaGrafoDbpediaCommand, IBD.ToParam("GnossID"), DbType.Guid, "GnossID", DataRowVersion.Current);
                    AgregarParametro(ModifyColaGrafoDbpediaCommand, IBD.ToParam("UriDbpedia"), DbType.String, "UriDbpedia", DataRowVersion.Current);
                    AgregarParametro(ModifyColaGrafoDbpediaCommand, IBD.ToParam("ProyectoID"), DbType.Guid, "ProyectoID", DataRowVersion.Current);
                    AgregarParametro(ModifyColaGrafoDbpediaCommand, IBD.ToParam("Accion"), DbType.Int16, "Accion", DataRowVersion.Current);
                    AgregarParametro(ModifyColaGrafoDbpediaCommand, IBD.ToParam("Estado"), DbType.Int16, "Estado", DataRowVersion.Current);
                    AgregarParametro(ModifyColaGrafoDbpediaCommand, IBD.ToParam("Prioridad"), DbType.Int16, "Prioridad", DataRowVersion.Current);
                    AgregarParametro(ModifyColaGrafoDbpediaCommand, IBD.ToParam("FechaPuestaEnCola"), DbType.DateTime, "FechaPuestaEnCola", DataRowVersion.Current);
                    AgregarParametro(ModifyColaGrafoDbpediaCommand, IBD.ToParam("TipoRecurso"), DbType.String, "TipoRecurso", DataRowVersion.Current);
                    AgregarParametro(ModifyColaGrafoDbpediaCommand, IBD.ToParam("FechaProcesado"), DbType.DateTime, "FechaProcesado", DataRowVersion.Current);

                    ActualizarBaseDeDatos(addedAndModifiedDataSet, "ColaGrafoDbpedia", InsertColaGrafoDbpediaCommand, ModifyColaGrafoDbpediaCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    if (mTablaBaseProyectoID > -1) //Si no hay proyectoID, no es necesario actualizar estas tablas
                    {
                        #region Actualizar tabla COM_USU_PRIV

                        DbCommand InsertCOM_USU_PRIVCommand = ObtenerComando(sqlCOM_USU_PRIVInsert);

                        AgregarParametro(InsertCOM_USU_PRIVCommand, IBD.ToParam("Tag1"), DbType.String, "Tag1", DataRowVersion.Current);
                        AgregarParametro(InsertCOM_USU_PRIVCommand, IBD.ToParam("Tag2"), DbType.String, "Tag2", DataRowVersion.Current);
                        AgregarParametro(InsertCOM_USU_PRIVCommand, IBD.ToParam("Tipo"), DbType.Int16, "Tipo", DataRowVersion.Current);
                        AgregarParametro(InsertCOM_USU_PRIVCommand, IBD.ToParam("CercaniaDirecta"), DbType.Int32, "CercaniaDirecta", DataRowVersion.Current);
                        AgregarParametro(InsertCOM_USU_PRIVCommand, IBD.ToParam("CercaniaIndirecta"), DbType.Int32, "CercaniaIndirecta", DataRowVersion.Current);
                        AgregarParametro(InsertCOM_USU_PRIVCommand, IBD.ToParam("IdentidadID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadID", DataRowVersion.Current);

                        DbCommand ModifyCOM_USU_PRIVCommand = ObtenerComando(sqlCOM_USU_PRIVModify);
                        AgregarParametro(ModifyCOM_USU_PRIVCommand, IBD.ToParam("O_Tag1"), DbType.String, "Tag1", DataRowVersion.Original);
                        AgregarParametro(ModifyCOM_USU_PRIVCommand, IBD.ToParam("O_Tag2"), DbType.String, "Tag2", DataRowVersion.Original);
                        AgregarParametro(ModifyCOM_USU_PRIVCommand, IBD.ToParam("O_Tipo"), DbType.Int16, "Tipo", DataRowVersion.Original);
                        AgregarParametro(ModifyCOM_USU_PRIVCommand, IBD.ToParam("O_CercaniaDirecta"), DbType.Int32, "CercaniaDirecta", DataRowVersion.Original);
                        AgregarParametro(ModifyCOM_USU_PRIVCommand, IBD.ToParam("O_CercaniaIndirecta"), DbType.Int32, "CercaniaIndirecta", DataRowVersion.Original);
                        AgregarParametro(ModifyCOM_USU_PRIVCommand, IBD.ToParam("O_IdentidadID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadID", DataRowVersion.Original);

                        AgregarParametro(ModifyCOM_USU_PRIVCommand, IBD.ToParam("Tag1"), DbType.String, "Tag1", DataRowVersion.Current);
                        AgregarParametro(ModifyCOM_USU_PRIVCommand, IBD.ToParam("Tag2"), DbType.String, "Tag2", DataRowVersion.Current);
                        AgregarParametro(ModifyCOM_USU_PRIVCommand, IBD.ToParam("Tipo"), DbType.Int16, "Tipo", DataRowVersion.Current);
                        AgregarParametro(ModifyCOM_USU_PRIVCommand, IBD.ToParam("CercaniaDirecta"), DbType.Int32, "CercaniaDirecta", DataRowVersion.Current);
                        AgregarParametro(ModifyCOM_USU_PRIVCommand, IBD.ToParam("CercaniaIndirecta"), DbType.Int32, "CercaniaIndirecta", DataRowVersion.Current);
                        AgregarParametro(ModifyCOM_USU_PRIVCommand, IBD.ToParam("IdentidadID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadID", DataRowVersion.Current);

                        ActualizarBaseDeDatos(addedAndModifiedDataSet, "COM_USU_PRIV", InsertCOM_USU_PRIVCommand, ModifyCOM_USU_PRIVCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                        #endregion

                        #region Actualizar tabla COMUNIDAD

                        DbCommand InsertCOMUNIDADCommand = ObtenerComando(sqlCOMUNIDADInsert);

                        AgregarParametro(InsertCOMUNIDADCommand, IBD.ToParam("Tag1"), DbType.String, "Tag1", DataRowVersion.Current);
                        AgregarParametro(InsertCOMUNIDADCommand, IBD.ToParam("Tag2"), DbType.String, "Tag2", DataRowVersion.Current);
                        AgregarParametro(InsertCOMUNIDADCommand, IBD.ToParam("Tipo"), DbType.Int16, "Tipo", DataRowVersion.Current);
                        AgregarParametro(InsertCOMUNIDADCommand, IBD.ToParam("CercaniaDirecta"), DbType.Int32, "CercaniaDirecta", DataRowVersion.Current);
                        AgregarParametro(InsertCOMUNIDADCommand, IBD.ToParam("CercaniaIndirecta"), DbType.Int32, "CercaniaIndirecta", DataRowVersion.Current);

                        DbCommand ModifyCOMUNIDADCommand = ObtenerComando(sqlCOMUNIDADModify);
                        AgregarParametro(ModifyCOMUNIDADCommand, IBD.ToParam("O_Tag1"), DbType.String, "Tag1", DataRowVersion.Original);
                        AgregarParametro(ModifyCOMUNIDADCommand, IBD.ToParam("O_Tag2"), DbType.String, "Tag2", DataRowVersion.Original);
                        AgregarParametro(ModifyCOMUNIDADCommand, IBD.ToParam("O_Tipo"), DbType.Int16, "Tipo", DataRowVersion.Original);
                        AgregarParametro(ModifyCOMUNIDADCommand, IBD.ToParam("O_CercaniaDirecta"), DbType.Int32, "CercaniaDirecta", DataRowVersion.Original);
                        AgregarParametro(ModifyCOMUNIDADCommand, IBD.ToParam("O_CercaniaIndirecta"), DbType.Int32, "CercaniaIndirecta", DataRowVersion.Original);

                        AgregarParametro(ModifyCOMUNIDADCommand, IBD.ToParam("Tag1"), DbType.String, "Tag1", DataRowVersion.Current);
                        AgregarParametro(ModifyCOMUNIDADCommand, IBD.ToParam("Tag2"), DbType.String, "Tag2", DataRowVersion.Current);
                        AgregarParametro(ModifyCOMUNIDADCommand, IBD.ToParam("Tipo"), DbType.Int16, "Tipo", DataRowVersion.Current);
                        AgregarParametro(ModifyCOMUNIDADCommand, IBD.ToParam("CercaniaDirecta"), DbType.Int32, "CercaniaDirecta", DataRowVersion.Current);
                        AgregarParametro(ModifyCOMUNIDADCommand, IBD.ToParam("CercaniaIndirecta"), DbType.Int32, "CercaniaIndirecta", DataRowVersion.Current);

                        ActualizarBaseDeDatos(addedAndModifiedDataSet, "COMUNIDAD", InsertCOMUNIDADCommand, ModifyCOMUNIDADCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                        #endregion
                    }
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
        /// <param name="pIBD">Objecto IBaseDatos para el archivo pasado al constructor del AD</param>
        private void CargarConsultasYDataAdapters(IBaseDatos pIBD)
        {
            #region Consultas

            this.sqlSelectColaTagsComunidades = "SELECT ColaTagsComunidades.OrdenEjecucion, ColaTagsComunidades.TablaBaseProyectoID, ColaTagsComunidades.Tags, ColaTagsComunidades.Tipo, ColaTagsComunidades.Estado, ColaTagsComunidades.FechaPuestaEnCola, ColaTagsComunidades.FechaProcesado , ColaTagsComunidades.Prioridad, ColaTagsComunidades.EstadoTags, ColaTagsComunidades.EstadoCargaID, ColaTagsComunidades.TipoAccionCarga  FROM ColaTagsComunidades ";

            this.sqlSelectColaTagsComunidadesSearch = "SELECT ColaTagsComunidadesSearch.OrdenEjecucion, ColaTagsComunidadesSearch.TablaBaseProyectoID, ColaTagsComunidadesSearch.Tags, ColaTagsComunidadesSearch.Tipo, ColaTagsComunidadesSearch.Estado, ColaTagsComunidadesSearch.FechaPuestaEnCola, ColaTagsComunidadesSearch.FechaProcesado , ColaTagsComunidadesSearch.Prioridad, ColaTagsComunidadesSearch.EstadoTags, ColaTagsComunidadesSearch.EstadoCargaID, ColaTagsComunidadesSearch.TipoAccionCarga  FROM ColaTagsComunidadesSearch ";

            this.sqlSelectColaTagsComPriv = "SELECT ColaTagsComPriv.OrdenEjecucion, ColaTagsComPriv.TablaBaseProyectoID, " + IBD.CargarGuid("ColaTagsComPriv.IdentidadID") + " , ColaTagsComPriv.Tags, ColaTagsComPriv.Tipo, ColaTagsComPriv.Estado, ColaTagsComPriv.FechaPuestaEnCola, ColaTagsComPriv.FechaProcesado, ColaTagsComPriv.Prioridad FROM ColaTagsComPriv ";

            this.sqlSelectCOM_USU_PRIV = "SELECT " + mNombreTablaCOM_USU_PRIV + ".Tag1, " + mNombreTablaCOM_USU_PRIV + ".Tag2, " + mNombreTablaCOM_USU_PRIV + ".Tipo, " + mNombreTablaCOM_USU_PRIV + ".CercaniaDirecta, " + mNombreTablaCOM_USU_PRIV + ".CercaniaIndirecta, " + IBD.CargarGuid(mNombreTablaCOM_USU_PRIV + "." + CampoIdentidadID) + " FROM " + mNombreTablaCOM_USU_PRIV + " ";

            this.sqlSelectCOMUNIDAD = "SELECT " + mNombreTablaCOMUNIDADES + ".Tag1, " + mNombreTablaCOMUNIDADES + ".Tag2, " + mNombreTablaCOMUNIDADES + ".Tipo, " + mNombreTablaCOMUNIDADES + ".CercaniaDirecta, " + mNombreTablaCOMUNIDADES + ".CercaniaIndirecta FROM " + mNombreTablaCOMUNIDADES + " ";

            //this.sqlSelectColaServicioUDP = "SELECT ColaServicioUDP.OrdenEjecucion, ColaServicioUDP.Consulta,  ColaServicioUDP.Estado, ColaServicioUDP.FechaPuestaEnCola, ColaServicioUDP.FechaProcesado, ColaServicioUDP.Prioridad FROM ColaServicioUDP";

            this.sqlSelectColaTagsMyGnoss = "SELECT ColaTagsMyGnoss.OrdenEjecucion, ColaTagsMyGnoss.TablaBaseProyectoID, " + IBD.CargarGuid("ColaTagsMyGnoss.UsuarioID") + ", ColaTagsMyGnoss.Tags, ColaTagsMyGnoss.Tipo, ColaTagsMyGnoss.TipoEnMyGnoss, ColaTagsMyGnoss.Estado, ColaTagsMyGnoss.FechaPuestaEnCola, ColaTagsMyGnoss.FechaProcesado, ColaTagsMyGnoss.Prioridad FROM ColaTagsMyGnoss";
            this.sqlSelectREC_BR_PER_PUBLICOS = "SELECT REC_BR_PER_PUBLICOS.Tag1, REC_BR_PER_PUBLICOS.Tag2, REC_BR_PER_PUBLICOS.Tipo, REC_BR_PER_PUBLICOS.CercaniaDirecta, REC_BR_PER_PUBLICOS.CercaniaIndirecta FROM REC_BR_PER_PUBLICOS";
            this.sqlSelectREC_PRIVADOS_COM_USU = "SELECT REC_PRIVADOS_COM_USU.Tag1, REC_PRIVADOS_COM_USU.Tag2, REC_PRIVADOS_COM_USU.Tipo, REC_PRIVADOS_COM_USU.CercaniaDirecta, REC_PRIVADOS_COM_USU.CercaniaIndirecta, " + IBD.CargarGuid("REC_PRIVADOS_COM_USU.UsuarioID") + " FROM REC_PRIVADOS_COM_USU";
            this.sqlSelectREC_PUBLICOS_MYGNOSS = "SELECT REC_PUBLICOS_MYGNOSS.Tag1, REC_PUBLICOS_MYGNOSS.Tag2, REC_PUBLICOS_MYGNOSS.Tipo, REC_PUBLICOS_MYGNOSS.CercaniaDirecta, REC_PUBLICOS_MYGNOSS.CercaniaIndirecta FROM REC_PUBLICOS_MYGNOSS";

            #endregion

            #region DataAdapter

            #region ColaTagsComunidades

            this.sqlColaTagsComunidadesInsert = IBD.ReplaceParam("INSERT INTO ColaTagsComunidades (TablaBaseProyectoID, Tags, Tipo, Estado, FechaPuestaEnCola, FechaProcesado,Prioridad, EstadoTags, EstadoCargaID, TipoAccionCarga) VALUES (@TablaBaseProyectoID, @Tags, @Tipo, @Estado, @FechaPuestaEnCola, @FechaProcesado,@Prioridad, @EstadoTags, @EstadoCargaID, @TipoAccionCarga)");

            this.sqlColaTagsComunidadesDelete = IBD.ReplaceParam("DELETE FROM ColaTagsComunidades WHERE (OrdenEjecucion = @O_OrdenEjecucion)");

            this.sqlColaTagsComunidadesModify = IBD.ReplaceParam("UPDATE ColaTagsComunidades SET TablaBaseProyectoID = @TablaBaseProyectoID, Tags = @Tags, Tipo = @Tipo, Estado = @Estado, FechaPuestaEnCola = @FechaPuestaEnCola, FechaProcesado = @FechaProcesado, Prioridad = @Prioridad, EstadoTags = @EstadoTags, EstadoCargaID = @EstadoCargaID, TipoAccionCarga = @TipoAccionCarga WHERE (OrdenEjecucion = @O_OrdenEjecucion)");

            #endregion

            #region ColaTagsComunidadesSearch

            this.sqlColaTagsComunidadesSearchInsert = IBD.ReplaceParam("INSERT INTO ColaTagsComunidadesSearch (TablaBaseProyectoID, Tags, Tipo, Estado, FechaPuestaEnCola, FechaProcesado,Prioridad, EstadoTags, EstadoCargaID, TipoAccionCarga) VALUES (@TablaBaseProyectoID, @Tags, @Tipo, @Estado, @FechaPuestaEnCola, @FechaProcesado,@Prioridad, @EstadoTags, @EstadoCargaID, @TipoAccionCarga)");

            this.sqlColaTagsComunidadesSearchDelete = IBD.ReplaceParam("DELETE FROM ColaTagsComunidadesSearch WHERE (OrdenEjecucion = @O_OrdenEjecucion)");

            this.sqlColaTagsComunidadesSearchModify = IBD.ReplaceParam("UPDATE ColaTagsComunidadesSearch SET TablaBaseProyectoID = @TablaBaseProyectoID, Tags = @Tags, Tipo = @Tipo, Estado = @Estado, FechaPuestaEnCola = @FechaPuestaEnCola, FechaProcesado = @FechaProcesado, Prioridad = @Prioridad, EstadoTags = @EstadoTags, EstadoCargaID = @EstadoCargaID, TipoAccionCarga = @TipoAccionCarga WHERE (OrdenEjecucion = @O_OrdenEjecucion)");

            #endregion

            #region ColaTagsComPriv

            this.sqlColaTagsComPrivInsert = IBD.ReplaceParam("INSERT INTO ColaTagsComPriv (TablaBaseProyectoID, IdentidadID, Tags, Tipo, Estado, FechaPuestaEnCola, FechaProcesado,Prioridad) VALUES (@TablaBaseProyectoID, " + pIBD.GuidParamColumnaTabla("IdentidadID") + ", @Tags, @Tipo, @Estado, @FechaPuestaEnCola, @FechaProcesado,@Prioridad)");

            this.sqlColaTagsComPrivDelete = IBD.ReplaceParam("DELETE FROM ColaTagsComPriv WHERE (OrdenEjecucion = @O_OrdenEjecucion) AND (TablaBaseProyectoID = @O_TablaBaseProyectoID) AND (IdentidadID = " + pIBD.GuidParamColumnaTabla("O_IdentidadID") + ") AND (Tags = @O_Tags) AND (Tipo = @O_Tipo) AND (Estado = @O_Estado) AND (FechaPuestaEnCola = @O_FechaPuestaEnCola) AND (FechaProcesado = @O_FechaProcesado OR @O_FechaProcesado IS NULL AND FechaProcesado IS NULL)");

            this.sqlColaTagsComPrivModify = IBD.ReplaceParam("UPDATE ColaTagsComPriv SET TablaBaseProyectoID = @TablaBaseProyectoID, IdentidadID = " + pIBD.GuidParamColumnaTabla("IdentidadID") + " , Tags = @Tags, Tipo = @Tipo, Estado = @Estado, FechaPuestaEnCola = @FechaPuestaEnCola, FechaProcesado = @FechaProcesado, Prioridad = @Prioridad WHERE (OrdenEjecucion = @O_OrdenEjecucion) AND (TablaBaseProyectoID = @O_TablaBaseProyectoID) AND (IdentidadID = " + pIBD.GuidParamColumnaTabla("O_IdentidadID") + ") AND (Tags = @O_Tags) AND (Tipo = @O_Tipo) AND (Estado = @O_Estado) AND (FechaPuestaEnCola = @O_FechaPuestaEnCola) AND (FechaProcesado = @O_FechaProcesado OR @O_FechaProcesado IS NULL AND FechaProcesado IS NULL)");

            #endregion

            #region COM_USU_PRIV

            this.sqlCOM_USU_PRIVInsert = IBD.ReplaceParam("INSERT INTO " + mNombreTablaCOM_USU_PRIV + " (Tag1, Tag2, Tipo, CercaniaDirecta, CercaniaIndirecta, IdentidadID) VALUES (@Tag1, @Tag2, @Tipo, @CercaniaDirecta, @CercaniaIndirecta, " + IBD.GuidParamColumnaTabla("IdentidadID") + ")");

            this.sqlCOM_USU_PRIVDelete = IBD.ReplaceParam("DELETE FROM " + mNombreTablaCOM_USU_PRIV + " WHERE (Tag1 = @O_Tag1) AND (Tag2 = @O_Tag2) AND (Tipo = @O_Tipo) AND (CercaniaDirecta = @O_CercaniaDirecta) AND (CercaniaIndirecta = @O_CercaniaIndirecta) AND (IdentidadID = " + IBD.GuidParamColumnaTabla("O_IdentidadID") + ")");

            this.sqlCOM_USU_PRIVModify = IBD.ReplaceParam("UPDATE " + mNombreTablaCOM_USU_PRIV + " SET Tag1 = @Tag1, Tag2 = @Tag2, Tipo = @Tipo, CercaniaDirecta = @CercaniaDirecta, CercaniaIndirecta = @CercaniaIndirecta, IdentidadID = " + IBD.GuidParamColumnaTabla("IdentidadID") + " WHERE (Tag1 = @O_Tag1) AND (Tag2 = @O_Tag2) AND (Tipo = @O_Tipo) AND (CercaniaDirecta = @O_CercaniaDirecta) AND (CercaniaIndirecta = @O_CercaniaIndirecta) AND (IdentidadID = " + IBD.GuidParamColumnaTabla("O_IdentidadID") + ")");

            #endregion

            #region COMUNIDAD

            this.sqlCOMUNIDADInsert = IBD.ReplaceParam("INSERT INTO " + mNombreTablaCOMUNIDADES + " (Tag1, Tag2, Tipo, CercaniaDirecta, CercaniaIndirecta) VALUES (@Tag1, @Tag2, @Tipo, @CercaniaDirecta, @CercaniaIndirecta)");

            this.sqlCOMUNIDADDelete = IBD.ReplaceParam("DELETE FROM " + mNombreTablaCOMUNIDADES + " WHERE (Tag1 = @O_Tag1) AND (Tag2 = @O_Tag2) AND (Tipo = @O_Tipo) AND (CercaniaDirecta = @O_CercaniaDirecta) AND (CercaniaIndirecta = @O_CercaniaIndirecta)");

            this.sqlCOMUNIDADModify = IBD.ReplaceParam("UPDATE " + mNombreTablaCOMUNIDADES + " SET Tag1 = @Tag1, Tag2 = @Tag2, Tipo = @Tipo, CercaniaDirecta = @CercaniaDirecta, CercaniaIndirecta = @CercaniaIndirecta WHERE (Tag1 = @O_Tag1) AND (Tag2 = @O_Tag2) AND (Tipo = @O_Tipo) AND (CercaniaDirecta = @O_CercaniaDirecta) AND (CercaniaIndirecta = @O_CercaniaIndirecta)");

            #endregion

            #region ColaServicioUDP
            this.sqlColaServicioUDPInsert = IBD.ReplaceParam("INSERT INTO ColaServicioUDP (Consulta, Estado, Prioridad, FechaPuestaEnCola, FechaProcesado) VALUES (@Consulta, @Estado, @Prioridad, @FechaPuestaEnCola, @FechaProcesado)");

            this.sqlColaServicioUDPDelete = IBD.ReplaceParam("DELETE FROM ColaServicioUDP WHERE (OrdenEjecucion = @Original_OrdenEjecucion)");

            this.sqlColaServicioUDPModify = IBD.ReplaceParam("UPDATE ColaServicioUDP SET Consulta = @Consulta, Estado = @Estado, Prioridad = @Prioridad, FechaPuestaEnCola = @FechaPuestaEnCola, FechaProcesado = @FechaProcesado  WHERE OrdenEjecucion = @Original_OrdenEjecucion");
            #endregion

            #region ColaTagsMyGnoss
            this.sqlColaTagsMyGnossInsert = IBD.ReplaceParam("INSERT INTO ColaTagsMyGnoss (TablaBaseProyectoID, UsuarioID, Tags, Tipo, TipoEnMyGnoss, Estado, FechaPuestaEnCola, FechaProcesado,Prioridad) VALUES (@TablaBaseProyectoID, " + IBD.GuidParamColumnaTabla("UsuarioID") + ", @Tags, @Tipo, @TipoEnMyGnoss, @Estado, @FechaPuestaEnCola, @FechaProcesado,@Prioridad)");
            this.sqlColaTagsMyGnossDelete = IBD.ReplaceParam("DELETE FROM ColaTagsMyGnoss WHERE (TablaBaseProyectoID = @Original_TablaBaseProyectoID) AND (UsuarioID = " + IBD.GuidParamColumnaTabla("Original_UsuarioID") + " OR " + IBD.GuidParamColumnaTabla("Original_UsuarioID") + " IS NULL AND UsuarioID IS NULL) AND (Tags = @Original_Tags) AND (Tipo = @Original_Tipo) AND (TipoEnMyGnoss = @Original_TipoEnMyGnoss) AND (Estado = @Original_Estado) AND (FechaPuestaEnCola = @Original_FechaPuestaEnCola) AND (FechaProcesado = @Original_FechaProcesado OR @Original_FechaProcesado IS NULL AND FechaProcesado IS NULL)");
            this.sqlColaTagsMyGnossModify = IBD.ReplaceParam("UPDATE ColaTagsMyGnoss SET TablaBaseProyectoID = @TablaBaseProyectoID, UsuarioID = " + IBD.GuidParamColumnaTabla("UsuarioID") + ", Tags = @Tags, Tipo = @Tipo, TipoEnMyGnoss = @TipoEnMyGnoss, Estado = @Estado, FechaPuestaEnCola = @FechaPuestaEnCola, FechaProcesado = @FechaProcesado, Prioridad = @Prioridad WHERE (TablaBaseProyectoID = @Original_TablaBaseProyectoID) AND (UsuarioID = " + IBD.GuidParamColumnaTabla("Original_UsuarioID") + " OR " + IBD.GuidParamColumnaTabla("Original_UsuarioID") + " IS NULL AND UsuarioID IS NULL) AND (Tags = @Original_Tags) AND (Tipo = @Original_Tipo) AND (TipoEnMyGnoss = @Original_TipoEnMyGnoss) AND (Estado = @Original_Estado) AND (FechaPuestaEnCola = @Original_FechaPuestaEnCola) AND (FechaProcesado = @Original_FechaProcesado OR @Original_FechaProcesado IS NULL AND FechaProcesado IS NULL)");
            #endregion

            #region REC_BR_PER_PUBLICOS
            this.sqlREC_BR_PER_PUBLICOSInsert = IBD.ReplaceParam("INSERT INTO REC_BR_PER_PUBLICOS (Tag1, Tag2, Tipo, CercaniaDirecta, CercaniaIndirecta) VALUES (@Tag1, @Tag2, @Tipo, @CercaniaDirecta, @CercaniaIndirecta)");
            this.sqlREC_BR_PER_PUBLICOSDelete = IBD.ReplaceParam("DELETE FROM REC_BR_PER_PUBLICOS WHERE (Tag1 = @Original_Tag1) AND (Tag2 = @Original_Tag2) AND (Tipo = @Original_Tipo) AND (CercaniaDirecta = @Original_CercaniaDirecta) AND (CercaniaIndirecta = @Original_CercaniaIndirecta)");
            this.sqlREC_BR_PER_PUBLICOSModify = IBD.ReplaceParam("UPDATE REC_BR_PER_PUBLICOS SET Tag1 = @Tag1, Tag2 = @Tag2, Tipo = @Tipo, CercaniaDirecta = @CercaniaDirecta, CercaniaIndirecta = @CercaniaIndirecta WHERE (Tag1 = @Original_Tag1) AND (Tag2 = @Original_Tag2) AND (Tipo = @Original_Tipo) AND (CercaniaDirecta = @Original_CercaniaDirecta) AND (CercaniaIndirecta = @Original_CercaniaIndirecta)");
            #endregion

            #region REC_PRIVADOS_COM_USU
            this.sqlREC_PRIVADOS_COM_USUInsert = IBD.ReplaceParam("INSERT INTO REC_PRIVADOS_COM_USU (Tag1, Tag2, Tipo, CercaniaDirecta, CercaniaIndirecta, UsuarioID) VALUES (@Tag1, @Tag2, @Tipo, @CercaniaDirecta, @CercaniaIndirecta, " + IBD.GuidParamColumnaTabla("UsuarioID") + ")");
            this.sqlREC_PRIVADOS_COM_USUDelete = IBD.ReplaceParam("DELETE FROM REC_PRIVADOS_COM_USU WHERE (Tag1 = @Original_Tag1) AND (Tag2 = @Original_Tag2) AND (Tipo = @Original_Tipo) AND (CercaniaDirecta = @Original_CercaniaDirecta) AND (CercaniaIndirecta = @Original_CercaniaIndirecta) AND (UsuarioID = " + IBD.GuidParamColumnaTabla("Original_UsuarioID") + ")");
            this.sqlREC_PRIVADOS_COM_USUModify = IBD.ReplaceParam("UPDATE REC_PRIVADOS_COM_USU SET Tag1 = @Tag1, Tag2 = @Tag2, Tipo = @Tipo, CercaniaDirecta = @CercaniaDirecta, CercaniaIndirecta = @CercaniaIndirecta, UsuarioID = " + IBD.GuidParamColumnaTabla("UsuarioID") + " WHERE (Tag1 = @Original_Tag1) AND (Tag2 = @Original_Tag2) AND (Tipo = @Original_Tipo) AND (CercaniaDirecta = @Original_CercaniaDirecta) AND (CercaniaIndirecta = @Original_CercaniaIndirecta) AND (UsuarioID = " + IBD.GuidParamColumnaTabla("Original_UsuarioID") + ")");
            #endregion

            #region REC_PUBLICOS_MYGNOSS
            this.sqlREC_PUBLICOS_MYGNOSSInsert = IBD.ReplaceParam("INSERT INTO REC_PUBLICOS_MYGNOSS (Tag1, Tag2, Tipo, CercaniaDirecta, CercaniaIndirecta) VALUES (@Tag1, @Tag2, @Tipo, @CercaniaDirecta, @CercaniaIndirecta)");
            this.sqlREC_PUBLICOS_MYGNOSSDelete = IBD.ReplaceParam("DELETE FROM REC_PUBLICOS_MYGNOSS WHERE (Tag1 = @Original_Tag1) AND (Tag2 = @Original_Tag2) AND (Tipo = @Original_Tipo) AND (CercaniaDirecta = @Original_CercaniaDirecta) AND (CercaniaIndirecta = @Original_CercaniaIndirecta)");
            this.sqlREC_PUBLICOS_MYGNOSSModify = IBD.ReplaceParam("UPDATE REC_PUBLICOS_MYGNOSS SET Tag1 = @Tag1, Tag2 = @Tag2, Tipo = @Tipo, CercaniaDirecta = @CercaniaDirecta, CercaniaIndirecta = @CercaniaIndirecta WHERE (Tag1 = @Original_Tag1) AND (Tag2 = @Original_Tag2) AND (Tipo = @Original_Tipo) AND (CercaniaDirecta = @Original_CercaniaDirecta) AND (CercaniaIndirecta = @Original_CercaniaIndirecta)");
            #endregion

            #region ColaGrafoDbpedia

            this.sqlColaGrafoDbpediaInsert = IBD.ReplaceParam("INSERT INTO ColaGrafoDbpedia (GnossID, UriDbpedia, ProyectoID, Accion, Estado, Prioridad, FechaPuestaEnCola, FechaProcesado, TipoRecurso) VALUES (@GnossID, @UriDbpedia, @ProyectoID, @Accion, @Estado, @Prioridad, @FechaPuestaEnCola, @FechaProcesado, @TipoRecurso)");

            this.sqlColaGrafoDbpediaDelete = IBD.ReplaceParam("DELETE FROM ColaGrafoDbpedia WHERE (GnossID = @Original_GnossID) AND (UriDbpedia = @Original_UriDbpedia) AND (Accion = @Original_Accion)");

            this.sqlColaGrafoDbpediaModify = IBD.ReplaceParam("UPDATE ColaTagsComunidades SET GnossID = @GnossID, UriDbpedia = @UriDbpedia, ProyectoID = @ProyectoID, Accion = @Accion, Estado = @Estado, Prioridad = @Prioridad, FechaPuestaEnCola = @FechaPuestaEnCola, FechaProcesado = @FechaProcesado, TipoRecurso = @TipoRecurso  WHERE (GnossID = @Original_GnossID) AND (UriDbpedia = @Original_UriDbpedia) AND (Accion = @Original_Accion)");

            #endregion

            #endregion
        }

        #endregion

        #region Métodos de colas

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

        public void InsertarColaTagsComunidadesLinkedData(BaseRecursosComunidadDS pDataSet)
        {
            InsertarFilasEnRabbit("ColaTagsComunidadesLinkedData", pDataSet, "ColaTagsComunidades");
        }

        public void InsertarColaTagsComunidadesSearch(BaseRecursosComunidadDS.ColaTagsComunidadesRow pFilaCola)
        {
            string comando = $"INSERT INTO  colatagscomunidadesSearch(TablaBaseProyectoID, Tags, Tipo, Estado, FechaPuestaEnCola, FechaProcesado, Prioridad, EstadoTags, EstadoCargaID, TipoAccionCarga) VALUES ({pFilaCola.TablaBaseProyectoID}, '{pFilaCola.Tags}', {pFilaCola.Tipo}, {pFilaCola.Estado}, {IBD.ToParam("FechaPuestaEnCola")}, {IBD.ToParam("FechaProcesado")}, {pFilaCola.Prioridad}, {pFilaCola.EstadoTags}, {IBD.ToParam("EstadoCargaID")}, {IBD.ToParam("TipoAccionCarga")})";
            DbCommand commandsqlconsulta = ObtenerComando(comando);
            AgregarParametro(commandsqlconsulta, IBD.ToParam("FechaPuestaEnCola"), DbType.DateTime, pFilaCola.FechaPuestaEnCola);
            if (pFilaCola.IsFechaProcesadoNull())
            {
                AgregarParametro(commandsqlconsulta, IBD.ToParam("FechaProcesado"), DbType.DateTime, DBNull.Value);
            }
            else
            {
                AgregarParametro(commandsqlconsulta, IBD.ToParam("FechaProcesado"), DbType.DateTime, pFilaCola.FechaProcesado);
            }

            if (pFilaCola.IsEstadoCargaIDNull())
            {
                AgregarParametro(commandsqlconsulta, IBD.ToParam("EstadoCargaID"), DbType.Int64, DBNull.Value);
            }
            else
            {
                AgregarParametro(commandsqlconsulta, IBD.ToParam("EstadoCargaID"), DbType.Int64, pFilaCola.EstadoCargaID);
            }

            if (pFilaCola.IsTipoAccionCargaNull())
            {
                AgregarParametro(commandsqlconsulta, IBD.ToParam("TipoAccionCarga"), DbType.Int64, DBNull.Value);
            }
            else
            {
                AgregarParametro(commandsqlconsulta, IBD.ToParam("TipoAccionCarga"), DbType.Int64, pFilaCola.TipoAccionCarga);
            }
            ActualizarBaseDeDatos(commandsqlconsulta);
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
            List<TiposElementosEnCola> listaTiposElementos = null;
            if (pTiposElementos.HasValue)
            {
                listaTiposElementos = new List<TiposElementosEnCola>() { pTiposElementos.Value };
            }

            return ObtenerElementosColaPendientes(listaTiposElementos, pEstadoInferior, pEstadoSuperior, pNumMaxItems, pSoloPrioridad0);
        }

        /// <summary>
        /// Obtiene los elementos pendientes de la cola de tags
        /// </summary>
        /// <returns>Devuelve un dataset con los elementos de la cola cargados</returns>
        /// <param name="pEstadoInferior">Estado mínimo de los elementos de la cola</param>
        /// <param name="pEstadoSuperior">Estado máximo de los elementos de la cola</param>
        /// <param name="pTiposElementos">Lista de los tipo de elementos a obtener (Agregado, eliminado, ...). Null para obtener todos</param>
        /// <param name="pNumMaxItems">Numero máximo de items a traer</param>
        public override DataSet ObtenerElementosColaPendientes(List<TiposElementosEnCola> pTiposElementos, EstadosColaTags pEstadoInferior, EstadosColaTags pEstadoSuperior, int pNumMaxItems, bool? pSoloPrioridad0)
        {
            BaseRecursosComunidadDS brComDS = new BaseRecursosComunidadDS();

            StringBuilder andTipoElemento = new StringBuilder();

            if (pTiposElementos != null && pTiposElementos.Count > 0)
            {
                string coma = "";
                andTipoElemento.Append(" AND Tipo IN ( ");
                foreach (TiposElementosEnCola tipoElemento in pTiposElementos)
                {
                    andTipoElemento.Append(coma + (short)tipoElemento);
                    coma = ", ";
                }
                andTipoElemento.Append(")");
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

            string consultaComunidades = sqlSelectColaTagsComunidades.Replace("SELECT ", "SELECT top(" + pNumMaxItems + ") ") + " WHERE Estado >= " + (short)pEstadoInferior + " AND Estado <= " + (short)pEstadoSuperior + andTipoElemento.ToString() + andPrioridad + " ORDER BY Prioridad ASC, OrdenEjecucion ASC";

            DbCommand cmdObtnerElementosColaPendientesComunidades = ObtenerComando(consultaComunidades);
            CargarDataSet(cmdObtnerElementosColaPendientesComunidades, brComDS, "ColaTagsComunidades");

            return brComDS;
        }

        /// <summary>
        /// Obtiene los elementos pendientes de la cola de tags
        /// </summary>
        /// <returns>Devuelve un dataset con los elementos de la cola cargados</returns>
        /// <param name="pEstadoInferior">Estado mínimo de los elementos de la cola</param>
        public BaseRecursosComunidadDS ObtenerElementosColaSearch(EstadosColaTags pEstadoInferior, EstadosColaTags pEstadoSuperior)
        {
            BaseRecursosComunidadDS brComDS = new BaseRecursosComunidadDS();

            StringBuilder andTipoElemento = new StringBuilder();

            string consultaComunidades = sqlSelectColaTagsComunidadesSearch.Replace("SELECT ", "SELECT top(1) ") + " WHERE Estado >= " + (short)pEstadoInferior + " AND Estado <= " + (short)pEstadoSuperior + andTipoElemento.ToString() + " ORDER BY Prioridad ASC, OrdenEjecucion ASC";

            DbCommand cmdObtnerElementosColaPendientesComunidades = ObtenerComando(consultaComunidades);
            CargarDataSet(cmdObtnerElementosColaPendientesComunidades, brComDS, "ColaTagsComunidadesSearch");

            return brComDS;
        }

        /// <summary>
        /// Obtiene los elementos pendientes de la cola de tags para procesar tags.
        /// </summary>
        /// <param name="pNumMaxItems">Numero máximo de items a traer</param>
        /// <returns>Devuelve un dataset con los elementos de la cola cargados</returns>
        public BaseRecursosComunidadDS ObtenerElementosColaPendientesProcesarTags(int pNumMaxItems)
        {
            BaseRecursosComunidadDS brComDS = new BaseRecursosComunidadDS();

            string consultaComunidades = sqlSelectColaTagsComunidades.Replace("SELECT ", "SELECT top(" + pNumMaxItems + ") ") + " WHERE Estado >= " + (short)EstadosColaTags.Procesado + " AND EstadoTags < " + (short)EstadosColaTags.Reintento2 + " ORDER BY Prioridad ASC, OrdenEjecucion ASC";

            DbCommand cmdObtnerElementosColaPendientesComunidades = ObtenerComando(consultaComunidades);
            CargarDataSet(cmdObtnerElementosColaPendientesComunidades, brComDS, "ColaTagsComunidades");

            return brComDS;
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
            DbCommand cmdComprobarNumeroElementosFallidosEnXHoras = ObtenerComando("SELECT count(*) FROM ColaTagsComunidades WHERE Estado >= " + (short)pEstadoInferior + " AND Estado <= " + (short)pEstadoSuperior + "  AND DATEADD(HH, 24, fechaprocesado)>GETDATE()");
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

        /// <summary>
        /// Comprueba si existe ya una entrada en la tabla ColaGrafoDbpedia una lista de uris dbpedia para un recurso
        /// </summary>
        /// <param name="pDocumentoID">Identificador del recurso</param>
        /// <param name="pUrisDbpedia">Lista de urls de entidades de dbpedia a comprobar</param>
        /// <param name="pAccion">Accion de la cola que se quiere comprobar</param>
        /// <returns></returns>
        public List<string> ExistenEntidadesParaRecursoEnColaGrafoDbpedia(Guid pDocumentoID, List<string> pUrisDbpedia, int pAccion)
        {
            List<string> listaEntidadesExistentes = new List<string>();

            if (pUrisDbpedia.Count > 0)
            {
                DbCommand comando = ObtenerComando("SELECT UriDbpedia FROM ColaGrafoDbpedia WHERE GnossID = " + IBD.GuidValor(pDocumentoID) + " AND Accion = " + pAccion + " AND Estado = 0 AND UriDbpedia ");

                if (pUrisDbpedia.Count == 1)
                {
                    comando.CommandText += "= " + IBD.ToParam("uriDbpedia");
                    AgregarParametro(comando, IBD.ToParam("uriDbpedia"), DbType.String, pUrisDbpedia[0]);
                }
                else
                {
                    comando.CommandText += "IN (";

                    int contador = 0;
                    string coma = "";
                    foreach (string uriDbpedia in pUrisDbpedia)
                    {
                        string parametro = IBD.ToParam("uriDbpedia" + contador++);
                        comando.CommandText += coma + parametro;
                        coma = ", ";
                        AgregarParametro(comando, parametro, DbType.String, uriDbpedia);
                    }

                    comando.CommandText += ")";
                }

                IDataReader reader = null;

                try
                {
                    reader = EjecutarReader(comando);

                    while (reader.Read())
                    {
                        listaEntidadesExistentes.Add(reader.GetString(0));
                    }
                }
                finally
                {
                    if (reader != null)
                    {
                        reader.Close();
                        reader.Dispose();
                        reader = null;
                    }
                }
            }

            return listaEntidadesExistentes;
        }

        #endregion

        #region Verificación de existencia y creación de tablas

        /// <summary>
        /// Comprueba si existe la tabla de comunidades. Si no existe la crea 
        /// </summary>
        /// <param name="pCrearTablaSiNoExiste">Verdad si se debe crear la tabla en caso de que no exista</param>
        /// <param name="pTipoConsulta">Tipo de consulta que se va a realizar</param>
        /// <returns>Verdad si la tabla existe (o ha sido recién creada).</returns>
        public override bool VerificarExisteTabla(TiposConsultaObtenerTags pTipoConsulta, bool pCrearTablaSiNoExiste)
        {
            bool existeTabla = false;

            if (pTipoConsulta.Equals(TiposConsultaObtenerTags.RecursosComunidad))
            {
                existeTabla = VerificarExisteTabla(mNombreTablaCOMUNIDADES);
            }
            else if (pTipoConsulta.Equals(TiposConsultaObtenerTags.RecursosComunidadPrivada))
            {
                existeTabla = VerificarExisteTabla(mNombreTablaCOM_USU_PRIV);
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
            if (pTipoConsulta.Equals(TiposConsultaObtenerTags.RecursosComunidad))
            {
                CrearTabla("CREATE TABLE [dbo]." + mNombreTablaCOMUNIDADES + "([Tag1] [nvarchar](1000) COLLATE Modern_Spanish_CI_AI NOT NULL, [Tag2] [nvarchar](1000) COLLATE Modern_Spanish_CI_AI NOT NULL,	[Tipo] [smallint] NOT NULL,	[CercaniaDirecta] [int] NOT NULL, [CercaniaIndirecta] [int] NOT NULL) ON [PRIMARY]");
            }
            else if (pTipoConsulta.Equals(TiposConsultaObtenerTags.RecursosComunidadPrivada))
            {
                CrearTabla("CREATE TABLE [dbo]." + mNombreTablaCOM_USU_PRIV + "([Tag1] [nvarchar](1000) COLLATE Modern_Spanish_CI_AI NOT NULL, [Tag2] [nvarchar](1000) COLLATE Modern_Spanish_CI_AI NOT NULL, [Tipo] [smallint] NOT NULL, [CercaniaDirecta] [int] NOT NULL, [CercaniaIndirecta] [int] NOT NULL,	[IdentidadID] [uniqueidentifier] NOT NULL) ON [PRIMARY]");
            }
            else
            {
                base.CrearTabla(pTipoConsulta);
            }
        }

        #endregion

        #region Override

        /// <summary>
        /// Obtiene el sql correspondiente a un tipo de consulta
        /// </summary>
        /// <param name="pTipoConsulta">Tipo de consulta</param>
        /// <returns></returns>
        protected internal override string ObtenerNombreTablaPorTipoConsulta(TiposConsultaObtenerTags pTipoConsulta)
        {
            string nombreTabla = "";

            switch (pTipoConsulta)
            {
                case TiposConsultaObtenerTags.RecursosComunidad:
                case TiposConsultaObtenerTags.RecursosComunidadPrivada:
                    nombreTabla = mNombreTablaCOMUNIDADES;
                    break;
                case TiposConsultaObtenerTags.RecursosComunidadSoloPrivados:
                    nombreTabla = mNombreTablaCOM_USU_PRIV;
                    break;
            }
            return nombreTabla;
        }

        /// <summary>
        /// Obtiene el sql correspondiente a un tipo de consulta
        /// </summary>
        /// <param name="pTipoConsulta">Tipo de consulta</param>
        /// <returns></returns>
        protected override string ObtenerNombreTablaPorTipoConsultaConIDTags(TiposConsultaObtenerTags pTipoConsulta)
        {
            string nombreTabla = "";

            switch (pTipoConsulta)
            {
                case TiposConsultaObtenerTags.RecursosComunidad:
                    nombreTabla = mNombreTablaCOMUNIDADES;
                    break;
                case TiposConsultaObtenerTags.RecursosComunidadPrivada:
                    nombreTabla = mNombreTablaCOM_USU_PRIV;
                    break;
                case TiposConsultaObtenerTags.RecursosComunidadSoloPrivados:
                    nombreTabla = mNombreTablaCOM_USU_PRIV;
                    break;
            }
            return nombreTabla;
        }

        /// <summary>
        /// Obtiene el nombre correcto de la tabla en la base de datos
        /// </summary>
        /// <param name="pNombreTabla">Nombre de la tabla en el DataSet</param>
        /// <returns></returns>
        protected override string ObtenerNombreCorrectoTabla(string pNombreTabla)
        {
            switch (pNombreTabla)
            {
                case "COM_USU_PRIV":
                    return NombreTablaCOM_USU_PRIV;
                case "COMUNIDAD":
                    return NombreTablaCOMUNIDADES;
                default:
                    return base.ObtenerNombreCorrectoTabla(pNombreTabla);
            }
        }

        #endregion

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene el nombre real de la tabla que deberemos consultar para los tags de los recursos de una coomunidad pubñica en funcion del identificador numerico del proyecto
        /// </summary>
        public string NombreTablaCOMUNIDADES
        {
            get
            {
                return mNombreTablaCOMUNIDADES;
            }
            set
            {
                this.mNombreTablaCOMUNIDADES = value;
            }
        }

        /// <summary>
        /// Obtiene el nombre real de la tabla que deberemos consultar para los tags de los recursos privados por usuario de una coomunidad en funcion del identificador numerico del proyecto
        /// </summary>
        public string NombreTablaCOM_USU_PRIV
        {
            get
            {
                return mNombreTablaCOM_USU_PRIV;
            }
        }

        #region MyGnoss

        /// <summary>
        /// Obtiene el nombre real de la tabla que deberemos consultar para los tags de los recursos privados por usuario de una coomunidad en funcion del identificador numerico del proyecto
        /// </summary>
        public string NombreTablaREC_BR_PER_PUBLICOS
        {
            get
            {
                return mNombreTablaREC_BR_PER_PUBLICOS;
            }
        }

        /// <summary>
        /// Obtiene el nombre real de la tabla que deberemos consultar para los tags de los recursos privados por usuario de una coomunidad en funcion del identificador numerico del proyecto
        /// </summary>
        public string NombreTablaREC_PRIVADOS_COM_USU
        {
            get
            {
                return mNombreTablaREC_PRIVADOS_COM_USU;
            }
        }

        /// <summary>
        /// Obtiene el nombre real de la tabla que deberemos consultar para los tags de los recursos privados por usuario de una coomunidad en funcion del identificador numerico del proyecto
        /// </summary>
        public string NombreTablaREC_PUBLICOS_MYGNOSS
        {
            get
            {
                return mNombreTablaREC_PUBLICOS_MYGNOSS;
            }
        }

        #endregion

        /// <summary>
        /// Obtiene o establece un valor que indica, en caso que estemos consultando las tablas de una comunidad concreta, si tiene creada la tabla de documentos privados o no
        /// </summary>
        public bool ExisteTablaPrivada
        {
            get
            {
                return mExisteTablaPrivada;
            }
            set
            {
                mExisteTablaPrivada = value;
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
