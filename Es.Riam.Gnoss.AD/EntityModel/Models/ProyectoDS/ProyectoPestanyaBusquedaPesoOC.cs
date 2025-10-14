using Es.Riam.Gnoss.AD.EntityModel.Models.Faceta;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS
{
    [Serializable]
    [Table("ProyectoPestanyaBusquedaPesoOC")]
    public class ProyectoPestanyaBusquedaPesoOC
    {
        public Guid OrganizacionID { get; set; }

        public Guid ProyectoID { get; set; }

        [Column("OntologiaProyecto")]
        [StringLength(100)]
        public string OntologiaProyecto1 { get; set; }

        public Guid PestanyaID { get; set; }

        [StringLength(100)]
        public string Tipo { get; set; }

        public short Peso { get; set; }

        public ProyectoPestanyaBusqueda ProyectoPestanyaBusqueda { get; set; }
        public OntologiaProyecto OntologiaProyecto { get; set; }
    }
}
