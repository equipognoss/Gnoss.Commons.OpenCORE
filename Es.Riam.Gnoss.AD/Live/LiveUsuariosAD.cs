using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.Live.Model;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace Es.Riam.Gnoss.AD.Live
{
    /// <summary>
    /// Enumeración para distinguir los tipos de actividad reciente
    /// </summary>
    public enum TipoActividadReciente
    {
        /// <summary>
        /// Actividad reciente de la home del usuario conectado
        /// </summary>
        HomeUsuario = 0,
        /// <summary>
        /// Actividad reciente de la home de un proyecto
        /// </summary>
        HomeProyecto = 1,
        /// <summary>
        /// Actividad reciente del perfil de persona en un proyecto
        /// </summary>
        PerfilProyecto = 2,
        /// <summary>
        /// Actividad reciente a la que estás suscrito en un proyecto
        /// </summary>
        SuscripcionProyecto = 3,
        /// <summary>
        /// Actividad reciente a la que estás suscrito en un proyecto
        /// </summary>
        Suscripcion = 4,
        /// <summary>
        /// Actividad reciente a la que estás suscrito en un proyecto, si no tienes, mostramos la home del proyecto
        /// </summary>
        SuscripcionSiNoHomeProyecto = 5,
    }

    /// <summary>
    /// DataAdapter de Live
    /// </summary>
    public class LiveUsuariosAD : BaseAD
    {
        #region Miembros

        private string mCaracteresPerfilOProyecto = "";

        public const string PRIVACIDAD_CAMBIADA = "Privacidad cambiada";

        public const string EDITOR_ELIMINADO = "##ID_EDITOR_ELIMINADO##";

        public const string GRUPO_EDITORES_ELIMINADO = "##ID_GRUPO_EDITOR_ELIMINADO##";

        #endregion

        #region DataAdapter

        #region ColaUsuarios

        private string sqlColaUsuariosInsert;
        private string sqlColaUsuariosInsertConColaID;
        private string sqlColaUsuariosDelete;
        private string sqlColaUsuariosModify;

        #endregion

        #region EventoUsuario

        private string sqlEventoUsuarioInsert;
        private string sqlEventoUsuarioDelete;
        private string sqlEventoUsuarioModify;

        #endregion

        #endregion

        #region Consultas

        private string sqlSelectColaUsuarios;
        private string sqlSelectEventoUsuario;

        #endregion

        #region Constructores

        /// <summary>
        /// El por defecto, utilizado cuando se requiere el GnossConfig.xml por defecto
        /// </summary>
        public LiveUsuariosAD(LoggingService loggingService, EntityContext entityContext, ConfigService configService)
            : base("base", loggingService, entityContext, configService)
        {
            this.CargarConsultasYDataAdapters();
        }

        #endregion

        #region Metodos generales

        #region Métodos AD

        /// <summary>
        /// Actualiza la base de datos
        /// </summary>
        /// <param name="pDataSet">Data set con las modificaciones</param>
        public void ActualizarBD(DataSet pDataSet, string pNombreTablaCola)
        {
            EliminarBorrados(pDataSet, pNombreTablaCola);
            GuardarActualizaciones(pDataSet, pNombreTablaCola);
        }

        /// <summary>
        /// Actualiza la base de datos eliminando los elementos borrados
        /// </summary>
        /// <param name="pDataSet">Data set con los elementos borrados</param>
        public void EliminarBorrados(DataSet pDataSet, string pNombreTablaCola)
        {
            try
            {
                DataSet deletedDataSet = pDataSet.GetChanges(DataRowState.Deleted);

                if (deletedDataSet != null)
                {
                    #region Deleted

                    #region Eliminar tabla EventoUsuario

                    if (mCaracteresPerfilOProyecto != "")
                    {
                        DbCommand DeleteEventoUsuarioCommand = ObtenerComando(sqlEventoUsuarioDelete);
                        AgregarParametro(DeleteEventoUsuarioCommand, IBD.ToParam("Original_ElementoID"), IBD.TipoGuidToObject(DbType.Guid), "ElementoID", DataRowVersion.Original);
                        AgregarParametro(DeleteEventoUsuarioCommand, IBD.ToParam("Original_Tipo"), DbType.Int16, "Tipo", DataRowVersion.Original);
                        AgregarParametro(DeleteEventoUsuarioCommand, IBD.ToParam("Original_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);
                        AgregarParametro(DeleteEventoUsuarioCommand, IBD.ToParam("Original_PerfilEventoID"), IBD.TipoGuidToObject(DbType.Guid), "PerfilEventoID", DataRowVersion.Original);
                        AgregarParametro(DeleteEventoUsuarioCommand, IBD.ToParam("Original_Accion"), DbType.Int16, "Accion", DataRowVersion.Original);
                        AgregarParametro(DeleteEventoUsuarioCommand, IBD.ToParam("Original_Fecha"), DbType.DateTime, "Fecha", DataRowVersion.Original);
                        AgregarParametro(DeleteEventoUsuarioCommand, IBD.ToParam("Original_Eliminado"), DbType.Boolean, "Eliminado", DataRowVersion.Original);
                        AgregarParametro(DeleteEventoUsuarioCommand, IBD.ToParam("Original_Datos"), DbType.String, "Datos", DataRowVersion.Original);

                        ActualizarBaseDeDatos(deletedDataSet, "EventoUsuario", null, null, DeleteEventoUsuarioCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);
                    }

                    #endregion

                    #region Eliminar tabla ColaUsuarios

                    string comandoDeleteCola = sqlColaUsuariosDelete;

                    if (!string.IsNullOrEmpty(pNombreTablaCola))
                    {
                        comandoDeleteCola = comandoDeleteCola.Replace("ColaUsuarios", pNombreTablaCola);
                    }

                    DbCommand DeleteColaUsuariosCommand = ObtenerComando(comandoDeleteCola);
                    AgregarParametro(DeleteColaUsuariosCommand, IBD.ToParam("O_ColaId"), DbType.Int32, "ColaId", DataRowVersion.Original);
                    AgregarParametro(DeleteColaUsuariosCommand, IBD.ToParam("O_ProyectoId"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoId", DataRowVersion.Original);
                    AgregarParametro(DeleteColaUsuariosCommand, IBD.ToParam("O_Id"), IBD.TipoGuidToObject(DbType.Guid), "Id", DataRowVersion.Original);
                    AgregarParametro(DeleteColaUsuariosCommand, IBD.ToParam("O_Tipo"), DbType.Int16, "Tipo", DataRowVersion.Original);
                    AgregarParametro(DeleteColaUsuariosCommand, IBD.ToParam("O_Accion"), DbType.Int16, "Accion", DataRowVersion.Original);

                    ActualizarBaseDeDatos(deletedDataSet, "ColaUsuarios", null, null, DeleteColaUsuariosCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

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
        /// Guarda las actualizaciones llevadas a cabo
        /// </summary>
        /// <param name="pDataSet">Data set con las actualizaciones realizadas</param>
        public void GuardarActualizaciones(DataSet pDataSet, string pNombreTablaCola)
        {
            try
            {
                DataSet addedAndModifiedDataSet = pDataSet.GetChanges(DataRowState.Added | DataRowState.Modified);

                if (addedAndModifiedDataSet != null)
                {
                    #region AddedAndModified



                    #region Actualizar tabla ColaUsuarios

                    string comandoInsertCola = sqlColaUsuariosInsert;
                    string comandoUpdateCola = sqlColaUsuariosModify;

                    if (!string.IsNullOrEmpty(pNombreTablaCola))
                    {
                        comandoUpdateCola = sqlColaUsuariosModify;
                        comandoInsertCola = comandoInsertCola.Replace("ColaUsuarios", pNombreTablaCola);
                        comandoUpdateCola = comandoUpdateCola.Replace("ColaUsuarios", pNombreTablaCola);
                    }

                    DbCommand InsertColaUsuariosCommand = ObtenerComando(comandoInsertCola);
                    AgregarParametro(InsertColaUsuariosCommand, IBD.ToParam("ProyectoId"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoId", DataRowVersion.Current);
                    AgregarParametro(InsertColaUsuariosCommand, IBD.ToParam("Id"), IBD.TipoGuidToObject(DbType.Guid), "Id", DataRowVersion.Current);
                    AgregarParametro(InsertColaUsuariosCommand, IBD.ToParam("Accion"), DbType.Int32, "Accion", DataRowVersion.Current);
                    AgregarParametro(InsertColaUsuariosCommand, IBD.ToParam("Tipo"), DbType.Int32, "Tipo", DataRowVersion.Current);
                    AgregarParametro(InsertColaUsuariosCommand, IBD.ToParam("NumIntentos"), DbType.Int32, "NumIntentos", DataRowVersion.Current);
                    AgregarParametro(InsertColaUsuariosCommand, IBD.ToParam("Fecha"), DbType.DateTime, "Fecha", DataRowVersion.Current);
                    AgregarParametro(InsertColaUsuariosCommand, IBD.ToParam("Prioridad"), DbType.Int16, "Prioridad", DataRowVersion.Current);
                    AgregarParametro(InsertColaUsuariosCommand, IBD.ToParam("InfoExtra"), DbType.String, "InfoExtra", DataRowVersion.Current);

                    DbCommand ModifyColaUsuariosCommand = ObtenerComando(comandoUpdateCola);
                    AgregarParametro(ModifyColaUsuariosCommand, IBD.ToParam("O_ColaId"), DbType.Int32, "ColaId", DataRowVersion.Original);

                    AgregarParametro(ModifyColaUsuariosCommand, IBD.ToParam("ProyectoId"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoId", DataRowVersion.Current);
                    AgregarParametro(ModifyColaUsuariosCommand, IBD.ToParam("Id"), IBD.TipoGuidToObject(DbType.Guid), "Id", DataRowVersion.Current);
                    AgregarParametro(ModifyColaUsuariosCommand, IBD.ToParam("Accion"), DbType.Int32, "Accion", DataRowVersion.Current);
                    AgregarParametro(ModifyColaUsuariosCommand, IBD.ToParam("Tipo"), DbType.Int32, "Tipo", DataRowVersion.Current);
                    AgregarParametro(ModifyColaUsuariosCommand, IBD.ToParam("NumIntentos"), DbType.Int32, "NumIntentos", DataRowVersion.Current);
                    AgregarParametro(ModifyColaUsuariosCommand, IBD.ToParam("Fecha"), DbType.DateTime, "Fecha", DataRowVersion.Current);
                    AgregarParametro(ModifyColaUsuariosCommand, IBD.ToParam("Prioridad"), DbType.Int16, "Prioridad", DataRowVersion.Current);
                    AgregarParametro(ModifyColaUsuariosCommand, IBD.ToParam("InfoExtra"), DbType.String, "InfoExtra", DataRowVersion.Current);


                    ActualizarBaseDeDatos(addedAndModifiedDataSet, "ColaUsuarios", InsertColaUsuariosCommand, ModifyColaUsuariosCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #region Actualizar tabla EventoUsuario

                    if (mCaracteresPerfilOProyecto != "")
                    {
                        DbCommand InsertEventoUsuarioCommand = ObtenerComando(sqlEventoUsuarioInsert);
                        AgregarParametro(InsertEventoUsuarioCommand, IBD.ToParam("ElementoID"), IBD.TipoGuidToObject(DbType.Guid), "ElementoID", DataRowVersion.Current);
                        AgregarParametro(InsertEventoUsuarioCommand, IBD.ToParam("Tipo"), DbType.Int16, "Tipo", DataRowVersion.Current);
                        AgregarParametro(InsertEventoUsuarioCommand, IBD.ToParam("ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Current);
                        AgregarParametro(InsertEventoUsuarioCommand, IBD.ToParam("PerfilEventoID"), IBD.TipoGuidToObject(DbType.Guid), "PerfilEventoID", DataRowVersion.Current);
                        AgregarParametro(InsertEventoUsuarioCommand, IBD.ToParam("Accion"), DbType.Int16, "Accion", DataRowVersion.Current);
                        AgregarParametro(InsertEventoUsuarioCommand, IBD.ToParam("Fecha"), DbType.DateTime, "Fecha", DataRowVersion.Current);
                        AgregarParametro(InsertEventoUsuarioCommand, IBD.ToParam("Eliminado"), DbType.Boolean, "Eliminado", DataRowVersion.Current);
                        AgregarParametro(InsertEventoUsuarioCommand, IBD.ToParam("Datos"), DbType.String, "Datos", DataRowVersion.Current);


                        DbCommand ModifyEventoUsuarioCommand = ObtenerComando(sqlEventoUsuarioModify);
                        AgregarParametro(ModifyEventoUsuarioCommand, IBD.ToParam("Original_ElementoID"), IBD.TipoGuidToObject(DbType.Guid), "ElementoID", DataRowVersion.Original);
                        AgregarParametro(ModifyEventoUsuarioCommand, IBD.ToParam("Original_Tipo"), DbType.Int16, "Tipo", DataRowVersion.Original);
                        AgregarParametro(ModifyEventoUsuarioCommand, IBD.ToParam("Original_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);
                        AgregarParametro(ModifyEventoUsuarioCommand, IBD.ToParam("Original_PerfilEventoID"), IBD.TipoGuidToObject(DbType.Guid), "PerfilEventoID", DataRowVersion.Original);
                        AgregarParametro(ModifyEventoUsuarioCommand, IBD.ToParam("Original_Accion"), DbType.Int16, "Accion", DataRowVersion.Original);
                        AgregarParametro(ModifyEventoUsuarioCommand, IBD.ToParam("Original_Fecha"), DbType.DateTime, "Fecha", DataRowVersion.Original);
                        AgregarParametro(ModifyEventoUsuarioCommand, IBD.ToParam("Original_Eliminado"), DbType.Boolean, "Eliminado", DataRowVersion.Original);
                        AgregarParametro(ModifyEventoUsuarioCommand, IBD.ToParam("Original_Datos"), DbType.String, "Datos", DataRowVersion.Original);


                        AgregarParametro(ModifyEventoUsuarioCommand, IBD.ToParam("ElementoID"), IBD.TipoGuidToObject(DbType.Guid), "ElementoID", DataRowVersion.Current);
                        AgregarParametro(ModifyEventoUsuarioCommand, IBD.ToParam("Tipo"), DbType.Int16, "Tipo", DataRowVersion.Current);
                        AgregarParametro(ModifyEventoUsuarioCommand, IBD.ToParam("ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Current);
                        AgregarParametro(ModifyEventoUsuarioCommand, IBD.ToParam("PerfilEventoID"), IBD.TipoGuidToObject(DbType.Guid), "PerfilEventoID", DataRowVersion.Current);
                        AgregarParametro(ModifyEventoUsuarioCommand, IBD.ToParam("Accion"), DbType.Int16, "Accion", DataRowVersion.Current);
                        AgregarParametro(ModifyEventoUsuarioCommand, IBD.ToParam("Eliminado"), DbType.Boolean, "Eliminado", DataRowVersion.Current);
                        AgregarParametro(ModifyEventoUsuarioCommand, IBD.ToParam("Datos"), DbType.String, "Datos", DataRowVersion.Current);
                        AgregarParametro(ModifyEventoUsuarioCommand, IBD.ToParam("Fecha"), DbType.DateTime, "Fecha", DataRowVersion.Current);

                        ActualizarBaseDeDatos(addedAndModifiedDataSet, "EventoUsuario", InsertEventoUsuarioCommand, ModifyEventoUsuarioCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);
                    }

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
        /// En caso de que se utilice un GnossConfig.xml que no es el de por defecto se pasa un objeto IBaseDatos creado con respecto
        /// al fichero de configuracion que se ha apsado como parámetro
        /// </summary>
        /// <param name="IBD">Objecto IBaseDatos para el archivo pasado al constructor del AD</param>
        private void CargarConsultasYDataAdapters()
        {
            #region DataAdapter

            #region ColaUsuarios

            this.sqlColaUsuariosInsert = IBD.ReplaceParam("INSERT INTO ColaUsuarios (ProyectoId, Id, Accion, Tipo, NumIntentos, Fecha,Prioridad, InfoExtra) VALUES (" + IBD.GuidParamColumnaTabla("ProyectoId") + ", " + IBD.GuidParamColumnaTabla("Id") + ", @Accion, @Tipo, @NumIntentos, @Fecha, @Prioridad, @InfoExtra)");

            sqlColaUsuariosInsertConColaID = IBD.ReplaceParam("INSERT INTO ColaUsuarios (ColaID, ProyectoId, Id, Accion, Tipo, NumIntentos, Fecha,Prioridad, InfoExtra) VALUES (@ColaID, " + IBD.GuidParamColumnaTabla("ProyectoId") + ", " + IBD.GuidParamColumnaTabla("Id") + ", @Accion, @Tipo, @NumIntentos, @Fecha, @Prioridad, @InfoExtra)");

            this.sqlColaUsuariosDelete = IBD.ReplaceParam("DELETE FROM ColaUsuarios WHERE (ColaId = @O_ColaId AND ProyectoId = @O_ProyectoId AND Id = @O_Id AND Tipo = @O_Tipo AND Accion = @O_Accion)");

            this.sqlColaUsuariosModify = IBD.ReplaceParam("UPDATE ColaUsuarios SET ProyectoId = " + IBD.GuidParamColumnaTabla("ProyectoId") + ", Id = " + IBD.GuidParamColumnaTabla("Id") + ", Accion = @Accion, Tipo = @Tipo, NumIntentos = @NumIntentos, Fecha = @Fecha, Prioridad = @Prioridad, InfoExtra = @InfoExtra WHERE (ColaId = @O_ColaId AND ProyectoId = @ProyectoId AND Id = @Id AND Tipo = @Tipo AND Accion = @Accion)");

            #endregion

            #region EventoUsuario

            this.sqlEventoUsuarioInsert = IBD.ReplaceParam("INSERT INTO EventoUsuario_" + mCaracteresPerfilOProyecto + " (ElementoID, Tipo, ProyectoID, PerfilEventoID, Accion, Fecha, Eliminado, Datos) VALUES (" + IBD.GuidParamColumnaTabla("ElementoID") + ", @Tipo, " + IBD.GuidParamColumnaTabla("ProyectoID") + ", " + IBD.GuidParamColumnaTabla("PerfilEventoID") + ", @Accion, @Fecha, @Eliminado, @Datos)");

            this.sqlEventoUsuarioDelete = IBD.ReplaceParam("DELETE FROM EventoUsuario_" + mCaracteresPerfilOProyecto + " WHERE (ElementoID = " + IBD.GuidParamColumnaTabla("Original_ElementoID") + ") AND (Tipo = @Original_Tipo) AND (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ") AND (PerfilEventoID = " + IBD.GuidParamColumnaTabla("Original_PerfilEventoID") + ") AND (Accion = @Original_Accion) AND (Fecha = @Original_Fecha) AND (Eliminado = @Original_Eliminado) AND (Datos = @Original_Datos OR @Original_Datos IS NULL AND Datos IS NULL)");

            this.sqlEventoUsuarioModify = IBD.ReplaceParam("UPDATE EventoUsuario_" + mCaracteresPerfilOProyecto + " SET ElementoID = " + IBD.GuidParamColumnaTabla("ElementoID") + ", Tipo = @Tipo, ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + ", PerfilEventoID = " + IBD.GuidParamColumnaTabla("PerfilEventoID") + ", Accion = @Accion, Fecha = @Fecha, Eliminado = @Eliminado, Datos = @Datos WHERE (ElementoID = " + IBD.GuidParamColumnaTabla("Original_ElementoID") + ") AND (Tipo = @Original_Tipo) AND (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ") AND (PerfilEventoID = " + IBD.GuidParamColumnaTabla("Original_PerfilEventoID") + ") AND (Accion = @Original_Accion) AND (Fecha = @Original_Fecha) AND (Eliminado = @Original_Eliminado) AND (Datos = @Original_Datos OR @Original_Datos IS NULL AND Datos IS NULL)");

            #endregion

            #endregion

            #region Consultas

            sqlSelectColaUsuarios = "SELECT ColaUsuarios.ColaId, " + IBD.CargarGuid("ColaUsuarios.ProyectoId") + ", " + IBD.CargarGuid("ColaUsuarios.Id") + ", ColaUsuarios.Accion, ColaUsuarios.Tipo, ColaUsuarios.NumIntentos, ColaUsuarios.Fecha, ColaUsuarios.Prioridad, ColaUsuarios.InfoExtra FROM ColaUsuarios";

            sqlSelectEventoUsuario = "SELECT " + IBD.CargarGuid("EventoUsuario.ElementoID") + ", EventoUsuario.Tipo, " + IBD.CargarGuid("EventoUsuario.ProyectoID") + ", " + IBD.CargarGuid("EventoUsuario.PerfilEventoID") + ", EventoUsuario.Accion, EventoUsuario.Titulo, EventoUsuario.Fecha, EventoUsuario.Eliminado, EventoUsuario.Datos FROM EventoUsuario";

            #endregion
        }


        /// <summary>
        /// Obtiene los eventos de la home del perfil que hay en la cola
        /// </summary>
        /// <returns>Data set con los eventos a procesar</returns>
        public LiveUsuariosDS ObtenerCola(string pNombreTabla)
        {
            LiveUsuariosDS liveUsuariosDS = new LiveUsuariosDS();

            string selectCola = sqlSelectColaUsuarios.Replace("SELECT", "SELECT DISTINCT TOP 10") + " WHERE NumIntentos < 6 ORDER BY ColaId, Prioridad, InfoExtra";

            if (!string.IsNullOrEmpty(pNombreTabla))
            {
                selectCola = selectCola.Replace("ColaUsuarios", pNombreTabla);
            }

            DbCommand commandsqlSelectCola = ObtenerComando(selectCola);
            CargarDataSet(commandsqlSelectCola, liveUsuariosDS, "ColaUsuarios");

            return liveUsuariosDS;
        }

        public LiveUsuariosDS ObtenerColaUsuariosEspecificoSinEventosPorProyecto(string pNombreTabla, List<Guid> pProyectosID)
        {
            LiveUsuariosDS liveUsuariosDS = new LiveUsuariosDS();

            string selectCola = sqlSelectColaUsuarios.Replace("SELECT", "SELECT DISTINCT TOP 10") + " WHERE NumIntentos < 6 ";

            if (pProyectosID.Count > 0)
            {
                selectCola += " AND ColaID NOT IN ( SELECT DISTINCT ColaID From ColaUSuarios  WHERE NumIntentos < 6 ";

                selectCola += " AND ProyectoID IN (";
                foreach (Guid proyectoID in pProyectosID)
                {
                    selectCola += "'" + proyectoID + "',";
                }
                if (selectCola.EndsWith(","))
                {
                    selectCola = selectCola.Substring(0, selectCola.Length - 1);
                }

                selectCola += ") AND Tipo = 4 AND Accion = 0 )";
            }

            selectCola += " ORDER BY ColaId, Prioridad, InfoExtra";

            if (!string.IsNullOrEmpty(pNombreTabla))
            {
                selectCola = selectCola.Replace("ColaUsuarios", pNombreTabla);
            }

            DbCommand commandsqlSelectCola = ObtenerComando(selectCola);
            CargarDataSet(commandsqlSelectCola, liveUsuariosDS, "ColaUsuarios");

            return liveUsuariosDS;
        }

        public LiveUsuariosDS ObtenerColaUsuariosEspecificoConEventosPorProyecto(string pNombreTabla, List<Guid> pProyectosID)
        {
            LiveUsuariosDS liveUsuariosDS = new LiveUsuariosDS();

            string selectCola = ObtenerQueryAgruparNuevosRegistrosListaProyectos(pProyectosID);
            selectCola += " ORDER BY ColaId, Prioridad, InfoExtra";

            if (!string.IsNullOrEmpty(pNombreTabla))
            {
                selectCola = selectCola.Replace("ColaUsuarios", pNombreTabla);
            }

            DbCommand commandsqlSelectCola = ObtenerComando(selectCola);
            CargarDataSet(commandsqlSelectCola, liveUsuariosDS, "ColaUsuarios");

            return liveUsuariosDS;
        }

        public string ObtenerQueryAgruparNuevosRegistrosListaProyectos(List<Guid> pProyectosID)
        {
            string selectCola = sqlSelectColaUsuarios.Replace("SELECT", "SELECT DISTINCT TOP 10") + " WHERE NumIntentos < 6 ";

            selectCola += " AND ProyectoID IN (";
            foreach (Guid proyectoID in pProyectosID)
            {
                selectCola += "'" + proyectoID + "',";
            }
            if (selectCola.EndsWith(","))
            {
                selectCola = selectCola.Substring(0, selectCola.Length - 1);
            }

            selectCola += ") AND Tipo = 4 AND Accion = 0 ";

            return selectCola;
        }

        /// <summary>
        /// Obtiene los eventos de la home del perfil que hay en la cola
        /// </summary>
        /// <returns>Data set con los eventos a procesar</returns>
        public LiveUsuariosDS ObtenerColaProcesados(string pNombreTabla)
        {
            LiveUsuariosDS liveUsuariosDS = new LiveUsuariosDS();

            string selectCola = sqlSelectColaUsuarios.Replace("SELECT", "SELECT DISTINCT TOP 10") + " WHERE NumIntentos = 7 ORDER BY ColaId, Prioridad, InfoExtra";

            if (!string.IsNullOrEmpty(pNombreTabla))
            {
                selectCola = selectCola.Replace("ColaUsuarios", pNombreTabla);
            }

            DbCommand commandsqlSelectCola = ObtenerComando(selectCola);
            CargarDataSet(commandsqlSelectCola, liveUsuariosDS, "ColaUsuarios");

            return liveUsuariosDS;
        }

        /// <summary>
        /// Inserta una fila del LIVE en la cola de Usuarios
        /// </summary>
        /// <param name="pFila">Fila de una de las colas del LIVE</param>
        /// <param name="pCaracteresUsuario">Primeros dos caracteres del id de los usuarios que están afectados por esta fila</param>
        public void InsertarFilaLiveEnColaUsuariosPorNombre(List<LiveUsuariosDS.ColaUsuariosRow> pFilas, string pNombreTabla)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO " + pNombreTabla + "(ColaID, ProyectoId, Id, Accion, Tipo, NumIntentos, Fecha, Prioridad, InfoExtra) VALUES ");
            string coma = "";
            DbCommand insertColaUsuariosCommand = ObtenerComando(sb.ToString());
            int i = 1;

            foreach (LiveUsuariosDS.ColaUsuariosRow fila in pFilas)
            {
                Guid proyID = fila.ProyectoId;
                Guid docID = fila.Id;
                sb.Append(coma + "(" + IBD.ToParam("ColaID" + i) + ", " + IBD.GuidValor(proyID) + ", " + IBD.GuidValor(docID) + ", " + IBD.ToParam("Accion" + i) + ", " + IBD.ToParam("Tipo" + i) + ", " + IBD.ToParam("NumIntentos" + i) + ", " + IBD.ToParam("Fecha" + i) + ", " + IBD.ToParam("Prioridad" + i) + ", " + IBD.ToParam("InfoExtra" + i) + ")");
                coma = ", ";

                AgregarParametro(insertColaUsuariosCommand, IBD.ToParam("ColaID" + i), DbType.Int32, fila["ColaID"]);
                AgregarParametro(insertColaUsuariosCommand, IBD.ToParam("Accion" + i), DbType.Int32, fila["Accion"]);
                AgregarParametro(insertColaUsuariosCommand, IBD.ToParam("Tipo" + i), DbType.Int32, fila["Tipo"]);
                AgregarParametro(insertColaUsuariosCommand, IBD.ToParam("NumIntentos" + i), DbType.Int32, 0);
                AgregarParametro(insertColaUsuariosCommand, IBD.ToParam("Fecha" + i), DbType.DateTime, fila["Fecha"]);
                AgregarParametro(insertColaUsuariosCommand, IBD.ToParam("Prioridad" + i), DbType.Int16, fila["Prioridad"]);
                AgregarParametro(insertColaUsuariosCommand, IBD.ToParam("InfoExtra" + i), DbType.String, fila["InfoExtra"]);
                i++;
            }

            insertColaUsuariosCommand.CommandText = sb.ToString();
            ActualizarBaseDeDatos(insertColaUsuariosCommand);

            //if (!esColaGeneral)
            //{
            //    //Si no es la cola general, insertamos el id en las tablas distribuidas para saber cuál es su origen
            //    AgregarParametro(InsertColaUsuariosCommand, IBD.ToParam("ColaId"), DbType.Int32, pFila["ColaId"]);
            //}

            //AgregarParametro(InsertColaUsuariosCommand, IBD.ToParam("ProyectoId"), IBD.TipoGuidToObject(DbType.Guid), pFila["ProyectoId"]);
            //AgregarParametro(InsertColaUsuariosCommand, IBD.ToParam("Id"), IBD.TipoGuidToObject(DbType.Guid), pFila["Id"]);
            //AgregarParametro(InsertColaUsuariosCommand, IBD.ToParam("Accion"), DbType.Int32, pFila["Accion"]);
            //AgregarParametro(InsertColaUsuariosCommand, IBD.ToParam("Tipo"), DbType.Int32, pFila["Tipo"]);
            //AgregarParametro(InsertColaUsuariosCommand, IBD.ToParam("NumIntentos"), DbType.Int32, 0);
            //AgregarParametro(InsertColaUsuariosCommand, IBD.ToParam("Fecha"), DbType.DateTime, pFila["Fecha"]);
            //AgregarParametro(InsertColaUsuariosCommand, IBD.ToParam("Prioridad"), DbType.Int16, pFila["Prioridad"]);
            //AgregarParametro(InsertColaUsuariosCommand, IBD.ToParam("InfoExtra"), DbType.String, pFila["InfoExtra"]);

            //ActualizarBaseDeDatos(InsertColaUsuariosCommand);
        }

        /// <summary>
        /// Inserta una fila del LIVE en la cola de Usuarios
        /// </summary>
        /// <param name="pFila">Fila de una de las colas del LIVE</param>
        /// <param name="pCaracteresUsuario">Primeros dos caracteres del id de los usuarios que están afectados por esta fila</param>
        public void InsertarFilaLiveEnColaUsuariosPorNombre(DataRow pFila, string pNombreTabla)
        {
            string consulta = sqlColaUsuariosInsert;

            bool esColaGeneral = !pNombreTabla.Equals("ColaUsuariosHTML");

            if (!esColaGeneral)
            {
                consulta = sqlColaUsuariosInsertConColaID;
            }

            if (!string.IsNullOrEmpty(pNombreTabla))
            {
                consulta = consulta.Replace("ColaUsuarios", pNombreTabla);
            }

            DbCommand InsertColaUsuariosCommand = ObtenerComando(consulta);

            if (!esColaGeneral)
            {
                //Si no es la cola general, insertamos el id en las tablas distribuidas para saber cuál es su origen
                AgregarParametro(InsertColaUsuariosCommand, IBD.ToParam("ColaId"), DbType.Int32, pFila["ColaId"]);
            }

            AgregarParametro(InsertColaUsuariosCommand, IBD.ToParam("ProyectoId"), IBD.TipoGuidToObject(DbType.Guid), pFila["ProyectoId"]);
            AgregarParametro(InsertColaUsuariosCommand, IBD.ToParam("Id"), IBD.TipoGuidToObject(DbType.Guid), pFila["Id"]);
            AgregarParametro(InsertColaUsuariosCommand, IBD.ToParam("Accion"), DbType.Int32, pFila["Accion"]);
            AgregarParametro(InsertColaUsuariosCommand, IBD.ToParam("Tipo"), DbType.Int32, pFila["Tipo"]);
            AgregarParametro(InsertColaUsuariosCommand, IBD.ToParam("NumIntentos"), DbType.Int32, 0);
            AgregarParametro(InsertColaUsuariosCommand, IBD.ToParam("Fecha"), DbType.DateTime, pFila["Fecha"]);
            AgregarParametro(InsertColaUsuariosCommand, IBD.ToParam("Prioridad"), DbType.Int16, pFila["Prioridad"]);
            AgregarParametro(InsertColaUsuariosCommand, IBD.ToParam("InfoExtra"), DbType.String, pFila["InfoExtra"]);

            ActualizarBaseDeDatos(InsertColaUsuariosCommand);
        }

        #endregion

        #endregion
    }
}
