using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.CMS;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.IdentidadDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.Roles;
using Es.Riam.Gnoss.AD.Identidad;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Usuarios;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Es.Riam.Gnoss.Logica.Identidad
{
    /// <summary>
    /// Lógica de identidades
    /// </summary>
    public class IdentidadCN : BaseCN, IDisposable
    {

        private LoggingService mLoggingService;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        #region Constructor

        /// <summary>
        /// Constructor sin parámetros
        /// </summary>
        public IdentidadCN(EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<IdentidadCN> logger, ILoggerFactory loggerFactory)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        {
            mLoggingService = loggingService;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            if(loggerFactory == null)
            {
                IdentidadAD = new IdentidadAD(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication, null, null);
            }
            else
            {
                IdentidadAD = new IdentidadAD(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadAD>(), mLoggerFactory);
            }     
        }

        #endregion

        #region Métodos generales

        /// <summary>
        /// Obtiene la fecha de alta de una identidad
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la identidad</param>
        /// <returns>DateTime con la fecha de alta de una identidad. Null en caso de que no exista la identidad</returns>
        public DateTime? ObtenerFechaAltaPorIdentidadID(Guid pIdentidadID)
        {
            return IdentidadAD.ObtenerFechaAltaPorIdentidadID(pIdentidadID);
        }

        public Guid? ObtenerPerfilIDPorIDTesauro(Guid pTesauroID)
        {
            return IdentidadAD.ObtenerPerfilIDPorIDTesauro(pTesauroID);
        }

        /// <summary>
        /// Obtiene identidadID de una persona en un proyecto mediante su nombre corto
        /// </summary>
        /// <param name="pNombre">Nombre corto de la persona</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Guid con la identidadID o Guid.Empty si no existe</returns>
        public Guid ObtenerIdentidadIDPorNombreCorto(string pNombre, Guid pProyectoID)
        {
            return IdentidadAD.ObtenerIdentidadIDPorNombreCorto(pNombre, pProyectoID);
        }

        /// <summary>
        /// Obtiene perfilID de una persona en un proyecto mediante su nombre corto
        /// </summary>
        /// <param name="pNombre">Nombre corto de la persona</param>
        /// <returns>Guid con la identidadID o Guid.Empty si no existe</returns>
        public Guid ObtenerPerfilIDPorNombreCorto(string pNombre)
        {
            return IdentidadAD.ObtenerPerfilIDPorNombreCorto(pNombre);
        }

        /// <summary>
        /// Actualiza el número de conexiones a un proyecto
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la identidad</param>
        public void ActualizarNumeroConexionesProyecto(Guid pIdentidadID)
        {
            try
            {
                if (Transaccion != null)
                {
                    IdentidadAD.ActualizarNumeroConexionesProyecto(pIdentidadID);
                }
                else
                {
                    IdentidadAD.IniciarTransaccionEntityContext();
                    {
                        IdentidadAD.ActualizarNumeroConexionesProyecto(pIdentidadID);
                        TerminarTransaccion(true);
                    }
                }
            }
            catch (DBConcurrencyException ex)
            {
                TerminarTransaccion(false);
                // Error de concurrencia
                mLoggingService.GuardarLogError(ex,mlogger);
                throw new ErrorConcurrencia();
            }
            catch (DataException ex)
            {
                TerminarTransaccion(false);
                //Error interno de la aplicación				
                mLoggingService.GuardarLogError(ex, mlogger);
                throw new ErrorInterno();
            }
            catch
            {
                TerminarTransaccion(false);
                throw;
            }
        }


        /// <summary>
        /// Devuelve las organizaciones que han contribuido en un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <returns>Lista con los identificadores de las organizaciones</returns>
        public List<Guid> ObtenerOrganizacionesDeContribuidoresEnProyecto(Guid pProyectoID)
        {
            return IdentidadAD.ObtenerOrganizacionesDeContribuidoresEnProyecto(pProyectoID);
        }

        /// <summary>
        /// Devuelve los perfiles que han contribuido en un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <returns>Lista con los identificadores de las identidades</returns>
        public List<Guid> ObtenerPerfilesDeContribuidoresEnProyecto(Guid pProyectoID)
        {
            return IdentidadAD.ObtenerPerfilesDeContribuidoresEnProyecto(pProyectoID);
        }

        public string[] ObtenerNombresIdentidadesAmigosPorPrefijo(Guid pIdentidadID, string pPrefijo, int pNumeroResultados, bool pAmigosConPermiso, Guid pIdentidadOrganizacion, List<string> pListaAnteriores)
        {
            return IdentidadAD.ObtenerNombresIdentidadesAmigosPorPrefijo(pIdentidadID, pPrefijo, pNumeroResultados, pAmigosConPermiso, pIdentidadOrganizacion, pListaAnteriores);
        }
        /// <summary>
        /// Obtiene el nombre al que pertenece ese identificador de perfil
        /// </summary>
        /// <param name="pPerfilID">Identificador del perfil</param>
        /// <returns>Nombre de un perfil</returns>
        public string ObtenerNombredePerfilID(Guid pPerfilid)
        {
            return IdentidadAD.ObtenerNombredePerfilID(pPerfilid);
        }

        /// <summary>
        /// Obtiene el perfil de un usuario en un proyecto
        /// </summary>
        ///<param name="pUsuarioID">UsuarioID</param>
        ///<param name="pProyectoID">ProyectoID</param>
        ///<returns>perfilID</returns>
        public Guid ObtenerPerfilUsuarioEnProyecto(Guid pUsuarioID, Guid pProyectoID)
        {
            return IdentidadAD.ObtenerPerfilUsuarioEnProyecto(pUsuarioID, pProyectoID);

        }

        /// <summary>
        /// Obtiene el proyecto en el que participa la identidad
        /// </summary>
        ///<param name="pUsuarioID">UsuarioID</param>
        ///<param name="pProyectoID">ProyectoID</param>
        ///<returns>identidadID</returns>
        public Guid ObtenerIdentidadUsuarioEnProyecto(Guid pUsuarioID, Guid pProyectoID)
        {
            return IdentidadAD.ObtenerIdentidadUsuarioEnProyecto(pUsuarioID, pProyectoID);
        }

        public List<Guid> ObtenerComunidadesPrivadas(Guid pIdentidadID)
        {
            return IdentidadAD.ObtenerComunidadesPrivadas(pIdentidadID);
        }

        /// <summary>
        /// Obtiene el proyecto al que pertenece la identidad
        /// </summary>
        ///<param name="pIdentidadID">IdentidadID</param>
        ///<returns>ProyectoID</returns>
        public Guid ObtenerProyectoDeIdentidad(Guid pIdentidadID)
        {
            return IdentidadAD.ObtenerProyectoDeIdentidad(pIdentidadID);
        }

        ///// <summary>
        ///// Obtiene los primeros nombres visibles de una identidad
        ///// </summary>
        ///// <param name="pIdentidadID">Identificador de la identidad</param>
        ///// <param name="pPrefijo">Prefijo por el que tienen que comenzar los nombres</param>
        ///// <param name="pNumeroResultados">Número de resultados a obtener</param>
        ///// <param name="pIdentidadOrganizacion">Identidad de organizacion</param>
        //public string[] ObtenerNombresIdentidadesPorPrefijo(Guid pIdentidadID, string pPrefijo, int pNumeroResultados, Guid pIdentidadOrganizacion)
        //{
        //    return IdentidadAD.ObtenerNombresIdentidadesPorPrefijo(pIdentidadID, pPrefijo, pNumeroResultados, pIdentidadOrganizacion);
        //}


        /// <summary>
        /// Obtiene los primeros nombres visibles de una identidad en las comunidades privadas que participa
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la identidad</param>
        /// <param name="pPrefijo">Prefijo por el que tienen que comenzar los nombres</param>
        /// <param name="pNumeroResultados">Número de resultados a obtener</param>
        public string[] ObtenerNombresIdentidadesPorPrefijoEnMisProyectosPrivados(Guid pIdentidadID, string pPrefijo, int pNumeroResultados, List<string> pListaAnteriores)
        {
            return IdentidadAD.ObtenerNombresIdentidadesPorPrefijoEnMisProyectosPrivados(pIdentidadID, pPrefijo, pNumeroResultados, pListaAnteriores);
        }

        /// <summary>
        /// Obtiene los IDs de las identidades en las comunidades privadas que participa
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la identidad</param>
        /// <returns>Lista con los IDs de las identiades</returns>
        public List<Guid> ObtenerIdentidadesIDEnMisProyectosPrivados(Guid pIdentidadID)
        {
            return IdentidadAD.ObtenerIdentidadesIDEnMisProyectosPrivados(pIdentidadID);
        }

        /// <summary>
        /// Obtiene las identidades de los miembros de una organizacion con permisos para ver al contacto
        /// </summary>
        /// <param name="pIdentidadOrganizacion">Identidad en MyGNOSS de la organizacion</param>
        /// <param name="pIdentidad">Identidad en MyGNOSS del usuario</param>
        /// <returns>IdentidadDS</returns>
        public DataWrapperIdentidad ObtenerMiembrosOrganizacionConPermisoDeContactoAIdentidad(Guid pIdentidadOrganizacion, Guid pIdentidad)
        {
            return IdentidadAD.ObtenerMiembrosOrganizacionConPermisoDeContactoAIdentidad(pIdentidadOrganizacion, pIdentidad);
        }

        /// <summary>
        /// Verdad si la identidad es de tipo profesor, flaso en caso contrario
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la identidad</param>
        /// <param name="pNombreCortoOrg">Nombre de la clase a la que intenta acceder</param>
        /// <returns></returns>
        public bool ComprobarIdentidadDeProfesor(Guid pIdentidadID, string pNombreCortoOrg)
        {
            return IdentidadAD.ComprobarIdentidadDeProfesor(pIdentidadID, pNombreCortoOrg);
        }

        /// <summary>
        /// Devuelve una lista con las identidades ID de las clases del profesor.
        /// </summary>
        /// <param name="guid">IdentidadID del profesor</param>
        /// <returns>Lista de ID's de las clases del profesor</returns>
        public List<Guid> ObtenerClasesProfesor(Guid pIdentidadID)
        {
            return IdentidadAD.ObtenerClasesProfesor(pIdentidadID);
        }


        /// <summary>
        /// Obtiene una lista con todos los perfiles de una organización (empleados y organización).
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <returns>Lista con todos los perfiles de una organización (empleados y organización)</returns>
        public List<Guid> ObtenerListaPerfilesDeOrganizacion(Guid pOrganizacionID)
        {
            return IdentidadAD.ObtenerListaPerfilesDeOrganizacion(pOrganizacionID);
        }

        /// <summary>
        /// Obtiene el identificador del perfil de la organización
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <returns></returns>
        public Guid? ObtenerPerfilIDDeOrganizacion(Guid pOrganizacionID)
        {
            return IdentidadAD.ObtenerPerfilIDDeOrganizacion(pOrganizacionID);
        }

        /// <summary>
        /// Obtiene el perfil de la organización pasada por parámetro (Identidad, Perfil y PerfilOrganizacion)
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerPerfilDeOrganizacion(Guid pOrganizacionID)
        {
            return IdentidadAD.ObtenerPerfilDeOrganizacion(pOrganizacionID);
        }

        /// <summary>
        /// Obtiene una lista de identificadores de perfiles de todos los usuarios que participan en un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto del que se quieren conocer los perfiles</param>
        /// <returns>Lista de identificadores de perfiles</returns>
        public List<Guid> ObtenerPerfilesIDDeProyecto(Guid pProyectoID)
        {
            return IdentidadAD.ObtenerPerfilesIDDeProyecto(pProyectoID);
        }

        /// <summary>
        /// Obtiene el los miembros de una comunidad
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerMiembrosComunidad(Guid pProyectoID)
        {
            return IdentidadAD.ObtenerMiembrosComunidad(pProyectoID);
        }

        /// <summary>
        /// Obtiene los perfiles de una comunidad a partir del nombre para el autocompletar
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto</param>
        /// <param name="pIdentidadID">Identidad del usuario que hace la petición(para que no aparezca el mismo)</param>
        /// <param name="pBusqueda">Texto a buscar</param>
        /// <param name="pNumero">Número de resultados</param>
        /// <returns>Diccionario con:
        /// Clave=PerfilID
        /// Item1=NombrePerfil
        /// Item2=NombreOrg
        /// Item3=PersonaID
        /// Item4=OrganizacionID
        /// </returns>
        public Dictionary<Guid, Tuple<string, string, Guid?, Guid?>> ObtenerPerfilesParaAutocompletar(Guid pProyectoID, Guid pIdentidadID, string pBusqueda, int pNumero, bool pOmitirMiPerfil)
        {
            return IdentidadAD.ObtenerPerfilesParaAutocompletar(pProyectoID, pIdentidadID, pBusqueda, pNumero, pOmitirMiPerfil);
        }

        /// <summary>
        /// Obtiene los perfiles de una comunidad a partir del nombre para el autocompletar
        /// </summary>
        /// <param name="pListaIdentidadesID">Lista de identidades de las que queremos obtener el perfil</param>
        /// <param name="pProyectoID">Clave del proyecto</param>
        /// <param name="pIdentidadID">Identidad del usuario que hace la petición(para que no aparezca el mismo)</param>
        /// <param name="pBusqueda">Texto a buscar</param>
        /// <param name="pNumero">Número de resultados</param>
        /// <returns>Diccionario con:
        /// Clave=PerfilID
        /// Item1=NombrePerfil
        /// Item2=NombreOrg
        /// Item3=PersonaID
        /// Item4=OrganizacionID
        /// </returns>
        public Dictionary<Guid, Tuple<string, string, Guid?, Guid?>> ObtenerPerfilesParaAutocompletarDeIdentidadesID(List<Guid> pListaIdentidadesID, Guid pProyectoID, Guid pIdentidadID, string pBusqueda, int pNumero, bool pOmitirMiPerfil)
        {
            return IdentidadAD.ObtenerPerfilesParaAutocompletarDeIdentidadesID(pListaIdentidadesID, pProyectoID, pIdentidadID, pBusqueda, pNumero, pOmitirMiPerfil);
        }


        /// <summary>
        /// Obtiene el los nombres de los grupos de una comunidad
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto</param>
        /// <returns>Diccionario con GrupoID-NombreGrupo/returns>
        public Dictionary<Guid, string> ObtenerNombresTodosGruposProyecto(Guid pProyectoID)
        {
            return IdentidadAD.ObtenerNombresTodosGruposProyecto(pProyectoID);
        }

        /// <summary>
        /// Obtiene el los miembros de una comunidad a partir del nombre para el autocompletar
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto</param>
        /// <param name="pIdentidadID">Identidad del usuario que hace la petición(para que aparezcan los públicos o aquellos a los que pertenezca la identidad)</param>
        /// <param name="pBusqueda">Texto a buscar</param>
        /// <param name="pNumero">Número de resultados</param>
        /// <param name="pEsSupervisor">Indica si el usuario que hace la petición tiene permiso de supervisión para mostrarle todos los grupos de la comunidad</param>
        /// <returns>Diccionario con GrupoID-NombreGrupo/returns>
        public Dictionary<Guid, string> ObtenerGruposParaAutocompletar(Guid pProyectoID, Guid pIdentidadID, string pBusqueda, int pNumero, bool pEsSupervisor)
        {
            return IdentidadAD.ObtenerGruposParaAutocompletar(pProyectoID, pIdentidadID, pBusqueda, pNumero, pEsSupervisor);
        }

        /// <summary>
        /// Obtiene el los miembros de una organizacion
        /// </summary>
        /// <param name="pOrganizacionID">Clave de la organizacion</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerMiembrosOrganizacionParaFiltroGrupos(Guid pOrganizacionID)
        {
            return IdentidadAD.ObtenerMiembrosOrganizacionParaFiltroGrupos(pOrganizacionID);
        }

        public Perfil ObtenerMiembrosComunidadFiltro(Guid pProyectoID, string pFiltro, Guid pIdentidadOmisionID)
        {
            return IdentidadAD.ObtenerMiembrosComunidadFiltro(pProyectoID, pFiltro, pIdentidadOmisionID);
        }

        public GrupoIdentidades ObtenerGruposComunidadFiltro(Guid pProyectoID, string pFiltro, Guid pIdentidadParticipanteID)
        {
            return IdentidadAD.ObtenerGruposComunidadFiltro(pProyectoID, pFiltro, pIdentidadParticipanteID);
        }

        /// <summary>
        /// Obtiene los mismbros de gnoss visibles.
        /// </summary>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerMiembrosGnossVisibles()
        {
            return IdentidadAD.ObtenerMiembrosGnossVisibles();
        }

        /// <summary>
        /// Obtiene los grupos a los que la identidad tiene permisos para hacer envios
        /// </summary>
        /// <param name="pIdentidadID">Clave de la identidad</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerGruposEnvios(Guid pIdentidadID)
        {
            return IdentidadAD.ObtenerGruposEnvios(pIdentidadID);
        }

        /// <summary>
        /// Obtiene los perfiles de las organizaciónes pasada por parámetro 
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <returns>Dataset de identidades</returns>
        public List<Perfil> ObtenerPerfilesDeOrganizaciones(List<Guid> pOrganizacionesIDs)
        {
            return IdentidadAD.ObtenerPerfilesDeOrganizaciones(pOrganizacionesIDs);
        }
        /// <summary>
        /// Obtiene los perfiles de la organización pasada por parámetro 
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <returns>Dataset de identidades</returns>
        public List<Perfil> ObtenerPerfilesDeUnaOrganizacion(Guid organizacionID)
        {
            return IdentidadAD.ObtenerPerfilesDeUnaOrganizacion(organizacionID);
        }

        /// <summary>
        /// Obtiene los perfiles de una persona pasada por parámetro
        /// </summary>
        /// <param name="pPersonaID">Identificador de persona</param>
        /// <param name="pObtenerSoloActivos">TRUE si obtiene sólo lo que este activo (no eliminado, no fecha de baja,..)</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerPerfilesDePersona(Guid pPersonaID, bool pObtenerSoloActivos)
        {
            if (pPersonaID.Equals(UsuarioAD.Invitado))
            {
                return ObtenerIdentidadInvitado();
            }
            else
            {
                return IdentidadAD.ObtenerPerfilesDePersona(pPersonaID, pObtenerSoloActivos);
            }
        }

        /// <summary>
        /// Obtiene los perfiles de una persona pasada por parámetro
        /// </summary>
        /// <param name="pPersonaID">Identificador de persona</param>
        /// <param name="pObtenerSoloActivos">TRUE si obtiene sólo lo que este activo (no eliminado, no fecha de baja,..)</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerPerfilesDePersona(Guid pPersonaID, bool pObtenerSoloActivos, Guid pIdentidadID, bool usarCompartir = false)
        {
            if (pPersonaID.Equals(UsuarioAD.Invitado))
            {
                return ObtenerIdentidadInvitado();
            }
            else
            {
                if (usarCompartir)
                {
                    return IdentidadAD.ObtenerPerfilesDePersonaCompartir(pPersonaID, pObtenerSoloActivos, pIdentidadID);
                }
                else
                {
                    return IdentidadAD.ObtenerPerfilesDePersona(pPersonaID, pObtenerSoloActivos, pIdentidadID);
                }
            }
        }

        /// <summary>
        /// Obtiene las tablas Perfil e Identidad de personas en un proyecto determinado
        /// </summary>
        /// <param name="pIdentidadesID">Identificadores de las personas</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pIdentidadUsuarioID">Identificador de la identidad del usuario</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerPerfilIdentidadDeIdentidadesEnProyectoNoSuscritas(Guid pIdentidadUsuarioID, List<Guid> pIdentidadesID, Guid pProyectoID)
        {
            return IdentidadAD.ObtenerPerfilIdentidadDeIdentidadesEnProyectoNoSuscritas(pIdentidadUsuarioID, pIdentidadesID, pProyectoID);
        }

        /// <summary>
        /// Obtiene la urlPerfil de una identidad
        /// </summary>
        /// <param name="pIdentidad">Clave de la identidad</param>
        /// <returns>urlPerfil de una identidad</returns>
        public KeyValuePair<string, string> ObtenerURLPerfilPorIdentidad(Guid pIdentidad)
        {
            return IdentidadAD.ObtenerURLPerfilPorIdentidad(pIdentidad);
        }


        /// <summary>
        /// Obtiene los IDs de identidades suscritas a un perfil
        /// </summary>
        /// <returns>Dataset de identidades</returns>
        public List<Guid> ObtenerListaIdentidadesSusucritasAPerfil(Guid pPerfilID)
        {
            return IdentidadAD.ObtenerListaIdentidadesSusucritasAPerfil(pPerfilID);
        }

        /// <summary>
        /// Obtiene los datos de identidades suscritas a un perfil
        /// </summary>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadesSusucritasAPerfil(Guid pPerfilID)
        {
            return IdentidadAD.ObtenerIdentidadesSusucritasAPerfil(pPerfilID);
        }

        /// <summary>
        /// Obtiene los datos de identidades suscritas a un perfil
        /// </summary>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadesSusucritasAPerfil(Guid pPerfilID, bool pCargarIdentidadesPrivadas)
        {
            return IdentidadAD.ObtenerIdentidadesSusucritasAPerfil(pPerfilID, pCargarIdentidadesPrivadas);
        }

        /// <summary>
        /// Obtiene "Identidad", "Perfil", "PerfilPersonaOrg" y "PerfilPersona"
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerMiembrosDeProyectoParaMosaico(Guid pProyectoID, int pNumeroMiembros, bool pOrdenarPorFechaAlta)
        {
            return IdentidadAD.ObtenerMiembrosDeProyectoParaMosaico(pProyectoID, pNumeroMiembros, pOrdenarPorFechaAlta);
        }

        /// <summary>
        /// Obtiene los IDs de identidades suscritas a un perfil en un determinado proyecto
        /// </summary>
        /// <returns>Dataset de identidades</returns>
        public List<Guid> ObtenerListaIdentidadesIDSusucritasAPerfilEnProyecto(Guid pPerfilID, Guid pProyectoID)
        {
            return IdentidadAD.ObtenerListaIdentidadesIDSusucritasAPerfilEnProyecto(pPerfilID, pProyectoID);
        }

        /// <summary>
        /// Obtiene los datos de identidades suscritas a un perfil en un determinado proyecto
        /// </summary>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadesSusucritasAPerfilEnProyecto(Guid pPerfilID, Guid pProyectoID)
        {
            return IdentidadAD.ObtenerIdentidadesSusucritasAPerfilEnProyecto(pPerfilID, pProyectoID);
        }

        /// <summary>
        /// Obtiene los datos de identidades suscritas a un perfil en un determinado proyecto
        /// </summary>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadesSusucritasAPerfilEnProyecto(Guid pPerfilID, Guid pProyectoID, bool pCargarIdentidadesPrivadas)
        {
            return IdentidadAD.ObtenerIdentidadesSusucritasAPerfilEnProyecto(pPerfilID, pProyectoID, pCargarIdentidadesPrivadas);
        }

        /// <summary>
        /// Obtiene los datos de identidades a las que esta suscrito un perfil
        /// </summary>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadesSusucritasPorPerfil(Guid pPerfilID)
        {
            return IdentidadAD.ObtenerIdentidadesSusucritasPorPerfil(pPerfilID);
        }

        /// <summary>
        /// Obtiene los ids de identidades a las que esta suscrito un perfil
        /// </summary>
        /// <returns>Dataset de identidades</returns>
        public List<Guid> ObtenerListaIdentidadesSusucritasPorPerfil(Guid pPerfilID)
        {
            return IdentidadAD.ObtenerListaIdentidadesSusucritasPorPerfil(pPerfilID);
        }

        /// <summary>
        /// Obtiene los datos de identidades a las que esta suscrito un perfil
        /// </summary>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadesSusucritasPorPerfil(Guid pPerfilID, bool pCargarIdentidadesPrivadas)
        {
            return IdentidadAD.ObtenerIdentidadesSusucritasPorPerfil(pPerfilID, pCargarIdentidadesPrivadas);
        }

        /// <summary>
        /// Obtiene los datos de identidades a las que esta suscrito un perfil en un determinado proyecto
        /// </summary>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadesSusucritasPorPerfilEnProyecto(Guid pPerfilID, Guid pProyectoID)
        {
            return IdentidadAD.ObtenerIdentidadesSusucritasPorPerfilEnProyecto(pPerfilID, pProyectoID);
        }

        /// <summary>
        /// Obtiene los datos de identidades a las que esta suscrito un perfil en un determinado proyecto
        /// </summary>
        /// <returns>Dataset de identidades</returns>
        public List<Guid> ObtenerListaIdentidadesSusucritasPorPerfilEnProyecto(Guid pPerfilID, Guid pProyectoID)
        {
            return IdentidadAD.ObtenerListaIdentidadesSusucritasPorPerfilEnProyecto(pPerfilID, pProyectoID);
        }

        /// <summary>
        /// Obtiene los datos de identidades a las que esta suscrito un perfil en un determinado proyecto
        /// </summary>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadesSusucritasPorPerfilEnProyecto(Guid pPerfilID, Guid pProyectoID, bool pCargarIdentidadesPrivadas)
        {
            return IdentidadAD.ObtenerIdentidadesSusucritasPorPerfilEnProyecto(pPerfilID, pProyectoID, pCargarIdentidadesPrivadas);
        }

        /// <summary>
        /// Obtiene las Identidades de los Administradores de la organizacion, (perfil de organizacion+persona tipo 1 , 2) en MYGNOSS.
        /// "Identidad" , "Perfil" y "PerfilPersonaOrg"
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organizacion</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadesConRolAdministradorDeOrganizacion(Guid pOrganizacionID)
        {
            return IdentidadAD.ObtenerIdentidadesConRolAdministradorDeOrganizacion(pOrganizacionID);
        }

        /// <summary>
        /// Obtiene una carga ligera de las identidades de la persona
        /// </summary>
        /// <param name="pPersonaID">Identificador de la persona</param>
        /// <returns>DataSet con las identidades de la persona</returns>
        public DataWrapperIdentidad ObtenerIdentidadesDePersonaMuyLigera(Guid pPersonaID)
        {
            return IdentidadAD.ObtenerIdentidadesDePersonaMuyLigera(pPersonaID);
        }

        /// <summary>
        /// Obtiene los datos de identidades de personas de MyGnoss
        /// </summary>
        ///<param name="pPersonaID">Identificador de la persona</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadesDePersonaDeMyGNOSS(Guid pPersonaID)
        {
            if (pPersonaID.Equals(UsuarioAD.Invitado))
            {
                return ObtenerIdentidadInvitado();
            }
            else
            {
                return IdentidadAD.ObtenerIdentidadesDePersonaDeMyGNOSS(pPersonaID);
            }
        }

        /// <summary>
        /// Obtiene los datos de identidad de una identidad en MyGnoss
        /// </summary>
        ///<param name="pIdentidadID">Identificador de identidad</param>
        /// <returns>Dataset de identidades</returns>
        public Guid ObtenerIdentidadIDDeMyGNOSSPorIdentidad(Guid pIdentidadID)
        {
            if (pIdentidadID.Equals(UsuarioAD.Invitado))
            {
                return pIdentidadID;
            }
            else
            {
                return IdentidadAD.ObtenerIdentidadIDDeMyGNOSSPorIdentidad(pIdentidadID);
            }
        }

        /// <summary>
        /// Obtiene el identificador de la identidad en My Gnoss de la persona pasada como parámetro por nombre
        /// </summary>
        /// <param name="pNombre">Nombre de la persona</param>
        /// <returns></returns>
        public Guid ObtenerIdentidadIDDeMyGNOSSPorNombre(string pNombre)
        {
            return IdentidadAD.ObtenerIdentidadEnOrgIDDeMyGNOSSPorNombre(pNombre, "");
        }

        /// <summary>
        /// Obtiene el identificador del perfil de una identidad
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la identidad</param>
        /// <returns></returns>
        public Guid ObtenerPerfilIDDeIdentidadID(Guid pIdentidadID)
        {
            return IdentidadAD.ObtenerPerfilIDDeIdentidadID(pIdentidadID);
        }

        /// <summary>
        /// Obtiene los identificadores del perfil de varias identidades
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la identidad</param>
        /// <returns></returns>
        public Dictionary<Guid, Guid> ObtenerPerfilesIDDeIdentidadesID(List<Guid> pIdentidadesID)
        {
            return IdentidadAD.ObtenerPerfilesIDDeIdentidadesID(pIdentidadesID);
        }

        /// <summary>
        /// Obtiene el identificador del perfil de una identidad
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la identidad</param>
        /// <returns></returns>
        public Guid? ObtenerPerfilPersonalIDDeUsuarioID(Guid pUsuarioID)
        {
            return IdentidadAD.ObtenerPerfilPersonalIDDeUsuarioID(pUsuarioID);
        }

        /// <summary>
        /// Obtiene los datos de las identidades de unas identidades en MyGnoss.
        /// </summary>
        ///<param name="pIdentidadesID">IDs de Identidades</param>
        /// <returns>Lista de identidades</returns>
        public List<Guid> ObtenerIdentidadesIDDeMyGNOSSPorIdentidades(List<Guid> pIdentidadesID)
        {
            return IdentidadAD.ObtenerIdentidadesIDDeMyGNOSSPorIdentidades(pIdentidadesID);
        }

        /// <summary>
        /// Obtiene el identificador de la identidad en My Gnoss de la organizacion pasada como parámetro por nombre
        /// </summary>
        /// <param name="pNombre">Nombre de la persona</param>
        /// <param name="pNombreOrganizacion">Nombre de la organización</param>
        /// <returns></returns>
        public Guid ObtenerIdentidadEnOrgIDDeMyGNOSSPorNombre(string pNombre, string pNombreOrganizacion)
        {
            return IdentidadAD.ObtenerIdentidadEnOrgIDDeMyGNOSSPorNombre(pNombre, pNombreOrganizacion);
        }

        /// <summary>
        /// Obtiene el identificador de la identidad en My Gnoss de la persona pasada como parámetro por nombre
        /// </summary>
        /// <param name="pNombre">Nombre de la persona</param>
        /// <returns></returns>
        public Guid ObtenerIdentidadOrganizacionIDDeMyGNOSSPorNombre(string pNombre)
        {
            return IdentidadAD.ObtenerIdentidadOrganizacionIDDeMyGNOSSPorNombre(pNombre);
        }

        /// <summary>
        /// Obtiene el identificador del grupo pasado como parámetro por nombre Nombre grupo . nomrbe comunidad
        /// </summary>
        /// <param name="pNombre">Nombre de la persona</param>
        /// <returns></returns>
        public Guid ObtenerGrupoIDPorNombre(string pNombre)
        {
            return IdentidadAD.ObtenerGrupoIDPorNombre(pNombre);
        }

        /// <summary>
        /// Obtiene el identificador del grupo a partir de su NombreCorto
        /// </summary>
        /// <param name="pNombreCorto">NombreCorto del grupo</param>
        /// <returns>Identificador del grupo</returns>
        public Guid ObtenerGrupoIDPorNombreCorto(string pNombreCorto)
        {
            return IdentidadAD.ObtenerGrupoIDPorNombreCorto(pNombreCorto);
        }

        /// <summary>
        /// Obtiene el identificador del grupo pasado como parámetro del proyecto pasado por parámetro
        /// </summary>
        /// <param name="pNombre">Nombre del grupo</param>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <returns></returns>
        public Guid ObtenerGrupoIDPorNombreYProyectoID(string pNombre, Guid pProyectoID)
        {
            return IdentidadAD.ObtenerGrupoIDPorNombreYProyectoID(pNombre, pProyectoID);
        }

        /// <summary>
        /// Obtiene el nombrecorto del grupo a partir de su ID.
        /// </summary>
        /// <param name="pGrupoID">ID del grupo</param>
        /// <returns></returns>
        public string ObtenerNombreCortoGrupoPorID(Guid pGrupoID)
        {
            return IdentidadAD.ObtenerNombreCortoGrupoPorID(pGrupoID);
        }

        /// <summary>
        /// Obtiene el nombrecorto del grupo a partir de su ID.
        /// </summary>
        /// <param name="pGrupoID">ID del grupo</param>
        /// <returns></returns>
        public List<string> ObtenerNombresCortosGruposPorID(List<Guid> pGruposID)
        {
            return IdentidadAD.ObtenerNombresCortosGruposPorID(pGruposID);
        }

        /// <summary>
        /// Obtiene el identificador de los miembros del grupo en MyGnoss pasado como parámetro
        /// </summary>
        /// <param name="pNombre">Nombre de la persona</param>
        /// <returns></returns>
        public List<Guid> ObtenerListaIdentidadesMyGnossDeGrupos(Guid pGrupoID)
        {
            return IdentidadAD.ObtenerListaIdentidadesMyGnossDeGrupos(pGrupoID);
        }

        /// <summary>
        /// Obtiene el identificador de la identidad en My Gnoss del profesor pasado como parámetro por nombre
        /// </summary>
        /// <param name="pNombre">Nombre del profesor</param>
        /// <returns></returns>
        public Guid ObtenerIdentidadProfesorIDDeMyGNOSSPorNombre(string pNombre)
        {
            return IdentidadAD.ObtenerIdentidadProfesorIDDeMyGNOSSPorNombre(pNombre);
        }

        /// <summary>
        /// Obtiene los datos de identidades
        /// </summary>
        ///<param name="pIdentidadID">Identificador de la identidad</param>
        /// <param name="pObtenerSoloActivos">TRUE si obtiene sólo lo que este activo (no eliminado, no fecha de baja,..)</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadPorID(Guid pIdentidadID, bool pObtenerSoloActivos)
        {
            if (pIdentidadID.Equals(UsuarioAD.Invitado))
            {
                return ObtenerIdentidadInvitado();
            }
            else
            {
                return IdentidadAD.ObtenerIdentidadPorID(pIdentidadID, pObtenerSoloActivos);
            }
        }

        /// <summary>
        /// Obtiene los datos Extra de un usuario  (En un proyecto y en el ecosistema)
        /// </summary>
        /// <param name="pIdentidadID">Clave de la identidad</param>
        /// <returns>DataSet de Proyecto con las tablas DatoExtraProyecto y DatoExtraProyectoOpcion</returns>
        public DataWrapperIdentidad ObtenerDatosExtraProyectoOpcionIdentidadPorIdentidadID(Guid pIdentidadID)
        {
            List<Guid> listaIdentidades = new List<Guid>();
            listaIdentidades.Add(pIdentidadID);
            return ObtenerDatosExtraProyectoOpcionIdentidadPorIdentidadID(listaIdentidades);
        }

        /// <summary>
        /// Obtiene los datos Extra de varios usuarios  (En un proyecto y en el ecosistema)
        /// </summary>
        /// <param name="pListaIdentidadesID">Clave de las identidades</param>
        /// <returns>DataSet de Proyecto con las tablas DatoExtraProyecto y DatoExtraProyectoOpcion</returns>
        public DataWrapperIdentidad ObtenerDatosExtraProyectoOpcionIdentidadPorIdentidadID(List<Guid> pListaIdentidadesID)
        {
            return IdentidadAD.ObtenerDatosExtraProyectoOpcionIdentidadPorIdentidadID(pListaIdentidadesID);
        }


        /// <summary>
        /// Obtiene la identidad a partir de su identificador, solo tablas Identidad y Perfil.
        /// </summary>
        ///<param name="pIdentidadID">Identificador de identidad</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadPorIDCargaLigeraTablas(Guid pIdentidadID)
        {
            return IdentidadAD.ObtenerIdentidadPorIDCargaLigeraTablas(pIdentidadID);
        }

        /// <summary>
        /// Obtiene el identificador del perfil de un profesor a partir de un personaID
        /// </summary>
        /// <param name="pPerfilID">Identificador del perfil</param>
        /// <returns>Identificador del perfil del profesor</returns>
        public Guid? ObtenerIdentidadProfesor(Guid pPerfilID)
        {
            return IdentidadAD.ObtenerIdentidadProfesor(pPerfilID);
        }

        /// <summary>
        /// Obtiene el identificador del perfil de un profesor a partir de un personaID
        /// </summary>
        /// <param name="pPersonaID">Identificador de la persona</param>
        /// <returns>Identificador del perfil del profesor</returns>
        public Guid? ObtenerProfesorID(Guid pPersonaID)
        {
            return IdentidadAD.ObtenerProfesorID(pPersonaID);
        }

        /// <summary>
        /// Obtiene el nombre corto de una identidad
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la identidad</param>
        /// <returns></returns>
        public string ObtenerNombreCortoIdentidad(Guid pIdentidadID)
        {
            return IdentidadAD.ObtenerNombreCortoIdentidad(pIdentidadID);
        }

        /// <summary>
        /// Obtiene el nombre corto de un perfil
        /// </summary>
        /// <param name="pPerfilID">Clave del perfil</param>
        /// <returns></returns>
        public KeyValuePair<string, string> ObtenerNombreCortoPerfil(Guid pPerfilID)
        {
            return IdentidadAD.ObtenerNombreCortoPerfil(pPerfilID);
        }

        /// <summary>
        /// Obtiene el nombre corto de la organización con la que participa un usuario en un proyecto. NULL si no participa en el proyecto o participa con identidad personal.
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <param name="pUsuarioID">ID del usuario</param>
        /// <returns>Nombre corto de la organización con la que participa un usuario en un proyecto. NULL si no participa en el proyecto o participa con identidad personal</returns>
        public string ObtenerNombreCortoOrgPerfilParticipaUsuarioEnProyecto(Guid pProyectoID, Guid pUsuarioID)
        {
            return IdentidadAD.ObtenerNombreCortoOrgPerfilParticipaUsuarioEnProyecto(pProyectoID, pUsuarioID);
        }

        /// <summary>
        /// Crea una identidad virtual para el perfil con el que está conectado actualmente el usuario
        /// </summary>
        /// <param name="pDataWrapperIdentidad">Dataset de identidades</param>
        /// <param name="pPerfilID">Identificador del perfil al que se le quiere agregar la identidad de invitado</param>
        /// <param name="pOrganizacionID">Identificador de la organización del proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto para el que se crea la identidad</param>
        /// <returns>Dataset de identidades</returns>
        public AD.EntityModel.Models.IdentidadDS.Identidad CrearIdentidadUsuarioInvitadoParaPerfil(DataWrapperIdentidad pDataWrapperIdentidad, Guid pPerfilID, Guid pOrganizacionID, Guid pProyectoID, Guid pPersonaID)
        {
            if (pPerfilID.Equals(UsuarioAD.Invitado) && pDataWrapperIdentidad.ListaPerfil.FirstOrDefault(perfil => perfil.PerfilID.Equals(pPerfilID)) == null)
            {
                //Creo la fila de perfil para el invitado
                Perfil filaPerfil = new Perfil();
                filaPerfil.CaducidadResSusc = 0;
                filaPerfil.Eliminado = false;
                filaPerfil.NombrePerfil = "Invitado";
                filaPerfil.PerfilID = UsuarioAD.Invitado;
                filaPerfil.PersonaID = UsuarioAD.Invitado;
                if (pPersonaID != null)
                {
                    filaPerfil.PersonaID = pPersonaID;
                }
                filaPerfil.TieneTwitter = false;

                pDataWrapperIdentidad.ListaPerfil.Add(filaPerfil);
                //EntityContext.Perfil.Add(filaPerfil);
            }

            //Creo la fila de identidad
            AD.EntityModel.Models.IdentidadDS.Identidad filaIdent = new Gnoss.AD.EntityModel.Models.IdentidadDS.Identidad();

            //Asigno los valores
            filaIdent.PerfilID = pPerfilID;
            filaIdent.IdentidadID = UsuarioAD.Invitado;
            filaIdent.OrganizacionID = pOrganizacionID;
            filaIdent.ProyectoID = pProyectoID;
            filaIdent.FechaAlta = DateTime.Now;
            filaIdent.NumConnexiones = 0;
            filaIdent.Tipo = (short)TiposIdentidad.Personal; // David: En UtilUsuario se configura el tipo correctamente
            filaIdent.NombreCortoIdentidad = string.Empty; // David: En UtilUsuario se configura el nombre correctamente
            filaIdent.RecibirNewsLetter = true;
            filaIdent.MostrarBienvenida = false;
            filaIdent.ValorAbsoluto = 1;
            filaIdent.DiasUltActualizacion = 0;
            filaIdent.Rank = 0;
            filaIdent.ActualizaHome = true;
            filaIdent.Foto = PersonaAD.SIN_IMAGENES_PERSONA;
			Perfil perfil = pDataWrapperIdentidad.ListaPerfil.FirstOrDefault(perfil => perfil.PerfilID.Equals(pPerfilID));				
            //perfil.Identidad.Add(filaIdent);
            
			//La añado al dataset
			pDataWrapperIdentidad.ListaIdentidad.Add(filaIdent);
            //pDataWrapperIdentidad.AcceptChanges();

            return filaIdent;
        }

        /// <summary>
        /// Obtiene una lista con los perfiles de una persona
        /// </summary>
        /// <param name="pPersonaID">Identificador de la persona</param>
        /// <returns>Lista con los perfiles de una persona</returns>
        public Dictionary<string, string> ObtenerListaPerfilesPersona(Guid pPersonaID)
        {
            return IdentidadAD.ObtenerListaPerfilesPersona(pPersonaID, null);
        }

        /// <summary>
        /// Obtiene una lista con los perfiles de una persona
        /// </summary>
        /// <param name="pPersonaID">Identificador de la persona</param>
        /// <param name="pNombreOrgPerfilID">Lista con el nombre de la org, su ID de perfil y el ID de la org</param>
        /// <returns>Lista con los perfiles de una persona</returns>
        public Dictionary<string, string> ObtenerListaPerfilesPersona(Guid pPersonaID, Dictionary<string, KeyValuePair<Guid, Guid>> pNombreOrgPerfilID)
        {
            return IdentidadAD.ObtenerListaPerfilesPersona(pPersonaID, pNombreOrgPerfilID);
        }

        /// <summary>
        /// Obtiene las identidades del usuario invitado
        /// </summary>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadInvitado()
        {
            DataWrapperIdentidad dataWrapperIdentidad = new DataWrapperIdentidad();

            //Creo las filas de identidad
            Perfil filaPerfil = new Perfil();
            PerfilPersona filaPerfilPersona = new PerfilPersona();
            AD.EntityModel.Models.IdentidadDS.Identidad filaIdent = new Gnoss.AD.EntityModel.Models.IdentidadDS.Identidad();

            //Asigno los valores
            filaPerfil.Eliminado = false;
            filaPerfil.NombreCortoOrg = string.Empty;
            filaPerfil.NombreCortoUsu = "Invitado";
            filaPerfil.NombreOrganizacion = "";
            filaPerfil.NombrePerfil = "Invitado";
            filaPerfil.PerfilID = UsuarioAD.Invitado;
            filaPerfil.PersonaID = UsuarioAD.Invitado;
            filaPerfil.TieneTwitter = false;
            filaPerfil.CaducidadResSusc = 0;

            filaPerfilPersona.PerfilID = UsuarioAD.Invitado;
            filaPerfilPersona.PersonaID = UsuarioAD.Invitado;

            filaIdent.PerfilID = UsuarioAD.Invitado;
            filaIdent.IdentidadID = UsuarioAD.Invitado;
            filaIdent.OrganizacionID = ProyectoAD.MetaOrganizacion;
            filaIdent.ProyectoID = ProyectoAD.MetaProyecto;
            filaIdent.FechaAlta = DateTime.Now;
            filaIdent.NumConnexiones = 0;
            filaIdent.Tipo = (short)TiposIdentidad.Personal;
            filaIdent.NombreCortoIdentidad = "Invitado";
            filaIdent.RecibirNewsLetter = false;
            filaIdent.MostrarBienvenida = false;
            filaIdent.ValorAbsoluto = 0;
            filaIdent.DiasUltActualizacion = 0;
            filaIdent.Rank = 0;
            filaIdent.ActualizaHome = true;
            filaIdent.Foto = PersonaAD.SIN_IMAGENES_PERSONA;

            //Las añado al dataset
            filaPerfil.Identidad.Add(filaIdent);
            dataWrapperIdentidad.ListaPerfil.Add(filaPerfil);
            dataWrapperIdentidad.ListaPerfilPersona.Add(filaPerfilPersona);
            dataWrapperIdentidad.ListaIdentidad.Add(filaIdent);

            return dataWrapperIdentidad;
        }

        /// <summary>
        /// Obtiene las identidad de una persona en un proyecto "Identidad", "Perfil", "PerfilPersona", "PerfilPersonaOrg"
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pPersonaID">Identificador de la persona</param>
        /// <returns>IdentidadDS</returns>
        public DataWrapperIdentidad ObtenerIdentidadDePersonaEnProyecto(Guid pProyectoID, Guid pPersonaID)
        {
            return IdentidadAD.ObtenerIdentidadDePersonaEnProyecto(pProyectoID, pPersonaID);
        }

        /// <summary>
        /// Obtiene las identidad de una persona en un proyecto "Identidad", "Perfil", "PerfilPersona", "PerfilPersonaOrg"
        /// </summary>
        /// <param name="pPersonaID">Identificador de la persona</param>
        /// <param name="pOrganizacionID">Identificador de la organizacion</param>
        /// <returns>IdentidadDS</returns>
        public DataWrapperIdentidad ObtenerIdentidadDePersonaEnOrganizacion(Guid pPersonaID, Guid pOrganizacionID)
        {
            return IdentidadAD.ObtenerIdentidadDePersonaEnOrganizacion(pPersonaID, pOrganizacionID);
        }

        /// <summary>
        /// Obtiene las identidad de unos usuarios en un proyecto
        /// </summary>
        /// <param name="pListaUsuariosID">Lista de identificadores de usuario</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pOrganizacionID">Identificador de la organizacion</param>
        /// <returns>IdentidadDS</returns>
        public DataWrapperIdentidad ObtenerIdentidadesDeUsuariosEnProyectoYOrg(List<Guid> pListaUsuariosID, Guid pProyectoID, Guid pOrganizacionID)
        {
            return IdentidadAD.ObtenerIdentidadesDeUsuariosEnProyectoYOrg(pListaUsuariosID, pProyectoID, pOrganizacionID);
        }

        /// <summary>
        /// Obtiene los GUIDs de los amigos que pertenecen a un proyecto
        /// </summary>
        /// <param name="pIdentidadMyGnossActual">Identificador de la identidad actual en MyGnoss</param>
        /// <param name="pProyecto">Identificador de proyecto</param>
        /// <returns>Dataset de identidades</returns>
        public List<Guid> ObtenerListaIdentidadesAmigosPertenecenProyecto(Guid pIdentidadMyGnossActual, Guid pProyecto)
        {
            return IdentidadAD.ObtenerListaIdentidadesAmigosPertenecenProyecto(pIdentidadMyGnossActual, pProyecto);
        }

        /// <summary>
        /// Obtiene las identidades de Amigos que pertenecen a un proyecto
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la identidad</param>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadesAmigosPertenecenProyecto(Guid pIdentidadID, Guid pProyectoID)
        {
            return IdentidadAD.ObtenerIdentidadesAmigosPertenecenProyecto(pIdentidadID, pProyectoID);
        }

        /// <summary>
        /// Obtiene las identidades de Amigos de la identidad pasada por parámetro
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la identidad</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadesAmigos(Guid pIdentidadID)
        {
            return IdentidadAD.ObtenerIdentidadesAmigos(pIdentidadID);
        }

        /// <summary>
        /// Comprueba si la identidad tiene imagen pregenerada o no
        /// </summary>
        /// <param name="pIdentidadID">Identidad id</param>
        /// <returns></returns>
        public string ObtenerSiIdentidadTieneFoto(Guid pIdentidadID)
        {
            List<Guid> listaIds = new List<Guid>();
            listaIds.Add(pIdentidadID);

            return ObtenerSiListaIdentidadesTienenFoto(listaIds)[pIdentidadID];
        }

        /// <summary>
        /// Comprueba si la identidad tiene imagen pregenerada o no
        /// </summary>
        /// <param name="pIdentidadID">Identidad id</param>
        /// <param name="pTipo">Tipo de identidad</param>
        /// <returns></returns>
        public string ObtenerSiIdentidadTieneFoto(Guid pIdentidadID, TiposIdentidad pTipo)
        {
            List<Guid> listaIds = new List<Guid>();
            listaIds.Add(pIdentidadID);

            return ObtenerSiListaIdentidadesTienenFoto(listaIds)[pIdentidadID];
        }

        /// <summary>
        /// Actualiza la foto desnormalizada en Identidad de una persona.
        /// </summary>
        /// <param name="pPersonaID">ID de persona</param>
        /// <param name="pBorrar">TRUE si la persona ha borrado la foto, FALSE si la ha actualizado</param>
        public void ActualizarFotoIdentidadesPersona(Guid pPersonaID, bool pBorrar)
        {
            List<Guid> listaPersonas = new List<Guid>();
            listaPersonas.Add(pPersonaID);
            ActualizarFotoIdentidadesPersonas(listaPersonas, pBorrar);
        }

        /// <summary>
        /// Actualiza las fotos desnormalizadas en Identidades de varias personas.
        /// </summary>
        /// <param name="pPersonaID">ID de persona</param>
        /// <param name="pBorrar">TRUE si la persona ha borrado la foto, FALSE si la ha actualizado</param>
        public void ActualizarFotoIdentidadesPersonas(List<Guid> pListaPersonaID, bool pBorrar)
        {
            IdentidadAD.ActualizarFotoIdentidadesPersonas(pListaPersonaID, pBorrar);
        }

        /// <summary>
        /// Actualiza la foto desnormalizada en Identidad de una organización.
        /// </summary>
        /// <param name="pOrganizacionID">ID de organización</param>
        /// <param name="pBorrar">TRUE si la organización ha borrado la foto, FALSE si la ha actualizado</param>
        public void ActualizarFotoIdentidadesOrganizacion(Guid pOrganizacionID, bool pBorrar)
        {
            List<Guid> listaOrganizaciones = new List<Guid>();
            listaOrganizaciones.Add(pOrganizacionID);
            ActualizarFotoIdentidadesOrganizaciones(listaOrganizaciones, pBorrar);
        }

        /// <summary>
        /// Actualiza las fotos desnormalizadas en Identidades de varias organizaciones.
        /// </summary>
        /// <param name="pOrganizacionID">ID de organización</param>
        /// <param name="pBorrar">TRUE si las organizaciones han borrado la foto, FALSE si la han actualizado</param>
        public void ActualizarFotoIdentidadesOrganizaciones(List<Guid> pListaOrganizacionesID, bool pBorrar)
        {
            IdentidadAD.ActualizarFotoIdentidadesOrganizaciones(pListaOrganizacionesID, pBorrar);
        }

        /// <summary>
        /// Actualiza la foto desnormalizada en Identidad de una organización.
        /// </summary>
        /// <param name="pPersonaID">ID de persona</param>
        /// <param name="pOrganizacionID">ID de organización</param>
        /// <param name="pBorrar">TRUE si la organización ha borrado la foto, FALSE si la ha actualizado</param>
        public void ActualizarFotoIdentidadesDePersonaDeOrganizacion(Guid pPersonaID, Guid pOrganizacionID, bool pBorrar, bool pUsarFotoPersonal)
        {
            List<KeyValuePair<Guid, Guid>> listaOrgPersona = new List<KeyValuePair<Guid, Guid>>();
            listaOrgPersona.Add(new KeyValuePair<Guid, Guid>(pOrganizacionID, pPersonaID));
            ActualizarFotoIdentidadesDePersonasDeOrganizaciones(listaOrgPersona, pBorrar, pUsarFotoPersonal);
        }

        /// <summary>
        /// Actualiza las foto desnormalizadas en Identidad de una organización.
        /// </summary>
        /// <param name="pListaOrgPersona">Lista de pares con Organizacion-Persona</param>
        /// <param name="pBorrar">TRUE si la organización ha borrado la foto, FALSE si la ha actualizado</param>
        public void ActualizarFotoIdentidadesDePersonasDeOrganizaciones(List<KeyValuePair<Guid, Guid>> pListaOrgPersona, bool pBorrar, bool pUsarFotoPersonal)
        {
            IdentidadAD.ActualizarFotoIdentidadesDePersonasDeOrganizaciones(pListaOrgPersona, pBorrar, pUsarFotoPersonal);
        }

        /// <summary>
        /// Obtiene el id de la persona si la identidad es de tipo 0, el de la organización si es cualquier otro tipo
        /// </summary>
        /// <param name="pIdentidadID">Identidad id</param>
        /// <returns></returns>
        public Guid ObtenerPersonaOOrganizacionIDDeIdentidad(Guid pIdentidadID)
        {
            return IdentidadAD.ObtenerPersonaOOrganizacionIDDeIdentidad(pIdentidadID);
        }

        /// <summary>
        /// Obtiene el id de la persona si la identidad es de tipo 0, el de la organización si es cualquier otro tipo
        /// </summary>
        /// <param name="pIdentidadID">Identidad id</param>
        /// <returns></returns>
        public Guid? ObtenerPersonaIDDeIdentidad(Guid pIdentidadID)
        {
            return IdentidadAD.ObtenerPersonaIDDeIdentidad(pIdentidadID);
        }

        /// <summary>
        /// Indica si la identidad ya participa en el grupo
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la Identidad</param>
        /// <returns></returns>
        public bool ParticipaIdentidadEnGrupo(Guid pIdentidadID, List<Guid> pListaGrupos)
        {
            return IdentidadAD.ParticipaIdentidadEnGrupo(pIdentidadID, pListaGrupos);
        }

        /// <summary>
        /// Indica si la identidad de mygnoss de esta persona participa en los grupos
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la Identidad</param>
        /// <returns></returns>
        public bool ParticipaIdentidadMyGnossParticipaEnGrupo(Guid pIdentidadID, List<Guid> pListaGrupos)
        {
            return IdentidadAD.ParticipaIdentidadMyGnossParticipaEnGrupo(pIdentidadID, pListaGrupos);
        }

        /// <summary>
        /// Indica si la identidad ya participa en el grupo
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la Identidad</param>
        /// <param name="pGrupoID">Identificador del Grupo</param>
        /// <returns></returns>
        public bool ParticipaIdentidadEnGrupo(Guid pIdentidadID, Guid pGrupoID)
        {
            return IdentidadAD.ParticipaIdentidadEnGrupo(pIdentidadID, pGrupoID);
        }

        /// <summary>
        /// Indica si el perfil ya participa en el grupo
        /// </summary>
        /// <param name="pPerfilID">Identificador del perfil</param>
        /// <param name="pGrupoID">Identificador del Grupo</param>
        /// <returns></returns>
        public bool ParticipaPerfilEnGrupo(Guid pPerfilID, List<Guid> pListaGruposID)
        {
            return IdentidadAD.ParticipaPerfilEnGrupo(pPerfilID, pListaGruposID);
        }

        /// <summary>
        /// Obtiene una lista con los nombres de las identidades que le pasamos por parametro
        /// </summary>
        /// <param name="pListaIdentidades">Lista de identidades</param>
        /// <returns></returns>
        public string ObtenerNombreDeIdentidad(Guid pIdentidadID)
        {
            List<Guid> listaIdentidades = new List<Guid>();
            listaIdentidades.Add(pIdentidadID);

            Dictionary<Guid, string> resultado = IdentidadAD.ObtenerNombresDeIdentidades(listaIdentidades);
            if (resultado.Count > 0)
            {
                return resultado[pIdentidadID];
            }

            return "";
        }

        /// <summary>
        /// Obtiene una lista con los nombres de las identidades que le pasamos por parametro
        /// </summary>
        /// <param name="pListaIdentidades">Lista de identidades</param>
        /// <returns></returns>
        public Dictionary<Guid, string> ObtenerNombresDeIdentidades(IList<Guid> pListaIdentidades)
        {
            return IdentidadAD.ObtenerNombresDeIdentidades(pListaIdentidades);
        }

        /// <summary>
        /// Obtiene una lista con los nombres de los perfiles que le pasamos por parametro
        /// </summary>
        /// <param name="pListaPerfiles">Lista de perfiles</param>
        /// <returns></returns>
        public Dictionary<Guid, string> ObtenerNombresDePerfiles(IList<Guid> pListaPerfiles)
        {
            return IdentidadAD.ObtenerNombresDePerfiles(pListaPerfiles);
        }

        /// <summary>
        /// Obtiene una lista con los nombres de los perfiles que le pasamos por parametro
        /// </summary>
        /// <param name="pListaPerfiles">Lista de perfiles</param>
        /// <returns></returns>
        public Dictionary<Guid, string> ObtenerNombresDePerfilesUOrganizacion(IList<Guid> pListaPerfiles)
        {
            return IdentidadAD.ObtenerNombresDePerfilesUOrganizacion(pListaPerfiles);
        }

        /// <summary>
        /// Obtiene una lista con los nombres de los grupos que le pasamos por parametro
        /// </summary>
        /// <param name="pListaIdentidades">Lista de grupos</param>
        /// <returns></returns>
        public Dictionary<Guid, string> ObtenerNombresDeGrupos(IList<Guid> pListaGrupos)
        {
            return IdentidadAD.ObtenerNombresDeGrupos(pListaGrupos);
        }

        /// <summary>
        /// Obtiene el nombre del grupoID pasado por parametro
        /// </summary>
        /// <param name="pGrupoID"></param>
        /// <returns></returns>
        public string ObtenerNombreDeGrupo(Guid pGrupoID)
        {
            return IdentidadAD.ObtenerNombreDeGrupo(pGrupoID);
        }

        /// <summary>
        /// Obtiene una lista con las identidades que tienen foto y las que no
        /// </summary>
        /// <param name="pListaIdentidades">Lista de identidades</param>
        /// <returns></returns>
        public Dictionary<Guid, string> ObtenerSiListaIdentidadesTienenFoto(List<Guid> pListaIdentidades)
        {
            return IdentidadAD.ObtenerFotosIdentidades(pListaIdentidades);
        }


        /// <summary>
        /// Obtiene una lista con las identidades que son visibles y las que no
        /// </summary>
        /// <param name="pListaIdentidades">Lista de identidades</param>
        /// <returns></returns>
        public Dictionary<Guid, bool> ObtenerSiListaIdentidadesSonVisibles(IList<Guid> pListaIdentidades, bool pEsIdentidadInvitada)
        {
            return IdentidadAD.ObtenerSiListaIdentidadesSonVisibles(pListaIdentidades, pEsIdentidadInvitada);
        }

        ///// <summary>
        ///// Comprueba si la identidad tiene imagen pregenerada o no (trata las identidades de Tipo 2 como si fueran de tipo 1 ) a la hora de calcular su foto
        ///// </summary>
        ///// <param name="pIdentidadID">Identidad id</param>
        ///// <returns>Url de la foto</returns>
        //public string ObtenerSiIdentidadTieneFotoSinImportarModoProfesional(Guid pIdentidadID)
        //{
        //    List<Guid> listaIds = new List<Guid>();
        //    listaIds.Add(pIdentidadID);
        //    return ObtenerSiListaIdentidadesTienenFotoSinImportarModoProfesional(listaIds)[pIdentidadID];
        //}

        ///// <summary>
        ///// Obtiene una lista con las identidades que tienen foto y las que no (trata las identidades de Tipo 2 como si fueran de tipo 1)
        ///// </summary>
        ///// <param name="pListaIdentidades">Lista de identidades</param>
        ///// <returns>Dictionary(identificador de Identidad, urlFoto)</returns>
        //public Dictionary<Guid, string> ObtenerSiListaIdentidadesTienenFotoSinImportarModoProfesional(List<Guid> pListaIdentidades)
        //{
        //    return IdentidadAD.ObtenerSiListaIdentidadesTienenFotoSinImportarModoProfesional(pListaIdentidades, Usuario.UsuarioActual.ProyectoID.Equals(ProyectoAD.MetaProyecto));
        //}



        /// <summary>
        /// Obtiene "Identidad", "Perfil", "PerfilPersonaOrg", "PerfilPersona" cargadas de los usuarios de un determinado proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador de proyecto a filtrar</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerTodasIdentidadesDeProyecto(Guid pProyectoID)
        {
            return IdentidadAD.ObtenerTodasIdentidadesDeProyecto(pProyectoID);
        }

        /// <summary>
        /// Obtiene "Identidad", "Perfil", "PerfilPersonaOrg" y "PerfilPersona" de TODOS (bloqueados y expulsados incluidos) los usuarios de un determinado proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Dataset de identidades</returns>
        public List<Guid> ObtenerTodasIdentidadesIDMyGnossDeProyectoActivas(Guid pProyectoID)
        {
            return IdentidadAD.ObtenerTodasIdentidadesIDMyGnossDeProyectoActivas(pProyectoID);
        }


        /// <summary>
        /// Obtiene "Identidad", "Perfil", "PerfilPersonaOrg" y "PerfilPersona" de TODOS (bloqueados y expulsados incluidos) los usuarios de un determinado proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadesDeProyecto(Guid pProyectoID)
        {
            return IdentidadAD.ObtenerIdentidadesDeProyecto(pProyectoID);
        }

        /// <summary>
        /// Obtiene "Identidad", "Perfil", "PerfilPersonaOrg", "PerfilPersona" cargadas de los usuarios de un determinado proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador de proyecto a filtrar</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadesDePersonasNoCorporativasDeProyecto(Guid pProyectoID)
        {
            return IdentidadAD.ObtenerIdentidadesDePersonasNoCorporativasDeProyecto(pProyectoID, false);
        }

        /// <summary>
        /// Obtiene "Identidad", "Perfil", "PerfilPersonaOrg" y "PerfilPersona" de los usuarios de un determinado proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pTraerNombreApellidos">Indica si se deben agregar a Identidad el nombre y apellidos de cada persona</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadesDePersonasNoCorporativasDeProyecto(Guid pProyectoID, bool pTraerNombreApellidos)
        {
            return IdentidadAD.ObtenerIdentidadesDePersonasNoCorporativasDeProyecto(pProyectoID, pTraerNombreApellidos);
        }



        /// <summary>
        /// Obtiene "Identidad", "Perfil", "PerfilPersonaOrg" y "PerfilPersona" de los usuarios de un determinado proyecto (excepto bloqueados y expulsados) en función de TipoListadoUsuariosCMS
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadesDeProyectoParaMosaicoIdentidades(Guid pProyectoID, int pNumeroIdentidades, TipoListadoUsuariosCMS pTipoListado)
        {
            return IdentidadAD.ObtenerIdentidadesDeProyectoParaMosaicoIdentidades(pProyectoID, pNumeroIdentidades, pTipoListado);
        }

        /// <summary>
        /// Obtiene "Identidad", "Perfil", "PerfilPersonaOrg" y "PerfilOrganizacion" de los usuarios vinculados a una determinada organización y la misma organización
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadesDeOrganizacionYEmpleados(Guid pOrganizacionID)
        {
            return ObtenerIdentidadesDeOrganizacionYEmpleados(pOrganizacionID, true);
        }

        /// <summary>
        /// Obtiene "Identidad", "Perfil", "PerfilPersonaOrg" y "PerfilOrganizacion" de los usuarios vinculados a una determinada organizacion y la misma organizacion
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <param name="pSoloActivos">Indica si debe traer solo las indentidades activas o todas</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadesDeOrganizacionYEmpleados(Guid pOrganizacionID, bool pSoloActivos)
        {
            return IdentidadAD.ObtenerIdentidadesDeOrganizacionYEmpleados(pOrganizacionID, pSoloActivos);
        }

        /// <summary>
        /// Obtiene las identidades de una organizacion en un proyecto
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <param name="pProyecto">Proyecto del que debe obtener las identidades</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadesDeOrganizacion(Guid pOrganizacionID, Guid pProyecto)
        {
            List<Guid> listaOrgs = new List<Guid>();
            listaOrgs.Add(pOrganizacionID);
            return ObtenerIdentidadesDeOrganizaciones(listaOrgs, pProyecto);
        }

        /// <summary>
        /// Obtiene las identidades de unas organizaciones en un proyecto
        /// </summary>
        /// <param name="pListaOrganizacionesID">Identificadores de las organizaciónes</param>
        /// <param name="pProyecto">Proyecto del que debe obtener las identidades</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadesDeOrganizaciones(IList<Guid> pListaOrganizacionesID, Guid pProyecto)
        {
            return ObtenerIdentidadesDeOrganizaciones(pListaOrganizacionesID, pProyecto, null);
        }

        /// <summary>
        /// Obtiene las identidades de una organizacion en un proyecto
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <param name="pProyecto">Identificador del proyecto</param>
        /// <param name="pObtenerMyGnoss">TRUE para obtener también la identidad en MyGnoss</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadDeOrganizacion(Guid pOrganizacionID, Guid pProyecto, bool pObtenerMyGnoss)
        {
            return IdentidadAD.ObtenerIdentidadDeOrganizacion(pOrganizacionID, pProyecto, pObtenerMyGnoss);
        }

        /// <summary>
        /// Obtiene las identidades de unas organizaciones en un proyecto
        /// </summary>
        /// <param name="pListaOrganizacionesID">Identificadores de las organizaciónes</param>
        /// <param name="pProyecto">Proyecto del que debe obtener las identidades</param>
        /// <param name="pTipoIdentidad">Tipo de identidad que se quiere obtener (NULL para obtener todos los tipos)</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadesDeOrganizaciones(IList<Guid> pListaOrganizacionesID, Guid pProyecto, TiposIdentidad? pTipoIdentidad)
        {
            return IdentidadAD.ObtenerIdentidadesDeOrganizaciones(pListaOrganizacionesID, pProyecto, pTipoIdentidad);
        }

        public Perfil ObtenerFilaPerfilPorID(Guid pPerfilID)
        {
            return IdentidadAD.ObtenerFilaPerfilPorID(pPerfilID);
        }

        /// <summary>
        /// Obtiene las identidades a partir de una lista de identificadores de identidad
        /// </summary>
        /// <param name="pListaIdentidades">Lista de identificadores de identidad</param>
        /// <param name="pObtenerSoloActivas">TRUE si debe obtener sólo las no eliminadas o que no tengan fecha de baja</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadesPorID(List<Guid> pListaIdentidades, bool pObtenerSoloActivas)
        {
            return this.IdentidadAD.ObtenerIdentidadesPorID(pListaIdentidades, pObtenerSoloActivas);
        }

        /// <summary>
        /// Obtiene las identidades a partir de una lista de nombres de perfil
        /// </summary>
        /// <param name="pListaNombres">Lista de nombres de perfil</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadesPorNombre(string[] pListaNombres)
        {
            return this.IdentidadAD.ObtenerIdentidadesPorNombre(pListaNombres);
        }
        /// <summary>
        /// Obtiene las identidades a partir de una lista de nombres de perfil
        /// </summary>
        /// <param name="pListaNombres">Lista de nombres de perfil</param>
        /// <param name="pProyectoID">proyectoID</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadesPorNombreYProyecto(IEnumerable<string> pListaNombres, Guid pProyectoID)
        {
            return this.IdentidadAD.ObtenerIdentidadesPorNombreYProyecto(pListaNombres, pProyectoID);
        }

        /// <summary>
        /// Devuelve una lista con todas las identidades, ordenadas por visibles y no visibles
        /// </summary>
        /// <param name="pListaIdentidades">Lista de identificadores de identidad</param>
        /// <returns>Lista ordenada de identidades</returns>
        public SortedList<Guid, bool> ObtenerListaIdentidadesVisiblesExternos(List<Guid> pListaIdentidades)
        {
            return this.IdentidadAD.ObtenerListaIdentidadesVisiblesExternos(pListaIdentidades);
        }

        /// <summary>
        /// Devuelve una lista con todas las identidades que tienen suscripciones
        /// </summary>
        /// <returns></returns>
        public List<Guid> ObtenerListaIdentidadesIDConSuscripcion()
        {
            return this.IdentidadAD.ObtenerListaIdentidadesIDConSuscripcion();
        }



        /// <summary>
        /// Devuelve una lista con todos las perfiles que tienen suscripciones
        /// </summary>
        /// <returns></returns>
        public List<Guid> ObtenerListaPerfilesIDConSuscripcionParaEnviar()
        {
            return this.IdentidadAD.ObtenerListaPerfilesIDConSuscripcionParaEnviar();
        }

        /// <summary>
        /// Devuelve una lista con todos las perfiles que tienen suscripciones
        /// </summary>
        /// <returns></returns>
        public List<Guid> ObtenerListaPerfilesIDConSuscripcion()
        {
            return this.IdentidadAD.ObtenerListaPerfilesIDConSuscripcion();
        }

        /// <summary>
        /// Devuelve una lista con todos los perfiles que tienen suscripciones en proyectos configurados para EnviarNotificacionesDeSuscripciones
        /// </summary>
        /// <returns>Lista con los identificadores de los perfiles suscritos</returns>
        public List<Guid> ObtenerListaPerfilesIDConSuscripcionPorProyectos(short pDiaSemana)
        {
            return this.IdentidadAD.ObtenerListaPerfilesIDConSuscripcionPorProyectos(pDiaSemana);
        }

        public Dictionary<Guid, string> ObtenerListaIdentidadesDeCorreosEnProyecto(List<string> pListaCorreos, Guid pProyectoID)
        {
            return this.IdentidadAD.ObtenerListaIdentidadesDeCorreosEnProyecto(pListaCorreos, pProyectoID);
        }

        /// <summary>
        /// Devuelve un dataSet con todos los perfiles que tienen suscripciones.
        /// </summary>
        /// <returns></returns>
        public List<Perfil> ObtenerPerfilesConSuscripcion()
        {
            return this.IdentidadAD.ObtenerPerfilesConSuscripcion();
        }

        /// <summary>
        /// Obtiene una lista de los Perfiles Personales a partir una lista de identificadores de usuario
        /// </summary>
        /// <param name="pListaUsuariosID">Lista de identificadores de usuario</param>
        /// <returns>Lista de perfiles</returns>
        public Dictionary<Guid, Guid> ObtenerListaPerfilPersonalPorUsuarioID(List<Guid> pListaUsuariosID)
        {
            return this.IdentidadAD.ObtenerListaPerfilPersonalPorUsuarioID(pListaUsuariosID);
        }

        /// <summary>
        /// Obtiene una lista de ProyectoID, IdentidadID en los que participa el usuarioActual
        /// </summary>
        /// <param name="pActivas">True si solo se quiere las activas, False si solo se quiere las No activas, 
        /// Null si se quieren todas</param>
        /// <param name="pPersonaID">Identificador de la persona</param>
        /// <returns>lista de ProyectoID, IdentidadID en los que participa el usuarioActual</returns>
        public Dictionary<Guid, Guid> ObtenerListaTodasMisIdentidades(bool? pActivas, Guid pPersonaID)
        {
            return ObtenerListaTodasMisIdentidades(pActivas, null, pPersonaID);
        }

        /// <summary>
        /// Obtiene una lista de ProyectoID, IdentidadID en los que participa el usuarioActual
        /// </summary>
        /// <param name="pActivas">True si solo se quiere las activas, False si solo se quiere las No activas, 
        /// Null si se quieren todas</param>
        /// <param name="pPerfilID">Identificador del perfil del que se quieren obtener las identidades (Puede ser NULL)</param>
        /// <returns>lista de ProyectoID, IdentidadID en los que participa el usuarioActual</returns>
        public Dictionary<Guid, Guid> ObtenerListaTodasMisIdentidadesDePerfil(bool? pActivas, Guid pPerfilID)
        {
            return this.IdentidadAD.ObtenerListaTodasMisIdentidadesDePerfil(pActivas, pPerfilID);
        }

        /// <summary>
        /// Obtiene una lista de ProyectoID, IdentidadID en los que participa el usuarioActual
        /// </summary>
        /// <param name="pActivas">True si solo se quiere las activas, False si solo se quiere las No activas, 
        /// Null si se quieren todas</param>
        /// <param name="pPerfilID">Identificador del perfil del que se quieren obtener las identidades (Puede ser NULL)</param>
        /// <param name="pPersonaID">Identificador de la persona</param>
        /// <returns>lista de ProyectoID, IdentidadID en los que participa el usuarioActual</returns>
        public Dictionary<Guid, Guid> ObtenerListaTodasMisIdentidades(bool? pActivas, Guid? pPerfilID, Guid pPersonaID)
        {
            return this.IdentidadAD.ObtenerListaTodasMisIdentidades(pActivas, pPerfilID, pPersonaID);
        }

        /// <summary>
        /// Carga identidad, perfil y perfilpersona de las identidades personales de MyGnoss 
        /// que son amigos de una identidad de MyGnoss, coinciden con la cadena pasada en nombre y no pertenecen a una organización
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de organización</param>
        /// <param name="pIdentidadUsuarioMyGnoss">Identificador de identidad de usuario de MyGnoss</param>
        /// <param name="pNombre">Cadena de filtro de nombre</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadesAmigosPuedoInvitarOrganizacion(Guid pOrganizacionID, Guid pIdentidadUsuarioMyGnoss, string pNombre)
        {
            return this.IdentidadAD.ObtenerIdentidadesAmigosPuedoInvitarOrganizacion(pOrganizacionID, pIdentidadUsuarioMyGnoss, pNombre);
        }


        /// <summary>
        /// Obtiene la identidad personal de MyGnoss de las personas pasadas por parámetro
        /// </summary>
        /// <param name="pListaPersonas">Lista de identificadores de personas</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadPersonalDeMyGnossDePersonas(List<Guid> pListaPersonas)
        {
            return this.IdentidadAD.ObtenerIdentidadPersonalDeMyGnossDePersonas(pListaPersonas);
        }

        /// <summary>
        /// Obtiene la identidad personal de MyGNOSS de las personas que tienen un email dado, si son visibles por el usuario con el PersonaID pasado
        /// </summary>
        /// <param name="pEmail">Email buscado</param>
        /// <param name="pPersonaID">PersonaID del usuario que realiza la busqueda</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadesVisiblesPorEmail(string pEmail, Guid pPersonaID)
        {
            return this.IdentidadAD.ObtenerIdentidadesVisiblesPorEmail(pEmail, pPersonaID);
        }

        /// <summary>
        /// Obtiene las identidades personales de MyGnoss de personas que no pertenecen a una organización
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización a la que no tienen que pertenecer</param>
        /// <param name="pNombre">Filtro por el nombre de la persona que se desea buscar</param>
        /// <returns>Dataset con las identidades cargadas</returns>
        public DataWrapperIdentidad ObtenerIdentidadesMyGnossNoPerteneceOrg(Guid pOrganizacionID, string pNombre)
        {
            return this.IdentidadAD.ObtenerIdentidadesMyGnossNoPerteneceOrg(pOrganizacionID, pNombre);
        }

        /// <summary>
        /// Obtiene todos los posibles perfiles e identidades que tenga (eliminados o no). Carga tabla "Perfil" e "Identidad"
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización de la que se quiere obtener las personas</param>
        /// <param name="pOrganizacionProyectoID">Identificador del proyecto de la organización del que se quieren obtener los participantes</param>
        /// <param name="pProyectoID">Identificador del proyecto del que se quieren obtener los participantes</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadesDePersonasConOrganizacionEnProyecto(Guid pOrganizacionID, Guid pOrganizacionProyectoID, Guid pProyectoID)
        {
            return IdentidadAD.ObtenerIdentidadesDePersonasConOrganizacionEnProyecto(pOrganizacionID, pOrganizacionProyectoID, pProyectoID, false);
        }

        /// <summary>
        /// Obtiene todos los posibles perfiles e identidades de los miembros de una organizacion en un proyectos. Carga tabla "Perfil" e "Identidad"
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización de la que se quiere obtener las personas</param>
        /// <param name="pProyectoID">Identificador del proyecto del que se quieren obtener los participantes</param>
        /// <param name="pTraerDadosDeBaja">Indica si se deben traer o no las identidades dadas de baja y expulsadas</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadesDeMiembrosDeOrganizacionEnProyecto(Guid pOrganizacionID, Guid pProyectoID, bool pTraerDadosDeBaja)
        {
            return IdentidadAD.ObtenerIdentidadesDeMiembrosDeOrganizacionEnProyecto(pOrganizacionID, pProyectoID, pTraerDadosDeBaja);
        }

        /// <summary>
        /// Obtiene a partir del identificador de organización pasado por parámetro todos 
        /// los posibles perfiles e identidades que tenga (eliminados o no). 
        /// Carga tabla "Perfil" e "Identidad"
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización de la que se quiere obtener las personas</param>
        /// <param name="pOrganizacionProyectoID">Identificador del proyecto de la organización del que se quieren obtener los participantes</param>
        /// <param name="pProyectoID">Identidad del proyecto del que se quieren obtener los participantes</param>
        /// <param name="pTraerNombreApellidos">Indica si se deben agregar a Identidad el nombre y apellidos de cada persona</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadesDePersonasConOrganizacionEnProyecto(Guid pOrganizacionID, Guid pOrganizacionProyectoID, Guid pProyectoID, bool pTraerNombreApellidos)
        {
            return IdentidadAD.ObtenerIdentidadesDePersonasConOrganizacionEnProyecto(pOrganizacionID, pOrganizacionProyectoID, pProyectoID, pTraerNombreApellidos);
        }

        /// <summary>
        /// Obtiene todos los gadgets de un perfil
        /// </summary>
        /// <param name="pPerfilID">Identificador del perfil del que se quieren obtener los gadgets</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerGadgetsPerfil(Guid pPerfilID)
        {
            return IdentidadAD.ObtenerGadgetsPerfil(pPerfilID);
        }

        /// <summary>
        /// Obtiene todos las identidades de una organizacion visibles en un proyecto
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización de la que se quiere obtener las personas</param>
        /// <param name="pProyectoID">Identificador del proyecto del que se quieren obtener los participantes</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadesDePersonasEnOrganizacionVisiblesEnProyecto(Guid pOrganizacionID, Guid pProyectoID)
        {
            return IdentidadAD.ObtenerIdentidadesDePersonasEnOrganizacionVisiblesEnProyecto(pOrganizacionID, pProyectoID);
        }

        /// <summary>
        /// Obtiene la clave de la identidad de un perfil en un proyecto concreto
        /// </summary>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <param name="pPerfilID">Identificador del perfil</param>
        /// <returns>IdentidadID</returns>
        public Guid? ObtenerIdentidadIDDePerfilEnProyecto(Guid pProyectoID, Guid pPerfilID)
        {
            return IdentidadAD.ObtenerIdentidadIDDePerfilEnProyecto(pProyectoID, pPerfilID);
        }

        /// <summary>
        /// Obtiene la clave de la identidad de un perfil en un proyecto concreto
        /// </summary>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <param name="pPerfilID">Identificador del perfil</param>
        /// <returns>IdentidadID</returns>
        public List<Guid> ObtenerIdentidadesIDDePerfilEnProyecto(Guid pProyectoID, List<Guid> pListaPerfilID)
        {
            return IdentidadAD.ObtenerIdentidadesIDDePerfilEnProyecto(pProyectoID, pListaPerfilID);
        }

        /// <summary>
        /// Obtiene la clave de la identidad de un perfil en un proyecto concreto
        /// </summary>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <param name="pPerfilID">Identificador del perfil</param>
        /// <returns>IdentidadID</returns>
        public Dictionary<Guid, Guid> ObtenerIdentidadesIDyPerfilEnProyecto(Guid pProyectoID, List<Guid> pListaPerfilID)
        {
            return IdentidadAD.ObtenerIdentidadesIDyPerfilEnProyecto(pProyectoID, pListaPerfilID);
        }

        /// <summary>
        /// Obtiene un array cuyo primer elemento es el identificador de la identidad de la persona y el segundo es el identificador del perfil de la identidad (en un proyecto)
        /// </summary>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <param name="pPersonaID">Identificador de la persona</param>
        /// <returns>Array de identificadores de identidad</returns>
        public Guid[] ObtenerIdentidadIDDePersonaEnProyecto(Guid pProyectoID, Guid pPersonaID)
        {
            return IdentidadAD.ObtenerIdentidadIDDePersonaEnProyecto(pProyectoID, pPersonaID);
        }

        /// <summary>
        /// Obtiene una lista de las identidades de la persona en un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <param name="pUsuariosID">Lista de identificadores de usuario</param>
        /// <returns>Identificador de la identidad</returns>
        public List<Guid> ObtenerIdentidadesIDDeusuariosEnProyecto(Guid pProyectoID, List<Guid> pUsuariosID)
        {
            return IdentidadAD.ObtenerIdentidadesIDDeusuariosEnProyecto(pProyectoID, pUsuariosID, false);
        }

        /// <summary>
        /// Obtiene una lista de las identidades de los usuarios en un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <param name="pUsuariosID">Lista de identificadores de usuario</param>
        /// <param name="pSoloActivos">Indica si obtiene únicamente las identidades activas en el proyecto</param>
        /// <returns>Identificador de la identidad</returns>
        public List<Guid> ObtenerIdentidadesIDDeusuariosEnProyecto(Guid pProyectoID, List<Guid> pUsuariosID, bool pSoloActivos)
        {
            return IdentidadAD.ObtenerIdentidadesIDDeusuariosEnProyecto(pProyectoID, pUsuariosID, pSoloActivos);
        }

        /// <summary>
        /// Obtiene un array cuyo primer elemento es el identificador de la identidad del usuario y el segundo es el identificador del perfil de la identidad (en un proyecto)
        /// </summary>
        /// <param name="pUsuario">Usuario</param>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <param name="pOrganizacion">Organización</param>
        /// <param name="pObtenerEliminadas">TRUE si se deben de obtener las filas ya eliminadas, FALSE en caso contrario</param>
        /// <returns>Array de identificadores de identidad</returns>
        public Guid[] ObtenerIdentidadIDDeUsuarioEnProyectoYOrg(string pUsuario, Guid pProyectoID, string pOrganizacion, bool pObtenerEliminadas)
        {
            return ObtenerIdentidadIDDeUsuarioEnProyectoYOrg(pUsuario, pProyectoID, pOrganizacion, Guid.Empty, pObtenerEliminadas);
        }

        /// <summary>
        /// Obtiene un array cuyo primer elemento es el identificador de la identidad del usuario y el segundo es el identificador del perfil de la identidad (en un proyecto)
        /// </summary>
        /// <param name="pUsuario">Usuario</param>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <param name="pOrganizacion">Organización</param>
        /// <param name="pOrganizacionID">Identificador de la organizacion</param>
        /// <param name="pObtenerEliminadas">TRUE si se deben de obtener las filas ya eliminadas, FALSE en caso contrario</param>
        /// <returns>Array de identificadores de identidad</returns>
        public Guid[] ObtenerIdentidadIDDeUsuarioEnProyectoYOrg(string pUsuario, Guid pProyectoID, string pOrganizacion, Guid pOrganizacionID, bool pObtenerEliminadas)
        {
            return IdentidadAD.ObtenerIdentidadIDDeUsuarioEnProyectoYOrg(pUsuario, pProyectoID, pOrganizacion, pOrganizacionID, pObtenerEliminadas);
        }

        /// <summary>
        /// Obtiene la identidad de una organización en un proyecto
        /// </summary>
        /// <param name="pOrganizacion">Identificador de organización</param>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <returns>Identificador de identidad</returns>
        public Guid ObtenerIdentidadIDDeOrganizacionEnProyecto(string pOrganizacion, Guid pProyectoID)
        {
            return IdentidadAD.ObtenerIdentidadIDDeOrganizacionEnProyecto(pOrganizacion, pProyectoID);
        }

        /// <summary>
        /// Obtiene el identificador de la identidad de una organización en un proyecto
        /// </summary>
        /// <param name="pOrganizacionID">ID de organización</param> 
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <returns>Identificador de la identidad</returns>
        public Guid ObtenerIdentidadIDDeOrganizacionIDEnProyecto(Guid pOrganizacionID, Guid pProyectoID)
        {
            return IdentidadAD.ObtenerIdentidadIDDeOrganizacionIDEnProyecto(pOrganizacionID, pProyectoID);
        }

        /// <summary>
        /// Obtiene las identidades de los miembros de una organizacion
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organizacion</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>IdentidadDS</returns>
        public List<IdentidadesDeUsuariosDeOrganizacionDeProyectoYDeMetaProyecto> ObtenerTodasIdentidadesDeUsuariosDeOrganizacionDeProyectoYDeMetaProyecto(Guid pOrganizacionID, Guid pProyectoID)
        {
            return IdentidadAD.ObtenerTodasIdentidadesDeUsuariosDeOrganizacionDeProyectoYDeMetaProyecto(pOrganizacionID, pProyectoID);
        }

        /// <summary>
        /// Obtiene atraves del UsuarioID todos los posibles perfiles e identidades que tenga (eliminados o no). Carga tabla "Perfil" y "Identidad" , "PerfilPersona" , "PerfilPersonaOrg"
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerPerfilesDeUsuario(Guid pUsuarioID)
        {
            if (pUsuarioID.Equals(UsuarioAD.Invitado))
            {
                return ObtenerIdentidadInvitado();
            }
            else
            {
                return this.IdentidadAD.ObtenerPerfilesDeUsuario(pUsuarioID);
            }
        }

        /// <summary>
        /// Obtiene a partir del identificador de usuario todos los posibles perfiles que tenga activos en una lista.
        /// </summary>
        /// <param name="pUsuarioID">Clave del usuario</param>
        /// <returns>Lista de IDs de perfiles</returns>
        public List<Guid> ObtenerListaPerfilesDeUsuario(Guid pUsuarioID)
        {
            return this.IdentidadAD.ObtenerListaPerfilesDeUsuario(pUsuarioID);
        }

        /// <summary>
        /// Obtiene a partir del identificador de usuario todos los posibles perfiles que tenga activos en una lista.
        /// </summary>
        /// <param name="pUsuarioID">Clave del usuario</param>
        /// <returns>Lista de IDs de perfiles</returns>
        public List<Guid> ObtenerListaIdentidadesDeUsuario(Guid pUsuarioID)
        {
            return this.IdentidadAD.ObtenerListaIdentidadesDeUsuario(pUsuarioID);
        }

        /// <summary>
        /// Obtiene a partir de los identificadores de usuario todos los posibles perfiles que tengan activos en una lista.
        /// </summary>
        /// <param name="pListaUsuarioIDs">Lista de identificadores de usuario</param>
        /// <returns>Diccionario de UsuarioID y lista de PerfilesID</returns>
        public Dictionary<Guid, List<Guid>> ObtenerListaIdentidadesDeListaUsuarios(List<Guid> pListaUsuarioIDs)
        {
            return this.IdentidadAD.ObtenerListaIdentidadesDeListaUsuarios(pListaUsuarioIDs);
        }


        /// <summary>
        /// Obtiene a partir de los identificadores de usuario todos los posibles perfiles que tengan activos en una lista.
        /// </summary>
        /// <param name="pListaUsuarioIDs">Lista de identificadores de usuario</param>
        /// <returns>Diccionario de UsuarioID y lista de PerfilesID</returns>
        public Dictionary<Guid, List<Guid>> ObtenerListaPerfilesDeListaUsuarios(List<Guid> pListaUsuarioIDs)
        {
            return this.IdentidadAD.ObtenerListaPerfilesDeListaUsuarios(pListaUsuarioIDs);
        }

        /// <summary>
        /// Obtiene atraves del UsuarioID todos el perfil proseor. Carga solo la tabla "Perfil" "Identidad"
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerPerfilProfesorDeUsuario(Guid pUsuarioID)
        {
            return this.IdentidadAD.ObtenerPerfilProfesorDeUsuario(pUsuarioID);
        }

        /// <summary>
        /// Obtiene atraves del UsuarioID todos los posibles perfiles e identidades que tenga (eliminados o no). Carga tabla "Perfil" y "Identidad" , "PerfilPersona" , "PerfilPersonaOrg"
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerPerfilesDeUsuarioEnProyecto(Guid pUsuarioID, Guid pProyectoID)
        {
            if (pUsuarioID.Equals(UsuarioAD.Invitado))
            {
                return ObtenerIdentidadInvitado();
            }
            else
            {
                return this.IdentidadAD.ObtenerPerfilesDeUsuarioEnProyecto(pUsuarioID, pProyectoID);
            }
        }

        /// <summary>
        /// Comprueba si existe algún perfil personal en base de datos para la persona pasada por parámetro
        /// </summary>
        /// <param name="pPersonaID">Identificador de la persona</param>
        /// <returns>TRUE si existe el perfil, FALSE en caso contrario</returns>
        public bool ExistePerfilPersonal(Guid pPersonaID)
        {
            return IdentidadAD.ExistePerfilPersonal(pPersonaID);
        }

        /// <summary>
        /// Comprueba si existe algun perfil Persona+Organizacion en base de datos para la persona y organización pasadas por parámetro
        /// </summary>
        /// <param name="pPersonaID">Identificador de la persona</param>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <returns>TRUE si existe el perfil </returns>
        public bool ExistePerfilPersonaOrg(Guid pPersonaID, Guid pOrganizacionID)
        {
            return IdentidadAD.ExistePerfilPersonaOrg(pPersonaID, pOrganizacionID);
        }

        /// <summary>
        /// Obtiene una lista con los IDs de los participantes del gurpo
        /// </summary>
        /// <param name="pGrupoID">Identificador del grupo</param>
        /// <returns></returns>
        public List<Guid> ObtenerParticipantesGrupo(Guid pGrupoID)
        {
            return IdentidadAD.ObtenerParticipantesGrupo(pGrupoID);
        }

        public List<Guid> ObtenerPerfilesParticipantesGrupo(Guid pGrupoID)
        {
            return IdentidadAD.ObtenerPerfilesParticipantesGrupo(pGrupoID);
        }

        /// <summary>
        /// Obtiene los grupos de una identidad
        /// </summary>
        /// <param name="pIdentidadID">Identidad de la que queremos obtener los grupos</param>
        /// <returns>DataSet con el grupo</returns>
        public DataWrapperIdentidad ObtenerGruposParticipaIdentidad(Guid pIdentidadID, bool pCargarPrivados)
        {
            return IdentidadAD.ObtenerGruposParticipaIdentidad(pIdentidadID, pCargarPrivados);
        }

        /// <summary>
        /// Obtiene la lista de nombres cortos de los grupos en los que participa un usuario en un proyecto concreto
        /// </summary>
        ///<param name="pUsuarioID">UsuarioID</param>
        ///<param name="pProyectoID">ProyectoID</param>
        ///<returns>Lista de los nombres cortos en los que participa un usuario</returns>
        public List<string> ObtenerGruposDeUsuarioEnProyecto(Guid pUsuarioID, Guid pProyectoID)
        {
            return IdentidadAD.ObtenerGruposDeUsuarioEnProyecto(pUsuarioID, pProyectoID);
        }

        /// <summary>
        /// Obtiene los grupos de un perfil.
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la identidad del usuario</param>
        /// <param name="pIdentidadMyGnossID">Identificador de la identidad del usuario en mygnoss</param>
        /// <returns>Lista con IDs de grupo</returns>
        public Dictionary<Guid, string> ObtenerGruposIDParticipaPerfil(Guid pIdentidadID, Guid pIdentidadMyGnossID)
        {
            return IdentidadAD.ObtenerGruposIDParticipaPerfil(pIdentidadID, pIdentidadMyGnossID);
        }

        /// <summary>
        /// Devuelve si la identidad pertenece a algun grupo
        /// </summary>
        /// <param name="pProyectoID"></param>
        /// <param name="pPerfilID"></param>
        /// <returns></returns>
        public bool TieneIdentidadGrupos(Guid pProyectoID, Guid pPerfilID)
        {
            return IdentidadAD.TieneIdentidadGrupos(pProyectoID, pPerfilID);
        }

        /// <summary>
        /// Devuelve si la identidad pertenece a algun grupo
        /// </summary>
        /// <param name="pProyectoID"></param>
        /// <param name="pPerfilID"></param>
        /// <returns></returns>
        public bool TieneIdentidadGrupos(Guid pIdentidadID)
        {
            return IdentidadAD.TieneIdentidadGrupos(pIdentidadID);
        }

        /// <summary>
        /// Devuelve si la identidad pertenece a algun grupo con recursos privados
        /// </summary>
        /// <param name="pProyectoID"></param>
        /// <param name="pPerfilID"></param>
        /// <returns></returns>
        public bool TieneIdentidadGruposConRecursosPrivados(Guid pProyectoID, Guid pPerfilID)
        {
            return IdentidadAD.TieneIdentidadGruposConRecursosPrivados(pProyectoID, pPerfilID);
        }

        /// <summary>
        /// Suma uno a uno de los contadores del usuario
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la identidad</param>
        /// <param name="pContador">String del contador a actualizar. Se pueden usar las constantes definidas en UsuarioAD: CONTADOR_NUMERO_ACCESOS, CONTADOR_NUMERO_DESCARGAS, CONTADOR_NUMERO_VISITAS</param>
        public void ActualizarContadorIdentidad(Guid pIdentidadID, string pContador)
        {
            try
            {
                IdentidadAD.ActualizarContadorIdentidad(pIdentidadID, pContador);
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, string.Format("Error al actualizar el contador {0} de la identidad {1}", pContador, pIdentidadID),mlogger);
            }
        }

        /// <summary>
        /// Obtiene los contadores de una identidad
        /// </summary>
        /// <param name="pListaIdentidadesID">Lista de identificadores de identidades</param>
        /// <returns>Devuelve los contadores de la identidad en un diccionario de diccionario, en el que las claves serán la identidad y una de las constantes de IdentidadAD: CONTADOR_NUMERO_VISITAS, CONTADOR_NUMERO_DESCARGAS; y el valor será el contador de la identidad</returns>
        public Dictionary<Guid, Dictionary<string, int>> ObtenerContadoresDeIdentidad(List<Guid> pListaIdentidadesID)
        {
            return IdentidadAD.ObtenerContadoresDeIdentidad(pListaIdentidadesID);
        }

        /// <summary>
        /// Actualiza el contador de una identidad identidad
        /// </summary>
        /// <param name="pIdentidadID">Identidad a la que modificar el contador</param>
        /// <param name="pTipoDoc">Tipo de documento</param>
        /// <param name="pNombreSem">Nombre semántico del documento(****.owl) en caso de que sea semántico</param>
        /// <param name="pIncrementarPublicado">Incremento que hay que hacer en el número de publicados (puede ser 0 o negativos)</param>
        /// <param name="pIncrementarCompartido">Incremento que hay que hacer en el número de compartidos (puede ser 0 o negativos)</param>
        /// <param name="pIncrementarComentario">Incremento que hay que hacer en el número de comentarios (puede ser 0 o negativos)</param>
        public void IncrementarIdentidadContadoresRecursos(Guid pIdentidadID, TiposDocumentacion pTipoDoc, string pNombreSem, int pIncrementoPublicados, int pIncrementoCompartidos, int pIncrementoComentarios)
        {
            IdentidadAD.IncrementarIdentidadContadoresRecursos(pIdentidadID, pTipoDoc, pNombreSem, pIncrementoPublicados, pIncrementoCompartidos, pIncrementoComentarios);
        }

        /// <summary>
        /// Devuelve si la identidad pertenece a algun grupo
        /// </summary>
        /// <param name="pProyectoID"></param>
        /// <param name="pPerfilID"></param>
        /// <returns></returns>
        public bool TienePerfilGruposConRecursosPrivadosEnComunConElPerfilPagina(Guid? pProyectoID, Guid pPerfilID, Guid pPerfilPaginaID)
        {
            return IdentidadAD.TienePerfilGruposConRecursosPrivadosEnComunConElPerfilPagina(pProyectoID, pPerfilID, pPerfilPaginaID);
        }

        /// <summary>
        /// Obtiene una lista con los usuarios de la comunidad que participan en algun grupo
        /// </summary>
        /// <param name="pProyectoID"></param>
        /// <returns></returns>
        public List<Guid> ObtenerPerfilesProyectoParticipanEnGrupos(Guid pProyectoID)
        {
            return IdentidadAD.ObtenerPerfilesProyectoParticipanEnGrupos(pProyectoID);
        }

        /// <summary>
        /// Obtiene una lista con los usuarios de la comunidad que participan en algun grupo
        /// </summary>
        /// <param name="pProyectoID"></param>
        /// <returns></returns>
        public List<Guid> ObtenerPerfilesProyectoParticipanEnGruposConRecursosPrivados(Guid pProyectoID)
        {
            return IdentidadAD.ObtenerPerfilesProyectoParticipanEnGruposConRecursosPrivados(pProyectoID);
        }

        /// <summary>
        /// Obtiene un grupo por el nombre corto y la organizacion
        /// </summary>
        /// <param name="pNombreCorto">Nombre corto del grupo</param>
        /// <param name="pOrganizacion">Organizacion del Grupo</param>
        /// <returns>DataSet con el grupo</returns>
        public DataWrapperIdentidad ObtenerGrupoPorNombreCortoYOrganizacion(string pNombreCorto, Guid pOrganizacionID)
        {
            return IdentidadAD.ObtenerGrupoPorNombreCortoYOrganizacion(pNombreCorto, pOrganizacionID, true);
        }

        /// <summary>
        /// Obtiene un grupo por el nombre corto y la organizacion
        /// </summary>
        /// <param name="pNombreCorto">Nombre corto del grupo</param>
        /// <param name="pOrganizacion">Organizacion del Grupo</param>
        /// <returns>DataSet con el grupo</returns>
        public DataWrapperIdentidad ObtenerGrupoPorNombreCortoYOrganizacion(string pNombreCorto, Guid pOrganizacionID, bool pCargarIdentidades)
        {
            return IdentidadAD.ObtenerGrupoPorNombreCortoYOrganizacion(pNombreCorto, pOrganizacionID, pCargarIdentidades);
        }

        public DataWrapperIdentidad ObtenerGrupoPorNombreCortoYPerfilID(string pNombreCorto, Guid pPerfilID)
        {
            return ObtenerGrupoPorNombreCortoYPerfilID(pNombreCorto, pPerfilID, true);
        }

        public DataWrapperIdentidad ObtenerGrupoPorNombreCortoYPerfilID(string pNombreCorto, Guid pPerfilID, bool pCargarIdentidades)
        {
            return IdentidadAD.ObtenerGrupoPorNombreCortoYPerfilID(pNombreCorto, pPerfilID, pCargarIdentidades);
        }

		/// <summary>
		/// Obtiene unos grupos por los nombres cortos y la organizacion
		/// </summary>
		/// <param name="pNombresCortos">Nombres cortos de lps grupos</param>
		/// <param name="pOrganizacionID">Organizacion del Grupo</param>
		/// <returns>Lista de IDs de grupos</returns>
		public List<Guid> ObtenerGruposIDPorNombreCortoYOrganizacion(List<string> pNombresCortos, Guid pOrganizacionID)
        {
            return IdentidadAD.ObtenerGruposIDPorNombreCortoYOrganizacion(pNombresCortos, pOrganizacionID);
        }

        public GrupoIdentidadesParticipacion ObtenerGrupoIdentidadesParticipacion(Guid pGrupoID, Guid pIdentidadID)
        {
            return IdentidadAD.ObtenerGrupoIdentidadesParticipacion(pGrupoID, pIdentidadID);
        }

		/// <summary>
		/// Obtiene el identificador del grupo por el nombre corto y la organizacion
		/// </summary>
		/// <param name="pNombreCorto">Nombre corto del grupo</param>
		/// <param name="pOrganizacionID">Organizacion del Grupo</param>
		/// <returns>Identificador del grupo</returns>
		public Guid ObtenerGrupoIDPorNombreCortoYOrganizacion(string pNombreCorto, Guid pOrganizacionID)
        {
            return IdentidadAD.ObtenerGrupoIDPorNombreCortoYOrganizacion(pNombreCorto, pOrganizacionID);
        }

        /// <summary>
        /// Obtiene un grupo por el nombre corto y el proyecto
        /// </summary>
        /// <param name="pNombreCorto">Nombre corto del grupo</param>
        /// <param name="pProyecto">Proyecto del Grupo</param>
        /// <returns>DataSet con el grupo</returns>
        public DataWrapperIdentidad ObtenerGrupoPorNombreCortoYProyecto(string pNombreCorto, Guid pProyectoID, bool pCargarIdentidades = true, bool pCargarSoloActivas = false)
        {
            return IdentidadAD.ObtenerGrupoPorNombreCortoYProyecto(pNombreCorto, pProyectoID, pCargarIdentidades, pCargarSoloActivas);
        }

        /// <summary>
        /// Obtiene unos grupos por los nombrse cortos y el proyecto
        /// </summary>
        /// <param name="pNombreCortos">Nombres cortos de lps grupos</param>
        /// <param name="pProyecto">Proyecto del Grupo</param>
        /// <returns>Lista de IDs de grupos</returns>
        public List<Guid> ObtenerGruposIDPorNombreCortoYProyecto(List<string> pNombreCortos, Guid pProyectoID)
        {
            return IdentidadAD.ObtenerGruposIDPorNombreCortoYProyecto(pNombreCortos, pProyectoID);
        }

        /// <summary>
        /// Obtiene los identificadores de perfil por los nombres cortos y el proyecto
        /// </summary>
        /// <param name="pNombresCortos">Nombres cortos de los perfiles</param>
        /// <param name="pProyecto">Proyecto del Grupo</param>
        /// <returns>Lista de IDs de grupos</returns>
        public List<Guid> ObtenerPerfilIDPorNombreCortoYProyecto(List<string> pNombresCortos, Guid pProyectoID)
        {
            return IdentidadAD.ObtenerPerfilIDPorNombreCortoYProyecto(pNombresCortos, pProyectoID);
        }

        /// <summary>
        /// Obtiene unos grupos por los nombrse cortos
        /// </summary>
        /// <param name="pNombreCortos">Nombres cortos de lps grupos</param>
        /// <returns>Lista de IDs de grupos</returns>
        public List<Guid> ObtenerGruposIDPorNombreCorto(List<string> pNombreCortos)
        {
            return IdentidadAD.ObtenerGruposIDPorNombreCorto(pNombreCortos);
        }

        /// <summary>
        /// Obtiene unos grupos por los nombrse cortos de un proyecto y de organizaciones.
        /// </summary>
        /// <param name="pNombreCortos">Nombres cortos de lps grupos</param>
        /// <param name="pNombresCortos">ID de proyecto</param>
        /// <returns>Lista de IDs de grupos</returns>
        public List<Guid> ObtenerGruposIDPorNombreCortoEnProyectoYEnOrganizacion(List<string> pNombresCortos, Guid pProyectoID)
        {
            return IdentidadAD.ObtenerGruposIDPorNombreCortoEnProyectoYEnOrganizacion(pNombresCortos, pProyectoID);
        }

        /// <summary>
        /// Comprueba si existe algun grupo en base de datos con el nombre pasado por parámetro
        /// </summary>
        /// <param name="pNombreGrupo">Nombre del grupo</param>
        /// <param name="pGrupoID">ID del grupo que se va a modificar</param>
        /// <param name="pOrganizacionID">Organizacion a la que pertenece el grupo</param>
        /// <returns>TRUE si existe</returns>
        public bool ExisteGrupoEnOrganizacionPorNombre(string pNombreGrupo, Guid pGrupoID, Guid pOrganizacionID)
        {
            return IdentidadAD.ExisteGrupoEnOrganizacionPorNombre(pNombreGrupo, pGrupoID, pOrganizacionID);
        }

        /// <summary>
        /// Comprueba si existe algun grupo en base de datos con el nombre pasado por parámetro
        /// </summary>
        /// <param name="pNombreGrupo">Nombre del grupo</param>
        /// <param name="pGrupoID">ID del grupo que se va a modificar</param>
        /// <param name="pProyectoID">ProyectoID al que pertenece el grupo</param>
        /// <returns>TRUE si existe</returns>
        public bool ExisteGrupoEnProyectoPorNombre(string pNombreGrupo, Guid pGrupoID, Guid pProyectoID)
        {
            return IdentidadAD.ExisteGrupoEnProyectoPorNombre(pNombreGrupo, pGrupoID, pProyectoID);
        }

        /// <summary>
        /// Comprueba si existe algun grupo en base de datos con el nombre corto pasado por parámetro
        /// </summary>
        /// <param name="pNombreGrupo">Nombre corto del grupo</param>
        /// <returns>TRUE si existe</returns>
        public bool ExisteGrupoEnOrganizacionPorNombreCorto(string pNombreCortoGrupo, Guid pOrganizacionID)
        {
            return IdentidadAD.ExisteGrupoEnOrganizacionPorNombreCorto(pNombreCortoGrupo, pOrganizacionID);
        }

        /// <summary>
        /// Comprueba si existe algun grupo en base de datos con el nombre corto pasado por parámetro
        /// </summary>
        /// <param name="pNombreGrupo">Nombre corto del grupo</param>
        /// <returns>TRUE si existe</returns>
        public bool ExisteGrupoEnProyectoPorNombreCorto(string pNombreCortoGrupo, Guid pProyectoID)
        {
            return IdentidadAD.ExisteGrupoEnProyectoPorNombreCorto(pNombreCortoGrupo, pProyectoID);
        }

        /// <summary>
        /// Obtiene los grupos de la organizacion
        /// </summary>
        /// <param name="pOrganizacionID">Id de la organizacion</param>
        /// <returns>DataSet con el grupo</returns>
        public DataWrapperIdentidad ObtenerGruposDeOrganizacion(Guid pOrganizacionID)
        {
            return ObtenerGruposDeOrganizacion(pOrganizacionID, true);
        }

        /// <summary>
        /// Obtiene la tabla GrupoIdentidades de los grupos por la organizacion
        /// </summary>
        /// <param name="pOrganizacionID">Proyecto del Grupo</param>
        /// <param name="pObtenerMiembros">Obtiene las tablas GrupoIdentidadesOrganizacio y GrupoIdentidadesParticipacion</param>
        /// <returns>DataSet con el grupo</returns>
        public DataWrapperIdentidad ObtenerGruposDeOrganizacion(Guid pOrganizacionID, bool pObtenerMiembros)
        {
            return IdentidadAD.ObtenerGruposDeOrganizacion(pOrganizacionID, pObtenerMiembros);
        }
        /// <summary>
        /// Obtiene los grupos por el proyecto
        /// </summary>
        /// <param name="pProyecto">Proyecto del Grupo</param>
        /// <returns>DataSet con el grupo</returns>
        public DataWrapperIdentidad ObtenerGruposDeProyecto(Guid pProyectoID)
        {
            return ObtenerGruposDeProyecto(pProyectoID, true);
        }

        /// <summary>
        /// Obtiene la tabla GrupoIdentidades de los grupos por el proyecto
        /// </summary>
        /// <param name="pProyecto">Proyecto del Grupo</param>
        /// <param name="pObtenerMiembros">Obtiene las tablas GrupoIdentidadesProyecto y GrupoIdentidadesParticipacion</param>
        /// <returns>DataSet con el grupo</returns>
        public DataWrapperIdentidad ObtenerGruposDeProyecto(Guid pProyectoID, bool pObtenerMiembros)
        {
            return IdentidadAD.ObtenerGruposDeProyecto(pProyectoID, pObtenerMiembros);
        }

        /// <summary>
        /// Obtiene los grupos por el id
        /// </summary>
        /// <param name="pGruposID">IDs del Grupo</param>
        /// <returns>DataSet con el grupo</returns>
        public DataWrapperIdentidad ObtenerGruposPorIDGrupo(List<Guid> pGruposID, bool pObtenerParticipantes = true)
        {
            return IdentidadAD.ObtenerGruposPorIDGrupo(pGruposID, pObtenerParticipantes);
        }

        /// <summary>
        /// Obtiene las identidades de MyGnoss de los miembros de los grupos
        /// </summary>
        /// <param name="pGruposID">IDs del Grupo</param>
        /// <returns>DataSet con el grupo</returns>
        public List<Guid> ObtenerIdentidadesDeMyGnossDeParticipantesDeGrupos(List<Guid> pGruposID)
        {
            return IdentidadAD.ObtenerIdentidadesDeMyGnossDeParticipantesDeGrupos(pGruposID);
        }

        /// <summary>
        /// Obtiene todas las identidades ("Identidad" y "Perfil") del perfil pasado por parámetro
        /// </summary>
        /// <param name="pPerfilID">Identificador de perfil</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadesDePerfil(Guid pPerfilID)
        {
            return ObtenerIdentidadesDePerfil(pPerfilID, false);
        }


        /// <summary>
        /// Obtiene la lista de identidades que pertenecen al perfil id pasado por parámetro
        /// </summary>
        /// <param name="pPerfilID">Identificador de perfil</param>
        /// <returns>Dataset de identidades</returns>
        public List<AD.EntityModel.Models.IdentidadDS.Identidad> ObtenerListaIdentidadesPorPerfilID(Guid pPerfilID)
        {
            return IdentidadAD.ObtenerListaIdentidadesPorPerfilID(pPerfilID);
        }

        /// <summary>
        /// Obtiene el id de los grupos de comunidad en los que participa la identidad
        /// </summary>
        /// <param name="pIdentidadID">Identidad que pertenece al grupo</param>
        /// <returns>DataSet con el grupo</returns>
        public List<Guid> ObtenerIDGruposDeIdentidad(Guid pIdentidadID)
        {

            return IdentidadAD.ObtenerIDGruposDeIdentidad(pIdentidadID);
        }

        /// <summary>
        /// Compruweba si los frupos existen
        /// </summary>
        /// <param name="pListaGrupos">Lista de grupos para comprobar</param>
        /// <returns>Lista de grupos que sio existen</returns>
        public List<Guid> ComprobarSiIDGruposExisten(List<Guid> pListaGrupos)
        {
            return IdentidadAD.ComprobarSiIDGruposExisten(pListaGrupos);
        }

        /// <summary>
        /// Obtiene a partir del identificador de perfil dado todos los posibles perfiles e identidades que tenga (eliminados o no)
        /// Carga tablas "Perfil" e "Identidad"
        /// </summary>
        /// <param name="pPerfilID">Identificador de perfil</param>
        /// <param name="pBajas">TRUE para obtener las identidades dadas de baja, FALSE en caso contrario</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadesDePerfil(Guid pPerfilID, bool pBajas)
        {
            if (pPerfilID.Equals(UsuarioAD.Invitado))
            {
                return ObtenerIdentidadInvitado();
            }
            else
            {
                return IdentidadAD.ObtenerIdentidadesDePerfil(pPerfilID, pBajas);
            }
        }

        /// <summary>
        /// Obtiene la identidades de los perfiles pasados como parámetro en un determinado proyecto.
        /// Carga tablas "Perfil" , "Identidad"
        /// </summary>
        /// <param name="pPerfilesID">Lista de identificadores de perfil</param>
        /// <param name="pProyectoID">Proyecto (NULL si solo se quiere obtener la tabla perfil)</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadDePerfilEnProyecto(Guid pPerfilID, Guid? pProyectoID)
        {
            List<Guid> listaPerfiles = new List<Guid>();
            listaPerfiles.Add(pPerfilID);
            return ObtenerIdentidadDePerfilEnProyecto(listaPerfiles, pProyectoID);
        }

        /// <summary>
        /// Obtiene la identidades de los perfiles pasados como parámetro en un determinado proyecto.
        /// Carga tablas "Perfil" , "Identidad"
        /// </summary>
        /// <param name="pPerfilesID">Lista de identificadores de perfil</param>
        /// <param name="pProyectoID">Proyecto (NULL si solo se quiere obtener la tabla perfil)</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadDePerfilEnProyecto(List<Guid> pPerfilesID, Guid? pProyectoID)
        {
            return IdentidadAD.ObtenerIdentidadDePerfilEnProyecto(pPerfilesID, pProyectoID);
        }

        /// <summary>
        /// Obtiene las identidades de los perfiles pasados como parámetro(eliminados o no)
        /// Carga tablas "Perfil" , "Identidad" , "PerfilPersonas" y "PerfilOrganizaciones"
        /// </summary>
        /// <param name="pPerfilesID">Lista de identificadores de perfil</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadesDePerfiles(List<Guid> pPerfilesID)
        {
            return IdentidadAD.ObtenerIdentidadesDePerfiles(pPerfilesID);
        }

        /// <summary>
        /// Obtiene las identidades de los perfiles pasados como parámetro en un determinado proyecto (eliminados o no)
        /// Carga tablas "Perfil" , "Identidad", "PerfilPersonas" y "PerfilOrganizaciones"
        /// </summary>
        /// <param name="pPerfilesID">Lista de identificadores de perfil</param>
        /// <param name="pProyectoID">Proyecto</param>
        /// <returns>Dataset de identidades</returns>
        public DataWrapperIdentidad ObtenerIdentidadesDePerfilesEnProyecto(List<Guid> pPerfilesID, Guid pProyectoID)
        {
            return IdentidadAD.ObtenerIdentidadesDePerfilesEnProyecto(pPerfilesID, pProyectoID);
        }

        /// <summary>
        /// Obtiene las identidades de los perfiles pasados como parámetro en un determinado proyecto.
        /// Carga Lista de IDs de identidades.
        /// </summary>
        /// <param name="pPerfilesID">Lista de identificadores de perfil</param>
        /// <param name="pProyectoID">Proyecto</param>
        /// <returns>Lista de identidades</returns>
        public List<Guid> ObtenerIdentidadesIDDePerfilesEnProyecto(List<Guid> pPerfilesID, Guid pProyectoID)
        {
            return IdentidadAD.ObtenerIdentidadesIDDePerfilesEnProyecto(pPerfilesID, pProyectoID);
        }

        /// <summary>
        /// Obtiene el identificador de la persona que publica un recurso que es votado 
        /// </summary>

        /// <param name="pIDVoto">ID del voto</param>
        /// <returns></returns>
        public Guid ObtenerIdentidadVotadorPorIDVoto(Guid pIDVoto)
        {
            return IdentidadAD.ObtenerIdentidadVotadorPorIDVoto(pIDVoto);

        }


        /// <summary>
        /// Obtiene el identificador de la persona que publica un recurso que es comentado 
        /// </summary>

        /// <param name="pIDVoto">ID del voto</param>
        /// <returns></returns>
        public Guid ObtenerIdentidadComentadorPorIDComentario(Guid pIDComentario)
        {
            return IdentidadAD.ObtenerIdentidadComentadorPorIDComentario(pIDComentario);

        }

        /// <summary>
        /// Actualiza identidades 
        /// </summary>
        /// <param name="pIdentidadesDS">Dataset de identidades para actualizar</param>
        public void ActualizaIdentidades()
        {
            base.Actualizar();
        }

        /// <summary>
        /// Comprueba si un usuario a participado con un perfil en una comunidad (Comprueba si hay una identidad con fecha de baja con ese perfil en un proyecto)
        /// </summary>
        /// <param name="pPerfilID">Clave del perfil</param>
        /// <param name="pProyectoID">Clave del proyecto</param>
        /// <returns>True si ha participado en el proyecto anteriormente con dicho perfil y abandono/expulsaron</returns>
        public bool HaParticipadoConPerfilEnComunidad(Guid pPerfilID, Guid pProyectoID)
        {
            return IdentidadAD.HaParticipadoConPerfilEnComunidad(pPerfilID, pProyectoID);
        }

        /// <summary>
        /// Comprueba si un usuario participa con un perfil en una comunidad 
        /// (Comprueba si hay una identidad sin fecha de baja con ese perfil en un proyecto)
        /// </summary>
        /// <param name="pPerfilID">Identificador de perfil</param>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <returns>TRUE si participa en el proyecto anteriormente con dicho perfil</returns>
        public bool ParticipaPerfilEnComunidad(Guid pPerfilID, Guid pProyectoID)
        {
            return IdentidadAD.ParticipaPerfilEnComunidad(pPerfilID, pProyectoID);
        }

        /// <summary>
        /// Comprueba si un usuario participa con una identidad en una comunidad
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la identidad</param>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <returns>TRUE si participa en el proyecto anteriormente con dicha identidad</returns>
        public bool ParticipaIdentidadEnComunidad(Guid pIdentidadID, Guid pProyectoID)
        {
            return IdentidadAD.ParticipaIdentidadEnComunidad(pIdentidadID, pProyectoID);
        }

        /// <summary>
        /// Comprueba si la persona participa en una comunidad 
        /// (Comprueba si hay una identidad sin fecha de baja de la persona en una comunidad )
        /// </summary>
        /// <param name="pPersonaID">Identificador de la Persona</param>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <returns>TRUE si participa en el proyecto con alguna identidad</returns>
        public bool ParticipaPersonaEnProyectoConAlgunaIdentidad(Guid pPersonaID, Guid pProyectoID)
        {
            return IdentidadAD.ParticipaPersonaEnProyectoConAlgunaIdentidad(pPersonaID, pProyectoID);
        }

        /// <summary>
        /// Comprueba para una identidad (a traves de perfil+proyecto) si dicha identidad tiene "FechaExpulsion"
        /// </summary>
        /// <param name="pPerfilID">Perfil de la identidad</param>
        /// <param name="pProyectoID">Proyecto de la identidad</param>
        /// <returns>True si hay Fecha de expulsion, False si no tiene O NO encontramos una identidad para ese perfil+proyecto</returns>
        public bool EstaIdentidadExpulsadaDeproyecto(Guid pPerfilID, Guid pProyectoID)
        {
            return IdentidadAD.EstaIdentidadExpulsadaDeproyecto(pPerfilID, pProyectoID);
        }

        /// <summary>
        /// Comprueba si una identidad está expulsada
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la identidad</param>
        /// <returns></returns>
        public bool EstaIdentidadExpulsada(Guid pIdentidadID)
        {
            return IdentidadAD.EstaIdentidadExpulsada(pIdentidadID);
        }

        /// <summary>
        /// Obtiene la popularidad de una identidad en un proyecto
        /// </summary>
        /// <param name="pIdentidadID">Identidad</param>
        /// <param name="pProyectoID">Proyecto</param>
        /// <returns>Número de recursos</returns>
        public double ObtenerPopularidadDeIdentidadEnProyecto(Guid pIdentidadID, Guid pProyectoID)
        {
            return IdentidadAD.ObtenerPopularidadDeIdentidadEnProyecto(pIdentidadID, pProyectoID);
        }


        /// <summary>
        /// Obtiene la popularidad maxima de las identidades en un proyecto
        /// </summary>        
        /// <param name="pProyectoID">Proyecto</param>
        /// <returns>Número de recursos</returns>
        public double ObtenerPopularidadMaxDeIdentidadEnProyecto(Guid pProyectoID)
        { return IdentidadAD.ObtenerPopularidadMaxDeIdentidadEnProyecto(pProyectoID); }

        /// <summary>
        /// Obtiene el número de recursos subidos por una identidad en un proyecto
        /// </summary>
        /// <param name="pIdentidadID">Identidad</param>
        /// <param name="pProyectoID">Proyecto</param>
        /// <returns>Número de recursos</returns>
        public int ObtenerNumRecursosDeIdentidadEnProyecto(Guid pIdentidadID, Guid pProyectoID)
        {
            return IdentidadAD.ObtenerNumRecursosDeIdentidadEnProyecto(pIdentidadID, pProyectoID);
        }

        /// <summary>
        /// Obtiene el número de recursos subidos por una identidad en un proyecto
        /// </summary>
        /// <param name="pIdentidadID">Identidad</param>
        /// <param name="pProyectoID">Proyecto</param>
        /// <returns>Número de recursos</returns>
        public int ObtenerNumDebatesDeIdentidadEnProyecto(Guid pIdentidadID, Guid pProyectoID)
        {
            return IdentidadAD.ObtenerNumDebatesDeIdentidadEnProyecto(pIdentidadID, pProyectoID);
        }

        /// <summary>
        /// Obtiene el número de recursos subidos por las identidades corporativas de una organizacion en un proyecto
        /// </summary>
        /// <param name="pOrganizacionID">Organizacion</param>
        /// <param name="pProyectoID">Proyecto</param>
        /// <returns>Número de recursos</returns>
        public int ObtenerNumRecursosDeIdentidadesDeOrgEnProyecto(Guid pOrganizacionID, Guid pProyectoID)
        {
            return IdentidadAD.ObtenerNumRecursosDeIdentidadesDeOrgEnProyecto(pOrganizacionID, pProyectoID);
        }

        /// <summary>
        /// Obtiene el número de recursos subidos por las identidades corporativas de una organizacion en un proyecto
        /// </summary>
        /// <param name="pOrganizacionID">Organizacion</param>
        /// <param name="pProyectoID">Proyecto</param>
        /// <returns>Número de recursos</returns>
        public int ObtenerNumDebatesDeIdentidadesDeOrgEnProyecto(Guid pOrganizacionID, Guid pProyectoID)
        {
            return IdentidadAD.ObtenerNumDebatesDeIdentidadesDeOrgEnProyecto(pOrganizacionID, pProyectoID);
        }

        /// <summary>
        /// Obtiene una apartir de una identidad "pIdentidadEnMygnoss" las identidades que otras organizaciones les han concedido permisos para ser sus contactos. Es decir quien de las organizaciones le han dado permiso para que "pIdentidadEnMygnoss" sea su contacto
        /// </summary>
        /// <param name="pIdentidadEnMygnoss">Clave de la identidad en MyGnoss</param>
        /// <returns>Lista con Guid</returns>
        public List<Guid> ObtenerListaIdentidadesPermitidasPorOrg(Guid pIdentidadEnMygnoss)
        {
            return IdentidadAD.ObtenerListaIdentidadesPermitidasPorOrg(pIdentidadEnMygnoss);
        }


        /// <summary>
        /// Obtiene todas las identidades en MyGNOSS de las personas que se corresponden con un nombre y apellidos, que son visibles por el usuario conectado
        /// </summary>
        /// <param name="pNombre">Nombre de la persona</param>
        /// <param name="pUsuarioID">Guid del usuario conectado</param>
        /// <param name="pIdentidadID">Guid de la identidad en MyGNOSS del usuario conectado</param>
        /// <returns>Data set de personas</returns>
        public DataWrapperIdentidad BuscarIdentidadesDePersonasVisiblesMyGNOSSPorNombre(string pNombre, Guid pUsuarioID, Guid pIdentidadID)
        {
            return IdentidadAD.BuscarIdentidadesDePersonasVisiblesMyGNOSSPorNombre(pNombre, pUsuarioID, pIdentidadID);
        }

        /// <summary>
        /// Actualiza la puntuación de las Identidades más activas en los últimos n días
        /// </summary>
        public void ActualizarRankingIdentidades(int pNumDias)
        {


            IdentidadAD.ActualizarRankingIdentidades(pNumDias);
        }


        public void ActualizarValorAbsoluto()
        {
            IdentidadAD.ActualizarValorAbsoluto();
        }

        /// <summary>
        /// Obtiene el ranking de las identidades de un determinado proyecto
        /// </summary>
        public Dictionary<Guid, double> ObtenerRankingIdentidades(Guid pProyecto)
        {
            return IdentidadAD.ObtenerRankingIdentidades(pProyecto);
        }

        /// <summary>
        /// Obtiene los IDs de los perfiles de unas determinadas identidades.
        /// </summary>
        /// <param name="pPerfilesID">Lista de IDs de perfiles</param>
        /// <returns>Lista con IDs de los perfiles de unas determinadas identidades</returns>
        public DataWrapperIdentidad ObtenerPerfilesPorPerfilesID(List<Guid> pPerfilesID)
        {
            return IdentidadAD.ObtenerPerfilesPorPerfilesID(pPerfilesID);
        }

        /// <summary>
        /// Carga el DataSet con la tabla "Perfil" de unos determinados usuario en un proyecto.
        /// </summary>
        /// <param name="pUsuarioIDs">Lista de IDs de usuarios</param>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <returns>DataSet con la tabla "Perfil" de unos determinados usuario en un proyecto</returns>
        public Dictionary<Guid, Guid> ObtenerPerfilesIDPorUsuariosIDEnProyecto(List<Guid> pUsuarioIDs, Guid pProyectoID)
        {
            return IdentidadAD.ObtenerPerfilesIDPorUsuariosIDEnProyecto(pUsuarioIDs, pProyectoID);
        }

        /// <summary>
        /// Obtiene los identificadores de identidad a partir de la lista de identificadores de usuarios
        /// </summary>
        /// <param name="pUsuarioIDs">Lista de usuarios de los que se quiere obtener el identificador de la identidad en el proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto en el que se quieren obtener las identidades de los usuarios</param>
        /// <returns>Lista de IDs de identidades en el proyecto</returns>
        public Dictionary<Guid, Guid> ObtenerIdentidadesIDPorUsuariosIDEnProyecto(List<Guid> pUsuarioIDs, Guid pProyectoID)
        {
            return IdentidadAD.ObtenerIdentidadesIDPorUsuariosIDEnProyecto(pUsuarioIDs, pProyectoID);
        }

        /// <summary>
        /// Obtiene los IDs de los perfiles de unas determinadas identidades.
        /// </summary>
        /// <param name="pIdentidadesID">Lista de IDs de identidades</param>
        /// <returns>Lista con IDs de los perfiles de unas determinadas identidades</returns>
        public List<Guid> ObtenerPerfilesDeIdentidades(List<Guid> pIdentidadesID)
        {
            return IdentidadAD.ObtenerPerfilesDeIdentidades(pIdentidadesID);
        }

        /// <summary>
        /// Actualiza la puntuación de las Identidades más activas en los últimos n días
        /// </summary>
        public void ActualizarRankingIdentidadesOrgdeIdentidad(Guid IdentidadID)
        {
            IdentidadAD.ActualizarRankingIdentidadesOrgdeIdentidad(IdentidadID);
        }
        /// <summary>
        /// Actualiza la puntuación de las Identidades por la noche
        /// </summary>
        public List<string> ActualizarIdentidadesNoche()
        {
            return IdentidadAD.ActualizarIdentidadesNoche();
        }

        /// <summary>
        /// Actualiza la puntuación de las Identidades en el momento
        /// </summary>
        public void ActualizarPopularidadIdentidades(Guid pIDIdentidad, double pNuevaAportacion)
        { IdentidadAD.ActualizarPopularidadIdentidades(pIDIdentidad, pNuevaAportacion); }


        /// <summary>
        /// Obtiene un array cuyo primer elemento es el perfil de la identidad dada y la segunda componente si existe es la identidad de la organizacion
        /// </summary>
        /// <param name="pIdentidadID">Identidad que busca</param>        
        /// <returns>array cuyo primer elemento es el perfil de la identidad dada y la segunda componente si existe es la identidad de la organizacion</returns>
        public List<Guid> ObtenerPerfilyOrganizacionID(Guid pIdentidadID)
        {
            return IdentidadAD.ObtenerPerfilyOrganizacionID(pIdentidadID);
        }

        /// <summary>
        /// Obtiene un diccionario con el perfil de la identidad dada, el id de la organización y el nombre corto del perfil (el de la organización o el del usuario, según corresponda)
        /// </summary>
        /// <param name="pIdentidadID">Identidad que busca</param>        
        /// <param name="pObtenerNombreCortoPerfil">Verdad si se debe obtener el nombre corto del perfil</param>
        /// <returns>Obtiene un diccionario con las claves "PerfilID", "OrganizacionID", "NombreCorto"</returns>
        public Dictionary<string, object> ObtenerPerfilyOrganizacionIDyNombreCortoPerfil(Guid pIdentidadID, bool pObtenerNombreCortoPerfil)
        {
            return IdentidadAD.ObtenerPerfilyOrganizacionIDyNombreCortoPerfil(pIdentidadID, pObtenerNombreCortoPerfil);
        }

        /// <summary>
        /// Obtiene el identificador del perfil del usuario que ha publicado un recurso en una comunidad
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns></returns>
        public Guid? ObtenerPerfilIDPublicadorRecursoEnProyecto(Guid pDocumentoID, Guid pProyectoID)
        {
            return IdentidadAD.ObtenerPerfilIDPublicadorRecursoEnProyecto(pDocumentoID, pProyectoID);
        }

        /// <summary>
        /// Obtiene el identificador del perfil del usuario que ha publicado un comentario en un recurso
        /// </summary>
        /// <param name="pComentarioID">Identificador del comentario</param>
        /// <returns></returns>
        public Guid? ObtenerPerfilIDPublicadorComentarioEnRecurso(Guid pComentarioID)
        {
            return IdentidadAD.ObtenerPerfilIDPublicadorComentarioEnRecurso(pComentarioID);
        }

        /// <summary>
        /// Obtiene el identificador del perfil del usuario que ha votado un recurso
        /// </summary>
        /// <param name="pVotoID">Identificador del voto</param>
        /// <returns></returns>
        public Guid? ObtenerPerfilIDPublicadorVotoEnRecurso(Guid pVotoID)
        {
            return IdentidadAD.ObtenerPerfilIDPublicadorComentarioEnRecurso(pVotoID);
        }

        /// <summary>
        /// Obtiene el UsuarioID a partir de la identidadID
        /// </summary>
        /// <param name="pIdentidadID">Identidad de la que se quiere obtener el UsuarioID</param>
        /// <returns>ID del usuario</returns>
        public Guid ObtenerUsuarioIDConIdentidadID(Guid pIdentidadID)
        {
            return IdentidadAD.ObtenerUsuarioIDConIdentidadID(pIdentidadID);
        }

        /// <summary>
        /// Obtiene una lista de IDs de usuario a partir de una con IDs de perfiles.
        /// </summary>
        /// <param name="pPerfilesID">IDs de perfiles</param>
        /// <returns>Diccionario con ID de perfil y ID de usuario</returns>
        public Dictionary<Guid, Guid> ObtenerUsuariosIDPorPerfilID(List<Guid> pPerfilesID)
        {
            return IdentidadAD.ObtenerUsuariosIDPorPerfilID(pPerfilesID);
        }

        /// <summary>
        /// Obtiene los UsuarioID a partir de la lista ed identidadesID
        /// </summary>
        /// <param name="pListaIdentidadesID">Lista de identidades de las que se quiere obtener el UsuarioID</param>
        /// <returns>Lista de IDs de usuario</returns>
        public Dictionary<Guid, Guid> ObtenerListaUsuarioIDConIdentidadesID(List<Guid> pListaIdentidadesID)
        {
            return IdentidadAD.ObtenerListaUsuarioIDConIdentidadesID(pListaIdentidadesID);
        }

        /// <summary>
        /// Obtiene si el perfil esta o no eliminado
        /// </summary>
        /// <param name="pPerfilID">Perfil id a consultar</param>
        /// <returns>True o false si esta o no eliminado respectivamente</returns>
        public bool EstaPerfilEliminado(Guid pPerfilID)
        {
            return IdentidadAD.EstaPerfilEliminado(pPerfilID);
        }

        /// <summary>
        /// Obtiene la organización ID a partir de la identidadID
        /// </summary>
        /// <param name="pIdentidadID">Identidad de la que se quiere obtener la OrganizacionID</param>
        /// <returns>ID de la organización</returns>
        public Guid? ObtenerOrganizacionIDConIdentidadID(Guid pIdentidadID)
        {
            return IdentidadAD.ObtenerOrganizacionIDConIdentidadID(pIdentidadID);
        }

        /// <summary>
        /// Obtiene la organización ID a partir del perfilID
        /// </summary>
        /// <param name="pPerfilID">Perfil de la que se quiere obtener la OrganizacionID</param>
        /// <returns>ID de la organización</returns>
        public Guid? ObtenerOrganizacionIDConPerfilID(Guid pPerfilID)
        {
            return IdentidadAD.ObtenerOrganizacionIDConPerfilID(pPerfilID);
        }

        /// <summary>
        /// Obtiene el perfil personal a partir de un perfil.
        /// </summary>
        /// <param name="pPerfilID">ID de perfil</param>
        /// <returns>ID del perfil personal a partir de un perfil</returns>
        public Guid ObtenerPerfilPersonalDePerfil(Guid pPerfilID)
        {
            return IdentidadAD.ObtenerPerfilPersonalDePerfil(pPerfilID);
        }

        public DataWrapperIdentidad ObtenerPerfilesValidosGnoss(DateTime? pFecha = null)
        {
            return IdentidadAD.ObtenerPerfilesValidosGnoss(pFecha);
        }

        public int ObtenerNumRecursosEnEspacioPersonalPorPerfil(Guid pPerfilID)
        {
            return IdentidadAD.ObtenerNumRecursosEnEspacioPersonalPorPerfil(pPerfilID);
        }

        public int ObtenerNumRecursosEnComunidadesPorPerfil(Guid pPerfilID)
        {
            return IdentidadAD.ObtenerNumRecursosEnComunidadesPorPerfil(pPerfilID);
        }

        public DataWrapperDatoExtra ObtenerIdentidadDatoExtraRegistroDeProyecto(Guid pProyectoID, Guid pIdentidadID)
        {
            return IdentidadAD.ObtenerIdentidadDatoExtraRegistroDeProyecto(pProyectoID, pIdentidadID);
        }

		public List<Rol> ObtenerRolesDeIdentidad(Guid pIdentidadID)
		{
			return IdentidadAD.ObtenerRolesDeIdentidad(pIdentidadID);
		}

        /// <summary>
        /// Asigna un nuevo rol con id pRolId a la identidad pIdentidadID
        /// </summary>
        /// <param name="pIdentidadID">ID de la identidad a la que se va a asignar el rol</param>
        /// <param name="pRolId">ID del rol a asignar</param>
        public void AsignarRolAIdentidad(Guid pIdentidadID, Guid pRolId)
        {
            IdentidadAD.AsignarRolAIdentidad(pIdentidadID,pRolId);
        }

        /// <summary>
        /// Elimina el rol con id pRolId a la identidad pIdentidadID
        /// </summary>
        /// <param name="pIdentidadID">ID de la identidad a la que se va a asignar el rol</param>
        /// <param name="pRolId">ID del rol a asignar</param>
        public void EliminarRolAIdentidad(Guid pIdentidadID, Guid pRolId)
        {
            IdentidadAD.EliminarRolAIdentidad(pIdentidadID, pRolId);
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Determina si está disposed
        /// </summary>
        private bool mDisposed = false;

        /// <summary>
        /// Destructor
        /// </summary>
        ~IdentidadCN()
        {
            //Libero los recursos
            Dispose(false);
        }

        /// <summary>
        /// Libera los recursos
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            //impido que se finalice dos veces este objeto
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Libera los recursos
        /// </summary>
        /// <param name="disposing">Determina si se está llamando desde el Dispose()</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!mDisposed)
            {
                mDisposed = true;

                if (disposing)
                {
                    //Libero todos los recursos administrados que he añadido a esta clase
                    if (IdentidadAD != null)
                    {
                        IdentidadAD.Dispose();
                    }
                }
                IdentidadAD = null;
            }
        }

       

        #endregion

        #region Propiedades

        private IdentidadAD IdentidadAD
        {
            get
            {
                return (IdentidadAD)AD;
            }
            set
            {
                this.AD = value;
            }
        }

        #endregion
    }
}
