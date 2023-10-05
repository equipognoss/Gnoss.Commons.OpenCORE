using Es.Riam.Gnoss.Web.MVC.Controles.Controladores;
using Es.Riam.Gnoss.Web.MVC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.Web.MVC.Controles
{
    public class MultiViewResult : JsonResult
    {

        private ControllerBaseGnoss mController;
        private ICompositeViewEngine mViewEngine;
        private List<View> views = new List<View>();

        public MultiViewResult(ControllerBaseGnoss controller, ICompositeViewEngine viewEngine)
            : base(null)
        {
            mController = controller;
            mViewEngine = viewEngine;
            //ContentType = "application/json";
        }

        public MultiViewResult AddView(string viewName, string containerId, object model = null)
        {
            views.Add(new View() { Kind = ViewKind.View, ViewName = viewName, ContainerId = containerId, Model = model });
            return this;
        }

        public MultiViewResult AddFirstView(string viewName, string containerId, object model = null)
        {
            views.Insert(0, new View() { Kind = ViewKind.View, ViewName = viewName, ContainerId = containerId, Model = model });
            return this;
        }

        public MultiViewResult AddContent(string content, string containerId)
        {
            views.Add(new View() { Kind = ViewKind.Content, Content = content, ContainerId = containerId });

            return this;
        }

        public MultiViewResult AddScript(string script)
        {
            views.Add(new View() { Kind = ViewKind.Script, Script = script });
            return this;
        }

        public override Task ExecuteResultAsync(ActionContext context)
        {
            //rendering views
            List<object> data = new List<object>();
            foreach (var view in views)
            {
                string html = string.Empty;
                if (view.Kind == ViewKind.View)
                {
                    //view result
                    html = RenderPartialViewToString(mController, view.ViewName, view.Model);
                    data.Add(new { updateTargetId = view.ContainerId, html = html });
                }
                else if (view.Kind == ViewKind.Content)
                {
                    //content result
                    html = view.Content;
                    data.Add(new { updateTargetId = view.ContainerId, html = html });
                }
                else if (view.Kind == ViewKind.Script)
                {
                    data.Add(new { script = view.Script });
                }

            }
            Value = data;

            return base.ExecuteResultAsync(context);
        }

        public override void ExecuteResult(ActionContext context)
        {
            //rendering views
            List<object> data = new List<object>();
            foreach (var view in views)
            {
                string html = string.Empty;
                if (view.Kind == ViewKind.View)
                {
                    //view result
                    html = RenderPartialViewToString(mController, view.ViewName, view.Model);
                    data.Add(new { updateTargetId = view.ContainerId, html = html });
                }
                else if (view.Kind == ViewKind.Content)
                {
                    //content result
                    html = view.Content;
                    data.Add(new { updateTargetId = view.ContainerId, html = html });
                }
                else if (view.Kind == ViewKind.Script)
                {
                    data.Add(new { script = view.Script });
                }

            }
            Value = data;

            base.ExecuteResult(context);
        }

        public string RenderPartialViewToString(ControllerBaseGnoss controller, string viewName, object model)
        {
            if (model != null)
                controller.ViewData.Model = model;
            try
            {
                using (StringWriter sw = new StringWriter())
                {
                    string personalizacion = ComprobarPersonalizacion(controller, viewName);

                    ViewEngineResult viewResult = mViewEngine.FindView(controller.ControllerContext, viewName + personalizacion, false);
                    if (viewResult.View == null) throw new Exception("View not found: " + viewName);
                    ViewContext viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw, new HtmlHelperOptions());
                    viewResult.View.RenderAsync(viewContext);

                    return sw.GetStringBuilder().ToString();
                }
            }
            catch (Exception)
            {
                //logging code
                throw;
            }
        }

        public static string ComprobarPersonalizacion(ControllerBaseGnoss controller, string viewName)
        {
            return ComprobarPersonalizacion(controller, (string)controller.ViewBag.ControllerName, viewName);
        }

        public static string ComprobarPersonalizacion(ControllerBaseGnoss controller, string pControllerName, string viewName)
        {
            return ComprobarPersonalizacion(controller.ViewBag, controller.Comunidad, pControllerName, viewName);
        }

        public static string ComprobarPersonalizacion(dynamic pViewBag, CommunityModel pComunidad, string viewName)
        {
            return ComprobarPersonalizacion(pViewBag, pComunidad, (string)pViewBag.ControllerName, viewName);
        }

        public static string ComprobarPersonalizacion(dynamic pViewBag, CommunityModel pComunidad, string pControllerName, string viewName)
        {
            if (pViewBag.CargandoPaginaCMS != null && pViewBag.CargandoPaginaCMS && !pControllerName.ToLower().Equals("cmspagina"))
            {
                pControllerName = "CMSPagina";
            }

            if (viewName.Equals("Error404"))
            {
                pControllerName = "Error";
            }

            string personalizacion = string.Empty;
            if ((!string.IsNullOrEmpty((string)pViewBag.Personalizacion) || !string.IsNullOrEmpty((string)pViewBag.PersonalizacionEcosistema) || !string.IsNullOrEmpty((string)pViewBag.PersonalizacionDominio)) && pComunidad != null)
            {
                string nombreVista = viewName;
                nombreVista = nombreVista.Replace("../Shared", "").Trim('/');
                nombreVista = nombreVista.Replace($"../{pControllerName}", "").Trim('/');
                List<string> listaPersonalizaciones = pComunidad.ListaPersonalizaciones.Select(item => item.ToLower()).ToList();
                List<string> listaPersonalizacionesDominio = pComunidad.ListaPersonalizacionesDominio.Select(item => item.ToLower()).ToList();
                List<string> listaPersonalizacionesEcosistema = pComunidad.ListaPersonalizacionesEcosistema.Select(item => item.ToLower()).ToList();
                
                if (listaPersonalizaciones.Contains($"/Views/{pControllerName}/{nombreVista}.cshtml".ToLower()) || listaPersonalizaciones.Contains($"/Views/Shared/{nombreVista}.cshtml".ToLower()))
                {
                    personalizacion = (string)pViewBag.Personalizacion;
                }
                else if (listaPersonalizacionesDominio != null && (listaPersonalizacionesDominio.Contains($"/Views/{pControllerName}/{nombreVista}.cshtml".ToLower()) || listaPersonalizacionesDominio.Contains($"/Views/Shared/{nombreVista}.cshtml".ToLower())))
                {
                    personalizacion = (string)pViewBag.PersonalizacionDominio;
                }
                else if (listaPersonalizacionesEcosistema.Contains($"/Views/{pControllerName}/{nombreVista}.cshtml".ToLower()) || listaPersonalizacionesEcosistema.Contains($"/Views/Shared/{nombreVista}.cshtml".ToLower()))
                {
                    personalizacion = (string)pViewBag.PersonalizacionEcosistema;
                }
            }

            return personalizacion;
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public enum ViewKind { View, UIMessage, Content, Script }
        class View
        {
            public ViewKind Kind { get; set; }

            public string ViewName { get; set; }
            public string Controllername { get; set; }
            public object Model { get; set; }
            public string Script { get; set; }
            public string ContainerId { get; set; }
            public string Content { get; set; }
        }

    }

    public class GnossResult : ActionResult
    {
        private ICompositeViewEngine mViewEngine;
        public enum GnossStatus
        {
            OK,
            Error,
            NOLOGIN
        }

        public Result result = new Result();

        public GnossResult(string pMessage, GnossStatus pStatus, ICompositeViewEngine viewEngine)
        {
            mViewEngine = viewEngine;
            result.Status = pStatus.ToString();
            if (pStatus == GnossStatus.NOLOGIN && (pMessage.StartsWith("http://") || pMessage.StartsWith("https://")))
            {
                result.UrlRedirect = pMessage;
            }
            else
            {
                result.Message = pMessage;
            }
        }

        public GnossResult(ControllerBaseGnoss pController, string pPartialViewName, object pModel, ICompositeViewEngine viewEngine)
        {
            mViewEngine = viewEngine;
            MultiViewResult multiViewResult = new MultiViewResult(null, mViewEngine);
            string html = multiViewResult.RenderPartialViewToString(pController, pPartialViewName, pModel);

            result.Status = GnossStatus.OK.ToString();
            result.Html = html;
        }

        public GnossResult(string pUrlRedirect, ICompositeViewEngine viewEngine)
        {
            mViewEngine = viewEngine;
            result.Status = GnossStatus.OK.ToString();
            result.UrlRedirect = pUrlRedirect;
        }

        public override Task ExecuteResultAsync(ActionContext context)
        {
            HttpResponse response = context.HttpContext.Response;

            if (context.HttpContext.Request.Headers.ContainsKey("Accept") && context.HttpContext.Request.Headers["Accept"].Contains("application/json"))
            {
                response.ContentType = "application/json";

                response.WriteAsync(JsonSerializer.Serialize(result));
            }
            else
            {
                if (result.Html != null)
                {
                    response.WriteAsync(result.Html);
                }
                else if (result.UrlRedirect != null)
                {
                    string destinationUrl = result.UrlRedirect;
                    //context.Controller.TempData.Keep();

                    try
                    {
                        response.Redirect(destinationUrl);
                    }
                    catch { }
                }
                else
                {
                    response.WriteAsync(result.Message);
                }
            }

            return base.ExecuteResultAsync(context);
        }

        /* Comentado porque el resultado ya es devuelto en ExecuteResultAsync.
         * public override void ExecuteResult(ActionContext context)
        {
            HttpResponse response = context.HttpContext.Response;

            if (context.HttpContext.Request.Headers.ContainsKey("Accept") && context.HttpContext.Request.Headers["Accept"].Contains("application/json"))
            {
                //response.ContentType = "application/json";

                response.WriteAsync(JsonSerializer.Serialize(result));                
            }
            else
            {
                if (result.Html != null)
                {
                    response.WriteAsync(result.Html);
                }
                else if (result.UrlRedirect != null)
                {
                    string destinationUrl = result.UrlRedirect;
                    //context.Controller.TempData.Keep();

                    try
                    {
                        response.Redirect(destinationUrl);
                    }
                    catch { }
                }
                else
                {
                    response.WriteAsync(result.Message);
                }
            }

            //Data = result;

            //base.ExecuteResult(context);
        }*/

        [Serializable]
        public class Result
        {
            public string Status { get; set; }
            public string Message { get; set; }
            public string Html { get; set; }
            public string UrlRedirect { get; set; }
        }
    }
}
