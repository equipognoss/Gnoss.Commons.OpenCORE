using System;
using System.ComponentModel.DataAnnotations;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
    
    public class Aplicacion
    {
        public string NombreApp { get; set; }
        public string NombreCorto { get; set; }
        public string RutaRepositorio { get; set; }
        public string TokenGit { get; set; }
        public string DockerFile { get; set; }
        //public string Variables { get; set; }
        public string Variables_des { get; set; }
        public string Variables_pre { get; set; }
        public string Variables_pro { get; set; }

    }
}
