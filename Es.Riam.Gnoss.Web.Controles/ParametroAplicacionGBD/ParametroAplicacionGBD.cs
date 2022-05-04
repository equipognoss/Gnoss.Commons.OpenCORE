using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.Elementos.ParametroAplicacion;
using Es.Riam.Gnoss.Logica.ParametroAplicacion;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using System.Collections.Generic;
using System.Linq;

namespace Es.Riam.Gnoss.Web.Controles.ParametroAplicacionGBD
{
    public class ParametroAplicacionGBD
    {
        private LoggingService mLoggingService;
        private EntityContext mEntityContext;
        private ConfigService mConfigService;

        public ParametroAplicacionGBD(LoggingService loggingService, EntityContext entityContext, ConfigService configService)
        {
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;
        }

        public GestorParametroAplicacion ObtenerConfiguracionBBDD()
        {
            GestorParametroAplicacion gestor = new GestorParametroAplicacion();
            ObtenerConfiguracionBBDD(gestor);
            return gestor;
        }

        public void ObtenerConfiguracionBBDD(GestorParametroAplicacion gestor)
        {
            gestor.ListaConfiguracionBBDD = cargarConfiguracionBBDD();
            gestor.ListaConfiguracionServicios = cargarConfiguracionServicios();
            gestor.ListaConfiguracionServiciosDominio = cargarConfiguracionServiciosDominio();
            gestor.ListaConfiguracionServiciosProyecto = cargarConfiguracionServiciosProyecto();
        }

        public void ObtenerConfiguracionGnoss(GestorParametroAplicacion gestor)
        {
            gestor.ParametroAplicacion = cargarParametroAplicacion();
            gestor.ListaProyectoRegistroObligatorio = cargarProyectoRegistroObligatorio();
            gestor.ListaProyectoSinRegistroObligatorio = cargarProyecoSinRegistroObligatorio();
            gestor.ListaAccionesExternas = cargarAccionesExternas();
            gestor.ListaConfigApplicationInsightsDominio = cargarConfigApplicationInsightsDominio();
            gestor.ListaTextosPersonalizadosPlataforma = cargarTextosPersonalizadosPlataforma();
        }
        public string ObtenerURLAPIV3()
        {
            return mEntityContext.ConfiguracionServicios.Where(url => url.Nombre.Equals("urlApiV3")).Select(param => param.Url).FirstOrDefault();
        }

        private List<ConfiguracionBBDD> cargarConfiguracionBBDD()
        {
            return mEntityContext.ConfiguracionBBDD.ToList();
        }

        private List<AD.EntityModel.ConfiguracionServicios> cargarConfiguracionServicios()
        {
            return mEntityContext.ConfiguracionServicios.ToList();
        }

        private List<ConfiguracionServiciosDominio> cargarConfiguracionServiciosDominio()
        {
            return mEntityContext.ConfiguracionServiciosDominio.ToList();
        }

        private List<ConfiguracionServiciosProyecto> cargarConfiguracionServiciosProyecto()
        {
            return mEntityContext.ConfiguracionServiciosProyecto.ToList();
        }

        private List<ParametroAplicacion> cargarParametroAplicacion()
        {
            return mEntityContext.ParametroAplicacion.ToList();
        }

        public void AddParametroAplicacion(ParametroAplicacion parametroAplicacion)
        {
            mEntityContext.ParametroAplicacion.Add(parametroAplicacion);
            mEntityContext.SaveChanges();
        }

        private List<ProyectoRegistroObligatorio> cargarProyectoRegistroObligatorio()
        {
            return mEntityContext.ProyectoRegistroObligatorio.ToList();
        }

        private List<ProyectoSinRegistroObligatorio> cargarProyecoSinRegistroObligatorio()
        {
            return mEntityContext.ProyectoSinRegistroObligatorio.ToList();
        }

        public void removeParametroAplicacion(ParametroAplicacion filaParametro)
        {
            mEntityContext.ParametroAplicacion.Remove(filaParametro);
            mEntityContext.SaveChanges();
        }

        private List<AccionesExternas> cargarAccionesExternas()
        {
            return mEntityContext.AccionesExternas.ToList();
        }

        private List<ConfigApplicationInsightsDominio> cargarConfigApplicationInsightsDominio()
        {
            return mEntityContext.ConfigApplicationInsightsDominio.ToList();
        }

        private List<TextosPersonalizadosPlataforma> cargarTextosPersonalizadosPlataforma()
        {
            return mEntityContext.TextosPersonalizadosPlataforma.ToList();
        }
    }
}
