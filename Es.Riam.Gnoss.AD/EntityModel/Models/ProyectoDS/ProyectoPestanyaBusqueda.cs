using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS
{
    [Serializable]
    [Table("ProyectoPestanyaBusqueda")]
    public partial class ProyectoPestanyaBusqueda
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ProyectoPestanyaBusqueda()
        {
            ProyectoPestanyaBusquedaExportacion = new HashSet<ProyectoPestanyaBusquedaExportacion>();
        }

        [Key]
        public Guid PestanyaID { get; set; }

        [Required(AllowEmptyStrings = true)]
        public string CampoFiltro { get; set; }

        public short NumeroRecursos { get; set; }

        [Required]
        [StringLength(10)]
        public string VistaDisponible { get; set; }

        public bool MostrarFacetas { get; set; }

        public bool MostrarCajaBusqueda { get; set; }

        public Guid? ProyectoOrigenID { get; set; }

        public bool OcultarResultadosSinFiltros { get; set; }

        public string PosicionCentralMapa { get; set; }

        public string GruposConfiguracion { get; set; }

        public bool GruposPorTipo { get; set; }

        public string TextoBusquedaSinResultados { get; set; }

        public string TextoDefectoBuscador { get; set; }

        public bool MostrarEnComboBusqueda { get; set; }

        public bool IgnorarPrivacidadEnBusqueda { get; set; }

        public bool OmitirCargaInicialFacetasResultados { get; set; }
        
        public string RelacionMandatory { get; set; }

        public string SearchPersonalizado {  get; set; }

        [DefaultValue(TipoAutocompletar.Basico)]
        public TipoAutocompletar TipoAutocompletar { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ProyectoPestanyaMenu ProyectoPestanyaMenu { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProyectoPestanyaBusquedaExportacion> ProyectoPestanyaBusquedaExportacion { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProyectoPestanyaBusquedaPesoOC> ProyectoPestanyaBusquedaPesoOC { get; set; }
    }

    public enum TipoAutocompletar
    {
        Basico,
        Enriquecido,
    }
}
