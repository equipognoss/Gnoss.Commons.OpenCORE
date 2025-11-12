using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
    public class AdministrarRolesControllerRolesViewModel
    {
        
    }
    public partial class RolModelController
    {
        public RolModelController()
        {
        }
        public Guid RolID { get; set; }
        public string Nombre { get; set; }
        public List<PermisoModelRolController> ListaPermisos { get; set; }
    }

    public partial class PermisoModelRolController
    {
        public string Nombre { get; set; }

        public string Seccion { get; set; }

        public bool Concedido { get; set; }
    }
}
