using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.Web.MVC.Models
{
    [Serializable]
    [Table("ProyectoServicioWeb")]
    public partial class ProyectoServicioWeb
    {
        [Key]
        [Column(Order = 0)]
        public Guid OrganizacionID { get; set; }
        [Key]
        [Column(Order = 1)]
        public Guid ProyectoID { get; set; }
        // <summary>
        /// Nombre de la Aplicacion Web en el Servidor
        /// </summary>
        [Key]
        [Column(Order = 2)]
        public string AplicacionWeb { get; set; }

        // <summary>
        /// Nombre del servicio Web
        /// </summary>
        [Column(Order = 3)]
        public string Nombre { get; set; }
    }
}
