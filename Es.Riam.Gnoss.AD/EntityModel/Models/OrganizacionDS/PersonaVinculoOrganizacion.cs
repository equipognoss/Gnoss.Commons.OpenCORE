using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.OrganizacionDS
{
    [Serializable]
    [Table("PersonaVinculoOrganizacion")]
    public partial class PersonaVinculoOrganizacion
    {
        [Column(Order = 0)]
        public Guid PersonaID { get; set; }

        [Column(Order = 1)]
        public Guid OrganizacionID { get; set; }

        [StringLength(255)]
        public string Cargo { get; set; }

        public Guid? PaisTrabajoID { get; set; }

        public Guid? ProvinciaTrabajoID { get; set; }

        [StringLength(255)]
        public string ProvinciaTrabajo { get; set; }

        [StringLength(15)]
        public string CPTrabajo { get; set; }

        [StringLength(1000)]
        public string DireccionTrabajo { get; set; }

        [StringLength(255)]
        public string LocalidadTrabajo { get; set; }

        [StringLength(13)]
        public string TelefonoTrabajo { get; set; }

        [StringLength(100)]
        public string Extension { get; set; }

        [StringLength(13)]
        public string TelefonoMovilTrabajo { get; set; }

        [StringLength(255)]
        public string EmailTrabajo { get; set; }

        public Guid? CategoriaProfesionalID { get; set; }

        public Guid? TipoContratoID { get; set; }

        public DateTime FechaVinculacion { get; set; }

        public byte[] Foto { get; set; }

        [StringLength(30)]
        public string CoordenadasFoto { get; set; }

        public bool UsarFotoPersonal { get; set; }

        public int? VersionFoto { get; set; }

        public DateTime? FechaAnadidaFoto { get; set; }

        public virtual Organizacion Organizacion { get; set; }
    }
}
