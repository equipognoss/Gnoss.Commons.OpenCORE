using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.RabbitMQ;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.InterfacesOpen;
using Es.Riam.Open.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace Es.Riam.Open
{
    public class PublishEventsOpen : IPublishEvents
    {
        #region Miembros

        public string mTipoCola = "direct";
        public string mQueueName = "ColaEventosInternos";
        public string mExchangeName = "";
        public string mRouting = "";
        public RabbitMQClient mColaEventosInternos;
        public bool publicacionONorOFF = true;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        #endregion

        #region Constructor

        public PublishEventsOpen(LoggingService pLoggingService, ConfigService pConfigService, ILogger<PublishEventsOpen> logger, ILoggerFactory loggerFactory)
        {
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            mColaEventosInternos = new RabbitMQClient(mTipoCola, mQueueName, pLoggingService, pConfigService, mLoggerFactory.CreateLogger<RabbitMQClient>(), mLoggerFactory, mExchangeName, mRouting);
        }

        #endregion

        #region Metodos

        /// <summary>
        /// Metodo encargardo de crear una cola agregando un elemento vacio
        /// </summary>
        public void CrearCola()
        {

        }
        /// <summary>
        /// Metodo encargado de añadir un Json con los datos del comentario a la cola
        /// </summary>
        /// <param name="pModelo">Modelo encargador de guardar los datos de los Comentarios</param>
        public void PublishComments(ModificarComentarios pModelo)
        {

        }

        /// <summary>
        /// Metodo encargado de añadir un Json con el tipo de accion en el recurso a la cola
        /// </summary>
        /// <param name="pModelo"></param>
        /// <param name="tipo">Modelo encargador de guardar los datos de los Recursos</param>
        public void PublishResource(PublicarModificarEliminarRecurso pModelo, string tipo)
        {

        }

        /// <summary>
        /// Metodo encargado de añadir un Json con los datos de los usuarios a la cola
        /// </summary>
        /// <param name="pModelo">Modelo encargador de guardar los datos de los Usuarios</param>
        public void PublishUser(ModificarUsuarios pModelo)
        {

        }
        #endregion
    }
}
