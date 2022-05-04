using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.Web.Util
{
    public class VirtualView : IView
    {
        private string _html;
        public VirtualView(string html, string path)
        {
            _html = html;
            Path = path;
        }
        public string Path { get; set; }

        public Task RenderAsync(ViewContext context)
        {
            return context.Writer.WriteAsync(_html);
        }
    }
}
