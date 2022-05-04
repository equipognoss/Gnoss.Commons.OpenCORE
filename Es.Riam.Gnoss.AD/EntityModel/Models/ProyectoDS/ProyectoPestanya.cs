using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS
{
    [Serializable]
    [Table("ProyectoPestanya")]
    public partial class ProyectoPestanya
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ProyectoPestanya()
        {
            ProyectoPestanyaRolGrupoIdentidades = new HashSet<ProyectoPestanyaRolGrupoIdentidades>();
            ProyectoPestanyaRolIdentidad = new HashSet<ProyectoPestanyaRolIdentidad>();
        }

        [Column(Order = 0)]
        public Guid ProyectoID { get; set; }

        [Column(Order = 1)]
        [StringLength(400)]
        public string Nombre { get; set; }

        public short Orden { get; set; }

        [Required]
        [StringLength(400)]
        public string Ruta { get; set; }

        public bool EsRutaInterna { get; set; }

        public bool EsSemantica { get; set; }

        [StringLength(500)]
        public string CampoFiltro { get; set; }

        public short? NumeroRecursos { get; set; }

        [StringLength(10)]
        public string VistaDisponible { get; set; }

        public bool NuevaPestanya { get; set; }

        public bool MostrarFacetas { get; set; }

        public bool MostrarCajaBusqueda { get; set; }

        public Guid? ProyectoOrigenID { get; set; }

        public bool Visible { get; set; }

        public bool CMS { get; set; }

        public bool OcultarResultadosSinFiltros { get; set; }

        public string posicionCentralMapa { get; set; }

        [StringLength(400)]
        public string NombrePestanyaPadre { get; set; }

        [StringLength(1000)]
        public string GruposConfiguracion { get; set; }

        public bool GruposPorTipo { get; set; }

        public string TextoBusquedaSinResultado { get; set; }

        public short Privacidad { get; set; }

        public string HTMLAlternativo { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProyectoPestanyaRolGrupoIdentidades> ProyectoPestanyaRolGrupoIdentidades { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProyectoPestanyaRolIdentidad> ProyectoPestanyaRolIdentidad { get; set; }
    }
}
