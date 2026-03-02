using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion 
{
	public sealed class PesoSubtiposOntoModel : IEquatable<PesoSubtiposOntoModel>
    {
		public string Ontologia {  get; set; }
		public string Subtipo {  get; set; }
		public int Peso {  get; set; }

		public bool Equals(PesoSubtiposOntoModel pOther)
		{
			if(pOther == null)
			{
				return false;
            }
			 if(ReferenceEquals(this, pOther))
			{
				return true;
            }

            return Ontologia == pOther.Ontologia && Subtipo == pOther.Subtipo && Peso == pOther.Peso;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as PesoSubtiposOntoModel);
        }

		public override int GetHashCode()
		{
			return HashCode.Combine(Ontologia, Subtipo, Peso);
        }
    }
}
