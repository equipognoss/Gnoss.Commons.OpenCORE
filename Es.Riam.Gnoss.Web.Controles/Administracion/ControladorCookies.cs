using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Cookies;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Logica.Cookie;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.Web.Controles.Administracion
{
    public class ControladorCookies : ControladorBase
    {
        private Elementos.ServiciosGenerales.Proyecto ProyectoSeleccionado = null;
        public ControladorCookies(Elementos.ServiciosGenerales.Proyecto pProyecto, LoggingService loggingService, EntityContext entityContext, ConfigService configService, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            :base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, servicesUtilVirtuosoAndReplication)
        {
            ProyectoSeleccionado = pProyecto;
            mVirtuosoAD = virtuosoAD;
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;
            mRedisCacheWrapper = redisCacheWrapper;
            mGnossCache = gnossCache;
        }

        public void GuardarCookie(CookiesModel pCookie)
        {
            List<Guid> listaCookiesNuevas = new List<Guid>();
            AnadirNuevaCookie(listaCookiesNuevas, pCookie);
            ModificarCookieExistente(listaCookiesNuevas, pCookie);
            EliminarCookie(listaCookiesNuevas, pCookie);
            using (ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication))
            {
                proyCN.ActualizarProyectos();
            }
        }

        public void GuardarCookies(List<CookiesModel> pListaCookies)
        {
            //Añadir las nuevas
            List<Guid> listaCookiesNuevas = new List<Guid>();
            foreach (CookiesModel cookie in pListaCookies)
            {
                AnadirNuevaCookie(listaCookiesNuevas, cookie);
            }
            //Modificar las que tienen cambios
            foreach (CookiesModel cookie in pListaCookies)
            {
                ModificarCookieExistente(listaCookiesNuevas, cookie);
            }
            //Eliminar las no incluidas
            foreach (CookiesModel cookie in pListaCookies)
            {
                EliminarCookie(listaCookiesNuevas, cookie);
            }
            using (ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication))
            {
                proyCN.ActualizarProyectos();
            }
        }

        public void EliminarCookie(List<Guid> listaCookiesNuevas, CookiesModel cookie)
        {
            if (cookie.Deleted && !listaCookiesNuevas.Contains(cookie.CookieID))
            {
                CookieCN cookieCN = new CookieCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                List<ProyectoCookie> filasCookies = cookieCN.ObtenerCookiesDeProyecto(ProyectoSeleccionado.Clave).Where(cookieProy => cookieProy.CookieID.Equals(cookie.CookieID)).ToList();
                if (filasCookies.Count > 0)
                {
                    mEntityContext.EliminarElemento(filasCookies.First());
                    mEntityContext.SaveChanges();
                }
            }
        }

        public void ModificarCookieExistente(List<Guid> listaCookiesNuevas, CookiesModel pCookie)
        {
            if (!pCookie.Deleted && !listaCookiesNuevas.Contains(pCookie.CookieID))
            {
                CookieCN cookieCN = new CookieCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                List<ProyectoCookie> filasCookies = cookieCN.ObtenerCookiesDeProyecto(ProyectoSeleccionado.Clave).Where(cookieProy => cookieProy.CookieID.Equals(pCookie.CookieID)).ToList();
                if (filasCookies.Count > 0)
                {
                    GuardarDatosProyectoCookie(filasCookies.First(), pCookie);
                }
            }
        }

        public void GuardarDatosProyectoCookie(ProyectoCookie pProyectoCookie, CookiesModel pCookie)
        {
            Guid categoriaID = ComprobarExisteCategoria(pCookie);

            pProyectoCookie.CategoriaID = categoriaID;
            pProyectoCookie.Descripcion = pCookie.Descripcion;
            pProyectoCookie.Nombre = pCookie.Nombre;
            pProyectoCookie.Tipo = (short)pCookie.Tipo;
            pProyectoCookie.EsEditable = !EsCookieTecnica(pCookie.Nombre);
        }
        public void AnadirNuevaCookie(List<Guid> listaCookiesNuevas, CookiesModel cookie)
        {
            if (!cookie.Deleted)
            {
                CookieCN cookieCN = new CookieCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                List<ProyectoCookie> filasCookies = cookieCN.ObtenerCookiesDeProyecto(ProyectoSeleccionado.Clave).Where(cookieProy => cookieProy.CookieID.Equals(cookie.CookieID)).ToList();
                if (filasCookies.Count == 0)
                {
                    listaCookiesNuevas.Add(cookie.CookieID);
                    AgregarCookieNueva(cookie);
                }
            }
        }

        public void AgregarCookieNueva(CookiesModel pCookie)
        {
            Guid categoriaID = ComprobarExisteCategoria(pCookie);
            ProyectoCookie cookie = new ProyectoCookie();
            cookie.CookieID = pCookie.CookieID;
            cookie.Nombre = pCookie.Nombre;
            cookie.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
            cookie.CategoriaID = categoriaID;
            cookie.EsEditable = !EsCookieTecnica(pCookie.Nombre);
            cookie.Tipo = (short)pCookie.Tipo;
            cookie.Descripcion = pCookie.Descripcion;
            cookie.ProyectoID = ProyectoSeleccionado.Clave;

            mEntityContext.ProyectoCookie.Add(cookie);
        }

        public bool EsCookieTecnica(string pNombreCookie)
        {
            string[] listaCookiesTecnicas = new string[] { "IdiomaActual", "cookieAviso.gnoss.com", "ASP.NET_SessionId", "UsuarioLogueado", "SesionUsuarioActiva" };

            return listaCookiesTecnicas.Contains(pNombreCookie);
        }

        public Guid ComprobarExisteCategoria(CookiesModel pCookie)
        {
            CookieCN cookieCN = new CookieCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

            CategoriaProyectoCookie categoria = cookieCN.ObtenerCategoriaPorNombreCorto(pCookie.Categoria.NombreCorto, ProyectoSeleccionado.Clave);
            Guid categoriaID = new Guid();
            if (categoria != null)
            {
                categoriaID = categoria.CategoriaID;
            }
            //si la categoria de la cookie no existe la creamos
            if (categoria == null)
            {
                CategoriaProyectoCookie nuevaCategoria = new CategoriaProyectoCookie();
                nuevaCategoria.Nombre = pCookie.Categoria.Nombre;
                nuevaCategoria.NombreCorto = pCookie.Categoria.NombreCorto;
                nuevaCategoria.EsCategoriaTecnica = false;
                nuevaCategoria.Descripcion = pCookie.Categoria.Descripcion;
                nuevaCategoria.CategoriaID = pCookie.Categoria.CategoriaID;
                nuevaCategoria.ProyectoID = ProyectoSeleccionado.Clave;
                nuevaCategoria.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
                categoriaID = nuevaCategoria.CategoriaID;
                mEntityContext.CategoriaProyectoCookie.Add(nuevaCategoria);
                mEntityContext.SaveChanges();
            }
            return categoriaID;
        }

        public short ObtenerTipoPorNombre(string pNombreTipo)
        {
            switch (pNombreTipo)
            {
                case "Persistent":
                    return 0;
                case "Session":
                    return 1;
                case "Third party":
                    return 2;
                default:
                    return 0;
            }
        }

    }
}
