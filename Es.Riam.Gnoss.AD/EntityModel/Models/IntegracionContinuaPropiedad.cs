using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models
{
    public enum TipoObjeto
    {
        Pagina,
        Gadget,
        Componente,
        Faceta
    }

    public enum TipoPropiedad{
        RutaPagina,
        CampoFiltroPagina,
        ComunidadOrigenGadget,
        FiltrosDestinoGadget,
        FiltrosOrigenGadget,
        RelacionOrigenDestinoGadget,
        IdsRecursosComponente,
        TesauroComponente,
        FiltrosFaceta,
        EnlaceComponente
    }
    [Serializable]

    [Table("IntegracionContinuaPropiedad")]
    public class IntegracionContinuaPropiedad
    {
        [Column(Order = 0)]
        public Guid ProyectoID { get; set; }

        [Column(Order = 1)]
        public short TipoObjeto { get; set; }

        [Column(Order = 2)]
        [StringLength(250)]
        public string ObjetoPropiedad { get; set; }

        [Column(Order = 3)]
        public short TipoPropiedad { get; set; }

        [Required]
        [Column(Order = 4)]
        public string ValorPropiedad { get; set; }

        [Column(Order = 5)]
        public string ValorPropiedadDestino { get; set; }

        [Column(Order = 6)]
		[DefaultValue(true)]
        public bool MismoValor { get; set; }

        [Column(Order = 7)]
		[DefaultValue(false)]
        public bool Revisada { get; set; }
    }
}
