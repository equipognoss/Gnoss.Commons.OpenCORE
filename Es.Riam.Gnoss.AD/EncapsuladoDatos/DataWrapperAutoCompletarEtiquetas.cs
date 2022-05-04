using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EncapsuladoDatos
{
    public class DataWrapperAutoCompletarEtiquetas
    {
        public List<ConfigAutocompletarProy> ListaConfigAutocompletarProys { get; set; }
        public List<ConfigSearchProy> ListaConfigSearchProy { get; set; }
        public DataWrapperAutoCompletarEtiquetas()
        {
            ListaConfigAutocompletarProys = new List<ConfigAutocompletarProy>();
            ListaConfigSearchProy = new List<ConfigSearchProy>();
        }
    }
}
