namespace Es.Riam.Gnoss.AD.EntityModel.Models.UsuarioDS
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Serializable]
    [Table("ClausulaRegistro")]
    public partial class ClausulaRegistro
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ClausulaRegistro()
        {
            ProyRolUsuClausulaReg = new HashSet<ProyRolUsuClausulaReg>();
        }

        [Column(Order = 0)]
        public Guid ClausulaID { get; set; }

        [Column(Order = 1)]
        public Guid OrganizacionID { get; set; }

        [Column(Order = 2)]
        public Guid ProyectoID { get; set; }

        [Required]
        public string Texto { get; set; }

        public short Tipo { get; set; }

        public int Orden { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProyRolUsuClausulaReg> ProyRolUsuClausulaReg { get; set; }
    }
}
