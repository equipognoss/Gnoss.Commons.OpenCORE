using System;

namespace Es.Riam.Gnoss.Web.Controles.ServicioImagenesWrapper.Model

{
    public class GnossImage
    {
        public byte[] file { get; set; }
        public string relative_path { get; set; }
        public string name { get; set; }
        public string extension { get; set; }
    }

    public class GnossPersonImage : GnossImage
    {
        public Guid person_id { get; set; }
    }

    public class OrganizationPersonImage : GnossImage
    {
        public Guid organization_id { get; set; }
    }

    public class CopyPasteImageModel
    {
        public bool copy { get; set; }
        public Guid person_id_origin { get; set; }
        public Guid organization_id_origin { get; set; }
        public Guid document_id_origin { get; set; }
        public Guid document_id_destination { get; set; }
        public Guid person_id_destination { get; set; }
        public Guid organization_id_destination { get; set; }
        public string extension { get; set; }
        public string relative_path { get; set; }

    }
}
