using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS
{
    [Serializable]
    [Table("PresentacionMosaicoSemantico")]
    public partial class PresentacionMosaicoSemantico
    {
        [Column(Order = 0)]
        public Guid OrganizacionID { get; set; }

        [Column(Order = 1)]
        public Guid ProyectoID { get; set; }

        [Column(Order = 2)]
        public Guid OntologiaID { get; set; }

        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short Orden { get; set; }

        [StringLength(1000)]
        public string Ontologia { get; set; }

        public string mPropiedad;

        [Required]
        [StringLength(2000)]
        public string Propiedad
        {
            get
            {
                if (mPropiedad == null)
                {
                    return "";
                }
                return mPropiedad;
            }
            set
            {
                mPropiedad = value;
            }

        }

        [StringLength(1000)]
        public string Nombre { get; set; }

        [DefaultValue(false)]
        public bool MostrarEnAutocompletar { get; set; }

        public virtual Proyecto Proyecto { get; set; }
    }
}
