using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModelBASE.Models
{
    [Serializable]
    [Table("ColaCorreo")]
    public partial class ColaCorreo
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ColaCorreo()
        {
            ColaCorreoDestinatario = new HashSet<ColaCorreoDestinatario>();
        }

        [Key]
        public int CorreoID { get; set; }

        [Required]
        [StringLength(250)]
        public string Remitente { get; set; }

        [Required]
        public string Asunto { get; set; }

        [Required]
        [Column(TypeName = "NCLOB")]
        public string HtmlTexto { get; set; }

        public bool EsHtml { get; set; }

        public short Prioridad { get; set; }

        public DateTime FechaPuestaEnCola { get; set; }

        [StringLength(250)]
        public string MascaraRemitente { get; set; }

        [StringLength(250)]
        public string DireccionRespuesta { get; set; }

        [StringLength(250)]
        public string MascaraDireccionRespuesta { get; set; }

        [Required]
        [StringLength(250)]
        public string SMTP { get; set; }

        [Required]
        [StringLength(100)]
        public string Usuario { get; set; }

        [Required]
        [StringLength(50)]
        public string Password { get; set; }

        public int Puerto { get; set; }

        public bool EsSeguro { get; set; }

        public bool EnviadoRabbit { get; set; }

        [Required]
        [StringLength(10)]
        public string tipo { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ColaCorreoDestinatario> ColaCorreoDestinatario { get; set; }
    }
}
