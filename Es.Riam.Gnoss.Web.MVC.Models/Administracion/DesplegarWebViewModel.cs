using Es.Riam.Gnoss.Web.MVC.Models.IntegracionContinua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
    [Serializable]
    public class DesplegarWebViewModel : PeticionApiBasica
    {
        public string VersionWebActualizada {  get; set; }
        public bool HayIntegracionContinua { get; set; }
        public string Version {  get; set; }
        public int EstadoWeb { get; set; }
        public List<string> ListaVersionesDesplegadas { get; set; }
    }
}
