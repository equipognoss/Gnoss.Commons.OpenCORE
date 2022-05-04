using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Es.Riam.Gnoss.AD.EntityModel.Models.ParametroGeneralDS
{
    [Serializable]
    [Table("TextosPersonalizadosPersonalizacion")]
    public partial class TextosPersonalizadosPersonalizacion
    {
        public TextosPersonalizadosPersonalizacion(Guid personalizacionID, string textoID, string language, string texto)
        {
            PersonalizacionID = personalizacionID;
            TextoID = textoID;
            Language = language;
            Texto = texto;
        }
        public TextosPersonalizadosPersonalizacion() { }

        [Column(Order = 0)]
        public Guid PersonalizacionID { get; set; }

        [Column(Order = 1)]
        [StringLength(100)]
        public string TextoID { get; set; }

        [Column(Order = 2)]
        [StringLength(100)]
        public string Language { get; set; }

        public string Texto { get; set; }

        public override bool Equals(object objeto)
        {
            TextosPersonalizadosPersonalizacion comparado = (TextosPersonalizadosPersonalizacion) objeto;
            return this.PersonalizacionID.Equals(comparado.PersonalizacionID) && this.TextoID.Equals(comparado.TextoID) && this.Language.Equals(comparado.Language) && this.Texto.Equals(comparado.Texto);
        }
    }
}
