using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Tesauro
{
    [Serializable]
    public class ObtenerCategoriaDeRecursos
    {
        public Guid DocumentoID { get; set; }
        public Guid CategoriaTesauroID { get; set; }
        public string Nombre { get; set; }
        public Guid? CategoriaSuperiorID { get; set; }
        
        public override bool Equals(object obj)
        {
            ObtenerCategoriaDeRecursos objetoParametro = null;
            if (obj.GetType() != typeof(ObtenerCategoriaDeRecursos))
            {
                return false;
            }
            else if (ReferenceEquals(null, obj))
            {
                return false;
            }
            else
            {
                objetoParametro = (ObtenerCategoriaDeRecursos)obj;
            }

            if (DocumentoID.Equals(objetoParametro.DocumentoID) && CategoriaTesauroID.Equals(objetoParametro.CategoriaTesauroID) && Nombre.Equals(objetoParametro.Nombre) && CategoriaSuperiorID.Equals(objetoParametro.CategoriaSuperiorID))
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
