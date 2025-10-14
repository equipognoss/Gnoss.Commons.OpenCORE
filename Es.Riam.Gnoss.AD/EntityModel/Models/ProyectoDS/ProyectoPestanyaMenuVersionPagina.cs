using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Es.Riam.Gnoss.AD.EntityModel.Models.Faceta;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS
{
    [Serializable]
    [Table("ProyectoPestanyaMenuVersionPagina")]
    public partial class ProyectoPestanyaMenuVersionPagina
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ProyectoPestanyaMenuVersionPagina() {
            VersionID = Guid.Empty;
            PestanyaID = Guid.Empty;
            IdentidadID = Guid.Empty;
            VersionAnterior = Guid.Empty;
            Fecha = DateTime.Now;
            Comentario = "";
            ModeloJSON = "";
            ProyectoPestanyaMenuVersionPagina2 = null;
            ProyectoPestanyaMenuVersionPagina1 = new HashSet<ProyectoPestanyaMenuVersionPagina>();
        }

        public ProyectoPestanyaMenuVersionPagina(Guid VersionID, Guid PestanyaID, Guid IdentidadID, Guid VersionAnterior, DateTime Fecha, string Comentario, string ModeloJSON, ProyectoPestanyaMenuVersionPagina proyectoPestanyaMenuVersionPagina2)
        {
            this.VersionID = VersionID;
            this.PestanyaID = PestanyaID;
            this.IdentidadID = IdentidadID;
            this.VersionAnterior = VersionAnterior;
            this.Fecha = Fecha;
            this.Comentario = Comentario;
            this.ModeloJSON = ModeloJSON;
            this.ProyectoPestanyaMenuVersionPagina2 = proyectoPestanyaMenuVersionPagina2;
            this.ProyectoPestanyaMenuVersionPagina1 = new HashSet<ProyectoPestanyaMenuVersionPagina>();
        }

        public Guid VersionID { get; set; }
        public Guid PestanyaID { get; set; }
        public Guid IdentidadID { get; set; }
        public Guid? VersionAnterior { get; set; }
        public DateTime Fecha { get; set; }
        public string Comentario { get; set; }
        public string ModeloJSON { get; set; }
        public virtual ProyectoPestanyaMenu  ProyectoPestanyaMenu {get; set;}
        public virtual ProyectoPestanyaMenuVersionPagina ProyectoPestanyaMenuVersionPagina2 { get; set; }
        public virtual ICollection<ProyectoPestanyaMenuVersionPagina> ProyectoPestanyaMenuVersionPagina1 { get; set; }
    }
}
