using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Live.Model;
using Es.Riam.Gnoss.AD.Live;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.Identidad;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.Live;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.Controles.Documentacion;
using Microsoft.AspNetCore.Http;
using System;
using Microsoft.Extensions.Logging;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.UtilServiciosWeb;

namespace Es.Riam.Gnoss.Web.Controles.Administracion
{
    public class ControladorComunidad : ControladorBase
    {
        #region Miembros

        private LoggingService mLoggingService;
        private VirtuosoAD mVirtuosoAD;
        private EntityContext mEntityContext;
        private IHttpContextAccessor mHttpContextAccessor;
        private ConfigService mConfigService;
        private RedisCacheWrapper mRedisCacheWrapper;
        private GnossCache mGnossCache;
        private EntityContextBASE mEntityContextBASE;
        private IServicesUtilVirtuosoAndReplication mServicesUtilVirtuosoAndReplication;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        #endregion

        #region Constructores

        public ControladorComunidad(LoggingService loggingService, EntityContext entityContext, ConfigService configService, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, EntityContextBASE pEntityContextBASE, ILogger<ControladorComunidad> logger, ILoggerFactory loggerFactory)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, servicesUtilVirtuosoAndReplication,logger,loggerFactory)
        {
            mVirtuosoAD = virtuosoAD;
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;
            mRedisCacheWrapper = redisCacheWrapper;
            mGnossCache = gnossCache;
            mEntityContextBASE = pEntityContextBASE;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        #endregion

        #region Métodos públicos

        /// <summary>
        /// Cierra la comunidad
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Cadena con el error en caso de existir. Cadena vacía en caso contrario</returns>
        public void CerrarComunidad(Guid pProyectoID, GestionProyecto pGestorProyecto)
        {
            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
            LiveCN liveCN = new LiveCN("base", mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<LiveCN>(), mLoggerFactory);
            LiveDS liveDS = new LiveDS();

            Proyecto proyecto = pGestorProyecto.ListaProyectos[pProyectoID];
            proyecto.Estado = (short)EstadoProyecto.Cerrado;

            foreach (AD.EntityModel.Models.ProyectoDS.AdministradorProyecto filaAdminProyecto in pGestorProyecto.DataWrapperProyectos.ListaAdministradorProyecto)
            {
                liveDS.Cola.AddColaRow(pProyectoID, filaAdminProyecto.UsuarioID, (short)AccionLive.ComunidadCerrada, 0, 0, DateTime.Now, false, (short)PrioridadLive.Baja, null);
            }

            proyCN.ActualizarProyectos(false);
            proyCN.Dispose();

            liveCN.ActualizarBD(liveDS);
            liveCN.Dispose();

            new ControladorDocumentacion(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorDocumentacion>(), mLoggerFactory).EstablecePrivacidadTodosRecursosComunidadEnMetaBuscador(pProyectoID, null, true);

            //Eliminar caché de todos usuarios de esa comunidad
            IdentidadCL identidadCL = new IdentidadCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCL>(), mLoggerFactory);
            IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
            GestionIdentidades gestorIdent = new GestionIdentidades(identCN.ObtenerIdentidadesDeProyecto(pProyectoID), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);

            foreach (Guid identidadID in gestorIdent.ListaIdentidades.Keys)
            {
                Identidad identidad = gestorIdent.ListaIdentidades[identidadID];
                if (identidad.PersonaID.HasValue)
                {
                    identidadCL.EliminarCacheGestorIdentidad(identidad.Clave, identidad.PersonaID.Value);
                }
                identidadCL.EliminarPerfilMVC(identidad.PerfilID);
            }
            identidadCL.Dispose();

        }

        #endregion
    }
}
