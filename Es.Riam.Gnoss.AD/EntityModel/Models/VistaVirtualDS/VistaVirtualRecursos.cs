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
    public partial class VistaVirtualRecursos : IDisposable
    {
        private bool disposed = false;

        [Column(Order = 0)]
        public Guid PersonalizacionID { get; set; }

        [Column(Order = 1)]
        [StringLength(50)]
        public string RdfType { get; set; }

        [Required]
        [Column(TypeName = "NCLOB")]
        public string HTML { get; set; }

        public virtual VistaVirtualPersonalizacion VistaVirtualPersonalizacion { get; set; }

        public void Dispose()
        {
            if (!disposed)
            {
                if(VistaVirtualPersonalizacion != null)
                {
                    disposed = true;
                    VistaVirtualPersonalizacion.Dispose();
                    HTML = string.Empty;
                }
            }
        }

        ~VistaVirtualRecursos()
        {
            Dispose();
        }
    }
}
