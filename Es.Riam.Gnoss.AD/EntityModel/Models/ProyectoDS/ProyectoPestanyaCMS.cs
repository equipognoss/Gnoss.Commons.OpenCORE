using Es.Riam.Gnoss.AD.EntityModel.Models.Flujos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS
{
    [Serializable]
    public partial class ProyectoPestanyaCMS
    {
        [Column(Order = 0)]
        public Guid PestanyaID { get; set; }

        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short Ubicacion { get; set; }

        public Guid? EstadoID { get; set; }

        public virtual ProyectoPestanyaMenu ProyectoPestanyaMenu { get; set; }

        public virtual Estado Estado { get; set; }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ICollection<HistorialTransicionPestanyaCMS> HistorialTransicionPestanyaCMS { get; set; }
	}
}
