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
    /// AD del modelo Base para personas y organizaciones
    /// </summary>
    public class BasePerOrgComunidadAD : BaseComunidadAD
    {
        #region Constantes

        /// <summary>
        ///Constante para codificar los tags de tipo Nombre de una persona
        /// </summary>
        public const string NOMBRE_PER_COMPLETO = "##NOM_PER##";

        /// <summary>
        ///Constante para codificar los tags de tipo Nombre de una persona
        /// </summary>
        public const string NOMBRE_PER_SIN_APP = "##NSA_PER##";

        /// <summary>
        ///Constante para codificar los tags de tipo Nombre de una persona
        /// </summary>
        public const string APELLIDOS_PER = "##APP_PER##";

        /// <summary>
        ///Constante para codificar los tags de tipo Nombre de una persona
        /// </summary>
        public const string PAIS_PER = "##PAIS_PER##";

        /// <summary>
        ///Constante para codificar los tags de tipo Nombre de una persona
        /// </summary>
        public const string PROVINCIA_PER = "##PROV_PER##";

        /// <summary>
        ///Constante para codificar los tags de tipo Nombre de una persona
        /// </summary>
        public const string COMUNIDAD_PER = "##COM_PER##";

        /// <summary>
        ///Constante para codificar el origen del tag, de persona o de organización
        /// </summary>
        public const string PERS_U_ORG = "##PERS-ORG##";

        /// <summary>
        ///Constante para codificar los tags de tipo nombre de una organización
        /// </summary>
        public const string NOMBRE_ORG = "##NOM_ORG##";

        /// <summary>
        ///Constante para codificar los tags de tipo url de una organización
        /// </summary>
        public const string URL_ORG = "##URL_ORG##";

        /// <summary>
        ///Constante para codificar los tags de tipo:  tipo de una organización
        /// </summary>
        public const string TIPO_ORG = "##TIPO_ORG##";

        /// <summary>
        ///Constante para codificar los tags de tipo sector de una organización
        /// </summary>
        public const string SEC_ORG = "##SEC_ORG##";

        /// <summary>
        ///Constante para codificar los tags de tipo Nombre de una persona
        /// </summary>
        public const string PAIS_ORG = "##PAIS_ORG##";

        /// <summary>
        ///Constante para codificar los tags de tipo Nombre de una persona
        /// </summary>
        public const string PROVINCIA_ORG = "##PROV_ORG##";

        /// <summary>
        ///Constante para codificar los tags de tipo Nombre de una persona
        /// </summary>
        public const string COMUNIDAD_ORG = "##COM_ORG##";

        /// <summary>
        ///Constante para codificar los tags de tipo Nombre de una persona
        /// </summary>
        public const string ALIAS_ORG = "##ALIAS_ORG##";

        /// <summary>
        ///Constante para codificar los tags de tipo Nombre de una persona
        /// </summary>
        public const string TAG_DESCOMPUESTO = "##DESC##";

        /// <summary>
        ///Constante para codificar los tags de tipo ID-Tag de persona
        /// </summary>
        public const string ID_TAG_PER = "##ID_TAG_PER##";


        #endregion

        #region Miembros

        /// <summary>
        /// identificador numerico de la organización
        /// </summary>
        private int mTablaBaseOrganizacionID;

        #endregion

        #region Constructor

        /// <summary>
        /// Cuando se desea pasar directamente la ruta del fichero de configuracion de conexion a la Base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Fichero de configuración de la base de datos BASE</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        /// <param name="pTablaBaseProyectoID">Identificador numerico del proyecto (-1 si no se va a actualizar tablas de proyecto)</param>
        /// <param name="pTablaBaseOrganizacionID">Identificador numerico de la organización (-1 si no se va a actualizar tablas de proyecto)</param>
        public BasePerOrgComunidadAD(string pFicheroConfiguracionBD, int pTablaBaseProyectoID, int pTablaBaseOrganizacionID, LoggingService loggingService, EntityContext entityContext, EntityContextBASE entityContextBASE, ConfigService configService)
            : base(pTablaBaseProyectoID, loggingService, entityContext, entityContextBASE, configService)
        {
            mTablaBaseOrganizacionID = pTablaBaseOrganizacionID;
            mNombreTablaCOM_PER_ORG = "COM_PER_ORG";
            mNombreTablaCOM_PER_ORG_VI = "COM_PER_ORG_VI";

            if (pTablaBaseProyectoID > -1)
            {
                mNombreTablaCOM_PER_ORG = "COM_PER_ORG_0000000000000".Substring(0, 25 - mTablaBaseProyectoID.ToString().Length) + mTablaBaseProyectoID.ToString();
                mNombreTablaCOM_PER_ORG_VI = "COM_PER_ORG_VI_0000000000".Substring(0, 25 - mTablaBaseProyectoID.ToString().Length) + mTablaBaseProyectoID.ToString();
            }

            if (pTablaBaseOrganizacionID > -1)
            {
                mNombreTablaCOM_X_ORG_X = "COM_00000000".Substring(0, 12 - mTablaBaseProyectoID.ToString().Length) + mTablaBaseProyectoID.ToString() + "_" + "ORG_00000000".Substring(0, 12 - pTablaBaseOrganizacionID.ToString().Length) + pTablaBaseOrganizacionID.ToString();

            }
            this.CargarConsultasYDataAdapters(IBD);
        }

        #endregion

        #region Consultas

        private string sqlSelectColaTagsCom_Per_Org;
        private string sqlSelectCOM_PER_ORG;
        private string sqlSelectCOM_PER_ORG_VI;
        private string sqlSelectCOM_X_ORG_X;

        #endregion

        #region DataAdapter

        #region ColaTagsCom_Per_Org
        private string sqlColaTagsCom_Per_OrgInsert;
        private string sqlColaTagsCom_Per_OrgDelete;
        private string sqlColaTagsCom_Per_OrgModify;
        #endregion

        #region ColaTagsCom_Per_Org_Vi
        private string sqlColaTagsCom_Per_Org_ViInsert;
        private string sqlColaTagsCom_Per_Org_Vielete;
        private string sqlColaTagsCom_Per_Org_ViModify;
        #endregion

        #region ColaTagsCOM_X_ORG_X
        private string sqlColaTagsCOM_X_ORG_XDelete;
        private string sqlColaTagsCOM_X_ORG_XInsert;
        private string sqlColaTagsCOM_X_ORG_XModify;
        #endregion

        #region COM_PER_ORG
        private string sqlCOM_PER_ORGInsert;
        private string sqlCOM_PER_ORGDelete;
        private string sqlCOM_PER_ORGModify;
        #endregion

        #region COM_PER_ORG_VI
        private string sqlCOM_PER_ORG_VIInsert;
        private string sqlCOM_PER_ORG_VIelete;
        private string sqlCOM_PER_ORG_VIModify;
        #endregion

        #region COM_X_ORG_X
        private string sqlCOM_X_ORG_XInsert;
        private string sqlCOM_X_ORG_XDelete;
        private string sqlCOM_X_ORG_XModify;
        #endregion

        #endregion

        #region Métodos generales

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
            BasePerOrgComunidadDS brPerOrgDS = new BasePerOrgComunidadDS();

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

            string consultaPerOrgCom = sqlSelectColaTagsCom_Per_Org.Replace("SELECT ", "SELECT top(" + pNumMaxItems + ") ") + " WHERE Estado >= " + (short)pEstadoInferior + " AND Estado <= " + (short)pEstadoSuperior + andTipoElemento + andPrioridad + " ORDER BY Prioridad ASC, OrdenEjecucion ASC";

            DbCommand cmdObtnerElementosColaPendientesPerOrgCom = ObtenerComando(consultaPerOrgCom);
            CargarDataSet(cmdObtnerElementosColaPendientesPerOrgCom, brPerOrgDS, "ColaTagsCom_Per_Org");

            return brPerOrgDS;
        }

        /// <summary>
        /// Obtiene los elementos pendientes de la cola de tags para procesar tags.
        /// </summary>
        /// <param name="pNumMaxItems">Numero máximo de items a traer</param>
        /// <returns>Devuelve un dataset con los elementos de la cola cargados</returns>
        public BasePerOrgComunidadDS ObtenerElementosColaPendientesProcesarTags(int pNumMaxItems)
        {
            BasePerOrgComunidadDS brPerOrgDS = new BasePerOrgComunidadDS();

            string consultaPerOrgCom = sqlSelectColaTagsCom_Per_Org.Replace("select ", "select top(" + pNumMaxItems + ") ") + " WHERE Estado >= " + (short)EstadosColaTags.Procesado + " AND EstadoTags < " + (short)EstadosColaTags.Reintento2 + " ORDER BY Prioridad ASC, OrdenEjecucion ASC";

            DbCommand cmdObtnerElementosColaPendientesComunidades = ObtenerComando(consultaPerOrgCom);
            CargarDataSet(cmdObtnerElementosColaPendientesComunidades, brPerOrgDS, "ColaTagsCom_Per_Org");

            return brPerOrgDS;
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
            DbCommand cmdComprobarNumeroElementosFallidosEnXHoras = ObtenerComando("SELECT count(*) FROM ColaTagsCom_Per_Org  WHERE Estado >= " + (short)pEstadoInferior + " AND Estado <= " + (short)pEstadoSuperior + "  AND DATEADD(HH, 24, fechaprocesado)>GETDATE()");
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

        #region Métodos AD

        /// <summary>
        /// Elimina los elementos borrados del dataset pasado como parámetro
        /// </summary>
        /// <param name="pDataSet">Dataset de eliminados</param>
        public override void EliminarBorrados(DataSet pDataSet)
        {
            try
            {
                DataSet deletedDataSet;
                deletedDataSet = pDataSet.GetChanges(DataRowState.Deleted);

                if (deletedDataSet != null)
                {
                    #region Deleted

                    if (mTablaBaseProyectoID > -1) //Si no hay proyectoID, no es necesario actualizar estas tablas
                    {

                        #region Eliminar tabla COM_PER_ORG
                        DbCommand DeleteCOM_PER_ORGCommand = ObtenerComando(sqlCOM_PER_ORGDelete);
                        AgregarParametro(DeleteCOM_PER_ORGCommand, IBD.ToParam("O_Tag1"), DbType.String, "Tag1", DataRowVersion.Original);
                        AgregarParametro(DeleteCOM_PER_ORGCommand, IBD.ToParam("O_Tag2"), DbType.String, "Tag2", DataRowVersion.Original);
                        AgregarParametro(DeleteCOM_PER_ORGCommand, IBD.ToParam("O_Tipo"), DbType.Int16, "Tipo", DataRowVersion.Original);
                        AgregarParametro(DeleteCOM_PER_ORGCommand, IBD.ToParam("O_CercaniaDirecta"), DbType.Int32, "CercaniaDirecta", DataRowVersion.Original);
                        AgregarParametro(DeleteCOM_PER_ORGCommand, IBD.ToParam("O_CercaniaIndirecta"), DbType.Int32, "CercaniaIndirecta", DataRowVersion.Original);
                        ActualizarBaseDeDatos(deletedDataSet, "COM_PER_ORG", null, null, DeleteCOM_PER_ORGCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                        #endregion

                        #region Eliminar tabla COM_PER_ORG_VI
                        DbCommand DeleteCOM_PER_ORG_VICommand = ObtenerComando(sqlCOM_PER_ORG_VIelete);
                        AgregarParametro(DeleteCOM_PER_ORG_VICommand, IBD.ToParam("O_Tag1"), DbType.String, "Tag1", DataRowVersion.Original);
                        AgregarParametro(DeleteCOM_PER_ORG_VICommand, IBD.ToParam("O_Tag2"), DbType.String, "Tag2", DataRowVersion.Original);
                        AgregarParametro(DeleteCOM_PER_ORG_VICommand, IBD.ToParam("O_Tipo"), DbType.Int16, "Tipo", DataRowVersion.Original);
                        AgregarParametro(DeleteCOM_PER_ORG_VICommand, IBD.ToParam("O_CercaniaDirecta"), DbType.Int32, "CercaniaDirecta", DataRowVersion.Original);
                        AgregarParametro(DeleteCOM_PER_ORG_VICommand, IBD.ToParam("O_CercaniaIndirecta"), DbType.Int32, "CercaniaIndirecta", DataRowVersion.Original);
                        ActualizarBaseDeDatos(deletedDataSet, "COM_PER_ORG_VI", null, null, DeleteCOM_PER_ORG_VICommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                        #endregion
                    }

                    if (mTablaBaseOrganizacionID > -1) //Si no hay organizacionID, no es necesario actualizar estas tablas
                    {

                        #region Eliminar tabla COM_X_ORG_X
                        DbCommand DeleteCOM_X_ORG_XCommand = ObtenerComando(sqlCOM_X_ORG_XDelete);
                        AgregarParametro(DeleteCOM_X_ORG_XCommand, IBD.ToParam("O_Tag1"), DbType.String, "Tag1", DataRowVersion.Original);
                        AgregarParametro(DeleteCOM_X_ORG_XCommand, IBD.ToParam("O_Tag2"), DbType.String, "Tag2", DataRowVersion.Original);
                        AgregarParametro(DeleteCOM_X_ORG_XCommand, IBD.ToParam("O_Tipo"), DbType.Int16, "Tipo", DataRowVersion.Original);
                        AgregarParametro(DeleteCOM_X_ORG_XCommand, IBD.ToParam("O_CercaniaDirecta"), DbType.Int32, "CercaniaDirecta", DataRowVersion.Original);
                        AgregarParametro(DeleteCOM_X_ORG_XCommand, IBD.ToParam("O_CercaniaIndirecta"), DbType.Int32, "CercaniaIndirecta", DataRowVersion.Original);
                        ActualizarBaseDeDatos(deletedDataSet, "COM_X_ORG_X", null, null, DeleteCOM_X_ORG_XCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                        #endregion
                    }


                    #region Eliminar tabla ColaTagsCom_Per_Org
                    DbCommand DeleteColaTagsCom_Per_OrgCommand = ObtenerComando(sqlColaTagsCom_Per_OrgDelete);
                    AgregarParametro(DeleteColaTagsCom_Per_OrgCommand, IBD.ToParam("O_OrdenEjecucion"), DbType.Int32, "OrdenEjecucion", DataRowVersion.Original);

                    ActualizarBaseDeDatos(deletedDataSet, "ColaTagsCom_Per_Org", null, null, DeleteColaTagsCom_Per_OrgCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #region Eliminar tabla ColaTagsCom_Per_Org_Vi
                    DbCommand DeleteColaTagsCom_Per_Org_ViCommand = ObtenerComando(sqlColaTagsCom_Per_Org_Vielete);
                    AgregarParametro(DeleteColaTagsCom_Per_Org_ViCommand, IBD.ToParam("O_TablaBaseProyectoID"), DbType.Int32, "TablaBaseProyectoID", DataRowVersion.Original);
                    AgregarParametro(DeleteColaTagsCom_Per_Org_ViCommand, IBD.ToParam("O_Tags"), DbType.String, "Tags", DataRowVersion.Original);
                    AgregarParametro(DeleteColaTagsCom_Per_Org_ViCommand, IBD.ToParam("O_Tipo"), DbType.Int16, "Tipo", DataRowVersion.Original);
                    AgregarParametro(DeleteColaTagsCom_Per_Org_ViCommand, IBD.ToParam("O_Estado"), DbType.Int16, "Estado", DataRowVersion.Original);
                    AgregarParametro(DeleteColaTagsCom_Per_Org_ViCommand, IBD.ToParam("O_FechaPuestaEnCola"), DbType.DateTime, "FechaPuestaEnCola", DataRowVersion.Original);
                    AgregarParametro(DeleteColaTagsCom_Per_Org_ViCommand, IBD.ToParam("O_FechaProcesado"), DbType.DateTime, "FechaProcesado", DataRowVersion.Original);
                    AgregarParametro(DeleteColaTagsCom_Per_Org_ViCommand, IBD.ToParam("O_Prioridad"), DbType.Int16, "Prioridad", DataRowVersion.Original);
                    ActualizarBaseDeDatos(deletedDataSet, "ColaTagsCom_Per_Org_Vi", null, null, DeleteColaTagsCom_Per_Org_ViCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    if (mTablaBaseOrganizacionID > -1) //Si no hay organizacionID, no es necesario actualizar estas tablas
                    {

                        #region Eliminar tabla ColaTagsCOM_X_ORG_X
                        DbCommand DeleteColaTagsCOM_X_ORG_XCommand = ObtenerComando(sqlColaTagsCOM_X_ORG_XDelete);
                        AgregarParametro(DeleteColaTagsCOM_X_ORG_XCommand, IBD.ToParam("O_TablaBaseProyectoID"), DbType.Int32, "TablaBaseProyectoID", DataRowVersion.Original);
                        AgregarParametro(DeleteColaTagsCOM_X_ORG_XCommand, IBD.ToParam("O_TablaBaseOrganizacionID"), DbType.Int32, "TablaBaseOrganizacionID", DataRowVersion.Original);
                        AgregarParametro(DeleteColaTagsCOM_X_ORG_XCommand, IBD.ToParam("O_Tags"), DbType.String, "Tags", DataRowVersion.Original);
                        AgregarParametro(DeleteColaTagsCOM_X_ORG_XCommand, IBD.ToParam("O_Tipo"), DbType.Int16, "Tipo", DataRowVersion.Original);
                        AgregarParametro(DeleteColaTagsCOM_X_ORG_XCommand, IBD.ToParam("O_Estado"), DbType.Int16, "Estado", DataRowVersion.Original);
                        AgregarParametro(DeleteColaTagsCOM_X_ORG_XCommand, IBD.ToParam("O_FechaPuestaEnCola"), DbType.DateTime, "FechaPuestaEnCola", DataRowVersion.Original);
                        AgregarParametro(DeleteColaTagsCOM_X_ORG_XCommand, IBD.ToParam("O_FechaProcesado"), DbType.DateTime, "FechaProcesado", DataRowVersion.Original);
                        AgregarParametro(DeleteColaTagsCOM_X_ORG_XCommand, IBD.ToParam("O_Prioridad"), DbType.Int16, "Prioridad", DataRowVersion.Original);
                        ActualizarBaseDeDatos(deletedDataSet, "ColaTagsCOM_X_ORG_X", null, null, DeleteColaTagsCOM_X_ORG_XCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    }

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
        /// Guarda los cambios realizado en el dataset pasado como parámetro
        /// </summary>
        /// <param name="pDataSet">Dataset con cambios</param>
        public override void GuardarActualizaciones(DataSet pDataSet)
        {
            try
            {
                InsertarFilasEnRabbit("ColaTagsCom_Per_Org", pDataSet);

                DataSet addedAndModifiedDataSet;
                addedAndModifiedDataSet = pDataSet.GetChanges(DataRowState.Added | DataRowState.Modified);

                if (addedAndModifiedDataSet != null)
                {
                    #region AddedAndModified

                    #region Actualizar tabla ColaTagsCom_Per_Org
                    DbCommand InsertColaTagsCom_Per_OrgCommand = ObtenerComando(sqlColaTagsCom_Per_OrgInsert);
                    AgregarParametro(InsertColaTagsCom_Per_OrgCommand, IBD.ToParam("TablaBaseProyectoID"), DbType.Int32, "TablaBaseProyectoID", DataRowVersion.Current);
                    AgregarParametro(InsertColaTagsCom_Per_OrgCommand, IBD.ToParam("Tags"), DbType.String, "Tags", DataRowVersion.Current);
                    AgregarParametro(InsertColaTagsCom_Per_OrgCommand, IBD.ToParam("Tipo"), DbType.Int16, "Tipo", DataRowVersion.Current);
                    AgregarParametro(InsertColaTagsCom_Per_OrgCommand, IBD.ToParam("Estado"), DbType.Int16, "Estado", DataRowVersion.Current);
                    AgregarParametro(InsertColaTagsCom_Per_OrgCommand, IBD.ToParam("FechaPuestaEnCola"), DbType.DateTime, "FechaPuestaEnCola", DataRowVersion.Current);
                    AgregarParametro(InsertColaTagsCom_Per_OrgCommand, IBD.ToParam("FechaProcesado"), DbType.DateTime, "FechaProcesado", DataRowVersion.Current);
                    AgregarParametro(InsertColaTagsCom_Per_OrgCommand, IBD.ToParam("Prioridad"), DbType.Int16, "Prioridad", DataRowVersion.Current);
                    AgregarParametro(InsertColaTagsCom_Per_OrgCommand, IBD.ToParam("EstadoTags"), DbType.Int16, "EstadoTags", DataRowVersion.Current);

                    DbCommand ModifyColaTagsCom_Per_OrgCommand = ObtenerComando(sqlColaTagsCom_Per_OrgModify);
                    AgregarParametro(ModifyColaTagsCom_Per_OrgCommand, IBD.ToParam("O_OrdenEjecucion"), DbType.Int32, "OrdenEjecucion", DataRowVersion.Original);

                    AgregarParametro(ModifyColaTagsCom_Per_OrgCommand, IBD.ToParam("TablaBaseProyectoID"), DbType.Int32, "TablaBaseProyectoID", DataRowVersion.Current);
                    AgregarParametro(ModifyColaTagsCom_Per_OrgCommand, IBD.ToParam("Tags"), DbType.String, "Tags", DataRowVersion.Current);
                    AgregarParametro(ModifyColaTagsCom_Per_OrgCommand, IBD.ToParam("Tipo"), DbType.Int16, "Tipo", DataRowVersion.Current);
                    AgregarParametro(ModifyColaTagsCom_Per_OrgCommand, IBD.ToParam("Estado"), DbType.Int16, "Estado", DataRowVersion.Current);
                    AgregarParametro(ModifyColaTagsCom_Per_OrgCommand, IBD.ToParam("FechaPuestaEnCola"), DbType.DateTime, "FechaPuestaEnCola", DataRowVersion.Current);
                    AgregarParametro(ModifyColaTagsCom_Per_OrgCommand, IBD.ToParam("FechaProcesado"), DbType.DateTime, "FechaProcesado", DataRowVersion.Current);
                    AgregarParametro(ModifyColaTagsCom_Per_OrgCommand, IBD.ToParam("Prioridad"), DbType.Int16, "Prioridad", DataRowVersion.Current);
                    AgregarParametro(ModifyColaTagsCom_Per_OrgCommand, IBD.ToParam("EstadoTags"), DbType.Int16, "EstadoTags", DataRowVersion.Current);

                    ActualizarBaseDeDatos(addedAndModifiedDataSet, "ColaTagsCom_Per_Org", InsertColaTagsCom_Per_OrgCommand, ModifyColaTagsCom_Per_OrgCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #region Actualizar tabla ColaTagsCom_Per_Org_Vi
                    DbCommand InsertColaTagsCom_Per_Org_ViCommand = ObtenerComando(sqlColaTagsCom_Per_Org_ViInsert);
                    AgregarParametro(InsertColaTagsCom_Per_Org_ViCommand, IBD.ToParam("TablaBaseProyectoID"), DbType.Int32, "TablaBaseProyectoID", DataRowVersion.Current);
                    AgregarParametro(InsertColaTagsCom_Per_Org_ViCommand, IBD.ToParam("Tags"), DbType.String, "Tags", DataRowVersion.Current);
                    AgregarParametro(InsertColaTagsCom_Per_Org_ViCommand, IBD.ToParam("Tipo"), DbType.Int16, "Tipo", DataRowVersion.Current);
                    AgregarParametro(InsertColaTagsCom_Per_Org_ViCommand, IBD.ToParam("Estado"), DbType.Int16, "Estado", DataRowVersion.Current);
                    AgregarParametro(InsertColaTagsCom_Per_Org_ViCommand, IBD.ToParam("FechaPuestaEnCola"), DbType.DateTime, "FechaPuestaEnCola", DataRowVersion.Current);
                    AgregarParametro(InsertColaTagsCom_Per_Org_ViCommand, IBD.ToParam("FechaProcesado"), DbType.DateTime, "FechaProcesado", DataRowVersion.Current);
                    AgregarParametro(InsertColaTagsCom_Per_Org_ViCommand, IBD.ToParam("Prioridad"), DbType.Int16, "Prioridad", DataRowVersion.Current);

                    DbCommand ModifyColaTagsCom_Per_Org_ViCommand = ObtenerComando(sqlColaTagsCom_Per_Org_ViModify);
                    AgregarParametro(ModifyColaTagsCom_Per_Org_ViCommand, IBD.ToParam("O_OrdenEjecucion"), DbType.Int32, "OrdenEjecucion", DataRowVersion.Original);
                    AgregarParametro(ModifyColaTagsCom_Per_Org_ViCommand, IBD.ToParam("O_TablaBaseProyectoID"), DbType.Int32, "TablaBaseProyectoID", DataRowVersion.Original);
                    AgregarParametro(ModifyColaTagsCom_Per_Org_ViCommand, IBD.ToParam("O_Tags"), DbType.String, "Tags", DataRowVersion.Original);
                    AgregarParametro(ModifyColaTagsCom_Per_Org_ViCommand, IBD.ToParam("O_Tipo"), DbType.Int16, "Tipo", DataRowVersion.Original);
                    AgregarParametro(ModifyColaTagsCom_Per_Org_ViCommand, IBD.ToParam("O_Estado"), DbType.Int16, "Estado", DataRowVersion.Original);
                    AgregarParametro(ModifyColaTagsCom_Per_Org_ViCommand, IBD.ToParam("O_FechaPuestaEnCola"), DbType.DateTime, "FechaPuestaEnCola", DataRowVersion.Original);
                    AgregarParametro(ModifyColaTagsCom_Per_Org_ViCommand, IBD.ToParam("O_FechaProcesado"), DbType.DateTime, "FechaProcesado", DataRowVersion.Original);
                    AgregarParametro(ModifyColaTagsCom_Per_Org_ViCommand, IBD.ToParam("O_Prioridad"), DbType.Int16, "Prioridad", DataRowVersion.Original);

                    AgregarParametro(ModifyColaTagsCom_Per_Org_ViCommand, IBD.ToParam("TablaBaseProyectoID"), DbType.Int32, "TablaBaseProyectoID", DataRowVersion.Current);
                    AgregarParametro(ModifyColaTagsCom_Per_Org_ViCommand, IBD.ToParam("Tags"), DbType.String, "Tags", DataRowVersion.Current);
                    AgregarParametro(ModifyColaTagsCom_Per_Org_ViCommand, IBD.ToParam("Tipo"), DbType.Int16, "Tipo", DataRowVersion.Current);
                    AgregarParametro(ModifyColaTagsCom_Per_Org_ViCommand, IBD.ToParam("Estado"), DbType.Int16, "Estado", DataRowVersion.Current);
                    AgregarParametro(ModifyColaTagsCom_Per_Org_ViCommand, IBD.ToParam("FechaPuestaEnCola"), DbType.DateTime, "FechaPuestaEnCola", DataRowVersion.Current);
                    AgregarParametro(ModifyColaTagsCom_Per_Org_ViCommand, IBD.ToParam("FechaProcesado"), DbType.DateTime, "FechaProcesado", DataRowVersion.Current);
                    AgregarParametro(ModifyColaTagsCom_Per_Org_ViCommand, IBD.ToParam("Prioridad"), DbType.Int16, "Prioridad", DataRowVersion.Current);
                    ActualizarBaseDeDatos(addedAndModifiedDataSet, "ColaTagsCom_Per_Org_Vi", InsertColaTagsCom_Per_Org_ViCommand, ModifyColaTagsCom_Per_Org_ViCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #region Actualizar tabla ColaTagsCOM_X_ORG_X
                    DbCommand InsertColaTagsCOM_X_ORG_XCommand = ObtenerComando(sqlColaTagsCOM_X_ORG_XInsert);
                    AgregarParametro(InsertColaTagsCOM_X_ORG_XCommand, IBD.ToParam("TablaBaseProyectoID"), DbType.Int32, "TablaBaseProyectoID", DataRowVersion.Current);
                    AgregarParametro(InsertColaTagsCOM_X_ORG_XCommand, IBD.ToParam("TablaBaseOrganizacionID"), DbType.Int32, "TablaBaseOrganizacionID", DataRowVersion.Current);
                    AgregarParametro(InsertColaTagsCOM_X_ORG_XCommand, IBD.ToParam("Tags"), DbType.String, "Tags", DataRowVersion.Current);
                    AgregarParametro(InsertColaTagsCOM_X_ORG_XCommand, IBD.ToParam("Tipo"), DbType.Int16, "Tipo", DataRowVersion.Current);
                    AgregarParametro(InsertColaTagsCOM_X_ORG_XCommand, IBD.ToParam("Estado"), DbType.Int16, "Estado", DataRowVersion.Current);
                    AgregarParametro(InsertColaTagsCOM_X_ORG_XCommand, IBD.ToParam("FechaPuestaEnCola"), DbType.DateTime, "FechaPuestaEnCola", DataRowVersion.Current);
                    AgregarParametro(InsertColaTagsCOM_X_ORG_XCommand, IBD.ToParam("FechaProcesado"), DbType.DateTime, "FechaProcesado", DataRowVersion.Current);
                    AgregarParametro(InsertColaTagsCOM_X_ORG_XCommand, IBD.ToParam("Prioridad"), DbType.Int16, "Prioridad", DataRowVersion.Current);

                    DbCommand ModifyColaTagsCOM_X_ORG_XCommand = ObtenerComando(sqlColaTagsCOM_X_ORG_XModify);
                    AgregarParametro(ModifyColaTagsCOM_X_ORG_XCommand, IBD.ToParam("O_OrdenEjecucion"), DbType.Int32, "OrdenEjecucion", DataRowVersion.Original);
                    AgregarParametro(ModifyColaTagsCOM_X_ORG_XCommand, IBD.ToParam("O_TablaBaseProyectoID"), DbType.Int32, "TablaBaseProyectoID", DataRowVersion.Original);
                    AgregarParametro(ModifyColaTagsCOM_X_ORG_XCommand, IBD.ToParam("O_TablaBaseOrganizacionID"), DbType.Int32, "TablaBaseOrganizacionID", DataRowVersion.Original);
                    AgregarParametro(ModifyColaTagsCOM_X_ORG_XCommand, IBD.ToParam("O_Tags"), DbType.String, "Tags", DataRowVersion.Original);
                    AgregarParametro(ModifyColaTagsCOM_X_ORG_XCommand, IBD.ToParam("O_Tipo"), DbType.Int16, "Tipo", DataRowVersion.Original);
                    AgregarParametro(ModifyColaTagsCOM_X_ORG_XCommand, IBD.ToParam("O_Estado"), DbType.Int16, "Estado", DataRowVersion.Original);
                    AgregarParametro(ModifyColaTagsCOM_X_ORG_XCommand, IBD.ToParam("O_FechaPuestaEnCola"), DbType.DateTime, "FechaPuestaEnCola", DataRowVersion.Original);
                    AgregarParametro(ModifyColaTagsCOM_X_ORG_XCommand, IBD.ToParam("O_FechaProcesado"), DbType.DateTime, "FechaProcesado", DataRowVersion.Original);
                    AgregarParametro(ModifyColaTagsCOM_X_ORG_XCommand, IBD.ToParam("O_Prioridad"), DbType.Int16, "Prioridad", DataRowVersion.Original);

                    AgregarParametro(ModifyColaTagsCOM_X_ORG_XCommand, IBD.ToParam("TablaBaseProyectoID"), DbType.Int32, "TablaBaseProyectoID", DataRowVersion.Current);
                    AgregarParametro(ModifyColaTagsCOM_X_ORG_XCommand, IBD.ToParam("TablaBaseOrganizacionID"), DbType.Int32, "TablaBaseOrganizacionID", DataRowVersion.Current);
                    AgregarParametro(ModifyColaTagsCOM_X_ORG_XCommand, IBD.ToParam("Tags"), DbType.String, "Tags", DataRowVersion.Current);
                    AgregarParametro(ModifyColaTagsCOM_X_ORG_XCommand, IBD.ToParam("Tipo"), DbType.Int16, "Tipo", DataRowVersion.Current);
                    AgregarParametro(ModifyColaTagsCOM_X_ORG_XCommand, IBD.ToParam("Estado"), DbType.Int16, "Estado", DataRowVersion.Current);
                    AgregarParametro(ModifyColaTagsCOM_X_ORG_XCommand, IBD.ToParam("FechaPuestaEnCola"), DbType.DateTime, "FechaPuestaEnCola", DataRowVersion.Current);
                    AgregarParametro(ModifyColaTagsCOM_X_ORG_XCommand, IBD.ToParam("FechaProcesado"), DbType.DateTime, "FechaProcesado", DataRowVersion.Current);
                    AgregarParametro(ModifyColaTagsCOM_X_ORG_XCommand, IBD.ToParam("Prioridad"), DbType.Int16, "Prioridad", DataRowVersion.Current);
                    ActualizarBaseDeDatos(addedAndModifiedDataSet, "ColaTagsCOM_X_ORG_X", InsertColaTagsCOM_X_ORG_XCommand, ModifyColaTagsCOM_X_ORG_XCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    if (mTablaBaseProyectoID > -1) //Si no hay proyectoID, no es necesario actualizar estas tablas
                    {
                        #region Actualizar tabla COM_PER_ORG
                        DbCommand InsertCOM_PER_ORGCommand = ObtenerComando(sqlCOM_PER_ORGInsert);
                        AgregarParametro(InsertCOM_PER_ORGCommand, IBD.ToParam("Tag1"), DbType.String, "Tag1", DataRowVersion.Current);
                        AgregarParametro(InsertCOM_PER_ORGCommand, IBD.ToParam("Tag2"), DbType.String, "Tag2", DataRowVersion.Current);
                        AgregarParametro(InsertCOM_PER_ORGCommand, IBD.ToParam("Tipo"), DbType.Int16, "Tipo", DataRowVersion.Current);
                        AgregarParametro(InsertCOM_PER_ORGCommand, IBD.ToParam("CercaniaDirecta"), DbType.Int32, "CercaniaDirecta", DataRowVersion.Current);
                        AgregarParametro(InsertCOM_PER_ORGCommand, IBD.ToParam("CercaniaIndirecta"), DbType.Int32, "CercaniaIndirecta", DataRowVersion.Current);

                        DbCommand ModifyCOM_PER_ORGCommand = ObtenerComando(sqlCOM_PER_ORGModify);
                        AgregarParametro(ModifyCOM_PER_ORGCommand, IBD.ToParam("O_Tag1"), DbType.String, "Tag1", DataRowVersion.Original);
                        AgregarParametro(ModifyCOM_PER_ORGCommand, IBD.ToParam("O_Tag2"), DbType.String, "Tag2", DataRowVersion.Original);
                        AgregarParametro(ModifyCOM_PER_ORGCommand, IBD.ToParam("O_Tipo"), DbType.Int16, "Tipo", DataRowVersion.Original);
                        AgregarParametro(ModifyCOM_PER_ORGCommand, IBD.ToParam("O_CercaniaDirecta"), DbType.Int32, "CercaniaDirecta", DataRowVersion.Original);
                        AgregarParametro(ModifyCOM_PER_ORGCommand, IBD.ToParam("O_CercaniaIndirecta"), DbType.Int32, "CercaniaIndirecta", DataRowVersion.Original);

                        AgregarParametro(ModifyCOM_PER_ORGCommand, IBD.ToParam("Tag1"), DbType.String, "Tag1", DataRowVersion.Current);
                        AgregarParametro(ModifyCOM_PER_ORGCommand, IBD.ToParam("Tag2"), DbType.String, "Tag2", DataRowVersion.Current);
                        AgregarParametro(ModifyCOM_PER_ORGCommand, IBD.ToParam("Tipo"), DbType.Int16, "Tipo", DataRowVersion.Current);
                        AgregarParametro(ModifyCOM_PER_ORGCommand, IBD.ToParam("CercaniaDirecta"), DbType.Int32, "CercaniaDirecta", DataRowVersion.Current);
                        AgregarParametro(ModifyCOM_PER_ORGCommand, IBD.ToParam("CercaniaIndirecta"), DbType.Int32, "CercaniaIndirecta", DataRowVersion.Current);
                        ActualizarBaseDeDatos(addedAndModifiedDataSet, "COM_PER_ORG", InsertCOM_PER_ORGCommand, ModifyCOM_PER_ORGCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                        #endregion

                        #region Actualizar tabla COM_PER_ORG_VI
                        DbCommand InsertCOM_PER_ORG_VICommand = ObtenerComando(sqlCOM_PER_ORG_VIInsert);
                        AgregarParametro(InsertCOM_PER_ORG_VICommand, IBD.ToParam("Tag1"), DbType.String, "Tag1", DataRowVersion.Current);
                        AgregarParametro(InsertCOM_PER_ORG_VICommand, IBD.ToParam("Tag2"), DbType.String, "Tag2", DataRowVersion.Current);
                        AgregarParametro(InsertCOM_PER_ORG_VICommand, IBD.ToParam("Tipo"), DbType.Int16, "Tipo", DataRowVersion.Current);
                        AgregarParametro(InsertCOM_PER_ORG_VICommand, IBD.ToParam("CercaniaDirecta"), DbType.Int32, "CercaniaDirecta", DataRowVersion.Current);
                        AgregarParametro(InsertCOM_PER_ORG_VICommand, IBD.ToParam("CercaniaIndirecta"), DbType.Int32, "CercaniaIndirecta", DataRowVersion.Current);

                        DbCommand ModifyCOM_PER_ORG_VICommand = ObtenerComando(sqlCOM_PER_ORG_VIModify);
                        AgregarParametro(ModifyCOM_PER_ORG_VICommand, IBD.ToParam("O_Tag1"), DbType.String, "Tag1", DataRowVersion.Original);
                        AgregarParametro(ModifyCOM_PER_ORG_VICommand, IBD.ToParam("O_Tag2"), DbType.String, "Tag2", DataRowVersion.Original);
                        AgregarParametro(ModifyCOM_PER_ORG_VICommand, IBD.ToParam("O_Tipo"), DbType.Int16, "Tipo", DataRowVersion.Original);
                        AgregarParametro(ModifyCOM_PER_ORG_VICommand, IBD.ToParam("O_CercaniaDirecta"), DbType.Int32, "CercaniaDirecta", DataRowVersion.Original);
                        AgregarParametro(ModifyCOM_PER_ORG_VICommand, IBD.ToParam("O_CercaniaIndirecta"), DbType.Int32, "CercaniaIndirecta", DataRowVersion.Original);

                        AgregarParametro(ModifyCOM_PER_ORG_VICommand, IBD.ToParam("Tag1"), DbType.String, "Tag1", DataRowVersion.Current);
                        AgregarParametro(ModifyCOM_PER_ORG_VICommand, IBD.ToParam("Tag2"), DbType.String, "Tag2", DataRowVersion.Current);
                        AgregarParametro(ModifyCOM_PER_ORG_VICommand, IBD.ToParam("Tipo"), DbType.Int16, "Tipo", DataRowVersion.Current);
                        AgregarParametro(ModifyCOM_PER_ORG_VICommand, IBD.ToParam("CercaniaDirecta"), DbType.Int32, "CercaniaDirecta", DataRowVersion.Current);
                        AgregarParametro(ModifyCOM_PER_ORG_VICommand, IBD.ToParam("CercaniaIndirecta"), DbType.Int32, "CercaniaIndirecta", DataRowVersion.Current);
                        ActualizarBaseDeDatos(addedAndModifiedDataSet, "COM_PER_ORG_VI", InsertCOM_PER_ORG_VICommand, ModifyCOM_PER_ORG_VICommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                        #endregion
                    }

                    if (mTablaBaseOrganizacionID > -1) //Si no hay organizacionID, no es necesario actualizar estas tablas
                    {
                        #region Actualizar tabla COM_X_ORG_X
                        DbCommand InsertCOM_X_ORG_XCommand = ObtenerComando(sqlCOM_X_ORG_XInsert);
                        AgregarParametro(InsertCOM_X_ORG_XCommand, IBD.ToParam("Tag1"), DbType.String, "Tag1", DataRowVersion.Current);
                        AgregarParametro(InsertCOM_X_ORG_XCommand, IBD.ToParam("Tag2"), DbType.String, "Tag2", DataRowVersion.Current);
                        AgregarParametro(InsertCOM_X_ORG_XCommand, IBD.ToParam("Tipo"), DbType.Int16, "Tipo", DataRowVersion.Current);
                        AgregarParametro(InsertCOM_X_ORG_XCommand, IBD.ToParam("CercaniaDirecta"), DbType.Int32, "CercaniaDirecta", DataRowVersion.Current);
                        AgregarParametro(InsertCOM_X_ORG_XCommand, IBD.ToParam("CercaniaIndirecta"), DbType.Int32, "CercaniaIndirecta", DataRowVersion.Current);

                        DbCommand ModifyCOM_X_ORG_XCommand = ObtenerComando(sqlCOM_X_ORG_XModify);
                        AgregarParametro(ModifyCOM_X_ORG_XCommand, IBD.ToParam("O_Tag1"), DbType.String, "Tag1", DataRowVersion.Original);
                        AgregarParametro(ModifyCOM_X_ORG_XCommand, IBD.ToParam("O_Tag2"), DbType.String, "Tag2", DataRowVersion.Original);
                        AgregarParametro(ModifyCOM_X_ORG_XCommand, IBD.ToParam("O_Tipo"), DbType.Int16, "Tipo", DataRowVersion.Original);
                        AgregarParametro(ModifyCOM_X_ORG_XCommand, IBD.ToParam("O_CercaniaDirecta"), DbType.Int32, "CercaniaDirecta", DataRowVersion.Original);
                        AgregarParametro(ModifyCOM_X_ORG_XCommand, IBD.ToParam("O_CercaniaIndirecta"), DbType.Int32, "CercaniaIndirecta", DataRowVersion.Original);

                        AgregarParametro(ModifyCOM_X_ORG_XCommand, IBD.ToParam("Tag1"), DbType.String, "Tag1", DataRowVersion.Current);
                        AgregarParametro(ModifyCOM_X_ORG_XCommand, IBD.ToParam("Tag2"), DbType.String, "Tag2", DataRowVersion.Current);
                        AgregarParametro(ModifyCOM_X_ORG_XCommand, IBD.ToParam("Tipo"), DbType.Int16, "Tipo", DataRowVersion.Current);
                        AgregarParametro(ModifyCOM_X_ORG_XCommand, IBD.ToParam("CercaniaDirecta"), DbType.Int32, "CercaniaDirecta", DataRowVersion.Current);
                        AgregarParametro(ModifyCOM_X_ORG_XCommand, IBD.ToParam("CercaniaIndirecta"), DbType.Int32, "CercaniaIndirecta", DataRowVersion.Current);
                        ActualizarBaseDeDatos(addedAndModifiedDataSet, "COM_X_ORG_X", InsertCOM_X_ORG_XCommand, ModifyCOM_X_ORG_XCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

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

            this.sqlSelectColaTagsCom_Per_Org = "SELECT ColaTagsCom_Per_Org.OrdenEjecucion, ColaTagsCom_Per_Org.TablaBaseProyectoID, ColaTagsCom_Per_Org.Tags, ColaTagsCom_Per_Org.Tipo, ColaTagsCom_Per_Org.Estado, ColaTagsCom_Per_Org.FechaPuestaEnCola, ColaTagsCom_Per_Org.FechaProcesado, ColaTagsCom_Per_Org.Prioridad, ColaTagsCom_Per_Org.EstadoTags FROM ColaTagsCom_Per_Org ";

            this.sqlSelectCOM_PER_ORG = "SELECT " + mNombreTablaCOM_PER_ORG + ".Tag1, " + mNombreTablaCOM_PER_ORG + ".Tag2, " + mNombreTablaCOM_PER_ORG + ".Tipo, " + mNombreTablaCOM_PER_ORG + ".CercaniaDirecta, " + mNombreTablaCOM_PER_ORG + ".CercaniaIndirecta FROM " + mNombreTablaCOM_PER_ORG + " ";

            this.sqlSelectCOM_PER_ORG_VI = "SELECT " + mNombreTablaCOM_PER_ORG_VI + ".Tag1, " + mNombreTablaCOM_PER_ORG_VI + ".Tag2, " + mNombreTablaCOM_PER_ORG_VI + ".Tipo, " + mNombreTablaCOM_PER_ORG_VI + ".CercaniaDirecta, " + mNombreTablaCOM_PER_ORG_VI + ".CercaniaIndirecta FROM " + mNombreTablaCOM_PER_ORG_VI + " ";

            this.sqlSelectCOM_X_ORG_X = "SELECT " + this.mNombreTablaCOM_X_ORG_X + ".Tag1, " + mNombreTablaCOM_X_ORG_X + ".Tag2, " + mNombreTablaCOM_X_ORG_X + ".Tipo, " + mNombreTablaCOM_X_ORG_X + ".CercaniaDirecta, " + mNombreTablaCOM_X_ORG_X + ".CercaniaIndirecta FROM " + mNombreTablaCOM_X_ORG_X + " ";

            #endregion

            #region DataAdapter

            #region ColaTagsCom_Per_Org

            this.sqlColaTagsCom_Per_OrgInsert = IBD.ReplaceParam("INSERT INTO ColaTagsCom_Per_Org (TablaBaseProyectoID, Tags, Tipo, Estado, FechaPuestaEnCola, FechaProcesado,Prioridad, EstadoTags) VALUES (@TablaBaseProyectoID, @Tags, @Tipo, @Estado, @FechaPuestaEnCola, @FechaProcesado,@Prioridad, @EstadoTags)");

            this.sqlColaTagsCom_Per_OrgDelete = IBD.ReplaceParam("DELETE FROM ColaTagsCom_Per_Org WHERE (OrdenEjecucion = @O_OrdenEjecucion)");

            this.sqlColaTagsCom_Per_OrgModify = IBD.ReplaceParam("UPDATE ColaTagsCom_Per_Org SET TablaBaseProyectoID = @TablaBaseProyectoID, Tags = @Tags, Tipo = @Tipo, Estado = @Estado, FechaPuestaEnCola = @FechaPuestaEnCola, FechaProcesado = @FechaProcesado, Prioridad = @Prioridad, EstadoTags = @EstadoTags WHERE (OrdenEjecucion = @O_OrdenEjecucion)");

            #endregion

            #region ColaTagsCom_Per_Org_Vi

            this.sqlColaTagsCom_Per_Org_ViInsert = IBD.ReplaceParam("INSERT INTO ColaTagsCom_Per_Org_Vi (TablaBaseProyectoID, Tags, Tipo, Estado, FechaPuestaEnCola, FechaProcesado,Prioridad) VALUES (@TablaBaseProyectoID, @Tags, @Tipo, @Estado, @FechaPuestaEnCola, @FechaProcesado,@Prioridad)");

            this.sqlColaTagsCom_Per_Org_Vielete = IBD.ReplaceParam("DELETE FROM ColaTagsCom_Per_Org_Vi WHERE (OrdenEjecucion = @O_OrdenEjecucion) AND (TablaBaseProyectoID = @O_TablaBaseProyectoID) AND (Tags = @O_Tags) AND (Tipo = @O_Tipo) AND (Estado = @O_Estado) AND (FechaPuestaEnCola = @O_FechaPuestaEnCola) AND (FechaProcesado = @O_FechaProcesado OR @O_FechaProcesado IS NULL AND FechaProcesado IS NULL)");
            this.sqlColaTagsCom_Per_Org_ViModify = IBD.ReplaceParam("UPDATE ColaTagsCom_Per_Org_Vi SET TablaBaseProyectoID = @TablaBaseProyectoID, Tags = @Tags, Tipo = @Tipo, Estado = @Estado, FechaPuestaEnCola = @FechaPuestaEnCola, FechaProcesado = @FechaProcesado, Prioridad = @Prioridad WHERE (OrdenEjecucion = @O_OrdenEjecucion) AND (TablaBaseProyectoID = @O_TablaBaseProyectoID) AND (Tags = @O_Tags) AND (Tipo = @O_Tipo) AND (Estado = @O_Estado) AND (FechaPuestaEnCola = @O_FechaPuestaEnCola) AND (FechaProcesado = @O_FechaProcesado OR @O_FechaProcesado IS NULL AND FechaProcesado IS NULL)");

            #endregion

            #region ColaTagsCOM_X_ORG_X

            this.sqlColaTagsCOM_X_ORG_XInsert = IBD.ReplaceParam("INSERT INTO ColaTagsCOM_X_ORG_X (TablaBaseProyectoID,TablaBaseOrganizacionID, Tags, Tipo, Estado, FechaPuestaEnCola, FechaProcesado,Prioridad) VALUES (@TablaBaseProyectoID,@TablaBaseOrganizacionID, @Tags, @Tipo, @Estado, @FechaPuestaEnCola, @FechaProcesado,@Prioridad)");

            this.sqlColaTagsCOM_X_ORG_XDelete = IBD.ReplaceParam("DELETE FROM ColaTagsCOM_X_ORG_X WHERE (OrdenEjecucion = @O_OrdenEjecucion) AND (TablaBaseProyectoID = @O_TablaBaseProyectoID) AND (TablaBaseOrganizacionID = @O_TablaBaseOrganizacionID) AND (Tags = @O_Tags) AND (Tipo = @O_Tipo) AND (Estado = @O_Estado) AND (FechaPuestaEnCola = @O_FechaPuestaEnCola) AND (FechaProcesado = @O_FechaProcesado OR @O_FechaProcesado IS NULL AND FechaProcesado IS NULL)");

            this.sqlColaTagsCOM_X_ORG_XModify = IBD.ReplaceParam("UPDATE ColaTagsCOM_X_ORG_X SET TablaBaseProyectoID = @TablaBaseProyectoID, TablaBaseOrganizacionID = @TablaBaseOrganizacionID, Tags = @Tags, Tipo = @Tipo, Estado = @Estado, FechaPuestaEnCola = @FechaPuestaEnCola, FechaProcesado = @FechaProcesado, Prioridad = @Prioridad WHERE (OrdenEjecucion = @O_OrdenEjecucion) AND (TablaBaseProyectoID = @O_TablaBaseProyectoID) AND (TablaBaseOrganizacionID = @O_TablaBaseOrganizacionID) AND (Tags = @O_Tags) AND (Tipo = @O_Tipo) AND (Estado = @O_Estado) AND (FechaPuestaEnCola = @O_FechaPuestaEnCola) AND (FechaProcesado = @O_FechaProcesado OR @O_FechaProcesado IS NULL AND FechaProcesado IS NULL)");

            #endregion

            #region COM_PER_ORG

            this.sqlCOM_PER_ORGInsert = IBD.ReplaceParam("INSERT INTO " + mNombreTablaCOM_PER_ORG + " (Tag1, Tag2, Tipo, CercaniaDirecta, CercaniaIndirecta) VALUES (@Tag1, @Tag2, @Tipo, @CercaniaDirecta, @CercaniaIndirecta)");

            this.sqlCOM_PER_ORGDelete = IBD.ReplaceParam("DELETE FROM " + mNombreTablaCOM_PER_ORG + " WHERE (Tag1 = @O_Tag1) AND (Tag2 = @O_Tag2) AND (Tipo = @O_Tipo) AND (CercaniaDirecta = @O_CercaniaDirecta) AND (CercaniaIndirecta = @O_CercaniaIndirecta)");

            this.sqlCOM_PER_ORGModify = IBD.ReplaceParam("UPDATE " + mNombreTablaCOM_PER_ORG + " SET Tag1 = @Tag1, Tag2 = @Tag2, Tipo = @Tipo, CercaniaDirecta = @CercaniaDirecta, CercaniaIndirecta = @CercaniaIndirecta WHERE (Tag1 = @O_Tag1) AND (Tag2 = @O_Tag2) AND (Tipo = @O_Tipo) AND (CercaniaDirecta = @O_CercaniaDirecta) AND (CercaniaIndirecta = @O_CercaniaIndirecta)");

            #endregion

            #region COM_PER_ORG_VI

            this.sqlCOM_PER_ORG_VIInsert = IBD.ReplaceParam("INSERT INTO " + mNombreTablaCOM_PER_ORG_VI + " (Tag1, Tag2, Tipo, CercaniaDirecta, CercaniaIndirecta) VALUES (@Tag1, @Tag2, @Tipo, @CercaniaDirecta, @CercaniaIndirecta)");

            this.sqlCOM_PER_ORG_VIelete = IBD.ReplaceParam("DELETE FROM " + mNombreTablaCOM_PER_ORG_VI + " WHERE (Tag1 = @O_Tag1) AND (Tag2 = @O_Tag2) AND (Tipo = @O_Tipo) AND (CercaniaDirecta = @O_CercaniaDirecta) AND (CercaniaIndirecta = @O_CercaniaIndirecta)");

            this.sqlCOM_PER_ORG_VIModify = IBD.ReplaceParam("UPDATE " + mNombreTablaCOM_PER_ORG_VI + " SET Tag1 = @Tag1, Tag2 = @Tag2, Tipo = @Tipo, CercaniaDirecta = @CercaniaDirecta, CercaniaIndirecta = @CercaniaIndirecta WHERE (Tag1 = @O_Tag1) AND (Tag2 = @O_Tag2) AND (Tipo = @O_Tipo) AND (CercaniaDirecta = @O_CercaniaDirecta) AND (CercaniaIndirecta = @O_CercaniaIndirecta)");

            #endregion

            #region COM_X_ORG_X

            this.sqlCOM_X_ORG_XInsert = IBD.ReplaceParam("INSERT INTO " + mNombreTablaCOM_X_ORG_X + " (Tag1, Tag2, Tipo, CercaniaDirecta, CercaniaIndirecta) VALUES (@Tag1, @Tag2, @Tipo, @CercaniaDirecta, @CercaniaIndirecta)");

            this.sqlCOM_X_ORG_XDelete = IBD.ReplaceParam("DELETE FROM " + mNombreTablaCOM_X_ORG_X + " WHERE (Tag1 = @O_Tag1) AND (Tag2 = @O_Tag2) AND (Tipo = @O_Tipo) AND (CercaniaDirecta = @O_CercaniaDirecta) AND (CercaniaIndirecta = @O_CercaniaIndirecta)");

            this.sqlCOM_X_ORG_XModify = IBD.ReplaceParam("UPDATE " + mNombreTablaCOM_X_ORG_X + " SET Tag1 = @Tag1, Tag2 = @Tag2, Tipo = @Tipo, CercaniaDirecta = @CercaniaDirecta, CercaniaIndirecta = @CercaniaIndirecta WHERE (Tag1 = @O_Tag1) AND (Tag2 = @O_Tag2) AND (Tipo = @O_Tipo) AND (CercaniaDirecta = @O_CercaniaDirecta) AND (CercaniaIndirecta = @O_CercaniaIndirecta)");

            #endregion

            #endregion
        }

        #endregion

        #region Override

        /// <summary>
        /// Comprueba si existen las tablas sobre las que está configurado este AD. Si no existen las crea. 
        /// </summary>
        /// <param name="pTipoConsulta">Tipo de consulta</param>
        /// <param name="pCrearTablaSiNoExiste">TRUE si debe crear la tabla si no existe, FALSE en caso contrario</param>
        /// <returns>Verdad si la tabla existe (o ha sido recién creada).</returns>
        public override bool VerificarExisteTabla(TiposConsultaObtenerTags pTipoConsulta, bool pCrearTablaSiNoExiste)
        {
            bool existeTabla = false;

            switch (pTipoConsulta)
            {
                case TiposConsultaObtenerTags.PersonasYOrganizacionesComunidad:
                    existeTabla = VerificarExisteTabla(mNombreTablaCOM_PER_ORG);
                    break;
                case TiposConsultaObtenerTags.PersonasYOrganizacionesVisiblesInvitadoComunidad:
                    existeTabla = VerificarExisteTabla(mNombreTablaCOM_PER_ORG_VI);
                    break;
                case TiposConsultaObtenerTags.PersonasDeOrganizacionParticipanComunidad:
                    existeTabla = VerificarExisteTabla(mNombreTablaCOM_X_ORG_X);
                    break;
                default:
                    existeTabla = base.VerificarExisteTabla(pTipoConsulta, pCrearTablaSiNoExiste);
                    break;
            }

            if ((!existeTabla) && (pCrearTablaSiNoExiste))
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
            if (pTipoConsulta.Equals(TiposConsultaObtenerTags.PersonasYOrganizacionesComunidad))
            {
                CrearTabla("CREATE TABLE [dbo]." + mNombreTablaCOM_PER_ORG + "([Tag1] [nvarchar](1000) COLLATE Modern_Spanish_CI_AI NOT NULL,[Tag2] [nvarchar](1000) COLLATE Modern_Spanish_CI_AI NOT NULL,[Tipo] [smallint] NOT NULL,[CercaniaDirecta] [int] NOT NULL,[CercaniaIndirecta] [int] NOT NULL) ON [PRIMARY]");
            }
            else if (pTipoConsulta.Equals(TiposConsultaObtenerTags.PersonasYOrganizacionesVisiblesInvitadoComunidad))
            {
                CrearTabla("CREATE TABLE [dbo]." + mNombreTablaCOM_PER_ORG_VI + "([Tag1] [nvarchar](1000) COLLATE Modern_Spanish_CI_AI NOT NULL,[Tag2] [nvarchar](1000) COLLATE Modern_Spanish_CI_AI NOT NULL,[Tipo] [smallint] NOT NULL,[CercaniaDirecta] [int] NOT NULL,[CercaniaIndirecta] [int] NOT NULL) ON [PRIMARY]");
            }
            else if (pTipoConsulta.Equals(TiposConsultaObtenerTags.PersonasDeOrganizacionParticipanComunidad))
            {
                CrearTabla("CREATE TABLE [dbo]." + mNombreTablaCOM_X_ORG_X + "([Tag1] [nvarchar](1000) COLLATE Modern_Spanish_CI_AI NOT NULL,[Tag2] [nvarchar](1000) COLLATE Modern_Spanish_CI_AI NOT NULL,[Tipo] [smallint] NOT NULL,[CercaniaDirecta] [int] NOT NULL,[CercaniaIndirecta] [int] NOT NULL) ON [PRIMARY]");
            }
            else
            {
                base.CrearTabla(pTipoConsulta);
            }
        }

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
                case TiposConsultaObtenerTags.PersonasYOrganizacionesComunidad:
                case TiposConsultaObtenerTags.PersonasDeOrganizacionParticipanComunidad:
                    nombreTabla = mNombreTablaCOM_PER_ORG;
                    break;
                case TiposConsultaObtenerTags.PersonasYOrganizacionesVisiblesInvitadoComunidad:
                    nombreTabla = mNombreTablaCOM_PER_ORG_VI;
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
                case "COM_PER_ORG":
                    return NombreTablaCOM_PER_ORG;
                case "COM_PER_ORG_VI":
                    return NombreTablaCOM_PER_ORG_VI;
                default:
                    return base.ObtenerNombreCorrectoTabla(pNombreTabla);
            }
        }

        #endregion

        #endregion

        #region Propiedades
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
        /// <summary>
        /// Obtiene el nombre real de la tabla que deberemos consultar para los tags de las personas y organizaciones de una comunidad (en funcion del identificador numerico del proyecto) que participen en el
        /// </summary>
        public string NombreTablaCOM_PER_ORG
        {
            get
            {
                return mNombreTablaCOM_PER_ORG;
            }
        }

        /// <summary>
        /// Obtiene el nombre real de la tabla que deberemos consultar para los tags de las personas y organizaciones de una comunidad (en funcion del identificador numerico del proyecto) que han decidido ser buscables para externos.
        /// </summary>
        public string NombreTablaCOM_PER_ORG_VI
        {
            get
            {
                return mNombreTablaCOM_PER_ORG_VI;
            }
        }

        /// <summary>
        /// Obtiene el nombre real de la tabla que deberemos consultar para los tags de las personas y organizaciones de una organizacion que participa en una determinada comunidad (en funcion del identificador numerico del proyecto y el identificador numerico de la organización)
        /// </summary>
        public string NombreTablaCOM_X_ORG_X
        {
            get
            {
                return mNombreTablaCOM_X_ORG_X;
            }
        }

        #endregion
    }
}
