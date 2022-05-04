using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.MVC
{
    [Serializable]
    public class ParticipantesGrupoContactos
    {
        public Guid GrupoID { get; set; }
        public Guid IdentidadAmigoID { get; set; }
        public int Tipo { get; set; }
        public string Nombre { get; set; }
        public Guid IdentidadID { get; set; }

        public override bool Equals(object obj)
        {
            ParticipantesGrupoContactos objetoParametro = null;
            if (obj.GetType() != typeof(ParticipantesGrupoContactos))
            {
                return false;
            }
            else if (ReferenceEquals(null, obj))
            {
                return false;
            }
            else
            {
                objetoParametro = (ParticipantesGrupoContactos)obj;
            }

            if ((IdentidadID.Equals(objetoParametro.IdentidadID) || IdentidadID.Equals(Guid.Empty) || objetoParametro.IdentidadID.Equals(Guid.Empty))&& Tipo.Equals(objetoParametro.Tipo) && Nombre.Equals(objetoParametro.Nombre) && IdentidadAmigoID.Equals(objetoParametro.IdentidadAmigoID) && GrupoID.Equals(objetoParametro.GrupoID))
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
