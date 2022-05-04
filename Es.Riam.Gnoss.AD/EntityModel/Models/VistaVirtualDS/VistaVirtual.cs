using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.VistaVirtualDS
{
    [Serializable]
    [Table("VistaVirtual")]
    public partial class VistaVirtual : IDisposable
    {
        private bool disposed = false;

        [Column(Order = 0)]
        public Guid PersonalizacionID { get; set; }

        [Column(Order = 1)]
        [StringLength(100)]
        public string TipoPagina { get; set; }

        [Required]
        public string HTML { get; set; }

        public virtual VistaVirtualPersonalizacion VistaVirtualPersonalizacion { get; set; }

        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                
                HTML = null;
            }
        }

        ~VistaVirtual()
        {
            Dispose();
        }
    }
}
