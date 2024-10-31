using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{

    public class AdministrarComunidadUtilidades
    {
        public bool WikiDisponible { get; set; }
        public List<PermisoDocumentacion> PermisosDocumentacion { get; set; }
        public List<PermisoDocumentacionSemantica> PermisosDocumentacionSemantica { get; set; }
        public bool NivelesCertificacionDisponibles { get; set; }
        public List<NivelCertificacion> NivelesCertificacion { get; set; }
        public string PoliticaCertificacion { get; set; }
        public bool PermitirDescargarDocUsuInvitado { get; set; }
        public Dictionary<string, string> ListaIdiomas { get; set; }
        public string IdiomaDefecto { get; set; }

        public class PermisoDocumentacion
        {
            public TiposDocumentacion TipoDocumento { get; set; }
            public short TipoPermiso { get; set; }
        }
        public class PermisoDocumentacionSemantica
        {
            public string TipoDocumento { get; set; }
            public string Ontologia { get; set; }
            public short TipoPermiso { get; set; }
            public Dictionary<Guid, string> PrivacidadGrupos { get; set; }
            public bool EsSecundaria { get; set; }
        }
        public class NivelCertificacion
        {
            public Guid CertificacionID { get; set; }
            public string Nombre { get; set; }
            public short Orden { get; set; }
            public bool TieneDocsAsociados { get; set; }
        }
    }
}
