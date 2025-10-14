using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.BASE_BD.Model;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.RabbitMQ;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Es.Riam.Gnoss.AD.BASE_BD
{
    /// <summary>
    /// Clase base para los AD del modelo BASE
    /// </summary>
    public class ReplicacionAD : BaseAD
    {
        public const string COLA_REPLICACION_MASTER = IServicesUtilVirtuosoAndReplication.COLA_REPLICACION_MASTER;
        public const string COLA_REPLICACION_MASTER_HOME = IServicesUtilVirtuosoAndReplication.COLA_REPLICACION_MASTER_HOME;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        #region Constructores

        /// <summary>
        /// El por defecto
        /// </summary>
        public ReplicacionAD(LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<ReplicacionAD> logger, ILoggerFactory loggerFactory)
            : base("colasReplicacion", loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication,logger,loggerFactory)
        {
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        #endregion

        #region Metodos generales

        /// <summary>
        /// Se inserta la query en la cola de replicación pasada como parámetro
        /// </summary>
        /// <param name="pQuery">Query que se va a ejecutar</param>
        /// <param name="pPrioridad">Prioridad que tiene la query</param>
        /// <param name="pTablaReplicacion">Tabla donde se va a replicar la query.</param>
        /// <param name="pInfoExtra">Información extra para el procesado por el base.</param>
        public void InsertarEnReplicacion(string pQuery, string pTablaReplicacion, bool pUsarHttpPost = true)
        {
            List<string> listaQuerys = new List<string>();
            listaQuerys.Add(pQuery);

            InsertarEnReplicacion(listaQuerys, pTablaReplicacion, pUsarHttpPost);
        }

        /// <summary>
        /// Se inserta la query en la cola de replicación pasada como parámetro
        /// </summary>
        /// <param name="pQuery">Query que se va a ejecutar</param>
        /// <param name="pPrioridad">Prioridad que tiene la query</param>
        /// <param name="pTablaReplicacion">Tabla donde se va a replicar la query.</param>
        /// <param name="pInfoExtra">Información extra para el procesado por el base.</param>
        public void InsertarEnReplicacion(List<string> pListaQuerys, string pTablaReplicacion, bool pUsarHttpPost = true)
        {
            mServicesUtilVirtuosoAndReplication.InsertarEnReplicacion(pListaQuerys, pTablaReplicacion, pUsarHttpPost);
        }

        /// <summary>
        /// Se inserta la query en la cola de replicación por defecto.
        /// </summary>
        /// <param name="pQuery">Query que se va a ejecutar</param>
        /// <param name="pPrioridad">Prioridad que tiene la query</param>
        /// <param name="pInfoExtra">Información extra para el procesado por el base.</param>
        public void InsertarConsultaEnColaReplicacion(string pQuery, int pPrioridad, bool pUsarHttpPost = true, string pInfoExtra = null)
        {
            InsertarConsultaEnColaReplicacion(pQuery, pPrioridad, "ColaReplicacionMaster", pInfoExtra, pUsarHttpPost);
        }

        /// <summary>
        /// Se inserta la query en la cola de replicación pasada como parámetro
        /// </summary>
        /// <param name="pQuery">Query que se va a ejecutar</param>
        /// <param name="pPrioridad">Prioridad que tiene la query</param>
        /// <param name="pTablaReplicacion">Tabla donde se va a replicar la query.</param>
        /// <param name="pInfoExtra">Información extra para el procesado por el base.</param>
        public void InsertarConsultaEnColaReplicacion(string pQuery, int pPrioridad, string pTablaReplicacion, string pInfoExtra, bool pUsarHttpPost = true)
        {
            try
            {
                if (string.IsNullOrEmpty(pTablaReplicacion))
                {
                    pTablaReplicacion = "ColaReplicacionMaster";
                }

                DbCommand cmdInsertReplica = ObtenerComando("INSERT INTO " + pTablaReplicacion + " (Consulta, Estado, Prioridad, FechaPuestaEnCola, InfoExtra, UsarHttpPost) VALUES (" + IBD.ToParam("consulta") + ", 0, " + pPrioridad + ", getdate(), '" + pInfoExtra + "', " + Convert.ToInt32(pUsarHttpPost) + ")");
                AgregarParametro(cmdInsertReplica, IBD.ToParam("consulta"), DbType.String, pQuery);
                ActualizarBaseDeDatos(cmdInsertReplica);
            }
            catch
            {
                //Ecribimos en el fichero de replicación para que lo replique otro servicio
                if (!pTablaReplicacion.Equals("ColaReplicacionMaster"))
                {
                    //Reintentamos otra vez
                    try
                    {
                        DbCommand cmdInsertReplica = ObtenerComando("INSERT INTO " + pTablaReplicacion + " (Consulta, Estado, Prioridad, FechaPuestaEnCola, InfoExtra, UsarHttpPost) VALUES (" + IBD.ToParam("consulta") + ", 0, " + pPrioridad + ", getdate(), '" + pInfoExtra + "', " + Convert.ToInt32(pUsarHttpPost) + ")");
                        AgregarParametro(cmdInsertReplica, IBD.ToParam("consulta"), DbType.String, pQuery);
                        ActualizarBaseDeDatos(cmdInsertReplica);
                    }
                    catch
                    {
                        string jsonObject = "{ \"replicacion\": { " +
                        "\"Query\": \"" + pQuery.Replace("\"", "\\\"") + "\",  " +
                        "\"Prioridad\": \"" + pPrioridad + "\",  " +
                        "\"TablaReplicacion\": \"" + pTablaReplicacion + "\",  " +
                        "\"InfoExtra\": \"" + pInfoExtra + "\" " +
                       " }}, ";

                        mLoggingService.GuardarLogError(jsonObject, mlogger);
                    }
                }
            }
        }

        /// <summary>
        /// Se inserta la query en la cola de replicación pasada como parámetro
        /// </summary>
        /// <param name="pQuery">Query que se va a ejecutar</param>
        /// <param name="pPrioridad">Prioridad que tiene la query</param>
        /// <param name="pTablaReplicacion">Tabla donde se va a replicar la query.</param>
        /// <param name="pInfoExtra">Información extra para el procesado por el base.</param>
        public void InsertarLogConsultaCostosa(string pConsulta, int pTiempo, string pServidor, string pError)
        {
            DbCommand cmdInsertReplica = ObtenerComando(string.Format("INSERT INTO LogConsultasVirtuoso (Fecha, Milisegundos, Servidor, Consulta, Error) VALUES (getdate(), {0}, {1}, {2}, {3})", IBD.ToParam("milisegundos"), IBD.ToParam("servidor"), IBD.ToParam("consulta"), IBD.ToParam("error")));

            AgregarParametro(cmdInsertReplica, IBD.ToParam("milisegundos"), DbType.Int32, pTiempo);
            AgregarParametro(cmdInsertReplica, IBD.ToParam("servidor"), DbType.String, pServidor);
            AgregarParametro(cmdInsertReplica, IBD.ToParam("consulta"), DbType.String, pConsulta);

            AgregarParametro(cmdInsertReplica, IBD.ToParam("error"), DbType.String, pError);

            ActualizarBaseDeDatos(cmdInsertReplica);
        }

        /// <summary>
        /// Método que inserta en la BD las consultas de virtuoso.
        /// </summary>
        /// <param name="pQuery">Consulta para actualizar virtuoso</param>
        /// <param name="pGrafoID">Grafo en el que se inserta la consulta</param>
        /// <param name="pPrioridad">Prioridad de la consulta</param>
        /// <param name="pTablaReplicacion">Tabla en que se debe insertar la consulta</param>
        public void InsertarQueryActualizarVirtuosoServicioReplicacion(string pQuery, string pGrafoID, int pPrioridad, string pTablaReplicacion)
        {
            InsertarQueryActualizarVirtuosoServicioReplicacion(pQuery, pGrafoID, pPrioridad, pTablaReplicacion, "");
        }


        /// <summary>
        /// Método que inserta en la BD las consultas de virtuoso.
        /// </summary>
        /// <param name="pQuery">Consulta para actualizar virtuoso</param>
        /// <param name="pGrafoID">Grafo en el que se inserta la consulta</param>
        /// <param name="pPrioridad">Prioridad de la consulta</param>
        /// <param name="pTablaReplicacion">Tabla en que se debe insertar la consulta</param>
        /// <param name="pInfoExtra">Información extra para el procesado por el base.</param>
        public void InsertarQueryActualizarVirtuosoServicioReplicacion(string pQuery, string pGrafoID, int pPrioridad, string pTablaReplicacion, string pInfoExtra, bool pUsarHttpPost = true)
        {
            string query = "INSERT INTO " + pTablaReplicacion + " (Consulta, Estado, Prioridad, FechaPuestaEnCola, InfoExtra, UsarHttpPost) ";
            //query += " OUTPUT inserted.OrdenEjecucion ";
            query += " VALUES (" + IBD.ToParam("consulta") + ", 0, " + pPrioridad + ", GETDATE(), " + IBD.ToParam("infoExtra") + ", " + IBD.ToParam("usarHttpPost") + ")";

            //ActualizarBaseDeDatos(cmdInsertColaActualizarVirtuoso);

            DbCommand cmdInsertColaActualizarVirtuoso = ObtenerComando(query);
            AgregarParametro(cmdInsertColaActualizarVirtuoso, IBD.ToParam("consulta"), DbType.String, pQuery);
            AgregarParametro(cmdInsertColaActualizarVirtuoso, IBD.ToParam("infoExtra"), DbType.String, pInfoExtra);
            AgregarParametro(cmdInsertColaActualizarVirtuoso, IBD.ToParam("usarHttpPost"), DbType.Boolean, pUsarHttpPost);

            EjecutarEscalar(cmdInsertColaActualizarVirtuoso);
        }

        public void InsertarConsultaEnReplica(int pOrdenEjecucion, string pNombreTablaReplica, string pNombreTablaOrigen)
        {
            DbCommand cmdInsertReplica = ObtenerComando("INSERT INTO " + pNombreTablaReplica + " (OrdenEjecucion, Consulta, Estado, Prioridad, FechaPuestaEnCola, InfoExtra, UsarHttpPost) (SELECT OrdenEjecucion, Consulta, Estado, Prioridad, FechaPuestaEnCola, InfoExtra, UsarHttpPost FROM " + pNombreTablaOrigen + " WHERE OrdenEjecucion = " + IBD.ToParam("ordenEjecucion") + ")");
            AgregarParametro(cmdInsertReplica, IBD.ToParam("ordenEjecucion"), DbType.Int32, pOrdenEjecucion);
            ActualizarBaseDeDatos(cmdInsertReplica);
        }

        public int ContarReplicacionesPendientesEnReplica(string pNombreConexion)
        {
            return mServicesUtilVirtuosoAndReplication.ContarReplicacionesPendientesEnReplica(pNombreConexion);
        }

        /// <summary>
        /// Comprueba si una consulta ya existe en una réplica particular
        /// </summary>
        /// <param name="pOrdenEjecucion">Identificador de la consulta a comprobar</param>
        /// <param name="pNombreTablaReplica">Nombre de la tabla en la que se va a comprobar la consulta</param>
        public bool ComprobarConsultaYaInsertadaEnReplica(int pOrdenEjecucion, string pNombreTablaReplica)
        {
            DbCommand cmdComprobarConsultaEnReplica = ObtenerComando("SELECT 1 FROM " + pNombreTablaReplica + " WHERE OrdenEjecucion = " + IBD.ToParam("ordenEjecucion"));
            AgregarParametro(cmdComprobarConsultaEnReplica, IBD.ToParam("ordenEjecucion"), DbType.Int32, pOrdenEjecucion);
            object resultado = EjecutarEscalar(cmdComprobarConsultaEnReplica);

            return (resultado != null && resultado.Equals(1));
        }

        public int? ObtenerUltimaOrdenEjecucionDeCola(string pNombreTablaReplica)
        {
            DbCommand cmdObtenerUltimaOrdenEjecucion = ObtenerComando("SELECT MAX(OrdenEjecucion) FROM " + pNombreTablaReplica + " WHERE Estado = 0");
            object resultado = EjecutarEscalar(cmdObtenerUltimaOrdenEjecucion);

            if (resultado != null && resultado is int)
            {
                return (int)resultado;
            }

            return null;
        }

        public void TransferirFilasACola(string pNombreTablaMaster, string pNombreTablaReplica, int pMaxOrdenEjecucion)
        {
            DbCommand cmdTransferirFilasACola = ObtenerComando("INSERT INTO " + pNombreTablaReplica + " SELECT * FROM " + pNombreTablaMaster + " WHERE Estado = 0 AND OrdenEjecucion <= " + pMaxOrdenEjecucion + " AND NOT EXISTS (SELECT 1 FROM " + pNombreTablaReplica + " WHERE " + pNombreTablaReplica + ".OrdenEjecucion = " + pNombreTablaMaster + ".OrdenEjecucion)");
            cmdTransferirFilasACola.CommandTimeout = 600;

            ActualizarBaseDeDatos(cmdTransferirFilasACola);
        }

        public void ActualizarEstadoCola(string pNombreTablaMaster, int pMaxOrdenEjecucion)
        {
            DbCommand cmdTransferirFilasACola = ObtenerComando("UPDATE " + pNombreTablaMaster + " SET Estado = 5 WHERE Estado = 0 and OrdenEjecucion <= " + pMaxOrdenEjecucion);

            ActualizarBaseDeDatos(cmdTransferirFilasACola);
        }

        public void ActualizarEstadoCola(int pOrdenEjecucion, short pEstado, string pNombreTablaReplica)
        {
            DbCommand cmdUpdateReplica = ObtenerComando("UPDATE " + pNombreTablaReplica + " SET Estado = " + IBD.ToParam("estado") + ", FechaProcesado = getdate() WHERE OrdenEjecucion = " + IBD.ToParam("ordenEjecucion"));
            AgregarParametro(cmdUpdateReplica, IBD.ToParam("ordenEjecucion"), DbType.Int32, pOrdenEjecucion);
            AgregarParametro(cmdUpdateReplica, IBD.ToParam("estado"), DbType.Int16, pEstado);
            ActualizarBaseDeDatos(cmdUpdateReplica);
        }

        public void ActualizarEstadoCola(List<int> pOrdenesEjecucion, short pEstado, string pNombreTablaReplica)
        {
            if (pOrdenesEjecucion.Count > 0)
            {
                DbCommand cmdUpdateReplica = ObtenerComando("UPDATE " + pNombreTablaReplica + " SET Estado = " + IBD.ToParam("estado") + ", FechaProcesado = getdate() ");

                string ordenesEjecucion = " WHERE OrdenEjecucion IN (";
                string coma = "";
                foreach (int ordenEjecucion in pOrdenesEjecucion)
                {
                    ordenesEjecucion += coma + ordenEjecucion;
                    coma = ", ";
                }

                ordenesEjecucion += ")";

                cmdUpdateReplica.CommandText += ordenesEjecucion;

                AgregarParametro(cmdUpdateReplica, IBD.ToParam("estado"), DbType.Int16, pEstado);
                ActualizarBaseDeDatos(cmdUpdateReplica);
            }
        }

        public BaseComunidadDS ObtenerElementosPendientesColaReplicacion(int pNumMaxItems, string pNombreTablaReplica, short pEstadoMaximo)
        {
            BaseComunidadDS brComDS = new BaseComunidadDS();

            string consultaComunidades = "SELECT TOP " + pNumMaxItems + " * FROM " + pNombreTablaReplica + " WHERE Estado < " + pEstadoMaximo + " ORDER BY OrdenEjecucion ASC";

            DbCommand cmdObtnerElementosColaPendientesComunidades = ObtenerComando(consultaComunidades);

            CargarDataSet(cmdObtnerElementosColaPendientesComunidades, brComDS, "ColaReplicacion");

            return brComDS;
        }

        public BaseComunidadDS ObtenerElementosColaReplicacionMismaTransaccion(string pNombreTablaReplica, short pEstadoMaximo, string pInfoExtra)
        {
            BaseComunidadDS baseComDS = new BaseComunidadDS();

            string consultaComunidades = $"SELECT * FROM {pNombreTablaReplica} WHERE Estado < {pEstadoMaximo} AND InfoExtra = {IBD.ToParam("infoExtra")} ORDER BY OrdenEjecucion ASC";

            DbCommand cmdObtnerElementosColaPendientesComunidades = ObtenerComando(consultaComunidades);
            AgregarParametro(cmdObtnerElementosColaPendientesComunidades, IBD.ToParam("infoExtra"), DbType.String, pInfoExtra);

            CargarDataSet(cmdObtnerElementosColaPendientesComunidades, baseComDS, "ColaReplicacion");

            return baseComDS;
        }


        /// <summary>
        /// Elimina de la cola los elementos que han sido procesado exitósamente hace una semana
        /// Si es la cola maestra, copia el contenido en la tabla de histórico
        /// </summary>
        /// <param name="pEsMaster">Verdad si es la cola maestra</param>
        /// <param name="pNombreTablaCola">Nombre de la tabla de cola</param>
        /// <param name="pFechaLimiteEliminacion">Fecha límite hasta la cuál se eliminaran los elementos en cola</param>
        public void EliminarElementosColaReplicaProcesados(string pNombreTablaCola, bool pEsMaster, DateTime pFechaLimiteEliminacion)
        {
            DbCommand cmdEliminarElementosColaProcesados = ObtenerComando("DELETE FROM " + pNombreTablaCola + " WHERE Estado = " + (short)EstadosColaTags.Procesado + " AND FechaProcesado < " + IBD.ToParam("fecha"));
            cmdEliminarElementosColaProcesados.CommandTimeout = 1800; // 1800 segundos = 30 minutos

            AgregarParametro(cmdEliminarElementosColaProcesados, IBD.ToParam("fecha"), DbType.DateTime, pFechaLimiteEliminacion);

            ActualizarBaseDeDatos(cmdEliminarElementosColaProcesados);
        }

        #endregion


    }
}
