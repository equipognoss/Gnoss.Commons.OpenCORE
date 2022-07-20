using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS
{
    [Serializable]
    [Table("ProyectoGadgetContexto")]
    public partial class ProyectoGadgetContexto
    {
        public ProyectoGadgetContexto()
        {
            OrdenContexto = "";
            ServicioResultados = "";

        }
        [Column(Order = 0)]
        public Guid GadgetID { get; set; }

        [Column(Order = 1)]
        public Guid OrganizacionID { get; set; }

        [Column(Order = 2)]
        public Guid ProyectoID { get; set; }

        [Required]
        public string ComunidadOrigen { get; set; }

        public string ComunidadOrigenFiltros { get; set; }

        [Required]
        public string FiltrosOrigenDestino { get; set; }

        public string ComunidadDestinoFiltros { get; set; }

        public string OrdenContexto { get; set; }

        public short Imagen { get; set; }

        public short NumRecursos { get; set; }

        public string ServicioResultados { get; set; }

        public Guid ProyectoOrigenID { get; set; }

        public bool MostrarEnlaceOriginal { get; set; }

        public bool OcultarVerMas { get; set; }

        public string NamespacesExtra { get; set; }

        public string ItemsBusqueda { get; set; }

        public string ResultadosEliminar { get; set; }

        public bool NuevaPestanya { get; set; }

        [StringLength(50)]
        public string NombreCorto { get; set; }

        public bool ObtenerPrivados { get; set; }
        [NotMapped]
        public virtual ProyectoGadget ProyectoGadget { get; set; }
    }
}
