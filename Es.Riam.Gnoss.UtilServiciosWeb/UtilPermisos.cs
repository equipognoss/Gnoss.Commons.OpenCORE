using Es.Riam.AbstractsOpen;
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
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.UtilServiciosWeb
{
    public class DiccionarioDePermisos
    {
        public bool CrearRecursoSemantico { get; set; }
        public bool EditarRecursoSemantico { get; set; }
        public bool EliminarRecursoSemantico { get; set; }
        public bool RestaurarVersionRecursoSemantico { get; set; }
        public bool EliminarVersionRecursoSemantico { get; set; }
    }
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


        /// <summary>
        /// Carga la lista de permisos de un rol de ecosistema
        /// </summary>
        /// <param name="pRolID">ID del rol</param>
        /// <param name="pEntityContext">Contexto de entidad</param>
        /// <returns>Lista de permisos con su estado</returns>
        public static List<PermisoModelRolController> CargarListaDePermisosDeRolEcosistema(Guid pRolID, EntityContext pEntityContext)
        {
            List<PermisoModelRolController> listaPermisos = new List<PermisoModelRolController>();

            RolEcosistema rol = pEntityContext.RolEcosistema.FirstOrDefault(x => x.RolID.Equals(pRolID));
            if (rol != null || pRolID.Equals(Guid.Empty))
            {
                foreach (PermisoEcosistema permisoEcosistema in Enum.GetValues(typeof(PermisoEcosistema)))
                {
                    PermisoModelRolController permiso = new PermisoModelRolController();
                    permiso.Nombre = permisoEcosistema.ToString();

                    if (!pRolID.Equals(Guid.Empty) && rol != null)
                    {
                        permiso.Concedido = TienePermiso(rol.Permisos, (ulong)permisoEcosistema);
                    }

                    permiso.Seccion = UtilCadenas.ObtenerAtributoEnum<SectionAttribute>(permisoEcosistema).Nombre;

                    listaPermisos.Add(permiso);
                }
            }

            return listaPermisos;
        }

        /// <summary>
        /// Carga la lista de permisos de un rol de comunidad
        /// </summary>
        /// <param name="pRolID">ID del rol</param>
        /// <param name="pEntityContext">Contexto de entidad</param>
        /// <returns>Lista de permisos con su estado</returns>
        public static List<PermisoModelRolController> CargarListaDePermisosDeRolComunidad(Guid pRolID, EntityContext pEntityContext)
        {
            List<PermisoModelRolController> listaPermisos = new List<PermisoModelRolController>();

            Rol rol = pEntityContext.Rol.FirstOrDefault(x => x.RolID.Equals(pRolID));
            if (rol != null || pRolID.Equals(Guid.Empty))
            {
                // Permisos de Administración
                foreach (PermisoComunidad permisoComunidad in Enum.GetValues(typeof(PermisoComunidad)))
                {
                    PermisoModelRolController permiso = new PermisoModelRolController();
                    permiso.Nombre = permisoComunidad.ToString();

                    if (!pRolID.Equals(Guid.Empty) && rol != null)
                    {
                        permiso.Concedido = TienePermiso(rol.PermisosAdministracion, (ulong)permisoComunidad);
                    }

                    permiso.Seccion = UtilCadenas.ObtenerAtributoEnum<SectionAttribute>(permisoComunidad).Nombre;

                    listaPermisos.Add(permiso);
                }

                // Permisos de Contenidos
                foreach (PermisoContenidos permisoContenidos in Enum.GetValues(typeof(PermisoContenidos)))
                {
                    PermisoModelRolController permiso = new PermisoModelRolController();
                    permiso.Nombre = permisoContenidos.ToString();

                    if (!pRolID.Equals(Guid.Empty) && rol != null)
                    {
                        permiso.Concedido = TienePermiso(rol.PermisosContenidos, (ulong)permisoContenidos);
                    }

                    permiso.Seccion = UtilCadenas.ObtenerAtributoEnum<SectionAttribute>(permisoContenidos).Nombre;

                    listaPermisos.Add(permiso);
                }

                // Permisos de Recursos
                foreach (PermisoRecursos permisoRecurso in Enum.GetValues(typeof(PermisoRecursos)))
                {
                    PermisoModelRolController permiso = new PermisoModelRolController();
                    permiso.Nombre = permisoRecurso.ToString();

                    if (!pRolID.Equals(Guid.Empty) && rol != null)
                    {
                        permiso.Concedido = TienePermiso(rol.PermisosRecursos, (ulong)permisoRecurso);
                    }

                    permiso.Seccion = UtilCadenas.ObtenerAtributoEnum<SectionAttribute>(permisoRecurso).Nombre;

                    listaPermisos.Add(permiso);
                }
            }

            return listaPermisos;
        }

        public static ulong ObtenerSumaRolesDeBinario(string pCadenaBinaria, TipoDePermiso pTipoPermiso)
        {
            ulong resultado = 0;
            ulong[] valoresPermisos = null;
            switch (pTipoPermiso)
            {
                case TipoDePermiso.Comunidad:
                    valoresPermisos = (ulong[])Enum.GetValues(typeof(PermisoComunidad));
                    break;
                case TipoDePermiso.Contenidos:
                    valoresPermisos = (ulong[])Enum.GetValues(typeof(PermisoContenidos));
                    break;
                case TipoDePermiso.Recursos:
                    valoresPermisos = (ulong[])Enum.GetValues(typeof(PermisoRecursos));
                    break;
                case TipoDePermiso.Ecosistema:
                    valoresPermisos = (ulong[])Enum.GetValues(typeof(PermisoEcosistema));
                    break;
            }

            for (int i = 0; i < pCadenaBinaria.Length; i++)
            {
                if (pCadenaBinaria[i].Equals('1'))
                {
                    resultado += valoresPermisos[i];
                }
            }

            return resultado;
        }

		/// <summary>
		/// Procesa los permisos de recursos semánticos para un rol específico
		/// </summary>
		/// <param name="pPermisosRecursosSemanticos">Diccionario con los permisos por ontología</param>
		/// <param name="pRolID">ID del rol</param>
		/// <param name="pAmbito">Ámbito del rol</param>
		/// <exception cref="GnossException">Se lanza si el ámbito es Ecosistema y hay permisos asignados</exception>
		public static ulong ProcesarPermisosRecursosSemanticos(
			Dictionary<Guid, DiccionarioDePermisos> pPermisosRecursosSemanticos,
			Guid pRolID,
			short pAmbito, EntityContext pEntityContext)
		{
			ulong permisos = 0;
			// Validación temprana
			if (pPermisosRecursosSemanticos == null || pPermisosRecursosSemanticos.Count == 0)
			{
				return permisos;
			}

			foreach (var kvp in pPermisosRecursosSemanticos)
			{
				Guid ontologiaID = kvp.Key;
				DiccionarioDePermisos dicPermisos = kvp.Value;

				// Calcular permisos agregados
				permisos = CalcularPermisos(dicPermisos);

				// Guardar o actualizar permisos
				ActualizarPermisoRolOntologia(ontologiaID, pRolID, permisos,pEntityContext);
				
			}
			return permisos;
		}

        /// <summary>
        /// Calcula el valor agregado de permisos basándose en los flags del diccionario
        /// </summary>
        public static ulong CalcularPermisos(DiccionarioDePermisos dicPermisos)
		{
			ulong permisos = 0;

			if (dicPermisos.CrearRecursoSemantico)
			{
				permisos += (ulong)TipoPermisoRecursosSemanticos.Crear;
			}
			if (dicPermisos.EditarRecursoSemantico)
			{
				permisos += (ulong)TipoPermisoRecursosSemanticos.Modificar;
			}
			if (dicPermisos.EliminarRecursoSemantico)
			{
				permisos += (ulong)TipoPermisoRecursosSemanticos.Eliminar;
			}
			if (dicPermisos.RestaurarVersionRecursoSemantico)
			{
				permisos += (ulong)TipoPermisoRecursosSemanticos.RestaurarVersion;
			}
			if (dicPermisos.EliminarVersionRecursoSemantico)
			{
				permisos += (ulong)TipoPermisoRecursosSemanticos.EliminarVersion;
			}

			return permisos;
		}

        /// <summary>
        /// Valida que un rol con ámbito Ecosistema no tenga permisos de recursos semánticos
        /// </summary>
        public static bool EcosistemaConPermisosSemanticos(short pAmbito, ulong permisos)
		{
			bool result = false;

			if (pAmbito.Equals((short)AmbitoRol.Ecosistema) && permisos > 0)
			{
				result = true;
			}
			return result;
		}

        /// <summary>
        /// Actualiza o crea el registro de permisos para una ontología y rol
        /// </summary>
        public static void ActualizarPermisoRolOntologia(Guid ontologiaID, Guid pRolID, ulong permisos, EntityContext pEntityContext)
		{
			var permisoRolOntologia = pEntityContext.RolOntologiaPermiso
				.FirstOrDefault(x => x.DocumentoID.Equals(ontologiaID) && x.RolID.Equals(pRolID));

			if (permisoRolOntologia != null)
			{
				permisoRolOntologia.Permisos = permisos;
			}
			else
			{
				permisoRolOntologia = new RolOntologiaPermiso
				{
					DocumentoID = ontologiaID,
					RolID = pRolID,
					Permisos = permisos
				};
				pEntityContext.RolOntologiaPermiso.Add(permisoRolOntologia);
			}
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
