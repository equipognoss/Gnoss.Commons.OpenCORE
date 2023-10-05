using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
    
    public class AplicacionEspecifica
    {
        public Guid AplicacionID { get; set; }
        public string NombreApp { get; set; }
        public string NombreCorto { get; set; }
        public string RutaRepositorio { get; set; }
        public string TokenGit { get; set; }
        public string DockerFile { get; set; }
        public bool Tipo { get; set; }
        public string RutaRelativa { get; set; }
        public Guid PersonalizacionID { get; set; }
        public List<Variables> ListaVariables { get; set; }
        public List<VolumenAplicacion> ListaVolumenesAplicaciones { get; set; }
        public List<Volumen> ListaVolumenes { get; set; }
        public string nombreOriginal { get; set; }
        
    }
}
