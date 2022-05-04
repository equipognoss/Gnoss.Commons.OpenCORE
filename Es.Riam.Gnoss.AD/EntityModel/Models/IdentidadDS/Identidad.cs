using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.OrganizacionDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.UsuarioDS;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.IdentidadDS
{
    [Table("Identidad")]
    [Serializable]
    public partial class Identidad
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Identidad()
        {
            GrupoIdentidadesParticipacion = new HashSet<GrupoIdentidadesParticipacion>();
            DatoExtraProyectoVirtuosoIdentidad = new HashSet<DatoExtraProyectoVirtuosoIdentidad>();
            DatoExtraProyectoOpcionIdentidad = new HashSet<DatoExtraProyectoOpcionIdentidad>();
            ProyectoUsuarioIdentidad = new HashSet<ProyectoUsuarioIdentidad>();
        }
        public Guid IdentidadID { get; set; }

        public Guid PerfilID { get; set; }

        public Guid OrganizacionID { get; set; }

        public Guid ProyectoID { get; set; }

        public Guid? CurriculumID { get; set; }

        public DateTime FechaAlta { get; set; }

        public DateTime? FechaBaja { get; set; }

        public int NumConnexiones { get; set; }

        public short Tipo { get; set; }

        [Required]
        [StringLength(300)]
        public string NombreCortoIdentidad { get; set; }

        public DateTime? FechaExpulsion { get; set; }

        public bool RecibirNewsLetter { get; set; }

        public double? Rank { get; set; }

        public bool MostrarBienvenida { get; set; }

        public int DiasUltActualizacion { get; set; }

        public double ValorAbsoluto { get; set; }

        public bool ActivoEnComunidad { get; set; }

        public bool ActualizaHome { get; set; }

        [StringLength(200)]
        public string Foto { get; set; }

        [ForeignKey("PerfilID")]
        public virtual Perfil Perfil { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GrupoIdentidadesParticipacion> GrupoIdentidadesParticipacion { get; set; }

        public virtual IdentidadContadores IdentidadContadores { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

        public virtual ICollection<DatoExtraProyectoVirtuosoIdentidad> DatoExtraProyectoVirtuosoIdentidad { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DatoExtraProyectoOpcionIdentidad> DatoExtraProyectoOpcionIdentidad { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrganizacionParticipaProy> OrganizacionParticipaProyecto { get; set; }
        public virtual ICollection<ProyectoUsuarioIdentidad> ProyectoUsuarioIdentidad { get; set; }
    }
}
