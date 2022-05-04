using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Es.Riam.Gnoss.AD.EntityModel.Models.VistaVirtualDS
{

    [Serializable]
    [Table("VistaVirtualProyecto")]
    public partial class VistaVirtualProyecto : IDisposable
    {
        private bool disposed = false;

        [Column(Order = 0)]
        public Guid OrganizacionID { get; set; }

        [Column(Order = 1)]
        public Guid ProyectoID { get; set; }

        [Column(Order = 2)]
        public Guid PersonalizacionID { get; set; }

        public virtual VistaVirtualPersonalizacion VistaVirtualPersonalizacion { get; set; }

        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                //if(VistaVirtualPersonalizacion != null) //Falla porque intenta traerlo con Entity
                //{
                //    VistaVirtualPersonalizacion.Dispose();
                //}
            }
        }

        ~VistaVirtualProyecto()
        {
            Dispose();
        }
    }
}
