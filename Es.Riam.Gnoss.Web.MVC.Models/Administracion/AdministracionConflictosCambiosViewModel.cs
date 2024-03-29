﻿using Es.Riam.Gnoss.Web.MVC.Models.IntegracionContinua;
using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
    public class AdministracionConflictosCambiosViewModel
    {
        public Guid EntornoMaster;
        public Guid EntornoDevelop;
        public Guid EntornoRelease;
        public Dictionary<DateTime, List<ChangesContinuosIntegration>> DiccionarioConflictos { get; set; }
        public string Error { get; set; }
        public Guid Entorno { get; set; }
    }
}
