using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS
{
    [Serializable]
    public class GrupoIdentidadesPorId
    {
        public Guid GrupoID { get; set; }
        public string Nombre { get; set; }
        public string NombreCorto { get; set; }
        public Guid? ProyectoID { get; set; }
        public string ProyectoNombreCorto { get; set; }
        public Guid? OrganizacionID { get; set; }

        public override bool Equals(object obj)
        {
            GrupoIdentidadesPorId objetoParametro = null;
            if (obj.GetType() != typeof(GrupoIdentidadesPorId))
            {
                return false;
            }
            else if (ReferenceEquals(null, obj))
            {
                return false;
            }
            else
            {
                objetoParametro = (GrupoIdentidadesPorId)obj;
            }

            if (GrupoID.Equals(objetoParametro.GrupoID) && Nombre.Equals(objetoParametro.Nombre) && NombreCorto.Equals(objetoParametro.NombreCorto) && (ProyectoID.Equals(objetoParametro.ProyectoID) || !ProyectoID.HasValue) && ProyectoNombreCorto.Equals(objetoParametro.ProyectoNombreCorto) && (OrganizacionID.Equals(objetoParametro.OrganizacionID) || !OrganizacionID.HasValue))
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
