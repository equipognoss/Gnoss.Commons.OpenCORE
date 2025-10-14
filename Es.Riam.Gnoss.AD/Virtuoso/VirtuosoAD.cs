using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.Facetado;
using Es.Riam.Gnoss.AD.Facetado.Model;
using Es.Riam.Gnoss.AD.Suscripcion;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.Extensions.Logging;
using OpenLink.Data.Virtuoso;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Es.Riam.Gnoss.AD.Virtuoso
{
    public class VirtuosoAD : IDisposable
    {
        public VirtuosoConnection Connection
        {
            get { return mServicesUtilVirtuosoAndReplication.Connection; }
            set
            {
                mServicesUtilVirtuosoAndReplication.Connection = value;
            }
        }
        public List<string> ListaGrafos 
        {
            get { return mServicesUtilVirtuosoAndReplication.ListaGrafos; }
            set { mServicesUtilVirtuosoAndReplication.ListaGrafos = value; }

        }       
        
        
        public DateTime FechaFinAfinidad 
        { 
            get { return mServicesUtilVirtuosoAndReplication.FechaFinAfinidad; }
            set { mServicesUtilVirtuosoAndReplication.FechaFinAfinidad = value; }
        }
        
        #region Miembros

              

        private LoggingService mLoggingService;
        private EntityContext mEntityContext;
        private ConfigService mConfigService;
        private IServicesUtilVirtuosoAndReplication mServicesUtilVirtuosoAndReplication;
        private ILogger mlogger;
        private ILoggerFactory mloggerFactory;

        #endregion

        #region Constructores

        public VirtuosoAD(LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<VirtuosoAD> logger, ILoggerFactory loggerFactory, VirtuosoConnectionData pVirtuosoConnectionData = null)
        {
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;
            mlogger= logger;
            mloggerFactory= loggerFactory;
            
            if(mServicesUtilVirtuosoAndReplication.VirtuosoConnectionData == null || pVirtuosoConnectionData != null)
            {
                mServicesUtilVirtuosoAndReplication.VirtuosoConnectionData = pVirtuosoConnectionData;
            }            
        }

        #endregion

        #region Métodos

        public void ActualizarVirtuoso_ClienteTradicional(string pQuery)
        {
            int resultado = 0;

            VirtuosoCommand myCommand = null;
            VirtuosoConnection conexion = null;

            int inicioConsulta = pQuery.IndexOf('{') + 1;
            int posicionTriples = -1;

            string instruccion = pQuery.Substring(0, inicioConsulta);
            if (instruccion.Contains(" INSERT INTO "))
            {
                string triples = pQuery.Substring(inicioConsulta, pQuery.LastIndexOf('}') - inicioConsulta);
                posicionTriples = mServicesUtilVirtuosoAndReplication.ObtenerPosicionTriple(triples);
            }

            if (posicionTriples != -1)
            {
                //Es un insert de más de 10000 líneas, la parto para que no falle
                EjecutarInsertPorTrozos(pQuery, posicionTriples, false);
            }
            else
            {
                try
                {
                    conexion = ObtenerConexion();

                    myCommand = new VirtuosoCommand(pQuery, conexion);
                    myCommand.CommandTimeout = TimeOutVirtuoso;

                    resultado = myCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    mServicesUtilVirtuosoAndReplication.CerrarConexion();
                    throw new ExcepcionDeBaseDeDatos(pQuery + "\r\n" + mServicesUtilVirtuosoAndReplication.VirtuosoConnectionData, ex);
                }
                finally
                {
                    try
                    {
                        if (myCommand != null)
                        {
                            myCommand.Dispose();
                        }

                        myCommand = null;
                    }
                    catch (Exception e)
                    {
                        mLoggingService.GuardarLogError(e, mlogger);
                    }
                }
            }
        }

        public int ActualizarVirtuoso(string pQuery)
        {
            int resultado = 0;

            int inicioConsulta = pQuery.IndexOf('{') + 1;
            int posicionTriples = -1;

            string instruccion = pQuery.Substring(0, inicioConsulta);
            if (instruccion.Contains(" INSERT INTO "))
            {
                string triples = pQuery.Substring(inicioConsulta, pQuery.LastIndexOf('}') - inicioConsulta);
                posicionTriples = mServicesUtilVirtuosoAndReplication.ObtenerPosicionTriple(triples);
            }

            if (posicionTriples != -1)
            {
                //Es un insert de más de 600 líneas, la parto para que no falle
                EjecutarInsertPorTrozos(pQuery, posicionTriples, true);
            }
            else
            {
                //Quito el inicio de SPARQL
                if (pQuery.Trim().ToUpper().StartsWith("SPARQL"))
                {
                    pQuery = pQuery.Trim().Substring(6);
                }

                NameValueCollection parametros = new NameValueCollection();
                parametros.Add("query", pQuery);
                //parametros.Add("timeout", TimeOutVirtuoso.ToString());
                //parametros.Add("format", "text/csv");

                try
                {
                    resultado = ActualizarVirtuoso_WebClient(mServicesUtilVirtuosoAndReplication.VirtuosoConnectionData, pQuery, parametros);
                }
                catch (ExcepcionCheckpointVirtuoso ex)
                {
                    mLoggingService.GuardarLogError(ex, mlogger);
                    //Cerramos las conexiones
                    //if (TransaccionVirtuoso != null)
                    //{
                    //    // Si hay una transacción activa no cierro la conexión, ya que habría que repetir todos los comandos que se hayan ejecutado antes. 
                    //    CerrarConexion();
                    //}
                    mServicesUtilVirtuosoAndReplication.Connection = null;
                    //mConexionMaster = null;
                    int i = 0;
                    bool estaOperativo = ServidorOperativo();

                    //Realizamos una consulta ask a virtuoso para comprobar si está funcionando
                    while ((!estaOperativo) && i < 5)
                    {
                        i++;
                        //Dormimos 5 segundos
                        Thread.Sleep(5 * 1000);
                        estaOperativo = ServidorOperativo();
                    }

                    if (estaOperativo)
                    {
                        ActualizarVirtuoso_WebClient(mServicesUtilVirtuosoAndReplication.VirtuosoConnectionData, pQuery, parametros);
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return resultado;
        }

        private int ActualizarVirtuoso_WebClient(VirtuosoConnectionData pVirtuosoConnectionData, string pQuery, NameValueCollection pParametros)
        {
            return mServicesUtilVirtuosoAndReplication.ActualizarVirtuoso_WebClient(pVirtuosoConnectionData, pQuery, pParametros);
        }

        private int ObtenerResultadoRespuesta(string pRespuesta)
        {
            //las cadenas de respuesta esperadas pueden ser del tipo:
            //Insert into <http://grafoprueba>, 1 (or less) triples -- done
            //Delete from <http://grafoprueba>, 1 (or less) triples -- done
            //Modify <http://grafoprueba>, delete 0 (or less) and insert 0 (or less) triples -- done

            int resultado = 0;

            if (!string.IsNullOrEmpty(pRespuesta) && pRespuesta.Contains(','))
            {
                try
                {
                    XmlDocument docXml = new XmlDocument();
                    docXml.LoadXml(pRespuesta);

                    XmlNamespaceManager nsmgr = new XmlNamespaceManager(docXml.NameTable);
                    nsmgr.AddNamespace("sp", "http://www.w3.org/2005/sparql-results#");
                    XmlNode nodoLiteral = docXml.SelectSingleNode("//sp:results//sp:result//sp:binding//sp:literal", nsmgr);

                    if (nodoLiteral != null)
                    {
                        string aux = nodoLiteral.InnerText;
                        bool esModify = aux.ToLower().StartsWith("modify");

                        if (!string.IsNullOrEmpty(aux) && pRespuesta.Contains(','))
                        {
                            int indiceComa = aux.IndexOf(',');
                            aux = aux.Substring(indiceComa + 1);

                            if (!string.IsNullOrEmpty(aux) && pRespuesta.Contains('('))
                            {
                                int indiceParentesis = aux.IndexOf('(');
                                if (!esModify)
                                {
                                    aux = aux.Substring(0, indiceParentesis - 1).Trim();
                                }
                                else
                                {
                                    int indiceDelete = aux.ToLower().IndexOf("delete") + 6;
                                    aux = aux.Substring(indiceDelete, indiceParentesis - indiceDelete).Trim();
                                }
                                int numero = 0;

                                if (int.TryParse(aux, out numero))
                                {
                                    resultado = numero;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    mLoggingService.GuardarLogError(ex, mlogger);
                }
            }

            return resultado;
        }

        public DataSet LeerDeVirtuoso_WebClient(string pQuery, string pNombreTablaDS)
        {
            FacetadoDS facetadoDS = new FacetadoDS();
            FacetadoAD facetadoAD = new FacetadoAD("", mLoggingService,mEntityContext, mConfigService, this, mServicesUtilVirtuosoAndReplication,mloggerFactory.CreateLogger<FacetadoAD>(), mloggerFactory);

            facetadoAD.LeerDeVirtuoso_WebClient(mServicesUtilVirtuosoAndReplication.VirtuosoConnectionData, pNombreTablaDS, facetadoDS, pQuery);

            return facetadoDS;
        }



        public DataSet LeerDeVirtuoso(string pQuery, string pNombreTablaDS)
        {
            DataSet ds = new DataSet();

            VirtuosoCommand myCommand = null;
            VirtuosoDataAdapter myAdapter = null;
            VirtuosoConnection conexion = null;

            string instruccion = pQuery.Substring(0, pQuery.IndexOf('{') + 1);
            Stopwatch sw = LoggingService.IniciarRelojTelemetria();

            try
            {
                conexion = ObtenerConexion();

                myCommand = new VirtuosoCommand(pQuery, conexion);
                myAdapter = new VirtuosoDataAdapter(myCommand);
                myCommand.CommandTimeout = TimeOutVirtuoso;

                if (ds != null && ds.Tables.Contains(pNombreTablaDS))
                {
                    ds.Tables[pNombreTablaDS].Clear();
                }

                myAdapter.Fill(ds, pNombreTablaDS);
                mLoggingService.AgregarEntradaDependencia($"Leo de virtuoso. Conexion: {conexion.ConnectionString}. Consulta: {pQuery}", false, "Leer de Virtuoso", sw, true);

                //Sustituimos las '' por ":
                //for (int j = 0; j < ds.Tables[pNombreTablaDS].Columns.Count; j++)
                //{
                //    if (ds.Tables[pNombreTablaDS].Columns[j].DataType.Name == "String")
                //    {
                //        foreach (DataRow fila in ds.Tables[pNombreTablaDS].Rows)
                //        {
                //            if (!fila.IsNull(j))
                //            {
                //                fila[j] = ((string)fila[j]).Replace("''", "\"");
                //            }
                //        }
                //    }
                //}

                //LoggingService.AgregarEntrada("Remplazo terminado.");
            }
            catch (Exception ex)
            {
                mLoggingService.AgregarEntradaDependencia($"Leo de virtuoso. Conexion: {conexion.ConnectionString}. Consulta: {pQuery}", false, "Leer de Virtuoso", sw, false);
                mServicesUtilVirtuosoAndReplication.CerrarConexion();
                throw new ExcepcionDeBaseDeDatos(pQuery, ex);
            }
            finally
            {
                try
                {
                    if (myAdapter != null)
                    {
                        myAdapter.Dispose();
                    }

                    if (myCommand != null)
                    {
                        myCommand.Dispose();
                    }

                    myAdapter = null;
                    myCommand = null;
                    conexion = null;
                }
                catch (Exception e)
                {
                    mLoggingService.GuardarLogError(e, mlogger);
                }
            }

            return ds;
        }

        private VirtuosoConnection ObtenerConexion()
        {
            VirtuosoConnection conexion = ParentConnection;

            if (conexion != null && !conexion.State.Equals(ConnectionState.Open))
            {
                mServicesUtilVirtuosoAndReplication.QuickOpen(conexion, 200);
                //conexion.Open();
            }

            return conexion;
        }

        /// <summary>
        /// Actualiza virtuoso (mediante un insert / update / delete)
        /// </summary>
        /// <param name="pQuery">Query a ejecutar (insert / update / delete)</param>
        /// <param name="pIndiceTriplePartir">Indice del triple por el que se va a partir los trozos a insertart</param>
        public void EjecutarInsertPorTrozos(string pQuery, int pIndiceTriplePartir, bool pUsarHttpPost)
        {
            try
            {
                bool transaccionIniciada = IniciarTransaccion();
                int indiceInicioTriples = pQuery.IndexOf('{') + 1;
                int finTriples = pQuery.LastIndexOf('}');
                string instruccion = pQuery.Substring(0, indiceInicioTriples);
                string triples = pQuery.Substring(indiceInicioTriples, finTriples - indiceInicioTriples);

                while (pIndiceTriplePartir != -1)
                {
                    string conjuntoTriples = triples.Substring(0, pIndiceTriplePartir);
                    triples = triples.Substring(pIndiceTriplePartir);

                    string consulta = $"{instruccion}{conjuntoTriples}}}";
                    
                    if (pUsarHttpPost)
                    {
                        ActualizarVirtuoso(consulta);
                    }
                    else
                    {
                        ActualizarVirtuoso_ClienteTradicional(consulta);
                    }

                    pIndiceTriplePartir = mServicesUtilVirtuosoAndReplication.ObtenerPosicionTriple(triples);
                }

                if (!string.IsNullOrEmpty(triples))
                {
                    string consulta = $"{instruccion}{triples}}}";

                    if (pUsarHttpPost)
                    {
                        ActualizarVirtuoso(consulta);
                    }
                    else
                    {
                        ActualizarVirtuoso_ClienteTradicional(consulta);
                    }
                }

                if (transaccionIniciada)
                {
                    TerminarTransaccion(true);
                }
            }
            catch
            {
                TerminarTransaccion(false);
                throw;
            }
        }

        public bool IniciarTransaccion()
        {
            string nombreTransaccion = $"VirtuosoTransaccion";
            if (mServicesUtilVirtuosoAndReplication.TransaccionVirtuoso != null)
            {
                return false;
            }
            else
            {
                DbTransaction transaccion = ParentConnection.BeginTransaction();

                mServicesUtilVirtuosoAndReplication.TransaccionVirtuoso = transaccion;

                if (mEntityContext.NoConfirmarTransacciones)
                {
                    mEntityContext.TransaccionesPendientes.Add(nombreTransaccion, transaccion);
                }
                return true;
            }
        }
       
        public string AfinidadVirtuoso { get { return mServicesUtilVirtuosoAndReplication.ConexionAfinidadVirtuoso; } }
        
        /// <summary>
        /// Terminamos la transaccción
        /// </summary>
        /// <param name="pExito"></param>
        public void TerminarTransaccion(bool pExito)
        {
            if (!mEntityContext.NoConfirmarTransacciones)
            {
                string nombreTransaccion = $"VirtuosoTransaccion";
                if (mServicesUtilVirtuosoAndReplication.TransaccionVirtuoso != null)
                {
                    DbTransaction transaccion = mServicesUtilVirtuosoAndReplication.TransaccionVirtuoso;
                    if (pExito)
                    {
                        transaccion.Commit();
                    }
                    else if (transaccion.Connection != null)
                    {
                        try
                        {
                            transaccion.Rollback();
                        }
                        catch (Exception ex)
                        {
                            mLoggingService.GuardarLogError(ex, mlogger);
                        }

                        mServicesUtilVirtuosoAndReplication.CerrarConexion();
                    }
                    transaccion = null;
                    
                }
                mServicesUtilVirtuosoAndReplication.TransaccionVirtuoso.Dispose();
                mServicesUtilVirtuosoAndReplication.TransaccionVirtuoso = null;
            }
        }

        

        /// <summary>
        /// Comprueba si la conexión es null o la cadena de conexión está vacía
        /// </summary>
        /// <param name="pConexion">Conexión a virtuoso</param>
        /// <returns></returns>
        private bool ComprobarConexionValida(VirtuosoConnection pConexion)
        {
            if (pConexion != null)
            {
                try
                {
                    string cadenaConexion = pConexion.ConnectionString;
                    if (string.IsNullOrEmpty(cadenaConexion))
                    {
                        //la cadena de conexión es vacía
                        return false;
                    }
                }
                catch (Exception e)
                {
                    mLoggingService.GuardarLogError(e, mlogger);
                    //La cadena de conexión está mal formada
                    return false;
                }
                //Si llega hasta aquí, la conexión es buena
                return true;
            }
            //La cadena de conexión es null
            return false;
        }

        

        #endregion

        #region Propiedades

        private static int? mTimeOutVirtuoso = null;

        private int TimeOutVirtuoso
        {
            get
            {
                if (!mTimeOutVirtuoso.HasValue)
                {
                    int timeOut = 60;

                    int.TryParse(mConfigService.ObtenerTimeoutVirtuoso(), out timeOut);
                    mTimeOutVirtuoso = timeOut;
                }
                return mTimeOutVirtuoso.Value;
            }
        }

        /// <summary>
        /// Obtiene la conexión a virtuoso
        /// </summary>
        private VirtuosoConnection ParentConnection
        {
            get
            {
                return mServicesUtilVirtuosoAndReplication.ParentConnection;
            }
        }

        public bool Conectado
        {
            get
            {
                bool conectado = false;
                try
                {
                    conectado = mServicesUtilVirtuosoAndReplication.Connection.State == ConnectionState.Open;
                }
                catch { conectado = false; }
                return conectado;
            }
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Determina si está disposed
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Destructor
        /// </summary>
        ~VirtuosoAD()
        {
            //Libero los recursos
            Dispose(false);
        }

        /// <summary>
        /// Libera los recursos
        /// </summary>
        public void Dispose()
        {
            mServicesUtilVirtuosoAndReplication.CerrarConexion();
            Dispose(true);

            //impido que se finalice dos veces este objeto
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Libera los recursos
        /// </summary>
        /// <param name="disposing">Determina si se está llamando desde el Dispose()</param>
        public void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                this.disposed = true;
            }

            //CerrarConexion("mConexion");
        }

        #endregion

        public bool ServidorOperativo()
        {
            VirtuosoAD pFacetadoCN = null;
            bool servidorOperativo = false;

            try
            {
                pFacetadoCN = new VirtuosoAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication,mloggerFactory.CreateLogger<VirtuosoAD>(),mloggerFactory);
                //Hacemos una consulta sencilla a virtuoso:
                string query = "SPARQL ASK {?s ?p ?o.}";
                pFacetadoCN.LeerDeVirtuoso(query, "TablaDePrueba");

                //GuardarLog("Servidor Operativo.", IpServidor);
                servidorOperativo = true;
            }
            catch (Exception)
            {
                //GuardarLog("Todavía no está operativo. '" + ex.Message + "'", IpServidor);
                servidorOperativo = false;
            }
            finally
            {
                pFacetadoCN.Dispose();
                pFacetadoCN = null;
            }

            return servidorOperativo;
        }
    }
}
