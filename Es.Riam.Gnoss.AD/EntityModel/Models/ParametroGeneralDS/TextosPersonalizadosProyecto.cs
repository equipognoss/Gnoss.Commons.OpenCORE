using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Es.Riam.Gnoss.AD.EntityModel.Models.ParametroGeneralDS
{
    [Serializable]
    [Table("TextosPersonalizadosProyecto")]
    public partial class TextosPersonalizadosProyecto
    {
       
        public TextosPersonalizadosProyecto(Guid pGuid, string pTextoId, string pLenguaje, string pTexto)
        {
            ProyectoID = pGuid;
            TextoID = pTextoId;
            Language = pLenguaje;
            Texto = pTexto;
        }

        public TextosPersonalizadosProyecto(){ }

        [Column(Order = 0)]
        public Guid OrganizacionID { get; set; }

        [Column(Order = 1)]
        public Guid ProyectoID { get; set; }

        [Column(Order = 2)]
        [StringLength(100)]
        public string TextoID { get; set; }

        [Column(Order = 3)]
        [StringLength(100)]
        public string Language { get; set; }

        public string Texto { get; set; }

        public override bool Equals(object objeto)
        {
            TextosPersonalizadosProyecto comparado = (TextosPersonalizadosProyecto)objeto;
            return this.ProyectoID.Equals(comparado.ProyectoID) && this.TextoID.Equals(comparado.TextoID) && this.Language.Equals(comparado.Language) && this.Texto.Equals(comparado.Texto);
        }
    }
}
