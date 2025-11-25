using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
    [Serializable]
    public class AdministrarRolesViewModel
    {
        public Dictionary<string, string> ListaIdiomas { get; set; }
        public string IdiomaPorDefecto { get; set; }
        public List<RolModel> ListaRoles { get; set; }
        public Dictionary<Guid, string> DiccionarioOntologias { get; set; }
        public bool PermitirDescargarDocUsuInvitado { get; set; }
        public List<PermisoModel> ListaPermisos { get; set; }
    }

    public partial class RolModel
    {
        public RolModel()
        {
        }

        public Guid ProyectoID { get; set; }

        public Guid RolID { get; set; }

        public short Tipo { get; set; }

        public string Nombre { get; set; }

        public string Descripcion { get; set; }

        public ulong PermisosAdministracion { get; set; }

        public ulong PermisosContenidos { get; set; }

        public ulong PermisosAdministracionEcosistema { get; set; }

        public ulong PermisosRecursos { get; set; }

        public string FechaModificacion { get; set; }

        public List<MiembroDeRol> ListaMiembros { get; set; }

        public List<PermisoRecursoSemantico> ListaPermisosRecursosSemanticos { get; set; }
        public bool Editable { get; set; }

        public bool Borrable { get; set; }

		public List<PermisoModel> ListaPermisos { get; set; }

		public bool EsRolUsuario { get; set; }
    }

	public partial class PermisoModel
	{
		public string Nombre { get; set; }

		public string Seccion { get; set; }

		public bool Concedido { get; set; }
	}

	public partial class PermisoRecursoSemantico
	{
		public Guid OntologiaID { get; set; }
		public string Nombre { get; set; }
		public bool PermisoCrear { get; set; }
		public bool PermisoEditar { get; set; }
		public bool PermisoEliminar { get; set; }
		public bool PermisoRestaurarVersion { get; set; }
		public bool PermisoEliminarVersion { get; set; }
	}

    public partial class CambiarRolUsuarioViewModel
    {
        public Guid IdentidadID { get; set; }

        public string NombreUsuario { get; set; }

        public List<RolModel> Roles { get; set; }

        public List<Guid> RolesYaTiene { get; set; }
        public List<RolHeredado> RolesHeredados { get;set; }
    }

    public partial class RolHeredado
    {
        public RolHeredado()
        {

        }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string GrupoOrigen { get;set; }
    }

    public partial class CambiarRolGrupoViewModel
    {
        public Guid GrupoID { get; set; }

        public string NombreGrupo { get; set; }

        public List<RolModel> Roles { get; set; }

        public List<Guid> RolesYaTiene { get; set; }
    }

    public partial class MiembroDeRol
    {
        public Guid ID { get; set; }

        public string Nombre { get; set; }

        public string NombreCompleto { get; set; }

        public string Url { get; set; }

        public string Foto { get; set; }

        public bool EsGrupo { get; set; }
    }
}
