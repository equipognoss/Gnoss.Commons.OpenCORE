using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;

namespace Es.Riam.Gnoss.Web.MVC.Controles
{
    public class SimpleResult : ActionResult
    {
        private Dictionary<string, string> mParamsHeader = null;
        private Stream mBody = null;
        private string mContentType = null;

        public SimpleResult(Dictionary<string, string> pParamsHeader, Stream pBody, string pContentType)
        {
            mParamsHeader = pParamsHeader;
            mBody = pBody;
            mContentType = pContentType;
        }

        public override void ExecuteResult(ActionContext context)
        {
            HttpResponse response = context.HttpContext.Response;
            //Preparamos cabecera          
            foreach (string nombre in mParamsHeader.Keys)
            {
                response.Headers[nombre] = mParamsHeader[nombre];
            }
            response.ContentType = mContentType;

            using (StreamReader sr = new StreamReader(mBody))
            {
                sr.BaseStream.Position = 0;
                response.WriteAsync(sr.ReadToEnd());
            }
        }
    }
}
