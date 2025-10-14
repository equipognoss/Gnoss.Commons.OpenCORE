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
		public string Rama { get; set; }
		public bool Tipo { get; set; }
        public int Version { get; set; }
        public short EstadoDespliegue { get; set; }
        public string RutaRelativa { get; set; }
        public Guid PersonalizacionID { get; set; }
        public List<Variables> ListaVariables { get; set; }
        public List<VolumenAplicacion> ListaVolumenes { get; set; }
        public List<Volumen> ListaVolumenesCompartidos { get; set; }
        public string nombreOriginal { get; set; }
        public Dictionary<Guid,string> VolumenCompartidoRuta { get; set; }
        public bool Delete { get; set; }
    }

    public class AplicacionEspecificaDatosEnviarDespliegue
    {
        public Guid AplicacionId { get; set; }
        public bool pDesplegarEntornoSuperior { get; set; }
        public string pProyectoId { get; set; }
        public Guid User { get; set; }
        public Guid Enviroment { get; set; }
    }
    public class AplicacionEspecificaDatos
    {
        public Guid AplicacionId { get; set; }
        public Guid User { get; set; }
        public Guid ProyectId { get; set; }
        public Guid Enviroment { get; set; }
    }
}
