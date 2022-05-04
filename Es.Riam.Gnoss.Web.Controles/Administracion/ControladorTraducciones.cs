using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.ParametroGeneralDS;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.ParametrosProyecto;
using Es.Riam.Gnoss.Elementos.ParametroGeneralDSEspacio;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.Controles.ParametroGeneralDSName;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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

        public ControladorTraducciones(LoggingService loggingService, EntityContext entityContext, ConfigService configService, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, servicesUtilVirtuosoAndReplication)
        {
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;
            mRedisCacheWrapper = redisCacheWrapper;
        }

        public void LimpiarCaches(Guid personalizacionID, bool EsAdministracionEcosistema)
        {
            using (ParametroGeneralCL paramGeneralCL = new ParametroGeneralCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication))
            using (GnossCacheCL gnossCacheCL = new GnossCacheCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication))
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
                    pTextosPersonalizadosDSGuardado.ListaTextosPersonalizadosPersonalizacion.Add(filaTextosPersonalizadosPersonalizacionRow);
                    gestorController.AddTextosPersonalizadosPersonalizacion(filaTextosPersonalizadosPersonalizacionRow);
                    gestorController.saveChanges();
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
                        gestorController.saveChanges();
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
                    gestorController.saveChanges();
                }
            }
        }
    }
}
