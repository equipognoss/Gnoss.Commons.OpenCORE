using Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.AD.EntityModel.Comparadores
{
    class ComparadorDocumento : IEqualityComparer<Documento>
    {
        public bool Equals(Documento x, Documento y)
        {
            if (x.DocumentoID.Equals(y.DocumentoID))
            {
                return true;
            }
            return false;
        }
        public int GetHashCode(Documento obj)
        {
            return obj.DocumentoID.GetHashCode();
        }
    }
}
