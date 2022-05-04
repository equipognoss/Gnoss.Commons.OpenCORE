using Es.Riam.Gnoss.Servicios;
using Es.Riam.Gnoss.Util.General;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.Servicios
{
    public abstract class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        protected int mSegundosEsperaEntreTareaYTarea = 2;
        private bool mParadaSolicitada = false;
        protected IServiceScopeFactory ScopedFactory { get; }

        public Worker(ILogger<Worker> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            ScopedFactory = scopeFactory;
        }
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Worker starts");
            await base.StartAsync(cancellationToken);
        }

        protected abstract List<ControladorServicioGnoss> ObtenerControladores();
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var workers = new List<Task>();
            //while (!stoppingToken.IsCancellationRequested)
            {
                foreach (ControladorServicioGnoss suscriptor in ObtenerControladores())
                {
                    try
                    {
                        workers.Add(LanzarTarea(suscriptor, stoppingToken));

                        if (mSegundosEsperaEntreTareaYTarea > 0)
                        {
                            Thread.Sleep(mSegundosEsperaEntreTareaYTarea * 1000);
                        }
                    }
                    catch (Exception ex)
                    {
                     //   mLoggingService.GuardarLog(mLoggingService.DevolverCadenaError(ex, "1.0"));
                    }
                }
            }

            await Task.WhenAll(workers.ToArray());
        }

        private Task LanzarTarea(ControladorServicioGnoss pControlador, CancellationToken stoppingToken)
        {
            Task tarea = Task.Factory.StartNew(() => pControlador.EmpezarMantenimiento(), stoppingToken);
            // Si la tarea falla por una excepción no controlada, se ejecutará el método RelanzarTareaFallida
            //tarea.ContinueWith(DelegadoErrorEnTarea => RelanzarTareaFallida(pControlador, tarea, stoppingToken), TaskContinuationOptions.NotOnCanceled);

            return tarea;
        }

        private void RelanzarTareaFallida(ControladorServicioGnoss pControlador, Task pTareaFallida, CancellationToken stoppingToken)
        {
            //if (mTimerInactividad != null)
            //{
            //    GuardarLog($"Relanzando tarea fallida. El controlador '{pControlador.FicheroConfiguracionBDOriginal}' ha superado el tiempo de inactividad de '{mTimerInactividad.Interval / 1000}' segundos y va a reiniciarse.");
            //}
            //else
            {
                //GuardarLog($"Relanzando tarea fallida. El controlador '{pControlador.FicheroConfiguracionBDOriginal}' va a reiniciarse. No hay tiempo de inactividad configurado.");
            }
            if (!mParadaSolicitada)
            {

                if (pTareaFallida != null && pTareaFallida.IsFaulted)
                {
                    Exception ex = pTareaFallida.Exception;
                    if (ex != null)
                    {
                        if (ex.InnerException != null)
                        {
                            ex = ex.InnerException;
                        }
                      //  mLoggingService.GuardarLog(mLoggingService.DevolverCadenaError(ex, "1.0.0.0"));
                    }
                    Random random = new Random((int)DateTime.Now.Ticks);
                    int seconds = random.Next(5, 60);
                    //Espero 5 segundos y arranco de nuevo la misma tarea: 
                    Thread.Sleep(seconds * 1000);
                    ControladorServicioGnoss controladorClon = (ControladorServicioGnoss)pControlador.Clone();
                    LanzarTarea(controladorClon, stoppingToken);
                }      
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Worker stops");
            mParadaSolicitada = true;
            await base.StopAsync(cancellationToken);
        }
    }
}
