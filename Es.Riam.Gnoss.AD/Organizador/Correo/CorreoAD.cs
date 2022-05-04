using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.Organizador.Correo.Model;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Es.Riam.Gnoss.AD.Organizador.Correo
{
    #region Enumeraciones

    /// <summary>
    /// Enumeración que contiene los diferentes tipos de listados de correo
    /// </summary>
    public enum TiposListadoCorreo
    {
        /// <summary>
        /// Correos en borrador
        /// </summary>
        Borrador = 0,
        /// <summary>
        /// Correos recibidos
        /// </summary>
        Recibido = 1,
        /// <summary>
        /// Correos enviados
        /// </summary>
        Enviado = 2,
        /// <summary>
        /// Correos eliminados
        /// </summary>
        Eliminado = 3
    }

    #endregion

    /// <summary>
    /// DataAdapter para correo interno
    /// </summary>
    public class CorreoAD : BaseAD
    {
        #region Constructores

        /// <summary>
        /// El por defecto, utilizado cuando se requiere el GnossConfig.xml por defecto
        /// </summary>
        public CorreoAD(LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication)
        {
            this.CargarConsultasYDataAdapters();
        }

        /// <summary>
        /// Cuando se desea pasar directamente la ruta del fichero de configuración de conexión a la base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuración de conexión a la base de datos</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public CorreoAD(string pFicheroConfiguracionBD, LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication)
        {
            this.CargarConsultasYDataAdapters(IBD);
        }

        #endregion

        #region Consultas

        private string selectCorreoInterno;
        private string sqlSelectCorreoInternoPorAutor;
        private string sqlSelectCorreoInternoPorDestinatario;
        private string sqlSelectCorreoInternoPendientePorDestinatario;
        private string sqlSelectCorreoEnPapeleraDeUsuario;

        #endregion

        #region DataAdapter

        #region CorreoInterno
        private string sqlCorreoInternoInsert;
        private string sqlCorreoInternoDelete;
        private string sqlCorreoInternoModify;
        #endregion

        private string sqlCorreoLeidoUpdate;

        private string sqlCorreoRestaurarUpdate;

        private string sqlCorreoEliminarDefinitivamenteUpdate;

        private string sqlCorreoEliminarUpdate;

        #endregion

        #region Métodos generales

        #region Públicos

        /// <summary>
        /// Actualiza la base de datos con eliminaciones y modificaciones del dataset pasado por parámetro
        /// </summary>
        /// <param name="pDataSet">Dataset para actualizar</param>
        public void ActualizarCorreo(DataSet pDataSet)
        {
            EliminarBorrados(pDataSet);
            GuardarActualizaciones(pDataSet);
        }

        /// <summary>
        /// Actualiza el correo pasado por parámetro marcándolo como leído
        /// </summary>
        /// <param name="pDataSet">Dataset</param>
        /// <param name="pCorreoID">Identificador de correo</param>
        /// <param name="pDestinatario">Identificador del destinatario del correo</param>
        public void ActualizarCorreoLeido(DataSet pDataSet, Guid pCorreoID, Guid pDestinatario)
        {
            bool esOracle = (ConexionMaster is OracleConnection);

            string tablaCorreoInterno = $"CorreoInterno_{pDestinatario.ToString().Substring(0, 2)}";

            DbCommand UpdateCorreoLeidoCommand = ObtenerComando($"UPDATE \"{tablaCorreoInterno}\" SET \"Leido\" = 1 WHERE \"CorreoID\" = {IBD.FormatearGuid(pCorreoID)} AND \"Destinatario\" = {IBD.FormatearGuid(pDestinatario)}");

            ActualizarBaseDeDatos(UpdateCorreoLeidoCommand, true, esOracle);

            pDataSet.AcceptChanges();
        }

        /// <summary>
        /// Restaura el correo pasado por parámetro
        /// </summary>
        /// <param name="pAutor"></param>
        /// <param name="pCorreoID"></param>
        /// <param name="pDestinatario"></param>
        public void RestaurarCorreo(Guid pCorreoID, Guid pAutor, Guid pDestinatario)
        {
            string tablaCorreoInterno = "CorreoInterno_" + pDestinatario.ToString().Substring(0, 2);

            //Enviado
            if (pDestinatario == Guid.Empty)
            {
                tablaCorreoInterno = "CorreoInterno_" + pAutor.ToString().Substring(0, 2);
            }
            DbCommand UpdateCorreoRestaurado = ObtenerComando($"UPDATE \"{tablaCorreoInterno}\" SET \"EnPapelera\" = 0 WHERE \"CorreoID\" = {IBD.FormatearGuid(pCorreoID)} AND \"Destinatario\" = {IBD.FormatearGuid(pDestinatario)} AND \"Autor\" = {IBD.FormatearGuid(pAutor)}");

            ActualizarBaseDeDatos(UpdateCorreoRestaurado);
        }

        /// <summary>
        /// Elimina el correo pasado por parámetro
        /// </summary>
        /// <param name="pAutor"></param>
        /// <param name="pCorreoID"></param>
        /// <param name="pDestinatario"></param>
        public void EliminarCorreoDefinitivamente(Guid pCorreoID, Guid pAutor, Guid pDestinatario)
        {
            string tablaCorreoInterno = "CorreoInterno_" + pDestinatario.ToString().Substring(0, 2);

            //Enviado
            if (pDestinatario == Guid.Empty)
            {
                tablaCorreoInterno = "CorreoInterno_" + pAutor.ToString().Substring(0, 2);
            }

            DbCommand UpdateCorreoEliminadoDefinitivamente = ObtenerComando($"UPDATE \"{tablaCorreoInterno}\" SET \"Eliminado\" = 1 WHERE \"CorreoID\" = {IBD.FormatearGuid(pCorreoID)} AND \"Destinatario\" = {IBD.FormatearGuid(pDestinatario)} AND \"Autor\" = {IBD.FormatearGuid(pAutor)}");

            ActualizarBaseDeDatos(UpdateCorreoEliminadoDefinitivamente);
        }

        /// <summary>
        /// Elimina el correo pasado por parámetro
        /// </summary>
        /// <param name="pAutor"></param>
        /// <param name="pCorreoID"></param>
        /// <param name="pDestinatario"></param>
        public void EliminarCorreo(Guid pCorreoID, Guid pAutor, Guid pDestinatario)
        {
            string tablaCorreoInterno = "CorreoInterno_" + pDestinatario.ToString().Substring(0, 2);

            //Enviado
            if (pDestinatario == Guid.Empty)
            {
                tablaCorreoInterno = "CorreoInterno_" + pAutor.ToString().Substring(0, 2);
            }

            DbCommand UpdateCorreoEliminado = ObtenerComando($"UPDATE \"{tablaCorreoInterno}\" SET \"EnPapelera\" = 1 WHERE \"CorreoID\" = {IBD.FormatearGuid(pCorreoID)} AND \"Destinatario\" = {IBD.FormatearGuid(pDestinatario)} AND \"Autor\" = {IBD.FormatearGuid(pAutor)}");

            ActualizarBaseDeDatos(UpdateCorreoEliminado);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pListaIdentidades"></param>
        /// <returns></returns>
        public Dictionary<Guid, string> ObtenerNombresCorreos(List<Guid> pListaIdentidades)
        {
            Dictionary<Guid, string> nombres = new Dictionary<Guid, string>();

            bool esOracle = (ConexionMaster is OracleConnection);

            if (pListaIdentidades.Count > 0)
            {
                string consultaNombres = $"SELECT {IBD.CargarGuid("\"Identidad\".\"IdentidadID\"")}, CASE WHEN \"Perfil\".\"PersonaID\" IS NULL THEN {IBD.CadenaVacia} ELSE \"Perfil\".\"NombrePerfil\" END {IBD.Concatenador} CASE WHEN \"Perfil\".\"PersonaID\" IS NULL OR \"Perfil\".\"OrganizacionID\" IS NULL THEN {IBD.CadenaVacia} ELSE ' @ ' END {IBD.Concatenador} CASE WHEN \"Perfil\".\"NombreOrganizacion\" IS NOT NULL THEN  \"Perfil\".\"NombreOrganizacion\" ELSE {IBD.CadenaVacia} END FROM \"Perfil\" INNER JOIN \"Identidad\" ON \"Identidad\".\"PerfilID\" = \"Perfil\".\"PerfilID\" WHERE \"IdentidadID\" IN(";

                foreach (Guid id in pListaIdentidades)
                {
                    consultaNombres += IBD.FormatearGuid(id) + ",";
                }
                consultaNombres = consultaNombres.Substring(0, consultaNombres.Length - 1);

                consultaNombres += ")";

                // Identidades que han enviado correos al usuario
                DbCommand commandSQLNombres = ObtenerComando(consultaNombres);
                DataSet dataset = new DataSet();
                CargarDataSet(commandSQLNombres, dataset, "Nombres", null, esOracle);

                foreach (DataRow fila in dataset.Tables["nombres"].Rows)
                {
                    nombres.Add(new Guid(fila[0].ToString()), (string)fila[1]);
                }
            }

            return nombres;
        }

        /// <summary>
        /// Elimina correos de la base de datos
        /// </summary>
        /// <param name="pDataSet">Dataset</param>
        public void EliminarBorrados(DataSet pDataSet)
        {
            try
            {
                DataSet deletedDataSet;
                deletedDataSet = pDataSet.GetChanges(DataRowState.Deleted);

                Dictionary<string, DataSet> dasetsActualizar = new Dictionary<string, DataSet>();

                if (deletedDataSet != null && deletedDataSet.Tables["CorreoInterno"] != null)
                {
                    foreach (DataRow fila in deletedDataSet.Tables["CorreoInterno"].Rows)
                    {
                        if ((Guid)fila["Destinatario"] == Guid.Empty)
                        {
                            //Enviado
                            Guid emisor = (Guid)fila["Autor"];
                            string tablaCorreoInterno = $"\"CorreoInterno_{emisor.ToString().Substring(0, 2)}\"";
                            if (dasetsActualizar.ContainsKey(tablaCorreoInterno))
                            {
                                dasetsActualizar[tablaCorreoInterno].Tables["CorreoInterno"].ImportRow(fila);
                            }
                            else
                            {
                                CorreoDS nuevo = new CorreoDS();
                                nuevo.CorreoInterno.ImportRow(fila);
                                dasetsActualizar.Add(tablaCorreoInterno, nuevo);
                            }
                        }
                        else
                        {
                            //Recibido
                            Guid receptor = (Guid)fila["Destinatario"];
                            string tablaCorreoInterno = $"CorreoInterno_{receptor.ToString().Substring(0, 2)}";
                            if (dasetsActualizar.ContainsKey(tablaCorreoInterno))
                            {
                                dasetsActualizar[tablaCorreoInterno].Tables["CorreoInterno"].ImportRow(fila);
                            }
                            else
                            {
                                CorreoDS nuevo = new CorreoDS();
                                nuevo.CorreoInterno.ImportRow(fila);
                                dasetsActualizar.Add(tablaCorreoInterno, nuevo);
                            }
                        }
                    }
                }

                foreach (string tabla in dasetsActualizar.Keys)
                {
                    DataSet dataSetAActualizar = dasetsActualizar[tabla];
                    if (dataSetAActualizar != null)
                    {
                        #region Deleted

                        #region Eliminar tabla CorreoInterno
                        DbCommand DeleteCorreoInternoCommand = ObtenerComando(sqlCorreoInternoDelete.Replace("CorreoInterno", tabla));
                        AgregarParametro(DeleteCorreoInternoCommand, IBD.ToParam("O_CorreoID"), IBD.TipoGuidToObject(DbType.Guid), "CorreoID", DataRowVersion.Original);
                        AgregarParametro(DeleteCorreoInternoCommand, IBD.ToParam("O_Destinatario"), IBD.TipoGuidToObject(DbType.Guid), "Destinatario", DataRowVersion.Original);
                        AgregarParametro(DeleteCorreoInternoCommand, IBD.ToParam("O_Autor"), IBD.TipoGuidToObject(DbType.Guid), "Autor", DataRowVersion.Original);

                        ActualizarBaseDeDatos(dataSetAActualizar, "CorreoInterno", null, null, DeleteCorreoInternoCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                        #endregion

                        #endregion
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Guarda en base de datos los correos añadidos o modificados 
        /// </summary>
        /// <param name="pDataSet">Dataset</param>
        public void GuardarActualizaciones(DataSet pDataSet)
        {
            try
            {
                DataSet addedAndModifiedDataSet;
                addedAndModifiedDataSet = pDataSet.GetChanges(DataRowState.Added | DataRowState.Modified);

                Dictionary<string, DataSet> dasetsActualizar = new Dictionary<string, DataSet>();

                if (addedAndModifiedDataSet != null && addedAndModifiedDataSet.Tables["CorreoInterno"] != null)
                {
                    foreach (DataRow fila in addedAndModifiedDataSet.Tables["CorreoInterno"].Rows)
                    {
                        if ((Guid)fila["Destinatario"] == Guid.Empty)
                        {
                            //Enviado
                            Guid emisor = (Guid)fila["Autor"];
                            string tablaCorreoInterno = $"\"CorreoInterno_{emisor.ToString().Substring(0, 2)}\"";
                            if (dasetsActualizar.ContainsKey(tablaCorreoInterno))
                            {
                                dasetsActualizar[tablaCorreoInterno].Tables["CorreoInterno"].ImportRow(fila);
                            }
                            else
                            {
                                CorreoDS nuevo = new CorreoDS();
                                nuevo.CorreoInterno.ImportRow(fila);
                                dasetsActualizar.Add(tablaCorreoInterno, nuevo);
                            }
                        }
                        else
                        {
                            //Recibido
                            Guid receptor = (Guid)fila["Destinatario"];
                            string tablaCorreoInterno = $"\"CorreoInterno_{receptor.ToString().Substring(0, 2)}\"";
                            if (dasetsActualizar.ContainsKey(tablaCorreoInterno))
                            {
                                dasetsActualizar[tablaCorreoInterno].Tables["CorreoInterno"].ImportRow(fila);
                            }
                            else
                            {
                                CorreoDS nuevo = new CorreoDS();
                                nuevo.CorreoInterno.ImportRow(fila);
                                dasetsActualizar.Add(tablaCorreoInterno, nuevo);
                            }
                        }
                    }
                }

                foreach (string tabla in dasetsActualizar.Keys)
                {
                    DataSet dataSetAActualizar = dasetsActualizar[tabla];

                    if (dataSetAActualizar != null)
                    {
                        #region AddedAndModified
                        #region Actualizar tabla CorreoInterno
                        DbCommand InsertCorreoInternoCommand = ObtenerComando(sqlCorreoInternoInsert.Replace("CorreoInterno", tabla));
                        AgregarParametro(InsertCorreoInternoCommand, IBD.ToParam("CorreoID"), IBD.TipoGuidToObject(DbType.Guid), "CorreoID", DataRowVersion.Current);
                        AgregarParametro(InsertCorreoInternoCommand, IBD.ToParam("Destinatario"), IBD.TipoGuidToObject(DbType.Guid), "Destinatario", DataRowVersion.Current);
                        AgregarParametro(InsertCorreoInternoCommand, IBD.ToParam("Autor"), IBD.TipoGuidToObject(DbType.Guid), "Autor", DataRowVersion.Current);
                        AgregarParametro(InsertCorreoInternoCommand, IBD.ToParam("Asunto"), DbType.String, "Asunto", DataRowVersion.Current);
                        AgregarParametro(InsertCorreoInternoCommand, IBD.ToParam("Cuerpo"), DbType.String, "Cuerpo", DataRowVersion.Current);
                        AgregarParametro(InsertCorreoInternoCommand, IBD.ToParam("Fecha"), DbType.DateTime, "Fecha", DataRowVersion.Current);
                        AgregarParametro(InsertCorreoInternoCommand, IBD.ToParam("Leido"), DbType.Boolean, "Leido", DataRowVersion.Current);
                        AgregarParametro(InsertCorreoInternoCommand, IBD.ToParam("Eliminado"), DbType.Boolean, "Eliminado", DataRowVersion.Current);
                        AgregarParametro(InsertCorreoInternoCommand, IBD.ToParam("EnPapelera"), DbType.Boolean, "EnPapelera", DataRowVersion.Current);
                        AgregarParametro(InsertCorreoInternoCommand, IBD.ToParam("DestinatariosID"), DbType.String, "DestinatariosID", DataRowVersion.Current);
                        AgregarParametro(InsertCorreoInternoCommand, IBD.ToParam("DestinatariosNombres"), DbType.String, "DestinatariosNombres", DataRowVersion.Current);
                        AgregarParametro(InsertCorreoInternoCommand, IBD.ToParam("ConversacionID"), DbType.Guid, "ConversacionID", DataRowVersion.Current);

                        DbCommand ModifyCorreoInternoCommand = ObtenerComando(sqlCorreoInternoModify.Replace("CorreoInterno", tabla));
                        AgregarParametro(ModifyCorreoInternoCommand, IBD.ToParam("O_CorreoID"), IBD.TipoGuidToObject(DbType.Guid), "CorreoID", DataRowVersion.Original);
                        AgregarParametro(ModifyCorreoInternoCommand, IBD.ToParam("O_Destinatario"), IBD.TipoGuidToObject(DbType.Guid), "Destinatario", DataRowVersion.Original);
                        AgregarParametro(ModifyCorreoInternoCommand, IBD.ToParam("O_Autor"), IBD.TipoGuidToObject(DbType.Guid), "Autor", DataRowVersion.Original);
                        AgregarParametro(ModifyCorreoInternoCommand, IBD.ToParam("O_Asunto"), DbType.String, "Asunto", DataRowVersion.Original);
                        AgregarParametro(ModifyCorreoInternoCommand, IBD.ToParam("O_Cuerpo"), DbType.String, "Cuerpo", DataRowVersion.Original);
                        AgregarParametro(ModifyCorreoInternoCommand, IBD.ToParam("O_Fecha"), DbType.DateTime, "Fecha", DataRowVersion.Original);
                        AgregarParametro(ModifyCorreoInternoCommand, IBD.ToParam("O_Leido"), DbType.Boolean, "Leido", DataRowVersion.Original);
                        AgregarParametro(ModifyCorreoInternoCommand, IBD.ToParam("O_Eliminado"), DbType.Boolean, "Eliminado", DataRowVersion.Original);
                        AgregarParametro(ModifyCorreoInternoCommand, IBD.ToParam("O_EnPapelera"), DbType.Boolean, "EnPapelera", DataRowVersion.Original);
                        AgregarParametro(ModifyCorreoInternoCommand, IBD.ToParam("O_DestinatariosID"), DbType.String, "DestinatariosID", DataRowVersion.Original);
                        AgregarParametro(ModifyCorreoInternoCommand, IBD.ToParam("O_DestinatariosNombres"), DbType.String, "DestinatariosNombres", DataRowVersion.Original);
                        AgregarParametro(ModifyCorreoInternoCommand, IBD.ToParam("O_ConversacionID"), DbType.Guid, "ConversacionID", DataRowVersion.Original);

                        AgregarParametro(ModifyCorreoInternoCommand, IBD.ToParam("CorreoID"), IBD.TipoGuidToObject(DbType.Guid), "CorreoID", DataRowVersion.Current);
                        AgregarParametro(ModifyCorreoInternoCommand, IBD.ToParam("Destinatario"), IBD.TipoGuidToObject(DbType.Guid), "Destinatario", DataRowVersion.Current);
                        AgregarParametro(ModifyCorreoInternoCommand, IBD.ToParam("Autor"), IBD.TipoGuidToObject(DbType.Guid), "Autor", DataRowVersion.Current);
                        AgregarParametro(ModifyCorreoInternoCommand, IBD.ToParam("Asunto"), DbType.String, "Asunto", DataRowVersion.Current);
                        AgregarParametro(ModifyCorreoInternoCommand, IBD.ToParam("Cuerpo"), DbType.String, "Cuerpo", DataRowVersion.Current);
                        AgregarParametro(ModifyCorreoInternoCommand, IBD.ToParam("Fecha"), DbType.DateTime, "Fecha", DataRowVersion.Current);
                        AgregarParametro(ModifyCorreoInternoCommand, IBD.ToParam("Leido"), DbType.Boolean, "Leido", DataRowVersion.Current);
                        AgregarParametro(ModifyCorreoInternoCommand, IBD.ToParam("Eliminado"), DbType.Boolean, "Eliminado", DataRowVersion.Current);
                        AgregarParametro(ModifyCorreoInternoCommand, IBD.ToParam("EnPapelera"), DbType.Boolean, "EnPapelera", DataRowVersion.Current);
                        AgregarParametro(ModifyCorreoInternoCommand, IBD.ToParam("DestinatariosID"), DbType.String, "DestinatariosID", DataRowVersion.Current);
                        AgregarParametro(ModifyCorreoInternoCommand, IBD.ToParam("DestinatariosNombres"), DbType.String, "DestinatariosNombres", DataRowVersion.Current);
                        AgregarParametro(ModifyCorreoInternoCommand, IBD.ToParam("ConversacionID"), DbType.Guid, "ConversacionID", DataRowVersion.Current);

                        ActualizarBaseDeDatos(dataSetAActualizar, "CorreoInterno", InsertCorreoInternoCommand, ModifyCorreoInternoCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                        #endregion

                        #endregion
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Obtiene un correo a partir del identificador pasado como parámetro
        /// </summary>
        /// <param name="pCorreoID">Identificador del correo</param>
        /// <param name="pIdentidad"></param>
        /// <param name="pIdentidadOrg"></param>
        /// <returns>Dataset de correo</returns>
        public CorreoDS ObtenerCorreoPorID(Guid pCorreoID, Guid pIdentidad, Guid? pIdentidadOrg)
        {
            bool esOracle = (ConexionMaster is OracleConnection);

            string tablaCorreoInterno = $"CorreoInterno_{pIdentidad.ToString().Substring(0, 2)}";

            CorreoDS correoDS = new CorreoDS();

            string where = " WHERE \"CorreoID\" = " + IBD.FormatearGuid(pCorreoID);

            string consulta = selectCorreoInterno + where;

            if (pIdentidadOrg.HasValue)
            {
                string tablaCorreoInternoOrg = $"CorreoInterno_{pIdentidadOrg.Value.ToString().Substring(0, 2)}";
                string consultaIdentidad = consulta.Replace("CorreoInterno", tablaCorreoInterno);
                string consultaIdentidadOrg = consulta.Replace("CorreoInterno", tablaCorreoInternoOrg);
                consulta = consultaIdentidad + " UNION ALL " + consultaIdentidadOrg;

            }
            else
            {
                consulta = consulta.Replace("CorreoInterno", tablaCorreoInterno);
            }

            DbCommand commandsqlSelectCorreoInternoPorID = ObtenerComando(consulta);
            CargarDataSet(commandsqlSelectCorreoInternoPorID, correoDS, "CorreoInterno", null, esOracle);

            return correoDS;
        }

        /// <summary>
        /// Obtiene un dataset de correo por una lista de IDs
        /// </summary>
        /// <param name="pListaCorreosIDs">Lista de identificadores de correso</param>
        /// <param name="pIdentidad"></param>
        /// <param name="pIdentidadOrg"></param>
        /// <param name="pCorreoDS">DataSet de Correo</param>
        public void ObtenerCorreoPorListaIDs(List<Guid> pListaCorreosIDs, Guid pIdentidad, Guid? pIdentidadOrg, ref CorreoDS pCorreoDS)
        {
            if (pListaCorreosIDs.Count > 0)
            {
                bool esOracle = (ConexionMaster is OracleConnection);
                string tablaCorreoInterno = $"CorreoInterno_{pIdentidad.ToString().Substring(0, 2)}";

                string where = " WHERE \"CorreoID\" IN (";
                foreach (Guid idCorreo in pListaCorreosIDs)
                {
                    where += IBD.FormatearGuid(idCorreo) + ", ";
                }
                where = where.Substring(0, where.Length - 2);
                where += ")";

                string consulta = selectCorreoInterno + where;

                if (pIdentidadOrg.HasValue)
                {
                    string tablaCorreoInternoOrg = $"CorreoInterno_{pIdentidadOrg.Value.ToString().Substring(0, 2)}";
                    string consultaIdentidad = consulta.Replace("CorreoInterno", tablaCorreoInterno);
                    string consultaIdentidadOrg = consulta.Replace("CorreoInterno", tablaCorreoInternoOrg);
                    consulta = consultaIdentidad + " UNION ALL " + consultaIdentidadOrg;

                }
                else
                {
                    consulta = consulta.Replace("CorreoInterno", tablaCorreoInterno);
                }

                DbCommand commandsqlSelectCorreoInternoPorID = ObtenerComando(consulta);
                CargarDataSet(commandsqlSelectCorreoInternoPorID, pCorreoDS, "CorreoInterno", null, esOracle);
            }
        }

        /// <summary>
        /// Obtiene un dataset de correo por una lista de IDs
        /// </summary>
        /// <param name="pListaCorreosIDs">Lista de identificadores de correso</param>
        /// <param name="pIdentidad">Identiad actual</param>
        /// <param name="pTipoBandeja">0 Entrada, 1 Enviados, 2 Eliminados, NULL sin especificar</param>
        /// <returns>DataSet de Correo</returns>
        public CorreoDS ObtenerCorreoPorListaIDs(List<Guid> pListaCorreosIDs, Guid pIdentidad, int? pTipoBandeja)
        {
            CorreoDS correoDS = new CorreoDS();

            if (pListaCorreosIDs.Count > 0)
            {
                bool esOracle = (ConexionMaster is OracleConnection);

                string tablaCorreoInterno = $"CorreoInterno_{pIdentidad.ToString().Substring(0, 2)}";

                string where = " WHERE \"CorreoID\" IN (";
                foreach (Guid idCorreo in pListaCorreosIDs)
                {
                    where += IBD.FormatearGuid(idCorreo) + ", ";
                }
                where = where.Substring(0, where.Length - 2);
                where += ")";

                if (pTipoBandeja.HasValue)
                {
                    if (pTipoBandeja.Value == 0)
                    {
                        where += $" AND \"EnPapelera\" = 0 AND \"Destinatario\" = {IBD.FormatearGuid(pIdentidad)}";
                    }
                    else if (pTipoBandeja.Value == 1)
                    {
                        where += $" AND \"EnPapelera\" = 0 AND \"Destinatario\" = {IBD.FormatearGuid(Guid.Empty)}";
                    }
                    else if (pTipoBandeja.Value == 2)
                    {
                        where += " AND \"EnPapelera\" = 1";
                    }
                }

                string consulta = selectCorreoInterno + where;
                consulta = consulta.Replace("CorreoInterno", tablaCorreoInterno);

                DbCommand commandsqlSelectCorreoInternoPorID = ObtenerComando(consulta);
                CargarDataSet(commandsqlSelectCorreoInternoPorID, correoDS, "CorreoInterno");
            }

            return correoDS;
        }

        /// <summary>
        /// Obtiene el Identificador del correo siguiente de un correo a partir del identificador pasado como parámetro
        /// </summary>
        /// <param name="pCorreoID">Identificador del correo</param>
        /// <param name="pIdentidadID">Identificador de la identidad actual</param>
        /// <param name="pTipoCorreo">Tipo de listado de corro</param>
        /// <returns>Dataset de correo</returns>
        public Guid ObtenerCorreoIDSiguienteDeCorreoPorID(Guid pCorreoID, Guid pIdentidadID, Guid? pIdentidadOrganizacion, TiposListadoCorreo pTipoCorreo)
        {
            string tablaCorreoInterno = $"\"CorreoInterno_{pIdentidadID.ToString().Substring(0, 2)}\"";

            string sqlCorreoInterno = "";

            string sqlFecha = $" SELECT Distinct \"Fecha\" FROM CorreoInterno WHERE \"CorreoID\" = {IBD.FormatearGuid(pCorreoID)} ";
            if (pTipoCorreo.Equals(TiposListadoCorreo.Recibido))
            {
                sqlCorreoInterno = "SELECT CorreoInterno.\"CorreoID\", CorreoInterno.\"Fecha\" FROM CorreoInterno  WHERE CorreoInterno.\"Destinatario\" = " + IBD.FormatearGuid(pIdentidadID) + " AND \"Fecha\" < (FECHA) AND CorreoInterno.\"Eliminado\" = 0 AND CorreoInterno.\"EnPapelera\" = 0 ";
            }
            else if (pTipoCorreo.Equals(TiposListadoCorreo.Enviado))
            {
                sqlCorreoInterno = "SELECT CorreoInterno.\"CorreoID\", CorreoInterno.\"Fecha\" FROM CorreoInterno INNER JOIN CorreoInterno C1 ON C1.\"Autor\" = CorreoInterno.\"Autor\"  WHERE C1.\"CorreoID\" = " + IBD.GuidValor(pCorreoID) + " AND CorreoInterno.\"Fecha\" < C1.\"Fecha\" AND CorreoInterno.\"Eliminado\" = 0 AND CorreoInterno.\"EnPapelera\" = 0 ";
            }
            else if (pTipoCorreo.Equals(TiposListadoCorreo.Eliminado))
            {
                sqlCorreoInterno = "SELECT CorreoInterno.\"CorreoID\", CorreoInterno.\"Fecha\" FROM (";

                sqlCorreoInterno += " SELECT CorreoInterno.\"CorreoID\", CorreoInterno.\"Fecha\" FROM CorreoInterno  WHERE CorreoInterno.\"Destinatario\" = " + IBD.GuidValor(pIdentidadID) + " AND \"Fecha\" < (FECHA) AND CorreoInterno.\"Eliminado\" = 0 AND CorreoInterno.\"EnPapelera\" = 1 ";

                sqlCorreoInterno += " UNION ALL SELECT CorreoInterno.\"CorreoID\", CorreoInterno.\"Fecha\" FROM CorreoInterno INNER JOIN CorreoInterno C1 ON C1.\"Autor\" = CorreoInterno.\"Autor\"  WHERE C1.\"CorreoID\" = " + IBD.GuidValor(pCorreoID) + " AND CorreoInterno.\"Fecha\" < C1.\"Fecha\" AND CorreoInterno.\"Eliminado\" = 0 AND CorreoInterno.\"EnPapelera\" = 1 ";

                sqlCorreoInterno += ") CorreoInterno ";
            }

            string sqlCorreoInternoFin = sqlCorreoInterno.Replace("CorreoInterno", tablaCorreoInterno);

            string sqlFechaFin = sqlFecha.Replace("CorreoInterno", tablaCorreoInterno);

            if (pIdentidadOrganizacion.HasValue)
            {
                string tablaCorreoInternoOrg = $"\"CorreoInterno_{pIdentidadOrganizacion.Value.ToString().Substring(0, 2)}\"";
                string sqlCorreoInternoIdentidadOrg = sqlCorreoInterno.Replace("CorreoInterno", tablaCorreoInternoOrg);
                string sqlFechaOrg = sqlFecha.Replace("CorreoInterno", tablaCorreoInternoOrg);
                sqlCorreoInternoFin = sqlCorreoInternoFin + " UNION " + sqlCorreoInternoIdentidadOrg;
                sqlFechaFin = sqlFechaFin + " UNION " + sqlFechaOrg;
            }

            bool esOracle = (ConexionMaster is OracleConnection);

            if (esOracle)
            {
                sqlCorreoInternoFin = " SELECT CorreoInternoFin.\"CorreoID\" FROM (" + sqlCorreoInternoFin.Replace("FECHA", sqlFechaFin) + ")CorreoInternoFin WHERE ROWNUM < 2 ORDER BY CorreoInternoFin.\"Fecha\" DESC";
            }
            else
            {
                sqlCorreoInternoFin = " SELECT top(1) CorreoInternoFin.\"CorreoID\" FROM (" + sqlCorreoInternoFin.Replace("FECHA", sqlFechaFin) + ") CorreoInternoFin ORDER BY CorreoInternoFin.\"Fecha\" DESC";
            }



            DbCommand commandsqlSelectCorreoInternoPorID = ObtenerComando(sqlCorreoInternoFin);
            object resultado = EjecutarEscalar(commandsqlSelectCorreoInternoPorID, esOracle);

            Guid id = Guid.Empty;
            if (resultado != null && resultado is Guid)
            {
                id = (Guid)resultado;
            }

            return id;
        }

        /// <summary>
        /// Obtiene el Identificador del correo anterior de un correo a partir del identificador pasado como parámetro
        /// </summary>
        /// <param name="pCorreoID">Identificador del correo</param>
        /// <param name="pIdentidadID">Identificador de la identidad actual</param>
        /// <param name="pTipoCorreo">Tipo de listado de corro</param>
        /// <returns>Dataset de correo</returns>
        public Guid ObtenerCorreoIDAnteriorDeCorreoPorID(Guid pCorreoID, Guid pIdentidadID, Guid? pIdentidadOrganizacion, TiposListadoCorreo pTipoCorreo)
        {
            string tablaCorreoInterno = $"\"CorreoInterno_{pIdentidadID.ToString().Substring(0, 2)}\"";

            string sqlCorreoInterno = "";

            string sqlFecha = $" SELECT DISTINCT \"Fecha\" FROM CorreoInterno WHERE \"CorreoID\" = {IBD.FormatearGuid(pCorreoID)}";

            if (pTipoCorreo.Equals(TiposListadoCorreo.Recibido))
            {
                sqlCorreoInterno = $"SELECT CorreoInterno.\"CorreoID\", CorreoInterno.\"Fecha\" FROM CorreoInterno  WHERE CorreoInterno.\"Destinatario\" = {IBD.FormatearGuid(pIdentidadID)} AND \"Fecha\" > (FECHA) AND CorreoInterno.\"Eliminado\" = 0 AND CorreoInterno.\"EnPapelera\" = 0 ";
            }
            else if (pTipoCorreo.Equals(TiposListadoCorreo.Enviado))
            {
                sqlCorreoInterno = $"SELECT CorreoInterno.\"CorreoID\", CorreoInterno.\"Fecha\" FROM CorreoInterno INNER JOIN CorreoInterno C1 ON C1.\"Autor\" = CorreoInterno.\"Autor\"  WHERE C1.\"CorreoID\" = {IBD.FormatearGuid(pCorreoID)} AND CorreoInterno.\"Fecha\" > C1.\"Fecha\" AND CorreoInterno.\"Eliminado\" = 0 AND CorreoInterno.\"EnPapelera\" = 0 ";
            }
            else if (pTipoCorreo.Equals(TiposListadoCorreo.Eliminado))
            {
                sqlCorreoInterno = "SELECT CorreoInterno.\"CorreoID\", CorreoInterno.\"Fecha\" FROM (";

                sqlCorreoInterno += $" SELECT CorreoInterno.\"CorreoID\", CorreoInterno.\"Fecha\" FROM CorreoInterno  WHERE CorreoInterno.\"Destinatario\" = {IBD.FormatearGuid(pIdentidadID)} AND \"Fecha\" > (FECHA) AND CorreoInterno.\"Eliminado\" = 0 AND CorreoInterno.\"EnPapelera\" = 1 ";

                sqlCorreoInterno += $" UNION ALL SELECT CorreoInterno.\"CorreoID\", CorreoInterno.\"Fecha\" FROM CorreoInterno INNER JOIN CorreoInterno C1 ON C1.\"Autor\" = CorreoInterno.\"Autor\"  WHERE C1.\"CorreoID\" = {IBD.GuidValor(pCorreoID)} AND CorreoInterno.\"Fecha\" > C1.\"Fecha\" AND CorreoInterno.\"Eliminado\" = 0 AND CorreoInterno.\"EnPapelera\" = 1 ";

                sqlCorreoInterno += ")CorreoInterno ";
            }

            string sqlCorreoInternoFin = sqlCorreoInterno.Replace("CorreoInterno", tablaCorreoInterno);

            string sqlFechaFin = sqlFecha.Replace("CorreoInterno", tablaCorreoInterno);

            if (pIdentidadOrganizacion.HasValue)
            {
                string tablaCorreoInternoOrg = $"\"CorreoInterno_{pIdentidadOrganizacion.Value.ToString().Substring(0, 2)}\"";
                string sqlCorreoInternoIdentidadOrg = sqlCorreoInterno.Replace("CorreoInterno", tablaCorreoInternoOrg);
                string sqlFechaOrg = sqlFecha.Replace("CorreoInterno", tablaCorreoInternoOrg);
                sqlCorreoInternoFin = sqlCorreoInternoFin + " UNION " + sqlCorreoInternoIdentidadOrg;
                sqlFechaFin = sqlFechaFin + " UNION " + sqlFechaOrg;
            }

            bool esOracle = (ConexionMaster is OracleConnection);

            if (esOracle)
            {
                sqlCorreoInternoFin = $" SELECT \"CorreoInternoFin\".\"CorreoID\" FROM ({sqlCorreoInternoFin.Replace("FECHA", sqlFechaFin)})\"CorreoInternoFin\" WHERE ROWNUM < 2 ORDER BY \"CorreoInternoFin\".\"Fecha\" ASC";
            }
            else
            {
                sqlCorreoInternoFin = $" SELECT top(1) \"CorreoInternoFin\".\"CorreoID\" FROM ({sqlCorreoInternoFin.Replace("FECHA", sqlFechaFin)})\"CorreoInternoFin\" ORDER BY \"CorreoInternoFin\".\"Fecha\" ASC";
            }

            DbCommand commandsqlSelectCorreoInternoPorID = ObtenerComando(sqlCorreoInternoFin);

            object resultado = EjecutarEscalar(commandsqlSelectCorreoInternoPorID, esOracle);

            Guid id = Guid.Empty;
            if (resultado != null && resultado is Guid)
            {
                id = (Guid)resultado;
            }

            return id;
        }

        /// <summary>
        /// Obtiene el número de correos no leídos del usuario pasado por parámetro
        /// </summary>
        /// <param name="pIdentidad">Identificador de identidad de usuario</param>
        /// <param name="pIdentidadOrg">Identificador de la identidad de organización</param>
        /// <param name="pBandejaEliminados">Indica si se debe mostrar los no leídos de la bandeja de entrada o de la bandeja de eliminados</param>
        /// <returns>Número de correos no leídos por el usuario</returns>
        public int ObtenerNumeroCorreosNoLeidos(Guid pIdentidad, Guid? pIdentidadOrg, bool pBandejaEliminados)
        {
            string tablaCorreoInterno = $"\"CorreoInterno_{pIdentidad.ToString().Substring(0, 2)}\"";

            string consulta = $"SELECT CorreoInterno.\"CorreoID\" FROM CorreoInterno WHERE CorreoInterno.\"Eliminado\" = 0 AND CorreoInterno.\"Leido\" = 0 AND (CorreoInterno.\"Destinatario\" ={IBD.FormatearGuid(pIdentidad)}";

            if (pIdentidadOrg.HasValue)
            {
                consulta += $" OR CorreoInterno.\"Destinatario\" = {IBD.FormatearGuid(pIdentidadOrg.Value)}";
            }

            consulta += ")";

            if (pBandejaEliminados)
            {
                consulta += " AND CorreoInterno.\"EnPapelera\" = 1";
            }
            else
            {
                consulta += " AND CorreoInterno.\"EnPapelera\" = 0";
            }

            if (pIdentidadOrg.HasValue)
            {
                string tablaCorreoInternoOrg = $"\"CorreoInterno_{pIdentidadOrg.Value.ToString().Substring(0, 2)}\"";
                consulta = "Select COUNT(*) FROM (" + consulta.Replace("CorreoInterno", tablaCorreoInterno) + " UNION " + consulta.Replace("CorreoInterno", tablaCorreoInternoOrg) + ")\"NoLeidos\"";
            }
            else
            {
                consulta = "Select COUNT(*) FROM (" + consulta.Replace("CorreoInterno", tablaCorreoInterno) + ")\"NoLeidos\"";
            }

            bool esOracle = (ConexionMaster is OracleConnection);

            object resultado;
            DbCommand comandoSQL = ObtenerComando(consulta);
            resultado = EjecutarEscalar(comandoSQL, esOracle);

            if (resultado == null)
                return 0;
            else
                return int.Parse(resultado.ToString());
        }

        /// <summary>
        /// Obtiene una lista con las tablas del dataSet en orden
        /// </summary>
        /// <param name="pDataSet">DataSet del que se quieren ordenar las tablas</param>
        /// <returns>Lista con las tablas del dataSet en orden</returns>
        public List<DataTable> ObtenerOrdenTablas(CorreoDS pCorreoDS)
        {
            List<DataTable> listaTablas = new List<DataTable>();

            listaTablas.Add(pCorreoDS.CorreoInterno);

            return listaTablas;
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

            this.selectCorreoInterno = $"SELECT {IBD.CargarGuid("\"CorreoInterno\".\"CorreoID\"")}, {IBD.CargarGuid("\"CorreoInterno\".\"Destinatario\"")}, {IBD.CargarGuid("\"CorreoInterno\".\"Autor\"")}, \"CorreoInterno\".\"Asunto\", \"CorreoInterno\".\"Cuerpo\", \"CorreoInterno\".\"Fecha\", \"CorreoInterno\".\"Leido\", \"CorreoInterno\".\"Eliminado\",  \"CorreoInterno\".\"EnPapelera\", \"CorreoInterno\".\"DestinatariosID\", \"CorreoInterno\".\"DestinatariosNombres\", {IBD.CargarGuid("\"CorreoInterno\".\"ConversacionID\"")} FROM \"CorreoInterno\"";

            this.sqlSelectCorreoInternoPorAutor = selectCorreoInterno + " WHERE CorreoInterno.Eliminado = 0 AND CorreoInterno.EnPapelera = 0 AND CorreoInterno.Autor =" + IBD.GuidParamValor("autor") + "";

            this.sqlSelectCorreoInternoPorDestinatario = selectCorreoInterno + " WHERE CorreoInterno.Eliminado = 0 AND CorreoInterno.EnPapelera = 0 AND CorreoInterno.Destinatario = " + IBD.GuidParamValor("destinatario");

            this.sqlSelectCorreoInternoPendientePorDestinatario = selectCorreoInterno + " WHERE CorreoInterno.Eliminado = 0 AND CorreoInterno.EnPapelera = 0 AND CorreoInterno.Leido = 0 AND CorreoInterno.Destinatario = " + IBD.GuidParamValor("destinatario");

            // David: Unir los correos recibidos de usuario eliminados y los correos enviados del usuario eliminados
            this.sqlSelectCorreoEnPapeleraDeUsuario = selectCorreoInterno + " WHERE ((CorreoInterno.Autor =" + IBD.GuidParamValor("autor") + "  OR CorreoInterno.Destinatario = " + IBD.GuidParamValor("destinatario") + ") AND CorreoInterno.Eliminado = 0 AND CorreoInterno.EnPapelera = 1)";

            #endregion

            #region DataAdapter

            #region CorreoInterno

            this.sqlCorreoInternoInsert = IBD.ReplaceParam("INSERT INTO CorreoInterno (\"CorreoID\", \"Destinatario\", \"Autor\", \"Asunto\", \"Cuerpo\", \"Fecha\", \"Leido\", \"Eliminado\", \"EnPapelera\", \"DestinatariosID\", \"DestinatariosNombres\", \"ConversacionID\") VALUES (" + IBD.GuidParamColumnaTabla("CorreoID") + "," + IBD.GuidParamColumnaTabla("Destinatario") + ", " + IBD.GuidParamColumnaTabla("Autor") + ", @Asunto, @Cuerpo, @Fecha,@Leido, @Eliminado, @EnPapelera,@DestinatariosID,@DestinatariosNombres,@ConversacionID)");

            this.sqlCorreoInternoDelete = IBD.ReplaceParam("DELETE FROM CorreoInterno WHERE (\"CorreoID\" = " + IBD.GuidParamColumnaTabla("O_CorreoID") + ") AND (\"Autor\" = " + IBD.GuidParamColumnaTabla("O_Autor") + ") AND (\"Destinatario\" = " + IBD.GuidParamColumnaTabla("O_Destinatario") + ")");

            this.sqlCorreoInternoModify = IBD.ReplaceParam("UPDATE CorreoInterno SET CorreoID = " + IBD.GuidParamColumnaTabla("CorreoID") + " , Destinatario=  " + IBD.GuidParamColumnaTabla("Destinatario") + ", Autor = " + IBD.GuidParamColumnaTabla("Autor") + ", Asunto = @Asunto, Cuerpo = @Cuerpo, Fecha = @Fecha,Leido = @Leido, Eliminado = @Eliminado, EnPapelera = @EnPapelera, DestinatariosID=@DestinatariosID, DestinatariosNombres=@DestinatariosNombres, ConversacionID=@ConversacionID WHERE (CorreoID = " + IBD.GuidParamColumnaTabla("O_CorreoID") + ") AND (Autor = " + IBD.GuidParamColumnaTabla("O_Autor") + ") AND (Destinatario = " + IBD.GuidParamColumnaTabla("O_Destinatario") + ") AND (ConversacionID = " + IBD.GuidParamColumnaTabla("O_ConversacionID") + " OR " + IBD.GuidParamColumnaTabla("O_ConversacionID") + " IS NULL AND ConversacionID IS NULL)");



            this.sqlCorreoLeidoUpdate = IBD.ReplaceParam("UPDATE CorreoInterno SET \"Leido\" = 1 WHERE \"CorreoID\" = " + IBD.ToParam("CorreoID") + " AND \"Destinatario\"=" + IBD.ToParam("Destinatario"));

            this.sqlCorreoRestaurarUpdate = IBD.ReplaceParam("UPDATE CorreoInterno SET EnPapelera = 0 WHERE CorreoID = " + IBD.GuidParamColumnaTabla("CorreoID") + " AND Destinatario=" + IBD.GuidParamColumnaTabla("Destinatario") + " AND Autor=" + IBD.GuidParamColumnaTabla("Autor"));

            this.sqlCorreoEliminarDefinitivamenteUpdate = IBD.ReplaceParam("UPDATE CorreoInterno SET Eliminado = 1 WHERE CorreoID = " + IBD.GuidParamColumnaTabla("CorreoID") + " AND Destinatario=" + IBD.GuidParamColumnaTabla("Destinatario") + " AND Autor=" + IBD.GuidParamColumnaTabla("Autor"));

            this.sqlCorreoEliminarUpdate = IBD.ReplaceParam("UPDATE CorreoInterno SET EnPapelera = 1 WHERE CorreoID = " + IBD.GuidParamColumnaTabla("CorreoID") + " AND Destinatario=" + IBD.GuidParamColumnaTabla("Destinatario") + " AND Autor=" + IBD.GuidParamColumnaTabla("Autor"));


            #endregion

            #endregion
        }

        #endregion

        #endregion
    }
}

