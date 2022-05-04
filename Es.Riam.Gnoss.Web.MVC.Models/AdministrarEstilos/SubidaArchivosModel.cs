using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;

namespace Es.Riam.Gnoss.Web.MVC.Models.AdministrarEstilos
{

    /// <summary>
    /// Modelo de una subida de archivos de estilos
    /// </summary>
    [Serializable]
    public class SubidaArchivosModel
    {
        public bool NewVersion { get; set; }
        public string NombreVersion { get; set; }
        public string TipoArchivos { get; set; }
        [JsonIgnore]
        public IFormFile ArchivoJS { get; set; }
        public string FicheroJS { get; set; }
        [JsonIgnore]
        public IFormFile ArchivoCSS { get; set; }
        public string FicheroCSS { get; set; }
        [JsonIgnore]
        public IFormFile ArchivoZIP { get; set; }
        public string FicheroZIP { get; set; }

    }
}
