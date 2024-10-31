using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.Web.MVC.Models.ViewModels
{
    [Serializable]
    public class Error500ViewModel
    {
        public bool ShowError { get; set; }
        public string Error { get; set; }
        public string ErrorTrace{ get; set; }
}
}
