namespace Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Serializable]
    [Table("DocumentoLecturaAumentada")]
    public partial class DocumentoLecturaAumentada
    {
        [Key]
        public Guid DocumentoID { get; set; }

        public string TituloAumentado { get; set; }

        public string DescripcionAumentada { get; set; }

        public bool Validada { get; set; }

        public string EntitiesInfo { get; set; }

        public string TopicsInfo { get; set; }        
        public virtual Documento Documento { get; set; }
    }
}
