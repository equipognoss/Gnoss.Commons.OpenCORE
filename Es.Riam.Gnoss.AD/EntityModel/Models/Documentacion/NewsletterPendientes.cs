using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion
{
    [Serializable]
    public class NewsletterPendientes
    {
        //SELECT DocumentoEnvioNewsLetter.DocumentoID, DocumentoEnvioNewsLetter.IdentidadID, DocumentoEnvioNewsLetter.Fecha, Documento.ProyectoID, Documento.Titulo, Documento.Descripcion, DocumentoEnvioNewsLetter.Idioma, DocumentoEnvioNewsLetter.EnvioSolicitado, DocumentoEnvioNewsLetter.EnvioRealizado, DocumentoEnvioNewsLetter.Grupos, null as Newsletter
        public Guid DocumentoID { get; set; }
        public Guid IdentidadID { get; set; }
        public DateTime Fecha { get; set; }
        public Guid? ProyectoID { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public string Idioma { get; set; }
        public bool EnvioSolicitado { get; set; }
        public bool EnvioRealizado { get; set; }
        public string Grupos { get; set; }
        public string Newsletter { get; set; }
    }
}
