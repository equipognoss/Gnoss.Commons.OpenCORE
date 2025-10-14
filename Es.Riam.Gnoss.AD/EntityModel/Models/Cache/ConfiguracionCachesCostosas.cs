using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Cache
{
	[Serializable]
	[Table("ConfiguracionCachesCostosas")]
	public partial class ConfiguracionCachesCostosas
	{
		public Guid OrganizacionID { get; set; }
		public Guid ProyectoID { get; set; }
		public bool CachesDeBusquedasActivas { get; set; }
		public bool CachesAnonimas { get; set; }
		public long TiempoExpiracion { get; set; }
		public long TiempoExpiracionCachesDeUsuario { get; set; }
		public long DuracionConsulta { get; set; }
		public long TiempoRecalcularCaches { get; set; }

		public virtual Proyecto Proyecto { get; set; }
	}
}
