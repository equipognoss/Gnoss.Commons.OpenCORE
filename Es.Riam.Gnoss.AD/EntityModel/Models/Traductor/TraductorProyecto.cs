using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Traductor
{
    [Serializable]
    [Table("TraductorProyecto")]
    public partial class TraductorProyecto
    {
        public Guid ProyectoID { get; set; }
        public Guid OrganizacionID { get; set; }
        public string Token{ get; set; }
        public string Endpoint { get; set; }
        public string Nivel { get; set; }
        public string Prompt { get; set; }
        public bool Activo {  get; set; }

        public virtual Proyecto Proyecto { get; set; }
    }
}
