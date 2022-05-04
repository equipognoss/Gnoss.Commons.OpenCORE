using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Es.Riam.Gnoss.RabbitMQ
{
    public class RabbitMQAMQP : IAMQPClient, IDisposable
    {
        private IConnection mConexion;
        private RabbitMQClient mGestorRabbit;
        private IModel mChannel;
        private LoggingService mLoggingService;
        private ConfigService mConfigService;

        public RabbitMQAMQP(RabbitMQClient pGestorRabbit, LoggingService loggingService, ConfigService configService)
        {
            mConfigService = configService;
            mLoggingService = loggingService;
            mGestorRabbit = pGestorRabbit;
        }

        public int ContarElementosEnCola()
        {
            int numElementos = -1;

            if (Conexion != null)
            {
                IModel channel = Conexion.CreateModel();

                numElementos = (int)channel.MessageCount(mGestorRabbit.QueueName);

                channel.Close();
            }

            return numElementos;
        }

        public static void CerrarConexion(object pConnection)
        {
            if (pConnection != null && pConnection is IConnection)
            {
                IConnection conexionAbierta = (IConnection)pConnection;
                if (conexionAbierta.IsOpen)
                {
                    conexionAbierta.Close();
                }
            }
        }

        public void ObtenerElementosDeCola(RabbitMQClient.ReceivedDelegate receivedFunction, RabbitMQClient.ShutDownDelegate shutdownFunction)
        {
            try
            {
                IModel channel = Channel;
                channel.BasicQos(0, 1, false);

                channel.QueueDeclare(queue: mGestorRabbit.QueueName,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                if (!string.IsNullOrEmpty(mGestorRabbit.ExchangeName))
                {
                    string tipoExchange = "fanout";

                    if (!string.IsNullOrEmpty(mGestorRabbit.Routing))
                    {
                        tipoExchange = "direct";
                    }

                    channel.ExchangeDeclare(mGestorRabbit.ExchangeName, tipoExchange, true);

                    channel.QueueBind(queue: mGestorRabbit.QueueName,
                                        exchange: mGestorRabbit.ExchangeName,
                                        routingKey: mGestorRabbit.Routing);
                }

                EventingBasicConsumer eventingBasicConsumer = new EventingBasicConsumer(channel);

                
                eventingBasicConsumer.Received += (sender, basicDeliveryEventArgs) =>
                {
                    try
                    {
                        // Por si existen cosas de otro hilo, las elimino
                        //UtilPeticion.EliminarObjetosDeHilo(Thread.CurrentThread.ManagedThreadId);

                        IBasicProperties basicProperties = basicDeliveryEventArgs.BasicProperties;

                        string body = Encoding.UTF8.GetString(basicDeliveryEventArgs.Body.Span);

                        if (receivedFunction(body))
                        {
                            channel.BasicAck(basicDeliveryEventArgs.DeliveryTag, false);
                        }
                        else
                        {
                            //Si hay fallo ya se habra guardado el fallo en el log.
                            //Quitar de la cola
                            channel.BasicAck(basicDeliveryEventArgs.DeliveryTag, false);
                            //channel.BasicNack(basicDeliveryEventArgs.DeliveryTag, false, true);
                        }
                    }
                    catch (Exception ex)
                    {
                        channel.BasicNack(basicDeliveryEventArgs.DeliveryTag, false, true);
                        //channel.Close();
                        //Conexion.Close();
                        //shutdownFunction();
                        mLoggingService.GuardarLogError(ex);
                        throw;
                    }
                };

                eventingBasicConsumer.Shutdown += (sender, shutdownEventArgs) =>
                {
                    mLoggingService.GuardarLogError(shutdownEventArgs.ReplyText);
                    shutdownFunction();
                };

                channel.BasicConsume(mGestorRabbit.QueueName, false, eventingBasicConsumer);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void AgregarElementoACola(string message)
        {

            byte[] messageBytes = Encoding.UTF8.GetBytes(message);

            using (var channel = Conexion.CreateModel())
            {
                channel.ConfirmSelect();

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;


                if (!string.IsNullOrEmpty(mGestorRabbit.ExchangeName))
                {
                    string tipoExchange = "fanout";

                    if (!string.IsNullOrEmpty(mGestorRabbit.Routing))
                    {
                        tipoExchange = "direct";
                    }

                    channel.ExchangeDeclare(mGestorRabbit.ExchangeName, tipoExchange, true);
                }
                else
                {
                    channel.QueueDeclare(queue: mGestorRabbit.QueueName,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
                }

                channel.BasicPublish(exchange: mGestorRabbit.ExchangeName,
                                     routingKey: mGestorRabbit.QueueName,
                                     basicProperties: properties,
                                     body: messageBytes);
            }
        }

        /// <summary>
        /// Cierra la conexión de lectura para dejar de escuchar eventos
        /// </summary>
        public void CerrarConexionLectura()
        {
            if (mChannel != null)
            {
                mChannel.Close();
            }
        }

        public void Dispose()
        {
            if(mChannel != null)
            {
                mChannel.Close();
            }
            if(mConexion != null)
            {
                mConexion.Close();
            }
        }

        private IConnection Conexion
        {
            get
            {
                if (mConexion == null || !mConexion.IsOpen)
                {
                    Dictionary<string, object> listaConexiones = null;

                    string cadenaRabbit = mConfigService.ObtenerRabbitMQClient(mGestorRabbit.TipoCola);
                    if ((mConexion == null || !mConexion.IsOpen) && !string.IsNullOrEmpty(cadenaRabbit))
                    {
                        ConnectionFactory connectionFactory = new ConnectionFactory();
                        connectionFactory.Uri = new Uri(cadenaRabbit);
                        mConexion = connectionFactory.CreateConnection(RabbitMQClient.ClientName);

                        if (listaConexiones == null)
                        {
                            listaConexiones = new Dictionary<string, object>();
                        }

                        if (listaConexiones.ContainsKey(mGestorRabbit.TipoCola))
                        {
                            listaConexiones[mGestorRabbit.TipoCola] = mConexion;
                        }
                        else
                        {
                            listaConexiones.Add(mGestorRabbit.TipoCola, mConexion);
                        }
                    }
                }

                return mConexion;
            }
        }

        private IModel Channel
        {
            get
            {
                if (mChannel == null)
                {
                    mChannel = Conexion.CreateModel();
                }
                return mChannel;
            }
        }
    }
}
