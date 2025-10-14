using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.PersonaDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.UsuarioDS;
using Es.Riam.Gnoss.AD.Identidad;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.Usuarios;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Es.Riam.Gnoss.AD.ServiciosGenerales
{

    public class JoinPersonaUsuario
    {
        public AD.EntityModel.Models.PersonaDS.Persona Persona { get; set; }
        public Usuario Usuario { get; set; }
    }

    public static class JoinsPersona
    {
        public static IQueryable<JoinPersonaUsuario> JoinUsuario(this IQueryable<Persona> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Usuario, persona => persona.UsuarioID, usuario => usuario.UsuarioID, (persona, usuario) => new JoinPersonaUsuario
            {
                Persona = persona,
                Usuario = usuario

            });
        }
    }
    #region Enumeraciones

    /// <summary>
    /// Enumeración para distinguir tipos de documentos acreditativos
    /// </summary>
    public enum TipoDocumentoAcreditativo
    {
        /// <summary>
        /// Ninguno
        /// </summary>
        NULL = -1,
        /// <summary>
        /// DNI
        /// </summary>
        DNI = 0,
        /// <summary>
        /// Pasaporte
        /// </summary>
        Pasaporte = 1,
        /// <summary>
        /// Tarjeta de residencia
        /// </summary>
        TarjetaResidencia = 2
    }

    /// <summary>
    /// Enumeración para el estado de la corrección de la identidad 
    /// </summary>
    public enum EstadoCorreccion
    {
        /// <summary>
        /// No tiene correccion
        /// </summary>
        NoCorreccion = 0,
        /// <summary>
        /// Corrección notificada pero sin cambios
        /// </summary>
        NotificadoNoCambiado = 1,
        /// <summary>
        /// Corrección notificada y cambiada
        /// </summary>
        NotificadoCambiado = 2,
        /// <summary>
        /// Corrección notificada y sin cambios en 3 días
        /// </summary>
        NotificadoNoCambiado3Dias = 3
    }

    #endregion

    /// <summary>
    /// Data adapter de persona
    /// </summary>
    public class PersonaAD : BaseAD
    {

        #region Miembros

        /// <summary>
        /// Indica si se deben actualizar los datos desnormalizados de persona
        /// </summary>
        private bool mActualizarDatosDesnormalizados = true;

        /// <summary>
        /// Indica si se debe omitir el guardado de el nombre y apellidos de una persona para no activar un trigger.
        /// </summary>
        public static bool NoGuardarNombreNiApellidosPersona = false;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        #endregion

        #region Constantes

        /// <summary>
        /// Ruta de las imágenes del perfil de persona
        /// </summary>
        public const string SIN_IMAGENES_PERSONA = "sinfoto";

        #endregion


        #region Constructores

        /// <summary>
        /// Constructor por defecto, sin parámetros, utilizado cuando se requiere el GnossConfig.xml por defecto
        /// </summary>
        public PersonaAD(LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<PersonaAD> logger, ILoggerFactory loggerFactory)
            : base(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication,logger, loggerFactory)
        {
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        /// <summary>
        /// Constructor a partir del fichero de configuración de conexión a la base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuración de base de datos</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public PersonaAD(string pFicheroConfiguracionBD, LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<PersonaAD> logger, ILoggerFactory loggerFactory)
            : base(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        {
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        #endregion

        #region Métodos generales

        #region Públicos

        /// <summary>
        /// Obtiene el usuarioID y PerfilID asociados a un correo a partir del nombre corto de la organización
        /// </summary>
        /// <param name="pNombreCortoOrg">Nombre corto de la organización</param>
        /// <param name="pListaCorreos">Lista con uno o varios correos de los que se desea obtener usuario y perfil</param>
        /// <returns>Diccionario con los usuarios y perfiles asociados a los correos facilitados</returns>
        public Dictionary<string, List<Guid>> ObtenerUsuarioIDPerfilIDPorEmailYOrganizacion(string pNombreCortoOrg, string[] pListaCorreos)
        {
            Dictionary<string, List<Guid>> dicUsuarioIDPerfilIDPorEmail = new Dictionary<string, List<Guid>>();

            var resultado = mEntityContext.Perfil.Join(mEntityContext.Persona, perfil => perfil.PersonaID, persona => persona.PersonaID, (perfil, persona) => new { Perfil = perfil, Persona = persona }).Join(mEntityContext.Organizacion, perfilPersona => perfilPersona.Perfil.OrganizacionID, organizacion => organizacion.OrganizacionID, (perfilPersona, organizacion) => new { Perfil = perfilPersona.Perfil, Persona = perfilPersona.Persona, Organizacion = organizacion }).Join(mEntityContext.Identidad, perfilPersonaOrg => perfilPersonaOrg.Perfil.PerfilID, identidad => identidad.PerfilID, (perfilPersonaOrg, identidad) => new { Perfil = perfilPersonaOrg.Perfil, Persona = perfilPersonaOrg.Persona, Organizacion = perfilPersonaOrg.Organizacion, Identidad = identidad }).Where(item => item.Organizacion.NombreCorto.Equals(pNombreCortoOrg) && !item.Perfil.Eliminado && !item.Persona.Eliminado && pListaCorreos.Contains(item.Persona.Email)).Select(item => new { Email = item.Persona.Email, UsuarioID = item.Persona.UsuarioID, PerfilID = item.Identidad.PerfilID }).Distinct().ToList();

            foreach (var item in resultado)
            {
                dicUsuarioIDPerfilIDPorEmail.Add(item.Email, new List<Guid>() { item.UsuarioID.Value, item.PerfilID });
            }

            return dicUsuarioIDPerfilIDPorEmail;
        }

        /// <summary>
        /// Obtiene el correo de la persona que se le pasa por parámetro en la organización indicada (o en modo personal si se le pasa NULL)
        /// </summary>
        /// <param name="pPersonaID">Identificador de persona</param>
        /// <param name="pOrganizacionID">Identificador de organización</param>
        /// <returns>Dirección de correo</returns>
        public string ObtenerCorreoDePersonaID(Guid pPersonaID, Guid? pOrganizacionID)
        {
            string correo = "";
            if (pOrganizacionID.HasValue)
            {
                correo = mEntityContext.PersonaVinculoOrganizacion.Where(personaVinculoOrganizacion => personaVinculoOrganizacion.PersonaID.Equals(pPersonaID) && personaVinculoOrganizacion.OrganizacionID.Equals(pOrganizacionID.Value)).Select(email => email.EmailTrabajo).ToList().FirstOrDefault();
            }
            else
            {
                correo = mEntityContext.Persona.Where(personaVinculoOrganizacion => personaVinculoOrganizacion.PersonaID.Equals(pPersonaID)).Select(email => email.Email).FirstOrDefault();
            }

            return correo;
        }

        /// <summary>
        /// Obtiene el usuarioID de la persona que se le pasa por parámetro
        /// </summary>
        /// <param name="pPersonaID">Identificador de persona</param>
        /// <returns>Guid</returns>
        public Guid? ObtenerUsuarioIDDePersonaID(Guid pPersonaID)
        {
            return mEntityContext.Persona.Where(persona => persona.PersonaID.Equals(pPersonaID)).Select(item => item.UsuarioID).ToList().FirstOrDefault();
        }

        /// <summary>
        /// Obtiene los usuarioID de la lista de perfiles que se le pasa por parámetro
        /// </summary>
        /// <param name="pListaPerfilID">Lista de perfiles</param>
        /// <returns>Lista de usuariosID</returns>
        public List<Guid> ObtenerUsuariosIDDeListaPerfil(List<Guid> pListaPerfilID)
        {
            List<Guid> listaGuidUsuariosID = mEntityContext.Persona.Join(mEntityContext.Perfil, persona => persona.PersonaID, perfil => perfil.PersonaID, (persona, perfil) => new { Perfil = perfil, Persona = persona }).Where(perfil => pListaPerfilID.Contains(perfil.Perfil.PerfilID)).Select(item => item.Persona.UsuarioID.Value).ToList();

            return listaGuidUsuariosID;
        }

        /// <summary>
        /// Obtiene el idioma de la persona que se le pasa por parámetro
        /// </summary>
        /// <param name="pPersonaID">Identificador de persona</param>
        /// <returns>Idioma</returns>
        public string ObtenerIdiomaDePersonaID(Guid pPersonaID)
        {
            return mEntityContext.Persona.Where(persona => persona.PersonaID.Equals(pPersonaID)).Select(item => item.Idioma).ToList().FirstOrDefault();
        }

        /// <summary>
        /// Comprueba si el email ya existe en la tabla de personas y la persona no está eliminada
        /// </summary>
        /// <param name="pEmail">Email que se quiere registrar</param>
        /// <returns>TRUE si ya existe, FALSE en caso contrario</returns>
        public bool ExisteEmail(string pEmail)
        {
            return mEntityContext.Persona.Where(persona => persona.Email.Equals(pEmail.ToLower()) && !persona.Eliminado).Select(item => item.Email).Concat(mEntityContext.SolicitudNuevoUsuario.Join(mEntityContext.Solicitud, solNuevoUsuario => solNuevoUsuario.SolicitudID, solicitud => solicitud.SolicitudID, (solNuevoUsuario, solicitud) => new { SolicitudNuevoUsuario = solNuevoUsuario, Solicitud = solicitud }).Where(item => item.SolicitudNuevoUsuario.Email.ToUpper().Equals(pEmail.ToUpper()) && item.Solicitud.Estado.Equals((short)EstadoSolicitud.Espera)).Select(item => item.SolicitudNuevoUsuario.Email)).Any();
        }

        /// <summary>
        /// Comprueba si el email ya existe en la tabla de personas excluyendo la propia solicitud
        /// </summary>
        /// <param name="pEmail">Email que se quiere registrar</param>
        /// <param name="pSolicitudID">Identificador de la solicitud que se excluirá de la búsqueda</param>
        /// <returns>TRUE si ya existe, FALSE en caso contrario</returns>
        public bool ExisteEmailExceptoEnSolicitud(string pEmail, Guid pSolicitudID)
        {
            return mEntityContext.Persona.Where(persona => persona.Email.Equals(pEmail.ToLower())).Select(item => item.Email).Concat(mEntityContext.SolicitudNuevoUsuario.Join(mEntityContext.Solicitud, solNuevoUsuario => solNuevoUsuario.SolicitudID, solicitud => solicitud.SolicitudID, (solNuevoUsuario, solicitud) => new { SolicitudNuevoUsuario = solNuevoUsuario, Solicitud = solicitud }).Where(item => item.SolicitudNuevoUsuario.Email.ToUpper().Equals(pEmail.ToUpper()) && item.Solicitud.Estado.Equals((short)EstadoSolicitud.Espera) && !item.SolicitudNuevoUsuario.SolicitudID.Equals(pSolicitudID)).Select(item => item.SolicitudNuevoUsuario.Email)).Any();
        }

        public void ModificarCorreoPersona(Guid pPersonaID, string pCorreoNuevo)
        {
            var resultado = mEntityContext.Persona.Where(persona => persona.PersonaID.Equals(pPersonaID)).FirstOrDefault();
            if (resultado != null)
            {
                resultado.Email = pCorreoNuevo.ToLower();
                mEntityContext.SaveChanges();
            }
        }

        /// <summary>
        /// Comprueba si el email ya existe en la tabla de personas
        /// </summary>
        /// <param name="pEmail">Email que se quiere registrar</param>
        /// <param name="pPersonaID">Identificador de la persona que se comprueba</param>
        /// <returns>TRUE si ya existe, FALSE en caso contrario</returns>
        public bool ExisteEmail(string pEmail, Guid pPersonaID)
        {
            return mEntityContext.Persona.Any(persona => persona.Email.Equals(pEmail.ToLower()) && !persona.PersonaID.Equals(pPersonaID) && !persona.Eliminado);
        }

        /// <summary>
        /// Comprueba si los correos de la lista ya existen
        /// </summary>
        /// <param name="pEmail">Lista de Emails que se quieren comprobar</param>
        /// <returns>Lista con los correos que ya existen</returns>
        public List<string> EmailYaPerteneceAPersona(string[] pListaCorreos)
        {
            return mEntityContext.Persona.Select(item => item.Email).Concat(mEntityContext.SolicitudNuevoUsuario.Join(mEntityContext.Solicitud, solNuevoUsuario => solNuevoUsuario.SolicitudID, solicitud => solicitud.SolicitudID, (solNuevoUsuario, solicitud) => new { SolicitudNuevoUsuario = solNuevoUsuario, Solicitud = solicitud }).Where(item => item.Solicitud.Estado.Equals((short)EstadoSolicitud.Espera) && pListaCorreos.Contains(item.SolicitudNuevoUsuario.Email)).Select(item => item.SolicitudNuevoUsuario.Email)).ToList();
        }

        /// <summary>
        /// Comprueba si un email pertenece a un usuario
        /// </summary>
        /// <param name="pEmail">Email que se quiere registrar</param>
        /// <param name="pUsuarioID">Identificador del usuario que se comprueba</param>
        /// <returns>TRUE si ya existe, FALSE en caso contrario</returns>
        public bool ComprobarEmailUsuario(string pEmail, Guid pUsuarioID)
        {
            return mEntityContext.Persona.Where(persona => persona.Email.Equals(pEmail.ToLower()) && persona.UsuarioID.Value.Equals(pUsuarioID)).Select(item => item.Email).Concat(mEntityContext.Usuario.Join(mEntityContext.Persona, usuario => usuario.UsuarioID, persona => persona.UsuarioID, (usuario, persona) => new { Usuario = usuario, Persona = persona }).Join(mEntityContext.PersonaVinculoOrganizacion, usuarioPersona => usuarioPersona.Persona.PersonaID, persVincOrg => persVincOrg.PersonaID, (usuarioPersona, persVincOrg) => new { Usuario = usuarioPersona.Usuario, Persona = usuarioPersona.Persona, PersonaVinculoOrganizacion = persVincOrg }).Where(item => item.PersonaVinculoOrganizacion.EmailTrabajo.ToUpper().Equals(pEmail.ToUpper())).Select(item => item.Persona.Email)).Any();
        }

        /// <summary>
        /// Comprueba si el email ya existe en la tabla de perfiles personas libres
        /// </summary>
        /// <param name="pEmail">Email que se quiere registrar</param>
        /// <param name="pPersonaID">Identificador de la persona que se comprueba</param>
        /// <returns>TRUE si ya existe, FALSE en caso contrario</returns>
        public bool ExisteEmailPerfilPersonaLibre(string pEmail, Guid pPersonaID)
        {
            return mEntityContext.DatosTrabajoPersonaLibre.Any(item => item.EmailTrabajo.ToUpper().Equals(pEmail.ToUpper()) && !item.PersonaID.Equals(pPersonaID));
        }

        /// <summary>
        /// Obtiene el identificador de la persona a partir del identificador del usuario
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <returns>Identificadro de la persona (null si no existe la persona)</returns>
        public Guid? ObtenerPersonaIDPorUsuarioID(Guid pUsuarioID)
        {
            return mEntityContext.Persona.Where(persona => persona.UsuarioID.Value.Equals(pUsuarioID)).Select(item => item.PersonaID).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene una fila de persona a partir de su usuario
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <returns>Dataset de personas</returns>
        public DataWrapperPersona ObtenerPersonaPorUsuario(Guid pUsuarioID)
        {
            return ObtenerPersonaPorUsuario(pUsuarioID, false, false);
        }

        /// <summary>
        /// Obtiene una fila de persona a partir de un usuario pasado por parámetro
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <param name="pObtenerTags">TRUE si se deben obtener los tags de persona, FALSE en caso contrario</param>
        /// <param name="pObtenerDatosTrabajoPersonaLibre">TRUE si se deben obtener los datos de persona libre, FALSE en otro caso</param>
        /// <returns>Dataset de personas</returns>
        public DataWrapperPersona ObtenerPersonaPorUsuario(Guid pUsuarioID, bool pObtenerTags, bool pObtenerDatosTrabajoPersonaLibre)
        {
            DataWrapperPersona dataWrapperPersona = new DataWrapperPersona();

            dataWrapperPersona.ListaPersona = mEntityContext.Persona.Where(persona => persona.UsuarioID.Value.Equals(pUsuarioID)).ToList();

            if (pObtenerDatosTrabajoPersonaLibre)
            {
                dataWrapperPersona.ListaDatosTrabajoPersonaLibre = mEntityContext.DatosTrabajoPersonaLibre.Join(mEntityContext.Persona, datosTrabajoPersonaLibre => datosTrabajoPersonaLibre.PersonaID, persona => persona.PersonaID, (datosTrabajoPersonaLibre, persona) => new { DatosTrabajoPersonaLibre = datosTrabajoPersonaLibre, Persona = persona }).Where(item => item.Persona.UsuarioID.Value.Equals(pUsuarioID)).Select(item => item.DatosTrabajoPersonaLibre).ToList();
            }
            return dataWrapperPersona;
        }

        /// <summary>
        /// Obtiene las personas de unos usuarios
        /// </summary>
        /// <param name="pListaUsuariosID">Lista de identificadores de usuario</param>
        /// <returns>PersonaDS</returns>
        public List<Persona> ObtenerPersonasDeUsuarios(List<Guid> pListaUsuariosID)
        {
            if (pListaUsuariosID.Count > 0)
            {
                return mEntityContext.Persona.Where(item => item.UsuarioID.HasValue && pListaUsuariosID.Contains(item.UsuarioID.Value)).ToList();
            }
            return new List<Persona>();
        }

        /// <summary>
        /// Obtiene el nombre de las personas de unos usuarios.
        /// </summary>
        /// <param name="pListaUsuariosID">Lista de identificadores de usuario</param>
        /// <returns>Lista con el ID del usuario y el nombre de la persona</returns>
        public Dictionary<Guid, string> ObtenerNombresPersonasDeUsuariosID(List<Guid> pListaUsuariosID)
        {
            Dictionary<Guid, string> nombresUsuarios = new Dictionary<Guid, string>();
            if (pListaUsuariosID.Count > 0)
            {
                var listaPersonas = mEntityContext.Persona.Where(persona => persona.UsuarioID.HasValue && pListaUsuariosID.Contains(persona.UsuarioID.Value)).Select(item => new { item.UsuarioID, item.Nombre, item.Apellidos }).ToList();

                foreach (var item in listaPersonas)
                {
                    Guid usuarioID = item.UsuarioID.Value;
                    string nombre = item.Nombre;
                    string apellidos = item.Apellidos;

                    if (!nombresUsuarios.ContainsKey(usuarioID))
                    {
                        nombresUsuarios.Add(usuarioID, nombre + " " + apellidos);
                    }
                }
            }
            return nombresUsuarios;
        }

        /// <summary>
        /// Obtiene el nombre de las personas de unos usuarios.
        /// </summary>
        /// <param name="pListaUsuariosID">Lista de identificadores de usuario</param>
        /// <returns>Lista con el nombrecorto del usuario y el nombre de la persona</returns>
        public Dictionary<string, string> ObtenerNombreCortoYNombresPersonasDeUsuariosID(List<Guid> pListaUsuariosID)
        {
            Dictionary<string, string> nombresUsuarios = new Dictionary<string, string>();
            if (pListaUsuariosID.Count > 0)
            {
                var listaPersonas = mEntityContext.Persona.JoinUsuario().Where(item => item.Persona.UsuarioID.HasValue && pListaUsuariosID.Contains(item.Persona.UsuarioID.Value)).Select(item => new { item.Usuario.NombreCorto, item.Persona.Nombre, item.Persona.Apellidos }).ToList();

                foreach (var item in listaPersonas)
                {
                    string nombreCorto = item.NombreCorto;
                    string nombre = item.Nombre;
                    string apellidos = item.Apellidos;

                    if (!nombresUsuarios.ContainsKey(nombreCorto))
                    {
                        nombresUsuarios.Add(nombreCorto, nombre + " " + apellidos);
                    }
                }
            }
            return nombresUsuarios;
        }

        /// <summary>
        /// Obtiene el nombre de las personas de unos usuarios.
        /// </summary>
        /// <param name="pListaUsuariosID">Lista de identificadores de usuario</param>
        /// <returns>Lista con el nombrecorto del usuario y el nombre de la persona</returns>
        public Dictionary<string, string[]> ObtenerNombreCortoYNombrePerfilPorUsuariosID(List<Guid> pListaUsuariosID)
        {
            Dictionary<string, string[]> nombresUsuarios = new Dictionary<string, string[]>();
            if (pListaUsuariosID.Count > 0)
            {
                var listaPersonas = mEntityContext.Persona.JoinPerfil().Where(item => item.Persona.UsuarioID.HasValue && pListaUsuariosID.Contains(item.Persona.UsuarioID.Value)).Select(item => new { item.Persona.UsuarioID, item.Perfil.NombreCortoUsu, item.Perfil.NombrePerfil }).ToList();

                foreach (var item in listaPersonas)
                {
                    string usuarioID = item.UsuarioID.ToString();
                    string nombreCorto = item.NombreCortoUsu;
                    string nombrePerfil = item.NombrePerfil;
                    string[] datos = new string[] { nombreCorto, nombrePerfil };

                    if (!nombresUsuarios.ContainsKey(usuarioID))
                    {
                        nombresUsuarios.Add(usuarioID, datos);
                    }
                }
            }
            return nombresUsuarios;
        }

        /// <summary>
        /// Obtiene la fila de persona a partir de un usuario pasado por parámetro
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <returns>ID de persona</returns>
        public Persona ObtenerFilaPersonaPorUsuario(Guid pUsuarioID)
        {
            return mEntityContext.Persona.Where(persona => persona.UsuarioID.Value.Equals(pUsuarioID)).ToList().FirstOrDefault();
        }

        /// <summary>
        /// Obtenemos todas las personas con notificaciones de cambio de identidad
        /// </summary>
        /// <returns>Dataset de personas</returns>
        public List<Persona> ObtenerPersonasConNotificacionDeCorreccionDeIdentidad()
        {
            return mEntityContext.Persona.Where(persona => persona.EstadoCorreccion != 0).ToList();
        }

        /// <summary>
        /// Obtiene una fila con los datos de persona libre
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <returns>Dataset de personas</returns>
        public DatosTrabajoPersonaLibre ObtenerDatosTrabajoPersonaLibre(Guid pUsuarioID)
        {
            return mEntityContext.DatosTrabajoPersonaLibre.Join(mEntityContext.Persona, datosTrabPersLibre => datosTrabPersLibre.PersonaID, persona => persona.PersonaID, (datosTrabPersLibre, persona) => new { DatosTrabajoPersonaLibre = datosTrabPersLibre, Persona = persona }).Where(item => item.Persona.UsuarioID.Value.Equals(pUsuarioID)).Select(item => item.DatosTrabajoPersonaLibre).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene el email personal de un usuario
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <returns>email de persona</returns>
        public string ObtenerEmailPersonalPorUsuario(Guid pUsuarioID)
        {
            string emailPersonal = mEntityContext.Persona.Where(persona => persona.UsuarioID.Value.Equals(pUsuarioID)).Select(item => item.Email).FirstOrDefault();

            return emailPersonal;
        }

        /// <summary>
        /// Obtiene todas las personas que se corresponden con un nombre
        /// </summary>
        /// <param name="pNombre">Nombre de la persona</param>
        /// <param name="pApellidos">Apellidos de la persona</param>
        /// <param name="pBuscarPersonasSinUsuario">TRUE si se deben buscar personas sin usuario, FALSE en caso contrario</param>
        /// <returns>Dataset de personas</returns>
        public List<Persona> ObtenerPersonasPorNombre(string pNombre, string pApellidos, bool pBuscarPersonasSinUsuario)
        {
            if (pBuscarPersonasSinUsuario)
            {
                return mEntityContext.Persona.Where(persona => persona.Nombre.Contains(pNombre) && persona.Apellidos.Contains(pApellidos) && !persona.UsuarioID.HasValue).ToList();
            }
            else
            {
                return mEntityContext.Persona.Where(persona => persona.Nombre.Contains(pNombre) && persona.Apellidos.Contains(pApellidos)).ToList();
            }
        }

        /// <summary>
        /// Obtiene un GUID de persona dado un email
        /// </summary>
        /// <param name="pEmail">Email de la persona a buscar</param>
        /// <returns>GUID de la persona a la que corresponde el email</returns>
        public Guid ObtenerPersonaPorEmail(string pEmail)
        {
            return mEntityContext.Persona.Where(persona => persona.Email.Equals(pEmail.ToLower()) && !persona.Eliminado).Select(item => item.PersonaID).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene las personas que participan en una organización. "Persona"
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <returns>Dataset de personas con las personas que participan en la organización</returns>
        public DataWrapperPersona ObtenerPersonasDeOrganizacionCargaLigera(Guid pOrganizacionID)
        {
            DataWrapperPersona dataWrapperPersona = new DataWrapperPersona();
            dataWrapperPersona.ListaPersona = mEntityContext.Persona.Join(mEntityContext.PersonaVinculoOrganizacion, persona => persona.PersonaID, persVincOrg => persVincOrg.PersonaID, (persona, persVincOrg) => new { Persona = persona, PersonaVinculoOrganizacion = persVincOrg }).Where(item => item.PersonaVinculoOrganizacion.OrganizacionID.Equals(pOrganizacionID)).Select(item => item.Persona).ToList();
            return dataWrapperPersona;
        }

        /// <summary>
        /// Obtiene la persona a la que pertenece un perfil
        /// </summary>
        /// <param name="pPerfilID">Guid del perfil</param>
        /// <returns>Dataset con la persona a la que pertenece el perfil</returns>
        public DataWrapperPersona ObtenerPersonaPorPerfil(Guid pPerfilID)
        {
            DataWrapperPersona dataWrapperPersona = new DataWrapperPersona();
            dataWrapperPersona.ListaPersona = mEntityContext.Persona.Join(mEntityContext.Perfil, persona => persona.PersonaID, perfil => perfil.PersonaID, (persona, perfil) => new { Persona = persona, Perfil = perfil }).Where(item => item.Perfil.PerfilID.Equals(pPerfilID)).Select(persona => new Persona
            {
                PersonaID = persona.Persona.PersonaID,
                UsuarioID = persona.Persona.UsuarioID,
                Nombre = persona.Persona.Nombre,
                Apellidos = persona.Persona.Apellidos,
                Email = persona.Persona.Email,
                EsBuscable = persona.Persona.EsBuscable,
                EsBuscableExternos = persona.Persona.EsBuscableExternos,
                Eliminado = persona.Persona.Eliminado,
                Idioma = persona.Persona.Idioma,
                FechaNacimiento = persona.Persona.FechaNacimiento,
                Sexo = persona.Persona.Sexo,
                EstadoCorreccion = persona.Persona.EstadoCorreccion,
                FechaNotificacionCorreccion = persona.Persona.FechaNotificacionCorreccion,
                VersionFoto = persona.Persona.VersionFoto,
                CoordenadasFoto = persona.Persona.CoordenadasFoto,
                FechaAnadidaFoto = persona.Persona.FechaAnadidaFoto
            }
                ).Distinct().ToList();
            return dataWrapperPersona;
        }

        /// <summary>
        /// Obtiene el ID de la persona a la que pertenece un perfil
        /// </summary>
        /// <param name="pPerfilID">Guid del perfil</param>
        /// <returns>Guid de la persona a la que pertenece el perfil</returns>
        public Guid ObtenerPersonaIDPorPerfil(Guid pPerfilID)
        {
            return mEntityContext.Perfil.Where(perfil => perfil.PerfilID.Equals(pPerfilID) && perfil.PersonaID.HasValue).Select(item => item.PersonaID.Value).ToList().FirstOrDefault();
        }

        /// <summary>
        /// Obtiene las personas a la que pertenecen unos perfiles.
        /// </summary>
        /// <param name="pPerfilesID">Guids de los perfiles</param>
        /// <returns>Dataset con las personas a la que pertenecen unos perfiles</returns>
        public DataWrapperPersona ObtenerPersonasPorPerfilesID(List<Guid> pPerfilesID)
        {
            DataWrapperPersona dataWrapperPersona = new DataWrapperPersona();
            if (pPerfilesID.Count > 0)
            {
                dataWrapperPersona.ListaPersona = mEntityContext.Persona.Join(mEntityContext.Perfil, persona => persona.PersonaID, perfil => perfil.PersonaID, (persona, perfil) => new { Persona = persona, Perfil = perfil }).Where(item => pPerfilesID.Contains(item.Perfil.PerfilID)).Select(item => item.Persona).ToList().Distinct().ToList();
            }
            return dataWrapperPersona;
        }

        /// <summary>
        /// Obtiene una persona a partir del identificador pasado por parámetro
        /// </summary>
        /// <param name="pPersonaID">Identificador de persona</param>
        /// <returns>Dataset de personas</returns>
        public List<Persona> ObtenerPersonaPorIDCargaLigera(Guid pPersonaID)
        {
            return mEntityContext.Persona.Where(persona => persona.PersonaID.Equals(pPersonaID)).ToList();
        }

        /// <summary>
        /// Obtiene la configuracion de una persona a partir del identificador pasado por parámetro
        /// </summary>
        /// <param name="pPersonaID">Identificador de persona</param>
        /// <returns>Dataset de personas</returns>
        public ConfiguracionGnossPersona ObtenerConfiguracionPersonaPorID(Guid pPersonaID)
        {
            return mEntityContext.ConfiguracionGnossPersona.Where(configGnossPersona => configGnossPersona.PersonaID.Equals(pPersonaID)).ToList().FirstOrDefault();
        }

        /// <summary>
        /// Comprueba si la persona pasada por parámetro permite que los usuarios de GNOSS vean sus recursos
        /// </summary>
        /// <param name="pPersonaID">Identificador de persona</param>
        /// <returns>TRUE si la persona permite que los usuarios de GNOSS vean sus recursos, FALSE en caso contrario</returns>
        public bool PersonaPermiteVerRecursosAUsusariosGNOSS(Guid pPersonaID)
        {
            var resultado = mEntityContext.ConfiguracionGnossPersona.Where(persona => persona.PersonaID.Equals(pPersonaID)).Select(item => item.VerRecursos).ToList();

            if (resultado.Count > 0)
            {
                return resultado.First();
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Obtiene las personas pertenecientes a un proyecto, es decir, que están en la estructura orgánica de la organización
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Dataset de personas con las personas del proyecto</returns>
        public List<Persona> ObtenerPersonasDeProyectoCargaLigera(Guid pOrganizacionID, Guid pProyectoID)
        {
            return mEntityContext.Persona.Join(mEntityContext.ProyectoRolUsuario, persona => persona.UsuarioID, proyectoRolUsuario => proyectoRolUsuario.UsuarioID, (persona, proyectoRolUsuario) => new { Persona = persona, ProyectoRolUsuario = proyectoRolUsuario }).Where(item => item.ProyectoRolUsuario.ProyectoID.Equals(pProyectoID) && item.ProyectoRolUsuario.OrganizacionGnossID.Equals(pOrganizacionID) && !item.Persona.Eliminado).Select(persona => new Persona
            {
                PersonaID = persona.Persona.PersonaID,
                UsuarioID = persona.Persona.UsuarioID,
                Nombre = persona.Persona.Nombre,
                Apellidos = persona.Persona.Apellidos,
                Email = persona.Persona.Email,
                EsBuscable = persona.Persona.EsBuscable,
                EsBuscableExternos = persona.Persona.EsBuscableExternos,
                Eliminado = persona.Persona.Eliminado,
                Idioma = persona.Persona.Idioma,
                FechaNacimiento = persona.Persona.FechaNacimiento,
                Sexo = persona.Persona.Sexo,
                EstadoCorreccion = persona.Persona.EstadoCorreccion,
                FechaNotificacionCorreccion = persona.Persona.FechaNotificacionCorreccion,
                VersionFoto = persona.Persona.VersionFoto,
                CoordenadasFoto = persona.Persona.CoordenadasFoto,
                FechaAnadidaFoto = persona.Persona.FechaAnadidaFoto
            }
                ).Distinct().ToList();
        }


        /// <summary>
        /// Obtiene las personas pertenecientes a un grupo de proyecto
        /// </summary>
        /// <param name="pGruposID">Identificador de los grupos</param>
        /// <returns>Dataset de personas con las personas del grupo</returns>
        public List<Persona> ObtenerPersonasDeGrupoDeProyectoCargaLigera(List<Guid> pGruposID)
        {
            List<Persona> listaPersonas = new List<Persona>();

            if (pGruposID.Count > 0)
            {
                listaPersonas = mEntityContext.Persona.JoinPerfil().JoinIdentidad().JoinGrupoIdentidadesParticipacion().Where(item => pGruposID.Contains(item.GrupoIdentidadesParticipacion.GrupoID) && !item.Persona.Eliminado).Select(item => item.Persona).Distinct().ToList();
            }

            return listaPersonas;
        }

        /// <summary>
        /// Obtiene los identificadores de las personas de una comunidad y sus correos electronicos. Si la comunidad es cadena vacía devuelve los del ecosistema
        /// </summary>
        /// <param name="pNombreCortoComunidad">Nombre corto de la comunidad</param>
        /// <returns>Array de cadenas con el identificador y el correo de la persona separados por una coma</returns>
        public string[] ObtenerPersonaIDEmailPersonasComunidad(string pNombreCortoComunidad)
        {
            if (!string.IsNullOrEmpty(pNombreCortoComunidad))
            {
                return mEntityContext.Persona.Join(mEntityContext.Perfil, persona => persona.PersonaID, perfil => perfil.PersonaID, (persona, perfil) => new { Persona = persona, Perfil = perfil }).Join(mEntityContext.Identidad, personaPerfil => personaPerfil.Perfil.PerfilID, identidad => identidad.PerfilID, (personaPerfil, identidad) => new { Persona = personaPerfil.Persona, Perfil = personaPerfil.Perfil, Identidad = identidad }).Join(mEntityContext.Proyecto, personaPerfilIdentidad => personaPerfilIdentidad.Identidad.ProyectoID, proyecto => proyecto.ProyectoID, (personaPerfilIdentidad, proyecto) => new { Identidad = personaPerfilIdentidad.Identidad, Persona = personaPerfilIdentidad.Persona, Perfil = personaPerfilIdentidad.Perfil, Proyecto = proyecto }).Where(item => item.Identidad.FechaBaja == null && item.Identidad.FechaExpulsion == null && !item.Perfil.Eliminado && item.Proyecto.NombreCorto.Equals(pNombreCortoComunidad)).Select(item => item.Persona.PersonaID + "," + item.Persona.Email).ToArray();
            }
            else
            {
                return mEntityContext.Persona.Where(persona => !persona.Eliminado).Select(item => item.PersonaID + "," + item.Email).ToArray();
            }
        }

        /// <summary>
        /// Devuelve si el email pasado como parametro ya pertenece al proyecto
        /// </summary>
        /// <param name="pProyectoID">Comunidad a la que se va a enviar la invitación</param>
        /// <param name="pEmail">Email al que se va a invitar a la comunidad</param>
        /// <returns>True si el correo ya es un miembro de la comunidad, False si aún no lo es.</returns>
        public bool ObtenerSiCorreoPerteneceAProyecto(Guid pProyectoID, string pEmail)
        {
            var resultado = mEntityContext.Persona.Join(mEntityContext.ProyectoRolUsuario, persona => persona.UsuarioID, proyectoRolUsuario => proyectoRolUsuario.UsuarioID, (persona, proyectoRolUsuario) => new { Persona = persona, ProyectoRolUsuario = proyectoRolUsuario }).Where(item => item.ProyectoRolUsuario.ProyectoID.Equals(pProyectoID) && item.Persona.Email.Equals(pEmail.ToLower())).ToList();
            if (resultado.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Obtiene todos los datos de las personas pertenecientes a un proyectos, es decir, 
        /// que están en la estructura orgánica de la organización
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Dataset de personas con las personas del proyecto</returns>
        public List<Persona> ObtenerPersonasDeProyecto(Guid pOrganizacionID, Guid pProyectoID)
        {
            return mEntityContext.Persona.Join(mEntityContext.ProyectoRolUsuario, persona => persona.UsuarioID, proyectoRolUsuario => proyectoRolUsuario.UsuarioID, (persona, proyectoRolUsuario) => new { Persona = persona, ProyectoRolUsuario = proyectoRolUsuario }).Where(item => item.ProyectoRolUsuario.ProyectoID.Equals(pProyectoID) && item.ProyectoRolUsuario.OrganizacionGnossID.Equals(pOrganizacionID)).Select(item => item.Persona).ToList();
        }

        /// <summary>
        /// Obtiene las personas a partir de identidades cargardas en un dataSet.
        /// </summary>
        /// <param name="pDataWrapperIdentidad">DataSet con las identidades de personas</param>
        /// <returns>Dataset de personas</returns>
        public DataWrapperPersona ObtenerPersonasIdentidadesCargadas(DataWrapperIdentidad pDataWrapperIdentidad)
        {
            DataWrapperPersona dataWrapperPersona = new DataWrapperPersona();
            List<Guid> listaGuid = new List<Guid>();
            foreach (AD.EntityModel.Models.IdentidadDS.Perfil filaPerfil in pDataWrapperIdentidad.ListaPerfil.Where(item => item.PersonaID.HasValue))
            {
                listaGuid.Add(filaPerfil.PersonaID.Value);
            }

            if (listaGuid.Count > 0)
            {
                dataWrapperPersona.ListaPersona = mEntityContext.Persona.Where(persona => listaGuid.Contains(persona.PersonaID)).ToList();

                dataWrapperPersona.ListaConfigGnossPersona = mEntityContext.ConfiguracionGnossPersona.Where(persona => listaGuid.Contains(persona.PersonaID)).ToList();

                dataWrapperPersona.ListaDatosTrabajoPersonaLibre = mEntityContext.DatosTrabajoPersonaLibre.Where(persona => listaGuid.Contains(persona.PersonaID)).ToList();
            }

            return dataWrapperPersona;
        }

        /// <summary>
        /// Obtiene las personas a partir de una lista de identidades
        /// </summary>
        /// <param name="pListaIdentidades">Lista de identificadores de identidades</param>
        /// <returns>Dataset de personas</returns>
        public DataWrapperPersona ObtenerPersonasPorIdentidad(List<Guid> pListaIdentidades)
        {
            DataWrapperPersona ListasPersonas = new DataWrapperPersona();

            if (pListaIdentidades.Count > 0)
            {
                ListasPersonas.ListaPersona = mEntityContext.Persona.Join(mEntityContext.Perfil, persona => persona.PersonaID, perfil => perfil.PersonaID, (persona, perfil) => new { Persona = persona, Perfil = perfil }).Join(mEntityContext.Identidad, personaPerfil => personaPerfil.Perfil.PerfilID, identidad => identidad.PerfilID, (personaPerfil, identidad) => new { Persona = personaPerfil.Persona, Perfil = personaPerfil.Perfil, Identidad = identidad }).Where(item => pListaIdentidades.Contains(item.Identidad.IdentidadID)).Select(item => item.Persona).ToList().Distinct().ToList();

                if (ListasPersonas.ListaPersona.Count > 0)
                {
                    List<Guid> listaPersonaID = ListasPersonas.ListaPersona.Select(item => item.PersonaID).ToList();
                    ListasPersonas.ListaConfigGnossPersona = mEntityContext.ConfiguracionGnossPersona.Where(item => listaPersonaID.Contains(item.PersonaID)).ToList();
                }
            }
            return ListasPersonas;
        }

        /// <summary>
        /// Obtiene la persona de una identidad
        /// </summary>
        /// <param name="pIdentidad">Identificador de la identidad</param>
        /// <returns>Dataset de personas</returns>
        public DataWrapperPersona ObtenerPersonasPorIdentidad(Guid pIdentidad)
        {


            DataWrapperPersona dataWrapperPersonas = new DataWrapperPersona();

            List<Guid> ListaGuidID = mEntityContext.PerfilPersona.Join(mEntityContext.Identidad, perfilPersona => perfilPersona.PerfilID, identidad => identidad.PerfilID, (perfilPersona, identidad) => new { PerfilPersona = perfilPersona, Identidad = identidad }).Where(item => item.Identidad.IdentidadID.Equals(pIdentidad)).Select(item => item.PerfilPersona.PersonaID).Concat(mEntityContext.PerfilPersonaOrg.Join(mEntityContext.Identidad, perfilPersonaOrg => perfilPersonaOrg.PerfilID, identidad => identidad.PerfilID, (perfilPersonaOrg, identidad) => new { PerfilPersonaOrg = perfilPersonaOrg, Identidad = identidad }).Where(item => item.Identidad.IdentidadID.Equals(pIdentidad)).Select(item => item.PerfilPersonaOrg.PersonaID)).ToList();

            dataWrapperPersonas.ListaPersona = mEntityContext.Persona.Where(persona => ListaGuidID.Contains(persona.PersonaID)).ToList();
            dataWrapperPersonas.ListaConfigGnossPersona = mEntityContext.ConfiguracionGnossPersona.Where(configGnossPersona => ListaGuidID.Contains(configGnossPersona.PersonaID)).ToList();
            dataWrapperPersonas.ListaDatosTrabajoPersonaLibre = mEntityContext.DatosTrabajoPersonaLibre.Where(datosTrabajoPersonaLibre => ListaGuidID.Contains(datosTrabajoPersonaLibre.PersonaID)).ToList();

            return dataWrapperPersonas;
        }

        /// <summary>
        /// Obtiene las personas pasando sus identidades como parámetro
        /// </summary>
        /// <param name="pListaIdentidades">Lista de ID's de las identidades</param>
        /// <returns>PersonaDS</returns>
        public DataWrapperPersona ObtenerPersonasPorIdentidadesCargaLigera(List<Guid> pListaIdentidades)
        {
            DataWrapperPersona dataWrapperPersona = new DataWrapperPersona();
            if (pListaIdentidades.Count > 0)
            {
                dataWrapperPersona.ListaPersona = mEntityContext.Identidad.Join(mEntityContext.Perfil, identidad => identidad.PerfilID, perfil => perfil.PerfilID, (identidad, perfil) => new { Identidad = identidad, Perfil = perfil }).Join(mEntityContext.Persona, identidadPerfil => identidadPerfil.Perfil.PersonaID, persona => persona.PersonaID, (identidadPerfil, persona) => new { Identidad = identidadPerfil.Identidad, Perfil = identidadPerfil.Perfil, Persona = persona }).Where(identidad => pListaIdentidades.Contains(identidad.Identidad.IdentidadID)).Select(item => item.Persona).ToList();
            }

            return dataWrapperPersona;
        }

        /// <summary>
        /// Obtiene una lista de personas
        /// </summary>
        /// <param name="pListaPersonaID">Lista de identificadores de personas</param>
        /// <returns>Dataset de personas</returns>
        public List<Persona> ObtenerPersonasPorIDCargaLigera(List<Guid> pListaPersonaID)
        {
            if (pListaPersonaID.Count > 0)
            {
                return mEntityContext.Persona.Where(item => pListaPersonaID.Contains(item.PersonaID)).ToList();
            }
            return new List<Persona>();
        }

        /// <summary>
        /// Obtiene los datos de una persona y sus tag
        /// </summary>
        /// <param name="pPersonaID">Identificador de la persona</param>
        /// <returns>Dataset de personas con la persona de organización</returns>
        public DataWrapperPersona ObtenerPersonaPorID(Guid pPersonaID)
        {
            DataWrapperPersona ListaDatosPersona = new DataWrapperPersona();

            ListaDatosPersona.ListaPersona = mEntityContext.Persona.Where(persona => persona.PersonaID.Equals(pPersonaID)).ToList();
            ListaDatosPersona.ListaDatosTrabajoPersonaLibre = mEntityContext.DatosTrabajoPersonaLibre.Where(item => item.PersonaID.Equals(pPersonaID)).ToList();
            ListaDatosPersona.ListaConfigGnossPersona = mEntityContext.ConfiguracionGnossPersona.Where(item => item.PersonaID.Equals(pPersonaID)).ToList();

            return ListaDatosPersona;
        }

        /// <summary>
        /// Obtiene los datos de las personas que se le pasan por parametro
        /// </summary>
        /// <param name="pListaPersonaID">Lista de identificadores de personas</param>
        /// <returns>Dataset de personas con las personas de la lista</returns>
        public DataWrapperPersona ObtenerPersonasPorID(List<Guid> pListaPersonaID)
        {
            DataWrapperPersona dataWrapper = new DataWrapperPersona();
            if (pListaPersonaID.Count > 0)
            {

                dataWrapper.ListaPersona = mEntityContext.Persona.Where(persona => pListaPersonaID.Contains(persona.PersonaID)).ToList();

                dataWrapper.ListaDatosTrabajoPersonaLibre = mEntityContext.DatosTrabajoPersonaLibre.Where(item => pListaPersonaID.Contains(item.PersonaID)).ToList();

                dataWrapper.ListaConfigGnossPersona = mEntityContext.ConfiguracionGnossPersona.Where(item => pListaPersonaID.Contains(item.PersonaID)).ToList();
            }

            return dataWrapper;
        }

        /// <summary>
        /// Obtiene una fila de persona.
        /// </summary>
        /// <param name="pPersonaID">Identificador de la persona</param>
        /// <returns>DataSet de persona con una fila</returns>
        public Persona ObtenerFilaPersonaPorID(Guid pPersonaID)
        {
            return mEntityContext.Persona.Where(persona => persona.PersonaID.Equals(pPersonaID)).ToList().FirstOrDefault();

        }

        /// <summary>
        /// Obtiene las personas que participan en una lista de organizaciones (Sin el campo Foto de la persona)
        /// </summary>
        /// <param name="pListaIDs">Lista de identificadores de organizaciones</param>
        /// <returns>Dataset de personas con las personas que participan en organización</returns>
        public DataWrapperPersona ObtenerPersonasDeListaOrganizaciones(List<Guid> pListaIDs)
        {
            DataWrapperPersona ListaOrganizaciones = new DataWrapperPersona();

            if (pListaIDs.Count > 0)
            {
                ListaOrganizaciones.ListaPersona = mEntityContext.Persona.Join(mEntityContext.PersonaVinculoOrganizacion, persona => persona.PersonaID, personaVinculoOrg => personaVinculoOrg.PersonaID, (persona, personaVinculoOrg) => new { Persona = persona, PersonaVinculoOrganizacion = personaVinculoOrg }).Where(item => pListaIDs.Contains(item.PersonaVinculoOrganizacion.OrganizacionID)).Select(item => item.Persona).ToList().Distinct().ToList();

                ListaOrganizaciones.ListaDatosTrabajoPersonaLibre = mEntityContext.DatosTrabajoPersonaLibre.Join(mEntityContext.PersonaVinculoOrganizacion, datosTrabajoPersonaLibre => datosTrabajoPersonaLibre.PersonaID, personaVinculoOrg => personaVinculoOrg.PersonaID, (datosTrabajoPersonaLibre, personaVinculoOrganizacion) => new { DatosTrabajoPersonaLibre = datosTrabajoPersonaLibre, PersonaVinculoOrganizacion = personaVinculoOrganizacion }).Where(item => pListaIDs.Contains(item.PersonaVinculoOrganizacion.OrganizacionID)).Select(item => item.DatosTrabajoPersonaLibre).Distinct().ToList();
            }
            return ListaOrganizaciones;
        }

        /// <summary>
        /// Obtiene todas las personas que solicitan acceso para un proyecto concreto
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Dataset de personas</returns>
        public List<Persona> ObtenerPersonasSolicitanAccesoAProyecto(Guid pOrganizacionID, Guid pProyectoID)
        {
            return mEntityContext.Persona.Join(mEntityContext.SolicitudUsuario, persona => persona.PersonaID, solUsuario => solUsuario.PersonaID, (persona, solUsuario) =>
            new
            {
                Persona = persona,
                SolicitudUsuario = solUsuario
            }).Join(mEntityContext.Solicitud, personaSolUsuario => personaSolUsuario.SolicitudUsuario.SolicitudID, solicitud => solicitud.SolicitudID, (personaSolUsuario, solicitud) =>
            new
            {
                Persona = personaSolUsuario.Persona,
                SolicitudUsuario = personaSolUsuario.SolicitudUsuario,
                Solicitud = solicitud
            }).Where(item => item.Solicitud.OrganizacionID.Equals(pOrganizacionID) && item.Solicitud.ProyectoID.Equals(pProyectoID) && item.Solicitud.Estado.Equals((short)EstadoSolicitud.Espera))
                .Select(item => item.Persona).ToList().Distinct().ToList();

        }

        /// <summary>
        /// Valida si existen personas asociadas a una organizacion
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización</param>      
        /// <returns>TRUE en caso de existir personas asociadas a la organización, FALSE en caso contrario</returns>
        public bool TienePersonasOrganizacion(Guid pOrganizacionID)
        {
            var resultado = mEntityContext.Persona.Join(mEntityContext.PersonaVinculoOrganizacion, persona => persona.PersonaID, personaVinculoOrganizacion => personaVinculoOrganizacion.PersonaID, (persona, personaVinculoOrganizacion) => new { Persona = persona, PersonaVinculoOrganizacion = personaVinculoOrganizacion }).Where(item => item.PersonaVinculoOrganizacion.OrganizacionID.Equals(pOrganizacionID)).ToList();

            if (resultado.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Actualiza el idioma de una persona
        /// </summary>
        /// <param name="pPersonaID">Identificador de la persona</param>
        /// <param name="pIdioma">Idioma</param>
        public void ActualizarIdiomaPersona(Guid pPersonaID, string pIdioma)
        {
            Persona resultado = mEntityContext.Persona.FirstOrDefault(persona => persona.PersonaID.Equals(pPersonaID));
            resultado.Idioma = pIdioma;

            mEntityContext.SaveChanges();
        }

        /// <summary>
        /// Actualiza personas
        /// </summary>
        /// <param name="pPersonaDS">Dataset de personas</param>
        public void ActualizarPersonas(DataWrapperPersona pDataWrapperPersona = null)
        {
            try
            {
                if (pDataWrapperPersona != null)
                {
                    foreach (Persona persona in mEntityContext.Persona.Local)
                    {
                        if (mEntityContext.Entry(persona).State == EntityState.Modified)
                        {
                            ActualizarNombrePersonaCambiado(persona);
                        }
                    }
                }

                mEntityContext.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Comprueba si existe el DNI a partir del texto pasado por parámetro
        /// </summary>
        /// <param name="pDNI">Texto del documento</param>
        /// <returns>TRUE si existe, FALSE en caso contrario</returns>
        public bool ExisteDNI(string pDNI)
        {
            var resultado = mEntityContext.Persona.Where(persona => persona.ValorDocumentoAcreditativo.ToUpper().Equals(pDNI.ToUpper()) && !persona.Eliminado).Select(item => item.PersonaID).ToList();
            if (resultado.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Devuelve los correos de las personas administradoras de un proyecto
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización a la que pertenece el proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto del que se desean obtener los administradores</param>
        /// <returns>Lista con los correos de los administradores</returns>
        public List<string> ObtenerCorreoDeAdministradoresDeProyecto(Guid pOrganizacionID, Guid pProyectoID)
        {
            return mEntityContext.Persona.Join(mEntityContext.AdministradorProyecto, persona => persona.UsuarioID, adminProyecto => adminProyecto.UsuarioID, (persona, adminProyecto) => new { Persona = persona, AdministradorProyecto = adminProyecto }).Where(item => item.AdministradorProyecto.ProyectoID.Equals(pProyectoID) && item.AdministradorProyecto.OrganizacionID.Equals(pOrganizacionID) && item.AdministradorProyecto.Tipo.Equals((short)TipoRolUsuario.Administrador) && !item.Persona.Eliminado).Select(item => item.Persona.Email).Distinct().ToList();
        }

        /// <summary>
        /// Obtiene el email y la identidad a partir del perfil pasado por parámetro
        /// </summary>
        /// <param name="pPerfilID">Identificador de perfil</param>
        /// <returns>Dataset no tipado</returns>
        public List<PersonaIdentidad> ObtenerEmaileIdentidadATravesDePerfil(Guid pPerfilID)
        {
            return mEntityContext.PerfilPersona.Join(mEntityContext.Identidad, perfPers => perfPers.PerfilID, identidad => identidad.PerfilID, (perfPers, identidad) => new
            {
                PerfilPersona = perfPers,
                Identidad = identidad
            }).Join(mEntityContext.Persona, perfPersIdent => perfPersIdent.PerfilPersona.PersonaID, persona => persona.PersonaID, (perfPersIdent, persona) =>
            new
            {
                PerfilPersona = perfPersIdent.PerfilPersona,
                Identidad = perfPersIdent.Identidad,
                Persona = persona
            })
                .Where(item => item.PerfilPersona.PerfilID.Equals(pPerfilID) && item.Identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto) && item.Persona.Email != null).Select(item => new PersonaIdentidad() { IdentidadId = item.Identidad.IdentidadID, PersonaID = item.PerfilPersona.PersonaID, Email = item.Persona.Email, Nombre = item.Persona.Nombre })
             .Union(mEntityContext.PerfilPersonaOrg.Join(mEntityContext.Identidad, perfPersOrg => perfPersOrg.PerfilID, identidad => identidad.PerfilID, (perfPersOrg, identidad) =>
             new
             {
                 PerfilPersonaOrg = perfPersOrg,
                 Identidad = identidad
             }).Join(mEntityContext.PersonaVinculoOrganizacion, perfPersOrgIdent =>
             new { perfPersOrgIdent.PerfilPersonaOrg.PersonaID, perfPersOrgIdent.PerfilPersonaOrg.OrganizacionID }, persVincOrg => new { persVincOrg.PersonaID, persVincOrg.OrganizacionID }, (persPerfOrgIdent, persVincOrg) =>
             new
             {
                 PerfilPersonaOrg = persPerfOrgIdent.PerfilPersonaOrg,
                 Identidad = persPerfOrgIdent.Identidad,
                 PersonaVinculoOrganizacion = persVincOrg
             }).Join(mEntityContext.Persona, perfPersOrgIden => perfPersOrgIden.PerfilPersonaOrg.PersonaID, persona => persona.PersonaID, (perfPersOrgIden, persona) => new
             {
                 PerfilPersonaOrg = perfPersOrgIden.PerfilPersonaOrg,
                 Identidad = perfPersOrgIden.Identidad,
                 PersonaVinculoOrganizacion = perfPersOrgIden.PersonaVinculoOrganizacion,
                 Persona = persona
             }).Where(item => item.PerfilPersonaOrg.PerfilID.Equals(pPerfilID) && item.Identidad.ProyectoID.Equals(ProyectoAD.MetaProyecto) && item.PersonaVinculoOrganizacion.EmailTrabajo != null)
             .Select(item => new PersonaIdentidad() { IdentidadId = item.Identidad.IdentidadID, PersonaID = item.PerfilPersonaOrg.PersonaID, Email = item.Persona.Email, Nombre = item.Persona.Nombre })).ToList();
        }

        /// <summary>
        /// Obtiene los tags de una persona en un proyecto
        /// </summary>
        /// <param name="pPersonaID">Identificador de la persona</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns></returns>
        public Dictionary<Guid, string> ObtenerTagsDePersonasEnProyecto(List<Guid> pListaPersonaID, Guid pProyectoID)
        {
            Dictionary<Guid, string> tags = new Dictionary<Guid, string>();
            foreach (Guid id in pListaPersonaID)
            {
                tags.Add(id, "");
            }
            var resultado = mEntityContext.Persona.Join(mEntityContext.Perfil, persona => persona.PersonaID, perfil => perfil.PersonaID, (persona, perfil) =>
            new
            {
                Persona = persona,
                Perfil = perfil
            }).Join(mEntityContext.Identidad, personaPerfil => personaPerfil.Perfil.PerfilID, identidad => identidad.PerfilID, (personaPerfil, identidad) =>
            new
            {
                Persona = personaPerfil.Persona,
                Perfil = personaPerfil.Perfil,
                Identidad = identidad
            }).Join(mEntityContext.Curriculum, personaPerfilIdentidad => personaPerfilIdentidad.Identidad.CurriculumID, curriculum => curriculum.CurriculumID, (personaPerfilIdentidad, curriculum) =>
             new
             {
                 Persona = personaPerfilIdentidad.Persona,
                 Perfil = personaPerfilIdentidad.Perfil,
                 Identidad = personaPerfilIdentidad.Identidad,
                 Curriculum = curriculum
             }).Where(item => pListaPersonaID.Contains(item.Persona.PersonaID) && item.Identidad.ProyectoID.Equals(pProyectoID) && !item.Identidad.FechaBaja.HasValue && !item.Identidad.FechaExpulsion.HasValue && !item.Perfil.Eliminado && !item.Persona.Eliminado && item.Identidad.Tipo != 2).Select(item => new { item.Persona.PersonaID, item.Curriculum.Tags }).ToList().Distinct().ToList();

            foreach (var fila in resultado)
            {
                if (fila.Tags != null)
                {
                    Guid idDoc = fila.PersonaID;
                    string tagss = fila.Tags;
                    tags[idDoc] = tagss;
                }
            }
            return tags;
        }

        /// <summary>
        /// Obtiene los tags de una persona en un proyecto
        /// </summary>
        /// <param name="pPersonaID">Identificador de la persona</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns></returns>
        public string ObtenerTagsDePersonaEnProyecto(Guid pPersonaID, Guid pProyectoID)
        {
            List<Guid> lista = new List<Guid>();
            lista.Add(pPersonaID);
            return ObtenerTagsDePersonasEnProyecto(lista, pProyectoID)[pPersonaID];
        }

        /// <summary>
        /// Actualiza el número de la versión de la foto de la persona
        /// </summary>
        /// <param name="pPersonaID">ID de la Persona</param>
        public void ActualizarVersionFotoPersona(Guid pPersonaID)
        {

            Persona resultado = mEntityContext.Persona.FirstOrDefault(persona => persona.PersonaID.Equals(pPersonaID));
            if (resultado != null)
            {
                if (!resultado.VersionFoto.HasValue)
                {
                    resultado.VersionFoto = 1;
                }
                else
                {
                    resultado.VersionFoto = resultado.VersionFoto.Value + 1;
                }
            }
            mEntityContext.SaveChanges();
        }

        /// <summary>
        /// Obtiene los IDs de las personas a partir de IDs de usuarios.
        /// </summary>
        /// <param name="pUsuariosID">IDs de usuarios</param>
        /// <returns>Diccionario UsuarioID, PersonaID</returns>
        public Dictionary<Guid, Guid> ObtenerPersonasIDDeUsuariosID(List<Guid> pUsuariosID)
        {
            Dictionary<Guid, Guid> usuPerID = new Dictionary<Guid, Guid>();
            if (pUsuariosID.Count > 0)
            {
                var resultado = mEntityContext.Persona.Where(persona => pUsuariosID.Contains(persona.UsuarioID.Value)).Select(item => new { item.UsuarioID, item.PersonaID });
                foreach (var item in resultado)
                {
                    usuPerID.Add(item.UsuarioID.Value, item.PersonaID);
                }
            }
            return usuPerID;
        }

        /// <summary>
        /// Obtiene los IDs de los usuarios a partir de IDs de personas.
        /// </summary>
        /// <param name="pPersonasID">IDs de personas</param>
        /// <returns>Diccionario PersonaID, UsuarioID</returns>
        public Dictionary<Guid, Guid> ObtenerUsuariosIDDePersonasID(List<Guid> pPersonasID)
        {
            Dictionary<Guid, Guid> perUsuID = new Dictionary<Guid, Guid>();
            if (pPersonasID.Count > 0)
            {
                var resultado = mEntityContext.Persona.Where(persona => pPersonasID.Contains(persona.PersonaID)).Select(item => new { item.PersonaID, item.UsuarioID }).ToList();
                foreach (var item in resultado)
                {
                    if (item.UsuarioID.HasValue)
                    {
                        perUsuID.Add(item.PersonaID, item.UsuarioID.Value);
                    }
                    else
                    {
                        perUsuID.Add(item.PersonaID, Guid.Empty);
                    }

                }
            }
            return perUsuID;

        }

        /// <summary>
        /// Obtiene los IDs de los usuarios y sus DNIs a partir de IDs de personas.
        /// </summary>
        /// <param name="pPersonasID">IDs de personas</param>
        /// <returns>Diccionario PersonaID, UsuarioID, DNI</returns>
        public Dictionary<Guid, KeyValuePair<Guid, string>> ObtenerUsuariosIDYDNIsDePersonasID(List<Guid> pPersonasID)
        {
            Dictionary<Guid, KeyValuePair<Guid, string>> perUsuID = new Dictionary<Guid, KeyValuePair<Guid, string>>();
            if (pPersonasID.Count > 0)
            {
                var resultado = mEntityContext.Persona.Where(persona => persona.UsuarioID.HasValue && pPersonasID.Contains(persona.PersonaID)).Select(item => new { item.PersonaID, item.UsuarioID, item.ValorDocumentoAcreditativo });

                foreach (var item in resultado)
                {
                    perUsuID.Add(item.PersonaID, new KeyValuePair<Guid, string>(item.UsuarioID.Value, item.ValorDocumentoAcreditativo));
                }
            }
            return perUsuID;
        }

        #endregion

        #region Privados

        /// <summary>
        /// Actualiza el cambio del nombre de una persona en sus perfiles
        /// </summary>
        /// <param name="pFilaPersona">Fila de la persona de la cual se ha cambiado el nombre</param>
        private void ActualizarNombrePersonaCambiado(Persona pFilaPersona)
        {
            //Tengo que actualizar en IdentidadDS / Perfil / "NombrePerfil"
            IdentidadAD identidadAD = new IdentidadAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication,mLoggerFactory.CreateLogger<IdentidadAD>(),mLoggerFactory);
            identidadAD.ActualizarCambioNombrePersona(pFilaPersona.PersonaID, pFilaPersona.Nombre, pFilaPersona.Apellidos);
            identidadAD.Dispose();
        }

        #endregion

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene o establece si se deben actualizar los datos desnormalizados de persona
        /// </summary>
        public bool ActualizarDatosDesnormalizados
        {
            get
            {
                return mActualizarDatosDesnormalizados;
            }
            set
            {
                this.mActualizarDatosDesnormalizados = value;
            }
        }

        #endregion
    }

    public class PersonaIdentidad
    {
        public Guid IdentidadId { get; set; }
        public Guid PersonaID { get; set; }
        public string Email { get; set; }
        public string Nombre { get; set; }

    }

}
