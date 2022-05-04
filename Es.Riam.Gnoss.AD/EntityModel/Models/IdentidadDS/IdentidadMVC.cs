using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.IdentidadDS
{
    [Serializable]
    [NotMapped]
    public class IdentidadMVC
    {
        public Guid IdentidadID { get; set; }
        public short Tipo { get; set; }
        public string Foto { get; set; }
        public int? NumConnexiones { get; set; }
        public string NombreCortoOrg { get; set; }
        public string NombreCortoUsu { get; set; }
        public string NombreOrganizacion { get; set; }
        public string NombrePerfil { get; set; }
        public Guid? OrganizacionID { get; set; }
        public Guid? PersonaID { get; set; }
        public string Tags { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public bool TieneEmailTutor { get; set; }

        public override bool Equals(object obj)
        {
            IdentidadMVC objetoParametro = null;
            if (obj.GetType() != typeof(IdentidadMVC))
            {
                return false;
            }
            else if (ReferenceEquals(null, obj))
            {
                return false;
            }
            else
            {
                objetoParametro = (IdentidadMVC)obj;
            }
            
            if (IdentidadID.Equals(objetoParametro.IdentidadID) && Tipo.Equals(objetoParametro.Tipo) && (NombreCortoOrg.Equals(objetoParametro.NombreCortoOrg) || NombreCortoOrg.Equals("")) && (NombreOrganizacion.Equals(objetoParametro.NombreOrganizacion) || NombreOrganizacion.Equals("")) && NombreCortoUsu.Equals(objetoParametro.NombreCortoUsu) && NombrePerfil.Equals(objetoParametro.NombrePerfil) && (OrganizacionID.Equals(objetoParametro.OrganizacionID) || OrganizacionID.Equals(Guid.Empty)) && (PersonaID.Equals(objetoParametro.PersonaID) || PersonaID.Equals(Guid.Empty)) && Foto.Equals(objetoParametro.Foto) && (Tags.Equals(objetoParametro.Tags) || Tags.Equals("")) && NumConnexiones.Equals(objetoParametro.NumConnexiones))
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


