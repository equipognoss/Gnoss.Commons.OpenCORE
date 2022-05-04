using System;
using System.Collections.Generic;
using System.Text;
using Es.Riam.Gnoss.AD.ServiciosGenerales.Model;
using System.Data.Common;
using System.Data;

namespace Es.Riam.Gnoss.AD.ServiciosGenerales
{
    /// <summary>
    /// Data adapter para tipos de contrato
    /// </summary>
    public class TipoContratoAD : BaseAD
    {
        #region Consultas
        string sqlSelectTipoContratoEnEmpleado;
        string sqlSelectTiposContratos;
        #endregion

        #region TipoContratoDataAdapter
        string sqlTipoContratoInsert;
        string sqlTipoContratoDelete;
        string sqlTipoContratoModify;
        #endregion

        #region Constructores

        /// <summary>
        /// El por defecto, utilizado cuando se requiere el GnossConfig.xml por defecto
        /// </summary>
        public TipoContratoAD()
            : base()
        {
            this.CargarConsultasYDataAdapters();
        }

        /// <summary>
        /// Cuando se desea pasar directamente la ruta del fichero de configuración de conexión a base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuración de la conexión a base de datos</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public TipoContratoAD(string pFicheroConfiguracionBD)
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
            sqlSelectTipoContratoEnEmpleado = "SELECT " + IBD.CargarGuid("TipoContrato.TipoContratoID") + " FROM TipoContrato INNER JOIN PersonaVinculoOrganizacion ON TipoContrato.TipoContratoID = PersonaVinculoOrganizacion.TipoContratoID WHERE (TipoContrato.TipoContratoID = " + IBD.GuidParamValor("tipoContratoID") + ")";
            sqlSelectTiposContratos = "SELECT " + IBD.CargarGuid("TipoContratoID") + ", Nombre FROM TipoContrato ORDER BY Nombre";
            #endregion

            #region TipoContratoDataAdapter
            sqlTipoContratoInsert = IBD.ReplaceParam("INSERT INTO TipoContrato (TipoContratoID, Nombre) VALUES (" + IBD.GuidParamColumnaTabla("TipoContratoID") + ", @Nombre)");
            sqlTipoContratoDelete = IBD.ReplaceParam("DELETE FROM TipoContrato WHERE (TipoContratoID = " + IBD.GuidParamColumnaTabla("O_TipoContratoID") + ") AND (Nombre = @O_Nombre)");
            sqlTipoContratoModify = IBD.ReplaceParam("UPDATE TipoContrato SET TipoContratoID = " + IBD.GuidParamColumnaTabla("TipoContratoID") + ", Nombre = @Nombre WHERE (TipoContratoID = " + IBD.GuidParamColumnaTabla("O_TipoContratoID") + ") AND (Nombre = @O_Nombre)");
            #endregion
        }

        #endregion

        #region Públicos
        /// <summary>
        /// Obtiene todos los tipos de contratos definidos
        /// </summary>
        /// <returns>Lista de tipos de contratos</returns>
        public TipoContratoDS ObtenerTiposContratos()
        {
            TipoContratoDS tipoContratoDS = new TipoContratoDS();
            DbCommand commandsqlSelectTiposContratos = ObtenerComando(sqlSelectTiposContratos);
            CargarDataSet(commandsqlSelectTiposContratos, tipoContratoDS, "TipoContrato");
            return (tipoContratoDS);
        }

        /// <summary>
        /// Valida si para un Tipo Contrato hay Empleados asignados
        /// </summary>
        /// <param name="pTipoContratoID">Identificador del Tipo Contrato para validar</param>
        /// <returns>Verdad en caso de haber, falso en caso contrario</returns>
        public bool EstaTipoContratoAsignadoAEmpleados(Guid pTipoContratoID)
        {
            Object tipoContratoID;
            DbCommand CommandsqlSelectTipoContratoEnEmpleado = ObtenerComando(sqlSelectTipoContratoEnEmpleado);
            AgregarParametro(CommandsqlSelectTipoContratoEnEmpleado, IBD.ToParam("tipoContratoID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pTipoContratoID));

            try
            {
                tipoContratoID = EjecutarEscalar(CommandsqlSelectTipoContratoEnEmpleado);
            }
            finally
            {
            }

            return (tipoContratoID != null);
        }

        /// <summary>
        /// Actualiza tipos de contratos
        /// </summary>
        /// <param name="pTipoContratoDS">Lista de tipos de contratos</param>
        public void ActualizarTiposContratos(TipoContratoDS pTipoContratoDS)
        {
            try
            {
                #region Actualizar tabla TipoContrato
                DataSet deletedDataSet = pTipoContratoDS.GetChanges(DataRowState.Deleted);
                if (deletedDataSet != null)
                {
                    DbCommand DeleteTipoContratoCommand = ObtenerComando(sqlTipoContratoDelete);
                    AgregarParametro(DeleteTipoContratoCommand, IBD.ToParam("O_TipoContratoID"), IBD.TipoGuidToObject(DbType.Guid), "TipoContratoID", DataRowVersion.Original);
                    AgregarParametro(DeleteTipoContratoCommand, IBD.ToParam("O_Nombre"), DbType.String, "Nombre", DataRowVersion.Original);
                    ActualizarBaseDeDatos(deletedDataSet, "TipoContrato", null, null, DeleteTipoContratoCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);
                    deletedDataSet.Dispose();
                }

                DataSet addedAndModifiedDataSet = pTipoContratoDS.GetChanges(DataRowState.Added | DataRowState.Modified);
                if (addedAndModifiedDataSet != null)
                {
                    DbCommand InsertTipoContratoCommand = ObtenerComando(sqlTipoContratoInsert);
                    AgregarParametro(InsertTipoContratoCommand, IBD.ToParam("TipoContratoID"), IBD.TipoGuidToObject(DbType.Guid), "TipoContratoID", DataRowVersion.Current);
                    AgregarParametro(InsertTipoContratoCommand, IBD.ToParam("Nombre"), DbType.String, "Nombre", DataRowVersion.Current);
                    DbCommand ModifyTipoContratoCommand = ObtenerComando(sqlTipoContratoModify);
                    AgregarParametro(ModifyTipoContratoCommand, IBD.ToParam("O_TipoContratoID"), IBD.TipoGuidToObject(DbType.Guid), "TipoContratoID", DataRowVersion.Original);
                    AgregarParametro(ModifyTipoContratoCommand, IBD.ToParam("O_Nombre"), DbType.String, "Nombre", DataRowVersion.Original);
                    AgregarParametro(ModifyTipoContratoCommand, IBD.ToParam("TipoContratoID"), IBD.TipoGuidToObject(DbType.Guid), "TipoContratoID", DataRowVersion.Current);
                    AgregarParametro(ModifyTipoContratoCommand, IBD.ToParam("Nombre"), DbType.String, "Nombre", DataRowVersion.Current);
                    ActualizarBaseDeDatos(addedAndModifiedDataSet, "TipoContrato", InsertTipoContratoCommand, ModifyTipoContratoCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);
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
