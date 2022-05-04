using System;
using System.Data.Common;
using System.Data;
using Es.Riam.Util;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.AD.EntityModel;

namespace Es.Riam.Gnoss.AD.ServiciosGenerales
{
    /// <summary>
    /// Data adapter para estado civil
    /// </summary>
    public class EstadoCivilAD : BaseAD
    {
        #region Consultas
        string sqlSelectEstadoCivilEnEmpleado;
        string sqlSelectEstadoCivilEnPersona;


        string sqlSelectEstadosCiviles;
        #endregion

        #region DataAdapter
        #region EstadoCivil
        private string sqlEstadoCivilInsert;
        private string sqlEstadoCivilDelete;
        private string sqlEstadoCivilModify;
        #endregion
        #endregion

        #region Constructores

        /// <summary>
        /// El por defecto, utilizado cuando se requiere el GnossConfig.xml por defecto
        /// </summary>
        public EstadoCivilAD(UtilPeticion utilPeticion, Conexion conexion, Error error, UtilTrazas utilTrazas, EntityContext entityContext)
            : base(utilPeticion, conexion, error, utilTrazas, entityContext)
        {
            this.CargarConsultasYDataAdapters();
        }

        /// <summary>
        /// Cuando se desea pasar directamente la ruta del fichero de configuración de conexión a base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuración de la conexión a base de datos</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public EstadoCivilAD(string pFicheroConfiguracionBD, UtilPeticion utilPeticion, Conexion conexion, Error error, UtilTrazas utilTrazas, EntityContext entityContext)
            : base(pFicheroConfiguracionBD, utilPeticion, conexion, error, utilTrazas, entityContext)
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
            sqlSelectEstadoCivilEnEmpleado = "SELECT " + IBD.CargarGuid("EstadoCivil.EstadoCivilID") + " FROM EstadoCivil INNER JOIN Empleado ON EstadoCivil.EstadoCivilID = Empleado.EstadoCivilID WHERE (EstadoCivil.EstadoCivilID = " + IBD.GuidParamValor("estadoCivilID") + ")";
            sqlSelectEstadoCivilEnPersona = "SELECT " + IBD.CargarGuid("EstadoCivil.EstadoCivilID") + " FROM EstadoCivil INNER JOIN Persona ON EstadoCivil.EstadoCivilID = Persona.EstadoCivilID WHERE (EstadoCivil.EstadoCivilID = " + IBD.GuidParamValor("estadoCivilID") + ")";


            sqlSelectEstadosCiviles = "SELECT " + IBD.CargarGuid("EstadoCivilID") + ", Descripcion FROM EstadoCivil";
            #endregion

            #region DataAdapter
            #region EstadoCivil
            sqlEstadoCivilInsert = IBD.ReplaceParam("INSERT INTO EstadoCivil (EstadoCivilID, Descripcion) VALUES (" + IBD.GuidParamColumnaTabla("EstadoCivilID") + ", @Descripcion)");
            sqlEstadoCivilDelete = IBD.ReplaceParam("DELETE FROM EstadoCivil WHERE (EstadoCivilID = " + IBD.GuidParamColumnaTabla("O_EstadoCivilID") + ") AND (Descripcion = @O_Descripcion)");
            sqlEstadoCivilModify = IBD.ReplaceParam("UPDATE EstadoCivil SET EstadoCivilID = " + IBD.GuidParamColumnaTabla("EstadoCivilID") + ", Descripcion = @Descripcion WHERE (EstadoCivilID = " + IBD.GuidParamColumnaTabla("O_EstadoCivilID") + ") AND (Descripcion = @O_Descripcion)");
            #endregion
            #endregion
        }

        #endregion

        #region Públicos
        /// <summary>
        ///  Obtiene todos los estados civiles definidos 
        /// </summary>
        /// <returns>Lista de estados civiles</returns>
        public EstadoCivilDS ObtenerEstadosCiviles()
        {
            EstadoCivilDS estadoCivilDS = new EstadoCivilDS();
            DbCommand commandsqlSelectEstadosCiviles = ObtenerComando(sqlSelectEstadosCiviles);
            CargarDataSet(commandsqlSelectEstadosCiviles, estadoCivilDS, "EstadoCivil");
            return (estadoCivilDS);
        }

        /// <summary>
        /// Valida si para un Estado Civil hay Empleados asignados
        /// </summary>
        /// <param name="pEstadoCivilID">Identificador del Estado Civil para validar</param>
        /// <returns>Verdad en caso de estar, falso en caso contrario</returns>
        public bool EstaEstadoCivilAsignadoAPersona(Guid pEstadoCivilID)
        {
            Object estadoCivilID;
            DbCommand commandsqlSelectEstadoCivilEnPersona = ObtenerComando(sqlSelectEstadoCivilEnPersona);
            AgregarParametro(commandsqlSelectEstadoCivilEnPersona, IBD.ToParam("estadoCivilID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pEstadoCivilID));
            try
            {
                estadoCivilID = EjecutarEscalar(commandsqlSelectEstadoCivilEnPersona);
            }
            finally
            {
            }

            return (estadoCivilID != null);

        }

        /// <summary>
        /// Actualiza los estados civiles
        /// </summary>
        /// <param name="pEstadoCivilDS">Lista de estados civiles</param>
        public void ActualizarEstadosCiviles(EstadoCivilDS pEstadoCivilDS)
        {
            try
            {
                #region Actualizo la tabla EstadoCivil
                DataSet deletedDataSet = pEstadoCivilDS.GetChanges(DataRowState.Deleted);
                if (deletedDataSet != null)
                {
                    #region Eliminar tabla EstadoCivil
                    DbCommand DeleteEstadoCivilCommand = ObtenerComando(sqlEstadoCivilDelete);
                    AgregarParametro(DeleteEstadoCivilCommand, IBD.ToParam("O_EstadoCivilID"), IBD.TipoGuidToObject(DbType.Guid), "EstadoCivilID", DataRowVersion.Original);
                    AgregarParametro(DeleteEstadoCivilCommand, IBD.ToParam("O_Descripcion"), DbType.String, "Descripcion", DataRowVersion.Original);
                    ActualizarBaseDeDatos(deletedDataSet, "EstadoCivil", null, null, DeleteEstadoCivilCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    deletedDataSet.Dispose();
                    #endregion
                }

                DataSet addedAndModifiedDataSet;
                addedAndModifiedDataSet = pEstadoCivilDS.GetChanges(DataRowState.Added | DataRowState.Modified);
                if (addedAndModifiedDataSet != null)
                {
                    #region Actualizar tabla EstadoCivil
                    DbCommand InsertEstadoCivilCommand = ObtenerComando(sqlEstadoCivilInsert);
                    AgregarParametro(InsertEstadoCivilCommand, IBD.ToParam("EstadoCivilID"), IBD.TipoGuidToObject(DbType.Guid), "EstadoCivilID", DataRowVersion.Current);
                    AgregarParametro(InsertEstadoCivilCommand, IBD.ToParam("Descripcion"), DbType.String, "Descripcion", DataRowVersion.Current);

                    DbCommand ModifyEstadoCivilCommand = ObtenerComando(sqlEstadoCivilModify);
                    AgregarParametro(ModifyEstadoCivilCommand, IBD.ToParam("O_EstadoCivilID"), IBD.TipoGuidToObject(DbType.Guid), "EstadoCivilID", DataRowVersion.Original);
                    AgregarParametro(ModifyEstadoCivilCommand, IBD.ToParam("O_Descripcion"), DbType.String, "Descripcion", DataRowVersion.Original);

                    AgregarParametro(ModifyEstadoCivilCommand, IBD.ToParam("EstadoCivilID"), IBD.TipoGuidToObject(DbType.Guid), "EstadoCivilID", DataRowVersion.Current);
                    AgregarParametro(ModifyEstadoCivilCommand, IBD.ToParam("Descripcion"), DbType.String, "Descripcion", DataRowVersion.Current);
                    ActualizarBaseDeDatos(addedAndModifiedDataSet, "EstadoCivil", InsertEstadoCivilCommand, ModifyEstadoCivilCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    addedAndModifiedDataSet.Dispose();
                    #endregion
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
