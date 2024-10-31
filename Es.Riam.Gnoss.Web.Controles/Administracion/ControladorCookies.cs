using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Cookies;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.Cookie;
using Es.Riam.Gnoss.Logica.Cookie;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Es.Riam.Gnoss.Web.Controles.Administracion
{
    public class ControladorCookies : ControladorBase
    {
        private Elementos.ServiciosGenerales.Proyecto proyecto = null;
        public ControladorCookies(Elementos.ServiciosGenerales.Proyecto pProyecto, LoggingService loggingService, EntityContext entityContext, ConfigService configService, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            :base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, servicesUtilVirtuosoAndReplication)
        {
            proyecto = pProyecto;
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


        public void GuardarCategoriasYCookies(CategoriasCookiesModel pCategoriasYCookies)
        {
            EliminarCategoriasYCookies();
            foreach (CategoriaProyectoCookieModel cat in pCategoriasYCookies.Categorias)
            {
                CategoriaProyectoCookie categoria = new CategoriaProyectoCookie();
                categoria.CategoriaID = cat.CategoriaID;
                categoria.EsCategoriaTecnica = cat.EsCategoriaTecnica;
                categoria.OrganizacionID = cat.OrganizacionID;
                categoria.Descripcion = cat.Descripcion;
                categoria.NombreCorto = cat.NombreCorto;
                categoria.Nombre = cat.Nombre;
                categoria.ProyectoID = cat.ProyectoID;

                mEntityContext.CategoriaProyectoCookie.Add(categoria);
            }
            mEntityContext.SaveChanges();

            foreach (ProyectoCookieModel cookie in pCategoriasYCookies.Cookies)
            {
                ProyectoCookie nuevaCookie = new ProyectoCookie();
                nuevaCookie.ProyectoID = cookie.ProyectoID;
                nuevaCookie.Tipo = cookie.Tipo;
                nuevaCookie.CookieID = cookie.CookieID;
                nuevaCookie.CategoriaID = cookie.CategoriaID;
                nuevaCookie.Descripcion = cookie.Descripcion;
                nuevaCookie.NombreCorto = cookie.NombreCorto;
                nuevaCookie.Nombre = cookie.Nombre;
                nuevaCookie.OrganizacionID = cookie.OrganizacionID;
                nuevaCookie.ProyectoID = cookie.ProyectoID;
                nuevaCookie.EsEditable = cookie.EsEditable;               

                mEntityContext.ProyectoCookie.Add(nuevaCookie);
            }
            mEntityContext.SaveChanges();

            CookieCL cookieCL = new CookieCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
            cookieCL.InvalidarCategoriaProyectoCookie(proyecto.Clave);
        }


        public void EliminarCategoriasYCookies()
        {
            CookieCN cookieCN = new CookieCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

            List<ProyectoCookie> cookiesBD = cookieCN.ObtenerCookiesDeProyecto(proyecto.Clave);
            foreach (ProyectoCookie cookie in cookiesBD)
            {
                mEntityContext.ProyectoCookie.Remove(cookie);
            }
            mEntityContext.SaveChanges();

            List<CategoriaProyectoCookie> categoriasBD = cookieCN.ObtenerCategoriasProyectoCookie(proyecto.Clave);
            foreach (CategoriaProyectoCookie categoria in categoriasBD)
            {
                if (!cookieCN.HayCookiesVinculadas(categoria.CategoriaID))
                {
                    mEntityContext.CategoriaProyectoCookie.Remove(categoria);
                }
                else
                {
                    string mensajeError = $"No se puede eliminar una categoría que tiene cookies vinculadas\n\tCategoriaID:{categoria.CategoriaID}";
                    mLoggingService.GuardarLogError(mensajeError);
                    throw new Exception(mensajeError);
                }
            }
            mEntityContext.SaveChanges();
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
                ProyectoCookie proyectoCookie = cookieCN.ObtenerCookiesDeProyecto(proyecto.Clave).Where(cookieProy => cookieProy.CookieID.Equals(cookie.CookieID)).FirstOrDefault();
                if (proyectoCookie != null)
                {
                    cookieCN.EliminarProyectoCookie(proyectoCookie);
                    cookieCN.Actualizar();
                }
            }
        }

        public void ModificarCookieExistente(List<Guid> listaCookiesNuevas, CookiesModel pCookie)
        {
            if (!pCookie.Deleted && !listaCookiesNuevas.Contains(pCookie.CookieID))
            {
                CookieCN cookieCN = new CookieCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                List<ProyectoCookie> filasCookies = cookieCN.ObtenerCookiesDeProyecto(proyecto.Clave).Where(cookieProy => cookieProy.CookieID.Equals(pCookie.CookieID)).ToList();
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
                List<ProyectoCookie> filasCookies = cookieCN.ObtenerCookiesDeProyecto(proyecto.Clave).Where(cookieProy => cookieProy.CookieID.Equals(cookie.CookieID)).ToList();
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
            cookie.OrganizacionID = proyecto.FilaProyecto.OrganizacionID;
            cookie.CategoriaID = categoriaID;
            cookie.EsEditable = !EsCookieTecnica(pCookie.Nombre);
            cookie.Tipo = (short)pCookie.Tipo;
            cookie.Descripcion = pCookie.Descripcion;
            cookie.ProyectoID = proyecto.Clave;

            CookieCN cookieCN = new CookieCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            cookieCN.AnyadirProyectoCookie(cookie);
            cookieCN.Actualizar();
        }

        public bool EsCookieTecnica(string pNombreCookie)
        {
            string[] listaCookiesTecnicas = new string[] { "IdiomaActual", "cookieAviso.gnoss.com", "ASP.NET_SessionId", "UsuarioLogueado", "SesionUsuarioActiva" };

            return listaCookiesTecnicas.Contains(pNombreCookie);
        }

        public Guid ComprobarExisteCategoria(CookiesModel pCookie)
        {
            CookieCN cookieCN = new CookieCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

            CategoriaProyectoCookie categoria = cookieCN.ObtenerCategoriaPorNombreCorto(pCookie.Categoria.NombreCorto, proyecto.Clave);
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
                nuevaCategoria.ProyectoID = proyecto.Clave;
                nuevaCategoria.OrganizacionID = proyecto.FilaProyecto.OrganizacionID;
                categoriaID = nuevaCategoria.CategoriaID;

                cookieCN.AnyadirCategoriaProyectoCookie(nuevaCategoria);
                cookieCN.Actualizar();
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
