using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.Logica.Notificacion;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.Extensions.Logging;
using System;
using System.Data;

namespace Es.Riam.Gnoss.CL.ServiciosGenerales
{
    public class PersonaCL : BaseCL
    {
        #region Miembros

        /// <summary>
        /// Clave MAESTRA de la caché
        /// </summary>
        private readonly string[] mMasterCacheKeyArray = { NombresCL.PERSONAS };

        private EntityContext mEntityContext;
        private LoggingService mLoggingService;
        private ConfigService mConfigService;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        #endregion

        public PersonaCL(EntityContext entityContext, LoggingService loggingService, RedisCacheWrapper redisCacheWrapper, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<PersonaCL> logger, ILoggerFactory loggerFactory)
            : base(entityContext, loggingService, redisCacheWrapper, configService, servicesUtilVirtuosoAndReplication,logger, loggerFactory)
        {
            mConfigService = configService;
            mEntityContext = entityContext;
            mLoggingService = loggingService;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        #region Métodos

        /// <summary>
        /// Obtiene las personas de la estructura de una organización (Empleados, Colaboradores, Proveedores)
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <returns>Personas participan en organización</returns>
        public DataWrapperPersona ObtenerPersonasDeOrganizacionCargaLigera(Guid pOrganizacionID)
        {
            mEntityContext.UsarEntityCache = true;
            string rawKey = pOrganizacionID.ToString();

            // Compruebo si está en la caché
            DataWrapperPersona dataWrapperPersona = ObtenerObjetoDeCache(rawKey, typeof(DataWrapperPersona)) as DataWrapperPersona;
            if (dataWrapperPersona == null)
            {
                PersonaCN PersonaCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<PersonaCN>(), mLoggerFactory);

                // Si no está, lo cargo y lo almaceno en la caché
                dataWrapperPersona = PersonaCN.ObtenerPersonasDeOrganizacionCargaLigera(pOrganizacionID);
                if (dataWrapperPersona != null)
                {
                    dataWrapperPersona.CargaRelacionesPerezosasCache();
                }
                AgregarObjetoCache(rawKey, dataWrapperPersona);

                PersonaCN.Dispose();
            }
            mEntityContext.UsarEntityCache = false;
            return dataWrapperPersona;
        }

        /// <summary>
        /// Nos indica si tiene notificaciones para mostrar al conectarse
        /// </summary>
        /// <param name="pPersonaID">ID de la persona</param>
        /// <param name="pCodigo">ID de la notificacion</param>
        /// <returns>TRUE si tiene notificacion</returns>
        public bool TieneNotificacionSinLeer(Guid pPersonaID, int pCodigo)
        {
            string rawKey = string.Concat(NombresCL.NOTIFICACIONPENDIENTE, "_", pPersonaID.ToString(), "_", pCodigo);

            // Compruebo si está en la caché
            bool? tiene = ObtenerObjetoDeCache(rawKey, typeof(bool)) as bool?;
            if (!tiene.HasValue)
            {
                NotificacionCN notificacionNC = new NotificacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<NotificacionCN>(), mLoggerFactory);
                tiene = notificacionNC.TieneNotificacionSinLeer(pPersonaID, pCodigo);
                AgregarObjetoCache(rawKey, tiene);

                notificacionNC.Dispose();
            }

            return tiene.Value;
        }


        /// <summary>
        /// Invalida la cache de notificaciones
        /// </summary>
        /// <param name="pPersonaID">ID de la persona</param>
        /// <param name="pCodigo">ID de la notificacion</param>
        public void InvalidarNotificacionSinLeer(Guid pPersonaID, int pCodigo)
        {
            string rawKey = string.Concat(NombresCL.NOTIFICACIONPENDIENTE, "_", pPersonaID.ToString(), "_", pCodigo);
            InvalidarCache(rawKey);
        }

        /// <summary>
        /// Actualiza personas
        /// </summary>
        /// <param name="pPersonaDS">Personas</param>
        public void ActualizarPersonas(DataSet pPersonaDS)
        {
            //Si hay cambios los guardo e invalido la caché
            if (pPersonaDS.HasChanges())
            {
                PersonaCN PersonaCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<PersonaCN>(), mLoggerFactory);

                //Actualizar los cambios
                PersonaCN.ActualizarPersonas();

                // Invalidar la caché
                InvalidarCache();

                PersonaCN.Dispose();
            }
        }

        /// <summary>
        /// modifica la vista del listado de mensajes, alternandola entre normal y compactada
        /// </summary>
        /// <param name="pPersonaID"></param>
        /// <param name="pVista"></param>
        public void ModificarVistaMenuMensajes(Guid pPersonaID, string pVista)
        {
            string rawKey = string.Concat(NombresCL.PERSONAS, "_" + pPersonaID + "_VistaMensajes");
            AgregarObjetoCache(rawKey, pVista);
        }

        /// <summary>
        /// obtiene la vista del listado de mensajes, normal o compactada
        /// </summary>
        /// <returns></returns>
        public string ObtenerVistaMenuMensajes(Guid pPersonaID)
        {
            return (string)ObtenerObjetoDeCache(string.Concat(NombresCL.PERSONAS, "_" + pPersonaID + "_VistaMensajes"),typeof(string));
        }

        #endregion        

        #region Propiedades

        /// <summary>
        /// Clave para la caché
        /// </summary>
        public override string[] ClaveCache
        {
            get
            {
                return mMasterCacheKeyArray;
            }
        }

        #endregion
    }
}
