using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.Cookie;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Cookies;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.Logica.Cookie
{
    public class CookieCN : BaseCN
    {
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        public CookieCN(EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<CookieCN> logger, ILoggerFactory loggerFactory)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        {
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            this.CookieAD = new CookieAD(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<CookieAD>(), mLoggerFactory);
        }
          

        public CookieCN(string pFicheroConfiguracionBD, EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<CookieCN> logger, ILoggerFactory loggerFactory)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        {
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            this.CookieAD = new CookieAD(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<CookieAD>(), mLoggerFactory);
        }

        public CookieAD CookieAD
        {
            get
            {
                return (CookieAD)AD;
            }
            set
            {
                this.AD = value;
            }
        }

        public void CrearCookiesInicialesProyecto(Guid pProyectoID, Guid pOrganizacionID)
        {
            CookieAD.CrearCookiesInicialesProyecto(pProyectoID, pOrganizacionID);
        }

        public bool ExistenCookiesAnaliticas()
        {
            return CookieAD.ExistenCookiesAnaliticas();
        }

        public bool ExistenCookiesYoutube()
        {
            return CookieAD.ExistenCookiesYoutube();
        }

        public List<CategoriaProyectoCookie> ObtenerCategoriasProyectoCookie(Guid pProyectoID)
        {
            return CookieAD.ObtenerCategoriasProyectoCookie(pProyectoID);
        }

        public bool TieneCategoriaCookiesVinculadas(Guid pCategoriaID)
        {
            return CookieAD.TieneCategoriaCookiesVinculadas(pCategoriaID);
        }

        public List<ProyectoCookie> ObtenerCookiesDeProyecto(Guid pProyectoID)
        {
            return CookieAD.ObtenerCookiesDeProyecto(pProyectoID);
        }

        public ProyectoCookie ObtenerCookiePorId(Guid pCookieID, Guid pProyectoID)
        {
            return CookieAD.ObtenerCookiePorId(pCookieID, pProyectoID);
        }

        public CategoriaProyectoCookie ObtenerCategoriaPorId(Guid pCategoriaID, Guid pProyectoID)
        {
            return CookieAD.ObtenerCategoriaPorId(pCategoriaID, pProyectoID);
        }

        public CategoriaProyectoCookie ObtenerCategoriaPorNombreCorto(string pNombreCorto, Guid pProyectoID)
        {
            return CookieAD.ObtenerCategoriaPorNombreCorto(pNombreCorto, pProyectoID);
        }

        public bool HayCookiesVinculadas(Guid pCategoriaID)
        {
            return CookieAD.HayCookiesVinculadas(pCategoriaID);
        }

        public List<ProyectoCookie> ObtenerCookiesDeCategoria(Guid pCategoriaID)
        {
            return CookieAD.ObtenerCookiesDeCategoria(pCategoriaID);
        }
    }
}
