namespace Traduccion.AccesoADatos
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ProyectoGadget")]
    public partial class ProyectoGadget
    {
        [Key]
        [Column(Order = 0)]
        public Guid OrganizacionID { get; set; }

        [Key]
        [Column(Order = 1)]
        public Guid ProyectoID { get; set; }

        [Key]
        [Column(Order = 2)]
        public Guid GadgetID { get; set; }

        [Required]
        public string Titulo { get; set; }

        [Required]
        public string Contenido { get; set; }

        public int Orden { get; set; }

        public short Tipo { get; set; }

        [Required]
        [StringLength(10)]
        public string Ubicacion { get; set; }

        [StringLength(100)]
        public string Clases { get; set; }

        public short TipoUbicacion { get; set; }

        public bool Visible { get; set; }

        public bool MultiIdioma { get; set; }

        public Guid? PersonalizacionComponenteID { get; set; }

        public bool CargarPorAjax { get; set; }

        public string ComunidadDestinoFiltros { get; set; }

        [Required]
        [StringLength(50)]
        public string NombreCorto { get; set; }
    }
}
