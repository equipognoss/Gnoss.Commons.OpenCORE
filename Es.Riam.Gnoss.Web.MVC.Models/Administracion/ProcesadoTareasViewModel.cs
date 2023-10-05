using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
    [Serializable]
    public class ProcesadoTareasViewModel
    {
        public string PaginaSiguiente { get; set; }
        public string UrlPeticion { get; set; }
        public string BodyPeticion { get; set; }
        public List<string> Mensajes { get; set; }
    }
}
