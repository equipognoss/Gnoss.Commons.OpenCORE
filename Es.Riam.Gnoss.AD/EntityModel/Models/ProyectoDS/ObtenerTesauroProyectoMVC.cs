using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS
{
    [Serializable]
    [NotMapped]
    public class ObtenerTesauroProyectoMVC
    {
        public Guid ProyectoID { get; set; }
        public Guid CategoriaTesauroID { get; set; }
        public string Nombre { get; set; }

        public override bool Equals(object obj)
        {
            ObtenerTesauroProyectoMVC objetoParametro = null;
            if (obj.GetType() != typeof(ObtenerTesauroProyectoMVC))
            {
                return false;
            }
            else if (ReferenceEquals(null, obj))
            {
                return false;
            }
            else
            {
                objetoParametro = (ObtenerTesauroProyectoMVC)obj;
            }
            
            if (this.ProyectoID.Equals(objetoParametro.ProyectoID) && this.Nombre.Equals(objetoParametro.Nombre) && this.CategoriaTesauroID.Equals(objetoParametro.CategoriaTesauroID))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
