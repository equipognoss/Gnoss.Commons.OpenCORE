using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Notificacion
{
    [Serializable]
    [Table("ColaTwitter")]
    public partial class ColaTwitter
    {
        [Key]
        public int ColaID { get; set; }

        public Guid PerfilID { get; set; }

        [StringLength(1000)]
        public string TokenTwitter { get; set; }

        [StringLength(1000)]
        public string TokenSecretoTwitter { get; set; }

        [Required]
        [StringLength(1000)]
        public string Mensaje { get; set; }

        [Required]
        [StringLength(1000)]
        public string Enlace { get; set; }
        [StringLength(1000)]
        public string ConsumerKey { get; set; }

        [StringLength(1000)]
        public string ConsumerSecret { get; set; }

        public short NumIntentos { get; set; }
    }
}
