using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Cookies;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.Cookie
{
    public class CookieAD : BaseAD
    {
        private EntityContext mEntityContext;
        private ConfigService mConfigService;
        private LoggingService mLoggingService;

        public CookieAD(LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication)
        {
            mEntityContext = entityContext;
            mLoggingService = loggingService;
            mConfigService = configService;
        }

        public CookieAD(string pFicheroConfiguracionBD, LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication)
        {
            mEntityContext = entityContext;
            mLoggingService = loggingService;
            mConfigService = configService;

        }

        public bool ExistenCookiesTecnicas()
        {
            if (mEntityContext.CategoriaProyectoCookie.Where(item => item.NombreCorto.Equals("Tecnica")).Any())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void CrearCookiesInicialesProyecto(Guid pProyectoID, Guid pOrganizacionID)
        {
            CategoriaProyectoCookie categoriaTecnica = new CategoriaProyectoCookie() { CategoriaID = Guid.NewGuid(), Nombre = "Técnica", NombreCorto = "Tecnica", Descripcion = "Las cookies técnicas son aquellas imprescindibles y estrictamente necesarias para el correcto funcionamiento del Sitio Web y la utilización de las diferentes opciones y servicios que ofrece, incluyendo aquellas que el editor utiliza para permitir la gestión operativa de la página web y habilitar sus funciones y servicios. Por ejemplo, las que sirven para el mantenimiento de la sesión, la gestión del tiempo de respuesta, rendimiento o validación de opciones, utilizar elementos de seguridad, compartir contenido con redes sociales, etc. La página web no puede funcionar adecuadamente sin estas cookies.", EsCategoriaTecnica = true, OrganizacionID = pOrganizacionID, ProyectoID = pProyectoID  };

            ProyectoCookie cookieAvisoGnoss = new ProyectoCookie() { CategoriaID = categoriaTecnica.CategoriaID, CookieID = Guid.NewGuid(), Nombre = "cookieAviso.gnoss.com", NombreCorto = "cookieAvisoGnoss", Descripcion = "Se sa para saber si el usuario ha aceptado la política de cookies", Tipo = (short)TipoCookies.Persistent, OrganizacionID = pOrganizacionID, ProyectoID = pProyectoID, EsEditable = false };
            ProyectoCookie cookieAspNetSesionID = new ProyectoCookie() { CategoriaID = categoriaTecnica.CategoriaID, CookieID = Guid.NewGuid(), Nombre = "ASP.NET_SessionId", NombreCorto = "aspNetSessionId", Descripcion = "Almacena la sesión del usuario actual", Tipo = (short)TipoCookies.Session, OrganizacionID = pOrganizacionID, ProyectoID = pProyectoID, EsEditable = false };
            ProyectoCookie cookieIdiomaActual = new ProyectoCookie() { CategoriaID = categoriaTecnica.CategoriaID, CookieID = Guid.NewGuid(), Nombre = "IdiomaActual", NombreCorto = "idiomaActual", Descripcion = "Se usa para almacenar el idioma de navegación del usuario", Tipo = (short)TipoCookies.Session, OrganizacionID = pOrganizacionID, ProyectoID = pProyectoID, EsEditable = false };
            ProyectoCookie cookieUsuarioLogueado = new ProyectoCookie() { CategoriaID = categoriaTecnica.CategoriaID, CookieID = Guid.NewGuid(), Nombre = "UsuarioLogueado", NombreCorto = "usuarioLogueado", Descripcion = "Se usa para saber si el usuario ha hecho login", Tipo = (short)TipoCookies.Session, OrganizacionID = pOrganizacionID, ProyectoID = pProyectoID, EsEditable = false };
            ProyectoCookie cookieSesionUsuarioAvtiva = new ProyectoCookie() { CategoriaID = categoriaTecnica.CategoriaID, CookieID = Guid.NewGuid(), Nombre = "SesionUsuarioActiva", NombreCorto = "sesionUsuarioActiva", Descripcion = "Almacena la duración de la sesión del usuario", Tipo = (short)TipoCookies.Session, OrganizacionID = pOrganizacionID, ProyectoID = pProyectoID, EsEditable = false };
        }

        public List<CategoriaProyectoCookie> ObtenerCategoriasProyectoCookie(Guid pProyectoID)
        {
            return mEntityContext.CategoriaProyectoCookie.Where(item => item.ProyectoID.Equals(pProyectoID)).ToList();
        }

        public bool TieneCategoriaCookiesVinculadas(Guid pCategoriaID)
        {
            return mEntityContext.ProyectoCookie.Where(item => item.CategoriaID.Equals(pCategoriaID)).Any();
        }

        public bool ExistenCookiesYoutube()
        {
            Guid categoriaIDRedesSociales = mEntityContext.CategoriaProyectoCookie.Where(item => item.NombreCorto.Equals("Redes sociales")).Select(item => item.CategoriaID).FirstOrDefault();
            if (categoriaIDRedesSociales != null && categoriaIDRedesSociales.Equals(Guid.Empty))
            {
                return mEntityContext.ProyectoCookie.Where(item => item.CategoriaID.Equals(categoriaIDRedesSociales)).Any();
            }
            return false;
        }

        public bool ExistenCookiesAnaliticas()
        {
            Guid categoriaIDCookiesAnaliticas = mEntityContext.CategoriaProyectoCookie.Where(item => item.NombreCorto.Equals("Analiticas")).Select(item => item.CategoriaID).FirstOrDefault();
            if (categoriaIDCookiesAnaliticas != null && categoriaIDCookiesAnaliticas.Equals(Guid.Empty))
            {
                return mEntityContext.ProyectoCookie.Where(item => item.CategoriaID.Equals(categoriaIDCookiesAnaliticas)).Any();
            }
            return false;
        }

        public List<ProyectoCookie> ObtenerCookiesDeProyecto(Guid pProyectoID)
        {
            return mEntityContext.ProyectoCookie.Where(item => item.ProyectoID.Equals(pProyectoID)).ToList();
        }

        public ProyectoCookie ObtenerCookiePorId(Guid pCookieID, Guid pProyectoID)
        {
            return mEntityContext.ProyectoCookie.Where(item => item.CookieID.Equals(pCookieID) && item.ProyectoID.Equals(pProyectoID)).FirstOrDefault();
        }

        public CategoriaProyectoCookie ObtenerCategoriaPorId(Guid pCategoriaID, Guid pProyectoID)
        {
            return mEntityContext.CategoriaProyectoCookie.Where(item => item.CategoriaID.Equals(pCategoriaID) && item.ProyectoID.Equals(pProyectoID)).FirstOrDefault();
        }

        public CategoriaProyectoCookie ObtenerCategoriaPorNombreCorto(string pNombreCorto, Guid pProyectoID)
        {
            return mEntityContext.CategoriaProyectoCookie.Where(item => item.NombreCorto.Equals(pNombreCorto) && item.ProyectoID.Equals(pProyectoID)).FirstOrDefault();
        }

        public bool HayCookiesVinculadas(Guid pCategoriaID)
        {
            return mEntityContext.ProyectoCookie.Where(item => item.CategoriaID.Equals(pCategoriaID)).Any();
        }

        public List<ProyectoCookie> ObtenerCookiesDeCategoria(Guid pCategoriaID)
        {
            return mEntityContext.ProyectoCookie.Where(item => item.CategoriaID.Equals(pCategoriaID)).ToList();
        }

    }
}
