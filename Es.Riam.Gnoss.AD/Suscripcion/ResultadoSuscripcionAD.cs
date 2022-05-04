using System;
using System.Collections.Generic;
using System.Text;

namespace Es.Riam.Gnoss.AD.Suscripcion
{
    class ResultadoSuscripcionAD : BaseAD
    {
        #region Constructores

        /// <summary>
        /// El por defecto, utilizado cuando se requiere el GnossConfig.xml por defecto
        /// </summary>
        public ResultadoSuscripcionAD() : base()
        {
            this.CargarConsultasYDataAdapters();
        }

        /// <summary>
        /// Cuando se desea pasar directamente la ruta del fichero de configuracion de conexion a la Base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD"></param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public ResultadoSuscripcionAD(string pFicheroConfiguracionBD, bool pUsarVariableEstatica)
            : base(pFicheroConfiguracionBD, pUsarVariableEstatica)
        {
            IBaseDatos IBDFicheroParametrizado = ObtenerBaseDatos(pFicheroConfiguracionBD);
            this.CargarConsultasYDataAdapters(IBDFicheroParametrizado);
        }

        #endregion

        #region Consultas

        private string sqlSelectSuscripciones;
        private string sqlSelectSuscripcionTesauroUsuario;
        private string sqlSelectSuscripcionTesauroUsuarioSimple;
        private string sqlSelectSuscripcionTesauroProyecto;
        private string sqlSelectSuscripcionTesauroProyectoSimple;
        private string sqlSelectCategoriasTesVinSuscrip;
        private string sqlSelectCategoriasTesVinSuscripSimple;
        private string sqlSelectSuscripcionBlog;
        private string sqlSelectSuscripcionBlogSimple;
        private string sqlSelectSuscripcionesAUsuario;

        #endregion

        #region DataAdapter

       

        #endregion

        #region Métodos

        #region Públicos

        /// <summary>
        /// Guarda los datos del dataset en la BD
        /// </summary>
        /// <param name="pSuscripcionDS">Dataset de suscripciones</param>
        public void GuardarSuscripcion(DataSet pSuscripcionDS)
        {
            try
            {
                #region Actualizo la parte de Suscripcion (Added|ModifiedCurrent)

                DataSet addedAndModifiedDataSet;
                addedAndModifiedDataSet = pSuscripcionDS.GetChanges(DataRowState.Added | DataRowState.Modified);
                
                if (addedAndModifiedDataSet != null)
                {
                    #region Actualizar tabla Suscripcion
                    DbCommand InsertSuscripcionCommand = mBaseDatos.GetSqlStringCommand(sqlSuscripcionInsert);
                    mBaseDatos.AddInParameter(InsertSuscripcionCommand, IBD.ToParam("SuscripcionID"), IBD.TipoGuidToObject(DbType.Guid), "SuscripcionID", DataRowVersion.Current);
                    mBaseDatos.AddInParameter(InsertSuscripcionCommand, IBD.ToParam("Periodicidad"), DbType.Int32, "Periodicidad", DataRowVersion.Current);
                    mBaseDatos.AddInParameter(InsertSuscripcionCommand, IBD.ToParam("IdentidadID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadID", DataRowVersion.Current);
                    mBaseDatos.AddInParameter(InsertSuscripcionCommand, IBD.ToParam("Bloqueada"), DbType.Boolean, "Bloqueada", DataRowVersion.Current);

                    DbCommand ModifySuscripcionCommand = mBaseDatos.GetSqlStringCommand(sqlSuscripcionModify);
                    mBaseDatos.AddInParameter(ModifySuscripcionCommand, IBD.ToParam("O_SuscripcionID"), IBD.TipoGuidToObject(DbType.Guid), "SuscripcionID", DataRowVersion.Original);
                    mBaseDatos.AddInParameter(ModifySuscripcionCommand, IBD.ToParam("O_Periodicidad"), DbType.Int32, "Periodicidad", DataRowVersion.Original);
                    mBaseDatos.AddInParameter(ModifySuscripcionCommand, IBD.ToParam("O_IdentidadID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadID", DataRowVersion.Original);
                    mBaseDatos.AddInParameter(ModifySuscripcionCommand, IBD.ToParam("O_Bloqueada"), DbType.Boolean, "Bloqueada", DataRowVersion.Original);

                    mBaseDatos.AddInParameter(ModifySuscripcionCommand, IBD.ToParam("SuscripcionID"), IBD.TipoGuidToObject(DbType.Guid), "SuscripcionID", DataRowVersion.Current);
                    mBaseDatos.AddInParameter(ModifySuscripcionCommand, IBD.ToParam("Periodicidad"), DbType.Int32, "Periodicidad", DataRowVersion.Current);
                    mBaseDatos.AddInParameter(ModifySuscripcionCommand, IBD.ToParam("IdentidadID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadID", DataRowVersion.Current);
                    mBaseDatos.AddInParameter(ModifySuscripcionCommand, IBD.ToParam("Bloqueada"), DbType.Boolean, "Bloqueada", DataRowVersion.Current);
                    mBaseDatos.UpdateDataSet(addedAndModifiedDataSet, "Suscripcion", InsertSuscripcionCommand, ModifySuscripcionCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #region Actualizar tabla SuscripcionBlog

                    DbCommand InsertSuscripcionBlogCommand = mBaseDatos.GetSqlStringCommand(sqlSuscripcionBlogInsert);
                    mBaseDatos.AddInParameter(InsertSuscripcionBlogCommand, IBD.ToParam("SuscripcionID"), IBD.TipoGuidToObject(DbType.Guid), "SuscripcionID", DataRowVersion.Current);
                    mBaseDatos.AddInParameter(InsertSuscripcionBlogCommand, IBD.ToParam("BlogID"), IBD.TipoGuidToObject(DbType.Guid), "BlogID", DataRowVersion.Current);

                    DbCommand ModifySuscripcionBlogCommand = mBaseDatos.GetSqlStringCommand(sqlSuscripcionBlogModify);
                    mBaseDatos.AddInParameter(ModifySuscripcionBlogCommand, IBD.ToParam("O_SuscripcionID"), IBD.TipoGuidToObject(DbType.Guid), "SuscripcionID", DataRowVersion.Original);
                    mBaseDatos.AddInParameter(ModifySuscripcionBlogCommand, IBD.ToParam("O_BlogID"), IBD.TipoGuidToObject(DbType.Guid), "BlogID", DataRowVersion.Original);

                    mBaseDatos.AddInParameter(ModifySuscripcionBlogCommand, IBD.ToParam("SuscripcionID"), IBD.TipoGuidToObject(DbType.Guid), "SuscripcionID", DataRowVersion.Current);
                    mBaseDatos.AddInParameter(ModifySuscripcionBlogCommand, IBD.ToParam("BlogID"), IBD.TipoGuidToObject(DbType.Guid), "BlogID", DataRowVersion.Current);
                    mBaseDatos.UpdateDataSet(addedAndModifiedDataSet, "SuscripcionBlog", InsertSuscripcionBlogCommand, ModifySuscripcionBlogCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #region Actualizar tabla SuscripcionTesauroProyecto

                    DbCommand InsertSuscripcionTesauroProyectoCommand = mBaseDatos.GetSqlStringCommand(sqlSuscripcionTesauroProyectoInsert);
                    mBaseDatos.AddInParameter(InsertSuscripcionTesauroProyectoCommand, IBD.ToParam("SuscripcionID"), IBD.TipoGuidToObject(DbType.Guid), "SuscripcionID", DataRowVersion.Current);
                    mBaseDatos.AddInParameter(InsertSuscripcionTesauroProyectoCommand, IBD.ToParam("OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Current);
                    mBaseDatos.AddInParameter(InsertSuscripcionTesauroProyectoCommand, IBD.ToParam("ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Current);
                    mBaseDatos.AddInParameter(InsertSuscripcionTesauroProyectoCommand, IBD.ToParam("TesauroID"), IBD.TipoGuidToObject(DbType.Guid), "TesauroID", DataRowVersion.Current);

                    DbCommand ModifySuscripcionTesauroProyectoCommand = mBaseDatos.GetSqlStringCommand(sqlSuscripcionTesauroProyectoModify);
                    mBaseDatos.AddInParameter(ModifySuscripcionTesauroProyectoCommand, IBD.ToParam("O_SuscripcionID"), IBD.TipoGuidToObject(DbType.Guid), "SuscripcionID", DataRowVersion.Original);
                    mBaseDatos.AddInParameter(ModifySuscripcionTesauroProyectoCommand, IBD.ToParam("O_OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Original);
                    mBaseDatos.AddInParameter(ModifySuscripcionTesauroProyectoCommand, IBD.ToParam("O_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);
                    mBaseDatos.AddInParameter(ModifySuscripcionTesauroProyectoCommand, IBD.ToParam("O_TesauroID"), IBD.TipoGuidToObject(DbType.Guid), "TesauroID", DataRowVersion.Original);

                    mBaseDatos.AddInParameter(ModifySuscripcionTesauroProyectoCommand, IBD.ToParam("SuscripcionID"), IBD.TipoGuidToObject(DbType.Guid), "SuscripcionID", DataRowVersion.Current);
                    mBaseDatos.AddInParameter(ModifySuscripcionTesauroProyectoCommand, IBD.ToParam("OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Current);
                    mBaseDatos.AddInParameter(ModifySuscripcionTesauroProyectoCommand, IBD.ToParam("ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Current);
                    mBaseDatos.AddInParameter(ModifySuscripcionTesauroProyectoCommand, IBD.ToParam("TesauroID"), IBD.TipoGuidToObject(DbType.Guid), "TesauroID", DataRowVersion.Current);
                    mBaseDatos.UpdateDataSet(addedAndModifiedDataSet, "SuscripcionTesauroProyecto", InsertSuscripcionTesauroProyectoCommand, ModifySuscripcionTesauroProyectoCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #region Actualizar tabla SuscripcionTesauroUsuario

                    DbCommand InsertSuscripcionTesauroUsuarioCommand = mBaseDatos.GetSqlStringCommand(sqlSuscripcionTesauroUsuarioInsert);
                    mBaseDatos.AddInParameter(InsertSuscripcionTesauroUsuarioCommand, IBD.ToParam("SuscripcionID"), IBD.TipoGuidToObject(DbType.Guid), "SuscripcionID", DataRowVersion.Current);
                    mBaseDatos.AddInParameter(InsertSuscripcionTesauroUsuarioCommand, IBD.ToParam("UsuarioID"), IBD.TipoGuidToObject(DbType.Guid), "UsuarioID", DataRowVersion.Current);
                    mBaseDatos.AddInParameter(InsertSuscripcionTesauroUsuarioCommand, IBD.ToParam("TesauroID"), IBD.TipoGuidToObject(DbType.Guid), "TesauroID", DataRowVersion.Current);

                    DbCommand ModifySuscripcionTesauroUsuarioCommand = mBaseDatos.GetSqlStringCommand(sqlSuscripcionTesauroUsuarioModify);
                    mBaseDatos.AddInParameter(ModifySuscripcionTesauroUsuarioCommand, IBD.ToParam("O_SuscripcionID"), IBD.TipoGuidToObject(DbType.Guid), "SuscripcionID", DataRowVersion.Original);
                    mBaseDatos.AddInParameter(ModifySuscripcionTesauroUsuarioCommand, IBD.ToParam("O_UsuarioID"), IBD.TipoGuidToObject(DbType.Guid), "UsuarioID", DataRowVersion.Original);
                    mBaseDatos.AddInParameter(ModifySuscripcionTesauroUsuarioCommand, IBD.ToParam("O_TesauroID"), IBD.TipoGuidToObject(DbType.Guid), "TesauroID", DataRowVersion.Original);

                    mBaseDatos.AddInParameter(ModifySuscripcionTesauroUsuarioCommand, IBD.ToParam("SuscripcionID"), IBD.TipoGuidToObject(DbType.Guid), "SuscripcionID", DataRowVersion.Current);
                    mBaseDatos.AddInParameter(ModifySuscripcionTesauroUsuarioCommand, IBD.ToParam("UsuarioID"), IBD.TipoGuidToObject(DbType.Guid), "UsuarioID", DataRowVersion.Current);
                    mBaseDatos.AddInParameter(ModifySuscripcionTesauroUsuarioCommand, IBD.ToParam("TesauroID"), IBD.TipoGuidToObject(DbType.Guid), "TesauroID", DataRowVersion.Current);
                    mBaseDatos.UpdateDataSet(addedAndModifiedDataSet, "SuscripcionTesauroUsuario", InsertSuscripcionTesauroUsuarioCommand, ModifySuscripcionTesauroUsuarioCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #region Actualizar tabla CategoriaTesVinSuscrip

                    DbCommand InsertCategoriaTesVinSuscripCommand = mBaseDatos.GetSqlStringCommand(sqlCategoriaTesVinSuscripInsert);
                    mBaseDatos.AddInParameter(InsertCategoriaTesVinSuscripCommand, IBD.ToParam("SuscripcionID"), IBD.TipoGuidToObject(DbType.Guid), "SuscripcionID", DataRowVersion.Current);
                    mBaseDatos.AddInParameter(InsertCategoriaTesVinSuscripCommand, IBD.ToParam("TesauroID"), IBD.TipoGuidToObject(DbType.Guid), "TesauroID", DataRowVersion.Current);
                    mBaseDatos.AddInParameter(InsertCategoriaTesVinSuscripCommand, IBD.ToParam("CategoriaTesauroID"), IBD.TipoGuidToObject(DbType.Guid), "CategoriaTesauroID", DataRowVersion.Current);

                    DbCommand ModifyCategoriaTesVinSuscripCommand = mBaseDatos.GetSqlStringCommand(sqlCategoriaTesVinSuscripModify);
                    mBaseDatos.AddInParameter(ModifyCategoriaTesVinSuscripCommand, IBD.ToParam("O_SuscripcionID"), IBD.TipoGuidToObject(DbType.Guid), "SuscripcionID", DataRowVersion.Original);
                    mBaseDatos.AddInParameter(ModifyCategoriaTesVinSuscripCommand, IBD.ToParam("O_TesauroID"), IBD.TipoGuidToObject(DbType.Guid), "TesauroID", DataRowVersion.Original);
                    mBaseDatos.AddInParameter(ModifyCategoriaTesVinSuscripCommand, IBD.ToParam("O_CategoriaTesauroID"), IBD.TipoGuidToObject(DbType.Guid), "CategoriaTesauroID", DataRowVersion.Original);

                    mBaseDatos.AddInParameter(ModifyCategoriaTesVinSuscripCommand, IBD.ToParam("SuscripcionID"), IBD.TipoGuidToObject(DbType.Guid), "SuscripcionID", DataRowVersion.Current);
                    mBaseDatos.AddInParameter(ModifyCategoriaTesVinSuscripCommand, IBD.ToParam("TesauroID"), IBD.TipoGuidToObject(DbType.Guid), "TesauroID", DataRowVersion.Current);
                    mBaseDatos.AddInParameter(ModifyCategoriaTesVinSuscripCommand, IBD.ToParam("CategoriaTesauroID"), IBD.TipoGuidToObject(DbType.Guid), "CategoriaTesauroID", DataRowVersion.Current);
                    mBaseDatos.UpdateDataSet(addedAndModifiedDataSet, "CategoriaTesVinSuscrip", InsertCategoriaTesVinSuscripCommand, ModifyCategoriaTesVinSuscripCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion
                    
                    addedAndModifiedDataSet.Dispose();
                }
                #endregion
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Elimina las filas de la BD que se han eliminado en el dataset
        /// </summary>
        /// <param name="pSuscripcionDS">Dataset de suscripciones</param>
        public void EliminarSuscripcion(DataSet pSuscripcionDS)
        {
            try
            {
                DataSet deletedDataSet;
                deletedDataSet = pSuscripcionDS.GetChanges(DataRowState.Deleted);
                
                if (deletedDataSet != null)
                {
                    #region Eliminar tabla CategoriaTesVinSuscrip

                    DbCommand DeleteCategoriaTesVinSuscripCommand = mBaseDatos.GetSqlStringCommand(sqlCategoriaTesVinSuscripDelete);
                    mBaseDatos.AddInParameter(DeleteCategoriaTesVinSuscripCommand, IBD.ToParam("O_SuscripcionID"), IBD.TipoGuidToObject(DbType.Guid), "SuscripcionID", DataRowVersion.Original);
                    mBaseDatos.AddInParameter(DeleteCategoriaTesVinSuscripCommand, IBD.ToParam("O_TesauroID"), IBD.TipoGuidToObject(DbType.Guid), "TesauroID", DataRowVersion.Original);
                    mBaseDatos.AddInParameter(DeleteCategoriaTesVinSuscripCommand, IBD.ToParam("O_CategoriaTesauroID"), IBD.TipoGuidToObject(DbType.Guid), "CategoriaTesauroID", DataRowVersion.Original);
                    mBaseDatos.UpdateDataSet(deletedDataSet, "CategoriaTesVinSuscrip", null, null, DeleteCategoriaTesVinSuscripCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #region Eliminar tabla SuscripcionTesauroUsuario

                    DbCommand DeleteSuscripcionTesauroUsuarioCommand = mBaseDatos.GetSqlStringCommand(sqlSuscripcionTesauroUsuarioDelete);
                    mBaseDatos.AddInParameter(DeleteSuscripcionTesauroUsuarioCommand, IBD.ToParam("O_SuscripcionID"), IBD.TipoGuidToObject(DbType.Guid), "SuscripcionID", DataRowVersion.Original);
                    mBaseDatos.AddInParameter(DeleteSuscripcionTesauroUsuarioCommand, IBD.ToParam("O_UsuarioID"), IBD.TipoGuidToObject(DbType.Guid), "UsuarioID", DataRowVersion.Original);
                    mBaseDatos.AddInParameter(DeleteSuscripcionTesauroUsuarioCommand, IBD.ToParam("O_TesauroID"), IBD.TipoGuidToObject(DbType.Guid), "TesauroID", DataRowVersion.Original);
                    mBaseDatos.UpdateDataSet(deletedDataSet, "SuscripcionTesauroUsuario", null, null, DeleteSuscripcionTesauroUsuarioCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #region Eliminar tabla SuscripcionTesauroProyecto

                    DbCommand DeleteSuscripcionTesauroProyectoCommand = mBaseDatos.GetSqlStringCommand(sqlSuscripcionTesauroProyectoDelete);
                    mBaseDatos.AddInParameter(DeleteSuscripcionTesauroProyectoCommand, IBD.ToParam("O_SuscripcionID"), IBD.TipoGuidToObject(DbType.Guid), "SuscripcionID", DataRowVersion.Original);
                    mBaseDatos.AddInParameter(DeleteSuscripcionTesauroProyectoCommand, IBD.ToParam("O_OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Original);
                    mBaseDatos.AddInParameter(DeleteSuscripcionTesauroProyectoCommand, IBD.ToParam("O_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);
                    mBaseDatos.AddInParameter(DeleteSuscripcionTesauroProyectoCommand, IBD.ToParam("O_TesauroID"), IBD.TipoGuidToObject(DbType.Guid), "TesauroID", DataRowVersion.Original);
                    mBaseDatos.UpdateDataSet(deletedDataSet, "SuscripcionTesauroProyecto", null, null, DeleteSuscripcionTesauroProyectoCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #region Eliminar tabla SuscripcionBlog

                    DbCommand DeleteSuscripcionBlogCommand = mBaseDatos.GetSqlStringCommand(sqlSuscripcionBlogDelete);
                    mBaseDatos.AddInParameter(DeleteSuscripcionBlogCommand, IBD.ToParam("O_SuscripcionID"), IBD.TipoGuidToObject(DbType.Guid), "SuscripcionID", DataRowVersion.Original);
                    mBaseDatos.AddInParameter(DeleteSuscripcionBlogCommand, IBD.ToParam("O_BlogID"), IBD.TipoGuidToObject(DbType.Guid), "BlogID", DataRowVersion.Original);
                    mBaseDatos.UpdateDataSet(deletedDataSet, "SuscripcionBlog", null, null, DeleteSuscripcionBlogCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #region Eliminar tabla Suscripcion

                    DbCommand DeleteSuscripcionCommand = mBaseDatos.GetSqlStringCommand(sqlSuscripcionDelete);
                    mBaseDatos.AddInParameter(DeleteSuscripcionCommand, IBD.ToParam("O_SuscripcionID"), IBD.TipoGuidToObject(DbType.Guid), "SuscripcionID", DataRowVersion.Original);
                    mBaseDatos.AddInParameter(DeleteSuscripcionCommand, IBD.ToParam("O_Periodicidad"), DbType.Int32, "Periodicidad", DataRowVersion.Original);
                    mBaseDatos.AddInParameter(DeleteSuscripcionCommand, IBD.ToParam("O_IdentidadID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadID", DataRowVersion.Original);
                    mBaseDatos.AddInParameter(DeleteSuscripcionCommand, IBD.ToParam("O_Bloqueada"), DbType.Boolean, "Bloqueada", DataRowVersion.Original);
                    mBaseDatos.UpdateDataSet(deletedDataSet, "Suscripcion", null, null, DeleteSuscripcionCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Actualiza los datos del dataset en la BD
        /// </summary>
        /// <param name="pSuscripcionDS">Dataset de suscripciones</param>
        public void ActualizarSuscripcion(DataSet pSuscripcionDS)
        {
            try
            {
                EliminarSuscripcion(pSuscripcionDS);
                GuardarSuscripcion(pSuscripcionDS);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Obtiene las suscripciones de una identidad (las suscripciones, no sus elementos)
        /// </summary>
        /// <param name="pIdentidadID">Identificador de identidad</param>
        /// <param name="pObtenerBloqueadas">TRUE si queremos obtener las suscripciones bloquedas además de las no bloqueadas</param>
        /// <returns>Dataset de suscripciones cargado con las suscripciones (Suscripcion,SuscripcionTesauroUsuario,
        /// SuscripcionTesauroProyecto,CategoriaTesVinSuscrip)</returns>
        public SuscripcionDS ObtenerSuscripcionesDeIdentidad(Guid pIdentidadID, bool pObtenerBloqueadas )
        {
            SuscripcionDS suscDS = new SuscripcionDS();

            //Suscripcion
            string selectSuscripcion = sqlSelectSuscripciones + " Where Suscripcion.IdentidadID=" + IBD.ToParam("IdentidadID");
            
            if (!pObtenerBloqueadas)
            { 
               sqlSelectSuscripciones = sqlSelectSuscripciones + " AND Suscripcion.Bloqueada = 0";
            }
            DbCommand commandSuscripcion = mBaseDatos.GetSqlStringCommand(selectSuscripcion);
            mBaseDatos.AddInParameter(commandSuscripcion, IBD.ToParam("IdentidadID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pIdentidadID));
            mBaseDatos.LoadDataSet(commandSuscripcion, suscDS, "Suscripcion");

            //SuscripcionTesauroUsuario
            string selectSuscTesUsuario = sqlSelectSuscripcionTesauroUsuario + " INNER JOIN Suscripcion ON Suscripcion.SuscripcionID = SuscripcionTesauroUsuario.SuscripcionID Where Suscripcion.IdentidadID=" + IBD.ToParam("IdentidadID");
           
            if (!pObtenerBloqueadas)
            {
                selectSuscTesUsuario = selectSuscTesUsuario + " AND Suscripcion.Bloqueada = 0";
            }
            DbCommand commandSuscTesUsuario = mBaseDatos.GetSqlStringCommand(selectSuscTesUsuario);
            mBaseDatos.AddInParameter(commandSuscTesUsuario, IBD.ToParam("IdentidadID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pIdentidadID));
            mBaseDatos.LoadDataSet(commandSuscTesUsuario, suscDS, "SuscripcionTesauroUsuario");

            //SuscripcionTesauroProyecto
            string selectSuscTesProyecto = sqlSelectSuscripcionTesauroProyecto + " INNER JOIN Suscripcion ON Suscripcion.SuscripcionID = SuscripcionTesauroProyecto.SuscripcionID Where Suscripcion.IdentidadID=" + IBD.ToParam("IdentidadID");
            
            if (!pObtenerBloqueadas)
            {
                selectSuscTesProyecto = selectSuscTesProyecto + " AND Suscripcion.Bloqueada = 0";
            }
            DbCommand commandSuscTesProyecto = mBaseDatos.GetSqlStringCommand(selectSuscTesProyecto);
            mBaseDatos.AddInParameter(commandSuscTesProyecto, IBD.ToParam("IdentidadID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pIdentidadID));
            mBaseDatos.LoadDataSet(commandSuscTesProyecto, suscDS, "SuscripcionTesauroProyecto");

            //CategoriaTesVinSuscrip
            string selectCatTesVinSuscrip = sqlSelectCategoriasTesVinSuscrip + " INNER JOIN Suscripcion ON Suscripcion.SuscripcionID = CategoriaTesVinSuscrip.SuscripcionID Where Suscripcion.IdentidadID=" + IBD.ToParam("IdentidadID");
            
            if (!pObtenerBloqueadas)
            {
                selectCatTesVinSuscrip = selectCatTesVinSuscrip + " AND Suscripcion.Bloqueada = 0";
            }
            DbCommand commandCatTesVinSuscrip = mBaseDatos.GetSqlStringCommand(selectCatTesVinSuscrip);
            mBaseDatos.AddInParameter(commandCatTesVinSuscrip, IBD.ToParam("IdentidadID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pIdentidadID));
            mBaseDatos.LoadDataSet(commandCatTesVinSuscrip, suscDS, "CategoriaTesVinSuscrip");

            //SuscripcionBlog
            string selectSuscBlog = sqlSelectSuscripcionBlog + " INNER JOIN Suscripcion ON Suscripcion.SuscripcionID = SuscripcionBlog.SuscripcionID Where Suscripcion.IdentidadID=" + IBD.ToParam("IdentidadID");
           
            if (!pObtenerBloqueadas)
            {
                selectCatTesVinSuscrip = selectCatTesVinSuscrip + " AND Suscripcion.Bloqueada = 0";
            }
            DbCommand commandSuscBlog = mBaseDatos.GetSqlStringCommand(selectSuscBlog);
            mBaseDatos.AddInParameter(commandSuscBlog, IBD.ToParam("IdentidadID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pIdentidadID));
            mBaseDatos.LoadDataSet(commandSuscBlog, suscDS, "SuscripcionBlog");

            return suscDS;
        }

        /// <summary>
        /// Obtiene las suscripciones a blogs de una identidad
        /// </summary>
        /// <param name="pIdentidadID">Identificador de identidad</param>
        /// <param name="pObtenerBloqueadas">TRUE si queremos obtener las suscripciones bloquedas además de las no bloqueadas</param>
        /// <returns>SuscripcionDS cargado con las suscripciones 
        /// (Suscripcion, SuscripcionTesauroUsuario, SuscripcionTesauroProyecto, CategoriaTesVinSuscrip)</returns>
        public SuscripcionDS ObtenerSuscripcionesDeBlog(Guid pIdentidadID, bool pObtenerBloqueadas)
        {
            SuscripcionDS suscDS = new SuscripcionDS();

            //Suscripcion
            string selectSuscripcion = sqlSelectSuscripciones + " Where Suscripcion.IdentidadID=" + IBD.ToParam("IdentidadID");
            
            if (!pObtenerBloqueadas)
            { 
              selectSuscripcion = selectSuscripcion + " AND Suscripcion.Bloqueada = 0";
            }
            DbCommand commandSuscripcion = mBaseDatos.GetSqlStringCommand(selectSuscripcion);
            mBaseDatos.AddInParameter(commandSuscripcion, IBD.ToParam("IdentidadID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pIdentidadID));
            mBaseDatos.LoadDataSet(commandSuscripcion, suscDS, "Suscripcion");

            //SuscripcionBlog
            string selectSuscBlog = sqlSelectSuscripcionBlog + " INNER JOIN Suscripcion ON Suscripcion.SuscripcionID = SuscripcionBlog.SuscripcionID Where Suscripcion.IdentidadID=" + IBD.ToParam("IdentidadID");
            
            if (!pObtenerBloqueadas)
            {
                selectSuscBlog = selectSuscBlog + " AND Suscripcion.Bloqueada = 0";
            }
            DbCommand commandSuscBlog = mBaseDatos.GetSqlStringCommand(selectSuscBlog);
            mBaseDatos.AddInParameter(commandSuscBlog, IBD.ToParam("IdentidadID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pIdentidadID));
            mBaseDatos.LoadDataSet(commandSuscBlog, suscDS, "SuscripcionBlog");

            return suscDS;
        }

        /// <summary>
        /// Obtiene las suscripciones al blog cuyo identificador se pasa por parámetro
        /// </summary>
        /// <param name="pBlogID">Identificador de blog</param>
        /// <returns>Dataset de suscripciones</returns>
        public SuscripcionDS ObtenerSuscripcionesABlog(Guid pBlogID)
        {
            SuscripcionDS suscDS = new SuscripcionDS();

            //Suscripcion
            string selectSuscripcion = sqlSelectSuscripciones + " INNER JOIN SuscripcionBlog ON Suscripcion.SuscripcionID = SuscripcionBlog.SuscripcionID Where SuscripcionBlog.BlogID=" + IBD.ToParam("BlogID");
            
            DbCommand commandSuscripcion = mBaseDatos.GetSqlStringCommand(selectSuscripcion);
            mBaseDatos.AddInParameter(commandSuscripcion, IBD.ToParam("BlogID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pBlogID));
            mBaseDatos.LoadDataSet(commandSuscripcion, suscDS, "Suscripcion");

            //SuscripcionBlog
            string selectSuscBlog = sqlSelectSuscripcionBlog + " INNER JOIN Suscripcion ON Suscripcion.SuscripcionID = SuscripcionBlog.SuscripcionID Where SuscripcionBlog.BlogID=" + IBD.ToParam("BlogID");

            DbCommand commandSuscBlog = mBaseDatos.GetSqlStringCommand(selectSuscBlog);
            mBaseDatos.AddInParameter(commandSuscBlog, IBD.ToParam("BlogID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pBlogID));
            mBaseDatos.LoadDataSet(commandSuscBlog, suscDS, "SuscripcionBlog");

            return suscDS;
        }

        /// <summary>
        /// Obtiene las suscripciones de un perfil (las suscripciones, no sus elementos)
        /// </summary>
        /// <param name="pPerfilID">PerfilID</param>
        /// <param name="pObtenerCamposDesnormalizados">True si se quieren obtener campos desnormalizados como el nombre de 
        /// las categorias suscritas, de los blogs, usuarios etc...</param>
        /// <param name="pObtenerBloqueadas">True si queremos obtener las suscripciones bloquedas ademas de las no bloqueadas</param>
        /// <returns>SuscripcionDS cargado con las suscripciones (Suscripcion,SuscripcionTesauroUsuario,
        /// SuscripcionTesauroProyecto,CategoriaTesVinSuscrip)</returns>
        public SuscripcionDS ObtenerSuscripcionesDePerfil(Guid pPerfilID, bool pObtenerCamposDesnormalizados, bool pObtenerBloqueadas)
        {
            SuscripcionDS suscDS = new SuscripcionDS();

            //Suscripcion
            string selectSuscripcion = "";
            
            if (pObtenerCamposDesnormalizados)
            {
                selectSuscripcion = sqlSelectSuscripciones + " INNER JOIN Identidad ON Identidad.IdentidadID = Suscripcion.IdentidadID Where Identidad.PerfilID=" + IBD.ToParam("PerfilID");
                
                if (!pObtenerBloqueadas)
                {
                    selectSuscripcion += " AND Suscripcion.Bloqueada = 0 ";
                }
            }
            else
            {
                selectSuscripcion = sqlSelectSuscripciones + " INNER JOIN Identidad ON Identidad.IdentidadID = Suscripcion.IdentidadID Where Identidad.PerfilID=" + IBD.ToParam("PerfilID");
                
                if (!pObtenerBloqueadas)
                {
                    selectSuscripcion += " AND Suscripcion.Bloqueada = 0 ";
                }
            }
            DbCommand commandSuscripcion = mBaseDatos.GetSqlStringCommand(selectSuscripcion);
            mBaseDatos.AddInParameter(commandSuscripcion, IBD.ToParam("PerfilID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pPerfilID));
            mBaseDatos.LoadDataSet(commandSuscripcion, suscDS, "Suscripcion");

            //SuscripcionTesauroUsuario
            string selectSuscTesUsuario = "";
            
            if (pObtenerCamposDesnormalizados)
            {
                selectSuscTesUsuario = sqlSelectSuscripcionTesauroUsuarioSimple + ", Perfil.NombrePerfil as NombreUsuario, IdentidadDestino.IdentidadID IdentidadUsuario from SuscripcionTesauroUsuario INNER JOIN Suscripcion ON Suscripcion.SuscripcionID = SuscripcionTesauroUsuario.SuscripcionID INNER JOIN Identidad ON Identidad.IdentidadID = Suscripcion.IdentidadID INNER JOIN Persona ON Persona.UsuarioID = SuscripcionTesauroUsuario.UsuarioID INNER JOIN PerfilPersona ON Persona.PersonaID = PerfilPersona.PersonaID INNER JOIN Perfil ON Perfil.PerfilID = PerfilPersona.PerfilID INNER JOIN Identidad IdentidadDestino ON IdentidadDestino.PerfilID = PerfilPersona.PerfilID Where Identidad.PerfilID=" + IBD.ToParam("PerfilID") + " AND IdentidadDestino.ProyectoID = '" + IBD.ValorDeGuid(ProyectoAD.MetaProyecto) + "'";
                
                if (!pObtenerBloqueadas)
                {
                    selectSuscTesUsuario += " AND Suscripcion.Bloqueada = 0 ";
                }
            }
            else
            {
                selectSuscTesUsuario = sqlSelectSuscripcionTesauroUsuario + " INNER JOIN Suscripcion ON Suscripcion.SuscripcionID = SuscripcionTesauroUsuario.SuscripcionID INNER JOIN Identidad ON Identidad.IdentidadID = Suscripcion.IdentidadID Where Identidad.PerfilID=" + IBD.ToParam("PerfilID");

                if (!pObtenerBloqueadas)
                {
                    selectSuscTesUsuario += " AND Suscripcion.Bloqueada = 0 ";
                }
            }
            DbCommand commandSuscTesUsuario = mBaseDatos.GetSqlStringCommand(selectSuscTesUsuario);
            mBaseDatos.AddInParameter(commandSuscTesUsuario, IBD.ToParam("PerfilID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pPerfilID));
            mBaseDatos.LoadDataSet(commandSuscTesUsuario, suscDS, "SuscripcionTesauroUsuario");

            //SuscripcionTesauroProyecto
            string selectSuscTesProyecto = "";
            
            if (pObtenerCamposDesnormalizados)
            {
                selectSuscTesProyecto = sqlSelectSuscripcionTesauroProyectoSimple + ", Proyecto.Nombre as NombreProyecto,Proyecto.NombreCorto as NombreCortoProyecto, ParametroGeneral.NombreImagenPeque as ImagenProyecto from SuscripcionTesauroProyecto INNER JOIN Suscripcion ON Suscripcion.SuscripcionID = SuscripcionTesauroProyecto.SuscripcionID INNER JOIN Identidad ON Identidad.IdentidadID = Suscripcion.IdentidadID INNER JOIN Proyecto ON Proyecto.ProyectoID = SuscripcionTesauroProyecto.ProyectoID INNER JOIN ParametroGeneral ON ParametroGeneral.ProyectoID = Proyecto.ProyectoID AND ParametroGeneral.OrganizacionID = Proyecto.OrganizacionID Where Identidad.PerfilID=" + IBD.ToParam("PerfilID");

                if (!pObtenerBloqueadas)
                {
                    selectSuscTesProyecto += " AND Suscripcion.Bloqueada = 0 ";
                }
            }
            else
            {
                selectSuscTesProyecto = sqlSelectSuscripcionTesauroProyecto + " INNER JOIN Suscripcion ON Suscripcion.SuscripcionID = SuscripcionTesauroProyecto.SuscripcionID INNER JOIN Identidad ON Identidad.IdentidadID = Suscripcion.IdentidadID Where Identidad.PerfilID=" + IBD.ToParam("PerfilID");

                if (!pObtenerBloqueadas)
                {
                    selectSuscTesProyecto += " AND Suscripcion.Bloqueada = 0 ";
                }
            }
            DbCommand commandSuscTesProyecto = mBaseDatos.GetSqlStringCommand(selectSuscTesProyecto);
            mBaseDatos.AddInParameter(commandSuscTesProyecto, IBD.ToParam("PerfilID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pPerfilID));
            mBaseDatos.LoadDataSet(commandSuscTesProyecto, suscDS, "SuscripcionTesauroProyecto");

            //CategoriaTesVinSuscrip
            string selectCatTesVinSuscrip="";
            
            if (pObtenerCamposDesnormalizados)
            {
                selectCatTesVinSuscrip = sqlSelectCategoriasTesVinSuscripSimple + ",CategoriaTesauro.Nombre as NombreCategoria from CategoriaTesVinSuscrip INNER JOIN Suscripcion ON Suscripcion.SuscripcionID = CategoriaTesVinSuscrip.SuscripcionID INNER JOIN Identidad ON Identidad.IdentidadID = Suscripcion.IdentidadID INNER JOIN CategoriaTesauro ON CategoriaTesauro.CategoriaTesauroID = CategoriaTesVinSuscrip.CategoriaTesauroID Where Identidad.PerfilID=" + IBD.ToParam("PerfilID");
                if (!pObtenerBloqueadas)
                {
                    selectCatTesVinSuscrip += " AND Suscripcion.Bloqueada = 0 ";
                }
            }
            else
            {
                selectCatTesVinSuscrip = sqlSelectCategoriasTesVinSuscrip + " INNER JOIN Suscripcion ON Suscripcion.SuscripcionID = CategoriaTesVinSuscrip.SuscripcionID INNER JOIN Identidad ON Identidad.IdentidadID = Suscripcion.IdentidadID Where Identidad.PerfilID=" + IBD.ToParam("PerfilID");

                if (!pObtenerBloqueadas)
                {
                    selectCatTesVinSuscrip += " AND Suscripcion.Bloqueada = 0 ";
                }
            }
            DbCommand commandCatTesVinSuscrip = mBaseDatos.GetSqlStringCommand(selectCatTesVinSuscrip);
            mBaseDatos.AddInParameter(commandCatTesVinSuscrip, IBD.ToParam("PerfilID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pPerfilID));
            mBaseDatos.LoadDataSet(commandCatTesVinSuscrip, suscDS, "CategoriaTesVinSuscrip");

            //SuscripcionBlog
            string selectSuscripcionBlog = "";
            
            if (pObtenerCamposDesnormalizados)
            {
                selectSuscripcionBlog = sqlSelectSuscripcionBlogSimple + ",Blog.Titulo as NombreBlog from SuscripcionBlog INNER JOIN Suscripcion ON Suscripcion.SuscripcionID = SuscripcionBlog.SuscripcionID INNER JOIN Identidad ON Identidad.IdentidadID = Suscripcion.IdentidadID INNER JOIN Blog ON Blog.BlogID = SuscripcionBlog.BlogID Where Identidad.PerfilID=" + IBD.ToParam("PerfilID");
                selectSuscripcionBlog += " AND Suscripcion.Bloqueada = 0 ";
            }
            else
            {
                selectSuscripcionBlog = sqlSelectSuscripcionBlog + " INNER JOIN Suscripcion ON Suscripcion.SuscripcionID = SuscripcionBlog.SuscripcionID INNER JOIN Identidad ON Identidad.IdentidadID = Suscripcion.IdentidadID Where Identidad.PerfilID=" + IBD.ToParam("PerfilID");
                selectSuscripcionBlog += " AND Suscripcion.Bloqueada = 0 ";
            }
            DbCommand commandSuscripcionBlog = mBaseDatos.GetSqlStringCommand(selectSuscripcionBlog);
            mBaseDatos.AddInParameter(commandSuscripcionBlog, IBD.ToParam("PerfilID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pPerfilID));
            mBaseDatos.LoadDataSet(commandSuscripcionBlog, suscDS, "SuscripcionBlog");

            return suscDS;
        }

        /// <summary>
        /// Obtiene los resultados de las suscripciones (ResultadoSuscripcion) de un perfil
        /// </summary>
        /// <param name="pResultadosDS">Dataset de resultados de suscripciones</param>
        /// <param name="pPerfilID">Id del perfil para el que se obtienen los resultados</param>
        /// <param name="pInicio">Elemento inicial para paginar</param>
        /// <param name="pLimite">Elemento final para paginar</param>
        /// <param name="pFechaInicical">Fecha inicial para los resultados a partir de la que se obtienen</param>
        /// <param name="pOrderBy">string para ordenar la consulta</param>
        /// <param name="pObtenerBloqueadas">true si se desea traer los resultados de suscripciones bloqueadas</param>
        /// <returns></returns>
        public int ObtenerResultadosSuscripcionesDePerfil(ResultadosSuscripcionDS pResultadosDS, Guid pPerfilID, int pInicio, int pLimite, DateTime pFechaInicical, string pOrderBy, bool pObtenerBloqueadas)
        {
            ResultadosSuscripcionDS resDS = new ResultadosSuscripcionDS();
            int cuantos = 0;

            #region ResultadoSuscripcion
            string selectGlobal = "";
            string parteSelectGlobal = "";
            string parteWithGlobal = "";
            string selectCategoriasSuscripcion = "SELECT CategoriaTesauroID from categoriaTesVinSuscrip INNER JOIN Suscripcion ON Suscripcion.SuscripcionID = categoriaTesVinSuscrip.SuscripcionID INNER JOIN Identidad ON Identidad.IdentidadID = Suscripcion.IdentidadID";

            #region Filtro Identidad;
            
            string whereFiltroIdentidad = " WHERE Identidad.PerfilID = " + IBD.GuidParamValor("PerfilID");
            if (!pObtenerBloqueadas)
            {
                whereFiltroIdentidad += " AND Suscripcion.Bloqueada = 0";
            }
            
            selectCategoriasSuscripcion += whereFiltroIdentidad;

            #endregion

            #region Proyecto

            string selectDocComunidades = "SELECT DISTINCT Documento.DocumentoID Id, Documento.Titulo, Documento.Descripcion," + ((int)TipoResultadoSuscripcion.RecursoComunidad) + " TipoResultado, Documento.FechaModificacion Fecha, NULL IdAutor, Proyecto.Nombre NombrePadre,Proyecto.NombreCorto NombreCortoPadre,Proyecto.ProyectoID IdPadre, Documento.Tipo TipoDocumento, Documento.Enlace UrlDocumento";

            string fromDocComunidades = " FROM Documento INNER JOIN DocumentoWebAgCatTesauro ON DocumentoWebAgCatTesauro.DocumentoID = Documento.DocumentoID INNER JOIN (" + selectCategoriasSuscripcion + ")tmp_CatTesSuscrip ON DocumentoWebAgCatTesauro.CategoriaTesauroID = tmp_CatTesSuscrip.CategoriaTesauroID INNER JOIN CategoriaTesauro ON DocumentoWebAgCatTesauro.CategoriaTesauroID = CategoriaTesauro.CategoriaTesauroID INNER JOIN TesauroProyecto ON CategoriaTesauro.TesauroID = TesauroProyecto.TesauroID INNER JOIN Proyecto ON TesauroProyecto.ProyectoID = Proyecto.ProyectoID INNER JOIN DocumentoWebVinBaseRecursos ON Documento.DocumentoID = DocumentoWebVinBaseRecursos.DocumentoID INNER JOIN BaseRecursosProyecto ON (DocumentoWebVinBaseRecursos.BaseRecursosID = BaseRecursosProyecto.BaseRecursosID AND BaseRecursosProyecto.ProyectoID = Proyecto.ProyectoID)";

            string whereDocComunidades = " WHERE Documento.Borrador = 0 AND Documento.UltimaVersion = 1 AND Documento.Eliminado = 0 AND DocumentoWebVinBaseRecursos.Eliminado = 0 AND Documento.FechaModificacion > " + IBD.ToParam("FechaInicial");
            
            string selectGlobalComunidades = selectDocComunidades + fromDocComunidades + whereDocComunidades;

#endregion

            #region Personas

            string selectCategoriaPublica = "SELECT DISTINCT CategoriaTesauro.CategoriaTesauroID from CategoriaTesauro INNER JOIN SuscripcionTesauroUsuario ON CategoriaTesauro.TesauroID = SuscripcionTesauroUsuario.TesauroID INNER JOIN Usuario ON Usuario.UsuarioID = SuscripcionTesauroUsuario.UsuarioID INNER JOIN Persona ON Usuario.UsuarioID = Persona.UsuarioID INNER JOIN ConfiguracionGnossPersona ON Persona.PersonaID = ConfiguracionGnossPersona.PersonaID INNER JOIN Suscripcion ON Suscripcion.SuscripcionID = SuscripcionTesauroUsuario.SuscripcionID INNER JOIN TesauroUsuario ON TesauroUsuario.TesauroID = CategoriaTesauro.TesauroID INNER JOIN Identidad ON Suscripcion.IdentidadID = Identidad.identidadID" + whereFiltroIdentidad + " AND CategoriaTesauro.CategoriaTesauroID = TesauroUsuario.CategoriaTesauroPublicoID AND ConfiguracionGnossPersona.VerRecursos = 1";

            string selectDocPersonas = "SELECT DISTINCT Documento.DocumentoID Id, Documento.Titulo, Documento.Descripcion, " + ((int)TipoResultadoSuscripcion.RecursoPersona) + " TipoResultado, Documento.FechaModificacion Fecha, IdentCreador.IdentidadID IdAutor, Perfil.NombrePerfil NombrePadre, Perfil.NombreCortoUsu NombreCortoPadre, IdentCreador.IdentidadID IdPadre, Documento.Tipo TipoDocumento,  Documento.Enlace UrlDocumento";

            string fromDocPersonas = "FROM Documento INNER JOIN DocumentoWebAgCatTesauro ON DocumentoWebAgCatTesauro.DocumentoID = Documento.DocumentoID INNER JOIN tmp_CatTesSuscripDoc ON DocumentoWebAgCatTesauro.CategoriaTesauroID = tmp_CatTesSuscripDoc.CategoriaInferiorID INNER JOIN CategoriaTesauro ON CategoriaTesauro.CategoriaTesauroID = DocumentoWebAgCatTesauro.CategoriaTesauroID INNER JOIN SuscripcionTesauroUsuario ON DocumentoWebAgCatTesauro.TesauroID = SuscripcionTesauroUsuario.TesauroID INNER JOIN Persona ON SuscripcionTesauroUsuario.UsuarioID = Persona.UsuarioID INNER JOIN PerfilPersona ON Persona.PersonaID = PerfilPersona.PersonaID INNER JOIN Perfil ON PerfilPersona.PerfilID = Perfil.PerfilID INNER JOIN Identidad IdentCreador ON IdentCreador.PerfilID = Perfil.PerfilID INNER JOIN Suscripcion ON Suscripcion.SuscripcionID = SuscripcionTesauroUsuario.SuscripcionID INNER JOIN Identidad ON Identidad.IdentidadID = Suscripcion.IdentidadID INNER JOIN DocumentoWebVinBaseRecursos ON Documento.DocumentoID = DocumentoWebVinBaseRecursos.DocumentoID INNER JOIN BaseRecursosUsuario ON (DocumentoWebVinBaseRecursos.BaseRecursosID = BaseRecursosUsuario.BaseRecursosID AND BaseRecursosUsuario.UsuarioID = Persona.UsuarioID)";

            string whereDocPersonas = whereFiltroIdentidad + " AND IdentCreador.ProyectoID = '" + ProyectoAD.MetaProyecto.ToString() + "' AND Documento.Borrador = 0 AND Documento.UltimaVersion = 1 AND Documento.Eliminado = 0 AND DocumentoWebVinBaseRecursos.Eliminado = 0 AND Documento.FechaModificacion > " + IBD.ToParam("FechaInicial");

            IConsultaJerarquica consultaJerarquicaPersonas = IBD.CrearConsultaJerarquicaDeGrafo(selectDocPersonas, fromDocPersonas, whereDocPersonas, "CategoriaSuperiorID", "CategoriaInferiorID", "CatTesauroAgCatTesauro", null, null, "tmp_CatTesSuscripDoc", "CategoriaTesauroID in (" + selectCategoriaPublica + ")", false, "CategoriaTesauro", "CategoriaTesauroID");

            #endregion

            #region Blog

            string selectEntradasBlogs = "select DISTINCT ent.EntradaBlogID Id, ent.Titulo, ent.Texto Descripcion, " + ((int)TipoResultadoSuscripcion.EntradaBlogPersona) + " TipoResultado, ent.FechaModificacion Fecha, ent.AutorID IdAutor, Blog.Titulo NombrePadre, Blog.NombreCorto NombreCortoPadre, Blog.BlogID IdPadre, NULL TipoDocumento, NULL UrlDocumento from EntradaBlog ent INNER JOIN Blog ON ent.BlogID = Blog.BlogID INNER JOIN Identidad IdentAutor ON IdentAutor.IdentidadID = ent.AutorID INNER JOIN Perfil ON Perfil.PerfilID = IdentAutor.PerfilID INNER JOIN SuscripcionBlog ON SuscripcionBlog.BlogID = ent.BlogID INNER JOIN Suscripcion ON Suscripcion.SuscripcionID = SuscripcionBlog.SuscripcionID INNER JOIN Identidad ON Identidad.IdentidadID = Suscripcion.IdentidadID" + whereFiltroIdentidad + " AND ent.FechaModificacion > " + IBD.ToParam("FechaInicial") + " AND ent.Borrador = 0 AND ent.Eliminado = 0";
            
            #endregion

            if (IBD is BDSqlServer)
            {
                consultaJerarquicaPersonas = IBD.CrearConsultaJerarquicaDeGrafo(selectDocPersonas, fromDocPersonas, whereDocPersonas + " UNION ALL " + selectEntradasBlogs + " UNION ALL " + selectGlobalComunidades, "CategoriaSuperiorID", "CategoriaInferiorID", "CatTesauroAgCatTesauro", null, null, "tmp_CatTesSuscripDoc", "CategoriaTesauroID IN (" + selectCategoriaPublica + ")", false, "CategoriaTesauro", "CategoriaTesauroID");

                parteWithGlobal = ((ConsultaJerarquicaSqlServer)consultaJerarquicaPersonas).ParteWith;

                parteSelectGlobal = ((ConsultaJerarquicaSqlServer)consultaJerarquicaPersonas).ParteSelect;

                selectGlobal = consultaJerarquicaPersonas.ConsultaJerarquica;
            }
            else if (IBD is BDOracle)
            {

            }

            DbCommand comandoResultados = mBaseDatos.GetSqlStringCommand(selectGlobal);
            mBaseDatos.AddInParameter(comandoResultados, IBD.GuidParamValor("PerfilID"), DbType.Guid, IBD.ValorDeGuid(pPerfilID));
            mBaseDatos.AddInParameter(comandoResultados, IBD.ToParam("FechaInicial"), DbType.DateTime, pFechaInicical);
            
            if (IBD is BDSqlServer)
            {
                cuantos = ((BDSqlServer)IBD).PaginarDatasetConsultaJerarquicaManual(comandoResultados, parteWithGlobal, parteSelectGlobal, pOrderBy, resDS, pInicio, pLimite, "ResultadoSuscripcion");
            }
            else if (IBD is BDOracle)
            {

            }

            #endregion

            #region ResultadoSuscAgCatTesauro
            //ResultadoSuscID,CatTesauroID

            string selectGlobalAgCat = "";
            string parteSelectGlobalAgCat = "";
            string parteWithGlobalAgCat = "";

            #region Proyecto

            string selectDocComunidadesAgCat = "SELECT DISTINCT DocumentoWebAgCatTesauro.DocumentoID ResultadoSuscID, Proyecto.ProyectoID IdPadre, DocumentoWebAgCatTesauro.CategoriaTesauroID CatTesauroID";

            string fromDocComunidadesAgCat = " FROM Documento INNER JOIN DocumentoWebAgCatTesauro ON DocumentoWebAgCatTesauro.DocumentoID = Documento.DocumentoID INNER JOIN (" + selectCategoriasSuscripcion + ")tmp_CatTesSuscripDoc ON DocumentoWebAgCatTesauro.CategoriaTesauroID = tmp_CatTesSuscripDoc.CategoriaTesauroID INNER JOIN CategoriaTesauro ON DocumentoWebAgCatTesauro.CategoriaTesauroID = CategoriaTesauro.CategoriaTesauroID INNER JOIN TesauroProyecto ON CategoriaTesauro.TesauroID = TesauroProyecto.TesauroID INNER JOIN Proyecto ON TesauroProyecto.ProyectoID = Proyecto.ProyectoID INNER JOIN DocumentoWebVinBaseRecursos ON Documento.DocumentoID = DocumentoWebVinBaseRecursos.DocumentoID INNER JOIN BaseRecursosProyecto ON (DocumentoWebVinBaseRecursos.BaseRecursosID = BaseRecursosProyecto.BaseRecursosID AND BaseRecursosProyecto.ProyectoID = Proyecto.ProyectoID)";

            string whereDocComunidadesAgCat = " WHERE Documento.Borrador = 0 AND Documento.UltimaVersion = 1 AND Documento.Eliminado = 0 AND DocumentoWebVinBaseRecursos.Eliminado = 0 AND Documento.FechaModificacion > " + IBD.ToParam("FechaInicial");

            string selectGlobalComunidadesAgCat = selectDocComunidadesAgCat + fromDocComunidadesAgCat + whereDocComunidadesAgCat;

            #endregion

            #region Personas

            string selectDocPersonasAgCat = "SELECT DISTINCT DocumentoWebAgCatTesauro.DocumentoID ResultadoSuscID, IdentCreador.IdentidadID IdPadre, DocumentoWebAgCatTesauro.CategoriaTesauroID CatTesauroID";

            string fromDocPersonasAgCat = "FROM Documento INNER JOIN DocumentoWebAgCatTesauro ON DocumentoWebAgCatTesauro.DocumentoID = Documento.DocumentoID INNER JOIN tmp_CatTesSuscripDoc ON DocumentoWebAgCatTesauro.CategoriaTesauroID = tmp_CatTesSuscripDoc.CategoriaInferiorID INNER JOIN CategoriaTesauro ON CategoriaTesauro.CategoriaTesauroID = DocumentoWebAgCatTesauro.CategoriaTesauroID INNER JOIN SuscripcionTesauroUsuario ON DocumentoWebAgCatTesauro.TesauroID = SuscripcionTesauroUsuario.TesauroID INNER JOIN Persona ON SuscripcionTesauroUsuario.UsuarioID = Persona.UsuarioID INNER JOIN PerfilPersona ON Persona.PersonaID = PerfilPersona.PersonaID INNER JOIN Perfil ON PerfilPersona.PerfilID = Perfil.PerfilID INNER JOIN Identidad IdentCreador ON IdentCreador.PerfilID = Perfil.PerfilID INNER JOIN DocumentoWebVinBaseRecursos ON Documento.DocumentoID = DocumentoWebVinBaseRecursos.DocumentoID INNER JOIN BaseRecursosUsuario ON (DocumentoWebVinBaseRecursos.BaseRecursosID = BaseRecursosUsuario.BaseRecursosID AND BaseRecursosUsuario.UsuarioID = Persona.UsuarioID)";

            string whereDocPersonasAgCat = "WHERE IdentCreador.ProyectoID = '" + ProyectoAD.MetaProyecto.ToString() + "' AND Documento.Borrador = 0 AND Documento.UltimaVersion = 1 AND Documento.Eliminado = 0 AND DocumentoWebVinBaseRecursos.Eliminado = 0 AND Documento.FechaModificacion > " + IBD.ToParam("FechaInicial");

            IConsultaJerarquica consultaJerarquicaPersonasAgCat = IBD.CrearConsultaJerarquicaDeGrafo(selectDocPersonasAgCat, fromDocPersonasAgCat, whereDocPersonasAgCat, "CategoriaSuperiorID", "CategoriaInferiorID", "CatTesauroAgCatTesauro", null, null, "tmp_CatTesSuscripDoc", "CategoriaTesauroID in (" + selectCategoriaPublica + ")", false, "CategoriaTesauro", "CategoriaTesauroID");

            #endregion

            #region Blog
            
            string selectEntradasBlogsAgCat = "SELECT DISTINCT EntradaBlogAgCatTesauro.EntradaBlogID ResultadoSuscID,SuscripcionBlog.BlogID IdPadre, EntradaBlogAgCatTesauro.CategoriaTesauroID CatTesauroID from EntradaBlog ent INNER JOIN SuscripcionBlog ON SuscripcionBlog.BlogID = ent.BlogID INNER JOIN EntradaBlogAgCatTesauro ON EntradaBlogAgCatTesauro.EntradaBlogID = ent.EntradaBlogID INNER JOIN Suscripcion ON Suscripcion.SuscripcionID = SuscripcionBlog.SuscripcionID INNER JOIN Identidad ON Identidad.IdentidadID = Suscripcion.IdentidadID" + whereFiltroIdentidad + " AND ent.FechaModificacion > " + IBD.ToParam("FechaInicial") + " AND ent.Borrador = 0 AND ent.Eliminado = 0";                    
            #endregion

            if (IBD is BDSqlServer)
            {
                consultaJerarquicaPersonasAgCat = IBD.CrearConsultaJerarquicaDeGrafo(selectDocPersonasAgCat, fromDocPersonasAgCat, whereDocPersonasAgCat + " UNION ALL " + selectEntradasBlogsAgCat + " UNION ALL " + selectGlobalComunidadesAgCat, "CategoriaSuperiorID", "CategoriaInferiorID", "CatTesauroAgCatTesauro", null, null, "tmp_CatTesSuscripDoc", "CategoriaTesauroID IN (" + selectCategoriaPublica + ")", false, "CategoriaTesauro", "CategoriaTesauroID");
                parteWithGlobalAgCat = ((ConsultaJerarquicaSqlServer)consultaJerarquicaPersonasAgCat).ParteWith;
                parteSelectGlobalAgCat = ((ConsultaJerarquicaSqlServer)consultaJerarquicaPersonasAgCat).ParteSelect;
                selectGlobalAgCat = parteWithGlobalAgCat + parteSelectGlobalAgCat;
            }
            else if (IBD is BDOracle)
            {

            }

            DbCommand comandoResultadosAgCat = mBaseDatos.GetSqlStringCommand(selectGlobalAgCat);
            mBaseDatos.AddInParameter(comandoResultadosAgCat, IBD.GuidParamValor("PerfilID"), DbType.Guid, IBD.ValorDeGuid(pPerfilID));
            mBaseDatos.AddInParameter(comandoResultadosAgCat, IBD.ToParam("FechaInicial"), DbType.DateTime, pFechaInicical);


            if (IBD is BDSqlServer)
            {
                mBaseDatos.LoadDataSet(comandoResultadosAgCat, resDS, "ResultadoSuscAgCatTesauro");
            }
            else if (IBD is BDOracle)
            {

            }

            #endregion

            #region CategoriaTesauro
            //ID,nombre

            string selectGlobalCat = "";
            string parteSelectGlobalCat = "";
            string parteWithGlobalCat = "";

            #region Proyecto
            
            string selectDocComunidadesCat = "SELECT DISTINCT CategoriaTesauro.CategoriaTesauroID ID, CategoriaTesauro.Nombre nombre";

            string fromDocComunidadesCat = " FROM Documento INNER JOIN DocumentoWebAgCatTesauro ON DocumentoWebAgCatTesauro.DocumentoID = Documento.DocumentoID INNER JOIN CategoriaTesauro ON DocumentoWebAgCatTesauro.CategoriaTesauroID = CategoriaTesauro.CategoriaTesauroID INNER JOIN (" + selectCategoriasSuscripcion + ")tmp_CatTesSuscrip ON DocumentoWebAgCatTesauro.CategoriaTesauroID = tmp_CatTesSuscrip.CategoriaTesauroID INNER JOIN DocumentoWebVinBaseRecursos ON Documento.DocumentoID = DocumentoWebVinBaseRecursos.DocumentoID INNER JOIN BaseRecursosProyecto ON (DocumentoWebVinBaseRecursos.BaseRecursosID = BaseRecursosProyecto.BaseRecursosID)";

            string whereDocComunidadesCat = " WHERE Documento.Borrador = 0 AND Documento.UltimaVersion = 1 AND Documento.Eliminado = 0 AND DocumentoWebVinBaseRecursos.Eliminado = 0 AND Documento.FechaModificacion > " + IBD.ToParam("FechaInicial");

            string selectGlobalComunidadesCat = selectDocComunidadesCat + fromDocComunidadesCat + whereDocComunidadesCat;
            
            #endregion

            #region Personas

            string selectDocPersonasCat = "SELECT DISTINCT CategoriaTesauro.CategoriaTesauroID ID, CategoriaTesauro.Nombre nombre";

            string fromDocPersonasCat = "FROM Documento INNER JOIN DocumentoWebAgCatTesauro ON DocumentoWebAgCatTesauro.DocumentoID = Documento.DocumentoID INNER JOIN CategoriaTesauro ON DocumentoWebAgCatTesauro.CategoriaTesauroID = CategoriaTesauro.CategoriaTesauroID INNER JOIN tmp_CatTesSuscripDoc ON DocumentoWebAgCatTesauro.CategoriaTesauroID = tmp_CatTesSuscripDoc.CategoriaInferiorID INNER JOIN DocumentoWebVinBaseRecursos ON Documento.DocumentoID = DocumentoWebVinBaseRecursos.DocumentoID INNER JOIN BaseRecursosUsuario ON (DocumentoWebVinBaseRecursos.BaseRecursosID = BaseRecursosUsuario.BaseRecursosID)";

            string whereDocPersonasCat = "WHERE Documento.Borrador = 0 AND Documento.UltimaVersion = 1 AND Documento.Eliminado = 0 AND DocumentoWebVinBaseRecursos.Eliminado = 0 AND Documento.FechaModificacion > " + IBD.ToParam("FechaInicial");

            IConsultaJerarquica consultaJerarquicaPersonasCat = IBD.CrearConsultaJerarquicaDeGrafo(selectDocPersonasCat, fromDocPersonasCat, whereDocPersonasCat, "CategoriaSuperiorID", "CategoriaInferiorID", "CatTesauroAgCatTesauro", null, null, "tmp_CatTesSuscripDoc", "CategoriaTesauroID in (" + selectCategoriaPublica + ")", false, "CategoriaTesauro", "CategoriaTesauroID");

            #endregion

            #region Blog
            
            string selectEntradasBlogsCat = "SELECT DISTINCT CategoriaTesauro.CategoriaTesauroID ID, CategoriaTesauro.Nombre from EntradaBlog ent INNER JOIN SuscripcionBlog ON SuscripcionBlog.BlogID = ent.BlogID INNER JOIN EntradaBlogAgCatTesauro ON EntradaBlogAgCatTesauro.EntradaBlogID = ent.EntradaBlogID INNER JOIN CategoriaTesauro ON EntradaBlogAgCatTesauro.CategoriaTesauroID = CategoriaTesauro.CategoriaTesauroID INNER JOIN Suscripcion ON Suscripcion.SuscripcionID = SuscripcionBlog.SuscripcionID INNER JOIN Identidad ON Identidad.IdentidadID = Suscripcion.IdentidadID" + whereFiltroIdentidad + " AND ent.FechaModificacion > " + IBD.ToParam("FechaInicial") + " AND ent.Borrador = 0 AND ent.Eliminado = 0";

            #endregion

            if (IBD is BDSqlServer)
            {
                consultaJerarquicaPersonasCat = IBD.CrearConsultaJerarquicaDeGrafo(selectDocPersonasCat, fromDocPersonasCat, whereDocPersonasCat + " UNION ALL " + selectEntradasBlogsCat + " UNION ALL " + selectGlobalComunidadesCat, "CategoriaSuperiorID", "CategoriaInferiorID", "CatTesauroAgCatTesauro", null, null, "tmp_CatTesSuscripDoc", "CategoriaTesauroID IN (" + selectCategoriaPublica + ")", false, "CategoriaTesauro", "CategoriaTesauroID");
                parteWithGlobalCat = ((ConsultaJerarquicaSqlServer)consultaJerarquicaPersonasCat).ParteWith;
                parteSelectGlobalCat = ((ConsultaJerarquicaSqlServer)consultaJerarquicaPersonasCat).ParteSelect;
                selectGlobalCat = parteWithGlobalCat + parteSelectGlobalCat;
            }
            else if (IBD is BDOracle)
            {

            }

            DbCommand comandoResultadosCat = mBaseDatos.GetSqlStringCommand(selectGlobalCat);
            mBaseDatos.AddInParameter(comandoResultadosCat, IBD.GuidParamValor("PerfilID"), DbType.Guid, IBD.ValorDeGuid(pPerfilID));
            mBaseDatos.AddInParameter(comandoResultadosCat, IBD.ToParam("FechaInicial"), DbType.DateTime, pFechaInicical);


            if (IBD is BDSqlServer)
            {
                mBaseDatos.LoadDataSet(comandoResultadosCat, resDS, "CategoriaTesauro");
            }
            else if (IBD is BDOracle)
            {

            }

            #endregion

            pResultadosDS.Merge(resDS, true);
            return cuantos;
        }

        /// <summary>
        /// Obtiene el numero de resultados de las suscripciones de un perfil
        /// </summary>
        /// <param name="pPerfilID">Id del perfil para el que se obtiene el número de resultados</param>
        /// <param name="pFechaInicical">Fecha a partir de la cual un resultado entra en el cómputo</param>
        /// <param name="pObtenerBloqueadas">true si se desean contar los resultados de suscripciones bloqueadas</param>
        /// <returns></returns>
        public int ObtenerNumeroResultadosSuscripcionesDePerfil(Guid pPerfilID, DateTime pFechaInicical, bool pObtenerBloqueadas)
        {
            int cuantos = 0;

            #region ResultadoSuscripcion

            string selectGlobal = "";
            string parteSelectGlobal = "";
            string parteWithGlobal = "";
            string selectCategoriasSuscripcion = "SELECT CategoriaTesauroID from categoriaTesVinSuscrip INNER JOIN Suscripcion ON Suscripcion.SuscripcionID = categoriaTesVinSuscrip.SuscripcionID INNER JOIN Identidad ON Identidad.IdentidadID = Suscripcion.IdentidadID";

            #region Filtro Identidad;

            string whereFiltroIdentidad = " WHERE Identidad.PerfilID = " + IBD.GuidParamValor("PerfilID");
            if (!pObtenerBloqueadas)
            {
                whereFiltroIdentidad += " AND Suscripcion.Bloqueada = 0";
            }

            selectCategoriasSuscripcion += whereFiltroIdentidad;

            #endregion

            #region Proyecto

            string selectDocComunidades = "SELECT DISTINCT Documento.DocumentoID Id,Proyecto.ProyectoID IdPadre";

            string fromDocComunidades = " FROM Documento INNER JOIN DocumentoWebAgCatTesauro ON DocumentoWebAgCatTesauro.DocumentoID = Documento.DocumentoID INNER JOIN (" + selectCategoriasSuscripcion + ")tmp_CatTesSuscrip ON DocumentoWebAgCatTesauro.CategoriaTesauroID = tmp_CatTesSuscrip.CategoriaTesauroID INNER JOIN CategoriaTesauro ON DocumentoWebAgCatTesauro.CategoriaTesauroID = CategoriaTesauro.CategoriaTesauroID INNER JOIN TesauroProyecto ON CategoriaTesauro.TesauroID = TesauroProyecto.TesauroID INNER JOIN Proyecto ON TesauroProyecto.ProyectoID = Proyecto.ProyectoID INNER JOIN DocumentoWebVinBaseRecursos ON Documento.DocumentoID = DocumentoWebVinBaseRecursos.DocumentoID";

            string whereDocComunidades = " WHERE Documento.Borrador = 0 AND Documento.UltimaVersion = 1 AND Documento.Eliminado = 0 AND DocumentoWebVinBaseRecursos.Eliminado = 0 AND Documento.FechaModificacion > " + IBD.ToParam("FechaInicial");

            string selectGlobalComunidades = selectDocComunidades + fromDocComunidades + whereDocComunidades;

            #endregion

            #region Personas

            //Id,Titulo,Descripcion,TipoResultado,Fecha,NombrePadre,IdPadre,UrlFotoAutor,IdAutor,NombreComunidad,IdComunidad,TipoDocumento, UrlDocumento

            string selectCategoriaPublica = "SELECT DISTINCT CategoriaTesauro.CategoriaTesauroID from CategoriaTesauro INNER JOIN SuscripcionTesauroUsuario ON CategoriaTesauro.TesauroID = SuscripcionTesauroUsuario.TesauroID INNER JOIN Usuario ON Usuario.UsuarioID = SuscripcionTesauroUsuario.UsuarioID INNER JOIN Persona ON Usuario.UsuarioID = Persona.UsuarioID INNER JOIN ConfiguracionGnossPersona ON Persona.PersonaID = ConfiguracionGnossPersona.PersonaID INNER JOIN Suscripcion ON Suscripcion.SuscripcionID = SuscripcionTesauroUsuario.SuscripcionID INNER JOIN TesauroUsuario ON TesauroUsuario.TesauroID = CategoriaTesauro.TesauroID INNER JOIN Identidad ON Suscripcion.IdentidadID = Identidad.identidadID" + whereFiltroIdentidad + " AND CategoriaTesauro.CategoriaTesauroID = TesauroUsuario.CategoriaTesauroPublicoID AND ConfiguracionGnossPersona.VerRecursos = 1";

            string selectDocPersonas = "SELECT DISTINCT Documento.DocumentoID Id, IdentCreador.IdentidadID IdPadre";

            string fromDocPersonas = "FROM Documento INNER JOIN DocumentoWebAgCatTesauro ON DocumentoWebAgCatTesauro.DocumentoID = Documento.DocumentoID INNER JOIN tmp_CatTesSuscripDoc ON DocumentoWebAgCatTesauro.CategoriaTesauroID = tmp_CatTesSuscripDoc.CategoriaInferiorID INNER JOIN CategoriaTesauro ON CategoriaTesauro.CategoriaTesauroID = DocumentoWebAgCatTesauro.CategoriaTesauroID INNER JOIN SuscripcionTesauroUsuario ON DocumentoWebAgCatTesauro.TesauroID = SuscripcionTesauroUsuario.TesauroID INNER JOIN Persona ON SuscripcionTesauroUsuario.UsuarioID = Persona.UsuarioID INNER JOIN PerfilPersona ON Persona.PersonaID = PerfilPersona.PersonaID INNER JOIN Identidad IdentCreador ON IdentCreador.PerfilID = PerfilPersona.PerfilID INNER JOIN Suscripcion ON Suscripcion.SuscripcionID = SuscripcionTesauroUsuario.SuscripcionID INNER JOIN Identidad ON Identidad.IdentidadID = Suscripcion.IdentidadID INNER JOIN DocumentoWebVinBaseRecursos ON Documento.DocumentoID = DocumentoWebVinBaseRecursos.DocumentoID";

            string whereDocPersonas = whereFiltroIdentidad + " AND IdentCreador.ProyectoID = '" + ProyectoAD.MetaProyecto.ToString() + "' AND Documento.Borrador = 0 AND Documento.UltimaVersion = 1 AND Documento.Eliminado = 0 AND DocumentoWebVinBaseRecursos.Eliminado = 0 AND Documento.FechaModificacion > " + IBD.ToParam("FechaInicial");

            IConsultaJerarquica consultaJerarquicaPersonas = IBD.CrearConsultaJerarquicaDeGrafo(selectDocPersonas, fromDocPersonas, whereDocPersonas, "CategoriaSuperiorID", "CategoriaInferiorID", "CatTesauroAgCatTesauro", null, null, "tmp_CatTesSuscripDoc", "CategoriaTesauroID in (" + selectCategoriaPublica + ")", false, "CategoriaTesauro", "CategoriaTesauroID");

            #endregion

            #region Blog

            string selectEntradasBlogs = "select DISTINCT ent.EntradaBlogID Id, Blog.BlogID IdPadre from EntradaBlog ent INNER JOIN Blog ON ent.BlogID = Blog.BlogID INNER JOIN SuscripcionBlog ON SuscripcionBlog.BlogID = ent.BlogID INNER JOIN Suscripcion ON Suscripcion.SuscripcionID = SuscripcionBlog.SuscripcionID INNER JOIN Identidad ON Identidad.IdentidadID = Suscripcion.IdentidadID" + whereFiltroIdentidad + " AND ent.FechaModificacion > " + IBD.ToParam("FechaInicial") + " AND ent.Borrador = 0 AND ent.Eliminado = 0";

            #endregion

            if (IBD is BDSqlServer)
            {
                consultaJerarquicaPersonas = IBD.CrearConsultaJerarquicaDeGrafo(selectDocPersonas, fromDocPersonas, whereDocPersonas + " UNION ALL " + selectEntradasBlogs + " UNION ALL " + selectGlobalComunidades, "CategoriaSuperiorID", "CategoriaInferiorID", "CatTesauroAgCatTesauro", null, null, "tmp_CatTesSuscripDoc", "CategoriaTesauroID IN (" + selectCategoriaPublica + ")", false, "CategoriaTesauro", "CategoriaTesauroID");

                parteWithGlobal = ((ConsultaJerarquicaSqlServer)consultaJerarquicaPersonas).ParteWith;

                parteSelectGlobal = ((ConsultaJerarquicaSqlServer)consultaJerarquicaPersonas).ParteSelect;

                selectGlobal = parteWithGlobal + parteSelectGlobal;
            }
            else if (IBD is BDOracle)
            {

            }

            DbCommand comandoResultados = mBaseDatos.GetSqlStringCommand(selectGlobal);
            mBaseDatos.AddInParameter(comandoResultados, IBD.GuidParamValor("PerfilID"), DbType.Guid, IBD.ValorDeGuid(pPerfilID));
            mBaseDatos.AddInParameter(comandoResultados, IBD.ToParam("FechaInicial"), DbType.DateTime, pFechaInicical);

            if (IBD is BDSqlServer) 
            {
                comandoResultados.CommandText = parteWithGlobal + "Select count(*) from (" + parteSelectGlobal + ") as Contar";
                cuantos = System.Convert.ToInt32(mBaseDatos.ExecuteScalar(comandoResultados));
                 
            }
            else if (IBD is BDOracle)
            {

            }

            #endregion

            return cuantos;
        }

        /// <summary>
        /// Obtiene los resultados de una suscripcion concreta
        /// </summary>
        /// <param name="pResultadosDS">Dataset de resultados</param>
        /// <param name="pSuscripcionID">Suscripcion para la que se obtienen los resultados</param>
        /// <param name="pTipoSuscripcion">Tipo de la suscripcion</param>
        /// <param name="pObtenerBloqueadas">true si se desea traer los resultados de suscripciones bloqueadas</param>
        /// <param name="pFechaInicical">Fecha inicial para los resultados a partir de la que se obtienen</param>
        /// <param name="pInicio">Elemento inicial para paginar</param>
        /// <param name="pLimite">Elemento final para paginar</param>
        /// <param name="pOrderBy">string para ordenar la consulta</param>
        /// <returns></returns>
        public int ObtenerResultadosSuscripcionPorSuscripcion(ResultadosSuscripcionDS pResultadosDS, Guid pSuscripcionID, TipoSuscripciones pTipoSuscripcion, bool pObtenerBloqueadas,DateTime pFechaInicical, int pInicio, int pLimite, string pOrderBy)
        {
            ResultadosSuscripcionDS resDS = new ResultadosSuscripcionDS();
            int cuantos;

            #region ResultadoSuscripcion

            string selectGlobal = "";

            IConsultaJerarquica consultaJerarquicaComunidades = null;
            IConsultaJerarquica consultaJerarquicaPersonas = null;
            string selectEntradasBlogs;
            string selectCategoriasSuscripcion = "";
            string selectCategoriaPublica = "";
            
            switch (pTipoSuscripcion)
            {
                case TipoSuscripciones.Comunidades:
                    selectCategoriasSuscripcion = "SELECT DISTINCT CategoriaTesauroID CategoriaSuperiorID from categoriaTesVinSuscrip WHERE categoriaTesVinSuscrip.suscripcionID = " + IBD.GuidParamValor("SuscripcionID");

                    string selectDocComunidades = "SELECT DISTINCT Documento.DocumentoID Id, Documento.Titulo, Documento.Descripcion," + ((int)TipoResultadoSuscripcion.RecursoComunidad) + " TipoResultado, Documento.FechaModificacion Fecha, NULL IdAutor, Proyecto.Nombre NombrePadre,Proyecto.NombreCorto NombreCortoPadre,Proyecto.ProyectoID IdPadre, Documento.Tipo TipoDocumento, Documento.Enlace UrlDocumento";
                    string fromDocComunidades = "FROM Documento INNER JOIN DocumentoWebAgCatTesauro ON DocumentoWebAgCatTesauro.DocumentoID = Documento.DocumentoID INNER JOIN tmp_CatTesSuscrip ON DocumentoWebAgCatTesauro.CategoriaTesauroID = tmp_CatTesSuscrip.CategoriaInferiorID INNER JOIN CategoriaTesauro ON DocumentoWebAgCatTesauro.CategoriaTesauroID = CategoriaTesauro.CategoriaTesauroID INNER JOIN TesauroProyecto ON CategoriaTesauro.TesauroID = TesauroProyecto.TesauroID INNER JOIN Proyecto ON TesauroProyecto.ProyectoID = Proyecto.ProyectoID INNER JOIN DocumentoWebVinBaseRecursos ON Documento.DocumentoID = DocumentoWebVinBaseRecursos.DocumentoID INNER JOIN BaseRecursosProyecto ON (DocumentoWebVinBaseRecursos.BaseRecursosID = BaseRecursosProyecto.BaseRecursosID AND BaseRecursosProyecto.ProyectoID = Proyecto.ProyectoID)";

                    string whereDocComunidades = " WHERE Documento.Borrador = 0 AND Documento.UltimaVersion = 1 AND Documento.Eliminado = 0 AND DocumentoWebVinBaseRecursos.Eliminado = 0 AND Documento.FechaModificacion > " + IBD.ToParam("FechaInicial");

                    consultaJerarquicaComunidades = IBD.CrearConsultaJerarquicaDeGrafo(selectDocComunidades, fromDocComunidades, whereDocComunidades, "CategoriaSuperiorID", "CategoriaInferiorID", "CatTesauroAgCatTesauro", null, null, "tmp_CatTesSuscrip", "CategoriaTesauroID IN (" + selectCategoriasSuscripcion + ")", false, "CategoriaTesauro", "CategoriaTesauroID");

                    selectGlobal = consultaJerarquicaComunidades.ConsultaJerarquica;

                    break;
                case TipoSuscripciones.Personas:
                    selectCategoriaPublica = "SELECT DISTINCT CategoriaTesauro.CategoriaTesauroID from CategoriaTesauro INNER JOIN SuscripcionTesauroUsuario ON CategoriaTesauro.TesauroID = SuscripcionTesauroUsuario.TesauroID INNER JOIN Usuario ON Usuario.UsuarioID = SuscripcionTesauroUsuario.UsuarioID INNER JOIN Persona ON Usuario.UsuarioID = Persona.UsuarioID INNER JOIN ConfiguracionGnossPersona ON Persona.PersonaID = ConfiguracionGnossPersona.PersonaID  INNER JOIN TesauroUsuario ON TesauroUsuario.TesauroID = CategoriaTesauro.TesauroID WHERE SuscripcionTesauroUsuario.SuscripcionID = " + IBD.GuidParamValor("SuscripcionID") + " AND CategoriaTesauro.CategoriaTesauroID = TesauroUsuario.CategoriaTesauroPublicoID AND ConfiguracionGnossPersona.VerRecursos = 1";
                    string selectDocPersonas = "SELECT DISTINCT Documento.DocumentoID Id, Documento.Titulo, Documento.Descripcion, " + ((int)TipoResultadoSuscripcion.RecursoPersona) + " TipoResultado, Documento.FechaModificacion Fecha, Documento.CreadorID IdAutor, Perfil.NombrePerfil NombrePadre, Perfil.NombreCortoUsu NombreCortoPadre, Documento.DocumentoID IdPadre, Documento.Tipo TipoDocumento,  Documento.Enlace UrlDocumento";

                    string fromDocPersonas = "FROM Documento INNER JOIN DocumentoWebAgCatTesauro ON DocumentoWebAgCatTesauro.DocumentoID = Documento.DocumentoID INNER JOIN tmp_CatTesSuscripDoc ON DocumentoWebAgCatTesauro.CategoriaTesauroID = tmp_CatTesSuscripDoc.CategoriaInferiorID INNER JOIN CategoriaTesauro ON CategoriaTesauro.CategoriaTesauroID = DocumentoWebAgCatTesauro.CategoriaTesauroID INNER JOIN SuscripcionTesauroUsuario ON DocumentoWebAgCatTesauro.TesauroID = SuscripcionTesauroUsuario.TesauroID INNER JOIN Identidad IdentCreador ON Documento.CreadorID = IdentCreador.IdentidadID INNER JOIN Perfil ON Perfil.PerfilID = IdentCreador.PerfilID INNER JOIN Suscripcion ON Suscripcion.SuscripcionID = SuscripcionTesauroUsuario.SuscripcionID INNER JOIN Identidad ON Identidad.IdentidadID = Suscripcion.IdentidadID INNER JOIN DocumentoWebVinBaseRecursos ON Documento.DocumentoID = DocumentoWebVinBaseRecursos.DocumentoID INNER JOIN BaseRecursosUsuario ON (DocumentoWebVinBaseRecursos.BaseRecursosID = BaseRecursosUsuario.BaseRecursosID)";

                    string whereDocPersonas = "WHERE Documento.Borrador = 0 AND Documento.UltimaVersion = 1 AND Documento.Eliminado = 0 AND DocumentoWebVinBaseRecursos.Eliminado = 0 AND Documento.FechaModificacion > " + IBD.ToParam("FechaInicial");

                    consultaJerarquicaPersonas = IBD.CrearConsultaJerarquicaDeGrafo(selectDocPersonas, fromDocPersonas, whereDocPersonas, "CategoriaSuperiorID", "CategoriaInferiorID", "CatTesauroAgCatTesauro", null, null, "tmp_CatTesSuscripDoc", "CategoriaTesauroID in (" + selectCategoriaPublica + ")", false, "CategoriaTesauro", "CategoriaTesauroID");

                    selectGlobal = consultaJerarquicaPersonas.ConsultaJerarquica;

                    break;
                case TipoSuscripciones.Blogs:
                    selectEntradasBlogs = "SELECT ent.EntradaBlogID Id, ent.Titulo, ent.Texto Descripcion, " + ((int)TipoResultadoSuscripcion.EntradaBlogPersona) + " TipoResultado, ent.FechaModificacion Fecha, ent.AutorID IdAutor, Blog.Titulo NombrePadre, Blog.NombreCorto NombreCortoPadre, Blog.BlogID IdPadre, NULL TipoDocumento, NULL UrlDocumento from EntradaBlog ent INNER JOIN Blog ON ent.BlogID = Blog.BlogID INNER JOIN Identidad IdentAutor ON IdentAutor.IdentidadID = ent.AutorID INNER JOIN Perfil ON Perfil.PerfilID = IdentAutor.PerfilID INNER JOIN SuscripcionBlog ON SuscripcionBlog.BlogID = ent.BlogID WHERE SuscripcionBlog.SuscripcionID = " + IBD.GuidParamValor("SuscripcionID") + " AND ent.FechaModificacion > " + IBD.ToParam("FechaInicial") + " AND ent.Borrador = 0 AND ent.Eliminado = 0";

                    selectGlobal = selectEntradasBlogs;

                    break;
            }
            DbCommand comandoResultados = mBaseDatos.GetSqlStringCommand(selectGlobal);
            mBaseDatos.AddInParameter(comandoResultados, IBD.GuidParamValor("SuscripcionID"), DbType.Guid, IBD.ValorDeGuid(pSuscripcionID));
            mBaseDatos.AddInParameter(comandoResultados, IBD.ToParam("FechaInicial"), DbType.DateTime, pFechaInicical);

            if (pTipoSuscripcion == TipoSuscripciones.Comunidades)
            {
                cuantos = IBD.PaginarDataSetConConsultaJerarquica(comandoResultados, consultaJerarquicaComunidades, pOrderBy, resDS, pInicio, pLimite, "ResultadoSuscripcion");
            }
            else if (pTipoSuscripcion == TipoSuscripciones.Personas)
            {
                cuantos = IBD.PaginarDataSetConConsultaJerarquica(comandoResultados, consultaJerarquicaPersonas, pOrderBy, resDS, pInicio, pLimite, "ResultadoSuscripcion");
            }
            else
            {
                cuantos = IBD.PaginarDataSet(comandoResultados, pOrderBy, resDS, pInicio, pLimite, "ResultadoSuscripcion");
            }

            #endregion

            #region ResultadoSuscAgCatTesauro
            //ResultadoSuscID, IdPadre, CatTesauroID

            string selectGlobalAgCat = "";

            IConsultaJerarquica consultaJerarquicaComunidadesAgCat = null;
            IConsultaJerarquica consultaJerarquicaPersonasAgCat = null;
            string selectEntradasBlogsAgCat;

            switch (pTipoSuscripcion)
            {
                case TipoSuscripciones.Comunidades:
                    string selectDocComunidadesAgCat = "SELECT DISTINCT DocumentoWebAgCatTesauro.DocumentoID ResultadoSuscID, Proyecto.ProyectoID IdPadre, DocumentoWebAgCatTesauro.CategoriaTesauroID CatTesauroID";

                    string fromDocComunidadesAgCat = "FROM Documento INNER JOIN DocumentoWebAgCatTesauro ON DocumentoWebAgCatTesauro.DocumentoID = Documento.DocumentoID INNER JOIN tmp_CatTesSuscrip ON DocumentoWebAgCatTesauro.CategoriaTesauroID = tmp_CatTesSuscrip.CategoriaInferiorID INNER JOIN CategoriaTesauro ON DocumentoWebAgCatTesauro.CategoriaTesauroID = CategoriaTesauro.CategoriaTesauroID INNER JOIN TesauroProyecto ON CategoriaTesauro.TesauroID = TesauroProyecto.TesauroID INNER JOIN Proyecto ON TesauroProyecto.ProyectoID = Proyecto.ProyectoID INNER JOIN DocumentoWebVinBaseRecursos ON Documento.DocumentoID = DocumentoWebVinBaseRecursos.DocumentoID INNER JOIN BaseRecursosProyecto ON (DocumentoWebVinBaseRecursos.BaseRecursosID = BaseRecursosProyecto.BaseRecursosID AND BaseRecursosProyecto.ProyectoID = Proyecto.ProyectoID)";

                    string whereDocComunidadesAgCat = " WHERE Documento.Borrador = 0 AND Documento.UltimaVersion = 1 AND Documento.Eliminado = 0 AND DocumentoWebVinBaseRecursos.Eliminado = 0 AND Documento.FechaModificacion > " + IBD.ToParam("FechaInicial");

                    consultaJerarquicaComunidadesAgCat = IBD.CrearConsultaJerarquicaDeGrafo(selectDocComunidadesAgCat, fromDocComunidadesAgCat, whereDocComunidadesAgCat, "CategoriaSuperiorID", "CategoriaInferiorID", "CatTesauroAgCatTesauro", null, null, "tmp_CatTesSuscrip", "CategoriaTesauroID IN (" + selectCategoriasSuscripcion + ")", false, "CategoriaTesauro", "CategoriaTesauroID");

                    selectGlobalAgCat = consultaJerarquicaComunidadesAgCat.ConsultaJerarquica;

                    break;
                case TipoSuscripciones.Personas:
                    string selectDocPersonasAgCat = "SELECT DISTINCT DocumentoWebAgCatTesauro.DocumentoID ResultadoSuscID, IdentCreador.IdentidadID IdPadre, DocumentoWebAgCatTesauro.CategoriaTesauroID CatTesauroID";

                    string fromDocPersonasAgCat = "FROM Documento INNER JOIN DocumentoWebAgCatTesauro ON DocumentoWebAgCatTesauro.DocumentoID = Documento.DocumentoID INNER JOIN tmp_CatTesSuscripDoc ON DocumentoWebAgCatTesauro.CategoriaTesauroID = tmp_CatTesSuscripDoc.CategoriaInferiorID INNER JOIN CategoriaTesauro ON CategoriaTesauro.CategoriaTesauroID = DocumentoWebAgCatTesauro.CategoriaTesauroID INNER JOIN SuscripcionTesauroUsuario ON DocumentoWebAgCatTesauro.TesauroID = SuscripcionTesauroUsuario.TesauroID INNER JOIN Persona ON SuscripcionTesauroUsuario.UsuarioID = Persona.UsuarioID INNER JOIN PerfilPersona ON Persona.PersonaID = PerfilPersona.PersonaID INNER JOIN Perfil ON PerfilPersona.PerfilID = Perfil.PerfilID INNER JOIN Identidad IdentCreador ON IdentCreador.PerfilID = Perfil.PerfilID INNER JOIN DocumentoWebVinBaseRecursos ON Documento.DocumentoID = DocumentoWebVinBaseRecursos.DocumentoID INNER JOIN BaseRecursosUsuario ON (DocumentoWebVinBaseRecursos.BaseRecursosID =  BaseRecursosUsuario.BaseRecursosID AND BaseRecursosUsuario.UsuarioID = Persona.UsuarioID)";

                    string whereDocPersonasAgCat = "WHERE IdentCreador.ProyectoID = '" + ProyectoAD.MetaProyecto.ToString() + "' AND Documento.Borrador = 0 AND Documento.UltimaVersion = 1 AND Documento.Eliminado = 0 AND DocumentoWebVinBaseRecursos.Eliminado = 0 AND Documento.FechaModificacion > " + IBD.ToParam("FechaInicial");

                    consultaJerarquicaPersonasAgCat = IBD.CrearConsultaJerarquicaDeGrafo(selectDocPersonasAgCat, fromDocPersonasAgCat, whereDocPersonasAgCat, "CategoriaSuperiorID", "CategoriaInferiorID", "CatTesauroAgCatTesauro", null, null, "tmp_CatTesSuscripDoc", "CategoriaTesauroID in (" + selectCategoriaPublica + ")", false, "CategoriaTesauro", "CategoriaTesauroID");

                    selectGlobalAgCat = consultaJerarquicaPersonasAgCat.ConsultaJerarquica;

                    break;
                case TipoSuscripciones.Blogs:
                    selectEntradasBlogsAgCat = "SELECT DISTINCT EntradaBlogAgCatTesauro.EntradaBlogID ResultadoSuscID, SuscripcionBlog.BlogID IdPadre, EntradaBlogAgCatTesauro.CategoriaTesauroID CatTesauroID from EntradaBlog ent INNER JOIN SuscripcionBlog ON SuscripcionBlog.BlogID = ent.BlogID INNER JOIN EntradaBlogAgCatTesauro ON EntradaBlogAgCatTesauro.EntradaBlogID = ent.EntradaBlogID WHERE SuscripcionBlog.SuscripcionID = " + IBD.GuidParamValor("SuscripcionID") + " AND ent.FechaModificacion > " + IBD.ToParam("FechaInicial") + " AND ent.Borrador = 0 AND ent.Eliminado = 0";

                    selectGlobalAgCat = selectEntradasBlogsAgCat;

                    break;
            }

            DbCommand comandoResultadosAgCat = mBaseDatos.GetSqlStringCommand(selectGlobalAgCat);
            mBaseDatos.AddInParameter(comandoResultadosAgCat, IBD.GuidParamValor("SuscripcionID"), DbType.Guid, IBD.ValorDeGuid(pSuscripcionID));
            mBaseDatos.AddInParameter(comandoResultadosAgCat, IBD.ToParam("FechaInicial"), DbType.DateTime, pFechaInicical);

            if (pTipoSuscripcion == TipoSuscripciones.Comunidades)
            {
                cuantos = IBD.PaginarDataSetConConsultaJerarquica(comandoResultadosAgCat, consultaJerarquicaComunidadesAgCat, "ORDER BY ResultadoSuscID", resDS, pInicio, pLimite, "ResultadoSuscAgCatTesauro");
            }
            else if (pTipoSuscripcion == TipoSuscripciones.Personas)
            {
                cuantos = IBD.PaginarDataSetConConsultaJerarquica(comandoResultadosAgCat, consultaJerarquicaPersonasAgCat, "ORDER BY ResultadoSuscID", resDS, pInicio, pLimite, "ResultadoSuscAgCatTesauro");
            }
            else
            {
                cuantos = IBD.PaginarDataSet(comandoResultadosAgCat, "ORDER BY ResultadoSuscID", resDS, pInicio, pLimite, "ResultadoSuscAgCatTesauro");
            }

            #endregion

            #region CategoriaTesauro

            //ID,nombre

            string selectGlobalCat = "";

            IConsultaJerarquica consultaJerarquicaComunidadesCat = null;
            IConsultaJerarquica consultaJerarquicaPersonasCat = null;
            string selectEntradasBlogsCat;

            switch (pTipoSuscripcion)
            {
                case TipoSuscripciones.Comunidades:
                    string selectDocComunidadesCat = "SELECT DISTINCT CategoriaTesauro.CategoriaTesauroID ID, CategoriaTesauro.Nombre nombre";

                    string fromDocComunidadesCat = "FROM Documento INNER JOIN DocumentoWebAgCatTesauro ON DocumentoWebAgCatTesauro.DocumentoID = Documento.DocumentoID INNER JOIN CategoriaTesauro ON DocumentoWebAgCatTesauro.CategoriaTesauroID = CategoriaTesauro.CategoriaTesauroID INNER JOIN tmp_CatTesSuscrip ON DocumentoWebAgCatTesauro.CategoriaTesauroID = tmp_CatTesSuscrip.CategoriaInferiorID INNER JOIN DocumentoWebVinBaseRecursos ON Documento.DocumentoID = DocumentoWebVinBaseRecursos.DocumentoID INNER JOIN BaseRecursosProyecto ON DocumentoWebVinBaseRecursos.BaseRecursosID = BaseRecursosProyecto.BaseRecursosID ";

                    string whereDocComunidadesCat = " WHERE Documento.Borrador = 0 AND Documento.UltimaVersion = 1 AND Documento.Eliminado = 0 AND DocumentoWebVinBaseRecursos.Eliminado = 0 AND Documento.FechaModificacion > " + IBD.ToParam("FechaInicial");

                    consultaJerarquicaComunidadesCat = IBD.CrearConsultaJerarquicaDeGrafo(selectDocComunidadesCat, fromDocComunidadesCat, whereDocComunidadesCat, "CategoriaSuperiorID", "CategoriaInferiorID", "CatTesauroAgCatTesauro", null, null, "tmp_CatTesSuscrip", "CategoriaTesauroID IN (" + selectCategoriasSuscripcion + ")", false, "CategoriaTesauro", "CategoriaTesauroID");

                    selectGlobalCat = consultaJerarquicaComunidadesCat.ConsultaJerarquica;

                    break;
                case TipoSuscripciones.Personas:
                    string selectDocPersonasCat = "SELECT DISTINCT CategoriaTesauro.CategoriaTesauroID ID, CategoriaTesauro.Nombre nombre";

                    string fromDocPersonasCat = "FROM Documento INNER JOIN DocumentoWebVinBaseRecursos ON Documento.DocumentoID = DocumentoWebVinBaseRecursos.DocumentoID INNER JOIN BaseRecursosUsuario ON DocumentoWebVinBaseRecursos.BaseRecursosID = BaseRecursosUsuario.BaseRecursosID INNER JOIN DocumentoWebAgCatTesauro ON DocumentoWebAgCatTesauro.DocumentoID = Documento.DocumentoID INNER JOIN CategoriaTesauro ON DocumentoWebAgCatTesauro.CategoriaTesauroID = CategoriaTesauro.CategoriaTesauroID INNER JOIN tmp_CatTesSuscripDoc ON DocumentoWebAgCatTesauro.CategoriaTesauroID = tmp_CatTesSuscripDoc.CategoriaInferiorID";

                    string whereDocPersonasCat = "WHERE Documento.Borrador = 0 AND Documento.UltimaVersion = 1 AND Documento.Eliminado = 0 AND DocumentoWebVinBaseRecursos.Eliminado = 0 AND Documento.FechaModificacion > " + IBD.ToParam("FechaInicial");

                    consultaJerarquicaPersonasCat = IBD.CrearConsultaJerarquicaDeGrafo(selectDocPersonasCat, fromDocPersonasCat, whereDocPersonasCat, "CategoriaSuperiorID", "CategoriaInferiorID", "CatTesauroAgCatTesauro", null, null, "tmp_CatTesSuscripDoc", "CategoriaTesauroID in (" + selectCategoriaPublica + ")", false, "CategoriaTesauro", "CategoriaTesauroID");

                    selectGlobalCat = consultaJerarquicaPersonasCat.ConsultaJerarquica;

                    break;
                case TipoSuscripciones.Blogs:
                    selectEntradasBlogsCat = "SELECT DISTINCT CategoriaTesauro.CategoriaTesauroID ID, CategoriaTesauro.Nombre nombre from EntradaBlog ent INNER JOIN SuscripcionBlog ON SuscripcionBlog.BlogID = ent.BlogID INNER JOIN EntradaBlogAgCatTesauro ON EntradaBlogAgCatTesauro.EntradaBlogID = ent.EntradaBlogID INNER JOIN CategoriaTesauro ON EntradaBlogAgCatTesauro.CategoriaTesauroID = CategoriaTesauro.CategoriaTesauroID WHERE SuscripcionBlog.SuscripcionID = " + IBD.GuidParamValor("SuscripcionID") + " AND ent.FechaModificacion > " + IBD.ToParam("FechaInicial") + " AND ent.Borrador = 0 AND ent.Eliminado = 0";

                    selectGlobalCat = selectEntradasBlogsCat;

                    break;
            }
            DbCommand comandoResultadosCat = mBaseDatos.GetSqlStringCommand(selectGlobalCat);
            mBaseDatos.AddInParameter(comandoResultadosCat, IBD.GuidParamValor("SuscripcionID"), DbType.Guid, IBD.ValorDeGuid(pSuscripcionID));
            mBaseDatos.AddInParameter(comandoResultadosCat, IBD.ToParam("FechaInicial"), DbType.DateTime, pFechaInicical);

            if (pTipoSuscripcion == TipoSuscripciones.Comunidades)
            {
                cuantos = IBD.PaginarDataSetConConsultaJerarquica(comandoResultadosCat, consultaJerarquicaComunidadesCat, "ORDER BY ID" , resDS, pInicio, pLimite, "CategoriaTesauro");
            }
            else if (pTipoSuscripcion == TipoSuscripciones.Personas)
            {
                cuantos = IBD.PaginarDataSetConConsultaJerarquica(comandoResultadosCat, consultaJerarquicaPersonasCat, "ORDER BY ID", resDS, pInicio, pLimite, "CategoriaTesauro");
            }
            else
            {
                cuantos = IBD.PaginarDataSet(comandoResultadosCat, "ORDER BY ID", resDS, pInicio, pLimite, "CategoriaTesauro");
            }

            #endregion

            pResultadosDS.Merge(resDS);
            return cuantos;
        }

        /// <summary>
        /// Obtiene los resultados de las suscripciones de un perfil, de un tipo concreto
        /// </summary>
        /// <param name="pResultadosDS">Dataset de resultados</param>
        /// <param name="pPerfilID">Id del perfil para el que se obtienen los resultados</param>
        /// <param name="pTipoSuscripcion">Tipo de la suscripcion</param>
        /// <param name="pObtenerBloqueadas">true si se desea traer los resultados de suscripciones bloqueadas</param>
        /// <param name="pFechaInicical">Fecha inicial para los resultados a partir de la que se obtienen</param>
        /// <param name="pOrderBy">string para ordenar la consulta</param>
        /// <returns></returns>
        public void ObtenerResultadosSuscripcionDePerfilPorTipoSuscripcion(ResultadosSuscripcionDS pResultadosDS, Guid pPerfilID, TipoSuscripciones pTipoSuscripcion, bool pObtenerBloqueadas, DateTime pFechaInicical, string pOrderBy)
        {
            ResultadosSuscripcionDS resDS = new ResultadosSuscripcionDS();

            #region ResultadoSuscripcion
            string selectGlobal = "";
            string selectCategoriasSuscripcion = "SELECT CategoriaTesauroID from categoriaTesVinSuscrip INNER JOIN Suscripcion ON Suscripcion.SuscripcionID = categoriaTesVinSuscrip.SuscripcionID INNER JOIN Identidad ON Identidad.IdentidadID = Suscripcion.IdentidadID";

            #region Filtro Identidad;

            string whereFiltroIdentidad = " WHERE Identidad.PerfilID = " + IBD.GuidParamValor("PerfilID");
            if (!pObtenerBloqueadas)
            {
                whereFiltroIdentidad += " AND Suscripcion.Bloqueada = 0";
            }

            selectCategoriasSuscripcion += whereFiltroIdentidad;

            #endregion

            string selectCategoriaPublica = "SELECT DISTINCT CategoriaTesauro.CategoriaTesauroID from CategoriaTesauro INNER JOIN SuscripcionTesauroUsuario ON CategoriaTesauro.TesauroID = SuscripcionTesauroUsuario.TesauroID INNER JOIN Suscripcion ON Suscripcion.SuscripcionID = SuscripcionTesauroUsuario.SuscripcionID INNER JOIN TesauroUsuario ON TesauroUsuario.TesauroID = CategoriaTesauro.TesauroID INNER JOIN Identidad ON Suscripcion.IdentidadID = Identidad.identidadID" + whereFiltroIdentidad + " AND CategoriaTesauro.CategoriaTesauroID = TesauroUsuario.CategoriaTesauroPublicoID";
            switch (pTipoSuscripcion)
            {
                case TipoSuscripciones.Comunidades:
                    #region proyecto
                    string selectDocComunidades = "SELECT DISTINCT Documento.DocumentoID Id, Documento.Titulo, Documento.Descripcion," + ((int)TipoResultadoSuscripcion.RecursoComunidad) + " TipoResultado, Documento.FechaModificacion Fecha, NULL IdAutor, Proyecto.Nombre NombrePadre,Proyecto.NombreCorto NombreCortoPadre,Proyecto.ProyectoID IdPadre, Documento.Tipo TipoDocumento, Documento.Enlace UrlDocumento";

                    string fromDocComunidades = " FROM Documento INNER JOIN DocumentoWebAgCatTesauro ON DocumentoWebAgCatTesauro.DocumentoID = Documento.DocumentoID INNER JOIN (" + selectCategoriasSuscripcion + ")tmp_CatTesSuscrip ON DocumentoWebAgCatTesauro.CategoriaTesauroID = tmp_CatTesSuscrip.CategoriaTesauroID INNER JOIN CategoriaTesauro ON DocumentoWebAgCatTesauro.CategoriaTesauroID = CategoriaTesauro.CategoriaTesauroID INNER JOIN TesauroProyecto ON CategoriaTesauro.TesauroID = TesauroProyecto.TesauroID INNER JOIN Proyecto ON TesauroProyecto.ProyectoID = Proyecto.ProyectoID INNER JOIN DocumentoWebVinBaseRecursos ON Documento.DocumentoID = DocumentoWebVinBaseRecursos.DocumentoID INNER JOIN BaseRecursosProyecto ON (DocumentoWebVinBaseRecursos.BaseRecursosID = BaseRecursosProyecto.BaseRecursosID AND BaseRecursosProyecto.ProyectoID = Proyecto.ProyectoID)";

                    string whereDocComunidades = " WHERE Documento.Borrador = 0 AND Documento.UltimaVersion = 1 AND Documento.Eliminado = 0 AND DocumentoWebVinBaseRecursos.Eliminado = 0 AND Documento.FechaModificacion > " + IBD.ToParam("FechaInicial");

                    selectGlobal = selectDocComunidades + fromDocComunidades + whereDocComunidades;
                    break;
                    #endregion
                case TipoSuscripciones.Personas:
                    #region Personas

                    string selectDocPersonas = "SELECT DISTINCT Documento.DocumentoID Id, Documento.Titulo, Documento.Descripcion, " + ((int)TipoResultadoSuscripcion.RecursoPersona) + " TipoResultado, Documento.FechaModificacion Fecha, IdentCreador.IdentidadID IdAutor, Perfil.NombrePerfil NombrePadre, Perfil.NombreCortoUsu NombreCortoPadre, IdentCreador.IdentidadID IdPadre, Documento.Tipo TipoDocumento,  Documento.Enlace UrlDocumento";

                    string fromDocPersonas = "FROM Documento INNER JOIN DocumentoWebAgCatTesauro ON DocumentoWebAgCatTesauro.DocumentoID = Documento.DocumentoID INNER JOIN tmp_CatTesSuscripDoc ON DocumentoWebAgCatTesauro.CategoriaTesauroID = tmp_CatTesSuscripDoc.CategoriaInferiorID INNER JOIN CategoriaTesauro ON CategoriaTesauro.CategoriaTesauroID = DocumentoWebAgCatTesauro.CategoriaTesauroID INNER JOIN SuscripcionTesauroUsuario ON DocumentoWebAgCatTesauro.TesauroID = SuscripcionTesauroUsuario.TesauroID INNER JOIN Persona ON SuscripcionTesauroUsuario.UsuarioID = Persona.UsuarioID INNER JOIN PerfilPersona ON Persona.PersonaID = PerfilPersona.PersonaID INNER JOIN Perfil ON PerfilPersona.PerfilID = Perfil.PerfilID INNER JOIN Identidad IdentCreador ON IdentCreador.PerfilID = Perfil.PerfilID INNER JOIN Suscripcion ON Suscripcion.SuscripcionID = SuscripcionTesauroUsuario.SuscripcionID INNER JOIN Identidad ON Identidad.IdentidadID = Suscripcion.IdentidadID INNER JOIN DocumentoWebVinBaseRecursos ON Documento.DocumentoID = DocumentoWebVinBaseRecursos.DocumentoID INNER JOIN BaseRecursosUsuario ON (DocumentoWebVinBaseRecursos.BaseRecursosID = BaseRecursosUsuario.BaseRecursosID AND BaseRecursosUsuario.UsuarioID = Persona.UsuarioID)";

                    string whereDocPersonas = whereFiltroIdentidad + " AND IdentCreador.ProyectoID = '" + ProyectoAD.MetaProyecto.ToString() + "' AND Documento.Borrador = 0 AND Documento.UltimaVersion = 1 AND Documento.Eliminado = 0 AND DocumentoWebVinBaseRecursos.Eliminado = 0 AND Documento.FechaModificacion > " + IBD.ToParam("FechaInicial");

                    IConsultaJerarquica consultaJerarquicaPersonas = IBD.CrearConsultaJerarquicaDeGrafo(selectDocPersonas, fromDocPersonas, whereDocPersonas, "CategoriaSuperiorID", "CategoriaInferiorID", "CatTesauroAgCatTesauro", null, null, "tmp_CatTesSuscripDoc", "CategoriaTesauroID in (" + selectCategoriaPublica + ")", false, "CategoriaTesauro", "CategoriaTesauroID");
                    selectGlobal = consultaJerarquicaPersonas.ConsultaJerarquica;
                    break;
                    #endregion
                case TipoSuscripciones.Blogs:
                    #region Blogs
                    string selectEntradasBlogs = "select DISTINCT ent.EntradaBlogID Id, ent.Titulo, ent.Texto Descripcion, " + ((int)TipoResultadoSuscripcion.EntradaBlogPersona) + " TipoResultado, ent.FechaModificacion Fecha, ent.AutorID IdAutor, Blog.Titulo NombrePadre, Blog.NombreCorto NombreCortoPadre, Blog.BlogID IdPadre, NULL TipoDocumento, NULL UrlDocumento from EntradaBlog ent INNER JOIN Blog ON ent.BlogID = Blog.BlogID INNER JOIN Identidad IdentAutor ON IdentAutor.IdentidadID = ent.AutorID INNER JOIN Perfil ON Perfil.PerfilID = IdentAutor.PerfilID INNER JOIN SuscripcionBlog ON SuscripcionBlog.BlogID = ent.BlogID INNER JOIN Suscripcion ON Suscripcion.SuscripcionID = SuscripcionBlog.SuscripcionID INNER JOIN Identidad ON Identidad.IdentidadID = Suscripcion.IdentidadID" + whereFiltroIdentidad + " AND ent.FechaModificacion > " + IBD.ToParam("FechaInicial") + " AND ent.Borrador = 0 AND ent.Eliminado = 0";

                    selectGlobal = selectEntradasBlogs;
                    break;
                    #endregion
            }

            DbCommand comandoResultados = mBaseDatos.GetSqlStringCommand(selectGlobal + " " + pOrderBy);
            mBaseDatos.AddInParameter(comandoResultados, IBD.GuidParamValor("PerfilID"), DbType.Guid, IBD.ValorDeGuid(pPerfilID));
            mBaseDatos.AddInParameter(comandoResultados, IBD.ToParam("FechaInicial"), DbType.DateTime, pFechaInicical);

            mBaseDatos.LoadDataSet(comandoResultados, resDS, "ResultadoSuscripcion");

            #endregion

            #region ResultadoSuscAgCatTesauro
            //ResultadoSuscID,CatTesauroID

            string selectGlobalAgCat = "";

            switch (pTipoSuscripcion)
            {
                case TipoSuscripciones.Comunidades:
                    #region Proyecto

                    string selectDocComunidadesAgCat = "SELECT DISTINCT DocumentoWebAgCatTesauro.DocumentoID ResultadoSuscID, Proyecto.ProyectoID IdPadre, DocumentoWebAgCatTesauro.CategoriaTesauroID CatTesauroID";

                    string fromDocComunidadesAgCat = " FROM Documento INNER JOIN DocumentoWebAgCatTesauro ON DocumentoWebAgCatTesauro.DocumentoID = Documento.DocumentoID INNER JOIN (" + selectCategoriasSuscripcion + ")tmp_CatTesSuscrip ON DocumentoWebAgCatTesauro.CategoriaTesauroID = tmp_CatTesSuscrip.CategoriaTesauroID INNER JOIN CategoriaTesauro ON DocumentoWebAgCatTesauro.CategoriaTesauroID = CategoriaTesauro.CategoriaTesauroID INNER JOIN TesauroProyecto ON CategoriaTesauro.TesauroID = TesauroProyecto.TesauroID INNER JOIN Proyecto ON TesauroProyecto.ProyectoID = Proyecto.ProyectoID INNER JOIN DocumentoWebVinBaseRecursos ON Documento.DocumentoID = DocumentoWebVinBaseRecursos.DocumentoID INNER JOIN BaseRecursosProyecto ON (DocumentoWebVinBaseRecursos.BaseRecursosID = BaseRecursosProyecto.BaseRecursosID AND BaseRecursosProyecto.ProyectoID = Proyecto.ProyectoID)";

                    string whereDocComunidadesAgCat = " WHERE Documento.Borrador = 0 AND Documento.UltimaVersion = 1 AND Documento.Eliminado = 0 AND DocumentoWebVinBaseRecursos.Eliminado = 0 AND Documento.FechaModificacion > " + IBD.ToParam("FechaInicial");

                    selectGlobalAgCat = selectDocComunidadesAgCat + fromDocComunidadesAgCat + whereDocComunidadesAgCat;
                    
                    #endregion
                    break;
                case TipoSuscripciones.Personas:
                    #region Personas

                    string selectDocPersonasAgCat = "SELECT DISTINCT DocumentoWebAgCatTesauro.DocumentoID ResultadoSuscID, IdentCreador.IdentidadID IdPadre, DocumentoWebAgCatTesauro.CategoriaTesauroID CatTesauroID";

                    string fromDocPersonasAgCat = "FROM Documento INNER JOIN DocumentoWebAgCatTesauro ON DocumentoWebAgCatTesauro.DocumentoID = Documento.DocumentoID INNER JOIN tmp_CatTesSuscripDoc ON DocumentoWebAgCatTesauro.CategoriaTesauroID = tmp_CatTesSuscripDoc.CategoriaInferiorID INNER JOIN CategoriaTesauro ON CategoriaTesauro.CategoriaTesauroID = DocumentoWebAgCatTesauro.CategoriaTesauroID INNER JOIN SuscripcionTesauroUsuario ON DocumentoWebAgCatTesauro.TesauroID = SuscripcionTesauroUsuario.TesauroID INNER JOIN Persona ON SuscripcionTesauroUsuario.UsuarioID = Persona.UsuarioID INNER JOIN PerfilPersona ON Persona.PersonaID = PerfilPersona.PersonaID INNER JOIN Perfil ON PerfilPersona.PerfilID = Perfil.PerfilID INNER JOIN Identidad IdentCreador ON IdentCreador.PerfilID = Perfil.PerfilID INNER JOIN DocumentoWebVinBaseRecursos ON Documento.DocumentoID = DocumentoWebVinBaseRecursos.DocumentoID INNER JOIN BaseRecursosUsuario ON (DocumentoWebVinBaseRecursos.BaseRecursosID = BaseRecursosUsuario.BaseRecursosID AND BaseRecursosUsuario.UsuarioID = Persona.UsuarioID)";

                    string whereDocPersonasAgCat = "WHERE IdentCreador.ProyectoID = '" + ProyectoAD.MetaProyecto.ToString() + "' AND Documento.Borrador = 0 AND Documento.UltimaVersion = 1 AND Documento.Eliminado = 0 AND DocumentoWebVinBaseRecursos.Eliminado = 0 AND Documento.FechaModificacion > " + IBD.ToParam("FechaInicial");

                    IConsultaJerarquica consultaJerarquicaPersonasAgCat = IBD.CrearConsultaJerarquicaDeGrafo(selectDocPersonasAgCat, fromDocPersonasAgCat, whereDocPersonasAgCat, "CategoriaSuperiorID", "CategoriaInferiorID", "CatTesauroAgCatTesauro", null, null, "tmp_CatTesSuscripDoc", "CategoriaTesauroID in (" + selectCategoriaPublica + ")", false, "CategoriaTesauro", "CategoriaTesauroID");

                    selectGlobalAgCat = consultaJerarquicaPersonasAgCat.ConsultaJerarquica;

                    #endregion
                    break;
                case TipoSuscripciones.Blogs:
                    #region Blog

                    string selectEntradasBlogsAgCat = "SELECT DISTINCT EntradaBlogAgCatTesauro.EntradaBlogID ResultadoSuscID,SuscripcionBlog.BlogID IdPadre, EntradaBlogAgCatTesauro.CategoriaTesauroID CatTesauroID from EntradaBlog ent INNER JOIN SuscripcionBlog ON SuscripcionBlog.BlogID = ent.BlogID INNER JOIN EntradaBlogAgCatTesauro ON EntradaBlogAgCatTesauro.EntradaBlogID = ent.EntradaBlogID INNER JOIN Suscripcion ON Suscripcion.SuscripcionID = SuscripcionBlog.SuscripcionID INNER JOIN Identidad ON Identidad.IdentidadID = Suscripcion.IdentidadID" + whereFiltroIdentidad + " AND ent.FechaModificacion > " + IBD.ToParam("FechaInicial") + " AND ent.Borrador = 0 AND ent.Eliminado = 0";

                    selectGlobalAgCat = selectEntradasBlogsAgCat;

                    #endregion
                    break;
            }

            DbCommand comandoResultadosAgCat = mBaseDatos.GetSqlStringCommand(selectGlobalAgCat);
            mBaseDatos.AddInParameter(comandoResultadosAgCat, IBD.GuidParamValor("PerfilID"), DbType.Guid, IBD.ValorDeGuid(pPerfilID));
            mBaseDatos.AddInParameter(comandoResultadosAgCat, IBD.ToParam("FechaInicial"), DbType.DateTime, pFechaInicical);

            mBaseDatos.LoadDataSet(comandoResultadosAgCat, resDS, "ResultadoSuscAgCatTesauro");

            #endregion

            #region CategoriaTesauro
            //ID,nombre

            string selectGlobalCat = "";

            switch (pTipoSuscripcion)
            {
                case TipoSuscripciones.Comunidades:
                    #region Proyecto

                    string selectDocComunidadesCat = "SELECT DISTINCT CategoriaTesauro.CategoriaTesauroID ID, CategoriaTesauro.Nombre nombre";

                    string fromDocComunidadesCat = " FROM Documento INNER JOIN DocumentoWebAgCatTesauro ON DocumentoWebAgCatTesauro.DocumentoID = Documento.DocumentoID INNER JOIN CategoriaTesauro ON DocumentoWebAgCatTesauro.CategoriaTesauroID = CategoriaTesauro.CategoriaTesauroID INNER JOIN (" + selectCategoriasSuscripcion + ") tmp_CatTesSuscrip ON DocumentoWebAgCatTesauro.CategoriaTesauroID = tmp_CatTesSuscrip.CategoriaTesauroID INNER JOIN DocumentoWebVinBaseRecursos ON Documento.DocumentoID = DocumentoWebVinBaseRecursos.DocumentoID INNER JOIN BaseRecursosProyecto ON (DocumentoWebVinBaseRecursos.BaseRecursosID = BaseRecursosProyecto.BaseRecursosID)";

                    string whereDocComunidadesCat = " WHERE Documento.Borrador = 0 AND Documento.UltimaVersion = 1 AND Documento.Eliminado = 0 AND DocumentoWebVinBaseRecursos.Eliminado = 0 AND Documento.FechaModificacion > " + IBD.ToParam("FechaInicial");

                    selectGlobalCat = selectDocComunidadesCat + fromDocComunidadesCat + whereDocComunidadesCat;

                    #endregion
                    break;
                case TipoSuscripciones.Personas:
                    #region Personas

                    string selectDocPersonasCat = "SELECT DISTINCT CategoriaTesauro.CategoriaTesauroID ID, CategoriaTesauro.Nombre nombre";

                    string fromDocPersonasCat = "FROM Documento INNER JOIN DocumentoWebAgCatTesauro ON DocumentoWebAgCatTesauro.DocumentoID = Documento.DocumentoID INNER JOIN CategoriaTesauro ON DocumentoWebAgCatTesauro.CategoriaTesauroID = CategoriaTesauro.CategoriaTesauroID INNER JOIN tmp_CatTesSuscripDoc ON DocumentoWebAgCatTesauro.CategoriaTesauroID = tmp_CatTesSuscripDoc.CategoriaInferiorID INNER JOIN DocumentoWebVinBaseRecursos ON Documento.DocumentoID = DocumentoWebVinBaseRecursos.DocumentoID INNER JOIN BaseRecursosUsuario ON (DocumentoWebVinBaseRecursos.BaseRecursosID = BaseRecursosUsuario.BaseRecursosID)";

                    string whereDocPersonasCat = "WHERE Documento.Borrador = 0 AND Documento.UltimaVersion = 1 AND Documento.Eliminado = 0 AND DocumentoWebVinBaseRecursos.Eliminado = 0 AND Documento.FechaModificacion > " + IBD.ToParam("FechaInicial");

                    IConsultaJerarquica consultaJerarquicaPersonasCat = IBD.CrearConsultaJerarquicaDeGrafo(selectDocPersonasCat, fromDocPersonasCat, whereDocPersonasCat, "CategoriaSuperiorID", "CategoriaInferiorID", "CatTesauroAgCatTesauro", null, null, "tmp_CatTesSuscripDoc", "CategoriaTesauroID in (" + selectCategoriaPublica + ")", false, "CategoriaTesauro", "CategoriaTesauroID");

                    selectGlobalCat = consultaJerarquicaPersonasCat.ConsultaJerarquica;

                    #endregion
                    break;
                case TipoSuscripciones.Blogs:
                    #region Blog

                    string selectEntradasBlogsCat = "SELECT DISTINCT CategoriaTesauro.CategoriaTesauroID ID, CategoriaTesauro.Nombre from EntradaBlog ent INNER JOIN SuscripcionBlog ON SuscripcionBlog.BlogID = ent.BlogID INNER JOIN EntradaBlogAgCatTesauro ON EntradaBlogAgCatTesauro.EntradaBlogID = ent.EntradaBlogID INNER JOIN CategoriaTesauro ON EntradaBlogAgCatTesauro.CategoriaTesauroID = CategoriaTesauro.CategoriaTesauroID INNER JOIN Suscripcion ON Suscripcion.SuscripcionID = SuscripcionBlog.SuscripcionID INNER JOIN Identidad ON Identidad.IdentidadID = Suscripcion.IdentidadID" + whereFiltroIdentidad + " AND ent.FechaModificacion > " + IBD.ToParam("FechaInicial") + " AND ent.Borrador = 0 AND ent.Eliminado = 0";
                    selectGlobalCat = selectEntradasBlogsCat;

                    #endregion
                    break;
            }

            DbCommand comandoResultadosCat = mBaseDatos.GetSqlStringCommand(selectGlobalCat);
            mBaseDatos.AddInParameter(comandoResultadosCat, IBD.GuidParamValor("PerfilID"), DbType.Guid, IBD.ValorDeGuid(pPerfilID));
            mBaseDatos.AddInParameter(comandoResultadosCat, IBD.ToParam("FechaInicial"), DbType.DateTime, pFechaInicical);

            mBaseDatos.LoadDataSet(comandoResultadosCat, resDS, "CategoriaTesauro");

            #endregion

            pResultadosDS.Merge(resDS, true);
        }

        /// <summary>
        /// Devuelve true si existe alguna fila de suscripcion de la identidad para la categoria
        /// </summary>
        /// <param name="pIdentidadID">IdentidadID</param>
        /// <param name="pCategoriaID">CategoriaID</param>
        /// <param name="pObtenerBloqueadas">True si Buscar entre las suscripciones bloquedas ademas de las no bloqueadas</param>
        /// <returns>True si encuentra alguna suscripcion</returns>
        public bool IdentidadTieneSuscripcionEnCategoria(Guid pIdentidadID, Guid pCategoriaID, bool pObtenerBloqueadas)
        {
            SuscripcionDS suscDS = new SuscripcionDS();
            
            string select = sqlSelectSuscripciones + " INNER JOIN SuscripcionTesauroProyecto ON Suscripcion.SuscripcionID=SuscripcionTesauroProyecto.SuscripcionID INNER JOIN CategoriaTesVinSuscrip ON CategoriaTesVinSuscrip.SuscripcionID = Suscripcion.SuscripcionID Where Suscripcion.IdentidadID = " + IBD.ToParam("IdentidadID") + " AND CategoriaTesVinSuscrip.CategoriaTesauroID = " + IBD.ToParam("CategoriaID");

            if (!pObtenerBloqueadas)
            {
                select = select  +" AND Suscripcion.Bloqueada = 0 ";
            }
            DbCommand commandSelectSuscripcion = mBaseDatos.GetSqlStringCommand(select);
            mBaseDatos.AddInParameter(commandSelectSuscripcion, IBD.ToParam("IdentidadID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pIdentidadID));
            mBaseDatos.AddInParameter(commandSelectSuscripcion, IBD.ToParam("CategoriaID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pCategoriaID));
            mBaseDatos.LoadDataSet(commandSelectSuscripcion, suscDS, "Suscripcion");
            
            return suscDS.Suscripcion.Rows.Count > 0;
        }

        /// <summary>
        /// Devuelve el dataset con los datos de las suscripciones.
        /// </summary>
        /// <param name="pObtenerBloqueadas">True si queremos obtener las suscripciones bloquedas ademas de las no bloqueadas</param>
        /// <returns>Devuelve el dataset con las suscripciones.</returns>
        public SuscripcionDS ObtenerSuscripciones(bool pObtenerBloqueadas)
        {
            SuscripcionDS suscripcionDS = new SuscripcionDS();
            string consulta = sqlSelectSuscripciones;
            string consulta2 = sqlSelectCategoriasTesVinSuscrip;
            
            if (!pObtenerBloqueadas)
            { 
                consulta = consulta + " WHERE Suscripcion.Bloqueada = 0 " ;
                consulta2 = consulta2 + " WHERE Suscripcion.Bloqueada = 0 ";
            }
            DbCommand commandsqlSelectSuscripciones = mBaseDatos.GetSqlStringCommand(consulta);
            mBaseDatos.LoadDataSet(commandsqlSelectSuscripciones, suscripcionDS, "Suscripcion");

            DbCommand commandsqlSelectCategoriasTesVinSuscrip = mBaseDatos.GetSqlStringCommand(consulta2);
            mBaseDatos.LoadDataSet(commandsqlSelectCategoriasTesVinSuscrip, suscripcionDS, "CategoriaTesVinSuscrip");

            return (suscripcionDS);
        }

        /// <summary>
        /// Obtiene SuscripcionDS con todas las suscripciones "SuscripcionTesauroUsuario","SuscripcionTesauroProyecto",
        /// "CategoriaTesVinSuscrip", "TesVinSuscrip","SuscripcionBlog" de todas las identidades del usuario, 
        /// a través de la persona 'pPersonaID' vinculada a él
        /// </summary>
        /// <param name="pPersonaID">Clave de la persona vinculada al usuario</param>
        /// <param name="pObtenerBloqueadas">True si queremos obtener las suscripciones bloquedas ademas de las no bloqueadas</param>
        /// <returns>SuscripcionDS</returns>
        public SuscripcionDS ObtenerSuscripcionesDeUsuario(Guid pPersonaID, bool pObtenerBloqueadas)
        {
            SuscripcionDS suscDS = new SuscripcionDS();

            //Suscripcion
            string selectSuscripcion = sqlSelectSuscripciones + " INNER JOIN Identidad ON Identidad.IdentidadID = Suscripcion.IdentidadID INNER JOIN Perfil ON  Identidad.PerfilID= Perfil.PerfilID WHERE Perfil.PersonaID=" + IBD.ToParam("personaID");
            
            if (!pObtenerBloqueadas)
            { 
                selectSuscripcion = selectSuscripcion + " AND Suscripcion.Bloqueada = 0 ";
            }
            DbCommand commandSuscripcion = mBaseDatos.GetSqlStringCommand(selectSuscripcion);
            mBaseDatos.AddInParameter(commandSuscripcion, IBD.ToParam("personaID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pPersonaID));
            mBaseDatos.LoadDataSet(commandSuscripcion, suscDS, "Suscripcion");

            //SuscripcionTesauroUsuario
            string selectSuscTesUsuario = sqlSelectSuscripcionTesauroUsuario + " INNER JOIN Suscripcion ON Suscripcion.SuscripcionID = SuscripcionTesauroUsuario.SuscripcionID INNER JOIN Identidad ON Identidad.IdentidadID = Suscripcion.IdentidadID  INNER JOIN Perfil ON  Identidad.PerfilID= Perfil.PerfilID WHERE Perfil.PersonaID=" + IBD.ToParam("personaID");
            
            if (!pObtenerBloqueadas)
            {
                selectSuscTesUsuario = selectSuscTesUsuario + " AND Suscripcion.Bloqueada = 0 ";
            }
            DbCommand commandSuscTesUsuario = mBaseDatos.GetSqlStringCommand(selectSuscTesUsuario);
            mBaseDatos.AddInParameter(commandSuscTesUsuario, IBD.ToParam("personaID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pPersonaID));
            mBaseDatos.LoadDataSet(commandSuscTesUsuario, suscDS, "SuscripcionTesauroUsuario");

            //SuscripcionTesauroProyecto
            string selectSuscTesProyecto = sqlSelectSuscripcionTesauroProyecto + " INNER JOIN Suscripcion ON Suscripcion.SuscripcionID = SuscripcionTesauroProyecto.SuscripcionID INNER JOIN Identidad ON Identidad.IdentidadID = Suscripcion.IdentidadID  INNER JOIN Perfil ON  Identidad.PerfilID= Perfil.PerfilID WHERE Perfil.PersonaID=" + IBD.ToParam("personaID");
            
            if (!pObtenerBloqueadas)
            {
                selectSuscTesProyecto = selectSuscTesProyecto + " AND Suscripcion.Bloqueada = 0 ";
            }
            DbCommand commandSuscTesProy = mBaseDatos.GetSqlStringCommand(selectSuscTesProyecto);
            mBaseDatos.AddInParameter(commandSuscTesProy, IBD.ToParam("personaID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pPersonaID));
            mBaseDatos.LoadDataSet(commandSuscTesProy, suscDS, "SuscripcionTesauroProyecto");

            //CategoriaTesVinSuscrip
            string selectCatTesVinSuscrip = sqlSelectCategoriasTesVinSuscrip + " INNER JOIN Suscripcion ON Suscripcion.SuscripcionID = CategoriaTesVinSuscrip.SuscripcionID INNER JOIN Identidad ON Identidad.IdentidadID = Suscripcion.IdentidadID  INNER JOIN Perfil ON  Identidad.PerfilID= Perfil.PerfilID WHERE Perfil.PersonaID=" + IBD.ToParam("personaID");
            
            if (!pObtenerBloqueadas)
            {
                selectCatTesVinSuscrip = selectCatTesVinSuscrip + " AND Suscripcion.Bloqueada = 0 ";
            }
            DbCommand commandCatTesVinSuscrip = mBaseDatos.GetSqlStringCommand(selectCatTesVinSuscrip);
            mBaseDatos.AddInParameter(commandCatTesVinSuscrip, IBD.ToParam("personaID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pPersonaID));
            mBaseDatos.LoadDataSet(commandCatTesVinSuscrip, suscDS, "CategoriaTesVinSuscrip");

            //SuscripcionBlog
            string selectSuscripcionBlog = sqlSelectSuscripcionBlog + " INNER JOIN Suscripcion ON Suscripcion.SuscripcionID = SuscripcionBlog.SuscripcionID INNER JOIN Identidad ON Identidad.IdentidadID = Suscripcion.IdentidadID  INNER JOIN Perfil ON  Identidad.PerfilID= Perfil.PerfilID WHERE Perfil.PersonaID=" + IBD.ToParam("personaID");
            
            if (!pObtenerBloqueadas)
            {
                selectSuscripcionBlog = selectSuscripcionBlog + " AND Suscripcion.Bloqueada = 0 ";
            }
            DbCommand commandSuscripcionBlog = mBaseDatos.GetSqlStringCommand(selectSuscripcionBlog);
            mBaseDatos.AddInParameter(commandSuscripcionBlog, IBD.ToParam("personaID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pPersonaID));
            mBaseDatos.LoadDataSet(commandSuscripcionBlog, suscDS, "SuscripcionBlog");

            return suscDS;
        }

        /// <summary>
        ///  Obtiene todas las suscripciones al tesauro personal de un usuario pasado como parámetro
        /// </summary>
        /// <param name="pUsuarioID">Clave del usuario</param>
        /// <param name="pObtenerBloqueadas">True si queremos obtener las suscripciones bloquedas ademas de las no bloqueadas</param>
        /// <returns>SuscripcionDS</returns>
        public SuscripcionDS ObtenerSuscripcionesATesaurodeUsuario(Guid pUsuarioID, bool pObtenerBloqueadas)
        {
            SuscripcionDS suscripcionDS = new SuscripcionDS();
            string consulta = sqlSelectSuscripcionesAUsuario;
            string consulta2 = sqlSelectSuscripcionTesauroUsuario + " WHERE SuscripcionTesauroUsuario.UsuarioID = " + IBD.GuidParamValor("UsuarioID");

            if (!pObtenerBloqueadas)
            { 
                consulta = consulta + " AND Suscripcion.Bloqueada = 0 ";
                consulta2 = consulta2 + " AND Suscripcion.Bloqueada = 0 ";
            }
            DbCommand commandsqlSelectSuscripcionesAUsuario = mBaseDatos.GetSqlStringCommand(consulta);
            mBaseDatos.AddInParameter(commandsqlSelectSuscripcionesAUsuario, IBD.ToParam("UsuarioID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pUsuarioID));
            mBaseDatos.LoadDataSet(commandsqlSelectSuscripcionesAUsuario, suscripcionDS, "Suscripcion");

            DbCommand commandsqlSelectSuscripcionTesauroUsuario = mBaseDatos.GetSqlStringCommand(consulta2);
            mBaseDatos.AddInParameter(commandsqlSelectSuscripcionTesauroUsuario, IBD.ToParam("UsuarioID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pUsuarioID));
            mBaseDatos.LoadDataSet(commandsqlSelectSuscripcionTesauroUsuario, suscripcionDS, "SuscripcionTesauroUsuario");

            return suscripcionDS;
        }

        /// <summary>
        /// Obtiene las vinculaciones de las suscripciones que poseen ciertas categorías.
        /// </summary>
        /// <param name="pListaCategorias">Identificadores de las categorías</param>
        /// <param name="pTesauroID">Identificador del tesauro al que pertenecen las categorías</param>
        /// <returns>DataSet con las vinculaciones cargadas</returns>
        public SuscripcionDS ObtenerVinculacionesSuscripcionesDeCategoriasTesauro(List<Guid> pListaCategorias, Guid pTesauroID)
        {
            SuscripcionDS suscripcionDS = new SuscripcionDS();

            if (pListaCategorias.Count > 0)
            {
                DbCommand comando = mBaseDatos.GetSqlStringCommand(sqlSelectCategoriasTesVinSuscrip);
                comando.CommandText += " WHERE (";
                int contador = 0;
                string concatenador = "";
                
                foreach (Guid categoriaID in pListaCategorias)
                {
                    string parametroSuscripcion = IBD.ToParam("CategoriaTesauroID" + contador.ToString());

                    comando.CommandText += concatenador + " (CategoriaTesauroID=" + parametroSuscripcion + ") ";
                    mBaseDatos.AddInParameter(comando, parametroSuscripcion, IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(categoriaID));

                    contador++;
                    concatenador = " OR ";
                }
                comando.CommandText += ") AND (TesauroID=" + IBD.ToParam("TesauroID") + ") ";
                mBaseDatos.AddInParameter(comando, IBD.ToParam("TesauroID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pTesauroID));

                mBaseDatos.LoadDataSet(comando, suscripcionDS, "CategoriaTesVinSuscrip");
            }
            return suscripcionDS;
        }

        #endregion

        #region Privados

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

            this.sqlSelectSuscripciones = "SELECT " + IBD.CargarGuid("Suscripcion.SuscripcionID") + ", " + IBD.CargarGuid("Suscripcion.IdentidadID") + ", Suscripcion.Periodicidad, Suscripcion.Bloqueada FROM Suscripcion";

            this.sqlSelectSuscripcionTesauroProyecto = "SELECT " + IBD.CargarGuid("SuscripcionTesauroProyecto.SuscripcionID") + "," + IBD.CargarGuid("SuscripcionTesauroProyecto.organizacionID") + "," + IBD.CargarGuid("SuscripcionTesauroProyecto.proyectoID") + "," + IBD.CargarGuid("SuscripcionTesauroProyecto.tesauroID") + " from SuscripcionTesauroProyecto";

            this.sqlSelectSuscripcionTesauroProyectoSimple = "SELECT " + IBD.CargarGuid("SuscripcionTesauroProyecto.SuscripcionID") + "," + IBD.CargarGuid("SuscripcionTesauroProyecto.organizacionID") + "," + IBD.CargarGuid("SuscripcionTesauroProyecto.proyectoID") + "," + IBD.CargarGuid("SuscripcionTesauroProyecto.tesauroID");

            this.sqlSelectSuscripcionTesauroUsuario = "SELECT " + IBD.CargarGuid("SuscripcionTesauroUsuario.SuscripcionID") + "," + IBD.CargarGuid("SuscripcionTesauroUsuario.usuarioID") + "," + IBD.CargarGuid("SuscripcionTesauroUsuario.tesauroID") + " from SuscripcionTesauroUsuario";

            this.sqlSelectSuscripcionTesauroUsuarioSimple = "SELECT " + IBD.CargarGuid("SuscripcionTesauroUsuario.SuscripcionID") + "," + IBD.CargarGuid("SuscripcionTesauroUsuario.usuarioID") + "," + IBD.CargarGuid("SuscripcionTesauroUsuario.tesauroID");

            this.sqlSelectCategoriasTesVinSuscrip = "SELECT " + IBD.CargarGuid("CategoriaTesVinSuscrip.SuscripcionID") + ", " + IBD.CargarGuid("CategoriaTesVinSuscrip.TesauroID") + ", " + IBD.CargarGuid("CategoriaTesVinSuscrip.CategoriaTesauroID") + " FROM CategoriaTesVinSuscrip";

            this.sqlSelectCategoriasTesVinSuscripSimple = "SELECT " + IBD.CargarGuid("CategoriaTesVinSuscrip.SuscripcionID") + ", " + IBD.CargarGuid("CategoriaTesVinSuscrip.TesauroID") + ", " + IBD.CargarGuid("CategoriaTesVinSuscrip.CategoriaTesauroID");

            this.sqlSelectSuscripcionBlog = "SELECT " + IBD.CargarGuid("SuscripcionBlog.SuscripcionID") + ", " + IBD.CargarGuid("SuscripcionBlog.BlogID") + " FROM SuscripcionBlog";

            this.sqlSelectSuscripcionBlogSimple = "SELECT " + IBD.CargarGuid("SuscripcionBlog.SuscripcionID") + ", " + IBD.CargarGuid("SuscripcionBlog.BlogID");

            this.sqlSelectSuscripcionesAUsuario = sqlSelectSuscripciones + " INNER JOIN SuscripcionTesauroUsuario ON SuscripcionTesauroUsuario.SuscripcionID = Suscripcion.SuscripcionID WHERE SuscripcionTesauroUsuario.UsuarioID = " + IBD.GuidParamValor("UsuarioID");
            
            #endregion

            #region DataAdapter

            #region Suscripcion

            this.sqlSuscripcionInsert = IBD.ReplaceParam("INSERT INTO Suscripcion (SuscripcionID, Periodicidad, IdentidadID,Bloqueada) VALUES (" + IBD.GuidParamColumnaTabla("SuscripcionID") + ", @Periodicidad, " + IBD.GuidParamColumnaTabla("IdentidadID") + ", @Bloqueada)");

            this.sqlSuscripcionDelete = IBD.ReplaceParam("DELETE FROM Suscripcion WHERE (SuscripcionID = " + IBD.GuidParamColumnaTabla("O_SuscripcionID") + ") AND (Periodicidad = @O_Periodicidad) AND (IdentidadID = " + IBD.GuidParamColumnaTabla("O_IdentidadID") + ") AND (Bloqueada = @O_Bloqueada)");

            this.sqlSuscripcionModify = IBD.ReplaceParam("UPDATE Suscripcion SET SuscripcionID = " + IBD.GuidParamColumnaTabla("SuscripcionID") + ", Periodicidad = @Periodicidad, IdentidadID = " + IBD.GuidParamColumnaTabla("IdentidadID") + " , Bloqueada = @Bloqueada WHERE (SuscripcionID = " + IBD.GuidParamColumnaTabla("O_SuscripcionID") + ") AND (Periodicidad = @O_Periodicidad) AND (IdentidadID = " + IBD.GuidParamColumnaTabla("O_IdentidadID") + ") AND (Bloqueada = @O_Bloqueada)");

            #endregion

            #region SuscripcionBlog

            this.sqlSuscripcionBlogInsert = IBD.ReplaceParam("INSERT INTO SuscripcionBlog (SuscripcionID, BlogID) VALUES (" + IBD.GuidParamColumnaTabla("SuscripcionID") + ", " + IBD.GuidParamColumnaTabla("BlogID") + ")");
            
            this.sqlSuscripcionBlogDelete = IBD.ReplaceParam("DELETE FROM SuscripcionBlog WHERE (SuscripcionID = " + IBD.GuidParamColumnaTabla("O_SuscripcionID") + ") AND (BlogID = " + IBD.GuidParamColumnaTabla("O_BlogID") + ")");
            
            this.sqlSuscripcionBlogModify = IBD.ReplaceParam("UPDATE SuscripcionBlog SET SuscripcionID = " + IBD.GuidParamColumnaTabla("SuscripcionID") + ", BlogID = " + IBD.GuidParamColumnaTabla("BlogID") + " WHERE (SuscripcionID = " + IBD.GuidParamColumnaTabla("O_SuscripcionID") + ") AND (BlogID = " + IBD.GuidParamColumnaTabla("O_BlogID") + ")");

            #endregion

            #region SuscripcionTesauroProyecto

            this.sqlSuscripcionTesauroProyectoInsert = IBD.ReplaceParam("INSERT INTO SuscripcionTesauroProyecto (SuscripcionID, OrganizacionID, ProyectoID, TesauroID) VALUES (" + IBD.GuidParamColumnaTabla("SuscripcionID") + ", " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", " + IBD.GuidParamColumnaTabla("ProyectoID") + ", " + IBD.GuidParamColumnaTabla("TesauroID") + ")");
            
            this.sqlSuscripcionTesauroProyectoDelete = IBD.ReplaceParam("DELETE FROM SuscripcionTesauroProyecto WHERE (SuscripcionID = " + IBD.GuidParamColumnaTabla("O_SuscripcionID") + ") AND (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("O_ProyectoID") + ") AND (TesauroID = " + IBD.GuidParamColumnaTabla("O_TesauroID") + ")");
            this.sqlSuscripcionTesauroProyectoModify = IBD.ReplaceParam("UPDATE SuscripcionTesauroProyecto SET SuscripcionID = " + IBD.GuidParamColumnaTabla("SuscripcionID") + ", OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + ", TesauroID = " + IBD.GuidParamColumnaTabla("TesauroID") + " WHERE (SuscripcionID = " + IBD.GuidParamColumnaTabla("O_SuscripcionID") + ") AND (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("O_ProyectoID") + ") AND (TesauroID = " + IBD.GuidParamColumnaTabla("O_TesauroID") + ")");

            #endregion

            #region SuscripcionTesauroUsuario

            this.sqlSuscripcionTesauroUsuarioInsert = IBD.ReplaceParam("INSERT INTO SuscripcionTesauroUsuario (SuscripcionID, UsuarioID, TesauroID) VALUES (" + IBD.GuidParamColumnaTabla("SuscripcionID") + ", " + IBD.GuidParamColumnaTabla("UsuarioID") + ", " + IBD.GuidParamColumnaTabla("TesauroID") + ")");

            this.sqlSuscripcionTesauroUsuarioDelete = IBD.ReplaceParam("DELETE FROM SuscripcionTesauroUsuario WHERE (SuscripcionID = " + IBD.GuidParamColumnaTabla("O_SuscripcionID") + ") AND (UsuarioID = " + IBD.GuidParamColumnaTabla("O_UsuarioID") + ") AND (TesauroID = " + IBD.GuidParamColumnaTabla("O_TesauroID") + ")");

            this.sqlSuscripcionTesauroUsuarioModify = IBD.ReplaceParam("UPDATE SuscripcionTesauroUsuario SET SuscripcionID = " + IBD.GuidParamColumnaTabla("SuscripcionID") + ", UsuarioID = " + IBD.GuidParamColumnaTabla("UsuarioID") + ", TesauroID = " + IBD.GuidParamColumnaTabla("TesauroID") + " WHERE (SuscripcionID = " + IBD.GuidParamColumnaTabla("O_SuscripcionID") + ") AND (UsuarioID = " + IBD.GuidParamColumnaTabla("O_UsuarioID") + ") AND (TesauroID = " + IBD.GuidParamColumnaTabla("O_TesauroID") + ")");

            #endregion

            #region CategoriaTesVinSuscrip

            this.sqlCategoriaTesVinSuscripInsert = IBD.ReplaceParam("INSERT INTO CategoriaTesVinSuscrip (SuscripcionID, TesauroID, CategoriaTesauroID) VALUES (" + IBD.GuidParamColumnaTabla("SuscripcionID") + ", " + IBD.GuidParamColumnaTabla("TesauroID") + ", " + IBD.GuidParamColumnaTabla("CategoriaTesauroID") + ")");

            this.sqlCategoriaTesVinSuscripDelete = IBD.ReplaceParam("DELETE FROM CategoriaTesVinSuscrip WHERE (SuscripcionID = " + IBD.GuidParamColumnaTabla("O_SuscripcionID") + ") AND (TesauroID = " + IBD.GuidParamColumnaTabla("O_TesauroID") + ") AND (CategoriaTesauroID = " + IBD.GuidParamColumnaTabla("O_CategoriaTesauroID") + ")");

            this.sqlCategoriaTesVinSuscripModify = IBD.ReplaceParam("UPDATE CategoriaTesVinSuscrip SET SuscripcionID = " + IBD.GuidParamColumnaTabla("SuscripcionID") + ", TesauroID = " + IBD.GuidParamColumnaTabla("TesauroID") + ", CategoriaTesauroID = " + IBD.GuidParamColumnaTabla("CategoriaTesauroID") + " WHERE (SuscripcionID = " + IBD.GuidParamColumnaTabla("O_SuscripcionID") + ") AND (TesauroID = " + IBD.GuidParamColumnaTabla("O_TesauroID") + ") AND (CategoriaTesauroID = " + IBD.GuidParamColumnaTabla("O_CategoriaTesauroID") + ")");

            #endregion

            #endregion
        }

        #endregion

        #endregion
    }
}
