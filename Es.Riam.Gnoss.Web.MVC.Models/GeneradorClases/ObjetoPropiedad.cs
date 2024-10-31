using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.Web.MVC.Models.GeneradorClases
{
    public class ObjetoPropiedad
    {
        public enum TipoObjeto
        {
            image,
            link,
            textEditor,
            ArchivoLink,
            other
        }
        public string NombrePropiedad { get; set; }
        public string NombreEntidad { get; set; }
        public string NombreOntologia { get; set; }
        public string Rango { get; set; }
        public bool EsNullable { get; set; }
        public bool Multivaluada { get; set; }
        public bool Multiidioma { get; set; }
        public TipoObjeto Tipo { get; set; }

    }
}
