using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.Web.MVC.Models.ViewModels
{
    public class RegistrarOrgViewModel
    {
        public string PageName { get; set; }
        public Guid IDOrg { get; set; }
        public bool Error { get; set; }
        public bool Success { get; set; }
    }
}
