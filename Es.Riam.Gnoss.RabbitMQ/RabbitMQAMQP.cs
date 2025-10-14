using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Channels;

namespace Es.Riam.Gnoss.RabbitMQ
{
    public class RabbitMQAMQP : IAMQPClient, IDisposable
    {
        private IConnection mConexion;
        private RabbitMQClient mGestorRabbit;
        private IModel mChannel;
        private LoggingService mLoggingService;
        private ConfigService mConfigService;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        public RabbitMQAMQP(RabbitMQClient pGestorRabbit, LoggingService loggingService, ConfigService configService, ILogger<RabbitMQAMQP> logger, ILoggerFactory loggerFactory)
        {
            mConfigService = configService;
            mLoggingService = loggingService;
            mGestorRabbit = pGestorRabbit;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        public int ContarElementosEnCola()
        {
            int numElementos = -1;
            try
            {
                if (Conexion != null)
                {
                    IModel channel = Conexion.CreateModel();

                    numElementos = (int)channel.MessageCount(mGestorRabbit.QueueName);

                    channel.Close();
                }
            }
            catch(Exception ex)
            {
                mLoggingService.GuardarLogError(ex, $"Error en la conexion {Conexion.Endpoint}",mlogger);
                throw;
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
                //Pruebas Quorum
                                     //arguments: new Dictionary<string, object>() { { "x-queue-type", "quorum" } });

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
                            channel.BasicNack(basicDeliveryEventArgs.DeliveryTag, false, true);
                        }
                    }
                    catch (Exception ex)
                    {
                        channel.BasicNack(basicDeliveryEventArgs.DeliveryTag, false, true);
                        mLoggingService.GuardarLogError(ex, mlogger);
                        throw;
                    }
                };

                eventingBasicConsumer.Shutdown += (sender, shutdownEventArgs) =>
                {
                    mLoggingService.GuardarLogError(shutdownEventArgs.ReplyText, mlogger);
                    shutdownFunction();
                };

                channel.BasicConsume(mGestorRabbit.QueueName, false, eventingBasicConsumer);
            }
            catch
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

        public IList<string> AgregarElementosACola(IEnumerable<string> messages)
        {
            List<string> failedMessages = new List<string>();

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

                foreach(string message in messages)
                {
                    try
                    {
                        byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                        channel.BasicPublish(exchange: mGestorRabbit.ExchangeName,
                                         routingKey: mGestorRabbit.QueueName,
                                         basicProperties: properties,
                                         body: messageBytes);
                    }
                    catch
                    {
                        mLoggingService.GuardarLogError($"Error al encolar el mensaje: \n {message} en la cola {mGestorRabbit.QueueName}.",mlogger);
                        failedMessages.Add(message);
                    }
                }           
            }

            return failedMessages;
        }

        /// <summary>
        /// Comprueba si existe o no la cola indicada por parámetro en RabbitMQ
        /// </summary>
        /// <param name="pNombreCola">Nombre de la cola a comprobar si existe</param>
        /// <returns>true o false si existe o no la cola respectivamente</returns>
        public bool ExisteColaRabbit(string pNombreCola)
        {
            try
            {
                using (var channel = Conexion.CreateModel())
                {
                    channel.QueueDeclarePassive(pNombreCola);
                }
                return true;
            }
            catch 
            {
                return false;            
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
                        try
                        {
                            mConexion = connectionFactory.CreateConnection(RabbitMQClient.ClientName);
                        }
                        catch(Exception ex){
                            mLoggingService.GuardarLogError($"Error al crear la conexion: {cadenaRabbit}", mlogger);
                            throw;
                        }
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
