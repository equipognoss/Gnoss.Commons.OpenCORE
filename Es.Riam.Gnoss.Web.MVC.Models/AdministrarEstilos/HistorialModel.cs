using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Web.MVC.Models.AdministrarEstilos
{
    [Serializable]
    public class HistorialModel
    {
        public HistorialModel() { }

        public List<string[]> Histroial { get; set; }
    }
}
