using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Es.Riam.Gnoss.Web.MVC.Models.Administracion.TabModel;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
    public class AdministrarFacetasPestanyasModel
    {
        public List<FacetasTabModel> FacetasPestanyas { get; set; }
        public List<FacetaModel> ListadoFacetas { get; set; }
    }
}
