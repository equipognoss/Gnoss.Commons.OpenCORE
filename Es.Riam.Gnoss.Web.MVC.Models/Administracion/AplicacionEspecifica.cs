using System;
using System.ComponentModel.DataAnnotations;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
    public class AplicacionEspecifica
    {
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string NombreCorto { get; set; }
        [Required]
        public string RutaRepositorio { get; set; }
        public string UrlRepositorio { get; set; }
        public string RutaArchivoCsproj { get; set; }
        public string NombreArchivoSln { get; set; }
        public string NombreEjecutable { get; set; }
        public short Tipo { get; set; }
        public bool Deleted { get; set; }
        public Guid ApplicacionID { get; set; }
    }
}
