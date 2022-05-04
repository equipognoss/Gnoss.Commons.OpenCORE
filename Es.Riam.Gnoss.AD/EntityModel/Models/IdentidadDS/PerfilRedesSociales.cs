using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.IdentidadDS
{
    [Serializable]
    [Table("PerfilRedesSociales")]
    public class PerfilRedesSociales
    {
        [Column(Order = 0)]
        public Guid PerfilID { get; set; }
        [Column(Order = 1)]
        public string NombreRedSocial { get; set; }
        
        public string urlUsuario { get; set; }
        
        public string Usuario { get; set; }
        
        public string Token { get; set; }
        
        public string TokenSecreto { get; set; }
    }
}
