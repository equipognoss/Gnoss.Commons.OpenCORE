using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.Asistentes;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Asistente;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Logica.Asistentes
{
    public class AsistenteCN : BaseCN, IDisposable
    {

        #region Miembros

        private EntityContext mEntityContext;
        private LoggingService mLoggingService;
        private ConfigService mConfigService;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;

        #endregion

        public AsistenteCN(EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<AsistenteCN> logger, ILoggerFactory loggerFactory) : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        {
            mEntityContext = entityContext;
            mLoggingService = loggingService;
            mConfigService = configService;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            if (loggerFactory == null)
            {
                AsistenteAD = new AsistenteAD(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication, null, null);
            }
            else
            {
                AsistenteAD = new AsistenteAD(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<AsistenteAD>(), mLoggerFactory);
            }
        }

        #region Metodos Publicos

        public void GuardarAsistente(Asistente pAsistente)
        {
            AsistenteAD.GuardarAsistente(pAsistente);
        }

        public List<Guid> ObtenerRolesAsistente(Guid pAsistenteId)
        {
            return AsistenteAD.ObtenerRolesAsistente(pAsistenteId);
        }

        public void GuardarRolAsistente(RolAsistente pRolAsistente)
        {
            AsistenteAD.GuardarRolAsistente(pRolAsistente);
        }

        public void EliminarRolesAsistente(List<Guid> pRolesId, Guid pAsistenteId)
        {
            AsistenteAD.EliminarRolesAsistente(pRolesId, pAsistenteId);
        }

        public List<Guid> EliminarRolAsistente(Guid pRolId)
        {
            return AsistenteAD.EliminarRolAsistente(pRolId);
        }

        public bool ComprobarAsistenteSinRoles(Guid pAsistenteId)
        {
            return AsistenteAD.ComprobarAsistenteSinRoles(pAsistenteId);
        }

        public List<Asistente> ObtenerAsistentesPorProyecto(Guid pProyectoId)
        {
            return AsistenteAD.ObtenerAsistentesPorProyecto(pProyectoId);
        }

        public Asistente ObtenerAsistenePorProyecto(Guid pAsistenteId, Guid pProyectoId)
        {
            return AsistenteAD.ObtenerAsistenePorProyecto(pAsistenteId,  pProyectoId);
        }

        public bool ExisteAsistenteEnProyecto(Guid pAsistenteId, Guid pProyectoId)
        {
            return AsistenteAD.ExisteAsistenteEnProyecto(pAsistenteId, pProyectoId);
        }

        public void EliminarAsistente(Guid pAsistenteId, Guid pProyectoID)
        {
            AsistenteAD.EliminarAsistente(pAsistenteId, pProyectoID);
        }

        public Dictionary<Guid, string> ObtenerAsistentesAfectadosPorRol(Guid pRolId)
        {
            return AsistenteAD.ObtenerAsistentesAfectadosPorRol(pRolId);
        }

        #endregion

        #region Propiedades 

        public AsistenteAD AsistenteAD
        {
            get
            {
                return (AsistenteAD)AD;
            }
            set
            {
                AD = value;
            }
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Determina si está disposed
        /// </summary>
        private bool mDisposed = false;

        /// <summary>
        /// Destructor
        /// </summary>
        ~AsistenteCN()
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
        /// <param name="pDisposing">Determina si se está llamando desde el Dispose()</param>
        protected virtual void Dispose(bool pDisposing)
        {
            if (!mDisposed)
            {
                mDisposed = true;

                if (pDisposing)
                {
                    //Libero todos los recursos administrados que he añadido a esta clase
                    if (this.AsistenteAD != null)
                    {
                        AsistenteAD.Dispose();
                    }
                }
                AsistenteAD = null;
            }
        }

        #endregion
    }
}
