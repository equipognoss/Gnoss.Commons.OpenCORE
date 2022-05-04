using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion
{
    [Serializable]
    public class ObtenerRecursosEvento
    {
        public Guid DocumentoID { get; set; }
        public int TipoEvento { get; set; }
        public DateTime? FechaEvento { get; set; }
        public string Evento { get; set; }

        public override bool Equals(object obj)
        {
            ObtenerRecursosEvento objetoParametro = null;
            if (obj.GetType() != typeof(ObtenerRecursosEvento))
            {
                return false;
            }
            else if (ReferenceEquals(null, obj))
            {
                return false;
            }
            else
            {
                objetoParametro = (ObtenerRecursosEvento)obj;
            }

            if (DocumentoID.Equals(objetoParametro.DocumentoID) && TipoEvento.Equals(objetoParametro.TipoEvento) && FechaEvento.Equals(objetoParametro.FechaEvento) && Evento.ToLower().Equals(objetoParametro.Evento.ToLower()))
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
