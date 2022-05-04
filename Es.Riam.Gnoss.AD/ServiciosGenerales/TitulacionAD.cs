using System;
using System.Collections.Generic;
using System.Text;
using Es.Riam.Gnoss.AD.ServiciosGenerales.Model;
using System.Data.Common;
using System.Data;

namespace Es.Riam.Gnoss.AD.ServiciosGenerales
{
    /// <summary>
    /// Data adapter para titulaciones
    /// </summary>
    public class TitulacionAD : BaseAD
    {
        #region Consultas

        string sqlSelectTitulacionEnEmpleado;

        string sqlSelectTitulaciones;

        #endregion

        #region TitulacionDataAdapter

        string sqlTitulacionInsert;

        string sqlTitulacionDelete;

        string sqlTitulacionModify;

        #endregion

        #region Constructores

        /// <summary>
        /// El por defecto, utilizado cuando se requiere el GnossConfig.xml por defecto
        /// </summary>
        public TitulacionAD()
            : base()
        {
            this.CargarConsultasYDataAdapters();
        }

        /// <summary>
        /// Cuando se desea pasar directamente la ruta del fichero de configuración de conexión a base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuración de la conexión a base de datos</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public TitulacionAD(string pFicheroConfiguracionBD)
            : base(pFicheroConfiguracionBD)
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

            sqlSelectTitulacionEnEmpleado = "SELECT " + IBD.CargarGuid("Titulacion.TitulacionID") + " FROM Titulacion INNER JOIN Persona ON Titulacion.TitulacionID = Persona.TitulacionID WHERE (Titulacion.TitulacionID = " + IBD.GuidParamValor("titulacionID") + ")";

            sqlSelectTitulaciones = "SELECT " + IBD.CargarGuid("TitulacionID") + ", Descripcion, AñosEscolaridad FROM Titulacion ORDER BY Descripcion";

            #endregion

            #region TitulacionDataAdapter

            sqlTitulacionInsert = IBD.ReplaceParam("INSERT INTO Titulacion (TitulacionID, Descripcion, AñosEscolaridad) VALUES (" + IBD.GuidParamColumnaTabla("TitulacionID") + ", @Descripcion, @AñosEscolaridad)");

            sqlTitulacionDelete = IBD.ReplaceParam("DELETE FROM Titulacion WHERE (TitulacionID = " + IBD.GuidParamColumnaTabla("O_TitulacionID") + ") AND (Descripcion = @O_Descripcion) AND (AñosEscolaridad = @O_AñosEscolaridad)");

            sqlTitulacionModify = IBD.ReplaceParam("UPDATE Titulacion SET TitulacionID = " + IBD.GuidParamColumnaTabla("TitulacionID") + ", Descripcion = @Descripcion, AñosEscolaridad = @AñosEscolaridad WHERE (TitulacionID = " + IBD.GuidParamColumnaTabla("O_TitulacionID") + ") AND (Descripcion = @O_Descripcion) AND (AñosEscolaridad = @O_AñosEscolaridad)");

            #endregion
        }

        #endregion

        #region Públicos

        /// <summary>
        /// Obtiene todas las titulaciones definidas
        /// </summary>
        /// <returns>Dataset de titulaciones</returns>
        public TitulacionDS ObtenerTitulaciones()
        {
            TitulacionDS titulacionDS = new TitulacionDS();
            DbCommand commandsqlSelectTitulaciones = ObtenerComando(sqlSelectTitulaciones);
            CargarDataSet(commandsqlSelectTitulaciones, titulacionDS, "Titulacion");
            return (titulacionDS);
        }

        /// <summary>
        /// Comprueba si para una titulación hay empleados asignados
        /// </summary>
        /// <param name="pTitulacionID">Identificador de la titulación para validar</param>
        /// <returns>TRUE en caso de estar asignado, FALSE en caso contrario</returns>
        public bool EstaTitulacionAsignadaAEmpleados(Guid pTitulacionID)
        {
            Object titulacionID;
            DbCommand CommandsqlSelectTitulacionEnEmpleado = ObtenerComando(sqlSelectTitulacionEnEmpleado);
            AgregarParametro(CommandsqlSelectTitulacionEnEmpleado, IBD.ToParam("titulacionID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pTitulacionID));

            try
            {
                titulacionID = EjecutarEscalar(CommandsqlSelectTitulacionEnEmpleado);
            }
            finally
            {
            }

            return (titulacionID != null);
        }

        /// <summary>
        /// Actualiza las titulaciones
        /// </summary>
        /// <param name="pTitulacionDS">Dataset de titulaciones</param>
        public void ActualizarTitulaciones(TitulacionDS pTitulacionDS)
        {
            try
            {
                #region Actualizo la tabla Titulacion

                DataSet deletedDataSet = pTitulacionDS.GetChanges(DataRowState.Deleted);

                if (deletedDataSet != null)
                {
                    DbCommand DeleteTitulacionCommand = ObtenerComando(sqlTitulacionDelete);
                    AgregarParametro(DeleteTitulacionCommand, IBD.ToParam("O_TitulacionID"), IBD.TipoGuidToObject(DbType.Guid), "TitulacionID", DataRowVersion.Original);
                    AgregarParametro(DeleteTitulacionCommand, IBD.ToParam("O_Descripcion"), DbType.String, "Descripcion", DataRowVersion.Original);
                    AgregarParametro(DeleteTitulacionCommand, IBD.ToParam("O_AñosEscolaridad"), DbType.Int16, "AñosEscolaridad", DataRowVersion.Original);
                    ActualizarBaseDeDatos(deletedDataSet, "Titulacion", null, null, DeleteTitulacionCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);
                    deletedDataSet.Dispose();
                }

                DataSet addedAndModifiedDataSet = pTitulacionDS.GetChanges(DataRowState.Added | DataRowState.Modified);

                if (addedAndModifiedDataSet != null)
                {
                    DbCommand InsertTitulacionCommand = ObtenerComando(sqlTitulacionInsert);
                    AgregarParametro(InsertTitulacionCommand, IBD.ToParam("TitulacionID"), IBD.TipoGuidToObject(DbType.Guid), "TitulacionID", DataRowVersion.Current);
                    AgregarParametro(InsertTitulacionCommand, IBD.ToParam("Descripcion"), DbType.String, "Descripcion", DataRowVersion.Current);
                    AgregarParametro(InsertTitulacionCommand, IBD.ToParam("AñosEscolaridad"), DbType.Int16, "AñosEscolaridad", DataRowVersion.Current);

                    DbCommand ModifyTitulacionCommand = ObtenerComando(sqlTitulacionModify);
                    AgregarParametro(ModifyTitulacionCommand, IBD.ToParam("O_TitulacionID"), IBD.TipoGuidToObject(DbType.Guid), "TitulacionID", DataRowVersion.Original);
                    AgregarParametro(ModifyTitulacionCommand, IBD.ToParam("O_Descripcion"), DbType.String, "Descripcion", DataRowVersion.Original);
                    AgregarParametro(ModifyTitulacionCommand, IBD.ToParam("O_AñosEscolaridad"), DbType.Int16, "AñosEscolaridad", DataRowVersion.Original);
                    AgregarParametro(ModifyTitulacionCommand, IBD.ToParam("TitulacionID"), IBD.TipoGuidToObject(DbType.Guid), "TitulacionID", DataRowVersion.Current);
                    AgregarParametro(ModifyTitulacionCommand, IBD.ToParam("Descripcion"), DbType.String, "Descripcion", DataRowVersion.Current);
                    AgregarParametro(ModifyTitulacionCommand, IBD.ToParam("AñosEscolaridad"), DbType.Int16, "AñosEscolaridad", DataRowVersion.Current);
                    ActualizarBaseDeDatos(addedAndModifiedDataSet, "Titulacion", InsertTitulacionCommand, ModifyTitulacionCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);
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

        #endregion
    }
}
