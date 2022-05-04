using System;

namespace Es.Riam.Gnoss.Elementos.Documentacion
{
    public class DocumentoNewsletter
    {        
        #region Propiedades

        public Guid DocumentoID { get; set; }
        public Guid ProyectoID { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public Guid IdentidadID { get; set; }
        public string IdiomaDestinatarios { get; set; }
        public bool EnvioSolicitado { get; set; }
        public bool EnvioRealizado { get; set; }
        public string GruposID{ get; set; }
        public string HtmlNewsletter { get; set; }
        public DateTime Fecha { get; set; }

        #endregion
    }
}
