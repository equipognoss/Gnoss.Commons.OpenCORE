using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.MVC
{
    [Serializable]
    public class ContactosPorID
    {
        public Guid IdentidadID { get; set; }
        public int TipoContacto { get; set; }
        public string NombrePerfil { get; set; }
        public string NombreCortoUsu { get; set; }
        public string NombreCortoOrg { get; set; }
        public string Foto { get; set; }
        public Guid GrupoID { get; set; }
        public string Nombre { get; set; }
        public Guid OrganizacionID { get; set; }

        public override bool Equals(object obj)
        {
            ContactosPorID objetoParametro = null;
            if (obj.GetType() != typeof(ContactosPorID))
            {
                return false;
            }
            else if (ReferenceEquals(null, obj))
            {
                return false;
            }
            else
            {
                objetoParametro = (ContactosPorID)obj;
            }

            if (IdentidadID.Equals(objetoParametro.IdentidadID) && TipoContacto.Equals(objetoParametro.TipoContacto) && NombrePerfil.Equals(objetoParametro.NombrePerfil) && NombreCortoUsu.Equals(objetoParametro.NombreCortoUsu) && Foto.Equals(objetoParametro.Foto) && (NombreCortoOrg.Equals(objetoParametro.NombreCortoOrg) || NombreCortoOrg.Equals("")))
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
