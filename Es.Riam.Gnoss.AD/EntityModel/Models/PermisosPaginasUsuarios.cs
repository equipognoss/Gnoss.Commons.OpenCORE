namespace Es.Riam.Gnoss.AD.EntityModel.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
 
    [Serializable]
    [Table("PermisosPaginasUsuarios")]
    public partial class PermisosPaginasUsuarios
    {
        [Column(Order = 0)]
        public Guid UsuarioID { get; set; }

        [Column(Order = 1)]
        public Guid OrganizacionID { get; set; }

        [Column(Order = 2)]
        public Guid ProyectoID { get; set; }

        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short Pagina { get; set; }
    }
}
