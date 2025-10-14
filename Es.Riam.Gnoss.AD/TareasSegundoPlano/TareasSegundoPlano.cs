using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.TareasSegundoPlano
{
	public class TareasSegundoPlano
	{
		[Key]
		public Guid Id { get; set; }

		public Guid ProyectoID { set; get; }
		public Guid OrganizacionID { set; get; }

		public string Tipo { get; set; }
		public string Nombre { get; set; }
		public EstadoTarea Estado { get; set; }
		public DateTime FechaInicio { get; set; }
		public int EventosTotales { get; set; }
		public virtual Proyecto Proyecto { get; set; }
	}

	public enum EstadoTarea
	{
		Pendiente = 0,
		EnProceso = 1,
		Error = 2,
		Finalizado = 3,
	}
}
