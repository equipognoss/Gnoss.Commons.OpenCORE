using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS
{
    [NotMapped]
    [Serializable]
    public class ProyectoNumConexiones
    {

        public ProyectoNumConexiones()
        {

        }
        public Guid ProyectoID {
            get; set;
        }

        public string NombreCorto
        {
            get; set;
        }

        public string Nombre
        {
            get; set;
        }

        public short TipoAcceso
        {
            get;set;
        }
        public int NumConexiones
        {
            get; set;
        }
        public short TipoProyecto { get; set; }
    }
}
