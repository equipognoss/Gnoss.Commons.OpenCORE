namespace Traduccion.AccesoADatos
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("FacetaObjetoConocimientoProyecto")]
    public partial class FacetaObjetoConocimientoProyecto
    {
        [Key]
        [Column(Order = 0)]
        public Guid OrganizacionID { get; set; }

        [Key]
        [Column(Order = 1)]
        public Guid ProyectoID { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(50)]
        public string ObjetoConocimiento { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(300)]
        public string Faceta { get; set; }

        public short Orden { get; set; }

        public bool Autocompletar { get; set; }

        public short? TipoPropiedad { get; set; }

        public short Comportamiento { get; set; }

        public bool MostrarSoloCaja { get; set; }

        public short? Excluida { get; set; }

        public bool? Oculta { get; set; }

        public short TipoDisenio { get; set; }

        public short ElementosVisibles { get; set; }

        public short AlgoritmoTransformacion { get; set; }

        [StringLength(5)]
        public string NivelSemantico { get; set; }

        public bool EsSemantica { get; set; }

        public short Mayusculas { get; set; }

        [Required]
        public string NombreFaceta { get; set; }

        public bool Excluyente { get; set; }

        public string SubTipo { get; set; }

        public short Reciproca { get; set; }

        public string FacetaPrivadaParaGrupoEditores { get; set; }

        public bool ComportamientoOr { get; set; }

        public bool OcultaEnFacetas { get; set; }

        public bool OcultaEnFiltros { get; set; }

        public string Condicion { get; set; }

        public bool PriorizarOrdenResultados { get; set; }

        public bool Inmutable { get; set; }

        public Guid? AgrupacionID { get; set; }
    }
}
