using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
//using static Es.Riam.Gnoss.Web.MVC.Controllers.Administracion.AdministrarFusionCambiosController;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
    public class AdministrarCambiosModel
    {
        public string TextArea { get; set; }
        public string FilePath { get; set; }
        public string Entorno { get; set; }
        public long Fecha { get; set; }
        public string TabModel { get; set; }
        public TipoFusion TipoFusion { get; set; }
        public string Tipo { get; set; }
        public string FacetaModel { get; set; }
        public string ContextoModel { get; set; }
        public string ComponenteCMSModel { get; set; }
        public string CookieModel { get; set; }
        public IFormFile FileInput { get; set; }
    }

    public enum TipoFusion
    {
        //Una modificacion de un archivo existente en los dos entornos
        Cambio,
        //En el entorno donde se ha generado el cambio se ha borrado el archivo
        Borrar,
        //En el entorno donde se ha generado el cambio se ha creado el archivo
        Crear
    }

}
