namespace Es.Riam.Gnoss.AD.EntityModel.Models.CMS
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Serializable]
    [Table("CMSPropiedadComponente")]
    public partial class CMSPropiedadComponente
    {
        [Column(Order = 0)]
        public Guid ComponenteID { get; set; }

        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short TipoPropiedadComponente { get; set; }

        [Required]
        public string ValorPropiedad { get; set; }

        public virtual CMSComponente CMSComponente { get; set; }
    }
}
