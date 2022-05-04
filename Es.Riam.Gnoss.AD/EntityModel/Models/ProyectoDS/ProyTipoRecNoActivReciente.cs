using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS
{
    [Serializable]
    [Table("ProyTipoRecNoActivReciente")]
    public partial class ProyTipoRecNoActivReciente
    {
        [Column(Order = 0)]
        public Guid ProyectoID { get; set; }

        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short TipoRecurso { get; set; }

        public string OntologiasID { get; set; }
    }
}
