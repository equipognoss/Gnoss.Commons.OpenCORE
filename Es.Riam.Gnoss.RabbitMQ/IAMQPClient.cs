using System.Collections;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.RabbitMQ
{
    public interface IAMQPClient
    {
        void AgregarElementoACola(string message);
		void AgregarElementoAColaConReintentosExchange(string message, byte priority);
        IList<string> AgregarElementosAColaConReintentosExchange(IEnumerable<string> messages, byte priority);
        IList<string> AgregarElementosACola(IEnumerable<string> messages);
        void ObtenerElementosDeCola(RabbitMQClient.ReceivedDelegate receivedFunction, RabbitMQClient.ShutDownDelegate shutdownFunction);
        public void ObtenerElementosDeColaReintentos(RabbitMQClient.ReceivedDelegateRetry receivedFunction, RabbitMQClient.ShutDownDelegate shutdownFunction, string pErrorExchange);

        int ContarElementosEnCola();

        void CerrarConexionLectura();

        void Dispose();

        bool ExisteColaRabbit(string pNombreCola);
    }
}
