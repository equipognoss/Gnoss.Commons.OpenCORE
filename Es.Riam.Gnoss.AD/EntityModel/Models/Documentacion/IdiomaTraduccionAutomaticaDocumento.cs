using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion
{
    [Serializable]
    [Table("IdiomaTraduccionAutomaticaDocumento")]
    public class IdiomaTraduccionAutomaticaDocumento
    {
        public Guid DocumentoID { get; set; }
        public string Idioma { get; set; }
        public virtual Documento Documento { get; set; }
    }
}