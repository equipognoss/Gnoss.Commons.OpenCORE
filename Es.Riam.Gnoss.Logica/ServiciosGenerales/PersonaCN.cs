using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.PersonaDS;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Usuarios;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Es.Riam.Gnoss.Logica.ServiciosGenerales
{
    #region Enumeración

    /// <summary>
    /// Intervalos de edades para una persona
    /// </summary>
    public enum IntervalosEdadPersona
    {
        /// <summary>
        /// No aplicar ningún intervalo
        /// </summary>
        Ninguno,

        /// <summary>
        /// Edad comprendida entre 18 y 25
        /// </summary>
        Entre18y25,

        /// <summary>
        /// Edad comprendida entre 25 y 30
        /// </summary>
        Entre25y30,

        /// <summary>
        /// Edad comprendida entre 30 y 40
        /// </summary>
        Entre30y40,

        /// <summary>
        /// Edad comprendida entre 40 y 50
        /// </summary>
        Entre40y50,

        /// <summary>
        /// Edad superior a 50 años
        /// </summary>
        Mayor50
    }

    #endregion

    /// <summary>
    /// Lógica referente a persona
    /// </summary>
    public class PersonaCN : BaseCN, IDisposable
    {

        #region Miembros

        private LoggingService mLoggingService;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        #endregion

        #region Constructores

        /// <summary>
        /// Constructor sin parámetros
        /// </summary>
        public PersonaCN(EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<PersonaCN> logger, ILoggerFactory loggerFactory)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication,logger,loggerFactory)
        {
            mLoggingService = loggingService;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            if(loggerFactory == null)
            {
                this.PersonaAD = new PersonaAD(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication, null, null);
            }
            else
            {
                this.PersonaAD = new PersonaAD(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<PersonaAD>(), mLoggerFactory);
            }
                
        }

        /// <summary>
        /// Constructor a partir del fichero de configuración de la base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuración</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public PersonaCN(string pFicheroConfiguracionBD, EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<PersonaCN> logger, ILoggerFactory loggerFactory)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication,logger,loggerFactory)
        {
            mLoggingService = loggingService;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            this.PersonaAD = new PersonaAD(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication,mLoggerFactory.CreateLogger<PersonaAD>(),mLoggerFactory);
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
            return PersonaAD.ObtenerUsuarioIDPerfilIDPorEmailYOrganizacion(pNombreCortoOrg, pListaCorreos);
        }

        /// <summary>
        /// Obtiene el correo de la persona que se le pasa por parámetro
        /// </summary>
        /// <param name="pPersonaID">Identificador de persona</param>
        /// <param name="pOrganizacionID">Identificador de organización</param>
        /// <returns>Email</returns>
        public string ObtenerCorreoDePersonaID(Guid pPersonaID, Guid? pOrganizacionID)
        {
            return PersonaAD.ObtenerCorreoDePersonaID(pPersonaID, pOrganizacionID);
        }

        /// <summary>
        /// Obtiene el idioma de la persona que se le pasa por parámetro
        /// </summary>
        /// <param name="pPersonaID">Identificador de persona</param>
        /// <returns>Idioma</returns>
        public string ObtenerIdiomaDePersonaID(Guid pPersonaID)
        {
            if (pPersonaID.Equals(UsuarioAD.Invitado))
            {
                return "es";
            }
            else
            {
                return PersonaAD.ObtenerIdiomaDePersonaID(pPersonaID);
            }
        }


        /// <summary>
        /// Obtiene el usuarioID de la persona que se le pasa por parámetro
        /// </summary>
        /// <param name="pPersonaID">Identificador de persona</param>
        /// <returns>Guid</returns>
        public Guid? ObtenerUsuarioIDDePersonaID(Guid pPersonaID)
        {
            return PersonaAD.ObtenerUsuarioIDDePersonaID(pPersonaID);
        }

        /// <summary>
        /// Obtiene los usuarioID de la lista de perfiles que se le pasa por parámetro
        /// </summary>
        /// <param name="pListaPerfilID">Lista de perfiles</param>
        /// <returns>Lista de usuariosID</returns>
        public List<Guid> ObtenerUsuariosIDDeListaPerfil(List<Guid> pListaPerfilID)
        {
            return PersonaAD.ObtenerUsuariosIDDeListaPerfil(pListaPerfilID);
        }

        /// <summary>
        /// Comprueba si los correos de la lista ya existen
        /// </summary>
        /// <param name="pEmail">Lista de Emails que se quieren comprobar</param>
        /// <returns>Lista con los correos que ya existen</returns>
        public List<string> EmailYaPerteneceAPersona(string[] pListaCorreos)
        {
            return PersonaAD.EmailYaPerteneceAPersona(pListaCorreos);
        }

        /// <summary>
        /// Comprueba si el email ya existe en la tabla de personas
        /// </summary>
        /// <param name="pEmail">Email que se quiere registrar</param>
        /// <param name="pPersonaID">Identificador de la persona que se comprueba</param>
        /// <returns>TRUE si ya existe, FALSE en caso contrario</returns>
        public bool ExisteEmail(string pEmail, Guid pPersonaID)
        {
            return PersonaAD.ExisteEmail(pEmail, pPersonaID);
        }

        /// <summary>
        /// Comprueba si un email pertenece a un usuario
        /// </summary>
        /// <param name="pEmail">Email que se quiere registrar</param>
        /// <param name="pUsuarioID">Identificador del usuario que se comprueba</param>
        /// <returns>TRUE si ya existe, FALSE en caso contrario</returns>
        public bool ComprobarEmailUsuario(string pEmail, Guid pUsuarioID)
        {
            return PersonaAD.ComprobarEmailUsuario(pEmail, pUsuarioID);
        }

        /// <summary>
        /// Comprueba si el email ya existe en la tabla de personas y la persona no está eliminada
        /// </summary>
        /// <param name="pEmail">Email que se quiere registrar</param>
        /// <returns>TRUE si ya existe, FALSE en caso contrario</returns>
        public bool ExisteEmail(string pEmail)
        {
            return PersonaAD.ExisteEmail(pEmail);
        }

        /// <summary>
        /// Comprueba si el email ya existe en la tabla de personas excluyendo la propia solicitud
        /// </summary>
        /// <param name="pEmail">Email que se quiere registrar</param>
        /// <param name="pSolicitudID">Identificador de la solicitud que se excluirá de la búsqueda</param>
        /// <returns>TRUE si ya existe, FALSE en caso contrario</returns>
        public bool ExisteEmailExceptoEnSolicitud(string pEmail, Guid pSolicitudID)
        {
            return PersonaAD.ExisteEmailExceptoEnSolicitud(pEmail, pSolicitudID);
        }

        /// <summary>
        /// Comprueba si el email ya existe en la tabla de perfiles de personas libres
        /// </summary>
        /// <param name="pEmail">Email que se quiere registrar</param>
        /// <param name="pPersonaID">Identificador de la persona que se comprueba</param>
        /// <returns>TRUE si ya existe, FALSE en caso contrario</returns>
        public bool ExisteEmailPerfilPersonaLibre(string pEmail, Guid pPersonaID)
        {
            return PersonaAD.ExisteEmailPerfilPersonaLibre(pEmail, pPersonaID);
        }

        /// <summary>
        /// Obtiene en formato string el tipo de documento acreditativo de una persona
        /// </summary>
        /// <param name="pTipo">Tipo de documento</param>
        /// <returns>Tipo de documento acreditativo de una persona en formato string</returns>
        public static string TipoDocumentoAcreditativoToString(TipoDocumentoAcreditativo pTipo)
        {
            return TipoDocumentoAcreditativoToString((short)pTipo);
        }

        /// <summary>
        /// Obtiene en formato string del tipo de documento acreditativo de una persona
        /// </summary>
        /// <param name="pTipoDocumento">Tipo de documento</param>
        public static string TipoDocumentoAcreditativoToString(short pTipoDocumento)
        {
            switch (pTipoDocumento)
            {
                case (short)TipoDocumentoAcreditativo.DNI:
                    {
                        return "DNI";
                    }
                case (short)TipoDocumentoAcreditativo.Pasaporte:
                    {
                        return "Pasaporte";
                    }
                case (short)TipoDocumentoAcreditativo.TarjetaResidencia:
                    {
                        return "T. residencia";
                    }
                default:
                    {
                        return "(Sin definir)"; // David: Este caso nunca debería darse
                    }
            }
        }

        /// <summary>
        /// Obtiene todas las personas que se corresponden con un nombre y apellidos pasados por parámetro
        /// </summary>
        /// <param name="pNombre">Nombre de la persona</param>
        /// <param name="pApellidos">Apellidos de la persona</param>
        /// <param name="pBuscarPersonasSinUsuario">TRUE si se debe buscar personas sin usuario, FALSE en caso contrario</param>
        /// <returns>Dataset de personas</returns>
        public List<Persona> ObtenerPersonasPorNombre(string pNombre, string pApellidos, bool pBuscarPersonasSinUsuario)
        {
            return PersonaAD.ObtenerPersonasPorNombre(pNombre, pApellidos, pBuscarPersonasSinUsuario);
        }

        /// <summary>
        /// Obtiene un GUID de persona dado un email
        /// </summary>
        /// <param name="pEmail">Email de la persona a buscar</param>
        /// <returns>GUID de la persona a la que corresponde el email</returns>
        public Guid ObtenerPersonaPorEmail(string pEmail)
        {
            return PersonaAD.ObtenerPersonaPorEmail(pEmail);
        }

        /// <summary>
        /// Obtiene una persona a partir del identificador de la misma
        /// </summary>
        /// <param name="pPersonaID">Identificador de la persona</param>
        /// <returns>Dataset de persona</returns>
        public DataWrapperPersona ObtenerPersonaPorIDCargaLigera(Guid pPersonaID)
        {
            if (pPersonaID.Equals(UsuarioAD.Invitado))
            {
                return this.ObtenerPersonaInvitado();
            }
            else
            {
                DataWrapperPersona personaDW = new DataWrapperPersona();
                personaDW.ListaPersona = this.PersonaAD.ObtenerPersonaPorIDCargaLigera(pPersonaID);
                return personaDW;
            }
        }

        /// <summary>
        /// Obtiene una fila de persona.
        /// </summary>
        /// <param name="pPersonaID">Identificador de la persona</param>
        /// <returns>DataSet de persona con una fila</returns>
        public Persona ObtenerFilaPersonaPorID(Guid pPersonaID)
        {
            return PersonaAD.ObtenerFilaPersonaPorID(pPersonaID);
        }

        /// <summary>
        /// Obtiene la configuración de una persona a partir del identificador pasado por parámetro
        /// </summary>
        /// <param name="pPersonaID">Identificador de persona</param>
        /// <returns>Dataset de personas</returns>
        public ConfiguracionGnossPersona ObtenerConfiguracionPersonaPorID(Guid pPersonaID)
        {
            return (this.PersonaAD.ObtenerConfiguracionPersonaPorID(pPersonaID));
        }

        /// <summary>
        /// Devuelve true si la persona pasada por id permite en su configuración que los usuarios de GNOSS vean sus recursos, false en caso contrario
        /// </summary>
        /// <param name="pPersonaID">Id de la persona para la que se desea saber su configuración de recursos para usuarios de GNOSS</param>
        /// <returns></returns>
        public bool PersonaPermiteVerRecursosAUsusariosGNOSS(Guid pPersonaID)
        {
            return PersonaAD.PersonaPermiteVerRecursosAUsusariosGNOSS(pPersonaID);
        }

        /// <summary>
        /// Obtiene las personas a partir de identidades cargardas en un dataSet.
        /// </summary>
        /// <param name="pDataWrapperIdentidad">DataSet con las identidades de personas</param>
        /// <returns>Dataset de personas</returns>
        public DataWrapperPersona ObtenerPersonasIdentidadesCargadas(DataWrapperIdentidad pDataWrapperIdentidad)
        {
            return PersonaAD.ObtenerPersonasIdentidadesCargadas(pDataWrapperIdentidad);
        }

        /// <summary>
        /// Obtiene las personas de una lista de identidades
        /// </summary>
        /// <param name="pListaIdentidades">Lista de identificadores de identidades</param>
        /// <returns>Dataset de persona</returns>
        public DataWrapperPersona ObtenerPersonasPorIdentidad(List<Guid> pListaIdentidades)
        {
            return this.PersonaAD.ObtenerPersonasPorIdentidad(pListaIdentidades);
        }

        /// <summary>
        /// Obtiene una fila de persona pasando su identidad como parámetro
        /// </summary>
        /// <param name="pIdentidad">Identificador de identidad</param>
        /// <returns>Datarow de la persona</returns>
        public Persona ObtenerPersonaPorIdentidadCargaLigera(Guid pIdentidad)
        {
            if (pIdentidad.Equals(UsuarioAD.Invitado))
            {
                return this.ObtenerPersonaInvitado().ListaPersona.First();
            }
            else
            {
                List<Guid> listaIDs = new List<Guid>();
                listaIDs.Add(pIdentidad);

                var dataWrapperPersona = ObtenerPersonasPorIdentidadesCargaLigera(listaIDs);

                if (dataWrapperPersona.ListaPersona.Count > 0)
                {
                    return dataWrapperPersona.ListaPersona.First();
                }

                return null;
            }
        }

        /// <summary>
        /// Obtiene las personas pasando sus identidades como parámetro
        /// </summary>
        /// <param name="pListaIdentidades">Lista de identificadores de identidad</param>
        /// <returns>Dataset de persona</returns>
        public DataWrapperPersona ObtenerPersonasPorIdentidadesCargaLigera(List<Guid> pListaIdentidades)
        {
            return PersonaAD.ObtenerPersonasPorIdentidadesCargaLigera(pListaIdentidades);
        }

        /// <summary>
        /// Obtiene la persona por la identidad
        /// </summary>
        /// <returns>Dataset de persona</returns>
        public DataWrapperPersona ObtenerPersonaInvitado()
        {
            DataWrapperPersona personaDW = new DataWrapperPersona();

            //Creo la persona
            Persona persona = new Persona();
            persona.PersonaID = UsuarioAD.Invitado;
            persona.Idioma = "es";
            persona.Nombre = "invitado";
            persona.EsBuscable = false;
            persona.Eliminado = false;
            persona.Apellidos = "";
            persona.CoordenadasFoto = "";
            persona.DireccionPersonal = "";
            persona.EsBuscableExternos = false;
            persona.Sexo = "H";
            persona.UsuarioID = UsuarioAD.Invitado;
            persona.EstadoCorreccion = (short)EstadoCorreccion.NoCorreccion;

            //La agrego al dataset
            personaDW.ListaPersona.Add(persona);


            return personaDW;
        }

        /// <summary>
        /// Obtiene la persona por la identidad
        /// </summary>
        /// <param name="pIdentidad">Identificador de identidad</param>
        /// <returns>Dataset de persona</returns>
        public DataWrapperPersona ObtenerPersonasPorIdentidad(Guid pIdentidad)
        {
            if (pIdentidad.Equals(UsuarioAD.Invitado))
            {
                return this.ObtenerPersonaInvitado();
            }
            else
            {

                return this.PersonaAD.ObtenerPersonasPorIdentidad(pIdentidad);
            }
        }

        /// <summary>
        /// Obtiene una persona a partir del identificador de la misma
        /// </summary>
        /// <param name="pListaPersonaID">Lista de identificadores de persona</param>
        /// <returns>Dataset de persona</returns>
        public List<AD.EntityModel.Models.PersonaDS.Persona> ObtenerPersonasPorIDCargaLigera(List<Guid> pListaPersonaID)
        {
            return (this.PersonaAD.ObtenerPersonasPorIDCargaLigera(pListaPersonaID));
        }

        /// <summary>
        /// Obtiene una persona a partir del identificador de la misma
        /// </summary>
        /// <param name="pListaPersonaID">Lista de identificadores de persona</param>
        /// <returns>Dataset de persona</returns>
        public DataWrapperPersona ObtenerPersonasPorID(List<Guid> pListaPersonaID)
        {
            return (this.PersonaAD.ObtenerPersonasPorID(pListaPersonaID));
        }


        /// <summary>
        /// Obtiene una fila con los datos de persona libre
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <returns>Dataset de personas</returns>
        public DatosTrabajoPersonaLibre ObtenerDatosTrabajoPersonaLibre(Guid pUsuarioID)
        {
            return this.PersonaAD.ObtenerDatosTrabajoPersonaLibre(pUsuarioID);
        }

        /// <summary>
        /// Obtiene el email personal de un usuario
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <returns>email de persona</returns>
        public string ObtenerEmailPersonalPorUsuario(Guid pUsuarioID)
        {
            return this.PersonaAD.ObtenerEmailPersonalPorUsuario(pUsuarioID);
        }

        /// <summary>
        /// Obtiene una persona a partir del identificador de la misma
        /// </summary>
        /// <param name="pPersonaID">Identificador de persona</param>
        /// <returns>Dataset de persona</returns>
        public DataWrapperPersona ObtenerPersonaPorID(Guid pPersonaID)
        {
            if (pPersonaID.Equals(UsuarioAD.Invitado))
            {
                return this.ObtenerPersonaInvitado();
            }
            else
            {
                return this.PersonaAD.ObtenerPersonaPorID(pPersonaID);
            }
        }

        /// <summary>
        /// Obtiene (Persona, DatosTrabajoPersonaLibre y TagPersona) de personas 
        /// que están vinculadas en una organización (Sin el campo Foto de la persona)
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de organización</param>
        /// <returns>Dataset de persona</returns>
        public DataWrapperPersona ObtenerPersonasDeOrganizacion(Guid pOrganizacionID)
        {
            List<Guid> lista = new List<Guid>();
            lista.Add(pOrganizacionID);

            return ObtenerPersonasDeListaOrganizaciones(lista);
        }

        /// <summary>
        /// Obtiene (Persona, DatosTrabajoPersonaLibre) de personas 
        /// que están vinculadas en una lista de organizaciones (Sin el campo Foto de la persona)
        /// </summary>
        /// <param name="pListaIDs">Lista de identificadores de organización</param>
        /// <returns>Dataset de persona</returns>
        public DataWrapperPersona ObtenerPersonasDeListaOrganizaciones(List<Guid> pListaIDs)
        {
            return (this.PersonaAD.ObtenerPersonasDeListaOrganizaciones(pListaIDs));
        }

        /// <summary>
        /// Obtiene las personas de una organización. "Persona"
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <returns>Dataset de personas con las personas que participan en organización</returns>
        public DataWrapperPersona ObtenerPersonasDeOrganizacionCargaLigera(Guid pOrganizacionID)
        {
            //ChequeoSeguridad.ComprobarAutorizacion((ulong)Capacidad.General.CapacidadesOrganizacion.VerPersonal);

            return (this.PersonaAD.ObtenerPersonasDeOrganizacionCargaLigera(pOrganizacionID));
        }

        /// <summary>
        /// Obtiene la persona a la que pertenece un perfil
        /// </summary>
        /// <param name="pPerfilID">Guid del perfil</param>
        /// <returns>Dataset con la persona a la que pertenece el perfil</returns>
        public DataWrapperPersona ObtenerPersonaPorPerfil(Guid pPerfilID)
        {
            return this.PersonaAD.ObtenerPersonaPorPerfil(pPerfilID);
        }

        /// <summary>
        /// Obtiene el ID de la persona a la que pertenece un perfil
        /// </summary>
        /// <param name="pPerfilID">Guid del perfil</param>
        /// <returns>Guid de la persona a la que pertenece el perfil</returns>
        public Guid ObtenerPersonaIDPorPerfil(Guid pPerfilID)
        {
            return this.PersonaAD.ObtenerPersonaIDPorPerfil(pPerfilID);
        }

        /// <summary>
        /// Obtiene las personas a la que pertenecen unos perfiles.
        /// </summary>
        /// <param name="pPerfilesID">Guids de los perfiles</param>
        /// <returns>Dataset con las personas a la que pertenecen unos perfiles</returns>
        public DataWrapperPersona ObtenerPersonasPorPerfilesID(List<Guid> pPerfilesID)
        {
            return this.PersonaAD.ObtenerPersonasPorPerfilesID(pPerfilesID);
        }

        /// <summary>
        /// Obtiene las personas pertenecientes a un proyectos, es decir, que están en la estructura orgánica de la organización
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Dataset de personas con las personas del proyecto</returns>
        public List<Persona> ObtenerPersonasDeProyectoCargaLigera(Guid pOrganizacionID, Guid pProyectoID)
        {
            return PersonaAD.ObtenerPersonasDeProyectoCargaLigera(pOrganizacionID, pProyectoID);
        }

        /// <summary>
        /// Obtiene las personas pertenecientes a un grupo de proyecto
        /// </summary>
        /// <param name="pGruposID">Identificador de los grupos</param>
        /// <returns>Dataset de personas con las personas del grupo</returns>
        public List<Persona> ObtenerPersonasDeGruposDeProyectoCargaLigera(List<Guid> pGruposID)
        {
            return PersonaAD.ObtenerPersonasDeGrupoDeProyectoCargaLigera(pGruposID);
        }

        /// <summary>
        /// Obtiene los identificadores de las personas de una comunidad y sus correos electronicos
        /// </summary>
        /// <param name="pNombreCortoComunidad">Nombre corto de la comunidad</param>
        /// <returns>Array de cadenas con el identificador y el correo de la persona separados por una coma</returns>
        public string[] ObtenerPersonaIDEmailPersonasComunidad(string pNombreCortoComunidad)
        {
            return PersonaAD.ObtenerPersonaIDEmailPersonasComunidad(pNombreCortoComunidad);
        }

        /// <summary>
        /// Devuelve si el email pasado como parametro ya pertenece al proyecto
        /// </summary>
        /// <param name="pProyectoID">Comunidad a la que se va a enviar la invitación</param>
        /// <param name="pEmail">Email al que se va a invitar a la comunidad</param>
        /// <returns>True si el correo ya es un miembro de la comunidad, False si aún no lo es.</returns>
        public bool ObtenerSiCorreoPerteneceAProyecto(Guid pProyectoID, string pEmail)
        {
            return PersonaAD.ObtenerSiCorreoPerteneceAProyecto(pProyectoID, pEmail);
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
            return PersonaAD.ObtenerPersonasDeProyecto(pOrganizacionID, pProyectoID);
        }

        /// <summary>
        /// Comprueba si la organización tiene personas vinculadas
        /// </summary>
        /// <param name="pOrganizacion">Identificador de organización para validar</param>
        /// <returns>TRUE si la organización tiene personas, FALSE en caso contrario</returns>
        public bool TienePersonasOrganizacion(Guid pOrganizacion)
        {
            if (this.PersonaAD.TienePersonasOrganizacion(pOrganizacion))
                return true;
            return false;
        }

        /// <summary>
        /// Actualiza el idioma de una persona
        /// </summary>
        /// <param name="pPersonaID">Identificador de la persona</param>
        /// <param name="pIdioma">Idioma</param>
        public void ActualizarIdiomaPersona(Guid pPersonaID, string pIdioma)
        {
            try
            {
                if (Transaccion != null)
                {
                    this.PersonaAD.ActualizarIdiomaPersona(pPersonaID, pIdioma);
                }
                else
                {
                    IniciarTransaccion();
                    {
                        this.PersonaAD.ActualizarIdiomaPersona(pPersonaID, pIdioma);

                        TerminarTransaccion(true);
                    }
                }
            }
            catch (DBConcurrencyException ex)
            {
                TerminarTransaccion(false);
                // Error de concurrencia
                mLoggingService.GuardarLogError(ex, mlogger);
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
        /// Actualiza personas
        /// </summary>
        /// <param name="pPersonaDS">Dataset de personas</param>
        /// <param name="pComprobarAutorizacion">TRUE si se debe de comprobar si el usuario está autorizado</param>
        public void ActualizarPersonas(DataWrapperPersona pDataWrapperPersona = null)
        {
            try
            {

                bool transaccionIniciada = PersonaAD.IniciarTransaccionEntityContext();

                PersonaAD.ActualizarPersonas(pDataWrapperPersona);

                if (transaccionIniciada)
                {
                    TerminarTransaccion(true);
                }


            }
            catch (DBConcurrencyException ex)
            {
                TerminarTransaccion(false);
                // Error de concurrencia
                mLoggingService.GuardarLogError(ex, mlogger);
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
        /// Obtiene todas las personas que solicitan acceso para un proyecto concreto
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Dataset de personas</returns>
        public List<Persona> ObtenerPersonasSolicitanAccesoAProyecto(Guid pOrganizacionID, Guid pProyectoID)
        {
            return PersonaAD.ObtenerPersonasSolicitanAccesoAProyecto(pOrganizacionID, pProyectoID);
        }


        /// <summary>
        /// Obtiene el identificador de la persona a partir del identificador del usuario
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <returns>Identificador de la persona (null si no existe la persona)</returns>
        public Guid? ObtenerPersonaIDPorUsuarioID(Guid pUsuarioID)
        {
            return this.PersonaAD.ObtenerPersonaIDPorUsuarioID(pUsuarioID);
        }

        /// <summary>
        /// Obtenemos todos los datos de la persona a través de su usuario
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <returns>Dataset de personas</returns>
        public DataWrapperPersona ObtenerPersonaPorUsuario(Guid pUsuarioID)
        {
            return this.PersonaAD.ObtenerPersonaPorUsuario(pUsuarioID);
        }

        /// <summary>
        /// Obtiene las personas de unos usuarios
        /// </summary>
        /// <param name="pListaUsuariosID">Lista de identificadores de usuario</param>
        /// <returns>PersonaDS</returns>
        public List<Persona> ObtenerPersonasDeUsuarios(List<Guid> pListaUsuariosID)
        {
            return PersonaAD.ObtenerPersonasDeUsuarios(pListaUsuariosID);
        }

        public void ModificarCorreoPersona(Guid pPersonaID, string pCorreoNuevo)
        {
            PersonaAD.ModificarCorreoPersona(pPersonaID, pCorreoNuevo);
        }

        /// <summary>
        /// Obtiene el nombre de las personas de unos usuarios.
        /// </summary>
        /// <param name="pListaUsuariosID">Lista de identificadores de usuario</param>
        /// <returns>Lista con el ID del usuario y el nombre de la persona</returns>
        public Dictionary<Guid, string> ObtenerNombresPersonasDeUsuariosID(List<Guid> pListaUsuariosID)
        {
            return PersonaAD.ObtenerNombresPersonasDeUsuariosID(pListaUsuariosID);
        }

        /// <summary>
        /// Obtiene el nombre de las personas de unos usuarios.
        /// </summary>
        /// <param name="pListaUsuariosID">Lista de identificadores de usuario</param>
        /// <returns>Lista con el nombrecorto del usuario y el nombre de la persona</returns>
        public Dictionary<string, string> ObtenerNombreCortoYNombresPersonasDeUsuariosID(List<Guid> pListaUsuariosID)
        {
            return PersonaAD.ObtenerNombreCortoYNombresPersonasDeUsuariosID(pListaUsuariosID);
        }

        /// <summary>
        /// Obtiene el nombre de las personas de unos usuarios.
        /// </summary>
        /// <param name="pListaUsuariosID">Lista de identificadores de usuario</param>
        /// <returns>Lista con el nombrecorto del usuario y el nombre de la persona</returns>
        public Dictionary<string, string[]> ObtenerNombreCortoYNombresPerfilPorUsuariosID(List<Guid> pListaUsuariosID)
        {
            return PersonaAD.ObtenerNombreCortoYNombrePerfilPorUsuariosID(pListaUsuariosID);
        }

        /// <summary>
        /// Obtenemos todos los datos de la persona a través de su usuario
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <param name="pObtenerTags">TRUE si se deben de obtener los tags de la persona</param>
        /// <param name="pObtenerDatosTrabajoPersonaLibre">TRUE si se deben de obtener datos de trabajo de persona sin vincular a organización</param>
        /// <returns>Dataset de personas</returns>
        public DataWrapperPersona ObtenerPersonaPorUsuario(Guid pUsuarioID, bool pObtenerTags, bool pObtenerDatosTrabajoPersonaLibre)
        {
            if (pUsuarioID.Equals(UsuarioAD.Invitado))
            {
                return this.ObtenerPersonaInvitado();
            }
            else
            {
                return this.PersonaAD.ObtenerPersonaPorUsuario(pUsuarioID, pObtenerTags, pObtenerDatosTrabajoPersonaLibre);
            }
        }

        /// <summary>
        /// Obtiene la fila de persona a partir de un usuario pasado por parámetro
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        /// <returns>Fila de persona</returns>
        public Persona ObtenerFilaPersonaPorUsuario(Guid pUsuarioID)
        {
            return this.PersonaAD.ObtenerFilaPersonaPorUsuario(pUsuarioID);
        }

        /// <summary>
        /// Obtenemos todas las personas con notificaciones de cambio de identidad
        /// </summary>
        /// <returns>Dataset de personas</returns>
        public List<Persona> ObtenerPersonasConNotificacionDeCorreccionDeIdentidad()
        {
            return this.PersonaAD.ObtenerPersonasConNotificacionDeCorreccionDeIdentidad();
        }

        /// <summary>
        /// Comprueba si el documento acreditativo (DNI, Pasaporte) ya existe en la tabla de personas
        /// </summary>
        /// <param name="textoDoc">Documento acreditativo</param>
        /// <returns>TRUE si existe el documento acreditativo, FALSE en caso contrario</returns>
        public bool ExisteDNI(string pDNI)
        {
            return PersonaAD.ExisteDNI(pDNI);
        }

        /// <summary>
        /// Devuelve los correos de las personas administradoras de un proyecto.
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización a la que pertenece el proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto del que se desean obtener los administradores</param>
        /// <returns>Lista con los correos de los administradores</returns>
        public List<string> ObtenerCorreoDeAdministradoresDeProyecto(Guid pOrganizacionID, Guid pProyectoID)
        {
            return PersonaAD.ObtenerCorreoDeAdministradoresDeProyecto(pOrganizacionID, pProyectoID);
        }

        /// <summary>
        /// Obtiene el email y la identidad correspondientes al perfil pasado por parámetro
        /// </summary>
        /// <param name="pPerfilID">Identificador de perfil</param>
        /// <returns>dataset no tipado</returns>
        public List<PersonaIdentidad> ObtenerEmaileIdentidadATravesDePerfil(Guid pPerfilID)
        {
            return PersonaAD.ObtenerEmaileIdentidadATravesDePerfil(pPerfilID);
        }

        /// <summary>
        /// Obtiene los tags de una persona en un proyecto
        /// </summary>
        /// <param name="pPersonaID">Identificador de la persona</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns></returns>
        public string ObtenerTagsDePersonaEnProyecto(Guid pPersonaID, Guid pProyectoID)
        {
            return PersonaAD.ObtenerTagsDePersonaEnProyecto(pPersonaID, pProyectoID);
        }

        /// <summary>
        /// Actualiza el número de la versión de la foto de la persona
        /// </summary>
        /// <param name="pPersonaID">ID de la Persona</param>
        public void ActualizarVersionFotoPersona(Guid pPersonaID)
        {
            PersonaAD.ActualizarVersionFotoPersona(pPersonaID);
        }

        /// <summary>
        /// Obtiene los IDs de las personas a partir de IDs de usuarios.
        /// </summary>
        /// <param name="pUsuariosID">IDs de usuarios</param>
        /// <returns>Diccionario UsuarioID, PersonaID</returns>
        public Dictionary<Guid, Guid> ObtenerPersonasIDDeUsuariosID(List<Guid> pUsuariosID)
        {
            return PersonaAD.ObtenerPersonasIDDeUsuariosID(pUsuariosID);
        }

        /// <summary>
        /// Obtiene los IDs de los usuarios a partir de IDs de personas.
        /// </summary>
        /// <param name="pPersonasID">IDs de personas</param>
        /// <returns>Diccionario PersonaID, UsuarioID</returns>
        public Dictionary<Guid, Guid> ObtenerUsuariosIDDePersonasID(List<Guid> pPersonasID)
        {
            return PersonaAD.ObtenerUsuariosIDDePersonasID(pPersonasID);
        }

        /// <summary>
        /// Obtiene los IDs de los usuarios y sus DNIs a partir de IDs de personas.
        /// </summary>
        /// <param name="pPersonasID">IDs de personas</param>
        /// <returns>Diccionario PersonaID, UsuarioID, DNI</returns>
        public Dictionary<Guid, KeyValuePair<Guid, string>> ObtenerUsuariosIDYDNIsDePersonasID(List<Guid> pPersonasID)
        {
            return PersonaAD.ObtenerUsuariosIDYDNIsDePersonasID(pPersonasID);
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
                return PersonaAD.ActualizarDatosDesnormalizados;
            }
            set
            {
                this.PersonaAD.ActualizarDatosDesnormalizados = value;
            }
        }

        /// <summary>
        /// Dataadapter de persona
        /// </summary>
        protected PersonaAD PersonaAD
        {
            get
            {
                return (PersonaAD)AD;
            }
            set
            {
                AD = value;
            }
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Determina si está disposed
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Destructor
        /// </summary>
        ~PersonaCN()
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
        protected void Dispose(bool disposing)
        {
            if (!disposed)
            {
                disposed = true;
                if (disposing)
                {
                    //Libero todos los recursos administrados que he añadido a esta clase
                    if (PersonaAD != null)
                        PersonaAD.Dispose();
                }
                PersonaAD = null;
            }
        }

        #endregion
    }
}
