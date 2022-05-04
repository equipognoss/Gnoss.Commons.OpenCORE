using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Es.Riam.Gnoss.AD.BASE_BD
{
    public class BaseVisitasAD : BaseAD
    {

        #region Constructores

        /// <summary>
        /// Cuando se desea pasar directamente la ruta del fichero de configuracion de conexion a la Base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Fichero de configuración de la base de datos BASE</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public BaseVisitasAD(string pFicheroConfiguracionBD, LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication)
        {
            this.CargarConsultasYDataAdapters(IBD);
        }

        #endregion

        #region Consultas

        private string sqlSelectVisitasDiarias;
        private string sqlSelectVisitasMensuales;

        #endregion

        #region DataAdapter

        #region VisitasDiarias
        private string sqlVisitasDiariasInsert;
        private string sqlVisitasDiariasDelete;
        private string sqlVisitasDiariasModify;
        #endregion

        #region VisitasMensuales
        private string sqlVisitasMensualesInsert;
        private string sqlVisitasMensualesDelete;
        private string sqlVisitasMensualesModify;
        #endregion

        #endregion

        #region Métodos generales

        #region Metodos AD

        /// <summary>
        /// Actualiza la BD
        /// </summary>
        /// <param name="pDataSet">Data set a actualizar</param>
        public void ActualizarBD(DataSet pDataSet)
        {
            EliminarBorrados(pDataSet);
            GuardarActualizaciones(pDataSet);
        }

        /// <summary>
        /// Elimina los elementos borrados del DataSet
        /// </summary>
        /// <param name="pDataSet">DataSet de eliminados</param>
        public void EliminarBorrados(DataSet pDataSet)
        {
            try
            {
                DataSet deletedDataSet;
                deletedDataSet = pDataSet.GetChanges(DataRowState.Deleted);
                if (deletedDataSet != null)
                {
                    #region Deleted

                    #region Eliminar tabla VisitasDiarias

                    DbCommand DeleteVisitasDiariasCommand = ObtenerComando(sqlVisitasDiariasDelete);
                    AgregarParametro(DeleteVisitasDiariasCommand, IBD.ToParam("Original_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);
                    AgregarParametro(DeleteVisitasDiariasCommand, IBD.ToParam("Original_DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Original);
                    AgregarParametro(DeleteVisitasDiariasCommand, IBD.ToParam("Original_Fecha"), DbType.DateTime, "Fecha", DataRowVersion.Original);
                    ActualizarBaseDeDatos(deletedDataSet, "VisitasDiarias", null, null, DeleteVisitasDiariasCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #region Eliminar tabla VisitasMensuales

                    DbCommand DeleteVisitasMensualesCommand = ObtenerComando(sqlVisitasMensualesDelete);
                    AgregarParametro(DeleteVisitasMensualesCommand, IBD.ToParam("Original_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);
                    AgregarParametro(DeleteVisitasMensualesCommand, IBD.ToParam("Original_DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Original);
                    AgregarParametro(DeleteVisitasMensualesCommand, IBD.ToParam("Original_Mes"), DbType.Int16, "Mes", DataRowVersion.Original);
                    ActualizarBaseDeDatos(deletedDataSet, "VisitasMensuales", null, null, DeleteVisitasMensualesCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

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
        public void GuardarActualizaciones(DataSet pDataSet)
        {
            try
            {
                DataSet addedAndModifiedDataSet;
                addedAndModifiedDataSet = pDataSet.GetChanges(DataRowState.Added | DataRowState.Modified);
                if (addedAndModifiedDataSet != null)
                {
                    #region AddedAndModified

                    #region Actualizar tabla VisitasDiarias

                    DbCommand InsertVisitasDiariasCommand = ObtenerComando(sqlVisitasDiariasInsert);

                    AgregarParametro(InsertVisitasDiariasCommand, IBD.ToParam("ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Current);
                    AgregarParametro(InsertVisitasDiariasCommand, IBD.ToParam("DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Current);
                    AgregarParametro(InsertVisitasDiariasCommand, IBD.ToParam("Fecha"), DbType.DateTime, "Fecha", DataRowVersion.Current);
                    AgregarParametro(InsertVisitasDiariasCommand, IBD.ToParam("NumVisitas"), DbType.Int32, "NumVisitas", DataRowVersion.Current);

                    DbCommand ModifyVisitasDiariasCommand = ObtenerComando(sqlVisitasDiariasModify);
                    AgregarParametro(ModifyVisitasDiariasCommand, IBD.ToParam("Original_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);
                    AgregarParametro(ModifyVisitasDiariasCommand, IBD.ToParam("Original_DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Original);
                    AgregarParametro(ModifyVisitasDiariasCommand, IBD.ToParam("Original_Fecha"), DbType.DateTime, "Fecha", DataRowVersion.Original);

                    AgregarParametro(ModifyVisitasDiariasCommand, IBD.ToParam("ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Current);
                    AgregarParametro(ModifyVisitasDiariasCommand, IBD.ToParam("DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Current);
                    AgregarParametro(ModifyVisitasDiariasCommand, IBD.ToParam("Fecha"), DbType.DateTime, "Fecha", DataRowVersion.Current);
                    AgregarParametro(ModifyVisitasDiariasCommand, IBD.ToParam("NumVisitas"), DbType.Int32, "NumVisitas", DataRowVersion.Current);

                    ActualizarBaseDeDatos(addedAndModifiedDataSet, "VisitasDiarias", InsertVisitasDiariasCommand, ModifyVisitasDiariasCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #region Actualizar tabla VisitasMensuales

                    DbCommand InsertVisitasMensualesCommand = ObtenerComando(sqlVisitasMensualesInsert);

                    AgregarParametro(InsertVisitasMensualesCommand, IBD.ToParam("ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Current);
                    AgregarParametro(InsertVisitasMensualesCommand, IBD.ToParam("DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Current);
                    AgregarParametro(InsertVisitasMensualesCommand, IBD.ToParam("Mes"), DbType.Int16, "Mes", DataRowVersion.Current);
                    AgregarParametro(InsertVisitasMensualesCommand, IBD.ToParam("NumVisitas"), DbType.Int32, "NumVisitas", DataRowVersion.Current);

                    DbCommand ModifyVisitasMensualesCommand = ObtenerComando(sqlVisitasMensualesModify);
                    AgregarParametro(ModifyVisitasMensualesCommand, IBD.ToParam("Original_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);
                    AgregarParametro(ModifyVisitasMensualesCommand, IBD.ToParam("Original_DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Original);
                    AgregarParametro(ModifyVisitasMensualesCommand, IBD.ToParam("Original_Mes"), DbType.Int16, "Mes", DataRowVersion.Original);

                    AgregarParametro(ModifyVisitasMensualesCommand, IBD.ToParam("ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Current);
                    AgregarParametro(ModifyVisitasMensualesCommand, IBD.ToParam("DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Current);
                    AgregarParametro(ModifyVisitasMensualesCommand, IBD.ToParam("Mes"), DbType.Int16, "Mes", DataRowVersion.Current);
                    AgregarParametro(ModifyVisitasMensualesCommand, IBD.ToParam("NumVisitas"), DbType.Int32, "NumVisitas", DataRowVersion.Current);

                    ActualizarBaseDeDatos(addedAndModifiedDataSet, "VisitasMensuales", InsertVisitasMensualesCommand, ModifyVisitasMensualesCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

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

            this.sqlSelectVisitasDiarias = "SELECT VisitasDiarias.ProyectoID, VisitasDiarias.DocumentoID, VisitasDiarias.Fecha, VisitasDiarias.NumVisitas FROM VisitasDiarias";
            this.sqlSelectVisitasMensuales = "SELECT VisitasMensuales.ProyectoID, VisitasMensuales.DocumentoID, VisitasMensuales.Mes, VisitasMensuales.NumVisitas FROM VisitasMensuales";

            #endregion

            #region DataAdapter

            #region VisitasDiarias

            this.sqlVisitasDiariasInsert = IBD.ReplaceParam("INSERT INTO VisitasDiarias (ProyectoID, DocumentoID, Fecha, NumVisitas) VALUES (@ProyectoID, @DocumentoID, @Fecha, @NumVisitas)");
            this.sqlVisitasDiariasDelete = IBD.ReplaceParam("DELETE FROM VisitasDiarias WHERE (ProyectoID = @Original_ProyectoID) AND (DocumentoID = @Original_DocumentoID) AND (Fecha = @Original_Fecha)");
            this.sqlVisitasDiariasModify = IBD.ReplaceParam("UPDATE VisitasDiarias SET ProyectoID = @ProyectoID, DocumentoID = @DocumentoID, Fecha = @Fecha, NumVisitas = @NumVisitas WHERE (ProyectoID = @Original_ProyectoID) AND (DocumentoID = @Original_DocumentoID) AND (Fecha = @Original_Fecha)");

            #endregion

            #region VisitasMensuales

            this.sqlVisitasMensualesInsert = IBD.ReplaceParam("INSERT INTO VisitasMensuales (ProyectoID, DocumentoID, Mes, NumVisitas) VALUES (@ProyectoID, @DocumentoID, @Mes, @NumVisitas)");
            this.sqlVisitasMensualesDelete = IBD.ReplaceParam("DELETE FROM VisitasMensuales WHERE (ProyectoID = @Original_ProyectoID) AND (DocumentoID = @Original_DocumentoID) AND (Mes = @Original_Mes)");
            this.sqlVisitasMensualesModify = IBD.ReplaceParam("UPDATE VisitasMensuales SET ProyectoID = @ProyectoID, DocumentoID = @DocumentoID, Mes = @Mes, NumVisitas = @NumVisitas WHERE (ProyectoID = @Original_ProyectoID) AND (DocumentoID = @Original_DocumentoID) AND (Mes = @Original_Mes)");

            #endregion

            #endregion

        }

        #endregion

        public DataSet ObtenerRecursosMasVisistadosProyecto(Guid pProyectoID, int dias, int pNumDocumentos)
        {
            DataSet dataSet = new DataSet();

            string nombreTabla = "VisitasDiarias" + "_" + pProyectoID.ToString().Substring(0, 2);

            string sqlVisitas = "SELECT TOP " + pNumDocumentos + " ProyectoID, DocumentoID, SUM(NumVisitas) Visitas FROM " + nombreTabla + " WHERE (ProyectoID = " + IBD.GuidValor(pProyectoID) + ")AND (Fecha > @Fecha) GROUP BY DocumentoID, ProyectoID ORDER BY Visitas DESC ";

            DbCommand cmdSqlVisitas = ObtenerComando(sqlVisitas);
            AgregarParametro(cmdSqlVisitas, IBD.ToParam("Fecha"), DbType.DateTime, DateTime.Now.Date.AddDays(-dias));
            CargarDataSet(cmdSqlVisitas, dataSet, "MasVisitados");

            return dataSet;
        }

        public void ActualizarNumeroVisitasDiariasDocumentoProyecto(Guid pDocumentoID, Guid pProyectoID, DateTime pFecha, int pNumVisitas)
        {
            DateTime fecha = new DateTime(pFecha.Year, pFecha.Month, pFecha.Day);
            string nombreTabla = "VisitasDiarias" + "_" + pProyectoID.ToString().Substring(0, 2);

            bool existeTabla = VerificarExisteTabla(nombreTabla);

            if (!existeTabla)
            {
                CrearTabla(nombreTabla);
            }

            string sqlActualizacion = "IF NOT EXISTS ( " + sqlSelectVisitasDiarias.Replace("VisitasDiarias", nombreTabla) + " WHERE (ProyectoID = " + IBD.GuidValor(pProyectoID) + ") AND (DocumentoID = " + IBD.GuidValor(pDocumentoID) + ") AND (Fecha = @Fecha)) " +
            "BEGIN " +
            "  INSERT INTO " + nombreTabla + "(ProyectoID, DocumentoID, Fecha, NumVisitas) VALUES(" + IBD.GuidValor(pProyectoID) + ", " + IBD.GuidValor(pDocumentoID) + ", @Fecha, @NumVisitas) " +
            "END " +
            "ELSE " +
            "BEGIN " +
            "  UPDATE " + nombreTabla + " SET NumVisitas = NumVisitas + @NumVisitas WHERE (ProyectoID = " + IBD.GuidValor(pProyectoID) + ") AND (DocumentoID = " + IBD.GuidValor(pDocumentoID) + ") AND (Fecha = @Fecha) " +
            "END";

            DbCommand cmdSqlActualizacion = ObtenerComando(sqlActualizacion);

            AgregarParametro(cmdSqlActualizacion, IBD.ToParam("Fecha"), DbType.DateTime, fecha);
            AgregarParametro(cmdSqlActualizacion, IBD.ToParam("NumVisitas"), DbType.Int32, pNumVisitas);

            ActualizarBaseDeDatos(cmdSqlActualizacion);
        }

        public void ActualizarNumeroVisitasMensualesDocumentoProyecto(Guid pDocumentoID, Guid pProyectoID, DateTime pFecha, int pNumVisitas)
        {
            string nombreTabla = "VisitasMensuales" + "_" + pFecha.Year;

            bool existeTabla = VerificarExisteTabla(nombreTabla);

            if (!existeTabla)
            {
                CrearTabla(nombreTabla);
            }

            string sqlActualizacion = "IF NOT EXISTS ( " + sqlSelectVisitasMensuales.Replace("VisitasMensuales", nombreTabla) + " WHERE (ProyectoID = " + IBD.GuidValor(pProyectoID) + ") AND (DocumentoID = " + IBD.GuidValor(pDocumentoID) + ") AND (Mes = @Mes)) " +
            "BEGIN " +
            "  INSERT INTO " + nombreTabla + "(ProyectoID, DocumentoID, Mes, NumVisitas) VALUES(" + IBD.GuidValor(pProyectoID) + ", " + IBD.GuidValor(pDocumentoID) + ", @Mes, @NumVisitas) " +
            "END " +
            "ELSE " +
            "BEGIN " +
            "  UPDATE " + nombreTabla + " SET NumVisitas = NumVisitas + @NumVisitas WHERE (ProyectoID = " + IBD.GuidValor(pProyectoID) + ") AND (DocumentoID = " + IBD.GuidValor(pDocumentoID) + ") AND (Mes = @Mes) " +
            "END";

            DbCommand cmdSqlActualizacion = ObtenerComando(sqlActualizacion);

            AgregarParametro(cmdSqlActualizacion, IBD.ToParam("Mes"), DbType.Int32, pFecha.Month);
            AgregarParametro(cmdSqlActualizacion, IBD.ToParam("NumVisitas"), DbType.Int32, pNumVisitas);

            ActualizarBaseDeDatos(cmdSqlActualizacion);
        }

        public void EliminarVisitasDiariasAntiguas(int pNumDiasTranscurridos)
        {
            DateTime fecha = DateTime.Now.Date;

            List<string> listaTablas = ObtenerTablasVisitasDiarias();
            foreach (string tabla in listaTablas)
            {
                string sqlActualizacion = "DELETE FROM " + tabla + " WHERE (Fecha < @Fecha)";
                DbCommand cmdSqlActualizacion = ObtenerComando(sqlActualizacion);
                AgregarParametro(cmdSqlActualizacion, IBD.ToParam("Fecha"), DbType.DateTime, fecha.AddDays(-40));

                ActualizarBaseDeDatos(cmdSqlActualizacion);
            }
        }

        /// <summary>
        /// Comprueba si existen las tablas sobre las que está configurado este AD. Si no existen las crea. 
        /// </summary>
        /// <returns>Verdad si la tabla existe (o ha sido recién creada).</returns>
        public List<string> ObtenerTablasVisitasDiarias()
        {
            string sqlTablasVisitasDiarias = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME LIKE 'VisitasDiarias%'";

            DbCommand cmdTablasVisitasDiarias = ObtenerComando(sqlTablasVisitasDiarias);

            IDataReader reader = EjecutarReader(cmdTablasVisitasDiarias);

            List<string> listaNombresTablas = new List<string>();
            while (reader.Read())
            {
                listaNombresTablas.Add(reader.GetString(0));
            }

            reader.Close();
            reader.Dispose();

            return listaNombresTablas;
        }


        /// <summary>
        /// Comprueba si existen las tablas sobre las que está configurado este AD. Si no existen las crea. 
        /// </summary>
        /// <returns>Verdad si la tabla existe (o ha sido recién creada).</returns>
        public bool VerificarExisteTabla(string pNombreTabla)
        {
            string existeTabla = "SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME = " + IBD.ToParam("nombreTabla");

            DbCommand cmdExisteTabla = ObtenerComando(existeTabla);
            AgregarParametro(cmdExisteTabla, IBD.ToParam("nombreTabla"), DbType.String, pNombreTabla);

            object resultado = EjecutarEscalar(cmdExisteTabla);

            return (resultado != null) && (resultado is int) && (resultado.Equals(1));
        }

        /// <summary>
        /// Crea una tabla en función de un tipo de conuslta
        /// </summary>
        private void CrearTabla(string pNombreTabla)
        {
            string sqlCreateTable = "";

            if (pNombreTabla.StartsWith("VisitasDiarias_"))
            {
                sqlCreateTable = "CREATE TABLE [" + pNombreTabla + "] ([ProyectoId] [uniqueidentifier] NOT NULL, [DocumentoId] [uniqueidentifier] NOT NULL, [Fecha] [datetime] NOT NULL, [NumVisitas] [int] NOT NULL, CONSTRAINT [PK_" + pNombreTabla + "] PRIMARY KEY CLUSTERED ([ProyectoId] ASC,[DocumentoId] ASC,[Fecha] ASC)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]) ON [PRIMARY]";
            }
            else if (pNombreTabla.StartsWith("VisitasMensuales_"))
            {
                sqlCreateTable = "CREATE TABLE [" + pNombreTabla + "] ([ProyectoId] [uniqueidentifier] NOT NULL, [DocumentoId] [uniqueidentifier] NOT NULL, [Mes] [smallint] NOT NULL, [NumVisitas] [int] NOT NULL, CONSTRAINT [PK_" + pNombreTabla + "] PRIMARY KEY CLUSTERED ([ProyectoId] ASC,[DocumentoId] ASC,[Mes] ASC)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]) ON [PRIMARY]";
            }

            DbCommand cmdCrearTabla = ObtenerComando(sqlCreateTable);
            ActualizarBaseDeDatos(cmdCrearTabla);
        }

        #endregion

    }
}
