using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.Live;
using Es.Riam.Gnoss.AD.Live.Model;
using Es.Riam.Gnoss.RabbitMQ;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;


namespace Es.Riam.Gnoss.Logica.Live
{
    public class LiveUsuariosCN : BaseCN
    {
        #region Miembros

        private const string COLA_USUARIOS = "ColaUsuarios";
        private const string EXCHANGE = "";

        private LoggingService mLoggingService;
        private ConfigService mConfigService;

        #endregion

        #region Constructores

        /// <summary>
        /// Constructor a partir del fichero de configuración de base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuración de base de datos</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public LiveUsuariosCN(string pFicheroConfiguracionBD, EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base( entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication)
        {
            mLoggingService = loggingService;
            mConfigService = configService;
        }

        /// <summary>
        /// Constructor a partir del fichero de configuración de base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuración de base de datos</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public LiveUsuariosCN( EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base( entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication)
        {
            mLoggingService = loggingService;
            mConfigService = configService;

        }

        #endregion

        #region Metodos generales
        /// <summary>
        /// Inserta una fila en la cola del LIVE de usuarios a partir del contenido de la fila
        /// </summary>
        /// <param name="pProyectoid">Identificador del proyecto</param>
        /// <param name="pId">Identificador del elemento</param>
        /// <param name="pAccion">Acción</param>
        /// <param name="pTipo">Tipo de elemento</param>
        public void InsertarFilaEnColaUsuarios(Guid pProyectoid, Guid pId, int pAccion, int pTipo, string pInfoExtra)
        {
            InsertarFilaEnCola(pProyectoid, pId, pAccion, pTipo, pInfoExtra, "");
        }


        private void InsertarFilaEnCola(Guid pProyectoid, Guid pId, int pAccion, int pTipo, string pInfoExtra, string pNombreTabla)
        {
            LiveUsuariosDS liveUsuariosDS = new LiveUsuariosDS();
            LiveUsuariosDS.ColaUsuariosRow filaCola = liveUsuariosDS.ColaUsuarios.NewColaUsuariosRow();

            filaCola.ProyectoId = pProyectoid;
            filaCola.Id = pId;
            filaCola.Accion = pAccion;
            filaCola.Tipo = pTipo;
            filaCola.Fecha = DateTime.Now;
            filaCola.NumIntentos = 0;
            filaCola.Prioridad = 0;
            filaCola.InfoExtra = pInfoExtra;

            try
            {
                InsertarFilaEnColaUsuariosRabbitMQ(filaCola);
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, $"Fallo al insertar en Rabbit, ColaUsuarios:  {JsonConvert.SerializeObject(filaCola)}");
                //LiveUsuariosAD.InsertarFilaLiveEnColaUsuariosPorNombre(filaCola, pNombreTabla);
            }
        }

        public void InsertarFilaEnColaUsuariosRabbitMQ(LiveUsuariosDS.ColaUsuariosRow pFilaColaTagsUsuario)
        {
            
            if (!string.IsNullOrEmpty(mConfigService.ObtenerRabbitMQClient(RabbitMQClient.BD_SERVICIOS_WIN)))
            {
                using (RabbitMQClient rabbitMQ = new RabbitMQClient(RabbitMQClient.BD_SERVICIOS_WIN, COLA_USUARIOS, mLoggingService, mConfigService, EXCHANGE, COLA_USUARIOS))
                {
                    rabbitMQ.AgregarElementoACola(JsonConvert.SerializeObject(pFilaColaTagsUsuario.ItemArray));
                }
            }
        }

        #endregion
    }
}
