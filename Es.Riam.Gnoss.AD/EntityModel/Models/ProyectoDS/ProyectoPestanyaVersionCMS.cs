using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS
{
    [Serializable]
    [Table("ProyectoPestanyaVersionCMS")]
    public partial class ProyectoPestanyaVersionCMS
    {
        public ProyectoPestanyaVersionCMS()
        {

        }
        public ProyectoPestanyaVersionCMS(Guid VersionID, Guid PestanyaID, Guid IdentidadID, Guid VersionAnterior, DateTime Fecha, string Comentario, string ModeloJSON)
        {
            this.VersionID = VersionID;
            this.PestanyaID = PestanyaID;
            this.IdentidadID = IdentidadID;
            this.VersionAnterior = VersionAnterior;
            this.Fecha = Fecha;
            this.Comentario = Comentario;
            this.ModeloJSON = ModeloJSON;

        }

        [Key]
        public Guid VersionID { get; set; }

        public Guid PestanyaID { get; set; }

        public Guid IdentidadID { get; set; }

        public Guid VersionAnterior { get; set; }

        public DateTime Fecha { get; set; }

        public string Comentario { get; set; }

        public string ModeloJSON { get; set; }

        [ForeignKey("PestanyaID")]
        public virtual ProyectoPestanyaMenu ProyectoPestanyaMenu { get; set; }
    }
}
