using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Cookies
{
    [Serializable]
    public partial class CategoryCookieModel
    {
        public CategoriaProyectoCookie CategoriaProyectoCookie { get; set; }
        public List<ProyectoCookie> ListaCookies { get; set; }
    }
}
