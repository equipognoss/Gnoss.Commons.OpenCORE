using Es.Riam.Util;
using System;
using System.Collections.Generic;
using System.IO;

namespace Es.Riam.Gnoss.Util.General
{
    public class ConfiguracionDespliegue
    {
        public Entornos Entornos { get; set; }
    }
    public class Actual
    {
        public string nombre_entorno { get; set; }
        public string server_front { get; set; }
        public string server_back { get; set; }
        public string ruta_front { get; set; }
        public string ruta_back { get; set; }
    }

    public class Entornos
    {
        public Actual Actual { get; set; }
        public Superior Superior { get; set; }
    }


    public class Superior
    {
        public string nombre_entorno { get; set; }
        public string server_front { get; set; }
        public string server_back { get; set; }
        public string ruta_front { get; set; }
        public string ruta_back { get; set; }
    }
}
