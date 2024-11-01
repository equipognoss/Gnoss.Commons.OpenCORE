﻿using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.InterfacesOpenArchivos;
using System;
using System.Collections.Concurrent;

namespace Es.Riam.Gnoss.RabbitMQ
{
    public class RabbitMQClient : IDisposable
    {
        public const string BD_REPLICACION = "colaReplicacion";
        public const string BD_SERVICIOS_WIN = "colaServiciosWin";

        public delegate bool ReceivedDelegate(string s);

        public delegate void ShutDownDelegate();

        [ThreadStatic]
        private static string mClientName;

        private readonly string mTipoCola;

        private readonly string mQueueName;

        private readonly string mExchangeName;

        private readonly string mRouting;

        private static ConcurrentDictionary<string, ConcurrentDictionary<string, string>> mListaCadenasConexiones = new ConcurrentDictionary<string, ConcurrentDictionary<string, string>>();

        private readonly IAMQPClient Cliente;

        public RabbitMQClient(string pTipoCola, string pQueueName, LoggingService loggingService, ConfigService configService, string pExchangeName = "", string pRouting = "")
        {
            mTipoCola = pTipoCola;
            mQueueName = pQueueName;
            mExchangeName = pExchangeName;
            mRouting = pRouting;
            Cliente = new RabbitMQAMQP(this, loggingService, configService);
        }

        public int ContarElementosEnCola()
        {
            return Cliente.ContarElementosEnCola();
        }

        public void AgregarElementoACola(string message)
        {
            Cliente.AgregarElementoACola(message);
        }

        public void ObtenerElementosDeCola(ReceivedDelegate receivedFunction, ShutDownDelegate shutdownFunction)
        {
            Cliente.ObtenerElementosDeCola(receivedFunction, shutdownFunction);
        }

        public static bool HayConexionRabbit(string pFicheroConexion, string pTipoCola)
        {
            return mListaCadenasConexiones != null && mListaCadenasConexiones.ContainsKey(pFicheroConexion) && mListaCadenasConexiones[pFicheroConexion] != null && mListaCadenasConexiones[pFicheroConexion].ContainsKey(pTipoCola);
        }

        public void CerrarConexionLectura()
        {
            Cliente.CerrarConexionLectura();
        }

        public void Dispose()
        {
            Cliente.Dispose();
        }

        /// <summary>
        /// Comprueba si existe o no la cola indicada por parámetro en RabbitMQ
        /// </summary>
        /// <param name="pNombreCola">Nombre de la cola a comprobar si existe</param>
        /// <returns>true o false si existe o no la cola respectivamente</returns>
        public bool ExisteColaRabbit(string pNombreCola)
        {
            return Cliente.ExisteColaRabbit(pNombreCola);
        }

        public static string ClientName
        {
            get
            {
                return mClientName;
            }
            set
            {
                mClientName = value;
            }
        }

        public string TipoCola
        {
            get
            {
                return mTipoCola;
            }
        }

        public string QueueName
        {
            get
            {
                return mQueueName;
            }
        }

        public string ExchangeName
        {
            get
            {
                return mExchangeName;
            }
        }

        public string Routing
        {
            get
            {
                return mRouting;
            }
        }
    }
}
