using System;
using System.Collections.Generic;
using System.Text;
using Es.Riam.Gnoss.AD.ServiciosGenerales.Model;
using System.Data.Common;
using System.Data;

namespace Es.Riam.Gnoss.AD.ServiciosGenerales
{
    /// <summary>
    /// Data adapter para estado laboral
    /// </summary>
    public class EstadoLaboralAD : BaseAD
    {
        #region Consultas

        string sqlSelectEstadosLaborales;

        string sqlSelectEstadoLaboralEnHistorico;

        #endregion

        #region DataAdapterEstadoLaboral
        string sqlEstadoLaboralInsert;

        string sqlEstadoLaboralDelete;

        string sqlEstadoLaboralModify;
        #endregion

        #region Constructores

        /// <summary>
        /// El por defecto, utilizado cuando se requiere el GnossConfig.xml por defecto
        /// </summary>
        public EstadoLaboralAD()
            : base()
        {
            this.CargarConsultasYDataAdapters();
        }

        /// <summary>
        /// Cuando se desea pasar directamente la ruta del fichero de configuración de conexión a base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuración de la conexión a base de datos</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public EstadoLaboralAD(string pFicheroConfiguracionBD)
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

            sqlSelectEstadosLaborales = "SELECT " + IBD.CargarGuid("EstadoLaboralID") + ", Descripcion FROM EstadoLaboral ORDER BY Descripcion";

            sqlSelectEstadoLaboralEnHistorico = "SELECT " + IBD.CargarGuid("EstadoLaboral.EstadoLaboralID") + " FROM EstadoLaboral INNER JOIN EmpleadoHistoricoEstadoLaboral ON EstadoLaboral.EstadoLaboralID = EmpleadoHistoricoEstadoLaboral.EstadoLaboralID WHERE (EstadoLaboral.EstadoLaboralID = " + IBD.GuidParamValor("estadoLaboralID") + ")";

            #endregion

            #region DataAdapterEstadoLaboral
            sqlEstadoLaboralInsert = IBD.ReplaceParam("INSERT INTO EstadoLaboral (EstadoLaboralID, Descripcion) VALUES (" + IBD.GuidParamColumnaTabla("EstadoLaboralID") + ", @Descripcion)");

            sqlEstadoLaboralDelete = IBD.ReplaceParam("DELETE FROM EstadoLaboral WHERE (EstadoLaboralID = " + IBD.GuidParamColumnaTabla("O_EstadoLaboralID") + ") AND (Descripcion = @O_Descripcion)");

            sqlEstadoLaboralModify = IBD.ReplaceParam("UPDATE EstadoLaboral SET EstadoLaboralID = " + IBD.GuidParamColumnaTabla("EstadoLaboralID") + ", Descripcion = @Descripcion WHERE (EstadoLaboralID = " + IBD.GuidParamColumnaTabla("O_EstadoLaboralID") + ") AND (Descripcion = @O_Descripcion)");
            #endregion
        }

        #endregion

        #region Públicos
        /// <summary> 
        /// Obtiene todos los estados laborales
        /// </summary>
        public EstadoLaboralDS ObtenerEstadosLaborales()
        {
            EstadoLaboralDS estadoLaboralDS = new EstadoLaboralDS();
            DbCommand commandSelectEstadosLaboralesOrganizacion = ObtenerComando(sqlSelectEstadosLaborales);
            CargarDataSet(commandSelectEstadosLaboralesOrganizacion, estadoLaboralDS, "EstadoLaboral");
            return (estadoLaboralDS);
        }
        /// <summary>
        /// Valida si una Estado Laboral está asignado en un Histórico de persona
        /// </summary>
        /// <param name="pEstadoLaboralID">Identificador del EstadoLaboral</param>
        /// <returns>Verdad en caso de estar, falso en caso contrario</returns>
        public bool EstaEstadoLaboralAsignadoEnHistorico(Guid pEstadoLaboralID)
        {
            try
            {
                object estadoLaboralID;
                DbCommand CommandSelectEstadoLaboralEnHistorico = ObtenerComando(sqlSelectEstadoLaboralEnHistorico);
                AgregarParametro(CommandSelectEstadoLaboralEnHistorico, IBD.ToParam("estadoLaboralID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pEstadoLaboralID));
                estadoLaboralID = EjecutarEscalar(CommandSelectEstadoLaboralEnHistorico);
                return (estadoLaboralID != null);
            }
            catch (Exception x)
            {
                throw x;
            }
        }
        /// <summary>
        /// Actualiza los estados laborales 
        /// </summary>
        /// <param name="pEstadoLaboralDS">Lista de estados laborales</param>
        public void ActualizarEstadosLaborales(EstadoLaboralDS pEstadoLaboralDS)
        {
            try
            {

                #region Eliminar borrados de la tabla EstadoLaboral
                DataSet deletedDataSet = pEstadoLaboralDS.GetChanges(DataRowState.Deleted);
                if (deletedDataSet != null)
                {
                    DbCommand DeleteEstadoLaboralCommand = ObtenerComando(sqlEstadoLaboralDelete);
                    AgregarParametro(DeleteEstadoLaboralCommand, IBD.ToParam("O_EstadoLaboralID"), IBD.TipoGuidToObject(DbType.Guid), "EstadoLaboralID", DataRowVersion.Original);
                    AgregarParametro(DeleteEstadoLaboralCommand, IBD.ToParam("O_Descripcion"), DbType.String, "Descripcion", DataRowVersion.Original);
                    ActualizarBaseDeDatos(deletedDataSet, "EstadoLaboral", null, null, DeleteEstadoLaboralCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    deletedDataSet.Dispose();
                }

                #endregion

                #region Guardar Actualizaciones de EstadoLaboral
                DataSet addedAndModifiedDataSet = pEstadoLaboralDS.GetChanges(DataRowState.Added | DataRowState.Modified);
                if (addedAndModifiedDataSet != null)
                {
                    DbCommand InsertEstadoLaboralCommand = ObtenerComando(sqlEstadoLaboralInsert);
                    AgregarParametro(InsertEstadoLaboralCommand, IBD.ToParam("EstadoLaboralID"), IBD.TipoGuidToObject(DbType.Guid), "EstadoLaboralID", DataRowVersion.Current);
                    AgregarParametro(InsertEstadoLaboralCommand, IBD.ToParam("Descripcion"), DbType.String, "Descripcion", DataRowVersion.Current);

                    DbCommand ModifyEstadoLaboralCommand = ObtenerComando(sqlEstadoLaboralModify);
                    AgregarParametro(ModifyEstadoLaboralCommand, IBD.ToParam("O_EstadoLaboralID"), IBD.TipoGuidToObject(DbType.Guid), "EstadoLaboralID", DataRowVersion.Original);
                    AgregarParametro(ModifyEstadoLaboralCommand, IBD.ToParam("O_Descripcion"), DbType.String, "Descripcion", DataRowVersion.Original);
                    AgregarParametro(ModifyEstadoLaboralCommand, IBD.ToParam("EstadoLaboralID"), IBD.TipoGuidToObject(DbType.Guid), "EstadoLaboralID", DataRowVersion.Current);
                    AgregarParametro(ModifyEstadoLaboralCommand, IBD.ToParam("Descripcion"), DbType.String, "Descripcion", DataRowVersion.Current);
                    ActualizarBaseDeDatos(addedAndModifiedDataSet, "EstadoLaboral", InsertEstadoLaboralCommand, ModifyEstadoLaboralCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

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
