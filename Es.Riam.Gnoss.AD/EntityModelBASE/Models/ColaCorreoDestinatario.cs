using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModelBASE.Models
{
    [Serializable]
    [Table("ColaCorreoDestinatario")]
    public partial class ColaCorreoDestinatario
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CorreoID { get; set; }

        [StringLength(400)]
        public string Email { get; set; }

        [StringLength(250)]
        public string MascaraDestinatario { get; set; }

        public short Estado { get; set; }

        public DateTime? FechaProcesado { get; set; }

        public virtual ColaCorreo ColaCorreo { get; set; }
    }
}
