using Es.Riam.Gnoss.AD.ServiciosGenerales;
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
    public class ObtenerComunidades
    {
        public Guid ProyectoID { get; set; }
        public string Nombre { get; set; }
        public string NombreCorto { get; set; }
        public string NombrePresentacion { get; set; }
        public string Descripcion { get; set; }
        public int? NumeroOrgRegistradas { get; set; }
        public int? NumeroMiembros { get; set; }
        public int? NumeroRecursos { get; set; }
        public short TipoAcceso { get; set; }
        public string Tags { get; set; }
        public string NombreImagenPeque { get; set; }
        public DateTime? FechaInicio { get; set; }

        public override bool Equals(object obj)
        {
            ObtenerComunidades objetoParametro = null;
            if (obj.GetType() != typeof(ObtenerComunidades))
            {
                return false;
            }
            else if (ReferenceEquals(null, obj))
            {
                return false;
            }
            else
            {
                objetoParametro = (ObtenerComunidades)obj;
            }

            if (this.ProyectoID.Equals(objetoParametro.ProyectoID) && this.Nombre.Equals(objetoParametro.Nombre) && this.NombreCorto.Equals(objetoParametro.NombreCorto) && (this.NombrePresentacion.Equals(objetoParametro.NombrePresentacion) || this.NombrePresentacion.Equals("")) && this.Descripcion.Equals(objetoParametro.Descripcion) && this.NumeroOrgRegistradas.Equals(objetoParametro.NumeroOrgRegistradas) && this.NumeroMiembros.Equals(objetoParametro.NumeroMiembros) && this.NumeroRecursos.Equals(objetoParametro.NumeroRecursos) && this.TipoAcceso.Equals(objetoParametro.TipoAcceso) && this.Tags.Equals(objetoParametro.Tags) && this.NombreImagenPeque.Equals(objetoParametro.NombreImagenPeque))
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
