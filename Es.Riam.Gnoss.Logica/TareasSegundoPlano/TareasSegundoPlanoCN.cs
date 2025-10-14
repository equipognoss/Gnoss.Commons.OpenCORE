using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.Documentacion;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.TareasSegundoPlano;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.Logica.TareasSegundoPlano
{
    public class TareasSegundoPlanoCN : BaseCN, IDisposable
    {
        private LoggingService mLoggingService;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        #region Constructores

        /// <summary>
        /// Constructor para DocumentacionCN
        /// </summary>
        public TareasSegundoPlanoCN(EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<TareasSegundoPlanoCN> logger, ILoggerFactory loggerFactory)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        {
            mLoggingService = loggingService;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            this.TareasSegundoPlanoAD = new TareasSegundoPlanoAD(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<TareasSegundoPlanoAD>(), mLoggerFactory);
        }

        #endregion

        public Guid NuevaTarea(Guid pProyectoId, Guid pOrganizacionId, string pTipo, string pNombre, DateTime pFechaInicio, int pEventosTotales)
        {
            return TareasSegundoPlanoAD.NuevaTarea(pProyectoId, pOrganizacionId, pTipo, pNombre, pFechaInicio, pEventosTotales);
        }
        public List<AD.TareasSegundoPlano.TareasSegundoPlano> TareasDeProyecto(Guid pProyectoId)
        {
            return TareasSegundoPlanoAD.TareasDeProyecto(pProyectoId);
        }
        public void ActualizarEstado(Guid pTareaId, EstadoTarea pEstado)
        {
            TareasSegundoPlanoAD.ActualizarEstado(pTareaId, pEstado);
        }
        public void VaciarTablaDeProyecto(Guid pProyectoId)
        {
            TareasSegundoPlanoAD.VaciarTablaDeProyecto(pProyectoId);
        }



        #region Dispose

        /// <summary>
        /// Determina si está disposed
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Destructor
        /// </summary>
        ~TareasSegundoPlanoCN()
        {
            //Libero los recursos
            Dispose(false);
        }

        /// <summary>
        /// Libera los recursos
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            //impido que se finalice dos veces este objeto
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Libera los recursos
        /// </summary>
        /// <param name="disposing">Determina si se está llamando desde el Dispose()</param>
        protected void Dispose(bool disposing)
        {
            if (!disposed)
            {
                disposed = true;
                //Libero todos los recursos administrados que he añadido a esta clase
                if (TareasSegundoPlanoAD != null)
                {
                    TareasSegundoPlanoAD.Dispose();
                }
                TareasSegundoPlanoAD = null;
            }
        }

        #endregion

        #region Propiedades

        private TareasSegundoPlanoAD TareasSegundoPlanoAD
        {
            get
            {
                return (TareasSegundoPlanoAD)AD;
            }
            set
            {
                this.AD = value;
            }
        }

        #endregion
    }
}
