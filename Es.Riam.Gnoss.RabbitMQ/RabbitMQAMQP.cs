using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.RabbitMQ
{
    public class RabbitMQAMQP : IAMQPClient, IDisposable
    {
        private const int ESPERA_RECONEXION_INICIAL_SEGUNDOS = 5;
        private const int ESPERA_RECONEXION_MAXIMA_SEGUNDOS = 60;

        private IConnection mConexion;
        private RabbitMQClient mGestorRabbit;
        private IModel mChannel;
        private string mConsumerTag;
        private LoggingService mLoggingService;
        private ConfigService mConfigService;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;

        private readonly CancellationTokenSource mCts = new CancellationTokenSource();
        private readonly object mReconexionLock = new object();
        private bool mReconectando;
        private bool mDisposed;
        private Action mAccionSuscripcion;

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
            IniciarConsumoConReintentos(() => SuscribirConsumer(receivedFunction, shutdownFunction));
        }

        private void SuscribirConsumer(RabbitMQClient.ReceivedDelegate receivedFunction, RabbitMQClient.ShutDownDelegate shutdownFunction)
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
                NotificarShutdownYReconectar(shutdownFunction);
            };

            mConsumerTag = channel.BasicConsume(mGestorRabbit.QueueName, false, eventingBasicConsumer);
        }

        public void ObtenerElementosDeColaReintentos(RabbitMQClient.ReceivedDelegateRetry receivedFunction, RabbitMQClient.ShutDownDelegate shutdownFunction, string pErrorExchange)
        {
            IniciarConsumoConReintentos(() => SuscribirConsumerConReintentos(receivedFunction, shutdownFunction, pErrorExchange));
        }

        private void SuscribirConsumerConReintentos(RabbitMQClient.ReceivedDelegateRetry receivedFunction, RabbitMQClient.ShutDownDelegate shutdownFunction, string pErrorExchange)
        {
            IModel channel = Channel;
            channel.BasicQos(0, 1, false);

            channel.QueueBind(queue: mGestorRabbit.QueueName,
                                    exchange: mGestorRabbit.ExchangeName,
                                    routingKey: mGestorRabbit.Routing);

            EventingBasicConsumer eventingBasicConsumer = new EventingBasicConsumer(channel);


            eventingBasicConsumer.Received += (sender, basicDeliveryEventArgs) =>
            {
                try
                {
                    // Por si existen cosas de otro hilo, las elimino
                    //UtilPeticion.EliminarObjetosDeHilo(Thread.CurrentThread.ManagedThreadId);

                    IBasicProperties basicProperties = basicDeliveryEventArgs.BasicProperties;

                    string body = Encoding.UTF8.GetString(basicDeliveryEventArgs.Body.Span);
                    int retryCount = GetRetryCount(basicDeliveryEventArgs);
                    bool procesadoCorrecto = receivedFunction(body, retryCount);
                    if(procesadoCorrecto)
                    {
                        channel.BasicAck(basicDeliveryEventArgs.DeliveryTag, false);
                    }
                    else if (!procesadoCorrecto && retryCount > 3)
                    {
                        if (!string.IsNullOrEmpty(pErrorExchange))
                        {
                            SendToErrorQueue(basicDeliveryEventArgs, body, pErrorExchange);
                        }
                        channel.BasicAck(basicDeliveryEventArgs.DeliveryTag, false);
                    }
                    else
                    {
                        channel.BasicNack(basicDeliveryEventArgs.DeliveryTag, false, false);
                    }
                }
                catch (Exception ex)
                {
                    channel.BasicNack(basicDeliveryEventArgs.DeliveryTag, false, false);
                    mLoggingService.GuardarLogError(ex, mlogger);
                    throw;
                }
            };

            eventingBasicConsumer.Shutdown += (sender, shutdownEventArgs) =>
            {
                mLoggingService.GuardarLogError(shutdownEventArgs.ReplyText, mlogger);
                NotificarShutdownYReconectar(shutdownFunction);
            };

            mConsumerTag = channel.BasicConsume(mGestorRabbit.QueueName, false, eventingBasicConsumer);
        }

        /// <summary>
        /// Notifica al worker el corte de conexión (informativo, p. ej. para logging/métricas) y reconecta
        /// el consumer internamente. La reconexión ya no depende de que el worker reaccione al aviso.
        /// </summary>
        private void NotificarShutdownYReconectar(RabbitMQClient.ShutDownDelegate shutdownFunction)
        {
            try
            {
                shutdownFunction?.Invoke();
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, mlogger);
            }

            IniciarConsumoConReintentos(mAccionSuscripcion);
        }

        /// <summary>
        /// Mantiene un consumer activo en la cola mientras esta instancia exista: si la suscripción falla
        /// (broker caído al arrancar) o se cae en caliente, reintenta en segundo plano con backoff exponencial
        /// (5s -> 60s) en vez de propagar la excepción una única vez al llamador.
        /// </summary>
        private void IniciarConsumoConReintentos(Action accionSuscripcion)
        {
            mAccionSuscripcion = accionSuscripcion;

            lock (mReconexionLock)
            {
                if (mDisposed || mReconectando)
                {
                    return;
                }
                mReconectando = true;
            }

            Task.Run(() => BucleReconexion(accionSuscripcion));
        }

        private void BucleReconexion(Action accionSuscripcion)
        {
            int esperaSegundos = ESPERA_RECONEXION_INICIAL_SEGUNDOS;

            while (!mCts.IsCancellationRequested && !mDisposed)
            {
                try
                {
                    accionSuscripcion();
                    break;
                }
                catch (Exception ex)
                {
                    mLoggingService.GuardarLogError(ex, $"No se ha podido conectar/suscribir el consumer de RabbitMQ para la cola '{mGestorRabbit.QueueName}'. Reintentando en {esperaSegundos}s.", mlogger);

                    mCts.Token.WaitHandle.WaitOne(TimeSpan.FromSeconds(esperaSegundos));
                    esperaSegundos = Math.Min(esperaSegundos * 2, ESPERA_RECONEXION_MAXIMA_SEGUNDOS);
                }
            }

            lock (mReconexionLock)
            {
                mReconectando = false;
            }
        }

        private void SendToErrorQueue(BasicDeliverEventArgs ea, string message, string pErrorExchange)
        {
            IModel channel = Channel;
            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.Headers = new Dictionary<string, object>
            {
                { "original-routing-key", ea.RoutingKey },
                { "error-time", DateTime.UtcNow.ToString("O") },
                { "retry-count", GetRetryCount(ea) }
            };

            channel.BasicPublish(
                exchange: pErrorExchange,
                routingKey: "",
                basicProperties: properties,
                body: ea.Body
            );
        }

        private int GetRetryCount(BasicDeliverEventArgs pEa)
        {
            if (pEa.BasicProperties.Headers != null)
            {
                // Intentar obtener el contador de x-death
                if (pEa.BasicProperties.Headers.TryGetValue("x-death", out var xDeathObj))
                {
                    if (xDeathObj is List<object> xDeathList && xDeathList.Count > 0)
                    {
                        if (xDeathList[0] is Dictionary<string, object> deathInfo)
                        {
                            if (deathInfo.TryGetValue("count", out var countObj))
                            {
                                return Convert.ToInt32(countObj);
                            }
                        }
                    }
                }
            }
            return 0;
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

		public void AgregarElementoAColaConReintentosExchange(string message, byte priority)
		{

			byte[] messageBytes = Encoding.UTF8.GetBytes(message);
			using (var channel = Conexion.CreateModel())
			{
				channel.ConfirmSelect();

				var properties = channel.CreateBasicProperties();
				properties.Persistent = true;
                properties.Priority = priority;

				if (!string.IsNullOrEmpty(mGestorRabbit.ExchangeName))
				{
					string tipoExchange = "fanout";

					if (!string.IsNullOrEmpty(mGestorRabbit.Routing))
					{
						tipoExchange = "topic";
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

        public IList<string> AgregarElementosAColaConReintentosExchange(IEnumerable<string> messages, byte priority)
        {
            List<string> failedMessages = new List<string>();
            using (var channel = Conexion.CreateModel())
            {
                channel.ConfirmSelect();

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;
                properties.Priority = priority;

                if (!string.IsNullOrEmpty(mGestorRabbit.ExchangeName))
                {
                    string tipoExchange = "fanout";

                    if (!string.IsNullOrEmpty(mGestorRabbit.Routing))
                    {
                        tipoExchange = "topic";
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
                foreach (string message in messages)
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
                        mLoggingService.GuardarLogError($"Error al encolar el mensaje: \n {message} en la cola {mGestorRabbit.QueueName}.", mlogger);
                        failedMessages.Add(message);
                    }
                }
            }
            return failedMessages;
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
            lock (mReconexionLock)
            {
                mDisposed = true;
            }
            mCts.Cancel();

            if (mChannel != null)
            {
                mChannel.Close();
            }
        }

        public void Dispose()
        {
            lock (mReconexionLock)
            {
                mDisposed = true;
            }
            mCts.Cancel();

            if (mChannel != null)
            {
                try
                {
                    if (mChannel.IsOpen)
                    {
                        if (!string.IsNullOrEmpty(mConsumerTag))
                        {
                            mChannel.BasicCancel(mConsumerTag);
                        }
                        mChannel.Close();
                    }
                }
                catch (Exception ex)
                {
                    mLoggingService.GuardarLogError(ex, mlogger);
                }
                mChannel = null;
            }
            if (mConexion != null)
            {
                try
                {
                    if (mConexion.IsOpen)
                    {
                        mConexion.Close();
                    }
                }
                catch (Exception ex)
                {
                    mLoggingService.GuardarLogError(ex, mlogger);
                }
                mConexion = null;
            }
        }

        private IConnection Conexion
        {
            get
            {
                if (mConexion == null || !mConexion.IsOpen)
                {
                    string cadenaRabbit = mConfigService.ObtenerRabbitMQClient(mGestorRabbit.TipoCola);
                    if ((mConexion == null || !mConexion.IsOpen) && !string.IsNullOrEmpty(cadenaRabbit))
                    {
                        ConnectionFactory connectionFactory = new ConnectionFactory();
                        connectionFactory.Uri = new Uri(cadenaRabbit);
                        // La reconexión la gestiona IniciarConsumoConReintentos/BucleReconexion; si se deja la
                        // auto-recovery de la librería activa, compite con ese bucle y duplica consumers tras
                        // un corte del broker (ver docs/eficiencia-recursos/plan-fix-rabbitmq-doble-consumer-y-reconexion-arranque.md).
                        connectionFactory.AutomaticRecoveryEnabled = false;
                        try
                        {
                            mConexion = connectionFactory.CreateConnection(RabbitMQClient.ClientName);
                        }
                        catch(Exception ex){
                            mLoggingService.GuardarLogError($"Error al crear la conexion: {cadenaRabbit}", mlogger);
                            throw;
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
                if (mChannel == null || !mChannel.IsOpen)
                {
                    mChannel = Conexion.CreateModel();
                }
                return mChannel;
            }
        }
    }
}
