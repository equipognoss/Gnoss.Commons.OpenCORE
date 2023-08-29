using Es.Riam.Gnoss.Web.MVC.Models.IntegracionContinua;
using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
    public class AdministrarListaCambiosViewModel
    {
        public Guid EntornoMaster { get; set; }
        public Guid EntornoDevelop { get; set; }
        public Guid EntornoRelease { get; set; }
        public Dictionary<DateTime, List<ChangesContinuosIntegration>> DiccionarioCambios { get; set; }
        public Dictionary<DateTime, List<ChangesContinuosIntegration>> DiccionarioCambiosConflicto { get; set; }
        public string Error { get; set; }
        public Guid Entorno { get; set; }
    }
}

