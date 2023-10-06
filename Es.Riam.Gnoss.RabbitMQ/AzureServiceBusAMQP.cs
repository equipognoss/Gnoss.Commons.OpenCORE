using Es.Riam.Gnoss.FileManager;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.InterfacesOpenArchivos;
using Microsoft.Azure.ServiceBus;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.RabbitMQ
{
    public class AzureServiceBusAMQP : IAMQPClient
    {
        private IQueueClient mQueueClient;
        private RabbitMQClient mGestorRabbit;
        private LoggingService mLoggingService;
        private RabbitMQClient.ReceivedDelegate mReceivedDelegate;
        private GestionArchivos mGestorArchivos;
        private string mAzureStorageConnectionString;
        private readonly ConfigService mConfigService;
        private static string mComienzoFichero;

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

        public AzureServiceBusAMQP(RabbitMQClient pGestorRabbit, LoggingService pLoggingService , ConfigService configService)
        {
            mGestorRabbit = pGestorRabbit;
            mLoggingService = pLoggingService;
            mConfigService = configService;
            
            mAzureStorageConnectionString = mConfigService.ObtenerAzureStorageConnectionString();

            //mRutaFicheros = System.Web.Hosting.HostingEnvironment.MapPath("~/" + "serviceBUS");
            
            mRutaFicheros = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "serviceBUS");

            mComienzoFichero = "[[FileMessage]]";
            if (!string.IsNullOrEmpty(mAzureStorageConnectionString))
            {
                mAzureStorageConnectionString += "/" + "serviceBUS";
                mGestorArchivos = new GestionArchivos(mLoggingService, null, pRutaArchivos: mRutaFicheros, pAzureStorageConnectionString: mAzureStorageConnectionString);
            }
            var conexiones = mConfigService.ObtenerRabbitMQClient(mGestorRabbit.TipoCola);

            mQueueClient = new QueueClient(conexiones, mGestorRabbit.QueueName);
            
        }

        public void AgregarElementoACola(string message)
        {
            //AgregarElementoAColaAsync(message).GetAwaiter().GetResult();
            try
            {
                //AgregarElementoAColaAsync(message).GetAwaiter().GetResult();
                //Desarrollo del Service BUS de AZURE.
                byte[] mensajeBytes = Encoding.UTF8.GetBytes(message);
                mLoggingService.AgregarEntrada($"Convertido mensaje a bytes: {mensajeBytes.Length} bytes");
                Message mensaje = null;
                //Modificacion para que sea un byte
                if (mensajeBytes.Length > 256 * 1024)
                {
                    mLoggingService.AgregarEntrada("Mensaje superior a 256 Kb");
                    Guid guidFichero = Guid.NewGuid();
                    string nombreFichero = $"{guidFichero}_{DateTime.Now.ToString("yyyy-MM-dd")}";
                    mGestorArchivos.CrearFicheroFisicoSinEncriptar("", $"{nombreFichero}.txt", mensajeBytes);
                    mLoggingService.AgregarEntrada("Fichero creado");
                    mensaje = new Message(Encoding.UTF8.GetBytes($"{mComienzoFichero}{nombreFichero}"));
                    mLoggingService.AgregarEntrada("Mensaje creado");
                }
                else
                {
                    // Creamos un mensaje a enviar a la cola
                    mensaje = new Message(Encoding.UTF8.GetBytes(message));
                    mLoggingService.AgregarEntrada("Mensaje creado");
                }

                mLoggingService.AgregarEntrada("Envio a Azure Service Bus");
                // Enviamos mensaje a la cola
                mQueueClient.SendAsync(mensaje).GetAwaiter().GetResult();
                mLoggingService.AgregarEntrada("Enviado a Azure Service Bus");

            }
            catch (Exception exception)
            {
                mLoggingService.GuardarLogError(exception, $"No se pudo replicar en {mQueueClient?.QueueName}: {message}");
            }
        }

        public int ContarElementosEnCola()
        {
            return 0;
        }

        public void ObtenerElementosDeCola(RabbitMQClient.ReceivedDelegate receivedFunction, RabbitMQClient.ShutDownDelegate shutdownFunction)
        {
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };
            mReceivedDelegate = receivedFunction;
            mQueueClient.RegisterMessageHandler(ProcessMessageAsync, messageHandlerOptions);

        }

        async Task ProcessMessageAsync(Message message, CancellationToken token)
        {
            try
            {
                //Desarrollo del Service BUS de AZURE.
                string mensaje = Encoding.UTF8.GetString(message.Body);
                byte[] mensajeByte = message.Body;
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
                    await mQueueClient.CompleteAsync(message.SystemProperties.LockToken);
                }
                else
                {
                    await mQueueClient.AbandonAsync(message.SystemProperties.LockToken);
                }
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex);
                await mQueueClient.AbandonAsync(message.SystemProperties.LockToken);
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

        static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {

            Console.WriteLine($"El manejador encontro una excepcion {exceptionReceivedEventArgs.Exception}.");
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            Console.WriteLine($"- Endpoint: {context.Endpoint}");
            Console.WriteLine($"- Entity Path: {context.EntityPath}");
            Console.WriteLine($"- Exceuting Action: {context.Action}");
            return Task.CompletedTask;
        }

        public static void CerrarConexion(object pConnection)
        {
            if (pConnection is IQueueClient)
            {
                ((IQueueClient)pConnection).CloseAsync().GetAwaiter().GetResult();
            }
        }

        public void CerrarConexionLectura()
        {
            if (mQueueClient != null)
            {
                mQueueClient.CloseAsync().GetAwaiter().GetResult();
            }
        }

        public void Dispose()
        {
            if (mQueueClient != null)
            {
                mQueueClient.CloseAsync().GetAwaiter().GetResult();
            }
        }

        public bool ExisteColaRabbit(string pNombreCola)
        {
            throw new NotImplementedException();
        }
    }
}