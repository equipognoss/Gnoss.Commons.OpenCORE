namespace Traduccion.AccesoADatos
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("OntologiaProyecto")]
    public partial class OntologiaProyecto
    {
        [Key]
        [Column(Order = 0)]
        public Guid OrganizacionID { get; set; }

        [Key]
        [Column(Order = 1)]
        public Guid ProyectoID { get; set; }

        [Key]
        [Column("OntologiaProyecto", Order = 2)]
        [StringLength(100)]
        public string OntologiaProyecto1 { get; set; }

        public string NombreOnt { get; set; }

        [StringLength(100)]
        public string Namespace { get; set; }

        [StringLength(1000)]
        public string NamespacesExtra { get; set; }

        public string SubTipos { get; set; }

        public string NombreCortoOnt { get; set; }

        public bool CachearDatosSemanticos { get; set; }

        public bool EsBuscable { get; set; }
    }
}
