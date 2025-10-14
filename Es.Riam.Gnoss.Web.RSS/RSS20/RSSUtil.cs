using System;
using System.Collections.Generic;
using System.Web;
using Es.Riam.Gnoss.Logica.Usuarios;
using Es.Riam.Gnoss.AD.Usuarios.Model;
using System.IO;
using Es.Riam.Gnoss.AD.Suscripcion;
using Es.Riam.Gnoss.Recursos;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Microsoft.Extensions.Logging;
using Es.Riam.Gnoss.AD.ParametroAplicacion;

namespace Es.Riam.Gnoss.Web.RSS.RSS20
{

    public class RSSUtil
    {
        private EntityContext mEntityContext;
        private LoggingService mLoggingService;
        private ConfigService mConfigService;
        private IServicesUtilVirtuosoAndReplication mServicesUtilVirtuosoAndReplication;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        public RSSUtil(EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<RSSUtil> logger, ILoggerFactory loggerFactory)
        {
            mConfigService = configService;
            mEntityContext = entityContext;
            mLoggingService = loggingService;
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        /// <summary>
        /// Método encargado de loguear a un usuario RSS
        /// </summary>
        /// <param name="pHttpRequest"></param>
        /// <param name="pHttpResponse"></param>
        /// <returns>Guid del usuario logueado</returns>
        public Guid LoginRSSPrivado(HttpRequest pHttpRequest, HttpResponse pHttpResponse)
        {
            string pNombrePassBase64 = pHttpRequest.Headers["Authorization"];
            Guid usuarioID = new Guid();
            bool acceso = false;
            if (pNombrePassBase64 != null)
            {
                byte[] nombrePassBase64 = Convert.FromBase64String(pNombrePassBase64.Substring(pNombrePassBase64.IndexOf(' ')));
                string nombrePass = System.Text.Encoding.UTF8.GetString(nombrePassBase64);
                string nombre = nombrePass.Substring(0, nombrePass.IndexOf(':'));
                string pass = nombrePass.Substring(nombrePass.IndexOf(':') + 1);
                try
                {
                    UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<UsuarioCN>(), mLoggerFactory);
                    DataWrapperUsuario dataWrapperUsuario = usuarioCN.AutenticarLogin(nombre, false);
                    AD.EntityModel.Models.UsuarioDS.Usuario filaUsuario = dataWrapperUsuario.ListaUsuario.FirstOrDefault();
                    acceso = usuarioCN.ValidarPasswordUsuarioParaSolicitud(filaUsuario, pass);

                    if (acceso)
                    {
                        usuarioID = filaUsuario.UsuarioID;
                    }
                }
                catch (Exception ex) 
                {
                    mLoggingService.GuardarLogError(ex, mlogger);
                }
            }
            if (usuarioID == new Guid())
            {
                //pHttpResponse.Status = "401 Authorization Required";
                pHttpResponse.StatusCode = 401;
                //pHttpResponse.StatusDescription = "Authorization Required";
                pHttpResponse.Headers.Add("WWW-Authenticate", "BASIC realm=\"www.gnoss.com\"");
                //pHttpResponse.End();
            }
            return usuarioID;
        }

        /// <summary>
        /// Método encargado de loguear a un usuario RSS
        /// </summary>
        /// <param name="pHttpRequest"></param>
        /// <param name="pHttpResponse"></param>
        /// <returns>Guid del usuario logueado</returns>
        public Guid LoginRSSPrivado(HttpRequest pHttpRequest, HttpResponse pHttpResponse, Guid pProyectoID, string pUrlOriginal)
        {
            string pNombrePassBase64 = pHttpRequest.Headers["Authorization"];
            Guid usuarioID = new Guid();
            bool acceso = false;
            if (pNombrePassBase64 != null)
            {
                byte[] nombrePassBase64 = Convert.FromBase64String(pNombrePassBase64.Substring(pNombrePassBase64.IndexOf(' ')));
                string nombrePass = System.Text.Encoding.UTF8.GetString(nombrePassBase64);
                string nombre = nombrePass.Substring(0, nombrePass.IndexOf(':'));
                string pass = nombrePass.Substring(nombrePass.IndexOf(':') + 1);
                try
                {
                    UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<UsuarioCN>(), mLoggerFactory);
                    DataWrapperUsuario dataWrapperUsuario = usuarioCN.AutenticarLogin(nombre, false);
                    AD.EntityModel.Models.UsuarioDS.Usuario filaUsuario = dataWrapperUsuario.ListaUsuario.FirstOrDefault();
                    acceso = usuarioCN.ValidarPasswordUsuarioParaSolicitud(filaUsuario, pass);

                    if (acceso)
                    {
                        if (usuarioCN.EstaUsuarioEnProyecto(filaUsuario.UsuarioID, pProyectoID))
                        {
                            usuarioID = filaUsuario.UsuarioID;
                        }
                    }
                }
                catch (Exception ex) 
                {
                    mLoggingService.GuardarLogError(ex, mlogger);
                }
            }
            if (usuarioID == new Guid())
            {
                string autoRedirect = "<html><head><meta http-equiv='refresh' content='1;url=" + pUrlOriginal + "'></head><body>Redirigiendo a la página anterior, si no se redirige pulse <a href='" + pUrlOriginal + "'>aquí</a>.</body></html>";
                //pHttpResponse.Status = "401 Authorization Required";
                pHttpResponse.StatusCode = 401;
                //pHttpResponse.StatusDescription = "Authorization Required";
                pHttpResponse.Headers.Add("WWW-Authenticate", "BASIC realm=\"www.gnoss.com\"");
                pHttpResponse.WriteAsync(autoRedirect);
                //pHttpResponse.End();
            }
            return usuarioID;
        }

        /// <summary>
        /// Método que devuelve un string en formato html que representa la imagen del documento
        /// </summary>
        /// <param name="pDocWeb">Documento del que se quiere obtener la imagen</param>
        /// <param name="pURLBase">URLBase de donde se encuentran las imágenes</param>
        /// <returns>String en formato html que representa la imagen del documento</returns>
        public static string crearHtmlImagenDocumento(string pTitulo, string pImagen)
        {
            string htmlimagen = "";
            if (!string.IsNullOrEmpty(pImagen))
            {
                htmlimagen = "<img style=\"margin:15px\" align=\"left\" height=\"54\" width=\"71\" alt=\"" + pTitulo + "\" src=\"" + pImagen + "\"/>";
            }

            return htmlimagen;
        }

        /// <summary>
        /// Método que devuelve un string en formato html que representa las etiquetas de un documento
        /// </summary>
        /// <param name="pDocWeb">Documento del que se quieren obtener las etiquetas</param>
        /// <param name="pURLBaseEtiquetas">URLBase del link de las etiquetas</param>
        /// <returns>String en formato html que representa las etiquetas de un documento</returns>
        public static string crearHtmlEtiquetasDocumento(List<string> pListaTags, string pURLBaseEtiquetas)
        {
            string htmlEtiquetas = "<p>Etiquetas:";
            if (pURLBaseEtiquetas.Contains("/en/"))
            {
                htmlEtiquetas = "<p>Tags:";
            }
            int i = 0;
            foreach (string etiqueta in pListaTags)
            {
                if (i == 0)
                {
                    htmlEtiquetas += " " + "<a href=\"" + pURLBaseEtiquetas + "/tag/" + etiqueta + "\">" + etiqueta + "</a>";
                }
                else
                {
                    htmlEtiquetas += ", " + "<a href=\"" + pURLBaseEtiquetas + "/tag/" + etiqueta + "\">" + etiqueta + "</a>";
                }
                i++;
            }
            htmlEtiquetas += "</p>";
            return htmlEtiquetas;
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="pHttpRequest"></param>
        ///// <returns></returns>
        //public static UtilIdiomas obtenerUtilIdiomasDeRequest(HttpRequest pHttpRequest, Guid pOrganizacionID, Guid pProyectoID)
        //{
        //    string idioma = "es";
        //    if (pHttpRequest.PathInfo.StartsWith("/en/"))
        //    {
        //        idioma = "en";
        //    }
        //    if (pHttpRequest.PathInfo.StartsWith("/pt/"))
        //    {
        //        idioma = "pt";
        //    }
        //    UtilIdiomas utilIdiomas = new UtilIdiomas(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + Path.DirectorySeparatorChar + "languages", new string[0], idioma, Guid.Empty, Guid.Empty, Guid.Empty);
        //    return utilIdiomas;
        //}
    }
}
