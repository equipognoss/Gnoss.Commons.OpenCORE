namespace Es.Riam.Gnoss.RabbitMQ
{
    public interface IAMQPClient
    {
        void AgregarElementoACola(string message);

        void ObtenerElementosDeCola(RabbitMQClient.ReceivedDelegate receivedFunction, RabbitMQClient.ShutDownDelegate shutdownFunction);

        int ContarElementosEnCola();

        void CerrarConexionLectura();

        void Dispose();

        bool ExisteColaRabbit(string pNombreCola);
    }
}
