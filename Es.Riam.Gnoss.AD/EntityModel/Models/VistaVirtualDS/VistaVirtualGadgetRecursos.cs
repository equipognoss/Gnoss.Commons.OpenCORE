﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.VistaVirtualDS
{
    [Serializable]
    [Table("VistaVirtualGadgetRecursos")]
    public partial class VistaVirtualGadgetRecursos : IDisposable
    {
        private bool disposed = false;

        [Column(Order = 0)]
        public Guid PersonalizacionID { get; set; }

        [Column(Order = 1)]
        public Guid PersonalizacionComponenteID { get; set; }

        [Required]
        public string HTML { get; set; }

        [Required]
        [StringLength(50)]
        public string Nombre { get; set; }

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

        ~VistaVirtualGadgetRecursos()
        {
            Dispose();
        }
    }
}
