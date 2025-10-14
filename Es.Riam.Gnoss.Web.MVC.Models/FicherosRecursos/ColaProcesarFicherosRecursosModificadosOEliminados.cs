using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.Web.MVC.Models.FicherosRecursos
{
    public class ColaProcesarFicherosRecursosModificadosOEliminados
    {
        public ColaProcesarFicherosRecursosModificadosOEliminados() 
        { 
        }

        public Guid DocumentoID { get; set; }
        public TipoEventoProcesarFicherosRecursos TipoEvento { get; set; }
    }

    public enum TipoEventoProcesarFicherosRecursos
    {
        Borrado,
        Modificado,
        BorradoPersistente
    }
}
