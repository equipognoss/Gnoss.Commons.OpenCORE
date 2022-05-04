using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.IdentidadDS
{
    [NotMapped]
    [Serializable]
    public class IdentidadPerfil
    {
        public Guid PerfilID { get; set; }

        public string NombrePerfil { get; set; }

        public bool Eliminado { get; set; }

        public string NombreCortoUsu { get; set; }

        public Guid? PersonaID { get; set; }

        public bool TieneTwitter { get; set; }

        public int CaducidadResSusc { get; set; }
        
        public Guid IdentidadID { get; set; }        

        public Guid OrganizacionID { get; set; }

        public Guid ProyectoID { get; set; }


        public DateTime FechaAlta { get; set; }

        public int NumConnexiones { get; set; }

        public short Tipo { get; set; }
        public string NombreCortoIdentidad { get; set; }

        public bool RecibirNewsLetter { get; set; }

        public double? Rank { get; set; }

        public bool MostrarBienvenida { get; set; }

        public int DiasUltActualizacion { get; set; }

        public double ValorAbsoluto { get; set; }

        public bool ActivoEnComunidad { get; set; }

        public bool ActualizaHome { get; set; }

         public string Foto { get; set; }
        public Guid PerfilID1 { get; set; }
    }

}
