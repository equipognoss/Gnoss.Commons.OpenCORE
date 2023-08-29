using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.Web.MVC.Models.OAuth
{
    public class OAuthModel
    {
        public OAuthModel()
        {

        }

        public OAuthModel(string pShortName)
        {
            this.ShortName = pShortName;
        }

        public string TokenKey { get; set; }
        public string TokenSecret { get; set; }
        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }
        public string API { get; set; }
        public string ShortName { get; set; }
        public string OntologyName { get; set; }
        public string DevEmail { get; set; }
        public string LogPath { get; set; }
        public string LogFileName { get; set; }
        public string LogLevel { get; set; }
    }
}
