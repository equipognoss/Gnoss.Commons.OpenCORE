using Es.Riam.Gnoss.Web.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Web.Util
{
    /// <summary>
    /// Class that renders MVC views to a string using the
    /// standard MVC View Engine to render the view. 
    /// </summary>
    public class ViewRenderer : Controller
    {
        private ICompositeViewEngine mViewEngine;

        public ViewRenderer(ICompositeViewEngine viewEngine)
        {
            mViewEngine = viewEngine;
        }

        /// <summary>
        /// Required Controller Context
        /// </summary>
        protected ControllerContext Context { get; set; }

        /// <summary>
        /// Initializes the ViewRenderer with a Context.
        /// </summary>
        /// <param name="controllerContext">
        /// If you are running within the context of an ASP.NET MVC request pass in
        /// the controller's context. 
        /// Only leave out the context if no context is otherwise available.
        /// </param>
        public ViewRenderer(ControllerContext controllerContext = null, ICompositeViewEngine viewEngine = null)
        {
            // Create a known controller from HttpContext if no context is passed
            if (controllerContext != null && viewEngine != null)
            {
                mViewEngine = viewEngine;
                Context = controllerContext;
            }
            
        }

        /// <summary>
        /// Renders a full MVC view to a string. Will render with the full MVC
        /// View engine including running _ViewStart and merging into _Layout        
        /// </summary>
        /// <param name="viewPath">
        /// The path to the view to render. Either in same controller, shared by 
        /// name or as fully qualified ~/ path including extension
        /// </param>
        /// <param name="model">The model to render the view with</param>
        /// <returns>String of the rendered view or null on error</returns>
        public string RenderView(string viewPath, object model, string phtml = "", ITempDataDictionary tempData = null)
        {
            return RenderViewToStringInternal(viewPath, model, tempData, phtml).Result;
        }

        /// <summary>
        /// Renders a partial MVC view to string. Use this method to render
        /// a partial view that doesn't merge with _Layout and doesn't fire
        /// _ViewStart.
        /// </summary>
        /// <param name="viewPath">
        /// The path to the view to render. Either in same controller, shared by 
        /// name or as fully qualified ~/ path including extension
        /// </param>
        /// <param name="model">The model to pass to the viewRenderer</param>
        /// <param name="controllerContext">Active Controller context</param>
        /// <returns>String of the rendered view or null on error</returns>
        public string RenderView(string viewPath, object model, ControllerContext controllerContext, string phtml = "",ITempDataDictionary tempData = null)
        {
            ViewRenderer renderer = new ViewRenderer(controllerContext, mViewEngine);
            return renderer.RenderView(viewPath, model, phtml, tempData);
        }

        /// <summary>
        /// Renders a partial MVC view to string. Use this method to render
        /// a partial view that doesn't merge with _Layout and doesn't fire
        /// _ViewStart.
        /// </summary>
        /// <param name="viewPath">
        /// The path to the view to render. Either in same controller, shared by 
        /// name or as fully qualified ~/ path including extension
        /// </param>
        /// <param name="model">The model to pass to the viewRenderer</param>
        /// <param name="controllerContext">Active Controller context</param>
        /// <param name="errorMessage">optional out parameter that captures an error message instead of throwing</param>
        /// <returns>String of the rendered view or null on error</returns>
        public string RenderView(string viewPath, object model,
                                        ControllerContext controllerContext, string phtml,
                                        out string errorMessage)
        {
            errorMessage = null;
            try
            {
                ViewRenderer renderer = new ViewRenderer(controllerContext, mViewEngine);
                return renderer.RenderView(viewPath, model, phtml);
            }
            catch (Exception ex)
            {
                errorMessage = ex.GetBaseException().Message;
            }
            return null;
        }

        /// <summary>
        /// Internal method that handles rendering of either partial or 
        /// or full views.
        /// </summary>
        /// <param name="viewPath">
        /// The path to the view to render. Either in same controller, shared by 
        /// name or as fully qualified ~/ path including extension
        /// </param>
        /// <param name="model">Model to render the view with</param>
        /// <param name="partial">Determines whether to render a full or partial view</param>
        /// <returns>String of the rendered view</returns>
        protected async Task<string> RenderViewToStringInternal(string viewPath, object model, ITempDataDictionary tempData = null, string phtml = "")
        {
            // first find the ViewEngine for this view
            IView view;
            if (string.IsNullOrEmpty(phtml))
            {
                //string path = viewPath.Replace("~/Views", "Views/temp");
                //using (FileStream fs = System.IO.File.Create(path))
                //{
                //    byte[] info = new UTF8Encoding(true).GetBytes(phtml);
                //    fs.Write(info, 0, info.Length);
                //}

                ViewEngineResult viewEngineResult = mViewEngine.FindView(Context, viewPath, false);

                if (viewEngineResult == null)
                    throw new FileNotFoundException();

                // get the view and attach the model to view data
                view = viewEngineResult.View;
                ViewData.Model = model;
                //System.IO.File.Delete(path);
            }
            else
            {
                view = new VirtualView(phtml, viewPath);
            }
            string result = null;
            if (TempData != null || tempData != null)
            {
                if(tempData != null && TempData == null)
                {
                    TempData = tempData;
                }
                using (var sw = new StringWriter())
                {
                    var ctx = new ViewContext(Context, view, ViewData, TempData, sw, new HtmlHelperOptions());
                    await view.RenderAsync(ctx);
                    result = sw.GetStringBuilder().ToString();
                }
            }
            return result;
        }
    }
}
