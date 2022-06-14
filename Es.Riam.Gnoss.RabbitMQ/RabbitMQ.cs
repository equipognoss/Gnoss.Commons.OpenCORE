using Es.Riam.Gnoss.Util.Configuracion;
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
        public static string mClientName = "";

        private string mTipoCola = "";

        private string mQueueName = "";

        private string mExchangeName = "";

        private string mRouting = "";

        private static ConcurrentDictionary<string, ConcurrentDictionary<string, string>> mListaCadenasConexiones = new ConcurrentDictionary<string, ConcurrentDictionary<string, string>>();

        private IAMQPClient Cliente;

        private static bool mEsAzureServiceBus;

        public RabbitMQClient(string pTipoCola, string pQueueName, LoggingService loggingService,ConfigService configService, string pExchangeName = "", string pRouting = "")
        {
            mTipoCola = pTipoCola;
            mQueueName = pQueueName;
            mExchangeName = pExchangeName;
            mRouting = pRouting;

            if (mEsAzureServiceBus)//Comprobar si es AzureServiceBus
            {
                Cliente = new AzureServiceBusAMQP(this, loggingService, configService);


                if (!string.IsNullOrEmpty(configService.ObtenerAzureServiceBusReintentos()))
                {
                    AzureServiceBusAMQP.Reintentos = int.Parse(configService.ObtenerAzureServiceBusReintentos());
                }
                else if (!string.IsNullOrEmpty(configService.ObtenerAzureServiceBusEspera()))
                {
                    AzureServiceBusAMQP.Espera = int.Parse(configService.ObtenerAzureServiceBusEspera());
                }

            }
            else
            {
                Cliente = new RabbitMQAMQP(this, loggingService, configService);
            }
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
