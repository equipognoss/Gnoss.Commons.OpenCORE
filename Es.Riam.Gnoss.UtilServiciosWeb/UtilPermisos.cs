using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Roles;
using Es.Riam.Gnoss.AD.RDF;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Documentacion;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.UtilServiciosWeb
{
	public class UtilPermisos
	{
		private EntityContext _entityContext;
		private LoggingService _loggingService;
		private ConfigService _configService;
        private ILogger mlogger;
        private ILoggerFactory mloggerFactory;

        public UtilPermisos(EntityContext pEntityContext, LoggingService pLoggingService, ConfigService pConfigService, ILogger<UtilPermisos> logger, ILoggerFactory loggerFactory)
		{
			_entityContext = pEntityContext;
			_loggingService = pLoggingService;
			_configService = pConfigService;
            mlogger = logger;
            mloggerFactory = loggerFactory;
        }

		#region Métodos públicos
		public bool IdentidadTienePermiso(ulong pPermiso, Guid pIdentidadID, Guid pIdentidadMyGNOSS, TipoDePermiso pTipoPermiso)
		{
			if (pTipoPermiso.Equals(TipoDePermiso.Ecosistema))
			{
				return false;
			}

			List<Rol> listaRolesUsuario = ObtenerRolesIdentidad(pIdentidadID, pIdentidadMyGNOSS);			
			ulong rolesUsuario = 0;
			foreach (Rol rol in listaRolesUsuario)
			{
				switch (pTipoPermiso)
				{
					case TipoDePermiso.Comunidad:
						rolesUsuario = rolesUsuario | rol.PermisosAdministracion;
						break;
					case TipoDePermiso.Contenidos:
						rolesUsuario = rolesUsuario | rol.PermisosContenidos;
						break;
					case TipoDePermiso.Recursos:
						rolesUsuario = rolesUsuario | rol.PermisosRecursos;
						break;
				}
			}

			return TienePermiso(rolesUsuario, pPermiso);
		}

		public bool IdentidadTienePermisoRecursoSemantico(Guid pIdentidadID, Guid pIdentidadMyGNOSS, TipoPermisoRecursosSemanticos pTipoPermiso, Guid pDocumentoID)
		{
			List<Rol> listaRolesIdentidad = ObtenerRolesIdentidad(pIdentidadID, pIdentidadMyGNOSS);
			if (listaRolesIdentidad.FirstOrDefault(x => x.RolID.Equals(ProyectoAD.RolAdministrador)) != null)
			{
				return true;
			}
			foreach (Rol rol in listaRolesIdentidad)
			{
				RolOntologiaPermiso permisosOntologia = _entityContext.RolOntologiaPermiso.Where(x => x.DocumentoID.Equals(pDocumentoID) && x.RolID.Equals(rol.RolID)).FirstOrDefault();
				if (permisosOntologia != null && TienePermiso(permisosOntologia.Permisos, (ulong)pTipoPermiso))
				{
					return true;
				}				
			}

			return false;
		}

		public bool UsuarioTienePermisoAdministracionEcosistema(ulong pPermiso, Guid pUsuarioID)
		{
			List<RolEcosistema> listaRoles = ObtenerRolesEcosistemaUsuario(pUsuarioID);
			ulong roles = 0;

			foreach (RolEcosistema rol in listaRoles)
			{
				roles = roles | rol.Permisos;
			}

			return TienePermiso(roles, pPermiso);
		}

		public List<RolEcosistema> ObtenerRolesEcosistemaUsuario(Guid pUsuarioID)
		{
			ProyectoCN proyectoCN = new ProyectoCN(_entityContext, _loggingService, _configService, null, mloggerFactory.CreateLogger<ProyectoCN>(), mloggerFactory);
			List<RolEcosistema> rolesUsuario = proyectoCN.ObtenerRolesAdministracionEcosistemaDeUsuario(pUsuarioID);

			proyectoCN.Dispose();

			return rolesUsuario;
		}

		public List<Rol> ObtenerRolesIdentidad(Guid pIdentidadID, Guid pIdentidadMyGNOSS)
		{
			ProyectoCN proyectoCN = new ProyectoCN(_entityContext, _loggingService, _configService, null, mloggerFactory.CreateLogger<ProyectoCN>(), mloggerFactory);
			IdentidadCN identidadCN = new IdentidadCN(_entityContext, _loggingService, _configService, null, mloggerFactory.CreateLogger<IdentidadCN>(), mloggerFactory);

			List<Rol> rolesIdentidad = identidadCN.ObtenerRolesDeIdentidad(pIdentidadID);
			Guid proyectoID = identidadCN.ObtenerProyectoDeIdentidad(pIdentidadID);
			if (!proyectoID.Equals(ProyectoAD.MetaProyecto))
			{
				Rol rolUsuario = proyectoCN.ObtenerRolUsuario(proyectoID);
				rolesIdentidad.Add(rolUsuario);
			}
			
			List<Guid> gruposIdentidad = identidadCN.ObtenerGruposIDParticipaPerfil(pIdentidadID, pIdentidadMyGNOSS).Keys.ToList();
			List<Rol> rolesTotalesUsuario = proyectoCN.ObtenerRolesDeGrupos(gruposIdentidad).Union(rolesIdentidad).ToList();
			

			proyectoCN.Dispose();
			identidadCN.Dispose();

			return rolesTotalesUsuario;
		}

		public List<PermisoComunidad> ObtenerListaPermisosAdministracionIdentidad(Guid pIdentidad, Guid pIdentidadMyGNOSS)
		{
			List<PermisoComunidad> listaPermisos = new List<PermisoComunidad>();
			foreach (PermisoComunidad permiso in Enum.GetValues(typeof(PermisoComunidad)))
			{
				if (IdentidadTienePermiso((ulong)permiso, pIdentidad, pIdentidadMyGNOSS, TipoDePermiso.Comunidad))
				{
					listaPermisos.Add(permiso);
				}
			}
			return listaPermisos;
		}

		public List<PermisoContenidos> ObtenerListaPermisosContenidosIdentidad(Guid pIdentidad, Guid pIdentidadMyGNOSS)
		{
			List<PermisoContenidos> listaPermisos = new List<PermisoContenidos>();
			foreach (PermisoContenidos permiso in Enum.GetValues(typeof(PermisoContenidos)))
			{
				if (IdentidadTienePermiso((ulong)permiso, pIdentidad, pIdentidadMyGNOSS, TipoDePermiso.Contenidos))
				{
					listaPermisos.Add(permiso);
				}
			}
			return listaPermisos;
		}

		public List<PermisoEcosistema> ObtenerListaPermisosAdministracionEcosistema(Guid pUsuarioID)
		{
			List<PermisoEcosistema> listaPermisos = new List<PermisoEcosistema>();
			foreach (PermisoEcosistema permiso in Enum.GetValues(typeof(PermisoEcosistema)))
			{
				if (UsuarioTienePermisoAdministracionEcosistema((ulong)permiso, pUsuarioID))
				{
					listaPermisos.Add(permiso);
				}
			}
			return listaPermisos;
		}

		public bool ComprobarSiTieneAlgunPermisoAdministracion(Guid pIdentidad, Guid pIdentidadMyGNOSS)
		{
			foreach (PermisoComunidad permiso in Enum.GetValues(typeof(PermisoComunidad)))
			{
				if (IdentidadTienePermiso((ulong)permiso, pIdentidad, pIdentidadMyGNOSS, TipoDePermiso.Comunidad))
				{
					return true;
				}
			}

			foreach (PermisoContenidos permiso in Enum.GetValues(typeof(PermisoContenidos)))
			{
				if (IdentidadTienePermiso((ulong)permiso, pIdentidad, pIdentidadMyGNOSS, TipoDePermiso.Contenidos))
				{
					return true;
				}
			}

			return false;
		}

		public bool ComprobarSiTieneAlgunPermisoAdministracionEcosistema(Guid pUsuarioID)
		{
			foreach (PermisoEcosistema permiso in Enum.GetValues(typeof(PermisoEcosistema)))
			{
				if (UsuarioTienePermisoAdministracionEcosistema((ulong)permiso, pUsuarioID))
				{
					return true;
				}
			}

			return false;
		}

		public bool ComprobarPermisoSobreRecurso(TiposDocumentacion pTipo, ulong pPermiso, Guid pIdentidad)
		{
			bool tienePermiso = false;
			switch (pTipo)
			{
				case TiposDocumentacion.Hipervinculo:
					tienePermiso = IdentidadTienePermiso(pPermiso, pIdentidad, pIdentidad, TipoDePermiso.Recursos);					
				break;
				case TiposDocumentacion.ReferenciaADoc:
					break;
				case TiposDocumentacion.Video:
					break;
				case TiposDocumentacion.FicheroServidor:
					break;
				case TiposDocumentacion.Encuesta:
					break;
				case TiposDocumentacion.Debate:
					break;
				case TiposDocumentacion.Pregunta:
					break;
				case TiposDocumentacion.Semantico:
					break;
			}

			return tienePermiso;
		}

		#endregion

		#region Métodos estáticos

		public static bool TienePermiso(ulong pRoles, ulong pPermiso)
		{
			ulong auxPermiso = pRoles & pPermiso;
			bool tienePermiso = auxPermiso == pPermiso;

			return tienePermiso;
		}

		public static ulong ObtenerPermisosUsuarioPorDefecto()
		{
			ulong permisos = (ulong)PermisoRecursos.CrearDebate + (ulong)PermisoRecursos.CrearPregunta + (ulong)PermisoRecursos.CrearNota + (ulong)PermisoRecursos.CrearRecursoTipoAdjunto + (ulong)PermisoRecursos.CrearRecursoTipoEnlace + (ulong)PermisoRecursos.CrearRecursoTipoReferenciaADocumentoFisico + (ulong)PermisoRecursos.CrearEncuesta;
			return permisos;
		}

		#endregion
	}
}
