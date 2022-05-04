using System;
using System.Data.Common;
using System.Data;
using Es.Riam.Util;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.Facetado;

namespace Es.Riam.Gnoss.AD.ParametroAplicacion
{
    /// <summary>
    /// Data adapter para versión
    /// </summary>
    public class VersionAD : BaseAD
    {
        #region Consultas

        string sqlSelectUltimaActualizacion;

        string sqlSelectVersion;

        #endregion

        #region DataAdapterParametrosAplicacion

        string sqlVersionInsert;

        string sqlVersionDelete;

        string sqlVersionModify;

        #endregion

        #region Constructores

        /// <summary>
        /// El por defecto, utilizado cuando se requiere el GnossConfig.xml por defecto
        /// </summary>
        public VersionAD(UtilPeticion utilPeticion, Conexion conexion, Error error, UtilTrazas utilTrazas, EntityContext entityContext, Configuracion configuracion, FacetadoAD facetadoAD)
            : base(utilPeticion, conexion, error, utilTrazas, entityContext, configuracion, facetadoAD)
        {
            this.CargarConsultasYDataAdapters();
        }

        /// <summary>
        /// Cuando se desea pasar directamente la ruta del fichero de configuración de conexión a base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuración de la conexión a base de datos</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public VersionAD(string pFicheroConfiguracionBD, UtilPeticion utilPeticion, Conexion conexion, Error error, UtilTrazas utilTrazas, EntityContext entityContext, Configuracion configuracion, FacetadoAD facetadoAD)
            : base(pFicheroConfiguracionBD, utilPeticion, conexion, error, utilTrazas, entityContext, configuracion, facetadoAD)
        {
            this.CargarConsultasYDataAdapters(IBD);
        }

        #endregion

        #region Métodos generales

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

            sqlSelectUltimaActualizacion = "SELECT ActualizacionID, FechaActualizacion, VersionGNOSS, VersionBD, NombreInstalador FROM Version WHERE (ActualizacionID IN (SELECT MAX(ActualizacionID) FROM Version))";

            sqlSelectVersion = "SELECT ActualizacionID, FechaActualizacion, VersionGNOSS, VersionBD, NombreInstalador FROM Version";

            #endregion

            #region DataAdapterParametrosAplicacion

            sqlVersionInsert = IBD.ReplaceParam("INSERT INTO Version (ActualizacionID, FechaActualizacion, VersionGNOSS, VersionBD, NombreInstalador) VALUES (@ActualizacionID, @FechaActualizacion, @VersionGNOSS, @VersionBD, @NombreInstalador)");

            sqlVersionDelete = IBD.ReplaceParam("DELETE FROM Version WHERE (ActualizacionID = @O_ActualizacionID) AND (FechaActualizacion = @O_FechaActualizacion) AND (VersionGNOSS = @O_VersionGNOSS) AND (VersionBD = @O_VersionBD OR @O_VersionBD IS NULL AND VersionBD IS NULL) AND (NombreInstalador = @O_NombreInstalador)");

            sqlVersionModify = IBD.ReplaceParam("UPDATE Version SET ActualizacionID = @ActualizacionID, FechaActualizacion = @FechaActualizacion, VersionGNOSS = @VersionGNOSS, VersionBD = @VersionBD, NombreInstalador = @NombreInstalador WHERE (ActualizacionID = @O_ActualizacionID) AND (FechaActualizacion = @O_FechaActualizacion) AND (VersionGNOSS = @O_VersionGNOSS) AND (VersionBD = @O_VersionBD OR @O_VersionBD IS NULL AND VersionBD IS NULL) AND (NombreInstalador = @O_NombreInstalador)");

            #endregion
        }

        #endregion

        /// <summary>
        /// Obtiene la última actualización
        /// </summary>
        /// <returns>Dataset de versiones</returns>
        public VersionDS ObtenerUltimaActualizacion()
        {
            VersionDS parametrosAplicacionDS = new VersionDS();
            DbCommand commandsqlSelectUltimaActualizacion = ObtenerComando(sqlSelectUltimaActualizacion);
            CargarDataSet(commandsqlSelectUltimaActualizacion, parametrosAplicacionDS, "Version");
            return (parametrosAplicacionDS);
        }

        /// <summary>
        /// Obtiene el historial de las actualizaciones
        /// </summary>
        /// <returns>Dataset de versiones</returns>
        public VersionDS ObtenerHistorialActualizaciones()
        {
            VersionDS versionDS = new VersionDS();
            DbCommand commandsqlSelectVersion = ObtenerComando(sqlSelectVersion);
            CargarDataSet(commandsqlSelectVersion, versionDS, "Version");
            return (versionDS);
        }

        /// <summary>
        /// Actualiza la tabla de parámetros de la aplicación
        /// </summary>
        /// <param name="pVersionDS">Dataset de versiones</param>
        public void ActualizarVersion(VersionDS pVersionDS)
        {
            try
            {
                //Modifico la tabla de parámetrosAplicación
                #region Delete

                DataSet deletedDataSet = pVersionDS.GetChanges(DataRowState.Deleted);

                if (deletedDataSet != null)
                {
                    DbCommand DeleteVersionCommand = ObtenerComando(sqlVersionDelete);
                    AgregarParametro(DeleteVersionCommand, IBD.ToParam("O_ActualizacionID"), DbType.Int32, "ActualizacionID", DataRowVersion.Original);
                    AgregarParametro(DeleteVersionCommand, IBD.ToParam("O_FechaActualizacion"), DbType.DateTime, "FechaActualizacion", DataRowVersion.Original);
                    AgregarParametro(DeleteVersionCommand, IBD.ToParam("O_VersionGNOSS"), DbType.String, "VersionGNOSS", DataRowVersion.Original);
                    AgregarParametro(DeleteVersionCommand, IBD.ToParam("O_VersionBD"), DbType.Int32, "VersionBD", DataRowVersion.Original);
                    AgregarParametro(DeleteVersionCommand, IBD.ToParam("O_NombreInstalador"), DbType.String, "NombreInstalador", DataRowVersion.Original);
                    ActualizarBaseDeDatos(deletedDataSet, "Version", null, null, DeleteVersionCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);
                    deletedDataSet.Dispose();
                }

                #endregion

                #region AddedAndModified

                DataSet addedAndModifiedDataSet = pVersionDS.GetChanges(DataRowState.Added | DataRowState.Modified);

                if (addedAndModifiedDataSet != null)
                {
                    DbCommand InsertVersionCommand = ObtenerComando(sqlVersionInsert);
                    AgregarParametro(InsertVersionCommand, IBD.ToParam("ActualizacionID"), DbType.Int32, "ActualizacionID", DataRowVersion.Current);
                    AgregarParametro(InsertVersionCommand, IBD.ToParam("FechaActualizacion"), DbType.DateTime, "FechaActualizacion", DataRowVersion.Current);
                    AgregarParametro(InsertVersionCommand, IBD.ToParam("VersionGNOSS"), DbType.String, "VersionGNOSS", DataRowVersion.Current);
                    AgregarParametro(InsertVersionCommand, IBD.ToParam("VersionBD"), DbType.Int32, "VersionBD", DataRowVersion.Current);
                    AgregarParametro(InsertVersionCommand, IBD.ToParam("NombreInstalador"), DbType.String, "NombreInstalador", DataRowVersion.Current);
                    DbCommand ModifyVersionCommand = ObtenerComando(sqlVersionModify);
                    AgregarParametro(ModifyVersionCommand, IBD.ToParam("O_ActualizacionID"), DbType.Int32, "ActualizacionID", DataRowVersion.Original);
                    AgregarParametro(ModifyVersionCommand, IBD.ToParam("O_FechaActualizacion"), DbType.DateTime, "FechaActualizacion", DataRowVersion.Original);
                    AgregarParametro(ModifyVersionCommand, IBD.ToParam("O_VersionGNOSS"), DbType.String, "VersionGNOSS", DataRowVersion.Original);
                    AgregarParametro(ModifyVersionCommand, IBD.ToParam("O_VersionBD"), DbType.Int32, "VersionBD", DataRowVersion.Original);
                    AgregarParametro(ModifyVersionCommand, IBD.ToParam("O_NombreInstalador"), DbType.String, "NombreInstalador", DataRowVersion.Original);
                    AgregarParametro(ModifyVersionCommand, IBD.ToParam("ActualizacionID"), DbType.Int32, "ActualizacionID", DataRowVersion.Current);
                    AgregarParametro(ModifyVersionCommand, IBD.ToParam("FechaActualizacion"), DbType.DateTime, "FechaActualizacion", DataRowVersion.Current);
                    AgregarParametro(ModifyVersionCommand, IBD.ToParam("VersionGNOSS"), DbType.String, "VersionGNOSS", DataRowVersion.Current);
                    AgregarParametro(ModifyVersionCommand, IBD.ToParam("VersionBD"), DbType.Int32, "VersionBD", DataRowVersion.Current);
                    AgregarParametro(ModifyVersionCommand, IBD.ToParam("NombreInstalador"), DbType.String, "NombreInstalador", DataRowVersion.Current);
                    ActualizarBaseDeDatos(addedAndModifiedDataSet, "Version", InsertVersionCommand, ModifyVersionCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);
                    addedAndModifiedDataSet.Dispose();
                }

                #endregion
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

    }
}
