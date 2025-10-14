using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.ParametroGeneralDS;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.ParametrosProyecto;
using Es.Riam.Gnoss.Elementos.ParametroGeneralDSEspacio;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.UtilServiciosWeb;
using Es.Riam.Gnoss.Web.Controles.ParametroGeneralDSName;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace Es.Riam.Gnoss.Web.Controles.Administracion
{
    public class ControladorTraducciones : ControladorBase
    {

        private LoggingService mLoggingService;
        private EntityContext mEntityContext;
        private ConfigService mConfigService;
        private RedisCacheWrapper mRedisCacheWrapper;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        public ControladorTraducciones(LoggingService loggingService, EntityContext entityContext, ConfigService configService, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<ControladorTraducciones> logger, ILoggerFactory loggerFactory)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, servicesUtilVirtuosoAndReplication,logger,loggerFactory)
        {
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;
            mRedisCacheWrapper = redisCacheWrapper;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        public void LimpiarCaches(Guid personalizacionID, bool EsAdministracionEcosistema)
        {
            using (ParametroGeneralCL paramGeneralCL = new ParametroGeneralCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroGeneralCL>(), mLoggerFactory))
            using (GnossCacheCL gnossCacheCL = new GnossCacheCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<GnossCacheCL>(), mLoggerFactory))
            {
                if (EsAdministracionEcosistema)
                {
                    paramGeneralCL.InvalidarCacheTextosPersonalizadosPersonalizacionEcosistema(personalizacionID);
                }
                else
                {
                    paramGeneralCL.InvalidarCacheParametrosGeneralesDeProyecto(ProyectoSeleccionado.Clave);
                }
                if (EsAdministracionEcosistema)
                {
                    gnossCacheCL.AgregarObjetoCache(GnossCacheCL.CLAVE_REFRESCO_CACHE_TRADUCCIONES + PersonalizacionEcosistemaID, Guid.NewGuid());
                }
                else if (!ProyectoSeleccionado.PersonalizacionID.Equals(Guid.Empty))
                {
                    gnossCacheCL.AgregarObjetoCache(GnossCacheCL.CLAVE_REFRESCO_CACHE_TRADUCCIONES + ProyectoSeleccionado.PersonalizacionID, Guid.NewGuid());
                }
            }
        }

        public void AlmacenarDatosTextosPersonalizados(string pTextoID, bool pEsEdicion, GestorParametroGeneral pTextosPersonalizadosDSGuardado, Guid pPersonalizacionID, string pIdioma, string ptexto)
        {
            // ParametroGeneralDSName.TextosPersonalizadosPersonalizacionRow filaTextoPersonalizado = textosPersonalizadosDSGuardado.TextosPersonalizadosPersonalizacion.FindByPersonalizacionIDTextoIDLanguage(personalizacionID, pTextoID, pIdioma);
            DateTime fechaActual = DateTime.Now;
            TextosPersonalizadosPersonalizacion filaTextoPersonalizado = pTextosPersonalizadosDSGuardado.ListaTextosPersonalizadosPersonalizacion.FirstOrDefault(textoPersonalizado => textoPersonalizado.PersonalizacionID.Equals(pPersonalizacionID) && textoPersonalizado.TextoID.Equals(pTextoID) && textoPersonalizado.Language.Equals(pIdioma));
            ParametroGeneralGBD gestorController = new ParametroGeneralGBD(mEntityContext);
            if (!string.IsNullOrEmpty(pTextoID) && !string.IsNullOrEmpty(ptexto))
            {
                if (filaTextoPersonalizado == null)
                {
                    TextosPersonalizadosPersonalizacion filaTextosPersonalizadosPersonalizacionRow = new TextosPersonalizadosPersonalizacion();
                    filaTextosPersonalizadosPersonalizacionRow.PersonalizacionID = pPersonalizacionID;
                    filaTextosPersonalizadosPersonalizacionRow.TextoID = pTextoID;
                    filaTextosPersonalizadosPersonalizacionRow.Language = pIdioma;
                    filaTextosPersonalizadosPersonalizacionRow.Texto = ptexto;
                    filaTextosPersonalizadosPersonalizacionRow.FechaCreacion = fechaActual;
                    filaTextosPersonalizadosPersonalizacionRow.FechaActualizacion = fechaActual;
                    pTextosPersonalizadosDSGuardado.ListaTextosPersonalizadosPersonalizacion.Add(filaTextosPersonalizadosPersonalizacionRow);
                    gestorController.AddTextosPersonalizadosPersonalizacion(filaTextosPersonalizadosPersonalizacionRow);
                    gestorController.SaveChanges();
                }
                else
                {
                    if (pEsEdicion)
                    {

                        if (gestorController.ObtenerEstado(filaTextoPersonalizado) == EntityState.Detached)
                        {
                            filaTextoPersonalizado = gestorController.ObtenerTextoPersonalizadoPersonalizacion(pPersonalizacionID, pTextoID, pIdioma);
                        }
                        filaTextoPersonalizado.Texto = ptexto;
                        filaTextoPersonalizado.FechaActualizacion = fechaActual;
                        gestorController.SaveChanges();
                    }
                    else
                    {
                        throw new ErrorEdicionNoValida("El TextId que se intenta crear ya existe");
                    }
                }
            }
            else
            {
                //eliminamos la fila si existe
                if (filaTextoPersonalizado != null)
                {
                    //   filaTextoPersonalizado.Delete();
                    pTextosPersonalizadosDSGuardado.ListaTextosPersonalizadosPersonalizacion.Remove(filaTextoPersonalizado);
                    gestorController.DeleteTextoPersonalizadoPersonalizacion(filaTextoPersonalizado);
                    gestorController.SaveChanges();
                }
            }
        }
    }
}
