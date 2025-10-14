using Es.Riam.Gnoss.FileManager;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.InterfacesOpenArchivos;
using Azure.Messaging.ServiceBus;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Es.Riam.Gnoss.RabbitMQ
{
    public class AzureServiceBusAMQP : IAMQPClient
    {
        private ServiceBusClient mQueueClient;
        private ServiceBusSender mSender;
        private ServiceBusProcessor mProcessor;
        private RabbitMQClient mGestorRabbit;
        private LoggingService mLoggingService;
        private RabbitMQClient.ReceivedDelegate mReceivedDelegate;
        private GestionArchivos mGestorArchivos;
        private string mAzureStorageConnectionString;
        private readonly ConfigService mConfigService;
        private static string mComienzoFichero;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;

        /// <summary>
        /// Numero de Reintentos para obtener el fichero.
        /// </summary>
        public static int Reintentos = 5;
        /// <summary>
        /// Numero de segundos entre intento e intento.
        /// </summary>
        public static int Espera = 1;

        /// <summary>
        /// Almacena la ruta de los ficheros
        /// </summary>
        private static string mRutaFicheros = null;

        public AzureServiceBusAMQP(RabbitMQClient pGestorRabbit, LoggingService pLoggingService , ConfigService configService, ILogger<AzureServiceBusAMQP> logger, ILoggerFactory loggerFactory)
        {
            mGestorRabbit = pGestorRabbit;
            mLoggingService = pLoggingService;
            mConfigService = configService;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            mAzureStorageConnectionString = mConfigService.ObtenerAzureStorageConnectionString();

            //mRutaFicheros = System.Web.Hosting.HostingEnvironment.MapPath("~/" + "serviceBUS");
            
            mRutaFicheros = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "serviceBUS");

            mComienzoFichero = "[[FileMessage]]";
            if (!string.IsNullOrEmpty(mAzureStorageConnectionString))
            {
                mAzureStorageConnectionString += "/" + "serviceBUS";
                mGestorArchivos = new GestionArchivos(mLoggingService, null,mLoggerFactory.CreateLogger<GestionArchivos>(),mLoggerFactory, pRutaArchivos: mRutaFicheros,pAzureStorageConnectionString: mAzureStorageConnectionString);
            }
            var conexiones = mConfigService.ObtenerRabbitMQClient(mGestorRabbit.TipoCola);

            mQueueClient = new ServiceBusClient(conexiones);
            mLoggingService.AgregarEntrada($"Se ha creado el cliente");
            mSender = mQueueClient.CreateSender(mGestorRabbit.QueueName);
            mLoggingService.AgregarEntrada($"Se ha creado el sender correctamente hacia la cola {mGestorRabbit.QueueName}");
        }

        public void AgregarElementoACola(string message)
        {
            //AgregarElementoAColaAsync(message).GetAwaiter().GetResult();
            try
            {
                //AgregarElementoAColaAsync(message).GetAwaiter().GetResult();
                //Desarrollo del Service BUS de AZURE.
                ServiceBusMessage mensaje = new ServiceBusMessage(message);
                mLoggingService.AgregarEntrada("Mensaje creado");
                mLoggingService.AgregarEntrada("Envio a Azure Service Bus");
                // Enviamos mensaje a la cola
                mSender.SendMessageAsync(mensaje).GetAwaiter().GetResult();
                mLoggingService.AgregarEntrada("Enviado a Azure Service Bus");
            }
            catch (Exception exception)
            {
                mLoggingService.GuardarLogError(exception, $"No se pudo replicar en {mGestorRabbit?.QueueName}: {message}",mlogger);
            }
        }

        public IList<string> AgregarElementosACola(IEnumerable<string> messages)
        {            
            try
            {
                //Desarrollo del Service BUS de AZURE.
                List<string> mensajesFallidos = new List<string>();

                foreach(string message in messages)
                {
                    byte[] mensajeBytes = Encoding.UTF8.GetBytes(message);
                    mLoggingService.AgregarEntrada($"Convertido mensaje a bytes: {mensajeBytes.Length} bytes");
					ServiceBusMessage mensaje = null;
                    //Modificacion para que sea un byte
                    if (mensajeBytes.Length > 256 * 1024)
                    {
                        mLoggingService.AgregarEntrada("Mensaje superior a 256 Kb");
                        Guid guidFichero = Guid.NewGuid();
                        string nombreFichero = $"{guidFichero}_{DateTime.Now.ToString("yyyy-MM-dd")}";
                        mGestorArchivos.CrearFicheroFisicoSinEncriptar("", $"{nombreFichero}.txt", mensajeBytes);
                        mLoggingService.AgregarEntrada("Fichero creado");
                        mensaje = new ServiceBusMessage(Encoding.UTF8.GetBytes($"{mComienzoFichero}{nombreFichero}"));
                        mLoggingService.AgregarEntrada("Mensaje creado");
                    }
                    else
                    {
                        // Creamos un mensaje a enviar a la cola
                        mensaje = new ServiceBusMessage(Encoding.UTF8.GetBytes(message));
                        mLoggingService.AgregarEntrada("Mensaje creado");
                    }
                    try
                    {
                        mLoggingService.AgregarEntrada("Envio a Azure Service Bus");
                        // Enviamos mensaje a la cola
                        mSender.SendMessageAsync(mensaje).GetAwaiter().GetResult();
                        mLoggingService.AgregarEntrada("Enviado a Azure Service Bus");
                    }
                    catch
                    {
                        mLoggingService.GuardarLogError($"Error al enviar el mensaje hacia Azure\n Mensaje: {mensaje}",mlogger);
                        mensajesFallidos.Add(message);
                    }
                }
                return mensajesFallidos;
            }
            catch (Exception exception)
            {
                mLoggingService.GuardarLogError(exception, $"No se pudo replicar en {mGestorRabbit?.QueueName}",mlogger);
                return messages.ToList();
            }
        }

        public int ContarElementosEnCola()
        {
            return 0;
        }

        public void ObtenerElementosDeCola(RabbitMQClient.ReceivedDelegate receivedFunction, RabbitMQClient.ShutDownDelegate shutdownFunction)
        {
            ServiceBusProcessorOptions options = new ServiceBusProcessorOptions()
            {
                MaxConcurrentCalls = 1,
                AutoCompleteMessages = false
            };
            mReceivedDelegate = receivedFunction;
            ServiceBusProcessor processor = mQueueClient.CreateProcessor(queueName: mGestorRabbit.QueueName, options);
            processor.ProcessMessageAsync += ProcessMessageAsync;
            processor.ProcessErrorAsync += ExceptionReceivedHandler;
            processor.StartProcessingAsync();
        }

        async Task ProcessMessageAsync(ProcessMessageEventArgs args)
        {
            try
            {
                //Desarrollo del Service BUS de AZURE.
                string mensaje = Encoding.UTF8.GetString(args.Message.Body);
                //bool esFichero = false;
                string nombreFichero = "";
                if (mensaje.StartsWith(mComienzoFichero))
                {
                    //esFichero = true;
                    nombreFichero = mensaje.Replace(mComienzoFichero, "") + ".txt";
                    mensaje = "";
                    mensaje = ObtenerFichero(nombreFichero);
                }

                if (mReceivedDelegate(mensaje))
                {
                    //Eliminacion Fichero.
                    //if (esFichero) { BorrarFichero(nombreFichero); }

                    await args.CompleteMessageAsync(args.Message);
                }
                else
                {
                    await args.AbandonMessageAsync(args.Message);
                }
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, mlogger);
                await args.AbandonMessageAsync(args.Message);
                throw;
            }
        }

        private void BorrarFichero(string nombreFichero)
        {
            if (!string.IsNullOrEmpty(nombreFichero))
            {
                mGestorArchivos.EliminarFicheroFisico("", nombreFichero);
            }
        }

        private string ObtenerFichero(string mensaje)
        {
            string textoMensaje = "";
            int numReintentos = 0;

            while (Reintentos > numReintentos)
            {
                if (!mGestorArchivos.ComprobarExisteArchivo("", mensaje).Result)
                {
                    Thread.Sleep(Espera * 1000);
                    numReintentos++;
                }
                else
                {
                    byte[] fichero = mGestorArchivos.DescargarFicheroSinEncriptar("", mensaje).Result;
                    textoMensaje = Encoding.UTF8.GetString(fichero);
                    break;
                }
            }
            return textoMensaje;
        }

        static Task ExceptionReceivedHandler(ProcessErrorEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine($"El manejador encontro una excepcion {exceptionReceivedEventArgs.Exception}.");
            //var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            Console.WriteLine($"- Endpoint: {exceptionReceivedEventArgs.FullyQualifiedNamespace}");
            Console.WriteLine($"- Entity Path: {exceptionReceivedEventArgs.EntityPath}");
            Console.WriteLine($"- Exceuting Action: {exceptionReceivedEventArgs.ErrorSource}");
            return Task.CompletedTask;
        }

        public static void CerrarConexion(object pConnection)
        {
            if (pConnection is ServiceBusClient)
            {
                ((ServiceBusClient)pConnection).DisposeAsync().GetAwaiter().GetResult();
            }
        }

        public void CerrarConexionLectura()
        {
            if (mQueueClient != null)
            {
                mQueueClient.DisposeAsync().GetAwaiter().GetResult();
            }
        }

        public void Dispose()
        {
            if (mQueueClient != null)
            {
                mQueueClient.DisposeAsync().GetAwaiter().GetResult();
            }
        }

        public bool ExisteColaRabbit(string pNombreCola)
        {
            throw new NotImplementedException();
        }
    }
}