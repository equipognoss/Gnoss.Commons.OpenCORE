using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
    public class AdministrarSharepointViewModel
    {
        public string DominioBase { get; set; }

        public string ClientID { get; set; }

        public string ClientSecret { get; set; }

        public string TenantID { get; set; }

        public string UrlAdminConsent { get; set; }

        public string UrlLoginSharepoint { get; set; }

        public string UrlObtenerTokenSharepoint { get; set; }

        public string UrlRedireccionSharepoint { get; set; }

        public bool PermisosConcedidos { get; set; }

    }
}
