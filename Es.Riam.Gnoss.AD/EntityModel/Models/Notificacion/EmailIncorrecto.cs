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
    [Table("EmailIncorrecto")]
    public partial class EmailIncorrecto
    {
        [Key]
        [StringLength(100)]
        public string Email { get; set; }
    }
}
