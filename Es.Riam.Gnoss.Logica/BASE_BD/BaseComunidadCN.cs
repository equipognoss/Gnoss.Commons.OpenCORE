using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.BASE_BD;
using Es.Riam.Gnoss.AD.BASE_BD.Model;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.EntityModelBASE.Models;
using Es.Riam.Gnoss.AD.Facetado;
using Es.Riam.Gnoss.RabbitMQ;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Util;
using Es.Riam.Util.Correo;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;

namespace Es.Riam.Gnoss.Logica.BASE_BD
{
    /// <summary>
    /// Lógica para modelo BASE de comunidad
    /// </summary>
    public class BaseComunidadCN : BaseCN, IDisposable
    {
        #region constantes

        private const string COLA_CORREO = "ColaCorreo";
        private const string EXCHANGE = "";

        #endregion

        #region Miembro

        /// <summary>
        /// Nos indica si actualmente hay conexion a RabbitMQ
        /// </summary>
        private static bool? mHayConexionRabbit = null;

        #endregion

        public BaseComunidadCN(EntityContext entityContext, LoggingService loggingService, EntityContextBASE entityContextBASE, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication)
        {
            BaseComunidadAD = new BaseComunidadAD(loggingService, entityContext, entityContextBASE, configService, servicesUtilVirtuosoAndReplication);
        }

        /// <summary>
        /// Cuando se desea pasar directamente la ruta del fichero de configuracion de conexion a la Base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Fichero de configuración de la base de datos BASE</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public BaseComunidadCN(string pFicheroConfiguracionBD, EntityContext entityContext, LoggingService loggingService, EntityContextBASE entityContextBASE, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(entityContext, loggingService, configService, entityContextBASE, servicesUtilVirtuosoAndReplication)
        {
            BaseComunidadAD = new BaseComunidadAD(-1, loggingService, entityContext, entityContextBASE, configService, servicesUtilVirtuosoAndReplication);
        }
        public BaseComunidadCN(string pFicheroConfiguracionBD, int pTablaBaseProyectoID, EntityContext entityContext, LoggingService loggingService, EntityContextBASE entityContextBASE, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication)
        {
            this.BaseComunidadAD = new BaseComunidadAD(pTablaBaseProyectoID, loggingService, entityContext, entityContextBASE, configService, servicesUtilVirtuosoAndReplication);
        }
        #region Métodos generales        

        #region Métodos de carga del DS

        /// <summary>
        /// Obtiene los documentos modificados posteriormente a la fecha pasada como parámetro
        /// </summary>
        /// <param name="pFechaModificacion">Fecha de búsqueda de modificaciones</param>
        /// <returns>Lista con los documentosID obtenidos en la búsqueda</returns>
        public List<Guid> ObtenerDocumentosModificados(DateTime pFechaModificacion, int pTopN)
        {
            return BaseComunidadAD.ObtenerDocumentosModificados(pFechaModificacion, pTopN);
        }

        #endregion

        #region Métodos de colas
        public void InsertarFilasEnRabbit(string pColaRabbit, DataSet pDataSet, string pNombreTabla = null)
        {
            BaseComunidadAD.InsertarFilasEnRabbit(pColaRabbit, pDataSet, pNombreTabla);
        }

            public void ActualizarBD(BaseComunidadDS baseComDS, bool pUsarRabbitSiEstaConfigurado = true)
        {
            BaseComunidadAD.ActualizarBD(baseComDS, pUsarRabbitSiEstaConfigurado);
        }

        /// <summary>
        /// Obtiene los elementos pendientes de la cola de compartición automática
        /// </summary>
        /// <returns></returns>
        public BaseComunidadDS ObtenerColaModificacionSearchPendientes()
        {
            return BaseComunidadAD.ObtenerColaModificacionSearchPendientes();
        }

        /// <summary>
        /// Obtiene los elementos pendientes de la cola de compartición automática
        /// </summary>
        /// <returns></returns>
        public BaseComunidadDS ObtenerColaComparticionAutomaticaPendientes()
        {
            return BaseComunidadAD.ObtenerColaComparticionAutomaticaPendientes();
        }

        /// <summary>
        /// Obtiene los elementos pendientes de la cola de sitemaps
        /// </summary>
        /// <returns></returns>
        public BaseComunidadDS ObtenerColaSitemapsPendientes()
        {
            return BaseComunidadAD.ObtenerColaSitemapsPendientes();
        }

        /// <summary>
        /// Obtiene los elementos pendientes de la cola de sitemaps
        /// </summary>
        /// <returns></returns>
        public BaseComunidadDS ObtenerColaSitemapsPendientesTipoComunidad()
        {
            return BaseComunidadAD.ObtenerColaSitemapsPendientesTipoComunidad();
        }

        /// <summary>
        /// Obtiene los elementos pendientes de la cola de sitemaps
        /// </summary>
        /// <returns></returns>
        public BaseComunidadDS ObtenerColaSitemapsPendientesPorProyecto(string pNombreCorto)
        {
            return BaseComunidadAD.ObtenerColaSitemapsPendientesPorProyecto(pNombreCorto);
        }

        /// <summary>
        /// Elimina las filas procesadas correctamente anteriores a la fecha actual menos un dia
        /// </summary>
        public void BorrarFilasProcesadasCorrectamente()
        {
            BaseComunidadAD.BorrarFilasProcesadasCorrectamente();
        }

        /// <summary>
        /// Inserta las filas correspondientes en ColaCorreo y ColaCorreoDestinatario
        /// </summary>
        /// <param name="pParametrosCorreo">Parámetros de configuración del servicio de correo</param>
        /// <param name="pDestinatarios">Lista de correos de los destinatarios</param>
        /// <param name="pAsunto">Asunto</param>
        /// <param name="pCuerpo">Mensaje</param>
        /// <param name="pEsHtml">Indica si el mensaje es en formato HTML</param>
        public void InsertarCorreo(ConfiguracionEnvioCorreo pParametrosCorreo, List<string> pDestinatarios, string pAsunto, string pCuerpo, bool pEsHtml)
        {
            InsertarCorreo(pParametrosCorreo, pDestinatarios, pAsunto, pCuerpo, pEsHtml, "");
        }

        /// <summary>
        /// Inserta las filas correspondientes en ColaCorreo y ColaCorreoDestinatario
        /// </summary>
        /// <param name="pParametrosCorreo">Parámetros de configuración del servicio de correo</param>
        /// <param name="pDestinatarios">Lista de correos de los destinatarios</param>
        /// <param name="pAsunto">Asunto</param>
        /// <param name="pCuerpo">Mensaje</param>
        /// <param name="pEsHtml">Indica si el mensaje es en formato HTML</param>
        public int InsertarCorreo(ConfiguracionEnvioCorreo pParametrosCorreo, List<string> pDestinatarios, string pAsunto, string pCuerpo, bool pEsHtml, string pMascaraRemitente)
        {
            try
            {
                bool esSeguro = false;
                int correoID;

                if (pParametrosCorreo.puerto.Equals((short)465) || pParametrosCorreo.puerto.Equals((short)587))
                {
                    esSeguro = true;
                }

                IniciarTransaccionBASE();
                {
                    if (pMascaraRemitente == null)
                    {
                        pMascaraRemitente = string.Empty;
                    }

                    correoID = this.InsertarFilasEnColaCorreo(pParametrosCorreo.email, pAsunto, pCuerpo, pEsHtml, (short)PrioridadBase.ApiRecursos, pMascaraRemitente, pParametrosCorreo.emailsugerencias, "", pParametrosCorreo.smtp, pParametrosCorreo.usuario, pParametrosCorreo.clave, pParametrosCorreo.puerto, esSeguro, pParametrosCorreo.tipo);

                    if (correoID > 0)
                    {
                        foreach (string destinatario in pDestinatarios)
                        {
                            this.InsertarFilasEnColaCorreoDestinatarios(correoID, destinatario, "");
                        }

                        mEntityContextBASE.SaveChanges();
                    }
                    TerminarTransaccionBASE(true);

                    InsertarCorreoIDColaCorreoRabbitMQ(correoID);

                }
                return correoID;
            }
            catch
            {
                TerminarTransaccion(false);
                throw;
            }
        }

        public bool HayConexionRabbit
        {
            get
            {
                if (!mHayConexionRabbit.HasValue)
                {
                    string cadena = mConfigService.ObtenerRabbitMQClient(RabbitMQClient.BD_SERVICIOS_WIN);
                    mHayConexionRabbit = !string.IsNullOrEmpty(cadena);
                }
                return mHayConexionRabbit.Value;
            }
        }

        private void InsertarCorreoIDColaCorreoRabbitMQ(int pCorreoID)
        {
            if (HayConexionRabbit)
            {
                using (RabbitMQClient rabbitMQ = new RabbitMQClient(RabbitMQClient.BD_SERVICIOS_WIN, COLA_CORREO, mLoggingService, mConfigService, EXCHANGE, COLA_CORREO))
                {
                    rabbitMQ.AgregarElementoACola(JsonConvert.SerializeObject(pCorreoID));
                }
            }
        }

        /// <summary>
        /// Inserta una fila en ColaCorreo
        /// </summary>
        /// <param name="pRemitente">Email del remitente del mensaje</param>
        /// <param name="pAsunto">Asunto del mensaje</param>
        /// <param name="pHtmlTexto">Cuerpo del mensaje</param>
        /// <param name="pEsHtml">Indica si el cuerpo del mensaje está en formato HTML</param>
        /// <param name="pPrioridad">Prioridad para procesar el mensaje</param>
        /// <param name="pMascaraRemitente">Máscara del remitente</param>
        /// <param name="pDirecionRespuesta">Email donde recibir la respuesta</param>
        /// <param name="pMascaraDireccionRespuesta">Máscara de la dirección de respuesta</param>
        /// <param name="pSMTP">Url del servicio SMTP</param>
        /// <param name="pUsuario">Usuario del servicio de correo</param>
        /// <param name="pPassword">Contraseña del servicio de correo</param>
        /// <param name="pPuerto">Puerto del servicio de correo</param>
        /// <param name="pEsSeguro">Indica si el servicio de correo usa protocolo seguridad</param>
        /// <returns>Entero con el identificador del correo que se ha insertado</returns>
        public int InsertarFilasEnColaCorreo(string pRemitente, string pAsunto, string pHtmlTexto, bool pEsHtml, short pPrioridad, string pMascaraRemitente, string pDireccionRespuesta, string pMascaraDireccionRespuesta, string pSMTP, string pUsuario, string pPassword, int pPuerto, bool pEsSeguro, string pTipo)
        {
            return BaseComunidadAD.InsertarFilasEnColaCorreo(pRemitente, pAsunto, pHtmlTexto, pEsHtml, pPrioridad, pMascaraRemitente, pDireccionRespuesta, pMascaraDireccionRespuesta, pSMTP, pUsuario, pPassword, pPuerto, pEsSeguro, pTipo);
        }

        /// <summary>
        /// Inserta una fila pendiente de procesar en ColaCorreoDestinatario
        /// </summary>
        /// <param name="pCorreoID">Identificador del correo a enviar</param>
        /// <param name="pEmail">Email del destinatario</param>
        /// <param name="pMascaraDestinatario">Máscara del destinatario</param>
        /// <returns>True si se ha insertado. False en caso contrario</returns>
        public bool InsertarFilasEnColaCorreoDestinatarios(int pCorreoID, string pEmail, string pMascaraDestinatario)
        {
            return BaseComunidadAD.InsertarFilasEnColaCorreoDestinatarios(pCorreoID, pEmail, pMascaraDestinatario);
        }

        /// <summary>
        /// Inserta una fila en la cola de sitemaps
        /// </summary>
        /// <param name="pDocumentoID">ID del documento</param>
        /// <param name="pTipoEvento">Tipo de evento del site map</param>
        /// <param name="pEstado">Estado de la fila</param>
        /// <param name="pFechaCreacion">Fecha de creación/comkpartición de recurso en la comunidad</param>
        /// <param name="pPrioridad">Priorida de la fila</param>
        /// <param name="pComunidad">Nombre corto de la comunidad</param>
        public void InsertarFilaEnColaColaSitemaps(Guid pDocumentoID, TiposEventoSitemap pTipoEvento, short pEstado, DateTime pFechaCreacion, short pPrioridad, string pComunidad)
        {
            BaseComunidadAD.InsertarFilaEnColaColaSitemaps(pDocumentoID, pTipoEvento, pEstado, pFechaCreacion, pPrioridad, pComunidad);
        }

        /// <summary>
        /// Inserta una fila en la cola de actualizacion de contextos
        /// </summary>
        /// <param name="pDocumentoID">Id del documento</param>
        /// <param name="pEstado">Estado de la fila</param>
        /// <param name="pPrioridad">Prioridad de la fila</param>
        /// <param name="FechaPuestaEnCola">Fecha de creacion del recurso</param>
        public void InsertarFilaEnColaActualizaContextos(Guid pDocumentoID, short pEstado, short pPrioridad, DateTime pFechaCreacion)
        {
            BaseComunidadAD.InsertarFilaEnColaActualizaContextos(pDocumentoID, pEstado, pPrioridad, pFechaCreacion);
        }

        /// <summary>
        /// Obtiene los elementos pendientes de la cola de compartición automática
        /// </summary>
        /// <returns></returns>
        public void ActualizarEstadoColaModificacionSearch(int pOrdenEjecucion, short pEstado)
        {
            BaseComunidadAD.ActualizarEstadoColaModificacionSearch(pOrdenEjecucion, pEstado);
        }

        /// <summary>
        /// Obtiene los elementos pendientes de la cola de compartición automática
        /// </summary>
        /// <returns></returns>
        public void ActualizarEstadoColaComparticionAutomatica(Guid pComparticionID, short pEstado)
        {
            ActualizarEstadoColaComparticionAutomatica(pComparticionID, pEstado, "");
        }

        /// <summary>
        /// Obtiene los elementos pendientes de la cola de compartición automática
        /// </summary>
        /// <returns></returns>
        public void ActualizarEstadoColaComparticionAutomatica(Guid pComparticionID, short pEstado, string pIncidencia)
        {
            BaseComunidadAD.ActualizarEstadoColaComparticionAutomatica(pComparticionID, pEstado, pIncidencia);
        }

        /// <summary>
        /// Obtiene los elementos pendientes de la cola de sitemaps
        /// </summary>
        /// <returns></returns>
        public void ActualizarEstadoColaSitemaps(int pOrdenEjecucion, short pEstado)
        {
            BaseComunidadAD.ActualizarEstadoColaSitemaps(pOrdenEjecucion, pEstado);
        }

        /// <summary>
        /// Obtiene una lista con los nombres cortos de los proyectos que tienen filas pendientes en colasitemaps
        /// </summary>
        /// <returns>Lista de cadenas con los nombres cortos de los proyectos</returns>
        public List<string> ObtenerNombresCortosProyectosPendientesColaSitemap()
        {
            return BaseComunidadAD.ObtenerNombresCortosProyectosPendientesColaSitemap();
        }

        /// <summary>
        /// Obtiene los elementos pendientes de la cola de tags
        /// </summary>
        /// <param name="pPrioridadBase">Prioridad del base</param>
        /// <param name="pNumMaxItems">Numero máximo de items a traer</param>
        /// <returns></returns>
        public DataSet ObtenerElementosColaPendientes(int pNumMaxItems)
        {
            return ObtenerElementosColaPendientes(EstadosColaTags.EnEspera, EstadosColaTags.PrimerError, null, pNumMaxItems, null);
        }

        /// <summary>
        /// Obtiene los elementos pendientes de la cola de tags
        /// </summary>
        /// <param name="pPrioridadBase">Prioridad del base</param>
        /// <param name="pNumMaxItems">Numero máximo de items a traer</param>
        /// <returns></returns>
        public DataSet ObtenerElementosColaPendientes(int pNumMaxItems, bool? pSoloPrioridad0)
        {
            return ObtenerElementosColaPendientes(EstadosColaTags.EnEspera, EstadosColaTags.PrimerError, null, pNumMaxItems, pSoloPrioridad0);
        }

        /// <summary>
        /// Obtiene el número de filas que hay en la cola que se encuentran en estado 2(error) en las últimas pHoras horas
        /// </summary>
        ///<param name="pHoras">Horas</param>
        /// <returns></returns>
        public int ObtenerNumeroElementosEnXHoras(int pHoras, EstadosColaTags pEstadoInferior, EstadosColaTags pEstadoSuperior)
        {
            return BaseComunidadAD.ObtenerNumeroElementosEnXHoras(pHoras, pEstadoInferior, pEstadoSuperior);
        }

        /// <summary>
        /// Obtiene los elementos pendientes de la cola de tags
        /// </summary>
        /// <returns>Devuelve un dataset con los elementos de la cola cargados</returns>
        /// <param name="pEstadoInferior">Estado mínimo de los elementos de la cola</param>
        /// <param name="pEstadoSuperior">Estado máximo de los elementos de la cola</param>
        /// <param name="pTiposElementos">Tipo de los elementos a obtener (Agregado, eliminado, ...). Null para obtener todos</param>
        /// <param name="pPrioridadBase">Prioridad del base</param>
        /// <param name="pNumMaxItems">Numero máximo de items a traer</param>
        public DataSet ObtenerElementosColaPendientes(EstadosColaTags pEstadoInferior, EstadosColaTags pEstadoSuperior, TiposElementosEnCola? pTiposElementos, int pNumMaxItems, bool? pSoloPrioridad0)
        {
            if (pSoloPrioridad0.HasValue)
            {
                return BaseComunidadAD.ObtenerElementosColaPendientes(pEstadoInferior, pEstadoSuperior, pTiposElementos, pNumMaxItems, pSoloPrioridad0);
            }
            else
            {
                return BaseComunidadAD.ObtenerElementosColaPendientes(pEstadoInferior, pEstadoSuperior, pTiposElementos, pNumMaxItems);
            }
        }

        /// <summary>
        /// Obtiene los elementos pendientes de la cola de tags
        /// </summary>
        /// <returns>Devuelve un dataset con los elementos de la cola cargados</returns>
        /// <param name="pEstadoInferior">Estado mínimo de los elementos de la cola</param>
        /// <param name="pEstadoSuperior">Estado máximo de los elementos de la cola</param>
        /// <param name="pTiposElementos">Lista de los tipo de elementos a obtener (Agregado, eliminado, ...). Null para obtener todos</param>
        /// <param name="pNumMaxItems">Numero máximo de items a traer</param>
        public DataSet ObtenerElementosColaPendientes(List<TiposElementosEnCola> pTiposElementos, EstadosColaTags pEstadoInferior, EstadosColaTags pEstadoSuperior, int pNumMaxItems, bool? pSoloPrioridad0)
        {
            return BaseComunidadAD.ObtenerElementosColaPendientes(pTiposElementos, pEstadoInferior, pEstadoSuperior, pNumMaxItems, pSoloPrioridad0);
        }

        ///// <summary>
        ///// Obtiene los elementos pendientes de la cola de replicacion MASTER
        ///// </summary>
        ///// <param name="pPrioridadBase">Prioridad de los elementos que se quieren obtener</param>
        ///// <param name="pNumMaxItems">Número máximo de Items a obtener</param>
        ///// <returns></returns>
        //public BaseComunidadDS ObtenerElementosPendientesColaReplicacion(int pNumMaxItems, short pEstadoMaximo)
        //{
        //    return ObtenerElementosPendientesColaReplicacion(pNumMaxItems, "ColaReplicacionMaster", pEstadoMaximo);
        //}

        ///// <summary>
        ///// Obtiene los elementos pendientes de una cola de replicacion
        ///// </summary>
        ///// <param name="pPrioridadBase">Prioridad de los elementos que se quieren obtener</param>
        ///// <param name="pNumMaxItems">Número máximo de Items a obtener</param>
        ///// <param name="pTablaColaReplica">Tabla de la cola que se quiere cargar</param>
        ///// <returns></returns>
        //public BaseComunidadDS ObtenerElementosPendientesColaReplicacion(int pNumMaxItems, string pTablaColaReplica, short pEstadoMaximo)
        //{
        //    return BaseComunidadAD.ObtenerElementosPendientesColaReplicacion(pNumMaxItems, pTablaColaReplica, pEstadoMaximo);
        //}

        //public BaseComunidadDS ObtenerElementosColaReplicacionMismaTransaccion(string pNombreTablaReplica, short pEstadoMaximo, string pInfoExtra)
        //{
        //    return BaseComunidadAD.ObtenerElementosColaReplicacionMismaTransaccion(pNombreTablaReplica, pEstadoMaximo, pInfoExtra);
        //}

        ///// <summary>
        ///// Inserta en una cola de una réplica particular una consulta
        ///// </summary>
        ///// <param name="pOrdenEjecucion">Identificador de la consulta a replicar</param>
        ///// <param name="pNombreTablaReplica">Nombre de la tabla en la que se va a replicar la consulta</param>
        //public void InsertarConsultaEnReplica(int pOrdenEjecucion, string pNombreTablaReplica)
        //{
        //    InsertarConsultaEnReplica(pOrdenEjecucion, pNombreTablaReplica, "ColaReplicacionMaster");
        //}

        ///// <summary>
        ///// Inserta en una cola de una réplica particular una consulta
        ///// </summary>
        ///// <param name="pOrdenEjecucion">Identificador de la consulta a replicar</param>
        ///// <param name="pNombreTablaReplica">Nombre de la tabla en la que se va a replicar la consulta</param>
        ///// <param name="pNombreTablaOrigen">Nombre de la tabla de origen desde la que se va a copiar la fila</param>
        //public void InsertarConsultaEnReplica(int pOrdenEjecucion, string pNombreTablaReplica, string pNombreTablaOrigen)
        //{
        //    BaseComunidadAD.InsertarConsultaEnReplica(pOrdenEjecucion, pNombreTablaReplica, pNombreTablaOrigen);
        //}

        /// <summary>
        /// Inserta una fila en la cola de refresco de caché para que se actualice una búsqueda determinada en un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pTipoBusqueda">Tipo de búsqueda que hay que refrescar</param>
        public void InsertarFilaEnColaRefrescoCache(Guid pProyectoID, TiposEventosRefrescoCache pTipoEvento, TipoBusqueda pTipoBusqueda)
        {
            InsertarFilaEnColaRefrescoCache(pProyectoID, pTipoEvento, pTipoBusqueda, null);
        }

        /// <summary>
        /// Inserta una fila en la cola de refresco de caché para que se actualice una búsqueda determinada en un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pTipoBusqueda">Tipo de búsqueda que hay que refrescar</param>
        /// <param name="pInfoExtra">Información extra (puede ser NULL)</param>
        public void InsertarFilaEnColaRefrescoCache(Guid pProyectoID, TiposEventosRefrescoCache pTipoEvento, TipoBusqueda pTipoBusqueda, string pInfoExtra)
        {
            BaseComunidadAD.InsertarFilaEnColaRefrescoCache(pProyectoID, pTipoEvento, pTipoBusqueda, pInfoExtra);
        }

        /// <summary>
        /// Inserta la fila indicada en la cola ColaRefrescoCahce
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pTipoEvento">Tipo de evento del proyecto señalado</param>
        /// <param name="pTipoBusqueda">Tipo de búsqueda que hay que refrescar</param>
        /// <param name="pInfoExtra">Información extra (puede ser NULL)</param>
        public void InsertarFilaColaRefrescoCacheEnRabbitMQ(Guid pProyectoID, TiposEventosRefrescoCache pTipoEvento, TipoBusqueda pTipoBusqueda, string pInfoExtra)
        {
            BaseComunidadAD.InsertarFilaColaRefrescoCacheEnRabbitMQ(pProyectoID, pTipoEvento, pTipoBusqueda, pInfoExtra);
        }

        public BaseComunidadDS ObtenerColaRefrescoCachePendientes()
        {
            return BaseComunidadAD.ObtenerColaRefrescoCachePendientes();
        }

        public void EliminarColaRefrescoCachePendientesRepetidas()
        {
            BaseComunidadAD.EliminarColaRefrescoCachePendientesRepetidas();
        }

        public BaseComunidadDS ObtenerColaRefrescoCacheBandejaMensajesPendientes()
        {
            return BaseComunidadAD.ObtenerColaRefrescoCacheBandejaMensajesPendientes();
        }

        /// <summary>
        /// Elimina una fila de la cola de refresco de caché
        /// </summary>
        /// <param name="pColaID">Identificador de la fila</param>
        public void EliminarFilaColaRefrescoCache(int pColaID)
        {
            BaseComunidadAD.EliminarFilaColaRefrescoCache(pColaID);
        }

        /// <summary>
        /// Actualiza el estado de una fila de la cola de refresco de caché
        /// </summary>
        /// <param name="pColaID">Identificador de la fila</param>
        /// <param name="pEstado">Estado nuevo de la fila</param>
        public void AcutalizarEstadoColaRefrescoCache(int pColaID, short pEstado)
        {
            BaseComunidadAD.AcutalizarEstadoColaRefrescoCache(pColaID, pEstado);
        }

        /// <summary>
        /// Actualiza el estado de una fila de la cola de actualización de contextos
        /// </summary>
        /// <param name="pDocumentoID"></param>
        /// <param name="pEstado"></param>
        public void ActualizarEstadoColaActualizarContextos(Guid pDocumentoID, short pEstado, DateTime pFechaProcesado)
        {
            BaseComunidadAD.ActualizarEstadoColaActualizarContextos(pDocumentoID, pEstado, pFechaProcesado);
        }

        ///// <summary>
        ///// Comprueba si una consulta ya existe en una réplica particular
        ///// </summary>
        ///// <param name="pOrdenEjecucion">Identificador de la consulta a comprobar</param>
        ///// <param name="pNombreTablaReplica">Nombre de la tabla en la que se va a comprobar la consulta</param>
        //public bool ComprobarConsultaYaInsertadaEnReplica(int pOrdenEjecucion, string pNombreTablaReplica)
        //{
        //    return BaseComunidadAD.ComprobarConsultaYaInsertadaEnReplica(pOrdenEjecucion, pNombreTablaReplica);
        //}

        ///// <summary>
        ///// Actualiza el estado de la cola de replicación MASTER
        ///// </summary>
        ///// <param name="pOrdenEjecucion">Identificador del elemento que se va a actualizar</param>
        ///// <param name="pEstado">Nuevo estado del elemento de la cola</param>
        //public void ActualizarEstadoCola(int pOrdenEjecucion, short pEstado)
        //{
        //    ActualizarEstadoCola(pOrdenEjecucion, pEstado, "ColaReplicacionMaster");
        //}

        ///// <summary>
        ///// Actualiza el estado de una cola de replicación
        ///// </summary>
        ///// <param name = "pOrdenEjecucion" > Identificador del elemento que se va a actualizar</param>
        ///// <param name = "pEstado" > Nuevo estado del elemento de la cola</param>
        ///// <param name = "pNombreTablaReplica" > Nombre de la tabla en la que se va a actualizar el estado</param>
        //public void ActualizarEstadoCola(int pOrdenEjecucion, short pEstado, string pNombreTablaReplica)
        //{
        //    BaseComunidadAD.ActualizarEstadoCola(pOrdenEjecucion, pEstado, pNombreTablaReplica);
        //}

        ///// <summary>
        ///// Actualiza el estado de una cola de replicación
        ///// </summary>
        ///// <param name="pOrdenEjecucion">Identificador del elemento que se va a actualizar</param>
        ///// <param name="pEstado">Nuevo estado del elemento de la cola</param>
        ///// <param name="pNombreTablaReplica">Nombre de la tabla en la que se va a actualizar el estado</param>
        //public void ActualizarEstadoCola(List<int> pOrdenesEjecucion, short pEstado, string pNombreTablaReplica)
        //{
        //    BaseComunidadAD.ActualizarEstadoCola(pOrdenesEjecucion, pEstado, pNombreTablaReplica);
        //}

        //public int? ObtenerUltimaOrdenEjecucionDeCola(string pNombreTablaReplica)
        //{
        //    return BaseComunidadAD.ObtenerUltimaOrdenEjecucionDeCola(pNombreTablaReplica);
        //}

        //public void TransferirFilasACola(string pNombreTablaMaster, string pNombreTablaReplica, int pMaxOrdenEjecucion)
        //{
        //    BaseComunidadAD.TransferirFilasACola(pNombreTablaMaster, pNombreTablaReplica, pMaxOrdenEjecucion);
        //}

        //public void ActualizarEstadoCola(string pNombreTablaMaster, int pMaxOrdenEjecucion)
        //{
        //    BaseComunidadAD.ActualizarEstadoCola(pNombreTablaMaster, pMaxOrdenEjecucion);
        //}

        /// <summary>
        /// Elimina de la cola los elementos que han sido procesado exitósamente hace una semana
        /// </summary>
        public virtual void EliminarElementosColaProcesadosViejos() { }

        ///// <summary>
        ///// Elimina de la cola los elementos que han sido procesado exitósamente hace una semana
        ///// </summary>
        //public void EliminarElementosColaModificacionSearchViejos()
        //{
        //    BaseComunidadAD.EliminarElementosColaModificacionSearchViejos();
        //}

        /// <summary>
        /// Obtiene una lista de los buzones con correos pendientes en la cola de correo
        /// </summary>
        /// <returns></returns>
        public List<string> ObtenerBuzonesCorreo()
        {
            return BaseComunidadAD.ObtenerBuzonesCorreo();
        }

        /// <summary>
        /// Obtiene los elementos pendientes de la cola de correo
        /// </summary>
        /// <returns></returns>
        public List<Email> ObtenerColaCorreo()
        {
            return BaseComunidadAD.ObtenerColaCorreo();
        }

        /// <summary>
        /// Devuelve el ColaCorreo correspondiente al id dado por parametro
        /// </summary>
        /// <param name="pColaCorreoID">El Id del correo que queremos obtener</param>
        /// <returns></returns>
        public ColaCorreo ObtenerColaCorreoCorreoID(int pColaCorreoID)
        {
            return BaseComunidadAD.ObtenerColaCorreoCorreoID(pColaCorreoID);
        }

        /// <summary>
        /// Obtiene los elementos pendientes de la cola de correo para un buzon determinado
        /// </summary>
        /// <returns></returns>
        public List<Email> ObtenerColaCorreoBuzon(string pBuzon)
        {
            return BaseComunidadAD.ObtenerColaCorreoBuzon(pBuzon);
        }

        public List<Email> ObtenerColaCorreoEmailCorreoID(int pCorreoID)
        {
            return BaseComunidadAD.ObtenerColaCorreoEmailCorreoID(pCorreoID);
        }

        /// <summary>
        /// Obtiene true o false en función de si hay correos pendientes de enviar para el correo indicado
        /// </summary>
        /// <param name="pCorreoID">El id del correo que queremos enviar</param>
        /// <returns>True si hay correos pendientes de envío y false si no los hay</returns>
        public bool HayCorreosPendientesBuzon(int pCorreoID)
        {
            return BaseComunidadAD.HayCorreosPendientesBuzon(pCorreoID);
        }

        /// <summary>
        /// Modifica el estado del correo
        /// </summary>
        public void ModificarEstadoCorreo(int pCorreoID, string pEmail, short pEstado)
        {
            BaseComunidadAD.ModificarEstadoCorreo(pCorreoID, pEmail, pEstado);
        }

        /// <summary>
        /// Comprueba si el correo tiene filas fallidas o pendientes en ColaCorreoDestinatario
        /// </summary>
        /// <param name="pCorreoID">Identificador del correo</param>
        /// <returns>True si quedan correos pendientes de enviar o fallidos. False en caso contrario</returns>
        public bool ComprobarCorreosPendientesEnviar(int pCorreoID)
        {
            return BaseComunidadAD.ComprobarCorreosPendientesEnviar(pCorreoID);
        }

        /// <summary>
        /// Elimina la fila de ColaCorreo del correo
        /// </summary>
        /// <param name="pCorreoID">Identificador del correo</param>
        public void BorrarCorreo(int pCorreoID)
        {
            BaseComunidadAD.BorrarCorreo(pCorreoID);
        }

        /// <summary>
        /// Elimina las filas de correos procesadas correctamente
        /// </summary>
        /// <param name="pCorreoID">Identificador del correo</param>
        public void BorrarCorreosEnviadosCorrectamente(int pCorreoID)
        {
            BaseComunidadAD.BorrarCorreosEnviadosCorrectamente(pCorreoID);
        }
        #endregion

        #region Métodos de verificación de existencia de tablas

        /// <summary>
        /// Comprueba si existe la tabla de comunidades. Si no existe la crea 
        /// </summary>
        /// <param name="pCrearTablaSiNoExiste">Verdad si se debe crear la tabla en caso de que no exista</param>
        /// <param name="pTipoConsulta">Tipo de consulta que se va a realizar</param>
        /// <returns>Verdad si la tabla existe (o ha sido recién creada).</returns>
        public bool VerificarExisteTabla(TiposConsultaObtenerTags pTipoConsulta, bool pCrearTablaSiNoExiste)
        {
            return BaseComunidadAD.VerificarExisteTabla(pTipoConsulta, pCrearTablaSiNoExiste);
        }

        #endregion

        #region Métodos de actualización

        /// <summary>
        /// Genera un script con las inserciones y actualizaciones necesarias para actualizar la base de datos
        /// </summary>
        /// <param name="pDataSet">DataSet con los cambios</param>
        public string GuardarActualizacionInsercionMedianteScript(DataSet pDataSet)
        {
            return this.BaseComunidadAD.GuardarActualizacionInsercionMedianteScript(pDataSet, true);
        }

        #endregion

        #region Metodos de Freebase

        /// <summary>
        /// Comprueba de una lista de Guids de Freebase, cuáles NO estan ya en la tabla de Freebase de un proyecto
        /// </summary>
        /// <param name="pListaGuidFreebase">Lista de Guids de freebase que se quieren añadir</param>
        /// <returns>Lista de Guids de Freebase que NO estan ya agregados a la tabla de freebase</returns>
        public List<string> ComprobarSiGuidFreebaseEstaEnProyecto(List<string> pListaGuidFreebase)
        {
            return BaseComunidadAD.ComprobarSiGuidFreebaseEstaEnProyecto(pListaGuidFreebase);
        }

        //Dado un tag devuelve las filas de freebase que lo contienen
        public void ObtenerTagsRelacionadosFreebase(DataSet pDataSet, List<string> pListaTags)
        {
            BaseComunidadAD.ObtenerTagsRelacionadosFreebase(pDataSet, pListaTags);
        }

        //Dado una lista de tags devuelve las filas de freebase que contienen algun tag de la lista
        public void ObtenerEntidadFreebase(DataSet pDataSet, List<string> LTag)
        {
            BaseComunidadAD.ObtenerEntidadFreebase(pDataSet, LTag);
        }

        //Coge el campo del Dataset WikepediaID y lo transforma en la URI
        public string GeneraWikipediaURL(string infotabla)
        {


            string aux = infotabla.Substring(infotabla.LastIndexOf("/") + 1);

            string nuevoUri = "http://en.wikipedia.org/wiki/index.html?curid=" + aux;

            return nuevoUri;

        }

        //Coge el campo del Dataset Ruta y lo transforma en la URI
        public string GeneraFreeBaseURL(string infotabla)
        {

            if (infotabla.Contains(","))
            {
                infotabla = infotabla.Substring(0, infotabla.IndexOf(','));
            }
            string nuevoUri = "http://www.freebase.com/view" + infotabla;

            return nuevoUri;

        }

        /// <summary>
        /// Genera la URL del rdf de geonames
        /// </summary>
        /// <param name="pGuidFreebase">Campo GuidFreebase de la tabla GnossToFreebase</param>
        /// <returns></returns>
        public string GeneraGeonamesUrlParaRDF(string pRutaGeonames)
        {
            if (!pRutaGeonames.EndsWith("/"))
            {
                pRutaGeonames += "/";
            }
            return pRutaGeonames + "about.rdf";
        }

        /// <summary>
        /// Genera la URL del rdf de NYT
        /// </summary>
        /// <param name="pGuidFreebase">Campo GuidFreebase de la tabla GnossToFreebase</param>
        /// <returns></returns>
        public string GeneraNYTUrlParaRDF(string pGuidFreebase)
        {
            while (pGuidFreebase.EndsWith("/"))
            {
                pGuidFreebase = pGuidFreebase.Remove(pGuidFreebase.Length - 1);
            }
            return pGuidFreebase + ".rdf";
        }

        //Coge el campo del Dataset GUID y lo transforma en la URI para mostrar el RDF
        public string GeneraFreeBaseURLParaRDF(string infotabla)
        {

            //string aux= infotabla.Replace("/", ".");
            // aux=aux.Substring(1);

            string nuevoUri = "http://rdf.freebase.com/rdf" + infotabla;

            return nuevoUri;

        }

        public static string DownloadWebPage(string Url)
        {
            // Open a connection
            HttpWebRequest WebRequestObject = (HttpWebRequest)HttpWebRequest.Create(Url);

            // You can also specify additional header values like 
            // the user agent or the referer:
            WebRequestObject.UserAgent = ".NET Framework/2.0";
            WebRequestObject.Referer = "http://www.example.com/";

            // Request response:
            WebResponse Response = WebRequestObject.GetResponse();

            // Open data stream:
            Stream WebStream = Response.GetResponseStream();

            // Create reader object:
            StreamReader Reader = new StreamReader(WebStream);

            // Read the entire stream content:
            string PageContent = Reader.ReadToEnd();

            // Cleanup
            Reader.Close();
            WebStream.Close();
            Response.Close();

            return PageContent;
        }


        //Dado la URI del RDF obtener la URI de la imagen
        public string GeneraFreeBaseIm(string RDFURI)
        {

            //string aux= infotabla.Replace("/", ".");
            // aux=aux.Substring(1);               


            String RDFURI2 = DownloadWebPage(RDFURI);


            int i = RDFURI2.IndexOf("fb:common.topic.image rdf:resource=\"http://rdf.freebase.com/ns/m");
            // 
            if (i != -1)
            {



                string im2 = RDFURI2.Substring(i + 65, 7);
                return "http://img.freebase.com/api/trans/image_thumb/m/" + im2;

            }
            else return "No hay Imagen en este recurso" + RDFURI2;

            //string nuevoUri = "http://img.freebase.com/api/trans/image_thumb/m/" + ima;

            //return nuevoUri;


        }

        //<div about="http://sw-app.org/mic.xhtml#org" typeof="vcard:Organization">
        //<span property="vcard:street-address" datatype="xsd:string">
        //    Digital Enterprise Research Institute (DERI),<br />
        //    National University of Ireland, Galway<br />
        //</span>
        //<span rel="owl:sameAs" resource="http://dbpedia.org/resource/Digital_Enterprise_Research_Institute" />
        //</div>


        // <rdf:Description rdf:about="#William_Jefferson_Clinton">
        //  <owl:sameAs rdf:resource="#BillClinton"/>
        // </rdf:Description>

        #endregion

        #region Metodos de RecursosRelacionados

        public int ObtenerListaRecursosRelacionados(DataSet pDataSet, String Recurso, int inicio, int final, Guid pProyectoId)
        { return BaseComunidadAD.ObtenerListaRecursosRelacionados(pDataSet, Recurso, inicio, final, pProyectoId); }

        public int ObtenerListaRecursosRelacionadosOtrasComunidades(DataSet pDataSet, String Recurso, int inicio, int final, Guid pProyectoId)
        { return BaseComunidadAD.ObtenerListaRecursosRelacionadosOtrasComunidades(pDataSet, Recurso, inicio, final, pProyectoId); }

        //public void ObtenerRecursosMasRelevantesComunidad(DataSet pDataSet)
        //{ BaseComunidadAD.ObtenerRecursosMasRelevantesComunidad(pDataSet); }

        #endregion

        #region Actualizar

        /// <summary>
        /// Actualiza el estado de tags de la cola.
        /// </summary>
        /// <param name="pFila">Fila de la cola</param>
        public void ActualizarEstadoTagsCola(DataRow pFila)
        {
            BaseComunidadAD.ActualizarEstadoTagsCola(pFila);
        }

        /// <summary>
        /// Actualiza el estado de la cola.
        /// </summary>
        /// <param name="pFila">Fila de la cola</param>
        public void ActualizarEstadoCola(DataRow pFila)
        {
            BaseComunidadAD.ActualizarEstadoCola(pFila);
        }

        #endregion

        #region Métodos de Ultimos Recursos visitados

        /// <summary>
        /// Obtiene los últimos recursos visitados de un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto del que se quieren obtener los útlimos documentos visitados</param>
        /// <returns>Lista de Identificadores de los últimos documentos visitados</returns>
        public List<Guid> ObtenerUltimosRecursosVisitadosDeProyecto(Guid pProyectoID)
        {
            return BaseComunidadAD.ObtenerUltimosRecursosVisitadosDeProyecto(pProyectoID);
        }

        /// <summary>
        /// Actualiza los útlimos recursos visitados de un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pListaDocumentosID">Lista de los últimos recursos visitados en este proyecto</param>
        /// <param name="pCrearNuevaFila">Verdad si la fila no existe y hay que crearla</param>
        public void ActualizarUltimosRecursosVisitadosDeProyecto(Guid pProyectoID, List<Guid> pListaDocumentosID, bool pCrearNuevaFila)
        {
            if (pCrearNuevaFila)
            {
                try
                {
                    BaseComunidadAD.InsertarUltimosRecursosVisitadosDeProyecto(pProyectoID, pListaDocumentosID);
                }
                catch (Exception ex)
                {
                    mLoggingService.GuardarLogError(ex);

                    //Ha fallado al insertar, es posible que la fila ya exista, pruebo a actualizar
                    BaseComunidadAD.ActualizarUltimosRecursosVisitadosDeProyecto(pProyectoID, pListaDocumentosID);
                }
            }
            else
            {
                try
                {
                    BaseComunidadAD.ActualizarUltimosRecursosVisitadosDeProyecto(pProyectoID, pListaDocumentosID);
                }
                catch (Exception ex)
                {
                    mLoggingService.GuardarLogError(ex);

                    //Ha fallado al actualizar, es probable que la fila no exista, pruebo a crearla
                    BaseComunidadAD.InsertarUltimosRecursosVisitadosDeProyecto(pProyectoID, pListaDocumentosID);
                }
            }
        }

        #endregion

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene o establece si la comunidad sobre la que se hacen las búsquedas es privada
        /// </summary>
        public bool EsComunidadPrivada
        {
            get
            {
                return BaseComunidadAD.EsComunidadPrivada;
            }
            set
            {
                BaseComunidadAD.EsComunidadPrivada = value;
            }
        }

        /// <summary>
        /// Obtiene o establece si el usuario que está realizando la consulta ha hecho login.
        /// </summary>
        public bool EstaUsuarioConectado
        {
            get
            {
                return BaseComunidadAD.EstaUsuarioConectado;
            }
            set
            {
                BaseComunidadAD.EstaUsuarioConectado = value;
            }
        }

        /// <summary>
        /// AD de baseComunidad.
        /// </summary>
        protected BaseComunidadAD BaseComunidadAD
        {
            get
            {
                return (BaseComunidadAD)AD;
            }
            set
            {
                this.AD = value;
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
        ~BaseComunidadCN()
        {
            //Libero los recursos
            Dispose(false);
        }

        /// <summary>
        /// Libera los recursos
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            //impido que se finalice dos veces este objeto
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Libera los recursos
        /// </summary>
        /// <param name="disposing">Determina si se está llamando desde el Dispose()</param>
        protected void Dispose(bool disposing)
        {
            if (!disposed)
            {
                disposed = true;

                if (disposing)
                {
                    //Libero todos los recursos administrados que he añadido a esta clase
                    if (BaseComunidadAD != null)
                    {
                        BaseComunidadAD.Dispose();
                    }
                }
                BaseComunidadAD = null;
            }
        }

        #endregion
    }
}
